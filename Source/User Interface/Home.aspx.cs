﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Linq;

using DataAccess;
using BusinessLayer;

using log4net;

namespace UserInterface
{
    public partial class Home : BasePage
    {
        protected long NewsNumber = 0;                                          // All news number
        protected long CurrNewsPage = 1;                                        // Current page
        protected long NewsOnPage = Configuration.HomePageNumberNewsOnPage;     // news per page

        private User currentUser = null;

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private static ILog log = LogManager.GetLogger(typeof(Home));

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (pnlChooseLang.Visible == true)
            {
                lblClickForEng.Attributes.Add("onclick", string.Format("HideElementWithID('{0}', 'false');", pnlChooseLang.ClientID));
                lblClickForEng1.Attributes.Add("onclick", string.Format("HideElementWithID('{0}', 'false');", pnlChooseLang.ClientID));
                imgEnFlag.Attributes.Add("onclick", string.Format("HideElementWithID('{0}', 'false');", pnlChooseLang.ClientID));

                lblClickForBulg.Attributes.Add("onclick", @"var newLocation = window.location.href.replace(""/en/"", ""/bg/"");
                window.location.href = newLocation;");
                lblClickForBulg1.Attributes.Add("onclick", @"var newLocation = window.location.href.replace(""/en/"", ""/bg/"");
                window.location.href = newLocation;");

                imgBgFlag.Attributes.Add("onclick", @"var newLocation = window.location.href.replace(""/en/"", ""/bg/"");
                window.location.href = newLocation;");
            }


            pnlShowForNewComersHeader.Attributes.Add("onclick", string.Format("ChangeLabelText('{0}', '{1}', '{2}');"
                , lblForNewcomers.ClientID, GetLocalResourceObject("lblForNewcomers").ToString()
                , GetLocalResourceObject("lblForNewcomers2").ToString()));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckParameters();  // checks page parameters
            CheckUser();        // shows options depended on user
            ShowInfo();         // shows page info
        }

        /// <summary>
        /// I the client was redirected to the home page because
        /// there was no application variant specified,
        /// ask the user to choose an application variant.
        /// </summary>
        private void PromptAppVariantChoice()
        {


            if (Session[RedirectedToHomeNoAppVariantSessKey] != null)
            {
                Session.Remove(RedirectedToHomeNoAppVariantSessKey);

                pnlChooseLang.Visible = true;

                lblLangInfoEng.Text = "Please choose language for the site.";
                lblLangInfoBgr.Text = "Моля, изберете език на сайта.";

                lblClickForEng.Text = "English";
                lblClickForEng1.Text = "Английски";

                lblClickForBulg.Text = "Bulgarian";
                lblClickForBulg1.Text = "Български";
            }
            else
            {
                pnlChooseLang.Visible = false;
            }
        }

        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);
            if (currUser != null)
            {
                currentUser = currUser;
            }
        }

        private void ShowInfo()
        {
            Title = string.Format("{0} : {1}", GetLocalResourceObject("title"), GetGlobalResourceObject("MasterPage", "SiteMotto"));

            FillAboutSite();
            FillTblPages();
            FillSiteNews();

            FillLastProducts();
            FillTblLastCompanies();
            FillProductsWithLastComments();

            ShowForBeginners();
            SetLocalText();
        }

