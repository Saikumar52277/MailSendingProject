using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MailSendingProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OtpController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("generate")]
        public IActionResult GenerateOtp([FromBody] OtpRequest request)
        {
            string otp = GenerateRandomOtp(6);
            SendOtpByEmail(request.Email, otp);
            return Ok(new { Message = "OTP generated and sent successfully." });
        }

        [HttpPost("validate")]
        public IActionResult ValidateOtp([FromBody] OtpValidationRequest request)
        {
            bool isValid = ValidateEnteredOtp(request.Email, request.EnteredOtp);

            if (isValid)
            {
                return Ok(new { Message = "OTP is valid." });
            }
            else
            {
                return BadRequest(new { Message = "Invalid OTP." });
            }
        }
        private string GenerateRandomOtp(int length)
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[length];
                rng.GetBytes(tokenData);

                int otpValue = BitConverter.ToInt32(tokenData, 0);
                return (otpValue % (int)Math.Pow(10, length)).ToString("D6");
            }
        }

        private void SendOtpByEmail(string email, string otp)
        {
            string smtpServer = _configuration["SmtpSettings:Server"];
            int smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
            string smtpUsername = _configuration["SmtpSettings:Username"];
            string smtpPassword = _configuration["SmtpSettings:Password"];

            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUsername),
                    Subject = "OTP Verification",
                    Body = $"Your OTP is: {otp}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(email);

                smtpClient.Send(mailMessage);
            }
        }

        private bool ValidateEnteredOtp(string email, string enteredOtp)
        {
            return string.Equals(enteredOtp, "SavedOtpFromDatabase", StringComparison.OrdinalIgnoreCase);
        }
    }
}

    public class OtpRequest
    {
        public string Email { get; set; }
    }

    public class OtpValidationRequest
    {
        public string Email { get; set; }
        public string EnteredOtp { get; set; }
    }


