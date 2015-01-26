﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Text;

using DataAccess;
using BusinessLayer;
using log4net;

namespace UserInterface
{
    public partial class CompanyPage : BasePage
    {
        private ILog log = LogManager.GetLogger(typeof(CompanyPage));

        protected long ProdNumber = 0;                                      // Numbers of Products in Company
        protected long PageNum = 1;                                         // Number of current page
        protected long ProdOnPage = Configuration.CompaniesProductsOnPage;  // Number of Products to show on page 
        protected long CategoryId = 0;                                      // Id of category by which products should be sorted

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private object addingImage = new object();

        private Company CurrentCompany = null;
        private User CurrentUser = null;

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (pnlEdit.Visible == true && apAddImage.Visible == true)
            {
                CustomServerControls.DecoratedButton btnUpload = (CustomServerControls.DecoratedButton)
                    CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "btnUpload");

                btnUpload.Attributes.Add("onclick", string.Format("ShowUploadingMsg('{0}','{1}')", uploadingSpan.ClientID, lblERROR.ClientID));
            }


            if (lblSendReport.Visible == true)
            {
                lblSendReport.Attributes.Add("onclick", string.Format("ShowReportData('{0}','{1}','{2}','{3}','{4}')"
                         , "company", CurrentCompany.ID, lblSendReport.ClientID, pnlActionReport.ClientID, pnlSendReport.ClientID));
            }

            if (btnSignForNotifies.Visible == true)
            {
                btnSignForNotifies.Attributes.Add("onclick", string.Format("NotifyForUpdates('{0}','{1}','{2}','{3}')"
                   , "company", CurrentCompany.ID, btnSignForNotifies.ClientID, pnlNotify.ClientID));
            }

            if (lblSendSuggestion.Visible == true)
            {
                if (hlSuggestionUser.Visible == true)
                {

                    lblSendSuggestion.Attributes.Add("onclick", string.Format(
                        "ShowSendTypeSuggestion('{0}', 'company', '{1}', '{2}', '{3}', '{4}', '{5}')"
                            , lblSendSuggestion.ClientID, CurrentCompany.ID, hlSuggestionUser.Attributes["userID"]
                            , ddlSuggestionUsers.ClientID, pnlSendTypeSuggestion.ClientID, pnlSendTypeSuggestionEnd.ClientID));
                }
                else
                {
                    lblSendSuggestion.Attributes.Add("onclick", string.Format(
                        "ShowSendTypeSuggestion('{0}', 'company', '{1}', '{2}', '{3}', '{4}', '{5}')"
                            , lblSendSuggestion.ClientID, CurrentCompany.ID, "empty"
                            , ddlSuggestionUsers.ClientID, pnlSendTypeSuggestion.ClientID, pnlSendTypeSuggestionEnd.ClientID));
                }
            }

            if (accEdit.Visible == true)
            {
                TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "tbAccEditDescription");
                description.Text = CurrentCompany.description;

