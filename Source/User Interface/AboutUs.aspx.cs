﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class AboutUs : BasePage
    {
        private Entities objectContext = null;

        protected void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ShowInfo();
        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            BusinessSiteText siteText = new BusinessSiteText();

            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutExtended");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblAbout.Text = aboutExtended.description;
            }
            else
            {
                lblAbout.Text = "No information entered.";
            }

            CommonCode.UiTools.HideUserNotificationPnl(pnlNotification, lblNotification, Page);

            SetLocalText();
            FillDdlSection();
        }

        private void SetLocalText()
        {

            lblPageIntro.Text = GetLocalResourceObject("pageIntro").ToString();

            lblContact.Text = GetLocalResourceObject("contact").ToString();
            lblSubject.Text = GetLocalResourceObject("subject").ToString();
            lblName.Text = GetLocalResourceObject("name").ToString();
            lblSection.Text = GetLocalResourceObject("section").ToString();
            lblDescr.Text = GetLocalResourceObject("description").ToString();
            lblEmail.Text = GetLocalResourceObject("email").ToString();
            btnSubmit.Text = GetLocalResourceObject("submit").ToString();

            tbCaptcha_TextBoxWatermarkExtender.WatermarkText = GetLocalResourceObject("captchaLetters").ToString();

            lblSiteTeam.Text = GetLocalResourceObject("Team").ToString();
            lblTeamMember1.Text = GetLocalResourceObject("TeamMember1").ToString();
            lblTeamMember2.Text = GetLocalResourceObject("TeamMember2").ToString();
            lblTeamMember3.Text = GetLocalResourceObject("TeamMember3").ToString();
        }

        private void FillDdlSection()
        {
            if (IsPostBack == false)
            {
                ddlSection.Items.Clear();

                ListItem generalItem = new ListItem();
                ddlSection.Items.Add(generalItem);
                generalItem.Text = GetLocalResourceObject("sectionGeneral").ToString();
                generalItem.Value = "0";

                ListItem advertisementItem = new ListItem();
                ddlSection.Items.Add(advertisementItem);
                advertisementItem.Text = GetLocalResourceObject("sectionAdvertisement").ToString();
                advertisementItem.Value = "1";

                ListItem technicalItem = new ListItem();
                ddlSection.Items.Add(technicalItem);
                technicalItem.Text = GetLocalResourceObject("sectionTechnical").ToString();
                technicalItem.Value = "2";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblError.Visible = true;
            string error = "";

#if DEBUG
            lblError.Text = "Mail sending in debug mode is forbidden.";
            return;
#endif

            NoBotState state;
            if (!NoBotAbout.IsValid(out state))
            {
                error = CommonCode.UiTools.GetNoBotInvalidMessage(state);
                lblError.Text = error;
                return;
            }

            ccContact.ValidateCaptcha(tbCaptcha.Text);
            tbCaptcha.Text = "";
            if (!ccContact.UserValidated)
            {
                error = GetGlobalResourceObject("SiteResources", "errorIncLetters").ToString();
                lblError.Text = error;
                return;
            }

            string toEmail = string.Empty;
            switch (ddlSection.SelectedIndex)
            {
                case 0:
                    toEmail = Configuration.SiteGeneralSectionMail;
                    break;
                case 1:
                    toEmail = Configuration.SiteAdvertisementsSectionMail;
                    break;
                case 2:
                    toEmail = Configuration.SiteSupportSectionMail;
                    break;
                default:
                    throw new CommonCode.UIException(string.Format("ddlSection.SelectedIndex = {0} is not valid index", ddlSection.SelectedIndex));
            }

            string description = tbDescription.Text;
            string subject = tbSubject.Text;
            string name = tbName.Text;

            if (CommonCode.Validate.ValidateDescription(0, 100, ref subject, "subject", out error, Configuration.FieldsDefMaxWordLength))
            {
                if (CommonCode.Validate.ValidateDescription(1, 50, ref name, "name", out error, Configuration.FieldsDefMaxWordLength))
                {
                    if (CommonCode.Validate.ValidateEmailAdress(tbEmail.Text, out error))
                    {
                        if (CommonCode.Validate.ValidateDescription(1, 50000, ref description, "description", out error, 200))
                        {
                            lblError.Visible = false;

                            CommonCode.UiTools.ShowUserNotificationPnl(pnlNotification, lblNotification
                                , GetLocalResourceObject("emailSent").ToString());

                            SmtpMailSend mailSend = new SmtpMailSend();
                            mailSend.SendMessageToSite(name, subject, tbEmail.Text, description, toEmail);

                            tbName.Text = string.Empty;
                            tbDescription.Text = string.Empty;
                            tbSubject.Text = string.Empty;
                            tbEmail.Text = string.Empty;
                        }
                        else
                        {
                            error = string.Format("{0} {1} - {2} {3}", GetGlobalResourceObject("SiteResources", "errorDescr1")
                                , 1, 5000, GetGlobalResourceObject("SiteResources", "errorDescr2"));
                        }
                    }
                    else
                    {
                        error = GetGlobalResourceObject("SiteResources", "errorEmail").ToString();
                    }
                }
                else
                {
                    error = GetGlobalResourceObject("SiteResources", "errorName").ToString();
                }
            }
            else
            {
                error = GetGlobalResourceObject("SiteResources", "errorSubject").ToString();
            }

            lblError.Text = error;
        }

    }
}
