using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
using System.Net.Mail;
using System.IO;
using System.Net;
using log4net;
using System.Collections;
using System.Text.RegularExpressions;
using AMS.Core.Helper.HtmlParser;


using System.Net.Mime;
using System.Collections.Generic;


namespace BestRadioNet.Core.Helper
{


    /// <summary>
    /// Summary description for MailUtil
    /// </summary>
    public static class MailHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MailHelper));

        public static int READY_SEND = 0;
        public static int SEND_SUCCESS = 1;
        public static int SEND_FAIL = 2;


        


        public static void Send(string SMTPServer, int SMTPPort, string fromFromAddress, string toEmailAddress, string subject, string body, bool isBodyHtml)
        {

            Send(SMTPServer, SMTPPort, fromFromAddress, toEmailAddress, subject, body, isBodyHtml, null);
        }



        public static void Send(string SMTPServer, int SMTPPort, string fromFromAddress, string toEmailAddress, string subject, string body, bool isBodyHtml, string[] attachFilePath)
        {

            try
            {

                log.Info("Send Mail Begin");

                //SmtpClient smtpClient = new SmtpClient(SMTPServer,SMTPPort);
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                smtpClient.Port = SMTPPort;
                smtpClient.Host = SMTPServer;

                MailMessage mailMessage = new MailMessage();


                IList embeddedImageList = new ArrayList();

                mailMessage.From = new MailAddress(fromFromAddress);

                char[] delimiterChars = { ',' };

                string text = toEmailAddress;

                string[] words = text.Split(delimiterChars);

                foreach (string s in words)
                {
                    mailMessage.To.Add(s);


                }

                // mailMessage.To.Add(toEmailAddress);

                mailMessage.IsBodyHtml = isBodyHtml;

                mailMessage.Subject = subject;

                log.Info("Send Mail Begin");

                if (isBodyHtml)
                {
                    log.Info("Mail Body HTML");

                    string webContent = ScreenScrapeHtml(body);

                    //log.Info("Body Content="+webContent);

                    HtmlDocument doc = HtmlDocument.Create(webContent);

                    HtmlNodeCollection hc = doc.Nodes;

                    GetImageArrayList(hc, ref embeddedImageList);

                    if (embeddedImageList.Count > 0)
                    {
                        for (int i = 0; i < embeddedImageList.Count; i++)
                        {
                            string str = embeddedImageList[i].ToString();

                            log.Info("i=" + i + ":ImagePath=" + str);

                            webContent = webContent.Replace(str, "cid:image" + i.ToString());
                        }
                    }

                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(webContent, null, "text/html");

                    if (embeddedImageList.Count > 0)
                    {
                        for (int i = 0; i < embeddedImageList.Count; i++)
                        {
                            //�[�J�Ҧ���Embedded�Ϥ--------------------------------------
                            string imageUrl = GetRealImagePath(body, embeddedImageList[i].ToString());
                            log.Info("Image URL=" + imageUrl + "Start");

                            WebRequest imageCID = HttpWebRequest.Create(imageUrl);

                            imageCID.Credentials = CredentialCache.DefaultCredentials;

                            Stream sr = imageCID.GetResponse().GetResponseStream();

                            LinkedResource logo = new LinkedResource(sr);

                            logo.ContentId = "image" + i.ToString();

                            htmlView.LinkedResources.Add(logo);

                            log.Info("Get Image OK");

                            //Embedded�Ϥ-------------------------------------------------
                        }
                    }

                    mailMessage.AlternateViews.Add(htmlView);
                }
                else
                {
                    mailMessage.Body = body;
                }

                //加上附件檔
                if (attachFilePath != null)
                {
                    foreach (string s in attachFilePath)
                    {
                        Attachment data = new Attachment(s, MediaTypeNames.Application.Octet);
                        // Add time stamp information for the file.
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(s);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(s);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(s);

                        mailMessage.Attachments.Add(data);

                    }


                }

                //mailMessage.Body = body;
                log.Info("Send Mail");
                smtpClient.Send(mailMessage);
            }
            catch (Exception err)
            {
                string errMsg = "";

                if (err.InnerException != null)
                {
                    errMsg = err.InnerException.Message;    
                }


                log.Info("SMTPServer="+SMTPServer+";SMTPPort="+SMTPPort+";fromFromAddress="+fromFromAddress+";toEmailAddress="+toEmailAddress+";subject="+subject+";body="+isBodyHtml);
                log.Info("ErrMsg"+err.Message+"Stack trace"+err.StackTrace);
                log.Info("ErrMsg" + errMsg);

                throw err;
                
            }
        }

        public static void SendBodyHtml(string SMTPServer, int SMTPPort, string fromFromAddress, string toEmailAddress, string subject, string body, bool isBodyHtml, List<LinkedResource> linkResourceList)
        {

            SendBodyHtml(SMTPServer, SMTPPort, fromFromAddress, toEmailAddress, subject, body, isBodyHtml, null, linkResourceList);
        }

        public static void SendBodyHtml(string SMTPServer, int SMTPPort, string fromFromAddress, string toEmailAddress, string subject, string body, bool isBodyHtml, string[] attachFilePath, List<LinkedResource> linkResourceList)
        {

            try
            {
                //SmtpClient smtpClient = new SmtpClient(SMTPServer,SMTPPort);
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                smtpClient.Port = SMTPPort;
                smtpClient.Host = SMTPServer;

                MailMessage mailMessage = new MailMessage();


                IList embeddedImageList = new ArrayList();

                mailMessage.From = new MailAddress(fromFromAddress);

                char[] delimiterChars = { ',' };

                string text = toEmailAddress;

                string[] words = text.Split(delimiterChars);

                foreach (string s in words)
                {
                    mailMessage.To.Add(s);


                }

                mailMessage.IsBodyHtml = isBodyHtml;

                mailMessage.Subject = subject;

                AlternateView htmlView = null;

                if (isBodyHtml)
                {
                    htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                }
                else
                {
                    mailMessage.Body = body;
                }

                if (linkResourceList != null)
                {
                    foreach (LinkedResource s in linkResourceList)
                    {
                        htmlView.LinkedResources.Add(s);
                    }
                }

                //加上附件檔
                if (attachFilePath != null)
                {
                    foreach (string s in attachFilePath)
                    {
                        Attachment data = new Attachment(s, MediaTypeNames.Application.Octet);
                        // Add time stamp information for the file.
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(s);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(s);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(s);

                        mailMessage.Attachments.Add(data);

                    }


                }

                if (isBodyHtml)
                {
                    mailMessage.AlternateViews.Add(htmlView);
                }
                else
                {
                    mailMessage.Body = body;
                }


                //mailMessage.Body = body;   
                smtpClient.Send(mailMessage);
            }
            catch (Exception err)
            {
                string errMsg = "";

                if (err.InnerException != null)
                {
                    errMsg = err.InnerException.Message;
                }


                log.Info("SMTPServer=" + SMTPServer + ";SMTPPort=" + SMTPPort + ";fromFromAddress=" + fromFromAddress + ";toEmailAddress=" + toEmailAddress + ";subject=" + subject + ";body=" + isBodyHtml);
                log.Info("ErrMsg" + err.Message + "Stack trace" + err.StackTrace);
                log.Info("ErrMsg2" + errMsg);

                throw err;
                
            }
        }

        private static string GetRealImagePath(string url, string imagePath)
        {
            string regexPattern = @"^(?<s1>(?<s0>[^:/\?#]+):)?(?<a1>"
                                 + @"//(?<a0>[^/\?#]*))?(?<p0>[^\?#]*)"
                                 + @"(?<q1>\?(?<q0>[^#]*))?"
                                 + @"(?<f1>#(?<f0>.*))?";

            Regex re = new Regex(regexPattern, RegexOptions.ExplicitCapture);
            Match m = re.Match(url);
            if (m.Success)
            {
                string httpUrl = m.Groups["s1"].Value + m.Groups["a1"].Value;

                if (imagePath.Substring(0, 1).Equals("/"))
                {
                    imagePath = httpUrl + imagePath;
                }
                else if (imagePath.Substring(0, 1).Equals("."))
                {

                }
                else
                {
                    imagePath = httpUrl + m.Groups["p0"].Value.Substring(0, m.Groups["p0"].Value.LastIndexOf("/") + 1) + imagePath;
                }
            }
            else
            {
                throw new Exception("url is Exist");
            }


            return imagePath;
        }



        public static void GetImageArrayList(HtmlNodeCollection nodes, ref IList imgArrayList)
        {
            //ListItem li = null;
            foreach (HtmlNode t in nodes)
            {

                if (t is HtmlElement)
                {
                    
                    if (IsProcessElement((HtmlElement)t))
                    {

                        foreach (HtmlAttribute arrt in ((HtmlElement)t).Attributes)
                        {

                            if (arrt.Value != null)
                            {
                                if (arrt.Value.ToLower().IndexOf(".jpg") > 0 || arrt.Value.ToLower().IndexOf(".gif") > 0)
                                {
                                    imgArrayList.Add(arrt.Value);
                                }
                            }

                        }


                    }

                }


                if (t is HtmlElement)
                {
                    GetImageArrayList(((HtmlElement)t).Nodes, ref imgArrayList);
                }
            }
        }

        private static bool IsProcessElement(HtmlElement element)
        {
            bool isFlag = false;

            foreach (HtmlAttribute t in element.Attributes)
            {

                if (t.Value != null)
                {
                    if (t.Value.ToLower().IndexOf(".jpg") > 0 || t.Value.ToLower().IndexOf(".gif") > 0)
                    {
                        isFlag = true;
                        break;
                    }
                }

            }

            return isFlag;
        }

        /// <summary>
        /// �N�ǤJurl���ഫ��string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ScreenScrapeHtml(string url)
        {
            log.Info("URL="+url);
            WebRequest objRequest = System.Net.HttpWebRequest.Create(url);
            objRequest.Credentials = CredentialCache.DefaultCredentials;

            log.Info("StreamReader");
            StreamReader sr = new StreamReader(objRequest.GetResponse().GetResponseStream());

            log.Info("ReadToEnd");
            string result = sr.ReadToEnd();
                        
            sr.Close();
            log.Info("result"+result);
            return result;
        }


    }
}