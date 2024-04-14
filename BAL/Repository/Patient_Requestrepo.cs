using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography.X509Certificates;

namespace BAL.Repository
{
    public class Patient_Requestrepo : IPatient_Request
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Patient> _passwordHasher;

        public Patient_Requestrepo(ApplicationDbContext context, IPasswordHasher<Patient> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public void AddPatient(Patient patient)
        {
            AspNetUser aspnetUser = new AspNetUser();
            User user = new User();
            Request request = new Request();


            //Status shows that user is Exists or not
            var status = _context.Users.FirstOrDefault(User => User.Email == patient.Email);

            RequestClient request_c = new RequestClient();


            if (patient != null && status == null && patient.PasswordHash == patient.Confirmpassword)
            {


                Guid id = Guid.NewGuid();
                aspnetUser.AspNetUserId = id.ToString();


                aspnetUser.UserName = String.Concat(patient.FirstName, ' ', patient.LastName);
                aspnetUser.Email = patient.Email;
                aspnetUser.PasswordHash = _passwordHasher.HashPassword(null, patient.PasswordHash);
                aspnetUser.PhoneNumber = patient.PhoneNumber;
                aspnetUser.CreatedDate = DateTime.Now;

                _context.AspNetUsers.Add(aspnetUser);
                _context.SaveChanges();



                user.AspNetUserId = aspnetUser.AspNetUserId;
                user.FirstName = patient.FirstName;
                user.LastName = patient.LastName;
                user.Email = patient.Email;
                user.Mobile = patient.PhoneNumber;
                user.CreatedDate = DateTime.Now;
                user.Street = patient.Street;
                user.City = patient.City;
                user.State = _context.Regions.FirstOrDefault(s => s.RegionId == int.Parse(patient.State)).Name;
                user.ZipCode = patient.ZipCode;
                user.IntYear = patient.BirthDate.Value.Year;
                user.IntDate = patient.BirthDate.Value.Day;
                user.StrMonth = (patient.BirthDate.Value.Month).ToString();
                user.CreatedBy = patient.FirstName;
                user.CreatedDate = DateTime.Now;
                _context.Users.Add(user);
                _context.SaveChanges();

                request.UserId = user.UserId;
                request.FirstName = patient.FirstName;
                request.LastName = patient.LastName;
                request.Email = patient.Email;
                request.PhoneNumber = patient.PhoneNumber;
                request.CreatedDate = DateTime.Now;
                request.RequestTypeId = 1;
                _context.Requests.Add(request);
                _context.SaveChanges();


                request_c.RequestId = request.RequestId;
                request_c.FirstName = patient.FirstName;
                request_c.LastName = patient.LastName;
                request_c.Email = patient.Email;
                request_c.PhoneNumber = patient.PhoneNumber;
                request_c.Street = patient.Street;
                request_c.City = patient.City;
                request_c.State = user.State;
                request_c.ZipCode = patient.ZipCode;
                request_c.IntYear = patient.BirthDate.Value.Year;
                request_c.IntDate = patient.BirthDate.Value.Day;
                request_c.StrMonth = (patient.BirthDate.Value.Month).ToString();


                if (patient.State != null)
                {
                    request_c.RegionId = int.Parse(patient.State);
                }

                _context.RequestClients.Add(request_c);
                _context.SaveChanges();

                var region = _context.Regions.FirstOrDefault(x => x.RegionId == request_c.RegionId);
                var count = _context.Requests.Where(x => x.CreatedDate.Date == request.CreatedDate.Date).Count();


                //-------------------------------------
                if (region != null)
                {
                    var confirmationnum = region.Abbreviation.ToUpper() + request.CreatedDate.ToString("ddMMyy") +
                        request_c.LastName.Substring(0, 2).ToUpper() + request_c.FirstName.Substring(0, 2).ToUpper() + count.ToString("D4");
                    request.ConfirmationNumber = confirmationnum;
                }
                else
                {
                    var confirmationnum = "AB" + request.CreatedDate.ToString("ddMMyy") +
                        request_c.LastName.Substring(0, 2).ToUpper() + request_c.FirstName.Substring(0, 2).ToUpper() + count.ToString("D4");
                    request.ConfirmationNumber = confirmationnum;
                }


                _context.Update(request);
                _context.SaveChanges();
            }
            else if (patient.PasswordHash == patient.Confirmpassword)
            {

                request.UserId = status.UserId;
                request.FirstName = patient.FirstName;
                request.LastName = patient.LastName;
                request.Email = patient.Email;
                request.PhoneNumber = patient.PhoneNumber;
                request.CreatedDate = DateTime.Now;
                request.RequestTypeId = 1;
                _context.Requests.Add(request);
                _context.SaveChanges();

                var users = _context.Users.FirstOrDefault(x => x.Email == request.Email);


                request_c.RequestId = request.RequestId;
                request_c.FirstName = patient.FirstName;
                request_c.LastName = patient.LastName;
                request_c.Email = patient.Email;
                request_c.PhoneNumber = patient.PhoneNumber;
                request_c.Street = patient.Street;
                request_c.City = patient.City;
                request_c.State = patient.State;
                request_c.ZipCode = patient.ZipCode;
                request_c.IntYear = patient.BirthDate.Value.Year;
                request_c.IntDate = patient.BirthDate.Value.Day;
                request_c.StrMonth = (patient.BirthDate.Value.Month).ToString();

                if (users != null)
                {
                    request_c.RegionId = users.RegionId;
                }

                _context.RequestClients.Add(request_c);
                _context.SaveChanges();

                var region = _context.Regions.FirstOrDefault(x => x.RegionId == request_c.RegionId);
                var count = _context.Requests.Where(x => x.CreatedDate.Date == request.CreatedDate.Date).Count();

                if (region != null)
                {
                    var confirmationnum = region.Abbreviation.ToUpper() + request.CreatedDate.ToString("ddMMyy") +
                        request_c.LastName.Substring(0, 2).ToUpper() + request_c.FirstName.Substring(0, 2).ToUpper() + count.ToString("D4");
                    request.ConfirmationNumber = confirmationnum;
                }
                else
                {
                    var confirmationnum = "AB" + request.CreatedDate.ToString("ddMMyy") +
                        request_c.LastName.Substring(0, 2).ToUpper() + request_c.FirstName.Substring(0, 2).ToUpper() + count.ToString("D4");
                    request.ConfirmationNumber = confirmationnum;
                }


                _context.Update(request);
                _context.SaveChanges();


            }
        }
        public Request GetUserByEmail(string email)
        {
            return _context.Requests.OrderBy(e => e.RequestId).LastOrDefault(x => x.Email == email);
        }

        public void RequestWiseFile(string filename, int Requestid)
        {
            //make new obj
            var RequestwiseFile = new RequestWiseFile()
            {
                FileName = filename,
                RequestId = Requestid,
                CreatedDate = DateTime.Now,
            };

            _context.RequestWiseFiles.Add(RequestwiseFile);
            _context.SaveChanges();
        }
    }
}