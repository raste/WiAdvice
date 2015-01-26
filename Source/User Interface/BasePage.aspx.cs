﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Text;
using System.Web;
using System.Web.Services;

using BusinessLayer;
using DataAccess;

using log4net;

namespace UserInterface
{
    public partial class BasePage : System.Web.UI.Page
    {
        public BasePage()
        {
            this.PreInit += new EventHandler(BasePage_PreInit);
            this.Unload += new EventHandler(BasePage_Unload);
            this.PreRenderComplete += new EventHandler(BasePage_PreRenderComplete);
            this.Load += new EventHandler(BasePage_Load);
            this.PreRender += new EventHandler(BasePage_PreRender);
        }

        private Boolean needsToBeLogged = false;    // Used for pages that can be seen only if user is logged
        private Boolean redirrectOnLogIn = false;   // Used for pages that should redirrect on LogIn
        private DateTime creationStart = DateTime.UtcNow;  // The date/time when the page creation started
        private static ILog log = LogManager.GetLogger(typeof(BasePage));

        protected void SetNeedsToBeLogged()
        {
            needsToBeLogged = true;
        }

        protected void SetRedirrectOnLogIn()
        {
            redirrectOnLogIn = true;
        }

        public Boolean RedirrectOnLogIn
        {
            get { return redirrectOnLogIn; }
        }

        public Boolean NeedToBeLogged
        {
            get { return needsToBeLogged; }
        }

        // The name of the generic error page, i.e. "GenericErrorPage.htm".
        private static readonly string GenErrPage = "GenericErrorPage.htm";

        /// <summary>
        /// A key in the Session which indicates that the client was redirected to the same page (with un-rewritten URL)
        /// because the client specified a rewritten URL.
        /// </summary>
        protected static readonly string RedirectedFromRewrittenURLSessKey = "RedirectedFromRewrittenURL";

        /// <summary>
        /// A key in the Session which indicates that an attempt to retrieve the client time zone offset is already made.
        /// </summary>
        public static readonly string ClientTimeZoneOffsetSessKey = "ClientTimeZoneOffset";

        /// <summary>
        /// A key in the Session which identifies the URL of the page that was requested when no attempt to retrieve the client time zone offset was made yet.
        /// After an attempt to retrieve the client time zone offset is made, the client will be redirected to the requested URL.
        /// </summary>
        protected static readonly string ClientTimeZoneOffsetReturnUrlSessKey = "ClientTimeZoneOffsetReturnUrl";

        /// <summary>
        /// Gets or sets the number of redirects caused by client specifying a rewritten URL.
        /// </summary>
        private int RedirectsFromRewrittenURL
        {
            get
            {
                string sessKey = RedirectedFromRewrittenURLSessKey;
                int redirectsFromRewrittenURL = GetSessValueInt(sessKey);
                return redirectsFromRewrittenURL;
            }
            set
            {
                Session[RedirectedFromRewrittenURLSessKey] = value.ToString();
            }
        }

