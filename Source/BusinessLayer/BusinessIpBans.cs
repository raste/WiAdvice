// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessIpBans
    {
        private static object addBan = new object();
        private static object modifyBan = new object();

        public void BanIpAdress(EntitiesUsers userContext, Entities objectContext, BusinessLog bLog, string IpAdress, User currUser
            , string description, DateTime activeTillDate)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            BusinessUser businessUser = new BusinessUser();

            if (!businessUser.IsFromAdminTeam(currUser))
            {
                throw new BusinessException(string.Format("User : {0}, ID : {1}, cannot Ban Ip adress : {2}, because he is not administrator."
                    , currUser.username, currUser.ID, IpAdress));
            }

            if (string.IsNullOrEmpty(IpAdress))
            {
                throw new BusinessException("IpAdress is empty");
            }

            if ((activeTillDate != DateTime.MinValue) && (DateTime.Compare(activeTillDate, DateTime.UtcNow) < 1))
            {
                throw new BusinessException(string.Format("IpBan cannot be active untill past time, User ID : {0}", currUser.ID));
            }

            lock (addBan)
            {
                if (IsThereBanForIpAdress(userContext, IpAdress) == true)
                {
                    IpBan currBan = Get(userContext, IpAdress);
                    if (currBan == null)
                    {
                        throw new BusinessException(string.Format("There is no active IpBan with adress : {0}, User id : {1}"
                            , IpAdress, currUser.ID));
                    }

                    ChangeActiveStateOfIpBan(userContext, objectContext, bLog, currBan, currUser, description, activeTillDate);
                }
                else
                {
                    IpBan newIpBan = new IpBan();
                    newIpBan.byUser = currUser;
                    newIpBan.active = true;
                    newIpBan.dateCreated = DateTime.UtcNow;
                    newIpBan.IPadress = IpAdress;
                    newIpBan.lastModified = newIpBan.dateCreated;
                    newIpBan.modifiedBy = newIpBan.byUser;

                    if (string.IsNullOrEmpty(description))
                    {
                        newIpBan.notes = string.Format("Ip adress ({0}) ban created by : {1}, ID : {2}, on {3}{4}",
                        IpAdress, currUser.username, currUser.ID, newIpBan.dateCreated.ToString(), Environment.NewLine);
                    }
                    else
                    {
                        newIpBan.notes = string.Format("Ip adress ({0}) ban created by : {1}, ID : {2}, on {3}{4} with notes : {5}{4}",
                        IpAdress, currUser.username, currUser.ID, newIpBan.dateCreated.ToString(), Environment.NewLine, description);
                    }

                    if (activeTillDate == DateTime.MinValue)
                    {
                        newIpBan.untillDate = null;
                    }
                    else
                    {
                        newIpBan.untillDate = activeTillDate;
                    }

                    userContext.AddToIpBanSet(newIpBan);
                    Tools.Save(userContext);

                    bLog.LogIpBan(objectContext, newIpBan, LogType.create, string.Empty, string.Empty, currUser);
                }
            }
        }

        /// <summary>
        /// Checks if there is ban for ip adress which is actibe
        /// </summary>
        public bool IsThereActiveBanForIpAdress(EntitiesUsers userContext, string IpAdress)
        {
            Tools.AssertObjectContextExists(userContext);
            if (string.IsNullOrEmpty(IpAdress))
            {
                throw new BusinessException("IpAdress is empty");
            }

            IpBan checkBan = userContext.IpBanSet.FirstOrDefault(ban => ban.IPadress == IpAdress && ban.active == true);

            if (checkBan == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// doesnt check for active
        /// </summary>
        public bool IsThereBanForIpAdress(EntitiesUsers userContext, string IpAdress)
        {
            Tools.AssertObjectContextExists(userContext);
            if (string.IsNullOrEmpty(IpAdress))
            {
                throw new BusinessException("IpAdress is empty");
            }

            IpBan checkBan = userContext.IpBanSet.FirstOrDefault(ban => ban.IPadress == IpAdress);

            if (checkBan == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void ChangeBanExpireDate(EntitiesUsers userContext, Entities objectContext, BusinessLog bLog, IpBan ban
            , User currUser, string description, DateTime activeTillDate)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (ban == null)
            {
                throw new BusinessException("ban is null");
            }

            if ((activeTillDate != DateTime.MinValue) && (DateTime.Compare(activeTillDate, DateTime.UtcNow) < 1))
            {
                throw new BusinessException(string.Format("IpBan cannot be active untill past time, User ID : {0}", currUser.ID));
            }

            lock (modifyBan)
            {

                string oldValue = "";
                if (ban.untillDate == null)
                {
                    oldValue = "null";
                }
                else
                {
                    oldValue = ban.untillDate.Value.ToString();
                }

                if (activeTillDate == DateTime.MinValue)
                {
                    ban.untillDate = null;
                }
                else
                {
                    ban.untillDate = activeTillDate;
                }

                ban.lastModified = DateTime.UtcNow;
                ban.modifiedBy = currUser;

                if (string.IsNullOrEmpty(description))
                {
                    ban.notes += string.Format("{0} Ban untill date modified by : {1}, ID : {2}, on {3}.{0}",
                            Environment.NewLine, currUser.username, currUser.ID, ban.lastModified.ToString());
                }
                else
                {
                    ban.notes += string.Format("{0} Ban untill date modified by : {1}, ID : {2}, on {3}.{0} with notes : {4}{0}",
                           Environment.NewLine, currUser.username, currUser.ID, ban.lastModified.ToString(), description);
                }

                Tools.Save(userContext);

                bLog.LogIpBan(objectContext, ban, LogType.edit, "untillDate", oldValue, currUser);
            }
        }

        public void ChangeActiveStateOfIpBan(EntitiesUsers userContext, Entities objectContext, BusinessLog bLog
            , IpBan ban, User currUser, string description, DateTime activeTillDate)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (ban == null)
            {
                throw new BusinessException("ban is null");
            }

            BusinessUser businessUser = new BusinessUser();

            if (businessUser.IsFromUserTeam(currUser))
            {
                throw new BusinessException(string.Format("User : {0}, ID : {1}, cannot Ban Ip adress : {2}, because he is not administrator."
                    , currUser.username, currUser.ID, ban.IPadress));
            }

            if ((activeTillDate != DateTime.MinValue) && (DateTime.Compare(activeTillDate, DateTime.UtcNow) < 1))
            {
                throw new BusinessException(string.Format("IpBan cannot be active untill past time, User ID : {0}", currUser.ID));
            }

            lock (modifyBan)
            {
                string oldValue = ban.active.ToString();
                string oldDate = "";
                bool dateChanged = false;

                if (ban.untillDate != null)
                {
                    oldDate = ban.untillDate.Value.ToString();
                }
                else
                {
                    oldDate = "null";
                }

                if (ban.active == true)
                {
                    ban.active = false;

                    if (activeTillDate != DateTime.MinValue)
                    {
                        ban.untillDate = activeTillDate;
                        dateChanged = true;
                    }
                }
                else
                {
                    ban.active = true;

                    if (activeTillDate == DateTime.MinValue)
                    {
                        if (ban.untillDate != null && (DateTime.Compare(activeTillDate, DateTime.UtcNow) < 1))
                        {
                            ban.untillDate = null;
                            dateChanged = true;
                        }
                    }
                    else
                    {
                        ban.untillDate = activeTillDate;
                        dateChanged = true;
                    }
                }

                ban.lastModified = DateTime.UtcNow;
                ban.modifiedBy = currUser;

                if (string.IsNullOrEmpty(description))
                {
                    ban.notes += string.Format("{0} Ban modified by : {1}, ID : {2}, on {3}, active set to : {4}{0}",
                        Environment.NewLine, currUser.username, currUser.ID, ban.lastModified.ToString(), ban.active.ToString());
                }
                else
                {
                    ban.notes += string.Format("{0} Ban modified by : {1}, ID : {2}, on {3}, active set to : {4}{0} with notes : {5}{0}",
                       Environment.NewLine, currUser.username, currUser.ID, ban.lastModified.ToString(), ban.active.ToString(), description);
                }

                Tools.Save(userContext);

                if (ban.active == true)
                {
                    bLog.LogIpBan(objectContext, ban, LogType.undelete, string.Empty, string.Empty, currUser);
                }
                else
                {
                    bLog.LogIpBan(objectContext, ban, LogType.delete, string.Empty, string.Empty, currUser);
                }

                if (dateChanged)
                {
                    bLog.LogIpBan(objectContext, ban, LogType.edit, "untillDate", oldDate, currUser);
                }
            }
        }

        public IpBan GetActiveBan(EntitiesUsers userContext, string ipAdress)
        {
            Tools.AssertObjectContextExists(userContext);
            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            IpBan ban = userContext.IpBanSet.FirstOrDefault(bn => bn.IPadress == ipAdress && bn.active == true);

            return ban;
        }

        /// <summary>
        /// Returns ban for ip adress if found..doesnt matter if active or not
        /// </summary>
        public IpBan Get(EntitiesUsers userContext, string ipAdress)
        {
            Tools.AssertObjectContextExists(userContext);
            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            IpBan ban = userContext.IpBanSet.FirstOrDefault(bn => bn.IPadress == ipAdress);

            return ban;
        }

        public List<IpBan> GetActiveBans(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);

            List<IpBan> bans = userContext.IpBanSet.Where(ban => ban.active == true).ToList();
            if (bans.Count > 0)
            {
                bans.Reverse();
            }

            return bans;
        }


        public void ScriptCheckBansActiveUntillDate(EntitiesUsers userContext, Entities objectContext, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            List<IpBan> activeBans = userContext.IpBanSet.Where(ban => ban.active == true && ban.untillDate != null).ToList();

            if (activeBans.Count > 0)
            {
                List<IpBan> expiredDates = new List<IpBan>();

                DateTime now = DateTime.UtcNow;

                foreach (IpBan ban in activeBans)
                {
                    if (DateTime.Compare(ban.untillDate.Value, now) < 1)
                    {
                        expiredDates.Add(ban);
                    }
                }

                if (expiredDates.Count > 0)
                {
                    BusinessUser businessUser = new BusinessUser();
                    User system = businessUser.GetSystem(userContext);
                    string description = "Ban date expired.";

                    foreach (IpBan ban in expiredDates)
                    {
                        ChangeActiveStateOfIpBan(userContext, objectContext, bLog, ban, system, description, DateTime.MinValue);
                    }
                }
            }
        }

        public int CounRegisteredUsersFromIpAdress(EntitiesUsers userContext, string ipAdress)
        {
            Tools.AssertObjectContextExists(userContext);
            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            int result = 0;

            result = userContext.UserLogSet.Count(log => log.type == "create" && log.typeModified == "user" && log.IpAdress == ipAdress);

            return result;
        }

    }
}
