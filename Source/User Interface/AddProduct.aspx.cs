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

namespace UserInterface
{
    public partial class AddProduct : BasePage 
    {
        private User currentUser = null;

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private Company chosenCompany = null;
        private Category chosenCategory = null;

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            tbName.Attributes.Add("onblur", string.Format("JSCheckData('{0}','productnameAdd','{1}',''); GetSimilarNames('{2}', '{3}', '{4}', '{5}');"
                , tbName.ClientID, lblCName.ClientID
                , pnlSimilarNames.ClientID, "product", tbName.ClientID, pnlPopUp.ClientID));

            tbDescr.Attributes.Add("onKeyUp", string.Format("ShowCharsCountInField('{0}', '{1}', '{2}', '{3}');"
                , tbDescr.ClientID, tbSymbolsCount.ClientID, Configuration.ProductsMinDescriptionLength, Configuration.ProductsMaxDescriptionLength));
            tbDescr.Attributes.Add("onBlur", string.Format("ShowCharsCountInField('{0}', '{1}', '{2}', '{3}');"
                , tbDescr.ClientID, tbSymbolsCount.ClientID, Configuration.ProductsMinDescriptionLength, Configuration.ProductsMaxDescriptionLength));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetNeedsToBeLogged();
            // checks Company/Category parameters and fills DropDownLists
            CheckParams();
            if (currentUser == null)
            {
                throw new CommonCode.UIException("currentUser is null");
            }

            ShowInfo();
        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            lblErrorChooseCom.Visible = false;

            BusinessSiteText siteText = new BusinessSiteText();

            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutAddProduct");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblAbout.Text = aboutExtended.description;
            }
            else
            {
                lblAbout.Text = "About Add Product text not typed.";
            }

            tbSymbolsCount.Text = tbDescr.Text.Length.ToString();

            SetLocalText();

