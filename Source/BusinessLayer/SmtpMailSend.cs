﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Net.Mail;
using System.Text;
using System.Threading;

using DataAccess;
using log4net;

namespace BusinessLayer
{
    public class SmtpMailSend
    {
        private class MailThreadParams
        {
            public SmtpClient Client { get; set; }
            public MailMessage Message { get; set; }
        }

        private SmtpClient client;
        private static object sendingMail = new object();

        public SmtpMailSend()
        {
            try
            {
                client = new SmtpClient();
                PostInitClient();
            }
            catch (Exception ex)
            {
                string errMessage = "Could not initialize SMTP client.";
                throw new BusinessException(errMessage, ex);
            }
        }

        public SmtpMailSend(string smtpServer)
        {
            if (smtpServer == null)
            {
                throw new ArgumentNullException("smtpServer");
            }
            if (smtpServer == string.Empty)
            {
                throw new ArgumentException("smtpServer is empty.");
            }

            try
            {
                client = new SmtpClient(smtpServer);
                PostInitClient();
            }
            catch (Exception ex)
            {
                string errMessage = "Could not initialize SMTP client.";
                throw new BusinessException(errMessage, ex);
            }
        }

        public SmtpMailSend(string smtpServer, Int32 port)
        {
            if (smtpServer == null)
            {
                throw new ArgumentNullException("smtpServer");
            }
            if (smtpServer == string.Empty)
            {
                throw new ArgumentException("smtpServer is empty.");
            }
            if (port <= 0)
            {
                throw new ArgumentException("port is <= 0.");
            }

            try
            {
                client = new SmtpClient(smtpServer, port);
                PostInitClient();
            }
            catch (Exception ex)
            {
                string errMessage = "Could not initialize SMTP client.";
                throw new BusinessException(errMessage, ex);
            }
        }

        /// <summary>
        /// Additionally configures the SMTP client after it is created.
        /// </summary>
        private void PostInitClient()
        {
            if (client == null)
            {
                throw new BusinessException("client is not initialized.");
            }

            if (Configuration.SendMailsViaSSL == true)
            {
                client.EnableSsl = true;
            }
        }

        /// <summary>
        /// Sends message to receiver
        /// </summary>
        public void Send(string receiverMail, string bodyText, string subject)
        {
            if (string.IsNullOrEmpty(receiverMail))
            {
                throw new ArgumentNullException("receiverMail");
            }
            if (string.IsNullOrEmpty(bodyText))
            {
                throw new ArgumentNullException("bodyText");
            }

            if (client == null)
            {
                throw new BusinessException("client is not initialized.");
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException("subject");
            }

            MailMessage message = new MailMessage();

            string receiverAddress = receiverMail;

            message.IsBodyHtml = true;

            message.Body = bodyText;
            message.Subject = subject;
            message.To.Add(receiverAddress);

            try
            {
                ParameterizedThreadStart sendingThreadStart = new ParameterizedThreadStart(SendThreadProc);
                Thread sendingThread = new Thread(sendingThreadStart);
                MailThreadParams sendingThreadParams = new MailThreadParams();

                sendingThreadParams.Client = client;
                sendingThreadParams.Message = message;

                sendingThread.Start(sendingThreadParams);
            }
            catch (Exception ex)
            {
                // Do not include the message body, because it may contain a password.
                string errMsg = string.Format("Could not send email To : '{0}'.", receiverMail);
                throw new BusinessException(errMsg, ex);
            }
        }

