using coremvctest.IService;
using coremvctest.Models;
using coremvctest.Utility.Content;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace coremvctest.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private IConfiguration _config;
        public AuthenticationService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateTokenForFoodInventory(FoodStoreEntity? foodInventory)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? string.Empty));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, foodInventory?.FoodInventoryUserName??string.Empty),
                new Claim(ClaimTypes.NameIdentifier, Convert.ToString(foodInventory?.FoodInventoryId)??string.Empty),
                //new Claim(ClaimTypes.Role, Convert.ToString(user?.Role?.Name)??string.Empty),
            };
            var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
            if (token == null)
                return UserMessages.somethingWentWrong;
            return token;
        }

        public bool validateJwtToken(string jwtToken, HttpContext httpContext)
        {
            try { 
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_config["Jwt:Key"] ?? string.Empty);
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out SecurityToken validatedToken);
            JwtSecurityToken validatedJWT = (JwtSecurityToken)validatedToken;
            string userId = validatedJWT.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
