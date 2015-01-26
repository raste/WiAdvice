﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using BusinessLayer;
using DataAccess;

namespace UserInterface
{
    public partial class ForgottenPassword : BasePage
    {
        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetRedirrectOnLogIn();
            CheckUser();
            ShowInfo();
        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            SetLocalText();
        }

        private void SetLocalText()
        {
            lblChangePass.Text = GetLocalResourceObject("ToChangePass").ToString();

            lblRetrPass1.Text = GetLocalResourceObject("retrPassInfo1").ToString();
            btnProceedEmail.Text = GetGlobalResourceObject("SiteResources", "Proceed").ToString();
            lblNote.Text = GetLocalResourceObject("note").ToString();

            lblRetrPass2.Text = GetLocalResourceObject("retrPassInfo2").ToString();
            btnProceedSecQnA.Text = GetGlobalResourceObject("SiteResources", "Proceed").ToString();
            btnSecretStep2.Text = GetGlobalResourceObject("SiteResources", "Proceed").ToString();

            lblUsername1.Text = GetLocalResourceObject("username").ToString();
            lblMail.Text = GetLocalResourceObject("mail").ToString();
            btnSendMail.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
            btnCancel.Text = GetGlobalResourceObject("SiteResources", "Cancel").ToString();

            lblUsername2.Text = GetLocalResourceObject("username").ToString();
            lblAnswer.Text = GetLocalResourceObject("answer").ToString();
            lblQuestion.Text = GetLocalResourceObject("question").ToString();
            btnLogIn.Text = GetGlobalResourceObject("SiteResources", "Continue").ToString();
            btnCancel1.Text = GetGlobalResourceObject("SiteResources", "Cancel").ToString();
            btnCancel2.Text = GetGlobalResourceObject("SiteResources", "Cancel").ToString();

        }

        private void CheckUser()
        {
            long currentUserID = CommonCode.CurrentUser.GetCurrentUserId();
            if (currentUserID > 0)
            {
                pnlRetrievePass.Visible = false;
                lblLoggedSucc.Text = GetLocalResourceObject("errIsLogged").ToString();
                lblLoggedSucc.Visible = true;
            }
            else
            {
                pnlRetrievePass.Visible = true;
            }
        }

        protected void btnProceedEmail_Click(object sender, EventArgs e)
        {
            btnProceedEmail.Enabled = false;
            btnProceedSecQnA.Enabled = false;

            pnlByEmail.Visible = true;

        }

        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            phMail.Visible = true;
            phMail.Controls.Add(lblNotif);

            BusinessIpBans businessIpBans = new BusinessIpBans();
            if (businessIpBans.IsThereActiveBanForIpAdress(userContext, Request.UserHostAddress))
            {
                lblNotif.Text = GetLocalResourceObject("errIpBan").ToString();
                return;
            }

            string error = string.Empty;
            if (IsRequestValid(userContext, IpAttemptTry.guessUserAndMail, out error) == false)
            {
                lblNotif.Text = error;
                return;
            }

            bool mailSent = false;

