﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class Search : BasePage
    {
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
            string buttonName = "btnSearch";
            bool btnSearchClicked = IsButtonClicked(buttonName);

            if ((IsPostBack == false) || (btnSearchClicked == false))
            {
                CheckParams();
                ShowInfo();
            }
        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            if (IsPostBack == false)
            {
                FillCheckBoxList();              // Fills check box list with options
            }

            ShowAdvertisement();
            SetLocalText();
        }

        private void SetLocalText()
        {
            lblAdvSearch.Text = GetLocalResourceObject("AdvancedSearch").ToString();
            lblSearch.Text = GetLocalResourceObject("search").ToString();
            lblSearchIn.Text = GetLocalResourceObject("in").ToString();
            btnAdvSearch.Text = GetLocalResourceObject("searchBtn").ToString();
            btnSearchMakers.Text = GetLocalResourceObject("searchBtn").ToString();
            lblSearchMaker.Text = GetLocalResourceObject("SerachForMakers").ToString();
        }

        private void ShowAdvertisement()
        {
            if (Configuration.AdvertsNumAdvertsOnSearchPage > 0)
            {
                phAdvert.Controls.Clear();
                adCell.Attributes.Clear();

                phAdvert.Controls.Add(CommonCode.ImagesAndAdverts.GetAdvertisements
                    (objectContext, Server, "general", 1, Configuration.AdvertsNumAdvertsOnSearchPage));

                if (CommonCode.ImagesAndAdverts.getAdvertisementsNumber(phAdvert) > 0)
                {
                    phAdvert.Visible = true;

                    adCell.Width = "252px";
                    adCell.VAlign = "top";
                }
                else
                {
                    phAdvert.Visible = false;

                    adCell.Width = "0px";
                }
            }
        }


        private void CheckPageParam()
        {
            String page = Request.Params["page"];
            if (!string.IsNullOrEmpty(page))
            {
                List<string> parameters = new List<string>();

                parameters.Add(Request.Params["products"]);
                parameters.Add(Request.Params["companies"]);
                parameters.Add(Request.Params["categories"]);
                parameters.Add(Request.Params["catID"]);
                parameters.Add(Request.Params["users"]);

                int paramCount = 0;
                foreach (string paramer in parameters)
                {
                    if (!string.IsNullOrEmpty(paramer))
                    {
                        paramCount++;
                    }
                }

                if (paramCount != 1)
                {
                    String search = Request.Params["Search"];
                    if (string.IsNullOrEmpty(search))
                    {
                        RedirectToOtherUrl("Search.aspx");
                    }
                    else
                    {
                        RedirectToOtherUrl(string.Format("Search.aspx?Search={0}", search));
                    }
                }
            }
        }

        private void CheckParams()
        {
            CheckPageParam();

            BusinessSearch businessSearch = new BusinessSearch();

            lblError.Visible = false;
            pnlProductResults.Visible = false;
            pnlCompaniesResults.Visible = false;
            pnlCategories.Visible = false;
            pnlUsers.Visible = false;

            String search = HttpUtility.HtmlDecode(Request.Params["Search"]);

            if (ValidateSearchStringPassed(search) && !string.IsNullOrEmpty(search))
            {
                search = Tools.TrimString(search, Configuration.SearchMaxSearchStringLength, false, false);

                AddSearchStringToAutoComplete(search);                              // Add`s the searched string to auto-complete table

                String prod = Request.Params["products"];
                String comp = Request.Params["companies"];
                String cat = Request.Params["categories"];
                String catID = Request.Params["catID"];
                String user = Request.Params["users"];

                if (prod == null && comp == null && cat == null && catID == null && user == null) //&& type == null)
                {
                    FillCompanies(businessSearch, search, 0);
                    pnlCompaniesResults.Visible = true;
                    FillProducts(businessSearch, search, -1);
                    pnlProductResults.Visible = true;
                }
                else if (string.IsNullOrEmpty(catID))
                {
                    if (cat != null)
                    {
                        if (cat == "yes")
                        {
                            // categories = OK
                            FillCategories(businessSearch, search);
                            pnlCategories.Visible = true;
                        }
                        else
                        {
                            lblError.Text = GetLocalResourceObject("errInvParCat").ToString();
                            lblError.Visible = true;
                        }

                    }

                    if (comp != null)
                    {
                        if (comp == "yes")
                        {
                            FillCompanies(businessSearch, search, 0);
                            pnlCompaniesResults.Visible = true;
                        }
                        else
                        {
                            lblError.Text = GetLocalResourceObject("errInvParCompanies").ToString();
                            lblError.Visible = true;
                        }

                    }

                    if (prod != null)
                    {
                        if (prod == "yes")
                        {
                            // products = OK
                            FillProducts(businessSearch, search, -1);
                            pnlProductResults.Visible = true;
                        }
                        else
                        {
                            lblError.Text = GetLocalResourceObject("errInvParProducts").ToString();
                            lblError.Visible = true;
                        }
                    }

                    if (user != null)
                    {
                        if (user == "yes")
                        {
                            FillUsers(businessSearch, search);
                            pnlUsers.Visible = true;
                        }
                        else
                        {
                            lblError.Text = GetLocalResourceObject("errInvParUsers").ToString();
                            lblError.Visible = true;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(prod) && string.IsNullOrEmpty(comp) &&
                        string.IsNullOrEmpty(cat) && string.IsNullOrEmpty(user))
                    {
                        long id = -1;
                        long.TryParse(catID, out id);
                        if (id < 1)
                        {
                            lblError.Text = GetLocalResourceObject("errInvParCat2").ToString();
                            lblError.Visible = true;
                        }
                        else
                        {
                            BusinessCategory businessCategory = new BusinessCategory();
                            Category category = businessCategory.Get(objectContext, id);
                            if (category != null && category.last == true)
                            {
                                FillProducts(businessSearch, search, id);
                                pnlProductResults.Visible = true;
                            }
                            else
                            {
                                lblError.Visible = true;
                                lblError.Text = GetLocalResourceObject("errInvCat").ToString();
                            }

                        }
                    }
                    else
                    {
                        lblError.Text = GetLocalResourceObject("errrInvPar").ToString();
                        lblError.Visible = true;
                    }
                }


            }

        }

        private void AddSearchStringToAutoComplete(string search)
        {
            if (string.IsNullOrEmpty(Request.Params["page"]))
            {
                if (!string.IsNullOrEmpty(search))
                {
                    AutoComplete autoComplete = new AutoComplete();
                    BusinessSearch businessSearch = new BusinessSearch();

                    long companies = businessSearch.CountSearchResults(userContext, objectContext, search, "companies");
                    long products = businessSearch.CountSearchResults(userContext, objectContext, search, "products");
                    long users = businessSearch.CountSearchResults(userContext, objectContext, search, "users");
                    long categories = businessSearch.CountSearchResults(userContext, objectContext, search, "categories");

                    if ((companies + products + users + categories) > 0)
                    {
                        autoComplete.StringSearched(objectContext, search, companies, products, categories, users);
                    }
                }
            }
        }

        private void GetPagesRowAndFromToNumbers(long itemsCount, string url, out TableRow pagesTopRow
            , out TableRow pagesBtmRow, out long from, out long to)
        {
            long page = 0;
            long itemsOnPage = 0;

            CommonCode.Pages.CheckPageParameters(Response, itemsCount, Configuration.SearchItemsPerSearchPage.ToString(),
                Request.Params["page"], url, out page, out itemsOnPage);

            pagesTopRow = CommonCode.Pages.GetPagesRow(itemsCount, itemsOnPage, page, url);
            pagesBtmRow = CommonCode.Pages.GetPagesRow(itemsCount, itemsOnPage, page, url);

            from = 0;
            to = 0;

            CommonCode.Pages.GetFromItemNumberToItemNumber(page, itemsOnPage, out from, out to);
        }

        private void GetPagesPlaceholdersAndFromToNumbers(long itemsCount, long alternaticesCount, string url, out PlaceHolder pagesTopPh
           , out PlaceHolder pagesBtmPh, out long from, out long to, out long fromAltName, out long toAltName)
        {
            long page = 0;
            long itemsOnPage = 0;

            long alternativesPage = 0;
            long altNameItemsOnPage = 0;

            bool mainChecks = CommonCode.Pages.CheckPageParameters(itemsCount, Configuration.SearchItemsPerSearchPage.ToString(),
                Request.Params["page"], out page, out itemsOnPage);

            bool altNamesChecks = CommonCode.Pages.CheckPageParameters(alternaticesCount, Configuration.SearchAlternativeItemsPerSearchPage.ToString(),
                Request.Params["page"], out alternativesPage, out altNameItemsOnPage);


            long linksItemsOnPage = itemsOnPage;
            long linksItemsCount = itemsCount;

            // Checks parameters for ProsuctsCount and AlternativesCouns, and if both are false, then redirects to erro page
            if (mainChecks == false && altNamesChecks == false)
            {
                CommonCode.UiTools.RedirectToOtherUrl(url);
            }
            else
            {
                if (page < alternativesPage)
                {
                    page = alternativesPage;
                }

                long numOfMainItemsPages = itemsCount / itemsOnPage;
                if ((itemsCount % itemsOnPage) > 0)
                {
                    numOfMainItemsPages++;
                }

                long numOfAltNamesPages = alternaticesCount / altNameItemsOnPage;
                if ((alternaticesCount % altNameItemsOnPage) > 0)
                {
                    numOfAltNamesPages++;
                }

                if (numOfMainItemsPages < numOfAltNamesPages)
                {
                    linksItemsOnPage = altNameItemsOnPage;
                    linksItemsCount = alternaticesCount;
                }

            }

            url = GetUrlWithVariant(url);
            pagesTopPh = CommonCode.Pages.GetPagesPlaceHolder(linksItemsCount, linksItemsOnPage, page, url);
            pagesBtmPh = CommonCode.Pages.GetPagesPlaceHolder(linksItemsCount, linksItemsOnPage, page, url);

            from = 0;
            to = 0;

            fromAltName = 0;
            toAltName = 0;

            CommonCode.Pages.GetFromItemNumberToItemNumber(page, itemsOnPage, out from, out to);
            CommonCode.Pages.GetFromItemNumberToItemNumber(page, altNameItemsOnPage, out fromAltName, out toAltName);
        }

        private void GetPagesPlaceholdersAndFromToNumbers(long itemsCount, string url, out PlaceHolder pagesTopPh
           , out PlaceHolder pagesBtmPh, out long from, out long to)
        {
            long page = 0;
            long itemsOnPage = 0;


            bool mainChecks = CommonCode.Pages.CheckPageParameters(itemsCount, Configuration.SearchItemsPerSearchPage.ToString(),
                Request.Params["page"], out page, out itemsOnPage);


            // Checks parameters for ProsuctsCount and if  false, redirects to error page
            if (mainChecks == false)
            {
                CommonCode.UiTools.RedirectToOtherUrl(url); // ne ba4ka
            }

            url = GetUrlWithVariant(url);
            pagesTopPh = CommonCode.Pages.GetPagesPlaceHolder(itemsCount, itemsOnPage, page, url);
            pagesBtmPh = CommonCode.Pages.GetPagesPlaceHolder(itemsCount, itemsOnPage, page, url);

            from = 0;
            to = 0;

            CommonCode.Pages.GetFromItemNumberToItemNumber(page, itemsOnPage, out from, out to);
        }

        public void FillProducts(BusinessSearch businessSearch, String search, long catID)
        {
            pnlProductResults.Controls.Clear();

            if (string.IsNullOrEmpty(search))
            {
                throw new CommonCode.UIException("search string is null or empty");
            }

            if (catID < 0)
            {
                catID = 0;
            }

            long count = 0;
            long alternativesCount = 0;

            String SearchIn = "";
            String url = "";
            if (catID < 1)
            {
                count = businessSearch.SearchCountInProducts(search, 0, 0, long.MaxValue, out alternativesCount);

                SearchIn = string.Format("{0}  << {1} >> {2} {3}", GetLocalResourceObject("ResultsFor")
                    , HttpUtility.HtmlEncode(search), GetLocalResourceObject("inProducts"), count);
                url = string.Format("Search.aspx?Search={0}&products=yes", HttpUtility.UrlEncode(search));
            }
            else
            {
                count = businessSearch.SearchCountInProducts(search, catID, 0, long.MaxValue, out alternativesCount);

                SearchIn = string.Format("{0} << {1} >> {2} {3}", GetLocalResourceObject("ResultsFor")
                    , HttpUtility.HtmlEncode(search), GetLocalResourceObject("inProductsInCat"), count);
                url = string.Format("Search.aspx?Search={0}&catID={1}", HttpUtility.UrlEncode(search), catID);
            }

            Panel firstPnl = new Panel();
            pnlProductResults.Controls.Add(firstPnl);
            firstPnl.CssClass = "textHeaderWA";
            firstPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(SearchIn, false));
            firstPnl.HorizontalAlign = HorizontalAlign.Center;

            if (count > 0 || alternativesCount > 0)
            {
                long from = 0;
                long to = 0;

                long fromAltName = 0;
                long toAltName = 0;


                PlaceHolder topPagesPh = new PlaceHolder();
                PlaceHolder btmPagesPh = new PlaceHolder();

                GetPagesPlaceholdersAndFromToNumbers(count, alternativesCount, url, out topPagesPh, out btmPagesPh, out from, out to, out fromAltName, out toAltName);
                pnlProductResults.Controls.Add(topPagesPh);

                BusinessCompany businessCompany = new BusinessCompany();
                BusinessProduct businessProduct = new BusinessProduct();
                BusinessRating businessRating = new BusinessRating();
                ImageTools imageTools = new ImageTools();


                string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);

                List<Product> products = businessSearch.SearchInProducts(objectContext, search, catID, from, to);
                List<AlternativeProductName> alternativeProducts = businessSearch.SearchInAlternativeProducts(objectContext, search, catID, fromAltName, toAltName);
                List<ProductVariant> variants = businessSearch.SearchInProductsVariants(objectContext, search, catID, fromAltName, toAltName);
                List<ProductSubVariant> subvariants = businessSearch.SearchInProductsSubVariants(objectContext, search, catID, fromAltName, toAltName);

                int i = 0;

                // Show main product results if page is first
                List<long> mainResultIds = new List<long>();
                bool thereAreMainResults = false;

                if (from < 1 && products.Count > 0)
                {
                    List<Product> mainResults = new List<Product>();

                    foreach (Product product in products)
                    {
                        if ((string.Compare(product.name, search, true) == 0))
                        {
                            mainResults.Add(product);
                            thereAreMainResults = true;
                            mainResultIds.Add(product.ID);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (thereAreMainResults == true && mainResults.Count > 0)
                    {
                        foreach (Product result in mainResults)
                        {
                            ShowProductResult(imageTools, businessCompany, businessRating, businessProduct, result, contPageId, i, true);
                            i++;
                        }

                    }
                }

                ShowNoExactMatchesPnl(from, thereAreMainResults, "product", true);

                // Show product variants and sub variants
                ShowProductsVariantsAndSubVariantsResults(variants, subvariants);

                // Show alternative names if there are such
                if (alternativeProducts != null && alternativeProducts.Count > 0)
                {
                    Panel altPnl = new Panel();
                    pnlProductResults.Controls.Add(altPnl);
                    altPnl.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, "300px");
                    altPnl.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginTop, "20px");
                    altPnl.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginBottom, "20px");

                    Panel hdrAltPnl = new Panel();
                    altPnl.Controls.Add(hdrAltPnl);
                    hdrAltPnl.CssClass = "textHeaderWA searchPageComments";
                    hdrAltPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(GetLocalResourceObject("altProdResults").ToString(), false));
                    hdrAltPnl.HorizontalAlign = HorizontalAlign.Center;

                    foreach (AlternativeProductName altName in alternativeProducts)
                    {
                        ShowAlternativeProductResult(altName, altPnl);
                    }
                }

                int mainResCount = mainResultIds.Count;

                // Show rest of the products
                foreach (Product product in products)
                {
                    if (mainResCount == 0 || !mainResultIds.Contains(product.ID))
                    {
                        ShowProductResult(imageTools, businessCompany, businessRating, businessProduct, product, contPageId, i, false);
                        i++;
                    }
                }

                pnlProductResults.Controls.Add(btmPagesPh);
            }
            else
            {
                ShowNoExactMatchesPnl(0, false, "product", false);
            }
        }

        private void ShowProductsVariantsAndSubVariantsResults(List<ProductVariant> variants, List<ProductSubVariant> subvariants)
        {
            if (variants.Count > 0 || subvariants.Count > 0)
            {
                Table tblVariants = new Table();
                pnlProductResults.Controls.Add(tblVariants);
                tblVariants.Width = Unit.Percentage(100);

                tblVariants.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginTop, "10px");
                tblVariants.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginBottom, "10px");

                TableRow newRow = new TableRow();
                tblVariants.Rows.Add(newRow);

                TableCell variantsCell = new TableCell();
                newRow.Cells.Add(variantsCell);

                variantsCell.Width = Unit.Percentage(50);
                variantsCell.VerticalAlign = VerticalAlign.Top;

                TableCell subVariantsCell = new TableCell();
                newRow.Cells.Add(subVariantsCell);

                subVariantsCell.Width = Unit.Percentage(50);
                subVariantsCell.VerticalAlign = VerticalAlign.Top;

                string orangeArrow = "<span class=\"darkOrange\">></span>";

                if (variants.Count > 0)
                {
                    Panel pnlVarHdr = new Panel();
                    variantsCell.Controls.Add(pnlVarHdr);

                    pnlVarHdr.HorizontalAlign = HorizontalAlign.Center;
                    pnlVarHdr.CssClass = "textHeaderWA searchPageComments";
                    pnlVarHdr.Controls.Add(CommonCode.UiTools.GetLabelWithText(GetLocalResourceObject("ProductVariants").ToString(), false));

                    foreach (ProductVariant variant in variants)
                    {
                        Panel varPnl = new Panel();
                        variantsCell.Controls.Add(varPnl);

                        varPnl.CssClass = "marginTB5";
                        varPnl.BackColor = CommonCode.UiTools.GetStandardBlueColor();
                        varPnl.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "10px");

                        Image newImg = new Image();
                        varPnl.Controls.Add(newImg);
                        newImg.ImageUrl = "~/images/SiteImages/triangle.png";
                        newImg.CssClass = "itemImage";

                        if (!variant.ProductReference.IsLoaded)
                        {
                            variant.ProductReference.Load();
                        }

                        HyperLink prodLink = CommonCode.UiTools.GetProductHyperLink(variant.Product);
                        varPnl.Controls.Add(prodLink);
                        prodLink.ID = string.Format("p{0}v{1}lnk", variant.Product.ID, variant.ID);
                        prodLink.Text = string.Format("{0} {1} {2}", variant.Product.name, orangeArrow, variant.name);
                        prodLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}','{2}')", variant.Product.ID, prodLink.ClientID, pnlPopUp.ClientID));
                        prodLink.Attributes.Add("onmouseout", "HideData()");

                    }
                }

                if (subvariants.Count > 0)
                {
                    Panel pnlSubVarHdr = new Panel();
                    subVariantsCell.Controls.Add(pnlSubVarHdr);

                    pnlSubVarHdr.HorizontalAlign = HorizontalAlign.Center;
                    pnlSubVarHdr.CssClass = "textHeaderWA searchPageComments";
                    pnlSubVarHdr.Controls.Add(CommonCode.UiTools.GetLabelWithText(GetLocalResourceObject("ProductSubVariants").ToString(), false));

                    foreach (ProductSubVariant subvariant in subvariants)
                    {
                        Panel subVarPnl = new Panel();
                        subVariantsCell.Controls.Add(subVarPnl);

                        subVarPnl.CssClass = "marginTB5";
                        subVarPnl.BackColor = CommonCode.UiTools.GetStandardBlueColor();
                        subVarPnl.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "10px");

                        Image newImg = new Image();
                        subVarPnl.Controls.Add(newImg);
                        newImg.ImageUrl = "~/images/SiteImages/triangle.png";
                        newImg.CssClass = "itemImage";

                        if (!subvariant.ProductReference.IsLoaded)
                        {
                            subvariant.ProductReference.Load();
                        }

                        if (!subvariant.VariantReference.IsLoaded)
                        {
                            subvariant.VariantReference.Load();
                        }

                        HyperLink prodLink = CommonCode.UiTools.GetProductHyperLink(subvariant.Product);
                        subVarPnl.Controls.Add(prodLink);
                        prodLink.ID = string.Format("p{0}v{1}sv{2}lnk", subvariant.Product.ID, subvariant.Variant.ID, subvariant.ID);
                        prodLink.Text = string.Format("{0} {1} {2} {1} {3}", subvariant.Product.name, orangeArrow, subvariant.Variant.name, subvariant.name);
                        prodLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}','{2}')", subvariant.Product.ID, prodLink.ClientID, pnlPopUp.ClientID));
                        prodLink.Attributes.Add("onmouseout", "HideData()");

                    }
                }
            }
        }

        /// <summary>
        /// type : product/company
        /// </summary>
        private void ShowNoExactMatchesPnl(long from, bool thereAreMainResults, string type, bool foundResults)
        {
            if (from < 1 && thereAreMainResults == false)
            {
                Panel noMainRes = new Panel();
                noMainRes.CssClass = "searchNoMainResFoundPnl";
                UserRoles checkRole;

                string clickToAdd = string.Empty;
                string toReadHowToAdd = string.Empty;
                string regToAdd = string.Empty;
                string readUrl = string.Empty;
                string addUrl = string.Empty;
                string clickHereToRead = string.Empty;

                switch (type)
                {
                    case "company":
                        pnlCompaniesResults.Controls.Add(noMainRes);

                        checkRole = UserRoles.AddCompanies;
                        clickToAdd = GetLocalResourceObject("toAddCompany").ToString();
                        toReadHowToAdd = GetLocalResourceObject("toReadHowToAddCompany").ToString();
                        regToAdd = GetLocalResourceObject("RegToAddCompany").ToString();
                        clickHereToRead = GetLocalResourceObject("ClickHereToReadCompany").ToString();

                        readUrl = CommonCode.UiTools.GetUrlWithVariant("Guide.aspx#infaaemaker");
                        addUrl = CommonCode.UiTools.GetUrlWithVariant("AddCompany.aspx");

                        break;
                    case "product":
                        pnlProductResults.Controls.Add(noMainRes);

                        checkRole = UserRoles.AddProducts;
                        clickToAdd = GetLocalResourceObject("toAddProduct").ToString();
                        toReadHowToAdd = GetLocalResourceObject("toReadHowToAddProduct").ToString();
                        regToAdd = GetLocalResourceObject("RegToAddProduct").ToString();
                        clickHereToRead = GetLocalResourceObject("ClickHereToReadProduct").ToString();

                        readUrl = CommonCode.UiTools.GetUrlWithVariant("Guide.aspx#infaeproducts");
                        addUrl = CommonCode.UiTools.GetUrlWithVariant("AddProduct.aspx");

                        break;
                    default:
                        throw new CommonCode.UIException(string.Format("type = {0}, is not supported for ShowNoExactMatchesPnl", type));
                }


                // Show links to add product/company if there are no main results

                if (foundResults == true)
                {
                    Label lblNoMainResFound = new Label();
                    noMainRes.Controls.Add(lblNoMainResFound);
                    lblNoMainResFound.Text = string.Format("{0} <br />", GetLocalResourceObject("noMainResultsFound"));
                    lblNoMainResFound.CssClass = "searchPageRatings";
                }
                else
                {
                    Label lblNoResFound = new Label();
                    noMainRes.Controls.Add(lblNoResFound);
                    lblNoResFound.Text = string.Format("{0} <br />", GetLocalResourceObject("noResults"));
                    lblNoResFound.CssClass = "searchPageRatings";

                }



                HyperLink hlClickHereToRead = new HyperLink();
                noMainRes.Controls.Add(hlClickHereToRead);
                hlClickHereToRead.Text = clickHereToRead;
                hlClickHereToRead.Target = "_blank";
                hlClickHereToRead.NavigateUrl = readUrl;

                Label lblToRead = new Label();
                noMainRes.Controls.Add(lblToRead);
                lblToRead.Text = string.Format(" {0} <br />", toReadHowToAdd);


                User currUser = CommonCode.UiTools.GetCurrentUserNoExc(userContext);
                if (currUser != null)
                {
                    BusinessUser bUser = new BusinessUser();
                    if (bUser.CanUserDo(userContext, currUser, checkRole) == true)
                    {
                        HyperLink hlClickHereToAdd = new HyperLink();
                        noMainRes.Controls.Add(hlClickHereToAdd);
                        hlClickHereToAdd.Text = GetLocalResourceObject("ClickHere").ToString();
                        hlClickHereToAdd.NavigateUrl = addUrl;
                        hlClickHereToAdd.Target = "_blank";

                        Label lblToAddProduct = new Label();
                        noMainRes.Controls.Add(lblToAddProduct);
                        lblToAddProduct.Text = string.Format(" {0} <br />", clickToAdd);
                    }
                }
                else
                {
                    HyperLink hlRegister = new HyperLink();
                    noMainRes.Controls.Add(hlRegister);
                    hlRegister.Text = GetLocalResourceObject("hlRegister").ToString();
                    hlRegister.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Registration.aspx");
                    hlRegister.Target = "_blank";

                    Label lblRegToAddProduct = new Label();
                    noMainRes.Controls.Add(lblRegToAddProduct);
                    lblRegToAddProduct.Text = string.Format(" {0}", regToAdd);
                }
            }
        }

        private void ShowAlternativeProductResult(AlternativeProductName altName, Panel inPnl)
        {
            Panel newPnl = new Panel();
            inPnl.Controls.Add(newPnl);
            newPnl.CssClass = "marginTB5";
            newPnl.BackColor = CommonCode.UiTools.GetStandardBlueColor();
            newPnl.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "10px");

            Image newImg = new Image();
            newPnl.Controls.Add(newImg);
            newImg.ImageUrl = "~/images/SiteImages/triangle.png";
            newImg.CssClass = "itemImage";

            if (!altName.ProductReference.IsLoaded)
            {
                altName.ProductReference.Load();
            }

            HyperLink prodLink = CommonCode.UiTools.GetProductHyperLink(altName.Product);
            newPnl.Controls.Add(prodLink);
            prodLink.ID = string.Format("p{0}alt{1}lnk", altName.Product.ID, altName.ID);
            prodLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}','{2}')", altName.Product.ID, prodLink.ClientID, pnlPopUp.ClientID));
            prodLink.Attributes.Add("onmouseout", "HideData()");

            newPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText("&nbsp;&nbsp; : &nbsp;&nbsp;", false));

            Label altSname = CommonCode.UiTools.GetLabelWithText(altName.name, false);
            newPnl.Controls.Add(altSname);
            altSname.CssClass = "darkOrange";
        }

        private void ShowAlternativeCompanyResult(AlternativeCompanyName altName, Panel inPnl)
        {
            Panel newPnl = new Panel();
            inPnl.Controls.Add(newPnl);
            newPnl.CssClass = "marginTB5";
            newPnl.BackColor = CommonCode.UiTools.GetStandardBlueColor();
            newPnl.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "10px");

            Image newImg = new Image();
            newPnl.Controls.Add(newImg);
            newImg.ImageUrl = "~/images/SiteImages/triangle.png";
            newImg.CssClass = "itemImage";

            if (!altName.CompanyReference.IsLoaded)
            {
                altName.CompanyReference.Load();
            }


            HyperLink compLink = CommonCode.UiTools.GetCompanyHyperLink(altName.Company);
            newPnl.Controls.Add(compLink);
            compLink.ID = string.Format("p{0}alt{1}lnk", altName.Company.ID, altName.ID);
            compLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}','{2}')", altName.Company.ID, compLink.ClientID, pnlPopUp.ClientID));
            compLink.Attributes.Add("onmouseout", "HideData()");

            newPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText("&nbsp;&nbsp; : &nbsp;&nbsp;", false));

            Label altSname = CommonCode.UiTools.GetLabelWithText(altName.name, false);
            newPnl.Controls.Add(altSname);
            altSname.CssClass = "darkOrange";
        }

        private void ShowProductResult(ImageTools imageTools, BusinessCompany businessCompany, BusinessRating businessRating
           , BusinessProduct businessProduct, Product product, string contPageId, int i, bool match)
        {
            if (!product.CompanyReference.IsLoaded)
            {
                product.CompanyReference.Load();
            }

            Panel newPanel = new Panel();
            pnlProductResults.Controls.Add(newPanel);

            string appendToHlId = string.Empty;

            if (match == true)
            {
                newPanel.CssClass = "searchDefResultMatch";
                appendToHlId = "m";

                newPanel.BackColor = CommonCode.UiTools.GetStandardBlueColor();
            }
            else
            {
                newPanel.CssClass = "searchDefResult";

                if (i % 2 == 0)
                {
                    newPanel.BackColor = CommonCode.UiTools.GetLightBlueColor();
                }
            }

            Table tblForImg = new Table();
            newPanel.Controls.Add(tblForImg);
            tblForImg.Width = Unit.Percentage(100);
            TableRow row = new TableRow();
            tblForImg.Rows.Add(row);
            TableCell cell = new TableCell();
            row.Cells.Add(cell);

            ProductImage img = imageTools.GetProductImageThumbnail(objectContext, product);
            if (img != null)
            {

                int width = 0;
                int height = 0;
                CommonCode.ImagesAndAdverts.ResizeImage(img.height, img.width, 110, 200, out height, out width);

                Image image = new Image();
                image.CssClass = "searchPageImages";
                image.ImageAlign = ImageAlign.Left;
                image.ImageUrl = img.url;
                image.Width = width;
                image.Height = height;

                image.BorderColor = System.Drawing.Color.Black;
                image.BorderStyle = BorderStyle.Solid;
                image.BorderWidth = Unit.Pixel(1);

                if (img.description.Length > 0)
                {
                    image.ToolTip = img.description;
                }

                ProductImage parentImage = imageTools.GetProductImage(objectContext, img.mainImgID.Value);
                if (parentImage == null)
                {
                    throw new CommonCode.UIException(string.Format
                        ("Thumbnail ID = {0} `s main image is null , shouldnt happen as there is check when getting all thumbnails"
                        , image.ID));
                }

                HyperLink imgLink = new HyperLink();
                imgLink.Controls.Add(image);
                cell.Controls.Add(imgLink);

                imgLink.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", product.ID));
                imgLink.Target = "_blank";
                imgLink.Visible = true;
            }


            HyperLink productLink = CommonCode.UiTools.GetProductHyperLink(product);
            cell.Controls.Add(productLink);
            productLink.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "20px");


            cell.Controls.Add(CommonCode.UiTools.GetComaLabel());
            if (businessCompany.IsOther(objectContext, product.Company))
            {
                Label otherLbl = new Label();
                otherLbl.Text = GetGlobalResourceObject("SiteResources", "other").ToString();
                cell.Controls.Add(otherLbl);
            }
            else
            {

                HyperLink compLink = CommonCode.UiTools.GetCompanyHyperLink(product.Company);
                compLink.ID = string.Format("p{0}comp{1}{2}lnk", product.ID, product.Company.ID, appendToHlId);
                compLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}{2}','{3}')", product.Company.ID, contPageId, compLink.ClientID, pnlPopUp.ClientID));
                compLink.Attributes.Add("onmouseout", "HideData()");
                cell.Controls.Add(compLink);
            }

            cell.Controls.Add(CommonCode.UiTools.GetNewLineControl());

            Label ratingLbl = new Label();
            ratingLbl.Width = Unit.Pixel(150);
            ratingLbl.Text = string.Format("{0} {1} ", GetLocalResourceObject("rating"), businessRating.GetProductRating(product));
            ratingLbl.CssClass = "searchPageRatings";
            cell.Controls.Add(ratingLbl);


            string comms = businessProduct.CountProductComments(objectContext, product).ToString();
            Label commLbl = new Label();
            commLbl.Text = string.Format(" {0} {1} ", GetLocalResourceObject("comments"), comms);
            commLbl.CssClass = "searchPageComments";
            cell.Controls.Add(commLbl);


            cell.Controls.Add(CommonCode.UiTools.GetNewLineControl());

            if (!string.IsNullOrEmpty(product.description))
            {
                Label descrLbl = new Label();
                cell.Controls.Add(descrLbl);
                descrLbl.Text = Tools.TrimString(product.description, 220, false, true);
            }
        }

        public void FillCategories(BusinessSearch businessSearch, String search)
        {
            pnlCategories.Controls.Clear();

            if (string.IsNullOrEmpty(search))
            {
                throw new CommonCode.UIException("Search string is null or empty");
            }

            long count = businessSearch.SearchCountInCategories(search, 0, long.MaxValue);
            string url = string.Format("Search.aspx?Search={0}&categories=yes", search);

            Panel pnlHeader = new Panel();
            pnlCategories.Controls.Add(pnlHeader);
            pnlHeader.CssClass = "textHeaderWA";
            pnlHeader.HorizontalAlign = HorizontalAlign.Center;

            Label lblResFor = new Label();
            pnlHeader.Controls.Add(lblResFor);
            lblResFor.Text = string.Format("{0} << {1} >> {2} {3}", GetLocalResourceObject("ResultsFor")
                , HttpUtility.HtmlEncode(search), GetLocalResourceObject("inCategories"), count);


            if (count > 0)
            {
                long from = 0;
                long to = 0;

                PlaceHolder topPagesPh = new PlaceHolder();
                PlaceHolder btmPagesPh = new PlaceHolder();

                GetPagesPlaceholdersAndFromToNumbers(count, url, out topPagesPh, out btmPagesPh, out from, out to);
                pnlCategories.Controls.Add(topPagesPh);

                List<Category> categories = businessSearch.SearchInCategories(objectContext, search, from, to);

                long i = 0;

                foreach (Category category in categories)
                {
                    ShowCategoryResult(i, category);
                    i++;
                }

                pnlCategories.Controls.Add(btmPagesPh);

            }
            else
            {

                Panel noRes = new Panel();
                pnlCategories.Controls.Add(noRes);
                noRes.CssClass = "searchNoMainResFoundPnl";

                Label lblNoResFound = new Label();
                noRes.Controls.Add(lblNoResFound);
                lblNoResFound.Text = GetLocalResourceObject("noResults").ToString();
                lblNoResFound.CssClass = "searchPageRatings";

            }


        }

        private void ShowCategoryResult(long i, Category category)
        {
            Panel newPnl = new Panel();
            pnlCategories.Controls.Add(newPnl);

            newPnl.CssClass = "marginTB5";
            newPnl.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "5px");

            if (category.last)
            {
                HyperLink catLink = CommonCode.UiTools.GetCategoryHyperLink(category);
                newPnl.Controls.Add(catLink);
            }
            else
            {
                Label catLbl = new Label();
                catLbl.Text = category.name;
                newPnl.Controls.Add(catLbl);
            }

            Label restLbl = new Label();
            restLbl.Text = string.Format(" , {0}", Tools.CategoryName(objectContext, category, true));
            newPnl.Controls.Add(restLbl);

            if (i % 2 == 0)
            {
                newPnl.BackColor = CommonCode.UiTools.GetLightBlueColor();
            }
        }

        public void FillCompanies(BusinessSearch businessSearch, String search, long typeID)
        {
            pnlCompaniesResults.Controls.Clear();

            if (string.IsNullOrEmpty(search))
            {
                throw new CommonCode.UIException("Search string is null or empty");
            }

            long companiesCount = 0;
            long alternativesCount = 0;

            businessSearch.SearchCountInCompanies(search, 0, long.MaxValue, typeID, out companiesCount, out alternativesCount);

            string url = "";

            Panel hdrPnl = new Panel();
            pnlCompaniesResults.Controls.Add(hdrPnl);
            hdrPnl.CssClass = "textHeaderWA";
            hdrPnl.HorizontalAlign = HorizontalAlign.Center;

            Label searchRes = new Label();
            hdrPnl.Controls.Add(searchRes);

            if (typeID < 1)
            {
                searchRes.Text = string.Format("{0} << {1} >> {2} {3}", GetLocalResourceObject("ResultsFor")
                    , HttpUtility.HtmlEncode(search), GetLocalResourceObject("inMakers"), companiesCount);
                url = string.Format("Search.aspx?Search={0}&companies=yes", search);
            }
            else
            {
                throw new CommonCode.UIException("Search for company with type is not supported");
            }


            if (companiesCount > 0 || alternativesCount > 0)
            {
                long from = 0;
                long to = 0;

                long fromAltName = 0;
                long toAltName = 0;

                PlaceHolder topPagesPh = new PlaceHolder();
                PlaceHolder btmPagesPh = new PlaceHolder();

                GetPagesPlaceholdersAndFromToNumbers(companiesCount, alternativesCount, url, out topPagesPh, out btmPagesPh, out from, out to, out fromAltName, out toAltName);
                pnlCompaniesResults.Controls.Add(topPagesPh);

                BusinessCompany businessCompany = new BusinessCompany();
                ImageTools imageTools = new ImageTools();

                List<Company> companies = businessSearch.SearchInCompanies(objectContext, search, from, to, typeID);
                List<AlternativeCompanyName> altCompanyNames = businessSearch.SearchInAlternativeCompanies(objectContext, search, typeID, fromAltName, toAltName);

                Company other = businessCompany.GetOther(objectContext);

                long i = 0;

                // Show main product results if page is first
                List<long> mainResultIds = new List<long>();
                bool thereAreMainResults = false;
                if (from < 1 && companies.Count > 0)
                {
                    List<Company> mainResults = new List<Company>();

                    foreach (Company company in companies)
                    {
                        if ((string.Compare(company.name, search, true) == 0))
                        {
                            mainResults.Add(company);
                            thereAreMainResults = true;
                            mainResultIds.Add(company.ID);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (thereAreMainResults == true && mainResults.Count > 0)
                    {
                        foreach (Company result in mainResults)
                        {
                            ShowCompanyResult(imageTools, other, i, result, true);
                            i++;
                        }

                    }
                }

                ShowNoExactMatchesPnl(from, thereAreMainResults, "company", true);

                // Show alternative names if there are such
                if (altCompanyNames != null && altCompanyNames.Count > 0)
                {
                    Panel altCmp = new Panel();
                    pnlCompaniesResults.Controls.Add(altCmp);
                    altCmp.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, "300px");
                    altCmp.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginTop, "20px");
                    altCmp.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginBottom, "20px");

                    Panel hdrAltPnl = new Panel();
                    altCmp.Controls.Add(hdrAltPnl);
                    hdrAltPnl.CssClass = "textHeaderWA searchPageComments";
                    hdrAltPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(GetLocalResourceObject("altCompResults").ToString(), false));
                    hdrAltPnl.HorizontalAlign = HorizontalAlign.Center;

                    foreach (AlternativeCompanyName altName in altCompanyNames)
                    {
                        ShowAlternativeCompanyResult(altName, altCmp);
                    }
                }

                int mainResCount = mainResultIds.Count;

                foreach (Company company in companies)
                {

                    if (mainResCount == 0 || !mainResultIds.Contains(company.ID))
                    {
                        ShowCompanyResult(imageTools, other, i, company, false);
                        i++;
                    }
                }

                pnlCompaniesResults.Controls.Add(btmPagesPh);
            }
            else
            {
                ShowNoExactMatchesPnl(0, false, "company", false);
            }
        }

        private void ShowCompanyResult(ImageTools imageTools, Company other, long i, Company company, bool match)
        {
            Panel compPnl = new Panel();
            pnlCompaniesResults.Controls.Add(compPnl);

            if (match == true)
            {
                compPnl.CssClass = "searchDefResultMatch";

                compPnl.BackColor = CommonCode.UiTools.GetStandardBlueColor();
            }
            else
            {
                compPnl.CssClass = "searchDefResult";

                if (i % 2 == 0)
                {
                    compPnl.BackColor = CommonCode.UiTools.GetLightBlueColor();
                }
            }


            Table tblForImg = new Table();
            compPnl.Controls.Add(tblForImg);
            tblForImg.Width = Unit.Percentage(100);
            TableRow row = new TableRow();
            tblForImg.Rows.Add(row);
            TableCell cell = new TableCell();
            row.Cells.Add(cell);

            if (other.ID == company.ID)
            {
                compPnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(company.name, false));
            }
            else
            {
                CompanyImage img = imageTools.GetCompanyImageThumbnail(objectContext, company);
                if (img != null)
                {

                    int width = 0;
                    int height = 0;
                    CommonCode.ImagesAndAdverts.ResizeImage(img.height, img.width, 110, 200, out height, out width);

                    Image image = new Image();
                    image.CssClass = "searchPageImages";
                    image.ImageAlign = ImageAlign.Left;
                    image.ImageUrl = img.url;
                    image.Width = width;
                    image.Height = height;

                    image.BorderColor = System.Drawing.Color.Black;
                    image.BorderStyle = BorderStyle.Solid;
                    image.BorderWidth = Unit.Pixel(1);

                    if (img.description.Length > 0)
                    {
                        image.ToolTip = img.description;
                    }

                    CompanyImage parentImage = imageTools.GetCompanyImage(objectContext, img.mainImgID.Value);
                    if (parentImage == null)
                    {
                        throw new CommonCode.UIException(string.Format
                            ("Thumbnail ID = {0} `s main image is null , shouldnt happen as there is check when getting all thumbnails"
                            , image.ID));
                    }

                    HyperLink imgLink = new HyperLink();
                    imgLink.Controls.Add(image);
                    cell.Controls.Add(imgLink);

                    imgLink.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", company.ID));
                    imgLink.Target = "_blank";
                    imgLink.Visible = true;
                }



                HyperLink compLink = CommonCode.UiTools.GetCompanyHyperLink(company);
                cell.Controls.Add(compLink);
                compLink.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "20px");

                cell.Controls.Add(CommonCode.UiTools.GetNewLineControl());

                if (!string.IsNullOrEmpty(company.description))
                {
                    Label descrLbl = new Label();
                    cell.Controls.Add(descrLbl);
                    descrLbl.Text = Tools.TrimString(company.description, 250, false, true);
                }

            }
        }

        public TableRow getNoDataRow(int colspan)
        {
            if (colspan < 1)
            {
                throw new CommonCode.UIException("invalid colspan");
            }

            TableRow newRow = new TableRow();
            TableCell newCell = new TableCell();
            newCell.ColumnSpan = colspan;
            newCell.Text = GetLocalResourceObject("noResults").ToString();
            newRow.Cells.Add(newCell);

            return newRow;
        }


        protected void btnAdvSearch_Click(object sender, EventArgs e)
        {
            string urlEncodedCriteria = HttpUtility.UrlEncode(tbSearch.Text);

            if (ValidateSearchStringPassed(urlEncodedCriteria))
            {
                System.Text.StringBuilder url = new System.Text.StringBuilder();
                url.Append("Search.aspx?");

                if (cblSearchOptions.Items[1].Selected && cblSearchOptions.Items[2].Selected &&
                    cblSearchOptions.Items[0].Selected == false && cblSearchOptions.Items[3].Selected == false)
                {
                    url.Append("Search=");
                    url.Append(urlEncodedCriteria);
                }
                else
                {
                    if (cblSearchOptions.Items[0].Selected)
                    {
                        url.Append("categories=yes&");
                    }
                    if (cblSearchOptions.Items[1].Selected)
                    {
                        url.Append("companies=yes&");
                    }
                    if (cblSearchOptions.Items[2].Selected)
                    {
                        url.Append("products=yes&");
                    }
                    if (cblSearchOptions.Items[3].Selected)
                    {
                        url.Append("users=yes&");
                    }

                    url.Append("Search=");
                    url.Append(urlEncodedCriteria);
                }

                RedirectToOtherUrl(url.ToString());
            }
        }

        public void FillCheckBoxList()
        {
            ListItem catItem = new ListItem();
            catItem.Text = string.Format("{0} ", GetLocalResourceObject("categories"));
            catItem.Value = "1";
            cblSearchOptions.Items.Add(catItem);

            ListItem compItem = new ListItem();
            compItem.Text = string.Format("{0} ", GetLocalResourceObject("makers"));
            compItem.Value = "2";
            cblSearchOptions.Items.Add(compItem);

            ListItem prodItem = new ListItem();
            prodItem.Text = string.Format("{0} ", GetLocalResourceObject("products"));
            prodItem.Value = "3";
            cblSearchOptions.Items.Add(prodItem);

            ListItem userItem = new ListItem();
            userItem.Text = string.Format("{0} ", GetLocalResourceObject("users"));
            userItem.Value = "4";
            cblSearchOptions.Items.Add(userItem);
        }


        public Boolean ValidateSearchStringPassed(string text)
        {
            Boolean result = false;
            lblError.Visible = true;

            if (Tools.NullValidatorPassed(text))
            {
                if (Tools.StringRangeValidatorPassed(1, Configuration.SearchMaxSearchStringLength, text))
                {
                    lblError.Visible = false;
                    result = true;
                }
                else
                {
                    lblError.Text = string.Format("{0} {1} {2}", GetLocalResourceObject("errSearchStrLength")
                        , Configuration.SearchMaxSearchStringLength, GetLocalResourceObject("errSearchStrLength2"));
                }
            }
            else
            {
                lblError.Text = GetLocalResourceObject("errTypeSearchString").ToString();
            }


            return result;
        }


        public void FillUsers(BusinessSearch businessSearch, String search)
        {
            pnlUsers.Controls.Clear();

            if (string.IsNullOrEmpty(search))
            {
                throw new CommonCode.UIException("Search string is null or empty");
            }

            long count = businessSearch.SearchCountInUsers(search, 0, long.MaxValue);
            string url = string.Format("Search.aspx?Search={0}&users=yes", search);

            Panel pnlHeader = new Panel();
            pnlUsers.Controls.Add(pnlHeader);
            pnlHeader.CssClass = "textHeaderWA";
            pnlHeader.HorizontalAlign = HorizontalAlign.Center;

            Label lblResFor = new Label();
            pnlHeader.Controls.Add(lblResFor);
            lblResFor.Text = string.Format("{0} << {1} >> {2} {3}", GetLocalResourceObject("ResultsFor")
                , HttpUtility.HtmlEncode(search), GetLocalResourceObject("inUsers"), count);

            if (count > 0)
            {
                long from = 0;
                long to = 0;
                long i = 0;

                PlaceHolder topPagesPh = new PlaceHolder();
                PlaceHolder btmPagesPh = new PlaceHolder();

                GetPagesPlaceholdersAndFromToNumbers(count, url, out topPagesPh, out btmPagesPh, out from, out to);
                pnlUsers.Controls.Add(topPagesPh);

                BusinessUser businessUser = new BusinessUser();
                BusinessComment businessComment = new BusinessComment();

                List<User> users = businessSearch.SearchInUsers(userContext, search, from, to);

                // Show main product results if page is first
                List<long> mainResultIds = new List<long>();
                bool thereAreMainResults = false;
                if (from < 1 && users.Count > 0)
                {
                    List<User> mainResults = new List<User>();

                    foreach (User user in users)
                    {
                        if ((string.Compare(user.username, search, true) == 0))
                        {
                            mainResults.Add(user);
                            thereAreMainResults = true;
                            mainResultIds.Add(user.ID);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (thereAreMainResults == true && mainResults.Count > 0)
                    {
                        foreach (User user in users)
                        {
                            ShowUserResult(businessComment, i, user, true);
                            i++;
                        }

                    }
                }

                int mainResultsCount = mainResultIds.Count;

                foreach (User user in users)
                {
                    if (mainResultsCount == 0 || !mainResultIds.Contains(user.ID))
                    {
                        ShowUserResult(businessComment, i, user, false);
                        i++;
                    }
                }

                pnlUsers.Controls.Add(btmPagesPh);
            }
            else
            {
                Panel noRes = new Panel();
                pnlUsers.Controls.Add(noRes);
                noRes.CssClass = "searchNoMainResFoundPnl";

                Label lblNoResFound = new Label();
                noRes.Controls.Add(lblNoResFound);
                lblNoResFound.Text = GetLocalResourceObject("noResults").ToString();
                lblNoResFound.CssClass = "searchPageRatings";
            }
        }

        private void ShowUserResult(BusinessComment businessComment, long i, User user, bool mainResult)
        {
            Panel newPnl = new Panel();
            pnlUsers.Controls.Add(newPnl);

            if (mainResult == true)
            {
                newPnl.CssClass = "marginTB5 searchDefResultMatch";

                newPnl.BackColor = CommonCode.UiTools.GetStandardBlueColor();
            }
            else
            {
                newPnl.CssClass = "marginTB5";

                if (i % 2 == 0)
                {
                    newPnl.BackColor = CommonCode.UiTools.GetLightBlueColor();
                }
            }

            newPnl.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "5px");

            Panel usrPnl = new Panel();
            newPnl.Controls.Add(usrPnl);
            usrPnl.CssClass = "panelInline";
            usrPnl.Width = Unit.Pixel(250);

            HyperLink usrLink = CommonCode.UiTools.GetUserHyperLink(user);
            usrPnl.Controls.Add(usrLink);

            Panel regPnl = new Panel();
            newPnl.Controls.Add(regPnl);
            regPnl.CssClass = "panelInline";
            regPnl.Width = Unit.Pixel(250);

            Label regLbl = new Label();
            regLbl.Text = string.Format("{0} {1}", GetLocalResourceObject("RegisteredOn")
                , CommonCode.UiTools.DateTimeToLocalShortDateString(user.dateCreated));
            regPnl.Controls.Add(regLbl);


            string comms = businessComment.CountUserComments(objectContext, user).ToString();
            Label commLbl = new Label();
            commLbl.Text = string.Format("{0} {1}", GetLocalResourceObject("comments"), comms);
            commLbl.CssClass = "searchPageComments";
            newPnl.Controls.Add(commLbl);
        }


        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }

        protected void btnSearchMakers_Click(object sender, EventArgs e)
        {
            RedirectToOtherUrl(string.Format("Search.aspx?search={0}&companies=yes&type={1}"
                , HttpUtility.UrlEncode(tbSearchMaker.Text), ddlMakers.SelectedValue));
        }

        private void FillDdlMakerTypes()
        {
            ddlMakers.Items.Clear();

            BusinessCompanyType businessCompanyType = new BusinessCompanyType();
            List<CompanyType> types = businessCompanyType.GetAllCompanyTypes(objectContext, true, true);
            if (types.Count < 1)
            {
                throw new CommonCode.UIException("There are no company types.");
            }

            foreach (CompanyType type in types)
            {
                ListItem newItem = new ListItem();
                newItem.Text = type.name;
                newItem.Value = type.ID.ToString();
                ddlMakers.Items.Add(newItem);
            }
        }

    }
}