        private void SendThreadProc(object threadParams)
        {
            try
            {
                lock (sendingMail)
                {

                    try
                    {
                        ILog log = LogManager.GetLogger(typeof(SmtpMailSend));
                        if (log.IsInfoEnabled)
                        {
                            log.Info("Going to sent mail.");
                        }
                    }
                    catch { }

                    MailThreadParams mailParams = threadParams as MailThreadParams;

                    if (mailParams == null)
                    {
                        string msg = string.Format(
                            "threadParams argument must be of type \"{0}\". Actual type passed: \"{1}\".",
                            typeof(MailThreadParams).FullName,
                            threadParams != null ? threadParams.GetType().FullName : "null");
                        throw new ArgumentException(msg);
                    }
                    if (mailParams.Client == null)
                    {
                        throw new ArgumentException("No Client.", "threadParams");
                    }
                    if (mailParams.Message == null)
                    {
                        throw new ArgumentException("No Message.", "threadParams");
                    }

                    try
                    {
                        client.Send(mailParams.Message);

                        try
                        {
                            ILog log = LogManager.GetLogger(typeof(SmtpMailSend));
                            if (log.IsInfoEnabled)
                            {
                                log.Info("Mail sent.");
                            }
                        }
                        catch { }
                    }
                    finally
                    {
                        mailParams.Message.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                // Must catch and log every exception here, because this is a separate thread.
                try
                {
                    ILog log = LogManager.GetLogger(typeof(SmtpMailSend));
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Sending mail thread failed.", ex);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Sends automatic message to user, body depending on MailKind, newPassword is used only when user resets password
        /// </summary>
        public void SendAutomaticMessageToUser(User receiver, MailKind kind, string newPassword)
        {
            if (receiver == null)
            {
                throw new ArgumentNullException("receiver");
            }

            if (string.IsNullOrEmpty(receiver.email))
            {
                throw new BusinessException(string.Format("user id = {0} cannot receive mail because it is empty (email)"
                    , receiver.ID));
            }

            receiver.UserOptionsReference.Load();

            StringBuilder mailBody = new StringBuilder();
            string subject = "";

            string currentVariant = Tools.ApplicationVariantString;

            switch (kind)
            {
                case MailKind.UserActivationLink:
                    if (receiver.UserOptions.activated == true)
                    {
                        throw new BusinessException(string.Format
                            ("User ID = {0} is already activated, no need to receive activation link mail."
                            , receiver.ID));
                    }

                    subject = Tools.GetResource("sendMailToUserOnRegistrationSubject");

                    mailBody.Append(string.Format("{0} <br /><br />", Tools.GetResource("sendMailToUserOnRegistrationDescription")));
                    mailBody.Append(string.Format("{0} {1} <br /><br />", Tools.GetResource("sendMailToUserOnRegistrationDescription2")
                        , receiver.username));
                    mailBody.Append(string.Format("{0} <br /><br />", Tools.GetResource("sendMailToUserOnRegistrationDescription3")));

                    string activationLink = string.Format("{0}{1}/ActivateAccount.aspx?user={2}&activatekey={3}"
                        , Configuration.SiteDomainAdress, currentVariant, receiver.ID, receiver.UserOptions.activationCode);

                    mailBody.Append(string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a> <br /><br />", activationLink));
                    mailBody.Append(Tools.GetResource("sendMailToUserOnRegistrationDescription4"));

                    mailBody.Append(string.Format(string.Format("<br /><br /> {0}", Tools.GetResource("sendMailToUserOnRegistrationDescription5"))));
                    break;
                case MailKind.ResetPasswordLink:

                    if (receiver.UserOptions.activated == false)
                    {
                        throw new BusinessException(string.Format("user ID = {0} is not activated, thats why he cannot receive reset link mail."
                            , receiver.ID));
                    }

                    if (string.IsNullOrEmpty(receiver.UserOptions.resetPasswordKey))
                    {
                        throw new BusinessException(string.Format("user id = {0} cannot receive reset password link because resetPasswordKey is null"
                            , receiver.ID));
                    }

                    subject = string.Format("{0} '{1}' {2}", Tools.GetResource("sendMailToUserResetPassLinkSubject")
                        , receiver.username, Tools.GetResource("sendMailToUserResetPassLinkSubject2"));

                    mailBody.Append(string.Format("{0} {1}{2} {3} {4} <br /><br />"
                        , Tools.GetResource("sendMailToUserResetPassLinkDescription")
                        , receiver.username, Tools.GetResource("sendMailToUserResetPassLinkDescription2")
                        , Configuration.SiteDomainAdress, Tools.GetResource("sendMailToUserResetPassLinkDescription3")));

                    mailBody.Append(string.Format("{0}<br /><br />"
                        , Tools.GetResource("sendMailToUserResetPassLinkDescription4")));

                    mailBody.Append(string.Format("{0}<br /><br />", Tools.GetResource("sendMailToUserResetPassLinkDescription5")));

                    string resetLink = string.Format("{0}{1}/ActivateAccount.aspx?user={2}&resetkey={3}"
                        , Configuration.SiteDomainAdress, currentVariant, receiver.ID, receiver.UserOptions.resetPasswordKey);

                    mailBody.Append(string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a><br /><br />", resetLink));

                    mailBody.Append(string.Format("{0}", Tools.GetResource("sendMailToUserResetPassLinkDescription6")));
                    break;
                case MailKind.UserNewPassword:

                    if (string.IsNullOrEmpty(newPassword))
                    {
                        throw new BusinessException(string.Format("user id = {0} cannot receive new password mail because NewPassword is empty"
                            , receiver.ID));
                    }
                    if (receiver.UserOptions.activated == false)
                    {
                        throw new BusinessException(string.Format("user ID = {0} is not activated, thats why he cannot receive new password mail."
                            , receiver.ID));
                    }

                    subject = string.Format("{0} '{1}' {2}", Tools.GetResource("sendMailToUserNewPassSubject")
                        , receiver.username, Tools.GetResource("sendMailToUserNewPassSubject2"));

                    mailBody.Append(string.Format("{0} {1}{2}<br /><br />"
                        , Tools.GetResource("sendMailToUserNewPassDescription"), receiver.username
                        , Tools.GetResource("sendMailToUserNewPassDescription2")));

                    mailBody.Append(string.Format("{0} \"{1}\"<br/><br/>", Tools.GetResource("sendMailToUserNewPassDescription3"), newPassword));
                    mailBody.Append(Tools.GetResource("sendMailToUserNewPassDescription4"));

                    mailBody.Append(string.Format("<br /><br /> {0}", Tools.GetResource("sendMailToUserNewPassDescription5")));
                    break;
                default:
                    string msg = string.Format("Unexpected mail kind: \"{0}\".", kind);
                    throw new BusinessException(msg);
            }

            Send(receiver.email, mailBody.ToString(), subject);
        }

        public void SendMessageToSite(string from, string subject, string email, string text, string toSectionEmail)
        {
            if (string.IsNullOrEmpty(from))
            {
                throw new BusinessException("from is null");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new BusinessException("email is null");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new BusinessException("text is null");
            }

            if (string.IsNullOrEmpty(toSectionEmail))
            {
                throw new BusinessException("toSectionEmail is null");
            }

            if (string.IsNullOrEmpty(subject))
            {
                subject = "None";
            }

            StringBuilder messageBody = new StringBuilder();

            messageBody.Append(string.Format("From : {0} <br />", from));
            messageBody.Append(string.Format("Subject : {0} <br />", subject));
            messageBody.Append(string.Format("Email : {0} <br /><br />", email));
            messageBody.Append(string.Format("Text : <br /> {0}<br />{1}"
                , "--------------------------------------------------------------"
                , text));

            messageBody.Append(string.Format("{0}<br /><br /> This message is sent via contact form in About page."
                , "--------------------------------------------------------------"));

            Send(toSectionEmail, messageBody.ToString(), subject);
        }


    }
}
