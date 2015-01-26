// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessProductLink
    {
        public void Add(Entities objectContext, BusinessLog bLog, Product forProduct
            , User currUser, string link, string description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (forProduct == null)
            {
                throw new BusinessException("forProduct is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (string.IsNullOrEmpty(link))
            {
                throw new BusinessException("link is empty");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }

            if (CountProductLinks(forProduct, true) >= Configuration.ProductLinksMaxPerProduct)
            {
                return;
            }

            if (Tools.NameValidatorPassed(objectContext, "productLink", link, forProduct.ID) == false)
            {
                return;
            }

            ProductLink newLink = new ProductLink();
            newLink.Product = forProduct;
            newLink.link = link;
            newLink.description = description;
            newLink.User = Tools.GetUserID(objectContext, currUser);
            newLink.dateCreated = DateTime.UtcNow;
            newLink.LastModifiedBy = newLink.User;
            newLink.dateLastModified = newLink.dateCreated;
            newLink.visible = true;

            objectContext.AddToProductLinkSet(newLink);
            Tools.Save(objectContext);

            bLog.LogProductLink(objectContext, newLink, LogType.create, string.Empty, string.Empty, currUser);
        }

        public void DeleteLink(Entities objectContext, EntitiesUsers userContext, BusinessLog bLog, ProductLink link
            , User currUser, bool sendWarning)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (link == null)
            {
                throw new BusinessException("link is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (link.visible == false)
            {
                return;
            }

            link.visible = false;
            link.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
            link.dateLastModified = DateTime.UtcNow;
            Tools.Save(objectContext);

            bLog.LogProductLink(objectContext, link, LogType.delete, string.Empty, string.Empty, currUser);

            if (!link.ProductReference.IsLoaded)
            {
                link.ProductReference.Load();
            }
            if (!link.UserReference.IsLoaded)
            {
                link.UserReference.Load();
            }

            BusinessUser bUser = new BusinessUser();
            User userAdded = bUser.GetWithoutVisible(userContext, link.User.ID, true);

            if (userAdded.ID != currUser.ID)
            {
                string sysMsg = string.Format("{0} \" {1} \" {2} {3}", Tools.GetResource("SMproductLinkDeleted")
                    , link.Product.name, Tools.GetResource("SMproductLinkDeleted2"), link.link);

                BusinessSystemMessages bSM = new BusinessSystemMessages();
                bSM.Add(userContext, userAdded, sysMsg);
            }

            if (sendWarning == true && userAdded.visible == true)
            {
                if (!bUser.IsFromAdminTeam(currUser))
                {
                    return;
                }

                string warnMsg = string.Format("{0} \" {1} \" {2} {3}", Tools.GetResource("productLinkDeletedW")
                    , link.Product.name, Tools.GetResource("productLinkDeletedW2"), link.link);

                BusinessWarnings bWarnings = new BusinessWarnings();
                bWarnings.AddWarning(userContext, objectContext, null, "general", warnMsg, userAdded, currUser, bLog);
            }

            BusinessReport bReport = new BusinessReport();
            bReport.ResolveReportsForProductLinkWhichIsBeingDeleted(objectContext, userContext, link, bLog, currUser);

        }

        public int CountProductLinks(Product forProduct, bool onlyVisible)
        {
            if (forProduct == null)
            {
                throw new BusinessException("forProduct is null");
            }

            int count = 0;

            forProduct.ProductLinks.Load();

            if (onlyVisible == true)
            {
                count = forProduct.ProductLinks.Count(pl => pl.visible == true);
            }
            else
            {
                count = forProduct.ProductLinks.Count();
            }

            return count;
        }

        public void ChangeLinkDescription(Entities objectContext, EntitiesUsers userContext, BusinessLog bLog, ProductLink link
            , User currUser, string newDescription)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (link == null)
            {
                throw new BusinessException("link is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (string.IsNullOrEmpty(newDescription))
            {
                throw new BusinessException("newDescription is empty");
            }

            string oldDescr = link.description;

            link.description = newDescription;
            link.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
            link.dateLastModified = DateTime.UtcNow;

            Tools.Save(objectContext);

            bLog.LogProductLink(objectContext, link, LogType.edit, "description", oldDescr, currUser);

            if (!link.UserReference.IsLoaded)
            {
                link.UserReference.Load();
            }

            if (link.User.ID != currUser.ID)
            {
                if (!link.ProductReference.IsLoaded)
                {
                    link.ProductReference.Load();
                }

                BusinessUser bUser = new BusinessUser();
                User userAdded = bUser.GetWithoutVisible(userContext, link.User.ID, true);

                string sysMsg = string.Format("{0} {1} {2} {3} {4} {5}.", Tools.GetResource("SMproductLinkDescrChanged")
                    , link.link, Tools.GetResource("SMproductLinkDescrChanged2"), link.Product.name
                    , Tools.GetResource("SMproductLinkDescrChanged3"), currUser.username);

                BusinessSystemMessages bSM = new BusinessSystemMessages();
                bSM.Add(userContext, userAdded, sysMsg);
            }
        }

        public List<ProductLink> GetProductLinks(Entities objectContext, Product product, bool onlyVisibleTrue)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            List<ProductLink> links = new List<ProductLink>();

            if (onlyVisibleTrue == true)
            {
                links = objectContext.ProductLinkSet.Where(pl => pl.Product.ID == product.ID && pl.visible == true).ToList();
            }
            else
            {
                links = objectContext.ProductLinkSet.Where(pl => pl.Product.ID == product.ID).ToList();
            }

            return links;
        }

        /// <summary>
        /// only editors and admins (which can edit product) can edit links
        /// </summary>
        public bool CanUserModifyLink(Entities objectContext, EntitiesUsers userContext, ProductLink link, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (link == null)
            {
                throw new BusinessException("link is null");
            }

            if (currUser == null)
            {
                return false;
            }

            if (!link.ProductReference.IsLoaded)
            {
                link.ProductReference.Load();
            }

            BusinessUser bUser = new BusinessUser();

            if (bUser.IsFromAdminTeam(currUser) && !bUser.CanAdminDo(userContext, currUser, AdminRoles.EditProducts))
            {
                return false;
            }

            if ((link.visible == false || link.Product.visible == false) && !bUser.IsFromAdminTeam(currUser))
            {
                return false;
            }

            if (!bUser.IsFromAdminTeam(currUser))
            {
                if (bUser.CanUserModifyProduct(objectContext, link.Product.ID, currUser.ID) == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// only editors and admins (which can edit product) can edit links
        /// </summary>
        public bool CanUserModifyProductLinks(Entities objectContext, EntitiesUsers userContext, Product product, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            if (currUser == null)
            {
                return false;
            }

            BusinessUser bUser = new BusinessUser();

            if (bUser.IsFromAdminTeam(currUser) && !bUser.CanAdminDo(userContext, currUser, AdminRoles.EditProducts))
            {
                return false;
            }

            if (product.visible == false && !bUser.IsFromAdminTeam(currUser))
            {
                return false;
            }

            if (!bUser.IsFromAdminTeam(currUser))
            {
                if (bUser.CanUserModifyProduct(objectContext, product.ID, currUser.ID) == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// olny users who can add Products and administrators (which can edit product) can add links.
        /// </summary>
        public bool CanUserAddLink(Entities objectContext, EntitiesUsers userContext, Product toProduct, User currUser, bool checkForTime, out string error)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            error = string.Empty;

            if (toProduct == null)
            {
                throw new BusinessException("toProduct is null");
            }

            if (currUser == null)
            {
                return false;
            }

            BusinessUser bUser = new BusinessUser();

            bool isAdmin = bUser.IsFromAdminTeam(currUser);

            if (isAdmin == true && !bUser.CanAdminDo(userContext, currUser, AdminRoles.EditProducts))
            {
                return false;
            }

            if (toProduct.visible == false && isAdmin == false)
            {
                return false;
            }

            if (isAdmin == false && !bUser.CanUserDo(userContext, currUser, UserRoles.AddProducts))
            {
                return false;
            }

            if (CountProductLinks(toProduct, true) >= Configuration.ProductLinksMaxPerProduct)
            {
                error = Tools.GetResource("errMaxProducLinksReached");
                return false;
            }

            // min time between adding
            if (checkForTime == true && isAdmin == false && Configuration.ProductLinksMinTimeBetweenAdding > 0)
            {
                ProductLink lastAddedByUser = null;

                List<ProductLink> linksAddedbyUser = objectContext.ProductLinkSet.Where(pl => pl.User.ID == currUser.ID).ToList();
                if (linksAddedbyUser.Count > 0)
                {
                    lastAddedByUser = linksAddedbyUser.Last();
                }

                if (lastAddedByUser != null)
                {
                    DateTime prodTime = lastAddedByUser.dateCreated;

                    TimeSpan span = DateTime.UtcNow - lastAddedByUser.dateCreated;
                    int minPassed = (int)span.TotalMinutes;

                    if (minPassed < Configuration.ProductLinksMinTimeBetweenAdding)
                    {
                        int minToWait = Configuration.ProductLinksMinTimeBetweenAdding - minPassed;
                        if (minToWait == 0)
                        {
                            minToWait = 1;
                        }

                        error = string.Format("{0} {1}", Tools.GetResource("errProdLinkMinToWaitToAddAnother")
                            , minToWait);

                        return false;
                    }
                }
            }

            return true;
        }

        public ProductLink Get(Entities objectContext, long id, bool onlyVisible, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            ProductLink link = null;

            if (onlyVisible == true)
            {
                link = objectContext.ProductLinkSet.FirstOrDefault(pl => pl.ID == id && pl.visible == true);
            }
            else
            {
                link = objectContext.ProductLinkSet.FirstOrDefault(pl => pl.ID == id);
            }

            if (link == null && throwExcIfNull == true)
            {
                string visible = string.Empty;
                if (onlyVisible == true)
                {
                    visible = " visible:true";
                }

                throw new BusinessException(string.Format("There is no{0} link with id = {1}", visible, id));
            }

            return link;
        }

    }
}
