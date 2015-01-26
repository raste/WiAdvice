﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessTypeSuggestions
    {
        private void Add(Entities objectContext, EntitiesUsers userContext, TypeSuggestion suggestion,
            User currentUser, User toUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }
            if (toUser == null)
            {
                throw new BusinessException("toUser is null");
            }
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }

            objectContext.AddToTypeSuggestionSet(suggestion);
            Tools.Save(objectContext);

            BusinessUserOptions buOptions = new BusinessUserOptions();
            buOptions.ChangeIfUserHaveUnseenTypeSuggestionData(userContext, toUser, true);

            bLog.LogTypeSuggestion(objectContext, userContext, suggestion, LogType.create, currentUser, string.Empty);
        }

        public void AddProductSuggestion(Entities objectContext, EntitiesUsers userContext, User currentUser
            , User toUser, Product product, string description, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }

            string error = string.Empty;

            if (!CanUserSendSuggestionForProduct(userContext, objectContext, currentUser, toUser, product, out error))
            {
                throw new BusinessException(string.Format("User id : {0} cannot send suggestion for product id : {1} reason from error ' {2} '."
                    , currentUser.ID, product.ID, error));
            }

            TypeSuggestion suggestion = new TypeSuggestion();

            suggestion.ByUser = Tools.GetUserID(objectContext, currentUser.ID, true);
            suggestion.active = true;
            suggestion.dateCreated = DateTime.UtcNow;
            suggestion.description = description;
            suggestion.ToUser = Tools.GetUserID(objectContext, toUser.ID, true);
            suggestion.type = "product";
            suggestion.typeID = product.ID;
            suggestion.visible = true;
            suggestion.visibleByUser = true;
            suggestion.visibleToUser = true;
            suggestion.changedStatus = null;

            Add(objectContext, userContext, suggestion, currentUser, toUser, bLog);
        }

        public void AddCompanySuggestion(Entities objectContext, EntitiesUsers userContext, User currentUser
            , User toUser, Company company, string description, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }

            string error = string.Empty;

            if (!CanUserSendSuggestionForCompany(userContext, objectContext, currentUser, toUser, company, out error))
            {
                throw new BusinessException(string.Format("User id : {0} cannot send suggestion for company id : {1}, reason from error ' {2} '."
                    , currentUser.ID, company.ID, error));
            }

            TypeSuggestion suggestion = new TypeSuggestion();

            suggestion.ByUser = Tools.GetUserID(objectContext, currentUser.ID, true);
            suggestion.active = true;
            suggestion.dateCreated = DateTime.UtcNow;
            suggestion.description = description;
            suggestion.ToUser = Tools.GetUserID(objectContext, toUser.ID, true);
            suggestion.type = "company";
            suggestion.typeID = company.ID;
            suggestion.visible = true;
            suggestion.visibleByUser = true;
            suggestion.visibleToUser = true;
            suggestion.changedStatus = null;

            Add(objectContext, userContext, suggestion, currentUser, toUser, bLog);
        }

        /// <summary>
        /// Checks if User can send suggestion for product..true if can..false if cannot with error
        /// </summary>
        public bool CanUserSendSuggestionForProduct(EntitiesUsers userContext, Entities objectContext
            , User currentUser, User toUser, Product product, out string error)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }
            if (toUser == null)
            {
                throw new BusinessException("toUser is null");
            }
            if (toUser.visible == false)
            {
                throw new BusinessException("toUser.visible is false");
            }
            if (product == null)
            {
                throw new BusinessException("product is null");
            }
            if (product.visible == false)
            {
                throw new BusinessException(string.Format("User id : {0} cannot add suggestion for product ID : {1}, because the product is visible:false"
                    , currentUser.ID, product.ID));
            }
            if (currentUser == toUser)
            {
                throw new BusinessException("currentUser == toUser");
            }

            error = string.Empty;
            bool result = false;

            if (CheckUser(userContext, objectContext, currentUser, out error))
            {
                if (CountUserSuggestions(objectContext, true, currentUser) < Configuration.TypeSuggestionMaxActiveSuggestionsPerUser)
                {
                    if (CheckIfUserHaveActiveSuggestionAboutType(objectContext, "product", product.ID, currentUser) == false)
                    {
                        BusinessUserTypeActions butAction = new BusinessUserTypeActions();
                        UsersTypeAction action = butAction.GetUserTypeAction(objectContext, "product", product.ID, toUser);

                        bool validToUser = false;
                        bool validToCurrUser = false;

                        if (action != null && action.visible == true)
                        {
                            validToUser = true;
                        }
                        else
                        {
                            if (!product.CompanyReference.IsLoaded)
                            {
                                product.CompanyReference.Load();
                            }

                            action = butAction.GetUserTypeAction(objectContext, "aCompProdModificator", product.Company.ID, toUser);

                            if (action != null && action.visible == true)
                            {
                                validToUser = true;
                            }
                        }

                        if (validToUser == true)
                        {
                            UsersTypeAction currUserAction = butAction.GetUserTypeAction(objectContext, "product", product.ID, currentUser);

                            if (currUserAction != null && currUserAction.visible == true)
                            {
                                error = Tools.GetResource("errTypeSuggCantSent");
                            }
                            else
                            {
                                if (!product.CompanyReference.IsLoaded)
                                {
                                    product.CompanyReference.Load();
                                }

                                currUserAction = butAction.GetUserTypeAction(objectContext, "aCompProdModificator", product.Company.ID, currentUser);

                                if (currUserAction != null && currUserAction.visible == true)
                                {
                                    error = Tools.GetResource("errTypeSuggCantSent");
                                }
                                else
                                {
                                    validToCurrUser = true;
                                }
                            }
                        }

                        if (validToUser == true && validToCurrUser == true)
                        {
                            result = true;
                        }
                        else
                        {
                            if (validToUser == false)
                            {
                                error = string.Format("{0} {1} {2}.", toUser.username
                                    , Tools.GetResource("errTypeSuggUserCantEdit"), product.name);
                            }
                        }
                    }
                    else
                    {
                        error = Tools.GetResource("errTypeSuggAlreadyHaveSugg");
                    }
                }
                else
                {
                    error = Tools.GetResource("errMaxTypeSuggNumberReached");
                }
            }

            return result;

        }

        /// <summary>
        /// Checks if User can send suggestion for company..true if can..false if cannot with error
        /// </summary>
        public bool CanUserSendSuggestionForCompany(EntitiesUsers userContext, Entities objectContext
            , User currentUser, User toUser, Company company, out string error)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }
            if (toUser == null)
            {
                throw new BusinessException("toUser is null");
            }
            if (toUser.visible == false)
            {
                throw new BusinessException("toUser.visible is false");
            }
            if (company == null)
            {
                throw new BusinessException("company is null");
            }
            if (company.visible == false)
            {
                throw new BusinessException(string.Format("User id : {0} cannot add suggestion for company ID : {1}, because the product is visible:false"
                    , currentUser.ID, company.ID));
            }
            if (currentUser == toUser)
            {
                throw new BusinessException("currentUser == toUser");
            }

            error = string.Empty;
            bool result = false;

            if (CheckUser(userContext, objectContext, currentUser, out error))
            {
                if (CountUserSuggestions(objectContext, true, currentUser) < Configuration.TypeSuggestionMaxActiveSuggestionsPerUser)
                {
                    if (CheckIfUserHaveActiveSuggestionAboutType(objectContext, "company", company.ID, currentUser) == false)
                    {
                        BusinessUserTypeActions butAction = new BusinessUserTypeActions();
                        UsersTypeAction action = butAction.GetUserTypeAction(objectContext, "company", company.ID, toUser);

                        if (action != null && action.visible == true)
                        {
                            UsersTypeAction currUserAction = butAction.GetUserTypeAction(objectContext, "company", company.ID, currentUser);

                            if (currUserAction != null && currUserAction.visible == true)
                            {
                                error = Tools.GetResource("errTypeSuggCantSentMaker");
                            }
                            else
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            error = string.Format("{0} {1} {2}.", toUser.username
                                , Tools.GetResource("errTypeSuggUserCantEditMaker"), company.name);
                        }
                    }
                    else
                    {
                        error = Tools.GetResource("errTypeSuggAlreadyHaveSuggMaker");
                    }
                }
                else
                {
                    error = Tools.GetResource("errMaxTypeSuggNumberReached");
                }
            }

            return result;

        }

        /// <summary>
        /// Returns true if user can send report about suggestion, otherwise false . (NOT CHECKING FOR USER REPORTS NUMBER, AND IF HAVE ROLE)
        /// </summary>
        public bool CanUserReportTypeSuggestion(User currentUser, TypeSuggestion suggestion)
        {
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }
            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }

            if (!suggestion.ByUserReference.IsLoaded)
            {
                suggestion.ByUserReference.Load();
            }
            if (!suggestion.ToUserReference.IsLoaded)
            {
                suggestion.ToUserReference.Load();
            }

            bool result = false;

            if (suggestion.ByUser.ID != currentUser.ID)
            {
                if (suggestion.active == false && suggestion.status != null)
                {
                    switch (suggestion.status)
                    {
                        case "accepted":

                            // check status date
                            TimeSpan span = DateTime.UtcNow - suggestion.changedStatus.Value;
                            if (span.Days >= Configuration.TypeSuggestionMaxDaysToUpdateWhenAccepted)
                            {
                                result = true;
                            }

                            break;
                        case "declined":

                            if (!suggestion.StatusByReference.IsLoaded)
                            {
                                suggestion.StatusByReference.Load();
                            }
                            if (suggestion.StatusBy == null)
                            {
                                throw new BusinessException(string.Format("Suggestion ID : {0} have status : {1} but no user (null) which changed status"
                            , suggestion.ID, suggestion.status));
                            }
                            if (suggestion.ToUser.ID == suggestion.StatusBy.ID)
                            {
                                result = true;
                            }

                            break;
                        case "expired":

                            result = true;

                            break;
                        default:
                            throw new BusinessException(string.Format("suggestion.status = {0} on suggestion id : {1} is not supported"
                                , suggestion.status, suggestion.ID));
                    }
                }
            }
            else if (suggestion.ToUser.ID != currentUser.ID)
            {
                if (suggestion.active == false)
                {
                    result = true;
                }
            }

            return result;

        }

        /// <summary>
        /// True if user can send suggestions otherwise false
        /// </summary>
        private bool CheckUser(EntitiesUsers userContext, Entities objectContext, User currentUser, out string error)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);

            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }

            error = string.Empty;
            bool result = false;

            BusinessUser bUser = new BusinessUser();
            if (bUser.IsUser(currentUser))
            {
                if (bUser.CanUserDo(userContext, currentUser, UserRoles.WriteSuggestions))
                {
                    if (CountUserSuggestions(objectContext, true, currentUser) <=
                        Configuration.TypeSuggestionMaxActiveSuggestionsPerUser)
                    {
                        result = true;
                    }
                    else
                    {
                        error = Tools.GetResource("errTypeSuggLimitReached");
                    }
                }
                else
                {
                    error = Tools.GetResource("errTypeSuggCantSentSuggestions");
                }
            }
            else
            {
                error = Tools.GetResource("errTypeSuggCantSentSuggestions");
            }

            return result;
        }

        /// <summary>
        /// Returns number of User Sugesstions (only those who are visible:true)
        /// </summary>
        public int CountUserSuggestions(Entities objectContext, bool activeOnly, User user)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            int count = 0;

            if (activeOnly == true)
            {
                count = objectContext.TypeSuggestionSet.Count(ts => ts.ByUser.ID == user.ID && ts.active == true && ts.visible == true);
            }
            else
            {
                count = objectContext.TypeSuggestionSet.Count(ts => ts.ByUser.ID == user.ID && ts.visible == true);
            }

            return count;

        }


        public void AddCommentToSuggestion(Entities objectContext, EntitiesUsers userContext
            , TypeSuggestion suggestion, User currentUser, bool bySystem, string description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }
            if (suggestion.active == false)
            {
                throw new BusinessException(string.Format("user id : {0} cannot add comment to type suggestion id : {1} because its not active"
                    , currentUser.ID, suggestion.ID));
            }
            if (suggestion.visible == false)
            {
                throw new BusinessException(string.Format("user id : {0} cannot add comment to type suggestion id : {1} because its not visible"
                    , currentUser.ID, suggestion.ID));
            }
            if (suggestion.visibleByUser == false || suggestion.visibleToUser == false)
            {
                throw new BusinessException(string.Format("user id : {0} cannot add comment to type suggestion id : {1} because its not visible locally visible to receiver or sender"
                    , currentUser.ID, suggestion.ID));
            }

            BusinessUser bUser = new BusinessUser();
            User toUser = null;

            if (!suggestion.ToUserReference.IsLoaded)
            {
                suggestion.ToUserReference.Load();
            }
            if (!suggestion.ByUserReference.IsLoaded)
            {
                suggestion.ByUserReference.Load();
            }


            if (bySystem == false)
            {
                if (suggestion.ToUser.ID != currentUser.ID)
                {
                    toUser = bUser.Get(userContext, suggestion.ToUser.ID, true);

                    if (suggestion.ByUser.ID != currentUser.ID)
                    {
                        throw new BusinessException(string.Format("user id : {0} cannot add comment to type suggestion id : {1} because he is not the one receiving or sending suggestion."
                        , currentUser.ID, suggestion.ID));
                    }
                }
                else
                {
                    toUser = bUser.Get(userContext, suggestion.ByUser.ID, true);
                }
            }
            else
            {
                if (bUser.IsUserValidType(currentUser) == true)
                {
                    throw new BusinessException(string.Format("User id : {0} is not system user, bySystem = true."));
                }
            }

            TypeSuggestionComment comment = new TypeSuggestionComment();

            comment.dateCreated = DateTime.UtcNow;
            comment.description = description;
            comment.Suggestion = suggestion;
            comment.User = Tools.GetUserID(objectContext, currentUser.ID, true);
            comment.visible = true;

            objectContext.AddToTypeSuggestionCommentSet(comment);
            Tools.Save(objectContext);

            BusinessUserOptions buOptions = new BusinessUserOptions();

            if (bySystem == false)
            {
                buOptions.ChangeIfUserHaveUnseenTypeSuggestionData(userContext, toUser, true);
            }
            else
            {
                toUser = bUser.Get(userContext, suggestion.ToUser.ID, true);
                User byUser = bUser.Get(userContext, suggestion.ByUser.ID, true);

                buOptions.ChangeIfUserHaveUnseenTypeSuggestionData(userContext, toUser, true);
                buOptions.ChangeIfUserHaveUnseenTypeSuggestionData(userContext, byUser, true);
            }
        }


        public void ScriptCheckForExpiredSuggestions(Entities objectContext, EntitiesUsers userContext, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            List<TypeSuggestion> suggestions = objectContext.TypeSuggestionSet.Where
                (ts => ts.active == true && ts.visible == true).ToList();

            if (suggestions.Count > 0)
            {
                List<TypeSuggestion> expiredSuggestions = new List<TypeSuggestion>();

                TimeSpan span = new TimeSpan();
                int expireAfterDays = Configuration.TypeSuggestionDaysAfterWhichSuggestionExpires;

                foreach (TypeSuggestion suggestion in suggestions)
                {
                    span = DateTime.UtcNow - suggestion.dateCreated;
                    if (span.Days >= expireAfterDays)
                    {
                        expiredSuggestions.Add(suggestion);
                    }
                }

                if (expiredSuggestions.Count > 0)
                {
                    BusinessUser bUser = new BusinessUser();
                    User System = bUser.GetSystem(userContext);

                    BusinessUserOptions bUserOptions = new BusinessUserOptions();
                    BusinessSystemMessages bSystemMessage = new BusinessSystemMessages();
                    BusinessCompany bCompany = new BusinessCompany();
                    BusinessProduct bProduct = new BusinessProduct();
                    User toUser = null;
                    User byUser = null;
                    Product product = null;
                    Company company = null;

                    string descrToAuthor = string.Empty;
                    string descrToEditor = string.Empty;

                    string smFirstPartAuthor = Tools.GetResource("SMtoAuthorOfEditSuggestWhenExpires").ToString();
                    string smSecondPartAuthor = Tools.GetResource("SMtoAuthorOfEditSuggestWhenExpires1").ToString();
                    string smThirdPartAuthor = Tools.GetResource("SMtoAuthorOfEditSuggestWhenExpires2").ToString();
                    string smForthPartAuthor = Tools.GetResource("SMtoAuthorOfEditSuggestWhenExpires3").ToString();

                    string smFirstPartEditor = Tools.GetResource("SMtoEditorWhenEditSuggestExpires").ToString();
                    string smSecondPartEditor = Tools.GetResource("SMtoEditorWhenEditSuggestExpires1").ToString();
                    string smThirdPartEditor = Tools.GetResource("SMtoEditorWhenEditSuggestExpires2").ToString();
                    string smForthPartEditor = Tools.GetResource("SMtoEditorWhenEditSuggestExpires3").ToString();

                    string suggForType = string.Empty;
                    string suggTypeProduct = Tools.GetResource("maker");
                    string suggTypeCompany = Tools.GetResource("product");
                    string suggTypeName = string.Empty;

                    foreach (TypeSuggestion suggestion in expiredSuggestions)
                    {
                        suggestion.active = false;
                        suggestion.status = "expired";
                        suggestion.StatusBy = Tools.GetUserID(objectContext, System.ID, true);
                        suggestion.changedStatus = DateTime.UtcNow;

                        Tools.Save(objectContext);

                        bLog.LogTypeSuggestion(objectContext, userContext, suggestion, LogType.edit, System, "expired");

                        if (!suggestion.ByUserReference.IsLoaded)
                        {
                            suggestion.ByUserReference.Load();
                        }
                        if (!suggestion.ToUserReference.IsLoaded)
                        {
                            suggestion.ToUserReference.Load();
                        }

                        toUser = bUser.Get(userContext, suggestion.ToUser.ID, true);
                        byUser = bUser.Get(userContext, suggestion.ByUser.ID, true);

                        bUserOptions.ChangeIfUserHaveUnseenTypeSuggestionData(userContext, toUser, true);
                        bUserOptions.ChangeIfUserHaveUnseenTypeSuggestionData(userContext, byUser, true);

                        switch (suggestion.type)
                        {
                            case "product":
                                suggForType = suggTypeProduct;
                                product = bProduct.GetProductByIDWV(objectContext, suggestion.typeID);
                                if (product == null)
                                {
                                    throw new BusinessException(string.Format("There is no product ID = {0}, for which User id = {1} wrote sugegstion (suggestion expired)"
                                        , suggestion.typeID, byUser.username));
                                }
                                suggTypeName = product.name;
                                break;
                            case "company":
                                suggForType = suggTypeCompany;
                                company = bCompany.GetCompanyWV(objectContext, suggestion.typeID);
                                if (company == null)
                                {
                                    throw new BusinessException(string.Format("There is no company ID = {0}, for which User id = {1} wrote sugegstion (suggestion expired)"
                                        , suggestion.typeID, byUser.username));
                                }
                                suggTypeName = company.name;
                                break;
                            default:
                                suggForType = suggestion.type;
                                suggTypeName = suggestion.typeID.ToString();
                                break;
                        }

                        //"The edit suggestion for product/company [name] which you sent to [user] expired, because [user] didn`t accept or decline it in [count] days.";
                        descrToAuthor = string.Format("{0} {1} '{2}' {3} '{4}' {5} {6} {7}"
                            , smFirstPartAuthor, suggForType, suggTypeName, smSecondPartAuthor, toUser.username, smThirdPartAuthor
                            , expireAfterDays, smForthPartAuthor);

                        //Edit suggestion for product/company [name] which was sent to you by [user] expired, because you didn`t accept or decline it in [count] days.
                        descrToEditor = string.Format("{0} {1} '{2}' {3} '{4}' {5} '{6}' {7}"
                            , smFirstPartEditor, suggForType, suggTypeName, smSecondPartEditor, byUser.username, smThirdPartEditor,
                            expireAfterDays, smForthPartEditor);

                        bSystemMessage.Add(userContext, byUser, descrToAuthor);
                        bSystemMessage.Add(userContext, toUser, descrToEditor);
                    }
                }

            }

        }

        public void AcceptSuggestion(Entities objectContext, EntitiesUsers userContext, BusinessLog bLog
            , TypeSuggestion suggestion, User currentUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (suggestion == null)
            {
                throw new BusinessException("Suggestion is null");
            }
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }
            if (suggestion.visible == false)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot accept suggestion id : {1} because the suggestion is not visible"
                    , currentUser.ID, suggestion.ID));
            }
            if (suggestion.active == false)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot accept suggestion id : {1} because the sugegstion is not active"
                    , currentUser.ID, suggestion.ID));
            }

            if (!suggestion.ToUserReference.IsLoaded)
            {
                suggestion.ToUserReference.Load();
            }
            if (!suggestion.ByUserReference.IsLoaded)
            {
                suggestion.ToUserReference.Load();
            }

            if (suggestion.ToUser.ID != currentUser.ID)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot accept suggestion id : {1} because he is not the one to which is the suggestion"
                    , currentUser.ID, suggestion.ID));
            }

            suggestion.status = "accepted";
            suggestion.active = false;
            suggestion.StatusBy = Tools.GetUserID(objectContext, currentUser.ID, true);
            suggestion.changedStatus = DateTime.UtcNow;

            Tools.Save(objectContext);

            bLog.LogTypeSuggestion(objectContext, userContext, suggestion, LogType.edit, currentUser, "accepted");

            BusinessUser bUser = new BusinessUser();
            User byUser = bUser.Get(userContext, suggestion.ByUser.ID, true);

            BusinessUserOptions buOptions = new BusinessUserOptions();
            buOptions.ChangeIfUserHaveUnseenTypeSuggestionData(userContext, byUser, true);

        }


        public void DeleteSuggestion(Entities objectContext, EntitiesUsers userContext, BusinessLog bLog
            , TypeSuggestion suggestion, User currentUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (suggestion == null)
            {
                throw new BusinessException("Suggestion is null");
            }
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }
            if (suggestion.visible == false)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot delete suggestion id : {1} because the suggestion is not visible"
                    , currentUser.ID, suggestion.ID));
            }

            if (!suggestion.ByUserReference.IsLoaded)
            {
                suggestion.ByUserReference.Load();
            }
            if (suggestion.ByUser.ID == currentUser.ID)
            {
                if (suggestion.active == true)
                {
                    DeclineSuggestion(objectContext, userContext, bLog, suggestion, currentUser, false, string.Empty);
                }

                suggestion.visibleByUser = false;

                Tools.Save(objectContext);
            }
            else
            {
                if (!suggestion.ToUserReference.IsLoaded)
                {
                    suggestion.ToUserReference.Load();
                }

                if (suggestion.ToUser.ID == currentUser.ID)
                {

                    if (suggestion.active == true)
                    {
                        DeclineSuggestion(objectContext, userContext, bLog, suggestion, currentUser, false, string.Empty);
                    }

                    suggestion.visibleToUser = false;

                    Tools.Save(objectContext);
                }
                else
                {
                    throw new BusinessException(string.Format("User Id : {0} cannot delete suggestion id : {1} because he is not the one who sent or received the suggestion"
                        , currentUser.ID, suggestion.ID));
                }
            }
        }

        /// <summary>
        /// bySystem:true only when system is automatically declining suggestions because roles were deleted
        /// </summary>
        public void DeclineSuggestion(Entities objectContext, EntitiesUsers userContext, BusinessLog bLog
            , TypeSuggestion suggestion, User currentUser, bool bySystem, string description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (suggestion == null)
            {
                throw new BusinessException("Suggestion is null");
            }
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }
            if (suggestion.visible == false)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot decline suggestion id : {1} because the suggestion is not visible"
                    , currentUser.ID, suggestion.ID));
            }
            if (suggestion.active == false)
            {
                throw new BusinessException(string.Format("User Id : {0} cannot decline suggestion id : {1} because the suggestion is not active"
                    , currentUser.ID, suggestion.ID));
            }

            if (!suggestion.ByUserReference.IsLoaded)
            {
                suggestion.ByUserReference.Load();
            }
            if (!suggestion.ToUserReference.IsLoaded)
            {
                suggestion.ToUserReference.Load();
            }

            BusinessUser bUser = new BusinessUser();
            BusinessUserOptions bUserOptions = new BusinessUserOptions();
            User userSm = null;

            if (bySystem == false)
            {
                if (suggestion.ByUser.ID == currentUser.ID)
                {
                    userSm = bUser.Get(userContext, suggestion.ToUser.ID, true);
                    bUserOptions.ChangeIfUserHaveUnseenTypeSuggestionData(userContext, userSm, true);
                }
                else if (suggestion.ToUser.ID == currentUser.ID)
                {
                    userSm = bUser.Get(userContext, suggestion.ByUser.ID, true);
                    bUserOptions.ChangeIfUserHaveUnseenTypeSuggestionData(userContext, userSm, true);
                }
                else
                {
                    throw new BusinessException(string.Format("User Id : {0} cannot decline suggestion id : {1} because he is not the one who sent or received the suggestion, and is not the system"
                        , currentUser.ID, suggestion.ID));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(description))
                {
                    throw new BusinessException("description is empty");
                }

                if (bUser.IsUserValidType(currentUser))
                {
                    throw new BusinessException(string.Format("User id : {0} is not system user, bySystem = true."));
                }

                AddCommentToSuggestion(objectContext, userContext, suggestion, currentUser, true, description);
            }

            suggestion.active = false;

            suggestion.status = "declined";
            suggestion.StatusBy = Tools.GetUserID(objectContext, currentUser.ID, true);
            suggestion.changedStatus = DateTime.UtcNow;

            Tools.Save(objectContext);

            bLog.LogTypeSuggestion(objectContext, userContext, suggestion, LogType.edit, currentUser, "declined");

        }


        /// <summary>
        /// returns visible true suggestion
        /// </summary>
        public TypeSuggestion GetSuggestion(Entities objectContext, long id, bool onlyActive, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            TypeSuggestion suggestion = null;
            string active = string.Empty;

            if (onlyActive == true)
            {
                active = "active:true ";
                suggestion = objectContext.TypeSuggestionSet.FirstOrDefault(ts => ts.ID == id && ts.active == true && ts.visible == true);
            }
            else
            {
                suggestion = objectContext.TypeSuggestionSet.FirstOrDefault(ts => ts.ID == id && ts.visible == true);
            }

            if (throwExcIfNull == true && suggestion == null)
            {
                throw new BusinessException(string.Format("There is no visible:true {0}type suggestion id : {1}", active, id));
            }

            return suggestion;
        }

        /// <summary>
        /// returns true if user have active sugegstion about type
        /// </summary>
        /// <param name="type">product, company</param>
        /// <param name="typeId"></param>
        public bool CheckIfUserHaveActiveSuggestionAboutType(Entities objectContext, string type, long typeId, User user)
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

            TypeSuggestion suggestion = objectContext.TypeSuggestionSet.FirstOrDefault
                (ts => ts.ByUser.ID == user.ID && ts.type == type && ts.typeID == typeId && ts.active == true && ts.visible == true);

            if (suggestion != null)
            {
                result = true;
            }

            return result;
        }


        /// <summary>
        /// returns visible:true suggestions for product/company
        /// </summary>
        /// <param name="type">product, company</param>
        public List<TypeSuggestion> GetSuggestionsForType(Entities objectContext, string type, long id, bool onlyActive)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (string.IsNullOrEmpty(type))
            {
                throw new BusinessException("type is empty");
            }

            if (type != "product" && type != "company")
            {
                throw new BusinessException(string.Format("type = {0} is not supported type", type));
            }

            List<TypeSuggestion> suggestions = new List<TypeSuggestion>();

            if (onlyActive == true)
            {
                suggestions = objectContext.TypeSuggestionSet.Where
                    (ts => ts.type == type && ts.typeID == id && ts.active == true && ts.visible == true).ToList();
            }
            else
            {
                suggestions = objectContext.TypeSuggestionSet.Where(ts => ts.type == type && ts.typeID == id && ts.visible == true).ToList();
            }

            return suggestions;

        }

        /// <summary>
        /// returns visible:true suggestions for product/company which are sent to user
        /// </summary>
        public List<TypeSuggestion> GetSuggestionsForTypeToUser(Entities objectContext, string type, long id, long userID, bool onlyActive)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (string.IsNullOrEmpty(type))
            {
                throw new BusinessException("type is empty");
            }

            if (type != "product" && type != "company")
            {
                throw new BusinessException(string.Format("type = {0} is not supported type", type));
            }

            List<TypeSuggestion> suggestions = new List<TypeSuggestion>();

            if (onlyActive == true)
            {
                suggestions = objectContext.TypeSuggestionSet.Where
                    (ts => ts.type == type && ts.typeID == id && ts.ToUser.ID == userID && ts.active == true && ts.visible == true).ToList();
            }
            else
            {
                suggestions = objectContext.TypeSuggestionSet.Where
                    (ts => ts.type == type && ts.typeID == id && ts.ToUser.ID == userID && ts.visible == true).ToList();
            }

            return suggestions;

        }

        /// <summary>
        /// returns visible:true suggestions sent to user
        /// </summary>

        public List<TypeSuggestion> GetSuggestionsToUser(Entities objectContext, User user, bool onlyActive, bool visibleToUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            List<TypeSuggestion> suggestions = new List<TypeSuggestion>();

            if (onlyActive == true)
            {
                if (visibleToUser == true)
                {
                    suggestions = objectContext.TypeSuggestionSet.Where
                        (ts => ts.ToUser.ID == user.ID && ts.active == true && ts.visibleToUser == true && ts.visible == true).ToList();
                }
                else
                {
                    suggestions = objectContext.TypeSuggestionSet.Where
                        (ts => ts.ToUser.ID == user.ID && ts.active == true && ts.visible == true).ToList();
                }
            }
            else
            {
                if (visibleToUser == true)
                {
                    suggestions = objectContext.TypeSuggestionSet.Where
                        (ts => ts.ToUser.ID == user.ID && ts.visibleToUser == true && ts.visible == true).ToList();
                }
                else
                {
                    suggestions = objectContext.TypeSuggestionSet.Where(ts => ts.ToUser.ID == user.ID && ts.visible == true).ToList();
                }
            }

            if (suggestions.Count > 1)
            {
                suggestions.Reverse();
            }

            return suggestions;
        }

        /// <summary>
        /// returns visible:true suggestions sent by user
        /// </summary>
        public List<TypeSuggestion> GetSuggestionsFromUser(Entities objectContext, User user, bool onlyActive, bool visibleFromUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            List<TypeSuggestion> suggestions = new List<TypeSuggestion>();

            if (onlyActive == true)
            {
                if (visibleFromUser == true)
                {
                    suggestions = objectContext.TypeSuggestionSet.Where
                        (ts => ts.ByUser.ID == user.ID && ts.active == true && ts.visibleByUser == true && ts.visible == true).ToList();
                }
                else
                {
                    suggestions = objectContext.TypeSuggestionSet.Where
                        (ts => ts.ByUser.ID == user.ID && ts.active == true && ts.visible == true).ToList();
                }

            }
            else
            {
                if (visibleFromUser == true)
                {
                    suggestions = objectContext.TypeSuggestionSet.Where
                        (ts => ts.ByUser.ID == user.ID && ts.visibleByUser == true && ts.visible == true).ToList();
                }
                else
                {
                    suggestions = objectContext.TypeSuggestionSet.Where(ts => ts.ByUser.ID == user.ID && ts.visible == true).ToList();
                }
            }

            if (suggestions.Count > 1)
            {
                suggestions.Reverse();
            }

            return suggestions;

        }

        /// <summary>
        /// returns visible:true comments for type suggestion
        /// </summary>
        public List<TypeSuggestionComment> GetSuggestionComments(Entities objectContext, TypeSuggestion suggestion)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }

            List<TypeSuggestionComment> comments = objectContext.TypeSuggestionCommentSet.Where
                (comm => comm.Suggestion.ID == suggestion.ID && comm.visible == true).ToList();

            return comments;
        }

        /// <summary>
        /// Used when role to user is removed, to decline also all edit suggestions to that user which are related with that role
        /// </summary>
        public void DeclineAllSuggestionToTypeAfteRoleIsRemoved(EntitiesUsers userContext, Entities objectContext
            , UsersTypeAction action, BusinessLog Blog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(Blog);

            if (action == null)
            {
                throw new BusinessException("action is null");
            }

            if (action.visible == true)
            {
                throw new BusinessException("action.visible is true");
            }

            if (!action.TypeActionReference.IsLoaded)
            {
                action.TypeActionReference.Load();
            }
            if (!action.UserReference.IsLoaded)
            {
                action.UserReference.Load();
            }

            BusinessUser bUser = new BusinessUser();
            TypeAction typeAction = action.TypeAction;
            List<TypeSuggestion> suggestions = new List<TypeSuggestion>();

            User toUser = bUser.GetWithoutVisible(userContext, action.User.ID, true);

            string description = string.Empty;

            switch (typeAction.type)
            {
                case "product":
                    BusinessProduct businessProduct = new BusinessProduct();
                    Product currProduct = businessProduct.GetProductByIDWV(objectContext, typeAction.typeID);
                    if (currProduct == null)
                    {
                        throw new BusinessException(string.Format("There is no product with ID : {0}", typeAction.typeID));
                    }

                    suggestions = GetSuggestionsForTypeToUser(objectContext, "product", currProduct.ID, action.User.ID, true);

                    description = string.Format("{0} {1} {2} {3}.", Tools.GetResource("TypeSuggACdeclined")
                        , toUser.username, Tools.GetResource("TypeSuggACdeclined2"), currProduct.name);

                    break;
                case "company":
                    BusinessCompany businessCompany = new BusinessCompany();
                    Company currCompany = businessCompany.GetCompanyWV(objectContext, typeAction.typeID);
                    if (currCompany == null)
                    {
                        throw new BusinessException(string.Format("There is no company with ID : {0}", typeAction.typeID));
                    }

                    suggestions = GetSuggestionsForTypeToUser(objectContext, "company", currCompany.ID, action.User.ID, true);

                    description = string.Format("{0} {1} {2} {3}.", Tools.GetResource("TypeSuggACdeclined")
                        , toUser.username, Tools.GetResource("TypeSuggACdeclined2"), currCompany.name);

                    break;
                case "aCompProdModificator":
                    BusinessCompany bCompany = new BusinessCompany();
                    BusinessProduct bProduct = new BusinessProduct();
                    Company currCompany1 = bCompany.GetCompanyWV(objectContext, typeAction.typeID);
                    if (currCompany1 == null)
                    {
                        throw new BusinessException(string.Format("There is no company with ID : {0}", typeAction.typeID));
                    }

                    List<TypeSuggestion> allSuggestions = GetSuggestionsToUser(objectContext, toUser, true, false);
                    if (allSuggestions.Count > 0)
                    {

                        description = string.Format
                            ("Automatic comment : {0} {1} {2} {3}.", Tools.GetResource("TypeSuggACdeclined")
                        , toUser.username, Tools.GetResource("TypeSuggACdeclined3"), currCompany1.name);

                        BusinessUserTypeActions butActions = new BusinessUserTypeActions();
                        List<long> productsForWhichHaveAction = new List<long>();

                        Product product = null;

                        foreach (TypeSuggestion suggestion in allSuggestions)
                        {
                            if (suggestion.type == "product")
                            {
                                if (!productsForWhichHaveAction.Contains(suggestion.typeID))
                                {
                                    product = bProduct.GetProductByIDWV(objectContext, suggestion.typeID);
                                    if (product == null)
                                    {
                                        throw new BusinessException(string.Format("There is no product id : {0} for which is type suggestion id : {1}"
                                            , suggestion.typeID, suggestion.ID));
                                    }
                                    if (!product.CompanyReference.IsLoaded)
                                    {
                                        product.CompanyReference.Load();
                                    }
                                    if (product.Company.ID == currCompany1.ID)
                                    {
                                        if (butActions.CheckIfUserHaveActionForType(objectContext, toUser, "product", product.ID) == true)
                                        {
                                            productsForWhichHaveAction.Add(product.ID);
                                        }
                                        else
                                        {
                                            // no roles for this product
                                            suggestions.Add(suggestion);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;
                default:
                    throw new BusinessException(string.Format("Type action with type : {0} is not supported when declining suggestions to removed role."
                        , typeAction.type));
            }

            if (suggestions.Count > 0)
            {
                User system = bUser.GetSystem(userContext);

                foreach (TypeSuggestion suggestion in suggestions)
                {
                    DeclineSuggestion(objectContext, userContext, Blog, suggestion, system, true, description);
                }
            }

        }

        public static string GetSuggestionStatus(TypeSuggestion suggestion)
        {
            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }

            string status = string.Empty;

            if (suggestion.status != null)
            {
                switch (suggestion.status)
                {
                    case "declined":
                        status = Tools.GetResource("ESuggestionStatusAccepted");
                        break;
                    case "expired":
                        status = Tools.GetResource("ESuggestionStatusExpired");
                        break;
                    case "accepted":
                        status = Tools.GetResource("ESuggestionStatusDeclined");
                        break;
                    default:
                        throw new BusinessException(string.Format("Suggestions status = {0} is not supported", suggestion.status));
                }
            }

            return status;
        }

        /// <summary>
        /// Declines all edit suggestions to/from user which is being deleted. User have to be visible.true.
        /// </summary>
        public void DeclineAllEditSuggestionWithUserWhenUserIsDeleted(EntitiesUsers userContext, Entities objectContext
            , User userDeleted, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (userDeleted == null)
            {
                throw new BusinessException("userDeleted is null");
            }

            if (userDeleted.visible == false)
            {
                throw new BusinessException(string.Format("Cannot decline all active edit suggestions to/from user id : {0}, because he is already deleted."
                    , userDeleted.ID));
            }

            List<TypeSuggestion> suggestions = objectContext.TypeSuggestionSet.Where
                        (ts => (ts.ByUser.ID == userDeleted.ID || ts.ToUser.ID == userDeleted.ID) && ts.active == true && ts.visible == true).ToList();

            if (suggestions != null && suggestions.Count > 0)
            {
                BusinessUser bUser = new BusinessUser();
                User system = bUser.GetSystem(userContext);
                string description = string.Format("{0} {1} {2}", Tools.GetResource("TypeSuggDeclinedCusUserIsDeleted")
                    , userDeleted.username, Tools.GetResource("TypeSuggDeclinedCusUserIsDeleted2"));

                foreach (TypeSuggestion suggestion in suggestions)
                {
                    DeclineSuggestion(objectContext, userContext, bLog, suggestion, system, true, description);
                }
            }

        }

        /// <summary>
        /// NOT USED (When company/product is deleted, editors have their roles removed..and from that..all edit suggestions to them)
        /// Declines all active edit suggestions for product/company which is deleted.
        /// </summary>
        /// <param name="type">product, company</param>
        /// <param name="currUser"> The user deleting the product/company</param>
        public void DeclineAllEditSuggestionsWhenTypeIsDeleted(EntitiesUsers userContext, Entities objectContext, string type, long id
            , BusinessLog bLog, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (string.IsNullOrEmpty(type))
            {
                throw new BusinessException("type is null or empty");
            }

            if (id < 1)
            {
                throw new BusinessException("id < 1");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            List<TypeSuggestion> activeSuggestions = new List<TypeSuggestion>();
            string description = string.Empty;

            switch (type)
            {
                case "product":
                    BusinessProduct bProduct = new BusinessProduct();
                    Product currProduct = bProduct.GetProductByIDWV(objectContext, id);
                    if (currProduct == null)
                    {
                        throw new BusinessException(string.Format("There is no product id : {0}, user id : {1}", id, currUser.ID));
                    }

                    if (currProduct.visible == true)
                    {
                        throw new BusinessException(string.Format("Cannot decline all edit suggestions for product ID : {0}, because the company is not deleted, user id : {1}."
                            , currProduct.ID, currUser.ID));
                    }

                    activeSuggestions = GetSuggestionsForType(objectContext, "product", currProduct.ID, true);
                    description = Tools.GetResource("CommToUsersAboutEditSuggWhenProductIsDeleted");

                    break;
                case "company":
                    BusinessCompany bCompany = new BusinessCompany();
                    Company currCompany = bCompany.GetCompanyWV(objectContext, id);
                    if (currCompany == null)
                    {
                        throw new BusinessException(string.Format("There is no company id : {0}, user id : {1}", id, currUser.ID));
                    }

                    if (currCompany.visible == true)
                    {
                        throw new BusinessException(string.Format("Cannot decline all edit suggestions for company ID : {0}, because the company is not deleted, user id : {1}."
                            , currCompany.ID, currUser.ID));
                    }

                    activeSuggestions = GetSuggestionsForType(objectContext, "company", currCompany.ID, true);
                    description = Tools.GetResource("CommToUsersAboutEditSuggWhenCompanyIsDeleted");

                    break;
                default:
                    throw new BusinessException(string.Format("Edit suggestion type = {0}, is not supported type, used id : {1}"
                        , type, currUser.ID));
            }

            if (activeSuggestions.Count > 0)
            {
                BusinessUser bUser = new BusinessUser();
                currUser = bUser.GetSystem(userContext);

                foreach (TypeSuggestion suggestion in activeSuggestions)
                {
                    DeclineSuggestion(objectContext, userContext, bLog, suggestion, currUser, true, description);
                }
            }

        }


    }
}
