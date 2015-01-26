// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessSiteText
    {
        private static object creatingSiteText = new object();

        /// <summary>
        /// Add`s new SiteText to database
        /// </summary>
        private void Add(Entities objectContext, SiteNews news, BusinessLog bLog, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (news == null)
            {
                throw new BusinessException("news is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            objectContext.AddToSiteNewsSet(news);
            Tools.Save(objectContext);
            bLog.LogSiteText(objectContext, news, LogType.create, string.Empty, string.Empty, currUser);
        }

        /// <summary>
        /// Returns SiteText with id
        /// </summary>
        public SiteNews Get(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id < 1");
            }

            SiteNews wanted = objectContext.SiteNewsSet.FirstOrDefault(text => text.ID == id);

            return wanted;
        }

        /// <summary>
        /// Returns SiteText with name
        /// </summary>
        public SiteNews Get(Entities objectContext, String name)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }

            SiteNews wanted = objectContext.SiteNewsSet.FirstOrDefault(text => text.name == name);

            return wanted;
        }

        /// <summary>
        /// Creates News and Add`s them to database
        /// </summary>
        public void CreateNews(Entities objectContext, BusinessLog bLog, String description, User createdBy, String name, string linkID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (createdBy == null)
            {
                throw new BusinessException("createdBy is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }
            if (string.IsNullOrEmpty(linkID))
            {
                throw new BusinessException("linkID is empty");
            }

            if (!Tools.NameValidatorPassed(objectContext, "siteText", name, 0))
            {
                throw new BusinessException(string.Format("There is already site text with name {0}", name));
            }

            if (CheckIfLinkIdIsTaken(objectContext, "news", linkID) == true)
            {
                throw new BusinessException(string.Format("New news with name = {0} cannot be added because the provided Link ID is already taken, User id : {1}"
                    , name, linkID));
            }

            lock (creatingSiteText)
            {
                SiteNews newNews = new SiteNews();

                newNews.name = name;
                newNews.description = description;
                newNews.dateCreated = DateTime.UtcNow;
                newNews.type = "news";
                newNews.visible = true;
                newNews.CreatedBy = Tools.GetUserID(objectContext, createdBy);
                newNews.lastModified = newNews.dateCreated;
                newNews.LastModifiedBy = newNews.CreatedBy;
                newNews.linkID = linkID;

                Add(objectContext, newNews, bLog, createdBy);
            }
        }

        public void CreateWarningPattern(Entities objectContext, BusinessLog bLog, String description, User createdBy, String name, string linkID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (createdBy == null)
            {
                throw new BusinessException("createdBy is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }
            if (!Tools.NameValidatorPassed(objectContext, "siteText", name, 0))
            {
                throw new BusinessException(string.Format("There is already site text with name {0}", name));
            }
            if (string.IsNullOrEmpty(linkID))
            {
                throw new BusinessException("linkID is empty");
            }
            if (CheckIfLinkIdIsTaken(objectContext, "warningPattern", linkID) == true)
            {
                throw new BusinessException(string.Format("New warning pattern with name = {0} cannot be added because the provided Link ID is already taken, User id : {1}"
                    , name, linkID));
            }

            lock (creatingSiteText)
            {
                SiteNews newNews = new SiteNews();

                newNews.name = name;
                newNews.description = description;
                newNews.dateCreated = DateTime.UtcNow;
                newNews.type = "warningPattern";
                newNews.visible = true;
                newNews.CreatedBy = Tools.GetUserID(objectContext, createdBy);
                newNews.lastModified = newNews.dateCreated;
                newNews.LastModifiedBy = newNews.CreatedBy;
                newNews.linkID = linkID;

                Add(objectContext, newNews, bLog, createdBy);
            }
        }

        public void CreateReportPattern(Entities objectContext, BusinessLog bLog, String description, User createdBy, String name, string linkID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (createdBy == null)
            {
                throw new BusinessException("createdBy is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }
            if (string.IsNullOrEmpty(linkID))
            {
                throw new BusinessException("linkID is empty");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }
            if (!Tools.NameValidatorPassed(objectContext, "siteText", name, 0))
            {
                throw new BusinessException(string.Format("There is already site text with name {0}", name));
            }
            if (CheckIfLinkIdIsTaken(objectContext, "reportPattern", linkID) == true)
            {
                throw new BusinessException(string.Format("New report pattern with name = {0} cannot be added because the provided Link ID is already taken, User id : {1}"
                    , name, linkID));
            }

            lock (creatingSiteText)
            {
                SiteNews newNews = new SiteNews();

                newNews.name = name;
                newNews.description = description;
                newNews.dateCreated = DateTime.UtcNow;
                newNews.type = "reportPattern";
                newNews.visible = true;
                newNews.CreatedBy = Tools.GetUserID(objectContext, createdBy);
                newNews.lastModified = newNews.dateCreated;
                newNews.LastModifiedBy = newNews.CreatedBy;
                newNews.linkID = linkID;

                Add(objectContext, newNews, bLog, createdBy);
            }
        }

        /// <summary>
        /// Created and Add`s new SiteText
        /// </summary>
        public void CreateSiteText(Entities objectContext, BusinessLog bLog, String description, User createdBy, String name, string linkID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty or null");
            }
            if (createdBy == null)
            {
                throw new BusinessException("createdBy is null");
            }
            if (string.IsNullOrEmpty(linkID))
            {
                throw new BusinessException("linkID is empty");
            }
            if (CheckIfLinkIdIsTaken(objectContext, "text", linkID) == true)
            {
                throw new BusinessException(string.Format("New text with  name = {0} cannot be added because the provided Link ID is already taken, User id : {1}"
                    , name, linkID));
            }

            lock (creatingSiteText)
            {
                SiteNews checkName = objectContext.SiteNewsSet.FirstOrDefault(news => news.name == name);
                if (checkName != null)
                {
                    throw new BusinessException(string.Format("SiteText with name {0} already exists!", name));
                }

                SiteNews newText = new SiteNews();
                newText.name = name;
                newText.description = description;
                newText.CreatedBy = Tools.GetUserID(objectContext, createdBy);
                newText.dateCreated = DateTime.UtcNow;
                newText.type = "text";
                newText.visible = true;
                newText.LastModifiedBy = newText.CreatedBy;
                newText.lastModified = newText.dateCreated;
                newText.linkID = linkID;

                Add(objectContext, newText, bLog, createdBy);
            }
        }

        /// <summary>
        /// returns false if linkID for the type is not taken, otherwise false.
        /// </summary>
        public bool CheckIfLinkIdIsTaken(Entities objectContext, string textType, string linkId)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(textType))
            {
                throw new BusinessException("textType is empty");
            }

            if (string.IsNullOrEmpty(linkId))
            {
                throw new BusinessException("linkId is empty");
            }

            bool result = false;

            List<string> allowedTypes = new List<string> { "qna", "text", "rule", "news", "warningPattern", "reportPattern", "information" };
            bool isTypeValid = false;

            foreach (string type in allowedTypes)
            {
                if (textType == type)
                {
                    isTypeValid = true;
                    break;
                }
            }

            if (isTypeValid == false)
            {
                throw new BusinessException(string.Format("Text type = {0} is not supported type when checking if linkID is taken."
                        , textType));
            }

            SiteNews testNews = objectContext.SiteNewsSet.FirstOrDefault(text => text.type == textType && text.linkID == linkId);
            if (testNews != null)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks linkID format. Returns true if format is correct, otherwise false. 
        /// </summary>
        public bool IsLinkIdFormatValid(string linkID)
        {
            if (string.IsNullOrEmpty(linkID))
            {
                return false;
            }

            bool result = true;

            if (linkID.StartsWith(" ") || linkID.EndsWith(" "))
            {
                result = false;
            }

            return result;
        }


        /// <summary>
        /// Creates and Add`s to Database QuestionAndAnswer
        /// </summary>
        public void CreateQuestionAndAnswer(Entities objectContext, BusinessLog bLog, String description, User createdBy, String name, string linkID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (createdBy == null)
            {
                throw new BusinessException("createdBy is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }
            if (string.IsNullOrEmpty(linkID))
            {
                throw new BusinessException("linkID is empty");
            }
            if (!Tools.NameValidatorPassed(objectContext, "siteText", name, 0))
            {
                throw new BusinessException(string.Format("There is already site text with name {0}", name));
            }
            if (CheckIfLinkIdIsTaken(objectContext, "qna", linkID) == true)
            {
                throw new BusinessException(string.Format("New qna with name = {0} cannot be added because the provided Link ID is already taken, User id : {1}"
                    , name, linkID));
            }

            lock (creatingSiteText)
            {
                SiteNews newNews = new SiteNews();

                newNews.name = name;
                newNews.description = description;
                newNews.dateCreated = DateTime.UtcNow;
                newNews.type = "qna";
                newNews.visible = true;
                newNews.CreatedBy = Tools.GetUserID(objectContext, createdBy);
                newNews.lastModified = newNews.dateCreated;
                newNews.LastModifiedBy = newNews.CreatedBy;
                newNews.linkID = linkID;

                Add(objectContext, newNews, bLog, createdBy);
            }
        }

        /// <summary>
        /// Creates and Add`s Rule To database
        /// </summary>
        public void CreateRule(Entities objectContext, BusinessLog bLog, String description, User createdBy, String name, string linkID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (createdBy == null)
            {
                throw new BusinessException("createdBy is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }
            if (string.IsNullOrEmpty(linkID))
            {
                throw new BusinessException("linkID is empty");
            }
            if (!Tools.NameValidatorPassed(objectContext, "siteText", name, 0))
            {
                throw new BusinessException(string.Format("There is already site text with name {0}", name));
            }
            if (CheckIfLinkIdIsTaken(objectContext, "rule", linkID) == true)
            {
                throw new BusinessException(string.Format("New rule with name = {0} cannot be added because the provided Link ID is already taken, User id : {1}"
                    , name, linkID));
            }

            lock (creatingSiteText)
            {
                SiteNews newNews = new SiteNews();

                newNews.name = name;
                newNews.description = description;
                newNews.dateCreated = DateTime.UtcNow;
                newNews.type = "rule";
                newNews.visible = true;
                newNews.CreatedBy = Tools.GetUserID(objectContext, createdBy);
                newNews.lastModified = newNews.dateCreated;
                newNews.LastModifiedBy = newNews.CreatedBy;
                newNews.linkID = linkID;

                Add(objectContext, newNews, bLog, createdBy);
            }
        }

        public void CreateInformation(Entities objectContext, BusinessLog bLog, String description, User createdBy, String name, string linkID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (createdBy == null)
            {
                throw new BusinessException("createdBy is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }
            if (string.IsNullOrEmpty(linkID))
            {
                throw new BusinessException("linkID is empty");
            }
            if (!Tools.NameValidatorPassed(objectContext, "siteText", name, 0))
            {
                throw new BusinessException(string.Format("There is already site text with name {0}", name));
            }
            if (CheckIfLinkIdIsTaken(objectContext, "information", linkID) == true)
            {
                throw new BusinessException(string.Format("New information with name = {0} cannot be added because the provided Link ID is already taken, User id : {1}"
                    , name, linkID));
            }

            lock (creatingSiteText)
            {
                SiteNews newNews = new SiteNews();

                newNews.name = name;
                newNews.description = description;
                newNews.dateCreated = DateTime.UtcNow;
                newNews.type = "information";
                newNews.visible = true;
                newNews.CreatedBy = Tools.GetUserID(objectContext, createdBy);
                newNews.lastModified = newNews.dateCreated;
                newNews.LastModifiedBy = newNews.CreatedBy;
                newNews.linkID = linkID;

                Add(objectContext, newNews, bLog, createdBy);
            }
        }


        /// <summary>
        /// Makes SiteNews obect visible=false
        /// </summary>
        public void DeleteNewsOrText(Entities objectContext, BusinessLog bLog, User currUser, SiteNews news)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (news == null)
            {
                throw new BusinessException("news is null");
            }
            if (news.visible == false)
            {
                throw new BusinessException(string.Format("news or text with id:{0} name:{1} is already visible false", news.ID, news.name));
            }

            if (news.visible == true)
            {
                news.visible = false;
                news.lastModified = DateTime.UtcNow;
                news.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                Tools.Save(objectContext);
                bLog.LogSiteText(objectContext, news, LogType.delete, string.Empty, string.Empty, currUser);
            }
        }

        /// <summary>
        /// Makes visible=true SiteNews obect
        /// </summary>
        public void UnDeleteNewsOrText(Entities objectContext, BusinessLog bLog, User currUser, SiteNews news)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (news == null)
            {
                throw new BusinessException("news is null");
            }
            if (news.visible)
            {
                throw new BusinessException(string.Format("news or text with id:{0} name:{1} is already visible true", news.ID, news.name));
            }

            if (news.visible == false)
            {
                news.visible = true;
                news.lastModified = DateTime.UtcNow;
                news.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                Tools.Save(objectContext);
                bLog.LogSiteText(objectContext, news, LogType.undelete, string.Empty, string.Empty, currUser);
            }
        }

        /// <summary>
        /// Changes SiteNews description
        /// </summary>
        public void ChangeDescription(Entities objectContext, BusinessLog bLog, User currUser, SiteNews news, String newDescription)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (news == null)
            {
                throw new BusinessException("news is null");
            }
            if (string.IsNullOrEmpty(newDescription))
            {
                throw new BusinessException("newDescription is null or empty");
            }

            if (news.description != newDescription)
            {
                String oldValue = news.description;

                news.description = newDescription;
                news.lastModified = DateTime.UtcNow;
                news.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                Tools.Save(objectContext);
                bLog.LogSiteText(objectContext, news, LogType.edit, "description", oldValue, currUser);
            }
        }

        public void ChangeLinkID(Entities objectContext, BusinessLog bLog, User currUser, SiteNews news, string newLinkID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (news == null)
            {
                throw new BusinessException("news is null");
            }
            if (string.IsNullOrEmpty(newLinkID))
            {
                throw new BusinessException("newLinkID is empty");
            }
            if (CheckIfLinkIdIsTaken(objectContext, news.type, newLinkID) == true)
            {
                throw new BusinessException(string.Format("Site text with type = {0}, ID = {1}, link id cannot be changed to : {2} , because its already taken. USer id : {3}"
                    , news.type, news.ID, newLinkID, currUser.ID));
            }

            if (news.linkID != newLinkID)
            {
                String oldValue = news.linkID;

                news.linkID = newLinkID;
                news.lastModified = DateTime.UtcNow;
                news.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                Tools.Save(objectContext);
                bLog.LogSiteText(objectContext, news, LogType.edit, "linkID", oldValue, currUser);
            }
        }

        /// <summary>
        /// Changes name of SiteNews obect 
        /// </summary>
        public void ChangeName(Entities objectContext, BusinessLog bLog, User currUser, SiteNews news, String newName)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (news == null)
            {
                throw new BusinessException("news is null");
            }
            if (news.visible == false)
            {
                throw new BusinessException(string.Format("news or text with id:{0} name:{1} is already visible false", news.ID, news.name));
            }
            if (string.IsNullOrEmpty(newName))
            {
                throw new BusinessException("newName is null or empty");
            }
            if (!Tools.NameValidatorPassed(objectContext, "siteText", newName, 0))
            {
                throw new BusinessException(string.Format("There is already text with name = {0}", newName));
            }

            lock (creatingSiteText)
            {
                String oldValue = news.name;

                news.name = newName;
                news.lastModified = DateTime.UtcNow;
                news.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                Tools.Save(objectContext);
                bLog.LogSiteText(objectContext, news, LogType.edit, "description", oldValue, currUser);
            }
        }

        /// <summary>
        /// Returns number of visible=true News
        /// </summary>
        public int CountSiteNews(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            int count = objectContext.SiteNewsSet.Count<SiteNews>
                (news => news.type == "news" && news.visible == true);
            return count;
        }

        /// <summary>
        /// Returns FAQ`s, ordered by name when returns only visible
        /// </summary>
        /// <param name="withOutVisible">true if should get all,false if only visible=true</param>
        public IEnumerable<SiteNews> GetFAQ(Entities objectContext, Boolean withOutVisible)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<SiteNews> FAQ = null;
            if (withOutVisible)
            {
                FAQ = objectContext.SiteNewsSet.Where(qna => qna.type == "qna");
            }
            else
            {
                FAQ = objectContext.SiteNewsSet.Where(qna => qna.type == "qna" && qna.visible == true)
                    .OrderBy<SiteNews, string>(new Func<SiteNews, string>(NameSelector));
            }

            return FAQ;
        }

        /// <summary>
        /// Returns Rules, ordered by name when returns only visible
        /// </summary>
        /// <param name="withOutVisible">true if should get all,false if only visible=true</param>
        public IEnumerable<SiteNews> GetRules(Entities objectContext, Boolean withOutVisible)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<SiteNews> Rules = null;
            if (withOutVisible)
            {
                Rules = objectContext.SiteNewsSet.Where(rule => rule.type == "rule");
            }
            else
            {
                Rules = objectContext.SiteNewsSet.Where(rule => rule.type == "rule" && rule.visible == true)
                    .OrderBy<SiteNews, string>(new Func<SiteNews, string>(NameSelector));
            }

            return Rules;
        }

        /// <summary>
        /// Returns guide text, ordered by name when returns only visible
        /// </summary>
        public IEnumerable<SiteNews> GetInformation(Entities objectContext, Boolean withOutVisible)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<SiteNews> Rules = null;
            if (withOutVisible)
            {
                Rules = objectContext.SiteNewsSet.Where(qna => qna.type == "information");
            }
            else
            {
                Rules = objectContext.SiteNewsSet.Where(qna => qna.type == "information" && qna.visible == true)
                    .OrderBy<SiteNews, string>(new Func<SiteNews, string>(NameSelector));
            }

            return Rules;
        }


        /// <summary>
        /// Returns Last number added text of type
        /// </summary>
        /// <param name="number">bigger than 0</param>
        /// <param name="type">all,news,text,qna,rules,warning patterns,report patterns,information</param>
        /// <param name="visibility">0 - all, 1 - true, 2 - false</param>
        public List<SiteNews> GetLastTexts(Entities objectContext, int number, string type, int visibility)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new BusinessException("type is null or empty");
            }
            if (visibility < 0 || visibility > 2)
            {
                throw new BusinessException("visibility is < 0 or > 2");
            }
            List<SiteNews> lastNumTexts = new List<SiteNews>();

            IEnumerable<SiteNews> Texts = null;
            switch (visibility)
            {   //0 - all, 1 - true, 2 - false
                case 0:
                    Texts = objectContext.SiteNewsSet.OrderByDescending<SiteNews, long>(new Func<SiteNews, long>(IdSelector));
                    break;
                case 1:
                    Texts = objectContext.SiteNewsSet.Where(text => text.visible == true).
                    OrderByDescending<SiteNews, long>(new Func<SiteNews, long>(IdSelector));
                    break;
                case 2:
                    Texts = objectContext.SiteNewsSet.Where(text => text.visible == false).
                    OrderByDescending<SiteNews, long>(new Func<SiteNews, long>(IdSelector));
                    break;
                default:
                    throw new BusinessException(string.Format("visibility = {0} is not supported case", visibility));
            }

            if (Texts.Count<SiteNews>() > 0)
            {
                int i = 0;

                switch (type)
                {
                    case ("all"):
                        foreach (SiteNews text in Texts)
                        {
                            if (i < number)
                            {
                                lastNumTexts.Add(text);
                            }
                            else
                            {
                                break;
                            }
                            i++;
                        }
                        break;
                    case ("news"):
                        foreach (SiteNews text in Texts)
                        {
                            if (i < number)
                            {
                                if (text.type == "news")
                                {
                                    lastNumTexts.Add(text);
                                    i++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    case ("text"):
                        foreach (SiteNews text in Texts)
                        {
                            if (i < number)
                            {
                                if (text.type == "text")
                                {
                                    lastNumTexts.Add(text);
                                    i++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    case ("qna"):
                        foreach (SiteNews text in Texts)
                        {
                            if (i < number)
                            {
                                if (text.type == "qna")
                                {
                                    lastNumTexts.Add(text);
                                    i++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    case ("rules"):
                        foreach (SiteNews text in Texts)
                        {
                            if (i < number)
                            {
                                if (text.type == "rule")
                                {
                                    lastNumTexts.Add(text);
                                    i++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    case ("warning patterns"):
                        foreach (SiteNews text in Texts)
                        {
                            if (i < number)
                            {
                                if (text.type == "warningPattern")
                                {
                                    lastNumTexts.Add(text);
                                    i++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    case ("report patterns"):
                        foreach (SiteNews text in Texts)
                        {
                            if (i < number)
                            {
                                if (text.type == "reportPattern")
                                {
                                    lastNumTexts.Add(text);
                                    i++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    case ("information"):
                        foreach (SiteNews text in Texts)
                        {
                            if (i < number)
                            {
                                if (text.type == "information")
                                {
                                    lastNumTexts.Add(text);
                                    i++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    default:
                        throw new BusinessException(string.Format("type = {0} is not supported type", type));

                }

            }

            return lastNumTexts;
        }

        /// <summary>
        /// Returns SiteText with Name
        /// </summary>
        /// <param name="name">about,aboutFAQ,aboutExtended,suggestionsAbout,aboutRules,registration,aboutAdmins,
        /// aboutAdverts,aboutLogs,aboutAddCompany,aboutAddProduct,aboutReports,aboutSiteTexts,aboutStatistics,aboutTopicReporting</param>
        public SiteNews GetSiteText(Entities objectContext, string name)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }

            SiteNews wantedText = null;

            switch (name)
            {
                case ("about"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                (abt => abt.name == "about");
                    break;
                case ("aboutFAQ"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                (abt => abt.name == "aboutFAQ");
                    break;
                case ("aboutExtended"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                (abt => abt.name == "aboutExtended");
                    break;
                case ("suggestionsAbout"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
               (abt => abt.name == "suggestionsAbout");
                    break;
                case ("aboutRules"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                (abt => abt.name == "aboutRules");
                    break;
                case ("registration"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                (abt => abt.name == "registration");
                    break;
                case ("aboutAdmins"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutAdmins");
                    break;
                case ("aboutAdverts"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutAdverts");
                    break;
                case ("aboutLogs"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutLogs");
                    break;
                case ("aboutAddCompany"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutAddCompany");
                    break;
                case ("aboutAddProduct"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutAddProduct");
                    break;
                case ("aboutReports"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutReports");
                    break;
                case ("aboutSiteTexts"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutSiteTexts");
                    break;
                case ("aboutStatistics"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutStatistics");
                    break;
                case ("aboutUserReporting"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutUserReporting");
                    break;
                case ("aboutGeneralReporting"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutGeneralReporting");
                    break;
                case ("aboutReportUser"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutReportUser");
                    break;
                case ("aboutCompanyTypes"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutCompanyTypes");
                    break;
                case ("aboutLogIn"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutLogIn");
                    break;
                case ("aboutWarnings"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutWarnings");
                    break;
                case ("aboutEditSuggestions"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutEditSuggestions");
                    break;
                case ("aboutReportEditSuggestion"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutReportEditSuggestion");
                    break;
                case ("aboutGuide"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutGuide");
                    break;
                case ("aboutTopicReporting"):
                    wantedText = objectContext.SiteNewsSet.FirstOrDefault
                        (abt => abt.name == "aboutTopicReporting");
                    break;
                default:
                    throw new BusinessException(string.Format("name = {0} is not supported name"));
            }

            return wantedText;
        }

        /// <summary>
        /// Returns names of all siteTexts that arent added to database
        /// </summary>
        public List<String> GetMissingTexts(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<string> missingTexts = new List<string>();
            SiteNews checkForText = null;

            List<string> texts = new List<string>();
            texts.Add("about");
            texts.Add("aboutExtended");
            texts.Add("suggestionsAbout");
            texts.Add("aboutFAQ");
            texts.Add("aboutRules");
            texts.Add("registration");
            texts.Add("aboutAdmins");
            texts.Add("aboutAdverts");
            texts.Add("aboutLogs");
            texts.Add("aboutAddCompany");
            texts.Add("aboutAddProduct");
            texts.Add("aboutReports");
            texts.Add("aboutSiteTexts");
            texts.Add("aboutStatistics");
            texts.Add("aboutUserReporting");
            texts.Add("aboutGeneralReporting");
            texts.Add("aboutCompanyTypes");
            texts.Add("aboutLogIn");
            texts.Add("aboutReportUser");
            texts.Add("aboutWarnings");
            texts.Add("aboutEditSuggestions");
            texts.Add("aboutReportEditSuggestion");
            texts.Add("aboutGuide");
            texts.Add("aboutTopicReporting");

            // Add new texts that need to be added to database and arent

            if (texts.Count > 0)
            {
                foreach (string text in texts)
                {
                    checkForText = GetSiteText(objectContext, text); // Add them here also
                    if (checkForText == null)
                    {
                        missingTexts.Add(text);
                    }
                }
            }

            return missingTexts;
        }

        private long IdSelector(SiteNews news)
        {
            if (news == null)
            {
                throw new ArgumentNullException("news");
            }
            return news.ID;
        }

        /// <summary>
        /// Function used in Sort By Descending .. sorts by name
        /// </summary>
        private string NameSelector(SiteNews news)
        {
            if (news == null)
            {
                throw new ArgumentNullException("news");
            }
            return news.name;
        }

    }
}
