﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Text;

using DataAccess;
using BusinessLayer;

namespace UserInterface.CommonCode
{
    public class WebMethods
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(UiTools));

        private static object lockSpam = new object();
        private static object lockRating = new object();
        private static object lockSendMessage = new object();
        private static object lockVisits = new object();

        /// <summary>
        /// Returns HTML code which will be shown in PopUp panel
        /// </summary>
        /// <param name="type">product,company</param>
        /// <param name="strId">type ID</param>
        public static string GetTypeData(string type, string strId)
        {
            try
            {
                string result = string.Empty;

                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(strId))
                {
                    return result;
                }

                long typeId = -1;
                if (!long.TryParse(strId, out typeId))
                {
                    return result;
                }

                Entities ObjectContext = UiTools.CreateEntitiesForWebMethod();

                switch (type)
                {
                    case ("product"):
                        BusinessProduct businessProduct = new BusinessProduct();
                        BusinessRating businessRating = new BusinessRating();
                        Product currProduct = businessProduct.GetProductByIDWV(ObjectContext, typeId);
                        if (currProduct == null)
                        {
                            return result;
                        }
                        result = GetProductBriefData(ObjectContext, currProduct, businessProduct, businessRating);
                        break;
                    case ("company"):
                        BusinessCompany businessCompany = new BusinessCompany();
                        Company currCompany = businessCompany.GetCompanyWV(ObjectContext, typeId);
                        if (currCompany == null)
                        {
                            return result;
                        }
                        result = GetCompanyBriefData(ObjectContext, currCompany, businessCompany);
                        break;
                    // Add new Cases for other types
                    default:
                        return result;
                }

                return result;
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string GetTypeSuggestion(string strId)
        {
            try
            {

                long id = 0;
                if (!long.TryParse(strId, out id))
                {
                    return string.Empty;
                }
                Entities ObjectContext = UiTools.CreateEntitiesForWebMethod();

                BusinessTypeSuggestions btSuggestion = new BusinessTypeSuggestions();
                TypeSuggestion currSuggestion = btSuggestion.GetSuggestion(ObjectContext, id, false, false);
                if (currSuggestion == null)
                {
                    return string.Empty;
                }

                EntitiesUsers userContext = new EntitiesUsers();
                BusinessUser bUser = new BusinessUser();
                BusinessReport bReport = new BusinessReport();

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, ObjectContext, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }

                User fromUser = null;
                User toUser = null;

                if (!currSuggestion.ToUserReference.IsLoaded)
                {
                    currSuggestion.ToUserReference.Load();
                }
                if (!currSuggestion.ByUserReference.IsLoaded)
                {
                    currSuggestion.ByUserReference.Load();
                }

                bool isAdmin = false;

                if (bUser.IsUser(currentUser))
                {
                    if (currSuggestion.ByUser.ID != currentUser.ID && currSuggestion.ToUser.ID != currentUser.ID)
                    {
                        return string.Empty;
                    }
                }
                else if (bUser.IsFromAdminTeam(currentUser))
                {
                    isAdmin = true;
                }
                else
                {
                    return string.Empty;
                }

                fromUser = bUser.GetWithoutVisible(userContext, currSuggestion.ByUser.ID, false);
                toUser = bUser.GetWithoutVisible(userContext, currSuggestion.ToUser.ID, false);

                if (fromUser == null || toUser == null)
                {
                    return string.Empty;
                }

                string type = string.Empty;
                string link = string.Empty;

                if (currSuggestion.type == "product")
                {
                    BusinessProduct businessProduct = new BusinessProduct();
                    Product currProduct = businessProduct.GetProductByIDWV(ObjectContext, currSuggestion.typeID);
                    if (currProduct == null)
                    {
                        return string.Empty;
                    }

                    type = HttpContext.GetGlobalResourceObject("WebMethods", "product").ToString();
                    link = string.Format("<a href=\"{0}{1}\">{2}</a>", CommonCode.UiTools.GetUrlWithVariant("Product.aspx?Product=")
                        , currProduct.ID, currProduct.name);
                }
                else if (currSuggestion.type == "company")
                {
                    BusinessCompany businessCompany = new BusinessCompany();
                    Company currCompany = businessCompany.GetCompanyWV(ObjectContext, currSuggestion.typeID);
                    if (currCompany == null)
                    {
                        return string.Empty;
                    }

                    type = HttpContext.GetGlobalResourceObject("WebMethods", "maker").ToString();
                    link = string.Format("<a href=\"{0}{1}\">{2}</a>", CommonCode.UiTools.GetUrlWithVariant("Company.aspx?Company=")
                        , currCompany.ID, currCompany.name);

                }
                else
                {
                    return string.Empty;
                }

                // user is ok

                string result = string.Empty;

                StringBuilder description = new StringBuilder();

                string strDescription = Tools.BreakLongWordsInString(currSuggestion.description, 50);

                description.Append("<div class=\"pnlPopUpTypeSuggestion roundedCorners5\">");

                if (isAdmin == true)
                {
                    description.Append(string.Format("ID:{0} {1} {2} {3}", currSuggestion.ID
                   , HttpContext.GetGlobalResourceObject("WebMethods", "for1"), type, link));
                }
                else
                {
                    description.Append(string.Format("{0} {1} {2}"
                    , HttpContext.GetGlobalResourceObject("WebMethods", "for1"), type, link));
                }

                description.Append(string.Format("<div class=\"floatRightNoMrg\"> {0} <a href=\"{1}{2}\">{3}</a> "
                    , HttpContext.GetGlobalResourceObject("WebMethods", "by")
                    , CommonCode.UiTools.GetUrlWithVariant("Profile.aspx?User=")
                    , fromUser.ID, fromUser.username));

                description.Append(string.Format("<span style=\"padding-left:10px;\" class=\"commentsDate\">{0}  </span>"
                    , CommonCode.UiTools.DateTimeToLocalShortDateString(currSuggestion.dateCreated)));
                if (currSuggestion.active == false && currSuggestion.status != null)
                {
                    description.Append(string.Format("<span style=\"padding-left:10px;\" class=\"searchPageRatings\">{0}</span>"
                        , BusinessTypeSuggestions.GetSuggestionStatus(currSuggestion)));
                }
                description.Append("</div>");
                description.Append(string.Format("<div>{0}</div>", strDescription));

                List<TypeSuggestionComment> comments = btSuggestion.GetSuggestionComments(ObjectContext, currSuggestion);
                if (comments.Count > 0)
                {
                    foreach (TypeSuggestionComment comment in comments)
                    {
                        if (!comment.UserReference.IsLoaded)
                        {
                            comment.UserReference.Load();
                        }

                        description.Append("<div class='panelRows yellowCellBgr marginRightComm'>");
                        description.Append(string.Format("{0} ", HttpContext.GetGlobalResourceObject("WebMethods", "From")));
                        if (comment.User.ID == fromUser.ID)
                        {
                            description.Append(string.Format("<a href=\"{0}{1}\">{2}</a>"
                                , CommonCode.UiTools.GetUrlWithVariant("Profile.aspx?User=")
                                , fromUser.ID, fromUser.username));
                        }
                        else
                        {
                            description.Append(string.Format("<a href=\"{0}{1}\">{2}</a>"
                                , CommonCode.UiTools.GetUrlWithVariant("Profile.aspx?User=")
                                , toUser.ID, toUser.username));
                        }

                        strDescription = Tools.BreakLongWordsInString(comment.description, 50);

                        description.Append(string.Format("<div class=\"floatRight commentsDate\">{0}</div>", UiTools.DateTimeToLocalString(comment.dateCreated)));
                        description.Append(string.Format("<div>{0}</div>", strDescription));
                        description.Append("</div>");
                    }
                }

                description.Append("</div>");

#if DEBUG
                if (!Validate.ValidateHtml(string.Format("<span>{0}</span>", description)))
                {
                    throw new CommonCode.UIException("Invalid HTML code generated from function.");
                }
#endif

                result = description.ToString();

                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        /// <summary>
        /// Returns HTML code for Company
        /// </summary>
        private static string GetCompanyBriefData(Entities ObjectContext, Company currCompany, BusinessCompany businessCompany)
        {
            string result = string.Empty;

            if (ObjectContext != null && currCompany != null && !businessCompany.IsOther(ObjectContext, currCompany))
            {
                try
                {
                    CommonCode.UiTools.ChangeUiCultureFromSession();

                    ImageTools imageTools = new ImageTools();

                    System.Text.StringBuilder description = new System.Text.StringBuilder();

                    string companyDescription = string.Empty;

                    if (string.IsNullOrEmpty(currCompany.description))
                    {
                        companyDescription = HttpContext.GetGlobalResourceObject("WebMethods", "DescrNotEntered").ToString();
                    }
                    else
                    {
                        companyDescription = Tools.TrimString(currCompany.description, 150, false, true);
                        companyDescription = Tools.BreakLongWordsInString(companyDescription, 25);
                    }

                    CompanyImage image = imageTools.GetCompanyImageThumbnail(ObjectContext, currCompany);
                    if (image != null)
                    {
                        description.Append(string.Format("<img src='{0}' align='Left' class='popUpImagesStyle' ></img>"
                            , image.url));
                    }

                    description.Append(string.Format("<div style='text-align:center' class='popUpHeaders'>{0}</div>", currCompany.name));
                    description.Append(string.Format("<span class='popUpDescription'>{0} </span>"
                        , HttpContext.GetGlobalResourceObject("WebMethods", "Description")));
                    description.Append(string.Format("<span class='descriptionLittle'>{0}</span><br />"
                         , companyDescription));

#if DEBUG
                    if (!Validate.ValidateHtml(string.Format("<span>{0}</span>", description)))
                    {
                        throw new CommonCode.UIException("Invalid HTML code generated from function.");
                    }
#endif
                    result = description.ToString();
                }
                catch (System.Xml.XmlException ex)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error(ex);
                    }
                    result = string.Empty;
                }
            }
            return result;
        }


        /// <summary>
        /// Returns HTML code for Product
        /// </summary>
        private static string GetProductBriefData(Entities ObjectContext, Product currProduct, BusinessProduct businessProduct, BusinessRating businessRating)
        {
            string result = string.Empty;

            if (ObjectContext != null && currProduct != null)
            {
                try
                {
                    CommonCode.UiTools.ChangeUiCultureFromSession();
                    ImageTools imageTools = new ImageTools();

                    System.Text.StringBuilder description = new System.Text.StringBuilder();

                    string productDescription = string.Empty;

                    if (string.IsNullOrEmpty(currProduct.description))
                    {
                        productDescription = HttpContext.GetGlobalResourceObject("WebMethods", "DescrNotEntered").ToString();
                    }
                    else
                    {
                        productDescription = Tools.TrimString(currProduct.description, 150, false, true);
                        productDescription = Tools.BreakLongWordsInString(productDescription, 25);
                    }


                    ProductImage image = imageTools.GetProductImageThumbnail(ObjectContext, currProduct);
                    if (image != null)
                    {
                        description.Append(string.Format("<img src='{0}' align='Left' class='popUpImagesStyle' ></img>", image.url));
                    }

                    description.Append(string.Format("<div style='text-align:center' class='popUpHeaders'>{0}</div>", currProduct.name));

                    description.Append(string.Format("<span class='searchPageRatings'>{0} {1}</span><br />"
                        , HttpContext.GetGlobalResourceObject("WebMethods", "Rating"), businessRating.GetProductRating(currProduct)));

                    description.Append(string.Format("<span class='searchPageComments'>{0} {1}</span><br />"
                        , HttpContext.GetGlobalResourceObject("WebMethods", "Comments"), currProduct.comments));

                    description.Append(string.Format("<span class='popUpDescription'>{0} </span>"
                        , HttpContext.GetGlobalResourceObject("WebMethods", "Description")));

                    description.Append(string.Format("<span class='descriptionLittle'>{0}</span><br />"
                        , productDescription));

#if DEBUG
                    if (!Validate.ValidateHtml(string.Format("<span>{0}</span>", description)))
                    {
                        throw new CommonCode.UIException("Invalid HTML code generated from function.");
                    }
#endif

                    result = description.ToString();
                }
                catch (System.Xml.XmlException ex)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error(ex);
                    }
                    result = string.Empty;
                }
            }
            return result;
        }

        public static string GetSiteText(string strId, string type)
        {
            try
            {

                string result = string.Empty;

                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(strId))
                {
                    return string.Empty;
                }

                long typeID = 0;
                if (!long.TryParse(strId, out typeID))
                {
                    return string.Empty;
                }

                if (typeID < 1)
                {
                    return string.Empty;
                }

                EntitiesUsers userContext = new EntitiesUsers();
                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                BusinessUser businessUser = new BusinessUser();
                BusinessSiteText bSiteText = new BusinessSiteText();

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);

                if (currentUser == null || !businessUser.IsFromAdminTeam(currentUser))
                {
                    return string.Empty;
                }

                SiteNews siteText = bSiteText.Get(objectContext, typeID);
                if (siteText == null)
                {
                    return string.Empty;
                }

                switch (type)
                {
                    case "warning pattern":
                        if (siteText.type != "warningPattern")
                        {
                            return string.Empty;
                        }

                        result = Tools.DecodeHtmlSpacesAndNewLines(siteText.description);

                        break;
                    case "report pattern":
                        if (siteText.type != "reportPattern")
                        {
                            return string.Empty;
                        }

                        result = Tools.DecodeHtmlSpacesAndNewLines(siteText.description);

                        break;
                    default:
                        return string.Empty;
                }

                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }
        }


        public static string GetSimilarNames(string type, string name, string popUpID)
        {
            try
            {

                name = Tools.RemoveSpacesAtEndOfString(name);

                if (string.IsNullOrEmpty(type))
                {
                    return string.Empty;
                }
                if (name.Length < 2)
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(popUpID))
                {
                    return string.Empty;
                }
                if (name.StartsWith(" ") || name.EndsWith(" "))
                {
                    return string.Empty;
                }
                if (type != "product" && type != "company")
                {
                    return string.Empty;
                }

                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                AutoComplete bAutoComplete = new AutoComplete();

                StringBuilder htmlcode = new StringBuilder();

                switch (type)
                {
                    case "product":
                        List<Product> products = new List<Product>();
                        List<AlternativeProductName> altNames = new List<AlternativeProductName>();
                        List<ProductVariant> variants = new List<ProductVariant>();
                        List<ProductSubVariant> subvariants = new List<ProductSubVariant>();

                        bAutoComplete.GetProductResults(objectContext, name, 10, out products, out altNames, out variants, out subvariants);

                        if (products.Count > 0 || altNames.Count > 0 || variants.Count > 0 || subvariants.Count > 0)
                        {
                            htmlcode.Append("<div class=\"similarNamesPnl\">");
                            htmlcode.Append(string.Format("<div style=\"text-align:center;\"> {0} </div>"
                                , HttpContext.GetGlobalResourceObject("WebMethods", "ProductsWithSimNames")));

                            if (products.Count > 0)
                            {
                                foreach (Product product in products)
                                {
                                    htmlcode.Append("<img class=\"itemImage\" src=\"images/SiteImages/triangle.png\" style=\"border-width:0px;\" />");

                                    htmlcode.Append(string.Format("<a id=\"snprod{0}\"", product.ID));
                                    htmlcode.Append(string.Format("onmouseover=\"ShowData('product','{0}','snprod{0}','{1}')\"", product.ID, popUpID));
                                    htmlcode.Append("onmouseout=\"HideData()\"");
                                    htmlcode.Append(string.Format("href=\"{0}{1}\">{2}</a>"
                                        , "Product.aspx?Product="
                                        , product.ID, product.name));
                                    htmlcode.Append("<br />");
                                }
                            }

                            if (altNames.Count > 0)
                            {
                                foreach (AlternativeProductName altName in altNames)
                                {
                                    if (!altName.ProductReference.IsLoaded)
                                    {
                                        altName.ProductReference.Load();
                                    }

                                    htmlcode.Append("<img class=\"itemImage\" src=\"images/SiteImages/triangle.png\" style=\"border-width:0px;\" />");

                                    htmlcode.Append(string.Format("<a id=\"snaprod{0}\"", altName.Product.ID));
                                    htmlcode.Append(string.Format("onmouseover=\"ShowData('product','{0}','snaprod{0}','{1}')\"", altName.Product.ID, popUpID));
                                    htmlcode.Append("onmouseout=\"HideData()\"");
                                    htmlcode.Append(string.Format("href=\"{0}{1}\">{2} : {3}</a>"
                                        , "Product.aspx?Product="
                                        , altName.Product.ID, altName.Product.name, altName.name));
                                    htmlcode.Append("<br />");
                                }
                            }

                            if (variants.Count > 0)
                            {
                                foreach (ProductVariant variant in variants)
                                {
                                    if (!variant.ProductReference.IsLoaded)
                                    {
                                        variant.ProductReference.Load();
                                    }

                                    htmlcode.Append("<img class=\"itemImage\" src=\"images/SiteImages/triangle.png\" style=\"border-width:0px;\" />");

                                    htmlcode.Append(string.Format("<a id=\"snvprod{0}\"", variant.Product.ID));
                                    htmlcode.Append(string.Format("onmouseover=\"ShowData('product','{0}','snvprod{0}','{1}')\"", variant.Product.ID, popUpID));
                                    htmlcode.Append("onmouseout=\"HideData()\"");
                                    htmlcode.Append(string.Format("href=\"{0}{1}\">{2} : {3}</a>"
                                        , "Product.aspx?Product="
                                        , variant.Product.ID, variant.Product.name, variant.name));
                                    htmlcode.Append("<br />");
                                }
                            }

                            if (subvariants.Count > 0)
                            {
                                foreach (ProductSubVariant subvariant in subvariants)
                                {
                                    if (!subvariant.ProductReference.IsLoaded)
                                    {
                                        subvariant.ProductReference.Load();
                                    }

                                    if (!subvariant.VariantReference.IsLoaded)
                                    {
                                        subvariant.VariantReference.Load();
                                    }

                                    htmlcode.Append("<img class=\"itemImage\" src=\"images/SiteImages/triangle.png\" style=\"border-width:0px;\" />");

                                    htmlcode.Append(string.Format("<a id=\"snsvprod{0}\"", subvariant.Product.ID));
                                    htmlcode.Append(string.Format("onmouseover=\"ShowData('product','{0}','snsvprod{0}','{1}')\"", subvariant.Product.ID, popUpID));
                                    htmlcode.Append("onmouseout=\"HideData()\"");
                                    htmlcode.Append(string.Format("href=\"{0}{1}\">{2} : {3} : {4}</a>"
                                        , "Product.aspx?Product="
                                        , subvariant.Product.ID, subvariant.Product.name, subvariant.Variant.name, subvariant.name));
                                    htmlcode.Append("<br />");
                                }
                            }

                            htmlcode.Append("</div>");
                        }
                        break;
                    case "company":
                        List<AlternativeCompanyName> altcNames = new List<AlternativeCompanyName>();
                        List<Company> companies = new List<Company>();
                        bAutoComplete.GetCompanyResults(objectContext, name, 10, out companies, out altcNames);

                        if (companies.Count > 0 || altcNames.Count > 0)
                        {
                            htmlcode.Append("<div class=\"similarNamesPnl\">");
                            htmlcode.Append(string.Format("<div style=\"text-align:center;\"> {0} </div>"
                                , HttpContext.GetGlobalResourceObject("WebMethods", "MakersWithSimNames")));

                            if (companies.Count > 0)
                            {
                                foreach (Company company in companies)
                                {
                                    htmlcode.Append("<img class=\"itemImage\" src=\"images/SiteImages/triangle.png\" style=\"border-width:0px;\" />");

                                    htmlcode.Append(string.Format("<a id=\"sncomp{0}\"", company.ID));
                                    htmlcode.Append(string.Format("onmouseover=\"ShowData('company','{0}','sncomp{0}','{1}')\"", company.ID, popUpID));
                                    htmlcode.Append("onmouseout=\"HideData()\"");
                                    htmlcode.Append(string.Format("href=\"{0}{1}\">{2}</a>"
                                        , "Company.aspx?Company="
                                        , company.ID, company.name));
                                    htmlcode.Append("<br />");
                                }
                            }

                            if (altcNames.Count > 0)
                            {
                                foreach (AlternativeCompanyName altCn in altcNames)
                                {
                                    if (!altCn.CompanyReference.IsLoaded)
                                    {
                                        altCn.CompanyReference.Load();
                                    }

                                    htmlcode.Append("<img class=\"itemImage\" src=\"images/SiteImages/triangle.png\" style=\"border-width:0px;\" />");

                                    htmlcode.Append(string.Format("<a id=\"snacomp{0}\"", altCn.Company.ID));
                                    htmlcode.Append(string.Format("onmouseover=\"ShowData('company','{0}','snacomp{0}','{1}')\"", altCn.Company.ID, popUpID));
                                    htmlcode.Append("onmouseout=\"HideData()\"");
                                    htmlcode.Append(string.Format("href=\"{0}{1}\">{2} : {3}</a>"
                                        , "Company.aspx?Company="
                                        , altCn.Company.ID, altCn.Company.name, altCn.name));
                                    htmlcode.Append("<br />");
                                }
                            }


                            htmlcode.Append("</div>");
                        }
                        break;
                    default:
                        return string.Empty;
                }

                return htmlcode.ToString();

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }


        public static string SendReport(string type, string strTypeId, string description)
        {
            try
            {
                string result = string.Empty;
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(strTypeId))
                {
                    return string.Empty;
                }

                EntitiesUsers userContext = new EntitiesUsers();
                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                BusinessUser businessUser = new BusinessUser();
                BusinessReport businessReport = new BusinessReport();

                if (string.IsNullOrEmpty(description))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errTypeReport").ToString();
                }

                description = Tools.BreakPossibleHtmlCode(description);

                long typeID = 0;
                if (!long.TryParse(strTypeId, out typeID))
                {
                    return string.Empty;
                }

                if (typeID < 1)
                {
                    return string.Empty;
                }


                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);

                // ALL CHECKS ARE HERE !!!
                string error = string.Empty;
                if (businessReport.CheckIfUserCanSendReportAboutType(objectContext, userContext, currentUser, "irregularity", type, typeID, out error) == false)
                {
                    return error;
                }

                if (!Validate.ValidateComment(ref description, out result))
                {
                    return result;
                }

                BusinessLog bLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                switch (type)
                {
                    case "category":

                        BusinessCategory businessCategory = new BusinessCategory();

                        Category currCategory = businessCategory.Get(objectContext, typeID);
                        if (currCategory == null)
                        {
                            return string.Empty;
                        }

                        if (currCategory.last == false || currCategory.visible == false)
                        {
                            return string.Empty;
                        }

                        businessReport.CreateCategoryIrregularityReport(userContext, objectContext, currentUser, bLog, currCategory, description);

                        break;
                    case "product":

                        BusinessProduct businessProduct = new BusinessProduct();

                        Product currProduct = businessProduct.GetProductByID(objectContext, typeID);
                        if (currProduct == null)
                        {
                            return string.Empty;
                        }

                        businessReport.CreateProductIrregularityReport(userContext, objectContext, currentUser, bLog, currProduct, description);

                        break;
                    case "user":

                        User userToReport = businessUser.Get(userContext, typeID, false);
                        if (userToReport == null)
                        {
                            return string.Empty;
                        }


                        businessReport.CreateReportAboutUser(userContext, objectContext, currentUser, userToReport, bLog, description);

                        break;
                    case "company":

                        BusinessCompany businessCompany = new BusinessCompany();

                        Company currCompany = businessCompany.GetCompany(objectContext, typeID);
                        if (currCompany == null)
                        {
                            return string.Empty;
                        }
                        if (currCompany.visible == false)
                        {
                            return string.Empty;
                        }

                        businessReport.CreateCompanyIrregularityReport(userContext, objectContext, currentUser, bLog, currCompany, description);

                        break;

                    case "typeSuggestion":

                        BusinessTypeSuggestions btSuggestion = new BusinessTypeSuggestions();
                        TypeSuggestion currSuggestion = btSuggestion.GetSuggestion(objectContext, typeID, false, false);
                        if (currSuggestion == null)
                        {
                            return string.Empty;
                        }

                        businessReport.CreateTypeSuggestionIrregularityReport(userContext, objectContext, currentUser, bLog, currSuggestion, description);

                        break;

                    case "productTopic":

                        BusinessProductTopics bpTopic = new BusinessProductTopics();
                        ProductTopic topic = bpTopic.Get(objectContext, typeID, true, false);
                        if (topic == null || topic.visible == false)
                        {
                            return string.Empty;
                        }

                        businessReport.CreateProductTopicIrregularityReport(userContext, objectContext, currentUser, bLog, topic, description);

                        break;
                    default:
                        return string.Empty;
                }

                int remReports = businessReport.NumberOfReportsWhichUserCanSend(objectContext, currentUser);

                if (remReports > 0)
                {
                    result = string.Format("{0} {1}", HttpContext.GetGlobalResourceObject("WebMethods", "ReportSent"), remReports);
                }
                else
                {
                    result = string.Format("{0} {1}{2}", HttpContext.GetGlobalResourceObject("WebMethods", "ReportSent"), remReports
                        , "<span id=\"hideBtn\" style=\"visibility:hidden;\"></span>");
                }

                return result;
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string SignForNotifies(string type, string strTypeID)
        {
            try
            {

                string result = "";

                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(strTypeID))
                {
                    return string.Empty;
                }

                long id = 0;
                if (!long.TryParse(strTypeID, out id))
                {
                    return string.Empty;
                }
                else if (id < 1)
                {
                    return string.Empty;
                }


                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();

                BusinessUser businessUser = new BusinessUser();

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }

                BusinessLog bLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                BusinessNotifies businessNotifies = new BusinessNotifies();
                if (businessNotifies.IsMaxNotificationsNumberReached(objectContext, currentUser))
                {
                    return string.Format("{0} {1} {2}.", HttpContext.GetGlobalResourceObject("WebMethods", "errNotifLimit")
                        , Configuration.NotifiesMaxNumberNotifies, HttpContext.GetGlobalResourceObject("WebMethods", "errNotifLimit2"));
                }

                NotifyOnNewContent notify = null;

                switch (type)
                {
                    case "product":

                        BusinessProduct businessProduct = new BusinessProduct();
                        Product currProduct = businessProduct.GetProductByID(objectContext, id);
                        if (currProduct == null)
                        {
                            return string.Empty;
                        }
                        if (!businessProduct.CheckIfProductsIsValidWithConnections(objectContext, currProduct, out result))
                        {
                            return string.Empty;
                        }

                        notify = businessNotifies.Get(objectContext, currentUser, NotifyType.Product, id, true);
                        if (notify != null)
                        {
                            return string.Empty;
                        }

                        businessNotifies.AddNewNotify(objectContext, bLog, currentUser, id, NotifyType.Product);

                        result = HttpContext.GetGlobalResourceObject("WebMethods", "SignNotifOnProduct").ToString();

                        break;
                    case "company":

                        BusinessCompany businessCompany = new BusinessCompany();
                        Company currCompany = businessCompany.GetCompany(objectContext, id);
                        if (currCompany == null)
                        {
                            return string.Empty;
                        }

                        notify = businessNotifies.Get(objectContext, currentUser, NotifyType.Company, id, true);
                        if (notify != null)
                        {
                            return string.Empty;
                        }

                        businessNotifies.AddNewNotify(objectContext, bLog, currentUser, id, NotifyType.Company);

                        result = HttpContext.GetGlobalResourceObject("WebMethods", "SignNotifOnMaker").ToString();

                        break;

                    case "productForum":

                        BusinessProduct bProduct = new BusinessProduct();
                        Product cProduct = bProduct.GetProductByID(objectContext, id);
                        if (cProduct == null)
                        {
                            return string.Empty;
                        }
                        if (!bProduct.CheckIfProductsIsValidWithConnections(objectContext, cProduct, out result))
                        {
                            return string.Empty;
                        }

                        notify = businessNotifies.Get(objectContext, currentUser, NotifyType.ProductForum, id, true);
                        if (notify != null)
                        {
                            return string.Empty;
                        }

                        businessNotifies.AddNewNotify(objectContext, bLog, currentUser, id, NotifyType.ProductForum);

                        result = HttpContext.GetGlobalResourceObject("WebMethods", "SignNotifOnProductForum").ToString();

                        break;

                    case "productTopic":

                        BusinessProductTopics bTopic = new BusinessProductTopics();
                        ProductTopic topic = bTopic.Get(objectContext, id, true, false);
                        if (topic == null || topic.locked == true)
                        {
                            return string.Empty;
                        }

                        notify = businessNotifies.Get(objectContext, currentUser, NotifyType.ProductTopic, id, true);
                        if (notify != null)
                        {
                            return string.Empty;
                        }

                        businessNotifies.AddNewNotify(objectContext, bLog, currentUser, id, NotifyType.ProductTopic);

                        result = HttpContext.GetGlobalResourceObject("WebMethods", "SignNotifOnProductTopic").ToString();

                        break;

                    default:
                        return string.Empty;
                }

                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string TransferAction(string strId, string name, string description)
        {
            try
            {

                if (string.IsNullOrEmpty(strId))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(name))
                {
                    return string.Empty;
                }
                string error = string.Empty;
                description = Tools.BreakPossibleHtmlCode(description);
                if (!Validate.ValidateDescription(0, Configuration.FieldsMaxDescriptionFieldLength, ref description, "description", out error, 90))
                {
                    return error;
                }

                long id = 0;
                if (!long.TryParse(strId, out id))
                {
                    return string.Empty;
                }

                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();
                BusinessUser businessUser = new BusinessUser();
                BusinessUserTypeActions buTypeActions = new BusinessUserTypeActions();
                BusinessTransferAction bTtansferAction = new BusinessTransferAction();


                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }
                BusinessLog bLog = new BusinessLog(CurrentUser.GetCurrentUserId(), HttpContext.Current.Request.UserHostAddress);

                User receiver = businessUser.GetByName(userContext, name, false, true);
                if (receiver == null)
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errNoUser").ToString();
                }

                UsersTypeAction action = buTypeActions.GetUserTypeAction(objectContext, currentUser.ID, id, false);
                if (action == null)
                {
                    return string.Empty;
                }

                string result = string.Empty;

                if (!bTtansferAction.CanUserTransferActionTo(objectContext, userContext, currentUser, receiver, action, out error))
                {
                    return error;
                }

                bTtansferAction.Add(objectContext, userContext, currentUser, receiver, action, description, bLog);

                result = string.Format("{0} <span id=\"reload\" style=\"visibility:hidden;\"></span>"
                    , HttpContext.GetGlobalResourceObject("WebMethods", "TransferCreated"));

                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string GetCommentDescription(string commID)
        {
            try
            {

                if (string.IsNullOrEmpty(commID))
                {
                    return string.Empty;
                }

                long comID = 0;
                if (long.TryParse(commID, out comID))
                {
                    if (comID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();
                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();

                Comment currComment = businessComment.Get(objectContxt, comID);
                if (currComment == null)
                {
                    return string.Empty;
                }

                if (!currComment.UserIDReference.IsLoaded)
                {
                    currComment.UserIDReference.Load();
                }

                long userID = CommonCode.CurrentUser.GetCurrentUserId();
                User currentUser = businessUser.Get(userContext, userID, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }
                else if (currentUser.ID != currComment.UserID.ID && !businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
                {
                    return string.Empty;
                }


                Page somPage = new Page();

                string description = somPage.Server.HtmlEncode(currComment.description);

                return description;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string EditComment(string commID, string username, string message)
        {
            try
            {

                if (string.IsNullOrEmpty(commID) || string.IsNullOrEmpty(username))
                {
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(message))
                {
                    return "Edit comment !";
                }

                message = Tools.BreakPossibleHtmlCode(message);

                long comID = 0;
                if (long.TryParse(commID, out comID))
                {
                    if (comID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();
                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();

                long userID = CommonCode.CurrentUser.GetCurrentUserId();
                User currentUser = businessUser.Get(userContext, userID, false);
                if (currentUser == null || !businessUser.IsFromAdminTeam(currentUser))
                {
                    return "Only administrators can edit comments";
                }

                BusinessLog bLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                Comment currComment = businessComment.Get(objectContxt, comID);
                if (currComment == null)
                {
                    return string.Empty;
                }

                if (!currComment.UserIDReference.IsLoaded)
                {
                    currComment.UserIDReference.Load();
                }

                if (!BusinessUser.CanAdminEditStuffFromUser(currentUser, Tools.GetUserFromUserDatabase(currComment.UserID)))
                {
                    return "You cannot edit this comment.";
                }

                string result = "";

                if (CommonCode.Validate.ValidateComment(ref message, out result))
                {
                    Page somPage = new Page();
                    message = somPage.Server.HtmlEncode(message);

                    if (message == currComment.description)
                    {
                        return "Edit comment !";
                    }

                    businessComment.ModifyCommentDescription(objectContxt, currComment, message, currentUser, bLog, CommentType.Product);

                    result = "Comment changed!<span id=\"reload\" style=\"visibility:hidden;\"></span>";
                }

                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string EditTopicComment(string commID, string message)
        {
            try
            {

                if (string.IsNullOrEmpty(commID))
                {
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(message))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errCantDelTopicComm").ToString();
                }

                message = Tools.BreakPossibleHtmlCode(message);

                long comID = 0;
                if (long.TryParse(commID, out comID))
                {
                    if (comID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();
                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();

                Comment currComment = businessComment.Get(objectContxt, comID);
                if (currComment == null)
                {
                    return string.Empty;
                }

                if (!currComment.UserIDReference.IsLoaded)
                {
                    currComment.UserIDReference.Load();
                }

                long userID = CommonCode.CurrentUser.GetCurrentUserId();
                User currentUser = businessUser.Get(userContext, userID, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }
                else
                {
                    if (currComment.UserID.ID != currentUser.ID)
                    {
                        if (businessUser.IsFromAdminTeam(currentUser))
                        {
                            if (!BusinessUser.CanAdminEditStuffFromUser(currentUser, Tools.GetUserFromUserDatabase(currComment.UserID)))
                            {
                                return HttpContext.GetGlobalResourceObject("WebMethods", "errAdmCantModifTopicComm").ToString();
                            }
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }

                BusinessLog bLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                string result = "";

                if (CommonCode.Validate.ValidateComment(ref message, out result, 1, Configuration.FieldsMaxDescriptionFieldLength))
                {
                    Page somPage = new Page();
                    message = somPage.Server.HtmlEncode(message);

                    if (message == currComment.description)
                    {
                        return HttpContext.GetGlobalResourceObject("WebMethods", "errChangeTopicComm").ToString();
                    }

                    businessComment.ModifyCommentDescription(objectContxt, currComment, message, currentUser, bLog, CommentType.Topic);

                    result = string.Format("{0}<span id=\"reload\" style=\"visibility:hidden;\"></span>",
                        HttpContext.GetGlobalResourceObject("WebMethods", "TopicCommUpdated"));
                }

                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string EditTopic(string strTopicID, string subject, string description)
        {
            try
            {

                if (string.IsNullOrEmpty(strTopicID) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(description))
                {
                    return string.Empty;
                }

                description = Tools.BreakPossibleHtmlCode(description);
                subject = Tools.BreakPossibleHtmlCode(subject);

                long topicID = 0;
                if (long.TryParse(strTopicID, out topicID))
                {
                    if (topicID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                BusinessUser businessUser = new BusinessUser();
                BusinessProductTopics bpTopic = new BusinessProductTopics();
                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();

                ProductTopic currTopic = bpTopic.Get(objectContxt, topicID, false, false);
                if (currTopic == null)
                {
                    return string.Empty;
                }

                if (!currTopic.UserReference.IsLoaded)
                {
                    currTopic.UserReference.Load();
                }

                long userID = CommonCode.CurrentUser.GetCurrentUserId();
                User currentUser = businessUser.Get(userContext, userID, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }
                else
                {
                    if (currTopic.User.ID == currentUser.ID)
                    {
                        if (currTopic.visible == false || currTopic.locked == true)
                        {
                            return string.Empty;
                        }

                        BusinessProduct bProduct = new BusinessProduct();
                        if (!currTopic.ProductReference.IsLoaded)
                        {
                            currTopic.ProductReference.Load();
                        }

                        string error = string.Empty;
                        if (!bProduct.CheckIfProductsIsValidWithConnections(objectContxt, currTopic.Product, out error))
                        {
                            return string.Empty;
                        }
                    }
                    else
                    {
                        if (businessUser.IsFromAdminTeam(currentUser))
                        {
                            if (!BusinessUser.CanAdminEditStuffFromUser(currentUser, Tools.GetUserFromUserDatabase(currTopic.User)))
                            {
                                return HttpContext.GetGlobalResourceObject("WebMethods", "errAdmCantModifTopic").ToString();
                            }
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }

                BusinessLog bLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                string result = "";
                Page somPage = new Page();
                bool changeDone = false;

                if (!Validate.ValidateDescription(Configuration.TopicSubjectMinLength
                     , Configuration.TopicSubjectMaxLength, ref subject
                     , "subject", out result, 90))
                {
                    return result;
                }
                else if (!Validate.ValidateNamesForSpacesFormat(ref subject, out result))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errTopicSubjectFormat").ToString();
                }

                if (!Validate.ValidateDescription(Configuration.FieldsMinDescriptionFieldLength
                    , Configuration.FieldsMaxDescriptionFieldLength, ref description
                    , "description", out result, 90))
                {
                    return result;
                }

                if (subject != currTopic.name)
                {
                    subject = somPage.Server.HtmlEncode(subject);
                    bpTopic.UpdateTopicName(objectContxt, userContext, currentUser, bLog, currTopic, subject);
                    changeDone = true;
                }

                if (description != currTopic.description)
                {
                    description = somPage.Server.HtmlEncode(description);
                    changeDone = true;
                    bpTopic.UpdateTopicDescription(objectContxt, userContext, currentUser, bLog, currTopic, description);
                }

                if (changeDone == true)
                {
                    result = string.Format("{0}<span id=\"reload\" style=\"visibility:hidden;\"></span>",
                        HttpContext.GetGlobalResourceObject("WebMethods", "TopicUpdated"));
                }
                else
                {
                    result = HttpContext.GetGlobalResourceObject("WebMethods", "errUpdateTopic").ToString();
                }

                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string DeleteComment(string commID, bool sendWarning)
        {
            try
            {

                if (string.IsNullOrEmpty(commID))
                {
                    return string.Empty;
                }

                long comID = 0;
                if (long.TryParse(commID, out comID))
                {
                    if (comID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                EntitiesUsers userContext = new EntitiesUsers();
                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();

                Comment currComment = businessComment.Get(objectContxt, comID);
                if (currComment == null)
                {
                    return string.Empty;
                }

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContxt, false);
                if (currentUser == null || !businessUser.IsFromAdminTeam(currentUser))
                {
                    return "Only administrators can delete comments.";
                }
                else if (!businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
                {
                    return "You cannot delete comments.";
                }

                BusinessLog bLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                if (!currComment.UserIDReference.IsLoaded)
                {
                    currComment.UserIDReference.Load();
                }

                if (!BusinessUser.CanAdminEditStuffFromUser(currentUser, Tools.GetUserFromUserDatabase(userContext, currComment.UserID)))
                {
                    return "You cannot delete this comment!";
                }

                string result = string.Empty;

                lock (lockSpam)
                {

                    businessComment.DeleteComment(objectContxt, userContext, currComment, currentUser, bLog, true, sendWarning);

                    result = "Comment deleted!<span id=\"reload\" style=\"visibility:hidden;\"></span>";
                    return result;
                }

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }


        public static string AddCommentToTopic(string strTopicId, string message)
        {

            try
            {

                if (string.IsNullOrEmpty(strTopicId))
                {
                    return string.Empty;
                }

                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();
                BusinessProductTopics bpTopic = new BusinessProductTopics();

                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();

                if (string.IsNullOrEmpty(message))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errWriteComment").ToString();
                }

                message = Tools.BreakPossibleHtmlCode(message);

                long topicID = 0;
                if (long.TryParse(strTopicId, out topicID))
                {
                    if (topicID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContxt, false);
                if (currentUser == null)
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errLogInToReply").ToString();
                }
                else if (!businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errCantWrite").ToString();
                }
                BusinessLog businessLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                string result = "";

                ProductTopic topic = bpTopic.Get(objectContxt, topicID, false, true);

                if (!CanUserPostCommentToTopic(topic, out result))
                {
                    return result;
                }

                if (!CommonCode.Validate.ValidateComment(ref message, out result, 1, Configuration.FieldsMaxDescriptionFieldLength))
                {
                    return result;
                }

                Page somPage = new Page();
                message = somPage.Server.HtmlEncode(message);

                businessComment.AddTopicComment(userContext, objectContxt, currentUser, topic, message
                    , businessLog, HttpContext.Current.Request.UserHostAddress, CommentSubType.Comment, topic.ID);

                result = string.Format("{0}<span id=\"reload\" style=\"visibility:hidden;\"></span>"
                            , HttpContext.GetGlobalResourceObject("WebMethods", "CommToTopicAdded"));

                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string ReplyToComment(string commID, string username, string message, CommentType commType)
        {
            try
            {

                if (string.IsNullOrEmpty(commID))
                {
                    return string.Empty;
                }

                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();


                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();

                if (string.IsNullOrEmpty(message))
                {
                    switch (commType)
                    {
                        case CommentType.Product:
                            return HttpContext.GetGlobalResourceObject("WebMethods", "errWriteOpinion").ToString();
                        case CommentType.Topic:
                            return HttpContext.GetGlobalResourceObject("WebMethods", "errWriteComment").ToString();
                        default:
                            throw new UIException(string.Format("CommentType = {0} is not supported when Replying to comment", commType));
                    }
                }

                message = Tools.BreakPossibleHtmlCode(message);

                long comID = 0;
                if (long.TryParse(commID, out comID))
                {
                    if (comID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContxt, false);
                if (currentUser == null)
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errLogInToReply").ToString();
                }
                else if (!businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errCantWrite").ToString();
                }
                BusinessLog businessLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                Comment currComment = businessComment.Get(objectContxt, comID);
                if (currComment == null)
                {
                    return string.Empty;
                }

                int level = businessComment.GetSubCommentLevel(objectContxt, currComment);

                if (level >= Configuration.CommentsMaxCommentsReplyLevel)
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errMaxReplyLvl").ToString();
                }


                string result = "";

                switch (commType)
                {
                    case CommentType.Product:
                        if (!CommonCode.Validate.ValidateComment(ref message, out result))
                        {
                            return result;
                        }
                        break;
                    case CommentType.Topic:
                        if (!CommonCode.Validate.ValidateComment(ref message, out result, 1, Configuration.FieldsMaxDescriptionFieldLength))
                        {
                            return result;
                        }
                        break;
                    default:
                        if (!CommonCode.Validate.ValidateComment(ref message, out result))
                        {
                            return result;
                        }
                        break;
                }


                Page somPage = new Page();
                message = somPage.Server.HtmlEncode(message);

                CommentSubType subType = CommentSubType.SubComment;
                long subTypeID = currComment.ID;

                switch (commType)
                {
                    case CommentType.Product:

                        BusinessProduct businessProduct = new BusinessProduct();

                        Product currProduct = businessProduct.GetProductByIDWV(objectContxt, currComment.typeID);
                        if (currProduct == null)
                        {
                            throw new CommonCode.UIException(string.Format("There is no product ID : {0}, which can be commented by user id {1}"
                                , currComment.typeID, currentUser.ID));
                        }

                        if (!CanUserPostCommentToProduct(currProduct, out result))
                        {
                            return result;
                        }

                        businessComment.AddProductComment(userContext, objectContxt, currentUser, "none", currProduct
                        , null, message, businessLog, HttpContext.Current.Request.UserHostAddress,
                        subType, subTypeID, null, null);

                        break;
                    case CommentType.Topic:

                        BusinessProductTopics bpTopic = new BusinessProductTopics();

                        ProductTopic topic = bpTopic.Get(objectContxt, currComment.typeID, false, true);

                        if (!CanUserPostCommentToTopic(topic, out result))
                        {
                            return result;
                        }

                        businessComment.AddTopicComment(userContext, objectContxt, currentUser, topic, message
                            , businessLog, HttpContext.Current.Request.UserHostAddress, subType, subTypeID);

                        break;
                    default:
                        throw new UIException(string.Format("CommentType = {0} is not supported when Replying to comment", commType));
                }


                result = string.Format("{0}<span id=\"reload\" style=\"visibility:hidden;\"></span>"
                            , HttpContext.GetGlobalResourceObject("WebMethods", "ReplySent"));


                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static bool CanUserPostCommentToProduct(Product currProduct, out string error)
        {
            try
            {
                if (currProduct == null)
                {
                    throw new CommonCode.UIException("currProduct is null");
                }

                bool result = false;
                error = string.Empty;

                Entities objContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();

                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();
                User currUser = CommonCode.UiTools.GetCurrentUser(userContext, objContext, false);
                string ipAdress = HttpContext.Current.Request.UserHostAddress;

                if (currUser == null || businessUser.IsFromUserTeam(currUser))
                {
                    if (currProduct.visible == false)
                    {
                        return false;
                    }
                }

                if (businessComment.CheckIfMaxUserCommentsForProductsAreReached(objContext, currProduct, currUser, ipAdress) == false)
                {
                    int min = 0;

                    if (businessComment.CheckIfMinimumTimeBetweenPostingCommentsPassed(objContext, currUser, ipAdress, out min) == true)
                    {
                        result = true;
                    }
                    else
                    {
                        if (min < 1)
                        {
                            min = 1;
                        }


                        error = string.Format("{0} {1}{2} {3}{4}."
                            , HttpContext.GetGlobalResourceObject("WebMethods", "errMinTimeBetwPostingComm")
                            , Configuration.ProductsMinTimeBetweenComments
                            , HttpContext.GetGlobalResourceObject("WebMethods", "errMinTimeBetwPostingComm2")
                            , min
                            , HttpContext.GetGlobalResourceObject("WebMethods", "errMinTimeBetwPostingComm3"));

                    }
                }
                else
                {
                    error = HttpContext.GetGlobalResourceObject("WebMethods", "errMaxNumCommReached").ToString();
                }

                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                error = HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();

                return false;
            }

        }

        public static bool CanUserPostCommentToTopic(ProductTopic currTopic, out string error)
        {
            try
            {
                if (currTopic == null)
                {
                    throw new CommonCode.UIException("currTopic is null");
                }

                bool result = false;
                error = string.Empty;

                Entities objContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();

                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();

                User currUser = CommonCode.UiTools.GetCurrentUser(userContext, objContext, false);
                string ipAdress = HttpContext.Current.Request.UserHostAddress;

                if (businessUser.IsFromUserTeam(currUser))
                {
                    if (currTopic.visible == false || currTopic.locked == true)
                    {
                        return false;
                    }

                    BusinessProduct bProduct = new BusinessProduct();

                    if (!currTopic.ProductReference.IsLoaded)
                    {
                        currTopic.ProductReference.Load();
                    }

                    if (!bProduct.CheckIfProductsIsValidWithConnections(objContext, currTopic.Product, out error))
                    {
                        error = string.Empty;
                        return false;
                    }
                }

                int min = 0;

                if (businessComment.CheckIfMinimumTimeBetweenPostingCommentsPassed(objContext, currUser, ipAdress, out min) == true)
                {
                    result = true;
                }
                else
                {
                    if (min < 1)
                    {
                        min = 1;
                    }


                    error = string.Format("{0} {1}{2} {3}{4}."
                        , HttpContext.GetGlobalResourceObject("WebMethods", "errMinTimeBetwPostingTopicComm")
                        , Configuration.ProductsMinTimeBetweenComments
                        , HttpContext.GetGlobalResourceObject("WebMethods", "errMinTimeBetwPostingComm2")
                        , min
                        , HttpContext.GetGlobalResourceObject("WebMethods", "errMinTimeBetwPostingComm3"));

                }


                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                error = HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();

                return false;
            }

        }

        public static string SendReplyToReport(string strId, string description)
        {
            try
            {

                if (string.IsNullOrEmpty(strId))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(description))
                {
                    return string.Empty;
                }

                description = Tools.BreakPossibleHtmlCode(description);

                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();

                BusinessUser bUser = new BusinessUser();
                BusinessReport bReport = new BusinessReport();


                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }
                BusinessLog bLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), HttpContext.Current.Request.UserHostAddress);

                long id = 0;
                if (!long.TryParse(strId, out id))
                {
                    return string.Empty;
                }

                Report currReport = bReport.Get(objectContext, id);
                if (currReport == null)
                {
                    return string.Empty;
                }

                if (currReport.isResolved == true || currReport.isDeletedByUser == true || currReport.reportType == "spam")
                {
                    return string.Empty;
                }

                string error = string.Empty;
                if (!Validate.ValidateDescription(1, Configuration.FieldsMaxDescriptionFieldLength, ref description, "description", out error, 90))
                {
                    return error;
                }

                if (bUser.IsFromAdminTeam(currentUser))
                {

                    if (bUser.IsModerator(currentUser) == true)
                    {
                        if (currReport.aboutType != "user" || currReport.isResolved)
                        {
                            return string.Empty;
                        }
                    }

                    bReport.CreateReportComment(objectContext, userContext, currentUser, currReport, description, true, bLog);

                    return string.Format("{0}{1}", HttpContext.GetGlobalResourceObject("WebMethods", "ReplyToReportSent")
                    , "<span id=\"postback\" style=\"visibility:hidden;\"></span>");
                }
                else
                {
                    // IF USER
                    if (!currReport.CreatedByReference.IsLoaded)
                    {
                        currReport.CreatedByReference.Load();
                    }

                    if (currReport.CreatedBy.ID != currentUser.ID)
                    {
                        return string.Empty;
                    }

                    if (bReport.CountReportComments(objectContext, currReport) < 1)
                    {
                        return string.Empty;
                    }

                    bReport.CreateReportComment(objectContext, userContext, currentUser, currReport, description, false, bLog);

                    return string.Format("{0}{1}", HttpContext.GetGlobalResourceObject("WebMethods", "ReplyToReportSent")
                    , "<span id=\"reload\" style=\"visibility:hidden;\"></span>");
                }



            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }


        public static string SendTypeSuggestion(string strUserID, string type, string strTypeID, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(strUserID))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(type))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(strTypeID))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(description))
                {
                    return string.Empty;
                }

                string error = string.Empty;
                description = Tools.BreakPossibleHtmlCode(description);
                if (!Validate.ValidateDescription(0, Configuration.FieldsMaxDescriptionFieldLength, ref description
                    , "description", out error, 90))
                {
                    return error;
                }

                long userId = 0;
                if (!long.TryParse(strUserID, out userId))
                {
                    return string.Empty;
                }

                long typeId = 0;
                if (!long.TryParse(strTypeID, out typeId))
                {
                    return string.Empty;
                }

                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();
                BusinessUser businessUser = new BusinessUser();
                BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();


                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }
                BusinessLog bLog = new BusinessLog(CurrentUser.GetCurrentUserId(), HttpContext.Current.Request.UserHostAddress);

                User receiver = businessUser.Get(userContext, userId, false);
                if (receiver == null)
                {
                    return string.Empty; ;
                }

                if (currentUser == receiver)
                {
                    return string.Empty;
                }

                if (btSuggestions.CountUserSuggestions(objectContext, true, currentUser) >=
                    Configuration.TypeSuggestionMaxActiveSuggestionsPerUser)
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errMaxTypeSuggNumberReached").ToString();
                }

                string result = string.Empty;

                switch (type)
                {
                    case "product":
                        BusinessProduct bProduct = new BusinessProduct();
                        Product currProduct = bProduct.GetProductByID(objectContext, typeId);
                        if (currProduct == null)
                        {
                            return string.Empty;
                        }

                        if (btSuggestions.CanUserSendSuggestionForProduct
                            (userContext, objectContext, currentUser, receiver, currProduct, out result) == true)
                        {
                            btSuggestions.AddProductSuggestion(objectContext, userContext, currentUser, receiver, currProduct, description, bLog);
                            result = string.Format("{0} {1} {2} {3}!", HttpContext.GetGlobalResourceObject("WebMethods", "TypeSuggestionSent")
                                , currProduct.name, HttpContext.GetGlobalResourceObject("WebMethods", "TypeSuggestionSent2"), receiver.username);
                        }

                        break;
                    case "company":
                        BusinessCompany bCompany = new BusinessCompany();
                        Company currCompany = bCompany.GetCompany(objectContext, typeId);
                        if (currCompany == null)
                        {
                            return string.Empty;
                        }

                        if (btSuggestions.CanUserSendSuggestionForCompany
                            (userContext, objectContext, currentUser, receiver, currCompany, out result) == true)
                        {
                            btSuggestions.AddCompanySuggestion(objectContext, userContext, currentUser, receiver, currCompany, description, bLog);
                            result = string.Format("{0} {1} {2} {3}!", HttpContext.GetGlobalResourceObject("WebMethods", "TypeSuggestionSent")
                                , currCompany.name, HttpContext.GetGlobalResourceObject("WebMethods", "TypeSuggestionSent2"), receiver.username);
                        }

                        break;
                    default:
                        return string.Empty;
                }


                return result;

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }
        }

        public static string AddCommentToSuggestion(string strSuggId, string description)
        {
            try
            {

                if (string.IsNullOrEmpty(strSuggId))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(description))
                {
                    return string.Empty;
                }

                string error = string.Empty;
                description = Tools.BreakPossibleHtmlCode(description);
                if (!Validate.ValidateDescription(0, Configuration.FieldsMaxDescriptionFieldLength, ref description
                    , "description", out error, 90))
                {
                    return error;
                }

                long id = 0;
                if (!long.TryParse(strSuggId, out id))
                {
                    return string.Empty;
                }

                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();
                BusinessUser businessUser = new BusinessUser();
                BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();


                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }
                BusinessLog bLog = new BusinessLog(CurrentUser.GetCurrentUserId(), HttpContext.Current.Request.UserHostAddress);

                TypeSuggestion currSuggestion = btSuggestions.GetSuggestion(objectContext, id, true, false);
                if (currSuggestion == null)
                {
                    return string.Empty;
                }

                if (currSuggestion.active == false)
                {
                    return string.Empty;
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
                    return string.Empty;
                }

                btSuggestions.AddCommentToSuggestion(objectContext, userContext, currSuggestion, currentUser, false, description);
                return string.Format("{0}<span id=\"reload\" style=\"visibility:hidden;\"></span>"
                    , HttpContext.GetGlobalResourceObject("WebMethods", "CommentToSuggestionWritten"));

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }


        }


        public static string GetRateCommentData(string commID, string rating)
        {
            try
            {

                if (string.IsNullOrEmpty(commID) || string.IsNullOrEmpty(rating))
                {
                    return string.Empty;
                }

                EntitiesUsers userContext = new EntitiesUsers();
                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();
                BusinessRating businessRating = new BusinessRating();


                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContxt, false);
                if (currentUser == null)
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errLogInToRate").ToString();
                }
                else if (!businessUser.CanUserDo(userContext, currentUser, UserRoles.RateComments))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errCantRate").ToString();
                }
                BusinessLog bLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), HttpContext.Current.Request.UserHostAddress);

                int rat = 1;   // 1 = agree , -1 = disagree
                if (int.TryParse(rating, out rat))
                {
                    if (rat != -1 && rat != 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                long comID = 0;
                if (long.TryParse(commID, out comID))
                {
                    if (comID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                Comment currComment = businessComment.Get(objectContxt, comID);
                if (currComment == null)
                {
                    return string.Empty;
                }

                System.Text.StringBuilder result = new System.Text.StringBuilder();


                if (!currComment.UserIDReference.IsLoaded)
                {
                    currComment.UserIDReference.Load();
                }

                if (currComment.UserID.ID == currentUser.ID)
                {
                    result.Append(HttpContext.GetGlobalResourceObject("WebMethods", "errCantRateMyComms"));

                    return result.ToString();
                }
                else if (businessUser.IsFromAdminTeam(Tools.GetUserFromUserDatabase(currComment.UserID)))
                {
                    result.Append(HttpContext.GetGlobalResourceObject("WebMethods", "errCantRateAdminComms"));

                    return result.ToString();
                }

                DateTime userLocalTimeNow = UiTools.DateTimeToUserLocalTime(DateTime.UtcNow);
                string error = string.Empty;

                lock (lockRating)
                {
                    if (!businessRating.CanUserRateThisComment(objectContxt, currentUser, currComment, userLocalTimeNow, out error))
                    {
                        result.Append(error);
                    }
                    else
                    {
                        businessRating.RateComment(objectContxt, currentUser, currComment, rat, bLog, userLocalTimeNow);

                        result.Append(string.Format("{0} <span style=\"visibility:hidden;\"></span>"
                            , HttpContext.GetGlobalResourceObject("WebMethods", "CommentRated")));
                    }
                }

                return result.ToString();

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }


        }

        public static string SetMsgAsViolation(string commID, CommentType commType)
        {

            try
            {

                if (string.IsNullOrEmpty(commID))
                {
                    return string.Empty;
                }

                long comID = 0;
                if (long.TryParse(commID, out comID))
                {
                    if (comID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                EntitiesUsers userContext = new EntitiesUsers();
                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();
                BusinessReport businessReport = new BusinessReport();

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContxt, false);

                int numReports = businessReport.NumberOfSpamReportsWhichUserCanSend(objectContxt, currentUser);

                string error = string.Empty;
                string resultDescr = string.Empty;

                switch (commType)
                {
                    case CommentType.Product:
                        if (businessReport.CheckIfUserCanSendReportAboutType(objectContxt, userContext, currentUser, "spam", "comment", comID, out error) == false)
                        {
                            return error;
                        }

                        resultDescr = string.Format("{0} {1}", HttpContext.GetGlobalResourceObject("WebMethods", "OpinionMarkedSpam"), (numReports - 1));

                        break;
                    case CommentType.Topic:
                        if (businessReport.CheckIfUserCanSendReportAboutType(objectContxt, userContext, currentUser, "spam", "comment", comID, out error) == false)
                        {
                            return error;
                        }

                        resultDescr = string.Format("{0} {1}", HttpContext.GetGlobalResourceObject("WebMethods", "CommentMarkedSpam"), (numReports - 1));

                        break;
                    default:
                        throw new UIException(string.Format("Comment type = {0} is not supported when reporting for spam,", commType));
                }


                Comment currComment = businessComment.Get(objectContxt, comID);
                if (currComment == null)
                {
                    return string.Empty;
                }

                BusinessLog bLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                string result = resultDescr;

                lock (lockSpam)
                {
                    businessReport.CreateCommentViolationReport(objectContxt, userContext, currentUser, bLog, currComment, commType);

                    return result;
                }

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }
        }


        public static string SendMsgToUser(string commID, string username, string message, string subject, string saveInSent)
        {
            try
            {

                if (string.IsNullOrEmpty(commID) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(saveInSent))
                {
                    return string.Empty;
                }

                EntitiesUsers userContext = new EntitiesUsers();
                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();
                BusinessMessages businessMessages = new BusinessMessages();

                if (string.IsNullOrEmpty(message))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errWriteMessage").ToString();
                }

                long comID = 0;
                if (long.TryParse(commID, out comID))
                {
                    if (comID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                bool save = false;
                if (!bool.TryParse(saveInSent, out save))
                {
                    return string.Empty;
                }

                Comment currComment = businessComment.Get(objectContxt, comID);
                if (currComment == null)
                {
                    return string.Empty;
                }

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContxt, false);
                if (currentUser == null)
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errLogInSendMsg").ToString();
                }

                bool isAdmin = businessUser.IsFromAdminTeam(currentUser);

                if (!businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errCantSendMsg").ToString();
                }

                BusinessLog bLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                if (!currComment.UserIDReference.IsLoaded)
                {
                    currComment.UserIDReference.Load();
                }

                if (Tools.GetUserFromUserDatabase(currComment.UserID) == currentUser)
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errCantSendMsgToUrself").ToString();
                }

                if (businessUser.IsGuest(userContext, Tools.GetUserFromUserDatabase(currComment.UserID)))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errCantSendMsgToGuest").ToString();
                }

                if (!isAdmin && !businessUser.IsFromUserTeam(Tools.GetUserFromUserDatabase(currComment.UserID)))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errCantSendMsgToAdmin").ToString();
                }

                if (!businessMessages.CanUserSendMessageTo(userContext, currentUser.ID, currComment.UserID.ID))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errCantSendMsgToUser").ToString();
                }

                string result = string.Empty;

                lock (lockSendMessage)
                {
                    if (CommonCode.Validate.ValidateDescription(Configuration.MessagesMinSubjectLength,
                        Configuration.MessagesMaxSubjectLength, ref subject, "subject", out result, 90))
                    {
                        if (CommonCode.Validate.ValidateDescription(1, Configuration.FieldsMaxDescriptionFieldLength, ref message, "description", out result, 90))
                        {

                            int subjectLength = subject.Length;
                            if (Configuration.MessagesMinSubjectLength <= subjectLength
                                && Configuration.MessagesMaxSubjectLength >= subjectLength)
                            {

                                Page somPage = new Page();
                                message = somPage.Server.HtmlEncode(message);

                                Message newMessage = new Message();
                                newMessage.FromUser = currentUser;
                                newMessage.ToUser = Tools.GetUserFromUserDatabase(userContext, currComment.UserID);
                                if (string.IsNullOrEmpty(subject))
                                {
                                    newMessage.subject = HttpContext.GetGlobalResourceObject("WebMethods", "NoSubject").ToString();
                                }
                                else
                                {
                                    newMessage.subject = subject;
                                }
                                newMessage.description = message;
                                if (save)
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


                                result = HttpContext.GetGlobalResourceObject("WebMethods", "MessageSent").ToString();
                            }
                            else
                            {
                                result = HttpContext.GetGlobalResourceObject("WebMethods", "InvalidSubjectLength").ToString();
                            }
                        }
                    }
                    return result;
                }


            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }
        }

        public static string SetSuggestionAsViolation(string suggID)
        {
            try
            {

                if (string.IsNullOrEmpty(suggID))
                {
                    return string.Empty;
                }

                long sugID = 0;
                if (long.TryParse(suggID, out sugID))
                {
                    if (sugID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                EntitiesUsers userContext = new EntitiesUsers();
                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                BusinessUser businessUser = new BusinessUser();
                BusinessSuggestion businessSuggestion = new BusinessSuggestion();
                BusinessReport businessReport = new BusinessReport();

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContxt, false);

                string error = string.Empty;
                if (businessReport.CheckIfUserCanSendReportAboutType(objectContxt, userContext, currentUser, "spam", "suggestion", sugID, out error) == false)
                {
                    return error;
                }

                Suggestion currSuggestion = businessSuggestion.Get(objectContxt, sugID);
                if (currSuggestion == null)
                {
                    return string.Empty;
                }

                BusinessLog bLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                int numReports = businessReport.NumberOfSpamReportsWhichUserCanSend(objectContxt, currentUser);

                string result = string.Empty;

                lock (lockSpam)
                {
                    businessReport.CreateSuggestionViolationReport(userContext, objectContxt, currentUser, bLog, currSuggestion);

                    result = string.Format("{0} {1}", HttpContext.GetGlobalResourceObject("WebMethods", "SuggestionMarkedAsSpam")
                        , (numReports - 1));
                    return result;
                }

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }
        }

        public static string SetProductLinkAsViolation(string strLinkId)
        {
            try
            {

                if (string.IsNullOrEmpty(strLinkId))
                {
                    return string.Empty;
                }

                long linkID = 0;
                if (long.TryParse(strLinkId, out linkID))
                {
                    if (linkID < 1)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                EntitiesUsers userContext = new EntitiesUsers();
                Entities objectContxt = UiTools.CreateEntitiesForWebMethod();
                BusinessUser businessUser = new BusinessUser();
                BusinessProductLink businessLink = new BusinessProductLink();
                BusinessReport businessReport = new BusinessReport();

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContxt, false);

                string error = string.Empty;
                if (businessReport.CheckIfUserCanSendReportAboutType(objectContxt, userContext, currentUser, "spam", "productLink", linkID, out error) == false)
                {
                    return error;
                }

                ProductLink currLink = businessLink.Get(objectContxt, linkID, true, false);
                if (currLink == null)
                {
                    return string.Empty;
                }

                BusinessLog bLog = new BusinessLog(currentUser.ID, HttpContext.Current.Request.UserHostAddress);

                int numReports = businessReport.NumberOfSpamReportsWhichUserCanSend(objectContxt, currentUser);

                string result = string.Empty;

                lock (lockSpam)
                {
                    businessReport.CreateProductViolationReport(userContext, objectContxt, currentUser, bLog, currLink);

                    result = string.Format("{0} {1}", HttpContext.GetGlobalResourceObject("WebMethods", "ProductLinkMarkedAsSpam")
                        , (numReports - 1));
                    return result;
                }

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }
        }

        public static void ValidateUserInput(string text, string type, string strCharId, out string error)
        {
            try
            {

                error = "";

                if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(text))
                {
                    Entities ObjectContext = UiTools.CreateEntitiesForWebMethod();

                    switch (type)
                    {
                        case ("usernameReg"):    // For username (format/existing) when regging
                            Validate.ValidateUserName(ObjectContext, ref text, out error);
                            break;
                        case ("usernameFound"):  // For fields where existing name is needed

                            EntitiesUsers userContext = new EntitiesUsers();

                            text = Tools.RemoveSpacesAtEndOfString(text);

                            error = HttpContext.GetGlobalResourceObject("Validate", "errNoSuchUser").ToString();
                            if (text[0] != ' ' && text[text.Length - 1] != ' ')
                            {
                                if (BusinessUser.CheckIfThereIsUserWithName(userContext, text, false))
                                {
                                    error = "";
                                }
                            }
                            break;
                        case ("usernameFormat"): // For fields where username should be typed without checking for existing
                            Validate.ValidateUserNameFormat(ref text, out error, true);
                            break;
                        case ("passFormat"):     // For user password fields
                            Validate.ValidatePassword(text, out error);
                            break;
                        case ("emailFormat"):    // For email fields
                            Validate.ValidateEmailAdress(text, out error);
                            break;
                        case ("productnameAdd"): // For product name (format/existing)
                            Validate.ValidateName(ObjectContext, "products", ref text, Configuration.ProductsMinProductNameLength,
                                Configuration.ProductsMaxProductNameLength, out error, 0);
                            break;
                        case ("productCharAdd"): // For product characteristic name (format/existing)
                            if (!string.IsNullOrEmpty(strCharId))
                            {
                                long charId = 0;
                                if (long.TryParse(strCharId, out charId))
                                {
                                    if (charId > 0)
                                    {
                                        Validate.ValidateName(ObjectContext, "prodChar", ref text, Configuration.ProductsMinProductNameLength,
                                            Configuration.ProductsMaxProductNameLength, out error, charId);
                                    }
                                    else
                                    {
                                        throw new CommonCode.UIException(string.Format("charId = '{0}' is < 1", charId));
                                    }
                                }
                                else
                                {
                                    throw new CommonCode.UIException(string.Format("Couldnt parse strCharId = '{0}' to Long", strCharId));
                                }
                            }
                            else
                            {
                                throw new CommonCode.UIException("strCharId is null or empty.");
                            }
                            break;
                        case ("companynameAdd"): // For company name (format/existing)
                            Validate.ValidateName(ObjectContext, "companies", ref text, Configuration.CompaniesMinCompanyNameLength,
                                Configuration.CompaniesMaxCompanyNameLength, out error, 0);
                            break;
                        case ("companyType"): // For company type name (format/existing)
                            Validate.ValidateName(ObjectContext, "companyType", ref text, Configuration.CompaniesMinCompanyNameLength,
                               Configuration.CompaniesMaxCompanyNameLength, out error, 0);
                            break;
                        case ("companyCharAdd"): // For company characteristic name (format/existing)
                            if (!string.IsNullOrEmpty(strCharId))
                            {
                                long compId = 0;
                                if (long.TryParse(strCharId, out compId))
                                {
                                    if (compId > 0)
                                    {
                                        Validate.ValidateName(ObjectContext, "compChar", ref text, Configuration.CompaniesMinCompanyNameLength,
                                            Configuration.CompaniesMaxCompanyNameLength, out error, compId);
                                    }
                                    else
                                    {
                                        throw new CommonCode.UIException(string.Format("compId = '{0}' is < 1", compId));
                                    }
                                }
                                else
                                {
                                    throw new CommonCode.UIException(string.Format("Couldnt parse strCompId = '{0}' to Long", strCharId));
                                }
                            }
                            else
                            {
                                throw new CommonCode.UIException("strCompId is null or empty.");
                            }
                            break;
                        case ("sitetextAdd"):    // For Site text name (format/existing)
                            Validate.ValidateName(ObjectContext, "siteText", ref text, Configuration.SiteTextsMinSiteTextNameLength,
                                Configuration.SiteTextsMaxSiteTextNameLength, out error, 0);
                            break;
                        case ("categoryAdd"):    // For category name fields (format/existing)

                            if (!string.IsNullOrEmpty(strCharId))
                            {
                                long charId = 0;
                                if (long.TryParse(strCharId, out charId))
                                {
                                    if (charId >= 0)
                                    {
                                        Validate.ValidateName(ObjectContext, "categories", ref text, Configuration.CategoriesMinCategoryNameLength,
                                Configuration.CategoriesMaxCategoryNameLength, out error, charId);
                                    }
                                    else
                                    {
                                        throw new CommonCode.UIException(string.Format("charId = '{0}' is < 1", charId));
                                    }
                                }
                                else
                                {
                                    throw new CommonCode.UIException(string.Format("Couldnt parse strCharId = '{0}' to Long", strCharId));
                                }
                            }
                            else
                            {
                                throw new CommonCode.UIException("strCharId is null or empty.");
                            }

                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                error = HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string[] GetAutoCompleteList(string prefixText, int count)
        {
            try
            {
                string[] result = null;

                Entities objectContext = CommonCode.UiTools.CreateEntitiesForWebMethod();

                if (prefixText.Length < 30)
                {
                    AutoComplete aComplete = new AutoComplete();

                    result = aComplete.GetResults(objectContext, prefixText, count);

                    if (result == null || result.Length == 0)
                    {
                        return null;
                    }

                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                return null;
            }

        }

        public static string[] GetCompaniesList(string prefixText, int count)
        {
            try
            {
                string[] result = null;

                Entities objectContext = CommonCode.UiTools.CreateEntitiesForWebMethod();

                if (prefixText.Length < 30)
                {
                    AutoComplete aComplete = new AutoComplete();

                    result = aComplete.GetCompanyNamesResults(objectContext, prefixText, count);

                    if (result == null || result.Length == 0)
                    {
                        return null;
                    }

                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                return null;
            }

        }

        /// <summary>
        /// Sets client`s TimeZone offset in Session
        /// </summary>
        public static string SetOffSetTimeZoneInSession(string OffSet)
        {
            string result = "success";

            if (HttpContext.Current.Session["ClientTimeZoneOffset"] == null)
            {
                int clientTimeZoneOffsetMinutes = 0;

                bool parsed = int.TryParse(OffSet, out clientTimeZoneOffsetMinutes);

                HttpContext.Current.Session["ClientTimeZoneOffset"] = clientTimeZoneOffsetMinutes.ToString();

                if (log.IsDebugEnabled == true)
                {
                    if (parsed == true)
                    {
                        log.DebugFormat("Client time zome offset in minutes: {0}.", clientTimeZoneOffsetMinutes);
                    }
                    else
                    {
                        result = "failure";

                        log.DebugFormat("Client time zome offset in minutes: unknown (could not parse \"{0}\"). Assuming {1}.",
                            OffSet, clientTimeZoneOffsetMinutes);
                    }
                }
            }

            return result;
        }

        public static string AddTopic(string toProduct, string subject, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(toProduct))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(subject))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(description))
                {
                    return string.Empty;
                }

                string error = string.Empty;

                UiTools.ChangeUiCultureFromSession();

                description = Tools.BreakPossibleHtmlCode(description);
                subject = Tools.BreakPossibleHtmlCode(subject);

                if (!Validate.ValidateDescription(10
                    , Configuration.FieldsMaxDescriptionFieldLength, ref description
                    , "description", out error, 90))
                {
                    return error;
                }

                if (!Validate.ValidateDescription(Configuration.TopicSubjectMinLength
                    , Configuration.TopicSubjectMaxLength, ref subject
                    , "subject", out error, 90))
                {
                    return error;
                }
                else if (!Validate.ValidateNamesForSpacesFormat(ref subject, out error))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errTopicSubjectFormat").ToString();
                }

                long id = 0;
                if (!long.TryParse(toProduct, out id))
                {
                    return string.Empty;
                }

                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();
                BusinessUser businessUser = new BusinessUser();
                BusinessProduct bProduct = new BusinessProduct();
                BusinessProductTopics bTopic = new BusinessProductTopics();

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }

                BusinessLog bLog = new BusinessLog(CurrentUser.GetCurrentUserId(), HttpContext.Current.Request.UserHostAddress);

                if (!businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
                {
                    return string.Empty;
                }

                Product product = bProduct.GetProductByID(objectContext, id);
                if (product == null)
                {
                    return string.Empty;
                }
                if (!bProduct.CheckIfProductsIsValidWithConnections(objectContext, product, out error))
                {
                    return string.Empty;
                }

                int minToWait = 0;
                if (!bTopic.CheckIfMinimumTimeBetweenAddingTopicsPassed(objectContext, currentUser, out minToWait))
                {
                    error = string.Format("{0} {1} {2}", HttpContext.GetGlobalResourceObject("WebMethods", "errTimeAddTopic")
                        , minToWait, HttpContext.GetGlobalResourceObject("WebMethods", "errTimeAddTopic2"));

                    return error;
                }

                bTopic.AddTopic(objectContext, userContext, currentUser, product, subject, description, bLog);

                ProductTopic lastTopic = bTopic.GetLastUserTopic(objectContext, currentUser, true);
                HttpContext.Current.Session.Add("redirrectToNewTopic", lastTopic.ID);

                return string.Format("{0}<span id=\"reload\" style=\"visibility:hidden;\"></span>",
                    HttpContext.GetGlobalResourceObject("WebMethods", "topicCreated"));

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static void UpdateTypeVisits(VisitedType type, long typeID)
        {
            if (typeID < 1)
            {
                throw new UIException("typeID < 1");
            }

            Entities objectContext = UiTools.CreateEntitiesForWebMethod();
            EntitiesUsers userContext = new EntitiesUsers();
            BusinessVisits bVisits = new BusinessVisits();

            User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);

            string key = string.Empty;

            switch (type)
            {
                case VisitedType.ProductTopic:

                    key = "VisitedProductTopics";

                    break;
                default:
                    throw new UIException(string.Format("VisitedType = {0} is not supported", type));
            }

            lock (lockVisits)
            {
                object visitedTypeListObj = HttpContext.Current.Session[key];

                bool addVisit = false;
                bool updateVisit = false;

                List<long> visits = new List<long>();

                if (visitedTypeListObj == null)
                {
                    addVisit = true;
                }
                else
                {
                    visits = visitedTypeListObj as List<long>;
                    if (visits == null)
                    {
                        HttpContext.Current.Session[key] = null;
                        throw new UIException(string.Format("Couldn't parse Session[{0}] to List<long>", key));
                    }

                    if (visits.Contains(typeID))
                    {
                        updateVisit = true;
                    }
                    else
                    {
                        addVisit = true;
                    }
                }

                if (addVisit == true)
                {
                    switch (type)
                    {
                        case VisitedType.ProductTopic:

                            BusinessProductTopics bpTopic = new BusinessProductTopics();
                            ProductTopic topic = bpTopic.Get(objectContext, typeID, false, true);

                            bVisits.ProductTopicVisited(objectContext, userContext, topic, currentUser, HttpContext.Current.Request.UserHostAddress);

                            break;
                        default:
                            throw new UIException(string.Format("VisitedType = {0} is not supported", type));
                    }

                    visits.Add(typeID);
                    HttpContext.Current.Session[key] = visits;
                }

                if (updateVisit == true)
                {
                    switch (type)
                    {
                        case VisitedType.ProductTopic:

                            BusinessProductTopics bpTopic = new BusinessProductTopics();
                            ProductTopic topic = bpTopic.Get(objectContext, typeID, false, true);

                            bVisits.UpdateProductTopicVisited(objectContext, userContext, topic, currentUser, HttpContext.Current.Request.UserHostAddress);

                            break;
                        default:
                            throw new UIException(string.Format("VisitedType = {0} is not supported", type));
                    }

                }

            }

        }

        public static string AddProductLink(string toProduct, string link, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(toProduct))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(link))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(description))
                {
                    return string.Empty;
                }

                string error = string.Empty;

                UiTools.ChangeUiCultureFromSession();

                if (description == HttpContext.GetGlobalResourceObject("SiteResources", "addProdLinkDescription").ToString())
                {
                    return HttpContext.GetGlobalResourceObject("SiteResources", "addProdLinkDescription").ToString();
                }

                description = Tools.BreakPossibleHtmlCode(description);
                link = Tools.BreakPossibleHtmlCode(link);

                if (!Validate.ValidateDescription(Configuration.ProductLinksMinDescrLength
                    , Configuration.ProductLinksMaxDescrLength, ref description
                    , "description", out error, 50))
                {
                    return error;
                }

                if (!Validate.ValidateNamesForSpacesFormat(ref link, out error))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errProductLinkFormat").ToString();
                }
                if (!Validate.ValidateSiteAdress(link, out error, false))
                {
                    return error;
                }

                long id = 0;
                if (!long.TryParse(toProduct, out id))
                {
                    return string.Empty;
                }

                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();
                BusinessUser businessUser = new BusinessUser();
                BusinessProduct bProduct = new BusinessProduct();
                BusinessProductLink bLink = new BusinessProductLink();

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }

                BusinessLog bLog = new BusinessLog(CurrentUser.GetCurrentUserId(), HttpContext.Current.Request.UserHostAddress);

                Product product = bProduct.GetProductByIDWV(objectContext, id);
                if (product == null)
                {
                    return string.Empty;
                }

                if (bLink.CanUserAddLink(objectContext, userContext, product, currentUser, true, out error) == false)
                {
                    return error;
                }

                if (!Tools.NameValidatorPassed(objectContext, "productLink", link, product.ID))
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errProdLinkDuplicating").ToString();
                }

                bLink.Add(objectContext, bLog, product, currentUser, link, description);

                return string.Format("{0}<span id=\"reload\" style=\"visibility:hidden;\"></span>",
                    HttpContext.GetGlobalResourceObject("WebMethods", "ProductLinkAdded"));

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string EditProductLinkDescr(string strLinkId, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(strLinkId))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(description))
                {
                    return string.Empty;
                }

                string error = string.Empty;

                UiTools.ChangeUiCultureFromSession();

                description = Tools.BreakPossibleHtmlCode(description);

                if (!Validate.ValidateDescription(Configuration.ProductLinksMinDescrLength
                    , Configuration.ProductLinksMaxDescrLength, ref description
                    , "description", out error, 50))
                {
                    return error;
                }


                long id = 0;
                if (!long.TryParse(strLinkId, out id))
                {
                    return string.Empty;
                }

                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();
                BusinessUser businessUser = new BusinessUser();
                BusinessProductLink bLink = new BusinessProductLink();

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }

                BusinessLog bLog = new BusinessLog(CurrentUser.GetCurrentUserId(), HttpContext.Current.Request.UserHostAddress);

                ProductLink currLink = bLink.Get(objectContext, id, true, false);
                if (currLink == null)
                {
                    return string.Empty;
                }

                if (currLink.description == description)
                {
                    return HttpContext.GetGlobalResourceObject("WebMethods", "errUpdateDescription").ToString();
                }

                if (bLink.CanUserModifyLink(objectContext, userContext, currLink, currentUser) == false)
                {
                    return string.Empty; ;
                }

                bLink.ChangeLinkDescription(objectContext, userContext, bLog, currLink, currentUser, description);

                return string.Format("{0}<span id=\"reload\" style=\"visibility:hidden;\"></span>",
                    HttpContext.GetGlobalResourceObject("WebMethods", "ProductLinkDescriptionChanged"));

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

        public static string DeleteProductLink(string strLinkId, bool sendWarning)
        {
            try
            {
                if (string.IsNullOrEmpty(strLinkId))
                {
                    return string.Empty;
                }


                string error = string.Empty;

                UiTools.ChangeUiCultureFromSession();

                long id = 0;
                if (!long.TryParse(strLinkId, out id))
                {
                    return string.Empty;
                }

                Entities objectContext = UiTools.CreateEntitiesForWebMethod();
                EntitiesUsers userContext = new EntitiesUsers();
                BusinessUser businessUser = new BusinessUser();
                BusinessProductLink bLink = new BusinessProductLink();

                User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, false);
                if (currentUser == null)
                {
                    return string.Empty;
                }

                if (sendWarning == true && businessUser.IsFromAdminTeam(currentUser) == false)
                {
                    return string.Empty;
                }

                BusinessLog bLog = new BusinessLog(CurrentUser.GetCurrentUserId(), HttpContext.Current.Request.UserHostAddress);

                ProductLink currLink = bLink.Get(objectContext, id, true, false);
                if (currLink == null)
                {
                    return string.Empty;
                }

                if (bLink.CanUserModifyLink(objectContext, userContext, currLink, currentUser) == false)
                {
                    return string.Empty; ;
                }

                bLink.DeleteLink(objectContext, userContext, bLog, currLink, currentUser, sendWarning);

                return string.Format("{0}<span id=\"reload\" style=\"visibility:hidden;\"></span>",
                    HttpContext.GetGlobalResourceObject("WebMethods", "ProductLinkDeleted"));

            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(ex);
                }

                UiTools.ChangeUiCultureFromSession();
                return HttpContext.GetGlobalResourceObject("WebMethods", "errExceptionMsg").ToString();
            }

        }

    }
}
