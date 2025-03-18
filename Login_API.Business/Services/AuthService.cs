    using FundooNotes.Business.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    namespace BusinessLayer.Services
    {
        public class AuthService : IAuthService
        {
            private readonly IConfiguration _config;

            public AuthService(IConfiguration config)
            {
                _config = config;
            }


        public string GenerateToken(int userId, string email, bool isResetToken = false)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email)
            };

            var expires = DateTime.UtcNow.AddMinutes(isResetToken ? 30 : Convert.ToInt32(_config["Jwt:ExpireMinutes"]));

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

            public bool ValidateResetToken(string token, out string email)
            {
                email = null;
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

                    var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _config["Jwt:Issuer"],
                        ValidAudience = _config["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    email = ((JwtSecurityToken)validatedToken).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                    return email != null;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
