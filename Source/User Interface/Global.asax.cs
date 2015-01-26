﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Specialized;
using System.Web;
using BusinessLayer;
using DataAccess;

using log4net;
using log4net.Config;

namespace UserInterface
{
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// The name of the site generic error page.
        /// </summary>
        private static readonly string GenericErrorPage = "GenericErrorPage.htm";

        private static ILog log = LogManager.GetLogger(typeof(Global));

        protected void Application_Start(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();
            if (log.IsInfoEnabled)
            {
                log.Info("Application started!");
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (Request.Url.AbsolutePath.EndsWith("/SessionRefresh.aspx", StringComparison.OrdinalIgnoreCase) == false)
            {
                string requestedAppVariantStr = HttpContext.Current.Request.Params[CommonCode.UiUrl.QueryStringApplicationVariantKey];
                StringCollection supportedAppVariants = Configuration.SupportedApplicationVariants;
                string appVariantToUse = requestedAppVariantStr;

                if ((string.IsNullOrEmpty(appVariantToUse) == false) &&
                    (supportedAppVariants.Contains(appVariantToUse) == true))
                {
                    // appVariantToUse is Ok
                }
                else
                {
                    //
                    // ATTENTION! Keep this consistent with the logic of BasePage.CheckApplicationVariant(),
                    //            which assumes application variant when no such is specified.
                    //
                    appVariantToUse = Configuration.DefaultApplicationVariant;
                }
                string connStr = Configuration.GetEntitiesConnectionString(appVariantToUse);
                Entities objectContext = new Entities(connStr);
                EntitiesUsers userContext = new EntitiesUsers();

                BusinessStatistics businessStatistic = new BusinessStatistics();
                businessStatistic.SessionStarted(userContext);

                BusinessUser businessUser = new BusinessUser();
                businessUser.AddGuest(CommonCode.CurrentUser.GetCurrentUserId());

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Session started. Client IP address: \"{0}\".", Request.UserHostAddress);
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

            if (Configuration.SiteRunning == false)
            {
                string url = Request.Url.ToString();

                if (url.Contains(".aspx"))
                {
                    Response.Redirect("Stopped.htm");
                }
            }

            // Url rewriting code moved to RewriteUrl.cs

            string path = HttpContext.Current.Request.Url.AbsolutePath;
            bool httpsOn = (HttpContext.Current.Request.ServerVariables["HTTPS"] == "on");

            if (httpsOn == true)
            {
                if (CommonCode.SecurePath.IsSecure(path))
                {
                    //do nothing
                }
                else
                {
                    bool pathIsSubcontent = CommonCode.SecurePath.IsSubcontent(path);
                    bool refererIsSecure = false;

                    if (pathIsSubcontent == true)
                    {
                        Uri refererUri = HttpContext.Current.Request.UrlReferrer;
                        if (refererUri != null)
                        {
                            string refererPath = refererUri.AbsolutePath;
                            if (string.IsNullOrEmpty(refererPath) == false)
                            {
                                refererIsSecure = CommonCode.SecurePath.IsSecure(refererPath);
                            }
                        }
                    }

                    if ((pathIsSubcontent == true) && (refererIsSecure == true))
                    {
                        // Do nothing
                    }
                    else
                    {
                        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri.Replace("https://", "http://"));
                        return;
                    }
                }
            }

            if (httpsOn == false)
            {
                if (CommonCode.SecurePath.IsSecure(path))
                {
                    //Redirect to https version
                    HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri.Replace("http://", "https://"));
                }
            }

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // By some reason (because of the master page or because "/Error.aspx" is a content page?)
            // if the exception occurs in the master page,
            // Application_Error() is called twice for the same exception.

            Exception ex = Server.GetLastError();

            string additionalMsg = string.Empty;
            string errKind = (ex != null) ? "Exception" : "Error";
            try
            {
                string fileRequested = Request.RawUrl;
                string referrer = string.Empty;

                if (Request.UrlReferrer != null)
                {
                    referrer =
                        string.Format(" URL referrer: \"{0}\".", Request.UrlReferrer);
                }
                additionalMsg = string.Format(
                    "{0} occured while serving \"{1}\".{2}", errKind, fileRequested, referrer);
            }
            catch { }

