﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.IO;

using DataAccess;
using BusinessLayer;

namespace UserInterface.CommonCode
{
    public class ImagesAndAdverts
    {
        /// <summary>
        /// Generates Thumbnail for ProductImage and add`s it to database
        /// </summary>
        public static void GenerateProductThumbnail(EntitiesUsers userContext, Entities objectContext
            , BusinessLog bLog, ProductImage mainImg, Product currProduct, User currUser, byte[] fileContents)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (mainImg == null)
            {
                throw new BusinessException("mainImg is null");
            }
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            String fileType = ".bmp";
            ImageTools imageTools = new ImageTools();

            int width = 0;
            int height = 0;
            ResizeImage(mainImg.height, mainImg.width, Configuration.ImagesThumbnailPrictureHeight,
                Configuration.ImagesThumbnailPictureWidth, out height, out width);

            ProductImage thumbImg = new ProductImage();
            thumbImg.Product = currProduct;
            thumbImg.url = "";
            thumbImg.main = false;
            thumbImg.dateCreated = mainImg.dateCreated;
            thumbImg.CreatedBy = Tools.GetUserID(objectContext, currUser);
            thumbImg.description = mainImg.description;
            thumbImg.width = width;
            thumbImg.height = height;
            thumbImg.isThumbnail = true;
            thumbImg.mainImgID = mainImg.ID;
            imageTools.AddProductImage(userContext, objectContext, thumbImg, bLog, currUser);

            String path = "";
            String url = "";
            GenerateProductImageNamePathUrl(fileType, currProduct, thumbImg, out path, out url);
            imageTools.ChangeProductImageUrl(objectContext, thumbImg, url);

            string appPath = CommonCode.PathMap.PhysicalApplicationPath;  // CommonCode.PathMap.GetImagesPhysicalPathRoot();
            string imagePathFromRoot = System.IO.Path.Combine(appPath, path);

            SaveThumbnail(fileContents, width, height, imagePathFromRoot, null);
        }

        /// <summary>
        /// Creates thumbnail from BytesArray and saves it to path or stream. Only path OR stream needs to be given,
        /// the other need to be null/empty or it will trow exception
        /// </summary>
        private static void SaveThumbnail(byte[] fileContents, int width, int height, string imagePathFromRoot, Stream targetStream)
        {
            if (string.IsNullOrEmpty(imagePathFromRoot) && targetStream == null)
            {
                throw new UIException("imagePathFromRoot is empty AND targetStream is null");
            }
            else if (!string.IsNullOrEmpty(imagePathFromRoot) && targetStream != null)
            {
                throw new UIException("imagePathFromRoot is not empty AND targetStream is not null");
            }


            using (System.IO.Stream strm = new System.IO.MemoryStream(fileContents, false))
            {
                using (System.Drawing.Image img = System.Drawing.Image.FromStream(strm, true, true))
                {
                    using (Bitmap thumbBitmap = new Bitmap(width, height))
                    {
                        using (Graphics thumbGrapgh = Graphics.FromImage(thumbBitmap))
                        {
                            thumbGrapgh.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            thumbGrapgh.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            thumbGrapgh.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                            Rectangle imageRec = new Rectangle(0, 0, width, height);
                            thumbGrapgh.DrawImage(img, imageRec);

                            if (!string.IsNullOrEmpty(imagePathFromRoot))
                            {
                                thumbBitmap.Save(imagePathFromRoot, img.RawFormat);
                            }
                            else
                            {
                                thumbBitmap.Save(targetStream, img.RawFormat);
                            }
                        }
                    }
                }

            }
        }


