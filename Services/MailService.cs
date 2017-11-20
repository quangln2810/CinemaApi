using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Services
{
    public class MailService
    {
        public interface IMessageService
        {
            Task Send(string email, string subject, string message);
        }

        public class FileMessageService : IMessageService
        {
            Task IMessageService.Send(string email, string subject, string message)
            {
                var emailContent = $"To: {email}\nSubject: {subject}\nMessage: {message}\n\n";
                File.AppendAllText("emails.txt", emailContent);
                return Task.FromResult(0);
            }
        }
    }
}
