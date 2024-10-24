using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using GiecChallenge.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GiecChallenge.Services
{
    public interface IUserRepository
    {
        Task<object> Login(UserDto userDto);
        Task Register(UserDto userDto);
    }

    public class UserService : IUserRepository
    {
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly GiecChallengeContext _context;
        private readonly IConfiguration _configuration;

        public UserService(ILogger<UserService> logger,
                              IMapper mapper,
                              GiecChallengeContext context,
                              IConfiguration configuration) {
            this._logger = logger;
            this._mapper = mapper;
            this._context = context;
            this._configuration = configuration;
        }

        public async Task<object> Login(UserDto userDto)
        {
            var user = await _context.Users.Include(u => u.groups).ThenInclude(g => g.userGroup).FirstOrDefaultAsync(u => u.email == userDto.email);
            if (user != null && String.Compare(EncryptPassword(userDto.password, user.hash).hash, user.password) == 0)
            {
                var userRoles = user.groups;

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole.userGroup.name));
                }

                var token = GetToken(authClaims);
                return new { 
                    token = new JwtSecurityTokenHandler().WriteToken(token), 
                    validTo = token.ValidTo 
                };
            }
            throw new Exception("Email or password is incorrect");
        }

        public async Task Register(UserDto userDto)
        {   
            if (await this._context.Users.AnyAsync(u => u.email == userDto.email))
                throw new Exception("User already exist");

            var user = _mapper.Map<User>(userDto);

            Language? favoriteLanguage = await _context.Languages.FirstOrDefaultAsync(l => l.ISOCode == userDto.language);

            if (favoriteLanguage == null)
                throw new Exception("Language does not exist");

            user.favoriteLanguage = favoriteLanguage;

            var group = await _context.UserGroups.FirstOrDefaultAsync(ug => ug.name == "Classic");

            if (group == null)
                throw new Exception("Group does not exist");

            user.groups.Add(new UserInGroup() {
                userGroup = group,
                user = user
            });

            var hashSalt = EncryptPassword(userDto.password);
            user.password = hashSalt.hash;
            user.hash = hashSalt.salt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        private HashSalt EncryptPassword(string password, byte[] salt = null!)
        {
            if (salt == null || salt.Length == 0) {
                salt = new byte[128 / 8]; // Generate a 128-bit salt using a secure PRNG
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
                salt = Concat(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("PasswordHash")!), salt);
            }
            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            return new HashSalt { hash = encryptedPassw , salt = salt };
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {            
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Secret")!));

            var token = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("JWT:ValidIssuer")!,
                audience: _configuration.GetValue<string>("JWT:ValidAudience")!,
                expires: DateTime.Now.AddDays(5),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        static byte[] Concat(byte[] a, byte[] b)
        {           
            byte[] output = new byte[a.Length + b.Length];
            for (int i = 0; i < a.Length; i++)
                output[i] = a[i];
            for (int j = 0; j < b.Length; j++)
                output[a.Length+j] = b[j];
            return output;           
        }

        private class HashSalt
        {
            public string hash {get;set;} = string.Empty;
            public byte[] salt {get;set;} = null!;
        }
    }
}