        /// <summary>
        /// Generates CompanyImage thumbnail and add`s it to database
        /// </summary>
        public static void GenerateCompanyThumbnail(EntitiesUsers userContext, Entities objectContext, BusinessLog bLog, CompanyImage mainImg,
            Company currCompany, User currUser, byte[] fileContents, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (mainImg == null)
            {
                throw new BusinessException("mainImg is null");
            }
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            String fileType = ".bmp";
            ImageTools imageTools = new ImageTools();

            int width = 0;
            int height = 0;
            ResizeImage(mainImg.height, mainImg.width, Configuration.ImagesThumbnailPrictureHeight,
               Configuration.ImagesThumbnailPictureWidth, out height, out width);

            CompanyImage thumbImg = new CompanyImage();
            thumbImg.Company = currCompany;
            thumbImg.url = "";
            thumbImg.isLogo = false;
            thumbImg.dateCreated = mainImg.dateCreated;
            thumbImg.CreatedBy = Tools.GetUserID(objectContext, currUser);
            thumbImg.description = mainImg.description;
            thumbImg.width = width;
            thumbImg.height = height;
            thumbImg.isThumbnail = true;
            thumbImg.mainImgID = mainImg.ID;
            imageTools.AddCompanyImage(userContext, objectContext, thumbImg, bLog, currUser);

            String path = "";
            String url = "";
            GenerateCompanyImageNamePathUrl(fileType, currCompany, thumbImg, out path, out url);
            imageTools.ChangeCompanyImageUrl(objectContext, thumbImg, url);


            string completePath = System.IO.Path.Combine(appPath, path);

            SaveThumbnail(fileContents, width, height, completePath, null);
        }

        /// <summary>
        /// Generates for new ProductImage file path and url 
        /// </summary>
        public static void GenerateProductImageNamePathUrl(string fileType, Product currProduct, ProductImage newImage, out String path, out String url)
        {
            ImageTools imageTools = new ImageTools();
            String name = imageTools.GetProductImageName(currProduct, newImage.ID, CommonCode.PathMap.GetProductImageDirPath(), fileType);
            path = CommonCode.PathMap.GetProductImagePath(name);
            url = CommonCode.PathMap.GetProductImgUrl(name);
        }

        /// <summary>
        /// Generates for new CompanyImage file path and url 
        /// </summary>
        public static void GenerateCompanyImageNamePathUrl(string fileType, Company currCompany, CompanyImage newImage, out String path, out String url)
        {
            ImageTools imageTools = new ImageTools();
            String name = imageTools.GetCompanyImageName(currCompany, newImage.ID, CommonCode.PathMap.GetCompanyImageDirPath(), fileType);
            path = CommonCode.PathMap.GetCompanyImagePath(name);
            url = CommonCode.PathMap.GetCompanyImgUrl(name);
        }


        public static void GenerateCategoryImageNamePathUrl(string fileType, Category currCategory, out String url)
        {
            ImageTools imageTools = new ImageTools();
            String name = imageTools.GetCategoryImageName(currCategory, CommonCode.PathMap.GetCategoryImageDirPath(), fileType);
            url = CommonCode.PathMap.GetCategoryImgUrl(name);
        }

        /// <summary>
        /// Generates for new Advertisement file path and url
        /// </summary>
        public static void GenerateAdvertisementFileNamePathUrl(string fileType, Advertisement currAdvert, out String path, out String url)
        {
            BusinessAdvertisement businessAdvert = new BusinessAdvertisement();
            String name = businessAdvert.GetAdvertisementFileName(currAdvert, CommonCode.PathMap.GetAdvertisementDirPath(), fileType);
            path = CommonCode.PathMap.GetAdvertisementPath(name);
            url = CommonCode.PathMap.GetAdvertisementUrl(name);
        }

