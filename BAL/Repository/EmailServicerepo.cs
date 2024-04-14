using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using System.Net;
using System.Net.Mail;

namespace BAL.Repository
{
    public class EmailServicerepo : IEmailService
    {
        private readonly ApplicationDbContext _context;

        public EmailServicerepo(ApplicationDbContext context)
        {
            _context = context;

        }

        public void EmailLog(string template, string subject, string email, int requestid = 0, int adminId = 0, int physicianId = 0, string confirmationnum = null, string filePath = null)
        {
            EmailLog emailLog = new EmailLog();

            emailLog.EmailTemplate = template;
            emailLog.SubjectName = subject;
            emailLog.EmailId = email;
            emailLog.RequestId = requestid;
            emailLog.AdminId = adminId;
            emailLog.PhysicianId = physicianId;
            emailLog.ConfirmationNumber = confirmationnum;
            emailLog.FilePath = filePath;
            emailLog.SentTries = 1;
            emailLog.SentDate = DateTime.Now;
            emailLog.CreateDate = DateTime.Now;
            emailLog.IsEmailSent = new System.Collections.BitArray(new[] { true });
            emailLog.RoleId = 1;

            _context.EmailLogs.Add(emailLog);
            _context.SaveChanges();
        }

        public void SendEmail(string email, string subject, string message, List<Attachment> attachments = null)
        {

            SmtpClient smtpClient = new("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tatva.dotnet.priyankpatel@outlook.com", "Priyank@94095"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            // Create the MailMessage object
            MailMessage mail = new MailMessage("tatva.dotnet.priyankpatel@outlook.com", email, subject, message);
            mail.IsBodyHtml = true;
            attachments?.ForEach(attachment => mail.Attachments.Add(attachment));

            try
            {

                smtpClient.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }

}