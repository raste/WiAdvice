﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Specialized;
using System.Configuration;

namespace UserInterface
{
    public partial class SessionRefresh : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool refreshSession = false;
            NameValueCollection appSettingsCollection = ConfigurationManager.AppSettings;
            if (appSettingsCollection != null)
            {
                string refreshSessionSetting = appSettingsCollection["refreshSession"];

                if (string.Equals("True", refreshSessionSetting))
                {
                    refreshSession = true;
                }
            }

            if (refreshSession == true)
            {
                string refreshKey = "SessionRefreshJavascriptInserted";
                bool javascriptRegistered = ClientScript.IsClientScriptBlockRegistered(refreshKey);

                if (javascriptRegistered == false)
                {
                    int timeoutSeconds = (Session.Timeout * 60) - 10;
                    int timeoutMilliseconds = timeoutSeconds * 1000;
                    string javascriptCode =
                        string.Format("setTimeout(\"document.location.reload()\", {0});", timeoutMilliseconds);

                    ClientScript.RegisterClientScriptBlock(GetType(), refreshKey, javascriptCode, true);
                }
            }
        }   
    }
}
