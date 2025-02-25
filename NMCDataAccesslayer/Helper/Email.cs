using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using NMCDataAccesslayer.DataModel;

namespace NMCDataAccesslayer.Helper
{
    public static class Email
    {
        public static async Task SendMailtoUser(string emailid, string subject, string body)
        {
            try
            {
                if (emailid != "")
                {
                    SmtpClient smtpClient = new SmtpClient();
                    MailMessage mailMessage = new MailMessage();

                    mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mailMessage.To.Add(new MailAddress(emailid));
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    smtpClient.Host = ConfigurationManager.AppSettings["smtpServer"].ToString();
                    smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
                    smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["smtpSSL"]);
                    smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"].ToString(), ConfigurationManager.AppSettings["smtpPass"].ToString());
                    //smtpClient.Send(mailMessage);

                    try
                    {
                        mailMessage.Priority = MailPriority.High;
                        mailMessage.IsBodyHtml = true;
                        await Task.Yield();
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                throw ex;
            }
        }
        public static async Task SendSMStoUser(string contacts, string msg)
        {
            try
            {
                if (contacts != "")
                {
                    foreach (var mobile in contacts.Split(',')) {
                        int j = msg.Length;
                        var client = new RestClient();
                        var request = new RestRequest();

                        IRestResponse response;
                        //string pl = "http://www.alots.in/sms-panel/api/http/index.php?username=pravin123&apikey=FB53D-D6D2B&apirequest=Text&sender=TSTMSG&mobile="+ contacts+ ",9479630784" + "&message="+ msg + "&route=TRANS&format=JSON";
                        string pl = "http://www.alots.in/sms-panel/api/http/index.php?username=pravin123T&apikey=7DB1F-DA016&apirequest=Text&sender=NMCGAS&mobile=" + mobile + "" + "&message=" + msg + "&route=TRANS&format=JSON";
                        client = new RestClient(pl);
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                        request = new RestRequest(Method.POST);
                        request.AddHeader("content-type", "application/json");
                        ///await Task.Yield();
                        response = client.Execute(request);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        public static async Task SendMailtoMultipleUser(string emailid, string subject, string body)
        {
            try
            {
            foreach ( var e   in  emailid.Split(','))
                {
                    SmtpClient smtpClient = new SmtpClient();
                    MailMessage mailMessage = new MailMessage();

                    mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                    mailMessage.To.Add(new MailAddress(e));
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    smtpClient.Host = ConfigurationManager.AppSettings["smtpServer"].ToString();
                    smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
                    smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["smtpSSL"]);
                    smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUser"].ToString(), ConfigurationManager.AppSettings["smtpPass"].ToString());
                    //smtpClient.Send(mailMessage);

                    try
                    {
                        mailMessage.Priority = MailPriority.High;
                        mailMessage.IsBodyHtml = true;
                        await Task.Yield();
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                throw ex;
            }
        }
    }
}
