using AspNetCoreHero.ToastNotification.Abstractions;
using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModel;
using System.Collections;
using System.Web.WebPages;

namespace BAL.Repository
{
    public class AdminAction : IAdminAction
    {

        private readonly ApplicationDbContext _context;
        private readonly IAdminDashBoard _adminDashBoard;
		private readonly INotyfService _notyf;

		public AdminAction(ApplicationDbContext context,IAdminDashBoard adminDashBoard,INotyfService notyfService)
        { 
            _context = context;
            _adminDashBoard = adminDashBoard;
            _notyf = notyfService;
        }
        public bool CancelCase(int Requestid, string Reason, string Notes)
        {
            var user = _context.Requests.FirstOrDefault(h => h.RequestId == Requestid);


            if (user != null)
            {
                user.Status = 3;
                user.CaseTag = Reason;


                RequestStatusLog requeststatuslog = new RequestStatusLog();

                requeststatuslog.RequestId = Requestid;
                requeststatuslog.Notes = Notes;
                requeststatuslog.CreatedDate = DateTime.Now;
                requeststatuslog.Status = 3;

                _context.Add(requeststatuslog);
                _context.SaveChanges();

                _context.Update(user);
                _context.SaveChanges();

                return true;
            }

            else
            { return false; }

        }


        public ViewCase ViewCase(int id, int status)
        {
            var patientdata = _adminDashBoard.getregionwise().Where(s => s.reqclientid == id).FirstOrDefault();
            ViewCase viewCase = new ViewCase();
            viewCase.FirstName = patientdata.Name;
            viewCase.LastName = patientdata.LastName;
            viewCase.DateOfBirth = DateTime.Parse(patientdata.BirthDate);
            viewCase.Phone = patientdata.PhoneNumber_P;
            viewCase.Email = patientdata.Email;
            viewCase.Notes = patientdata.Notes;
            viewCase.region = patientdata.regionname;
            viewCase.address = patientdata.Address;
            viewCase.ConfirmationNumber = patientdata.confirmationnum;
            viewCase.status = status;
            viewCase.requestid = patientdata.requestid;
            viewCase.cases = _context.CaseTags.ToList();

            return viewCase;
        }




        public void AssignCase(int req, string Description, string phyid)
        {
            var user = _context.Requests.FirstOrDefault(h => h.RequestId == req);

            if (user != null)
            {
     
                user.ModifiedDate = DateTime.Now;
                user.PhysicianId = int.Parse(phyid);

                _context.Update(user);
                _context.SaveChanges();

                RequestStatusLog requeststatuslog = new RequestStatusLog();

                requeststatuslog.RequestId = req;
                requeststatuslog.Notes = Description;
                requeststatuslog.CreatedDate = DateTime.Now;
            

                _context.Add(requeststatuslog);
                _context.SaveChanges();

            }
        }

        public void TransferCase(int transferid, string Descriptionoftra, string phyidtra)
        {
            var user = _context.Requests.FirstOrDefault(h => h.RequestId == transferid);

            if (user != null)
            {
                user.Status = 2;
                user.ModifiedDate = DateTime.Now;
                user.PhysicianId = int.Parse(phyidtra);

                _context.Update(user);
                _context.SaveChanges();

                RequestStatusLog requeststatuslog = new RequestStatusLog();

                requeststatuslog.RequestId = transferid;
                requeststatuslog.Notes = Descriptionoftra;
                requeststatuslog.CreatedDate = DateTime.Now;
                requeststatuslog.Status = 2;

                _context.Add(requeststatuslog);
                _context.SaveChanges();

            }
        }

        public void BlockCase(int blocknameid, string blocknotes)
        {
            var user = _context.Requests.FirstOrDefault(h => h.RequestId == blocknameid);

            if (user != null)
            {
                user.Status = 11;


                _context.Update(user);
                _context.SaveChanges();

                RequestStatusLog requeststatuslog = new RequestStatusLog();

                requeststatuslog.RequestId = blocknameid;
                requeststatuslog.Notes = blocknotes ?? "--";
                requeststatuslog.CreatedDate = DateTime.Now;
                requeststatuslog.Status = 11;

                _context.Add(requeststatuslog);
                _context.SaveChanges();

                BlockRequest blockRequest = new BlockRequest();

                blockRequest.RequestId = blocknameid.ToString();
                blockRequest.CreatedDate = DateTime.Now;
                blockRequest.Email = user.Email;
                blockRequest.PhoneNumber = user.PhoneNumber;
                blockRequest.Reason = blocknotes ?? "--";
                blockRequest.IsActive = new System.Collections.BitArray(new[] { true });

                _context.Add(blockRequest);
                _context.SaveChanges();
            }
        }

