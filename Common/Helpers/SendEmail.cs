using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SendEmail
    {
        private static readonly string SmtpClientAddress_Gmail = ConfigurationManager.AppSettings["SmtpClientAddress_Gmail"].ToString();
        private static readonly string SmtpClientAddress_VNUmail = ConfigurationManager.AppSettings["SmtpClientAddress_VNUmail"].ToString();

        private static readonly string SmtpClientPost_Gmail = ConfigurationManager.AppSettings["SmtpClientPost_Gmail"].ToString();
        private static readonly string SmtpClientPost_VNUmail = ConfigurationManager.AppSettings["SmtpClientPost_VNUmail"].ToString();

        public static bool Send(string displayName, string sender, string senderPassword, string receiver, string subject, string content)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                var client = new SmtpClient(sender.EndsWith("vnu.edu.vn") ? SmtpClientAddress_VNUmail : SmtpClientAddress_Gmail, Convert.ToInt32(sender.EndsWith("vnu.edu.vn") ? SmtpClientPost_VNUmail : SmtpClientPost_Gmail))
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(sender.Split('@')[0], senderPassword),
                    EnableSsl = true
                };
                if (sender.EndsWith("vnu.edu.vn"))
                    ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors
    sslPolicyErrors)
                    { return true; };
                mail.From = new MailAddress(sender, displayName);
                mail.To.Add(StringHelper.KillChars(receiver));
                mail.Subject = subject;
                mail.Body = content;
                client.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Gửi mail có đính kèm
        public static bool Send(string displayName, string sender, string senderPassword, string receiver, string subject, string content, List<string> att)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                var client = new SmtpClient(sender.EndsWith("vnu.edu.vn") ? SmtpClientAddress_VNUmail : SmtpClientAddress_Gmail, Convert.ToInt32(sender.EndsWith("vnu.edu.vn") ? SmtpClientPost_VNUmail : SmtpClientPost_Gmail))
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(sender.Split('@')[0], senderPassword),
                    EnableSsl = true
                };
                if (sender.EndsWith("vnu.edu.vn"))
                    ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors
    sslPolicyErrors)
                    { return true; };
                mail.From = new MailAddress(sender, displayName);
                mail.To.Add(StringHelper.KillChars(receiver));
                mail.Subject = subject;
                mail.Body = content;
                if (att.Any())
                {
                    foreach (var file in att)
                    {
                        if (System.IO.File.Exists(file))
                        {

                            Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                            ContentDisposition disposition = data.ContentDisposition;
                            disposition.CreationDate = System.IO.File.GetCreationTime(file);
                            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                            mail.Attachments.Add(data);
                        }
                    }
                }
                client.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> SendAsync(string displayName, string sender, string senderPassword, string receiver, string subject, string content)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                var client = new SmtpClient(sender.EndsWith("vnu.edu.vn") ? SmtpClientAddress_VNUmail : SmtpClientAddress_Gmail, Convert.ToInt32(sender.EndsWith("vnu.edu.vn") ? SmtpClientPost_VNUmail : SmtpClientPost_Gmail))
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(sender.Split('@')[0], senderPassword),
                    EnableSsl = true
                };
                if (sender.EndsWith("vnu.edu.vn"))
                    ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors
    sslPolicyErrors)
                    { return true; };
                mail.From = new MailAddress(sender, displayName);
                mail.To.Add(StringHelper.KillChars(receiver));
                mail.Subject = subject;
                mail.Body = content;
                await client.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Gửi mail có đính kèm
        public static async Task<bool> SendAsync(string displayName, string sender, string senderPassword, string receiver, string subject, string content, List<string> att)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                var client = new SmtpClient(sender.EndsWith("vnu.edu.vn") ? SmtpClientAddress_VNUmail : SmtpClientAddress_Gmail, Convert.ToInt32(sender.EndsWith("vnu.edu.vn") ? SmtpClientPost_VNUmail : SmtpClientPost_Gmail))
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(sender.Split('@')[0], senderPassword),
                    EnableSsl = true
                };
                if (sender.EndsWith("vnu.edu.vn"))
                    ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors
    sslPolicyErrors)
                    { return true; };
                mail.From = new MailAddress(sender, displayName);
                mail.To.Add(StringHelper.KillChars(receiver));
                mail.Subject = subject;
                mail.Body = content;
                if (att.Any())
                {
                    foreach (var file in att)
                    {
                        if (System.IO.File.Exists(file))
                        {

                            Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                            ContentDisposition disposition = data.ContentDisposition;
                            disposition.CreationDate = System.IO.File.GetCreationTime(file);
                            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                            mail.Attachments.Add(data);
                        }
                    }
                }
                await client.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
