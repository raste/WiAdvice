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
    public class BusinessCompany
    {
        object addIngCategories = new object();
        object deletingCompanyCategories = new object();
        object undeletingCompanyCategories = new object();

        /// <summary>
        /// Returns Company with ID
        /// </summary>
        public Company GetCompanyWV(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            Company company = objectContext.CompanySet.FirstOrDefault<Company>(comp => comp.ID == id);

            return company;
        }

        /// <summary>
        /// Returns visible=true Company with ID
        /// </summary>
        public Company GetCompany(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }
            Company company = objectContext.CompanySet.FirstOrDefault<Company>(comp => comp.ID == id && comp.visible == true);
            return company;
        }

        /// <summary>
        /// Returns 'Other' company
        /// </summary>
        public Company GetOther(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            User System = Tools.GetSystem();

            string otherCompanyName = Tools.GetConfigurationResource("OtherCompany");

            Company company = objectContext.CompanySet.FirstOrDefault<Company>
                (comp => comp.name == otherCompanyName && comp.CreatedBy.ID == System.ID);

            if (company == null)
            {
                throw new BusinessException(string.Format("Couldnt Find 'Other' Company with local name = {0}", otherCompanyName));
            }
            return company;
        }

        /// <summary>
        /// Checks if Company is 'Other'
        /// </summary>
        /// <returns>true if it is, otherwise false</returns>
        public Boolean IsOther(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("invalid id");
            }

            Company other = GetOther(objectContext);
            if (other == null)
            {
                throw new BusinessException("other company is null");
            }

            Boolean result = false;
            if (id == other.ID)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Checks if Company is 'Other'
        /// </summary>
        /// <returns>true if it is, otherwise false</returns>
        public Boolean IsOther(Entities objectContext, Company company)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (company == null)
            {
                throw new BusinessException("company is null");
            }

            Company other = GetOther(objectContext);
            if (other == null)
            {
                throw new BusinessException("other company is null");
            }
            if (other == company)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns visible=true Company Characteristics with ID
        /// </summary>
        public CompanyCharacterestics GetCompanyCharacterestic(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }
            CompanyCharacterestics compChar =
                objectContext.CompanyCharacteresticsSet.FirstOrDefault<CompanyCharacterestics>(cc => cc.ID == id && cc.visible == true);
            return compChar;
        }

        /// <summary>
        /// Add`s new Company
        /// </summary>
        private void Add(EntitiesUsers userContext, Entities objectContext, Company newCompany, BusinessLog bLog, User currUser)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (newCompany == null)
            {
                throw new ArgumentNullException("newCompany");
            }

            objectContext.AddToCompanySet(newCompany);
            Tools.Save(objectContext);

            bLog.LogCompany(objectContext, newCompany, LogType.create, string.Empty, string.Empty, currUser);

            /// code for adding action needed for modifying the company
            /// and useraction for the user who`s adding the company
            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();
            businessUserTypeActions.AddNewCompanyActions(userContext, objectContext, currUser, bLog, newCompany);

            BusinessStatistics businessStatistic = new BusinessStatistics();
            businessStatistic.CompanyCreated(userContext, objectContext);

            BusinessCategory businessCategory = new BusinessCategory();
            Category unspecifiedCategory = businessCategory.GetUnspecifiedCategory(objectContext);
            AddCompanyCategories(userContext, objectContext, unspecifiedCategory, newCompany, currUser, bLog);
        }

        public void AddCompany(EntitiesUsers userContext, Entities objectContext, BusinessLog bLog, User currUser
            , string name, string description, string site, CompanyType currType)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currType == null)
            {
                throw new BusinessException("currType is null");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty");
            }

            if (!Tools.NameValidatorPassed(objectContext, "companies", name, 0))
            {
                throw new BusinessException(string.Format("Company with name : '{0}' cannot be added because this name is already taken, user id : {1}"
                    , name, currUser.ID));
            }

            Company newCompany = new Company();

            newCompany.type = currType;
            newCompany.name = name;
            newCompany.visible = true;
            newCompany.dateCreated = DateTime.UtcNow;

            if (string.IsNullOrEmpty(site))
            {
                newCompany.website = null;
            }
            else
            {
                newCompany.website = Tools.GetCorrectedUrl(site);
            }

            newCompany.description = description;
            newCompany.lastModified = newCompany.dateCreated;

            newCompany.CreatedBy = Tools.GetUserID(objectContext, currUser);
            newCompany.LastModifiedBy = newCompany.CreatedBy;

            newCompany.canUserTakeRoleIfNoEditors = Configuration.CompaniesCanUserTakeRoleIfNoEditors;

            Add(userContext, objectContext, newCompany, bLog, currUser);

        }

        /// <summary>
        /// Adds new Company Characteristic
        /// </summary>
        public void AddCompanyCharacteristic(EntitiesUsers userContext, Entities objectContext, Company company, BusinessLog bLog
            , User currUser, string name, string description)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (company == null)
            {
                throw new ArgumentNullException("company is null");
            }
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty");
            }

            if (!Tools.NameValidatorPassed(objectContext, "compChar", name, company.ID))
            {
                throw new BusinessException(string.Format("User ID : {0} cannot add characteristic with name : {1} for company ID : {2}, because there is already with such name."
                    , currUser.ID, company.ID, name));
            }

            CompanyCharacterestics testChar = objectContext.CompanyCharacteresticsSet.FirstOrDefault<CompanyCharacterestics>
                (cc => cc.Company.ID == company.ID && cc.name == name);

            if (testChar == null)
            {
                CompanyCharacterestics newChar = new CompanyCharacterestics();
                newChar.Company = company;
                newChar.name = name;
                newChar.description = description;
                newChar.dateCreated = DateTime.UtcNow;
                newChar.CreatedBy = Tools.GetUserID(objectContext, currUser);
                newChar.visible = true;
                newChar.lastModified = DateTime.UtcNow;
                newChar.LastModifiedBy = newChar.CreatedBy;

                objectContext.AddToCompanyCharacteresticsSet(newChar);
                Tools.Save(objectContext);
                bLog.LogCompanyCharacteristic(objectContext, newChar, LogType.create, string.Empty, string.Empty, currUser);
                BusinessStatistics stat = new BusinessStatistics();
                stat.CompCharCreated(userContext, objectContext);

                UpdateLastModifiedAndModifiedBy(objectContext, company, newChar.CreatedBy);
            }
            else
            {
                if (testChar.visible == true)
                {
                    throw new BusinessException(string.Format("User ID : {0} cannot add characteristic with name : {1} for company ID : {2}, because there is already with such name."
                    , currUser.ID, company.ID, name));
                }

                testChar.visible = true;
                testChar.lastModified = DateTime.UtcNow;
                testChar.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                Tools.Save(objectContext);

                bLog.LogCompanyCharacteristic(objectContext, testChar, LogType.undelete, string.Empty, string.Empty, currUser);

                if (testChar.description != description)
                {
                    string oldValue = testChar.description;
                    testChar.description = description;
                    Tools.Save(objectContext);
                    bLog.LogCompanyCharacteristic(objectContext, testChar, LogType.edit, "description", oldValue, currUser);
                }

                UpdateLastModifiedAndModifiedBy(objectContext, company, testChar.LastModifiedBy);

            }
        }

        /// <summary>
        /// Adds new company Category if it doesnt Exist , else it makes it visible=true
        /// </summary>
        private void AddCompanyCategory(EntitiesUsers userContext, Entities objectContext, CategoryCompany newCategory, BusinessLog bLog, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (newCategory == null)
            {
                throw new ArgumentNullException("newCategory");
            }
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            BusinessCategory bCategory = new BusinessCategory();
            Category unspecifiedCategory = bCategory.GetUnspecifiedCategory(objectContext);

            CategoryCompany testCC =
                objectContext.CategoryCompanySet.FirstOrDefault
                (cc => cc.Company.ID == newCategory.Company.ID && cc.Category.ID == newCategory.Category.ID);
            if (testCC == null)
            {
                objectContext.AddToCategoryCompanySet(newCategory);
                Tools.Save(objectContext);

                // updates company last modified/by
                if (!newCategory.CompanyReference.IsLoaded)
                {
                    newCategory.CompanyReference.Load();
                }
                BusinessUser bUser = new BusinessUser();
                UserID currUserId = Tools.GetUserID(objectContext, currUser);
                UpdateLastModifiedAndModifiedBy(objectContext, newCategory.Company, currUserId);

                if (!newCategory.CategoryReference.IsLoaded)
                {
                    newCategory.CategoryReference.Load();
                }
                if (newCategory.Category.ID != unspecifiedCategory.ID)
                {
                    bLog.LogCompanyCategory(objectContext, newCategory, LogType.create, currUser);
                }

                MoveUnspecifiedProductsInWantedCategory(userContext, objectContext, newCategory, currUser, bLog);
            }
            else
            {
                if (testCC.visible)
                {
                    throw new BusinessException(string.Format("There is already category ID = {0} which is visible=true " +
                        ", it cant be added again , there are validators checking for that before this function", testCC.ID));
                }
                else
                {
                    testCC.visible = true;
                    Tools.Save(objectContext);

                    // updates company last modified/by
                    if (!testCC.CompanyReference.IsLoaded)
                    {
                        testCC.CompanyReference.Load();
                    }
                    BusinessUser bUser = new BusinessUser();
                    UserID currUserId = Tools.GetUserID(objectContext, currUser);
                    UpdateLastModifiedAndModifiedBy(objectContext, testCC.Company, currUserId);

                    if (!testCC.CategoryReference.IsLoaded)
                    {
                        newCategory.CategoryReference.Load();
                    }
                    if (testCC.Category.ID != unspecifiedCategory.ID)
                    {
                        bLog.LogCompanyCategory(objectContext, testCC, LogType.undelete, currUser);
                    }

                    MoveUnspecifiedProductsInWantedCategory(userContext, objectContext, testCC, currUser, bLog);
                }
            }
        }

        public void AddCompanyCategories(EntitiesUsers userContext, Entities objectContext, Category currCategory
            , Company currentCompany, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (currentCompany == null)
            {
                throw new BusinessException("currentCompany is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            BusinessCategory businessCategory = new BusinessCategory();

            lock (addIngCategories)
            {
                if (currCategory.last == true)
                {
                    CategoryCompany testCC =
                       objectContext.CategoryCompanySet.FirstOrDefault<CategoryCompany>
                       (cc => cc.Company.ID == currentCompany.ID && cc.Category.ID == currCategory.ID);

                    if (testCC == null)
                    {
                        CategoryCompany addCategory = new CategoryCompany();
                        addCategory.Category = currCategory;
                        addCategory.Company = currentCompany;
                        addCategory.CreatedBy = Tools.GetUserID(objectContext, currUser);
                        addCategory.dateCreated = DateTime.UtcNow;
                        addCategory.visible = true;
                        addCategory.LastModifiedBy = addCategory.CreatedBy;
                        addCategory.lastModified = addCategory.dateCreated;

                        AddCompanyCategory(userContext, objectContext, addCategory, bLog, currUser);
                    }
                    else if (testCC.visible == false)
                    {
                        MakeCompanyCategoryVisible(userContext, objectContext, testCC, currUser, bLog);
                    }
                }
                else
                {
                    List<Category> SubCategories = businessCategory.GetAllSubCategories(objectContext, currCategory, true, false).ToList();
                    if (SubCategories.Count > 0)
                    {
                        foreach (Category category in SubCategories)
                        {
                            AddCompanyCategories(userContext, objectContext, category, currentCompany, currUser, bLog);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When category is added to company, all unspecified products with wanted category (the new added) are moved to it
        /// </summary>
        private void MoveUnspecifiedProductsInWantedCategory(EntitiesUsers userContext, Entities objectContext, CategoryCompany catcomp
            , User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (catcomp == null)
            {
                throw new BusinessException("catcomp is null");
            }
            if (catcomp.visible == false)
            {
                throw new BusinessException("catcomp is visible.false");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (!catcomp.CompanyReference.IsLoaded)
            {
                catcomp.CompanyReference.Load();
            }

            Company company = catcomp.Company;

            if (!catcomp.CategoryReference.IsLoaded)
            {
                catcomp.CategoryReference.Load();
            }

            Category wantedCategory = catcomp.Category;

            List<ProductInUnspecifiedCategory> products = objectContext.ProductInUnspecifiedCategorySet.Where
                (pic => pic.WantedCategory.ID == wantedCategory.ID).ToList();

            if (products.Count > 0)
            {
                BusinessProduct bProduct = new BusinessProduct();

                foreach (ProductInUnspecifiedCategory product in products)
                {
                    if (!product.ProductReference.IsLoaded)
                    {
                        product.ProductReference.Load();
                    }
                    if (!product.Product.CompanyReference.IsLoaded)
                    {
                        product.Product.CompanyReference.Load();
                    }

                    if (product.Product.Company == company)
                    {
                        bProduct.ChangeProductCategory(userContext, objectContext, product.Product, wantedCategory, currUser, bLog, true);
                    }
                }
            }
        }
   
        /// <summary>
        /// Returns all companies
        /// </summary>
        public IEnumerable<Company> GetAllCompanies(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Company> companies = objectContext.CompanySet;
            return companies;
        }

        public List<Company> GetLastAddedCompanies(Entities objectContext, long number)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }

            List<Company> companies = objectContext.GetLastCompanies(number).ToList();

            return companies;
        }

        public List<Company> GetLastVisibleAddedCompanies(Entities objectContext, long number)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }

            List<Company> companies = objectContext.GetLastAddedCompanies(number).ToList();

            return companies;
        }

        /// <summary>
        /// Returns all visible=true companies sorted by Descending
        /// </summary>
        public IEnumerable<Company> GetAllVisibleCompaniesByDescending(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Company> companies = objectContext.CompanySet.Where(comp => comp.visible == true).
                OrderByDescending<Company, long>(new Func<Company, long>(IdSelector));
            return companies;
        }

        /// <summary>
        /// Returns all visivle=false companies sorted by Descending
        /// </summary>
        public IEnumerable<Company> GetAllDeletedCompaniesByDescending(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Company> companies = objectContext.CompanySet.Where(comp => comp.visible == false).
                OrderByDescending<Company, long>(new Func<Company, long>(IdSelector));
            return companies;
        }

        /// <summary>
        /// Returns all visible=true Company Characteristics
        /// </summary>
        public IEnumerable<CompanyCharacterestics> GetCompanyCharacterestics(Entities objectContext, long? companyID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<CompanyCharacterestics> companyCharacterestics;
            if ((companyID == null) || (companyID.Value < 1))
            {
                throw new BusinessException("companyID is null or < 1");
            }

            Company company = GetCompanyWV(objectContext, companyID.Value);
            if (company == null)
            {
                throw new BusinessException(string.Format("Theres no Company ID = {0}", companyID.Value));
            }
            if (company.Characterestics.Count == 0)
            {
                company.Characterestics.Load();
            }

            companyCharacterestics = company.Characterestics.Where(cc => cc.visible == true);

            return companyCharacterestics;
        }

        /// <summary>
        /// Returns number of visible=true Company characteristics
        /// </summary>
        public int CountCompanyCharacterestics(Entities objectContext, Company currCompany)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCompany == null)
            {
                throw new BusinessException("currCompany = null");
            }

            int chars = objectContext.CompanyCharacteresticsSet.Count(cc => cc.Company.ID == currCompany.ID && cc.visible == true);

            return chars;
        }

        /// <summary>
        /// Returns the number of visible=true company categories
        /// </summary>
        public int CountCompanyCategories(Entities objectContext, Company currCompany)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCompany == null)
            {
                throw new BusinessException("currCompany = null");
            }

            int categories = objectContext.CategoryCompanySet.Count(cc => cc.Company.ID == currCompany.ID && cc.visible == true);

            return categories;
        }

        /// <summary>
        /// Returns all visible=false Company Characteristics sorted by Descending
        /// </summary>
        public IEnumerable<CompanyCharacterestics> GetDeletedCompanyCharacteresticsByDescending(Entities objectContext, long? companyID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<CompanyCharacterestics> companyCharacterestics;
            if ((companyID == null) || (companyID.Value < 1))
            {
                throw new BusinessException("companyID is null or < 1");
            }

            Company company = GetCompanyWV(objectContext, companyID.Value);
            if (company == null)
            {
                throw new BusinessException(string.Format("Theres no Company ID = {0}", companyID.Value));
            }
            if (company.Characterestics.Count == 0)
            {
                company.Characterestics.Load();
            }

            companyCharacterestics = company.Characterestics.Where(cc => cc.visible == false);

            return companyCharacterestics;
        }

        /// <summary>
        /// Returns all visible=true last=true Categories in which Company have relations with
        /// </summary>
        public IEnumerable<Category> GetCompanyCategories(Entities objectContext, long? companyID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<CategoryCompany> companyCategories;
            IList<Category> compCategories = new List<Category>();

            BusinessCategory bussCategory = new BusinessCategory();

            if ((companyID == null) || (companyID.Value < 1))
            {
                throw new BusinessException("companyID is null or < 1");
            }
            else
            {
                companyCategories = objectContext.CategoryCompanySet.Where
                    (cc => cc.Company.ID == companyID && cc.visible == true && cc.Category.visible == true && cc.Category.last == true);

                foreach (CategoryCompany compcat in companyCategories)
                {
                    if (compcat.CategoryReference.IsLoaded == false)
                    {
                        compcat.CategoryReference.Load();
                    }
                    if (compcat.Category != null)
                    {
                        compCategories.Add(compcat.Category);
                    }
                }


            }
            return compCategories;
        }

        /// <summary>
        /// Returns all visible=true CategoryCompany with Company
        /// </summary>
        public List<CategoryCompany> GetAllCompanyCategories(Entities objectContext, Company currCompany)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            List<CategoryCompany> companyCategories = objectContext.CategoryCompanySet.Where
                  (cc => cc.Company.ID == currCompany.ID && cc.visible == true).ToList();
            return companyCategories;
        }

        /// <summary>
        /// Returns CompanyCategory ID
        /// </summary>
        public long GetCompanyCategoryID(Entities objectContext, Company currCompany, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);

            CategoryCompany compCat = objectContext.CategoryCompanySet.FirstOrDefault
                (cc => cc.Category.ID == currCategory.ID && cc.Company.ID == currCompany.ID);

            if (compCat == null)
            {
                throw new BusinessException(string.Format("Theres no company category with company id {0} " +
                    "and category id = {1}", currCompany.ID, currCategory.ID));
            }

            return compCat.ID;
        }

        /// <summary>
        /// Returns CompanyCategory
        /// </summary>
        /// <param name="visible">true if should check for visible only, otherwise false</param>
        public CategoryCompany GetCategoryCompany(Entities objectContext, long companyId, long categoryID, bool visible)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (companyId < 1)
            {
                throw new BusinessException("companyID is < 1");
            }
            if (categoryID < 1)
            {
                throw new BusinessException("categoryID is < 1");
            }

            CategoryCompany currCC = null;

            if (visible)
            {
                currCC = objectContext.CategoryCompanySet.FirstOrDefault<CategoryCompany>
                        (cc => cc.Category.ID == categoryID && cc.Company.ID == companyId && cc.visible == true);
            }
            else
            {
                currCC = objectContext.CategoryCompanySet.FirstOrDefault<CategoryCompany>
                        (cc => cc.Category.ID == categoryID && cc.Company.ID == companyId);
            }

            return currCC;
        }

        /// <summary>
        /// Returns company category if found
        /// </summary>
        public CategoryCompany GetCategoryCompany(Entities objectContext, Company currCompany, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            CategoryCompany currCC = objectContext.CategoryCompanySet.FirstOrDefault<CategoryCompany>
                        (cc => cc.Category.ID == currCategory.ID && cc.Company.ID == currCompany.ID);

            return currCC;
        }

        /// <summary>
        /// Returns visible=true Company with name
        /// </summary>
        public Company GetCompanyByName(Entities objectContext, String name)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }
            Company company = objectContext.CompanySet.FirstOrDefault<Company>(comp => comp.name == name && comp.visible == true);
            return company;
        }

        /// <summary>
        /// Compares two <see cref="Company"/> objects by their <c>name</c> properties.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>If a &lt; b: -1; if a &gt; b: 1; otherwise, 0.</returns>
        private int CompanyNameComparison(Company a, Company b)
        {
            if (object.ReferenceEquals(a, b) == true)
            {
                return 0;
            }
            else if (a == null)
            {
                return -1;
            }
            else if (b == null)
            {
                return 1;
            }
            else if (object.ReferenceEquals(a.name, b.name) == true)
            {
                return 0;
            }
            else if (a.name == null)
            {
                return -1;
            }
            else if (b.name == null)
            {
                return 1;
            }
            else
            {
                return string.Compare(a.name, b.name);
            }
        }

        /// <summary>
        /// Returns Companies which have Relations with Category, also adds Other company to that list. Orders companies by Name, except Other company
        /// </summary>
        public IEnumerable<Company> GetCompaniesByCategory(Entities objectContext, long? categoryID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<CategoryCompany> companyCategories;
            List<Company> companies = new List<Company>();

            if ((categoryID == null) || (categoryID.Value < 1))
            {
                throw new BusinessException("categoryID is null or < 1");
            }
            else
            {
                companyCategories = objectContext.CategoryCompanySet.Where
                    (cc => cc.Category.ID == categoryID && cc.Company.visible == true && cc.visible == true);

                foreach (CategoryCompany compcat in companyCategories)
                {
                    if (compcat.CompanyReference.IsLoaded == false)
                    {
                        compcat.CompanyReference.Load();
                    }

                    companies.Add(compcat.Company);
                }

                if (companies.Count<Company>() > 1)
                {
                    companies.Sort(new Comparison<Company>(CompanyNameComparison));
                }

                companies.Insert(0, GetOther(objectContext));
            }
            return companies;  
        }

        /// <summary>
        /// Returns CategoryCompanes which are with category
        /// </summary>
        public IEnumerable<CategoryCompany> GetCompanyCategoriesWithCategory(Entities objectContext, long categoryID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (categoryID < 1)
            {
                throw new BusinessException("categoryID is < 1");
            }

            BusinessCategory businessCategory = new BusinessCategory();
            Category category = businessCategory.GetWithoutVisible(objectContext, categoryID);
            if (category == null)
            {
                throw new BusinessException(string.Format("Theres no category ID = {0}", categoryID));
            }
            if (category.CategoryCompanies.Count == 0)
            {
                category.CategoryCompanies.Load();
            }

            IEnumerable<CategoryCompany> compCategories = category.CategoryCompanies;

            return compCategories;
        }

        /// <summary>
        /// Returns all CategoryCompanies with Company
        /// </summary>
        public IEnumerable<CategoryCompany> GetCompanyCategoriesWithCompany(Entities objectContext, long companyID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (companyID < 1)
            {
                throw new BusinessException("companyID is < 1");
            }

            IEnumerable<CategoryCompany> compCategories = null;
            Company currCompany = GetCompanyWV(objectContext, companyID);
            if (currCompany != null)
            {
                currCompany.Categories.Load();
                compCategories = currCompany.Categories;
            }

            return compCategories;
        }

        /// <summary>
        /// Makes Company visible=false
        /// </summary>
        public void DeleteCompany(EntitiesUsers userContext, Entities objectContext, Company currCompany,
            User currUser, BusinessLog bLog, bool changeProductsCompany)
        {
            Tools.AssertObjectContextExists(userContext);
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

            if (currCompany.visible == false)
            {
                throw new BusinessException(string.Format("Company ID = {0} is already visible=false , " +
                    " cant be deleted twice , there are validators checking for that before this function", currCompany.ID));
            }

            List<Product> products = new List<Product>();
            if (CheckIfCompanyHaveProductsInUnspecifiedCategory(objectContext, currCompany, out products) == true)
            {
                throw new BusinessException(string.Format("Admin id : {0} cannot delete company ID : {1}, because the company have products in Unspecified category"
                    , currUser.ID, currCompany.ID));
            }

            currCompany.visible = false;
            currCompany.lastModified = DateTime.UtcNow;
            currCompany.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogCompany(objectContext, currCompany, LogType.delete, string.Empty, string.Empty, currUser);
            BusinessStatistics stat = new BusinessStatistics();
            stat.CompanyDeleted(userContext, objectContext);

            BusinessNotifies businessNotifies = new BusinessNotifies();
            businessNotifies.RemoveNotifiesForType(objectContext, userContext, bLog, NotifyType.Company, currCompany.ID);

            BusinessUserTypeActions bUserTypeActions = new BusinessUserTypeActions();
            bUserTypeActions.RemoveAllTypeActionsForCompanyWhenDeleted(objectContext, userContext, currCompany, currUser, bLog);

            // DELETE COMPANY CATEGORIES
            List<CategoryCompany> companyCategories = GetAllCompanyCategories(objectContext, currCompany);
            if (companyCategories.Count > 0)
            {
                foreach (CategoryCompany category in companyCategories)
                {
                    DeleteCompanyCategory(objectContext, userContext, category, currUser, bLog, changeProductsCompany, false);
                }
            }

        }

        /// <summary>
        /// Makes company visible=true
        /// </summary>
        public void MakeVisibleCompany(Entities objectContext, Company currCompany, User currUser, BusinessLog bLog)
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

            if (currCompany.visible == true)
            {
                throw new BusinessException(string.Format("Company ID = {0} is already visible=true , " +
                    " cant be UNdeleted twice , there are validators checking for that before this function", currCompany.ID));
            }

            DateTime fromTime = DateTime.Now;    // gets the date when company is deleted
            DateTime toTime = DateTime.Now;
            List<Log> logs = bLog.GetLogs(objectContext, "setDeleted", "company", currCompany.ID, 1, 0);
            if (logs != null && logs.Count > 0)
            {
                fromTime = logs[0].dateCreated;
            }
            else
            {
                fromTime = currCompany.lastModified.Subtract(new TimeSpan(0, 1, 0));
            }

            toTime = fromTime.AddMinutes(3);

            currCompany.visible = true;
            currCompany.lastModified = DateTime.UtcNow;
            currCompany.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogCompany(objectContext, currCompany, LogType.undelete, string.Empty, string.Empty, currUser);

            // undelete connections
            List<CategoryCompany> companyCategories = objectContext.CategoryCompanySet.Where
                  (cc => cc.Company.ID == currCompany.ID && cc.visible == false
                      && cc.lastModified >= fromTime && cc.lastModified <= toTime).ToList();

            if (companyCategories != null && companyCategories.Count > 0)
            {
                foreach (CategoryCompany category in companyCategories)
                {
                    UnDeleteCompanyCategory(objectContext, category, currUser, bLog, fromTime, toTime);
                }
            }

        }

        /// <summary>
        /// Makes CategoryCompany visible=true
        /// </summary>
        public void MakeCompanyCategoryVisible(EntitiesUsers userContext, Entities objectContext, CategoryCompany currCompCat, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (currCompCat == null)
            {
                throw new BusinessException("Invalid company");
            }
            if (currUser == null)
            {
                throw new BusinessException("Invalid user");
            }

            if (currCompCat.visible == true)
            {
                throw new BusinessException(string.Format("CategoryCompany with ID = {0} is already visible=true , " +
                    " cant be undeleted twice , there are validators checking for that before this function", currCompCat.ID));
            }

            currCompCat.visible = true;
            currCompCat.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
            currCompCat.lastModified = DateTime.UtcNow;

            Tools.Save(objectContext);

            bLog.LogCompanyCategory(objectContext, currCompCat, LogType.undelete, currUser);

            if (!currCompCat.CompanyReference.IsLoaded)
            {
                currCompCat.CompanyReference.Load();
            }
            UpdateLastModifiedAndModifiedBy(objectContext, currCompCat.Company, currCompCat.LastModifiedBy);

            MoveUnspecifiedProductsInWantedCategory(userContext, objectContext, currCompCat, currUser, bLog);
        }

        /// <summary>
        /// Changes Company description
        /// </summary>
        public void ChangeCompanyDescription(Entities objectContext, Company currCompany, String newDescription, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompany == null)
            {
                throw new BusinessException("curr company is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }

            String oldDescription = currCompany.description;

            currCompany.description = newDescription;
            currCompany.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
            currCompany.lastModified = DateTime.UtcNow;

            Tools.Save(objectContext);

            bLog.LogCompany(objectContext, currCompany, LogType.edit, "description", oldDescription, currUser);
        }

        /// <summary>
        /// Changes company web site
        /// </summary>
        public void ChangeCompanyWebSite(Entities objectContext, Company currCompany, String newSite, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompany == null)
            {
                throw new BusinessException("curr company is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }

            if (currCompany.website != newSite)
            {
                String oldSite = currCompany.website;

                currCompany.website = newSite;
                currCompany.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                currCompany.lastModified = DateTime.UtcNow;

                Tools.Save(objectContext);

                bLog.LogCompany(objectContext, currCompany, LogType.edit, "website", oldSite, currUser);
            }
        }

        public void ChangeCompanyType(Entities objectContext, Company currCompany, CompanyType newType, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompany == null)
            {
                throw new BusinessException("curr company is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }
            if (newType == null)
            {
                throw new BusinessException("newType is null");
            }
            if (!currCompany.typeReference.IsLoaded)
            {
                currCompany.typeReference.Load();
            }
            if (currCompany.type == newType)
            {
                throw new BusinessException(string.Format("Cannot change company type to current one, user id = {0}", currUser.ID));
            }
            if (newType.visible == false)
            {
                throw new BusinessException(string.Format("Cannot change company type to newType because its not visible , user id = {0}", currUser.ID));
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.IsAdminOrGlobalAdmin(currUser))
            {
                throw new BusinessException(string.Format("User id : {0} cannot change type of company id : {1}, because he is not administrator or global administrator"
                    , currUser.ID, currCompany.ID));
            }

            string oldType = string.Format("Type name = '{0}', type id = '{1}'", currCompany.type.name, currCompany.type.ID);

            currCompany.type = newType;
            currCompany.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
            currCompany.lastModified = DateTime.UtcNow;

            Tools.Save(objectContext);

            bLog.LogCompany(objectContext, currCompany, LogType.edit, "type", oldType, currUser);

        }

        /// <summary>
        /// Changes Company name
        /// </summary>
        public void ChangeCompanyName(Entities objectContext, Company currCompany, String newName, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }
            if (newName == null || newName.Length < 1)
            {
                throw new BusinessException("newName is null or empty");
            }

            bool changeName = false;

            if (string.Equals(currCompany.name, newName, StringComparison.InvariantCultureIgnoreCase) == true
               && string.Equals(currCompany.name, newName, StringComparison.InvariantCulture) == false)
            {
                changeName = true;
            }
            else
            {
                changeName = Tools.NameValidatorPassed(objectContext, "companies", newName, currCompany.ID);
            }

            if (changeName == true)
            {
                String oldName = currCompany.name;

                currCompany.name = newName;
                currCompany.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                currCompany.lastModified = DateTime.UtcNow;

                Tools.Save(objectContext);

                bLog.LogCompany(objectContext, currCompany, LogType.edit, "name", oldName, currUser);
            }
            else
            {
                throw new BusinessException(string.Format("Company ID = {0} cannot change its name to = {1}{2}, User id : {3}",
                    currCompany.ID, newName, " because there is already company with that name", currUser.ID));
            }
        }

        public void ChangeIfUsersCanTakeActionIfThereAreNoEditors(Entities objectContext, Company currCompany,
            User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.IsAdminOrGlobalAdmin(currUser))
            {
                throw new BusinessException(string.Format("User id : {0} cannot change canUserTakeRoleIfNoEditors of company id : {1}, because he is not administrator or global administrator"
                    , currUser.ID, currCompany.ID));
            }

            bool oldValue = currCompany.canUserTakeRoleIfNoEditors;

            if (currCompany.canUserTakeRoleIfNoEditors == true)
            {
                currCompany.canUserTakeRoleIfNoEditors = false;
            }
            else
            {
                currCompany.canUserTakeRoleIfNoEditors = true;
            }

            Tools.Save(objectContext);

            bLog.LogCompany(objectContext, currCompany, LogType.edit, "canUserTakeRoleIfNoEditors", oldValue.ToString(), currUser);

        }

        /// <summary>
        /// Changes Company Characteristic name
        /// </summary>
        public void ChangeCompanyCharacteristicName(Entities objectContext, CompanyCharacterestics currCompChar,
            String newName, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompChar == null)
            {
                throw new BusinessException("curr company characteristic is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }
            if (newName == null || newName.Length < 1)
            {
                throw new BusinessException("newName is null or empty");
            }

            if (!currCompChar.CompanyReference.IsLoaded)
            {
                currCompChar.CompanyReference.Load();
            }
            if (!Tools.NameValidatorPassed(objectContext, "compChar", newName, currCompChar.Company.ID))
            {
                throw new BusinessException(string.Format("CompanyCharacteristic ID = {0} cannot change" +
                    " its name to {1} because there already exist company char with that name", currCompChar.ID, newName));
            }

            String oldName = currCompChar.name;

            currCompChar.name = newName;
            currCompChar.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
            currCompChar.lastModified = DateTime.UtcNow;

            Tools.Save(objectContext);

            bLog.LogCompanyCharacteristic(objectContext, currCompChar, LogType.edit, "name", oldName, currUser);

            if (!currCompChar.CompanyReference.IsLoaded)
            {
                currCompChar.CompanyReference.Load();
            }
            UpdateLastModifiedAndModifiedBy(objectContext, currCompChar.Company, currCompChar.LastModifiedBy);
        }

        /// <summary>
        /// Changes Company Characteristic Description
        /// </summary>
        /// <param name="newDescription">cannot be null</param>
        public void ChangeCompanyCharacteristicDescription(Entities objectContext, CompanyCharacterestics currCompChar,
            String newDescription, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompChar == null)
            {
                throw new BusinessException("curr company characteristic is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }
            if (newDescription == null || newDescription.Length < 1)
            {
                throw new BusinessException("newDescription is null or empty");
            }

            String oldDescr = currCompChar.description;

            currCompChar.description = newDescription;
            currCompChar.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
            currCompChar.lastModified = DateTime.UtcNow;

            Tools.Save(objectContext);

            bLog.LogCompanyCharacteristic(objectContext, currCompChar, LogType.edit, "description", oldDescr, currUser);

            if (!currCompChar.CompanyReference.IsLoaded)
            {
                currCompChar.CompanyReference.Load();
            }
            UpdateLastModifiedAndModifiedBy(objectContext, currCompChar.Company, currCompChar.LastModifiedBy);
        }

        /// <summary>
        /// Makes Company characteristic visible=false
        /// </summary>
        public void DeleteCompanyCharacteristic(EntitiesUsers userContext, Entities objectContext
            , CompanyCharacterestics currCompChar, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompChar == null)
            {
                throw new BusinessException("curr company characteristic is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }

            if (currCompChar.visible == false)
            {
                throw new BusinessException(string.Format("Company characteristic id = {0} is already" +
                    " visible=false, there are validators checking for that before this function", currCompChar.ID));
            }

            currCompChar.visible = false;
            currCompChar.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
            currCompChar.lastModified = DateTime.UtcNow;

            Tools.Save(objectContext);

            bLog.LogCompanyCharacteristic(objectContext, currCompChar, LogType.delete, string.Empty, string.Empty, currUser);
            BusinessStatistics stat = new BusinessStatistics();
            stat.CompCharDeleted(userContext, objectContext);

            if (!currCompChar.CompanyReference.IsLoaded)
            {
                currCompChar.CompanyReference.Load();
            }
            UpdateLastModifiedAndModifiedBy(objectContext, currCompChar.Company, currCompChar.LastModifiedBy);
        }

        /// <summary>
        /// Makes visible=false company category AND if there are company products from this category 
        /// changes their company to Other. changeProductsCompany = true if should change products in this category to Company : Other,
        /// otherwise will delete them
        /// </summary>
        /// <param name="userDeleting">True if user is deleting company category, otherwise false</param>
        public void DeleteCompanyCategory(Entities objectContext, EntitiesUsers userContext, CategoryCompany currCompCat, User currUser, BusinessLog bLog
            , bool changeProductsCompany, bool userDeleting)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompCat == null)
            {
                throw new BusinessException("curr company category is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }

            if (currCompCat.visible == false)
            {
                throw new BusinessException(string.Format("Company category id = {0} is already visible=false " +
                    ", there are valdiators checking for that before this function", currCompCat.ID));
            }

            if (!currCompCat.CompanyReference.IsLoaded)
            {
                currCompCat.CompanyReference.Load();
            }
            if (!currCompCat.CategoryReference.IsLoaded)
            {
                currCompCat.CategoryReference.Load();
            }
            BusinessCategory businessCategory = new BusinessCategory();
            Category unspecified = businessCategory.GetUnspecifiedCategory(objectContext);

            if (unspecified != currCompCat.Category)
            {
                lock (deletingCompanyCategories)
                {
                    BusinessProduct businessProduct = new BusinessProduct();

                    List<Product> allProducts = businessProduct.GetAllProductsFromCompanyWithCategory(objectContext,
                        currCompCat.Company.ID, currCompCat.Category.ID, true).ToList();

                    if (userDeleting == true && allProducts.Count > 0)
                    {
                        throw new BusinessException(string.Format("User ID : {0}, cannot detele category ID : {1} in company ID : {2}, because there are visible.true products in it."
                            , currUser.ID, currCompCat.Category.ID, currCompCat.Company.ID));
                    }
                    else
                    {
                        Company otherCompany = GetOther(objectContext);

                        if (allProducts.Count > 0)
                        {
                            foreach (Product product in allProducts)
                            {
                                if (changeProductsCompany)
                                {
                                    businessProduct.ChangeProductCompany(objectContext, product, otherCompany, currUser, bLog);
                                }
                                else
                                {
                                    businessProduct.DeleteProduct(objectContext, userContext, product, currUser, bLog);
                                }
                            }
                        }
                    }

                    currCompCat.visible = false;
                    currCompCat.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                    currCompCat.lastModified = DateTime.UtcNow;

                    Tools.Save(objectContext);

                    bLog.LogCompanyCategory(objectContext, currCompCat, LogType.delete, currUser);

                    UpdateLastModifiedAndModifiedBy(objectContext, currCompCat.Company, currCompCat.LastModifiedBy);
                }
            }
        }

        /// <summary>
        /// Undeletes company category and all deleted products in it for the same product..which were deleted at given period (used when company is being undeleted)
        /// </summary>
        public void UnDeleteCompanyCategory(Entities objectContext, CategoryCompany currCompCat, User currUser, BusinessLog bLog
            , DateTime deletedFrom, DateTime deletedTo)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompCat == null)
            {
                throw new BusinessException("curr company category is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }

            if (currCompCat.visible == true)
            {
                return;
            }

            if (deletedFrom >= deletedTo)
            {
                throw new BusinessException("deletedFrom >= deletedTo");
            }

            if (!currCompCat.CompanyReference.IsLoaded)
            {
                currCompCat.CompanyReference.Load();
            }
            if (!currCompCat.CategoryReference.IsLoaded)
            {
                currCompCat.CategoryReference.Load();
            }

            lock (undeletingCompanyCategories)
            {
                BusinessProduct businessProduct = new BusinessProduct();

                List<Product> allProducts = businessProduct.GetAllDeletedProductsFromCompanyWithCategory(objectContext,
                    currCompCat.Company.ID, currCompCat.Category.ID, deletedFrom, deletedTo, bLog).ToList();

                if (allProducts != null && allProducts.Count > 0)
                {
                    foreach (Product product in allProducts)
                    {
                        businessProduct.MakeVisibleProduct(objectContext, product, currUser, bLog);
                    }
                }

                currCompCat.visible = true;
                currCompCat.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                currCompCat.lastModified = DateTime.UtcNow;

                Tools.Save(objectContext);

                bLog.LogCompanyCategory(objectContext, currCompCat, LogType.undelete, currUser);
            }

        }


        /// <summary>
        /// Checks if Company have Connections with visible=true last=true Categories 
        /// </summary>
        /// <param name="id">Company id</param>
        public Boolean IfHaveValidCategories(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }

            Boolean result = false;

            IEnumerable<Category> compCat = GetCompanyCategories(objectContext, id);
            if (compCat.Count<Category>() > 0)
            {
                foreach (Category category in compCat)
                {
                    if (category.visible == true && category.last == true)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Function used in Sort By Descending , orders by ID
        /// </summary>
        private long IdSelector(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException("company");
            }
            return company.ID;
        }

        /// <summary>
        /// Function used in Sort By, orders by name
        /// </summary>
        private string NameSelector(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException("company");
            }
            return company.name;
        }


        /// <summary>
        /// Returns the number of visible=true (and correct connections) products added in company
        /// </summary>
        public long CountNumberOfProductsInCompany(Entities objectContext, Company currCompany)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            Company other = GetOther(objectContext);
            long num = 0;
            if (other != currCompany)
            {
                IEnumerable<Product> products = objectContext.ProductSet.Where<Product>
                    (prod => prod.Company.ID == currCompany.ID && prod.visible == true && prod.Category.visible == true);

                if (products.Count() > 0)
                {
                    BusinessProduct businessProduct = new BusinessProduct();
                    string error;

                    foreach (Product product in products)
                    {
                        if (businessProduct.CheckIfProductsIsValidWithConnections(objectContext, product, out error))
                        {
                            num++;
                        }
                    }
                }
            }
            else
            {
                num = objectContext.ProductSet.Count<Product>
                    (prod => prod.Company.ID == currCompany.ID && prod.visible == true && prod.Category.visible == true);
            }
            return num;
        }

        /// <summary>
        /// Returns number of valid company products in category
        /// </summary>
        public long CountNumberOfCompanyProductsInCategory(Entities objectContext, Company currCompany, long catId)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (catId < 1)
            {
                throw new BusinessException("catId is < 1");
            }

            Company other = GetOther(objectContext);
            long num = 0;
            if (other != currCompany)
            {
                IEnumerable<Product> products = objectContext.ProductSet.Where<Product>
                    (prod => prod.Company.ID == currCompany.ID && prod.Category.ID == catId
                        && prod.visible == true && prod.Category.visible == true);

                if (products.Count() > 0)
                {
                    BusinessProduct businessProduct = new BusinessProduct();
                    string error;

                    foreach (Product product in products)
                    {
                        if (businessProduct.CheckIfProductsIsValidWithConnections(objectContext, product, out error))
                        {
                            num++;
                        }
                    }
                }
            }
            else
            {
                num = objectContext.ProductSet.Count<Product>
                    (prod => prod.Company.ID == currCompany.ID && prod.Category.ID == catId
                        && prod.visible == true && prod.Category.visible == true);
            }
            return num;
        }

        /// <summary>
        /// Checks if company can add products in category, if company is OTher returns true
        /// </summary>
        /// <returns>true if it can , otherwise false</returns>
        public Boolean CheckIfCompanyCanHaveProductsInCategory(Entities objectContext, Company currCompany, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            Boolean result = false;

            Company other = GetOther(objectContext);
            if (currCompany.ID != other.ID)
            {
                CategoryCompany newCompanyCategory = objectContext.CategoryCompanySet.FirstOrDefault
                                (cc => cc.Company.ID == currCompany.ID && cc.Category.ID == product.Category.ID &&
                                    cc.visible == true && currCompany.visible == true);

                if (newCompanyCategory != null)
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks if company can add products in category, if company is Other returns true
        /// </summary>
        /// <returns>true if it can , otherwise false</returns>
        public Boolean CheckIfCompanyCanHaveProductsInCategory(Entities objectContext, Company currCompany, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            Boolean result = false;

            Company other = GetOther(objectContext);
            if (currCompany.ID == other.ID)
            {
                result = true;
            }
            else
            {
                CategoryCompany newCompanyCategory = objectContext.CategoryCompanySet.FirstOrDefault
                           (cc => cc.Company.ID == currCompany.ID && cc.Category.ID == currCategory.ID &&
                               cc.visible == true && currCompany.visible == true);

                if (newCompanyCategory != null)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// True if selected category have subcategories which can be added to company, otherwise false
        /// </summary>
        public Boolean CheckIfCategoryHaveNotAddedLastCategories(Entities objectContext, Company currCompany, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            bool result = false;

            BusinessCategory businessCategory = new BusinessCategory();
            CategoryCompany currCompCat = null;

            List<Category> categoriesToProceed = new List<Category>();

            if (currCategory.last == true)
            {
                currCompCat = objectContext.CategoryCompanySet.FirstOrDefault<CategoryCompany>
                               (cc => cc.Category.ID == currCategory.ID && cc.Company.ID == currCompany.ID && cc.visible == true);
                if (currCompCat == null)
                {
                    result = true;
                }
            }
            else
            {
                List<Category> subCategories = businessCategory.GetAllSubCategories(objectContext, currCategory, true, false).ToList();
                if (subCategories.Count > 0)
                {
                    foreach (Category category in subCategories)
                    {
                        if (category.last == true)
                        {
                            currCompCat = objectContext.CategoryCompanySet.FirstOrDefault<CategoryCompany>
                                (cc => cc.Category.ID == category.ID && cc.Company.ID == currCompany.ID && cc.visible == true);
                            if (currCompCat == null)
                            {
                                result = true;
                                break;
                            }
                        }
                        else
                        {
                            if (businessCategory.CountVisibleSubCategories(objectContext, category) > 0)
                            {
                                categoriesToProceed.Add(category);
                            }
                        }
                    }
                }
            }

            if (result == false && categoriesToProceed.Count > 0)
            {
                foreach (Category category in categoriesToProceed)
                {
                    if (CheckIfCategoryHaveNotAddedLastCategories(objectContext, currCompany, category))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if company have products (doesnt check for visibility) which are in unspecified category, True if there are..
        /// </summary>
        public bool CheckIfCompanyHaveProductsInUnspecifiedCategory(Entities objectContext, Company currCompany, out List<Product> products)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            BusinessCategory bCategory = new BusinessCategory();
            Category unspecified = bCategory.GetUnspecifiedCategory(objectContext);

            products = objectContext.ProductSet.Where(prod => prod.Company.ID == currCompany.ID
                && prod.Category.ID == unspecified.ID).ToList();

            if (products != null && products.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Returns true if Configuration.CompaniesTimeWhichNeedsToPassToAddAnother time passed after last added company by user, always true for admins
        /// </summary>
        public bool CheckIfMinimumTimeBetweenAddingCompaniesPassed(Entities objectContext, User currUser, out int minToWait)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser in null");
            }

            minToWait = 1;
            bool result = true;

            if (Configuration.CompaniesTimeWhichNeedsToPassToAddAnother > 0)
            {
                BusinessUser businessUser = new BusinessUser();
                if (businessUser.IsFromAdminTeam(currUser))
                {
                    return true;
                }

                Company lastAddedByUser = null;
                List<Company> companiesAddedbyUser = objectContext.CompanySet.Where(comp => comp.CreatedBy.ID == currUser.ID).ToList();
                if (companiesAddedbyUser.Count > 0)
                {
                    lastAddedByUser = companiesAddedbyUser.Last();
                }

                if (lastAddedByUser != null)
                {
                    DateTime prodTime = lastAddedByUser.dateCreated;

                    TimeSpan span = DateTime.UtcNow - lastAddedByUser.dateCreated;
                    int minPassed = (int)span.TotalMinutes;

                    if (minPassed < Configuration.CompaniesTimeWhichNeedsToPassToAddAnother)
                    {
                        result = false;

                        minToWait = Configuration.CompaniesTimeWhichNeedsToPassToAddAnother - minPassed;
                        if (minToWait == 0)
                        {
                            minToWait = 1;
                        }
                    }
                }

            }

            return result;
        }

        /// <summary>
        /// Updates company`s LastModified and ModifiedBy properties. Should be called when company images/characteristics/categories are modified
        /// </summary>
        public void UpdateLastModifiedAndModifiedBy(Entities objectContext, Company currCompany, UserID userID)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (userID == null)
            {
                throw new BusinessException("userID is null");
            }

            currCompany.lastModified = DateTime.UtcNow;
            currCompany.LastModifiedBy = userID;
            Tools.Save(objectContext);
        }


        public List<Company> GetLastDeletedCompanies(Entities objectContext, int number, string nameContains, long compId, User byUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<Company> companies = new List<Company>();

            if (byUser != null)
            {
                if (!string.IsNullOrEmpty(nameContains))
                {
                    if (compId > 0)
                    {
                        companies = objectContext.GetLastDeletedCompanies(nameContains, (long)number, byUser.ID, compId).ToList();
                    }
                    else
                    {
                        companies = objectContext.GetLastDeletedCompanies(nameContains, (long)number, byUser.ID, null).ToList();
                    }
                }
                else
                {
                    if (compId > 0)
                    {
                        companies = objectContext.GetLastDeletedCompanies(null, (long)number, byUser.ID, compId).ToList();
                    }
                    else
                    {
                        companies = objectContext.GetLastDeletedCompanies(null, (long)number, byUser.ID, null).ToList();
                    }
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(nameContains))
                {
                    if (compId > 0)
                    {
                        companies = objectContext.GetLastDeletedCompanies(nameContains, (long)number, null, compId).ToList();
                    }
                    else
                    {
                        companies = objectContext.GetLastDeletedCompanies(nameContains, (long)number, null, null).ToList();
                    }
                }
                else
                {
                    if (compId > 0)
                    {
                        companies = objectContext.GetLastDeletedCompanies(null, (long)number, null, compId).ToList();
                    }
                    else
                    {
                        companies = objectContext.GetLastDeletedCompanies(null, (long)number, null, null).ToList();
                    }
                }
            }

            return companies;
        }

    }
}
