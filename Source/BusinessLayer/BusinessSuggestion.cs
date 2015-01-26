// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessSuggestion
    {
        /// <summary>
        /// Adds new Suggestion 
        /// </summary>
        /// <param name="description">Suggestions text</param>
        public void Add(Entities objectContext, User currentUser, String description, SuggestionType type, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null or empty");
            }

            Suggestion suggestion = new Suggestion();

            suggestion.User = Tools.GetUserID(objectContext, currentUser);
            suggestion.dateCreated = DateTime.UtcNow;
            suggestion.description = description;
            suggestion.visible = true;
            suggestion.lastModified = suggestion.dateCreated;
            suggestion.LastModifiedBy = suggestion.User;
            suggestion.category = GetSuggestionTypeFromEnum(type);

            objectContext.AddToSuggestionSet(suggestion);
            Tools.Save(objectContext);

            bLog.LogSuggestion(objectContext, suggestion, LogType.create, currentUser);
        }

        /// <summary>
        /// Returns visible=true Suggestion with ID 
        /// </summary>
        public Suggestion Get(Entities objectContext, long id)
        {
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }
            Tools.AssertObjectContextExists(objectContext);
            Suggestion suggestion = objectContext.SuggestionSet.FirstOrDefault<Suggestion>(sugg => sugg.ID == id && sugg.visible == true);
            return suggestion;
        }

        /// <summary>
        /// Returns Suggestion with ID
        /// </summary>
        /// <param name="id">Comment ID</param>
        public Suggestion GetWithoutVisible(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }
            Suggestion suggestion = objectContext.SuggestionSet.FirstOrDefault<Suggestion>(comm => comm.ID == id);
            return suggestion;
        }

        /// <summary>
        /// Makes visible=false suggestion
        /// </summary>
        /// <param name="modifier">User performing the operation</param>
        public void DeleteSuggestion(Entities objectContext, EntitiesUsers userContext, Suggestion suggestion
            , User modifier, BusinessLog bLog, bool sendWarning)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }

            if (modifier == null)
            {
                throw new BusinessException("modifier is null");
            }

            if (suggestion.visible == false)
            {
                throw new BusinessException(string.Format("suggestion {0} is already deleted", suggestion.ID));
            }

            suggestion.visible = false;
            suggestion.lastModified = DateTime.UtcNow;
            suggestion.LastModifiedBy = Tools.GetUserID(objectContext, modifier);

            Tools.Save(objectContext);
            bLog.LogSuggestion(objectContext, suggestion, LogType.delete, modifier);

            BusinessReport businessReport = new BusinessReport();
            businessReport.ResolveReportsForSuggestionWhichIsBeingDeleted(objectContext, userContext, suggestion, bLog, modifier);

            if (!suggestion.UserReference.IsLoaded)
            {
                suggestion.UserReference.Load();
            }

            BusinessUser businessUser = new BusinessUser();
            User suggUser = businessUser.GetWithoutVisible(userContext, suggestion.User.ID, true);

            if (sendWarning == true)
            {
                BusinessUserActions buActions = new BusinessUserActions();
                UserAction action = buActions.GetUserAction(userContext, UserRoles.WriteSuggestions, suggUser);
                if (action != null && action.visible == true)
                {
                    BusinessWarnings bWarning = new BusinessWarnings();
                    string reason = string.Format("{0}<br />{1} ' {2} '", Tools.GetResource("DeletingSuggestionW")
                        , Tools.GetResource("DeletingSuggestionW2"), suggestion.description);

                    UserRoles forRole = UserRoles.WriteSuggestions;
                    bWarning.AddWarning(userContext, objectContext, action, forRole.ToString(), reason, suggUser, modifier, bLog);
                }
            }

            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
            string description = string.Format("{0}<br /><br /> ' {1} '"
                , Tools.GetResource("DeletingSuggestionSM"), suggestion.description);

            bSystemMessages.Add(userContext, suggUser, description);

        }

        /// <summary>
        /// Returns last number visible=false Suggestions
        /// </summary>
        public List<Suggestion> GetDeletedSuggestions(Entities objectContext, long number)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new BusinessException("number is < 1");
            }

            IEnumerable<Suggestion> Suggestions = objectContext.SuggestionSet.Where
                 (sugg => sugg.visible == false).OrderByDescending(new Func<Suggestion, long>(IdSelector));

            List<Suggestion> LastNumSuggestions = new List<Suggestion>();
            if (Suggestions.Count<Suggestion>() > 0)
            {
                long i = 0;

                foreach (Suggestion suggestion in Suggestions)
                {
                    if (i < number)
                    {
                        LastNumSuggestions.Add(suggestion);
                    }
                    else
                    {
                        break;
                    }
                    i++;
                }
            }

            return LastNumSuggestions;
        }

        /// <summary>
        /// Returns all visible = true suggestions
        /// </summary>
        public List<Suggestion> GetSuggestions(Entities objectContext, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (from >= to)
            {
                throw new BusinessException("from >= to");
            }

            IEnumerable<Suggestion> Suggestions = objectContext.GetSuggestionsForCategory("all", from, to);

            return Suggestions.ToList();
        }

        public static string GetSuggestionCategoryLocalText(Suggestion suggestion)
        {
            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }

            string result = string.Empty;

            switch (suggestion.category)
            {
                case "general":
                    result = Tools.GetResource("SuggestionGeneral");
                    break;
                case "design":
                    result = Tools.GetResource("SuggestionDesign");
                    break;
                case "features":
                    result = Tools.GetResource("SuggestionFeatures");
                    break;
                default:
                    throw new BusinessException(string.Format("suggestion.category = {0} is not supported for suggestion type.", suggestion.category));
            }

            return result;
        }

        /// <summary>
        /// doesn`t check for visible
        /// </summary>
        public int CountUserSuggestions(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            int count = objectContext.SuggestionSet.Count(sugg => sugg.User.ID == currUser.ID);

            return count;

        }

        /// <summary>
        /// Returns user suggestions
        /// </summary>
        public List<Suggestion> GetUserSuggestions(Entities objectContext, User currentUser, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }

            List<Suggestion> suggestions = new List<Suggestion>();

            if (onlyVisible)
            {
                suggestions = objectContext.SuggestionSet.Where(sugg => sugg.User.ID == currentUser.ID && sugg.visible == true).ToList();
            }
            else
            {
                suggestions = objectContext.SuggestionSet.Where(sugg => sugg.User.ID == currentUser.ID).ToList();
            }

            if (suggestions.Count > 0)
            {
                suggestions.Reverse();
            }

            return suggestions;
        }

        /// <summary>
        /// Returns suggestion from type
        /// </summary>
        public List<Suggestion> GetSuggestionsWithType(Entities objectContext, SuggestionType type, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (from >= to)
            {
                throw new BusinessException("from >= to");
            }

            string strType = GetSuggestionTypeFromEnum(type);

            IEnumerable<Suggestion> Suggestions = objectContext.GetSuggestionsForCategory(strType, from, to);

            return Suggestions.ToList();
        }


        /// <summary>
        /// Makes Suggestion visible=true
        /// </summary>
        public void UnDeleteSuggestion(Entities objectContext, Suggestion suggestion, User modifier, BusinessLog bLog)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);

            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }

            if (modifier == null)
            {
                throw new BusinessException("modifier is null");
            }

            if (suggestion.visible)
            {
                throw new BusinessException(string.Format("suggestion {0} is already visible", suggestion.ID));
            }

            suggestion.visible = true;
            suggestion.lastModified = DateTime.UtcNow;
            suggestion.LastModifiedBy = Tools.GetUserID(objectContext, modifier);

            Tools.Save(objectContext);
            bLog.LogSuggestion(objectContext, suggestion, LogType.undelete, modifier);
        }


        /// <summary>
        /// Returns number of all visible=true suggestions
        /// </summary>
        public long CountVisibleSuggestions(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            long count = objectContext.SuggestionSet.Count<Suggestion>(sugg => sugg.visible == true);
            return count;
        }

        /// <summary>
        /// Returns number of all visible=true suggestions from type
        /// </summary>
        public long CountVisibleSuggestionFromType(Entities objectContext, SuggestionType type)
        {
            Tools.AssertObjectContextExists(objectContext);
            string strType = GetSuggestionTypeFromEnum(type);
            long count = objectContext.SuggestionSet.Count<Suggestion>(sugg => sugg.category == strType && sugg.visible == true);
            return count;
        }

        /// <summary>
        /// Function used in Sort By Descending .. sorts by id
        /// </summary>
        private long IdSelector(Suggestion suggestion)
        {
            if (suggestion == null)
            {
                throw new ArgumentNullException("suggestion");
            }
            return suggestion.ID;
        }

        private string GetSuggestionTypeFromEnum(SuggestionType type)
        {
            string strType = "";

            switch (type)
            {
                case SuggestionType.General:
                    strType = "general";
                    break;
                case SuggestionType.Design:
                    strType = "design";
                    break;
                case SuggestionType.Features:
                    strType = "features";
                    break;
                default:
                    throw new BusinessException(string.Format("SuggestionType = {0} is not supported type", type));
            }

            return strType;
        }

    }
}
