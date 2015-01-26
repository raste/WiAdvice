﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

using BusinessLayer;

namespace UserInterface.CommonCode
{
    /// <summary>
    /// Provides means for URL rewriting.
    /// </summary>
    public class UiUrl
    {
        /// <summary>
        /// The number of '/' symbols before the first '.' synbol in the URL that corresponds to an URL that needs to be rewritten.
        /// <para>I.e. "http://server/xx/page.aspx" needs to be rewritten, but "http://server/page.aspx" needs not to be rewritten.</para>
        /// </summary>
        private static readonly int minExpectedSlashesBeforeDot = Configuration.UrlRewritingDirectoryLevel + 2;

        /// <summary>
        /// How many are the '/' symbols before the culture specification.
        /// <para>I.e. 1 for "http://site/en/page.aspx", 2 for "http://site/dir/en/page.aspx", 3 for "http://site/dir1/dir2/en/page.aspx" and so on.</para>
        /// </summary>
        private static readonly int slashesBeforeCultureSpecification = Configuration.UrlRewritingDirectoryLevel + 1;

        /// <summary>
        /// The length of the culture specification in an URL that needs to be rewritten.
        /// <para>Examples of culture specification: "en/", "bg/".</para>
        /// </summary>
        private static readonly int expectedCultureSpecificationLength = 3;

        /// <summary>
        /// The URL path separator as a string (i.e. "/").
        /// </summary>
        private static readonly string slash = "/";

        /// <summary>
        /// The URL query string params separator as a string (i.e. "&").
        /// </summary>
        public static readonly string amp = "&";

        /// <summary>
        /// The symbol which separates the query string from the rest of the URL, as a string (i.e. "?").
        /// </summary>
        public static readonly string quest = "?";

        /// <summary>
        /// The symbol which separates a query string key from the respective query string value, as a string (i.e. "=").
        /// </summary>
        private static readonly string eq = "=";

        /// <summary>
        /// The name of the key in the query string which value specifies the requested gulture.
        /// </summary>
        public static readonly string QueryStringApplicationVariantKey = "lang";  // Must NOT be null or an empty string

        /// <summary>
        /// Types of the pages for which URL rewriting is used.
        /// <para>These file types support query string.</para>
        /// </summary>
        private static string[] SupportedFileTypesWithQueryString = { ".aspx", ".ashx" };

        /// <summary>
        /// Types of the pages for which URL rewriting is used.
        /// <para>These file types do not support query string.</para>
        /// </summary>
        private static string[] SupportedFileTypesWithoutQueryString = { ".asmx", ".js", ".css", ".jpg", ".png", ".bmp", ".gif", ".htm", ".ico", ".swf" };  // No ".html"!

