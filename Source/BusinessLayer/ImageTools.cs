﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using DataAccess;
using log4net;

namespace BusinessLayer
{
    public class ImageTools
    {
        private static ILog log = LogManager.GetLogger(typeof(ImageTools));

        static ImageTools()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// Checks if Image File is Valid (length is between 1kb and 2mb and image type is correct)
        /// </summary>
        /// <returns>true if its valid,otherwise false</returns>
        public static bool IsValidImage(string fileName, byte[] fileBytes, ref string imageErrorDescription, bool logo)
        {
            bool imageOk = false;

            if (fileBytes != null)
            {
                int maxFileLength = 2097152;  // 2MB
                int minFileLength = 1024; // 1kb

                if (minFileLength < fileBytes.Length && fileBytes.Length <= maxFileLength)
                {
                    if (IsImage(fileName, fileBytes, out imageErrorDescription) == true)
                    {
                        int minWidth = Configuration.ImagesMinImageWidth;
                        int minHeight = Configuration.ImagesMinImageHeight;

                        if (logo == true)
                        {
                            minWidth = Configuration.ImagesMinCompLogoWidth;
                            minHeight = Configuration.ImagesMinCompLogoHeight;
                        }

                        if (AreImageDimensionsValid(fileBytes, minWidth, minHeight))
                        {
                            imageOk = true;
                        }
                        else
                        {
                            imageErrorDescription = string.Format("{0} {1}/{2}.", Tools.GetResource("errImgWidthHeight"), minWidth, minHeight);
                        }
                    }
                    else
                    {
                        imageErrorDescription =
                            string.IsNullOrEmpty(imageErrorDescription) ? Tools.GetResource("errImgTypeNotSupported") : imageErrorDescription;
                    }
                }
                else
                {
                    imageErrorDescription = string.Format("{0} {1}.", Tools.GetResource("errImgFileSize")
                        , fileBytes.Length > maxFileLength ? Tools.GetResource("big") : Tools.GetResource("small"));
                }
            }
            else
            {
                imageErrorDescription = Tools.GetResource("errImgUploading");
            }
            return imageOk;
        }

        /// <summary>
        /// True if image dimensions are not below wanted, otherwise false
        /// </summary>
        public static bool AreImageDimensionsValid(byte[] fileContents, int minWidth, int minHeight)
        {
            bool valid = true;

            int width = 0;
            int height = 0;
            if (GetImageWidthAndHeight(fileContents, out width, out height))
            {
                if (width < minWidth || height < minHeight)
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }

            return valid;
        }

        /// <summary>
        /// Checks if image type is correct
        /// </summary>
        /// <returns>true if its correct , otherwise false</returns>
        public static bool IsImage(string fileName, byte[] fileContents, out string errorMessage)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            if (fileName == string.Empty)
            {
                throw new ArgumentNullException("fileName is empty.");
            }
            if (fileContents == null)
            {
                throw new ArgumentNullException("fileContents");
            }
            if (fileContents.Length == 0)
            {
                throw new ArgumentNullException("fileContents is empty.");
            }

            errorMessage = string.Empty;
            bool result = false;
            try
            {
                string fileType = System.IO.Path.GetExtension(fileName);

                fileType = (fileType ?? string.Empty).ToUpper();
                switch (fileType)
                {
                    case ".JPG":
                        result = IsJpg(fileContents);
                        if (result == false)
                        {
                            errorMessage = string.Format("{0} \"{1}\" file.", Tools.GetResource("errImgNotValidType")
                                , fileType, Tools.GetResource("file"));
                        }
                        break;
                    case ".PNG":
                        result = IsPng(fileContents);
                        if (result == false)
                        {
                            errorMessage = string.Format("{0} \"{1}\" file.", Tools.GetResource("errImgNotValidType")
                                , fileType, Tools.GetResource("file"));
                        }
                        break;
                    case ".BMP":
                        result = IsBmp(fileContents);
                        if (result == false)
                        {
                            errorMessage = string.Format("{0} \"{1}\" file.", Tools.GetResource("errImgNotValidType")
                                , fileType, Tools.GetResource("file"));
                        }
                        break;
                    default:
                        errorMessage = string.Format(
                            "{0} \"{1}\" {2}.", Tools.GetResource("errImgFileType"),
                            fileType, Tools.GetResource("errImgFileType2"));
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException("Cound not determine whether the file is an image", ex);
            }
            return result;
        }

        /// <summary>
        /// Returns image`s width and height and checks if they are more than 0
        /// </summary>
        /// <returns>true if width and heigh are more than 0</returns>
        public static Boolean GetImageWidthAndHeight(byte[] fileContents, out int width, out int height)
        {
            width = 0;
            height = 0;
            Boolean result = false;

            try
            {
                // if fails, means that the image file is corrupted

                using (System.IO.Stream strm = new System.IO.MemoryStream(fileContents, false))
                {
                    System.Drawing.Image img = System.Drawing.Image.FromStream(strm, true, true);
                    height = img.Height;
                    width = img.Width;
                }
            }
            catch { }


            if (width > 0 && height > 0)
            {
                result = true;
            }
            return result;
        }

