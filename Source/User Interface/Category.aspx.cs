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

using DataAccess;
using BusinessLayer;
using log4net;

namespace UserInterface
{
    public partial class CategoryPage : BasePage
    {
        private ILog log = LogManager.GetLogger(typeof(CategoryPage));

        protected long ProdNumber = 0;
        protected long PageNum = 1;                                             // Current Page number
        protected long ProdOnPage = Configuration.CategoriesProdsPerPage;       // Numbers of Products to show on page
        protected char currChar = '0';                                          // Char with which orders products by ('0' for all , '1' for numbers)
        protected long prodInthisCategory = 0;                                  // Number of Products in This Category 

        protected bool canUndelete = true;                                      // used when global is logged, if false current category cannot be undeleted,made last or cannot add new sub-categories

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private Category currentCategory = null;                                // Used in methods who work with current category
        private User currentUser = null;                                        // Used in methods who need to know who the user is

        protected object addingImage = new object();                            // Used when adding/changing category image

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            lblSendReport.Attributes.Add("onclick", string.Format("ShowReportData('{0}','{1}','{2}','{3}','{4}')"
                    , "category", currentCategory.ID, lblSendReport.ClientID, pnlActionReport.ClientID, pnlSendReport.ClientID));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            currentUser = GetCurrentUser(userContext, objectContext);

            checkCategory();                                  // Checks for valid category and user

            currentCategory = getCurrentCategory();

            ShowByUser();                                     // Shows Buttons depending on user

            CheckShowHideButtons();

