﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.IO;
using System.Web;

using BusinessLayer;
using log4net;

namespace UserInterface.CommonCode
{
    /// <summary>
    /// Determines the theme to be used by the User Interface.
    /// </summary>
    public class UITheme
    {
        private static readonly string defaultTheme = "MainTheme";
        private static ILog log = LogManager.GetLogger(typeof(UITheme));

        /// <summary>
        /// The name of the cookie which contains information about the chosen theme.
        /// </summary>
        public static readonly string ThemeCookieName = "ClientOptions";

        /// <summary>
        /// Gets the name of the theme that is to be used by the whole site.
        /// </summary>
        public static string SiteTheme
        {
            get
            {
                string themeToUse = GetThemeNameFromKookie();

                if (string.IsNullOrEmpty(themeToUse) == true)
                {
                    themeToUse = Configuration.DefaultUITheme;

                    if (string.IsNullOrEmpty(themeToUse) == true)
                    {
                        themeToUse = defaultTheme;
                    }
                }
                return themeToUse;
            }
        }

        /// <summary>
        /// Gets the names of the available themes of the site.
        /// <para>If there are no available themes, returns an array with no (i.e. zero) elements.</para>
        /// </summary>
        public static string[] AvailableThemes
        {
            get
            {
                string[] themesAvailable = null;
                string themesFolder = Path.Combine(PathMap.PhysicalApplicationPath, "App_Themes");

                if ((string.IsNullOrEmpty(themesFolder) == false) && (Directory.Exists(themesFolder) == true))
                {
                    themesAvailable = Directory.GetDirectories(themesFolder, "*", SearchOption.TopDirectoryOnly);
                }
                if (themesAvailable == null)
                {
                    themesAvailable = new string[0];
                }
                for (int i = 0; i < themesAvailable.Length; i++)
                {
                    string themeName = string.Empty;
                    string themePath = themesAvailable[i];
                    if (string.IsNullOrEmpty(themePath) == false)
                    {
                        themeName = themePath.Replace(themesFolder, string.Empty);
                        themeName = themeName.Replace(Path.DirectorySeparatorChar.ToString(), string.Empty);
                    }
                    themesAvailable[i] = themeName ?? string.Empty;
                }
                return themesAvailable;
            }
        }

        /// <summary>
        /// Gets the name of the theme to use from an <see cref="HttpRequest"/>.
        /// </summary>
        /// <param name="req">The HttpRequest to get the theme name from.</param>
        /// <param name="paramName">The name of the control which contains information which theme to use.</param>
        /// <returns>The name of the theme to use or an empty string if no information found in the request.</returns>
        public static string GetThemeNameFromRequest(HttpRequest req, string controlName)
        {
            string themeName = null;

            if ((req != null) && (string.IsNullOrEmpty(controlName) == false))
            {
                string[] reqParamsKeys = req.Params.AllKeys;
                if (reqParamsKeys != null)
                {
                    int keyIndex = -1;
                    bool multipleKeys = false;

                    for (int i = 0; (multipleKeys == false) && (i < reqParamsKeys.Length); i++)
                    {
                        string reqParamKey = reqParamsKeys[i];

                        if ((string.IsNullOrEmpty(reqParamKey) == false) && (reqParamKey.EndsWith(controlName) == true))
                        {
                            if (keyIndex == -1)
                            {
                                keyIndex = i;
                            }
                            else
                            {
                                multipleKeys = true;
                            }
                        }
                    }
                    if ((keyIndex != -1) && (multipleKeys == false))
                    {

                        string ddlThemeNamesSelIndStr = req.Params[keyIndex];
                        int ddlThemeNamesSelInd;

                        if (int.TryParse(ddlThemeNamesSelIndStr, out ddlThemeNamesSelInd) == true)
                        {
                            string[] themeNames = CommonCode.UITheme.AvailableThemes;
                            if ((0 <= ddlThemeNamesSelInd) && (themeNames != null) && (ddlThemeNamesSelInd < themeNames.Length))
                            {
                                themeName = themeNames[ddlThemeNamesSelInd];
                            }
                        }
                    }
                    if (multipleKeys == true)
                    {
                        if (log.IsErrorEnabled == true)
                        {
                            string errMsg = string.Format(
                                "More than one request keys that end with \"{0}\" found. Cannot get the name of the theme.",
                                controlName);
                            log.Error(errMsg);
                        }
                    }
                }
            }
            return themeName ?? string.Empty;
        }

