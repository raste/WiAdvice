// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessReport
    {
        /// <summary>
        /// Adds new report to database
        /// </summary>
        private void Add(EntitiesUsers userContext, Entities objectContext, Report newReport, BusinessLog bLog, User currUser)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            if (newReport == null)
            {
                throw new BusinessException("newReport is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            objectContext.AddToReportSet(newReport);
            Tools.Save(objectContext);
            bLog.LogReport(objectContext, newReport, LogType.create, string.Empty, currUser);

            BusinessStatistics businessStatistics = new BusinessStatistics();
            businessStatistics.ReportWritten(userContext, objectContext);
        }

        /// <summary>
        /// Returns Report with ID
        /// </summary>
        public Report Get(Entities objectContext, long reportID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (reportID < 1)
            {
                throw new BusinessException("reportID is < 1");
            }

            Report currReport = objectContext.ReportSet.FirstOrDefault(rep => rep.ID == reportID);
            return currReport;
        }

        /// <summary>
        /// Initialises some of the Fields on New report
        /// </summary>
        private void CreateReport(EntitiesUsers userContext, Entities objectContext, User currUser, BusinessLog blog, Report newReport)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (newReport == null)
            {
                throw new BusinessException("newReport is null");
            }
            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsFromUserTeam(currUser.ID))
            {
                throw new BusinessException(string.Format("User : {0}, ID : {1}, cannot create report  because he is not from the users group"
                    , currUser.username, currUser.ID));
            }

            newReport.dateCreated = DateTime.UtcNow;
            newReport.CreatedBy = Tools.GetUserID(objectContext, currUser);
            newReport.isResolved = false;
            newReport.isViewed = false;
            newReport.isDeletedByUser = false;

            Add(userContext, objectContext, newReport, blog, currUser);
        }

        /// <summary>
        /// Initialises aboutType and aboutTypeId fields of new Category Report
        /// </summary>
        private void CreateCategoyReport(EntitiesUsers userContext, Entities objectContext, User currUser
            , BusinessLog blog, Category currCategory, Report newReport)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }
            if (newReport == null)
            {
                throw new BusinessException("newReport is null");
            }

            newReport.aboutType = "category";
            newReport.aboutTypeId = currCategory.ID;
            newReport.aboutTypeParentId = null;

            CreateReport(userContext, objectContext, currUser, blog, newReport);
        }

        /// <summary>
        /// Creates new category irregular type report and add`s it to database
        /// </summary>
        public void CreateCategoryIrregularityReport(EntitiesUsers userContext, Entities objectContext
            , User currUser, BusinessLog blog, Category currCategory, String description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null or empty");
            }

            Report newReport = new Report();
            newReport.reportType = "irregularity";
            newReport.description = description;

            CreateCategoyReport(userContext, objectContext, currUser, blog, currCategory, newReport);
        }

        private void CreateProductLinkReport(EntitiesUsers userContext, Entities objectContext, User currUser
           , BusinessLog blog, ProductLink currLink, Report newReport)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currLink == null)
            {
                throw new BusinessException("currLink is null");
            }
            if (newReport == null)
            {
                throw new BusinessException("newReport is null");
            }

            newReport.aboutType = "productLink";
            newReport.aboutTypeId = currLink.ID;
            if (!currLink.UserReference.IsLoaded)
            {
                currLink.UserReference.Load();
            }
            newReport.aboutTypeParentId = currLink.User.ID;

            CreateReport(userContext, objectContext, currUser, blog, newReport);
        }


        private void CreateSuggestionReport(EntitiesUsers userContext, Entities objectContext, User currUser
            , BusinessLog blog, Suggestion currSuggestion, Report newReport)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currSuggestion == null)
            {
                throw new BusinessException("currSuggestion is null");
            }
            if (newReport == null)
            {
                throw new BusinessException("newReport is null");
            }

            newReport.aboutType = "suggestion";
            newReport.aboutTypeId = currSuggestion.ID;
            if (!currSuggestion.UserReference.IsLoaded)
            {
                currSuggestion.UserReference.Load();
            }
            newReport.aboutTypeParentId = currSuggestion.User.ID;

            CreateReport(userContext, objectContext, currUser, blog, newReport);
        }


        private void CreateCommentReport(EntitiesUsers userContext, Entities objectContext, User currUser
            , BusinessLog blog, Comment currComment, Report newReport, CommentType commType)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currComment == null)
            {
                throw new BusinessException("currComment is null");
            }
            if (newReport == null)
            {
                throw new BusinessException("newReport is null");
            }

            switch (commType)
            {
                case CommentType.Product:

                    newReport.aboutType = "comment";

                    break;
                case CommentType.Topic:

                    newReport.aboutType = "comment";

                    break;
                default:
                    throw new BusinessException(string.Format("Comment type = {0} is not supported when reporting for spam", commType));
            }

            newReport.aboutTypeId = currComment.ID;
            if (!currComment.UserIDReference.IsLoaded)
            {
                currComment.UserIDReference.Load();
            }
            newReport.aboutTypeParentId = currComment.UserID.ID;

            CreateReport(userContext, objectContext, currUser, blog, newReport);
        }

        /// <summary>
        /// Creates new Commment spam report and add`s it to database
        /// </summary>
        public void CreateCommentViolationReport(Entities objectContext, EntitiesUsers userContext, User currUser, BusinessLog blog
            , Comment currComment, CommentType commType)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currComment == null)
            {
                throw new BusinessException("currComment is null");
            }

            if (!currComment.UserIDReference.IsLoaded)
            {
                currComment.UserIDReference.Load();
            }

            Report newReport = new Report();
            newReport.reportType = "spam";

            BusinessUser businessUser = new BusinessUser();
            User mainUser = Tools.GetUserFromUserDatabase(currComment.UserID);

            switch (commType)
            {
                case CommentType.Product:

                    if (businessUser.IsGuest(userContext, mainUser))
                    {
                        newReport.description = string.Format("Automatic : {0} reported opinion ID: {1} in {2} ID = {3} for spam, written by guest",
                        currUser.username, currComment.ID, currComment.type, currComment.typeID);
                    }
                    else
                    {
                        newReport.description = string.Format("Automatic : {0} reported opinion ID: {1} in {2} ID = {3} for spam, written by '{4}' ID = {5}",
                        currUser.username, currComment.ID, currComment.type, currComment.typeID, mainUser.username, currComment.UserID.ID);
                    }

                    break;
                case CommentType.Topic:

                    newReport.description = string.Format("Automatic : {0} reported comment ID: {1} in {2} ID = {3} for spam, written by '{4}' ID = {5}",
                       currUser.username, currComment.ID, currComment.type, currComment.typeID, mainUser.username, currComment.UserID.ID);

                    break;
                default:
                    throw new BusinessException(string.Format("Comment type = {0} is not supported when reporting for spam", commType));
            }

            CreateCommentReport(userContext, objectContext, currUser, blog, currComment, newReport, commType);
        }


        public void CreateSuggestionViolationReport(EntitiesUsers userContext, Entities objectContext
            , User currUser, BusinessLog blog, Suggestion currSuggestion)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currSuggestion == null)
            {
                throw new BusinessException("currSuggestion is null");
            }

            if (!currSuggestion.UserReference.IsLoaded)
            {
                currSuggestion.UserReference.Load();
            }

            Report newReport = new Report();
            newReport.reportType = "spam";

            newReport.description = string.Format("Automatic : {0} reported suggestion ID: {1} for spam, written by '{2}' ID = {3}",
            currUser.username, currSuggestion.ID, Tools.GetUserFromUserDatabase(currSuggestion.User).username, currSuggestion.User.ID);

            CreateSuggestionReport(userContext, objectContext, currUser, blog, currSuggestion, newReport);
        }

        public void CreateProductViolationReport(EntitiesUsers userContext, Entities objectContext
          , User currUser, BusinessLog blog, ProductLink currLink)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currLink == null)
            {
                throw new BusinessException("currLink is null");
            }

            if (!currLink.UserReference.IsLoaded)
            {
                currLink.UserReference.Load();
            }
            if (currLink.ProductReference.IsLoaded)
            {
                currLink.ProductReference.Load();
            }

            Report newReport = new Report();
            newReport.reportType = "spam";

            newReport.description = string.Format
                ("Automatic : {0} reported product (\" {1} \" ID : {2}) link ID: {3}, URL : \" {4} \" for violation, written by '{5}' ID = {6}",
            currUser.username, currLink.Product.name, currLink.Product.ID, currLink.ID, currLink.link,
            Tools.GetUserFromUserDatabase(userContext, currLink.User).username, currLink.User.ID);

            CreateProductLinkReport(userContext, objectContext, currUser, blog, currLink, newReport);
        }

        private void CreateCompanyReport(EntitiesUsers userContext, Entities objectContext
            , User currUser, BusinessLog blog, Company currCompany, Report newReport)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (newReport == null)
            {
                throw new BusinessException("newReport is null");
            }

            newReport.aboutType = "company";
            newReport.aboutTypeId = currCompany.ID;
            newReport.aboutTypeParentId = null;

            CreateReport(userContext, objectContext, currUser, blog, newReport);
        }

        private void CreateProductTopicReport(EntitiesUsers userContext, Entities objectContext
            , User currUser, BusinessLog blog, ProductTopic currTopic, Report newReport)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currTopic == null)
            {
                throw new BusinessException("currTopic is null");
            }
            if (newReport == null)
            {
                throw new BusinessException("newReport is null");
            }

            newReport.aboutType = "productTopic";
            newReport.aboutTypeId = currTopic.ID;

            if (!currTopic.ProductReference.IsLoaded)
            {
                currTopic.ProductReference.Load();
            }

            newReport.aboutTypeParentId = currTopic.Product.ID;

            CreateReport(userContext, objectContext, currUser, blog, newReport);
        }


        /// <summary>
        /// Creates and Add`s to databes new Company Ireggular report
        /// </summary>
        public void CreateCompanyIrregularityReport(EntitiesUsers userContext, Entities objectContext
            , User currUser, BusinessLog blog, Company currCompany, String description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null or empty");
            }

            Report newReport = new Report();
            newReport.reportType = "irregularity";
            newReport.description = description;

            CreateCompanyReport(userContext, objectContext, currUser, blog, currCompany, newReport);
        }

        public void CreateProductTopicIrregularityReport(EntitiesUsers userContext, Entities objectContext
            , User currUser, BusinessLog blog, ProductTopic currTopic, String description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currTopic == null)
            {
                throw new BusinessException("currTopic is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null or empty");
            }

            Report newReport = new Report();
            newReport.reportType = "irregularity";
            newReport.description = description;

            CreateProductTopicReport(userContext, objectContext, currUser, blog, currTopic, newReport);
        }

        public void CreateTypeSuggestionIrregularityReport(EntitiesUsers userContext, Entities objectContext
            , User currUser, BusinessLog blog, TypeSuggestion currSuggestion, String description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currSuggestion == null)
            {
                throw new BusinessException("currSuggestion is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null or empty");
            }

            Report newReport = new Report();
            newReport.reportType = "irregularity";
            newReport.description = description;

            CreateTypeSuggestionReport(userContext, objectContext, currUser, blog, currSuggestion, newReport);

        }

        private void CreateTypeSuggestionReport(EntitiesUsers userContext, Entities objectContext, User currUser
            , BusinessLog blog, TypeSuggestion currSuggestion, Report newReport)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currSuggestion == null)
            {
                throw new BusinessException("currSuggestion is null");
            }
            if (newReport == null)
            {
                throw new BusinessException("newReport is null");
            }

            newReport.aboutType = "typeSuggestion";
            newReport.aboutTypeId = currSuggestion.ID;
            newReport.aboutTypeParentId = null;

            CreateReport(userContext, objectContext, currUser, blog, newReport);
        }

        private void CreateProductReport(EntitiesUsers userContext, Entities objectContext
            , User currUser, BusinessLog blog, Product currProduct, Report newReport)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }
            if (newReport == null)
            {
                throw new BusinessException("newReport is null");
            }

            newReport.aboutType = "product";
            newReport.aboutTypeId = currProduct.ID;
            if (!currProduct.CompanyReference.IsLoaded)
            {
                currProduct.CompanyReference.Load();
            }
            newReport.aboutTypeParentId = currProduct.Company.ID;

            CreateReport(userContext, objectContext, currUser, blog, newReport);
        }

        /// <summary>
        /// Creates and Add`s to databse new Product Ireggular report
        /// </summary>
        public void CreateProductIrregularityReport(EntitiesUsers userContext, Entities objectContext
            , User currUser, BusinessLog blog, Product currProduct, String description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null or empty");
            }

            Report newReport = new Report();
            newReport.reportType = "irregularity";
            newReport.description = description;

            CreateProductReport(userContext, objectContext, currUser, blog, currProduct, newReport);
        }

        public void CreateGeneralReport(EntitiesUsers userContext, Entities objectContext, User currUser, BusinessLog blog, String description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null or empty");
            }

            Report newReport = new Report();
            newReport.reportType = "irregularity";
            newReport.description = description;
            newReport.aboutType = "general";
            newReport.aboutTypeId = 1;
            newReport.aboutTypeParentId = null;

            CreateReport(userContext, objectContext, currUser, blog, newReport);
        }

        public void CreateReportAboutUser(EntitiesUsers userContext, Entities objectContext
            , User currUser, User aboutUser, BusinessLog blog, String description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(blog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (aboutUser == null)
            {
                throw new BusinessException("aboutUser is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null or empty");
            }

            Report newReport = new Report();
            newReport.reportType = "irregularity";
            newReport.description = description;
            newReport.aboutType = "user";
            newReport.aboutTypeId = aboutUser.ID;
            newReport.aboutTypeParentId = null;

            CreateReport(userContext, objectContext, currUser, blog, newReport);
        }


        /// <summary>
        /// Changes Report.isViewed to true
        /// </summary>
        public void ReportIsViewed(Entities objectContext, Report currReport, BusinessLog bLog, User currUser)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            if (currReport == null)
            {
                throw new BusinessException("currReport is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currReport.isViewed == false)
            {
                currReport.isViewed = true;
                Tools.Save(objectContext);
                bLog.LogReport(objectContext, currReport, LogType.edit, "isViewed", currUser);
            }
        }

        /// <summary>
        /// Changes Report.isDeletedByUser to true
        /// </summary>
        public void ReportIsDeletedByUser(Entities objectContext, EntitiesUsers userContext, Report currReport, BusinessLog bLog, User currUser)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(objectContext);

            if (currReport == null)
            {
                throw new BusinessException("currReport is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currReport.isDeletedByUser == false)
            {
                currReport.isDeletedByUser = true;
                Tools.Save(objectContext);
                bLog.LogReport(objectContext, currReport, LogType.edit, "isDeletedByUser", currUser);
                ReportIsResolved(objectContext, userContext, currReport, bLog, false, currUser, string.Empty);
            }
        }

        /// <summary>
        /// Changes Report.isResolved to true
        /// </summary>
        /// <param name="automaticMessage">Used only when isResolvedByAdmin = true, leave it empty if the comment is the standart one after admin is resolving report</param>
        public void ReportIsResolved(Entities objectContext, EntitiesUsers userContext, Report currReport, BusinessLog bLog
            , Boolean isResolvedByAdmin, User currUser, string automaticMessage)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (currReport == null)
            {
                throw new BusinessException("currReport is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currReport.isResolved == false)
            {
                if (isResolvedByAdmin)
                {
                    currReport.isViewed = true;
                    currReport.isResolved = true;
                }
                else
                {
                    currReport.isResolved = true;
                }

                Tools.Save(objectContext);
                bLog.LogReport(objectContext, currReport, LogType.edit, "isResolved", currUser);

                if (string.IsNullOrEmpty(automaticMessage))
                {
                    automaticMessage = Tools.GetResource("reportReolvedMessage");
                }

                if (currReport.reportType != "spam" && isResolvedByAdmin)
                {
                    BusinessUser businessUser = new BusinessUser();

                    if (string.IsNullOrEmpty(automaticMessage))
                    {
                        automaticMessage = Tools.GetResource("reportReolvedMessage");
                    }

                    CreateReportComment(objectContext, userContext, businessUser.GetSystem(userContext), currReport, automaticMessage, true, bLog);
                }
            }
        }

        /// <summary>
        /// To use only when comment is being deleted
        /// </summary>
        public void ResolveReportsForCommentWhichIsBeingDeleted(Entities objectContext, EntitiesUsers userContext, Comment comment, BusinessLog bLog, User currentUser)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }

            if (comment.visible == true)
            {
                throw new BusinessException(string.Format("Comment ID : {0} is not deleted, because of that reports for him cannot be resolved, User ID : {1}"
                    , comment.ID, currentUser.ID));
            }

            List<Report> commentReports = objectContext.ReportSet.Where(rep => rep.reportType == "spam" && rep.aboutType == "comment"
                && rep.aboutTypeId == comment.ID && rep.isResolved == false).ToList();

            if (commentReports.Count > 0)
            {
                foreach (Report commentReport in commentReports)
                {
                    ReportIsResolved(objectContext, userContext, commentReport, bLog, true, currentUser, string.Empty);
                }
            }
        }

        /// <summary>
        /// To use only when link is being deleted
        /// </summary>
        public void ResolveReportsForProductLinkWhichIsBeingDeleted(Entities objectContext, EntitiesUsers userContext
            , ProductLink link, BusinessLog bLog, User currentUser)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            if (link == null)
            {
                throw new BusinessException("link is null");
            }
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }

            if (link.visible == true)
            {
                return;
            }

            BusinessUser bUser = new BusinessUser();
            bool byAdmin = bUser.IsFromAdminTeam(currentUser);

            List<Report> linkReports = objectContext.ReportSet.Where(rep => rep.reportType == "spam" && rep.aboutType == "productLink"
                && rep.aboutTypeId == link.ID && rep.isResolved == false).ToList();

            if (linkReports.Count > 0)
            {
                foreach (Report linkReport in linkReports)
                {
                    ReportIsResolved(objectContext, userContext, linkReport, bLog, byAdmin, currentUser, string.Empty);
                }
            }
        }

        /// <summary>
        /// To use only when suggestion is being deleted
        /// </summary>>
        public void ResolveReportsForSuggestionWhichIsBeingDeleted(Entities objectContext, EntitiesUsers userContext, Suggestion suggestion, BusinessLog bLog, User currentUser)
        {
            Tools.AssertBusinessLogExists(bLog);
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
            if (suggestion.visible == true)
            {
                throw new BusinessException(string.Format("Suggestion ID : {0} is not deleted, because of that reports for him cannot be resolved, User ID : {1}"
                    , suggestion.ID, currentUser.ID));
            }


            List<Report> suggestionReports = objectContext.ReportSet.Where(rep => rep.reportType == "spam" && rep.aboutType == "suggestion"
                && rep.aboutTypeId == suggestion.ID && rep.isResolved == false).ToList();

            if (suggestionReports.Count > 0)
            {
                foreach (Report suggestionReport in suggestionReports)
                {
                    ReportIsResolved(objectContext, userContext, suggestionReport, bLog, true, currentUser, string.Empty);
                }
            }
        }

        /// <summary>
        /// Resolves all not-resolved user reports, and sends automatic comment.. Used when the user is being deleted.
        /// </summary>
        public void ResolveAllUserReportsWhenItsBeingDeleted(Entities objectContext, EntitiesUsers userContext, BusinessLog bLog, User userDeleted)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (userDeleted == null)
            {
                throw new BusinessException("userDeleted is null");
            }

            UserID userID = Tools.GetUserID(objectContext, userDeleted);

            userID.ReportsCreated.Load();

            List<Report> activeReports = userID.ReportsCreated.Where(rep => rep.isResolved == false).ToList();

            if (activeReports != null && activeReports.Count > 0)
            {
                string description = Tools.GetResource("repResCusUserDeleted");

                foreach (Report activeReport in activeReports)
                {
                    ReportIsResolved(objectContext, userContext, activeReport, bLog, true, userDeleted, description);
                }
            }
        }

        private List<Report> SortByType(Entities objectContext, String aboutType, long typeID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(aboutType))
            {
                throw new BusinessException("aboutType is null or empty");
            }
            if (typeID < 1)
            {
                throw new BusinessException("invalid typeID");
            }

            IEnumerable<Report> reports = null;

            switch (aboutType)
            {
                case ("company"):
                    reports = objectContext.ReportSet.Where
                        (rep => rep.aboutType == aboutType && rep.aboutTypeId == typeID);
                    break;
                case ("product"):
                    reports = objectContext.ReportSet.Where
                        (rep => rep.aboutType == aboutType && rep.aboutTypeId == typeID);
                    break;
                case ("productTopic"):
                    reports = objectContext.ReportSet.Where
                        (rep => rep.aboutType == aboutType && rep.aboutTypeId == typeID);
                    break;
                case ("productLink"):
                    reports = objectContext.ReportSet.Where
                        (rep => rep.aboutType == aboutType && rep.aboutTypeId == typeID);
                    break;
                case ("category"):
                    reports = objectContext.ReportSet.Where
                        (rep => rep.aboutType == aboutType && rep.aboutTypeId == typeID);
                    break;
                case ("suggestion"):
                    reports = objectContext.ReportSet.Where
                        (rep => rep.aboutType == aboutType && rep.aboutTypeId == typeID);
                    break;
                case ("user"):
                    reports = objectContext.ReportSet.Where
                        (rep => rep.aboutType == aboutType && rep.aboutTypeId == typeID);
                    break;
                case ("typeSuggestion"):
                    reports = objectContext.ReportSet.Where
                        (rep => rep.aboutType == aboutType && rep.aboutTypeId == typeID);
                    break;
                case ("all"):
                    reports = objectContext.ReportSet;
                    break;
                case ("aCompanies"):
                    reports = objectContext.ReportSet.Where(rep => rep.aboutType == "company");
                    break;
                case ("aProducts"):
                    reports = objectContext.ReportSet.Where(rep => rep.aboutType == "product");
                    break;
                case ("aProductTopics"):
                    reports = objectContext.ReportSet.Where(rep => rep.aboutType == "productTopic");
                    break;
                case ("aProductLinks"):
                    reports = objectContext.ReportSet.Where(rep => rep.aboutType == "productLink");
                    break;
                case ("aCategories"):
                    reports = objectContext.ReportSet.Where(rep => rep.aboutType == "category");
                    break;
                case ("aComments"):
                    reports = objectContext.ReportSet.Where(rep => rep.aboutType == "comment");
                    break;
                case ("aSuggestions"):
                    reports = objectContext.ReportSet.Where(rep => rep.aboutType == "suggestion");
                    break;
                case ("aUsers"):
                    reports = objectContext.ReportSet.Where(rep => rep.aboutType == "user");
                    break;
                case ("aTypeSuggestions"):
                    reports = objectContext.ReportSet.Where(rep => rep.aboutType == "typeSuggestion");
                    break;
                case ("general"):
                    reports = objectContext.ReportSet.Where(rep => rep.aboutType == "general");
                    break;
                default:
                    String error = string.Format("aboutType = " + aboutType + " is not valid type.");
                    throw new BusinessException(error);
            }

            List<Report> reportsList = reports.ToList();
            reportsList.Reverse();

            return reportsList;
        }

        private List<Report> SortByNumber(List<Report> reportList, long number)
        {
            if (number < 1)
            {
                throw new BusinessException("number is < 1");
            }

            long count = reportList.Count<Report>();
            if (count > 0)
            {
                if (number >= count)
                {
                    return reportList;
                }
                else
                {
                    List<Report> SortedList = new List<Report>();
                    for (int i = 0; i < number; i++)
                    {
                        SortedList.Add(reportList[i]);
                    }
                    return SortedList;
                }

            }
            else
            {
                return reportList;
            }
        }

        private List<Report> SortByReportType(List<Report> reportList, String reportType)
        {
            if (string.IsNullOrEmpty(reportType))
            {
                throw new BusinessException("reportType is null or empty");
            }

            long count = reportList.Count<Report>();

            if (count > 0)
            {
                List<Report> SortedList = new List<Report>();

                switch (reportType)
                {
                    case ("spam"):
                        foreach (Report report in reportList)
                        {
                            if (report.reportType == reportType)
                            {
                                SortedList.Add(report);
                            }
                        }
                        break;
                    case ("irregularity"):
                        foreach (Report report in reportList)
                        {
                            if (report.reportType == reportType)
                            {
                                SortedList.Add(report);
                            }
                        }
                        break;
                    case ("all"):
                        SortedList = reportList;
                        break;
                    default:
                        String error = string.Format("reportType = " + reportType + " is not valid type");
                        throw new BusinessException(error);
                }

                return SortedList;

            }
            else
            {
                return reportList;
            }

        }

        private List<Report> SortByViewedMode(List<Report> reportList, String viewedMode)
        {
            if (string.IsNullOrEmpty(viewedMode))
            {
                throw new BusinessException("viewedMode is null or empty");
            }

            long count = reportList.Count<Report>();
            if (count > 0)
            {
                List<Report> SortedList = new List<Report>();

                switch (viewedMode)
                {
                    case ("notViewed"):
                        foreach (Report report in reportList)
                        {
                            if (report.isViewed == false)
                            {
                                SortedList.Add(report);
                            }
                        }
                        break;
                    case ("Viewed"):
                        foreach (Report report in reportList)
                        {
                            if (report.isViewed && (report.isResolved == false))
                            {
                                SortedList.Add(report);
                            }
                        }
                        break;
                    case ("Resolved"):
                        foreach (Report report in reportList)
                        {
                            if (report.isResolved)
                            {
                                SortedList.Add(report);
                            }
                        }
                        break;
                    case ("notResolved"):
                        foreach (Report report in reportList)
                        {
                            if (!report.isResolved)
                            {
                                SortedList.Add(report);
                            }
                        }
                        break;
                    case ("all"):
                        SortedList = reportList;
                        break;
                    default:
                        string error = string.Format("viewedMode = " + viewedMode + " is not valid mode.");
                        throw new BusinessException(error);
                }

                return SortedList;

            }
            else
            {
                return reportList;
            }
        }


        public void ResolveAllSpamReportsForSameType(Entities objectContext, EntitiesUsers userContext, Report currReport, BusinessLog bLog, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currReport == null)
            {
                throw new BusinessException("currReport is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            IEnumerable<Report> reports = objectContext.ReportSet.Where(rep => rep.reportType == "spam" &&
                  rep.aboutTypeId == currReport.aboutTypeId && rep.aboutType == currReport.aboutType
                  && rep.isResolved == false);

            if (reports.Count<Report>() > 0)
            {
                List<Report> lReports = reports.ToList();
                foreach (Report report in lReports)
                {
                    ReportIsResolved(objectContext, userContext, report, bLog, true, currUser, string.Empty);
                }
            }

        }

        /// <summary>
        /// Get Reports sorted by params
        /// </summary>
        public List<Report> GetReports(Entities objectContext, String aboutType, long typeID, String reportType, String viewedMode, long number)
        {
            List<Report> Reports = new List<Report>();
            if (AreMembersValid(objectContext, aboutType, typeID, reportType, viewedMode, number))
            {
                /// SortByType
                Reports = SortByType(objectContext, aboutType, typeID);

                /// SortByReportType
                Reports = SortByReportType(Reports, reportType);

                /// SortByViewedMode
                Reports = SortByViewedMode(Reports, viewedMode);

                /// SortByNumber
                Reports = SortByNumber(Reports, number);
            }
            return Reports;
        }

        /// <summary>
        /// Returns True if User have reports that are not deleted from his reports box, otherwise false
        /// </summary>
        public Boolean CheckIfUserHaveNotDeletedReports(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            Boolean result = false;

            if (objectContext.ReportSet.Count(rep => rep.CreatedBy.ID == currUser.ID && rep.isDeletedByUser == false) > 0)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks if report (user, type, aboutType, aboutTypeID) parameters are correct, No error message if aboutType or aboutTypeID are wrong. Report description is not checked.
        /// </summary>
        /// <param name="reportType">irregularity, spam</param>
        /// <param name="type">category, company, product, productLink, user, typeSuggestion, general; comment, suggestion</param>
        /// <param name="typeID">1 if its general</param>
        public bool CheckIfUserCanSendReportAboutType(Entities objectContext, EntitiesUsers userContext, User currentUser, string reportType
            , string type, long typeID, out string error)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            error = string.Empty;
            bool passed = true;

            if (string.IsNullOrEmpty(reportType))
            {
                throw new BusinessException("reportType is empty");
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new BusinessException("type is empty");
            }

            BusinessUser bUser = new BusinessUser();

            if (currentUser == null || !bUser.CanUserDo(userContext, currentUser, UserRoles.ReportInappropriate))
            {
                error = Tools.GetResource("errCantSendReport").ToString();
                return false;
            }

            if (reportType != "irregularity" && reportType != "spam")
            {
                throw new BusinessException(string.Format("report type = {0} is not supported type", reportType));
            }

            if (reportType == "irregularity")
            {
                if (IsMaxActiveIrregularityReportsReached(objectContext, currentUser) == true)
                {
                    error = Tools.GetResource("errMaxIrrRepLimitReached").ToString();
                    return false;
                }
            }
            else
            {
                if (IsMaxActiveSpamReportsReached(objectContext, currentUser) == true)
                {
                    error = Tools.GetResource("errMaxSpamReached").ToString();
                    return false;
                }
            }

            if (reportType != "irregularity" || type != "general")   // CHecks if there is already active report for same type from current user
            {
                Report activeReportForType = objectContext.ReportSet.FirstOrDefault(rep => rep.CreatedBy.ID == currentUser.ID
                    && rep.reportType == reportType && rep.aboutType == type && rep.aboutTypeId == typeID
                    && rep.isResolved == false && rep.isDeletedByUser == false);

                if (activeReportForType != null)
                {
                    error = Tools.GetResource("errRepAlreadyHaveSuchActive").ToString();
                    return false;
                }
            }

            // Checks if report about type and about type ID are correct
            if (reportType == "irregularity")
            {
                switch (type)
                {
                    case "category":

                        BusinessCategory businessCategory = new BusinessCategory();

                        Category currCategory = businessCategory.Get(objectContext, typeID);
                        if (currCategory == null)
                        {
                            passed = false;
                        }
                        else
                        {
                            if (currCategory.last == false || currCategory.visible == false)
                            {
                                passed = false;
                            }
                        }

                        break;
                    case "product":

                        BusinessProduct businessProduct = new BusinessProduct();

                        Product currProduct = businessProduct.GetProductByID(objectContext, typeID);
                        if (currProduct == null)
                        {
                            passed = false;
                        }
                        else
                        {
                            if (!businessProduct.CheckIfProductsIsValidWithConnections(objectContext, currProduct, out error))
                            {
                                error = string.Empty;
                                passed = false;
                            }
                        }

                        break;
                    case "user":

                        User userToReport = bUser.Get(userContext, typeID, false);
                        if (userToReport == null)
                        {
                            passed = false;
                        }
                        else
                        {
                            if (userToReport.visible == false)
                            {
                                passed = false;
                            }

                            if (userToReport == currentUser)
                            {
                                passed = false;
                            }

                            if (!userToReport.UserOptionsReference.IsLoaded)
                            {
                                userToReport.UserOptionsReference.Load();
                            }
                            if (userToReport.UserOptions.activated == false)
                            {
                                passed = false;
                            }

                            if (!bUser.IsFromUserTeam(userToReport))
                            {
                                passed = false;
                            }
                        }


                        break;
                    case "company":

                        BusinessCompany businessCompany = new BusinessCompany();

                        Company currCompany = businessCompany.GetCompany(objectContext, typeID);
                        if (currCompany == null)
                        {
                            passed = false;
                        }
                        else
                        {
                            if (currCompany.visible == false)
                            {
                                passed = false;
                            }
                        }

                        break;

                    case "typeSuggestion":

                        BusinessTypeSuggestions btSuggestion = new BusinessTypeSuggestions();
                        TypeSuggestion currSuggestion = btSuggestion.GetSuggestion(objectContext, typeID, false, false);
                        if (currSuggestion == null)
                        {
                            passed = false;
                        }
                        else
                        {
                            if (btSuggestion.CanUserReportTypeSuggestion(currentUser, currSuggestion) == false)
                            {
                                passed = false;
                            }
                        }
                        break;

                    case "productTopic":

                        BusinessProductTopics bpTopic = new BusinessProductTopics();
                        ProductTopic topic = bpTopic.Get(objectContext, typeID, true, false);
                        if (topic == null || topic.visible == false)
                        {
                            passed = false;
                        }
                        else
                        {

                            BusinessProduct bProduct = new BusinessProduct();
                            if (!topic.ProductReference.IsLoaded)
                            {
                                topic.ProductReference.Load();
                            }

                            if (!bProduct.CheckIfProductsIsValidWithConnections(objectContext, topic.Product, out error))
                            {
                                passed = false;
                                error = string.Empty;
                            }
                        }

                        break;

                    case "general":
                        break;

                    default:
                        throw new BusinessException(string.Format("Irregularity report about type = '{0}' is not supported type, user id = {1}",
                            type, currentUser.ID));
                }
            }
            else
            {
                switch (type)
                {
                    case "comment":

                        BusinessComment bComment = new BusinessComment();

                        Comment comment = bComment.Get(objectContext, typeID);
                        if (comment == null)
                        {
                            passed = false;
                        }
                        else
                        {
                            if (comment.visible == false)
                            {
                                passed = false;
                            }
                            else
                            {
                                if (!comment.UserIDReference.IsLoaded)
                                {
                                    comment.UserIDReference.Load();
                                }
                                if (comment.UserID.ID != currentUser.ID)
                                {
                                    User commentUser = bUser.GetWithoutVisible(userContext, comment.UserID.ID, true);

                                    if (bUser.IsFromAdminTeam(commentUser) == true)
                                    {
                                        passed = false;
                                    }
                                }
                            }
                        }

                        break;


                    case "suggestion":

                        BusinessSuggestion bSuggestion = new BusinessSuggestion();

                        Suggestion suggestion = bSuggestion.Get(objectContext, typeID);

                        if (suggestion == null)
                        {
                            passed = false;
                        }
                        else
                        {
                            if (suggestion.visible == false)
                            {
                                passed = false;
                            }
                            else
                            {
                                if (!suggestion.UserReference.IsLoaded)
                                {
                                    suggestion.UserReference.Load();
                                }

                                if (currentUser.ID != suggestion.User.ID)
                                {
                                    User suggUser = bUser.GetWithoutVisible(userContext, suggestion.User.ID, true);

                                    if (bUser.IsFromAdminTeam(suggUser) == true)
                                    {
                                        passed = false;
                                    }
                                }
                            }
                        }

                        break;

                    case "productLink":

                        BusinessProductLink bLink = new BusinessProductLink();

                        ProductLink link = bLink.Get(objectContext, typeID, true, false);

                        if (link == null)
                        {
                            passed = false;
                        }
                        else
                        {
                            if (!link.ProductReference.IsLoaded)
                            {
                                link.ProductReference.Load();
                            }

                            if (link.Product.visible == false)
                            {
                                passed = false;
                            }
                        }

                        break;

                    default:
                        throw new BusinessException(string.Format("Spam report about type = '{0}' is not supported type, user id = {1}",
                           type, currentUser.ID));
                }
            }

            if (passed == false && string.IsNullOrEmpty(error))
            {
                error = Tools.GetResource("errReportInvalidType").ToString();
            }

            return passed;
        }

        /// <summary>
        /// Returns User reports with options
        /// </summary>
        /// <param name="withSpamReports">true if it should return and spam reports , otherwise false</param>
        /// <param name="evenDeletedByUser">true if it should return those who are deleted by user, otherwise false</param>
        /// <returns></returns>
        public IEnumerable<Report> GetUserReports(Entities objectContext, User currUser, Boolean withSpamReports, Boolean evenDeletedByUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            UserID userID = Tools.GetUserID(objectContext, currUser);

            userID.ReportsCreated.Load();

            IEnumerable<Report> userReports = null;

            if (withSpamReports)
            {
                if (evenDeletedByUser)
                {
                    userReports = userID.ReportsCreated;
                }
                else
                {
                    userReports = userID.ReportsCreated.Where(rep => rep.isDeletedByUser == false);
                }
            }
            else
            {
                if (evenDeletedByUser)
                {
                    userReports = userID.ReportsCreated.Where(rep => rep.reportType != "spam");
                }
                else
                {
                    userReports = userID.ReportsCreated.Where(rep => rep.reportType != "spam" && rep.isDeletedByUser == false);
                }

            }

            return userReports;
        }


        private Boolean AreMembersValid(Entities objectContext, String aboutType, long typeID, String reportType, String viewedMode, long number)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(aboutType))
            {
                throw new BusinessException("aboutType is null or empty");
            }
            else
            {
                String[] aboutTypes = { "company", "product", "productTopic", "category", "suggestion", "all", "aCategories", "user", "typeSuggestion"
                                          , "aProducts", "aProductTopics", "aCompanies", "aComments", "general", "aSuggestions", "aUsers", "aTypeSuggestions"
                                      , "productLink", "aProductLinks"};

                Boolean pass = false;

                for (int i = 0; i < aboutTypes.Length; i++)
                {
                    if (aboutTypes[i] == aboutType)
                    {
                        pass = true;
                        break;
                    }
                }

                if (!pass)
                {
                    String error = string.Format("aboutType = {0} is not a valid type", aboutType);
                    throw new BusinessException(error);
                }
            }

            if (typeID < 1)
            {
                throw new BusinessException("invalid typeID");
            }

            if (string.IsNullOrEmpty(reportType))
            {
                throw new BusinessException("reportType is null or empty");
            }
            else
            {
                String[] reportTypes = { "spam", "irregularity", "all" };
                Boolean pass = false;

                for (int i = 0; i < reportTypes.Length; i++)
                {
                    if (reportTypes[i] == reportType)
                    {
                        pass = true;
                        break;
                    }
                }
                if (!pass)
                {
                    String error = string.Format("reportType = " + reportType + " is not a valid type");
                    throw new BusinessException(error);
                }
            }

            if (string.IsNullOrEmpty(viewedMode))
            {
                throw new BusinessException("viewedMode is null or empty");
            }
            else
            {
                String[] viewedModes = { "notViewed", "Viewed", "Resolved", "all", "notResolved" };
                Boolean pass = false;
                for (int i = 0; i < viewedModes.Length; i++)
                {
                    if (viewedModes[i] == viewedMode)
                    {
                        pass = true;
                        break;
                    }
                }
                if (!pass)
                {
                    String error = string.Format("viewedMode = " + viewedMode + " is not a valid type");
                    throw new BusinessException(error);
                }
            }

            if (number < 1)
            {
                throw new BusinessException("number is <1");
            }

            return true;
        }



        private long IdSelector(Report report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }
            return report.ID;
        }

        /// <summary>
        /// True if userToCheck NOT resolved iiregilarity reports are more or equal to maximum limit
        /// , otherwise false
        /// </summary>
        public bool IsMaxActiveIrregularityReportsReached(Entities objectContext, User userToCheck)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (userToCheck == null)
            {
                throw new BusinessException("userToCheck is null");
            }

            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsFromUserTeam(userToCheck))
            {
                throw new BusinessException(string.Format("{0} , ID = {1} , cannot send reports because he is not user."
                    , userToCheck.username, userToCheck.ID));
            }

            int count = objectContext.ReportSet.Count(rep => rep.CreatedBy.ID == userToCheck.ID &&
                rep.reportType == "irregularity" && rep.isResolved == false);

            bool result = true;

            if (count < Configuration.ReportsNumMaxNotResolvedIrregularity)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Returns number of remaining irregularity reports which user can send
        /// </summary>
        public int NumberOfReportsWhichUserCanSend(Entities objectContext, User userToCheck)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (userToCheck == null)
            {
                throw new BusinessException("userToCheck is null");
            }

            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsFromUserTeam(userToCheck))
            {
                throw new BusinessException(string.Format("{0} , ID = {1} , cannot send reports because he is not user."
                    , userToCheck.username, userToCheck.ID));
            }

            int count = objectContext.ReportSet.Count(rep => rep.CreatedBy.ID == userToCheck.ID &&
               rep.reportType == "irregularity" && rep.isResolved == false);

            if (count < Configuration.ReportsNumMaxNotResolvedIrregularity)
            {
                return (Configuration.ReportsNumMaxNotResolvedIrregularity - count);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns number of remaining spam reports which user can send
        /// </summary>
        public int NumberOfSpamReportsWhichUserCanSend(Entities objectContext, User userToCheck)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (userToCheck == null)
            {
                throw new BusinessException("userToCheck is null");
            }

            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsFromUserTeam(userToCheck))
            {
                throw new BusinessException(string.Format("{0} , ID = {1} , cannot send reports because he is not user."
                    , userToCheck.username, userToCheck.ID));
            }

            int count = objectContext.ReportSet.Count(rep => rep.CreatedBy.ID == userToCheck.ID &&
               rep.reportType == "spam" && rep.isResolved == false);

            if (count < Configuration.ReportsNumMaxNotResolvedSpam)
            {
                return (Configuration.ReportsNumMaxNotResolvedSpam - count);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// True if userToCheck NOT resolved spam reports are more or equal to maximum limit
        /// , otherwise false
        /// </summary>
        public bool IsMaxActiveSpamReportsReached(Entities objectContext, User userToCheck)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (userToCheck == null)
            {
                throw new BusinessException("userToCheck is null");
            }

            int count = objectContext.ReportSet.Count(rep => rep.CreatedBy.ID == userToCheck.ID &&
                rep.reportType == "spam" && rep.isResolved == false);

            bool result = true;

            if (count < Configuration.ReportsNumMaxNotResolvedSpam)
            {
                result = false;
            }

            return result;
        }


        /// <summary>
        /// Add`s new Comment which is about Report
        /// </summary>
        /// <param name="reportID">ID of the report which comment is about</param>
        public void CreateReportComment(Entities objectContext, EntitiesUsers userContext, User currUser, Report report
            , String description, bool fromAdmin, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (report == null)
            {
                throw new BusinessException("report is null");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is null or empty");
            }

            ReportComment newComment = new ReportComment();

            newComment.User = Tools.GetUserID(objectContext, currUser);
            newComment.Report = report;
            newComment.dateCreated = DateTime.UtcNow;
            newComment.description = description;
            newComment.visible = true;
            newComment.lastModified = newComment.dateCreated;
            newComment.lastModifiedBy = newComment.User;

            objectContext.AddToReportCommentSet(newComment);
            Tools.Save(objectContext);

            if (fromAdmin == true)
            {

                ReportIsViewed(objectContext, report, bLog, currUser);

                if (!report.CreatedByReference.IsLoaded)
                {
                    report.CreatedByReference.Load();
                }

                BusinessUser bUser = new BusinessUser();
                User reportOwner = bUser.GetWithoutVisible(userContext, report.CreatedBy.ID, true);

                BusinessUserOptions bUserOptions = new BusinessUserOptions();
                bUserOptions.ChangeIfUserHaveNewReportReply(userContext, reportOwner, true);
            }
        }

        /// <summary>
        /// Returns all comments about Report
        /// </summary>
        public List<ReportComment> GetReportComments(Entities objectContext, Report currReport)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currReport == null)
            {
                throw new BusinessException("currReport is null");
            }

            IEnumerable<ReportComment> comments = objectContext.ReportCommentSet.Where
                (comm => comm.Report.ID == currReport.ID && comm.visible == true);

            return comments.ToList();
        }

        public int CountReportComments(Entities objectContext, Report currReport)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currReport == null)
            {
                throw new BusinessException("currReport is null");
            }

            int comments = objectContext.ReportCommentSet.Count
                (comm => comm.Report.ID == currReport.ID && comm.visible == true);

            return comments;
        }

        public static string GetReportAboutType(Report report)
        {
            if (report == null)
            {
                throw new BusinessException("report is null");
            }

            string result = string.Empty;

            switch (report.aboutType)
            {
                case "category":
                    result = Tools.GetResource("category");
                    break;
                case "company":
                    result = Tools.GetResource("maker");
                    break;
                case "product":
                    result = Tools.GetResource("product");
                    break;
                case "comment":
                    result = Tools.GetResource("comment");
                    break;
                case "user":
                    result = Tools.GetResource("user");
                    break;
                case "typeSuggestion":
                    break;
                case "productTopic":
                    result = Tools.GetResource("topic");
                    break;
                case "general":
                    result = Tools.GetResource("general");
                    break;
                default:
                    throw new BusinessException(string.Format("Report id : {0} type = {1} is not supported", report.ID, report.reportType));
            }

            return result;
        }
    }
}
