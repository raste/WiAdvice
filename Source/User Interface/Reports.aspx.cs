﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.Services;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class Reports : BasePage 
    {
        private User CurrentUser = null;

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
            CheckUser();                                // Checks user if not admin redirrects to error page
            if (IsPostBack == false)
            {                                           // Fills RadioButtonLists and DropDown lists with options
                FillRblViewedMode(rblViewedMode);       
                FillDdlAboutType();
                FillDdlTypeReport(ddlTypeReport);
                ///////
                FillRblViewedMode(rblViewedMode2);
                FillDdlAboutType2();
                FillDdlTypeReport(ddlTypeReport2);
                //////
                hfReplyToReport.Value = "";
            }

            ShowInfo();
            ShowReportsTableFromSessionParams();        // Shows reports
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);
        }

        private void ShowInfo()
        {
            Title = "Reports Page";

            BusinessSiteText siteText = new BusinessSiteText();

            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutReports");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblAbout.Text = aboutExtended.description; 
            }
            else
            {
                lblAbout.Text = "About Reports Text not typed.";
            }

            FillReportPatterns();
        }

        private void ShowReportsTableFromSessionParams()
        {
            if (IsPostBack)
            {
                object aboutTypeObj = Session["ReportAboutType"];
                object reportTypeObj = Session["ReportType"];
                object viewTypeObj = Session["ReportViewType"];
                object countObj = Session["ReportsCount"];
                object aboutTypeIdObj = Session["ReportAboutTypeID"];

                if (aboutTypeObj != null && reportTypeObj != null && viewTypeObj != null
                    && countObj != null && aboutTypeIdObj != null)
                {
                    string aboutType = aboutTypeObj as string;
                    string reportType = reportTypeObj as string;
                    string viewType = viewTypeObj as string;
                    int count;
                    bool countParsed = int.TryParse(countObj.ToString(), out count);
                    long aboutTypeId;
                    bool aboutTypeIdParsed = long.TryParse(aboutTypeIdObj.ToString(), out aboutTypeId);

                    if (aboutType == null)
                    {
                        string msg = string.Format("{0} must be of type \"{1}\", but it is of type \"{2}\".",
                            "aboutType", typeof(string).FullName, aboutTypeObj.GetType().FullName);
                        throw new CommonCode.UIException(msg);
                    }
                    if (reportType == null)
                    {
                        string msg = string.Format("{0} must be of type \"{1}\", but it is of type \"{2}\".",
                            "reportType", typeof(string).FullName, reportTypeObj.GetType().FullName);
                        throw new CommonCode.UIException(msg);
                    }
                    if (viewType == null)
                    {
                        string msg = string.Format("{0} must be of type \"{1}\", but it is of type \"{2}\".",
                            "viewType", typeof(string).FullName, viewTypeObj.GetType().FullName);
                        throw new CommonCode.UIException(msg);
                    }
                    if (countParsed == false)
                    {
                        string msg = string.Format("{0} must be of type \"{1}\", but it is of type \"{2}\".",
                            "count", typeof(int).FullName, countObj.GetType().FullName);
                        throw new CommonCode.UIException(msg);
                    }
                    if (aboutTypeIdParsed == false)
                    {
                        string msg = string.Format("{0} must be of type \"{1}\", but it is of type \"{2}\".",
                            "aboutTypeId", typeof(int).FullName, aboutTypeIdObj.GetType().FullName);
                        throw new CommonCode.UIException(msg);
                    }

                    BusinessReport businessReport = new BusinessReport();
                    List<Report> Reports = businessReport.GetReports(objectContext,
                        aboutType, aboutTypeId, reportType, viewType, count);

                    FillReportsTable(Reports);

                }
            }
        }

        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);
            if (currUser != null)
            {
                if (businessUser.IsFromAdminTeam(currUser))
                {
                    CurrentUser = currUser;
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

            if (CurrentUser == null)
            {
                throw new CommonCode.UIException("CurrentUser is null");
            }
        }

        private void FillRblViewedMode(RadioButtonList rblList)
        {
            ListItem allItem = new ListItem();
            allItem.Text = "all";
            allItem.Value = "all";
            rblList.Items.Add(allItem);

            ListItem viewedItem = new ListItem();
            viewedItem.Text = "viewed";
            viewedItem.Value = "Viewed";
            rblList.Items.Add(viewedItem);

            ListItem notViewedItem = new ListItem();
            notViewedItem.Text = "not viewed";
            notViewedItem.Value = "notViewed";
            rblList.Items.Add(notViewedItem);

            ListItem notResolvedItem = new ListItem();
            notResolvedItem.Text = "not resolved";
            notResolvedItem.Value = "notResolved";
            rblList.Items.Add(notResolvedItem);

            ListItem resolvedItem = new ListItem();
            resolvedItem.Text = "resolved";
            resolvedItem.Value = "Resolved";
            rblList.Items.Add(resolvedItem);

            rblList.SelectedIndex = 0;
        }

        private void FillDdlTypeReport(DropDownList ddlList)
        {
            ListItem allItem = new ListItem();
            allItem.Text = "all";
            allItem.Value = "all";
            ddlList.Items.Add(allItem);

            ListItem irregItem = new ListItem();
            irregItem.Text = "irregularity";
            irregItem.Value = "irregularity";
            ddlList.Items.Add(irregItem);

            ListItem spamItem = new ListItem();
            spamItem.Text = "spam";
            spamItem.Value = "spam";
            ddlList.Items.Add(spamItem);

            ddlList.SelectedIndex = 0;
        }

        private void FillDdlAboutType()
        {
            ListItem allItem = new ListItem();
            allItem.Text = "all";
            allItem.Value = "all";
            ddlAboutType.Items.Add(allItem);

            ListItem genItem = new ListItem();
            genItem.Text = "general";
            genItem.Value = "general";
            ddlAboutType.Items.Add(genItem);

            ListItem compItem = new ListItem();
            compItem.Text = "companies";
            compItem.Value = "aCompanies";
            ddlAboutType.Items.Add(compItem);

            ListItem prodItem = new ListItem();
            prodItem.Text = "products";
            prodItem.Value = "aProducts";
            ddlAboutType.Items.Add(prodItem);

            ListItem prodTopicItem = new ListItem();
            prodTopicItem.Text = "product topics";
            prodTopicItem.Value = "aProductTopics";
            ddlAboutType.Items.Add(prodTopicItem);

            ListItem prodLinks = new ListItem();
            prodLinks.Text = "product links";
            prodLinks.Value = "aProductLinks";
            ddlAboutType.Items.Add(prodLinks);

            ListItem catItem = new ListItem();
            catItem.Text = "categories";
            catItem.Value = "aCategories";
            ddlAboutType.Items.Add(catItem);

            ListItem commItem = new ListItem();
            commItem.Text = "comments";
            commItem.Value = "aComments";
            ddlAboutType.Items.Add(commItem);

            ListItem suggItem = new ListItem();
            suggItem.Text = "suggestions";
            suggItem.Value = "aSuggestions";
            ddlAboutType.Items.Add(suggItem);

            ListItem typeSuggItem = new ListItem();
            typeSuggItem.Text = "type suggestions";
            typeSuggItem.Value = "aTypeSuggestions";
            ddlAboutType.Items.Add(typeSuggItem);

            ListItem usrItem = new ListItem();
            usrItem.Text = "users";
            usrItem.Value = "aUsers";
            ddlAboutType.Items.Add(usrItem);

            ddlAboutType.SelectedIndex = 0;

        }

        private void FillDdlAboutType2()
        {
            ListItem prodItem = new ListItem();
            prodItem.Text = "product";
            prodItem.Value = "product";
            ddlAboutType2.Items.Add(prodItem);

            ListItem prodTopicItem = new ListItem();
            prodTopicItem.Text = "product topic";
            prodTopicItem.Value = "productTopic";
            ddlAboutType2.Items.Add(prodTopicItem);

            ListItem prodLink = new ListItem();
            prodLink.Text = "product link";
            prodLink.Value = "productLink";
            ddlAboutType2.Items.Add(prodLink);

            ListItem compItem = new ListItem();
            compItem.Text = "company";
            compItem.Value = "company";
            ddlAboutType2.Items.Add(compItem);

            ListItem catItem = new ListItem();
            catItem.Text = "category";
            catItem.Value = "category";
            ddlAboutType2.Items.Add(catItem);

            ListItem usrItem = new ListItem();
            usrItem.Text = "user";
            usrItem.Value = "user";
            ddlAboutType2.Items.Add(usrItem);

            ListItem commItem = new ListItem();
            commItem.Text = "comment";
            commItem.Value = "comment";
            ddlAboutType2.Items.Add(commItem);

            ListItem suggItem = new ListItem();
            suggItem.Text = "suggestion";
            suggItem.Value = "suggestion";
            ddlAboutType2.Items.Add(suggItem);

            ListItem typeSuggItem = new ListItem();
            typeSuggItem.Text = "type suggestion";
            typeSuggItem.Value = "typeSuggestion";
            ddlAboutType2.Items.Add(typeSuggItem);

            ddlAboutType2.SelectedIndex = 0;

        }



        protected void btnGetNoType_Click(object sender, EventArgs e)
        {
            ph1.Visible = true;
            ph1.Controls.Add(lblError);

            long count = -1;
            if (long.TryParse(tbLastNumReports.Text, out count))
            {
                if (count > 0 && count < 1000)
                {
                    BusinessReport businessReport = new BusinessReport();

                    String viewType = rblViewedMode.SelectedValue;
                    String reportType = ddlTypeReport.SelectedValue;
                    String aboutType = ddlAboutType.SelectedValue;

                    List<Report> Reports = businessReport.GetReports(objectContext, aboutType, 1, reportType, viewType, count);

                    ph1.Visible = false;

                    FillReportsTable(Reports);
                    tblReports.Visible = true;

                    // fills session parameters
                    FillSessionParameters(count, viewType, reportType, aboutType, 1);
                }
                else
                {
                    lblError.Text = "Number must be between 1 and 1000.";
                }
            }
            else
            {
                lblError.Text = "Type number .";
            }
        }

        private void FillSessionParameters(long count, String viewType, String reportType, String aboutType, long aboutTypeID)
        {
            Session["ReportAboutType"] = aboutType;
            Session["ReportType"] = reportType;
            Session["ReportViewType"] = viewType;
            Session["ReportsCount"] = count;
            Session["ReportAboutTypeID"] = aboutTypeID;
        }

        private void FillReportsTable(List<Report> reports)
        {
            tblReports.Rows.Clear();

            BusinessUser businessUser = new BusinessUser();
            BusinessReport businessReport = new BusinessReport();

            Boolean isModerator = businessUser.IsModerator(CurrentUser);

            if (reports.Count<Report>() > 0)
            {
                string contentPHid = pnlSendReplyToReport.ClientID.Substring(0, pnlSendReplyToReport.ClientID.Length - pnlSendReplyToReport.ID.Length);

                User system = businessUser.GetSystem(userContext);

                int i = 0;
                foreach (Report report in reports)
                {
                    if (!report.CreatedByReference.IsLoaded)
                    {
                        report.CreatedByReference.Load();
                    }

                    TableRow tblRow = new TableRow();
                    tblReports.Rows.Add(tblRow);

                    TableCell tblCell = new TableCell();
                    tblRow.Cells.Add(tblCell);

                    Table newTable = new Table();
                    newTable.Width = Unit.Percentage(100);
                    newTable.CellSpacing = 0;

                    newTable.CssClass = "commentsTD";
                    tblCell.Controls.Add(newTable);

                    if (i % 2 == 0)
                    {
                        newTable.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                    }
                    else
                    {
                        newTable.BackColor = CommonCode.UiTools.GetStandardCellBgrColor();
                    }
                    ///

                    TableRow newRow = new TableRow();
                    newTable.Rows.Add(newRow);

                    TableCell mainCell = new TableCell();
                    mainCell.CssClass = "allBorders";
                    mainCell.Width = Unit.Pixel(300);
                    mainCell.VerticalAlign = VerticalAlign.Top;
                    newRow.Cells.Add(mainCell);

                    System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    mainCell.Controls.Add(div);

                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("ID : {0}",report.ID.ToString()), true));
                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText("Written by : ", false));
                    div.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                        (Tools.GetUserFromUserDatabase(userContext, report.CreatedBy)));
                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText("", true));

                    Label lblType = CommonCode.UiTools.GetLabelWithText(string.Format("Type : {0}", report.reportType), true);
                    div.Controls.Add(lblType);
                    lblType.CssClass = "searchPageComments";

                    Label lblDate = CommonCode.UiTools.GetLabelWithText(string.Format("Date : {0}"
                        , CommonCode.UiTools.DateTimeToLocalString(report.dateCreated)), true);
                    div.Controls.Add(lblDate);
                    lblDate.CssClass = "commentsDate";

                    div.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("Viewed : {0}", report.isViewed.ToString()), true));

                    Label lblRes = CommonCode.UiTools.GetLabelWithText(string.Format("Resolved : {0}", report.isResolved.ToString()), true);
                    div.Controls.Add(lblRes);
                    lblRes.CssClass = "searchPageRatings";

                    string aboutDescription = string.Empty;

                    if(report.aboutType == "typeSuggestion")
                    {
                        BusinessTypeSuggestions btSuggestion = new BusinessTypeSuggestions();

                        TypeSuggestion currTypeSuggestion = btSuggestion.GetSuggestion(objectContext, report.aboutTypeId, false, true);

                        div.Controls.Add(CommonCode.UiTools.GetLabelWithText("About : ", false));

                        Label aboutTypeLink = new Label();
                        div.Controls.Add(aboutTypeLink);
                        aboutTypeLink.CssClass = "lblEditors";
                        aboutTypeLink.ID = string.Format("suggAboutReport{0}", report.ID);
                        aboutTypeLink.Text = "Edit suggestion";

                        AjaxControlToolkit.PopupControlExtender extender = new AjaxControlToolkit.PopupControlExtender();
                        extender.ID = string.Format("typeSuggestion{0}rep{1}", currTypeSuggestion.ID, report.ID);
                        extender.TargetControlID = aboutTypeLink.ID;
                        extender.PopupControlID = pnlPopUpTypeSuggestion.ClientID;
                        extender.Position = AjaxControlToolkit.PopupControlPopupPosition.Center;
                        extender.DynamicControlID = lblTypeSuggestion.ClientID;
                        extender.DynamicServiceMethod = "WMGetTypeSuggestion";
                        extender.DynamicContextKey = currTypeSuggestion.ID.ToString();
                        extender.Enabled = true;
                        extender.DynamicServicePath = "";

                        
                        div.Controls.Add(extender);
                    }
                    else
                    {
                        HyperLink hlAboutType = new HyperLink();
                        HyperLink hlAboutParent = new HyperLink();
   
                        if (report.aboutTypeParentId == null)
                        {
                            GetAboutTypeIdAndParentHyperlinks(report, report.aboutType, report.aboutTypeId, 0, out hlAboutType, out hlAboutParent
                                , out aboutDescription);
                        }
                        else
                        {
                            GetAboutTypeIdAndParentHyperlinks(report, report.aboutType, report.aboutTypeId, report.aboutTypeParentId.Value
                                , out hlAboutType, out hlAboutParent, out aboutDescription);
                        }

                        if (!string.IsNullOrEmpty(hlAboutType.NavigateUrl))
                        {
                            div.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("About : {0} , ", report.aboutType), false));
                            div.Controls.Add(hlAboutType);
                        }
                        else
                        {
                            div.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("About : {0}", report.aboutType), false));
                        }

                        if (!string.IsNullOrEmpty(hlAboutParent.NavigateUrl))
                        {
                            div.Controls.Add(CommonCode.UiTools.GetLabelWithText("<br />Parent : ", false));
                            div.Controls.Add(hlAboutParent);
                        }
                    }
                        
                    TableCell descrCell = new TableCell();
                    descrCell.CssClass = "allBorders";
                    descrCell.VerticalAlign = VerticalAlign.Top;
                    descrCell.ColumnSpan = newRow.Cells.Count;

                    switch (report.aboutType)
                    {
                        case "comment":
                            descrCell.Text = string.Format("Comment : {0}<br/><br/>{1}", aboutDescription, Tools.GetFormattedTextFromDB(report.description));
                            break;
                        case "suggestion":
                            descrCell.Text = string.Format("Suggestion : {0}<br/><br/>{1}", aboutDescription, Tools.GetFormattedTextFromDB(report.description));
                            break;
                        case "productLink":
                            descrCell.Text = string.Format("Product link : {0}<br/><br/>{1}", aboutDescription, Tools.GetFormattedTextFromDB(report.description));
                            break;
                        default:
                            descrCell.Text = Tools.GetFormattedTextFromDB(report.description);
                            break;
                    }
                    
                    newRow.Cells.Add(descrCell);

                    List<ReportComment> ReportComments = businessReport.GetReportComments(objectContext, report);
                    if (ReportComments.Count<ReportComment>() > 0)
                    {
                        foreach (ReportComment comment in ReportComments)
                        {
                            if (!comment.UserReference.IsLoaded)
                            {
                                comment.UserReference.Load();
                            }

                            TableRow commRow = new TableRow();
                            newTable.Rows.Add(commRow);

                            TableCell commsCell = new TableCell();
                            commsCell.CssClass = "allBorders";
                            commsCell.VerticalAlign = VerticalAlign.Top;
                            commRow.Cells.Add(commsCell);

                            System.Web.UI.HtmlControls.HtmlGenericControl commDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                            commsCell.Controls.Add(commDiv);

                            commDiv.Controls.Add(CommonCode.UiTools.GetLabelWithText("Reply by : ", false));

                            if (comment.User.ID != system.ID)
                            {
                                commDiv.Controls.Add(CommonCode.UiTools.GetUserHyperLink(Tools.GetUserFromUserDatabase(userContext, comment.User)));
                            }
                            else
                            {
                                commDiv.Controls.Add(CommonCode.UiTools.GetLabelWithText(system.username, false));
                            }
                            commDiv.Controls.Add(CommonCode.UiTools.GetLabelWithText("", true));
                            commDiv.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("Date : {0}"
                                , CommonCode.UiTools.DateTimeToLocalString(comment.dateCreated)), false));


                            TableCell descrCCell = new TableCell();
                            descrCCell.CssClass = "allBorders";
                            descrCCell.VerticalAlign = VerticalAlign.Top;
                            descrCCell.Text = Tools.GetFormattedTextFromDB(comment.description);
                            commRow.Cells.Add(descrCCell);
                        }
                    }

                    TableRow actionsRow = new TableRow();
                    actionsRow.Attributes.Add("id", report.ID.ToString());
                    
                    TableCell actionsCell = new TableCell();
                    actionsCell.CssClass = "allBorders";
                    actionsCell.ColumnSpan = newRow.Cells.Count;
                    actionsRow.Cells.Add(actionsCell);
                    actionsCell.ColumnSpan = newRow.Cells.Count;


                    if (isModerator == true)
                    {
                        if ((report.reportType != "spam") && report.aboutType == "user" && !report.isViewed)
                        {
                            Button mvBtn = new Button();
                            mvBtn.ID = string.Format("MakeViewed{0}", report.ID);
                            mvBtn.Text = "Viewed";
                            mvBtn.Click += new EventHandler(mvBtn_Click);

                            actionsCell.Controls.Add(mvBtn);
                        }

                        if (((report.reportType == "spam") || (report.reportType != "spam" && report.aboutType == "user"))
                            && !report.isResolved)
                        {
                            Button mrBtn = new Button();
                            mrBtn.ID = string.Format("MakeResolved{0}", report.ID);
                            mrBtn.Text = "Resolve";
                            mrBtn.Click += new EventHandler(mrBtn_Click);
                            actionsCell.Controls.Add(mrBtn);
                        }

                        if (report.reportType != "spam" && report.aboutType == "user" && !report.isResolved)
                        {

                            System.Web.UI.HtmlControls.HtmlInputButton replyBtn = new System.Web.UI.HtmlControls.HtmlInputButton("button");
                            actionsCell.Controls.Add(replyBtn);
                            replyBtn.ID = string.Format("ReplyTo{0}", report.ID);
                            replyBtn.Value = "Reply";
                            replyBtn.Attributes.Add("onclick",
                                string.Format("ShowReplyToReportPnl('{0}{1}','{2}','{3}','{4}')"
                                , contentPHid, replyBtn.ClientID, report.ID, pnlSendReplyToReport.ClientID
                                , pnlReplyToReportEnd.ClientID));
                            replyBtn.Attributes.Add("class", "htmlButtonStyle");
                        }

                    }
                    else
                    {
                        if ((report.reportType != "spam") && !report.isViewed)
                        {
                            Button mvBtn = new Button();
                            mvBtn.ID = string.Format("MakeViewed{0}", report.ID);
                            mvBtn.Text = "Viewed";
                            mvBtn.Click += new EventHandler(mvBtn_Click);

                            actionsCell.Controls.Add(mvBtn);
                        }

                        if (!report.isResolved)
                        {

                            Button mrBtn = new Button();
                            mrBtn.ID = string.Format("MakeResolved{0}", report.ID);
                            mrBtn.Text = "Resolve";
                            mrBtn.Click += new EventHandler(mrBtn_Click);
                            actionsCell.Controls.Add(mrBtn);
                        }

                        if ((report.reportType != "spam") && !report.isResolved)
                        {

                            System.Web.UI.HtmlControls.HtmlInputButton replyBtn = new System.Web.UI.HtmlControls.HtmlInputButton("button");
                            actionsCell.Controls.Add(replyBtn);
                            replyBtn.ID = string.Format("ReplyTo{0}", report.ID);
                            replyBtn.Value = "Reply";
                            replyBtn.Attributes.Add("onclick",
                                string.Format("ShowReplyToReportPnl('{0}{1}','{2}','{3}','{4}')"
                                , contentPHid, replyBtn.ClientID, report.ID, pnlSendReplyToReport.ClientID
                                , pnlReplyToReportEnd.ClientID));
                            replyBtn.Attributes.Add("class", "htmlButtonStyle");

                        }

                    }

                    if ((report.reportType == "spam") && !report.isResolved)
                    {

                        Button delCommBtn = new Button();
                        delCommBtn.ID = string.Format("DeleteSpamReport{0}", report.ID);
                        delCommBtn.Click += new EventHandler(delCommBtn_Click);
                        actionsCell.Controls.Add(delCommBtn);

                        switch (report.aboutType)
                        {
                            case "comment":
                                delCommBtn.Text = "Delete comment";
                                break;
                            case "suggestion":
                                delCommBtn.Text = "Delete suggestion";
                                break;
                            case "productLink":
                                delCommBtn.Text = "Delete link";
                                break;
                            default:
                                throw new CommonCode.UIException(string.Format("report about type : {0} not supported for deleting", report.aboutType));
                        }
                    }
                    
                    if (actionsCell.Controls.Count > 0)
                    {
                        newTable.Rows.Add(actionsRow);
                    }

                    i++;
                }
            }
            else
            {
                TableRow noDataRow = new TableRow();
                TableCell noDataCell = new TableCell();
                noDataCell.Text = "No reports.";
                noDataRow.Cells.Add(noDataCell);
                tblReports.Rows.Add(noDataRow);
            }
        }


        private void FillReportPatterns()
        {
            tblPatterns.Rows.Clear();

            BusinessSiteText bSiteText = new BusinessSiteText();

            List<SiteNews> patterns = bSiteText.GetLastTexts(objectContext, int.MaxValue, "report patterns", 1);

            if (patterns.Count > 0)
            {
                pnlShowPatterns.Visible = true;

                TableRow newRow = new TableRow();
                tblPatterns.Rows.Add(newRow);

                int i = 0;

                foreach (SiteNews pattern in patterns)
                {

                    if (i % 2 == 0)
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
                        , pattern.ID, "report pattern", "tbReplyDescription"));

                    i++;
                }
            }
            else
            {
                pnlShowPatterns.Visible = false;
            }
        }


        void replyToReport_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            BusinessReport businessReport = new BusinessReport();

            Button btnReply = sender as Button;
            if (btnReply != null)
            {
                TableCell tblCell = btnReply.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long repID = -1;
                        string repIdStr = tblRow.Attributes["id"];
                        if (long.TryParse(repIdStr, out repID))
                        {
                            Report currReport = businessReport.Get(objectContext, repID);
                            if (currReport == null)
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("Theres no report ID = {0} (comming from tblRow.Attributes['id']) , user id = {1}"
                                    , repID, CurrentUser.ID));
                            }

                            pnlReplyToReport.Visible = true;
                            if (!currReport.CreatedByReference.IsLoaded)
                            {
                                currReport.CreatedByReference.Load();
                            }
                            lblReportInfo.Text = string.Format("Reply to {0}`s report about {1} ID {2} written on {3} report type is {4}",
                                businessUser.GetUserName(userContext, currReport.CreatedBy.ID), currReport.aboutType,
                                CommonCode.UiTools.DateTimeToLocalString(currReport.dateCreated), currReport.ID, currReport.reportType);

                            tbReportDescription.Text = currReport.description;

                            hfReplyToReport.Value = currReport.ID.ToString();
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['id'] = {0} to long , user id = {1}", repIdStr, CurrentUser.ID));
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
                throw new CommonCode.UIException("coultn get Button");
            }
        }

        public void mrBtn_Click(object sender, EventArgs e)
        {
            Button btnResolve = sender as Button;
            if (btnResolve != null)
            {
                TableCell tblCell = btnResolve.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long repID = -1;
                        string repIdStr = tblRow.Attributes["id"];
                        if (long.TryParse(repIdStr, out repID))
                        {
                            BusinessReport businessReport = new BusinessReport();

                            Report currReport = businessReport.Get(objectContext, repID);
                            if (currReport == null)
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("Theres ni report ID = {0} (comming from tblRow.Attributes['id']) , user id = {1}"
                                    , repIdStr, CurrentUser.ID));
                            }
                            businessReport.ReportIsResolved(objectContext, userContext, currReport, businessLog, true, CurrentUser, string.Empty);

                            if (currReport.reportType == "spam")
                            {
                                businessReport.ResolveAllSpamReportsForSameType(objectContext, userContext, currReport, businessLog, CurrentUser);
                            }

                            ShowReportsTableFromSessionParams();
                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Report resolved!");
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['id'] to long , user id = {0}",CurrentUser.ID));
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

        void mvBtn_Click(object sender, EventArgs e)
        {
            Button btnViewed = sender as Button;
            if (btnViewed != null)
            {
                TableCell tblCell = btnViewed.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long repID = -1;
                        string repIdStr = tblRow.Attributes["id"];
                        if (long.TryParse(repIdStr, out repID))
                        {
                            BusinessReport businessReport = new BusinessReport();

                            Report currReport = businessReport.Get(objectContext, repID);
                            if (currReport == null)
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("Theres ni report ID = {0} (comming from tblRow.Attributes['id'] , user id = {1})"
                                    , repIdStr, CurrentUser.ID));
                            }
                            businessReport.ReportIsViewed(objectContext, currReport, businessLog, CurrentUser);

                            ShowReportsTableFromSessionParams();
                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Report is now viewed!");
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['id'] = {0} to long , user id = {1}", repIdStr, CurrentUser.ID));
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

        public void GetAboutTypeIdAndParentHyperlinks(Report currReport, String aboutType, long aboutTypeId, long parentId, out HyperLink aboutTypeLink
            , out HyperLink parentLink, out string description)
        {

            aboutTypeLink = new HyperLink();
            parentLink = new HyperLink();
            description = string.Empty;

            BusinessUser businessUser = new BusinessUser();

            switch (aboutType)
            {
                case("product") :
                    BusinessProduct businessProduct = new BusinessProduct();

                    Product currProd = businessProduct.GetProductByIDWV(objectContext, aboutTypeId);
                    if (currProd == null)
                    {
                        throw new CommonCode.UIException(string.Format("Theres no product ID = {0} , user id = {1}",aboutTypeId, CurrentUser.ID));
                    }
                    else
                    {
                        aboutTypeLink = CommonCode.UiTools.GetProductHyperLink(currProd);

                        if (!currProd.CreatedByReference.IsLoaded)
                        {
                            currProd.CreatedByReference.Load();
                        }
                        parentLink = CommonCode.UiTools.GetUserHyperLink(
                            Tools.GetUserFromUserDatabase(userContext, currProd.CreatedBy));
                    }
                    break;
                case("company") :
                    BusinessCompany businessCompany = new BusinessCompany();

                    Company currCompany = businessCompany.GetCompanyWV(objectContext, aboutTypeId);
                    if (currCompany == null)
                    {
                        throw new CommonCode.UIException(string.Format("Theres no company ID = {0} , user id = {1}", aboutTypeId, CurrentUser.ID));
                    }
                    else
                    {
                        if (businessCompany.IsOther(objectContext, currCompany.ID))
                        {
                            aboutTypeLink.Text = currCompany.name;
                        }
                        else
                        {
                            aboutTypeLink = CommonCode.UiTools.GetCompanyHyperLink(currCompany);
                        }

                        if (!currCompany.CreatedByReference.IsLoaded)
                        {
                            currCompany.CreatedByReference.Load();
                        }

                        parentLink = CommonCode.UiTools.GetUserHyperLink
                            (Tools.GetUserFromUserDatabase(userContext, currCompany.CreatedBy));
                    }
                    break;
                case("category") :
                    BusinessCategory businessCategory = new BusinessCategory();

                    Category currCategory = businessCategory.GetWithoutVisible(objectContext, aboutTypeId);
                    if (currCategory == null)
                    {
                        throw new BusinessException(string.Format("Theres no category ID = {0} , user id = {1}", aboutTypeId, CurrentUser.ID));
                    }
                    else
                    {
                        aboutTypeLink = CommonCode.UiTools.GetCategoryHyperLink(currCategory);

                        if (!currCategory.CreatedByReference.IsLoaded)
                        {
                            currCategory.CreatedByReference.Load();
                        }

                        parentLink = CommonCode.UiTools.GetUserHyperLink
                            (Tools.GetUserFromUserDatabase(userContext, currCategory.CreatedBy));
                    }
                    break;
                case("user") :
                    User aboutUser = businessUser.GetWithoutVisible(userContext, aboutTypeId, true);
                   
                    parentLink.Text = "none";

                    if (businessUser.IsUserValidType(userContext, aboutUser.ID))
                    {
                        aboutTypeLink = CommonCode.UiTools.GetUserHyperLink(aboutUser);
                    }
                    else
                    {
                        aboutTypeLink.Text = aboutUser.username;
                    }
                    
                    break;
                case("comment") :
                    BusinessComment businessComment = new BusinessComment();

                    Comment currComment = businessComment.GetWithoutVisible(objectContext, aboutTypeId);
                    if (currComment == null)
                    {
                        throw new CommonCode.UIException(string.Format("Theres no comment ID = {0} , user id = {1}", aboutTypeId, CurrentUser.ID));
                    }
                    else
                    {
                        description = currComment.description;
                        aboutTypeLink.Text = "none";

                        if (parentId > 0)
                        {
                            User commentUser = businessUser.GetWithoutVisible(userContext, parentId, true);
                           
                            if (businessUser.IsUserValidType(commentUser))
                            {
                                parentLink = CommonCode.UiTools.GetUserHyperLink(commentUser);
                            }
                            else
                            {
                                parentLink.Text = "none";
                            }
                            
                            
                        }
                        else
                        {
                            throw new CommonCode.UIException("parentId is < 1");
                        }
                    }
                    break;
                case ("suggestion"):
                    BusinessSuggestion businessSuggestion = new BusinessSuggestion();

                    Suggestion currSuggestion = businessSuggestion.GetWithoutVisible(objectContext, aboutTypeId);
                    if (currSuggestion == null)
                    {
                        throw new CommonCode.UIException(string.Format("Theres no suggestion ID = {0} , user id = {1}", aboutTypeId, CurrentUser.ID));
                    }
                    else
                    {
                        description = currSuggestion.description;
                        aboutTypeLink.Text = "none";

                        if (parentId > 0)
                        {
                            User suggestionUser = businessUser.GetWithoutVisible(userContext, parentId, true);
                            
                            if (businessUser.IsUserValidType(suggestionUser))
                            {
                                parentLink = CommonCode.UiTools.GetUserHyperLink(suggestionUser);
                            }
                            else
                            {
                                parentLink.Text = "none";
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException("parentId is < 1");
                        }
                    }
                    break;
                case "productTopic":

                    BusinessProductTopics bpTopic = new BusinessProductTopics();

                    ProductTopic topic = bpTopic.Get(objectContext, aboutTypeId, false, true);

                    aboutTypeLink.Text = topic.name;
                    aboutTypeLink.NavigateUrl = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", topic.ID));

                    if (!topic.UserReference.IsLoaded)
                    {
                        topic.UserReference.Load();
                    }

                    parentLink = CommonCode.UiTools.GetUserHyperLink(
                        Tools.GetUserFromUserDatabase(userContext, topic.User));
                    
                    break;
                case "productLink":

                    BusinessProductLink bpLink = new BusinessProductLink();

                    ProductLink link = bpLink.Get(objectContext, aboutTypeId, false, true);

                    aboutTypeLink.Text = "Product link";

                    if (!link.ProductReference.IsLoaded)
                    {
                        link.ProductReference.Load();
                    }

                    aboutTypeLink.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", link.Product.ID));

                    if (!link.UserReference.IsLoaded)
                    {
                        link.UserReference.Load();
                    }

                    parentLink = CommonCode.UiTools.GetUserHyperLink(
                        Tools.GetUserFromUserDatabase(userContext, link.User));

                    description = string.Format("{0}<br /> Description : {1}", link.link, link.description);

                    break;
                default :
                        parentLink.Text = "none";
                        aboutTypeLink.Text = "none";
                    break;
            }
            
        }

        private TableCell GetCreatedByCell(BusinessUser businessUser, Report report)
        {
            TableCell byCell = new TableCell();
            if (!report.CreatedByReference.IsLoaded)
            {
                report.CreatedByReference.Load();
            }
            User byUser = Tools.GetUserFromUserDatabase(userContext, report.CreatedBy);
            if (businessUser.IsUserValidType(userContext, byUser.ID))
            {
                byCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink(byUser));
            }
            else
            {
                byCell.Text = byUser.username;
            }
       
            return byCell;
        }

        private static TableRow GetReportsTableFirstRow(Boolean canEdit)
        {
            TableRow tblRow = new TableRow();
            TableCell tblCell = new TableCell();
           
            tblRow.Cells.Add(tblCell);

            Table newTable = new Table();
            newTable.CssClass = "commentsTD";

            tblCell.Controls.Add(newTable);

            TableRow firstRow = new TableRow();
            newTable.Rows.Add(firstRow);

            TableCell idFCell = new TableCell();
            idFCell.Text = "id";
            firstRow.Cells.Add(idFCell);

            TableCell byFCell = new TableCell();
            byFCell.Text = "Written by";
            firstRow.Cells.Add(byFCell);

            TableCell typeFCell = new TableCell();
            typeFCell.Text = "Report type";
            firstRow.Cells.Add(typeFCell);

            TableCell dateFCell = new TableCell();
            dateFCell.Text = "date created";
            firstRow.Cells.Add(dateFCell);

            TableCell vFCell = new TableCell();
            vFCell.Text = "is viewed";
            firstRow.Cells.Add(vFCell);

            TableCell rFCell = new TableCell();
            rFCell.Text = "is resolved";
            firstRow.Cells.Add(rFCell);

            TableCell aboutFCell = new TableCell();
            aboutFCell.Text = "about";
            firstRow.Cells.Add(aboutFCell);

            TableCell aboutNFCell = new TableCell();
            aboutNFCell.Text = "name";
            firstRow.Cells.Add(aboutNFCell);

            TableCell parentFCell = new TableCell();
            parentFCell.Text = "parent";
            firstRow.Cells.Add(parentFCell);

            if (canEdit)
            {
                TableCell makeVieCell = new TableCell();
                makeVieCell.Text = "make viewed";
                firstRow.Cells.Add(makeVieCell);

                TableCell makeResCell = new TableCell();
                makeResCell.Text = "resolve";
                firstRow.Cells.Add(makeResCell);
            }

            TableCell replyCell = new TableCell();
            replyCell.Text = "reply";
            firstRow.Cells.Add(replyCell);

            return tblRow;
        }

        protected void btnReplyToReport_Click(object sender, EventArgs e)
        {
            phLblErrorReply.Visible = true;
            phLblErrorReply.Controls.Add(lblError);
            string error = "";

            BusinessReport businessReport = new BusinessReport();
            long repID = -1;
            if (long.TryParse(hfReplyToReport.Value, out repID))
            {
                Report currReport = businessReport.Get(objectContext, repID);

                if (currReport != null)
                {
                    string description = tbReportReply.Text;

                    if (CommonCode.Validate.ValidateDescription(ref description, out error))
                    {
                        businessReport.CreateReportComment(objectContext, userContext, CurrentUser, currReport, description, true, businessLog);

                        phLblErrorReply.Visible = false;
                        pnlReplyToReport.Visible = false;

                        hfReplyToReport.Value = "";

                        ShowReportsTableFromSessionParams();
                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Reply to report sent!");
                        pnlReplyToReport.Visible = false;
                    }
                }
                else
                {
                    throw new CommonCode.UIException(string.Format("Theres no report ID = {0} , user id = {1}", repID, CurrentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("Couldnt parse hfReplyToReport.Value = {0} to long , user id = {1}"
                    , hfReplyToReport.Value, CurrentUser.ID));
            }

            lblError.Text = error;
        }

        private void HideLabels()
        {
            ph1.Visible = false;
            phLblErrorReply.Visible = false;
        }

        protected void btnGetReportsAboutType_Click(object sender, EventArgs e)
        {
            ph2.Visible = true;
            ph2.Controls.Add(lblError);

            long count = -1;
            if (long.TryParse(tbLastNumReports2.Text, out count))
            {
                if (count > 0 && count < 1000)
                {
                    long id = -1;
                    if (long.TryParse(tbAboutTypeId.Text, out id))
                    {
                        if (id > 0)
                        {
                            BusinessReport businessReport = new BusinessReport();

                            String viewType = rblViewedMode2.SelectedValue;
                            String reportType = ddlTypeReport2.SelectedValue;
                            String aboutType = ddlAboutType2.SelectedValue;

                            List<Report> Reports = businessReport.GetReports(objectContext, aboutType, id, reportType, viewType, count);

                            ph2.Visible = false;

                            FillReportsTable(Reports);
                            tblReports.Visible = true;

                            FillSessionParameters(count, viewType, reportType, aboutType, id);
                        }
                        else
                        {
                            lblError.Text = "ID must be positive.";
                        }
                    }
                    else
                    {
                        lblError.Text = "Type ID.";
                    }
                }
                else
                {
                    lblError.Text = "Number must be between 1 and 1000.";
                }
            }
            else
            {
                lblError.Text = "Type number .";
            }
        }

        protected void btnCancelReply_Click(object sender, EventArgs e)
        {
            pnlReplyToReport.Visible = false;
        }

        protected void delCommBtn_Click(object sender, EventArgs e)
        {

            Button btnDelComm = sender as Button;
            if (btnDelComm != null)
            {
                TableCell tblCell = btnDelComm.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long repID = -1;
                        string repIdStr = tblRow.Attributes["id"];
                        if (long.TryParse(repIdStr, out repID))
                        {
                            BusinessReport businessReport = new BusinessReport();

                            Report currReport = businessReport.Get(objectContext, repID);
                            if (currReport == null)
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("Theres no report ID = {0} (comming from tblRow.Attributes['id'] , user id = {1})"
                                    , repIdStr, CurrentUser.ID));
                            }

                            switch (currReport.aboutType)
                            {
                                case "comment":
                                    BusinessComment businessComment = new BusinessComment();

                                    Comment comment = businessComment.GetWithoutVisible(objectContext, currReport.aboutTypeId);
                                    if (comment == null)
                                    {
                                        throw new CommonCode.UIException(string.Format
                                            ("Theres no comment ID = {0} which is being reported as spam, report id = {1}, user id = {2}",
                                            currReport.aboutTypeId, currReport.ID, CurrentUser.ID));
                                    }
                                    businessComment.DeleteComment(objectContext, userContext, comment, CurrentUser, businessLog, true, true);
                                    businessReport.ReportIsResolved(objectContext, userContext, currReport, businessLog, true, CurrentUser, string.Empty);
                                    businessReport.ResolveAllSpamReportsForSameType(objectContext, userContext, currReport, businessLog, CurrentUser);

                                    ShowReportsTableFromSessionParams();
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Comment deleted and report is resolved.");
                                    break;
                                case "suggestion":
                                    BusinessSuggestion businessSuggestion = new BusinessSuggestion();

                                    Suggestion suggestion = businessSuggestion.GetWithoutVisible(objectContext, currReport.aboutTypeId);
                                    if (suggestion == null)
                                    {
                                        throw new CommonCode.UIException(string.Format
                                            ("Theres no suggestion ID = {0} which is being reported as spam, report id = {1}, user id = {2}",
                                            currReport.aboutTypeId, currReport.ID, CurrentUser.ID));
                                    }

                                    businessSuggestion.DeleteSuggestion(objectContext, userContext, suggestion, CurrentUser, businessLog, true);
                                    businessReport.ReportIsResolved(objectContext, userContext, currReport, businessLog, true, CurrentUser, string.Empty);
                                    businessReport.ResolveAllSpamReportsForSameType(objectContext, userContext, currReport, businessLog, CurrentUser);
                                    ShowReportsTableFromSessionParams();
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Suggestion deleted and report is resolved.");
                                    break;
                                case "productLink":
                                    BusinessProductLink bpLink = new BusinessProductLink();

                                    ProductLink link = bpLink.Get(objectContext, currReport.aboutTypeId, false, true);

                                    bpLink.DeleteLink(objectContext, userContext, businessLog, link, CurrentUser, true);
                                    businessReport.ReportIsResolved(objectContext, userContext, currReport, businessLog, true, CurrentUser, string.Empty);
                                    businessReport.ResolveAllSpamReportsForSameType(objectContext, userContext, currReport, businessLog, CurrentUser);

                                    ShowReportsTableFromSessionParams();
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Product link deleted and report is resolved.");

                                    break;
                                default:
                                    throw new CommonCode.UIException(string.Format("report about type : {0} not supported for deleting", currReport.aboutType));
                            }

                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['id'] = {0} to long , user id = {1}", repIdStr, CurrentUser.ID));
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


        [WebMethod]
        public static string WMSendReplyToReport(string id, string description)
        {
            return CommonCode.WebMethods.SendReplyToReport(id, description);
        }

        [WebMethod]
        public static string WMGetTypeSuggestion(string contextKey)
        {
            return CommonCode.WebMethods.GetTypeSuggestion(contextKey); 
        }

        [WebMethod]
        public static string WMGetSiteText(string Id, string textType)
        {
            return CommonCode.WebMethods.GetSiteText(Id, textType);
        }

    }
}
