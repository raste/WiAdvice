﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessUserOptions
    {
        object DeletingUsers = new object();
        object AddUserOptions = new object();

        /// <summary>
        /// Add`s new user option
        /// </summary>
        public void Add(EntitiesUsers userContext, User currUser, String secQuestion, String secAnswer, bool byAdmin)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            bool sendActivationMail = false;

            BusinessUser businessUser = new BusinessUser();

            UserOptions currUserOption = new UserOptions();
            currUserOption.User = currUser;
            currUserOption.canReceiveMessages = true;

            currUserOption.haveNewMessages = false;
            currUserOption.haveNewSystemMessages = false;
            currUserOption.haveNewWarning = false;
            currUserOption.unreadReportReply = false;
            currUserOption.unseenTypeSuggestionData = false;

            currUserOption.warnings = 0;

            currUserOption.secretQuestion = secQuestion;
            currUserOption.secretAnswer = businessUser.GetHashed(secAnswer);

            currUserOption.resetPasswordKey = null;
            currUserOption.dateResetPasswordKeyCreated = null;

            currUserOption.changeName = false;

            if (!string.IsNullOrEmpty(currUser.email))
            {
                currUserOption.registeredWithMail = currUser.email;
            }
            else
            {
                currUserOption.registeredWithMail = null;
            }

            if (Configuration.SendActivationCodeOnRegistering == true && byAdmin == false)
            {
                if (businessUser.IsUser(currUser) == true)
                {
                    currUserOption.activated = false;

                    sendActivationMail = true;
                }
                else
                {
                    currUserOption.activated = true;
                }
            }
            else
            {
                currUserOption.activated = true;
            }

            if (currUserOption.activated == true)
            {
                currUserOption.activationCode = "not used";
            }
            else
            {
                currUserOption.activationCode = businessUser.GetHashed(string.Format("{0}{1}1", currUser.ID, currUser.username));
            }

            userContext.AddToUserOptionsSet(currUserOption);
            Tools.Save(userContext);

            if (sendActivationMail == true)
            {
                // SEND ACTIVATION MAIL
                SmtpMailSend mailSend = new SmtpMailSend();
                mailSend.SendAutomaticMessageToUser(currUser, MailKind.UserActivationLink, "");
            }
        }



        /// <summary>
        /// Returns UserOptions for user
        /// </summary>
        private UserOptions Get(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (!currUser.UserOptionsReference.IsLoaded)
            {
                currUser.UserOptionsReference.Load();
            }

            return currUser.UserOptions;
        }

        /// <summary>
        /// Checks if user can receive messages
        /// </summary>
        /// <returns>true if he ca receive,otherwise false</returns>
        public Boolean CanUserReceiveMessages(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);

            bool result = false;

            if (currUserOption.activated == true && currUserOption.canReceiveMessages == true)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Changes UserOption.canReceiveMessages for User
        /// </summary>
        /// <param name="can">true if he should be able to receive messages , otherwise false</param>
        public void ChangeCanUserReceiveMessages(EntitiesUsers userContext, User currUser, Boolean can)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsFromUserTeam(currUser))
            {
                throw new BusinessException(string.Format("Admin ID = {0} is trying to Block/Unblock his message box, this action is not allowed.", currUser.ID));
            }

            UserOptions currUserOption = Get(userContext, currUser);
            currUserOption.canReceiveMessages = can;
            Tools.Save(userContext);
        }

        public void IncreaseUserWarnings(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsFromUserTeam(currUser))
            {
                throw new BusinessException(string.Format("Warnings cannot be increased on admins, Admin ID = {0}.", currUser.ID));
            }

            UserOptions currUserOption = Get(userContext, currUser);
            currUserOption.warnings++;
            Tools.Save(userContext);
        }

        public void DecreaseUserWarnings(EntitiesUsers userContext, User user)
        {
            Tools.AssertObjectContextExists(userContext);
            if (user == null)
            {
                throw new BusinessException("currUser is null");
            }

            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsFromUserTeam(user))
            {
                throw new BusinessException(string.Format("Warnings cannot be decreased on admins, Admin ID = {0}.", user.ID));
            }

            UserOptions currUserOption = Get(userContext, user);
            if (currUserOption.warnings == 0)
            {
                throw new BusinessException(string.Format("User ID : {0} already have 0 warnings, cannot be decreased more than that.", user.ID));
            }
            else
            {
                currUserOption.warnings--;
                Tools.Save(userContext);
            }
        }


        /// <summary>
        /// Changes haveNewMessages property in Database .. used to show in master page if user have unread messages
        /// </summary>
        /// <param name="haveNewMessages">true if have unread, otherwise false</param>
        public void ChangeIfUserHaveNewMessages(EntitiesUsers userContext, User currUser, Boolean haveNewMessages)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            if (currUserOption.haveNewMessages != haveNewMessages)
            {
                currUserOption.haveNewMessages = haveNewMessages;
                Tools.Save(userContext);
            }
        }

        public void ChangeIfUserHaveUnseenTypeSuggestionData(EntitiesUsers userContext, User currUser, Boolean haveUnseenData)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            if (currUserOption.unseenTypeSuggestionData != haveUnseenData)
            {
                currUserOption.unseenTypeSuggestionData = haveUnseenData;
                Tools.Save(userContext);
            }
        }

        public void ChangeIfUserHaveNewWarning(EntitiesUsers userContext, User currUser, Boolean haveNewWarning)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            if (currUserOption.haveNewWarning != haveNewWarning)
            {
                currUserOption.haveNewWarning = haveNewWarning;
                Tools.Save(userContext);
            }
        }

        public void ChangeIfUserHaveNewReportReply(EntitiesUsers userContext, User currUser, Boolean haveNewReply)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            if (currUserOption.unreadReportReply != haveNewReply)
            {
                currUserOption.unreadReportReply = haveNewReply;
                Tools.Save(userContext);
            }
        }

        /// <summary>
        /// Sets to User useroptions.changeName to true or false. True can be set only from administrator team to user team. 
        /// False can be set only from User from user team to himself, or Admin to user.
        /// </summary>
        public void ChangeIfUserNeedToChangeName(EntitiesUsers userContext, Entities objectContext, User userWhichNeedsToChangeName
            , Boolean changeName, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (userWhichNeedsToChangeName == null)
            {
                throw new BusinessException("userWhichNeedsToChangeName is null");
            }

            BusinessUser bUser = new BusinessUser();

            if (changeName == true)
            {
                if (currUser == userWhichNeedsToChangeName)
                {
                    throw new BusinessException(string.Format("User ID : {0} cannot set to himself useroptions.changeName to true"
                        , currUser.ID));
                }

                if (bUser.IsFromUserTeam(currUser))
                {
                    throw new BusinessException(string.Format("Curr User ID : {0} cannot set useroptions.changeName to true on User ID : {1}, because current user is not admin"
                        , currUser.ID, userWhichNeedsToChangeName.ID));
                }

                if (bUser.IsFromAdminTeam(userWhichNeedsToChangeName))
                {
                    throw new BusinessException(string.Format("Curr User ID : {0} cannot set useroptions.changeName to true on Admin ID : {1} (needs to be from user team)"
                        , currUser.ID, userWhichNeedsToChangeName.ID));
                }
            }
            else
            {
                if (currUser != userWhichNeedsToChangeName && bUser.IsFromUserTeam(currUser))
                {
                    throw new BusinessException(string.Format("Curr User ID : {0} cannot set useroptions.changeName to false to User id : {1}, because current user is not admin."
                        , currUser.ID, userWhichNeedsToChangeName.ID));
                }
            }

            UserOptions userOption = Get(userContext, userWhichNeedsToChangeName);
            if (userOption.changeName != changeName)
            {
                userOption.changeName = changeName;
                Tools.Save(userContext);

                if (changeName == true)
                {
                    bLog.LogUser(objectContext, userContext, userWhichNeedsToChangeName, LogType.edit, "changeName", "false", currUser);
                }
                else if (bUser.IsFromAdminTeam(currUser))
                {
                    bLog.LogUser(objectContext, userContext, userWhichNeedsToChangeName, LogType.edit, "changeName", "true", currUser);
                }
            }
        }


        public void ChangeIfUserHaveNewContentOnNotifies(Entities objectContext, UserID currUser, Boolean haveNewContent)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currUser.haveNewContent != haveNewContent)
            {
                currUser.haveNewContent = haveNewContent;
                Tools.Save(objectContext);
            }
        }

        public void ChangeIfUserHaveNewContentOnNotifies(Entities objectContext, User currUser, Boolean haveNewContent)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserID localUser = Tools.GetUserID(objectContext, currUser);

            if (localUser.haveNewContent != haveNewContent)
            {
                localUser.haveNewContent = haveNewContent;
                Tools.Save(objectContext);
            }
        }

        public bool CheckIfUserHavesNewContentOnNotifies(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserID localUser = Tools.GetUserID(objectContext, currUser);
            return localUser.haveNewContent;
        }


        public void ChangeIfUserHaveNewSystemMessages(EntitiesUsers userContext, User currUser, Boolean haveNewSystemMessages)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            if (currUserOption.haveNewSystemMessages != haveNewSystemMessages)
            {
                currUserOption.haveNewSystemMessages = haveNewSystemMessages;
                Tools.Save(userContext);
            }
        }

        /// <summary>
        /// Returns true or false if user haves unread messages
        /// </summary>
        public bool CheckIfUserHavesNewMessages(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            return currUserOption.haveNewMessages;
        }

        /// <summary>
        /// Returns true or false if user have unseesn suggestions data
        /// </summary>
        public bool CheckIfUserHaveUnseenTypeSuggestionData(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            return currUserOption.unseenTypeSuggestionData;
        }

        /// <summary>
        /// Returns true if user haves unread system messages, otherwise false
        /// </summary>
        public bool CheckIfUserHavesNewSystemMessages(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            return currUserOption.haveNewSystemMessages;
        }

        /// <summary>
        /// Returns true if user have new warning, otherwise false
        /// </summary>
        public bool CheckIfUserHavesNewWarning(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            return currUserOption.haveNewWarning;
        }

        public bool CheckIfUserHavesNewReplyToReport(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            return currUserOption.unreadReportReply;
        }

        /// <summary>
        /// returns number of all user warnings
        /// </summary>
        public int CountOfUserWarnings(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserOptions currUserOption = Get(userContext, currUser);
            return currUserOption.warnings;
        }

        /// <summary>
        /// Changes user secret question to new one
        /// </summary>
        public void ChangeUserSecretQuestion(EntitiesUsers userContext, Entities objectContext, UserOptions currUserOption, string newQuestion, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUserOption == null)
            {
                throw new BusinessException("currUserOption is null");
            }

            if (string.IsNullOrEmpty(newQuestion))
            {
                throw new BusinessException("newQuestion is null or empty");
            }

            if (currUserOption.secretQuestion == newQuestion)
            {
                throw new BusinessException(string.Format("user`s old secret question is equal to new one, user id = {0}"
                    , currUserOption.userID));
            }

            string oldQuestion = currUserOption.secretQuestion;
            currUserOption.secretQuestion = newQuestion;
            Tools.Save(userContext);

            if (!currUserOption.UserReference.IsLoaded)
            {
                currUserOption.UserReference.Load();
            }

            bLog.LogUser(objectContext, userContext, currUserOption.User, LogType.edit, "secret question", oldQuestion, currUserOption.User);
        }

        public void ChangeUserSecretAnswer(EntitiesUsers userContext, Entities objectContext, UserOptions currUserOption, string newAnswer, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUserOption == null)
            {
                throw new BusinessException("currUserOption is null");
            }

            if (string.IsNullOrEmpty(newAnswer))
            {
                throw new BusinessException("newAnswer is null or empty");
            }

            BusinessUser businessUser = new BusinessUser();

            currUserOption.secretAnswer = businessUser.GetHashed(newAnswer);
            Tools.Save(userContext);

            if (!currUserOption.UserReference.IsLoaded)
            {
                currUserOption.UserReference.Load();
            }

            bLog.LogUser(objectContext, userContext, currUserOption.User, LogType.edit, "secret question`s answer", "******", currUserOption.User);
        }

        /// <summary>
        /// Marks user as activated, which will enable him to log in
        /// </summary>
        public void SetAccountAsActivated(EntitiesUsers userContext, Entities objectContext, UserOptions currUserOption, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUserOption == null)
            {
                throw new BusinessException("currUserOption is null");
            }

            BusinessUser businessUser = new BusinessUser();

            if (currUserOption.activated == true)
            {
                throw new BusinessException(string.Format("user id = {0}, is already active (cannot be done again)", currUserOption.userID));
            }

            currUserOption.activated = true;
            Tools.Save(userContext);

            if (!currUserOption.UserReference.IsLoaded)
            {
                currUserOption.UserReference.Load();
            }

            bLog.LogUser(objectContext, userContext, currUserOption.User, LogType.edit, "Activated", "false", currUserOption.User);
        }

        /// <summary>
        /// true if activated, otherwise false
        /// </summary>
        public bool IsUserActivated(UserOptions userOptions)
        {
            if (userOptions == null)
            {
                throw new BusinessException("userOptions is null");
            }

            return userOptions.activated;
        }

        /// <summary>
        /// true if name must be changed, otherwise false
        /// </summary>
        public bool CheckIfUserHaveToChangeName(UserOptions userOptions)
        {
            if (userOptions == null)
            {
                throw new BusinessException("userOptions is null");
            }

            return userOptions.changeName;
        }

        public bool IsUserActivated(User currUser)
        {
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (!currUser.UserOptionsReference.IsLoaded)
            {
                currUser.UserOptionsReference.Load();
            }

            return currUser.UserOptions.activated;
        }

        /// <summary>
        /// True if new resetPasswordKey can be created, otherwise false (checks the date ot the last created)
        /// </summary>
        public bool CanUserCreateNewResetPasswordKey(Entities objectContext, UserOptions userOptions)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (userOptions == null)
            {
                throw new BusinessException("userOptions is null");
            }

            bool can = true;

            if (userOptions.dateResetPasswordKeyCreated != null)
            {
                DateTime dateReset = userOptions.dateResetPasswordKeyCreated.Value;

                DateTime timeNow = DateTime.UtcNow;

                TimeSpan timePasses = timeNow - dateReset;
                if (timePasses.TotalHours < 24)
                {
                    can = false;
                }
            }
            else
            {
                if (userOptions.resetPasswordKey != null)
                {
                    userOptions.resetPasswordKey = null;
                    Tools.Save(objectContext);
                }
            }
            return can;
        }


        public void CreateAndSendNewResetPasswordMail(EntitiesUsers userContext, Entities objectContext, UserOptions userOptions)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            if (userOptions == null)
            {
                throw new BusinessException("userOptions is null");
            }

            if (!CanUserCreateNewResetPasswordKey(objectContext, userOptions))
            {
                throw new BusinessException(string.Format("new reset password key cannot be createed for user id = {0}, because 24 hours didnt pass from last"
                    , userOptions.userID));
            }

            if (!userOptions.UserReference.IsLoaded)
            {
                userOptions.UserReference.Load();
            }

            if (userOptions.activated == false)
            {
                throw new BusinessException(string.Format("Reset password link cannot be sent to user: {0} , because it isn`t activated!"
                    , userOptions.User.username));
            }

            if (userOptions.User.email == null)
            {
                throw new BusinessException(string.Format("Cannot send reset password key to User id = {0}, because he doesnt have email typed.", userOptions.User.ID));
            }

            string oldKeyDate = "null";
            if (userOptions.dateResetPasswordKeyCreated != null)
            {
                oldKeyDate = userOptions.dateResetPasswordKeyCreated.Value.ToString();
            }

            BusinessUser businessUser = new BusinessUser();

            userOptions.dateResetPasswordKeyCreated = DateTime.UtcNow;
            userOptions.resetPasswordKey = businessUser.GetHashed(string.Format("resetpass{0}", userOptions.userID));

            Tools.Save(userContext);

            BusinessLog bLog = new BusinessLog(userOptions.userID, "127.0.0.1");
            bLog.LogUser(objectContext, userContext, userOptions.User, LogType.edit, "date new reset password key", oldKeyDate, userOptions.User);

            ///// SEND MAIL 
            SmtpMailSend mailSend = new SmtpMailSend();
            mailSend.SendAutomaticMessageToUser(userOptions.User, MailKind.ResetPasswordLink, "");

        }

        /// <summary>
        /// Resets user password and sends him email with the new one
        /// </summary>
        public void ResetUserPasswordAndSendHimMail(EntitiesUsers userContext, Entities objectContext, UserOptions userOptions, BusinessLog blog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            if (userOptions == null)
            {
                throw new BusinessException("userOptions is null");
            }

            if (!userOptions.UserReference.IsLoaded)
            {
                userOptions.UserReference.Load();
            }

            if (CanUserCreateNewResetPasswordKey(objectContext, userOptions))
            {
                throw new BusinessException(string.Format("user id = {0}, cannot reset password by visiting reset link which is created before more than 24 hours"
                    , userOptions.userID));
            }

            if (userOptions.User.email == null)
            {
                throw new BusinessException(string.Format("Cannot send resetted password to User id = {0}, because he doesnt have email typed.", userOptions.User.ID));
            }

            BusinessUser businessUser = new BusinessUser();
            string newPassword = businessUser.ResetUserPassword(userContext, objectContext, blog, userOptions.User);

            userOptions.resetPasswordKey = null;
            Tools.Save(userContext);

            ///// SEND MAIL
            SmtpMailSend mailSend = new SmtpMailSend();
            mailSend.SendAutomaticMessageToUser(userOptions.User, MailKind.UserNewPassword, newPassword);
        }

        /// <summary>
        /// Checks if there are users which havent been activated in 24 hours and deletes them
        /// </summary>
        public void ScriptCheckIfThereAreUnActivatedUsersForMoreThan24Hours(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);
            BusinessUserActions businessUserActions = new BusinessUserActions();

            User currentUser = null;
            IEnumerable<UserOptions> ieUsersOpt = userContext.UserOptionsSet.Where(uo => uo.activated == false);
            if (ieUsersOpt != null && ieUsersOpt.Count() > 0)
            {
                List<UserAction> userRoles = new List<UserAction>();
                List<UserOptions> userOptions = ieUsersOpt.ToList();

                lock (DeletingUsers)
                {
                    foreach (UserOptions options in userOptions)
                    {
                        if (!options.UserReference.IsLoaded)
                        {
                            options.UserReference.Load();
                        }

                        currentUser = options.User;

                        DateTime now = DateTime.UtcNow;
                        TimeSpan span = now - currentUser.dateCreated;

                        List<UserLog> userLogs = new List<UserLog>();

                        if (span.TotalHours >= 24)
                        {

                            userLogs = userContext.UserLogSet.Where(log => (log.UserModified.ID == currentUser.ID || log.UserModifying.ID == currentUser.ID)).ToList();
                            if (userLogs != null && userLogs.Count > 0)
                            {
                                foreach (UserLog log in userLogs)
                                {
                                    userContext.DeleteObject(log);
                                }
                            }

                            userRoles = businessUserActions.GetUserActions(userContext, currentUser.ID, false).ToList(); ;
                            if (userRoles.Count > 0)
                            {
                                foreach (UserAction action in userRoles)
                                {
                                    userContext.DeleteObject(action);
                                }
                            }

                            userContext.DeleteObject(options);
                            userContext.DeleteObject(currentUser);

                            Tools.Save(userContext);
                        }
                    }
                }
            }
        }


    }
}
