using Moq;
using Microsoft.EntityFrameworkCore;
using GiecChallenge.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace GiecChallenge.Tests
{
    public static class Common
    {
        public static Mock<GiecChallengeContext> GetContext() {
            var options = new DbContextOptionsBuilder<GiecChallengeContext>()
                            .UseInMemoryDatabase(databaseName: "GiecChallengeContext")
                            .Options;
            
            Mock<GiecChallengeContext> dataContext = new Mock<GiecChallengeContext>(options);

            dataContext.Setup(m => m.AddAsync(It.IsAny<Aliment>(), It.IsAny<CancellationToken>())).Callback<Aliment, CancellationToken>((s, token) =>
            {
                Common.GetAliments().Add(s);
            });
        
            dataContext.Setup(c => c.SaveChangesAsync(default))
            .Returns(Task.FromResult(1))
            .Verifiable();

            return dataContext; 
        }

        public static Mock<DbSet<T>> GetMockDbSet<T>(List<T> objectList) where T : class
        {
            var objectQueryable = objectList.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(default(CancellationToken)))
                .Returns(new AsyncEnumeratorWrapper<T>(objectQueryable.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new AsyncQueryProvider<T>(objectQueryable.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(objectQueryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(objectQueryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => objectQueryable.GetEnumerator());
            mockSet.Setup(m => m.AddAsync(It.IsAny<T>(), default)).Callback<T, CancellationToken>((s, token) => { objectList.Add(s); });
            return mockSet;
        }

        #region Aliments
        public static List<AlimentDto> GetAlimentDto() {
            return new List<AlimentDto>() {
                new AlimentDto() { nom_francais = "tomate", ciqual_code = "1002", impact_environnemental = new ImpactEnvironnementalAlimentDto() { changement_climatique = new ChangementClimatiqueDto() { synthese = 3, unite = "l" }, epuisement_eau = new EpuisementEauDto() { synthese = 15, unite = "kg" }}},
                new AlimentDto() { nom_francais = "poireau", ciqual_code = "1003", impact_environnemental = new ImpactEnvironnementalAlimentDto() { changement_climatique = new ChangementClimatiqueDto() { synthese = 4, unite = "l" }, epuisement_eau = new EpuisementEauDto() { synthese = 0.15, unite = "kg" }}},
                new AlimentDto() { nom_francais = "chou", ciqual_code = "1004", impact_environnemental = new ImpactEnvironnementalAlimentDto() { changement_climatique = new ChangementClimatiqueDto() { synthese = 5, unite = "l" }, epuisement_eau = new EpuisementEauDto() { synthese = 0.015, unite = "kg" }}}
            };
        }

        public static List<Aliment> GetAliments() {
            return new List<Aliment>() {
                new Aliment() { 
                    id = Guid.Parse("aac1d6b7-4ef7-4fff-bd6f-9acce73b671b"),
                    ciqual = "1002",
                    subgroup = GetProductSubGroup().First(),
                    names = new List<ProductLanguage>() {
                        new ProductLanguage() {
                            id = Guid.Parse("5ece3218-cc5d-4a13-bece-2e62b0a6ca06"),
                            language = GetLanguages().First(),
                            name = "tomate"
                        },
                        new ProductLanguage() {
                            id = Guid.Parse("f02f5cf5-3e6d-407f-84cb-68deb7361c54"),
                            language = GetLanguages().First(l => l.ISOCode == "EN"),
                            name = "tomato"
                        }
                    },
                    CO2 = 0.62369712,
                    CO2Unit = "kg CO2 eq/kg de produit",
                    water = 2.3763367,
                    waterUnit = "m3 depriv./kg de produit"
                },
                new Aliment() { 
                    id = Guid.Parse("ba0091db-4ce3-402e-b382-dbd2a8f4bfa4"),
                    ciqual = "1003",
                    subgroup = GetProductSubGroup().First(),
                    names = new List<ProductLanguage>() {
                        new ProductLanguage() {
                            id = Guid.Parse("2e8507f4-213e-482f-b8d0-94647ee8b155"),
                            language = GetLanguages().First(),
                            name = "chou vert"
                        },
                        new ProductLanguage() {
                            id = Guid.Parse("eeb5a793-33b4-4524-b84a-5c71c9a67253"),
                            language = GetLanguages().First(l => l.ISOCode == "EN"),
                            name = "green sprout"
                        }
                    },
                    CO2 = 2.0557422,
                    CO2Unit = "kg CO2 eq/kg de produit",
                    water = 0.58653066,
                    waterUnit = "m3 depriv./kg de produit"
                },
                new Aliment() { 
                    id = Guid.Parse("0a5fae36-10b3-46d2-84af-8ce174ec64f3"),
                    ciqual = "1004",
                    subgroup = GetProductSubGroup().First(),
                    names = new List<ProductLanguage>() {
                        new ProductLanguage() {
                            id = Guid.Parse("60113b02-c446-4254-8614-fde7984fe2d7"),
                            language = GetLanguages().First(),
                            name = "Poireau"
                        },
                        new ProductLanguage() {
                            id = Guid.Parse("bce6224d-efd6-4c9b-97f2-182cb0d91515"),
                            language = GetLanguages().First(l => l.ISOCode == "EN"),
                            name = "Leek"
                        }
                    },
                    CO2 = 0.71433485,
                    CO2Unit = "kg CO2 eq/kg de produit",
                    water = 1.8527791,
                    waterUnit = "m3 depriv./kg de produit"
                }
            };
        }

        public static Aliment GetAlimentTest(string name, string ciqual) {
            return  new Aliment() { 
                id = Guid.NewGuid(),
                ciqual = ciqual,
                names = new List<ProductLanguage>() {
                    new ProductLanguage() {
                        id = Guid.NewGuid(),
                        language = GetLanguages().First(),
                        name = name
                    },
                    new ProductLanguage() {
                        id = Guid.NewGuid(),
                        language = GetLanguages().First(l => l.ISOCode == "EN"),
                        name = name
                    }
                },
                CO2 = 0.62369712,
                CO2Unit = "kg CO2 eq/kg de produit",
                water = 2.3763367,
                waterUnit = "m3 depriv./kg de produit"
            };
        }

        public static AlimentDto GetAlimentDtoTest(string name, string ciqual) {
            return new AlimentDto() { 
                nom_francais = name, 
                ciqual_code = ciqual, 
                groupe = GetProductSubGroup().First().names.First().name,
                impact_environnemental = new ImpactEnvironnementalAlimentDto() { 
                    changement_climatique = new ChangementClimatiqueDto() { 
                        synthese = 5, 
                        unite = "l" 
                    }, 
                    epuisement_eau = new EpuisementEauDto() { 
                        synthese = 0.015, 
                        unite = "kg" 
                    }
                }
            };
        }
        #endregion

        #region Products
        public static List<ProductDto> GetProductsDto() {
            return new List<ProductDto>() {
                new ProductDto() {     
                    names = new List<ProductNamesDto>() {
                        new ProductNamesDto() {
                            name = "iPad",
                            language = "EN"
                        },
                        new ProductNamesDto() {
                            name = "iPad",
                            language = "FR"
                        }
                    },
                    language = "FR",
                    group = "Tablette",
                    CO2 = 52,
                    CO2Unit = "kg CO2 eq/kg de produit",
                    water = 22.3763367,
                    waterUnit = "m3 depriv./kg de produit",
                    amortization = 48
                },
                new ProductDto() {     
                    names = new List<ProductNamesDto>() {
                        new ProductNamesDto() {
                            name = "Pen",
                            language = "EN"
                        },
                        new ProductNamesDto() {
                            name = "Stylo",
                            language = "FR"
                        }
                    },
                    language = "FR",
                    group = "Papeterie",
                    CO2 = 0.005,
                    CO2Unit = "kg CO2 eq/kg de produit",
                    water = 0.25,
                    waterUnit = "m3 depriv./kg de produit",
                    amortization = 0
                },
                new ProductDto() {     
                    names = new List<ProductNamesDto>() {
                        new ProductNamesDto() {
                            name = "Gas car",
                            language = "EN"
                        },
                        new ProductNamesDto() {
                            name = "Voiture thermique",
                            language = "FR"
                        }
                    },
                    language = "FR",
                    group = "Locomotion",
                    CO2 = 520000,
                    CO2Unit = "kg CO2 eq/kg de produit",
                    water = 16161.548,
                    waterUnit = "m3 depriv./kg de produit",
                    amortization = 180
                },
            };
        }

        public static List<Product> GetProducts() {
            return new List<Product>() {
                new Product() { 
                    id = Guid.Parse("e5f89b1d-171f-4460-a2cc-18e1534b5bae"),
                    subgroup = GetProductSubGroup().Last(),
                    names = new List<ProductLanguage>() {
                        new ProductLanguage() {
                            id = Guid.Parse("6380098e-fc8e-455a-94df-2635e4245d25"),
                            language = GetLanguages().First(),
                            name = "iPad"
                        },
                        new ProductLanguage() {
                            id = Guid.Parse("aa930251-be87-4d56-b132-f7d3b7c1b1a6"),
                            language = GetLanguages().First(l => l.ISOCode == "EN"),
                            name = "iPad"
                        }
                    },
                    amortization = 48,
                    CO2 = 0.62369712,
                    CO2Unit = "kg CO2 eq/kg de produit",
                    water = 2.3763367,
                    waterUnit = "m3 depriv./kg de produit"
                },
                new Product() { 
                    id = Guid.Parse("526ea756-50da-486f-8a44-5e964f249c1e"),
                    subgroup = GetProductSubGroup().First(),
                    names = new List<ProductLanguage>() {
                        new ProductLanguage() {
                            id = Guid.Parse("0da29478-8b97-4224-b99b-e3bb8895301d"),
                            language = GetLanguages().First(),
                            name = "chou vert"
                        },
                        new ProductLanguage() {
                            id = Guid.Parse("9ff4dab7-8fe6-4147-9edf-e8bda65a5f9d"),
                            language = GetLanguages().First(l => l.ISOCode == "EN"),
                            name = "green sprout"
                        }
                    },
                    CO2 = 2.0557422,
                    CO2Unit = "kg CO2 eq/kg de produit",
                    water = 0.58653066,
                    waterUnit = "m3 depriv./kg de produit"
                },
                new Product() { 
                    id = Guid.Parse("fb3ee25c-b3a4-4b80-a14e-9a1a8093554d"),
                    subgroup = GetProductSubGroup().First(),
                    names = new List<ProductLanguage>() {
                        new ProductLanguage() {
                            id = Guid.Parse("41049c06-f3e1-4744-8958-dc3f9b1c172d"),
                            language = GetLanguages().First(),
                            name = "Stylo"
                        },
                        new ProductLanguage() {
                            id = Guid.Parse("f46b47bb-bc9a-4563-82ae-40a73777464d"),
                            language = GetLanguages().First(l => l.ISOCode == "EN"),
                            name = "Pen"
                        }
                    },
                    CO2 = 0.71433485,
                    CO2Unit = "kg CO2 eq/kg de produit",
                    water = 1.8527791,
                    waterUnit = "m3 depriv./kg de produit"
                }
            };
        }

        public static Product GetProductTest(string name, Guid id) {
            return  new Product() { 
                id = Guid.NewGuid(),
                names = new List<ProductLanguage>() {
                    new ProductLanguage() {
                        id = Guid.Parse("fe7ebeb5-11b6-4c4f-8ab3-e02e663f553a"),
                        language = GetLanguages().First(),
                        name = name
                    },
                    new ProductLanguage() {
                        id = Guid.Parse("2185d186-1676-42ca-a444-f05884b5d4fd"),
                        language = GetLanguages().First(l => l.ISOCode == "EN"),
                        name = name
                    }
                },
                CO2 = 0.62369712,
                CO2Unit = "kg CO2 eq/kg de produit",
                water = 2.3763367,
                waterUnit = "m3 depriv./kg de produit"
            };
        }

        public static ProductDto GetProductDtoTest(string name, string group, string language) {
            return new ProductDto() { 
                names = new List<ProductNamesDto>() {
                    new ProductNamesDto() {
                        name = name,
                        language = language
                    },
                    new ProductNamesDto() {
                        name = name,
                        language = "f3390acd-acf2-4ab9-8d39-25b216182320"
                    }
                },
                language = language,
                group = group,
                CO2 = 31,
                CO2Unit = "kg CO2 eq/kg de produit",
                water = 2.3763367,
                waterUnit = "m3 depriv./kg de produit",
                amortization = 48
            };
        }

        public static ProductUserTranslationDTO GetProductTranslationDtoTest(string name, string user, string product, string id) {
            return new ProductUserTranslationDTO() { 
                user = user,
                product = product,
                name = name,
                id = id
            };
        }

        public static List<ProductUserTranslation> GetProductUserTranslations() {
            return new List<ProductUserTranslation>() {
                 new ProductUserTranslation() { 
                    id = Guid.Parse("479d91d4-8f93-433f-8b4c-b5b08c12db5c"), 
                    user = GetUsers().First(),
                    product = GetProducts().First(),
                    name = "tomate de saison"
                },
                new ProductUserTranslation() { 
                    id = Guid.Parse("5823ec98-2726-4b39-b01e-8453bbde5524"), 
                    user = GetUsers().First(u => u.email == "toto1@toto.com"),
                    product = GetProducts().First(p => p.id == Guid.Parse("526ea756-50da-486f-8a44-5e964f249c1e")),
                    name = "chou de saison"
                }
            };
        }
        #endregion

        #region Language
        public static List<Language> GetLanguages() {
            var languages = new List<Language>() {
                new Language() { 
                    id = Guid.Parse("f3390acd-acf2-4ab9-8d39-25b216182320"), 
                    ISOCode = "FR"
                },
                 new Language() { 
                    id = Guid.Parse("0b1307be-9ffd-4dcd-9431-4fe58b6420f7"), 
                    ISOCode = "EN"
                },
                 new Language() { 
                    id = Guid.Parse("d7ee9249-1edc-4158-ad4d-9892fb703e47"), 
                    ISOCode = "DE"
                }
            };

            languages.First().names = new List<LanguageLanguage>() {
                new LanguageLanguage() {
                    id = Guid.Parse("aab43c46-b2c9-4b7f-bc1f-9b93690b7ffb"),
                    language = languages.First(),
                    languageToChange = languages.First(),
                    name = "Français"
                },
                new LanguageLanguage() {
                    id = Guid.Parse("c5ef4000-6bf6-4206-af78-edecd622b43b"),
                    language = languages.First(l => l.ISOCode == "EN"),
                    languageToChange = languages.First(),
                    name = "French"
                }
            };
            

            languages.First(l => l.ISOCode == "EN").names = new List<LanguageLanguage>() {
                new LanguageLanguage() {
                    id = Guid.Parse("f28163fd-ffc6-4da9-beed-2367ee4e2c9e"),
                    language = languages.First(),
                    languageToChange = languages.First(l => l.ISOCode == "EN"),
                    name = "Anglais"
                },
                new LanguageLanguage() {
                    id = Guid.Parse("190f992f-b2bc-44a2-a062-0129088d557a"),
                    language = languages.First(l => l.ISOCode == "EN"),
                    languageToChange = languages.First(l => l.ISOCode == "EN"),
                    name = "English"
                }
            };

            languages.Last().names = new List<LanguageLanguage>() {
                new LanguageLanguage() {
                    id = Guid.Parse("d8ea2692-dbf4-467e-b767-b0187f333775"),
                    language = languages.First(),
                    languageToChange = languages.Last(),
                    name = "Allemand"
                }
            };

            return languages;
        }

        public static List<LanguageDto> GetLanguagesDto(string nameFR, string nameEN) {
            return new List<LanguageDto>() {
                 new LanguageDto() { 
                    ISOCode = "FR",
                    names = new List<LanguageNamesDto>() {
                        new LanguageNamesDto() {
                            name = nameFR,
                            language = "EN"
                        },
                        new LanguageNamesDto() {
                            name = nameFR,
                            language = "FR"
                        }
                    }
                },
                new LanguageDto() { 
                    ISOCode = "EN",
                    names = new List<LanguageNamesDto>() {
                        new LanguageNamesDto() {
                            name = nameEN,
                            language = "EN"
                        },
                        new LanguageNamesDto() {
                            name = nameEN,
                            language = "FR"
                        }
                    }
                }
            };
        }

        public static LanguageDto GetLanguageDtoTest(string ISOCode, string name, string language = "FR") {
            return new LanguageDto() { 
                ISOCode = ISOCode,
                names = new List<LanguageNamesDto>() {
                    new LanguageNamesDto() {
                        name = name,
                        language = language
                    }
                }
            };
        }
        #endregion

        #region Group
        public static List<ProductGroup> GetProductGroup() {
            return new List<ProductGroup>() {
                new ProductGroup() { 
                    id = Guid.Parse("991979cd-b95f-4e9a-85e7-e1f7ce6932fb"), 
                    names = new List<ProductGroupLanguage>() { 
                        new ProductGroupLanguage() { 
                            id = Guid.Parse("6d5b7831-c61d-4923-820d-9bde93dd2723"), 
                            language = GetLanguages().First(l => l.ISOCode == "FR"),
                            name = "Aliment"
                        },
                        new ProductGroupLanguage() { 
                            id = Guid.Parse("092f0c65-9bdd-46a3-81f0-2c521b34596e"), 
                            language = GetLanguages().First(l => l.ISOCode == "EN"),
                            name = "Food"
                        }
                    }
                },
                new ProductGroup() { 
                    id = Guid.Parse("3a69d206-7236-11ed-a1eb-0242ac120002"), 
                    names = new List<ProductGroupLanguage>() { 
                        new ProductGroupLanguage() { 
                            id = Guid.Parse("3a69d4c2-7236-11ed-a1eb-0242ac120002"), 
                            language = GetLanguages().First(l => l.ISOCode == "FR"),
                            name = "Éléctronique"
                        },
                        new ProductGroupLanguage() { 
                            id = Guid.Parse("3a69d8b4-7236-11ed-a1eb-0242ac120002"), 
                            language = GetLanguages().First(l => l.ISOCode == "EN"),
                            name = "Electronic devices"
                        }
                    }
                }
            };
        }

        public static List<ProductGroup> GetGroups() {
            return new List<ProductGroup>() {
                new ProductGroup() { 
                    id = Guid.Parse("b21a6403-f428-454f-942d-dbd1fc3fa551"), 
                    names = new List<ProductGroupLanguage>() {
                        GetGroupLanguage().First(),
                        GetGroupLanguage().First(gpl => gpl.name == "Food"),
                    }
                },
                new ProductGroup() { 
                    id = Guid.Parse("8f46bf6f-6cbf-47ac-8d51-039eabc820c3"), 
                    names = new List<ProductGroupLanguage>() {
                        GetGroupLanguage().Last()
                    } 
                }
            };
        }

        public static List<ProductGroupLanguage> GetGroupLanguage() {
            return new List<ProductGroupLanguage>() {
                new ProductGroupLanguage() {
                    id = Guid.Parse("fae75424-d098-43fd-97ca-296da57501c2"),
                    language = GetLanguages().First(),
                    name = "Aliment"
                },
                new ProductGroupLanguage() {
                    id = Guid.Parse("fc394591-5b8b-4493-9d40-fc6a1a25b0c7"),
                    language = GetLanguages().First(l => l.ISOCode == "EN"),
                    name = "Food"
                },
                new ProductGroupLanguage() {
                    id = Guid.Parse("b615e2d8-ebc1-4b2b-a2fd-0bab9449278e"),
                    language = GetLanguages().First(l => l.ISOCode == "EN"),
                    name = "Electronic device"
                }
            };
        }

        public static List<GroupDto> GetGroupsDto(string nameFR, string nameEN) {
            return new List<GroupDto>() {
                 new GroupDto() { 
                    id = Guid.Parse("b21a6403-f428-454f-942d-dbd1fc3fa551"),
                    names = new List<GroupNamesDto>() {
                        new GroupNamesDto() {
                            name = nameFR,
                            language = "EN"
                        },
                        new GroupNamesDto() {
                            name = nameFR,
                            language = "FR"
                        }
                    }
                },
                new GroupDto() { 
                    id = Guid.Parse("8f46bf6f-6cbf-47ac-8d51-039eabc820c3"),
                    names = new List<GroupNamesDto>() {
                        new GroupNamesDto() {
                            name = nameEN,
                            language = "EN"
                        },
                        new GroupNamesDto() {
                            name = nameEN,
                            language = "FR"
                        }
                    }
                }
            };
        }

        public static GroupDto GetGroupDtoTest(string name, string language = "FR") {
            return new GroupDto() { 
                names = new List<GroupNamesDto>() {
                    new GroupNamesDto() {
                        name = name,
                        language = language
                    }
                }
            };
        }
        #endregion

        #region SubGroup

        public static List<ProductSubGroup> GetProductSubGroup() {
            return new List<ProductSubGroup>() {
                new ProductSubGroup() { 
                    id = Guid.Parse("4f52f771-7752-472f-921e-88824fc4c5d5"), 
                    names = new List<ProductSubGroupLanguage>() { 
                        new ProductSubGroupLanguage() { 
                            id = Guid.Parse("9a43209f-9a7e-4890-94fc-3c8d8d26a614"), 
                            language = GetLanguages().First(l => l.ISOCode == "FR"),
                            name = "Boisson"
                        },
                        new ProductSubGroupLanguage() { 
                            id = Guid.Parse("698c1cd6-66c9-45b8-95e7-da65da4772c0"), 
                            language = GetLanguages().First(l => l.ISOCode == "EN"),
                            name = "Beverage"
                        }
                    },
                    Groupe = GetProductGroup().First()
                },
                new ProductSubGroup() { 
                    id = Guid.Parse("04f3eb50-6119-487a-86a6-b6c24e620536"), 
                    names = new List<ProductSubGroupLanguage>() { 
                        new ProductSubGroupLanguage() { 
                            id = Guid.Parse("f2a3ed11-7205-46e8-b1b2-911db83bbe90"), 
                            language = GetLanguages().First(l => l.ISOCode == "FR"),
                            name = "Téléphone portable"
                        },
                        new ProductSubGroupLanguage() { 
                            id = Guid.Parse("300416ca-9a76-4f1b-9b98-7742b0f93098"), 
                            language = GetLanguages().First(l => l.ISOCode == "EN"),
                            name = "Smartphone"
                        }
                    },
                    Groupe = GetProductGroup().Last()
                }
            };
        }

        public static List<ProductSubGroup> GetSubGroups() {
            return new List<ProductSubGroup>() {
                new ProductSubGroup() { 
                    id = Guid.Parse("bf0cc5d3-2b4f-4761-ac4f-5dc25005aa32"), 
                    names = new List<ProductSubGroupLanguage>() {
                        GetSubGroupLanguage().First(),
                        GetSubGroupLanguage().First(sgl => sgl.id == Guid.Parse("a4b8abef-fd9d-4695-b8d6-8bbc005bf596"))
                    },
                    Groupe = GetGroups().First()
                },
                new ProductSubGroup() { 
                    id = Guid.Parse("1dda078c-d158-4078-aa8e-981d5ac5cd57"), 
                    names = new List<ProductSubGroupLanguage>() {
                        GetSubGroupLanguage().Last()
                    } ,
                    Groupe = GetGroups().Last()
                }
            };
        }

        public static List<ProductSubGroupLanguage> GetSubGroupLanguage() {
            return new List<ProductSubGroupLanguage>() {
                new ProductSubGroupLanguage() {
                    id = Guid.Parse("b713e22b-a4c1-4b57-8135-9f3ab0c7b760"),
                    language = GetLanguages().First(),
                    name = "Boisson"
                },
                new ProductSubGroupLanguage() {
                    id = Guid.Parse("a4b8abef-fd9d-4695-b8d6-8bbc005bf596"),
                    language = GetLanguages().First(l => l.ISOCode == "EN"),
                    name = "Drink"
                },
                new ProductSubGroupLanguage() {
                    id = Guid.Parse("bf5f66ea-9cca-41ee-b4be-1d9a9f6a450c"),
                    language = GetLanguages().First(l => l.ISOCode == "EN"),
                    name = "Smartphone"
                }
            };
        }

        public static SubGroupDto GetSubGroupDtoTest(string name, string groupName, string language = "FR") {
            return new SubGroupDto() { 
                names = new List<SubGroupNamesDto>() {
                    new SubGroupNamesDto() {
                        name = name,
                        language = language
                    }
                },
                language = language,
                group = groupName
            };
        }
        #endregion

        #region Users
        public static UserDto GetUserDto(string email, string password, string language) {
            return new UserDto() {
                email = email,
                password = password,
                language = language
            };
        }

        public static List<User> GetUsers() {
            return new List<User>() {
                 new User() { 
                    id = Guid.Parse("0a891394-be17-473b-9924-eccaf6ce79ed"), 
                    email = "toto@toto.com",
                    password = EncryptPassword("password1", Encoding.ASCII.GetBytes("hYHDi5p23oOzZQVSyMUAanvYFtBhpFgYPwpkfs5iVEs=")),
                    hash = Encoding.ASCII.GetBytes("hYHDi5p23oOzZQVSyMUAanvYFtBhpFgYPwpkfs5iVEs="),
                    creationDate = DateTime.Now,
                    favoriteLanguage = GetLanguages().First()
                },
                new User() { 
                    id = Guid.Parse("9beb47ab-0def-437c-b510-02d8f9623ebb"), 
                    email = "toto1@toto.com",
                    password = EncryptPassword("password2", Encoding.ASCII.GetBytes("NBLjPomVm3HeIRetpgDzavp3axIKNJQKa85XY8KJ1zY=")),
                    hash = Encoding.ASCII.GetBytes("NBLjPomVm3HeIRetpgDzavp3axIKNJQKa85XY8KJ1zY="),
                    creationDate = DateTime.Now,
                    favoriteLanguage = GetLanguages().First(l => l.ISOCode == "EN")
                }
            };
        }

        public static List<UserGroup> GetUserGroups() {
            return new List<UserGroup>() {
                 new UserGroup() { 
                    id = Guid.Parse("eb3641a9-84c3-4d38-b350-925f58b04506"), 
                    name = "Classic"
                },
                new UserGroup() { 
                    id = Guid.Parse("bebf7ca6-27a7-47de-affe-ca7f5536ad96"), 
                    name = "Admin"
                }
            };
        }
        #endregion

        #region Purchase
        public static PurchaseDto GetPurchaseDto(DateTime date, List<string> products, List<string> currencies, List<double> prices, List<double> quantities) {
            return new PurchaseDto() {
                datePurchase = date,
                products = GetProductPurchaseDto(products, currencies, prices, quantities)
            };
        }

        public static List<ProductPurchaseDto> GetProductPurchaseDto(List<string> products, List<string> currencies, List<double> prices, List<double> quantities) {
            List<ProductPurchaseDto> purchases = new List<ProductPurchaseDto>();
            if (products.Count != currencies.Count && currencies.Count != prices.Count)
                throw new Exception("Tab must have the same length");
            int i = 0;
            foreach(string product in products) {
                purchases.Add(new ProductPurchaseDto() {
                    product = product,
                    currencyIsoCode = currencies[i],
                    price = prices[i],
                    quantity = quantities[i]
                });
                i++;
            }
            return purchases;
        }

        public static List<CarbonLoan> GetCarbonLoans() {
            return new List<CarbonLoan>() {
                 new CarbonLoan() { 
                    id = Guid.Parse("d7205c76-a159-11ed-a8fc-0242ac120002"), 
                    user = GetUsers().First(),
                    productPurchase = GetProductPurchases().First(),
                    dateBegin = GetProductPurchases().First().purchase.datePurchase,
                    dateEnd = GetProductPurchases().First().purchase.datePurchase.AddMonths(GetProductPurchases().First().product.amortization)
                }
            };
        }

        public static List<Purchase> GetPurchases() {
            return new List<Purchase>() {
                 new Purchase() { 
                    id = Guid.Parse("e2075166-6f2c-4172-8906-2f100a6a1456"), 
                    datePurchase = DateTime.Parse("03/01/2022"),
                    user = GetUsers().First(),
                    products = new List<ProductPurchase>() {
                        GetProductPurchases().First(pp => pp.id == Guid.Parse("788b7e57-9b59-43ce-af8e-2d097551e442"))
                    }
                },
                new Purchase() { 
                    id = Guid.Parse("b516e589-a039-4ce0-b6be-75bfc08cf6d3"), 
                    datePurchase = DateTime.Parse("01/01/2022"),
                    user = GetUsers().First(),
                    products = new List<ProductPurchase>() {
                        GetProductPurchases().First(pp => pp.id == Guid.Parse("869a7030-f400-4b1b-aa12-e9245f791d0a")),
                        GetProductPurchases().First(pp => pp.id == Guid.Parse("32ed916c-6161-4565-8c8a-40a3e262bd40"))
                    }
                },
                 new Purchase() { 
                    id = Guid.Parse("51e6eec0-d9fd-47fc-830a-88d1e6638b88"), 
                    datePurchase = DateTime.Parse("03/01/2022"),
                    user = GetUsers().Last(),
                    products = new List<ProductPurchase>() {
                        GetProductPurchases().First(pp => pp.id == Guid.Parse("bb4179dc-c18e-4576-b9f6-12e8c9f6456f"))
                    }
                },
            };
        }

        public static List<ProductPurchase> GetProductPurchases() {
            return new List<ProductPurchase>() {
                new ProductPurchase() {
                    id = Guid.Parse("788b7e57-9b59-43ce-af8e-2d097551e442"),
                    product = GetProducts().First(),
                    currency = GetCurrencies().First(),
                    price = 10
                },
                new ProductPurchase() {
                    id = Guid.Parse("869a7030-f400-4b1b-aa12-e9245f791d0a"),
                    product = GetProducts().First(),
                    currency = GetCurrencies().First(),
                    price = 10
                },
                new ProductPurchase() {
                    id = Guid.Parse("32ed916c-6161-4565-8c8a-40a3e262bd40"),
                    product = GetProducts().Last(),
                    currency = GetCurrencies().First(),
                    price = 15
                },
                new ProductPurchase() {
                    id = Guid.Parse("bb4179dc-c18e-4576-b9f6-12e8c9f6456f"),
                    product = GetProducts().First(),
                    currency = GetCurrencies().Last(),
                    price = 100
                }
            };
        }

        public static List<Currency> GetCurrencies() {
            return new List<Currency>() {
                new Currency() {
                    id = Guid.Parse("1a7d6616-dfd1-47c8-ba42-2b12e71c43af"),
                    ISOCode = "USD"
                },
                new Currency() {
                    id = Guid.Parse("a408c030-1b87-41bc-ad63-378e93f3780f"),
                    ISOCode = "EUR"
                },
                new Currency() {
                    id = Guid.Parse("2b4d7ecd-6ec3-439c-8984-e657a9bcc9c2"),
                    ISOCode = "CHF"
                }
            };
        }
        public static PurchaseLaRucheDto GetPurchaseLaRucheDto(DateTime date, string command) {
            return new PurchaseLaRucheDto() {
                datePurchase = date,
                command = command
            };
        }
        #endregion

        public static string EncryptPassword(string password, byte[] salt)
        {
            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            return encryptedPassw;
        }

        public static JwtSecurityToken GetToken(string idUser, string secret, string validIssuer, string validAudience)
        {         
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, idUser),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            authClaims.Add(new Claim(ClaimTypes.Role, "Admin"));
            authClaims.Add(new Claim(ClaimTypes.Role, "Classic"));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var token = new JwtSecurityToken(
                issuer: validIssuer,
                audience: validAudience,
                expires: DateTime.Now.AddDays(5),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}