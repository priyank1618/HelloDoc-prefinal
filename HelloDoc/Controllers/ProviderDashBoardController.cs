using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using static BAL.Repository.Authorizationrepo;

namespace HelloDoc.Controllers
{
    [CustomAuthorize(new string[] {"Physician"})]
    public class ProviderDashBoardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProviderDashBoard _providerDashBoard;
        private readonly IAdminDashBoard _AdminDashBoard;
        private readonly IAdminAction _adminAction;
        private readonly IEmailService _emailService;

        public ProviderDashBoardController(ApplicationDbContext context,IProviderDashBoard providerDashBoard,IAdminDashBoard adminDashBoard,IAdminAction adminAction,IEmailService emailService) { 
           

           _context = context;
            _providerDashBoard = providerDashBoard;
            _AdminDashBoard = adminDashBoard;
            _adminAction = adminAction;
            _emailService = emailService;
        }
     
        public IActionResult ProviderDashBoard()
        {
            var email = HttpContext.Session.GetString("Email");
            int phyId = 0;
            if (email != null)
            {
                ViewBag.username = _context.AspNetUsers.First(u => u.Email == email).UserName;
                phyId = _context.Physicians.First(u => u.Email == email).PhysicianId;
            }
            var DashData = _AdminDashBoard.GetList();

            var dashboardData = _providerDashBoard.GetCount(phyId);

            // Set ViewBag properties
            ViewBag.NewCount = dashboardData.NewCount;
            ViewBag.PendingCount = dashboardData.PendingCount;
            ViewBag.ActiveCount = dashboardData.ActiveCount;
            ViewBag.Conclude = dashboardData.Conclude;
            ViewBag.ToClosed = dashboardData.ToClosed;
            ViewBag.Unpaid = dashboardData.Unpaid;

            return View("DashBoard/ProviderDashBoard",DashData.ToList());
       
        }

        public JsonResult CheckSession()
        {
            var request = HttpContext.Request;
            var token = request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { sessionExists = false });
            }
            else
            {
                return Json(new { sessionExists = true });
            }
        }

        public IActionResult SearchPatient(string SearchValue, string Filterselect,
           string selectvalue, string partialName, int[] currentstatus, int currentpage, int pagesize)
        {
            var email = HttpContext.Session.GetString("Email");
            int phyId = 0;
            if (email != null)
            {
                 phyId = _context.Physicians.First(u => u.Email == email).PhysicianId;
            }

            var FilterData = _AdminDashBoard.GetRequestDataPhy(SearchValue, Filterselect, selectvalue,
            partialName, currentstatus, phyId).ToList();

            int totalItems = FilterData.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            if (SearchValue != null || selectvalue != null || Filterselect != null)
            {
                if (totalPages <= 1)
                {
                    currentpage = 1;
                }
            }
            var paginatedData = FilterData.Skip((currentpage - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentpage;
            return PartialView(partialName, paginatedData);

        }

        public IActionResult Accept(int id)
        {
            var user = _context.Requests.FirstOrDefault(h => h.RequestId == id);

            if (user != null)
            {
                user.Status = 2;
                user.ModifiedDate = DateTime.Now;
             
                _context.Update(user);
                _context.SaveChanges();

                RequestStatusLog requeststatuslog = new RequestStatusLog();

                requeststatuslog.RequestId = id;
                requeststatuslog.Status = 2;
                requeststatuslog.CreatedDate = DateTime.Now;


                _context.Add(requeststatuslog);
                _context.SaveChanges();

            }

            return RedirectToAction("ProviderDashBoard");
        }


        public IActionResult TransferCase(int transferid, string Descriptionoftra)
        {
            var user = _context.Requests.FirstOrDefault(h => h.RequestId == transferid);

            if (user != null)
            {
                user.Status = 1;
                user.PhysicianId = null;
                user.ModifiedDate = DateTime.Now;
                _context.Update(user);
                _context.SaveChanges();

                RequestStatusLog requeststatuslog = new RequestStatusLog();
                requeststatuslog.RequestId = transferid;
                requeststatuslog.Notes = Descriptionoftra;
                requeststatuslog.CreatedDate = DateTime.Now;
                requeststatuslog.Status = 1;
                _context.Add(requeststatuslog);
                _context.SaveChanges();

            }

            return Ok();
        }


        public IActionResult SendAgreement(int sendagreementid)
        {
            var agreementLink = Url.Action("ReviewAgreement", "Request", new { requestid = sendagreementid }, protocol: HttpContext.Request.Scheme);
            var subject = "Acceptance of the agreement";

            if (agreementLink != null)
            {
                _emailService.SendEmail("patelpriyank3112002@gmail.com", subject,
                    $"<a href='{agreementLink}'>Click here </a> for further procedure");
                return Ok();
            }

            return BadRequest();
        }

       public IActionResult HouseCall(int requestid)
        {
            var  user = _context.Requests.FirstOrDefault(s => s.RequestId == requestid);

            if (user != null)
            {
                user.Status = 5;
                _context.Update(user) ;
                _context.SaveChanges();
            }
            return RedirectToAction("ProviderDashBoard");
        }
        
        public IActionResult Consult(int requestid)
        {
            var  user = _context.Requests.FirstOrDefault(s => s.RequestId == requestid);

            if (user != null)
            {
                user.Status = 6;
                _context.Update(user);
                _context.SaveChanges();
            }
            return RedirectToAction("ProviderDashBoard");
        }

        public IActionResult HouseCalled(int id)
        {
            var user = _context.Requests.FirstOrDefault(s => s.RequestId == id);

            if (user != null)
            {
                user.Status = 6;
                _context.Update(user);
                _context.SaveChanges();
            }
            return RedirectToAction("ProviderDashBoard");
        }

        public IActionResult ConcludeCare()
        {

            return View("DashBoard/ConcludeCare");
        }


    }
}