        private void SetLocalText()
        {

            lblForNewcomers.Text = GetLocalResourceObject("lblForNewcomers").ToString();
            lblNcProducts.Text = GetLocalResourceObject("lblNcProducts").ToString();
            lblNcProductsText.Text = GetLocalResourceObject("lblNcProductsText").ToString();

            lblNcCompanies.Text = GetLocalResourceObject("lblNcCompanies").ToString();
            lblNcCompaniesText.Text = GetLocalResourceObject("lblNcCompaniesText").ToString();

            lblNcOpinions.Text = GetLocalResourceObject("lblNcOpinions").ToString();
            lblNcOpinionsText.Text = GetLocalResourceObject("lblNcOpinionsText").ToString();

            lblNcCategories.Text = GetLocalResourceObject("lblNcCategories").ToString();
            lblNcCategoriesText.Text = GetLocalResourceObject("lblNcCategoriesText").ToString();

            lblNcOthers.Text = GetLocalResourceObject("lblNcOthers").ToString();

            lblNcOthersZero.Text = GetLocalResourceObject("lblNcOthersZero").ToString();
            lblNcOthersOne.Text = GetLocalResourceObject("lblNcOthersOne").ToString();

            lblNcOthersTwo.Text = GetLocalResourceObject("lblNcOthersTwo").ToString();
            hlNcOthersTwo.Text = GetLocalResourceObject("hlNcOthersTwo").ToString();
            hlNcOthersTwo.NavigateUrl = GetUrlWithVariant("Guide.aspx#ginfrt");
            lblNcOthersTwo2.Text = GetLocalResourceObject("lblNcOthersTwo2").ToString();

            lblNcOthersThree.Text = GetLocalResourceObject("lblNcOthersThree").ToString();

            lblNcOthersFour.Text = GetLocalResourceObject("lblNcOthersFour").ToString();
            hlNcOthersFour.Text = GetLocalResourceObject("hlNcOthersFour").ToString();
            hlNcOthersFour.NavigateUrl = GetUrlWithVariant("Guide.aspx#infts");
            lblNcOthersFour2.Text = GetLocalResourceObject("lblNcOthersFour2").ToString();

            lblNcOthersFive.Text = GetLocalResourceObject("lblNcOthersFive").ToString();
            //------------------------------------------------

            BusinessUser businessUser = new BusinessUser();
            long usersBrowsing = businessUser.GetGuests() + businessUser.GetLoggedUsers().Count;

            if (usersBrowsing < 1)
            {
                usersBrowsing = 1;
            }

            BusinessStatistics statistics = new BusinessStatistics();
            lblStatisics.Text = GetLocalResourceObject("StatisticsForToday").ToString();

            lblUsersBrowsing.Text = string.Format("{0} {1}", GetLocalResourceObject("UsersOnline"), usersBrowsing);
            lblCommentsWritten.Text = string.Format("{0} {1}", GetLocalResourceObject("CommentsWritten")
                , statistics.getTodaysWrittenComments(objectContext));
            lblProductsAdded.Text = string.Format("{0} {1}", GetLocalResourceObject("ProductsAdded")
                , statistics.getTodaysProductsCreated(objectContext));
            lblCompaniesAdded.Text = string.Format("{0} {1}", GetLocalResourceObject("CompaniesAdded")
               , statistics.getTodaysCompaniesCreated(objectContext));
            lblUserRegistered.Text = string.Format("{0} {1}", GetLocalResourceObject("UsersRegistered")
                , statistics.GetTodaysUsersRegistered(objectContext));
            lblVisits.Text = string.Format("{0} {1}", GetLocalResourceObject("Visits")
                , statistics.getTodaysSessions(objectContext));
        }

