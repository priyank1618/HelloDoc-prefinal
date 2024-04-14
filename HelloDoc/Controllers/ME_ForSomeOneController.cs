using AspNetCoreHero.ToastNotification.Abstractions;
using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HelloDoc.Controllers
{
    


    public class ME_ForSomeOneController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IPatient_Request _request;
        private readonly INotyfService _notyf;
      



        //-----------------------Add Context---------------------------------

        public ME_ForSomeOneController(ApplicationDbContext context, IPatient_Request patient_Request,INotyfService notyf)
        {
            _context = context;
            _request = patient_Request;
                _notyf = notyf;
         
        }
        public IActionResult Me()
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            var patient = new Patient
            {
                Email=email,
                FirstName = user.FirstName, 
                LastName = user.LastName,
                PhoneNumber = user.Mobile
            };

            return View(patient);
        }

        [HttpPost]
        public IActionResult Me(Patient patient)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if(ModelState.IsValid)
            {
             _request.AddPatient(patient);
             _notyf.Success("Data Added Successfully");
             return RedirectToAction("Me");

            }
          
            return View();
        } 
        
        
        
        public IActionResult SomeOne()
        {
            return View();
        }
    }
}
