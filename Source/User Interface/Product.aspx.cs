﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Text;

using log4net;

using BusinessLayer;
using DataAccess;

namespace UserInterface
{
    public partial class ProductPage : BasePage
    {

        private ILog log = LogManager.GetLogger(typeof(CompanyPage));

        protected int commentsOnPage = Configuration.ProductsCommentsOnPage;        // number of comments on page to show
        protected int numberOfPage = 1;                                             // number of current page
        protected long CommNumber = 0;                                              // number of product`s comments for the chosen sortBy
        // If comments are sorted by Variant, then it`s the number of comments for this variant

        protected int minCommOnPage = Configuration.ProductsMinCommentsOnPage;      // minimum comments on page
        protected int defCommOnPage = Configuration.ProductsDefCommentsOnPage;      // average comments on page
        protected int maxCommOnPage = Configuration.ProductsMaxCommentsOnPage;      // max comments on page

        protected long SortByChar = 0;                                              // sort comments by product characteristic
        protected long SortByVariant = 0;
        protected long SortBySubVariant = 0;
        protected Boolean SortByNoAbout = false;                                    // true if should show comments which aren`t for char or variant

        protected Boolean SortByDateDesc = true;                                    // if comments should be sorted by Date written

        protected string notifError = "";                                           // shown to admins to inform why product is not visible by users (IF IT ISNT)
        protected SortOptions  // int 
                      SortByRating = SortOptions.None;  // 0;
        // 0 - default,not sorted by rating; 
        // 1 - sorted from biggest to lowest;
        // 2 - sorted from lowest to biggest 

        protected Boolean changedChars = false;
        protected Boolean changedVariants = false;

        protected object addingImage = new object();

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private Product currentProduct = null;                                      // used in methods requiring current Product
        private User currentUser = null;                                            // used in methods requiring current USer

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (pnlEdit.Visible == true && apUploadImage.Visible == true)
            {
                CustomServerControls.DecoratedButton btnUpload = (CustomServerControls.DecoratedButton)
                    CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "btnUpload");

                btnUpload.Attributes.Add("onclick", string.Format("ShowUploadingMsg('{0}','{1}')", uploadingSpan.ClientID, lblError.ClientID));
            }

            if (lblSendReport.Visible == true)
            {
                lblSendReport.Attributes.Add("onclick", string.Format("ShowReportData('{0}','{1}','{2}','{3}','{4}')"
                        , "product", currentProduct.ID, lblSendReport.ClientID, pnlActionReport.ClientID, pnlSendReport.ClientID));
            }

            if (btnSignForNotifies.Visible == true)
            {
                btnSignForNotifies.Attributes.Add("onclick", string.Format("NotifyForUpdates('{0}','{1}','{2}','{3}')"
                   , "product", currentProduct.ID, btnSignForNotifies.ClientID, pnlNotify.ClientID));
            }

            if (lblSendSuggestion.Visible == true)
            {
                if (hlSuggestionUser.Visible == true)
                {
                    lblSendSuggestion.Attributes.Add("onclick", string.Format(
                        "ShowSendTypeSuggestion('{0}', 'product', '{1}', '{2}', '{3}', '{4}', '{5}')"
                            , lblSendSuggestion.ClientID, currentProduct.ID, hlSuggestionUser.Attributes["userID"]
                            , ddlSuggestionUsers.ClientID, pnlSendTypeSuggestion.ClientID, pnlSendTypeSuggestionEnd.ClientID));
                }
                else
                {
                    lblSendSuggestion.Attributes.Add("onclick", string.Format(
                        "ShowSendTypeSuggestion('{0}', 'product', '{1}', '{2}', '{3}', '{4}', '{5}')"
                            , lblSendSuggestion.ClientID, currentProduct.ID, "empty"
                            , ddlSuggestionUsers.ClientID, pnlSendTypeSuggestion.ClientID, pnlSendTypeSuggestionEnd.ClientID));
                }
            }

            if (accEdit.Visible == true)
            {
                TextBox tbEditDescription = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "tbAccEditDescription");
                tbEditDescription.Text = currentProduct.description;

