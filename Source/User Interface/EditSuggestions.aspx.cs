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
    public partial class EditSuggestions : BasePage
    {
        private User currentUser = null;
        private bool canSendReports = false;

        private bool haveSentSuggestions = false;
        private bool haveReceivedSuggestions = false;

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
            CheckUserAndParams();

            ShowInfo();
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);
        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            BusinessUserOptions userOptions = new BusinessUserOptions();
            if (userOptions.CheckIfUserHaveUnseenTypeSuggestionData(userContext, currentUser) == true)
            {
                userOptions.ChangeIfUserHaveUnseenTypeSuggestionData(userContext, currentUser, false);
            }

            BusinessSiteText siteText = new BusinessSiteText();
            SiteNews about = siteText.GetSiteText(objectContext, "aboutEditSuggestions"); ;
            if (about == null)
            {
                lblGeneralInfo.Text = "About Edit suggestions text not typed.";
            }
            else
            {
                lblGeneralInfo.Text = about.description;
            }

            if (string.IsNullOrEmpty(lblReporting.Text))
            {
                SiteNews aboutEditSuggestions = siteText.GetSiteText(objectContext, "aboutReportEditSuggestion"); ;
                if (aboutEditSuggestions == null)
                {
                    lblReporting.Text = "About report edit suggestion text not typed.";
                }
                else
                {
                    lblReporting.Text = aboutEditSuggestions.description;
                }
            }

            FillSuggestions();
            SetLocalText();
        }

        private void SetLocalText()
        {
            lblReplySuggestion.Text = GetLocalResourceObject("ReplyToSuggestion").ToString();
            btnAddComment.Value = GetGlobalResourceObject("SiteResources", "Send").ToString();
            btnCancelComment.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();

            lblReport.Text = GetGlobalResourceObject("SiteResources", "reportIrregularity").ToString();
            btnSendReport.Value = GetGlobalResourceObject("SiteResources", "Send").ToString();
            btnHideRepData.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();
        }

        private void FillSuggestions()
        {
            BusinessUser bUser = new BusinessUser();
            BusinessReport bReport = new BusinessReport();

            if (bUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate))
            {
                canSendReports = !bReport.IsMaxActiveIrregularityReportsReached(objectContext, currentUser);
            }

            FillSuggestionsByMe();

            FillSuggestionsToMe();

            if (haveSentSuggestions == true)
            {
                if (haveReceivedSuggestions == false)
                {
                    accMySuggestions.RequireOpenedPane = true;
                }

                accMySuggestions.Enabled = true;

                pnlShowMySuggestions.CssClass = "accordionHeaders";

                lblMySuggestions.Text = GetLocalResourceObject("accMySuggestions").ToString();
            }
            else
            {
                accMySuggestions.Enabled = false;

                pnlShowMySuggestions.CssClass = "accordionHeadersNoCursor";

                lblMySuggestions.Text = GetLocalResourceObject("accNoMySuggestions").ToString();
            }

            if (haveReceivedSuggestions == true)
            {
                if (haveSentSuggestions == false)
                {
                    accSuggestionsToMe.RequireOpenedPane = true;
                }

                accSuggestionsToMe.Enabled = true;

                pnlShowSuggestionsToMe.CssClass = "accordionHeaders";

                lblSugegstionsToMe.Text = string.Format("{0} {1}", GetLocalResourceObject("accSuggestionsToMe"), currentUser.username);
            }
            else
            {
                accSuggestionsToMe.Enabled = false;

                pnlShowSuggestionsToMe.CssClass = "accordionHeadersNoCursor";

                lblSugegstionsToMe.Text = GetLocalResourceObject("accNoSuggestionsToMe").ToString();
            }

        }

        private void FillSuggestionsByMe()
        {
            phMySuggestions.Controls.Clear();

            BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();
            List<TypeSuggestion> suggestions = btSuggestions.GetSuggestionsFromUser(objectContext, currentUser, false, true);

            if (suggestions.Count > 0)
            {
                haveSentSuggestions = true;

                BusinessProduct bProduct = new BusinessProduct();
                BusinessCompany bCompany = new BusinessCompany();
                BusinessUser bUser = new BusinessUser();

                List<TypeSuggestionComment> comments = new List<TypeSuggestionComment>();

                User toUser = null;
                User system = bUser.GetSystem(userContext);

                string apSuggByMeClientId = string.Format("{0}_content_", apMySuggestions.ClientID);

                int i = 0;

                Panel paddPanel = new Panel();
                phMySuggestions.Controls.Add(paddPanel);
                paddPanel.CssClass = "paddingLR4";

                foreach (TypeSuggestion suggestion in suggestions)
                {
                    if (!suggestion.ToUserReference.IsLoaded)
                    {
                        suggestion.ToUserReference.Load();
                    }

                    toUser = bUser.GetWithoutVisible(userContext, suggestion.ToUser.ID, true);

                    Panel newPanel = new Panel();
                    paddPanel.Controls.Add(newPanel);
                    newPanel.CssClass = "panelRows greenCellBgr";

                    Panel forPnl = new Panel();
                    newPanel.Controls.Add(forPnl);
                    forPnl.CssClass = "panelInline";
                    forPnl.Width = Unit.Pixel(300);

                    Label forLbl = new Label();
                    forPnl.Controls.Add(forLbl);
                    forLbl.Text = string.Format("{0} ", GetLocalResourceObject("for").ToString());

                    GetSuggestionTypeLabelAndLink(forPnl, suggestion, bProduct, bCompany, apSuggByMeClientId);

                    Panel toPnl = new Panel();
                    newPanel.Controls.Add(toPnl);
                    toPnl.CssClass = "panelInline";
                    toPnl.Width = Unit.Pixel(200);

                    Label toLbl = new Label();
                    toPnl.Controls.Add(toLbl);
                    toLbl.Text = string.Format("{0} ", GetLocalResourceObject("to").ToString());
                    toLbl.CssClass = "marginsLR";

                    if (toUser.visible == true)
                    {
                        toPnl.Controls.Add(CommonCode.UiTools.GetUserHyperLink(toUser));
                    }
                    else
                    {
                        toPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(toUser.username, false));
                    }

                    Label dateLbl = new Label();
                    newPanel.Controls.Add(dateLbl);
                    dateLbl.Text = string.Format("{0}", CommonCode.UiTools.DateTimeToLocalString(suggestion.dateCreated));
                    dateLbl.CssClass = "commentsDate";

                    if (suggestion.active == false && !string.IsNullOrEmpty(suggestion.status))
                    {
                        Panel statusPnl = new Panel();
                        newPanel.Controls.Add(statusPnl);
                        statusPnl.CssClass = "floatRight";

                        //put in panel float right
                        Label statuslbl = new Label();
                        statusPnl.Controls.Add(statuslbl);
                        statuslbl.Text = string.Format("{0} {1}", GetLocalResourceObject("status"), GetStatusText(suggestion));
                        statuslbl.CssClass = "searchPageRatings";
                    }

                    Panel descrPanel = new Panel();
                    newPanel.Controls.Add(descrPanel);

                    descrPanel.Controls.Add(CommonCode.UiTools.GetLabelWithText
                        (Tools.GetFormattedTextFromDB(suggestion.description), false));

                    //// suggestion buttons

                    Panel btnPanel = new Panel();
                    newPanel.Controls.Add(btnPanel);
                    btnPanel.HorizontalAlign = HorizontalAlign.Right;

                    if (suggestion.active == true)
                    {
                        Button dbDecline = new Button();
                        btnPanel.Controls.Add(dbDecline);
                        dbDecline.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px"); ;
                        dbDecline.ID = string.Format("declineMy{0}", suggestion.ID);
                        dbDecline.Text = GetGlobalResourceObject("SiteResources", "Decline").ToString();
                        dbDecline.Click += new EventHandler(dbDecline_Click);
                        dbDecline.Attributes.Add("suggID", suggestion.ID.ToString());
                    }

                    if (canSendReports == true && btSuggestions.CanUserReportTypeSuggestion(currentUser, suggestion) == true)
                    {
                        System.Web.UI.HtmlControls.HtmlInputButton sendReportBtn = new System.Web.UI.HtmlControls.HtmlInputButton("button");
                        btnPanel.Controls.Add(sendReportBtn);
                        sendReportBtn.ID = string.Format("reportBy{0}", suggestion.ID);
                        sendReportBtn.Value = GetGlobalResourceObject("SiteResources", "lblWriteReport").ToString();
                        ////
                        sendReportBtn.Attributes.Add("onclick", string.Format("ShowReportData('{0}','{1}','{2}','{3}','{4}')"
                            , "typeSuggestion", suggestion.ID, sendReportBtn.ClientID, pnlActionReport.ClientID, pnlSendReport.ClientID));
                        ////
                        sendReportBtn.Attributes.Add("class", "htmlButtonStyle");
                        sendReportBtn.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px");
                    }

                    Button dbDelete = new Button();
                    btnPanel.Controls.Add(dbDelete);
                    dbDelete.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px");
                    dbDelete.ID = string.Format("deleteMy{0}", suggestion.ID);
                    dbDelete.Text = GetGlobalResourceObject("SiteResources", "delete").ToString();
                    dbDelete.Click += new EventHandler(dbDelete_Click);
                    dbDelete.Attributes.Add("suggID", suggestion.ID.ToString());


                    //// suggestion comments

                    comments = btSuggestions.GetSuggestionComments(objectContext, suggestion);
                    if (comments.Count > 0)
                    {

                        User commentUser = null;

                        foreach (TypeSuggestionComment comment in comments)
                        {
                            if (!comment.UserReference.IsLoaded)
                            {
                                comment.UserReference.Load();
                            }

                            commentUser = bUser.GetWithoutVisible(userContext, comment.User.ID, true);

                            Panel commPnl = new Panel();
                            newPanel.Controls.Add(commPnl);
                            commPnl.CssClass = "panelRows yellowCellBgr marginRightComm";

                            Panel fromPnl = new Panel();
                            commPnl.Controls.Add(fromPnl);
                            fromPnl.CssClass = "panelInline";
                            fromPnl.Width = Unit.Pixel(295);

                            Label commFrom = new Label();
                            fromPnl.Controls.Add(commFrom);
                            commFrom.Text = string.Format("{0} ", GetLocalResourceObject("from").ToString());

                            if (system.ID == commentUser.ID)
                            {
                                Label userLbl = CommonCode.UiTools.GetLabelWithText(commentUser.username, false);
                                fromPnl.Controls.Add(userLbl);
                                userLbl.CssClass = "userNames";
                            }
                            else
                            {
                                if (commentUser.visible == true)
                                {
                                    fromPnl.Controls.Add(CommonCode.UiTools.GetUserHyperLink(commentUser));
                                }
                                else
                                {
                                    fromPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(commentUser.username, false));
                                }
                            }

                            Label commDate = new Label();
                            commPnl.Controls.Add(commDate);
                            commDate.Text = string.Format("{0}", CommonCode.UiTools.DateTimeToLocalString(comment.dateCreated));
                            commDate.CssClass = "commentsDate";

                            Panel commDescr = new Panel();
                            commPnl.Controls.Add(commDescr);

                            commDescr.Controls.Add(CommonCode.UiTools.GetLabelWithText
                                (Tools.GetFormattedTextFromDB(comment.description), false));
                        }

                        if (suggestion.active == true)
                        {
                            Panel addCommPnl = new Panel();
                            newPanel.Controls.Add(addCommPnl);
                            addCommPnl.HorizontalAlign = HorizontalAlign.Center;

                            System.Web.UI.HtmlControls.HtmlInputButton addCommentBtn = new System.Web.UI.HtmlControls.HtmlInputButton("button");
                            addCommPnl.Controls.Add(addCommentBtn);
                            addCommentBtn.ID = string.Format("commentTo{0}", suggestion.ID);
                            addCommentBtn.Value = GetGlobalResourceObject("SiteResources", "Reply").ToString();
                            ////
                            addCommentBtn.Attributes.Add("onclick", string.Format("ShowAddCommentToSuggestion('{0}', '{1}', '{2}', '{3}')"
                                , addCommentBtn.ClientID, suggestion.ID, pnlAddCommentToSuggestion.ClientID, pnlAddCommentToSuggestionEndPnl.ClientID));
                            ////
                            addCommentBtn.Attributes.Add("class", "htmlButtonStyle");
                            addCommentBtn.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px");
                        }
                    }
                    else if (suggestion.active == true)
                    {
                        System.Web.UI.HtmlControls.HtmlInputButton addCommentBtn = new System.Web.UI.HtmlControls.HtmlInputButton("button");
                        btnPanel.Controls.Add(addCommentBtn);
                        addCommentBtn.Attributes.Add("class", "margins1px");
                        addCommentBtn.ID = string.Format("commentTo{0}", suggestion.ID);
                        addCommentBtn.Value = GetGlobalResourceObject("SiteResources", "Reply").ToString();
                        ////
                        addCommentBtn.Attributes.Add("onclick", string.Format("ShowAddCommentToSuggestion('{0}', '{1}', '{2}', '{3}')"
                            , addCommentBtn.ClientID, suggestion.ID, pnlAddCommentToSuggestion.ClientID, pnlAddCommentToSuggestionEndPnl.ClientID));
                        ////
                        addCommentBtn.Attributes.Add("class", "htmlButtonStyle");
                        addCommentBtn.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px");
                    }

                    i++;
                }
            }
            else
            {
                haveSentSuggestions = false;
            }
        }

        private string GetStatusText(TypeSuggestion suggestion)
        {
            if (suggestion == null)
            {
                throw new CommonCode.UIException("suggestion is null");
            }

            if (suggestion.status == null)
            {
                throw new CommonCode.UIException("suggestion.status is null");
            }

            string result = "";

            switch (suggestion.status)
            {
                case "accepted":
                    result = GetLocalResourceObject("accepted").ToString();
                    break;
                case "declined":
                    result = GetLocalResourceObject("declined").ToString();
                    break;
                case "expired":
                    result = GetLocalResourceObject("expired").ToString();
                    break;
                default:
                    throw new CommonCode.UIException(string.Format("suggestion.status = {0} is not supported", suggestion.type));
            }

            return result;
        }

        private void FillSuggestionsToMe()
        {
            phSuggestionsToMe.Controls.Clear();

            BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();
            List<TypeSuggestion> suggestions = btSuggestions.GetSuggestionsToUser(objectContext, currentUser, false, true);

            if (suggestions.Count > 0)
            {
                haveReceivedSuggestions = true;

                BusinessProduct bProduct = new BusinessProduct();
                BusinessCompany bCompany = new BusinessCompany();
                BusinessUser bUser = new BusinessUser();

                List<TypeSuggestionComment> comments = new List<TypeSuggestionComment>();

                User system = bUser.GetSystem(userContext);
                User byUser = null;

                bool thereAreActiveSugg = false;

                string apSuggToMeClientId = string.Format("{0}_content_", apSuggestionsToMe.ClientID);

                int i = 0;

                Panel paddPanel = new Panel();
                phSuggestionsToMe.Controls.Add(paddPanel);
                paddPanel.CssClass = "paddingLR4";

                foreach (TypeSuggestion suggestion in suggestions)
                {
                    if (!suggestion.ByUserReference.IsLoaded)
                    {
                        suggestion.ByUserReference.Load();
                    }

                    byUser = bUser.GetWithoutVisible(userContext, suggestion.ByUser.ID, true);

                    Panel newPanel = new Panel();
                    paddPanel.Controls.Add(newPanel);
                    newPanel.CssClass = "panelRows blueCellBgr";

                    Panel forPnl = new Panel();
                    newPanel.Controls.Add(forPnl);
                    forPnl.CssClass = "panelInline";
                    forPnl.Width = Unit.Pixel(300);

                    Label forLbl = new Label();
                    forPnl.Controls.Add(forLbl);
                    forLbl.Text = string.Format("{0} ", GetLocalResourceObject("for").ToString());

                    GetSuggestionTypeLabelAndLink(forPnl, suggestion, bProduct, bCompany, apSuggToMeClientId);

                    Panel fromPnl = new Panel();
                    newPanel.Controls.Add(fromPnl);
                    fromPnl.CssClass = "panelInline";
                    fromPnl.Width = Unit.Pixel(200);

                    Label fromLbl = new Label();
                    fromPnl.Controls.Add(fromLbl);
                    fromLbl.Text = string.Format("{0} ", GetLocalResourceObject("from").ToString());

                    if (byUser.visible == true)
                    {
                        fromPnl.Controls.Add(CommonCode.UiTools.GetUserHyperLink(byUser));
                    }
                    else
                    {
                        fromPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(byUser.username, false));
                    }

                    Label dateLbl = new Label();
                    newPanel.Controls.Add(dateLbl);
                    dateLbl.Text = string.Format("{0}", CommonCode.UiTools.DateTimeToLocalString(suggestion.dateCreated));
                    dateLbl.CssClass = "commentsDate";

                    if (suggestion.active == false && !string.IsNullOrEmpty(suggestion.status))
                    {
                        Panel statusPnl = new Panel();
                        newPanel.Controls.Add(statusPnl);
                        statusPnl.CssClass = "floatRight";

                        //put in panel float right
                        Label statuslbl = new Label();
                        statusPnl.Controls.Add(statuslbl);
                        statuslbl.Text = string.Format("{0} {1}", GetLocalResourceObject("status"), GetStatusText(suggestion));
                        statuslbl.CssClass = "searchPageRatings";
                    }

                    Panel descrPanel = new Panel();
                    newPanel.Controls.Add(descrPanel);

                    descrPanel.Controls.Add(CommonCode.UiTools.GetLabelWithText
                        (Tools.GetFormattedTextFromDB(suggestion.description), false));

                    //// suggestion buttons

                    Panel btnPanel = new Panel();
                    newPanel.Controls.Add(btnPanel);
                    btnPanel.HorizontalAlign = HorizontalAlign.Right;

                    if (suggestion.active == true)
                    {
                        Button dbAccept = new Button();
                        btnPanel.Controls.Add(dbAccept);
                        dbAccept.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px");
                        dbAccept.ID = string.Format("acceptBy{0}", suggestion.ID);
                        dbAccept.Text = GetGlobalResourceObject("SiteResources", "Accept").ToString();
                        dbAccept.Click += new EventHandler(dbAccept_Click);
                        dbAccept.Attributes.Add("suggID", suggestion.ID.ToString());

                        Button dbDecline = new Button();
                        btnPanel.Controls.Add(dbDecline);
                        dbDecline.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px");
                        dbDecline.ID = string.Format("declineBy{0}", suggestion.ID);
                        dbDecline.Text = GetGlobalResourceObject("SiteResources", "Decline").ToString();
                        dbDecline.Click += new EventHandler(dbDecline_Click);
                        dbDecline.Attributes.Add("suggID", suggestion.ID.ToString());
                    }

                    if (canSendReports == true && btSuggestions.CanUserReportTypeSuggestion(currentUser, suggestion) == true)
                    {
                        System.Web.UI.HtmlControls.HtmlInputButton sendReportBtn = new System.Web.UI.HtmlControls.HtmlInputButton("button");
                        btnPanel.Controls.Add(sendReportBtn);
                        sendReportBtn.ID = string.Format("reportTo{0}", suggestion.ID);
                        sendReportBtn.Value = GetGlobalResourceObject("SiteResources", "lblWriteReport").ToString();
                        ////
                        sendReportBtn.Attributes.Add("onclick", string.Format("ShowReportData('{0}','{1}','{2}','{3}','{4}')"
                                    , "typeSuggestion", suggestion.ID, sendReportBtn.ClientID, pnlActionReport.ClientID, pnlSendReport.ClientID));
                        ////
                        sendReportBtn.Attributes.Add("class", "htmlButtonStyle");
                        sendReportBtn.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px");
                    }

                    Button dbDelete = new Button();
                    btnPanel.Controls.Add(dbDelete);
                    dbDelete.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px");
                    dbDelete.ID = string.Format("deleteMy{0}", suggestion.ID);
                    dbDelete.Text = GetGlobalResourceObject("SiteResources", "delete").ToString();
                    dbDelete.Click += new EventHandler(dbDelete_Click);
                    dbDelete.Attributes.Add("suggID", suggestion.ID.ToString());

                    //// suggestion comments

                    comments = btSuggestions.GetSuggestionComments(objectContext, suggestion);
                    if (comments.Count > 0)
                    {

                        User commentUser = null;

                        foreach (TypeSuggestionComment comment in comments)
                        {
                            if (!comment.UserReference.IsLoaded)
                            {
                                comment.UserReference.Load();
                            }

                            commentUser = bUser.GetWithoutVisible(userContext, comment.User.ID, true);

                            Panel commPnl = new Panel();
                            newPanel.Controls.Add(commPnl);
                            commPnl.CssClass = "panelRows yellowCellBgr marginRightComm";

                            Panel fromCommPnl = new Panel();
                            commPnl.Controls.Add(fromCommPnl);
                            fromCommPnl.CssClass = "panelInline";
                            fromCommPnl.Width = Unit.Pixel(295);

                            Label commFrom = new Label();
                            fromCommPnl.Controls.Add(commFrom);
                            commFrom.Text = string.Format("{0} ", GetLocalResourceObject("from").ToString());

                            if (system.ID == commentUser.ID)
                            {
                                Label userLbl = CommonCode.UiTools.GetLabelWithText(commentUser.username, false);
                                fromCommPnl.Controls.Add(userLbl);
                                userLbl.CssClass = "userNames";
                            }
                            else
                            {
                                if (commentUser.visible == true)
                                {
                                    fromCommPnl.Controls.Add(CommonCode.UiTools.GetUserHyperLink(commentUser));
                                }
                                else
                                {
                                    fromCommPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(commentUser.username, false));
                                }
                            }

                            Label commDate = new Label();
                            commPnl.Controls.Add(commDate);
                            commDate.Text = string.Format("{0}", CommonCode.UiTools.DateTimeToLocalString(comment.dateCreated));
                            commDate.CssClass = "commentsDate";

                            Panel commDescr = new Panel();
                            commPnl.Controls.Add(commDescr);

                            commDescr.Controls.Add(CommonCode.UiTools.GetLabelWithText
                                (Tools.GetFormattedTextFromDB(comment.description), false));
                        }

                        if (suggestion.active == true)
                        {
                            Panel addCommPnl = new Panel();
                            newPanel.Controls.Add(addCommPnl);
                            addCommPnl.HorizontalAlign = HorizontalAlign.Center;

                            System.Web.UI.HtmlControls.HtmlInputButton addCommentBtn = new System.Web.UI.HtmlControls.HtmlInputButton("button");
                            addCommPnl.Controls.Add(addCommentBtn);
                            addCommentBtn.ID = string.Format("commentTo{0}", suggestion.ID);
                            addCommentBtn.Value = GetGlobalResourceObject("SiteResources", "Reply").ToString();
                            ////
                            addCommentBtn.Attributes.Add("onclick", string.Format("ShowAddCommentToSuggestion('{0}', '{1}', '{2}', '{3}')"
                                , addCommentBtn.ClientID, suggestion.ID, pnlAddCommentToSuggestion.ClientID, pnlAddCommentToSuggestionEndPnl.ClientID));
                            ////
                            addCommentBtn.Attributes.Add("class", "htmlButtonStyle");
                            addCommentBtn.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px");
                        }
                    }
                    else if (suggestion.active == true)
                    {
                        System.Web.UI.HtmlControls.HtmlInputButton addCommentBtn = new System.Web.UI.HtmlControls.HtmlInputButton("button");
                        btnPanel.Controls.Add(addCommentBtn);
                        addCommentBtn.ID = string.Format("commentTo{0}", suggestion.ID);
                        addCommentBtn.Value = GetGlobalResourceObject("SiteResources", "Reply").ToString();
                        ////
                        addCommentBtn.Attributes.Add("onclick", string.Format("ShowAddCommentToSuggestion('{0}', '{1}', '{2}', '{3}')"
                            , addCommentBtn.ClientID, suggestion.ID, pnlAddCommentToSuggestion.ClientID, pnlAddCommentToSuggestionEndPnl.ClientID));
                        ////
                        addCommentBtn.Attributes.Add("class", "htmlButtonStyle");
                        addCommentBtn.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px 0px 1px");
                    }

                    if (suggestion.active == true)
                    {
                        thereAreActiveSugg = true;
                    }

                    i++;
                }

                if (thereAreActiveSugg == true)
                {
                    Panel infoPnl = new Panel();
                    phSuggestionsToMe.Controls.AddAt(0, infoPnl);

                    infoPnl.HorizontalAlign = HorizontalAlign.Right;
                    infoPnl.CssClass = "marginTB5";

                    Label infoLbl = new Label();
                    infoPnl.Controls.Add(infoLbl);

                    infoLbl.Text = string.Format("{0} {1} {2}", GetLocalResourceObject("infoAccSuggestions")
                        , Configuration.TypeSuggestionDaysAfterWhichSuggestionExpires
                        , GetLocalResourceObject("infoAccSuggestions2")); // You have 1 day to accept or decline the suggestions.
                }

            }
            else
            {
                haveReceivedSuggestions = false;
            }
        }

        void dbDelete_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
            {
                throw new CommonCode.UIException("Couln`t get button dbDelete.");
            }

            BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();

            long suggID = 0;
            string strId = btn.Attributes["suggID"];
            if (!long.TryParse(strId, out suggID))
            {
                throw new CommonCode.UIException("couldn`t parse dbDelete.Attributes[suggID] to long");
            }

            TypeSuggestion currSuggestion = btSuggestions.GetSuggestion(objectContext, suggID, false, true);

            if (!currSuggestion.ByUserReference.IsLoaded)
            {
                currSuggestion.ByUserReference.Load();
            }
            if (!currSuggestion.ToUserReference.IsLoaded)
            {
                currSuggestion.ToUserReference.Load();
            }

            if (currSuggestion.ToUser.ID != currentUser.ID && currSuggestion.ByUser.ID != currentUser.ID)
            {
                throw new CommonCode.UIException(string.Format("User id : {0} cannot delete suggestion id : {1}, because hes not sender or receiver."));
            }

            btSuggestions.DeleteSuggestion(objectContext, userContext, businessLog, currSuggestion, currentUser);

            FillSuggestions();

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification,
                GetLocalResourceObject("SuggestionDeleted").ToString());
        }

        void dbAccept_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
            {
                throw new CommonCode.UIException("Couln`t get button dbAccept.");
            }

            BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();

            long suggID = 0;
            string strId = btn.Attributes["suggID"];
            if (!long.TryParse(strId, out suggID))
            {
                throw new CommonCode.UIException("couldn`t parse dbAccept.Attributes[suggID] to long");
            }

            TypeSuggestion currSuggestion = btSuggestions.GetSuggestion(objectContext, suggID, false, true);

            if (currSuggestion.active == false)
            {
                throw new CommonCode.UIException(string.Format("User id : {0} cannot accept suggestion id : {1} because its not active."
                    , currentUser.ID, currSuggestion.ID));
            }

            if (!currSuggestion.ToUserReference.IsLoaded)
            {
                currSuggestion.ToUserReference.Load();
            }

            if (currSuggestion.ToUser.ID != currentUser.ID)
            {
                throw new CommonCode.UIException(string.Format("User id : {0} cannot accept suggestion id : {1}, because hes not the receiver."
                    , currentUser.ID, currSuggestion.ID));
            }

            btSuggestions.AcceptSuggestion(objectContext, userContext, businessLog, currSuggestion, currentUser);

            FillSuggestions();

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("SuggestionAccepted").ToString());

        }

        void dbDecline_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
            {
                throw new CommonCode.UIException("Couln`t get button dbDecline.");
            }

            BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();

            long suggID = 0;
            string strId = btn.Attributes["suggID"];
            if (!long.TryParse(strId, out suggID))
            {
                throw new CommonCode.UIException("couldn`t parse dbDecline.Attributes[suggID] to long");
            }

            TypeSuggestion currSuggestion = btSuggestions.GetSuggestion(objectContext, suggID, false, true);

            if (currSuggestion.active == false)
            {
                throw new CommonCode.UIException(string.Format("User id : {0} cannot decline suggestion id : {1} because its not active."
                    , currentUser.ID, currSuggestion.ID));
            }

            if (!currSuggestion.ByUserReference.IsLoaded)
            {
                currSuggestion.ByUserReference.Load();
            }
            if (!currSuggestion.ToUserReference.IsLoaded)
            {
                currSuggestion.ToUserReference.Load();
            }

            if (currSuggestion.ToUser.ID != currentUser.ID && currSuggestion.ByUser.ID != currentUser.ID)
            {
                throw new CommonCode.UIException(string.Format("User id : {0} cannot decline suggestion id : {1}, because hes not sender or receiver."
                    , currentUser.ID, currSuggestion.ID));
            }

            btSuggestions.DeclineSuggestion(objectContext, userContext, businessLog, currSuggestion, currentUser, false, string.Empty);

            FillSuggestions();

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("SuggestionDeclined").ToString());
        }



        private void GetSuggestionTypeLabelAndLink(Panel currPanel, TypeSuggestion suggestion, BusinessProduct bProduct,
            BusinessCompany bCompany, string apSuggClientId)
        {
            if (suggestion == null)
            {
                throw new CommonCode.UIException("suggestion is null");
            }

            switch (suggestion.type)
            {
                case "product":
                    Product currProduct = bProduct.GetProductByIDWV(objectContext, suggestion.typeID);
                    if (currProduct == null)
                    {
                        throw new CommonCode.UIException(string.Format("There is no product with id : {0} which is for type suggestion id : {1}"
                            , suggestion.typeID, suggestion.ID));
                    }

                    if (currProduct.visible == true)
                    {
                        HyperLink link = new HyperLink();
                        currPanel.Controls.Add(link);
                        link.CssClass = "marginsLR";

                        link.Text = Tools.BreakLongWordsInString(currProduct.name, 35);
                        link.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", currProduct.ID));

                        link.ID = string.Format("prod{0}sugg{1}", currProduct.ID, suggestion.ID);
                        link.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}','{2}')", currProduct.ID, link.ClientID, pnlPopUp.ClientID));
                        link.Attributes.Add("onmouseout", "HideData()");
                    }
                    else
                    {
                        Label nameLbl = new Label();
                        currPanel.Controls.Add(nameLbl);
                        nameLbl.CssClass = "marginsLR";
                        nameLbl.Text = Tools.BreakLongWordsInString(currProduct.name, 35);
                    }

                    break;
                case "company":
                    Company currCompany = bCompany.GetCompanyWV(objectContext, suggestion.typeID);
                    if (currCompany == null)
                    {
                        throw new CommonCode.UIException(string.Format("There is no company with id : {0} which is for type suggestion id : {1}"
                            , suggestion.typeID, suggestion.ID));
                    }

                    if (currCompany.visible == true)
                    {
                        HyperLink link = new HyperLink();
                        currPanel.Controls.Add(link);
                        link.CssClass = "marginsLR";

                        link.Text = Tools.BreakLongWordsInString(currCompany.name, 35);
                        link.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", currCompany.ID));

                        link.ID = string.Format("maker{0}sugg{1}", currCompany.ID, suggestion.ID);
                        link.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}','{2}')", currCompany.ID, link.ClientID, pnlPopUp.ClientID));
                        link.Attributes.Add("onmouseout", "HideData()");
                    }
                    else
                    {
                        Label nameLbl = new Label();
                        currPanel.Controls.Add(nameLbl);
                        nameLbl.CssClass = "marginsLR";
                        nameLbl.Text = Tools.BreakLongWordsInString(currCompany.name, 35);
                    }

                    break;
                default:
                    throw new CommonCode.UIException(
                        string.Format("suggestion.type = {0} is not supported type", suggestion.type));
            }

        }

        private void CheckUserAndParams()
        {
            BusinessUser businessUser = new BusinessUser();

            User currUser = GetCurrentUser(userContext, objectContext);
            currentUser = currUser;

            if (currUser == null)
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetGlobalResourceObject("SiteResources", "errorHaveToBeLogged").ToString());
            }
            else if (!businessUser.IsUser(currUser))
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errHaveToBeUser").ToString());
            }
        }


        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }

        [WebMethod]
        public static string WMAddCommentSuggestionToUser(string suggID, string description)
        {
            return CommonCode.WebMethods.AddCommentToSuggestion(suggID, description);
        }

        [WebMethod]
        public static string WMSendReport(string type, string strTypeId, string description)
        {
            return CommonCode.WebMethods.SendReport(type, strTypeId, description);
        }
    }
}
