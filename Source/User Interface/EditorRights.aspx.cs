﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Globalization;

using DataAccess;
using BusinessLayer;
using CustomServerControls;

namespace UserInterface
{
    public partial class EditorRights : BasePage
    {
        private Boolean canEdit = false;
        private Boolean isVisited = false;
        private Boolean canTakeAction = false;

        private User visiterUser = null;
        private User currentUser = null;

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
            CheckGetActionOptions();

            ShowInfo();
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);
        }

        private void ShowInfo()
        {

            string variant = Tools.ApplicationVariantString;
            string username = string.Empty;

            if (isVisited == true)
            {
                username = visiterUser.username;
            }
            else
            {
                username = currentUser.username;
            }

            switch (variant)
            {
                case "bg":
                    Title = string.Format("{0} {1} {2}", GetLocalResourceObject("title"), username
                        , GetLocalResourceObject("title2"));
                    break;
                default:
                    Title = string.Format("{0}{1}", username, GetLocalResourceObject("title"));
                    break;
            }

            phInfo.Controls.Clear();

            if (isVisited)
            {
                phInfo.Controls.Add(lblInfo);
                lblInfo.Text = string.Format("{0} ", GetLocalResourceObject("visitedInfo").ToString());

                HyperLink hlVisited = new HyperLink();
                phInfo.Controls.Add(hlVisited);
                hlVisited.NavigateUrl = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", visiterUser.ID));
                hlVisited.Text = visiterUser.username;

                if (canTakeAction == true)
                {
                    pnlInfo.Visible = true;

                    lblInfo3.Visible = true;
                    lblInfo3.Text = string.Format("{0} {1} {2} {3} {4}", GetLocalResourceObject("currCanTakRole")
                        , visiterUser.username, GetLocalResourceObject("currCanTakRole2")
                        , Configuration.GetUserActionTimeAfterWhichActionsCanBeTaken
                        , GetLocalResourceObject("days"));
                }
                else
                {
                    lblInfo3.Visible = false;
                    pnlInfo.Visible = false;
                }
            }
            else
            {
                phInfo.Controls.Add(lblInfo);

                lblInfo.Text = GetLocalResourceObject("currUserInfo").ToString();

                btnCreateTransfer.Value = GetGlobalResourceObject("SiteResources", "Send").ToString();
                btnCancelTransfer.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();

                BusinessUser bUser = new BusinessUser();
                if (bUser.IsUser(currentUser) == true)
                {
                    pnlInfo.Visible = true;

                    lblTransferTo.Text = GetLocalResourceObject("TransferTo").ToString();

                    lblInfo3.Text = string.Format("{0} {1} {2}", GetLocalResourceObject("LogInInfo")
                        , Configuration.GetUserActionTimeAfterWhichActionsCanBeTaken
                        , GetLocalResourceObject("LogInInfo2"));
                }
                else
                {
                    lblInfo3.Visible = false;

                    pnlInfo.Visible = false;
                }
            }


            FillEditRoles();
            FillTransfers();
            SetLocalText();

        }

        private void SetLocalText()
        {

            if (pnlReadHowToAdd.Visible == true)
            {
                lblHowToHaveEditRights.Text = GetLocalResourceObject("howToHaveRights").ToString();

                hlClickToAddProduct.Text = GetLocalResourceObject("ClickHereToAddProd").ToString();
                hlClickToReadHowToAddProduct.Text = GetLocalResourceObject("ClickHereToReadAddProd").ToString();

                lblClickToAddProduct.Text = GetLocalResourceObject("toAddProduct").ToString();
                lblClickToReadHowToAddProduct.Text = GetLocalResourceObject("toReadHowToAddProduct").ToString();

                hlClickToAddCompany.Text = GetLocalResourceObject("ClickHereToAddComp").ToString();
                hlClickToReadHowToAddCompany.Text = GetLocalResourceObject("ClickHereToReadAddComp").ToString();

                lblClickToAddCompany.Text = GetLocalResourceObject("toAddCompany").ToString();
                lblClickToReadHowToAddCompany.Text = GetLocalResourceObject("toReadHowToAddCompany").ToString();
            }
        }

        private void CheckGetActionOptions()
        {
            if (isVisited == true)
            {
                if (canEdit == true)
                {
                    // admin 
                    return;
                }

                if (currentUser != null)
                {
                    BusinessUser bUser = new BusinessUser();
                    if (bUser.IsUser(currentUser))
                    {
                        BusinessUserTypeActions butActions = new BusinessUserTypeActions();
                        if (butActions.CanTypeActionsBeTakenFromUser(visiterUser))
                        {
                            if (butActions.CanUserTakeActionFromEditor(userContext, objectContext, currentUser))
                            {
                                canTakeAction = true;
                            }
                        }
                    }
                }

            }
        }

        private void FillTransfers()
        {
            phTransfers.Controls.Clear();

            User forUser = currentUser;
            if (isVisited == true)
            {
                if (canEdit == true)
                {
                    forUser = visiterUser;
                }
                else
                {
                    // only admins can see user transfers
                    return;
                }
            }

            BusinessTransferAction bTransfer = new BusinessTransferAction();
            List<TransferTypeAction> transfersBy = bTransfer.GetUserTransfers(objectContext, forUser);
            List<TransferTypeAction> transfersTo = bTransfer.GetTransfersToUser(objectContext, forUser);
            if (transfersBy.Count > 0 || transfersTo.Count > 0)
            {
                phTransfers.Visible = true;

                BusinessUser bUser = new BusinessUser();
                BusinessCompany bCompany = new BusinessCompany();
                BusinessProduct bProduct = new BusinessProduct();

                TypeAction action = null;
                User otherUser = null;

                string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);

                if (transfersBy.Count > 0)
                {
                    Panel infoPnl = new Panel();
                    phTransfers.Controls.Add(infoPnl);
                    infoPnl.CssClass = "sectionTextHeader";

                    Label infoLbl = new Label();
                    infoPnl.Controls.Add(infoLbl);
                    infoLbl.Text = GetLocalResourceObject("currTransfRoles").ToString();

                    foreach (TransferTypeAction transfer in transfersBy)
                    {
                        Panel newPanel = new Panel();
                        phTransfers.Controls.Add(newPanel);
                        newPanel.CssClass = "panelRows";

                        if (!transfer.UserTypeActionReference.IsLoaded)
                        {
                            transfer.UserTypeActionReference.Load();
                        }
                        if (!transfer.UserTypeAction.TypeActionReference.IsLoaded)
                        {
                            transfer.UserTypeAction.TypeActionReference.Load();
                        }
                        if (!transfer.UserReceivingReference.IsLoaded)
                        {
                            transfer.UserReceivingReference.Load();
                        }

                        action = transfer.UserTypeAction.TypeAction;

                        ////
                        Label itemLbl = new Label();
                        HyperLink itemLink = new HyperLink();

                        GetActionTypeLinkAndLabel(action, bProduct, bCompany, "trn", contPageId, out itemLbl, out itemLink);

                        newPanel.Controls.Add(itemLbl);

                        Label sepLbl = new Label();
                        newPanel.Controls.Add(sepLbl);
                        sepLbl.Text = " : ";
                        sepLbl.CssClass = "searchPageRatings";

                        newPanel.Controls.Add(itemLink);
                        ////

                        Label toLbl = new Label();
                        newPanel.Controls.Add(toLbl);
                        toLbl.CssClass = "marginLeft";
                        toLbl.Text = string.Format("{0} ", GetLocalResourceObject("to").ToString());

                        otherUser = bUser.Get(userContext, transfer.UserReceiving.ID, true);
                        newPanel.Controls.Add(CommonCode.UiTools.GetUserHyperLink(otherUser));

                        Label dateLbl = new Label();
                        newPanel.Controls.Add(dateLbl);
                        dateLbl.CssClass = "marginLeft commentsDate";
                        dateLbl.Text = CommonCode.UiTools.DateTimeToLocalString(transfer.dateCreated);

                        if (!isVisited)
                        {
                            Panel buttonPnl = new Panel();
                            newPanel.Controls.Add(buttonPnl);
                            buttonPnl.CssClass = "floatRightNoMrg";

                            DecoratedButton dbDecline = new DecoratedButton();
                            buttonPnl.Controls.Add(dbDecline);
                            dbDecline.Attributes.Add("id", transfer.ID.ToString());
                            dbDecline.Text = GetGlobalResourceObject("SiteResources", "Decline").ToString();
                            dbDecline.Click += new EventHandler(dbDecline_Click);
                        }

                        if (!string.IsNullOrEmpty(transfer.description))
                        {
                            Panel descrPanel = new Panel();
                            newPanel.Controls.Add(descrPanel);
                            descrPanel.Controls.Add(CommonCode.UiTools.GetLabelWithText(
                                Tools.GetFormattedTextFromDB(transfer.description), false));
                        }
                    }

                    if (!isVisited == true)
                    {
                        Panel helpPnl = new Panel();
                        phTransfers.Controls.Add(helpPnl);
                        helpPnl.HorizontalAlign = HorizontalAlign.Right;

                        Label helplbl = new Label();
                        helpPnl.Controls.Add(helplbl);
                        helplbl.Text = string.Format("{0} {1} {2}", GetLocalResourceObject("transferTimeToAccept")
                            , Configuration.ActionTransactionNumDaysActive, GetLocalResourceObject("transferTimeToAccept2"));
                    }
                    phTransfers.Controls.Add(CommonCode.UiTools.GetNewLineControl());
                }

                if (transfersTo.Count > 0)
                {
                    Panel infoPnl = new Panel();
                    phTransfers.Controls.Add(infoPnl);
                    infoPnl.CssClass = "sectionTextHeader";

                    Label infoLbl = new Label();
                    infoPnl.Controls.Add(infoLbl);
                    infoLbl.Text = string.Format("{0} {1} :", GetLocalResourceObject("rolesTransfToYou"), forUser.username);

                    foreach (TransferTypeAction transfer in transfersTo)
                    {
                        Panel newPanel = new Panel();
                        phTransfers.Controls.Add(newPanel);
                        newPanel.CssClass = "panelRows";

                        if (!transfer.UserTypeActionReference.IsLoaded)
                        {
                            transfer.UserTypeActionReference.Load();
                        }
                        if (!transfer.UserTypeAction.TypeActionReference.IsLoaded)
                        {
                            transfer.UserTypeAction.TypeActionReference.Load();
                        }
                        if (!transfer.UserTransferingReference.IsLoaded)
                        {
                            transfer.UserTransferingReference.Load();
                        }

                        action = transfer.UserTypeAction.TypeAction;

                        ////
                        Label itemLbl = new Label();
                        HyperLink itemLink = new HyperLink();

                        GetActionTypeLinkAndLabel(action, bProduct, bCompany, "trn", contPageId, out itemLbl, out itemLink);

                        newPanel.Controls.Add(itemLbl);

                        Label sepLbl = new Label();
                        newPanel.Controls.Add(sepLbl);
                        sepLbl.Text = " : ";
                        sepLbl.CssClass = "searchPageRatings";

                        newPanel.Controls.Add(itemLink);
                        ////

                        Label toLbl = new Label();
                        newPanel.Controls.Add(toLbl);
                        toLbl.CssClass = "marginLeft";
                        toLbl.Text = string.Format("{0} ", GetLocalResourceObject("from").ToString());

                        otherUser = bUser.Get(userContext, transfer.UserTransfering.ID, true);
                        newPanel.Controls.Add(CommonCode.UiTools.GetUserHyperLink(otherUser));

                        Label dateLbl = new Label();
                        newPanel.Controls.Add(dateLbl);
                        dateLbl.CssClass = "marginLeft commentsDate";
                        dateLbl.Text = CommonCode.UiTools.DateTimeToLocalString(transfer.dateCreated);

                        if (!isVisited)
                        {
                            Panel buttonPnl = new Panel();
                            newPanel.Controls.Add(buttonPnl);
                            buttonPnl.CssClass = "floatRightNoMrg";

                            DecoratedButton dbAccept = new DecoratedButton();
                            buttonPnl.Controls.Add(dbAccept);
                            dbAccept.Attributes.Add("id", transfer.ID.ToString());
                            dbAccept.Text = GetGlobalResourceObject("SiteResources", "Accept").ToString();
                            dbAccept.Click += new EventHandler(dbAccept_Click);

                            DecoratedButton dbDecline = new DecoratedButton();
                            buttonPnl.Controls.Add(dbDecline);
                            dbDecline.Attributes.Add("id", transfer.ID.ToString());
                            dbDecline.Text = GetGlobalResourceObject("SiteResources", "Decline").ToString(); ;
                            dbDecline.Click += new EventHandler(dbDecline_Click);
                        }

                        if (!string.IsNullOrEmpty(transfer.description))
                        {
                            Panel descrPanel = new Panel();
                            newPanel.Controls.Add(descrPanel);
                            descrPanel.Controls.Add(CommonCode.UiTools.GetLabelWithText(
                                Tools.GetFormattedTextFromDB(transfer.description), false));
                        }
                    }

                    if (!isVisited == true)
                    {
                        Panel helpPnl = new Panel();
                        phTransfers.Controls.Add(helpPnl);
                        helpPnl.HorizontalAlign = HorizontalAlign.Right;

                        Label helplbl = new Label();
                        helpPnl.Controls.Add(helplbl);
                        helplbl.Text = string.Format("{0} {1} {2}", GetLocalResourceObject("transferToMeAccept")
                            , Configuration.ActionTransactionNumDaysActive
                            , GetLocalResourceObject("transferToMeAccept2"));
                    }

                    phTransfers.Controls.Add(CommonCode.UiTools.GetNewLineControl());

                }


            }
            else
            {
                phTransfers.Visible = false;
            }
        }

        void dbDecline_Click(object sender, EventArgs e)
        {
            DecoratedButton btn = sender as DecoratedButton;
            if (btn != null)
            {
                BusinessTransferAction bTransfer = new BusinessTransferAction();

                long transferID = 0;
                string strId = btn.Attributes["id"];
                if (!long.TryParse(strId, out transferID))
                {
                    throw new CommonCode.UIException("couldn`t parse dbAccept.Attributes[id] to long");
                }

                TransferTypeAction currTransfer = bTransfer.Get(objectContext, transferID, true, true);
                if (!currTransfer.UserReceivingReference.IsLoaded)
                {
                    currTransfer.UserReceivingReference.Load();
                }
                if (!currTransfer.UserTransferingReference.IsLoaded)
                {
                    currTransfer.UserTransferingReference.Load();
                }

                if (currTransfer.UserReceiving.ID != currentUser.ID && currTransfer.UserTransfering.ID != currentUser.ID)
                {
                    throw new BusinessException(string.Format("User id : {0} cannot decline action transfer ID : {1}, because he is not the one transfering or receiving."
                        , currentUser.ID, transferID));
                }

                bTransfer.DeclineTransfer(objectContext, userContext, currentUser, currTransfer, businessLog, false);

                ShowInfo();
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                    , GetLocalResourceObject("transferDeclined").ToString());
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }
        }

        void dbAccept_Click(object sender, EventArgs e)
        {
            DecoratedButton btn = sender as DecoratedButton;
            if (btn != null)
            {
                BusinessTransferAction bTransfer = new BusinessTransferAction();

                long transferID = 0;
                string strId = btn.Attributes["id"];
                if (!long.TryParse(strId, out transferID))
                {
                    throw new CommonCode.UIException("couldn`t parse dbAccept.Attributes[id] to long");
                }

                TransferTypeAction currTransfer = bTransfer.Get(objectContext, transferID, true, true);
                if (!currTransfer.UserReceivingReference.IsLoaded)
                {
                    currTransfer.UserReceivingReference.Load();
                }

                if (currTransfer.UserReceiving.ID != currentUser.ID)
                {
                    throw new BusinessException(string.Format("User id : {0} cannot accept action transfer ID : {1}, because he is not the receiver."
                        , currentUser.ID, transferID));
                }

                bTransfer.AcceptTransfer(objectContext, userContext, currentUser, currTransfer, businessLog);

                ShowInfo();
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                    , GetLocalResourceObject("transferAccepted").ToString());
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }
        }

        public void GetActionTypeLinkAndLabel(TypeAction action, BusinessProduct bProduct, BusinessCompany bCompany
            , string prefix, string contPageId, out Label itemLbl, out HyperLink itemLink)
        {
            if (action == null)
            {
                throw new CommonCode.UIException("action is null");
            }
            if (bProduct == null)
            {
                throw new CommonCode.UIException("bProduct is null");
            }
            if (bCompany == null)
            {
                throw new CommonCode.UIException("bCompany is null");
            }
            if (string.IsNullOrEmpty(prefix))
            {
                throw new CommonCode.UIException("prefix is empty");
            }

            itemLbl = new Label();
            itemLink = new HyperLink();

            switch (action.type)
            {
                case ("product"):
                    itemLbl.Text = GetGlobalResourceObject("SiteResources", "product").ToString();
                    itemLink = CommonCode.UiTools.GetProductHyperLink(objectContext, bProduct, action.typeID);

                    itemLink.ID = string.Format("{0}{1}prod{2}", prefix, action.ID, action.typeID);
                    itemLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}{2}','{3}')", action.typeID, contPageId, itemLink.ClientID, pnlPopUp.ClientID));
                    itemLink.Attributes.Add("onmouseout", "HideData()");
                    break;
                case ("company"):
                    itemLbl.Text = GetGlobalResourceObject("SiteResources", "CompanyName").ToString();
                    itemLink = CommonCode.UiTools.GetCompanyHyperLink(objectContext, bCompany, action.typeID);

                    itemLink.ID = string.Format("{0}{1}comp{2}", prefix, action.ID, action.typeID);
                    itemLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}{2}','{3}')", action.typeID, contPageId, itemLink.ClientID, pnlPopUp.ClientID));
                    itemLink.Attributes.Add("onmouseout", "HideData()");
                    break;
                default:
                    throw new CommonCode.UIException(string.Format("Action Type = {0} is not supported type , user id = {0}", action.type, currentUser.ID));
            }


        }

        private void FillEditRoles()
        {
            phEditRoles.Controls.Clear();

            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();
            List<TypeAction> userActions = new List<TypeAction>();

            bool activatedUser = true;
            if (isVisited)
            {
                userActions = businessUserTypeActions.GetUserModificatorRoles(objectContext, visiterUser, true);
                if (!visiterUser.UserOptionsReference.IsLoaded)
                {
                    visiterUser.UserOptionsReference.Load();
                }
                activatedUser = visiterUser.UserOptions.activated;
            }
            else
            {
                userActions = businessUserTypeActions.GetUserModificatorRoles(objectContext, currentUser, true);
            }

            if (userActions.Count() > 0)
            {
                phEditRoles.Visible = true;

                lblRolesInfo.Text = string.Format("{0} {1}", GetLocalResourceObject("editRoles"), userActions.Count());

                userActions.Reverse();

                int i = 0;
                bool transfer = false;
                string roleFor = string.Empty;

                string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);

                Panel mainPnl = new Panel();
                phEditRoles.Controls.Add(mainPnl);
                mainPnl.CssClass = "blueBorderPnl";

                foreach (TypeAction action in userActions)
                {
                    Panel newPanel = new Panel();
                    mainPnl.Controls.Add(newPanel);

                    if (i % 2 == 0)
                    {
                        newPanel.CssClass = "hoverPnl";
                    }
                    else
                    {
                        newPanel.CssClass = "hoverPnlNoBorder";
                    }

                    Panel role = new Panel();
                    newPanel.Controls.Add(role);
                    role.CssClass = "panelInline";
                    role.Width = Unit.Pixel(400);

                    Label itemLbl = new Label();
                    role.Controls.Add(itemLbl);

                    Label sepLbl = new Label();
                    role.Controls.Add(sepLbl);
                    sepLbl.Text = " : ";
                    sepLbl.CssClass = "searchPageRatings";

                    HyperLink itemLink = new HyperLink();

                    switch (action.type)
                    {
                        case ("product"):
                            itemLbl.Text = GetGlobalResourceObject("SiteResources", "product").ToString();
                            itemLink = CommonCode.UiTools.GetProductHyperLink(objectContext, action.typeID);

                            itemLink.ID = string.Format("act{0}prod{1}", action.ID, action.typeID);
                            itemLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}{2}','{3}')", action.typeID, contPageId, itemLink.ClientID, pnlPopUp.ClientID));
                            itemLink.Attributes.Add("onmouseout", "HideData()");

                            roleFor = string.Format("{0} {1}", itemLbl.Text, itemLink.Text);
                            transfer = true;
                            break;
                        case ("company"):
                            itemLbl.Text = GetGlobalResourceObject("SiteResources", "CompanyName").ToString();
                            itemLink = CommonCode.UiTools.GetCompanyHyperLink(objectContext, action.typeID);

                            itemLink.ID = string.Format("act{0}comp{1}", action.ID, action.typeID);
                            itemLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}{2}','{3}')", action.typeID, contPageId, itemLink.ClientID, pnlPopUp.ClientID));
                            itemLink.Attributes.Add("onmouseout", "HideData()");

                            roleFor = string.Format("{0} {1}", itemLbl.Text, itemLink.Text);
                            transfer = true;
                            break;
                        case ("aCompProdModificator"):
                            itemLbl.Text = GetLocalResourceObject("allMakerProducts").ToString();
                            itemLink = CommonCode.UiTools.GetCompanyHyperLink(objectContext, action.typeID);

                            itemLink.ID = string.Format("act{0}acomp{1}", action.ID, action.typeID);
                            itemLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}{2}','{3}')", action.typeID, contPageId, itemLink.ClientID, pnlPopUp.ClientID));
                            itemLink.Attributes.Add("onmouseout", "HideData()");

                            roleFor = string.Format("{0} {1} {2}", GetGlobalResourceObject("SiteResources", "CompanyName")
                                , itemLink.Text, GetGlobalResourceObject("SiteResources", "products"));
                            transfer = false;
                            break;
                        default:
                            String error = string.Format("Action Type = {0} is not supported type , user id = {0}", action.type, currentUser.ID);
                            throw new CommonCode.UIException(error);
                    }
                    role.Controls.Add(itemLink);

                    Label dateLbl = new Label();
                    newPanel.Controls.Add(dateLbl);

                    dateLbl.Text = string.Format("{0}    ", action.dateCreated.ToString("G", DateTimeFormatInfo.InvariantInfo));

                    if (!isVisited && transfer == true)
                    {
                        Label lblTransfer = new Label();
                        newPanel.Controls.Add(lblTransfer);
                        lblTransfer.ID = string.Format("transfer{0}", action.ID);
                        lblTransfer.Text = GetLocalResourceObject("transfer").ToString();
                        lblTransfer.CssClass = "pointerCursor searchPageComments";
                        lblTransfer.Attributes.Add("style", "margin-left:15px");

                        lblTransfer.Attributes.Add("onclick", string.Format("ShowTransferPnl('{0}','{1}','{2}','{3}','{4}','{5}')"
                            , lblTransfer.ClientID, action.ID, pnlTransfer.ClientID, pnlTransferEnd.ClientID,
                            roleFor, lblTransferRole.ClientID));
                    }

                    if (isVisited && canTakeAction == true)
                    {
                        Panel remove = new Panel();
                        newPanel.Controls.Add(remove);
                        remove.CssClass = "floatRight";

                        LinkButton dbTakeAction = new LinkButton();
                        remove.Controls.Add(dbTakeAction);

                        dbTakeAction.Text = GetLocalResourceObject("Take").ToString();
                        dbTakeAction.Click += new EventHandler(dbTakeAction_Click);
                        dbTakeAction.Attributes.Add("id", action.ID.ToString());
                    }

                    if (activatedUser && ((isVisited && canEdit) || !isVisited))
                    {
                        Panel remove = new Panel();
                        newPanel.Controls.Add(remove);
                        remove.CssClass = "floatRight";

                        LinkButton remBtn = new LinkButton();
                        remBtn.CssClass = "searchPageRatings marginsLR";
                        remove.Controls.Add(remBtn);

                        remBtn.Attributes.Add("id", action.ID.ToString());
                        remBtn.ID = string.Format("removeAction{0}", action.ID);
                        remBtn.Text = GetGlobalResourceObject("SiteResources", "Remove").ToString();
                        remBtn.Click += new EventHandler(remBtn_Click);
                    }

                    i++;
                }
            }
            else
            {

                ShowHowToAddPnl();

                if (isVisited)
                {
                    lblRolesInfo.Text = GetLocalResourceObject("noRolesVisited").ToString();
                }
                else
                {
                    lblRolesInfo.Text = GetLocalResourceObject("noRoles").ToString();
                    lblRolesInfo.CssClass = "darkOrange";
                }

                phEditRoles.Visible = false;
            }

        }

        private void ShowHowToAddPnl()
        {
            if (isVisited == true)
            {
                pnlReadHowToAdd.Visible = false;
                return;
            }

            BusinessUser bUser = new BusinessUser();

            bool canAddProducts = bUser.CanUserDo(userContext, currentUser, UserRoles.AddProducts);
            bool canAddCompanies = bUser.CanUserDo(userContext, currentUser, UserRoles.AddCompanies);

            if (canAddCompanies == true || canAddProducts == true)
            {
                pnlReadHowToAdd.Visible = true;

                if (canAddProducts == true)
                {
                    pnlLearnHowToAddProduct.Visible = true;

                    hlClickToAddProduct.NavigateUrl = GetUrlWithVariant("AddProduct.aspx");
                    hlClickToReadHowToAddProduct.NavigateUrl = GetUrlWithVariant("Guide.aspx#infaeproducts");
                }
                else
                {
                    pnlLearnHowToAddProduct.Visible = false;
                }

                if (canAddCompanies == true)
                {
                    pnlLearnHowToAddCompany.Visible = true;

                    hlClickToAddCompany.NavigateUrl = GetUrlWithVariant("AddCompany.aspx");
                    hlClickToReadHowToAddCompany.NavigateUrl = GetUrlWithVariant("Guide.aspx#infaaemaker");
                }
                else
                {
                    pnlLearnHowToAddCompany.Visible = false;
                }
            }
            else
            {
                pnlReadHowToAdd.Visible = false;
            }


        }

        void dbTakeAction_Click(object sender, EventArgs e)
        {
            LinkButton button = sender as LinkButton;
            if (button == null)
            {
                throw new CommonCode.UIException("Couldn`t get button dbTakeAction");
            }

            if (!isVisited)
            {
                throw new CommonCode.UIException(string.Format("User ID : {0} is not visiting other user, so he can`t take actions."
                    , currentUser.ID));
            }

            if (!canTakeAction)
            {
                throw new CommonCode.UIException(string.Format("User ID : {0} can`t take actions from user id : {1}"
                    , currentUser.ID, visiterUser.ID));
            }

            long actId = 0;
            if (!long.TryParse(button.Attributes["id"], out actId))
            {
                throw new CommonCode.UIException(string.Format("Couldn`t parse dbTakeAction.Attributes['id'] to long, User ID : {0}"
                    , currentUser.ID));
            }

            BusinessUserTypeActions butActions = new BusinessUserTypeActions();
            TypeAction action = butActions.GetTypeAction(objectContext, actId);
            if (action == null)
            {
                throw new CommonCode.UIException(string.Format("No TypeAction with ID : {0}, User id : {1}"
                    , actId, currentUser.ID));
            }

            UsersTypeAction userAction = butActions.GetUserTypeAction(objectContext, currentUser.ID, actId, false);
            if (userAction != null)
            {
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("YouHaveRole").ToString());
                return;
            }

            userAction = butActions.GetUserTypeAction(objectContext, visiterUser.ID, actId, true);

            butActions.TakeActionFromUser(userContext, objectContext, currentUser, visiterUser, userAction, businessLog);
            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                , GetLocalResourceObject("roleTaken").ToString());

            ShowInfo();
        }

        void remBtn_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn != null)
            {
                BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

                long actionID = 0;
                String action = btn.Attributes["id"];
                long.TryParse(action, out actionID);

                if (actionID > 0)
                {
                    UsersTypeAction userAction = null;
                    if (isVisited)
                    {
                        if (!canEdit)
                        {
                            throw new CommonCode.UIException(string.Format("User : {0}, ID : {1}, cannot remove user action ID : {2} on User : {3}, ID : {4} because HE doesn`t have roles to do this."
                                , currentUser.username, currentUser.ID, actionID, visiterUser.username, visiterUser.ID));
                        }

                        userAction = businessUserTypeActions.GetUserTypeAction(objectContext, visiterUser.ID, actionID, true);

                        if (userAction == null)
                        {
                            throw new CommonCode.UIException(string.Format("User : {0}, ID : {1}, cannot remove user action ID = {2} on User : {3}, ID : {4} because there isn`t such"
                                , currentUser.username, currentUser.ID, actionID, visiterUser.username, visiterUser.ID));
                        }

                        if (!visiterUser.UserOptionsReference.IsLoaded)
                        {
                            visiterUser.UserOptionsReference.Load();
                        }
                        if (visiterUser.UserOptions.activated == false)
                        {
                            throw new CommonCode.UIException(string.Format("User ID : {0} cannot remove type role ID : {1} on User ID : {2}, because he is not activated."
                                , currentUser.ID, userAction.ID, visiterUser.ID));
                        }
                    }
                    else
                    {
                        userAction = businessUserTypeActions.GetUserTypeAction(objectContext, currentUser.ID, actionID, true);

                        if (userAction == null)
                        {
                            throw new CommonCode.UIException(string.Format("User : {0}, ID : {1}, cannot remove user action ID = {2}, because it isn`t his (or no such action)"
                                , currentUser.username, currentUser.ID, actionID));
                        }
                    }

                    if (isVisited == true)
                    {
                        businessUserTypeActions.RemoveUserTypeAction(objectContext, userContext, userAction, currentUser, businessLog, true);
                    }
                    else
                    {
                        businessUserTypeActions.RemoveUserTypeAction(objectContext, userContext, userAction, currentUser, businessLog, false);
                    }

                    ShowInfo();
                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("roleRemoved").ToString());
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("actionID is < 1 (comming from btn.Attributes['id']) , user id = {0}", currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }
        }


        private void CheckUserAndParams()
        {
            BusinessUser businessUser = new BusinessUser();

            User currUser = GetCurrentUser(userContext, objectContext);
            currentUser = currUser;

            Visited(businessUser, currUser);

            if (!isVisited)
            {
                if (currUser == null)
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                        GetGlobalResourceObject("SiteResources", "errorHaveToBeLogged").ToString());
                }
                else if (!businessUser.IsFromUserTeam(currUser))
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                        GetGlobalResourceObject("SiteResources", "errorPageForUsersOnly").ToString());
                }
            }

        }

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }

        private void Visited(BusinessUser businessUser, User currUser)
        {
            String userParam = Request.Params["User"];
            if (!string.IsNullOrEmpty(userParam))
            {
                long id = -1;
                long.TryParse(userParam, out id);

                if (id < 1)
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                        GetLocalResourceObject("errorIncParameters").ToString());
                }
                else
                {
                    visiterUser = businessUser.GetWithoutVisible(userContext, id, false);

                    if (visiterUser == null)
                    {
                        CommonCode.UiTools.RedirrectToErrorPage(Response, Session
                            , GetGlobalResourceObject("SiteResources", "errorNoSuchUser").ToString());
                    }

                    if (!businessUser.IsUserValidType(visiterUser))
                    {
                        CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                            GetGlobalResourceObject("SiteResources", "errorNoSuchUser").ToString());
                    }

                    if (visiterUser.visible == false &&
                        (currUser == null || !businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditUsers)))
                    {

                        CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                            GetGlobalResourceObject("SiteResources", "errorNoSuchUser").ToString());

                    }

                    if (currUser != null)
                    {
                        if (visiterUser == currUser)
                        {
                            RedirectToOtherUrl("EditorRights.aspx");
                        }

                        if (businessUser.IsFromUserTeam(currUser))
                        {
                            if (businessUser.IsFromUserTeam(visiterUser))
                            {
                                // ok ..no editing options
                                canEdit = false;
                            }
                            else
                            {
                                CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                                    GetLocalResourceObject("errorCantSeePage").ToString());
                            }
                        }
                        else
                        {
                            if (businessUser.IsFromAdminTeam(visiterUser))
                            {
                                CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                                    GetLocalResourceObject("errorCantSeePage").ToString());
                            }
                            else
                            {
                                // ok ..admin observing user page

                                if (!businessUser.IsModerator(currentUser)
                                    && businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditUsers))
                                {
                                    canEdit = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        canEdit = false;
                    }

                    isVisited = true;

                }
            }
        }


        [WebMethod]
        public static string WMCreateTransfer(string id, string name, string description)
        {
            return CommonCode.WebMethods.TransferAction(id, name, description);
        }


    }
}
