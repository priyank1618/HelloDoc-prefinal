using DAL.DataModels;
using DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IAdminDashboardRecords
    {
        public List<SearchRecords> SearchRecords(int[] status, string patientName,
            string providername, string PhoneNum, string email, string requesttype, int pagesize = 5, int currentpage = 1);

        public List<PatientHistory> ExploreRecords(int userid);

        public List<BlockHistory> BlockedPatientRecords(string email, string name, string phone, DateTime date);

        public List<User> PatientRecords(string firstName, string lastName, string email, string phone, int currentpage, int pagesize);
    }
}
