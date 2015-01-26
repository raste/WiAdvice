﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Specialized;
using System.Web;

using Microsoft.Web;

using BusinessLayer;
using DataAccess;

namespace UserInterface
{
    public class GetImageHandler : ImageHandler
    {
        public GetImageHandler()
        {
            // Set caching settings and add image transformations here
            // EnableServerCache = true;
        }

        public override ImageInfo GenerateImage(NameValueCollection parameters)
        {
            Entities objectContext = CommonCode.UiTools.CreateEntities();
            EntitiesUsers userContext = new EntitiesUsers();

            // Add image generation logic here and return an instance of ImageInfo

            ImageInfo generatedImgInfo = new ImageInfo(System.Net.HttpStatusCode.NotFound);

            User system = Tools.GetSystem();
            BusinessLog businessLog = new BusinessLog(system.ID);
            ImageTools imageTools = new ImageTools();

            string strProdID = parameters["prodID"];
            string strCompID = parameters["compID"];
            string strCatID = parameters["catID"];
            string strShow = parameters["show"];

            string fileType = string.Empty;
            long id = -1;

            if (!string.IsNullOrEmpty(strProdID))
            {
                if (long.TryParse(strProdID, out id))
                {
                    BusinessProduct businessProduct = new BusinessProduct();
                    Product currProduct = businessProduct.GetProductByIDWV(objectContext, id);
                    if (currProduct != null)
                    {
                        string appPath = HttpContext.Current.Request.PhysicalApplicationPath;
                        ProductImage img = imageTools.GetProductMainImage(objectContext, currProduct, businessLog, appPath);
                        if (img != null)
                        {
                            Byte[] bytes = null;

                            bytes = CommonCode.ImagesAndAdverts.LoadImageFileAsBytes(appPath, img.url);
                            if (bytes != null)
                            {
                                fileType = System.IO.Path.GetExtension(img.url);

                                if (img.width > Configuration.ImagesMainImageWidth || 
                                    img.height > Configuration.ImagesMainImageHeight)
                                {
                                    CommonCode.ImagesAndAdverts imagesAndAdverts = new UserInterface.CommonCode.ImagesAndAdverts();
                                    bytes = imagesAndAdverts.GetResizedImgFromBytes(bytes, Configuration.ImagesMainImageWidth,
                                        Configuration.ImagesMainImageHeight, true, fileType);
                                }
                                
                                generatedImgInfo = new ImageInfo(bytes);
                            }
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(strCompID))
            {
                if (long.TryParse(strCompID, out id))
                {
                    BusinessCompany businessCompany = new BusinessCompany();
                    Company currCompany = businessCompany.GetCompanyWV(objectContext, id);
                    if (currCompany != null)
                    {
                        string appPath = HttpContext.Current.Request.PhysicalApplicationPath;
                        CompanyImage img = imageTools.GetCompanyLogo(objectContext, currCompany, businessLog, appPath);
                        if (img != null)
                        {
                            Byte[] bytes = null;

                            bytes = CommonCode.ImagesAndAdverts.LoadImageFileAsBytes(appPath, img.url);
                            if (bytes != null)
                            {
                                if (img.width > Configuration.ImagesMainImageWidth || 
                                    img.height > Configuration.ImagesMainImageHeight)
                                {
                                    fileType = System.IO.Path.GetExtension(img.url);

                                    CommonCode.ImagesAndAdverts imagesAndAdverts = new UserInterface.CommonCode.ImagesAndAdverts();
                                    bytes = imagesAndAdverts.GetResizedImgFromBytes(bytes, Configuration.ImagesMainImageWidth
                                        , Configuration.ImagesMainImageHeight, true, fileType);
                                }
                                generatedImgInfo = new ImageInfo(bytes);
                            }
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(strCatID))
            {
                if (long.TryParse(strCatID, out id))
                {
                    BusinessCategory businessCategory = new BusinessCategory();
                    Category currCategory = businessCategory.GetWithoutVisible(objectContext, id);

                    if (currCategory != null)
                    {
                        string appPath = HttpContext.Current.Request.PhysicalApplicationPath;

                        if (imageTools.CheckIfCategoryImageExists(userContext, objectContext, currCategory, true, appPath))
                        {
                            Byte[] bytes = null;

                            bytes = CommonCode.ImagesAndAdverts.LoadImageFileAsBytes(appPath, currCategory.imageUrl);
                            if (bytes != null)
                            {
                                if (currCategory.imageHeight > Configuration.ImagesCategoryMaxHeight ||
                                    currCategory.imageWidth > Configuration.ImagesCategoryMaxWidth)
                                {
                                    fileType = System.IO.Path.GetExtension(currCategory.imageUrl);

                                    CommonCode.ImagesAndAdverts imagesAndAdverts = new UserInterface.CommonCode.ImagesAndAdverts();
                                    bytes = imagesAndAdverts.GetResizedImgFromBytes(bytes, Configuration.ImagesCategoryMaxWidth,
                                        Configuration.ImagesCategoryMaxHeight, true, fileType);
                                }
                                generatedImgInfo = new ImageInfo(bytes);
                            }

                        }
                    }
                }

            }
            else if (!string.IsNullOrEmpty(strShow))
            {
                string appPath = HttpContext.Current.Request.PhysicalApplicationPath;
                string imgUrl;

                int width;
                int height;

                GetImageUrlAndDimensions(strShow, out imgUrl, out width, out height);

                if (!string.IsNullOrEmpty(imgUrl))
                {
                    if (Tools.CheckIfFileExists(appPath, imgUrl))
                    {
                        Byte[] bytes = null;

                        bytes = CommonCode.ImagesAndAdverts.LoadImageFileAsBytes(appPath, imgUrl);

                        if (bytes != null)
                        {
                            fileType = System.IO.Path.GetExtension(imgUrl);

                            CommonCode.ImagesAndAdverts imagesAndAdverts = new UserInterface.CommonCode.ImagesAndAdverts();
                            bytes = imagesAndAdverts.GetResizedImgFromBytes(bytes, width, height, false, fileType);

                            generatedImgInfo = new ImageInfo(bytes);
                        }
                    }
                }

            }

            fileType = fileType.ToLower();
            switch (fileType)
            {
                case ".jpeg":
                    this.ContentType = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case ".jpg":
                    this.ContentType = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    this.ContentType = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                default:
                    this.ContentType = System.Drawing.Imaging.ImageFormat.Png;
                    break;
            }

            return generatedImgInfo;
        }

        /// <summary>
        /// Retrieves the URL and dimensions of a sample image specified
        /// by <paramref name="strWhichImage"/>.
        /// </summary>
        /// <param name="strWhichImage">Which image to show, e.g. one of "minCompanyImage", "minProductImage", "minLogo".</param>
        /// <param name="imgUrl">After the method returns will contain the image URL, or an empty string on error.</param>
        /// <param name="width">After the method returns will contain the image width, or <c>0</c> on error.</param>
        /// <param name="height">After the method returns will contain the image height, or <c>0</c> on error.</param>
        internal static void GetImageUrlAndDimensions(string strWhichImage, 
            out string imgUrl, out int width, out int height)
        {
            if (strWhichImage == null)
            {
                throw new ArgumentNullException("strWhichImage");
            }
            switch (strWhichImage)
            {
                case "minCompanyImage":

                    imgUrl = "images/SiteImages/minCompImg.jpg";

                    width = Configuration.ImagesMinImageWidth;
                    height = Configuration.ImagesMinImageHeight;

                    break;
                case "minProductImage":

                    imgUrl = "images/SiteImages/minProdImg.jpg";

                    width = Configuration.ImagesMinImageWidth;
                    height = Configuration.ImagesMinImageHeight;

                    break;
                case "minLogo":

                    imgUrl = "images/SiteImages/minLogoImg.jpg";

                    width = Configuration.ImagesMinCompLogoWidth;
                    height = Configuration.ImagesMinCompLogoHeight;

                    break;
                default:
                    imgUrl = string.Empty;
                    width = 0;
                    height = 0;
                    break;
            }
        }

    }
}