        private void BasePage_PreInit(object sender, EventArgs e)
        {
            creationStart = DateTime.UtcNow;
            if (Request.Browser.Browser.Contains("Safari"))
            {
                this.ClientTarget = "uplevel";
            }

            // In case the client specified a rewritten URL, redirect to the respective un-rewritten URL
            if (BusinessLayer.Configuration.UseUrlRewriting == true)
            {
                int redirsFromRewrittenURL = RedirectsFromRewrittenURL;
                string reqUrlStr = Request.Url.ToString();

                if (reqUrlStr.EndsWith(Request.RawUrl) == true)
                {
                    string requestedCultureStr = HttpContext.Current.Request.Params[CommonCode.UiUrl.QueryStringApplicationVariantKey];

                    if (string.IsNullOrEmpty(requestedCultureStr) == false)
                    {
                        // To avoid endless redirects in case of error
                        if (redirsFromRewrittenURL > 5)
                        {
                            if (log.IsErrorEnabled)
                            {
                                string errMsg =
                                    string.Format("Too many redirects caused by client specifying a rewritten URL: {0}. Redirecting to \"{1}\".",
                                    redirsFromRewrittenURL, GenErrPage);
                                log.Error(errMsg);
                            }
                            Response.Redirect(GenErrPage);
                        }
                        RedirectsFromRewrittenURL = redirsFromRewrittenURL + 1;

                        if (log.IsWarnEnabled)
                        {
                            StringBuilder formatStrBuilder = new StringBuilder();
                            formatStrBuilder.Append("Applicarion variant in the query string of the URL specified by the client: \"{0}\". ");
                            formatStrBuilder.Append("Try to place the application variant specifier as a folder and ");
                            formatStrBuilder.Append("to redirect to that URL.");
                            string formatString = formatStrBuilder.ToString();
                            log.WarnFormat(formatString, Request.RawUrl);
                        }
                        RedirectToSameUrl(Request.RawUrl);
                    }
                    else
                    {
                        // Will be redirected by CheckApplicationVariant().
                    }
                }
                if (redirsFromRewrittenURL > 0)
                {
                    RedirectsFromRewrittenURL = redirsFromRewrittenURL - 1;
                }
            }
        }

        void BasePage_Unload(object sender, EventArgs e)
        {
            DateTime creationEnd = DateTime.UtcNow;
            TimeSpan creationTime = creationEnd - creationStart;
            double creationSeconds = creationTime.TotalSeconds;

            if (log.IsInfoEnabled)
            {
                bool skipLogEntry = false;
                string pageTitle = string.Empty;
                try
                {
                    pageTitle = this.Title;
                }
                catch
                {
                    pageTitle = string.Empty;
                }
                if (string.IsNullOrEmpty(pageTitle))
                {
                    Type sessionRefreshType = typeof(SessionRefresh);
                    Type thisTypeBase = GetType().BaseType;
                    if ((thisTypeBase != null) && (thisTypeBase == sessionRefreshType))
                    {
                        skipLogEntry = true;
                        pageTitle = string.Empty;
                    }
                    else
                    {
                        pageTitle = GetType().FullName;
                    }
                }
                if (skipLogEntry == false)
                {
                    if (log.IsInfoEnabled)
                    {
                        string msg = string.Format("Page \"{0}\" created in {1} s.", pageTitle, creationSeconds);
                        log.Debug(msg);
                    }
                }
            }

            Type pageClass = GetType();
            if (pageClass.BaseType != typeof(SessionRefresh))
            {
                object objPageLoaded = Session["PageLoaded"];
                if (objPageLoaded != null)
                {
                    Session["PageLoaded"] = DateTime.UtcNow; 
                }
                else
                {
                    Session.Add("PageLoaded", DateTime.UtcNow); 
                }

            }
        }

        void BasePage_PreRenderComplete(object sender, EventArgs e)
        {
            MasterPage master = this.Master as MasterPage;
            if (master != null)
            {
                DateTime creationEnd = DateTime.UtcNow;
                TimeSpan creationTime = creationEnd - creationStart;
                double creationSeconds = creationTime.TotalSeconds;

                master.ShowCreationTimeSeconds(creationSeconds);
            }
        }

