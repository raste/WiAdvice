﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessWarnings
    {

        /// <summary>
        /// If it`s general, leave uaction null and set role = "general"
        /// </summary>
        public void AddWarning(EntitiesUsers userContext, Entities objectContext, UserAction uaction, string role
            , string description, User forUser, User currAdmin, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (string.IsNullOrEmpty(role))
            {
                throw new BusinessException("role is empty");
            }
            if (uaction == null && role != "general")
            {
                throw new BusinessException("uaction is null and role is not general");
            }
            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }
            if (currAdmin == null)
            {
                throw new BusinessException("currAdmin is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }

            BusinessUser bUser = new BusinessUser();
            if (bUser.IsFromAdminTeam(forUser))
            {
                return;
            }
            if (forUser.ID == currAdmin.ID)
            {
                return;
            }

            Warning newWarning = new Warning();

            newWarning.ByAdmin = currAdmin;
            newWarning.dateCreated = DateTime.UtcNow;
            newWarning.lastModified = newWarning.dateCreated;
            newWarning.description = description;
            newWarning.ModifiedBy = currAdmin;
            newWarning.modifiedReason = null;
            newWarning.User = forUser;
            newWarning.UserAction = uaction;
            newWarning.visible = true;

            userContext.AddToWarningSet(newWarning);
            Tools.Save(userContext);

            if (uaction != null)
            {
                // CHECK IF SHOULD REMOVE ROLE AND REMOVE IT
                RemoveUserActionIfConditionMet(userContext, objectContext, uaction, forUser, bLog);
            }

            BusinessUserOptions bUserOptions = new BusinessUserOptions();
            bUserOptions.IncreaseUserWarnings(userContext, forUser);
            bUserOptions.ChangeIfUserHaveNewWarning(userContext, forUser, true);

            // CHECK IF SHOULD REMOVE USER AND REMOVE IT
            RemoveUserIfConditionMet(userContext, objectContext, forUser, bLog);
        }


        public void AddTypeWarning(EntitiesUsers userContext, Entities objectContext, UsersTypeAction uaction
            , string description, User forUser, User currAdmin, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (uaction == null)
            {
                throw new BusinessException("uaction is null");
            }
            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }
            if (currAdmin == null)
            {
                throw new BusinessException("currAdmin is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }

            TypeWarning newWarning = new TypeWarning();

            newWarning.ByAdmin = Tools.GetUserID(objectContext, currAdmin);
            newWarning.dateCreated = DateTime.UtcNow;
            newWarning.lastModified = newWarning.dateCreated;
            newWarning.description = description;
            newWarning.ModifiedBy = newWarning.ByAdmin;
            newWarning.modifiedReason = null;
            newWarning.User = Tools.GetUserID(objectContext, forUser);
            newWarning.UserTypeAction = uaction;
            newWarning.visible = true;

            objectContext.AddToTypeWarningSet(newWarning);
            Tools.Save(objectContext);

            // CHECK IF SHOULD REMOVE ROLE AND REMOVE IT
            RemoveUserTypeActionIfConditionMet(userContext, objectContext, uaction, forUser, bLog);

            BusinessUserOptions bUserOptions = new BusinessUserOptions();
            bUserOptions.IncreaseUserWarnings(userContext, forUser);
            bUserOptions.ChangeIfUserHaveNewWarning(userContext, forUser, true);

            // CHECK IF SHOULD REMOVE USER AND REMOVE IT
            RemoveUserIfConditionMet(userContext, objectContext, forUser, bLog);
        }


        /// <summary>
        /// Deletes user type warning..only global administrators can do that
        /// </summary>
        public void DeleteTypeWarning(EntitiesUsers userContext, Entities objectContext, TypeWarning warning
            , User forUser, User currAdmin, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (warning == null)
            {
                throw new BusinessException("warning is null");
            }
            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }
            if (currAdmin == null)
            {
                throw new BusinessException("currAdmin is null");
            }

            if (!warning.UserReference.IsLoaded)
            {
                warning.UserReference.Load();
            }

            if (warning.User.ID != forUser.ID)
            {
                throw new BusinessException(string.Format("Warning id = {0} is not for user id = {1}, because of that admin id = {2}, cannot delete that warning!"
                    , warning.ID, forUser.ID, currAdmin.ID));
            }

            if (warning.visible == false)
            {
                return;
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.IsGlobalAdministrator(currAdmin) || !bUser.CanAdminDo(userContext, currAdmin, AdminRoles.EditUsers))
            {
                throw new BusinessException(string.Format("User ID = {0}, is not global admin or cannot edit users, because of that he cannot delete user id = {1} warning"
                    , currAdmin.ID, forUser.ID));
            }

            warning.visible = false;
            Tools.Save(objectContext);

            BusinessUserOptions bUserOptions = new BusinessUserOptions();
            bUserOptions.DecreaseUserWarnings(userContext, forUser);

            string sysMsg = string.Format("{0} <br/> \"{1}\"", Tools.GetResource("SMwarningRemoved"), warning.description);
            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
            bSystemMessages.Add(userContext, forUser, sysMsg);
        }

        /// <summary>
        /// Deletes user warning..only global administrators can do that
        /// </summary>
        public void DeleteWarning(EntitiesUsers userContext, Warning warning
            , User forUser, User currAdmin, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (warning == null)
            {
                throw new BusinessException("warning is null");
            }
            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }
            if (currAdmin == null)
            {
                throw new BusinessException("currAdmin is null");
            }

            if (!warning.UserReference.IsLoaded)
            {
                warning.UserReference.Load();
            }

            if (warning.User.ID != forUser.ID)
            {
                throw new BusinessException(string.Format("Warning id = {0} is not for user id = {1}, because of that admin id = {2}, cannot delete that warning!"
                    , warning.ID, forUser.ID, currAdmin.ID));
            }

            if (warning.visible == false)
            {
                return;
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.IsGlobalAdministrator(currAdmin) || !bUser.CanAdminDo(userContext, currAdmin, AdminRoles.EditUsers))
            {
                throw new BusinessException(string.Format("User ID = {0}, is not global admin or cannot edit users, because of that he cannot delete user id = {1} warning"
                    , currAdmin.ID, forUser.ID));
            }

            warning.visible = false;
            Tools.Save(userContext);

            BusinessUserOptions bUserOptions = new BusinessUserOptions();
            bUserOptions.DecreaseUserWarnings(userContext, forUser);

            string sysMsg = string.Format("{0} <br/> \"{1}\"", Tools.GetResource("SMwarningRemoved"), warning.description);
            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
            bSystemMessages.Add(userContext, forUser, sysMsg);
        }

        /// <summary>
        /// Checks if new Warning fields are OK, returs true if everything`s ok, otherwise false with error
        /// </summary>
        public bool CheckWarning(EntitiesUsers userContext, Entities objectContext, string role
            , string description, User forUser, User currAdmin, out string error)
        {
            if (string.IsNullOrEmpty(role))
            {
                throw new BusinessException("role is empty");
            }


            bool result = true;

            error = string.Empty;
            StringBuilder errors = new StringBuilder();

            string commonData = string.Empty;
            if (!CheckCommonData(userContext, objectContext, forUser, currAdmin, description, out commonData))
            {
                result = false;
                errors.Append(commonData);
            }

            if (role != "general")
            {
                BusinessUser bUser = new BusinessUser();
                UserRoles roleChosen = bUser.GetUserRoleFromString(role);

                bool checksOk = true;

                if (bUser.IsModerator(currAdmin))
                {
                    if (roleChosen == UserRoles.AddCompanies || roleChosen == UserRoles.AddProducts)
                    {
                        errors.Append("You can't send warning for this role.");
                        checksOk = false;
                    }
                }

                if (checksOk == true)
                {
                    BusinessUserActions userActions = new BusinessUserActions();
                    UserAction userAction = userActions.GetUserAction(userContext, roleChosen, forUser);

                    if (userAction == null)
                    {
                        errors.Append(string.Format("{0} doesn`t have action {1}.<br />", forUser.username, roleChosen));
                        result = false;
                    }
                    else if (userAction.visible == false)
                    {
                        errors.Append(string.Format("{0}`s action {1} is already removed.<br />", forUser.username, roleChosen));
                        result = false;
                    }
                }
            }

            error = errors.ToString();
            return result;
        }


        /// <summary>
        /// Checks if new Warning fields are OK, returs true if everything`s ok, otherwise false with error
        /// </summary>
        public bool CheckTypeWarning(EntitiesUsers userContext, Entities objectContext, string type, long typeID
            , string description, User forUser, User currAdmin, out string error)
        {
            bool result = true;

            StringBuilder errors = new StringBuilder();

            string commonData = string.Empty;
            if (!CheckCommonData(userContext, objectContext, forUser, currAdmin, description, out commonData))
            {
                result = false;
                errors.Append(commonData);
            }

            if (type != "product" && type != "company" && type != "aCompProdModificator")
            {
                errors.Append("Warning type must be product, company or all company products.<br />");
                result = false;
            }
            if (typeID < 1)
            {
                errors.Append("Type id of type.<br />");
                result = false;
            }

            string typeError = type;
            if (type == "aCompProdModificator")
            {
                typeError = "all company products";
            }

            BusinessUser bUser = new BusinessUser();
            if (bUser.IsModerator(currAdmin))
            {
                errors.Append("You can't send warning for type roles.");
                result = false;
            }

            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();
            UsersTypeAction userAction = businessUserTypeActions.GetUserTypeAction(objectContext, type, typeID, forUser);
            if (userAction == null)
            {
                errors.Append(string.Format("{0} doesn`t have action for {1} {2}.<br />", forUser.username, typeError, typeID));
                result = false;
            }
            else if (userAction.visible == false)
            {
                errors.Append(string.Format("{0}`s action for {1} {2} is already removed.<br />", forUser.username, typeError, typeID));
                result = false;
            }

            error = errors.ToString();
            return result;
        }


        private bool CheckCommonData(EntitiesUsers userContext, Entities objectContext, User forUser
            , User currAdmin, string description, out string error)
        {
            bool result = true;

            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }
            if (currAdmin == null)
            {
                throw new BusinessException("currAdmin is null");
            }

            error = string.Empty;
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(description))
            {
                errors.Append("Type warning description.<br />");
                result = false;
            }
            else if (description.Length > Configuration.FieldsMaxDescriptionFieldLength)
            {
                errors.Append("Warning description length is too much.<br />");
                result = false;
            }

            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsFromUserTeam(forUser))
            {
                errors.Append(string.Format("{0} is not from users team.<br />", forUser.username));
                result = false;
            }

            if (forUser.visible == false)
            {
                errors.Append(string.Format("{0} is not visible.<br />", forUser.username));
                result = false;
            }

            BusinessUserOptions businessUserOptions = new BusinessUserOptions();
            if (!businessUserOptions.IsUserActivated(forUser))
            {
                errors.Append(string.Format("{0} is not activated.<br />", forUser.username));
                result = false;
            }

            if (!businessUser.CanAdminDo(userContext, currAdmin, AdminRoles.EditUsers))
            {
                errors.Append(string.Format("You cannot edit users.<br />", forUser.username));
                result = false;
            }

            if (businessUser.IsFromUserTeam(currAdmin))
            {
                throw new BusinessException(string.Format("User '{0}' ID : {1} , cannot add warning, because he is from user team"
                    , currAdmin.username, currAdmin.ID));
            }

            error = errors.ToString();
            return result;
        }


        private void RemoveUserActionIfConditionMet(EntitiesUsers userContext, Entities objectContext, UserAction uaction, User forUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (uaction == null)
            {
                throw new BusinessException("uaction is null");
            }
            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }
            if (uaction.visible == false)
            {
                throw new BusinessException(string.Format("User action id : {0} is already visible false", uaction.ID));
            }

            if (CountUserWarningsAboutAction(userContext, uaction, forUser) >=
                Configuration.WarningsNumberOnActionsOnWhichShouldRemoveAction)
            {
                BusinessUser businessUser = new BusinessUser();
                User system = businessUser.GetSystem(userContext);

                string description = "Excessive breaking of the site rules.";

                BusinessUserActions businessUserAction = new BusinessUserActions();
                businessUserAction.RemoveUserAction(userContext, objectContext, uaction, system, bLog, description);
            }

        }


        private void RemoveUserTypeActionIfConditionMet(EntitiesUsers userContext, Entities objectContext, UsersTypeAction uaction, User forUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (uaction == null)
            {
                throw new BusinessException("uaction is null");
            }
            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }
            if (uaction.visible == false)
            {
                throw new BusinessException(string.Format("User action id : {0} is already visible false", uaction.ID));
            }

            if (CountUserWarningsAboutTypeAction(objectContext, uaction, forUser) >=
                Configuration.WarningsNumberOnActionsOnWhichShouldRemoveAction)
            {
                BusinessUser businessUser = new BusinessUser();
                User system = businessUser.GetSystem(userContext);

                BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();
                businessUserTypeActions.RemoveUserTypeAction(objectContext, userContext, uaction, system, bLog, true);
            }

        }

        private void RemoveUserIfConditionMet(EntitiesUsers userContext, Entities objectContext, User forUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }
            if (forUser.visible == false)
            {
                throw new BusinessException(string.Format("User id : {0} is aready visible false", forUser.ID));
            }

            forUser.UserOptionsReference.Load();

            if (forUser.UserOptions.warnings >= Configuration.WarningsOnHowManyToDeleteUser)
            {
                BusinessUser businessUser = new BusinessUser();
                User system = businessUser.GetSystem(userContext);

                businessUser.DeleteUser(userContext, forUser, bLog, system);
            }
        }

        public int CountUserWarningsAboutTypeAction(Entities objectContext, UsersTypeAction uaction, User forUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (uaction == null)
            {
                throw new BusinessException("uaction is null");
            }
            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }

            int count = objectContext.TypeWarningSet.Count(war => war.User.ID == forUser.ID && war.UserTypeAction.ID == uaction.ID && war.visible == true);

            return count;
        }

        public int CountUserWarningsAboutAction(EntitiesUsers userContext, UserAction uaction, User forUser)
        {
            Tools.AssertObjectContextExists(userContext);

            if (uaction == null)
            {
                throw new BusinessException("uaction is null");
            }
            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }

            int count = userContext.WarningSet.Count(war => war.User.ID == forUser.ID && war.UserAction.ID == uaction.ID && war.visible == true);

            return count;
        }

        /// <summary>
        /// Returns visible true user warnings
        /// </summary>
        public List<Warning> GetUserWarnings(EntitiesUsers userContext, User forUser)
        {
            Tools.AssertObjectContextExists(userContext);

            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }

            List<Warning> warnings = userContext.WarningSet.Where(war => war.User.ID == forUser.ID && war.visible == true).ToList();
            warnings.Reverse();

            return warnings;
        }


        public Warning GetUserWarning(EntitiesUsers userContext, long id, bool onlyVisible, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(userContext);

            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }

            Warning warning = null;

            if (onlyVisible == true)
            {
                warning = userContext.WarningSet.FirstOrDefault(war => war.ID == id && war.visible == true);
            }
            else
            {
                warning = userContext.WarningSet.FirstOrDefault(war => war.ID == id);
            }

            if (throwExcIfNull == true && warning == null)
            {
                if (onlyVisible == true)
                {
                    throw new BusinessException(string.Format("There is no visible=true user warning ID = {0}", id));
                }
                else
                {
                    throw new BusinessException(string.Format("There is no user warning ID = {0}", id));
                }

            }

            return warning;
        }

        public TypeWarning GetUserTypeWarning(Entities objectContext, long id, bool onlyVisible, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }

            TypeWarning warning = null;

            if (onlyVisible == true)
            {
                warning = objectContext.TypeWarningSet.FirstOrDefault(war => war.ID == id && war.visible == true);
            }
            else
            {
                warning = objectContext.TypeWarningSet.FirstOrDefault(war => war.ID == id);
            }

            if (throwExcIfNull == true && warning == null)
            {
                if (onlyVisible == true)
                {
                    throw new BusinessException(string.Format("There is no visible=true user warning ID = {0}", id));
                }
                else
                {
                    throw new BusinessException(string.Format("There is no user warning ID = {0}", id));
                }

            }

            return warning;
        }

        /// <summary>
        /// Returns visible true user type warnings
        /// </summary>
        public List<TypeWarning> GetUserTypeWarnings(Entities objectContext, User forUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (forUser == null)
            {
                throw new BusinessException("forUser is null");
            }

            List<TypeWarning> warnings = objectContext.TypeWarningSet.Where(war => war.User.ID == forUser.ID && war.visible == true).ToList();
            warnings.Reverse();

            return warnings;
        }

    }
}
