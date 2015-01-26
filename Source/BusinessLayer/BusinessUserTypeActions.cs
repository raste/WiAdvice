﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessUserTypeActions
    {

        /// <summary>
        /// Add`s Action for Company or Product to database
        /// </summary>
        /// <param name="type">company,product,aCompProdModificator</param>
        private void AddAction(Entities objectContext, string type, long typeID)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (type == null || type == string.Empty)
            {
                throw new BusinessException("the action type is either null or empty ");
            }

            if (typeID < 1)
            {
                throw new BusinessException("typeID is < 1");
            }

            switch (type)
            {
                case ("company"):
                    BusinessCompany businessCompany = new BusinessCompany();
                    Company testCompany = businessCompany.GetCompanyWV(objectContext, typeID);
                    if (testCompany == null)
                    {
                        throw new BusinessException(string.Format("Theres no Company with id = {0}", typeID));
                    }
                    break;
                case ("product"):
                    BusinessProduct businessProduct = new BusinessProduct();
                    Product testProduct = businessProduct.GetProductByID(objectContext, typeID);
                    if (testProduct == null)
                    {
                        throw new BusinessException(string.Format("Theres no Product with id = {0}", typeID));
                    }
                    break;
                case ("aCompProdModificator"):
                    BusinessCompany businessCompanyy = new BusinessCompany();
                    Company testCompanyy = businessCompanyy.GetCompanyWV(objectContext, typeID);
                    if (testCompanyy == null)
                    {
                        throw new BusinessException(string.Format("Theres no Company with id = {0} for which " +
                            "'aCompProdModificator' action needs to be added", typeID));
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("Action type = {0} is not supported type", type));
            }

            TypeAction checkAction = objectContext.TypeActionSet.FirstOrDefault(ca => ca.type == type && ca.typeID == typeID);
            if (checkAction != null)
            {
                String error = string.Format("there is already action involving type {0} ID : {1} , there shouldnt be second", type, typeID);
                throw new BusinessException(error);
            }

            TypeAction newAction = new TypeAction();
            newAction.dateCreated = DateTime.UtcNow;
            newAction.type = type;
            newAction.typeID = typeID;
            if (type == "company" || type == "product")
            {
                newAction.description = string.Format("role for modificating  {0} {1} data", type, typeID);
                newAction.name = string.Format("{0}{1}modificator", type, typeID);
            }
            else if (type == "aCompProdModificator")
            {
                newAction.description = string.Format("role for modificating all company {0} products.", typeID);
                newAction.name = string.Format("company{0}allProductsModificator", typeID);
            }

            objectContext.AddToTypeActionSet(newAction);
            Tools.Save(objectContext);
        }

        public void AddActionAllCompanyProductsModificator(Entities objectContext, long companyID)
        {
            Tools.AssertObjectContextExists(objectContext);
            AddAction(objectContext, "aCompProdModificator", companyID);
        }

        public void AddActionCompanyModificator(Entities objectContext, long companyID)
        {
            Tools.AssertObjectContextExists(objectContext);
            AddAction(objectContext, "company", companyID);
        }

        public void AddActionProductModificator(Entities objectContext, long productID)
        {
            Tools.AssertObjectContextExists(objectContext);
            AddAction(objectContext, "product", productID);
        }

        /// <summary>
        /// Add`s UserAction All Company`s Products Modificator to User
        /// </summary>
        public void AddUserActionAllCompanyProductsModificator(EntitiesUsers userContext, Entities objectContext, User currUser,
            Company company, User approvedBy, BusinessLog bLog, bool sendSystemMessage)
        {

            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (approvedBy == null)
            {
                throw new BusinessException("approvedBy is null");
            }

            if (company == null)
            {
                throw new BusinessException("company is null");
            }

            if (company.visible == false)
            {
                throw new BusinessException("company.visible is false");
            }

            if (currUser.type != "user")
            {
                throw new BusinessException(string.Format("{0} ID : {1} cannot receive role to edit all company id : {2} products, because he is not of type 'user', approver id : {1}"
                    , currUser.username, currUser.ID, company.ID, approvedBy.ID));
            }

            BusinessUserOptions userOptions = new BusinessUserOptions();
            if (!currUser.UserOptionsReference.IsLoaded)
            {
                currUser.UserOptionsReference.Load();
            }
            if (!userOptions.IsUserActivated(currUser.UserOptions))
            {
                throw new BusinessException(string.Format("User ID : {0} cannot receive role for type modification because he is not activates, User id : {1}"
                    , currUser.ID, approvedBy.ID));
            }

            TypeAction aProdModifier = objectContext.TypeActionSet.FirstOrDefault
                (action => action.type == "aCompProdModificator" && action.typeID == company.ID);

            if (aProdModifier == null)
            {
                AddActionAllCompanyProductsModificator(objectContext, company.ID);

                aProdModifier = objectContext.TypeActionSet.FirstOrDefault
                    (action => action.type == "aCompProdModificator" && action.typeID == company.ID);
                if (aProdModifier == null)
                {
                    throw new BusinessException(string.Format("there isnt action aCompProdModificators for company ID = {0} " +
                        " even after creating it.", company.ID));
                }
            }

            BusinessTransferAction bTransfer = new BusinessTransferAction();

            UsersTypeAction testAction = objectContext.UsersTypeActionSet.FirstOrDefault
                (ta => ta.User.ID == currUser.ID && ta.TypeAction.ID == aProdModifier.ID);

            if (testAction != null)
            {
                if (testAction.visible)
                {
                    throw new BusinessException(string.Format("The user id = {0} already have the action aCompProdModificator for company = {1}"
                        , currUser.ID, company.ID));
                }
                else
                {
                    testAction.visible = true;
                    Tools.Save(objectContext);

                    bLog.LogUserTypeAction(objectContext, testAction, LogType.undelete, currUser);

                    bTransfer.RemoveTransfersToUserAboutAction(objectContext, userContext, testAction, bLog);
                }
            }
            else
            {
                UsersTypeAction newUserAction = new UsersTypeAction();
                newUserAction.User = Tools.GetUserID(objectContext, currUser);
                newUserAction.TypeAction = aProdModifier;
                newUserAction.dateCreated = DateTime.UtcNow;
                newUserAction.ApprovedBy = Tools.GetUserID(objectContext, approvedBy);
                newUserAction.visible = true;

                objectContext.AddToUsersTypeActionSet(newUserAction);
                Tools.Save(objectContext);

                bLog.LogUserTypeAction(objectContext, newUserAction, LogType.create, approvedBy);

                bTransfer.RemoveTransfersToUserAboutAction(objectContext, userContext, newUserAction, bLog);
            }

            if (sendSystemMessage)
            {
                string description = string.Format("{0} {1}.", Tools.GetResource("AddUserTypeActionAllMakerProducts")
                    , company.name);

                BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                bSystemMessages.Add(userContext, currUser, description);
            }
        }

        public void AddActionForCompanyToUserWhenThereAreNoEditors(EntitiesUsers userContext, Entities objectContext, User currUser,
            Company company, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (company == null)
            {
                throw new BusinessException("company is null");
            }

            if (currUser.type != "user")
            {
                throw new BusinessException(string.Format("{0} ID : {1} cannot receive role to edit company id : {2} , because he is not of type 'user'."
                    , currUser.username, currUser.ID, company.ID));
            }

            if (company.canUserTakeRoleIfNoEditors == false)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot take action for company id : {1} when there are no editors, because canUserTakeRoleIfNoEditors is false"
                    , currUser.ID, company.ID));
            }

            List<UsersTypeAction> currentModificators = GetCompanyModificators(objectContext, company.ID).ToList();
            if (currentModificators.Count > 0)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot take action for company id : {1} when there are no editors, because there are other editors."));
            }

            AddUserActionCompanyModificator(userContext, objectContext, currUser, company, currUser, bLog, false);
        }

        public void AddActionForProductToUserWhenThereAreNoEditors(EntitiesUsers userContext, Entities objectContext, User currUser,
            Product product, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            if (currUser.type != "user")
            {
                throw new BusinessException(string.Format("{0} ID : {1} cannot receive role to edit product id : {2} , because he is not of type 'user'."
                    , currUser.username, currUser.ID, product.ID));
            }

            if (product.canUserTakeRoleIfNoEditors == false)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot take action for product id : {1} when there are no editors, because canUserTakeRoleIfNoEditors is false"
                    , currUser.ID, product.ID));
            }

            List<UsersTypeAction> currentModificators = GetProductModificators(objectContext, product.ID).ToList();
            if (currentModificators.Count > 0)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot take action for product id : {1} when there are no editors, because there are other editors."));
            }

            if (!product.CompanyReference.IsLoaded)
            {
                product.CompanyReference.Load();
            }

            currentModificators = GetAllCompanyProductsModificators(objectContext, product.Company.ID).ToList();
            if (currentModificators.Count > 0)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot take action for product id : {1} when there are no editors, because there are other editors."));
            }

            AddUserActionProductModificator(userContext, objectContext, currUser, product, currUser, bLog, false);
        }

        /// <summary>
        /// Adds actions for new company and UserAction Company Modificator for user added the Company
        /// </summary>
        /// <param name="currUser">user added the company</param>
        public void AddNewCompanyActions(EntitiesUsers userContext, Entities objectContext, User currUser, BusinessLog bLog, Company currCompany)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            AddActionCompanyModificator(objectContext, currCompany.ID);
            AddActionAllCompanyProductsModificator(objectContext, currCompany.ID);

            BusinessUser businessUser = new BusinessUser();
            if (businessUser.IsUser(currUser))
            {
                AddUserActionCompanyModificator(userContext, objectContext, currUser, currCompany, Tools.GetSystem(), bLog, false);
            }

        }


        /// <summary>
        /// Adds actions for new Product and UserAction Product Modificator for user who added the product
        /// </summary>
        /// <param name="currUser">user added the product</param>
        public void AddNewProductActions(EntitiesUsers userContext, Entities objectContext, User currUser, BusinessLog bLog, Product currProduct)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            AddActionProductModificator(objectContext, currProduct.ID);

            BusinessUser businessUser = new BusinessUser();
            if (businessUser.IsUser(currUser))
            {
                AddUserActionProductModificator(userContext, objectContext, currUser, currProduct, Tools.GetSystem(), bLog, false);
            }
        }

        /// <summary>
        /// Add`s user action Company Modificator
        /// </summary>
        public void AddUserActionCompanyModificator(EntitiesUsers userContext, Entities objectContext
            , User currUser, Company company, User approver, BusinessLog bLog, bool sendSystemMessage)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (approver == null)
            {
                throw new BusinessException("approver is null");
            }

            if (company == null)
            {
                throw new BusinessException("company is null");
            }

            if (company.visible == false)
            {
                throw new BusinessException("company is visible.false");
            }

            if (currUser.type != "user")
            {
                throw new BusinessException(string.Format("{0} ID : {1} cannot receive role for company id : {2}, because he is not of type 'user', approver id : {1}"
                    , currUser.username, currUser.ID, company.ID, approver.ID));
            }

            BusinessUserOptions userOptions = new BusinessUserOptions();
            if (!currUser.UserOptionsReference.IsLoaded)
            {
                currUser.UserOptionsReference.Load();
            }
            if (!userOptions.IsUserActivated(currUser.UserOptions))
            {
                throw new BusinessException(string.Format("User ID : {0} cannot receive role for type modification because he is not activated, User id : {1}"
                    , currUser.ID, approver.ID));
            }

            TypeAction modifier = objectContext.TypeActionSet.FirstOrDefault(action => action.type == "company" && action.typeID == company.ID);
            if (modifier == null)
            {
                throw new BusinessException(string.Format("There`s no action Company Modificator for company id = {0}", company.ID));
            }

            BusinessTransferAction bTransfer = new BusinessTransferAction();

            UsersTypeAction testAction = objectContext.UsersTypeActionSet.FirstOrDefault
                (ta => ta.User.ID == currUser.ID && ta.TypeAction.ID == modifier.ID);

            if (testAction != null)
            {
                if (testAction.visible)
                {
                    throw new BusinessException(string.Format("The user id = {0} already have the action Company Modificator for company id = {1}"
                        , currUser.ID, company.ID));
                }
                else
                {
                    testAction.visible = true;
                    Tools.Save(objectContext);

                    bLog.LogUserTypeAction(objectContext, testAction, LogType.undelete, approver);

                    bTransfer.RemoveTransfersToUserAboutAction(objectContext, userContext, testAction, bLog);
                }
            }
            else
            {
                UsersTypeAction newUserAction = new UsersTypeAction();
                newUserAction.User = Tools.GetUserID(objectContext, currUser);
                newUserAction.TypeAction = modifier;
                newUserAction.dateCreated = DateTime.UtcNow;
                newUserAction.ApprovedBy = Tools.GetUserID(objectContext, approver);
                newUserAction.visible = true;

                objectContext.AddToUsersTypeActionSet(newUserAction);
                Tools.Save(objectContext);

                bLog.LogUserTypeAction(objectContext, newUserAction, LogType.create, approver);

                bTransfer.RemoveTransfersToUserAboutAction(objectContext, userContext, newUserAction, bLog);
            }

            if (sendSystemMessage)
            {
                string description = string.Format("{0} {1}.", Tools.GetResource("AddUserTypeActionEditMaker")
                    , company.name);

                BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                bSystemMessages.Add(userContext, currUser, description);
            }
        }


        /// <summary>
        /// Add`s user action Product Modificator
        /// </summary>
        public void AddUserActionProductModificator(EntitiesUsers userContext, Entities objectContext
            , User currUser, Product product, User approver, BusinessLog bLog, bool sendSystemMessage)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);

            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (approver == null)
            {
                throw new BusinessException("approver is null");
            }

            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            if (product.visible == false)
            {
                throw new BusinessException("product.visible is false");
            }

            if (currUser.type != "user")
            {
                throw new BusinessException(string.Format("{0} ID : {1} cannot receive role for product id : {2}, because he is not of type 'user', approver id : {1}"
                    , currUser.username, currUser.ID, product.ID, approver.ID));
            }

            BusinessUserOptions userOptions = new BusinessUserOptions();
            if (!currUser.UserOptionsReference.IsLoaded)
            {
                currUser.UserOptionsReference.Load();
            }
            if (!userOptions.IsUserActivated(currUser.UserOptions))
            {
                throw new BusinessException(string.Format("User ID : {0} cannot receive role for type modification because he is not activates, User id : {1}"
                    , currUser.ID, approver.ID));
            }

            TypeAction modifier = objectContext.TypeActionSet.FirstOrDefault(action => action.type == "product" && action.typeID == product.ID);
            if (modifier == null)
            {
                throw new BusinessException(string.Format("There isnt Action Product Modifier for Product ID = {0}", product.ID));
            }

            BusinessTransferAction bTransfer = new BusinessTransferAction();

            UsersTypeAction testAction = objectContext.UsersTypeActionSet.FirstOrDefault
                (ta => ta.User.ID == currUser.ID && ta.TypeAction.ID == modifier.ID);

            if (testAction != null)
            {
                if (testAction.visible)
                {
                    throw new BusinessException(string.Format("The user id = {0} already have  the action Product Modifier for product id = {1}", currUser.ID, product.ID));
                }
                else
                {
                    testAction.visible = true;
                    Tools.Save(objectContext);

                    bLog.LogUserTypeAction(objectContext, testAction, LogType.undelete, approver);

                    bTransfer.RemoveTransfersToUserAboutAction(objectContext, userContext, testAction, bLog);
                }
            }
            else
            {
                UsersTypeAction newUserAction = new UsersTypeAction();
                newUserAction.User = Tools.GetUserID(objectContext, currUser);
                newUserAction.TypeAction = modifier;
                newUserAction.dateCreated = DateTime.UtcNow;
                newUserAction.ApprovedBy = Tools.GetUserID(objectContext, approver);
                newUserAction.visible = true;

                objectContext.AddToUsersTypeActionSet(newUserAction);
                Tools.Save(objectContext);

                bLog.LogUserTypeAction(objectContext, newUserAction, LogType.create, approver);

                bTransfer.RemoveTransfersToUserAboutAction(objectContext, userContext, newUserAction, bLog);
            }

            if (sendSystemMessage)
            {
                string description = string.Format("{0} {1}.", Tools.GetResource("AddUserTypeActionEditProduct")
                    , product.name);

                BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                bSystemMessages.Add(userContext, currUser, description);
            }
        }

        /// <summary>
        /// Un-deletes user type action which is visible false, if there is no such throws exception. 
        /// Also checks if the type (connection) is OK (Visible.true ..etc), if it's not ok, the action is not undeleted
        /// </summary>
        /// <returns>True if the action is undeleted, otherwise false</returns>
        public bool UnDeleteUserTypeAction(Entities objectContext, EntitiesUsers userContext
            , User userToGetRole, User approverdBy, TypeAction currTypeAction, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);
            if (userToGetRole == null)
            {
                throw new BusinessException("userToGetRole is null");
            }

            if (approverdBy == null)
            {
                throw new BusinessException("approvedBy is nill");
            }

            if (currTypeAction == null)
            {
                throw new BusinessException("currTypeAction is null");
            }

            UsersTypeAction currAction = objectContext.UsersTypeActionSet.FirstOrDefault(action => action.User.ID == userToGetRole.ID
                && action.TypeAction.ID == currTypeAction.ID && action.visible == false);

            if (currAction == null)
            {
                throw new BusinessException(string.Format("User ID : {0} don`t have removed type action id : {1}, User approving : {2}",
                   userToGetRole.ID, currTypeAction.ID, approverdBy.ID));
            }

            bool undeleteAction = true;

            string description = string.Empty;
            switch (currTypeAction.type)
            {
                case "product":
                    BusinessProduct businessProduct = new BusinessProduct();
                    Product currProduct = businessProduct.GetProductByIDWV(objectContext, currTypeAction.typeID);
                    if (currProduct == null)
                    {
                        throw new BusinessException(string.Format("There is no product with ID : {0}", currTypeAction.typeID));
                    }
                    //
                    if (businessProduct.CheckIfProductsIsValidWithConnections(objectContext, currProduct, out description) == true)
                    {
                        description = string.Format("{0} {1}.", Tools.GetResource("AddUserTypeActionEditProduct"), currProduct.name);
                    }
                    else
                    {
                        undeleteAction = false;
                    }
                    break;
                case "company":
                    BusinessCompany businessCompany = new BusinessCompany();
                    Company currCompany = businessCompany.GetCompanyWV(objectContext, currTypeAction.typeID);
                    if (currCompany == null)
                    {
                        throw new BusinessException(string.Format("There is no company with ID : {0}", currTypeAction.typeID));
                    }
                    //
                    if (currCompany.visible == true)
                    {
                        description = string.Format("{0} {1}.", Tools.GetResource("AddUserTypeActionEditMaker"), currCompany.name);
                    }
                    else
                    {
                        undeleteAction = false;
                    }
                    break;
                case "aCompProdModificator":
                    BusinessCompany businessCompany1 = new BusinessCompany();
                    Company currCompany1 = businessCompany1.GetCompanyWV(objectContext, currTypeAction.typeID);
                    if (currCompany1 == null)
                    {
                        throw new BusinessException(string.Format("There is no company with ID : {0}", currTypeAction.typeID));
                    }
                    //
                    if (currCompany1.visible == true)
                    {
                        description = string.Format("{0} {1}.", Tools.GetResource("AddUserTypeActionAllMakerProducts"), currCompany1.name);
                    }
                    else
                    {
                        undeleteAction = false;
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("TypeAction with name : {0} is not supported for System Messages", currTypeAction.name));
            }

            if (undeleteAction == true)
            {
                currAction.visible = true;
                Tools.Save(objectContext);

                bLog.LogUserTypeAction(objectContext, currAction, LogType.undelete, approverdBy);

                BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                bSystemMessages.Add(userContext, userToGetRole, description);

                BusinessTransferAction bTransfer = new BusinessTransferAction();
                bTransfer.RemoveTransfersToUserAboutAction(objectContext, userContext, currAction, bLog);
            }

            return undeleteAction;
        }

        /// <summary>
        /// Makes visible=false All Company`s Products Modificator user action for User
        /// </summary>
        public void RemoveUserActionAllCompanyProductsModificator(Entities objectContext, EntitiesUsers userContext
            , long userID, User approvedBy, Company company, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }
            if (approvedBy == null)
            {
                throw new BusinessException("approvedBy is null");
            }
            if (company == null)
            {
                throw new BusinessException("companyID is null");
            }

            UsersTypeAction userAction = objectContext.UsersTypeActionSet.FirstOrDefault
                (ua => ua.User.ID == userID && ua.TypeAction.type == "aCompProdModificator" && ua.TypeAction.typeID == company.ID);

            if (userAction == null)
            {
                throw new BusinessException(string.Format("User id = {0} dont have action aCompProdModificator for company id = {1}"
                    , userID, company.ID));
            }

            if (userAction.visible == true)
            {
                userAction.visible = false;
                userAction.ApprovedBy = Tools.GetUserID(objectContext, approvedBy.ID, true);

                Tools.Save(objectContext);

                bLog.LogUserTypeAction(objectContext, userAction, LogType.delete, approvedBy);

                BusinessUser businessUser = new BusinessUser();
                if (!userAction.UserReference.IsLoaded)
                {
                    userAction.UserReference.Load();
                }
                User actionUser = businessUser.GetWithoutVisible(userContext, userAction.User.ID, true);
                string description = string.Format("{0} {1} {2}", Tools.GetResource("RemoveUserTypeActionAllMakerProducts")
                    , company.name, Tools.GetResource("RemoveUserTypeActionAllMakerProducts2"));

                BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                bSystemMessages.Add(userContext, actionUser, description);

                BusinessTransferAction bTransfer = new BusinessTransferAction();
                bTransfer.RemoveUserTransfersForAction(objectContext, userContext, userAction, bLog);

                BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();
                btSuggestions.DeclineAllSuggestionToTypeAfteRoleIsRemoved(userContext, objectContext, userAction, bLog);
            }
        }


        /// <summary>
        /// Makes visible=false user action Product Modificator for User
        /// </summary>
        public void RemoveUserActionProductModificator(Entities objectContext, EntitiesUsers userContext
            , long userID, User approvedBy, Product product, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }
            if (approvedBy == null)
            {
                throw new BusinessException("approvedBy is null");
            }
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            UsersTypeAction userAction = objectContext.UsersTypeActionSet.FirstOrDefault
                (ua => ua.User.ID == userID && ua.TypeAction.type == "product" && ua.TypeAction.typeID == product.ID);

            if (userAction == null)
            {
                throw new BusinessException(string.Format("User id = {0} dont have action ProductModificator for product = {1}", userID, product.ID));
            }

            if (userAction.visible == true)
            {
                userAction.visible = false;
                userAction.ApprovedBy = Tools.GetUserID(objectContext, approvedBy.ID, true);

                Tools.Save(objectContext);

                bLog.LogUserTypeAction(objectContext, userAction, LogType.delete, approvedBy);

                BusinessUser businessUser = new BusinessUser();
                if (!userAction.UserReference.IsLoaded)
                {
                    userAction.UserReference.Load();
                }
                User actionUser = businessUser.GetWithoutVisible(userContext, userAction.User.ID, true);
                string description = string.Format("{0} {1} {2}", Tools.GetResource("RemoveUserTypeActionEditProduct")
                    , product.name, Tools.GetResource("RemoveUserTypeActionEditProduct2"));

                BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                bSystemMessages.Add(userContext, actionUser, description);

                BusinessTransferAction bTransfer = new BusinessTransferAction();
                bTransfer.RemoveUserTransfersForAction(objectContext, userContext, userAction, bLog);

                BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();
                btSuggestions.DeclineAllSuggestionToTypeAfteRoleIsRemoved(userContext, objectContext, userAction, bLog);
            }
        }



        /// <summary>
        /// Makes visible=false CompanyModificator UserAction for User
        /// </summary>
        public void RemoveUserActionCompanyModificator(Entities objectContext, EntitiesUsers userContext
            , long userID, User approvedBy, Company company, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }
            if (approvedBy == null)
            {
                throw new BusinessException("approvedBy is null");
            }
            if (company == null)
            {
                throw new BusinessException("company is null");
            }

            UsersTypeAction userAction = objectContext.UsersTypeActionSet.FirstOrDefault
                (ua => ua.User.ID == userID && ua.TypeAction.type == "company" && ua.TypeAction.typeID == company.ID);

            if (userAction == null)
            {
                throw new BusinessException(string.Format("The user id = {0} dont have action Company Modificator for company = {1}"
                    , userID, company.ID));
            }

            if (userAction.visible == true)
            {
                userAction.visible = false;
                userAction.ApprovedBy = Tools.GetUserID(objectContext, approvedBy.ID, true);

                Tools.Save(objectContext);

                bLog.LogUserTypeAction(objectContext, userAction, LogType.delete, approvedBy);

                BusinessUser businessUser = new BusinessUser();
                if (!userAction.UserReference.IsLoaded)
                {
                    userAction.UserReference.Load();
                }
                User actionUser = businessUser.GetWithoutVisible(userContext, userAction.User.ID, true);
                string description = string.Format("{0} {1} {2}", Tools.GetResource("RemoveUserTypeActionEditMaker")
                    , company.name, Tools.GetResource("RemoveUserTypeActionEditMaker2"));

                BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
                bSystemMessages.Add(userContext, actionUser, description);

                BusinessTransferAction bTransfer = new BusinessTransferAction();
                bTransfer.RemoveUserTransfersForAction(objectContext, userContext, userAction, bLog);

                BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();
                btSuggestions.DeclineAllSuggestionToTypeAfteRoleIsRemoved(userContext, objectContext, userAction, bLog);
            }


        }

        /// <summary>
        /// returns visible true user type action
        /// </summary>
        public UsersTypeAction GetUserTypeAction(Entities objectContext, long userID, long actionID, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (userID < 1)
            {
                throw new BusinessException("userID is < 1");
            }
            if (actionID < 1)
            {
                throw new BusinessException("actionID is < 1");
            }

            UsersTypeAction userAction = objectContext.UsersTypeActionSet.FirstOrDefault
                (ua => ua.User.ID == userID && ua.TypeAction.ID == actionID && ua.visible == true);

            if (throwExcIfNull == true && userAction == null)
            {
                throw new BusinessException(string.Format("Theres no visible true user type action with userID = {0} and actionID = {1}", userID, actionID));
            }

            return userAction;

        }

        /// <summary>
        /// Returns true if user have visible TypeAction, otherwise false
        /// </summary>
        public bool CheckIfUserHaveAction(Entities objectContext, User user, TypeAction action)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }
            if (action == null)
            {
                throw new BusinessException("action is null");
            }

            bool result = false;

            UsersTypeAction userAction = objectContext.UsersTypeActionSet.FirstOrDefault
                (ua => ua.User.ID == user.ID && ua.TypeAction.ID == action.ID && ua.visible == true);
            if (userAction != null)
            {
                result = true;
            }

            return result;
        }


        /// <summary>
        /// Returns true if user have action for type
        /// </summary>
        public bool CheckIfUserHaveActionForType(Entities objectContext, User user, string type, long typeId)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new BusinessException("type is empty");
            }

            bool result = false;

            UsersTypeAction userAction = objectContext.UsersTypeActionSet.FirstOrDefault
                (ua => ua.User.ID == user.ID && ua.TypeAction.type == type && ua.TypeAction.typeID == typeId && ua.visible == true);
            if (userAction != null)
            {
                result = true;
            }

            return result;
        }


        public TypeAction GetTypeAction(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            TypeAction action = objectContext.TypeActionSet.FirstOrDefault<TypeAction>
                (act => act.ID == id);
            return action;
        }

        /// <summary>
        /// Returns UserTypeAction if found, otherwise null, doesn`t check for visibility
        /// </summary>
        public UsersTypeAction GetUserTypeAction(Entities objectContext, string type, long typeID, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new BusinessException("type is empty");
            }
            if (typeID < 1)
            {
                throw new BusinessException("typeID < 1");
            }

            TypeAction action = objectContext.TypeActionSet.FirstOrDefault(act => act.type == type && act.typeID == typeID);
            UsersTypeAction currAction = null;

            if (action != null)
            {
                currAction = objectContext.UsersTypeActionSet.FirstOrDefault
                    (ua => ua.User.ID == currUser.ID && ua.TypeAction.ID == action.ID);
            }

            return currAction;
        }

        /// <summary>
        /// Returns all Product Modificator UserActions
        /// </summary>
        public IEnumerable<UsersTypeAction> GetProductModificators(Entities objectContext, long productID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (productID < 1)
            {
                throw new BusinessException("invalid product !");
            }

            IEnumerable<UsersTypeAction> userActions = objectContext.UsersTypeActionSet.Where
                (ua => ua.TypeAction.type == "product" && ua.TypeAction.typeID == productID && ua.visible == true);

            return userActions;

        }

        /// <summary>
        /// Returns Product Modificator and Company Moficator roles That User have 
        /// </summary>
        public List<TypeAction> GetUserModificatorRoles(Entities objectContext, User currUser, bool checkForProductsWithInvConnections)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            List<TypeAction> ModificatorRoles = new List<TypeAction>();
            IEnumerable<UsersTypeAction> userActions = GetUserTypeActions(objectContext, currUser, true);
            if (userActions.Count<UsersTypeAction>() > 0)
            {
                BusinessProduct bProduct = new BusinessProduct();
                Product product = null;
                string error = string.Empty;

                foreach (UsersTypeAction action in userActions)
                {
                    if (!action.TypeActionReference.IsLoaded)
                    {
                        action.TypeActionReference.Load();
                    }
                    if (action.TypeAction.type == "product" || action.TypeAction.type == "company" || action.TypeAction.type == "aCompProdModificator")
                    {
                        if (checkForProductsWithInvConnections == false)
                        {
                            ModificatorRoles.Add(action.TypeAction);
                        }
                        else
                        {
                            if (checkForProductsWithInvConnections == true && action.TypeAction.type == "product")
                            {
                                product = bProduct.GetProductByIDWV(objectContext, action.TypeAction.typeID);
                                if (product == null)
                                {
                                    throw new BusinessException(string.Format("There is no product ID = {0}, for which there are actions", action.TypeAction.typeID));
                                }

                                if (bProduct.CheckIfProductsIsValidWithConnections(objectContext, product, out error) == true)
                                {
                                    ModificatorRoles.Add(action.TypeAction);
                                }
                            }
                            else
                            {
                                ModificatorRoles.Add(action.TypeAction);
                            }
                        }
                    }
                }
            }
            return ModificatorRoles;
        }

        /// <summary>
        /// Returns all Company`s Products Modificator UserActions
        /// </summary>
        public IEnumerable<UsersTypeAction> GetAllCompanyProductsModificators(Entities objectContext, long companyID)
        {

            Tools.AssertObjectContextExists(objectContext);
            if (companyID < 1)
            {
                throw new BusinessException("invalid company !");
            }

            IEnumerable<UsersTypeAction> userActions = objectContext.UsersTypeActionSet.Where
                (ua => ua.TypeAction.type == "aCompProdModificator" && ua.TypeAction.typeID == companyID && ua.visible == true);

            return userActions;
        }


        /// <summary>
        /// Returns number of Modificator Roles for User
        /// </summary>
        public int CountUserModificatorRoles(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            IEnumerable<UsersTypeAction> userActions = GetUserTypeActions(objectContext, currUser, true);

            int count = 0;

            if (userActions.Count<UsersTypeAction>() > 0)
            {
                foreach (UsersTypeAction action in userActions)
                {
                    if (!action.TypeActionReference.IsLoaded)
                    {
                        action.TypeActionReference.Load();
                    }
                    if (action.TypeAction.type == "product" || action.TypeAction.type == "company" || action.TypeAction.type == "aCompProdModificator")
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Returns Company`s Modificator UserActions
        /// </summary>
        public IEnumerable<UsersTypeAction> GetCompanyModificators(Entities objectContext, long companyID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (companyID < 1)
            {
                throw new BusinessException("invalid company !");
            }

            IEnumerable<UsersTypeAction> userActions = objectContext.UsersTypeActionSet.Where
                (ua => ua.TypeAction.type == "company" && ua.TypeAction.typeID == companyID && ua.visible == true);

            return userActions;
        }

        /// <summary>
        /// Returns user type actions for user id (products, companies, allcompany products)
        /// </summary>
        public IEnumerable<UsersTypeAction> GetUserTypeActions(Entities objectContext, User user, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            IEnumerable<UsersTypeAction> userActions = null;

            if (onlyVisible == true)
            {
                userActions = objectContext.UsersTypeActionSet.Where(ua => ua.User.ID == user.ID && ua.visible == true);
            }
            else
            {
                userActions = objectContext.UsersTypeActionSet.Where(ua => ua.User.ID == user.ID);
            }

            return userActions;
        }

        public List<UsersTypeAction> GetUserTypeActions(Entities objectContext, TypeAction action, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (action == null)
            {
                throw new BusinessException("action is null");
            }

            IEnumerable<UsersTypeAction> userActions = null;

            if (onlyVisible == true)
            {
                userActions = objectContext.UsersTypeActionSet.Where(ua => ua.TypeAction.ID == action.ID && ua.visible == true);
            }
            else
            {
                userActions = objectContext.UsersTypeActionSet.Where(ua => ua.TypeAction.ID == action.ID);
            }

            return userActions.ToList();
        }

        public void RemoveUserTypeAction(Entities objectContext, EntitiesUsers userContext, UsersTypeAction uaction, User approver
            , BusinessLog bLog, bool sendSystemMessage)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
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
                uaction.ApprovedBy = Tools.GetUserID(objectContext, approver);

                Tools.Save(objectContext);

                bLog.LogUserTypeAction(objectContext, uaction, LogType.delete, approver);

                BusinessTransferAction bTransfer = new BusinessTransferAction();
                bTransfer.RemoveUserTransfersForAction(objectContext, userContext, uaction, bLog);

                BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();
                btSuggestions.DeclineAllSuggestionToTypeAfteRoleIsRemoved(userContext, objectContext, uaction, bLog);

                if (sendSystemMessage == true)
                {
                    if (!uaction.TypeActionReference.IsLoaded)
                    {
                        uaction.TypeActionReference.Load();
                    }

                    string description = string.Empty;

                    switch (uaction.TypeAction.type)
                    {
                        case "product":

                            BusinessProduct bProduct = new BusinessProduct();
                            Product product = bProduct.GetProductByIDWV(objectContext, uaction.TypeAction.typeID);

                            if (product == null)
                            {
                                throw new BusinessException(string.Format("There is no product id : {0}, for which there are TypeActions", uaction.TypeAction.typeID));
                            }

                            description = string.Format("{0} {1} {2}", Tools.GetResource("RemoveUserTypeActionEditProduct")
                             , product.name, Tools.GetResource("RemoveUserTypeActionEditProduct2"));

                            break;
                        case "company":

                            BusinessCompany bCompany = new BusinessCompany();
                            Company company = bCompany.GetCompanyWV(objectContext, uaction.TypeAction.typeID);

                            if (company == null)
                            {
                                throw new BusinessException(string.Format("There is no company id : {0}, for which there are TypeActions", uaction.TypeAction.typeID));
                            }

                            description = string.Format("{0} {1} {2}", Tools.GetResource("RemoveUserTypeActionEditMaker")
                               , company.name, Tools.GetResource("RemoveUserTypeActionEditMaker2"));

                            break;
                        case "aCompProdModificator":

                            BusinessCompany bCompany1 = new BusinessCompany();
                            Company company1 = bCompany1.GetCompanyWV(objectContext, uaction.TypeAction.typeID);

                            if (company1 == null)
                            {
                                throw new BusinessException(string.Format("There is no company id : {0}, for which there are TypeActions", uaction.TypeAction.typeID));
                            }

                            description = string.Format("{0} {1} {2}", Tools.GetResource("RemoveUserTypeActionAllMakerProducts")
                             , company1.name, Tools.GetResource("RemoveUserTypeActionAllMakerProducts2"));

                            break;
                        default:
                            throw new BusinessException(string.Format("TypeAction type = {0} is not supported.", uaction.TypeAction.type));
                    }

                    if (!uaction.UserReference.IsLoaded)
                    {
                        uaction.UserReference.Load();
                    }

                    BusinessUser bUser = new BusinessUser();
                    User user = bUser.GetWithoutVisible(userContext, uaction.User.ID, true);

                    BusinessSystemMessages bSystemMessage = new BusinessSystemMessages();
                    bSystemMessage.Add(userContext, user, description);

                }
            }
        }

        /// <summary>
        /// Returns removed user`s type actions (actions that he had and were removed)
        /// </summary>
        public List<TypeAction> GetDeletedUserTypeActions(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUSer is null");
            }

            List<UsersTypeAction> deletedActions = objectContext.UsersTypeActionSet.Where(ua => ua.User.ID == currUser.ID
                && ua.visible == false).ToList();

            List<TypeAction> deletedTypeActions = new List<TypeAction>();
            if (deletedActions.Count > 0)
            {
                foreach (UsersTypeAction typeAction in deletedActions)
                {
                    if (!typeAction.TypeActionReference.IsLoaded)
                    {
                        typeAction.TypeActionReference.Load();
                    }

                    deletedTypeActions.Add(typeAction.TypeAction);
                }

            }

            return deletedTypeActions;
        }

        /// <summary>
        /// Removes all user roles for product which is being deleted, also sends system messages to those which had roles for him
        /// </summary>
        public void RemoveAllTypeActionsForProductWhenDeleted(Entities objectContext, EntitiesUsers userContext, Product currProduct
            , User approver, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            if (currProduct.visible == true)
            {
                throw new BusinessException(string.Format("product ID : {0} is visibe:true, needs to be visible:false to delete all modificator roles, User ID : {1}"
                    , currProduct.ID, approver.ID));
            }

            if (approver == null)
            {
                throw new BusinessException("approver is null");
            }

            List<UsersTypeAction> modificators = GetProductModificators(objectContext, currProduct.ID).ToList();
            if (modificators.Count > 0)
            {
                BusinessUser bUser = new BusinessUser();
                BusinessSystemMessages bSystemMessage = new BusinessSystemMessages();
                string description = string.Format("{0} {1} {2}", Tools.GetResource("RemoveUserTypeActionEditProductCusDeleted")
                    , currProduct.name, Tools.GetResource("RemoveUserTypeActionEditProductCusDeleted2"));

                User actionUser = null;

                foreach (UsersTypeAction action in modificators)
                {
                    RemoveUserTypeAction(objectContext, userContext, action, approver, bLog, false);

                    if (!action.UserReference.IsLoaded)
                    {
                        action.UserReference.Load();
                    }
                    actionUser = bUser.GetWithoutVisible(userContext, action.User.ID, true);

                    bSystemMessage.Add(userContext, actionUser, description);
                }
            }

        }

        /// <summary>
        ///  Removes all user roles for company which is being deleted, also sends system messages to those which had roles for it
        /// </summary>
        public void RemoveAllTypeActionsForCompanyWhenDeleted(Entities objectContext, EntitiesUsers userContext, Company currCompany
           , User approver, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompany == null)
            {
                throw new BusinessException("currProduct is null");
            }

            if (currCompany.visible == true)
            {
                throw new BusinessException(string.Format("company ID : {0} is visibe:true, needs to be visible:false to delete all modificator roles, User ID : {1}"
                    , currCompany.ID, approver.ID));
            }

            if (approver == null)
            {
                throw new BusinessException("approver is null");
            }

            List<UsersTypeAction> compModificators = GetCompanyModificators(objectContext, currCompany.ID).ToList();
            List<UsersTypeAction> allCompProdModifs = GetAllCompanyProductsModificators(objectContext, currCompany.ID).ToList();

            if (compModificators.Count > 0 || allCompProdModifs.Count > 0)
            {
                BusinessUser bUser = new BusinessUser();
                BusinessSystemMessages bSystemMessage = new BusinessSystemMessages();

                string description = string.Empty;
                User actionUser = null;

                if (allCompProdModifs.Count > 0)
                {
                    description = string.Format("{0} {1} {2}", Tools.GetResource("RemoveUserTypeActionEditAllMakerProdsCusDeleted")
                        , currCompany.name, Tools.GetResource("RemoveUserTypeActionEditAllMakerProdsCusDeleted2"));

                    foreach (UsersTypeAction action in allCompProdModifs)
                    {
                        RemoveUserTypeAction(objectContext, userContext, action, approver, bLog, false);

                        if (!action.UserReference.IsLoaded)
                        {
                            action.UserReference.Load();
                        }
                        actionUser = bUser.GetWithoutVisible(userContext, action.User.ID, true);

                        bSystemMessage.Add(userContext, actionUser, description);
                    }
                }

                if (compModificators.Count > 0)
                {
                    description = string.Format("{0} {1} {2}", Tools.GetResource("RemoveUserTypeActionEditMakerCusDeleted")
                        , currCompany.name, Tools.GetResource("RemoveUserTypeActionEditMakerCusDeleted2"));

                    foreach (UsersTypeAction action in compModificators)
                    {
                        RemoveUserTypeAction(objectContext, userContext, action, approver, bLog, false);

                        if (!action.UserReference.IsLoaded)
                        {
                            action.UserReference.Load();
                        }
                        actionUser = bUser.GetWithoutVisible(userContext, action.User.ID, true);

                        bSystemMessage.Add(userContext, actionUser, description);
                    }
                }
            }

        }


        /// <summary>
        /// Returns true if user havent logged in for time after which his action roles can be taken, otherwise false.
        /// Checks the user type also.
        /// </summary>
        public bool CanTypeActionsBeTakenFromUser(User user)
        {
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            bool result = false;

            BusinessUser businessUser = new BusinessUser();
            if (businessUser.IsUser(user))
            {
                TimeSpan span = DateTime.UtcNow - user.lastLogIn;
                if (span.Days >= Configuration.GetUserActionTimeAfterWhichActionsCanBeTaken)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool CanUserTakeActionFromEditor(EntitiesUsers userContext, Entities objectContext, User user)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);

            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            bool result = false;

            BusinessUserOptions bUserOptions = new BusinessUserOptions();
            BusinessUser bUser = new BusinessUser();

            if (bUser.IsUser(user))
            {
                if (bUserOptions.CountOfUserWarnings(userContext, user) <= Configuration.GetUserActionMaxNumberWarnings)
                {
                    if (Configuration.GetUserActionMinNumberComments > 0)
                    {
                        BusinessComment bComment = new BusinessComment();
                        if (bComment.CountUserComments(objectContext, user) >= Configuration.GetUserActionMinNumberComments)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Used when user is taking action from another user, because the second haven`t logged for long time
        /// </summary>
        public void TakeActionFromUser(EntitiesUsers userContext, Entities objectContext, User userTaking
            , User userWithAction, UsersTypeAction typeAction, BusinessLog Blog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);

            Tools.AssertBusinessLogExists(Blog);

            if (userWithAction == null)
            {
                throw new BusinessException("userWithAction is null");
            }
            if (userTaking == null)
            {
                throw new BusinessException("userTaking is null");
            }
            if (typeAction == null)
            {
                throw new BusinessException("typeAction is null");
            }
            if (typeAction.visible == false)
            {
                throw new BusinessException("typeAction.visible is false");
            }
            if (userTaking == userWithAction)
            {
                throw new BusinessException("userTaking == userWithAction");
            }

            if (!CanTypeActionsBeTakenFromUser(userWithAction))
            {
                throw new BusinessException(string.Format("User id : {0} cannot take actions from USer id : {1}."
                    , userTaking.ID, userWithAction.ID));
            }

            if (!CanUserTakeActionFromEditor(userContext, objectContext, userTaking))
            {
                throw new BusinessException(string.Format("User ID : {0} cannot take actions from other users because he doesn`t cover the requirments."
                    , userTaking.ID));
            }

            if (!typeAction.UserReference.IsLoaded)
            {
                typeAction.UserReference.Load();
            }

            if (typeAction.User.ID != userWithAction.ID)
            {
                throw new BusinessException(String.Format("UserTypeAction ID : {0} is not for User ID : {1}."
                    , typeAction.ID, userWithAction.ID));
            }

            if (!typeAction.TypeActionReference.IsLoaded)
            {
                typeAction.TypeActionReference.Load();
            }

            if (CheckIfUserHaveAction(objectContext, userTaking, typeAction.TypeAction))
            {
                throw new BusinessException(string.Format("User ID : {0} cannot take UserTypeAction ID : {1} from User ID : {2}, because he already have that action."
                    , userTaking.ID, typeAction.ID, userWithAction.ID));
            }

            RemoveUserTypeAction(objectContext, userContext, typeAction, userTaking, Blog, false);

            TypeAction action = typeAction.TypeAction;

            string type = string.Empty;
            string typeName = string.Empty;

            // ADD NEW ACTION
            switch (action.type)
            {
                case "product":
                    BusinessProduct businessProduct = new BusinessProduct();
                    Product currProduct = businessProduct.GetProductByID(objectContext, action.typeID);
                    if (currProduct == null)
                    {
                        throw new BusinessException(string.Format("There is no visible product id : {0}", action.typeID));
                    }

                    type = Tools.GetResource("product");
                    typeName = currProduct.name;

                    AddUserActionProductModificator(userContext, objectContext, userTaking, currProduct, userTaking, Blog, false);
                    break;
                case "company":
                    BusinessCompany businessCompany = new BusinessCompany();
                    Company currCompany = businessCompany.GetCompany(objectContext, action.typeID);
                    if (currCompany == null)
                    {
                        throw new BusinessException(string.Format("There is no visible company id : {0}.", action.typeID));
                    }

                    type = Tools.GetResource("maker");
                    typeName = currCompany.name;

                    AddUserActionCompanyModificator(userContext, objectContext, userTaking, currCompany, userTaking, Blog, false);
                    break;
                default:
                    throw new BusinessException(string.Format("Action.name = {0} is not supported when taking actions from users."
                        , action.name));
            }

            // SEND SYSTEM MESSAGE TO USER FROM WHICH ACTION IS TAKEN
            string smDescription = string.Format("{0} '{1}' {2} {3} {4} {5} {6} {7} {8}."
                , Tools.GetResource("UserB"), userTaking.username, Tools.GetResource("UserTookActionFrom"), type, typeName
                , Tools.GetResource("UserTookActionFrom2"), Tools.GetResource("UserTookActionFrom3")
                , Configuration.GetUserActionTimeAfterWhichActionsCanBeTaken, Tools.GetResource("days"));

            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
            bSystemMessages.Add(userContext, userWithAction, smDescription);

            // LOG THAT user took action from another user
            Blog.LogGetActionFromUser(objectContext, action, userTaking, userWithAction);
        }

    }
}
