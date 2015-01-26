﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Threading;

using DataAccess;
using BusinessLayer;
using AjaxControlToolkit;

namespace UserInterface.CommonCode
{
    public class UiTools
    {

        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(UiTools));

        /// <summary>
        /// Returns table cell with hyperlink to users page
        /// </summary>
        public static TableCell GetUserTableCell(EntitiesUsers userContext, long aboutUserId)
        {
            Tools.AssertObjectContextExists(userContext);

            BusinessUser businessUser = new BusinessUser();

            if (aboutUserId < 1)
            {
                throw new CommonCode.UIException("aboutUserId is < 1");
            }

            User aboutUser = businessUser.GetWithoutVisible(userContext, aboutUserId, true);

            TableCell newCell = new TableCell();

            if (businessUser.IsUserValidType(userContext, aboutUser.ID))
            {
                HyperLink userLink = new HyperLink();
                userLink.Text = aboutUser.username; ;
                userLink.NavigateUrl = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", aboutUser.ID));
                newCell.Controls.Add(userLink);
            }
            else
            {
                newCell.Text = aboutUser.username;
            }

            return newCell;
        }

        public static System.Drawing.Color GetStandardCellBgrColor()
        {
            return System.Drawing.Color.FromArgb(243, 243, 243);
        }

        public static System.Drawing.Color GetStandardGreenCellBgrColor()
        {
            return System.Drawing.Color.FromArgb(248, 255, 248);
        }

        public static System.Drawing.Color GetStandardGreenColor()
        {
            return System.Drawing.Color.FromArgb(66, 130, 32);
        }

        public static System.Drawing.Color GetStandardBlueColor()
        {
            return System.Drawing.Color.FromArgb(232, 242, 255);
        }

        public static System.Drawing.Color GetLightBlueColor()
        {
            return System.Drawing.Color.FromArgb(240, 247, 255);
        }

        /// <summary>
        /// Returns control with ID = controlID in AccordionPane, if not found throws exception
        /// </summary>
        public static Control GetControlFromAccordionPane(AjaxControlToolkit.AccordionPane pane, string controlID)
        {
            if (pane == null)
            {
                throw new CommonCode.UIException("AjaxControlToolkit.AccordionPane is null");
            }
            if (string.IsNullOrEmpty(controlID))
            {
                throw new CommonCode.UIException("controlID is null");
            }

            Control wantedControl = pane.FindControl(controlID);
            if (wantedControl == null)
            {
                throw new CommonCode.UIException(string.Format("Control with ID = {0} could not be found in Accordion pane ID = {1}"
                    , controlID, pane.ID));
            }

            return wantedControl;
        }

        /// <summary>
        /// Returns table cell with hyperlink to users page
        /// </summary>
        public static TableCell GetUserTableCell(User user)
        {
            if (user == null)
            {
                throw new CommonCode.UIException("user is null");
            }

            TableCell newCell = new TableCell();
            BusinessUser businessUser = new BusinessUser();
            if (businessUser.IsUserValidType(user))
            {
                HyperLink userLink = new HyperLink();
                userLink.Text = user.username; ;
                userLink.NavigateUrl = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", user.ID));
                newCell.Controls.Add(userLink);
            }
            else
            {
                newCell.Text = user.username;
            }

            return newCell;
        }

        public static string GetCorrectedUrl(string url)
        {
            if (!url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
                && !url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                url = string.Format("http://{0}", url);
            }

            return url;
        }

        /// <summary>
        /// Returns hyperlink to user`s page
        /// </summary>
        public static HyperLink GetUserHyperLink(User user)
        {
            if (user == null)
            {
                throw new CommonCode.UIException("user is null");
            }

            HyperLink userLink = new HyperLink();
            userLink.Text = user.username; ;
            userLink.NavigateUrl = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", user.ID));

            return userLink;
        }

        /// <summary>
        /// Returns hyperlink to product`s page
        /// </summary>
        public static HyperLink GetProductHyperLink(Product product)
        {
            if (product == null)
            {
                throw new CommonCode.UIException("product is null");
            }
            HyperLink userLink = new HyperLink();
            userLink.Text = product.name;
            userLink.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", product.ID));

            return userLink;
        }

        /// <summary>
        /// Returns hyperlink to product`s page
        /// </summary>
        public static HyperLink GetProductHyperLink(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (id < 1)
            {
                throw new CommonCode.UIException("id < 1");
            }

            BusinessProduct businessProduct = new BusinessProduct();
            Product currProduct = businessProduct.GetProductByIDWV(objectContext, id);
            if (currProduct == null)
            {
                throw new CommonCode.UIException("currProduct is null");
            }

            HyperLink userLink = new HyperLink();
            userLink.Text = currProduct.name;
            userLink.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", currProduct.ID));

            return userLink;
        }

        public static HyperLink GetProductHyperLink(Entities objectContext, BusinessProduct businessProduct, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (id < 1)
            {
                throw new CommonCode.UIException("id < 1");
            }
            if (businessProduct == null)
            {
                throw new CommonCode.UIException("businessProduct is null");
            }

            Product currProduct = businessProduct.GetProductByIDWV(objectContext, id);
            if (currProduct == null)
            {
                throw new CommonCode.UIException("currProduct is null");
            }

            HyperLink userLink = new HyperLink();
            userLink.Text = currProduct.name;
            userLink.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", currProduct.ID));

            return userLink;
        }

        /// <summary>
        /// Returns hyperlink to category`s page
        /// </summary>
        public static HyperLink GetCategoryHyperLink(Category category)
        {
            if (category == null)
            {
                throw new CommonCode.UIException("product is null");
            }
            HyperLink userLink = new HyperLink();
            userLink.Text = category.name;
            userLink.NavigateUrl = GetUrlWithVariant(string.Format("Category.aspx?Category={0}", category.ID));

            return userLink;
        }

        /// <summary>
        /// Returns hyperlink to category`s page
        /// </summary>
        public static HyperLink GetCategoryHyperLink(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (id < 1)
            {
                throw new CommonCode.UIException("id < 1");
            }

            BusinessCategory businessCategory = new BusinessCategory();
            Category currCat = businessCategory.GetWithoutVisible(objectContext, id);
            if (currCat == null)
            {
                throw new CommonCode.UIException("currCat is null");
            }

            HyperLink userLink = new HyperLink();
            userLink.Text = currCat.name;
            userLink.NavigateUrl = GetUrlWithVariant(string.Format("Category.aspx?Category={0}", currCat.ID));

            return userLink;
        }

