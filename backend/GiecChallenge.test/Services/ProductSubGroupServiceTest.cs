using Moq;
using GiecChallenge.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GiecChallenge.Models;
using GiecChallenge.Profiles;

namespace GiecChallenge.Tests
{
    public class ProductSubGroupServiceTest
    {
        private Mock<ILogger<SubGroupService>> _logger = null!;
        private Mock<GiecChallengeContext> _context = null!;
        private SubGroupService _service = null!;
        private List<GroupDto> _allGroupsDTO = null!;
        private string _Group = "FR";

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new SubGroupProfile());
            });
            
            IMapper mapper = config.CreateMapper();
            _logger = new Mock<ILogger<SubGroupService>>();

            _allGroupsDTO = Common.GetGroupsDto("Français", "English");            
            _context = Common.GetContext();            
            _context.Setup(g => g.ProductGroups).Returns(Common.GetMockDbSet<ProductGroup>(Common.GetGroups()).Object);
            _context.Setup(g => g.ProductSubGroups).Returns(Common.GetMockDbSet<ProductSubGroup>(Common.GetSubGroups()).Object);
            _context.Setup(g => g.ProductSubGroupLanguages).Returns(Common.GetMockDbSet<ProductSubGroupLanguage>(Common.GetSubGroupLanguage()).Object);
            _context.Setup(g => g.Languages).Returns(Common.GetMockDbSet<Language>(Common.GetLanguages()).Object);
            
            _service = new SubGroupService(_logger.Object, mapper, _context.Object);
        }

        [Test]
        public async Task GetAllGroups()
        {
            var result = await _service.GetAllSubGroups();
            
            Assert.AreEqual(result.Count, Common.GetGroups().Count);
        }

        [Test]
        [TestCase("Boisson")]
        [TestCase("hon")]
        public async Task GetByName(string name)
        {
            var result = await _service.GetSubGroups(name, _Group);
            
            Assert.AreEqual(result.Any(re => re.names.Any(rer => rer.name.ToLower().Contains(name.ToLower()))), true);
        }

        [Test]
        [TestCase("vian")]
        [TestCase("poire")]
        public async Task GetByNameNothing(string name)
        {
            var result = await _service.GetSubGroups(name, _Group);

            Assert.AreEqual(result.Any(), false);
        }

        [Test]
        [TestCase("bf0cc5d3-2b4f-4761-ac4f-5dc25005aa32")]
        [TestCase("1dda078c-d158-4078-aa8e-981d5ac5cd57")]
        public async Task GetSubGroup(Guid id)
        {
            var result = await _service.GetSubGroup(id);
            
            Assert.AreEqual(result!.id, id);
        }

        [Test]
        [TestCase("46c619b1-1859-4665-bc8b-cf51eb30777e")]
        [TestCase("eca28033-9954-498f-89d4-a911e40f5a33")]
        public void GetSubGroupNothing(Guid id)
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetSubGroup(id));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Sub Group does not exist")));
        }

        [Test]
        [TestCase("Viande", "b21a6403-f428-454f-942d-dbd1fc3fa551", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        [TestCase("Tablet", "8f46bf6f-6cbf-47ac-8d51-039eabc820c3", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        public async Task Create(string name, string groupOfName, string language)
        { 
            var subGroupDto = Common.GetSubGroupDtoTest(name, groupOfName, language);

            await _service.Create(subGroupDto);
            
            Assert.AreEqual(_context.Object.ProductSubGroups.Any(l => l.names.Any(ln => ln.name == name)), true);
        }

        [Test]
        [TestCase("Boisson", "b21a6403-f428-454f-942d-dbd1fc3fa551", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        [TestCase("Smartphone", "8f46bf6f-6cbf-47ac-8d51-039eabc820c3", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        public void CreateAlreadyExists(string name, string groupOfName, string language)
        { 
            var subGroupDto = Common.GetSubGroupDtoTest(name, groupOfName, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(subGroupDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Sub Group already exists")));
        }

        [Test]
        [TestCase("bf0cc5d3-2b4f-4761-ac4f-5dc25005aa32", "Tablet", "8f46bf6f-6cbf-47ac-8d51-039eabc820c3", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        [TestCase("1dda078c-d158-4078-aa8e-981d5ac5cd57", "Viande", "b21a6403-f428-454f-942d-dbd1fc3fa551", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        public async Task Update(Guid id, string name, string groupOfName, string language)
        { 
            var subGroupDto = Common.GetSubGroupDtoTest(name, groupOfName, language);
            
            await _service.Update(id, subGroupDto);
            
            Assert.True(_context.Object.ProductSubGroups.Any(l => l.names.Any(ln => ln.name == name)));
        }

        [Test]
        [TestCase("46c619b1-1859-4665-bc8b-cf51eb30777e", "Tablet", "991979cd-b95f-4e9a-85e7-e1f7ce6932fb", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        [TestCase("eca28033-9954-498f-89d4-a911e40f5a33", "Viande", "3a69d206-7236-11ed-a1eb-0242ac120002", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        public void UpdateDoesNotExists(Guid id, string isoCode, string name, string language)
        { 
            var subGroupDto = Common.GetSubGroupDtoTest(isoCode, name, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(id, subGroupDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Sub Group does not exist")));
        }

        [Test]
        [TestCase("bf0cc5d3-2b4f-4761-ac4f-5dc25005aa32", "Tablet", "Elec", "FR")]
        [TestCase("1dda078c-d158-4078-aa8e-981d5ac5cd57", "Viande", "Bouffe", "EN")]
        public void UpdateWithGroupBadFormat(Guid id, string isoCode, string name, string language)
        { 
            var subGroupDto = Common.GetSubGroupDtoTest(isoCode, name, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(id, subGroupDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat(name, " is not valid")));
        }

        [Test]
        [TestCase("bf0cc5d3-2b4f-4761-ac4f-5dc25005aa32", "Tablet", "bf0cc5d3-2b4f-4761-ac4f-5dc25005aa32", "f3390acd-acf2-4ab9-8d39-25b216182320")]
        [TestCase("1dda078c-d158-4078-aa8e-981d5ac5cd57", "Viande", "1dda078c-d158-4078-aa8e-981d5ac5cd57", "0b1307be-9ffd-4dcd-9431-4fe58b6420f7")]
        public void UpdateWithGroupUnknown(Guid id, string isoCode, string name, string language)
        { 
            var subGroupDto = Common.GetSubGroupDtoTest(isoCode, name, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(id, subGroupDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Group does not exist")));
        }

        [Test]
        [TestCase("bf0cc5d3-2b4f-4761-ac4f-5dc25005aa32", "Tablet", "991979cd-b95f-4e9a-85e7-e1f7ce6932fb", "IT")]
        [TestCase("1dda078c-d158-4078-aa8e-981d5ac5cd57", "Viande", "3a69d206-7236-11ed-a1eb-0242ac120002", "ES")]
        public void UpdateWithLanguageBadValue(Guid id, string isoCode, string name, string language)
        { 
            var subGroupDto = Common.GetSubGroupDtoTest(isoCode, name, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(id, subGroupDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat(language, " is not valid")));
        }

        [Test]
        [TestCase("bf0cc5d3-2b4f-4761-ac4f-5dc25005aa32", "Tablet", "991979cd-b95f-4e9a-85e7-e1f7ce6932fb", "991979cd-b95f-4e9a-85e7-e1f7ce6932fb")]
        [TestCase("1dda078c-d158-4078-aa8e-981d5ac5cd57", "Viande", "3a69d206-7236-11ed-a1eb-0242ac120002", "3a69d206-7236-11ed-a1eb-0242ac120001")]
        public void UpdateWithLanguageUnknown(Guid id, string isoCode, string name, string language)
        { 
            var subGroupDto = Common.GetSubGroupDtoTest(isoCode, name, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Update(id, subGroupDto));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Language ", language, " doesn't exist")));
        }
    }
}