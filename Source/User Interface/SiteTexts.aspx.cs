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
    public partial class SiteTexts : BasePage
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

        protected void Page_PreRender(object sender, EventArgs e)
        {
            tbName.Attributes.Add("onkeyup", string.Format("JSCheckData('{0}','sitetextAdd','{1}','');", tbName.ClientID, lblCName.ClientID));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetNeedsToBeLogged();
            CheckUser();                            // Checks User and redirrects to error page if hes not admin
            ShowInfo();
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);
        }

        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);
            if (currUser != null)
            {
                if (businessUser.IsAdminOrGlobalAdmin(currUser))
                {
                    // ok 
                    currentUser = currUser;
                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, "This page is for Administrators only.");
                }
            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, "This page is for Administrators only.");
            }
        }

        private void ShowInfo()
        {
            Title = "Edit Site Text page";
            if (IsPostBack == false)
            {
                FillDdlType();
                FillDdlShowLastTypes();
            }

            FillTblLastTexts();
            CheckSiteTextForm();
            FillTblMissingTexts();

            lblCName.Text = "";

            BusinessSiteText siteText = new BusinessSiteText();
            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutSiteTexts");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblAbout.Text = aboutExtended.description;
            }
            else
            {
                lblAbout.Text = "About SiteTexts text not typed.";
            }
        }

        private void CheckSiteTextForm()
        {
            String strTextId = tbName.Attributes["textId"];
            if (!string.IsNullOrEmpty(strTextId))
            {
                long id = -1;
                if (long.TryParse(strTextId, out id))
                {
                    BusinessSiteText businessSiteText = new BusinessSiteText();
                    SiteNews textToEdit = businessSiteText.Get(objectContext, id);
                    if (textToEdit != null)
                    {
                        btnCancel.Visible = true;
                        ddlType.Enabled = false;

                        switch (textToEdit.type)
                        {
                            case("news"):
                                ddlType.SelectedIndex = 2;
                                break;
                            case("rule"):
                                ddlType.SelectedIndex = 4;
                                break;
                            case("text"):
                                ddlType.SelectedIndex = 1;
                                break;
                            case("qna"):
                                ddlType.SelectedIndex = 3;
                                break;
                            case ("warningPattern"):
                                ddlType.SelectedIndex = 5;
                                break;
                            case ("reportPattern"):
                                ddlType.SelectedIndex = 6;
                                break;
                            case ("information"):
                                ddlType.SelectedIndex = 7;
                                break;
                            default:
                                throw new CommonCode.UIException(string.Format("SiteText with type = '{0}' is not supported type (add new case for that type), user id = {1}"
                                    , textToEdit.type, currentUser.ID));
                        }
                    }
                    else
                    {
                        tbName.Attributes.Clear();
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Couldnt parse hfTextToEdit.Value to long.");
                }
            }
        }

        private void FillDdlType()
        {
            ddlType.Items.Clear();

            ListItem chooseItem = new ListItem();
            chooseItem.Text = "choose...";
            chooseItem.Value = "-1";
            ddlType.Items.Add(chooseItem);

            ListItem otherItem = new ListItem();
            otherItem.Text = "text";
            otherItem.Value = "0";
            ddlType.Items.Add(otherItem);

            ListItem newsItem = new ListItem();
            newsItem.Text = "news";
            newsItem.Value = "1";
            ddlType.Items.Add(newsItem);

            ListItem qnaItem = new ListItem();
            qnaItem.Text = "question and answer";
            qnaItem.Value = "2";
            ddlType.Items.Add(qnaItem);

            ListItem ruleItem = new ListItem();
            ruleItem.Text = "rule";
            ruleItem.Value = "3";
            ddlType.Items.Add(ruleItem);

            ListItem warnItem = new ListItem();
            warnItem.Text = "warning pattern";
            warnItem.Value = "4";
            ddlType.Items.Add(warnItem);

            ListItem repPattItem = new ListItem();
            repPattItem.Text = "report pattern";
            repPattItem.Value = "5";
            ddlType.Items.Add(repPattItem);

            ListItem infItem = new ListItem();
            infItem.Text = "guide text";
            infItem.Value = "6";
            ddlType.Items.Add(infItem);


            ddlType.SelectedIndex = 0;
        }

        private void FillDdlShowLastTypes()
        {

            ddlShowLastTypes.Items.Clear();
            
            ListItem allItem = new ListItem();
            allItem.Text = "all";
            allItem.Value = "all";
            ddlShowLastTypes.Items.Add(allItem);

            ListItem otherItem = new ListItem();
            otherItem.Text = "texts";
            otherItem.Value = "text";
            ddlShowLastTypes.Items.Add(otherItem);

            ListItem newsItem = new ListItem();
            newsItem.Text = "news";
            newsItem.Value = "news";
            ddlShowLastTypes.Items.Add(newsItem);

            ListItem qnaItem = new ListItem();
            qnaItem.Text = "questions and answers";
            qnaItem.Value = "qna";
            ddlShowLastTypes.Items.Add(qnaItem);

            ListItem ruleItem = new ListItem();
            ruleItem.Text = "rules";
            ruleItem.Value = "rules";
            ddlShowLastTypes.Items.Add(ruleItem);

            ListItem warnItem = new ListItem();
            warnItem.Text = "warning patterns";
            warnItem.Value = "warning patterns";
            ddlShowLastTypes.Items.Add(warnItem);

            ListItem repPattItem = new ListItem();
            repPattItem.Text = "report patterns";
            repPattItem.Value = "report patterns";
            ddlShowLastTypes.Items.Add(repPattItem);

            ListItem infItem = new ListItem();
            infItem.Text = "guide text";
            infItem.Value = "information";
            ddlShowLastTypes.Items.Add(infItem);

            ddlType.SelectedIndex = 0;

        }

        private void FillTblMissingTexts()
        {
            tblMissing.Rows.Clear();

            BusinessSiteText businessSiteText = new BusinessSiteText();
            List<String> missingTexts = businessSiteText.GetMissingTexts(objectContext);

            if (missingTexts.Count > 0)
            {
                tblMissing.Visible = true;

                TableRow firstRow = new TableRow();
                tblMissing.Rows.Add(firstRow);

                TableCell aboutCell = new TableCell();
                firstRow.Cells.Add(aboutCell);
                aboutCell.Text = "The Texts with following names are not typed :";

                foreach (String text in missingTexts)
                {
                    TableRow newRow = new TableRow();
                    tblMissing.Rows.Add(newRow);

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);
                    nameCell.Text = text;
                }
            }
            else
            {
                tblMissing.Visible = false;
            }

        }

        protected void btnAddText_Click(object sender, EventArgs e)
        {
            phAddText.Controls.Add(lblError);
            phAddText.Visible = true;
            string error = "";

            BusinessSiteText businessSiteText = new BusinessSiteText();
            string strTextToEdit = tbName.Attributes["textId"];

            if (string.IsNullOrEmpty(strTextToEdit))
            {
                String strType = ddlType.SelectedValue;
                int type = -1;
                if (int.TryParse(strType, out type))
                {
                    strType = string.Empty;
                    bool valdiationPassed = true;

                    switch (type)
                    {
                        case (-1):
                            valdiationPassed = false;
                            error = "choose text type !";
                            strType = "error";
                            break;
                        case (0):
                            strType = "text";
                            break;
                        case (1):
                            strType = "news";
                            break;
                        case (2):
                            strType = "qna";
                            break;
                        case (3):
                            strType = "rule";
                            break;
                        case (4):
                            strType = "warningPattern";
                            break;
                        case (5):
                            strType = "reportPattern";
                            break;
                        case (6):
                            strType = "information";
                            break;
                        default:
                            throw new CommonCode.UIException(string.Format("Type = {0} is not supported type , user id = {1}", type, currentUser.ID));
                    }

                    string description = ajxEditor.Content;
                    string name = tbName.Text;

                    if (valdiationPassed == false || !CommonCode.Validate.ValidateName(objectContext, "siteText", ref name, Configuration.SiteTextsMinSiteTextNameLength,
                               Configuration.SiteTextsMaxSiteTextNameLength, out error, 0))
                    {
                        valdiationPassed = false;
                    }
                 
                    if (valdiationPassed == false || !CommonCode.Validate.ValidateDescription(Configuration.SiteTextsMinSiteTextDescr,
                    Configuration.SiteTextsMaxSiteTextDescr, ref description, "description", out error, 200))
                    {
                        valdiationPassed = false;
                    }

                    if (valdiationPassed == false || !businessSiteText.IsLinkIdFormatValid(tbLinkId.Text))
                    {
                        if (valdiationPassed == true)
                        {
                            error = "Invalid Link ID format.";
                            valdiationPassed = false;
                        }
                    }

                    if (valdiationPassed == false || businessSiteText.CheckIfLinkIdIsTaken(objectContext, strType, tbLinkId.Text) == true)
                    {
                        if (valdiationPassed == true)
                        {
                            error = "This Link ID is already taken.";
                            valdiationPassed = false;
                        }
                    }

                    if (valdiationPassed == true)
                    {
                        Boolean changeDone = false;

                        switch (strType)
                        {
                            case ("text"):
                                businessSiteText.CreateSiteText(objectContext, businessLog, description, currentUser, name, tbLinkId.Text);
                                changeDone = true;
                                break;
                            case ("news"):
                                businessSiteText.CreateNews(objectContext, businessLog, description, currentUser, name, tbLinkId.Text);
                                changeDone = true;
                                break;
                            case ("qna"):
                                businessSiteText.CreateQuestionAndAnswer(objectContext, businessLog, description, currentUser, name, tbLinkId.Text);
                                changeDone = true;
                                break;
                            case ("rule"):
                                businessSiteText.CreateRule(objectContext, businessLog, description, currentUser, name, tbLinkId.Text);
                                changeDone = true;
                                break;
                            case ("warningPattern"):
                                businessSiteText.CreateWarningPattern(objectContext, businessLog, description, currentUser, name, tbLinkId.Text);
                                changeDone = true;
                                break;
                            case ("reportPattern"):
                                businessSiteText.CreateReportPattern(objectContext, businessLog, description, currentUser, name, tbLinkId.Text);
                                changeDone = true;
                                break;
                            case ("information"):
                                businessSiteText.CreateInformation(objectContext, businessLog, description, currentUser, name, tbLinkId.Text);
                                changeDone = true;
                                break;
                            default:
                                throw new CommonCode.UIException(string.Format("Type = {0} is not supported type , user id = {1}", strType, currentUser.ID));
                        }

                        if (changeDone)
                        {
                            tbName.Text = string.Empty;
                            ajxEditor.Content = string.Empty;
                            tbLinkId.Text = string.Empty;
                            phAddText.Visible = false;
                            ShowInfo();
                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Text added!");
                        }


                    }

                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("couldnt parse ddlType.SelectedValue = {0} to int , user id = {1}", strType, currentUser.ID));
                }
            }
            else
            {
                long id = -1;
                if (long.TryParse(strTextToEdit, out id))
                {
                    SiteNews TextToBeEdited = businessSiteText.Get(objectContext, id);
                    if (TextToBeEdited == null)
                    {
                        throw new BusinessException(string.Format("There is no text id = {0} , user id = {1}", id, currentUser.ID));
                    }

                    Boolean namePassed = true;
                    Boolean newName = false;

                    string name = tbName.Text;

                    if (TextToBeEdited.name != tbName.Text)
                    {
                        if (CommonCode.Validate.ValidateName(objectContext, "siteText", ref name, Configuration.SiteTextsMinSiteTextNameLength,
                               Configuration.SiteTextsMaxSiteTextNameLength, out error, 0))
                        {
                            newName = true;
                        }
                        else
                        {
                            namePassed = false;
                        }
                    }

                    if (namePassed)
                    {
                        Boolean newDescription = false;
                        Boolean descrPassed = true;

                        string textDescr = ajxEditor.Content;

                        if (TextToBeEdited.description != ajxEditor.Content /*tbDescription.Text*/)
                        {
                            if (CommonCode.Validate.ValidateDescription(Configuration.SiteTextsMinSiteTextDescr,
                            Configuration.SiteTextsMaxSiteTextDescr, ref textDescr, "description", out error, 90))
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
                            bool newLinkID = false;
                            bool linkIDPassed = true;

                            if (tbLinkId.Text != TextToBeEdited.linkID)
                            {
                                if (businessSiteText.IsLinkIdFormatValid(tbLinkId.Text) == false)
                                {
                                    error = "Incorrect Link ID format.";
                                    linkIDPassed = false;
                                }
                                else if (businessSiteText.CheckIfLinkIdIsTaken(objectContext, TextToBeEdited.type, tbLinkId.Text) == false)
                                {
                                    newLinkID = true;
                                }
                                else
                                {
                                    linkIDPassed = false;
                                    error = "That Link ID is already taken.";
                                }
                            }

                            if (linkIDPassed)
                            {
                                if (newName)
                                {
                                    businessSiteText.ChangeName(objectContext, businessLog, currentUser, TextToBeEdited, name);
                                }
                                if (newDescription)
                                {
                                    businessSiteText.ChangeDescription(objectContext, businessLog, currentUser, TextToBeEdited, textDescr);
                                }
                                if (newLinkID)
                                {
                                    businessSiteText.ChangeLinkID(objectContext, businessLog, currentUser, TextToBeEdited, tbLinkId.Text);
                                }

                                if (newName == false && newDescription == false && newLinkID == false)
                                {
                                    error = "Type either new : name, description, link ID, or press cancel.";
                                }
                                else
                                {
                                    tbName.Text = "";
                                    ajxEditor.Content = "";
                                    tbLinkId.Text = string.Empty;
                                    tbName.Attributes.Clear();
                                    ddlType.Enabled = true;
                                    btnCancel.Visible = false;
                                    ShowInfo();
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Text updated!");
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new BusinessException(string.Format
                        ("Couldnt parse tbName.Attributes['textId'] = {0} to long , user id = {1}", strTextToEdit, currentUser.ID));
                }
            }

            lblError.Text = error;

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ddlType.Enabled = true;
            btnCancel.Visible = false;
            tbName.Attributes.Clear();
            ajxEditor.Content = "";
            tbName.Text = "";
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)  // ne se polzva
        {
            String strValue = ddlType.SelectedValue;
            int value = -1;
            if (int.TryParse(strValue, out value))
            {
                switch (value)
                {
                    case(0) :
                        tbName.Enabled = true;
                        break;
                    case(1) :
                        tbName.Enabled = false;
                        break;
                    default :
                        throw new CommonCode.UIException(string.Format
                            ("ddlType SelectedIndexValue = {0} is not supported value , user id = {1}", value, currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format
                    ("Couldnt Parse ddlType.SelectedValue = {0 }to int , user id = {1}", strValue, currentUser.ID));
            }
        }

        private void FillTblLastTexts()
        {
            tblLastTexts.Rows.Clear();

            BusinessSiteText businessSiteText = new BusinessSiteText();
            List<SiteNews> LastTexts = GetSiteTextsFromParams(businessSiteText);
            
            int count = LastTexts.Count;

            if (count > 0)
            {
                foreach (SiteNews text in LastTexts)
                {
                    if (!text.CreatedByReference.IsLoaded)
                    {
                        text.CreatedByReference.Load();
                    }
                    if (!text.LastModifiedByReference.IsLoaded)
                    {
                        text.LastModifiedByReference.Load();
                    }

                    TableRow textRow = new TableRow();
                    tblLastTexts.Rows.Add(textRow);

                    TableCell textCell = new TableCell();
                    textRow.Cells.Add(textCell);
                    textCell.CssClass = "commentsTD";

                    Table newTable = new Table();
                    textCell.Controls.Add(newTable);
                    newTable.GridLines = GridLines.Both;
                    newTable.Width = Unit.Percentage(100);

                    //////////

                    TableRow newRow = new TableRow();
                    newTable.Rows.Add(newRow);

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);
                    nameCell.ColumnSpan = 2;
                    nameCell.Text = text.name;

                    TableRow descrRow = new TableRow();
                    newTable.Rows.Add(descrRow);

                    TableCell infoCell = new TableCell();
                    infoCell.Width = Unit.Pixel(300);
                    infoCell.VerticalAlign = VerticalAlign.Top;
                    descrRow.Cells.Add(infoCell);

                    System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    infoCell.Controls.Add(div);

                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("ID : {0}", text.ID.ToString()), true));
                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("Type : {0}", text.type), true));

                    if (string.IsNullOrEmpty(text.linkID))
                    {
                        div.Controls.Add(CommonCode.UiTools.GetLabelWithText("Link ID : Not set", true));
                    }
                    else
                    {
                        div.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("Link ID : {0}", text.linkID), true));
                    }

                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("Date : {0}"
                        , CommonCode.UiTools.DateTimeToLocalString(text.dateCreated)), true));
                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText("Written by : ", false));
                    div.Controls.Add(CommonCode.UiTools.GetUserHyperLink(Tools.GetUserFromUserDatabase(text.CreatedBy)));
                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText("", true));
                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText("Last modified by : ", false));
                    div.Controls.Add(CommonCode.UiTools.GetUserHyperLink(Tools.GetUserFromUserDatabase(text.LastModifiedBy)));
                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText("", true));
                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("Last Modified : {0}"
                        , CommonCode.UiTools.DateTimeToLocalString(text.lastModified)), true));
                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("Visible : {0}", text.visible.ToString()), true));

                    TableCell descrCell = new TableCell();
                    descrCell.VerticalAlign = VerticalAlign.Top;
                    descrRow.Cells.Add(descrCell);
                    descrCell.Text = text.description; 

                    TableRow btnsRow = new TableRow();
                    btnsRow.Attributes.Add("TextId", text.ID.ToString());
                    newTable.Rows.Add(btnsRow);

                    TableCell btnsCell = new TableCell();
                    btnsCell.ColumnSpan = 2;
                    btnsRow.Cells.Add(btnsCell);

                    Button editBtn = new Button();
                    editBtn.ID = string.Format("Edit{0}",text.ID);
                    editBtn.Text = "Edit";
                    editBtn.Click += new EventHandler(EditSiteText_Click);
                    btnsCell.Controls.Add(editBtn);

                    Button delUndelBtn = new Button();
                    delUndelBtn.ID = string.Format("DelOrUndel{0}", text.ID);
                    if (text.visible)
                    {
                        delUndelBtn.Text = "Delete";
                    }
                    else
                    {
                        delUndelBtn.Text = "UnDelete";
                    }
                    delUndelBtn.Click += new EventHandler(DeleteOrUndeleteSiteText_Click);
                    btnsCell.Controls.Add(delUndelBtn);
                }

            }
            else
            {
                TableRow lastRow = new TableRow();
                tblLastTexts.Rows.Add(lastRow);

                TableCell lastCell = new TableCell();
                lastRow.Cells.Add(lastCell);
                lastCell.Text = "No entered Site Texts";

            }
        }

        private List<SiteNews> GetSiteTextsFromParams(BusinessSiteText businessSiteText)
        {
            List<SiteNews> LastTexts = new List<SiteNews>();

            if (!string.IsNullOrEmpty(hfId.Value) || !string.IsNullOrEmpty(hfShowLast.Value))
            {
                if (!string.IsNullOrEmpty(hfId.Value) && !string.IsNullOrEmpty(hfShowLast.Value))
                {
                    hfId.Value = null;
                    hfShowLast = null;
                    hfShowTypes = null;
                    hfVisible = null;
                    LastTexts = businessSiteText.GetLastTexts(objectContext, Configuration.SiteTextsNumLastTextsToShow, "all", 0);
                }
                else
                {
                    if (!string.IsNullOrEmpty(hfId.Value))
                    {
                        if (string.IsNullOrEmpty(hfShowTypes.Value))  // search for text with ID
                        {
                            long id = -1;
                            if (long.TryParse(hfId.Value, out id))
                            {
                                SiteNews textToShow = businessSiteText.Get(objectContext, id);
                                if (textToShow == null)
                                {
                                    throw new CommonCode.UIException(string.Format("Theres no SiteText ID = {0} , user id = {1}", id, currentUser.ID));
                                }

                                LastTexts.Add(textToShow);
                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("Couldnt parse hfId.Value = {0} to long , user id = {1}", hfId.Value, currentUser.ID));
                            }
                        }
                        else if (hfShowTypes.Value == "search") // search for text with NAME
                        {
                            BusinessSearch businessSearch = new BusinessSearch();
                            LastTexts = businessSearch.SearchInSiteTexts(objectContext, hfId.Value);
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("Type = '{0}' is not supported type , User id = {1}",
                                hfShowTypes.Value, currentUser.ID));
                        }
                    }
                    else if (!string.IsNullOrEmpty(hfShowLast.Value))
                    {
                        int num = -1;
                        if (int.TryParse(hfShowLast.Value, out num))
                        {
                            string type = hfShowTypes.Value;
                            if (!string.IsNullOrEmpty(type))
                            {
                                if (string.IsNullOrEmpty(hfVisible.Value))
                                {
                                    throw new CommonCode.UIException(string.Format("hfVisible.Value is empty or null , user id = {0}", currentUser.ID));
                                }

                                int visibility = 0;
                                if (int.TryParse(hfVisible.Value, out visibility))
                                {
                                    if (num > 0)
                                    {
                                        LastTexts = businessSiteText.GetLastTexts(objectContext, num, type, visibility);
                                    }
                                    else
                                    {
                                        throw new CommonCode.UIException(string.Format
                                            ("num is < 1 (comming from hfShowLast.Value) , user id = {0}", currentUser.ID));
                                    }
                                }
                                else
                                {
                                    throw new CommonCode.UIException(string.Format("hfShowTypes.Value couldnt be parsed to int , user id = {0}", currentUser.ID));
                                }
                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format("hfShowTypes.Value is empty or null , user id = {0}", currentUser.ID));
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse hfShowLast.Value = {0} to int , user id = {1}", hfShowLast.Value, currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format("Both hfShowLast and hfId are empty or null , user id = {0}", currentUser.ID));
                    }
                }
            }
            else
            {
                LastTexts = businessSiteText.GetLastTexts(objectContext, Configuration.SiteTextsNumLastTextsToShow, "all", 0);
            }

            return LastTexts;
        }

        void DeleteOrUndeleteSiteText_Click(object sender, EventArgs e)
        {
            BusinessSiteText businessSiteText = new BusinessSiteText();
        
            Button btnedit = sender as Button;
            if (btnedit != null)
            {
                TableCell tblCell = btnedit.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long textID = 0;
                        string commentIdStr = tblRow.Attributes["TextId"];
                        long.TryParse(commentIdStr, out textID);

                        if (textID > 0)
                        {
                            SiteNews textToEdit = businessSiteText.Get(objectContext, textID);

                            if (textToEdit != null)
                            {
                                if (textToEdit.visible)
                                {
                                    businessSiteText.DeleteNewsOrText(objectContext, businessLog, currentUser, textToEdit);
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Text deleted!");
                                }
                                else
                                {
                                    businessSiteText.UnDeleteNewsOrText(objectContext, businessLog, currentUser, textToEdit);
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Text undeleted!");
                                }

                                ShowInfo();
                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format("Theres no SiteText ID = {0} , user id = {1}", textID, currentUser.ID));
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("textID is < 1 (comming from tblRow.Attributes['TextId']) , user id = {0}", currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldnt get parent Row");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Couldnt get parent Cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }
        
        }

        protected void EditSiteText_Click(object sender, EventArgs e)
        {
            BusinessSiteText businessSiteText = new BusinessSiteText();
           
            Button btnedit = sender as Button;
            if (btnedit != null)
            {
                TableCell tblCell = btnedit.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long textID = 0;
                        string commentIdStr = tblRow.Attributes["TextId"];
                        long.TryParse(commentIdStr, out textID);

                        if (textID > 0)
                        {
                            SiteNews textToEdit = businessSiteText.Get(objectContext, textID);

                            if (textToEdit != null)
                            {
                                tbName.Attributes.Add("textId", textToEdit.ID.ToString());
                                tbName.Text = textToEdit.name;
                                tbLinkId.Text = textToEdit.linkID;
                                ajxEditor.Content  = textToEdit.description;
                                CheckSiteTextForm();
                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("Theres no SiteText ID = {0} , user id = {1}", textID, currentUser.ID));
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("textID is < 1 (comming from tblRow.Attributes['TextId']) , user id = {0}", currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldnt get parent Row");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Couldnt get parent Cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }
        }

        protected void btnShowLast_Click(object sender, EventArgs e)
        {
            phShowLast.Visible = true;
            phShowLast.Controls.Add(lblError);
            String error = "";

            if (CommonCode.Validate.ValidateLong(tbShowLast.Text, out error))
            {
                long num = -1;
                if (long.TryParse(tbShowLast.Text, out num))
                {
                    string type = ddlShowLastTypes.SelectedValue;
                    if (!string.IsNullOrEmpty(type))
                    {
                        int visible = 0;
                        if (int.TryParse(ddlVisible.SelectedValue, out visible))
                        {
                            if (visible < 0 || visible > 2)
                            {
                                throw new CommonCode.UIException(string.Format("visible is < 0 or > 2, user id = {0}", currentUser.ID));
                            }

                            BusinessSiteText businessSiteText = new BusinessSiteText();
                            if (num > 0)
                            {
                                hfShowLast.Value = num.ToString();
                                hfShowTypes.Value = type;
                                hfVisible.Value = visible.ToString();
                                hfId.Value = null;
                                FillTblLastTexts();
                                phShowLast.Visible = false;

                                ClearEditText();
                            }
                            else
                            {
                                error = "Number must be positive.";
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("ddlVisible.SelectedValue couldn`t be parsed to int , user id = {0}", currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format
                            ("ddlShowLastTypes.SelectedValue is null or empty , user id = {0}", currentUser.ID));
                    }   
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("Couldnt parse tbShowLast.Text = {0} to long , user id = {1}", tbShowLast.Text, currentUser.ID));
                }
            }
            

            lblError.Text = error;
        }

        private void ClearEditText()
        {
            tbName.Attributes.Clear();
            tbName.Text = "";
            ajxEditor.Content = "";
            ddlType.Enabled = true;
            btnCancel.Visible = false;
            tbSearchFor.Text = "";        
        }

        protected void btnShowId_Click(object sender, EventArgs e)
        {
            phShowId.Visible = true;
            phShowId.Controls.Add(lblError);
            String error = "";

            if (CommonCode.Validate.ValidateLong(tbShowId.Text, out error))
            {
                long id = -1;
                if (long.TryParse(tbShowId.Text, out id))
                {
                    BusinessSiteText businessSiteText = new BusinessSiteText();
                    SiteNews TextToShow = businessSiteText.Get(objectContext, id);

                    if (TextToShow != null)
                    {
                        hfShowLast.Value = null;
                        hfShowTypes.Value = null;
                        hfVisible.Value = null;

                        hfId.Value = TextToShow.ID.ToString();
                        FillTblLastTexts();
                        phShowId.Visible = false;

                        ClearEditText();
                    }
                    else
                    {
                        error = string.Format("There is no text id = {0} ", id);
                    }
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("Couldnt parse tbShowLast.Text = {0} to long , user id = {1}", tbShowLast.Text, currentUser.ID));
                }
            }

            lblError.Text = error;
        }

        [WebMethod]
        public static string CheckData(string text, string type, string notUsed)
        {
            string error = "";
            CommonCode.WebMethods.ValidateUserInput(text, type, "", out error);
            return error;
        }

        protected void btnSearchText_Click(object sender, EventArgs e)
        {
            phSearch.Visible = true;
            phSearch.Controls.Add(lblError);
            String error = "";

            if (CommonCode.Validate.ValidateDescription(Configuration.SiteTextsMinSiteTextNameLength,
                Configuration.SiteTextsMaxSiteTextNameLength, tbSearchFor.Text, "name", out error, Configuration.FieldsDefMaxWordLength))
            {
                
                BusinessSiteText businessSiteText = new BusinessSiteText();
                BusinessSearch businessSearch = new BusinessSearch();

                List<SiteNews> foundTexts = businessSearch.SearchInSiteTexts(objectContext, tbSearchFor.Text);

                hfVisible.Value = null;
                hfShowLast.Value = null;
                hfShowTypes.Value = null;
                hfId.Value = null;

                hfShowTypes.Value = "search";
                hfId.Value = tbSearchFor.Text;
                FillTblLastTexts();
                phSearch.Visible = false;

                ClearEditText();
            }

            lblError.Text = error;
        }

    }
}
