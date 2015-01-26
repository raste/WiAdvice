// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessCategory
    {

        /// <summary>
        /// Returns all categories , doesnt check for visible
        /// </summary>
        public IEnumerable<Category> GetAll(Entities objectContext)         
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Category> categories = objectContext.CategorySet;
            return categories;
        }

        /// <summary>
        /// Returns all categories , sorted By Descending , doesnt check for visible
        /// </summary>
        public IEnumerable<Category> GetAllbyDescending(Entities objectContext)           
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Category> categories = objectContext.CategorySet.OrderByDescending<Category, long>(new Func<Category, long>(IdSelector));
            return categories;
        }

        /// <summary>
        /// Returns all visible Categories , sorted by Descending
        /// </summary>
        public IEnumerable<Category> GetAllVisiblebyDescending(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Category> categories =
                objectContext.CategorySet.Where(cat => cat.visible == true).
                OrderByDescending<Category, long>(new Func<Category, long>(IdSelector));
            return categories;
        }

        /// <summary>
        /// Returns all visible=false Categories , sorted by Descending
        /// </summary>
        public IEnumerable<Category> GetAllDeletedbyDescending(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Category> categories =
                objectContext.CategorySet.Where(cat => cat.visible == false).
                OrderByDescending<Category, long>(new Func<Category, long>(IdSelector));
            return categories;
        }

        public IEnumerable<Category> GetAllDeleted(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Category> categories =
                objectContext.CategorySet.Where(cat => cat.visible == false);
            return categories;
        }

        /// <summary>
        /// Returns all Categories which have parent category and are visible=true
        /// </summary>
        /// <param name="parentCategoryID">ID of the parent Category (can be null)</param>
        public List<Category> GetAllByParentCategoryID(Entities objectContext, long? parentCategoryID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Category> categories;
            if (parentCategoryID == null)
            {
                categories =
                    objectContext.CategorySet.Where(cat => cat.parentID == null && cat.visible == true);
            }
            else
            {
                // Sorts first by DisplayOrder and then by name
                categories = objectContext.GetCategorySubCategories(parentCategoryID, true);
            }
            return categories.ToList();
        }

        /// <summary>
        /// Add`s new Category to Category table
        /// </summary>
        public void Add(EntitiesUsers userContext, Entities objectContext, Category newCategory, BusinessLog bLog, User currUser)
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
            objectContext.AddToCategorySet(newCategory);
            Tools.Save(objectContext);
            bLog.LogCategory(objectContext, newCategory, LogType.create, string.Empty, string.Empty, currUser);
            BusinessStatistics stat = new BusinessStatistics();
            stat.CategoryCreated(userContext, objectContext);
        }

        /// <summary>
        /// Returns Category visible=true with ID , null if theres no such
        /// </summary>
        public Category Get(Entities objectContext, long? id)
        {
            if (id == null || id < 1)
            {
                throw new BusinessException("id is null or < 1");
            }

            Tools.AssertObjectContextExists(objectContext);
            Category category = objectContext.CategorySet.FirstOrDefault<Category>(cat => cat.ID == id && cat.visible == true);
            return category;
        }

        /// <summary>
        /// returns 'unspecified' category for the current language variant
        /// </summary>
        public Category GetUnspecifiedCategory(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            string strID = Tools.GetConfigurationResource("UnspecifiedCategoryID");
            long id = 0;
            if (long.TryParse(strID, out id))
            {
                Category category = objectContext.CategorySet.FirstOrDefault<Category>(cat => cat.ID == id && cat.visible == true);

                if (category == null)
                {
                    throw new BusinessException(string.Format("Couldn`t find (visible = true) Unspecified category with id = {0}", id));
                }

                return category;
            }
            else
            {
                throw new BusinessException(string.Format("Couldn`t parse Configuration resource 'UnspecifiedCategoryID' = '{0}' to long."
                    , strID));
            }
        }

        /// <summary>
        /// Returns Category  with ID , null if theres no such , doesnt check for visible
        /// </summary>
        public Category GetWithoutVisible(Entities objectContext, long id) 
        {
            Tools.AssertObjectContextExists(objectContext);
            Category category = objectContext.CategorySet.FirstOrDefault<Category>(cat => cat.ID == id);
            return category;
        }

        /// <summary>
        /// Returns the name of category with ID which is visible=true
        /// </summary>
        public String GetNameByID(Entities objectContext, long? categoryID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (categoryID == null || categoryID < 1)
            {
                throw new BusinessException("categoryID is null or < 1");
            }
            Category category = objectContext.CategorySet.FirstOrDefault<Category>(cat => cat.ID == categoryID && cat.visible == true);
            if (category == null)
            {
                throw new BusinessException(string.Format("Theres no visible Category with id = {0}", categoryID));
            }
            String name = category.name;
            return name;
        }

        /// <summary>
        /// Returns all categories which are Created by USer with ID 
        /// </summary>
        public IEnumerable<Category> GetAllCreatedBy(Entities objectContext, long? userid)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Category> categories;
            if ((userid == null) || (userid.Value < 1))
            {
                throw new BusinessException("userid is null or < 1");
            }
            else
            {
                BusinessUser businessUser = new BusinessUser();
                UserID user = Tools.GetUserID(objectContext, userid.Value, true);

                user.CategoriesCreated.Load();

                categories = user.CategoriesCreated;
            }
            return categories;
        }

        /// <summary>
        /// Returns all 'visible' Sub Categories of currCategory
        /// </summary>
        /// <param name="visible">true if should get only visible=true , false for visible=false</param>
        /// <param name="all">true if should get all found categories (visible true and false)</param>
        public IEnumerable<Category> GetAllSubCategories(Entities objectContext, Category currCategory, bool visible, bool all)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Category> categories;
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            if (all)
            {
                categories = objectContext.CategorySet.Where
                (cat => cat.parentID == currCategory.ID);
            }
            else
            {
                categories = objectContext.CategorySet.Where
                (cat => cat.parentID == currCategory.ID && cat.visible == visible);
            }

            return categories;
        }

        /// <summary>
        /// Returns count of found visible sub categories
        /// </summary>
        public int CountVisibleSubCategories(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            int count = objectContext.CategorySet.Count(cat => cat.parentID == currCategory.ID && cat.visible == true);

            return count;
        }

        /// <summary>
        /// Makes visible=false currCategory and all her Sub Categories and all Company Categories which are connected with them
        /// </summary>
        /// <param name="currCategory">Category which is being Deleted</param>
        public void DeleteCategoryWithEveryConnection(EntitiesUsers userContext, Entities objectContext, Category currCategory, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currCategory.visible == true)
            {
                currCategory.visible = false;
                currCategory.lastModified = DateTime.UtcNow;
                currCategory.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                bLog.LogCategory(objectContext, currCategory, LogType.delete, string.Empty, string.Empty, currUser);
                BusinessStatistics stat = new BusinessStatistics();
                stat.CategoryDeleted(userContext, objectContext);


                if (!currCategory.last)
                {
                    IEnumerable<Category> subCategories = GetAllSubCategories(objectContext, currCategory, true, false);

                    if (subCategories.Count<Category>() > 0)
                    {
                        List<Category> categoriesList = new List<Category>();
                        foreach (Category category in subCategories)
                        {
                            categoriesList.Add(category);
                        }

                        foreach (Category category in categoriesList)
                        {
                            DeleteCategoryWithEveryConnection(userContext, objectContext, category, currUser, bLog);
                        }
                    }
                }
                else
                {
                    DeleteCompanyCategoriesWithCategory(objectContext, currCategory, currUser, bLog);
                }
            }
            else
            {
                throw new BusinessException(string.Format("Category ID = {0} is already visible=false , cant be deleted twice", currCategory.ID));
            }
        }

        /// <summary>
        /// Deletes all CompanyCategories which have currCategory
        /// </summary>
        private void DeleteCompanyCategoriesWithCategory(Entities objectContext, Category currCategory, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            BusinessCompany businessCompany = new BusinessCompany();
            IEnumerable<CategoryCompany> companyCategories =
                businessCompany.GetCompanyCategoriesWithCategory(objectContext, currCategory.ID);
            if (companyCategories.Count<CategoryCompany>() > 0)
            {
                foreach (CategoryCompany catCompany in companyCategories)
                {
                    catCompany.visible = false;
                    catCompany.lastModified = DateTime.UtcNow;
                    catCompany.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                    bLog.LogCompanyCategory(objectContext, catCompany, LogType.delete, currUser);
                }
            }
        }

        /// <summary>
        /// Makes Visible=true Category and all her sub categories and all CompanyCategories which 
        /// are connected with THem (opposite of DeleteCategoryWithEveryConnection)
        /// </summary>
        /// <param name="currCategory">Category which is being Undeleted</param>
        public void MakeVisibleCategoryWithEveryConnection(Entities objectContext, Category currCategory, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (!CheckIfParentCategoryIsOK(objectContext, currCategory))
            {
                throw new BusinessException(string.Format("Category ID = {0} cannot be edited because parent category is Last, User ID = {1}",
                    currCategory.ID, currUser.ID));
            }

            if (currCategory.visible == false)
            {
                currCategory.visible = true;
                currCategory.lastModified = DateTime.UtcNow;
                currCategory.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                bLog.LogCategory(objectContext, currCategory, LogType.undelete, string.Empty, string.Empty, currUser);

                if (!currCategory.last)
                {
                    IEnumerable<Category> subCategories = GetAllSubCategories(objectContext, currCategory, false, false);
                    if (subCategories.Count<Category>() > 0)
                    {
                        List<Category> categoriesList = new List<Category>();
                        foreach (Category category in subCategories)
                        {
                            categoriesList.Add(category);
                        }

                        foreach (Category category in categoriesList)
                        {
                            MakeVisibleCategoryWithEveryConnection(objectContext, category, currUser, bLog);
                        }
                    }
                }
                else
                {
                    MakeVisibleCompanyCategoriesWithCategory(objectContext, currCategory, currUser, bLog);
                }
            }
            else
            {
                throw new BusinessException(string.Format("Category Id = {0} is already visible=true , cant be undeleted", currCategory.ID));
            }
        }

        /// <summary>
        /// Makes visible all CompanyCategories which are with currCategory only if CompanyCategories.Company.visible = true
        /// </summary>
        private void MakeVisibleCompanyCategoriesWithCategory(Entities objectContext, Category currCategory, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            BusinessCompany businessCompany = new BusinessCompany();
            IEnumerable<CategoryCompany> companyCategories = objectContext.CategoryCompanySet.Where
                (cc => cc.Category.ID == currCategory.ID && cc.Company.visible == true);

            if (companyCategories.Count<CategoryCompany>() > 0)
            {

                List<CategoryCompany> catCompList = new List<CategoryCompany>();
                foreach (CategoryCompany catCompany in companyCategories)
                {
                    catCompList.Add(catCompany);
                }

                foreach (CategoryCompany catCompany in catCompList)
                {
                    catCompany.visible = true;
                    catCompany.lastModified = DateTime.UtcNow;
                    catCompany.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                    bLog.LogCompanyCategory(objectContext, catCompany, LogType.undelete, currUser);
                }
            }
        }

        /// <summary>
        /// Changes Category name with new one
        /// </summary>
        /// <param name="newName">cannot be null or empty , also there shouldnt be other category with same name</param>
        /// <param name="modifiedBy">user which is performing this operation</param>
        public void ChangeCategoryName(Entities objectContext, Category currCategory, string newName, User modifiedBy, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }
            if (newName == null || newName.Length < 1)
            {
                throw new BusinessException("invalid new name");
            }
            if (modifiedBy == null)
            {
                throw new BusinessException("modifiedBy is null");
            }
            if (!Tools.NameValidatorPassed(objectContext, "categories", newName, 0))
            {
                throw new BusinessException(string.Format("Category with name = {0} ID = {1}" +
                " cannot change name with = {2} , because there is already category with that name" +
                " , there are validators checking for that before entering that function",
                currCategory.name, currCategory.ID, newName));
            }

            string oldName = currCategory.name;

            currCategory.name = newName;
            currCategory.lastModified = DateTime.UtcNow;
            currCategory.LastModifiedBy = Tools.GetUserID(objectContext, modifiedBy);

            Tools.Save(objectContext);

            bLog.LogCategory(objectContext, currCategory, LogType.edit, "name", oldName, modifiedBy);
        }

        /// <summary>
        /// Changes Category description with newDescription
        /// </summary>
        public void ChangeCategoryDescription(Entities objectContext, Category currCategory, string newDescription, User modifiedBy, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCategory == null)
            {
                throw new BusinessException("curr category is null");
            }

            if (modifiedBy == null)
            {
                throw new BusinessException("invalid curr user");
            }

            string oldDescription = currCategory.description;

            currCategory.description = newDescription;
            currCategory.lastModified = DateTime.UtcNow;
            currCategory.LastModifiedBy = Tools.GetUserID(objectContext, modifiedBy);

            Tools.Save(objectContext);

            bLog.LogCategory(objectContext, currCategory, LogType.edit, "description", oldDescription, modifiedBy);
        }

        public void ChangeParentCategory(Entities objectContext, Category currCategory, Category parentCategory, User modifiedBy, BusinessLog bLog)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            if (modifiedBy == null)
            {
                throw new BusinessException("modifiedBy is null");
            }
            if (parentCategory == null)
            {
                throw new BusinessException("parentCategory is null");
            }
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            string oldparent = currCategory.parentID.ToString();

            currCategory.parentID = parentCategory.ID;
            currCategory.lastModified = DateTime.UtcNow;
            currCategory.LastModifiedBy = Tools.GetUserID(objectContext, modifiedBy);

            Tools.Save(objectContext);

            bLog.LogCategory(objectContext, currCategory, LogType.edit, "parent category", oldparent, modifiedBy);
        }

        public void ChangeDisplayOrder(Entities objectContext, Category currCategory, int displayOrder, User modifiedBy, BusinessLog bLog)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            if (modifiedBy == null)
            {
                throw new BusinessException("modifiedBy is null");
            }
            if (displayOrder < 0 || displayOrder > 255)
            {
                throw new BusinessException("displayOrder is < 0 or > 255");
            }
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            if (displayOrder != currCategory.displayOrder)
            {
                int oldNumber = currCategory.displayOrder;

                currCategory.displayOrder = displayOrder;
                currCategory.lastModified = DateTime.UtcNow;
                currCategory.LastModifiedBy = Tools.GetUserID(objectContext, modifiedBy);

                Tools.Save(objectContext);

                bLog.LogCategory(objectContext, currCategory, LogType.edit, "display order", oldNumber.ToString(), modifiedBy);
            }
        }

        /// <summary>
        /// Makes Category Last , throws exceptions if it is already last or if it have visible sub categories
        /// </summary>
        public void MakeCategoryLast(Entities objectContext, Category currCategory, User modifiedBy, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCategory == null)
            {
                throw new BusinessException("curr category is null");
            }
            if (modifiedBy == null)
            {
                throw new BusinessException("modifiedBy is null");
            }

            if (IfHaveSubCategory(objectContext, currCategory))
            {
                throw new BusinessException(string.Format("Category with ID = {0} , cannot be last " +
                            " because she have sub category/es which are visible=true", currCategory.ID));
            }

            if (currCategory.last)
            {
                throw new BusinessException(string.Format("Category={0} ID={1} is already last , there are" +
                    "valdiators checking for that before this function", currCategory.name, currCategory.ID));
            }

            if (!CheckIfParentCategoryIsOK(objectContext, currCategory))
            {
                throw new BusinessException(string.Format("Category ID = {0} cannot be edited because parent category is Last, User ID = {1}",
                    currCategory.ID, modifiedBy.ID));
            }

            string oldValue = currCategory.last.ToString();

            currCategory.last = true;
            currCategory.lastModified = DateTime.UtcNow;
            currCategory.LastModifiedBy = Tools.GetUserID(objectContext, modifiedBy);

            Tools.Save(objectContext);

            bLog.LogCategory(objectContext, currCategory, LogType.edit, "last", oldValue, modifiedBy);

        }

        /// <summary>
        /// Changes Category.last to false , throws exception if its already last=false
        /// </summary>
        public void UnMakeCategoryLast(Entities objectContext, Category currCategory, User modifiedBy, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCategory == null)
            {
                throw new BusinessException("curr category is null");
            }
            if (modifiedBy == null)
            {
                throw new BusinessException("modifiedBy is null");
            }
            if (currCategory.last == false)
            {
                throw new BusinessException(string.Format("Category ID={0} is already last=false , " +
                    "there are validators checking for that before this function", currCategory.ID));
            }

            string oldValue = currCategory.last.ToString();

            currCategory.last = false;
            currCategory.lastModified = DateTime.UtcNow;
            currCategory.LastModifiedBy = Tools.GetUserID(objectContext, modifiedBy);

            Tools.Save(objectContext);

            bLog.LogCategory(objectContext, currCategory, LogType.edit, "last", oldValue, modifiedBy);
        }

        /// <summary>
        /// function used to sort Categories by Descending , sorts them by ID
        /// </summary>
        private long IdSelector(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException("statistic");
            }
            return category.ID;
        }

        /// <summary>
        /// Checks if Category have sub categories which are visible=true
        /// </summary>
        /// <returns>true if there are sub categories , otherwise false</returns>
        public Boolean IfHaveSubCategory(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            Boolean result = false;

            Category subCategory = objectContext.CategorySet.FirstOrDefault
                        (sc => sc.parentID == currCategory.ID && sc.visible == true);

            if (subCategory != null)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks if Category Have connections with companies and/or products
        /// </summary>
        /// <returns>true if it have connections , otherwise false</returns>
        public Boolean IfHaveConnections(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            Boolean result = true;

            CategoryCompany catCompany = objectContext.CategoryCompanySet.FirstOrDefault
                            (cc => cc.Category.ID == currCategory.ID && cc.visible == true);
            Product prod = objectContext.ProductSet.FirstOrDefault
                (pr => pr.Category.ID == currCategory.ID && pr.visible == true);

            if (catCompany == null && prod == null)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Returns number of Products which are in Category
        /// </summary>
        public long NumberOfProductsInCategory(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            long num = objectContext.ProductSet.Count<Product>
                (prod => prod.Category.ID == currCategory.ID && prod.visible == true && prod.Company.visible == true);

            return num;
        }

        /// <summary>
        /// Returns all Parent Categories of currCategory
        /// </summary>
        public List<Category> GetAllParentCategories(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            List<Category> parentCategories = new List<Category>();

            int i = 1;
            while (currCategory.parentID != null && i < 100)
            {
                currCategory = GetWithoutVisible(objectContext, currCategory.parentID.Value);
                if (currCategory != null)
                {
                    parentCategories.Add(currCategory);
                }
                else
                {
                    break;
                }
                i++;
            }
            if (i >= 100)
            {
                throw new BusinessException("couldnt get parent categories for 100 cycles");
            }

            return parentCategories;
        }

        /// <summary>
        /// Returns parent category of current category
        /// </summary>
        public Category GetParentCategory(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            Category parentCategory = null;

            if (currCategory.parentID != null)
            {
                parentCategory = GetWithoutVisible(objectContext, currCategory.parentID.Value);
            }

            return parentCategory;
        }

        /// <summary>
        /// Checks if parent category is last, true if its not ,otherwise false (Used for editing subcategory)
        /// </summary>
        public Boolean CheckIfParentCategoryIsOK(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            bool passed = true;

            Category parentCategory = GetParentCategory(objectContext, currCategory);

            if (parentCategory != null && parentCategory.last == true)
            {
                passed = false;
            }

            return passed;
        }

        /// <summary>
        /// function used to sort Categories by ascending , sorts them by name
        /// </summary>
        private string NameSelector(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException("statistic");
            }
            return category.name;
        }

        /// <summary>
        /// Returns true if there aren`t products (in Others -> Varios) with wanted category 'currCategory', otherwise false 
        /// </summary>
        public bool CheckIfCategoryIsntWantedForProducts(Entities objectContext, Category currCategory, out string error)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }
            error = string.Empty;
            bool result = true;

            Category unspecified = GetUnspecifiedCategory(objectContext);

            if (currCategory != unspecified)
            {
                int count = objectContext.ProductInUnspecifiedCategorySet
                    .Count(pr => pr.WantedCategory.ID == currCategory.ID);

                if (count > 0)
                {
                    result = false;
                    error = "There are products in category 'Others > Various' with wanted category current, there need to be none in order this action to execute.";
                }
            }
            else
            {
                result = false;
                error = "This action cannot be executed for this category.";
            }

            return result;
        }

        public List<Category> GetLastDeletedCategories(Entities objectContext, int number, string nameContains, long catId, User byUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<Category> categories = new List<Category>();

            if (byUser != null)
            {
                if (!string.IsNullOrEmpty(nameContains))
                {
                    if (catId > 0)
                    {
                        categories = objectContext.GetLastDeletedCategories(nameContains, (long)number, byUser.ID, catId).ToList();
                    }
                    else
                    {
                        categories = objectContext.GetLastDeletedCategories(nameContains, (long)number, byUser.ID, null).ToList();
                    }
                }
                else
                {
                    if (catId > 0)
                    {
                        categories = objectContext.GetLastDeletedCategories(null, (long)number, byUser.ID, catId).ToList();
                    }
                    else
                    {
                        categories = objectContext.GetLastDeletedCategories(null, (long)number, byUser.ID, null).ToList();
                    }
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(nameContains))
                {
                    if (catId > 0)
                    {
                        categories = objectContext.GetLastDeletedCategories(nameContains, (long)number, null, catId).ToList();
                    }
                    else
                    {
                        categories = objectContext.GetLastDeletedCategories(nameContains, (long)number, null, null).ToList();
                    }
                }
                else
                {
                    if (catId > 0)
                    {
                        categories = objectContext.GetLastDeletedCategories(null, (long)number, null, catId).ToList();
                    }
                    else
                    {
                        categories = objectContext.GetLastDeletedCategories(null, (long)number, null, null).ToList();
                    }
                }
            }

            return categories;
        }

    }

}
