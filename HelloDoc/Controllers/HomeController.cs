using HelloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using DAL.DataContext;
using DAL.DataModels;
using System.Drawing;
using System.Security.Principal;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using DAL.ViewModel;
using BAL.Interface;

namespace HelloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IHome _home;

        public object AspNetUsers { get; private set; }

        public HomeController(ApplicationDbContext context,IHome home)
        {
            _context = context;
            _home = home;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult  Patient_submit_request()

        {
            return View();
        }  
        public IActionResult  Patient_site()

        {
            return View();
        }
          
        public IActionResult  Create_Patient(string email)
        {   
            CreateAccount createAccount = new CreateAccount();
            createAccount.Email = email;    
            return View(createAccount);
        }

        [HttpPost]
        public IActionResult  Create_Patient(CreateAccount createAccount)
        {   
            if(ModelState.IsValid)
            { 
                if(createAccount.Password != createAccount.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty,"Password and confirmpassword doesn't match");
                    return View(createAccount);
                }
                
                 _home.AddData(createAccount);
               return  RedirectToAction("Patient_login","Login");
            }
            return View(createAccount);
        }

      
        public IActionResult AccessDenied()
        {
            return View();  
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