        /// <summary>
        /// Returns an URL to be used by the application dependent on the supplied user-requested URL.
        /// <para>For example, for "http://server/en/page.aspx" the result is "http://server/page.aspx?lang=en".</para>
        /// </summary>
        /// <param name="userRequestedUrl">The URL that is requested by the client. No query string should be included.</param>
        /// <returns>The requested URL to be used by the application.s</returns>
        public static string UrlRewrite(string userRequestedUrl)
        {
            if (userRequestedUrl == null)
            {
                throw new ArgumentNullException("userRequestedUrl");
            }
            if (userRequestedUrl == string.Empty)
            {
                throw new ArgumentException("userRequestedUrl" + " is empty.");
            }
            if (HttpContext.Current == null)
            {
                throw new UIException("HttpContext.Current is null.");
            }
            if (HttpContext.Current.Request == null)
            {
                throw new UIException("HttpContext.Current.Request is null.");
            }
            if ((userRequestedUrl.Contains("?") == true) || (userRequestedUrl.Contains("&") == true))
            {
                throw new ArgumentException("The query string must be excluded from the URL.", "userRequestedUrl");
            }

            HttpRequest req = HttpContext.Current.Request;
            string serverSideUrl = userRequestedUrl;
            bool fileTypeSupportsQueryString = false;
            bool rewritten = false;

            // Determine whether we have "user-friendly" URL like http://server/xx/page.aspx, where "xx/" identifies a 
            // culture (language), e.g. "en/", "bg/", etc.
            int dotIndex = -1;

            for (int i = 0; (dotIndex == -1) && (i < SupportedFileTypesWithQueryString.Length); i++)
            {
                string currentFileTypeQS = SupportedFileTypesWithQueryString[i];
                if (string.IsNullOrEmpty(currentFileTypeQS) == false)
                {
                    int matchIndexQS = serverSideUrl.IndexOf(currentFileTypeQS, StringComparison.OrdinalIgnoreCase);
                    if (matchIndexQS != -1)
                    {
                        if (
                            (serverSideUrl.EndsWith(currentFileTypeQS, StringComparison.OrdinalIgnoreCase) == true)  // No path info
                            ||
                            (serverSideUrl[matchIndexQS + currentFileTypeQS.Length] == '/')  // With path info
                            )
                        {
                            dotIndex = matchIndexQS;
                            fileTypeSupportsQueryString = true;
                        }
                    }
                }
            }
            if (dotIndex == -1)
            {
                // Still -1
                for (int j = 0; (dotIndex == -1) && (j < SupportedFileTypesWithoutQueryString.Length); j++)
                {
                    string currentFileTypeNoQS = SupportedFileTypesWithoutQueryString[j];
                    if (string.IsNullOrEmpty(currentFileTypeNoQS) == false)
                    {
                        int matchIndexNoQS = serverSideUrl.IndexOf(currentFileTypeNoQS, StringComparison.OrdinalIgnoreCase);
                        if (matchIndexNoQS != -1)
                        {
                            if (
                                (serverSideUrl.EndsWith(currentFileTypeNoQS, StringComparison.OrdinalIgnoreCase) == true)  // No path info
                                ||
                                (serverSideUrl[matchIndexNoQS + currentFileTypeNoQS.Length] == '/')  // With path info
                                )
                            {
                                dotIndex = matchIndexNoQS;
                            }
                        }
                    }
                }
            }

            int cultureSpecificationStartIndex = -1;
            int previousSlashIndex = -1;
            int slashesBeforeDot = 0;

            if ((0 < dotIndex) && (dotIndex < serverSideUrl.Length))
            {
                int currentSlashIndex = -1;

                // Determine the number of '/' symbols before the .aspx page name
                currentSlashIndex = serverSideUrl.IndexOf(slash, 0, dotIndex);
                while ((0 <= currentSlashIndex) && (currentSlashIndex < dotIndex))
                {
                    if (slashesBeforeDot == slashesBeforeCultureSpecification)
                    {
                        cultureSpecificationStartIndex = previousSlashIndex + 1;
                    }

                    slashesBeforeDot++;

                    previousSlashIndex = currentSlashIndex;
                    currentSlashIndex = serverSideUrl.IndexOf(slash, currentSlashIndex + 1, dotIndex - currentSlashIndex - 1);
                }
            }

            if ((slashesBeforeDot >= minExpectedSlashesBeforeDot) &&
                (expectedCultureSpecificationLength > 0) &&
                (0 < cultureSpecificationStartIndex) && (cultureSpecificationStartIndex < dotIndex)
                )
            {
                if (serverSideUrl.StartsWith(req.ApplicationPath) == true)
                {

                    string partBeforeCultureSpecificationFakeDir = serverSideUrl.Substring(0, cultureSpecificationStartIndex);

                    serverSideUrl = serverSideUrl.Remove(0, partBeforeCultureSpecificationFakeDir.Length);

                    string cultureSpecification = null;
                    int firstSlashIndex = serverSideUrl.IndexOf(slash);

                    if ((0 < firstSlashIndex) && (firstSlashIndex < serverSideUrl.Length))
                    {
                        int cultureSpecificationLength = firstSlashIndex + 1;
                        if (cultureSpecificationLength == expectedCultureSpecificationLength)
                        {
                            cultureSpecification = serverSideUrl.Substring(0, expectedCultureSpecificationLength);

                            serverSideUrl = serverSideUrl.Remove(0, cultureSpecification.Length);
                            string cultureSpecificationWithoutSlash = cultureSpecification.Remove(cultureSpecification.Length - 1);
                            StringBuilder serverSideUrlBuilder = new StringBuilder();

                            serverSideUrlBuilder.Append(partBeforeCultureSpecificationFakeDir);
                            serverSideUrlBuilder.Append(serverSideUrl);
                            if (fileTypeSupportsQueryString == true)
                            {
                                string queryString = req.Url.Query;
                                bool variantSpecificationNecessary = true;
                                if (string.IsNullOrEmpty(queryString) == false)
                                {
                                    serverSideUrlBuilder.Append(queryString);
                                    string requestedCultureStr = HttpContext.Current.Request.Params[CommonCode.UiUrl.QueryStringApplicationVariantKey];
                                    if (string.IsNullOrEmpty(requestedCultureStr) == true)
                                    {
                                        serverSideUrlBuilder.Append(amp);
                                    }
                                    else
                                    {
                                        variantSpecificationNecessary = false;
                                        if (string.Equals(cultureSpecificationWithoutSlash, requestedCultureStr) == false)
                                        {
                                            string errMsg = string.Format(
                                                "Contradicting application variants specified. By directory - \"{0}\"; by query string - \"{1}\".",
                                                cultureSpecificationWithoutSlash, requestedCultureStr);
                                            throw new UIException(errMsg);
                                        }
                                    }
                                }
                                else
                                {
                                    serverSideUrlBuilder.Append(quest);
                                }
                                if (variantSpecificationNecessary == true)
                                {
                                    serverSideUrlBuilder.Append(QueryStringApplicationVariantKey);
                                    serverSideUrlBuilder.Append(eq);
                                    serverSideUrlBuilder.Append(cultureSpecificationWithoutSlash);
                                }
                            }
                            serverSideUrl = serverSideUrlBuilder.ToString();
                            rewritten = true;
                        }
                    }
                }
            }

            if (rewritten == false)
            {
                serverSideUrl = userRequestedUrl;
            }
            return serverSideUrl;
        }

