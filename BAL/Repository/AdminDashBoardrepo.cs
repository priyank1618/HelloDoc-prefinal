using BAL.Interface;
using DAL.DataContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using DAL.ViewModel;
using DAL.DataModels;



namespace BAL.Repository
{
    public class AdminDashBoardrepo : IAdminDashBoard
    {

        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<AdminProfile> _passwordHasher;
        private readonly IUploadProvider _uploadprovider;


        public AdminDashBoardrepo(ApplicationDbContext context, IUploadProvider uploadprovider,IPasswordHasher<AdminProfile> passwordHasher)
        {
            _context = context;
            _uploadprovider = uploadprovider;
            _passwordHasher = passwordHasher;
        }


        public IQueryable<Admin_DashBoard> GetList()
        {
            var DashData = (from req in _context.Requests
                            join reqclient in _context.RequestClients
                           on req.RequestId equals reqclient.RequestId


                            select new Admin_DashBoard()
                            {
                                Name = reqclient.FirstName,
                                Requestor = req.FirstName,
                                BirthDate = (new DateTime((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate)).ToString("MMM dd,yyyy"),
                                RequestedDate = req.CreatedDate,
                                Email = reqclient.Email,
                                PhoneNumber = req.PhoneNumber,
                                requesttypeid = req.RequestTypeId,
                                PhoneNumber_P = reqclient.PhoneNumber,
                                regionid = reqclient.RegionId,
                                Address = reqclient.Street + " " + reqclient.City + " " + reqclient.State + " " + reqclient.ZipCode,
                                status = req.Status,
                                requestid = reqclient.RequestId,
                                cases = _context.CaseTags.ToList(),
                                region = _context.Regions.ToList(),
                                Isfinalise = _context.EncounterForms.Where(x => x.RequestId == req.RequestId).Select(x => x.IsFinalize).FirstOrDefault()
                            }); ;

                            return DashData;   
        }

        public List<Admin_DashBoard> GetRequestData(string SearchValue, string Filterselect,
            string selectvalue, string partialName, int[] currentstatus)
        {
            var DashData = (from req in _context.Requests
                            join reqclient in _context.RequestClients
                             on req.RequestId equals reqclient.RequestId
                             join phy in _context.Physicians on
                             req.PhysicianId equals phy.PhysicianId into phys
                             from totalreqs in phys.DefaultIfEmpty()

                            select new Admin_DashBoard()
                            {
                                Name = reqclient.FirstName.ToLower(),
                                LastName = reqclient.LastName,
                                Requestor = req.FirstName,
                                BirthDate = (new DateTime((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate)).ToString("MMM dd,yyyy"),
                                RequestedDate = req.CreatedDate,
                                PhoneNumber = req.PhoneNumber,
                                requesttypeid = req.RequestTypeId,
                                PhoneNumber_P = reqclient.PhoneNumber,
                                regionid = reqclient.RegionId,
                                Address = reqclient.Street + " " + reqclient.City + " " + reqclient.State + " " + reqclient.ZipCode,
                                status = req.Status,
                                reqclientid = reqclient.RequestClientId,
                                Email = reqclient.Email,
                                Notes = reqclient.Notes,
                                confirmationnum = req.ConfirmationNumber,
                                requestid = reqclient.RequestId,
                                physicianName = totalreqs.FirstName,
                                physicianid = req.PhysicianId,
                                Isfinalise = _context.EncounterForms.Where(x => x.RequestId == req.RequestId).Select(x => x.IsFinalize).FirstOrDefault()


                            }).Where(item => (string.IsNullOrEmpty(SearchValue) || item.Name.Contains(SearchValue)) &&
                              (string.IsNullOrEmpty(Filterselect) || item.requesttypeid == int.Parse(Filterselect)) &&
                              (string.IsNullOrEmpty(selectvalue) || item.regionid == int.Parse(selectvalue)) &&
                               currentstatus.Any(status => item.status == status)).ToList();

            return DashData;

        }

        public List<Admin_DashBoard> GetRequestDataPhy(string SearchValue, string Filterselect,
            string selectvalue, string partialName, int[] currentstatus,int PhyId)
        {
            var DashData = (from req in _context.Requests
                            join reqclient in _context.RequestClients
                             on req.RequestId equals reqclient.RequestId
                             where req.PhysicianId == PhyId
                            select new Admin_DashBoard()
                            {
                                Name = reqclient.FirstName.ToLower(),
                                LastName = reqclient.LastName,
                                Requestor = req.FirstName,
                                BirthDate = (new DateTime((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate)).ToString("MMM dd,yyyy"),
                                RequestedDate = req.CreatedDate,
                                PhoneNumber = req.PhoneNumber,
                                requesttypeid = req.RequestTypeId,
                                PhoneNumber_P = reqclient.PhoneNumber,
                                regionid = reqclient.RegionId,
                                Address = reqclient.Street + " " + reqclient.City + " " + reqclient.State + " " + reqclient.ZipCode,
                                status = req.Status,
                                reqclientid = reqclient.RequestClientId,
                                Email = reqclient.Email,
                                Notes = reqclient.Notes,
                                confirmationnum = req.ConfirmationNumber,
                                requestid = reqclient.RequestId,
                                Isfinalise = _context.EncounterForms.Where(x => x.RequestId == req.RequestId).Select(x => x.IsFinalize).FirstOrDefault()

                            }).Where(item => (string.IsNullOrEmpty(SearchValue) || item.Name.Contains(SearchValue)) &&
                              (string.IsNullOrEmpty(Filterselect) || item.requesttypeid == int.Parse(Filterselect)) &&
                              (string.IsNullOrEmpty(selectvalue) || item.regionid == int.Parse(selectvalue)) &&
                               currentstatus.Any(status => item.status == status)).ToList();

            return DashData;

        }

        public IQueryable<Admin_DashBoard> getregionwise()
        {
            var DashData = (from req in _context.Requests
                            join reqclient in _context.RequestClients
                             on req.RequestId equals reqclient.RequestId
                            join region in _context.Regions on reqclient.RegionId equals region.RegionId

                            select new Admin_DashBoard()
                            {
                                Name = reqclient.FirstName.ToLower(),
                                LastName = reqclient.LastName,
                                Requestor = req.FirstName,
                                BirthDate = (new DateTime((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate)).ToString("MMM dd,yyyy"),
                                RequestedDate = req.CreatedDate,
                                PhoneNumber = req.PhoneNumber,
                                requesttypeid = req.RequestTypeId,
                                PhoneNumber_P = reqclient.PhoneNumber,
                                regionid = reqclient.RegionId,
                                Address = reqclient.Street + " " + reqclient.City + " " + reqclient.State + " " + reqclient.ZipCode,
                                status = req.Status,
                                reqclientid = reqclient.RequestClientId,
                                Email = reqclient.Email,
                                Notes = reqclient.Notes,
                                regionname = region.Name,
                                confirmationnum = req.ConfirmationNumber,
                                requestid = reqclient.RequestId,
                                Isfinalise = _context.EncounterForms.Where(x => x.RequestId == req.RequestId).Select(x => x.IsFinalize).FirstOrDefault()
                            });

            return DashData;
        }

        public IQueryable<ViewNotes> GetViewNotes(int id)
        {
            var result = (from reqnote in _context.RequestNotes
                          join
                          reqstatuslog in _context.RequestStatusLogs
                          on reqnote.RequestId equals reqstatuslog.RequestId
                          into grp
                          where reqnote.RequestId == id
                          from reqstatuslog in grp.DefaultIfEmpty()
                          select new ViewNotes
                          {
                              AdminNotes = reqnote.AdminNotes,
                              PhysicianNotes = reqnote.PhysicianNotes,
                              TransferNotes = reqstatuslog.Notes ?? "-----"

                          });

            return result;
        }

      

        public AdminProfile GetAdminData(string Email)
        {
            AdminProfile adminProfile = new AdminProfile();
            adminProfile.Regions = _context.Regions.ToList();

            var aspNetUser = _context.AspNetUsers.FirstOrDefault(x => x.Email == Email);
            if (aspNetUser != null)
            {
                var admin = _context.Admins.FirstOrDefault(x => x.AspNetUserId == aspNetUser.AspNetUserId);
                var region = _context.Regions.FirstOrDefault(s => s.RegionId == admin.RegionId);
                var states = _context.AdminRegions.Where(v => v.AdminId == admin.AdminId);
                adminProfile.Address1 = admin.Address1;
                adminProfile.Address2 = admin.Address2;
                adminProfile.PhoneNumAspNetUsers = aspNetUser.PhoneNumber;
                adminProfile.UserName = Email;
                adminProfile.Email = Email;
                adminProfile.zip = admin.Zip;
                adminProfile.City = admin.City;
                adminProfile.state = region.RegionId;
                adminProfile.FirstName = admin.FirstName;
                adminProfile.LastName = admin.LastName;
                adminProfile.MobileNumAdmin = admin.Mobile;
                adminProfile.SelectedRegions = states.Select(x => x.RegionId).ToList();
                 adminProfile.statesForChecked= (from adminregion in _context.AdminRegions
                           where adminregion.AdminId == admin.AdminId
                           select new CheckboxList_model
                           {
                               Value = adminregion.RegionId,
                               Selected = true
                           }).ToList();

                

            }
            return adminProfile;
        }

        public void AdministratorInformation(AdminProfile adminProfile, string Email, List<string> states)
        {

            AspNetUser user = _context.AspNetUsers.FirstOrDefault(s => s.Email == Email);
            Admin admin = _context.Admins.FirstOrDefault(s => s.AspNetUserId == user.AspNetUserId);
            var List = _context.AdminRegions.Where(s => s.AdminId == admin.AdminId).ToList();

            _context.AdminRegions.RemoveRange(List);

            //add on the adminid 
            foreach (var item in states)
            {
            AdminRegion adminRegion = new AdminRegion();
                adminRegion.AdminId = admin.AdminId;
                adminRegion.RegionId = int.Parse(item);
                _context.Add(adminRegion);
                _context.SaveChanges();
            }

            user.Email = adminProfile.Email;
            user.PhoneNumber = adminProfile.PhoneNumAspNetUsers;
            admin.Email = adminProfile.Email;
            admin.FirstName = adminProfile.FirstName;
            admin.LastName = adminProfile.LastName;
            _context.Update(user);
            _context.Update(admin);
            _context.SaveChanges();


        }

        public void AccountInformation(string password, string Email)
        {
            AspNetUser user = _context.AspNetUsers.FirstOrDefault(s => s.Email == Email);
            user.PasswordHash = password;

            _context.Update(user);
            _context.SaveChanges();
        }

        public void MailingBillingInformation(AdminProfile adminProfile, string Email)
        {
            AspNetUser user = _context.AspNetUsers.FirstOrDefault(s => s.Email == Email);
            Admin admin = _context.Admins.FirstOrDefault(s => s.AspNetUserId == user.AspNetUserId);
            var region = _context.Regions.FirstOrDefault(s => s.RegionId == admin.RegionId);
            admin.Address1 = adminProfile.Address1;
            admin.Address2 = adminProfile.Address2;
            admin.Zip = adminProfile.zip;
            admin.City = adminProfile.City;
            admin.RegionId = adminProfile.SelectedStateId;
            admin.Mobile = adminProfile.MobileNumAdmin;
            _context.Update(admin);
            _context.SaveChanges();
        }

        public int CountByStatus(int[] status)
        {
            throw new NotImplementedException();
        }

        public void AddCreateRequest(CreateRequest patient, string Email, string SelectedStateId)
        {
          
                var admin = _context.Admins.Where(x => x.Email == Email).FirstOrDefault();
                if (patient != null)
                {
                    patient.CreatedDate = DateTime.Now;
                    var request = new Request();

                    request.FirstName = admin.FirstName;
                    request.LastName = admin.LastName;
                    request.CreatedDate = DateTime.Now;
                    request.PhoneNumber = admin.Mobile;
                    request.Email = Email;
                   request.RequestTypeId = 1;


                    _context.Requests.Add(request);
                    _context.SaveChanges();


                    var requestClient = new RequestClient();

                    requestClient.RequestId = request.RequestId;
                    requestClient.FirstName = patient.FirstName;
                    requestClient.LastName = patient.LastName;
                    requestClient.Email = patient.Email;
                    requestClient.PhoneNumber = patient.PhoneNumber;
                    requestClient.Street = patient.Street;
                    requestClient.City = patient.City;
                    requestClient.State = _context.Regions.FirstOrDefault(s => s.RegionId == int.Parse(SelectedStateId)).Name;
                    requestClient.ZipCode = patient.ZipCode;
                    requestClient.IntDate = patient.BirthDate.Value.Day;
                    requestClient.IntYear = patient.BirthDate.Value.Year;
                    requestClient.StrMonth = patient.BirthDate.Value.Month.ToString();
                    requestClient.RegionId = int.Parse(SelectedStateId);


                    _context.RequestClients.Add(requestClient);
                    _context.SaveChanges();


                var region = _context.Regions.FirstOrDefault(x => x.RegionId == requestClient.RegionId);
                var count = _context.Requests.Where(x => x.CreatedDate.Date == request.CreatedDate.Date).Count();


                //-------------------------------------
                if (region != null)
                {
                    var confirmationnum = region.Abbreviation.ToUpper() + request.CreatedDate.ToString("ddMMyy") +
                        requestClient.LastName.Substring(0, 2).ToUpper() + requestClient.FirstName.Substring(0, 2).ToUpper() + count.ToString("D4");
                    request.ConfirmationNumber = confirmationnum;
                }
                else
                {
                    var confirmationnum = "AB" + request.CreatedDate.ToString("ddMMyy") +
                        requestClient.LastName.Substring(0, 2).ToUpper() + requestClient.FirstName.Substring(0, 2).ToUpper() + count.ToString("D4");
                    request.ConfirmationNumber = confirmationnum;
                }

                _context.Update(request);
                _context.SaveChanges();

                var reqnotes = new RequestNote();
                    reqnotes.RequestId = request.RequestId;
                    reqnotes.AdminNotes = patient.AdminNote;
                    reqnotes.CreatedDate = DateTime.Now;
                    reqnotes.CreatedBy = request.FirstName + request.LastName;
                    reqnotes.PhysicianNotes = "-";
                    _context.RequestNotes.Add(reqnotes);
                    _context.SaveChanges();
                }
            }

        public GetCount GetCount()
        {
            var newcount = (_context.Requests.Where(item => item.Status == 1)).Count();
            var pendingcount = (_context.Requests.Where(item => item.Status == 2)).Count();
            var activecount = (_context.Requests.Where(item => item.Status == 4 || item.Status == 5)).Count();
            var conclude = (_context.Requests.Where(item => item.Status == 6)).Count();
            var toclosed = (_context.Requests.Where(item => item.Status == 3 || item.Status == 7 || item.Status == 8)).Count();
            var unpaid = (_context.Requests.Where(item => item.Status == 9)).Count();

            return new GetCount
            {
                NewCount = newcount,
                PendingCount = pendingcount,
                ActiveCount = activecount,
                Conclude = conclude,
                ToClosed = toclosed,
                Unpaid = unpaid
            };
        }

        public void UpdateProviderProfile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile)
        {
            var physician = _context.Physicians.FirstOrDefault(item => item.PhysicianId == id);

            if (physician != null)
            {
                physician.BusinessName = businessName;
                physician.BusinessWebsite = businessWebsite;

                if (signatureFile != null && signatureFile.FileName != null)
                {
                    string signatureFileName = _uploadprovider.UploadSignature(signatureFile, id);
                    physician.Signature = signatureFileName;
                }

                if (photoFile != null && photoFile.FileName != null)
                {
                    string photoFileName = _uploadprovider.UploadPhoto(photoFile, id);
                    physician.Photo = photoFileName;
                }

                _context.Physicians.Update(physician);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Physician not found");
            }
        }

        public List<Provider> providers(string Region)
        {
            var result = (from phy in _context.Physicians
                          join role in _context.Roles
                          on phy.RoleId equals role.RoleId
                          join notify in _context.PhysicianNotifications
            on phy.PhysicianId equals notify.PhysicianId
            orderby phy.CreatedDate
                          where (string.IsNullOrEmpty(Region) || phy.RegionId == int.Parse(Region))
                          select new Provider
                          {
                              Name = phy.FirstName,
                              Role = role.Name,
                              OnCallStaus = new BitArray(new[] { notify.IsNotificationStopped[0] }),
                              status = phy.Status,
                              regions = _context.Regions.ToList(),
                              physicianid = phy.PhysicianId
                          }).ToList();

            return result;
        }

        public PhysicianProfile PhysicianProfile(int id)
        {
            Physician? physician = _context.Physicians.FirstOrDefault(item => item.PhysicianId == id);

            PhysicianProfile physicanProfile = new PhysicianProfile();
            physicanProfile.FirstName = physician.FirstName;
            physicanProfile.LastName = physician.LastName ?? "";
            physicanProfile.Email = physician.Email;
            physicanProfile.Address1 = physician.Address1 ?? "";
            physicanProfile.Address2 = physician.Address2 ?? "";
            physicanProfile.City = physician.City ?? "";
            physicanProfile.ZipCode = physician.Zip ?? "";
            physicanProfile.MobileNo = physician.Mobile ?? "";
            physicanProfile.Regions = _context.Regions.ToList();
            physicanProfile.MedicalLicense = physician.MedicalLicense;
            physicanProfile.NPINumber = physician.Npinumber;
            physicanProfile.SynchronizationEmail = physician.SyncEmailAddress;
            physicanProfile.physicianid = physician.PhysicianId;
            physicanProfile.WorkingRegions = _context.PhysicianRegions.Where(item => item.PhysicianId == physician.PhysicianId).ToList();
            physicanProfile.State = physician.RegionId;
            physicanProfile.SignatureFilename = physician.Signature;
            physicanProfile.BusinessWebsite = physician.BusinessWebsite;
            physicanProfile.BusinessName = physician.BusinessName;
            physicanProfile.PhotoFileName = physician.Photo;
            physicanProfile.IsAgreement = physician.IsAgreementDoc;
            physicanProfile.IsBackground = physician.IsBackgroundDoc;
            physicanProfile.IsHippa = physician.IsAgreementDoc;
            physicanProfile.NonDiscoluser = physician.IsNonDisclosureDoc;
            physicanProfile.License = physician.IsLicenseDoc;
            return physicanProfile;
        }

      

        public bool UploadDocumetnsProvider(string fileName, IFormFile File, int physicianid)
        {
            Physician? physician = _context.Physicians.FirstOrDefault(item => item.PhysicianId == physicianid);
            if (physician != null)
            {

                if (fileName == "ICA")
                {
                    var docfile = _uploadprovider.UploadDocFile(File, physicianid, fileName);
                    physician.IsAgreementDoc = new BitArray(new[] { true });
                }
                if (fileName == "Background")
                {
                    var docfile = _uploadprovider.UploadDocFile(File, physicianid, fileName);
                    physician.IsBackgroundDoc = new BitArray(new[] { true });
                }
                if (fileName == "Hippa")
                {
                    var docfile = _uploadprovider.UploadDocFile(File, physicianid, fileName);
                    physician.IsTrainingDoc = new BitArray(new[] { true });
                }
                if (fileName == "NonDiscoluser")
                {
                    var docfile = _uploadprovider.UploadDocFile(File, physicianid, fileName);
                    physician.IsNonDisclosureDoc = new BitArray(new[] { true });
                }
                if (fileName == "License")
                {
                    var docfile = _uploadprovider.UploadDocFile(File, physicianid, fileName);
                    physician.IsLicenseDoc = new BitArray(new[] { true });
                }
                _context.Physicians.Update(physician);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
		

        
	}
}



//join region in _context.Regions on reqclient.RegionId equals region.RegionId