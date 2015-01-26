﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.Services;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class Warnings : BasePage
    {
        private User currentUser = null;

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

        private void ShowInfo()
        {
            Title = "User warnings";

            BusinessSiteText businessText = new BusinessSiteText();
            SiteNews warningAbout = businessText.GetSiteText(objectContext, "aboutWarnings");
            if (warningAbout != null && warningAbout.visible)
            {
                lblInfo.Text = warningAbout.description;
            }
            else
            {
                lblInfo.Text = "aboutWarnings text not typed.";
            }

            lblWarningsToRemoveRole.Text = string.Format("Number of warnings on user role to automatically remove it : {0}"
                , Configuration.WarningsNumberOnActionsOnWhichShouldRemoveAction);
            lblWarningsToDelUser.Text = string.Format("Number or total warnings on user to automatically remove him : {0}"
               , Configuration.WarningsOnHowManyToDeleteUser);

            if (IsPostBack == false)
            {
                BusinessUser bUser = new BusinessUser();
                bool isModer = bUser.IsModerator(currentUser);
                if (isModer == true)
                {
                    ddlTypeRoles.Enabled = false;
                    tbTypeID.Enabled = false;
                }
                else
                {
                    ddlTypeRoles.Enabled = true;
                    tbTypeID.Enabled = true;
                }

                FillDdlUserRoles(isModer);
            }

            FillUserWarnings();
            FillWarningPatterns();

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

        private void FillDdlUserRoles(bool isModerator)
        {
            ddlUserRoles.Items.Clear();

            ListItem chooseItem = new ListItem();
            ddlUserRoles.Items.Add(chooseItem);
            chooseItem.Text = "... choose";
            chooseItem.Value = "0";

            ListItem generalItem = new ListItem();
            ddlUserRoles.Items.Add(generalItem);
            generalItem.Text = "General";
            generalItem.Value = "general";

            ListItem commMsgItem = new ListItem();
            ddlUserRoles.Items.Add(commMsgItem);
            commMsgItem.Text = "Write comments and messages";
            commMsgItem.Value = "commenter";

            ListItem suggItem = new ListItem();
            ddlUserRoles.Items.Add(suggItem);
            suggItem.Text = "Write suggestions";
            suggItem.Value = "suggestor";

            ListItem repItem = new ListItem();
            ddlUserRoles.Items.Add(repItem);
            repItem.Text = "Report";
            repItem.Value = "flagger";


            if (isModerator == false)
            {

                ListItem aProdItem = new ListItem();
                ddlUserRoles.Items.Add(aProdItem);
                aProdItem.Text = "Add products";
                aProdItem.Value = "product";

                ListItem aCompItem = new ListItem();
                ddlUserRoles.Items.Add(aCompItem);
                aCompItem.Text = "Add companies";
                aCompItem.Value = "company";

            }

            ListItem rProdItem = new ListItem();
            ddlUserRoles.Items.Add(rProdItem);
            rProdItem.Text = "Rate products";
            rProdItem.Value = "prater";

            ListItem rCommItem = new ListItem();
            ddlUserRoles.Items.Add(rCommItem);
            rCommItem.Text = "Rate comments";
            rCommItem.Value = "commrater";
  
        }

        protected void btnViewUserWarnings_Click(object sender, EventArgs e)
        {
            phErrorUserWarnings.Visible = true;
            phErrorUserWarnings.Controls.Add(lblError);

            string username = tbUserWarnings.Text;
            if (string.IsNullOrEmpty(username))
            {
                lblError.Text = "Type username.";
                return;
            }

            BusinessUser businessUser = new BusinessUser();
            User user = businessUser.GetWithoutVisible(userContext, username, false);
            if (user == null)
            {
                lblError.Text = "No such user.";
                return;
            }
            if (!businessUser.IsFromUserTeam(user))
            {
                lblError.Text = string.Format("{0} is not from user team.", user.username);
                return;
            }

            phErrorUserWarnings.Visible = false;

            Session.Add("warningsFor", username);
            FillUserWarnings();

        }

        private void FillWarningPatterns()
        {
            tblPatterns.Rows.Clear();

            BusinessSiteText bSiteText = new BusinessSiteText();

            List<SiteNews> patterns = bSiteText.GetLastTexts(objectContext, int.MaxValue, "warning patterns", 1);

            if (patterns.Count > 0)
            {
                pnlShowPatterns.Visible = true;

                TableRow newRow = new TableRow();
                tblPatterns.Rows.Add(newRow);

                int i = 0;

                foreach (SiteNews pattern in patterns)
                {

                    if (i % 3 == 0)
                    {
                        newRow = new TableRow();
                        tblPatterns.Rows.Add(newRow);
                    }

                    TableCell patternCell = new TableCell();
                    newRow.Cells.Add(patternCell);
                    patternCell.CssClass = "editCell";

                    if (i % 2 == 1)
                    {
                        patternCell.BackColor = CommonCode.UiTools.GetStandardCellBgrColor();
                    }

                    Label newLabel = new Label();
                    patternCell.Controls.Add(newLabel);
                    newLabel.Text = pattern.name;
                    newLabel.ID = string.Format("patt{0}", pattern.ID);
                    newLabel.CssClass = "pointerCursor";
                    newLabel.ForeColor = System.Drawing.Color.Blue;
                    newLabel.Attributes.Add("onclick", string.Format("SetText('{0}','{1}','{2}')"
                        , pattern.ID, "warning pattern", tbWarningDescription.ClientID));

                    i++;
                }
            }
            else
            {
                pnlShowPatterns.Visible = false;
            }
        }

        private void FillUserWarnings()
        {
            phUserWarnings.Controls.Clear();
            lblStatus.Visible = false;

            object objUserName = Session["warningsFor"];
            if (objUserName == null)
            {
                return;
            }

            string username = objUserName.ToString();
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            BusinessUser businessUser = new BusinessUser();
            User user = businessUser.GetWithoutVisible(userContext, username, true);

            BusinessWarnings businessWarnings = new BusinessWarnings();
            List<Warning> userWarnings = businessWarnings.GetUserWarnings(userContext, user);
            List<TypeWarning> typeWarnings = businessWarnings.GetUserTypeWarnings(objectContext, user);

            lblStatus.Visible = true;

            if (userWarnings.Count > 0 || typeWarnings.Count > 0)
            {
                lblStatus.Text = string.Format("Warnings for ' {0} '.", username);

                int i = 0;

                if (userWarnings.Count > 0)
                {
                    Panel namePanel = new Panel();
                    phUserWarnings.Controls.Add(namePanel);
                    namePanel.HorizontalAlign = HorizontalAlign.Center;
                    namePanel.CssClass = "sectionTextHeader";
                    Label nameLbl = new Label();
                    namePanel.Controls.Add(nameLbl);
                    nameLbl.Text = "Main roles warnings.";

                    foreach (Warning warning in userWarnings)
                    {
                        Panel newPanel = new Panel();
                        phUserWarnings.Controls.Add(newPanel);
                        newPanel.CssClass = "panelRows";

                        if (i % 2 == 0)
                        {
                            newPanel.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                        }
                        else
                        {
                            newPanel.BackColor = CommonCode.UiTools.GetStandardCellBgrColor();
                        }

                        Label lblDate = new Label();
                        newPanel.Controls.Add(lblDate);
                        lblDate.CssClass = "commentsDate";
                        lblDate.Text = CommonCode.UiTools.DateTimeToLocalString(warning.dateCreated);

                        if(!warning.ByAdminReference.IsLoaded)
                        {
                            warning.ByAdminReference.Load();
                        }

                        HyperLink byLink = CommonCode.UiTools.GetUserHyperLink(warning.ByAdmin);
                        newPanel.Controls.Add(byLink);
                        byLink.CssClass = "marginLeft";

                        if (!warning.UserActionReference.IsLoaded)
                        {
                            warning.UserActionReference.Load();
                        }

                        if (warning.UserAction != null)
                        {
                            if (!warning.UserAction.ActionReference.IsLoaded)
                            {
                                warning.UserAction.ActionReference.Load();
                            }

                            Label roleLbl = new Label();
                            newPanel.Controls.Add(roleLbl);
                            roleLbl.CssClass = "searchPageRatings marginLeft";
                            roleLbl.Text = businessUser.GetUserRoleFromString(warning.UserAction.Action.name).ToString();
                        }
                        else
                        {
                            Label roleLbl = new Label();
                            newPanel.Controls.Add(roleLbl);
                            roleLbl.CssClass = "searchPageRatings marginLeft";
                            roleLbl.Text = "general";
                        }

                        Panel pnlDescription = new Panel();
                        newPanel.Controls.Add(pnlDescription);
                        pnlDescription.Controls.Add
                            (CommonCode.UiTools.GetLabelWithText(Tools.GetFormattedTextFromDB(warning.description), false));

                        i++;
                    }
                }

                if (typeWarnings.Count > 0)
                {
                    Panel namePanel = new Panel();
                    phUserWarnings.Controls.Add(namePanel);
                    namePanel.HorizontalAlign = HorizontalAlign.Center;
                    namePanel.CssClass = "sectionTextHeader";
                    Label nameLbl = new Label();
                    namePanel.Controls.Add(nameLbl);
                    nameLbl.Text = "Type roles warnings.";

                    BusinessProduct businessProduct = new BusinessProduct();
                    BusinessCompany businessCompany = new BusinessCompany();

                    foreach (TypeWarning warning in typeWarnings)
                    {
                        Panel newPanel = new Panel();
                        phUserWarnings.Controls.Add(newPanel);
                        newPanel.CssClass = "panelRows";

                        if (i % 2 == 0)
                        {
                            newPanel.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                        }
                        else
                        {
                            newPanel.BackColor = CommonCode.UiTools.GetStandardCellBgrColor();
                        }

                        Label lblDate = new Label();
                        newPanel.Controls.Add(lblDate);
                        lblDate.CssClass = "commentsDate";
                        lblDate.Text = CommonCode.UiTools.DateTimeToLocalString(warning.dateCreated);

                        if (!warning.ByAdminReference.IsLoaded)
                        {
                            warning.ByAdminReference.Load();
                        }

                        HyperLink byLink = CommonCode.UiTools.GetUserHyperLink
                            (businessUser.GetWithoutVisible(userContext, warning.ByAdmin.ID, true));
                        newPanel.Controls.Add(byLink);
                        byLink.CssClass = "marginLeft";

                        if (!warning.UserTypeActionReference.IsLoaded)
                        {
                            warning.UserTypeActionReference.Load();
                        }
                        if (!warning.UserTypeAction.TypeActionReference.IsLoaded)
                        {
                            warning.UserTypeAction.TypeActionReference.Load();
                        }

                        Label lblType = new Label();
                        newPanel.Controls.Add(lblType);
                        lblType.CssClass = "marginLeft";

                        string type = warning.UserTypeAction.TypeAction.type;
                        long typeID = warning.UserTypeAction.TypeAction.typeID;

                        // ADD POPUP PANEL ATTRIBUTES

                        switch (type)
                        {
                            case "product":

                                lblType.Text = "product : ";

                                Product currProduct = businessProduct.GetProductByIDWV(objectContext, typeID);
                                if (currProduct == null)
                                {
                                    throw new CommonCode.UIException(string.Format(
                                        "There is no product with ID : {0} linked in type action id : {1}"
                                        , typeID, warning.UserTypeAction.TypeAction.ID));
                                }

                                HyperLink prodLink = CommonCode.UiTools.GetProductHyperLink(currProduct);
                                newPanel.Controls.Add(prodLink);

                                prodLink.ID = string.Format("IDtype{0}", warning.ID);
                                prodLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}','{2}')"
                                    , currProduct.ID, prodLink.ClientID, pnlPopUp.ClientID));
                                prodLink.Attributes.Add("onmouseout", "HideData()");

                                break;
                            case "company":

                                lblType.Text = "company : ";

                                Company currCompany = businessCompany.GetCompanyWV(objectContext, typeID);
                                if (currCompany == null)
                                {
                                    throw new CommonCode.UIException(string.Format(
                                        "There is no company with ID : {0} linked in type action id : {1}"
                                        , typeID, warning.UserTypeAction.TypeAction.ID));
                                }

                                HyperLink compLink = CommonCode.UiTools.GetCompanyHyperLink(currCompany);
                                newPanel.Controls.Add(compLink);

                                compLink.ID = string.Format("IDtype{0}", warning.ID);
                                compLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}','{2}')"
                                    , currCompany.ID, compLink.ClientID, pnlPopUp.ClientID));
                                compLink.Attributes.Add("onmouseout", "HideData()");

                                break;
                            case "aCompProdModificator":

                                lblType.Text = " all company products : ";

                                Company currCompanyProds = businessCompany.GetCompanyWV(objectContext, typeID);
                                if (currCompanyProds == null)
                                {
                                    throw new CommonCode.UIException(string.Format(
                                        "There is no company with ID : {0} linked in type action id : {1}"
                                        , typeID, warning.UserTypeAction.TypeAction.ID));
                                }

                                HyperLink compProdsLink = CommonCode.UiTools.GetCompanyHyperLink(currCompanyProds);
                                newPanel.Controls.Add(compProdsLink);

                                compProdsLink.ID = string.Format("IDtype{0}", warning.ID);
                                compProdsLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}','{2}')"
                                    , currCompanyProds.ID, compProdsLink.ClientID, pnlPopUp.ClientID));
                                compProdsLink.Attributes.Add("onmouseout", "HideData()");
                                break;
                            default:
                                throw new CommonCode.UIException(string.Format
                                    ("action type = ' {0} ' is not supported", type));
                        }


                        Panel pnlDescription = new Panel();
                        newPanel.Controls.Add(pnlDescription);
                        pnlDescription.Controls.Add
                            (CommonCode.UiTools.GetLabelWithText(Tools.GetFormattedTextFromDB(warning.description), false));

                        i++;
                    }
                }

            }
            else
            {
                Session["warningsFor"] = null;

                lblStatus.Text = string.Format("User ' {0} ' don`t have warnings.", username);
            }

        }


        protected void btnAddUserWarning_Click(object sender, EventArgs e)
        {
            phErrorAddWarning.Visible = true;
            phErrorAddWarning.Controls.Add(lblError);

            string username = tbAddUserWarning.Text;
            if (string.IsNullOrEmpty(username))
            {
                lblError.Text = "Type username.";
                return;
            }

            string description = tbWarningDescription.Text;
            if (string.IsNullOrEmpty(description))
            {
                lblError.Text = "Type warning description.";
                return;
            }

            BusinessUser businessUser = new BusinessUser();
            User user = businessUser.GetWithoutVisible(userContext, username, false);
            if (user == null)
            {
                lblError.Text = "No such user.";
                return;
            }

            BusinessWarnings businessWarning = new BusinessWarnings();

            if (ddlUserRoles.SelectedIndex > 0 && ddlTypeRoles.SelectedIndex > 0)
            {
                lblError.Text = "Choose only from one drop down list.";
                ddlUserRoles.SelectedIndex = 0;
                ddlTypeRoles.SelectedIndex = 0;
                tbTypeID.Text = string.Empty;
                return;
            }
            else if (ddlUserRoles.SelectedIndex > 0 && !string.IsNullOrEmpty(tbTypeID.Text))
            {
                lblError.Text = "If the warning is for user role...the ID field need to be empty";
                ddlUserRoles.SelectedIndex = 0;
                tbTypeID.Text = string.Empty;
                return;
            }


            string error = string.Empty;
            if (ddlUserRoles.SelectedIndex > 0)
            {
                // USER ROLE WARNING
                if (businessWarning.CheckWarning(userContext, objectContext, ddlUserRoles.SelectedValue, description, user, currentUser, out error))
                {
                    UserAction userAction = null;

                    if (ddlUserRoles.SelectedValue != "general")
                    {
                        UserRoles roleChosen = businessUser.GetUserRoleFromString(ddlUserRoles.SelectedValue);

                        if (roleChosen == UserRoles.HaveSignature)
                        {
                            throw new CommonCode.UIException(string.Format("Admin ID : {0} cannot send warning for signature role to user, because that role is not used."
                                , currentUser.ID));
                        }

                        BusinessUserActions businessUserAction = new BusinessUserActions();
                        userAction = businessUserAction.GetUserAction(userContext, roleChosen, user);
                        if (userAction == null)
                        {
                            throw new CommonCode.UIException(string.Format("User id : {0}, doesn`t have role : {1}", user.ID, ddlUserRoles.SelectedValue));
                        }
                    }
                    
                    businessWarning.AddWarning(userContext, objectContext, userAction, ddlUserRoles.SelectedValue, description, user, currentUser, businessLog);
                    
                    tbAddUserWarning.Text = string.Empty;
                    tbWarningDescription.Text = string.Empty;
                    ddlUserRoles.SelectedIndex = 0;
                    ddlTypeRoles.SelectedIndex = 0;
                    tbTypeID.Text = string.Empty;

                    if (Session["warningsFor"] != null && Session["warningsFor"].ToString() == username)
                    {
                        FillUserWarnings();
                    }

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, string.Format("Warning to {0} sent.", username));
                }
                else
                {
                    lblError.Text = error;
                    return;
                }

            }
            else if (ddlTypeRoles.SelectedIndex > 0)
            {
                // USER TYPE ROLE WARNING

                string strTypeId = tbTypeID.Text;
                if (string.IsNullOrEmpty(strTypeId))
                {
                    lblError.Text = String.Format("Type the ID of {0}.", ddlTypeRoles.SelectedValue);
                    return;
                }

                long typeID = 0;
                if (!long.TryParse(strTypeId, out typeID))
                {
                    lblError.Text = "Type id number.";
                    return;
                }

                if (businessWarning.CheckTypeWarning(userContext, objectContext, ddlTypeRoles.SelectedValue, typeID, description, user, currentUser, out error))
                {
                    BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();
                    UsersTypeAction typeAction = businessUserTypeActions.GetUserTypeAction(objectContext, ddlTypeRoles.SelectedValue, typeID, user);
                    if (typeAction == null)
                    {
                        throw new CommonCode.UIException(string.Format("User id : {0}, doesn`t have role for type : {1} , ID : {2}"
                            , user.ID, ddlTypeRoles.SelectedValue, typeID));
                    }

                    businessWarning.AddTypeWarning(userContext, objectContext, typeAction, description, user, currentUser, businessLog);

                    tbAddUserWarning.Text = string.Empty;
                    tbWarningDescription.Text = string.Empty;
                    ddlUserRoles.SelectedIndex = 0;
                    ddlTypeRoles.SelectedIndex = 0;
                    tbTypeID.Text = string.Empty;

                    if (Session["warningsFor"] != null && Session["warningsFor"].ToString() == username)
                    {
                        FillUserWarnings();
                    }

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, string.Format("Warning for {0} created.", username));
                }
                else
                {
                    lblError.Text = error;
                    return;
                }

            }
            else
            {
                lblError.Text = "Choose the warning for which user role or type role is about.";
                return;
            }

            phErrorAddWarning.Visible = false;
        }

       

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }

        [WebMethod]
        public static string WMGetSiteText(string Id, string textType)
        {
            return CommonCode.WebMethods.GetSiteText(Id, textType); 
        }


    }
}
