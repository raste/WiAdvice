﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using DataAccess;

using BusinessLayer;

namespace UserInterface
{
    public partial class LogIn : BasePage
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

            lblWelcome.Text = GetLocalResourceObject("title").ToString();

            BusinessSiteText siteText = new BusinessSiteText();

            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutLogIn");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblAbout.Text = aboutExtended.description; 
            }
            else
            {
                lblAbout.Text = "No information entered.";
            }

            if (pnlError.Visible == true)
            {
                pnlError.Visible = false;
            }

            lblusername.Text = GetLocalResourceObject("username").ToString();
            lblpassword.Text = GetLocalResourceObject("password").ToString();
            btnLogIn.Text = GetLocalResourceObject("LogIn").ToString();
        }

        private void CheckUser()
        {
             long currentUserID = CommonCode.CurrentUser.GetCurrentUserId();
             if (currentUserID > 0)
             {
                 CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errNotLogged").ToString());
             }
        }

        private void ShowAdvertisement()
        {
            if (Configuration.AdvertsNumAdvertsOnCategoryPage > 0)
            {
                phAdvert.Controls.Clear();
                adcell.Attributes.Clear();
                aboutCell.Attributes.Clear();

                phAdvert.Controls.Add(CommonCode.ImagesAndAdverts.GetAdvertisements
                    (objectContext, Server, "general", 1, 1));
                if (CommonCode.ImagesAndAdverts.getAdvertisementsNumber(phAdvert) > 0)
                {
                    phAdvert.Visible = true;
                    adcell.Width = "252px";
                    adcell.VAlign = "top";

                    aboutCell.VAlign = "top";
                    aboutCell.Width = "400px";
                }
                else
                {
                    phAdvert.Visible = false;
                    adcell.Width = "0px";

                    aboutCell.Width = "450px";
                    aboutCell.VAlign = "top";
                }
            }
        }

        protected void btnLogIn_Click(object sender, EventArgs e)
        {
            string error = string.Empty;
            if (IsRequestValid(userContext, IpAttemptTry.LogIn, out error) == true)
            {
                pnlError.Visible = false;
                Boolean loggedIn = false;

                if (!string.IsNullOrEmpty(tbUsername.Text) && !string.IsNullOrEmpty(tbPassword.Text))
                {
                    BusinessUser businessUser = new BusinessUser();
                    BusinessIpBans businessIpBans = new BusinessIpBans();

                    long userID = businessUser.Login(objectContext, userContext, tbUsername.Text, tbPassword.Text);

                    if (userID > 0)
                    {
                        User userLoggingIn = businessUser.GetWithoutVisible(userContext, userID, true);

                        if (businessIpBans.IsThereActiveBanForIpAdress(userContext, Request.UserHostAddress))
                        {
                            if (!businessUser.IsFromAdminTeam(userLoggingIn))
                            {
                                lblError.Text = GetLocalResourceObject("errIpAdress").ToString();
                                pnlError.Visible = true;

                                return;
                            }
                        }

                        if (userLoggingIn.visible == false)
                        {
                            lblError.Text = GetLocalResourceObject("errUserDeleted").ToString();
                            pnlError.Visible = true;

                            return;
                        }

                        BusinessUserOptions userOptions = new BusinessUserOptions();
                        if (!userLoggingIn.UserOptionsReference.IsLoaded)
                        {
                            userLoggingIn.UserOptionsReference.Load();
                        }

                        if (Configuration.CheckIfUserIsActivatedOnLogin == true)
                        {
                            if (!userOptions.IsUserActivated(userLoggingIn.UserOptions))
                            {
                                tbUsername.Text = "";

                                lblError.Text = string.Format("{0}{1}", GetLocalResourceObject("errActivate")
                                    , GetLocalResourceObject("errActivate2"));

                                pnlError.Visible = true;

                                return;
                            }
                        }

                        if (Configuration.CheckForAlreadyLoggedUserOnLogIn == true)
                        {
                            if (!businessUser.IsUserLoggedIn(userID))
                            {
                                if (businessUser.IsFromUserTeam(userLoggingIn) == true
                                && userOptions.CheckIfUserHaveToChangeName(userLoggingIn.UserOptions) == true)
                                {
                                    ShowChangeNamePanel(userLoggingIn); // Shows panel to change name and hides the log in panel
                                    return;
                                }

                                businessUser.RemoveGuest(CommonCode.CurrentUser.GetCurrentUserId());

                                Session[CommonCode.CurrentUser.CurrentUserIdKey] = userID;
                                businessUser.AddLoggedUser(userContext, objectContext, userLoggingIn, businessLog);
                                loggedIn = true;

                                BusinessStatistics businessStatistics = new BusinessStatistics();
                                businessStatistics.UserLogged(userContext);
                            }
                            else
                            {
                                Session[CommonCode.CurrentUser.CurrentUserIdKey] = null;

                                lblError.Text = string.Format("{0} {1} {2}", GetLocalResourceObject("errTimeOut")
                                    , Session.Timeout, GetLocalResourceObject("errTimeOut2"));

                                pnlError.Visible = true;
                                return;
                            }
                        }
                        else
                        {
                            if (businessUser.IsFromUserTeam(userLoggingIn) == true
                                && userOptions.CheckIfUserHaveToChangeName(userLoggingIn.UserOptions) == true)
                            {
                                ShowChangeNamePanel(userLoggingIn); // Shows panel to change name and hides the log in panel
                                return;
                            }

                            businessUser.RemoveGuest(CommonCode.CurrentUser.GetCurrentUserId());

                            Session[CommonCode.CurrentUser.CurrentUserIdKey] = userID;
                            businessUser.AddLoggedUser(userContext, objectContext, userLoggingIn, businessLog);
                            loggedIn = true;

                            BusinessStatistics businessStatistics = new BusinessStatistics();
                            businessStatistics.UserLogged(userContext);
                        }


                        if (loggedIn == true)
                        {
                            tbUsername.Text = "";

                            BasePage basePg = Page as BasePage;
                            if ((basePg != null) && (basePg.RedirrectOnLogIn == true))
                            {
                                RedirectToOtherUrl("Home.aspx");
                            }
                            else
                            {
                                RedirectToSameUrl(Request.Url.ToString());
                            }
                        }
                        
                    }
                    else
                    {
                        Session[CommonCode.CurrentUser.CurrentUserIdKey] = null;

                        pnlError.Visible = true;

                        if (businessIpBans.IsThereActiveBanForIpAdress(userContext, Request.UserHostAddress) == true)
                        {
                            lblError.Text = GetLocalResourceObject("errIpAdress").ToString();
                        }
                        else
                        {
                            lblError.Text = GetLocalResourceObject("errUser").ToString();
                        }
                    }
                }
                else
                {
                    pnlError.Visible = true;
                    lblError.Text = GetLocalResourceObject("errTypeNameAndPass").ToString();
                }

                if (loggedIn == false)
                {
                    CommonCode.IpAttempts.AttemptWrong(userContext, IpAttemptTry.LogIn);
                }
            }
            else
            {
                pnlError.Visible = true;
                lblError.Text = error;
            }

            tbUsername.Text = "";
        }

        private void ShowChangeNamePanel(User userLoggingIn)
        {
            if (userLoggingIn == null)
            {
                throw new CommonCode.UIException("userLoggingIn is null");
            }

            pnlLogIn.Visible = false;
            pnlError.Visible = false;

            pnlChangeName.Visible = true;
            lblChangeNameInfo.Text = GetLocalResourceObject("ChangeNameInfo").ToString();

            lblCurrNameT.Text = GetLocalResourceObject("CurrentName").ToString();
            lblCurrName.Text = userLoggingIn.username;

            lblNewName.Text = GetLocalResourceObject("NewName").ToString();
            lblRepName.Text = GetLocalResourceObject("RepeatNewName").ToString();
            btnChangeName.Text = GetLocalResourceObject("Change").ToString();

            Session["changingName"] = userLoggingIn.username;
        }

        protected void btnChangeName_Click(object sender, EventArgs e)
        {
            if (CommonCode.CurrentUser.GetCurrentUserId() > 0)
            {
                RedirectToOtherUrl("Home.aspx");
            }

            object objChangingName = Session["changingName"];
            if (objChangingName == null)
            {
                RedirectToOtherUrl("Home.aspx");
            }

            string name = objChangingName.ToString();
            if (string.IsNullOrEmpty(name))
            {
                RedirectToOtherUrl("Home.aspx");
            }

            BusinessUser bUser = new BusinessUser();

            User userChangingName = bUser.GetByName(userContext, name, false, true);
            if (userChangingName == null)
            {
                RedirectToOtherUrl("Home.aspx");
            }
            if (lblCurrName.Text != userChangingName.username)
            {
                RedirectToOtherUrl("Home.aspx");
            }
            if (bUser.IsFromUserTeam(userChangingName) == false)
            {
                RedirectToOtherUrl("Home.aspx");
            }

            BusinessUserOptions buOptions = new BusinessUserOptions();
            if (!userChangingName.UserOptionsReference.IsLoaded)
            {
                userChangingName.UserOptionsReference.Load();
            }

            if (buOptions.CheckIfUserHaveToChangeName(userChangingName.UserOptions) == false)
            {
                RedirectToOtherUrl("Home.aspx");
            }

            pnlError.Visible = true;
            lblError.Text = string.Empty;
            string error = string.Empty;

            string newName = tbNewName.Text;

            if (CommonCode.Validate.ValidateUserName(objectContext, ref newName, out error) == true)
            {
                if (newName == tbRepName.Text)
                {
                    if (newName != userChangingName.username)
                    {

                        bUser.ChangeUserData(userContext, objectContext, userChangingName, "name", newName, businessLog);
                        buOptions.ChangeIfUserNeedToChangeName(userContext, objectContext, userChangingName, false, userChangingName, businessLog);

                        Session["changingName"] = null;

                        bUser.RemoveGuest(CommonCode.CurrentUser.GetCurrentUserId());

                        Session[CommonCode.CurrentUser.CurrentUserIdKey] = userChangingName.ID;

                        bUser.AddLoggedUser(userContext, objectContext, userChangingName, businessLog);

                        BusinessStatistics businessStatistics = new BusinessStatistics();
                        businessStatistics.UserLogged(userContext);

                        RedirectToOtherUrl("Home.aspx");
                    }
                    else
                    {
                        error = GetLocalResourceObject("errNameIsSameAsOld").ToString();
                    }
                }
                else
                {
                    error = GetLocalResourceObject("errNewNameAndRepName").ToString();
                }
            }

            lblError.Text = error;

        }
    }
}
