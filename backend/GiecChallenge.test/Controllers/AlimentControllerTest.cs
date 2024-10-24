using Moq;
using GiecChallenge.Controllers;
using GiecChallenge.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace GiecChallenge.Tests
{
    public class AlimentControllerTests
    {
        private Mock<ILogger<AlimentController>> _logger = null!;
        private Mock<IAlimentRepository> _alimentRepository = null!;
        private AlimentController _controller = null!;
        private List<AlimentDto> _allAliments = null!;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<AlimentController>>();
            _alimentRepository = new Mock<IAlimentRepository>();
            _controller = new AlimentController(_logger.Object, _alimentRepository.Object);
            _allAliments = Common.GetAlimentDto();
        }

        [Test]
        public async Task GetAllAliments()
        {
            _alimentRepository.Setup(p => p.GetAllAliments()).ReturnsAsync(_allAliments);

            var result = await _controller.Get() as OkObjectResult;
            
            Assert.AreEqual(result!.Value, _allAliments);
        }

        [Test]
        [TestCase("tom")]
        [TestCase("tomate")]
        public async Task GetByCode(string name)
        {
            _alimentRepository.Setup(p => p.GetAliments(It.IsAny<string>())).ReturnsAsync(new List<AlimentDto>() { _allAliments.First() });

            var result = await _controller.GetByName(name) as OkObjectResult;
            
            Assert.AreEqual(result!.Value, new List<AlimentDto>() { _allAliments.First() });
        }

        [Test]
        [TestCase("pou")]
        [TestCase("tos")]
        public async Task GetByCodeNoAnswer(string name)
        {
            _alimentRepository.Setup(p => p.GetAliments(It.IsAny<string>())).ReturnsAsync(new List<AlimentDto>());

            var result = await _controller.GetByName(name) as OkObjectResult;
            
            Assert.AreEqual(result!.Value, new List<AlimentDto>());
        }

        [Test]
        [TestCase("1002")]
        [TestCase("1003")]
        public async Task GetById(string id)
        {
            _alimentRepository.Setup(p => p.GetAliment(It.IsAny<string>())).ReturnsAsync(_allAliments.First());

            var result = await _controller.GetByCode(id) as OkObjectResult;
            
            Assert.AreEqual(result!.Value, _allAliments.First());
        }

        [Test]
        [TestCase("5")]
        [TestCase("6")]
        public async Task GetByIdNoAnswerAsync(string ciqual)
        {
            var expected = new AlimentDto();
            _alimentRepository.Setup(p => p.GetAliment(It.IsAny<string>())).ThrowsAsync(new Exception(string.Concat(ciqual, " does not exist")));
            
            var ex =  await _controller.GetByCode(ciqual) as ObjectResult;
            Assert.AreEqual(ex!.StatusCode, 500);
        }

        [Test]
        [TestCase("Poire", "1005")]
        [TestCase("Abricot", "1006")]
        public async Task Post(string name, string ciqual)
        {
            bool postIsOk = true;
            var alimentToSend = Common.GetAlimentDtoTest(name, ciqual);
            _alimentRepository.Setup(p => p.Create(alimentToSend!));

            try {
                await _controller.Post(alimentToSend);
            }
            catch {
                postIsOk = false;
            }
            
            Assert.AreEqual(postIsOk, true);
        }

        [Test]
        [TestCase("1002")]
        [TestCase("1003")]
        public async Task PostCiqualAlreadyExistAsync(string ciqual)
        {
            var alimentToSend = _allAliments.First(a => a.ciqual_code == ciqual);
            _alimentRepository.Setup(p =>p.Create(alimentToSend!)).ThrowsAsync(new Exception(string.Concat(ciqual, " already exists")));
            
            var ex =  await _controller.Post(alimentToSend) as ObjectResult;
            Assert.AreEqual(ex!.StatusCode, 500);
        }

        [Test]
        [TestCase("1002")]
        [TestCase("1003")]
        public async Task Update(string ciqual)
        {
            bool postIsOk = true;
            var alimentToSend = _allAliments.First(a => a.ciqual_code == ciqual);
            alimentToSend.nom_francais = "toto";
            _alimentRepository.Setup(p => p.Update(alimentToSend!));

            try {
                await _controller.Update(alimentToSend);
            }
            catch {
                postIsOk = false;
            }
            
            Assert.AreEqual(postIsOk, true);
        }

        [Test]
        [TestCase("1002")]
        [TestCase("1003")]
        public async Task UpdateNotExistAsync(string ciqual)
        {
            var alimentToSend = _allAliments.First(a => a.ciqual_code == ciqual);
            alimentToSend.nom_francais = "toto";
            
            _alimentRepository.Setup(p =>p.Update(alimentToSend!)).ThrowsAsync(new Exception("Aliment does not exist"));
            
            var ex = await _controller.Update(alimentToSend) as ObjectResult;
            Assert.AreEqual(ex!.StatusCode, 500);
        }
    }
}