        public void ClearCase(int clearcaseid)
        {
            var request = _context.Requests.FirstOrDefault(s => s.RequestId == clearcaseid);

            if (request != null)
            {
                request.Status = 10;

                _context.Update(request);
                _context.SaveChanges();

                RequestStatusLog requeststatuslog = new RequestStatusLog();

                requeststatuslog.RequestId = clearcaseid;
                requeststatuslog.CreatedDate = DateTime.Now;
                requeststatuslog.Status = 10;

                _context.Add(requeststatuslog);
                _context.SaveChanges();

            }
        }

        public void SendOrder(SendOrder sendOrder)
        {
            OrderDetail orderDetail = new OrderDetail();

            orderDetail.RequestId = sendOrder.requestid;
            orderDetail.VendorId = sendOrder.vendorId;
            orderDetail.FaxNumber = sendOrder.FaxNum;
            orderDetail.Email = sendOrder.Email;
            orderDetail.BusinessContact = sendOrder.BusinessContact;
            orderDetail.Prescription = sendOrder.Disciription;
            orderDetail.CreatedDate = DateTime.Now;

            _context.Add(orderDetail);
            _context.SaveChanges();
        }

        public CloseCase CloseCase(int requestid)
        {
            var requestClient = _context.RequestClients.FirstOrDefault(s => s.RequestId == requestid);
            var docData = _context.RequestWiseFiles.Where(s => s.RequestId == requestid).ToList();
            var Confirmationnum = _context.Requests.FirstOrDefault(s => s.RequestId == requestid).ConfirmationNumber;

            CloseCase closeCase = new CloseCase();
            if (docData != null && requestClient != null)
            {
                closeCase.FirstName = requestClient.FirstName;
                closeCase.LastName = requestClient.LastName;
                closeCase.Email = requestClient.Email;
                closeCase.Phonenum = requestClient.PhoneNumber;
                closeCase.DateOfBirth = (new DateOnly((int)requestClient.IntYear, int.Parse(requestClient.StrMonth), (int)requestClient.IntDate));
                closeCase.Files = docData;
                closeCase.ConfirmationNum = Confirmationnum;
                closeCase.requestid = requestid;
            }

            return closeCase;
        }

        public void CloseCasePost(CloseCase closeCase, int id)
        {
            var reqclient = _context.RequestClients.FirstOrDefault(s => s.RequestId == id);

            if (reqclient != null)
            {
                reqclient.PhoneNumber = closeCase.Phonenum;
                reqclient.FirstName = closeCase.FirstName;
                reqclient.LastName = closeCase.LastName;
                reqclient.IntDate = closeCase.DateOfBirth.Day;
                reqclient.IntYear = closeCase.DateOfBirth.Year;
                reqclient.StrMonth = closeCase.DateOfBirth.Month.ToString();

                _context.Update(reqclient);
                _context.SaveChanges();
            }
        }