            try
            {
                if (ex != null)
                {
                    HttpException httpEx = ex as HttpException;
                    bool notFound = ((httpEx != null) &&
                        (string.IsNullOrEmpty(httpEx.Message) == false) &&
                        (httpEx.Message.StartsWith("The file ", StringComparison.OrdinalIgnoreCase) == true) &&  // Sorry, no better way
                        (httpEx.Message.EndsWith(" does not exist.", StringComparison.OrdinalIgnoreCase) == true)  // Sorry, no better way
                        );

                    if (notFound == false)
                    {
                        // Keep the log level used in the first log entry consistent with the text of the second log entry.
                        if (log.IsErrorEnabled)
                        {
                            log.Error(additionalMsg, ex);
                        }
                        if ((httpEx != null) &&
                            ((httpEx.Message == "This is an invalid script resource request.")  // Sorry, no better way
                              ||
                            (httpEx.Message == "This is an invalid webresource request."))  // Sorry, no better way
                            )
                        {
                            if (log.IsDebugEnabled)
                            {
                                log.DebugFormat("Addition to a previous {0} entry: client IP address - \"{1}\".", "ERROR", Request.UserHostAddress);
                            }
                        }
                    }
                    else
                    {
                        // Keep the log level used in the first log entry consistent with the text of the second log entry.
                        if (log.IsWarnEnabled)
                        {
                            log.WarnFormat("{0} {1}", additionalMsg, ex.Message ?? string.Empty);
                        }
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Addition to a previous {0} entry: client IP address - \"{1}\".", "WARN", Request.UserHostAddress);
                        }
                    }

