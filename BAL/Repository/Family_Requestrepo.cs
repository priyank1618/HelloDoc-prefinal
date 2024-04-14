
using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModel;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class Family_Requestrepo : IFamily_Request
    {

        private readonly ApplicationDbContext _context;

        public Family_Requestrepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddData(Other_Request req)
        {
            //other data will be added to the request
            Request request = new Request();

            request.Email = req.EmailOther;
            request.PhoneNumber = req.PhoneNumberother;
            request.FirstName = req.FirstNameOther;
            request.LastName = req.LastNameOther;
            request.RelationName = req.Relation;
            request.CreatedDate = DateTime.Now;
            request.RequestTypeId = 2;
            _context.Requests.Add(request);
            _context.SaveChanges();


            //patient data will be added to the request client
            RequestClient requestClient = new RequestClient();

            requestClient.RequestId = request.RequestId;
            requestClient.Email = req.Email_P;
            requestClient.FirstName = req.FirstName_P;
            requestClient.LastName = req.LastName_P;
            requestClient.PhoneNumber = req.PhoneNumber_P;
            requestClient.Street = req.Street;
            requestClient.City = req.City;
            requestClient.State = _context.Regions.FirstOrDefault(s => s.RegionId == int.Parse(req.State)).Name;
            requestClient.ZipCode = req.Zipcode;
            requestClient.StrMonth = req.BirthDate_P.Month.ToString();
            requestClient.IntYear = req.BirthDate_P.Year;
            requestClient.IntDate = req.BirthDate_P.Day;
            requestClient.RegionId = int.Parse(req.State);
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            var region = _context.Regions.FirstOrDefault(x => x.RegionId == requestClient.RegionId);
            var count = _context.Requests.Where(x => x.CreatedDate.Date == request.CreatedDate.Date).Count();

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


        }
    }
}