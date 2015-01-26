// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessNotifies
    {
        object updatingNotifies = new object();
        object removingNotifies = new object();

        private void Add(Entities objectContext, User currUser, BusinessLog bLog, NotifyOnNewContent newNotify)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (newNotify == null)
            {
                throw new BusinessException("newNotify is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            objectContext.AddToNotifyOnNewContentSet(newNotify);
            Tools.Save(objectContext);

            bLog.LogNotify(objectContext, newNotify, LogType.create, currUser);
        }

        public void AddNewNotify(Entities objectContext, BusinessLog bLog, User currUser, long typeID, NotifyType type)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (typeID < 1)
            {
                throw new BusinessException("typeID < 1");
            }

            string strType = GetTypeString(type);

            switch (type)
            {
                case NotifyType.Company:
                    BusinessCompany businessCompany = new BusinessCompany();
                    Company currCompany = businessCompany.GetCompany(objectContext, typeID);
                    if (currCompany == null)
                    {
                        throw new BusinessException(string.Format("User ID : {0}, cannot be notified for updates on Company ID : {1} because there isn`t such or it is visible=false"
                            , currUser.ID, typeID));
                    }
                    break;
                case NotifyType.Product:
                    BusinessProduct businessProduct = new BusinessProduct();
                    Product currProduct = businessProduct.GetProductByID(objectContext, typeID);
                    if (currProduct == null)
                    {
                        throw new BusinessException(string.Format("User ID : {0}, cannot be notified for updates on Product ID : {1} because there isn`t such or it is visible=false"
                            , currUser.ID, typeID));
                    }
                    break;
                case NotifyType.ProductForum:
                    BusinessProduct bProduct = new BusinessProduct();
                    Product cProduct = bProduct.GetProductByID(objectContext, typeID);
                    if (cProduct == null)
                    {
                        throw new BusinessException(string.Format("User ID : {0}, cannot be notified for updates on Product Forum ID : {1} because there isn`t such or it is visible=false"
                            , currUser.ID, typeID));
                    }
                    break;
                case NotifyType.ProductTopic:
                    BusinessProductTopics bTopics = new BusinessProductTopics();
                    ProductTopic topic = bTopics.Get(objectContext, typeID, true, false);
                    if (topic == null)
                    {
                        throw new BusinessException(string.Format("User ID : {0}, cannot be notified for updates on Product Topic ID : {1} because there isn`t such or it is visible=false"
                            , currUser.ID, typeID));
                    }
                    else if (topic.locked == true)
                    {
                        throw new BusinessException(string.Format("User ID : {0}, cannot be notified for updates on Product Topic ID : {1} because the topic is locked"
                            , currUser.ID, typeID));
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("NotifyType = {0} is not supported type when adding new notifies", type));

            }

            NotifyOnNewContent test = Get(objectContext, currUser, type, typeID, false);
            if (test == null)
            {
                NotifyOnNewContent newNotify = new NotifyOnNewContent();
                newNotify.active = true;
                newNotify.dateCreated = DateTime.UtcNow;
                newNotify.lastModified = newNotify.dateCreated;
                newNotify.ModifiedBy = Tools.GetUserID(objectContext, currUser);
                newNotify.User = newNotify.ModifiedBy;
                newNotify.type = strType;
                newNotify.typeID = typeID;
                newNotify.newInformation = false;

                Add(objectContext, currUser, bLog, newNotify);
            }
            else
            {
                if (test.active == true)
                {
                    throw new BusinessException(string.Format("User ID : {0} cannot add notify for {1} ID : {2}, because there is already such notify in database"
                        , currUser.ID, type, typeID));
                }
                else
                {
                    ActivateNotify(objectContext, bLog, currUser, test);
                }
            }
        }

        private void ActivateNotify(Entities objectContext, BusinessLog bLog, User currUser, NotifyOnNewContent currNotify)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currNotify == null)
            {
                throw new BusinessException("currNotify is null");
            }

            if (currNotify.active == true)
            {
                throw new BusinessException("currNotify is active");
            }

            currNotify.active = true;
            currNotify.newInformation = false;
            currNotify.lastModified = DateTime.UtcNow;
            currNotify.ModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogNotify(objectContext, currNotify, LogType.undelete, currUser);
        }

        /// <summary>
        /// Removes (set to active.false) all user notifications. Used when user is being deleted.
        /// </summary>
        public void RemoveAllUserNotifies(EntitiesUsers userContext, Entities objectContext, BusinessLog bLog, User forUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }

            List<NotifyOnNewContent> notifies = GetAllUserNotifies(objectContext, forUser, true);
            if (notifies != null && notifies.Count > 0)
            {
                BusinessUser bUser = new BusinessUser();
                User system = bUser.GetSystem(userContext);

                foreach (NotifyOnNewContent notify in notifies)
                {
                    RemoveNotify(objectContext, bLog, system, notify);
                }

            }

        }

        public void RemoveNotify(Entities objectContext, BusinessLog bLog, User currUser, NotifyOnNewContent currNotify)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currNotify == null)
            {
                throw new BusinessException("currNotify is null");
            }

            if (currNotify.active == false)
            {
                throw new BusinessException("currNotify is not active");
            }

            currNotify.active = false;
            currNotify.lastModified = DateTime.UtcNow;
            currNotify.ModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogNotify(objectContext, currNotify, LogType.delete, currUser);

        }

        public NotifyOnNewContent Get(Entities objectContext, User currUser, NotifyType type, long typeID, bool checkForActive)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            string strType = GetTypeString(type);

            NotifyOnNewContent currNotify = null;

            if (checkForActive)
            {
                currNotify = objectContext.NotifyOnNewContentSet.FirstOrDefault(notif => notif.User.ID == currUser.ID
                    && notif.typeID == typeID && notif.type == strType && notif.active == true);
            }
            else
            {
                currNotify = objectContext.NotifyOnNewContentSet.FirstOrDefault
                    (notif => notif.User.ID == currUser.ID && notif.typeID == typeID && notif.type == strType);
            }

            return currNotify;
        }

        public NotifyOnNewContent Get(Entities objectContext, long Id)
        {
            Tools.AssertObjectContextExists(objectContext);

            NotifyOnNewContent currNotify = objectContext.NotifyOnNewContentSet.FirstOrDefault(notif => notif.ID == Id);

            return currNotify;
        }

        public void UpdateNotifiesForType(Entities objectContext, NotifyType notifType, long typeID, User currentUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }

            if (typeID < 1)
            {
                throw new BusinessException("typeID < 1");
            }

            List<NotifyOnNewContent> notifies = GetNotifiesForType(objectContext, notifType, typeID, true);

            if (notifies.Count > 0)
            {
                BusinessUserOptions bUserOptions = new BusinessUserOptions();

                lock (updatingNotifies)
                {
                    foreach (NotifyOnNewContent notify in notifies)
                    {
                        if (!notify.UserReference.IsLoaded)
                        {
                            notify.UserReference.Load();
                        }

                        if (notify.User.ID != currentUser.ID)
                        {
                            notify.newInformation = true;
                            Tools.Save(objectContext);

                            bUserOptions.ChangeIfUserHaveNewContentOnNotifies(objectContext, notify.User, true);
                        }
                    }
                }
            }
        }

        public void RemoveNotifiesForType(Entities objectContext, EntitiesUsers userContext
            , BusinessLog bLog, NotifyType notifType, long typeID)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (typeID < 1)
            {
                throw new BusinessException("typeID < 1");
            }

            List<NotifyOnNewContent> notifies = GetNotifiesForType(objectContext, notifType, typeID, false);

            if (notifies.Count > 0)
            {
                BusinessUser businessUser = new BusinessUser();
                User system = businessUser.GetSystem(userContext);

                BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                User notifyUser = null;
                string description = "";
                switch (notifType)
                {
                    case NotifyType.Product:
                        BusinessProduct businessProduct = new BusinessProduct();
                        Product product = businessProduct.GetProductByIDWV(objectContext, typeID);
                        if (product == null)
                        {
                            throw new BusinessException(string.Format("There is no product with ID : {0} for which notifies should be deleted"
                                , typeID));
                        }

                        description = string.Format("{0} \" {1} \" {2}", Tools.GetResource("notifRemovedProduct")
                            , product.name, Tools.GetResource("notifRemovedProduct2"));

                        break;

                    case NotifyType.Company:
                        BusinessCompany businessCompany = new BusinessCompany();
                        Company company = businessCompany.GetCompanyWV(objectContext, typeID);
                        if (company == null)
                        {
                            throw new BusinessException(string.Format("There is no company with ID : {0} for which notifies should be deleted"
                               , typeID));
                        }

                        description = string.Format("{0} \" {1} \" {2}", Tools.GetResource("notifRemovedMaker")
                            , company.name, Tools.GetResource("notifRemovedMaker2"));

                        break;

                    case NotifyType.ProductForum:
                        BusinessProduct bProduct = new BusinessProduct();
                        Product prod = bProduct.GetProductByIDWV(objectContext, typeID);
                        if (prod == null)
                        {
                            throw new BusinessException(string.Format("There is no product (forum) with ID : {0} for which notifies should be deleted"
                                , typeID));
                        }

                        description = string.Format("{0} \" {1} \" {2}", Tools.GetResource("notifRemovedProductForum")
                            , prod.name, Tools.GetResource("notifRemovedProductForum2"));
                        break;

                    case NotifyType.ProductTopic:
                        BusinessProductTopics bTopic = new BusinessProductTopics();
                        ProductTopic topic = bTopic.Get(objectContext, typeID, false, false);
                        if (topic == null)
                        {
                            throw new BusinessException(string.Format("There is no product topic with ID : {0} for which notifies should be deleted"
                                , typeID));
                        }

                        description = string.Format("{0} \" {1} \" {2}", Tools.GetResource("notifRemovedProductTopic")
                            , topic.name, Tools.GetResource("notifRemovedProductTopic2"));
                        break;

                    default:
                        throw new BusinessException(string.Format("NotifyType = {0} is not supported for System Messages."));
                }

                lock (removingNotifies)
                {
                    foreach (NotifyOnNewContent notify in notifies)
                    {
                        if (!notify.UserReference.IsLoaded)
                        {
                            notify.UserReference.Load();
                        }
                        notifyUser = businessUser.GetWithoutVisible(userContext, notify.User.ID, true);
                        if (businessUser.IsFromUserTeam(notifyUser))
                        {
                            bSystemMessages.Add(userContext, notifyUser, description);
                        }
                        RemoveNotify(objectContext, bLog, system, notify);
                    }
                }
            }
        }

        /// <summary>
        /// Returns active notifies for type (checkForNewInfo = true if should get only those which newInformation = false)
        /// </summary>
        private List<NotifyOnNewContent> GetNotifiesForType(Entities objectContext, NotifyType notifType, long typeID, bool checkForNewInfo)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (typeID < 1)
            {
                throw new BusinessException("typeID < 1");
            }

            string type = GetTypeString(notifType);

            List<NotifyOnNewContent> notifies = new List<NotifyOnNewContent>();


            if (checkForNewInfo)
            {
                notifies = objectContext.NotifyOnNewContentSet.Where
                 (notif => notif.type == type && notif.typeID == typeID && notif.active == true && notif.newInformation == false).ToList();
            }
            else
            {
                notifies = objectContext.NotifyOnNewContentSet.Where
                 (notif => notif.type == type && notif.typeID == typeID && notif.active == true).ToList();
            }

            return notifies;
        }

        /// <summary>
        /// Counts the number of user notifications which have new information
        /// </summary>
        public int CountUserNotifiesWhichHaveNewInformation(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            int count = 0;
            count = objectContext.NotifyOnNewContentSet.Count(notif => notif.User.ID == currUser.ID
                && notif.active == true && notif.newInformation == true);

            return count;
        }

        private void SetNewInformationAsFalse(Entities objectContext, User currUser, NotifyOnNewContent currNotify)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currNotify == null)
            {
                throw new BusinessException("currNotify is null");
            }

            if (currNotify.newInformation == false)
            {
                throw new BusinessException(string.Format("User ID : {0} cannot set Notify ID : {1} new information as false, because it is already."
                    , currUser.ID, currNotify.ID));
            }

            currNotify.newInformation = false;
            currNotify.ModifiedBy = Tools.GetUserID(objectContext, currUser);
            currNotify.lastModified = DateTime.UtcNow;

            Tools.Save(objectContext);

            //if user doesn`t have any notifies with new information - hide the icon for notifies
            if (CountUserNotifiesWhichHaveNewInformation(objectContext, currUser) < 1)
            {
                BusinessUserOptions bUserOptions = new BusinessUserOptions();
                bUserOptions.ChangeIfUserHaveNewContentOnNotifies(objectContext, currUser, false);
            }
        }

        public List<NotifyOnNewContent> GetAllUserNotifies(Entities objectContext, User currUser, bool checkForActiveTrue)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            List<NotifyOnNewContent> notifies = new List<NotifyOnNewContent>();

            if (checkForActiveTrue == true)
            {
                notifies = objectContext.NotifyOnNewContentSet.Where
                    (notif => notif.User.ID == currUser.ID && notif.active == true).ToList();
            }
            else
            {
                notifies = objectContext.NotifyOnNewContentSet.Where
                    (notif => notif.User.ID == currUser.ID).ToList();
            }

            return notifies;
        }

        public List<NotifyOnNewContent> GetUserNotifiesWithNewInformation(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            List<NotifyOnNewContent> notifies = objectContext.NotifyOnNewContentSet.Where
                (notif => notif.User.ID == currUser.ID && notif.active == true && notif.newInformation == true).ToList();

            return notifies;
        }

        public List<NotifyOnNewContent> GetUserNotifiesWithOutNewInformation(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            List<NotifyOnNewContent> notifies = objectContext.NotifyOnNewContentSet.Where
                (notif => notif.User.ID == currUser.ID && notif.active == true && notif.newInformation == false).ToList();

            return notifies;
        }

        private string GetTypeString(NotifyType type)
        {
            string strType = string.Empty;

            switch (type)
            {
                case NotifyType.Company:
                    strType = "company";
                    break;
                case NotifyType.Product:
                    strType = "product";
                    break;
                case NotifyType.ProductForum:
                    strType = "productForum";
                    break;
                case NotifyType.ProductTopic:
                    strType = "roductTopic";
                    break;
                default:
                    throw new BusinessException(string.Format("NotifyType {0} is not supported type", type));
            }

            return strType;
        }

        /// <summary>
        /// Sets new information false on user notify for product if found, returns true if user have active notify for product otherwise false
        /// </summary>
        public bool SetNewInformationFalseForProductIfUserIsSigned(Entities objectContext, User currUser, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            bool result = false;

            NotifyOnNewContent currNotify = Get(objectContext, currUser, NotifyType.Product, currProduct.ID, true);
            if (currNotify != null)
            {
                if (currNotify.newInformation == true)
                {
                    SetNewInformationAsFalse(objectContext, currUser, currNotify);
                }

                result = true;

            }

            return result;
        }

        public bool IsMaxNotificationsNumberReached(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            bool result = false;

            long count = objectContext.NotifyOnNewContentSet.Count
                (notif => notif.User.ID == currUser.ID && notif.active == true);

            if (count >= Configuration.NotifiesMaxNumberNotifies)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Sets new information false on user notify for company if found, returns true if user have active notify for company otherwise false
        /// </summary>
        public bool SetNewInformationFalseForCompanyIfUserIsSigned(Entities objectContext, User currUser, Company currCompany)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            bool result = false;

            NotifyOnNewContent currNotify = Get(objectContext, currUser, NotifyType.Company, currCompany.ID, true);
            if (currNotify != null)
            {

                if (currNotify.newInformation == true)
                {
                    SetNewInformationAsFalse(objectContext, currUser, currNotify);
                }

                result = true;
            }

            return result;
        }

        public bool SetNewInformationFalseForProductForumIfUserIsSigned(Entities objectContext, User currUser, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            bool result = false;

            NotifyOnNewContent currNotify = Get(objectContext, currUser, NotifyType.ProductForum, product.ID, true);
            if (currNotify != null)
            {

                if (currNotify.newInformation == true)
                {
                    SetNewInformationAsFalse(objectContext, currUser, currNotify);
                }

                result = true;
            }

            return result;
        }

        public bool SetNewInformationFalseForProductTopicIfUserIsSigned(Entities objectContext, User currUser, ProductTopic topic)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            bool result = false;

            NotifyOnNewContent currNotify = Get(objectContext, currUser, NotifyType.ProductTopic, topic.ID, true);
            if (currNotify != null)
            {

                if (currNotify.newInformation == true)
                {
                    SetNewInformationAsFalse(objectContext, currUser, currNotify);
                }

                result = true;
            }

            return result;
        }

        public static NotifyType GetTypeFromString(NotifyOnNewContent currNotify)
        {
            if (currNotify == null)
            {
                throw new BusinessException("currNotify is null");
            }

            NotifyType notifType = NotifyType.Company;

            string type = currNotify.type;

            switch (type)
            {
                case "product":
                    notifType = NotifyType.Product;
                    break;
                case "company":
                    notifType = NotifyType.Company;
                    break;
                case "productForum":
                    notifType = NotifyType.ProductForum;
                    break;
                case "roductTopic":
                    notifType = NotifyType.ProductTopic;
                    break;
                default:
                    throw new BusinessException(string.Format("Notify type {0} is not supported type", type));
            }

            return notifType;
        }

    }
}
