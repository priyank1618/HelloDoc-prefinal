using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModel;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static BAL.Repository.Authorizationrepo;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HelloDoc.Controllers
{

   
    [CustomAuthorize(new string[] { "Patient" })]
    public class DashBoardController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;
        private readonly IAddFile _files;
        private readonly IPatient_Request _patient;

        public DashBoardController(ApplicationDbContext context,IHostingEnvironment environment , IAddFile files,IPatient_Request patient)
        {
            _context = context;
            _environment = environment;
            _files = files;
            _patient = patient;
        }


        public IActionResult Index()
       {
            
            var Email = HttpContext.Session.GetString("Email");
            var mail = _context.Users.FirstOrDefault(u => u.Email == Email);
   
            if (mail == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.username = mail.FirstName + " " + mail.LastName;
            }

            var result = from req in _context.Requests 
                         join reqclient in _context.RequestClients 
                         on req.RequestId equals reqclient.RequestId
                         join requestfile in _context.RequestWiseFiles 
                         on req.RequestId equals requestfile.RequestId                        
                         into reqs
                         from requestfile in reqs.DefaultIfEmpty()
                         where reqclient.Email == Email

                         select new Patient_Dash
                         {
                             CreatedDate = req.CreatedDate,
                             CurrentStatus = req.Status,
                             FilePath = requestfile.FileName != null ? requestfile.FileName : null,
                             requestid = req.RequestId,
                             count = _context.RequestWiseFiles.Count(u => u.RequestId == req.RequestId),
                         };

            return View(result.ToList());

        }

        public IActionResult viewDocs(int requestid)
        {
            var Email = HttpContext.Session.GetString("Email");
            var mail = _context.Users.FirstOrDefault(u => u.Email == Email);
            var reque = _context.RequestWiseFiles.Where(u => u.RequestId == requestid).ToList();
            if (mail == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.username = mail.FirstName + " " +mail.LastName;
            }
            var result = new ViewDoc
            {
                requestwisefile = reque,
                requestid = requestid,
                confirmationnum = _context.Requests.FirstOrDefault(s => s.RequestId == requestid).ConfirmationNumber,
            };
            return View(result);
        }



        [HttpPost]
        public IActionResult uploadfile(int reqid)
        {
            var file = Request.Form.Files["file"];
            var uniquefilesavetoken = new Guid().ToString();

            string fileName = Path.GetFileName(file.FileName);
            fileName = $"{fileName}_{uniquefilesavetoken}";
            string path = Path.Combine(_environment.WebRootPath, "Files");
            _files.AddFile(file, path, fileName);

            _patient.RequestWiseFile(fileName, reqid);
            return RedirectToAction("viewDocs", new { requestid = reqid} );

        }

       
       

        public  IActionResult  Patient_Profile()
        {
            var Email = HttpContext.Session.GetString("Email");
            var mail = _context.Users.FirstOrDefault(u => u.Email == Email);

            if (mail == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.username = mail.FirstName + " " + mail.LastName;
            }

            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            var Patient_Profile = new Patient_Profile();
            Patient_Profile.FirstName = user.FirstName;
            Patient_Profile.Email = user.Email;
            Patient_Profile.LastName = user.LastName;
            Patient_Profile.PhoneNumber = user.Mobile;
           
            Patient_Profile.Street = user.Street;
            Patient_Profile.City = user.City;
            Patient_Profile.State = user.State;
            Patient_Profile. ZipCode = user.ZipCode;

        
             
            return View(Patient_Profile);
        }

        [HttpPost]
        public  IActionResult  Patient_Profile(Patient_Profile patient_Profile)
        {

            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
              

            if(ModelState.IsValid)
            {

                user.FirstName = patient_Profile.FirstName;
                user.LastName = patient_Profile.LastName;
                user.Mobile = patient_Profile.PhoneNumber;
                user.Street = patient_Profile.Street;
                user.City = patient_Profile.City;
                user.State = patient_Profile.State;
                user.IntDate = patient_Profile.BirthDate.Value.Day;
                user.IntYear = patient_Profile.BirthDate.Value.Year;
                user.StrMonth = (patient_Profile.BirthDate.Value.Month).ToString();
                user.ZipCode = patient_Profile.ZipCode;

                _context.Update(user);
                _context.SaveChanges();
                return RedirectToAction("Patient_Profile");
            }
            else
            {
                return View(patient_Profile);
            }

          
                
        }
    }
}





      
       
        
