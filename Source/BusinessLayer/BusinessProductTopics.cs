// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessProductTopics
    {
        private static object commentsSync = new object();

        public void AddTopic(Entities objectContext, EntitiesUsers userContext, User byUser, Product forProduct
            , string name, string description, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (byUser == null)
            {
                throw new BusinessException("byUser is null");
            }

            if (forProduct == null)
            {
                throw new BusinessException("forProduct is null");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }
            else if (name.Length > 200)
            {
                throw new BusinessException("name length is more than 200");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null or empty");
            }

            if (forProduct.visible == false)
            {
                throw new BusinessException(string.Format("User id = {0} cannot create topic for product id = {1}, because it`s visible=false"
                    , byUser.ID, forProduct.ID));
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.CanUserDo(userContext, byUser, UserRoles.WriteCommentsAndMessages))
            {
                throw new BusinessException(string.Format("User id = {0} cannot create new product topics", byUser.ID));
            }

            ProductTopic newTopic = new ProductTopic();

            newTopic.Product = forProduct;
            newTopic.User = Tools.GetUserID(objectContext, byUser);

            newTopic.name = name;
            newTopic.description = description;

            newTopic.dateCreated = DateTime.UtcNow;
            newTopic.lastModified = newTopic.dateCreated;
            newTopic.LastModifiedBy = newTopic.User;

            newTopic.lastCommentDate = newTopic.dateCreated;
            newTopic.LastCommentBy = newTopic.User;
            newTopic.comments = 0;
            newTopic.visible = true;
            newTopic.locked = false;
            newTopic.visits = 0;

            objectContext.AddToProductTopicSet(newTopic);
            Tools.Save(objectContext);

            // log new topic created
            bLog.LogProductTopic(objectContext, newTopic, LogType.create, string.Empty, string.Empty, byUser);

            BusinessNotifies businessNotifies = new BusinessNotifies();
            businessNotifies.UpdateNotifiesForType(objectContext, NotifyType.ProductForum, forProduct.ID, byUser);

            BusinessStatistics bStatistics = new BusinessStatistics();
            bStatistics.TopicAdded(userContext, objectContext);
        }

        public List<ProductTopic> GetProductTopics(Entities objectContext, Product product, long from, long to,
            bool sortedByLastCommentDate)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            List<ProductTopic> topics = objectContext.GetProductTopics(product.ID, from, to, sortedByLastCommentDate).ToList();

            return topics;
        }

        public void UpdateTopicDescription(Entities objectContext, EntitiesUsers userContext, User byUser, BusinessLog bLog
            , ProductTopic topic, string newDescription)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            if (topic.visible == false)
            {
                throw new BusinessException(string.Format("User id = {0} cannot modify productTopic id = {1}, because it`s visible=false"
                    , byUser.ID, topic.ID));
            }

            if (string.IsNullOrEmpty(newDescription))
            {
                throw new BusinessException("new description is null or empty");
            }

            if (!topic.UserReference.IsLoaded)
            {
                topic.UserReference.Load();
            }

            BusinessUser bUser = new BusinessUser();

            if (topic.User.ID != byUser.ID)
            {
                if (!bUser.IsFromAdminTeam(byUser))
                {
                    throw new BusinessException(string.Format("User id = {0} cannot modify topic id = {1} description because he is not the who created it and he isn`t admin"
                        , byUser.ID, topic.ID));
                }
            }

            if (topic.description == newDescription)
            {
                return;
            }

            string oldDescription = topic.description;

            topic.description = newDescription;
            topic.lastModified = DateTime.UtcNow;
            topic.LastModifiedBy = Tools.GetUserID(objectContext, byUser);
            Tools.Save(objectContext);

            bLog.LogProductTopic(objectContext, topic, LogType.edit, "description", oldDescription, byUser);

        }

        public void UpdateTopicName(Entities objectContext, EntitiesUsers userContext, User byUser, BusinessLog bLog
            , ProductTopic topic, string newName)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            if (topic.visible == false)
            {
                throw new BusinessException(string.Format("User id = {0} cannot modify productTopic id = {1}, because it`s visible=false"
                    , byUser.ID, topic.ID));
            }

            if (string.IsNullOrEmpty(newName))
            {
                throw new BusinessException("new name is null or empty");
            }
            else if (newName.Length > 200)
            {
                throw new BusinessException("newName length is more than 200");
            }

            if (!topic.UserReference.IsLoaded)
            {
                topic.UserReference.Load();
            }

            BusinessUser bUser = new BusinessUser();

            if (topic.User.ID != byUser.ID)
            {
                if (!bUser.IsFromAdminTeam(byUser))
                {
                    throw new BusinessException(string.Format("User id = {0} cannot modify topic id = {1} description because he is not the who created it and he isn`t admin"
                        , byUser.ID, topic.ID));
                }
            }

            if (topic.name == newName)
            {
                return;
            }

            string oldName = topic.name;

            topic.name = newName;
            topic.lastModified = DateTime.UtcNow;
            topic.LastModifiedBy = Tools.GetUserID(objectContext, byUser);
            Tools.Save(objectContext);

            bLog.LogProductTopic(objectContext, topic, LogType.edit, "name", oldName, byUser);
        }

        public void LockTopic(Entities objectContext, EntitiesUsers userContext, User byUser, BusinessLog bLog
           , ProductTopic topic, bool sendWarning)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            if (topic.locked == true)
            {
                return;
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.IsFromAdminTeam(byUser))
            {
                throw new BusinessException(string.Format("User id = {0} cannot lock topic id = {1}, because he is not admin."
                    , byUser.ID, topic.ID));
            }

            topic.locked = true;
            topic.lastModified = DateTime.UtcNow;
            topic.LastModifiedBy = Tools.GetUserID(objectContext, byUser);

            Tools.Save(objectContext);

            bLog.LogProductTopic(objectContext, topic, LogType.edit, "locked", "false", byUser);

            if (!topic.UserReference.IsLoaded)
            {
                topic.UserReference.Load();
            }
            if (!topic.ProductReference.IsLoaded)
            {
                topic.ProductReference.Load();
            }

            User userCreatedTopic = bUser.GetWithoutVisible(userContext, topic.User.ID, true);
            string description = string.Format("{0} \" {1} \" {2} \" {3} \" {4}", Tools.GetResource("ProductTopicLocked")
                , topic.name, Tools.GetResource("ProductTopicLocked2")
                , topic.Product.name, Tools.GetResource("ProductTopicLocked3"));

            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
            bSystemMessages.Add(userContext, userCreatedTopic, description);

            if (sendWarning == true)
            {
                BusinessUserActions buActions = new BusinessUserActions();
                UserAction action = buActions.GetUserAction(userContext, UserRoles.WriteCommentsAndMessages, userCreatedTopic);
                if (action != null && action.visible == true)
                {
                    BusinessWarnings bWarning = new BusinessWarnings();

                    string reason = string.Format("{0}<br />{1} \" {2} \" {3} \" {4} \".", Tools.GetResource("LockingProductTopicW")
                        , Tools.GetResource("LockingProductTopicW2"), topic.name
                        , Tools.GetResource("LockingProductTopicW3"), topic.Product.name);

                    UserRoles forRole = UserRoles.WriteCommentsAndMessages;
                    bWarning.AddWarning(userContext, objectContext, action, forRole.ToString(), reason, userCreatedTopic, byUser, bLog);
                }
            }

            BusinessNotifies bNotifies = new BusinessNotifies();
            bNotifies.RemoveNotifiesForType(objectContext, userContext, bLog, NotifyType.ProductTopic, topic.ID);

        }

        public void UnlockTopic(Entities objectContext, EntitiesUsers userContext, User byUser, BusinessLog bLog
           , ProductTopic topic)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            if (topic.locked == false)
            {
                return;
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.IsFromAdminTeam(byUser))
            {
                throw new BusinessException(string.Format("User id = {0} cannot unlock topic id = {1}, because he is not admin."
                    , byUser.ID, topic.ID));
            }

            topic.locked = false;
            topic.lastModified = DateTime.UtcNow;
            topic.LastModifiedBy = Tools.GetUserID(objectContext, byUser);

            Tools.Save(objectContext);

            bLog.LogProductTopic(objectContext, topic, LogType.edit, "locked", "true", byUser);

        }

        public void DeleteTopic(Entities objectContext, EntitiesUsers userContext, User byUser, BusinessLog bLog
          , ProductTopic topic, bool sendWarning)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            if (topic.visible == false)
            {
                throw new BusinessException(string.Format("User id = {0} cannot delete productTopic id = {1}, because it`s visible=false"
                    , byUser.ID, topic.ID));
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.IsFromAdminTeam(byUser))
            {
                throw new BusinessException(string.Format("User id = {0} cannot delete topic id = {1}, because he is not admin."
                    , byUser.ID, topic.ID));
            }

            topic.visible = false;
            topic.lastModified = DateTime.UtcNow;
            topic.LastModifiedBy = Tools.GetUserID(objectContext, byUser);

            Tools.Save(objectContext);

            bLog.LogProductTopic(objectContext, topic, LogType.delete, string.Empty, string.Empty, byUser);

            BusinessStatistics bStatistics = new BusinessStatistics();
            bStatistics.TopicDeleted(userContext, objectContext);

            if (!topic.UserReference.IsLoaded)
            {
                topic.UserReference.Load();
            }
            if (!topic.ProductReference.IsLoaded)
            {
                topic.ProductReference.Load();
            }

            User userCreatedTopic = bUser.GetWithoutVisible(userContext, topic.User.ID, true);

            if (topic.User.ID != byUser.ID)
            {
                string description = string.Format("{0} \" {1} \" {2} \" {3} \" {4}", Tools.GetResource("ProductTopicDeleted")
                    , topic.name, Tools.GetResource("ProductTopicDeleted2")
                    , topic.Product.name, Tools.GetResource("ProductTopicDeleted3"));

                BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                bSystemMessages.Add(userContext, userCreatedTopic, description);
            }

            if (sendWarning == true)
            {
                BusinessUserActions buActions = new BusinessUserActions();
                UserAction action = buActions.GetUserAction(userContext, UserRoles.WriteCommentsAndMessages, userCreatedTopic);
                if (action != null && action.visible == true)
                {
                    BusinessWarnings bWarning = new BusinessWarnings();

                    string reason = string.Format("{0}<br />{1} \" {2} \" {3} \" {4} \".", Tools.GetResource("DeletingProductTopicW")
                        , Tools.GetResource("DeletingProductTopicW2"), topic.name
                        , Tools.GetResource("DeletingProductTopicW3"), topic.Product.name);

                    UserRoles forRole = UserRoles.WriteCommentsAndMessages;
                    bWarning.AddWarning(userContext, objectContext, action, forRole.ToString(), reason, userCreatedTopic, byUser, bLog);
                }
            }

            BusinessNotifies bNotifies = new BusinessNotifies();
            bNotifies.RemoveNotifiesForType(objectContext, userContext, bLog, NotifyType.ProductTopic, topic.ID);
        }

        public void UnDeleteTopic(Entities objectContext, EntitiesUsers userContext, User byUser, BusinessLog bLog
          , ProductTopic topic)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            if (topic.visible == true)
            {
                throw new BusinessException(string.Format("User id = {0} cannot undelete productTopic id = {1}, because it`s visible=true"
                    , byUser.ID, topic.ID));
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.IsFromAdminTeam(byUser))
            {
                throw new BusinessException(string.Format("User id = {0} cannot delete topic id = {1}, because he is not admin."
                    , byUser.ID, topic.ID));
            }

            topic.visible = true;
            topic.lastModified = DateTime.UtcNow;
            topic.LastModifiedBy = Tools.GetUserID(objectContext, byUser);

            Tools.Save(objectContext);

            bLog.LogProductTopic(objectContext, topic, LogType.undelete, string.Empty, string.Empty, byUser);
        }

        public ProductTopic Get(Entities objectContext, long id, bool onlyVisibleTrue, bool throwExceptIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            ProductTopic topic = null;

            if (onlyVisibleTrue == true)
            {
                topic = objectContext.ProductTopicSet.FirstOrDefault(pt => pt.ID == id && pt.visible == true);
            }
            else
            {
                topic = objectContext.ProductTopicSet.FirstOrDefault(pt => pt.ID == id);
            }

            if (topic == null && throwExceptIfNull == true)
            {
                if (onlyVisibleTrue == true)
                {
                    throw new BusinessException(string.Format("There is no product topic with id = {0}, which is visible=true."
                        , id));
                }
                else
                {
                    throw new BusinessException(string.Format("There is no product topic with id = {0}.", id));
                }
            }

            return topic;
        }

        public List<ProductTopic> GetUserTopics(Entities objectContext, User forUser, bool onlyVisibleTrue)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }

            List<ProductTopic> topics = new List<ProductTopic>();

            UserID user = Tools.GetUserID(objectContext, forUser);

            user.ProductTopicsCreated.Load();

            if (onlyVisibleTrue == true)
            {
                topics = user.ProductTopicsCreated.Where(pt => pt.visible == true).ToList();
            }
            else
            {
                topics = user.ProductTopicsCreated.ToList();
            }

            return topics;
        }

        public long CountProductTopics(Entities objectContext, Product product, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            long count = 0;

            if (onlyVisible == true)
            {
                count = objectContext.ProductTopicSet.Count(pt => pt.Product.ID == product.ID && pt.visible == true);
            }
            else
            {
                count = objectContext.ProductTopicSet.Count(pt => pt.Product.ID == product.ID);
            }

            return count;
        }

        /// <summary>
        /// Returns true if Configuration.TopicsTimeWhichNeedsToPassToAddAnother time passed after last added topic by user, always true for admins
        /// </summary>
        public bool CheckIfMinimumTimeBetweenAddingTopicsPassed(Entities objectContext, User currUser, out int minToWait)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser in null");
            }

            minToWait = 1;
            bool result = true;

            if (Configuration.TopicsTimeWhichNeedsToPassToAddAnother > 0)
            {
                BusinessUser businessUser = new BusinessUser();
                if (businessUser.IsFromAdminTeam(currUser))
                {
                    return true;
                }

                ProductTopic lastAddedByUser = null;
                List<ProductTopic> topicsAddedbyUser = objectContext.ProductTopicSet.Where(pt => pt.User.ID == currUser.ID).ToList();
                if (topicsAddedbyUser.Count > 0)
                {
                    lastAddedByUser = topicsAddedbyUser.Last();
                }

                if (lastAddedByUser != null)
                {
                    DateTime prodTime = lastAddedByUser.dateCreated;

                    TimeSpan span = DateTime.UtcNow - lastAddedByUser.dateCreated;
                    int minPassed = (int)span.TotalMinutes;

                    if (minPassed < Configuration.TopicsTimeWhichNeedsToPassToAddAnother)
                    {
                        result = false;

                        minToWait = Configuration.TopicsTimeWhichNeedsToPassToAddAnother - minPassed;
                        if (minToWait == 0)
                        {
                            minToWait = 1;
                        }
                    }
                }

            }

            return result;
        }

        /// <summary>
        /// Increases by 1 Topic`s comments
        /// </summary>
        public void IncreaseTopicComments(Entities objectContext, ProductTopic topic, Comment newComment)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (topic == null)
            {
                throw new BusinessException("topic is Null");
            }
            if (newComment == null)
            {
                throw new BusinessException("newComment is Null");
            }

            lock (commentsSync)
            {
                if (!newComment.UserIDReference.IsLoaded)
                {
                    newComment.UserIDReference.Load();
                }

                UserID userPostedComm = Tools.GetUserID(objectContext, newComment.UserID.ID, true);

                topic.comments += 1;
                topic.LastCommentBy = userPostedComm;
                topic.lastCommentDate = newComment.dateCreated;

                Tools.Save(objectContext);
            }
        }

        public void IncreaseTopicVisits(Entities objectContext, ProductTopic topic)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (topic == null)
            {
                throw new BusinessException("topic is Null");
            }

            lock (commentsSync)
            {
                topic.visits += 1;
                Tools.Save(objectContext);
            }
        }

        public void DecreaseTopicComments(Entities objectContext, ProductTopic topic)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (topic == null)
            {
                throw new BusinessException("topic is Null");
            }

            if (topic.comments < 1)
            {
                throw new BusinessException(string.Format("Currently Comments in topic ID = '{0}' are 0 " +
                    "there shouldnt be comments to delete.", topic.ID));
            }

            lock (commentsSync)
            {
                topic.comments -= 1;
                Tools.Save(objectContext);

                if (topic.comments < 1)
                {
                    if (!topic.UserReference.IsLoaded)
                    {
                        topic.UserReference.Load();
                    }

                    UserID userPostedTopic = Tools.GetUserID(objectContext, topic.User.ID, true);

                    topic.LastCommentBy = userPostedTopic;
                    topic.lastCommentDate = topic.dateCreated;

                    Tools.Save(objectContext);
                }
            }
        }

        public bool AreThereNewCommentsSinceLastUserVisit(Entities objectContext, ProductTopic topic, User currUser, string ipAdress)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (topic == null)
            {
                throw new BusinessException("topic is Null");
            }

            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            bool result = false;

            if (topic.comments < 1)
            {
                return false;
            }

            BusinessVisits bVisits = new BusinessVisits();

            DateTime lastVisitByUser = DateTime.UtcNow;
            DateTime lastVisitByIpAdress = DateTime.UtcNow;
            DateTime lastVisit = DateTime.UtcNow;

            bool visitByUser = false;
            if (currUser != null)
            {
                visitByUser = bVisits.CheckIfUserVisitedProductTopic(objectContext, topic, currUser, ref lastVisitByUser);
            }

            bool visitByIp = bVisits.CheckIfIpAdressVisitedProductTopic(objectContext, topic, ipAdress, ref lastVisitByIpAdress);

            if (visitByIp == true && visitByUser == true)
            {
                if (lastVisitByUser > lastVisitByIpAdress)
                {
                    lastVisit = lastVisitByUser;
                }
                else
                {
                    lastVisit = lastVisitByIpAdress;
                }

                if (topic.lastCommentDate > lastVisit)
                {
                    result = true;
                }

            }
            else
            {
                if (visitByIp == false && visitByUser == false)
                {
                    result = true;
                }
                else if (visitByIp == true)
                {
                    if (topic.lastCommentDate > lastVisitByIpAdress)
                    {
                        result = true;
                    }
                }
                else if (visitByUser == true)
                {
                    if (topic.lastCommentDate > lastVisitByUser)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public ProductTopic GetLastUserTopic(Entities objectContext, User currUser, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            List<ProductTopic> topics = new List<ProductTopic>();
            ProductTopic lastTopic = null;

            if (onlyVisible == true)
            {
                topics = objectContext.ProductTopicSet.Where(top => top.User.ID == currUser.ID && top.visible == true).ToList();
            }
            else
            {
                topics = objectContext.ProductTopicSet.Where(top => top.User.ID == currUser.ID).ToList();
            }

            if (topics != null && topics.Count > 0)
            {
                lastTopic = topics.Last();
            }

            return lastTopic;
        }

        /// <summary>
        /// no check for visibility
        /// </summary>
        public List<ProductTopic> GetLastTopics(Entities objectContext, long number)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }

            return objectContext.GetLastTopics(number).ToList();
        }

        public List<ProductTopic> GetLastDeletedTopics(Entities objectContext, int number, string nameContains, long topicId, User byUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<ProductTopic> topics = new List<ProductTopic>();

            if (byUser != null)
            {
                if (!string.IsNullOrEmpty(nameContains))
                {
                    if (topicId > 0)
                    {
                        topics = objectContext.GetLastDeletedProductTopics(nameContains, (long)number, byUser.ID, topicId).ToList();
                    }
                    else
                    {
                        topics = objectContext.GetLastDeletedProductTopics(nameContains, (long)number, byUser.ID, null).ToList();
                    }
                }
                else
                {
                    if (topicId > 0)
                    {
                        topics = objectContext.GetLastDeletedProductTopics(null, (long)number, byUser.ID, topicId).ToList();
                    }
                    else
                    {
                        topics = objectContext.GetLastDeletedProductTopics(null, (long)number, byUser.ID, null).ToList();
                    }
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(nameContains))
                {
                    if (topicId > 0)
                    {
                        topics = objectContext.GetLastDeletedProductTopics(nameContains, (long)number, null, topicId).ToList();
                    }
                    else
                    {
                        topics = objectContext.GetLastDeletedProductTopics(nameContains, (long)number, null, null).ToList();
                    }
                }
                else
                {
                    if (topicId > 0)
                    {
                        topics = objectContext.GetLastDeletedProductTopics(null, (long)number, null, topicId).ToList();
                    }
                    else
                    {
                        topics = objectContext.GetLastDeletedProductTopics(null, (long)number, null, null).ToList();
                    }
                }
            }

            return topics;
        }


    }
}
