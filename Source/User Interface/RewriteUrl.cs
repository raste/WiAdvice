﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Web;

using BusinessLayer;

using log4net;

namespace UserInterface
{
    public class RewriteUrl : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            // Below is an example of how you can handle LogRequest event and provide 
            // custom logging implementation for it
            //context.LogRequest += new EventHandler(OnLogRequest);

            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        #endregion

        private static ILog log = LogManager.GetLogger(typeof(RewriteUrl));

        void context_BeginRequest(object sender, EventArgs e)
        {
            if ((Configuration.UseUrlRewriting == true) && 
                (Configuration.UseExternalUrlRewriteModule == false))
            {
                string urlToRewrite = HttpContext.Current.Request.Url.AbsolutePath;
                bool asciiUrl = AsciiSymbolsOnlyUrl(urlToRewrite);

                if (asciiUrl == true)
                {
                    string rewrittenUrl = CommonCode.UiUrl.UrlRewrite(urlToRewrite);
                    if (string.Equals(rewrittenUrl, urlToRewrite) == false)
                    {
                        if (log.IsDebugEnabled == true)
                        {
                            log.DebugFormat("URL rewritten from \"{0}\" to \"{1}\".",
                                HttpContext.Current.Request.Url.AbsolutePath, rewrittenUrl);
                        }
                        HttpContext.Current.RewritePath(rewrittenUrl, true);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the supplied URL contains ASCII symbols only.
        /// </summary>
        /// <param name="url">The URL to be checked.</param>
        /// <returns>If the URL contains ASCII symbols only - <c>true</c>; otherwise - <c>false</c>.</returns>
        private bool AsciiSymbolsOnlyUrl(string url)
        {
            bool asciiOnly = false;

            if (string.IsNullOrEmpty(url) == false)
            {
                asciiOnly = (url.Contains("%") == false);
                for (int i = 0; (asciiOnly == true) && (i < url.Length); i++)
                {
                    char ch = url[i];
                    if ((ch < '!') || ('z' < ch))
                    {
                        asciiOnly = false;
                    }
                }
            }
            return asciiOnly;
        }
    }
}
