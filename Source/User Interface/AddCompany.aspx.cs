﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.Services;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class AddCompany : BasePage
    {
        private User currentUser = null;

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            tbName.Attributes.Add("onblur", string.Format("JSCheckData('{0}','companynameAdd','{1}',''); GetSimilarNames('{2}', '{3}', '{4}', '{5}');"
                , tbName.ClientID, lblCcompName.ClientID
                , pnlSimilarNames.ClientID, "company", tbName.ClientID, pnlPopUp.ClientID));

            tbDescription.Attributes.Add("onKeyUp", string.Format("ShowCharsCountInField('{0}', '{1}', '{2}', '{3}');"
                , tbDescription.ClientID, tbSymbolsCount.ClientID, Configuration.CompaniesMinDescriptionLength, Configuration.CompaniesMaxDescriptionLength));
            tbDescription.Attributes.Add("onBlur", string.Format("ShowCharsCountInField('{0}', '{1}', '{2}', '{3}');"
                , tbDescription.ClientID, tbSymbolsCount.ClientID, Configuration.CompaniesMinDescriptionLength, Configuration.CompaniesMaxDescriptionLength));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetNeedsToBeLogged();
            checkUser();
            if (currentUser == null)
            {
                throw new CommonCode.UIException("currentUser is null");
            }
            ShowInfo();
            
        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            lblCcompName.Text = "";

            BusinessSiteText siteText = new BusinessSiteText();

            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutAddCompany");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblAbout.Text = aboutExtended.description; 
            }
            else
            {
                lblAbout.Text = string.Format("About Add {0} text not typed.", Configuration.CompanyName);
            }

            lblSiteRules.Text = string.Format("{0}", GetLocalResourceObject("siteRules"));

            SetLocalText();

            if (Configuration.CompaniesMinDescriptionLength < 1)
            {
                rfvDescription.Enabled = false;
            }
        }

        private void SetLocalText()
        {
            lblPageIntro.Text = GetLocalResourceObject("pageIntro").ToString();

            lblNameRules.Text = string.Format("{0} {1}-{2} {3}.", GetLocalResourceObject("nameRules")
                , Configuration.CompaniesMinCompanyNameLength, Configuration.CompaniesMaxCompanyNameLength
                , GetLocalResourceObject("characters"));

            lblDescriptionRules.Text = string.Format("{0} {1}-{2} {3}.{4}", GetLocalResourceObject("descriptionRules"),
                Configuration.CompaniesMinDescriptionLength, Configuration.CompaniesMaxDescriptionLength
                , GetLocalResourceObject("characters"), GetLocalResourceObject("descriptionRules2"));

            lblSymbolsCount.Text = GetLocalResourceObject("textLength").ToString();
            tbSymbolsCount.Text = tbDescription.Text.Length.ToString();

            lblCompanyData.Text = GetLocalResourceObject("companyData").ToString();
            lblCompName.Text = GetLocalResourceObject("name").ToString();
            lblSite.Text = GetLocalResourceObject("website").ToString();
            btnCompSubmit.Text = GetLocalResourceObject("submit").ToString();
            lblDescription.Text = GetLocalResourceObject("description").ToString();
        }

        private void FillDdlCompanyTypes()
        {
            ddlType.Items.Clear();

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
                ddlType.Items.Add(newItem);
            }
        }

        private void checkUser()
        {
            BusinessUser bUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);

            if (currUser != null)
            {
                if (bUser.CanUserDo(userContext, currUser, UserRoles.AddCompanies))
                {
                    pnlAddCompanyForm.Visible = true;
                    currentUser = currUser;
                }
                else
                {
                    CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errCantAdd").ToString());
                }
            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, GetLocalResourceObject("errLogIn").ToString());
            }
        }

        protected void btnCompSubmit_Click(object sender, EventArgs e)
        {
            lblError.Visible = true;
            String error = "";

            BusinessUser businessUser = new BusinessUser();

            if (businessUser.CanUserDo(userContext, currentUser, UserRoles.AddCompanies))
            {
                BusinessCompany businessCompany = new BusinessCompany();
                int timeToWait = 0;

                if (businessCompany.CheckIfMinimumTimeBetweenAddingCompaniesPassed(objectContext, currentUser, out timeToWait) == true)
                {
                    string name = tbName.Text;

                    if (CommonCode.Validate.ValidateName(objectContext, "companies", ref name, Configuration.CompaniesMinCompanyNameLength
                        , Configuration.CompaniesMaxCompanyNameLength, out error, 0))
                    {

                        string description = tbDescription.Text;

                        if (CommonCode.Validate.ValidateDescription(Configuration.CompaniesMinDescriptionLength,
                            Configuration.CompaniesMaxDescriptionLength, ref description, "description", out error, 110))
                        {
                            if (CommonCode.Validate.ValidateSiteAdress(tbSite.Text, out error, true))
                            {
                                
                                lblError.Visible = false;

                                BusinessCategory businessCategory = new BusinessCategory();
                                BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();

                                //CompanyType selectedType = GetSelectedType();
                                BusinessCompanyType businessCompanyType = new BusinessCompanyType();
                                CompanyType type = businessCompanyType.GetCompanyType(objectContext, userContext);

                                businessCompany.AddCompany(userContext, objectContext, businessLog
                                    , currentUser, name, description, tbSite.Text, type);

                                Company currCompany = businessCompany.GetCompanyByName(objectContext, name);
                                if (currCompany == null)
                                {
                                    throw new BusinessException(string.Format("Couldn`t find company with name : '{0}' right after its creation, User id : {1}"
                                        , tbName.Text, currentUser.ID));
                                }

                                pnlAddSucc.Visible = true;
                                lblAddSucc.Text = GetLocalResourceObject("addSucc").ToString();

                                lblGoToCompsPage.Text = GetLocalResourceObject("goTo").ToString();
                                lblGoToCompsPage2.Text = GetLocalResourceObject("goTo2").ToString();

                                hlCompPage.Text = currCompany.name;
                                hlCompPage.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", currCompany.ID));

                                FillBulletedLists();

                                pnlAddCompanyForm.Visible = false;
                                
                            }
                        }
                    }
                }
                else
                {
                    error = string.Format("{0} {1} {2}", GetLocalResourceObject("errMinTimeAfterAddingCompanyDidntPass")
                       , timeToWait, GetLocalResourceObject("errMinTimeAfterAddingCompanyDidntPass2"));
                }

                lblError.Text = error;
            }
            else
            {
                throw new CommonCode.UIException(string.Format("User id = {0} cannot add company",currentUser.ID));
            }
        }

        private void FillBulletedLists()
        {
            blToDo1.Items.Clear();
            blToDo1.Items.Add(GetListItem(GetLocalResourceObject("blAddCategories").ToString()));

            blToDo2.Items.Clear();
            blToDo2.Items.Add(GetListItem(GetLocalResourceObject("blAddProduct").ToString()));
            blToDo2.Items.Add(GetListItem(GetLocalResourceObject("blModifDescription").ToString()));
            blToDo2.Items.Add(GetListItem(GetLocalResourceObject("blAddAltNames").ToString()));
            blToDo2.Items.Add(GetListItem(GetLocalResourceObject("blAddCharacteristics").ToString()));
            blToDo2.Items.Add(GetListItem(GetLocalResourceObject("blFillGallery").ToString()));
        }

        private ListItem GetListItem(string text)
        {
            ListItem newItem = new ListItem();
            newItem.Text = text;
            return newItem;
        }

        [WebMethod]
        public static string CheckData(string text, string type, string notUsed)
        {
            string error = "";

            CommonCode.WebMethods.ValidateUserInput(text, type, "", out error);

            return error;

        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTypeInfo();
        }

        private void UpdateTypeInfo()
        {
            CompanyType selectedType = GetSelectedType();
            if (!string.IsNullOrEmpty(selectedType.description))
            {
                lblTypeInfo.Text = Tools.GetFormattedTextFromDB(selectedType.description);
            }
            else
            {
                lblTypeInfo.Text = "";
            }
        }

        private CompanyType GetSelectedType()
        {
            BusinessCompanyType businessCompanyType = new BusinessCompanyType();

            string strTypeId = ddlType.SelectedValue;
            if (string.IsNullOrEmpty(strTypeId))
            {
                throw new CommonCode.UIException(string.Format("ddlType.SelectedValue is empty or null, user id = {0}"
                    , currentUser.ID));
            }
            long typeId = -1;
            if (!long.TryParse(strTypeId, out typeId))
            {
                throw new CommonCode.UIException(string.Format("couldnt parse ddlType.SelectedValue to long, user id = {0}"
                    , currentUser.ID));
            }

            CompanyType selectedType = businessCompanyType.Get(objectContext, typeId, true);
            if (selectedType == null)
            {
                throw new CommonCode.UIException(string.Format("There`s no type ID = {0} , or it is visible=false, user id = {1}"
                    , typeId, currentUser.ID));
            }

            return selectedType;
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

    }
}