        /// <summary>
        /// Returns an URL to be used by the client dependent on the supplied URL that is used by the application.
        /// <para>For example, for "http://server/page.aspx?lang=en" the result is "http://server/en/page.aspx".</para>
        /// </summary>
        /// <param name="serverSideUrl">The URL that is used by the application.</param>
        /// <returns>The requested URL to be used by the client.</returns>
        public static string UrlUnRewrite(string serverSideUrl)
        {
            if (serverSideUrl == null)
            {
                throw new ArgumentNullException("serverSideUrl");
            }
            if (serverSideUrl == string.Empty)
            {
                throw new ArgumentException("serverSideUrl" + " is empty.");
            }
            if (HttpContext.Current == null)
            {
                throw new UIException("HttpContext.Current is null.");
            }
            if (HttpContext.Current.Request == null)
            {
                throw new UIException("HttpContext.Current.Request is null.");
            }

            HttpRequest req = HttpContext.Current.Request;

            string userSeenUrl = serverSideUrl;
            string cultureSpecificationKeyStr = QueryStringApplicationVariantKey + eq;
            int cultureSpecificationIndex = serverSideUrl.IndexOf(cultureSpecificationKeyStr);

            if ((0 < cultureSpecificationIndex) && (cultureSpecificationIndex < serverSideUrl.Length) &&
                (quest.Length <= cultureSpecificationIndex) && (amp.Length <= cultureSpecificationIndex))
            {
                string serverSideUrlBeforeCultureSpecification =
                    serverSideUrl.Substring(0, cultureSpecificationIndex);

                if ((serverSideUrlBeforeCultureSpecification.EndsWith(quest) == true) ||
                    (serverSideUrlBeforeCultureSpecification.EndsWith(amp) == true))
                {
                    // Initially, this will be for example "lang=en&key1=val1&key2=val2", "lang=bgkey1=val1&key2=val2", etc.
                    // In the end, this will be for example "lang=en", "lang=bg", etc. 
                    string fullCultureSpecification = serverSideUrl.Substring(cultureSpecificationIndex);

                    // In the end, this will be for example "lang=en", "lang=bg", "lang=en&", "lang=bg&", etc. 
                    string toBeRemovedFromQueryString = fullCultureSpecification;

                    int nextQueryStringSeparator = fullCultureSpecification.IndexOf(amp);

                    if ((0 < nextQueryStringSeparator) && (nextQueryStringSeparator < fullCultureSpecification.Length))
                    {
                        fullCultureSpecification = fullCultureSpecification.Substring(0, nextQueryStringSeparator);
                        toBeRemovedFromQueryString = fullCultureSpecification + amp;
                    }

                    // This will be for example "en", "bg", etc.
                    string cultureStr = fullCultureSpecification.Substring(cultureSpecificationKeyStr.Length);

                    if (string.IsNullOrEmpty(cultureStr) == false)
                    {
                        string userSeenUrlWithoutCultureSpecification =
                            userSeenUrl.Remove(cultureSpecificationIndex, toBeRemovedFromQueryString.Length);
                        string partBeforeCultureSpecification = req.ApplicationPath;

                        if (userSeenUrlWithoutCultureSpecification.StartsWith(partBeforeCultureSpecification) == false)
                        {
                            partBeforeCultureSpecification =
                                string.Format("http{0}://{1}{2}", req.IsSecureConnection ? "s" : "",
                                req.Url.Authority ?? string.Empty,  // req.Url.Host ?? string.Empty, 
                                req.ApplicationPath);
                        }
                        if (userSeenUrlWithoutCultureSpecification.StartsWith(partBeforeCultureSpecification) == false)
                        {
                            //partBeforeCultureSpecification = string.Empty;
                            string errMsg1 = string.Format("Unrecognized start of the \"{0}\" server-side URL. Cannot unrewrite.", serverSideUrl);
                            throw new UIException(errMsg1);
                        }
                        userSeenUrlWithoutCultureSpecification =
                            userSeenUrlWithoutCultureSpecification.Remove(0, partBeforeCultureSpecification.Length);

                        if (userSeenUrlWithoutCultureSpecification.EndsWith(quest) == true)
                        {
                            userSeenUrlWithoutCultureSpecification =
                                userSeenUrlWithoutCultureSpecification.Remove(userSeenUrlWithoutCultureSpecification.Length - quest.Length);
                        }
                        if ((userSeenUrlWithoutCultureSpecification.EndsWith(quest) == true) ||
                            (userSeenUrlWithoutCultureSpecification.EndsWith(amp) == true))
                        {
                            userSeenUrlWithoutCultureSpecification =
                                userSeenUrlWithoutCultureSpecification.Remove(userSeenUrlWithoutCultureSpecification.Length - 1);
                        }

                        string testForAlreadyUnrewritten = slash + cultureStr;
                        StringBuilder userSeenUrlBuilder = new StringBuilder();

                        userSeenUrlBuilder.Append(partBeforeCultureSpecification);
                        if (userSeenUrlWithoutCultureSpecification.StartsWith(testForAlreadyUnrewritten) == false)
                        {
                            // If the last appended symbol is not '/', append one.
                            int bldrLen = userSeenUrlBuilder.Length;
                            if ((bldrLen == 0) || (userSeenUrlBuilder[bldrLen - 1].ToString() != slash))
                            {
                                userSeenUrlBuilder.Append(slash);
                            }

                            userSeenUrlBuilder.Append(cultureStr);
                        }

                        // If the last appended symbol is not '/', and the next symbol to be
                        // appended is not '/', append one.
                        int bldrLen1 = userSeenUrlBuilder.Length;
                        if ((bldrLen1 == 0) || (userSeenUrlBuilder[bldrLen1 - 1].ToString() != slash))
                        {
                            if (userSeenUrlWithoutCultureSpecification.StartsWith(slash) == false)
                            {
                                userSeenUrlBuilder.Append(slash);
                            }
                        }

                        userSeenUrlBuilder.Append(userSeenUrlWithoutCultureSpecification);
                        userSeenUrl = userSeenUrlBuilder.ToString();
                    }
                }
            }
            if (userSeenUrl == null)
            {
                string errMsg = string.Format("Could not create the user-seen URL. The result is null. Server-side URL: \"{0}\".", serverSideUrl);
                throw new UIException(errMsg);
            }
            return userSeenUrl;
        }

