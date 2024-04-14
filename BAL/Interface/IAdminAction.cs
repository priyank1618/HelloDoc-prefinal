using DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IAdminAction
    {
        public bool CancelCase(int Requestid, string Reason, string Notes);
        public void AssignCase(int req, string Description, string phyid);
        public void TransferCase(int transferid, string Descriptionoftra, string phyidtra);
        public void BlockCase(int blocknameid, string blocknotes);
        public void ClearCase(int clearcaseid);
        public void SendOrder(SendOrder sendOrder);
        public CloseCase CloseCase(int requestid);
        public void CloseCasePost(CloseCase closeCase, int id);
        public Encounter EncounterForm(int id);
        public void EncounterPost(int id, Encounter enc);
        public bool CloseInstance(int reqid);
        public ViewCase ViewCase(int id, int status);

        public IQueryable<Admin_DashBoard> GetRequests(int[] status);



        #region Scheduling methods
        public List<Scheduling> GetEvents(int region);

        public void CreateShift(Scheduling model,string email);

        #endregion
    }
}
