﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using BusinessLayer;
using DataAccess;

namespace UserInterface
{
    public partial class Advertisements : BasePage
    {
        private User currentUser = null;
        private bool advertisementHtmlPostbackDecoded = false;

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            if (IsPostBack == true && advertisementHtmlPostbackDecoded == false)
            {
                DecodeTbHtmlText();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetNeedsToBeLogged();
            CheckUser();
            if (currentUser == null)
            {
                throw new CommonCode.UIException("currentUser is null");
            }

            btnSubmit.Attributes.Add("onclick", string.Format("htmlEncodeReplaceText('{0}')", tbHtml.ClientID));   // raboti
            btnPreviewAd.Attributes.Add("onclick", string.Format("htmlEncodeReplaceText('{0}')", tbHtml.ClientID)); // raboti
            cExpireDate.Attributes.Add("onclick", string.Format("htmlEncodeReplaceText('{0}')", tbHtml.ClientID));

            ShowInfo();
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);
        }

        /// <summary>
        /// Checks if user is administrator or Global administrator , if not throws exception , used when event is triggered
        /// </summary>
        private void CheckUserFromEvents()
        {
            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsAdminOrGlobalAdmin(currentUser))
            {
                throw new CommonCode.UIException("current user cannot add or edit advertisements");
            }
        }

        /// <summary>
        /// Checks if user is administrator or Global administrator , if not redirrects to error page 
        /// </summary>
        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);
            if (currUser != null && businessUser.IsAdminOrGlobalAdmin(currUser))
            {
                currentUser = currUser;
            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, "This page is only for administrators");
            }
        }

        private void ShowInfo()
        {
            Title = "Manage Advertisements";

            BusinessSiteText siteText = new BusinessSiteText();
            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutAdverts");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblAbout.Text = aboutExtended.description; 
            }
            else
            {
                lblAbout.Text = "About Advertisements information not typed.";
            }

            if (IsPostBack == false)
            {
                FillCblAddEditOptions();
                FillDdlShowType();
            }

            if (IsPostBack)
            {
                FillTblAdverts();
            }

            lblCompany.Text = string.Format("{0} IDs :", Tools.GetStringWithCapital(Configuration.CompanyName));
        }


        /// <summary>
        /// Checks if advert is being edited
        /// </summary>
        private void CheckForEditParams()
        {
            string hfEditValue = hfAdToEdit.Value;
            if (!string.IsNullOrEmpty(hfEditValue))
            {
                long id = -1;
                if (long.TryParse(hfEditValue, out id))
                {
                    BusinessAdvertisement businessAdvert = new BusinessAdvertisement();
                    Advertisement editAdvert = businessAdvert.Get(objectContext, id);
                    if (editAdvert != null)
                    {
                        lblAddEditAdvert.Text = "Edit Advertisement";
                        tbHtml.Text = Server.HtmlDecode(editAdvert.html);
                        fuUpload.Enabled = false;
                        tbAdUrl.Text = editAdvert.targetUrl;

                        string strLinkIDs = businessAdvert.GetAdvertTypeLinksAsString(objectContext, editAdvert, AdvertisementsFor.Categories, true);
                        if (!string.IsNullOrEmpty(strLinkIDs))
                        {
                            lblCurrCatIDs.Visible = true;
                            lblCurrCatIDs.Text = string.Format("Linked with categories : {0}", strLinkIDs);
                        }

                        strLinkIDs = businessAdvert.GetAdvertTypeLinksAsString(objectContext, editAdvert, AdvertisementsFor.Companies, true);
                        if (!string.IsNullOrEmpty(strLinkIDs))
                        {
                            lblCurrCompanyIDs.Visible = true;
                            lblCurrCompanyIDs.Text = string.Format("Linked with {0}s : {1}", Configuration.CompanyName, strLinkIDs);
                        }

                        strLinkIDs = businessAdvert.GetAdvertTypeLinksAsString(objectContext, editAdvert, AdvertisementsFor.Products, true);
                        if (!string.IsNullOrEmpty(strLinkIDs))
                        {
                            lblCurrProductIDs.Visible = true;
                            lblCurrProductIDs.Text = string.Format("Linked with products : {0}", strLinkIDs);
                        }

                        if (!string.IsNullOrEmpty(editAdvert.expireDate.ToString()))
                        {
                            tbExpireDate.Text = Tools.ParseDateTimeToString(editAdvert.expireDate.Value);
                        }
                        cbActVisGen.Enabled = false;
                        tbAdInfo.Text = editAdvert.info;

                        btnCancel.Visible = true;
                    }
                    else
                    {
                        hfAdToEdit.Value = null;
                    }
                }
                else
                {
                    hfAdToEdit.Value = null;
                }
            }

        }


        private void FillDdlShowType()
        {
            ddlShowType.Items.Clear();

            ListItem productItem = new ListItem();
            productItem.Text = "product";
            productItem.Value = "product";
            ddlShowType.Items.Add(productItem);

            ListItem catItem = new ListItem();
            catItem.Text = "category";
            catItem.Value = "category";
            ddlShowType.Items.Add(catItem);

            ListItem compItem = new ListItem();
            compItem.Text = "company";
            compItem.Value = "company";
            ddlShowType.Items.Add(compItem);
        }

        private void FillCblAddEditOptions()
        {
            cbActVisGen.Items.Clear();

            ListItem activeItem = new ListItem();
            activeItem.Text = "active ";
            activeItem.Value = "0";
            cbActVisGen.Items.Add(activeItem);
            cbActVisGen.Items[0].Selected = true;

            ListItem generalItem = new ListItem();
            generalItem.Text = "general ";
            generalItem.Value = "1";
            cbActVisGen.Items.Add(generalItem);
        }

        /// <summary>
        /// Checks fields of Advertisement which is being edited
        /// </summary>
        /// <param name="currAdvert"></param>
        /// <returns>true if fields are in correct format and if there is actually edited field,otherwise false</returns>
        private Boolean CheckEditAdvertFields(Advertisement currAdvert, out bool linksChanged)
        {
            if (currAdvert == null)
            {
                throw new CommonCode.UIException("currAdvert is null");
            }

            linksChanged = false;
            Boolean passed = true;
            Boolean changedField = false;
            string error = "";

            if (tbHtml.Text != currAdvert.html)
            {
                if (!string.IsNullOrEmpty(tbHtml.Text))
                {
                    if (CommonCode.Validate.ValidateHtml(Server.HtmlDecode(tbHtml.Text ?? string.Empty)))
                    {
                        changedField = true;
                    }
                    else
                    {
                        passed = false;
                        error = "The HTML text is not well formed.";
                    }
                }
                else
                {
                    changedField = true;
                }
            }

            if (passed && tbAdUrl.Text != currAdvert.targetUrl)
            {
                if (string.IsNullOrEmpty(tbAdUrl.Text))
                {
                    passed = false;
                    error = "Type Advert target url";
                }
                else
                {
                    passed = CommonCode.Validate.ValidateSiteAdress(Tools.GetCorrectedUrl(tbAdUrl.Text), out error, false);
                    if (passed == false)
                    {
                        error = "Incorrect advert url format.";
                    }
                    else
                    {
                        changedField = true;
                    }
                }
            }

            if (passed)
            {
                passed = CheckAdvertLinksOnEdit(out error, out linksChanged, currAdvert);
                if (linksChanged == true)
                {
                    changedField = true;
                }
            }

            if (passed && tbExpireDate.Text != currAdvert.expireDate.ToString())
            {
                if (!string.IsNullOrEmpty(tbExpireDate.Text))
                {
                    DateTime expDate;
                    if (Tools.ParseStringToDateTime(tbExpireDate.Text, out expDate))
                    {
                        if (expDate.CompareTo(DateTime.UtcNow) > 0)
                        {
                            changedField = true;
                        }
                        else
                        {
                            error = "Expire Date must be in future.";
                            passed = false;
                        }
                    }
                    else
                    {
                        error = "Type Correct expire Date";
                        passed = false;
                    }
                }
                else
                {
                    changedField = true;
                }
            }

            if (passed && tbAdInfo.Text != currAdvert.info)
            {
                string adInfo = tbAdInfo.Text;

                if (CommonCode.Validate.ValidateDescription(0, Configuration.FieldsMaxDescriptionFieldLength
                    , ref adInfo, "description", out error, Configuration.FieldsDefMaxWordLength))
                {
                    changedField = true;
                }
                else
                {
                    passed = false;
                }
            }

            if (passed && !changedField)
            {
                passed = false;
                error = "There isnt changed field , change field or press Cancel.";
            }

            if (passed && string.IsNullOrEmpty(tbCategoryID.Text) && string.IsNullOrEmpty(tbCompanyID.Text)
                && string.IsNullOrEmpty(tbProductID.Text) && !currAdvert.general)
            {
                passed = false;
                error = "There should be atleast one ID typed because the Advert is not general.";
            }

            if (passed && string.IsNullOrEmpty(tbHtml.Text) && string.IsNullOrEmpty(currAdvert.adurl))
            {
                passed = false;
                error = "There should be HTML code when there isnt Advertisement File.";
            }

            if (!passed)
            {
                DecodeTbHtmlText();
            }

            lblError.Text = error;
            return passed;
        }

        /// <summary>
        /// Checks fields on new advert 
        /// </summary>
        /// <returns>True if fields are in correct format , and the ones needed are filled , otherwise false</returns>
        private Boolean CheckAddNewAdvertFields()
        {
            Boolean passed = true;

            string error = "";

            if (cbActVisGen.Items[0] == null || cbActVisGen.Items[1] == null)
            {
                throw new CommonCode.UIException("Items in the add cbActVisGen are nulls");
            }

            if (!string.IsNullOrEmpty(tbHtml.Text))
            {
                // checks if HTML is WELL FORMED 

                if (CommonCode.Validate.ValidateHtml(Server.HtmlDecode(tbHtml.Text ?? string.Empty)))
                {
                }
                else
                {
                    passed = false;
                    error = "The HTML text is not well formed.";
                }

            }
            else
            {
                if (passed && string.IsNullOrEmpty(tbAdUrl.Text))
                {
                    error = "Type Advert Target url.";
                    passed = false;
                }
                else if (passed)
                {
                    passed = CommonCode.Validate.ValidateSiteAdress(Tools.GetCorrectedUrl(tbAdUrl.Text), out error, false);
                    if (passed == false)
                    {
                        error = "Incorrect url format.";
                    }
                }
                if (!fuUpload.HasFile)
                {
                    error = "File need to be uplaoded.";
                    passed = false;
                }

            }

            if (passed)
            {
                passed = CheckAdvertLinksOnAddEdit(out error);
            }

            if (passed && !string.IsNullOrEmpty(tbExpireDate.Text))
            {
                DateTime expDate;
                if (Tools.ParseStringToDateTime(tbExpireDate.Text, out expDate) == true)
                {
                    if (expDate.CompareTo(DateTime.UtcNow) > 0)
                    {

                    }
                    else
                    {
                        error = "Expire Date must be in future.";
                        passed = false;
                    }
                }
                else
                {
                    error = "Type Correct expire Date";
                    passed = false;
                }
            }

            if (passed && !string.IsNullOrEmpty(tbAdInfo.Text))
            {
                if (!CommonCode.Validate.ValidateDescription(0, Configuration.FieldsMaxDescriptionFieldLength
                    , tbAdInfo.Text, "description", out error, Configuration.FieldsDefMaxWordLength))
                {
                    passed = false;
                }
            }

            if (passed && string.IsNullOrEmpty(tbCategoryID.Text) && string.IsNullOrEmpty(tbCompanyID.Text)
                && string.IsNullOrEmpty(tbProductID.Text) && !cbActVisGen.Items[1].Selected)
            {
                passed = false;
                error = "If there isnt typed atleast one ID , general should be checked";
            }

            if (passed && string.IsNullOrEmpty(tbHtml.Text) && !fuUpload.HasFile)
            {
                passed = false;
                error = "There should be either HTML code typed or Advertisement File uploaded";
            }

            if (!passed)
            {
                DecodeTbHtmlText();
            }

            lblError.Text = error;

            return passed;
        }

        /// <summary>
        /// Checks IF typed categories, companies and product IDs are in correct format, and if they exist
        /// </summary>
        private bool CheckAdvertLinksOnAddEdit(out string error)
        {
            bool passed = true;
            error = "";

            List<long> listIds = new List<long>();

            if (!string.IsNullOrEmpty(tbCategoryID.Text))
            {
                if (Tools.GetAdvertLinkIDsFromString(tbCategoryID.Text, out listIds))
                {
                    BusinessCategory businessCategory = new BusinessCategory();

                    foreach (long id in listIds)
                    {
                        Category currCat = businessCategory.Get(objectContext, id);
                        if (currCat == null)
                        {
                            error = string.Format("There is no Category with ID = {0}, or it is visible false.", id);
                            passed = false;
                            break;
                        }
                    }
                }
                else
                {
                    passed = false;
                    error = "Incorrect format on Categories";
                }
            }

            if (passed && !string.IsNullOrEmpty(tbCompanyID.Text))
            {
                if (Tools.GetAdvertLinkIDsFromString(tbCompanyID.Text, out listIds))
                {
                    BusinessCompany businessCompany = new BusinessCompany();

                    foreach (long id in listIds)
                    {
                        Company currComp = businessCompany.GetCompany(objectContext, id);

                        if (currComp == null)
                        {
                            error = string.Format("There is no {0} with ID = {1}, or it is visible false."
                                , Tools.GetStringWithCapital(Configuration.CompanyName), id);
                            passed = false;
                            break;
                        }
                    }
                }
                else
                {
                    passed = false;
                    error = string.Format("Incorrect format on {0}s", Tools.GetStringWithCapital(Configuration.CompanyName));
                }
            }

            if (passed && !string.IsNullOrEmpty(tbProductID.Text))
            {

                if (Tools.GetAdvertLinkIDsFromString(tbProductID.Text, out listIds))
                {

                    BusinessProduct businessProduct = new BusinessProduct();

                    foreach (long id in listIds)
                    {
                        Product currProduct = businessProduct.GetProductByID(objectContext, id);

                        if (currProduct == null)
                        {
                            error = string.Format("There is no Product with ID = {0}, or it is visible false.", id);
                            passed = false;
                            break;
                        }
                    }
                }
                else
                {
                    passed = false;
                    error = "Incorrect format on Products";
                }
            }

            return passed;
        }


        /// <summary>
        /// Checks for id`s format, existing, and if there are different from current advert`s links
        /// </summary>
        private bool CheckAdvertLinksOnEdit(out string error, out bool change, Advertisement currAdvert)
        {
            if (currAdvert == null)
            {
                throw new CommonCode.UIException("currAdvert is null");
            }

            bool passed = true;
            change = false;
            error = "";

            if (CheckAdvertLinksOnAddEdit(out error))
            {
                BusinessAdvertisement businessAdvertisement = new BusinessAdvertisement();

                List<long> newIDs = new List<long>();
                List<long> oldIDs = new List<long>();

                oldIDs = businessAdvertisement.GetAdvertTypeLinksAsList
                            (objectContext, currAdvert, AdvertisementsFor.Categories);

                if (!string.IsNullOrEmpty(tbCategoryID.Text))
                {
                    if (Tools.GetAdvertLinkIDsFromString(tbCategoryID.Text, out newIDs))
                    {
                        if (Tools.AreTwoListsDiffent(newIDs, oldIDs))
                        {
                            change = true;
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format("tbCategoryID.Text is in invalid format, there is check before this one."));
                    }
                }
                else
                {
                    if (oldIDs.Count > 0)
                    {
                        change = true;
                    }
                }


                oldIDs = businessAdvertisement.GetAdvertTypeLinksAsList
                           (objectContext, currAdvert, AdvertisementsFor.Companies);
                if (change == false && !string.IsNullOrEmpty(tbCompanyID.Text))
                {
                    if (Tools.GetAdvertLinkIDsFromString(tbCompanyID.Text, out newIDs))
                    {
                        if (Tools.AreTwoListsDiffent(newIDs, oldIDs))
                        {
                            change = true;
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format("tbCompanyID.Text is in invalid format, there is check before this one."));
                    }
                }
                else
                {
                    if (oldIDs.Count > 0)
                    {
                        change = true;
                    }
                }


                oldIDs = businessAdvertisement.GetAdvertTypeLinksAsList
                           (objectContext, currAdvert, AdvertisementsFor.Products);
                if (change == false && !string.IsNullOrEmpty(tbProductID.Text))
                {
                    if (Tools.GetAdvertLinkIDsFromString(tbProductID.Text, out newIDs))
                    {
                        if (Tools.AreTwoListsDiffent(newIDs, oldIDs))
                        {
                            change = true;
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format("tbProductID.Text is in invalid format, there is check before this one."));
                    }
                }
                else
                {
                    if (oldIDs.Count > 0)
                    {
                        change = true;
                    }
                }
            }
            else
            {
                passed = false;
            }

            return passed;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            CheckUserFromEvents();

            phAddEditAdvert.Visible = true;
            phAddEditAdvert.Controls.Add(lblError);

            BusinessAdvertisement businessAdvertisement = new BusinessAdvertisement();

            if (string.IsNullOrEmpty(hfAdToEdit.Value))
            {
                // submit new advertisement part
                if (CheckAddNewAdvertFields())
                {
                    string ErrorDescription = string.Empty;
                    byte[] fileBytes;
                    string fileName = string.Empty;
                    string fileType = string.Empty;

                    Boolean uploadPassed = true;

                    if (fuUpload.HasFile)
                    {
                        fileBytes = fuUpload.FileBytes;
                        fileName = fuUpload.FileName;
                        fileType = System.IO.Path.GetExtension(fileName);

                        bool fileOk = BusinessAdvertisement.IsValidAdvertisementFile(fileName, fileBytes, ref ErrorDescription);
                        if (!fileOk)
                        {
                            uploadPassed = false;
                            lblError.Text = ErrorDescription;
                        }
                    }

                    if (uploadPassed)
                    {
                        Advertisement newAdvertisement = new Advertisement();

                        newAdvertisement.html = tbHtml.Text;
                        newAdvertisement.info = tbAdInfo.Text;
                        newAdvertisement.visible = true;
                        newAdvertisement.adpath = null;
                        newAdvertisement.adurl = null;
                        newAdvertisement.targetUrl = CommonCode.UiTools.GetCorrectedUrl(tbAdUrl.Text);

                        newAdvertisement.dateCreated = DateTime.UtcNow;
                        newAdvertisement.CreatedBy = Tools.GetUserID(objectContext, currentUser);
                        newAdvertisement.lastModified = newAdvertisement.dateCreated;
                        newAdvertisement.LastModifiedBy = newAdvertisement.CreatedBy;

                        if (cbActVisGen.Items[0] != null && cbActVisGen.Items[0].Selected)
                        {
                            newAdvertisement.active = true;
                        }
                        else
                        {
                            newAdvertisement.active = false;
                        }

                        if (cbActVisGen.Items[1] != null && cbActVisGen.Items[1].Selected)
                        {
                            newAdvertisement.general = true;
                        }
                        else
                        {
                            newAdvertisement.general = false;
                        }

                        if (string.IsNullOrEmpty(tbExpireDate.Text))
                        {
                            newAdvertisement.expireDate = null;
                        }
                        else
                        {
                            DateTime expDate;
                            if (Tools.ParseStringToDateTime(tbExpireDate.Text, out expDate))
                            {
                                newAdvertisement.expireDate = expDate;
                            }
                            else
                            {
                                throw new CommonCode.UIException(string.Format("Couldnt parse tbExpireDate.Text to date , user id = {0}", currentUser.ID));
                            }
                        }

                        businessAdvertisement.Add(objectContext, businessLog, newAdvertisement, currentUser);

                        if (!string.IsNullOrEmpty(tbCategoryID.Text) || !string.IsNullOrEmpty(tbCompanyID.Text)
                        || !string.IsNullOrEmpty(tbProductID.Text))
                        {
                            businessAdvertisement.AddToNewAdvertisementLinks(objectContext, newAdvertisement,
                                tbCategoryID.Text, tbCompanyID.Text, tbProductID.Text);
                        }

                        if (fuUpload.HasFile)
                        {
                            string path = "";
                            string url = "";

                            CommonCode.ImagesAndAdverts.GenerateAdvertisementFileNamePathUrl(fileType, newAdvertisement, out path, out url);
                            fuUpload.SaveAs(path);
                            businessAdvertisement.ChangeAdvertisementFilePathAndUrl(objectContext, newAdvertisement, path, url);
                        }

                        phAddEditAdvert.Visible = false;
                        Cancel();
                        FillTblAdverts();
                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Advertisement added!");
                    }
                }
            }
            else
            {
                // edit advertisement part

                long id = -1;
                if (long.TryParse(hfAdToEdit.Value, out id))
                {
                    Advertisement currAdvert = businessAdvertisement.Get(objectContext, id);
                    if (currAdvert == null)
                    {
                        throw new CommonCode.UIException(string.Format("Theres np Advert to edit with ID = {0}", id));
                    }

                    bool linksChanged = false;

                    if (CheckEditAdvertFields(currAdvert, out linksChanged))
                    {
                        if (tbHtml.Text != currAdvert.html)
                        {
                            businessAdvertisement.ChangeField(objectContext, businessLog, currentUser, currAdvert, "html", tbHtml.Text);
                        }

                        string newUrl = CommonCode.UiTools.GetCorrectedUrl(tbAdUrl.Text);

                        if (newUrl != currAdvert.targetUrl)
                        {
                            businessAdvertisement.ChangeField(objectContext, businessLog, currentUser, currAdvert, "targetUrl", tbAdUrl.Text);
                        }

                        if (linksChanged)
                        {
                            businessAdvertisement.UpdateAdvertisementLinks(objectContext, currAdvert, tbCategoryID.Text,
                                tbCompanyID.Text, tbProductID.Text, businessLog, currentUser);
                        }

                        if (tbExpireDate.Text != currAdvert.expireDate.ToString())
                        {
                            if (string.IsNullOrEmpty(tbExpireDate.Text))
                            {
                                businessAdvertisement.ChangeField(objectContext, businessLog, currentUser, currAdvert, "expireDate", null);
                            }
                            else
                            {
                                DateTime expDate;
                                if (Tools.ParseStringToDateTime(tbExpireDate.Text, out expDate))
                                {
                                    businessAdvertisement.ChangeField(objectContext, businessLog, currentUser, currAdvert, "expireDate", Tools.ParseDateTimeToString(expDate));
                                }
                                else
                                {
                                    throw new CommonCode.UIException(string.Format("Couldnt parse tbExpireDate.Text to date , user id = {0}", currentUser.ID));
                                }
                            }
                        }

                        if (tbAdInfo.Text != currAdvert.info)
                        {
                            businessAdvertisement.ChangeField(objectContext, businessLog, currentUser, currAdvert, "info", tbAdInfo.Text);
                        }

                        Cancel();
                        FillTblAdverts();
                        phAddEditAdvert.Visible = false;
                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Advertisement updated!");
                    }

                }
                else
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse hfAdToEdit.Value to long , user id = {0}", currentUser.ID));
                }

            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        /// <summary>
        /// Clear edit advert parameters , and returns add advert fields to normal form
        /// </summary>
        private void Cancel()
        {
            btnCancel.Visible = true;
            lblAddEditAdvert.Text = "Add new Advertisement";
            fuUpload.Enabled = true;
            cbActVisGen.Enabled = true;

            tbHtml.Text = "";
            tbAdUrl.Text = "";
            tbCategoryID.Text = "";
            tbCompanyID.Text = "";
            tbProductID.Text = "";
            tbExpireDate.Text = "";
            tbAdInfo.Text = "";

            hfAdToEdit.Value = null;

            lblCurrCatIDs.Visible = false;
            lblCurrCompanyIDs.Visible = false;
            lblCurrProductIDs.Visible = false;

            CheckForEditParams();
        }

        private void FillShowAttributes(string type, long number, long id,
            int visible, int general, int active, int level)
        {
            Session["AdvertType"] = type;
            Session["AdvertNumber"] = number;
            Session["AdvertTypeId"] = id;
            Session["AdvertVisible"] = visible;             // 0 - all , 1 - yes , 2 - no
            Session["AdvertGeneral"] = general;             // 0 - all , 1 - yes , 2 - no
            Session["AdvertActive"] = active;               // 0 - all , 1 - yes , 2 - no
            Session["AdvertToLevel"] = level;               // -1 - not checking, 1 - adverts only for chosen type, 2 - levels 1 and 2,
            //0 - all adverts which can be shown for type
        }

        private void ClearShowParams()
        {
            Session["AdvertType"] = null;
            Session["AdvertNumber"] = null;
            Session["AdvertTypeId"] = null;
            Session["AdvertShowAll"] = null;
            Session["AdvertVisible"] = null;
            Session["AdvertGeneral"] = null;
            Session["AdvertActive"] = null;
            Session["AdvertToLevel"] = null;
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            phShowAds.Visible = true;
            phShowAds.Controls.Add(lblError);
            string error = "";

            if (CommonCode.Validate.ValidateLong(tbShowLastNum.Text, out error))
            {
                long number = -1;
                if (!long.TryParse(tbShowLastNum.Text, out number))
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse tbShowLastNum.Text to long , user id = {0}", currentUser.ID));
                }

                int visible = 0;
                int general = 0;
                int active = 0;

                if (!int.TryParse(ddVisible.SelectedValue, out visible))
                {
                    throw new CommonCode.UIException(string.Format("Couldnt parse ddVisible.SelectedValue to INT, user id = {0}", currentUser.ID));
                }
                if (!int.TryParse(ddGeneal.SelectedValue, out general))
                {
                    throw new CommonCode.UIException(string.Format("Couldnt parse ddGeneal.SelectedValue to INT, user id = {0}", currentUser.ID));
                }
                if (!int.TryParse(ddActive.SelectedValue, out active))
                {
                    throw new CommonCode.UIException(string.Format("Couldnt parse ddActive.SelectedValue to INT, user id = {0}", currentUser.ID));
                }
                if (visible < 0 || visible > 2)
                {
                    throw new CommonCode.UIException(string.Format("ddVisible.SelectedValue is < 0 or > 2, user id = {0}", currentUser.ID));
                }
                if (general < 0 || general > 2)
                {
                    throw new CommonCode.UIException(string.Format("ddGeneal.SelectedValue is < 0 or > 2, user id = {0}", currentUser.ID));
                }
                if (active < 0 || active > 2)
                {
                    throw new CommonCode.UIException(string.Format("ddActive.SelectedValue is < 0 or > 2, user id = {0}", currentUser.ID));
                }

                phShowAds.Visible = false;

                FillShowAttributes("all", number, 1, visible, general, active, 0);
                FillTblAdverts();
            }

            lblError.Text = error;
        }

        protected void btnShowAdvert_Click(object sender, EventArgs e)
        {
            phShowAd.Visible = true;
            phShowAd.Controls.Add(lblError);
            string error = "";

            if (CommonCode.Validate.ValidateLong(tbShowID.Text, out error))
            {
                long id = -1;
                if (!long.TryParse(tbShowID.Text, out id))
                {
                    throw new BusinessException(string.Format("couldnt parse tbShowID.Text to long , user id = {0}", currentUser.ID));
                }

                if (string.IsNullOrEmpty(ddlShowType.SelectedValue))
                {
                    throw new BusinessException(string.Format("ddlShowType.SelectedValue is empty or null , user id = {0}", currentUser.ID));
                }


                bool getAdverts = true;

                switch (ddlShowType.SelectedValue)
                {
                    case "product":
                        BusinessProduct businessProduct = new BusinessProduct();
                        Product currProduct = businessProduct.GetProductByIDWV(objectContext, id);
                        if (currProduct == null)
                        {
                            error = "No such product";
                            getAdverts = false;
                        }

                        break;
                    case "company":
                        BusinessCompany bCompany = new BusinessCompany();
                        Company currCompany = bCompany.GetCompanyWV(objectContext, id);
                        if (currCompany == null)
                        {
                            error = "No such company";
                            getAdverts = false;
                        }

                        break;
                    case "category":
                        BusinessCategory bCategory = new BusinessCategory();
                        Category currCategory = bCategory.GetWithoutVisible(objectContext, id);
                        if (currCategory == null)
                        {
                            error = "No such category";
                            getAdverts = false;
                        }

                        break;
                    default: throw new CommonCode.UIException(string.Format("ddlShowType.SelectedValue = {0} is not supported type, user id : {1}"
                        , ddlShowType.SelectedValue, currentUser.ID));
                }

                if (getAdverts == true)
                {
                    phShowAd.Visible = false;

                    int toLevel = 0;
                    if (int.TryParse(rblAdvertsLevel.SelectedValue, out toLevel))
                    {
                        FillShowAttributes(ddlShowType.SelectedValue, 100, id, 0, 0, 0, toLevel);
                        FillTblAdverts();
                    }
                    else
                    {
                        throw new BusinessException(string.Format("Couldn`t parse ddlShowType.SelectedValue to int , user id = {0}", currentUser.ID));
                    }
                }

            }

            lblError.Text = error;
        }

        private List<Advertisement> GetAdvertsFromParams()
        {
            object type = Session["AdvertType"];
            object number = Session["AdvertNumber"];
            object id = Session["AdvertTypeId"];
            object visible = Session["AdvertVisible"];
            object general = Session["AdvertGeneral"];
            object active = Session["AdvertActive"];
            object level = Session["AdvertToLevel"];

            List<Advertisement> adverts = null;

            if (type != null && number != null && id != null && visible != null
                && general != null && active != null && level != null)
            {
                BusinessAdvertisement businessAdvert = new BusinessAdvertisement();

                string strType = type as string;
                long lngNumber = -1;
                if (!long.TryParse(number.ToString(), out lngNumber))
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse Session['AdvertNumber'] to long , user id = {0}", currentUser.ID));
                }
                else
                {
                    if (lngNumber < 1)
                    {
                        throw new CommonCode.UIException(string.Format("lngNumber is < 1 after parsing , user id = {0}", currentUser.ID));
                    }
                }

                long lngId = -1;
                if (!long.TryParse(id.ToString(), out lngId))
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse Session['AdvertTypeId'] to long , user id = {0}", currentUser.ID));
                }
                else
                {
                    if (lngId < 1)
                    {
                        throw new CommonCode.UIException(string.Format("lngId is < 1 after parsing , user id = {0}", currentUser.ID));
                    }
                }


                int intVisible = 0;
                if (!int.TryParse(visible.ToString(), out intVisible))
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse Session['AdvertVisible'] to int , user id = {0}", currentUser.ID));
                }
                int intGeneral = 0;
                if (!int.TryParse(general.ToString(), out intGeneral))
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse Session['AdvertGeneral'] to int , user id = {0}", currentUser.ID));
                }
                int intActive = 0;
                if (!int.TryParse(active.ToString(), out intActive))
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse Session['AdvertActive'] to int , user id = {0}", currentUser.ID));
                }

                int intLevel = 0;
                if (!int.TryParse(level.ToString(), out intLevel))
                {
                    throw new CommonCode.UIException(string.Format("couldnt parse Session['AdvertToLevel'] to int , user id = {0}", currentUser.ID));
                }
                else
                {
                    if (intLevel < 0 || intLevel > 3)
                    {
                        throw new CommonCode.UIException(string.Format("invalid Level chose for adverts = {0} , user id : {1}"
                            , intLevel, currentUser.ID));
                    }
                }

                if (intLevel < 1)
                {
                    adverts = businessAdvert.GetAdvertisements(objectContext, strType, lngNumber, lngId, intVisible, intGeneral, intActive);
                }
                else
                {
                    int pAds, sAds, tAds = 0;

                    adverts = CommonCode.ImagesAndAdverts.GetAdvertisementsForType(objectContext, strType, lngId, 100, intLevel, out pAds, out sAds, out tAds);
                }

                if (adverts.Count < 1)
                {
                    ClearShowParams();
                }
            }
            else
            {
                ClearShowParams();
            }

            return adverts;

        }

        private void FillTblAdverts()
        {
            tblAdverts.Rows.Clear();

            List<Advertisement> adverts = GetAdvertsFromParams();

            if (adverts != null)
            {
                if (adverts.Count > 0)
                {
                    foreach (Advertisement advert in adverts)
                    {
                        TableRow firstRow = new TableRow();
                        tblAdverts.Rows.Add(firstRow);

                        TableCell firstCell = new TableCell();
                        firstRow.Cells.Add(firstCell);
                        firstCell.CssClass = "commentsTD";

                        Table newTable = new Table();
                        newTable.GridLines = GridLines.Both;
                        firstCell.Controls.Add(newTable);
                        newTable.Width = Unit.Percentage(100);

                        TableRow newRow = new TableRow();
                        newTable.Rows.Add(newRow);

                        //// id , url , date added , added by , category , company , product , expire date , general , active , visible

                        if (advert.CreatedByReference.IsLoaded)
                        {
                            advert.CreatedByReference.Load();
                        }

                        TableCell adInfoCell = new TableCell();
                        newRow.Cells.Add(adInfoCell);
                        adInfoCell.Width = Unit.Percentage(30);
                        adInfoCell.VerticalAlign = VerticalAlign.Top;

                        ///
                        GetAdvertInfoTable(advert, adInfoCell);
                        ///

                        TableCell htmlCell = new TableCell();
                        newRow.Cells.Add(htmlCell);

                        htmlCell.VerticalAlign = VerticalAlign.Top;
                        htmlCell.Width = Unit.Pixel(10); 

                        TextBox htmlBox = new TextBox();
                        htmlCell.Controls.Add(htmlBox);
                        htmlBox.TextMode = TextBoxMode.MultiLine;
                        htmlBox.Rows = 5;
                        htmlBox.Columns = 50;
                        htmlBox.Enabled = false;
                        if (!string.IsNullOrEmpty(advert.html))
                        {
                            htmlBox.Text = Server.HtmlDecode(advert.html);
                        }

                        Label adInfo = new Label();
                        adInfo.Text = advert.info;
                        htmlCell.Controls.Add(adInfo);

                        TableCell fileCell = new TableCell();
                        newRow.Cells.Add(fileCell);
                        fileCell.HorizontalAlign = HorizontalAlign.Center;
                        if (!string.IsNullOrEmpty(advert.adurl))
                        {
                            if (advert.adurl.EndsWith(".swf"))
                            {
                                fileCell.Controls.Add(CommonCode.UiTools.GetSwfHtmlCode(advert.adurl));
                            }
                            else
                            {
                                Image newImage = new Image();
                                fileCell.Controls.Add(newImage);
                                newImage.ImageUrl = advert.adurl;
                            }
                        }

                        int colums = newRow.Cells.Count;

                        TableRow editRow = new TableRow();
                        editRow.Attributes.Add("adID", advert.ID.ToString());
                        newTable.Rows.Add(editRow);

                        TableCell editCell = new TableCell();
                        editCell.ColumnSpan = colums;
                        editRow.Cells.Add(editCell);

                        if (advert.visible)
                        {
                            Button editBtn = new Button();
                            editCell.Controls.Add(editBtn);
                            editBtn.ID = string.Format("edit{0}", advert.ID);
                            editBtn.Click += new EventHandler(EditAdvert_Click);
                            editBtn.Text = "edit";

                            Button gnrBtn = new Button();
                            editCell.Controls.Add(gnrBtn);
                            gnrBtn.ID = string.Format("gnr{0}", advert.ID);
                            gnrBtn.Click += new EventHandler(MakeGeneralOrNoAdvert_Click);
                            if (advert.general)
                            {
                                gnrBtn.Text = "remove general";
                            }
                            else
                            {
                                gnrBtn.Text = "make general";
                            }

                            Button actBtn = new Button();
                            editCell.Controls.Add(actBtn);
                            actBtn.ID = string.Format("active{0}", advert.ID);
                            actBtn.Click += new EventHandler(MakeActiveOrNoAdvert_Click);
                            if (advert.active)
                            {
                                actBtn.Text = "make inactive";
                            }
                            else
                            {
                                actBtn.Text = "make active";
                            }
                            if (advert.expireDate == null || DateTime.Compare(advert.expireDate.Value, DateTime.UtcNow) > 0)
                            {
                                actBtn.Enabled = true;
                            }
                            else
                            {
                                actBtn.Enabled = false;
                            }
                        }

                        if (Configuration.AdvertsCanAdvertBeUndeleted)
                        {
                            Button visibBtn = new Button();
                            editCell.Controls.Add(visibBtn);
                            visibBtn.ID = string.Format("visib{0}", advert.ID);
                            visibBtn.Click += new EventHandler(DeleteOrUnDeleteAdvert_Click);
                            if (advert.visible)
                            {
                                visibBtn.Text = "Delete";
                            }
                            else
                            {
                                visibBtn.Text = "Make Visible";
                            }
                        }
                        else if (advert.visible)
                        {
                            Button visibBtn = new Button();
                            editCell.Controls.Add(visibBtn);
                            visibBtn.ID = string.Format("visib{0}", advert.ID);
                            visibBtn.Click += new EventHandler(DeleteOrUnDeleteAdvert_Click);
                            visibBtn.Text = "Delete";
                        }

                    }
                }
                else
                {
                    TableRow lastRow = new TableRow();
                    tblAdverts.Rows.Add(lastRow);

                    TableCell lastCell = new TableCell();
                    lastRow.Cells.Add(lastCell);
                    lastCell.Text = "No Advertisements which statisfy theese conditions.";
                }
            }
        }

        private void GetAdvertInfoTable(Advertisement advert, TableCell adInfoCell)
        {
            Table adInfoTable = new Table();
            adInfoCell.Controls.Add(adInfoTable);
            adInfoTable.Width = Unit.Percentage(100);

            TableRow idRow = new TableRow();
            adInfoTable.Rows.Add(idRow);

            TableCell idCell = new TableCell();
            idRow.Cells.Add(idCell);
            idCell.Text = string.Format("ID : {0}", advert.ID.ToString());

            TableRow urlRow = new TableRow();
            adInfoTable.Rows.Add(urlRow);

            TableCell urlCell = new TableCell();
            urlRow.Cells.Add(urlCell);

            urlCell.Controls.Add(CommonCode.UiTools.GetLabelWithText("URL : ", false));

            if (string.IsNullOrEmpty(advert.targetUrl))
            {
                urlCell.Controls.Add(CommonCode.UiTools.GetLabelWithText("none", false));
            }
            else
            {
                HyperLink urlLink = new HyperLink();
                urlCell.Controls.Add(urlLink);
                urlLink.Target = "_blank";
                urlLink.Text = "click here";
                urlLink.NavigateUrl = advert.targetUrl;
            }

            TableRow dateRow = new TableRow();
            adInfoTable.Rows.Add(dateRow);

            TableCell dateCell = new TableCell();
            dateRow.Cells.Add(dateCell);
            dateCell.CssClass = "commentsDate";
            dateCell.Text = string.Format("Date added : {0}", CommonCode.UiTools.DateTimeToLocalShortDateString(advert.dateCreated));

            TableRow createdByROw = new TableRow();
            adInfoTable.Rows.Add(createdByROw);

            if (!advert.CreatedByReference.IsLoaded)
            {
                advert.CreatedByReference.Load();
            }

            TableCell createdByCell = new TableCell();
            createdByROw.Cells.Add(createdByCell);
            Label byLbl = new Label();
            byLbl.Text = "Added by : ";
            createdByCell.Controls.Add(byLbl);
            createdByCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                (Tools.GetUserFromUserDatabase(userContext, advert.CreatedBy)));

            TableRow catRow = new TableRow();
            adInfoTable.Rows.Add(catRow);

            TableCell CatCell = new TableCell();
            catRow.Cells.Add(CatCell);


            BusinessAdvertisement businessAdvertisement = new BusinessAdvertisement();
            string linkIDs = string.Empty;

            linkIDs = businessAdvertisement.GetAdvertTypeLinksAsString(objectContext, advert, AdvertisementsFor.Categories, true);
            if (!string.IsNullOrEmpty(linkIDs))
            {
                Label catLbl = new Label();
                catLbl.Text = string.Format("Category links : {0}", linkIDs);
                CatCell.Controls.Add(catLbl);
            }
            else
            {
                CatCell.Text = "No category links";
            }

            TableRow compRow = new TableRow();
            adInfoTable.Rows.Add(compRow);

            TableCell CompCell = new TableCell();
            compRow.Cells.Add(CompCell);

            linkIDs = businessAdvertisement.GetAdvertTypeLinksAsString(objectContext, advert, AdvertisementsFor.Companies, true);
            if (!string.IsNullOrEmpty(linkIDs))
            {
                Label compLbl = new Label();
                compLbl.Text = string.Format("Company links : {0}", linkIDs);
                CompCell.Controls.Add(compLbl);
            }
            else
            {
                CompCell.Text = string.Format("No company links");
            }

            TableRow prodRow = new TableRow();
            adInfoTable.Rows.Add(prodRow);

            TableCell ProdCell = new TableCell();
            prodRow.Cells.Add(ProdCell);

            linkIDs = businessAdvertisement.GetAdvertTypeLinksAsString(objectContext, advert, AdvertisementsFor.Products, true);
            if (!string.IsNullOrEmpty(linkIDs))
            {
                Label prodLbl = new Label();
                prodLbl.Text = string.Format("Product links : {0}", linkIDs);
                ProdCell.Controls.Add(prodLbl);
            }
            else
            {
                ProdCell.Text = "No product links";
            }

            TableRow expDRow = new TableRow();
            adInfoTable.Rows.Add(expDRow);

            TableCell expDateCell = new TableCell();
            expDRow.Cells.Add(expDateCell);
            expDateCell.CssClass = "commentsDate";
            if (advert.expireDate != null)
            {
                expDateCell.Text = string.Format("Expire date : {0}", CommonCode.UiTools.DateTimeToLocalShortDateString(advert.expireDate.Value));
            }
            else
            {
                expDateCell.Text = "Expire date : none";
            }

            TableRow genRow = new TableRow();
            adInfoTable.Rows.Add(genRow);

            TableCell generalCell = new TableCell();
            genRow.Cells.Add(generalCell);
            generalCell.CssClass = "searchPageComments";
            generalCell.Text = string.Format("General : {0}", advert.general.ToString());

            TableRow actRow = new TableRow();
            adInfoTable.Rows.Add(actRow);

            TableCell activeCell = new TableCell();
            actRow.Cells.Add(activeCell);
            activeCell.CssClass = "searchPageRatings";
            activeCell.Text = string.Format("Active : {0}", advert.active.ToString());

            TableRow visRow = new TableRow();
            adInfoTable.Rows.Add(visRow);

            TableCell visibCell = new TableCell();
            visRow.Cells.Add(visibCell);
            visibCell.Text = string.Format("Visible : {0}", advert.visible.ToString());
        }

        void EditAdvert_Click(object sender, EventArgs e)
        {
            CheckUserFromEvents();

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                TableCell tblCell = btnSender.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long advertID = 0;
                        string advertIdStr = tblRow.Attributes["adID"];
                        if (long.TryParse(advertIdStr, out advertID))
                        {
                            BusinessAdvertisement businessAdvert = new BusinessAdvertisement();
                            Advertisement currAdvert = businessAdvert.Get(objectContext, advertID);
                            if (currAdvert == null)
                            {
                                throw new CommonCode.UIException("currAdvert is null");
                            }

                            hfAdToEdit.Value = currAdvert.ID.ToString();
                            CheckForEditParams();

                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("couldnt parse tblRow.Attributes to long , user id = {0}", currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldnt get parent row");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Couldnt get parent cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }

        }

        void DeleteOrUnDeleteAdvert_Click(object sender, EventArgs e)
        {
            CheckUserFromEvents();

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                TableCell tblCell = btnSender.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long advertID = 0;
                        string advertIdStr = tblRow.Attributes["adID"];
                        if (long.TryParse(advertIdStr, out advertID))
                        {
                            BusinessAdvertisement businessAdvert = new BusinessAdvertisement();
                            Advertisement currAdvert = businessAdvert.Get(objectContext, advertID);
                            if (currAdvert == null)
                            {
                                throw new CommonCode.UIException(string.Format("Theres no advert ID = {0} , user id = {1}", advertID, currentUser.ID));
                            }

                            if (Configuration.AdvertsCanAdvertBeUndeleted)
                            {
                                if (currAdvert.visible)
                                {
                                    businessAdvert.ChangeActiveVisibleOrGeneral(objectContext, businessLog, currentUser, currAdvert, "visible", false);
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Advertisement deleted!");
                                }
                                else
                                {
                                    businessAdvert.ChangeActiveVisibleOrGeneral(objectContext, businessLog, currentUser, currAdvert, "visible", true);
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Advertisement undeleted!");
                                }
                            }
                            else
                            {
                                if (currAdvert.visible)
                                {
                                    businessAdvert.ChangeActiveVisibleOrGeneral(objectContext, businessLog, currentUser, currAdvert, "visible", false);
                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Advertisement deleted!");
                                }
                                else
                                {
                                    throw new CommonCode.UIException(string.Format("Advert ID = {0} cannot be undeleted , user id = {1}", currAdvert.ID, currentUser.ID));
                                }
                            }

                            FillTblAdverts();
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("couldnt parse tblRow.Attributes to long , user id = {0}", currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldnt get parent row");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Couldnt get parent cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }

        }

        void MakeActiveOrNoAdvert_Click(object sender, EventArgs e)
        {
            CheckUserFromEvents();

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                TableCell tblCell = btnSender.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long advertID = 0;
                        string advertIdStr = tblRow.Attributes["adID"];
                        if (long.TryParse(advertIdStr, out advertID))
                        {
                            BusinessAdvertisement businessAdvert = new BusinessAdvertisement();
                            Advertisement currAdvert = businessAdvert.Get(objectContext, advertID);
                            if (currAdvert == null)
                            {
                                throw new CommonCode.UIException(string.Format("Theres no advert id = {0} , user id = {1}", advertID, currentUser.ID));
                            }

                            if (currAdvert.active)
                            {
                                businessAdvert.ChangeActiveVisibleOrGeneral(objectContext, businessLog, currentUser, currAdvert, "active", false);
                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Advertisement set to not active!");
                            }
                            else
                            {
                                businessAdvert.ChangeActiveVisibleOrGeneral(objectContext, businessLog, currentUser, currAdvert, "active", true);
                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Advertisment set to active!");
                            }
                            FillTblAdverts();

                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("couldnt parse tblRow.Attributes to long , user id = {0}", currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldnt get parent row");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Couldnt get parent cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }

        }

        void MakeGeneralOrNoAdvert_Click(object sender, EventArgs e)
        {
            CheckUserFromEvents();

            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                TableCell tblCell = btnSender.Parent as TableCell;
                if (tblCell != null)
                {
                    TableRow tblRow = tblCell.Parent as TableRow;
                    if (tblRow != null)
                    {
                        long advertID = 0;
                        string advertIdStr = tblRow.Attributes["adID"];
                        if (long.TryParse(advertIdStr, out advertID))
                        {
                            BusinessAdvertisement businessAdvert = new BusinessAdvertisement();
                            Advertisement currAdvert = businessAdvert.Get(objectContext, advertID);
                            if (currAdvert == null)
                            {
                                throw new CommonCode.UIException(string.Format("Theres no advert id = {0} , user id = {1}", advertID, currentUser.ID));
                            }

                            if (currAdvert.general)
                            {
                                businessAdvert.ChangeActiveVisibleOrGeneral(objectContext, businessLog, currentUser, currAdvert, "general", false);
                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Advertisement set as not general!");
                            }
                            else
                            {
                                businessAdvert.ChangeActiveVisibleOrGeneral(objectContext, businessLog, currentUser, currAdvert, "general", true);
                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Advertisement set as general!");
                            }
                            FillTblAdverts();

                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("couldnt parse tblRow.Attributes to long , user id = {0}", currentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldnt get parent row");
                    }
                }
                else
                {
                    throw new CommonCode.UIException("Couldnt get parent cell");
                }
            }
            else
            {
                throw new CommonCode.UIException("Couldnt get button");
            }
        }

        protected void btnPreviewAd_Click(object sender, EventArgs e)
        {
            phAddEditAdvert.Visible = true;
            phAddEditAdvert.Controls.Add(lblError);
            string error = "";

            if (string.IsNullOrEmpty(tbHtml.Text))
            {
                if (!string.IsNullOrEmpty(hfAdToEdit.Value))
                {
                    long id = -1;
                    if (long.TryParse(hfAdToEdit.Value, out id))
                    {
                        BusinessAdvertisement businessAdvert = new BusinessAdvertisement();
                        Advertisement advert = businessAdvert.Get(objectContext, id);
                        if (advert == null)
                        {
                            throw new CommonCode.UIException(string.Format("Theres no advert id = {0} , user id = {1}", id, currentUser.ID));
                        }

                        if (!string.IsNullOrEmpty(tbAdUrl.Text))
                        {
                            if (advert.adurl != null)
                            {
                                phPreviewAdvert.Visible = true;

                                if (advert.adurl.EndsWith(".swf"))
                                {
                                    phPreviewAdvert.Controls.Add(CommonCode.UiTools.GetSwfHtmlCode(advert.adurl));
                                }
                                else
                                {
                                    HyperLink newLink = new HyperLink();
                                    phPreviewAdvert.Controls.Add(newLink);

                                    newLink.NavigateUrl = tbAdUrl.Text;
                                    newLink.ImageUrl = advert.adurl;
                                }

                                phAddEditAdvert.Visible = false;
                            }
                            else
                            {
                                error = "Not enough data to preview.";
                            }
                        }
                        else
                        {
                            error = "Not enough data to preview.";
                        }

                    }
                    else
                    {
                        throw new BusinessException(string.Format("couldnt parse hfAdToEdit.Value to id , user id = {0}", currentUser.ID));
                    }
                }
                else
                {
                    error = "Preview is available for adverts with html text, or those who are being edited.";
                }
            }
            else
            {
                DecodeTbHtmlText();

                if (CommonCode.Validate.ValidateHtml(tbHtml.Text))
                {
                    phPreviewAdvert.Visible = true;

                    System.Web.UI.HtmlControls.HtmlTable adTable = new System.Web.UI.HtmlControls.HtmlTable();
                    System.Web.UI.HtmlControls.HtmlTableRow adTableRow = new System.Web.UI.HtmlControls.HtmlTableRow();
                    System.Web.UI.HtmlControls.HtmlTableCell adTableCell = new System.Web.UI.HtmlControls.HtmlTableCell();
                    adTable.Rows.Add(adTableRow);
                    adTableRow.Cells.Add(adTableCell);

                    phPreviewAdvert.Controls.Add(adTable);

                    adTableCell.InnerHtml = tbHtml.Text;

                    phAddEditAdvert.Visible = false;
                }
                else
                {
                    error = "The HTML is not well formed.";
                }
            }

            lblError.Text = error;
        }

        /// <summary>
        /// Decodes encoded HTML
        /// </summary>
        private void DecodeTbHtmlText()
        {
            if (string.IsNullOrEmpty(tbHtml.Text) == false)
            {
                tbHtml.Text = Server.HtmlDecode(tbHtml.Text);
                advertisementHtmlPostbackDecoded = true;
            }
        }

        protected void cExpireDate_SelectionChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cExpireDate.SelectedDate.ToString()))
            {
                string dateStr = Tools.ParseDateTimeToString(cExpireDate.SelectedDate);
                tbExpireDate.Text = dateStr;
            }
        }


    }
}
