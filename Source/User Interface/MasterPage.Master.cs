﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

using BusinessLayer;
using DataAccess;
using log4net;

namespace UserInterface
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        private static ILog log = LogManager.GetLogger(typeof(BasePage));
        private static object navMenuCacheSync = new object();

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;
        private BusinessCategory businessCategory = new BusinessCategory();

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CommonCode.UiTools.CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            string oldOnsubmit = form1.Attributes["onsubmit"];
            if (oldOnsubmit == null)
            {
                oldOnsubmit = string.Empty;
            }
            if (string.IsNullOrEmpty(oldOnsubmit) == false)
            {
                oldOnsubmit = oldOnsubmit.TrimEnd();
                if (oldOnsubmit.EndsWith(";") == false)
                {
                    oldOnsubmit = oldOnsubmit + "; ";
                }
                else
                {
                    oldOnsubmit = oldOnsubmit + " ";
                }
            }
            string newOnsubmit = string.Format("{0}breakHtmlTags('{1}'); return true;", oldOnsubmit, form1.ClientID);
            form1.Attributes.Add("onsubmit", newOnsubmit);

            string browserType = Request.Browser.Type.ToUpper();     // Changes style because there is 1px difference between IE and other browsers
            if (browserType.Contains("IE") && browserType != "IE5")  // Because chrome is detected as IE5
            {                                                        // THIS code is pasted also in AddProduct/Company/Product pages (in menus, with check for IE9)
                headerMenuDiv.Attributes.Add("class", "menusIE");
            }
            else
            {
                headerMenuDiv.Attributes.Add("class", "menusOther");
            }
            //

            imgAdvSearch.Attributes.Add("onclick", string.Format("ShowAdvancedSearchPanel('{0}');", pnlAdvSearch.ClientID));
            divCloseAdvSearch.Attributes.Add("onclick", string.Format("HideElementWithID('{0}','{1}');", pnlAdvSearch.ClientID, "false"));
            tbSearch.Attributes.Add("onclick", string.Format("HideElementWithID('{0}','{1}');", pnlAdvSearch.ClientID, "false"));
            btnSearch.Attributes.Add("onclick", string.Format("HideElementWithID('{0}','{1}');", pnlAdvSearch.ClientID, "false"));

            divCloseOptions.Attributes.Add("onclick", string.Format("HideElementWithID('{0}','{1}');", "divOptions", "true"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ShowLoginStatus();                      // shows Login Control if not logged, otherwise log out link

            DateTime now = DateTime.UtcNow;
            ShowNavigationMenu();                   // shows navigation menu
            TimeSpan span = DateTime.UtcNow - now;
            lblCatTime.Text = ", Categories : " + span.TotalSeconds.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);

            ShowInfo();
        }

        private void ShowInfo()
        {

#if DEBUG
            loadTimeDiv.Visible = false;
#else
            loadTimeDiv.Visible = false;
#endif

            UpdateClientTheme(false);

            BusinessUser businessUser = new BusinessUser();
            User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, true);

            bool isAdmin = false;

            pnlAdmin.Visible = false;
            if (currentUser != null)
            {
                if (businessUser.IsFromAdminTeam(currentUser))
                {
                    isAdmin = true;
                }

                ShowAlertIcons(currentUser);
            }

            imgAdvSearch.ToolTip = GetGlobalResourceObject("MasterPage", "advSearchTooltip").ToString();

            hlHome.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Home.aspx");
            hlLogoToHome.NavigateUrl = hlHome.NavigateUrl;

            ShowLinksMenu(businessUser, currentUser, isAdmin);
            ShowUserMenu(businessUser, currentUser, isAdmin);

            if (pnlError.Visible == true)
            {
                pnlError.Visible = false;
            }

            FillAdvancedSearchCBL();
            SetLocalText();

            // Show the client time zone
            int clientOffsetInMinutes = CommonCode.UiTools.GetClientTimeZoneOffsetInMinutesFromSession();
            string timeZoneStr = "GMT";
            if (clientOffsetInMinutes != 0)
            {
                TimeSpan clientOffsetNegated = new TimeSpan(0, -clientOffsetInMinutes, 0);
                timeZoneStr =
                    string.Format("{0}{1}{2:00}:{3:00}",
                    timeZoneStr,
                    (clientOffsetInMinutes <= 0) ? "+" : "-",
                    clientOffsetNegated.Hours,
                    clientOffsetNegated.Minutes);
            }

