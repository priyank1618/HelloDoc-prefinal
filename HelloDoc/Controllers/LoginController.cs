using AspNetCoreHero.ToastNotification.Abstractions;
using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace HelloDoc.Controllers
{

    public class LoginController : Controller
    {


        private readonly ApplicationDbContext _context;

        private readonly IEmailService _emailService;
        private readonly IPasswordHasher<Patient_login> _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly INotyfService _notyf;
        public LoginController(ApplicationDbContext context, IEmailService emailService,
            IPasswordHasher<Patient_login> passwordHasher, IJwtService jwtService, INotyfService notyf)
        {
            _context = context;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _notyf = notyf;
        }
        public IActionResult Patient_login()

        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Patient_login(Patient_login patient)
        {


            if (ModelState.IsValid)
            {
                var Email = _context.AspNetUsers.FirstOrDefault(m => m.Email == patient.Email);
                var user = _context.AspNetUserRoles.FirstOrDefault(i => i.UserId == Email.AspNetUserId);
                var role = _context.AspNetRoles.FirstOrDefault(k => k.AspNetRoleId == user.RoleId).Name.Trim();
                var result = _passwordHasher.VerifyHashedPassword(null, Email.PasswordHash, patient.PasswordHash);
                bool verifiedpassword = result == PasswordVerificationResult.Success;

                if (Email != null && verifiedpassword)
                {

                    HttpContext.Session.SetString("Email", patient.Email);
                    HttpContext.Session.SetString("Role", role);
                    var jwt = _jwtService.Generatetoken(patient.Email, role);
                    Response.Cookies.Append("jwt", jwt);
                    if (role == "Patient")
                    {
                      
                        _notyf.Success("Successfully Login");
                        return RedirectToAction("Index", "DashBoard");
                    }
                    else if (role == "Admin")
                    {
                        _notyf.Success("Successfully Login");
                        return RedirectToAction("AdminDash", "AdminDash");
                    }
                    else if(role == "Physician")
                    {
                        _notyf.Success("Successfully Login");
                        return RedirectToAction("ProviderDashBoard", "ProviderDashBoard");
                    }
                }
                else if(Email == null || verifiedpassword == false)
                {
                    _notyf.Error("Invalid Email OR Password");
                }
            }

          
            return View(patient);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Patient_login");
        }

        public IActionResult Patient_ResetPassword()

        {
            return View();
        }


        //generate the token and validate the token
        public string GenerateToken(string mail)
        {
            Guid guid = Guid.NewGuid();
            DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            string currentTimeString = currentTime.ToString("yyyy-MM-ddTHH:mm:sszzz");
            string tokenData = $"{guid}_{mail}_{currentTimeString}";
            byte[] tokenBytes = Encoding.UTF8.GetBytes(tokenData);
            string token = Convert.ToBase64String(tokenBytes);
            return token;
        }

        public (bool isValid, string mail) ValidateToken(string token)
        {

            byte[] decodedTokenBytes = Convert.FromBase64String(token);
            string decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);


            if (string.IsNullOrEmpty(decodedToken))
                return (false, null);

            string[] tokenParts = decodedToken.Split('_');
            Console.WriteLine(tokenParts);
            if (tokenParts.Length != 3)
                return (false, null);

            // Extract timestamp and requestId from the token
            string mail = tokenParts[1];
            DateTimeOffset timestamp;
            if (!DateTimeOffset.TryParse(tokenParts[2], out timestamp))
                return (false, null);

            // Check if token has expired
            DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            if ((currentTime - timestamp).TotalHours > 2)
                return (false, null);

            return (true, mail);
        }


        //--------------------------------------    
        [HttpPost]
        public IActionResult Patient_ResetPassword(Patient_ResetPassword patient_ResetPassword)

        {

            var mail = patient_ResetPassword.Email;
            var token = GenerateToken(mail);
            var subject = "Change the Password";
            var agreementLink = Url.Action("ResetPasswordPage", "Login", new { token = token }, protocol: HttpContext.Request.Scheme);


            if (ModelState.IsValid)
            {

                if (agreementLink != null)
                {
                    _emailService.SendEmail("patelpriyank3112002@gmail.com", subject,
                        $"<a href='{agreementLink}'>Click here </a> for Change Your Password");
                }
            }

            return RedirectToAction("Patient_ResetPassword");


        }

        public IActionResult ResetPasswordPage(string token, bool show = false)
        {
            var (isValid, mail) = ValidateToken(token);
            if (!isValid)
            {

                return RedirectToAction("ExpiredLink");
            }

            else
            {
                Patient_ResetPassword patient = new Patient_ResetPassword();
                ViewBag.ShowAlert = show;
                patient.Email = mail;
                patient.token = token;
                return View(patient);

            }
        }

        [HttpPost]
        public IActionResult ResetPasswordPage(Patient_ResetPassword patient_ResetPassword, string token, string mail)
        {

            if (patient_ResetPassword.Password == patient_ResetPassword.ConfirmPassword)
            {
                var Email = _context.AspNetUsers.FirstOrDefault(s => s.Email == mail);
                //save chnages in the db
                return RedirectToAction("Patient_login");
            }

            else
            {
                Console.WriteLine("Password and Confirm Password do not match!");
                return RedirectToAction("ResetPasswordPage", new { token = token, show = true });

            }
        }



       
    }


}