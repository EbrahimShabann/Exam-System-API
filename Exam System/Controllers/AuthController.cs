using Exam_System.Models;
using Exam_System.Services.DTOs;
using ExamSystem.Models;
using ExamSystem.Services;
using ExamSystem.Services.AuthServices;
using ExamSystem.Services.credintailsService;
using ExamSystem.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace Exam_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JwtOptions jwtOptions;
        private readonly AuthService AuthService;
        private readonly ICredintailsService credintailsService;
        private readonly IEmailService emailService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            JwtOptions jwtOptions,
            AuthService AuthService,
            ICredintailsService credintailsService,
            IEmailService emailService


            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            this.jwtOptions = jwtOptions;
            this.AuthService = AuthService;
            this.credintailsService = credintailsService;
            this.emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result= await credintailsService.registerAsync(model);

            if(result.success)
                //send email with otp
                await emailService.SendEmailAsync(model.Email, "OTP Code Validation", 
                $"Your OTP Code is {result.otp}\ncode is valid for only one time");


            return result.success? Ok(new { message = result.message , success=result.success}) 
                                 : BadRequest(new{message= result.message, success=result.success });        
        }

        [HttpPost("validate-otp")]
        public async Task<IActionResult> ValidateOTP([FromBody] otpValidationDto dto )
        {
            var isValid = await credintailsService.ValidateOTPAsync(dto.email,dto.otpCode);
           
            return isValid? Ok(new { message = "OTP validated successfully." }) 
                          : BadRequest(new { message = "Invalid or expired OTP." });
            ;
        }

        [HttpGet("resend-otp")]
        public async Task<IActionResult> ResendOTP( string userEmail)
        {
           var otp = await credintailsService.GenerateOTP(userEmail);

           //send email with otp
           await emailService.SendEmailAsync(userEmail, "OTP Code Validation",
           $"Your OTP Code is {otp.OTPCode}\ncode is valid for only one time");

           return Ok(new { message = "OTP resent successfully." });
        }

        [HttpPost("login")]   
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized();

            var userRole = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
           

            var token = AuthService.GenerateJwtToken(user, userRole);
           
            return Ok(new { token=token ,userRole=userRole});
        }

        

        [Authorize]
        [HttpGet("me/username")]
        public async Task<IActionResult> GetLoggedInUsername()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            return Ok(new { username = user.UserName, role });
        }

        [HttpGet("LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle() 
        {
            string state = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("OAuthState", state);

            string url;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                //url = _configuration.GetValue<string>("OAuthDevelopmentUrl") ;
                url = _configuration["OAuthDevelopmentUrl"] ;
            }
            else
            {
                url = _configuration.GetValue<string>("OAuthProductionUrl") ;
            }

            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                $"client_id={_configuration["Google:ClientID"]}" +
                $"&redirect_uri={url}" +
                "&response_type=code" +
                "&scope=openid%20email%20profile" +
                $"&state={state}";

            return Redirect(authUrl);
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> Callback(string code, string state)
        {
            string savedState = HttpContext.Session.GetString("OAuthState");

            if(state != savedState)
                 return BadRequest("Invalid state parameter");

            //Exchange Code For Tokens to get Id Token
            var tokenResponse = await AuthService.ExchangeCodeForTokens(code);

            //Decode Id Token to JWT Token to get google user Info
            var googleUser = AuthService.DecodeIdToken(tokenResponse.IdToken);

            //Create or find this google user in database
            var appUser = await AuthService.FindOrCreateUser(googleUser);
            var userRole =_userManager.GetRolesAsync(appUser).Result.FirstOrDefault();

            string token = AuthService.GenerateJwtToken(appUser, userRole);

            HttpContext.Response.Cookies.Append("token", token,
                new CookieOptions { 
                    HttpOnly=false,
                    Secure=true,
                    SameSite=SameSiteMode.None,
                    Domain="localhost",
                    Path="/",
                    Expires=DateTimeOffset.UtcNow.AddMinutes(jwtOptions.Lifetime)

                });
            HttpContext.Response.Cookies.Append("UserRole", userRole, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = "localhost",
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(jwtOptions.Lifetime)

            });
            //return Ok(new { token = token, userRole = userRole });
            return Redirect("http://localhost:4200/home");


        }
    }
}