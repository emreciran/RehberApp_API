using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IMailRepository
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
