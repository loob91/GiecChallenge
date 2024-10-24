using Moq;
using GiecChallenge.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GiecChallenge.Models;
using GiecChallenge.Profiles;

namespace GiecChallenge.Tests
{
    public class AlimentServiceTest
    {
        private Mock<ILogger<AlimentService>> _logger = null!;
        private Mock<GiecChallengeContext> _context = null!;
        private AlimentService _service = null!;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AlimentProfile());
                cfg.AddProfile(new ProductProfile());
            });
            
            IMapper mapper = config.CreateMapper();
            _logger = new Mock<ILogger<AlimentService>>();
       
            _context = Common.GetContext();            
            _context.Setup(g => g.Languages).Returns(Common.GetMockDbSet<Language>(Common.GetLanguages()).Object);
            _context.Setup(g => g.ProductUserTranslations).Returns(Common.GetMockDbSet<ProductUserTranslation>(Common.GetProductUserTranslations()).Object);
            _context.Setup(g => g.ProductSubGroups).Returns(Common.GetMockDbSet<ProductSubGroup>(Common.GetProductSubGroup()).Object);
            _context.Setup(g => g.Aliments).Returns(Common.GetMockDbSet<Aliment>(Common.GetAliments()).Object);
            
            _service = new AlimentService(_logger.Object, mapper, _context.Object);
        }

        [Test]
        public async Task GetAllAliments()
        {
            var result = await _service.GetAllAliments();
            
            Assert.AreEqual(result.Count, Common.GetAliments().Count);
        }

        [Test]
        [TestCase("tom", "1002")]
        [TestCase("tomate", "1002")]
        [TestCase("poire", "1004")]
        public async Task GetByName(string name, string ciqual)
        {
            var result = await _service.GetAliments(name);
            
            Assert.AreEqual(result.Any(re => re.ciqual_code == ciqual), true);
        }

        [Test]
        [TestCase("pomme")]
        [TestCase("coca")]
        public async Task GetByNameNothing(string name)
        {
            var result = await _service.GetAliments(name);
            
            Assert.AreEqual(result.Any(), false);
        }

        [Test]
        [TestCase("1002")]
        [TestCase("1003")]
        public async Task GetAliment(string id)
        {
            var result = await _service.GetAliment(id);
            
            Assert.AreEqual(Common.GetAliments().First(al => al.ciqual == id).ciqual, id);
        }

        [Test]
        [TestCase("10000002")]
        [TestCase("10000003")]
        public void GetAlimentNothing(string id)
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetAliment(id));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat(id, " does not exist")));
        }

        [Test]
        [TestCase("10000002")]
        [TestCase("10000003")]
        public async Task Create(string id)
        { 
            var aliment = Common.GetAlimentTest("poire", id);
            var alimentDto = Common.GetAlimentDtoTest("poire", id);

            await _service.Create(alimentDto);
            
            Assert.AreEqual(_context.Object.Aliments.Any(l => l.ciqual == id), true);
        }

        [Test]
        [TestCase("1002")]
        [TestCase("1003")]
        public void CreateAlreadyExists(string id)
        { 
            var aliment = Common.GetAlimentTest("poire", id);
            var alimentDto = Common.GetAlimentDtoTest("poire", id);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(alimentDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat(id, " already exists")));
        }

        [Test]
        [TestCase("1002")]
        [TestCase("1003")]
        public async Task Update(string id)
        { 
            bool postIsOk = true;
            var alimentDto = Common.GetAlimentDtoTest("poire", id);

            try {
                await _service.Update(alimentDto);
            }
            catch {
                postIsOk = false;
            }
            
            Assert.AreEqual(postIsOk, true);
        }

        [Test]
        [TestCase("10000002")]
        [TestCase("10000003")]
        public void UpdateDoesNotExists(string id)
        { 
            var alimentDto = Common.GetAlimentDtoTest("poire", id);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(alimentDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Aliment does not exist")));
        }
    }
}