            CategoryInfo();                                   // Shows info about category

            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);                        // Hides notification panel                 
        }

        private void CategoryInfo()
        {

            Title = string.Format("{0} {1}", Tools.CategoryName(objectContext, currentCategory, false)
                , GetGlobalResourceObject("SiteResources", "InSite"));

            String description = Tools.GetFormattedTextFromDB(currentCategory.description);
            if (description.Length > 1)
            {
                lblDescription.Text = description;
                lblDescription.Visible = true;
            }
            else
            {
                lblDescription.Visible = false;
            }

            SetCategoryNameWithLink();   // Shows category path

            ShowCategoryImage();         // Shows Category Image if there is

            GetCategoryParams();         // Checks parameters of page

            CategoryLetters();           // Shows row with Letters for which products can be sorted
            CategoryPages(tblPages);     // Shows row with pages
            CategoryPages(tblPagesBottom);
            ShowProducts();              // Shows products

            ShowLast20Products();        // Shows 20 last products
            ShowMostCommentedProducts(); // Shows most commented products

            prodcell.Attributes.Clear();
            if (tblLast20.Rows.Count == 0 && tblMostCommented.Rows.Count == 0)
            {
                middlecell.Width = "0px";
                prodcell.Attributes.Add("class", "catPageProdColumn");
            }
            else
            {
                middlecell.Width = "300px";
                prodcell.Attributes.Add("class", "catPageProdColumn");
            }

            FillTblSearchAdd();          // Shows row with search box and Add Product option
            ShowAdvertisement();         // Shows Advertisements
            SetLocalText();

            phError.Controls.Clear();
            phError.Visible = false;
        }

        public void SetCategoryNameWithLink()
        {
            phPath.Controls.Clear();

            phPath.Controls.Add(CommonCode.UiTools.GetCategoryNameWithLink(currentCategory, objectContext, true, true, true));

        }

        private void SetLocalText()
        {
            tbSearch_TextBoxWatermarkExtender.WatermarkText = GetLocalResourceObject("tbSearch").ToString();
            btnSearch.Text = GetLocalResourceObject("btnSearch").ToString();

            lblActions.Text = GetGlobalResourceObject("SiteResources", "Actions").ToString();
            hlAddProduct.Text = GetGlobalResourceObject("SiteResources", "AddProduct").ToString();

            lblSendReport.Text = GetGlobalResourceObject("SiteResources", "lblWriteReport").ToString();
            lblReport.Text = GetGlobalResourceObject("SiteResources", "reportIrregularity").ToString();
            btnSendReport.Value = GetGlobalResourceObject("SiteResources", "btnReport").ToString();
            btnHideRepData.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();

        }

        /// <summary>
        /// Shows Category Image if there is one
        /// </summary>
        private void ShowCategoryImage()
        {
            if (currentCategory.imageUrl != null)
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

                imgCat.ImageHandlerUrl = giUrl;

                imgCat.Parameters.Clear();

                Microsoft.Web.ImageParameter catIdParam = new Microsoft.Web.ImageParameter();
                catIdParam.Name = "catID";
                catIdParam.Value = currentCategory.ID.ToString();
                imgCat.Parameters.Add(catIdParam);

                imgLink.NavigateUrl = currentCategory.imageUrl;
                imgLink.Target = "_blank";
                imgLink.Visible = true;

            }
            else
            {
                imgLink.Visible = false;
            }
        }

        private void ShowAdvertisement()
        {
            if (Configuration.AdvertsNumAdvertsOnCategoryPage > 0)
            {
                phAdvert.Controls.Clear();
                adcell.Attributes.Clear();

                phAdvert.Controls.Add(CommonCode.ImagesAndAdverts.GetAdvertisements
                    (objectContext, Server, "category", currentCategory.ID, Configuration.AdvertsNumAdvertsOnCategoryPage));
                if (CommonCode.ImagesAndAdverts.getAdvertisementsNumber(phAdvert) > 0)
                {
                    phAdvert.Visible = true;
                    adcell.Width = "252px";
                    adcell.Attributes.Add("class", "catPageAdColums");
                    adcell.VAlign = "top";
                }
                else
                {
                    phAdvert.Visible = false;
                    adcell.Width = "0px";
                }
            }
        }

        private void CheckIfUserCanEditCategoryFromEvents()
        {
            BusinessUser businessUser = new BusinessUser();
            if (currentUser != null)
            {
                if (!businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditCategories))
                {
                    throw new CommonCode.UIException(string.Format("User id = {0} cannot edit category id = {1}", currentUser.ID, currentCategory.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("Guest cannot edit category id = {0}", currentCategory.ID));
            }
        }

        private void FillTblSearchAdd()
        {
            tblSearchAdd.Rows.Clear();
            BusinessUser businessUser = new BusinessUser();

            TableRow newRow = new TableRow();

            if (ProdNumber > 0)
            {
                TableCell newCell = new TableCell();
                newCell.Width = Unit.Pixel(370);
                newCell.Controls.Add(tbSearch);
                newCell.Controls.Add(CommonCode.UiTools.GetSpaceLabel(1));
                newCell.Controls.Add(btnSearch);
                newRow.Cells.Add(newCell);
            }

            if (currentUser != null)
            {
                TableCell secCell = new TableCell();
                secCell.Controls.Add(lblActions);

                Label spaceLbl = new Label();
                spaceLbl.Text = " ";

                BusinessCategory businessCategory = new BusinessCategory();
                Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);

                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.AddProducts)
                    && currentCategory != unspecified)
                {
                    secCell.Controls.Add(spaceLbl);
                    secCell.Controls.Add(hlAddProduct);
                }

                BusinessReport businessReport = new BusinessReport();
                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate)
                    && !businessReport.IsMaxActiveIrregularityReportsReached(objectContext, currentUser))
                {
                    secCell.Controls.Add(lblSendReport);

                    if (string.IsNullOrEmpty(lblReporting.Text))
                    {
                        BusinessSiteText businessText = new BusinessSiteText();
                        SiteNews aboutReporting = businessText.GetSiteText(objectContext, "aboutUserReporting");
                        if (aboutReporting != null && !string.IsNullOrEmpty(aboutReporting.description))
                        {
                            lblReporting.Text = aboutReporting.description;
                        }
                    }
                }

                if (secCell.Controls.Count > 1)
                {
                    newRow.Cells.Add(secCell);
                }
            }

            tblSearchAdd.Rows.Add(newRow);
        }

        private void ShowByUser()
        {
            BusinessUser businessUser = new BusinessUser();
            BusinessCategory businessCategory = new BusinessCategory();

            if (currentUser != null)
            {
                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.AddProducts))
                {
                    hlAddProduct.NavigateUrl = GetUrlWithVariant(string.Format("AddProduct.aspx?Category={0}", currentCategory.ID));
                    hlAddProduct.Visible = true;
                }

                if (accAdmin.Visible) 
                {
                    if (currentCategory.visible == false || currentCategory.last == false)
                    {
                        accAdmin.RequireOpenedPane = true;
                    }

                    Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);
                    bool notUnspecified = true;
                    if (unspecified == currentCategory)
                    {
                        notUnspecified = false;
                    }

                    hlAddProduct.NavigateUrl = GetUrlWithVariant(string.Format("AddProduct.aspx?Category={0}", currentCategory.ID));
                    hlAddProduct.Visible = true;

                    lblCName.Text = "";

                    if (currentCategory.visible)
                    {
                        lblVisible.Text = string.Format("'{0}' is VISIBLE", currentCategory.name);
                    }
                    else
                    {
                        lblVisible.Text = string.Format("'{0}' is NOT VISIBLE", currentCategory.name);
                    }

                    if (notUnspecified == true)
                    {
                        lblDelete.Text = string.Format("DELETE '{0}'", currentCategory.name);

                        if (currentCategory.parentID == null)
                        {
                            lblDelete.Visible = false;
                            btnDelete.Visible = false;
                            lblDelInfo.Visible = false;

                            if (currentCategory.visible == false)
                            {
                                if (canUndelete == true)
                                {
                                    btnUndoDelete.Visible = true; 
                                    lblUndoDInfo.Visible = true;
                                }
                                else
                                {
                                    btnUndoDelete.Visible = false; 
                                    lblUndoDInfo.Visible = false;
                                }
                            }
                            else
                            {
                                btnUndoDelete.Visible = false;
                                lblUndoDInfo.Visible = false;
                            }
                        }
                        else
                        {
                            if (currentCategory.visible)
                            {
                                lblVisible.Text = string.Format("'{0}' is VISIBLE", currentCategory.name);
                                btnDelete.Visible = true; 
                                lblDelInfo.Visible = true;
                                lblDelete.Visible = true;

                                btnUndoDelete.Visible = false;
                                lblUndoDInfo.Visible = false;
                            }
                            else
                            {
                                lblVisible.Text = string.Format("'{0}' is NOT VISIBLE", currentCategory.name);

                                if (canUndelete == true)
                                {
                                    btnUndoDelete.Visible = true; 
                                    lblUndoDInfo.Visible = true;
                                }
                                else
                                {
                                    btnUndoDelete.Visible = false;
                                    lblUndoDInfo.Visible = false;
                                }

                                btnDelete.Visible = false; 
                                lblDelInfo.Visible = false;
                                lblDelete.Visible = false;

                            }
                        }
                    }
                    else
                    {
                        lblVisible.Visible = false;
                        btnUndoDelete.Visible = false;
                        lblUndoDInfo.Visible = false;

                        btnDelete.Visible = false;
                        lblDelInfo.Visible = false;
                        lblDelete.Visible = false;
                    }

                    if (IsPostBack == false)
                    {
                        // adding options to radio list
                        rblEditOptions(notUnspecified);
                    }

                    if (canUndelete == true && notUnspecified == true)
                    {
                        // DONE : It is able to set current category as Last 
                        // if there is subcategories which are visible.false 
                        if (businessCategory.IfHaveSubCategory(objectContext, currentCategory))  // if there is subcategory 
                        {
                            btnMakeLast.Visible = false; 
                            lblMakeLast.Visible = false;

                            lblLastInfo.Visible = false;

                            btnUncheckLast.Visible = false;
                            lblUncheckLast.Visible = false;
                        }
                        else if (!currentCategory.last)  // if theres no subcategory and current is not last 
                        {
                            btnMakeLast.Visible = true; 
                            lblMakeLast.Visible = true;

                            lblLastInfo.Visible = true;
                            lblLastInfo.Text = "This will enable products to be added to category, but no subcategories.";

                            btnUncheckLast.Visible = false;
                            lblUncheckLast.Visible = false;
                        }
                        else // if theres no subcategory and current is Last
                        {
                            btnMakeLast.Visible = false; 
                            lblMakeLast.Visible = false;

                            // code for unchecking category as Last, only if theres no connections with it
                            if (businessCategory.IfHaveConnections(objectContext, currentCategory))
                            {
                                btnUncheckLast.Visible = false; 
                                lblUncheckLast.Visible = false;

                                lblLastInfo.Visible = false;
                            }
                            else
                            {
                                btnUncheckLast.Visible = true; 
                                lblUncheckLast.Visible = true;

                                lblLastInfo.Visible = true;
                                lblLastInfo.Text = "This will remove option products to be added, subcategories would be able to be added.";
                            }
                        }
                    }
                    else
                    {
                        btnMakeLast.Visible = false;
                        lblMakeLast.Visible = false;

                        btnUncheckLast.Visible = false;
                        lblUncheckLast.Visible = false;

                        lblLastInfo.Visible = false;
                    }

                    BusinessProduct businessProduct = new BusinessProduct();
                    if (businessProduct.CountAllDeletedProductsFromCategory(objectContext, currentCategory) < 1)
                    {
                        btnShowDeletedProd.Visible = false;
                        lblShowDeletedProds.Visible = false;
                    }

                }
            }
            else
            {
                hlAddProduct.Visible = false;
            }
        }

        public void rblEditOptions(bool notUnspecified)
        {
            // category name 1 , description  2 , add new sub-category 3 , change parent category 4 , change image 5
            ListItem nameItem = new ListItem();
            nameItem.Text = "name ";
            nameItem.Value = "1";
            rblEdit.Items.Add(nameItem);

            ListItem descrItem = new ListItem();
            descrItem.Text = "description ";
            descrItem.Value = "2";
            rblEdit.Items.Add(descrItem);

            ListItem imgItem = new ListItem();
            imgItem.Text = "image ";
            imgItem.Value = "5";
            rblEdit.Items.Add(imgItem);

            if (!currentCategory.last && canUndelete == true && notUnspecified == true)
            {
                ListItem catItem = new ListItem();
                catItem.Text = "new sub-category ";
                catItem.Value = "3";
                rblEdit.Items.Add(catItem);
            }

            if (currentCategory.parentID != null)
            {
                ListItem parItem = new ListItem();
                parItem.Text = "parent ";
                parItem.Value = "4";
                rblEdit.Items.Add(parItem);
            }

            ListItem dispItem = new ListItem();
            dispItem.Text = "display order ";
            dispItem.Value = "6";
            rblEdit.Items.Add(dispItem);

            rblEdit.SelectedIndex = 1;
        }

        public void checkCategory()
        {
            BusinessUser businessUser = new BusinessUser();
            BusinessCategory businessCategory = new BusinessCategory();
            String CatName = Request.Params["Category"];

            if (CatName != null || CatName != "")
            {
                Category currCategory = null;

                long catID = 0;
                if (long.TryParse(CatName, out catID))
                {
                    currCategory = businessCategory.GetWithoutVisible(objectContext, catID);
                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
                }

                if (currCategory != null)
                {
                    if (currentUser != null)
                    {
                        if (!businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditCategories))
                        {
                            if (!currCategory.last)
                            {
                                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errCantSeeCat").ToString());
                            }
                            if (currCategory.visible == false)
                            {
                                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errCatDeleted").ToString());
                            }

                            if (currCategory.last && currCategory.visible)
                            {
                                pnlLastPage.Visible = true;
                            }
                            else
                            {
                                pnlLastPage.Visible = false;
                            }

                            accAdmin.Visible = false;
                        }
                        else  // global user
                        {
                            SetNeedsToBeLogged();

                            Category parentCat = businessCategory.GetParentCategory(objectContext, currCategory);
                            if (parentCat != null && parentCat.last == true)
                            {
                                canUndelete = false;
                                lblNotify.Text = "Current category cannot be UnDeleted because parent category is set to Last";
                                lblNotify.Visible = true;
                            }

                            if (currCategory.last && currCategory.visible)
                            {
                                pnlLastPage.Visible = true;
                            }
                            else
                            {
                                pnlLastPage.Visible = false;
                            }

                            accAdmin.Visible = true;
                        }

                        if (businessUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate))
                        {
                            lblSendReport.Visible = true;
                        }
                        else
                        {
                            lblSendReport.Visible = false;
                        }
                    }
                    else
                    {
                        if (!currCategory.last)
                        {
                            CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errCantSeeCat").ToString());
                        }
                        if (currCategory.last && currCategory.visible == false)
                        {
                            CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errCantSeeCat").ToString());
                        }

                        accAdmin.Visible = false;
                    }
                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errNoSuchCat").ToString());
                }
            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
            }
        }

        /// <summary>
        /// Returns Current Category
        /// </summary>
        public Category getCurrentCategory()
        {
            BusinessCategory businessCategory = new BusinessCategory();
            String CatName = Request.Params["Category"];
            long catID = 0;
            long.TryParse(CatName, out catID);
            Category currCategory = businessCategory.GetWithoutVisible(objectContext, catID);
            if (currCategory == null)
            {
                throw new CommonCode.UIException("currCategory is null");
            }
            return currCategory;
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditCategoryFromEvents();

            // category name 1 , category descr 2 , create new category 3 , change parent category 4
            // change image 5
            int selected = -1;
            int.TryParse(rblEdit.SelectedValue, out selected);

            tbEdit1.TextMode = TextBoxMode.SingleLine;
            tbEdit1.Columns = 30;
            tbEdit2.TextMode = TextBoxMode.SingleLine;
            tbEdit2.Columns = 30;
            tbEdit3.TextMode = TextBoxMode.SingleLine;
            tbEdit3.Columns = 30;

            tbEdit1.Attributes.Clear();

            if (selected > 0)
            {
                btnEdit.Enabled = false;
                rblEdit.Enabled = false;

                if (selected != 5)
                {
                    BtnSaveChanges.Visible = true;
                    btnDiscard.Visible = true;
                    pnlSubEdit.Visible = true;
                }

                switch (selected)
                {
                    //1 - new name, 2- new description, 3- new sub category, 4- new parent, 5- new image, 6 - new display order number

                    case 1:
                        long parent = 0;
                        if (currentCategory.parentID != null)
                        {
                            parent = currentCategory.parentID.Value;
                        }
                        tbEdit1.Attributes.Add("onblur", string.Format("JSCheckData('{0}','categoryAdd','{1}','{2}');", tbEdit1.ClientID, lblCName.ClientID, parent));

                        lblEdit1.Visible = true;
                        tbEdit1.Visible = true;
                        lblInfo1.Visible = true;

                        lblEdit1.Text = "New name : ";

                        lblInfo1.Text = string.Format("{0}{1}", "There can`t be categories with same name in same group.",
                        "<br />If you want to change the case of name, save as other name and then to the one with case.");

                        break;
                    case 2:
                        lblEdit2.Visible = true;
                        tbEdit2.Visible = true;
                        lblInfo2.Visible = true;

                        lblEdit2.Text = "Description : ";
                        lblInfo2.Text = "Description shouldn`t be longer than 10 rows.";

                        tbEdit2.TextMode = TextBoxMode.MultiLine;
                        tbEdit2.Rows = 5;
                        tbEdit2.Columns = 70;

                        tbEdit2.Text = Tools.GetFormattedTextFromDB(currentCategory.description);
                        break;
                    case 3:
                        tbEdit1.Attributes.Add("onblur", string.Format("JSCheckData('{0}','categoryAdd','{1}','{2}');", tbEdit1.ClientID, lblCName.ClientID, currentCategory.ID));

                        lblEdit1.Visible = true;
                        tbEdit1.Visible = true;

                        lblEdit2.Visible = true;
                        tbEdit2.Visible = true;

                        lblEdit3.Visible = true;
                        tbEdit3.Visible = true;

                        lblInfo1.Visible = true;
                        lblInfo2.Visible = true;
                        lblInfo3.Visible = true;

                        cbLast.Visible = true;
                        cbLast.Checked = false;

                        tbEdit2.Text = "0";

                        lblEdit1.Text = "New sub-category name : ";
                        lblInfo1.Text = "There can`t be categories with same name in one group.";

                        lblEdit2.Text = "Display order : ";
                        lblInfo2.Text = "0 if order is not important.";

                        lblEdit3.Text = "Description : ";
                        lblInfo3.Text = string.Format("{0}{1}", "Description should`t be longer than 10 rows.<br />"
                            , "Check last option if that category won`t have subcategories and products will be added to it.");



                        tbEdit3.TextMode = TextBoxMode.MultiLine;
                        tbEdit3.Columns = 70;
                        tbEdit3.Rows = 5;
                        break;
                    case 4:
                        if (currentCategory.parentID == null)
                        {
                            throw new CommonCode.UIException(string.Format("Category '{0}' ID = {1} cannot change its parent because it doesnt have one at the moment, user ID = {2}",
                                 currentCategory.name, currentCategory.ID, currentUser.ID));
                        }
                        lblInfo2.Visible = true;
                        lblEdit2.Visible = true;
                        tbEdit2.Visible = true;

                        lblEdit2.Text = "Id of the new parent category : ";
                        lblInfo2.Text = "Parent category cannot be: category which is last;<br /> category which is subcategory of current; current category.";
                        break;
                    case 5:
                        pnlEditImage.Visible = true;
                        if (currentCategory.imageUrl != null)
                        {
                            lblDelImg.Visible = true;
                            btnDelImg.Visible = true;

                            lblchngImage.Visible = false;
                            fuImage.Visible = false;
                            btnUpImg.Visible = false;
                            lblImgInfo.Visible = false;
                        }
                        else
                        {
                            lblchngImage.Visible = true;
                            fuImage.Visible = true;
                            btnUpImg.Visible = true;
                            lblImgInfo.Visible = true;
                        }
                        break;
                    case 6:
                        lblInfo1.Visible = true;
                        lblEdit1.Visible = true;
                        tbEdit1.Visible = true;

                        tbEdit1.Text = string.Empty;
                        lblInfo1.Text = string.Format("Current order number is {0}", currentCategory.displayOrder);
                        lblEdit1.Text = "New display order number : ";

                        break;
                    default:
                        throw new CommonCode.UIException(string.Format("rblEdit.SelectedValue = {0} is not supported , user id = {1}", rblEdit.SelectedValue, currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("rblEdit.SelectedValue = {0} is < 1 , user id = {1}", rblEdit.SelectedValue, currentUser.ID));
            }
        }

        /// <summary>
        /// Checks if New Category name and Description are Valid
        /// </summary>
        /// <param name="parentCatId">Id of parent category, 0 if the ID is null</param>
        /// <returns>true if they are valid , otherwise false</returns>
        private Boolean ValidateNewSubCategory(ref String name, String descr, long parentCatId, out string error)
        {
            Boolean pass = false;
            error = "";

            if (CommonCode.Validate.ValidateName(objectContext, "categories", ref name, Configuration.CategoriesMinCategoryNameLength,
                Configuration.CategoriesMaxCategoryNameLength, out error, parentCatId))
            {
                if (CommonCode.Validate.ValidateDescription(Configuration.CategoriesMinCategoryDescriptionLength,
                    Configuration.CategoriesMaxCategoryDescriptionLength, ref descr, "description", out error, 90))
                {
                    pass = true;
                }

            }
            return pass;
        }

        private void UpdateNavigationMenu()
        {
            MasterPage master = this.Master as MasterPage;
            master.UpdateNavigationMenuCache();
        }

        protected void BtnSaveChanges_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditCategoryFromEvents();

            phEdit.Visible = true;
            phEdit.Controls.Add(lblError);
            string error = "";

            // category name 1 , category description 2 , creating new cat 3 , new parent category 4, display order number 6
            int selected = -1;
            int.TryParse(rblEdit.SelectedValue, out selected);

            if (selected > 0)
            {
                BusinessUser businessUser = new BusinessUser();
                BusinessCategory businessCategory = new BusinessCategory();

                switch (selected)
                {
                    case 1:

                        long parentId = 0;
                        if (currentCategory.parentID != null)
                        {
                            parentId = currentCategory.parentID.Value;
                        }

                        string name = tbEdit1.Text;

                        if (CommonCode.Validate.ValidateName(objectContext, "categories", ref name, Configuration.CategoriesMinCategoryNameLength,
                            Configuration.CategoriesMaxCategoryNameLength, out error, parentId))
                        {
                            businessCategory.ChangeCategoryName(objectContext, currentCategory, name, currentUser, businessLog);

                            UpdateNavigationMenu();
                            RedirectToSameUrl(Request.Url.ToString());
                        }
                        break;
                    case 2:
                        if (tbEdit2.Text.Length > 0)
                        {
                            string description = tbEdit2.Text;

                            if (CommonCode.Validate.ValidateDescription(Configuration.CategoriesMinCategoryDescriptionLength,
                                Configuration.CategoriesMaxCategoryDescriptionLength, ref description, "description", out error, 90))
                            {
                                businessCategory.ChangeCategoryDescription(objectContext, currentCategory, description, currentUser, businessLog);
                                CategoryInfo();
                                Discard();

                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Category description updated!");
                            }
                        }
                        else
                        {
                            businessCategory.ChangeCategoryDescription(objectContext, currentCategory, tbEdit1.Text, currentUser, businessLog);
                            CategoryInfo();
                            Discard();

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Category description removed!");
                        }
                        break;
                    case 3:

                        string nscname = tbEdit1.Text;

                        if (ValidateNewSubCategory(ref nscname, tbEdit2.Text, currentCategory.ID, out error))
                        {

                            int dispNum = 0;

                            if (string.IsNullOrEmpty(tbEdit2.Text))
                            {
                                error = "Type display order number !";
                                break;
                            }
                            else if (int.TryParse(tbEdit2.Text, out dispNum))
                            {
                                if (dispNum < 0 || dispNum > 255)
                                {
                                    error = "Display number must be between 0 and 255";
                                    break;
                                }
                            }
                            else
                            {
                                error = "Type number !";
                                break;
                            }

                            if (!businessCategory.CheckIfParentCategoryIsOK(objectContext, currentCategory))
                            {
                                throw new CommonCode.UIException(string.Format("Category ID = {0} cannot be edited because parent category is Last, User ID = {1}",
                                    currentCategory.ID, currentUser.ID));
                            }

                            Category newCategory = new Category();
                            newCategory.parentID = currentCategory.ID;
                            newCategory.name = nscname;
                            newCategory.dateCreated = DateTime.UtcNow;
                            newCategory.last = cbLast.Checked;
                            newCategory.description = tbEdit3.Text;
                            newCategory.displayOrder = dispNum;
                            newCategory.CreatedBy = Tools.GetUserID(objectContext, currentUser);
                            newCategory.LastModifiedBy = newCategory.CreatedBy;
                            newCategory.lastModified = newCategory.dateCreated;
                            newCategory.visible = true;

                            businessCategory.Add(userContext, objectContext, newCategory, businessLog, currentUser);

                            UpdateNavigationMenu();
                            RedirectToSameUrl(Request.Url.ToString());
                        }
                        break;
                    case 4:
                        if (currentCategory.parentID == null)
                        {
                            throw new CommonCode.UIException(string.Format("Category '{0}' ID = {1} cannot change its parent because it doesnt have one at the moment, user ID = {2}",
                                 currentCategory.name, currentCategory.ID, currentUser.ID));
                        }

                        if (CommonCode.Validate.ValidateLong(tbEdit2.Text, out error))
                        {
                            long newParentId = -1;
                            if (long.TryParse(tbEdit2.Text, out newParentId))
                            {
                                Category parentCat = businessCategory.Get(objectContext, newParentId);
                                if (parentCat != null)
                                {
                                    if (ValidateNewParentCategory(parentCat, out error))
                                    {
                                        if (Tools.NameValidatorPassed(objectContext, "categories", currentCategory.name, newParentId))
                                        {
                                            businessCategory.ChangeParentCategory(objectContext, currentCategory, parentCat, currentUser, businessLog);

                                            UpdateNavigationMenu();
                                            RedirectToSameUrl(Request.Url.ToString());
                                        }
                                        else
                                        {
                                            error = "Cannot change parent because there is category with same name as current category. (change name)";
                                        }
                                    }
                                }
                                else
                                {
                                    error = "There`s no category with that id! (or it is visible false)";
                                }
                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format("Couldnt parse tbEdit2.Text to long, there is check before this, user ID = {0}", currentUser.ID));
                            }
                        }
                        break;
                    case 6:

                        int newDispNum = 0;

                        if (string.IsNullOrEmpty(tbEdit1.Text))
                        {
                            error = "Type display order number !";
                            break;
                        }
                        else if (int.TryParse(tbEdit1.Text, out newDispNum))
                        {
                            if (newDispNum < 0 || newDispNum > 255)
                            {
                                error = "Display number must be between 0 and 255";
                                break;
                            }
                        }
                        else
                        {
                            error = "Type number !";
                            break;
                        }

                        if (newDispNum == currentCategory.displayOrder)
                        {
                            error = "New number is same as old one.";
                            break;
                        }

                        businessCategory.ChangeDisplayOrder(objectContext, currentCategory, newDispNum, currentUser, businessLog);

                        UpdateNavigationMenu();
                        RedirectToSameUrl(Request.Url.ToString());
                        break;
                    default:
                        throw new CommonCode.UIException(string.Format("rblEdit.SelectedValue = {0} is not supported value , user id = {1}", rblEdit.SelectedValue, currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("rblEdit.SelectedValue = {0} is < 1 , user id = {1}", rblEdit.SelectedValue, currentUser.ID));
            }

            lblError.Text = error;
        }

        private Boolean ValidateNewParentCategory(Category parentCat, out string error)
        {
            Boolean passed = false;
            error = "";
            if (parentCat != null)
            {
                if (parentCat.visible == true)
                {
                    if (parentCat.last == false)
                    {
                        if (parentCat.ID != currentCategory.parentID)
                        {
                            if (parentCat.ID != currentCategory.ID)
                            {

                                BusinessCategory businessCategory = new BusinessCategory();
                                List<Category> parentCategories = businessCategory.GetAllParentCategories(objectContext, parentCat);
                                if (parentCategories.Count < 1)
                                {
                                    passed = true;
                                }
                                else
                                {
                                    passed = true;
                                    foreach (Category patCat in parentCategories)
                                    {
                                        if (patCat.ID == currentCategory.ID)
                                        {
                                            error = "Parent category cannot be a category that is sub-category of the current category.";
                                            passed = false;
                                            break;
                                        }
                                    }

                                }
                            }
                            else
                            {
                                error = "Parent category cannot be the current category.";
                            }
                        }
                        else
                        {
                            error = "New parent category is same as current parent.";
                        }
                    }
                    else
                    {
                        error = string.Format("The category '{0}' is last, it cannot have sub-categories", parentCat.name);
                    }
                }
                else
                {
                    error = string.Format("The category '{0}' is visible.false", parentCat.name);
                }
            }
            else
            {
                error = "There is no category with that ID , or it is visible.false";
            }

            return passed;
        }



        protected void btnDelete_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditCategoryFromEvents();

            if (currentCategory.parentID != null)
            {
                if (currentCategory.visible == true)
                {
                    BusinessCategory businessCategory = new BusinessCategory();

                    string error = string.Empty;
                    if (businessCategory.CheckIfCategoryIsntWantedForProducts(objectContext, currentCategory, out error) == true)
                    {
                        businessCategory.DeleteCategoryWithEveryConnection(userContext, objectContext, currentCategory, currentUser, businessLog);

                        UpdateNavigationMenu();
                        RedirectToSameUrl(Request.Url.ToString());
                    }
                    else
                    {
                        phError.Visible = true;
                        phError.Controls.Add(lblError);
                        lblError.Text = error;
                    }
                }
            }
        }

        protected void btnDiscard_Click(object sender, EventArgs e)
        {
            Discard();
        }

        public void Discard()
        {
            tbEdit1.Attributes.Clear();

            rblEdit.Enabled = true;
            btnEdit.Enabled = true;

            lblEdit1.Visible = false;
            tbEdit1.Visible = false;

            lblEdit2.Visible = false;
            tbEdit2.Visible = false;

            lblEdit3.Visible = false;
            tbEdit3.Visible = false;

            cbLast.Visible = false;
            btnDiscard.Visible = false;
            BtnSaveChanges.Visible = false;

            lblInfo1.Visible = false;
            lblInfo2.Visible = false;
            lblInfo3.Visible = false;

            phEdit.Visible = false;

            pnlSubEdit.Visible = false;
            tbEdit1.Text = "";
            tbEdit2.Text = "";
            tbEdit3.Text = "";

            phEditImg.Visible = false;
            pnlEditImage.Visible = false;
            lblDelImg.Visible = false;
            btnDelImg.Visible = false;
        }

        protected void btnMakeLast_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditCategoryFromEvents();

            if (currentCategory.last == false)
            {
                BusinessCategory businessCategory = new BusinessCategory();
                businessCategory.MakeCategoryLast(objectContext, currentCategory, currentUser, businessLog);

                UpdateNavigationMenu();
                RedirectToSameUrl(Request.Url.ToString());
            }

        }

        protected void btnUncheckLast_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditCategoryFromEvents();

            if (currentCategory.last == true)
            {
                string error = string.Empty;

                BusinessCategory businessCategory = new BusinessCategory();
                if (businessCategory.CheckIfCategoryIsntWantedForProducts(objectContext, currentCategory, out error))
                {
                    businessCategory.UnMakeCategoryLast(objectContext, currentCategory, currentUser, businessLog);

                    UpdateNavigationMenu();
                    RedirectToSameUrl(Request.Url.ToString());
                }
                else
                {
                    phError.Visible = true;
                    phError.Controls.Add(lblError);
                    lblError.Text = error;
                }

            }
        }

        protected void btnUndoDelete_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditCategoryFromEvents();

            if (currentCategory.visible == false)
            {
                BusinessCategory businessCategory = new BusinessCategory();
                businessCategory.MakeVisibleCategoryWithEveryConnection(objectContext, currentCategory, currentUser, businessLog);

                UpdateNavigationMenu();
                RedirectToSameUrl(Request.Url.ToString());
            }
        }

        public void CheckShowHideButtons()
        {
            if (btnShowDeletedProd.Text == "Hide")
            {
                ShowDeletedProductsLogs();
                tblShowDeletedProd.Visible = true;
            }
        }

        protected void btnShowDeletedProd_Click(object sender, EventArgs e)
        {
            if (btnShowDeletedProd.Text == "Show")
            {
                btnShowDeletedProd.Text = "Hide";
                ShowDeletedProductsLogs();
                tblShowDeletedProd.Visible = true;
            }
            else
            {
                btnShowDeletedProd.Text = "Show";
                tblShowDeletedProd.Visible = false;
            }
        }

        public void ShowDeletedProductsLogs()
        {
            tblShowDeletedProd.Rows.Clear();

            TableRow firstRow = new TableRow();

            TableCell ifFCell = new TableCell();
            ifFCell.Text = "id";
            ifFCell.Width = Unit.Pixel(50);
            firstRow.Cells.Add(ifFCell);

            TableCell nameFCell = new TableCell();
            nameFCell.Text = "product name";
            nameFCell.Width = Unit.Pixel(200);
            firstRow.Cells.Add(nameFCell);

            TableCell byFCell = new TableCell();
            byFCell.Text = "deleted by";
            byFCell.Width = Unit.Pixel(200);
            firstRow.Cells.Add(byFCell);

            TableCell dateFCell = new TableCell();
            dateFCell.Text = "date deleted";
            dateFCell.Width = Unit.Pixel(200);
            firstRow.Cells.Add(dateFCell);

            TableCell compFCell = new TableCell();
            compFCell.Text = "product company";
            firstRow.Cells.Add(compFCell);

            tblShowDeletedProd.Rows.Add(firstRow);

            BusinessProduct businessProduct = new BusinessProduct();
            IEnumerable<Product> products = businessProduct.GetAllDeletedProductsFromCategoryByDescending(objectContext, currentCategory.ID);
            if (products.Count<Product>() > 0)
            {
                BusinessUser businessUser = new BusinessUser();
                BusinessCompany businessCompany = new BusinessCompany();

                foreach (Product product in products)
                {
                    TableRow newRow = new TableRow();

                    TableCell idCell = new TableCell();
                    idCell.Text = product.ID.ToString();
                    newRow.Cells.Add(idCell);

                    newRow.Cells.Add(CommonCode.UiTools.GetProductLinkCell(product));

                    Log productLog = businessLog.getDeletedProduct(objectContext, product.ID);
                    if (product != null)
                    {
                        TableCell byCell = new TableCell();
                        if (!productLog.UserIDReference.IsLoaded)
                        {
                            productLog.UserIDReference.Load();
                        }
                        if (businessUser.IsUserValidType(userContext, productLog.UserID.ID))
                        {
                            byCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink(Tools.GetUserFromUserDatabase(userContext, productLog.UserID)));
                        }
                        else
                        {
                            byCell.Text = Tools.GetUserFromUserDatabase(userContext, productLog.UserID).username;
                        }
                        newRow.Cells.Add(byCell);

                        TableCell dateCell = new TableCell();
                        dateCell.Text = CommonCode.UiTools.DateTimeToLocalString(productLog.dateCreated);
                        newRow.Cells.Add(dateCell);

                    }
                    else
                    {
                        TableCell byCell = new TableCell();
                        byCell.Text = "no data";
                        newRow.Cells.Add(byCell);

                        TableCell dateCell = new TableCell();
                        dateCell.Text = "no data";
                        newRow.Cells.Add(dateCell);
                    }

                    TableCell compCell = new TableCell();
                    if (!product.CompanyReference.IsLoaded)
                    {
                        product.CompanyReference.Load();
                    }
                    if (businessCompany.IsOther(objectContext, product.Company.ID))
                    {
                        compCell.Text = product.Company.name;
                    }
                    else
                    {
                        compCell.Controls.Add(CommonCode.UiTools.GetCompanyHyperLink(product.Company));
                    }
                    newRow.Cells.Add(compCell);

                    tblShowDeletedProd.Rows.Add(newRow);
                }
            }
            else
            {
                TableRow secRow = new TableRow();
                TableCell secCell = new TableCell();
                secCell.ColumnSpan = 5;
                secCell.Text = "no data";
                secRow.Cells.Add(secCell);
                tblShowDeletedProd.Rows.Add(secRow);
            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Category currCategory = getCurrentCategory();
            RedirectToOtherUrl(string.Format("Search.aspx?catID={0}&Search={1}", currCategory.ID, tbSearch.Text));
        }


        private void GetCategoryParams()
        {
            BusinessCategory businessCategory = new BusinessCategory();
            BusinessProduct businessProduct = new BusinessProduct();
            // char - for char , page - for page number

            String strLetter = Request.Params["char"];
            char letter = '0';
            String strPage = Request.Params["page"];
            int page = -1;

            long prodNumber = businessCategory.NumberOfProductsInCategory(objectContext, currentCategory);
            prodInthisCategory = prodNumber;

            if (prodNumber > 0)
            {
                if (strLetter != null && strLetter.Length > 0 && char.TryParse(strLetter, out letter))
                {
                    if (Tools.IsCharFromEnglishAlphabet(letter, true))
                    {
                        // char is OK
                        IEnumerable<Product> products = null;
                        if (strLetter == "1")
                        {
                            products = businessProduct.GetAllProductsWhichStartsWithNumber(objectContext, currentCategory);
                        }
                        else if (strLetter == "2")
                        {
                            products = businessProduct.GetAllOtherProducts(objectContext, currentCategory);
                        }
                        else
                        {
                            products = businessProduct.GetAllProductsWhichStartsWith(objectContext, currentCategory, letter);
                        }

                        if (products.Count<Product>() > 0)
                        {
                            // there are products with the char

                            ProdNumber = products.Count<Product>();

                            currChar = letter;


                            if (strPage != null && strPage.Length > 0)
                            {

                                if (int.TryParse(strPage, out page))
                                {

                                    if (page > 1)
                                    {
                                        long expression = ProdNumber * (page - 1);
                                        if (ProdNumber > expression)
                                        {
                                            // OK
                                            PageNum = page;
                                        }
                                        else
                                        {
                                            // incorrect page number
                                            RedirectToOtherUrl(string.Format("Category.aspx?Category={0}", currentCategory.ID));
                                        }
                                    }
                                    else if (page < 1)
                                    {
                                        RedirectToOtherUrl(string.Format("Category.aspx?Category={0}", currentCategory.ID));
                                    }
                                    else
                                    {
                                        PageNum = 1;
                                    }
                                }
                                else
                                {
                                    // page parameter incorrect
                                    RedirectToOtherUrl(string.Format("Category.aspx?Category={0}", currentCategory.ID));
                                }
                            }
                            else
                            {
                                // no page param
                                PageNum = 1;
                            }
                        }
                        else
                        {
                            // no products with this char
                            RedirectToOtherUrl(string.Format("Category.aspx?Category={0}", currentCategory.ID));

                        }

                    }
                }
                else if (!string.IsNullOrEmpty(strPage) && int.TryParse(strPage, out page))
                {

                    // incorrect char

                    ProdNumber = prodNumber;

                    if (page > 1)
                    {
                        long expression = ProdOnPage * (page - 1);
                        if (ProdNumber > expression)
                        {
                            // valid page
                            PageNum = page;
                        }
                        else
                        {
                            // invalid page
                            RedirectToOtherUrl(string.Format("Category.aspx?Category={0}", currentCategory.ID));
                        }
                    }
                    else if (page < 1)
                    {
                        RedirectToOtherUrl(string.Format("Category.aspx?Category={0}", currentCategory.ID));
                    }
                    else
                    {
                        PageNum = 1;
                    }


                }
                else
                {
                    ProdNumber = prodNumber;
                    if (!string.IsNullOrEmpty(strPage))
                    {
                        RedirectToOtherUrl(string.Format("Category.aspx?Category={0}", currentCategory.ID));
                    }
                    if (!string.IsNullOrEmpty(strLetter))
                    {
                        RedirectToOtherUrl(string.Format("Category.aspx?Category={0}", currentCategory.ID));
                    }
                }

            }
            else
            {
                // no entered products
            }

        }


        private void CategoryLetters()
        {
            tblChars.Rows.Clear();

            BusinessProduct businessProduct = new BusinessProduct();

            bool hasOthers;

            List<Char> chars = businessProduct.GetCharsOnProducts(objectContext, currentCategory, out hasOthers);
            int countChars = chars.Count<Char>();
            if (countChars > 0 || hasOthers == true)
            {
                TableRow newRow = new TableRow();
                tblChars.Rows.Add(newRow);

                TableCell sortCell = new TableCell();
                newRow.Cells.Add(sortCell);
                sortCell.Text = GetLocalResourceObject("SortByLetter").ToString();

                TableCell allCell = new TableCell();
                if (currChar != '0')
                {

                    allCell.Controls.Add(CommonCode.UiTools.GetHyperLink
                        ("AllChars", GetUrlWithVariant(string.Format("Category.aspx?Category={0}", currentCategory.ID))
                        , GetLocalResourceObject("all").ToString()));

                }
                else
                {
                    allCell.Text = string.Format("{0}&nbsp;", GetLocalResourceObject("all").ToString());
                }
                newRow.Cells.Add(allCell);

                bool numbersAdded = false;
                char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

                if (countChars > 0)
                {
                    foreach (Char letter in chars)
                    {
                        if (!numbers.Contains(letter) || numbersAdded == false)
                        {
                            TableCell newCell = new TableCell();
                            if (currChar != letter)
                            {
                                if (numbers.Contains(letter))
                                {
                                    newCell.Controls.Add(CommonCode.UiTools.GetHyperLink("ShowChar{0-9}",
                                        GetUrlWithVariant(string.Format("Category.aspx?Category={0}&char=1", currentCategory.ID)),
                                        "0-9&nbsp;"));
                                    numbersAdded = true;
                                }
                                else
                                {
                                    newCell.Controls.Add(CommonCode.UiTools.GetHyperLink(string.Format("ShowChar{0}", letter),
                                        GetUrlWithVariant(string.Format("Category.aspx?Category={0}&char={1}", currentCategory.ID, letter.ToString())),
                                        string.Format("{0}&nbsp;", letter.ToString().ToUpperInvariant())));
                                }
                            }
                            else
                            {
                                if (currChar == '1')
                                {
                                    newCell.Text = "0-9&nbsp;";
                                    numbersAdded = true;
                                }
                                else
                                {
                                    newCell.Text = string.Format("{0}&nbsp;", letter.ToString().ToUpperInvariant());
                                }

                            }
                            newRow.Cells.Add(newCell);
                        }
                    }
                }

                if (hasOthers)
                {
                    TableCell othersCell = new TableCell();
                    newRow.Cells.Add(othersCell);

                    if (currChar == '2')
                    {
                        othersCell.Text = string.Format("{0}&nbsp;", GetLocalResourceObject("others").ToString());
                    }
                    else
                    {
                        othersCell.Controls.Add(CommonCode.UiTools.GetHyperLink("ShowOthers",
                        GetUrlWithVariant(string.Format("Category.aspx?Category={0}&char=2", currentCategory.ID))
                        , string.Format("{0}&nbsp;", GetLocalResourceObject("others").ToString())));
                    }

                }
            }
            else
            {
                // no chars (no entered products)
                if (ProdNumber > 0)
                {
                    throw new CommonCode.UIException("There are products which start with other characters(not english)");
                }
            }
        }

        private void CategoryPages(Table someTable)
        {
            someTable.Rows.Clear();

            String URL = "";
            if (currChar != '0')
            {
                URL = GetUrlWithVariant(string.Format("Category.aspx?Category={0}&char={1}", currentCategory.ID, currChar));
            }
            else
            {
                URL = GetUrlWithVariant(string.Format("Category.aspx?Category={0}", currentCategory.ID));
            }

            TableRow newRow = CommonCode.Pages.GetPagesRow(ProdNumber, ProdOnPage, PageNum, URL);
            someTable.Rows.Add(newRow);
        }

        private void ShowProducts()
        {
            tblProducts.Rows.Clear();

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessProduct businessProduct = new BusinessProduct();

            if (ProdNumber > 0)
            {
                TableRow firstRow = new TableRow();

                TableCell nameFCell = new TableCell();
                firstRow.Cells.Add(nameFCell);

                nameFCell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "25px");
                nameFCell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingBottom, "5px");
                nameFCell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingTop, "2px");
                nameFCell.CssClass = "searchPageComments textHeaderWA";

                nameFCell.Controls.Add(CommonCode.UiTools.GetLabelWithText
                    (GetGlobalResourceObject("SiteResources", "ProductL").ToString(), false));

                Panel newPanel = new Panel();
                nameFCell.Controls.Add(newPanel);
                newPanel.CssClass = "floatRightNoMrg searchPageComments textHeaderWA";
                newPanel.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingRight, "15px");
                newPanel.Controls.Add(CommonCode.UiTools.GetLabelWithText
                    (GetGlobalResourceObject("SiteResources", "CompanyL").ToString(), false));

                tblProducts.Rows.Add(firstRow);


                long from = ProdOnPage * (PageNum - 1);
                long to = ProdOnPage * PageNum;

                IEnumerable<Product> products;
                if (currChar == '0')
                {
                    products = businessProduct.GetAllProductsFromCategory(objectContext, currentCategory.ID, from, to);
                }
                else if (currChar == '1')
                {
                    products = businessProduct.GetAllProductsWhichStartsWithNumber(objectContext, currentCategory, from, to);
                }
                else if (currChar == '2')
                {
                    products = businessProduct.GetAllOtherProducts(objectContext, currentCategory, from, to);
                }
                else
                {
                    products = businessProduct.GetAllProductsWhichStartsWith(objectContext, currentCategory, currChar, from, to);
                }

                int i = 0;
                foreach (Product product in products)
                {
                    ShowProduct(product, i);
                    i++;
                }

            }
            else
            {
                TableRow newRow = new TableRow();
                TableCell newCell = new TableCell();
                newCell.Text = GetLocalResourceObject("noAddedProducts").ToString();
                newCell.CssClass = "searchPageComments";
                newRow.Cells.Add(newCell);
                tblProducts.Rows.Add(newRow);

                tbSearch.Visible = false;
                btnSearch.Visible = false;
            }
        }

        private void ShowProduct(Product currProduct, int rowNum)
        {
            BusinessCompany businessCompany = new BusinessCompany();

            TableRow newRow = new TableRow();

            Image newImg = new Image();
            newImg.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, "5px");
            newImg.ImageUrl = "~/images/SiteImages/triangle.png";
            newImg.CssClass = "itemImage";

            string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);

            TableCell nameCell = new TableCell();
            newRow.Cells.Add(nameCell);

            nameCell.Controls.Add(newImg);

            HyperLink productLink = CommonCode.UiTools.GetProductHyperLink(currProduct);
            productLink.ID = string.Format("prod{0}lnk", currProduct.ID);
            productLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}{2}','{3}')", currProduct.ID, contPageId, productLink.ClientID, pnlPopUp.ClientID));
            productLink.Attributes.Add("onmouseout", "HideData()");
            nameCell.Controls.Add(productLink);
            productLink.Text = Tools.BreakLongWordsInString(currProduct.name, 35);

            if (!currProduct.CompanyReference.IsLoaded)
            {
                currProduct.CompanyReference.Load();
            }

            Panel newPanel = new Panel();
            nameCell.Controls.Add(newPanel);
            newPanel.CssClass = "floatRightNoMrg";
            newPanel.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingRight, "5px");

            if (!businessCompany.IsOther(objectContext, currProduct.Company))
            {
                HyperLink compLink = CommonCode.UiTools.GetCompanyHyperLink(currProduct.Company);
                compLink.ID = string.Format("p{0}comp{1}lnk", currProduct.ID, currProduct.Company.ID);
                compLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}{2}','{3}')", currProduct.Company.ID, contPageId, compLink.ClientID, pnlPopUp.ClientID));
                compLink.Attributes.Add("onmouseout", "HideData()");
                newPanel.Controls.Add(compLink);
                compLink.CssClass = "brown";

            }
            else
            {
                newPanel.Controls.Add(CommonCode.UiTools.GetLabelWithText(currProduct.Company.name, false));
            }

            tblProducts.Rows.Add(newRow);
        }

        private void ShowLast20Products()
        {
            tblLast20.Rows.Clear();

            int minNumOfProdToShowThisTbl = Configuration.CategoriesMinProdNumToShowLastProductsTbl;
            int prodToShow = Configuration.CategoriesNumOfProductsToShowInLastProductsTbl;

            if (prodInthisCategory > minNumOfProdToShowThisTbl)
            {
                BusinessProduct businessProduct = new BusinessProduct();
                BusinessCompany businessCompany = new BusinessCompany();

                TableRow zeroRow = new TableRow();
                TableCell zeroCell = new TableCell();

                if (prodToShow > prodInthisCategory)
                {
                    prodToShow = (int)prodInthisCategory;
                }

                zeroCell.Text = string.Format("{0} {1} {2}", GetLocalResourceObject("LastProducts"), prodToShow
                    , GetLocalResourceObject("LastProducts2"));
                zeroCell.HorizontalAlign = HorizontalAlign.Center;
                zeroCell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingBottom, "5px");
                zeroCell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingTop, "2px");
                zeroRow.Cells.Add(zeroCell);
                zeroCell.CssClass = "textHeaderWA";
                tblLast20.Rows.Add(zeroRow);

                IEnumerable<Product> products = businessProduct.GetLastAddedProductsInCategory(objectContext, currentCategory, prodToShow);

                string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);

                int i = 0;

                if (products.Count() < 1)
                {
                    throw new CommonCode.UIException("Products in List which should contain last added products are 0");
                }

                foreach (Product product in products)
                {
                    TableRow newRow = new TableRow();
                    tblLast20.Rows.Add(newRow);

                    Image newImg = new Image();
                    newImg.ImageUrl = "~/images/SiteImages/triangle.png";
                    newImg.CssClass = "itemImage";
                    newImg.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "5px");

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);
                    nameCell.Controls.Add(newImg);
                    HyperLink productLink = CommonCode.UiTools.GetProductHyperLink(product);
                    productLink.ID = string.Format("prod{0}lnk{1}", product.ID, i);
                    productLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}{2}','{3}')", product.ID, contPageId, productLink.ClientID, pnlPopUp.ClientID));
                    productLink.Attributes.Add("onmouseout", "HideData()");
                    nameCell.Controls.Add(productLink);
                    productLink.Text = Tools.BreakLongWordsInString(product.name, 25);

                    if (!product.CompanyReference.IsLoaded)
                    {
                        product.CompanyReference.Load();
                    }

                    i++;
                }
            }
        }

        private void ShowMostCommentedProducts()
        {
            tblMostCommented.Rows.Clear();

            if (prodInthisCategory > 0)
            {
                int prodToShow = Configuration.CategoriesNumOfMostCommentedProducts;

                BusinessProduct businessProduct = new BusinessProduct();

                IEnumerable<Product> products = businessProduct
                    .GetMostCommentedProductsInCategory(objectContext, currentCategory, prodToShow);

                if (products.Count<Product>() > 0)
                {

                    tblMostCommented.Visible = true;

                    TableRow zeroRow = new TableRow();
                    TableCell zeroCell = new TableCell();
                    zeroCell.ColumnSpan = 2;
                    zeroCell.HorizontalAlign = HorizontalAlign.Center;
                    zeroCell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingBottom, "5px");
                    zeroCell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingTop, "2px");
                    zeroCell.Text = GetLocalResourceObject("MostCommentedProducts").ToString();
                    zeroRow.Cells.Add(zeroCell);
                    zeroCell.CssClass = "textHeaderWA";
                    tblMostCommented.Rows.Add(zeroRow);

                    string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);

                    int i = 0;

                    foreach (Product product in products)
                    {
                        TableRow row = new TableRow();
                        tblMostCommented.Rows.Add(row);

                        Image newImg = new Image();
                        newImg.ImageUrl = "~/images/SiteImages/triangle.png";
                        newImg.CssClass = "itemImage";
                        newImg.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "5px");

                        TableCell nameCell = new TableCell();
                        row.Cells.Add(nameCell);
                        nameCell.Controls.Add(newImg);
                        HyperLink productLink = CommonCode.UiTools.GetProductHyperLink(product);
                        productLink.ID = string.Format("prod{0}lnk{1}Comm", product.ID, i);
                        productLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}{2}','{3}')", product.ID, contPageId, productLink.ClientID, pnlPopUp.ClientID));
                        productLink.Attributes.Add("onmouseout", "HideData()");
                        nameCell.Controls.Add(productLink);
                        productLink.Text = Tools.BreakLongWordsInString(product.name, 25);

                        TableCell numCell = new TableCell();
                        numCell.Width = Unit.Pixel(30);
                        numCell.CssClass = "searchPageComments";
                        numCell.HorizontalAlign = HorizontalAlign.Center;
                        numCell.Text = product.comments.ToString();
                        row.Cells.Add(numCell);

                        i++;
                    }
                }
                else
                {
                    tblMostCommented.Visible = false;
                }
            }
            else
            {
                tblMostCommented.Visible = false;
            }
        }


        [WebMethod]
        public static string CheckData(string text, string type, string parentCatID)
        {
            string error = "";

            CommonCode.WebMethods.ValidateUserInput(text, type, parentCatID, out error);

            return error;
        }

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }

        protected void btnCancImg_Click(object sender, EventArgs e)
        {
            Discard();
        }

        protected void btnDelImg_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditCategoryFromEvents();

            ImageTools imageTools = new ImageTools();

            if (currentCategory.imageUrl != null)
            {
                string appPath = CommonCode.PathMap.PhysicalApplicationPath;  // CommonCode.PathMap.GetImagesPhysicalPathRoot();

                if (log.IsInfoEnabled == true)
                {
                    string logMsg = ImageTools.DeletingImageLogMessage(currentCategory.ID, currentCategory.imageUrl, "user request");
                    log.Info(logMsg);
                }

                if (imageTools.DeleteCategoryImage(userContext, objectContext, currentCategory, businessLog, appPath, currentUser) == true)
                {
                    ShowCategoryImage();
                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Category image removed!");
                }
                else
                {
                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "COULDN`T delete category image!");
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("Category '{0}' ID = {1} doesn`t have image , user id = {2}"
                    , currentCategory.name, currentCategory.ID, currentUser.ID));
            }

            Discard();
        }

        protected void btnUpImg_Click(object sender, EventArgs e)
        {
            CheckIfUserCanEditCategoryFromEvents();

            if (currentCategory.imageUrl != null)
            {
                throw new CommonCode.UIException(string.Format("In category '{0}' ID = {1} new image cannot be uploaded because it already have one, user id = {2}"
                    , currentCategory.name, currentCategory.ID, currentUser.ID));
            }

            phEditImg.Visible = true;
            phEditImg.Controls.Add(lblError);
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
                    int width = 0;
                    int height = 0;

                    if (ImageTools.GetImageWidthAndHeight(fileBytes, out width, out height))
                    {
                        ImageTools imageTools = new ImageTools();

                        bool deletedOldImage = true;

                        lock (addingImage)
                        {
                            string appPath = CommonCode.PathMap.PhysicalApplicationPath; 

                            if (currentCategory.imageUrl != null) // Delete old image if there is
                            {
                                string currImgPath = System.IO.Path.Combine(appPath, currentCategory.imageUrl);
                                deletedOldImage = imageTools.DeleteImageFromHD(currImgPath);
                            }

                            if (deletedOldImage == false)
                            {
                                String url;
                                CommonCode.ImagesAndAdverts.GenerateCategoryImageNamePathUrl(fileType, currentCategory, out url);

                                string imagePathFromRoot = System.IO.Path.Combine(appPath, url);
                                fuImage.SaveAs(imagePathFromRoot);

                                currentCategory.imageWidth = width;
                                currentCategory.imageHeight = height;
                                currentCategory.imageUrl = url;
                                objectContext.SaveChanges();
                            }
                        }

                        if (deletedOldImage == true)
                        {
                            Discard();
                            ShowCategoryImage();

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Category image uploaded!");
                        }
                        else
                        {
                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Couldn`t delete old category image. New image save aborted!");
                        }
                    }
                    else
                    {
                        error = "Incorrect image width/height or file is damaged.";
                    }
                }
                else
                {
                    error = string.IsNullOrEmpty(imageErrorDescription) ? "Error uploading the file." : imageErrorDescription;
                }
            }
            else
            {
                error = "Choose file to upload.";
            }

            lblError.Text = error;

        }

        [WebMethod]
        public static string WMSendReport(string type, string strTypeId, string description)
        {
            return CommonCode.WebMethods.SendReport(type, strTypeId, description);
        }

    }
}
