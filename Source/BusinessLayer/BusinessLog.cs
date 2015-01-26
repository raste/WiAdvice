// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessLog
    {
        private string userIPadress { get; set; }
        private long UserID { get; set; }

        public BusinessLog(long userID, string ipAdress)
        {
            UserID = userID;
            userIPadress = ipAdress;
        }

        public BusinessLog(long userID)
        {
            UserID = userID;
            userIPadress = "127.0.0.1";
        }

        /// <summary>
        /// Adds new Log
        /// </summary>
        private void Add(Entities objectContext, Log newLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (newLog == null)
            {
                throw new ArgumentNullException("newLog");
            }
            objectContext.AddToLogSet(newLog);

            Tools.Save(objectContext);
        }

        /// <summary>
        /// Builds new log
        /// </summary>
        private void BuildLog(Entities objectContext, LogType type, string typeOfObect, long obectID, string description)
        {
            if (userIPadress == null || userIPadress.Length < 1)
            {
                throw new BusinessException("userIPadress is null or empty");
            }

            if (typeOfObect == null || typeOfObect.Length < 1)
            {
                throw new BusinessException("typeOfObect is null or empty");
            }

            if (obectID < 1)
            {
                throw new BusinessException("obectID is < 1");
            }

            if (description == null || description.Length < 1)
            {
                throw new BusinessException("description is null or empty");
            }

            BusinessUser businessUser = new BusinessUser();

            User currUser = null;

            if (UserID < 1)
            {
                if (typeOfObect == "comment")
                {
                    currUser = businessUser.GetGuest();
                }
                else
                {
                    currUser = businessUser.GetSystem();
                }
            }
            else
            {
                currUser = businessUser.Get(UserID, true);
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserID user = Tools.GetUserID(objectContext, currUser);

            Log newLog = new Log();
            newLog.UserID = user;
            newLog.dateCreated = DateTime.UtcNow;
            newLog.description = description;
            newLog.type = GetLogTypeAsString(type);
            newLog.typeModifiedSubject = typeOfObect;
            newLog.IDModifiedSubject = obectID;
            newLog.userIPadress = userIPadress;

            Add(objectContext, newLog);
        }

        private void BuildUserLog(EntitiesUsers userContext, LogType type, string typeOfObect, User modifiedUser, string description)
        {
            if (userIPadress == null || userIPadress.Length < 1)
            {
                throw new BusinessException("userIPadress is null or empty");
            }

            if (typeOfObect == null || typeOfObect.Length < 1)
            {
                throw new BusinessException("typeOfObect is null or empty");
            }

            if (modifiedUser == null)
            {
                throw new BusinessException("modifiedUserID is null");
            }

            if (description == null || description.Length < 1)
            {
                throw new BusinessException("description is null or empty");
            }

            BusinessUser businessUser = new BusinessUser();

            User currUser = null;

            if (UserID < 1)
            {
                currUser = businessUser.GetSystem(userContext);
            }
            else
            {
                currUser = businessUser.Get(userContext, UserID, true);
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }


            UserLog newLog = new UserLog();
            newLog.UserModifying = currUser;
            newLog.dateCreated = DateTime.UtcNow;
            newLog.description = description;
            newLog.type = GetLogTypeAsString(type);
            newLog.typeModified = typeOfObect;
            newLog.UserModified = modifiedUser;
            newLog.IpAdress = userIPadress;

            userContext.AddToUserLogSet(newLog);
            Tools.Save(userContext);

        }

        public void LogAdvertisement(Entities objectContext, Advertisement advert, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (advert == null)
            {
                throw new BusinessException("advert is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Advertisement ID : {0} was added by {1}.", advert.ID, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Advertisement ID : {0} was deleted by {1}.", advert.ID, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Advertisement ID : {0} was UN-deleted by {1}.", advert.ID, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Advertisement ID : {0}, field : {1}, was modified by {2}, old value was : ' {3} '"
                        , advert.ID, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Advertisements", type));
            }

            BuildLog(objectContext, type, "advertisement", advert.ID, description);
        }

        public void LogSiteText(Entities objectContext, SiteNews siteText, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (siteText == null)
            {
                throw new BusinessException("siteText is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Site text : ' {0} ' was added by {1}.", siteText.name, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Site text : ' {0} ' was deleted by {1}.", siteText.name, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Site text : ' {0} ' was UN-deleted by {1}.", siteText.name, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Site text : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , siteText.name, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Site texts", type));
            }

            BuildLog(objectContext, type, "siteText", siteText.ID, description);
        }

        public void LogCategory(Entities objectContext, Category category, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (category == null)
            {
                throw new BusinessException("category is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Category : ' {0} ' was added by {1}.", category.name, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Category : ' {0} ' was deleted by {1}.", category.name, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Category : ' {0} ' was UN-deleted by {1}.", category.name, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Category : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , category.name, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Categories", type));
            }

            BuildLog(objectContext, type, "category", category.ID, description);
        }

        public void LogProduct(Entities objectContext, Product product, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Product : ' {0} ' was added by {1}.", product.name, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Product : ' {0} ' was deleted by {1}.", product.name, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Product : ' {0} ' was UN-deleted by {1}.", product.name, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Product : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , product.name, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Products", type));
            }

            BuildLog(objectContext, type, "product", product.ID, description);
        }

        public void LogSuggestion(Entities objectContext, Suggestion suggestion, LogType type, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Suggestion ID : '{0}' was added by {1}.", suggestion.ID, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Suggestion ID : '{0}' was deleted by {1}.", suggestion.ID, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Suggestion ID : '{0}' was UN-deleted by {1}.", suggestion.ID, currUser.username);
                    break;
                case LogType.edit:
                    throw new BusinessException("LogType.edit is not supported for suggestions");
            }

            BuildLog(objectContext, type, "suggestion", suggestion.ID, description);
        }

        public void LogRateProduct(Entities objectContext, Product product, ProductRating rating, LogType type, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }
            if (rating == null)
            {
                throw new BusinessException("rating is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Product : '{0}' was rated by {1}, rating id : {2}.", product.name, currUser.username, rating.ID);
                    break;
                case LogType.delete:

                    if (!rating.UserReference.IsLoaded)
                    {
                        rating.UserReference.Load();
                    }
                    User userRated = Tools.GetUserFromUserDatabase(rating.User.ID);

                    description = string.Format("Product : '{0}' rating by {1} ID : {2} was removed by {3}, rating id : {4}."
                        , product.name, userRated.username, userRated.ID, currUser.username, rating.ID);
                    break;
                case LogType.undelete:
                    if (!rating.UserReference.IsLoaded)
                    {
                        rating.UserReference.Load();
                    }
                    User userRated2 = Tools.GetUserFromUserDatabase(rating.User.ID);

                    description = string.Format("Product : '{0}' rating by {1} ID : {2} was Un-deleted by {3}, rating id : {4}."
                        , product.name, userRated2.username, userRated2.ID, currUser.username, rating.ID);
                    break;
                case LogType.edit:
                    throw new BusinessException("LogType.edit is not supported for rate products");
            }

            BuildLog(objectContext, type, "rateProduct", product.ID, description);
        }

        public void LogRateComment(Entities objectContext, Comment comment, CommentRating rating, LogType type, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }
            if (rating == null)
            {
                throw new BusinessException("rating is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Comment ID : '{0}' was rated by {1}, rating id : {2}.", comment.ID, currUser.username, rating.ID);
                    break;
                case LogType.delete:

                    if (!rating.UserReference.IsLoaded)
                    {
                        rating.UserReference.Load();
                    }
                    User userRated = Tools.GetUserFromUserDatabase(rating.User.ID);

                    description = string.Format("Comment ID : '{0}' rating by {1} ID : {2} was removed by {3}, rating id : {4}."
                        , comment.ID, userRated.username, userRated.ID, currUser.username, rating.ID);
                    break;
                case LogType.undelete:
                    if (!rating.UserReference.IsLoaded)
                    {
                        rating.UserReference.Load();
                    }
                    User userRated2 = Tools.GetUserFromUserDatabase(rating.User.ID);

                    description = string.Format("Comment ID : '{0}' rating by {1} ID : {2} was Un-deleted by {3}, rating id : {4}."
                        , comment.ID, userRated2.username, userRated2.ID, currUser.username, rating.ID);
                    break;
                case LogType.edit:
                    throw new BusinessException("LogType.edit is not supported for rate comments");
            }

            BuildLog(objectContext, type, "rateComment", comment.ID, description);
        }

        public void LogCompany(Entities objectContext, Company company, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (company == null)
            {
                throw new BusinessException("company is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Company : ' {0} ' was added by {1}.", company.name, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Company : ' {0} ' was deleted by {1}.", company.name, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Company : ' {0} ' was UN-deleted by {1}.", company.name, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Company : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , company.name, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Companies", type));
            }

            BuildLog(objectContext, type, "company", company.ID, description);
        }

        public void LogCompanyType(Entities objectContext, CompanyType companyType, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (companyType == null)
            {
                throw new BusinessException("companyType is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Company type : ' {0} ' was added by {1}.", companyType.name, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Company type : ' {0} ' was deleted by {1}.", companyType.name, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Company type : ' {0} ' was UN-deleted by {1}.", companyType.name, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Company type : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , companyType.name, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Company types", type));
            }

            BuildLog(objectContext, type, "companyType", companyType.ID, description);
        }

        public void LogUser(Entities objectContext, EntitiesUsers userContext, User userModified, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (userModified == null)
            {
                throw new BusinessException("userModified is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("User : ' {0} ' registered by {1}.", userModified.username, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("User : ' {0} ' was deleted by {1}.", userModified.username, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("User : ' {0} ' was UN-deleted by {1}.", userModified.username, currUser.username);
                    break;
                case LogType.edit:

                    if (currUser == userModified)
                    {
                        description = string.Format("User : ' {0} ' modified field : {1}, old value was : ' {2} '"
                           , userModified.username, field, oldValue);
                    }
                    else
                    {
                        description = string.Format("User : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                            , userModified.username, field, currUser.username, oldValue);
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Users", type));
            }

            BuildLog(objectContext, type, "user", userModified.ID, description);
            BuildUserLog(userContext, type, "user", userModified, description);
        }

        public void LogCompanyCharacteristic(Entities objectContext, CompanyCharacterestics characteristic, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (characteristic == null)
            {
                throw new BusinessException("characteristic is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Company characteristic : ' {0} ' was added by {1}.", characteristic.name, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Company characteristic : ' {0} ' was deleted by {1}.", characteristic.name, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Company characteristic : ' {0} ' was UN-deleted by {1}.", characteristic.name, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Company characteristic : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , characteristic.name, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Company characteristics", type));
            }

            BuildLog(objectContext, type, "companyCharacteristic", characteristic.ID, description);
        }


        public void LogProductCharacteristic(Entities objectContext, ProductCharacteristics characteristic, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (characteristic == null)
            {
                throw new BusinessException("characteristic is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Product characteristic : ' {0} ' was added by {1}.", characteristic.name, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Product characteristic : ' {0} ' was deleted by {1}.", characteristic.name, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Product characteristic : ' {0} ' was UN-deleted by {1}.", characteristic.name, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Product characteristic : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , characteristic.name, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Product characteristics", type));
            }

            BuildLog(objectContext, type, "productCharacteristic", characteristic.ID, description);
        }

        public void LogProductLink(Entities objectContext, ProductLink link, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (link == null)
            {
                throw new BusinessException("link is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (!link.ProductReference.IsLoaded)
            {
                link.ProductReference.Load();
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Product link : ' {0} ' (for product ' {1} ' ID : {2})  was added by {3}."
                        , link.link, link.Product.name, link.Product.ID, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Product link : ' {0} ' (for product ' {1} ' ID : {2}) was deleted by {3}."
                        , link.link, link.Product.name, link.Product.ID, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Product link : ' {0} ' (for product ' {1} ' ID : {2}) was UN-deleted by {3}."
                        , link.link, link.Product.name, link.Product.ID, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Product link : ' {0} ' (for product ' {1} ' ID : {2}), field : {3}, was modified by {4}, old value was : ' {5} '"
                        , link.link, link.Product.name, link.Product.ID, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Product links", type));
            }

            BuildLog(objectContext, type, "productLink", link.ID, description);
        }

        public void LogProductAlternativeName(Entities objectContext, AlternativeProductName altName, LogType type, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (altName == null)
            {
                throw new BusinessException("altName is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            if (!altName.ProductReference.IsLoaded)
            {
                altName.ProductReference.Load();
            }

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Alternative product name : ' {0} ' for product id : {1} was added by {2}."
                        , altName.name, altName.Product.ID, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Alternative product name : ' {0} ' for product id : {1} was deleted by {2}."
                        , altName.name, altName.Product.ID, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Alternative product name : ' {0} ' for product id : {1} was UN-deleted by {2}."
                        , altName.name, altName.Product.ID, currUser.username);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Alternative product name", type));
            }

            BuildLog(objectContext, type, "alternativeProductName", altName.ID, description);
        }

        public void LogCompanyAlternativeName(Entities objectContext, AlternativeCompanyName altName, LogType type, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (altName == null)
            {
                throw new BusinessException("altName is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            if (!altName.CompanyReference.IsLoaded)
            {
                altName.CompanyReference.Load();
            }

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Alternative company name : ' {0} ' for company id : {1} was added by {2}."
                        , altName.name, altName.Company.ID, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Alternative company name : ' {0} ' for company id : {1} was deleted by {2}."
                        , altName.name, altName.Company.ID, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Alternative company name : ' {0} ' for company id : {1} was UN-deleted by {2}."
                        , altName.name, altName.Company.ID, currUser.username);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Alternative company name", type));
            }

            BuildLog(objectContext, type, "alternativeCompanyName", altName.ID, description);
        }

        public void LogProductVariant(Entities objectContext, ProductVariant variant, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (variant == null)
            {
                throw new BusinessException("variant is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (!variant.ProductReference.IsLoaded)
            {
                variant.ProductReference.Load();
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Product variant : ' {0} ' for ' {1} ' ID : {2} was added by {3}."
                        , variant.name, variant.Product.name, variant.Product.ID, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Product variant : ' {0} '  for ' {1} ' ID : {2} was deleted by {2}."
                       , variant.name, variant.Product.name, variant.Product.ID, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Product variant : ' {0} '  for ' {1} ' ID : {2} was UN-deleted by {2}."
                       , variant.name, variant.Product.name, variant.Product.ID, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Product variant : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , variant.name, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Product SUB variants", type));
            }

            BuildLog(objectContext, type, "productVariant", variant.ID, description);
        }

        public void LogProductTopic(Entities objectContext, ProductTopic topic, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (!topic.ProductReference.IsLoaded)
            {
                topic.ProductReference.Load();
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Product topic : ' {0} ' for ' {1} ' ID : {2} was created by {3}."
                        , topic.name, topic.Product.name, topic.Product.ID, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Product topic : ' {0} ' for ' {1} ' ID : {2} was deleted by {3}."
                       , topic.name, topic.Product.name, topic.Product.ID, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Product topic : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , topic.name, field, currUser.username, oldValue);
                    break;
                case LogType.undelete:
                    description = string.Format("Product topic : ' {0} ', for ' {1} ' ID : {2} was UNdeleted by {3}."
                       , topic.name, topic.Product.name, topic.Product.ID, currUser.username);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Product topics", type));
            }

            BuildLog(objectContext, type, "productTopic", topic.ID, description);
        }

        public void LogProductSubVariant(Entities objectContext, ProductSubVariant variant, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (variant == null)
            {
                throw new BusinessException("variant is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (!variant.ProductReference.IsLoaded)
            {
                variant.ProductReference.Load();
            }
            if (!variant.VariantReference.IsLoaded)
            {
                variant.VariantReference.Load();
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Product sub variant : ' {0} ' for product ' {1} ' ID : {2}, variant ' {3} ' ID : {4}, was added by {5}."
                        , variant.name, variant.Product.name, variant.Product.ID, variant.Variant.name, variant.Variant.ID, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Product sub variant : ' {0} '  for product ' {1} ' ID : {2}, variant ' {3} ' ID : {4}, was deleted by {2}."
                       , variant.name, variant.Product.name, variant.Product.ID, variant.Variant.name, variant.Variant.ID, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Product sub variant : ' {0} '  for ' {1} ' ID : {2}, variant ' {3} ' ID : {4}, was UN-deleted by {2}."
                       , variant.name, variant.Product.name, variant.Product.ID, variant.Variant.name, variant.Variant.ID, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Product sub variant : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , variant.name, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Product variants", type));
            }

            BuildLog(objectContext, type, "productSubVariant", variant.ID, description);
        }

        /// <summary>
        /// guestname only if user added comment is guest, for other types user is wanted
        /// </summary>
        public void LogComment(Entities objectContext, Comment comment, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }

            if (currUser == null && string.IsNullOrEmpty(comment.guestname))
            {
                throw new BusinessException("currUser is null AND guestName is empty");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:

                    string name = string.Empty;
                    if (currUser == null)
                    {
                        name = string.Format("{0} (guest)", comment.guestname);
                    }
                    else
                    {
                        name = currUser.username;
                    }
                    description = string.Format("Comment ID : ' {0} ' was written by {1}.", comment.ID, name);
                    break;

                case LogType.delete:

                    if (currUser == null)
                    {
                        throw new BusinessException("User deleting comment cannot be null");
                    }
                    description = string.Format("Comment ID : ' {0} ' was deleted by {1}.", comment.ID, currUser.username);
                    break;

                case LogType.undelete:

                    if (currUser == null)
                    {
                        throw new BusinessException("User Un-deleting comment cannot be null");
                    }
                    description = string.Format("Comment ID : ' {0} ' was UN-deleted by {1}.", comment.ID, currUser.username);
                    break;

                case LogType.edit:

                    if (currUser == null)
                    {
                        throw new BusinessException("User editing comment cannot be null");
                    }
                    description = string.Format("Comment ID : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , comment.ID, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Comments", type));
            }

            BuildLog(objectContext, type, "comment", comment.ID, description);
        }


        public void LogIpBan(Entities objectContext, IpBan ipBan, LogType type, string field, string oldValue, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (ipBan == null)
            {
                throw new BusinessException("ipBan is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("{0} banned ip adress : {1}.", currUser.username, ipBan.IPadress);
                    break;
                case LogType.delete:
                    description = string.Format("{0} Unbanned adress : ' {1} '.", currUser.username, ipBan.IPadress);
                    break;
                case LogType.undelete:
                    description = string.Format("Ip adress : ' {0} ' was banned AGAIN, this time by {1}.", ipBan.IPadress, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Ip BAN ID : ' {0} ', field : {1}, was modified by {2}, old value was : ' {3} '"
                        , ipBan.ID, field, currUser.username, oldValue);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for ip bans", type));
            }

            BuildLog(objectContext, type, "IpBan", ipBan.ID, description);
        }

        public void LogNotify(Entities objectContext, NotifyOnNewContent notify, LogType type, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (notify == null)
            {
                throw new BusinessException("notify is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("{0} added to notifies {1} {2}.", currUser.username, notify.type, notify.ID);
                    break;
                case LogType.delete:
                    if (!notify.UserReference.IsLoaded)
                    {
                        notify.UserReference.Load();
                    }
                    if (notify.User.ID != currUser.ID)
                    {
                        User notifyUser = Tools.GetUserFromUserDatabase(notify.User);

                        description = string.Format("{0} unsubscribed user {1} ID : {2} on notifies for {3} {4}."
                            , currUser.username, notifyUser.username, notifyUser.ID, notify.type, notify.ID);
                    }
                    else
                    {
                        description = string.Format("{0} unsubscribed for notifies on {1} {2}.", currUser.username, notify.type, notify.ID);
                    }

                    break;
                case LogType.undelete:
                    description = string.Format("{0} subscribed for notifies AGAIN on {1} {2}.", currUser.username, notify.type, notify.ID);
                    break;
                case LogType.edit:
                    throw new BusinessException("LogType.edit is not supported on notifies.");
            }

            BuildLog(objectContext, type, "notify", notify.ID, description);
        }

        /// <summary>
        /// LogType.Edit when active status has changed with newstatus: declined, accepted, expired 
        /// </summary>
        public void LogTypeSuggestion(Entities objectContext, EntitiesUsers userContext
            , TypeSuggestion suggestion, LogType type, User currUser, string newstatus)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:

                    if (!suggestion.ToUserReference.IsLoaded)
                    {
                        suggestion.ToUserReference.Load();
                    }

                    BusinessUser bUser = new BusinessUser();
                    User toUser = bUser.Get(userContext, suggestion.ToUser.ID, true);

                    description = string.Format("{0} sent suggestion for {1} {2} to {3} ID : {4}."
                        , currUser.username, suggestion.type, suggestion.typeID, toUser.username, toUser.ID);

                    break;
                case LogType.delete:

                    description = string.Format("{0} removed suggestion.", currUser.username);

                    break;
                case LogType.edit:

                    if (string.IsNullOrEmpty(newstatus))
                    {
                        throw new BusinessException("newstatus is empty");
                    }

                    switch (newstatus)
                    {
                        case "declined":
                            description = string.Format("{0} declined suggestion.", currUser.username);
                            break;
                        case "accepted":
                            description = string.Format("{0} accepted suggestion.", currUser.username);
                            break;
                        case "expired":
                            description = string.Format("{0} set suggestion as EXPIRED, because it wasn`t accepted or declined in {1} days."
                                , currUser.username, Configuration.TypeSuggestionDaysAfterWhichSuggestionExpires);
                            break;
                        default:
                            throw new BusinessException(string.Format("newstatus = {0} is not supported status", newstatus));
                    }

                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {} is not supported type for typesuggestions", type));
            }

            BuildLog(objectContext, type, "typeSuggestion", suggestion.ID, description);
        }


        public void LogRole(Entities objectContext, EntitiesUsers userContext, UserAction uaction, LogType type, User approver)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (uaction == null)
            {
                throw new BusinessException("uaction is null");
            }
            if (approver == null)
            {
                approver = Tools.GetSystem();
            }

            if (!uaction.ActionReference.IsLoaded)
            {
                uaction.ActionReference.Load();
            }
            if (!uaction.UserReference.IsLoaded)
            {
                uaction.UserReference.Load();
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Role ' {0} ' was given to {1} ID : {2}, by {3}"
                        , uaction.Action.name, uaction.User.username, uaction.User.ID, approver.username);
                    break;
                case LogType.delete:
                    description = string.Format("Role ' {0} ' was removed from {1} ID : {2}, by {3}"
                         , uaction.Action.name, uaction.User.username, uaction.User.ID, approver.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Role ' {0} ' was given AGAIN to {1} ID : {2}, by {3}"
                        , uaction.Action.name, uaction.User.username, uaction.User.ID, approver.username);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported type for Roles.", type.ToString()));
            }

            BuildLog(objectContext, type, "role", uaction.User.ID, description);
            BuildUserLog(userContext, type, "role", uaction.User, description);
        }


        public void LogUserTypeAction(Entities objectContext, UsersTypeAction action, LogType type, User approver)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (action == null)
            {
                throw new BusinessException("action is null");
            }
            if (approver == null)
            {
                approver = Tools.GetSystem();
            }

            if (!action.TypeActionReference.IsLoaded)
            {
                action.TypeActionReference.Load();
            }
            if (!action.UserReference.IsLoaded)
            {
                action.UserReference.Load();
            }

            User actionUser = Tools.GetUserFromUserDatabase(action.User);

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    if (action.User.ID == approver.ID)
                    {
                        description = string.Format("{0} ID : {1} took type role for ' {2} ' (took the role from another user or the type didn`t had any editors)"
                            , actionUser.username, actionUser.ID, action.TypeAction.name);
                    }
                    else
                    {
                        description = string.Format("Type role ' {0} ' was given to {1} ID : {2}, by {3}"
                            , action.TypeAction.name, actionUser.username, actionUser.ID, approver.username);
                    }
                    break;
                case LogType.delete:
                    description = string.Format("Type role ' {0} ' was removed from {1} ID : {2}, by {3}"
                        , action.TypeAction.name, actionUser.username, actionUser.ID, approver.username);
                    break;
                case LogType.undelete:
                    if (action.User.ID == approver.ID)
                    {
                        description = string.Format("{0} ID : {1} took type role for ' {2} ' ( RECEIVED AGAIN ) (took the role from another user or the type didn`t had any editors)"
                            , actionUser.username, actionUser.ID, action.TypeAction.name);
                    }
                    else
                    {
                        description = string.Format("Type role ' {0} ' was given AGAIN to {1} ID : {2}, by {3}"
                             , action.TypeAction.name, actionUser.username, actionUser.ID, approver.username);
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported type for Type roles.", type.ToString()));
            }

            BuildLog(objectContext, type, "role", action.User.ID, description);
        }

        /// <summary>
        /// removeType : accept , decline, removing
        /// </summary>
        public void LogActionTransfer(Entities objectContext, EntitiesUsers userContext, TransferTypeAction transfer, LogType type, User user, string removeType)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (transfer == null)
            {
                throw new BusinessException("transfer is null");
            }

            BusinessUser bUser = new BusinessUser();

            if (user == null)
            {
                user = bUser.GetSystem(userContext);
            }

            if (!transfer.UserTypeActionReference.IsLoaded)
            {
                transfer.UserTypeActionReference.Load();
            }
            if (!transfer.UserTypeAction.TypeActionReference.IsLoaded)
            {
                transfer.UserTypeAction.TypeActionReference.Load();
            }
            if (!transfer.UserTransferingReference.IsLoaded)
            {
                transfer.UserTransferingReference.Load();
            }
            if (!transfer.UserReceivingReference.IsLoaded)
            {
                transfer.UserReceivingReference.Load();
            }

            User transferor = bUser.GetWithoutVisible(userContext, transfer.UserTransfering.ID, true);
            User receiver = bUser.GetWithoutVisible(userContext, transfer.UserReceiving.ID, true);

            string descrForTransferingUser = string.Empty;
            string descrForReceivingUser = string.Empty;

            string actionType = transfer.UserTypeAction.TypeAction.type;
            long typeID = transfer.UserTypeAction.TypeAction.typeID;

            switch (type)
            {
                case LogType.create:
                    descrForTransferingUser = string.Format("{0} started transfering role for {1} {2} to {3} ID : {4}. Transfer ID : {5}."
                        , transferor.username, actionType, typeID, receiver.username, receiver.ID, transfer.ID);

                    descrForReceivingUser = string.Format("{0} ID : {1} started transfering role for {2} {3} to {4}. Transfer ID : {5}."
                        , transferor.username, transferor.ID, actionType, typeID, receiver.username, transfer.ID);
                    break;
                case LogType.delete:

                    switch (removeType)
                    {
                        case "accept":
                            descrForReceivingUser = string.Format("{0} accepted transfer on role for {1} {2} by {3} ID : {4}. Transfer ID : {5}."
                                , receiver.username, actionType, typeID, transferor.username, transferor.ID, transfer.ID);

                            descrForTransferingUser = string.Format("{0} ID : {1} accepted transfer on role for {2} {3} by {4}. Transfer ID : {5}."
                                , receiver.username, receiver.ID, actionType, typeID, transferor.username, transfer.ID);
                            break;
                        case "decline":
                            if (transferor == user)
                            {
                                descrForReceivingUser = string.Format("{0} ID : {1} declined his transfer on role for {2} {3} to {4}. Transfer ID : {5}."
                                    , user.username, user.ID, actionType, typeID, receiver.username, transfer.ID);

                                descrForTransferingUser = string.Format("{0} declined his transfer on role for {1} {2} to {3} ID : {4}. Transfer ID : {5}."
                                    , user.username, actionType, typeID, receiver.username, receiver.ID, transfer.ID);
                            }
                            else if (receiver == user)
                            {
                                descrForReceivingUser = string.Format("{0} declined transfer on role for {1} {2} by {3} ID : {4}. Transfer ID : {5}."
                                    , user.username, actionType, typeID, transferor.username, transferor.ID, transfer.ID);

                                descrForTransferingUser = string.Format("{0} ID : {1} declined transfer on role for {2} {3} by {4}. Transfer ID : {5}."
                                    , user.username, user.ID, actionType, typeID, transferor.username, transfer.ID);
                            }
                            else
                            {
                                throw new BusinessException(string.Format
                                    ("User id : {0} shouldn`t be able to decline transfer ID : {0}, because he isn`t either the user transfering or receiving."
                                    , user.ID, transfer.ID));
                            }

                            break;
                        case "removing":
                            descrForReceivingUser = string.Format("{0} removed transfer on role for {1} {2} by {3} ID : {4}. Transfer ID : {5}."
                                    , user.username, actionType, typeID, transferor.username, transferor.ID, transfer.ID);

                            descrForTransferingUser = string.Format("{0} removed transfer on role for {1} {2} by {3}. Transfer ID : {4}."
                                , user.username, actionType, typeID, transferor.username, transfer.ID);


                            break;
                        default:
                            throw new BusinessException(string.Format("removeType = {0} is not supported type for Action transfer roles.", removeType));
                    }


                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported type for Action transfer roles.", type.ToString()));
            }

            BuildLog(objectContext, type, "roleTransfer", transferor.ID, descrForTransferingUser);
            BuildLog(objectContext, type, "roleTransfer", receiver.ID, descrForReceivingUser);
        }

        public void LogGetActionFromUser(Entities objectContext, TypeAction action, User userTaking, User userWhichHadAction)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (action == null)
            {
                throw new BusinessException("action is null");
            }
            if (userTaking == null)
            {
                throw new BusinessException("userTaking is null");
            }
            if (userWhichHadAction == null)
            {
                throw new BusinessException("userWhichHadAction is null");
            }

            string descrForUserTaking = string.Empty;
            string descrForUserWhichHadAction = string.Empty;

            descrForUserTaking = string.Format("{0} took action for {1} {2} from ' {3} ' ID : {4}.", userTaking.username
                , action.type, action.typeID, userWhichHadAction.username, userWhichHadAction.ID);

            descrForUserWhichHadAction = string.Format("' {0} ' ID : {1} took action for {2} {3} from {4}.", userTaking.username
                , userTaking.ID, action.type, action.typeID, userWhichHadAction.username);

            BuildLog(objectContext, LogType.create, "roleTaking", userTaking.ID, descrForUserTaking);
            BuildLog(objectContext, LogType.delete, "roleTaking", userWhichHadAction.ID, descrForUserWhichHadAction);
        }


        public void LogCompanyCategory(Entities objectContext, CategoryCompany category, LogType type, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (category == null)
            {
                throw new BusinessException("category is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (!category.CategoryReference.IsLoaded)
            {
                category.CategoryReference.Load();
            }
            if (!category.CompanyReference.IsLoaded)
            {
                category.CompanyReference.Load();
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    description = string.Format("Category ' {0} ' ID : {1}, was added to Company ' {2} ' ID : {3}, by {2}."
                        , category.Category.name, category.Category.ID, category.Company.name, category.Company.ID, currUser.username);
                    break;
                case LogType.delete:
                    description = string.Format("Category ' {0} ' ID : {1}, for Company ' {2} ' ID : {3}, was deleted by {2}."
                         , category.Category.name, category.Category.ID, category.Company.name, category.Company.ID, currUser.username);
                    break;
                case LogType.undelete:
                    description = string.Format("Category ' {0} ' ID : {1}, for Company ' {2} ' ID : {3}, was Un deleted by {2}."
                         , category.Category.name, category.Category.ID, category.Company.name, category.Company.ID, currUser.username);
                    break;
                case LogType.edit:
                    throw new BusinessException("LogType.edit is not supported for CategoryCompany");
            }

            BuildLog(objectContext, type, "categoryCompany", category.ID, description);
        }



        /// <summary>
        /// Returns logs type=create for products
        /// </summary>
        public IEnumerable<Log> getAddedProducts(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getProducts = objectContext.LogSet.Where(log => log.type == "create" && log.typeModifiedSubject == "product").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getProducts;
        }

        /// <summary>
        /// Returns logs type=setDeleted for products
        /// </summary>
        public IEnumerable<Log> getDeletedProducts(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getProducts = objectContext.LogSet.Where(log => log.type == "setDeleted" && log.typeModifiedSubject == "product").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getProducts;
        }


        public Log getDeletedProduct(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }
            Log getProduct = objectContext.LogSet.FirstOrDefault
                (log => log.type == "setDeleted" && log.typeModifiedSubject == "product" && log.IDModifiedSubject == id);

            return getProduct;
        }

        public IEnumerable<Log> getUnDeletedProducts(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getProducts = objectContext.LogSet.Where(log => log.type == "unSetDeleted" && log.typeModifiedSubject == "product").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getProducts;
        }

        public IEnumerable<Log> getProductEditLogs(Entities objectContext, long prodID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (prodID < 1)
            {
                throw new BusinessException("prodID is < 1");
            }

            IEnumerable<Log> getProduct = objectContext.LogSet.Where(log => log.typeModifiedSubject == "product" && log.IDModifiedSubject == prodID).
                OrderByDescending<Log, long>(new Func<Log, long>(IdSelectorLog));

            return getProduct;
        }

        public IEnumerable<Log> getProductCharacteristicEditLogs(Entities objectContext, long typeID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (typeID < 1)
            {
                throw new BusinessException("typeID is < 1");
            }

            IEnumerable<Log> getProductCharacteristics = objectContext.LogSet.Where
                (log => log.typeModifiedSubject == "productCharacteristic" && log.IDModifiedSubject == typeID).
                OrderByDescending<Log, long>(new Func<Log, long>(IdSelectorLog));

            return getProductCharacteristics;
        }


        public IEnumerable<Log> getAddedCategories(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getCategories = objectContext.LogSet.Where(log => log.type == "create" && log.typeModifiedSubject == "categoty").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getCategories;
        }

        public IEnumerable<Log> getDeletedCategories(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getCategories = objectContext.LogSet.Where(log => log.type == "setDeleted" && log.typeModifiedSubject == "category").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getCategories;
        }

        public Log getDeletedCategory(Entities objectContext, long catID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (catID < 1)
            {
                throw new BusinessException("catID is < 1");
            }

            Log category = objectContext.LogSet.
                OrderByDescending<Log, long>(new Func<Log, long>(IdSelectorLog)).
                FirstOrDefault(log => log.type == "setDeleted" &&
                    log.typeModifiedSubject == "category" && log.IDModifiedSubject == catID);

            return category;
        }

        public IEnumerable<Log> getUnDeletedCategories(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getCategories = objectContext.LogSet.Where(log => log.type == "unSetDeleted" && log.typeModifiedSubject == "category").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getCategories;
        }

        public IEnumerable<Log> getCategoryEditLogs(Entities objectContext, long typeID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (typeID < 1)
            {
                throw new BusinessException("typeID is < 1");
            }

            IEnumerable<Log> getLogs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "category" && log.IDModifiedSubject == typeID).
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getLogs;
        }

        public IEnumerable<Log> getAddedCompanies(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getCompanies = objectContext.LogSet.Where(log => log.type == "create" && log.typeModifiedSubject == "company").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getCompanies;
        }

        public IEnumerable<Log> getDeletedCompanies(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getCompanies = objectContext.LogSet.Where(log => log.type == "setDeleted" && log.typeModifiedSubject == "company").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getCompanies;
        }

        public Log getDeletedCompany(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }

            Log getCompanies = objectContext.LogSet.FirstOrDefault
                (log => log.type == "setDeleted" && log.typeModifiedSubject == "company" && log.IDModifiedSubject == id);

            return getCompanies;
        }

        public Log getDeletedCompanyCharacteristic(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }

            Log getCompanies = objectContext.LogSet.FirstOrDefault
                (log => log.type == "setDeleted" && log.typeModifiedSubject == "companyCharacteristic" && log.IDModifiedSubject == id);

            return getCompanies;
        }

        public IEnumerable<Log> getUnDeletedCompanies(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getCompanies = objectContext.LogSet.Where(log => log.type == "unSetDeleted" && log.typeModifiedSubject == "company").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getCompanies;
        }

        public IEnumerable<Log> getCompanyEditLogs(Entities objectContext, long typeID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (typeID < 1)
            {
                throw new BusinessException("id is < 1");
            }

            IEnumerable<Log> getLogs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "company" && log.IDModifiedSubject == typeID).
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getLogs;
        }

        public IEnumerable<Log> getCompanyCharacteristicEditLogs(Entities objectContext, long typeID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (typeID < 1)
            {
                throw new BusinessException("id is < 1");
            }

            IEnumerable<Log> getLogs = objectContext.LogSet.Where
                (log => log.typeModifiedSubject == "companyCharacteristic" && log.IDModifiedSubject == typeID).
                OrderByDescending<Log, long>(new Func<Log, long>(IdSelectorLog));

            return getLogs;
        }

        public IEnumerable<Log> getCompanyCategoryEditLogs(Entities objectContext, long typeID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (typeID < 1)
            {
                throw new BusinessException("id is < 1");
            }

            IEnumerable<Log> getLogs = objectContext.LogSet.Where
                (log => log.typeModifiedSubject == "categoryCompany" && log.IDModifiedSubject == typeID).
                OrderByDescending<Log, long>(new Func<Log, long>(IdSelectorLog));

            return getLogs;
        }

        public IEnumerable<Log> getRegisteredUsers(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getUsers = objectContext.LogSet.Where(log => log.type == "create" && log.typeModifiedSubject == "user").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getUsers;
        }

        public IEnumerable<Log> getDeletedUsers(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<Log> getUsers = objectContext.LogSet.Where(log => log.type == "setDeleted" && log.typeModifiedSubject == "user").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getUsers;
        }

        public Log getDeletedUser(Entities objectContext, long userID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }
            Log getUser = objectContext.LogSet.FirstOrDefault(logs => logs.type == "setDeleted" &&
                    logs.IDModifiedSubject == userID && logs.typeModifiedSubject == "user");

            return getUser;
        }

        public IEnumerable<Log> getUserChanges(Entities objectContext, long typeID)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (typeID < 1)
            {
                throw new BusinessException("typeID is < 1");
            }

            IEnumerable<Log> getUsers = objectContext.LogSet.Where(log => log.IDModifiedSubject == typeID && log.typeModifiedSubject == "user").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getUsers;
        }

        public IEnumerable<Log> getUserRolesChanges(Entities objectContext, long typeID)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (typeID < 1)
            {
                throw new BusinessException("typeID is < 1");
            }

            IEnumerable<Log> getLogs = objectContext.LogSet.Where(log => log.IDModifiedSubject == typeID && log.typeModifiedSubject == "role").
                OrderByDescending<Log, long>
                (new Func<Log, long>(IdSelectorLog));

            return getLogs;
        }

        /// <summary>
        /// used for Sorting by Descending , sorts by id
        /// </summary>
        private long IdSelectorLog(Log log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("statistic");
            }
            return log.ID;
        }

        public void LogCompanyImage(Entities objectContext, CompanyImage image, LogType type, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (image == null)
            {
                throw new BusinessException("image is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:

                    if (currUser == null)
                    {
                        throw new BusinessException("currUser is null");
                    }
                    description = string.Format("Company image id : ' {0} ' was uploaded by {1}.", image.ID, currUser.username);
                    break;

                case LogType.delete:

                    if (currUser == null)
                    {
                        currUser = Tools.GetSystem();
                    }
                    description = string.Format("Company image id : ' {0} ' was removed by {1}.", image.ID, currUser.username);
                    break;

                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Company images.", type.ToString()));
            }

            BuildLog(objectContext, type, "companyImage", image.ID, description);
        }

        public void LogProductImage(Entities objectContext, ProductImage image, LogType type, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (image == null)
            {
                throw new BusinessException("image is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:

                    if (currUser == null)
                    {
                        throw new BusinessException("currUser is null");
                    }
                    description = string.Format("Product image id : ' {0} ' was uploaded by {1}.", image.ID, currUser.username);
                    break;

                case LogType.delete:

                    if (currUser == null)
                    {
                        currUser = Tools.GetSystem();
                    }
                    description = string.Format("Product image id : ' {0} ' was removed by {1}.", image.ID, currUser.username);
                    break;

                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Product images.", type.ToString()));
            }

            BuildLog(objectContext, type, "productImage", image.ID, description);
        }


        public void LogCategoryImage(Entities objectContext, Category category, LogType type, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (category == null)
            {
                throw new BusinessException("category is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:

                    if (currUser == null)
                    {
                        throw new BusinessException("currUser is null");
                    }
                    description = string.Format("Category image id : ' {0} ' was uploaded by {1}.", category.ID, currUser.username);
                    break;

                case LogType.delete:

                    if (currUser == null)
                    {
                        currUser = Tools.GetSystem();
                    }
                    description = string.Format("Category image id : ' {0} ' was removed by {1}.", category.ID, currUser.username);
                    break;

                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Category images.", type.ToString()));
            }

            BuildLog(objectContext, type, "category", category.ID, description);
        }


        /// <summary>
        /// Used when systems deletes image (from table) because it doesnt exist
        /// </summary>
        public void SystemDeleteProductImageLog(Entities objectContext, ProductImage image)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (image == null)
            {
                throw new BusinessException("image is null.");
            }
            System.Text.StringBuilder descr = new StringBuilder();

            descr.Append("System is deleting ProductImage with ID ");
            descr.Append(image.ID);

            if (image.url == "")
            {
                descr.Append(" , because url : ");
                descr.Append(image.url);
                descr.Append(" hasnt changed for last5 minutes.");
            }
            else
            {
                descr.Append(" , because file with url : ");
                descr.Append(image.url);
                descr.Append(" doesnt exist.");
            }

            Log newLog = new Log();
            newLog.dateCreated = DateTime.UtcNow;
            newLog.description = descr.ToString();
            newLog.type = "setDeleted";
            newLog.typeModifiedSubject = "productImage";
            newLog.IDModifiedSubject = image.ID;
            newLog.UserID = Tools.GetUserID(objectContext, Tools.GetSystem());
            newLog.userIPadress = "127.0.0.1";

            Add(objectContext, newLog);
        }

        /// <summary>
        /// Used when systems deletes image (from table) because it doesnt exist
        /// </summary>
        public void SystemDeleteCompanyImageLog(Entities objectContext, CompanyImage image)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (image == null)
            {
                throw new BusinessException("image is null.");
            }

            System.Text.StringBuilder descr = new StringBuilder();

            descr.Append("System is deleting CompanyImage with ID ");
            descr.Append(image.ID);

            if (image.url == "")
            {
                descr.Append(" , because url : ");
                descr.Append(image.url);
                descr.Append(" hasnt changed for last5 minutes.");

            }
            else
            {
                descr.Append(" , because file with url : ");
                descr.Append(image.url);
                descr.Append(" doesnt exist.");
            }

            Log newLog = new Log();
            newLog.dateCreated = DateTime.UtcNow;
            newLog.description = descr.ToString();
            newLog.type = "setDeleted";
            newLog.typeModifiedSubject = "companyImage";
            newLog.IDModifiedSubject = image.ID;
            newLog.UserID = Tools.GetUserID(objectContext, Tools.GetSystem());
            newLog.userIPadress = "127.0.0.1";

            Add(objectContext, newLog);

        }

        public void LogReport(Entities objectContext, Report report, LogType type, string field, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (report == null)
            {
                throw new BusinessException("report is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string description = string.Empty;

            switch (type)
            {
                case LogType.create:
                    string aboutTypeId = string.Empty;
                    if (report.reportType != "general")
                    {
                        aboutTypeId = string.Format(", ID : {0}", report.aboutTypeId);
                    }

                    description = string.Format("Report type ' {0} ', about type ' {1} '{2}, was written by {3}"
                        , report.reportType, report.aboutType, aboutTypeId, currUser.username);
                    break;
                case LogType.edit:
                    description = string.Format("Report ID : {0} , field : {1}, was modified by {2}, old value was : false"
                        , report.ID, field, currUser.username);
                    break;
                default:
                    throw new BusinessException(string.Format("LogType = {0} is not supported for Reports."));
            }

            BuildLog(objectContext, type, "report", report.ID, description);
        }

        /// <summary>
        /// Return logs sorted by params
        /// </summary>
        /// <param name="logType">all,create,edit,setDeleted,unSetDeleted</param>
        public List<Log> GetLogs(Entities objectContext, String logType, String aboutType, long aboutTypeId, int numOfLogs, long fromUserId)
        {
            CheckData(objectContext, logType, aboutType, aboutTypeId, numOfLogs, fromUserId);
            List<Log> Logs = new List<Log>();

            Logs = SortByAboutType(objectContext, aboutType, aboutTypeId);
            Logs = SortByLogType(Logs, logType);
            Logs = SortByUserId(objectContext, Logs, fromUserId);
            Logs = SortByNumber(Logs, numOfLogs);

            return Logs;
        }

        private List<Log> SortByUserId(Entities objectContext, List<Log> Logs, long fromUserId)
        {
            if (fromUserId < 0)
            {
                throw new BusinessException("fromUserId < 0");
            }

            long count = Logs.Count<Log>();

            if (count > 0 && fromUserId != 0)
            {
                List<Log> SortedList = new List<Log>();

                BusinessUser businessUser = new BusinessUser();
                User user = businessUser.GetWithoutVisible(fromUserId, false);
                if (user != null)
                {
                    UserID userid = Tools.GetUserID(objectContext, user);

                    foreach (Log log in Logs)
                    {
                        if (log.UserID == userid)
                        {
                            SortedList.Add(log);
                        }
                    }

                    return SortedList;
                }
                else
                {
                    return SortedList;
                }
            }
            else
            {
                return Logs;
            }
        }

        private List<Log> SortByNumber(List<Log> Logs, int numOfLogs)
        {

            if (numOfLogs < 1)
            {
                throw new BusinessException("numOfLogs is < 1");
            }

            long count = Logs.Count<Log>();

            if (count > 1)
            {

                if (numOfLogs >= count)
                {
                    return Logs;
                }
                else
                {
                    List<Log> SortedList = new List<Log>();

                    for (int i = 0; i < numOfLogs; i++)
                    {
                        SortedList.Add(Logs[i]);
                    }

                    return SortedList;
                }
            }
            else
            {
                return Logs;
            }

        }

        private List<Log> SortByLogType(List<Log> Logs, String logType)
        {
            if (string.IsNullOrEmpty(logType))
            {
                throw new BusinessException("logType is null or empty");
            }

            if (Logs.Count<Log>() > 0)
            {
                List<Log> SortedList = new List<Log>();

                switch (logType)
                {
                    case ("all"):
                        return Logs;
                    case ("create"):
                        foreach (Log log in Logs)
                        {
                            if (log.type == logType)
                            {
                                SortedList.Add(log);
                            }
                        }
                        break;
                    case ("edit"):
                        foreach (Log log in Logs)
                        {
                            if (log.type == logType)
                            {
                                SortedList.Add(log);
                            }
                        }
                        break;
                    case ("setDeleted"):
                        foreach (Log log in Logs)
                        {
                            if (log.type == logType)
                            {
                                SortedList.Add(log);
                            }
                        }
                        break;
                    case ("unSetDeleted"):
                        foreach (Log log in Logs)
                        {
                            if (log.type == logType)
                            {
                                SortedList.Add(log);
                            }
                        }
                        break;
                    default:
                        string error = string.Format("logType = " + logType + " is not valid type.");
                        throw new BusinessException(error);
                }

                return SortedList;

            }
            else
            {
                return Logs;
            }
        }

        private List<Log> SortByAboutType(Entities objectContext, String aboutType, long aboutTypeId)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Log> logs;

            switch (aboutType)
            {
                case ("all"):
                    logs = objectContext.LogSet.OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("product"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "product" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("prodCharacteristic"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productCharacteristic" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aProducts"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "product").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aProdCharacteristics"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productCharacteristic").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aProdImages"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productImage").
                         OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("company"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "company" &&
                        log.IDModifiedSubject == aboutTypeId).
                         OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("compCharacteristic"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "companyCharacteristic" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("compCategory"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "categoryCompany" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aCompanies"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "company").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aCompCharacteristics"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "companyCharacteristic").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aCompCategories"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "categoryCompany").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aCompImages"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "companyImage").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("category"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "category" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aCategories"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "category").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aProductTopics"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productTopic").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("productTopic"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productTopic" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("user"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "user" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aUsers"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "user").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aUsrRoles"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "role").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aUsrTransfers"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "roleTransfer").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("userTransfers"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "roleTransfer" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aRoleTakings"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "roleTaking").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("roleTakings"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "roleTaking" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("report"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "report" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("notify"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "notify" &&
                        log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aReports"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "report").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aNotifies"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "notify").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("siteText"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "siteText" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("aSiteTexts"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "siteText").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("suggestion"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "suggestion" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("aSuggestions"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "suggestion").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("advertisement"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "advertisement" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("aAdvertisements"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "advertisement").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("usersRoles"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "role" && log.IDModifiedSubject == aboutTypeId).
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aComments"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "comment").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aCompTypes"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "companyType").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aRateComments"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "rateComment").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("aRateProducts"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "rateProduct").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("companyType"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "companyType" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("ipBan"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "IpBan" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("aIpBans"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "IpBan").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("productVariant"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productVariant" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("aProductVariants"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productVariant").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("productSubVariant"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productSubVariant" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("aProductSubVariants"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productSubVariant").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("typeSuggestion"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "typeSuggestion" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("aTypeSuggestions"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "typeSuggestion").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("altCompanyName"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "alternativeCompanyName" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("compsAlternativeNames"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "alternativeCompanyName").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("altProductyName"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "alternativeProductName" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("prodsAlternativeNames"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "alternativeProductName").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                case ("productLink"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productLink" &&
                        log.IDModifiedSubject == aboutTypeId);
                    break;
                case ("aProductLinks"):
                    logs = objectContext.LogSet.Where(log => log.typeModifiedSubject == "productLink").
                        OrderByDescending<Log, long>(new Func<Log, long>(IdSelector));
                    break;
                // add new types here
                default:
                    string error = string.Format("aboutType = {0} is not valid type", aboutType);
                    throw new BusinessException(error);
            }

            List<Log> SortedLogs = new List<Log>();
            foreach (Log log in logs)
            {
                SortedLogs.Add(log);
            }

            return SortedLogs;
        }

        private void CheckData(Entities objectContext, String logType, String aboutType, long aboutTypeId, int numOfLogs, long fromUserId)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (string.IsNullOrEmpty(logType))
            {
                throw new BusinessException("logType is null or empty");
            }

            if (string.IsNullOrEmpty(aboutType))
            {
                throw new BusinessException("aboutType is null or empty");
            }

            if (aboutTypeId < 1)
            {
                throw new BusinessException("aboutTypeId is < 1");
            }

            if (numOfLogs < 1)
            {
                throw new BusinessException("numOfLogs is < 1");
            }

            if (fromUserId < 0)
            {
                throw new BusinessException("fromUserId < 0");
            }
        }

        private long IdSelector(Log log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            return log.ID;
        }

        public List<Log> GetLogsWithIpAdress(Entities objectContext, string IpAdress, long number)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(IpAdress))
            {
                throw new BusinessException("IpAdress is empty");
            }

            List<Log> logs = objectContext.GetLogsWithIP(IpAdress, number).ToList();

            return logs;
        }

        private static string GetLogTypeAsString(LogType type)
        {
            string str = string.Empty;

            switch (type)
            {
                case LogType.create:
                    str = "create";
                    break;
                case LogType.delete:
                    str = "setDeleted";
                    break;
                case LogType.edit:
                    str = "edit";
                    break;
                case LogType.undelete:
                    str = "unSetDeleted";
                    break;
                default:
                    throw new BusinessException(string.Format("type = {0} is not supported type.", type.ToString()));
            }

            return str;
        }


    }
}
