﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using DataAccess;
using BusinessLayer;
using System.Text;

namespace UserInterface.Logs
{
    public partial class Logs : BasePage
    {
        private User CurrentUser = null;

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
            CheckParams();                  // checks user and shows options depenening on him
            CheckShowHideButtons();
            ShowInfo();                     // shows page info
        }

        private void ShowInfo()
        {
            Title = "Logs Page";

            BusinessSiteText siteText = new BusinessSiteText();

            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutLogs");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblAbout.Text = aboutExtended.description;
            }
            else
            {
                lblAbout.Text = "About Logs text not typed.";
            }

        }

        private void CheckParams()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);

            if (currUser != null)
            {

                if (businessUser.IsFromAdminTeam(currUser))
                {
                    CurrentUser = currUser;

                    if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditGlobalAdministrators))
                    {
                        pnlAdmin.Visible = true;
                        pnlGLobal.Visible = true;

                    }
                    else if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditModerators))
                    {
                        pnlGLobal.Visible = false;
                        pnlAdmin.Visible = true;
                    }
                    else
                    {
                        pnlAdmin.Visible = false;
                        pnlGLobal.Visible = false;
                    }

                    if (IsPostBack == false)
                    {
                        FillDdlLogsOptions(businessUser, currUser);
                        FillDDlLogsTypes(ddlLogsType);

                        FillDDlLogsTypes(ddlLogsType2);
                        FillDdlLogsOptions2(businessUser, currUser);

                        FillDDlLogsTypes(ddlLogsType3);
                    }

                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, "This page is only for Administrators.");
                }

            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, "This page is only for Administrators.");
            }

        }

        private void FillDdlLogsOptions(BusinessUser businessUser, User currUser)
        {
            ListItem newItem = new ListItem();
            newItem.Text = "user";
            newItem.Value = "user";
            ddLogsOptions.Items.Add(newItem);

            ListItem usrsItem = new ListItem();
            usrsItem.Text = "user`s roles";
            usrsItem.Value = "usersRoles";
            ddLogsOptions.Items.Add(usrsItem);

            ListItem usrsTransfersItem = new ListItem();
            usrsTransfersItem.Text = "user`s transfers";
            usrsTransfersItem.Value = "userTransfers";
            ddLogsOptions.Items.Add(usrsTransfersItem);

            ListItem usrsActTaking = new ListItem();
            usrsActTaking.Text = "user`s role takings";
            usrsActTaking.Value = "roleTakings";
            ddLogsOptions.Items.Add(usrsActTaking);

            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditCategories))
            {

                ListItem catItem = new ListItem();
                catItem.Text = "category";
                catItem.Value = "category";
                ddLogsOptions.Items.Add(catItem);

            }


            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditCompanies))
            {

                ListItem compItem = new ListItem();
                compItem.Text = "company";
                compItem.Value = "company";
                ddLogsOptions.Items.Add(compItem);

                ListItem cCharItem = new ListItem();
                cCharItem.Text = "company characteristic";
                cCharItem.Value = "compCharacteristic";
                ddLogsOptions.Items.Add(cCharItem);

                ListItem compsCharsItem = new ListItem();
                compsCharsItem.Text = "company`s characteristics";
                compsCharsItem.Value = "compsCharacteristics";
                ddLogsOptions.Items.Add(compsCharsItem);

                ListItem compCats = new ListItem();
                compCats.Text = "company`s categories";
                compCats.Value = "compsCategories";
                ddLogsOptions.Items.Add(compCats);

                ListItem ccItem = new ListItem();
                ccItem.Text = "company category";
                ccItem.Value = "compCategory";
                ddLogsOptions.Items.Add(ccItem);

                ListItem caltItem = new ListItem();
                caltItem.Text = "company alternative name";
                caltItem.Value = "altCompanyName";
                ddLogsOptions.Items.Add(caltItem);

                ListItem caltsItems = new ListItem();
                caltsItems.Text = "company`s alternative names";
                caltsItems.Value = "compsAlternativeNames";
                ddLogsOptions.Items.Add(caltsItems);

            }


            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditProducts))
            {

                ListItem prodItem = new ListItem();
                prodItem.Text = "product";
                prodItem.Value = "product";
                ddLogsOptions.Items.Add(prodItem);

                ListItem pcItem = new ListItem();
                pcItem.Text = "product characteristic";
                pcItem.Value = "prodCharacteristic";
                ddLogsOptions.Items.Add(pcItem);

                ListItem prodsCharsItem = new ListItem();
                prodsCharsItem.Text = "product`s characteristics";
                prodsCharsItem.Value = "prodsCharacteristics";
                ddLogsOptions.Items.Add(prodsCharsItem);

                ListItem prodVariant = new ListItem();
                prodVariant.Text = "product variant";
                prodVariant.Value = "productVariant";
                ddLogsOptions.Items.Add(prodVariant);

                ListItem prodSubVariant = new ListItem();
                prodSubVariant.Text = "product sub variant";
                prodSubVariant.Value = "productSubVariant";
                ddLogsOptions.Items.Add(prodSubVariant);

                ListItem prodVariants = new ListItem();
                prodVariants.Text = "product`s variants";
                prodVariants.Value = "prodsVariants";
                ddLogsOptions.Items.Add(prodVariants);

                ListItem prodAltItem = new ListItem();
                prodAltItem.Text = "product alternative name";
                prodAltItem.Value = "altProductyName";
                ddLogsOptions.Items.Add(prodAltItem);

                ListItem caltsItems = new ListItem();
                caltsItems.Text = "product`s alternative names";
                caltsItems.Value = "prodsAlternativeNames";
                ddLogsOptions.Items.Add(caltsItems);

                ListItem prodlink = new ListItem();
                prodlink.Text = "product link";
                prodlink.Value = "productLink";
                ddLogsOptions.Items.Add(prodlink);

                ListItem prodslink = new ListItem();
                prodslink.Text = "product's links";
                prodslink.Value = "productsLink";
                ddLogsOptions.Items.Add(prodslink);

            }

            if (businessUser.IsAdminOrGlobalAdmin(currUser))
            {
                ListItem newsItem = new ListItem();
                newsItem.Text = "site text";
                newsItem.Value = "siteText";
                ddLogsOptions.Items.Add(newsItem);

                ListItem advertItem = new ListItem();
                advertItem.Text = "advertisement";
                advertItem.Value = "advertisement";
                ddLogsOptions.Items.Add(advertItem);

                ListItem ipBan = new ListItem();
                ipBan.Text = "ip ban";
                ipBan.Value = "ipBan";
                ddLogsOptions.Items.Add(ipBan);
            }

            if (businessUser.IsGlobalAdministrator(currUser))
            {
                ListItem compTypesItem = new ListItem();
                compTypesItem.Text = string.Format("{0} type", Configuration.CompanyName);
                compTypesItem.Value = "companyType";
                ddLogsOptions.Items.Add(compTypesItem);
            }

            ListItem topicItem = new ListItem();
            topicItem.Text = "Topic";
            topicItem.Value = "productTopic";
            ddLogsOptions.Items.Add(topicItem);

            ListItem repItem = new ListItem();
            repItem.Text = "report";
            repItem.Value = "report";
            ddLogsOptions.Items.Add(repItem);

            ListItem suggItem = new ListItem();
            suggItem.Text = "suggestion";
            suggItem.Value = "suggestion";
            ddLogsOptions.Items.Add(suggItem);

            ListItem notifItem = new ListItem();
            notifItem.Text = "notify";
            notifItem.Value = "notify";
            ddLogsOptions.Items.Add(notifItem);

            ListItem editSuggestion = new ListItem();
            editSuggestion.Text = "edit suggestion";
            editSuggestion.Value = "typeSuggestion";
            ddLogsOptions.Items.Add(editSuggestion);
        }


        private void FillDdlLogsOptions2(BusinessUser businessUser, User currUser)
        {
            if (businessUser.IsAdminOrGlobalAdmin(currUser))
            {
                ListItem allItem = new ListItem();
                allItem.Text = "all";
                allItem.Value = "all";
                ddlLogsOptions2.Items.Add(allItem);
            }

            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditProducts))
            {
                ListItem prodItem = new ListItem();
                prodItem.Text = "products";
                prodItem.Value = "aProducts";
                ddlLogsOptions2.Items.Add(prodItem);

                ListItem prodChItem = new ListItem();
                prodChItem.Text = "product characteristics";
                prodChItem.Value = "aProdCharacteristics";
                ddlLogsOptions2.Items.Add(prodChItem);

                ListItem prodImgItem = new ListItem();
                prodImgItem.Text = "product images";
                prodImgItem.Value = "aProdImages";
                ddlLogsOptions2.Items.Add(prodImgItem);

                ListItem prodVariants = new ListItem();
                prodVariants.Text = "product variants";
                prodVariants.Value = "aProductVariants";
                ddlLogsOptions2.Items.Add(prodVariants);

                ListItem prodSubVariants = new ListItem();
                prodSubVariants.Text = "product sub variants";
                prodSubVariants.Value = "aProductSubVariants";
                ddlLogsOptions2.Items.Add(prodSubVariants);

                ListItem prodAltNames = new ListItem();
                prodAltNames.Text = "product alternative names";
                prodAltNames.Value = "prodsAlternativeNames";
                ddlLogsOptions2.Items.Add(prodAltNames);

                ListItem prodlink = new ListItem();
                prodlink.Text = "product links";
                prodlink.Value = "aProductLinks";
                ddlLogsOptions2.Items.Add(prodlink);

            }

            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditCompanies))
            {

                ListItem compItem = new ListItem();
                compItem.Text = "companies";
                compItem.Value = "aCompanies";
                ddlLogsOptions2.Items.Add(compItem);

                ListItem compChItem = new ListItem();
                compChItem.Text = "company characteristics";
                compChItem.Value = "aCompCharacteristics";
                ddlLogsOptions2.Items.Add(compChItem);

                ListItem compCtItem = new ListItem();
                compCtItem.Text = "company categories";
                compCtItem.Value = "aCompCategories";
                ddlLogsOptions2.Items.Add(compCtItem);

                ListItem compImgItem = new ListItem();
                compImgItem.Text = "company images";
                compImgItem.Value = "aCompImages";
                ddlLogsOptions2.Items.Add(compImgItem);

                ListItem compAltNames = new ListItem();
                compAltNames.Text = "company alternative names";
                compAltNames.Value = "compsAlternativeNames";
                ddlLogsOptions2.Items.Add(compAltNames);
            }

            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditCategories))
            {
                ListItem catItem = new ListItem();
                catItem.Text = "categories";
                catItem.Value = "aCategories";
                ddlLogsOptions2.Items.Add(catItem);
            }

            if (businessUser.IsAdminOrGlobalAdmin(currUser))
            {
                ListItem newsItem = new ListItem();
                newsItem.Text = "site texts";
                newsItem.Value = "aSiteTexts";
                ddlLogsOptions2.Items.Add(newsItem);

                ListItem advertItem = new ListItem();
                advertItem.Text = "advertisements";
                advertItem.Value = "aAdvertisements";
                ddlLogsOptions2.Items.Add(advertItem);

                ListItem ipBan = new ListItem();
                ipBan.Text = "ip bans";
                ipBan.Value = "aIpBans";
                ddlLogsOptions2.Items.Add(ipBan);
            }

            if (businessUser.IsGlobalAdministrator(currUser))
            {
                ListItem compTypesItem = new ListItem();
                compTypesItem.Text = string.Format("{0} types", Configuration.CompanyName);
                compTypesItem.Value = "aCompTypes";
                ddlLogsOptions2.Items.Add(compTypesItem);
            }

            ListItem userItem = new ListItem();
            userItem.Text = "users";
            userItem.Value = "aUsers";
            ddlLogsOptions2.Items.Add(userItem);

            ListItem roleItem = new ListItem();
            roleItem.Text = "user roles";
            roleItem.Value = "aUsrRoles";
            ddlLogsOptions2.Items.Add(roleItem);

            ListItem usrsTransfersItem = new ListItem();
            usrsTransfersItem.Text = "user transfers";
            usrsTransfersItem.Value = "aUsrTransfers";
            ddlLogsOptions2.Items.Add(usrsTransfersItem);

            ListItem usrsActionTakings = new ListItem();
            usrsActionTakings.Text = "users action takings";
            usrsActionTakings.Value = "aRoleTakings";
            ddlLogsOptions2.Items.Add(usrsActionTakings);

            ListItem topicItem = new ListItem();
            topicItem.Text = "Topics";
            topicItem.Value = "aProductTopics";
            ddlLogsOptions2.Items.Add(topicItem);

            ListItem reportItem = new ListItem();
            reportItem.Text = "reports";
            reportItem.Value = "aReports";
            ddlLogsOptions2.Items.Add(reportItem);

            ListItem suggItem = new ListItem();
            suggItem.Text = "suggestions";
            suggItem.Value = "aSuggestions";
            ddlLogsOptions2.Items.Add(suggItem);

            ListItem commItem = new ListItem();
            commItem.Text = "comments";
            commItem.Value = "aComments";
            ddlLogsOptions2.Items.Add(commItem);

            ListItem rateCommItem = new ListItem();
            rateCommItem.Text = "rate comments";
            rateCommItem.Value = "aRateComments";
            ddlLogsOptions2.Items.Add(rateCommItem);

            ListItem rateProdItem = new ListItem();
            rateProdItem.Text = "rate products";
            rateProdItem.Value = "aRateProducts";
            ddlLogsOptions2.Items.Add(rateProdItem);

            ListItem notifsItem = new ListItem();
            notifsItem.Text = "notifies";
            notifsItem.Value = "aNotifies";
            ddlLogsOptions2.Items.Add(notifsItem);

            ListItem editSuggestions = new ListItem();
            editSuggestions.Text = "edit suggestions";
            editSuggestions.Value = "aTypeSuggestions";
            ddlLogsOptions2.Items.Add(editSuggestions);

            ddlLogsOptions2.SelectedIndex = 0;

        }

        private void FillDDlLogsTypes(DropDownList ddlLogs)
        {
            ListItem allItem = new ListItem();
            allItem.Text = "all";
            allItem.Value = "all";
            ddlLogs.Items.Add(allItem);

            ListItem createItem = new ListItem();
            createItem.Text = "create";
            createItem.Value = "create";
            ddlLogs.Items.Add(createItem);

            ListItem editItem = new ListItem();
            editItem.Text = "edit";
            editItem.Value = "edit";
            ddlLogs.Items.Add(editItem);

            ListItem deleteItem = new ListItem();
            deleteItem.Text = "deleted";
            deleteItem.Value = "setDeleted";
            ddlLogs.Items.Add(deleteItem);

            ListItem undoItem = new ListItem();
            undoItem.Text = "undeleted";
            undoItem.Value = "unSetDeleted";
            ddlLogs.Items.Add(undoItem);

            ddlLogs.SelectedIndex = 0;
        }


        private void CheckShowHideButtons()
        {

            if (btnShowRegAdmins.Text == "Hide")
            {
                BindDataWithTable(tblRegAdmins, "registered", "global");
                tblRegAdmins.Visible = true;
            }
            if (btnShowDelAdmins.Text == "Hide")
            {
                BindDataWithTable(tblDelAdmins, "deleted", "global");
                tblDelAdmins.Visible = true;
            }
            if (btnShowRegModer.Text == "Hide")
            {
                BindDataWithTable(tblRegModer, "registered", "admin");
                tblRegModer.Visible = true;
            }

            if (btnShowDelModer.Text == "Hide")
            {
                BindDataWithTable(tblDelModer, "deleted", "admin");
                tblDelModer.Visible = true;
            }
            if (btnShowRegUsers.Text == "Hide")
            {
                BindDataWithTable(tblShowAddedUsers, "registered", "user");
                tblShowAddedUsers.Visible = true;
            } 

            if (btnShowDeletedCategories.Text == "Hide")
            {
                ShowDeletedCategories();
                tblShowAddedUsers.Visible = true;
            }
        }

        private void ShowDeletedCategories()
        {
            tblDeletedCategories.Rows.Clear();

            BusinessCategory businessCategory = new BusinessCategory();
            List<Category> deletedCategories = businessCategory.GetAllDeleted(objectContext).ToList();

            TableRow firstRow = new TableRow();
            tblDeletedCategories.Rows.Add(firstRow);

            TableCell idCell = new TableCell();
            firstRow.Cells.Add(idCell);
            idCell.Text = "id";

            TableCell catCell = new TableCell();
            firstRow.Cells.Add(catCell);
            catCell.Text = "category";

            TableCell lastCell = new TableCell();
            firstRow.Cells.Add(lastCell);
            lastCell.Text = "last";

            TableCell parCell = new TableCell();
            firstRow.Cells.Add(parCell);
            parCell.Text = "parent";

            if (deletedCategories.Count > 0)
            {
                BusinessUser bUser = new BusinessUser();

                foreach (Category category in deletedCategories)
                {
                    TableRow tRow = new TableRow();
                    tblDeletedCategories.Rows.Add(tRow);

                    TableCell tidCell = new TableCell();
                    tRow.Cells.Add(tidCell);
                    tidCell.Text = category.ID.ToString();

                    TableCell tcatCell = new TableCell();
                    tRow.Cells.Add(tcatCell);
                    tcatCell.Controls.Add(CommonCode.UiTools.GetCategoryHyperLink(category));

                    TableCell tlastCell = new TableCell();
                    tRow.Cells.Add(tlastCell);
                    tlastCell.Text = category.last.ToString();

                    TableCell tparCell = new TableCell();
                    tRow.Cells.Add(tparCell);
                    if (category.parentID != null)
                    {
                        tparCell.Controls.Add(CommonCode.UiTools.GetCategoryHyperLink(objectContext, category.parentID.Value));
                    }
                    else
                    {
                        tparCell.Text = "none";
                    }
                }
            }
            else
            {
                GetNoDataRow(4, "No deleted categories");
            }

        }

        protected void btnShowRegAdmins_Click(object sender, EventArgs e)
        {
            if (btnShowRegAdmins.Text == "Show")
            {
                btnShowRegAdmins.Text = "Hide";
                BindDataWithTable(tblRegAdmins, "registered", "global");
                tblRegAdmins.Visible = true;
            }
            else
            {
                btnShowRegAdmins.Text = "Show";
                tblRegAdmins.Visible = false;
            }
        }

        protected void btnShowDelAdmins_Click(object sender, EventArgs e)
        {
            if (btnShowDelAdmins.Text == "Show")
            {
                btnShowDelAdmins.Text = "Hide";
                BindDataWithTable(tblDelAdmins, "deleted", "global");
                tblDelAdmins.Visible = true;
            }
            else
            {
                btnShowDelAdmins.Text = "Show";
                tblDelAdmins.Visible = false;
            }
        }

        protected void btnShowRegModer_Click(object sender, EventArgs e)
        {
            if (btnShowRegModer.Text == "Show")
            {
                btnShowRegModer.Text = "Hide";
                BindDataWithTable(tblRegModer, "registered", "admin");
                tblRegModer.Visible = true;
            }
            else
            {
                btnShowRegModer.Text = "Show";
                tblRegModer.Visible = false;
            }
        }

        protected void btnShowDelModer_Click(object sender, EventArgs e)
        {
            if (btnShowDelModer.Text == "Show")
            {
                btnShowDelModer.Text = "Hide";
                BindDataWithTable(tblDelModer, "deleted", "admin");
                tblDelModer.Visible = true;
            }
            else
            {
                btnShowDelModer.Text = "Show";
                tblDelModer.Visible = false;
            }
        }

        protected void btnShowRegUsers_Click(object sender, EventArgs e)
        {
            if (btnShowRegUsers.Text == "Show")
            {
                btnShowRegUsers.Text = "Hide";
                BindDataWithTable(tblShowAddedUsers, "registered", "user");
                tblShowAddedUsers.Visible = true;
            }
            else
            {
                btnShowRegUsers.Text = "Show";
                tblShowAddedUsers.Visible = false;
            }
        }


        private void BindDataWithTable(Table currTable, string type, string typeUser)
        {
            currTable.Rows.Clear();

            if (!string.IsNullOrEmpty(type))
            {
                if (type != "registered" && type != "deleted")
                {
                    throw new CommonCode.UIException(string.Format("type = {0} is not supported type , user id = {1}",type,CurrentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("type is null or empty , user id = {0}",CurrentUser.ID));
            }


            if (!string.IsNullOrEmpty(typeUser))
            {
                if (typeUser != "global" && typeUser != "admin" && typeUser != "user")
                {
                    throw new CommonCode.UIException(string.Format("typeUser = {0} is not supported , user id = {1}",typeUser,CurrentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("typeUser is null or empty");
            }


            TableRow firstRow = new TableRow();

            TableCell idFCell = new TableCell();
            idFCell.Width = Unit.Pixel(10);
            idFCell.Text = "ID";
            firstRow.Cells.Add(idFCell);

            TableCell nameFCell = new TableCell();
            nameFCell.Text = "username";
            firstRow.Cells.Add(nameFCell);


            if (type == "registered")
            {
                TableCell dateFCell = new TableCell();
                dateFCell.Text = "date registered";
                firstRow.Cells.Add(dateFCell);

                if (typeUser != "user")
                {
                    TableCell regbyFCell = new TableCell();
                    regbyFCell.Text = "registered by";
                    firstRow.Cells.Add(regbyFCell);
                }
            }
            else
            {
                TableCell dateFCell = new TableCell();
                dateFCell.Text = "date deleted";
                firstRow.Cells.Add(dateFCell);

                TableCell delbyFCell = new TableCell();
                delbyFCell.Text = "deleted by";
                firstRow.Cells.Add(delbyFCell);

            }

            if (typeUser != "admin")
            {
                TableCell typeFCell = new TableCell();
                typeFCell.Text = "type";
                firstRow.Cells.Add(typeFCell);
            }

            currTable.Rows.Add(firstRow);

            BusinessUser businessUser = new BusinessUser();
            IEnumerable<User> users = null;

            if (typeUser == "global" && type == "registered")
            {
                users = businessUser.GetGlobalsAndAdministrators(userContext);
            }
            if (typeUser == "global" && type == "deleted")
            {
                users = businessUser.GetDeletedGlobalsAndAdministrators(userContext);
            }

            if (typeUser == "admin" && type == "registered")
            {
                users = businessUser.GetModerators(userContext);
            }
            if (typeUser == "admin" && type == "deleted")
            {
                users = businessUser.GetDeletedModerators(userContext);
            }

            if (typeUser == "user" && type == "registered")
            {
                users = businessUser.GetVisibleUsersAndWriters(userContext);
            }
            if (typeUser == "user" && type == "deleted")
            {
                users = businessUser.GetDeletedUsersAndWriters(userContext);
            }

            if (users.Count<User>() > 0)
            {
                foreach (User user in users)
                {
                    AddUserRow(currTable, type, typeUser, businessUser, user);
                }
            }
            else
            {

                TableRow secRow = new TableRow();
                TableCell secCell = new TableCell();
                secCell.ColumnSpan = firstRow.Cells.Count;
                secCell.Text = "no data";
                secRow.Cells.Add(secCell);
                currTable.Rows.Add(secRow);
            }

        }

        private void AddUserRow(Table currTable, string type, string typeUser, BusinessUser businessUser, User user)
        {
            TableRow newRow = new TableRow();

            TableCell idCell = new TableCell();
            idCell.Text = user.ID.ToString();
            newRow.Cells.Add(idCell);

            newRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(user));

            if (type == "registered")
            {
                TableCell dateCell = new TableCell();
                dateCell.Text = CommonCode.UiTools.DateTimeToLocalString(user.dateCreated);
                newRow.Cells.Add(dateCell);

                if (typeUser != "user")
                {
                    TableCell regCell = new TableCell();
                    User regBy = businessUser.Get(userContext, user.createdBy, true);
                    
                    if (businessUser.IsUserValidType(regBy))
                    {
                       
                        regCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink(regBy));
                    }
                    else
                    {
                        regCell.Text = regBy.username;
                   }
                    
                    newRow.Cells.Add(regCell);
                }
            }
            else
            {

                Log userLog = businessLog.getDeletedUser(objectContext, user.ID);
                if (userLog == null)
                {
                    TableCell delCell = new TableCell();
                    delCell.Text = "no data";
                    newRow.Cells.Add(delCell);

                    if (typeUser != "user")
                    {
                        TableCell delbyCell = new TableCell();
                        delbyCell.Text = "no data";
                        newRow.Cells.Add(delbyCell);
                    }
                }
                else
                {
                    TableCell delCell = new TableCell();
                    delCell.Text = CommonCode.UiTools.DateTimeToLocalString(userLog.dateCreated);
                    newRow.Cells.Add(delCell);


                    TableCell delbyCell = new TableCell();
                    if (!userLog.UserIDReference.IsLoaded)
                    {
                        userLog.UserIDReference.Load();
                    }
                    User delBy = businessUser.Get(userContext, userLog.UserID.ID, true);
                   
                    if (businessUser.IsUserValidType(delBy))
                    {
                        LinkButton delbyBtn = new LinkButton();
                        delbyBtn.Text = delBy.username;
                        delbyBtn.PostBackUrl = GetUrlWithVariant("Profile.aspx?User=" + delBy.ID);
                        delbyCell.Controls.Add(delbyBtn);
                    }
                    else
                    {
                        delbyCell.Text = delBy.username;
                    }
                    
                    newRow.Cells.Add(delbyCell);

                }

            }

            if (typeUser != "admin")
            {

                TableCell typeCell = new TableCell();
                typeCell.Text = user.type;
                newRow.Cells.Add(typeCell);
            }
            currTable.Rows.Add(newRow);
        }

        private TableRow GetNoDataRow(int columnspam, string message)
        {
            if (columnspam < 1)
            {
                throw new CommonCode.UIException(string.Format("columnspam is < 1 , user id = {0}",CurrentUser.ID));
            }

            TableRow lastRow = new TableRow();
            TableCell lastCell = new TableCell();
            lastCell.ColumnSpan = columnspam;
            if (string.IsNullOrEmpty(message))
            {
                lastCell.Text = "no data";
            }
            else
            {
                lastCell.Text = message;
            }
            
            lastRow.Cells.Add(lastCell);

            return lastRow;
        }

        protected void btnGetLogs_Click(object sender, EventArgs e)
        {
            phGetLogsByID.Visible = true;
            phGetLogsByID.Controls.Add(lblError);
            string error = "";

            if (CommonCode.Validate.ValidateLong(tbID.Text, out error))
            {
                long Id = -1;
                if (long.TryParse(tbID.Text, out Id))
                {
                    if (Id > 0)
                    {
                        int num = -1;
                        if (int.TryParse(tbNumLogs.Text, out num))
                        {
                            if (num > 0)
                            {

                                String logType = ddlLogsType.SelectedValue;
                                String aboutType = ddLogsOptions.SelectedValue;

                                phGetLogsByID.Visible = false;

                                if (aboutType == "prodsCharacteristics" || aboutType == "compsCharacteristics" 
                                    || aboutType == "compsCategories" || aboutType == "prodsVariants"
                                    || aboutType == "compsAlternativeNames" || aboutType == "prodsAlternativeNames"
                                    || aboutType == "productsLink")
                                {
                                    FillSpecificLogsTable(num, logType, aboutType, Id);
                                }
                                else
                                {
                                    FillLogTable(businessLog.GetLogs(objectContext, logType, aboutType, Id, num, 0));
                                }

                            }
                            else
                            {
                                error = "Number must be bigger than 0.";
                            }
                        }
                        else
                        {
                            error = "Type number";
                        }
                    }
                    else
                    {
                        error = "Id must be positive.";
                    }
                }
                else
                {
                    throw new CommonCode.UIException(string.Format("Couldnt Parse tbID.Text to long..this should " +
                        "happen as there is Validate check before parsing , user id = {0}",CurrentUser.ID));
                }
            }

            lblError.Text = error;

        }

        protected void btnGetLogs2_Click(object sender, EventArgs e)
        {
            phGetLogs.Visible = true;
            phGetLogs.Controls.Add(lblError);

            int num = -1;
            if (int.TryParse(tbNumLogs2.Text, out num))
            {
                if (num > 0)
                {
                    String logType = ddlLogsType2.SelectedValue;
                    String aboutType = ddlLogsOptions2.SelectedValue;

                    phGetLogs.Visible = false;
                    FillLogTable(businessLog.GetLogs(objectContext, logType, aboutType, 1, num, 0));

                }
                else
                {
                    lblError.Text = "Number must be bigger than 0.";
                }
            }
            else
            {
                lblError.Text = "Type number";
            }
        }

        protected void btnGetLogs3_Click(object sender, EventArgs e)
        {
            phGetLogsFrom.Visible = true;
            phGetLogsFrom.Controls.Add(lblError);

            int num = -1;
            if (int.TryParse(tbNumLogs3.Text, out num))
            {
                if (num > 0)
                {
                    long userId = -1;
                    if (long.TryParse(tbLogsFromUser.Text, out userId))
                    {
                        if (userId > 0)
                        {
                            String logType = ddlLogsType3.SelectedValue;
                            String aboutType = "all";

                            phGetLogsFrom.Visible = false;
                            FillLogTable(businessLog.GetLogs(objectContext, logType, aboutType, 1, num, userId));

                        }
                        else
                        {
                            lblError.Text = "User id must be positive.";
                        }
                       
                    }
                    else
                    {
                        lblError.Text = "Type user ID";
                    }
                }
                else
                {
                    lblError.Text = "Number must be positive.";
                }
            }
            else
            {
                lblError.Text = "Type number";
            }
        }

        public void FillSpecificLogsTable(int number, string logType, string aboutType, long ID)
        {
            tblSpecLogs.Rows.Clear();

            tblGetLogs.Visible = false;
            tblSpecLogs.Visible = true;

            switch (aboutType)
            {
                case ("compsCharacteristics"):

                    FillSLCompCharacteristics(number, logType, ID);
                    
                    break;
                case("compsCategories"):

                    FillSLCompCategories(number, logType, ID);

                    break;
                case ("prodsCharacteristics"):

                    FillSLProdChars(number, logType, ID);

                    break;
                case "prodsVariants":

                    FillSLVariants(number, logType, ID);

                    break;
                case ("prodsAlternativeNames"):

                    FillSLProdAlternativeNames(number, logType, ID);

                    break;
                case "compsAlternativeNames":

                    FillSLCompAlternativeNames(number, logType, ID);

                    break;
                case "productsLink":

                    FillSLProductLinks(number, logType, ID);

                    break;
                    /////
                default:
                    throw new CommonCode.UIException(string.Format("aboutType = '{0}' is not supported type , user ID = {1}", aboutType, CurrentUser.ID));
            }
        }

        private void FillSLCompAlternativeNames(int number, string logType, long ID)
        {
            BusinessCompany businessCompany = new BusinessCompany();
            BusinessAlternativeNames bAN = new BusinessAlternativeNames();

            Company currComp = businessCompany.GetCompanyWV(objectContext, ID);

            IEnumerable<Log> Logs = null;
            int logsCount = 0;
            string message = "";

            if (currComp == null)
            {
                message = "There`s no such company.";
                TableRow lastRow = GetNoDataRow(1, message);
                tblSpecLogs.Rows.Add(lastRow);
                lastRow.Cells[0].CssClass = null;
                lastRow.Cells[0].Text = message;
            }
            else
            {
                List<AlternativeCompanyName> altCompNames = bAN.GetAllAlternativeCompanyNames(objectContext, currComp);
                if (altCompNames != null && altCompNames.Count > 0)
                {
                    foreach (AlternativeCompanyName altName in altCompNames)
                    {
                        Logs = businessLog.GetLogs(objectContext, logType, "altCompanyName", altName.ID, number, 0);
                        if (Logs.Count<Log>() > 0)
                        {
                            TableRow nameRow = new TableRow();
                            tblSpecLogs.Rows.Add(nameRow);

                            TableCell nameCell = new TableCell();
                            nameRow.Cells.Add(nameCell);
                            nameCell.ColumnSpan = 5;
                            nameCell.CssClass = "textHeaderWA";
                            nameCell.HorizontalAlign = HorizontalAlign.Center;
                            nameCell.Text = string.Format("{0} , ID: {1}", altName.name, altName.ID);

                            foreach (Log log in Logs)
                            {
                                logsCount++;

                                if (!log.UserIDReference.IsLoaded)
                                {
                                    log.UserIDReference.Load();
                                }

                                TableRow logRow = new TableRow();
                                tblSpecLogs.Rows.Add(logRow);

                                TableCell typeCell = new TableCell();
                                logRow.Cells.Add(typeCell);
                                typeCell.Text = log.type;
                                typeCell.ColumnSpan = 0;
                                typeCell.CssClass = string.Empty;

                                TableCell byCell = CommonCode.UiTools.GetUserTableCell(userContext, log.UserID.ID);
                                logRow.Cells.Add(byCell);

                                TableCell IpCell = new TableCell();
                                logRow.Cells.Add(IpCell);
                                IpCell.Text = log.userIPadress;

                                TableCell dateCell = new TableCell();
                                logRow.Cells.Add(dateCell);
                                dateCell.Text = CommonCode.UiTools.DateTimeToLocalString("d/M/yyyy h:m:s.fff", log.dateCreated);

                                TableCell descrCell = new TableCell();
                                logRow.Cells.Add(descrCell);
                                descrCell.Text = Tools.GetFormattedTextFromDB(log.description);
                            }

                        }
                    }

                    if (logsCount == 0)
                    {
                        message = "There are no logs for this company`s alternative names.";
                        TableRow lastRow = GetNoDataRow(1, message);
                        tblSpecLogs.Rows.Add(lastRow);
                        lastRow.Cells[0].CssClass = null;
                        lastRow.Cells[0].Text = message;
                    }
                }
                else
                {
                    message = "No logs in database.";
                    TableRow lastRow = GetNoDataRow(1, message);
                    tblSpecLogs.Rows.Add(lastRow);
                    lastRow.Cells[0].CssClass = null;
                    lastRow.Cells[0].Text = message;
                }
            }
        }

        private void FillSLProductLinks(int number, string logType, long ID)
        {
            BusinessProduct businessProduct = new BusinessProduct();
            BusinessProductLink bpLinks = new BusinessProductLink();

            Product currProduct = businessProduct.GetProductByIDWV(objectContext, ID);

            IEnumerable<Log> Logs = null;
            int logsCount = 0;
            string message = "";

            if (currProduct == null)
            {
                message = "No such product";
                TableRow lastRow = GetNoDataRow(1, message);
                tblSpecLogs.Rows.Add(lastRow);
                lastRow.Cells[0].CssClass = null;
                lastRow.Cells[0].Text = message;
            }
            else
            {
                List<ProductLink> prodLinks = bpLinks.GetProductLinks(objectContext, currProduct, false);
                if (prodLinks != null && prodLinks.Count > 0)
                {
                    foreach (ProductLink link in prodLinks)
                    {
                        Logs = businessLog.GetLogs(objectContext, logType, "productLink", link.ID, number, 0);
                        if (Logs.Count<Log>() > 0)
                        {
                            TableRow nameRow = new TableRow();
                            tblSpecLogs.Rows.Add(nameRow);

                            TableCell nameCell = new TableCell();
                            nameRow.Cells.Add(nameCell);
                            nameCell.ColumnSpan = 5;
                            nameCell.CssClass = "textHeaderWA";
                            nameCell.HorizontalAlign = HorizontalAlign.Center;
                            nameCell.Text = string.Format("{0} , ID: {1}", link.link, link.ID);

                            foreach (Log log in Logs)
                            {
                                logsCount++;

                                if (!log.UserIDReference.IsLoaded)
                                {
                                    log.UserIDReference.Load();
                                }

                                TableRow logRow = new TableRow();
                                tblSpecLogs.Rows.Add(logRow);

                                TableCell typeCell = new TableCell();
                                logRow.Cells.Add(typeCell);
                                typeCell.Text = log.type;
                                typeCell.ColumnSpan = 0;
                                typeCell.CssClass = string.Empty;

                                TableCell byCell = CommonCode.UiTools.GetUserTableCell(userContext, log.UserID.ID);
                                logRow.Cells.Add(byCell);

                                TableCell IpCell = new TableCell();
                                logRow.Cells.Add(IpCell);
                                IpCell.Text = log.userIPadress;

                                TableCell dateCell = new TableCell();
                                logRow.Cells.Add(dateCell);
                                dateCell.Text = CommonCode.UiTools.DateTimeToLocalString("d/M/yyyy h:m:s.fff", log.dateCreated);

                                TableCell descrCell = new TableCell();
                                logRow.Cells.Add(descrCell);
                                descrCell.Text = Tools.GetFormattedTextFromDB(log.description);
                            }

                        }
                    }
                    if (logsCount == 0)
                    {
                        message = "There are no logs for this product`s links.";
                        TableRow lastRow = GetNoDataRow(1, message);
                        tblSpecLogs.Rows.Add(lastRow);
                        lastRow.Cells[0].CssClass = null;
                        lastRow.Cells[0].Text = message;
                    }
                }
                else
                {
                    string infoMessage = "No logs in database.";
                    TableRow rowToAdd = GetNoDataRow(1, infoMessage);
                    if (rowToAdd.Cells.Count > 0)
                    {
                        string messageUsed = rowToAdd.Cells[0].Text;
                        if (string.Equals(infoMessage, messageUsed) != true)
                        {
                            throw new CommonCode.UIException(
                                string.Format("Wrong message used: expected \"{0}\", actual \"{1}\".",
                                infoMessage ?? string.Empty, messageUsed ?? string.Empty));
                        }

                        int addedAtIndex = tblSpecLogs.Rows.Add(rowToAdd);

                        string rowToAddMessage = rowToAdd.Cells[0].Text;
                        if (string.Equals(infoMessage, rowToAddMessage) != true)
                        {
                            //
                            // This is somehow related to the fact that tblSpecLogs.EnableViewState is true.
                            //

                            // Fix the cell text.
                            rowToAdd.Cells[0].Text = infoMessage;
                            rowToAdd.Cells[0].CssClass = null;

                            // Check again
                            string rowToAddMessageAfterFix = rowToAdd.Cells[0].Text;
                            if (string.Equals(infoMessage, rowToAddMessageAfterFix) != true)
                            {
                                throw new CommonCode.UIException(
                                    string.Format("Could not fix the message: expected \"{0}\", actual \"{1}\".",
                                    infoMessage ?? string.Empty, rowToAddMessageAfterFix ?? string.Empty));
                            }
                        }
                        string actualMessage = tblSpecLogs.Rows[addedAtIndex].Cells[0].Text;
                        if (string.Equals(infoMessage, actualMessage) != true)
                        {
                            throw new CommonCode.UIException(
                                string.Format("Wrong informational message: expected \"{0}\", actual \"{1}\".",
                                infoMessage ?? string.Empty, actualMessage ?? string.Empty));
                        }
                    }
                }
            }
        }

        private void FillSLProdAlternativeNames(int number, string logType, long ID)
        {
            BusinessProduct businessProduct = new BusinessProduct();
            BusinessAlternativeNames bAN = new BusinessAlternativeNames();

            Product currProduct = businessProduct.GetProductByIDWV(objectContext, ID);

            IEnumerable<Log> Logs = null;
            int logsCount = 0;
            string message = "";

            if (currProduct == null)
            {
                message = "No such product";
                TableRow lastRow = GetNoDataRow(1, message);
                tblSpecLogs.Rows.Add(lastRow);
                lastRow.Cells[0].CssClass = null;
                lastRow.Cells[0].Text = message;
            }
            else
            {
                List<AlternativeProductName> prodAltNames = bAN.GetAllAlternativeProductNames(objectContext, currProduct);
                if (prodAltNames != null && prodAltNames.Count > 0)
                {
                    foreach (AlternativeProductName altName in prodAltNames)
                    {
                        Logs = businessLog.GetLogs(objectContext, logType, "altProductyName", altName.ID, number, 0);
                        if (Logs.Count<Log>() > 0)
                        {
                            TableRow nameRow = new TableRow();
                            tblSpecLogs.Rows.Add(nameRow);

                            TableCell nameCell = new TableCell();
                            nameRow.Cells.Add(nameCell);
                            nameCell.ColumnSpan = 5;
                            nameCell.CssClass = "textHeaderWA";
                            nameCell.HorizontalAlign = HorizontalAlign.Center;
                            nameCell.Text = string.Format("{0} , ID: {1}", altName.name, altName.ID);

                            foreach (Log log in Logs)
                            {
                                logsCount++;

                                if (!log.UserIDReference.IsLoaded)
                                {
                                    log.UserIDReference.Load();
                                }

                                TableRow logRow = new TableRow();
                                tblSpecLogs.Rows.Add(logRow);

                                TableCell typeCell = new TableCell();
                                logRow.Cells.Add(typeCell);
                                typeCell.Text = log.type;
                                typeCell.ColumnSpan = 0;
                                typeCell.CssClass = string.Empty;

                                TableCell byCell = CommonCode.UiTools.GetUserTableCell(userContext, log.UserID.ID);
                                logRow.Cells.Add(byCell);

                                TableCell IpCell = new TableCell();
                                logRow.Cells.Add(IpCell);
                                IpCell.Text = log.userIPadress;

                                TableCell dateCell = new TableCell();
                                logRow.Cells.Add(dateCell);
                                dateCell.Text = CommonCode.UiTools.DateTimeToLocalString("d/M/yyyy h:m:s.fff", log.dateCreated);

                                TableCell descrCell = new TableCell();
                                logRow.Cells.Add(descrCell);
                                descrCell.Text = Tools.GetFormattedTextFromDB(log.description);
                            }

                        }
                    }
                    if (logsCount == 0)
                    {
                        message = "There are no logs for this product`s alternative names.";
                        TableRow lastRow = GetNoDataRow(1, message);
                        tblSpecLogs.Rows.Add(lastRow);
                        lastRow.Cells[0].CssClass = null;
                        lastRow.Cells[0].Text = message;
                    }
                }
                else
                {
                    string infoMessage = "No logs in database.";
                    TableRow rowToAdd = GetNoDataRow(1, infoMessage);
                    if (rowToAdd.Cells.Count > 0)
                    {
                        string messageUsed = rowToAdd.Cells[0].Text;
                        if (string.Equals(infoMessage, messageUsed) != true)
                        {
                            throw new CommonCode.UIException(
                                string.Format("Wrong message used: expected \"{0}\", actual \"{1}\".",
                                infoMessage ?? string.Empty, messageUsed ?? string.Empty));
                        }

                        int addedAtIndex = tblSpecLogs.Rows.Add(rowToAdd);

                        string rowToAddMessage = rowToAdd.Cells[0].Text;
                        if (string.Equals(infoMessage, rowToAddMessage) != true)
                        {
                            //
                            // This is somehow related to the fact that tblSpecLogs.EnableViewState is true.
                            //

                            // Fix the cell text.
                            rowToAdd.Cells[0].Text = infoMessage;
                            rowToAdd.Cells[0].CssClass = null;

                            // Check again
                            string rowToAddMessageAfterFix = rowToAdd.Cells[0].Text;
                            if (string.Equals(infoMessage, rowToAddMessageAfterFix) != true)
                            {
                                throw new CommonCode.UIException(
                                    string.Format("Could not fix the message: expected \"{0}\", actual \"{1}\".",
                                    infoMessage ?? string.Empty, rowToAddMessageAfterFix ?? string.Empty));
                            }
                        }
                        string actualMessage = tblSpecLogs.Rows[addedAtIndex].Cells[0].Text;
                        if (string.Equals(infoMessage, actualMessage) != true)
                        {
                            throw new CommonCode.UIException(
                                string.Format("Wrong informational message: expected \"{0}\", actual \"{1}\".",
                                infoMessage ?? string.Empty, actualMessage ?? string.Empty));
                        }
                    }
                }
            }
        }


        private void FillSLCompCharacteristics(int number, string logType, long ID)
        {
            BusinessCompany businessCompany = new BusinessCompany();
            Company currComp = businessCompany.GetCompanyWV(objectContext, ID);

            IEnumerable<Log> Logs = null;
            int logsCount = 0;
            string message = "";

            if (currComp == null)
            {
                message = "There`s no such company.";
                TableRow lastRow = GetNoDataRow(1, message);
                tblSpecLogs.Rows.Add(lastRow);
                lastRow.Cells[0].CssClass = null;
                lastRow.Cells[0].Text = message;
            }
            else
            {
                IEnumerable<CompanyCharacterestics> compChars = businessCompany.GetCompanyCharacterestics(objectContext, ID);
                if (compChars.Count<CompanyCharacterestics>() > 0)
                {
                    foreach (CompanyCharacterestics compChar in compChars)
                    {
                        Logs = businessLog.GetLogs(objectContext, logType, "compCharacteristic", compChar.ID, number, 0);
                        if (Logs.Count<Log>() > 0)
                        {
                            TableRow nameRow = new TableRow();
                            tblSpecLogs.Rows.Add(nameRow);

                            TableCell nameCell = new TableCell();
                            nameRow.Cells.Add(nameCell);
                            nameCell.ColumnSpan = 5;
                            nameCell.CssClass = "textHeaderWA";
                            nameCell.HorizontalAlign = HorizontalAlign.Center;
                            nameCell.Text = string.Format("{0} , ID: {1}", compChar.name, compChar.ID);

                            foreach (Log log in Logs)
                            {
                                logsCount++;

                                if (!log.UserIDReference.IsLoaded)
                                {
                                    log.UserIDReference.Load();
                                }

                                TableRow logRow = new TableRow();
                                tblSpecLogs.Rows.Add(logRow);

                                TableCell typeCell = new TableCell();
                                logRow.Cells.Add(typeCell);
                                typeCell.Text = log.type;
                                typeCell.ColumnSpan = 0;
                                typeCell.CssClass = string.Empty;

                                TableCell byCell = CommonCode.UiTools.GetUserTableCell(userContext, log.UserID.ID);
                                logRow.Cells.Add(byCell);

                                TableCell IpCell = new TableCell();
                                logRow.Cells.Add(IpCell);
                                IpCell.Text = log.userIPadress;

                                TableCell dateCell = new TableCell();
                                logRow.Cells.Add(dateCell);
                                dateCell.Text = CommonCode.UiTools.DateTimeToLocalString("d/M/yyyy h:m:s.fff", log.dateCreated);

                                TableCell descrCell = new TableCell();
                                logRow.Cells.Add(descrCell);
                                descrCell.Text = Tools.GetFormattedTextFromDB(log.description);
                            }

                        }
                    }

                    if (logsCount == 0)
                    {
                        message = "There are no logs for this company`s characteristic.";
                        TableRow lastRow = GetNoDataRow(1, message);
                        tblSpecLogs.Rows.Add(lastRow);
                        lastRow.Cells[0].CssClass = null;
                        lastRow.Cells[0].Text = message;
                    }
                }
                else
                {
                    message = "No logs in database.";
                    TableRow lastRow = GetNoDataRow(1, message);
                    tblSpecLogs.Rows.Add(lastRow);
                    lastRow.Cells[0].CssClass = null;
                    lastRow.Cells[0].Text = message;
                }
            }
        }

        private void FillSLCompCategories(int number, string logType, long ID)
        {
            BusinessCompany businessCompany2 = new BusinessCompany();
            Company currentComp = businessCompany2.GetCompanyWV(objectContext, ID);

            IEnumerable<Log> Logs = null;
            int logsCount = 0;
            string message = "";

            if (currentComp == null)
            {
                message = "There`s no such company.";
                TableRow lastRow = GetNoDataRow(1, message);
                tblSpecLogs.Rows.Add(lastRow);
                lastRow.Cells[0].CssClass = null;
                lastRow.Cells[0].Text = message;
            }
            else
            {
                IEnumerable<CategoryCompany> compCats = businessCompany2.GetCompanyCategoriesWithCompany(objectContext, ID);
                if (compCats.Count<CategoryCompany>() > 0)
                {
                    foreach (CategoryCompany compCat in compCats)
                    {
                        Logs = businessLog.GetLogs(objectContext, logType, "compCategory", compCat.ID, number, 0);
                        if (Logs.Count<Log>() > 0)
                        {

                            if (!compCat.CategoryReference.IsLoaded)
                            {
                                compCat.CategoryReference.Load();
                            }

                            TableRow nameRow = new TableRow();
                            tblSpecLogs.Rows.Add(nameRow);

                            TableCell nameCell = new TableCell();
                            nameRow.Cells.Add(nameCell);
                            nameCell.ColumnSpan = 5;
                            nameCell.CssClass = "textHeaderWA";
                            nameCell.HorizontalAlign = HorizontalAlign.Center;
                            nameCell.Text = string.Format("{0} , ID: {1}", compCat.Category.name, compCat.ID);

                            foreach (Log log in Logs)
                            {
                                logsCount++;

                                if (!log.UserIDReference.IsLoaded)
                                {
                                    log.UserIDReference.Load();
                                }

                                TableRow logRow = new TableRow();
                                tblSpecLogs.Rows.Add(logRow);

                                TableCell typeCell = new TableCell();
                                logRow.Cells.Add(typeCell);
                                typeCell.Text = log.type;
                                typeCell.ColumnSpan = 0;
                                typeCell.CssClass = string.Empty;

                                TableCell byCell = CommonCode.UiTools.GetUserTableCell(userContext, log.UserID.ID);
                                logRow.Cells.Add(byCell);

                                TableCell IpCell = new TableCell();
                                logRow.Cells.Add(IpCell);
                                IpCell.Text = log.userIPadress;

                                TableCell dateCell = new TableCell();
                                logRow.Cells.Add(dateCell);
                                dateCell.Text = CommonCode.UiTools.DateTimeToLocalString("d/M/yyyy h:m:s.fff", log.dateCreated);

                                TableCell descrCell = new TableCell();
                                logRow.Cells.Add(descrCell);
                                descrCell.Text = Tools.GetFormattedTextFromDB(log.description);
                            }

                        }
                    }

                    if (logsCount == 0)
                    {
                        message = "There are no logs for this company`s category.";
                        TableRow lastRow = GetNoDataRow(1, message);
                        tblSpecLogs.Rows.Add(lastRow);
                        lastRow.Cells[0].CssClass = null;
                        lastRow.Cells[0].Text = message;
                    }
                }
                else
                {
                    message = "No logs in database.";
                    TableRow lastRow = GetNoDataRow(1, message);
                    tblSpecLogs.Rows.Add(lastRow);
                    lastRow.Cells[0].CssClass = null;
                    lastRow.Cells[0].Text = message;
                }
            }
        }

        private void FillSLProdChars(int number, string logType, long ID)
        {
            BusinessProduct businessProduct = new BusinessProduct();
            Product currProduct = businessProduct.GetProductByIDWV(objectContext, ID);

            IEnumerable<Log> Logs = null;
            int logsCount = 0;
            string message = "";

            if (currProduct == null)
            {
                message = "No such product";
                TableRow lastRow = GetNoDataRow(1, message);
                tblSpecLogs.Rows.Add(lastRow);
                lastRow.Cells[0].CssClass = null;
                lastRow.Cells[0].Text = message;
            }
            else
            {
                IEnumerable<ProductCharacteristics> prodChars = businessProduct.GetAllProductCharacteristics(objectContext, ID, true);
                if (prodChars.Count<ProductCharacteristics>() > 0)
                {
                    foreach (ProductCharacteristics prodChar in prodChars)
                    {
                        Logs = businessLog.GetLogs(objectContext, logType, "prodCharacteristic", prodChar.ID, number, 0);
                        if (Logs.Count<Log>() > 0)
                        {
                            TableRow nameRow = new TableRow();
                            tblSpecLogs.Rows.Add(nameRow);

                            TableCell nameCell = new TableCell();
                            nameRow.Cells.Add(nameCell);
                            nameCell.ColumnSpan = 5;
                            nameCell.CssClass = "textHeaderWA";
                            nameCell.HorizontalAlign = HorizontalAlign.Center;
                            nameCell.Text = string.Format("{0} , ID: {1}", prodChar.name, prodChar.ID);

                            foreach (Log log in Logs)
                            {
                                logsCount++;

                                if (!log.UserIDReference.IsLoaded)
                                {
                                    log.UserIDReference.Load();
                                }

                                TableRow logRow = new TableRow();
                                tblSpecLogs.Rows.Add(logRow);

                                TableCell typeCell = new TableCell();
                                logRow.Cells.Add(typeCell);
                                typeCell.Text = log.type;
                                typeCell.ColumnSpan = 0;
                                typeCell.CssClass = string.Empty;

                                TableCell byCell = CommonCode.UiTools.GetUserTableCell(userContext, log.UserID.ID);
                                logRow.Cells.Add(byCell);

                                TableCell IpCell = new TableCell();
                                logRow.Cells.Add(IpCell);
                                IpCell.Text = log.userIPadress;

                                TableCell dateCell = new TableCell();
                                logRow.Cells.Add(dateCell);
                                dateCell.Text = CommonCode.UiTools.DateTimeToLocalString("d/M/yyyy h:m:s.fff", log.dateCreated);

                                TableCell descrCell = new TableCell();
                                logRow.Cells.Add(descrCell);
                                descrCell.Text = Tools.GetFormattedTextFromDB(log.description);
                            }

                        }
                    }
                    if (logsCount == 0)
                    {
                        message = "There are no logs for this product`s characteristic.";
                        TableRow lastRow = GetNoDataRow(1, message);
                        tblSpecLogs.Rows.Add(lastRow);
                        lastRow.Cells[0].CssClass = null;
                        lastRow.Cells[0].Text = message;
                    }
                }
                else
                {
                    string infoMessage = "No logs in database.";
                    TableRow rowToAdd = GetNoDataRow(1, infoMessage);
                    if (rowToAdd.Cells.Count > 0)
                    {
                        string messageUsed = rowToAdd.Cells[0].Text;
                        if (string.Equals(infoMessage, messageUsed) != true)
                        {
                            throw new CommonCode.UIException(
                                string.Format("Wrong message used: expected \"{0}\", actual \"{1}\".",
                                infoMessage ?? string.Empty, messageUsed ?? string.Empty));
                        }

                        int addedAtIndex = tblSpecLogs.Rows.Add(rowToAdd);

                        string rowToAddMessage = rowToAdd.Cells[0].Text;
                        if (string.Equals(infoMessage, rowToAddMessage) != true)
                        {
                            //
                            // This is somehow related to the fact that tblSpecLogs.EnableViewState is true.
                            //

                            // Fix the cell text.
                            rowToAdd.Cells[0].Text = infoMessage;
                            rowToAdd.Cells[0].CssClass = null;

                            // Check again
                            string rowToAddMessageAfterFix = rowToAdd.Cells[0].Text;
                            if (string.Equals(infoMessage, rowToAddMessageAfterFix) != true)
                            {
                                throw new CommonCode.UIException(
                                    string.Format("Could not fix the message: expected \"{0}\", actual \"{1}\".",
                                    infoMessage ?? string.Empty, rowToAddMessageAfterFix ?? string.Empty));
                            }
                        }
                        string actualMessage = tblSpecLogs.Rows[addedAtIndex].Cells[0].Text;
                        if (string.Equals(infoMessage, actualMessage) != true)
                        {
                            throw new CommonCode.UIException(
                                string.Format("Wrong informational message: expected \"{0}\", actual \"{1}\".",
                                infoMessage ?? string.Empty, actualMessage ?? string.Empty));
                        }
                    }
                }
            }
        }

        private void FillSLVariants(int number, string logType, long ID)
        {
            BusinessProductVariant bpVariant = new BusinessProductVariant();
            BusinessProduct bProduct = new BusinessProduct();

            IEnumerable<Log> Logs = null;
            int logsCount = 0;
            string message = "";

            Product product = bProduct.GetProductByIDWV(objectContext, ID);
            if (product == null)
            {
                message = "There`s no such product.";
                TableRow lastRow = GetNoDataRow(1, message);
                tblSpecLogs.Rows.Add(lastRow);
                lastRow.Cells[0].CssClass = null;
                lastRow.Cells[0].Text = message;
            }
            else
            {
                List<ProductVariant> variants = bpVariant.GetAllProductVariants(objectContext, product);
                List<ProductSubVariant> subVariants = new List<ProductSubVariant>();

                if (variants.Count > 0)
                {
                    foreach (ProductVariant variant in variants)
                    {
                        Logs = businessLog.GetLogs(objectContext, logType, "productVariant", variant.ID, number, 0);
                        if (Logs.Count<Log>() > 0)
                        {
                            TableRow nameRow = new TableRow();
                            tblSpecLogs.Rows.Add(nameRow);

                            TableCell nameCell = new TableCell();
                            nameRow.Cells.Add(nameCell);
                            nameCell.ColumnSpan = 5;
                            nameCell.CssClass = "textHeaderWA";
                            nameCell.HorizontalAlign = HorizontalAlign.Center;
                            nameCell.Text = string.Format("Variant : {0} , ID: {1}", variant.name, variant.ID);
                            nameCell.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();

                            foreach (Log log in Logs)
                            {
                                logsCount++;

                                if (!log.UserIDReference.IsLoaded)
                                {
                                    log.UserIDReference.Load();
                                }

                                TableRow logRow = new TableRow();
                                tblSpecLogs.Rows.Add(logRow);

                                TableCell typeCell = new TableCell();
                                logRow.Cells.Add(typeCell);
                                typeCell.Text = log.type;
                                typeCell.ColumnSpan = 0;
                                typeCell.CssClass = string.Empty;

                                TableCell byCell = CommonCode.UiTools.GetUserTableCell(userContext, log.UserID.ID);
                                logRow.Cells.Add(byCell);

                                TableCell IpCell = new TableCell();
                                logRow.Cells.Add(IpCell);
                                IpCell.Text = log.userIPadress;

                                TableCell dateCell = new TableCell();
                                logRow.Cells.Add(dateCell);
                                dateCell.Text = CommonCode.UiTools.DateTimeToLocalString("d/M/yyyy h:m:s.fff", log.dateCreated);

                                TableCell descrCell = new TableCell();
                                logRow.Cells.Add(descrCell);
                                descrCell.Text = Tools.GetFormattedTextFromDB(log.description);
                            }
                        }

                        subVariants = bpVariant.GetAllSubVariants(objectContext, variant);
                        if (subVariants.Count > 0)
                        {
                            foreach (ProductSubVariant subvariant in subVariants)
                            {
                                Logs = businessLog.GetLogs(objectContext, logType, "productSubVariant", subvariant.ID, number, 0);
                                if (Logs.Count() > 0)
                                {
                                    TableRow nameRow = new TableRow();
                                    tblSpecLogs.Rows.Add(nameRow);

                                    TableCell nameCell = new TableCell();
                                    nameRow.Cells.Add(nameCell);
                                    nameCell.ColumnSpan = 5;
                                    nameCell.CssClass = "textHeaderWA";
                                    nameCell.HorizontalAlign = HorizontalAlign.Center;
                                    nameCell.Text = string.Format("Sub variant : {0} , ID: {1}", variant.name, variant.ID);
                                    nameCell.BackColor = CommonCode.UiTools.GetStandardCellBgrColor();

                                    foreach (Log log in Logs)
                                    {
                                        logsCount++;

                                        if (!log.UserIDReference.IsLoaded)
                                        {
                                            log.UserIDReference.Load();
                                        }

                                        TableRow logRow = new TableRow();
                                        tblSpecLogs.Rows.Add(logRow);

                                        TableCell typeCell = new TableCell();
                                        logRow.Cells.Add(typeCell);
                                        typeCell.Text = log.type;
                                        typeCell.ColumnSpan = 0;
                                        typeCell.CssClass = string.Empty;

                                        TableCell byCell = CommonCode.UiTools.GetUserTableCell(userContext, log.UserID.ID);
                                        logRow.Cells.Add(byCell);

                                        TableCell IpCell = new TableCell();
                                        logRow.Cells.Add(IpCell);
                                        IpCell.Text = log.userIPadress;

                                        TableCell dateCell = new TableCell();
                                        logRow.Cells.Add(dateCell);
                                        dateCell.Text = CommonCode.UiTools.DateTimeToLocalString("d/M/yyyy h:m:s.fff", log.dateCreated);

                                        TableCell descrCell = new TableCell();
                                        logRow.Cells.Add(descrCell);
                                        descrCell.Text = Tools.GetFormattedTextFromDB(log.description);
                                    }

                                }
                            }
                        }
                    }

                    if (logsCount == 0)
                    {
                        message = "There are no logs for this product variants.";
                        TableRow lastRow = GetNoDataRow(1, message);
                        tblSpecLogs.Rows.Add(lastRow);
                        lastRow.Cells[0].CssClass = null;
                        lastRow.Cells[0].Text = message;
                    }
                }
                else
                {
                    message = "No logs in database.";
                    TableRow lastRow = GetNoDataRow(1, message);
                    tblSpecLogs.Rows.Add(lastRow);
                    lastRow.Cells[0].CssClass = null;
                    lastRow.Cells[0].Text = message;
                }
            }
        }


        public void FillLogTable(List<Log> logs)
        {
            tblGetLogs.Rows.Clear();

            tblGetLogs.Visible = true;
            tblSpecLogs.Visible = false;

            if (logs.Count<Log>() > 0)
            {

                TableRow firstRow = new TableRow();

                TableCell typeFCell = new TableCell();
                typeFCell.Text = "type";
                firstRow.Cells.Add(typeFCell);

                TableCell byFCell = new TableCell();
                byFCell.Text = "by";
                firstRow.Cells.Add(byFCell);

                TableCell obectFIdCell = new TableCell();
                obectFIdCell.Text = "ID";
                firstRow.Cells.Add(obectFIdCell);

                TableCell IpFCell = new TableCell();
                IpFCell.Text = "Ip adress";
                firstRow.Cells.Add(IpFCell);

                TableCell dateFCell = new TableCell();
                dateFCell.Text = "date";
                dateFCell.CssClass = "commentsDate";
                firstRow.Cells.Add(dateFCell);

                TableCell descrFCell = new TableCell();
                descrFCell.Text = "description";
                firstRow.Cells.Add(descrFCell);

                tblGetLogs.Rows.Add(firstRow);

                BusinessUser businessUser = new BusinessUser();

                int i = 0;
                System.Drawing.Color color;
                foreach (Log log in logs)
                {
                    TableRow newRow = new TableRow();

                    if (i % 2 == 0)
                    {
                        color = CommonCode.UiTools.GetStandardCellBgrColor();
                    }
                    else
                    {
                        color = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                    }

                    TableCell typeCell = new TableCell();
                    typeCell.Text = log.type;
                    newRow.Cells.Add(typeCell);
                    typeCell.BackColor = color;

                    if (!log.UserIDReference.IsLoaded)
                    {
                        log.UserIDReference.Load();
                    }

                    TableCell byCell = CommonCode.UiTools.GetUserTableCell(userContext, log.UserID.ID);
                    newRow.Cells.Add(byCell);
                    byCell.BackColor = color;
                    

                    TableCell obectIdCell = new TableCell();
                    obectIdCell.Text = log.IDModifiedSubject.ToString();
                    newRow.Cells.Add(obectIdCell);
                    obectIdCell.BackColor = color;

                    TableCell IpCell = new TableCell();
                    IpCell.Text = log.userIPadress;
                    newRow.Cells.Add(IpCell);
                    IpCell.BackColor = color;

                    TableCell dateCell = new TableCell();
                    dateCell.CssClass = "commentsDate";
                    dateCell.Text = CommonCode.UiTools.DateTimeToLocalString("d/M/yyyy HH:mm:ss.fff", log.dateCreated);
                    newRow.Cells.Add(dateCell);
                    dateCell.BackColor = color;

                    TableCell descrCell = new TableCell();
                    descrCell.Text = log.description;
                    newRow.Cells.Add(descrCell);
                    descrCell.BackColor = color;

                    tblGetLogs.Rows.Add(newRow);

                    i++;
                }

            }
            else
            {
                tblGetLogs.Rows.Add(GetNoDataRow(4, null));
            }
        }

        protected void btnShowDeletedCategories_Click(object sender, EventArgs e)
        {
            if (btnShowDeletedCategories.Text == "Show")
            {
                btnShowDeletedCategories.Text = "Hide";
                ShowDeletedCategories();
                tblDeletedCategories.Visible = true;
            }
            else
            {
                btnShowDeletedCategories.Text = "Show";
                tblDeletedCategories.Visible = false;
            }
        }

        protected void btnLdShow_Click(object sender, EventArgs e)
        {
            phLastDeleted.Controls.Clear();

            long wantedTypeId = 0;
            string wanteTypeNameContains = string.Empty;
            int lastNumber = 0;

            User deletedBy = null;
            long byUserID = 0;

            string selectedType = string.Empty;

            StringBuilder errors = new StringBuilder();
            bool validationPassed = true;

            if (!string.IsNullOrEmpty(tbLdID.Text) && !string.IsNullOrEmpty(tbLdNameContains.Text))
            {
                validationPassed = false;
                errors.Append("Fill only wanted type ID field OR name contains field. <br />");
            }
            else
            {
                if(!string.IsNullOrEmpty(tbLdID.Text))
                {
                    if (long.TryParse(tbLdID.Text, out wantedTypeId))
                    {
                        if (wantedTypeId < 1)
                        {
                            validationPassed = false;
                            errors.Append("Type positive number for wanted type ID field. <br />");
                        }
                    }
                    else
                    {
                        validationPassed = false;
                        errors.Append("Type number for wanted type ID field. <br />");
                    }
                }
                else if(!string.IsNullOrEmpty(tbLdNameContains.Text))
                {
                    wanteTypeNameContains = tbLdNameContains.Text;
                }
            }

            if (!string.IsNullOrEmpty(tbLdLastNumber.Text))
            {
                if (int.TryParse(tbLdLastNumber.Text, out lastNumber))
                {
                    if (lastNumber < 1)
                    {
                        validationPassed = false;
                        errors.Append("Type positive number for 'Last number' field. <br />");
                    }
                }
                else
                {
                    validationPassed = false;
                    errors.Append("Type number for 'Last number' field. <br />");
                }
            }
            else
            {
                validationPassed = false;
                errors.Append("Type number for 'Last number' field. <br />");
            }

            if (!string.IsNullOrEmpty(tbLdByName.Text) && !string.IsNullOrEmpty(tbLdByUserId.Text))
            {
                validationPassed = false;
                errors.Append("Fill only by user name field OR by user id field. <br />");
            }
            else if(ddlLdChooseType.SelectedIndex == 4 && (!string.IsNullOrEmpty(tbLdByName.Text) || !string.IsNullOrEmpty(tbLdByUserId.Text)))
            {
                validationPassed = false;
                errors.Append("When listing deleted users, doesn`t look for who deleted them (clear the deleted by field). <br />");
            }
            else
            {
                if (!string.IsNullOrEmpty(tbLdByUserId.Text))
                {
                    if (long.TryParse(tbLdByUserId.Text, out byUserID))
                    {
                        if (byUserID > 0)
                        {
                            BusinessUser bUser = new BusinessUser();
                            deletedBy = bUser.GetWithoutVisible(userContext, byUserID, false);

                            if (deletedBy == null)
                            {
                                validationPassed = false;
                                errors.Append("There is no user with such ID. <br />");
                            }
                        }
                        else
                        {
                            validationPassed = false;
                            errors.Append("Type positive number for 'by user' ID field. <br />");
                        }
                    }
                    else
                    {
                        validationPassed = false;
                        errors.Append("Type number for 'by user' ID field. <br />");
                    }
                }
                else if (!string.IsNullOrEmpty(tbLdByName.Text))
                {
                    string byUserName = tbLdByName.Text;

                    BusinessUser bUser = new BusinessUser();
                    deletedBy = bUser.GetByName(userContext, byUserName, false, false);

                    if (deletedBy == null)
                    {
                        validationPassed = false;
                        errors.Append("There is no user with such name. <br />");
                    }
                }
            }

            if (ddlLdChooseType.SelectedIndex > 0)
            {
                switch (ddlLdChooseType.SelectedIndex)
                {
                    case 1:
                        selectedType = "products";
                        break;
                    case 2:
                        selectedType = "companies";
                        break;
                    case 3:
                        selectedType = "categories";
                        break;
                    case 4:
                        selectedType = "users";
                        break;
                    case 5:
                        selectedType = "topics";
                        break;
                    default:
                        validationPassed = false;
                        errors.Append("Choose type wanted. <br />");
                        break;
                }
            }
            else
            {
                validationPassed = false;
                errors.Append("Choose type wanted. <br />");
            }


            if (validationPassed == true)
            {
                phLastDeleted.Visible = false;
               
                switch (selectedType)
                {
                    case "products":

                        BusinessProduct bProduct = new BusinessProduct();
                        List<Product> products = bProduct.GetLastDeletedProducts(objectContext, lastNumber, wanteTypeNameContains, wantedTypeId, deletedBy);
                        FillLastDeletedProducts(products);

                        break;
                    case "companies":

                        BusinessCompany bCompany = new BusinessCompany();
                        List<Company> companies = bCompany.GetLastDeletedCompanies(objectContext, lastNumber, wanteTypeNameContains, wantedTypeId, deletedBy);
                        FillLastDeletedCompanies(companies);

                        break;
                    case "categories":

                        BusinessCategory bCategory = new BusinessCategory();
                        List<Category> categories = bCategory.GetLastDeletedCategories(objectContext, lastNumber, wanteTypeNameContains, wantedTypeId, deletedBy);
                        FillLastDeletedCategories(categories);

                        break;
                    case "users":

                        BusinessUser bUser = new BusinessUser();
                        List<User> users = bUser.GetLastDeletedUsers(userContext, lastNumber, wanteTypeNameContains, wantedTypeId);
                        FillLastDeletedUsers(users);

                        break;
                    case "topics":

                        BusinessProductTopics bpTopics = new BusinessProductTopics();
                        List<ProductTopic> topics = bpTopics.GetLastDeletedTopics(objectContext, lastNumber, wanteTypeNameContains, wantedTypeId, deletedBy);
                        FillLastDeletedTopics(topics);

                        break;
                    default:
                        throw new CommonCode.UIException(string.Format("selectedType = {0} is not supported, user id : {1}", selectedType, CurrentUser.ID));
                }


            }
            else
            {
                phLastDeleted.Controls.Clear();

                phLastDeleted.Visible = true;
                phLastDeleted.Controls.Add(lblError);
                lblError.Text = errors.ToString();
            }
        }

        private void FillLastDeletedProducts(List<Product> products)
        {
            tblShowLastDeleted.Rows.Clear();
            tblShowLastDeleted.Visible = true;

            if (products.Count > 0)
            {
                BusinessUser bUser = new BusinessUser();
                User user = null;

                TableRow firstRow = new TableRow();
                tblShowLastDeleted.Rows.Add(firstRow);

                TableCell nameFCell = new TableCell();
                firstRow.Cells.Add(nameFCell);
                nameFCell.Text = "product";

                TableCell lastFModified = new TableCell();
                firstRow.Cells.Add(lastFModified);
                lastFModified.Text = "Last modified on";

                TableCell lastFModifiedBy = new TableCell();
                firstRow.Cells.Add(lastFModifiedBy);
                lastFModifiedBy.Text = "Last modified by";

                foreach (Product product in products)
                {
                    TableRow newRow = new TableRow();
                    tblShowLastDeleted.Rows.Add(newRow);

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);

                    HyperLink prodLink = CommonCode.UiTools.GetProductHyperLink(product);
                    nameCell.Controls.Add(prodLink);

                    TableCell lastModified = new TableCell();
                    newRow.Cells.Add(lastModified);
                    lastModified.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(product.lastModified);

                    TableCell lastModifiedBy = new TableCell();
                    newRow.Cells.Add(lastModifiedBy);

                    if (!product.LastModifiedByReference.IsLoaded)
                    {
                        product.LastModifiedByReference.Load();
                    }

                    user = bUser.GetWithoutVisible(product.LastModifiedBy.ID, true);
                    lastModifiedBy.Controls.Add(CommonCode.UiTools.GetUserHyperLink(user));
                }
            }
            else
            {
                TableRow lastRow = new TableRow();
                tblShowLastDeleted.Rows.Add(lastRow);

                TableCell lastCell = new TableCell();
                lastRow.Cells.Add(lastCell);

                lastCell.Text = "no deleted products";

            }
            

        }

        private void FillLastDeletedTopics(List<ProductTopic> topics)
        {
            tblShowLastDeleted.Rows.Clear();
            tblShowLastDeleted.Visible = true;

            if (topics.Count > 0)
            {
                BusinessUser bUser = new BusinessUser();
                User user = null;

                TableRow firstRow = new TableRow();
                tblShowLastDeleted.Rows.Add(firstRow);

                TableCell nameFCell = new TableCell();
                firstRow.Cells.Add(nameFCell);
                nameFCell.Text = "topic";

                TableCell lastFModified = new TableCell();
                firstRow.Cells.Add(lastFModified);
                lastFModified.Text = "Last modified on";

                TableCell lastFModifiedBy = new TableCell();
                firstRow.Cells.Add(lastFModifiedBy);
                lastFModifiedBy.Text = "Last modified by";

                foreach (ProductTopic topic in topics)
                {
                    TableRow newRow = new TableRow();
                    tblShowLastDeleted.Rows.Add(newRow);

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);

                    HyperLink topicLink = new HyperLink();
                    nameCell.Controls.Add(topicLink);
                    topicLink.Text = topic.name;
                    topicLink.NavigateUrl = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", topic.ID));

                    TableCell lastModified = new TableCell();
                    newRow.Cells.Add(lastModified);
                    lastModified.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(topic.lastModified);

                    TableCell lastModifiedBy = new TableCell();
                    newRow.Cells.Add(lastModifiedBy);

                    if (!topic.LastModifiedByReference.IsLoaded)
                    {
                        topic.LastModifiedByReference.Load();
                    }

                    user = bUser.GetWithoutVisible(topic.LastModifiedBy.ID, true);
                    lastModifiedBy.Controls.Add(CommonCode.UiTools.GetUserHyperLink(user));
                }
            }
            else
            {
                TableRow lastRow = new TableRow();
                tblShowLastDeleted.Rows.Add(lastRow);

                TableCell lastCell = new TableCell();
                lastRow.Cells.Add(lastCell);

                lastCell.Text = "no deleted topics";

            }


        }

        private void FillLastDeletedCompanies(List<Company> companies)
        {
            tblShowLastDeleted.Rows.Clear();
            tblShowLastDeleted.Visible = true;

            if (companies.Count > 0)
            {
                BusinessUser bUser = new BusinessUser();
                User user = null;

                TableRow firstRow = new TableRow();
                tblShowLastDeleted.Rows.Add(firstRow);

                TableCell nameFCell = new TableCell();
                firstRow.Cells.Add(nameFCell);
                nameFCell.Text = "company";

                TableCell lastFModified = new TableCell();
                firstRow.Cells.Add(lastFModified);
                lastFModified.Text = "Last modified on";

                TableCell lastFModifiedBy = new TableCell();
                firstRow.Cells.Add(lastFModifiedBy);
                lastFModifiedBy.Text = "Last modified by";

                foreach (Company company in companies)
                {
                    TableRow newRow = new TableRow();
                    tblShowLastDeleted.Rows.Add(newRow);

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);

                    HyperLink compLink = CommonCode.UiTools.GetCompanyHyperLink(company);
                    nameCell.Controls.Add(compLink);

                    TableCell lastModified = new TableCell();
                    newRow.Cells.Add(lastModified);
                    lastModified.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(company.lastModified);

                    TableCell lastModifiedBy = new TableCell();
                    newRow.Cells.Add(lastModifiedBy);

                    if (!company.LastModifiedByReference.IsLoaded)
                    {
                        company.LastModifiedByReference.Load();
                    }

                    user = bUser.GetWithoutVisible(company.LastModifiedBy.ID, true);
                    lastModifiedBy.Controls.Add(CommonCode.UiTools.GetUserHyperLink(user));
                }
            }
            else
            {
                TableRow lastRow = new TableRow();
                tblShowLastDeleted.Rows.Add(lastRow);

                TableCell lastCell = new TableCell();
                lastRow.Cells.Add(lastCell);

                lastCell.Text = "no deleted companies";

            }

        }

        private void FillLastDeletedCategories(List<Category> categories)
        {
            tblShowLastDeleted.Rows.Clear();
            tblShowLastDeleted.Visible = true;

            if (categories.Count > 0)
            {
                BusinessUser bUser = new BusinessUser();
                User user = null;

                TableRow firstRow = new TableRow();
                tblShowLastDeleted.Rows.Add(firstRow);

                TableCell nameFCell = new TableCell();
                firstRow.Cells.Add(nameFCell);
                nameFCell.Text = "category";

                TableCell lastFModified = new TableCell();
                firstRow.Cells.Add(lastFModified);
                lastFModified.Text = "Last modified on";

                TableCell lastFModifiedBy = new TableCell();
                firstRow.Cells.Add(lastFModifiedBy);
                lastFModifiedBy.Text = "Last modified by";

                foreach (Category category in categories)
                {
                    TableRow newRow = new TableRow();
                    tblShowLastDeleted.Rows.Add(newRow);

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);

                    nameCell.Controls.Add(CommonCode.UiTools.GetCategoryNameWithLink(category, objectContext, false, false, false));

                    TableCell lastModified = new TableCell();
                    newRow.Cells.Add(lastModified);
                    lastModified.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(category.lastModified);

                    TableCell lastModifiedBy = new TableCell();
                    newRow.Cells.Add(lastModifiedBy);

                    if (!category.LastModifiedByReference.IsLoaded)
                    {
                        category.LastModifiedByReference.Load();
                    }

                    user = bUser.GetWithoutVisible(category.LastModifiedBy.ID, true);
                    lastModifiedBy.Controls.Add(CommonCode.UiTools.GetUserHyperLink(user));
                }
            }
            else
            {
                TableRow lastRow = new TableRow();
                tblShowLastDeleted.Rows.Add(lastRow);

                TableCell lastCell = new TableCell();
                lastRow.Cells.Add(lastCell);

                lastCell.Text = "no deleted categories";

            }



        }

        private void FillLastDeletedUsers(List<User> users)
        {
            tblShowLastDeleted.Rows.Clear();
            tblShowLastDeleted.Visible = true;

            if (users.Count > 0)
            {
                TableRow firstRow = new TableRow();
                tblShowLastDeleted.Rows.Add(firstRow);

                TableCell nameFCell = new TableCell();
                firstRow.Cells.Add(nameFCell);
                nameFCell.Text = "user";

                TableCell typrFCell = new TableCell();
                firstRow.Cells.Add(typrFCell);
                typrFCell.Text = "type";

                foreach (User user in users)
                {
                    TableRow newRow = new TableRow();
                    tblShowLastDeleted.Rows.Add(newRow);

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);

                    nameCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink(user));

                    TableCell typeCell = new TableCell();
                    newRow.Cells.Add(typeCell);

                    typeCell.Text = user.type;
                  
                }
            }
            else
            {
                TableRow lastRow = new TableRow();
                tblShowLastDeleted.Rows.Add(lastRow);

                TableCell lastCell = new TableCell();
                lastRow.Cells.Add(lastCell);

                lastCell.Text = "no deleted users";

            }
        }



    }
}