            ccMail.ValidateCaptcha(tbMailCaptcha.Text);
            tbMailCaptcha.Text = "";
            if (ccMail.UserValidated == true)
            {
                if (!string.IsNullOrEmpty(tbMUsername.Text))
                {
                    if (!string.IsNullOrEmpty(tbMemail.Text))
                    {
                        BusinessUser businessUser = new BusinessUser();

                        User user = businessUser.GetByName(userContext, tbMUsername.Text, false, true);
                        if (user != null)
                        {
                            BusinessUserOptions userOptions = new BusinessUserOptions();
                            if (!user.UserOptionsReference.IsLoaded)
                            {
                                user.UserOptionsReference.Load();
                            }

                            if (userOptions.IsUserActivated(user.UserOptions))
                            {
                                if (businessUser.IsUser(user))
                                {
                                    if (user.email == tbMemail.Text)
                                    {
                                        if (userOptions.CanUserCreateNewResetPasswordKey(objectContext, user.UserOptions))
                                        {
                                            userOptions.CreateAndSendNewResetPasswordMail(userContext, objectContext, user.UserOptions);

                                            pnlByEmail.Visible = false;
                                            tbMemail.Text = "";
                                            tbMUsername.Text = "";
                                            btnProceedSecQnA.Enabled = true;
                                            btnProceedEmail.Enabled = true;
                                            phMail.Visible = false;

                                            lblLoggedSucc.Visible = true;
                                            lblLoggedSucc.Text = GetLocalResourceObject("resetMailSent").ToString();

                                            mailSent = true;
                                        }
                                        else
                                        {
                                            lblNotif.Text = GetLocalResourceObject("errMoreTime").ToString();
                                        }
                                    }
                                    else
                                    {
                                        lblNotif.Text = GetLocalResourceObject("errIncMail").ToString();
                                    }
                                }
                                else
                                {
                                    lblNotif.Text = GetLocalResourceObject("errrIncUser").ToString();
                                }
                            }
                            else
                            {
                                lblNotif.Text = GetLocalResourceObject("errUserActivated").ToString();
                            }
                        }
                        else
                        {
                            lblNotif.Text = GetLocalResourceObject("errNoSuchUser").ToString();
                        }
                    }
                    else
                    {
                        lblNotif.Text = GetLocalResourceObject("errNoMail").ToString();
                    }
                }
                else
                {
                    lblNotif.Text = GetLocalResourceObject("errUserName").ToString();
                }
            }
            else
            {
                lblNotif.Text = GetGlobalResourceObject("SiteResources", "errorIncLetters").ToString();
            }

