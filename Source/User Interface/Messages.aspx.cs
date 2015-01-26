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

using DataAccess;
using BusinessLayer;
using CustomServerControls;

namespace UserInterface
{
    public partial class Messages : BasePage
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
            tbMsgTo.Attributes.Add("onblur", string.Format("JSCheckData('{0}','usernameFound','{1}','');", tbMsgTo.ClientID, lblCName.ClientID));
            tbBlockUser.Attributes.Add("onkeyup", string.Format("JSCheckData('{0}','usernameFound','{1}','');", tbBlockUser.ClientID, lblCBName.ClientID));
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

            lblCName.Text = string.Empty;
            lblCBName.Text = string.Empty;

            BusinessMessages businessMessages = new BusinessMessages();
            BusinessUser businessUser = new BusinessUser();

            if (businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages) == true)
            {
                pnlMessages.Visible = true;
            }
            else
            {
                pnlMessages.Visible = false;
            }

            if (businessUser.IsFromAdminTeam(currentUser))
            {
                lblBlockMsgs.Visible = false;
                btnBlockAllMsgs.Visible = false;
                btnBlockUser.Visible = false;
                tbBlockUser.Visible = false;
                lblBlockUser.Visible = false;

                apBlockedUsers.Visible = false;
            }
            else
            {
                if (businessMessages.CountBlockedUsers(userContext, currentUser) > 0)
                {
                    lblBlockedUsers.Text = GetLocalResourceObject("BlockedUsers").ToString();
                    pnlShowBlockedUsers.CssClass = "accordionHeaders";
                    tblBlockedUsers.Visible = true;
                    FillTblBlockedUsers();
                }
                else
                {
                    lblBlockedUsers.Text = GetLocalResourceObject("NoBlockedUsers").ToString();
                    pnlShowBlockedUsers.CssClass = "accordionHeadersNoCursor";
                    tblBlockedUsers.Visible = false;
                }
            }

            BusinessUserOptions userOptions = new BusinessUserOptions();
            if (userOptions.CheckIfUserHavesNewMessages(userContext, currentUser) == true)
            {
                userOptions.ChangeIfUserHaveNewMessages(userContext, currentUser, false);
            }

            if (ApplicationVariantString == "bg")
            {
                btnTransliterateMessage.Visible = true;
                btnTransliterateMessage.TargetTextBox = tbMsgDescr;
            }
            else
            {
                btnTransliterateMessage.Visible = false;
            }

            CheckMessageBox();
            FillMessages();
            ShowAdvertisements();

            SetLocalText();
        }

        private void SetLocalText()
        {
            lblWriteMessage.Text = GetLocalResourceObject("lblWriteMessage").ToString();
            lblMsgTo.Text = GetLocalResourceObject("lblTo").ToString();
            lblMsgSubject.Text = GetLocalResourceObject("lblSubject").ToString();
            cbMsgSave.Text = GetLocalResourceObject("cbSaveInSent").ToString();
            lblMsg.Text = GetLocalResourceObject("lblMessage").ToString();
            btnSendMessage.Text = GetLocalResourceObject("SendMsg").ToString();

            lblBlockUser.Text = GetLocalResourceObject("BlockUser").ToString();
            btnBlockUser.Text = GetLocalResourceObject("Block").ToString();
        }

        private void FillMessages()
        {
            phMessages.Controls.Clear();

            BusinessUser businessUser = new BusinessUser();
            BusinessMessages businessMessages = new BusinessMessages();

            List<Message> ReceivedMessages = businessMessages.GetReceivedMessages(objectContext, currentUser).ToList();
            List<Message> SentMessaged = businessMessages.GetSentMessages(currentUser).ToList();

            List<long> adminIDs = businessUser.AdminsIDsList(objectContext);
            int i = 0;

            Panel paddPanel = new Panel();
            phMessages.Controls.Add(paddPanel);
            paddPanel.CssClass = "paddingLR4";

            // received messages
            if (ReceivedMessages.Count<Message>() > 0)
            {
                Panel recMessages = new Panel();
                paddPanel.Controls.Add(recMessages);
                recMessages.CssClass = "sectionTextHeader";
                recMessages.Controls.Add(CommonCode.UiTools.GetLabelWithText
                    (GetLocalResourceObject("ReceivedMessages").ToString(), false));

                i = 0;

                foreach (Message message in ReceivedMessages)
                {
                    Panel newPanel = new Panel();
                    paddPanel.Controls.Add(newPanel);

                    newPanel.CssClass = "panelRows blueCellBgr";

                    Table recTable = new Table();
                    newPanel.Controls.Add(recTable);
                    recTable.Width = Unit.Percentage(100);

                    TableRow firstRow = new TableRow();
                    recTable.Rows.Add(firstRow);

                    if (!message.FromUserReference.IsLoaded)
                    {
                        message.FromUserReference.Load();
                    }
                    firstRow.Attributes.Add("msgID", message.ID.ToString());

                    TableCell fromCell = new TableCell();
                    firstRow.Cells.Add(fromCell);
                    
                    fromCell.Width = Unit.Pixel(250);
                    fromCell.VerticalAlign = VerticalAlign.Top;

                    if (message.FromUser.visible == true)
                    {
                        if (adminIDs.Contains(message.FromUser.ID))
                        {
                            Label adminLbl = CommonCode.UiTools.GetAdminLabel(message.FromUser.username);
                            fromCell.Controls.Add(adminLbl);
                            adminLbl.CssClass = "userNames";
                        }
                        else
                        {
                            HyperLink fromLink = CommonCode.UiTools.GetUserHyperLink(message.FromUser);
                            fromCell.Controls.Add(fromLink);
                            fromLink.CssClass = "userNames";
                        }
                    }
                    else
                    {
                        fromCell.CssClass = "userNames";
                        fromCell.Text = message.FromUser.username;
                    }

                    TableCell dateCell = new TableCell();
                    dateCell.VerticalAlign = VerticalAlign.Top;
                    dateCell.Width = Unit.Pixel(150);
                    dateCell.CssClass = "commentsDate";
                    dateCell.Text = message.dateCreated.ToString("G", DateTimeFormatInfo.InvariantInfo);
                    firstRow.Cells.Add(dateCell);

                    TableCell delCell = new TableCell();
                    delCell.VerticalAlign = VerticalAlign.Top;
                    delCell.HorizontalAlign = HorizontalAlign.Right;

                    DecoratedButton delMsg = new DecoratedButton();
                    delMsg.ID = string.Format("DeleteRecMsg{0}", message.ID);
                    delMsg.Text = GetLocalResourceObject("Delete").ToString();
                    delMsg.Click += new EventHandler(DeleteMessage_Click);
                    delCell.Controls.Add(delMsg);
                    firstRow.Cells.Add(delCell);
 
                    Label lblSugestion = new Label();
                    newPanel.Controls.Add(lblSugestion);
                    lblSugestion.Text = string.Format("{0}<br />", message.subject);
                    lblSugestion.CssClass = "searchPageComments";

                    Label lblDescr = new Label();
                    newPanel.Controls.Add(lblDescr);
                    lblDescr.Text = Tools.GetFormattedTextFromDB(message.description);

                    i++;
                }
            }
            else
            {
                Panel recMessages = new Panel();
                paddPanel.Controls.Add(recMessages);
                recMessages.HorizontalAlign = HorizontalAlign.Center;
                recMessages.CssClass = "sectionTextHeader";
                recMessages.Controls.Add(CommonCode.UiTools.GetLabelWithText
                    (GetLocalResourceObject("ReceivedMessagesNone").ToString(), false));
            }

            // sent messages
            if (SentMessaged.Count<Message>() > 0)
            {
                Panel recMessages = new Panel();
                paddPanel.Controls.Add(recMessages);
                recMessages.CssClass = "sectionTextHeader";
                recMessages.Controls.Add(CommonCode.UiTools.GetLabelWithText
                    (GetLocalResourceObject("SentMessages").ToString(), false));

                i = 0;

                foreach (Message message in SentMessaged)
                {
                    Panel newPanel = new Panel();
                    paddPanel.Controls.Add(newPanel);
                    newPanel.CssClass = "panelRows greenCellBgr";

                    Table sentTable = new Table();
                    newPanel.Controls.Add(sentTable);
                    sentTable.Width = Unit.Percentage(100);

                    TableRow firstRow = new TableRow();
                    sentTable.Rows.Add(firstRow);

                    if (!message.ToUserReference.IsLoaded)
                    {
                        message.ToUserReference.Load();
                    }
                    firstRow.Attributes.Add("msgID", message.ID.ToString());

                    TableCell toCell = new TableCell();
                    firstRow.Cells.Add(toCell);

                    toCell.VerticalAlign = VerticalAlign.Top;
                    toCell.Width = Unit.Pixel(250);
                   
                    if (message.ToUser.visible == true)
                    {
                        HyperLink userLink = CommonCode.UiTools.GetUserHyperLink(message.ToUser);
                        userLink.CssClass = "userNames";
                        toCell.Controls.Add(userLink);
                    }
                    else
                    {
                        toCell.CssClass = "userNames";
                        toCell.Text = message.ToUser.username;
                    }

                    TableCell dateCell = new TableCell();
                    dateCell.VerticalAlign = VerticalAlign.Top;
                    dateCell.Width = Unit.Pixel(150);
                    dateCell.CssClass = "commentsDate";
                    dateCell.Text = message.dateCreated.ToString("G", DateTimeFormatInfo.InvariantInfo);
                    firstRow.Cells.Add(dateCell);

                    TableCell delCell = new TableCell();
                    delCell.VerticalAlign = VerticalAlign.Top;
                    delCell.HorizontalAlign = HorizontalAlign.Right;
                    DecoratedButton delMsg = new DecoratedButton();
                    delMsg.ID = string.Format("DeleteSentMsg{0}", message.ID);
                    delMsg.Text = GetLocalResourceObject("Delete").ToString();
                    delMsg.Click += new EventHandler(DeleteMessage_Click);
                    delCell.Controls.Add(delMsg);
                    firstRow.Cells.Add(delCell);

                    Label lblSugestion = new Label();
                    newPanel.Controls.Add(lblSugestion);
                    lblSugestion.Text = string.Format("{0}<br />", message.subject);
                    lblSugestion.CssClass = "searchPageComments";

                    Label lblDescr = new Label();
                    newPanel.Controls.Add(lblDescr);
                    lblDescr.Text = Tools.GetFormattedTextFromDB(message.description);

                    i++;
                }
            }
            else
            {
                Panel recMessages = new Panel();
                paddPanel.Controls.Add(recMessages);
                recMessages.HorizontalAlign = HorizontalAlign.Center;
                recMessages.CssClass = "sectionTextHeader";
                recMessages.Controls.Add(CommonCode.UiTools.GetLabelWithText
                    (GetLocalResourceObject("SentMessagesNone").ToString(), false));
            }


        }


        private void FillTblBlockedUsers()
        {
            tblBlockedUsers.Rows.Clear();

            BusinessUser businessUser = new BusinessUser();
            BusinessMessages businessMessages = new BusinessMessages();

            IEnumerable<UsersBlocked> Blocked = businessMessages.GetBlockedUsers(currentUser);

            if (Blocked.Count<UsersBlocked>() > 0)
            {
                foreach (UsersBlocked user in Blocked)
                {
                    if (!user.BlockedUserReference.IsLoaded)
                    {
                        user.BlockedUserReference.Load();
                    }

                    TableRow newRow = new TableRow();
                    newRow.Attributes.Add("blockedID", user.BlockedUser.ID.ToString());

                    TableCell userCell = new TableCell();
                    HyperLink userLink = CommonCode.UiTools.GetUserHyperLink(user.BlockedUser);
                    userLink.CssClass = "userNames searchPageRatings";
                    userLink.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, "5px;");
                    userCell.Controls.Add(userLink);
                    userCell.Width = Unit.Pixel(300);
                    newRow.Cells.Add(userCell);

                    TableCell dateCell = new TableCell();
                    dateCell.CssClass = "commentsDate";
                    dateCell.Text = CommonCode.UiTools.DateTimeToLocalString(user.dateCreated);
                    newRow.Cells.Add(dateCell);

                    TableCell unbCell = new TableCell();
                    unbCell.Width = Unit.Pixel(100);
                    unbCell.HorizontalAlign = HorizontalAlign.Right;
                    DecoratedButton unbBtn = new DecoratedButton();
                    unbBtn.ID = string.Format("Unblock{0}", user.BlockedUser.ID);
                    unbBtn.Text = GetLocalResourceObject("Unblock").ToString();
                    unbBtn.Click += new EventHandler(UnblockUser_Click);
                    unbCell.Controls.Add(unbBtn);
                    newRow.Cells.Add(unbCell);

                    tblBlockedUsers.Rows.Add(newRow);
                }

            }
            else
            {
                TableRow lastRow = new TableRow();
                TableCell lastCell = new TableCell();
                lastCell.Text = GetLocalResourceObject("NoBlockedUsers").ToString();
                lastRow.Cells.Add(lastCell);
                tblBlockedUsers.Rows.Add(lastRow);
            }
        }

        void DeleteMessage_Click(object sender, EventArgs e)
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
                        long msgID = -1;
                        string msgIdStr = tblRow.Attributes["msgID"];
                        if (long.TryParse(msgIdStr, out msgID))
                        {
                            BusinessMessages businessMessages = new BusinessMessages();
                            Message message = businessMessages.GetMessage(userContext, msgID);
                            if (message == null)
                            {
                                throw new CommonCode.UIException(string.Format("Theres no Message ID = {0} , user id = {1}", msgID, currentUser.ID));
                            }

                            businessMessages.DeleteReceivedOrSentMessage(userContext, message.ID, currentUser);
                            ShowInfo();

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                , GetLocalResourceObject("MessageDeleted").ToString());
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['msgID'] = {0} to long , user id = {1}", msgIdStr, currentUser.ID));
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

      
        void BlockUser_Click(object sender, EventArgs e)
        {
            Button btnBlock = sender as Button;
            if (btnBlock != null)
            {
                TableCell tblCell = btnBlock.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long msgID = -1;
                        string msgIdStr = tblRow.Attributes["msgID"];
                        if (long.TryParse(msgIdStr, out msgID))
                        {
                            BusinessUser businessUser = new BusinessUser();
                            BusinessMessages businessMessages = new BusinessMessages();

                            Message message = businessMessages.GetMessage(userContext, msgID);
                            if (message == null)
                            {
                                throw new CommonCode.UIException(string.Format("Theres no Message ID = {0} , user id = {1}", msgID, currentUser.ID));
                            }

                            businessMessages.BlockUserFromMessage(userContext, currentUser, message);
                            ShowInfo();

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                , GetLocalResourceObject("UserBlocked").ToString());
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['msgID'] = {0} to long , user id = {1}", msgIdStr, currentUser.ID));
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


        void UnBlockUserFromMessage_Click(object sender, EventArgs e)
        {
            Button btnUnBlock = sender as Button;
            if (btnUnBlock != null)
            {
                TableCell tblCell = btnUnBlock.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long msgID = -1;
                        string msgIdStr = tblRow.Attributes["msgID"];
                        if (long.TryParse(msgIdStr, out msgID))
                        {
                            BusinessUser businessUser = new BusinessUser();
                            BusinessMessages businessMessages = new BusinessMessages();

                            Message message = businessMessages.GetMessage(userContext, msgID);
                            if (message == null)
                            {
                                throw new CommonCode.UIException(string.Format("Theres no Message ID = {0} , user id = {1}", msgID, currentUser.ID));
                            }

                            businessMessages.UnBlockUserFromMessage(userContext, currentUser, message);
                            ShowInfo();

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                , GetLocalResourceObject("UserUnblocked").ToString());
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['msgID'] = {0} to long , user id = {1}", msgIdStr, currentUser.ID));
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

        void UnblockUser_Click(object sender, EventArgs e)
        {
            Button btnUnblock = sender as Button;
            if (btnUnblock != null)
            {
                TableCell tblCell = btnUnblock.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long blockedID = -1;
                        string blockedIdStr = tblRow.Attributes["blockedID"];
                        if (long.TryParse(blockedIdStr, out blockedID))
                        {
                            BusinessUser businessUser = new BusinessUser();
                            BusinessMessages businessMessages = new BusinessMessages();

                            User userBlocked = businessUser.GetWithoutVisible(userContext, blockedID, true);

                            if (businessMessages.IsUserBlocking(userContext, currentUser.ID, userBlocked.ID))
                            {
                                businessMessages.UnblockUser(userContext, currentUser, userBlocked.ID);
                                ShowInfo();

                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                    , GetLocalResourceObject("UserUnblocked").ToString());
                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format
                                    ("The User ID = {0} isnt blocked , user id = {1}", userBlocked.ID, currentUser.ID));
                            }

                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse tblRow.Attributes['blockedID'] = {0} to long , user id = {1}", blockedIdStr, currentUser.ID));
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

        private void CheckUserAndParams()
        {
            User currUser = GetCurrentUser(userContext, objectContext);
            currentUser = currUser;

            if (currUser == null)
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errLogged").ToString());
            }
        }

        private void CheckMessageBox()
        {
            BusinessUserOptions businessUserOptions = new BusinessUserOptions();

            if (businessUserOptions.CanUserReceiveMessages(userContext, currentUser) == true)
            {
                lblBlockMsgs.Text = GetLocalResourceObject("BlockAllMsgs").ToString();
                btnBlockAllMsgs.Text = GetLocalResourceObject("Block").ToString();
            }
            else
            {
                lblBlockMsgs.Text = GetLocalResourceObject("UnblockMessageBox").ToString();
                btnBlockAllMsgs.Text = GetLocalResourceObject("Unblock").ToString();
            }
        }

        private void ShowAdvertisements()
        {
            if (Configuration.AdvertsNumAdvertsOnUserPage > 0)
            {
                phAdverts.Controls.Clear();
                adCell.Attributes.Clear();

                phAdverts.Controls.Add(CommonCode.ImagesAndAdverts.GetAdvertisements
                    (objectContext, Server, "general", 1, Configuration.AdvertsNumAdvertsOnMessagesPage));

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

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
            {
                throw new CommonCode.UIException(string.Format("User ID = {0} cannot send private messages", currentUser.ID));
            }

            phMessage.Visible = true;
            phMessage.Controls.Add(lblError);
            string error = "";

            BusinessMessages businessMessages = new BusinessMessages();

            if (CommonCode.Validate.ValidateMessageToUser(businessUser, businessMessages, currentUser, tbMsgTo.Text, out error))
            {
                string subject = tbMsgSubject.Text;
                string description = tbMsgDescr.Text;

                if (CommonCode.Validate.ValidateDescription(0, Configuration.MessagesMaxSubjectLength, ref subject, "subject", out error, 90))
                {
                    if (CommonCode.Validate.ValidateDescription(1, Configuration.CommentsMaxCommentDescriptionLength, ref description,
                        "description", out error, 80))
                    {
                        User ToUser = businessUser.GetByName(userContext, tbMsgTo.Text, false, true);
                        if (ToUser == null)
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Theres no User Name = {0} , to who needs to be sent message , user id = {1}", tbMsgTo.Text, currentUser.ID));
                        }

                        Message newMessage = new Message();
                        newMessage.FromUser = currentUser;
                        newMessage.ToUser = ToUser;
                        if (string.IsNullOrEmpty(subject))
                        {
                            newMessage.subject = GetLocalResourceObject("NoSubject").ToString();
                        }
                        else
                        {
                            newMessage.subject = subject;
                        }
                        newMessage.description = description;
                        if (cbMsgSave.Checked)
                        {
                            newMessage.visibleFromUser = true;
                        }
                        else
                        {
                            newMessage.visibleFromUser = false;
                        }
                        newMessage.visibleToUser = true;
                        newMessage.dateCreated = DateTime.UtcNow;

                        businessMessages.Add(userContext, newMessage);

                        ShowInfo();
                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                            , GetLocalResourceObject("MessageSent").ToString());

                        phMessage.Visible = false;
                        tbMsgDescr.Text = "";
                        tbMsgSubject.Text = "";
                        tbMsgTo.Text = "";
                        cbMsgSave.Checked = false;

                    }
                }
            }

            lblError.Text = error;
        }

        protected void btnBlockAllMsgs_Click(object sender, EventArgs e)
        {
            BusinessUserOptions businessUserOptions = new BusinessUserOptions();

            if (businessUserOptions.CanUserReceiveMessages(userContext, currentUser))
            {
                businessUserOptions.ChangeCanUserReceiveMessages(userContext, currentUser, false);
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                    , GetLocalResourceObject("BoxBlocked").ToString());
            }
            else
            {
                businessUserOptions.ChangeCanUserReceiveMessages(userContext, currentUser, true);
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                    , GetLocalResourceObject("BoxUnblocked").ToString());
            }

            CheckMessageBox();
        }

        [WebMethod]
        public static string CheckData(string text, string type, string notUsed)
        {
            string error = "";
            CommonCode.WebMethods.ValidateUserInput(text, type, "", out error);
            return error;
        }

        protected void btnBlockUser_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            BusinessMessages businessMessages = new BusinessMessages();

            phBlockUser.Visible = true;
            phBlockUser.Controls.Add(lblError);
            string error = "";

            if (ValidateUserToBlockName(businessMessages, currentUser, tbBlockUser.Text, out error))
            {
                User userToBlock = businessUser.GetByName(userContext, tbBlockUser.Text, false, true);
                if (userToBlock == null)
                {
                    throw new BusinessException(string.Format("Theres no UserToBlock Name = {0} , user id = {1}", tbBlockUser.Text, currentUser.ID));
                }

                businessMessages.BlockUser(userContext, currentUser, userToBlock.ID);

                phBlockUser.Visible = false;
                tbBlockUser.Text = "";

                ShowInfo();
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("UserBlocked").ToString());
            }

            lblError.Text = error;
        }

        private Boolean ValidateUserToBlockName(BusinessMessages businessMessages, User currUser, String username, out string error)
        {
            Boolean passed = false;
            error = "";

            if (Tools.NullValidatorPassed(username))
            {
                if (Tools.StringRangeValidatorPassed(Configuration.UsersMinUserNameLength, 50, username))
                {
                    BusinessUser businessUser = new BusinessUser();
                    User user = businessUser.GetByName(userContext, username, false, true);
                    if (user != null)
                    {
                        if (businessMessages.CanUserBlockUser(userContext, currUser.ID, user.ID, out error))
                        {
                            passed = true;
                        }
                    }
                    else
                    {
                        error = GetLocalResourceObject("errNoUser").ToString();
                    }
                }
                else
                {
                    error = GetLocalResourceObject("errNameLength").ToString();
                }

            }
            else
            {
                error = GetLocalResourceObject("errTypeName").ToString();
            }


            return passed;
        }

    }
}
