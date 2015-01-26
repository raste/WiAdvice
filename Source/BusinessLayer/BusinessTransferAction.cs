﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessTransferAction
    {
        public void Add(Entities objectContext, EntitiesUsers userContext, User userTransfering
            , User userReceiving, UsersTypeAction action, string description, BusinessLog bLog)
        {
            Tools.AssertBusinessLogExists(bLog);

            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (userTransfering == null)
            {
                throw new BusinessException("userTransfering is null");
            }
            if (userReceiving == null)
            {
                throw new BusinessException("userReceiving is null");
            }
            if (action == null)
            {
                throw new BusinessException("action is null");
            }

            string error = string.Empty;
            if (CanUserTransferActionTo(objectContext, userContext, userTransfering, userReceiving, action, out error) == false)
            {
                throw new BusinessException(string.Format("User ID : {0} cannot transfer UserTypeAction ID : {1} to user ID : {2}, error : {3}"
                    , userTransfering.ID, action.ID, userReceiving.ID, error));
            }

            TransferTypeAction newTransfer = new TransferTypeAction();

            newTransfer.UserReceiving = Tools.GetUserID(objectContext, userReceiving);
            newTransfer.UserTransfering = Tools.GetUserID(objectContext, userTransfering);
            newTransfer.UserTypeAction = action;
            newTransfer.dateCreated = DateTime.UtcNow;
            newTransfer.accepted = false;
            newTransfer.active = true;
            newTransfer.description = description;

            objectContext.AddToTransferTypeActionSet(newTransfer);
            Tools.Save(objectContext);

            // SEND SYSTEM MESSAGE
            string smDescription = GetSystemMessageDescription(objectContext, newTransfer, "add", userTransfering);
            if (!string.IsNullOrEmpty(description))
            {
                smDescription += string.Format("<br />{0} {1}", Tools.GetResource("Description"), description);
            }

            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
            bSystemMessages.Add(userContext, userReceiving, smDescription);

            // LOG
            bLog.LogActionTransfer(objectContext, userContext, newTransfer, LogType.create, userTransfering, string.Empty);
        }

        public void AcceptTransfer(Entities objectContext, EntitiesUsers userContext, User userAccepting
            , TransferTypeAction transfer, BusinessLog Blog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(Blog);

            if (userAccepting == null)
            {
                throw new BusinessException("userAccepting is null");
            }
            if (transfer == null)
            {
                throw new BusinessException("transfer is null");
            }
            if (transfer.active == false)
            {
                throw new BusinessException("transfer.active == false");
            }
            if (!transfer.UserReceivingReference.IsLoaded)
            {
                transfer.UserReceivingReference.Load();
            }
            if (!transfer.UserTransferingReference.IsLoaded)
            {
                transfer.UserTransferingReference.Load();
            }
            if (transfer.UserReceiving.ID != userAccepting.ID)
            {
                throw new BusinessException(string.Format("Transfer ID : {0}, was accepted by user {1}, but the transfer is for User ID : {2}"
                    , transfer.ID, userAccepting.ID, transfer.UserReceiving.ID));
            }

            User userTransfering = Tools.GetUserFromUserDatabase(userContext, transfer.UserTransfering);

            transfer.accepted = true;
            transfer.active = false;
            Tools.Save(objectContext);

            if (!transfer.UserTransferingReference.IsLoaded)
            {
                transfer.UserTypeActionReference.Load();
            }
            if (!transfer.UserTypeAction.TypeActionReference.IsLoaded)
            {
                transfer.UserTypeAction.TypeActionReference.Load();
            }

            TypeAction action = transfer.UserTypeAction.TypeAction;

            BusinessUserTypeActions buTypeActions = new BusinessUserTypeActions();
            buTypeActions.RemoveUserTypeAction(objectContext, userContext, transfer.UserTypeAction, userTransfering, Blog, false);

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

                    buTypeActions.AddUserActionProductModificator(userContext, objectContext, userAccepting, currProduct, userTransfering, Blog, false);
                    break;
                case "company":
                    BusinessCompany businessCompany = new BusinessCompany();
                    Company currCompany = businessCompany.GetCompany(objectContext, action.typeID);
                    if (currCompany == null)
                    {
                        throw new BusinessException(string.Format("There is no visible company id : {0}.", action.typeID));
                    }

                    buTypeActions.AddUserActionCompanyModificator(userContext, objectContext, userAccepting, currCompany, userTransfering, Blog, false);
                    break;
                default:
                    throw new BusinessException(string.Format("Action.name = {0} is not supported when accepting action transactions."
                        , action.name));
            }


            // SEND SYSTEM MESSAGE TO USER TRANSFERING
            string smDescription = GetSystemMessageDescription(objectContext, transfer, "accept", userAccepting);
            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
            bSystemMessages.Add(userContext, userTransfering, smDescription);

            // BLOG user accepted transaction
            Blog.LogActionTransfer(objectContext, userContext, transfer, LogType.delete, userAccepting, "accept");
        }

        public void DeclineTransfer(Entities objectContext, EntitiesUsers userContext, User userDeclining
           , TransferTypeAction transfer, BusinessLog Blog, bool userDeleted)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(Blog);

            if (userDeclining == null)
            {
                throw new BusinessException("userDeclining is null");
            }
            if (transfer == null)
            {
                throw new BusinessException("transfer is null");
            }
            if (transfer.active == false)
            {
                throw new BusinessException("transfer.active == false");
            }

            if (!transfer.UserReceivingReference.IsLoaded)
            {
                transfer.UserReceivingReference.Load();
            }
            if (!transfer.UserTransferingReference.IsLoaded)
            {
                transfer.UserTransferingReference.Load();
            }

            BusinessUser bUser = new BusinessUser();
            User userTransfering = bUser.Get(userContext, transfer.UserTransfering.ID, true);
            User userReceiving = bUser.Get(userContext, transfer.UserReceiving.ID, true);

            string smDescription = string.Empty;
            string declineType = "decline";
            if (userDeleted == true)
            {
                declineType = "deactUserDeleted";
            }

            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();


            if (userReceiving == userDeclining)
            {
                transfer.active = false;
                transfer.accepted = false;
                Tools.Save(objectContext);

                // SEND SYSTEM MESSAGE TO USER TRANSFERING
                smDescription = GetSystemMessageDescription(objectContext, transfer, declineType, userDeclining);
                bSystemMessages.Add(userContext, userTransfering, smDescription);
            }
            else if (userTransfering == userDeclining)
            {
                transfer.active = false;
                transfer.accepted = false;
                Tools.Save(objectContext);

                // SEND SYSTEM MESSAGE TO USER RECEICING
                smDescription = GetSystemMessageDescription(objectContext, transfer, declineType, userDeclining);
                bSystemMessages.Add(userContext, userReceiving, smDescription);
            }
            else
            {
                throw new BusinessException(string.Format("User ID : {0} cannot decline transfer ID : {1}  because he is not the one transfering or receiving."
                    , userDeclining.ID, transfer.ID));
            }

            // LOG
            Blog.LogActionTransfer(objectContext, userContext, transfer, LogType.delete, userDeclining, "decline");
        }

        private void DeactivateTransfer(Entities objectContext, EntitiesUsers userContext, TransferTypeAction transfer, BusinessLog Blog, bool expiredTime)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(Blog);

            if (transfer == null)
            {
                throw new BusinessException("transfer is null");
            }

            transfer.active = false;
            Tools.Save(objectContext);

            // SEND SYSTEM MESSAGE WITH REASON TO BOTH RECEIVER AND USER TRANSFERING
            if (!transfer.UserReceivingReference.IsLoaded)
            {
                transfer.UserReceivingReference.Load();
            }
            if (!transfer.UserTransferingReference.IsLoaded)
            {
                transfer.UserTransferingReference.Load();
            }

            BusinessUser bUser = new BusinessUser();
            User userTransfering = bUser.Get(userContext, transfer.UserTransfering.ID, true);
            User userReceiving = bUser.Get(userContext, transfer.UserReceiving.ID, true);

            string smDescription = string.Empty;

            if (expiredTime == false)
            {
                smDescription = GetSystemMessageDescription(objectContext, transfer, "deactivate", null);
            }
            else
            {
                smDescription = GetSystemMessageDescription(objectContext, transfer, "deactivateTime", null);
            }

            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
            bSystemMessages.Add(userContext, userTransfering, smDescription);
            bSystemMessages.Add(userContext, userReceiving, smDescription);

            // LOG
            Blog.LogActionTransfer(objectContext, userContext, transfer, LogType.delete, null, "removing");
        }

        /// <summary>
        /// Checks if user can transfer role to, true if can, false if cannot, and returns error. Throws exceptions on nulls and action.visible == false
        /// , userTransfering.visible/activated == false
        /// </summary>
        public bool CanUserTransferActionTo(Entities objectContext, EntitiesUsers userContext, User userTransfering
            , User userReceiving, UsersTypeAction action, out string error)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (userTransfering == null)
            {
                throw new BusinessException("userTransfering is null");
            }
            if (userReceiving == null)
            {
                throw new BusinessException("userReceiving is null");
            }
            if (action == null)
            {
                throw new BusinessException("action is null");
            }
            if (action.visible == false)
            {
                throw new BusinessException("action is visible:false");
            }

            error = string.Empty;
            bool result = false;

            if (CheckUsers(userContext, userTransfering, userReceiving, out error) == true)
            {
                if (!action.TypeActionReference.IsLoaded)
                {
                    action.TypeActionReference.Load();
                }

                if (IsUserTransferingAction(objectContext, userTransfering, action) == true)
                {
                    error = Tools.GetResource("errTransferActionAlreadyTransf");
                }
                else if (IsActionBeingTransferedTo(objectContext, userReceiving, action.TypeAction) == true)
                {
                    error = string.Format("{0} {1}.", Tools.GetResource("errTransferActionOtherIsTransf"), userReceiving.username);
                }
                else
                {
                    if (action.TypeAction.name != "aCompProdModificator")
                    {
                        BusinessUserTypeActions butAction = new BusinessUserTypeActions();
                        if (butAction.CheckIfUserHaveAction(objectContext, userReceiving, action.TypeAction) == true)
                        {
                            error = string.Format("{0} {1}", userReceiving.username, Tools.GetResource("errTransferActionUserAlreadyHave"));
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        error = Tools.GetResource("errTransferActionCantTransfer");
                    }
                }

            }

            return result;
        }

        private bool CheckUsers(EntitiesUsers userContext, User userTransfering, User userReceiving, out string error)
        {
            Tools.AssertObjectContextExists(userContext);

            if (userTransfering == null)
            {
                throw new BusinessException("userTransfering is null");
            }
            if (userReceiving == null)
            {
                throw new BusinessException("userReceiving is null");
            }
            if (!userTransfering.UserOptionsReference.IsLoaded)
            {
                userTransfering.UserOptionsReference.Load();
            }
            if (userTransfering.visible == false)
            {
                throw new BusinessException("userTransfering.visible == false");
            }
            if (userTransfering.UserOptions.activated == false)
            {
                throw new BusinessException("userTransfering.UserOptions.activated == false");
            }

            bool result = false;
            error = string.Empty;

            BusinessUser bUser = new BusinessUser();

            if (userReceiving == userTransfering)
            {
                error = Tools.GetResource("errTransferActionCantTransferToUrself");
            }
            else
            {
                BusinessMessages bMessages = new BusinessMessages();
                if (bMessages.IsUserBlocking(userContext, userReceiving.ID, userTransfering.ID) == true)
                {
                    error = string.Format("{0} {1}{2}", Tools.GetResource("errTransferActionCantTransferTo")
                        , userReceiving.username, Tools.GetResource("errTransferActionCantTransferTo2"));
                }
                else if (bMessages.IsUserBlocking(userContext, userTransfering.ID, userReceiving.ID) == true)
                {
                    error = string.Format("{0} {1}{2}", Tools.GetResource("errTransferActionCantTransferTo")
                        , userReceiving.username, Tools.GetResource("errTransferActionCantTransferTo3"));
                }
                else
                {
                    if (!bUser.IsFromUserTeam(userTransfering))
                    {
                        error = Tools.GetResource("errTransferActionCantTransferRoles");
                    }
                    else
                    {
                        if (userReceiving.visible == false)
                        {
                            error = Tools.GetResource("errNoSuchUser");
                        }
                        else
                        {
                            if (!bUser.IsUser(userReceiving))
                            {
                                error = string.Format("{0} {1}.", Tools.GetResource("errTransferActionCantTransferTo"), userReceiving.username);
                            }
                            else
                            {
                                if (!userReceiving.UserOptionsReference.IsLoaded)
                                {
                                    userReceiving.UserOptionsReference.Load();
                                }
                                if (userReceiving.UserOptions.activated == false)
                                {
                                    error = string.Format("{0} {1}", userReceiving.username, Tools.GetResource("errIsntActivated"));
                                }
                                else
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public bool IsUserTransferingAction(Entities objectContext, User user, UsersTypeAction action)
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

            TransferTypeAction ttAction = objectContext.TransferTypeActionSet.FirstOrDefault
                (tt => tt.UserTransfering.ID == user.ID && tt.UserTypeAction.ID == action.ID && tt.active == true);
            if (ttAction != null)
            {
                result = true;
            }

            return result;
        }

        public bool IsActionBeingTransferedTo(Entities objectContext, User user, TypeAction action)
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

            // if exception, get to user active transfers and check their action.typeaction.id

            bool result = false;

            TransferTypeAction ttAction = objectContext.TransferTypeActionSet.FirstOrDefault
                (tt => tt.UserReceiving.ID == user.ID && tt.UserTypeAction.TypeAction.ID == action.ID && tt.active == true);
            if (ttAction != null)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// type : add,decline,deactivate,accept,deactivateTime,deactUserDeleted. For deactivate and deactivateTime user is not used
        /// </summary>
        private string GetSystemMessageDescription(Entities objectContext, TransferTypeAction transfer, string type, User user)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (type != "deactivate" && type != "deactivateTime" && user == null)
            {
                throw new BusinessException("user is null");
            }

            if (!transfer.UserTypeActionReference.IsLoaded)
            {
                transfer.UserTypeActionReference.Load();
            }
            if (!transfer.UserTypeAction.TypeActionReference.IsLoaded)
            {
                transfer.UserTypeAction.TypeActionReference.Load();
            }

            TypeAction action = transfer.UserTypeAction.TypeAction;
            string actionType = string.Empty;
            string typeName = string.Empty;

            switch (action.type)
            {
                case "product":
                    BusinessProduct businessProduct = new BusinessProduct();
                    Product currProduct = businessProduct.GetProductByIDWV(objectContext, action.typeID);
                    if (currProduct == null)
                    {
                        throw new BusinessException(string.Format("There is no visible product id : {0}", action.typeID));
                    }

                    actionType = Tools.GetResource("product");
                    typeName = currProduct.name;
                    break;
                case "company":
                    BusinessCompany businessCompany = new BusinessCompany();
                    Company currCompany = businessCompany.GetCompanyWV(objectContext, action.typeID);
                    if (currCompany == null)
                    {
                        throw new BusinessException(string.Format("There is no visible company id : {0}.", action.typeID));
                    }

                    actionType = Tools.GetResource("maker");
                    typeName = currCompany.name;
                    break;
                default:
                    throw new BusinessException(string.Format("TypeAction name = {0} is not supported for system messages action transactions."
                        , action.name));
            }

            string description = string.Empty;

            switch (type)
            {
                case "add":
                    description = string.Format("{0} {1} {2} {3}. {4} {5} {6}", user.username
                        , Tools.GetResource("transferSmAdd"), actionType, typeName, Tools.GetResource("YouHave")
                        , Configuration.ActionTransactionNumDaysActive, Tools.GetResource("transferDaysToAcceptOrDecline"));
                    break;
                case "decline":
                    description = string.Format("{0} {1} {2} {3}.", user.username
                        , Tools.GetResource("transferSmDecline"), actionType, typeName);
                    break;
                case "accept":
                    description = string.Format("{0} {1} {2} {3}. {4} {2} {3}."
                        , user.username, Tools.GetResource("transferSmAccept"), actionType, typeName
                        , Tools.GetResource("transferSmAccept2"));
                    break;
                case "deactivate":
                    description = string.Format("{0} {1} {2} {3}", Tools.GetResource("transferSmDeactivate")
                        , actionType, typeName, Tools.GetResource("transferSmDeactivate2"));
                    break;
                case "deactUserDeleted":
                    description = string.Format("{0} {1} {2} {3} {4} {5}", Tools.GetResource("TransferSmDeactivateCusUserIsDeleted")
                        , actionType, typeName, Tools.GetResource("TransferSmDeactivateCusUserIsDeleted2")
                        , user.username, Tools.GetResource("TransferSmDeactivateCusUserIsDeleted3"));
                    break;
                case "deactivateTime":
                    description = string.Format("{0} {1} {2} {3} {4} {5}.", Tools.GetResource("transferSmDeactivateTime")
                        , actionType, typeName, Tools.GetResource("transferSmDeactivateTime2")
                        , Configuration.ActionTransactionNumDaysActive, Tools.GetResource("days"));
                    break;
                default:
                    throw new BusinessException(string.Format("System message type = {0} is not supported for action transactions", type));
            }

            return description;

        }

        /// <summary>
        /// Returns active transfers which user is iniciated 
        /// </summary>
        public List<TransferTypeAction> GetUserTransfers(Entities objectContext, User user)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            List<TransferTypeAction> transfers = objectContext.TransferTypeActionSet.Where
                (ttp => ttp.UserTransfering.ID == user.ID && ttp.active == true).ToList();

            return transfers;
        }

        /// <summary>
        /// Returns active transfers to user 
        /// </summary>
        public List<TransferTypeAction> GetTransfersToUser(Entities objectContext, User user)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            List<TransferTypeAction> transfers = objectContext.TransferTypeActionSet.Where
                (ttp => ttp.UserReceiving.ID == user.ID && ttp.active == true).ToList();

            return transfers;
        }

        public TransferTypeAction Get(Entities objectContext, long id, bool onlyActive, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id < 1");
            }

            TransferTypeAction transfer = null;
            if (onlyActive == true)
            {
                transfer = objectContext.TransferTypeActionSet.FirstOrDefault(tta => tta.ID == id && tta.active == true);
            }
            else
            {
                transfer = objectContext.TransferTypeActionSet.FirstOrDefault(tta => tta.ID == id);
            }

            if (throwExcIfNull == true && transfer == null)
            {
                string active = string.Empty;
                if (onlyActive == true)
                {
                    active = "active ";
                }
                throw new BusinessException(string.Format("No {0}transfer ID : {1}", active, id));
            }

            return transfer;
        }

        /// <summary>
        /// Used when user role is removed, to remove also all transfers for this role on that user 
        /// </summary>
        public void RemoveUserTransfersForAction(Entities objectContext, EntitiesUsers userContext, UsersTypeAction action, BusinessLog Blog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (action == null)
            {
                throw new BusinessException("action is null");
            }

            if (!action.UserReference.IsLoaded)
            {
                action.UserReference.Load();
            }
            User actionUser = Tools.GetUserFromUserDatabase(userContext, action.User.ID);

            List<TransferTypeAction> transfers = objectContext.TransferTypeActionSet.Where
                (tta => tta.UserTransfering.ID == actionUser.ID && tta.active == true && tta.UserTypeAction.ID == action.ID).ToList();

            if (transfers.Count > 0)
            {
                foreach (TransferTypeAction transfer in transfers)
                {
                    DeactivateTransfer(objectContext, userContext, transfer, Blog, false);
                }
            }

        }

        /// <summary>
        /// Declines all active action transfers from/to user which is being deleted. User have to be visible.false
        /// </summary>
        public void RemoveTransfersToOrByUserWhichIsBeingDeleted(Entities objectContext, EntitiesUsers userContext, BusinessLog Blog, User userDeleted)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(Blog);

            if (userDeleted == null)
            {
                throw new BusinessException("userDeleted is null");
            }

            if (userDeleted.visible == false)
            {
                throw new BusinessException(string.Format("Can`t remove all right transfers from/to user id : {0} , because he is visible.false"
                    , userDeleted.ID));
            }

            List<TransferTypeAction> transfers = objectContext.TransferTypeActionSet.Where
                (ttp => (ttp.UserReceiving.ID == userDeleted.ID || ttp.UserTransfering.ID == userDeleted.ID)
                    && ttp.active == true).ToList();

            if (transfers != null && transfers.Count > 0)
            {

                foreach (TransferTypeAction transfer in transfers)
                {
                    DeclineTransfer(objectContext, userContext, userDeleted, transfer, Blog, true);
                }
            }

        }

        /// <summary>
        /// Used when user receives role to remove every transaction to him for this role
        /// </summary>
        public void RemoveTransfersToUserAboutAction(Entities objectContext, EntitiesUsers userContext, UsersTypeAction userAction, BusinessLog Blog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(Blog);

            if (userAction == null)
            {
                throw new BusinessException("userAction is null");
            }

            if (!userAction.UserReference.IsLoaded)
            {
                userAction.UserReference.Load();
            }
            if (!userAction.TypeActionReference.IsLoaded)
            {
                userAction.TypeActionReference.Load();
            }

            User userReceiving = Tools.GetUserFromUserDatabase(userContext, userAction.User.ID);

            BusinessUserTypeActions buTypeActions = new BusinessUserTypeActions();
            List<UsersTypeAction> actions = buTypeActions.GetUserTypeActions(objectContext, userAction.TypeAction, false);

            if (actions.Count > 0)
            {
                List<TransferTypeAction> transfers = new List<TransferTypeAction>();

                foreach (UsersTypeAction action in actions)
                {
                    transfers = objectContext.TransferTypeActionSet.Where
                     (tta => tta.UserReceiving.ID == userReceiving.ID && tta.active == true && tta.UserTypeAction.ID == action.ID).ToList();

                    if (transfers.Count > 0)
                    {
                        foreach (TransferTypeAction transfer in transfers)
                        {
                            DeactivateTransfer(objectContext, userContext, transfer, Blog, false);
                        }
                    }
                }
            }

        }


        public void ScriptCheckForTransfersExpireTime(Entities objectContext, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            List<TransferTypeAction> transfers = objectContext.TransferTypeActionSet.Where
                (tta => tta.active == true).ToList();

            if (transfers != null && transfers.Count > 0)
            {
                List<TransferTypeAction> expireTransfers = new List<TransferTypeAction>();

                DateTime now = DateTime.UtcNow;

                foreach (TransferTypeAction transfer in transfers)
                {
                    TimeSpan span = DateTime.UtcNow - transfer.dateCreated;

                    if (span.Days >= Configuration.ActionTransactionNumDaysActive)
                    {
                        expireTransfers.Add(transfer);
                    }
                }

                EntitiesUsers contextUsers = new EntitiesUsers();

                if (expireTransfers.Count > 0)
                {
                    foreach (TransferTypeAction transfer in expireTransfers)
                    {
                        DeactivateTransfer(objectContext, contextUsers, transfer, bLog, true);
                    }
                }
            }
        }

    }
}
