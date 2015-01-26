﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using DataAccess;
using BusinessLayer;
using AjaxControlToolkit;

namespace UserInterface
{
    public partial class AddressActivity : BasePage
    {
        private User currentUser = null;
        private string checkIP = string.Empty;
        private long numberLogs = 50;

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
            SetNeedsToBeLogged();
            CheckUser();

            ShowInfo();
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);
        }

        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);
            if (currUser != null)
            {
                if (businessUser.IsFromAdminTeam(currUser))
                {
                    currentUser = currUser;
                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, "This page is only for Administrators.");
                }
            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, "This page is only for Administrators.");
            }

            if (currentUser == null)
            {
                throw new CommonCode.UIException("CurrentUser is null");
            }
        }


        private void ShowInfo()
        {
            object checkIp = Session["sessCheckIp"];
            if (checkIp != null)
            {
                checkIP = checkIp.ToString();
            }

            object numIp = Session["numLogsWithIp"];
            if (numIp != null)
            {
                if (!long.TryParse(numIp.ToString(), out numberLogs))
                {
                    throw new CommonCode.UIException("couldn`t parse Session['numLogsWithIp'] to long.");
                }
            }

            FillActivity();

        }

        private void FillActivity()
        {
            phActivity.Controls.Clear();

            if (!string.IsNullOrEmpty(checkIP))
            {

                List<Log> logs = businessLog.GetLogsWithIpAdress(objectContext, checkIP, numberLogs);

                if (logs.Count > 0)
                {
                    BusinessUser businessUser = new BusinessUser();
                    BusinessIpBans businessIpBans = new BusinessIpBans();

                    IpBan currBan = businessIpBans.Get(userContext, checkIP);
                    string status = "not banned";
                    string expireDate = string.Empty;
                    bool banned = false;
                    if (currBan != null && currBan.active == true)
                    {
                        banned = true;
                        status = "banned";

                        if (currBan.untillDate != null)
                        {
                            expireDate = string.Format(", expire date : {0}", currBan.untillDate.Value.ToShortDateString());
                        }
                    }


                    if (currBan == null)
                    {
                        btnChngDate.Visible = false;

                        pnlBanDescription.Visible = false;
                    }
                    else
                    {
                        btnChngDate.Visible = true;

                        pnlBanDescription.Visible = true;
                        lblBanDescription.Text = Tools.GetFormattedTextFromDB(currBan.notes);
                    }


                    pnlStatus.Visible = true;
                    lblStatus.Text = string.Format("Results for logs from ip adress : {0} , status : {1}{2}"
                        , checkIP, status, expireDate);

                    if (businessUser.IsGlobalAdministrator(currentUser))
                    {
                        pnlBanOrUnban.Visible = true;
                        if (banned == true)
                        {
                            btnBanUnban.Text = "Unban";
                        }
                        else
                        {
                            btnBanUnban.Text = "Ban";
                        }
                    }
                    User guest = businessUser.GetGuest(userContext);
                    User systerm = businessUser.GetSystem(userContext);
                    User logUser = null;

                    int i = 0;
                    foreach (Log log in logs)
                    {
                        Panel newPanel = new Panel();
                        phActivity.Controls.Add(newPanel);
                        newPanel.CssClass = "panelRows";

                        if (i % 2 == 0)
                        {
                            newPanel.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                        }
                        else
                        {
                            newPanel.BackColor = CommonCode.UiTools.GetStandardCellBgrColor();
                        }

                        Panel usrPnl = new Panel();
                        newPanel.Controls.Add(usrPnl);
                        usrPnl.CssClass = "panelInline";
                        usrPnl.Width = Unit.Pixel(250);

                        if (!log.UserIDReference.IsLoaded)
                        {
                            log.UserIDReference.Load();
                        }

                        logUser = Tools.GetUserFromUserDatabase(userContext, log.UserID);

                        if (logUser.ID != guest.ID && logUser.ID != systerm.ID)
                        {
                            usrPnl.Controls.Add(CommonCode.UiTools.GetUserHyperLink(logUser));
                        }
                        else if (logUser.ID == guest.ID)
                        {
                            usrPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(guest.username, false));
                        }
                        else
                        {
                            usrPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(systerm.username, false));
                        }

                        Panel datePnl = new Panel();
                        newPanel.Controls.Add(datePnl);
                        datePnl.CssClass = "panelInline";
                        datePnl.Width = Unit.Pixel(170);

                        Label lblDate = new Label();
                        datePnl.Controls.Add(lblDate);
                        lblDate.Text = CommonCode.UiTools.DateTimeToLocalString(log.dateCreated);
                        lblDate.CssClass = "commentsDate";

                        Label lblType = new Label();
                        newPanel.Controls.Add(lblType);
                        lblType.Text = string.Format("{0}", log.type);
                        lblType.CssClass = "searchPageRatings";

                        Label lblTypeObect = new Label();
                        newPanel.Controls.Add(lblTypeObect);
                        lblTypeObect.Text = string.Format("{0} , {1}", log.typeModifiedSubject, log.IDModifiedSubject);
                        lblTypeObect.CssClass = "marginLeft";

                        Panel descrPnl = new Panel();
                        newPanel.Controls.Add(descrPnl);
                        descrPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(Tools.GetFormattedTextFromDB(log.description), false));

                        i++;
                    }

                }
                else
                {
                    Session["sessCheckIp"] = null;
                    Session["numLogsWithIp"] = null;

                    pnlStatus.Visible = true;
                    lblStatus.Text = "No logs from this IP adress.";

                    pnlBanDescription.Visible = false;
                }
            }

        }

        protected void btnShowActivity_Click(object sender, EventArgs e)
        {
            phShowError.Visible = true;
            phShowError.Controls.Add(lblError);

            string strNum = tbNumber.Text;
            int num = 0;

            if (string.IsNullOrEmpty(strNum) || !int.TryParse(strNum, out num))
            {
                lblError.Text = "Type number of last logs which should be displayed.";
                return;
            }
            else if (num < 1)
            {
                lblError.Text = "Number must be bigger than 0.";
                return;
            }

            string ipAdress = tbIpAdress.Text;
            if (string.IsNullOrEmpty(ipAdress))
            {
                lblError.Text = "Type ip adress.";
                return;
            }

            phShowError.Visible = false;

            checkIP = ipAdress;
            numberLogs = num;

            Session["sessCheckIp"] = ipAdress;
            Session["numLogsWithIp"] = num.ToString();
            FillActivity();
        }


        protected void cExpireDate_SelectionChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cExpireDate.SelectedDate.ToString()))
            {

                string dateStr = Tools.ParseDateTimeToString(cExpireDate.SelectedDate);

                PopupControlExtender.GetProxyForCurrentPopup(Page).Commit(dateStr);
            }
        }

        protected void btnChngDate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(checkIP))
            {
                throw new CommonCode.UIException("checkIP is empty");
            }

            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsGlobalAdministrator(currentUser))
            {
                throw new BusinessException(string.Format("User : {0, ID : {1}, cannot modify bans because he is not global administrator."));
            }

            BusinessIpBans businessIpBans = new BusinessIpBans();
            IpBan currBan = businessIpBans.Get(userContext, checkIP);
            if (currBan == null)
            {
                throw new BusinessException(string.Format("User : {0}, ID : {1} cannot change expire date on ban for IP adress {2} , because there isnt such ban"
                    , currentUser.username, currentUser.ID, checkIP));
            }

            phEditError.Visible = true;
            phEditError.Controls.Add(lblError);

            DateTime untillDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(tbUntillDate.Text))
            {
                if (Tools.ParseStringToDateTime(tbUntillDate.Text, out untillDate))
                {
                    if (DateTime.Compare(untillDate, DateTime.UtcNow) < 1)
                    {
                        lblError.Text = "Date must be in future.";
                        return;
                    }
                }
                else
                {
                    lblError.Text = "Incorrect date format.";
                    return;
                }
            }

            string error;

            string description = tbDescription.Text;

            if (CommonCode.Validate.ValidateDescription(0, 1000, ref description, "description", out error, 90))
            {
                businessIpBans.ChangeBanExpireDate(userContext, objectContext, businessLog, currBan, currentUser, description, untillDate);

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, string.Format("Ip adress {0} expire date updated!", checkIP));

                phEditError.Visible = false;
                tbDescription.Text = string.Empty;
                tbUntillDate.Text = string.Empty;

                FillActivity();
            }
            else
            {
                lblError.Text = error;
            }
        }

        protected void btnBanUnban_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(checkIP))
            {
                throw new CommonCode.UIException("checkIP is empty");
            }

            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsGlobalAdministrator(currentUser))
            {
                throw new BusinessException(string.Format("User : {0, ID : {1}, cannot modify bans because he is not global administrator."));
            }

            phEditError.Visible = true;
            phEditError.Controls.Add(lblError);

            DateTime untillDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(tbUntillDate.Text))
            {

                if (Tools.ParseStringToDateTime(tbUntillDate.Text, out untillDate))
                {
                    if (DateTime.Compare(untillDate, DateTime.UtcNow) < 1)
                    {
                        lblError.Text = "Date must be in future.";
                        return;
                    }
                }
                else
                {
                    lblError.Text = "Incorrect date format.";
                    return;
                }
            }

            string error = "";
            string description = tbDescription.Text;

            if (CommonCode.Validate.ValidateDescription(0, 1000, ref description, "description", out error, Configuration.FieldsDefMaxWordLength))
            {
                BusinessIpBans businessIpBans = new BusinessIpBans();

                if (businessIpBans.IsThereActiveBanForIpAdress(userContext, checkIP))
                {
                    IpBan currBan = businessIpBans.GetActiveBan(userContext, checkIP);
                    if (currBan == null)
                    {
                        throw new BusinessException("currBan is null");
                    }

                    businessIpBans.ChangeActiveStateOfIpBan(userContext, objectContext, businessLog, currBan, currentUser, description, untillDate);

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, string.Format("Ip adress : {0} unbanned!", checkIP));
                }
                else
                {
                    businessIpBans.BanIpAdress(userContext, objectContext, businessLog, checkIP, currentUser, description, untillDate);

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, string.Format("Ip adress : {0} banned!", checkIP));
                }

                phEditError.Visible = false;
                tbDescription.Text = string.Empty;
                tbUntillDate.Text = string.Empty;

                FillActivity();
            }
            else
            {
                lblError.Text = error;
            }
        }

        protected void btnShowBanned_Click(object sender, EventArgs e)
        {
            Session["sessCheckIp"] = null;
            Session["numLogsWithIp"] = null;

            phActivity.Controls.Clear();

            BusinessIpBans businessIpBans = new BusinessIpBans();
            List<IpBan> bans = businessIpBans.GetActiveBans(userContext);

            pnlBanDescription.Visible = false;
            pnlBanOrUnban.Visible = false;

            if (bans.Count > 0)
            {
                pnlStatus.Visible = true;
                lblStatus.Text = "Active bans : ";

                int i = 0;
                foreach (IpBan ban in bans)
                {
                    Panel newPanel = new Panel();
                    phActivity.Controls.Add(newPanel);
                    newPanel.CssClass = "panelRows";

                    if (i % 2 == 0)
                    {
                        newPanel.BackColor = CommonCode.UiTools.GetStandardCellBgrColor();
                    }
                    else
                    {
                        newPanel.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                    }

                    Label lblIpAdress = new Label();
                    newPanel.Controls.Add(lblIpAdress);
                    lblIpAdress.CssClass = "searchPageRatings";
                    lblIpAdress.Text = ban.IPadress;

                    if (!ban.byUserReference.IsLoaded)
                    {
                        ban.byUserReference.Load();
                    }

                    HyperLink byUser = CommonCode.UiTools.GetUserHyperLink(ban.byUser);
                    newPanel.Controls.Add(byUser);
                    byUser.CssClass = "marginLeft";

                    if (ban.untillDate != null)
                    {
                        Label lblDate = new Label();
                        newPanel.Controls.Add(lblDate);
                        lblDate.Text = CommonCode.UiTools.DateTimeToLocalString(ban.untillDate.Value);
                        lblDate.CssClass = "marginLeft commentsDate";
                    }

                    Panel descrPanel = new Panel();
                    newPanel.Controls.Add(descrPanel);

                    descrPanel.Controls.Add(CommonCode.UiTools.GetLabelWithText
                        (Tools.GetFormattedTextFromDB(ban.notes), false));

                    i++;
                }
            }
            else
            {
                pnlStatus.Visible = true;
                lblStatus.Text = "No active bans.";
            }
        }


    }
}