        private void ShowForBeginners()
        {
            BusinessProduct bProduct = new BusinessProduct();
            ImageTools imgTools = new ImageTools();

            Product currProduct = null;
            ProductImage pImage = null;

            string appPath = CommonCode.PathMap.PhysicalApplicationPath;

            //put the ID's of the example products used for introduction
            int[] productIds = new int[1] { 0 };
            //put the ID's of the example companies used for introduction
            int[] companyIds = new int[1] { 0 };
            //put the ID's of the example products, which have opinions
            int[] productsWithOpinionsIds = new int[1] { 0 };
            //put the ID's of the example categories, which have a lot of products
            int[] categoriesIds = new int[1] { 0 };

            switch (Tools.ApplicationVariantString)
            {
                case "bg":

                    productIds = new int[5] { 53, 58, 54, 55, 59 };
                    companyIds = new int[5] { 9, 29, 25, 26, 28 };
                    productsWithOpinionsIds = new int[5] { 53, 58, 54, 55, 59 };
                    categoriesIds = new int[5] { 33, 342, 20, 103, 381 };

                    break;
                case "en":

                    productIds = new int[5] { 58, 61, 59, 60, 62 };
                    companyIds = new int[5] { 9, 31, 29, 30, 32 };
                    productsWithOpinionsIds = new int[5] { 58, 61, 59, 60, 62 };
                    categoriesIds = new int[5] { 33, 342, 20, 103, 381 };

                    break;
                default:
                    throw new CommonCode.UIException(string.Format("ApplicationVariantString = {0} is not supported variant.", Tools.ApplicationVariantString));
            }

            string[,] info = new string[5, 4];

            string url = "Product.aspx?Product=";

            for (int i = 0; i < 5; i++)
            {
                currProduct = bProduct.GetProductByID(objectContext, productIds[i]);
                if (currProduct == null)
                {
                    //throw new CommonCode.UIException(string.Format("There is no product with ID = {0}", productIds[i]));
                    break;
                }

                info[i, 0] = GetUrlWithVariant(url + currProduct.ID);

                pImage = imgTools.GetProductImageThumbnail(objectContext, currProduct);
                if (pImage == null)
                {
                    throw new CommonCode.UIException("couldn`t get image for product " + currProduct.ID);
                }

                info[i, 1] = pImage.url;
                info[i, 2] = pImage.width.ToString();
                info[i, 3] = pImage.height.ToString();

            }

            FillTblForBeginners(tblNcProducts, info);

            // -------------------------------

            BusinessCompany bCompany = new BusinessCompany();

            Company currCompany = null;
            CompanyImage cImage = null;

            info = new string[5, 4];

            url = "Company.aspx?Company=";

            for (int i = 0; i < 5; i++)
            {
                currCompany = bCompany.GetCompany(objectContext, companyIds[i]);
                if (currCompany == null)
                {
                    //throw new CommonCode.UIException(string.Format("There is no company with ID = {0}", companyIds[i]));
                    break;
                }

                info[i, 0] = GetUrlWithVariant(url + currCompany.ID);

                cImage = imgTools.GetCompanyImageThumbnail(objectContext, currCompany);
                if (cImage == null)
                {
                    throw new CommonCode.UIException("couldn`t get image for company " + currCompany.ID);
                }

                info[i, 1] = cImage.url;
                info[i, 2] = cImage.width.ToString();
                info[i, 3] = cImage.height.ToString();
            }

            FillTblForBeginners(tblNcCompanies, info);

            // --------------------------------------


            string[] imgurls = new string[5] { "images/SiteImages/opn1.jpg", "images/SiteImages/opn2.jpg"
            , "images/SiteImages/opn3.jpg", "images/SiteImages/opn4.jpg", "images/SiteImages/opn5.jpg"};
            url = "Product.aspx?Product=";

            info = new string[5, 4];

            for (int i = 0; i < 5; i++)
            {
                currProduct = bProduct.GetProductByID(objectContext, productsWithOpinionsIds[i]);
                if (currProduct == null)
                {
                    //throw new CommonCode.UIException(string.Format("There is no product with ID = {0}", productsWithOpinionsIds[i]));
                    break;
                }

                info[i, 0] = GetUrlWithVariant(url + currProduct.ID + "#opinions");
                info[i, 1] = imgurls[i];
                info[i, 2] = string.Empty;
                info[i, 3] = string.Empty;
            }

            FillTblForBeginners(tblNcOpinions, info);

            // ----------------------------------------

            BusinessCategory bCategory = new BusinessCategory();
            Category currCategory = null;

            info = new string[5, 4];

            url = "Category.aspx?Category=";

            for (int i = 0; i < 5; i++)
            {
                currCategory = bCategory.Get(objectContext, categoriesIds[i]);
                if (currCategory == null)
                {
                    //throw new CommonCode.UIException(string.Format("There is no category with ID = {0}", categoriesIds[i]));
                    break;
                }

                info[i, 0] = GetUrlWithVariant(url + currCategory.ID);
                info[i, 1] = currCategory.imageUrl;
                info[i, 2] = currCategory.imageWidth.ToString();
                info[i, 3] = currCategory.imageHeight.ToString();
            }

            FillTblForBeginners(tblNcCategories, info);
        }

