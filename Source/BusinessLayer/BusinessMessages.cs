// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessMessages
    {
        object userBlocking = new object();
        object unBlocking = new object();

        /// <summary>
        /// Adds new message
        /// </summary>
        public void Add(EntitiesUsers userContext, Message newMessage)
        {
            Tools.AssertObjectContextExists(userContext);
            if (newMessage == null)
            {
                throw new BusinessException("newMessage is null");
            }

            userContext.AddToMessageSet(newMessage);
            Tools.Save(userContext);

            BusinessUserOptions userOptions = new BusinessUserOptions();
            userOptions.ChangeIfUserHaveNewMessages(userContext, newMessage.ToUser, true);
        }

        /// <summary>
        /// Checks if User is blocking another user
        /// </summary>
        /// <returns>true if its blocking , otherwise false</returns>
        public Boolean IsUserBlocking(EntitiesUsers userContext, long userBlockingID, long userBlockedID)
        {
            Tools.AssertObjectContextExists(userContext);
            if (userBlockingID < 1)
            {
                throw new BusinessException("userBlockingID is < 1");
            }

            if (userBlockedID < 1)
            {
                throw new BusinessException("userBlockedID is < 1");
            }

            UsersBlocked userBlocking = userContext.UsersBlockedSet.FirstOrDefault
                (ub => ub.UserBlocking.ID == userBlockingID && ub.BlockedUser.ID == userBlockedID && ub.blockActive == true);

            if (userBlocking == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// User blocks another User
        /// </summary>
        public void BlockUser(EntitiesUsers userContext, User userBlocking, long userBlockedID)
        {
            Tools.AssertObjectContextExists(userContext);
            if (userBlocking == null)
            {
                throw new BusinessException("userBlocking is null");
            }

            BusinessUser businessUser = new BusinessUser();
            User userBlocked = businessUser.Get(userContext, userBlockedID, true);

            if (!businessUser.IsFromUserTeam(userBlocking))
            {
                throw new BusinessException(string.Format("Admin ID = {0} is trying to block another user ID = {1}, admins cannot block."
                    , userBlocking.ID, userBlocked.ID));
            }
            if (!businessUser.IsFromUserTeam(userBlocked))
            {
                throw new BusinessException(string.Format("User ID = {0} is trying to block Admin ID = {1}, users cannot block admins."
                    , userBlocking.ID, userBlocked.ID));
            }

            lock (userBlocking)
            {
                if (!IsUserBlocking(userContext, userBlocking.ID, userBlockedID))
                {

                    UsersBlocked newUserBlocked = new UsersBlocked();
                    newUserBlocked.UserBlocking = userBlocking;
                    newUserBlocked.BlockedUser = userBlocked;
                    newUserBlocked.dateCreated = DateTime.UtcNow;
                    newUserBlocked.blockActive = true;

                    userContext.AddToUsersBlockedSet(newUserBlocked);
                    Tools.Save(userContext);
                }
            }
        }

        /// <summary>
        /// Gets all Users which are being blocked by userBlocking
        /// </summary>
        public IEnumerable<UsersBlocked> GetBlockedUsers(User userBlocking)
        {
            if (userBlocking == null)
            {
                throw new BusinessException("userBlocking is null");
            }

            if (userBlocking.UserBlocking.Count == 0)
            {
                userBlocking.UserBlocking.Load();
            }

            IEnumerable<UsersBlocked> blockedUsers = userBlocking.UserBlocking.Where(bu => bu.blockActive == true);

            return blockedUsers;
        }

        /// <summary>
        /// Returns number of how many users is blocking "userBlocking"
        /// </summary>
        public int CountBlockedUsers(EntitiesUsers userContext, User userBlocking)
        {
            Tools.AssertObjectContextExists(userContext);
            if (userBlocking == null)
            {
                throw new BusinessException("userBlocking is null");
            }

            int count = userContext.UsersBlockedSet.Count(ub => ub.UserBlocking.ID == userBlocking.ID && ub.blockActive == true);
            return count;
        }

        /// <summary>
        /// User unblocks other user
        /// </summary>
        /// <param name="currUser">User that is blocking</param>
        /// <param name="userToUnblockID">Id user that will be unblocked</param>
        public void UnblockUser(EntitiesUsers userContext, User currUser, long userToUnblockID)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            lock (unBlocking)
            {
                if (IsUserBlocking(userContext, currUser.ID, userToUnblockID))
                {
                    UsersBlocked userBlocking = userContext.UsersBlockedSet.FirstOrDefault
                   (ub => ub.UserBlocking.ID == currUser.ID && ub.BlockedUser.ID == userToUnblockID && ub.blockActive == true);

                    if (userBlocking == null)
                    {
                        throw new BusinessException(string.Format("User id={0} isnt blocking user id={1} " +
                            ", so it cant be unblocked , there are validators checking for that before this function"
                            , currUser.ID, userToUnblockID));
                    }

                    userBlocking.blockActive = false;
                    Tools.Save(userContext);
                }
            }

        }

        /// <summary>
        /// Returns message with id
        /// </summary>
        public Message GetMessage(EntitiesUsers userContext, long msgID)
        {
            Tools.AssertObjectContextExists(userContext);
            if (msgID < 1)
            {
                throw new BusinessException("msgID is <1");
            }

            Message message = userContext.MessageSet.FirstOrDefault(msg => msg.ID == msgID);
            if (message == null)
            {
                throw new BusinessException(string.Format(" Theres no message with id = {0}", msgID));
            }
            return message;
        }

        /// <summary>
        /// Makes visibleFromUser=false message that was sent from user
        /// </summary>
        /// <param name="currUser">User sent message</param>
        public void DeleteSentMessage(EntitiesUsers userContext, long msgID, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (msgID < 1)
            {
                throw new BusinessException("msgID is <1");
            }

            Message currMessage = GetMessage(userContext, msgID);
            if (!currMessage.FromUserReference.IsLoaded)
            {
                currMessage.FromUserReference.Load();
            }

            if (currMessage.FromUser == currUser)
            {
                if (currMessage.visibleFromUser)
                {
                    currMessage.visibleFromUser = false;
                    Tools.Save(userContext);
                }
            }
        }

        /// <summary>
        /// Makes visibleToUser=false message that was sent to User
        /// </summary>
        /// <param name="currUser">User that the message was sent to</param>
        public void DeleteReceivedMessage(EntitiesUsers userContext, long msgID, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (msgID < 1)
            {
                throw new BusinessException("msgID is <1");
            }

            Message currMessage = GetMessage(userContext, msgID);
            if (!currMessage.ToUserReference.IsLoaded)
            {
                currMessage.ToUserReference.Load();
            }

            if (currMessage.ToUser == currUser)
            {
                if (currMessage.visibleToUser)
                {
                    currMessage.visibleToUser = false;
                    Tools.Save(userContext);
                }
            }
        }

        /// <summary>
        /// Makes either visibleToUser=false or visibleFromUser=false message which was sent to/or received by User
        /// </summary>
        public void DeleteReceivedOrSentMessage(EntitiesUsers userContext, long msgID, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (msgID < 1)
            {
                throw new BusinessException("msgID is <1");
            }

            Message currMessage = GetMessage(userContext, msgID);
            if (!currMessage.ToUserReference.IsLoaded)
            {
                currMessage.ToUserReference.Load();
            }
            if (!currMessage.FromUserReference.IsLoaded)
            {
                currMessage.FromUserReference.Load();
            }

            if (currMessage.ToUser == currUser)
            {
                if (currMessage.visibleToUser)
                {
                    currMessage.visibleToUser = false;
                    Tools.Save(userContext);
                }
            }
            else if (currMessage.FromUser == currUser)
            {
                if (currMessage.visibleFromUser)
                {
                    currMessage.visibleFromUser = false;
                    Tools.Save(userContext);
                }
            }
            else
            {
                string error = string.Format("Message {0} is not from/to user {1} ID {2}", msgID, currUser.username, currUser.ID);
                throw new BusinessException(error);
            }
        }

        /// <summary>
        /// Returns Messages Received by User ordered by descending
        /// </summary>
        /// <param name="currUser">User received messages</param>
        public IEnumerable<Message> GetReceivedMessages(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currUser.MessagesReceived.Count == 0)
            {
                currUser.MessagesReceived.Load();
            }

            IEnumerable<Message> Messages = currUser.MessagesReceived.Where(msgs => msgs.visibleToUser == true)
                .OrderByDescending<Message, long>(new Func<Message, long>(IdSelector));

            return Messages;
        }

        /// <summary>
        /// Returns messages sent by user ordered by descending
        /// </summary>
        /// <param name="currUser">user sent messages</param>
        public IEnumerable<Message> GetSentMessages(User currUser)
        {
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            currUser.MessagesSent.Load();

            IEnumerable<Message> Messages = currUser.MessagesSent.Where(msgs => msgs.visibleFromUser == true)
                .OrderByDescending<Message, long>(new Func<Message, long>(IdSelector)); ;

            return Messages;
        }

        /// <summary>
        /// Checks if User can send messages
        /// </summary>
        /// <returns>true if it can , otherwise false</returns>
        public bool CanUserSendMessages(EntitiesUsers userContext, long sender)
        {
            Tools.AssertObjectContextExists(userContext);
            if (sender < 1)
            {
                throw new BusinessException("sender is <1");
            }

            Boolean passed = false;

            BusinessUser businessUser = new BusinessUser();

            User Sender = businessUser.Get(userContext, sender, true);

            if (businessUser.CanUserDo(userContext, Sender, UserRoles.WriteCommentsAndMessages))
            {
                passed = true;
            }

            return passed;
        }

        /// <summary>
        /// Checks if user can receive messages (checks for activated also)
        /// </summary>
        /// <returns>true if it can otherwise false</returns>
        public bool CanUserReceiveMessages(EntitiesUsers userContext, long receiver)
        {
            Tools.AssertObjectContextExists(userContext);
            if (receiver < 1)
            {
                throw new BusinessException("sender is <1");
            }

            Boolean passed = false;

            BusinessUser businessUser = new BusinessUser();
            BusinessUserOptions businessUserOptions = new BusinessUserOptions();

            User Receiver = businessUser.Get(userContext, receiver, true);

            if (businessUserOptions.CanUserReceiveMessages(userContext, Receiver) == true &&
                businessUserOptions.IsUserActivated(Receiver) == true)
            {
                passed = true;
            }

            return passed;
        }

        /// <summary>
        /// Returns true if user have (received/sent) messages in box , otherwise false
        /// </summary>
        public bool CheckIfUserHaveMessagesInBox(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            bool haveMessages = false;
            if (userContext.MessageSet.Count(msg => msg.FromUser.ID == currUser.ID && msg.visibleFromUser == true) > 0)
            {
                haveMessages = true;
            }
            if (!haveMessages && userContext.MessageSet.Count(msg => msg.ToUser.ID == currUser.ID && msg.visibleToUser == true) > 0)
            {
                haveMessages = true;
            }
            return haveMessages;
        }

        /// <summary>
        /// Checks if user can send messages to other user
        /// </summary>
        /// <returns>true if it can , otherwise false</returns>
        public bool CanUserSendMessageTo(EntitiesUsers userContext, long sender, long receiver)
        {
            Tools.AssertObjectContextExists(userContext);
            if (sender < 1)
            {
                throw new BusinessException("sender is <1");
            }
            if (receiver < 1)
            {
                throw new BusinessException("receiver is <1");
            }

            Boolean passed = false;

            BusinessUser businessUser = new BusinessUser();
            User Sender = businessUser.Get(userContext, sender, true);
            User Receiver = businessUser.GetWithoutVisible(userContext, receiver, true);

            if (Receiver.visible == false)
            {
                return false;
            }
            if (!Receiver.UserOptionsReference.IsLoaded)
            {
                Receiver.UserOptionsReference.Load();
            }
            if (Receiver.UserOptions.activated == false)
            {
                return false;
            }

            if (businessUser.IsUserValidType(Sender) && businessUser.IsUserValidType(Receiver))
            {
                if (businessUser.IsFromUserTeam(Sender) && businessUser.IsFromAdminTeam(Receiver))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (CanUserSendMessages(userContext, Sender.ID) && Sender != Receiver
                && CanUserReceiveMessages(userContext, Receiver.ID)
                )
            {
                if (!IsUserBlocking(userContext, Receiver.ID, Sender.ID) && !IsUserBlocking(userContext, Sender.ID, Receiver.ID))
                {
                    passed = true;
                }

            }

            return passed;
        }

        /// <summary>
        /// Checks if User can block another user
        /// </summary>
        /// <param name="blocker">user trying to block</param>
        /// <param name="blocked">user trying to get blocked</param>
        /// <param name="error">saves error message if it cant be bloced</param>
        /// <returns>true if it can , otherwise false</returns>
        public Boolean CanUserBlockUser(EntitiesUsers userContext, long blocker, long blocked, out String error)
        {
            Tools.AssertObjectContextExists(userContext);
            if (blocker < 1)
            {
                throw new BusinessException("blocker is <1");
            }
            if (blocked < 1)
            {
                throw new BusinessException("blocked is <1");
            }

            error = "";
            Boolean passed = false;

            BusinessUser businessUser = new BusinessUser();
            User userBlocking = businessUser.Get(userContext, blocker, true);

            if (businessUser.IsFromUserTeam(userBlocking))
            {
                User userBlocked = businessUser.Get(userContext, blocked, false);
                if (userBlocked != null)
                {
                    if (!userBlocked.UserOptionsReference.IsLoaded)
                    {
                        userBlocked.UserOptionsReference.Load();
                    }
                    if (userBlocked.UserOptions.activated == true)
                    {
                        if (businessUser.IsFromUserTeam(userBlocked))
                        {
                            if (userBlocking != userBlocked)
                            {
                                if (!IsUserBlocking(userContext, userBlocking.ID, userBlocked.ID))
                                {
                                    passed = true;
                                }
                                else
                                {
                                    error = Tools.GetResource("errAlreadyBlockingUser");
                                }
                            }
                            else
                            {
                                error = Tools.GetResource("errCantBlockUrself");
                            }
                        }
                        else
                        {
                            error = Tools.GetResource("errCantBlockUser");
                        }
                    }
                    else
                    {
                        error = Tools.GetResource("errUserNotActivated");
                    }
                }
                else
                {
                    error = Tools.GetResource("errNoSuchUser");
                }
            }
            else
            {
                throw new BusinessException("user trying to block is not from user team, there are" +
                    " validators checking for that before this function");
            }

            return passed;
        }

        /// <summary>
        /// User is blocking other user to which he sent/received message
        /// </summary>
        public void BlockUserFromMessage(EntitiesUsers userContext, User currUser, Message currMessage)
        {
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currMessage == null)
            {
                throw new BusinessException("currMessage is null");
            }

            BusinessUser businessUser = new BusinessUser();
            User userToBlock = null;
            string error = "";

            if (currMessage.FromUser == currUser)
            {
                userToBlock = currMessage.ToUser;
            }
            else if (currMessage.ToUser == currUser)
            {
                userToBlock = currMessage.FromUser;
            }
            else
            {
                error = string.Format("Message ID = {0} is not from/to user ID = {1}", currMessage.ID, currUser.ID);
                throw new BusinessException(error);
            }

            if (!IsUserBlocking(userContext, currUser.ID, userToBlock.ID))
            {
                BlockUser(userContext, currUser, userToBlock.ID);
            }
            else
            {
                error = string.Format("User ID = {0} is already blocked from user ID = {1}", userToBlock.ID, currUser.ID);
                throw new BusinessException(error);
            }
        }

        /// <summary>
        /// User is UnBlocking user to which he sent/received message
        /// </summary>
        public void UnBlockUserFromMessage(EntitiesUsers userContext, User currUser, Message currMessage)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currMessage == null)
            {
                throw new BusinessException("currMessage is null");
            }

            BusinessUser businessUser = new BusinessUser();
            User userToUnBlock = null;
            string error = "";

            if (currMessage.FromUser == currUser)
            {
                userToUnBlock = currMessage.ToUser;
            }
            else if (currMessage.ToUser == currUser)
            {
                userToUnBlock = currMessage.FromUser;
            }
            else
            {
                error = string.Format("Message ID = {0} is not from/to user ID = {1}", currMessage.ID, currUser.ID);
                throw new BusinessException(error);
            }

            if (IsUserBlocking(userContext, currUser.ID, userToUnBlock.ID))
            {
                UnblockUser(userContext, currUser, userToUnBlock.ID);
            }
            else
            {
                error = string.Format("User ID = {0} is NOT blocked from user ID = {1}, it cannot be unblocked again", userToUnBlock.ID, currUser.ID);
                throw new BusinessException(error);
            }
        }

        /// <summary>
        /// Used for sorting by descending
        /// </summary>
        private long IdSelector(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return message.ID;
        }
    }
}
