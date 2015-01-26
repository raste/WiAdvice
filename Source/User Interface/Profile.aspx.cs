﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Collections;

using CustomServerControls;
using BusinessLayer;
using DataAccess;

namespace UserInterface
{
    public partial class Profile : BasePage
    {
        protected long CommentsNumber = 0;                                          // All user comments number
        protected long CurrCommentsPage = 1;                                        // Current page
        protected long CommentsOnPage = Configuration.UsersUserCommentsOnPage;      // Comments per page

        private Boolean canEdit = false;
        private Boolean isVisited = false;
        private User currentUser = null;
        private User visitedUser = null;

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
            if (isVisited == true && visitedUser != null)
            {
                lblSendReport.Attributes.Add("onclick", string.Format("ShowReportData('{0}','{1}','{2}','{3}','{4}')"
                        , "user", visitedUser.ID, lblSendReport.ClientID, pnlActionReport.ClientID, pnlSendReport.ClientID));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetNeedsToBeLogged();        // when user is logging out - to redirect him to home page
            CheckUserAndParams();        // Checks parameters and shows options depending on user
            ShowInfo();
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);   // Hides user notification panel
        }

        private void ShowInfo()
        {
            string username = string.Empty;
            if (isVisited == true)
            {
                username = visitedUser.username;

                if (apWarnings.Visible == true)
                {
                    visitedDivBr1.Visible = true;
                    visitedDivBr2.Visible = false;
                }
                else
                {
                    visitedDivBr1.Visible = true;
                    visitedDivBr2.Visible = true;
                }

            }
            else
            {
                username = currentUser.username;

                visitedDivBr1.Visible = false;
                visitedDivBr2.Visible = false;
            }

            Title = string.Format("{0} {1}", username, GetLocalResourceObject("title"));

            ShowAdvertisements();

            SetLocalText();
        }

        private void SetLocalText()
        {
            lblWarnings.Text = GetLocalResourceObject("Warnings").ToString();

            btnShowEditPanel.Text = GetLocalResourceObject("Edit").ToString();
            btnSubmitAction.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
            btnDiscard.Text = GetGlobalResourceObject("SiteResources", "Cancel").ToString();

            hlEditRoles.Text = GetLocalResourceObject("EditRoles").ToString();

            lblSendReport.Text = GetLocalResourceObject("Report").ToString();
            lblReportIrregularity.Text = GetGlobalResourceObject("SiteResources", "reportIrregularity").ToString();
            btnSendReport.Value = GetGlobalResourceObject("SiteResources", "Submit").ToString();
            btnHideRepData.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();
        }

        private void CheckUserAndParams()
        {
            BusinessUser businessUser = new BusinessUser();

            User currUser = GetCurrentUser(userContext, objectContext);
            currentUser = currUser;

            isVisited = Visited(businessUser, currUser);
            visitedUser = GetVisitedUser();


            if (isVisited)
            {
                FillData(visitedUser);

                if (businessUser.IsUser(visitedUser))
                {
                    hlEditRoles.Visible = true;
                    hlEditRoles.NavigateUrl = GetUrlWithVariant(string.Format("EditorRights.aspx?User={0}", visitedUser.ID));
                }

                if (canEdit)    // admin panel
                {
                    accAdmin.Visible = true;

                    if (!visitedUser.UserOptionsReference.IsLoaded)
                    {
                        visitedUser.UserOptionsReference.Load();
                    }

                    // shows user roles
                    FillUserRoles(visitedUser);
                    FillTblRolesThatUserDontHave(visitedUser);


                    if (visitedUser.UserOptions.activated == false)
                    {
                        btnDeleteUser.Enabled = false;
                    }
                    else
                    {
                        btnDeleteUser.Enabled = true;
                    }

                    if (visitedUser.visible == false)
                    {
                        lblDeleteUser.Text = "User is Deleted. Restore?";
                        btnDeleteUser.Text = "Restore";
                    }
                    else
                    {
                        lblDeleteUser.Text = "User is Visible. Delete?";
                        btnDeleteUser.Text = "Delete";
                    }

                    ShowDelCommButtonIfUserHaveComments(visitedUser);
                    ShowRemoveSignatureOption(businessUser, visitedUser);
                    ShowChangeNameOption(businessUser, visitedUser);
                    ShowActivateOptions(businessUser, visitedUser);
                }
                else
                {
                    accAdmin.Visible = false;
                }

                pnlSelfEdit.Visible = false;

                CheckIfUserCanSendReports();

                CheckCommentsPageParams(true);
                FillTblPages(true);

                ShowComments(visitedUser);
            }
            else if (currUser != null)
            {
                FillData(currUser);

                accAdmin.Visible = false;
                pnlSelfEdit.Visible = true;


                if (IsPostBack == false)
                {
                    ShowEditMembers(businessUser, currUser);
                    hfReplyToReport.Value = "";
                }

                CheckCommentsPageParams(false);
                FillTblPages(false);

                ShowComments(currUser);

            }
            else
            {
                RedirectToOtherUrl("Home.aspx");
            }
        }

        private void ShowActivateOptions(BusinessUser bUser, User visitedUser)
        {
            if (bUser.IsFromUserTeam(visitedUser) == true)
            {
                BusinessUserOptions bUserOptions = new BusinessUserOptions();

                if (!visitedUser.UserOptionsReference.IsLoaded)
                {
                    visitedUser.UserOptionsReference.Load();
                }

                if (bUserOptions.IsUserActivated(visitedUser.UserOptions) == false)
                {
                    lblUserActivated.Text = string.Format("{0} is NOT activated.", visitedUser.username);
                    btnActivateUser.Visible = true;
                }
                else
                {
                    lblUserActivated.Text = string.Format("{0} is activated.", visitedUser.username);
                    btnActivateUser.Visible = false;
                }
            }
            else
            {
                lblUserActivated.Visible = false;
                btnActivateUser.Visible = false;
            }
        }

        public void ShowRemoveSignatureOption(BusinessUser bUser, User forUser)
        {
            if (bUser.CanUserDo(userContext, forUser, UserRoles.HaveSignature) == true)
            {
                if (!string.IsNullOrEmpty(forUser.userData))
                {
                    lblRemoveUserSignature.Visible = true;
                    btnRemoveUserSignature.Visible = true;
                }
                else
                {
                    lblRemoveUserSignature.Visible = false;
                    btnRemoveUserSignature.Visible = false;
                }
            }
            else
            {
                lblRemoveUserSignature.Visible = false;
                btnRemoveUserSignature.Visible = false;
            }
        }

