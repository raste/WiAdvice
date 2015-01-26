﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessUserActions
    {
        /// <summary>
        /// Add`s role to administrator if he doesnt have it already
        /// </summary>
        public void AddAdminRole(EntitiesUsers userContext, Entities objectContext, User userToGetRole, User Approver, AdminRoles role, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (userToGetRole == null)
            {
                throw new BusinessException("userToGetRole is null");
            }
            if (Approver == null)
            {
                throw new BusinessException("Approver is null");
            }

            BusinessUser businessUser = new BusinessUser();

            if (!businessUser.IsFromAdminTeam(Approver))
            {
                throw new BusinessException(string.Format("User ID = {0} cannot receive roles from user id = {0}, because he is not administrator"
                    , userToGetRole.ID, Approver.ID));
            }

            if (!businessUser.IsFromAdminTeam(userToGetRole))
            {
                throw new BusinessException(string.Format("user id = {0} is not from admin team and cannot receive role {1}"
                    , userToGetRole.ID, role));
            }

            UserTypes userType = BusinessUser.GetUserType(userToGetRole.type);
            string strRole = businessUser.GetAdminRoleFromEnum(role);

            switch (role)
            {
                case AdminRoles.EditAdministrators:
                    if (userType == UserTypes.GlobalAdministrator)
                    {
                        AddUserAction(userContext, objectContext, userToGetRole.ID, Approver.ID, strRole, bLog, false);
                    }
                    break;
                case AdminRoles.EditCategories:
                    if (userType == UserTypes.GlobalAdministrator)
                    {
                        AddUserAction(userContext, objectContext, userToGetRole.ID, Approver.ID, strRole, bLog, false);
                    }
                    break;
                case AdminRoles.EditComments:
                    AddUserAction(userContext, objectContext, userToGetRole.ID, Approver.ID, strRole, bLog, false);
                    break;
                case AdminRoles.EditCompanies:
                    if (userType == UserTypes.GlobalAdministrator || userType == UserTypes.Administrator)
                    {
                        AddUserAction(userContext, objectContext, userToGetRole.ID, Approver.ID, strRole, bLog, false);
                    }
                    break;
                case AdminRoles.EditGlobalAdministrators:
                    if (userType == UserTypes.GlobalAdministrator)
                    {
                        AddUserAction(userContext, objectContext, userToGetRole.ID, Approver.ID, strRole, bLog, false);
                    }
                    break;
                case AdminRoles.EditModerators:
                    if (userType == UserTypes.GlobalAdministrator || userType == UserTypes.Administrator)
                    {
                        AddUserAction(userContext, objectContext, userToGetRole.ID, Approver.ID, strRole, bLog, false);
                    }
                    break;
                case AdminRoles.EditProducts:
                    if (userType == UserTypes.GlobalAdministrator || userType == UserTypes.Administrator)
                    {
                        AddUserAction(userContext, objectContext, userToGetRole.ID, Approver.ID, strRole, bLog, false);
                    }
                    break;
                case AdminRoles.EditUsers:
                    AddUserAction(userContext, objectContext, userToGetRole.ID, Approver.ID, strRole, bLog, false);
                    break;
                default:
                    throw new BusinessException(string.Format("AdminRole = {0} is not supported role", role));
            }
        }

        public void AddUserAction(EntitiesUsers userContext, Entities objectContext
            , long userID, long approvedBy, string rolename, BusinessLog bLog, bool SendSystemMessage)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);
            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }

            if (approvedBy < 1)
            {
                throw new BusinessException("approvedBy is < 1");
            }

            if (rolename == null || rolename == string.Empty)
            {
                throw new BusinessException("invalid role name , its either null or empty");
            }

            Actions currAction = userContext.ActionsSet.FirstOrDefault(action => action.name == rolename);
            if (currAction == null)
            {
                throw new BusinessException(string.Format("There`s no action with name = {0}", rolename));
            }

            BusinessUser businessUser = new BusinessUser();

            User user = businessUser.Get(userContext, userID, true);
            User approver = businessUser.Get(userContext, approvedBy, true);

            List<UserAction> useractions = userContext.UserActionSet.Where(ua => ua.User.ID == userID && ua.Action.name == rolename).ToList();
            if (useractions.Count<UserAction>() != 0)
            {
                if (useractions.Count<UserAction>() == 1)
                {
                    UserAction uaction = useractions.First<UserAction>();
                    if (!uaction.visible)
                    {
                        uaction.visible = true;
                        Tools.Save(userContext);

                        bLog.LogRole(objectContext, userContext, uaction, LogType.undelete, approver);
                    }
                }
                else if (useractions.Count<UserAction>() > 1)
                {
                    throw new BusinessException(string.Format("The user id = {0} have the action = {1} more than once already", userID, rolename));
                }
            }
            else
            {
                UserAction newAction = new UserAction();

                newAction.User = user;
                newAction.Action = currAction;
                newAction.dateCreated = DateTime.UtcNow;
                newAction.visible = true;
                newAction.ApprovedBy = approver;

                userContext.AddToUserActionSet(newAction);
                Tools.Save(userContext);

                bLog.LogRole(objectContext, userContext, newAction, LogType.create, approver);
            }

            if (businessUser.IsFromUserTeam(user) && SendSystemMessage == true)
            {
                string message = string.Empty;

                switch (rolename)
                {
                    case "commenter":
                        message = Tools.GetResource("AddUserActionCommenter");
                        break;
                    case "product":
                        message = Tools.GetResource("AddUserActionAddProducts");
                        break;
                    case "company":
                        message = Tools.GetResource("AddUserActionAddMakers");
                        break;
                    case "prater":
                        message = Tools.GetResource("AddUserActionRateProducts");
                        break;
                    case "commrater":
                        message = Tools.GetResource("AddUserActionRateComments");
                        break;
                    case "userrater":
                        message = Tools.GetResource("AddUserActionRateUsers");
                        break;
                    case "flagger":
                        message = Tools.GetResource("AddUserActionSendReports");
                        break;
                    case "suggestor":
                        message = Tools.GetResource("AddUserActionSendSuggestions");
                        break;
                    case "signature":
                        message = Tools.GetResource("AddUserActionHaveSignature");
                        break;
                    default:
                        throw new BusinessException(string.Format("Action name : {0} is not supported action for which to send System message."
                            , rolename));
                }

                BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                bSystemMessages.Add(userContext, user, message);
            }
        }


        /// <summary>
        /// Add`s standart roles for new Global Administrator
        /// </summary>
        public void AddUserRolesForNewGlobalAdministrator(Entities objectContext, EntitiesUsers userContext, long userID, long approvedBy, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            AddUserAction(userContext, objectContext, userID, approvedBy, "category", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "gCreator", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "aCreator", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "mCreator", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "acompanies", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "aproducts", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "acomments", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "ueditor", bLog, false);
        }

        /// <summary>
        /// Add`s standart roles for new Administrator
        /// </summary>
        public void AddUserRolesForNewAdministrator(EntitiesUsers userContext, Entities objectContext, long userID, long approvedBy, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            AddUserAction(userContext, objectContext, userID, approvedBy, "mCreator", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "acompanies", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "aproducts", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "acomments", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "ueditor", bLog, false);
        }

        /// <summary>
        /// Add`s standart roles for new Moderator
        /// </summary>
        public void AddUserRolesForNewModerator(EntitiesUsers userContext, Entities objectContext, long userID, long approvedBy, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            AddUserAction(userContext, objectContext, userID, approvedBy, "acomments", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "ueditor", bLog, false);
        }

        /// <summary>
        /// Add`s standart rolse for new user
        /// </summary>
        public void AddUserRolesForNewUser(EntitiesUsers userContext, Entities objectContext, long userID, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            User system = Tools.GetSystem();
            if (system == null)
            {
                throw new BusinessException("System user is null");
            }

            long approvedBy = system.ID;

            AddUserAction(userContext, objectContext, userID, approvedBy, "commenter", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "product", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "company", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "prater", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "commrater", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "userrater", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "flagger", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "suggestor", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "signature", bLog, false);
        }

        /// <summary>
        /// Add`s standart roles for new Writer
        /// </summary>
        public void AddUserRolesForNewWriter(EntitiesUsers userContext, Entities objectContext, long userID, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            User system = Tools.GetSystem();
            if (system == null)
            {
                throw new BusinessException("System user is null");
            }

            long approvedBy = system.ID;

            AddUserAction(userContext, objectContext, userID, approvedBy, "commenter", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "prater", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "commrater", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "userrater", bLog, false);
            AddUserAction(userContext, objectContext, userID, approvedBy, "flagger", bLog, false);
        }

        /// <summary>
        /// Returns action with ID
        /// </summary>
        public Actions GetAction(EntitiesUsers userContext, long id)
        {
            Tools.AssertObjectContextExists(userContext);
            Actions action = userContext.ActionsSet.FirstOrDefault<Actions>
                (act => act.ID == id);
            return action;
        }


        /// <summary>
        /// Returns UserAction ..doesn`t check for visibility, if there`s no such returns null
        /// </summary>
        public UserAction GetUserAction(EntitiesUsers userContext, UserRoles role, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }


            Actions action = GetActionForUser(userContext, role);
            UserAction currAction = userContext.UserActionSet.FirstOrDefault
                (ua => ua.User.ID == currUser.ID && ua.Action.ID == action.ID);

            return currAction;
        }

        public UserAction GetUserAction(EntitiesUsers userContext, UserRoles role, long userid)
        {
            Tools.AssertObjectContextExists(userContext);
            if (userid < 1)
            {
                throw new BusinessException("userid is < 1");
            }

            Actions action = GetActionForUser(userContext, role);
            UserAction currAction = userContext.UserActionSet.FirstOrDefault
                (ua => ua.User.ID == userid && ua.Action.ID == action.ID);

            return currAction;
        }

        /// <summary>
        /// Returns UserAction ehich is for role
        /// </summary>
        private Actions GetActionForUser(EntitiesUsers objectContext, UserRoles role)
        {
            Tools.AssertObjectContextExists(objectContext);

            Actions action = null;

            switch (role)
            {
                case (UserRoles.WriteCommentsAndMessages):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "commenter");
                    break;
                case (UserRoles.AddProducts):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "product");
                    break;
                case (UserRoles.AddCompanies):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "company");
                    break;
                case (UserRoles.RateProducts):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "prater");
                    break;
                case (UserRoles.RateComments):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "commrater");
                    break;
                case (UserRoles.RateUsers):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "userrater");
                    break;
                case (UserRoles.ReportInappropriate):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "flagger");
                    break;
                case (UserRoles.WriteSuggestions):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "suggestor");
                    break;
                case (UserRoles.HaveSignature):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "signature");
                    break;
                default:
                    throw new BusinessException(string.Format("user role = {0} is not supported role", role));
            }

            if (action == null)
            {
                throw new BusinessException(string.Format("theres no role for name = {0}", role));
            }

            return action;
        }

        /// <summary>
        /// Returns admin Action which is for role
        /// </summary>
        private Actions GetActionForAdmin(EntitiesUsers objectContext, AdminRoles role)
        {
            Tools.AssertObjectContextExists(objectContext);

            Actions action = null;

            switch (role)
            {
                case (AdminRoles.EditGlobalAdministrators):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "gCreator");
                    break;
                case (AdminRoles.EditAdministrators):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "aCreator");
                    break;
                case (AdminRoles.EditModerators):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "mCreator");
                    break;
                case (AdminRoles.EditCategories):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "category");
                    break;
                case (AdminRoles.EditCompanies):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "acompanies");
                    break;
                case (AdminRoles.EditProducts):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "aproducts");
                    break;
                case (AdminRoles.EditComments):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "acomments");
                    break;
                case (AdminRoles.EditUsers):
                    action = objectContext.ActionsSet.FirstOrDefault(act => act.name == "ueditor");
                    break;
                default:
                    throw new BusinessException(string.Format("Admin role = {0} is not supported role", role));
            }

            if (action == null)
            {
                throw new BusinessException(string.Format("Theres no role for name = {0}", role));
            }

            return action;
        }

        /// <summary>
        /// Makes visible false UserAction
        /// </summary>
        public void RemoveUserAction(EntitiesUsers userContext, Entities objectContext, UserAction uaction
            , User approver, BusinessLog bLog, string description)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (uaction == null)
            {
                throw new BusinessException("uaction is null");
            }

            if (approver == null)
            {
                throw new BusinessException("approver is null");
            }

            if (uaction.visible == true)
            {
                uaction.visible = false;
                uaction.ApprovedBy = approver;

                if (!uaction.ActionReference.IsLoaded)
                {
                    uaction.ActionReference.Load();
                }
                if (!uaction.UserReference.IsLoaded)
                {
                    uaction.UserReference.Load();
                }

                if (uaction.Action == GetActionForUser(userContext, UserRoles.HaveSignature))
                {

                    uaction.User.userData = null;
                }

                Tools.Save(userContext);

                bLog.LogRole(objectContext, userContext, uaction, LogType.delete, approver);

                BusinessUser businessUser = new BusinessUser();
                if (businessUser.IsFromUserTeam(uaction.User))
                {
                    string message = string.Empty;

                    switch (uaction.Action.name)
                    {
                        case "commenter":
                            message = Tools.GetResource("RemoveUserActionCommenter");
                            break;
                        case "product":
                            message = Tools.GetResource("RemoveUserActionAddProducts");
                            break;
                        case "company":
                            message = Tools.GetResource("RemoveUserActionAddMakers");
                            break;
                        case "prater":
                            message = Tools.GetResource("RemoveUserActionRateProducts");
                            break;
                        case "commrater":
                            message = Tools.GetResource("RemoveUserActionRateComments");
                            break;
                        case "userrater":
                            message = Tools.GetResource("RemoveUserActionRateUsers");
                            break;
                        case "flagger":
                            message = Tools.GetResource("RemoveUserActionSendReports");
                            break;
                        case "suggestor":
                            message = Tools.GetResource("RemoveUserActionSendSuggestions");
                            break;
                        case "signature":
                            message = Tools.GetResource("RemoveUserActionHaveSignature");
                            break;
                        default:
                            throw new BusinessException(string.Format("Action name : {0} is not supported action for which to send System message."
                                , uaction.Action.name));
                    }

                    if (!string.IsNullOrEmpty(description))
                    {
                        message = string.Format("{0}<br />{1} {2}", message, Tools.GetResource("Reason"), description);
                    }

                    BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                    bSystemMessages.Add(userContext, uaction.User, message);
                }
            }
        }


        /// <summary>
        /// Makes visible=false User role
        /// </summary>
        private void RemoveUserRole(EntitiesUsers userContext, Entities objectContext, long userID, long approvedBy, String rolename, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }

            if (approvedBy < 1)
            {
                throw new BusinessException("approvedBy is < 1");
            }

            if (rolename == null || rolename == string.Empty)
            {
                throw new BusinessException("rolename is null or empty");
            }

            IEnumerable<UserAction> userActions;
            userActions = userContext.UserActionSet.Where(usr => usr.User.ID == userID && usr.Action.name == rolename);
            if (userActions.Count<UserAction>() > 0)
            {
                if (userActions.Count<UserAction>() == 1)
                {
                    BusinessUser businessUser = new BusinessUser();

                    UserAction userAction = userActions.First<UserAction>();
                    User approver = businessUser.Get(userContext, approvedBy, true);

                    if (userAction != null)
                    {
                        if (userAction.visible)
                        {
                            userAction.visible = false;
                            userAction.ApprovedBy = approver;

                            Tools.Save(userContext);
                            bLog.LogRole(objectContext, userContext, userAction, LogType.delete, approver);
                        }
                        else
                        {
                            // if the role is already visible=false should it throw exception
                            throw new BusinessException(string.Format("The action id = {0} for user id = {1} is already visible=false", userAction.ID, userID));
                        }
                    }
                    else
                    {
                        throw new BusinessException("userAction is null");
                    }
                }
                else if (userActions.Count<UserAction>() > 1)
                {
                    throw new BusinessException(string.Format("The user id = {0} have the role ({1}) that you want to remove more than once", rolename, userID));
                }
            }
        }



        /// <summary>
        /// Returns visible UserAction, id there`s no such throws exception
        /// </summary>
        public UserAction GetUserAction(EntitiesUsers userContext, long userID, long actionID)
        {
            Tools.AssertObjectContextExists(userContext);
            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }
            if (actionID < 1)
            {
                throw new BusinessException("actionID is < 1");
            }

            UserAction userAction = userContext.UserActionSet.FirstOrDefault
                (ua => ua.User.ID == userID && ua.Action.ID == actionID && ua.visible == true);
            if (userAction == null)
            {
                throw new BusinessException(string.Format("Theres no user action with userID = {0} and actionID = {1}", userID, actionID));
            }

            return userAction;

        }

        /// <summary>
        /// Returns visible UserAction, id there`s no such returns null
        /// </summary>
        public UserAction GetUserActionWV(EntitiesUsers userContext, long userID, long actionID)
        {
            Tools.AssertObjectContextExists(userContext);
            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }
            if (actionID < 1)
            {
                throw new BusinessException("actionID is < 1");
            }

            UserAction userAction = userContext.UserActionSet.FirstOrDefault
                (ua => ua.User.ID == userID && ua.Action.ID == actionID && ua.visible == true);

            return userAction;

        }

        /// <summary>
        /// Returns UserActions for user with id
        /// </summary>
        public IEnumerable<UserAction> GetUserActions(EntitiesUsers userContext, long userID, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(userContext);
            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }

            BusinessUser businessUser = new BusinessUser();

            User user = businessUser.GetWithoutVisible(userContext, userID, true);

            IEnumerable<UserAction> userActions = null;

            if (onlyVisible == true)
            {
                userActions = userContext.UserActionSet.Where(ua => ua.User.ID == user.ID && ua.visible == true);
            }
            else
            {
                userActions = userContext.UserActionSet.Where(ua => ua.User.ID == user.ID);
            }

            return userActions;
        }

        /// <summary>
        /// Returns Actions that User dont have
        /// </summary>
        public List<Actions> GetRolesThatUserDontHave(EntitiesUsers userContext, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            BusinessUser businessUser = new BusinessUser();

            List<Actions> Actions = new List<Actions>();

            switch (currUser.type)
            {
                case ("writer"):
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.WriteCommentsAndMessages))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.WriteCommentsAndMessages));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.RateProducts))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.RateProducts));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.RateComments))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.RateComments));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.RateUsers))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.RateUsers));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.ReportInappropriate))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.ReportInappropriate));
                    }
                    break;
                case ("user"):
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.WriteCommentsAndMessages))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.WriteCommentsAndMessages));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.RateProducts))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.RateProducts));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.RateComments))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.RateComments));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.RateUsers))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.RateUsers));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.ReportInappropriate))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.ReportInappropriate));
                    }

                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.AddProducts))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.AddProducts));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.AddCompanies))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.AddCompanies));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.WriteSuggestions))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.WriteSuggestions));
                    }
                    if (!businessUser.CanUserDo(userContext, currUser, UserRoles.HaveSignature))
                    {
                        Actions.Add(GetActionForUser(userContext, UserRoles.HaveSignature));
                    }
                    break;
                case ("moderator"):
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditComments))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditComments));
                    }
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditUsers))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditUsers));
                    }
                    break;
                case ("administrator"):
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditComments))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditComments));
                    }
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditUsers))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditUsers));
                    }
                    ///

                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditModerators))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditModerators));
                    }
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditCompanies))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditCompanies));
                    }
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditProducts))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditProducts));
                    }
                    break;
                case ("global"):
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditComments))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditComments));
                    }
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditUsers))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditUsers));
                    }
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditModerators))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditModerators));
                    }
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditCompanies))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditCompanies));
                    }
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditProducts))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditProducts));
                    }

                    ///
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditAdministrators))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditAdministrators));
                    }
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditGlobalAdministrators))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditGlobalAdministrators));
                    }
                    if (!businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditCategories))
                    {
                        Actions.Add(GetActionForAdmin(userContext, AdminRoles.EditCategories));
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("User type = {0} is not supported type", currUser.type));
            }

            return Actions;
        }

    }
}