        #region Encounterform
        public Encounter EncounterForm(int id)
        {
            var result = (from req in _context.Requests
                          join
                       reqclient in _context.RequestClients on
            req.RequestId equals reqclient.RequestId
                          join enc in _context.EncounterForms on req.RequestId equals enc.RequestId
                        into reqs
                          from enc in reqs.DefaultIfEmpty()
                          where req.RequestId == id
                          select new Encounter()
                          {
                              FirstName = reqclient.FirstName,
                              LastName = reqclient.LastName,
                              Location = reqclient.Street + " " + reqclient.City,
                              BirthDate = new DateTime((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate),
                              ServiceDate = DateTime.Now,
                              IllnessOrInjury = enc.HistoryOfPresentIllnessOrInjury,
                              MedicalHistory = enc.MedicalHistory,
                              Medications = enc.Medications,
                              Allergies = enc.Allergies,
                              Temprature = enc.Temp,
                              HR = enc.Hr,
                              RR = enc.Rr,
                              SytolicBp = enc.BloodPressureSystolic,
                              DistolicBp = enc.BloodPressureDiastolic,
                              O2 = enc.O2,
                              Pain = enc.Pain,
                              Heent = enc.Heent,
                              Cv = enc.Cv,
                              Chest = enc.Chest,
                              ABD = enc.Abd,
                              Extr = enc.Extremeties,
                              Skin = enc.Skin,
                              Neuro = enc.Neuro,
                              Other = enc.Other,
                              Dignosis = enc.Diagnosis,
                              TreatmentPlan = enc.TreatmentPlan,
                              MedicationDispensed = enc.MedicationsDispensed,
                              Procedures = enc.Procedures,
                              Followup = enc.FollowUp,
                              requestid = id

                          }).FirstOrDefault();

            return result;

        }

        public void EncounterPost(int id, Encounter enc)
        {
            var availabledata = _context.EncounterForms.FirstOrDefault(s => s.RequestId == id);

            if (availabledata != null)
            {
                //update the data already present
                availabledata.HistoryOfPresentIllnessOrInjury = enc.IllnessOrInjury;
                availabledata.MedicalHistory = enc.MedicalHistory;
                availabledata.Medications = enc.Medications;
                availabledata.Allergies = enc.Allergies;
                availabledata.Temp = enc.Temprature;
                availabledata.Hr = enc.HR;
                availabledata.Rr = enc.RR;
                availabledata.BloodPressureSystolic = enc.SytolicBp;
                availabledata.BloodPressureDiastolic = enc.DistolicBp;
                availabledata.O2 = enc.O2;
                availabledata.Pain = enc.Pain;
                availabledata.Heent = enc.Heent;
                availabledata.Cv = enc.Cv;
                availabledata.Chest = enc.Chest;
                availabledata.Abd = enc.ABD;
                availabledata.Extremeties = enc.Extr;
                availabledata.Skin = enc.Skin;
                availabledata.Neuro = enc.Neuro;
                availabledata.Other = enc.Other;
                availabledata.Diagnosis = enc.Dignosis;
                availabledata.TreatmentPlan = enc.TreatmentPlan;
                availabledata.MedicationsDispensed = enc.MedicationDispensed;
                availabledata.Procedures = enc.Procedures;
                availabledata.FollowUp = enc.Followup;
                availabledata.IsFinalize = false;

                _context.Update(availabledata);
                _context.SaveChanges();
            }
            else
            {

                //add the data not present
                EncounterForm encounterForm = new EncounterForm();
                encounterForm.HistoryOfPresentIllnessOrInjury = enc.IllnessOrInjury;
                encounterForm.MedicalHistory = enc.MedicalHistory;
                encounterForm.Medications = enc.Medications;
                encounterForm.Allergies = enc.Allergies;
                encounterForm.Temp = enc.Temprature;
                encounterForm.Hr = enc.HR;
                encounterForm.Rr = enc.RR;
                encounterForm.BloodPressureSystolic = enc.SytolicBp;
                encounterForm.BloodPressureDiastolic = enc.DistolicBp;
                encounterForm.O2 = enc.O2;
                encounterForm.Pain = enc.Pain;
                encounterForm.Heent = enc.Heent;
                encounterForm.Cv = enc.Cv;
                encounterForm.Chest = enc.Chest;
                encounterForm.Abd = enc.ABD;
                encounterForm.Extremeties = enc.Extr;
                encounterForm.Skin = enc.Skin;
                encounterForm.Neuro = enc.Neuro;
                encounterForm.Other = enc.Other;
                encounterForm.Diagnosis = enc.Dignosis;
                encounterForm.TreatmentPlan = enc.TreatmentPlan;
                encounterForm.MedicationsDispensed = enc.MedicationDispensed;
                encounterForm.Procedures = enc.Procedures;
                encounterForm.FollowUp = enc.Followup;
                encounterForm.IsFinalize = false;
                encounterForm.RequestId = id;

                _context.Add(encounterForm);
                _context.SaveChanges();
            }
        }
        #endregion Encounterform

        public bool CloseInstance(int reqid)
        {
            var request = _context.Requests.FirstOrDefault(s => s.RequestId == reqid);

            if (request != null)
            {
                request.Status = 9;
                request.ModifiedDate = DateTime.Now;
                _context.Update(request);
                _context.SaveChanges();


                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.RequestId = reqid;
                requestStatusLog.Status = 9;
                requestStatusLog.CreatedDate = DateTime.Now;

                _context.Add(requestStatusLog);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }


        }

        public IQueryable<Admin_DashBoard> GetRequests(int[] status)
        {
           
            var result = (from req in _context.Requests
                          join reqclient in _context.RequestClients
                          on req.RequestId equals reqclient.RequestId
                          orderby req.CreatedDate
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
                              Isfinalise = _context.EncounterForms.FirstOrDefault(s => s.RequestId == req.RequestId).IsFinalize
                          }).Where(item => status.Any(s => item.status == s));


            return result;
        }