            if (Configuration.ProductsMinDescriptionLength < 1)
            {
                rfvDescription.Enabled = false;
            }
        }

        private void SetLocalText()
        {
            lblPageIntro.Text = GetLocalResourceObject("pageIntro").ToString();

            btnProceedCompany.Text = GetGlobalResourceObject("SiteResources", "Proceed").ToString();
            lblChooseComp.Text = GetLocalResourceObject("company").ToString();
            lblChooseCat.Text = GetLocalResourceObject("category").ToString();
            lblChoose.Text = GetLocalResourceObject("chooseCatOrComp").ToString();

            hlAddProdGuide.NavigateUrl = GetUrlWithVariant("Guide.aspx#infaeproducts");
            hlAddProdGuide.Text = GetLocalResourceObject("hlAddProdGuide").ToString();
            hlAddProdGuide.Target = "_blank";
            lblAddProdGuide.Text = GetLocalResourceObject("lblAddProdGuide").ToString();

            hlAddProdRules.NavigateUrl = GetUrlWithVariant("Rules.aspx#rulesaeprod");
            hlAddProdRules.Text = GetLocalResourceObject("hlAddProdRules").ToString();
            hlAddProdRules.Target = "_blank";
            lblAddProdRules.Text = GetLocalResourceObject("lblAddProdRules").ToString();

            lblCName.Text = "";
            lblCompany.Text = string.Format("{0} :"
                , Tools.GetStringWithCapital(GetGlobalResourceObject("SiteResources", "CompanyName").ToString()));

            lblCompHint1.Text = GetLocalResourceObject("CompHint1").ToString();
            lblCompHint2.Text = GetLocalResourceObject("CompHint2").ToString();
            lblCompHint3.Text = GetLocalResourceObject("CompHint3").ToString();
            lblCompHint4.Text = GetLocalResourceObject("CompHint4").ToString();
            hlCompHint4.Text = GetLocalResourceObject("HlCompHint4").ToString();
            hlCompHint4.NavigateUrl = GetUrlWithVariant("Guide.aspx#infts");

            lblCatHint1.Text = GetLocalResourceObject("CatHint1").ToString();
            lblCatHint2.Text = GetLocalResourceObject("CatHint2").ToString();
            lblCatHint3.Text = GetLocalResourceObject("CatHint3").ToString();


            lblProductData.Text = GetLocalResourceObject("productData").ToString();
            lblName.Text = GetLocalResourceObject("name").ToString();
            lblCategory.Text = GetLocalResourceObject("category").ToString();
            lblSite.Text = GetLocalResourceObject("site").ToString();
            lblDescription.Text = GetLocalResourceObject("description").ToString();
            btnSubmit.Text = GetLocalResourceObject("submit").ToString();

            lblNameRules.Text = string.Format("{0} {1}-{2} {3}.", GetLocalResourceObject("nameRules")
              , Configuration.ProductsMinProductNameLength, Configuration.ProductsMaxProductNameLength
              , GetLocalResourceObject("characters"));

            lblDescriptionRules.Text = string.Format("{0} {1}-{2} {3}", GetLocalResourceObject("descrRules"),
                Configuration.ProductsMinDescriptionLength, Configuration.ProductsMaxDescriptionLength
                , GetLocalResourceObject("descrRules2"));

            lblSymbolsCount.Text = GetLocalResourceObject("textLength").ToString();

            lblSiteRules.Text = GetLocalResourceObject("siteRules").ToString();
        }

        /// <summary>
        /// checks Company/Category parameters and fills DropDownLists
        /// </summary>
        private void CheckParams()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessCategory businessCategory = new BusinessCategory();

            Category currCategory = null;
            Company currCompany = null;

            String CatID = Request.Params["Category"];
            String CompID = Request.Params["Company"];

            if (!string.IsNullOrEmpty(CatID) && !string.IsNullOrEmpty(CompID))
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
            }

            if (!string.IsNullOrEmpty(CatID))
            {
                long catID = -1;
                if (!long.TryParse(CatID, out catID))
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
                }

                currCategory = businessCategory.Get(objectContext, catID);

                if (currCategory == null || currCategory.visible == false || currCategory.last == false)
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
                }

                Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);
                if (unspecified == currCategory)
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errCantAddInThisCategory").ToString());
                }

                chosenCategory = currCategory;
            }

            if (!string.IsNullOrEmpty(CompID))
            {
                long compID = -1;
                if (!long.TryParse(CompID, out compID))
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
                }

                currCompany = businessCompany.GetCompany(objectContext, compID);

                Company otherCompany = businessCompany.GetOther(objectContext);

                if (currCompany == null || currCompany == otherCompany)
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
                }

                chosenCompany = currCompany;
            }


            if (currUser != null)
            {

                if (businessUser.CanUserDo(userContext, currUser, UserRoles.AddProducts))
                {
                    currentUser = currUser;
                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errCantAdd").ToString());
                }

                if (currCompany == null && currCategory == null)
                {
                    //Choose category ot company

                    pnlAddProductForm.Visible = false;
                    pnlChooseCategoryOrCompany.Visible = true;

                    if (IsPostBack == false)
                    {
                        LoadChooseCategoryMenuWithItems();          // loads choose category menu with items
                    }
                }
                else
                {
                    pnlAddProductForm.Visible = true;
                    pnlChooseCategoryOrCompany.Visible = false;

                    pnlAddProduct.Visible = true;
                    if (currCompany != null)
                    {
                        bool dontAddUnspecifiedCat = IsCurrUserEditorOfChosenCompany(businessUser);

                        Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);

                        ListItem newItem = new ListItem();
                        newItem.Text = currCompany.name;
                        newItem.Value = currCompany.ID.ToString();
                        ddCompany.Items.Add(newItem);
                        ddCompany.Enabled = false;

                        /// getting categories in which company can add products
                        IEnumerable<Category> companyCategories = businessCompany.GetCompanyCategories(objectContext, currCompany.ID);

                        if (dontAddUnspecifiedCat == true && companyCategories.Count<Category>() < 2)
                        {   // redirects editor to error page when the company have only 1 category (the unspecified one)
                            CommonCode.UiTools.RedirrectToErrorPage(Response, Session,
                                GetLocalResourceObject("errEditorShouldAddCatToCompanyFirst").ToString());
                        }

                        if (IsPostBack == false)
                        {
                            FillCategoryDdl(businessCompany, dontAddUnspecifiedCat, unspecified);
                        }

                        if (dontAddUnspecifiedCat == false)
                        {
                            if (IsPostBack == false)
                            {
                                LoadWantedCategoryMenuWithItems();
                            }
                            pnlWantedCategory.Visible = true;
                            lblUnspecifiedCategoryInfo.Text = GetLocalResourceObject("addToUnspecifiedCatInfo").ToString();
                            ddCategory.Attributes.Add("onchange", string.Format("ShowWantedCategoryPanel('{0}', '{1}', '{2}')"
                                , unspecified.ID, pnlWantedCategory.ClientID, ddCategory.ClientID));
                        }
                    }
                    else if (currCategory != null)
                    {
                        pnlWantedCategory.Visible = false;

                        ListItem newItem = new ListItem();
                        newItem.Text = Tools.TrimString(Tools.CategoryName(objectContext, currCategory, false), 67, true, true);
                        newItem.Value = currCategory.ID.ToString();
                        ddCategory.Items.Add(newItem);
                        ddCategory.Enabled = false;

                        if (IsPostBack == false)
                        {
                            FillCompaniesInDDl(businessCompany);
                        }
                    }

                }
            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errLogIn").ToString());
            }
        }

        private bool IsCurrUserEditorOfChosenCompany(BusinessUser businessUser)
        {
            bool result = false;

            if (chosenCompany == null)
            {
                throw new CommonCode.UIException("chosenCompany is null");
            }

            if (businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditCompanies) == true ||
                businessUser.CanUserModifyCompany(objectContext, chosenCompany.ID, currentUser.ID) == true)
            {
                result = true;
            }
            return result;
        }

        private void FillCategoryDdl(BusinessCompany businessCompany, bool dontAddUnspecifiedCat, Category unspecified)
        {
            if (chosenCompany == null)
            {
                throw new CommonCode.UIException("chosenCompany is null");
            }

            IEnumerable<Category> companyCategories = businessCompany.GetCompanyCategories(objectContext, chosenCompany.ID);

            ddCategory.Items.Clear();

            if (companyCategories.Count<Category>() > 0)
            {
                ListItem firstItem = new ListItem();
                firstItem.Text = GetLocalResourceObject("choose").ToString();
                firstItem.Value = "0";
                ddCategory.Items.Add(firstItem);

                foreach (Category category in companyCategories)
                {
                    if (dontAddUnspecifiedCat == false || category.ID != unspecified.ID)
                    {
                        ListItem newItem2 = new ListItem();
                        newItem2.Text = Tools.TrimString(Tools.CategoryName(objectContext, category, false), 67, true, true);
                        newItem2.Value = category.ID.ToString();
                        ddCategory.Items.Add(newItem2);
                    }
                }

            }
            else
            {
                // company doesnt have added categories
                throw new CommonCode.UIException(string.Format("User ID = {0} , cannot add product with company ID = {1}" +
                    " ,because the company cannot have products (no categories in which company can add).", currentUser.ID, chosenCompany.ID));
            }
        }

        private void FillCompaniesInDDl(BusinessCompany businessCompany)
        {
            if (chosenCategory == null)
            {
                throw new CommonCode.UIException("chosenCategory is null");
            }

            ddCompany.Items.Clear();

            /// getting companies who can have products in this category
            IEnumerable<Company> CompanyList = businessCompany.GetCompaniesByCategory(objectContext, chosenCategory.ID);

            if (CompanyList.Count<Company>() == 0)
            {
                throw new CommonCode.UIException(string.Format("User ID = {0} , cannot add products" +
                    " in category ID = {0} , because there arent companies who can add in it. (even other)"
                    , currentUser.ID, chosenCategory.ID));
            }

            ListItem firstItem = new ListItem();
            firstItem.Text = GetLocalResourceObject("choose").ToString();
            firstItem.Value = "0";
            ddCompany.Items.Add(firstItem);

            foreach (Company company in CompanyList)
            {
                ListItem newItem2 = new ListItem();
                newItem2.Value = company.ID.ToString();
                newItem2.Text = company.name;
                ddCompany.Items.Add(newItem2);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            if (businessUser.CanUserDo(userContext, currentUser, UserRoles.AddProducts))
            {
                lblError.Visible = true;
                String error = "";

                BusinessProduct businessProduct = new BusinessProduct();
                int minToWait = 0;
                if (businessProduct.CheckIfMinimumTimeBetweenAddingProductsPassed(objectContext, currentUser, out minToWait) == true)
                {

                    string name = tbName.Text;

                    if (CommonCode.Validate.ValidateName(objectContext, "products", ref name, Configuration.ProductsMinProductNameLength,
                        Configuration.ProductsMaxProductNameLength, out error, 0))
                    {

                        string description = tbDescr.Text;

                        if (CommonCode.Validate.ValidateDescription(Configuration.ProductsMinDescriptionLength,
                            Configuration.ProductsMaxDescriptionLength, ref description, "description", out error, 110))
                        {
                            if (CommonCode.Validate.ValidateSiteAdress(tbSite.Text, out error, true))
                            {
                                Category wantedCategory = null;

                                if (ValidateCategoryAndCompany(out wantedCategory, out error) == true)
                                {
                                    lblError.Visible = false;

                                    BusinessCategory businessCategory = new BusinessCategory();
                                    BusinessCompany businessCompany = new BusinessCompany();
                                    BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

                                    Category currCategory = null;
                                    Company currCompany = null;

                                    long compID = -1;
                                    if (long.TryParse(ddCompany.SelectedValue, out compID))
                                    {
                                        currCompany = businessCompany.GetCompany(objectContext, compID);
                                    }
                                    else
                                    {
                                        throw new CommonCode.UIException(string.Format("couldnt parse ddCompany.SelectedValue to long" +
                                            " , user ID = {0} , there are validators before function checking for this.", currentUser.ID));
                                    }

                                    long catID = -1;
                                    if (long.TryParse(ddCategory.SelectedValue, out catID))
                                    {
                                        currCategory = businessCategory.Get(objectContext, catID);
                                    }
                                    else
                                    {
                                        throw new CommonCode.UIException(string.Format("couldnt parse ddCategory.SelectedValue to long" +
                                            " , user ID = {0} , there are validators before function checking for this.", currentUser.ID));
                                    }

                                    businessProduct.AddProduct(userContext, objectContext, businessLog, currCompany
                                        , currCategory, currentUser, name, description, tbSite.Text, wantedCategory);

                                    Product newProduct = businessProduct.GetProductByName(objectContext, name);
                                    if (newProduct == null)
                                    {
                                        throw new CommonCode.UIException(string.Format("Product with name : {0} is not found right after its creation, User Id : {1}"
                                            , name, currentUser.ID));
                                    }

                                    lblError.Visible = false;
                                    pnlAddProductForm.Visible = false;

                                    pnlAddSucc.Visible = true;
                                    lblAddSucc.Text = GetLocalResourceObject("addSucc").ToString();

                                    lblGoToProdsPage.Text = GetLocalResourceObject("goTo").ToString();
                                    lblGoToProdsPage2.Text = GetLocalResourceObject("goTo2").ToString();

                                    hlProductPage.Text = newProduct.name;
                                    hlProductPage.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", newProduct.ID));

                                    FillBulletedLists();
                                }
                            }
                        }
                    }
                }
                else
                {
                    error = string.Format("{0} {1} {2}", GetLocalResourceObject("errMinTimeAfterAddingProductDidntPass")
                        , minToWait, GetLocalResourceObject("errMinTimeAfterAddingProductDidntPass2"));
                }

                lblError.Text = error;

                BusinessCategory bCategory = new BusinessCategory();
                Category unspecified = bCategory.GetUnspecifiedCategory(objectContext);

                if (ddCategory.SelectedValue == unspecified.ID.ToString())
                {
                    pnlWantedCategory.Attributes.Clear();
                }
                else
                {
                    pnlWantedCategory.Attributes.CssStyle.Add(HtmlTextWriterStyle.Visibility, "hidden");
                }

            }
            else
            {
                throw new CommonCode.UIException(string.Format("User ID = {0} cannot add product", currentUser.ID));
            }
        }

        private void FillBulletedLists()
        {

            blToDo.Items.Clear();
            blToDo.Items.Add(GetListItem(GetLocalResourceObject("blAddVariants").ToString()));
            blToDo.Items.Add(GetListItem(GetLocalResourceObject("blModifDescription").ToString()));
            blToDo.Items.Add(GetListItem(GetLocalResourceObject("blAddAltNames").ToString()));
            blToDo.Items.Add(GetListItem(GetLocalResourceObject("blAddCharacteristics").ToString()));
            blToDo.Items.Add(GetListItem(GetLocalResourceObject("blFillGallery").ToString()));
        }

        private ListItem GetListItem(string text)
        {
            ListItem newItem = new ListItem();
            newItem.Text = text;
            return newItem;
        }

        private bool ValidateCategoryAndCompany(out Category wantedCategory, out string error)
        {
            wantedCategory = null;
            error = string.Empty;
            bool passed = true;

            BusinessCategory businessCategory = new BusinessCategory();
            BusinessCompany businessCompany = new BusinessCompany();

            Category currCategory = null;
            Company currCompany = null;

            if (ddCompany.SelectedValue == "0")
            {
                error = GetLocalResourceObject("errChooseCompany").ToString();
                return false;

            }
            else if (ddCategory.SelectedValue == "0")
            {
                error = GetLocalResourceObject("errChooseCategory").ToString();
                return false;
            }

            long compID = -1;
            if (long.TryParse(ddCompany.SelectedValue, out compID))
            {
                currCompany = businessCompany.GetCompany(objectContext, compID);
            }
            else
            {
                throw new CommonCode.UIException(string.Format("couldnt parse ddCompany.SelectedValue to long" +
                    " , user ID = {0}.", currentUser.ID));
            }

            long catID = -1;
            if (long.TryParse(ddCategory.SelectedValue, out catID))
            {
                currCategory = businessCategory.Get(objectContext, catID);
            }
            else
            {
                throw new CommonCode.UIException(string.Format("couldnt parse ddCategory.SelectedValue to long" +
                    " , user ID = {0}.", currentUser.ID));
            }

            BusinessUser bUser = new BusinessUser();
            Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);
            ///checks chosen company and category for null, visible=false etc...
            if (currCategory == null || currCategory.visible == false || currCategory.last == false)
            {
                passed = false;

                error = GetLocalResourceObject("errWrongCompany").ToString();
                bool dontAddUnspecifiedCat = IsCurrUserEditorOfChosenCompany(bUser);
                FillCategoryDdl(businessCompany, dontAddUnspecifiedCat, unspecified);

            }
            else if (currCompany == null || currCompany.visible == false)
            {
                passed = false;
                error = GetLocalResourceObject("errWrongCategory").ToString();
                FillCompaniesInDDl(businessCompany);
            }
            else
            {
                ///checks if selected company can have products in selected category
                Company other = businessCompany.GetOther(objectContext);
                if (currCompany != other)
                {
                    if (businessCompany.CheckIfCompanyCanHaveProductsInCategory(objectContext, currCompany, currCategory) == false)
                    {
                        passed = false;
                        error = GetLocalResourceObject("errCompCantHaveProdInCategory").ToString();

                        if (!string.IsNullOrEmpty(Request.Params["Category"]))
                        {
                            FillCompaniesInDDl(businessCompany);
                        }
                        else
                        {
                            bool dontAddUnspecifiedCat = IsCurrUserEditorOfChosenCompany(bUser);
                            FillCategoryDdl(businessCompany, dontAddUnspecifiedCat, unspecified);
                        }
                    }
                }
            }

            Category unspecifiedCategory = businessCategory.GetUnspecifiedCategory(objectContext);

            if (currCategory == unspecifiedCategory)
            {
                string strID = wantedCatMenu.SelectedValue;

                if (string.IsNullOrEmpty(strID))
                {
                    passed = false;
                    error = GetLocalResourceObject("errSelectWantedCat").ToString();
                }
                else
                {
                    long wantedID = 0;
                    if (!long.TryParse(strID, out wantedID))
                    {
                        throw new CommonCode.UIException(string.Format("couldnt parse wantedCatMenu.SelectedValue to long" +
                     " , user ID = {0}.", currentUser.ID));
                    }
                    else
                    {
                        wantedCategory = businessCategory.Get(objectContext, wantedID);
                        if (wantedCategory == null || wantedCategory.visible == false || wantedCategory.last == false)
                        {
                            passed = false;
                            GetLocalResourceObject("errInvWantedCat").ToString();
                            LoadWantedCategoryMenuWithItems();
                            tbWantedCategory.Text = string.Empty;
                        }
                        else
                        {
                            CategoryCompany wantedCategoryCompany = businessCompany.GetCategoryCompany(objectContext, currCompany, wantedCategory);
                            if (wantedCategoryCompany != null && wantedCategoryCompany.visible == true)
                            {
                                passed = false;
                                error = GetLocalResourceObject("errCompCanAddProdInWantCat").ToString();
                            }
                            else
                            {
                                // EVErything`s OK
                            }
                        }
                    }


                }
            }

            return passed;
        }

        [WebMethod]
        public static string CheckData(string text, string type, string notUsed)
        {
            string error = "";

            CommonCode.WebMethods.ValidateUserInput(text, type, "", out error);

            return error;

        }

        [WebMethod]
        public static string WMGetSimilarNames(string type, string name, string popUpID)
        {
            return CommonCode.WebMethods.GetSimilarNames(type, name, popUpID);
        }

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }


        protected void wantedCatMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            BusinessCategory bCategory = new BusinessCategory();
            long id = 0;
            if (long.TryParse(wantedCatMenu.SelectedValue, out id))
            {
                Category selectedCategory = bCategory.GetWithoutVisible(objectContext, id);
                if (selectedCategory != null)
                {
                    tbWantedCategory.Text = Tools.CategoryName(objectContext, selectedCategory, false);
                }
                else
                {
                    tbWantedCategory.Text = GetGlobalResourceObject("SiteResources", "ErrorOccured").ToString();
                }
            }
            else
            {
                tbWantedCategory.Text = GetGlobalResourceObject("SiteResources", "ErrorOccured").ToString();
            }
        }

        private void LoadWantedCategoryMenuWithItems()
        {
            string browserType = Request.Browser.Type.ToUpper();
            if (browserType.Contains("IE") && browserType != "IE5" && browserType != "IE9")  // Because chrome is detected as IE5
            {
                wantedCatMenu.DynamicHorizontalOffset = -1;
                wantedCatMenu.DynamicVerticalOffset = -2;
            }

            if (chosenCompany == null)
            {
                throw new CommonCode.UIException("chosenCompany is null");
            }

            BusinessCategory businessCategory = new BusinessCategory();
            BusinessCompany businessCompany = new BusinessCompany();

            wantedCatMenu.Items.Clear();

            IEnumerable<Category> categoryList = businessCompany.GetCompanyCategories(objectContext, chosenCompany.ID);
            List<Category> categories = businessCategory.GetAllByParentCategoryID(objectContext, null);
            IList<long> addedCategoryIDs = new List<long>();

            /////////
            if (categoryList.Count<Category>() > 0)
            {
                foreach (Category category in categoryList)
                {
                    addedCategoryIDs.Add(category.ID);
                }
            }
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

                    if (category.last == true && category.visible == true)
                    {
                        menuItem.Selectable = true;
                        menuItem.Text = CommonCode.UiTools.HackNavigationMenu(category.name, true, true);
                    }
                    else
                    {
                        menuItem.Selectable = false;
                        menuItem.Text = CommonCode.UiTools.HackNavigationMenu(category.name, false, true);
                    }

                    wantedCatMenu.Items.Add(menuItem);

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

                    if (category.last == true && category.visible == true)
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

        private void LoadChooseCategoryMenuWithItems()
        {
            string browserType = Request.Browser.Type.ToUpper();
            if (browserType.Contains("IE") && browserType != "IE5" && browserType != "IE9")  // Because chrome is detected as IE5
            {
                menuChooseCategory.DynamicHorizontalOffset = -1;
                menuChooseCategory.DynamicVerticalOffset = -2;
            }

            if (chosenCompany != null)
            {
                throw new CommonCode.UIException("chosenCompany is not null");
            }

            BusinessCategory businessCategory = new BusinessCategory();
            BusinessCompany businessCompany = new BusinessCompany();

            menuChooseCategory.Items.Clear();

            List<Category> categories = businessCategory.GetAllByParentCategoryID(objectContext, null);
            IList<long> addedCategoryIDs = new List<long>();

            Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);
            addedCategoryIDs.Add(unspecified.ID);

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

                    if (category.last == true && category.visible == true)
                    {
                        menuItem.NavigateUrl = GetUrlWithVariant(string.Format("AddProduct.aspx?Category={0}", category.ID));
                        menuItem.Text = CommonCode.UiTools.HackNavigationMenu(category.name, true, true);
                    }
                    else
                    {
                        menuItem.Selectable = false;
                        menuItem.Text = CommonCode.UiTools.HackNavigationMenu(category.name, false, true);
                    }

                    menuChooseCategory.Items.Add(menuItem);

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
                        AddChildMenuItemsToChooseCatMenu(menuItem, category.ID, addedCategoryIDs);
                    }
                }
            }
        }

        private void AddChildMenuItemsToChooseCatMenu(MenuItem menuItem, long parentCategoryID, IList<long> addedCategoryIDs)
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
                Boolean forbidden = false;

                if (addedCategoryIDs.Contains(category.ID))
                {
                    forbidden = true;
                }

                if (!forbidden)
                {
                    MenuItem childMenuItem = new MenuItem();
                    childMenuItem.Value = category.ID.ToString();

                    if (category.last == true && category.visible == true)
                    {
                        childMenuItem.NavigateUrl = GetUrlWithVariant(string.Format("AddProduct.aspx?Category={0}", category.ID));
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
                        AddChildMenuItemsToChooseCatMenu(childMenuItem, category.ID, addedCategoryIDs);
                    }
                }
            }
        }

        protected void btnProceedCompany_Click(object sender, EventArgs e)
        {
            lblErrorChooseCom.Visible = true;

            if (!string.IsNullOrEmpty(tbChooseCompany.Text))
            {
                BusinessCompany bCompany = new BusinessCompany();

                Company chosenCompany = bCompany.GetCompanyByName(objectContext, tbChooseCompany.Text);
                Company otherCompany = bCompany.GetOther(objectContext);

                if (chosenCompany == null)
                {
                    lblErrorChooseCom.Text = GetLocalResourceObject("errNoSuchCompany").ToString();
                }
                else if (chosenCompany == otherCompany)
                {
                    lblErrorChooseCom.Text = GetLocalResourceObject("errCantAddForThatCompany").ToString();
                }
                else
                {
                    RedirectToOtherUrl(string.Format("AddProduct.aspx?Company={0}", chosenCompany.ID));
                }
            }
            else
            {
                lblErrorChooseCom.Text = GetLocalResourceObject("errTypeCompanyName").ToString();
            }

        }


        [WebMethod]
        public static string[] GetCompaniesList(string prefixText, int count)
        {
            return CommonCode.WebMethods.GetCompaniesList(prefixText, count);
        }

    }
}