#if DEBUG
            lblTimeZone.Visible = false;
            lblTimeZone.Text = timeZoneStr;
#else
            lblTimeZone.Visible = false;
#endif

        }

        private void FillAdvancedSearchCBL()
        {
            if (IsPostBack == false)
            {
                cblAdvSearch.Items.Clear();

                ListItem prodItem = new ListItem();
                cblAdvSearch.Items.Add(prodItem);
                prodItem.Text = GetGlobalResourceObject("SiteResources", "products").ToString();
                prodItem.Selected = true;
                prodItem.Value = "0";

                ListItem compItem = new ListItem();
                cblAdvSearch.Items.Add(compItem);
                compItem.Text = GetGlobalResourceObject("SiteResources", "companies").ToString();
                prodItem.Value = "1";

                ListItem usersItem = new ListItem();
                cblAdvSearch.Items.Add(usersItem);
                usersItem.Text = GetGlobalResourceObject("SiteResources", "users").ToString();
                prodItem.Value = "2";

                ListItem catItem = new ListItem();
                cblAdvSearch.Items.Add(catItem);
                catItem.Text = GetGlobalResourceObject("SiteResources", "categories").ToString();
                prodItem.Value = "3";
            }
        }

        private void ShowAlertIcons(User currentUser)
        {
            BusinessUserOptions userOptions = new BusinessUserOptions();
            if (userOptions.CheckIfUserHavesNewMessages(userContext, currentUser) == true)
            {
                imgNewMail.Visible = true;
                imgNewMail.ToolTip = GetGlobalResourceObject("MasterPage", "tooltipImgNewMail").ToString();
            }
            if (userOptions.CheckIfUserHavesNewSystemMessages(userContext, currentUser) == true)
            {
                imgNewSysMsg.Visible = true;
                imgNewSysMsg.ToolTip = GetGlobalResourceObject("MasterPage", "tooltipImgNewSysMsg").ToString();
            }
            if (userOptions.CheckIfUserHavesNewWarning(userContext, currentUser) == true)
            {
                imgNewWarning.Visible = true;
                imgNewWarning.ToolTip = GetGlobalResourceObject("MasterPage", "tooltipImgNewWarning").ToString();
            }
            if (userOptions.CheckIfUserHavesNewContentOnNotifies(objectContext, currentUser) == true)
            {
                imgNewContent.Visible = true;
                imgNewContent.ToolTip = GetGlobalResourceObject("MasterPage", "tooltipImgNewContent").ToString();
            }
            if (userOptions.CheckIfUserHavesNewReplyToReport(userContext, currentUser) == true)
            {
                imgNewReportReply.Visible = true;
                imgNewReportReply.ToolTip = GetGlobalResourceObject("MasterPage", "tooltipImgNewReplyToReport").ToString();
            }
            if (userOptions.CheckIfUserHaveUnseenTypeSuggestionData(userContext, currentUser) == true)
            {
                imgNewEditSuggestionData.Visible = true;
                imgNewEditSuggestionData.ToolTip = GetGlobalResourceObject("MasterPage", "tooltipImgNewSuggComment").ToString();
            }
        }

        private void SetLocalText()
        {
            lblSiteMotto0.Text = GetGlobalResourceObject("MasterPage", "SiteMotto").ToString();
            lblOptions.Text = GetGlobalResourceObject("MasterPage", "optionslbl").ToString();
            btnSearch.Text = GetGlobalResourceObject("MasterPage", "Search").ToString();
            lblJavascriptError.Text = GetGlobalResourceObject("MasterPage", "errJavaScript").ToString();
            lblSite.Text = GetGlobalResourceObject("MasterPage", "bottomText").ToString();

            hlHome.Text = GetGlobalResourceObject("MasterPage", "Home").ToString();

            lblWidth.Text = GetGlobalResourceObject("MasterPage", "Width").ToString();
            lblMinWidthIs.Text = GetGlobalResourceObject("MasterPage", "MinWidth").ToString();
            lblCurrently.Text = GetGlobalResourceObject("MasterPage", "currently").ToString();
            lblMaxWidthIs.Text = GetGlobalResourceObject("MasterPage", "MaxWidth").ToString();
            cbMaxWidth.Text = GetGlobalResourceObject("MasterPage", "cbMaxWidth").ToString();
            btnSaveOptions.Text = GetGlobalResourceObject("MasterPage", "Save").ToString();
            btnResetOptions.Text = GetGlobalResourceObject("MasterPage", "Reset").ToString();
            lblOr.Text = GetGlobalResourceObject("MasterPage", "or").ToString();

            imgEngFlag.ToolTip = GetGlobalResourceObject("MasterPage", "English").ToString();
            imgBulgFlag.ToolTip = GetGlobalResourceObject("MasterPage", "Bulgarian").ToString();

            hlEnglish.NavigateUrl = Configuration.SiteDomainAdress + "en/Home.aspx";
            hlBulgarian.NavigateUrl = Configuration.SiteDomainAdress + "bg/Home.aspx";


            imgNewContent.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Notifications.aspx");
            imgNewEditSuggestionData.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("EditSuggestions.aspx");
            imgNewMail.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Messages.aspx");
            imgNewReportReply.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("MyReports.aspx");
            imgNewSysMsg.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Profile.aspx");
            imgNewWarning.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Profile.aspx");

        }

        private void ShowLinksMenu(BusinessUser businessUser, User currentUser, bool isAdmin)
        {
            if (IsPostBack == false)
            {
                string browserType = Request.Browser.Type.ToUpper();
                if (browserType.Contains("IE") && browserType != "IE5" && browserType != "IE9")  // Because chrome is detected as IE5
                {                                           // THIS code is pasted also in AddProduct and Company pages (in menus)
                    navLinks.DynamicHorizontalOffset = -1;
                    navLinks.DynamicVerticalOffset = -2;
                }

                MenuItem mainItem = new MenuItem();
                navLinks.Items.Add(mainItem);
                mainItem.Text = CommonCode.UiTools.HackNavigationMenu(
                    GetGlobalResourceObject("MasterPage", "Menu").ToString(), false, true);
                mainItem.Selectable = false;

                if (currentUser != null)
                {
                    if (isAdmin == true)
                    {

                        MenuItem adminItems = new MenuItem();
                        mainItem.ChildItems.Add(adminItems);
                        adminItems.Text = CommonCode.UiTools.HackNavigationMenu("Administration", false, false);
                        adminItems.Selectable = false;


                        MenuItem warnItem = new MenuItem();
                        adminItems.ChildItems.Add(warnItem);
                        warnItem.Text = CommonCode.UiTools.HackNavigationMenu("Warnings", true, false);
                        warnItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Warnings.aspx");

                        MenuItem repItem = new MenuItem();
                        adminItems.ChildItems.Add(repItem);
                        repItem.Text = CommonCode.UiTools.HackNavigationMenu("Reports", true, false);
                        repItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Reports.aspx");

                        MenuItem cpItem = new MenuItem();
                        adminItems.ChildItems.Add(cpItem);
                        cpItem.Text = CommonCode.UiTools.HackNavigationMenu("Last types", true, false);
                        cpItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("LastCP.aspx");

                        MenuItem commItem = new MenuItem();
                        adminItems.ChildItems.Add(commItem);
                        commItem.Text = CommonCode.UiTools.HackNavigationMenu("Last comments", true, false);
                        commItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("LastComments.aspx");

                        MenuItem addressItem = new MenuItem();
                        adminItems.ChildItems.Add(addressItem);
                        addressItem.Text = CommonCode.UiTools.HackNavigationMenu("Address activity", true, false);
                        addressItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("AddressActivity.aspx");

                        MenuItem logItem = new MenuItem();
                        adminItems.ChildItems.Add(logItem);
                        logItem.Text = CommonCode.UiTools.HackNavigationMenu("Logs", true, false);
                        logItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Logs.aspx");

                        MenuItem statItem = new MenuItem();
                        adminItems.ChildItems.Add(statItem);
                        statItem.Text = CommonCode.UiTools.HackNavigationMenu("Statistics", true, false);
                        statItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Statistics.aspx");

                        if (!businessUser.IsModerator(currentUser))
                        {
                            MenuItem advertItem = new MenuItem();
                            adminItems.ChildItems.Add(advertItem);
                            advertItem.Text = CommonCode.UiTools.HackNavigationMenu("Advertisements", true, false);
                            advertItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Advertisements.aspx");

                            MenuItem adminItem = new MenuItem();
                            adminItems.ChildItems.Add(adminItem);
                            adminItem.Text = CommonCode.UiTools.HackNavigationMenu("Administrators", true, false);
                            adminItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Administrators.aspx");

                            MenuItem textItem = new MenuItem();
                            adminItems.ChildItems.Add(textItem);
                            textItem.Text = CommonCode.UiTools.HackNavigationMenu("Texts", true, false);
                            textItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("SiteTexts.aspx");
                        }

                    }



                    if (businessUser.CanUserDo(userContext, currentUser, UserRoles.AddProducts))
                    {
                        MenuItem addProdItem = new MenuItem();
                        mainItem.ChildItems.Add(addProdItem);
                        addProdItem.Text = CommonCode.UiTools.HackNavigationMenu(
                            GetGlobalResourceObject("MasterPage", "AddProduct").ToString(), true, false);
                        addProdItem.Selectable = true;
                        addProdItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("AddProduct.aspx");
                    }

                    if (businessUser.CanUserDo(userContext, currentUser, UserRoles.AddCompanies))
                    {
                        MenuItem addCompItem = new MenuItem();
                        mainItem.ChildItems.Add(addCompItem);
                        addCompItem.Text = CommonCode.UiTools.HackNavigationMenu(
                            GetGlobalResourceObject("MasterPage", "AddMaker").ToString(), true, false);
                        addCompItem.Selectable = true;
                        addCompItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("AddCompany.aspx");
                    }
                }

                MenuItem guideItem = new MenuItem();
                mainItem.ChildItems.Add(guideItem);
                guideItem.Text = CommonCode.UiTools.HackNavigationMenu(
                    GetGlobalResourceObject("MasterPage", "Guide").ToString(), true, false);
                guideItem.Selectable = true;
                guideItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Guide.aspx");

                MenuItem suggItem = new MenuItem();
                mainItem.ChildItems.Add(suggItem);
                suggItem.Text = CommonCode.UiTools.HackNavigationMenu(
                    GetGlobalResourceObject("MasterPage", "Suggestions").ToString(), true, false);
                suggItem.Selectable = true;
                suggItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("SuggestionsForSite.aspx");

                MenuItem rulesItem = new MenuItem();
                mainItem.ChildItems.Add(rulesItem);
                rulesItem.Text = CommonCode.UiTools.HackNavigationMenu(
                    GetGlobalResourceObject("MasterPage", "Rules").ToString(), true, false);
                rulesItem.Selectable = true;
                rulesItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Rules.aspx");

                MenuItem faqItem = new MenuItem();
                mainItem.ChildItems.Add(faqItem);
                faqItem.Text = CommonCode.UiTools.HackNavigationMenu(
                    GetGlobalResourceObject("MasterPage", "FAQ").ToString(), true, false);
                faqItem.Selectable = true;
                faqItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("FAQ.aspx");

                MenuItem aboutItem = new MenuItem();
                mainItem.ChildItems.Add(aboutItem);
                aboutItem.Text = CommonCode.UiTools.HackNavigationMenu(
                    GetGlobalResourceObject("MasterPage", "About").ToString(), true, false);
                aboutItem.Selectable = true;
                aboutItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("AboutUs.aspx");
            }
        }

        private void ShowUserMenu(BusinessUser businessUser, User currentUser, bool isAdmin)
        {
            if (IsPostBack == false && currentUser != null)
            {
                userMenu.Visible = true;

                MenuItem mainItem = new MenuItem();
                userMenu.Items.Add(mainItem);
                mainItem.Text = CommonCode.UiTools.HackNavigationMenu(GetGlobalResourceObject("MasterPage", "Profile").ToString(), true, true);  // 
                mainItem.Selectable = true;
                mainItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Profile.aspx");

                MenuItem addCompItem = new MenuItem();
                mainItem.ChildItems.Add(addCompItem);
                addCompItem.Text = CommonCode.UiTools.HackNavigationMenu(GetGlobalResourceObject("MasterPage", "Messages").ToString(), true, false); // 
                addCompItem.Selectable = true;
                addCompItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Messages.aspx");

                MenuItem notifsItem = new MenuItem();
                mainItem.ChildItems.Add(notifsItem);
                notifsItem.Text = CommonCode.UiTools.HackNavigationMenu(GetGlobalResourceObject("MasterPage", "Notifies").ToString(), true, false); // 
                notifsItem.Selectable = true;
                notifsItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Notifications.aspx");

                if (!isAdmin)
                {
                    MenuItem logInItem = new MenuItem();
                    mainItem.ChildItems.Add(logInItem);
                    logInItem.Text = CommonCode.UiTools.HackNavigationMenu(
                        GetGlobalResourceObject("MasterPage", "MyReports").ToString(), true, false); // 
                    logInItem.Selectable = true;
                    logInItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("MyReports.aspx");

                    if (businessUser.IsUser(currentUser))
                    {
                        MenuItem regItem = new MenuItem();
                        mainItem.ChildItems.Add(regItem);
                        regItem.Text = CommonCode.UiTools.HackNavigationMenu(
                            GetGlobalResourceObject("MasterPage", "EditRoles").ToString(), true, false);  // 
                        regItem.Selectable = true;
                        regItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("EditorRights.aspx");

                        MenuItem editSuggItem = new MenuItem();
                        mainItem.ChildItems.Add(editSuggItem);
                        editSuggItem.Text = CommonCode.UiTools.HackNavigationMenu(
                            GetGlobalResourceObject("MasterPage", "EditSuggestions").ToString(), true, false);  // 
                        editSuggItem.Selectable = true;
                        editSuggItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("EditSuggestions.aspx");
                    }
                }
            }

        }

        // Menu appearance workarounds
        private void FixMenuAppearance(Menu menuToFix)
        {
            if (menuToFix == null)
            {
                throw new ArgumentNullException("menuToFix");
            }

            double brVer = 0;

            if (Request.Browser.Browser == "IE")
            {
                // brVer will be zero if the conversion fails.
                double.TryParse(Request.Browser.Version, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out brVer);

            }
            // Menu width
            if ((Request.Browser.Browser == "IE") &&
                (string.Equals(Request.Browser.Platform, "WinXP") == true) &&
                (brVer < 8))
            {
                if (object.ReferenceEquals(menuToFix, navMenu) == true)
                {
                    menuToFix.DynamicMenuStyle.CssClass = "catDynMenuPercentWidth";
                }
                else if (object.ReferenceEquals(menuToFix, navLinks) == true)
                {
                    menuToFix.DynamicMenuStyle.CssClass = "menuDynMenuPercentWidth";
                }
                else
                {
                    // Unknown menu - dDo nothing.
                }
            }
            else
            {
                // Leave it as is.
            }
        }

        private void ShowLoginStatus()
        {
            BusinessUser businessUser = new BusinessUser();
            User currentUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, true);

            if (currentUser != null)
            {
                lblUser.Text = string.Format("{0} {1}!", GetGlobalResourceObject("MasterPage", "Hi"), currentUser.username);

                lblUser.Visible = true;

                hlLogIn.Visible = false;
                hlRegister.Visible = false;
                lblComa.Visible = false;
                lbLogOut.Visible = true;

                lbLogOut.Text = GetGlobalResourceObject("MasterPage", "logout").ToString();
            }
            else
            {
                hlLogIn.Visible = true;
                hlRegister.Visible = true;
                lblComa.Visible = true;
                lbLogOut.Visible = false;

                hlLogIn.Text = GetGlobalResourceObject("MasterPage", "Login").ToString();
                hlLogIn.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("LogIn.aspx");

                hlRegister.Text = GetGlobalResourceObject("MasterPage", "Register").ToString();
                hlRegister.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Registration.aspx");


                lblUser.Visible = true;
                lblUser.Text = GetGlobalResourceObject("MasterPage", "HiGuest").ToString();
            }
        }

        private void ShowNavigationMenu()
        {
            string browserType = Request.Browser.Type.ToUpper();
            if (browserType.Contains("IE") && browserType != "IE5" && browserType != "IE9")  // Because chrome is detected as IE5                                   
            {                                                           // It`s fixed in IE9                                        
                navMenu.DynamicHorizontalOffset = -1;                   // THIS code is pasted also in AddProduct/Company/Product pages (in menus)
                navMenu.DynamicVerticalOffset = -2;
            }


            //// if user can edit categories he can enter every category page
            BusinessUser bussUser = new BusinessUser();
            User currUser = CommonCode.UiTools.GetCurrentUser(userContext, objectContext, true);
            Boolean global = false;
            if (currUser != null)
            {
                if (bussUser.CanAdminDo(userContext, currUser.ID, AdminRoles.EditCategories))
                {
                    global = true;
                }
            }
            /////

            DateTime menuCreationStart = DateTime.UtcNow;
            string navMenuKey = string.Empty;
            if (global == true)
            {
                navMenuKey = string.Format("{0}NavMenuAdminItems", Tools.ApplicationVariantString);
            }
            else
            {
                navMenuKey = string.Format("{0}NavMenuItems", Tools.ApplicationVariantString);
            }

            object cachedMenuItemsObj = Cache.Get(navMenuKey);
            MenuItemCollection newMenuItems = cachedMenuItemsObj as MenuItemCollection;

            if (newMenuItems == null)
            {
                newMenuItems = new MenuItemCollection();

                BusinessCategory businessCategory = new BusinessCategory();
                IEnumerable<Category> categories = businessCategory.GetAllByParentCategoryID(objectContext, null);
                IList<long> addedCategoryIDs = new List<long>();
                foreach (Category category in categories)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Value = category.ID.ToString();

                    if (global || category.last == true)
                    {
                        menuItem.Text = CommonCode.UiTools.HackNavigationMenu(category.name, true, true);
                        menuItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Category.aspx?Category=" + menuItem.Value);
                    }
                    else
                    {
                        menuItem.Text = CommonCode.UiTools.HackNavigationMenu(category.name, false, true);
                        menuItem.Selectable = false;
                    }

                    newMenuItems.Add(menuItem);
                    CheckCategoryNotAdded(addedCategoryIDs, category);
                    AddChildMenuItems(menuItem, category.ID, addedCategoryIDs, global);
                }
                lock (navMenuCacheSync)
                {
                    if (Cache.Get(navMenuKey) == null)  // No another thread has added the menu items to the cache yet
                    {
                        // Sliding expiration
                        Cache.Insert(navMenuKey, newMenuItems, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                            TimeSpan.FromMinutes(Configuration.CacheDefaultExpireTimeInMinutes), System.Web.Caching.CacheItemPriority.Default,
                            new System.Web.Caching.CacheItemRemovedCallback(NavMenuDeletedFromCache));
                    }
                }
            }

            navMenu.Items.Clear();
            foreach (MenuItem mItem in newMenuItems)
            {
                navMenu.Items.Add(mItem);
            }

            DateTime menuCreationEnd = DateTime.UtcNow;
            TimeSpan menuCreationTime = (menuCreationEnd - menuCreationStart);
            double menuCreationSeconds = menuCreationTime.TotalSeconds;
            if (menuCreationSeconds > 0.001)
            {
                if (log.IsInfoEnabled)
                {
                    string msg = string.Format("Navigation menu created in {0:F3} s.", menuCreationSeconds);
                    log.Debug(msg);
                }
            }
        }

        public void UpdateNavigationMenuCache()
        {
            Cache.Remove(string.Format("{0}NavMenuAdminItems", Tools.ApplicationVariantString));
            Cache.Remove(string.Format("{0}NavMenuItems", Tools.ApplicationVariantString));
            ShowNavigationMenu();
        }

        private static void NavMenuDeletedFromCache(string key, object value, System.Web.Caching.CacheItemRemovedReason reason)
        {
            // Empty for now.
        }

        private void AddChildMenuItems(MenuItem menuItem, long parentCategoryID, IList<long> addedCategoryIDs, bool global)
        {
            if (menuItem == null)
            {
                throw new ArgumentNullException("menuItem");
            }
            if (addedCategoryIDs == null)
            {
                throw new ArgumentNullException("addedCategoryIDs");
            }

            List<Category> categories = businessCategory.GetAllByParentCategoryID(objectContext, parentCategoryID);
            foreach (Category category in categories)
            {
                MenuItem childMenuItem = new MenuItem();

                childMenuItem.Value = category.ID.ToString();

                if (global || category.last == true)
                {
                    childMenuItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Category.aspx?Category=" + childMenuItem.Value);

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

                if (category.last == false)     // SAVES ALOT OF TIME
                {
                    AddChildMenuItems(childMenuItem, category.ID, addedCategoryIDs, global);
                }
            }

        }

        public void SetMenuItemUrl(Category category, MenuItem menuItem, bool global)
        {

            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            if (menuItem == null)
            {
                throw new ArgumentNullException("menuItem");
            }

            if (global || category.last == true)
            {
                menuItem.NavigateUrl = CommonCode.UiTools.GetUrlWithVariant("Category.aspx?Category=" + menuItem.Value);
            }
            else
            {
                menuItem.Selectable = false;
            }
        }

        private static void CheckCategoryNotAdded(IList<long> addedCategoryIDs, Category category)
        {
            if (addedCategoryIDs == null)
            {
                throw new ArgumentNullException("addedCategoryIDs");
            }
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            if (addedCategoryIDs.Contains(category.ID))
            {
                throw new InvalidOperationException(string.Format(
                    "The category with ID = {0} is already included in the menu.", category.ID));
            }
            else
            {
                addedCategoryIDs.Add(category.ID);
            }
        }

        protected void LogOutBtn_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            businessUser.RemoveLoggedUser(CommonCode.CurrentUser.GetCurrentUserId());

            Session[CommonCode.CurrentUser.CurrentUserIdKey] = null;
            businessUser.AddGuest(CommonCode.CurrentUser.GetCurrentUserId());

            BusinessStatistics businessStatistic = new BusinessStatistics();
            businessStatistic.UserLoggedOut(userContext);

            BasePage basePg = Page as BasePage;
            if ((basePg != null) && (basePg.NeedToBeLogged == true))
            {
                CommonCode.UiTools.RedirectToOtherUrl("Home.aspx");
            }
            else
            {
                if (BusinessLayer.Configuration.UseUrlRewriting == true)
                {
                    CommonCode.UiTools.RedirectToSameUrl(Request.Url.ToString());
                }
                else
                {
                    CommonCode.UiTools.RedirectToSameUrl(Request.Url.ToString());
                }
            }

        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string urlEncodedCriteria = HttpUtility.UrlEncode(tbSearch.Text);

            System.Text.StringBuilder url = new System.Text.StringBuilder();
            url.Append("Search.aspx?");

            if (cblAdvSearch.Items[0].Selected && cblAdvSearch.Items[1].Selected &&
                cblAdvSearch.Items[2].Selected == false && cblAdvSearch.Items[3].Selected == false)
            {
                url.Append("Search=");
                url.Append(urlEncodedCriteria);
            }
            else
            {
                if (cblAdvSearch.Items[3].Selected)
                {
                    url.Append("categories=yes&");
                }
                if (cblAdvSearch.Items[1].Selected)
                {
                    url.Append("companies=yes&");
                }
                if (cblAdvSearch.Items[0].Selected)
                {
                    url.Append("products=yes&");
                }
                if (cblAdvSearch.Items[2].Selected)
                {
                    url.Append("users=yes&");
                }

                url.Append("Search=");
                url.Append(urlEncodedCriteria);
            }

            CommonCode.UiTools.RedirectToOtherUrl(url.ToString());

        }

        public void ShowCreationTimeSeconds(double creationSeconds)
        {
            lblPageCreatedIn.Text = creationSeconds.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
        }

        public void ClearCreationTimeSeconds()
        {
            lblPageCreatedIn.Text = string.Empty;
        }

        public void ShowInfo(string str)
        {
            lblComms.Text = str;
        }

        protected void btnSaveOptions_Click(object sender, EventArgs e)
        {
            lblOptionError.Visible = false;

            string strMinWidth = tbMinWidth.Text;
            string strMaxWidth = tbMaxWidth.Text;
            bool boolMaxWidth = cbMaxWidth.Checked;
            bool saveSetting = true;

            System.Text.StringBuilder sbError = new System.Text.StringBuilder();
            sbError.Append("<br />");

            int minWidth = 0;
            if (!int.TryParse(strMinWidth, out minWidth))
            {
                sbError.Append(GetGlobalResourceObject("MasterPage", "errMinWidth"));
                sbError.Append("<br />");
                saveSetting = false;
            }

            int maxWidth = 1000;
            if (boolMaxWidth == false)
            {
                if (!int.TryParse(strMaxWidth, out maxWidth))
                {
                    sbError.Append(GetGlobalResourceObject("MasterPage", "errMaxWidth"));
                    sbError.Append("<br />");

                    saveSetting = false;
                }
            }

            string error = string.Empty;
            if (saveSetting == true)
            {
                if (CommonCode.UITheme.SaveSettings(Session, Response, Request, minWidth, maxWidth, boolMaxWidth, out error))
                {
                    UpdateClientTheme(true);
                }
                else
                {
                    lblOptionError.Visible = true;
                    lblOptionError.Text = string.Format("<br />{0}", error);
                }
            }
            else
            {
                lblOptionError.Visible = true;
                lblOptionError.Text = sbError.ToString();
            }
        }

        public void UpdateClientTheme(bool newSettings)
        {
            int minWidth = 0;
            int maxWidth = 0;
            bool boolMaxWidth = false;
            if (CommonCode.UITheme.GetSettings(Session, out minWidth, out maxWidth, out boolMaxWidth))
            {
                if (minWidth < 970)
                {
                    Session["utMaxWidth"] = null;
                    Session["utMinWidth"] = null;
                    Session["utBoolMaxWidth"] = null;

                    throw new CommonCode.UIException("minWidth < 970");
                }
                if (boolMaxWidth == false && maxWidth < minWidth)
                {
                    Session["utMaxWidth"] = null;
                    Session["utMinWidth"] = null;
                    Session["utBoolMaxWidth"] = null;

                    throw new CommonCode.UIException("minWidth > maxWidth");
                }

                pageDiv.Attributes.Clear();
                pageDiv.Attributes.Add("class", "page");

                if (boolMaxWidth == true)
                {
                    pageDiv.Attributes.Add("style", string.Format("min-width:{0}px; max-width:100%;", minWidth));
                }
                else
                {
                    pageDiv.Attributes.Add("style", string.Format("min-width:{0}px; max-width:{1}px;", minWidth, maxWidth));
                }

                if (newSettings)
                {
                    CommonCode.UiTools.RedirectToSameUrl(Request.Url.ToString());
                }

                if (boolMaxWidth == true)
                {
                    lblMaxWidth.Text = "100%";
                }
                else
                {
                    lblMaxWidth.Text = string.Format("{0}", maxWidth);
                }

                lblMinWidth.Text = string.Format("{0}", minWidth);

            }
            else
            {
                lblMaxWidth.Text = "1200";
                lblMinWidth.Text = "1000";
            }
        }

        protected void btnResetOptions_Click(object sender, EventArgs e)
        {
            CommonCode.UITheme.ResetSettings(Session, Response, Request);
            CommonCode.UiTools.RedirectToSameUrl(Request.Url.ToString());
        }

        protected void lbWidthOpt1_Click(object sender, EventArgs e)
        {
            lblOptionError.Visible = false;

            int minWidth = 1000;
            int maxWidth = 1000;

            string error = string.Empty;

            if (CommonCode.UITheme.SaveSettings(Session, Response, Request, minWidth, maxWidth, false, out error))
            {
                UpdateClientTheme(true);
            }
        }

        protected void lbWidthOpt2_Click(object sender, EventArgs e)
        {
            lblOptionError.Visible = false;

            int minWidth = 1000;
            int maxWidth = 1500;

            string error = string.Empty;

            if (CommonCode.UITheme.SaveSettings(Session, Response, Request, minWidth, maxWidth, false, out error))
            {
                UpdateClientTheme(true);
            }
        }

        protected void lbWidthOpt3_Click(object sender, EventArgs e)
        {
            lblOptionError.Visible = false;

            int minWidth = 1000;
            int maxWidth = 1;

            string error = string.Empty;

            if (CommonCode.UITheme.SaveSettings(Session, Response, Request, minWidth, maxWidth, true, out error))
            {
                UpdateClientTheme(true);
            }
        }


    }
}
