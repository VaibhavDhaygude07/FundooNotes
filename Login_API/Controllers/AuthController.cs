using FundooNotes.Business.DTO;
using FundooNotes.Business.Interfaces;
using FundooNotes.Data.Entity;
using Login_API.Business;
using Login_API.Business.Interfaces;
using Login_API.Data;
using Login_API.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LoginAPI.API.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IEmailService _emailService; // Fixed Email Service Injection

        public AuthController(UserDbContext context, JwtTokenService jwtTokenService, IEmailService emailService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                return BadRequest("User already exists");
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email);
            return Ok(new { Token = token });
        }

        //[HttpPost("ForgotPassword")]
        //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        //    if (user == null)
        //        return NotFound("User not found");

        //    // Generate Reset Token
        //    user.ResetToken = _jwtTokenService.GenerateToken(user.Id, user.Email);
        //    user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour
        //    await _context.SaveChangesAsync();

        //    // Create Reset Password Link
        //    var resetLink = $"https://yourdomain.com/reset-password?token={user.ResetToken}";

        //    // Send Email with Reset Link
        //    await _emailService.SendEmailAsync(user.Email, resetLink);


        //    return Ok(new { Message = "Password reset link sent successfully" });
        //}

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return NotFound("User not found");

            // Generate Reset Token
            user.ResetToken = _jwtTokenService.GenerateToken(user.Id, user.Email);
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour
            await _context.SaveChangesAsync();

            // Create Reset Password Link
            var resetLink = $"http://localhost:5021/api/reset-password?token={user.ResetToken}";

            // Log Reset Link
            Console.WriteLine($"Reset Link: {resetLink}");

            // Send Email with Reset Link
            var emailResult = await _emailService.SendEmailAsync(user.Email, resetLink);
            Console.WriteLine($"Email sent result: {emailResult}");

            return Ok(new { Message = "Password reset link sent successfully" });
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == request.Token && u.ResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
                return BadRequest("Invalid or expired token");

            // Hash the new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // Remove Token after use
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Password has been reset successfully" });
        }

       
    }
}