        private void FillTblForBeginners(Table tbl, string[,] info)
        {
            tbl.Rows.Clear();

            ImageTools imgTools = new ImageTools();

            int count = info.Length / 4;
            if (count < 5)
            {
                throw new CommonCode.UIException("less than 5 elements in 'string[][] info'");
            }
            for (int i = 0; i < count; i++)
            {
                if (info[i, 0] == null)
                {
                    return;
                }
            }

            TableRow newRow = new TableRow();
            tbl.Rows.Add(newRow);

            int imgWidth = 0;
            int imgHeight = 0;

            for (int i = 0; i < count; i++)
            {
                TableCell newCell = new TableCell();
                newRow.Cells.Add(newCell);

                newCell.Width = Unit.Percentage(100 / count);
                newCell.HorizontalAlign = HorizontalAlign.Center;
                newCell.VerticalAlign = VerticalAlign.Middle;

                HyperLink link = new HyperLink();
                newCell.Controls.Add(link);

                link.NavigateUrl = info[i, 0];
                link.Target = "_blank";

                Image img = new Image();
                link.Controls.Add(img);

                img.ImageUrl = info[i, 1];

                if (info[i, 2] != string.Empty &&
                    info[i, 3] != string.Empty)
                {
                    if (!int.TryParse(info[i, 2], out imgWidth))
                    {
                        throw new CommonCode.UIException(string.Format("Couldn`t parse {0} to integer (image width)", info[i, 2]));
                    }

                    if (!int.TryParse(info[i, 3], out imgHeight))
                    {
                        throw new CommonCode.UIException(string.Format("Couldn`t parse {0} to integer (image height)", info[i, 3]));
                    }

                    CommonCode.ImagesAndAdverts.ResizeImage(imgHeight, imgWidth, 120, 120, out imgHeight, out imgWidth);

                    img.Width = imgWidth;
                    img.Height = imgHeight;
                }
                else
                {
                    img.Width = 120;
                }
            }

        }

        public void CheckParameters()
        {
            BusinessSiteText bSiteText = new BusinessSiteText();
            NewsNumber = bSiteText.CountSiteNews(objectContext);

            CommonCode.Pages.CheckPageParameters(Response, NewsNumber, NewsOnPage.ToString(),
                Request.Params["page"], GetUrlWithVariant("Home.aspx"), out CurrNewsPage, out NewsOnPage);
        }

        private void FillTblPages()
        {
            string urlToAppend = GetUrlWithVariant("Home.aspx#news");

            tblPages.Rows.Clear();
            tblPages.Rows.Add(CommonCode.Pages.GetPagesRow(NewsNumber, NewsOnPage, CurrNewsPage, urlToAppend));

            tblPagesBtm.Rows.Clear();
            tblPagesBtm.Rows.Add(CommonCode.Pages.GetPagesRow(NewsNumber, NewsOnPage, CurrNewsPage, urlToAppend));
        }

        private void FillLastProducts()
        {
            tblLastProducts.Rows.Clear();

            BusinessProduct businessProduct = new BusinessProduct();

            List<Product> lastProducts = businessProduct.GetLastAddedProducts
                (objectContext, Configuration.HomePageNumberLastAddedProducts, true);

            if (lastProducts != null && lastProducts.Count > 0)
            {

                pnlLastProducts.Visible = true;

                lblLastProducts.Text = string.Format("{0} {2}", GetLocalResourceObject("LastProducts")
                    , lastProducts.Count, GetLocalResourceObject("LastProducts2"));

                ImageTools imgTools = new ImageTools();
                ProductImage pImage = null;

                int i = 0;
                int FieldsPerRow = 5;

                int width = 0;
                int height = 0;

                TableRow newRow = new TableRow();
                tblLastProducts.Rows.Add(newRow);

                foreach (Product product in lastProducts)
                {
                    if (i++ >= FieldsPerRow)
                    {
                        newRow = new TableRow();
                        tblLastProducts.Rows.Add(newRow);
                        i = 1;
                    }

                    TableCell newCell = new TableCell();
                    newRow.Cells.Add(newCell);
                    newCell.HorizontalAlign = HorizontalAlign.Center;
                    newCell.Width = Unit.Percentage(100 / FieldsPerRow);

                    HyperLink productLink = new HyperLink();
                    newCell.Controls.Add(productLink);
                    productLink.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", product.ID));

                    pImage = imgTools.GetProductImageThumbnail(objectContext, product);
                    if (pImage != null)
                    {
                        Image img = new Image();
                        productLink.Controls.Add(img);

                        img.CssClass = "HomeImageLastProducts";
                        img.BorderWidth = Unit.Pixel(1);

                        img.ImageUrl = pImage.url;

                        CommonCode.ImagesAndAdverts.ResizeImage(pImage.height, pImage.width, 100, 120, out height, out width);
                        img.Height = height;

                        newCell.Controls.Add(CommonCode.UiTools.GetNewLineControl());

                        HyperLink prodName = CommonCode.UiTools.GetProductHyperLink(product);
                        newCell.Controls.Add(prodName);
                        prodName.Font.Size = FontUnit.Medium;
                        prodName.Text = Tools.BreakLongWordsInString(product.name, 20);
                    }
                }
            }
            else
            {
                pnlLastProducts.Visible = false;
            }

        }

