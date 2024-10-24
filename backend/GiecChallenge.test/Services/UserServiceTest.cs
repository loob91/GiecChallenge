using Moq;
using GiecChallenge.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using GiecChallenge.Models;
using GiecChallenge.Profiles;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace GiecChallenge.Tests
{
    public class UserServiceTest
    {
        private Mock<ILogger<UserService>> _logger = null!;
        private Mock<GiecChallengeContext> _context = null!;
        private IConfiguration _configuration = null!;
        private UserService _service = null!;
        private List<UserDto> _allUsersDTO = null!;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserProfile());
            });
            
            IMapper mapper = config.CreateMapper();
            _logger = new Mock<ILogger<UserService>>();
            var inMemorySettings = new Dictionary<string, string?> {
                {"PasswordHash", "udr576eozuQkiLiLjpPJ"},
                {"JWT:Secret", "YcJ=OB0%uFr$Q8sT<(o'"},
                {"JWT:ValidIssuer", "http://localhost"},
                {"JWT:ValidAudience", "http://localhost"}
            };
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            _context = Common.GetContext();
            _context.Setup(g => g.Users).Returns(Common.GetMockDbSet<User>(Common.GetUsers()).Object);
            _context.Setup(g => g.Languages).Returns(Common.GetMockDbSet<Language>(Common.GetLanguages()).Object);
            _context.Setup(g => g.UserGroups).Returns(Common.GetMockDbSet<UserGroup>(Common.GetUserGroups()).Object);
            
            _service = new UserService(_logger.Object, mapper, _context.Object, _configuration);
        }

        [Test]
        [TestCase("email@email.com", "toto1", "FR")]
        [TestCase("email2@email.com", "toto3", "EN")]
        public async Task Register(string email, string password, string language)
        {
            var userToTest = Common.GetUserDto(email, password, language);
            await _service.Register(userToTest);
            
            Assert.IsTrue(_context.Object.Users.Any(u => u.email == email));
        }

        [Test]
        [TestCase("email@email.com", "toto1", "DK")]
        [TestCase("email2@email.com", "toto3", "CN")]
        public void RegisterLanguageNotExists(string email, string password, string language)
        { 
            var userToTest = Common.GetUserDto(email, password, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Register(userToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Language does not exist")));
        }

        [Test]
        [TestCase("toto@toto.com", "toto1", "FR")]
        [TestCase("toto1@toto.com", "toto3", "EN")]
        public void RegisterAlreadyExist(string email, string password, string language)
        { 
            var userToTest = Common.GetUserDto(email, password, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Register(userToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("User already exist")));
        }

        [Test]
        [TestCase("toto@toto.com", "password1", "FR")]
        [TestCase("toto1@toto.com", "password2", "EN")]
        public async Task Login(string email, string password, string language)
        {
            var userToTest = Common.GetUserDto(email, password, language);
            var result = await _service.Login(userToTest);
            
            Assert.IsTrue(_context.Object.Users.Any(u => u.email == email));
        }

        [Test]
        [TestCase("toto@toto.com", "toto1", "FR")]
        [TestCase("toto1@toto.com", "toto2", "FR")]
        [TestCase("toto1@toto.com", "password1", "FR")]
        [TestCase("toto@toto.com", "password2", "FR")]
        public void LoginNotExist(string email, string password, string language)
        { 
            var userToTest = Common.GetUserDto(email, password, language);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Login(userToTest));
            Assert.That(ex!.Message, Is.EqualTo(string.Concat("Email or password is incorrect")));
        }
    }
}