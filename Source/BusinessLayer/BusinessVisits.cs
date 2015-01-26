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
    public class BusinessVisits
    {
        private void AddVisit(Entities objectContext, EntitiesUsers userContext, VisitedType type, long id, User byUser, string ipAdress)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }

            if (byUser == null)
            {
                BusinessUser bUser = new BusinessUser();
                byUser = bUser.GetGuest(userContext);
            }

            UserID userId = Tools.GetUserID(objectContext, byUser);

            string strType = VisitType(type);

            Visit newVisit = new Visit();

            newVisit.type = strType;
            newVisit.typeID = id;
            newVisit.User = userId;
            newVisit.dateVisited = DateTime.UtcNow;
            newVisit.ipAdress = ipAdress;

            objectContext.AddToVisitSet(newVisit);
            Tools.Save(objectContext);
        }

        private static string VisitType(VisitedType type)
        {
            string strType = string.Empty;

            switch (type)
            {
                case VisitedType.ProductTopic:
                    strType = "productTopic";
                    break;
                default:
                    throw new BusinessException(string.Format("type = {0} is not supported VisitedType", type));
            }

            return strType;
        }

        public Visit GetLastUserVisitForType(Entities objectContext, VisitedType type, long typeId, User byUser, string ipAdress)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }
            if (typeId < 1)
            {
                throw new BusinessException("typeId is < 1");
            }

            string strType = VisitType(type);
            List<Visit> visits = new List<Visit>();
            Visit result = null;

            Visit lastVisByUser = null;
            if (byUser != null)
            {
                visits = objectContext.VisitSet.Where(vt => vt.type == strType && vt.typeID == typeId && vt.User.ID == byUser.ID).ToList();
                if (visits != null && visits.Count > 0)
                {
                    lastVisByUser = visits.Last();
                }
            }

            Visit lastVisByIp = null;
            visits = objectContext.VisitSet.Where(vt => vt.type == strType && vt.typeID == typeId && vt.ipAdress == ipAdress).ToList();
            if (visits != null && visits.Count > 0)
            {
                lastVisByIp = visits.Last();
            }

            if (lastVisByUser != null && lastVisByIp != null)
            {
                if (lastVisByUser.dateVisited > lastVisByIp.dateVisited)
                {
                    result = lastVisByUser;
                }
                else
                {
                    result = lastVisByIp;
                }
            }
            else if (lastVisByUser != null)
            {
                result = lastVisByUser;
            }
            else if (lastVisByIp != null)
            {
                result = lastVisByIp;
            }

            return result;
        }

        public void ProductTopicVisited(Entities objectContext, EntitiesUsers userContext, ProductTopic topic, User byUser, string ipAdress)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            AddVisit(objectContext, userContext, VisitedType.ProductTopic, topic.ID, byUser, ipAdress);

            BusinessProductTopics bpTopic = new BusinessProductTopics();
            bpTopic.IncreaseTopicVisits(objectContext, topic);
        }

        public void UpdateProductTopicVisited(Entities objectContext, EntitiesUsers userContext, ProductTopic topic, User byUser, string ipAdress)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            Visit lastVisit = GetLastUserVisitForType(objectContext, VisitedType.ProductTopic, topic.ID, byUser, ipAdress);
            if (lastVisit != null)
            {
                lastVisit.dateVisited = DateTime.UtcNow;
                Tools.Save(objectContext);
            }
        }


        public bool CheckIfUserVisitedProductTopic(Entities objectContext, ProductTopic topic, User byUser, ref DateTime when)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            if (byUser == null)
            {
                throw new BusinessException("byUser is null");
            }

            bool result = false;

            string type = VisitType(VisitedType.ProductTopic);

            List<Visit> visits = objectContext.VisitSet.Where(vt => vt.type == type && vt.typeID == topic.ID && vt.User.ID == byUser.ID).ToList();

            if (visits != null && visits.Count > 0)
            {
                result = true;
                when = visits.Last().dateVisited;
            }

            return result;
        }


        public bool CheckIfIpAdressVisitedProductTopic(Entities objectContext, ProductTopic topic, string ipAdress, ref DateTime when)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            bool result = false;

            string type = VisitType(VisitedType.ProductTopic);

            List<Visit> visits = objectContext.VisitSet.Where(vt => vt.type == type && vt.typeID == topic.ID && vt.ipAdress == ipAdress).ToList();

            if (visits != null && visits.Count > 0)
            {
                result = true;
                when = visits.Last().dateVisited;
            }

            return result;
        }




    }
}
