// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessAdvertisement
    {
        private object AddingLinks = new object();

        /// <summary>
        /// Adds new Advertisement to Advertisement table
        /// </summary>
        public void Add(Entities objectContext, BusinessLog bLog, Advertisement newAdvert, User currUser)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);

            if (newAdvert == null)
            {
                throw new BusinessException("newAdvert is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            objectContext.AddToAdvertisementSet(newAdvert);
            Tools.Save(objectContext);
            bLog.LogAdvertisement(objectContext, newAdvert, LogType.create, string.Empty, string.Empty, currUser);
        }

        /// <summary>
        /// Gets Advertisement with ID from the table , doesnt check for Visible
        /// </summary>

        public Advertisement Get(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (id < 1)
            {
                throw new BusinessException("Advertisement id is < 1");
            }

            Advertisement advert = objectContext.AdvertisementSet.FirstOrDefault(ad => ad.ID == id);
            return advert;
        }

        /// <summary>
        /// Updates field on Advertisement (without company,product and category links, they are in UpdateAdvertisementLinks)
        /// </summary>
        /// <param name="currAdvert">the Advertisement obect which is going to be updated</param>
        /// <param name="field">fields which can be updated : html,info,expireDate,targetUrl(cannot be empty or null)</param>
        /// <param name="newValue">the new value of the updated field</param>
        public void ChangeField(Entities objectContext, BusinessLog bLog, User currUser, Advertisement currAdvert, string field, string newValue)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currAdvert == null)
            {
                throw new BusinessException("currAdvert is null");
            }

            if (string.IsNullOrEmpty(field))
            {
                throw new BusinessException("field is null or empty");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string oldvalue = "";

            switch (field)
            {
                case ("html"):
                    if (string.IsNullOrEmpty(currAdvert.html))
                    {
                        oldvalue = string.Empty;
                    }
                    else
                    {
                        oldvalue = currAdvert.html;
                    }
                    currAdvert.html = newValue;
                    break;
                case ("info"):
                    if (string.IsNullOrEmpty(currAdvert.info))
                    {
                        oldvalue = string.Empty;
                    }
                    else
                    {
                        oldvalue = currAdvert.info;
                    }
                    currAdvert.info = newValue;
                    break;
                case ("expireDate"):
                    if (string.IsNullOrEmpty(currAdvert.expireDate.ToString()))
                    {
                        oldvalue = string.Empty;
                    }
                    else
                    {
                        oldvalue = currAdvert.expireDate.ToString();
                    }

                    if (string.IsNullOrEmpty(newValue))
                    {
                        currAdvert.expireDate = null;
                    }
                    else
                    {
                        DateTime expDate;
                        if (Tools.ParseStringToDateTime(newValue, out expDate) == true)
                        {
                            currAdvert.expireDate = expDate;
                        }
                        else
                        {
                            throw new BusinessException("couldnt parse newValue to DateTime");
                        }
                    }
                    break;
                case ("targetUrl"):
                    oldvalue = currAdvert.targetUrl;
                    if (string.IsNullOrEmpty(newValue))
                    {
                        throw new BusinessException("new target url is null or empty.");
                    }
                    currAdvert.targetUrl = newValue;
                    break;
                default:
                    throw new BusinessException(string.Format("Theres no field = {0}", field));
            }

            currAdvert.lastModified = DateTime.UtcNow;
            currAdvert.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);
            bLog.LogAdvertisement(objectContext, currAdvert, LogType.edit, field, oldvalue, currUser);
        }


        /// <summary>
        /// Changes one of The three boolean fields on Advertisement obect , if the newValue is same as olds it doesnt update
        /// </summary>
        /// <param name="field">boolean fields are : active,visible,general</param>
        /// <param name="newValue">true or false</param>
        public void ChangeActiveVisibleOrGeneral(Entities objectContext, BusinessLog bLog, User currUser, Advertisement currAdvert, string field, bool newValue)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currAdvert == null)
            {
                throw new BusinessException("currAdvert is null");
            }

            if (string.IsNullOrEmpty(field))
            {
                throw new BusinessException("field is null or empty");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            bool oldValue;

            UserID user = Tools.GetUserID(objectContext, currUser);

            switch (field)
            {
                case ("active"):
                    oldValue = currAdvert.active;
                    if (oldValue != newValue)
                    {
                        currAdvert.active = newValue;
                        currAdvert.LastModifiedBy = user;
                        currAdvert.lastModified = DateTime.UtcNow;
                        Tools.Save(objectContext);
                        bLog.LogAdvertisement(objectContext, currAdvert, LogType.edit, "active", oldValue.ToString(), currUser);
                    }
                    break;
                case ("visible"):
                    oldValue = currAdvert.visible;
                    if (oldValue)
                    {
                        if (oldValue != newValue)
                        {
                            currAdvert.visible = newValue;
                            currAdvert.LastModifiedBy = user;
                            currAdvert.lastModified = DateTime.UtcNow;
                            Tools.Save(objectContext);
                            bLog.LogAdvertisement(objectContext, currAdvert, LogType.delete, string.Empty, string.Empty, currUser);
                        }
                    }
                    else
                    {
                        if (oldValue != newValue)
                        {
                            currAdvert.visible = newValue;
                            currAdvert.LastModifiedBy = user;
                            currAdvert.lastModified = DateTime.UtcNow;
                            Tools.Save(objectContext);
                            bLog.LogAdvertisement(objectContext, currAdvert, LogType.undelete, string.Empty, string.Empty, currUser);
                        }
                    }
                    break;
                case ("general"):
                    oldValue = currAdvert.general;
                    if (oldValue != newValue)
                    {
                        currAdvert.general = newValue;
                        currAdvert.LastModifiedBy = user;
                        currAdvert.lastModified = DateTime.UtcNow;
                        Tools.Save(objectContext);
                        bLog.LogAdvertisement(objectContext, currAdvert, LogType.edit, "general", oldValue.ToString(), currUser);
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("field = {0} is not supported in this function", field));
            }
        }

        /// <summary>
        /// Generates a name for a Advertisement file
        /// </summary>
        /// <param name="path">the directory location where the file will be saved</param>
        /// <param name="extension">extension of the file</param>
        /// <returns>Complete path where the file will be saved whit new name</returns>
        public String GetAdvertisementFileName(Advertisement currAdvert, string path, string extension)
        {
            if (currAdvert == null)
            {
                throw new BusinessException("currAdvert is null");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new BusinessException("path is null or empty");
            }

            if (string.IsNullOrEmpty(extension))
            {
                throw new BusinessException("extension is null or empty");
            }

            string fileName = string.Empty;
            System.Text.StringBuilder name = new StringBuilder();
            Boolean generated = false;
            for (int i = 1; i < 100 && !generated; i++)
            {
                name.Length = 0;
                name.Append("a");
                name.Append(currAdvert.ID);
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
                    "Cannot generate unique file name for: Advert ID = {0}, path = \"{2}\", extension = \"{3}\".",
                    currAdvert.ID, path, extension);
                throw new BusinessException(errMsg);
            }
            return fileName;
        }

        /// <summary>
        /// Checks the file length and returns the result
        /// </summary>
        /// <param name="fileName">the name of the file</param>
        /// <param name="ErrorDescription">if result is false this string contains the error message</param>
        /// <returns>true if file length is between 1kb and 2mb , otherwise false</returns>
        public static bool IsValidAdvertisementFile(string fileName, byte[] fileBytes, ref string ErrorDescription)
        {
            bool advertOk = false;

            if (fileBytes != null)
            {
                int maxFileLength = 2097152;  // 2MB
                int minFileLength = 100; // 100 bytes

                if (minFileLength < fileBytes.Length && fileBytes.Length <= maxFileLength)
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        ErrorDescription = "File name is null or empty.";
                    }
                    else
                    {

                        if (ImageTools.IsImage(fileName, fileBytes, out ErrorDescription) == true)
                        {
                            int width;
                            int height;

                            if (ImageTools.GetImageWidthAndHeight(fileBytes, out width, out height))
                            {
                                if (width >= 250)
                                {
                                    ErrorDescription = "Image width cannot be more than 249px.";
                                }
                                else
                                {
                                    advertOk = true;
                                }
                            }
                            else
                            {
                                ErrorDescription = "Width/height of image is 0, or file is damaged.";
                            }

                        }
                        else if (ImageTools.IsSwf(fileBytes) == true || ImageTools.IsGif(fileBytes) == true)
                        {
                            advertOk = true;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(ErrorDescription))
                            {
                                ErrorDescription = "The file type is not supported";
                            }
                        }

                    }
                }
                else
                {
                    ErrorDescription = string.Format("The file is too {0}.", fileBytes.Length > maxFileLength ? "big" : "small");
                }
            }
            else
            {
                ErrorDescription = "Error uploading the file.";
            }
            return advertOk;
        }

        /// <summary>
        /// Changes in new Advertisement the location and the url of the file which are they using
        /// </summary>
        public void ChangeAdvertisementFilePathAndUrl(Entities objectContect, Advertisement currAdvert, string path, string url)
        {
            Tools.AssertObjectContextExists(objectContect);
            if (currAdvert == null)
            {
                throw new BusinessException("currAdvert is null");
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new BusinessException("path is null or empty");
            }
            if (string.IsNullOrEmpty(url))
            {
                throw new BusinessException("url is null or empty");
            }

            currAdvert.adpath = path;
            currAdvert.adurl = url;

            Tools.Save(objectContect);
        }

        /// <summary>
        /// Function using for Ordering Advertisement obects by ID field
        /// </summary>
        private long IdSelector(Advertisement advert)
        {
            if (advert == null)
            {
                throw new ArgumentNullException("advert");
            }
            return advert.ID;
        }

        /// <summary>
        /// Gets Advertisements sorted by a number of fields.
        /// </summary>
        /// <param name="type">all,product,category,company</param>
        /// <param name="number">number of found obects to return , sorts them by their ID , should be > 0</param>
        /// <param name="id">ID of the type wanted, if type is all then use 1</param>
        /// <param name="visible">0 - all , 1 - yes , 2 - no</param>
        /// <param name="general">0 - all , 1 - yes , 2 - no</param>
        /// <param name="active">0 - all , 1 - yes , 2 - no</param>
        /// <returns></returns>
        public List<Advertisement> GetAdvertisements(Entities objectContext, string type, long number, long id,
            int visible, int general, int active)
        {
            Tools.AssertObjectContextExists(objectContext);
            CheckSortByParams(type, number, id, visible, active, general);

            List<Advertisement> advertisements = SortByType(objectContext, type, id);

            if (visible != 0 || general != 0 || active != 0)
            {
                advertisements = SortByVisible(advertisements, visible);
                advertisements = SortByActive(advertisements, active);
                advertisements = SortByGeneral(advertisements, general);
            }
            advertisements = SortByNumber(advertisements, number);

            return advertisements;
        }

        private List<Advertisement> SortByNumber(List<Advertisement> adsList, long number)
        {

            int count = adsList.Count();

            if (count > 0)
            {
                if (number < 1)
                {
                    throw new BusinessException("number of wanted results is is < 1");
                }

                List<Advertisement> sortedAds = new List<Advertisement>();

                if (number > count)
                {
                    number = count;
                }

                for (int i = 0; i < number; i++)
                {
                    sortedAds.Add(adsList.ElementAt(i));
                }

                return sortedAds;
            }
            else
            {
                return adsList;
            }
        }

        private List<Advertisement> SortByVisible(List<Advertisement> adsList, int visible)
        {
            if (adsList.Count > 0 && visible != 0)
            {
                List<Advertisement> sortedAds = new List<Advertisement>();

                // 0 - all , 1 - yes , 2 - no

                switch (visible)
                {
                    case (1):
                        foreach (Advertisement ad in adsList)
                        {
                            if (ad.visible == true)
                            {
                                sortedAds.Add(ad);
                            }
                        }
                        break;
                    case (2):
                        foreach (Advertisement ad in adsList)
                        {
                            if (ad.visible == false)
                            {
                                sortedAds.Add(ad);
                            }
                        }
                        break;
                    default:
                        throw new BusinessException("visible is not 1 or 2");
                }
                return sortedAds;
            }
            else
            {
                return adsList;
            }
        }

        private List<Advertisement> SortByActive(List<Advertisement> adsList, int active)
        {
            if (adsList.Count > 0 && active != 0)
            {
                List<Advertisement> sortedAds = new List<Advertisement>();

                // 0 - all , 1 - yes , 2 - no

                switch (active)
                {
                    case (1):
                        foreach (Advertisement ad in adsList)
                        {
                            if (ad.active == true)
                            {
                                sortedAds.Add(ad);
                            }
                        }
                        break;
                    case (2):
                        foreach (Advertisement ad in adsList)
                        {
                            if (ad.active == false)
                            {
                                sortedAds.Add(ad);
                            }
                        }
                        break;
                    default:
                        throw new BusinessException("active is not 1 or 2");
                }
                return sortedAds;
            }
            else
            {
                return adsList;
            }
        }

        private List<Advertisement> SortByGeneral(List<Advertisement> adsList, int general)
        {
            if (adsList.Count > 0 && general != 0)
            {
                List<Advertisement> sortedAds = new List<Advertisement>();

                // 0 - all , 1 - yes , 2 - no

                switch (general)
                {
                    case (1):
                        foreach (Advertisement ad in adsList)
                        {
                            if (ad.general == true)
                            {
                                sortedAds.Add(ad);
                            }
                        }
                        break;
                    case (2):
                        foreach (Advertisement ad in adsList)
                        {
                            if (ad.general == false)
                            {
                                sortedAds.Add(ad);
                            }
                        }
                        break;
                    default:
                        throw new BusinessException("general is not 1 or 2");
                }
                return sortedAds;
            }
            else
            {
                return adsList;
            }
        }

        private List<Advertisement> SortByType(Entities objectContext, string type, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (id < 1)
            {
                throw new BusinessException("id of type wanted is less than 1");
            }

            List<Advertisement> adverts = new List<Advertisement>();
            if (!string.IsNullOrEmpty(type))
            {
                switch (type)
                {
                    case ("all"):
                        adverts = objectContext.AdvertisementSet.OrderByDescending<Advertisement, long>
                            (new Func<Advertisement, long>(IdSelector)).ToList();
                        break;
                    case ("product"):
                        adverts = GetProductCompanyOrCategoryAdverts(objectContext, "product", id, false);
                        break;
                    case ("category"):
                        adverts = GetProductCompanyOrCategoryAdverts(objectContext, "category", id, false);
                        break;
                    case ("company"):
                        adverts = GetProductCompanyOrCategoryAdverts(objectContext, "company", id, false);
                        break;
                    default:
                        throw new BusinessException(string.Format("type = {0} is not supported type.", type));
                }
            }
            else
            {
                throw new BusinessException("type is null or empty");
            }

            return adverts;
        }

        private static void CheckSortByParams(string type, long number, long id, int visible, int active, int general)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new BusinessException("type is null or empty");
            }

            if (number < 1)
            {
                throw new BusinessException("number of results wanted is less than 1");
            }

            if (id < 1)
            {
                throw new BusinessException("id of type wanted is less than 1");
            }

            if (visible < 0 || visible > 2)
            {
                throw new BusinessException("visible is < 0 || > 2");
            }

            if (active < 0 || active > 2)
            {
                throw new BusinessException("active is < 0 || > 2");
            }

            if (general < 0 || general > 2)
            {
                throw new BusinessException("general is < 0 || > 2");
            }
        }

        /// <summary>
        /// Returns all General=true adverts , which also are active=true amd visible=true
        /// </summary>
        public List<Advertisement> GetGeneralAdverts(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Advertisement> generals = objectContext.AdvertisementSet.Where
                (ad => ad.general == true && ad.active == true && ad.visible == true);

            return generals.ToList<Advertisement>();
        }

        /// <summary>
        /// Returns all Adverts which are set for Categories who are parent of the current
        /// </summary>
        /// <param name="id">Id of the category for which parents will be checked</param>
        public List<Advertisement> GetCategoryParentAdverts(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id of category is < 1");
            }
            BusinessCategory businessCategory = new BusinessCategory();
            Category currCategory = businessCategory.GetWithoutVisible(objectContext, id);

            if (currCategory == null)
            {
                throw new BusinessException(string.Format("theres no category with id {0}", id));
            }

            List<Category> parentCategories = businessCategory.GetAllParentCategories(objectContext, currCategory);

            List<Advertisement> adverts = new List<Advertisement>();
            if (parentCategories.Count > 0)
            {
                IEnumerable<AdvertisementsForCategory> catAdverts = null;

                foreach (Category category in parentCategories)
                {
                    catAdverts = objectContext.AdvertisementsForCategorySet.Where
                        (ad => ad.Category.ID == category.ID && ad.Category.visible == true);
                    if (catAdverts != null && catAdverts.Count() > 0)
                    {
                        foreach (AdvertisementsForCategory ad in catAdverts)
                        {
                            ad.AdvertisementReference.Load();
                            if (ad.Advertisement.visible == true && ad.Advertisement.active == true)
                            {
                                adverts.Add(ad.Advertisement);
                            }
                        }
                    }

                }

            }

            return adverts;
        }

        /// <summary>
        /// Returns all Advertisements which are Set for a Company`s products
        /// </summary>
        /// <param name="id">ID of the Company for which products are being checked</param>
        public List<Advertisement> GetCompanyProductsAdverts(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (id < 1)
            {
                throw new BusinessException("id of the company is < 1");
            }

            BusinessCompany businessCompany = new BusinessCompany();
            Company currCompany = businessCompany.GetCompanyWV(objectContext, id);
            if (currCompany == null)
            {
                throw new BusinessException(string.Format("theres no company with id {0}", id));
            }

            BusinessProduct businessProduct = new BusinessProduct();
            IEnumerable<Product> products = businessProduct.GetAllProductsFromCompany(objectContext, currCompany.ID, 0, long.MaxValue);

            List<Advertisement> adverts = new List<Advertisement>();

            if (products.Count<Product>() > 0)
            {
                IEnumerable<AdvertisementsForProduct> adsForProduct = null;

                foreach (Product product in products)
                {
                    product.AdvertisementsForProducts.Load();
                    adsForProduct = product.AdvertisementsForProducts.Where(ad => ad.visible == true);
                    if (adsForProduct != null && adsForProduct.Count() > 0)
                    {
                        foreach (AdvertisementsForProduct prodAd in adsForProduct)
                        {
                            prodAd.AdvertisementReference.Load();
                            if (prodAd.Advertisement.visible == true && prodAd.Advertisement.active == true)
                            {
                                if (!adverts.Contains(prodAd.Advertisement))
                                {
                                    adverts.Add(prodAd.Advertisement);
                                }
                            }
                        }
                    }
                }

            }
            return adverts;
        }

        /// <summary>
        /// Returns all ID`s of all products which have adverts
        /// </summary>
        public List<long> GetListOfProductsWhichHaveAdverts(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<long> ids = new List<long>();

            IEnumerable<AdvertisementsForProduct> ads = objectContext.AdvertisementsForProductSet.Where
                (ad => ad.visible == true);

            if (ads != null && ads.Count() > 0)
            {
                foreach (AdvertisementsForProduct ad in ads)
                {
                    ad.AdvertisementReference.Load();
                    if (ad.Advertisement.visible == true && ad.Advertisement.active == true)
                    {
                        ad.ProductReference.Load();
                        ids.Add(ad.Product.ID);
                    }
                }
            }

            return ids;
        }

        /// <summary>
        /// Returns all Adverts which are active=true and visible=true for type wih id
        /// </summary>
        /// <param name="type">company,product,category</param>
        /// <param name="id">ID of the type</param>
        public List<Advertisement> GetProductCompanyOrCategoryAdverts(Entities objectContext, string type, long id, bool checkForActiveAndVisibleAdvert)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(type))
            {
                throw new BusinessException("type is null or empty");
            }
            if (id < 1)
            {
                throw new BusinessException("id for type is < 1");
            }

            List<Advertisement> adverts = new List<Advertisement>();

            switch (type)
            {
                case ("company"):
                    IEnumerable<AdvertisementsForCompany> adsForCoompany = objectContext.AdvertisementsForCompanySet.Where
                       (ad => ad.Company.ID == id && ad.visible == true);

                    if (adsForCoompany != null && adsForCoompany.Count() > 0)
                    {
                        foreach (AdvertisementsForCompany ad in adsForCoompany)
                        {
                            ad.AdvertisementReference.Load();
                            if (checkForActiveAndVisibleAdvert)
                            {
                                if (ad.Advertisement.visible == true && ad.Advertisement.active == true)
                                {
                                    adverts.Add(ad.Advertisement);
                                }
                            }
                            else
                            {
                                adverts.Add(ad.Advertisement);
                            }
                        }
                    }


                    break;
                case ("product"):
                    IEnumerable<AdvertisementsForProduct> adsForProduct = objectContext.AdvertisementsForProductSet.Where
                       (ad => ad.Product.ID == id && ad.visible == true);

                    if (adsForProduct != null && adsForProduct.Count() > 0)
                    {
                        foreach (AdvertisementsForProduct ad in adsForProduct)
                        {
                            ad.AdvertisementReference.Load();
                            if (checkForActiveAndVisibleAdvert)
                            {
                                if (ad.Advertisement.visible == true && ad.Advertisement.active == true)
                                {
                                    adverts.Add(ad.Advertisement);
                                }
                            }
                            else
                            {
                                adverts.Add(ad.Advertisement);
                            }
                        }
                    }


                    break;
                case ("category"):
                    IEnumerable<AdvertisementsForCategory> adsForCategory = objectContext.AdvertisementsForCategorySet.Where
                       (ad => ad.Category.ID == id && ad.visible == true);

                    if (adsForCategory != null && adsForCategory.Count() > 0)
                    {
                        foreach (AdvertisementsForCategory ad in adsForCategory)
                        {
                            ad.AdvertisementReference.Load();
                            if (checkForActiveAndVisibleAdvert)
                            {
                                if (ad.Advertisement.visible == true && ad.Advertisement.active == true)
                                {
                                    adverts.Add(ad.Advertisement);
                                }
                            }
                            else
                            {
                                adverts.Add(ad.Advertisement);
                            }
                        }
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("type = {0} is not supported type", type));
            }

            return adverts;
        }

        /// <summary>
        /// Function which is checking if theres Visible and Active Advertisements which expire date is less than current ,
        /// if it founds such it makes them inactive , runs whenever new Statistics row is created (1 time everyday)
        /// </summary>
        public void ScriptCheckAdvertsExpirationDate(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<Advertisement> adverts = objectContext.AdvertisementSet.Where
                (ad => ad.visible == true && ad.active == true && ad.expireDate != null).ToList();

            if (adverts != null && adverts.Count() > 0)
            {
                DateTime dateNow = DateTime.UtcNow;

                foreach (Advertisement advert in adverts)
                {
                    if (DateTime.Compare(advert.expireDate.Value, dateNow) < 1)
                    {
                        advert.active = false;
                        Tools.Save(objectContext);
                    }
                }
            }

        }

        /// <summary>
        /// Returns as string (12,3123,456,) ID`s for which Advertisement have links of type
        /// </summary>
        public string GetAdvertTypeLinksAsString(Entities objectContext, Advertisement advert, AdvertisementsFor adFor, bool withSpaces)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (advert == null)
            {
                throw new BusinessException("advert is null");
            }

            StringBuilder strIDs = new StringBuilder();
            List<long> listIDs = new List<long>();

            listIDs = GetAdvertTypeLinksAsList(objectContext, advert, adFor);
            if (listIDs.Count > 0)
            {
                foreach (long id in listIDs)
                {
                    strIDs.Append(id);
                    if (withSpaces)
                    {
                        strIDs.Append(", ");
                    }
                    else
                    {
                        strIDs.Append(",");
                    }
                }
            }

            string ids = strIDs.ToString();
            if (ids.Length > 0)
            {
                if (withSpaces)
                {
                    ids = ids.Remove(ids.Length - 2);
                }
                else
                {
                    ids = ids.Remove(ids.Length - 1);
                }

            }

            return ids;
        }

        /// <summary>
        /// Returns list of ID`s of type for which Advertisement have visible=true links 
        /// </summary>
        public List<long> GetAdvertTypeLinksAsList(Entities objectContext, Advertisement advert, AdvertisementsFor adFor)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (advert == null)
            {
                throw new BusinessException("advert is null");
            }

            List<long> listIds = new List<long>();

            switch (adFor)
            {
                case AdvertisementsFor.Categories:

                    advert.AdvertisementsForCategories.Load();
                    if (advert.AdvertisementsForCategories.Count > 0)
                    {
                        foreach (AdvertisementsForCategory adCat in advert.AdvertisementsForCategories)
                        {
                            if (adCat.visible == true)
                            {
                                adCat.CategoryReference.Load();
                                listIds.Add(adCat.Category.ID);
                            }
                        }
                    }

                    break;
                case AdvertisementsFor.Companies:

                    advert.AdvertisementsForCompanies.Load();
                    if (advert.AdvertisementsForCompanies.Count > 0)
                    {
                        foreach (AdvertisementsForCompany adCat in advert.AdvertisementsForCompanies)
                        {
                            if (adCat.visible == true)
                            {
                                adCat.CompanyReference.Load();
                                listIds.Add(adCat.Company.ID);
                            }
                        }
                    }

                    break;
                case AdvertisementsFor.Products:

                    advert.AdvertisementsForProducts.Load();
                    if (advert.AdvertisementsForProducts.Count > 0)
                    {
                        foreach (AdvertisementsForProduct adCat in advert.AdvertisementsForProducts)
                        {
                            if (adCat.visible == true)
                            {
                                adCat.ProductReference.Load();
                                listIds.Add(adCat.Product.ID);
                            }
                        }
                    }

                    break;
                default:
                    throw new BusinessException(string.Format("{0} is not supported type", adFor));
            }

            return listIds;
        }

        public void AddToNewAdvertisementLinks(Entities objectContext, Advertisement advert,
            string categories, string companies, string products)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (advert == null)
            {
                throw new BusinessException("advert is null");
            }

            List<long> ids = new List<long>();

            List<Category> listCategories = new List<Category>();
            List<Company> listCompanies = new List<Company>();
            List<Product> listProducts = new List<Product>();

            if (!string.IsNullOrEmpty(categories) && Tools.GetAdvertLinkIDsFromString(categories, out ids))
            {
                BusinessCategory businessCategory = new BusinessCategory();

                foreach (long id in ids)
                {
                    Category currCat = businessCategory.Get(objectContext, id);
                    if (currCat == null)
                    {
                        throw new BusinessException(string.Format(
                            "There is no Category with ID = {0}  (or it is visible false), which can be linked with advertisement ID = {1}"
                            , id, advert.ID));
                    }

                    listCategories.Add(currCat);
                }
            }

            if (!string.IsNullOrEmpty(companies) && Tools.GetAdvertLinkIDsFromString(companies, out ids))
            {
                BusinessCompany businessCompany = new BusinessCompany();

                foreach (long id in ids)
                {
                    Company currComp = businessCompany.GetCompany(objectContext, id);

                    if (currComp == null)
                    {
                        throw new BusinessException(string.Format(
                             "There is no Company with ID = {0}  (or it is visible false), which can be linked with advertisement ID = {1}"
                             , id, advert.ID));
                    }

                    listCompanies.Add(currComp);
                }

            }

            if (!string.IsNullOrEmpty(products) && Tools.GetAdvertLinkIDsFromString(products, out ids))
            {
                BusinessProduct businessProduct = new BusinessProduct();

                foreach (long id in ids)
                {
                    Product currProduct = businessProduct.GetProductByID(objectContext, id);

                    if (currProduct == null)
                    {
                        throw new BusinessException(string.Format(
                             "There is no Product with ID = {0}  (or it is visible false), which can be linked with advertisement ID = {1}"
                             , id, advert.ID));
                    }

                    listProducts.Add(currProduct);
                }
            }

            lock (AddingLinks)
            {

                if (listCategories.Count > 0)
                {
                    foreach (Category category in listCategories)
                    {
                        AddCategoryLinkToAdvertisement(objectContext, advert, category);
                    }
                }

                if (listCompanies.Count > 0)
                {
                    foreach (Company company in listCompanies)
                    {
                        AddCompanyLinkToAdvertisement(objectContext, advert, company);
                    }
                }

                if (listProducts.Count > 0)
                {
                    foreach (Product product in listProducts)
                    {
                        AddProductLinkToAdvertisement(objectContext, advert, product);
                    }
                }

            }
        }

        private void AddCategoryLinkToAdvertisement(Entities objectContext, Advertisement advertisement, Category category)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (advertisement == null)
            {
                throw new BusinessException("advertisement is null");
            }
            if (category == null)
            {
                throw new BusinessException("category is null");
            }

            AdvertisementsForCategory testAdvert = null;
            testAdvert = objectContext.AdvertisementsForCategorySet.FirstOrDefault
                (ad => ad.Category.ID == category.ID && ad.Advertisement.ID == advertisement.ID);
            if (testAdvert == null)
            {
                AdvertisementsForCategory newAdvertLink = new AdvertisementsForCategory();
                newAdvertLink.Advertisement = advertisement;
                newAdvertLink.Category = category;
                newAdvertLink.visible = true;

                objectContext.AddToAdvertisementsForCategorySet(newAdvertLink);
                Tools.Save(objectContext);
            }
            else
            {
                if (testAdvert.visible == false)
                {
                    testAdvert.visible = true;
                    Tools.Save(objectContext);
                }
            }
        }

        private void AddCompanyLinkToAdvertisement(Entities objectContext, Advertisement advertisement, Company company)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (advertisement == null)
            {
                throw new BusinessException("advertisement is null");
            }
            if (company == null)
            {
                throw new BusinessException("company is null");
            }

            AdvertisementsForCompany testAdvert = null;
            testAdvert = objectContext.AdvertisementsForCompanySet.FirstOrDefault
                (ad => ad.Company.ID == company.ID && ad.Advertisement.ID == advertisement.ID);
            if (testAdvert == null)
            {
                AdvertisementsForCompany newAdvertLink = new AdvertisementsForCompany();
                newAdvertLink.Advertisement = advertisement;
                newAdvertLink.Company = company;
                newAdvertLink.visible = true;

                objectContext.AddToAdvertisementsForCompanySet(newAdvertLink);
                Tools.Save(objectContext);
            }
            else
            {
                if (testAdvert.visible == false)
                {
                    testAdvert.visible = true;
                    Tools.Save(objectContext);
                }
            }
        }

        private void AddProductLinkToAdvertisement(Entities objectContext, Advertisement advertisement, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (advertisement == null)
            {
                throw new BusinessException("advertisement is null");
            }
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            AdvertisementsForProduct testAdvert = null;
            testAdvert = objectContext.AdvertisementsForProductSet.FirstOrDefault
                (ad => ad.Product.ID == product.ID && ad.Advertisement.ID == advertisement.ID);
            if (testAdvert == null)
            {
                AdvertisementsForProduct newAdvertLink = new AdvertisementsForProduct();
                newAdvertLink.Advertisement = advertisement;
                newAdvertLink.Product = product;
                newAdvertLink.visible = true;

                objectContext.AddToAdvertisementsForProductSet(newAdvertLink);
                Tools.Save(objectContext);
            }
            else
            {
                if (testAdvert.visible == false)
                {
                    testAdvert.visible = true;
                    Tools.Save(objectContext);
                }
            }
        }

        /// <summary>
        /// Updates Advertisement links on EDIT
        /// </summary>
        public void UpdateAdvertisementLinks(Entities objectContext, Advertisement advertisement, string strCategories
            , string strCompanies, string strProducts, BusinessLog bLog, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (advertisement == null)
            {
                throw new BusinessException("advertisement is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            // Categories
            List<long> categoryIDs = new List<long>();
            Tools.GetAdvertLinkIDsFromString(strCategories, out categoryIDs);

            List<long> oldCategoryIDs = GetAdvertTypeLinksAsList(objectContext, advertisement, AdvertisementsFor.Categories);
            advertisement.AdvertisementsForCategories.Load();

            if (categoryIDs.Count == 0)
            {
                if (oldCategoryIDs.Count > 0)
                {
                    List<AdvertisementsForCategory> adForCats = new List<AdvertisementsForCategory>();
                    adForCats = advertisement.AdvertisementsForCategories.Where(ad => ad.visible == true).ToList();

                    if (adForCats.Count > 0)
                    {
                        foreach (AdvertisementsForCategory ad in adForCats)
                        {
                            ad.visible = false;
                            Tools.Save(objectContext);
                        }
                    }

                    bLog.LogAdvertisement(objectContext, advertisement, LogType.edit, "Category links", Tools.GetIDsAsString(oldCategoryIDs), currUser);
                }
            }
            else
            {
                BusinessCategory businessCategory = new BusinessCategory();

                Category currCategory = null;

                if (oldCategoryIDs.Count == 0)
                {
                    List<Category> catList = new List<Category>();

                    foreach (long id in categoryIDs)
                    {
                        currCategory = businessCategory.GetWithoutVisible(objectContext, id);
                        if (currCategory == null)
                        {
                            throw new BusinessException(string.Format("There is no category with id = {0}, which can be linked with advertisement"));
                        }
                        catList.Add(currCategory);
                    }

                    if (catList.Count > 0)
                    {
                        foreach (Category category in catList)
                        {
                            AddCategoryLinkToAdvertisement(objectContext, advertisement, category);
                        }

                        bLog.LogAdvertisement(objectContext, advertisement, LogType.edit, "Category links", "no links", currUser);
                    }

                }
                else
                {
                    AdvertisementsForCategory currAdForCat = null;

                    bLog.LogAdvertisement(objectContext, advertisement, LogType.edit, "Category links", Tools.GetIDsAsString(oldCategoryIDs), currUser);

                    foreach (long oldCatLink in oldCategoryIDs)
                    {
                        if (categoryIDs.Contains(oldCatLink))
                        {
                            categoryIDs.Remove(oldCatLink);
                        }
                        else
                        {
                            currAdForCat = objectContext.AdvertisementsForCategorySet.FirstOrDefault
                                (ad => ad.Category.ID == oldCatLink && ad.Advertisement.ID == advertisement.ID);

                            if (currAdForCat == null)
                            {
                                throw new BusinessException(string.Format("There is no AdvertisementsForCategory with Advertisement ID = {0} and Category ID = {1}"
                                  , advertisement.ID, oldCatLink));
                            }

                            currAdForCat.visible = false;
                            Tools.Save(objectContext);
                        }
                    }

                    if (categoryIDs.Count > 0)
                    {
                        foreach (long id in categoryIDs)
                        {
                            currCategory = businessCategory.GetWithoutVisible(objectContext, id);
                            if (currCategory == null)
                            {
                                throw new BusinessException(string.Format("There is no category ID = {0}, which can be linked with Advertisement id = {1}"
                                    , id, advertisement.ID));
                            }

                            AddCategoryLinkToAdvertisement(objectContext, advertisement, currCategory);
                        }
                    }


                }
            }


            // Companies

            List<long> companyIDs = new List<long>();
            Tools.GetAdvertLinkIDsFromString(strCompanies, out companyIDs);

            List<long> oldCompanyIDs = GetAdvertTypeLinksAsList(objectContext, advertisement, AdvertisementsFor.Companies);
            advertisement.AdvertisementsForCategories.Load();

            if (companyIDs.Count == 0)
            {
                if (oldCompanyIDs.Count > 0)
                {

                    List<AdvertisementsForCompany> adForComps = new List<AdvertisementsForCompany>();
                    adForComps = advertisement.AdvertisementsForCompanies.Where(ad => ad.visible == true).ToList();

                    if (adForComps.Count > 0)
                    {
                        foreach (AdvertisementsForCompany ad in adForComps)
                        {
                            ad.visible = false;
                            Tools.Save(objectContext);
                        }

                        bLog.LogAdvertisement(objectContext, advertisement, LogType.edit, "Company links", Tools.GetIDsAsString(oldCompanyIDs), currUser);
                    }
                }
            }
            else
            {
                BusinessCompany businessCompany = new BusinessCompany();

                Company currCompany = null;

                if (oldCompanyIDs.Count == 0)
                {

                    List<Company> compList = new List<Company>();

                    foreach (long id in companyIDs)
                    {
                        currCompany = businessCompany.GetCompanyWV(objectContext, id);
                        if (currCompany == null)
                        {
                            throw new BusinessException(string.Format("There is no company with id = {0}, which can be linked with advertisement"));
                        }
                        compList.Add(currCompany);
                    }

                    if (compList.Count > 0)
                    {
                        foreach (Company company in compList)
                        {
                            AddCompanyLinkToAdvertisement(objectContext, advertisement, company);
                        }

                        bLog.LogAdvertisement(objectContext, advertisement, LogType.edit, "Company links", "no links", currUser);
                    }

                }
                else
                {
                    AdvertisementsForCompany currAdForComp = null;

                    bLog.LogAdvertisement(objectContext, advertisement, LogType.edit, "Company links", Tools.GetIDsAsString(oldCompanyIDs), currUser);

                    foreach (long oldCompLink in oldCompanyIDs)
                    {
                        if (companyIDs.Contains(oldCompLink))
                        {
                            companyIDs.Remove(oldCompLink);
                        }
                        else
                        {
                            currAdForComp = objectContext.AdvertisementsForCompanySet.FirstOrDefault
                                (ad => ad.Company.ID == oldCompLink && ad.Advertisement.ID == advertisement.ID);

                            if (currAdForComp == null)
                            {
                                throw new BusinessException(string.Format("There is no AdvertisementsForCompany with Advertisement ID = {0} and Category ID = {1}"
                                  , advertisement.ID, oldCompLink));
                            }

                            currAdForComp.visible = false;
                            Tools.Save(objectContext);
                        }
                    }

                    if (companyIDs.Count > 0)
                    {
                        foreach (long id in companyIDs)
                        {
                            currCompany = businessCompany.GetCompanyWV(objectContext, id);
                            if (currCompany == null)
                            {
                                throw new BusinessException(string.Format("There is no company ID = {0}, which can be linked with Advertisement id = {1}"
                                    , id, advertisement.ID));
                            }

                            AddCompanyLinkToAdvertisement(objectContext, advertisement, currCompany);
                        }
                    }


                }
            }


            //Products

            List<long> productIDs = new List<long>();
            Tools.GetAdvertLinkIDsFromString(strProducts, out productIDs);

            List<long> oldProductIDs = GetAdvertTypeLinksAsList(objectContext, advertisement, AdvertisementsFor.Products);
            advertisement.AdvertisementsForCategories.Load();

            if (productIDs.Count == 0)
            {
                if (oldProductIDs.Count > 0)
                {

                    List<AdvertisementsForProduct> adForProds = new List<AdvertisementsForProduct>();
                    adForProds = advertisement.AdvertisementsForProducts.Where(ad => ad.visible == true).ToList();

                    if (adForProds.Count > 0)
                    {
                        bLog.LogAdvertisement(objectContext, advertisement, LogType.edit, "Product links", Tools.GetIDsAsString(oldProductIDs), currUser);

                        foreach (AdvertisementsForProduct ad in adForProds)
                        {
                            ad.visible = false;
                            Tools.Save(objectContext);
                        }
                    }
                }
            }
            else
            {
                BusinessProduct businessProduct = new BusinessProduct();

                Product currProduct = null;

                if (oldProductIDs.Count == 0)
                {

                    List<Product> prodList = new List<Product>();

                    foreach (long id in productIDs)
                    {
                        currProduct = businessProduct.GetProductByIDWV(objectContext, id);
                        if (currProduct == null)
                        {
                            throw new BusinessException(string.Format("There is no product with id = {0}, which can be linked with advertisement"));
                        }
                        prodList.Add(currProduct);
                    }

                    if (prodList.Count > 0)
                    {
                        foreach (Product product in prodList)
                        {
                            AddProductLinkToAdvertisement(objectContext, advertisement, product);
                        }
                    }

                    bLog.LogAdvertisement(objectContext, advertisement, LogType.edit, "Product links", "no links", currUser);
                }
                else
                {
                    AdvertisementsForProduct currAdForProd = null;

                    bLog.LogAdvertisement(objectContext, advertisement, LogType.edit, "Product links", Tools.GetIDsAsString(oldProductIDs), currUser);

                    foreach (long oldCompLink in oldProductIDs)
                    {
                        if (productIDs.Contains(oldCompLink))
                        {
                            productIDs.Remove(oldCompLink);
                        }
                        else
                        {
                            currAdForProd = objectContext.AdvertisementsForProductSet.FirstOrDefault
                                (ad => ad.Product.ID == oldCompLink && ad.Advertisement.ID == advertisement.ID);

                            if (currAdForProd == null)
                            {
                                throw new BusinessException(string.Format("There is no AdvertisementsForProduct with Advertisement ID = {0} and Category ID = {1}"
                                  , advertisement.ID, oldCompLink));
                            }

                            currAdForProd.visible = false;
                            Tools.Save(objectContext);
                        }
                    }

                    if (productIDs.Count > 0)
                    {
                        foreach (long id in productIDs)
                        {
                            currProduct = businessProduct.GetProductByIDWV(objectContext, id);
                            if (currProduct == null)
                            {
                                throw new BusinessException(string.Format("There is no product ID = {0}, which can be linked with Advertisement id = {1}"
                                    , id, advertisement.ID));
                            }

                            AddProductLinkToAdvertisement(objectContext, advertisement, currProduct);
                        }
                    }
                }
            }
        }
    }
}
