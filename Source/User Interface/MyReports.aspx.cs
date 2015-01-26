﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Globalization;

using BusinessLayer;
using DataAccess;
using CustomServerControls;

namespace UserInterface
{
    public partial class MyReports : BasePage
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
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckUserAndParams();
            SetNeedsToBeLogged();

            ShowInfo();
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);
        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            BusinessReport businessReport = new BusinessReport();

            int numRemRep = businessReport.NumberOfReportsWhichUserCanSend(objectContext, currentUser);
            lblRemIrrReports.Text = string.Format("{0} {1}", GetLocalResourceObject("RemIrrReports"), numRemRep);

            int numRemSpam = businessReport.NumberOfSpamReportsWhichUserCanSend(objectContext, currentUser);
            lblRemSpamReports.Text = string.Format("{0} {1}", GetLocalResourceObject("RemSpamReports"), numRemSpam);

            FillReports();
            CheckIfUserCanSendReports();

            BusinessUserOptions bUserOptions = new BusinessUserOptions();
            bUserOptions.ChangeIfUserHaveNewReportReply(userContext, currentUser, false);

            SetLocalText();
        }

        private void SetLocalText()
        {
            lblReport.Text = GetGlobalResourceObject("SiteResources", "reportIrregularity").ToString();
            btnSubmitReport.Text = GetGlobalResourceObject("SiteResources", "Send").ToString();
            lblReplyToReport.Text = GetLocalResourceObject("ReplyToReport").ToString();
            btnSendReply.Value = GetGlobalResourceObject("SiteResources", "Send").ToString();
            btnCancelSendReply.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();
        }

        private void CheckIfUserCanSendReports()
        {
            BusinessUser businessUser = new BusinessUser();
            BusinessReport businessReport = new BusinessReport();

            if (businessUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate) == true
                   && !businessReport.IsMaxActiveIrregularityReportsReached(objectContext, currentUser))
            {
                pnlReport.Visible = true;

                if (string.IsNullOrEmpty(lblAboutReporting.Text))
                {
                    BusinessSiteText businessText = new BusinessSiteText();
                    SiteNews aboutReporting = businessText.GetSiteText(objectContext, "aboutGeneralReporting");
                    if (aboutReporting != null)
                    {
                        lblAboutReporting.Text = aboutReporting.description;
                    }
                    else
                    {
                        lblAboutReporting.Text = "aboutGeneralReporting text not typed.";
                    }
                }
            }
            else
            {
                pnlReport.Visible = false;
            }

        }

        private void FillReports()
        {
            phReports.Controls.Clear();

            Panel paddPnl = new Panel();
            phReports.Controls.Add(paddPnl);
            paddPnl.CssClass = "paddingLR4";

            BusinessReport businessReport = new BusinessReport();

            List<Report> reports = businessReport.GetUserReports(objectContext, currentUser, false, false).ToList();

            int count = reports.Count<Report>();
            int i = 0;

            if (count > 0)
            {
                reports.Reverse();
                BusinessUser businessUser = new BusinessUser();

                Panel sentReports = new Panel();
                paddPnl.Controls.Add(sentReports);
                sentReports.CssClass = "sectionTextHeader";
                sentReports.Controls.Add(CommonCode.UiTools.GetLabelWithText
                    (GetLocalResourceObject("Reports").ToString(), false));
                sentReports.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginBottom, "10px");

                foreach (Report report in reports)
                {
                    Panel newPanel = new Panel();
                    paddPnl.Controls.Add(newPanel);

                    newPanel.CssClass = "panelRows";

                    Table repTbl = new Table();
                    repTbl.Width = Unit.Percentage(100);
                    newPanel.Controls.Add(repTbl);
                    ///

                    TableRow newRow = new TableRow();
                    repTbl.Rows.Add(newRow);
                    newRow.Attributes.Add("id", report.ID.ToString());

                    TableCell dateCell = new TableCell();
                    dateCell.Width = Unit.Pixel(180);
                    dateCell.CssClass = "commentsDate";
                    dateCell.Text = report.dateCreated.ToString("G", DateTimeFormatInfo.InvariantInfo);
                    dateCell.VerticalAlign = VerticalAlign.Top;
                    newRow.Cells.Add(dateCell);

                    TableCell aboutCell = new TableCell();
                    aboutCell.HorizontalAlign = HorizontalAlign.Left;
                    aboutCell.VerticalAlign = VerticalAlign.Top;

                    Label repAboutLbl = new Label();
                    if (report.aboutType != "typeSuggestion" && report.aboutType != "general")
                    {
                        repAboutLbl.Text = string.Format("{0} : ", BusinessReport.GetReportAboutType(report));
                    }
                    else
                    {
                        repAboutLbl.Text = BusinessReport.GetReportAboutType(report);
                    }
                    aboutCell.Controls.Add(repAboutLbl);
                    aboutCell.Controls.Add(GetAboutTypeControl(report.aboutType, report.aboutTypeId, report));
                    newRow.Cells.Add(aboutCell);

                    TableCell resCell = new TableCell();
                    resCell.VerticalAlign = VerticalAlign.Top;

                    if (report.isResolved == true)
                    {
                        resCell.Width = Unit.Pixel(200);
                    }
                    else
                    {
                        resCell.Width = Unit.Pixel(300);
                    }


                    resCell.HorizontalAlign = HorizontalAlign.Right;
                    resCell.CssClass = "searchPageRatings";
                    if (report.isResolved)
                    {
                        resCell.Text = string.Format("{0} &nbsp;", GetLocalResourceObject("Resolved"));
                    }
                    else
                    {
                        resCell.Attributes.Add("repID", report.ID.ToString());

                        resCell.Controls.Add(CommonCode.UiTools.GetLabelWithText
                            (string.Format("{0} &nbsp;", GetLocalResourceObject("NotResolved")), false));

                        DecoratedButton resolveBtn = new DecoratedButton();
                        resolveBtn.ID = string.Format("resolveReport{0}", report.ID);
                        resolveBtn.Text = GetLocalResourceObject("Resolve").ToString();
                        resolveBtn.Click += new EventHandler(resolveReport_Click);
                        resCell.Controls.Add(resolveBtn);
                    }
                    newRow.Cells.Add(resCell);


                    TableCell delCell = new TableCell();
                    delCell.HorizontalAlign = HorizontalAlign.Right;
                    delCell.VerticalAlign = VerticalAlign.Top;
                    delCell.Width = Unit.Pixel(1);

                    DecoratedButton delBtn = new DecoratedButton();
                    delBtn.ID = string.Format("deleteReport{0}", report.ID);
                    delBtn.Text = GetLocalResourceObject("Delete").ToString();
                    delBtn.Click += new EventHandler(deleteReport_Click);
                    delCell.Controls.Add(delBtn);
                    newRow.Cells.Add(delCell);


                    Label lblDescr = new Label();
                    newPanel.Controls.Add(lblDescr);
                    lblDescr.Text = Tools.GetFormattedTextFromDB(report.description);

                    List<ReportComment> ReportComments = businessReport.GetReportComments(objectContext, report);

                    count = ReportComments.Count<ReportComment>();

                    if (count > 0)
                    {
                        Panel commPanel = new Panel();
                        newPanel.Controls.Add(commPanel);

                        commPanel.CssClass = "reportComments";

                        User commentUser = null;

                        i = 0;

                        foreach (ReportComment comment in ReportComments)
                        {

                            i++;

                            Table rcTable = new Table();
                            commPanel.Controls.Add(rcTable);
                            rcTable.Width = Unit.Percentage(100);

                            TableRow commRow = new TableRow();
                            rcTable.Rows.Add(commRow);

                            TableCell byCell = new TableCell();
                            commRow.Cells.Add(byCell);
                            byCell.Width = Unit.Pixel(220);
                            byCell.CssClass = "userNames";
                            if (!comment.UserReference.IsLoaded)
                            {
                                comment.UserReference.Load();
                            }

                            commentUser = Tools.GetUserFromUserDatabase(userContext, comment.User);

                            if (businessUser.IsFromAdminTeam(commentUser))
                            {
                                byCell.Controls.Add(CommonCode.UiTools.GetAdminLabel(commentUser.username));
                            }
                            else
                            {
                                byCell.Text = commentUser.username;
                            }

                            TableCell dateCCell = new TableCell();
                            dateCCell.CssClass = "commentsDate";
                            dateCCell.Text = CommonCode.UiTools.DateTimeToLocalString(comment.dateCreated);
                            commRow.Cells.Add(dateCCell);

                            Label commDescrLbl = new Label();
                            commDescrLbl.Text = Tools.GetFormattedTextFromDB(comment.description);
                            commPanel.Controls.Add(commDescrLbl);

                            if (i < count)
                            {
                                commPanel.Controls.Add(CommonCode.UiTools.GetHorisontalFashionLinePanel(true));
                            }
                        }

                        if (report.isResolved == false && ReportComments.Count<ReportComment>() > 0)
                        {
                            Panel replyPanel = new Panel();
                            commPanel.Controls.Add(replyPanel);
                            replyPanel.Attributes.Add("id", report.ID.ToString());
                            replyPanel.HorizontalAlign = HorizontalAlign.Center;

                            System.Web.UI.HtmlControls.HtmlInputButton replyBtn = new System.Web.UI.HtmlControls.HtmlInputButton("button");
                            replyPanel.Controls.Add(replyBtn);
                            replyBtn.ID = string.Format("replyToReport{0}", report.ID);
                            replyBtn.Value = GetLocalResourceObject("Reply").ToString();
                            replyBtn.Attributes.Add("onclick",
                                string.Format("ShowReplyToReportPnl('{0}','{1}','{2}','{3}')"
                                , replyBtn.ClientID, report.ID, pnlSendReplyToReport.ClientID
                                , pnlReplyToReportEnd.ClientID));
                            replyBtn.Attributes.Add("class", "htmlButtonStyle");
                        }

                    }
                }

            }
            else
            {
                Panel sentReports = new Panel();
                paddPnl.Controls.Add(sentReports);
                sentReports.CssClass = "sectionTextHeader";
                sentReports.Controls.Add(CommonCode.UiTools.GetLabelWithText
                    (GetLocalResourceObject("NoPostedReports").ToString(), false));
            }
        }


        void deleteReport_Click(object sender, EventArgs e)
        {
            Button btnDelete = sender as Button;
            if (btnDelete != null)
            {
                TableCell tblCell = btnDelete.Parent as TableCell;
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
                                throw new CommonCode.UIException(string.Format("Theres no report ID = {0} , user id = {1}", repID, currentUser.ID));
                            }

                            businessReport.ReportIsDeletedByUser(objectContext, userContext, currReport, businessLog, currentUser);

                            ShowInfo();

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                , GetLocalResourceObject("ReportDeleted").ToString());
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['id'] to long , user id = {1}", repIdStr, currentUser.ID));
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

        void resolveReport_Click(object sender, EventArgs e)
        {
            Button btnResolve = sender as Button;
            if (btnResolve != null)
            {
                TableCell tblCell = btnResolve.Parent as TableCell;
                if (tblCell != null)
                {

                    long repID = -1;
                    string repIdStr = tblCell.Attributes["repID"];
                    if (long.TryParse(repIdStr, out repID))
                    {
                        BusinessReport businessReport = new BusinessReport();

                        Report currReport = businessReport.Get(objectContext, repID);
                        if (currReport == null)
                        {
                            throw new CommonCode.UIException(string.Format("Theres no report ID = {0} , user id = {1}", repID, currentUser.ID));
                        }

                        businessReport.ReportIsResolved(objectContext, userContext, currReport, businessLog, false, currentUser, string.Empty);

                        ShowInfo();

                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                            , GetLocalResourceObject("ReportResolved").ToString());
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format
                            ("Couldnt parse tblCell.Attributes['repID'] to long , user id = {1}", repIdStr, currentUser.ID));
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


        /// <summary>
        /// Returns PlaceHolder containing text or link about the type for which is the report
        /// </summary>
        public PlaceHolder GetAboutTypeControl(String aboutType, long aboutTypeId, Report currReport)
        {
            if (currReport == null)
            {
                throw new CommonCode.UIException("currReport is null");
            }

            BusinessUser businessUser = new BusinessUser();

            PlaceHolder somePh = new PlaceHolder();
            Label aboutTypeLbl = new Label();

            string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);

            switch (aboutType)
            {
                case ("product"):
                    BusinessProduct businessProduct = new BusinessProduct();

                    Product currProd = businessProduct.GetProductByIDWV(objectContext, aboutTypeId);
                    if (currProd == null)
                    {
                        throw new CommonCode.UIException(string.Format("Theres no product id = {0} , user id = {1}", aboutTypeId, currentUser.ID));
                    }
                    else
                    {
                        if (currProd.visible == true)
                        {
                            HyperLink productLink = CommonCode.UiTools.GetProductHyperLink(currProd);
                            somePh.Controls.Add(productLink);

                            productLink.ID = string.Format("rep{0}prod{1}", currReport.ID, currProd.ID);
                            productLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}{2}','{3}')", currProd.ID, contPageId, productLink.ClientID, pnlPopUp.ClientID));
                            productLink.Attributes.Add("onmouseout", "HideData()");
                        }
                        else
                        {
                            aboutTypeLbl.Text = currProd.name;
                            somePh.Controls.Add(aboutTypeLbl);
                        }
                    }
                    break;
                case ("company"):
                    BusinessCompany businessCompany = new BusinessCompany();

                    Company currCompany = businessCompany.GetCompanyWV(objectContext, aboutTypeId);
                    if (currCompany == null)
                    {
                        throw new CommonCode.UIException(string.Format("Theres no Company id = {0} , user id = {1}", aboutTypeId, currentUser.ID));
                    }
                    else
                    {
                        if (businessCompany.IsOther(objectContext, currCompany.ID) || currCompany.visible == false)
                        {
                            aboutTypeLbl.Text = currCompany.name;
                            somePh.Controls.Add(aboutTypeLbl);
                        }
                        else
                        {
                            HyperLink compLink = CommonCode.UiTools.GetCompanyHyperLink(currCompany);
                            somePh.Controls.Add(compLink);

                            compLink.ID = string.Format("rep{0}comp{1}", currReport.ID, currCompany.ID);
                            compLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}{2}','{3}')", currCompany.ID, contPageId, compLink.ClientID, pnlPopUp.ClientID));
                            compLink.Attributes.Add("onmouseout", "HideData()");
                        }
                    }
                    break;
                case ("category"):
                    BusinessCategory businessCategory = new BusinessCategory();

                    Category currCategory = businessCategory.GetWithoutVisible(objectContext, aboutTypeId);
                    if (currCategory == null)
                    {
                        throw new BusinessException(string.Format("Theres no category id = {0} , user id = {1}", aboutTypeId, currentUser.ID));
                    }

                    if (currCategory.visible == true)
                    {
                        somePh.Controls.Add(CommonCode.UiTools.GetCategoryNameWithLink(currCategory, objectContext, true, false, false));
                    }
                    else
                    {
                        aboutTypeLbl.Text = Tools.CategoryName(objectContext, currCategory, true);
                        somePh.Controls.Add(aboutTypeLbl);
                    }

                    break;
                case ("user"):
                    User aboutUser = businessUser.GetWithoutVisible(userContext, aboutTypeId, true);

                    if (businessUser.IsUserValidType(userContext, aboutUser.ID) && aboutUser.visible == true)
                    {
                        somePh.Controls.Add(CommonCode.UiTools.GetUserHyperLink(aboutUser));
                    }
                    else
                    {
                        aboutTypeLbl.Text = aboutUser.username;
                        somePh.Controls.Add(aboutTypeLbl);
                    }

                    break;
                case ("typeSuggestion"):

                    BusinessTypeSuggestions btSuggestion = new BusinessTypeSuggestions();

                    TypeSuggestion currTypeSuggestion = btSuggestion.GetSuggestion(objectContext, currReport.aboutTypeId, false, true);

                    somePh.Controls.Add(aboutTypeLbl);

                    aboutTypeLbl.CssClass = "lblEditors";
                    aboutTypeLbl.ID = string.Format("suggAboutReport{0}", currReport.ID);
                    aboutTypeLbl.Text = GetLocalResourceObject("EditSuggestion").ToString();

                    AjaxControlToolkit.PopupControlExtender extender = new AjaxControlToolkit.PopupControlExtender();
                    extender.ID = string.Format("typeSuggestion{0}rep{1}", currTypeSuggestion.ID, currReport.ID);
                    extender.TargetControlID = aboutTypeLbl.ID;
                    extender.PopupControlID = pnlPopUpTypeSuggestion.ClientID;
                    extender.Position = AjaxControlToolkit.PopupControlPopupPosition.Center;
                    extender.DynamicControlID = lblTypeSuggestion.ClientID;
                    extender.DynamicServiceMethod = "WMGetTypeSuggestion";
                    extender.DynamicContextKey = currTypeSuggestion.ID.ToString();
                    extender.Enabled = true;
                    extender.DynamicServicePath = "";

                    somePh.Controls.Add(extender);

                    break;

                case ("productTopic"):

                    BusinessProductTopics bpTopic = new BusinessProductTopics();

                    ProductTopic topic = bpTopic.Get(objectContext, aboutTypeId, false, true);

                    if (topic.visible == true)
                    {
                        HyperLink topicLink = new HyperLink();
                        somePh.Controls.Add(topicLink);

                        topicLink.Text = topic.name;
                        topicLink.NavigateUrl = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", topic.ID));
                    }
                    else
                    {
                        aboutTypeLbl.Text = topic.name;
                        somePh.Controls.Add(aboutTypeLbl);
                    }

                    break;

                default:
                    aboutTypeLbl.Text = string.Empty;
                    somePh.Controls.Add(aboutTypeLbl);
                    break;

            }

            return somePh;
        }


        private void CheckUserAndParams()
        {
            BusinessUser businessUser = new BusinessUser();

            User currUser = GetCurrentUser(userContext, objectContext);
            currentUser = currUser;

            if (currUser == null)
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session
                    , GetGlobalResourceObject("SiteResources", "errorHaveToBeLogged").ToString());
            }
            else if (!businessUser.IsFromUserTeam(currUser))
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session
                    , GetGlobalResourceObject("SiteResources", "errorPageForUsersOnly").ToString());
            }
        }

        protected void btnSubmitReport_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            if (currentUser == null || !businessUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate))
            {
                return;
            }

            BusinessReport businessReport = new BusinessReport();

            phReport.Visible = true;
            phReport.Controls.Add(lblError);

            string error = string.Empty;
            if (businessReport.CheckIfUserCanSendReportAboutType(objectContext, userContext, currentUser, "irregularity", "general", 1, out error) == true)
            {
                string description = tbReport.Text;

                if (CommonCode.Validate.ValidateComment(ref description, out error))
                {
                    businessReport.CreateGeneralReport(userContext, objectContext, currentUser, businessLog, description);

                    tbReport.Text = string.Empty;

                    phReport.Visible = false;

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetGlobalResourceObject("SiteResources", "ReportSent").ToString());
                }

                ShowInfo();
            }

            lblError.Text = error;
        }


        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
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
    }
}
