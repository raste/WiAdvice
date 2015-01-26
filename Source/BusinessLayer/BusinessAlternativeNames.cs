// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessAlternativeNames
    {
        public void AddAlternativeNameForProduct(Entities objectContext, Product currProduct, BusinessLog bLog, User currUser
            , string name)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty");
            }

            AlternativeProductName testAlternativeName = objectContext.AlternativeProductNameSet.FirstOrDefault
                (apn => apn.Product.ID == currProduct.ID && apn.name == name);

            if (testAlternativeName == null)
            {
                AlternativeProductName newName = new AlternativeProductName();
                newName.name = name;
                newName.Product = currProduct;
                newName.CreatedBy = Tools.GetUserID(objectContext, currUser);
                newName.dateCreated = DateTime.UtcNow;
                newName.LastModifiedBy = newName.CreatedBy;
                newName.lastModified = newName.dateCreated;
                newName.visible = true;

                objectContext.AddToAlternativeProductNameSet(newName);
                Tools.Save(objectContext);

                bLog.LogProductAlternativeName(objectContext, newName, LogType.create, currUser);

                BusinessProduct bProduct = new BusinessProduct();
                bProduct.UpdateLastModifiedAndModifiedBy(objectContext, currProduct, newName.CreatedBy);
            }
            else if (testAlternativeName.visible == false)
            {

                testAlternativeName.visible = true;
                testAlternativeName.lastModified = DateTime.UtcNow;
                testAlternativeName.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                bLog.LogProductAlternativeName(objectContext, testAlternativeName, LogType.undelete, currUser);

                BusinessProduct bProduct = new BusinessProduct();
                bProduct.UpdateLastModifiedAndModifiedBy(objectContext, currProduct, testAlternativeName.LastModifiedBy);
            }
            else
            {
                throw new BusinessException(string.Format("There is already visible.true alternative name = '{0}' for product ID : {1}, user id : {2}"
                    , name, currProduct.ID, currUser.ID));
            }
        }

        public AlternativeProductName GetForProduct(Entities objectContext, Product currProduct, long id, bool checkForVisibility, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            AlternativeProductName name = null;
            if (checkForVisibility == true)
            {
                name = objectContext.AlternativeProductNameSet.FirstOrDefault
                    (apn => apn.ID == id && apn.Product.ID == currProduct.ID && apn.visible == true);
            }
            else
            {
                name = objectContext.AlternativeProductNameSet.FirstOrDefault(apn => apn.ID == id && apn.Product.ID == currProduct.ID);
            }

            if (name == null && throwExcIfNull == true)
            {
                throw new BusinessException(string.Format("There is no alternative product name with ID : {0} which is for product id : {1}"
                    , id, currProduct.ID));
            }

            return name;
        }

        public bool IsThereAlternativeNameForProduct(Entities objectContext, Product currProduct, string name, bool checkForVisibility)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty");
            }

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            bool result = false;

            AlternativeProductName aname = null;
            if (checkForVisibility == true)
            {
                aname = objectContext.AlternativeProductNameSet.FirstOrDefault
                    (apn => apn.name == name && apn.Product.ID == currProduct.ID && apn.visible == true);
            }
            else
            {
                aname = objectContext.AlternativeProductNameSet.FirstOrDefault(apn => apn.name == name && apn.Product.ID == currProduct.ID);
            }

            if (aname != null)
            {
                result = true;
            }

            return result;
        }

        public void DeleteAlternativeProductName(Entities objectContext, BusinessLog bLog, User currUser, AlternativeProductName name)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (name == null)
            {
                throw new BusinessException("name is null");
            }

            if (name.visible == false)
            {
                throw new BusinessException(string.Format("AlternativeProductName ID : {0} is already visible:false, User id : {1}", name.ID, currUser.ID));
            }

            name.visible = false;
            name.lastModified = DateTime.UtcNow;
            name.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogProductAlternativeName(objectContext, name, LogType.delete, currUser);

            if (!name.ProductReference.IsLoaded)
            {
                name.ProductReference.Load();
            }
            BusinessProduct bProduct = new BusinessProduct();
            bProduct.UpdateLastModifiedAndModifiedBy(objectContext, name.Product, name.LastModifiedBy);
        }

        public List<AlternativeProductName> GetVisibleAlternativeProductNames(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            List<AlternativeProductName> names = objectContext.AlternativeProductNameSet.Where
                (apn => apn.Product.ID == currProduct.ID && apn.visible == true).ToList();

            return names;
        }

        public List<AlternativeProductName> GetAllAlternativeProductNames(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            List<AlternativeProductName> names = objectContext.AlternativeProductNameSet.Where
                (apn => apn.Product.ID == currProduct.ID).ToList();

            return names;
        }

        /// <summary>
        /// Counts alternative names for product.
        /// </summary>
        /// <param name="onlyVisibleTrue">True if should check only for visible.true, otherwise doesn`t check for visibility</param>
        public int CountAlternativeProductNames(Entities objectContext, Product currProduct, bool onlyVisibleTrue)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            int count = 0;

            if (onlyVisibleTrue == true)
            {
                count = objectContext.AlternativeProductNameSet.Count
                    (apn => apn.Product.ID == currProduct.ID && apn.visible == true);
            }
            else
            {
                count = objectContext.AlternativeProductNameSet.Count
                    (apn => apn.Product.ID == currProduct.ID);
            }

            return count;
        }

        public void AddAlternativeNameForCompany(Entities objectContext, Company currCompany, BusinessLog bLog, User currUser
           , string name)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty");
            }

            AlternativeCompanyName testAlternativeName = objectContext.AlternativeCompanyNameSet.FirstOrDefault
                (apn => apn.Company.ID == currCompany.ID && apn.name == name);

            if (testAlternativeName == null)
            {
                AlternativeCompanyName newName = new AlternativeCompanyName();
                newName.name = name;
                newName.Company = currCompany;
                newName.CreatedBy = Tools.GetUserID(objectContext, currUser);
                newName.dateCreated = DateTime.UtcNow;
                newName.LastModifiedBy = newName.CreatedBy;
                newName.lastModified = newName.dateCreated;
                newName.visible = true;

                objectContext.AddToAlternativeCompanyNameSet(newName);
                Tools.Save(objectContext);

                bLog.LogCompanyAlternativeName(objectContext, newName, LogType.create, currUser);

                BusinessCompany bCompany = new BusinessCompany();
                bCompany.UpdateLastModifiedAndModifiedBy(objectContext, currCompany, newName.CreatedBy);
            }
            else if (testAlternativeName.visible == false)
            {

                testAlternativeName.visible = true;
                testAlternativeName.lastModified = DateTime.UtcNow;
                testAlternativeName.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                bLog.LogCompanyAlternativeName(objectContext, testAlternativeName, LogType.undelete, currUser);

                BusinessCompany bCompany = new BusinessCompany();
                bCompany.UpdateLastModifiedAndModifiedBy(objectContext, currCompany, testAlternativeName.LastModifiedBy);
            }
            else
            {
                throw new BusinessException(string.Format("There is already visible.true alternative name = '{0}' for company ID : {1}, user id : {2}"
                    , name, currCompany.ID, currUser.ID));
            }
        }

        public AlternativeCompanyName GetForCompany(Entities objectContext, Company currCompany, long id, bool checkForVisibility, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            AlternativeCompanyName name = null;
            if (checkForVisibility == true)
            {
                name = objectContext.AlternativeCompanyNameSet.FirstOrDefault
                    (apn => apn.ID == id && apn.Company.ID == currCompany.ID && apn.visible == true);
            }
            else
            {
                name = objectContext.AlternativeCompanyNameSet.FirstOrDefault(apn => apn.ID == id && apn.Company.ID == currCompany.ID);
            }

            if (name == null && throwExcIfNull == true)
            {
                throw new BusinessException(string.Format("There is no alternative product name with ID : {0} which is for company id : {1}"
                    , id, currCompany.ID));
            }

            return name;
        }

        public bool IsThereAlternativeNameForCompany(Entities objectContext, Company currCompany, string name, bool checkForVisibility)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty");
            }

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            bool result = false;

            AlternativeCompanyName aname = null;
            if (checkForVisibility == true)
            {
                aname = objectContext.AlternativeCompanyNameSet.FirstOrDefault
                    (apn => apn.name == name && apn.Company.ID == currCompany.ID && apn.visible == true);
            }
            else
            {
                aname = objectContext.AlternativeCompanyNameSet.FirstOrDefault(apn => apn.name == name && apn.Company.ID == currCompany.ID);
            }

            if (aname != null)
            {
                result = true;
            }

            return result;
        }

        public void DeleteAlternativeCompanyName(Entities objectContext, BusinessLog bLog, User currUser, AlternativeCompanyName name)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (name == null)
            {
                throw new BusinessException("name is null");
            }

            if (name.visible == false)
            {
                throw new BusinessException(string.Format("AlternativeCompanyName ID : {0} is already visible:false, User id : {1}", name.ID, currUser.ID));
            }

            name.visible = false;
            name.lastModified = DateTime.UtcNow;
            name.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogCompanyAlternativeName(objectContext, name, LogType.delete, currUser);

            if (!name.CompanyReference.IsLoaded)
            {
                name.CompanyReference.Load();
            }

            BusinessCompany bCompany = new BusinessCompany();
            bCompany.UpdateLastModifiedAndModifiedBy(objectContext, name.Company, name.LastModifiedBy);
        }

        public List<AlternativeCompanyName> GetVisibleAlternativeCompanyNames(Entities objectContext, Company currCompany)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            List<AlternativeCompanyName> names = objectContext.AlternativeCompanyNameSet.Where
                (apn => apn.Company.ID == currCompany.ID && apn.visible == true).ToList();

            return names;
        }

        public List<AlternativeCompanyName> GetAllAlternativeCompanyNames(Entities objectContext, Company currCompany)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            List<AlternativeCompanyName> names = objectContext.AlternativeCompanyNameSet.Where
                (apn => apn.Company.ID == currCompany.ID).ToList();

            return names;
        }

        /// <summary>
        /// Counts alternative names for company.
        /// </summary>
        /// <param name="onlyVisibleTrue">True if should check only for visible.true, otherwise doesn`t check for visibility</param>
        public int CountAlternativeCompanyNames(Entities objectContext, Company currCompany, bool onlyVisibleTrue)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            int count = 0;

            if (onlyVisibleTrue == true)
            {
                count = objectContext.AlternativeCompanyNameSet.Count
                    (apn => apn.Company.ID == currCompany.ID && apn.visible == true);
            }
            else
            {
                count = objectContext.AlternativeCompanyNameSet.Count
                    (apn => apn.Company.ID == currCompany.ID);
            }

            return count;
        }

    }
}
