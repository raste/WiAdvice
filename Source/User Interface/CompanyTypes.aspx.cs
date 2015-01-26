﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class CompanyTypes : BasePage 
    {
        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private User currentUser = null;        // used in all actions done by the user

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            tbName.Attributes.Add("onkeyup", string.Format("JSCheckData('{0}','companyType','{1}','');", tbName.ClientID, lblCheckName.ClientID));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetNeedsToBeLogged();
            CheckUser();
            if (currentUser == null)
            {
                throw new CommonCode.UIException("currentUser is null");
            }
            ShowInfo();
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);
        }

        private void ShowInfo()
        {
            Title = "Company types";
            lblCheckName.Text = "";

            CheckAddEditCompanyTypeForm();                  // Checks if Company type should be edit at the moment
            FillTblCompanyTypes();

            if (IsPostBack == false)
            {
                FillDdlTypes();

                lblEditType.Text = string.Format("Add {0} type", Configuration.CompanyName);
            }

            BusinessSiteText siteText = new BusinessSiteText();
            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutCompanyTypes");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblInfo.Text = aboutExtended.description;
            }
            else
            {
                lblInfo.Text = "About Company types text not typed.";
            }

            lblShowCretors.Text = string.Format("Show {0}s with type : ", Configuration.CompanyName);
        }

        private void FillDdlTypes()
        {
            ddlTypes.Items.Clear();

            BusinessCompanyType businessCompanyType = new BusinessCompanyType();
            List<CompanyType> types = businessCompanyType.GetAllCompanyTypes(objectContext, false, true);

            if (types.Count < 1)
            {
                throw new CommonCode.UIException("There are no company types ! There should be atleast one.");
            }

            foreach (CompanyType type in types)
            {
                ListItem newItem = new ListItem();
                newItem.Text = type.name;
                newItem.Value = type.ID.ToString();
                ddlTypes.Items.Add(newItem);
            }

        }

        /// <summary>
        /// Checks if current user is global admin
        /// </summary>
        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);
            if (currUser == null || !businessUser.IsGlobalAdministrator(currUser))
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, "This Page is only for administrators.");
            }
            currentUser = currUser;
        }

        [WebMethod]
        public static string CheckData(string text, string type, string notUsed)
        {
            string error = "";

            CommonCode.WebMethods.ValidateUserInput(text, type, "", out error);

            return error;
        }

        private void FillTblCompanyTypes()
        {
            tblCompanyTypes.Rows.Clear();

            BusinessCompanyType businessCompanyType = new BusinessCompanyType();
            List<CompanyType> types = businessCompanyType.GetAllCompanyTypes(objectContext, false, false);

            if (types.Count > 0)
            {
                CompanyType other = businessCompanyType.GetOtherCompanyType(objectContext);
                CompanyType company = businessCompanyType.GetCompanyType(objectContext, userContext);

                foreach (CompanyType type in types)
                {
                    if (!type.CreatedByReference.IsLoaded)
                    {
                        type.CreatedByReference.Load();
                    }
                    if (!type.LstModifiedByReference.IsLoaded)
                    {
                        type.LstModifiedByReference.Load();
                    }

                    TableRow tblRow = new TableRow();
                    tblCompanyTypes.Rows.Add(tblRow);

                    TableCell tblCell = new TableCell();
                    tblRow.Cells.Add(tblCell);

                    Table newTable = new Table();
                    newTable.Width = Unit.Percentage(100);
                    newTable.GridLines = GridLines.Both;
                    newTable.CssClass = "commentsTD";
                    tblCell.Controls.Add(newTable);

                    //

                    TableRow newRow = new TableRow();
                    newTable.Rows.Add(newRow);

                    TableCell leftCell = new TableCell();
                    leftCell.Width = Unit.Pixel(300);
                    newRow.Cells.Add(leftCell);

                    leftCell.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("ID : {0}", type.ID.ToString()), true));

                    long numCompanies = businessCompanyType.CountAllCompaniesWhichAreFromType(objectContext, type);
                    leftCell.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("{1}s from this type : {0}", numCompanies, Configuration.CompanyName), true));

                    leftCell.Controls.Add(CommonCode.UiTools.GetLabelWithText("Written by : ", false));
                    leftCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink(Tools.GetUserFromUserDatabase(userContext, type.CreatedBy)));
                    leftCell.Controls.Add(CommonCode.UiTools.GetLabelWithText("", true));

                    Label lblDate = CommonCode.UiTools.GetLabelWithText(string.Format("Date written : {0}"
                        , CommonCode.UiTools.DateTimeToLocalString(type.dateCreated)), true);
                    leftCell.Controls.Add(lblDate);
                    lblDate.CssClass = "commentsDate";

                    leftCell.Controls.Add(CommonCode.UiTools.GetLabelWithText("Last modified by : ", false));
                    leftCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink(Tools.GetUserFromUserDatabase(userContext, type.LstModifiedBy)));
                    leftCell.Controls.Add(CommonCode.UiTools.GetLabelWithText("", true));

                    Label lblLastMod = CommonCode.UiTools.GetLabelWithText(string.Format("Last modified on : {0}",
                        CommonCode.UiTools.DateTimeToLocalString(type.lastModified)), true);
                    lblLastMod.CssClass = "commentsDate";
                    leftCell.Controls.Add(lblLastMod);

                    Label lblVisible = CommonCode.UiTools.GetLabelWithText(string.Format("Visible : {0}", type.visible.ToString()), true);
                    leftCell.Controls.Add(lblVisible);
                    lblVisible.CssClass = "searchPageRatings";

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);
                    nameCell.Height = Unit.Pixel(22);
                    nameCell.VerticalAlign = VerticalAlign.Top;

                    System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    nameCell.Controls.Add(div);
                    div.Attributes.Add("class", "searchPageRatings");
                    div.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText(type.name, false));
                    nameCell.Controls.Add(CommonCode.UiTools.GetHorisontalLineControl());
                    nameCell.Controls.Add(CommonCode.UiTools.GetLabelWithText(Tools.GetFormattedTextFromDB(type.description), false));

                    if (type != other && type != company)
                    {
                        TableRow btnsRow = new TableRow();
                        newTable.Rows.Add(btnsRow);

                        btnsRow.Attributes.Add("id", type.ID.ToString());

                        TableCell btnsCell = new TableCell();
                        btnsCell.ColumnSpan = 2;
                        btnsRow.Cells.Add(btnsCell);

                        Button visibleBtn = new Button();
                        btnsCell.Controls.Add(visibleBtn);
                        visibleBtn.CssClass = "marginsLR";
                        visibleBtn.ID = string.Format("ChangeVisible{0}", type.ID);
                        if (type.visible == true)
                        {
                            visibleBtn.Text = "Delete";
                        }
                        else
                        {
                            visibleBtn.Text = "UnDelete";
                        }
                        visibleBtn.Click += new EventHandler(visibleBtn_Click);

                        Button editBtn = new Button();
                        btnsCell.Controls.Add(editBtn);
                        editBtn.CssClass = "marginsLR";
                        editBtn.ID = string.Format("Edit{0}", type.ID);
                        editBtn.Text = "Edit";
                        editBtn.Click += new EventHandler(editBtn_Click);
                    }


                }
            }
            else
            {
                throw new CommonCode.UIException("There aren`t any company types, there should be atleast one (Other)");
            }
        }

        void editBtn_Click(object sender, EventArgs e)
        {
            Button btnVisible = sender as Button;
            if (btnVisible != null)
            {
                TableCell tblCell = btnVisible.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long typeID = -1;
                        string typeIdStr = tblRow.Attributes["id"];
                        if (long.TryParse(typeIdStr, out typeID))
                        {
                            BusinessCompanyType businessCompanyType = new BusinessCompanyType();

                            CompanyType currType = businessCompanyType.Get(objectContext, typeID, false);
                            if (currType == null)
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("Theres no Company type ID = {0} (comming from tblRow.Attributes['id']) , user id = {1}"
                                    , typeID, currentUser.ID));
                            }

                            tbName.Attributes.Add("typeId", currType.ID.ToString());
                            lblEditType.Text = string.Format("Edit '{0}' {1} type", currType.name, Configuration.CompanyName);
                            tbName.Text = currType.name;
                            tbDescription.Text = currType.description;
                            CheckAddEditCompanyTypeForm();

                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['id'] to long , user id = {0}", currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("coultn get parent Row");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("coultn get parent Cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("coultn get parent Button");
            }
        }


        void visibleBtn_Click(object sender, EventArgs e)
        {
            Button btnVisible = sender as Button;
            if (btnVisible != null)
            {
                TableCell tblCell = btnVisible.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long typeID = -1;
                        string typeIdStr = tblRow.Attributes["id"];
                        if (long.TryParse(typeIdStr, out typeID))
                        {
                            BusinessCompanyType businessCompanyType = new BusinessCompanyType();

                            CompanyType currType = businessCompanyType.Get(objectContext, typeID, false);
                            if (currType == null)
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("Theres no Company type ID = {0} (comming from tblRow.Attributes['id']) , user id = {1}"
                                    , typeID, currentUser.ID));
                            }

                            if (currType.visible == true)
                            {
                                businessCompanyType.DeleteCompanyType(objectContext, currType, currentUser, businessLog);
                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Company type deleted!");
                                FillDdlTypes();
                            }
                            else
                            {
                                businessCompanyType.UnDeleteCompanyType(objectContext, currType, currentUser, businessLog);
                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Company type Undeleted!");
                                FillDdlTypes();
                            }

                            FillTblCompanyTypes();
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['id'] to long , user id = {0}", currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("coultn get parent Row");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("coultn get parent Cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("coultn get parent Button");
            }
        }

        private void CheckAddEditCompanyTypeForm()
        {
            String strTypeId = tbName.Attributes["typeId"];
            if (!string.IsNullOrEmpty(strTypeId))
            {
                long id = -1;
                if (long.TryParse(strTypeId, out id))
                {
                    BusinessCompanyType businessCompanyType = new BusinessCompanyType();
                    CompanyType typeToEdit = businessCompanyType.Get(objectContext, id, false);
                    if (typeToEdit != null)
                    {
                        btnCancel.Visible = true;
                    }
                    else
                    {
                        Discard();
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Couldnt parse hfTextToEdit.Value to long.");
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Discard();
        }

        private void Discard()
        {
            tbName.Attributes.Clear();
            lblEditType.Text = string.Format("Add {0} type", Configuration.CompanyName);

            tbName.Text = "";
            tbDescription.Text = "";
            btnCancel.Visible = false;
            lblError.Visible = false;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblError.Visible = true;
            string error = "";

            BusinessCompanyType businessCompanyType = new BusinessCompanyType();
            string strTypeToEdit = tbName.Attributes["typeId"];

            if (string.IsNullOrEmpty(strTypeToEdit))
            {
                // Add

                string name = tbName.Text;

                if (CommonCode.Validate.ValidateName(objectContext, "companyType", ref name, Configuration.CompaniesMinCompanyNameLength,
                               Configuration.CompaniesMaxCompanyNameLength, out error, 0))
                {
                    string description = tbDescription.Text;

                    if (CommonCode.Validate.ValidateDescription(0,
                            Configuration.CompanyTypesMaxDescriptionLenght, ref description, "description", out error, Configuration.FieldsDefMaxWordLength))
                    {

                        businessCompanyType.AddCompanyType(objectContext, businessLog, currentUser, name, description);

                        FillDdlTypes();

                        ShowInfo();
                        tbName.Text = "";
                        tbDescription.Text = "";
                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "New company type added!");
                    }
                }


            }
            else
            {
                // Edit

                long typeID = -1;

                if (long.TryParse(strTypeToEdit, out typeID))
                {
                    CompanyType currType = businessCompanyType.Get(objectContext, typeID, false);
                    if (currType == null)
                    {
                        throw new CommonCode.UIException(string.Format
                            ("Theres no Company type ID = {0} which can be edited , user id = {1}"
                            , typeID, currentUser.ID));
                    }


                    // validate name

                    Boolean namePassed = true;
                    Boolean newName = false;

                    string name = tbName.Text;

                    if (currType.name != tbName.Text)
                    {
                        if (CommonCode.Validate.ValidateName(objectContext, "companyType", ref name, Configuration.CompaniesMinCompanyNameLength,
                               Configuration.CompaniesMaxCompanyNameLength, out error, 0))
                        {
                            newName = true;
                        }
                        else
                        {
                            namePassed = false;
                        }
                    }

                    // validate description

                    if (namePassed)
                    {
                        Boolean newDescription = false;
                        Boolean descrPassed = true;



                        if (currType.description != tbDescription.Text)
                        {
                            if (CommonCode.Validate.ValidateDescription(0, Configuration.CompanyTypesMaxDescriptionLenght,
                                tbDescription.Text, "description", out error, Configuration.FieldsDefMaxWordLength))
                            {
                                newDescription = true;
                            }
                            else
                            {
                                descrPassed = false;
                            }
                        }

                        if (descrPassed)
                        {
                            if (newName)
                            {
                                businessCompanyType.ChangeField(objectContext, currType, currentUser, businessLog, "name", name);
                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Company type name updated!");
                            }
                            if (newDescription)
                            {
                                businessCompanyType.ChangeField(objectContext, currType, currentUser, businessLog, "description", tbDescription.Text);
                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Company type description updated!");
                            }

                            if (newName == false && newDescription == false)
                            {
                                error = "Type new name, description or press cancel.";
                            }
                            else
                            {
                                FillDdlTypes();
                                Discard();
                                ShowInfo();
                            }
                        }


                    }
                }
            }

            lblError.Text = error;
        }

        protected void btnShowCompanies_Click(object sender, EventArgs e)
        {
            tblCompanies.Rows.Clear();
            tblCompanies.Visible = true;

            BusinessCompanyType businessCompanyType = new BusinessCompanyType();

            string strTypeID = ddlTypes.SelectedValue;
            if (string.IsNullOrEmpty(strTypeID))
            {
                throw new CommonCode.UIException(string.Format("ddlTypes.SelectedValue is null or emprty, user id = {0}", currentUser.ID));
            }

            long typeID = -1;
            if (long.TryParse(strTypeID, out typeID))
            {

                CompanyType currType = businessCompanyType.Get(objectContext, typeID, true);
                if (currType == null)
                {
                    throw new CommonCode.UIException(string.Format("There is no company type with id = {0}, or it is visible false, user id = {1}"
                        , typeID, currentUser.ID));
                }

                List<Company> companies = businessCompanyType.GetAllCompaniesWhichAreFromType(objectContext, currType);
                int compCount = companies.Count;
                if (compCount > 0)
                {
                    TableRow headerRow = new TableRow();
                    tblCompanies.Rows.Add(headerRow);

                    TableCell typeCell = new TableCell();
                    headerRow.Cells.Add(typeCell);
                    typeCell.Text = string.Format("{1}s with type : {0}", currType.name, Configuration.CompanyName);
                    typeCell.HorizontalAlign = HorizontalAlign.Center;

                    int fieldPerRow = 5;
                    int numCurrCompany = 0;

                    if (fieldPerRow >= compCount)
                    {
                        typeCell.ColumnSpan = fieldPerRow;
                    }
                    else
                    {
                        typeCell.ColumnSpan = compCount;
                    }

                    TableRow newRow = new TableRow();
                    tblCompanies.Rows.Add(newRow);

                    BusinessCompany businessCompany = new BusinessCompany();
                    Company otherCompany = businessCompany.GetOther(objectContext);

                    foreach (Company company in companies)
                    {
                        if (numCurrCompany++ == fieldPerRow)
                        {
                            newRow = new TableRow();
                            tblCompanies.Rows.Add(newRow);
                            numCurrCompany = 1;
                        }


                        TableCell newCell = new TableCell();
                        newRow.Cells.Add(newCell);
                        newCell.HorizontalAlign = HorizontalAlign.Center;
                        if (company != otherCompany)
                        {
                            newCell.Controls.Add(CommonCode.UiTools.GetCompanyHyperLink(company));
                        }
                        else
                        {
                            newCell.Text = otherCompany.name;
                        }
                    }

                }
                else
                {
                    TableRow nodescrRow = new TableRow();
                    tblCompanies.Rows.Add(nodescrRow);

                    TableCell noCell = new TableCell();
                    nodescrRow.Cells.Add(noCell);
                    noCell.Text = string.Format("No {1} with type '{0}'", currType.name, Configuration.CompanyName);
                }

            }
            else
            {
                throw new CommonCode.UIException(string.Format("couldn`t parse ddlTypes.SelectedValue to long, user id = {0}", currentUser.ID));
            }


        }

    }
}