        /// <summary>
        /// Returns tablecell with hyperlink to category`s page
        /// </summary>
        public static TableCell GetCategoryCell(Category category)
        {
            if (category == null)
            {
                throw new CommonCode.UIException("category is null");
            }

            TableCell catCell = new TableCell();
            HyperLink catLink = new HyperLink();
            catLink.Text = category.name;
            catLink.NavigateUrl = GetUrlWithVariant(string.Format("Category.aspx?Category={0}", category.ID));
            catCell.Controls.Add(catLink);

            return catCell;
        }

        /// <summary>
        /// Returns tablecell with hyperlink to category`s page
        /// </summary>
        public static TableCell GetCategoryCell(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }

            BusinessCategory businessCategory = new BusinessCategory();
            Category currCat = businessCategory.GetWithoutVisible(objectContext, id);
            if (currCat == null)
            {
                throw new BusinessException(string.Format("Theres no category with ID = {0}", id));
            }

            TableCell catCell = new TableCell();
            HyperLink catLink = new HyperLink();
            catLink.Text = currCat.name;
            catLink.NavigateUrl = GetUrlWithVariant(string.Format("Category.aspx?Category={0}", currCat.ID));
            catCell.Controls.Add(catLink);

            return catCell;
        }

        /// <summary>
        /// Returns hyperlink to company`s page
        /// </summary>
        public static HyperLink GetCompanyHyperLink(Company company)
        {
            if (company == null)
            {
                throw new CommonCode.UIException("company is null");
            }
            HyperLink userLink = new HyperLink();
            userLink.Text = company.name;
            userLink.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", company.ID));

            return userLink;
        }

        /// <summary>
        /// Returns hyperlink to company`s page
        /// </summary>
        public static HyperLink GetCompanyHyperLink(Entities objectContext, BusinessCompany businessCompany, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (id < 1)
            {
                throw new CommonCode.UIException("id < 1");
            }
            if (businessCompany == null)
            {
                throw new UIException("businessCompany is null");
            }

            Company currComp = businessCompany.GetCompanyWV(objectContext, id);
            if (currComp == null)
            {
                throw new CommonCode.UIException("currComp is null");
            }

            HyperLink userLink = new HyperLink();
            userLink.Text = currComp.name;
            userLink.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", currComp.ID));

            return userLink;
        }

        public static HyperLink GetCompanyHyperLink(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (id < 1)
            {
                throw new CommonCode.UIException("id < 1");
            }

            BusinessCompany businessCompany = new BusinessCompany();

            Company currComp = businessCompany.GetCompanyWV(objectContext, id);
            if (currComp == null)
            {
                throw new CommonCode.UIException("currComp is null");
            }

            HyperLink userLink = new HyperLink();
            userLink.Text = currComp.name;
            userLink.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", currComp.ID));

            return userLink;
        }

        /// <summary>
        /// Returns current user, if it isnt logged throws exception
        /// </summary>
        public static User GetCurrentUserExcIfNull(EntitiesUsers userContext, Entities objectContext)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);

            BusinessUser businessUser = new BusinessUser();

            User currUser = GetCurrentUser(userContext, objectContext, false);
            if (currUser == null)
            {
                throw new BusinessException("Cannot get current user (guest, or deleted).");
            }

            return currUser;
        }

        /// <summary>
        /// Gets currentUser depending on Session[CommonCode.CurrentUser.CurrentUserIdKey], If the SessionID is > 0 and there is no such user (deleted, druring curr session)
        /// the SessionID will be set to NULL, user will be logged off and added as guest
        /// </summary>
        /// <param name="redirrectToCurrPage">TRUE only if the method is called from page, if from web method should be false.
        /// Redirects only if SessionID is > 0 and there is no such visible.true user</param>
        /// <returns></returns>
        public static User GetCurrentUser(EntitiesUsers userContext, Entities objectContext, bool redirrectToCurrPage)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            BusinessUser businessUser = new BusinessUser();
            User currUser = null;
            long userID = CommonCode.CurrentUser.GetCurrentUserId();
            if (userID > 0)
            {
                currUser = businessUser.Get(userContext, userID, false);

                if (currUser == null)
                {
                    // deleted
                    businessUser.RemoveLoggedUser(userID);
                    HttpContext.Current.Session[CommonCode.CurrentUser.CurrentUserIdKey] = null;

                    businessUser.AddGuest(CommonCode.CurrentUser.GetCurrentUserId());

                    BusinessStatistics businessStatistic = new BusinessStatistics();
                    businessStatistic.UserLoggedOut(userContext);

                    // redirect
                    if (redirrectToCurrPage == true)
                    {
                        RedirectToSameUrl(HttpContext.Current.Request.Url.ToString());
                    }
                }
            }

            return currUser;
        }

        /// <summary>
        /// Returns current user, null if it isnt logged
        /// </summary>
        public static User GetCurrentUserNoExc(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);
            BusinessUser businessUser = new BusinessUser();

            User currUser = businessUser.Get(userContext, CurrentUser.GetCurrentUserId(), false);
            return currUser;
        }

        /// <summary>
        /// Return TableCell with hyperlink to hyperlinkURL
        /// </summary>
        public static TableCell GetHyperLinkCell(string hyperlinkID, string hyperlinkURL, string hyperlinkText)
        {
            if (string.IsNullOrEmpty(hyperlinkID))
            {
                throw new CommonCode.UIException("hyperlinkID is null or empty");
            }
            if (string.IsNullOrEmpty(hyperlinkURL))
            {
                throw new CommonCode.UIException("hyperlinkURL is null or empty");
            }
            if (string.IsNullOrEmpty(hyperlinkText))
            {
                throw new CommonCode.UIException("hyperlinkText is null or empty");
            }

            TableCell newCell = new TableCell();
            HyperLink newHyperlink = new HyperLink();
            newHyperlink.ID = hyperlinkID;
            newHyperlink.Text = hyperlinkText;
            newHyperlink.NavigateUrl = hyperlinkURL;
            newCell.Controls.Add(newHyperlink);
            return newCell;
        }

        /// <summary>
        /// Returns hyperlink
        /// </summary>
        public static HyperLink GetHyperLink(string hyperlinkID, string hyperlinkURL, string hyperlinkText)
        {
            if (string.IsNullOrEmpty(hyperlinkID))
            {
                throw new CommonCode.UIException("hyperlinkID is null");
            }
            if (string.IsNullOrEmpty(hyperlinkURL))
            {
                throw new CommonCode.UIException("hyperlinkURL is null");
            }
            if (string.IsNullOrEmpty(hyperlinkText))
            {
                throw new CommonCode.UIException("hyperlinkText is null");
            }

            HyperLink newHyperlink = new HyperLink();
            newHyperlink.ID = hyperlinkID;
            newHyperlink.Text = hyperlinkText;
            newHyperlink.NavigateUrl = hyperlinkURL;
            return newHyperlink;
        }

        public static HyperLink GetHyperLink(string hyperlinkURL, string hyperlinkText)
        {
            if (string.IsNullOrEmpty(hyperlinkURL))
            {
                throw new CommonCode.UIException("hyperlinkURL is null");
            }
            if (string.IsNullOrEmpty(hyperlinkText))
            {
                throw new CommonCode.UIException("hyperlinkText is null");
            }

            HyperLink newHyperlink = new HyperLink();
            newHyperlink.Text = hyperlinkText;
            newHyperlink.NavigateUrl = hyperlinkURL;
            return newHyperlink;
        }

        /// <summary>
        /// Returns tablecell with hyperlink to product`s page
        /// </summary>
        public static TableCell GetProductLinkCell(Entities objectContext, long productID)
        {
            if (productID < 1)
            {
                throw new UIException("productID is <1");
            }

            Tools.AssertObjectContextExists(objectContext);
            BusinessProduct businessProduct = new BusinessProduct();
            Product currProduct = businessProduct.GetProductByID(objectContext, productID);
            if (currProduct == null)
            {
                throw new UIException(string.Format("Theres no product with id = {0}", productID));
            }

            TableCell newCell = new TableCell();
            HyperLink newHyper = new HyperLink();
            newHyper.Text = currProduct.name;
            newHyper.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", currProduct.ID));
            newCell.Controls.Add(newHyper);
            return newCell;
        }

        /// <summary>
        /// Returns table cell with hyperlink
        /// </summary>
        /// <param name="text">hyperlink text</param>
        /// <param name="URL">hyperlink url</param>
        /// <returns></returns>
        public static TableCell GetLinkCell(String text, String URL)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new UIException("text is null or empty");
            }
            if (string.IsNullOrEmpty(URL))
            {
                throw new UIException("URL is null or empty");
            }

            TableCell newCell = new TableCell();
            HyperLink newHyper = new HyperLink();
            newHyper.Text = text;
            newHyper.NavigateUrl = URL;
            newCell.Controls.Add(newHyper);
            return newCell;
        }

        /// <summary>
        /// Returns tablecell with hyperlink to product`s page
        /// </summary>
        public static TableCell GetProductLinkCell(Product product)
        {
            if (product == null)
            {
                throw new UIException("product is null");
            }

            TableCell newCell = new TableCell();
            HyperLink newHyper = new HyperLink();
            newHyper.Text = product.name;
            newHyper.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", product.ID));
            newCell.Controls.Add(newHyper);
            return newCell;
        }

        /// <summary>
        /// Returns tablecell with hyperlink to company`s page
        /// </summary>
        public static TableCell GetCompanyLinkCell(Entities objectContext, Company company)
        {
            BusinessCompany businessCompany = new BusinessCompany();

            if (company == null)
            {
                throw new UIException("company is null");
            }

            TableCell newCell = new TableCell();
            if (businessCompany.IsOther(objectContext, company))
            {
                newCell.Text = company.name;
            }
            else
            {
                HyperLink newHyper = new HyperLink();
                newHyper.Text = company.name;
                newHyper.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", company.ID));
                newCell.Controls.Add(newHyper);
            }
            return newCell;
        }

        public static TableCell GetCompanyLinkCell(Entities objectContext, BusinessCompany businessCompany, Company company)
        {
            if (company == null)
            {
                throw new UIException("company is null");
            }
            if (businessCompany == null)
            {
                throw new UIException("businessCompany is null");
            }

            TableCell newCell = new TableCell();
            if (businessCompany.IsOther(objectContext, company))
            {
                newCell.Text = company.name;
            }
            else
            {
                HyperLink newHyper = new HyperLink();
                newHyper.Text = company.name;
                newHyper.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", company.ID));
                newCell.Controls.Add(newHyper);
            }
            return newCell;
        }

        /// <summary>
        /// Returns tablecell with hyperlink to company`s page
        /// </summary>
        public static TableCell GetCompanyLinkCell(Entities objectContext, long companyID)
        {

            if (companyID < 1)
            {
                throw new UIException("companyID is <1");
            }

            Tools.AssertObjectContextExists(objectContext);
            BusinessCompany businessCompany = new BusinessCompany();
            Company currCompany = businessCompany.GetCompany(objectContext, companyID);
            if (currCompany == null)
            {
                throw new UIException(string.Format("There`s company with id = {0}", companyID));
            }

            TableCell newCell = new TableCell();
            HyperLink newHyper = new HyperLink();
            newHyper.Text = currCompany.name;
            newHyper.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", currCompany.ID));
            newCell.Controls.Add(newHyper);
            return newCell;
        }

        public static TableCell GetCompanyLinkCell(Entities objectContext, BusinessCompany businessCompany, long companyID)
        {

            if (companyID < 1)
            {
                throw new UIException("companyID is <1");
            }
            if (businessCompany == null)
            {
                throw new UIException("businessCompany is null");
            }

            Tools.AssertObjectContextExists(objectContext);
            Company currCompany = businessCompany.GetCompany(objectContext, companyID);
            if (currCompany == null)
            {
                throw new UIException(string.Format("There`s company with id = {0}", companyID));
            }

            TableCell newCell = new TableCell();
            HyperLink newHyper = new HyperLink();
            newHyper.Text = currCompany.name;
            newHyper.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", currCompany.ID));
            newCell.Controls.Add(newHyper);
            return newCell;
        }

        public static String GetCommentAbout(EntitiesUsers userContext, Entities objectContext,
            BusinessComment businessComment, Comment comment, User currentUser)
        {
            CommonCode.UiTools.ChangeUiCultureFromSession();

            CommentType commType = businessComment.GetCommentTypeFromString(comment.type);
            CommentSubType commSubType = businessComment.GetCommentSubTypeFromString(comment);
            String about = "";

            switch (commSubType)
            {
                case CommentSubType.Comment:

                    switch (commType)
                    {
                        case CommentType.Product:

                            if (!comment.ForCharacteristicReference.IsLoaded)
                            {
                                comment.ForCharacteristicReference.Load();
                            }
                            if (!comment.ForVariantReference.IsLoaded)
                            {
                                comment.ForVariantReference.Load();
                            }
                            if (!comment.ForSubVariantReference.IsLoaded)
                            {
                                comment.ForSubVariantReference.Load();
                            }

                            if (comment.ForCharacteristic != null)
                            {
                                about = string.Format("{0} {1} {2} ", HttpContext.GetGlobalResourceObject("UiTools", "Characteristic")
                                    , Tools.BreakLongWordsInString(comment.ForCharacteristic.name, 20)
                                    , HttpContext.GetGlobalResourceObject("UiTools", "inn"));
                            }
                            else if (comment.ForVariant != null)
                            {
                                about = string.Format("{0} {1} {2} ", HttpContext.GetGlobalResourceObject("UiTools", "Variant")
                                    , Tools.BreakLongWordsInString(comment.ForVariant.name, 20)
                                    , HttpContext.GetGlobalResourceObject("UiTools", "inn"));
                            }
                            else if (comment.ForSubVariant != null)
                            {
                                if (!comment.ForSubVariant.VariantReference.IsLoaded)
                                {
                                    comment.ForSubVariant.VariantReference.Load();
                                }

                                about = string.Format("{0} {1} : {2} {3} ", HttpContext.GetGlobalResourceObject("UiTools", "Variant2")
                                    , Tools.BreakLongWordsInString(comment.ForSubVariant.Variant.name, 20)
                                    , Tools.BreakLongWordsInString(comment.ForSubVariant.name, 20)
                                    , HttpContext.GetGlobalResourceObject("UiTools", "inn"));
                            }


                            break;
                        case CommentType.Topic:

                            about = string.Format("{0} ", HttpContext.GetGlobalResourceObject("UiTools", "topic"));

                            break;
                        default:
                            throw new CommonCode.UIException(string.Format("Comment type = {0} is not supported when showing user comments", commType));
                    }


                    break;
                case CommentSubType.SubComment:
                    Comment parent = businessComment.GetWithoutVisible(objectContext, comment.subTypeID);
                    if (parent == null)
                    {
                        throw new CommonCode.UIException(string.Format("SubComment ID = {0} `s parent is null , user id = {1}", comment.ID, currentUser.ID));
                    }

                    switch (commType)
                    {
                        case CommentType.Product:

                            about = string.Format("{0} {1} {2} {3} {4} ", HttpContext.GetGlobalResourceObject("UiTools", "replyToOpinionFrom")
                        , businessComment.GetCommentUsername(userContext, objectContext, comment.subTypeID)
                        , HttpContext.GetGlobalResourceObject("UiTools", "on"), CommonCode.UiTools.DateTimeToLocalString(parent.dateCreated)
                        , HttpContext.GetGlobalResourceObject("UiTools", "inProduct"));

                            break;
                        case CommentType.Topic:

                            about = string.Format("{0} {1} {2} {3} {4} ", HttpContext.GetGlobalResourceObject("UiTools", "replyToCommentFrom")
                        , businessComment.GetCommentUsername(userContext, objectContext, comment.subTypeID)
                        , HttpContext.GetGlobalResourceObject("UiTools", "on"), CommonCode.UiTools.DateTimeToLocalString(parent.dateCreated)
                        , HttpContext.GetGlobalResourceObject("UiTools", "inTopic"));

                            break;
                        default:
                            throw new CommonCode.UIException(string.Format("Comment type = {0} is not supported when showing user comments", commType));
                    }

                    break;
                default:
                    String error = string.Format("Comment type = {0} is not valid type , user id = {1}", commType, currentUser.ID);
                    throw new CommonCode.UIException(error);
            }

            return about;
        }

        public static HyperLink GetCommentIn(Entities objectContext, BusinessComment businessComment
            , Comment comment, Panel pnlPopUp, User currentUser)
        {
            HyperLink hyperlink = new HyperLink();

            string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);

            CommentType commType = businessComment.GetCommentTypeFromString(comment.type);

            switch (commType)
            {
                case CommentType.Product:

                    BusinessProduct businessProduct = new BusinessProduct();

                    Product product = businessProduct.GetProductByIDWV(objectContext, comment.typeID);
                    if (product == null)
                    {
                        string error = string.Format("Theres no product id = {0} , user id = {1}", comment.typeID, currentUser.ID);
                        throw new CommonCode.UIException(error);
                    }

                    hyperlink.ID = string.Format("prod{0}comm{1}", product.ID, comment.ID);
                    hyperlink.Text = Tools.BreakLongWordsInString(product.name, 20);
                    hyperlink.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", product.ID));
                    hyperlink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}{2}','{3}')", product.ID, contPageId, hyperlink.ClientID, pnlPopUp.ClientID));
                    hyperlink.Attributes.Add("onmouseout", "HideData()");
                    break;
                case CommentType.Topic:

                    BusinessProductTopics bpTopic = new BusinessProductTopics();

                    ProductTopic topic = bpTopic.Get(objectContext, comment.typeID, false, true);

                    hyperlink.Text = Tools.BreakLongWordsInString(topic.name, 20);
                    hyperlink.NavigateUrl = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", topic.ID));

                    break;
                default:
                    string error2 = string.Format("Comment about type = {0} is not supported type , user id = {1}", comment.type, currentUser.ID);
                    throw new CommonCode.UIException(error2);
            }

            return hyperlink;
        }

        /// <summary>
        /// Returns url to error page
        /// </summary>
        public static string GetErrorPageUrl()
        {
            return GetUrlWithVariant("Error.aspx");
        }

        /// <summary>
        /// Returns Label with 'text' and ForeColor=Red
        /// </summary>
        public static Label GetAdminLabel(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new CommonCode.UIException("text is null");
            }

            Label adminText = new Label();
            adminText.Text = Tools.GetFormattedTextFromDB(text);
            adminText.ForeColor = System.Drawing.Color.Red;
            return adminText;
        }

        /// <summary>
        /// Redirrects to error page with error
        /// </summary>
        public static void RedirrectToErrorPage(HttpResponse response, System.Web.SessionState.HttpSessionState session, string error)
        {
            if (response == null)
            {
                throw new UIException("HttpResponse is null");
            }

            if (session == null)
            {
                throw new UIException("HttpSessionState is null");
            }

            session["error"] = error;
            UiTools.RedirectToOtherUrl("Error.aspx");
        }

        /// <summary>
        /// Returns label with spaces ' ' = mumber
        /// </summary>
        public static Label GetSpaceLabel(int spaces)
        {
            if (spaces < 1)
            {
                throw new CommonCode.UIException("spaces is < 1");
            }

            System.Text.StringBuilder freeSpaces = new System.Text.StringBuilder();
            for (int i = 0; i < spaces; i++)
            {
                freeSpaces.Append("&nbsp;");
            }

            Label spaceLabel = new Label();
            spaceLabel.Text = freeSpaces.ToString();
            return spaceLabel;
        }

        /// <summary>
        /// Return label : ' , '
        /// </summary>
        public static Label GetComaLabel()
        {
            Label comaLbl = new Label();
            comaLbl.Text = "&nbsp;,&nbsp;";
            return comaLbl;
        }

        /// <summary>
        /// Returns Html 'Begin Row (br)' control
        /// </summary>
        public static Control GetNewLineControl()
        {
            Label newLabel = new Label();
            newLabel.Text = "<br />";

            return newLabel;
        }

        /// <summary>
        /// Returns (hr) control
        /// </summary>
        public static System.Web.UI.HtmlControls.HtmlGenericControl GetHorisontalLineControl()
        {
            return new System.Web.UI.HtmlControls.HtmlGenericControl("hr");
        }

        public static Control GetHorisontalFashionLinePanel(bool withMargin)
        {
            Panel panel = new Panel();

            if (withMargin == true)
            {
                panel.CssClass = "divBottomHr";
            }
            else
            {
                panel.CssClass = "divBottomHrNoMargin";
            }

            Image imgL = new Image();
            panel.Controls.Add(imgL);
            imgL.ImageUrl = "~/images/SiteImages/horL.png";
            imgL.ImageAlign = ImageAlign.Left;

            Image imgR = new Image();
            panel.Controls.Add(imgR);
            imgR.ImageUrl = "~/images/SiteImages/horR.png";
            imgR.ImageAlign = ImageAlign.Right;

            return panel;
        }

        /// <summary>
        /// Returns label with specified text
        /// </summary>
        /// <param name="withBRtagOnEnd">true if the text should end with (BR) tag, otherwise false</param>
        public static Label GetLabelWithText(string text, bool withBRtagOnEnd)
        {
            Label newLabel = new Label();
            if (withBRtagOnEnd)
            {
                newLabel.Text = string.Format("{0}<br/>", text);
            }
            else
            {
                newLabel.Text = text;
            }
            return newLabel;
        }

        /// <summary>
        /// Returns error messgae based on NoBotState
        /// </summary>
        public static string GetNoBotInvalidMessage(NoBotState state)
        {
            string error = state.ToString();

            CommonCode.UiTools.ChangeUiCultureFromSession();

            //1. Valid
            //2. InvalidBadResponse
            //3. InvalidResponseTooSoon
            //4. InvalidAddressTooActive
            //5. InvalidBadSession
            //6. InvalidUnknown

            switch (state.ToString())
            {
                case ("InvalidResponseTooSoon"):
                    error = HttpContext.GetGlobalResourceObject("UiTools", "InvalidResponseTooSoon").ToString();
                    break;
                case ("InvalidAddressTooActive"):
                    error = HttpContext.GetGlobalResourceObject("UiTools", "InvalidAddressTooActive").ToString();
                    break;
                default:
                    error = HttpContext.GetGlobalResourceObject("UiTools", "InvalidNoBotDefault").ToString();
                    break;
            }

            return error;
        }

        /// <summary>
        /// Returns PlaceHolder with path to the currCategory`s parent followed by Link to currCategory
        /// </summary>
        /// <param name="styled"> true if lbl and link should be with styles (used in category page)</param>
        public static PlaceHolder GetCategoryNameWithLink(Category currCategory, Entities objectContext
            , bool styledArrow, bool styledPath, bool withCategoryTextInfront)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new UIException("currCategory is null");
            }

            CommonCode.UiTools.ChangeUiCultureFromSession();

            PlaceHolder holder = new PlaceHolder();

            BusinessCategory businessCategory = new BusinessCategory();

            System.Text.StringBuilder catName = new System.Text.StringBuilder();

            if (styledArrow)
            {
                if (withCategoryTextInfront == true)
                {
                    catName.Append(string.Format("<span class=\"searchPageComments\">{0}</span> {1} ", HttpContext.GetGlobalResourceObject("UiTools", "Category2")
                        , "<span class=\"searchPageRatings\"> : </span>"));
                }
            }
            else
            {
                if (withCategoryTextInfront == true)
                {
                    catName.Append(string.Format("{0} ", HttpContext.GetGlobalResourceObject("UiTools", "Category").ToString()));
                }
            }


            if (currCategory.parentID != null)
            {
                Category parentCategory = businessCategory.GetWithoutVisible
                    (objectContext, currCategory.parentID.Value);
                if (parentCategory == null)
                {
                    throw new CommonCode.UIException(string.Format
                        ("There is no category with ID = {0} which is a parent category on category ID = {1}"
                        , currCategory.parentID.Value, currCategory.ID));
                }

                if (styledArrow)
                {
                    List<string> parentCats = Tools.CategoryNameAsList(objectContext, parentCategory);
                    foreach (string category in parentCats)
                    {
                        catName.Append(category);
                        catName.Append("<span class=\"searchPageRatings\"> > </span>");
                    }
                }
                else
                {
                    catName.Append(Tools.CategoryName(objectContext, parentCategory, false));
                    catName.Append(" > ");
                }

            }

            HyperLink productCategoryLink = CommonCode.UiTools.GetCategoryHyperLink(currCategory);
            Label catChain = new Label();
            catChain.Text = catName.ToString();

            if (styledPath == true)
            {
                catChain.CssClass = "textHeader";
                productCategoryLink.CssClass = "textHeader";
            }

            holder.Controls.Add(catChain);
            holder.Controls.Add(productCategoryLink);

            return holder;
        }


        /// <summary>
        /// Returns HTML code which is showing flash file with url
        /// </summary>
        public static PlaceHolder GetSwfHtmlCode(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new CommonCode.UIException("url is null or empty");
            }

            PlaceHolder swfHolder = new PlaceHolder();

            System.Web.UI.HtmlControls.HtmlGenericControl flash = new System.Web.UI.HtmlControls.HtmlGenericControl("object");
            flash.Attributes.Add("classid", "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000");
            swfHolder.Controls.Add(flash);

            System.Web.UI.HtmlControls.HtmlGenericControl param1 = new System.Web.UI.HtmlControls.HtmlGenericControl("param");
            param1.Attributes.Add("name", "movie");
            param1.Attributes.Add("value", url);
            flash.Controls.Add(param1);

            System.Web.UI.HtmlControls.HtmlGenericControl param2 = new System.Web.UI.HtmlControls.HtmlGenericControl("param");
            param2.Attributes.Add("name", "quality");
            param2.Attributes.Add("value", "high");
            flash.Controls.Add(param2);

            System.Web.UI.HtmlControls.HtmlGenericControl param3 = new System.Web.UI.HtmlControls.HtmlGenericControl("param");
            param3.Attributes.Add("name", "wmode");                 // needed in order other elements to be able to show
            param3.Attributes.Add("value", "opaque");               // above the swf
            flash.Controls.Add(param3);

            ////

            System.Web.UI.HtmlControls.HtmlGenericControl param4 = new System.Web.UI.HtmlControls.HtmlGenericControl("param");
            param4.Attributes.Add("name", "width");
            param4.Attributes.Add("value", "240");
            flash.Controls.Add(param4);

            System.Web.UI.HtmlControls.HtmlGenericControl param5 = new System.Web.UI.HtmlControls.HtmlGenericControl("param");
            param5.Attributes.Add("name", "height");
            param5.Attributes.Add("value", "240");
            flash.Controls.Add(param5);

            ////

            System.Web.UI.HtmlControls.HtmlGenericControl embed = new System.Web.UI.HtmlControls.HtmlGenericControl("embed");
            embed.Attributes.Add("src", url);
            embed.Attributes.Add("quality", "high");
            embed.Attributes.Add("pluginspage", "http://www.macromedia.com/go/getflashplayer");
            embed.Attributes.Add("type", "application/x-shockwave-flash");
            embed.Attributes.Add("wmode", "opaque");

            embed.Attributes.Add("width", "240");
            embed.Attributes.Add("height", "240");

            flash.Controls.Add(embed);

            return swfHolder;
        }

        public static void AddShareStuff(Label shareLbl, Page currPage, string title)
        {
            if (shareLbl == null)
            {
                throw new UIException("shareLbl is null");
            }
            if (currPage == null)
            {
                throw new UIException("currPage is null");
            }
            if (string.IsNullOrEmpty(title))
            {
                throw new UIException("title is empty");
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("<table cellpadding='0' cellspacing='0' style=\"display:inline-table;\"><tr><td class=\"searchPageComments\">");
            builder.Append(string.Format("{0}&nbsp;</td>", HttpContext.GetGlobalResourceObject("SiteResources", "Share")));
            builder.Append("<td style=\"padding-top:2px; vertical-align:top\"> ");
            builder.Append(string.Format("{0}{1}{2}", "<a name=\"fb_share\" type=\"button\" ></a>",
               "<script src=\"http://static.ak.fbcdn.net/connect.php/js/FB.Share\"",
               "type=\"text/javascript\"></script>"));

            builder.Append("</td><td>&nbsp;");
            builder.Append(string.Format("{0}{1}{2}", "<a href=\"http://twitter.com/share\" class=\"twitter-share-button\" "
                , " data-count=\"none\">Tweet</a><script type=\"text/javascript\" "
                , " src=\"http://platform.twitter.com/widgets.js\"></script> "));
            builder.Append("</td></tr></table>");

            shareLbl.Text = builder.ToString();

            HtmlMeta tag = new HtmlMeta();
            tag.Name = "title";
            tag.Content = title;
            currPage.Header.Controls.Add(tag);

            HtmlMeta tag2 = new HtmlMeta();
            tag2.Name = "description";
            tag2.Content = string.Format("In {0} you can share/read opinions for different products.", Configuration.SiteName);
            currPage.Header.Controls.Add(tag2);

            HtmlLink link = new HtmlLink();
            link.Href = "http://www.wiadvice.com/images/SiteImages/WiAdvice_s.png";
            link.Attributes["rel"] = "image_src";
            currPage.Header.Controls.Add(link);
        }

        public static void ShowUserNotificationPnl(Panel panel, Label label, string message)
        {
            if (panel == null)
            {
                throw new UIException("panel is null");
            }
            if (label == null)
            {
                throw new UIException("label is null");
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new CommonCode.UIException("message is null or empty");
            }

            if (panel.Visible == false)
            {
                panel.Visible = true;
            }

            if (label.Text.Length > 0)
            {
                label.Text += "<br />" + message;
            }
            else
            {
                label.Text = message;
            }

        }

        public static void HideUserNotificationPnl(Panel panel, Label label, Page page)
        {
            if (panel == null)
            {
                throw new UIException("panel is null");
            }
            if (label == null)
            {
                throw new UIException("label is null");
            }
            if (page == null)
            {
                throw new UIException("page is null");
            }

            object objMsg = page.Session["notifMsg"];
            if (objMsg == null)
            {
                panel.Visible = false;
                label.Text = string.Empty;
            }
            else
            {
                panel.Visible = true;
                label.Text = objMsg.ToString();
                page.Session["notifMsg"] = null;
            }
        }

        /// <summary>
        /// Changes the UI cultute of the current thread depending on the requested
        /// application variant in the request query string.
        /// </summary>
        public static void ChangeUiCulture()
        {
            string requestedCultureStr = HttpContext.Current.Request.Params[CommonCode.UiUrl.QueryStringApplicationVariantKey];
            if (requestedCultureStr == null)
            {
                requestedCultureStr = string.Empty;
            }
            if (Configuration.IsSupportedApplicationVariant(requestedCultureStr) == true)
            {
                ChangeUICulture(requestedCultureStr);
            }
            else if (log.IsWarnEnabled)
            {
                log.WarnFormat("Non-supported application variant (\"{0}\") was requested.", requestedCultureStr);
            }
        }


        /// <summary>
        /// Changes the UI cultute of the current thread depending on the requested
        /// application variant in the request query string.
        /// </summary>
        public static void ChangeUiCultureFromSession()
        {
            object requestedCultureObj = HttpContext.Current.Session[CommonCode.UiUrl.QueryStringApplicationVariantKey];
            if (requestedCultureObj != null)
            {
                ChangeUICulture(requestedCultureObj.ToString());
            }
        }

        /// <summary>
        /// Changes the UI culture of the current thread depending on the supplied culture identifier.
        /// </summary>
        /// <param name="requestedCultureStr">The requested culture identifier (e.g. "en", "bg", etc.)</param>
        private static void ChangeUICulture(string requestedCultureStr)
        {
            if (string.IsNullOrEmpty(requestedCultureStr) == false)
            {
                try
                {
                    CultureInfo requestedCulture = CultureInfo.CreateSpecificCulture(requestedCultureStr);
                    if (requestedCulture != null)
                    {
                        Thread.CurrentThread.CurrentUICulture = requestedCulture;
                    }
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled == true)
                    {
                        log.Error(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Get current objectContext, use only for web methods. Calls ChangeUiCultureFromSession() to get the current culture, and there is no need to call it further in the WebMethod
        /// </summary>
        /// <returns></returns>
        public static Entities CreateEntitiesForWebMethod()
        {
            ChangeUiCultureFromSession();
            string appVariant = Tools.ApplicationVariantString;
            string connStr = Configuration.GetEntitiesConnectionString(appVariant);
            Entities applicationVariantEntities = new Entities(connStr);
            return applicationVariantEntities;
        }

        /// <summary>
        /// Used for pages/handlers which receive URL with language parameter
        /// </summary>
        public static Entities CreateEntities()
        {
            string cultureStr = HttpContext.Current.Request.Params[CommonCode.UiUrl.QueryStringApplicationVariantKey];
            string connStr = Configuration.GetEntitiesConnectionString(cultureStr);
            Entities applicationVariantEntities = new Entities(connStr);
            return applicationVariantEntities;
        }

        /// <summary>
        /// Redirects to other page..to be used only when Ui Culture is already set
        /// </summary>
        public static void RedirectToOtherUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new UIException("url is empty");
            }
            if (BusinessLayer.Configuration.UseUrlRewriting == true)
            {
                string urlWithVariant;
                if (Configuration.UseExternalUrlRewriteModule == false)
                {
                    urlWithVariant = string.Format("{0}/{1}", Tools.ApplicationVariantString, url);
                }
                else
                {
                    urlWithVariant = url;
                }
                CommonCode.UiUrl.VerifyNoExtraApplicationVariantSpecifiersInUrl(urlWithVariant);
                HttpContext.Current.Response.Redirect(urlWithVariant);
            }
            else
            {
                HttpContext.Current.Response.Redirect(url);
            }
        }

        /// <summary>
        /// Assembles an URL to a page that is not the current one, using the supplied arguments.
        /// </summary>
        /// <param name="url">The URL to the other page, no application variant specified.</param>
        /// <param name="applicationVariant">The application varialt.</param>
        /// <returns>The requested URL.</returns>
        public static string UrlToOtherPage(string url, string applicationVariant)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("url" + " is null or empty.");
            }
            if (string.IsNullOrEmpty(applicationVariant))
            {
                throw new ArgumentException("applicationVariant" + " is null or empty.");
            }
            Configuration.VerifyApplicationVariantSupported(applicationVariant);
            string urlWithVariant;
            if (Configuration.UseUrlRewriting == true)
            {
                urlWithVariant = string.Format("{0}/{1}", applicationVariant, url);
            }
            else
            {
                string keyPrefix = (url.Contains(UiUrl.quest) == false) ? UiUrl.quest : UiUrl.amp;
                urlWithVariant = string.Format("{0}{1}{2}={3}", url, keyPrefix, UiUrl.QueryStringApplicationVariantKey, applicationVariant);
            }
            CommonCode.UiUrl.VerifyNoExtraApplicationVariantSpecifiersInUrl(urlWithVariant);
            return urlWithVariant;
        }

        public static void RedirectToSameUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new CommonCode.UIException("url is empty");
            }

            if (BusinessLayer.Configuration.UseUrlRewriting == true)
            {
                string unRewrittenUrl = CommonCode.UiUrl.UrlUnRewrite(url);
                CommonCode.UiUrl.VerifyNoExtraApplicationVariantSpecifiersInUrl(unRewrittenUrl);
                if (string.Equals(url, unRewrittenUrl) == false)
                {
                    if (log.IsDebugEnabled == true)
                    {
                        log.DebugFormat("URL {0} from \"{1}\" to \"{2}\" before redirect.",
                            "unrewritten", url, unRewrittenUrl);
                    }
                }
                HttpContext.Current.Response.Redirect(unRewrittenUrl);
            }
            else
            {
                HttpContext.Current.Response.Redirect(url);
            }
        }

        /// <summary>
        /// Returns URL with added variant infront, used for all hyperlinks in site. 
        /// If Configuration.UseExternalUrlRewriteModule = false, variants are not added in front. 
        /// </summary>
        public static string GetUrlWithVariant(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new CommonCode.UIException("url is null or empty");
            }

            if (BusinessLayer.Configuration.UseUrlRewriting == true)
            {
                if (Configuration.UseExternalUrlRewriteModule == true)
                {
                    string variant = Tools.ApplicationVariantString;

                    if (url.Contains(variant + "/") == false)
                    {
                        url = string.Format("{0}/{1}", variant, url);
                    }

                    url = CheckUrlForRepeatingvariants(url);
                }

            }

            return url;
        }

        public static string CheckUrlForRepeatingvariants(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new CommonCode.UIException("url is null or empty");
            }

            if (url.Contains("/") == true)
            {
                string variant = url.Substring(0, 3);

                string subUrl = url.Remove(0, 3);
                while (subUrl.Contains(variant) == true)
                {
                    subUrl = url.Remove(0, 3);
                }

                url = string.Format("{0}{1}", variant, subUrl);
            }
            else
            {
                url = string.Format("{0}/{1}", Tools.ApplicationVariantString, url);
            }

            return url;
        }

        /// <summary>
        /// Gets the client time zone offset (in minutes) from the Session.
        /// <para>This is the difference between GMT and the client local time.</para>
        /// <para>For eastern countries the offset is a negative number. For western countries the offset is a positive number.</para>
        /// </summary>
        /// <returns>The requested client time zone offset (in minutes). On error, 0 is returned.</returns>
        public static int GetClientTimeZoneOffsetInMinutesFromSession()
        {
            int clientOffsetInMinutes = 0;
            HttpSessionState session = HttpContext.Current.Session;
            object clientTimeZoneOffsetInMinutesObj = session[BasePage.ClientTimeZoneOffsetSessKey];
            if (clientTimeZoneOffsetInMinutesObj != null)
            {
                string offsetInMinutesStr = clientTimeZoneOffsetInMinutesObj.ToString();
                int.TryParse(offsetInMinutesStr, out clientOffsetInMinutes);
            }
            return clientOffsetInMinutes;
        }

        /// <summary>
        /// 4/17/2006
        /// </summary>
        public static string DateTimeToLocalShortDateString(DateTime time)
        {
            int clientOffsetInMinutes = GetClientTimeZoneOffsetInMinutesFromSession();
            DateTime clientTime = time.AddMinutes(-clientOffsetInMinutes);
            string result = clientTime.ToString("d", Thread.CurrentThread.CurrentUICulture);
            return result;
        }

        /// <summary>
        /// Monday, April 17, 2006
        /// </summary>
        public static string DateTimeToLocalLongDateString(DateTime time)
        {
            int clientOffsetInMinutes = GetClientTimeZoneOffsetInMinutesFromSession();
            DateTime clientTime = time.AddMinutes(-clientOffsetInMinutes);
            string result = clientTime.ToString("D", Thread.CurrentThread.CurrentUICulture);
            return result;
        }

        /// <summary>
        /// 4/17/2006 2:22 PM
        /// </summary>
        public static string DateTimeToLocalString(DateTime time)
        {
            int clientOffsetInMinutes = GetClientTimeZoneOffsetInMinutesFromSession();
            DateTime clientTime = time.AddMinutes(-clientOffsetInMinutes);
            string result = clientTime.ToString("g", Thread.CurrentThread.CurrentUICulture);
            return result;
        }

        /// <summary>
        /// 4/17/2006
        /// </summary>
        public static string DateTimeToShortDateString(DateTime time)
        {
            string result = time.ToString("d", Thread.CurrentThread.CurrentUICulture);
            return result;
        }

        public static string DateTimeToLocalString(string format, DateTime time)
        {
            if (string.IsNullOrEmpty(format))
            {
                throw new UIException("DateTime format is empty");
            }

            int clientOffsetInMinutes = GetClientTimeZoneOffsetInMinutesFromSession();
            DateTime clientTime = time.AddMinutes(-clientOffsetInMinutes);
            string result = clientTime.ToString(format, Thread.CurrentThread.CurrentUICulture);
            return result;
        }

        public static DateTime DateTimeToUserLocalTime(DateTime time)
        {
            int clientOffsetInMinutes = GetClientTimeZoneOffsetInMinutesFromSession();

            DateTime clientTime = time.AddMinutes(-clientOffsetInMinutes);

            return clientTime;
        }

        /// <summary>
        /// Hacks navigation menu item swith style
        /// </summary>
        /// <param name="firstItem">true if its First item, because other style is used</param>
        public static string HackNavigationMenu(string text, bool selectable, bool firstItem)
        {
            string result = string.Empty;

            if (firstItem == true)
            {
                if (selectable == true)
                {
                    result = text;
                }
                else
                {
                    result = string.Format("<span class=\"defCursor\">{0}</span>", text);
                }
            }
            else
            {
                if (selectable == true)
                {
                    result = string.Format("<div class=\"cmenus\">{0}</div>", text);
                }
                else
                {
                    result = string.Format("<div class=\"cmenusNC\">{0}</div>", text);
                }
            }

            return result;
        }

        /// <summary>
        /// Shuffles the elements in list using random generator
        /// </summary>
        public static List<T> ShuffleList<T>(List<T> list)
        {
            List<T> shuffledList = new List<T>();

            if (list != null && list.Count > 0)
            {
                Random generator = new Random();
                int num = 0;

                while (list.Count > 0)
                {
                    num = generator.Next(0, list.Count);
                    shuffledList.Add(list[num]);
                    list.RemoveAt(num);
                }
            }

            return shuffledList;
        }

        /// <summary>
        /// Returns FULL url to page. Example : /en/GetImageHandler.ashx
        /// </summary>
        public static string UrlWithApplicationPath(string page)
        {
            if (string.IsNullOrEmpty(page))
            {
                throw new UIException("page is empty");
            }

            string applicationPathToUse = HttpContext.Current.Request.ApplicationPath != "/" ? HttpContext.Current.Request.ApplicationPath : string.Empty;

            string appVariant = Tools.ApplicationVariantString;

            string fullUrl = string.Format("{0}/{1}/{2}", applicationPathToUse, appVariant, page);

            return fullUrl;
        }

        public static string UrlWithApplicationPath(string page, string appVariant)
        {
            if (string.IsNullOrEmpty(page))
            {
                throw new UIException("page is empty");
            }

            if (string.IsNullOrEmpty(appVariant))
            {
                throw new UIException("appVariant is empty");
            }

            string applicationPathToUse = HttpContext.Current.Request.ApplicationPath != "/" ? HttpContext.Current.Request.ApplicationPath : string.Empty;

            string fullUrl = string.Format("{0}/{1}/{2}", applicationPathToUse, appVariant, page);

            return fullUrl;
        }


    }
}