        public List<Scheduling> GetEvents(int region)
        {
            var eventswithoutdelete = (from s in _context.Shifts
                                      join pd in _context.Physicians on s.PhysicianId equals pd.PhysicianId
                                      join sd in _context.ShiftDetails on s.ShiftId equals sd.ShiftId into shiftGroup
                                      from sd in shiftGroup.DefaultIfEmpty()

                                      select new Scheduling
                                      {
                                          Shiftid = sd.ShiftDetailId,
                                          Status = sd.Status,
                                          Starttime = sd.StartTime,
                                          Endtime = sd.EndTime,
                                          Physicianid = pd.PhysicianId,
                                          PhysicianName = pd.FirstName + ' ' + pd.LastName,
                                          Shiftdate = sd.ShiftDate,
                                          ShiftDetailId = sd.ShiftDetailId,
                                          Regionid = sd.RegionId,
                                          ShiftDeleted = sd.IsDeleted[0]
                                      }).Where(item => region == 0 || item.Regionid == region).ToList();
            var events = eventswithoutdelete.Where(item => !item.ShiftDeleted).ToList();
            return events;
        }

        #region Creatshift
        public void CreateShift(Scheduling model, string email)
        {
            var admin = _context.Admins.FirstOrDefault(s => s.Email == email);
	  bool shiftExists = _context.ShiftDetails.Any(sd => sd.Shift.PhysicianId == model.Physicianid &&
	  sd.ShiftDate.Date == model.Startdate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now)).Date &&
	  (sd.StartTime <= model.Endtime||
	  sd.EndTime >= model.Starttime));


            if(!shiftExists)
            {
				Shift shift = new Shift();
				shift.PhysicianId = model.Physicianid;
				shift.StartDate = model.Startdate;
				shift.IsRepeat = new BitArray(new[] { model.Isrepeat });
				shift.RepeatUpto = model.Repeatupto;
				shift.CreatedDate = DateTime.Now;
				shift.CreatedBy = admin.AspNetUserId;
				_context.Shifts.Add(shift);
				_context.SaveChanges();

				ShiftDetail sd = new ShiftDetail();
				sd.ShiftId = shift.ShiftId;
				sd.ShiftDate = new DateTime(model.Startdate.Year, model.Startdate.Month, model.Startdate.Day);
				sd.StartTime = model.Starttime;
				sd.EndTime = model.Endtime;
				sd.RegionId = model.Regionid;
				sd.Status = model.Status;
				sd.IsDeleted = new BitArray(new[] { false });


				_context.ShiftDetails.Add(sd);
				_context.SaveChanges();

				ShiftDetailRegion sr = new ShiftDetailRegion();
				sr.ShiftDetailId = sd.ShiftDetailId;
				sr.RegionId = (int)model.Regionid;
				sr.IsDeleted = new BitArray(new[] { false });
				_context.ShiftDetailRegions.Add(sr);
				_context.SaveChanges();

				if (shift.IsRepeat[0])
				{
					var stringArray = model.checkWeekday.Split(",");
					foreach (var weekday in stringArray)
					{

						DateTime startDateForWeekday = model.Startdate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now)).AddDays((7 + int.Parse(weekday) - (int)model.Startdate.DayOfWeek) % 7);


						if (startDateForWeekday < model.Startdate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now)))
						{
							startDateForWeekday = startDateForWeekday.AddDays(7); // Add 7 days to move it to the next occurrence
						}

						// Iterate over Refill times
						for (int i = 0; i < shift.RepeatUpto; i++)
						{
							bool shiftDetailsExists = _context.ShiftDetails.Any(sd => sd.Shift.PhysicianId == model.Physicianid &&
	                        sd.ShiftDate.Date == model.Startdate.ToDateTime(TimeOnly.FromDateTime(DateTime.Now)).Date &&
	                        (sd.StartTime <= model.Endtime ||
	                         sd.EndTime >= model.Starttime));
							// Create a new ShiftDetail instance for each occurrence

                            if(!shiftDetailsExists)
                            {
								ShiftDetail shiftDetail = new ShiftDetail
								{
									ShiftId = shift.ShiftId,
									ShiftDate = startDateForWeekday.AddDays(i * 7), // Add i  7 days to get the next occurrence
									RegionId = (int)model.Regionid,
									StartTime = model.Starttime,
									EndTime = model.Endtime,
									Status = 0,
									IsDeleted = new BitArray(new[] { false })
								};

								// Add the ShiftDetail to the database context
								_context.Add(shiftDetail);
								_context.SaveChanges();
							}
                            else
                            {

                                  _notyf.Error("shift already exist");
                            }
							
						}
					}
				}
			}
            else
            {
                _notyf.Error("shift already exist");
            }
			
        }

        #endregion Creatshift


    }
}
