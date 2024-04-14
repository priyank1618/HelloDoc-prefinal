using DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
	public interface IAccountsAccess
	{
		public void CreateProviderAccountPost(CreateProviderAccount CreateProviderAccount, string[] regions);
		public void CreateAdminAccountPost(AdminProfile profile, string[] regions);


		//accesses
		public List<UserAccess> GetUserAccessData(int role);
		public void CreateAccess(int[] rolemenu, string rolename, int accounttype);
	}
}