                TextBox tbCount = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "tbSymbolsCount");
                tbCount.Text = tbEditDescription.Text.Length.ToString();

                tbEditDescription.Attributes.Add("onKeyUp", string.Format("ShowCharsCountInField('{0}', '{1}', '{2}', '{3}');"
               , tbEditDescription.ClientID, tbCount.ClientID, Configuration.ProductsMinDescriptionLength, Configuration.ProductsMaxDescriptionLength));
                tbEditDescription.Attributes.Add("onBlur", string.Format("ShowCharsCountInField('{0}', '{1}', '{2}', '{3}');"
                    , tbEditDescription.ClientID, tbCount.ClientID, Configuration.ProductsMinDescriptionLength, Configuration.ProductsMaxDescriptionLength));
            }

            divClosePopUpLinks.Attributes.Add("onclick", string.Format("HideElementWithID('{0}','{1}');", pnlPopUpLinks.ClientID, "false"));

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            currentUser = GetCurrentUser(userContext, objectContext);

            currentProduct = GetProductFromParams();   // Checks PRroduct parameter if it`s correct , if no redirrects to error page

            CheckSortByParams();                       // Checks sort by parameters if they are correct

            CheckCommentParams();                      // Checks comments parameters if they are correct

            CheckUser();                               // Shows options to user depending on type and roles

            ShowInfo();                                // Shows info about the product

            Comments();                                // Methods connected with comments are here

            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);                 // Hides user notification panel
        }


        private void ShowProductInfo()
        {
            BusinessCategory businessCategory = new BusinessCategory();
            BusinessCompany businessCompany = new BusinessCompany();
            BusinessProduct businessProduct = new BusinessProduct();
            ImageTools imageTools = new ImageTools();
            BusinessUser businessUser = new BusinessUser();
            BusinessRating businessRating = new BusinessRating();
            BusinessAlternativeNames bAN = new BusinessAlternativeNames();

            Title = string.Format("{0} {1}", currentProduct.name, GetLocalResourceObject("title"));

            if (!currentProduct.CreatedByReference.IsLoaded)
            {
                currentProduct.CreatedByReference.Load();
            }

            /////////
            phAddedBy.Controls.Clear();
            phAddedBy.Controls.Add(lblAddedBy);
            if (businessUser.IsFromUserTeam(Tools.GetUserFromUserDatabase(userContext, currentProduct.CreatedBy)))
            {
                phAddedBy.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                    (Tools.GetUserFromUserDatabase(userContext, currentProduct.CreatedBy)));
            }
            else
            {
                Label admLabel = CommonCode.UiTools.GetAdminLabel(Tools.GetUserFromUserDatabase(userContext, currentProduct.CreatedBy).username);
                phAddedBy.Controls.Add(admLabel);
            }
            phAddedBy.Controls.Add(lblAddedOn);
            lblAddedOn.Text =
                string.Format(" , {0} {1}",
                GetLocalResourceObject("AddedOn"),
                CommonCode.UiTools.DateTimeToLocalShortDateString(currentProduct.dateCreated));
            ///////////

            phAddedAndModified.Controls.Clear();
            phAddedAndModified.Controls.Add(lblLastModifiedBy);
            if (!currentProduct.LastModifiedByReference.IsLoaded)
            {
                currentProduct.LastModifiedByReference.Load();
            }

            if (businessUser.IsFromUserTeam(Tools.GetUserFromUserDatabase(userContext, currentProduct.LastModifiedBy)))
            {
                phAddedAndModified.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                    (Tools.GetUserFromUserDatabase(userContext, currentProduct.LastModifiedBy)));
            }
            else
            {
                Label admLabel = CommonCode.UiTools.GetAdminLabel(Tools.GetUserFromUserDatabase(userContext, currentProduct.LastModifiedBy).username);
                phAddedAndModified.Controls.Add(admLabel);
            }

            if (currentProduct.dateCreated == currentProduct.lastModified)
            {
                lblLastModified.Text = string.Empty;
            }
            else
            {
                lblLastModified.Text = string.Format(" , {0} {1}", GetLocalResourceObject("AddedOn")
                  , CommonCode.UiTools.DateTimeToLocalShortDateString(currentProduct.lastModified));
            }
            phAddedAndModified.Controls.Add(lblLastModified);

            ShowMainImage(imageTools, currentProduct);  // shows main product`s image

            hlProductName.Text = currentProduct.name;
            hlProductName.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", currentProduct.ID));

            phSite.Controls.Clear();
            Label site = new Label();
            site.CssClass = "searchPageComments";
            site.Text = GetLocalResourceObject("Website").ToString();
            phSite.Controls.Add(site);
            if (!string.IsNullOrEmpty(currentProduct.website))
            {
                HyperLink siteLink = new HyperLink();
                siteLink.NavigateUrl = currentProduct.website;
                siteLink.Text = Tools.TrimString(currentProduct.website, 60, false, true);
                siteLink.Target = "_blank";
                phSite.Controls.Add(siteLink);
            }
            else
            {
                site.Text = GetLocalResourceObject("NoWebSite").ToString();
            }

            CommonCode.UiTools.AddShareStuff(lblShare, Page, Title);

            if (string.IsNullOrEmpty(currentProduct.description))
            {
                pnlProdDescription.Visible = false;
            }
            else
            {
                lblPDescription.Text = Tools.GetFormattedTextFromDB(currentProduct.description);
                pnlProdDescription.Visible = true;
            }


            Company currCompany = businessProduct.GetProductCompany(objectContext, currentProduct);

            phCompany.Controls.Clear();
            Label lblComp = new Label();
            lblComp.Text = string.Format("{0} : ", Tools.GetStringWithCapital
                (GetGlobalResourceObject("SiteResources", "CompanyName").ToString()));
            phCompany.Controls.Add(lblComp);
            lblComp.CssClass = "searchPageComments";
            if (businessCompany.IsOther(objectContext, currCompany.ID))
            {
                Label Company = new Label();
                Company.Text = currCompany.name;

                phCompany.Controls.Add(Company);
            }
            else
            {
                string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);

                HyperLink compLink = CommonCode.UiTools.GetHyperLink("GoToCompany",
                    GetUrlWithVariant("Company.aspx?Company=" + currCompany.ID), currCompany.name);
                compLink.ID = string.Format("prod{0}comp{1}lnk", currentProduct.ID, currCompany.ID);

                compLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}{2}','{3}')"
                    , currCompany.ID, contPageId, compLink.ClientID, pnlPopUp.ClientID));
                compLink.Attributes.Add("onmouseout", "HideData()");

                phCompany.Controls.Add(compLink);
            }

            List<AlternativeProductName> altNames = bAN.GetVisibleAlternativeProductNames(objectContext, currentProduct);
            if (altNames != null && altNames.Count > 0)
            {
                lblAlternativeNames.Visible = true;

                StringBuilder alternativeNames = new StringBuilder();
                alternativeNames.Append("<span class=\"searchPageComments\">");
                alternativeNames.Append(GetLocalResourceObject("AlternativeNames").ToString());
                alternativeNames.Append("</span>");
                alternativeNames.Append("<span class=\"darkOrange\">");

                foreach (AlternativeProductName name in altNames)
                {
                    alternativeNames.Append(string.Format(" {0};", name.name));
                }
                alternativeNames.Append("</span><br />");

                lblAlternativeNames.Text = alternativeNames.ToString();
            }
            else
            {
                lblAlternativeNames.Visible = false;
            }

            SetCategoryNameWithLink();  // Category Name

            lblProdRating.Text = string.Format("{0} {1}", GetLocalResourceObject("Rating"), businessRating.GetProductRating(currentProduct));

            if (currentProduct.usersRated > 0)
            {
                lblRatingComa.Visible = true;
                lblUsersRated.Visible = true;
                lblUsersRated.Text = string.Format("{0} {1}", GetLocalResourceObject("UsersRated"), currentProduct.usersRated.ToString());
            }
            else
            {
                lblRatingComa.Visible = false;
                lblUsersRated.Visible = false;
            }

            lblComments.Text = string.Format("{0} {1}", GetLocalResourceObject("Comments"), currentProduct.comments.ToString());

            hlCommsFastLink.NavigateUrl = string.Format("{0}#opinions", CurrProductUrl());

            hlForum.NavigateUrl = GetUrlWithVariant(string.Format("Forum.aspx?Product={0}", currentProduct.ID));
        }

        private void ShowInfo()
        {
            ShowProductInfo();

            ShowAccordionsInfo();

            if (IsPostBack == false)
            {
                tbName.Text = GetLocalResourceObject("Anonymous").ToString();
            }

            lblCCommenter.Text = "";
            Label accCharNewName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblAccCharNewName");
            accCharNewName.Text = "";

            Label checkCharName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblCheckCharName");
            checkCharName.Text = "";

            Label prodName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditName, "lblCheckProductNewName");
            prodName.Text = "";

            ShowNotification();

            if (pnlError.Visible == true)
            {
                pnlError.Visible = false;
            }

            ShowAdvertisement();

            FillPublicShownProductEditors();
            ShowLinks();

            SetLocalText();
        }

        private void ShowLinks()
        {
            phLinks.Controls.Clear();

            BusinessUser bUser = new BusinessUser();
            BusinessProductLink bpLinks = new BusinessProductLink();
            BusinessReport bReport = new BusinessReport();

            List<ProductLink> links = bpLinks.GetProductLinks(objectContext, currentProduct, true);

            User linkUser = null;
            string error = string.Empty;
            bool canAdd = bpLinks.CanUserAddLink(objectContext, userContext, currentProduct, currentUser, false, out error);
            bool canReport = false;
            bool canModify = false;

            if (currentUser != null)
            {
                canModify = bpLinks.CanUserModifyProductLinks(objectContext, userContext, currentProduct, currentUser);
                canReport = bUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate);
            }

            if (canModify == true && bUser.IsFromAdminTeam(currentUser))
            {
                tblDelProdLinkWarn.Visible = true;
            }
            else
            {
                tblDelProdLinkWarn.Visible = false;
            }

            if (canAdd == true)
            {
                lblAddProdLink.Visible = true;

                lblAddProdLink.Text = GetGlobalResourceObject("SiteResources", "Add").ToString();

                lblAddProdLink.Attributes.Add("onclick", string.Format("ShowAddProductLinkData('{0}','{1}');"
                    , lblAddProdLink.ClientID, pnlActionReport.ClientID));

                btnAddProductLink.Attributes.Add("onclick", string.Format("AddProductLink('{0}','{1}','{2}');"
                    , currentProduct.ID, tbAddProdLinkUrl.ClientID, tbAddProdLinkDescription.ClientID));
            }
            else
            {
                lblAddProdLink.Visible = false;
            }

            lblLinks.Text = string.Format("{0} ({1})", GetLocalResourceObject("links"), links.Count);

            if (links != null && links.Count > 0)
            {

                // set width 
                // set height if links are more than  ??

                pnlPopUpLinks.Width = Unit.Pixel(450);
                lblLinks.Attributes.Add("onclick", string.Format("ShowProductLinks('{0}','{1}','{2}','{3}');"
                    , lblLinks.ClientID, pnlPopUpLinks.ClientID, -480, -100));

                if (links.Count > 4) //10
                {
                    pnlPopUpLinks.Height = Unit.Pixel(365); //450
                    pnlPopUpLinks.ScrollBars = ScrollBars.Auto;
                }

                string linkName = string.Empty;

                foreach (ProductLink link in links)
                {
                    if (!link.UserReference.IsLoaded)
                    {
                        link.UserReference.Load();
                    }

                    Panel pnlLink = new Panel();
                    phLinks.Controls.Add(pnlLink);
                    pnlLink.CssClass = "pnlProdLink";

                    HyperLink hlLink = new HyperLink();
                    pnlLink.Controls.Add(hlLink);

                    hlLink.Target = "_blank";
                    hlLink.NavigateUrl = link.link;

                    linkName = link.link;
                    if (linkName.StartsWith("http://") || linkName.StartsWith("https://"))
                    {
                        linkName = linkName.Replace("http://", "");
                        linkName = linkName.Replace("https://", "");
                    }

                    if (linkName.Length > 42)
                    {
                        hlLink.Text = Tools.TrimString(linkName, 42, false, true);
                        hlLink.ToolTip = link.link;
                    }
                    else
                    {
                        hlLink.Text = linkName;
                    }

                    pnlLink.Controls.Add(CommonCode.UiTools.GetNewLineControl());

                    Label lblDescr = new Label();
                    pnlLink.Controls.Add(lblDescr);
                    lblDescr.Text = Tools.GetFormattedTextFromDB(link.description);
                    lblDescr.CssClass = "topicsTextStyleSmall";

                    Panel optPnl = new Panel();
                    pnlLink.Controls.Add(optPnl);

                    optPnl.CssClass = "clearfix";

                    linkUser = bUser.GetWithoutVisible(userContext, link.User.ID, true);

                    if (bUser.IsFromAdminTeam(linkUser) == true)
                    {
                        Label admLabel = CommonCode.UiTools.GetAdminLabel(linkUser.username);
                        optPnl.Controls.Add(admLabel);
                    }
                    else
                    {
                        HyperLink hlUser = CommonCode.UiTools.GetUserHyperLink(linkUser);
                        optPnl.Controls.Add(hlUser);
                    }

                    // EDIT

                    Panel pnlRight = new Panel();
                    optPnl.Controls.Add(pnlRight);

                    pnlRight.CssClass = "floatRightNoMrg";

                    if (canModify == true)
                    {
                        string modifDescr = Tools.EscapeNewLinesInString(link.description);

                        Label lblModify = new Label();
                        pnlRight.Controls.Add(lblModify);

                        lblModify.ID = string.Format("modifLink{0}", link.ID);
                        lblModify.Text = GetLocalResourceObject("modify").ToString();
                        lblModify.CssClass = "markLbl";

                        lblModify.Attributes.Add("onclick", string.Format("ShowModifyLinkData('{0}','{1}','{2}','{3}')"
                            , lblModify.ClientID, pnlActionReport.ClientID, link.ID, modifDescr));
                    }

                    if (canReport == true)
                    {
                        // violation
                        Label lblViolation = new Label();
                        pnlRight.Controls.Add(lblViolation);

                        lblViolation.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, "5px");

                        lblViolation.ID = string.Format("markViolLink{0}", link.ID);
                        lblViolation.Text = GetGlobalResourceObject("SiteResources", "violation").ToString();
                        lblViolation.ToolTip = GetLocalResourceObject("ProdLinkViolationTooltip").ToString();
                        lblViolation.CssClass = "markLbl";
                        lblViolation.Attributes.Add("onclick", string.Format("ReportProductLinkAsSpam('{0}','{1}','{2}')"
                            , pnlActionReport.ClientID, lblViolation.ClientID, link.ID));
                    }

                }

            }
            else
            {
                // set width
                pnlPopUpLinks.Width = Unit.Pixel(300);
                lblLinks.Attributes.Add("onclick", string.Format("ShowProductLinks('{0}','{1}','{2}','{3}');"
                    , lblLinks.ClientID, pnlPopUpLinks.ClientID, -330, 10));

                Panel noLinksPnl = new Panel();
                phLinks.Controls.Add(noLinksPnl);
                noLinksPnl.CssClass = "sectionTextHeader";

                Label lblNoEditors = new Label();
                noLinksPnl.Controls.Add(lblNoEditors);
                lblNoEditors.Text = GetLocalResourceObject("NoLinks").ToString();
            }

        }

        private void ShowAccordionsInfo()
        {
            BusinessProduct businessProduct = new BusinessProduct();
            ImageTools imageTools = new ImageTools();

            FillVariants();

            if (businessProduct.CountProductCharacteristics(objectContext, currentProduct) < 1)
            {
                apCharacteristics.Visible = false;
            }
            else
            {
                lblCharacteristics.Text = GetLocalResourceObject("Characteristics").ToString();
                tblChars.Visible = true;
                pnlShowChars.CssClass = "accordionHeaders";
                ShowCharacteristics();

                apCharacteristics.Visible = true;
            }

            if (imageTools.GetProductImagesCount(objectContext, currentProduct) < 1)
            {
                lblGallery.Text = GetLocalResourceObject("NoGallery").ToString();
                tblGallery.Visible = false;
                pnlShowGallery.CssClass = "accordionHeadersNoCursor";
            }
            else
            {
                lblGallery.Text = GetLocalResourceObject("Gallery").ToString();
                tblGallery.Visible = true;
                pnlShowGallery.CssClass = "accordionHeaders";
                FillGalleryTable();
            }

            CheckIfAccordionsInfoShouldBeVisible();

        }

        private void CheckIfAccordionsInfoShouldBeVisible()
        {
            if (apGallery.Visible == true || apCharacteristics.Visible == true || apVariants.Visible == true)
            {
                accInformation.Visible = true;
                hrBetweenAddEditAndInfo.Visible = true;
            }
            else
            {
                accInformation.Visible = false;
                hrBetweenAddEditAndInfo.Visible = false;
            }
        }

        private void SetLocalText()
        {

            lblAddedBy.Text = GetLocalResourceObject("AddedBy").ToString();
            lblLastModifiedBy.Text = GetLocalResourceObject("LastModifiedBy").ToString() + "&nbsp;";

            lblProductEditors.Text = GetLocalResourceObject("Editors").ToString();

            hlMoreInfo.Text = Tools.GetStringWithCapital(GetLocalResourceObject("ClickHere").ToString());
            lblMoreInfo.Text = GetLocalResourceObject("InfoOnEditing").ToString();

            hlForum.Text = GetGlobalResourceObject("SiteResources", "Forum").ToString();


            if (pnlEdit.Visible == true)
            {
                ////  EDIT //////
                hlMoreInfo.NavigateUrl = GetUrlWithVariant("Rules.aspx#rulesaeprod");

                lblAccEdit.Text = GetLocalResourceObject("Edit").ToString();

                Label editHDescr = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "lblEditDescription");
                editHDescr.Text = GetLocalResourceObject("EditDescription").ToString();
                Label editDescr = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "lblAccEditDescription");
                editDescr.Text = GetLocalResourceObject("ChangeDescription").ToString();
                Button editDescription = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "dbEditDescription");
                editDescription.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
                Label SymbolsCount = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "lblSymbolsCount");
                SymbolsCount.Text = GetLocalResourceObject("textLength").ToString();

                Label editChars = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblEditCharacteristics");
                editChars.Text = GetLocalResourceObject("EditCharacteristics").ToString();
                Label chooseChar = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblChooseChar");
                chooseChar.Text = GetLocalResourceObject("ChooseCharacteristic").ToString();
                Label newCharName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblNewCharName");
                newCharName.Text = GetLocalResourceObject("EditCharTopic").ToString();
                Label charDescription = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblCharDescrtiption");
                charDescription.Text = GetLocalResourceObject("ChangeDescription").ToString();
                Button btnEditChar = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "btnEditCharSave");
                btnEditCharSave.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
                Button btnDelCharr = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "btnDeleteCharacteristic");
                btnDelCharr.Text = GetGlobalResourceObject("SiteResources", "Delete").ToString();

                Label editSite = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditWebsite, "lblEditWebSite");
                editSite.Text = GetLocalResourceObject("EditWebsite").ToString();
                Label newSite = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditWebsite, "lblAccNewWebSite");
                newSite.Text = GetLocalResourceObject("NewWebsite").ToString();
                Button btnUpdateSite = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditWebsite, "btnUpdateWebSite");
                btnUpdateSite.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();

                Label editVariant = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "lblEditVariant");
                editVariant.Text = GetLocalResourceObject("EditVariant").ToString();
                Label chooseVariant = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "lblEditVariantChoose");
                chooseVariant.Text = GetLocalResourceObject("Variant").ToString();
                Label variantDescription = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "lblEditVariantDescription");
                variantDescription.Text = GetLocalResourceObject("ChangeDescription").ToString();
                Button btnEditVariant = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "btnEditVariant");
                btnEditVariant.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
                Button btnDelVariant = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "btnDeleteVariant");
                btnDelVariant.Text = GetGlobalResourceObject("SiteResources", "Delete").ToString();

                Label editSubVariant = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "lblEditSubVariant");
                editSubVariant.Text = GetLocalResourceObject("EditSubVariant").ToString();
                Label chooseSubVariant = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "lblEditSubVariantName");
                chooseSubVariant.Text = GetLocalResourceObject("SubVariant").ToString();
                Label editSubVariantDesc = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "lblEditSubVariantDescription");
                editSubVariantDesc.Text = GetLocalResourceObject("ChangeDescription").ToString();
                Button btnEditSubVariant = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "btnEditSubVariant");
                btnEditSubVariant.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
                Button btnDelSubVariant = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "btnDeleteSubVariant");
                btnDelSubVariant.Text = GetGlobalResourceObject("SiteResources", "Delete").ToString();

                Label removeShowAlternativeName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "lblShowRemoveAlternativeNames");
                removeShowAlternativeName.Text = GetLocalResourceObject("RemoveAlternativeNames").ToString();
                Label removeAlternativeName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "lblRemoveAlternativeNames");
                removeAlternativeName.Text = GetLocalResourceObject("AlternativeNames").ToString();
                Button btnRemoveAlternativeNames = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "btnRemoveAlternativeNames");
                btnRemoveAlternativeNames.Text = GetGlobalResourceObject("SiteResources", "Delete").ToString();

                if (apEditName.Visible == true)
                {
                    Label editProdName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditName, "lblEditName");
                    editProdName.Text = GetLocalResourceObject("EditName").ToString();
                    Label newProdName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditName, "lblAccEditName");
                    newProdName.Text = GetLocalResourceObject("NewProdName").ToString();
                    Button btnEditName = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditName, "btnUpdateProductName");
                    btnEditName.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
                }

                if (apEditCompany.Visible == true)
                {
                    Label lblEditCompHeader = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "lblEditCompany");
                    lblEditCompHeader.Text = GetLocalResourceObject("editCompany").ToString();

                    Label lblEditCompInfo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "lblEditorChnCompInfo");
                    lblEditCompInfo.Text = GetLocalResourceObject("editCompanyInfo").ToString();

                    Label lblCompanies = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "lblEditorNewCompany");
                    lblCompanies.Text = GetLocalResourceObject("newCompany").ToString();

                    Button btnEditComp = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "dbEditorUpdateCompany");
                    btnEditComp.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
                }

                if (apEditCategory.Visible == true)
                {
                    Label lblEditCatHeader = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "lblEditCategoryHeader");
                    lblEditCatHeader.Text = GetLocalResourceObject("changeCategory").ToString();

                    Label lblChooseCat = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "lblEditorChooseCat");
                    lblChooseCat.Text = GetLocalResourceObject("chooseCategory").ToString();
                    Button btnChangeCat = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "btnEditorChngCatWhenCompOther");
                    btnChangeCat.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();

                    if (!currentProduct.CompanyReference.IsLoaded)
                    {
                        currentProduct.CompanyReference.Load();
                    }

                    Label lblChooseCatInfo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "lblSubEditorChngCatInfo");
                    lblChooseCatInfo.Text = string.Format("{0} {1} {2}", GetLocalResourceObject("chooseCategoryInfo")
                        , currentProduct.Company.name, GetLocalResourceObject("chooseCategoryInfo2"));

                    Label lblChooseCat2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "lblEditorChooseCat2");
                    lblChooseCat2.Text = GetLocalResourceObject("chooseCategory").ToString();
                    Button btnChangeCat2 = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "btnEditorChngCat");
                    btnChangeCat2.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
                }

                /////////////// ADD /////////

                lblAddToProduct.Text = GetLocalResourceObject("Add").ToString();

                Label uploadImg = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "lblUploadImage");
                uploadImg.Text = GetLocalResourceObject("UploadImage").ToString();
                Label imgDescription = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "lblAddImageDescription");
                imgDescription.Text = GetLocalResourceObject("AddDescr").ToString();
                CheckBox imageMain = (CheckBox)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "cbImageMain");
                imageMain.Text = GetLocalResourceObject("main").ToString();
                Button btnUploadImg = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "btnUpload");
                btnUploadImg.Text = GetLocalResourceObject("Upload").ToString();
                lblUploadingImg.Text = GetLocalResourceObject("UploadingImg").ToString();

                Label chooseImg = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "lblChooseImg");
                chooseImg.Text = GetLocalResourceObject("chooseImage").ToString();

                HyperLink seeImg1 = (HyperLink)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "hlClickForMinImage");
                seeImg1.Text = GetLocalResourceObject("ClickHere").ToString();
                Label seeImg2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "lblClickForMinImage2");
                seeImg2.Text = GetLocalResourceObject("seeImgMinSize").ToString();



                Label addChar = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblAddCharacteristic");
                addChar.Text = GetLocalResourceObject("AddCharacteristic").ToString();
                Label charName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblAddCharName");
                charName.Text = GetLocalResourceObject("CharTopic").ToString();
                Label AddcharDescription = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblAddCharDescription");
                AddcharDescription.Text = GetLocalResourceObject("AddDescr").ToString();
                Button btnAddChar = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "btnAddChar");
                btnAddChar.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();

                Label addVariant = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddVariant, "lblAddVariant");
                addVariant.Text = GetLocalResourceObject("AddVariant").ToString();
                Label addVarName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddVariant, "lblAddVariantName");
                addVarName.Text = GetLocalResourceObject("Name").ToString();
                Label addVarDescr = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddVariant, "lblAddVariantDescription");
                addVarDescr.Text = GetLocalResourceObject("AddDescr").ToString();
                Button btnAddVar = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apAddVariant, "btnAddVariant");
                btnAddVariant.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();

                Label addSubVariant = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "lblAddSubVariant");
                addSubVariant.Text = GetLocalResourceObject("AddSubVariant").ToString();
                Label addSubVariantVariant = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "lblAddSubVariantVariant");
                addSubVariantVariant.Text = GetLocalResourceObject("Variant").ToString();
                Label addSubVariantName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "lblAddSubVariantName");
                addSubVariantName.Text = GetLocalResourceObject("Name").ToString();
                Label addSubVariantDescr = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "lblAddSubVariantDescription");
                addSubVariantDescr.Text = GetLocalResourceObject("AddDescr").ToString();
                Button btnAddSubVar = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "btnAddSubVariant");
                btnAddSubVar.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();


                Label addShowAddAlternativeName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddAlternativeName, "lblAddAlternativeNameShow");
                addShowAddAlternativeName.Text = GetLocalResourceObject("AddAlternativeName").ToString();
                Label addAddAlternativeName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddAlternativeName, "lblAddAlternativeNames");
                addAddAlternativeName.Text = GetLocalResourceObject("AlternativeName").ToString();
                Label addAddAlternativeNameInfo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddAlternativeName, "lblAddAlternativeNamesInfo");
                addAddAlternativeNameInfo.Text = string.Format("{0} {1} {2}<br />{3}<br />{4}", GetLocalResourceObject("AlternativeNamesInfo")
                    , Configuration.ProductsMaxAlternativeNames, GetLocalResourceObject("AlternativeNamesInfo2")
                    , GetLocalResourceObject("AlternativeNamesInfo3"), GetLocalResourceObject("AlternativeNamesInfo4"));
                Button btnAddAlternativeName = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apAddAlternativeName, "btnAddAlternativeName");
                btnAddAlternativeName.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();

            }
            ///

            lblSendSuggestion.Text = GetLocalResourceObject("SendSuggestion").ToString();

            lblSendReport.Text = GetGlobalResourceObject("SiteResources", "lblWriteReport").ToString();
            lblReportIrregularity.Text = GetGlobalResourceObject("SiteResources", "reportIrregularity").ToString();
            btnSendReport.Value = GetGlobalResourceObject("SiteResources", "Send").ToString();
            btnHideRepData.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();

            btnSignForNotifies.Value = GetLocalResourceObject("Notify").ToString();
            btnSignForNotifies.Attributes.Add("title", GetLocalResourceObject("SignNotifiesTooltip").ToString());
            btnTakeAction.Text = GetLocalResourceObject("TakeRole").ToString();

            lblCommentAction.Text = GetLocalResourceObject("WriteComment").ToString();
            lblCName.Text = GetLocalResourceObject("Name").ToString();
            lblCC.Text = GetLocalResourceObject("AboutCharacteristic").ToString();
            lblOr.Text = GetLocalResourceObject("orAbout").ToString();
            lblAboutVariant.Text = GetLocalResourceObject("AboutVariant").ToString();
            btnAddComment.Text = GetLocalResourceObject("AddComment").ToString();

            hlAllComments.Text = GetLocalResourceObject("AllComments").ToString();
            lblSortBy.Text = GetLocalResourceObject("SortBy").ToString();
            DateLink.Text = GetLocalResourceObject("Date").ToString();
            hlSortRating.Text = GetLocalResourceObject("SortByRating").ToString();
            hlSortByNoAbout.Text = GetLocalResourceObject("SortByNoAbout").ToString();

            lblMessageTo.Text = GetLocalResourceObject("MessageTo").ToString();
            lblMsgSubject.Text = GetLocalResourceObject("MsgSubject").ToString();
            lblCbSaveInSent.Text = GetLocalResourceObject("SaveInSent").ToString();
            btnSendMsgToUser.Value = GetGlobalResourceObject("SiteResources", "Send").ToString();
            btnCancelSendMsg.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();

            lblReplyToUser.Text = GetLocalResourceObject("ReplyTo").ToString();
            btnReplyToComment.Value = GetGlobalResourceObject("SiteResources", "Send").ToString();
            btnCancelReply.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();

            lblSuggestionInfo.Text = GetLocalResourceObject("SuggestionInfo").ToString();
            lblSuggestionTo.Text = GetLocalResourceObject("SugegstionTo").ToString();
            btnSendTypeSuggestion.Value = GetGlobalResourceObject("SiteResources", "Send").ToString();
            btnCancelSuggestion.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();

            tbCaptchaMsg_TextBoxWatermarkExtender.WatermarkText = GetLocalResourceObject("captchaLetters").ToString();


            if (lblAddProdLink.Visible == true)
            {
                tbAddProdLinkDescription_TextBoxWatermarkExtender.WatermarkText =
                    GetGlobalResourceObject("SiteResources", "addProdLinkDescription").ToString();

                //
                lblAddProductLink.Text = GetLocalResourceObject("AddProductLink").ToString();

                lblAddLinkRules.Text = GetLocalResourceObject("addProdLinkRules1").ToString();
                hlAddLinkRules.Text = GetLocalResourceObject("hlAddProdLinkRules").ToString();
                hlAddLinkRules.NavigateUrl = GetUrlWithVariant("Rules.aspx#rulesProdLinks").ToString();
                lblAddLinkRules2.Text = GetLocalResourceObject("addProdLinkRules2").ToString();

                lblProdLink.Text = GetLocalResourceObject("link").ToString();

                btnAddProductLink.Value = GetGlobalResourceObject("SiteResources", "Add").ToString();
                btnHideAddProdLinkData.Value = GetGlobalResourceObject("SiteResources", "Cancel").ToString();
                //

            }

            lblModifyProductLink.Text = GetLocalResourceObject("modifyLink").ToString();
            btnModifyProductLink.Value = GetGlobalResourceObject("SiteResources", "Modify").ToString();
            btnDeleteProductLink.Value = GetGlobalResourceObject("SiteResources", "delete").ToString();
            btnDeleteProductLinkWarn.Value = GetLocalResourceObject("deleteWithWarn").ToString();
            btnHideModifyProdLinkData.Value = GetGlobalResourceObject("SiteResources", "Cancel").ToString();


        }

        /// <summary>
        /// Methods connected with comments are here
        /// </summary>
        public void Comments()
        {
            WriteComment();                            // Part for writing/editing comments

            CommentsPageAndNumber();                   // Shows comments page and numbers

            SortCommentsBy(false, false);              // Shows sort by date, rating and characteristic options

            DateTime now = DateTime.UtcNow;

            ShowComments();                            // Shows comments

            TimeSpan span = DateTime.UtcNow - now;
            MasterPage master = this.Master as MasterPage;
            master.ShowInfo("Comms : " + span.TotalSeconds.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
        }


        private void CheckSortByParams()
        {
            String strRating = Request.Params["rating"];
            String strCharId = Request.Params["char"];
            String strDate = Request.Params["date"];
            String strVarId = Request.Params["variant"];
            String strSubVarId = Request.Params["subvariant"];
            String strNoAbout = Request.Params["noabout"];

            if (!string.IsNullOrEmpty(strRating) && !string.IsNullOrEmpty(strDate))
            {
                RedirectToCurrProductPage();
            }

            if (!string.IsNullOrEmpty(strNoAbout))
            {
                if (!string.IsNullOrEmpty(strCharId) || !string.IsNullOrEmpty(strVarId) || !string.IsNullOrEmpty(strSubVarId))
                {
                    RedirectToCurrProductPage();
                }
            }

            if (!string.IsNullOrEmpty(strCharId))
            {
                if (!string.IsNullOrEmpty(strSubVarId) || !string.IsNullOrEmpty(strVarId))
                {
                    RedirectToCurrProductPage();
                }
            }
            else if (!string.IsNullOrEmpty(strSubVarId))
            {
                if (!string.IsNullOrEmpty(strCharId) || !string.IsNullOrEmpty(strVarId))
                {
                    RedirectToCurrProductPage();
                }
            }
            else if (!string.IsNullOrEmpty(strVarId))
            {
                if (!string.IsNullOrEmpty(strCharId) || !string.IsNullOrEmpty(strSubVarId))
                {
                    RedirectToCurrProductPage();
                }
            }

            long typeID = 0;

            // sort by no about
            if (!string.IsNullOrEmpty(strNoAbout))
            {
                if (strNoAbout == "true")
                {
                    SortByNoAbout = true;
                }
                else
                {
                    RedirectToCurrProductPage();
                }
            }

            // sort by characteristic
            if (!string.IsNullOrEmpty(strCharId))
            {

                if (long.TryParse(strCharId, out typeID))
                {
                    BusinessProduct businessProduct = new BusinessProduct();
                    ProductCharacteristics currChar = businessProduct.GetCharacteristic(objectContext, typeID);
                    if (currChar == null)
                    {
                        RedirectToCurrProductPage();
                    }

                    currChar.ProductReference.Load();

                    if (currChar.Product != currentProduct)
                    {
                        RedirectToCurrProductPage();
                    }

                    SortByChar = currChar.ID;

                }
                else
                {
                    RedirectToCurrProductPage();
                }
            }

            // sort by variant
            if (!string.IsNullOrEmpty(strVarId))
            {
                if (long.TryParse(strVarId, out typeID))
                {
                    BusinessProductVariant bpVariant = new BusinessProductVariant();
                    ProductVariant currVariant = bpVariant.Get(objectContext, currentProduct, typeID, true, false);
                    if (currVariant == null)
                    {
                        RedirectToCurrProductPage();
                    }

                    SortByVariant = currVariant.ID;
                }
                else
                {
                    RedirectToCurrProductPage();
                }
            }

            // sort by subvariant
            if (!string.IsNullOrEmpty(strSubVarId))
            {
                if (long.TryParse(strSubVarId, out typeID))
                {
                    BusinessProductVariant bpVariant = new BusinessProductVariant();
                    ProductSubVariant currVariant = bpVariant.GetSubVariant(objectContext, currentProduct, typeID, true, false);
                    if (currVariant == null)
                    {
                        RedirectToCurrProductPage();
                    }

                    SortBySubVariant = currVariant.ID;
                }
                else
                {
                    RedirectToCurrProductPage();
                }
            }

            // sort by date
            if (!string.IsNullOrEmpty(strDate))
            {

                if (strDate == "desc")
                {
                    SortByDateDesc = true;
                }
                else if (strDate == "asc")
                {
                    SortByDateDesc = false;
                }
                else
                {
                    RedirectToCurrProductPage();
                }
            }

            // sort by rating
            if (!string.IsNullOrEmpty(strRating))
            {
                if (strRating == "desc")
                {
                    SortByRating = SortOptions.Descending;  // 1;
                }
                else if (strRating == "asc")
                {
                    SortByRating = SortOptions.Ascending;  // 2;
                }
                else
                {
                    RedirectToCurrProductPage();
                }
            }
            else
            {
                SortByRating = 0;
            }

        }

        private void RedirectToCurrProductPage()
        {
            RedirectToOtherUrl(string.Format("Product.aspx?Product={0}", currentProduct.ID));
        }

        private void SortCommentsBy(bool fillDdlSortByChars, bool fillDdlSortByVariant)
        {
            if (IsPostBack == false || fillDdlSortByChars == true || fillDdlSortByVariant == true)
            {
                if (IsPostBack == false || (fillDdlSortByChars == true && fillDdlSortByVariant == true))
                {
                    FillDdlSortByChars();
                    FillDdlSortByVariant();
                }
                else if (fillDdlSortByChars == true)
                {
                    FillDdlSortByChars();
                }
                else if (fillDdlSortByVariant == true)
                {
                    FillDdlSortByVariant();
                }
            }

            if (currentProduct.comments > 2)  // 5
            {
                pnlSortComments.Visible = true;

                DateLink.Visible = true;
                hlSortRating.Visible = true;
                lblSortBy.Visible = true;


                if (ddlSortByCHar.Visible == true || ddlSortByVariant.Visible == true)
                {
                    hlSortByNoAbout.Visible = true;
                    hlSortByNoAbout.NavigateUrl = GetUrlWithVariant(string.Format("{0}&noabout=true", CurrProductUrl()));
                }
                else
                {
                    hlSortByNoAbout.Visible = false;
                }

                if (SortByChar > 0 || SortByVariant > 0 || SortBySubVariant > 0 || SortByNoAbout == true)
                {
                    hlAllComments.Visible = true;
                    hlAllComments.NavigateUrl = hlProductName.NavigateUrl;
                }
                else
                {
                    hlAllComments.Visible = false;
                }

                String URL = Request.Url.ToString();

                // Date Link 
                String dateURL = URL;
                if (dateURL.Contains("&rating=asc"))
                {
                    dateURL = dateURL.Replace("&rating=asc", string.Empty);
                }
                else if (dateURL.Contains("&rating=desc"))
                {
                    dateURL = dateURL.Replace("&rating=desc", string.Empty);
                }

                if (SortByDateDesc)
                {
                    if (dateURL.Contains("date=desc"))
                    {
                        dateURL = dateURL.Replace("date=desc", "date=asc");
                        DateLink.NavigateUrl = dateURL;
                    }
                    else
                    {
                        DateLink.NavigateUrl = string.Format("{0}&date=asc", dateURL);
                    }
                }
                else
                {
                    if (dateURL.Contains("date=asc"))
                    {
                        dateURL = dateURL.Replace("date=asc", "date=desc");
                        DateLink.NavigateUrl = dateURL;
                    }
                    else
                    {
                        DateLink.NavigateUrl = string.Format("{0}&date=desc", dateURL);
                    }
                }

                // Rating Link
                String ratingURL = URL;
                if (ratingURL.Contains("&date=asc"))
                {
                    ratingURL = ratingURL.Replace("&date=asc", string.Empty);
                }
                else if (ratingURL.Contains("&date=desc"))
                {
                    ratingURL = ratingURL.Replace("&date=desc", string.Empty);
                }

                switch (SortByRating)
                {
                    case SortOptions.None:  // (0):
                        hlSortRating.NavigateUrl = (string.Format("{0}&rating=desc", ratingURL));
                        break;
                    case SortOptions.Descending:  // (1):
                        hlSortRating.NavigateUrl = ratingURL.Replace("rating=desc", "rating=asc");
                        break;
                    case SortOptions.Ascending:  // (2):
                        hlSortRating.NavigateUrl = ratingURL.Replace("rating=asc", "rating=desc");
                        break;
                    default:
                        throw new CommonCode.UIException(string.Format(
                            "SortByRating = '{0}' is not valid, Product ID = {1}",
                            SortByRating, currentProduct.ID));
                }

                hlSortRating.NavigateUrl = CommonCode.UiUrl.UrlUnRewrite(hlSortRating.NavigateUrl);
                DateLink.NavigateUrl = CommonCode.UiUrl.UrlUnRewrite(DateLink.NavigateUrl);
            }
            else
            {
                pnlSortComments.Visible = false;
            }

        }

        private long GetCharParamId()
        {
            long charId = -1;
            String strChar = Request.Params["char"];
            if (!string.IsNullOrEmpty(strChar))
            {
                long.TryParse(strChar, out charId);
            }
            return charId;
        }

        private void GetVariantParamIdAndType(out string type, out long id)
        {
            id = 0;
            type = string.Empty;

            if (!string.IsNullOrEmpty(Request["variant"]))
            {
                type = "variant";

                long.TryParse(Request["variant"], out id);
            }
            else if (!string.IsNullOrEmpty(Request["subvariant"]))
            {
                type = "subvariant";

                long.TryParse(Request["subvariant"], out id);
            }
        }


        private void FillDdlSortByChars()
        {
            ddlSortByCHar.Items.Clear();

            BusinessProduct businessProduct = new BusinessProduct();
            BusinessComment businessComment = new BusinessComment();

            IEnumerable<ProductCharacteristics> chars = businessProduct.GetAllProductCharacteristics(objectContext, currentProduct.ID, false);

            if (chars.Count<ProductCharacteristics>() > 0)
            {
                ddlSortByCHar.Visible = true;

                if (true)
                {

                    ListItem mainItem = new ListItem();
                    mainItem.Text = GetLocalResourceObject("..characterisic..").ToString();
                    mainItem.Value = "-1";
                    ddlSortByCHar.Items.Add(mainItem);

                    long charID = GetCharParamId();

                    int i = 1;

                    foreach (ProductCharacteristics characteristic in chars)
                    {

                        ListItem newItem = new ListItem();
                        newItem.Text = characteristic.name;
                        newItem.Value = characteristic.ID.ToString();
                        ddlSortByCHar.Items.Add(newItem);

                        if (characteristic.ID == charID)
                        {
                            ddlSortByCHar.SelectedIndex = i;
                        }
                        i++;

                    }
                }

            }
            else
            {
                ddlSortByCHar.Visible = false;
            }

        }

        private void FillDdlSortByVariant()
        {
            ddlSortByVariant.Items.Clear();

            BusinessProductVariant bpVariant = new BusinessProductVariant();
            BusinessComment businessComment = new BusinessComment();

            List<ProductVariant> variants = bpVariant.GetVisibleVariants(objectContext, currentProduct);
            if (variants.Count > 0)
            {
                ddlSortByVariant.Visible = true;

                List<ProductSubVariant> subVariants = new List<ProductSubVariant>();

                ListItem firstItem = new ListItem();
                ddlSortByVariant.Items.Add(firstItem);
                firstItem.Text = GetLocalResourceObject("..variant..").ToString();
                firstItem.Value = "0";


                string type = string.Empty;
                long typeId = 0;
                GetVariantParamIdAndType(out type, out typeId);

                int i = 1;

                foreach (ProductVariant variant in variants)
                {
                    ListItem varItem = new ListItem();
                    ddlSortByVariant.Items.Add(varItem);
                    varItem.Text = variant.name;
                    varItem.Value = string.Format("var{0}", variant.ID);

                    if (type == "variant" && variant.ID == typeId)
                    {
                        ddlSortByVariant.SelectedIndex = i;
                    }

                    variant.ProductSubVariants.Load();
                    subVariants = variant.ProductSubVariants.Where(var => var.visible == true).ToList();
                    if (subVariants.Count > 0)
                    {
                        foreach (ProductSubVariant subVariant in subVariants)
                        {
                            i++;

                            ListItem subVarItem = new ListItem();
                            ddlSortByVariant.Items.Add(subVarItem);
                            subVarItem.Text = string.Format("{0} : {1}", variant.name, subVariant.name);
                            subVarItem.Value = string.Format("sub{0}", subVariant.ID);

                            if (type == "subvariant" && subVariant.ID == typeId)
                            {
                                ddlSortByVariant.SelectedIndex = i;
                            }
                        }
                    }

                    i++;
                }
            }
            else
            {
                ddlSortByVariant.Visible = false;
            }
        }

        protected void ddlSortByCHar_SelectedIndexChanged(object sender, EventArgs e)
        {
            BusinessProduct businessProduct = new BusinessProduct();

            if (ddlSortByCHar.SelectedIndex == 0)
            {
                RedirectToCurrProductPage();
            }

            long charId = -1;
            if (long.TryParse(ddlSortByCHar.SelectedValue, out charId))
            {
                ProductCharacteristics currChar = businessProduct.GetCharacteristic(objectContext, charId);
                String error = "";
                if (currChar == null)
                {
                    error = string.Format("Theres no Product Id = {0} Characteristic ID {1}", currentProduct.ID, charId);
                    throw new CommonCode.UIException(error);
                }

                if (currChar.Product != currentProduct)
                {
                    error = string.Format("Characteristic with id {0} is not from product {1}", charId, currentProduct.ID);
                    throw new BusinessException(error);
                }

                RedirectToOtherUrl(string.Format("Product.aspx?Product={0}&char={1}", currentProduct.ID, currChar.ID));

            }
            else
            {
                throw new CommonCode.UIException(string.Format("Couldnt parse ddlSortByCHar.SelectedValue to long , Product id = {0}", currentProduct.ID));
            }

        }


        private void ShowCharacteristics()
        {
            tblChars.Rows.Clear();

            BusinessProduct businessProduct = new BusinessProduct();
            BusinessUser businessUser = new BusinessUser();

            Boolean showId = false;
            if (currentUser != null)
            {
                if (businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditProducts))
                {
                    showId = true;
                }
            }
            IEnumerable<ProductCharacteristics> productCharecteristics
                = businessProduct.GetAllProductCharacteristics(objectContext, currentProduct.ID, false);
            int charCount = productCharecteristics.Count<ProductCharacteristics>();

            if (charCount > 0)
            {

                int charNum = 0;
                foreach (ProductCharacteristics productCharacteristic in productCharecteristics)
                {
                    charNum++;

                    TableRow newRow = new TableRow();
                    tblChars.Rows.Add(newRow);

                    if (showId)
                    {
                        TableCell idCell = new TableCell();
                        idCell.Width = Unit.Percentage(5);
                        idCell.Text = productCharacteristic.ID.ToString();
                        newRow.Cells.Add(idCell);
                    }

                    TableCell charName = new TableCell();
                    newRow.Cells.Add(charName);

                    charName.HorizontalAlign = HorizontalAlign.Center;

                    Label lblCharName = new Label();
                    charName.Controls.Add(lblCharName);
                    lblCharName.Text = productCharacteristic.name;
                    lblCharName.CssClass = "textHeader";

                    Panel right = new Panel();
                    charName.Controls.Add(right);
                    right.CssClass = "floatRight";
                    right.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingRight, "9px");

                    Label lblComms = new Label();
                    right.Controls.Add(lblComms);
                    lblComms.Text = string.Format("{0} {1}", GetLocalResourceObject("commentsSmall")
                        , productCharacteristic.comments.ToString());
                    lblComms.CssClass = "commentsLarger";

                    TableRow descrRow = new TableRow();
                    tblChars.Rows.Add(descrRow);

                    TableCell charDescr = new TableCell();

                    Panel descrPnl = new Panel();
                    charDescr.Controls.Add(descrPnl);
                    descrPnl.CssClass = "padding5px";

                    Label descriptionLbl = new Label();
                    descriptionLbl.Text = Tools.GetFormattedTextFromDB(productCharacteristic.description);
                    descrPnl.Controls.Add(descriptionLbl);
                    if (charNum < charCount)
                    {
                        charDescr.Controls.Add(CommonCode.UiTools.GetHorisontalFashionLinePanel(true));
                    }
                    descrRow.Cells.Add(charDescr);

                    if (showId)
                    {
                        charDescr.ColumnSpan = 2;
                    }
                }
            }
            else
            {
                TableRow newRoll = new TableRow();
                TableCell noData = new TableCell();
                noData.Text = GetLocalResourceObject("NoEnteredCharacteristics").ToString();
                newRoll.Cells.Add(noData);
                tblChars.Rows.Add(newRoll);
            }
        }

        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            BusinessProduct businessProduct = new BusinessProduct();
            BusinessCompany bCompany = new BusinessCompany();
            BusinessCategory bCategory = new BusinessCategory();

            Category unspecified = bCategory.GetUnspecifiedCategory(objectContext);
            Company other = bCompany.GetOther(objectContext);
            Boolean pnlActionVisible = false;

            if (!currentProduct.CompanyReference.IsLoaded)
            {
                currentProduct.CompanyReference.Load();
            }
            if (!currentProduct.CategoryReference.IsLoaded)
            {
                currentProduct.CategoryReference.Load();
            }

            if (currentUser != null)
            {
                bool canEditAllProducts = false;
                if (businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditProducts))
                {
                    canEditAllProducts = true;
                }

                if (businessUser.CanUserModifyProduct(objectContext, currentProduct.ID, currentUser.ID) || canEditAllProducts == true)
                {
                    accordionAddEdit.Visible = true;

                    // edit menu
                    pnlEdit.Visible = true;
                    if (IsPostBack == false)
                    {
                        CheckAddEditProductData(canEditAllProducts);
                    }

                }

                if (canEditAllProducts == true)  // admin part
                {
                    accAdmin.Visible = true;

                    if (currentProduct.canUserTakeRoleIfNoEditors == true)
                    {
                        lblCanUserTakeRoleIfNoEditors.Text = "Currently users CAN take action to edit this product if there are no editors.";
                    }
                    else
                    {
                        lblCanUserTakeRoleIfNoEditors.Text = "Currently users CAN NOT take action to edit this product if there are no editors.";
                    }

                    if (currentProduct.visible)
                    {
                        lblVisible.Text = string.Format("{0} is VISIBLE", currentProduct.name);
                        btnMakeVisible.Enabled = false;

                        if (currentProduct.Category.ID == unspecified.ID)
                        {
                            btnDeleteProduct.Enabled = false;
                        }
                        else
                        {
                            btnDeleteProduct.Enabled = true;
                        }

                        btnUserRoleProd.Enabled = true;
                    }
                    else
                    {
                        lblVisible.Text = string.Format("{0} is NOT VISIBLE", currentProduct.name);
                        btnDeleteProduct.Enabled = false;
                        btnMakeVisible.Enabled = true;

                        btnUserRoleProd.Enabled = false;
                    }

                    ProductEditorsFill();                       // filling table with product editors
                    FillTblAllCompanyProductsModificators();
                }

                BusinessRating businessRating = new BusinessRating();

                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.AddProducts)
                    && currentProduct.Company.ID != other.ID
                    && bCompany.IfHaveValidCategories(objectContext, currentProduct.Company.ID))
                {
                    hlAddProduct.Text = GetLocalResourceObject("AddProduct").ToString();
                    hlAddProduct.ToolTip = string.Format("{0} {1}", GetLocalResourceObject("AddProductFor"), currentProduct.Company.name);
                    hlAddProduct.NavigateUrl = GetUrlWithVariant(string.Format("AddProduct.aspx?Company={0}", currentProduct.Company.ID));

                    hlAddProduct.Visible = true;
                    pnlActionVisible = true;
                }


                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.RateProducts))
                {
                    if (!businessRating.IsProductRatedByUser(objectContext, currentProduct, currentUser))
                    {
                        lblRateProduct.Text = GetLocalResourceObject("Rate").ToString();
                        lblRateProduct.Visible = true;

                        RateProductItems(ddlRateProduct.SelectedIndex);

                        ddlRateProduct.Visible = true;

                        pnlActionVisible = true;
                    }
                    else
                    {
                        lblRateProduct.Visible = false;
                        ddlRateProduct.Visible = false;
                    }
                }

                BusinessReport businessReport = new BusinessReport();
                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate)
                    && !businessReport.IsMaxActiveIrregularityReportsReached(objectContext, currentUser))
                {
                    lblSendReport.Visible = true;

                    pnlActionVisible = true;

                    if (string.IsNullOrEmpty(lblReporting.Text))
                    {
                        BusinessSiteText businessText = new BusinessSiteText();
                        SiteNews aboutReporting = businessText.GetSiteText(objectContext, "aboutUserReporting");
                        if (aboutReporting != null)
                        {
                            lblReporting.Text = aboutReporting.description;
                        }
                    }
                }

                BusinessNotifies businessNotifies = new BusinessNotifies();
                if (!businessNotifies.SetNewInformationFalseForProductIfUserIsSigned(objectContext, currentUser, currentProduct)
                    && !businessNotifies.IsMaxNotificationsNumberReached(objectContext, currentUser))
                {
                    pnlActionVisible = true;

                    btnSignForNotifies.Visible = true;
                }

                if (lblSendSuggestion.Visible == true)
                {
                    pnlActionVisible = true;
                }

                if (btnTakeAction.Visible == true)
                {
                    pnlActionVisible = true;
                }
            }

            pnlActions.Visible = pnlActionVisible;

        }

        private void CheckAddImageOptions()
        {
            ImageTools imageTools = new ImageTools();

            if (imageTools.GetProductImagesCount(objectContext, currentProduct) < Configuration.ImagesMaxProductImagesCount)
            {
                apUploadImage.Visible = true;

                string appPath = CommonCode.PathMap.PhysicalApplicationPath;


                Label lblInfo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "lblUpImgINfo");
                Label lblInfo2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "lblUpImgINfo2");

                lblInfo.Text = string.Format("{0} {1}/{2}.", GetLocalResourceObject("ImageRules")
                    , Configuration.ImagesMinImageWidth, Configuration.ImagesMinImageHeight);
                lblInfo2.Text = GetLocalResourceObject("ImageRules2").ToString();

                HyperLink seeImg1 = (HyperLink)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "hlClickForMinImage");
                seeImg1.NavigateUrl = "SampleImage.aspx?show=minProductImage";

                CheckBox cbImageMain = (CheckBox)apUploadImage.FindControl("cbImageMain");
                if (cbImageMain == null)
                {
                    throw new CommonCode.UIException("Cannot find cbImageMain inside apUploadImage");
                }

                if (!imageTools.IfProductHaveMainImg(objectContext, currentProduct.ID, businessLog, appPath))
                {
                    cbImageMain.Visible = true;
                }
                else
                {
                    cbImageMain.Visible = false;
                }
            }
            else
            {
                apUploadImage.Visible = false;
            }
        }

        private void CheckIfUserCanEditAllProductsFromEvents()
        {
            BusinessUser businessUser = new BusinessUser();
            if (currentUser != null)
            {
                if (!businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditProducts))
                {
                    throw new CommonCode.UIException(string.Format("User ID = {0} cannot edit all products", currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("Guest cannot edit all products");
            }
        }

        private void CheckIfUserCanEditProductFromEvents()
        {
            if (currentUser != null)
            {
                BusinessUser businessUser = new BusinessUser();
                BusinessProduct businessProduct = new BusinessProduct();
                Company currCompany = businessProduct.GetProductCompany(objectContext, currentProduct);

                if (!businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditProducts) &&
                    !businessUser.CanUserModifyProduct(objectContext, currentProduct.ID, currentUser.ID) &&
                    !businessUser.CanUserModifyAllCompanyProducts(objectContext, currCompany.ID, currentUser.ID))
                {
                    RedirectToSameUrl(Request.Url.ToString());
                }
            }
            else
            {
                RedirectToSameUrl(Request.Url.ToString());
            }
        }

        private void FillPublicShownProductEditors()
        {
            phPublicProductEditors.Controls.Clear();

            if (!currentProduct.CompanyReference.IsLoaded)
            {
                currentProduct.CompanyReference.Load();
            }

            BusinessUserTypeActions butActions = new BusinessUserTypeActions();
            BusinessUser bUser = new BusinessUser();

            List<UsersTypeAction> actionsForProduct = butActions.GetProductModificators(objectContext, currentProduct.ID).ToList();
            List<UsersTypeAction> actionsForCompany = butActions.GetAllCompanyProductsModificators(objectContext, currentProduct.Company.ID).ToList();

            List<User> usersToWhichCanBeSendSuggestion = new List<User>();

            bool currUserLogged = false;
            bool isUser = false;
            bool canUserTakeAction = false;
            bool currUserCanEditType = false;

            if (currentUser != null)
            {
                currUserCanEditType = currUserCanEditType = bUser.CanUserModifyProduct(objectContext, currentProduct.ID, currentUser.ID);

                currUserLogged = true;
                isUser = bUser.IsUser(currentUser);

                canUserTakeAction = butActions.CanUserTakeActionFromEditor(userContext, objectContext, currentUser);
            }

            if (actionsForProduct.Count > 0 || actionsForCompany.Count > 0)
            {
                Panel infoPanel = new Panel();
                phPublicProductEditors.Controls.Add(infoPanel);
                infoPanel.CssClass = "sectionTextHeader";

                Label lblEditors = new Label();
                infoPanel.Controls.Add(lblEditors);
                lblEditors.Text = GetLocalResourceObject("Editors").ToString();

                User editor = null;

                List<long> userIDs = new List<long>();

                foreach (UsersTypeAction action in actionsForProduct)
                {
                    if (!action.UserReference.IsLoaded)
                    {
                        action.UserReference.Load();
                    }

                    Panel newPanel = new Panel();
                    phPublicProductEditors.Controls.Add(newPanel);

                    Image newImg = new Image();
                    newPanel.Controls.Add(newImg);
                    newImg.ImageUrl = "~/images/SiteImages/triangle.png";
                    newImg.CssClass = "itemImage";

                    editor = bUser.Get(userContext, action.User.ID, true);

                    HyperLink userLink = CommonCode.UiTools.GetUserHyperLink(editor);
                    newPanel.Controls.Add(userLink);
                    userLink.Target = "_blank";

                    if (currUserLogged == true && isUser == true)
                    {
                        if (editor.ID != currentUser.ID)
                        {
                            if (canUserTakeAction == true && butActions.CanTypeActionsBeTakenFromUser(editor)
                                && currUserCanEditType == false)
                            {

                                Panel span = new Panel();
                                newPanel.Controls.Add(span);
                                span.CssClass = "floatRight";

                                HyperLink link = new HyperLink();
                                span.Controls.Add(link);

                                link.CssClass = "searchPageRatings";
                                link.Text = GetLocalResourceObject("TakeRoleS").ToString();
                                link.NavigateUrl = GetUrlWithVariant(string.Format("EditorRights.aspx?User={0}", editor.ID));
                                link.Target = "_blank";
                            }

                            usersToWhichCanBeSendSuggestion.Add(editor);
                        }
                    }

                    userIDs.Add(editor.ID);
                }

                foreach (UsersTypeAction action in actionsForCompany)
                {

                    if (!action.UserReference.IsLoaded)
                    {
                        action.UserReference.Load();
                    }

                    if (!userIDs.Contains(action.User.ID))
                    {
                        Panel newPanel = new Panel();
                        phPublicProductEditors.Controls.Add(newPanel);

                        Image newImg = new Image();
                        newPanel.Controls.Add(newImg);
                        newImg.ImageUrl = "~/images/SiteImages/triangle.png";
                        newImg.CssClass = "itemImage";

                        editor = bUser.Get(userContext, action.User.ID, true);

                        newPanel.Controls.Add(CommonCode.UiTools.GetUserHyperLink(editor));

                        if (currUserLogged == true && isUser == true)
                        {
                            if (editor.ID != currentUser.ID)
                            {
                                if (canUserTakeAction == true && butActions.CanTypeActionsBeTakenFromUser(editor)
                                    && currUserCanEditType == false)
                                {
                                    Panel span = new Panel();
                                    newPanel.Controls.Add(span);
                                    span.CssClass = "floatRight";

                                    HyperLink link = new HyperLink();
                                    span.Controls.Add(link);

                                    link.CssClass = "searchPageRatings";
                                    link.Text = GetLocalResourceObject("TakeRole").ToString();
                                    link.NavigateUrl = GetUrlWithVariant(string.Format("EditorRights.aspx?User={0}", editor.ID));
                                }

                                usersToWhichCanBeSendSuggestion.Add(editor);
                            }
                        }
                    }
                }


            }
            else
            {
                Panel newPanel = new Panel();
                phPublicProductEditors.Controls.Add(newPanel);
                newPanel.CssClass = "sectionTextHeader";

                Label lblNoEditors = new Label();
                newPanel.Controls.Add(lblNoEditors);
                lblNoEditors.Text = GetLocalResourceObject("NoEditors").ToString();
            }


            if (currentUser != null && isUser == true)
            {
                ShowToWhichUsersSuggestionsCanBeSent(usersToWhichCanBeSendSuggestion, currUserCanEditType);
                ShowOptionToTakeRoleIfThereAreNoEditors(currUserCanEditType, usersToWhichCanBeSendSuggestion.Count);
            }
            else
            {
                btnTakeAction.Visible = false;
                lblSendSuggestion.Visible = false;
            }

        }

        private void ShowOptionToTakeRoleIfThereAreNoEditors(bool canUserEditCurrType, int userToWhichCanBeSendSuggestions)
        {
            if (canUserEditCurrType == true || userToWhichCanBeSendSuggestions > 0 || currentProduct.canUserTakeRoleIfNoEditors == false)
            {
                btnTakeAction.Visible = false;
            }
            else
            {
                btnTakeAction.Visible = true;
                btnTakeAction.ToolTip = string.Format("{0} {1}", currentProduct.name, GetLocalResourceObject("DontHaveEditors"));
            }
        }

        private void ShowToWhichUsersSuggestionsCanBeSent(List<User> actionUsers, bool canEditCurrProduct)
        {
            int count = actionUsers.Count;

            if (count > 0 && currentUser != null && canEditCurrProduct == false)
            {
                lblSendSuggestion.Visible = true;

                BusinessUserTypeActions butActions = new BusinessUserTypeActions();

                if (count == 1)
                {
                    hlSuggestionUser.Visible = true;
                    ddlSuggestionUsers.Visible = false;

                    hlSuggestionUser.NavigateUrl = GetUrlWithVariant(string.Format("Profile.aspx?User={0}", actionUsers.First().ID));
                    hlSuggestionUser.Text = actionUsers.First().username;
                    hlSuggestionUser.Attributes.Add("userID", actionUsers.First().ID.ToString());
                }
                else
                {
                    hlSuggestionUser.Visible = false;
                    ddlSuggestionUsers.Visible = true;

                    ddlSuggestionUsers.Items.Clear();

                    ListItem first = new ListItem();
                    first.Text = GetLocalResourceObject("..user..").ToString();
                    first.Value = "0";
                    ddlSuggestionUsers.Items.Add(first);

                    foreach (User user in actionUsers)
                    {
                        ListItem item = new ListItem();
                        item.Text = user.username;
                        item.Value = user.ID.ToString();
                        ddlSuggestionUsers.Items.Add(item);
                    }

                }

            }
            else
            {
                lblSendSuggestion.Visible = false;
            }
        }

        protected void ddlActionUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList list = sender as DropDownList;
            if (list == null)
            {
                throw new CommonCode.UIException("Couldn`t get ddlActionUsers");
            }

            if (currentUser == null)
            {
                throw new CommonCode.UIException("currentUser is null");
            }

            if (list.SelectedIndex > 0)
            {
                long userId = 0;
                if (!long.TryParse(list.SelectedValue, out userId))
                {
                    throw new CommonCode.UIException(string.Format("Couldn`t parse ddlActionUsers.SelectedValue to long, User id : {1}", currentUser.ID));
                }

                BusinessUser bUser = new BusinessUser();
                User user = bUser.Get(userContext, userId, true);

                RedirectToOtherUrl(string.Format("EditorRights.aspx?User={0}", user.ID));
            }
            else
            {
                throw new CommonCode.UIException(string.Format("ddlActionUsers.SelectedIndex = 0, User id : {1}", currentUser.ID));
            }
        }

        /// <summary>
        /// Used to update Notification Label when change has been made that can affect product visibility by users
        /// </summary>
        private void UpdateProductNotification()
        {
            BusinessProduct businessProduct = new BusinessProduct();
            businessProduct.CheckIfProductsIsValidWithConnections(objectContext, currentProduct, out notifError);
        }

        /// <summary>
        /// Shows Notification label to admins if product cannot be seen by users with reason
        /// </summary>
        private void ShowNotification()
        {
            if (notifError.Length > 0)
            {
                lblProdNotif.Text = notifError;
                pnlNotification.Visible = true;
            }
            else
            {
                pnlNotification.Visible = false;
            }
        }

        private void ShowAdvertisement()
        {
            if (Configuration.AdvertsNumAdvertsOnProductPage > 0)
            {
                phAdvertisement.Controls.Clear();
                adCell.Attributes.Clear();

                phAdvertisement.Controls.Add(CommonCode.ImagesAndAdverts.GetAdvertisements
                    (objectContext, Server, "product", currentProduct.ID, Configuration.AdvertsNumAdvertsOnProductPage));
                if (CommonCode.ImagesAndAdverts.getAdvertisementsNumber(phAdvertisement) > 0)
                {
                    phAdvertisement.Visible = true;

                    adCell.Width = "252px";
                    adCell.VAlign = "top";
                }
                else
                {
                    phAdvertisement.Visible = false;
                    adCell.Width = "1px";
                }
            }
        }

        private void ShowMainImage(ImageTools imageTools, Product currProduct)
        {
            string appPath = CommonCode.PathMap.PhysicalApplicationPath;
            ProductImage img = imageTools.GetProductMainImage(objectContext, currProduct, businessLog, appPath);
            if (img != null)
            {
                string giUrl = "GetImageHandler.ashx";

                if (BusinessLayer.Configuration.UseUrlRewriting == true)
                {
                    if (Configuration.UseExternalUrlRewriteModule == false)
                    {
                        giUrl = string.Format("~/{0}", giUrl);
                    }
                    else
                    {
                        giUrl = CommonCode.UiTools.UrlWithApplicationPath(giUrl);
                    }
                }
                else
                {
                    giUrl = string.Format("~/{0}", giUrl);
                }

                giMainImage.ImageHandlerUrl = giUrl;

                giMainImage.Parameters.Clear();

                Microsoft.Web.ImageParameter prodIDParam = new Microsoft.Web.ImageParameter();
                prodIDParam.Name = "prodID";
                prodIDParam.Value = currentProduct.ID.ToString();
                giMainImage.Parameters.Add(prodIDParam);
                giMainImage.ToolTip = img.description;

                imgLink.NavigateUrl = img.url;
                imgLink.Target = "_blank";
                imgLink.Visible = true;

                if (img.description.Length > 0)
                {
                    giMainImage.ToolTip = img.description;
                }
            }
            else
            {
                imgLink.Visible = false;
            }
        }

        private String CurrProductUrl()
        {

            String URL = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", currentProduct.ID));
            return URL;
        }

        private string GetSortByParams()
        {
            System.Text.StringBuilder paramsQuery = new System.Text.StringBuilder();

            if (SortByRating == 0)
            {
                if (SortByDateDesc)
                {
                    paramsQuery.Append("&date=desc");
                }
                else
                {
                    paramsQuery.Append("&date=asc");
                }
            }
            else
            {
                switch (SortByRating)
                {
                    case SortOptions.Descending:  // 1:
                        paramsQuery.Append("&rating=desc");
                        break;
                    case SortOptions.Ascending:  // 2:
                        paramsQuery.Append("&rating=asc");
                        break;
                    default:
                        throw new CommonCode.UIException(string.Format("SortByRating = '{0}' is not valid case", SortByRating));
                }
            }


            if (SortByChar > 0)
            {
                paramsQuery.Append("&char=" + SortByChar);
            }
            else if (SortByVariant > 0)
            {
                paramsQuery.Append("&variant=" + SortByVariant);
            }
            else if (SortBySubVariant > 0)
            {
                paramsQuery.Append("&subvariant=" + SortBySubVariant);
            }
            else if (SortByNoAbout == true)
            {
                paramsQuery.Append("&noabout=true");
            }

            return paramsQuery.ToString();
        }

        private void CommentsPageAndNumber()
        {
            tblPages.Rows.Clear();
            tblPagesBottom.Rows.Clear();

            if (CommNumber > minCommOnPage)
            {

                TableRow newRow = new TableRow();
                tblPages.Rows.Add(newRow);
                TableCell pagesCell = new TableCell();
                newRow.Cells.Add(pagesCell);
                pagesCell.Text = GetLocalResourceObject("Page").ToString();

                TableRow btmRow = new TableRow();
                tblPagesBottom.Rows.Add(btmRow);
                TableCell btmCell = new TableCell();
                btmRow.Cells.Add(btmCell);
                btmCell.Text = GetLocalResourceObject("Page").ToString();

                String prodURL = CurrProductUrl();
                String SortBy = GetSortByParams();

                int numOfLinks = Configuration.PagesNumOfLinks;

                long numOfPages = CommNumber / commentsOnPage;
                if ((CommNumber % commentsOnPage) > 0)
                {
                    numOfPages++;
                }

                if (numOfPages <= numOfLinks)
                {
                    for (int i = 1; i <= numOfPages; i++)
                    {
                        if (i != numberOfPage)
                        {
                            newRow.Cells.Add(CommonCode.UiTools.GetHyperLinkCell(string.Format("GoToPage{0}", i),
                                string.Format("{0}&num={1}&page={2}{3}", prodURL, commentsOnPage, i, SortBy),
                                string.Format("&nbsp;{0}&nbsp;", i.ToString())));

                            btmRow.Cells.Add(CommonCode.UiTools.GetHyperLinkCell(string.Format("BtmGoToPage{0}", i),
                                string.Format("{0}&num={1}&page={2}{3}", prodURL, commentsOnPage, i, SortBy)
                                , string.Format("&nbsp;{0}&nbsp;", i.ToString())));
                        }
                        else
                        {
                            TableCell currCell = new TableCell();
                            Label currPage = new Label();
                            currPage.Text = numberOfPage.ToString();
                            currCell.Controls.Add(currPage);
                            newRow.Cells.Add(currCell);

                            TableCell currBtmCell = new TableCell();
                            Label currBtmPage = new Label();
                            currBtmPage.Text = numberOfPage.ToString();
                            currBtmCell.Controls.Add(currBtmPage);
                            btmRow.Cells.Add(currBtmCell);
                        }
                    }
                }
                else
                {
                    newRow.Cells.Add(CommonCode.UiTools.GetHyperLinkCell(string.Format("Page0"),
                        string.Format("{0}&num={1}&page=1{2}", prodURL, commentsOnPage, SortBy), "&nbsp;<<&nbsp;"));

                    btmRow.Cells.Add(CommonCode.UiTools.GetHyperLinkCell(string.Format("BtmPage0"),
                        string.Format("{0}&num={1}&page=1{2}", prodURL, commentsOnPage, SortBy), "&nbsp;<<&nbsp;"));

                    int pb = 0;    // pages behind
                    int pa = 0;    // pages after

                    int pagesBehindAndAfter = numOfLinks / 2;

                    for (int pagesBehind = pagesBehindAndAfter; pagesBehind > 0; pagesBehind--)
                    {
                        if (numberOfPage - pagesBehind >= 1)
                        {
                            pb++;
                        }
                    }

                    for (int pagesAfter = 1; pagesAfter <= pagesBehindAndAfter; pagesAfter++)
                    {
                        if (numberOfPage + pagesAfter <= numOfPages)
                        {
                            pa++;
                        }
                    }

                    if (pb < pagesBehindAndAfter)
                    {
                        pa += pagesBehindAndAfter - pb;
                    }
                    if (pa < pagesBehindAndAfter)
                    {
                        pb += pagesBehindAndAfter - pa;
                    }

                    for (int cp = (numberOfPage - pb); cp < numberOfPage; cp++)
                    {

                        newRow.Cells.Add(CommonCode.UiTools.GetHyperLinkCell(string.Format("Page{0}", cp),
                            string.Format("{0}&num={1}&page={2}{3}", prodURL, commentsOnPage, cp, SortBy),
                            string.Format("&nbsp;{0}&nbsp;", cp.ToString())));

                        btmRow.Cells.Add(CommonCode.UiTools.GetHyperLinkCell(string.Format("BtmPage{0}", cp),
                            string.Format("{0}&num={1}&page={2}{3}", prodURL, commentsOnPage, cp, SortBy),
                            string.Format("&nbsp;{0}&nbsp;", cp.ToString())));
                    }

                    TableCell currCell = new TableCell();
                    Label currPage = new Label();
                    currPage.Text = numberOfPage.ToString();
                    currCell.Controls.Add(currPage);
                    newRow.Cells.Add(currCell);

                    TableCell currBtmCell = new TableCell();
                    Label currBtmPage = new Label();
                    currBtmPage.Text = numberOfPage.ToString();
                    currBtmCell.Controls.Add(currBtmPage);
                    btmRow.Cells.Add(currBtmCell);

                    for (int cp = (numberOfPage + 1); cp <= (pa + numberOfPage); cp++)
                    {
                        newRow.Cells.Add(CommonCode.UiTools.GetHyperLinkCell(string.Format("Page{0}", cp),
                            string.Format("{0}&num={1}&page={2}{3}", prodURL, commentsOnPage, cp, SortBy),
                            string.Format("&nbsp;{0}&nbsp;", cp.ToString())));

                        btmRow.Cells.Add(CommonCode.UiTools.GetHyperLinkCell(string.Format("BtmPage{0}", cp),
                            string.Format("{0}&num={1}&page={2}{3}", prodURL, commentsOnPage, cp, SortBy),
                            string.Format("&nbsp;{0}&nbsp;", cp.ToString())));
                    }

                    newRow.Cells.Add(CommonCode.UiTools.GetHyperLinkCell(string.Format("LastPage{0}", numOfPages),
                        string.Format("{0}&num={1}&page={2}{3}", prodURL, commentsOnPage, numOfPages, SortBy), "&nbsp;>>&nbsp;"));

                    btmRow.Cells.Add(CommonCode.UiTools.GetHyperLinkCell(string.Format("BtmLastPage{0}", numOfPages),
                        string.Format("{0}&num={1}&page={2}{3}", prodURL, commentsOnPage, numOfPages, SortBy), "&nbsp;>>&nbsp;"));
                }

                TableCell spaceCell = new TableCell();
                spaceCell.CssClass = "searchPageComments";
                spaceCell.Text = string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0} ", GetLocalResourceObject("CommentsOnPage"));
                newRow.Cells.Add(spaceCell);

                // links changing comments on page
                TableCell MinCommNumCell = new TableCell();
                if (commentsOnPage == minCommOnPage)
                {
                    MinCommNumCell.Text = string.Format("&nbsp;{0}&nbsp;", minCommOnPage);
                }
                else
                {
                    MinCommNumCell.Controls.Add(CommonCode.UiTools.GetHyperLink(string.Format("CommentOnPage{0}", minCommOnPage),
                        string.Format("{0}&num={1}&page=1{2}", prodURL, minCommOnPage, SortBy),
                        string.Format("&nbsp;{0}&nbsp;", minCommOnPage)));

                }
                newRow.Cells.Add(MinCommNumCell);

                if (CommNumber > minCommOnPage)
                {
                    TableCell DeffCommNumCell = new TableCell();
                    if (commentsOnPage == defCommOnPage)
                    {
                        DeffCommNumCell.Text = defCommOnPage.ToString();
                    }
                    else
                    {
                        DeffCommNumCell.Controls.Add(CommonCode.UiTools.GetHyperLink(string.Format("CommentOnPage{0}", defCommOnPage),
                            string.Format("{0}&num={1}&page=1{2}", prodURL, defCommOnPage, SortBy),
                            string.Format("&nbsp;{0}&nbsp;", defCommOnPage)));
                    }
                    newRow.Cells.Add(DeffCommNumCell);
                }

                if (CommNumber > defCommOnPage)
                {
                    TableCell MaxCommNumCell = new TableCell();
                    if (commentsOnPage == maxCommOnPage)
                    {
                        MaxCommNumCell.Text = maxCommOnPage.ToString();
                    }
                    else
                    {

                        MaxCommNumCell.Controls.Add(CommonCode.UiTools.GetHyperLink(string.Format("CommentOnPage{0}", maxCommOnPage),
                            string.Format("{0}&num={1}&page=1{2}", prodURL, maxCommOnPage, SortBy),
                            string.Format("&nbsp;{0}&nbsp;", maxCommOnPage)));
                    }
                    newRow.Cells.Add(MaxCommNumCell);
                }


            }

        }

        private void WriteComment()
        {
            BusinessUser businessUser = new BusinessUser();
            BusinessProduct businessProduct = new BusinessProduct();

            if (currentUser != null)
            {
                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
                {
                    pnlWriteComment.Visible = true;
                    lblCName.Visible = false;
                    tbName.Visible = false;

                    CaptchaControl.Visible = false;
                    tbCaptchaMsg.Visible = false;
                }
                else
                {
                    pnlWriteComment.Visible = false;
                }

            }
            else
            {
                lblCName.Visible = true;
                tbName.Visible = true;

                CaptchaControl.Visible = true;
                tbCaptchaMsg.Visible = true;
            }

            if (ApplicationVariantString == "bg")
            {
                transliterateBtnComment.Visible = true;
                transliterateBtnComment.TargetTextBox = tbDescription;

                btnTransReplyToComment.Visible = true;
                btnTransReplyToComment.TargetTextArea = "tbReplyToUser";

                btnTransMsgToUser.Visible = true;
                btnTransMsgToUser.TargetTextArea = "tbMsgToUser";
            }
            else
            {

                transliterateBtnComment.Visible = false;
                btnTransReplyToComment.Visible = false;
                btnTransMsgToUser.Visible = false;
            }


            if (changedChars || changedVariants)
            {
                if (changedChars == true)
                {
                    FillCommDDLProdChars(currentProduct, businessProduct);
                }
                if (changedVariants == true)
                {
                    FillCommDDLAboutVariant();
                }

                if (ddlChars.Visible == true)
                {
                    ddlChars.Enabled = true;
                    ddlChars.SelectedIndex = 0;
                }
                if (ddlAboutVariant.Visible == true)
                {
                    ddlAboutVariant.Enabled = true;
                    ddlAboutVariant.SelectedIndex = 0;
                }
            }
            else if (IsPostBack == false)
            {
                FillCommDDLProdChars(currentProduct, businessProduct);
                FillCommDDLAboutVariant();
            }


            if (ddlChars.Visible == true && ddlAboutVariant.Visible == true)
            {
                lblOr.Visible = true;
            }
            else
            {
                lblOr.Visible = false;
            }

            if (currentUser != null)
            {
                pnlAddCommentUsername.Visible = false;
            }
            else
            {
                pnlAddCommentUsername.Visible = true;
            }
        }

        private void FillCommDDLProdChars(Product currProduct, BusinessProduct businessProduct)
        {
            ddlChars.Items.Clear();

            IEnumerable<ProductCharacteristics> productCharecteristics
            = businessProduct.GetAllProductCharacteristics(objectContext, currProduct.ID, false);

            if (productCharecteristics.Count<ProductCharacteristics>() > 0)
            {
                pnlAddCommentForChar.Visible = true;

                // if product have chars
                ListItem defItem = new ListItem();
                defItem.Text = GetLocalResourceObject("choose...").ToString();
                defItem.Value = "0";
                ddlChars.Items.Add(defItem);
                foreach (ProductCharacteristics productCharacteristic in productCharecteristics)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = productCharacteristic.name;
                    newItem.Value = productCharacteristic.ID.ToString();
                    ddlChars.Items.Add(newItem);
                }

            }
            else
            {
                pnlAddCommentForChar.Visible = false;
            }
        }

        private void FillCommDDLAboutVariant()
        {
            ddlAboutVariant.Items.Clear();
            ddlAboutSubVariant.Items.Clear();

            BusinessProductVariant bpVariant = new BusinessProductVariant();
            List<ProductVariant> variants = bpVariant.GetVisibleVariants(objectContext, currentProduct);
            if (variants.Count > 0)
            {
                pnlAddCommentForVariant.Visible = true;

                ddlAboutVariant.Enabled = true;

                ListItem firstItem = new ListItem();
                ddlAboutVariant.Items.Add(firstItem);
                firstItem.Text = GetLocalResourceObject("choose...").ToString();
                firstItem.Value = "0";

                foreach (ProductVariant variant in variants)
                {
                    ListItem newItem = new ListItem();
                    ddlAboutVariant.Items.Add(newItem);
                    newItem.Text = variant.name;
                    newItem.Value = variant.ID.ToString();
                }

                if (bpVariant.CountSubVariants(objectContext, currentProduct) > 0)
                {
                    ddlAboutSubVariant.Visible = true;
                    ddlAboutSubVariant.Enabled = false;

                    ListItem newItem = new ListItem();
                    ddlAboutSubVariant.Items.Add(newItem);
                    newItem.Text = GetLocalResourceObject("choose...").ToString();
                    newItem.Value = "0";
                }
                else
                {
                    ddlAboutSubVariant.Visible = false;
                }
            }
            else
            {
                pnlAddCommentForVariant.Visible = false;
            }

        }

        private void ShowComment(Control parentControl, Comment comment, int level, Boolean canUserRateComment, Boolean canUserEditComment,
            Boolean canUserWriteComment, Boolean canUserReportInappopriate, Boolean canUserSendPrivateMessages,
            User currUser, User guestUser, List<long> admins, int rowNum)
        {
            BusinessUser businessUser = new BusinessUser();
            ProductCharacteristics currPChar = new ProductCharacteristics();
            BusinessProduct businessProduct = new BusinessProduct();
            BusinessComment businessComment = new BusinessComment();
            BusinessRating businessRating = new BusinessRating();


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



            Table table = new Table();
            table.Width = Unit.Percentage(100);
            newPanel.Controls.Add(table);

            TableRow Roll = new TableRow();
            table.Rows.Add(Roll);
            Roll.Attributes.Add("commentID", comment.ID.ToString());

            if (!comment.UserIDReference.IsLoaded)
            {
                comment.UserIDReference.Load();
            }

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

            bool isAdmin = false;
            if (admins.Contains(comment.UserID.ID))
            {
                isAdmin = true;
            }

            if (canUserEditComment)
            {
                TableCell cellID = new TableCell();
                cellID.VerticalAlign = VerticalAlign.Top;
                cellID.Width = 40;
                cellID.Text = comment.ID.ToString();
                Roll.Cells.Add(cellID);
            }

            TableCell cellName = new TableCell();
            cellName.VerticalAlign = VerticalAlign.Top;
            cellName.Width = Unit.Pixel(200);


            User commentUser = Tools.GetUserFromUserDatabase(userContext, comment.UserID);

            String name = "";
            if (commentUser == guestUser)
            {
                Label nameLbl = new Label();
                nameLbl.Text = comment.guestname;
                cellName.Controls.Add(nameLbl);
                nameLbl.CssClass = "userNames";

                Label guestLbl = new Label();
                guestLbl.Text = string.Format(" {0}", GetLocalResourceObject("(guest)"));
                guestLbl.CssClass = "smallFontSize";
                cellName.Controls.Add(guestLbl);
            }
            else
            {
                cellName.CssClass = "userNames";

                name = commentUser.username;

                if (commentUser.visible == true)
                {
                    if (comment.UserID != null && isAdmin)
                    {
                        cellName.Controls.Add(CommonCode.UiTools.GetAdminLabel(name));
                    }
                    else
                    {
                        HyperLink userLink = CommonCode.UiTools.GetUserHyperLink(commentUser);
                        userLink.Font.Size = FontUnit.Point(15);
                        cellName.Controls.Add(userLink);
                    }
                }
                else
                {
                    if (comment.UserID != null && isAdmin)
                    {
                        cellName.Controls.Add(CommonCode.UiTools.GetAdminLabel(name));
                    }
                    else
                    {
                        cellName.Text = name;
                    }
                }
            }
            Roll.Cells.Add(cellName);

            TableCell dateCell = new TableCell();
            dateCell.VerticalAlign = VerticalAlign.Top;
            dateCell.Width = Unit.Pixel(135);
            Roll.Cells.Add(dateCell);

            Label dateLbl = new Label();
            dateLbl.Text = CommonCode.UiTools.DateTimeToLocalString("g", comment.dateCreated);
            dateLbl.CssClass = "commentsDate smallFontSize";
            dateCell.Controls.Add(dateLbl);

            TableCell cellForType = new TableCell();   // for which characteristic/variant/subvariant is the comment
            Roll.Cells.Add(cellForType);
            cellForType.ForeColor = CommonCode.UiTools.GetStandardGreenColor();
            cellForType.VerticalAlign = VerticalAlign.Top;

            if (comment.ForCharacteristic != null)
            {
                cellForType.Text = string.Format("{0}", Tools.BreakLongWordsInString(comment.ForCharacteristic.name, 20));
            }
            else if (comment.ForVariant != null)
            {
                cellForType.Text = string.Format("{0}", Tools.BreakLongWordsInString(comment.ForVariant.name, 20));
            }
            else if (comment.ForSubVariant != null)
            {
                if (!comment.ForSubVariant.VariantReference.IsLoaded)
                {
                    comment.ForSubVariant.VariantReference.Load();
                }

                cellForType.Text = string.Format("{0} : {1}", Tools.BreakLongWordsInString(comment.ForSubVariant.Variant.name, 20)
                    , Tools.BreakLongWordsInString(comment.ForSubVariant.name, 20));
            }
            else
            {
                cellForType.Text = "&nbsp;";
            }

            if (isAdmin == false)
            {
                TableCell imgCell = new TableCell();
                imgCell.Width = Unit.Pixel(100);
                imgCell.VerticalAlign = VerticalAlign.Top;
                imgCell.HorizontalAlign = HorizontalAlign.Right;
                Roll.Cells.Add(imgCell);

                Image plusBtn = new Image();
                imgCell.Controls.Add(plusBtn);

                plusBtn.ID = string.Format("agree{0}", comment.ID);
                plusBtn.ImageUrl = "~\\images\\SiteImages\\plus.png"; //plus.bmp
                plusBtn.CssClass = "middleAlign pointerCursor";

                Label agreesLbl = new Label();
                agreesLbl.Text = string.Format("({0}) ", comment.agrees);
                imgCell.Controls.Add(agreesLbl);

                plusBtn.Attributes.Add("onclick", string.Format("ShowRatingData('{0}','{1}','{2}','{3}','{4}','{5}')"
                    , agreesLbl.ClientID, comment.agrees, "1", plusBtn.ClientID, pnlRateComm.ClientID, comment.ID));

                Image minusBtn = new Image();
                minusBtn.ID = string.Format("disagree{0}", comment.ID);
                minusBtn.ImageUrl = "~\\images\\SiteImages\\minus.png";
                minusBtn.CssClass = "middleAlign pointerCursor";
                imgCell.Controls.Add(minusBtn);

                Label disagreesLbl = new Label();
                disagreesLbl.Text = string.Format("({0})", comment.disagrees);
                imgCell.Controls.Add(disagreesLbl);


                /////
                minusBtn.Attributes.Add("onclick", string.Format("ShowRatingData('{0}','{1}','{2}','{3}','{4}','{5}')"
                    , disagreesLbl.ClientID, comment.disagrees, "-1", minusBtn.ClientID, pnlRateComm.ClientID, comment.ID));
                ////
            }

            //////////////////////
            AddActionsCellToComments(comment, level, canUserEditComment, canUserWriteComment, canUserReportInappopriate
                , canUserSendPrivateMessages, currUser, businessUser, Roll, admins, isAdmin, guestUser, commentUser);
            //////////////////////

            Panel descrPanel = new Panel();
            newPanel.Controls.Add(descrPanel);

            Label descrLbl = new Label();
            descrLbl.Text = Tools.GetFormattedTextFromDB(comment.description);
            descrPanel.Controls.Add(descrLbl);

            if (comment.lastModified > comment.dateCreated)
            {
                if (!comment.LastModifiedByReference.IsLoaded)
                {
                    comment.LastModifiedByReference.Load();
                }

                Panel lastModif = new Panel();
                newPanel.Controls.Add(lastModif);
                lastModif.CssClass = "clearfix";

                User lastModifBy = businessUser.GetWithoutVisible(userContext, comment.LastModifiedBy.ID, true);

                Panel pnlLastModif = new Panel();
                lastModif.Controls.Add(pnlLastModif);
                pnlLastModif.CssClass = "floatRightNoMrg";

                Label lblLastModif = new Label();
                pnlLastModif.Controls.Add(lblLastModif);

                lblLastModif.Text = string.Format("{0} {1}, {2}", GetLocalResourceObject("lastEdited")
                    , lastModifBy.username, comment.lastModified.ToString());
                lblLastModif.CssClass = "topicsModificationLbl";
            }

            if (comment.haveSubcomments == true)
            {
                IEnumerable<Comment> SubComments = businessComment.GetAllSubComments(objectContext, comment.ID);
                if (SubComments.Count<Comment>() > 0)
                {
                    foreach (Comment subcomment in SubComments)
                    {
                        ShowComment(newPanel, subcomment, level + 1, canUserRateComment, canUserEditComment,
                            canUserWriteComment, canUserReportInappopriate, canUserSendPrivateMessages,
                            currUser, guestUser, admins, rowNum);
                    }
                }
            }

        }

        private void AddActionsCellToComments(Comment comment, int level, Boolean canUserEditComment
            , Boolean canUserWriteComment, Boolean canUserReportInappopriate, Boolean canUserSendPrivateMessages
            , User currUser, BusinessUser businessUser, TableRow Roll, List<long> admins, bool isAdmin, User guestUser, User commentUser)
        {
            if (canUserEditComment == true || canUserWriteComment == true || canUserReportInappopriate == true
                || canUserSendPrivateMessages == true)
            {

                TableCell actionsCell = new TableCell();
                actionsCell.VerticalAlign = VerticalAlign.Top;
                actionsCell.HorizontalAlign = HorizontalAlign.Right;
                actionsCell.Width = Unit.Pixel(80);
                Roll.Cells.Add(actionsCell);

                DropDownList actionsList = new DropDownList();
                actionsCell.Controls.Add(actionsList);

                actionsList.ID = string.Format("ActionsFor{0}", comment.ID);
                actionsList.Items.Clear();

                ListItem firstItem = new ListItem();
                firstItem.Text = GetLocalResourceObject("Actions").ToString();
                firstItem.Value = "0";
                actionsList.Items.Add(firstItem);

                actionsList.SelectedIndex = 0;

                int itemsAdded = 0;

                if (!comment.UserIDReference.IsLoaded)
                {
                    comment.UserIDReference.Load();
                }

                string msgToUser = commentUser.username;

                if (commentUser == guestUser)
                {
                    msgToUser = comment.guestname;
                }

                if (!isAdmin)
                {
                    if (canUserWriteComment && level < Configuration.CommentsMaxCommentsReplyLevel)
                    {
                        ListItem replyItem = new ListItem();
                        replyItem.Text = GetLocalResourceObject("Reply").ToString();
                        replyItem.Value = "1";
                        actionsList.Items.Add(replyItem);

                        itemsAdded++;
                    }
                }

                BusinessMessages businessMessages = new BusinessMessages();

                if (canUserSendPrivateMessages)
                {
                    bool canSendMsgToUser = false;

                    if (isAdmin)
                    {
                        if (admins.Contains(currentUser.ID))
                        {
                            canSendMsgToUser = true;
                        }
                    }
                    else if (businessUser.IsFromUserTeam(commentUser))
                    {
                        canSendMsgToUser = true;
                    }

                    if (canSendMsgToUser && commentUser != guestUser && commentUser != currUser
                        && businessUser.IsUserVisible(commentUser))
                    {
                        ListItem msgItem = new ListItem();
                        msgItem.Text = GetLocalResourceObject("Message").ToString();
                        msgItem.Value = "2";
                        actionsList.Items.Add(msgItem);

                        itemsAdded++;
                    }
                }

                if (canUserReportInappopriate && !isAdmin)
                {
                    ListItem spamItem = new ListItem();
                    spamItem.Text = GetGlobalResourceObject("SiteResources", "violation").ToString();
                    spamItem.Value = "3";
                    actionsList.Items.Add(spamItem);

                    itemsAdded++;
                }

                if (canUserEditComment)
                {
                    ListItem editItem = new ListItem();
                    editItem.Text = GetLocalResourceObject("Edit").ToString();
                    editItem.Value = "4";
                    actionsList.Items.Add(editItem);

                    itemsAdded++;

                    ListItem delItem = new ListItem();
                    delItem.Text = GetLocalResourceObject("Delete").ToString();
                    delItem.Value = "5";
                    actionsList.Items.Add(delItem);

                    itemsAdded++;

                    ListItem delNWItem = new ListItem();
                    delNWItem.Text = GetLocalResourceObject("DeleteNoWarn").ToString();
                    delNWItem.Value = "6";
                    actionsList.Items.Add(delNWItem);

                    itemsAdded++;
                }

                actionsList.Attributes.Add("onChange", string.Format("ShowActionData('{0}','{1}','{2}','{3}','{4}','{5}','{6}')"
                   , actionsList.ClientID, comment.ID, pnlActionComm.ClientID, pnlMsgToUser.ClientID
                   , msgToUser, pnlReplyToComm.ClientID, pnlEditComment.ClientID));

                if (itemsAdded < 1)
                {
                    actionsCell.Controls.Clear();
                }

            }

        }

        void actionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BusinessComment businessComment = new BusinessComment();
            BusinessUser businessUser = new BusinessUser();

            DropDownList ddlSender = sender as DropDownList;
            if (ddlSender != null)
            {
                TableCell tblCell = ddlSender.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long commID = 0;
                        string commentIdStr = tblRow.Attributes["commentID"];
                        long.TryParse(commentIdStr, out commID);
                        if (commID > 0)
                        {
                            int action = 0;
                            string actionStr = ddlSender.SelectedValue;
                            ddlSender.SelectedIndex = 0;
                            if (int.TryParse(actionStr, out action))
                            {
                                // 0 - nothing, 1 - reply, 2 - message, 3 - spam, 4 - edit, 5 - delete

                                Comment parentComm = businessComment.Get(objectContext, commID);
                                if (parentComm == null)
                                {
                                    throw new CommonCode.UIException(string.Format("There`s no parent comment with ID = {0} ", commID));
                                }

                                if (parentComm.UserIDReference.IsLoaded == false)
                                {
                                    parentComm.UserIDReference.Load();
                                }

                                switch (action)
                                {
                                    case 1:
                                        String commName = "";
                                        if (parentComm.guestname == null)
                                        {
                                            commName = Tools.GetUserFromUserDatabase(userContext, parentComm.UserID).username;
                                        }
                                        else
                                        {
                                            commName = parentComm.guestname;
                                        }
                                        String date = CommonCode.UiTools.DateTimeToLocalShortDateString(parentComm.dateCreated);

                                        lblReply.Visible = true;
                                        btnCancel.Visible = true;
                                        lblReplyTo.Visible = true;
                                        lblReplyTo.Text = (commName + " , " + date);
                                        lblReplyTo.Attributes.Clear();
                                        lblReplyTo.Attributes.Add("commID", parentComm.ID.ToString());
                                        lblReplyTo.Attributes.Add("sub", "yes");

                                        lblCC.Visible = false;
                                        ddlChars.Visible = false;
                                        break;

                                    case 2:
                                        if (currentUser == null
                                            || !businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
                                        {
                                            if (currentUser != null)
                                            {
                                                throw new CommonCode.UIException(string.Format("User ID = {0} cannot send private messages", currentUser.ID));
                                            }
                                            else
                                            {
                                                throw new CommonCode.UIException("Guest cannot send private messages");
                                            }
                                        }

                                        BusinessMessages businessMessages = new BusinessMessages();

                                        if (businessMessages.CanUserSendMessageTo(userContext, currentUser.ID, parentComm.UserID.ID))
                                        {
                                            RedirectToOtherUrl(string.Format("Profile.aspx?MsgTo={0}", parentComm.UserID.ID));
                                        }
                                        else
                                        {
                                            pnlError.Visible = true;
                                            lblCommError.Text = "You cannot send private messages to that user.";
                                        }
                                        break;

                                    case 3:
                                        if (currentUser == null || !businessUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate))
                                        {

                                            if (currentUser != null)
                                            {
                                                throw new CommonCode.UIException(string.Format("User ID = {0} cannot report messages for spam", currentUser.ID));
                                            }
                                            else
                                            {
                                                throw new CommonCode.UIException("Guest cannot report messages for spam");
                                            }
                                        }

                                        BusinessReport businessReport = new BusinessReport();
                                        if (businessReport.IsMaxActiveSpamReportsReached(objectContext, currentUser))
                                        {
                                            throw new CommonCode.UIException(string.Format("User id = {0} , cannot report spam because he reached the maximum active reports limit"
                                                , currentUser.ID));
                                        }

                                        businessReport.CreateCommentViolationReport(objectContext, userContext, currentUser, businessLog, parentComm, CommentType.Product);
                                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Comment marked as spam to moderators!");
                                        ShowComments(); ;
                                        break;

                                    case 4:
                                        if (currentUser == null || !businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
                                        {
                                            if (currentUser != null)
                                            {
                                                throw new CommonCode.UIException(string.Format("User ID = {0} cannot edit comments", currentUser.ID));
                                            }
                                            else
                                            {
                                                throw new CommonCode.UIException("Guest cannot edit comments");
                                            }
                                        }

                                        if (BusinessUser.CanAdminEditStuffFromUser(currentUser, Tools.GetUserFromUserDatabase(userContext, parentComm.UserID)))
                                        {
                                            String name = "";
                                            if (parentComm.guestname == null)
                                            {
                                                if (parentComm.UserIDReference.IsLoaded == false)
                                                {
                                                    parentComm.UserIDReference.Load();
                                                }

                                                name = Tools.GetUserFromUserDatabase(userContext, parentComm.UserID).username;
                                            }
                                            else
                                            {
                                                name = parentComm.guestname;
                                            }
                                            String dateEdit = CommonCode.UiTools.DateTimeToLocalShortDateString(parentComm.dateCreated);

                                            lblCommentAction.Text = "Edit comment :";

                                            lblReply.Visible = true;
                                            lblReply.Text = "Name : ";

                                            lblReplyTo.Visible = true;
                                            lblReplyTo.Text = string.Format("{0} , {1}", name, dateEdit);
                                            lblReplyTo.Attributes.Clear();
                                            lblReplyTo.Attributes.Add("edit", "yes");
                                            lblReplyTo.Attributes.Add("commID", parentComm.ID.ToString());

                                            tbDescription.Text = parentComm.description;

                                            btnCancel.Visible = true;
                                            lblCC.Visible = false;
                                            ddlChars.Visible = false;
                                        }
                                        else
                                        {
                                            pnlError.Visible = true;
                                            lblCommError.Text = "You cannot edit that user`s comment!";
                                        }
                                        break;

                                    case 5:
                                        if (currentUser == null || !businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
                                        {
                                            if (currentUser != null)
                                            {
                                                throw new CommonCode.UIException(string.Format("User ID = {0} cannot delete comments", currentUser.ID));
                                            }
                                            else
                                            {
                                                throw new CommonCode.UIException("Guest cannot delete comments");
                                            }
                                        }

                                        if (BusinessUser.CanAdminEditStuffFromUser(currentUser,
                                            Tools.GetUserFromUserDatabase(userContext, parentComm.UserID)))
                                        {
                                            businessComment.DeleteComment(objectContext, userContext, parentComm, currentUser, businessLog, true, true);

                                            if (tblChars.Visible)
                                            {
                                                ShowCharacteristics();
                                            }
                                            ShowInfo();
                                            ShowComments();

                                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Comment deleted!");
                                        }
                                        else
                                        {
                                            pnlError.Visible = true;
                                            lblCommError.Text = "You cannot delete that user`s comment!";
                                        }
                                        break;

                                    default:
                                        string error = string.Format("action N:{0} is not supported action.", action);
                                        throw new CommonCode.UIException(error);
                                }
                            }
                            else
                            {
                                throw new CommonCode.UIException("Couldnt parse ddlSender.SelectedValue to int");
                            }

                        }
                        else
                        {
                            throw new CommonCode.UIException(
                                string.Format("Invalid comment ID: \"{0}\".", commentIdStr ?? "null"));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("couldnt get parent row.");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("couldnt get parent cell.");
                }
            }
            else
            {
                throw new CommonCode.UIException("couldnt get drop down list.");
            }
        }

        private void ShowComments()
        {
            phComments.Controls.Clear();

            BusinessUser businessUser = new BusinessUser();
            BusinessProduct businessProduct = new BusinessProduct();
            BusinessCompany businessCompany = new BusinessCompany();
            BusinessReport businessReport = new BusinessReport();

            User guestUser = businessUser.GetGuest(userContext);

            Boolean canUserRateComment = false;
            Boolean canUserEditComment = false;
            Boolean canUserWriteComment = false;
            Boolean canUserReportInappopriate = false;
            Boolean canUserSendPrivateMessages = false;
            if (currentUser != null)
            {
                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.RateComments))
                {
                    canUserRateComment = true;
                }
                if (businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
                {
                    canUserEditComment = true;
                }
                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
                {
                    canUserWriteComment = true;
                }
                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate)
                    && !businessReport.IsMaxActiveSpamReportsReached(objectContext, currentUser))
                {
                    canUserReportInappopriate = true;
                }
                if (businessUser.IsFromUserTeam(currentUser) ||
                    businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
                {
                    canUserSendPrivateMessages = true;
                }
            }

            List<long> adminIDs = businessUser.AdminsIDsList(objectContext);

            int level = 1;

            //////
            int from = (commentsOnPage * numberOfPage) - commentsOnPage;
            int to = commentsOnPage * numberOfPage;
            int i = 0;
            /////
            /////

            IEnumerable<Comment> Comments = GetCommentsFromDB(from, to);

            if (Comments.Count<Comment>() > 0)
            {
                Panel newPanel = new Panel();
                phComments.Controls.Add(newPanel);
                newPanel.CssClass = "paddingLR4";

                foreach (Comment comment in Comments)
                {
                    ShowComment(newPanel, comment, level, canUserRateComment, canUserEditComment, canUserWriteComment,
                        canUserReportInappopriate, canUserSendPrivateMessages, currentUser, guestUser, adminIDs, i);
                    i++;
                }

            }
            else
            {
                Label noComm = new Label();
                phComments.Controls.Add(noComm);
                noComm.Text = GetLocalResourceObject("NoComments").ToString();
                noComm.CssClass = "commentsLarger";
                noComm.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, "5px;");
            }

        }

        private IEnumerable<Comment> GetCommentsFromDB(long from, long to)
        {
            BusinessComment businessComment = new BusinessComment();

            IEnumerable<Comment> Comments = businessComment.GetAllFromProduct(objectContext, currentProduct.ID,
                SortByDateDesc, SortByRating, SortByChar, from, to, SortByVariant, SortBySubVariant, SortByNoAbout);

            return Comments;
        }

        private Product GetProductFromParams()
        {
            BusinessCompany businessCompany = new BusinessCompany();

            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);

            String ProdID = Request.Params["Product"];
            Product currProduct = null;

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

                    if (!businessProduct.CheckIfProductsIsValidWithConnections(objectContext, currProduct, out notifError))
                    {
                        if (currUser != null)
                        {
                            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditProducts))
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

            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
            }

            if (currProduct == null)
            {
                throw new CommonCode.UIException("currProduct is null");
            }
            return currProduct;
        }

        protected void btnAddChar_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            PlaceHolder addChar = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "phAddChar");
            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "tbCharName");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "tbCharDescription");

            addChar.Visible = true;
            addChar.Controls.Add(lblError);
            string error = "";

            BusinessProduct businessProduct = new BusinessProduct();

            string strName = name.Text;

            if (CommonCode.Validate.ValidateName(objectContext, "prodChar", ref strName,
                Configuration.ProductsMinProductNameLength, Configuration.ProductsMaxProductNameLength,
                out error, currentProduct.ID))
            {
                string strDescr = description.Text;

                if (CommonCode.Validate.ValidateDescription(Configuration.FieldsMinDescriptionFieldLength,
                    Configuration.FieldsMaxDescriptionFieldLength, ref strDescr, "description", out error, 120))
                {

                    businessProduct.AddProductCharacteristic(userContext, objectContext, currentProduct, businessLog, currentUser, strName, strDescr);

                    description.Text = "";
                    name.Text = "";

                    changedChars = true;

                    FillDDlEditChars();
                    CheckAddCharacteristicsOptions();

                    SortCommentsBy(true, false);
                    WriteComment();

                    ShowAccordionsInfo();
                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("CharacteristicAdded").ToString());

                }
            }

            lblError.Text = error;

        }

        protected void btnAddComment_Click(object sender, EventArgs e)
        {
            Panel errPnl = new Panel();
            phComment.Controls.Add(errPnl);
            errPnl.Attributes.Add("style", "padding-left:10px;");
            errPnl.Controls.Add(lblError);

            if (CaptchaControl.Visible == true)
            {
                CaptchaControl.ValidateCaptcha(tbCaptchaMsg.Text);
                tbCaptchaMsg.Text = "";
                if (CaptchaControl.UserValidated == false)
                {
                    phComment.Visible = true;
                    lblError.Text = string.Format("{0}", GetGlobalResourceObject("SiteResources", "errorIncLetters").ToString());

                    return;
                }
            }


            BusinessUser businessUser = new BusinessUser();
            if (currentUser != null && !businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
            {
                WriteComment();
                return;
            }

            phComment.Visible = true;

            if (currentUser == null)
            {
                BusinessIpBans businessIpBans = new BusinessIpBans();
                if (businessIpBans.IsThereActiveBanForIpAdress(userContext, Request.UserHostAddress))
                {
                    lblError.Text = string.Format("{0}", GetLocalResourceObject("errIpBan"));
                    return;
                }
            }

            string error = "";

            BusinessComment businessComment = new BusinessComment();
            BusinessProduct businessProduct = new BusinessProduct();
            BusinessProductVariant bpVariant = new BusinessProductVariant();
            Comment newComment = new Comment();
            Boolean changeDone = false;

            string userName = tbName.Text;

            if (CommonCode.WebMethods.CanUserPostCommentToProduct(currentProduct, out error))
            {
                if (lblReplyTo.Attributes["edit"] == null || lblReplyTo.Attributes["edit"] == string.Empty)
                {
                    Boolean validate = true;
                    if (currentUser == null)
                    {
                        if (!string.IsNullOrEmpty(tbName.Text))
                        {
                            if (Tools.StringRangeValidatorPassed(Configuration.UsersMinUserNameLength, Configuration.UsersMaxUserNameLength, tbName.Text))
                            {
                                if (!CommonCode.Validate.ValidateUserNameFormat(ref userName, out error, false))
                                {
                                    validate = false;
                                }
                            }
                            else
                            {
                                validate = false;
                                error = string.Format("{0} {1}-{2} {3}.", GetLocalResourceObject("nameRules")
                                    , Configuration.UsersMinUserNameLength, Configuration.UsersMaxUserNameLength
                                    , GetLocalResourceObject("symbols"));
                            }
                        }
                        else
                        {
                            validate = false;
                            error = GetLocalResourceObject("errTypeName").ToString();
                        }
                    }

                    if (ddlChars.SelectedIndex > 0 && ddlAboutVariant.SelectedIndex > 0)
                    {
                        validate = false;
                        if (string.IsNullOrEmpty(error))
                        {
                            error = GetLocalResourceObject("errChoose").ToString();
                        }
                        else
                        {
                            error += "<br />" + GetLocalResourceObject("errChoose").ToString();
                        }
                    }

                    if (validate)
                    {
                        string commDescr = tbDescription.Text;

                        if (CommonCode.Validate.ValidateComment(ref commDescr, out error))
                        {

                            long subTypeID = 0;
                            CommentSubType subType;

                            if (lblReplyTo.Attributes["sub"] == null || lblReplyTo.Attributes["commID"] == null)
                            {
                                subTypeID = currentProduct.ID;
                                subType = CommentSubType.Comment;
                            }
                            else
                            {
                                subType = CommentSubType.SubComment;
                                long commid = 0;
                                long.TryParse(lblReplyTo.Attributes["commID"], out commid);
                                if (commid > 0)
                                {
                                    subTypeID = commid;
                                }
                                else
                                {
                                    throw new CommonCode.UIException("invalid comment id");
                                }
                            }

                            ProductVariant variant = null;
                            ProductSubVariant subvariant = null;
                            ProductCharacteristics prodChar = null;

                            long aboutId = 0;

                            if (ddlChars.SelectedIndex > 0)
                            {
                                if (!long.TryParse(ddlChars.SelectedValue, out aboutId))
                                {
                                    throw new CommonCode.UIException("Couldn`t parse ddlChars.SelectedValue to long.");
                                }

                                prodChar = businessProduct.GetCharacteristic(objectContext, aboutId);
                                if (prodChar == null)
                                {
                                    lblError.Text = string.Format("{0}", GetLocalResourceObject("errNoSuchChar"));
                                    FillCommDDLProdChars(currentProduct, businessProduct);
                                    return;
                                }

                            }
                            else if (ddlAboutSubVariant.SelectedIndex > 0)
                            {
                                if (!long.TryParse(ddlAboutSubVariant.SelectedValue, out aboutId))
                                {
                                    throw new CommonCode.UIException("Couldn`t parse ddlAboutSubVariant.SelectedValue to long.");
                                }

                                subvariant = bpVariant.GetSubVariant(objectContext, currentProduct, aboutId, true, false);

                                if (subvariant == null)
                                {
                                    lblError.Text = string.Format("{0}", GetLocalResourceObject("errNoSuchSubVariant"));
                                    FillCommDDLAboutVariant();
                                    return;
                                }

                            }
                            else if (ddlAboutVariant.SelectedIndex > 0)
                            {
                                if (!long.TryParse(ddlAboutVariant.SelectedValue, out aboutId))
                                {
                                    throw new CommonCode.UIException("Couldn`t parse ddlAboutVariant.SelectedValue to long.");
                                }

                                variant = bpVariant.Get(objectContext, currentProduct, aboutId, true, false);

                                if (variant == null)
                                {
                                    lblError.Text = string.Format("{0}", GetLocalResourceObject("errNoSuchVariant"));
                                    FillCommDDLAboutVariant();
                                    return;
                                }

                            }


                            businessComment.AddProductComment(userContext, objectContext, currentUser, userName, currentProduct
                                , prodChar, commDescr, businessLog, Request.UserHostAddress, subType, subTypeID
                                , variant, subvariant);

                            if (tblChars.Visible)
                            {
                                ShowCharacteristics();
                            }

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                , GetLocalResourceObject("CommentWritten").ToString());

                            changeDone = true;
                        }
                    }

                }
            }

            if (changeDone)
            {
                tbName.Text = GetLocalResourceObject("Anonymous").ToString();
                tbDescription.Text = "";

                lblReplyTo.Text = "";
                lblReplyTo.Attributes.Clear();

                if (ddlChars.Visible == true)
                {
                    ddlChars.SelectedIndex = 0;
                }
                if (ddlAboutVariant.Visible == true)
                {
                    ddlAboutVariant.SelectedIndex = 0;
                }
                if (ddlAboutSubVariant.Visible == true)
                {
                    ddlAboutSubVariant.SelectedIndex = 0;
                    ddlAboutSubVariant.Enabled = false;
                }

                CheckCommentParams(); // updates comments number

                ShowInfo();
                CheckUser();

                Comments();

                phComment.Visible = false;
            }

            lblError.Text = string.Format("{0}", error);

        }

        private void CheckAddEditProductData(bool canEditAllProducts)
        {
            BusinessUser businessUser = new BusinessUser();
            BusinessProduct businessProduct = new BusinessProduct();

            /// ---- EDIT -------

            CheckCrucialEditOptions(canEditAllProducts); // Show accordions to edit name/company/category if admin or editor (and the product is added before less than 30min)

            /// description
            TextBox tbEditDescription = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "tbAccEditDescription");
            tbEditDescription.Text = currentProduct.description;

            TextBox tbCount = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "tbSymbolsCount");
            tbCount.Text = tbEditDescription.Text.Length.ToString();

            FillDDlEditChars();

            /// new name
            Label nameInfo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditName, "lblAccProductNameRules");
            nameInfo.Text = string.Format("{0} {1}-{2} {3}.", GetLocalResourceObject("nameRules"), Configuration.ProductsMinProductNameLength
                , Configuration.ProductsMaxProductNameLength, GetLocalResourceObject("symbols"));

            TextBox tbNewName = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditName, "tbAccEditName");
            tbNewName.Text = string.Empty;

            Label lblCheckProductNewName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditName, "lblCheckProductNewName");

            ///
            Label webSite = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditWebsite, "lblEditWebSiteINfo");
            webSite.Text = GetLocalResourceObject("siteRules").ToString();

            // variants
            CheckEditVariantsOptions();

            CheckAddRemoveAlternativeNames();

            /// ----- ADD -------
            CheckAddCharacteristicsOptions();   // characteristics

            /// upload image
            CheckAddImageOptions();

            /// variants
            CheckAddVariantsOptions();
        }

        private void CheckCrucialEditOptions(bool canEditAllProducts)
        {
            bool showAccordions = false;

            if (canEditAllProducts == true)
            {
                showAccordions = true;
            }
            else
            {
                showAccordions = CheckIfTimeAfterWhichCrucialProductDataCanBeEditedDidntPassed();
            }

            if (showAccordions == true)
            {
                apEditName.Visible = true;
                apEditCompany.Visible = true;
                apEditCategory.Visible = true;

                ShowEditCompanyData(canEditAllProducts);
                ShowEditCategoryData(canEditAllProducts);

            }
            else
            {
                apEditName.Visible = false;
                apEditCompany.Visible = false;
                apEditCategory.Visible = false;
            }

        }

        private void ShowEditCategoryData(bool isAdmin)
        {

            BusinessCompany bCompany = new BusinessCompany();
            BusinessCategory bCategory = new BusinessCategory();

            Category unspecified = bCategory.GetUnspecifiedCategory(objectContext);
            Company other = bCompany.GetOther(objectContext);

            if (!currentProduct.CompanyReference.IsLoaded)
            {
                currentProduct.CompanyReference.Load();
            }
            if (!currentProduct.CategoryReference.IsLoaded)
            {
                currentProduct.CategoryReference.Load();
            }


            Panel pnlEditAdmin = (Panel)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "pnlSubAdmEditCategory");
            Panel pnlEditEditorWhenOther = (Panel)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "pnlSubEditorEditCategoryWhenCompOther");
            Panel pnlEditEditor = (Panel)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "pnlSubEditorEditCategory");

            if (isAdmin == true)
            {   // ADMIN PART

                if (currentProduct.visible == true)
                {
                    pnlEditAdmin.Visible = true;
                    pnlEditEditorWhenOther.Visible = false;
                    pnlEditEditor.Visible = false;
                }
                else
                {
                    apEditCategory.Visible = false;
                }
            }
            else
            {   // Editor part

                if (currentProduct.Company.ID == other.ID)
                {
                    pnlEditAdmin.Visible = false;
                    pnlEditEditorWhenOther.Visible = true;
                    pnlEditEditor.Visible = false;

                    if (IsPostBack == false)
                    {
                        LoadChooseCategoryMenuWithItems();
                    }

                }
                else
                {
                    List<Category> companyCategories = bCompany.GetCompanyCategories(objectContext, currentProduct.Company.ID).ToList();

                    companyCategories.Remove(currentProduct.Category);
                    companyCategories.Remove(unspecified);

                    if (companyCategories.Count > 0)
                    {
                        pnlEditAdmin.Visible = false;
                        pnlEditEditorWhenOther.Visible = false;
                        pnlEditEditor.Visible = true;

                        if (IsPostBack == false)
                        {
                            FillChooseCategoryDdl();
                        }
                    }
                    else
                    {
                        apEditCategory.Visible = false;
                    }
                }

            }

        }

        private void FillChooseCategoryDdl()
        {

            BusinessCompany bCompany = new BusinessCompany();
            BusinessCategory bCategory = new BusinessCategory();

            Category unspecified = bCategory.GetUnspecifiedCategory(objectContext);
            Company other = bCompany.GetOther(objectContext);

            if (!currentProduct.CompanyReference.IsLoaded)
            {
                currentProduct.CompanyReference.Load();
            }
            if (!currentProduct.CategoryReference.IsLoaded)
            {
                currentProduct.CategoryReference.Load();
            }

            if (currentProduct.Company.ID == other.ID)
            {
                throw new CommonCode.UIException("product`s company is OTHER, the function FillChooseCategoryDdl shouldn`t be called!");
            }

            List<Category> companyCategories = bCompany.GetCompanyCategories(objectContext, currentProduct.Company.ID).ToList();

            DropDownList ddlCategories = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "ddlChangeCategory");
            ddlCategories.Items.Clear();

            if (companyCategories.Count<Category>() > 0)
            {
                ListItem firstItem = new ListItem();
                firstItem.Text = GetLocalResourceObject("choose...").ToString();
                firstItem.Value = "0";
                ddlCategories.Items.Add(firstItem);

                foreach (Category category in companyCategories)
                {
                    if (category.ID != unspecified.ID && category.ID != currentProduct.Category.ID)
                    {
                        ListItem newItem2 = new ListItem();
                        newItem2.Text = Tools.TrimString(Tools.CategoryName(objectContext, category, false), 67, true, true);
                        newItem2.Value = category.ID.ToString();
                        ddlCategories.Items.Add(newItem2);
                    }
                }

            }
            else
            {
                // company doesnt have added categories
                throw new CommonCode.UIException(string.Format("companyCategories.Count<Category>() is < 1, this function shouldn`t be called in this case!"));
            }
        }


        private void LoadChooseCategoryMenuWithItems()
        {
            BusinessCategory businessCategory = new BusinessCategory();
            BusinessCompany businessCompany = new BusinessCompany();

            Menu menu = (Menu)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "menuChooseNewCategory");
            menu.Items.Clear();

            string browserType = Request.Browser.Type.ToUpper();
            if (browserType.Contains("IE") && browserType != "IE5" && browserType != "IE9")  // Because chrome is detected as IE5
            {
                menu.DynamicHorizontalOffset = -1;
                menu.DynamicVerticalOffset = -2;
            }

            List<Category> categories = businessCategory.GetAllByParentCategoryID(objectContext, null);

            IList<long> addedCategoryIDs = new List<long>();


            /////////
            if (!currentProduct.CategoryReference.IsLoaded)
            {
                currentProduct.CategoryReference.Load();
            }
            addedCategoryIDs.Add(currentProduct.Category.ID);

            Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);
            addedCategoryIDs.Add(unspecified.ID);
            /////////

            foreach (Category category in categories)
            {
                Boolean forbidden = false;

                if (addedCategoryIDs.Contains(category.ID))
                {
                    forbidden = true;
                }

                if (!forbidden)
                {
                    MenuItem menuItem = new MenuItem();

                    menuItem.Value = category.ID.ToString();

                    if (category.last == true)
                    {
                        menuItem.Selectable = true;
                        menuItem.Text = CommonCode.UiTools.HackNavigationMenu(category.name, true, true);
                    }
                    else
                    {
                        menuItem.Selectable = false;
                        menuItem.Text = CommonCode.UiTools.HackNavigationMenu(category.name, false, true);

                    }

                    menu.Items.Add(menuItem);

                    if (addedCategoryIDs.Contains(category.ID))
                    {
                        throw new InvalidOperationException(string.Format(
                            "The category with ID = {0} is already included in the menu.", category.ID));
                    }
                    else
                    {
                        addedCategoryIDs.Add(category.ID);
                    }

                    if (category.last == false)
                    {
                        AddChildMenuItems(menuItem, category.ID, addedCategoryIDs);
                    }
                }
            }
        }


        private void AddChildMenuItems(MenuItem menuItem, long parentCategoryID, IList<long> addedCategoryIDs)
        {
            if (menuItem == null)
            {
                throw new ArgumentNullException("menuItem");
            }
            if (addedCategoryIDs == null)
            {
                throw new ArgumentNullException("addedCategoryIDs");
            }
            BusinessCategory businessCategory = new BusinessCategory();
            List<Category> categories = businessCategory.GetAllByParentCategoryID(objectContext, parentCategoryID);
            foreach (Category category in categories)
            {
                // if category is already added to company to not be shown
                Boolean forbidden = false;

                if (addedCategoryIDs.Contains(category.ID))
                {
                    forbidden = true;
                }

                if (!forbidden)
                {
                    MenuItem childMenuItem = new MenuItem();

                    childMenuItem.Value = category.ID.ToString();

                    if (category.last == true)
                    {
                        childMenuItem.Selectable = true;
                        childMenuItem.Text = CommonCode.UiTools.HackNavigationMenu(category.name, true, false);
                    }
                    else
                    {
                        childMenuItem.Selectable = false;
                        childMenuItem.Text = CommonCode.UiTools.HackNavigationMenu(category.name, false, false);
                    }

                    menuItem.ChildItems.Add(childMenuItem);

                    if (addedCategoryIDs.Contains(category.ID))
                    {
                        throw new InvalidOperationException(string.Format(
                            "The category with ID = {0} is already included in the menu.", category.ID));
                    }
                    else
                    {
                        addedCategoryIDs.Add(category.ID);
                    }


                    if (category.last == false)
                    {
                        AddChildMenuItems(childMenuItem, category.ID, addedCategoryIDs);
                    }
                }
            }
        }

        private bool CheckIfTimeAfterWhichCrucialProductDataCanBeEditedDidntPassed()
        {
            bool result = false;

            TimeSpan span = new TimeSpan();
            span = DateTime.UtcNow - currentProduct.dateCreated;
            if (span.TotalMinutes < Configuration.ProdCompMinAfterWhichUserCannotEditCrucialData)
            {
                result = true;
            }

            return result;
        }


        private void ShowEditCompanyData(bool isAdmin)
        {
            if (!currentProduct.CompanyReference.IsLoaded)
            {
                currentProduct.CompanyReference.Load();
            }
            if (!currentProduct.CategoryReference.IsLoaded)
            {
                currentProduct.CategoryReference.Load();
            }

            BusinessCompany bCompany = new BusinessCompany();
            BusinessCategory bCategory = new BusinessCategory();

            Category unspecified = bCategory.GetUnspecifiedCategory(objectContext);
            Company other = bCompany.GetOther(objectContext);

            if (currentProduct.Category.ID != unspecified.ID)
            {

                Panel pnlEditAdmin = (Panel)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "pnlAdminChangeCompany");
                Panel pnlEditEditor = (Panel)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "pnlEditorChangeCompany");

                if (isAdmin == true)    // ADMIN PART
                {
                    pnlEditAdmin.Visible = true;
                    pnlEditEditor.Visible = false;

                    Panel pnlAdmSubRemove = (Panel)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "pnlAdmSubRemCompany");
                    Panel pnlAdmSubChangeComp = (Panel)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "pnlAdmSubChangeCompany");

                    if (currentProduct.Company.ID != other.ID && currentProduct.Category.ID != unspecified.ID)
                    {
                        pnlAdmSubRemove.Visible = true;
                    }
                    else
                    {
                        pnlAdmSubRemove.Visible = false;
                    }

                    if (currentProduct.visible)
                    {

                        if (currentProduct.Category.ID == unspecified.ID)
                        {
                            pnlAdmSubChangeComp.Visible = false;
                        }
                        else
                        {
                            pnlAdmSubChangeComp.Visible = true;
                        }

                    }
                    else
                    {
                        pnlAdmSubChangeComp.Visible = false;
                    }


                }
                else          // EDITOR PART
                {
                    pnlEditAdmin.Visible = false;
                    pnlEditEditor.Visible = true;

                    // The accordion is not shown if there are no other companies who can have products in this category!
                    List<Company> companyList = bCompany.GetCompaniesByCategory(objectContext, currentProduct.Category.ID).ToList();
                    if (companyList.Contains(currentProduct.Company) == true)
                    {
                        companyList.Remove(currentProduct.Company);
                    }

                    if (companyList.Count > 0)
                    {
                        if (IsPostBack == false)
                        {
                            FillChangeCompanyDDl();
                        }

                    }
                    else
                    {
                        apEditCompany.Visible = false;
                    }

                }

            }
            else
            {
                apEditCompany.Visible = false;
            }



        }

        protected void dbEditorUpdateCompany_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            if (CheckIfTimeAfterWhichCrucialProductDataCanBeEditedDidntPassed() == false)
            {
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Time passed!");
                return;
            }

            PlaceHolder phChngComp = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "phEditorUpdCompError");

            phChngComp.Visible = true;
            phChngComp.Controls.Add(lblError);
            string error = "";

            BusinessProduct businessProduct = new BusinessProduct();
            BusinessCompany businessCompany = new BusinessCompany();
            BusinessCategory businessCategory = new BusinessCategory();

            Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);
            if (!currentProduct.CategoryReference.IsLoaded)
            {
                currentProduct.CategoryReference.Load();
            }

            if (unspecified == currentProduct.Category)
            {
                return;
            }

            DropDownList ddlCompanies = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "ddlEditorChangeCompany");
            if (ddlCompanies.Items.Count < 1)
            {
                throw new CommonCode.UIException(string.Format("User id : {0}, cannot change product id : {1} company, because ddlEditorChangeCompany is empty after button click."
                    , currentUser.ID, currentProduct.ID));
            }

            if (ddlCompanies.SelectedIndex > 0)
            {

                long compId = -1;
                if (!long.TryParse(ddlCompanies.SelectedValue, out compId))
                {
                    throw new CommonCode.UIException("Couldnt parse ddlEditorChangeCompany.SelectedValue to long");
                }

                Company newCompany = businessCompany.GetCompany(objectContext, compId);

                if (newCompany == null)
                {
                    CheckUser();
                    FillChangeCompanyDDl();
                    return;
                }


                if (businessCompany.CheckIfCompanyCanHaveProductsInCategory(objectContext, newCompany, currentProduct))
                {
                    businessProduct.ChangeProductCompany(objectContext, currentProduct, newCompany, currentUser, businessLog);

                    CheckCrucialEditOptions(false);
                    FillChangeCompanyDDl();
                    FillChooseCategoryDdl();
                    LoadChooseCategoryMenuWithItems();
                    ShowInfo();

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("companyUpdated").ToString());
                }
                else
                {
                    CheckUser();
                    FillChangeCompanyDDl();
                    return;
                }


            }
            else
            {
                error = GetLocalResourceObject("errChooseNewCompany").ToString();
            }

            lblError.Text = error;
            tbChngCompany.Text = "";


        }

        private void FillChangeCompanyDDl()
        {
            DropDownList list = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "ddlEditorChangeCompany");

            list.Items.Clear();

            BusinessCompany businessCompany = new BusinessCompany();

            if (!currentProduct.CategoryReference.IsLoaded)
            {
                currentProduct.CategoryReference.Load();
            }
            if (!currentProduct.CompanyReference.IsLoaded)
            {
                currentProduct.CompanyReference.Load();
            }

            List<Company> CompanyList = businessCompany.GetCompaniesByCategory(objectContext, currentProduct.Category.ID).ToList();

            if (CompanyList.Contains(currentProduct.Company) == true)
            {
                CompanyList.Remove(currentProduct.Company);
            }

            if (CompanyList.Count<Company>() == 0)
            {
                throw new CommonCode.UIException("no companies in list");
            }

            ListItem firstItem = new ListItem();
            firstItem.Text = GetLocalResourceObject("choose...").ToString();
            firstItem.Value = "0";
            list.Items.Add(firstItem);

            foreach (Company company in CompanyList)
            {
                ListItem newItem2 = new ListItem();
                newItem2.Value = company.ID.ToString();
                newItem2.Text = company.name;
                list.Items.Add(newItem2);
            }

        }


        private void CheckAddRemoveAlternativeNames()
        {
            BusinessAlternativeNames bAN = new BusinessAlternativeNames();

            int namesCount = bAN.CountAlternativeProductNames(objectContext, currentProduct, true);

            if (namesCount < Configuration.ProductsMaxAlternativeNames)
            {
                apAddAlternativeName.Visible = true;
            }
            else
            {
                apAddAlternativeName.Visible = false;
            }

            if (namesCount > 0)
            {
                apRemoveAlternativeNames.Visible = true;

                if (IsPostBack == false)
                {
                    FillCblRemoveAlternativeNames();
                }
            }
            else
            {
                apRemoveAlternativeNames.Visible = false;
            }

        }

        private void FillCblRemoveAlternativeNames()
        {
            CheckBoxList cblRemoveAlternativeNames = (CheckBoxList)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "cblRemoveAlternativeNames");
            cblRemoveAlternativeNames.Items.Clear();

            BusinessAlternativeNames bAN = new BusinessAlternativeNames();

            List<AlternativeProductName> names = bAN.GetVisibleAlternativeProductNames(objectContext, currentProduct);
            if (names != null && names.Count > 0)
            {
                apRemoveAlternativeNames.Visible = true;

                foreach (AlternativeProductName name in names)
                {
                    ListItem item = new ListItem();
                    cblRemoveAlternativeNames.Items.Add(item);
                    item.Text = name.name;
                    item.Value = name.ID.ToString();
                    item.Selected = true;
                }
            }
            else
            {
                apRemoveAlternativeNames.Visible = false;
            }

        }

        private void CheckEditVariantsOptions()
        {
            BusinessProductVariant bpVariants = new BusinessProductVariant();

            int variantsCount = bpVariants.CountVariants(objectContext, currentProduct);

            if (variantsCount > 0)
            {
                apEditVariant.Visible = true;

                Label editVariantInfo1 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "lblEditVariantInfo1");

                editVariantInfo1.Text = string.Format("{0} {1}-{2} {3}.<br />{4}", GetLocalResourceObject("DescrRules")
                    , 0, Configuration.FieldsMaxDescriptionFieldLength, GetLocalResourceObject("symbols")
                    , GetLocalResourceObject("variantRules"));

                DropDownList ddlEditVariant = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "ddlEditVariant");
                ddlEditVariant.Items.Clear();

                List<ProductVariant> variants = bpVariants.GetVisibleVariants(objectContext, currentProduct);
                if (variants.Count == 0)
                {
                    throw new CommonCode.UIException(string.Format("There are no visible variants for product id : {0}, user id : {1}"
                        , currentProduct.ID, currentUser.ID));
                }

                ListItem firstItem = new ListItem();
                ddlEditVariant.Items.Add(firstItem);
                firstItem.Text = GetLocalResourceObject("choose...").ToString();
                firstItem.Value = "0";

                foreach (ProductVariant variant in variants)
                {
                    ListItem newItem = new ListItem();
                    ddlEditVariant.Items.Add(newItem);
                    newItem.Text = variant.name;
                    newItem.Value = variant.ID.ToString();
                }

                /// edit sub variants

                Label editSubVariantInfo1 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "lblEditSubVariantInfo1");

                editSubVariantInfo1.Text = string.Format("{0} {1}-{2} {3}.", GetLocalResourceObject("DescrRules")
                    , 0, Configuration.FieldsMaxDescriptionFieldLength, GetLocalResourceObject("symbols"));

                DropDownList ddlEditSubVariant = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "ddlEditSubVariant");
                ddlEditSubVariant.Items.Clear();

                int countSubVariants = 0;

                ListItem firstSubVariant = new ListItem();
                ddlEditSubVariant.Items.Add(firstSubVariant);
                firstSubVariant.Text = GetLocalResourceObject("choose...").ToString();
                firstSubVariant.Value = "0";

                List<ProductSubVariant> subVariants = new List<ProductSubVariant>();
                foreach (ProductVariant variant in variants)
                {
                    subVariants = bpVariants.GetVisibleSubVariants(objectContext, variant);
                    if (subVariants.Count > 0)
                    {
                        foreach (ProductSubVariant subVariant in subVariants)
                        {
                            ListItem newItem = new ListItem();
                            ddlEditSubVariant.Items.Add(newItem);
                            newItem.Text = string.Format("{0} : {1}", variant.name, subVariant.name);
                            newItem.Value = subVariant.ID.ToString();

                            countSubVariants++;
                        }
                    }
                }

                if (countSubVariants > 0)
                {
                    apEditSubVariant.Visible = true;
                }
                else
                {
                    apEditSubVariant.Visible = false;
                }

            }
            else
            {
                apEditVariant.Visible = false;
                apEditSubVariant.Visible = false;
            }

        }

        private void CheckAddVariantsOptions()
        {
            BusinessProductVariant bpVariants = new BusinessProductVariant();

            int variantsCount = bpVariants.CountVariants(objectContext, currentProduct);

            if (variantsCount < Configuration.ProductsMaxVariants)
            {
                apAddVariant.Visible = true;

                Label addVariantInfo1 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddVariant, "lblAddVariantInfo1");
                Label addVariantInfo2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddVariant, "lblAddVariantInfo2");

                addVariantInfo1.Text = string.Format("{0} {1}-{2} {3}.", GetLocalResourceObject("nameRules")
                    , Configuration.ProductsMinProductNameLength, Configuration.ProductsMaxProductNameLength
                    , GetLocalResourceObject("symbols"));
                addVariantInfo2.Text = string.Format("{0} {1}-{2} {3}.", GetLocalResourceObject("DescrRules")
                    , 0, Configuration.FieldsMaxDescriptionFieldLength, GetLocalResourceObject("symbols"));
            }
            else
            {
                apAddVariant.Visible = false;
            }

            if (variantsCount > 0)
            {
                if (bpVariants.CountSubVariants(objectContext, currentProduct) < Configuration.ProductsMaxVariantSubVariants)
                {
                    apAddSubVariant.Visible = true;

                    Label addSubVariantInfo1 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "lblAddSubVariantInfo1");
                    Label addSubVariantInfo2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "lblAddSubVariantInfo2");

                    addSubVariantInfo1.Text = string.Format("{0} {1}-{2} {3}.", GetLocalResourceObject("nameRules")
                    , Configuration.ProductsMinProductNameLength, Configuration.ProductsMaxProductNameLength
                    , GetLocalResourceObject("symbols"));

                    addSubVariantInfo2.Text = string.Format("{0} {1}-{2} {3}.", GetLocalResourceObject("DescrRules")
                        , 0, Configuration.FieldsMaxDescriptionFieldLength, GetLocalResourceObject("symbols"));

                    DropDownList ddlVariants = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "ddlAddVariant");
                    ddlVariants.Items.Clear();

                    List<ProductVariant> variants = bpVariants.GetVisibleVariants(objectContext, currentProduct);
                    if (variants.Count == 0)
                    {
                        throw new CommonCode.UIException(string.Format("There are no visible variants for product id : {0}, user id : {1}"
                            , currentProduct.ID, currentUser.ID));
                    }

                    ListItem firstItem = new ListItem();
                    ddlVariants.Items.Add(firstItem);
                    firstItem.Text = GetLocalResourceObject("choose...").ToString();
                    firstItem.Value = "0";

                    foreach (ProductVariant variant in variants)
                    {
                        ListItem newItem = new ListItem();
                        ddlVariants.Items.Add(newItem);
                        newItem.Text = variant.name;
                        newItem.Value = variant.ID.ToString();
                    }
                }
                else
                {
                    apAddSubVariant.Visible = false;
                }
            }
            else
            {
                apAddSubVariant.Visible = false;
            }

        }

        private void CheckAddCharacteristicsOptions()
        {
            BusinessProduct businessProduct = new BusinessProduct();

            if (businessProduct.CountProductCharacteristics(objectContext, currentProduct) < 20)
            {
                apAddCharacteristic.Visible = true;

                TextBox tbChar = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "tbCharName");
                tbChar.Attributes.Add("onblur", string.Format("JSCheckData('{0}','productCharAdd','{1}','{2}');",
                    tbChar.ClientID, lblCheckCharName.ClientID, currentProduct.ID.ToString()));

                Label charInfo1 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblAddCharInfo1");
                Label charInfo2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblAddCharInfo2");

                charInfo1.Text = string.Format("{0} {1}-{2} {3}. {4}"
                    , GetLocalResourceObject("CharTopicRules")
                    , Configuration.ProductsMinProductNameLength, Configuration.ProductsMaxProductNameLength
                    , GetLocalResourceObject("symbols"), GetLocalResourceObject("MaxCharsAllowed"));

                charInfo2.Text = string.Format("{0} {1}-{2} {3}.", GetLocalResourceObject("DescrRules"),
                    Configuration.FieldsMinDescriptionFieldLength, Configuration.FieldsMaxDescriptionFieldLength
                    , GetLocalResourceObject("symbols"));
            }
            else
            {
                apAddCharacteristic.Visible = false;
            }
        }

        private void FillDDlEditChars()
        {
            BusinessProduct businessProduct = new BusinessProduct();

            List<ProductCharacteristics> prodChars =
                       businessProduct.GetAllProductCharacteristics(objectContext, currentProduct.ID, false).ToList();

            if (prodChars.Count<ProductCharacteristics>() > 0)
            {
                apEditCharacteristics.Visible = true;

                DropDownList ddlEditCharacteristics = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "ddlEditCharacteristics");
                ddlEditCharacteristics.Items.Clear();

                Label charInfo1 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblCharInfo1");
                Label charInfo2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblCharInfo2");
                TextBox charDescription = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbAccEditCharDescription");
                TextBox charName = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbAccCharName");


                foreach (ProductCharacteristics prodChar in prodChars)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = prodChar.name;
                    newItem.Value = prodChar.ID.ToString();
                    ddlEditCharacteristics.Items.Add(newItem);
                }

                long selectedValue = -1;
                if (long.TryParse(ddlEditCharacteristics.SelectedValue, out selectedValue))
                {
                    foreach (ProductCharacteristics characterisitc in prodChars)
                    {
                        if (characterisitc.ID == selectedValue)
                        {
                            charDescription.Text = characterisitc.description;
                            break;
                        }
                    }
                }
                else
                {
                    throw new BusinessException(string.Format("Couldnt Parse ddEdit.SelectedValue to long , user id = {0}", currentUser.ID));
                }

                charInfo1.Text = GetLocalResourceObject("CharsInfo").ToString();

                charInfo2.Text = string.Format("{0} {1}-{2} {3}. {4} {5}-{6} {3}."
                    , GetLocalResourceObject("CharTopicRules")
                    , Configuration.ProductsMinProductNameLength, Configuration.ProductsMaxProductNameLength
                    , GetLocalResourceObject("symbols"), GetLocalResourceObject("DescrRules")
                    , Configuration.FieldsMinDescriptionFieldLength, Configuration.FieldsMaxDescriptionFieldLength);

                charName.Attributes.Add("onblur", string.Format("JSCheckData('{0}','productCharAdd','{1}','{2}');",
                    charName.ClientID, lblAccCharNewName.ClientID, currentProduct.ID.ToString()));
            }
            else
            {
                apEditCharacteristics.Visible = false;
            }
        }

        private void HidePlaceHolders()
        {
            phAddChar.Visible = false;
            phChangeCompany.Visible = false;
            phComment.Visible = false;
            phImage.Visible = false;
            phRoles.Visible = false;
            phChangeCategory.Visible = false;
        }

        protected void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllProductsFromEvents();

            if (currentProduct.visible == true)
            {

                BusinessCategory bCategory = new BusinessCategory();
                Category unspecified = bCategory.GetUnspecifiedCategory(objectContext);

                if (!currentProduct.CategoryReference.IsLoaded)
                {
                    currentProduct.CategoryReference.Load();
                }

                if (currentProduct.Category.ID != unspecified.ID)
                {
                    BusinessProduct businessProduct = new BusinessProduct();
                    businessProduct.DeleteProduct(objectContext, userContext, currentProduct, currentUser, businessLog);

                    btnDeleteProduct.Enabled = false;
                    btnMakeVisible.Enabled = true;

                    CheckUser();
                    ShowProductInfo();

                    UpdateProductNotification();
                    ShowNotification();
                }
                else
                {
                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "You cannot detele products which are in 'Unspecified'"
                    + " category. Move the product to other category and then delete it.");
                }
            }
        }

        protected void btnMakeVisible_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllProductsFromEvents();

            if (currentProduct.visible == false)
            {
                BusinessProduct businessProduct = new BusinessProduct();
                businessProduct.MakeVisibleProduct(objectContext, currentProduct, currentUser, businessLog);

                btnDeleteProduct.Enabled = true;
                btnMakeVisible.Enabled = false;

                CheckUser();
                ShowProductInfo();

                UpdateProductNotification();
                ShowNotification();
            }
        }


        protected void btnChngCompany_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllProductsFromEvents();

            PlaceHolder phChngComp = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditCompany, "phChangeCompany");

            phChngComp.Visible = true;
            phChngComp.Controls.Add(lblError);
            string error = "";

            BusinessProduct businessProduct = new BusinessProduct();
            BusinessCompany businessCompany = new BusinessCompany();

            if (currentProduct.visible)
            {
                String newCompID = tbChngCompany.Text;

                if (CommonCode.Validate.ValidateLong(newCompID, out error))
                {
                    long compId = -1;
                    long.TryParse(newCompID, out compId);

                    Company newCompany = businessCompany.GetCompany(objectContext, compId);

                    if (newCompany == null)
                    {
                        error = ("No such company (or it is not visible).");
                    }
                    else
                    {

                        BusinessCategory businessCategory = new BusinessCategory();
                        Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);
                        if (!currentProduct.CategoryReference.IsLoaded)
                        {
                            currentProduct.CategoryReference.Load();
                        }

                        if (unspecified != currentProduct.Category)
                        {
                            if (businessCompany.CheckIfCompanyCanHaveProductsInCategory(objectContext, newCompany, currentProduct))
                            {
                                businessProduct.ChangeProductCompany(objectContext, currentProduct, newCompany, currentUser, businessLog);

                                UpdateProductNotification();
                                //CheckUser();
                                CheckCrucialEditOptions(true);
                                ShowInfo();

                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Company changed!");
                            }
                            else
                            {
                                error = ("That company cannot have products in this product`s category.");
                            }
                        }
                        else
                        {
                            error = "You cannot change companies of products which are in 'Unspecified' category, first move the product to some other category.";
                        }
                    }
                }
            }
            else
            {
                error = "Current product is not visible.";
            }

            lblError.Text = error;
            tbChngCompany.Text = "";
        }


        public void ReplyToComment_Click(object sender, EventArgs e)
        {
            BusinessComment businessComment = new BusinessComment();
            BusinessUser businessUser = new BusinessUser();
            User currUser = currentUser;
            if (currUser == null)
            {
                currUser = businessUser.GetGuest(userContext);
            }
            else
            {
                if (!businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteCommentsAndMessages))
                {
                    throw new CommonCode.UIException(string.Format("User id = {0} , cannot write (Reply to) comments", currUser.ID));
                }
            }

            Button btnReply = sender as Button;
            if (btnReply != null)
            {
                TableCell tblCell = btnReply.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long commID = -1;
                        string commentIdStr = tblRow.Attributes["commentID"];
                        long.TryParse(commentIdStr, out commID);

                        if (commID > 0)
                        {
                            Comment parentComm = businessComment.Get(objectContext, commID);
                            if (parentComm != null)
                            {
                                String commName = "";
                                if (parentComm.guestname == null)
                                {
                                    if (parentComm.UserIDReference.IsLoaded == false)
                                    {
                                        parentComm.UserIDReference.Load();
                                    }

                                    commName = Tools.GetUserFromUserDatabase(parentComm.UserID).username;
                                }
                                else
                                {
                                    commName = parentComm.guestname;
                                }
                                String date = CommonCode.UiTools.DateTimeToLocalShortDateString(parentComm.dateCreated);

                                lblReply.Visible = true;
                                btnCancel.Visible = true;
                                lblReplyTo.Visible = true;
                                lblReplyTo.Text = (commName + " , " + date);
                                lblReplyTo.Attributes.Clear();
                                lblReplyTo.Attributes.Add("commID", parentComm.ID.ToString());
                                lblReplyTo.Attributes.Add("sub", "yes");

                                lblCC.Visible = false;
                                ddlChars.Visible = false;


                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format("There`s no comment ID = {0}", commID));
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("tblRow.Attributes['commentID'] is < 1 , user id = {0}", currUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldnt get parent row.");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Couldnt get parent cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button.");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Visible = false;
            lblReply.Visible = false;
            lblReplyTo.Visible = false;
            lblReplyTo.Attributes.Clear();

            WriteComment();
        }


        private void CheckCommentParams()
        {
            BusinessProduct businessProduct = new BusinessProduct();

            // num - for comments number , page - for page number
            String strNum = Request.Params["num"];
            String strPage = Request.Params["page"];

            long commNumber = 0;

            String strChar = Request.Params["char"];
            String strVarId = Request.Params["variant"];
            String strSubVarId = Request.Params["subvariant"];
            String strNoAbout = Request.Params["noabout"];

            long typeId = 0;

            if (string.IsNullOrEmpty(strChar) && string.IsNullOrEmpty(strVarId)
                && string.IsNullOrEmpty(strSubVarId) && string.IsNullOrEmpty(strNoAbout))
            {
                commNumber = businessProduct.CountProductComments(objectContext, currentProduct);
            }
            else if (!string.IsNullOrEmpty(strNoAbout))
            {
                if (strNoAbout == "true")
                {
                    commNumber = businessProduct.CountProductNoAboutComments(objectContext, currentProduct);
                }
                else
                {
                    throw new CommonCode.UIException("strNoAbout param is != true");
                }
            }
            else if (!string.IsNullOrEmpty(strChar))
            {
                if (long.TryParse(strChar, out typeId))
                {
                    commNumber = businessProduct.CountProductCharComments(objectContext, typeId);
                }
                else
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse Request.Params['char'] = {0} to long", strChar));
                }
            }
            else if (!string.IsNullOrEmpty(strVarId))
            {
                BusinessProductVariant bpVariant = new BusinessProductVariant();

                if (long.TryParse(strVarId, out typeId))
                {
                    commNumber = bpVariant.CountCommentsForVariant(objectContext, typeId);
                }
                else
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse Request.Params['variant'] = {0} to long", strVarId));
                }
            }
            else if (!string.IsNullOrEmpty(strSubVarId))
            {
                BusinessProductVariant bpVariant = new BusinessProductVariant();

                if (long.TryParse(strSubVarId, out typeId))
                {
                    commNumber = bpVariant.CountCommentsForSubVariant(objectContext, typeId);
                }
                else
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse Request.Params['subvariant'] = {0} to long", strSubVarId));
                }
            }

            CommNumber = commNumber;

            if (commNumber > 0)
            {
                if (!string.IsNullOrEmpty(strNum) && strNum.Length > 0)
                {
                    int num = 0;
                    int.TryParse(strNum, out num);
                    if (num == minCommOnPage || num == defCommOnPage || num == maxCommOnPage)
                    {

                        if (strPage != null && strPage.Length > 0)
                        {
                            int page = 0;
                            int.TryParse(strPage, out page);
                            if (page > 1)
                            {
                                int expression = num * (page - 1);
                                if (commNumber > expression)
                                {
                                    // oK
                                    numberOfPage = page;
                                    commentsOnPage = num;
                                }
                                else
                                {
                                    RedirectToCurrProductPage();
                                }
                            }
                            else if (page < 0)
                            {
                            }
                            else
                            {
                                //  OK
                                // num is valid , page is 1
                                commentsOnPage = num;
                            }
                        }
                        else
                        {
                            // OK
                            // num is valid
                            commentsOnPage = num;
                        }

                    }
                    else
                    {
                        RedirectToCurrProductPage();
                    }
                }
                else if (!string.IsNullOrEmpty(strPage) && strPage.Length > 0)  // nqma num
                {
                    int page = 0;
                    int.TryParse(strPage, out page);
                    if (page > 1)
                    {
                        int expression = (int)commNumber * (page - 1);
                        if (commNumber > expression)
                        {
                            // page is valid
                            numberOfPage = page;
                        }
                    }
                    else if (page < 0)
                    {
                        RedirectToCurrProductPage();
                    }
                }
            }
            else
            {
                if (strNum != null && strNum.Length > 0)
                {
                    RedirectToCurrProductPage();
                }

                if (strPage != null && strPage.Length > 0)
                {
                    RedirectToCurrProductPage();
                }
            }

        }

        private void RateProductItems(int selectedIndex)
        {
            ddlRateProduct.Items.Clear();

            ListItem zeroItem = new ListItem();
            zeroItem.Text = GetLocalResourceObject("choose...").ToString();
            zeroItem.Value = "0";
            ddlRateProduct.Items.Add(zeroItem);

            ListItem firstItem = new ListItem();
            firstItem.Text = Tools.GetConfigurationResource("RateProductLvlOneRate");
            firstItem.Value = "1";
            ddlRateProduct.Items.Add(firstItem);

            ListItem secondItem = new ListItem();
            secondItem.Text = Tools.GetConfigurationResource("RateProductLvlTwoRate");
            secondItem.Value = "2";
            ddlRateProduct.Items.Add(secondItem);

            ListItem thirdItem = new ListItem();
            thirdItem.Text = Tools.GetConfigurationResource("RateProductLvlThreeRate");
            thirdItem.Value = "3";
            ddlRateProduct.Items.Add(thirdItem);

            ListItem forthItem = new ListItem();
            forthItem.Text = Tools.GetConfigurationResource("RateProductLvlFourRate");
            forthItem.Value = "4";
            ddlRateProduct.Items.Add(forthItem);

            ListItem fifthItem = new ListItem();
            fifthItem.Text = Tools.GetConfigurationResource("RateProductLvlFiveRate");
            fifthItem.Value = "5";
            ddlRateProduct.Items.Add(fifthItem);

            ListItem sixthItem = new ListItem();
            sixthItem.Text = Tools.GetConfigurationResource("RateProductLvlSixRate");
            sixthItem.Value = "6";
            ddlRateProduct.Items.Add(sixthItem);

            ddlRateProduct.SelectedIndex = selectedIndex;
            ddlRateProduct.SelectedIndexChanged += new EventHandler(RateProduct_Click);
            ddlRateProduct.AutoPostBack = true;


        }

        protected void RateProduct_Click(object sender, EventArgs e)
        {
            BusinessComment businessComment = new BusinessComment();
            BusinessUser businessUser = new BusinessUser();
            BusinessProduct businessProduct = new BusinessProduct();
            BusinessRating businessRating = new BusinessRating();

            if (currentUser == null || !businessUser.CanUserDo(userContext, currentUser, UserRoles.RateProducts))
            {
                if (currentUser != null)
                {
                    RedirectToSameUrl(Request.Url.ToString());
                    return;
                }
                else
                {
                    RedirectToSameUrl(Request.Url.ToString());
                    return;
                }
            }

            int rate = 0;
            int.TryParse(ddlRateProduct.SelectedValue, out rate);
            if ((rate > 6) || (rate < 1))
            {
                throw new CommonCode.UIException(string.Format("ddlRateProduct.SelectedValue = {0} is invalid index , user id = {1}", rate, currentUser.ID));
            }

            businessRating.RateProduct(objectContext, currentUser, currentProduct, rate, businessLog);

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("ProductRated").ToString());
            ShowInfo();
            CheckUser();
        }

        protected void FillTblAllCompanyProductsModificators()
        {
            tblAllCompProdModificators.Rows.Clear();

            BusinessUser businessUser = new BusinessUser();
            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

            if (!currentProduct.CompanyReference.IsLoaded)
            {
                currentProduct.CompanyReference.Load();
            }

            IEnumerable<UsersTypeAction> usersAction =
                businessUserTypeActions.GetAllCompanyProductsModificators(objectContext, currentProduct.Company.ID);
            if (usersAction.Count<UsersTypeAction>() > 0)
            {
                tblAllCompProdModificators.Visible = true;
                lblProdCompEditors.Visible = true;

                TableRow newRow = new TableRow();

                TableCell idCell = new TableCell();
                idCell.Text = "Id";
                idCell.Width = Unit.Pixel(50);
                newRow.Cells.Add(idCell);

                TableCell nameCell = new TableCell();
                nameCell.Width = Unit.Pixel(200);
                nameCell.Text = "Name";
                newRow.Cells.Add(nameCell);

                TableCell dateCell = new TableCell();
                dateCell.Width = Unit.Pixel(200);
                dateCell.Text = "Date added";
                newRow.Cells.Add(dateCell);

                TableCell apprCell = new TableCell();
                apprCell.Text = "Approved by";
                newRow.Cells.Add(apprCell);

                tblAllCompProdModificators.Rows.Add(newRow);

                foreach (UsersTypeAction uact in usersAction)
                {
                    if (!uact.UserReference.IsLoaded)
                    {
                        uact.UserReference.Load();
                    }
                    User user = Tools.GetUserFromUserDatabase(userContext, uact.User);

                    TableRow userRow = new TableRow();

                    TableCell actidCell = new TableCell();
                    actidCell.Text = user.ID.ToString();
                    userRow.Cells.Add(actidCell);

                    userRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(user));

                    TableCell dtCell = new TableCell();
                    dtCell.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(uact.dateCreated);
                    userRow.Cells.Add(dtCell);

                    TableCell appCell = new TableCell();
                    userRow.Cells.Add(appCell);

                    if (!uact.ApprovedByReference.IsLoaded)
                    {
                        uact.ApprovedByReference.Load();
                    }
                    User approver = Tools.GetUserFromUserDatabase(userContext, uact.ApprovedBy);

                    if (businessUser.IsUserValidType(approver))
                    {
                        appCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink(approver));
                    }
                    else
                    {
                        appCell.Text = approver.username;
                    }

                    tblAllCompProdModificators.Rows.Add(userRow);

                }
            }
            else
            {
                tblAllCompProdModificators.Visible = false;
                lblProdCompEditors.Visible = false;
            }

        }

        protected void ProductEditorsFill()
        {
            tblProdEditors.Rows.Clear();

            BusinessUser businessUser = new BusinessUser();
            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

            IEnumerable<UsersTypeAction> usersAction = businessUserTypeActions.GetProductModificators(objectContext, currentProduct.ID);

            if (usersAction.Count<UsersTypeAction>() > 0)
            {
                tblProdEditors.Visible = true;
                lblProdEditors.Visible = true;

                TableRow newRow = new TableRow();

                TableCell idCell = new TableCell();
                idCell.Text = "Id";
                idCell.Width = Unit.Pixel(50);
                newRow.Cells.Add(idCell);

                TableCell nameCell = new TableCell();
                nameCell.Text = "Name";
                nameCell.Width = Unit.Pixel(200);
                newRow.Cells.Add(nameCell);

                TableCell dateCell = new TableCell();
                dateCell.Text = "Date added";
                dateCell.Width = Unit.Pixel(200);
                newRow.Cells.Add(dateCell);

                TableCell apprCell = new TableCell();
                apprCell.Text = "Approved by";
                newRow.Cells.Add(apprCell);

                TableCell remCell = new TableCell();
                remCell.Width = Unit.Pixel(10);
                remCell.Text = "Remove";
                newRow.Cells.Add(remCell);

                tblProdEditors.Rows.Add(newRow);

                foreach (UsersTypeAction uact in usersAction)
                {
                    if (!uact.UserReference.IsLoaded)
                    {
                        uact.UserReference.Load();
                    }
                    User user = Tools.GetUserFromUserDatabase(userContext, uact.User);

                    TableRow userRow = new TableRow();

                    TableCell actidCell = new TableCell();
                    actidCell.Text = user.ID.ToString();
                    userRow.Cells.Add(actidCell);

                    userRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(user));

                    TableCell dtCell = new TableCell();
                    dtCell.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(uact.dateCreated);
                    userRow.Cells.Add(dtCell);

                    if (!uact.ApprovedByReference.IsLoaded)
                    {
                        uact.ApprovedByReference.Load();
                    }
                    User approver = Tools.GetUserFromUserDatabase(userContext, uact.ApprovedBy);

                    if (approver != null && businessUser.IsUserValidType(approver))
                    {
                        userRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(approver));
                    }
                    else if (approver != null)
                    {
                        TableCell appCell = new TableCell();
                        appCell.Text = approver.username;
                        userRow.Cells.Add(appCell);
                    }

                    TableCell delCell = new TableCell();
                    Button delBtn = new Button();
                    delBtn.ID = string.Format("RemoveRole" + user.ID.ToString());
                    delBtn.Text = "Remove";
                    delBtn.Attributes.Add("userID", user.ID.ToString());
                    delBtn.Click += new EventHandler(RemoveRoleProductModificator);
                    delCell.Controls.Add(delBtn);
                    userRow.Cells.Add(delCell);

                    tblProdEditors.Rows.Add(userRow);

                }
            }
            else
            {
                tblProdEditors.Visible = false;
                lblProdEditors.Visible = false;
            }

        }

        void RemoveRoleProductModificator(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllProductsFromEvents();

            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

            Button btn = sender as Button;
            String usrID = btn.Attributes["userID"];
            long userID = -1;
            if (!long.TryParse(usrID, out userID))
            {
                throw new CommonCode.UIException(string.Format
                    ("Couldnt parse  btn.Attributes['userID'] = {0} to long , user id = {1}", usrID, currentUser.ID));
            }
            if (userID < 1)
            {
                throw new CommonCode.UIException(string.Format("btn.Attributes['userID'] is < 1 , user id = {0}", currentUser.ID));
            }

            businessUserTypeActions.RemoveUserActionProductModificator
                (objectContext, userContext, userID, currentUser, currentProduct, businessLog);

            ProductEditorsFill();
            FillPublicShownProductEditors();

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Editor roles removed from user!");
        }

        protected void btnUserRoleProd_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllProductsFromEvents();

            phRoles.Visible = true;
            phRoles.Controls.Add(lblError);
            string error = "";

            String userID = tbUserRoleProd.Text;

            if (CommonCode.Validate.ValidateLong(userID, out error))
            {
                long id = -1;
                long.TryParse(userID, out id);

                BusinessUser businessUser = new BusinessUser();
                BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

                User userToAddRole = businessUser.Get(userContext, id, false);
                if (userToAddRole != null)
                {
                    if (!userToAddRole.UserOptionsReference.IsLoaded)
                    {
                        userToAddRole.UserOptionsReference.Load();
                    }
                    if (userToAddRole.UserOptions.activated == true)
                    {
                        String type = userToAddRole.type;
                        if (type == "user")
                        {
                            if (!businessUser.CanUserModifyProduct(objectContext, currentProduct.ID, userToAddRole.ID))
                            {
                                BusinessProduct bProduct = new BusinessProduct();
                                if (bProduct.CheckIfProductsIsValidWithConnections(objectContext, currentProduct, out error) == true)
                                {
                                    businessUserTypeActions.AddUserActionProductModificator
                                        (userContext, objectContext, userToAddRole, currentProduct, currentUser, businessLog, true);
                                    ProductEditorsFill();

                                    FillPublicShownProductEditors();

                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Editor roles given to user!");
                                }
                                else
                                {
                                    error = "The product connections are not ok. You have to fix them before you can give roles.";
                                }
                            }
                            else
                            {
                                error = "The user already have this role";
                            }
                        }
                        else
                        {
                            error = "The user you are trying to give this role is not with type 'user'.";
                        }
                    }
                    else
                    {
                        error = "That user is not activated.";
                    }
                }
                else
                {
                    error = "No such user.";
                }
            }

            lblError.Text = error;
            tbUserRoleProd.Text = "";
        }

        protected void btnAddCharacteristic_Click1(object sender, EventArgs e)
        {
            pnlSubAddChar.Visible = true;

            HidePlaceHolders();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            PlaceHolder phImg = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "phImage");
            FileUpload fuImage = (FileUpload)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "fuImage");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "tbImageDescr");
            CheckBox main = (CheckBox)CommonCode.UiTools.GetControlFromAccordionPane(apUploadImage, "cbImageMain");

            phImg.Visible = true;
            phImg.Controls.Add(lblError);
            string error = "";

            if (fuImage.HasFile)
            {
                string imageErrorDescription = string.Empty;
                byte[] fileBytes = fuImage.FileBytes;
                string fileName = fuImage.FileName;
                string fileType = System.IO.Path.GetExtension(fileName);

                bool imageOk = ImageTools.IsValidImage(fileName, fileBytes, ref imageErrorDescription, false);
                if (imageOk == true)
                {

                    string strDescr = description.Text;

                    if (CommonCode.Validate.ValidateDescription(Configuration.ImagesMinPictureDescriptionLength,
                        Configuration.ImagesMaxPictureDescriptionLength, ref strDescr, "description", out error, Configuration.FieldsDefMaxWordLength))
                    {
                        int width = 0;
                        int height = 0;

                        if (ImageTools.GetImageWidthAndHeight(fileBytes, out width, out height))
                        {
                            ImageTools imageTools = new ImageTools();

                            lock (addingImage)
                            {
                                // creating new row in table
                                ProductImage newImage = new ProductImage();
                                newImage.Product = currentProduct;
                                newImage.url = "";
                                newImage.main = main.Checked;
                                newImage.dateCreated = DateTime.UtcNow;
                                newImage.CreatedBy = Tools.GetUserID(objectContext, currentUser);
                                newImage.description = strDescr;
                                newImage.width = width;
                                newImage.height = height;
                                newImage.isThumbnail = false;
                                newImage.mainImgID = null;
                                imageTools.AddProductImage(userContext, objectContext, newImage, businessLog, currentUser);

                                // function for name
                                String path;
                                String url;
                                CommonCode.ImagesAndAdverts.GenerateProductImageNamePathUrl(fileType, currentProduct, newImage, out path, out url);

                                string appPath = CommonCode.PathMap.PhysicalApplicationPath;  // CommonCode.PathMap.GetImagesPhysicalPathRoot();
                                string imagePathFromRoot = System.IO.Path.Combine(appPath, path);
                                fuImage.SaveAs(imagePathFromRoot);

                                imageTools.ChangeProductImageUrl(objectContext, newImage, url);

                                CommonCode.ImagesAndAdverts.GenerateProductThumbnail(userContext, objectContext, businessLog, newImage, currentProduct, currentUser, fileBytes);


                            }

                            phImg.Visible = false;
                            main.Checked = false;
                            description.Text = "";

                            CheckAddImageOptions();
                            ShowInfo();

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                , GetLocalResourceObject("imgUploaded").ToString());
                        }
                        else
                        {
                            error = GetLocalResourceObject("errImage").ToString();
                        }
                    }
                }
                else
                {
                    error = string.IsNullOrEmpty(imageErrorDescription) ?
                        GetLocalResourceObject("errUploading").ToString() : imageErrorDescription;
                }
            }
            else
            {
                error = GetLocalResourceObject("errChooseFile").ToString();
            }

            lblError.Text = error;
        }

        public void FillGalleryTable()
        {
            tblGallery.Rows.Clear();

            BusinessUser businessUser = new BusinessUser();
            ImageTools imageTools = new ImageTools();

            Boolean canEdit = false;
            if (currentUser != null)
            {
                if (businessUser.CanUserModifyProduct(objectContext, currentProduct.ID, currentUser.ID) ||
                    businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditProducts))
                {
                    canEdit = true;
                }
            }

            string appPath = CommonCode.PathMap.PhysicalApplicationPath; 

            List<ProductImage> images = imageTools.GetProductThumbnails(userContext, objectContext, currentProduct, businessLog, appPath);
            int count = images.Count<ProductImage>();
            if (count > 0)
            {
                int FieldsPerRow = 5;
                int i = 0;

                TableRow newRow = new TableRow();
                tblGallery.Rows.Add(newRow);

                foreach (ProductImage image in images)
                {
                    if (i++ >= FieldsPerRow)
                    {
                        newRow = new TableRow();
                        tblGallery.Rows.Add(newRow);
                        i = 1;
                    }

                    TableCell newCell = new TableCell();
                    newRow.Cells.Add(newCell);
                    newCell.CssClass = "galleryCell";
                    newCell.HorizontalAlign = HorizontalAlign.Center;

                    ProductImage parentImage = imageTools.GetProductImage(objectContext, image.mainImgID.Value);
                    if (parentImage == null)
                    {
                        throw new CommonCode.UIException(string.Format
                            ("Thumbnail ID = {0} `s main image is null , shouldnt happen as there is check when getting all thumbnails"
                            , image.ID));
                    }

                    HyperLink hlToBigImage = new HyperLink();
                    hlToBigImage.ImageUrl = image.url;
                    hlToBigImage.NavigateUrl = parentImage.url;
                    hlToBigImage.Target = "_NEW";
                    if (image.description.Length > 0)
                    {
                        hlToBigImage.ToolTip = image.description;
                    }
                    newCell.Controls.Add(hlToBigImage);
                    hlToBigImage.CssClass = "images";


                    if (canEdit)
                    {
                        Button delBtn = new Button();
                        delBtn.Text = GetGlobalResourceObject("SiteResources", "Delete").ToString();
                        delBtn.Attributes.Add("ImageID", image.ID.ToString());
                        delBtn.ID = string.Format("DeleteImageButton{0}", image.ID);
                        delBtn.Click += new EventHandler(delBtn_Click);
                        delBtn.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginTop, "5px");
                        newCell.Controls.Add(delBtn);
                    }
                }

            }
            else
            {
                TableRow noDataRow = new TableRow();
                TableCell noDataCell = new TableCell();
                noDataCell.Text = GetLocalResourceObject("NoImages").ToString();
                noDataRow.Cells.Add(noDataCell);
                tblGallery.Rows.Add(noDataRow);
            }

        }

        void delBtn_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            Button btn = sender as Button;
            if (btn != null)
            {
                long id = 0;
                if (btn.Attributes["ImageID"] != null)
                {
                    if (long.TryParse(btn.Attributes["ImageID"], out id))
                    {
                        ImageTools imageTools = new ImageTools();
                        ProductImage currImage = imageTools.GetProductImage(objectContext, id);

                        if (currImage != null)
                        {
                            string appPath = CommonCode.PathMap.PhysicalApplicationPath;

                            if (log.IsInfoEnabled == true)
                            {
                                string logMsg = ImageTools.DeletingImageLogMessage(currImage.ID, currImage.url, "user request");
                                log.Info(logMsg);
                            }

                            if (imageTools.DeleteProductImage(userContext, objectContext, currImage, businessLog, appPath, currentUser) == true)
                            {
                                CheckAddImageOptions();
                                ShowInfo();

                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                    , GetLocalResourceObject("ImageDeleted").ToString());
                            }
                            else
                            {
                                CheckAddImageOptions();
                                ShowInfo();

                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                    , GetLocalResourceObject("errCantDelImage").ToString());
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("Theres no image with ID = {0} , user id = {1}", id, currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format("couldnt parse btn.Attributes['ImageID'] = {0} to long , user id = {1}",
                            btn.Attributes["ImageID"], currentUser.ID));
                    }
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format
                    ("Delete image method (delBtn_Click) doesnt recognise sender button , user id = {0}", currentUser.ID));
            }
        }


        protected void btnChngCategory_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllProductsFromEvents();

            PlaceHolder phChangeCat = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "phChangeCategory");

            phChangeCat.Controls.Clear();
            phChangeCat.Visible = true;
            phChangeCat.Controls.Add(lblError);
            string error = "";

            if (currentProduct.visible)
            {
                String strCatId = tbChngCategory.Text;
                if (CommonCode.Validate.ValidateLong(strCatId, out error))
                {
                    long catID = -1;
                    if (long.TryParse(strCatId, out catID))
                    {
                        BusinessCategory businessCategory = new BusinessCategory();
                        BusinessProduct businessProduct = new BusinessProduct();

                        Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);

                        Category newCategory = businessCategory.GetWithoutVisible(objectContext, catID);
                        if (newCategory != null && newCategory.last == true)
                        {

                            if (!currentProduct.CategoryReference.IsLoaded)
                            {
                                currentProduct.CategoryReference.Load();
                            }

                            if (currentProduct.Category != newCategory)
                            {
                                if (unspecified != newCategory)
                                {
                                    bool moveFromUnspecifiedCategory = false;
                                    if (unspecified == currentProduct.Category)
                                    {
                                        moveFromUnspecifiedCategory = true;
                                    }

                                    businessProduct.ChangeProductCategory(userContext, objectContext, currentProduct, newCategory,
                                        CommonCode.UiTools.GetCurrentUserExcIfNull(userContext, objectContext), businessLog, moveFromUnspecifiedCategory);

                                    UpdateProductNotification();
                                    CheckCrucialEditOptions(true);
                                    ShowInfo();
                                    tbChngCategory.Text = "";

                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Product category changed!");
                                }
                                else
                                {
                                    error = "You cannot move products to 'Unspecified' category.";
                                }
                            }
                            else
                            {
                                error = "Choose product category which isn`t the current one.";
                            }
                        }
                        else
                        {
                            error = "No such Category or it isn`t last=true.";
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format("Couldnt parse tbChngCategory.Text = {0} to long , user id = {1}"
                            , strCatId, currentUser.ID));
                    }

                }
            }
            else
            {
                error = "Current Product is not visible.";
            }

            lblError.Text = error;
        }

        [WebMethod]
        public static string CheckData(string text, string type, string charId)
        {
            string error = "";

            CommonCode.WebMethods.ValidateUserInput(text, type, charId, out error);

            return error;

        }

        public void SetCategoryNameWithLink()
        {
            phCategory.Controls.Clear();

            if (!currentProduct.CategoryReference.IsLoaded)
            {
                currentProduct.CategoryReference.Load();
            }

            BusinessCategory businessCategory = new BusinessCategory();
            Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);

            phCategory.Controls.Add(CommonCode.UiTools.GetCategoryNameWithLink(currentProduct.Category, objectContext, true, false, true));
            if (currentProduct.Category == unspecified)
            {
                phCategory.Controls.Add(CommonCode.UiTools.GetNewLineControl());

                Label lblWantedCat = new Label();
                phCategory.Controls.Add(lblWantedCat);
                lblWantedCat.Text = GetLocalResourceObject("wantedCategory").ToString() + "&nbsp;";
                lblWantedCat.CssClass = "searchPageComments";

                currentProduct.ProductsInUnspecifiedCategory.Load();
                currentProduct.ProductsInUnspecifiedCategory.First().WantedCategoryReference.Load();
                Category wantedCategory = currentProduct.ProductsInUnspecifiedCategory.First().WantedCategory;

                phCategory.Controls.Add(CommonCode.UiTools.GetCategoryNameWithLink(wantedCategory, objectContext, true, false, false));
            }

        }

        protected void dbCancelUpload_Click(object sender, EventArgs e)
        {
            tbImageDescr.Text = "";
            pnlAddImage.Visible = false;
        }

        protected void dbCancelCharacteristic_Click(object sender, EventArgs e)
        {
            tbCharDescription.Text = "";
            tbCharName.Text = "";
            pnlSubAddChar.Visible = false;
        }


        protected void ddlEditCharacteristics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentUser == null)
            {
                throw new CommonCode.UIException("CurrentUser is null");
            }

            DropDownList ddlEditCharacteristics = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "ddlEditCharacteristics");
            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbAccCharName");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbAccEditCharDescription");

            BusinessProduct businessProduct = new BusinessProduct();
            long charID = -1;
            if (long.TryParse(ddlEditCharacteristics.SelectedValue, out charID))
            {
                ProductCharacteristics prodChar = businessProduct.GetCharacteristic(objectContext, charID);
                if (prodChar != null)
                {
                    description.Text = prodChar.description;
                    name.Text = "";
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("Theres no Product ID = {0} Characteristic = {1} (prodChar is null) , user id = {2}"
                        , currentProduct.ID, charID, currentUser.ID));
                }
            }
            else
            {
                FillDDlEditChars();
            }
        }

        protected void dbEditDescription_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            string error = "";

            BusinessProduct businessProduct = new BusinessProduct();

            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "tbAccEditDescription");

            string strDescr = description.Text;

            if (CommonCode.Validate.ValidateDescription(Configuration.ProductsMinDescriptionLength,
                       Configuration.ProductsMaxDescriptionLength, ref strDescr, "description", out error, 110)
                && description.Text != currentProduct.description)
            {
                businessProduct.ChangeProductDescription
                    (objectContext, currentProduct, strDescr, currentUser, businessLog);
                ShowInfo();

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("descrChanged").ToString());
            }
            else
            {
                if (string.IsNullOrEmpty(error))
                {
                    error = GetLocalResourceObject("errChangeDescr").ToString();
                }

                PlaceHolder phDescription = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "phEditDescription");

                phDescription.Visible = true;
                phDescription.Controls.Add(lblError);
                lblError.Text = error;
            }
        }

        protected void btnEditCharSave_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            BusinessProduct businessProduct = new BusinessProduct();
            BusinessUser businessUser = new BusinessUser();
            BusinessComment businessComment = new BusinessComment();

            PlaceHolder phChar = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "phEditChar");
            phChar.Visible = true;
            phChar.Controls.Add(lblError);

            DropDownList ddlEditChar = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "ddlEditCharacteristics");
            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbAccCharName");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbAccEditCharDescription");

            string error = "";
            bool changeDone = false;
            bool updateDDl = false;

            long charID = -1;
            long.TryParse(ddlEditChar.SelectedValue, out charID);

            ProductCharacteristics prodChar = businessProduct.GetCharacteristic(objectContext, charID);

            if (prodChar == null)
            {
                lblError.Text = GetLocalResourceObject("errNoSuchChar").ToString();

                name.Text = string.Empty;
                description.Text = string.Empty;

                FillDDlEditChars();
                ShowComments();
                SortCommentsBy(true, false);
                ShowCharacteristics();

                return;
            }

            string strName = Tools.RemoveSpacesAtEndOfString(name.Text);

            if (strName == prodChar.name && description.Text == prodChar.description)
            {
                error = GetLocalResourceObject("errEditCharTypeNewNameOrDescr").ToString();
            }
            else
            {

                if (strName.Length > 0)
                {
                    if (businessUser.IsFromUserTeam(currentUser))
                    {
                        if (businessComment.AreThereVisibleCommentsForCharacteristic(objectContext, prodChar) == true)
                        {
                            lblError.Text = string.Format("{0} ' {1} ' {2}."
                                , GetLocalResourceObject("errCantChangeCharName"), prodChar.name
                                , GetLocalResourceObject("errCantChangeCharName2"));
                            return;
                        }

                    }



                    if (CommonCode.Validate.ValidateName(objectContext, "prodChar", ref strName,
                        Configuration.ProductsMinProductNameLength, Configuration.ProductsMaxProductNameLength
                        , out error, currentProduct.ID))
                    {
                        businessProduct.ChangeProductCharacteristicName
                            (objectContext, prodChar, strName, currentUser, businessLog);
                        changeDone = true;
                        updateDDl = true;
                        ShowComments();
                        SortCommentsBy(true, false);

                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                            , GetLocalResourceObject("CharTopicChanged").ToString());
                    }
                }
                if (description.Text.Length > 0)
                {
                    string strDescr = description.Text;

                    if (strDescr != prodChar.description)
                    {
                        if (CommonCode.Validate.ValidateDescription(Configuration.FieldsMinDescriptionFieldLength,
                            Configuration.FieldsMaxDescriptionFieldLength, ref strDescr, "description", out error, 120))
                        {
                            businessProduct.ChangeProductCharacteristicDescription
                                (objectContext, prodChar, strDescr, currentUser, businessLog);
                            changeDone = true;

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                , GetLocalResourceObject("CharDescrChanged").ToString());
                        }
                    }
                    else if (changeDone == false && string.IsNullOrEmpty(error))
                    {
                        error = GetLocalResourceObject("errEditCharTypeNewDescr").ToString();
                    }
                }
                if (string.IsNullOrEmpty(description.Text) && string.IsNullOrEmpty(name.Text))
                {
                    error = GetLocalResourceObject("errEditCharTypeNewNameOrDescr").ToString();
                }
            }

            if (changeDone)
            {
                name.Text = string.Empty;
                description.Text = string.Empty;

                if (tblChars.Visible)
                {
                    ShowCharacteristics();
                }
                if (updateDDl == true)
                {
                    FillDDlEditChars();
                }

                changedChars = true;
                WriteComment();
            }


            lblError.Text = error;
        }

        protected void btnDeleteCharacteristic_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            BusinessProduct businessProduct = new BusinessProduct();

            DropDownList ddlEditChar = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "ddlEditCharacteristics");

            long charID = -1;
            long.TryParse(ddlEditChar.SelectedValue, out charID);

            ProductCharacteristics prodChar =
                businessProduct.GetCharacteristic(objectContext, charID);

            if (prodChar != null)
            {
                if (prodChar.visible == true)
                {
                    businessProduct.DeleteProductCharacteristic(userContext, objectContext, prodChar, currentUser, businessLog);
                }

                Session.Add("notifMsg", GetLocalResourceObject("CharDeleted").ToString()); // used for showing in user notification panel that char is deleted
                RedirectToCurrProductPage();
            }
            else
            {
                PlaceHolder phChar = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "phEditChar");
                phChar.Visible = true;
                phChar.Controls.Add(lblError);

                lblError.Text = GetLocalResourceObject("errNoSuchChar").ToString();
                FillDDlEditChars();
                return;
            }

        }

        protected void btnUpdateProductName_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            if (currentUser != null)
            {
                if (!businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditProducts))
                {
                    CheckIfUserCanEditProductFromEvents();

                    if (CheckIfTimeAfterWhichCrucialProductDataCanBeEditedDidntPassed() == false)
                    {
                        return;
                    }

                }
            }
            else
            {
                return;
            }

            BusinessProduct businessProduct = new BusinessProduct();

            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditName, "tbAccEditName");
            string error = "";

            string strName = name.Text;
            bool changeName = false;

            if (string.Equals(currentProduct.name, strName, StringComparison.InvariantCultureIgnoreCase) == true
                && string.Equals(currentProduct.name, strName, StringComparison.InvariantCulture) == false)
            {
                changeName = true;
            }
            else
            {
                changeName = CommonCode.Validate.ValidateName(objectContext, "products", ref strName, Configuration.ProductsMinProductNameLength,
                Configuration.ProductsMaxProductNameLength, out error, 0);
            }

            if (changeName == true)
            {
                businessProduct.ChangeProductName(objectContext, currentProduct, currentUser, businessLog, strName);
                ShowInfo();

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification,
                    GetLocalResourceObject("ProdNameUpdated").ToString());
            }
            else
            {
                PlaceHolder phName = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditName, "phNewName");
                phName.Visible = true;
                phName.Controls.Add(lblError);
                lblError.Text = error;
            }
        }

        protected void btnUpdateWebSite_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            BusinessProduct businessProduct = new BusinessProduct();

            TextBox website = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditWebsite, "tbAccEditWebsite");
            string error = "";

            String newSiteAdress = string.Empty;
            if (!string.IsNullOrEmpty(website.Text))
            {
                newSiteAdress = CommonCode.UiTools.GetCorrectedUrl(website.Text);
            }

            if (newSiteAdress != currentProduct.website)
            {
                if (string.IsNullOrEmpty(newSiteAdress))
                {
                    businessProduct.ChangeProductWebSite(objectContext, currentProduct, newSiteAdress, currentUser, businessLog);
                    ShowInfo();

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("SiteRemoved").ToString());
                }
                else
                {
                    if (CommonCode.Validate.ValidateSiteAdress(newSiteAdress, out error, false))
                    {
                        businessProduct.ChangeProductWebSite(objectContext, currentProduct, newSiteAdress, currentUser, businessLog);
                        ShowInfo();

                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                            , GetLocalResourceObject("SiteChanged").ToString());
                    }
                }
            }
            else
            {
                error = GetLocalResourceObject("errSite").ToString();
            }

            if (!string.IsNullOrEmpty(error))
            {
                PlaceHolder phSite = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditWebsite, "phNewWebsite");
                phSite.Visible = true;
                phSite.Controls.Add(lblError);
                lblError.Text = error;
            }
        }

        [WebMethod]
        public static string WMRateComment(string commID, string rating)
        {
            return CommonCode.WebMethods.GetRateCommentData(commID, rating);
        }

        [WebMethod]
        public static string WMSetMsgAsSpam(string commID)
        {
            return CommonCode.WebMethods.SetMsgAsViolation(commID, CommentType.Product);
        }

        [WebMethod]
        public static string WMSetLinkAsViolation(string linkID)
        {
            return CommonCode.WebMethods.SetProductLinkAsViolation(linkID);
        }

        [WebMethod]
        public static string WMSendMsgToUser(string commID, string username, string message, string subject, string saveInSent)
        {
            return CommonCode.WebMethods.SendMsgToUser(commID, username, message, subject, saveInSent);

        }

        [WebMethod]
        public static string WMReplyToComment(string commID, string username, string message)
        {
            return CommonCode.WebMethods.ReplyToComment(commID, string.Empty, message, CommentType.Product);
        }

        [WebMethod]
        public static string WMDeleteComment(string commID, string sendWarning)
        {
            bool sendwarn = true;

            if (bool.TryParse(sendWarning, out sendwarn))
            {
                return CommonCode.WebMethods.DeleteComment(commID, sendwarn);
            }
            else
            {
                return string.Empty;
            }
        }

        [WebMethod]
        public static string WMEditComment(string commID, string username, string message)
        {
            return CommonCode.WebMethods.EditComment(commID, username, message);
        }

        [WebMethod]
        public static string WMGetCommentDescription(string commID)
        {
            return CommonCode.WebMethods.GetCommentDescription(commID);
        }

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }

        [WebMethod]
        public static string WMSendReport(string type, string strTypeId, string description)
        {
            return CommonCode.WebMethods.SendReport(type, strTypeId, description);
        }

        [WebMethod]
        public static string WMAddProductLink(string prodId, string link, string description)
        {
            return CommonCode.WebMethods.AddProductLink(prodId, link, description);
        }

        [WebMethod]
        public static string WMSignForNotifies(string type, string Id)
        {
            return CommonCode.WebMethods.SignForNotifies(type, Id);
        }

        [WebMethod]
        public static string WMDeleteProductLink(string strLinkID)
        {
            return CommonCode.WebMethods.DeleteProductLink(strLinkID, false);
        }

        [WebMethod]
        public static string WMDeleteProductLinkW(string strLinkID)
        {
            return CommonCode.WebMethods.DeleteProductLink(strLinkID, true);
        }

        [WebMethod]
        public static string WMChangeProductLinkDescr(string strLinkID, string newDescr)
        {
            return CommonCode.WebMethods.EditProductLinkDescr(strLinkID, newDescr);
        }

        protected void btnAddVariant_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            PlaceHolder addVariant = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apAddVariant, "phAddVariant");
            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddVariant, "tbAddVariantName");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddVariant, "tbAddVariantDescription");

            addVariant.Visible = true;
            addVariant.Controls.Add(lblError);
            string error = "";

            BusinessProductVariant bpVariant = new BusinessProductVariant();

            string strName = name.Text;

            if (CommonCode.Validate.ValidateVariantName(objectContext, currentProduct, ref strName, Configuration.ProductsMinProductNameLength,
                Configuration.ProductsMaxProductNameLength, out error, 0))
            {
                string strDescr = description.Text;

                if (CommonCode.Validate.ValidateDescription(0, Configuration.FieldsMaxDescriptionFieldLength
                    , ref strDescr, "description", out error, 110))
                {
                    addVariant.Visible = false;

                    bpVariant.AddVariant(objectContext, currentProduct, businessLog, currentUser, strName, strDescr);

                    description.Text = "";
                    name.Text = "";

                    CheckAddVariantsOptions();
                    CheckEditVariantsOptions();

                    // UPDATE COMMENTS DDS
                    changedVariants = true;
                    WriteComment();
                    // UPDATE DDL SORT BY VARIANT
                    SortCommentsBy(false, true); 

                    ShowAccordionsInfo();

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("variantAdded").ToString());
                }
            }

            lblError.Text = error;
        }

        protected void btnAddSubVariant_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            PlaceHolder addVariant = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "phAddSubVariant");
            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "tbAddSubVariantName");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "tbAddSubVariantDescription");
            DropDownList ddlVariants = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apAddSubVariant, "ddlAddVariant");

            addVariant.Visible = true;
            addVariant.Controls.Add(lblError);
            string error = "";

            BusinessProductVariant bpVariant = new BusinessProductVariant();

            string strVariantId = ddlVariants.SelectedValue;
            long varId = 0;
            if (!long.TryParse(strVariantId, out varId))
            {
                throw new CommonCode.UIException(string.Format("Couldn`t parse ddlAddVariant.SelectedValue to long, user id : {0}", currentUser.ID));
            }

            if (varId == 0)
            {
                lblError.Text = GetLocalResourceObject("errChooseVariant").ToString();
                return;
            }

            ProductVariant chosenVariant = bpVariant.Get(objectContext, currentProduct, varId, true, false);
            if (chosenVariant == null)
            {
                lblError.Text = GetLocalResourceObject("errNoSuchVariant").ToString();
                CheckEditVariantsOptions();
                return;
            }

            string strName = name.Text;

            if (CommonCode.Validate.ValidateVariantName(objectContext, currentProduct, ref strName, Configuration.ProductsMinProductNameLength,
                Configuration.ProductsMaxProductNameLength, out error, chosenVariant.ID))
            {
                string strDescr = description.Text;

                if (CommonCode.Validate.ValidateDescription(0, Configuration.FieldsMaxDescriptionFieldLength
                    , ref strDescr, "description", out error, 110))
                {
                    addVariant.Visible = false;

                    bpVariant.AddSubVariant(objectContext, currentProduct, businessLog, currentUser, chosenVariant, strName, strDescr);

                    description.Text = "";
                    name.Text = "";

                    CheckAddVariantsOptions();
                    CheckEditVariantsOptions();

                    // UPDATE COMMENTS DDS
                    changedVariants = true;
                    WriteComment();
                    // UPDATE DDL SORT BY VARIANT
                    SortCommentsBy(false, true);

                    ShowAccordionsInfo();

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("subvariantAdded").ToString());
                }
            }

            lblError.Text = error;
        }


        protected void ddlEditVariant_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentUser == null)
            {
                throw new CommonCode.UIException("CurrentUser is null");
            }

            DropDownList ddlEditVariant = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "ddlEditVariant");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "tbEditVariantDescription");

            BusinessProductVariant bpVariant = new BusinessProductVariant();
            long varID = -1;
            if (long.TryParse(ddlEditVariant.SelectedValue, out varID))
            {
                if (varID > 0)
                {
                    ProductVariant currVariant = bpVariant.Get(objectContext, currentProduct, varID, true, true);
                    description.Text = currVariant.description;
                }
                else
                {
                    description.Text = string.Empty;
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("Couldnt parse ddlEditVariant.SelectedValue = {0} to long , user id = {1}"
                    , ddlEditCharacteristics.SelectedValue, currentUser.ID));
            }
        }

        protected void btnEditVariant_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            PlaceHolder editVariant = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "phEditVariant");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "tbEditVariantDescription");
            DropDownList ddlVariants = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "ddlEditVariant");

            editVariant.Visible = true;
            editVariant.Controls.Add(lblError);
            string error = "";

            BusinessProductVariant bpVariant = new BusinessProductVariant();

            string strVariantId = ddlVariants.SelectedValue;
            long varId = 0;
            if (!long.TryParse(strVariantId, out varId))
            {
                throw new CommonCode.UIException(string.Format("Couldn`t parse ddlEditVariant.SelectedValue to long, user id : {0}", currentUser.ID));
            }

            if (varId == 0)
            {
                lblError.Text = GetLocalResourceObject("errChooseVariant").ToString();
                return;
            }

            ProductVariant chosenVariant = bpVariant.Get(objectContext, currentProduct, varId, true, false);

            if (chosenVariant == null)
            {
                lblError.Text = GetLocalResourceObject("errNoSuchVariant").ToString();
                CheckEditVariantsOptions();
                return;
            }


            if (chosenVariant.description == description.Text)
            {
                lblError.Text = GetLocalResourceObject("errNewDescr").ToString();
                return;
            }

            string strDescr = description.Text;

            if (CommonCode.Validate.ValidateDescription(0, Configuration.FieldsMaxDescriptionFieldLength
                , ref strDescr, "description", out error, 110))
            {
                editVariant.Visible = false;

                bpVariant.ChangeVariantDescription(objectContext, businessLog, currentUser, chosenVariant, strDescr);

                description.Text = "";

                CheckAddVariantsOptions();
                CheckEditVariantsOptions();

                // UPDATE COMMENTS DDS

                FillVariants();

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                    , string.Format("{0} {1} {2}", chosenVariant.name, GetLocalResourceObject("DescriptionUpdated")
                    , GetLocalResourceObject("DescriptionUpdated2")));
            }


            lblError.Text = error;

        }

        protected void btnDeleteVariant_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            PlaceHolder editVariant = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "phEditVariant");
            DropDownList ddlVariants = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "ddlEditVariant");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditVariant, "tbEditVariantDescription");

            editVariant.Visible = true;
            editVariant.Controls.Add(lblError);
            string error = "";

            BusinessProductVariant bpVariant = new BusinessProductVariant();

            string strVariantId = ddlVariants.SelectedValue;
            long varId = 0;
            if (!long.TryParse(strVariantId, out varId))
            {
                throw new CommonCode.UIException(string.Format("Couldn`t parse ddlEditVariant.SelectedValue to long, user id : {0}", currentUser.ID));
            }

            if (varId == 0)
            {
                lblError.Text = GetLocalResourceObject("errChooseVariant").ToString();
                return;
            }

            ProductVariant chosenVariant = bpVariant.Get(objectContext, currentProduct, varId, true, false);
            if (chosenVariant == null)
            {
                lblError.Text = GetLocalResourceObject("errNoSuchVariant").ToString();
                CheckEditVariantsOptions();
                return;
            }

            editVariant.Visible = false;

            bpVariant.DeleteVariant(objectContext, businessLog, currentUser, chosenVariant);
            description.Text = string.Empty;

            CheckAddVariantsOptions();
            CheckEditVariantsOptions();

            // UPDATE COMMENTS DDS
            changedVariants = true;
            WriteComment();
            // UPDATE DDL SORT BY VARIANT
            SortCommentsBy(false, true); 

            ShowAccordionsInfo();

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                , GetLocalResourceObject("VarDeleted").ToString());

            lblError.Text = error;
        }


        protected void ddlEditSubVariant_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentUser == null)
            {
                throw new CommonCode.UIException("CurrentUser is null");
            }

            DropDownList ddlEditSubVariant = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "ddlEditSubVariant");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "tbEditSubVariantDescription");

            BusinessProductVariant bpVariant = new BusinessProductVariant();
            long varID = -1;
            if (long.TryParse(ddlEditSubVariant.SelectedValue, out varID))
            {
                if (varID > 0)
                {
                    ProductSubVariant currVariant = bpVariant.GetSubVariant(objectContext, currentProduct, varID, true, true);
                    description.Text = currVariant.description;
                }
                else
                {
                    description.Text = string.Empty;
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("Couldnt parse ddlEditSubVariant.SelectedValue = {0} to long , user id = {1}"
                    , ddlEditCharacteristics.SelectedValue, currentUser.ID));
            }
        }


        protected void btnEditSubVariant_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            PlaceHolder editVariant = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "phEditSubVariant");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "tbEditSubVariantDescription");
            DropDownList ddlVariants = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "ddlEditSubVariant");

            editVariant.Visible = true;
            editVariant.Controls.Add(lblError);
            string error = "";

            BusinessProductVariant bpVariant = new BusinessProductVariant();

            string strVariantId = ddlVariants.SelectedValue;
            long varId = 0;
            if (!long.TryParse(strVariantId, out varId))
            {
                throw new CommonCode.UIException(string.Format("Couldn`t parse ddlEditVariant.SelectedValue to long, user id : {0}", currentUser.ID));
            }

            if (varId == 0)
            {
                lblError.Text = GetLocalResourceObject("errChooseVariant").ToString();
                return;
            }

            ProductSubVariant chosenVariant = bpVariant.GetSubVariant(objectContext, currentProduct, varId, true, false);
            if (chosenVariant == null)
            {
                lblError.Text = GetLocalResourceObject("errNoSuchSubVariant").ToString();
                CheckEditVariantsOptions();
                return;
            }

            if (chosenVariant.description == description.Text)
            {
                lblError.Text = GetLocalResourceObject("errNewDescr").ToString();
                return;
            }

            string strDescr = description.Text;

            if (CommonCode.Validate.ValidateDescription(0, Configuration.FieldsMaxDescriptionFieldLength
                , ref strDescr, "description", out error, 110))
            {
                editVariant.Visible = false;

                bpVariant.ChangeSubVariantDescription(objectContext, businessLog, currentUser, chosenVariant, strDescr);

                description.Text = "";

                CheckAddVariantsOptions();
                CheckEditVariantsOptions();

                FillVariants();

                if (!chosenVariant.VariantReference.IsLoaded)
                {
                    chosenVariant.VariantReference.Load();
                }

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                    , string.Format("{0} {1} : {2} {3}", GetLocalResourceObject("DescriptionUpdated")
                    , chosenVariant.Variant.name, chosenVariant.name
                    , GetLocalResourceObject("DescriptionUpdated2")));
            }


            lblError.Text = error;

        }
        protected void btnDeleteSubVariant_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            PlaceHolder editVariant = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "phEditSubVariant");
            DropDownList ddlVariants = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "ddlEditSubVariant");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditSubVariant, "tbEditSubVariantDescription");

            editVariant.Visible = true;
            editVariant.Controls.Add(lblError);
            string error = "";

            BusinessProductVariant bpVariant = new BusinessProductVariant();

            string strVariantId = ddlVariants.SelectedValue;
            long varId = 0;
            if (!long.TryParse(strVariantId, out varId))
            {
                throw new CommonCode.UIException(string.Format("Couldn`t parse ddlEditVariant.SelectedValue to long, user id : {0}", currentUser.ID));
            }

            if (varId == 0)
            {
                lblError.Text = GetLocalResourceObject("errChooseVariant").ToString();
                return;
            }

            ProductSubVariant chosenVariant = bpVariant.GetSubVariant(objectContext, currentProduct, varId, true, false);
            if (chosenVariant == null)
            {
                lblError.Text = GetLocalResourceObject("errNoSuchSubVariant").ToString();
                CheckEditVariantsOptions();
                return;
            }

            editVariant.Visible = false;

            bpVariant.DeleteSubVariant(objectContext, businessLog, currentUser, chosenVariant);
            description.Text = string.Empty;

            CheckAddVariantsOptions();
            CheckEditVariantsOptions();

            // UPDATE COMMENTS DDS
            changedVariants = true;
            WriteComment();
            // UPDATE DDL SORT BY VARIANT
            SortCommentsBy(false, true);   

            ShowAccordionsInfo();

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                , GetLocalResourceObject("SubvarDeleted").ToString());

            lblError.Text = error;
        }

        protected void ddlAboutVariant_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAboutVariant.SelectedIndex > 0)
            {
                if (ddlChars.Visible == true)
                {
                    ddlChars.SelectedIndex = 0;
                }

                if (ddlAboutSubVariant.Visible == true)
                {
                    ddlAboutSubVariant.Items.Clear();

                    ListItem item = new ListItem();
                    ddlAboutSubVariant.Items.Add(item);
                    item.Text = GetLocalResourceObject("choose...").ToString();
                    item.Value = "0";

                    // FILL DDL ABOUT SUB VARIANT

                    string strId = ddlAboutVariant.SelectedValue;
                    long id = 0;
                    if (!long.TryParse(strId, out id))
                    {
                        throw new CommonCode.UIException("Couldn`t parse ddlAboutVariant.SelectedValue to long");
                    }

                    BusinessProductVariant bpVariant = new BusinessProductVariant();
                    ProductVariant currVariant = bpVariant.Get(objectContext, currentProduct, id, true, false);

                    if (currVariant == null)
                    {
                        ddlAboutSubVariant.Enabled = false;
                        ddlAboutVariant.SelectedIndex = 0;
                        FillCommDDLAboutVariant();
                        return;
                    }
                    else
                    {
                        List<ProductSubVariant> subVariants = bpVariant.GetVisibleSubVariants(objectContext, currVariant);
                        if (subVariants.Count > 0)
                        {
                            ddlAboutSubVariant.Enabled = true;

                            foreach (ProductSubVariant variant in subVariants)
                            {
                                ListItem newItem = new ListItem();
                                ddlAboutSubVariant.Items.Add(newItem);
                                newItem.Text = variant.name;
                                newItem.Value = variant.ID.ToString();
                            }

                        }
                        else
                        {
                            ddlAboutSubVariant.Enabled = false;
                        }
                    }

                }
            }
            else
            {
                if (ddlChars.Visible == true)
                {
                    ddlChars.SelectedIndex = 0;
                }
                if (ddlAboutSubVariant.Visible == true)
                {
                    ddlAboutSubVariant.Enabled = false;

                    ddlAboutSubVariant.Items.Clear();

                    ListItem item = new ListItem();
                    ddlAboutSubVariant.Items.Add(item);
                    item.Text = GetLocalResourceObject("choose...").ToString();
                    item.Value = "0";
                }
            }
        }

        protected void ddlChars_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlChars.SelectedIndex > 0)
            {
                if (ddlAboutVariant.Visible == true)
                {
                    ddlAboutVariant.SelectedIndex = 0;
                }
                if (ddlAboutSubVariant.Visible == true)
                {
                    ddlAboutSubVariant.Enabled = false;
                    ddlAboutSubVariant.SelectedIndex = 0;
                }
            }
            else
            {
                if (ddlAboutVariant.Visible == true)
                {
                    ddlAboutVariant.SelectedIndex = 0;
                }
                if (ddlAboutSubVariant.Visible == true)
                {
                    ddlAboutSubVariant.Enabled = false;
                    ddlAboutSubVariant.SelectedIndex = 0;
                }
            }
        }

        protected void ddlSortByVariant_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSortByVariant.SelectedIndex == 0)
            {
                RedirectToCurrProductPage();
            }

            long varId = -1;
            string strVar = ddlSortByVariant.SelectedValue;
            if (string.IsNullOrEmpty(strVar))
            {
                RedirectToCurrProductPage();
            }
            string strVarID = string.Empty;
            string type = string.Empty;

            if (strVar.Contains("var"))
            {
                strVarID = strVar.Substring(3);
                type = "variant";
            }
            else if (strVar.Contains("sub"))
            {
                strVarID = strVar.Substring(3);
                type = "subvariant";
            }
            else
            {
                RedirectToCurrProductPage();
            }

            if (!long.TryParse(strVarID, out varId))
            {
                RedirectToCurrProductPage();
            }

            BusinessProductVariant bpVariant = new BusinessProductVariant();

            switch (type)
            {
                case "variant":
                    ProductVariant variant = bpVariant.Get(objectContext, currentProduct, varId, true, false);
                    if (variant == null)
                    {
                        RedirectToCurrProductPage();
                    }

                    RedirectToOtherUrl(string.Format("Product.aspx?Product={0}&variant={1}", currentProduct.ID, variant.ID));

                    break;
                case "subvariant":
                    ProductSubVariant subVariant = bpVariant.GetSubVariant(objectContext, currentProduct, varId, true, false);
                    if (subVariant == null)
                    {
                        RedirectToCurrProductPage();
                    }

                    RedirectToOtherUrl(string.Format("Product.aspx?Product={0}&subvariant={1}", currentProduct.ID, subVariant.ID));

                    break;
                default:
                    throw new CommonCode.UIException(string.Format("type = {0} is not supported type", type));
            }

        }


        public void FillVariants()
        {
            phVariants.Controls.Clear();

            BusinessProductVariant bpVariants = new BusinessProductVariant();
            List<ProductVariant> variants = bpVariants.GetVisibleVariants(objectContext, currentProduct);
            if (variants.Count > 0)
            {
                lblVariants.Text = GetLocalResourceObject("Variants").ToString();
                pnlShowVariants.CssClass = "accordionHeaders";

                apVariants.Visible = true;

                List<ProductSubVariant> subvariants = new List<ProductSubVariant>();
                int i = 0;
                foreach (ProductVariant variant in variants)
                {
                    Panel newPanel = new Panel();
                    phVariants.Controls.Add(newPanel);
                    newPanel.CssClass = "panelRows variantsPnl";

                    Panel hdrPnl = new Panel();
                    newPanel.Controls.Add(hdrPnl);
                    hdrPnl.CssClass = "variantMarginLeft";

                    hdrPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(GetLocalResourceObject("Variant2").ToString(), false));

                    Label spaceLbl = new Label();
                    hdrPnl.Controls.Add(spaceLbl);
                    spaceLbl.Text = ">>>";
                    spaceLbl.CssClass = "maringsLRVariants";

                    Label nameLbl = CommonCode.UiTools.GetLabelWithText(variant.name, false);
                    hdrPnl.Controls.Add(nameLbl);
                    nameLbl.CssClass = "textHeader";

                    Panel right = new Panel();
                    hdrPnl.Controls.Add(right);
                    right.CssClass = "floatRight";
                    right.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingRight, "9px");

                    Label commNum = new Label();
                    right.Controls.Add(commNum);
                    commNum.Text = string.Format("{0} {1}", GetLocalResourceObject("commentsSmall"), variant.comments);
                    commNum.CssClass = "commentsLarger";

                    Panel descrPnl = new Panel();
                    newPanel.Controls.Add(descrPnl);
                    descrPnl.CssClass = "padding5px";
                    descrPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(Tools.GetFormattedTextFromDB(variant.description), false));

                    subvariants = bpVariants.GetVisibleSubVariants(objectContext, variant);
                    if (subvariants.Count > 0)
                    {
                        foreach (ProductSubVariant subVariant in subvariants)
                        {
                            Panel svPanel = new Panel();
                            newPanel.Controls.Add(svPanel);
                            svPanel.CssClass = "panelRows subVariantsPnl";

                            Panel hdrSvPnl = new Panel();
                            svPanel.Controls.Add(hdrSvPnl);
                            hdrSvPnl.CssClass = "subVariantMarginLeft";

                            hdrSvPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(GetLocalResourceObject("SubVariant2").ToString(), false));

                            Label spaceSvLbl = new Label();
                            hdrSvPnl.Controls.Add(spaceSvLbl);
                            spaceSvLbl.Text = ">>>";
                            spaceSvLbl.CssClass = "maringsLRVariants";

                            Label nameSvLbl = CommonCode.UiTools.GetLabelWithText(subVariant.name, false);
                            hdrSvPnl.Controls.Add(nameSvLbl);
                            nameSvLbl.CssClass = "textHeader";

                            Panel svright = new Panel();
                            hdrSvPnl.Controls.Add(svright);
                            svright.CssClass = "floatRight";

                            Label commSvNum = new Label();
                            svright.Controls.Add(commSvNum);
                            commSvNum.Text = string.Format("{0} {1}", GetLocalResourceObject("commentsSmall"), subVariant.comments);
                            commSvNum.CssClass = "commentsLarger";

                            Panel descrSvPnl = new Panel();
                            svPanel.Controls.Add(descrSvPnl);
                            descrSvPnl.CssClass = "padding5px";
                            descrSvPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(Tools.GetFormattedTextFromDB(subVariant.description), false));
                        }
                    }

                    i++;
                }

            }
            else
            {
                apVariants.Visible = false;
            }
        }

        [WebMethod]
        public static string WMSendTypeSuggestionToUser(string userID, string type, string typeID, string description)
        {
            return CommonCode.WebMethods.SendTypeSuggestion(userID, type, typeID, description);
        }

        protected void btnChangeCanUserTakeRole_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllProductsFromEvents();

            BusinessProduct bProduct = new BusinessProduct();
            bProduct.ChangeIfUsersCanTakeActionIfThereAreNoEditors(objectContext, currentProduct, currentUser, businessLog);

            if (currentProduct.canUserTakeRoleIfNoEditors == true)
            {
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Users now CAN take roles to edit product if there are no editors!");
            }
            else
            {
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Users now CAN NOT take roles to edit product if there are no editors!");
            }

            if (currentProduct.canUserTakeRoleIfNoEditors == true)
            {
                lblCanUserTakeRoleIfNoEditors.Text = "Currently users CAN take action to edit this product if there are no editors.";
            }
            else
            {
                lblCanUserTakeRoleIfNoEditors.Text = "Currently users CAN NOT take action to edit this product if there are no editors.";
            }
        }

        protected void btnTakeAction_Click(object sender, EventArgs e)
        {
            BusinessUserTypeActions butActions = new BusinessUserTypeActions();
            List<UsersTypeAction> actions = butActions.GetProductModificators(objectContext, currentProduct.ID).ToList();
            if (actions.Count < 1 && currentProduct.canUserTakeRoleIfNoEditors == true)
            {

                butActions.AddActionForProductToUserWhenThereAreNoEditors(userContext, objectContext, currentUser, currentProduct, businessLog);

                Session["notifMsg"] = string.Format("{0} {1}.", GetLocalResourceObject("CanEditProduct"), currentProduct.name);

                RedirectToSameUrl(Request.Url.ToString());
            }
        }

        protected void btnRemCompany_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditAllProductsFromEvents();

            BusinessProduct businessProduct = new BusinessProduct();
            BusinessCompany businessCompany = new BusinessCompany();

            if (!currentProduct.CompanyReference.IsLoaded)
            {
                currentProduct.CompanyReference.Load();
            }

            Company otherCompany = businessCompany.GetOther(objectContext);

            if (currentProduct.Company.ID == otherCompany.ID)
            {
                return;
            }

            BusinessCategory businessCategory = new BusinessCategory();
            Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);
            if (!currentProduct.CategoryReference.IsLoaded)
            {
                currentProduct.CategoryReference.Load();
            }

            if (unspecified != currentProduct.Category)
            {
                businessProduct.ChangeProductCompany(objectContext, currentProduct, otherCompany, currentUser, businessLog);

                UpdateProductNotification();
                CheckCrucialEditOptions(true);
                ShowInfo();

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Company changed to Other!");

            }
            else
            {
                throw new CommonCode.UIException(string.Format("User id = {0}, cannot change product ID = {1} `s company to Other, because the product is in Unspecified category!"
                     , currentUser.ID, currentProduct.ID));
            }
        }

        protected void btnAddAlternativeName_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            PlaceHolder addAlternativeNameError = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apAddAlternativeName, "phAddAlternativeNameError");
            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddAlternativeName, "tbAlternativeNames");

            addAlternativeNameError.Visible = true;
            addAlternativeNameError.Controls.Clear();
            addAlternativeNameError.Controls.Add(lblError);
            string error = "";

            BusinessAlternativeNames bAN = new BusinessAlternativeNames();

            string strname = name.Text;
            bool nameOk = false;

            if (string.IsNullOrEmpty(name.Text))
            {
                error = GetGlobalResourceObject("Validate", "errTypeName").ToString();
            }
            else
            {
                if (name.Text != currentProduct.name)
                {
                    if (bAN.IsThereAlternativeNameForProduct(objectContext, currentProduct, strname, true) == false)
                    {
                        if (CommonCode.Validate.ValidateDescription(Configuration.ProductsMinProductNameLength, Configuration.ProductsMaxProductNameLength
                            , name.Text, "name", out error, Configuration.ProductsMaxProductNameLength) == true)
                        {
                            if (CommonCode.Validate.ValidateNamesForSpacesFormat(ref strname, out error) == true)
                            {
                                nameOk = true;
                            }
                        }
                    }
                    else
                    {
                        error = GetLocalResourceObject("errAltNameIsSameAsAnother").ToString();
                    }
                }
                else
                {
                    error = GetLocalResourceObject("errAltNameCanntBeSameAsProduct").ToString();
                }
            }

            if (bAN.CountAlternativeProductNames(objectContext, currentProduct, true) < Configuration.ProductsMaxAlternativeNames)
            {
                if (nameOk == true)
                {
                    addAlternativeNameError.Visible = false;

                    bAN.AddAlternativeNameForProduct(objectContext, currentProduct, businessLog, currentUser, strname);

                    name.Text = string.Empty;

                    ShowProductInfo();
                    CheckAddRemoveAlternativeNames();
                    FillCblRemoveAlternativeNames();

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("AlternativeNameAdded").ToString());
                }
            }
            else
            {
                CheckAddRemoveAlternativeNames();
                return;
            }

            lblError.Text = error;
        }

        protected void btnRemoveAlternativeNames_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            PlaceHolder removeAlternativeNameError = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "phRemoveAlternativeNamesError");
            CheckBoxList cblNames = (CheckBoxList)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "cblRemoveAlternativeNames");

            removeAlternativeNameError.Visible = true;
            removeAlternativeNameError.Controls.Clear();
            removeAlternativeNameError.Controls.Add(lblError);
            string error = "";

            BusinessAlternativeNames bAN = new BusinessAlternativeNames();
            List<AlternativeProductName> namesToDelete = new List<AlternativeProductName>();

            if (cblNames.Items.Count < 1)
            {
                CheckAddRemoveAlternativeNames();
                FillCblRemoveAlternativeNames();
                return;
            }
            else
            {
                AlternativeProductName currName = null;
                long nameId = 0;
                for (int i = 0; i < cblNames.Items.Count; i++)
                {
                    if (cblNames.Items[i].Selected == true)
                    {
                        if (!long.TryParse(cblNames.Items[i].Value, out nameId))
                        {
                            throw new CommonCode.UIException(string.Format("Couldn`t parse cblNames.Items[{0}] to long, in product id : {1}, user id : {2}"
                                , cblNames.Items[i].Value, currentProduct.ID, currentUser.ID));
                        }

                        currName = bAN.GetForProduct(objectContext, currentProduct, nameId, true, false);
                        if (currName == null || currName.visible == false)
                        {
                            CheckAddRemoveAlternativeNames();
                            FillCblRemoveAlternativeNames();
                            return;
                        }

                        namesToDelete.Add(currName);
                    }
                }
            }

            if (namesToDelete.Count > 0)
            {
                removeAlternativeNameError.Visible = false;

                foreach (AlternativeProductName nameToDel in namesToDelete)
                {
                    bAN.DeleteAlternativeProductName(objectContext, businessLog, currentUser, nameToDel);
                }

                ShowProductInfo();
                CheckAddRemoveAlternativeNames();
                FillCblRemoveAlternativeNames();

                if (namesToDelete.Count > 1)
                {
                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("AlternativeNamesDeleted").ToString());
                }
                else
                {
                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("AlternativeNameDeleted").ToString());
                }
            }
            else
            {
                error = GetLocalResourceObject("errSelectAltNameToDel").ToString();
            }

            lblError.Text = error;
        }

        protected void btnEditorChngCatWhenCompOther_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            if (CheckIfTimeAfterWhichCrucialProductDataCanBeEditedDidntPassed() == false)
            {
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Time passed!");
                return;
            }

            PlaceHolder phCatError = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "phEditorChngCatWhenCompOther");
            phCatError.Visible = true;
            phCatError.Controls.Add(lblError);
            string error = "";

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessCategory businessCategory = new BusinessCategory();
            BusinessProduct businessProduct = new BusinessProduct();

            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "tbNewCategory");
            Menu menu = (Menu)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "menuChooseNewCategory");

            if (string.IsNullOrEmpty(name.Text))
            {
                error = GetLocalResourceObject("errChooseCategory").ToString();

                name.Text = string.Empty;
            }
            else
            {
                string strcatid = menu.SelectedItem.Value;
                if (strcatid == null || strcatid.Length < 1)
                {
                    throw new CommonCode.UIException(string.Format("menuChooseNewCategory.SelectedItem.Value is null or empty , user id = {0}", currentUser.ID));
                }
                long catid = -1;
                long.TryParse(strcatid, out catid);
                if (catid < 1)
                {
                    throw new CommonCode.UIException(string.Format("catid is < 1 , user id = {0}", currentUser.ID));
                }

                Category category = businessCategory.Get(objectContext, catid);
                if (category == null)
                {
                    throw new CommonCode.UIException(string.Format("There`s no category ID = {0}, selected from menuChooseNewCategory, user id = {1}",
                        catid, currentUser.ID));
                }

                if (!currentProduct.CategoryReference.IsLoaded)
                {
                    currentProduct.CategoryReference.Load();
                }

                Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);

                if (category.last == true && category.ID != unspecified.ID && currentProduct.Category.ID != category.ID)
                {
                    name.Text = string.Empty;

                    businessProduct.ChangeProductCategory(userContext, objectContext, currentProduct, category, currentUser, businessLog, false);

                    ShowInfo();
                    CheckCrucialEditOptions(false);
                    LoadChooseCategoryMenuWithItems();
                    FillChangeCompanyDDl();

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("categoryChanged").ToString());

                }
                else
                {
                    name.Text = string.Empty;
                }
            }

            lblError.Text = error;
        }

        protected void menuChooseNewCategory_MenuItemClick(object sender, MenuEventArgs e)
        {
            CheckIfUserCanEditProductFromEvents();

            if (CheckIfTimeAfterWhichCrucialProductDataCanBeEditedDidntPassed() == false)
            {
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Time passed!");
                return;
            }

            BusinessCategory bCategory = new BusinessCategory();

            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "tbNewCategory");
            Menu menu = (Menu)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "menuChooseNewCategory");

            long id = 0;
            if (long.TryParse(menu.SelectedValue, out id))
            {
                Category selectedCategory = bCategory.GetWithoutVisible(objectContext, id);
                if (selectedCategory != null)
                {
                    name.Text = Tools.CategoryName(objectContext, selectedCategory, false);
                }
                else
                {
                    name.Text = GetGlobalResourceObject("SiteResources", "ErrorOccured").ToString();
                }
            }
            else
            {
                name.Text = GetGlobalResourceObject("SiteResources", "ErrorOccured").ToString();
            }

        }

        protected void btnEditorChngCat_Click(object sender, EventArgs e)
        {

            CheckIfUserCanEditProductFromEvents();

            if (CheckIfTimeAfterWhichCrucialProductDataCanBeEditedDidntPassed() == false)
            {
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Time passed!");
                return;
            }

            PlaceHolder phChngCat = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "phEditorChngCat");

            phChngCat.Visible = true;
            phChngCat.Controls.Clear();
            phChngCat.Controls.Add(lblError);
            string error = "";

            BusinessProduct businessProduct = new BusinessProduct();
            BusinessCompany businessCompany = new BusinessCompany();
            BusinessCategory businessCategory = new BusinessCategory();

            Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);

            if (!currentProduct.CategoryReference.IsLoaded)
            {
                currentProduct.CategoryReference.Load();
            }
            if (!currentProduct.CompanyReference.IsLoaded)
            {
                currentProduct.CompanyReference.Load();
            }


            DropDownList ddlCategories = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCategory, "ddlChangeCategory");
            if (ddlCategories.Items.Count < 1)
            {
                throw new CommonCode.UIException(string.Format("User id : {0}, cannot change product id : {1} category, because ddlChangeCategory is empty after button click."
                    , currentUser.ID, currentProduct.ID));
            }

            if (ddlCategories.SelectedIndex > 0)
            {

                long catId = -1;
                if (!long.TryParse(ddlCategories.SelectedValue, out catId))
                {
                    throw new CommonCode.UIException("Couldnt parse ddlChangeCategory.SelectedValue to long");
                }

                Category newCategory = businessCategory.Get(objectContext, catId);

                if (newCategory == null || newCategory.last == false || newCategory.ID == currentProduct.Category.ID
                    || newCategory.ID == unspecified.ID)
                {
                    CheckUser();
                    FillChooseCategoryDdl();
                    return;
                }

                bool mooveFromUnspecCat = false;
                if (currentProduct.Category.ID == unspecified.ID)
                {
                    mooveFromUnspecCat = true;
                }

                if (businessCompany.CheckIfCompanyCanHaveProductsInCategory(objectContext, currentProduct.Company, newCategory))
                {
                    businessProduct.ChangeProductCategory(userContext, objectContext, currentProduct, newCategory, currentUser, businessLog, mooveFromUnspecCat);

                    CheckCrucialEditOptions(false);
                    FillChooseCategoryDdl();
                    FillChangeCompanyDDl();
                    ShowInfo();

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("categoryChanged").ToString());
                }
                else
                {
                    CheckUser();
                    FillChooseCategoryDdl();
                    return;
                }


            }
            else
            {
                error = GetLocalResourceObject("errChooseCategory").ToString();
            }

            lblError.Text = error;

        }

    }
}