        /// <param name="tolevel">1 - level 1; 2 - level 1,2; 3 - all </param>
        public static List<Advertisement> GetAdvertisementsForType(Entities objectContext, String type, long id, int numAds, int tolevel
            , out int primaryAds, out int secAds, out int thirdAds)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new CommonCode.UIException("id is < 1");
            }
            if (numAds < 1)
            {
                throw new CommonCode.UIException("numAds is < 1");
            }

            BusinessAdvertisement businessAdvertisement = new BusinessAdvertisement();

            List<Advertisement> adverts = new List<Advertisement>();
            List<Advertisement> otherAds = new List<Advertisement>();

            primaryAds = 0;
            secAds = 0;
            thirdAds = 0;

            switch (type)
            {
                case ("general"):
                    adverts = businessAdvertisement.GetGeneralAdverts(objectContext);
                    primaryAds = adverts.Count;
                    break;
                case ("company"):
                    adverts = businessAdvertisement.GetProductCompanyOrCategoryAdverts(objectContext, "company", id, true);
                    primaryAds = adverts.Count;
                    if (primaryAds < numAds && tolevel >= 2) // level 2
                    {
                        otherAds = businessAdvertisement.GetCompanyProductsAdverts(objectContext, id);
                        if (otherAds.Count > 0)
                        {
                            foreach (Advertisement advert in otherAds)
                            {
                                if (!adverts.Contains(advert))
                                {
                                    adverts.Add(advert);
                                    secAds++;
                                }
                            }
                        }

                        if (adverts.Count < numAds && tolevel >= 3)  // level 3
                        {
                            otherAds = businessAdvertisement.GetGeneralAdverts(objectContext);
                            foreach (Advertisement advert in otherAds)
                            {
                                if (!adverts.Contains(advert))
                                {
                                    adverts.Add(advert);
                                    thirdAds++;
                                }
                            }
                        }
                    }
                    break;
                case ("category"):
                    adverts = businessAdvertisement.GetProductCompanyOrCategoryAdverts(objectContext, "category", id, true);
                    primaryAds = adverts.Count;
                    if (primaryAds < numAds && tolevel >= 2) // level 2
                    {
                        otherAds = businessAdvertisement.GetCategoryParentAdverts(objectContext, id);
                        if (otherAds.Count > 0)
                        {
                            foreach (Advertisement advert in otherAds)
                            {
                                if (!adverts.Contains(advert))
                                {
                                    adverts.Add(advert);
                                    secAds++;
                                }
                            }
                        }

                        if (adverts.Count < numAds && tolevel >= 3) // level 3
                        {

                            otherAds = businessAdvertisement.GetGeneralAdverts(objectContext);
                            foreach (Advertisement advert in otherAds)
                            {
                                if (!adverts.Contains(advert))
                                {
                                    adverts.Add(advert);
                                    thirdAds++;
                                }
                            }

                        }
                    }
                    break;
                case ("product"):
                    adverts = businessAdvertisement.GetProductCompanyOrCategoryAdverts(objectContext, "product", id, true);
                    primaryAds = adverts.Count;
                    if (primaryAds < numAds && tolevel >= 2) // level 2
                    {
                        BusinessProduct businessProduct = new BusinessProduct();
                        Product currProduct = businessProduct.GetProductByIDWV(objectContext, id);
                        if (currProduct == null)
                        {
                            throw new UIException("currProduct is null");
                        }
                        if (!currProduct.CompanyReference.IsLoaded)
                        {
                            currProduct.CompanyReference.Load();
                        }

                        /// second level
                        otherAds = businessAdvertisement.GetCompanyProductsAdverts(objectContext, currProduct.Company.ID);

                        List<Advertisement> prodCompAds = businessAdvertisement.GetProductCompanyOrCategoryAdverts(objectContext, "company", currProduct.Company.ID, true);
                        if (prodCompAds.Count > 0)
                        {
                            foreach (Advertisement compAd in prodCompAds)
                            {
                                if (!otherAds.Contains(compAd))
                                {
                                    otherAds.Add(compAd);
                                }
                            }
                        }

                        if (otherAds.Count > 0)
                        {
                            foreach (Advertisement advert in otherAds)
                            {
                                if (!adverts.Contains(advert))
                                {
                                    adverts.Add(advert);
                                    secAds++;
                                }
                            }
                        }

                        /// level 3
                        if (adverts.Count < numAds && tolevel >= 3)
                        {
                            int remNumNeedAdverts = numAds - adverts.Count;

                            if (!currProduct.CategoryReference.IsLoaded)
                            {
                                currProduct.CategoryReference.Load();
                            }

                            otherAds = businessAdvertisement.GetProductCompanyOrCategoryAdverts
                                (objectContext, "category", currProduct.Category.ID, true); // advertisements for product`s category

                            List<Advertisement> otherProdAds = businessAdvertisement.GetCategoryParentAdverts(objectContext, currProduct.Category.ID);
                            if (otherProdAds.Count > 0)  // puts advertisements for parent categories of the category in which is the product
                            {
                                foreach (Advertisement padv in otherProdAds)
                                {
                                    if (!otherAds.Contains(padv))
                                    {
                                        otherAds.Add(padv);
                                    }
                                }
                            }

                            /// level 3.5
                            if (remNumNeedAdverts < otherAds.Count)
                            {
                                otherProdAds = businessAdvertisement.GetGeneralAdverts(objectContext); // puts general advertisements
                                if (otherProdAds.Count > 0)
                                {
                                    foreach (Advertisement genAd in otherProdAds)
                                    {
                                        if (!otherAds.Contains(genAd))
                                        {
                                            otherAds.Add(genAd);
                                        }
                                    }
                                }
                            }

                            if (otherAds.Count > 0)
                            {
                                foreach (Advertisement advert in otherAds)
                                {
                                    if (!adverts.Contains(advert))
                                    {
                                        adverts.Add(advert);
                                        thirdAds++;
                                    }
                                }
                            }
                        }

                    }
                    break;
                default:
                    throw new UIException(string.Format("type = {0} is not supported type", type));
            }


            return adverts;
        }

        /// <summary>
        /// Returns Ready to Show advertisments in PlaceHolder
        /// </summary>
        /// <param name="type">general,company,category,product</param>
        /// <param name="id">1 if its general</param>
        public static PlaceHolder GetAdvertisements(Entities objectContext, HttpServerUtility Server, String type, long id, int adsOnPage)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (Server == null)
            {
                throw new UIException("HttpServerUtility Server is null");
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new CommonCode.UIException("type is null or empty");
            }

            if (id < 1)
            {
                throw new CommonCode.UIException("id is < 1");
            }

            if (adsOnPage < 0 || adsOnPage > 20)
            {
                throw new CommonCode.UIException(string.Format("adsOnPage is {0}", adsOnPage));
            }

            if (adsOnPage == 0)
            {
                return null;
            }

            BusinessAdvertisement businessAdvertisement = new BusinessAdvertisement();

            List<Advertisement> adverts = new List<Advertisement>();

            int primaryAds = 0;
            int secAds = 0;
            int thirdAds = 0;

            adverts = GetAdvertisementsForType(objectContext, type, id, adsOnPage, 3, out primaryAds, out secAds, out thirdAds); // gets the advertisements

            PlaceHolder phAds = new PlaceHolder();

            int numAdverts = adverts.Count;
            int numAdvertsShown = 0;

            if (numAdverts > 0)
            {
                Random generator = new Random();
                List<int> numbers = new List<int>();
                int num = 0;

                List<Control> advertisements = new List<Control>();

                if (numAdverts <= adsOnPage)
                {
                    foreach (Advertisement advert in adverts)
                    {
                        advertisements.Add(GetShowReadyAdvert(advert, Server));
                    }
                }
                else
                {

                    if (primaryAds <= adsOnPage)
                    {
                        if (primaryAds > 0)
                        {
                            for (int i = 0; i < primaryAds; i++)
                            {
                                numAdvertsShown++;
                                advertisements.Add(GetShowReadyAdvert(adverts[i], Server));
                            }
                        }

                        int adsToBeShown = adsOnPage - numAdvertsShown;
                        if (secAds <= adsToBeShown)
                        {
                            if (secAds > 0)
                            {
                                int tillAdvNum = secAds + numAdvertsShown;

                                for (int i = numAdvertsShown; i < tillAdvNum; i++)
                                {
                                    numAdvertsShown++;
                                    advertisements.Add(GetShowReadyAdvert(adverts[i], Server));
                                }
                            }

                            adsToBeShown = adsOnPage - numAdvertsShown;
                            if (thirdAds <= adsToBeShown)
                            {
                                if (thirdAds > 0)
                                {
                                    int tillAdvNum = thirdAds + numAdvertsShown;

                                    for (int i = numAdvertsShown; i < tillAdvNum; i++)
                                    {
                                        numAdvertsShown++;
                                        advertisements.Add(GetShowReadyAdvert(adverts[i], Server));
                                    }
                                }
                            }
                            else
                            {
                                while (numbers.Count < adsToBeShown)
                                {
                                    num = generator.Next(numAdvertsShown, numAdverts);
                                    if (!numbers.Contains(num))
                                    {
                                        numbers.Add(num);
                                    }
                                }

                                foreach (int number in numbers)
                                {
                                    advertisements.Add(GetShowReadyAdvert(adverts[number], Server));
                                }
                            }
                        }
                        else
                        {
                            while (numbers.Count < adsToBeShown)
                            {
                                num = generator.Next(numAdvertsShown, numAdverts);
                                if (!numbers.Contains(num))
                                {
                                    numbers.Add(num);
                                }
                            }

                            foreach (int number in numbers)
                            {
                                advertisements.Add(GetShowReadyAdvert(adverts[number], Server));
                            }
                        }
                    }
                    else
                    {
                        while (numbers.Count < adsOnPage)
                        {
                            num = generator.Next(numAdverts);
                            if (!numbers.Contains(num))
                            {
                                numbers.Add(num);
                            }
                        }

                        foreach (int number in numbers)
                        {
                            advertisements.Add(GetShowReadyAdvert(adverts[number], Server));
                        }
                    }

                }

                if (advertisements.Count > 0)
                {
                    advertisements = CommonCode.UiTools.ShuffleList(advertisements);
                    foreach (Control advert in advertisements)
                    {
                        phAds.Controls.Add(advert);
                    }
                }
            }

            return phAds;
        }

        /// <summary>
        /// Returns Advertisement as control ready to be showed
        /// </summary>
        private static PlaceHolder GetShowReadyAdvert(Advertisement advertisement, HttpServerUtility Server)
        {

            UiTools.ChangeUiCultureFromSession();

            if (advertisement == null)
            {
                throw new UIException("advertisement is null");
            }

            if (Server == null)
            {
                throw new UIException("Server is null");
            }

            PlaceHolder phAdvert = new PlaceHolder();

            Table adTable = new Table();

            TableRow adRow = new TableRow();
            adTable.Rows.Add(adRow);

            TableCell lCell = new TableCell();
            adRow.Cells.Add(lCell);
            lCell.Width = Unit.Percentage(49);
            lCell.Controls.Add(UiTools.GetHorisontalLineControl());

            TableCell cCell = new TableCell();
            cCell.Text = HttpContext.GetGlobalResourceObject("UiTools", "Advertisement").ToString();
            cCell.CssClass = "advertText";
            adRow.Cells.Add(cCell);

            TableCell rCell = new TableCell();
            adRow.Cells.Add(rCell);
            rCell.Controls.Add(UiTools.GetHorisontalLineControl());
            rCell.Width = Unit.Percentage(49);

            phAdvert.Controls.Add(adTable);

            Panel adPnl = new Panel();
            phAdvert.Controls.Add(adPnl);
            adPnl.HorizontalAlign = HorizontalAlign.Center;

            if (string.IsNullOrEmpty(advertisement.html))
            {
                if (advertisement.targetUrl != null && advertisement.adurl != null)
                {
                    UriBuilder uBuilder = new UriBuilder();
                    uBuilder.Path = advertisement.targetUrl;

                    // Alternative approach that also seems to work
                    string fullUrl = advertisement.targetUrl;
                    if ((fullUrl.StartsWith("http") == false)
                        && (fullUrl.StartsWith("#") == false)  // Link to the same page
                        )
                    {
                        fullUrl = string.Format("http://{0}", fullUrl);
                    }

                    HyperLink adLink = new HyperLink();
                    adLink.CssClass = "margins";
                    adLink.NavigateUrl = fullUrl;
                    adLink.Target = "_blank";
                    adPnl.Controls.Add(adLink);

                    if (advertisement.adurl.EndsWith(".swf"))
                    {
                        adLink.Controls.Add(UiTools.GetSwfHtmlCode(advertisement.adurl));                // link not working
                    }
                    else
                    {
                        adLink.ImageUrl = advertisement.adurl;
                    }

                }
            }
            else
            {
                System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                div.InnerHtml = Server.HtmlDecode(advertisement.html);
                adPnl.Controls.Add(div);
            }

            return phAdvert;
        }


        /// <summary>
        /// Returns the number of advertisements in final placeholder
        /// </summary>
        public static int getAdvertisementsNumber(PlaceHolder phAdvert)
        {
            if (phAdvert == null)
            {
                throw new CommonCode.UIException("phAdvert is null");
            }

            int number = 0;

            if (phAdvert.Controls[0] != null && phAdvert.Controls.Count > 0)
            {
                if (phAdvert.Controls[0].Controls != null && phAdvert.Controls[0].Controls.Count > 0)
                {
                    number = phAdvert.Controls[0].Controls.Count;
                }
            }

            return number;
        }

        /// <summary>
        /// To be only used by ProcessThumbnailAbort().
        /// </summary>
        private bool ThumbnailAborted { get; set; }

        /// <summary>
        /// Loads Image and returns it as Filebytes
        /// </summary>
        public static byte[] LoadImageFileAsBytes(string appPath, string url)
        {
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }
            if (string.IsNullOrEmpty(url))
            {
                throw new BusinessException("url is null or empty");
            }

            string imagePathFromRoot = System.IO.Path.Combine(appPath, url);

            if (System.IO.File.Exists(imagePathFromRoot))
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(imagePathFromRoot);

                string fileType = System.IO.Path.GetExtension(url);

                MemoryStream ms = new MemoryStream();

                switch (fileType)
                {
                    case ".bmp":
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".jpg":
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".jpeg":
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    default:
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }

                return ms.ToArray();
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Returns resized image from filebytes
        /// </summary>
        public byte[] GetResizedImgFromBytes(byte[] fileBytes, int width, int height, bool keepRatio, string filetype)
        {
            byte[] thumbnailBytes = ResizeFromBytes(width, height, fileBytes, keepRatio, filetype);

            if (thumbnailBytes == null)
            {
                string msg = string.Format("Could not create a thumbnail for image from bytes ");
                throw new InvalidOperationException(msg);
            }
            return thumbnailBytes;
        }


        /// <summary>
        /// Resizes image to wanted width or height.
        /// </summary>
        private byte[] ResizeFromBytes(int thumbWidth, int thumbHeight, byte[] filebytes, bool keepRatio, string filetype)
        {
            byte[] thumbnailBytes = null;

            using (System.IO.Stream strm = new System.IO.MemoryStream(filebytes, false))
            {
                using (System.Drawing.Image img = System.Drawing.Image.FromStream(strm, true, true))
                {
                    if (img.Width > 0)
                    {
                        int height = thumbHeight;
                        int width = thumbWidth;

                        if (keepRatio == true)
                        {
                            ResizeImage(img.Height, img.Width, thumbHeight, thumbWidth, out height, out width);
                        }

                        int buffSize = width * height * 4;  // * 4 - for 32-bit color

                        ThumbnailAborted = false;
                        using (System.Drawing.Image thumbnail = img.GetThumbnailImage(width, height, ProcessThumbnailAbort, IntPtr.Zero))
                        {
                            if (ThumbnailAborted == false)
                            {
                                string tempFileName = Path.GetTempFileName();
                                using (FileStream tempFileStream = File.Create(tempFileName, buffSize, FileOptions.DeleteOnClose))
                                {
                                    try
                                    {

                                        filetype = filetype.ToLower();

                                        switch (filetype)
                                        {
                                            case ".bmp":
                                                thumbnail.Save(tempFileStream, System.Drawing.Imaging.ImageFormat.Bmp);
                                                break;
                                            case ".jpg":
                                                // Necessary because there is quality loss on .JPG when resizing
                                                SaveThumbnail(filebytes, width, height, string.Empty, tempFileStream);

                                                break;
                                            case ".jpeg":
                                                thumbnail.Save(tempFileStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                                break;
                                            default:
                                                thumbnail.Save(tempFileStream, System.Drawing.Imaging.ImageFormat.Png);
                                                break;
                                        }

                                        long streamSize = tempFileStream.Position;
                                        if (streamSize <= int.MaxValue)
                                        {
                                            int thumbnailBytesCount = (int)streamSize;

                                            thumbnailBytes = new byte[thumbnailBytesCount];
                                            tempFileStream.Seek(0, SeekOrigin.Begin);
                                            if (tempFileStream.Read(thumbnailBytes, 0, thumbnailBytesCount) != thumbnailBytesCount)
                                            {
                                                throw new InvalidOperationException("Unsuccessful read from temporary file.");
                                            }
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException("Temporary file too big.");
                                        }
                                    }
                                    finally
                                    {
                                        tempFileStream.Close();
                                    }
                                }
                            }
                            else
                            {
                                string msg = "Thumbnail couldn`t be created."; 
                                throw new BusinessException(msg);
                            }
                        }
                    }
                }
            }

            if (thumbnailBytes == null)
            {
                string msg = string.Format("Could not create a thumbnail of the image.");
                throw new BusinessException(msg);
            }

            return thumbnailBytes;
        }

        /// <summary>
        /// used by ResizeFromBytes
        /// </summary>
        private bool ProcessThumbnailAbort()
        {
            ThumbnailAborted = true;
            return false;
        }

        /// <summary>
        /// Resizes and keeps proportions of Image Height and Width and returns wanted width and height
        /// </summary>
        public static void ResizeImage(int imgHeight, int imgWidth, int wantedHeight, int wantedWidth, out int height, out int width)
        {
            if (wantedHeight < 10)
            {
                throw new BusinessException("wantedHeight is < 10 ");
            }
            if (wantedWidth < 10)
            {
                throw new BusinessException("wantedWidth is < 10 ");
            }
            if (imgHeight < 10)
            {
                throw new BusinessException("imgHeight is < 10 ");
            }
            if (imgWidth < 10)
            {
                throw new BusinessException("imgWidth is < 10 ");
            }
            height = 0;
            width = 0;

            if (imgWidth > wantedWidth || imgHeight > wantedHeight)
            {
                double newHeight = (1.0 * wantedWidth * imgHeight) / imgWidth;
                double newWidth = (1.0 * imgWidth * wantedHeight) / imgHeight;

                if (newHeight <= wantedHeight)
                {
                    height = (int)newHeight;
                    width = wantedWidth;
                }
                else if (newWidth <= wantedWidth)
                {
                    width = (int)newWidth;
                    height = wantedHeight;
                }
                else
                {
                    String error = string.Format("newHeight < wantedHeight && newWidth < wantedWidth : {0}"
                       + " < {1} && {2} < {3}", newHeight, wantedHeight, newWidth, wantedWidth);
                    throw new BusinessException(error);
                }
            }
            else
            {
                height = imgHeight;
                width = imgWidth;
            }

        }
    }
}