        /// <summary>
        /// Checks the supplied URL for extra application variant specifiers.
        /// </summary>
        /// <param name="url">The URL to be checked.</param>
        /// <returns>If there are no extra application variant specifiers, <c>true</c>; otherwise, <c>false</c>.</returns>
        public static bool CheckNoExtraApplicationVariantSpecifiersInUrl(string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            if (url == string.Empty)
            {
                throw new ArgumentException("url" + " is empty.");
            }

            bool passed = true;
            bool appVariantInQueryString = false;
            int posOfQueryStringApplicationVariantKey =
                url.IndexOf(CommonCode.UiUrl.QueryStringApplicationVariantKey, StringComparison.OrdinalIgnoreCase);
            if ((0 < posOfQueryStringApplicationVariantKey) && (posOfQueryStringApplicationVariantKey < url.Length))
            {
                char prevCh = url[posOfQueryStringApplicationVariantKey - 1];
                if ((prevCh == '?') || (prevCh == '&'))
                {
                    int appVarKeyLength = CommonCode.UiUrl.QueryStringApplicationVariantKey.Length;
                    int posAfterKey = posOfQueryStringApplicationVariantKey + appVarKeyLength;
                    if ((posAfterKey < url.Length) && (url[posAfterKey] == '='))
                    {
                        appVariantInQueryString = true;
                    }
                }
            }

            StringCollection supportedAppVariants = Configuration.SupportedApplicationVariants;
            int appVariantsAsDir = 0;

            for (int i = 0; (passed == true) && (i < supportedAppVariants.Count); i++)
            {
                string appVariant = supportedAppVariants[i];
                char slashCh = '/';
                int appVarPos = url.IndexOf(appVariant, StringComparison.OrdinalIgnoreCase);
                if ((0 <= appVarPos) && (appVarPos < url.Length))
                {
                    int posAfterAppVariant = appVarPos + appVariant.Length;
                    if ((url.Length > (posAfterAppVariant)) && (url[posAfterAppVariant] == slashCh))
                    {
                        if ((appVarPos == 0) || (url[appVarPos - 1] == slashCh))
                        {
                            appVariantsAsDir++;
                            if ((appVariantInQueryString == true) || (appVariantsAsDir > 1))
                            {
                                passed = false;
                            }
                        }
                    }
                }
            }

            return passed;
        }

