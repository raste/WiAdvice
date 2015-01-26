﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Web;

namespace UserInterface
{
    public partial class Error : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetRedirrectOnLogIn();
            Title = GetLocalResourceObject("title").ToString();
            CheckError();
        }

        private void CheckError()
        {
            object error = Session["error"];
            string errorParam = Request["error"];
            if (error != null || !string.IsNullOrEmpty(errorParam))
            {
                if (error != null)
                {
                    if (error.ToString() == "exception")
                    {
                        pnlOccTime.Visible = true;
                        lblUtcTime.Text = string.Format("{0} : {1}", GetLocalResourceObject("time"),
                            DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture));

                        lblError.Text = GetLocalResourceObject("exception").ToString();
                    }
                    else
                    {
                        pnlOccTime.Visible = false;
                        lblError.Text = error.ToString();
                    }
                }
                else if (!string.IsNullOrEmpty(errorParam))
                {
                    if (errorParam == "img")
                    {
                        pnlOccTime.Visible = false;
                        lblError.Text = GetLocalResourceObject("errorImageSize").ToString();
                    }
                    else if (errorParam == "exc")
                    {
                        pnlOccTime.Visible = true;
                        lblUtcTime.Text = string.Format("{0} : {1}", GetLocalResourceObject("time"),
                            DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture));

                        lblError.Text = GetLocalResourceObject("exception").ToString();
                    }
                    else if (errorParam == "notfound")
                    {
                        string notFoundUrl = Request.QueryString["url"] ?? string.Empty;
                        string notFoundUrlDoubleDecoded = HttpUtility.UrlDecode(HttpUtility.UrlDecode(notFoundUrl));
                        string errorPagePartialUrl = "/Error.aspx?error=notfound&url=";
                        if (notFoundUrlDoubleDecoded.Contains(errorPagePartialUrl) == true)
                        {
                            // Temporary workaround (should not stay this way because the URL in the address bar of the web browser
                            // indicates that we did not specify the URL of Error.aspx correctly).
                            notFoundUrlDoubleDecoded = HttpUtility.UrlDecode(notFoundUrlDoubleDecoded);
                            int errorPagePartialUrlInd = notFoundUrlDoubleDecoded.IndexOf(errorPagePartialUrl);
                            if (errorPagePartialUrlInd != -1)
                            {
                                notFoundUrlDoubleDecoded =
                                    notFoundUrlDoubleDecoded.Substring(errorPagePartialUrlInd + errorPagePartialUrl.Length);
                            }
                        }
                        pnlOccTime.Visible = false;

                        if (string.IsNullOrEmpty(notFoundUrl))
                        {
                            RedirectToOtherUrl("Home.aspx");
                        }

                        lblError.Text = string.Format("{0}<br/>{1}", GetLocalResourceObject("err404"), notFoundUrlDoubleDecoded);
                    }
                    else
                    {
                        RedirectToOtherUrl("Home.aspx");
                    }
                }
                else
                {
                    RedirectToOtherUrl("Home.aspx");
                }

                Session["error"] = null;
                error = null;
            }
            else
            {
                if (IsPostBack == false)
                {
                    RedirectToOtherUrl("Home.aspx");
                }
            }
        }

    }
}