                TextBox tbCount = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "tbSymbolsCount");
                tbCount.Text = description.Text.Length.ToString();

                description.Attributes.Add("onKeyUp", string.Format("ShowCharsCountInField('{0}', '{1}', '{2}', '{3}');"
                , description.ClientID, tbCount.ClientID, Configuration.CompaniesMinDescriptionLength, Configuration.CompaniesMaxDescriptionLength));
                description.Attributes.Add("onBlur", string.Format("ShowCharsCountInField('{0}', '{1}', '{2}', '{3}');"
                    , description.ClientID, tbCount.ClientID, Configuration.CompaniesMinDescriptionLength, Configuration.CompaniesMaxDescriptionLength));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser = GetCurrentUser(userContext, objectContext);

            CurrentCompany = GetCurrentCompany(objectContext);              // Checks Company parameter

            CheckSortByParams();                                            // Checks sort by category parameter

            CheckPageParam();                                               // Checks Page parameter

            CheckUser();                                                    // Depending on user shows options

            ShowInfo();                                                     // Shows company info aswell as chars/comments/products

            ShowActionOptions();                                            // Shows action panel if user can interact with the company

            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);       // Hides user notification panel   
        }

        private void CheckUser()
        {
            /// Shows button for adding product/char/company category

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessUser businessUser = new BusinessUser();

            if (CurrentUser != null)
            {
                bool canEditAllCompanies = businessUser.CanAdminDo(userContext, CurrentUser, AdminRoles.EditCompanies);

                if (businessUser.CanUserModifyCompany(objectContext, CurrentCompany.ID, CurrentUser.ID) || canEditAllCompanies)
                {
                    IEnumerable<Category> companyCategories = businessCompany.GetCompanyCategories(objectContext, CurrentCompany.ID);

                    // loading category menu with options
                    if (IsPostBack == false)
                    {
                        /// shows menu with edit options
                        pnlEdit.Visible = true;
                        CheckAddEditData(canEditAllCompanies);
                    }

                    lblMoreInfo.Text = GetLocalResourceObject("forMoreInfoOnEditing").ToString();

                    int compCatCount = companyCategories.Count<Category>();
                    UpdateCompanyNotification(compCatCount);

                    // Admin panel
                    if (canEditAllCompanies)
                    {

                        if (CurrentCompany.canUserTakeRoleIfNoEditors == true)
                        {
                            lblCanUserTakeRoleIfNoEditors.Text = "Currently users CAN take action to edit this company if there are no editors.";
                        }
                        else
                        {
                            lblCanUserTakeRoleIfNoEditors.Text = "Currently users CAN NOT take action to edit this company if there are no editors.";
                        }

                        if (CurrentCompany.visible)
                        {
                            lblVisible.Text = string.Format("{0} is VISIBLE", CurrentCompany.name);
                            btnDeleteCompany.Visible = true;
                            btnUndoDelete.Visible = false;

                            pnlGiveRoleToUser.Visible = true;
                            pnlGiveRoleForAllProducts.Visible = true;

                            cbDeleteAllCompanyProducts.Visible = true;
                        }
                        else
                        {
                            lblVisible.Text = string.Format("{0} is NOT VISIBLE", CurrentCompany.name);
                            btnDeleteCompany.Visible = false;
                            btnUndoDelete.Visible = true;

                            pnlGiveRoleToUser.Visible = false;
                            pnlGiveRoleForAllProducts.Visible = false;

                            cbDeleteAllCompanyProducts.Visible = false;
                        }


                        // Fills table with company editors
                        CompanyEditorsFill();
                        ACompanyProductsEditorsFill();

                        accAdmin.Visible = true;
                    }

                }
            }
        }

        private void ShowActionOptions()
        {
            if (CurrentUser == null)
            {
                pnlActions.Visible = false;
                return;
            }

            Boolean pnlActionsVisible = false;

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessUser businessUser = new BusinessUser();

            if (businessUser.CanUserDo(userContext, CurrentUser, UserRoles.AddProducts))
            {
                if (businessCompany.IfHaveValidCategories(objectContext, CurrentCompany.ID))
                {
                    hlAddProduct.NavigateUrl = GetUrlWithVariant(string.Format("AddProduct.aspx?Company={0}", CurrentCompany.ID));
                    hlAddProduct.Visible = true;

                    pnlActionsVisible = true;
                }
            }

            BusinessReport businessReport = new BusinessReport();
            if (businessUser.CanUserDo(userContext, CurrentUser, UserRoles.ReportInappropriate)
                && !businessReport.IsMaxActiveIrregularityReportsReached(objectContext, CurrentUser))
            {
                //btnReport.Visible = true;
                lblSendReport.Visible = true;

                pnlActionsVisible = true;

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

            BusinessNotifies businessNotifies = new BusinessNotifies();
            if (!businessNotifies.SetNewInformationFalseForCompanyIfUserIsSigned(objectContext, CurrentUser, CurrentCompany)
                && !businessNotifies.IsMaxNotificationsNumberReached(objectContext, CurrentUser))
            {
                pnlActionsVisible = true;

                btnSignForNotifies.Visible = true;
            }

            if (lblSendSuggestion.Visible == true)
            {
                pnlActionsVisible = true;
            }

            if (btnTakeAction.Visible == true)
            {
                pnlActionsVisible = true;
            }

            pnlActions.Visible = pnlActionsVisible;
        }

        private void SetLocalText()
        {
            lblAdded.Text = string.Format("{0} ", GetLocalResourceObject("AddedBy2").ToString());
            lblLastModifiedBy.Text = GetLocalResourceObject("LastModifiedBy").ToString() + "&nbsp;";

            if (pnlEdit.Visible == true)
            {
                // ADD
                lblAccAdd.Text = GetLocalResourceObject("Add").ToString();

                hlMoreInfo.NavigateUrl = GetUrlWithVariant("Rules.aspx#rulesaemaker");

                Label addCategory = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCategory, "lblAddCategoriesHEader");
                addCategory.Text = GetLocalResourceObject("AddCategory").ToString();
                Label addCategoryInfo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCategory, "lblAddCatInfo");
                addCategoryInfo.Text = GetLocalResourceObject("addCategoryInfo").ToString();
                Button btnSC = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apAddCategory, "btnSubmitCategory");
                btnSC.Text = GetLocalResourceObject("Submit").ToString();
                Label lblAC = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCategory, "lblCat1");
                lblAC.Text = GetLocalResourceObject("lblAddCategory").ToString();

                Label addChar = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblAddCharHeader");
                addChar.Text = GetLocalResourceObject("AddCharacteristics").ToString();
                Label lblCW = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblCharName1");
                lblCW.Text = GetLocalResourceObject("CharTopic").ToString();
                Label lblACD = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblAddCharDescription");
                lblACD.Text = GetLocalResourceObject("Description").ToString();
                Button btnSCh = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "btnSubmitChar");
                btnSCh.Text = GetLocalResourceObject("Submit").ToString();

                Label addImg = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "lblAddImageHEader");
                lblAddImageHEader.Text = GetLocalResourceObject("AddImage").ToString();
                CheckBox cbL = (CheckBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "cbLogo");
                cbL.Text = GetLocalResourceObject("Logo").ToString();
                Label lblUI = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "lblUploadImageDescr");
                lblUI.Text = GetLocalResourceObject("Description").ToString();
                lblUploadingImg.Text = GetLocalResourceObject("UploadingImg").ToString();
                Button btnUpload = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "btnUpload");
                btnUpload.Text = GetLocalResourceObject("Upload").ToString();

                Label chooseImg = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "lblChooseImg");
                chooseImg.Text = GetLocalResourceObject("chooseImage").ToString();


                HyperLink seeImg1 = (HyperLink)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "hlClickForMinImage");
                seeImg1.Text = GetLocalResourceObject("ClickHere").ToString();
                Label seeImg2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "lblClickForMinImage2");
                seeImg2.Text = GetLocalResourceObject("seeImgMinSize").ToString();
                HyperLink seeLogo = (HyperLink)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "hlClickForMinLogoSize");
                seeLogo.Text = GetLocalResourceObject("ClickHere").ToString();
                Label clickToSeeLogo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "lblClickForMinLogoSize2");
                clickToSeeLogo.Text = GetLocalResourceObject("seeLogoMinSize").ToString();

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

                ///// EDIT
                lblAccEdit.Text = GetLocalResourceObject("Edit").ToString();

                Label editWebsite = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apChangeWebSite, "lblEditWebsiteHEader");
                editWebsite.Text = GetLocalResourceObject("EditSite").ToString();
                Label lblNWS = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apChangeWebSite, "lblNewWebSite");
                lblNWS.Text = GetLocalResourceObject("newSite").ToString();
                Button btnEW = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apChangeWebSite, "dbEditWebite");
                btnEW.Text = GetLocalResourceObject("Change").ToString();

                Label deleteCategories = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apDeleteCategories, "lblAccDeleteCategoryesHEader");
                deleteCategories.Text = GetLocalResourceObject("DeleteCategories").ToString();
                Button btnDC = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apDeleteCategories, "dbDeleteCategories");
                btnDC.Text = GetLocalResourceObject("Delete").ToString();
                Label lblDC = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apDeleteCategories, "lblDeleteCategories");
                lblDC.Text = GetLocalResourceObject("Categories").ToString();

                Label editChar = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblEditCharHeader");
                editChar.Text = GetLocalResourceObject("EditCharacteristics").ToString();
                Button btnEC = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "dbEditCharacteristic");
                btnEC.Text = GetLocalResourceObject("Update").ToString();
                Button btnDCh = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "dbDeleteCharacteristic");
                btnDCh.Text = GetLocalResourceObject("Delete").ToString();
                Label lblEDC = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblEditCharDescription");
                lblEDC.Text = GetLocalResourceObject("Description").ToString();
                Label lblEDCh = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblEditCharChoose");
                lblEDCh.Text = GetLocalResourceObject("Characteristic").ToString();
                Label lblECS = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblEditCharNewName");
                lblECS.Text = GetLocalResourceObject("EditCharNewTopic").ToString();

                Label description = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "lblEditDescriptionHEader");
                description.Text = GetLocalResourceObject("EditDescription").ToString();
                Label lblEDs = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "lblEditDescription");
                lblEDs.Text = GetLocalResourceObject("Description").ToString();
                Button btnED = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "dbEditDescription");
                btnED.Text = GetLocalResourceObject("Update").ToString();
                Label SymbolsCount = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "lblSymbolsCount");
                SymbolsCount.Text = GetLocalResourceObject("textLength").ToString();

                Label editCompName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apChangeName, "lblEditNameHeader");
                editCompName.Text = GetLocalResourceObject("EditName").ToString();
                Label newCompName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apChangeName, "lblNewName");
                newCompName.Text = GetLocalResourceObject("NewCompName").ToString();
                Button btnEditName = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apChangeName, "dbChangeName");
                btnEditName.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();

                Label removeShowAlternativeName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "lblShowRemoveAlternativeNames");
                removeShowAlternativeName.Text = GetLocalResourceObject("RemoveAlternativeNames").ToString();
                Label removeAlternativeName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "lblRemoveAlternativeNames");
                removeAlternativeName.Text = GetLocalResourceObject("AlternativeNames").ToString();
                Button btnRemoveAlternativeNames = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "btnRemoveAlternativeNames");
                btnRemoveAlternativeNames.Text = GetGlobalResourceObject("SiteResources", "Delete").ToString();

                if (apChangeName.Visible == true)
                {
                    Label lblChangeNameHeader = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apChangeName, "lblEditNameHeader");
                    lblChangeNameHeader.Text = GetLocalResourceObject("changeName").ToString();

                    Label info = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apChangeName, "lblAccNewNameInfo");
                    info.Text = string.Format("{0} {1}-{2} {3}", GetLocalResourceObject("nameRules"), Configuration.CompaniesMinCompanyNameLength
                        , Configuration.CompaniesMaxCompanyNameLength, GetLocalResourceObject("symbols"));

                    Button btnChangeName = (Button)CommonCode.UiTools.GetControlFromAccordionPane(apChangeName, "dbChangeName");
                    btnChangeName.Text = GetGlobalResourceObject("SiteResources", "Submit").ToString();
                }
            }
            ///

            hlAddProduct.Text = GetGlobalResourceObject("SiteResources", "AddProduct").ToString();
            lblSendSuggestion.Text = GetLocalResourceObject("SendSuggestion").ToString();
            btnSignForNotifies.Value = GetLocalResourceObject("SignNotifies").ToString();
            btnSignForNotifies.Attributes.Add("title", GetLocalResourceObject("SignNotifiesTooltip").ToString());
            btnTakeAction.Text = GetLocalResourceObject("TakeRole").ToString();
            lblSortBy.Text = GetLocalResourceObject("SortBy").ToString();

            lblSendReport.Text = GetGlobalResourceObject("SiteResources", "lblWriteReport").ToString();
            lblRepIrregularity.Text = GetGlobalResourceObject("SiteResources", "reportIrregularity").ToString();
            btnSendReport.Value = GetGlobalResourceObject("SiteResources", "btnReport").ToString();
            btnHideRepData.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();

            //lblActions.Text = GetGlobalResourceObject("SiteResources", "Actions").ToString();

            lblCompanyEditors.Text = GetLocalResourceObject("MakerEditors").ToString();

            lblSendSuggestionInfo.Text = GetLocalResourceObject("SendSuggestion").ToString();
            lblSendSuggestionTo.Text = GetLocalResourceObject("SendSuggestionTo").ToString();
            btnSendTypeSuggestion.Value = GetGlobalResourceObject("SiteResources", "Send").ToString();
            btnCancelSuggestion.Value = GetGlobalResourceObject("SiteResources", "Close").ToString();

            hlMoreInfo.Text = GetLocalResourceObject("ClickHere").ToString();
        }

        private void CheckAddImageOptions()
        {
            ImageTools imageTools = new ImageTools();

            if (imageTools.GetCompanyImagesCount(objectContext, CurrentCompany) < Configuration.ImagesMaxCompanyImagesCount)
            {
                apAddImage.Visible = true;

                Label lblInfo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "lblUpImgINfo");
                Label lblInfo2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "lblUpImgINfo2");
                lblInfo.Text = string.Format("{0} {1}/{2}.", GetLocalResourceObject("imageRules")
                    , Configuration.ImagesMinImageWidth, Configuration.ImagesMinImageHeight);
                lblInfo2.Text = GetLocalResourceObject("imageRules2").ToString();
                Label lblLogoInfo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "lblUpImgLogoInfo");

                HyperLink seeImg1 = (HyperLink)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "hlClickForMinImage");
                seeImg1.NavigateUrl = "SampleImage.aspx?show=minCompanyImage";  // "GetImageHandler.ashx?show=minCompanyImage";

                CheckBox logo = (CheckBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "cbLogo");
                HyperLink seeLogo = (HyperLink)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "hlClickForMinLogoSize");
                Label clickToSeeLogo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "lblClickForMinLogoSize2");


                string appPath = CommonCode.PathMap.PhysicalApplicationPath;  // CommonCode.PathMap.GetImagesPhysicalPathRoot();
                if (!imageTools.IfCompanyHaveLogo(objectContext, CurrentCompany, businessLog, appPath))
                {
                    logo.Visible = true;

                    seeLogo.Visible = true;
                    seeLogo.NavigateUrl = "SampleImage.aspx?show=minLogo";  // "GetImageHandler.ashx?show=minLogo";

                    clickToSeeLogo.Visible = true;

                    lblLogoInfo.Visible = true;
                    lblLogoInfo.Text = string.Format("{0} {1}/{2}. ", GetLocalResourceObject("imageLogoRules")
                        , Configuration.ImagesMinCompLogoWidth, Configuration.ImagesMinCompLogoHeight);


                }
                else
                {
                    logo.Visible = false;

                    seeLogo.Visible = false;
                    clickToSeeLogo.Visible = false;

                    lblLogoInfo.Visible = false;
                }
            }
            else
            {
                apAddImage.Visible = false;
            }
        }

        /// <summary>
        /// Shows Notification label if company doesnt have added categories : 
        /// num > 0 there are visible categories ; num == 0 no visible categories ;
        /// num below 0 then checks number of categories 
        /// </summary>
        private void UpdateCompanyNotification(int count)
        {
            lblCompNotif.Text = string.Empty;

            if (count == 0)
            {
                pnlNotification.Visible = true;
                lblCompNotif.Text = GetLocalResourceObject("compNotif").ToString();
            }
            else
            {
                if (count < 0 || count == 1)
                {
                    BusinessCompany businessCompany = new BusinessCompany();
                    List<CategoryCompany> companyCategories = businessCompany.GetAllCompanyCategories(objectContext, CurrentCompany).ToList();
                    if (companyCategories.Count > 0)
                    {
                        if (companyCategories.Count == 1)
                        {
                            if (!companyCategories[0].CategoryReference.IsLoaded)
                            {
                                companyCategories[0].CategoryReference.Load();
                            }

                            BusinessCategory businessCategory = new BusinessCategory();
                            Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);

                            if (unspecified == companyCategories[0].Category)
                            {
                                pnlNotification.Visible = true;
                                lblCompNotif.Text = GetLocalResourceObject("compNotif").ToString();
                            }
                            else
                            {
                                pnlNotification.Visible = false;
                            }

                        }
                        else
                        {
                            pnlNotification.Visible = false;
                        }
                    }
                    else
                    {
                        pnlNotification.Visible = true;
                        lblCompNotif.Text = GetLocalResourceObject("compNotif").ToString();
                    }
                }
                else
                {
                    pnlNotification.Visible = false;
                }
            }

            if (CurrentCompany.visible == false)
            {
                pnlNotification.Visible = true;
                if (string.IsNullOrEmpty(lblCompNotif.Text))
                {
                    lblCompNotif.Text = "The company is DELETED.";
                }
                else
                {
                    lblCompNotif.Text += "<br />The company is DELETED.";
                }
            }
        }

        private void ShowCompanyInfo()
        {
            ImageTools imageTools = new ImageTools();
            BusinessUser businessUser = new BusinessUser();
            BusinessAlternativeNames bAN = new BusinessAlternativeNames();

            Title = string.Format("{0} {1}", CurrentCompany.name, GetLocalResourceObject("title"));

            if (!CurrentCompany.CreatedByReference.IsLoaded)
            {
                CurrentCompany.CreatedByReference.Load();
            }

            phAddedBy.Controls.Clear();
            phAddedBy.Controls.Add(lblAdded);
            if (businessUser.IsFromUserTeam(Tools.GetUserFromUserDatabase(userContext, CurrentCompany.CreatedBy)))
            {
                phAddedBy.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                    (Tools.GetUserFromUserDatabase(userContext, CurrentCompany.CreatedBy)));
            }
            else
            {
                Label admLabel = CommonCode.UiTools.GetAdminLabel(Tools.GetUserFromUserDatabase(userContext, CurrentCompany.CreatedBy).username);
                phAddedBy.Controls.Add(admLabel);
            }
            phAddedBy.Controls.Add(lblAddedOn);
            lblAddedOn.Text = string.Format(" , {0} {1}", GetLocalResourceObject("AddedOn"), CommonCode.UiTools.DateTimeToLocalShortDateString(CurrentCompany.dateCreated));


            phAddedAndModified.Controls.Clear();
            phAddedAndModified.Controls.Add(lblLastModifiedBy);
            if (!CurrentCompany.LastModifiedByReference.IsLoaded)
            {
                CurrentCompany.LastModifiedByReference.Load();
            }
            if (businessUser.IsFromUserTeam(Tools.GetUserFromUserDatabase(userContext, CurrentCompany.LastModifiedBy)))
            {
                phAddedAndModified.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                    (Tools.GetUserFromUserDatabase(userContext, CurrentCompany.LastModifiedBy)));
            }
            else
            {
                Label admLabel = CommonCode.UiTools.GetAdminLabel(Tools.GetUserFromUserDatabase(userContext, CurrentCompany.LastModifiedBy).username);
                phAddedAndModified.Controls.Add(admLabel);
            }

            phAddedAndModified.Controls.Add(lblLastModified);
            if (CurrentCompany.lastModified == CurrentCompany.dateCreated)
            {
                lblLastModified.Text = string.Empty;
            }
            else
            {
                lblLastModified.Text = string.Format("  , {0} {1}", GetLocalResourceObject("AddedOn")
                    , CommonCode.UiTools.DateTimeToLocalShortDateString(CurrentCompany.lastModified));
            }

            hlCompanyName.Text = CurrentCompany.name;
            hlCompanyName.NavigateUrl = getCurrCompanyLink(CurrentCompany);

            phWebSite.Controls.Clear();
            Label site = new Label();
            site.Text = GetLocalResourceObject("Site").ToString();
            site.CssClass = "searchPageComments";
            phWebSite.Controls.Add(site);
            if (!string.IsNullOrEmpty(CurrentCompany.website))
            {
                HyperLink siteLink = new HyperLink();
                siteLink.NavigateUrl = CurrentCompany.website;
                siteLink.Text = Tools.TrimString(CurrentCompany.website, 60, false, true);
                siteLink.Target = "_blank";
                phWebSite.Controls.Add(siteLink);
            }
            else
            {
                site.Text = GetLocalResourceObject("NoSite").ToString();
            }

            CommonCode.UiTools.AddShareStuff(lblShare, Page, Title);

            if (string.IsNullOrEmpty(CurrentCompany.description))
            {
                pnlCompDescr.Visible = false;
            }
            else
            {
                pnlCompDescr.Visible = true;
                lblCompDescr.Text = Tools.GetFormattedTextFromDB(CurrentCompany.description);
            }

            ShowLogo(imageTools);

            List<AlternativeCompanyName> altNames = bAN.GetVisibleAlternativeCompanyNames(objectContext, CurrentCompany);
            if (altNames != null && altNames.Count > 0)
            {
                lblAlternativeNames.Visible = true;

                StringBuilder alternativeNames = new StringBuilder();
                alternativeNames.Append("<span class=\"searchPageComments\">");
                alternativeNames.Append(GetLocalResourceObject("AlternativeNames").ToString());
                alternativeNames.Append("</span>");
                alternativeNames.Append("<span class=\"darkOrange\">");

                foreach (AlternativeCompanyName name in altNames)
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

        }

        private void ShowInfo()
        {
            ShowCompanyInfo();

            ShowAccordionsInfo();

            ShowPages(tblPages);        // shows pages with products 
            ShowPages(tblPagesBtm);

            ShowProducts(); // shows products

            ShowAdvertisement();
            EmptyLabels();
            FillPublicShownCompanyEditors();

            if (IsPostBack == false)
            {
                FillDdlSortByCategories();
            }

            SetLocalText();
        }

        private void ShowAccordionsInfo()
        {

            BusinessCompany businessCompany = new BusinessCompany();
            ImageTools imageTools = new ImageTools();

            ShowCategories();

            if (businessCompany.CountCompanyCharacterestics(objectContext, CurrentCompany) < 1)
            {
                apShowCharacteristics.Visible = false;
            }
            else
            {
                lblChars.Text = GetLocalResourceObject("accChars").ToString();
                tblCharacteristics.Visible = true;
                pnlShowCharacteristics.CssClass = "accordionHeaders";
                ShowCharacteristics();

                apShowCharacteristics.Visible = true;
            }
            if (imageTools.GetCompanyImagesCount(objectContext, CurrentCompany) < 1)
            {
                lblGallery.Text = GetLocalResourceObject("accNoGallery").ToString();
                tblGallery.Visible = false;
                pnlShowGallery.CssClass = "accordionHeadersNoCursor";
            }
            else
            {
                lblGallery.Text = GetLocalResourceObject("accGallery").ToString();
                tblGallery.Visible = true;
                pnlShowGallery.CssClass = "accordionHeaders";
                FillGalleryTable();
            }

            CheckIfAccordionsInfoShouldBeVisible();
        }

        private void CheckIfAccordionsInfoShouldBeVisible()
        {
            if (apSHowGallery.Visible == true || apShowCharacteristics.Visible == true || apShowCategories.Visible == true)
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

        private void FillPublicShownCompanyEditors()
        {
            phPublicCompEditors.Controls.Clear();

            BusinessUserTypeActions butActions = new BusinessUserTypeActions();
            BusinessUser bUser = new BusinessUser();

            List<UsersTypeAction> actions = butActions.GetCompanyModificators(objectContext, CurrentCompany.ID).ToList();
            List<User> usersToWhichCanBeSendSuggestion = new List<User>();

            bool currUserLogged = false;
            bool isUser = false;
            bool canUserTakeAction = false;
            bool currUserCanEditType = false;

            if (CurrentUser != null)
            {
                currUserCanEditType = bUser.CanUserModifyCompany(objectContext, CurrentCompany.ID, CurrentUser.ID);

                currUserLogged = true;
                isUser = bUser.IsUser(CurrentUser);

                canUserTakeAction = butActions.CanUserTakeActionFromEditor(userContext, objectContext, CurrentUser);
            }

            if (actions.Count > 0)
            {
                Panel infoPanel = new Panel();
                phPublicCompEditors.Controls.Add(infoPanel);
                infoPanel.CssClass = "sectionTextHeader";

                Label lblEditors = new Label();
                infoPanel.Controls.Add(lblEditors);
                lblEditors.Text = GetLocalResourceObject("Editors").ToString();

                User editor = null;
                foreach (UsersTypeAction action in actions)
                {
                    if (!action.UserReference.IsLoaded)
                    {
                        action.UserReference.Load();
                    }

                    Panel newPanel = new Panel();
                    phPublicCompEditors.Controls.Add(newPanel);

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
                        if (editor.ID != CurrentUser.ID)
                        {
                            if (canUserTakeAction == true && butActions.CanTypeActionsBeTakenFromUser(editor) == true
                                && currUserCanEditType == false)
                            {
                                Panel right = new Panel();
                                newPanel.Controls.Add(right);
                                right.CssClass = "floatRight";

                                HyperLink link = new HyperLink();
                                right.Controls.Add(link);

                                link.CssClass = "searchPageRatings";
                                link.Text = GetLocalResourceObject("TakeRoleS").ToString();
                                link.NavigateUrl = GetUrlWithVariant(string.Format("EditorRights.aspx?User={0}", editor.ID));
                                link.Target = "_blank";
                            }

                            usersToWhichCanBeSendSuggestion.Add(editor);
                        }

                    }
                }
            }
            else
            {
                Panel newPanel = new Panel();
                phPublicCompEditors.Controls.Add(newPanel);
                newPanel.CssClass = "sectionTextHeader";

                Label lblNoEditors = new Label();
                newPanel.Controls.Add(lblNoEditors);
                lblNoEditors.Text = GetLocalResourceObject("NoEditors").ToString();
            }

            if (CurrentUser != null && isUser == true)
            {
                ShowToWhichUsersSuggestionsCanBeSent(usersToWhichCanBeSendSuggestion, currUserCanEditType);
                ShowOptionToTakeRoleIfThereAreNoEditors(currUserCanEditType, usersToWhichCanBeSendSuggestion.Count);
            }
            else
            {
                lblSendSuggestion.Visible = false;
                btnTakeAction.Visible = false;
            }
        }

        private void ShowOptionToTakeRoleIfThereAreNoEditors(bool canUserEditCurrType, int userToWhichCanBeSendSuggestions)
        {
            if (canUserEditCurrType == true || userToWhichCanBeSendSuggestions > 0 || CurrentCompany.canUserTakeRoleIfNoEditors == false)
            {
                btnTakeAction.Visible = false;
            }
            else
            {
                btnTakeAction.Visible = true;
                btnTakeAction.ToolTip = string.Format("{0} {1}", CurrentCompany.name, GetLocalResourceObject("DontHaveEditors"));
            }
        }

        private void ShowToWhichUsersSuggestionsCanBeSent(List<User> actionUsers, bool canEditCurrComp)
        {
            int count = actionUsers.Count;

            if (count > 0 && CurrentUser != null && canEditCurrComp == false)
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
                    first.Text = GetLocalResourceObject("user").ToString();
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

        private void EmptyLabels()
        {
            Label editCharName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblEditCharCHeckNewName");
            editCharName.Text = string.Empty;

            Label addCharName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblCCName");
            addCharName.Text = string.Empty;

            Label changeName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apChangeName, "lblAccCheckNewName");
            changeName.Text = string.Empty;
        }

        /// <summary>
        /// Fills DropDownList SortByCat with items
        /// </summary>
        private void FillDdlSortByCategories()
        {
            ddlSortByCat.Items.Clear();
            BusinessCompany businessCompany = new BusinessCompany();

            List<Category> categories = businessCompany.GetCompanyCategories(objectContext, CurrentCompany.ID).ToList();
            if (categories != null && categories.Count() > 1)
            {

                ddlSortByCat.Visible = true;
                lblSortBy.Visible = true;

                ListItem allCats = new ListItem();
                allCats.Text = GetLocalResourceObject("allCategories").ToString();
                allCats.Value = "0";
                ddlSortByCat.Items.Add(allCats);

                string catPath = string.Empty;
                long currCatId = CategoryId;

                int menuNum = 1;
                string catName = string.Empty;
                foreach (Category category in categories)
                {
                    catName = Tools.CategoryName(objectContext, category, false);

                    catPath = Tools.TrimString(catName, 200, true, true);
                    ListItem newItem = new ListItem();
                    newItem.Text = catPath;
                    newItem.Value = category.ID.ToString();
                    ddlSortByCat.Items.Add(newItem);

                    if (currCatId == category.ID)
                    {
                        ddlSortByCat.SelectedIndex = menuNum;
                    }

                    menuNum++;
                }
            }
            else
            {
                ddlSortByCat.Visible = false;
                lblSortBy.Visible = false;
            }

        }

        private void ShowAdvertisement()
        {
            if (Configuration.AdvertsNumAdvertsOnCompanyPage > 0)
            {
                phAdvert.Controls.Clear();
                adCell.Attributes.Clear();

                phAdvert.Controls.Add(CommonCode.ImagesAndAdverts.GetAdvertisements
                    (objectContext, Server, "company", CurrentCompany.ID, Configuration.AdvertsNumAdvertsOnCompanyPage));
                if (CommonCode.ImagesAndAdverts.getAdvertisementsNumber(phAdvert) > 0)
                {
                    phAdvert.Visible = true;

                    adCell.Width = "252px";
                    adCell.VAlign = "top";
                }
                else
                {
                    phAdvert.Visible = false;
                    adCell.Width = "1px";
                }
            }
        }

        private void CheckIfUserCanModifyAllCompaniesFromEvents()
        {
            BusinessUser businessUser = new BusinessUser();
            if (businessUser != null)
            {
                if (!businessUser.CanAdminDo(userContext, CurrentUser, AdminRoles.EditCompanies))
                {
                    throw new CommonCode.UIException(string.Format("User ID = {0} cannot modify all companies", CurrentUser.ID));
                }
            }
            else
            {
                throw new CommonCode.UIException("Guest cannot modify all companies");
            }
        }

        private void CheckIfUserCanModifyCompanyFromEvents()
        {
            if (CurrentUser != null)
            {
                BusinessUser businessUser = new BusinessUser();

                if (!businessUser.CanAdminDo(userContext, CurrentUser, AdminRoles.EditCompanies) &&
                    !businessUser.CanUserModifyCompany(objectContext, CurrentCompany.ID, CurrentUser.ID))
                {
                    RedirectToSameUrl(Request.Url.ToString());
                }
            }
            else
            {
                RedirectToSameUrl(Request.Url.ToString());
            }
        }

        private void ShowLogo(ImageTools imageTools)
        {
            string appPath = CommonCode.PathMap.PhysicalApplicationPath;
            CompanyImage img = imageTools.GetCompanyLogo(objectContext, CurrentCompany, businessLog, appPath);
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


                giLogo.ImageHandlerUrl = giUrl;

                giLogo.Parameters.Clear();

                Microsoft.Web.ImageParameter compIDParam = new Microsoft.Web.ImageParameter();
                compIDParam.Name = "compID";
                compIDParam.Value = CurrentCompany.ID.ToString();
                giLogo.Parameters.Add(compIDParam);
                giLogo.ToolTip = img.description;

                imgLink.NavigateUrl = img.url;
                imgLink.Target = "_blank";
                imgLink.Visible = true;
            }
            else
            {
                imgLink.Visible = false;
            }
        }

        private void LoadCategoryMenuWithItems()
        {
            BusinessCategory businessCategory = new BusinessCategory();
            BusinessCompany businessCompany = new BusinessCompany();

            Menu menu = (Menu)CommonCode.UiTools.GetControlFromAccordionPane(apAddCategory, "navMenu");
            menu.Items.Clear();

            string browserType = Request.Browser.Type.ToUpper();
            if (browserType.Contains("IE") && browserType != "IE5" && browserType != "IE9")  // Because chrome is detected as IE5
            {
                menu.DynamicHorizontalOffset = -1;
                menu.DynamicVerticalOffset = -2;
            }

            IEnumerable<Category> categoryList = businessCompany.GetCompanyCategories(objectContext, CurrentCompany.ID);
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

                    if (category.parentID != null)
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

                    if (category.parentID != null)
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

        private void ShowCategories()
        {
            tblCategories.Rows.Clear();

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessUser businessUser = new BusinessUser();
            Boolean idfield = false;

            if (CurrentUser != null)
            {
                if (businessUser.CanAdminDo(userContext, CurrentUser, AdminRoles.EditCompanies))
                {
                    idfield = true;
                }
            }

            List<Category> compCategories = businessCompany.GetCompanyCategories(objectContext, CurrentCompany.ID).ToList();

            int count = compCategories.Count;

            if (count > 0)
            {
                lblCategories.Text = GetLocalResourceObject("accCategories").ToString();
                tblCategories.Visible = true;
                pnlShowCategories.CssClass = "accordionHeaders";

                apShowCategories.Visible = true;

                int i = 0;

                foreach (Category category in compCategories)
                {
                    i++;

                    TableRow newRoll = new TableRow();
                    tblCategories.Rows.Add(newRoll);

                    TableCell Cell = new TableCell();
                    newRoll.Cells.Add(Cell);
                    Cell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingLeft, "5px");

                    if (i < count)
                    {
                        Cell.CssClass = "compCatTblBgr";
                    }

                    if (idfield)
                    {
                        long id = businessCompany.GetCompanyCategoryID(objectContext, CurrentCompany, category);
                        Cell.Controls.Add(CommonCode.UiTools.GetLabelWithText(string.Format("{0}&nbsp;&nbsp;&nbsp;", id), false));
                    }

                    string catPathStyled = Tools.CategoryName(objectContext, category, true);
                    Cell.Controls.Add(CommonCode.UiTools.GetLabelWithText(catPathStyled, false));
                }
            }
            else
            {
                apShowCategories.Visible = false;
            }

        }

        private void ShowCharacteristics()
        {
            tblCharacteristics.Rows.Clear();

            tblCharacteristics.Style.Add(HtmlTextWriterStyle.PaddingTop, "5px");
            tblCharacteristics.Style.Add(HtmlTextWriterStyle.PaddingBottom, "5px");

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessUser businessUser = new BusinessUser();
            Boolean idfield = false;

            if (CurrentUser != null)
            {
                if (businessUser.CanAdminDo(userContext, CurrentUser, AdminRoles.EditCompanies))
                {
                    idfield = true;
                }
            }

            IEnumerable<CompanyCharacterestics> currCompanyCharacteristics
                = businessCompany.GetCompanyCharacterestics(objectContext, CurrentCompany.ID);
            int charCount = currCompanyCharacteristics.Count<CompanyCharacterestics>();
            if (charCount > 0)
            {
                int charNum = 0;
                foreach (CompanyCharacterestics characteristics in currCompanyCharacteristics)
                {
                    charNum++;

                    TableRow newRow = new TableRow();
                    tblCharacteristics.Rows.Add(newRow);
                    newRow.CssClass = "textHeader";

                    if (idfield)
                    {
                        TableCell idCell = new TableCell();
                        idCell.Width = Unit.Percentage(5);
                        idCell.Text = characteristics.ID.ToString();
                        newRow.Cells.Add(idCell);
                    }

                    TableCell nameCell = new TableCell();
                    nameCell.Width = Unit.Percentage(80);
                    nameCell.HorizontalAlign = HorizontalAlign.Center;
                    nameCell.Text = characteristics.name;
                    newRow.Cells.Add(nameCell);

                    TableRow descrRoll = new TableRow();
                    tblCharacteristics.Rows.Add(descrRoll);

                    TableCell descrCell = new TableCell();
                    descrRoll.Cells.Add(descrCell);

                    Panel descrPnl = new Panel();
                    descrCell.Controls.Add(descrPnl);
                    descrPnl.CssClass = "padding5px";

                    Label text = new Label();
                    text.Text = Tools.GetFormattedTextFromDB(characteristics.description); ;
                    descrPnl.Controls.Add(text);

                    if (charNum < charCount)
                    {
                        descrCell.Controls.Add(CommonCode.UiTools.GetHorisontalFashionLinePanel(true));
                    }
                    if (idfield)
                    {
                        descrCell.ColumnSpan = 2;
                    }
                }
            }
            else
            {
                TableRow lastRow = new TableRow();
                tblCharacteristics.Rows.Add(lastRow);

                TableCell lastCell = new TableCell();
                lastCell.Text = GetLocalResourceObject("accNoChars").ToString();
                lastRow.Cells.Add(lastCell);
            }
        }

        protected void navMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddCategory, "tbCatName1");
            Menu menu = (Menu)CommonCode.UiTools.GetControlFromAccordionPane(apAddCategory, "navMenu");

            BusinessCategory bCategory = new BusinessCategory();
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

        protected void btnAddChar_Click(object sender, EventArgs e)
        {
            pnlAddChar.Visible = true;
            phAddChar.Visible = false;
        }

        protected void btnSubmitChar_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            PlaceHolder phAChar = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "phAddChar");

            phAChar.Controls.Add(lblERROR);
            phAChar.Visible = true;
            String error = "";

            BusinessCompany businessCompany = new BusinessCompany();

            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "tbCharName1");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "tbCharDescr1");

            string strName = name.Text;

            if (CommonCode.Validate.ValidateName(objectContext, "compChar", ref strName, Configuration.CompaniesMinCompanyNameLength,
                Configuration.CompaniesMaxCompanyNameLength, out error, CurrentCompany.ID))
            {

                string strDescription = description.Text;

                if (CommonCode.Validate.ValidateDescription(Configuration.FieldsMinDescriptionFieldLength,
                    Configuration.FieldsMaxDescriptionFieldLength, ref strDescription, "description", out error, 120))
                {
                    businessCompany.AddCompanyCharacteristic(userContext, objectContext, CurrentCompany, businessLog, CurrentUser, strName, strDescription);

                    CheckAddCharacteristicsOptions();
                    FillEditCharacteristicsAP();

                    ShowAccordionsInfo();

                    name.Text = string.Empty;
                    description.Text = string.Empty;

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("CharAdded").ToString());
                }
            }
            lblERROR.Text = error;
        }



        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            pnlAddCategory.Visible = true;
            LoadCategoryMenuWithItems();

            phAddCategory.Visible = false;
        }

        protected void btnSubmitCategory_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            PlaceHolder phCategory = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apAddCategory, "phAddCategory");
            phCategory.Visible = true;
            phCategory.Controls.Add(lblERROR);
            string error = "";

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessUser businessUser = new BusinessUser();
            BusinessCategory businessCategory = new BusinessCategory();

            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddCategory, "tbCatName1");
            Menu catMenu = (Menu)CommonCode.UiTools.GetControlFromAccordionPane(apAddCategory, "navMenu");

            if (string.IsNullOrEmpty(name.Text))
            {
                error = GetLocalResourceObject("chooseCategory").ToString();


                name.Text = string.Empty;
            }
            else
            {
                string strcatid = catMenu.SelectedItem.Value;
                if (strcatid == null || strcatid.Length < 1)
                {
                    throw new CommonCode.UIException(string.Format("navMenu.SelectedItem.Value is null or empty , user id = {0}", CurrentUser.ID));
                }
                long catid = -1;
                long.TryParse(strcatid, out catid);
                if (catid < 1)
                {
                    throw new CommonCode.UIException(string.Format("catid is < 1 , user id = {0}", CurrentUser.ID));
                }

                int beforeAddCatCount = businessCompany.CountCompanyCategories(objectContext, CurrentCompany);

                Category category = businessCategory.Get(objectContext, catid);
                if (category == null)
                {
                    throw new CommonCode.UIException(string.Format("There`s no category ID = {0}, selected from add category menu, user id = {1}",
                        catid, CurrentUser.ID));
                }

                if (businessCompany.CheckIfCategoryHaveNotAddedLastCategories(objectContext, CurrentCompany, category) == true)
                {
                    businessCompany.AddCompanyCategories(userContext, objectContext, category, CurrentCompany, CurrentUser, businessLog);

                    name.Text = string.Empty;

                    FillDeleteCompaniesCBL();
                    UpdateProductsNumber();

                    ShowAccordionsInfo();

                    int afterAddCatCount = businessCompany.CountCompanyCategories(objectContext, CurrentCompany);

                    UpdateCompanyNotification(-1);
                    LoadCategoryMenuWithItems();
                    FillDdlSortByCategories();

                    if ((afterAddCatCount - beforeAddCatCount) > 1)
                    {
                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("newCategoriesAdded").ToString());
                    }
                    else
                    {
                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("newCategoryAdded").ToString());
                    }
                }
                else
                {
                    error = GetLocalResourceObject("noLastCategories").ToString();


                    name.Text = string.Empty;
                }
            }

            lblERROR.Text = error;
        }

        private void CheckAddEditData(bool canEditAllCompanies)
        {
            // ------------ EDIT ---------------

            CheckCrucialEditOptions(canEditAllCompanies);

            /// description
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "tbAccEditDescription");
            description.Text = CurrentCompany.description;

            TextBox tbCount = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "tbSymbolsCount");
            tbCount.Text = description.Text.Length.ToString();

            /// edit characteristics
            FillEditCharacteristicsAP();

            /// categories
            FillDeleteCompaniesCBL();

            // web site
            Label websiteInfo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apChangeWebSite, "lblWebSiteInfo");
            websiteInfo.Text = GetLocalResourceObject("siteRules").ToString();

            apChangeType.Visible = false;

            CheckAddRemoveAlternativeNames();

            if (CurrentCompany.visible == true)
            {
                apAdd.Visible = true;

                // ---------------- ADD ----------------

                // image
                CheckAddImageOptions();

                // characteristic
                CheckAddCharacteristicsOptions();

                // categories
                LoadCategoryMenuWithItems();
            }
            else
            {
                apAdd.Visible = false;
            }
        }

        private void CheckCrucialEditOptions(bool canEditAllCompanies)
        {
            bool showAccordions = false;

            if (canEditAllCompanies == true)
            {
                showAccordions = true;
            }
            else
            {
                showAccordions = CheckIfTimeAfterWhichCrucialProductDataCanBeEditedDidntPassed();
            }

            if (showAccordions == true)
            {
                apChangeName.Visible = true;
            }
            else
            {
                apChangeName.Visible = false;
            }

        }

        private bool CheckIfTimeAfterWhichCrucialProductDataCanBeEditedDidntPassed()
        {
            bool result = false;

            TimeSpan span = new TimeSpan();
            span = DateTime.UtcNow - CurrentCompany.dateCreated;
            if (span.TotalMinutes < Configuration.ProdCompMinAfterWhichUserCannotEditCrucialData)
            {
                result = true;
            }

            return result;
        }


        private void CheckAddRemoveAlternativeNames()
        {
            BusinessAlternativeNames bAN = new BusinessAlternativeNames();

            int namesCount = bAN.CountAlternativeCompanyNames(objectContext, CurrentCompany, true);

            if (namesCount < Configuration.CompaniesMaxAlternativeNames)
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

            List<AlternativeCompanyName> names = bAN.GetVisibleAlternativeCompanyNames(objectContext, CurrentCompany);
            if (names != null && names.Count > 0)
            {
                apRemoveAlternativeNames.Visible = true;

                foreach (AlternativeCompanyName name in names)
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

        private void CheckAddCharacteristicsOptions()
        {
            BusinessCompany businessCompany = new BusinessCompany();

            if (businessCompany.CountCompanyCharacterestics(objectContext, CurrentCompany) < 20)
            {
                apAddCharacteristic.Visible = true;

                TextBox charName = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "tbCharName1");
                Label checkCharName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblCCName");
                Label charInfo1 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblAddCharInfo1");
                Label charInfo2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apAddCharacteristic, "lblAddCharInfo2");

                charName.Attributes.Add("onblur", string.Format("JSCheckData('{0}','companyCharAdd','{1}','{2}');",
                    charName.ClientID, checkCharName.ClientID, CurrentCompany.ID.ToString()));

                charInfo1.Text = string.Format("{0} {1}-{2} {3}", GetLocalResourceObject("CharTopicRules")
                    , Configuration.CompaniesMinCompanyNameLength, Configuration.CompaniesMaxCompanyNameLength
                    , GetLocalResourceObject("charRules"));
                charInfo2.Text = string.Format("{0} {1}-{2} {3}", GetLocalResourceObject("descrRules"),
                    Configuration.FieldsMinDescriptionFieldLength, Configuration.FieldsMaxDescriptionFieldLength
                    , GetLocalResourceObject("symbols"));
            }
            else
            {
                apAddCharacteristic.Visible = false;
            }
        }

        private void FillChangeTypeDDL()
        {
            BusinessCompanyType businessCompanyType = new BusinessCompanyType();
            List<CompanyType> types = businessCompanyType.GetAllCompanyTypes(objectContext, true, true);

            if (types.Count > 1)
            {
                apChangeType.Visible = true;

                DropDownList typesDdl = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apChangeType, "ddlChangeType");
                typesDdl.Items.Clear();

                Label info = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apChangeType, "lblChangeTypeInfo");

                if (!CurrentCompany.typeReference.IsLoaded)
                {
                    CurrentCompany.typeReference.Load();
                }

                foreach (CompanyType type in types)
                {
                    if (type != CurrentCompany.type)
                    {
                        ListItem newItem = new ListItem();
                        newItem.Text = type.name;
                        newItem.Value = type.ID.ToString();
                        typesDdl.Items.Add(newItem);
                    }
                }

                CompanyType selectedType = GetSelectedType(typesDdl);
                if (!string.IsNullOrEmpty(selectedType.description))
                {
                    info.Text = Tools.GetFormattedTextFromDB(selectedType.description);
                }
                else
                {
                    info.Text = "";
                }
            }
            else
            {
                apChangeType.Visible = false;
            }
        }

        private void FillDeleteCompaniesCBL()
        {
            BusinessCompany businessCompany = new BusinessCompany();
            BusinessUser businessUser = new BusinessUser();

            bool canEditAllCompanies = businessUser.CanAdminDo(userContext, CurrentUser, AdminRoles.EditCompanies);

            List<Category> compCategories = businessCompany.GetCompanyCategories(objectContext, CurrentCompany.ID).ToList();
            if (compCategories.Count<Category>() > 0)
            {
                BusinessCategory bCategory = new BusinessCategory();
                Category unspecified = bCategory.GetUnspecifiedCategory(objectContext);
                if (compCategories.Count == 1 && compCategories[0] == unspecified)
                {
                    apDeleteCategories.Visible = false;
                }
                else
                {
                    apDeleteCategories.Visible = true;

                    CheckBoxList cblDelCategories = (CheckBoxList)CommonCode.UiTools.GetControlFromAccordionPane(apDeleteCategories, "cblDeleteCategories");
                    cblDelCategories.Items.Clear();

                    Label info = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apDeleteCategories, "lblDeleteCategoriesInfo");
                    CheckBox deleteProducts = (CheckBox)CommonCode.UiTools.GetControlFromAccordionPane(apDeleteCategories, "cbAdminDeleteCategories");

                    if (canEditAllCompanies == true)
                    {
                        // admin ..can delete every category
                        deleteProducts.Visible = true;

                        foreach (Category category in compCategories)
                        {
                            if (unspecified != category)
                            {
                                ListItem newItem = new ListItem();
                                newItem.Text = Tools.TrimString(Tools.CategoryName(objectContext, category, false), 400, true, true);
                                newItem.Value = category.ID.ToString();
                                cblDelCategories.Items.Add(newItem);
                            }
                        }
                    }
                    else
                    {
                        // user .. can delete only categories in which there arent products
                        long prodCount = 0;
                        foreach (Category category in compCategories)
                        {
                            prodCount = businessCompany.CountNumberOfCompanyProductsInCategory(objectContext, CurrentCompany, category.ID);
                            if (prodCount == 0)
                            {
                                if (unspecified != category)
                                {
                                    ListItem newItem = new ListItem();
                                    newItem.Text = Tools.TrimString(Tools.CategoryName(objectContext, category, false), 400, true, true);
                                    newItem.Value = category.ID.ToString();
                                    cblDelCategories.Items.Add(newItem);
                                }
                            }
                        }

                        if (cblDelCategories.Items.Count == 0)
                        {
                            apDeleteCategories.Visible = false;
                        }
                    }

                    if (canEditAllCompanies)
                    {
                        info.CssClass = "searchPageRatings";
                        info.Text = string.Format("After deleting categories in which company have products, the products company will change to 'Other'.{0}"
                            , "<br />If not checked products in theese categories will be deleted.");
                    }
                    else
                    {
                        info.Text = GetLocalResourceObject("categoriesNoProducts").ToString();
                    }
                }
            }
            else
            {
                apDeleteCategories.Visible = false;
            }
        }

        private void FillEditCharacteristicsAP()
        {
            BusinessCompany businessCompany = new BusinessCompany();

            List<CompanyCharacterestics> compChar =
                       businessCompany.GetCompanyCharacterestics(objectContext, CurrentCompany.ID).ToList();

            if (compChar.Count<CompanyCharacterestics>() > 0)
            {
                apEditCharacteristics.Visible = true;

                DropDownList ddlChars = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "ddlEditCharacteristics");
                ddlChars.Items.Clear();

                TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbEditCharNewName");
                TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbEditCharDescription");
                Label info1 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblEditCharInfo1");
                Label info2 = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblEditCharInfo2");
                Label checkName = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "lblEditCharCHeckNewName");

                CompanyCharacterestics firstChar = compChar.First<CompanyCharacterestics>();

                foreach (CompanyCharacterestics cChar in compChar)   // Fills characteristics in drop down list
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = cChar.name;
                    newItem.Value = cChar.ID.ToString();
                    ddlChars.Items.Add(newItem);
                }

                info1.Text = GetLocalResourceObject("charRules2").ToString();
                info2.Text = string.Format("{0} {1}-{2} {3} {4} {5} - {6} {3}", GetLocalResourceObject("CharTopicRules")
                    , Configuration.CompaniesMinCompanyNameLength, Configuration.CompaniesMaxCompanyNameLength
                    , GetLocalResourceObject("characters"), GetLocalResourceObject("descrRules")
                    , Configuration.FieldsMinDescriptionFieldLength, Configuration.FieldsMaxDescriptionFieldLength);


                //name.Text = firstChar.name;
                description.Text = firstChar.description;

                name.Attributes.Add("onblur", string.Format("JSCheckData('{0}','companyCharAdd','{1}','{2}');",
                    name.ClientID, checkName.ClientID, CurrentCompany.ID.ToString()));
            }
            else
            {
                apEditCharacteristics.Visible = false;
            }
        }


        public Company GetCurrentCompany(Entities objectContext)
        {
            BusinessCompany businessCompany = new BusinessCompany();
            BusinessCategory businessCategory = new BusinessCategory();

            Company currCompany = new Company();

            String id = Request.Params["Company"];
            if (id != null || id != "")
            {

                long compid = -1;
                if (long.TryParse(id, out compid))
                {
                    currCompany = businessCompany.GetCompanyWV(objectContext, compid);
                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
                }

                if (currCompany == null)
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errNoSychCompany").ToString());
                }
                else if (currCompany == businessCompany.GetOther(objectContext))
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errCantSeeCompany").ToString());
                }
                else if (currCompany.visible == false)
                {
                    BusinessUser businessUser = new BusinessUser();

                    if (CurrentUser != null)
                    {
                        if (businessUser.CanAdminDo(userContext, CurrentUser, AdminRoles.EditCompanies))
                        {
                            // if user is admin or global
                        }
                        else
                        {
                            CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errCompanyDeleted").ToString());
                        }
                    }
                    else
                    {
                        CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errCompanyDeleted").ToString());
                    }
                }
            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errIncParameters").ToString());
            }

            if (currCompany == null)
            {
                throw new CommonCode.UIException("currCompany is null");
            }
            return currCompany;
        }

        protected void btnDeleteCompany_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyAllCompaniesFromEvents();

            if (CurrentCompany.visible == true)
            {
                BusinessCompany businessCompany = new BusinessCompany();

                List<Product> productsInUnsCat = new List<Product>();
                if (businessCompany.CheckIfCompanyHaveProductsInUnspecifiedCategory(objectContext, CurrentCompany, out productsInUnsCat) == false)
                {
                    bool changeProductCompany = cbDeleteAllCompanyProducts.Checked;

                    businessCompany.DeleteCompany(userContext, objectContext, CurrentCompany, CurrentUser, businessLog, changeProductCompany);

                    Session.Add("notifMsg", "Company deleted!");
                    RedirectToSameUrl(Request.Url.ToString());
                }
                else
                {
                    StringBuilder names = new StringBuilder();
                    names.Append("The company have products which are in unspecified category. Move them to be able to delete it.<br /> The products are : ");
                    foreach (Product product in productsInUnsCat)
                    {
                        names.Append(string.Format("{0} {1}, ", product.name, product.ID));
                    }

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, names.ToString());
                }
            }
        }

        protected void btnUndoDelete_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyAllCompaniesFromEvents();

            if (CurrentCompany.visible == false)
            {
                BusinessCompany businessCompany = new BusinessCompany();
                businessCompany.MakeVisibleCompany(objectContext, CurrentCompany, CurrentUser, businessLog);

                Session.Add("notifMsg", "Company is visible again !");
                RedirectToSameUrl(Request.Url.ToString());
            }
        }

        protected void CompanyEditorsFill()
        {
            tblCompEditors.Rows.Clear();

            BusinessUser businessUser = new BusinessUser();
            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

            IEnumerable<UsersTypeAction> usersAction = businessUserTypeActions.GetCompanyModificators(objectContext, CurrentCompany.ID);
            if (usersAction.Count<UsersTypeAction>() > 0)
            {
                tblCompEditors.Visible = true;
                lblCompEditors.Visible = true;

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

                TableCell remCell = new TableCell();
                remCell.Width = Unit.Pixel(10);
                remCell.Text = "Remove";
                newRow.Cells.Add(remCell);

                tblCompEditors.Rows.Add(newRow);

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

                    TableCell appCell = new TableCell();
                    if (approver != null && businessUser.IsUserValidType(approver))
                    {

                        appCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink(approver));
                    }
                    else if (approver != null)
                    {
                        appCell.Text = approver.username;
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format("action {0} whit user {1} ID {2} doesnt have approver"
                           , uact.ID, user.username, uact.User.ID));
                    }
                    userRow.Cells.Add(appCell);

                    TableCell delCell = new TableCell();
                    Button delBtn = new Button();
                    delBtn.Text = "Remove";
                    delBtn.Attributes.Add("userID", user.ID.ToString());
                    delBtn.Click += new EventHandler(RemoveRoleCompanyModificator);
                    delCell.Controls.Add(delBtn);
                    userRow.Cells.Add(delCell);

                    tblCompEditors.Rows.Add(userRow);

                }
            }
            else
            {
                tblCompEditors.Visible = false;
                lblCompEditors.Visible = false;
            }

        }

        protected void ACompanyProductsEditorsFill()
        {

            tblACompProdModificators.Rows.Clear();

            BusinessUser businessUser = new BusinessUser();
            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

            IEnumerable<UsersTypeAction> usersAction = businessUserTypeActions.GetAllCompanyProductsModificators(objectContext, CurrentCompany.ID);
            if (usersAction.Count<UsersTypeAction>() > 0)
            {
                tblACompProdModificators.Visible = true;
                lblACompProdModificators.Visible = true;

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

                TableCell remCell = new TableCell();
                remCell.Width = Unit.Pixel(10);
                remCell.Text = "Remove";
                newRow.Cells.Add(remCell);

                tblACompProdModificators.Rows.Add(newRow);

                foreach (UsersTypeAction uact in usersAction)
                {
                    if (!uact.UserReference.IsLoaded)
                    {
                        uact.UserReference.Load();
                    }

                    TableRow userRow = new TableRow();

                    TableCell actidCell = new TableCell();
                    actidCell.Text = uact.User.ID.ToString();
                    userRow.Cells.Add(actidCell);

                    User user = Tools.GetUserFromUserDatabase(userContext, uact.User);

                    userRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(user));

                    TableCell dtCell = new TableCell();
                    dtCell.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(uact.dateCreated);
                    userRow.Cells.Add(dtCell);

                    if (!uact.ApprovedByReference.IsLoaded)
                    {
                        uact.ApprovedByReference.Load();
                    }
                    User approver = Tools.GetUserFromUserDatabase(userContext, uact.ApprovedBy);

                    TableCell appCell = new TableCell();
                    if (approver != null && businessUser.IsUserValidType(approver))
                    {
                        appCell.Controls.Add(CommonCode.UiTools.GetUserHyperLink(approver));
                    }
                    else if (approver != null)
                    {
                        appCell.Text = approver.username;
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format("action {0} whit user {1} ID {2} doesnt have approver"
                            , uact.ID, user.username, uact.User.ID));
                    }
                    userRow.Cells.Add(appCell);

                    TableCell delCell = new TableCell();
                    Button delBtn = new Button();
                    delBtn.Text = "Remove";
                    delBtn.Attributes.Add("userID", uact.User.ID.ToString());
                    delBtn.Click += new EventHandler(RemoveRoleAllCompanyProductsModificator);
                    delCell.Controls.Add(delBtn);
                    userRow.Cells.Add(delCell);

                    tblACompProdModificators.Rows.Add(userRow);

                }
            }
            else
            {
                tblACompProdModificators.Visible = false;
                lblACompProdModificators.Visible = false;

            }

        }


        void RemoveRoleAllCompanyProductsModificator(object sender, EventArgs e)
        {
            CheckIfUserCanModifyAllCompaniesFromEvents();

            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

            Button btn = sender as Button;
            String usrID = btn.Attributes["userID"];
            long userID = -1;
            long.TryParse(usrID, out userID);
            if (userID < 1)
            {
                throw new CommonCode.UIException("userID is < 1");
            }

            businessUserTypeActions.RemoveUserActionAllCompanyProductsModificator
                (objectContext, userContext, userID, CurrentUser, CurrentCompany, businessLog);
            ACompanyProductsEditorsFill();

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "All company products modificator role removed from user!");
        }


        void RemoveRoleCompanyModificator(object sender, EventArgs e)
        {
            CheckIfUserCanModifyAllCompaniesFromEvents();

            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

            Button btn = sender as Button;
            String usrID = btn.Attributes["userID"];
            long userID = -1;
            long.TryParse(usrID, out userID);
            if (userID < 1)
            {
                throw new CommonCode.UIException("userID is < 1");
            }

            businessUserTypeActions.RemoveUserActionCompanyModificator
                (objectContext, userContext, userID, CurrentUser, CurrentCompany, businessLog);

            CompanyEditorsFill();

            FillPublicShownCompanyEditors();

            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Company modificator role removed from user!");
        }



        protected void btnGiveUserRoles_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyAllCompaniesFromEvents();

            phUserRoles.Controls.Add(lblERROR);
            phUserRoles.Visible = true;

            string error = "";
            String strUserId = tbUserRoles.Text;

            if (CurrentCompany.visible == false)
            {
                lblERROR.Text = "You cannot give roles to user while the company is Deleted!";
                return;
            }

            if (CommonCode.Validate.ValidateLong(strUserId, out error))
            {
                BusinessUser businessUser = new BusinessUser();
                BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

                long userId = -1;
                if (long.TryParse(strUserId, out userId))
                {
                    User userToAddRole = businessUser.Get(userContext, userId, false);
                    if (userToAddRole != null)
                    {
                        if (!userToAddRole.UserOptionsReference.IsLoaded)
                        {
                            userToAddRole.UserOptionsReference.Load();
                        }
                        if (userToAddRole.UserOptions.activated == true)
                        {
                            String type = userToAddRole.type;
                            if (businessUser.IsUser(userToAddRole))
                            {
                                if (!businessUser.CanUserModifyCompany(objectContext, CurrentCompany.ID, userToAddRole.ID))
                                {
                                    businessUserTypeActions.AddUserActionCompanyModificator(userContext, objectContext, userToAddRole
                                        , CurrentCompany, CurrentUser, businessLog, true);
                                    phUserRoles.Visible = false;
 
                                    CompanyEditorsFill();

                                    FillPublicShownCompanyEditors();

                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Company modificator roles given to user!");
                                }
                                else
                                {
                                    error = "The user already have this role";
                                }
                            }
                            else
                            {
                                error = "The user you are trying to give this role is not with type 'user'";
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
                else
                {
                    throw new CommonCode.UIException(string.Format("Couldnt parse strUserId to long, user id = {0}", CurrentUser.ID));
                }
            }

            lblERROR.Text = error;
            tbUserRoles.Text = "";
        }

        private void CheckPageParam()
        {
            BusinessCompany businessCompany = new BusinessCompany();
            BusinessProduct businessProduct = new BusinessProduct();
            //page - for page number

            String strPage = Request.Params["page"];
            int page = -1;

            UpdateProductsNumber();

            if (ProdNumber > 0)
            {
                if (strPage != null && strPage.Length > 0 && int.TryParse(strPage, out page))
                {
                    if (page > 1)
                    {
                        long expression = ProdOnPage * (page - 1);
                        if (ProdNumber > expression)
                        {
                            // page is valid
                            PageNum = page;
                        }
                        else
                        {
                            // invalid page
                            RedirectToOtherUrl(string.Format("Company.aspx?Company={0}", CurrentCompany.ID));
                        }
                    }
                    else if (page < 1)
                    {
                        RedirectToOtherUrl(string.Format("Company.aspx?Company={0}", CurrentCompany.ID));
                    }
                    else
                    {
                        PageNum = 1;
                    }

                }
                else
                {
                    if (strPage != null && strPage.Length > 0)
                    {
                        RedirectToOtherUrl(string.Format("Company.aspx?Company={0}", CurrentCompany.ID));
                    }
                }

            }
            else
            {
                // no entered products
            }
        }

        /// <summary>
        /// Updates Products number (depends if category is selected)
        /// </summary>
        private void UpdateProductsNumber()
        {
            BusinessCompany businessCompany = new BusinessCompany();

            if (CategoryId > 0)
            {
                ProdNumber = businessCompany.CountNumberOfCompanyProductsInCategory(objectContext, CurrentCompany, CategoryId);
            }
            else
            {
                ProdNumber = businessCompany.CountNumberOfProductsInCompany(objectContext, CurrentCompany);
            }
        }


        private void ShowProducts()
        {
            tblProducts.Rows.Clear();

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessProduct businessProduct = new BusinessProduct();
            BusinessRating businessRating = new BusinessRating();
            BusinessUser businessUser = new BusinessUser();

            if (ProdNumber > 0)
            {
                TableRow firstRow = new TableRow();

                TableCell nameFCell = new TableCell();
                nameFCell.Text = string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}", GetLocalResourceObject("Products").ToString());
                firstRow.Cells.Add(nameFCell);

                TableCell dateFCell = new TableCell();
                dateFCell.Text = GetLocalResourceObject("dateAdded").ToString();
                dateFCell.Width = Unit.Pixel(110);
                firstRow.Cells.Add(dateFCell);

                TableCell commFCell = new TableCell();
                commFCell.CssClass = "searchPageComments";
                commFCell.Text = GetLocalResourceObject("Comments").ToString();
                commFCell.HorizontalAlign = HorizontalAlign.Center;
                commFCell.Width = Unit.Pixel(100);
                firstRow.Cells.Add(commFCell);

                TableCell rateFCell = new TableCell();
                rateFCell.CssClass = "searchPageRatings";
                rateFCell.Text = string.Format("{0}", GetLocalResourceObject("Rating").ToString());
                rateFCell.Width = Unit.Pixel(90);
                rateFCell.HorizontalAlign = HorizontalAlign.Center;
                firstRow.Cells.Add(rateFCell);

                tblProducts.Rows.Add(firstRow);

                long from = ProdOnPage * (PageNum - 1);
                long to = ProdOnPage * PageNum;

                List<Product> products;
                if (CategoryId > 0)
                {
                    products = businessProduct.GetAllProductsFromCompanyWithCategory(objectContext, CurrentCompany, CategoryId, from, to);
                }
                else
                {
                    products = businessProduct.GetAllProductsFromCompany(objectContext, CurrentCompany.ID, from, to);
                }


                int i = 0;
                foreach (Product product in products)
                {
                    ShowProduct(businessProduct, businessRating, businessUser, product, i);
                    i++;
                }
            }
            else
            {
                TableRow newRow = new TableRow();
                TableCell newCell = new TableCell();
                newCell.CssClass = "searchPageComments";
                newCell.Text = GetLocalResourceObject("NoAddedProducts").ToString();
                newRow.Cells.Add(newCell);
                tblProducts.Rows.Add(newRow);
            }
        }

        private void ShowProduct(BusinessProduct businessProduct, BusinessRating businessRating
            , BusinessUser businessUser, Product currProduct, int rowNum)
        {

            TableRow newRow = new TableRow();
            newRow.CssClass = "hoverBGR";

            Image newImg = new Image();
            newImg.ImageUrl = "~/images/SiteImages/triangle.png";
            newImg.CssClass = "itemImage";
            newImg.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, "5px");
            newImg.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginRight, "10px");

            string contPageId = pnlPopUp.ClientID.Substring(0, pnlPopUp.ClientID.Length - pnlPopUp.ID.Length);

            TableCell nameCell = new TableCell();
            nameCell.Controls.Add(newImg);
            HyperLink productLink = CommonCode.UiTools.GetProductHyperLink(currProduct);
            productLink.ID = string.Format("prod{0}lnk", currProduct.ID);
            productLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}{2}','{3}')", currProduct.ID, contPageId, productLink.ClientID, pnlPopUp.ClientID));
            productLink.Attributes.Add("onmouseout", "HideData()");
            nameCell.Controls.Add(productLink);
            newRow.Cells.Add(nameCell);


            TableCell dateCell = new TableCell();
            dateCell.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(currProduct.dateCreated);
            newRow.Cells.Add(dateCell);

            TableCell commCell = new TableCell();
            commCell.CssClass = "searchPageComments";
            commCell.HorizontalAlign = HorizontalAlign.Center;
            commCell.Text = currProduct.comments.ToString();
            newRow.Cells.Add(commCell);

            TableCell rateCell = new TableCell();
            rateCell.HorizontalAlign = HorizontalAlign.Center;
            rateCell.CssClass = "searchPageRatings";
            rateCell.Text = businessRating.GetProductRating(currProduct);
            newRow.Cells.Add(rateCell);

            tblProducts.Rows.Add(newRow);
        }

        private string getCurrCompanyLink(Company currCompany)
        {
            if (currCompany == null)
            {
                throw new CommonCode.UIException("currCompany is null");
            }

            string url = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", currCompany.ID));
            return url;
        }

        private void ShowPages(Table someTable)
        {
            someTable.Rows.Clear();
            string compURL = getCurrCompanyLink(CurrentCompany);

            if (CategoryId > 0)
            {
                compURL += "&cat=" + CategoryId;
            }

            TableRow newRow = CommonCode.Pages.GetPagesRow(ProdNumber, ProdOnPage, PageNum, compURL);
            someTable.Rows.Add(newRow);
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            PlaceHolder phImage = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "phAddImage");

            phImage.Controls.Add(lblERROR);
            phImage.Visible = true;
            string error = "";

            FileUpload fImage = (FileUpload)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "fuImage");

            if (fImage.HasFile)
            {
                string imageErrorDescription = string.Empty;
                byte[] fileBytes = fuImage.FileBytes;
                string fileName = fuImage.FileName;
                string fileType = System.IO.Path.GetExtension(fileName);

                CheckBox cLogo = (CheckBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "cbLogo");

                bool imageOk = ImageTools.IsValidImage(fileName, fileBytes, ref imageErrorDescription, cLogo.Checked);
                if (imageOk == true)
                {
                    TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddImage, "tbImageDescr");

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
                                CompanyImage newImage = new CompanyImage();
                                newImage.Company = CurrentCompany;
                                newImage.url = "";
                                newImage.dateCreated = DateTime.UtcNow;
                                newImage.CreatedBy = Tools.GetUserID(objectContext, CurrentUser);
                                newImage.description = strDescr;
                                newImage.width = width;
                                newImage.height = height;
                                newImage.isThumbnail = false;
                                newImage.mainImgID = null;
                                newImage.isLogo = cLogo.Checked;
                                imageTools.AddCompanyImage(userContext, objectContext, newImage, businessLog, CurrentUser);

                                //file name function
                                String path;
                                String url;
                                CommonCode.ImagesAndAdverts.GenerateCompanyImageNamePathUrl(fileType, CurrentCompany, newImage, out path, out url);

                                string appPath = CommonCode.PathMap.PhysicalApplicationPath;
                                string imageCompletePath = System.IO.Path.Combine(appPath, path);
                                fuImage.SaveAs(imageCompletePath);

                                imageTools.ChangeCompanyImageUrl(objectContext, newImage, url);

                                CommonCode.ImagesAndAdverts.GenerateCompanyThumbnail(userContext, objectContext, businessLog
                                    , newImage, CurrentCompany, CurrentUser, fileBytes, appPath);
                            }

                            cLogo.Checked = false;

                            CheckAddImageOptions();
                            ShowInfo();

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                , GetLocalResourceObject("imageUploaded").ToString());
                        }
                        else
                        {
                            error = GetLocalResourceObject("errIncImageWH").ToString();
                        }
                    }

                }
                else
                {
                    error = string.IsNullOrEmpty(imageErrorDescription) ? "Error uploading the file." : imageErrorDescription;
                }
            }
            else
            {
                error = GetLocalResourceObject("chooseFileUpload").ToString();
            }

            lblERROR.Text = error;
        }

        public void FillGalleryTable()
        {
            tblGallery.Rows.Clear();

            BusinessUser businessUser = new BusinessUser();
            ImageTools imageTools = new ImageTools();

            Boolean canEdit = false;
            if (CurrentUser != null)
            {
                if (businessUser.CanUserModifyCompany(objectContext, CurrentCompany.ID, CurrentUser.ID) ||
                    businessUser.CanAdminDo(userContext, CurrentUser, AdminRoles.EditCompanies))
                {
                    canEdit = true;
                }
            }

            string appPath = CommonCode.PathMap.PhysicalApplicationPath;  // CommonCode.PathMap.GetImagesPhysicalPathRoot();
            List<CompanyImage> images = imageTools.GetCompanyThumbnails(userContext, objectContext, CurrentCompany, businessLog, appPath);
            int count = images.Count<CompanyImage>();
            if (count > 0)
            {
                int FieldsPerRow = 5;
                int i = 0;

                TableRow newRow = new TableRow();
                tblGallery.Rows.Add(newRow);

                foreach (CompanyImage image in images)
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

                    CompanyImage parentImage = imageTools.GetCompanyImage(objectContext, image.mainImgID.Value);
                    if (parentImage == null)
                    {
                        throw new CommonCode.UIException(string.Format("Thumbnail`s ID = {0} parentImage is null , shouldnt happen as there is check when getting all thumbnails", image.ID));
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
                        delBtn.Text = GetGlobalResourceObject("SiteResources", "delete").ToString();
                        delBtn.ID = string.Format("DeleteImageButton{0}", image.ID);
                        delBtn.Attributes.Add("ID", image.ID.ToString());
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
                noDataCell.Text = GetLocalResourceObject("noImages").ToString();
                noDataRow.Cells.Add(noDataCell);
                tblGallery.Rows.Add(noDataRow);
            }
        }

        void delBtn_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            Button btn = sender as Button;
            if (btn != null)
            {
                long id = 0;
                if (btn.Attributes["ID"] != null)
                {
                    if (long.TryParse(btn.Attributes["ID"], out id))
                    {
                        ImageTools imageTools = new ImageTools();
                        CompanyImage currImage = imageTools.GetCompanyImage(objectContext, id);

                        if (currImage != null)
                        {
                            string appPath = CommonCode.PathMap.PhysicalApplicationPath;
                            if (log.IsInfoEnabled == true)
                            {
                                string logMsg = ImageTools.DeletingImageLogMessage(currImage.ID, currImage.url, "user request");
                                log.Info(logMsg);
                            }

                            if (imageTools.DeleteCompanyImage(userContext, objectContext, currImage, businessLog, appPath, CurrentUser) == true)
                            {
                                ShowInfo();
                                CheckAddImageOptions();

                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                    , GetLocalResourceObject("imageDeleted").ToString());
                            }
                            else
                            {
                                ShowInfo();
                                CheckAddImageOptions();
                                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                    , GetLocalResourceObject("errCantDelImage").ToString());
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("Image ID = {0} is null , user id = {1}", id, CurrentUser.ID));
                        }
                    }
                    else
                    {
                        throw new CommonCode.UIException(string.Format("btn.Attributes['ID'] to long , user id = {0}", CurrentUser.ID));
                    }
                }
            }
            else
            {
                throw new CommonCode.UIException("Delete image method (delBtn_Click) doesnt recognise parent button");
            }
        }

        protected void btnGiveRolesForAllProducts_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyAllCompaniesFromEvents();

            phACompProdRoles.Controls.Add(lblERROR);
            phACompProdRoles.Visible = true;

            string error = "";
            String strUserId = tbRolesForAllProducts.Text;

            if (CurrentCompany.visible == false)
            {
                lblERROR.Text = "You cannot give roles to user while the company is Deleted!";
                return;
            }

            if (CommonCode.Validate.ValidateLong(tbRolesForAllProducts.Text, out error))
            {
                BusinessUser businessUser = new BusinessUser();
                BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

                long userId = -1;
                if (long.TryParse(strUserId, out userId))
                {
                    User userToAddRole = businessUser.Get(userContext, userId, false);
                    if (userToAddRole != null)
                    {
                        if (!userToAddRole.UserOptionsReference.IsLoaded)
                        {
                            userToAddRole.UserOptionsReference.Load();
                        }
                        if (userToAddRole.UserOptions.activated == true)
                        {
                            if (businessUser.IsUser(userToAddRole))
                            {
                                if (!businessUser.CanUserModifyAllCompanyProducts(objectContext, CurrentCompany.ID, userToAddRole.ID))
                                {
                                    businessUserTypeActions.AddUserActionAllCompanyProductsModificator
                                        (userContext, objectContext, userToAddRole, CurrentCompany, CurrentUser, businessLog, true);
                                    phACompProdRoles.Visible = false;
                                    ACompanyProductsEditorsFill();

                                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Company product modificator roles given to user!");
                                }
                                else
                                {
                                    error = "The user already have this role";
                                }
                            }
                            else
                            {
                                error = "The user you are trying to give this role is not with type 'user'";
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
                else
                {
                    throw new CommonCode.UIException(string.Format("Couldnt parse strUserId to long, shouldnt happen as there is check earlier , user id = {0}", CurrentUser.ID));
                }
            }

            lblERROR.Text = error;
            tbRolesForAllProducts.Text = "";
        }

        private CompanyType GetSelectedType(DropDownList ddlist)
        {
            BusinessCompanyType businessCompanyType = new BusinessCompanyType();

            if (ddlist == null)
            {
                throw new CommonCode.UIException("ddlist is null");
            }

            string strTypeId = ddlist.SelectedValue;
            if (string.IsNullOrEmpty(strTypeId))
            {
                throw new CommonCode.UIException(string.Format("ddlist.SelectedValue is empty or null, user id = {0}"
                    , CurrentUser.ID));
            }
            long typeId = -1;
            if (!long.TryParse(strTypeId, out typeId))
            {
                throw new CommonCode.UIException(string.Format("couldnt parse ddlType.SelectedValue to long, user id = {0}"
                    , CurrentUser.ID));
            }

            CompanyType selectedType = businessCompanyType.Get(objectContext, typeId, true);
            if (selectedType == null)
            {
                throw new CommonCode.UIException(string.Format("There`s no type ID = {0} , or it is visible=false, user id = {1}"
                    , typeId, CurrentUser.ID));
            }

            return selectedType;
        }


        [WebMethod]
        public static string CheckData(string text, string type, string compId)
        {
            string error = "";

            CommonCode.WebMethods.ValidateUserInput(text, type, compId, out error);

            return error;
        }

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }

        private void RedirrectToCurrCompanyPage()
        {
            RedirectToOtherUrl("Company.aspx?Company=" + CurrentCompany.ID);
        }

        protected void ddlSortByCat_SelectedIndexChanged(object sender, EventArgs e)
        {

            BusinessCompany businessCompany = new BusinessCompany();
            if (ddlSortByCat.SelectedIndex == 0)
            {
                RedirrectToCurrCompanyPage();
            }

            long catId = -1;

            if (long.TryParse(ddlSortByCat.SelectedValue, out catId))
            {
                BusinessCategory businessCategory = new BusinessCategory();

                CategoryCompany currCatCompany = businessCompany.GetCategoryCompany(objectContext, CurrentCompany.ID, catId, true);
                if (currCatCompany == null)
                {
                    throw new CommonCode.UIException(string.Format("Company ID = {0} cant have products in category ID = {1}"
                        , CurrentCompany.ID, catId));
                }
                Category selectedCategory = businessCategory.Get(objectContext, catId);
                if (selectedCategory == null)
                {
                    throw new CommonCode.UIException(string.Format("There is no category with id = {0}", catId));
                }

                RedirectToOtherUrl(string.Format("Company.aspx?Company={0}&cat={1}", CurrentCompany.ID, catId));
            }
            else
            {
                throw new CommonCode.UIException(string.Format("Couldnt parse ddlSortByCat.SelectedValue to long , Company id = {0}", CurrentCompany.ID));
            }
        }

        private void CheckSortByParams()
        {
            String strCategory = Request.Params["cat"];

            if (!string.IsNullOrEmpty(strCategory))
            {
                long catId = -1;
                if (long.TryParse(strCategory, out catId))
                {
                    BusinessCategory businessCategory = new BusinessCategory();
                    Category selectedCat = businessCategory.Get(objectContext, catId);
                    if (selectedCat == null)
                    {
                        RedirrectToCurrCompanyPage();
                    }

                    BusinessCompany businessCompany = new BusinessCompany();
                    if (businessCompany.CheckIfCompanyCanHaveProductsInCategory(objectContext, CurrentCompany, selectedCat))
                    {
                        CategoryId = catId;
                    }
                    else
                    {
                        RedirrectToCurrCompanyPage();
                    }
                }
                else
                {
                    RedirrectToCurrCompanyPage();
                }
            }
        }

        protected void dbCancelAddChar_Click(object sender, EventArgs e)
        {
            tbCharName1.Text = "";
            tbCharDescr1.Text = "";
            pnlAddChar.Visible = false;
        }

        protected void dbCancelImage_Click(object sender, EventArgs e)
        {
            tbImageDescr.Text = "";
            pnlAddImage.Visible = false;
        }

        protected void dbCancelCategory_Click(object sender, EventArgs e)
        {
            tbCatName1.Text = "";
            pnlAddCategory.Visible = false;
        }

        protected void dbEditDescription_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            BusinessCompany businessCompany = new BusinessCompany();
            string error = "";

            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditDescription, "tbAccEditDescription");
            phEditDescription.Visible = true;
            phEditDescription.Controls.Add(lblERROR);

            string strDescr = description.Text;

            if (CommonCode.Validate.ValidateDescription(Configuration.CompaniesMinDescriptionLength,
                        Configuration.CompaniesMaxDescriptionLength, ref strDescr, "description", out error, 110)
                        && description.Text != CurrentCompany.description)
            {
                businessCompany.ChangeCompanyDescription(objectContext, CurrentCompany, strDescr, CurrentUser, businessLog);
                ShowInfo();

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("DescrChanged").ToString());
            }
            else if (string.IsNullOrEmpty(error))
            {
                error = GetLocalResourceObject("errChangeDescr").ToString();
            }

            lblERROR.Text = error;
        }

        protected void ddlEditCharacteristics_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            DropDownList editChars = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "ddlEditCharacteristics");

            long itemID = -1;
            if (!long.TryParse(editChars.SelectedValue, out itemID))
            {
                throw new CommonCode.UIException(string.Format
                ("Couldnt parse 'ddlEditCharacteristics.SelectedValue' = '{0}' to long, User Id = {1}, Company Id = {2}"
                , editChars.SelectedValue, CurrentUser.ID, CurrentCompany.ID));
            }

            BusinessCompany businessCompany = new BusinessCompany();

            CompanyCharacterestics companyChar =
            businessCompany.GetCompanyCharacterestic(objectContext, itemID);

            if (companyChar != null)
            {
                TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbEditCharNewName");
                TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbEditCharDescription");

                name.Text = "";
                description.Text = companyChar.description;
            }
            else
            {
                FillEditCharacteristicsAP();
            }
        }

        protected void dbEditCharacteristic_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();
            BusinessCompany businessCompany = new BusinessCompany();

            DropDownList editChatDdl = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "ddlEditCharacteristics");

            phEditCharacteristic.Visible = true;
            phEditCharacteristic.Controls.Add(lblERROR);
            string error = string.Empty;

            long charID = -1;
            long.TryParse(editChatDdl.SelectedValue, out charID);

            CompanyCharacterestics companyChar =
                businessCompany.GetCompanyCharacterestic(objectContext, charID);

            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbEditCharNewName");
            TextBox description = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "tbEditCharDescription");

            if (companyChar == null)
            {
                lblERROR.Text = GetLocalResourceObject("errNoSuchChar").ToString();

                name.Text = string.Empty;
                description.Text = string.Empty;

                FillEditCharacteristicsAP();
                return;
            }


            Boolean changeDone = false;
            bool updateDDl = false;

            if (name.Text == companyChar.name && description.Text == companyChar.description)
            {
                error = GetLocalResourceObject("errEditCharTypeNewNameOrDescr").ToString();
            }
            else
            {
                String newName = name.Text;
                if (newName != null && newName.Length > 0)
                {
                    if (CommonCode.Validate.ValidateName(objectContext, "compChar", ref newName, Configuration.CompaniesMinCompanyNameLength,
                       Configuration.CompaniesMaxCompanyNameLength, out error, CurrentCompany.ID))
                    {
                        businessCompany.ChangeCompanyCharacteristicName
                            (objectContext, companyChar, newName, CurrentUser, businessLog);
                        changeDone = true;
                        updateDDl = true;

                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("CharTopicChanged").ToString());
                    }
                }

                String newDescr = description.Text;
                if (newDescr != null && newDescr.Length > 0)
                {
                    if (newDescr != companyChar.description)
                    {
                        if (CommonCode.Validate.ValidateDescription(Configuration.FieldsMinDescriptionFieldLength,
                           Configuration.FieldsMaxDescriptionFieldLength, ref newDescr, "description", out error, 120))
                        {
                            businessCompany.ChangeCompanyCharacteristicDescription
                                (objectContext, companyChar, newDescr, CurrentUser, businessLog);
                            changeDone = true;

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                                , GetLocalResourceObject("charDescrUpdated").ToString());
                        }
                    }
                    else if (changeDone == false && string.IsNullOrEmpty(error))
                    {
                        error = GetLocalResourceObject("errEditCharTypeNewDescr").ToString();
                    }
                }
            }
            if (name.Text == string.Empty && description.Text == string.Empty)
            {
                error = GetLocalResourceObject("errEditCharTypeNewNameOrDescr").ToString();
            }
            if (changeDone)
            {
                name.Text = string.Empty;
                description.Text = string.Empty;

                if (updateDDl)
                {
                    FillEditCharacteristicsAP();
                }

                ShowInfo();
            }

            lblERROR.Text = error;
        }

        protected void dbDeleteCharacteristic_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            BusinessCompany businessCompany = new BusinessCompany();

            DropDownList editCharDdl = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "ddlEditCharacteristics");

            long charID = -1;
            long.TryParse(editCharDdl.SelectedValue, out charID);

            CompanyCharacterestics companyChar =
                businessCompany.GetCompanyCharacterestic(objectContext, charID);

            if (companyChar != null)
            {
                businessCompany.DeleteCompanyCharacteristic(userContext, objectContext, companyChar, CurrentUser, businessLog);

                FillEditCharacteristicsAP();

                ShowAccordionsInfo();

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, GetLocalResourceObject("charDeleted").ToString());
            }
            else
            {
                PlaceHolder phChar = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apEditCharacteristics, "phEditCharacteristic");
                phChar.Visible = true;
                phChar.Controls.Add(lblERROR);

                lblERROR.Text = GetLocalResourceObject("errNoSuchChar").ToString();
                FillEditCharacteristicsAP();
                return;
            }
        }

        protected void dbDeleteCategories_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessUser businessUser = new BusinessUser();

            bool isAdmin = false;
            if (businessUser.CanAdminDo(userContext, CurrentUser, AdminRoles.EditCompanies))
            {
                isAdmin = true;
            }

            List<CategoryCompany> companyCategories = new List<CategoryCompany>();

            string strCompCatID = string.Empty;
            long compCatID = -1;

            PlaceHolder phDelCategoris = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apDeleteCategories, "phDeleteCategories");
            phDeleteCategories.Controls.Add(lblERROR);
            phDelCategoris.Visible = true;
            string error = string.Empty;

            CategoryCompany currCompCat = null;

            CheckBoxList CategoriesCBL = (CheckBoxList)CommonCode.UiTools.GetControlFromAccordionPane(apDeleteCategories, "cblDeleteCategories");

            bool changeProductsCompany = true;
            if (isAdmin)
            {
                CheckBox cbDeleteProducts = (CheckBox)CommonCode.UiTools.GetControlFromAccordionPane(apDeleteCategories, "cbAdminDeleteCategories");
                changeProductsCompany = cbDeleteProducts.Checked;
            }

            long countDeletedProducts = 0;
            long productsInCategory = 0;

            bool updateCategoriesCheckboxes = false;

            for (int i = 0; i < CategoriesCBL.Items.Count; i++)
            {
                if (CategoriesCBL.Items[i].Selected == true)
                {
                    strCompCatID = CategoriesCBL.Items[i].Value;
                    if (!long.TryParse(strCompCatID, out compCatID))
                    {
                        throw new CommonCode.UIException(string.Format("couldnt parse cblDeleteCategories.Items[{0}] to long, user id = {1}",
                            i, CurrentUser.ID));
                    }

                    currCompCat = businessCompany.GetCategoryCompany(objectContext, CurrentCompany.ID, compCatID, true);

                    if (currCompCat == null)
                    {
                        currCompCat = businessCompany.GetCategoryCompany(objectContext, CurrentCompany.ID, compCatID, false);

                        updateCategoriesCheckboxes = true;

                        if (currCompCat != null)
                        {
                            if (!currCompCat.CategoryReference.IsLoaded)
                            {
                                currCompCat.CategoryReference.Load();
                            }

                            error += string.Format("{0} {1} {2}<br/ >", GetLocalResourceObject("errThereIsntSuchCat")
                                , Tools.CategoryName(objectContext, currCompCat.Category, false), GetLocalResourceObject("errThereIsntSuchCat2"));
                        }

                    }
                    else
                    {

                        productsInCategory = businessCompany.CountNumberOfCompanyProductsInCategory(objectContext, CurrentCompany, compCatID);
                        countDeletedProducts += productsInCategory;

                        if (isAdmin == false && productsInCategory > 0)
                        {
                            updateCategoriesCheckboxes = true;

                            if (!currCompCat.CategoryReference.IsLoaded)
                            {
                                currCompCat.CategoryReference.Load();
                            }

                            error += string.Format("{0} {1} {2}<br />", GetLocalResourceObject("errCantDelCatCusThereAreProducts")
                                , Tools.CategoryName(objectContext, currCompCat.Category, false), GetLocalResourceObject("errCantDelCatCusThereAreProducts2"));

                        }
                        else
                        {
                            companyCategories.Add(currCompCat);
                        }
                    }
                }
            }

            int compCategoriesCount = companyCategories.Count;

            if (compCategoriesCount > 0)
            {
                bool redirrectAfterDeleting = false;

                foreach (CategoryCompany compCat in companyCategories)
                {
                    if (!compCat.CategoryReference.IsLoaded)
                    {
                        compCat.CategoryReference.Load();
                    }
                    if (CategoryId == compCat.Category.ID)
                    {
                        redirrectAfterDeleting = true;
                    }

                    businessCompany.DeleteCompanyCategory(objectContext, userContext, compCat, CurrentUser, businessLog, changeProductsCompany, !isAdmin);
                }

                string mainMsg = string.Empty;
                string secMsg = string.Empty;
                if (compCategoriesCount > 1)
                {
                    mainMsg = string.Format("{0}", GetLocalResourceObject("categoriesRemoved"));

                    if (changeProductsCompany)
                    {
                        secMsg = string.Format("<br /> {0} products from that category (if there were any) had their companies changed to Other."
                            , Tools.GetStringWithCapital(Configuration.CompanyName));
                    }
                    else
                    {
                        secMsg = string.Format("<br /> {0} products from that category (if there were any) were deleted."
                            , Tools.GetStringWithCapital(Configuration.CompanyName));
                    }

                }
                else
                {
                    mainMsg = string.Format("{0}", GetLocalResourceObject("categoriesRemoved2"));

                    if (changeProductsCompany)
                    {
                        secMsg = string.Format("<br /> {0} products from theese categories (if there were any) had their company changed to Other."
                            , Tools.GetStringWithCapital(Configuration.CompanyName));

                    }
                    else
                    {
                        secMsg = string.Format("<br /> {0} products from theese categories (if there were any) were deleted."
                            , Tools.GetStringWithCapital(Configuration.CompanyName));
                    }

                }

                if (redirrectAfterDeleting == true)
                {
                    if (isAdmin == false)
                    {
                        Session.Add("notifMsg", mainMsg);
                    }
                    else
                    {
                        Session.Add("notifMsg", string.Format("{0}{1}", mainMsg, secMsg));
                    }

                    RedirrectToCurrCompanyPage();
                }
                else
                {
                    if (isAdmin == false)
                    {
                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, mainMsg);
                    }
                    else
                    {
                        if (countDeletedProducts > 0)
                        {
                            Session.Add("notifMsg", string.Format("{0}{1}", mainMsg, secMsg));
                            RedirrectToCurrCompanyPage();
                        }
                        else
                        {
                            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, string.Format("{0}{1}", mainMsg, secMsg));
                        }
                    }
                }


                FillDeleteCompaniesCBL();
                LoadCategoryMenuWithItems();

                UpdateProductsNumber();
                ShowProducts();
                ShowAccordionsInfo();
                UpdateCompanyNotification(-1);
                FillDdlSortByCategories();
            }
            else
            {
                if (string.IsNullOrEmpty(error))
                {
                    error = GetLocalResourceObject("selectCategory").ToString();
                }

                if (updateCategoriesCheckboxes == true)
                {
                    FillDeleteCompaniesCBL();
                    LoadCategoryMenuWithItems();
                    ShowCategories();
                    FillDdlSortByCategories();
                }
            }

            lblERROR.Text = error;
        }

        protected void dbChangeName_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            if (CurrentUser != null)
            {
                if (!businessUser.CanAdminDo(userContext, CurrentUser, AdminRoles.EditCompanies))
                {
                    CheckIfUserCanModifyCompanyFromEvents();

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


            BusinessCompany businessCompany = new BusinessCompany();

            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apChangeName, "tbEditName");

            String newCompName = name.Text;
            bool changeName = false;
            string error = "";

            if (string.Equals(CurrentCompany.name, newCompName, StringComparison.InvariantCultureIgnoreCase) == true
                && string.Equals(CurrentCompany.name, newCompName, StringComparison.InvariantCulture) == false)
            {
                changeName = true;
            }
            else
            {
                changeName = CommonCode.Validate.ValidateName(objectContext, "companies", ref newCompName, Configuration.CompaniesMinCompanyNameLength,
                Configuration.CompaniesMaxCompanyNameLength, out error, 0);
            }

            if (changeName == true)
            {
                businessCompany.ChangeCompanyName(objectContext, CurrentCompany, newCompName, CurrentUser, businessLog);
                ShowInfo();

                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification,
                    GetLocalResourceObject("CompNameUpdated").ToString());
            }
            else
            {
                PlaceHolder phChngName = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apChangeName, "phChangeName");
                phChngName.Visible = true;
                phChngName.Controls.Add(lblERROR);
                lblERROR.Text = error;
            }
        }

        protected void dbEditWebite_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            BusinessCompany businessCompany = new BusinessCompany();

            PlaceHolder phEditSite = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apChangeWebSite, "phEditWebsite");
            phEditSite.Visible = true;
            phEditSite.Controls.Add(lblERROR);
            string error = "";

            TextBox site = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apChangeWebSite, "tbEditWebsite");

            String newSiteAdress = string.Empty;
            if (!string.IsNullOrEmpty(site.Text))
            {
                newSiteAdress = CommonCode.UiTools.GetCorrectedUrl(site.Text);
            }

            if (newSiteAdress != CurrentCompany.website)
            {
                if (string.IsNullOrEmpty(newSiteAdress))
                {
                    businessCompany.ChangeCompanyWebSite(objectContext, CurrentCompany, newSiteAdress, CurrentUser, businessLog);
                    ShowInfo();

                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification
                        , GetLocalResourceObject("siteChanged").ToString());
                }
                else
                {
                    if (CommonCode.Validate.ValidateSiteAdress(newSiteAdress, out error, false))
                    {
                        businessCompany.ChangeCompanyWebSite(objectContext, CurrentCompany, newSiteAdress, CurrentUser, businessLog);
                        ShowInfo();

                        CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification,
                            GetLocalResourceObject("siteChanged").ToString());
                    }
                }
            }
            else
            {
                error = GetLocalResourceObject("errorSite").ToString();
            }

            lblERROR.Text = error;
        }

        protected void dbChangeType_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            BusinessCompany businessCompany = new BusinessCompany();

            DropDownList ddlTypes = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apChangeType, "ddlChangeType");

            CompanyType selectedType = GetSelectedType(ddlTypes);

            if (!CurrentCompany.typeReference.IsLoaded)
            {
                CurrentCompany.typeReference.Load();
            }

            if (CurrentCompany.type == selectedType)
            {
                throw new CommonCode.UIException(string.Format("cannot change company type to current one , user id = {0}"
                    , CurrentUser.ID));
            }

            businessCompany.ChangeCompanyType(objectContext, CurrentCompany, selectedType, CurrentUser, businessLog);

            FillChangeTypeDDL();
            ShowInfo();
            CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Type updated!");
        }

        protected void ddlChangeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            DropDownList ddlTypes = (DropDownList)CommonCode.UiTools.GetControlFromAccordionPane(apChangeType, "ddlChangeType");

            long itemID = -1;
            if (!long.TryParse(ddlTypes.SelectedValue, out itemID))
            {
                throw new CommonCode.UIException(string.Format
                ("Couldnt parse 'ddlChangeType.SelectedValue' = '{0}' to long, User Id = {1}, Company Id = {2}"
                , ddlTypes.SelectedValue, CurrentUser.ID, CurrentCompany.ID));
            }

            CompanyType selectedType = GetSelectedType(ddlTypes);

            Label typeInfo = (Label)CommonCode.UiTools.GetControlFromAccordionPane(apChangeType, "lblChangeTypeInfo");

            if (!string.IsNullOrEmpty(selectedType.description))
            {
                typeInfo.Text = Tools.GetFormattedTextFromDB(selectedType.description);
            }
            else
            {
                typeInfo.Text = "";
            }
        }


        [WebMethod]
        public static string WMSendReport(string type, string strTypeId, string description)
        {
            return CommonCode.WebMethods.SendReport(type, strTypeId, description);
        }

        [WebMethod]
        public static string WMSignForNotifies(string type, string Id)
        {
            return CommonCode.WebMethods.SignForNotifies(type, Id);
        }

        [WebMethod]
        public static string WMSendTypeSuggestionToUser(string userID, string type, string typeID, string description)
        {
            return CommonCode.WebMethods.SendTypeSuggestion(userID, type, typeID, description);
        }

        protected void btnChangeCanUserTakeRole_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyAllCompaniesFromEvents();

            BusinessCompany bCompany = new BusinessCompany();
            bCompany.ChangeIfUsersCanTakeActionIfThereAreNoEditors(objectContext, CurrentCompany, CurrentUser, businessLog);

            if (CurrentCompany.canUserTakeRoleIfNoEditors == true)
            {
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Users now CAN take roles to edit company if there are no editors!");
            }
            else
            {
                CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, "Users now CAN NOT take roles to edit company if there are no editors!");
            }

            if (CurrentCompany.canUserTakeRoleIfNoEditors == true)
            {
                lblCanUserTakeRoleIfNoEditors.Text = "Currently users CAN take action to edit this company if there are no editors.";
            }
            else
            {
                lblCanUserTakeRoleIfNoEditors.Text = "Currently users CAN NOT take action to edit this company if there are no editors.";
            }
        }

        protected void btnTakeAction_Click(object sender, EventArgs e)
        {
            BusinessUserTypeActions butActions = new BusinessUserTypeActions();

            List<UsersTypeAction> actions = butActions.GetCompanyModificators(objectContext, CurrentCompany.ID).ToList();
            if (actions.Count < 1 && CurrentCompany.canUserTakeRoleIfNoEditors == true)
            {
                butActions.AddActionForCompanyToUserWhenThereAreNoEditors(userContext, objectContext, CurrentUser, CurrentCompany, businessLog);

                Session["notifMsg"] = string.Format("{0} {1}.", GetLocalResourceObject("YouCanEdit"), CurrentCompany.name);

                RedirectToSameUrl(Request.Url.ToString());
            }
        }

        protected void btnRemoveAlternativeNames_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            PlaceHolder removeAlternativeNameError = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "phRemoveAlternativeNamesError");
            CheckBoxList cblNames = (CheckBoxList)CommonCode.UiTools.GetControlFromAccordionPane(apRemoveAlternativeNames, "cblRemoveAlternativeNames");

            removeAlternativeNameError.Visible = true;
            removeAlternativeNameError.Controls.Clear();
            removeAlternativeNameError.Controls.Add(lblERROR);
            string error = "";

            BusinessAlternativeNames bAN = new BusinessAlternativeNames();
            List<AlternativeCompanyName> namesToDelete = new List<AlternativeCompanyName>();

            if (cblNames.Items.Count < 1)
            {
                CheckAddRemoveAlternativeNames();
                FillCblRemoveAlternativeNames();
                return;
            }
            else
            {
                AlternativeCompanyName currName = null;
                long nameId = 0;
                for (int i = 0; i < cblNames.Items.Count; i++)
                {
                    if (cblNames.Items[i].Selected == true)
                    {
                        if (!long.TryParse(cblNames.Items[i].Value, out nameId))
                        {
                            throw new CommonCode.UIException(string.Format("Couldn`t parse cblNames.Items[{0}] to long, in company id : {1}, user id : {2}"
                                , cblNames.Items[i].Value, CurrentCompany.ID, CurrentUser.ID));
                        }

                        currName = bAN.GetForCompany(objectContext, CurrentCompany, nameId, true, false);
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

                foreach (AlternativeCompanyName nameToDel in namesToDelete)
                {
                    bAN.DeleteAlternativeCompanyName(objectContext, businessLog, CurrentUser, nameToDel);
                }

                ShowCompanyInfo(); ;
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

            lblERROR.Text = error;
        }

        protected void btnAddAlternativeName_Click(object sender, EventArgs e)
        {
            CheckIfUserCanModifyCompanyFromEvents();

            PlaceHolder addAlternativeNameError = (PlaceHolder)CommonCode.UiTools.GetControlFromAccordionPane(apAddAlternativeName, "phAddAlternativeNameError");
            TextBox name = (TextBox)CommonCode.UiTools.GetControlFromAccordionPane(apAddAlternativeName, "tbAlternativeNames");

            addAlternativeNameError.Visible = true;
            addAlternativeNameError.Controls.Clear();
            addAlternativeNameError.Controls.Add(lblERROR);
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
                if (name.Text != CurrentCompany.name)
                {
                    if (bAN.IsThereAlternativeNameForCompany(objectContext, CurrentCompany, strname, true) == false)
                    {
                        if (CommonCode.Validate.ValidateDescription(Configuration.CompaniesMinCompanyNameLength, Configuration.CompaniesMaxCompanyNameLength
                            , name.Text, "name", out error, Configuration.CompaniesMaxCompanyNameLength) == true)
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

            if (bAN.CountAlternativeCompanyNames(objectContext, CurrentCompany, true) < Configuration.CompaniesMaxAlternativeNames)
            {
                if (nameOk == true)
                {
                    addAlternativeNameError.Visible = false;

                    bAN.AddAlternativeNameForCompany(objectContext, CurrentCompany, businessLog, CurrentUser, strname);

                    name.Text = string.Empty;

                    ShowCompanyInfo();
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

            lblERROR.Text = error;
        }

    }
}
