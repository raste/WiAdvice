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
    public partial class Forum : BasePage
    {

        private User currentUser = null;

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private Product currProduct = null;

        protected long TopicsNumber = 0;                                       // Numbers of Topics for the product
        protected long PageNum = 1;                                            // Number of current page
        protected long TopicsOnPage = Configuration.TopicsPerPage;             // Number of Topics to show on page 

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (tblAddTopicTop.Visible == true || tblAddTopicBottom.Visible == true)
            {
                btnAddTopicTop.Attributes.Add("onclick", string.Format("ShowAddTopicData('{0}', '{1}')"
                    , pnlAction.ClientID, btnAddTopicTop.ClientID));

                btnAddTopicBottom.Attributes.Add("onclick", string.Format("ShowAddTopicData('{0}', '{1}')"
                    , pnlAction.ClientID, btnAddTopicBottom.ClientID));

                btnCreateTopic.Attributes.Add("onclick", string.Format("AddTopic('{0}')", currProduct.ID));
            }

            if (notifyTbl.Visible == true)
            {
                btnSignNotifiesTop.Attributes.Add("onclick", string.Format("NotifyForUpdates('{0}','{1}','{2}','{3}')"
                   , "productForum", currProduct.ID, notifyTbl.ClientID, pnlAction.ClientID));
            }

        }


        protected void Page_Load(object sender, EventArgs e)
        {
            CheckParameters();  // sets User and checks Product and Page parameter
            ShowOptions();      // shows Options depending on user

            ShowInfo();
        }

        private void ShowInfo()
        {
            ShowForumInfo();



            ShowPagesLinks();
            ShowTopics();

            SetLocalText();
        }

        private void ShowForumInfo()
        {

            if (!currProduct.CategoryReference.IsLoaded)
            {
                currProduct.CategoryReference.Load();
            }
            if (!currProduct.CompanyReference.IsLoaded)
            {
                currProduct.CompanyReference.Load();
            }

            hlProduct.Text = currProduct.name;
            hlProduct.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}','{2}')", currProduct.ID, hlProduct.ClientID, pnlPopUp.ClientID));
            hlProduct.Attributes.Add("onmouseout", "HideData()");
            hlProduct.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", currProduct.ID));
        }

        private void ShowOptions()
        {
            if (currentUser != null)
            {
                pnlAddTopicTop.Visible = true;
                pnlAddTopicBottom.Visible = true;
                pnlRegToAdd.Visible = false;

                BusinessUser bUser = new BusinessUser();

                if (bUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
                {
                    tblAddTopicTop.Visible = true;
                    tblAddTopicBottom.Visible = true;

                    if (ApplicationVariantString == "bg")
                    {
                        btnTransAddTopivComment.Visible = true;
                        btnTransAddTopivComment.TargetTextArea = "taTopicDescription";
                    }

                }
                else
                {
                    tblAddTopicTop.Visible = false;
                    tblAddTopicBottom.Visible = false;
                }

                BusinessNotifies businessNotifies = new BusinessNotifies();
                if (!businessNotifies.SetNewInformationFalseForProductForumIfUserIsSigned(objectContext, currentUser, currProduct)
                    && !businessNotifies.IsMaxNotificationsNumberReached(objectContext, currentUser))
                {
                    notifyTbl.Visible = true;
                }
                else
                {
                    notifyTbl.Visible = false;
                }


            }
            else
            {
                pnlAddTopicTop.Visible = false;
                pnlAddTopicBottom.Visible = false;
                pnlRegToAdd.Visible = true;
            }

        }

        private void ShowPagesLinks()
        {
            string url = GetUrlWithVariant(string.Format("Forum.aspx?Product={0}", currProduct.ID));

            phPagesTop.Controls.Clear();
            phPagesBottom.Controls.Clear();

            phPagesTop.Controls.Add(CommonCode.Pages.GetPagesPlaceHolder(TopicsNumber, TopicsOnPage, PageNum, url));
            phPagesBottom.Controls.Add(CommonCode.Pages.GetPagesPlaceHolder(TopicsNumber, TopicsOnPage, PageNum, url));
        }

        private void ShowTopics()
        {
            phTopics.Controls.Clear();

            BusinessProductTopics bTopics = new BusinessProductTopics();

            long from = 0;
            long to = 0;
            CommonCode.Pages.GetFromItemNumberToItemNumber(PageNum, TopicsOnPage, out from, out to);

            List<ProductTopic> topics = bTopics.GetProductTopics(objectContext, currProduct, from, to, true);

            if (topics.Count > 0)
            {

                ShowInfoTopicPanel();

                foreach (ProductTopic topic in topics)
                {
                    ShowTopic(topic);
                }

            }
            else
            {
                lblNoPostedTopics.Visible = true;
                lblNoPostedTopics.Text = GetLocalResourceObject("NoPostedTopics").ToString();

                pnlAddTopicBottom.Visible = false;
            }

        }

        private void ShowTopic(ProductTopic topic)
        {

            BusinessUser bUser = new BusinessUser();
            BusinessComment bComment = new BusinessComment();
            BusinessProductTopics bpTopic = new BusinessProductTopics();

            if (!topic.UserReference.IsLoaded)
            {
                topic.UserReference.Load();
            }

            Panel firstPnl = new Panel();
            phTopics.Controls.Add(firstPnl);

            firstPnl.CssClass = "clearfix topicsMainPnl";
            firstPnl.ToolTip = Tools.TrimString(topic.description, 500, false, true);

            ////////////
            Panel titlePnl = new Panel();
            firstPnl.Controls.Add(titlePnl);
            titlePnl.CssClass = "topicsTitle";

            Image msgImg = new Image();
            titlePnl.Controls.Add(msgImg);

            msgImg.ImageAlign = ImageAlign.Left;
            msgImg.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginRight, "10px");
            msgImg.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginTop, "4px"); //1
            msgImg.Height = Unit.Pixel(33); //40

            if (topic.visible == true)
            {
                if (topic.locked == false)
                {
                    if (bpTopic.AreThereNewCommentsSinceLastUserVisit(objectContext, topic, currentUser, Request.UserHostAddress) == true)
                    {
                        msgImg.ImageUrl = "~\\images\\SiteImages\\topic_comments.png";
                        msgImg.ToolTip = GetLocalResourceObject("topicNewComments").ToString();
                    }
                    else
                    {
                        msgImg.ImageUrl = "~\\images\\SiteImages\\topic_nocomments.png";
                        msgImg.ToolTip = GetLocalResourceObject("topicNoNewComments").ToString();
                    }
                }
                else
                {
                    msgImg.ImageUrl = "~\\images\\SiteImages\\topic_locked.png";
                    msgImg.ToolTip = GetLocalResourceObject("topicLocked").ToString();
                }
            }
            else
            {
                msgImg.ImageUrl = "~\\images\\SiteImages\\topic_deleted.png";
                msgImg.ToolTip = GetLocalResourceObject("topicDeleted").ToString();
            }



            HyperLink hlTitle = new HyperLink();
            titlePnl.Controls.Add(hlTitle);

            hlTitle.NavigateUrl = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", topic.ID));
            hlTitle.Text = topic.name;
            hlTitle.CssClass = "topicSubjectLink";

            titlePnl.Controls.Add(CommonCode.UiTools.GetNewLineControl());

            Label lblStartedBy = new Label();
            titlePnl.Controls.Add(lblStartedBy);
            lblStartedBy.Text = string.Format("{0} ", GetLocalResourceObject("startedBy"));
            lblStartedBy.CssClass = "topicsTextStyleSmall";

            User startedBy = bUser.GetWithoutVisible(userContext, topic.User.ID, true);

            if (bUser.IsFromAdminTeam(startedBy))
            {
                titlePnl.Controls.Add(CommonCode.UiTools.GetAdminLabel(startedBy.username));
            }
            else
            {
                HyperLink hlStartedBy = new HyperLink();
                titlePnl.Controls.Add(hlStartedBy);

                hlStartedBy.Text = startedBy.username;
                hlStartedBy.NavigateUrl = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", startedBy.ID));
            }

            Label lblStartedOn = new Label();
            titlePnl.Controls.Add(lblStartedOn);
            lblStartedOn.Text = string.Format(", {0}&nbsp;&nbsp;", topic.dateCreated.ToString());
            lblStartedOn.CssClass = "topicsTextStyleSmall";

            if (topic.comments > Configuration.TopicCommentsPerPage)
            {
                string url = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", topic.ID));
                long opinions = bComment.CountTopicMainComments(objectContext, topic);
                PlaceHolder phTopicPage = CommonCode.Pages.GetPagesPlaceHolderSmallStyle(opinions, Configuration.TopicCommentsPerPage, 0, url);
                titlePnl.Controls.Add(phTopicPage);
            }

            ////////////

            Panel visitsRepliesPnl = new Panel();
            firstPnl.Controls.Add(visitsRepliesPnl);
            visitsRepliesPnl.CssClass = "topicsViewsVisits";

            Label lblReplies = new Label();
            visitsRepliesPnl.Controls.Add(lblReplies);
            lblReplies.Text = string.Format("{0} : {1}", GetLocalResourceObject("replies"), topic.comments);
            lblReplies.CssClass = "topicsTextStyleSmall";

            visitsRepliesPnl.Controls.Add(CommonCode.UiTools.GetNewLineControl());

            Label lblVisits = new Label();
            visitsRepliesPnl.Controls.Add(lblVisits);
            lblVisits.Text = string.Format("{0} : {1}", GetLocalResourceObject("visits"), topic.visits);
            lblVisits.CssClass = "topicsTextStyleSmall";

            //////////////

            Panel lastCommentByPnl = new Panel();
            firstPnl.Controls.Add(lastCommentByPnl);
            lastCommentByPnl.CssClass = "topicsLastPostBy";

            if (topic.comments > 0)
            {
                if (!topic.LastCommentByReference.IsLoaded)
                {
                    topic.LastCommentByReference.Load();
                }

                User lastCommBy = bUser.GetWithoutVisible(userContext, topic.LastCommentBy.ID, true);

                if (bUser.IsFromAdminTeam(lastCommBy))
                {
                    Label adminComm = CommonCode.UiTools.GetAdminLabel(lastCommBy.username);
                    lastCommentByPnl.Controls.Add(adminComm);
                }
                else
                {
                    HyperLink lastCommUser = new HyperLink();
                    lastCommentByPnl.Controls.Add(lastCommUser);

                    lastCommUser.NavigateUrl = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", lastCommBy.ID));
                    lastCommUser.Text = lastCommBy.username;
                }

                lastCommentByPnl.Controls.Add(CommonCode.UiTools.GetNewLineControl());

                Label lblLastCommDate = CommonCode.UiTools.GetLabelWithText(topic.lastCommentDate.ToString(), false);
                lastCommentByPnl.Controls.Add(lblLastCommDate);
                lblLastCommDate.CssClass = "topicsTextStyleSmall";
            }
        }

        private void ShowInfoTopicPanel()
        {
            Panel firstPnl = new Panel();
            phTopics.Controls.Add(firstPnl);

            firstPnl.CssClass = "clearfix topicsInfoPnl";

            Panel titlePnl = new Panel();
            firstPnl.Controls.Add(titlePnl);
            titlePnl.CssClass = "topicsInfoTitle";

            Label lblTitle = CommonCode.UiTools.GetLabelWithText(GetLocalResourceObject("topicTitle").ToString(), false);
            titlePnl.Controls.Add(lblTitle);

            Panel visitsRepliesPnl = new Panel();
            firstPnl.Controls.Add(visitsRepliesPnl);
            visitsRepliesPnl.CssClass = "topicsInfoViewsVisits";

            Label lblVisits = CommonCode.UiTools.GetLabelWithText(GetLocalResourceObject("topicVisitsViews").ToString(), false);
            visitsRepliesPnl.Controls.Add(lblVisits);

            Panel lastCommentByPnl = new Panel();
            firstPnl.Controls.Add(lastCommentByPnl);
            lastCommentByPnl.CssClass = "topicsInfoLastPostBy";

            Label lblLastCommBy = CommonCode.UiTools.GetLabelWithText(GetLocalResourceObject("topicLastComment").ToString(), false);
            lastCommentByPnl.Controls.Add(lblLastCommBy);
        }

        private void SetLocalText()
        {
            Title = string.Format("{0} \" {1} \" {2}", GetLocalResourceObject("title")
                , currProduct.name, GetLocalResourceObject("title2"));

            lblForumFor.Text = GetLocalResourceObject("forumFor").ToString();

            btnSignNotifiesTop.Value = GetLocalResourceObject("SignNotifies").ToString();
            btnSignNotifiesTop.Attributes.Add("title", GetLocalResourceObject("SignNotifiesTooltip").ToString());

            if (tblAddTopicTop.Visible == true || tblAddTopicBottom.Visible == true)
            {
                lblTopicRules1.Text = GetLocalResourceObject("topicRules1").ToString();
                lblTopicRules2.Text = GetLocalResourceObject("topicRules2").ToString();

                hlTopicRules.Text = GetLocalResourceObject("hltopicRules").ToString();
                hlTopicRules.Target = "_blank";
                hlTopicRules.NavigateUrl = GetUrlWithVariant("Rules.aspx#rulesTopic");

                btnAddTopicTop.Value = GetLocalResourceObject("btnAddTopic").ToString();
                btnAddTopicBottom.Value = GetLocalResourceObject("btnAddTopic").ToString();

                lblAddTopic.Text = GetLocalResourceObject("addTopic").ToString();
                lblTopicSubject.Text = GetLocalResourceObject("topicSubject").ToString();
                lblTopicDescription.Text = GetLocalResourceObject("topicDescr").ToString();

                btnCreateTopic.Value = GetLocalResourceObject("add").ToString();
                btnHideTopicData.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();
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

        private void CheckParameters()
        {
            object objNewTopicId = Session["redirrectToNewTopic"];
            if (objNewTopicId != null)
            {
                string strTopicId = objNewTopicId.ToString();
                long topicId = 0;
                long.TryParse(strTopicId, out topicId);

                Session["redirrectToNewTopic"] = null;

                if (topicId > 0)
                {
                    RedirectToOtherUrl(string.Format("Topic.aspx?id={0}", topicId));
                }
            }


            BusinessUser bUser = new BusinessUser();
            currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, true);

            CheckProductParams(bUser);

            CheckPageParams();
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
                        RedirectToOtherUrl(string.Format("Forum.aspx?Product={0}", currProduct.ID));
                    }
                }
            }

            BusinessProductTopics bTopics = new BusinessProductTopics();
            TopicsNumber = bTopics.CountProductTopics(objectContext, currProduct, true);

            if (CommonCode.Pages.CheckPageParameters(TopicsNumber, PageNum, TopicsOnPage) == false)
            {
                RedirectToOtherUrl(string.Format("Forum.aspx?Product={0}", currProduct.ID));
            }
        }

        private void CheckProductParams(BusinessUser bUser)
        {
            String ProdID = Request.Params["Product"];
            if (!string.IsNullOrEmpty(ProdID))
            {
                BusinessProduct businessProduct = new BusinessProduct();

                long prodID = -1;
                if (long.TryParse(ProdID, out prodID))
                {
                    currProduct = businessProduct.GetProductByIDWV(objectContext, prodID);
                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session
                        , GetLocalResourceObject("errIncParameters").ToString());
                }

                if (currProduct == null)
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session
                        , GetLocalResourceObject("errNoProduct").ToString());
                }
                else
                {
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
                                // admin 
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
                        //  forum is valid
                    }
                }
            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
            }
        }


        [WebMethod]
        public static string AddTopic(string productId, string subject, string description)
        {
            return CommonCode.WebMethods.AddTopic(productId, subject, description);
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

    }
}
