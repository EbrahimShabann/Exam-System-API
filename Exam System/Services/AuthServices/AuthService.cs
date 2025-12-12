using Exam_System.Models;
using Exam_System.Services;
using Exam_System.Services.ReposService.IRepos;
using ExamSystem.Models;
using ExamSystem.Models.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ExamSystem.Services.AuthServices
{
    public class AuthService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AppDbContext dbContext;
        private readonly JwtOptions jwtOptions;

        public AuthService(IConfiguration configuration,
            UserManager<ApplicationUser> userManager, AppDbContext dbContext,
            JwtOptions jwtOptions)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.jwtOptions = jwtOptions;
            this.dbContext = dbContext;
        }

        internal  async Task<TokenResponse> ExchangeCodeForTokens(string code)
        {
            var httpClient = new HttpClient();

            string url;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                //url = _configuration.GetValue<string>("OAuthDevelopmentUrl") ;
                url = configuration["OAuthDevelopmentUrl"];
            }
            else
            {
                url = configuration.GetValue<string>("OAuthProductionUrl");
            }

            var requestBody = new Dictionary<string, string>
            {
                {"code",code },
                {"client_id", configuration["Google:ClientID"]},
                {"client_secret",configuration["Google:ClientSecret"]},
                {"redirect_uri", url },
                {"grant_type","authorization_code" }
            };

            var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(requestBody));
            if(!response.IsSuccessStatusCode)
            {
                throw new Exception("Error exchanging code for tokens");
            }

            string jsonContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TokenResponse>(jsonContent);
;
        }
    
        internal  GoogleUser DecodeIdToken(string idToken)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(idToken);

            return new GoogleUser
            {
                GoogleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub").Value,
                Email = jwtToken.Claims.FirstOrDefault(c=>c.Type=="email").Value,
                Name = jwtToken.Claims.FirstOrDefault(c=>c.Type=="name").Value,
                Picture = jwtToken.Claims.FirstOrDefault(c=>c.Type=="picture").Value,

            };
        }
    
        internal  async Task<ApplicationUser> FindOrCreateUser(GoogleUser googleUser)
        {
            var user= await dbContext.ApplicationUsers.FirstOrDefaultAsync(u=>u.Email==googleUser.Email);

            if (user is null)
            {

                user = new ApplicationUser
                {
                    Id=googleUser.GoogleId,
                    Email = googleUser.Email,
                    NormalizedEmail = googleUser.Email.ToUpper(),
                    EmailConfirmed = true,
                    UserName = googleUser.Name,
                    NormalizedUserName=googleUser.Name.ToUpper(),

                };

                dbContext.ApplicationUsers.Add(user);
                await userManager.AddToRoleAsync(user, StaticDetails.StudentRole);
                await dbContext.SaveChangesAsync();
            }
                return user;


        }
        internal string GenerateJwtToken(ApplicationUser user, string userRole)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                Expires = DateTime.UtcNow.AddMinutes(jwtOptions.Lifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    //new (JwtRegisteredClaimNames.Sub, user.Email),
                    new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new (ClaimTypes.NameIdentifier, user.Id),
                    new (ClaimTypes.Email, user.Email),
                    new (ClaimTypes.Role, userRole)
                })
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }


    }
}