        private static bool IsJpg(byte[] fileContents)
        {
            if (fileContents == null)
            {
                throw new ArgumentNullException("fileContents");
            }
            if (fileContents.Length == 0)
            {
                throw new ArgumentNullException("fileContents is empty.");
            }

            byte[] jpgStart = new byte[] { 0xFF, 0xD8, 0xFF };
            bool isJpg = ByteArrayStartsWith(fileContents, jpgStart);
            return isJpg;
        }

        private static bool IsBmp(byte[] fileContents)
        {
            if (fileContents == null)
            {
                throw new ArgumentNullException("fileContents");
            }
            if (fileContents.Length == 0)
            {
                throw new ArgumentNullException("fileContents is empty.");
            }

            byte[] jpgStart = new byte[] { 0x42, 0x4D };
            bool isJpg = ByteArrayStartsWith(fileContents, jpgStart);
            return isJpg;
        }


        private static bool IsPng(byte[] fileContents)
        {
            if (fileContents == null)
            {
                throw new ArgumentNullException("fileContents");
            }
            if (fileContents.Length == 0)
            {
                throw new ArgumentNullException("fileContents is empty.");
            }

            byte[] pngStart = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
            bool isPng = ByteArrayStartsWith(fileContents, pngStart);
            return isPng;
        }

        public static bool IsGif(byte[] fileContents)
        {
            if (fileContents == null)
            {
                throw new ArgumentNullException("fileContents");
            }
            if (fileContents.Length == 0)
            {
                throw new ArgumentNullException("fileContents is empty.");
            }

            byte[] pngStart = new byte[] { 0x47, 0x49, 0x46 };
            bool isPng = ByteArrayStartsWith(fileContents, pngStart);
            return isPng;
        }

        public static bool IsSwf(byte[] fileContents)
        {
            if (fileContents == null)
            {
                throw new ArgumentNullException("fileContents");
            }
            if (fileContents.Length == 0)
            {
                throw new ArgumentNullException("fileContents is empty.");
            }

            byte[] pngStart = new byte[] { 0x43, 0x57, 0x53, 0x0A };
            bool isPng = ByteArrayStartsWith(fileContents, pngStart);
            return isPng;
        }


        private static bool ByteArrayStartsWith(byte[] byteArray, byte[] controlArray)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
            }
            if (controlArray == null)
            {
                throw new ArgumentNullException("controlArray");
            }
            if (controlArray.Length == 0)
            {
                throw new ArgumentNullException("controlArray is empty.");
            }

            bool beginningMatches = false;