        private void FillTblLastCompanies()
        {
            tblLastCompanies.Rows.Clear();

            BusinessCompany bCompany = new BusinessCompany();

            List<Company> lastCompanies = bCompany.GetLastVisibleAddedCompanies(objectContext, Configuration.HomePageNumberLastAddedCompanies);

            if (lastCompanies != null && lastCompanies.Count > 0)
            {

                TableRow firstRow = new TableRow();
                tblLastCompanies.Rows.Add(firstRow);

                TableCell firstCell = new TableCell();
                firstRow.Cells.Add(firstCell);
                firstCell.HorizontalAlign = HorizontalAlign.Center;


                Label headerLbl = new Label();
                firstCell.Controls.Add(headerLbl);

                headerLbl.Text = string.Format("{0} {2}", GetLocalResourceObject("LastCompanies")
                    , lastCompanies.Count, GetLocalResourceObject("LastCompanies2"));
                headerLbl.CssClass = "sectionTextHeader";
                firstCell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingBottom, "5px");

                string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);
                int i = 1;

                foreach (Company company in lastCompanies)
                {
                    TableRow newRow = new TableRow();
                    tblLastCompanies.Rows.Add(newRow);

                    TableCell newCell = new TableCell();
                    newRow.Cells.Add(newCell);

                    Image newImg = new Image();
                    newImg.ImageUrl = "~/images/SiteImages/triangle.png";
                    newImg.CssClass = "itemImage";

                    newCell.Controls.Add(newImg);

                    HyperLink companyLink = CommonCode.UiTools.GetCompanyHyperLink(company);
                    companyLink.ID = string.Format("comp{0}lnk{1}", company.ID, i);
                    companyLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}{2}','{3}')", company.ID, contPageId, companyLink.ClientID, pnlPopUp.ClientID));
                    companyLink.Attributes.Add("onmouseout", "HideData()");
                    newCell.Controls.Add(companyLink);
                    companyLink.Text = Tools.BreakLongWordsInString(company.name, 20);

