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
    public partial class FAQ : BasePage
    {

        private User currentUser = null;
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
            currentUser = currUser;

            if (currUser != null && businessUser.IsAdminOrGlobalAdmin(currUser))
            {
                AdminLogged = true;
            }

        }

        private void ShowInfo()
        {
            Title = GetLocalResourceObject("title").ToString();

            BusinessSiteText siteText = new BusinessSiteText();
            SiteNews about = siteText.GetSiteText(objectContext, "aboutFAQ"); ;
            if (about == null)
            {
                lblAbout.Text = "About FAQ text not typed.";
            }
            else
            {
                lblAbout.Text = about.description;
            }

            SetLocalText();
            FillData();
        }

        private void SetLocalText()
        {
            lblPageIntro.Text = GetLocalResourceObject("pageIntro").ToString();
        }

        private void FillData()
        {
            BusinessSiteText siteTexts = new BusinessSiteText();
            List<SiteNews> faq = siteTexts.GetFAQ(objectContext, false).ToList(); ;

            FillQuestions(faq);
            FillAnswers(faq);
        }

        private void FillQuestions(List<SiteNews> questions)
        {
            tblQuestions.Rows.Clear();

            if (questions.Count<SiteNews>() > 0)
            {
                foreach (SiteNews question in questions)
                {
                    TableRow newRow = new TableRow();
                    tblQuestions.Rows.Add(newRow);

                    TableCell newCell = new TableCell();
                    newRow.Cells.Add(newCell);

                    Image newImg = new Image();
                    newImg.ImageUrl = "~/images/SiteImages/triangle.png";
                    newImg.CssClass = "itemImage";

                    newCell.Controls.Add(newImg);

                    HyperLink questLink = new HyperLink();
                    newCell.Controls.Add(questLink);
                    questLink.Text = question.name;
                    questLink.NavigateUrl = string.Format("{0}#{1}", GetUrlWithVariant("FAQ.aspx"), question.linkID);
                }
            }
        }

        private void FillAnswers(List<SiteNews> answers)
        {

            System.Web.UI.HtmlControls.HtmlGenericControl mainDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            phAnswers.Controls.Add(mainDiv);

            int count = answers.Count<SiteNews>();

            if (count > 0)
            {

                int i = 0;

                foreach (SiteNews answer in answers)
                {
                    System.Web.UI.HtmlControls.HtmlAnchor placeToGo = new System.Web.UI.HtmlControls.HtmlAnchor();
                    placeToGo.Name = answer.linkID;
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
                    header.Attributes.Add("class", "textHeaderWA");
                    header.Attributes.Add("align", "center");
                    if (AdminLogged)
                    {
                        Label idLbl = new Label();
                        idLbl.Text = string.Format("{0}&nbsp;&nbsp;&nbsp;", answer.ID.ToString());
                        header.Controls.Add(idLbl);
                    }

                    Label questLbl = new Label();
                    questLbl.Text = string.Format("{0}&nbsp;&nbsp;&nbsp;", answer.name);
                    header.Controls.Add(questLbl);

                    HyperLink goToTop = new HyperLink();
                    goToTop.Text = GetLocalResourceObject("toTop").ToString();
                    goToTop.NavigateUrl = GetUrlWithVariant("FAQ.aspx#");
                    header.Controls.Add(goToTop);

                    Label lblDescr = new Label();
                    lblDescr.Text = answer.description;
                    newDiv.Controls.Add(lblDescr);

                    if (i < count)
                    {
                        mainDiv.Controls.Add(CommonCode.UiTools.GetHorisontalFashionLinePanel(false));
                    }
                }
            }
        }


    }
}