        private void CheckIfUserCanSendReports()
        {
            if (currentUser == null)
            {
                lblSendReport.Visible = false;
                return;
            }

            BusinessUser businessUser = new BusinessUser();
            BusinessReport businessReport = new BusinessReport();

            if (businessUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate)
                   && !businessReport.IsMaxActiveIrregularityReportsReached(objectContext, currentUser))
            {
                lblSendReport.Visible = true;

                if (string.IsNullOrEmpty(lblReporting.Text))
                {
                    BusinessSiteText businessText = new BusinessSiteText();
                    SiteNews aboutReporting = businessText.GetSiteText(objectContext, "aboutReportUser");
                    if (aboutReporting != null && !string.IsNullOrEmpty(aboutReporting.description))
                    {
                        lblReporting.Text = aboutReporting.description;
                    }
                    else
                    {
                        lblReporting.Text = "About reporting user text not typed.";
                    }
                }
            }
            else
            {
                lblSendReport.Visible = false;
            }

        }

        private void ShowDelCommButtonIfUserHaveComments(User userVisited)
        {
            BusinessComment businessComment = new BusinessComment();
            if (businessComment.CountUserComments(objectContext, userVisited) > 0)
            {
                lblDelOpinions.Visible = true;
                btnDelOpinions.Visible = true;
                cbSendWarning.Visible = true;
            }
            else
            {
                lblDelOpinions.Visible = false;
                btnDelOpinions.Visible = false;
                cbSendWarning.Visible = false;
            }
        }

        private void FillSuggestions(User user)
        {
            phSuggestions.Controls.Clear();

            BusinessSuggestion businessSuggestions = new BusinessSuggestion();

            List<Suggestion> suggestions = businessSuggestions.GetUserSuggestions(objectContext, user, true);

            if (suggestions.Count > 0)
            {
                lblSuggestions.Text = GetLocalResourceObject("Suggestions").ToString();
                pnlShowSuggestions.CssClass = "accordionHeaders";

                int i = 0;
                foreach (Suggestion suggestion in suggestions)
                {
                    Panel newPanel = new Panel();
                    phSuggestions.Controls.Add(newPanel);


                    if (i % 2 == 0)
                    {
                        newPanel.CssClass = "suggestions blueCellBgr";
                    }
                    else
                    {
                        newPanel.CssClass = "suggestions";
                        newPanel.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                    }

                    Panel pnlType = new Panel();
                    newPanel.Controls.Add(pnlType);
                    pnlType.CssClass = "panelInline";
                    pnlType.Width = Unit.Pixel(250);

                    Label lblType = CommonCode.UiTools.GetLabelWithText(string.Format("{0} : {1}"
                        , GetLocalResourceObject("About"), BusinessSuggestion.GetSuggestionCategoryLocalText(suggestion)), false);
                    lblType.CssClass = "searchPageComments";
                    pnlType.Controls.Add(lblType);

                    Label lblDate = CommonCode.UiTools.GetLabelWithText(CommonCode.UiTools.DateTimeToLocalString(suggestion.dateCreated), false);
                    newPanel.Controls.Add(lblDate);
                    lblDate.CssClass = "commentsDate marginLeft";

                    Panel descrPnl = new Panel();
                    newPanel.Controls.Add(descrPnl);
                    descrPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(suggestion.description, false));

                    i++;
                }
            }
            else
            {
                lblSuggestions.Text = GetLocalResourceObject("NoSuggestions").ToString();
                pnlShowSuggestions.CssClass = "accordionHeadersNoCursor";
            }


        }

        private void FillSystemMessages()
        {
            phSystemMessages.Controls.Clear();

            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();

            List<SystemMessage> messages = bSystemMessages.GetUserMessages(userContext, currentUser, true);

            if (messages.Count > 0)
            {
                lblSystemMessages.Text = GetLocalResourceObject("SystemMessages").ToString();
                pnlShowSystemMessages.CssClass = "accordionHeaders";

                int i = 0;
                foreach (SystemMessage message in messages)
                {
                    Panel newPanel = new Panel();
                    phSystemMessages.Controls.Add(newPanel);


                    if (i % 2 == 0)
                    {
                        newPanel.CssClass = "suggestions blueCellBgr";
                    }
                    else
                    {
                        newPanel.CssClass = "suggestions";
                        newPanel.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                    }

                    Label lblDate = CommonCode.UiTools.GetLabelWithText(CommonCode.UiTools.DateTimeToLocalString(message.dateCreated), false);
                    newPanel.Controls.Add(lblDate);
                    lblDate.CssClass = "commentsDate";

                    Panel delPnl = new Panel();
                    newPanel.Controls.Add(delPnl);
                    delPnl.CssClass = "floatRight";

                    DecoratedButton dbDeleteSystemMessage = new DecoratedButton();
                    delPnl.Controls.Add(dbDeleteSystemMessage);
                    dbDeleteSystemMessage.ID = string.Format("DelSysMsg{0}", message.ID);
                    dbDeleteSystemMessage.Attributes.Add("msgID", message.ID.ToString());
                    dbDeleteSystemMessage.Text = GetGlobalResourceObject("SiteResources", "delete").ToString();
                    dbDeleteSystemMessage.Click += new EventHandler(dbDeleteSystemMessage_Click);

                    Panel descrPnl = new Panel();
                    newPanel.Controls.Add(descrPnl);
                    descrPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(message.description, false));

                    i++;
                }
            }
            else
            {
                lblSystemMessages.Text = GetLocalResourceObject("NoSystemMessages").ToString();
                pnlShowSystemMessages.CssClass = "accordionHeadersNoCursor";
            }


        }

        void dbDeleteSystemMessage_Click(object sender, EventArgs e)
        {
            CheckIfUserIsModifyingHimSelf();

            Button btnDelMsg = sender as Button;
            if (btnDelMsg != null)
            {
                long msgId = -1;
                string msgIdStr = btnDelMsg.Attributes["msgID"];
                if (long.TryParse(msgIdStr, out msgId))
                {
                    BusinessSystemMessages bSystemMessage = new BusinessSystemMessages();
                    SystemMessage currMessage = bSystemMessage.Get(userContext, currentUser, msgId, true, true);

                    bSystemMessage.DeleteSystemMessage(userContext, currentUser, currMessage);

                    CheckSystemMessagesOptions();

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("SMdeleted").ToString());
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("Couldnt parse btnDelMsg.Attributes['msgID'] = {0} to long , user id = {1}", msgIdStr, currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("coultn get Button");
            }
        }

        private void ShowAdvertisements()
        {
            if (Configuration.AdvertsNumAdvertsOnUserPage > 0)
            {
                phAdverts.Controls.Clear();
                adCell.Attributes.Clear();

                phAdverts.Controls.Add(CommonCode.ImagesAndAdverts.GetAdvertisements
                    (objectContext, Server, "general", 1, Configuration.AdvertsNumAdvertsOnUserPage));

                if (CommonCode.ImagesAndAdverts.getAdvertisementsNumber(phAdverts) > 0)
                {
                    phAdverts.Visible = true;
                    adCell.Width = "252px";
                    adCell.VAlign = "top";
                }
                else
                {
                    phAdverts.Visible = false;
                    adCell.Width = "0px";
                }
            }
        }

        private void CheckIfUserCanEditAllUsersFromEvents()
        {
            BusinessUser businessUser = new BusinessUser();

            if (isVisited)
            {
                User userVisited = GetVisitedUser();

                if (businessUser.CanAdminEditUserOrAdmin(userContext, currentUser, userVisited) == false)
                {
                    throw new CommonCode.UIException(string.Format("User ID = {0} cannot modify user id = {1}", currentUser.ID, userVisited.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("User ID = {0} is not visited (trying to edit himself ?)", currentUser.ID));
            }
        }

        private void CheckIfUserIsModifyingHimSelf()
        {
            if (isVisited)
            {
                throw new CommonCode.UIException(string.Format("User Id = {0} is trying to modify other user", currentUser.ID));
            }
        }

        private void CheckCommentsPageParams(Boolean visited)
        {
            BusinessComment businessComment = new BusinessComment();
            string urlToRedirect = "";
            if (visited)
            {
                urlToRedirect = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", GetVisitedUser().ID));
                CommentsNumber = businessComment.CountUserComments(objectContext, GetVisitedUser());

            }
            else
            {
                urlToRedirect = GetUrlWithVariant("Profile.aspx");
                CommentsNumber = businessComment.CountUserComments(objectContext, currentUser);
            }

            CommonCode.Pages.CheckPageParameters(Response, CommentsNumber, CommentsOnPage.ToString(),
                Request.Params["page"], urlToRedirect, out CurrCommentsPage, out CommentsOnPage);
        }

        private void FillTblPages(Boolean visited)
        {
            string urlToAppend = "";
            if (visited)
            {
                urlToAppend = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", GetVisitedUser().ID));
            }
            else
            {
                urlToAppend = GetUrlWithVariant("Profile.aspx");
            }

            tblPages.Rows.Clear();
            tblPages.Rows.Add(CommonCode.Pages.GetPagesRow(CommentsNumber, CommentsOnPage, CurrCommentsPage, urlToAppend));

            tblPagesBtm.Rows.Clear();
            tblPagesBtm.Rows.Add(CommonCode.Pages.GetPagesRow(CommentsNumber, CommentsOnPage, CurrCommentsPage, urlToAppend));
        }

        private void ShowComments(User user)
        {
            phComments.Controls.Clear();

            BusinessComment businessComment = new BusinessComment();
            BusinessProduct businessProduct = new BusinessProduct();
            BusinessUser businessUser = new BusinessUser();
            BusinessRating businessRating = new BusinessRating();

            long from = 0;
            long to = 0;
            CommonCode.Pages.GetFromItemNumberToItemNumber(CurrCommentsPage, CommentsOnPage, out from, out to);

            IEnumerable<Comment> comments = businessComment.GetAllFromUser(userContext, objectContext, user.ID, true, from, to);
            lbComments.Text = string.Format("{0} {1}", GetLocalResourceObject("Comments"), CommentsNumber.ToString());

            User currUser = CommonCode.UiTools.GetCurrentUserNoExc(userContext);

            if (CommentsNumber > 0)
            {
                long i = 0;

                foreach (Comment comment in comments)
                {
                    Panel newPanel = new Panel();
                    phComments.Controls.Add(newPanel);
                    newPanel.CssClass = "panelRows greenCellBgr";

                    Table newTable = new Table();
                    newPanel.Controls.Add(newTable);
                    newTable.Width = Unit.Percentage(100);

                    TableRow row = new TableRow();
                    newTable.Rows.Add(row);

                    row.Attributes.Add("commID", comment.ID.ToString());
                    row.Attributes.Add("usr", user.ID.ToString());

                    TableCell cellDate = new TableCell();
                    cellDate.VerticalAlign = VerticalAlign.Top;
                    cellDate.CssClass = "commentsDate";
                    cellDate.Width = Unit.Pixel(200);
                    //cellDate.BorderWidth = 1;
                    cellDate.Text = CommonCode.UiTools.DateTimeToLocalString(comment.dateCreated);
                    row.Cells.Add(cellDate);

                    TableCell aboutCell = new TableCell();
                    row.Cells.Add(aboutCell);
                    aboutCell.VerticalAlign = VerticalAlign.Top;

                    Label lblAbout = new Label();
                    lblAbout.Text = CommonCode.UiTools.GetCommentAbout(userContext, objectContext, businessComment
                        , comment, currentUser);

                    aboutCell.Controls.Add(lblAbout);
                    aboutCell.Controls.Add(CommonCode.UiTools.GetCommentIn(objectContext, businessComment
                        , comment, pnlPopUp, currentUser));

                    TableCell ratingCell = new TableCell();
                    row.Cells.Add(ratingCell);
                    ratingCell.VerticalAlign = VerticalAlign.Top;
                    ratingCell.HorizontalAlign = HorizontalAlign.Right;
                    ratingCell.Width = Unit.Pixel(100);

                    Image plusImg = new Image();
                    plusImg.ImageUrl = "~\\images\\SiteImages\\plus.png";
                    plusImg.CssClass = "middleAlign";
                    ratingCell.Controls.Add(plusImg);

                    Label agreesLbl = new Label();
                    agreesLbl.Text = string.Format("({0}) ", comment.agrees);
                    ratingCell.Controls.Add(agreesLbl);

                    Image minusImg = new Image();
                    minusImg.ImageUrl = "~\\images\\SiteImages\\minus.png";
                    minusImg.CssClass = "middleAlign";
                    ratingCell.Controls.Add(minusImg);

                    Label disagreesLbl = new Label();
                    disagreesLbl.Text = string.Format("({0}) ", comment.disagrees);
                    ratingCell.Controls.Add(disagreesLbl);

                    if (canEdit)
                    {
                        TableCell delCell = new TableCell();
                        row.Cells.Add(delCell);
                        delCell.Width = Unit.Pixel(210);
                        delCell.VerticalAlign = VerticalAlign.Top;

                        Button delBtn = new Button();
                        delBtn.ID = string.Format("DeleteComm{0}", comment.ID);
                        delBtn.Text = GetGlobalResourceObject("SiteResources", "delete").ToString();
                        delBtn.Click += new EventHandler(DeleteComment_Click);
                        delCell.Controls.Add(delBtn);


                        Button delBtnNW = new Button();
                        delBtnNW.ID = string.Format("DeleteCommNW{0}", comment.ID);
                        delBtnNW.Text = GetLocalResourceObject("deleteNoWarning").ToString();
                        delBtnNW.ToolTip = GetLocalResourceObject("deleteNoWarningTooltip").ToString();
                        delBtnNW.Click += new EventHandler(delBtnNW_Click);
                        delBtnNW.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, "5px");
                        delCell.Controls.Add(delBtnNW);
                    }

                    Panel descPanel = new Panel();
                    newPanel.Controls.Add(descPanel);

                    Label lblDescription = new Label();
                    descPanel.Controls.Add(lblDescription);
                    lblDescription.Text = Tools.GetFormattedTextFromDB(comment.description);


                    i++;
                }
            }
        }

        void delBtnNW_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();
            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
            {
                throw new CommonCode.UIException(string.Format("User ID = {0} cannot delete comment", currentUser.ID));
            }

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                TableCell tblCell = btnSender.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long commID = 0;
                        string commentIdStr = tblRow.Attributes["commID"];
                        long.TryParse(commentIdStr, out commID);

                        string strUser = tblRow.Attributes["usr"];
                        long userID = 0;
                        if (long.TryParse(strUser, out userID))
                        {

                            if (commID > 0)
                            {
                                DeleteUserComment(businessUser, userID, commID, false);
                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("commID is < 1 (comming from tblRow.Attributes['commID']) , user id = {0}", currentUser.ID));
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("Couldnt parse strUser = {0} to long , user id = {1}", strUser, currentUser.ID));
                        }

                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldnt get parent row");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Coudltn get parent cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button.");
            }

        }


        protected void DeleteComment_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();
            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
            {
                throw new CommonCode.UIException(string.Format("User ID = {0} cannot delete comment", currentUser.ID));
            }

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                TableCell tblCell = btnSender.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long commID = 0;
                        string commentIdStr = tblRow.Attributes["commID"];
                        long.TryParse(commentIdStr, out commID);

                        string strUser = tblRow.Attributes["usr"];
                        long userID = 0;
                        if (long.TryParse(strUser, out userID))
                        {

                            if (commID > 0)
                            {
                                DeleteUserComment(businessUser, userID, commID, true);
                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("commID is < 1 (comming from tblRow.Attributes['commID']) , user id = {0}", currentUser.ID));
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("Couldnt parse strUser = {0} to long , user id = {1}", strUser, currentUser.ID));
                        }

                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldnt get parent row");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Coudltn get parent cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button.");
            }
        }


        private void DeleteUserComment(BusinessUser bUser, long userID, long commID, bool sendWarning)
        {

            User user = bUser.GetWithoutVisible(userContext, userID, true);

            BusinessComment businessComment = new BusinessComment();

            Comment currComment = businessComment.Get(objectContext, commID);
            if (currComment != null)
            {
                businessComment.DeleteComment(objectContext, userContext, currComment, currentUser, businessLog, true, sendWarning);

                CommentsNumber = businessComment.CountUserComments(objectContext, user);
                FillTblPages(true);
                ShowComments(user);
                Discard();

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Comment deleted!");
            }
            else
            {
                throw new CommonCode.UIException(string.Format("Theres no comment id = {0} , user id = {1}", commID, currentUser.ID));
            }
        }

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }

        private void FillTblRolesThatUserDontHave(User currUser)
        {
            tblRolesThatDontHave.Rows.Clear();

            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();
            BusinessUserActions businessUserActions = new BusinessUserActions();

            List<Actions> Actions = businessUserActions.GetRolesThatUserDontHave(userContext, currUser);
            List<TypeAction> TypeActions = businessUserTypeActions.GetDeletedUserTypeActions(objectContext, currUser);

            if (Actions.Count > 0 || TypeActions.Count > 0)
            {
                if (!visitedUser.UserOptionsReference.IsLoaded)
                {
                    visitedUser.UserOptionsReference.Load();
                }
                bool activatedUser = visitedUser.UserOptions.activated;

                bool isModerator = false;
                if (UserTypes.Moderator == BusinessUser.GetUserType(currentUser.type))
                {
                    isModerator = true;
                }
                bool canGive = true;

                tblRolesThatDontHave.Visible = true;

                TableRow firstRow = new TableRow();
                tblRolesThatDontHave.Rows.Add(firstRow);
                TableCell firstCell = new TableCell();
                firstRow.Cells.Add(firstCell);
                firstCell.Text = "Roles that user don`t have or were removed";
                firstCell.CssClass = "textHeaderWA";
                firstCell.HorizontalAlign = HorizontalAlign.Center;
                firstCell.ColumnSpan = 4;

                if (Actions.Count > 0)
                {
                    foreach (Actions action in Actions)
                    {
                        TableRow newRow = new TableRow();
                        tblRolesThatDontHave.Rows.Add(newRow);

                        TableCell idCell = new TableCell();
                        idCell.Width = Unit.Pixel(50);
                        idCell.Text = action.ID.ToString();
                        newRow.Cells.Add(idCell);

                        TableCell cellRoleName = new TableCell();
                        newRow.Cells.Add(cellRoleName);
                        cellRoleName.Width = Unit.Pixel(200);
                        cellRoleName.Text = action.name;

                        TableCell cellRoleDescr = new TableCell();
                        newRow.Cells.Add(cellRoleDescr);
                        cellRoleDescr.Text = action.description;

                        if (isModerator == true)
                        {
                            if (action.type != null && action.typeID != null)
                            {
                                canGive = false;
                            }
                            else
                            {
                                canGive = true;
                            }
                        }

                        TableCell cellGive = new TableCell();
                        newRow.Cells.Add(cellGive);
                        cellGive.Width = Unit.Pixel(10);

                        if (canGive == true && activatedUser == true)
                        {
                            cellGive.Width = Unit.Pixel(10);
                            Button addBtn = new Button();
                            addBtn.ID = string.Format("AddUserAction{0}", action.ID);
                            addBtn.Text = "Give";
                            addBtn.Attributes.Add("userID", currUser.ID.ToString());
                            addBtn.Attributes.Add("actionID", action.ID.ToString());
                            addBtn.Click += new EventHandler(AddRole);
                            cellGive.Controls.Add(addBtn);
                        }

                        idCell.BorderWidth = 2;
                        idCell.BorderColor = System.Drawing.Color.Black;
                        cellRoleName.BorderWidth = 2;
                        cellRoleName.BorderColor = System.Drawing.Color.Black;
                        cellRoleDescr.BorderColor = System.Drawing.Color.Black;
                        cellRoleDescr.BorderWidth = 2;
                        cellGive.BorderWidth = 2;
                        cellGive.BorderColor = System.Drawing.Color.Black;
                    }
                }

                if (TypeActions.Count > 0)
                {
                    if (isModerator == true)
                    {
                        canGive = false;
                    }

                    BusinessProduct businessProduct = new BusinessProduct();
                    BusinessCompany businessCompany = new BusinessCompany();
                    Product currProduct = null;
                    Company currCompany = null;

                    bool typeVisible = true;

                    foreach (TypeAction action in TypeActions)
                    {
                        TableRow newRow = new TableRow();
                        tblRolesThatDontHave.Rows.Add(newRow);

                        TableCell idCell = new TableCell();
                        idCell.Width = Unit.Pixel(50);
                        idCell.Text = action.ID.ToString();
                        newRow.Cells.Add(idCell);

                        TableCell cellRoleName = new TableCell();
                        newRow.Cells.Add(cellRoleName);
                        cellRoleName.Width = Unit.Pixel(200);
                        cellRoleName.Text = action.name;

                        TableCell cellRoleDescr = new TableCell();
                        newRow.Cells.Add(cellRoleDescr);
                        cellRoleDescr.Text = action.description;

                        TableCell cellGive = new TableCell();
                        newRow.Cells.Add(cellGive);

                        if (canGive == true && activatedUser == true)
                        {
                            switch (action.type)
                            {
                                case "aCompProdModificator":

                                    currCompany = businessCompany.GetCompanyWV(objectContext, action.typeID);
                                    if (currCompany == null)
                                    {
                                        throw new CommonCode.UIException(string.Format("There is no company with ID : {0} for which user ID : {1} had removed role."
                                            , action.typeID, currUser.ID));
                                    }
                                    typeVisible = currCompany.visible;

                                    break;
                                case "company":

                                    currCompany = businessCompany.GetCompanyWV(objectContext, action.typeID);
                                    if (currCompany == null)
                                    {
                                        throw new CommonCode.UIException(string.Format("There is no company with ID : {0} for which user ID : {1} had removed role."
                                            , action.typeID, currUser.ID));
                                    }
                                    typeVisible = currCompany.visible;

                                    break;
                                case "product":

                                    currProduct = businessProduct.GetProductByIDWV(objectContext, action.typeID);
                                    if (currProduct == null)
                                    {
                                        throw new CommonCode.UIException(string.Format("There is no product with ID : {0} for which user ID : {1} had removed role."
                                            , action.typeID, currUser.ID));
                                    }
                                    typeVisible = currProduct.visible;

                                    break;
                                default:
                                    throw new CommonCode.UIException(string.Format("action.type = {0} is not supported for returning removed roles on user."));
                            }


                            if (typeVisible == true)
                            {
                                cellGive.Width = Unit.Pixel(10);
                                Button addBtn = new Button();
                                addBtn.ID = string.Format("AddUserTypeAction{0}", action.ID);
                                addBtn.Text = "Give";
                                addBtn.Attributes.Add("userID", currUser.ID.ToString());
                                addBtn.Attributes.Add("typeActionID", action.ID.ToString());
                                addBtn.Click += new EventHandler(AddRole);
                                cellGive.Controls.Add(addBtn);
                            }
                        }

                        idCell.BorderWidth = 2;
                        idCell.BorderColor = System.Drawing.Color.Black;
                        cellRoleName.BorderWidth = 2;
                        cellRoleName.BorderColor = System.Drawing.Color.Black;
                        cellRoleDescr.BorderColor = System.Drawing.Color.Black;
                        cellRoleDescr.BorderWidth = 2;
                        cellGive.BorderWidth = 2;
                        cellGive.BorderColor = System.Drawing.Color.Black;
                    }
                }

            }
            else
            {
                tblRolesThatDontHave.Visible = false;
            }
        }

        private void FillUserRoles(User currUser)
        {
            tblRoles.Rows.Clear();

            BusinessUserActions businessUserActions = new BusinessUserActions();
            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

            List<UserAction> userActions = businessUserActions.GetUserActions(userContext, currUser.ID, true).ToList();

            if (userActions.Count<UserAction>() > 0)
            {
                if (!visitedUser.UserOptionsReference.IsLoaded)
                {
                    visitedUser.UserOptionsReference.Load();
                }
                bool activatedUser = visitedUser.UserOptions.activated;

                bool isModerator = false;
                if (UserTypes.Moderator == BusinessUser.GetUserType(currentUser.type))
                {
                    isModerator = true;
                }
                bool canDelete = true;

                tblRoles.Visible = true;

                if (userActions.Count > 0)
                {

                    TableRow firstRow = new TableRow();
                    tblRoles.Rows.Add(firstRow);
                    TableCell firstCell = new TableCell();
                    firstRow.Cells.Add(firstCell);
                    firstCell.Text = "User roles";
                    firstCell.CssClass = "textHeaderWA";
                    firstCell.HorizontalAlign = HorizontalAlign.Center;
                    firstCell.ColumnSpan = 4;

                    foreach (UserAction useraction in userActions)
                    {
                        TableRow newRow = new TableRow();
                        tblRoles.Rows.Add(newRow);

                        TableCell cellRoleName = new TableCell();
                        TableCell cellRoleDescr = new TableCell();

                        TableCell idCell = new TableCell();
                        idCell.Text = useraction.ID.ToString();
                        idCell.Width = Unit.Pixel(50);
                        newRow.Cells.Add(idCell);

                        if (!useraction.ActionReference.IsLoaded)
                        {
                            useraction.ActionReference.Load();
                        }

                        cellRoleName.Text = useraction.Action.name;
                        cellRoleDescr.Text = useraction.Action.description;

                        cellRoleName.Width = Unit.Pixel(200);

                        newRow.Cells.Add(cellRoleName);
                        newRow.Cells.Add(cellRoleDescr);

                        if (isModerator == true && activatedUser == true)
                        {
                            if (useraction.Action.type != null && useraction.Action.typeID != null)
                            {
                                canDelete = false;
                            }
                            else
                            {
                                canDelete = true;
                            }
                        }

                        TableCell cellDelete = new TableCell();
                        newRow.Cells.Add(cellDelete);
                        cellDelete.Width = Unit.Pixel(10);

                        if (canDelete == true && activatedUser == true)
                        {
                            Button delBtn = new Button();
                            delBtn.ID = string.Format("RemoveUserAction{0}", useraction.ID);
                            delBtn.Text = "Remove";
                            if (!useraction.ActionReference.IsLoaded)
                            {
                                useraction.ActionReference.Load();
                            }
                            if (!useraction.UserReference.IsLoaded)
                            {
                                useraction.UserReference.Load();
                            }
                            delBtn.Attributes.Add("userID", useraction.User.ID.ToString());
                            delBtn.Attributes.Add("actionID", useraction.Action.ID.ToString());
                            delBtn.Click += new EventHandler(RemoveRole);
                            cellDelete.Controls.Add(delBtn);
                        }

                        idCell.BorderWidth = 2;
                        idCell.BorderColor = System.Drawing.Color.Black;
                        cellRoleName.BorderWidth = 2;
                        cellRoleName.BorderColor = System.Drawing.Color.Black;
                        cellRoleDescr.BorderWidth = 2;
                        cellRoleDescr.BorderColor = System.Drawing.Color.Black;
                        cellDelete.BorderWidth = 2;
                        cellDelete.BorderColor = System.Drawing.Color.Black;
                    }
                }
            }
            else
            {
                tblRoles.Visible = false;
            }
        }

        protected void AddRole(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();

            BusinessUser businessUser = new BusinessUser();
            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();
            BusinessUserActions businessUserActions = new BusinessUserActions();

            Button btn = sender as Button;
            if (btn != null)
            {
                long userID = 0;
                String user = btn.Attributes["userID"];
                long.TryParse(user, out userID);

                if (userID > 0)
                {
                    User userToAddRole = businessUser.Get(userContext, userID, true);

                    if (!userToAddRole.UserOptionsReference.IsLoaded)
                    {
                        userToAddRole.UserOptionsReference.Load();
                    }
                    if (userToAddRole.UserOptions.activated == false)
                    {
                        throw new CommonCode.UIException(string.Format("User ID : {0} cannot add role on User ID : {1}, because he is not activated."
                            , currentUser.ID, userToAddRole.ID));
                    }

                    long actionID = 0;
                    String action = btn.Attributes["actionID"];
                    long.TryParse(action, out actionID);

                    long typeActionID = 0;
                    string typeAction = btn.Attributes["typeActionID"];
                    long.TryParse(typeAction, out typeActionID);

                    if (actionID > 0)
                    {
                        Actions currAction = businessUserActions.GetAction(userContext, actionID);
                        if (currAction == null)
                        {
                            throw new CommonCode.UIException(string.Format("Theres no action id = {0} , user id = {1}", actionID, currentUser.ID));
                        }
                        businessUserActions.AddUserAction(userContext, objectContext, userToAddRole.ID, currentUser.ID, currAction.name, businessLog, true);

                        FillUserRoles(userToAddRole);
                        FillTblRolesThatUserDontHave(userToAddRole);

                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Role added!");
                    }
                    else if (typeActionID > 0)
                    {
                        TypeAction currAction = businessUserTypeActions.GetTypeAction(objectContext, typeActionID);
                        if (currAction == null)
                        {
                            throw new CommonCode.UIException(string.Format("Theres no action id = {0} , user id = {1}", actionID, currentUser.ID));
                        }


                        if (businessUserTypeActions.UnDeleteUserTypeAction(objectContext, userContext, userToAddRole, currentUser, currAction, businessLog) == true)
                        {
                            FillUserRoles(userToAddRole);
                            FillTblRolesThatUserDontHave(userToAddRole);

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Role added!");
                        }
                        else
                        {
                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                , "Type connections are not OK. You have to fix them to be able to return the right.");
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format(
                            "actionID and typeActionID are < 1 (comming from btn.Attributes['actionID' / 'typeActionID']) , user id = {0}", currentUser.ID));
                    }
                }
                else
                {
                    throw new CommonCode.UIException(string.Format("userID is < 1 (comming from btn.Attributes['userID']) , user id = {0}", currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }
        }

        protected void RemoveRole(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();

            Button btn = sender as Button;
            if (btn != null)
            {
                long userID = 0;
                String user = btn.Attributes["userID"];
                long.TryParse(user, out userID);

                if (userID > 0)
                {
                    BusinessUser businessUser = new BusinessUser();
                    BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();
                    BusinessUserActions businessUserActions = new BusinessUserActions();

                    User userToRemoveRole = businessUser.Get(userContext, userID, true);

                    if (!userToRemoveRole.UserOptionsReference.IsLoaded)
                    {
                        userToRemoveRole.UserOptionsReference.Load();
                    }
                    if (userToRemoveRole.UserOptions.activated == false)
                    {
                        throw new CommonCode.UIException(string.Format("User ID : {0} cannot remove role from User ID : {1}, because he is not activated."
                            , currentUser.ID, userToRemoveRole.ID));
                    }

                    long actionID = 0;
                    String action = btn.Attributes["actionID"];
                    long.TryParse(action, out actionID);

                    long typeActionID = 0;
                    String typeAction = btn.Attributes["typeActionID"];
                    long.TryParse(typeAction, out typeActionID);

                    if (actionID > 0)
                    {
                        UserAction userAction = businessUserActions.GetUserAction(userContext, userID, actionID);
                        businessUserActions.RemoveUserAction(userContext, objectContext, userAction, currentUser, businessLog, string.Empty);
                    }
                    else if (typeActionID > 0)
                    {
                        UsersTypeAction userAction = businessUserTypeActions.GetUserTypeAction(objectContext, userID, typeActionID, true);
                        businessUserTypeActions.RemoveUserTypeAction(objectContext, userContext, userAction, currentUser, businessLog, true);
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format
                            ("actionID and typeActionID are < 1 (comming from btn.Attributes['actionID' / 'typeActionID']) , user id = {0}", currentUser.ID));
                    }

                    FillUserRoles(userToRemoveRole);
                    FillTblRolesThatUserDontHave(userToRemoveRole);
                    FillData(userToRemoveRole);
                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Role removed!");

                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("userID is < 1 (comming from btn.Attributes['userID']) , user id = {0}", currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }
        }

        private void FillData(User currUser)
        {
            BusinessUser businessUser = new BusinessUser();

            lbDateCreated.Text = string.Format("{0} {1}", GetLocalResourceObject("DateRegistered")
                , currUser.dateCreated.ToShortDateString().ToString());

            lbUserName.Text = string.Format("{0} {1}", GetLocalResourceObject("Username"), currUser.username);

            if (!isVisited)
            {
                lblLastLogIn.Visible = false;

                if (!string.IsNullOrEmpty(currUser.email))
                {
                    lbEmail.Visible = true;
                    lbEmail.Text = string.Format("{0} {1}", GetLocalResourceObject("Email"), currUser.email);
                }
                else
                {
                    lbEmail.Visible = false;
                }
            }
            else
            {

                lblLastLogIn.Visible = true;

                lblLastLogIn.Text = string.Format("{0} {1}", GetLocalResourceObject("LastLogIn")
                    , CommonCode.UiTools.DateTimeToLocalString(currUser.lastLogIn));
                lbEmail.Visible = false;
            }

            if (businessUser.IsFromUserTeam(currUser))
            {
                FillSuggestions(currUser);
                FillWarnings(currUser);
            }
            else
            {
                apWarnings.Visible = false;
                apSuggestions.Visible = false;
            }

            if (!isVisited)
            {
                BusinessUserOptions userOptions = new BusinessUserOptions();
                if (userOptions.CheckIfUserHavesNewSystemMessages(userContext, currentUser) == true)
                {
                    userOptions.ChangeIfUserHaveNewSystemMessages(userContext, currentUser, false);
                }
                if (userOptions.CheckIfUserHavesNewWarning(userContext, currentUser) == true)
                {
                    userOptions.ChangeIfUserHaveNewWarning(userContext, currentUser, false);
                }
            }

            CheckSystemMessagesOptions();
        }

        private void CheckSystemMessagesOptions()
        {
            if (!isVisited)
            {
                FillSystemMessages();
            }
            else
            {
                apSystemMessages.Visible = false;
            }
        }

        private Boolean Visited(BusinessUser businessUser, User currUser)
        {
            Boolean result = false;

            String userParam = Request.Params["User"];
            if (userParam == null || userParam.Length == 0)
            {
                if (currUser == null)
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                        GetGlobalResourceObject("SiteResources", "errorHaveToBeLogged").ToString());
                }

                result = false;
            }
            else
            {
                long id = -1;
                long.TryParse(userParam, out id);

                if (id < 1)
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                        GetLocalResourceObject("errIncParams").ToString());
                }
                else
                {
                    User userVisited = businessUser.GetWithoutVisible(userContext, id, false);
                    if (userVisited == null)
                    {
                        CommonCode.UiTools.RedirrectToErrorPage(Response, Session
                                , GetLocalResourceObject("NoSuchUser").ToString());
                    }

                    if (!businessUser.IsUserValidType(userVisited))
                    {
                        CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                            GetLocalResourceObject("cantSeePage").ToString());
                    }

                    if (userVisited.visible == false && (currUser == null || businessUser.IsFromUserTeam(currUser) == true))
                    {
                        CommonCode.UiTools.RedirrectToErrorPage(Response, Session
                            , GetLocalResourceObject("UserDeleted").ToString());

                    }

                    if (currUser != null)
                    {
                        if (userVisited.ID == currUser.ID)
                        {
                            RedirectToOtherUrl("Profile.aspx");
                        }

                        if (businessUser.IsFromUserTeam(currUser) == true)
                        {
                            if (businessUser.IsFromUserTeam(userVisited))
                            {
                                canEdit = false;
                            }
                            else
                            {
                                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("cantSeePage").ToString());
                            }
                        }
                        else
                        {
                            canEdit = businessUser.CanAdminEditUserOrAdmin(userContext, currUser, userVisited);
                        }
                    }
                    else
                    {
                        canEdit = false;
                    }

                    result = true;
                }
            }

            return result;
        }

        private void ShowEditMembers(BusinessUser businessUser, User currUser)
        {
            ListItem passItem = new ListItem();
            passItem.Text = string.Format("{0} ", GetLocalResourceObject("password"));
            passItem.Value = "1";
            rblEditChoices.Items.Add(passItem);

            rblEditChoices.SelectedIndex = 0;

            if (businessUser.IsFromUserTeam(currUser))
            {
                if (businessUser.IsUser(currUser))
                {
                    ListItem emailItem = new ListItem();
                    emailItem.Text = string.Format("{0} ", GetLocalResourceObject("emailSmall"));
                    emailItem.Value = "2";
                    rblEditChoices.Items.Add(emailItem);
                }

                ListItem secItem = new ListItem();
                secItem.Text = string.Format("{0} ", GetLocalResourceObject("SQNA"));
                secItem.Value = "4";
                rblEditChoices.Items.Add(secItem);
            }
        }

        protected void btnShowEditPanel_Click(object sender, EventArgs e)
        {
            CheckIfUserIsModifyingHimSelf();

            int selected = -1;
            int.TryParse(rblEditChoices.SelectedValue, out selected);

            tbEdit2.Columns = 20;
            tbEdit1.Columns = 20;
            tbEdit1.TextMode = TextBoxMode.SingleLine;
            tbEdit2.TextMode = TextBoxMode.SingleLine;
            tbEdit3.TextMode = TextBoxMode.SingleLine;

            btnSubmitAction.Visible = true;
            btnDiscard.Visible = true;
            rblEditChoices.Enabled = false;

            BusinessUser businessUser = new BusinessUser();

            switch (selected)
            {
                // 1 - new pass, 2 mail, 3 signature, 4 sec question and answer
                case 1:

                    lbEdit1.Text = string.Format("{0} ", GetLocalResourceObject("CurrentPassword"));
                    lbEdit2.Text = string.Format("{0} ", GetLocalResourceObject("NewPassword"));
                    lbEdit3.Text = string.Format("{0} ", GetLocalResourceObject("RepeatNewPassword"));

                    lbEdit1.Visible = true;
                    lbEdit2.Visible = true;
                    lbEdit3.Visible = true;

                    tbEdit1.Visible = true;
                    tbEdit3.Visible = true;
                    tbEdit2.Visible = true;

                    tbEdit1.TextMode = TextBoxMode.Password;
                    tbEdit2.TextMode = TextBoxMode.Password;
                    tbEdit3.TextMode = TextBoxMode.Password;

                    lblInfo2.Visible = true;
                    lblInfo2.Text = string.Format
                    ("{0} <br />{1} {2}-{3}.", GetLocalResourceObject("passwordRules"), GetLocalResourceObject("passwordRules2")
                    , Configuration.UsersMinPasswordLength, Configuration.UsersMaxPasswordLength);

                    break;
                case 2:

                    lbEdit1.Text = string.Format("{0} ", GetLocalResourceObject("CurrentPassword"));
                    lbEdit2.Text = string.Format("{0} ", GetLocalResourceObject("newMail"));

                    tbEdit1.TextMode = TextBoxMode.Password;

                    lbEdit1.Visible = true;
                    lbEdit2.Visible = true;

                    tbEdit1.Visible = true;
                    tbEdit2.Visible = true;

                    lblInfo1.Visible = true;
                    lblInfo1.Text = string.Format("{0} : {1}", GetLocalResourceObject("currMail"), currentUser.email);

                    lblInfo2.Visible = true;
                    lblInfo2.Text = string.Format("{0}", GetLocalResourceObject("mailRules"));
                    break;
              
                case 4:
                    if (!businessUser.IsFromUserTeam(currentUser))
                    {
                        throw new CommonCode.UIException(string.Format("user id = {0}, cannot change secret question or answer", currentUser.ID));
                    }

                    tbEdit1.TextMode = TextBoxMode.Password;

                    lbEdit1.Text = string.Format("{0} ", GetLocalResourceObject("CurrentPassword"));
                    lbEdit2.Text = string.Format("{0} ", GetLocalResourceObject("NewSecretQuestion"));
                    lbEdit3.Text = string.Format("{0} ", GetLocalResourceObject("NewAnswer"));

                    lbEdit1.Visible = true;
                    lbEdit2.Visible = true;
                    lbEdit3.Visible = true;

                    tbEdit1.Visible = true;
                    tbEdit2.Visible = true;
                    tbEdit3.Visible = true;

                    tbEdit2.Columns = 30;
                    tbEdit3.Columns = 30;

                    lblInfo2.Visible = true;
                    lblInfo2.Text = GetLocalResourceObject("questionRules").ToString();

                    lblInfo3.Visible = true;
                    lblInfo3.Text = GetLocalResourceObject("answerRules").ToString();

                    if (!currentUser.UserOptionsReference.IsLoaded)
                    {
                        currentUser.UserOptionsReference.Load();
                    }

                    tbEdit2.Text = currentUser.UserOptions.secretQuestion;
                    break;
                default:
                    throw new CommonCode.UIException(string.Format
                        ("rblEditChoices.SelectedValue = {0} is invalid index , user id = {1}", selected, currentUser.ID));
            }

            pnlSelfEditSub.Visible = true;
        }

        protected void btnDiscard_Click(object sender, EventArgs e)
        {
            Discard();
        }

        private Boolean ValidateNewPassword(string pass, string repPass, out string error)
        {
            Boolean result = false;
            error = "";

            if (CommonCode.Validate.ValidatePassword(pass, out error))
            {
                if (CommonCode.Validate.ValidateRepeatPassword(currentUser.username, pass, repPass, out error))
                {
                    result = true;
                }
            }

            return result;
        }


        protected void btnSubmitAction_Click(object sender, EventArgs e)
        {
            CheckIfUserIsModifyingHimSelf();

            phEdit.Visible = true;
            phEdit.Controls.Add(lblError);
            string error = "";

            BusinessUser businessUser = new BusinessUser();
            int selected = -1;
            int.TryParse(rblEditChoices.SelectedValue, out selected);

            switch (selected)
            {
                case 1: // password

                    if (CommonCode.Validate.ValidateCurrUserPassword(currentUser, tbEdit1.Text, out error))
                    {
                        if (ValidateNewPassword(tbEdit2.Text, tbEdit3.Text, out error))
                        {
                            if (businessUser.CheckPassword(tbEdit2.Text, currentUser.password) == false)
                            {

                                businessUser.ChangeUserData(userContext, objectContext, currentUser, "password", tbEdit2.Text, businessLog);
                                Discard();

                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification,
                                    GetLocalResourceObject("PasswordChanged").ToString());

                            }
                            else
                            {
                                error = GetLocalResourceObject("errNewPassIsEqualToOld").ToString();
                            }
                        }
                    }
                    break;
                case 2: // email
                    if (CommonCode.Validate.ValidateCurrUserPassword(currentUser, tbEdit1.Text, out error))
                    {
                        if (CommonCode.Validate.ValidateEmailAdress(tbEdit2.Text, out error))
                        {
                            if (tbEdit2.Text != currentUser.email)
                            {
                                if (businessUser.CountRegisteredUsersWithMail(userContext, tbEdit2.Text)
                                    < Configuration.MaximumNumberOfUsersRegisteredWithMail)
                                {

                                    businessUser.ChangeUserData(userContext, objectContext, currentUser, "email", tbEdit2.Text, businessLog);
                                    FillData(currentUser);
                                    Discard();

                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification,
                                        GetLocalResourceObject("EmailUpdated").ToString());
                                }
                                else
                                {
                                    error = string.Format("{0} {1} {2}", GetLocalResourceObject("errEmailRegsReached")
                                        , Configuration.MaximumNumberOfUsersRegisteredWithMail
                                        , GetLocalResourceObject("errEmailRegsReached2"));
                                }
                            }
                            else
                            {
                                error = GetLocalResourceObject("errTypeNewMail").ToString(); ;
                            }
                        }
                    }
                    break;

                case 4: // secret question and answer

                    if (CommonCode.Validate.ValidateCurrUserPassword(currentUser, tbEdit1.Text, out error))
                    {
                        if (!currentUser.UserOptionsReference.IsLoaded)
                        {
                            currentUser.UserOptionsReference.Load();
                        }

                        bool pass = true;
                        bool newQuestion = false;
                        bool newAnswer = false;

                        if (tbEdit2.Text != currentUser.UserOptions.secretQuestion)
                        {
                            if (CommonCode.Validate.ValidateSecretQnA(1, 100, tbEdit2.Text))
                            {
                                newQuestion = true;
                            }
                            else
                            {
                                error = GetLocalResourceObject("errInvFormQuestion").ToString();
                                pass = false;
                            }
                        }

                        if (pass == true && !string.IsNullOrEmpty(tbEdit3.Text))
                        {
                            if (CommonCode.Validate.ValidateSecretQnA(1, 100, tbEdit3.Text))
                            {
                                newAnswer = true;
                            }
                            else
                            {
                                error = GetLocalResourceObject("errInvFormAnswer").ToString();
                                pass = false;
                            }
                        }

                        if (pass == true)
                        {
                            if (newAnswer == true || newQuestion == true)
                            {
                                BusinessUserOptions userOptions = new BusinessUserOptions();
                                if (!currentUser.UserOptionsReference.IsLoaded)
                                {
                                    currentUser.UserOptionsReference.Load();
                                }

                                if (newQuestion)
                                {
                                    userOptions.ChangeUserSecretQuestion(userContext, objectContext, currentUser.UserOptions, tbEdit2.Text, businessLog);
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification,
                                        GetLocalResourceObject("questionUpdated").ToString());
                                }

                                if (newAnswer)
                                {
                                    userOptions.ChangeUserSecretAnswer(userContext, objectContext, currentUser.UserOptions, tbEdit3.Text, businessLog);
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification,
                                        GetLocalResourceObject("answerUpdated").ToString());
                                }

                                Discard();
                            }
                            else
                            {
                                error = GetLocalResourceObject("errQNA").ToString();
                            }
                        }
                    }
                    break;
                default:
                    throw new CommonCode.UIException(string.Format
                        ("rblEditChoices.SelectedValue = {0} is invalid index , user id = {0}", selected, currentUser.ID));
            }

            lblError.Text = error;
        }

        private void Discard()
        {
            rblEditChoices.Enabled = true;

            lbEdit1.Visible = false;
            lbEdit2.Visible = false;
            lbEdit3.Visible = false;

            tbEdit1.Visible = false;
            tbEdit2.Visible = false;
            tbEdit3.Visible = false;

            btnSubmitAction.Visible = false;
            btnDiscard.Visible = false;
            phEdit.Visible = false;

            lblInfo1.Visible = false;
            lblInfo2.Visible = false;
            lblInfo3.Visible = false;

            tbEdit1.Text = "";
            tbEdit2.Text = "";
            tbEdit3.Text = "";

            pnlSelfEditSub.Visible = false;
        }

        protected void btnDeleteUser_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();

            User visited = GetVisitedUser();
            BusinessUser businessUser = new BusinessUser();
            if (visited.visible == true)
            {
                businessUser.DeleteUser(userContext, visited, businessLog, currentUser);
                CheckUserAndParams();

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "User deleted!");
            }
            else
            {
                businessUser.UnDeleteUser(userContext, objectContext, visited, businessLog, currentUser);
                CheckUserAndParams();

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "User undeleted!");
            }
        }

        public User GetVisitedUser()
        {
            User userVisited = null;

            if (Request.Params["User"] != null)
            {
                BusinessUser businessUser = new BusinessUser();
                String userVisitedStr = Request.Params["User"];
                long userVid = -1;
                long.TryParse(userVisitedStr, out userVid);
                userVisited = businessUser.GetWithoutVisible(userContext, userVid, true);
            }

            return userVisited;
        }


        [WebMethod]
        public static string CheckData(string text, string type, string notUsed)
        {
            string error = "";
            CommonCode.WebMethods.ValidateUserInput(text, type, "", out error);
            return error;
        }


        protected void btnDelOpinions_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();

            BusinessComment businessComment = new BusinessComment();
            User visitedUser = GetVisitedUser();

            bool sendWarning = cbSendWarning.Checked;
            businessComment.DeleteAllUserComments(objectContext, userContext, visitedUser, currentUser, businessLog, sendWarning);

            CheckUserAndParams();

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "All user comments deleted!");
        }

        [WebMethod]
        public static string WMSendReport(string type, string strTypeId, string description)
        {
            return CommonCode.WebMethods.SendReport(type, strTypeId, description);
        }

        private void FillWarnings(User currUser)
        {
            phWarnings.Controls.Clear();

            BusinessWarnings businessWarnings = new BusinessWarnings();
            BusinessUser businessUser = new BusinessUser();

            List<Warning> userWarnings = businessWarnings.GetUserWarnings(userContext, currUser);
            List<TypeWarning> typeWarnings = businessWarnings.GetUserTypeWarnings(objectContext, currUser);

            if (userWarnings.Count > 0 || typeWarnings.Count > 0)
            {
                apWarnings.Visible = true;

                int i = 0;

                bool canAdminDeleteWarns = false;
                if (canEdit == true && businessUser.IsGlobalAdministrator(currentUser) && businessUser.IsFromUserTeam(currUser)
                    && businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditUsers))
                {
                    canAdminDeleteWarns = true;
                }

                if (userWarnings.Count > 0)
                {
                    foreach (Warning warning in userWarnings)
                    {
                        Panel newPanel = new Panel();
                        phWarnings.Controls.Add(newPanel);
                        newPanel.CssClass = "panelRows yellowCellBgr";

                        Panel pnldate = new Panel();
                        newPanel.Controls.Add(pnldate);
                        pnldate.CssClass = "panelInline";
                        pnldate.Width = Unit.Pixel(200);

                        Label lblDate = new Label();
                        pnldate.Controls.Add(lblDate);
                        lblDate.CssClass = "commentsDate";
                        lblDate.Text = CommonCode.UiTools.DateTimeToLocalString(warning.dateCreated);

                        if (!warning.ByAdminReference.IsLoaded)
                        {
                            warning.ByAdminReference.Load();
                        }

                        Label byLbl = CommonCode.UiTools.GetLabelWithText(warning.ByAdmin.username, false);
                        newPanel.Controls.Add(byLbl);
                        byLbl.CssClass = "marginLeft";

                        if (canAdminDeleteWarns)
                        {
                            Panel pnlRight = new Panel();
                            newPanel.Controls.Add(pnlRight);
                            pnlRight.CssClass = "floatRightNoMrg";

                            DecoratedButton dbDeleteWarning = new DecoratedButton();
                            pnlRight.Controls.Add(dbDeleteWarning);
                            dbDeleteWarning.ID = string.Format("DelWarn{0}", warning.ID);
                            dbDeleteWarning.Attributes.Add("warnID", warning.ID.ToString());
                            dbDeleteWarning.Text = GetGlobalResourceObject("SiteResources", "delete").ToString();
                            dbDeleteWarning.Click += new EventHandler(dbDeleteWarning_Click);
                        }

                        Panel pnlDescription = new Panel();
                        newPanel.Controls.Add(pnlDescription);
                        pnlDescription.CssClass = "searchPageRatings";
                        pnlDescription.Controls.Add
                            (CommonCode.UiTools.GetLabelWithText(Tools.GetFormattedTextFromDB(warning.description), false));

                        i++;
                    }
                }

                if (typeWarnings.Count > 0)
                {
                    foreach (TypeWarning warning in typeWarnings)
                    {
                        Panel newPanel = new Panel();
                        phWarnings.Controls.Add(newPanel);
                        newPanel.CssClass = "panelRows yellowCellBgr";

                        Panel pnldate = new Panel();
                        newPanel.Controls.Add(pnldate);
                        pnldate.CssClass = "panelInline";
                        pnldate.Width = Unit.Pixel(200);

                        Label lblDate = new Label();
                        pnldate.Controls.Add(lblDate);
                        lblDate.CssClass = "commentsDate";
                        lblDate.Text = CommonCode.UiTools.DateTimeToLocalString(warning.dateCreated);

                        if (!warning.ByAdminReference.IsLoaded)
                        {
                            warning.ByAdminReference.Load();
                        }

                        Label byLbl = CommonCode.UiTools.GetLabelWithText
                            (businessUser.GetWithoutVisible(userContext, warning.ByAdmin.ID, true).username, false);
                        newPanel.Controls.Add(byLbl);
                        byLbl.CssClass = "marginLeft";

                        if (canAdminDeleteWarns)
                        {
                            Panel pnlRight = new Panel();
                            newPanel.Controls.Add(pnlRight);
                            pnlRight.CssClass = "floatRightNoMrg";

                            DecoratedButton dbDeleteTypeWarning = new DecoratedButton();
                            pnlRight.Controls.Add(dbDeleteTypeWarning);
                            dbDeleteTypeWarning.ID = string.Format("DelTypeWarn{0}", warning.ID);
                            dbDeleteTypeWarning.Attributes.Add("typeWarnID", warning.ID.ToString());
                            dbDeleteTypeWarning.Text = GetGlobalResourceObject("SiteResources", "delete").ToString();
                            dbDeleteTypeWarning.Click += new EventHandler(dbDeleteTypeWarning_Click);
                        }

                        Panel pnlDescription = new Panel();
                        newPanel.Controls.Add(pnlDescription);
                        pnlDescription.CssClass = "searchPageRatings";
                        pnlDescription.Controls.Add
                            (CommonCode.UiTools.GetLabelWithText(Tools.GetFormattedTextFromDB(warning.description), false));

                        i++;
                    }
                }

            }
            else
            {
                apWarnings.Visible = false;
            }


        }

        void dbDeleteTypeWarning_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();

            Button btnDelWarn = sender as Button;
            if (btnDelWarn != null)
            {
                long warnId = -1;
                string warnIdStr = btnDelWarn.Attributes["typeWarnID"];
                if (long.TryParse(warnIdStr, out warnId))
                {
                    BusinessWarnings bWarnings = new BusinessWarnings();
                    TypeWarning warning = bWarnings.GetUserTypeWarning(objectContext, warnId, true, false);

                    if (warning == null)
                    {
                        return;
                    }

                    bWarnings.DeleteTypeWarning(userContext, objectContext, warning, visitedUser, currentUser, businessLog);

                    FillWarnings(visitedUser);

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("WarningDeleted").ToString());
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("Couldnt parse DelTypeWarn.Attributes['typeWarnID'] = {0} to long , user id = {1}", warnIdStr, currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("coultn get Button");
            }
        }

        void dbDeleteWarning_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();

            Button btnDelWarn = sender as Button;
            if (btnDelWarn != null)
            {
                long warnId = -1;
                string warnIdStr = btnDelWarn.Attributes["warnID"];
                if (long.TryParse(warnIdStr, out warnId))
                {
                    BusinessWarnings bWarnings = new BusinessWarnings();
                    Warning warning = bWarnings.GetUserWarning(userContext, warnId, true, false);

                    if (warning == null)
                    {
                        return;
                    }

                    bWarnings.DeleteWarning(userContext, warning, visitedUser, currentUser, businessLog);

                    FillWarnings(visitedUser);

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("WarningDeleted").ToString());
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("Couldnt parse dbDeleteWarning.Attributes['warnID'] = {0} to long , user id = {1}", warnIdStr, currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("coultn get Button");
            }
        }

        protected void btnRemoveUserSignature_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();

            BusinessUser bUser = new BusinessUser();

            if (!bUser.CanUserDo(userContext, visitedUser, UserRoles.HaveSignature))
            {
                throw new CommonCode.UIException(string.Format("Admin id : {0} cannot remove signature of user id : {1}, because he cannto have one."
                    , currentUser.ID, visitedUser.ID));
            }

            bUser.RemoveUserSignature(userContext, objectContext, visitedUser, businessLog, currentUser);

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Signature removed!");
            ShowRemoveSignatureOption(bUser, visitedUser);
            FillData(visitedUser);
        }

        private void ShowChangeNameOption(BusinessUser bUser, User forUser)
        {
            if (bUser.IsFromUserTeam(forUser) == true)
            {
                if (!forUser.UserOptionsReference.IsLoaded)
                {
                    forUser.UserOptionsReference.Load();
                }

                BusinessUserOptions buOptions = new BusinessUserOptions();
                if (buOptions.CheckIfUserHaveToChangeName(forUser.UserOptions) == false)
                {
                    lblChangeUserNameInfo.Text = string.Format("Set to {0} that he needs to change name next time he log in ?", forUser.username);
                    btnSetChangeNameToUser.Text = "Change";
                }
                else
                {
                    lblChangeUserNameInfo.Text = string.Format("Currently {0} have to change name when he log in nex time. Remove change of name ?", forUser.username);
                    btnSetChangeNameToUser.Text = "Remove";
                }
            }
            else
            {
                lblChangeUserNameInfo.Visible = false;
                btnSetChangeNameToUser.Visible = false;
            }
        }

        protected void btnSetChangeNameToUser_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();

            BusinessUser bUser = new BusinessUser();

            if (!bUser.IsFromUserTeam(visitedUser))
            {
                throw new CommonCode.UIException(string.Format("Admin id : {0} cannot modify useroptions.changeName to Admin ID : {1} , only to users this options can be used."
                    , currentUser.ID, visitedUser.ID));
            }

            if (!visitedUser.UserOptionsReference.IsLoaded)
            {
                visitedUser.UserOptionsReference.Load();
            }

            BusinessUserOptions buOptions = new BusinessUserOptions();
            bool changeName = !buOptions.CheckIfUserHaveToChangeName(visitedUser.UserOptions);
            buOptions.ChangeIfUserNeedToChangeName(userContext, objectContext, visitedUser, changeName, currentUser, businessLog);

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification,
                string.Format("Change name on {0} set to {1}!", visitedUser.username, changeName));

            ShowChangeNameOption(bUser, visitedUser);
        }

        protected void btnActivateUser_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllUsersFromEvents();

            BusinessUser bUser = new BusinessUser();
            if (bUser.IsFromUserTeam(visitedUser) == false)
            {
                throw new CommonCode.UIException(string.Format("User ID : {0} cannot activate user id : {1}, because he is not from user team."
                    , currentUser.ID, visitedUser.ID));
            }

            if (!visitedUser.UserOptionsReference.IsLoaded)
            {
                visitedUser.UserOptionsReference.Load();
            }

            BusinessUserOptions buOptions = new BusinessUserOptions();
            if (buOptions.IsUserActivated(visitedUser.UserOptions) == true)
            {
                return;
            }

            buOptions.SetAccountAsActivated(userContext, objectContext, visitedUser.UserOptions, businessLog);

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "User activated!");

            ShowActivateOptions(bUser, visitedUser);

        }

    }
}