            if (mailSent == false)
            {
                CommonCode.IpAttempts.AttemptWrong(userContext, IpAttemptTry.guessUserAndMail);
            }
        }

        protected void btnProceedSecQnA_Click(object sender, EventArgs e)
        {
            btnProceedEmail.Enabled = false;
            btnProceedSecQnA.Enabled = false;

            pnlBySecQnA.Visible = true;
        }

        protected void btnSecretStep2_Click(object sender, EventArgs e)
        {
            phSecStep1.Visible = true;
            phSecStep1.Controls.Add(lblNotif);

            BusinessIpBans businessIpBans = new BusinessIpBans();
            if (businessIpBans.IsThereActiveBanForIpAdress(userContext, Request.UserHostAddress))
            {
                lblNotif.Text = GetLocalResourceObject("errIpBan").ToString();
                return;
            }

            if (!string.IsNullOrEmpty(tbSusername.Text))
            {
                BusinessUser businessUser = new BusinessUser();

                User user = businessUser.GetByName(userContext, tbSusername.Text, false, true);
                if (user != null)
                {
                    if (businessUser.IsFromUserTeam(user))
                    {

                        BusinessUserOptions userOptions = new BusinessUserOptions();
                        if (!user.UserOptionsReference.IsLoaded)
                        {
                            user.UserOptionsReference.Load();
                        }

                        if (userOptions.IsUserActivated(user.UserOptions))
                        {
                            tbSusername.Enabled = false;

                            phSecStep1.Visible = false;
                            btnSecretStep2.Enabled = false;
                            btnCancel1.Enabled = false;

                            pnlSecQnAStep2.Visible = true;

                            if (!user.UserOptionsReference.IsLoaded)
                            {
                                user.UserOptionsReference.Load();
                            }

                            lblSecQustion.Text = user.UserOptions.secretQuestion;
                        }
                        else
                        {
                            lblNotif.Text = GetLocalResourceObject("errUserActivated").ToString();
                        }
                    }
                    else
                    {
                        lblNotif.Text = GetLocalResourceObject("errNoSecQuest").ToString();
                    }
                }
                else
                {
                    lblNotif.Text = GetLocalResourceObject("errNoSuchUser").ToString();
                }

            }
            else
            {
                lblNotif.Text = GetLocalResourceObject("errUserName").ToString();
            }
        }

        protected void btnLogIn_Click(object sender, EventArgs e)
        {
            phSecStep2.Visible = true;
            phSecStep2.Controls.Add(lblNotif);


            BusinessIpBans businessIpBans = new BusinessIpBans();
            if (businessIpBans.IsThereActiveBanForIpAdress(userContext, Request.UserHostAddress))
            {
                lblNotif.Text = GetLocalResourceObject("errIpBan").ToString();
                return;
            }

            bool newPass = false;
            string error = string.Empty;

            if (IsRequestValid(userContext, IpAttemptTry.AnswerSecQuestion, out error) == false)
            {
                lblNotif.Text = error;
                return;
            }

            ccSecretQuest.ValidateCaptcha(tbSecCaptcha.Text);
            tbSecCaptcha.Text = "";
            if (ccSecretQuest.UserValidated == true)
            {
                if (!string.IsNullOrEmpty(tbSecAnswer.Text))
                {
                    BusinessUser businessUser = new BusinessUser();

                    User user = businessUser.GetByName(userContext, tbSusername.Text, false, true);
                    if (user != null)
                    {
                        BusinessUserOptions userOptions = new BusinessUserOptions();

                        if (!user.UserOptionsReference.IsLoaded)
                        {
                            user.UserOptionsReference.Load();
                        }

                        if (userOptions.IsUserActivated(user.UserOptions) == false)
                        {
                            throw new CommonCode.UIException(string.Format("Reset password link cannot be sent to user: {0} , ID = {1} , because it isn`t activated!"
                                , user.username, user.ID));
                        }


                        if (businessUser.IsFromUserTeam(user))
                        {
                            if (businessUser.CheckPassword(tbSecAnswer.Text, user.UserOptions.secretAnswer))
                            {
                                newPass = true;

                                BusinessLog bLog = new BusinessLog(user.ID, Request.UserHostAddress);
                                string newPassword = "testpass";
                                newPassword = businessUser.ResetUserPassword(userContext, objectContext, bLog, user);

                                pnlRetrievePass.Visible = false;
                                lblLoggedSucc.Visible = true;
                                lblLoggedSucc.Text = string.Format("{0} '{1}'", GetLocalResourceObject("answCorrect").ToString(), newPassword);

                            }
                            else
                            {
                                lblNotif.Text = GetLocalResourceObject("errIncAnswer").ToString();
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException("User to which secret question should be answered IS not from user team.");
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("Cannot get user to which should secret question should be answered");
                    }
                }
                else
                {
                    lblNotif.Text = GetLocalResourceObject("errTypeAnsw").ToString();
                }
            }
            else
            {
                lblNotif.Text = GetGlobalResourceObject("SiteResources", "errorIncLetters").ToString();
            }

            if (newPass == false)
            {
                CommonCode.IpAttempts.AttemptWrong(userContext, IpAttemptTry.AnswerSecQuestion);
            }

        }

        private void Discard()
        {
            lblLoggedSucc.Visible = false;

            btnProceedSecQnA.Enabled = true;
            btnProceedEmail.Enabled = true;

            pnlByEmail.Visible = false;
            tbMemail.Text = "";
            tbMUsername.Text = "";
            lblNotif.Text = "";

            pnlBySecQnA.Visible = false;
            tbSusername.Text = "";
            tbSusername.Enabled = true;
            btnCancel1.Enabled = true;
            btnSecretStep2.Enabled = true;

            pnlSecQnAStep2.Visible = false;
            tbSecAnswer.Text = "";

            tbMailCaptcha.Text = "";
            tbSecCaptcha.Text = "";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Discard();
        }

        protected void btnCancel1_Click(object sender, EventArgs e)
        {
            Discard();
        }

        protected void btnCancel2_Click(object sender, EventArgs e)
        {
            Discard();
        }


    }
}
