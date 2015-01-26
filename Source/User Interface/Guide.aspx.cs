﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class Guide : BasePage
    {
        private Boolean AdminLogged = false;

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
            CheckUser();
            ShowInfo();
        }

        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);

            if (currUser != null && businessUser.IsAdminOrGlobalAdmin(currUser))
            {
                AdminLogged = true;
            }
        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            BusinessSiteText siteText = new BusinessSiteText();
            SiteNews about = siteText.GetSiteText(objectContext, "aboutGuide"); ;
            if (about == null)
            {
                lblAbout.Text = "About guide information text not typed.";
            }
            else
            {
                lblAbout.Text = about.description;
            }

            FillData();
            SetLocalText();
        }

        private void SetLocalText()
        {
            lblPageIntro.Text = GetLocalResourceObject("pageIntro").ToString();


        }

        private void FillData()
        {
            BusinessSiteText siteTexts = new BusinessSiteText();
            List<SiteNews> information = siteTexts.GetInformation(objectContext, false).ToList();

            FillTblInformation(information);
            FillTblInformationDescr(information);
        }


        private void FillTblInformation(List<SiteNews> rules)
        {
            tblInformation.Rows.Clear();

            if (rules.Count<SiteNews>() > 0)
            {
                foreach (SiteNews rule in rules)
                {
                    Image newImg = new Image();
                    newImg.ImageUrl = "~/images/SiteImages/triangle.png";
                    newImg.CssClass = "itemImage";

                    TableRow newRow = new TableRow();
                    tblInformation.Rows.Add(newRow);

                    TableCell newCell = new TableCell();
                    newRow.Cells.Add(newCell);

                    newCell.Controls.Add(newImg);

                    HyperLink ruleLink = new HyperLink();
                    newCell.Controls.Add(ruleLink);
                    ruleLink.Text = rule.name;
                    ruleLink.NavigateUrl = string.Format("{0}#{1}", GetUrlWithVariant("Guide.aspx"), rule.linkID);

                }
            }
        }

        private void FillTblInformationDescr(List<SiteNews> rules)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl mainDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            phInformation.Controls.Add(mainDiv);

            int count = rules.Count<SiteNews>();

            if (count > 0)
            {

                int i = 0;

                foreach (SiteNews rule in rules)
                {

                    System.Web.UI.HtmlControls.HtmlAnchor placeToGo = new System.Web.UI.HtmlControls.HtmlAnchor();
                    placeToGo.Name = rule.linkID;
                    mainDiv.Controls.Add(placeToGo);

                    System.Web.UI.HtmlControls.HtmlGenericControl newDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    mainDiv.Controls.Add(newDiv);

                    i++;

                    if (i % 2 == 1)
                    {
                        newDiv.Attributes.Add("class", "infoTextDiv1");
                    }
                    else
                    {
                        newDiv.Attributes.Add("class", "infoTextDiv2");
                    }

                    System.Web.UI.HtmlControls.HtmlGenericControl header = new System.Web.UI.HtmlControls.HtmlGenericControl("p");
                    newDiv.Controls.Add(header);
                    header.Attributes.Add("align", "center");
                    header.Attributes.Add("class", "textHeaderWA");

                    if (AdminLogged)
                    {
                        Label idLbl = new Label();
                        idLbl.Text = string.Format("{0}&nbsp;&nbsp;&nbsp;", rule.ID.ToString());
                        header.Controls.Add(idLbl);
                    }

                    Label ruleLbl = new Label();
                    ruleLbl.Text = string.Format(("{0}&nbsp;&nbsp;&nbsp;"), rule.name);
                    header.Controls.Add(ruleLbl);

                    HyperLink goToTop = new HyperLink();
                    goToTop.Text = GetLocalResourceObject("ToTop").ToString();
                    goToTop.NavigateUrl = GetUrlWithVariant("Guide.aspx#");
                    header.Controls.Add(goToTop);

                    Label descrLbl = new Label();
                    descrLbl.Text = rule.description;
                    newDiv.Controls.Add(descrLbl);

                    if (i < count)
                    {
                        mainDiv.Controls.Add(CommonCode.UiTools.GetHorisontalFashionLinePanel(false));
                    }
                }
            }
            else
            {

                System.Web.UI.HtmlControls.HtmlGenericControl description = new System.Web.UI.HtmlControls.HtmlGenericControl("p");
                mainDiv.Controls.Add(description);

                Label descrLbl = new Label();
                descrLbl.Text = GetLocalResourceObject("noEnteredGuides").ToString();
                description.Controls.Add(descrLbl);
            }

        }


    }
}
