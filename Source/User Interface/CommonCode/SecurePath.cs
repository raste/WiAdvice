﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace UserInterface.CommonCode
{
    #region "SecurePage Class"
    public class SecurePath
    {

        public static bool IsSecure(string path)
        {
            List<SecurePage> lstPages = new List<SecurePage>();

            bool isSecure = false;

            NameValueCollection sectionPages = (NameValueCollection)ConfigurationManager.GetSection("SecurePages");

            foreach (string key in sectionPages)
            {
                if ((!string.IsNullOrEmpty(key)) && (!string.IsNullOrEmpty(sectionPages.Get(key))))
                {
                    lstPages.Add(new SecurePage { PathType = sectionPages.Get(key), Path = key });
                }
            }

            bool pageFound = false;

            for (int i = 0; ((pageFound == false) && (i < lstPages.Count)); i++)
            {
                SecurePage page = lstPages[i];

                switch (page.PathType.ToLower().Trim())
                {
                    case "directory":
                        if (path.Contains(page.Path))
                        {
                            isSecure = true;
                            pageFound = true;
                        }
                        break;
                    case "page":
                        if (path.ToLower().Trim() == page.Path.ToLower().Trim())
                        {
                            isSecure = true;
                            pageFound = true;
                        }
                        break;
                    default:
                        isSecure = false;
                        break;
                }
            }

            return isSecure;
        }

        public static bool IsSubcontent(string path)
        {
            List<SecurePage> lstPages = new List<SecurePage>();

            bool isSubcontent = false;

            NameValueCollection sectionPages = (NameValueCollection)ConfigurationManager.GetSection("SecurePages");

            foreach (string key in sectionPages)
            {
                if ((!string.IsNullOrEmpty(key)) && (!string.IsNullOrEmpty(sectionPages.Get(key))))
                {
                    lstPages.Add(new SecurePage { PathType = sectionPages.Get(key), Path = key });
                }
            }


            bool pageFound = false;

            for (int i = 0; ((pageFound == false) && (i < lstPages.Count)); i++)
            {
                SecurePage page = lstPages[i];

                switch (page.PathType.ToLower().Trim())
                {
                    case "subcontent":
                        if (path.ToLower().Trim() == page.Path.ToLower().Trim())
                        {
                            isSubcontent = true;
                            pageFound = true;
                        }
                        break;
                    default:
                        isSubcontent = false;
                        break;
                }
            }

            return isSubcontent;
        }

    }
    #endregion
}
