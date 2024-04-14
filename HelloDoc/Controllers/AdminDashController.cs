
using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Net.Mail;
using static BAL.Repository.Authorizationrepo;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Identity;
using DAL.DataModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.Controllers
{
    public class AdminDashController : Controller
    {

		#region variables
		private readonly ApplicationDbContext _context;
        private readonly IAdminDashBoard _AdminDashboard;
        private readonly IHostingEnvironment _environment;
        private readonly IAddFile _files;
        private readonly IPatient_Request _patient;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher<AdminProfile> _passwordHasher;
        private readonly INotyfService _notyf;
        private readonly IUploadProvider _uploadProvider;
        private readonly IAdminAction _adminAction;
        private readonly IAccountsAccess _acc;
        private readonly IAdminDashboardRecords _adminDashboardRecords;


        public AdminDashController(ApplicationDbContext context, IAdminDashBoard adminDashboard, IHostingEnvironment environment, IAddFile files, IPatient_Request patient, IEmailService emailService,
            IPasswordHasher<AdminProfile> passwordHasher,
            INotyfService notyf, IUploadProvider uploadProvider,
            IAdminAction adminAction, IAccountsAccess accountsAccess, IAdminDashboardRecords adminDashboardRecords)
        {
            _context = context;
            _AdminDashboard = adminDashboard;
            _environment = environment;
            _files = files;
            _patient = patient;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _notyf = notyf;
            _uploadProvider = uploadProvider;
            _adminAction = adminAction;
            _acc = accountsAccess;
            _adminDashboardRecords = adminDashboardRecords;
        }

		#endregion

		#region AdminDashboard
		public IActionResult AdminDash()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email != null)
            {
                ViewBag.username = _context.AspNetUsers.First(u => u.Email == email).UserName;
            }
            var DashData = _AdminDashboard.GetList();

            var dashboardData = _AdminDashboard.GetCount();

            // Set ViewBag properties
            ViewBag.NewCount = dashboardData.NewCount;
            ViewBag.PendingCount = dashboardData.PendingCount;
            ViewBag.ActiveCount = dashboardData.ActiveCount;
            ViewBag.Conclude = dashboardData.Conclude;
            ViewBag.ToClosed = dashboardData.ToClosed;
            ViewBag.Unpaid = dashboardData.Unpaid;

            return View(DashData.ToList());
        }


        public IActionResult SearchPatient(string SearchValue, string Filterselect,
            string selectvalue, string partialName, int[] currentstatus, int currentpage, int pagesize)
        {

            var FilterData = _AdminDashboard.GetRequestData(SearchValue, Filterselect, selectvalue,
            partialName, currentstatus).ToList();

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
		#endregion AdminDashboard

		#region viewcase,viewnotes,cancel,assign,teansfer,block,viewupload,ClearCase

		[CustomAuthorize(new string[] { "Admin", "Physician" })]
        [HttpGet("ProviderDashBoard/ViewCase/{id}", Name = "ProviderCase")]
        [HttpGet("AdminDash/ViewCase/{id}/{status}", Name = "AdminCase")]
        public IActionResult ViewCase(int id, int status)
        {
            string email = HttpContext.Session.GetString("Email");
           
            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);

            if (admin != null)
            {
                ViewBag.IsPhysican = false;
                
            }
            else
            {
                ViewBag.IsPhysican = true;
                
            }
            var ViewCase = _adminAction.ViewCase(id, status);
            return View(ViewCase);
        }

		public IActionResult ClearCase(int clearcaseid)
		{
			_adminAction.ClearCase(clearcaseid);
			return Ok();
		}

		[CustomAuthorize(new string[] { "Admin", "Physician" })]
        [HttpGet("ProviderDashBoard/ViewNotes/{id}", Name = "ProviderNotes")]
        [HttpGet("AdminDash/ViewNotes/{id}", Name = "AdminCaseNotes")]
        public ActionResult ViewNotes(int id)
        {

            //if admin add notes in transfer request that will added in the requeststatus log
            //tables notes it will known as the transfer notes

            string email = HttpContext.Session.GetString("Email");

            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            if (admin != null)
            {
                ViewBag.IsPhysican = false;

            }
            else
            {
                ViewBag.IsPhysican = true;

            }
            var result = _AdminDashboard.GetViewNotes(id).ToList();
            return View(result);
        }


        public ActionResult CancelCase(int Requestid, string Reason, string Notes)
        {
            if (_adminAction.CancelCase(Requestid, Reason, Notes))
            {
                return RedirectToAction("AdminDash");
            }
            return Ok();
        }

        public IActionResult GetPhysician(string regionid)
        {
            var result = (from physician in _context.Physicians
                          join
                           region in _context.PhysicianRegions on
                           physician.PhysicianId equals region.PhysicianId into phy
                          select physician).Where(s => s.RegionId == int.Parse(regionid)).ToList();
            return Json(result);
        }


        public IActionResult AssignCase(int req, string Description, string phyid)
        {
            _adminAction.AssignCase(req, Description, phyid);
            return Ok();
        }


        public IActionResult TransferCase(int transferid, string Descriptionoftra, string phyidtra)
        {
            _adminAction.TransferCase(transferid, Descriptionoftra, phyidtra);
            return Ok();
        }
        public IActionResult GetPhysicianForTransfer(string regionid)
        {
            var result = (from physician in _context.Physicians
                          join
                           region in _context.PhysicianRegions on
                           physician.PhysicianId equals region.PhysicianId into phy
                          select physician).Where(s => s.RegionId == int.Parse(regionid)).ToList();
            return Json(result);
        }

        public IActionResult BlockCase(int blocknameid, string blocknotes)
        {
            _adminAction.BlockCase(blocknameid, blocknotes);
            return Ok();
        }

        [CustomAuthorize(new string[] { "Admin", "Physician" })]
        [HttpGet("ProviderDashBoard/ViewUpload/{id}", Name = "ProviderUploads")]
        [HttpGet("AdminDash/ViewUpload/{id}", Name = "AdminCaseNotesUploads")]
        public IActionResult ViewUpload(int id)
        {
            string email = HttpContext.Session.GetString("Email");

            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            if (admin != null)
            {
                ViewBag.IsPhysican = false;

            }
            else
            {
                ViewBag.IsPhysican = true;

            }
            bool[] bitValues = { true };
            BitArray bits = new BitArray(bitValues);
            var reque = _context.RequestWiseFiles.Where(u => u.RequestId == id && u.IsDeleted != bits).ToList();

            var result = new ViewDoc
            {
                requestwisefile = reque,
                requestid = id,
                //confirmationnum = _context.Requests.FirstOrDefault(s => s.RequestId == requestid).ConfirmationNumber,
            };

            return View(result);
        }

		#endregion viewcase,viewnotes,cancel,assign,teansfer,block,viewupload

		#region closecase,closecaseinstance

		public IActionResult CloseCase(int requestid)
		{
			var closecase = _adminAction.CloseCase(requestid);
			return View(closecase);
		}



		[HttpPost]
		public IActionResult CloseCase(CloseCase closeCase, int id)
		{
			_adminAction.CloseCasePost(closeCase, id);
			return RedirectToAction("CloseCase", new { requestid = id });
		}

		public IActionResult CloseInstance(int reqid)
		{
			if (_adminAction.CloseInstance(reqid))
			{
				return RedirectToAction("AdminDash");
			}
			else
			{
				return RedirectToAction("CloseCase", new { requestid = reqid });
			}
		}

		#endregion

		#region Uploadfile,deleteindividual,deleteselected,sendselected

		[HttpPost]
        public IActionResult uploadfile(int reqid)
        {
            var file = Request.Form.Files["file"];
            var uniquefilesavetoken = Guid.NewGuid().ToString();

            string fileName = Path.GetFileName(file.FileName);
            fileName = $"{uniquefilesavetoken}_{fileName}";
            string path = Path.Combine(_environment.WebRootPath, "Files");
            _files.AddFile(file, path, fileName);

            _patient.RequestWiseFile(fileName, reqid);
            return RedirectToAction("ViewUpload", new { id = reqid });

        }


        public IActionResult DeleteFile(int reqid, string name)
        {

            string path = Path.Combine(_environment.WebRootPath, "Files", name);
            // _files.RemoveFile(path);

            RequestWiseFile reqFile = _context.RequestWiseFiles.Where(x => x.FileName == name).FirstOrDefault();
            if (reqFile != null)
            {
                bool[] bitValues = { true };
                BitArray bits = new BitArray(bitValues);
                reqFile.IsDeleted = bits;
                _context.Update(reqFile);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewUpload", new { id = reqid });
        }
        //---------------------------DeleteSelectedFiles----------------------

        [HttpPost]
        public IActionResult DeleteSelectedFiles(List<string> filenames)
        {

            try
            {
                foreach (var filename in filenames)
                {
                    var file = _context.RequestWiseFiles.FirstOrDefault(item => item.FileName == filename);

                    if (file != null)
                    {
                        file.IsDeleted = new BitArray(new[] { true });
                        _context.SaveChanges();
                    }
                }

                return Ok(new { message = "Files delete successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting files: {ex.Message}" });
            }
        }
        //---------------------------DeleteSelectedFiles----------------------


        [HttpPost]
        public IActionResult SendFiles(List<string> filenames, int id)
        {

            try
            {

                List<Attachment> files = new List<Attachment>();

                if (filenames != null)
                {
                    foreach (var filename in filenames)
                    {
                        string path = Path.Combine(_environment.WebRootPath, "Files", filename);
                        var attach = new Attachment(path);
                        files.Add(attach);
                    }
                    var req = _context.Requests.FirstOrDefault(s => s.RequestId == id);

                    var subject = "Uploaded Documents";
                    var body = "Docs";

                    _emailService.SendEmail("patelpriyank3112002@gmail.com", subject, body, files);
                    _notyf.Success("Email sent successfully");
                    _emailService.EmailLog("template", subject, req.Email, id);
                }
                return Ok(new { message = "Files Send successfully" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error Sending  files: {ex.Message}" });
            }
        }

		#endregion Uploadfile,deleteindividual,deleteselected,sendselected

		#region CheckSession
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

		#endregion

		#region sendorder,sendagreement
		[CustomAuthorize(new string[] { "Admin", "Physician" })]
        [HttpGet("AdminDash/SendOrder/{id}", Name = "AdminCaseNotesorder")]
        [HttpGet("ProviderDashBoard/SendOrder/{id}", Name = "Providerorder")]
        public IActionResult SendOrder(int id)
        {
            bool controllerName = Request.Path.ToString().Contains("ProviderDashBoard");
            string email = HttpContext.Session.GetString("Email");

            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            if (admin != null)
            {
                ViewBag.IsPhysican = false;

            }
            else
            {
                ViewBag.IsPhysican = true;

            }
            var Profession = _context.HealthProfessionalTypes.ToList();

            SendOrder sendOrder = new SendOrder();

            sendOrder.ProfessionName = Profession;
            sendOrder.requestid = id;
            return View(sendOrder);
        }


        [CustomAuthorize(new string[] {"Admin", "Physician"})]
        [HttpPost("AdminDash/SendOrder/{id}", Name = "AdminCaseNotesorderr")]
        [HttpPost("ProviderDashBoard/SendOrder/{id}", Name = "Providerorderr")]
        public IActionResult SendOrder(SendOrder sendOrder,int id)
        {
            bool controller = Request.Path.ToString().Contains("ProviderDashBoard");
            string controllerName = null;
            if (controller)
            {
               controllerName = "ProviderDashBoard";
            }
            else
            {
                controllerName = "AdminDash";
            }

            // _adminAction.SendOrder(sendOrder);
            return RedirectToAction("SendOrder", controllerName,new { id = id });
        }

          [CustomAuthorize(new string[] { "Admin", "Physician" })]
        public IActionResult GetBusinessName(string professionId)
        {
            var result = _context.HealthProfessionals.Where(r => r.Profession == int.Parse(professionId)).ToList();
            return Json(result);
        }

        public IActionResult GetBusinessData(string vendorId)
        {
            var result = _context.HealthProfessionals.Where(r => r.VendorId == int.Parse(vendorId)).FirstOrDefault();
            return Json(result);

        }

       

        [HttpPost]
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

		#endregion

		#region Encounterform,finalize,generatepdf
		[CustomAuthorize(new string[] { "Admin", "Physician" })]
        [HttpGet("ProviderDashBoard/EncounterForm/{id}", Name = "ProviderForms")]
        [HttpGet("AdminDash/EncounterForm/{id}", Name = "AdminForms")]
        public IActionResult EncounterForm(int id)
        {
            string email = HttpContext.Session.GetString("Email");

            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            if (admin != null)
            {
                ViewBag.IsPhysican = false;

            }
            else
            {
                ViewBag.IsPhysican = true;

            }

            var result = _adminAction.EncounterForm(id);
            return View(result);
        }

        [CustomAuthorize(new string[] { "Admin", "Physician" })]
        [HttpPost("ProviderDashBoard/EncounterForm/{id}", Name = "ProviderForms")]
        [HttpPost("AdminDash/EncounterForm/{id}", Name = "AdminForms")]
        public IActionResult EncounterForm(int id, Encounter enc)
        {
            bool controller = Request.Path.ToString().Contains("ProviderDashBoard");
            string controllerName = null;
            if (controller)
            {
                controllerName = "ProviderDashBoard";
            }
            else
            {
                controllerName = "AdminDash";
            }
            _adminAction.EncounterPost(id, enc);
            return RedirectToAction("EncounterForm", controllerName, new { id = id });
        }

        public IActionResult Finalize(int id)
        {
            var finalize = _context.EncounterForms.FirstOrDefault(s => s.RequestId == id);

            if (finalize != null)
            {
                finalize.IsFinalize = true;
                _context.Update(finalize);
                _context.SaveChanges();
            }


            return RedirectToAction("ProviderDashBoard");
        }


        

        public IActionResult GeneratePDF(int requeid)
        {
            var EncounterForm = _adminAction.EncounterForm(requeid);

            if (EncounterForm == null)
            {
                return NotFound();
            }

            return new ViewAsPdf("EncounterFormDetails", EncounterForm)
            {
                FileName = "Encounter_Form.pdf"
            };
        }

		#endregion

		#region AdminProfile
		public IActionResult AdminProfile()
		{
			var Email = HttpContext.Session.GetString("Email");
			var adminProfile = _AdminDashboard.GetAdminData(Email);

			return View(adminProfile);
		}

		public IActionResult AdministratorInformation(AdminProfile adminProfile, List<string> states)

        {

            var Email = HttpContext.Session.GetString("Email");
            if (Email != null)
            {
                _AdminDashboard.AdministratorInformation(adminProfile, Email, states);
                _notyf.Success("The Data Saved Successfully");
            }
            HttpContext.Session.SetString("Email", adminProfile.Email);
            return RedirectToAction("AdminProfile");
        }


        public IActionResult MailingBillingInformation(AdminProfile adminProfile)
        {
            var Email = HttpContext.Session.GetString("Email");
            if (Email != null)
            {
                _AdminDashboard.MailingBillingInformation(adminProfile, Email);
                _notyf.Success("The Data Saved Successfully");
            }

            return RedirectToAction("AdminProfile");
        }

        public IActionResult AccountInformation([FromForm] string Password)
        {
            var Email = HttpContext.Session.GetString("Email");

            if (Email != null && Password != null)
            {
                var hashPassword = _passwordHasher.HashPassword(null, Password);
                _AdminDashboard.AccountInformation(hashPassword, Email);
                _notyf.Success("PasswordChanged Successfully");
                return RedirectToAction("Patient_login", "Login");
            }
            else
            {
                _notyf.Error("Please Enter the Password");
                _notyf.Warning("Password is Required");
                return RedirectToAction("AdminProfile");
            }
        }
		#endregion

		#region AdminDashButtons->SendLink,Createrequest,ExportExcle
		public IActionResult SendLinkForm(string sendLinkFirstname, string sendLinkLastname, string sendLinkEmail)
        {
            var mail = sendLinkEmail;
            var subject = "Creat Patient Request";
            var formLink = Url.ActionLink("Patient_Request", "Request", protocol: HttpContext.Request.Scheme);


            if (ModelState.IsValid)
            {

                if (formLink != null)
                {
                    _emailService.SendEmail("patelpriyank3112002@gmail.com", subject,
                     $"<a href='{formLink}'>Click here </a> for Request");
                    _notyf.Success("Email Sent Successfully");
                }
            }

            return RedirectToAction("AdminDash");
        }


        [CustomAuthorize(new string[] { "Admin", "Physician" })]
        [HttpGet("AdminDash/CreateRequest", Name = "AdminRequest")]
        [HttpGet("ProviderDashBoard/CreateRequest", Name = "ProviderRequest")]
        public IActionResult CreateRequest()
        {
            string email = HttpContext.Session.GetString("Email");

            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            if (admin != null)
            {
                ViewBag.IsPhysican = false;

            }
            else
            {
                ViewBag.IsPhysican = true;

            }
            CreateRequest createRequest = new CreateRequest();
            createRequest.Region = _context.Regions.ToList();
            return View(createRequest);
        }

        [CustomAuthorize(new string[] { "Admin", "Physician" })]
        [HttpPost("ProviderDashBoard/CreateRequest", Name = "ProviderRequests")]
        [HttpPost("AdminDash/CreateRequest", Name = "AdminRequests")]
        public IActionResult CreateRequest(CreateRequest createRequest, string SelectedStateId)
        {
            string email = HttpContext.Session.GetString("Email");

            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            ViewBag.IsPhysican = admin != null ? false : true;

            bool controller = Request.Path.ToString().Contains("ProviderDashBoard");
            string controllerName = (controller) ? "ProviderDashBoard" : "AdminDash";

            var Email = HttpContext.Session.GetString("Email");
            var subject = "Creat Patient Request";
            var formLink = Url.ActionLink("Patient_Request", "Request", protocol: HttpContext.Request.Scheme);


            if (ModelState.IsValid)
            {
                _AdminDashboard.AddCreateRequest(createRequest, Email, SelectedStateId);
                if (formLink != null)
                {
                    //send email on createRequest.Email which is enter by admin
                    _emailService.SendEmail("patelpriyank3112002@gmail.com", subject,
                     $"<a href='{formLink}'>Click here </a> for Request");
                    _notyf.Success("Email Sent Successfully");
                }
                return RedirectToAction("CreateRequest", controllerName);
            }

            createRequest.Region = _context.Regions.ToList();
            return View(createRequest);
        }


		public IActionResult ExportRequests(string[] currentstatus, int? pagesize = null, int? currentpage = null)
		{
			string[] num = currentstatus[0].Split(',');
			int[] nums = Array.ConvertAll(num, int.Parse);

			var result = _adminAction.GetRequests(nums);

			if (pagesize.HasValue && currentpage.HasValue)
			{
				result = result.Skip((currentpage.Value - 1) * pagesize.Value).Take(pagesize.Value);
			}

			using (var excel = new ExcelPackage())
			{
				var worksheet = excel.Workbook.Worksheets.Add("sheet1");
				worksheet.Cells[1, 1].Value = "PatientName";
				worksheet.Cells[1, 2].Value = "BirthDate";
				worksheet.Cells[1, 3].Value = "RequestorName";
				worksheet.Cells[1, 4].Value = "RequestDate";
				worksheet.Cells[1, 5].Value = "phone";
				worksheet.Cells[1, 6].Value = "address";
				worksheet.Cells[1, 7].Value = "Email";

				var row = 2;
				foreach (var item in result)
				{
					worksheet.Cells[row, 1].Value = item.Name;
					worksheet.Cells[row, 2].Value = item.BirthDate;
					worksheet.Cells[row, 3].Value = item.Requestor;
					worksheet.Cells[row, 4].Value = item.RequestedDate;
					worksheet.Cells[row, 5].Value = item.PhoneNumber;
					worksheet.Cells[row, 6].Value = item.Address;
					worksheet.Cells[row, 7].Value = item.Email;
					row++;
				}

				var excelBytes = excel.GetAsByteArray();
				return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
			}
		}
		#endregion

		#region provider

		#region Createprovideraccount
		public IActionResult CreateProviderAccount()
		{
			CreateProviderAccount createProviderAccount = new CreateProviderAccount();
			createProviderAccount.regions = _context.Regions.ToList();
			createProviderAccount.roles = _context.Roles.ToList();
			return View(createProviderAccount);

		}


		[HttpPost]
		public IActionResult CreateProviderAccount(CreateProviderAccount CreateProviderAccount, string[] regions)
		{
			if (ModelState.IsValid)
			{
				_acc.CreateProviderAccountPost(CreateProviderAccount, regions);
				_notyf.Success("Physician Added Successfully");
				return RedirectToAction("CreateProviderAccount");
			}

			else
			{
				CreateProviderAccount.regions = _context.Regions.ToList();
				CreateProviderAccount.roles = _context.Roles.ToList();
				return View("CreateProviderAccount", CreateProviderAccount);
			}
		}
		#endregion

		#region ProviderTab
		public IActionResult Provider()
        {
            Provider provider = new Provider();
            provider.regions = _context.Regions.ToList();
            return View(provider);
        }

        public IActionResult ProvidersData(string region)
        {
            var result = _AdminDashboard.providers(region);
            return PartialView("_ProviderTable", result);
        }
		#endregion

		#region EditPhysicianProfile
		public IActionResult PhysicianProfile(int id)
        {
            var physicanProfile = _AdminDashboard.PhysicianProfile(id);
            return View(physicanProfile);
        }

        public IActionResult ResetPhysicianPassword(string Password, int physicianid)
        {

            Physician? physician = _context.Physicians.FirstOrDefault(item => item.PhysicianId == physicianid);
            AspNetUser? account = _context.AspNetUsers.FirstOrDefault(item => item.Email == physician.Email);


            if (account != null && Password != null)
            {
                string passwordhash = _passwordHasher.HashPassword(null, Password);
                account.PasswordHash = passwordhash;
                _context.AspNetUsers.Update(account);
                _context.SaveChanges();
                _notyf.Success("Password Changed Successfully");
            }
            else
            {
                _notyf.Error("Please Enter the Password");
            }

            return RedirectToAction("PhysicianProfile", "AdminDash", new { id = physicianid });
        }

        public IActionResult PhysicianInformation(int id, string MobileNo, string[] Region, string SynchronizationEmail, string NPINumber, string MedicalLicense)
        {
            Physician? physician = _context.Physicians.FirstOrDefault(item => item.PhysicianId == id);

            AspNetUser? account = _context.AspNetUsers.FirstOrDefault(item => item.Email == physician.Email);
            if (physician != null)
            {
                physician.Mobile = MobileNo;
                physician.Npinumber = NPINumber;
                physician.MedicalLicense = MedicalLicense;
                physician.SyncEmailAddress = SynchronizationEmail;
                _context.Physicians.Update(physician);
                _context.SaveChanges();


                List<PhysicianRegion> region = _context.PhysicianRegions.
                    Where(item => item.PhysicianId == physician.PhysicianId).ToList();

                _context.PhysicianRegions.RemoveRange(region);
                _context.SaveChanges();

                foreach (var item in Region)
                {
                    PhysicianRegion physicianRegion = new PhysicianRegion();
                    physicianRegion.PhysicianId = id;
                    physicianRegion.RegionId = int.Parse(item);
                    _context.Add(physicianRegion);
                    _context.SaveChanges();
                }

                _notyf.Success("Data Saved Successfully");

            }
            else
            {
                _notyf.Error("Data Can not be added");
            }
            return RedirectToAction("PhysicianProfile", "AdminDash", new { id = id });

        }

        public IActionResult ProviderLocation()
        {
            return View();
        }

        public IActionResult MailingBillingInformationProvider(int physicianid, string MobileNo, string Address1, string Address2, string City, int State, string Zipcode)
        {

            Physician? physician = _context.Physicians.FirstOrDefault(item => item.PhysicianId == physicianid);
            if (physician != null)
            {
                physician.Address1 = Address1;
                physician.Address2 = Address2;
                physician.City = City;
                physician.Mobile = MobileNo;
                physician.RegionId = State;
                physician.Zip = Zipcode;
                _context.Physicians.Update(physician);
                _context.SaveChanges();
                _notyf.Success("Data Saved Successfully");
            }
            else
            {
                _notyf.Error("Data Can not be added");

            }
            return RedirectToAction("PhysicianProfile", "AdminDash", new { id = physicianid });
        }

        [HttpPost]
        public IActionResult SaveSignatureImage(IFormFile signatureImage, int id)
        {

            if (signatureImage != null && signatureImage.Length > 0)
            {
                string fileName = _uploadProvider.UploadSignature(signatureImage, id);
                var physician = _context.Physicians.FirstOrDefault(item => item.PhysicianId == id);
                physician.Signature = fileName;
                _context.Physicians.Update(physician);
                _context.SaveChanges();
                _notyf.Success("Signature Saved Successfully");
                return Ok();
            }
            else
            {
                _notyf.Error("signature can not be saved");
                return BadRequest();
            }


        }

        public IActionResult Providerprofile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile)
        {
            try
            {
                _AdminDashboard.UpdateProviderProfile(id, businessName, businessWebsite, signatureFile, photoFile);
                _notyf.Success("Data Added Successfully");
            }
            catch (InvalidOperationException ex)
            {
                _notyf.Error("Error While Saving the data");
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("PhysicianProfile", "AdminDash", new { id = id });
        }
        public IActionResult NotificationManagement(bool isChecked, string id)
        {
            var physicianNotification = _context.PhysicianNotifications.Where(u => u.PhysicianId == int.Parse(id)).FirstOrDefault();
            if (physicianNotification != null)
            {
                if (isChecked == true)
                {
                    physicianNotification.IsNotificationStopped = new BitArray(new[] { true });
                }
                else
                {
                    physicianNotification.IsNotificationStopped = new BitArray(new[] { false });
                }

                _context.PhysicianNotifications.Update(physicianNotification);
                _context.SaveChanges();
                return RedirectToAction("Provider");
            }
            return RedirectToAction("Provider");
        }

        [HttpPost]
        public IActionResult UploadDocumetns(string fileName, IFormFile File, int physicianid)
        {

            if (_AdminDashboard.UploadDocumetnsProvider(fileName, File, physicianid))
            {
                _notyf.Success("Document Saved Successfully");
                return Ok();
            }
            else
            {
                return BadRequest("No Doc File received.");
            }
        }

       
        public IActionResult SendEmailSMS(string id, string Message, string radioForprovider)
        {

            if (id != null)
            {
                var physicianEmail = _context.Physicians.FirstOrDefault(s => s.PhysicianId == int.Parse(id)).Email;
            }
            ///do for value 3 and 2 
            if (radioForprovider == "1")
            {
                _emailService.SendEmail("Patelpriyank3112002@gmail.com", "For Contact", Message);
                _notyf.Success("Email sent successully");

            }
            else
            {
                _notyf.Error("Email can not be sent");
            }
            return RedirectToAction("AdminDash");
        }
		#endregion

		#endregion

		#region CreateAdminAccount
		public IActionResult CreateAdminAccount()
        {
            AdminProfile profile = new AdminProfile();
            profile.Regions = _context.Regions.ToList();
            profile.roles = _context.Roles.ToList();
            return View("Access/CreateAdminAccount", profile);
        }

        [HttpPost]
        public IActionResult CreateAdminAccount(AdminProfile profile, string[] regions)
        {

            if (ModelState.IsValid)
            {

                _acc.CreateAdminAccountPost(profile, regions);
                _notyf.Success("Data Added Successfully");
                return RedirectToAction("CreateAdminAccount");
            }
            else
            {

                profile.Regions = _context.Regions.ToList();
                profile.roles = _context.Roles.ToList();
                return View("Access/CreateAdminAccount", profile);
            }

        }
		#endregion

		#region AccessTab

		#region UserAccess
		public IActionResult UserAccess()
		{
			return View("Access/UserAccess");
		}

		public IActionResult UserAccessData(int role)
		{
			var result = _acc.GetUserAccessData(role);
			return PartialView("Access/_UserAccessPartial", result);
		}
		#endregion

		#region AccountAccess
		public IActionResult AccountAccess()
        {
            var roles = _context.Roles.ToList();
            var list = roles.Where(item => item.IsDeleted != null && (item.IsDeleted.Length == 0 || !item.IsDeleted[0]));
            return View("Access/AccountAccess", list.ToList());
        }

		#endregion

		#region Createaccess,Partial,Postmethod
		public IActionResult CreateAccess()
        {
            return View("Access/CreateAccess");
        }

     
        public IActionResult GetRoles(int role)
        {
            var menu = _context.Menus.Where(item => role == 0 || item.AccountType == role).ToList();
            return PartialView("Access/_CreateAccessPartial", menu);
        }

       
        [HttpPost]
        public IActionResult CreateAccess(int[] rolemenu, string rolename, int accounttype)
        {
            if (rolemenu.Length != 0 && rolename != null && accounttype != 0)
            {
                _acc.CreateAccess(rolemenu, rolename, accounttype);
                _notyf.Success("Role Created successfully");

            }
            else if (rolemenu.Length == 0)
            {
                _notyf.Error("Please select the roles");
                _notyf.Error("Role Can not be added");

            }

            return RedirectToAction("CreateAccess");
        }
		#endregion

		#region EditAccess,Partial,Postmethod
		public IActionResult EditAccess(int roleid)
		{
			var rolemenu = _context.RoleMenus.Where(item => item.RoleId == roleid).Select(item => item.MenuId).ToList();
			var role = _context.Roles.Where(item => item.RoleId == roleid).FirstOrDefault();
			Access access = new Access();
			access.Menu = rolemenu;
			access.Name = role.Name;
			access.roleid = roleid;
			access.Accounttype = role.AccountType;

			return View("Access/EditAccess", access);
		}
		//you can edit  roles data
		public IActionResult EditRolesData(int role, int roleid)
        {
            var rolemenu = _context.RoleMenus.Where(item => item.RoleId == roleid).Select(item => item.MenuId).ToList();
            var menuList = _context.Menus.Where(item => role == 0 || item.AccountType == role).ToList();

            var viewModel = new RoleMenuModel
            {
                RoleMenuIds = rolemenu,
                MenuList = menuList,
            };

            return PartialView("Access/_EditAccessPartial", viewModel);
        }

		[HttpPost]
        public IActionResult EditAccess(int id, int[] rolemenu, string rolename, int accounttype)
        {
            var role = _context.Roles.FirstOrDefault(item => item.RoleId == id);
            var menulist = _context.RoleMenus.Where(item => item.RoleId == id).ToList();
            role.Name = rolename;
            role.AccountType = (short)accounttype;
            _context.Roles.Update(role);
            _context.SaveChanges();
            _context.RoleMenus.RemoveRange(menulist);
            _context.SaveChanges();
            foreach (var item in rolemenu)
            {
                RoleMenu rolemenu1 = new RoleMenu();
                rolemenu1.MenuId = item;
                rolemenu1.RoleId = id;
                _context.RoleMenus.Add(rolemenu1);
            }
            _context.SaveChanges();
            return RedirectToAction("EditAccess", new { roleid = id });
        }

        public IActionResult DeleteRole(int id)
        {

            Role? role = _context.Roles.Where(item => item.RoleId == id).FirstOrDefault();
            if (role != null)
            {
                role.IsDeleted = new BitArray(new[] { true });
                _context.Roles.Update(role);
                _context.SaveChanges();
                _notyf.Success("Your Role Has Been Deleted");
            }
            else
            {
                _notyf.Success("Your Role Has Been Not Deleted");
            }
            return RedirectToAction("AccountAccess");
        }

		#endregion

		#endregion

		#region Scheduling
		public IActionResult PhysicianScheduling()
        {
            var region = _context.Regions.ToList();
            ViewBag.regions = region;
            return View();
        }

        [HttpGet]
        //this action is called with the region from ajax
        public IActionResult GetPhysicianShift(int region)
        {
            // Retrieve physicians associated with the specified region
            var physicians = (from physicianRegion in _context.PhysicianRegions
                              where region == 0 || physicianRegion.RegionId == region
                              select physicianRegion.Physician)
                             .ToList();

            return Json(physicians);
        }

        [HttpGet]
        public IActionResult GetEvents(int region)
        {
            var events = _adminAction.GetEvents(region);
            var mappedEvents = events.Select(e => new
            {
                id = e.Shiftid,
                resourceId = e.Physicianid,
                title = e.PhysicianName,
                start = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Starttime.Hour, e.Starttime.Minute, e.Starttime.Second),
                end = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Endtime.Hour, e.Endtime.Minute, e.Endtime.Second),
                ShiftDetailId = e.ShiftDetailId,
                region = _context.Regions.Where(i => i.RegionId == e.Regionid),
                status = e.Status
            }).ToList();

            return Json(mappedEvents);
        }

        public IActionResult CreateShift(Scheduling model)
        {
            var email = HttpContext.Session.GetString("Email");
            _adminAction.CreateShift(model, email);
            return RedirectToAction("PhysicianScheduling");
        }

        [HttpPost]
        public IActionResult ChangeShift(int shiftDetailId, DateTime startDate, TimeOnly startTime, TimeOnly endTime, int region)
        {
            // Find the shift detail by its ID
            ShiftDetail shiftdetail = _context.ShiftDetails.Find(shiftDetailId);

            // If shift detail is not found, return a 404 Not Found response
            if (shiftdetail == null)
            {
                return BadRequest();
            }

            try
            {
                // Update the shift detail properties
                shiftdetail.ShiftDate = startDate;
                shiftdetail.StartTime = startTime;
                shiftdetail.EndTime = endTime;

                // Update the database
                _context.ShiftDetails.Update(shiftdetail);
                _context.SaveChanges();
                var events = _adminAction.GetEvents(region);
                var mappedEvents = events.Select(e => new
                {
                    id = e.Shiftid,
                    resourceId = e.Physicianid,
                    title = e.PhysicianName,
                    start = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Starttime.Hour, e.Starttime.Minute, e.Starttime.Second),
                    end = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Endtime.Hour, e.Endtime.Minute, e.Endtime.Second),
                    ShiftDetailId = e.ShiftDetailId,
                    region = _context.Regions.Where(i => i.RegionId == e.Regionid),
                    status = e.Status
                }).ToList();
                // Return a 200 OK response
                return Ok(new { message = "Shift detail updated successfully.", events = mappedEvents });
            }
            catch (Exception ex)
            {
                // Return a 400 Bad Request response with the error message
                return BadRequest("Error updating shift detail: " + ex.Message);
            }
        }
        public IActionResult DeleteShift(int shiftDetailId, int region)
        {
            ShiftDetail shiftdetail = _context.ShiftDetails.Find(shiftDetailId);
            if (shiftdetail == null)
            {
                return NotFound("Shift detail not found.");
            }
            shiftdetail.IsDeleted = new BitArray(new[] { true });
            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();
            var events = _adminAction.GetEvents(region);
            var mappedEvents = events.Select(e => new
            {
                id = e.Shiftid,
                resourceId = e.Physicianid,
                title = e.PhysicianName,
                start = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Starttime.Hour, e.Starttime.Minute, e.Starttime.Second),
                end = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Endtime.Hour, e.Endtime.Minute, e.Endtime.Second),
                ShiftDetailId = e.ShiftDetailId,
                region = _context.Regions.Where(i => i.RegionId == e.Regionid),
                status = e.Status
            }).ToList();
            return Ok(new { message = "Shift detail Deleted successfully.", events = mappedEvents });

        }     

        public IActionResult ReturnShift(int shiftDetailId, int region)
        {
            ShiftDetail shiftdetail = _context.ShiftDetails.Find(shiftDetailId);

            // If shift detail is not found, return a 404 Not Found response
            if (shiftdetail == null)
            {
                return NotFound("Shift detail not found.");
            }
            shiftdetail.Status = (short)((shiftdetail.Status == 0) ? 1 : 0);

            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();
            var events = _adminAction.GetEvents(region);
            var mappedEvents = events.Select(e => new
            {
                id = e.Shiftid,
                resourceId = e.Physicianid,
                title = e.PhysicianName,
                start = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Starttime.Hour, e.Starttime.Minute, e.Starttime.Second),
                end = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Endtime.Hour, e.Endtime.Minute, e.Endtime.Second),
                ShiftDetailId = e.ShiftDetailId,
                region = _context.Regions.Where(i => i.RegionId == e.Regionid),
                status = e.Status
            }).ToList();
            return Ok(new { message = "Shift detail updated successfully.", events = mappedEvents });

        }

        public IActionResult ReviewShift()
        {
            ShiftDetailModel shiftDetailModel = new ShiftDetailModel();
            shiftDetailModel.Regions = _context.Regions.ToList();
            return View(shiftDetailModel);
        }

        public IActionResult GetShifts(int region)
        {
            //0 for APPROVED
            //1 for Pending
            var result = (from shiftDetail in _context.ShiftDetails
                          where ((shiftDetail.RegionId == region || region == 0) &&
                             shiftDetail.Status != 0 && shiftDetail.IsDeleted != new BitArray(new[] { true }))
                          select new ShiftDetailModel
                          {
                              physicianName = shiftDetail.Shift.Physician.FirstName,
                              ShiftDetailId = shiftDetail.ShiftDetailId,
                              day = shiftDetail.ShiftDate.ToString("MMM dd, yyyy"),
                              starttime = shiftDetail.StartTime,
                              endtime = shiftDetail.EndTime,
                              Regioname = _context.Regions.FirstOrDefault(s => s.RegionId == shiftDetail.RegionId).Name,

                          }).ToList();
            return PartialView("_ReviewShiftPartial", result);
        }



		[HttpPost]
        public IActionResult ApprovedSelected(string[] checkedValues)
        {

            foreach (var item in checkedValues)
            {
                if(item != "0")
                {
                    var status = _context.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == int.Parse(item));
                    status.Status = 0;  
                    _context.ShiftDetails.Update(status);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("ReviewShift");

        }

        public IActionResult DeleteSelected(string[] checkedValues)
        {

            foreach (var item in checkedValues)
            {
                if (item != "0")
                {
                    var shiftToDelete = _context.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == int.Parse(item));
                    shiftToDelete.IsDeleted = new BitArray(new[] { true });
                    _context.ShiftDetails.Update(shiftToDelete);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("ReviewShift");

        }

        public IActionResult MdOnCall()
        {
            List<Region> regions = _context.Regions.ToList();
            ViewBag.Regions = regions;
            return View();


        }


        public IActionResult GetPhysiciansOnCall(string region)
        {
            DateTime today = DateTime.Today;
            BitArray trueBitArray = new BitArray(new[] { true });
            var onDuty = (from physician in _context.Physicians
                          join shift in _context.Shifts on physician.PhysicianId equals shift.PhysicianId into shiftJoin
                          from shiftRecord in shiftJoin.DefaultIfEmpty()
                          join shiftDetail in _context.ShiftDetails on shiftRecord.ShiftId equals shiftDetail.ShiftId into shiftDetailJoin
                          from shiftDetailRecord in shiftDetailJoin.DefaultIfEmpty()
                          where shiftDetailRecord.IsDeleted != trueBitArray && shiftDetailRecord.ShiftDate.Date == today.Date
                                                            && shiftDetailRecord.StartTime <= TimeOnly.FromDateTime(DateTime.Now)
                                                            && shiftDetailRecord.EndTime >= TimeOnly.FromDateTime(DateTime.Now)
                          select physician).Where(x => region == "0" || x.RegionId == int.Parse(region)).Distinct().ToList();

            var offDuty = _context.Physicians.Where(x => region == "0" || x.RegionId == int.Parse(region)).ToList().Except(onDuty).ToList();

            ProvidersOnCallModel providersOnCall = new ProvidersOnCallModel { OffDuty = offDuty, OnDuty = onDuty };

            return PartialView("_MdOnCallPartial", providersOnCall);
        }

		#endregion

		#region Partner
		public IActionResult Partners()
        {


            List<HealthProfessionalType> healthProfessionalTypes = _context.HealthProfessionalTypes.ToList();
            ViewBag.HealthProfessionalTypes = healthProfessionalTypes;
            return View("Partners/Partners");
        }

        public IActionResult GetBusinessInfo(string Profession, string searchValue)
        {
            var data = (from profession in _context.HealthProfessionalTypes
                        join business in _context.HealthProfessionals
                        on profession.HealthProfessionalId equals business.Profession
                        select new BusinessModel
                        {
                            profession = profession.ProfessionName,
                            BusinessName = business.VendorName,
                            Email = business.Email ?? "--",
                            FaxNumber = business.FaxNumber ?? "--",
                            PhoneNum = business.PhoneNumber ?? "--",
                            BusinessContact = business.BusinessContact ?? "--",
                            ProfessionId = business.Profession.ToString(),
                            vendorid = business.VendorId
                        }).Where(s => (Profession == "0" || s.ProfessionId == Profession) &&
                            (string.IsNullOrEmpty(searchValue) || s.BusinessName.Contains(searchValue))).ToList();

            return PartialView("Partners/_PartnersPartial", data);
        }


        public IActionResult AddBusiness()
        {
            List<HealthProfessionalType> healthProfessionalTypes = _context.HealthProfessionalTypes.ToList();
            ViewBag.HealthProfessionalTypes = healthProfessionalTypes;
            List<Region> regions = _context.Regions.ToList();
            ViewBag.Regions = regions;
            return View("Partners/AddBusiness");
        }

        [HttpPost]
        public IActionResult AddBusiness(AddBusiness addBusiness)
        {
            if(ModelState.IsValid)
            {
                HealthProfessional healthProfessional =  new HealthProfessional();
                healthProfessional.VendorName = addBusiness.BusinessName;
                healthProfessional.Profession = int.Parse(addBusiness.Profession);
                healthProfessional.BusinessContact = addBusiness.BusinessContact;
                healthProfessional.FaxNumber = addBusiness.FaxNumber;
                healthProfessional.Address = addBusiness.city + addBusiness.street;
                healthProfessional.City = addBusiness.city;
                healthProfessional.State = _context.Regions.FirstOrDefault(s => s.RegionId == int.Parse(addBusiness.state)).Name;
                healthProfessional.RegionId = int.Parse(addBusiness.state);
                healthProfessional.CreatedDate=DateTime.Now;
                healthProfessional.PhoneNumber = addBusiness.PhoneNum;
                healthProfessional.Email = addBusiness.Email;
                _context.HealthProfessionals.Add(healthProfessional);
                _context.SaveChanges();
                _notyf.Success("Data Added Successfully");
               return RedirectToAction("AddBusiness");
            }
            else
            {
                List<HealthProfessionalType> healthProfessionalTypes = _context.HealthProfessionalTypes.ToList();
                ViewBag.HealthProfessionalTypes = healthProfessionalTypes;
                List<Region> regions = _context.Regions.ToList();
                ViewBag.Regions = regions;
                _notyf.Error("Data can not be added");
                return View("Partners/AddBusiness");
            }
        }

        public IActionResult EditBusinessData(int id)
        {

            List<HealthProfessionalType> healthProfessionalTypes = _context.HealthProfessionalTypes.ToList();
            ViewBag.HealthProfessionalTypes = healthProfessionalTypes;
            List<Region> regions = _context.Regions.ToList();
            ViewBag.Regions = regions;
            var vendor = _context.HealthProfessionals.FirstOrDefault(s => s.VendorId == id);

            if (vendor != null)
            {
                AddBusiness businessModel = new AddBusiness();
                businessModel.Profession = vendor.Profession.ToString();
                businessModel.BusinessName = vendor.VendorName;
                businessModel.BusinessContact = vendor.BusinessContact;
                businessModel.FaxNumber = vendor.FaxNumber;
                businessModel.PhoneNum = vendor.PhoneNumber;
                businessModel.state = _context.Regions.FirstOrDefault(s => s.Name == vendor.State).RegionId.ToString();
                businessModel.city = vendor.City;
                businessModel.Email = vendor.Email;
                businessModel.street = vendor.City;
                businessModel.vendorid = id;
                return View("Partners/EditBusinessData",businessModel);
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPost]
		public IActionResult EditBusinessPost(AddBusiness addBusiness, int id)
		{
			var healthProfessional = _context.HealthProfessionals.FirstOrDefault(s => s.VendorId == id);
			healthProfessional.VendorName = addBusiness.BusinessName;
			healthProfessional.Profession = int.Parse(addBusiness.Profession);
			healthProfessional.BusinessContact = addBusiness.BusinessContact;
			healthProfessional.FaxNumber = addBusiness.FaxNumber;
			healthProfessional.Address = addBusiness.city + addBusiness.street;
			healthProfessional.City = addBusiness.city;
			healthProfessional.State = _context.Regions.FirstOrDefault(s => s.RegionId == int.Parse(addBusiness.state)).Name;
			healthProfessional.RegionId = int.Parse(addBusiness.state);
			healthProfessional.CreatedDate = DateTime.Now;
			healthProfessional.PhoneNumber = addBusiness.PhoneNum;
			healthProfessional.Email = addBusiness.Email;
			_context.HealthProfessionals.Update(healthProfessional);
			_context.SaveChanges();
			_notyf.Success("Data Updated Successfully");
			return RedirectToAction("EditBusinessData", new { id = id });
		}

		public IActionResult DeleteBusinessData(int vendorid)
		{
			var vendor = _context.HealthProfessionals.FirstOrDefault(s => s.VendorId == vendorid);

			if (vendor != null)
			{
				vendor.IsDeleted = new BitArray(new[] { true });
				_context.Update(vendor);
				_context.SaveChanges();
			}

			return RedirectToAction("Partners");
		}
		#endregion

		#region RecordsTab

		#region PatientHistory,Explore
		public IActionResult PatientHistory()
		{
			return View("RecordsMenu/PatientHistory");
		}

		public IActionResult GetPatientRecords(string firstName, string lastName, string email, string phone,int currentpage,int pagesize)
		{
            var records = _adminDashboardRecords.PatientRecords(firstName, lastName, email, phone, currentpage, pagesize);

            int totalItems = records.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            if (totalPages <= 1)
            {
                currentpage = 1;
            }
            var paginatedData = records.Skip((currentpage - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentpage;

            return PartialView("RecordsMenu/_PatientHistoryPartial", paginatedData);
		}

        public IActionResult ExploreRecords(int userid) 
		{
			var records = _adminDashboardRecords.ExploreRecords(userid);
            return View("RecordsMenu/ExploreRecords", records);
		}
		#endregion

		#region Searchrecords,Exclefile
		public IActionResult SearchRecords()
		{
			return View("RecordsMenu/SearchRecords");
		}

		public IActionResult GetSearchRecords(int[] status, string patientName,
			string providername, string PhoneNum, string email, string requesttype,int pagesize,int currentpage)
		{

            var searchRecords = _adminDashboardRecords.SearchRecords(status, patientName, providername, PhoneNum, email
                , requesttype, pagesize, currentpage);

            int totalItems = searchRecords.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            if (totalPages <= 1)
            {
                currentpage = 1;
            }
            var paginatedData = searchRecords.Skip((currentpage - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentpage;
            return PartialView("RecordsMenu/_SearchrecordPartial", paginatedData);
		}

        public IActionResult ExcleFromRecords(int[] status,string requesttype,string providername,
            string email,string PhoneNum,string patientName)
        {
            var searchRecords = _adminDashboardRecords.SearchRecords(status, patientName, providername, PhoneNum, email
                , requesttype);

            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells[1, 1].Value = "PatientName";
                worksheet.Cells[1, 3].Value = "RequestorName";
                worksheet.Cells[1, 4].Value = "RequestDate";
                worksheet.Cells[1, 5].Value = "phone";
                worksheet.Cells[1, 6].Value = "address";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "Zip";
                worksheet.Cells[1, 9].Value = "Physician";

                var row = 2;
                foreach (var item in searchRecords)
                {
                    worksheet.Cells[row, 1].Value = item.PatientName;
                    worksheet.Cells[row, 3].Value = item.Requestor;
                    worksheet.Cells[row, 4].Value = item.DateOfService;
                    worksheet.Cells[row, 5].Value = item.PhoneNumber;
                    worksheet.Cells[row, 6].Value = item.Address;
                    worksheet.Cells[row, 7].Value = item.Email;
                    worksheet.Cells[row, 8].Value = item.Zip;
                    worksheet.Cells[row, 9].Value = item.PhysicianName;
                    row++;
                }

                var excelBytes = excel.GetAsByteArray();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
            }
        }



        public IActionResult DeleteRecord(int id)
		{
			Request? request = _context.Requests.Find(id);
			if (request != null)
			{
				request.IsDeleted = new BitArray(new[] { true });
				_context.Requests.Update(request);
			}
			_notyf.Success("The patient deleted successfully");
			_context.SaveChanges();
			return RedirectToAction("SearchRecords");
		}
		#endregion

		#region BlockRecords
		public IActionResult BlockHistory()
		{
			return View("RecordsMenu/BlockHistory");
		}

		public IActionResult GetBlockedPatientRecords(string email, string name, string phone, DateTime date)
		{
			var list = _adminDashboardRecords.BlockedPatientRecords(email, name, phone, date);  
			return PartialView("RecordsMenu/_BlockHistoryPartial", list);
		}

        public IActionResult Unblock(int id)
        {
            var user = _context.BlockRequests.FirstOrDefault(s => s.RequestId ==  id.ToString());

            if (user != null)
            {
                _context.BlockRequests.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("BlockHistory");
        }
		#endregion

		#endregion

		#region PhysicianLocation
		public IActionResult GetPhysicianLocation()
        {
            List<PhysicianLocation> physicianLocations = _context.PhysicianLocations.ToList();
            return Json(physicianLocations);
        }
		#endregion
	}

}
