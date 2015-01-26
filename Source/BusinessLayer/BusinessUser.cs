﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Specialized;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessUser
    {
        private static object guestsSync = new object();
        private static object loggedUserSync = new object();
        private static List<long> LoggedUsers = new List<long>();
        private static List<long> Guests = new List<long>();
        private static readonly int saltBytesNumber = 20;

        public List<long> GetLoggedUsers()
        {
            return LoggedUsers;
        }

        /// <summary>
        /// returns number of guests which are browsing
        /// </summary>
        public long GetGuests()
        {
            return Guests.Count;
        }

        /// <summary>
        /// Add`s to LoggedUsers list another logged user id
        /// </summary>
        public void AddLoggedUser(EntitiesUsers userContext, Entities objectContext, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);

            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            lock (loggedUserSync)
            {
                if (!LoggedUsers.Contains(currUser.ID))
                {
                    LoggedUsers.Add(currUser.ID);
                }
            }

            DateTime lastLogIn = currUser.lastLogIn;

            currUser.lastLogIn = DateTime.UtcNow;
            Tools.Save(userContext);

            bLog.LogUser(objectContext, userContext, currUser, LogType.edit, "lastLogIn", lastLogIn.ToString(), currUser);
        }

        /// <summary>
        /// Removes user from LoggedUsers list
        /// </summary>
        public void RemoveLoggedUser(long userId)
        {
            if (userId > 0)
            {
                lock (loggedUserSync)
                {
                    if (LoggedUsers.Contains(userId))
                    {
                        LoggedUsers.Remove(userId);
                    }
                }
            }
        }

        public void AddGuest(long userId)
        {
            if (userId < 1)
            {
                lock (guestsSync)
                {
                    Guests.Add(userId);
                }
            }
        }


        public void RemoveGuest(long userId)
        {
            if (userId < 1)
            {
                lock (guestsSync)
                {
                    if (Guests.Count > 0)
                    {
                        Guests.Remove(userId);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if user is logged in
        /// </summary>
        /// <returns>true if it is logged , otherwise false</returns>
        public Boolean IsUserLoggedIn(long userId)
        {
            Boolean isLogged = false;

            if (userId > 0)
            {
                lock (loggedUserSync)
                {
                    if (LoggedUsers.Contains(userId))
                    {
                        isLogged = true;
                    }
                }
            }
            return isLogged;
        }

        /// <summary>
        /// Returns true if user is visible (doesnt check for active)
        /// </summary>
        public Boolean IsUserVisible(User currentUser)
        {
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }

            Boolean visible = false;

            if (currentUser.visible == true)
            {
                visible = true;
            }

            return visible;
        }

        /// <summary>
        /// Returns list with currently logged Users (objects)
        /// </summary>
        public List<User> GetLoggedUsers(EntitiesUsers objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            List<User> loggedUsers = new List<User>();
            List<long> loggedUsersIds = GetLoggedUsers();

            User currentUser = null;
            if (loggedUsersIds.Count > 0)
            {
                foreach (long userid in loggedUsersIds)
                {
                    currentUser = GetWithoutVisible(objectContext, userid, true);
                    loggedUsers.Add(currentUser);
                }
            }

            return loggedUsers;
        }


        /// <summary>
        /// Returns visible=true User with ID
        /// </summary>
        public User Get(EntitiesUsers userContext, long id, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(userContext);

            User user = userContext.UserSet.FirstOrDefault<User>
                (usr => usr.ID == id && usr.visible == true);

            if (throwExcIfNull == true && user == null)
            {
                throw new BusinessException(string.Format("No user with ID : {0}.", id));
            }

            return user;
        }

        public User Get(long id, bool throwExcIfNull)
        {
            EntitiesUsers userContext = new EntitiesUsers();

            User user = userContext.UserSet.FirstOrDefault<User>
                (usr => usr.ID == id && usr.visible == true);

            if (throwExcIfNull == true && user == null)
            {
                throw new BusinessException(string.Format("No user with ID : {0}.", id));
            }

            return user;
        }

        /// <summary>
        /// Returns user with id
        /// </summary>
        public User GetWithoutVisible(EntitiesUsers userContext, long id, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(userContext);

            User user = userContext.UserSet.FirstOrDefault<User>
                (usr => usr.ID == id);

            if (throwExcIfNull == true && user == null)
            {
                throw new BusinessException(string.Format("No user with ID : {0}.", id));
            }

            return user;
        }

        public User GetWithoutVisible(EntitiesUsers userContext, string name, bool throwExcIfNull)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty");
            }

            Tools.AssertObjectContextExists(userContext);
            User user = userContext.UserSet.FirstOrDefault<User>
                (usr => usr.username == name);

            if (throwExcIfNull == true && user == null)
            {
                throw new BusinessException(string.Format("No user with name : {0}.", name));
            }

            return user;
        }

        public User GetWithoutVisible(long id, bool throwExcIfNull)
        {
            EntitiesUsers userContext = new EntitiesUsers();

            Tools.AssertObjectContextExists(userContext);

            User user = userContext.UserSet.FirstOrDefault<User>
                (usr => usr.ID == id);

            if (throwExcIfNull == true && user == null)
            {
                throw new BusinessException(string.Format("No user with ID : {0}.", id));
            }

            return user;
        }

        /// <summary>
        /// Returns UserName of USer with id
        /// </summary>
        public String GetUserName(EntitiesUsers userContext, long userID)
        {
            Tools.AssertObjectContextExists(userContext);
            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }

            BusinessUser businessUser = new BusinessUser();
            User user = businessUser.GetWithoutVisible(userContext, userID, true);
            String name = user.username;

            return name;
        }

        /// <summary>
        /// Returns User with username
        /// </summary>
        public User GetByName(EntitiesUsers userContext, string name, bool throwExcIfNull, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(userContext);
            if (name == null || name.Length < 1)
            {
                throw new BusinessException("invalid name");
            }

            User user = null;

            if (onlyVisible == true)
            {
                user = userContext.UserSet.FirstOrDefault<User>
                    (usr => usr.username == name && usr.visible == true);
            }
            else
            {
                user = userContext.UserSet.FirstOrDefault<User>
                    (usr => usr.username == name);
            }

            if (throwExcIfNull == true && user == null)
            {
                if (onlyVisible == true)
                {
                    throw new BusinessException(string.Format("There is no visible:true user with name = {0}", name));
                }
                else
                {
                    throw new BusinessException(string.Format("There is no user with name = {0}", name));
                }
            }


            return user;
        }

        /// <summary>
        /// Checks if There is Existing User with Name , True if there is , false if not
        /// </summary>
        /// <param name="withVisibleFalse">True if it should check for Visible False users also , otherwise false</param>
        /// <returns>True if there is user with name , otherwise false</returns>
        public static Boolean CheckIfThereIsUserWithName(EntitiesUsers userContext, string name, Boolean withVisibleFalse)
        {
            Tools.AssertObjectContextExists(userContext);
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            User user = null;
            if (withVisibleFalse)
            {
                user = userContext.UserSet.FirstOrDefault(usr => usr.username == name);
            }
            else
            {
                user = userContext.UserSet.FirstOrDefault(usr => usr.username == name && usr.visible == true);
            }

            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Returns All users (all types)
        /// </summary>
        public IEnumerable<User> GetAll(EntitiesUsers objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<User> users = objectContext.UserSet;
            return users;
        }

        /// <summary>
        /// Tries to logg in User 
        /// </summary>
        /// <returns>if name and password correct returns user id , otherwise -1. DOESNT CHECK FOR VISIBILITY.</returns>
        public long Login(Entities objectContext, EntitiesUsers userContext, String name, String password)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name is null or empty");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new BusinessException("password is null or empty");
            }

            Tools.AssertObjectContextExists(userContext);

            string hashedPassword = GetHashed(password);
            User user = userContext.UserSet.FirstOrDefault<User>
                (usr => usr.username == name);  

            long userid = -1;

            if (user != null && IsUserValidType(userContext, user.ID))
            {
                if (CheckPassword(password, user.password))
                {
                    userid = user.ID;
                }
            }

            return userid;
        }

        /// <summary>
        /// Returns the user which is with username and password
        /// </summary>
        public User GetUserByNameAndPassword(EntitiesUsers userContext, String name, String password)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name is null or empty");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new BusinessException("password is null or empty");
            }

            Tools.AssertObjectContextExists(userContext);

            string hashedPassword = GetHashed(password);
            User user = userContext.UserSet.FirstOrDefault<User>
                (usr => usr.username == name && usr.visible == true);

            if (user != null && !IsUserValidType(userContext, user.ID))
            {
                user = null;
            }

            return user;
        }

        /// <summary>
        /// Add`s new User
        /// </summary>
        public void AddUser(EntitiesUsers userContext, Entities objectContext, User newUser
            , BusinessLog bLog, String secQuestion, String secAnswer, bool byAdmin, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);
            if (newUser == null)
            {
                throw new ArgumentNullException("newUser");
            }

            if (byAdmin == true && currUser == null)
            {
                throw new BusinessException(string.Format("currUser is null"));
            }

            userContext.AddToUserSet(newUser);
            Tools.Save(userContext);

            /////
            UserID newUsedID = new UserID();
            newUsedID.ID = newUser.ID;
            newUsedID.haveNewContent = false;

            objectContext.AddToUserIDSet(newUsedID);
            Tools.Save(objectContext);
            /////

            StringCollection contextStringCollection = Configuration.ApplicationVariantConnectionStrings(false);
            if (contextStringCollection.Count > 0)
            {
                foreach (string entityString in contextStringCollection)
                {
                    Entities localContext = new Entities(entityString);
                    UserID testForExistingId = localContext.UserIDSet.FirstOrDefault(user => user.ID == newUser.ID);
#if DEBUG
                    // doesnt throw exception if there is already user with id = newuserID for the local database
                    if (testForExistingId == null)
                    {
                        UserID localUserID = new UserID();
                        localUserID.ID = newUser.ID;
                        localUserID.haveNewContent = false;

                        localContext.AddToUserIDSet(localUserID);
                        Tools.Save(localContext);
                    }

#else
                    // throws exception if there is already user with id = newuserID for the local database
                    if (testForExistingId != null)
                    {
                        throw new BusinessException(string.Format("There is already user with ID : {0} for database : '{1}'"));
                    }
                    else
                    {
                        UserID localUserID = new UserID();
                        localUserID.ID = newUser.ID;
                        localUserID.haveNewContent = false;

                        localContext.AddToUserIDSet(localUserID);
                        Tools.Save(localContext);
                    }
#endif
                }
            }

            BusinessStatistics businessStatistic = new BusinessStatistics();
            if (IsFromAdminTeam(newUser))
            {
                businessStatistic.AdminRegistered(userContext);
            }
            else
            {
                businessStatistic.UserRegistered(userContext);
            }

            ////

            if (byAdmin == true)
            {
                bLog.LogUser(objectContext, userContext, newUser, LogType.create, string.Empty, string.Empty, currUser);
            }
            else
            {
                bLog.LogUser(objectContext, userContext, newUser, LogType.create, string.Empty, string.Empty, newUser);
            }

            BusinessUserOptions userOptions = new BusinessUserOptions();
            userOptions.Add(userContext, newUser, secQuestion, secAnswer, byAdmin);
        }

        /// <summary>
        /// Returns hashed text, used for passwords, user`s secret answer (UserOptions), user activation code,
        /// user reset password key 
        /// </summary>
        public String GetHashed(String password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new BusinessException("password is null or empty");
            }

            byte[] salt = new byte[saltBytesNumber];
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();
            Gen.GetBytes(salt);

            StringBuilder saltedPassword = new StringBuilder();
            for (int j = 0; j < salt.Length; j++)
            {
                saltedPassword.Append(salt[j].ToString("X2"));
            }
            saltedPassword.Append(password);

            String SaltedPasswordStr = saltedPassword.ToString();

            UTF8Encoding encoder = new UTF8Encoding();
            SHA256CryptoServiceProvider sha256hasher = new SHA256CryptoServiceProvider();
            byte[] hashed256bytes = sha256hasher.ComputeHash(encoder.GetBytes(SaltedPasswordStr));

            StringBuilder output = new StringBuilder("");
            for (int i = 0; i < hashed256bytes.Length; i++)
            {
                output.Append(hashed256bytes[i].ToString("X2"));
            }

            for (int k = 0; k < salt.Length; k++)
            {
                output.Append(salt[k].ToString("X2"));
            }

            return output.ToString();
        }

        /// <summary>
        /// Checks if user typed password or secret answer is equal to the hashed one
        /// </summary>
        /// <returns>true id they are equal,otherwise false</returns>
        public Boolean CheckPassword(String password, String storedHashedPass)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new BusinessException("password is null");
            }
            if (string.IsNullOrEmpty(storedHashedPass))
            {
                throw new BusinessException("hashedPass is null");
            }

            Boolean passed = false;
            string salt = storedHashedPass.Substring(storedHashedPass.Length - (2 * saltBytesNumber));

            ///
            String SaltedPassword = string.Format("{0}{1}", salt, password);

            UTF8Encoding encoder = new UTF8Encoding();
            SHA256CryptoServiceProvider sha256hasher = new SHA256CryptoServiceProvider();
            byte[] hashed256bytes = sha256hasher.ComputeHash(encoder.GetBytes(SaltedPassword));
            StringBuilder output = new StringBuilder("");
            for (int i = 0; i < hashed256bytes.Length; i++)
            {
                output.Append(hashed256bytes[i].ToString("X2"));
            }
            ///

            output.Append(salt);

            if (storedHashedPass.Equals(output.ToString()))
            {
                passed = true;
            }

            return passed;
        }

        /// <summary>
        /// Returns Guest user
        /// </summary>
        public User GetGuest(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);
            IEnumerable<User> users;
            User guestuser = new User();

            users = userContext.UserSet.Where(usr => usr.username == "guest" && usr.type == "system");
            if (users.Count<User>() != 1)
            {
                throw new InvalidOperationException("there is either no guest system user , or there is more than one with that name !");
            }
            else
            {
                guestuser = users.First<User>();
            }

            return guestuser;
        }
        public User GetGuest()
        {
            EntitiesUsers userContext = new EntitiesUsers();
            Tools.AssertObjectContextExists(userContext);
            IEnumerable<User> users;
            User guestuser = new User();

            users = userContext.UserSet.Where(usr => usr.username == "guest" && usr.type == "system");
            if (users.Count<User>() != 1)
            {
                throw new InvalidOperationException("there is either no guest system user , or there is more than one with that name !");
            }
            else
            {
                guestuser = users.First<User>();
            }

            return guestuser;
        }

        /// <summary>
        /// Returns System user
        /// </summary>
        public User GetSystem()
        {
            EntitiesUsers objectContext = new EntitiesUsers();

            Tools.AssertObjectContextExists(objectContext);
            User system = new User();

            List<User> users = objectContext.UserSet.Where(usr => usr.username == "system" && usr.type == "system").ToList();

            if (users.Count<User>() != 1)
            {
                throw new InvalidOperationException("there is either no system user , or there is more than one with that name !");
            }
            else
            {
                system = users.First<User>();
            }

            return system;
        }

        public User GetSystem(EntitiesUsers userContext)
        {

            Tools.AssertObjectContextExists(userContext);
            IEnumerable<User> users;
            User system = new User();

            users = userContext.UserSet.Where(usr => usr.username == "system" && usr.type == "system");
            if (users.Count<User>() != 1)
            {
                throw new InvalidOperationException("there is either no system user , or there is more than one with that name !");
            }
            else
            {
                system = users.First<User>();
            }

            return system;
        }

        public bool CanUserModifyCompany(Entities objectContext, long companyID, long userID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Boolean result = CanUserModify(objectContext, "company", companyID, userID);
            return result;
        }

        public bool CanUserModifyAllCompanyProducts(Entities objectContext, long companyID, long userID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Boolean result = CanUserModify(objectContext, "aCompProdModificator", companyID, userID);
            return result;
        }

        public bool CanUserModifyProduct(Entities objectContext, long productID, long userID)
        {
            Tools.AssertObjectContextExists(objectContext);

            Boolean result = CanUserModify(objectContext, "product", productID, userID);

            if (result == false)
            {
                BusinessProduct businessProduct = new BusinessProduct();
                Product currProduct = businessProduct.GetProductByIDWV(objectContext, productID);
                if (currProduct == null)
                {
                    throw new BusinessException("currProduct is null");
                }
                currProduct.CompanyReference.Load();
                result = CanUserModifyAllCompanyProducts(objectContext, currProduct.Company.ID, userID);
            }

            return result;
        }

        /// <summary>
        /// Chechks if user can modify Type with TypeID
        /// </summary>
        private bool CanUserModify(Entities objectContext, String type, long typeID, long userID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Boolean result = false;
            if (type == null || type == string.Empty)
            {
                throw new ArgumentNullException("type");
            }
            else
            {
                if (typeID < 1)
                {
                    throw new BusinessException("typeID is < 1");
                }
                if (userID < 1)
                {
                    throw new BusinessException("userID is < 1");
                }

                if (type != "company" && type != "product" && type != "aCompProdModificator")
                {
                    throw new BusinessException(string.Format("role type {0} is not supported role type", type));
                }

                UsersTypeAction userAction =
                    objectContext.UsersTypeActionSet.FirstOrDefault
                    (ua => ua.User.ID == userID && ua.TypeAction.type == type && ua.TypeAction.typeID == typeID && ua.visible == true);

                if (userAction != null)
                {
                    result = true;
                }

            }
            return result;
        }



        /// <summary>
        /// Checks if User Have visible=true ActionName 
        /// </summary>
        /// <returns>true if he have,otherwise false</returns>
        private Boolean ActionAllowed(EntitiesUsers objectContext, long userID, string actionName)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(actionName))
            {
                throw new BusinessException("actionName is null or empty");
            }
            Boolean result = false;
            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }

            ObjectParameter userIdParam = new ObjectParameter("userId", userID);  // using System.Data.Objects;
            ObjectParameter actionNameParam = new ObjectParameter("actionName", actionName);
            ObjectParameter[] parameters = new ObjectParameter[] { userIdParam, actionNameParam };

            string predicate = "it.User.ID == @userId && it.Action.Name == @actionName";
            ObjectQuery<UserAction> actions = objectContext.UserActionSet.Where(predicate, parameters);
            if (actions.Count<UserAction>() == 1)
            {
                UserAction userAction = actions.First<UserAction>();
                result = (userAction.visible == true);
            }
            return result;
        }

        public User GetGlobalOrAdministrator(EntitiesUsers objectContext, long id)
        {

            Tools.AssertObjectContextExists(objectContext);
            User user = objectContext.UserSet.FirstOrDefault<User>
                (usr => usr.ID == id && usr.visible == true && (usr.type == "administrator" || usr.type == "global"));
            return user;
        }

        public User GetModerator(EntitiesUsers objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            User user = objectContext.UserSet.FirstOrDefault<User>
                (usr => usr.ID == id && usr.visible == true && usr.type == "moderator");
            return user;
        }

        public User GetGlobal(EntitiesUsers objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            User user = objectContext.UserSet.FirstOrDefault<User>
                (usr => usr.ID == id && usr.visible == true && usr.type == "global");
            return user;
        }

        /// <summary>
        /// checks if user is one of the types : writer,user,moderator,administrator,global
        /// </summary>
        /// <returns>true if its one of the types,otherwise false</returns>
        public bool IsUserValidType(EntitiesUsers userContext, long userID)
        {
            Tools.AssertObjectContextExists(userContext);
            Boolean result = false;
            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }

            User user = userContext.UserSet.FirstOrDefault
                (us => us.ID == userID && (us.type == "writer" || us.type == "user" || us.type == "moderator" ||
                    us.type == "administrator" || us.type == "global"));

            if (user != null)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// checks if user is one of the types : writer,user,moderator,administrator,global
        /// </summary>
        /// <returns>true if its one of the types,otherwise false</returns>
        public bool IsUserValidType(User currUser)
        {
            Boolean result = false;
            if (currUser == null)
            {
                throw new BusinessException("invalid user");
            }

            String type = currUser.type;

            if (type == "writer" || type == "user" || type == "moderator" || type == "administrator" || type == "global")
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// checks if User is Guest
        /// </summary>
        /// <returns>true if its guest, otherwise false</returns>
        public bool IsGuest(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);

            Boolean result = true;
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            User guest = GetGuest(userContext);
            if (currUser != guest)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// checks if user is of type 'user'
        /// </summary>
        /// <returns>true if its user,otherwise false</returns>
        public bool IsUser(User currUser)
        {
            Boolean result = false;
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currUser.type == "user")
            {
                result = true;
            }

            return result;
        }


        /// <summary>
        /// Changes data of one of User fields
        /// </summary>
        /// <param name="field">password,email,signature,name</param>
        public void ChangeUserData(EntitiesUsers userContext, Entities objectContext, User currUser, string field, string newValue, BusinessLog bLog)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (field == null || field.Length < 1)
            {
                throw new BusinessException("field is null or empty");
            }

            String oldValue = "";

            switch (field)
            {
                case ("password"):
                    if (newValue == null || newValue.Length < 1)
                    {
                        throw new BusinessException("newValue is null or empty");
                    }
                    oldValue = "***";

                    currUser.password = GetHashed(newValue);
                    Tools.Save(userContext);

                    bLog.LogUser(objectContext, userContext, currUser, LogType.edit, "password", oldValue, currUser);
                    break;
                case ("email"):
                    if (newValue == null || newValue.Length < 1)
                    {
                        throw new BusinessException("newValue is null or empty");
                    }

                    oldValue = currUser.email;
                    currUser.email = newValue;

                    Tools.Save(userContext);

                    bLog.LogUser(objectContext, userContext, currUser, LogType.edit, "email", oldValue, currUser);
                    break;
                case ("signature"):
                    oldValue = currUser.userData;
                    currUser.userData = newValue;

                    Tools.Save(userContext);

                    bLog.LogUser(objectContext, userContext, currUser, LogType.edit, "signature", oldValue, currUser);
                    break;
                case "name":
                    if (newValue == null || newValue.Length < 1)
                    {
                        throw new BusinessException("newValue is null or empty");
                    }

                    if (IsFromUserTeam(currUser) == false)
                    {
                        throw new BusinessException(string.Format("User id : {0} cannot change his username because he is not from user team"
                            , currUser.ID));
                    }

                    if (!currUser.UserOptionsReference.IsLoaded)
                    {
                        currUser.UserOptionsReference.Load();
                    }
                    if (currUser.UserOptions.changeName == false)
                    {
                        throw new BusinessException(string.Format("User id : {0} cannot change his username because UserOptions.changeName is false"
                            , currUser.ID));
                    }

                    if (Tools.NameValidatorPassed(objectContext, "users", newValue, 0) == false)
                    {
                        throw new BusinessException(string.Format("User id : {0} cannot change his username to : {1} , because it didn`t pass name validator."
                            , currUser.ID, newValue));
                    }

                    oldValue = currUser.username;

                    currUser.username = newValue;
                    Tools.Save(userContext);

                    bLog.LogUser(objectContext, userContext, currUser, LogType.edit, "username", oldValue, currUser);

                    break;
                default:
                    throw new BusinessException(string.Format("field = {0} is not supported", field));
            }

        }

        /// <summary>
        /// Makes visible=false user, Also removes all his type roles, declines all his active edit right transfers,
        /// and declines all active edit suggestions from/by him
        /// </summary>
        public void DeleteUser(EntitiesUsers userContext, User userDeleting, BusinessLog bLog, User modifier)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (userDeleting == null)
            {
                throw new BusinessException("userDeleting is null");
            }
            if (modifier == null)
            {
                throw new BusinessException("modifier is null");
            }

            if (userDeleting.visible == true)
            {

                StringCollection contextStringCollection = Configuration.ApplicationVariantConnectionStrings(true);
                if (contextStringCollection.Count > 0)
                {
                    foreach (string entityString in contextStringCollection)
                    {
                        Entities localContext = new Entities(entityString);

                        ///Removes all user type actions..declines all rights transfers(from him) and declines all type suggestions(to him) to the user for theese type actions
                        BusinessUserTypeActions butActions = new BusinessUserTypeActions();
                        List<UsersTypeAction> userTypeActions = butActions.GetUserTypeActions(localContext, userDeleting, true).ToList();
                        if (userTypeActions != null && userTypeActions.Count > 0)
                        {
                            foreach (UsersTypeAction typeAction in userTypeActions)
                            {
                                butActions.RemoveUserTypeAction(localContext, userContext, typeAction, modifier, bLog, true);
                            }
                        }

                        //Declines all rest active type suggestions (active sent type suggestions)
                        BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();
                        btSuggestions.DeclineAllEditSuggestionWithUserWhenUserIsDeleted(userContext, localContext,
                            userDeleting, bLog);

                        //Removes all rest right transfers (active transfers to user which is being deleted)
                        BusinessTransferAction btActions = new BusinessTransferAction();
                        btActions.RemoveTransfersToOrByUserWhichIsBeingDeleted(localContext, userContext, bLog, userDeleting);

                        //Removes all user Notifies
                        BusinessNotifies bNotifies = new BusinessNotifies();
                        bNotifies.RemoveAllUserNotifies(userContext, localContext, bLog, userDeleting);

                        //Resolves all user reports
                        BusinessReport bReport = new BusinessReport();
                        bReport.ResolveAllUserReportsWhenItsBeingDeleted(localContext, userContext, bLog, userDeleting);

                        bLog.LogUser(localContext, userContext, userDeleting, LogType.delete, string.Empty, string.Empty, modifier);
                    }
                }

                BusinessStatistics stat = new BusinessStatistics();
                if (IsFromAdminTeam(userDeleting))
                {
                    stat.AdminDeleted(userContext);
                }
                else
                {
                    stat.UserDeleted(userContext);
                }

                // Deleteting is last
                userDeleting.visible = false;
                Tools.Save(userContext);

            }
        }

        public void RemoveUserSignature(EntitiesUsers userContext, Entities objectContext, User user, BusinessLog bLog, User admin)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (user == null)
            {
                throw new BusinessException("user is null");
            }
            if (admin == null)
            {
                throw new BusinessException("admin is null");
            }

            if (!IsFromUserTeam(user))
            {
                throw new BusinessException(string.Format("Admin id : {0} cannot remove signature of non user id : {1}."
                    , admin.ID, user.ID));
            }
            if (!CanAdminDo(userContext, admin, AdminRoles.EditUsers))
            {
                throw new BusinessException(string.Format("User id : {0} cannot remove signature of user id : {1}, because he cannot edit users."
                    , admin.ID, user.ID));
            }

            if (!CanUserDo(userContext, user, UserRoles.HaveSignature))
            {
                throw new BusinessException(string.Format("Admin id : {0} cannot remove signature of user id : {1}, because he cannot have one."
                    , admin.ID, user.ID));
            }

            if (string.IsNullOrEmpty(user.userData))
            {
                throw new BusinessException(string.Format("Admin id : {0} cannot remove signature of user id : {1}, because he don`t have signature."
                    , admin.ID, user.ID));
            }

            string oldValue = user.userData;

            user.userData = string.Empty;
            Tools.Save(userContext);

            bLog.LogUser(objectContext, userContext, user, LogType.edit, "userData", oldValue, admin);

            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
            string systemMessage = Tools.GetResource("SignatureRemoved");
            bSystemMessages.Add(userContext, user, systemMessage);

        }

        public void UnDeleteUser(EntitiesUsers userContext, Entities objectContext, User currUser, BusinessLog bLog, User undeletedBy)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (undeletedBy == null)
            {
                throw new BusinessException("undeletedBy is null");
            }

            if (currUser.visible == false)
            {
                currUser.visible = true;

                Tools.Save(userContext);

                bLog.LogUser(objectContext, userContext, currUser, LogType.undelete, string.Empty, string.Empty, undeletedBy);
            }
        }

        /// <summary>
        /// Checks if user is of type moderator
        /// </summary>
        /// <returns>true if its moderator,otherwise false</returns>
        public Boolean IsModerator(User currUser)
        {
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }

            Boolean result = false;
            if (currUser.type == "moderator")
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks if user is of type administrator
        /// </summary>
        /// <returns>true if its administrator,otherwise false</returns>
        public Boolean IsAdministrator(User currUser)
        {
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            Boolean result = false;
            if (currUser.type == "administrator")
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks if user is of type Global Administrator
        /// </summary>
        /// <returns>true if its global administrator , otherwise false</returns>
        public Boolean IsGlobalAdministrator(User currUser)
        {
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            Boolean result = false;
            if (currUser.type == "global")
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks if user is of type Administrator or Global Administrator
        /// </summary>
        /// <returns>true if its one of the types , otherwise false</returns>
        public Boolean IsAdminOrGlobalAdmin(User currUser)
        {
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            Boolean result = false;
            if (currUser.type == "global" || currUser.type == "administrator")
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks if user is of type Moderator or Administrator or GLobal Adminsitrator
        /// </summary>
        /// <returns>true if its one of the types,othwerise false</returns>
        public Boolean IsFromAdminTeam(User currUser)
        {
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            Boolean result = false;
            if (currUser.type == "global" || currUser.type == "administrator" || currUser.type == "moderator")
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks if user is of type user or writer
        /// </summary>
        /// <returns>true is its one of the types,othwerwise false</returns>
        public Boolean IsFromUserTeam(User currUser)
        {
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            Boolean result = false;
            if (currUser.type == "user" || currUser.type == "writer")
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks if user is of type user or writer
        /// </summary>
        /// <returns>true is its one of the types,othwerwise false</returns>
        public Boolean IsFromUserTeam(long userID)
        {
            EntitiesUsers objectContext = new EntitiesUsers();
            Tools.AssertObjectContextExists(objectContext);

            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }

            Boolean result = false;

            User user = Get(objectContext, userID, false);
            if (user != null)
            {
                if (user.type == "user" || user.type == "writer")
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all visible.true users of type Global Administrator and Administrator
        /// </summary>
        public IEnumerable<User> GetGlobalsAndAdministrators(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);

            IEnumerable<User> users = userContext.UserSet.Where
                (usr => (usr.type == "global" || usr.type == "administrator") && usr.visible == true).
                OrderByDescending<User, long>(new Func<User, long>(IdSelector));

            return users;
        }

        /// <summary>
        /// Returns all users of type Global Administrator
        /// </summary>

        public IEnumerable<User> GetGlobals(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);

            IEnumerable<User> users = userContext.UserSet.Where
                (usr => usr.type == "global" && usr.visible == true);

            return users;
        }

        /// <summary>
        /// Returns all users of type Admministrator
        /// </summary>
        public IEnumerable<User> GetAdministrators(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);

            IEnumerable<User> users = userContext.UserSet.Where
                (usr => usr.type == "administrator" && usr.visible == true);

            return users;
        }

        /// <summary>
        /// Returns all visible Admins (moderators, admins , globals)
        /// </summary>
        public IEnumerable<User> GetAllAdministrators(bool onlyVisible)
        {
            EntitiesUsers userContext = new EntitiesUsers();
            Tools.AssertObjectContextExists(userContext);

            IEnumerable<User> users = null;

            if (onlyVisible == true)
            {
                users = userContext.UserSet.Where
                (usr => (usr.type == "administrator" || usr.type == "moderator" || usr.type == "global")
                    && usr.visible == true);
            }
            else
            {
                users = userContext.UserSet.Where
               (usr => (usr.type == "administrator" || usr.type == "moderator" || usr.type == "global"));
            }

            return users;
        }

        /// <summary>
        /// Returns all visible=false Users of type Global Administrator and Administrator
        /// </summary>
        public IEnumerable<User> GetDeletedGlobalsAndAdministrators(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);

            IEnumerable<User> users = userContext.UserSet.Where
                (usr => (usr.type == "global" || usr.type == "administrator") && usr.visible == false).
                OrderByDescending<User, long>(new Func<User, long>(IdSelector));

            return users;
        }

        /// <summary>
        /// Returns all visible.true users of type Moderator
        /// </summary>
        public IEnumerable<User> GetModerators(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);

            IEnumerable<User> users = userContext.UserSet.Where
                (usr => usr.type == "moderator" && usr.visible == true).
                OrderByDescending<User, long>(new Func<User, long>(IdSelector));

            return users;
        }

        /// <summary>
        /// Returns all visible=false Users of type Moderator
        /// </summary>
        public IEnumerable<User> GetDeletedModerators(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);

            IEnumerable<User> users = userContext.UserSet.Where
                (usr => usr.type == "moderator" && usr.visible == false).
                OrderByDescending<User, long>(new Func<User, long>(IdSelector));

            return users;
        }

        /// <summary>
        /// Returns all visible=true Users of Type User and Writer
        /// </summary>
        public IEnumerable<User> GetVisibleUsersAndWriters(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);

            IEnumerable<User> users = userContext.UserSet.Where
                (usr => (usr.type == "user" || usr.type == "writer") && usr.visible == true).
                OrderByDescending<User, long>(new Func<User, long>(IdSelector));

            return users;
        }

        /// <summary>
        /// Returns all visible=false users of type User and Writer
        /// </summary>
        public IEnumerable<User> GetDeletedUsersAndWriters(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);

            IEnumerable<User> users = userContext.UserSet.Where
                (usr => (usr.type == "user" || usr.type == "writer") && usr.visible == false).
                OrderByDescending<User, long>(new Func<User, long>(IdSelector));

            return users;
        }

        /// <summary>
        /// Used for sorting by descending
        /// </summary>
        private long IdSelector(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return user.ID;
        }

        /// <summary>
        /// Creates new user and Add`s it to database
        /// </summary>
        public void RegisterUser(EntitiesUsers userContext, Entities objectContext, String name, String password, String email, BusinessLog bLog
            , String secQuestion, String secAnswer, bool byAdmin, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty or null");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new BusinessException("password is empty or null");
            }

            if (!Tools.NameValidatorPassed(objectContext, "users", name, 0))
            {
                throw new BusinessException(string.Format("Name = {0} is taken already", name));
            }
            User system = GetSystem(userContext);
            if (system == null)
            {
                throw new BusinessException("System user is null");
            }

            if (byAdmin == true && currUser == null)
            {
                throw new BusinessException(string.Format("currUser is null"));
            }

            User newUser = new User();
            newUser.username = name;
            newUser.password = GetHashed(password);
            if (string.IsNullOrEmpty(email))
            {
                newUser.email = null;
                newUser.type = "writer";
            }
            else
            {
                newUser.email = email;
                newUser.type = "user";
            }

            newUser.dateCreated = DateTime.UtcNow;
            newUser.lastLogIn = newUser.dateCreated;
            newUser.visible = true;

            if (byAdmin == true)
            {
                newUser.createdBy = currUser.ID;
            }
            else
            {
                newUser.createdBy = system.ID;
            }
            newUser.rating = 0;

            AddUser(userContext, objectContext, newUser, bLog, secQuestion, secAnswer, byAdmin, currUser);

            BusinessUserActions businessUserActions = new BusinessUserActions();

            if (newUser.type == "user")
            {
                businessUserActions.AddUserRolesForNewUser(userContext, objectContext, newUser.ID, bLog);
            }
            else if (newUser.type == "writer")
            {
                businessUserActions.AddUserRolesForNewWriter(userContext, objectContext, newUser.ID, bLog);
            }
            else
            {
                throw new BusinessException(string.Format("User type = {0} is wrong, should be user or writer ", newUser.type));
            }
        }

        /// <summary>
        /// Counts how many users are registered(or their current email adress is) with email = 'text' 
        /// </summary>
        public int CountRegisteredUsersWithMail(EntitiesUsers userContext, string text)
        {
            Tools.AssertObjectContextExists(userContext);
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            int count = 0;

            int usersRegisteredWithMail = userContext.UserOptionsSet.Count<UserOptions>(uo => uo.registeredWithMail == text);
            int actualUserMailsCount = userContext.UserSet.Count<User>(usr => usr.email == text);

            if (usersRegisteredWithMail >= actualUserMailsCount)
            {
                count = usersRegisteredWithMail;
            }
            else
            {
                count = actualUserMailsCount;
            }

            return count;
        }

        /// <summary>
        /// Resets user password (and hashes it) , and returns the new password (non hashed)
        /// </summary>
        public string ResetUserPassword(EntitiesUsers userContext, Entities objectContext, BusinessLog bLog, User user)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            byte[] salt = new byte[5];
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();
            Gen.GetBytes(salt);
            List<byte> byteList = salt.ToList();
            StringBuilder sbPassword = new StringBuilder();
            foreach (byte number in byteList)
            {
                sbPassword.Append(number.ToString());
            }

            string newPassword = sbPassword.ToString();

            user.password = GetHashed(newPassword);
            Tools.Save(userContext);

            bLog.LogUser(objectContext, userContext, user, LogType.edit, "password", "*******", user);

            return newPassword;
        }

        /// <summary>
        /// returns the ID`s of all admin team users
        /// </summary>
        public List<long> AdminsIDsList(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<long> adminsList = new List<long>();

            IEnumerable<User> admins = GetAllAdministrators(false);
            if (admins != null && admins.Count() > 0)
            {
                foreach (User admin in admins)
                {
                    adminsList.Add(admin.ID);
                }
            }

            return adminsList;
        }

        /// <summary>
        /// True if user have role, otherwise false (if user is admin checks appropriate admin`s role)
        /// </summary>
        public bool CanUserDo(EntitiesUsers userContext, long userID, UserRoles role)
        {
            Tools.AssertObjectContextExists(userContext);

            User user = GetWithoutVisible(userContext, userID, true);

            bool result = CanUserDo(userContext, user, role);
            return result;
        }

        /// <summary>
        /// True if user have role, otherwise false (if user is admin checks appropriate admin`s role)
        /// </summary>
        public bool CanUserDo(EntitiesUsers userContext, User user, UserRoles role)
        {
            Tools.AssertObjectContextExists(userContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            bool result = false;

            UserTypes userType = GetUserType(user.type);
            bool admin = false;
            if (userType == UserTypes.Administrator || userType == UserTypes.GlobalAdministrator
                || userType == UserTypes.Moderator)
            {
                admin = true;
            }

            string userRole = GetUserRoleFromEnum(role);

            if (userType != UserTypes.System)
            {
                switch (role)
                {
                    case UserRoles.AddCompanies:
                        if (admin)
                        {
                            result = CanAdminDo(userContext, user, AdminRoles.EditCompanies);
                        }
                        else if (userType == UserTypes.User)
                        {
                            result = ActionAllowed(userContext, user.ID, userRole);
                        }
                        break;
                    case UserRoles.AddProducts:
                        if (admin)
                        {
                            result = CanAdminDo(userContext, user, AdminRoles.EditProducts);
                        }
                        else if (userType == UserTypes.User)
                        {
                            result = ActionAllowed(userContext, user.ID, userRole);
                        }
                        break;
                    case UserRoles.HaveSignature:
                        if (userType == UserTypes.User)
                        {
                            result = ActionAllowed(userContext, user.ID, userRole);
                        }
                        break;
                    case UserRoles.RateComments:
                        if (!admin)
                        {
                            result = ActionAllowed(userContext, user.ID, userRole);
                        }
                        break;
                    case UserRoles.RateProducts:
                        if (!admin)
                        {
                            result = ActionAllowed(userContext, user.ID, userRole);
                        }
                        break;
                    case UserRoles.RateUsers:
                        if (!admin)
                        {
                            result = ActionAllowed(userContext, user.ID, userRole);
                        }
                        break;
                    case UserRoles.ReportInappropriate:
                        if (!admin)
                        {
                            result = ActionAllowed(userContext, user.ID, userRole);
                        }
                        break;
                    case UserRoles.WriteCommentsAndMessages:
                        if (admin)
                        {
                            result = CanAdminDo(userContext, user, AdminRoles.EditComments);
                        }
                        else
                        {
                            result = ActionAllowed(userContext, user.ID, userRole);
                        }
                        break;
                    case UserRoles.WriteSuggestions:
                        if (!admin)
                        {
                            result = ActionAllowed(userContext, user.ID, userRole);
                        }
                        break;
                    default:
                        throw new BusinessException(string.Format("UserRole = {0} is not supported role", role));
                }
            }

            return result;
        }

        /// <summary>
        /// True if admin have role
        /// </summary>
        public bool CanAdminDo(EntitiesUsers userContext, long adminID, AdminRoles role)
        {
            Tools.AssertObjectContextExists(userContext);

            User admin = GetWithoutVisible(userContext, adminID, true);

            bool result = CanAdminDo(userContext, admin, role);
            return result;
        }

        /// <summary>
        ///  True if admin have role
        /// </summary>
        public bool CanAdminDo(EntitiesUsers userContext, User user, AdminRoles role)
        {
            Tools.AssertObjectContextExists(userContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            UserTypes userType = GetUserType(user.type);
            if (userType != UserTypes.Administrator && userType != UserTypes.GlobalAdministrator
                && userType != UserTypes.Moderator)
            {
                return false;
            }

            string adminRole = GetAdminRoleFromEnum(role);

            bool result = false;
            UserAction userAction = null;

            switch (role)
            {
                case AdminRoles.EditAdministrators:
                    if (userType == UserTypes.GlobalAdministrator)
                    {
                        userAction = userContext.UserActionSet.FirstOrDefault
                        (ua => ua.User.ID == user.ID && ua.Action.name == adminRole && ua.visible == true);
                    }
                    break;
                case AdminRoles.EditCategories:
                    if (userType == UserTypes.GlobalAdministrator)
                    {
                        userAction = userContext.UserActionSet.FirstOrDefault
                        (ua => ua.User.ID == user.ID && ua.Action.name == adminRole && ua.visible == true);
                    }
                    break;
                case AdminRoles.EditComments:
                    userAction = userContext.UserActionSet.FirstOrDefault
               (ua => ua.User.ID == user.ID && ua.Action.name == adminRole && ua.visible == true);
                    break;
                case AdminRoles.EditCompanies:
                    if (userType == UserTypes.GlobalAdministrator || userType == UserTypes.Administrator)
                    {
                        userAction = userContext.UserActionSet.FirstOrDefault
                        (ua => ua.User.ID == user.ID && ua.Action.name == adminRole && ua.visible == true);
                    }
                    break;
                case AdminRoles.EditGlobalAdministrators:
                    if (userType == UserTypes.GlobalAdministrator)
                    {
                        userAction = userContext.UserActionSet.FirstOrDefault
                        (ua => ua.User.ID == user.ID && ua.Action.name == adminRole && ua.visible == true);
                    }
                    break;
                case AdminRoles.EditModerators:
                    if (userType == UserTypes.GlobalAdministrator || userType == UserTypes.Administrator)
                    {
                        userAction = userContext.UserActionSet.FirstOrDefault
                        (ua => ua.User.ID == user.ID && ua.Action.name == adminRole && ua.visible == true);
                    }
                    break;
                case AdminRoles.EditProducts:
                    if (userType == UserTypes.GlobalAdministrator || userType == UserTypes.Administrator)
                    {
                        userAction = userContext.UserActionSet.FirstOrDefault
                        (ua => ua.User.ID == user.ID && ua.Action.name == adminRole && ua.visible == true);
                    }
                    break;
                case AdminRoles.EditUsers:
                    userAction = userContext.UserActionSet.FirstOrDefault
              (ua => ua.User.ID == user.ID && ua.Action.name == adminRole && ua.visible == true);
                    break;
                default:
                    throw new BusinessException(string.Format("AdminRole = {0} is not supported role", role));
            }

            if (userAction != null)
            {
                result = true;
            }

            return result;
        }



        /// <summary>
        /// returns User`s type 
        /// </summary>
        public static UserTypes GetUserType(string strtype)
        {
            if (string.IsNullOrEmpty(strtype))
            {
                throw new BusinessException("strtype is null or empty");
            }

            UserTypes type;

            switch (strtype)
            {
                case "user":
                    type = UserTypes.User;
                    break;
                case "writer":
                    type = UserTypes.Writer;
                    break;
                case "system":
                    type = UserTypes.System;
                    break;
                case "global":
                    type = UserTypes.GlobalAdministrator;
                    break;
                case "moderator":
                    type = UserTypes.Moderator;
                    break;
                case "administrator":
                    type = UserTypes.Administrator;
                    break;
                default:
                    throw new BusinessException(string.Format("User type = {0} is not supported type", strtype));
            }

            return type;
        }

        private string GetUserRoleFromEnum(UserRoles role)
        {
            string result = "";

            switch (role)
            {
                case UserRoles.AddCompanies:
                    result = "company";
                    break;
                case UserRoles.AddProducts:
                    result = "product";
                    break;
                case UserRoles.HaveSignature:
                    result = "signature";
                    break;
                case UserRoles.RateProducts:
                    result = "prater";
                    break;
                case UserRoles.RateUsers:
                    result = "userrater";
                    break;
                case UserRoles.ReportInappropriate:
                    result = "flagger";
                    break;
                case UserRoles.WriteCommentsAndMessages:
                    result = "commenter";
                    break;
                case UserRoles.WriteSuggestions:
                    result = "suggestor";
                    break;
                case UserRoles.RateComments:
                    result = "commrater";
                    break;
                default:
                    throw new BusinessException(string.Format("UserRole = {0} is not supported role", role));
            }

            return result;
        }

        public UserRoles GetUserRoleFromString(string role)
        {
            UserRoles result = UserRoles.AddCompanies;

            switch (role)
            {
                case "company":
                    result = UserRoles.AddCompanies;
                    break;
                case "product":
                    result = UserRoles.AddProducts;
                    break;
                case "signature":
                    result = UserRoles.HaveSignature;
                    break;
                case "prater":
                    result = UserRoles.RateProducts;
                    break;
                case "userrater":
                    result = UserRoles.RateUsers;
                    break;
                case "flagger":
                    result = UserRoles.ReportInappropriate;
                    break;
                case "commenter":
                    result = UserRoles.WriteCommentsAndMessages;
                    break;
                case "suggestor":
                    result = UserRoles.WriteSuggestions;
                    break;
                case "commrater":
                    result = UserRoles.RateComments;
                    break;
                default:
                    throw new BusinessException(string.Format("role = {0} is not supported role", role));
            }

            return result;
        }

        public string GetAdminRoleFromEnum(AdminRoles role)
        {
            string result = "";

            switch (role)
            {
                case AdminRoles.EditAdministrators:
                    result = "aCreator";
                    break;
                case AdminRoles.EditCategories:
                    result = "category";
                    break;
                case AdminRoles.EditComments:
                    result = "acomments";
                    break;
                case AdminRoles.EditCompanies:
                    result = "acompanies";
                    break;
                case AdminRoles.EditGlobalAdministrators:
                    result = "gCreator";
                    break;
                case AdminRoles.EditModerators:
                    result = "mCreator";
                    break;
                case AdminRoles.EditProducts:
                    result = "aproducts";
                    break;
                case AdminRoles.EditUsers:
                    result = "ueditor";
                    break;
                default:
                    throw new BusinessException(string.Format("ADminRole = {0} is not supported role", role));
            }

            return result;
        }

        private string GetUserTypeFromEnum(UserTypes type)
        {
            string result = "";

            switch (type)
            {
                case UserTypes.Administrator:
                    result = "administrator";
                    break;
                case UserTypes.GlobalAdministrator:
                    result = "global";
                    break;
                case UserTypes.Moderator:
                    result = "moderator";
                    break;
                case UserTypes.User:
                    result = "user";
                    break;
                case UserTypes.Writer:
                    result = "writer";
                    break;
                case UserTypes.System:
                    result = "system";
                    break;
                default:
                    throw new BusinessException(string.Format("UserType = {0} is not supported type"));
            }

            return result;
        }


        /// <summary>
        /// Returns true if currAdmin`s level is equal or higher than fromAdmin`s level. Can be used if one or both are users.
        /// Global Administrator > Administrator > Moderator > User/Writer
        /// </summary>
        public static bool CanAdminEditStuffFromUser(User currAdmin, User fromAdmin)
        {
            if (currAdmin == null)
            {
                throw new BusinessException("currAdmin is null");
            }
            if (fromAdmin == null)
            {
                throw new BusinessException("fromAdmin is null");
            }

            bool result = false;

            UserTypes currAdminType = GetUserType(currAdmin.type);
            UserTypes fromAdminType = GetUserType(fromAdmin.type);

            if (currAdminType == UserTypes.GlobalAdministrator || currAdminType == UserTypes.Administrator
                || currAdminType == UserTypes.Moderator)
            {
                if (fromAdminType == UserTypes.GlobalAdministrator || fromAdminType == UserTypes.Administrator
                || fromAdminType == UserTypes.Moderator)
                {
                    switch (currAdminType)
                    {
                        case UserTypes.GlobalAdministrator:
                            result = true;
                            break;
                        case UserTypes.Administrator:
                            if (fromAdminType != UserTypes.GlobalAdministrator)
                            {
                                result = true;
                            }
                            break;
                        case UserTypes.Moderator:
                            if (fromAdminType != UserTypes.GlobalAdministrator && fromAdminType != UserTypes.Administrator)
                            {
                                result = true;
                            }
                            break;
                        default:
                            throw new BusinessException(string.Format("currAdminType = {0} is not supported type.", currAdminType));
                    }
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns True if Admin can edit user/admin, otherwise false
        /// </summary>
        public bool CanAdminEditUserOrAdmin(EntitiesUsers userContext, User currAdmin, User userToEdit)
        {

            if (currAdmin == null)
            {
                throw new BusinessException("currAdmin is null");
            }
            if (userToEdit == null)
            {
                throw new BusinessException("userToEdit is null");
            }

            bool result = false;

            UserTypes currAdminType = GetUserType(currAdmin.type);
            UserTypes userToEditType = GetUserType(userToEdit.type);

            if (currAdminType == UserTypes.GlobalAdministrator || currAdminType == UserTypes.Administrator
                || currAdminType == UserTypes.Moderator)
            {
                if (userToEditType == UserTypes.GlobalAdministrator || userToEditType == UserTypes.Administrator
                || userToEditType == UserTypes.Moderator)
                {
                    switch (currAdminType)
                    {
                        case UserTypes.GlobalAdministrator:

                            switch (userToEditType)
                            {
                                case UserTypes.GlobalAdministrator:  // Global admin which is created by System can be modified only by another global admin created by System
                                    if (CanAdminDo(userContext, currAdmin, AdminRoles.EditGlobalAdministrators) == true)
                                    {
                                        User System = GetSystem(userContext);
                                        User createdBy = GetWithoutVisible(userContext, userToEdit.createdBy, false);

                                        if (createdBy != null && createdBy.ID == System.ID)
                                        {
                                            createdBy = GetWithoutVisible(userContext, currAdmin.createdBy, false);
                                            if (createdBy != null && createdBy.ID == System.ID)
                                            {
                                                result = true;
                                            }
                                        }
                                        else
                                        {
                                            result = true;
                                        }
                                    }
                                    break;
                                case UserTypes.Administrator:
                                    if (CanAdminDo(userContext, currAdmin, AdminRoles.EditAdministrators) == true)
                                    {
                                        result = true;
                                    }
                                    break;
                                case UserTypes.Moderator:
                                    if (CanAdminDo(userContext, currAdmin, AdminRoles.EditModerators) == true)
                                    {
                                        result = true;
                                    }
                                    break;
                                default: result = false;
                                    break;
                            }
                            break;
                        case UserTypes.Administrator:
                            switch (userToEditType)
                            {
                                case UserTypes.Administrator:
                                    if (CanAdminDo(userContext, currAdmin, AdminRoles.EditAdministrators) == true)
                                    {
                                        result = true;
                                    }
                                    break;
                                case UserTypes.Moderator:
                                    if (CanAdminDo(userContext, currAdmin, AdminRoles.EditModerators) == true)
                                    {
                                        result = true;
                                    }
                                    break;
                                default: result = false;
                                    break;
                            }
                            break;
                        case UserTypes.Moderator:
                            switch (userToEditType)
                            {
                                case UserTypes.Moderator:
                                    if (CanAdminDo(userContext, currAdmin, AdminRoles.EditModerators) == true)
                                    {
                                        result = true;
                                    }
                                    break;
                                default: result = false;
                                    break;
                            }
                            break;
                        default:
                            throw new BusinessException(string.Format("currAdminType = {0} is not supported type.", currAdminType));
                    }
                }
                else if (CanAdminDo(userContext, currAdmin, AdminRoles.EditUsers))
                {
                    result = true;
                }
            }

            return result;
        }

        public List<User> GetLastDeletedUsers(EntitiesUsers userContext, int number, string nameContains, long userId)
        {
            Tools.AssertObjectContextExists(userContext);

            List<User> users = new List<User>();

            if (!string.IsNullOrEmpty(nameContains))
            {
                if (userId > 0)
                {
                    users = userContext.GetLastDeletedUsers(nameContains, (long)number, userId).ToList();
                }
                else
                {
                    users = userContext.GetLastDeletedUsers(nameContains, (long)number, null).ToList();
                }
            }
            else
            {
                if (userId > 0)
                {
                    users = userContext.GetLastDeletedUsers(null, (long)number, userId).ToList();
                }
                else
                {
                    users = userContext.GetLastDeletedUsers(null, (long)number, null).ToList();
                }
            }

            return users;
        }
    }
}