            if (byteArray.Length > controlArray.Length)
            {
                bool difference = false;
                for (int i = 0; difference == false && i < controlArray.Length; i++)
                {
                    byte fileBute = byteArray[i];
                    byte controlByte = controlArray[i];
                    if (fileBute != controlByte)
                    {
                        difference = true;
                    }
                }
                if (difference == false)
                {
                    beginningMatches = true;
                }
            }
            return beginningMatches;
        }

        /// <summary>
        /// Add`s ProductImage to database
        /// </summary>
        public void AddProductImage(EntitiesUsers userContext, Entities objectContext, ProductImage image, BusinessLog bLog, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (image == null)
            {
                throw new BusinessException("image is null.");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null.");
            }

            objectContext.AddToProductImageSet(image);
            Tools.Save(objectContext);

            bLog.LogProductImage(objectContext, image, LogType.create, currUser);

            if (!image.ProductReference.IsLoaded)
            {
                image.ProductReference.Load();
            }
            BusinessProduct bProduct = new BusinessProduct();
            bProduct.UpdateLastModifiedAndModifiedBy(objectContext, image.Product, Tools.GetUserID(objectContext, currUser));

            BusinessStatistics stat = new BusinessStatistics();
            stat.PictureUploaded(userContext, objectContext);
        }

        /// <summary>
        /// Add`s CompanyImage to database
        /// </summary>
        public void AddCompanyImage(EntitiesUsers userContext, Entities objectContext, CompanyImage image, BusinessLog bLog, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (image == null)
            {
                throw new BusinessException("image is null.");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null.");
            }

            objectContext.AddToCompanyImageSet(image);
            Tools.Save(objectContext);
            bLog.LogCompanyImage(objectContext, image, LogType.create, currUser);

            if (!image.CompanyReference.IsLoaded)
            {
                image.CompanyReference.Load();
            }
            BusinessCompany bCompany = new BusinessCompany();
            bCompany.UpdateLastModifiedAndModifiedBy(objectContext, image.Company, Tools.GetUserID(objectContext, currUser));

            BusinessStatistics stat = new BusinessStatistics();
            stat.PictureUploaded(userContext, objectContext);
        }

        /// <summary>
        /// Deletes (permanently from database and harddrive) ProductImage
        /// </summary>
        public bool DeleteProductImage(EntitiesUsers userContext, Entities objectContext, ProductImage image, BusinessLog bLog, string appPath, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (image == null)
            {
                throw new BusinessException("image is null.");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null.");
            }
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            if (image.isThumbnail)
            {
                ProductImage mainIMG = GetProductImage(objectContext, image.mainImgID.Value);
                if (mainIMG != null)
                {
                    if (DeleteProductImage(userContext, objectContext, mainIMG, bLog, appPath, currUser) == false)
                    {
                        return false;
                    }
                }
            }

            string imagePath = System.IO.Path.Combine(appPath, image.url);

            if (log.IsInfoEnabled == true)
            {
                log.InfoFormat("Deleting product image with ID = {0}, URL = \"{1}\".",
                    image.ID, image.url);
            }

            if (!image.ProductReference.IsLoaded)
            {
                image.ProductReference.Load();
            }

            bool imageDeleted = DeleteImageFromHD(imagePath);

            if (imageDeleted == true)
            {
                BusinessProduct bProduct = new BusinessProduct();
                bProduct.UpdateLastModifiedAndModifiedBy(objectContext, image.Product, Tools.GetUserID(objectContext, currUser));

                bLog.LogProductImage(objectContext, image, LogType.delete, currUser);

                objectContext.DeleteObject(image);
                Tools.Save(objectContext);

                BusinessStatistics stat = new BusinessStatistics();
                stat.PictureDeleted(userContext, objectContext);
            }

            return imageDeleted;
        }


        public bool DeleteCategoryImage(EntitiesUsers userContext, Entities objectContext
            , Category currCategory, BusinessLog bLog, string appPath, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null.");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null.");
            }
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }
            if (string.IsNullOrEmpty(currCategory.imageUrl))
            {
                throw new BusinessException("imageUrl is null or empty");
            }

            string imagePath = System.IO.Path.Combine(appPath, currCategory.imageUrl);

            if (log.IsInfoEnabled == true)
            {
                log.InfoFormat("Deleting category ID = {0} image , URL = \"{1}\".",
                    currCategory.ID, currCategory.imageUrl);
            }

            bool imageDeleted = DeleteImageFromHD(imagePath);

            if (imageDeleted == true)
            {
                bLog.LogCategoryImage(objectContext, currCategory, LogType.delete, currUser);

                currCategory.imageHeight = null;
                currCategory.imageWidth = null;
                currCategory.imageUrl = null;
                Tools.Save(objectContext);

                BusinessStatistics stat = new BusinessStatistics();
                stat.PictureDeleted(userContext, objectContext);
            }

            return imageDeleted;
        }

        /// <summary>
        /// Deletes category image from Database and Hard disk, used when category image(file) is not found when loading category page
        /// </summary>
        public bool DeleteCategoryImageFromDB(EntitiesUsers userContext, Entities objectContext
            , Category currCategory, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null.");
            }
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }
            if (string.IsNullOrEmpty(currCategory.imageUrl))
            {
                throw new BusinessException("imageUrl is null or empty");
            }

            string imagePath = System.IO.Path.Combine(appPath, currCategory.imageUrl);

            if (log.IsInfoEnabled == true)
            {
                log.InfoFormat("Deleting category ID = {0} image , URL = \"{1}\".",
                    currCategory.ID, currCategory.imageUrl);
            }

            bool imageDeleted = DeleteImageFromHD(imagePath);

            if (imageDeleted == true)
            {
                currCategory.imageHeight = null;
                currCategory.imageWidth = null;
                currCategory.imageUrl = null;
                Tools.Save(objectContext);

                BusinessStatistics stat = new BusinessStatistics();
                stat.PictureDeleted(userContext, objectContext);
            }

            return imageDeleted;
        }

        /// <summary>
        /// Deletes (permanently from database and harddrive) CompanyImage
        /// </summary>
        public bool DeleteCompanyImage(EntitiesUsers userContext, Entities objectContext
            , CompanyImage image, BusinessLog bLog, string appPath, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (image == null)
            {
                throw new BusinessException("image is null.");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null.");
            }
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            if (image.isThumbnail)
            {
                CompanyImage mainIMG = GetCompanyImage(objectContext, image.mainImgID.Value);
                if (mainIMG != null)
                {
                    if (DeleteCompanyImage(userContext, objectContext, mainIMG, bLog, appPath, currUser) == false)
                    {
                        return false;
                    }
                }
            }

            string completePath = System.IO.Path.Combine(appPath, image.url);

            if (log.IsInfoEnabled == true)
            {
                log.InfoFormat("Deleting company image with ID = {0}, URL = \"{1}\".",
                    image.ID, image.url);
            }

            if (!image.CompanyReference.IsLoaded)
            {
                image.CompanyReference.Load();
            }

            bool imageDeleted = DeleteImageFromHD(completePath);

            if (imageDeleted == true)
            {
                BusinessCompany bCompany = new BusinessCompany();
                bCompany.UpdateLastModifiedAndModifiedBy(objectContext, image.Company, Tools.GetUserID(objectContext, currUser));

                bLog.LogCompanyImage(objectContext, image, LogType.delete, currUser);

                objectContext.DeleteObject(image);
                Tools.Save(objectContext);

                BusinessStatistics stat = new BusinessStatistics();
                stat.PictureDeleted(userContext, objectContext);
            }

            return imageDeleted;

        }

        public static string DeletingImageLogMessage(long imageID, string imageUrl, string becauseOf)
        {
            if (string.IsNullOrEmpty(imageUrl) == true)
            {
                imageUrl = "unspecified URL";
            }
            if (string.IsNullOrEmpty(becauseOf) == true)
            {
                becauseOf = "unspecified reason";
            }
            string logMsg = string.Format("About to delete image with ID = {0}, URL = \"{1}\" because of {2}.",
                imageID, imageUrl, becauseOf);
            return logMsg;
        }

        /// <summary>
        /// Deletes image from harddrive
        /// </summary>
        public bool DeleteImageFromHD(string imgPath)
        {
            if (string.IsNullOrEmpty(imgPath))
            {
                throw new BusinessException("imgPath is null or empty");
            }

            bool fileDeleted = false;

            try
            {
                if (File.Exists(imgPath))
                {
                    File.Delete(imgPath);

                    fileDeleted = true;
                }
                else
                {
                    if (log.IsErrorEnabled == true)
                    {
                        log.Error(string.Format("Couldn`t delete file ' {0} ' , because it doesn`t exists.", imgPath));
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled == true)
                {
                    log.Error(ex.Message);
                }
            }

            return fileDeleted;

        }

        /// <summary>
        /// Changes ProductImages`s image pathg and url
        /// </summary>
        public void ChangeProductImageUrl(Entities objectContext, ProductImage image, string newUrl)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (image == null)
            {
                throw new BusinessException("image is null.");
            }
            if (string.IsNullOrEmpty(newUrl))
            {
                throw new BusinessException("newUrl is null or empty");
            }

            image.url = newUrl;
            Tools.Save(objectContext);
        }

        /// <summary>
        /// Changes CompanyImages`s image pathg and url
        /// </summary>
        public void ChangeCompanyImageUrl(Entities objectContext, CompanyImage image, string newUrl)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (image == null)
            {
                throw new BusinessException("image is null.");
            }

            if (string.IsNullOrEmpty(newUrl))
            {
                throw new BusinessException("newUrl is null or empty");
            }

            image.url = newUrl;
            Tools.Save(objectContext);
        }

        /// <summary>
        /// Generates new unique ProductImage file name
        /// </summary>
        public String GetProductImageName(Product currProduct, long imageID, string path, string extension)
        {
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }
            if (imageID < 1)
            {
                throw new BusinessException("imageID is < 1");
            }
            if (path == null || path == string.Empty)
            {
                throw new BusinessException("path is null or empty");
            }
            if (extension == null || extension == string.Empty)
            {
                throw new BusinessException("extension is null or empty");
            }

            string fileName = string.Empty;
            System.Text.StringBuilder name = new StringBuilder();
            Boolean generated = false;
            for (int i = 1; i < 100 && !generated; i++)
            {
                name.Length = 0;
                name.Append("p");
                name.Append(currProduct.ID);
                name.Append("i");
                name.Append(imageID);
                name.Append("n");
                name.Append(i);
                name.Append(extension);

                fileName = name.ToString();
                string fullFileName = Path.Combine(path, fileName);
                if (File.Exists(fullFileName) == false)
                {
                    generated = true;
                }
            }
            if ((generated == false) || (string.IsNullOrEmpty(fileName) == true))
            {
                string errMsg = string.Format(
                    "Cannot generate unique file name for: Product ID = {0}, image ID = {1}, path = \"{2}\", extension = \"{3}\".",
                    currProduct.ID, imageID, path, extension);
                throw new BusinessException(errMsg);
            }
            return fileName;
        }

        /// <summary>
        /// Generates new unique CompanyImage file name
        /// </summary>
        public String GetCompanyImageName(Company currCompany, long imageID, string path, string extension)
        {
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (imageID < 1)
            {
                throw new BusinessException("imageID is < 1");
            }
            if (path == null || path == string.Empty)
            {
                throw new BusinessException("path is null or empty");
            }
            if (extension == null || extension == string.Empty)
            {
                throw new BusinessException("extension is null or empty");
            }

            string fileName = string.Empty;
            System.Text.StringBuilder name = new StringBuilder();
            Boolean generated = false;
            for (int i = 1; i < 100 && !generated; i++)
            {
                name.Length = 0;
                name.Append("c");
                name.Append(currCompany.ID);
                name.Append("i");
                name.Append(imageID);
                name.Append("n");
                name.Append(i);
                name.Append(extension);

                fileName = name.ToString();
                string fullFileName = Path.Combine(path, fileName);
                if (File.Exists(fullFileName) == false)
                {
                    generated = true;
                }
            }
            if ((generated == false) || (string.IsNullOrEmpty(fileName) == true))
            {
                string errMsg = string.Format(
                    "Cannot generate unique file name for: Company ID = {0}, image ID = {1}, path = \"{2}\", extension = \"{3}\".",
                    currCompany.ID, imageID, path, extension);
                throw new BusinessException(errMsg);
            }
            return fileName;
        }


        /// <summary>
        /// Generates unique name for CategoryImage
        /// </summary>
        public String GetCategoryImageName(Category currCategory, string path, string extension)
        {
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }
            if (path == null || path == string.Empty)
            {
                throw new BusinessException("path is null or empty");
            }
            if (extension == null || extension == string.Empty)
            {
                throw new BusinessException("extension is null or empty");
            }

            string fileName = string.Empty;
            System.Text.StringBuilder name = new StringBuilder();
            Boolean generated = false;
            for (int i = 1; i < 100 && !generated; i++)
            {
                name.Length = 0;
                name.Append("cat");
                name.Append(currCategory.ID);
                name.Append("imgNum");
                name.Append(i);
                name.Append(extension);

                fileName = name.ToString();
                string fullFileName = Path.Combine(path, fileName);
                if (File.Exists(fullFileName) == false)
                {
                    generated = true;
                }
            }
            if ((generated == false) || (string.IsNullOrEmpty(fileName) == true))
            {
                string errMsg = string.Format(
                    "Cannot generate unique file name for: Category ID = {0}, path = \"{1}\", extension = \"{2}\".",
                    currCategory.ID, path, extension);
                throw new BusinessException(errMsg);
            }
            return fileName;
        }

        /// <summary>
        /// Checks if Image file exists from ProductImage , if it doesnt then deletes the ProductImage row from database
        /// </summary>
        /// <returns>true if it exists, false oterwise</returns>
        private Boolean CheckIfImageProductExists(Entities objectContext, ProductImage image, BusinessLog bLog, Boolean deleteIfFalse, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            if (image == null)
            {
                throw new BusinessException("image is null");
            }

            Boolean result = false;

            string path = System.IO.Path.Combine(appPath, image.url);

            if (image.url != string.Empty)
            {
                if (File.Exists(path))
                {
                    result = true;
                }
                else if (deleteIfFalse)
                {
                    if (log.IsInfoEnabled == true)
                    {
                        string becauseOf = string.Format("invalid physical path (\"{0}\")", path);
                        string logMsg = ImageTools.DeletingImageLogMessage(image.ID, image.url, becauseOf);
                        log.Info(logMsg);
                    }
                    DeleteProductImageFromDB(objectContext, image, bLog);
                }
            }
            else
            {
                TimeSpan minTimeAfterCreation = TimeSpan.FromMinutes(5.0);
                TimeSpan timeAfterCreation = DateTime.UtcNow - image.dateCreated;
                if (timeAfterCreation > minTimeAfterCreation && deleteIfFalse)
                {
                    if (log.IsInfoEnabled == true)
                    {
                        string logMsg = ImageTools.DeletingImageLogMessage(image.ID, image.url, "no path");
                        log.Info(logMsg);
                    }
                    DeleteProductImageFromDB(objectContext, image, bLog);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if Image file exists from CompanyImage , if it doesnt then deletes the CompanyImage row from database
        /// </summary>
        /// <returns>true if it exists, false oterwise</returns>
        private Boolean CheckIfImageCompanyExists(Entities objectContext, CompanyImage image, BusinessLog bLog,
            Boolean deleteIfFalse, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (image == null)
            {
                throw new BusinessException("image is null");
            }
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            Boolean result = false;

            string completePath = System.IO.Path.Combine(appPath, image.url);

            if (image.url != string.Empty)
            {
                if (File.Exists(completePath))
                {
                    result = true;
                }
                else if (deleteIfFalse)
                {
                    if (log.IsInfoEnabled == true)
                    {
                        string becauseOf = string.Format("invalid physical path (\"{0}\")", completePath);
                        string logMsg = ImageTools.DeletingImageLogMessage(image.ID, image.url, becauseOf);
                        log.Info(logMsg);
                    }
                    DeleteCompanyImageFromDB(objectContext, image, bLog);
                }
            }
            else
            {
                TimeSpan minTimeAfterCreation = TimeSpan.FromMinutes(5.0);
                TimeSpan timeAfterCreation = DateTime.UtcNow - image.dateCreated;
                if (timeAfterCreation > minTimeAfterCreation && deleteIfFalse)
                {
                    if (log.IsInfoEnabled == true)
                    {
                        string logMsg = ImageTools.DeletingImageLogMessage(image.ID, image.url, "no url");
                        log.Info(logMsg);
                    }
                    DeleteCompanyImageFromDB(objectContext, image, bLog);
                }
            }

            return result;
        }

        public Boolean CheckIfCategoryImageExists(EntitiesUsers userContext, Entities objectContext
            , Category category, Boolean deleteIfFalse, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (category == null)
            {
                throw new BusinessException("category is null");
            }
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            Boolean result = false;

            string completePath = System.IO.Path.Combine(appPath, category.imageUrl);

            if (category.imageUrl != string.Empty)
            {
                if (File.Exists(completePath))
                {
                    result = true;
                }
                else if (deleteIfFalse)
                {
                    if (log.IsInfoEnabled == true)
                    {
                        string becauseOf = string.Format("invalid category image physical path (\"{0}\")", completePath);
                        string logMsg = ImageTools.DeletingImageLogMessage(category.ID, category.imageUrl, becauseOf);
                        log.Info(logMsg);
                    }

                    DeleteCategoryImageFromDB(userContext, objectContext, category, appPath);
                }
            }
            else
            {
                if (category.imageHeight != null)
                {
                    category.imageHeight = null;
                    Tools.Save(objectContext);
                }
                if (category.imageWidth != null)
                {
                    category.imageWidth = null;
                    Tools.Save(objectContext);
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes ProductImage from database
        /// </summary>
        private void DeleteProductImageFromDB(Entities objectContext, ProductImage image, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (image == null)
            {
                throw new BusinessException("image is null");
            }

            if (log.IsInfoEnabled == true)
            {
                log.InfoFormat("Deleting from DB product image with ID = {0}, URL = \"{1}\".",
                    image.ID, image.url);
            }
            bLog.SystemDeleteProductImageLog(objectContext, image);
            objectContext.DeleteObject(image);
            Tools.Save(objectContext);

        }

        /// <summary>
        /// Deletes CompanyImage from database
        /// </summary>
        private void DeleteCompanyImageFromDB(Entities objectContext, CompanyImage image, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (image == null)
            {
                throw new BusinessException("image is null");
            }

            if (log.IsInfoEnabled == true)
            {
                log.InfoFormat("Deleting from DB company image with ID = {0}, URL = \"{1}\".",
                    image.ID, image.url);
            }
            bLog.SystemDeleteCompanyImageLog(objectContext, image);
            objectContext.DeleteObject(image);
            Tools.Save(objectContext);
        }

        /// <summary>
        /// Checks if Product Have main image
        /// </summary>
        /// <returns>true if it haves,otherwise false</returns>
        public Boolean IfProductHaveMainImg(Entities objectContext, long currProdId, BusinessLog bLog, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            if (currProdId < 1)
            {
                throw new BusinessException("currProdId is < 1");
            }

            Boolean result = false;

            ProductImage img = objectContext.ProductImageSet.FirstOrDefault
                (image => image.Product.ID == currProdId && image.main == true);

            if (img != null)
            {
                if (CheckIfImageProductExists(objectContext, img, bLog, true, appPath))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Checks if Comapny Have logo
        /// </summary>
        /// <returns>true if it haves,otherwise false</returns>
        public Boolean IfCompanyHaveLogo(Entities objectContext, Company currCompany, BusinessLog bLog, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            Boolean result = false;

            CompanyImage img = GetCompanyLogo(objectContext, currCompany, bLog, appPath);

            if (img != null)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Returns Product Main image if there is , otherwise first found product image
        /// </summary>
        public ProductImage GetProductMainImage(Entities objectContext, Product currProduct, BusinessLog bLog, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            ProductImage retImage = null;

            if (IfProductHaveMainImg(objectContext, currProduct.ID, bLog, appPath))
            {
                ProductImage img = objectContext.ProductImageSet.FirstOrDefault
                (image => image.Product.ID == currProduct.ID && image.main == true);
                if (img == null)
                {
                    throw new BusinessException(string.Format("main img is null of product id = {0} , there is check for that earlier.", currProduct.ID));
                }
                retImage = img;
            }
            else
            {
                ProductImage image = GetProductFirstImage(objectContext, currProduct, bLog, appPath);
                if (image != null)
                {
                    retImage = image;
                }
            }

            return retImage;
        }

        /// <summary>
        /// Returns Company Logo if there is , otherwise null
        /// </summary>
        public CompanyImage GetCompanyLogo(Entities objectContext, Company currCompany, BusinessLog bLog, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            CompanyImage retImage = null;

            CompanyImage Logo = objectContext.CompanyImageSet.FirstOrDefault
                (img => img.Company.ID == currCompany.ID && img.isLogo == true && img.isThumbnail == false);
            if (Logo != null)
            {
                if (CheckIfImageCompanyExists(objectContext, Logo, bLog, true, appPath))
                {
                    retImage = Logo;
                }
            }

            return retImage;
        }

        /// <summary>
        /// Returns Product first found image
        /// </summary>
        private ProductImage GetProductFirstImage(Entities objectContext, Product currProduct, BusinessLog bLog, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            IEnumerable<ProductImage> Images = objectContext.ProductImageSet.Where
                (img => img.Product.ID == currProduct.ID);

            ProductImage retImage = null;

            if (Images.Count<ProductImage>() > 0)
            {
                IList<ProductImage> imagesList = new List<ProductImage>();

                foreach (ProductImage image in Images)
                {
                    imagesList.Add(image);
                }

                foreach (ProductImage img in imagesList)
                {
                    if (CheckIfImageProductExists(objectContext, img, bLog, true, appPath))
                    {
                        retImage = img;
                        break;
                    }
                }
            }

            return retImage;
        }

        /// <summary>
        /// Returns ProductImage with id
        /// </summary>
        public ProductImage GetProductImage(Entities objectContext, long imgID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (imgID < 1)
            {
                throw new BusinessException("imgID is < 1");
            }

            ProductImage image = objectContext.ProductImageSet.FirstOrDefault(img => img.ID == imgID);
            return image;
        }

        /// <summary>
        /// Returns CompanyImage with id
        /// </summary>
        public CompanyImage GetCompanyImage(Entities objectContext, long imgID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (imgID < 1)
            {
                throw new BusinessException("imgID is < 1");
            }

            CompanyImage image = objectContext.CompanyImageSet.FirstOrDefault(img => img.ID == imgID);
            return image;
        }

        /// <summary>
        /// Returns Product`s Thumbanials
        /// </summary>
        public List<ProductImage> GetProductThumbnails(EntitiesUsers userContext, Entities objectContext
            , Product currProduct, BusinessLog bLog, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            IEnumerable<ProductImage> allThumbnails = objectContext.ProductImageSet.Where
                (img => img.Product.ID == currProduct.ID && img.isThumbnail == true);

            List<ProductImage> thumbList = new List<ProductImage>();

            User system = Tools.GetSystem();

            if (allThumbnails.Count<ProductImage>() > 0)
            {

                List<ProductImage> Thumbnails = new List<ProductImage>();
                foreach (ProductImage thumb in allThumbnails)
                {
                    Thumbnails.Add(thumb);
                }

                foreach (ProductImage thumbnail in Thumbnails)
                {
                    ProductImage mainImage = GetProductImage(objectContext, thumbnail.mainImgID.Value);
                    if (mainImage != null)
                    {
                        if (CheckIfImageProductExists(objectContext, thumbnail, bLog, false, appPath))
                        {
                            if (CheckIfImageProductExists(objectContext, mainImage, bLog, true, appPath))
                            {
                                thumbList.Add(thumbnail);
                            }
                            else
                            {
                                if (log.IsInfoEnabled == true)
                                {
                                    string logMsg = ImageTools.DeletingImageLogMessage(thumbnail.ID, thumbnail.url, "no product");
                                    log.Info(logMsg);
                                }

                                DeleteProductImageFromDB(objectContext, thumbnail, bLog);
                            }
                        }
                        else
                        {
                            if (CheckIfImageProductExists(objectContext, mainImage, bLog, true, appPath))
                            {

                                string imgPath = System.IO.Path.Combine(appPath, mainImage.url);

                                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(imgPath))
                                {
                                    using (System.Drawing.Bitmap bmpThum = new System.Drawing.Bitmap(bmp, thumbnail.width, thumbnail.height))
                                    {
                                        string thumbPath = System.IO.Path.Combine(appPath, thumbnail.url);

                                        bmpThum.Save(thumbPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    }
                                }
                                if (CheckIfImageProductExists(objectContext, thumbnail, bLog, false, appPath))
                                {
                                    thumbList.Add(thumbnail);
                                }
                                else
                                {
                                    string error = string.Format("Thumbnail {0} was recreated from main image {1}" +
                                        " and after this it still doesnt exist (CheckIfImageExists function called from GetProductThumbnails)", thumbnail.ID, mainImage.ID);
                                    throw new BusinessException(error);
                                }

                            }
                            else
                            {
                                if (log.IsInfoEnabled == true)
                                {
                                    string logMsg = ImageTools.DeletingImageLogMessage(thumbnail.ID, thumbnail.url, "no product");
                                    log.Info(logMsg);
                                }

                                DeleteProductImageFromDB(objectContext, thumbnail, bLog);
                            }
                        }
                    }
                    else
                    {
                        if (log.IsInfoEnabled == true)
                        {
                            string logMsg = ImageTools.DeletingImageLogMessage(thumbnail.ID, thumbnail.url, "no main image");
                            log.Info(logMsg);
                        }
                        DeleteProductImage(userContext, objectContext, thumbnail, bLog, appPath, system);
                    }
                }
            }

            return thumbList;
        }

        /// <summary>
        /// Return Product main Image Thumbnail if there is , if there isnt main image returns first found product thumbnail.
        /// Doesnt throw exceptions on incorrect data.
        /// </summary>
        public ProductImage GetProductImageThumbnail(Entities objectContext, Product currProduct)
        {
            if (currProduct == null || objectContext == null)
            {
                return null;
            }

            ProductImage mainImg = objectContext.ProductImageSet.FirstOrDefault
                (img => img.Product.ID == currProduct.ID && img.main == true && img.isThumbnail == false);

            ProductImage notMainImg = null;

            if (mainImg != null)
            {
                ProductImage mainThumb = objectContext.ProductImageSet.FirstOrDefault
                    (img => img.mainImgID == mainImg.ID && img.isThumbnail == true);
                if (mainThumb != null)
                {
                    return mainThumb;
                }
                else
                {
                    notMainImg = objectContext.ProductImageSet.FirstOrDefault
                        (img => img.Product.ID == currProduct.ID && img.isThumbnail == true);

                    return notMainImg;
                }
            }
            else
            {
                notMainImg = objectContext.ProductImageSet.FirstOrDefault
                        (img => img.Product.ID == currProduct.ID && img.isThumbnail == true);

                return notMainImg;
            }
        }

        /// <summary>
        /// Return Company main Image Thumbnail if there is , if there isnt main image returns first found company thumbnail.
        /// Doesnt throw exceptions on incorrect data. 
        /// </summary>
        public CompanyImage GetCompanyImageThumbnail(Entities objectContext, Company currCompany)
        {
            if (currCompany == null || objectContext == null)
            {
                return null;
            }

            CompanyImage mainImg = objectContext.CompanyImageSet.FirstOrDefault
                (img => img.Company.ID == currCompany.ID && img.isLogo == true && img.isThumbnail == false);

            CompanyImage notMainImg = null;

            if (mainImg != null)
            {
                CompanyImage mainThumb = objectContext.CompanyImageSet.FirstOrDefault
                    (img => img.mainImgID == mainImg.ID && img.isThumbnail == true);
                if (mainThumb != null)
                {
                    return mainThumb;
                }
                else
                {
                    notMainImg = objectContext.CompanyImageSet.FirstOrDefault
                        (img => img.Company.ID == currCompany.ID && img.isThumbnail == true);
                    return notMainImg;
                }
            }
            else
            {
                notMainImg = objectContext.CompanyImageSet.FirstOrDefault
                        (img => img.Company.ID == currCompany.ID && img.isThumbnail == true);
                return notMainImg;
            }
        }

        /// <summary>
        /// Returns Company`s thumbanils
        /// </summary>
        public List<CompanyImage> GetCompanyThumbnails(EntitiesUsers userContext, Entities objectContext
            , Company currCompany, BusinessLog bLog, string appPath)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (string.IsNullOrEmpty(appPath))
            {
                throw new BusinessException("appPath is null or empty");
            }

            IEnumerable<CompanyImage> allThumbnails = objectContext.CompanyImageSet.Where
                (img => img.Company.ID == currCompany.ID && img.isThumbnail == true);

            List<CompanyImage> thumbList = new List<CompanyImage>();

            User system = Tools.GetSystem();

            if (allThumbnails.Count<CompanyImage>() > 0)
            {

                List<CompanyImage> Thumbnails = new List<CompanyImage>();
                foreach (CompanyImage thumb in allThumbnails)
                {
                    Thumbnails.Add(thumb);
                }

                foreach (CompanyImage thumbnail in Thumbnails)
                {
                    CompanyImage mainImage = GetCompanyImage(objectContext, thumbnail.mainImgID.Value);
                    if (mainImage != null)
                    {
                        if (CheckIfImageCompanyExists(objectContext, thumbnail, bLog, false, appPath))
                        {
                            if (CheckIfImageCompanyExists(objectContext, mainImage, bLog, true, appPath))
                            {
                                thumbList.Add(thumbnail);
                            }
                            else
                            {
                                if (log.IsInfoEnabled == true)
                                {
                                    string logMsg = ImageTools.DeletingImageLogMessage(thumbnail.ID, thumbnail.url, "no company");
                                    log.Info(logMsg);
                                }
                                DeleteCompanyImageFromDB(objectContext, thumbnail, bLog);
                            }
                        }
                        else
                        {
                            if (CheckIfImageCompanyExists(objectContext, mainImage, bLog, true, appPath))
                            {

                                string mainImgPath = System.IO.Path.Combine(appPath, mainImage.url);

                                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(mainImgPath))
                                {
                                    using (System.Drawing.Bitmap bmpThum = new System.Drawing.Bitmap(bmp, thumbnail.width, thumbnail.height))
                                    {
                                        string thumbImgPath = System.IO.Path.Combine(appPath, thumbnail.url);

                                        bmpThum.Save(thumbImgPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    }

                                }
                                if (CheckIfImageCompanyExists(objectContext, thumbnail, bLog, false, appPath))
                                {
                                    thumbList.Add(thumbnail);
                                }
                                else
                                {
                                    string error = string.Format("Thumbnail {0} was recreated from main image {1}" +
                                        " and after this it still doesnt exist (CheckIfImageExists function called from GetProductThumbnails)", thumbnail.ID, mainImage.ID);
                                    throw new BusinessException(error);
                                }

                            }
                            else
                            {
                                if (log.IsInfoEnabled == true)
                                {
                                    string logMsg = ImageTools.DeletingImageLogMessage(thumbnail.ID, thumbnail.url, "no company");
                                    log.Info(logMsg);
                                }

                                DeleteCompanyImageFromDB(objectContext, thumbnail, bLog);
                            }
                        }
                    }
                    else
                    {
                        if (log.IsInfoEnabled == true)
                        {
                            string logMsg = ImageTools.DeletingImageLogMessage(thumbnail.ID, thumbnail.url, "no main image");
                            log.Info(logMsg);
                        }
                        DeleteCompanyImage(userContext, objectContext, thumbnail, bLog, appPath, system);
                    }
                }
            }

            return thumbList;
        }

        /// <summary>
        /// Returns count of Product`s images
        /// </summary>
        public long GetProductImagesCount(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            long countThumb = objectContext.ProductImageSet.Count
                (img => img.Product.ID == currProduct.ID && img.isThumbnail == true);

            return countThumb;
        }

        /// <summary>
        /// Returns count of Company`s images
        /// </summary>
        public long GetCompanyImagesCount(Entities objectContext, Company currCompany)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            long countThumb = objectContext.CompanyImageSet.Count
                (img => img.Company.ID == currCompany.ID && img.isThumbnail == true);

            return countThumb;
        }

    }
}
