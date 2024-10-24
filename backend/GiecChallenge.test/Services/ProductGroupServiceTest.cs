using Moq;
using GiecChallenge.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GiecChallenge.Models;
using GiecChallenge.Profiles;

namespace GiecChallenge.Tests
{
    public class ProductGroupServiceTest
    {
        private Mock<ILogger<GroupService>> _logger = null!;
        private Mock<GiecChallengeContext> _context = null!;
        private GroupService _service = null!;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new GroupProfile());
            });
            
            IMapper mapper = config.CreateMapper();
            _logger = new Mock<ILogger<GroupService>>();
        
            _context = Common.GetContext();            
            _context.Setup(g => g.ProductGroups).Returns(Common.GetMockDbSet<ProductGroup>(Common.GetGroups()).Object);
            _context.Setup(g => g.ProductGroupLanguages).Returns(Common.GetMockDbSet<ProductGroupLanguage>(Common.GetGroupLanguage()).Object);
            _context.Setup(g => g.Languages).Returns(Common.GetMockDbSet<Language>(Common.GetLanguages()).Object);
            _context.Setup(g => g.LanguageLanguages).Returns(Common.GetMockDbSet<LanguageLanguage>(Common.GetLanguages().SelectMany(l => l.names).ToList()).Object);
            
            _service = new GroupService(_logger.Object, mapper, _context.Object);
        }

        [Test]
        public async Task GetAllGroups()
        {
            var result = await _service.GetAllGroups();
            
            Assert.AreEqual(result.Count, Common.GetGroups().Count);
        }

        [Test]
        [TestCase("aliMent", "FR")]
        [TestCase("tro", "EN")]
        public async Task GetByName(string name, string language)
        {
            var result = await _service.GetGroups(name, language);
            
            Assert.AreEqual(result.Any(re => re.names.Any(rer => rer.name.ToLower().Contains(name.ToLower()))), true);
        }

        [Test]
        [TestCase("pomme", "FR")]
        [TestCase("poire", "EN")]
        [TestCase("aliMent", "EN")]
        public async Task GetByNameNothing(string name, string language)
        {
            var result = await _service.GetGroups(name, language);

            Assert.AreEqual(result.Any(), false);
        }

        [Test]
        [TestCase("b21a6403-f428-454f-942d-dbd1fc3fa551")]
        [TestCase("8f46bf6f-6cbf-47ac-8d51-039eabc820c3")]
        public async Task GetGroup(Guid id)
        {
            var result = await _service.GetGroup(id);
            
            Assert.AreEqual(result!.id, id);
        }

        [Test]
        [TestCase("46c619b1-1859-4665-bc8b-cf51eb30777e")]
        [TestCase("eca28033-9954-498f-89d4-a911e40f5a33")]
        public void GetGroupNothing(Guid id)
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetGroup(id));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Group does not exist")));
        }

        [Test]
        [TestCase("Transport", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        [TestCase("Commute", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        public async Task Create(string name, string language)
        { 
            var groupDto = Common.GetGroupDtoTest(name, language);

            await _service.Create(groupDto);
            
            Assert.AreEqual(_context.Object.ProductGroups.Any(l => l.names.Any(ln => ln.name == name)), true);
        }

        [Test]
        [TestCase("aliMent", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        [TestCase("ElectroniC device", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        public void CreateAlreadyExists(string name, string language)
        { 
            var groupDto = Common.GetGroupDtoTest(name, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(groupDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Group already exists")));
        }

        [Test]
        [TestCase("b21a6403-f428-454f-942d-dbd1fc3fa551", "Nourriture", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        [TestCase("8f46bf6f-6cbf-47ac-8d51-039eabc820c3", "Materiel electronique", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        public async Task Update(Guid id, string name, string isoCode)
        { 
            var groupDto = Common.GetGroupDtoTest(name, isoCode);
            
            await _service.Update(id, groupDto);
            
            Assert.True(_context.Object.ProductGroups.Any(l => l.names.Any(ln => ln.name == name)));
        }

        [Test]
        [TestCase("46c619b1-1859-4665-bc8b-cf51eb30777e", "f3390acd-acf2-4ab9-8d39-25b216182320", "Français")]
        [TestCase("eca28033-9954-498f-89d4-a911e40f5a33", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7", "English")]
        public void UpdateDoesNotExists(Guid id, string isoCode, string name)
        { 
            var groupDto = Common.GetGroupDtoTest(isoCode, name);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(id, groupDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Group does not exist")));
        }
    }
}