        /// <summary>
        /// Checks the supplied URL for extra application variant specifiers.
        /// <para>If extra application variant specifiers, an exception is thrown.</para>
        /// </summary>
        /// <param name="url">The URL to be checked.</param>
        public static void VerifyNoExtraApplicationVariantSpecifiersInUrl(string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            if (url == string.Empty)
            {
                throw new ArgumentException("url" + " is empty.");
            }
            if (CheckNoExtraApplicationVariantSpecifiersInUrl(url) == false)
            {
                string errMsg = string.Format("The \"{0}\" URL has extra application variant specifiers.", url);
                throw new UIException(errMsg);
            }
        }

        /// <summary>
        /// Extracts the could-be application variant which is specified as a directory in the supplied URL.
        /// <para>For example, if the URL is "http://server/abc/page.aspx", the application variant will be "abc".</para>
        /// <para>The could-be application variant is a subdirectory at specific depth in the virtual path contained in the URL.</para>
        /// </summary>
        /// <param name="url">The URL to extract the application variant from.</param>
        /// <returns>The application variant or an empty string if no application variant is extracted.</returns>
        public static string NonCheckedApplicationVariantFromUrl(string url)
        {
            string appVariant = string.Empty;

            if (string.IsNullOrEmpty(url) == false)
            {
                int leftSlashIndex = url.IndexOf(slash);

                for (int i = 0; (leftSlashIndex != -1) && (i < (slashesBeforeCultureSpecification - 1)); i++)
                {
                    leftSlashIndex = url.IndexOf(slash, leftSlashIndex + 1);
                }

                int rightSlashIndex = (leftSlashIndex != -1) ? url.IndexOf(slash, leftSlashIndex + 1) : -1;

                if ((leftSlashIndex != -1) && (rightSlashIndex != -1))
                {
                    appVariant = url.Substring(leftSlashIndex + 1, rightSlashIndex - leftSlashIndex - 1);
                }
            }
            return appVariant;
        }

        /// <summary>
        /// Extracts the application variant which is specified as a directory in the supplied URL.
        /// <para>For example, if the URL is "http://server/en/page.aspx", the application variant will be "en".</para>
        /// </summary>
        /// <param name="url">The URL to extract the application variant from.</param>
        /// <returns>The application variant or an empty string if no valid application variant is specified in the URL.</returns>
        public static string ApplicationVariantFromUrl(string url)
        {
            string appVariant = NonCheckedApplicationVariantFromUrl(url);

            if ((appVariant == null) || (Configuration.IsSupportedApplicationVariant(appVariant) == false))
            {
                appVariant = string.Empty;
            }
            return appVariant;
        }
    }
}
