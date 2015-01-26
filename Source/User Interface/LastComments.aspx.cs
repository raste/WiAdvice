﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.UI.WebControls;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class LastComments : BasePage
    {
        private User currentUser = null;

        private string ipAdress = string.Empty;
        private int commNumber = 0;
        private int maxLength = 300;
        private bool all = true;
        private bool visible = true;

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
            Title = "Last comments";

            object objIpAdress = Session["lcIpAdress"];
            if (objIpAdress != null)
            {
                ipAdress = objIpAdress.ToString();
            }

            object objNumComm = Session["lcNumber"];
            if (objNumComm != null)
            {
                if (!int.TryParse(objNumComm.ToString(), out commNumber))
                {
                    throw new CommonCode.UIException("couldn`t parse Session['lcNumber'] to int.");
                }
            }

            object objMaxLength = Session["lcMaxLength"];
            if (objMaxLength != null)
            {
                if (!int.TryParse(objMaxLength.ToString(), out maxLength))
                {
                    throw new CommonCode.UIException("couldn`t parse Session['lcMaxLength'] to int.");
                }
            }

            object objAll = Session["lcAll"];
            if (objAll != null)
            {
                if (!bool.TryParse(objAll.ToString(), out all))
                {
                    throw new CommonCode.UIException("couldn`t parse Session['lcAll'] to bool.");
                }
            }

            object objVisible = Session["lcVisible"];
            if (objVisible != null)
            {
                if (!bool.TryParse(objVisible.ToString(), out visible))
                {
                    throw new CommonCode.UIException("couldn`t parse Session['lcVisible'] to int.");
                }
            }

            FillComments();
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

        protected void btnShow_Click(object sender, EventArgs e)
        {
            lblError.Visible = true;

            string strNumber = tbNumber.Text;
            int number = 0;

            if (string.IsNullOrEmpty(strNumber))
            {
                lblError.Text = "Type number of last comments which should display.";
                return;
            }
            else if (!int.TryParse(strNumber, out number))
            {
                lblError.Text = "Incorrect number format.";
                return;
            }

            if (number < 1 || number > 1000)
            {
                lblError.Text = "Number must be between 1 and 1000.";
                return;
            }

            string strMaxLength = tbMaxLength.Text;
            int maxShowLength = 0;

            if (string.IsNullOrEmpty(strMaxLength))
            {
                lblError.Text = "Type number of maximum comment legth.";
                return;
            }
            else if (!int.TryParse(strMaxLength, out maxShowLength))
            {
                lblError.Text = "Incorrect number format.";
                return;
            }

            if (maxShowLength < 1)
            {
                lblError.Text = "Number must be bigger than 0.";
                return;
            }

            string strIpAdress = tbIpAdress.Text;

            lblError.Visible = false;

            Session["lcNumber"] = number;
            Session["lcMaxLength"] = maxShowLength;
            Session["lcIpAdress"] = strIpAdress;

            switch (rblVisibility.SelectedIndex)
            {
                case 0:
                    Session["lcAll"] = "true";
                    Session["lcVisible"] = "true";

                    all = true;
                    visible = true;
                    break;
                case 1:
                    Session["lcAll"] = "false";
                    Session["lcVisible"] = "true";

                    all = false;
                    visible = true;
                    break;
                case 2:
                    Session["lcAll"] = "false";
                    Session["lcVisible"] = "false";

                    all = false;
                    visible = false;
                    break;
                default:
                    throw new CommonCode.UIException(string.Format("rblVisibility.SelectedIndex = {0}, is not valid index, user id : {1}"
                        , rblVisibility.SelectedIndex, currentUser.ID));
            }


            ipAdress = strIpAdress;
            commNumber = number;
            maxLength = maxShowLength;


            FillComments();
        }

        private void FillComments()
        {
            phComments.Controls.Clear();

            if (commNumber > 0)
            {
                if (maxLength < 1)
                {
                    throw new CommonCode.UIException("maxLength < 1");
                }

                BusinessComment businessComment = new BusinessComment();
                List<Comment> lastComments = businessComment.GetLastComments(objectContext, commNumber, maxLength, ipAdress, all, visible);
                if (lastComments.Count > 0)
                {
                    pnlResults.Visible = true;
                    lblResults.CssClass = "";
                    lblResults.Text = "Results :";

                    BusinessUser businessUser = new BusinessUser();
                    BusinessProduct businessProduct = new BusinessProduct();
                    User guest = businessUser.GetGuest();

                    bool canEdit = false;
                    if (businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
                    {
                        canEdit = true;
                    }

                    int i = 0;

                    foreach (Comment comment in lastComments)
                    {
                        Panel newPanel = new Panel();
                        phComments.Controls.Add(newPanel);
                        newPanel.CssClass = "panelRows";

                        if (i % 2 == 0)
                        {
                            newPanel.BackColor = CommonCode.UiTools.GetStandardCellBgrColor();
                        }
                        else
                        {
                            newPanel.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                        }

                        if (!comment.UserIDReference.IsLoaded)
                        {
                            comment.UserIDReference.Load();
                        }

                        Panel usrPnl = new Panel();
                        newPanel.Controls.Add(usrPnl);
                        usrPnl.CssClass = "panelInline";
                        usrPnl.Width = Unit.Pixel(250);

                        User commentUser = Tools.GetUserFromUserDatabase(userContext, comment.UserID);

                        if (commentUser.ID != guest.ID)
                        {
                            usrPnl.Controls.Add(CommonCode.UiTools.GetUserHyperLink(commentUser));
                        }
                        else
                        {
                            usrPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("{0} |guest", comment.guestname), false));
                        }

                        Panel datePnl = new Panel();
                        newPanel.Controls.Add(datePnl);
                        datePnl.CssClass = "panelInline";
                        datePnl.Width = Unit.Pixel(170);

                        Label lblDate = new Label();
                        datePnl.Controls.Add(lblDate);
                        lblDate.Text = CommonCode.UiTools.DateTimeToLocalString(comment.dateCreated);
                        lblDate.CssClass = "commentsDate";

                        Label lblAbout = new Label();
                        lblAbout.Text = CommonCode.UiTools.GetCommentAbout(userContext, objectContext, businessComment
                            , comment, currentUser);

                        newPanel.Controls.Add(lblAbout);
                        newPanel.Controls.Add(CommonCode.UiTools.GetCommentIn(objectContext, businessComment
                            , comment, pnlPopUp, currentUser));

                        Panel description = new Panel();
                        newPanel.Controls.Add(description);
                        description.Controls.Add(CommonCode.UiTools.GetLabelWithText(Tools.GetFormattedTextFromDB(comment.description), false));

                        if (comment.visible == true && canEdit == true)
                        {
                            Panel btnPanel = new Panel();
                            newPanel.Controls.Add(btnPanel);

                            btnPanel.HorizontalAlign = HorizontalAlign.Right;

                            Button delBtn = new Button();
                            btnPanel.Controls.Add(delBtn);
                            delBtn.ID = string.Format("comm{0}", comment.ID);
                            delBtn.Attributes.Add("ID", comment.ID.ToString());
                            delBtn.Text = "Delete";
                            delBtn.Click += new EventHandler(delBtn_Click);
                        }

                        i++;
                    }
                }
                else
                {
                    pnlResults.Visible = true;
                    lblResults.CssClass = "searchPageRatings";
                    lblResults.Text = "No found comments for theese criterias.";

                    Session["lcNumber"] = null;
                    Session["lcMaxLength"] = null;
                    Session["lcIpAdress"] = null;

                }
            }
            else
            {

            }
        }

        void delBtn_Click(object sender, EventArgs e)
        {
            Button currBtn = sender as Button;
            if (currBtn != null)
            {
                string strId = currBtn.Attributes["ID"];
                if (!string.IsNullOrEmpty(strId))
                {
                    long commID = 0;

                    if (!long.TryParse(strId, out commID))
                    {
                        throw new CommonCode.UIException(string.Format("Couldnt parse ID attribute on button to long, User id : {0}", currentUser.ID));
                    }

                    BusinessUser businessUser = new BusinessUser();
                    BusinessComment businessComment = new BusinessComment();

                    if (!businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
                    {
                        throw new CommonCode.UIException(string.Format("User ID : {0} cannot delete comments", currentUser.ID));
                    }

                    Comment currComment = businessComment.Get(objectContext, commID);
                    if (currComment == null)
                    {
                        throw new CommonCode.UIException(string.Format("There is no visible comment with ID : {0} which can be deleted by user id : {1}"
                            , commID, currentUser.ID));
                    }

                    businessComment.DeleteComment(objectContext, userContext, currComment, currentUser, businessLog, true, true);

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Comment deleted!");

                    FillComments();
                }
                else
                {
                    throw new CommonCode.UIException(string.Format("Couldnt get ID attribute on button, User id : {0}", currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("Couldnt get parent button, User id : {0}", currentUser.ID));
            }

        }

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }

    }
}
