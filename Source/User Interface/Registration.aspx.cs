﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Web.Services;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class Registration : BasePage
    {
        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            tbUsername.Attributes.Add("onblur", string.Format("JSCheckData('{0}','usernameReg','{1}','');", tbUsername.ClientID, lblCUser.ClientID));

            tbPassword.Attributes.Add("onblur", string.Format("JSCheckData('{0}','passFormat','{1}','');", tbPassword.ClientID, lblCPassword.ClientID));

            tbEmail.Attributes.Add("onblur", string.Format("JSCheckData('{0}','emailFormat','{1}','');", tbEmail.ClientID, lblCEmail.ClientID));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetRedirrectOnLogIn();
            CheckUser();            // Checks user , if hes logged then it doesnt show the reg panel
            ShowInfo();                 
        }

        /// <summary>
        /// returns true if user is logged in automatically after his registration (happens right after registration only)
        /// </summary>
        private Boolean IsNewUserCurrentlyRegistered()
        {
            object newUser = Session["newUserRegistered"];
            bool result = false;
            if (newUser != null)
            {
                bool isNewUser = false;
                if (bool.TryParse(newUser.ToString(), out isNewUser))
                {
                    if (isNewUser == true)
                    {
                        lblReg.Text = GetLocalResourceObject("regSuccessful").ToString();
                        lblReg.Visible = true;
                        regPanel.Visible = false;

                        Session["newUserRegistered"] = null;
                        result = true;
                    }
                }
            }

            return result;
        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            if (regPanel.Visible)
            {
                BusinessSiteText businessText = new BusinessSiteText();
                SiteNews regAbout = businessText.GetSiteText(objectContext, "registration");
                if (regAbout != null && regAbout.visible)
                {
                    lblRegAbout.Text = regAbout.description;
                }
                else
                {
                    lblRegAbout.Visible = false;
                }

                lblUsernameRules.Text = string.Format
                    ("{0}<br />{1} {2}-{3} {4}.", GetLocalResourceObject("usernameRules"), GetLocalResourceObject("usernameRules2")
                    , Configuration.UsersMinUserNameLength, Configuration.UsersMaxUserNameLength, GetLocalResourceObject("characters"));

                lblPassRules.Text = string.Format
                    ("{0}<br />{1} {2}-{3} {4}.", GetLocalResourceObject("passwordRules"), GetLocalResourceObject("passwordRules2")
                    , Configuration.UsersMinPasswordLength, Configuration.UsersMaxPasswordLength, GetLocalResourceObject("characters"));

                lblMailRules.Text = GetLocalResourceObject("emailRules").ToString();

                lblSecretRules.Text = string.Format("{0}<br />{1}", GetLocalResourceObject("qnaRules")
                    , GetLocalResourceObject("qnaRules2"));
            }

            lblCUser.Text = "";
            lblCPassword.Text = "";
            lblCEmail.Text = "";

            SetLocalText();
        }

        private void SetLocalText()
        {
            lblRegForm.Text = GetLocalResourceObject("regForm").ToString();

            lblUsername.Text = GetLocalResourceObject("username").ToString();
            lblPassword.Text = GetLocalResourceObject("password").ToString();
            lblRPassword.Text = GetLocalResourceObject("repPassword").ToString();
            lblEmail.Text = GetLocalResourceObject("email").ToString();
            lblSecQuest.Text = GetLocalResourceObject("secQuestion").ToString();
            lblQuestInfo.Text = GetLocalResourceObject("questionInfo").ToString();
            lblSecAnswer.Text = GetLocalResourceObject("answer").ToString();
            lblCaptchaInfo.Text = GetLocalResourceObject("captchaInfo").ToString();
            lblCaptchaInfo2.Text = GetLocalResourceObject("captchaInfo2").ToString();
            lblIAgree.Text = GetLocalResourceObject("AgreeTerms").ToString();
            lblIAgree2.Text = GetLocalResourceObject("AgreeTerms2").ToString();
            hlTerms.Text = GetLocalResourceObject("terms").ToString();
            btnSubmit.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();

            tbPassword_PasswordStrength.PrefixText = string.Format("{0} ", GetLocalResourceObject("Strength").ToString());
            tbPassword_PasswordStrength.TextStrengthDescriptions = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}"
                , GetLocalResourceObject("PassStrengthLvl1"), GetLocalResourceObject("PassStrengthLvl2"),
                GetLocalResourceObject("PassStrengthLvl3"), GetLocalResourceObject("PassStrengthLvl4"),
                GetLocalResourceObject("PassStrengthLvl5"), GetLocalResourceObject("PassStrengthLvl6"),
                GetLocalResourceObject("PassStrengthLvl7"), GetLocalResourceObject("PassStrengthLvl8"),
                GetLocalResourceObject("PassStrengthLvl9"), GetLocalResourceObject("PassStrengthLvl10"));

            hlTerms.NavigateUrl = GetUrlWithVariant("Rules.aspx");
        }

        private void CheckUser()
        {
            long currentUserID = CommonCode.CurrentUser.GetCurrentUserId();
            if (currentUserID > 0)
            {
                if (IsNewUserCurrentlyRegistered() == false)
                {
                    regPanel.Visible = false;
                    lblReg.Text = GetLocalResourceObject("errLoggedIn").ToString();
                    lblReg.Visible = true;
                }
            }
            else
            {
                regPanel.Visible = true;
            }
        }

        
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (CommonCode.CurrentUser.GetCurrentUserId() > 0)
            {
                throw new CommonCode.UIException(string.Format("User ID = {0} tried to register new user"));
            }

            lblError.Visible = true;

            BusinessIpBans businessIpBans = new BusinessIpBans();
            if (businessIpBans.IsThereActiveBanForIpAdress(userContext, Request.UserHostAddress))
            {
                lblError.Text = GetLocalResourceObject("errIpBan").ToString();
                return;
            }

            if (businessIpBans.CounRegisteredUsersFromIpAdress(userContext, Request.UserHostAddress) 
                > Configuration.RegisterMaxNumberRegistrationsFromIp)
            {
                lblError.Text = GetLocalResourceObject("errRegsReached").ToString();
                return;
            }

            string error = "";

            ccReg.ValidateCaptcha(tbCaptcha.Text);
            tbCaptcha.Text = "";
            if (ccReg.UserValidated == false)
            {
                lblError.Text = GetGlobalResourceObject("SiteResources", "errorIncLetters").ToString();
                return;
            }

            if (cbAgreeTerms.Checked == false)
            {
                lblError.Text = GetLocalResourceObject("errTerms").ToString();
                return;
            }

            string username = tbUsername.Text;

            if (CommonCode.Validate.ValidateUserName(objectContext, ref username, out error))
            {
                if (CommonCode.Validate.ValidatePassword(tbPassword.Text, out error))
                {
                    if (CommonCode.Validate.ValidateRepeatPassword(tbUsername.Text, tbPassword.Text, tbRPassword.Text, out error))
                    {
                        if (CommonCode.Validate.ValidateSecretQnA(1, 100, tbSecQuestion.Text))
                        {
                            if (CommonCode.Validate.ValidateSecretQnA(1, 100, tbSecAnswer.Text))
                            {
                                if (tbEmail.Text.Length > 0)
                                {
                                    if (CommonCode.Validate.ValidateEmailAdress(tbEmail.Text, out error))
                                    {
                                        int maxNumUsersPerMail = Configuration.MaximumNumberOfUsersRegisteredWithMail;

                                        BusinessUser businessUser = new BusinessUser();
                                        if (businessUser.CountRegisteredUsersWithMail(userContext, tbEmail.Text) < maxNumUsersPerMail)
                                        {
                                            lblError.Visible = false;
                                            RegisterUser( username, tbPassword.Text, tbEmail.Text, tbSecQuestion.Text, tbSecAnswer.Text);
                                        }
                                        else
                                        {
                                            error = string.Format("{0} {1} {2}", GetLocalResourceObject("errEmailRegsReached")
                                                , maxNumUsersPerMail, GetLocalResourceObject("errEmailRegsReached2"));
                                        }
                                    }
                                }
                                else
                                {
                                    lblError.Visible = false;
                                    RegisterUser(tbUsername.Text, tbPassword.Text, tbEmail.Text, tbSecQuestion.Text, tbSecAnswer.Text);
                                }

                            }
                            else
                            {
                                error = GetLocalResourceObject("errIncAnswer").ToString();
                            }
                        }
                        else
                        {
                            error = GetLocalResourceObject("errIncQuest").ToString();
                        }
       
                    }
                }
            }

            lblError.Text = error;

        }

        private void RegisterUser(String name, String password, String email, String secQuest, String secAnsw)
        {
            BusinessUser businessUser = new BusinessUser();
            businessUser.RegisterUser(userContext, objectContext, name, password, email, businessLog, secQuest, secAnsw, false, null);

            if (Configuration.SendActivationCodeOnRegistering == false)
            {
                LogInAfterRegistering(businessUser);
            }
            else
            {
                User newUser = businessUser.GetUserByNameAndPassword(userContext, tbUsername.Text, tbPassword.Text);
                if (newUser == null)
                {
                    throw new CommonCode.UIException(string.Format("Couldn`t gen user automatically after registering, username = {0}", tbUsername.Text));
                }

                if (!newUser.UserOptionsReference.IsLoaded)
                {
                    newUser.UserOptionsReference.Load();
                }

                if (newUser.UserOptions.activated == false)
                {
                    regPanel.Visible = false;

                    lblReg.Visible = true;
                    lblReg.Text = string.Format("{0}<br />{1}<br />{2}<br />", GetLocalResourceObject("accRegistered")
                        , GetLocalResourceObject("accRegistered2"), GetLocalResourceObject("accRegistered3"));
                }
                else
                {
                    LogInAfterRegistering(businessUser);
                }

               
            }
        }

        private void LogInAfterRegistering(BusinessUser businessUser)
        {
            //////////////////////////////// automatic log in after registration///////////////////
            long userID = businessUser.Login(objectContext, userContext, tbUsername.Text, tbPassword.Text);
            if (userID > 0)
            {
                Session[CommonCode.CurrentUser.CurrentUserIdKey] = userID;

                User currUser = businessUser.Get(userContext, userID, true);

                businessUser.RemoveGuest(userID);
                businessUser.AddLoggedUser(userContext, objectContext, currUser, businessLog);

                BusinessStatistics businessStatistics = new BusinessStatistics();
                businessStatistics.UserLogged(userContext);

                Session.Add("newUserRegistered", true);

                RedirectToSameUrl(Request.Url.ToString());
            }
            else
            {
                throw new CommonCode.UIException(string.Format("Couldn`t log in automatically after registering, username = {0}", tbUsername.Text));
            }
        }
       

        [WebMethod]
        public static string CheckData(string text, string type, string notUsed)
        {
            string error = "";

            CommonCode.WebMethods.ValidateUserInput(text, type, "", out error);

            return error; 
        }

    }
}
