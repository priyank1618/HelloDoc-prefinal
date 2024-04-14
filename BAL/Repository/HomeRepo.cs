using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModel;
using DAL.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BAL.Repository
{
    public class HomeRepo : IHome
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<CreateAccount> _passwordHasher;
        public HomeRepo(ApplicationDbContext context,IPasswordHasher<CreateAccount> passwordHasher) 
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public void AddData(CreateAccount createAccount)
        {
           var reqClient = _context.RequestClients.FirstOrDefault(s => s.Email == createAccount.Email);
            if (reqClient != null)
            {
                 AspNetUser aspnetUser = new AspNetUser();
                 Guid id = Guid.NewGuid();
                aspnetUser.AspNetUserId = id.ToString();


                aspnetUser.UserName = String.Concat(reqClient.FirstName, ' ', reqClient.LastName);
                aspnetUser.Email = reqClient.Email;
                aspnetUser.PasswordHash = _passwordHasher.HashPassword(null, createAccount.Password);
                aspnetUser.PhoneNumber = reqClient.PhoneNumber;
                aspnetUser.CreatedDate = DateTime.Now;

                _context.AspNetUsers.Add(aspnetUser);
                _context.SaveChanges();

                User user = new User();
                user.AspNetUserId = aspnetUser.AspNetUserId;
                user.FirstName = reqClient.FirstName;
                user.LastName = reqClient.LastName;
                user.Email = reqClient.Email;
                user.Mobile = reqClient.PhoneNumber;
                user.CreatedDate = DateTime.Now;
                user.Street = reqClient.Street;
                user.City = reqClient.City;
                user.State = reqClient.State;
                user.ZipCode = reqClient.ZipCode;
                user.IntYear = reqClient.IntYear;
                user.IntDate = reqClient.IntDate;
                user.StrMonth = (reqClient.StrMonth);
                user.CreatedBy = reqClient.FirstName;
                user.CreatedDate = DateTime.Now;
                _context.Users.Add(user);
                _context.SaveChanges();

                AspNetUserRole role = new AspNetUserRole();
                role.RoleId = 2.ToString();
                role.UserId = aspnetUser.AspNetUserId;

                _context.AspNetUserRoles.Add(role);
                _context.SaveChanges();

            }
        }
    }
}
