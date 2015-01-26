﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class SuggestionsForSite : BasePage
    {
        protected long SuggestionsNumber = 0;                                   // Number of suggestions posted
        protected long CurrSuggestionsPage = 1;                                 // Current suggestions page
        protected long SuggestionsOnPage = Configuration.SuggestionsPerPage;    // Suggestions per page
        protected long SuggestionsType = 0;    // Show suggestion from type : 0 - all , 1 - general , 2 - design, 3 - features

        private User currentUser = null;

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        public static object lockSpam = new object();

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckPageParams();                   // Checks page parameter 
            CheckUser();                        // Checks User , if hes logged and can , shows send suggestions pnl
            ShowInfo();
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);
        }

        private void CheckPageParams()
        {
            CheckTypeParam();       // checks about parameter
            CountSuggestions();     // counts suggestions

            CommonCode.Pages.CheckPageParameters(Response, SuggestionsNumber, SuggestionsOnPage.ToString(),
                Request.Params["page"], GetUrlWithVariant("SuggestionsForSite.aspx"), out CurrSuggestionsPage, out SuggestionsOnPage);
        }

        private void CheckTypeParam()
        {
            string strType = Request.Params["about"];
            if (string.IsNullOrEmpty(strType))
            {
                SuggestionsType = 0;
            }
            else
            {
                switch (strType)
                {
                    case ("general"):
                        SuggestionsType = 1;
                        break;
                    case ("design"):
                        SuggestionsType = 2;
                        break;
                    case ("features"):
                        SuggestionsType = 3;
                        break;
                    default:
                        RedirectToOtherUrl("SuggestionsForSite.aspx");
                        break;
                }
            }
        }

        private void CountSuggestions()
        {
            BusinessSuggestion businessSuggestion = new BusinessSuggestion();


            lblCount.Style.Add(HtmlTextWriterStyle.MarginBottom, "5px");
            switch (SuggestionsType)
            {
                case 0:
                    SuggestionsNumber = businessSuggestion.CountVisibleSuggestions(objectContext);
                    lblCount.Text = string.Format("{0} {1}", GetLocalResourceObject("Suggestions"), SuggestionsNumber);
                    break;
                case 1:
                    SuggestionsNumber = businessSuggestion.CountVisibleSuggestionFromType(objectContext, SuggestionType.General);
                    lblCount.Text = string.Format("{0} {1}", GetLocalResourceObject("GeneralSuggestions"), SuggestionsNumber);
                    break;
                case 2:
                    SuggestionsNumber = businessSuggestion.CountVisibleSuggestionFromType(objectContext, SuggestionType.Design);
                    lblCount.Text = string.Format("{0} {1}", GetLocalResourceObject("DesignSuggestions"), SuggestionsNumber);
                    break;
                case 3:
                    SuggestionsNumber = businessSuggestion.CountVisibleSuggestionFromType(objectContext, SuggestionType.Features);
                    lblCount.Text = string.Format("{0} {1}", GetLocalResourceObject("FeaturesSuggestions"), SuggestionsNumber);
                    break;
                default:
                    throw new CommonCode.UIException(string.Format("SuggestionsType = {0} is not supported type", SuggestionsType));
            }


        }

        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);

            if (currUser != null)
            {
                currentUser = currUser;
                if (businessUser.CanUserDo(userContext, currUser, UserRoles.WriteSuggestions))
                {
                    BusinessSuggestion businessSuggestion = new BusinessSuggestion();

                    int currSuggCount = businessSuggestion.CountUserSuggestions(objectContext, currentUser);

                    if (currSuggCount >= Configuration.SuggestionsMaxUserSuggestions)
                    {
                        pnlWriteSuggestion.Visible = false;

                        lblCountInfo.Text = GetLocalResourceObject("suggestionsCountInfo2").ToString();
                    }
                    else
                    {
                        pnlWriteSuggestion.Visible = true;

                        int count = Configuration.SuggestionsMaxUserSuggestions - currSuggCount;
                        lblCountInfo.Text = string.Format("{0} {1}", GetLocalResourceObject("suggestionsCountInfo"), count);
                    }
                }
                else
                {
                    lblCountInfo.Visible = false;
                }

                if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditUsers))
                {
                    accAdmin.Visible = true;
                }
            }
            else
            {
                lblCountInfo.Visible = false;
            }

        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            BusinessSuggestion businessSuggestion = new BusinessSuggestion();
            BusinessSiteText siteText = new BusinessSiteText();
            SiteNews about = siteText.GetSiteText(objectContext, "suggestionsAbout");

            if (about != null && about.visible)
            {
                lblAbout.Text = about.description;
            }
            else
            {
                lblAbout.Text = "About Suggestions text not written";
            }
            lblAbout.Style.Add(HtmlTextWriterStyle.MarginBottom, "5px");

            FillDdlShowByType();
            FillDdlSuggType();

            FillTblPages();
            FillTblSuggestions();
            FillTblShowLastDeleted();

            ShowAdvertisement();
            SetLocalText();
        }

        private void SetLocalText()
        {
            lblWriteSugg.Text = GetLocalResourceObject("WriteSuggestion").ToString();
            lblSuggType.Text = GetLocalResourceObject("About").ToString();
            lblSuggestionDescr.Text = GetLocalResourceObject("Description").ToString();
            btnAddSuggestion.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
            lblShow.Text = GetLocalResourceObject("Show").ToString();

        }

        private void FillDdlShowByType()
        {
            if (IsPostBack == false)
            {
                ddlShowByType.Items.Clear();

                ListItem firstItem = new ListItem();
                ddlShowByType.Items.Add(firstItem);
                firstItem.Text = GetLocalResourceObject("select").ToString();
                firstItem.Value = "select";

                ListItem allItem = new ListItem();
                ddlShowByType.Items.Add(allItem);
                allItem.Text = GetLocalResourceObject("all").ToString();
                allItem.Value = "all";

                ListItem genItem = new ListItem();
                ddlShowByType.Items.Add(genItem);
                genItem.Text = GetLocalResourceObject("generalMany").ToString();
                genItem.Value = "0";

                ListItem designItem = new ListItem();
                ddlShowByType.Items.Add(designItem);
                designItem.Text = GetLocalResourceObject("designMany").ToString();
                designItem.Value = "1";

                ListItem featItem = new ListItem();
                ddlShowByType.Items.Add(featItem);
                featItem.Text = GetLocalResourceObject("featuresMany").ToString();
                featItem.Value = "2";

            }
        }

        private void FillDdlSuggType()
        {
            if (IsPostBack == false)
            {
                ddlSuggType.Items.Clear();

                ListItem genItem = new ListItem();
                ddlSuggType.Items.Add(genItem);
                genItem.Text = GetLocalResourceObject("general").ToString();
                genItem.Value = "0";

                ListItem designItem = new ListItem();
                ddlSuggType.Items.Add(designItem);
                designItem.Text = GetLocalResourceObject("design").ToString();
                designItem.Value = "1";

                ListItem featItem = new ListItem();
                ddlSuggType.Items.Add(featItem);
                featItem.Text = GetLocalResourceObject("features").ToString();
                featItem.Value = "2";

            }
        }

        private void ShowAdvertisement()
        {
            if (Configuration.AdvertsNumAdvertsOnCategoryPage > 0)
            {
                phAdvert.Controls.Clear();
                adcell.Attributes.Clear();


                phAdvert.Controls.Add(CommonCode.ImagesAndAdverts.GetAdvertisements
                    (objectContext, Server, "general", 1, Configuration.AdvertsNumAdvertsOnSuggestionsPage));
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

        private void FillTblPages()
        {
            string url = GetUrlWithVariant("SuggestionsForSite.aspx");
            switch (SuggestionsType)
            {
                case 1:
                    url += "?about=general";
                    break;
                case 2:
                    url += "?about=design";
                    break;
                case 3:
                    url += "?about=features";
                    break;
                default:
                    break;
            }

            tblPages.Rows.Clear();
            TableRow pagesRow = CommonCode.Pages.GetPagesRow(SuggestionsNumber, SuggestionsOnPage,
                CurrSuggestionsPage, url);
            tblPages.Rows.Add(pagesRow);

            tblPagesBtm.Rows.Clear();
            TableRow pagesRowBtm = CommonCode.Pages.GetPagesRow(SuggestionsNumber, SuggestionsOnPage,
                CurrSuggestionsPage, url);
            tblPagesBtm.Rows.Add(pagesRowBtm);
        }

        private void FillTblSuggestions()
        {
            phSuggestions.Controls.Clear();

            BusinessUser businessUser = new BusinessUser();
            BusinessSuggestion businessSuggestion = new BusinessSuggestion();

            long from = 0;
            long to = 0;

            CommonCode.Pages.GetFromItemNumberToItemNumber(CurrSuggestionsPage, SuggestionsOnPage, out from, out to);

            List<Suggestion> Suggestions = new List<Suggestion>();
            switch (SuggestionsType)
            {
                case 0:
                    Suggestions = businessSuggestion.GetSuggestions(objectContext, from, to);
                    break;
                case 1:
                    Suggestions = businessSuggestion.GetSuggestionsWithType(objectContext, SuggestionType.General, from, to);
                    break;
                case 2:
                    Suggestions = businessSuggestion.GetSuggestionsWithType(objectContext, SuggestionType.Design, from, to);
                    break;
                case 3:
                    Suggestions = businessSuggestion.GetSuggestionsWithType(objectContext, SuggestionType.Features, from, to);
                    break;
                default:
                    throw new CommonCode.UIException(string.Format("SuggestionsType = {0} is not supported type"));
            }


            Boolean canEdit = false;
            Boolean canReport = false;

            if (currentUser != null)
            {
                if (businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditUsers))
                {
                    canEdit = true;
                }
                if (businessUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate))
                {
                    canReport = true;
                }

            }

            if (Suggestions.Count<Suggestion>() > 0)
            {
                long i = 0;
                foreach (Suggestion suggestion in Suggestions)
                {
                    if (!suggestion.UserReference.IsLoaded)
                    {
                        suggestion.UserReference.Load();
                    }

                    Panel sugg = new Panel();
                    phSuggestions.Controls.Add(sugg);
                    sugg.CssClass = "suggestions";

                    if (i % 2 == 0)
                    {
                        sugg.CssClass = "suggestions greenCellBgr";
                    }
                    else
                    {
                        sugg.CssClass = "suggestions blueCellBgr";
                    }

                    System.Web.UI.HtmlControls.HtmlGenericControl header = new System.Web.UI.HtmlControls.HtmlGenericControl("p");

                    Panel inlPanel = new Panel();
                    sugg.Controls.Add(inlPanel);
                    inlPanel.CssClass = "panelInline";
                    inlPanel.Width = Unit.Pixel(220);

                    HyperLink userLink = new HyperLink();
                    userLink = CommonCode.UiTools.GetUserHyperLink(Tools.GetUserFromUserDatabase(userContext, suggestion.User));
                    userLink.CssClass = "userNames";
                    inlPanel.Controls.Add(userLink);

                    Label dateLbl = new Label();
                    dateLbl.Text = string.Format("({0})", CommonCode.UiTools.DateTimeToLocalString(suggestion.dateCreated));
                    dateLbl.CssClass = "smallFontSize";
                    sugg.Controls.Add(dateLbl);

                    sugg.Controls.Add(CommonCode.UiTools.GetSpaceLabel(7));

                    Label lblType = new Label();
                    lblType.Text = BusinessSuggestion.GetSuggestionCategoryLocalText(suggestion);
                    lblType.CssClass = "searchPageComments";
                    sugg.Controls.Add(lblType);

                    if (canEdit)
                    {
                        sugg.Controls.Add(CommonCode.UiTools.GetSpaceLabel(7));

                        Button delBtn = new Button();
                        sugg.Controls.Add(delBtn);
                        delBtn.Attributes.Add("suggId", suggestion.ID.ToString());
                        delBtn.ID = string.Format("Delete{0}", suggestion.ID);
                        delBtn.Text = "Delete";
                        delBtn.Click += new EventHandler(DeleteSuggestion_Click);

                        delBtn.Attributes.Add("style", "margin-left:3px; margin-right:3px;");


                        sugg.Controls.Add(CommonCode.UiTools.GetSpaceLabel(7));

                        Button delBtnWithWarn = new Button();
                        sugg.Controls.Add(delBtnWithWarn);
                        delBtnWithWarn.Attributes.Add("suggId", suggestion.ID.ToString());
                        delBtnWithWarn.ID = string.Format("DeleteWW{0}", suggestion.ID);
                        delBtnWithWarn.Text = "DeleteWithWarning";
                        delBtnWithWarn.Click += new EventHandler(DeleteSuggestionWithWarning_Click);

                        delBtnWithWarn.Attributes.Add("style", "margin-left:3px; margin-right:3px;");
                    }



                    if (canReport)
                    {
                        Panel newPanel = new Panel();
                        sugg.Controls.Add(newPanel);
                        newPanel.CssClass = "floatRightNoMrg";

                        Label spamlbl = new Label();
                        spamlbl.ID = string.Format("markSpam{0}", suggestion.ID);
                        newPanel.Controls.Add(spamlbl);
                        spamlbl.Text = GetGlobalResourceObject("SiteResources", "violation").ToString();
                        spamlbl.ToolTip = GetGlobalResourceObject("SiteResources", "violationTooltip").ToString();
                        spamlbl.CssClass = "markLbl";
                        spamlbl.Attributes.Add("onclick", string.Format("ShowSuggActionData('{0}','{1}','{2}')"
                            , pnlSuggAction.ClientID, spamlbl.ClientID, suggestion.ID));
                    }

                    Label descrLbl = new Label();
                    descrLbl.Text = string.Format("<br/>{0}", Tools.GetFormattedTextFromDB(suggestion.description));
                    sugg.Controls.Add(descrLbl);

                    // }
                    i++;
                }
            }
            else
            {
                if (SuggestionsType != 0)
                {
                    Panel noSuggPnl = new Panel();
                    phSuggestions.Controls.Add(noSuggPnl);

                    Label noResults = new Label();
                    noSuggPnl.Controls.Add(noResults);

                    noResults.Text = "&nbsp" + GetLocalResourceObject("NoSuggestions").ToString();
                    noResults.CssClass = "errors";
                }

            }


        }

        void DeleteSuggestion_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            if (currentUser == null || !businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
            {
                if (currentUser != null)
                {
                    throw new CommonCode.UIException(string.Format("User id = {0} cannot delete suggestions", currentUser.ID));
                }
                else
                {
                    throw new CommonCode.UIException("Guest cannot delete suggestions");
                }
            }

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                long suggID = 0;
                string suggIdStr = btnSender.Attributes["suggId"];
                long.TryParse(suggIdStr, out suggID);

                if (suggID > 0)
                {
                    BusinessSuggestion businessSuggestion = new BusinessSuggestion();
                    Suggestion currSuggestion = businessSuggestion.Get(objectContext, suggID);
                    if (currSuggestion != null)
                    {
                        businessSuggestion.DeleteSuggestion(objectContext, userContext, currSuggestion, currentUser, businessLog, false);
                        CountSuggestions();
                        ShowInfo();

                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Suggestion deleted!");
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format
                            ("Theres no Suggestion Id = {0} (comment table) , user id = {1}", suggID, currentUser.ID));
                    }
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("suggID is < 1 (comming from tblRow.Attributes['suggId']) , user id = {0}", currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get Button.");
            }
        }

        void DeleteSuggestionWithWarning_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            if (currentUser == null || !businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
            {
                if (currentUser != null)
                {
                    throw new CommonCode.UIException(string.Format("User id = {0} cannot delete suggestions", currentUser.ID));
                }
                else
                {
                    throw new CommonCode.UIException("Guest cannot delete suggestions");
                }
            }

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                long suggID = 0;
                string suggIdStr = btnSender.Attributes["suggId"];
                long.TryParse(suggIdStr, out suggID);

                if (suggID > 0)
                {
                    BusinessSuggestion businessSuggestion = new BusinessSuggestion();
                    Suggestion currSuggestion = businessSuggestion.Get(objectContext, suggID);
                    if (currSuggestion != null)
                    {
                        businessSuggestion.DeleteSuggestion(objectContext, userContext, currSuggestion, currentUser, businessLog, true);
                        CountSuggestions();
                        ShowInfo();

                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Suggestion deleted and warning sent to user!");
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format
                            ("Theres no Suggestion Id = {0} (comment table) , user id = {1}", suggID, currentUser.ID));
                    }
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("suggID is < 1 (comming from tblRow.Attributes['suggId']) , user id = {0}", currentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get Button.");
            }
        }



        protected void btnAddSuggestion_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            if (currentUser == null || !businessUser.CanUserDo(userContext, currentUser, UserRoles.WriteSuggestions))
            {
                return;
            }

            phAddSuggestion.Visible = true;
            phAddSuggestion.Controls.Add(lblError);
            String error = "";

            BusinessSuggestion businessSuggestion = new BusinessSuggestion();
            if (businessSuggestion.CountUserSuggestions(objectContext, currentUser)
                <= Configuration.SuggestionsMaxUserSuggestions)
            {

                string description = tbSuggestionDescr.Text;

                if (CommonCode.Validate.ValidateComment(ref description, out error))
                {

                    businessSuggestion.Add(objectContext, currentUser, description, GetSuggestionType(ddlSuggType), businessLog);

                    phAddSuggestion.Visible = false;
                    tbSuggestionDescr.Text = "";

                    CountSuggestions();
                    ShowInfo();
                    CheckUser();

                    int remSuggestions = (Configuration.SuggestionsMaxUserSuggestions
                        - businessSuggestion.CountUserSuggestions(objectContext, currentUser));

                    if (remSuggestions < 0)
                    {
                        remSuggestions = 0;
                    }

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , string.Format("{0}<br />{1} {2}", GetLocalResourceObject("SuggWritten")
                        , GetLocalResourceObject("SuggWritten2"), remSuggestions));
                }
            }
            else
            {
                throw new CommonCode.UIException(string.Format("User : {0}, ID : {1}, cannot post anymore suggestions."
                    , currentUser.username, currentUser.ID));
            }
            lblError.Text = error;
        }

        protected void btnShowDeleted_Click(object sender, EventArgs e)
        {
            phShowDeleted.Visible = true;
            phShowDeleted.Controls.Add(lblError);
            String error = "";

            if (CommonCode.Validate.ValidateLong(tbShowDeleted.Text, out error))
            {
                long num = -1;
                if (long.TryParse(tbShowDeleted.Text, out num))
                {
                    if (num < 1)
                    {
                        throw new CommonCode.UIException(string.Format("num is < 1 (comming from tbShowDeleted.Text) , user id = {0}", currentUser.ID));
                    }

                    hfDeletedNum.Value = num.ToString();
                    FillTblShowLastDeleted();
                }
                else
                {
                    throw new CommonCode.UIException(string.Format
                        ("Couldnt parse tbShowDeleted.Text = {0} to long , user id = {1}", tbShowDeleted.Text, currentUser.ID));
                }
            }
            lblError.Text = error;
        }

        private void FillTblShowLastDeleted()
        {
            tblShowLastDeleted.Rows.Clear();

            BusinessUser businessUser = new BusinessUser();
            if (currentUser != null && businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditUsers))
            {
                if (!string.IsNullOrEmpty(hfDeletedNum.Value))
                {
                    long num = -1;
                    if (long.TryParse(hfDeletedNum.Value, out num))
                    {
                        if (num < 1)
                        {
                            hfDeletedNum.Value = null;
                            tblShowLastDeleted.Visible = false;
                        }
                        else
                        {

                            tblShowLastDeleted.Visible = true;

                            BusinessSuggestion businessSuggestion = new BusinessSuggestion();
                            List<Suggestion> DeletedSuggestions = businessSuggestion.GetDeletedSuggestions(objectContext, num);

                            if (DeletedSuggestions.Count > 0)
                            {
                                foreach (Suggestion suggestion in DeletedSuggestions)
                                {
                                    if (!suggestion.UserReference.IsLoaded)
                                    {
                                        suggestion.UserReference.Load();
                                    }
                                    if (!suggestion.LastModifiedByReference.IsLoaded)
                                    {
                                        suggestion.LastModifiedByReference.Load();
                                    }

                                    TableRow aboutRow = new TableRow();
                                    tblShowLastDeleted.Rows.Add(aboutRow);
                                    aboutRow.Attributes.Add("suggToUnDel", suggestion.ID.ToString());

                                    TableCell writtenCell = new TableCell();
                                    writtenCell.Width = Unit.Pixel(240);
                                    writtenCell.CssClass = "allBorders";
                                    aboutRow.Cells.Add(writtenCell);
                                    writtenCell.Text = "Created : " + CommonCode.UiTools.DateTimeToLocalString(suggestion.dateCreated);

                                    TableCell whenCell = new TableCell();
                                    whenCell.Width = Unit.Pixel(240);
                                    whenCell.CssClass = "allBorders";
                                    aboutRow.Cells.Add(whenCell);
                                    whenCell.Text = "Deleted : " + CommonCode.UiTools.DateTimeToLocalString(suggestion.lastModified);

                                    TableCell whoCell = new TableCell();
                                    whoCell.CssClass = "allBorders";
                                    aboutRow.Cells.Add(whoCell);
                                    Label delbyLbl = new Label();
                                    delbyLbl.Text = "Deleted by : ";
                                    whoCell.Controls.Add(delbyLbl);
                                    whoCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                                        (Tools.GetUserFromUserDatabase(userContext, suggestion.LastModifiedBy)));

                                    TableCell fromCell = new TableCell();
                                    fromCell.CssClass = "allBorders";
                                    aboutRow.Cells.Add(fromCell);
                                    Label fromLbl = new Label();
                                    fromLbl.Text = "From : ";
                                    fromCell.Controls.Add(fromLbl);
                                    fromCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                                        (Tools.GetUserFromUserDatabase(userContext, suggestion.User)));

                                    TableCell aboutCell = new TableCell();
                                    aboutCell.CssClass = "allBorders";
                                    aboutCell.Width = Unit.Pixel(100);
                                    aboutCell.HorizontalAlign = HorizontalAlign.Center;
                                    aboutRow.Cells.Add(aboutCell);
                                    aboutCell.Text = suggestion.category;

                                    TableCell unDelCell = new TableCell();
                                    unDelCell.CssClass = "allBorders";
                                    unDelCell.Width = Unit.Pixel(10);
                                    aboutRow.Cells.Add(unDelCell);
                                    Button unDelBtn = new Button();
                                    unDelCell.Controls.Add(unDelBtn);
                                    unDelBtn.ID = string.Format("unDel{0}", suggestion.ID);
                                    unDelBtn.Text = "Undelete";
                                    unDelBtn.Click += new EventHandler(UnDeleteSuggestion_Click);

                                    TableRow descrRow = new TableRow();
                                    tblShowLastDeleted.Rows.Add(descrRow);

                                    TableCell descrCell = new TableCell();
                                    descrCell.CssClass = "allBorders";
                                    descrRow.Cells.Add(descrCell);
                                    descrCell.ColumnSpan = 6;
                                    descrCell.Text = Tools.GetFormattedTextFromDB(suggestion.description);

                                }
                            }
                            else
                            {
                                TableRow lastRow = new TableRow();
                                tblShowLastDeleted.Rows.Add(lastRow);

                                TableCell lastCell = new TableCell();
                                lastRow.Cells.Add(lastCell);
                                lastCell.Text = "There is no deleted suggestions.";

                                hfDeletedNum.Value = null;
                            }

                        }
                    }
                    else
                    {
                        hfDeletedNum.Value = null;
                        tblShowLastDeleted.Visible = false;
                    }
                }
            }
            else
            {
                tblShowLastDeleted.Visible = false;
            }
        }

        protected void UnDeleteSuggestion_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            if (currentUser == null || !businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditComments))
            {
                if (currentUser != null)
                {
                    throw new CommonCode.UIException(string.Format("User ID = {0} cannot UNdelete suggestions", currentUser.ID));
                }
                else
                {
                    throw new CommonCode.UIException("Guest cannot UNdelete suggestions");
                }
            }

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                TableCell tblCell = btnSender.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long suggID = 0;
                        string suggIdStr = tblRow.Attributes["suggToUnDel"];
                        long.TryParse(suggIdStr, out suggID);

                        if (suggID > 0)
                        {

                            BusinessSuggestion businessSuggestion = new BusinessSuggestion();
                            Suggestion currSuggestion = businessSuggestion.GetWithoutVisible(objectContext, suggID);
                            if (currSuggestion != null)
                            {
                                businessSuggestion.UnDeleteSuggestion(objectContext, currSuggestion, currentUser, businessLog);
                                CountSuggestions();
                                ShowInfo();

                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Suggestion UN-deleted!");
                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format("Theres no comment id = {0} , user id = {1}", suggID, currentUser.ID));
                            }

                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("suggID is < 1 (comming from tblRow.Attributes['suggToUnDel']) , user id = {0}", currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldnt get parent row.");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Couldnt Get parent cell.");
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get Button.");
            }
        }



        private SuggestionType GetSuggestionType(DropDownList list)
        {
            if (list == null)
            {
                throw new CommonCode.UIException("list is null");
            }

            if (string.IsNullOrEmpty(list.SelectedValue))
            {
                throw new CommonCode.UIException("list.SelectedValue is null or empty");
            }

            SuggestionType type;

            switch (list.SelectedValue)
            {
                case "0":
                    type = SuggestionType.General;
                    break;
                case "1":
                    type = SuggestionType.Design;
                    break;
                case "2":
                    type = SuggestionType.Features;
                    break;
                default:
                    throw new CommonCode.UIException(string.Format("Selected Value = {0} is not supported value for suggestion type."
                        , list.SelectedValue));
            }

            return type;
        }

        protected void ddlShowByType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlShowByType.SelectedValue == "all")
            {
                RedirectToOtherUrl("SuggestionsForSite.aspx");
            }
            else
            {
                SuggestionType type = GetSuggestionType(ddlShowByType);

                switch (type)
                {
                    case SuggestionType.General:
                        RedirectToOtherUrl("SuggestionsForSite.aspx?about=general");
                        break;
                    case SuggestionType.Design:
                        RedirectToOtherUrl("SuggestionsForSite.aspx?about=design");
                        break;
                    case SuggestionType.Features:
                        RedirectToOtherUrl("SuggestionsForSite.aspx?about=features");
                        break;
                    default:
                        throw new CommonCode.UIException(string.Format("Suggestion type = {0}, is not supported type", type));
                }
            }
        }

        [WebMethod]
        public static string WMSetSuggAsSpam(string suggID)
        {
            return CommonCode.WebMethods.SetSuggestionAsViolation(suggID);
        }

    }
}
