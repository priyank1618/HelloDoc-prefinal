using DAL.DataModels;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IPatient_Request
    {
        public void AddPatient(Patient patient);

        public Request GetUserByEmail(string email);

       public void RequestWiseFile(string filename, int Requestid);

       // public string GetConfirmationNumber(User user);
    }
}