                    if ((httpEx != null) &&
                        (string.Equals(httpEx.Message, "Maximum request length exceeded.", StringComparison.OrdinalIgnoreCase) == true)  // Sorry, no better way
                        )
                    {
                        string reqEntityTooLargeErrorPage = "Error.aspx?error=img"; //"EntityTooLarge.htm";
                        Server.ClearError();  // It seems it is impossible to redirect without clearing the error first.

                        RedirectToErrorPage(reqEntityTooLargeErrorPage);
                    }
                    else if (notFound == true)
                    {
                        Server.ClearError();  // It seems it is impossible to redirect without clearing the error first.

                        // Session state is not available in this context.

                        // Double-encode the URL in order to avoid an erroneous detection of application variant specified
                        // in both ways - as a direcory and in the query string.
                        string doubleEncodedUrl = HttpUtility.UrlEncode(HttpUtility.UrlEncode(Request.RawUrl));

                        string errorPage =  // "NotFound.html";
                            string.Format("{0}?error=notfound&url={1}", "Error.aspx", doubleEncodedUrl);
                        RedirectToErrorPage(errorPage);
                    }
                    else
                    {
                        //Server.ClearError();  // Commented out, because it rather causes problems:
                        // The second time 'ex' is null and Response.Redirect(genericErrorPage)
                        // throws a "Cannot redirect after HTTP headers have been sent." exception).

                        string errorPage = "Error.aspx";

                        try
                        {
                            Session["error"] = "exception";
                        }
                        catch (Exception ex2)
                        {
                            if (log.IsWarnEnabled)
                            {
                                log.Warn("Application_Error problem.", ex2);
                            }

                            errorPage = "Error.aspx?error=exc";

                        }

                        if (Request.Url.AbsolutePath.EndsWith("/" + errorPage, StringComparison.OrdinalIgnoreCase) == false)
                        {
                            Server.ClearError();  // It seems it is impossible to redirect without clearing the error first.
                            RedirectToErrorPage(errorPage);
                        }
                        else
                        {
                            RedirectToErrorPage(GenericErrorPage);
                        }
                    }
                }
                else
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error(additionalMsg + " Error information not available.");
                    }

                    RedirectToErrorPage(GenericErrorPage);
                }
            }
            catch (Exception ex1)
            {
                try
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Application_Error failed.", ex1);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Redirects the request to the specified error page.
        /// </summary>
        /// <param name="errorPage">The error page (e.g. "ErrorRage.aspx", "GenericErrorPage.htm", etc.).</param>
        private void RedirectToErrorPage(string errorPage)
        {
            if (string.IsNullOrEmpty(errorPage) == true)
            {
                errorPage = GenericErrorPage;
            }
            string requestedCultureStr =
                HttpContext.Current.Request.Params[CommonCode.UiUrl.QueryStringApplicationVariantKey];
            if (string.IsNullOrEmpty(requestedCultureStr) == false)
            {
                string errorPageWithVariant = ErrorPageForApplicationVariant(errorPage, requestedCultureStr);
                Response.Redirect(errorPageWithVariant);
            }
            else
            {
                if (BusinessLayer.Configuration.UseUrlRewriting == true)
                {
                    string url = Request.Url.AbsolutePath;
                    string errorPageForAppVar = ErrorPageForApplicationVariantFromUrl(errorPage, url);
                    Response.Redirect(errorPageForAppVar);
                }
                else
                {
                    CommonCode.UiTools.RedirectToOtherUrl(errorPage);
                }
            }
        }

        /// <summary>
        /// Returns the URL for the specified error page and URL which processing has failed.
        /// </summary>
        /// <param name="errorPage">The error page (e.g. "ErrorRage.aspx", "GenericErrorPage.htm", etc.).</param>
        /// <param name="url">The URL which processing has failed.</param>
        /// <returns>The requested error page URL.</returns>
        private string ErrorPageForApplicationVariantFromUrl(string errorPage, string url)
        {
            if (string.IsNullOrEmpty(errorPage) == true)
            {
                errorPage = GenericErrorPage;
            }
            if (string.IsNullOrEmpty(url) == true)
            {
                url = Request.Url.AbsolutePath;
            }

            string nonCheckedAppVariant = CommonCode.UiUrl.NonCheckedApplicationVariantFromUrl(url);
            string appVariant = CommonCode.UiUrl.ApplicationVariantFromUrl(url);
            string errorPageForAppVar = ErrorPageForApplicationVariant(errorPage, appVariant);
            if (string.Equals(nonCheckedAppVariant, appVariant) == false)
            {
                // Invalid application variant is specified in the URL.
                // Assemble an URL that does not contain invalid application variant.

                if (Request.UrlReferrer != null)
                {
                    // Look for application variant in the URL referrer
                    string referrerUrl = Request.UrlReferrer.AbsolutePath;
                    nonCheckedAppVariant = CommonCode.UiUrl.NonCheckedApplicationVariantFromUrl(referrerUrl);
                    appVariant = CommonCode.UiUrl.ApplicationVariantFromUrl(referrerUrl);
                    errorPageForAppVar = ErrorPageForApplicationVariant(errorPage, appVariant);
                }
            }
            return errorPageForAppVar;
        }

        /// <summary>
        /// Returns the URL for the specified error page and application variant.
        /// </summary>
        /// <param name="errorPage">The error page (e.g. "ErrorRage.aspx", "GenericErrorPage.htm", etc.).</param>
        /// <param name="appVariantStr">The application variant ("en", "bg", etc.).</param>
        /// <returns>The requested error page URL.</returns>
        private string ErrorPageForApplicationVariant(string errorPage, string appVariantStr)
        {
            if (string.IsNullOrEmpty(errorPage) == true)
            {
                errorPage = GenericErrorPage;
            }
            if (string.IsNullOrEmpty(appVariantStr) == true)
            {
                appVariantStr = Configuration.DefaultApplicationVariant;
            }

            string errorPageWithVariant;
            if (BusinessLayer.Configuration.UseUrlRewriting == true)
            {
                if (Configuration.UseExternalUrlRewriteModule == false)
                {
                    errorPageWithVariant = string.Format("{0}/{1}/{2}", Request.ApplicationPath, appVariantStr, errorPage);
                }
                else
                {
                    errorPageWithVariant = CommonCode.UiTools.UrlWithApplicationPath(errorPage, appVariantStr);
                }
            }
            else
            {
                errorPageWithVariant =
                    string.Format("{0}?{1}={2}",
                    errorPage, CommonCode.UiUrl.QueryStringApplicationVariantKey, appVariantStr);
            }
            return errorPageWithVariant;
        }

        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                BusinessUser businessUser = new BusinessUser();

                // Similar code is present in CommonCode.CurrentUser.GetCurrentUserId().
                // Keep both pieces of code consistent --------------------------------------
                long currentUserId = -1;
                object currentUserIdObj = Session[CommonCode.CurrentUser.CurrentUserIdKey];
                if (currentUserIdObj != null)
                {
                    try
                    {
                        currentUserId = Convert.ToInt64(currentUserIdObj);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            string.Format("The current user ID is not a \"{0}\".",
                            typeof(long).FullName),
                            ex);
                    }
                }
                // --------------------------------------------------------------------------

                if (currentUserId > 0)
                {
                    businessUser.RemoveLoggedUser(currentUserId);
                }
                else
                {
                    businessUser.RemoveGuest(currentUserId);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {
            log.Info("Application end!");
        }


    }
}