﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.UI.WebControls;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class LastCP : BasePage
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

        protected void Page_Load(object sender, EventArgs e)
        {
            SetNeedsToBeLogged();
            CheckUser();

            ShowInfo();
        }

        private void ShowInfo()
        {
            Title = "Last added products and companies";
        }

        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);
            if (currUser != null)
            {
                if (businessUser.IsFromAdminTeam(currUser))
                {
                    currentUser = currUser;
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

            if (currentUser == null)
            {
                throw new CommonCode.UIException("CurrentUser is null");
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            lblError.Visible = true;

            string strNumber = tbNumber.Text;
            int number = 0;

            if (string.IsNullOrEmpty(strNumber))
            {
                lblError.Text = "Type number.";
                return;
            }
            else if (!int.TryParse(strNumber, out number))
            {
                lblError.Text = "Incorrect number format.";
                return;
            }

            if (number < 1 || number > 1000 )
            {
                lblError.Text = "Number must be between 1 and 1000.";
                return;
            }

            string selected = rblType.SelectedValue;

            if (string.IsNullOrEmpty(selected))
            {
                lblError.Text = "Choose companies or products.";
                return;
            }

            lblError.Visible = false;

            
            switch (selected)
            {
                case ("companies"):
                    FillCompanies(number);
                    break;
                case("products"):
                    FillProducts(number);
                    break;
                case ("topics"):
                    FillTopics(number);
                    break;
                default:
                    throw new CommonCode.UIException(string.Format("Selected value isnt supported ({0})", selected));
            }

        }

        private void FillCompanies(int number)
        {
            phCompanies.Controls.Clear();

            BusinessCompany businessCompany = new BusinessCompany();
            
            List<Company> companies = businessCompany.GetLastAddedCompanies(objectContext, (long)number);

            if (companies.Count > 0)
            {
                Company other = businessCompany.GetOther(objectContext);

                int i = 0;

                foreach (Company company in companies)
                {
                    Panel newPanel = new Panel();
                    phCompanies.Controls.Add(newPanel);
                    newPanel.CssClass = "panelRows";

                    if (i % 2 == 0)
                    {
                        newPanel.BackColor = CommonCode.UiTools.GetStandardGreenCellBgrColor();
                    }

                    Panel namePnl = new Panel();
                    newPanel.Controls.Add(namePnl);
                    namePnl.CssClass = "panelInline";
                    namePnl.Width = Unit.Pixel(400);

                    if (company != other)
                    {
                        HyperLink compLink = new HyperLink();
                        namePnl.Controls.Add(compLink);
                        compLink.NavigateUrl = GetUrlWithVariant(string.Format("Company.aspx?Company={0}", company.ID));
                        compLink.Text = company.name;
                        compLink.Attributes.Add("onmouseover", string.Format("ShowData('company','{0}','{1}','{2}')", company.ID, compLink.ClientID, pnlPopUp.ClientID));
                        compLink.Attributes.Add("onmouseout", "HideData()");
                    }
                    else
                    {
                        namePnl.Controls.Add(CommonCode.UiTools.GetLabelWithText(company.name, false));
                    }

                    Label byUser = new Label();
                    newPanel.Controls.Add(byUser);
                    byUser.CssClass = "marginLeft";
                    byUser.Text = "By : ";

                    if (!company.CreatedByReference.IsLoaded)
                    {
                        company.CreatedByReference.Load();
                    }

                    newPanel.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                        (Tools.GetUserFromUserDatabase(userContext, company.CreatedBy)));

                    Label dateLbl = new Label();
                    newPanel.Controls.Add(dateLbl);
                    dateLbl.CssClass = "marginLeft commentsDate";
                    dateLbl.Text = CommonCode.UiTools.DateTimeToLocalString(company.dateCreated);

                    Label visibleLbl = new Label();
                    newPanel.Controls.Add(visibleLbl);
                    visibleLbl.CssClass = "marginLeft searchPageRatings";
                    visibleLbl.Text = string.Format("visible : {0}", company.visible);

                    i++;
                }
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "No found results";
            }
        }

        private void FillProducts(int number)
        {
            phProducts.Controls.Clear();

            BusinessProduct businessProducts = new BusinessProduct();

            List<Product> products = businessProducts.GetLastProducts(objectContext, (long)number);

            if (products.Count > 0)
            {
                int i = 0;

                foreach (Product product in products)
                {
                    Panel newPanel = new Panel();
                    phProducts.Controls.Add(newPanel);
                    newPanel.CssClass = "panelRows";

                    if (i % 2 == 0)
                    {
                        newPanel.BackColor = CommonCode.UiTools.GetStandardCellBgrColor();
                    }

                    Panel namePnl = new Panel();
                    newPanel.Controls.Add(namePnl);
                    namePnl.CssClass = "panelInline";
                    namePnl.Width = Unit.Pixel(400);

                    HyperLink prodLink = new HyperLink();
                    namePnl.Controls.Add(prodLink);
                    prodLink.NavigateUrl = GetUrlWithVariant(string.Format("Product.aspx?Product={0}", product.ID));
                    prodLink.Text = product.name;
                    prodLink.Attributes.Add("onmouseover", string.Format("ShowData('product','{0}','{1}','{2}')", product.ID, prodLink.ClientID, pnlPopUp.ClientID));
                    prodLink.Attributes.Add("onmouseout", "HideData()");
                   

                    Label byUser = new Label();
                    newPanel.Controls.Add(byUser);
                    byUser.CssClass = "marginLeft";
                    byUser.Text = "By : ";

                    if (!product.CreatedByReference.IsLoaded)
                    {
                        product.CreatedByReference.Load();
                    }

                    newPanel.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                        (Tools.GetUserFromUserDatabase(userContext, product.CreatedBy)));

                    Label dateLbl = new Label();
                    newPanel.Controls.Add(dateLbl);
                    dateLbl.CssClass = "marginLeft commentsDate";
                    dateLbl.Text = CommonCode.UiTools.DateTimeToLocalString(product.dateCreated);

                    Label visibleLbl = new Label();
                    newPanel.Controls.Add(visibleLbl);
                    visibleLbl.CssClass = "marginLeft searchPageRatings";
                    visibleLbl.Text = string.Format("visible : {0}", product.visible);

                    i++;
                }
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "No found results";
            }
        }

        private void FillTopics(int number)
        {
            phTopics.Controls.Clear();

            BusinessProductTopics bpTopics = new BusinessProductTopics();

            List<ProductTopic> topics = bpTopics.GetLastTopics(objectContext, (long)number);

            if (topics.Count > 0)
            {
                int i = 0;

                foreach (ProductTopic topic in topics)
                {
                    Panel newPanel = new Panel();
                    phTopics.Controls.Add(newPanel);
                    newPanel.CssClass = "panelRows";

                    if (i % 2 == 0)
                    {
                        newPanel.BackColor = CommonCode.UiTools.GetStandardCellBgrColor();
                    }

                    Panel namePnl = new Panel();
                    newPanel.Controls.Add(namePnl);
                    namePnl.CssClass = "panelInline";
                    namePnl.Width = Unit.Pixel(450);

                    HyperLink topicLink = new HyperLink();
                    namePnl.Controls.Add(topicLink);
                    topicLink.NavigateUrl = GetUrlWithVariant(string.Format("Topic.aspx?id={0}", topic.ID));
                    topicLink.Text = topic.name;

                    Label byUser = new Label();
                    newPanel.Controls.Add(byUser);
                    byUser.CssClass = "marginLeft";
                    byUser.Text = "By : ";

                    if (!topic.UserReference.IsLoaded)
                    {
                        topic.UserReference.Load();
                    }

                    newPanel.Controls.Add(CommonCode.UiTools.GetUserHyperLink
                        (Tools.GetUserFromUserDatabase(userContext, topic.User)));

                    Label dateLbl = new Label();
                    newPanel.Controls.Add(dateLbl);
                    dateLbl.CssClass = "marginLeft commentsDate";
                    dateLbl.Text = CommonCode.UiTools.DateTimeToLocalString(topic.dateCreated);

                    Label visibleLbl = new Label();
                    newPanel.Controls.Add(visibleLbl);
                    visibleLbl.CssClass = "marginLeft searchPageRatings";
                    visibleLbl.Text = string.Format("visible : {0}", topic.visible);

                    if (topic.locked == true)
                    {
                        Label lockedLbl = new Label();
                        newPanel.Controls.Add(lockedLbl);
                        lockedLbl.CssClass = "marginLeft searchPageRatings";
                        lockedLbl.Text = "locked";
                    }

                    i++;
                }
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "No found results";
            }
             
        }

        [WebMethod]
        public static string WMGetData(string type, string Id)
        {
            return CommonCode.WebMethods.GetTypeData(type, Id);
        }


    }
}