        void BasePage_Load(object sender, EventArgs e)
        {
            if (BusinessLayer.Configuration.UseUrlRewriting == true)
            {
                string url = Request.Url.ToString();
                CommonCode.UiUrl.VerifyNoExtraApplicationVariantSpecifiersInUrl(url);
                CheckApplicationVariant();
            }

            MasterPage master = this.Master as MasterPage;
            if (master != null)
            {
                master.ClearCreationTimeSeconds();
            }

            if (Request.Url.AbsolutePath.EndsWith("/SessionRefresh.aspx", StringComparison.OrdinalIgnoreCase) == false)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("{0} requested. Client IP address: \"{1}\".", Request.RawUrl, Request.UserHostAddress);
                }
            }
        }


        void BasePage_PreRender(object sender, EventArgs e)
        {
            if (Configuration.UseUrlRewriting == true)
            {
                // Uncomment the following line if the pages do not post back to themselves.
                RegisterUnRewriteFormActionScript();
                CheckTimeZoneSettings();
            }
        }

        protected bool IsRequestValid(EntitiesUsers userContext, IpAttemptTry attemptTry, out string error)
        {
            Tools.AssertObjectContextExists(userContext);

            error = string.Empty;

            if (IsPostBack == false)
            {
                return false;
            }

            bool result = true;

            object objIsBot = Session["IsBot"];
            if (objIsBot != null && objIsBot.ToString() == "true")
            {
                error = GetGlobalResourceObject("SiteResources", "InvalidRequestIsBot").ToString();
                result = false;
            }

            if (result == true && CommonCode.IpAttempts.MinTimeAfterPageLoadPassed() == false)
            {
                error = GetGlobalResourceObject("SiteResources", "InvalidRequestTooSoon").ToString();
                result = false;
            }

            int minutesLeftToWait = 0;
            if (result == true && CommonCode.IpAttempts.MaxTriesReached(userContext, attemptTry, out minutesLeftToWait) == true)
            {
                error = string.Format("{0} {1} {2}", GetGlobalResourceObject("SiteResources", "InvalidRequestMaxTriesReached"),
                    minutesLeftToWait, GetGlobalResourceObject("SiteResources", "InvalidRequestMaxTriesReached2"));
                result = false;
            }

            return result;
        }

        private void RegisterUnRewriteFormActionScript()
        {
            if (BusinessLayer.Configuration.UseUrlRewriting == true)
            {
                if (this.Form != null)
                {
                    string formActionUnrewriteKey = "FormActionUnrewrite";

                    if (ClientScript.IsStartupScriptRegistered(formActionUnrewriteKey) == false)
                    {
                        string formClientId = this.Form.ClientID;
                        if (string.IsNullOrEmpty(formClientId) == false)
                        {
                            string urlToUnrewrite = HttpContext.Current.Request.RawUrl;
                            string newActionUrl = CommonCode.UiUrl.UrlUnRewrite(urlToUnrewrite);
                            CommonCode.UiUrl.VerifyNoExtraApplicationVariantSpecifiersInUrl(newActionUrl);
                            if (newActionUrl != null)
                            {
                                string unrewriteFormActionFuncText = @"
function UnrewriteFormAction(formId, newAction) {
    var result = false;
    if (formId) {
        if (newAction) {
            var frm = document.getElementById(formId);
            if (frm) {
                frm.setAttribute(""action"", newAction);

                var actionAttr = frm.getAttribute(""action"");
                if (newAction == actionAttr) {
                    result = true;
                }
                else {
                    alert(""Could not set form action. Window will be closed."");
                    window.close();
                }
            }
            else {
                alert(""No form."");
            }
        }
        else {
            alert(""No action"");
        }
    }
    else {
        alert(""formId is missing."");
    }
    return result;
}
";
                                string newLine = @"
";
                                string scriptText =
                                    string.Format("{0}UnrewriteFormAction(\"{1}\", \"{2}\");{3}",
                                    unrewriteFormActionFuncText, formClientId, newActionUrl, newLine);
                                Type pageType = this.GetType();
                                ClientScript.RegisterStartupScript(pageType, formActionUnrewriteKey, scriptText, true);
                                if (log.IsDebugEnabled == true)
                                {
                                    if (string.Equals(urlToUnrewrite, newActionUrl) == false)
                                    {
                                        log.DebugFormat("Script to unrewrite the form action from \"{0}\" to \"{1}\" registered.",
                                            urlToUnrewrite, newActionUrl);
                                    }
                                    else
                                    {
                                        log.DebugFormat("Script to unrewrite the form action from to \"{0}\" registered.",
                                            newActionUrl);
                                    }
                                }

                                // It is possible thar after calling Ajaxm, the URL contains the application variant specified in both ways -
                                // as a directory and in the query string.
                                // Fix the postback URL also just before submitting the form.
                                string onsubmitFunc = string.Format("UnrewriteFormAction(\"{0}\", \"{1}\"); ", formClientId, newActionUrl);
                                this.Form.Attributes.Add("onsubmit", onsubmitFunc);
                            }
                        }
                    }
                }
            }
        }

        protected override void InitializeCulture()
        {
            base.InitializeCulture();

            CommonCode.UiTools.ChangeUiCulture();
            ApplicationVariantString = Tools.ApplicationVariantString;
            Session[CommonCode.UiUrl.QueryStringApplicationVariantKey] = ApplicationVariantString;
        }

        /// <summary>
        /// Identifies the application variant that is derived from the current thread UI culture.
        /// <para>For example: "en", "bg", etc.</para>
        /// </summary>
        private string applicationVariantString = Configuration.DefaultApplicationVariant;

        /// <summary>
        /// Gets a string value identifying the application variant that is derived from the current thread UI culture.
        /// <para>For example: "en", "bg", etc.</para>
        /// </summary>
        protected string ApplicationVariantString
        {
            get { return applicationVariantString ?? string.Empty; }
            set { applicationVariantString = value; }
        }



        /// <summary>
        /// Creates an <see cref="Entities"/> object depending on the culture (language) that is specified in the Request.
        /// </summary>
        /// <returns>The requested <c>Entities</c> instance. Does not return <c>null</c>.</returns>
        protected Entities CreateEntities()
        {
            string cultureStr = Request.Params[CommonCode.UiUrl.QueryStringApplicationVariantKey];
            string connStr = Configuration.GetEntitiesConnectionString(cultureStr);
            Entities applicationVariantEntities = new Entities(connStr);
            return applicationVariantEntities;
        }

        protected void RedirectToSameUrl(string url)
        {
            CommonCode.UiTools.RedirectToSameUrl(url);
        }

        /// <summary>
        /// Includes the application variant as a prefix to the specified URL and redirects the client browser to the new URL.
        /// </summary>
        /// <param name="url">The URL that does not contain application variant.</param>
        protected void RedirectToOtherUrl(string url)
        {
            CommonCode.UiTools.RedirectToOtherUrl(url);
        }

        /// <summary>
        /// A key in the Session which indicates that the client was redirected to the home page because
        /// there was no application variant specified.
        /// </summary>
        protected static readonly string RedirectedToHomeNoAppVariantSessKey = "RedirectedToHomeNoApplicationVariant";

        /// <summary>
        /// Checks whether the application variant that is (or is not) specified in the request corresponds to
        /// the current application variant. If not, the client is redirected to Home.aspx,
        /// with default application variant specified.
        /// </summary>
        private void CheckApplicationVariant()
        {
            if (BusinessLayer.Configuration.UseUrlRewriting == true)
            {
                string requestedCultureStr = HttpContext.Current.Request.Params[CommonCode.UiUrl.QueryStringApplicationVariantKey];
                int sessAppVarRedirects = SessionAppVariantRedirects;

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Request application path: \"{0}\".", Request.ApplicationPath);
                }
                if (string.Equals(ApplicationVariantString, requestedCultureStr) == false)
                {
                    if (string.IsNullOrEmpty(requestedCultureStr) == true)
                    {
                        if (log.IsDebugEnabled)
                        {
                            string infoMsg = "No application variant specified in the request.";
                            log.Debug(infoMsg);
                        }
                    }
                    else
                    {
                        if (log.IsErrorEnabled)
                        {
                            string errMsg = string.Format("Requested {0}: \"{1}\", actual {0}: \"{2}\".",
                                "application variant", requestedCultureStr, ApplicationVariantString);
                            log.Error(errMsg);
                        }
                    }

                    string dfltAppVariant = Configuration.DefaultApplicationVariant;
                    if (log.IsDebugEnabled)
                    {
                        string infoMsg = string.Format("Redirecting using the \"{0}\" {1}.", dfltAppVariant, "application variant");
                        log.Debug(infoMsg);
                    }

                    // To avoid endless redirects in case of error
                    if (sessAppVarRedirects > 2)
                    {
                        if (log.IsErrorEnabled)
                        {
                            string errMsg1 = string.Format("Too many application variant redirects: {0}. Redirecting to \"{1}\".",
                                sessAppVarRedirects, GenErrPage);
                            log.Error(errMsg1);
                        }
                        Response.Redirect(GenErrPage);
                    }

                    SessionAppVariantRedirects = sessAppVarRedirects + 1;

                    string errorPage = "Error.aspx";
                    bool errorPageRequested = Request.Url.AbsolutePath.EndsWith("/" + errorPage, StringComparison.OrdinalIgnoreCase);

                    //
                    // ATTENTION! Keep this consistent with the logic of Global.Session_Start(),
                    //            which connects to a database when no application variant is specified.
                    //
                    string urlWithVariant;
                    if (Configuration.UseExternalUrlRewriteModule == false)
                    {
                        urlWithVariant = string.Format("{0}/{1}", dfltAppVariant, errorPageRequested ? errorPage : "Home.aspx");
                    }
                    else
                    {
                        if (errorPageRequested == false)
                        {
                            errorPage = "Home.aspx";
                        }

                        urlWithVariant = CommonCode.UiTools.UrlWithApplicationPath(errorPage, dfltAppVariant);
                    }


                    Session[RedirectedToHomeNoAppVariantSessKey] = true;

                    // TODO: Consider how and where in the code an invalig application version specified in the request
                    //       can be removed from it.

                    Response.Redirect(urlWithVariant);
                }

                if (sessAppVarRedirects > 0)
                {
                    SessionAppVariantRedirects = sessAppVarRedirects - 1;
                }
            }
        }

        /// <summary>
        /// Key for accessing the number of redirects caused by application variant inconsistencies, which is stored in the session.
        /// </summary>
        private readonly string SessionAppVariantRedirectsKey = "ApplicationVariantRedirects";

        /// <summary>
        /// Gets or sets the number of redirects caused by application variant inconsistencies.
        /// </summary>
        private int SessionAppVariantRedirects
        {
            get
            {
                string sessKey = SessionAppVariantRedirectsKey;
                int appVariantRedirects = GetSessValueInt(sessKey);
                return appVariantRedirects;
            }
            set
            {
                Session[SessionAppVariantRedirectsKey] = value.ToString();
            }
        }

        /// <summary>
        ///  Gets an <see cref="int"/> value stored in the Session by its key.
        /// </summary>
        /// <param name="sessKey">The key in the Session which identifies the value.</param>
        /// <returns>The requested value.</returns>
        protected int GetSessValueInt(string sessKey)
        {
            if (sessKey == null)
            {
                throw new ArgumentNullException("sessKey");
            }
            if (sessKey == string.Empty)
            {
                throw new ArgumentException("sessKey" + " is empty.");
            }

            int sessValueInt = 0;
            object sessValueObj = Session[sessKey];
            if (sessValueObj != null)
            {
                string sessValueStr = sessValueObj.ToString();
                if (int.TryParse(sessValueStr, out sessValueInt) == false)
                {
                    if (sessValueStr != string.Empty)
                    {
                        string errMsg =
                            string.Format("Invalid {0}: {1}.", sessKey, sessValueStr);
                        throw new CommonCode.UIException(errMsg);
                    }
                }
            }
            return sessValueInt;
        }

        [WebMethod]
        public static string[] GetCompletionList(string prefixText, int count)
        {
            return CommonCode.WebMethods.GetAutoCompleteList(prefixText, count);
        }

        [WebMethod]
        public static string SetClientTimeZoneOffset(string offset)
        {
            return CommonCode.WebMethods.SetOffSetTimeZoneInSession(offset);
        }

        private void CheckTimeZoneSettings()
        {
            if (Request.Url.AbsolutePath.EndsWith("/SessionRefresh.aspx", StringComparison.OrdinalIgnoreCase) == false)
            {
                if (Session["ClientTimeZoneOffset"] == null)
                {
                    if (this.Form != null)
                    {
                        string timeZoneOffsetAutoSubmitKey = "TimeZoneOffsetAutoSubmit";

                        if (ClientScript.IsStartupScriptRegistered(timeZoneOffsetAutoSubmitKey) == false)
                        {
                            string formClientId = this.Form.ClientID;
                            string submitFormFuncText = @"
function SetClientTimeZone(formId) {

    if (formId) {
            var frm = document.getElementById(formId);
            if (frm) {

                    var dt = new Date();
                    var tzOffs = dt.getTimezoneOffset();

                    PageMethods.SetClientTimeZoneOffset(tzOffs, succSetClientTimeZone, timeOut, error)

            }
            else {
                alert(""No form."");
            }
    }
    else {
        alert(""formId is missing."");
    }
}
function succSetClientTimeZone(str) {
if(str = 'success'){
//window.location = document.location.href;
}
};";
                            string newLine = @"
";
                            string scriptText =
                                string.Format("{0}SetClientTimeZone(\"{1}\");{2}",
                                submitFormFuncText, formClientId, newLine);
                            Type pageType = this.GetType();
                            ClientScript.RegisterStartupScript(pageType, timeZoneOffsetAutoSubmitKey, scriptText, true);
                            if (log.IsDebugEnabled == true)
                            {
                                log.Debug("Script to auto-submit the client time zone offset registered.");
                            }
                        }
                    }

                }
            }
        }

        protected string GetUrlWithVariant(string url)
        {
            return CommonCode.UiTools.GetUrlWithVariant(url);
        }

        protected User GetCurrentUser(EntitiesUsers userContext, Entities objectContext)
        {
            return CommonCode.UiTools.GetCurrentUser(userContext, objectContext, true);
        }

        /// <summary>
        /// Determines whether a button with the specified ID is clicked.
        /// </summary>
        /// <param name="buttonID">The button ID.</param>
        /// <returns>If the button is present in the form and was clicked - <c>true</c>; otherwise - <c>false</c>.</returns>
        protected bool IsButtonClicked(string buttonID)
        {
            bool buttonClicked = false;
            string keyEnding = "$" + buttonID;
            string formKeyName = null;
            for (int i = 0; (formKeyName == null) && (i < Request.Form.AllKeys.Length); i++)
            {
                string formKey = Request.Form.AllKeys[i];
                if (formKey != null)
                {
                    if ((formKey == buttonID) || (formKey.EndsWith(keyEnding) == true))
                    {
                        formKeyName = formKey;
                    }
                }
            }
            if (string.IsNullOrEmpty(formKeyName) == false)
            {
                // It is probably not necessary to check the value of 
                // Request.Form[formKeyName]
                // Moreover, it can vary depending on the application variant - for example "Search" or "Търси".

                buttonClicked = true;
            }
            return buttonClicked;
        }


        public void ShowTransBtnIfBg(CustomServerControls.WbkdTransButton control)
        {
            if (control == null)
            {
                throw new CommonCode.UIException("control is null");
            }

            if (Tools.ApplicationVariantString == "bg")
            {
                control.Visible = true;
                control.CssClass = "webkbd-switcher webkbd-main roundedCorners5 ";

                string browserType = Request.Browser.Type.ToUpper();

                if (browserType == "IE5" || browserType.Contains("SAFARI")) // Chrome
                {
                    control.CssClass += "webkbd-Chrome";
                }
                else if (browserType.Contains("IE"))  // IE
                {
                    control.CssClass += "webkbd-IE";
                }
                else if (browserType.Contains("FIREFOX")) // Mozzilla
                {
                    control.CssClass += "webkbd-Firefox";  // other
                }
                else
                {
                    control.CssClass += "webkbd-Firefox";
                }

            }
            else
            {
                control.Visible = false;
            }

        }

    }
}
