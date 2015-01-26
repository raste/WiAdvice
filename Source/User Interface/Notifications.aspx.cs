﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

using BusinessLayer;
using DataAccess;
using CustomServerControls;

namespace UserInterface
{
    public partial class Notifications : BasePage
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

            BusinessUserOptions bUserOptions = new BusinessUserOptions();
            bUserOptions.ChangeIfUserHaveNewContentOnNotifies(objectContext, currentUser, false);

            FillNotifies();

            lblInfo.Text = GetLocalResourceObject("info").ToString();
        }

        private Control getPaddingDiv(string cssclass, string text)
        {
            Panel pnl = new Panel();
            pnl.CssClass = "notifCellClass " + cssclass;

            pnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(text, false));

            return pnl;
        }

        private void FillNotifies()
        {
            tblNotifies.Rows.Clear();

            BusinessNotifies businessNotifies = new BusinessNotifies();
            List<NotifyOnNewContent> infoNotifies = businessNotifies.GetUserNotifiesWithNewInformation(objectContext, currentUser);
            List<NotifyOnNewContent> noInfoNotifies = businessNotifies.GetUserNotifiesWithOutNewInformation(objectContext, currentUser);

            if (infoNotifies.Count > 0 || noInfoNotifies.Count > 0)
            {
                TableRow firstRow = new TableRow();
                tblNotifies.Rows.Add(firstRow);

                TableCell fTypeCell = new TableCell();
                firstRow.Cells.Add(fTypeCell);
                fTypeCell.Width = Unit.Pixel(100);
                fTypeCell.Text = GetLocalResourceObject("Type").ToString();
                fTypeCell.CssClass = "notifiesTypeMain";

                TableCell fLinkCell = new TableCell();
                firstRow.Cells.Add(fLinkCell);
                fLinkCell.Width = Unit.Pixel(350);
                fLinkCell.Text = GetLocalResourceObject("Link").ToString();
                fLinkCell.CssClass = "notifiesLinkMain";

                TableCell fInfoCell = new TableCell();
                firstRow.Cells.Add(fInfoCell);
                fInfoCell.Text = GetLocalResourceObject("NewInformation").ToString();
                fInfoCell.CssClass = "searchPageComments notifiesInfoMain";

                TableCell fRemoveCell = new TableCell();
                firstRow.Cells.Add(fRemoveCell);

                NotifyType currType = NotifyType.Company;

                int i = 0;

                if (infoNotifies.Count > 0)
                {
                    foreach (NotifyOnNewContent notify in infoNotifies)
                    {
                        TableRow newRow = new TableRow();
                        tblNotifies.Rows.Add(newRow);

                        TableCell typeCell = new TableCell();
                        newRow.Cells.Add(typeCell);
                        typeCell.CssClass = "notifiesTypeRest notifiesTypeRestFont";

                        currType = BusinessNotifies.GetTypeFromString(notify);

                        switch (currType)
                        {
                            case NotifyType.Product:
                                typeCell.Text = GetGlobalResourceObject("SiteResources", "product").ToString();
                                break;
                            case NotifyType.Company:
                                typeCell.Text = GetGlobalResourceObject("SiteResources", "CompanyName").ToString();
                                break;
                            case NotifyType.ProductForum:
                                typeCell.Text = GetLocalResourceObject("forum").ToString();
                                break;
                            case NotifyType.ProductTopic:
                                typeCell.Text = GetLocalResourceObject("topic").ToString();
                                break;
                            default:
                                throw new CommonCode.UIException(string.Format("Notify ID : {0} type = {1} is not supported type."
                                    , notify.ID, notify.type));
                        }


                        TableCell typeLinkCell = new TableCell();
                        newRow.Cells.Add(typeLinkCell);
                        typeLinkCell.CssClass = "notifiesLinkRest";

                        AddTypeLinkToCell(typeLinkCell, notify, currType);

                        TableCell infoCell = new TableCell();
                        newRow.Cells.Add(infoCell);
                        infoCell.CssClass = "searchPageComments notifiesInfoRest";

                        switch (currType)
                        {
                            case NotifyType.Company:
                                infoCell.Text = GetLocalResourceObject("NewProducts").ToString();
                                break;
                            case NotifyType.Product:
                                infoCell.Text = GetLocalResourceObject("NewOpinions").ToString();
                                break;
                            case NotifyType.ProductForum:
                                infoCell.Text = GetLocalResourceObject("NewTopics").ToString();
                                break;
                            case NotifyType.ProductTopic:
                                infoCell.Text = GetLocalResourceObject("NewComments").ToString();
                                break;
                            default:
                                throw new CommonCode.UIException(string.Format("Notification type = {0} is not supported type", currType));
                        }

                        TableCell remCell = new TableCell();
                        newRow.Cells.Add(remCell);
                        remCell.CssClass = "notifiesButtonRest";

                        DecoratedButton button = new DecoratedButton();
                        remCell.Controls.Add(button);
                        button.ID = string.Format("remNotif{0}", notify.ID);
                        button.Text = GetLocalResourceObject("Remove").ToString();
                        button.Attributes.Add("id", notify.ID.ToString());
                        button.Click += new EventHandler(RemoveNotification);

                        if (i % 2 == 0)
                        {
                            typeCell.BackColor = CommonCode.UiTools.GetLightBlueColor();
                            typeLinkCell.BackColor = CommonCode.UiTools.GetLightBlueColor();
                            infoCell.BackColor = CommonCode.UiTools.GetLightBlueColor();
                        }
                        else
                        {
                            typeCell.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                            typeLinkCell.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                            infoCell.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                        }

                        i++;
                    }
                }

                if (noInfoNotifies.Count > 0)
                {
                    if (i % 2 == 0)
                    {
                        i = 2;
                    }
                    else
                    {
                        i = 1;
                    }

                    foreach (NotifyOnNewContent notify in noInfoNotifies)
                    {
                        TableRow newRow = new TableRow();
                        tblNotifies.Rows.Add(newRow);

                        TableCell typeCell = new TableCell();
                        newRow.Cells.Add(typeCell);
                        typeCell.CssClass = "notifiesTypeRest notifiesTypeRestFont";

                        currType = BusinessNotifies.GetTypeFromString(notify);

                        switch (currType)
                        {
                            case NotifyType.Product:
                                typeCell.Text = GetGlobalResourceObject("SiteResources", "product").ToString();
                                break;
                            case NotifyType.Company:
                                typeCell.Text = GetGlobalResourceObject("SiteResources", "CompanyName").ToString();
                                break;
                            case NotifyType.ProductForum:
                                typeCell.Text = GetLocalResourceObject("forum").ToString();
                                break;
                            case NotifyType.ProductTopic:
                                typeCell.Text = GetLocalResourceObject("topic").ToString();
                                break;
                            default:
                                throw new CommonCode.UIException(string.Format("Notify ID : {0} type = {1} is not supported type."
                                    , notify.ID, notify.type));
                        }

                        TableCell typeLinkCell = new TableCell();
                        newRow.Cells.Add(typeLinkCell);
                        typeLinkCell.CssClass = "notifiesLinkRest";

                        AddTypeLinkToCell(typeLinkCell, notify, currType);

                        TableCell infoCell = new TableCell();
                        newRow.Cells.Add(infoCell);
                        infoCell.Text = GetLocalResourceObject("NoNewContent").ToString();
                        infoCell.CssClass = "notifiesInfoRest";

                        TableCell remCell = new TableCell();
                        newRow.Cells.Add(remCell);
                        remCell.CssClass = "notifiesButtonRest";

                        DecoratedButton button = new DecoratedButton();
                        remCell.Controls.Add(button);
                        button.ID = string.Format("remNotif{0}", notify.ID);
                        button.Text = GetLocalResourceObject("Remove").ToString();
                        button.Attributes.Add("id", notify.ID.ToString());
                        button.Click += new EventHandler(RemoveNotification);

                        if (i % 2 == 0)
                        {
                            typeCell.BackColor = CommonCode.UiTools.GetLightBlueColor();
                            typeLinkCell.BackColor = CommonCode.UiTools.GetLightBlueColor();
                            infoCell.BackColor = CommonCode.UiTools.GetLightBlueColor();
                        }
                        else
                        {
                            typeCell.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                            typeLinkCell.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                            infoCell.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                        }

                        i++;
                    }
                }
            }
            else
            {
                TableRow newRow = new TableRow();
                tblNotifies.Rows.Add(newRow);

                TableCell newCell = new TableCell();
                newRow.Cells.Add(newCell);

                newCell.HorizontalAlign = HorizontalAlign.Center;
                newCell.CssClass = "searchPageRatings";
                newCell.Font.Size = FontUnit.Large;
                newCell.Text = GetLocalResourceObject("HaventSigned").ToString();
            }
        }

        void RemoveNotification(object sender, EventArgs e)
        {
            Button btnRemove = sender as Button;
            if (btnRemove != null)
            {
                long notifID = -1;
                string IdStr = btnRemove.Attributes["id"];
                if (long.TryParse(IdStr, out notifID))
                {
                    BusinessNotifies businessNotifies = new BusinessNotifies();

                    NotifyOnNewContent currNotify = businessNotifies.Get(objectContext, notifID);
                    if (currNotify == null)
                    {
                        throw new CommonCode.UIException(string.Format("Theres no notify ID = {0} , user id = {1}.", notifID, currentUser.ID));
                    }

                    if (!currNotify.UserReference.IsLoaded)
                    {
                        currNotify.UserReference.Load();
                    }
                    if (currNotify.User.ID != currentUser.ID)
                    {
                        throw new CommonCode.UIException(string.Format("User ID : {0} cannot remove notification ID : {1}, because it isn`t for him."
                            , currentUser.ID, currNotify.ID));
                    }

                    businessNotifies.RemoveNotify(objectContext, businessLog, currentUser, currNotify);

                    ShowInfo();

                    string unsignFrom = string.Empty;
                    string unsignDescr = string.Empty;
                    NotifyType currType = BusinessNotifies.GetTypeFromString(currNotify);

                    switch (currType)
                    {
                        case NotifyType.Product:

                            BusinessProduct businessProduct = new BusinessProduct();

                            Product currProduct = businessProduct.GetProductByIDWV(objectContext, currNotify.typeID);
                            if (currProduct == null)
                            {
                                throw new CommonCode.UIException(string.Format("There is no product ID : {0} (ot it is visible false) which is being tracked for notifies by user ID : {1}"
                                    , currNotify.typeID, currentUser.ID));
                            }

                            unsignFrom = currProduct.name;
                            unsignDescr = GetLocalResourceObject("Unsigned").ToString();

                            break;
                        case NotifyType.Company:

                            BusinessCompany businessCompany = new BusinessCompany();

                            Company currCompany = businessCompany.GetCompanyWV(objectContext, currNotify.typeID);
                            if (currCompany == null)
                            {
                                throw new CommonCode.UIException(string.Format("There is no company ID : {0} (ot it is visible false) which is being tracked for notifies by user ID : {1}"
                                    , currNotify.typeID, currentUser.ID));
                            }

                            unsignFrom = currCompany.name;
                            unsignDescr = GetLocalResourceObject("Unsigned").ToString();

                            break;

                        case NotifyType.ProductForum:

                            BusinessProduct bProduct = new BusinessProduct();

                            Product cProduct = bProduct.GetProductByIDWV(objectContext, currNotify.typeID);
                            if (cProduct == null)
                            {
                                throw new CommonCode.UIException(string.Format("There is no product ID : {0} which is being tracked for notifies by user ID : {1}"
                                    , currNotify.typeID, currentUser.ID));
                            }

                            unsignFrom = cProduct.name;
                            unsignDescr = GetLocalResourceObject("UnsignedForum").ToString();

                            break;

                        case NotifyType.ProductTopic:

                            BusinessProductTopics bpTopic = new BusinessProductTopics();
                            ProductTopic topic = bpTopic.Get(objectContext, currNotify.typeID, false, true);

                            unsignFrom = string.Format("\" {0} \"", topic.name);
                            unsignDescr = GetLocalResourceObject("UnsignedTopic").ToString();

                            break;

                        default:
                            throw new CommonCode.UIException(string.Format("Notification type = {0} is not supported type", currType));
                    }

                    unsignFrom = string.Format("{0} {1}!", unsignDescr, unsignFrom);

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, unsignFrom);
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("Couldnt parse btnRemove.Attributes['id'] to long , user id = {1}", IdStr, currentUser.ID));
                }

            }
            else
            {
                throw new CommonCode.UIException("couldn`t get Button");
            }

        }

        private void AddTypeLinkToCell(TableCell currCell, NotifyOnNewContent currNotify, NotifyType currType)
        {

            switch (currType)
            {
                case NotifyType.Product:

                    BusinessProduct businessProduct = new BusinessProduct();

                    Product currProduct = businessProduct.GetProductByID(objectContext, currNotify.typeID);
                    if (currProduct == null)
                    {
                        throw new CommonCode.UIException(string.Format("There is no product ID : {0} (ot it is visible false) which is being tracked for notifies by user ID : {1}"
                            , currNotify.typeID, currentUser.ID));
                    }

                    HyperLink prodLink = CommonCode.UiTools.GetProductHyperLink(currProduct);
                    currCell.Controls.Add(prodLink);
                    prodLink.Text = Tools.BreakLongWordsInString(currProduct.name, 35);

                    prodLink.ID = string.Format("prod{0}", currProduct.ID);
                    prodLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}','{2}')", currProduct.ID, prodLink.ClientID, pnlPopUp.ClientID));
                    prodLink.Attributes.Add("onmouseout", "HideData()");

                    break;
                case NotifyType.Company:

                    BusinessCompany businessCompany = new BusinessCompany();

                    Company currCompany = businessCompany.GetCompany(objectContext, currNotify.typeID);
                    if (currCompany == null)
                    {
                        throw new CommonCode.UIException(string.Format("There is no company ID : {0} (or it is visible false) which is being tracked for notifies by user ID : {1}"
                            , currNotify.typeID, currentUser.ID));
                    }

                    HyperLink compLink = CommonCode.UiTools.GetCompanyHyperLink(currCompany);
                    currCell.Controls.Add(compLink);
                    compLink.Text = Tools.BreakLongWordsInString(currCompany.name, 35);

                    compLink.ID = string.Format("comp{0}", currCompany.ID);
                    compLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}','{2}')", currCompany.ID, compLink.ClientID, pnlPopUp.ClientID));
                    compLink.Attributes.Add("onmouseout", "HideData()");

                    break;

                case NotifyType.ProductForum:

                    BusinessProduct bProduct = new BusinessProduct();

                    Product cProduct = bProduct.GetProductByIDWV(objectContext, currNotify.typeID);
                    if (cProduct == null)
                    {
                        throw new CommonCode.UIException(string.Format("There is no product ID : {0} which is being tracked for notifies by user ID : {1}"
                            , currNotify.typeID, currentUser.ID));
                    }

                    HyperLink forumLink = new HyperLink();
                    currCell.Controls.Add(forumLink);

                    forumLink.Text = Tools.BreakLongWordsInString(cProduct.name, 35);
                    forumLink.NavigateUrl = GetUrlWithVariant(string.Format("Forum.aspx?Product={0}", cProduct.ID));

                    forumLink.ID = string.Format("prod{0}forum", cProduct.ID);
                    forumLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}','{2}')", cProduct.ID, forumLink.ClientID, pnlPopUp.ClientID));
                    forumLink.Attributes.Add("onmouseout", "HideData()");

                    break;
                case NotifyType.ProductTopic:

                    BusinessProductTopics bpTopic = new BusinessProductTopics();
                    ProductTopic topic = bpTopic.Get(objectContext, currNotify.typeID, false, true);

                    HyperLink topicLink = new HyperLink();
                    currCell.Controls.Add(topicLink);

                    topicLink.Text = Tools.BreakLongWordsInString(topic.name, 35);
                    topicLink.NavigateUrl = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", topic.ID));

                    break;
                default:
                    throw new CommonCode.UIException(string.Format("Notification type = {0} is not supported type", currType));
            }
        }

        private void CheckUserAndParams()
        {
            User currUser = GetCurrentUser(userContext, objectContext);
            currentUser = currUser;

            if (currUser == null)
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session
                    , GetGlobalResourceObject("SiteResources", "errorHaveToBeLogged").ToString());
            }
        }

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }

    }
}
