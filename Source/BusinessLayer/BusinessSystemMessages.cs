﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessSystemMessages
    {
        public void Add(EntitiesUsers userContext, User forUser, string description)
        {
            Tools.AssertObjectContextExists(userContext);
            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }

            if (!forUser.UserOptionsReference.IsLoaded)
            {
                forUser.UserOptionsReference.Load();
            }
            if (forUser.UserOptions.activated == false)
            {
                throw new BusinessException(string.Format("User ID : {0} is not activated, he shouldn`t receive system messages", forUser.ID));
            }

            SystemMessage newMessage = new SystemMessage();
            newMessage.dateCreated = DateTime.UtcNow;
            newMessage.description = description;
            newMessage.User = forUser;
            newMessage.visible = true;

            userContext.AddToSystemMessageSet(newMessage);
            Tools.Save(userContext);

            BusinessUserOptions bUserOptions = new BusinessUserOptions();
            bUserOptions.ChangeIfUserHaveNewSystemMessages(userContext, forUser, true);
        }

        public void DeleteSystemMessage(EntitiesUsers userContext, User user, SystemMessage message)
        {
            Tools.AssertObjectContextExists(userContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }
            if (message == null)
            {
                throw new BusinessException("message is null");
            }

            if (!message.UserReference.IsLoaded)
            {
                message.UserReference.Load();
            }

            if (message.User != user)
            {
                throw new BusinessException(string.Format("User ID : {0} is not owner of system message ID : {1} ( which is for User ID : {2}) and because of that he can`t delete it"
                    , user.ID, message.ID, message.User.ID));
            }

            message.visible = false;

            Tools.Save(userContext);
        }

        /// <summary>
        /// Returns system messages for user ordered by desc
        /// </summary>
        public List<SystemMessage> GetUserMessages(EntitiesUsers userContext, User user, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(userContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            List<SystemMessage> messages = new List<SystemMessage>();

            if (onlyVisible == true)
            {
                messages = userContext.SystemMessageSet.Where(sm => sm.User.ID == user.ID && sm.visible == true).ToList();
            }
            else
            {
                messages = userContext.SystemMessageSet.Where(sm => sm.User.ID == user.ID).ToList();
            }

            if (messages.Count > 1)
            {
                messages.Reverse();
            }

            return messages;
        }

        public SystemMessage Get(EntitiesUsers userContext, long ID, bool onlyVisible, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(userContext);

            SystemMessage message = null;

            if (onlyVisible == true)
            {
                message = userContext.SystemMessageSet.FirstOrDefault(sm => sm.ID == ID && sm.visible == true);
            }
            else
            {
                message = userContext.SystemMessageSet.FirstOrDefault(sm => sm.ID == ID);
            }

            if(throwExcIfNull == true && message == null)
            {
                if (onlyVisible == true)
                {
                    throw new BusinessException(string.Format("There is no visible:true system message ID : {0}.", ID));
                }
                else
                {
                    throw new BusinessException(string.Format("There is no system message ID : {0}.", ID));
                }
            }

            return message;
        }

        public SystemMessage Get(EntitiesUsers userContext, User user, long ID, bool onlyVisible, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(userContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            SystemMessage message = null;

            if (onlyVisible == true)
            {
                message = userContext.SystemMessageSet.FirstOrDefault(sm => sm.ID == ID && sm.User.ID == user.ID && sm.visible == true);
            }
            else
            {
                message = userContext.SystemMessageSet.FirstOrDefault(sm => sm.ID == ID && sm.User.ID == user.ID);
            }

            if (throwExcIfNull == true && message == null)
            {
                if (onlyVisible == true)
                {
                    throw new BusinessException(string.Format("There is no visible:true system message with ID : {0} for user ID : {1}"
                        , ID, user.ID));
                }
                else
                {
                    throw new BusinessException(string.Format("There is no system message with ID : {0} for user ID : {1}"
                        , ID, user.ID));
                }
            }

            return message;
        }

        /// <summary>
        /// Checks if user have visible:true system messages, true if he have, false if not
        /// </summary>
        public bool UserHaveSystemMessages(EntitiesUsers userContext, User user)
        {
            Tools.AssertObjectContextExists(userContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            SystemMessage message = userContext.SystemMessageSet.FirstOrDefault(msg => msg.User.ID == user.ID && msg.visible == true);
            bool result = false;
            if (message != null)
            {
                result = true;
            }

            return result;
        }


    }
}
