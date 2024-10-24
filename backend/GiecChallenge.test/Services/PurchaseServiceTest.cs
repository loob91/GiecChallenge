using Moq;
using GiecChallenge.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GiecChallenge.Models;
using GiecChallenge.Profiles;

namespace GiecChallenge.Tests
{
    public class PurchaseServiceTest
    {
        private Mock<ILogger<PurchaseService>> _logger = null!;
        private Mock<GiecChallengeContext> _context = null!;
        private PurchaseService _service = null!;
        private List<UserDto> _allUsersDTO = null!;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new PurchaseProfile());
            });
            
            IMapper mapper = config.CreateMapper();
            _logger = new Mock<ILogger<PurchaseService>>();
            _context = Common.GetContext();
            _context.Setup(x => x.SetEntityStateModified(It.IsAny<Purchase>()));
            _context.Setup(g => g.Purchases).Returns(Common.GetMockDbSet<Purchase>(Common.GetPurchases()).Object);
            _context.Setup(g => g.Products).Returns(Common.GetMockDbSet<Product>(Common.GetProducts()).Object);
            _context.Setup(g => g.ProductPurchases).Returns(Common.GetMockDbSet<ProductPurchase>(Common.GetProductPurchases()).Object);
            _context.Setup(g => g.ProductUserTranslations).Returns(Common.GetMockDbSet<ProductUserTranslation>(Common.GetProductUserTranslations()).Object);
            _context.Setup(g => g.Users).Returns(Common.GetMockDbSet<User>(Common.GetUsers()).Object);
            _context.Setup(g => g.Currencies).Returns(Common.GetMockDbSet<Currency>(Common.GetCurrencies()).Object);
            _context.Setup(g => g.CarbonLoans).Returns(Common.GetMockDbSet<CarbonLoan>(Common.GetCarbonLoans()).Object);
            
            _service = new PurchaseService(_logger.Object, mapper, _context.Object);
        }

        [Test]
        [TestCase("0a891394-be17-473b-9924-eccaf6ce79ed")]
        [TestCase("9beb47ab-0def-437c-b510-02d8f9623ebb")]
        public async Task GetAll(string idUser)
        {
            var result = await _service.GetAll(Guid.Parse(idUser));
            
            Assert.AreEqual(_context.Object.Purchases.Count(u => u.user.id == Guid.Parse(idUser)), result.Count());
        }

        [Test]
        [TestCase("9a3f9eb6-1f46-46b5-aa66-b0e3d0b37c82")]
        [TestCase("d89b6c34-ae14-43f6-b5e0-26f6265a9bd2")]
        public void GetAllUserNotCorrect(string idUser)
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetAll(Guid.Parse(idUser)));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Nice try")));
        }

        [Test]
        [TestCase("0a891394-be17-473b-9924-eccaf6ce79ed", "30/11/2021", "05/01/2022", 2)]
        [TestCase("0a891394-be17-473b-9924-eccaf6ce79ed", "30/11/2021", "02/01/2022", 1)]
        [TestCase("9beb47ab-0def-437c-b510-02d8f9623ebb", "30/11/2021", "05/01/2022", 1)]
        [TestCase("9beb47ab-0def-437c-b510-02d8f9623ebb", "05/11/2022", "06/11/2022", 0)]
        public async Task GetByDate(string idUser, string dateBegin, string dateEnd, int expectedResult)
        {
            var result = await _service.GetBetweenDate(Guid.Parse(idUser), DateTime.Parse(dateBegin), DateTime.Parse(dateEnd));
            
            Assert.AreEqual(expectedResult, result.Count());
        }

        [Test]
        [TestCase("9a3f9eb6-1f46-46b5-aa66-b0e3d0b37c82", "30/11/2021", "05/01/2022", 2)]
        [TestCase("d89b6c34-ae14-43f6-b5e0-26f6265a9bd2", "30/11/2021", "05/01/2022", 1)]
        public void GetUserNotCorrect(string idUser, string dateBegin, string dateEnd, int expectedResult)
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetBetweenDate(Guid.Parse(idUser), DateTime.Parse(dateBegin), DateTime.Parse(dateEnd)));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Nice try")));
        }

        [Test]
        [TestCase("05/11/2023", 
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.0, 12.2}, 
                  new[] {1.2, 2.3})]
        [TestCase("07/12/2023", 
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.0, 12.2}, 
                  new[] {3, 4.3})]
        public async Task Create(DateTime date, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            await _service.Create(Guid.Parse(idUser), purchaseToTest);
            
            var purchase = _context.Object.Purchases.First(u => u.datePurchase == date);
            foreach (string product in products) {
                Assert.IsTrue(purchase.products.Any(p => p.product.id == Guid.Parse(product)));
            }
        }

        [Test]
        [TestCase("01/01/2023", 
                  "9a3f9eb6-1f46-46b5-aa66-b0e3d0b37c82",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.2, 12.2}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "d89b6c34-ae14-43f6-b5e0-26f6265a9bd2",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.2, 12.0}, 
                  new[] {1.2, 2.3})]
        public void CreateUserNotExists(DateTime date, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(Guid.Parse(idUser), purchaseToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Nice try")));
        }

        [Test]
        [TestCase("01/01/2023", 
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "5a39736d-de7b-4b26-b4c6-b5841a52ddbf"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.2, 12.2}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new[] {"4ec6db81-f8e0-4343-9973-f6a91e4d4c29", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.2, 12.2}, 
                  new[] {1.2, 2.3})]
        public void CreateProductNotExists(DateTime date, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(Guid.Parse(idUser), purchaseToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Product does not exist")));
        }

        [Test]
        [TestCase("01/01/2023", 
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"f150f3ca-1a18-40e1-a01a-fad22724514a", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.2, 12.2}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "f2672cf5-761e-4f5a-8fc2-b3264fc5dd0d"}, 
                  new[] {15.2, 12.2}, 
                  new[] {1.2, 2.3})]
        public void CreateCurrencyNotExists(DateTime date, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(Guid.Parse(idUser), purchaseToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Currency does not exist")));
        }

        [Test]
        [TestCase("01/01/2023", 
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {-5, 1.22}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {1.25, -1}, 
                  new[] {1.2, 2.3})]
        public void CreatePriceNotCorrect(DateTime date, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(Guid.Parse(idUser), purchaseToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Price must be superior than 0")));
        }

        [Test]
        [TestCase("01/01/2023", 
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new string[] {}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {-5, 1.22}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new string[] {}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {1.25, -1}, 
                  new[] {1.2, 2.3})]
        public void CreateNoProduct(DateTime date, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(Guid.Parse(idUser), purchaseToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("No product selected")));
        }

        [Test]
        [TestCase("01/01/2023", 
                  "e2075166-6f2c-4172-8906-2f100a6a1456",
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.0, 12.2}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "51e6eec0-d9fd-47fc-830a-88d1e6638b88",
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.0, 12.2}, 
                  new[] {3, 4.51})]
        [TestCase("01/01/2023", 
                  "51e6eec0-d9fd-47fc-830a-88d1e6638b88",
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.0, 12.2}, 
                  new[] {5.6, 6.7})]
        public async Task Update(DateTime date, string idPurchase, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            purchaseToTest.id = Guid.Parse(idPurchase);
            await _service.Update(Guid.Parse(idUser), purchaseToTest);
            
            var purchase = _context.Object.Purchases.First(p => p.id == Guid.Parse(idPurchase));
            foreach (string product in products) {
                Assert.IsTrue(purchase.products.Any(p => p.product.id == Guid.Parse(product)));
            }
        }

        [Test]
        [TestCase("01/01/2023", 
                  "e2075166-6f2c-4172-8906-2f100a6a1456",
                  "9a3f9eb6-1f46-46b5-aa66-b0e3d0b37c82",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.2, 12.2}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "51e6eec0-d9fd-47fc-830a-88d1e6638b88",
                  "d89b6c34-ae14-43f6-b5e0-26f6265a9bd2",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.2, 12.0}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "e2075166-6f2c-4172-8906-2f100a6a1456",
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new string[] {}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {-5, 1.22}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "51e6eec0-d9fd-47fc-830a-88d1e6638b88",
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new string[] {}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {1.25, -1}, 
                  new[] {1.2, 2.3})]
        public void UpdateUserNotExists(DateTime date, string idPurchase, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            purchaseToTest.id = Guid.Parse(idPurchase);
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(Guid.Parse(idUser), purchaseToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Purchase does not exist")));
        }

        [Test]
        [TestCase("01/01/2023", 
                  "e2075166-6f2c-4172-8906-2f100a6a1456",
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "5a39736d-de7b-4b26-b4c6-b5841a52ddbf"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.2, 12.2}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "51e6eec0-d9fd-47fc-830a-88d1e6638b88",
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new[] {"4ec6db81-f8e0-4343-9973-f6a91e4d4c29", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.2, 12.2}, 
                  new[] {1.2, 2.3})]
        public void UpdateProductNotExists(DateTime date, string idPurchase, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            purchaseToTest.id = Guid.Parse(idPurchase);
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(Guid.Parse(idUser), purchaseToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Product does not exist")));
        }

        [Test]
        [TestCase("01/01/2023", 
                  "e2075166-6f2c-4172-8906-2f100a6a1456",
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"f150f3ca-1a18-40e1-a01a-fad22724514a", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {15.2, 12.2}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "51e6eec0-d9fd-47fc-830a-88d1e6638b88",
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "f2672cf5-761e-4f5a-8fc2-b3264fc5dd0d"}, 
                  new[] {15.2, 12.2}, 
                  new[] {1.2, 2.3})]
        public void UpdateCurrencyNotExists(DateTime date, string idPurchase, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            purchaseToTest.id = Guid.Parse(idPurchase);
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(Guid.Parse(idUser), purchaseToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Currency does not exist")));
        }

        [Test]
        [TestCase("01/01/2023", 
                  "e2075166-6f2c-4172-8906-2f100a6a1456",
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {-5, 1.22}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "51e6eec0-d9fd-47fc-830a-88d1e6638b88",
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new[] {"e5f89b1d-171f-4460-a2cc-18e1534b5bae", "526ea756-50da-486f-8a44-5e964f249c1e"}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {1.25, -1}, 
                  new[] {1.2, 2.3})]
        public void UpdatePriceNotCorrect(DateTime date, string idPurchase, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            purchaseToTest.id = Guid.Parse(idPurchase);
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(Guid.Parse(idUser), purchaseToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Price must be superior than 0")));
        }

        [Test]
        [TestCase("01/01/2023", 
                  "e2075166-6f2c-4172-8906-2f100a6a1456",
                  "0a891394-be17-473b-9924-eccaf6ce79ed",
                  new string[] {}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {-5, 1.22}, 
                  new[] {1.2, 2.3})]
        [TestCase("01/01/2023", 
                  "51e6eec0-d9fd-47fc-830a-88d1e6638b88",
                  "9beb47ab-0def-437c-b510-02d8f9623ebb",
                  new string[] {}, 
                  new[] {"1a7d6616-dfd1-47c8-ba42-2b12e71c43af", "1a7d6616-dfd1-47c8-ba42-2b12e71c43af"}, 
                  new[] {1.25, -1}, 
                  new[] {1.2, 2.3})]
        public void UpdateNoProduct(DateTime date, string idPurchase, string idUser, string[] products, string[] currencies, double[] prices, double[] quantities)
        {
            var purchaseToTest = Common.GetPurchaseDto(date, products.ToList(), currencies.ToList(), prices.ToList(), quantities.ToList());
            purchaseToTest.id = Guid.Parse(idPurchase);
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(Guid.Parse(idUser), purchaseToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("No product selected")));
        }

        [Test]
        [TestCase("0a891394-be17-473b-9924-eccaf6ce79ed",
                  "01/01/2022",
                  "\nPomme de Terre Monalisa\n1 × 2.5 kg\n3.90 €\n \n\nAubergine\n1 × 400 g\n2.20 €\n \n\nCarottes\n1 × 1 kg\n2.20 €\n \n\nOignons Jaune\n1 × 500 g\n1.40 €\n \n\nPotimarron\n2 × 1.2 kg\n7.60 €\n \n\nPersil\n1 × 1\n1.40 €\n \n\nPatate Douce\n2 × 500 g\n5.60 €\n \nEARL LA POMME DORET\n \n\nPoire Conference\n1 × 1 kg\n2.20 €\n \n\nPomme Golden\n1 × 1 kg\n2.00 €\n \nEARL BERGERIE DE BAISENAZ\n \n\nFlan de Brebis Vanille\n3 × (2 × 12.5 cL)\n8.16 €\n \n\nYaourt de Brebis Fraise\n1 × (2 × 125 g)\n2.60 €\n \n\nYaourt de Brebis Fruits Des Bois\n1 × (2 × 125 g)\n2.60 €\n \nGoûter Desserts\n \n\nFarine de Blé T65 Label Rouge\n1 × (1 × 1 kg)\n2.00 €\n \n\nFarine de Blé T80 Bio\n1 × (1 × 1 kg)\n2.40 €\n \n\nSucette Chocolat Au Lait\n1 × 15 g\n1.50 €\n \n\nTablette de Chocolat Lait Afrique\n1 × (1 × 100 g)\n5.50 €\n \nGAEC les Maillets\n \n\nFv- Le Frais de Vache\n1 × 170 g\n2.00 €",
                  0)]
        [TestCase("0a891394-be17-473b-9924-eccaf6ce79ed",
                  "01/01/2022",
                  "\nchou vert\n1 × 2.5 kg\n3.90 €\n \n\ntomate de saison\n1 × 400 g\n2.20 €\n \n\nCarottes\n1 × 1 kg\n2.20 €\n \n\nOignons Jaune\n1 × 500 g\n1.40 €\n \n\nPotimarron\n2 × 1.2 kg\n7.60 €\n \n\nPersil\n1 × 1\n1.40 €\n \n\nPatate Douce\n2 × 500 g\n5.60 €\n \nEARL LA POMME DORET\n \n\nPoire Conference\n1 × 1 kg\n2.20 €\n \n\nPomme Golden\n1 × 1 kg\n2.00 €\n \nEARL BERGERIE DE BAISENAZ\n \n\nFlan de Brebis Vanille\n3 × (2 × 12.5 cL)\n8.16 €\n \n\nYaourt de Brebis Fraise\n1 × (2 × 125 g)\n2.60 €\n \n\nYaourt de Brebis Fruits Des Bois\n1 × (2 × 125 g)\n2.60 €\n \nGoûter Desserts\n \n\nFarine de Blé T65 Label Rouge\n1 × (1 × 1 kg)\n2.00 €\n \n\nFarine de Blé T80 Bio\n1 × (1 × 1 kg)\n2.40 €\n \n\nSucette Chocolat Au Lait\n1 × 15 g\n1.50 €\n \n\nTablette de Chocolat Lait Afrique\n1 × (1 × 100 g)\n5.50 €\n \nGAEC les Maillets\n \n\nFv- Le Frais de Vache\n1 × 170 g\n2.00 €",
                  2)]
        public async Task PurchaseLaRuche(string idUser, DateTime datePurchase, string command, int productWaited) {
            PurchaseLaRucheDto purchaseLaRucheDto = Common.GetPurchaseLaRucheDto(datePurchase, command);
            var result = await _service.ImportLaRuchePurchase(Guid.Parse(idUser), purchaseLaRucheDto);
            
            Assert.IsTrue(_context.Object.Purchases.Any(p => p.id == result.id));
            Assert.AreEqual(_context.Object.Purchases.First(p => p.id == result.id).products.Count(), productWaited);
        }
    }
}