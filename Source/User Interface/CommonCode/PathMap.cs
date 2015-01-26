﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

#define USE_ORIGINAL_IMAGE_PATH_CONSTRUCTION

using System;
using System.Web;

using BusinessLayer;

namespace UserInterface.CommonCode
{
    public class PathMap
    {
        /// <summary>
        /// Gets the name of the local resource directory in which are all the resources(images) local language variant.
        /// Returns rEN for en; rBG for bg. To be used only when Thread.CurrentThread.CurrentUICulture is SET, otherwise will return default language name dir
        /// </summary>
        private static string LocalResourceDir
        {
            get
            {
                string applicationVariant = Tools.ApplicationVariantString;

                string localResourceDir = string.Empty;

                switch (applicationVariant)
                {
                    case "bg":
                        localResourceDir = "rBG";
                        break;
                    case "en":
                        localResourceDir = "rEN";
                        break;
                    default:
                        throw new UIException(string.Format("ApplicationVariantString = {0} is not supported for local resources directory"
                            , applicationVariant));
                }

                return localResourceDir;
            }
        }

        /// <summary>
        /// Gets the fyle system path of the root directory of the application.
        /// <para>Example: "C:\inetpub\MySite".</para>
        /// </summary>
        public static string PhysicalApplicationPath
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    throw new UIException("HttpContext.Current is null.");
                }
                if (HttpContext.Current.Request == null)
                {
                    throw new UIException("HttpContext.Current.Request is null.");
                }
                if (string.IsNullOrEmpty(HttpContext.Current.Request.PhysicalApplicationPath) == true)
                {
                    throw new UIException("HttpContext.Current.Request.PhysicalApplicationPath is null or empty.");
                }
                return HttpContext.Current.Request.PhysicalApplicationPath;
            }
        }

        /// <summary>
        /// Gets the virtual application root path on the server.
        /// <para>Example: "/".</para>
        /// </summary>
        public static string WebApplicationPath
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    throw new UIException("HttpContext.Current is null.");
                }
                if (HttpContext.Current.Request == null)
                {
                    throw new UIException("HttpContext.Current.Request is null.");
                }
                if (string.IsNullOrEmpty(HttpContext.Current.Request.ApplicationPath) == true)
                {
                    throw new UIException("HttpContext.Current.Request.ApplicationPath is null or empty.");
                }
                string webAppPath = HttpContext.Current.Request.ApplicationPath;
                if ((string.IsNullOrEmpty(webAppPath) == false) && (webAppPath.EndsWith("/") == false))
                {
                    webAppPath = webAppPath + "/";
                }
                return webAppPath;
            }
        }

        /// <summary>
        /// Returns path to directory where company images should be saved
        /// </summary>
        public static String GetCompanyImagePath(string savedAsName)
        {
#if USE_ORIGINAL_IMAGE_PATH_CONSTRUCTION
            string physicalAppPath = PhysicalApplicationPath;
            string physicalPath = System.IO.Path.Combine(physicalAppPath,
                string.Format("images\\{0}\\CompanyImages", LocalResourceDir));
            if (!System.IO.Directory.Exists(physicalPath))
            {
                System.IO.Directory.CreateDirectory(physicalPath);
            }
            String path = System.IO.Path.Combine(physicalPath, savedAsName);
            return path;
#else
            string physicalAppPath = GetImagesPhysicalPathRoot();  // PhysicalApplicationPath;
            string physicalPath = System.IO.Path.Combine(physicalAppPath, 
                string.Format("images\\{0}\\CompanyImages", LocalResourceDir));
            if (!System.IO.Directory.Exists(physicalPath))
            {
                System.IO.Directory.CreateDirectory(physicalPath);
            }

            String path = System.IO.Path.Combine(physicalPath , savedAsName);
            return path;
#endif
        }

        /// <summary>
        /// Returns Company Image URL
        /// </summary>
        /// <returns>name of company image file</returns>
        public static string GetCompanyImgUrl(string savedAsName)
        {
#if USE_ORIGINAL_IMAGE_URL_CONSTRUCTION
            string virtualAppPath = WebApplicationPath;
            if (string.Equals("/", virtualAppPath) == true)
            {
                virtualAppPath = string.Empty;
            }
            string imgUrl = string.Format(
                "{0}{1}{2}{3}{4}",  
                virtualAppPath,
                "images/",
                LocalResourceDir,
                "/CompanyImages/",
                savedAsName);
            return imgUrl;
#else
            CheckSavedAsName(savedAsName);
            string imgUrl = string.Format("images/{0}/CompanyImages/{1}", LocalResourceDir, savedAsName);
            return imgUrl;
#endif
        }

        public static string GetCategoryImgUrl(string savedAsName)
        {
#if USE_ORIGINAL_IMAGE_URL_CONSTRUCTION
            string virtualAppPath = WebApplicationPath;
            if (string.Equals("/", virtualAppPath) == true)
            {
                virtualAppPath = string.Empty;
            }
            string imgUrl = string.Format(
                "{0}{1}{2}{3}{4}",  
                virtualAppPath,
                "images/",
                LocalResourceDir,
                "/CategoryImages/",
                savedAsName);
            return imgUrl;
#else
            CheckSavedAsName(savedAsName);
            string imgUrl = string.Format("images/{0}/CategoryImages/{1}", LocalResourceDir, savedAsName);
            return imgUrl;
#endif
        }

        private static void CheckSavedAsName(string savedAsName)
        {
            if (savedAsName == null)
            {
                throw new ArgumentNullException("savedAsName");
            }
            if (savedAsName == string.Empty)
            {
                throw new ArgumentException("savedAsName" + " is empty.");
            }
        }

        public static String GetCompanyImageDirPath()
        {
            string physicalAppPath = PhysicalApplicationPath;  // GetImagesPhysicalPathRoot();
            string physicalPath = System.IO.Path.Combine(physicalAppPath, string.Format("images\\{0}\\CompanyImages", LocalResourceDir));
            return physicalPath;
        }

        public static String GetCategoryImageDirPath()
        {
            string physicalAppPath = PhysicalApplicationPath;  // GetImagesPhysicalPathRoot();
            string physicalPath = System.IO.Path.Combine(physicalAppPath, string.Format("images\\{0}\\CategoryImages", LocalResourceDir));
            return physicalPath;
        }

        public static String GetProductImageDirPath()
        {
            string physicalAppPath = PhysicalApplicationPath;  // GetImagesPhysicalPathRoot();
            string physicalPath = System.IO.Path.Combine(physicalAppPath, string.Format("images\\{0}\\ProductImages", LocalResourceDir));
            return physicalPath;
        }

        public static String GetProductImagePath(string savedAsName)
        {
#if USE_ORIGINAL_IMAGE_PATH_CONSTRUCTION
            string physicalAppPath = PhysicalApplicationPath;
            string physicalPath = System.IO.Path.Combine(physicalAppPath
                , string.Format("images\\{0}\\ProductImages", LocalResourceDir));
            if (!System.IO.Directory.Exists(physicalPath))
            {
                System.IO.Directory.CreateDirectory(physicalPath);
            }
            String path = System.IO.Path.Combine(physicalPath, savedAsName);
            return path;
#else
            string physicalAppPath = GetImagesPhysicalPathRoot();  // PhysicalApplicationPath;
            string physicalPath = System.IO.Path.Combine(physicalAppPath, 
                string.Format("images\\{0}\\ProductImages", LocalResourceDir));
            if (!System.IO.Directory.Exists(physicalPath))
            {
                System.IO.Directory.CreateDirectory(physicalPath);
            }

            String path = System.IO.Path.Combine(physicalPath, savedAsName);
            return path;
#endif
        }

        /// <summary>
        /// Returns ProductImage URL
        /// </summary>
        /// <param name="savedAsName">name of product image file</param>
        public static string GetProductImgUrl(string savedAsName)
        {
#if USE_ORIGINAL_IMAGE_URL_CONSTRUCTION
            string virtualAppPath = WebApplicationPath;
            if (string.Equals("/", virtualAppPath) == true)
            {
                virtualAppPath = string.Empty;
            }
            string imgUrl = string.Format(
                "{0}{1}{2}{3}{4}", 
                virtualAppPath,
                "images/",
                LocalResourceDir,
                "/ProductImages/",
                savedAsName);
            return imgUrl;
#else
            CheckSavedAsName(savedAsName);
            string imgUrl = string.Format("images/{0}/ProductImages/{1}", LocalResourceDir, savedAsName);
            return imgUrl;
#endif
        }


        /// <summary>
        /// Returns Path to directory where Adverisement files should be saved
        /// </summary>
        public static String GetAdvertisementDirPath()
        {
            string physicalAppPath = PhysicalApplicationPath;  // GetImagesPhysicalPathRoot();
            string physicalPath = System.IO.Path.Combine(physicalAppPath
                , string.Format("images\\{0}\\Advertisements", LocalResourceDir));
            return physicalPath;
        }

        /// <summary>
        /// Returns complete path to advertisement file
        /// </summary>
        /// <param name="savedAsName">advertisement file</param>
        public static String GetAdvertisementPath(string savedAsName)
        {
#if USE_ORIGINAL_IMAGE_PATH_CONSTRUCTION
            string physicalAppPath = PhysicalApplicationPath;  // GetImagesPhysicalPathRoot();
            string physicalPath = System.IO.Path.Combine(physicalAppPath
                , string.Format("images\\{0}\\Advertisements", LocalResourceDir));
            if (!System.IO.Directory.Exists(physicalPath))
            {
                System.IO.Directory.CreateDirectory(physicalPath);
            }
            String path = System.IO.Path.Combine(physicalPath, savedAsName);
            return path;
#else
            string physicalAppPath = GetImagesPhysicalPathRoot();  // PhysicalApplicationPath;
            string physicalPath = System.IO.Path.Combine(physicalAppPath
                , string.Format("images\\{0}\\Advertisements", LocalResourceDir));
            if (!System.IO.Directory.Exists(physicalPath))
            {
                System.IO.Directory.CreateDirectory(physicalPath);
            }

            String path = System.IO.Path.Combine(physicalPath, savedAsName);
            return path;
#endif
        }


        /// <summary>
        /// Returns Adverisement file URL
        /// </summary>
        /// <param name="savedAsName">name of the advertisment file</param>
        public static string GetAdvertisementUrl(string savedAsName)
        {
#if USE_ORIGINAL_IMAGE_URL_CONSTRUCTION
            string virtualAppPath = WebApplicationPath;
            if (string.Equals("/", virtualAppPath) == true)
            {
                virtualAppPath = string.Empty;
            }
            string imgUrl = string.Format(
                "{0}{1}{2}{3}{4}",  
                virtualAppPath,
                "images/",
                LocalResourceDir,
                "/Advertisements/",
                savedAsName);
            return imgUrl;
#else
            CheckSavedAsName(savedAsName);
            string imgUrl = string.Format("images/{0}/Advertisements/{1}", LocalResourceDir, savedAsName);
            return imgUrl;
#endif
        }


    }
}
