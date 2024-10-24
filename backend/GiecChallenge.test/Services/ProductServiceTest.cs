using Moq;
using GiecChallenge.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GiecChallenge.Models;
using GiecChallenge.Profiles;

namespace GiecChallenge.Tests
{
    public class ProductServiceTest
    {
        private Mock<ILogger<ProductService>> _logger = null!;
        private Mock<GiecChallengeContext> _context = null!;
        private ProductService _service = null!;
        private List<ProductDto> _allProductsDTO = null!;
        private string _language = "f3390acd-acf2-4ab9-8d39-25b216182320";

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ProductProfile());
            });
            
            IMapper mapper = config.CreateMapper();
            _logger = new Mock<ILogger<ProductService>>();

            _allProductsDTO = Common.GetProductsDto();            
            _context = Common.GetContext();            
            _context.Setup(g => g.Languages).Returns(Common.GetMockDbSet<Language>(Common.GetLanguages()).Object);
            _context.Setup(g => g.ProductSubGroups).Returns(Common.GetMockDbSet<ProductSubGroup>(Common.GetProductSubGroup()).Object);
            _context.Setup(g => g.ProductGroups).Returns(Common.GetMockDbSet<ProductGroup>(Common.GetProductGroup()).Object);
            _context.Setup(g => g.ProductUserTranslations).Returns(Common.GetMockDbSet<ProductUserTranslation>(Common.GetProductUserTranslations()).Object);
            _context.Setup(g => g.Products).Returns(Common.GetMockDbSet<Product>(Common.GetProducts()).Object);
            _context.Setup(g => g.Users).Returns(Common.GetMockDbSet<User>(Common.GetUsers()).Object);
            
            _service = new ProductService(_logger.Object, mapper, _context.Object);
        }

        [Test]
        public async Task GetAllProducts()
        {
            var result = await _service.GetAllProducts();
            
            Assert.AreEqual(result.Count, Common.GetProducts().Count);
        }

        [Test]
        [TestCase("ip")]
        [TestCase("pen", "EN")]
        public async Task GetByName(string name, string language = "FR")
        {
            var result = await _service.GetProducts(name, language);
            
            Assert.AreEqual(result.Any(re => re.names.Any(rer => rer.name.ToLower().Contains(name))), true);
        }

        [Test]
        [TestCase("pomme")]
        [TestCase("poire")]
        public async Task GetByNameNothing(string name)
        {
            var result = await _service.GetProducts(name, _language);

            Assert.AreEqual(result.Any(), false);
        }

        [Test]
        [TestCase("e5f89b1d-171f-4460-a2cc-18e1534b5bae")]
        [TestCase("526ea756-50da-486f-8a44-5e964f249c1e")]
        public async Task GetProduct(Guid id)
        {
            var result = await _service.GetProduct(id);
            
            Assert.AreEqual(result!.id, id);
        }

        [Test]
        [TestCase("46c619b1-1859-4665-bc8b-cf51eb30777e")]
        [TestCase("eca28033-9954-498f-89d4-a911e40f5a33")]
        public void GetProductNothing(Guid id)
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetProduct(id));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Product does not exist")));
        }

        [Test]
        [TestCase("991979cd-b95f-4e9a-85e7-e1f7ce6932fb", "526ea756-50da-486f-8a44-5e964f249c1e")]
        [TestCase("3a69d206-7236-11ed-a1eb-0242ac120002", "e5f89b1d-171f-4460-a2cc-18e1534b5bae")]
        public async Task GetProductByGroup(string group, Guid id)
        {
            var result = await _service.GetProductsByGroup(group);
            
            Assert.AreEqual(result!.First().id, id);
        }

        [Test]
        [TestCase("Voiture")]
        [TestCase("Animal")]
        public void GetProductByGroupBadValue(string group)
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetProductsByGroup(group));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Group ", group, " doesn't exist")));
        }

        [Test]
        [TestCase("fd837d14-a085-11ed-a8fc-0242ac120002")]
        [TestCase("fd837ea4-a085-11ed-a8fc-0242ac120002")]
        public async Task GetProductByGroupNothing(string group)
        {
            var result = await _service.GetProductsByGroup(group);

            Assert.AreEqual(result.Any(), false);
        }

        [Test]
        [TestCase("04f3eb50-6119-487a-86a6-b6c24e620536", "e5f89b1d-171f-4460-a2cc-18e1534b5bae")]
        [TestCase("4f52f771-7752-472f-921e-88824fc4c5d5", "526ea756-50da-486f-8a44-5e964f249c1e")]
        public async Task GetProductBySubGroup(string subgroup, Guid id)
        {
            var result = await _service.GetProductsBySubGroup(subgroup);
            
            Assert.AreEqual(result!.First().id, id);
        }

        [Test]
        [TestCase("Voiture")]
        [TestCase("Animal")]
        public void GetProductBySubGroupBadValue(string subgroup)
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetProductsBySubGroup(subgroup));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Sub group ", subgroup, " doesn't exist")));
        }

        [Test]
        [TestCase("db815ede-1764-46c4-9f37-7a80851930a2")]
        [TestCase("fd83786e-a085-11ed-a8fc-0242ac120002")]
        public async Task GetProductBySubGroupNothing(string subgroup)
        {
            var result = await _service.GetProductsBySubGroup(subgroup);

            Assert.AreEqual(result.Any(), false);
        }

        [Test]
        [TestCase("iPhone", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        [TestCase("Apple Watch", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        public async Task Create(string name, string language)
        { 
            var productDto = Common.GetProductDtoTest(name, Common.GetProductSubGroup().First().id.ToString(), language);

            await _service.Create(productDto);
            
            Assert.AreEqual(_context.Object.Products.Any(l => l.names.Any(ln => ln.name == name)), true);
        }

        [Test]
        [TestCase("iPhone", "0b1307be-9ffd-4dcd-9431-4fe58b6420d7")]
        [TestCase("Apple Watch", "0b1307be-9ffd-4dcd-9431-4fe58b6420g7")]
        public void CreateLanguageBadValue(string name, string language)
        { 
            var productDto = Common.GetProductDtoTest(name, Common.GetProductSubGroup().First().id.ToString(), language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(productDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Language ", language, " doesn't exist")));
        }

        [Test]
        [TestCase("iPhone", "0b1307be-9ffd-4dcd-9431-4fe58b6420d7")]
        [TestCase("Apple Watch", "0b1307be-9ffd-4dcd-9431-4fe58b6420g7")]
        public void CreateLanguageNotExists(string name, string language)
        { 
            var productDto = Common.GetProductDtoTest(name, Common.GetProductSubGroup().First().id.ToString(), language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(productDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Language ", language, " doesn't exist")));
        }

        [Test]
        [TestCase("iPhone", "Tablet", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        [TestCase("Apple Watch", "Watch", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        public void CreateGroupNotExists(string name, string group, string language)
        { 
            var productDto = Common.GetProductDtoTest(name, group, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(productDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Group does not exist")));
        }

        [Test]
        [TestCase("iPad", "04f3eb50-6119-487a-86a6-b6c24e620536", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        [TestCase("chou vert", "4f52f771-7752-472f-921e-88824fc4c5d5", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        public void CreateAlreadyExists(string name, string group, string language)
        { 
            var productDto = Common.GetProductDtoTest(name, group, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(productDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Product already exists")));
        }

        [Test]
        [TestCase("e5f89b1d-171f-4460-a2cc-18e1534b5bae", "iPhone", "04f3eb50-6119-487a-86a6-b6c24e620536", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        [TestCase("526ea756-50da-486f-8a44-5e964f249c1e", "Poire", "4f52f771-7752-472f-921e-88824fc4c5d5", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        public async Task Update(Guid id, string name, string group, string language)
        { 
            bool postIsOk = true;
            var productDto = Common.GetProductDtoTest(name, group, language);

            try {
                await _service.Update(id, productDto);
            }
            catch {
                postIsOk = false;
            }
            
            Assert.AreEqual(postIsOk, true);
        }

        [Test]
        [TestCase("46c619b1-1859-4665-bc8b-cf51eb30777e")]
        [TestCase("eca28033-9954-498f-89d4-a911e40f5a33")]
        public void UpdateDoesNotExists(Guid id)
        { 
            var productDto = Common.GetProductDtoTest("poire", "4f52f771-7752-472f-921e-88824fc4c5d5", "f3390acd-acf2-4ab9-8d39-25b216182320");

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(id, productDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Product does not exist")));
        }

        [Test]
        [TestCase("e5f89b1d-171f-4460-a2cc-18e1534b5bae", "iPhone", "04f3eb50-6119-487a-86a6-b6c24e620536", "CN")]
        [TestCase("526ea756-50da-486f-8a44-5e964f249c1e", "Poire", "4f52f771-7752-472f-921e-88824fc4c5d5", "FI")]
        public void UpdateLanguageNotExists(Guid id, string name, string group, string language)
        { 
            var productDto = Common.GetProductDtoTest(name, group, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(id, productDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Language ", language, " doesn't exist")));
        }

        [Test]
        [TestCase("e5f89b1d-171f-4460-a2cc-18e1534b5bae", "iPhone", "Phone", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        [TestCase("526ea756-50da-486f-8a44-5e964f249c1e", "Poire", "Alcool", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        public void UpdateGroupNotExists(Guid id, string name, string group, string language)
        { 
            var productDto = Common.GetProductDtoTest(name, group, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(id, productDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Group does not exist")));
        }

        [Test]
        [TestCase("Galaxy Tab", "0a891394-be17-473b-9924-eccaf6ce79ed", "e5f89b1d-171f-4460-a2cc-18e1534b5bae")]
        [TestCase("Waterman", "0a891394-be17-473b-9924-eccaf6ce79ed", "526ea756-50da-486f-8a44-5e964f249c1e")]
        public async Task CreateUserTranslation(string name, string user, string product)
        { 
            var productUserTranslationDTO = Common.GetProductTranslationDtoTest(name, user, product, string.Empty);

            await _service.CreateTranslation(productUserTranslationDTO);
            
            Assert.True(_context.Object.ProductUserTranslations.Any(l => l.name == name && l.user.id.ToString() == user && l.product.id.ToString() == product));
        }

        [Test]
        [TestCase("Galaxy Tab", "0a891394-be17-473b-9924-eccafwce79ed", "e5f89b1d-171f-4460-a2cc-18e1534b5bae")]
        [TestCase("Waterman", "0a891394-be17-473b-9924-eccaf6ci79ed", "526ea756-50da-486f-8a44-5e964f249c1e")]
        public void CreateUserTranslationUserNotExist(string name, string user, string product)
        { 
            var productUserTranslationDTO = Common.GetProductTranslationDtoTest(name, user, product, string.Empty);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.CreateTranslation(productUserTranslationDTO));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("User does not exist")));
        }

        [Test]
        [TestCase("Galaxy Tab", "0a891394-be17-473b-9924-eccaf6ce79ed", "e5f89b1d-171f-4460-a2cd-18e1534b5bae")]
        [TestCase("Waterman", "0a891394-be17-473b-9924-eccaf6ce79ed", "526ea756-50da-486f-8a4y-5e964f249c1e")]
        public void CreateUserTranslationProductNotExists(string name, string user, string product)
        { 
            var productUserTranslationDTO = Common.GetProductTranslationDtoTest(name, user, product, string.Empty);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.CreateTranslation(productUserTranslationDTO));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Product does not exist")));
        }

        [Test]
        [TestCase("tomate", "0a891394-be17-473b-9924-eccaf6ce79ed", "479d91d4-8f93-433f-8b4c-b5b08c12db5c")]
        [TestCase("chou", "9beb47ab-0def-437c-b510-02d8f9623ebb", "5823ec98-2726-4b39-b01e-8453bbde5524")]
        public async Task UpdateUserTranslation(string name, string user, string id)
        { 
            var productUserTranslationDTO = Common.GetProductTranslationDtoTest(name, string.Empty, string.Empty, id);
            var previousName = _context.Object.ProductUserTranslations.First(l => l.id == Guid.Parse(id)).name;

            await _service.UpdateTranslation(productUserTranslationDTO, Guid.Parse(user));
            
            Assert.AreNotEqual(_context.Object.ProductUserTranslations.First(l => l.id == Guid.Parse(id)).name, previousName);
        }

        [Test]
        [TestCase("tomate", "0a891394-be17-473b-9924-eccaf6ce79ed", "f917a5bf-4a96-496b-90cc-ad24d6d4f4fe")]
        [TestCase("chou", "9beb47ab-0def-437c-b510-02d8f9623ebb", "debc6de1-48ce-430c-b4e4-0ab5024272f4")]
        public void UpdateUserTranslationNotExists(string name, string user, string id)
        { 
            var productUserTranslationDTO = Common.GetProductTranslationDtoTest(name, string.Empty, string.Empty, id);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.UpdateTranslation(productUserTranslationDTO, Guid.Parse(user)));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Translation doesn't exist")));
        }

        [Test]
        [TestCase("tomate", "0a891394-be17-473b-9924-eccaf6ce79ed", "479d91d4-8f93-433f-8b4c-b5b08c12db5c")]
        [TestCase("chou", "9beb47ab-0def-437c-b510-02d8f9623ebb", "5823ec98-2726-4b39-b01e-8453bbde5524")]
        public async Task DeleteUserTranslation(string name, string user, string id)
        { 
            var productUserTranslationDTO = Common.GetProductTranslationDtoTest(name, string.Empty, string.Empty, id);

            await _service.DeleteTranslation(productUserTranslationDTO, Guid.Parse(user));
            
            Assert.IsTrue(_context.Object.ProductUserTranslations.Any(l => l.id == Guid.Parse(id)));
        }

        [Test]
        [TestCase("tomate", "0a891394-be17-473b-9924-eccaf6ce79ed", "f917a5bf-4a96-496b-90cc-ad24d6d4f4fe")]
        [TestCase("chou", "9beb47ab-0def-437c-b510-02d8f9623ebb", "debc6de1-48ce-430c-b4e4-0ab5024272f4")]
        public void DeleteUserTranslationNotExists(string name, string user, string id)
        { 
            var productUserTranslationDTO = Common.GetProductTranslationDtoTest(name, string.Empty, string.Empty, id);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.DeleteTranslation(productUserTranslationDTO, Guid.Parse(user)));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Translation does not exist")));
        }
    }
}