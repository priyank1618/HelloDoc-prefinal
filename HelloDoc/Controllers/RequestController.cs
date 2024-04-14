using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModel;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HelloDoc.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPatient_Request _request;
        private readonly IFamily_Request _Family_Request;
        private readonly IConcierge_Request _concierge;
        private readonly IBusiness_Request _business;
        private readonly IAddFile _file;
        private readonly IHostingEnvironment _environment;
        private readonly IEmailService _emailService;



        //-----------------------Add Context---------------------------------

        public RequestController(ApplicationDbContext context, IPatient_Request patient_Request, IFamily_Request Family_Req,
            IConcierge_Request concierge, IBusiness_Request business_Request, IAddFile file, 
            IHostingEnvironment hostingEnvironment, IEmailService emailService)
        {
            _context = context;
            _request = patient_Request;
            _Family_Request = Family_Req;
            _concierge = concierge;
            _business = business_Request;
            _file = file;
            _environment = hostingEnvironment;
            _emailService = emailService;
        }

        //-----------------------Add Context-------------------------------------

        //----------------Patient Request----------------------------
        //verify email
        [HttpPost]
        public IActionResult CheckEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            return Json(new { exists = user != null });
        }

        public IActionResult Patient_Request()
        {
            Patient patient = new Patient();
            patient.regions = _context.Regions.ToList();
            return View(patient);
        }



        [HttpPost]
        public IActionResult Patient_Request(Patient patient)
        {
            if (ModelState.IsValid)
            {
                _request.AddPatient(patient);
                if (patient.Filedata != null)
                {
                    string path = Path.Combine(this._environment.WebRootPath, "Files");
                    var uniquefilesavetoken = new Guid().ToString();

                    string fileName = Path.GetFileName(patient.Filedata.FileName);
                    fileName = $"{uniquefilesavetoken}_{fileName}";
                    _file.AddFile(patient.Filedata, path, fileName);

                    var Request = _request.GetUserByEmail(patient.Email);
                    _request.RequestWiseFile(fileName, Request.RequestId);

                }
                return RedirectToAction("Patient_Request");
            }
            patient.regions = _context.Regions.ToList();
            return View("Patient_Request", patient);
        }
        //----------------Patient Request----------------------------


        //--------------------Family/Friend----------------------------------------
        public IActionResult Family_Friend_Request()

        {
            Other_Request other_Request = new Other_Request();
            other_Request.regions = _context.Regions.ToList();
            return View(other_Request);

        }

        [HttpPost]
        public IActionResult Family_Friend_Request(Other_Request other_Reqs)

        {
            var subject = "Creat Patient Request";
            var formLink = Url.ActionLink("Create_Patient", "Home", new { email = other_Reqs.Email_P }, protocol: HttpContext.Request.Scheme);
            var present = _context.AspNetUsers.FirstOrDefault(s => s.Email == other_Reqs.Email_P);
            if (ModelState.IsValid)
            {
                _Family_Request.AddData(other_Reqs);
                if (formLink != null && present == null)
                {
                    //send mail on the other_request.Email_p
                    _emailService.SendEmail("patelpriyank3112002@gmail.com",subject,
                   $"<a href='{formLink}'>Click here </a> for Request");
                }
                if (other_Reqs.Filedata != null)
                {
                    string path = Path.Combine(this._environment.WebRootPath, "Files");
                    var uniquefilesavetoken = new Guid().ToString();

                    string fileName = Path.GetFileName(other_Reqs.Filedata.FileName);
                    fileName = $"{uniquefilesavetoken}_{fileName}";
                    _file.AddFile(other_Reqs.Filedata, path, fileName);
                    var Request = _request.GetUserByEmail(other_Reqs.Email_P);
                    _request.RequestWiseFile(fileName, Request.RequestId);

                }
               return RedirectToAction("Family_Friend_Request");
            }
            other_Reqs.regions = _context.Regions.ToList();
            return View("Family_Friend_Request", other_Reqs);
        }
        //--------------------Family/Friend----------------------------------------

        //--------------------Concierge---------------------------------------

        public IActionResult Concierge_Request()
        {
            Other_Request other_Request = new Other_Request();
            other_Request.regions = _context.Regions.ToList();
            return View(other_Request);
        }

        [HttpPost]
        public IActionResult Concierge_Request(Other_Request other_Reqs)

        {
            var subject = "Creat Patient Request";
            var formLink = Url.ActionLink("Create_Patient", "Home", new { email = other_Reqs.Email_P }, protocol: HttpContext.Request.Scheme);
            var present = _context.AspNetUsers.FirstOrDefault(s => s.Email == other_Reqs.Email_P);
            if (ModelState.IsValid)
            {
               _concierge.AddData(other_Reqs);
                if (formLink != null && present != null)
                {
                    //send mail on the other_request.Email_p
                    _emailService.SendEmail("patelpriyank3112002@gmail.com", subject,
                   $"<a href='{formLink}'>Click here </a> for Request");
                }
                return RedirectToAction("Concierge_Request");

            }
            other_Reqs.regions = _context.Regions.ToList();
            return View("Family_Friend_Request", other_Reqs);
        }
        //--------------------Concierge---------------------------------------

        //--------------------Business Request---------------------------------------

        public IActionResult Business_Request()

        {
            Other_Request other_Request = new Other_Request();
            other_Request.regions = _context.Regions.ToList();
            return View(other_Request);

        }

        [HttpPost]
        public IActionResult Business_Request(Other_Request other_Reqs)
        {
            var subject = "Creat Patient Request";
            var formLink = Url.ActionLink("Create_Patient", "Home",new {email = other_Reqs.Email_P}, protocol: HttpContext.Request.Scheme);
            var present = _context.AspNetUsers.FirstOrDefault(s => s.Email == other_Reqs.Email_P);
            if (ModelState.IsValid)
            {
             _business.addbusinessdata(other_Reqs);
                if (formLink != null && present != null)
                {
                    //send mail on the other_request.Email_P if its not exist in Db
                    _emailService.SendEmail("patelpriyank3112002@gmail.com", subject,
                   $"<a href='{formLink}'>Click here </a> for Request");
                }
                return RedirectToAction("Business_Request");
            }
            other_Reqs.regions = _context.Regions.ToList() ;
            return View("Family_Friend_Request", other_Reqs);
        }


        public IActionResult ReviewAgreement(int requestid)
        {

            var request = _context.Requests.FirstOrDefault(s => s.RequestId == requestid);
            var name = _context.RequestClients.FirstOrDefault(s => s.RequestId == requestid);


            ViewBag.requestid = requestid;
            ViewBag.name = name.FirstName + " " + name.LastName;

            if (request.Status == 2)
            {
                return View();
            }
            else
            {
              return  RedirectToAction("Patient_login", "Login");
            }
        
        }

        public IActionResult Agree(int id)
        {
            var request = _context.Requests.FirstOrDefault(s => s.RequestId == id);

            if (request != null)
            {
                request.Status = 4;
                request.ModifiedDate = DateTime.Now;

                _context.Update(request);
                _context.SaveChanges();

                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = id;
                requestStatusLog.CreatedDate = DateTime.Now;
                requestStatusLog.Status = 4;
                _context.Add(requestStatusLog);
                _context.SaveChanges();

            }
           return  RedirectToAction("Patient_login", "Login");
        }

        public IActionResult CancelAgreement(string Notes, int id)
        {
            var req = _context.Requests.FirstOrDefault(s => s.RequestId == id);

            if (req != null)
            {
                req.Status = 7;
                req.ModifiedDate = DateTime.Now;
                _context.Update(req);
                _context.SaveChanges();

                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.Notes = Notes;
                requestStatusLog.RequestId = id;
                requestStatusLog.CreatedDate = DateTime.Now;
                requestStatusLog.Status = 7;
                _context.Add(requestStatusLog);
                _context.SaveChanges();

                return RedirectToAction("Patient_login", "Login");

            }
            return BadRequest();
        }

        //--------------------Business Request---------------------------------------
    }
}