        /// <summary>
        /// Gets the name of the chosen theme from the respective request cookie.
        /// </summary>
        /// <returns>The name theme name ftom the cookie of an empty string if no cookie found.</returns>
        private static string GetThemeNameFromKookie()
        {
            HttpContext currentContext = HttpContext.Current;
            if (currentContext == null)
            {
                throw new InvalidOperationException("The current HttpContext is not available.");
            }

            string themeName = string.Empty;
            HttpCookieCollection cookies = currentContext.Request.Cookies;
            string[] cookieNames = cookies.AllKeys;
            bool cookieFound = false;

            for (int i = 0; (cookieFound == false) && (i < cookieNames.Length); i++)
            {
                if ((ThemeCookieName != null) && (ThemeCookieName == cookieNames[i]))
                {
                    HttpCookie themeCookie = cookies[ThemeCookieName];
                    themeName = themeCookie.Value ?? string.Empty;
                    cookieFound = true;
                }
            }


            return themeName;
        }

        /// <param name="boolMaxWidth">True if max width should be 100%, otherwise false</param>
        public static bool SaveSettings(System.Web.SessionState.HttpSessionState Session, HttpResponse Response, HttpRequest Request
            , int minWidth, int maxWidth, bool boolMaxWidth, out string error)
        {
            if (Session == null)
            {
                throw new UIException("Session is null");
            }
            if (Response == null)
            {
                throw new UIException("Response is null");
            }
            if (Request == null)
            {
                throw new UIException("Request is null");
            }

            bool correctSettings = true;
            error = string.Empty;
            System.Text.StringBuilder sbError = new System.Text.StringBuilder();

            UiTools.ChangeUiCultureFromSession();

            if (minWidth < 970)
            {
                correctSettings = false;
                sbError.Append(string.Format("{0}<br />", HttpContext.GetGlobalResourceObject("UiTheme", "errMinWidthCantBeLessThan")));
            }
            else if (minWidth > 1500)
            {
                correctSettings = false;
                sbError.Append(string.Format("{0}<br />", HttpContext.GetGlobalResourceObject("UiTheme", "errMinWidthCantBeMoreThan")));
            }

            if (boolMaxWidth == false)
            {
                if (minWidth > maxWidth)
                {
                    correctSettings = false;
                    sbError.Append(string.Format("{0}<br />", HttpContext.GetGlobalResourceObject("UiTheme", "errMinWidthCantBeMoreThanMax")));
                }
                else if (maxWidth > 2000)
                {
                    correctSettings = false;
                    sbError.Append(string.Format("{0}<br />", HttpContext.GetGlobalResourceObject("UiTheme", "errMaxWidthCantBeMoreThan")));
                }
            }

            if (correctSettings)
            {
                // Cookie/Session format : minWidth,maxWidth ,[new stuff]

                System.Text.StringBuilder strCookieSettings = new System.Text.StringBuilder();
                strCookieSettings.Append(minWidth);
                if (boolMaxWidth == true)
                {
                    strCookieSettings.Append(",true");
                }
                else
                {
                    strCookieSettings.Append(",");
                    strCookieSettings.Append(maxWidth);
                }

                HttpContext currentContext = HttpContext.Current;
                HttpCookieCollection cookies = currentContext.Request.Cookies;
                string[] cookieNames = cookies.AllKeys;
                bool cookieExist = false;
                int timesFound = 0;
                for (int i = 0; i < cookieNames.Length; i++)
                {
                    if ((ThemeCookieName != null) && (ThemeCookieName == cookieNames[i]))
                    {
                        cookieExist = true;
                        timesFound++;
                    }
                }

                HttpCookie clientSettingsCookie = new HttpCookie(ThemeCookieName, strCookieSettings.ToString());
                clientSettingsCookie.Expires = DateTime.Today.AddMonths(12);  // TODO: Update cookie expiration mechanism is necessary.

                if (cookieExist)
                {
                    if (timesFound > 1)
                    {
                        for (int i = 0; i < timesFound; i++)
                        {
                            HttpCookie myCookie = new HttpCookie(ThemeCookieName);
                            myCookie.Expires = DateTime.Now.AddDays(-1d);
                            Response.Cookies.Add(myCookie);
                        }

                        Response.Cookies.Add(clientSettingsCookie);
                    }
                    else
                    {
                        Response.Cookies.Set(clientSettingsCookie);
                    }
                }
                else
                {
                    Response.Cookies.Add(clientSettingsCookie);
                }

                Session["utMinWidth"] = minWidth;
                if (boolMaxWidth == false)
                {
                    Session["utMaxWidth"] = maxWidth;
                    Session["utBoolMaxWidth"] = null;
                }
                else
                {
                    Session["utBoolMaxWidth"] = true;
                    Session["utMaxWidth"] = null;
                }

            }
            error = sbError.ToString();
            return correctSettings;
        }