                    i++;
                }
            }
            else
            {
                tblLastCompanies.Visible = false;
            }

        }

        private void FillProductsWithLastComments()
        {
            tblLastProductsWithComments.Rows.Clear();

            BusinessProduct bProduct = new BusinessProduct();

            List<Product> lastProducts = bProduct.GetProductsWithMostRecentComments(objectContext, 10);

            if (lastProducts != null && lastProducts.Count > 0)
            {

                TableRow firstRow = new TableRow();
                tblLastProductsWithComments.Rows.Add(firstRow);

                TableCell firstCell = new TableCell();
                firstRow.Cells.Add(firstCell);
                firstCell.HorizontalAlign = HorizontalAlign.Center;
                firstCell.CssClass = "sectionTextHeader";
                firstCell.Text = string.Format("{0}", GetLocalResourceObject("LastCommentedProducts"));
                firstCell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingBottom, "5px");

                string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);
                int i = 1;

                foreach (Product product in lastProducts)
                {
                    TableRow newRow = new TableRow();
                    tblLastProductsWithComments.Rows.Add(newRow);

                    TableCell newCell = new TableCell();
                    newRow.Cells.Add(newCell);

                    Image newImg = new Image();
                    newImg.ImageUrl = "~/images/SiteImages/triangle.png";
                    newImg.CssClass = "itemImage";

                    newCell.Controls.Add(newImg);

                    HyperLink prodLink = CommonCode.UiTools.GetProductHyperLink(product);
                    prodLink.ID = string.Format("prod{0}lnk{1}lc", product.ID, i);
                    prodLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}{2}','{3}')", product.ID, contPageId, prodLink.ClientID, pnlPopUp.ClientID));
                    prodLink.Attributes.Add("onmouseout", "HideData()");
                    newCell.Controls.Add(prodLink);
                    prodLink.Text = Tools.BreakLongWordsInString(product.name, 20);

                    i++;
                }
            }
            else
            {
                tblLastProductsWithComments.Visible = false;
            }

        }

        private void FillSiteNews()
        {
            phSiteNews.Controls.Clear();

            BusinessUser businessUser = new BusinessUser();
            BusinessSiteText businessSiteText = new BusinessSiteText();

            long from = 0;
            long to = 0;
            CommonCode.Pages.GetFromItemNumberToItemNumber(CurrNewsPage, NewsOnPage, out from, out to);

            List<SiteNews> LastNews = businessSiteText.GetLastTexts(objectContext, int.MaxValue, "news", 1);

            Boolean userIsAdmin = false;
            if (currentUser != null && businessUser.IsAdminOrGlobalAdmin(currentUser))
            {
                userIsAdmin = true;
            }

            if (LastNews.Count > 0)
            {
                lblSiteMessages.Text = GetLocalResourceObject("News").ToString();

                int i = 0;

                foreach (SiteNews news in LastNews)
                {
                    if (i >= from && i < to)
                    {

                        Panel msgPanel = new Panel();
                        phSiteNews.Controls.Add(msgPanel);
                        msgPanel.CssClass = "newsPnl";

                        Panel hdrPnl = new Panel();
                        msgPanel.Controls.Add(hdrPnl);
                        hdrPnl.CssClass = "newsHdrPnl";

                        Label dateLbl = new Label();
                        hdrPnl.Controls.Add(dateLbl);
                        dateLbl.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(news.dateCreated);

                        Label nameLbl = CommonCode.UiTools.GetLabelWithText(string.Format("&nbsp;&nbsp;&nbsp;{0}", news.name), false);
                        hdrPnl.Controls.Add(nameLbl);
                        nameLbl.CssClass = "textHeader";

                        Panel newPanel = new Panel();
                        hdrPnl.Controls.Add(newPanel);
                        newPanel.CssClass = "floatRight";

                        if (userIsAdmin)
                        {
                            newPanel.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format(" ID:{0} ", news.ID.ToString()), false));
                        }

                        Panel descrPanel = new Panel();
                        msgPanel.Controls.Add(descrPanel);
                        descrPanel.CssClass = "homeNewsPnl";

                        Label descrlbl = CommonCode.UiTools.GetLabelWithText(news.description, false);
                        descrPanel.Controls.Add(descrlbl);
                    }

                    i++;
                }
            }
            else
            {
                // no entered news
                lblSiteMessages.Text = GetLocalResourceObject("NoSiteMessages").ToString();
                phSiteNews.Visible = false;
            }
        }

        private void FillAboutSite()
        {
            BusinessUser businessUser = new BusinessUser();
            BusinessSiteText siteText = new BusinessSiteText();
            SiteNews about = siteText.GetSiteText(objectContext, "about");

            lblAboutSiteHeader.Text = GetLocalResourceObject("About").ToString();

            if (about != null && about.visible)
            {
                lblAboutSite.Text = about.description;
            }
            else
            {
                lblAboutSite.Text = "About site text is not written.";
            }
        }

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            string result = CommonCode.WebMethods.GetTypeData(type, Id);
            return result;
        }




    }
}
