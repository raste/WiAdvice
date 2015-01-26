// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessStatistics
    {
        private static object updateStatistic = new object();

        private Statistic GetLastRecord(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            Statistic currStatistic = objectContext.StatisticSet.OrderByDescending<Statistic, long>(
                new Func<Statistic, long>(IdSelector)).FirstOrDefault<Statistic>();

            return currStatistic;
        }

        private long IdSelector(Statistic statistic)
        {
            if (statistic == null)
            {
                throw new ArgumentNullException("statistic");
            }
            return statistic.ID;
        }

        /// <summary>
        /// Returns statistic from specified Date
        /// </summary>
        public Statistic Get(Entities objectContext, DateTime date)
        {
            Tools.AssertObjectContextExists(objectContext);

            DateTime dateOnly = date.Date;
            DateTime nextDate = dateOnly.AddDays(1);
            Statistic currStatistic = objectContext.StatisticSet.FirstOrDefault<Statistic>
                (stat => (stat.forDate >= dateOnly) && (stat.forDate < nextDate));

            return currStatistic;
        }

        /// <summary>
        /// Creates Ands Add`s statistic for current day
        /// </summary>
        /// <param name="currDate">The date to create a statistics record for.</param>
        private void CreateStatistic(EntitiesUsers userContext, Entities objectContext, DateTime currDate)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            Statistic lastRecord = GetLastRecord(objectContext);
            if (lastRecord != null)
            {
                if (currDate.Date.Equals(lastRecord.forDate.Date))
                {
                    throw new BusinessException("there is already record for today");
                }
            }

            Statistic newStatistic = new Statistic();
            newStatistic.forDate = currDate;
            newStatistic.sessionsStarted = 0;
            newStatistic.usersLogged = 0;
            newStatistic.usersLoggedOut = 0;
            newStatistic.usersRegistered = 0;
            newStatistic.companiesCreated = 0;
            newStatistic.productsCreated = 0;
            newStatistic.commentsWritten = 0;
            newStatistic.commentsDeleted = 0;
            newStatistic.reportsWritten = 0;
            newStatistic.picturesUploaded = 0;
            newStatistic.picturesDeleted = 0;
            newStatistic.companiesDeleted = 0;
            newStatistic.productsDeleted = 0;
            newStatistic.categoriesCreated = 0;
            newStatistic.categoriesDeleted = 0;
            newStatistic.usersDeleted = 0;
            newStatistic.adminsRegistered = 0;
            newStatistic.adminsDeleted = 0;
            newStatistic.prodCharsCreated = 0;
            newStatistic.prodCharsDeleted = 0;
            newStatistic.compCharsCreated = 0;
            newStatistic.compCharsDeleted = 0;
            newStatistic.topicsCreated = 0;
            newStatistic.topicsDeleted = 0;

            objectContext.AddToStatisticSet(newStatistic);

            Tools.Save(objectContext);

            BusinessAdvertisement businessAdvert = new BusinessAdvertisement();
            businessAdvert.ScriptCheckAdvertsExpirationDate(objectContext);

            BusinessUserOptions businessUserOptions = new BusinessUserOptions();
            businessUserOptions.ScriptCheckIfThereAreUnActivatedUsersForMoreThan24Hours(userContext);

            BusinessIpBans businessIpBans = new BusinessIpBans();
            businessIpBans.ScriptCheckBansActiveUntillDate(userContext, objectContext, new BusinessLog(-1));

            BusinessTransferAction bTransfer = new BusinessTransferAction();
            bTransfer.ScriptCheckForTransfersExpireTime(objectContext, new BusinessLog(-1));

            BusinessIpAttempts bIpAttempts = new BusinessIpAttempts();
            bIpAttempts.DeleteOldIpAttempts(userContext);

            BusinessTypeSuggestions btSuggestions = new BusinessTypeSuggestions();
            btSuggestions.ScriptCheckForExpiredSuggestions(objectContext, userContext, new BusinessLog(-1));
        }

        /// <summary>
        /// Uincreases with 1 , field from current day`s statistic
        /// </summary>
        private void UpdateStatistic(EntitiesUsers userContext, Entities objectContext, string field)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);

            if (field == null || field.Length < 1)
            {
                throw new BusinessException("field is null or empty");
            }

            lock (updateStatistic)
            {
                DateTime dtUtcNow = DateTime.UtcNow;
                Statistic currStatistic = Get(objectContext, dtUtcNow);
                if (currStatistic == null)
                {
                    CreateStatistic(userContext, objectContext, dtUtcNow);
                    currStatistic = Get(objectContext, dtUtcNow);
                    if (currStatistic == null)
                    {
                        throw new BusinessException("statistic for current day didnt create ot parameters are wrong");
                    }
                }

                switch (field)
                {
                    case ("companiesCreated"):
                        currStatistic.companiesCreated += 1;
                        break;
                    case ("productsCreated"):
                        currStatistic.productsCreated += 1;
                        break;
                    case ("commentsWritten"):
                        currStatistic.commentsWritten += 1;
                        break;
                    case ("commentsDeleted"):
                        currStatistic.commentsDeleted += 1;
                        break;
                    case ("reportsWritten"):
                        currStatistic.reportsWritten += 1;
                        break;
                    case ("picturesUploaded"):
                        currStatistic.picturesUploaded += 1;
                        break;
                    case ("picturesDeleted"):
                        currStatistic.picturesDeleted += 1;
                        break;
                    case ("companiesDeleted"):
                        currStatistic.companiesDeleted += 1;
                        break;
                    case ("productsDeleted"):
                        currStatistic.productsDeleted += 1;
                        break;
                    case ("categoriesCreated"):
                        currStatistic.categoriesCreated += 1;
                        break;
                    case ("categoriesDeleted"):
                        currStatistic.categoriesDeleted += 1;
                        break;
                    case ("prodCharsCreated"):
                        currStatistic.prodCharsCreated += 1;
                        break;
                    case ("prodCharsDeleted"):
                        currStatistic.prodCharsDeleted += 1;
                        break;
                    case ("compCharsCreated"):
                        currStatistic.compCharsCreated += 1;
                        break;
                    case ("compCharsDeleted"):
                        currStatistic.compCharsDeleted += 1;
                        break;
                    case ("topicsCreated"):
                        currStatistic.topicsCreated += 1;
                        break;
                    case ("topicsDeleted"):
                        currStatistic.topicsDeleted += 1;
                        break;
                    default:
                        throw new BusinessException(string.Format("field = {0} is not supported", field));

                }
                Tools.Save(objectContext);
            }
        }


        /// <summary>
        /// Updates statistic in all local databases including current
        /// </summary>
        private void UpdateStatisticInAllDatabases(EntitiesUsers userContext, string field)
        {
            Tools.AssertObjectContextExists(userContext);

            if (field == null || field.Length < 1)
            {
                throw new BusinessException("field is null or empty");
            }

            StringCollection contextStringCollection = Configuration.ApplicationVariantConnectionStrings(true);
            if (contextStringCollection.Count > 0)
            {
                lock (updateStatistic)
                {
                    foreach (string entityString in contextStringCollection)
                    {
                        Entities localContext = new Entities(entityString);

                        DateTime dtUtcNow = DateTime.UtcNow;
                        Statistic currStatistic = Get(localContext, dtUtcNow);
                        if (currStatistic == null)
                        {
                            CreateStatistic(userContext, localContext, dtUtcNow);
                            currStatistic = Get(localContext, dtUtcNow);
                            if (currStatistic == null)
                            {
                                throw new BusinessException("statistic for current day didnt create ot parameters are wrong");
                            }
                        }

                        switch (field)
                        {
                            case ("usersLogged"):
                                currStatistic.usersLogged += 1;
                                break;
                            case ("usersLoggedOut"):
                                currStatistic.usersLoggedOut += 1;
                                break;
                            case ("usersRegistered"):
                                currStatistic.usersRegistered += 1;
                                break;
                            case ("usersDeleted"):
                                currStatistic.usersDeleted += 1;
                                break;
                            case ("adminsRegistered"):
                                currStatistic.adminsRegistered += 1;
                                break;
                            case ("adminsDeleted"):
                                currStatistic.adminsDeleted += 1;
                                break;
                            case ("sessionsStarted"):
                                currStatistic.sessionsStarted += 1;
                                break;
                            default:
                                throw new BusinessException(string.Format("field = {0} is not supported", field));

                        }

                        Tools.Save(localContext);

                    }
                }
            }
        }

        public void CompCharDeleted(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "compCharsDeleted");
        }

        public void CompCharCreated(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "compCharsCreated");
        }

        public void ProdCharDeleted(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "prodCharsDeleted");
        }

        public void ProdCharCreated(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "prodCharsCreated");
        }

        public void AdminDeleted(EntitiesUsers userContext)
        {
            UpdateStatisticInAllDatabases(userContext, "adminsDeleted");
        }

        public void AdminRegistered(EntitiesUsers userContext)
        {
            UpdateStatisticInAllDatabases(userContext, "adminsRegistered");
        }

        public void UserDeleted(EntitiesUsers userContext)
        {
            UpdateStatisticInAllDatabases(userContext, "usersDeleted");
        }

        public void CategoryDeleted(EntitiesUsers userContext, Entities objectContext)
        {

            UpdateStatistic(userContext, objectContext, "categoriesDeleted");
        }

        public void CategoryCreated(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "categoriesCreated");
        }

        public void ProductDeleted(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "productsDeleted");
        }

        public void CompanyDeleted(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "companiesDeleted");
        }

        public void PictureDeleted(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "picturesDeleted");
        }

        public void PictureUploaded(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "picturesUploaded");
        }

        public void UserRegistered(EntitiesUsers userContext)
        {
            UpdateStatisticInAllDatabases(userContext, "usersRegistered");
        }

        public void SessionStarted(EntitiesUsers userContext)
        {
            UpdateStatisticInAllDatabases(userContext, "sessionsStarted");
        }


        public void UserLogged(EntitiesUsers userContext)
        {
            UpdateStatisticInAllDatabases(userContext, "usersLogged");
        }

        public void UserLoggedOut(EntitiesUsers userContext)
        {
            UpdateStatisticInAllDatabases(userContext, "usersLoggedOut");
        }

        public void CompanyCreated(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "companiesCreated");
        }

        public void ProductCreated(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "productsCreated");
        }

        public void TopicAdded(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "topicsCreated");
        }

        public void TopicDeleted(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "topicsDeleted");
        }


        public void CommentWritten(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "commentsWritten");
        }

        public void CommentDeleted(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "commentsDeleted");
        }


        public void ReportWritten(EntitiesUsers userContext, Entities objectContext)
        {
            UpdateStatistic(userContext, objectContext, "reportsWritten");
        }

        public long getTodaysCompaniesCreated(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            long result = 0;

            DateTime today = DateTime.UtcNow;
            Statistic currStat = Get(objectContext, today);
            if (currStat != null)
            {
                result = currStat.companiesCreated;
            }

            return result;
        }

        public long getTodaysProductsCreated(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            long result = 0;

            DateTime today = DateTime.UtcNow;
            Statistic currStat = Get(objectContext, today);
            if (currStat != null)
            {
                result = currStat.productsCreated;
            }

            return result;
        }

        public long getTodaysReportsWritten(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            long result = 0;

            DateTime today = DateTime.UtcNow;
            Statistic currStat = Get(objectContext, today);
            if (currStat != null)
            {
                result = currStat.reportsWritten;
            }

            return result;
        }


        public long getTodaysWrittenComments(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            long result = 0;

            DateTime today = DateTime.UtcNow;
            Statistic currStat = Get(objectContext, today);
            if (currStat != null)
            {
                result = currStat.commentsWritten; ;
                if (result < 0)
                {
                    result = 0;
                }

            }

            return result;
        }

        public long GetTodaysUsersRegistered(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            long result = 0;

            DateTime today = DateTime.UtcNow;
            Statistic currStat = Get(objectContext, today);
            if (currStat != null)
            {
                result = currStat.usersRegistered;
            }

            return result;
        }


        public long getTodaysSessions(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            long result = 0;

            DateTime today = DateTime.UtcNow;
            Statistic currStat = Get(objectContext, today);
            if (currStat != null)
            {
                result = currStat.sessionsStarted;
            }

            return result;
        }


        /// <summary>
        /// Returns Last number od Statistics
        /// </summary>
        public List<Statistic> GetLastNumStatistics(Entities objectContext, long number)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }

            IEnumerable<Statistic> stats = objectContext.StatisticSet.
                OrderByDescending<Statistic, long>(new Func<Statistic, long>(IdSelector));

            long count = stats.Count<Statistic>();
            long num = number;

            if (count < num)
            {
                num = count;
            }

            List<Statistic> Statistics = new List<Statistic>();

            for (int i = 0; i < num; i++)
            {
                Statistics.Add(stats.ElementAt<Statistic>(i));
            }

            return Statistics;
        }



    }
}