        public static void ResetSettings(System.Web.SessionState.HttpSessionState Session, HttpResponse Response, HttpRequest Request)
        {
            HttpContext currentContext = HttpContext.Current;
            HttpCookieCollection cookies = currentContext.Request.Cookies;
            string[] cookieNames = cookies.AllKeys;
            bool cookieExist = false;
            int timesFound = 0;
            for (int i = 0; i < cookieNames.Length; i++)
            {
                if ((ThemeCookieName != null) && (ThemeCookieName == cookieNames[i]))
                {
                    cookieExist = true;
                    timesFound++;
                }
            }

            if (cookieExist)
            {
                for (int i = 0; i < timesFound; i++)
                {
                    HttpCookie myCookie = new HttpCookie(ThemeCookieName);
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(myCookie);
                }
            }

            Session["utMaxWidth"] = null;
            Session["utMinWidth"] = null;
            Session["utBoolMaxWidth"] = null;
        }

        public static bool GetSettings(System.Web.SessionState.HttpSessionState Session
           , out int minWidth, out int maxWidth, out bool boolMaxWidth)
        {
            if (Session == null)
            {
                throw new UIException("Session is null");
            }

            minWidth = 0;
            maxWidth = 0;
            boolMaxWidth = false;

            bool gotSettings = true;

            object objUtCookie = Session["utCheckedForCookie"];
            if (objUtCookie == null)
            {
                HttpContext currentContext = HttpContext.Current;
                if (currentContext == null)
                {
                    throw new InvalidOperationException("The current HttpContext is not available.");
                }

                string cookieValues = string.Empty;
                HttpCookieCollection cookies = currentContext.Request.Cookies;
                string[] cookieNames = cookies.AllKeys;
                bool cookieFound = false;

                for (int i = 0; (cookieFound == false) && (i < cookieNames.Length); i++)
                {
                    if ((ThemeCookieName != null) && (ThemeCookieName == cookieNames[i]))
                    {
                        HttpCookie themeCookie = cookies[ThemeCookieName];
                        cookieValues = themeCookie.Value ?? string.Empty;
                        cookieFound = true;
                    }
                }

                Session["utCheckedForCookie"] = true.ToString();

                if (!string.IsNullOrEmpty(cookieValues))
                {
                    string[] delimiters = new string[] { "," };
                    string[] textWords = cookieValues.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    if (textWords.Length >= 2)  // minWidth, maxWidth(bool or int)
                    {
                        if (int.TryParse(textWords[0], out minWidth))
                        {
                            Session["utMinWidth"] = minWidth;
                        }
                        else
                        {
                            gotSettings = false;
                        }

                        if (int.TryParse(textWords[1], out maxWidth))
                        {
                            Session["utMaxWidth"] = maxWidth;
                        }
                        else
                        {
                            if (bool.TryParse(textWords[1], out boolMaxWidth))
                            {
                                Session["utBoolMaxWidth"] = boolMaxWidth.ToString();
                            }
                            else
                            {
                                gotSettings = false;
                            }
                        }
                    }
                    else
                    {
                        gotSettings = false;
                    }
                }
                else
                {
                    gotSettings = false;
                }
            }
            else
            {
                object objUtMaxWidth = Session["utMaxWidth"];
                object objUtMinWidth = Session["utMinWidth"];
                object objUtBoolMaxWidth = Session["utBoolMaxWidth"];

                if (objUtMinWidth != null && (objUtMaxWidth != null || objUtBoolMaxWidth != null))
                {
                    if (!int.TryParse(objUtMinWidth.ToString(), out minWidth))
                    {
                        gotSettings = false;
                    }

                    if (objUtMaxWidth == null || !int.TryParse(objUtMaxWidth.ToString(), out maxWidth))
                    {
                        if (!bool.TryParse(objUtBoolMaxWidth.ToString(), out boolMaxWidth))
                        {
                            gotSettings = false;
                        }
                    }
                }
                else
                {
                    gotSettings = false;
                }
            }

            return gotSettings;
        }


        public static bool IsCookiesEnabled(HttpRequest Request, HttpResponse Response)
        {
            string currentUrl = Request.RawUrl;

            if (Request.QueryString["cookieCheck"] == null)
            {
                try
                {
                    HttpCookie c = new HttpCookie("SupportCookies", "true");
                    Response.Cookies.Add(c);

                    if (currentUrl.IndexOf("?") > 0)
                        currentUrl = currentUrl + "&cookieCheck=true";
                    else
                        currentUrl = currentUrl + "?cookieCheck=true";

                    Response.Redirect(currentUrl);
                }
                catch
                {
                }
            }

            bool result = true;

            if (!Request.Browser.Cookies || Request.Cookies["SupportCookies"] == null)
            {
                result = false;
            }

            return result;
        }

    }
}
