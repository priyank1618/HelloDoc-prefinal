using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModel;

namespace BAL.Repository
{
    public class Concierge_Requestrepo : IConcierge_Request
    {
        private readonly ApplicationDbContext _context;

        public Concierge_Requestrepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void AddData(Other_Request req)
        {
            //other data will be added to the request
            var request = new Request();
            var requestClient = new RequestClient();
            var concierge = new Concierge();


            request.FirstName = req.FirstNameOther;
            request.LastName = req.LastNameOther;
            request.Email = req.EmailOther;
            request.PhoneNumber = req.PhoneNumberother;
            request.RequestTypeId = 3;
            request.CreatedDate = DateTime.Now;

            _context.Requests.Add(request);
            _context.SaveChanges();



            requestClient.RequestId = request.RequestId;
            requestClient.FirstName = req.FirstName_P;
            requestClient.LastName = req.LastName_P;
            requestClient.Email = req.Email_P;
            requestClient.PhoneNumber = req.PhoneNumber_P;
            requestClient.StrMonth = req.BirthDate_P.Month.ToString();
            requestClient.IntYear = req.BirthDate_P.Year;
            requestClient.IntDate = req.BirthDate_P.Day;
            requestClient.Street = req.Street;
            requestClient.City = req.City;
            requestClient.State = _context.Regions.FirstOrDefault(s => s.RegionId == int.Parse(req.State)).Name;
            requestClient.ZipCode = req.Zipcode;
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

            concierge.ConciergeName = req.FirstNameOther + req.LastNameOther;
            concierge.Address = req.Street + req.City + req.State + req.Zipcode;
            concierge.Street = req.Street;
            concierge.City = req.City;
            concierge.State = req.State;
            concierge.ZipCode = req.Zipcode;
            concierge.RegionId = 1;

            _context.Concierges.Add(concierge);
            _context.SaveChanges();

        }
    }
}