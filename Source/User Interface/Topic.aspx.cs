﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class Topic : BasePage
    {
        private User currentUser = null;

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private Product currProduct = null; 
        private ProductTopic currTopic = null;

        private bool isAdmin = false;
        private bool canReply = false;

        protected long CommentsNumber = 0;                         // Numbers of MAIN comments (not subcomments) for the topic
        protected long PageNum = 1;                                            // Number of current page
        protected long CommentsOnPage = Configuration.TopicCommentsPerPage;    // Number of MAIN comments to show on page 

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (tblNotify.Visible == true)
            {
                btnSignNotifiesTop.Attributes.Add("onclick", string.Format("NotifyForUpdates('{0}','{1}','{2}','{3}')"
                   , "productTopic", currTopic.ID, tblNotify.ClientID, pnlAction.ClientID));
            }

            if (tblReplyTop.Visible == true || tblReplyBottom.Visible == true)
            {

                btnReplyTop.Attributes.Add("onclick", string.Format("ShowReplyToTopicPnl('{0}', '{1}')"
                    , pnlAction.ClientID, btnReplyTop.ClientID));

                btnReplyBottom.Attributes.Add("onclick", string.Format("ShowReplyToTopicPnl('{0}', '{1}')"
                    ,pnlAction.ClientID, btnReplyBottom.ClientID));

                btnAddReply.Attributes.Add("onclick", string.Format("AddReplyToTopic('{0}')", currTopic.ID));
            }

            if (lblReport.Visible == true)
            {
                lblReport.Attributes.Add("onclick", string.Format("ShowReportData('{0}','{1}','{2}','{3}','{4}')"
                    , "productTopic", currTopic.ID, lblReport.ClientID, pnlAction.ClientID, pnlSendReport.ClientID));
            }

            if (lblModifyTopic.Visible == true)
            {
                lblModifyTopic.Attributes.Add("onclick", string.Format("ShowModifyTopicData('{0}','{1}')"
                    , lblModifyTopic.ClientID, pnlAction.ClientID));

                btnModifyTopic.Attributes.Add("onclick", string.Format("ModifyTopic('{0}','{1}','{2}')"
                    , currTopic.ID, tbModifTopicSubject.ClientID, tbModifTopicDescription.ClientID));
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckParams();                  // Checks Product, topic and page parameters
            SetShowOptionsDependingOnUser();    // fills isAdmin/canReply and shows admin panel if admin

            ShowInfo();

            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page); 
        }

        private void SetShowOptionsDependingOnUser()
        {
            if (currentUser != null)
            {
                pnlRegToAdd.Visible = false;

                BusinessUser bUser = new BusinessUser();

                if (bUser.IsFromAdminTeam(currentUser))
                {
                    if (bUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
                    {
                        isAdmin = true;
                    }

                    canReply = true;

                    tblReplyTop.Visible = true;
                    tblReplyBottom.Visible = true;
                }
                else
                {
                    if (bUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages) && currTopic.locked == false)
                    {
                        canReply = true;

                        tblReplyTop.Visible = true;
                        tblReplyBottom.Visible = true;
                    }
                    else
                    {
                        tblReplyTop.Visible = false;
                        tblReplyBottom.Visible = false;
                    }
                }

                if (currTopic.locked == false)
                {
                    BusinessNotifies businessNotifies = new BusinessNotifies();
                    if (!businessNotifies.SetNewInformationFalseForProductTopicIfUserIsSigned(objectContext, currentUser, currTopic)
                        && !businessNotifies.IsMaxNotificationsNumberReached(objectContext, currentUser))
                    {
                        tblNotify.Visible = true;
                    }
                    else
                    {
                        tblNotify.Visible = false;
                    }
                }
                else
                {
                    tblNotify.Visible = false;
                }

            }
            else
            {
                tblReplyTop.Visible = false;
                tblReplyBottom.Visible = false;
                tblNotify.Visible = false;
                pnlRegToAdd.Visible = true;
            }

            ShowAdminPanel();
        }

        private void ShowAdminPanel()
        {
            if (isAdmin == true)
            {
                accAdmin.Visible = true;

                string visible = string.Empty;
                string locked = string.Empty;

                if (currTopic.visible == true)
                {
                    btnUndelete.Visible = false;
                    btnDelTopic.Visible = true;

                    if (currTopic.locked == true)
                    {
                        btnDelTopicW.Visible = false;
                    }
                    else
                    {
                        btnDelTopicW.Visible = true;
                    }
                   

                    visible = GetLocalResourceObject("admTopicVisible").ToString();
                }
                else
                {
                    btnUndelete.Visible = true;
                    btnDelTopic.Visible = false;
                    btnDelTopicW.Visible = false;

                    visible = GetLocalResourceObject("admTopicNotVisible").ToString();
                }

                if (currTopic.locked == true)
                {
                    btnUnlock.Visible = true;
                    btnLock.Visible = false;
                    btnLockW.Visible = false;

                    locked = GetLocalResourceObject("admTopicLocked").ToString();
                }
                else
                {
                    btnUnlock.Visible = false;
                    btnLock.Visible = true;
                    btnLockW.Visible = true;

                    locked = GetLocalResourceObject("admTopicNotLocked").ToString();
                }

                lblTopicStatus.Text = string.Format("{0} {1}", visible, locked);
            }
            else
            {
                accAdmin.Visible = false;
            }
        }

        private void CheckIfAdminCanEditTheTopic()
        {
            if (currentUser == null)
            {
                throw new CommonCode.UIException("Guest cannot edit the topic");
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.IsFromAdminTeam(currentUser))
            {
                throw new CommonCode.UIException(string.Format("User id : {0}, cannot edit topic ID : {1} or comments in it, because he is not admin."
                    , currentUser.ID, currTopic.ID));
            }
            else if (!bUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
            {
                throw new CommonCode.UIException(string.Format("Admin id : {0}, cannot edit topic ID : {1} or comments in it, because cannot edit comments."
                    , currentUser.ID, currTopic.ID));
            }
        }

        private void CheckParams()
        {
            BusinessUser bUser = new BusinessUser();
            currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, true);

            CheckTopicParams(bUser);
            CheckProduct(bUser);
            CheckPageParams();

        }

        private void CheckProduct(BusinessUser bUser)
        {
            if (currProduct == null)
            {
                throw new CommonCode.UIException("currProduct is null");
            }

            BusinessProduct businessProduct = new BusinessProduct();

       
            if (currProduct.CompanyReference.IsLoaded == false)
            {
                currProduct.CompanyReference.Load();
            }
            if (currProduct.CategoryReference.IsLoaded == false)
            {
                currProduct.CategoryReference.Load();
            }

            string notifError = string.Empty;

            if (!businessProduct.CheckIfProductsIsValidWithConnections(objectContext, currProduct, out notifError))
            {
                if (currentUser != null)
                {
                    if (bUser.IsFromAdminTeam(currentUser))
                    {
                        // admin part
                    }
                    else
                    {
                        CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errProdDeleted").ToString());
                    }
                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errProdDeleted").ToString());
                }
            }
            else
            {
                //  product is valid
            }
            
        }

        private void CheckTopicParams(BusinessUser bUser)
        {
            String strTopicID = Request.Params["id"];
            if (!string.IsNullOrEmpty(strTopicID))
            {
                BusinessProductTopics bTopics = new BusinessProductTopics();

                long topicID = -1;
                if (long.TryParse(strTopicID, out topicID))
                {
                    currTopic = bTopics.Get(objectContext, topicID, false, false);
                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session
                        , GetLocalResourceObject("errIncParameters").ToString());
                }

                if (currTopic == null)
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session
                        , GetLocalResourceObject("errNoTopic").ToString());
                }
                else
                {

                    if (!currTopic.ProductReference.IsLoaded)
                    {
                        currTopic.ProductReference.Load();
                    }

                    currProduct = currTopic.Product;

                    if (currTopic.visible == false)
                    {
                        if (currentUser != null)
                        {
                            if (bUser.IsFromAdminTeam(currentUser))
                            {
                                // can see
                            }
                            else
                            {
                                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errTopicDeleted").ToString());
                            }
                        }
                        else
                        {
                            CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errTopicDeleted").ToString());
                        }
                    }
        
                }
            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
            }
        }

        private void CheckPageParams()
        {

            String strPage = Request.Params["page"];
            if (!string.IsNullOrEmpty(strPage))
            {
                if (!long.TryParse(strPage, out PageNum))
                {
                    if (PageNum < 1)
                    {
                        RedirectToOtherUrl(string.Format("Topic.aspx?id={0}", currTopic.ID));
                    }
                }
            }

            BusinessComment bComments = new BusinessComment();
            CommentsNumber = bComments.CountTopicMainComments(objectContext, currTopic);

            if (CommonCode.Pages.CheckPageParameters(CommentsNumber, PageNum, CommentsOnPage) == false)
            {
                RedirectToOtherUrl(string.Format("Topic.aspx?id={0}", currTopic.ID));
            }

        }

        private void ShowInfo()
        {
            CommonCode.WebMethods.UpdateTypeVisits(VisitedType.ProductTopic, currTopic.ID);

            ShowTopicInfo();

            ShowPagesLinks();
            ShowTopic();
            ShowComments();

            SetLocalText();
            ShowVariantData(); // shows buttons specific to language variant (transliterate button)

            ShowAdvertisement();
        }

        private void ShowAdvertisement()
        {
            if (Configuration.AdvertsNumAdvertsOnCategoryPage > 0)
            {
                phAdverts.Controls.Clear();
                advertsDiv.Attributes.Clear();
                commentsDiv.Attributes.Clear();

                phAdverts.Controls.Add(CommonCode.ImagesAndAdverts.GetAdvertisements
                    (objectContext, Server, "product", currProduct.ID, Configuration.AdvertsNumAdvertsOnTopicPage));
                if (CommonCode.ImagesAndAdverts.getAdvertisementsNumber(phAdverts) > 0)
                {
                    phAdverts.Visible = true;
                    advertsDiv.Attributes.Add("class", "topicAdvertsShow");
                    commentsDiv.Attributes.Add("class", "topicCommentsWithAd");
                }
                else
                {
                    phAdverts.Visible = false;
                    advertsDiv.Attributes.Add("class", "topicAdvertsHide");
                }
            }
        }

        private void ShowVariantData()
        {
            if (btnReplyTop.Visible == true || btnReplyBottom.Visible == true)
            {
                if (ApplicationVariantString == "bg")
                {
                    btnTransReplyToTopic.Visible = true;
                    btnTransReplyToTopic.TargetTextArea = "taReplyDescription";
                }
            }

            if (currentUser != null)
            {
                if (ApplicationVariantString == "bg")
                {
                    btnTransModifyComment.Visible = true;
                    btnTransModifyComment.TargetTextArea = "taModifCommDescr";

                    btnTransModifyTopic.Visible = true;
                    btnTransModifyTopic.TargetTextBox = tbModifTopicDescription;
                }

                
            }

            if (canReply == true)
            {
                if (ApplicationVariantString == "bg")
                {
                    btnTransReplyToComment.Visible = true;
                    btnTransReplyToComment.TargetTextArea = "taReplyToComment";
                }
                
            }
        }

        private void ShowPagesLinks()
        {
            string url = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", currTopic.ID));

            phPagesTop.Controls.Clear();
            phPagesBottom.Controls.Clear();

            phPagesTop.Controls.Add(CommonCode.Pages.GetPagesPlaceHolder(CommentsNumber, CommentsOnPage, PageNum, url));
            phPagesBottom.Controls.Add(CommonCode.Pages.GetPagesPlaceHolder(CommentsNumber, CommentsOnPage, PageNum, url));
        }

        private void ShowTopicInfo()
        {
            hlProduct.Text = currProduct.name;
            hlProduct.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", currProduct.ID));
            hlProduct.ID = string.Format("prod{0}lnk", currProduct.ID);
            hlProduct.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}','{2}')", currProduct.ID, hlProduct.ClientID, pnlPopUp.ClientID));
            hlProduct.Attributes.Add("onmouseout", "HideData()");

            hlForum.NavigateUrl = GetUrlWithVariant(string.Format("Forum.aspx?Product={0}", currProduct.ID));
            hlForum.Text = GetLocalResourceObject("forum").ToString();

            hlTopic.Text = currTopic.name;
            hlTopic.NavigateUrl = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", currTopic.ID));
        }

        private void ShowComments()
        {
            phComments.Controls.Clear();

            BusinessComment bComment = new BusinessComment();

            long from = 0;
            long to = 0;
            CommonCode.Pages.GetFromItemNumberToItemNumber(PageNum, CommentsOnPage, out from, out to);

            List<Comment> comments = bComment.GetTopicComments(objectContext, currTopic.ID, (int)from, (int)to);

            if (comments.Count > 0)
            {
                phComments.Visible = true;

                BusinessUser bUser = new BusinessUser();
                List<long> adminIDs = bUser.AdminsIDsList(objectContext);

                bool canReport = false;
                if (currentUser != null && isAdmin == false)
                {
                    BusinessReport businessReport = new BusinessReport();
                    if (bUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate)
                        && !businessReport.IsMaxActiveSpamReportsReached(objectContext, currentUser))
                    {
                        canReport = true;
                    }
                }


                foreach (Comment comment in comments)
                {
                    ShowComment(comment, phComments, 1, adminIDs, canReport);
                }

            }
            else
            {
                phComments.Visible = false;
            }


        }

        private void ShowComment(Comment comment , Control parentControl, int level, List<long> admins, bool canReport)
        {

            Panel newPanel = new Panel();
            parentControl.Controls.Add(newPanel);

            if (level < 1)
            {
                throw new CommonCode.UIException("comment level is < 1");
            }
            
            switch (level)
            {
                case 1:
                    newPanel.CssClass = "panelRows greenCellBgr";
                    break;
                case 2:
                    newPanel.CssClass = "panelRows yellowCellBgr marginRightComm";
                    break;
                case 3:
                    newPanel.CssClass = "panelRows blueCellBgr marginRightComm";
                    break;
                case 4:
                    newPanel.CssClass = "panelRows greyCellBgr marginRightComm";
                    break;
                default:
                    newPanel.CssClass = "panelRows greenCellBgr marginRightComm";
                    break;
            }
         

            BusinessUser bUser = new BusinessUser();
            BusinessComment bComment = new BusinessComment();

            ////// header

            Panel header = new Panel();
            newPanel.Controls.Add(header);

            header.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginBottom, "5px");

            Label lblDate = new Label();
            header.Controls.Add(lblDate);
            lblDate.Width = Unit.Pixel(200);
            lblDate.Text = comment.dateCreated.ToString();
            lblDate.CssClass = "topicsTextStyleSmall";

            if (!comment.UserIDReference.IsLoaded)
            {
                comment.UserIDReference.Load();
            }

            User userPostedComm = bUser.GetWithoutVisible(userContext, comment.UserID.ID, true);

            bool commFromAdmin = false;

            if (admins.Contains(userPostedComm.ID))
            {
                Label admLabel = CommonCode.UiTools.GetAdminLabel(userPostedComm.username);
                header.Controls.Add(admLabel);
                commFromAdmin = true;
            }
            else
            {
                HyperLink hlUser = new HyperLink();
                header.Controls.Add(hlUser);

                hlUser.Text = userPostedComm.username;
                hlUser.NavigateUrl = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", userPostedComm.ID));
            }

            if (commFromAdmin == false)
            {   // rate comment buttons

                Panel ratePnl = new Panel();
                header.Controls.Add(ratePnl);

                ratePnl.CssClass = "floatRightNoMrg";

                Image plusBtn = new Image();
                ratePnl.Controls.Add(plusBtn);

                plusBtn.ID = string.Format("agree{0}", comment.ID);
                plusBtn.ImageUrl = "~\\images\\SiteImages\\plus.png"; 
                
                Label agreesLbl = new Label();
                agreesLbl.Text = string.Format("({0}) ", comment.agrees);
                ratePnl.Controls.Add(agreesLbl);

                Image minusBtn = new Image();
                minusBtn.ID = string.Format("disagree{0}", comment.ID);
                minusBtn.ImageUrl = "~\\images\\SiteImages\\minus.png";
               
                ratePnl.Controls.Add(minusBtn);

                Label disagreesLbl = new Label();
                disagreesLbl.Text = string.Format("({0})", comment.disagrees);
                ratePnl.Controls.Add(disagreesLbl);

                if (currTopic.locked == false)
                {
                    plusBtn.CssClass = "middleAlign pointerCursor";

                    /////
                    plusBtn.Attributes.Add("onclick", string.Format("ShowRatingData('{0}','{1}','{2}','{3}','{4}','{5}')"
                        , agreesLbl.ClientID, comment.agrees, "1", plusBtn.ClientID, pnlAction.ClientID, comment.ID));
                    ////

                    minusBtn.CssClass = "middleAlign pointerCursor";

                    /////
                    minusBtn.Attributes.Add("onclick", string.Format("ShowRatingData('{0}','{1}','{2}','{3}','{4}','{5}')"
                        , disagreesLbl.ClientID, comment.disagrees, "-1", minusBtn.ClientID, pnlAction.ClientID, comment.ID));
                    ////
                }
                else
                {
                    plusBtn.CssClass = "middleAlign";
                    minusBtn.CssClass = "middleAlign";
                }

            }

            /////// Description

            Panel pnlDescr = new Panel();
            newPanel.Controls.Add(pnlDescr);

            Label lblDescription = new Label();
            pnlDescr.Controls.Add(lblDescription);

            lblDescription.Text = Tools.GetFormattedTextFromDB(comment.description);

            ////// Actions
            if (isAdmin || canReport || comment.lastModified > comment.dateCreated
                || (canReply == true && commFromAdmin == false && level < Configuration.CommentsMaxCommentsReplyLevel)
                || (currentUser != null && comment.UserID.ID == currentUser.ID && currTopic.locked == false))
            {
               
                Panel pnlActions = new Panel();
                newPanel.Controls.Add(pnlActions);

                pnlActions.CssClass = "clearfix2";
                pnlActions.Attributes.CssStyle.Add(HtmlTextWriterStyle.Padding, "5px 0px 5px 0px");

                if (canReply == true && commFromAdmin == false && level < Configuration.CommentsMaxCommentsReplyLevel)
                {
                    Label replyLbl = new Label();
                    pnlActions.Controls.Add(replyLbl);

                    replyLbl.ID = string.Format("replyToComm{0}", comment.ID);
                    replyLbl.Text = GetLocalResourceObject("Reply").ToString();
                    replyLbl.Attributes.Add("onclick",
                        string.Format("ShowReplyToCommentPnl('{0}','{1}','{2}')"
                        , pnlAction.ClientID, replyLbl.ClientID, comment.ID));

                    replyLbl.CssClass = "commentActionButton";
                }



                if (isAdmin == true || (currentUser != null && currTopic.locked == false && currentUser.ID == comment.UserID.ID))
                {  ////////// MODIFY //////

                    Label lblModify = new Label();
                    pnlActions.Controls.Add(lblModify);

                    lblModify.ID = string.Format("modify{0}", comment.ID);
                    lblModify.Text = GetLocalResourceObject("modify").ToString();
                    lblModify.CssClass = "commentActionButton";
                    lblModify.Attributes.Add("onclick", string.Format("ShowModifyCommentDiv('{0}','{1}','{2}')"
                        , pnlAction.ClientID, lblModify.ClientID, comment.ID));
                }


                Panel pnlRightsActions = new Panel();
                pnlActions.Controls.Add(pnlRightsActions);

                pnlRightsActions.CssClass = "panelInline";

                if (canReport == true)
                {
                    Label lblSpam = new Label();
                    pnlRightsActions.Controls.Add(lblSpam);

                    lblSpam.ID = string.Format("markSpam{0}", comment.ID);
                    lblSpam.Text = GetGlobalResourceObject("SiteResources", "violation").ToString();
                    lblSpam.ToolTip = GetGlobalResourceObject("SiteResources", "violationTooltip").ToString();
                    lblSpam.CssClass = "markLbl";
                    lblSpam.Attributes.Add("onclick", string.Format("ReportCommentAsSpam('{0}','{1}','{2}')"
                        , pnlAction.ClientID, lblSpam.ClientID, comment.ID));
                }

                if (isAdmin == true)
                {
                    if (commFromAdmin == false)
                    {
                        Button btnDelComm = new Button();
                        pnlRightsActions.Controls.Add(btnDelComm);

                        btnDelComm.ID = string.Format("delComm{0}", comment.ID);
                        btnDelComm.Attributes.Add("commID", comment.ID.ToString());
                        btnDelComm.Text = GetLocalResourceObject("DelCommWW").ToString();
                        btnDelComm.Click += new EventHandler(btnDelComm_Click);
                    }
                    
                    Button btnDelCommNoWarn = new Button();
                    pnlRightsActions.Controls.Add(btnDelCommNoWarn);

                    btnDelCommNoWarn.ID = string.Format("delCommNoW{0}", comment.ID);
                    btnDelCommNoWarn.Attributes.Add("commID", comment.ID.ToString());
                    btnDelCommNoWarn.Text = GetLocalResourceObject("DelComm").ToString();
                    btnDelCommNoWarn.Click += new EventHandler(btnDelCommNoWarn_Click);
                    
                }

                ////// Last modified
                if (comment.lastModified > comment.dateCreated)
                {
                    if (!comment.LastModifiedByReference.IsLoaded)
                    {
                        comment.LastModifiedByReference.Load();
                    }

                    User lastModifBy = bUser.GetWithoutVisible(userContext, comment.LastModifiedBy.ID, true);

                    Panel pnlLastModif = new Panel();
                    pnlActions.Controls.Add(pnlLastModif);
                    pnlLastModif.CssClass = "floatRightNoMrg";

                    Label lblLastModif = new Label();
                    pnlLastModif.Controls.Add(lblLastModif);

                    lblLastModif.Text = string.Format("{0} {1}, {2}", GetLocalResourceObject("lastEdited")
                        , lastModifBy.username, comment.lastModified.ToString());
                    lblLastModif.CssClass = "topicsModificationLbl";
                }

            }

            //// Sub-comments

            if (comment.haveSubcomments == true)
            {
                List<Comment> SubComments = bComment.GetAllSubComments(objectContext, comment.ID).ToList();
                if (SubComments.Count<Comment>() > 0)
                {
                    foreach (Comment subcomment in SubComments)
                    {
                        ShowComment(subcomment, newPanel, level + 1, admins, canReport);
                    }
                }
            }

        }

        private void SetLocalText()
        {
            Title = currTopic.name;

            lblAdminPanel.Text = GetGlobalResourceObject("SiteResources", "AdminPanel").ToString();

            if (btnReplyTop.Visible == true || btnReplyBottom.Visible == true)
            {
                btnReplyTop.Value = GetLocalResourceObject("Reply").ToString();
                btnReplyBottom.Value = GetLocalResourceObject("Reply").ToString();

                lblCommentRules1.Text = GetLocalResourceObject("CommentRules1").ToString();
                lblCommentRules2.Text = GetLocalResourceObject("CommentRules2").ToString();

                hlCommentRules.Text = GetLocalResourceObject("hlCommentRules").ToString();
                hlCommentRules.Target = "_blank";
                hlCommentRules.NavigateUrl = GetUrlWithVariant("Rules.aspx#rulesTopic");

                lblReplyToTopic.Text = GetLocalResourceObject("ReplyToTopic").ToString();
                btnAddReply.Value = GetLocalResourceObject("Reply").ToString();
                btnHideReplyData.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();

            }

            if (tblNotify.Visible == true)
            {
                btnSignNotifiesTop.Value = GetLocalResourceObject("SignNotifies").ToString();
                btnSignNotifiesTop.Attributes.Add("title", GetLocalResourceObject("SignNotifiesTooltip").ToString());
            }

            if (lblReport.Visible == true)
            {
                lblReport.Text = GetGlobalResourceObject("SiteResources", "lblWriteReport").ToString();
                lblReportInfo.Text = GetGlobalResourceObject("SiteResources", "reportIrregularity").ToString();
                btnSendReport.Value = GetGlobalResourceObject("SiteResources", "btnReport").ToString();
                btnHideRepData.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();
            }

            if (lblModifyTopic.Visible == true)
            {
                lblModifyTopic.Text = GetLocalResourceObject("modify").ToString();
                lblModifyTopicInfo.Text = GetLocalResourceObject("ModifyInfo").ToString();
                lblModifTopicSubject.Text = GetLocalResourceObject("Subject").ToString();
                lblModifTopicDescr.Text = GetLocalResourceObject("Description").ToString();
                btnModifyTopic.Value = GetLocalResourceObject("modify").ToString();
                btnHideModifyTopicData.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();
            }

            if (canReply == true)
            {
                lblReplyCommRules1.Text = GetLocalResourceObject("CommentRules1").ToString();
                lblReplyCommRules2.Text = GetLocalResourceObject("CommentRules2").ToString();

                hlReplyCommRules.Text = GetLocalResourceObject("hlCommentRules").ToString();
                hlReplyCommRules.Target = "_blank";
                hlReplyCommRules.NavigateUrl = GetUrlWithVariant("Rules.aspx#rulesTopic");

                lblReplyToComment.Text = GetLocalResourceObject("ReplyToComment").ToString();
                btnReplyToComment.Value = GetLocalResourceObject("Reply").ToString();
                btnCloseReplyToComment.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();
            }

            if (currentUser != null)
            {
                lblModifyComment.Text = GetLocalResourceObject("ModifyComment").ToString();
                btnModifyComment.Value = GetLocalResourceObject("modify").ToString();
                btnCloseModifComm.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();
            }

            if (isAdmin == true)
            {
                btnDelTopic.Text = GetLocalResourceObject("admDelTopic").ToString();
                btnDelTopicW.Text = GetLocalResourceObject("admDelTopicWarning").ToString();
                btnUndelete.Text = GetLocalResourceObject("admUndelTopic").ToString();
                btnLock.Text = GetLocalResourceObject("admlockTopic").ToString();
                btnLockW.Text = GetLocalResourceObject("admLockTopicWarning").ToString();
                btnUnlock.Text = GetLocalResourceObject("admUnlockTopic").ToString();
            }

            if (pnlRegToAdd.Visible == true)
            {
                lblRegToAdd1.Text = GetLocalResourceObject("lblRegToAdd1").ToString();

                hlRegToAdd1.Text = GetLocalResourceObject("hlRegToAdd1").ToString();
                hlRegToAdd1.NavigateUrl = GetUrlWithVariant("Registration.aspx");

                hlRegToAdd2.Text = GetLocalResourceObject("hlRegToAdd2").ToString();
                hlRegToAdd2.NavigateUrl = GetUrlWithVariant("LogIn.aspx");


                lblRegToAdd2.Text = GetLocalResourceObject("lblRegToAdd2").ToString();
            }

        }

        private void ShowTopic()
        {
            BusinessUser businessUser = new BusinessUser();

            if (PageNum <= 1)
            {
                pnlTopic.Visible = true;
                pnlTopicInfo.Visible = false;
               
                if (!currTopic.UserReference.IsLoaded)
                {
                    currTopic.UserReference.Load();
                }

                if (currTopic.visible == true)
                {
                    if (currTopic.locked == false)
                    {
                        imgTopic.ImageUrl = "~\\images\\SiteImages\\topic_nocomments.png";
                    }
                    else
                    {
                        imgTopic.ImageUrl = "~\\images\\SiteImages\\topic_locked.png";
                    }
                }
                else
                {
                    imgTopic.ImageUrl = "~\\images\\SiteImages\\topic_deleted.png";
                }


                hlTopicName.Text = currTopic.name;
                hlTopicName.NavigateUrl = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", currTopic.ID));


                if (currentUser != null)
                {
                    if (isAdmin == true || (currTopic.User.ID == currentUser.ID && currTopic.locked == false))
                    {
                        lblModifyTopic.Visible = true;

                        tbModifTopicSubject.Text = currTopic.name;
                        tbModifTopicDescription.Text = currTopic.description;
                    }

                    BusinessReport businessReport = new BusinessReport();
                    if (businessUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate)
                        && !businessReport.IsMaxActiveIrregularityReportsReached(objectContext, currentUser))
                    {
                        lblReport.Visible = true;

                        if (string.IsNullOrEmpty(lblReporting.Text))
                        {
                            BusinessSiteText businessText = new BusinessSiteText();
                            SiteNews aboutReporting = businessText.GetSiteText(objectContext, "aboutTopicReporting");
                            if (aboutReporting != null && !string.IsNullOrEmpty(aboutReporting.description))
                            {
                                lblReporting.Text = aboutReporting.description;
                            }
                        }
                    }
                }

                lblStartedBy.Text = string.Format("{0} ", GetLocalResourceObject("startedBy"));

                User startedBy = businessUser.GetWithoutVisible(userContext, currTopic.User.ID, true);

                if (businessUser.IsFromAdminTeam(startedBy))
                {
                    hlStartedBy.Visible = false;
                    lblStartedByAdm.Visible = true;

                    lblStartedByAdm.Text = Tools.GetFormattedTextFromDB(startedBy.username);
                    lblStartedByAdm.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    hlStartedBy.Visible = true;
                    lblStartedByAdm.Visible = false;

                    hlStartedBy.Text = startedBy.username;
                    hlStartedBy.NavigateUrl = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", startedBy.ID));
                }


                hlStartedBy.Text = startedBy.username;
                hlStartedBy.NavigateUrl = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", startedBy.ID));

                lblStartedOn.Text = string.Format(", {0}", currTopic.dateCreated.ToString());

                lblDescription.Text = Tools.GetFormattedTextFromDB(currTopic.description);

                if (currTopic.lastModified > currTopic.dateCreated)
                {
                    pnlTopicModifications.Visible = true;

                    if (!currTopic.LastModifiedByReference.IsLoaded)
                    {
                        currTopic.LastModifiedByReference.Load();
                    }

                    User lastEditedBy = businessUser.GetWithoutVisible(userContext, currTopic.LastModifiedBy.ID, true);

                    lblTopicModification.Text = string.Format("{0} {1}, {2}", GetLocalResourceObject("lastEdited")
                        , lastEditedBy.username, currTopic.lastModified.ToString());
                }
                else
                {
                    pnlTopicModifications.Visible = false;
                }

            }
            else
            {
                pnlTopicInfo.Visible = true;
                pnlTopic.Visible = false;

                lblTopicName.Text = currTopic.name;
            }
        }

        void btnDelCommNoWarn_Click(object sender, EventArgs e)
        {
            CheckIfAdminCanEditTheTopic();

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                long commID = 0;
                string commentIdStr = btnSender.Attributes["commID"];
                long.TryParse(commentIdStr, out commID);

                BusinessComment bComment = new BusinessComment();
                Comment currComment = bComment.Get(objectContext, commID);

                if (currComment == null || currComment.visible == false)
                {
                    return;
                }

                bComment.DeleteComment(objectContext, userContext, currComment, currentUser, businessLog, true, false); 
 
                string description = GetLocalResourceObject("admCommDeleted").ToString();
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, description);

                ShowComments();
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button.");
            }

    
        }

        void btnDelComm_Click(object sender, EventArgs e)
        {
            CheckIfAdminCanEditTheTopic();

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                long commID = 0;
                string commentIdStr = btnSender.Attributes["commID"];
                long.TryParse(commentIdStr, out commID);

                BusinessComment bComment = new BusinessComment();
                Comment currComment = bComment.Get(objectContext, commID);

                if (currComment == null || currComment.visible == false)
                {
                    return;
                }

                bComment.DeleteComment(objectContext, userContext, currComment, currentUser, businessLog, true, true); 
 
                string description = GetLocalResourceObject("admCommDeleted").ToString();
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, description);

                ShowComments();
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button.");
            }
        }


        protected void btnDelTopic_Click(object sender, EventArgs e)
        {
            CheckIfAdminCanEditTheTopic();

            if (currTopic.visible == false)
            {
                return;
            }

            BusinessProductTopics bpTopics = new BusinessProductTopics();
            bpTopics.DeleteTopic(objectContext, userContext, currentUser, businessLog, currTopic, false);

            string description = GetLocalResourceObject("admTopicNotifDeleted").ToString();
            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, description);

            ShowAdminPanel();
            ShowTopic();
        }

        protected void btnDelTopicW_Click(object sender, EventArgs e)
        {
            CheckIfAdminCanEditTheTopic();

            if (currTopic.visible == false)
            {
                return;
            }

            BusinessProductTopics bpTopics = new BusinessProductTopics();
            bpTopics.DeleteTopic(objectContext, userContext, currentUser, businessLog, currTopic, true);

            string description = GetLocalResourceObject("admTopicNotifDeleted").ToString();
            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, description);

            ShowAdminPanel();
            ShowTopic();
        }

        protected void btnUndelete_Click(object sender, EventArgs e)
        {
            CheckIfAdminCanEditTheTopic();

            if (currTopic.visible == true)
            {
                return;
            }

            BusinessProductTopics bpTopics = new BusinessProductTopics();
            bpTopics.UnDeleteTopic(objectContext, userContext, currentUser, businessLog, currTopic);

            string description = GetLocalResourceObject("admTopicNotifUndeleted").ToString();
            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, description);

            ShowAdminPanel();
            ShowTopic();
        }


        protected void btnLock_Click(object sender, EventArgs e)
        {
            CheckIfAdminCanEditTheTopic();

            if (currTopic.locked == true)
            {
                return;
            }

            BusinessProductTopics bpTopics = new BusinessProductTopics();
            bpTopics.LockTopic(objectContext, userContext, currentUser, businessLog, currTopic, false);

            string description = GetLocalResourceObject("admNotifTopicLocked").ToString();
            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, description);

            ShowAdminPanel();
            ShowTopic();
        }

        protected void btnLockW_Click(object sender, EventArgs e)
        {
            CheckIfAdminCanEditTheTopic();

            if (currTopic.locked == true)
            {
                return;
            }

            BusinessProductTopics bpTopics = new BusinessProductTopics();
            bpTopics.LockTopic(objectContext, userContext, currentUser, businessLog, currTopic, true);

            string description = GetLocalResourceObject("admNotifTopicLocked").ToString();
            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, description);

            ShowAdminPanel();
            ShowTopic();
        }

       

        protected void btnUnlock_Click(object sender, EventArgs e)
        {
            CheckIfAdminCanEditTheTopic();

            if (currTopic.locked == false)
            {
                return;
            }

            BusinessProductTopics bpTopics = new BusinessProductTopics();
            bpTopics.UnlockTopic(objectContext, userContext, currentUser, businessLog, currTopic);

            string description = GetLocalResourceObject("admNotifTopicUnlocked").ToString();
            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, description);

            ShowAdminPanel();
            ShowTopic();

        }
        

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            string result = CommonCode.WebMethods.GetTypeData(type, Id);
            return result;
        }

        [WebMethod]
        public static string WMSignForNotifies(string type, string Id)
        {
            return CommonCode.WebMethods.SignForNotifies(type, Id);
        }

        [WebMethod]
        public static string WMSendReport(string type, string strTypeId, string description)
        {
            return CommonCode.WebMethods.SendReport(type, strTypeId, description);
        }

        [WebMethod]
        public static string WMAddReplyToTopic(string strTopicId, string description)
        {
            return CommonCode.WebMethods.AddCommentToTopic(strTopicId, description);
        }

        [WebMethod]
        public static string WMRateComment(string commID, string rating)
        {
            return CommonCode.WebMethods.GetRateCommentData(commID, rating);
        }

        [WebMethod]
        public static string WMSetMsgAsSpam(string commID)
        {
            return CommonCode.WebMethods.SetMsgAsViolation(commID, CommentType.Topic);
        }

        [WebMethod]
        public static string AddReplyToComment(string commID, string message)
        {
            return CommonCode.WebMethods.ReplyToComment(commID, string.Empty, message, CommentType.Topic);
        }

        [WebMethod]
        public static string WMGetComment(string commID)
        {
            return CommonCode.WebMethods.GetCommentDescription(commID);
        }

        [WebMethod]
        public static string WMModifyComment(string commID, string description)
        {
            return CommonCode.WebMethods.EditTopicComment(commID, description);
        }

        [WebMethod]
        public static string WMModifyTopic(string topicID, string subject, string description)
        {
            return CommonCode.WebMethods.EditTopic(topicID, subject, description);
        }



       
        
    }
}
