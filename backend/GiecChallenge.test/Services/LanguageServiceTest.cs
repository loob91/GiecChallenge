using Moq;
using GiecChallenge.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GiecChallenge.Models;
using GiecChallenge.Profiles;

namespace GiecChallenge.Tests
{
    public class LanguageServiceTest
    {
        private Mock<ILogger<LanguageService>> _logger = null!;
        private Mock<GiecChallengeContext> _context = null!;
        private LanguageService _service = null!;
        private List<LanguageDto> _allLanguagesDTO = null!;
        private string _language = "FR";

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new LanguageProfile());
            });
            
            IMapper mapper = config.CreateMapper();
            _logger = new Mock<ILogger<LanguageService>>();

            _allLanguagesDTO = Common.GetLanguagesDto("Français", "English");            
            _context = Common.GetContext();            
            _context.Setup(g => g.Languages).Returns(Common.GetMockDbSet<Language>(Common.GetLanguages()).Object);
            _context.Setup(g => g.LanguageLanguages).Returns(Common.GetMockDbSet<LanguageLanguage>(Common.GetLanguages().SelectMany(l => l.names).ToList()).Object);
            
            _service = new LanguageService(_logger.Object, mapper, _context.Object);
        }

        [Test]
        public async Task GetAllLanguages()
        {
            var result = await _service.GetAllLanguages();
            
            Assert.AreEqual(result.Count, Common.GetLanguages().Count);
        }

        [Test]
        [TestCase("Anglais")]
        [TestCase("çais")]
        public async Task GetByName(string name)
        {
            var result = await _service.GetLanguages(name, _language);
            
            Assert.AreEqual(result.Any(re => re.names.Any(rer => rer.name.ToLower().Contains(name.ToLower()))), true);
        }

        [Test]
        [TestCase("pomme")]
        [TestCase("poire")]
        public async Task GetByNameNothing(string name)
        {
            var result = await _service.GetLanguages(name, _language);

            Assert.AreEqual(result.Any(), false);
        }

        [Test]
        [TestCase("f3390acd-acf2-4ab9-8d39-25b216182320")]
        [TestCase("0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        public async Task GetLanguage(Guid id)
        {
            var result = await _service.GetLanguage(id);
            
            Assert.AreEqual(result!.id, id);
        }

        [Test]
        [TestCase("46c619b1-1859-4665-bc8b-cf51eb30777e")]
        [TestCase("eca28033-9954-498f-89d4-a911e40f5a33")]
        public void GetLanguageNothing(Guid id)
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetLanguage(id));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Language does not exist")));
        }

        [Test]
        [TestCase("IT", "Italien", "IT")]
        [TestCase("ES", "Espagnol", "FR")]
        public async Task Create(string isoCode, string name, string languageOfName)
        { 
            var languageDto = Common.GetLanguageDtoTest(isoCode, name, languageOfName);

            await _service.Create(languageDto);
            
            Assert.AreEqual(_context.Object.Languages.Any(l => l.ISOCode == isoCode), true);
            Assert.AreEqual(_context.Object.Languages.Any(l => l.ISOCode == isoCode && l.names.Any()), true);
        }

        [Test]
        [TestCase("FR", "Français", "FR")]
        [TestCase("EN", "English", "EN")]
        public void CreateAlreadyExists(string isoCode, string name, string languageOfName)
        { 
            var languageDto = Common.GetLanguageDtoTest(isoCode, name, languageOfName);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(languageDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Language already exists")));
        }

        [Test]
        [TestCase("f3390acd-acf2-4ab9-8d39-25b216182320", "FR", "Italien", "FR")]
        [TestCase("0b1307be-9ffd-4dcd-9431-4fe58b6420f7", "ES", "Espagnol", "FR")]
        [TestCase("d7ee9249-1edc-4158-ad4d-9892fb703e47", "EN", "German", "EN")]
        public async Task Update(Guid id, string isoCode, string name, string languageOfName)
        { 
            var languageDto = Common.GetLanguageDtoTest(isoCode, name, languageOfName);
            
            await _service.Update(id, languageDto);
            
            Assert.True(_context.Object.Languages.Any(l => l.names.Any(ln => ln.language.ISOCode == languageOfName)));
        }

        [Test]
        [TestCase("46c619b1-1859-4665-bc8b-cf51eb30777e", "FR", "Français")]
        [TestCase("eca28033-9954-498f-89d4-a911e40f5a33", "EN", "English")]
        public void UpdateDoesNotExists(Guid id, string isoCode, string name)
        { 
            var languageDto = Common.GetLanguageDtoTest(isoCode, name);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(id, languageDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Language does not exist")));
        }
    }
}