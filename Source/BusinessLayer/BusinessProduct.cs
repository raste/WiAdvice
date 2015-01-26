// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessProduct
    {
        private static object commentsSync = new object();

        /// <summary>
        /// Adds new Product to database
        /// </summary>
        private void Add(EntitiesUsers userContext, Entities objectContext, Product newProduct, BusinessLog bLog, User currUser, Category wantedCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (newProduct == null)
            {
                throw new ArgumentNullException("newProduct");
            }
            objectContext.AddToProductSet(newProduct);
            Tools.Save(objectContext);

            bLog.LogProduct(objectContext, newProduct, LogType.create, string.Empty, string.Empty, currUser);

            BusinessUserTypeActions businessUserTypeActions = new BusinessUserTypeActions();
            businessUserTypeActions.AddNewProductActions(userContext, objectContext, currUser, bLog, newProduct);

            BusinessStatistics businessStatistic = new BusinessStatistics();
            businessStatistic.ProductCreated(userContext, objectContext);

            if (!newProduct.CompanyReference.IsLoaded)
            {
                newProduct.CompanyReference.Load();
            }

            BusinessCompany businessCompany = new BusinessCompany();
            if (businessCompany.GetOther(objectContext) != newProduct.Company)
            {
                BusinessNotifies businessNotifies = new BusinessNotifies();
                businessNotifies.UpdateNotifiesForType(objectContext, NotifyType.Company, newProduct.Company.ID, currUser);
            }

            AddInUnspecifiedCategory(userContext, objectContext, newProduct, wantedCategory, currUser);
        }

        /// <summary>
        /// Id wantedCategory != null, add new row in ProductInUnspecifiedCategory table and sends system messages to company editors
        /// </summary>
        private static void AddInUnspecifiedCategory(EntitiesUsers userContext, Entities objectContext, Product newProduct
            , Category wantedCategory, User userAdding)
        {
            // Adding ProductInUnspecifiedCategory and sending system messages to editors
            if (wantedCategory != null)
            {
                if (!newProduct.CategoryReference.IsLoaded)
                {
                    newProduct.CategoryReference.Load();
                }

                ProductInUnspecifiedCategory productInUnsCat = new ProductInUnspecifiedCategory();

                productInUnsCat.Product = newProduct;
                productInUnsCat.UnspecifiedCategory = newProduct.Category;
                productInUnsCat.WantedCategory = wantedCategory;

                objectContext.AddToProductInUnspecifiedCategorySet(productInUnsCat);
                Tools.Save(objectContext);

                // System message to company editors
                BusinessUserTypeActions butActions = new BusinessUserTypeActions();
                Company company = newProduct.Company;
                List<UsersTypeAction> actions = butActions.GetCompanyModificators(objectContext, company.ID).ToList();
                if (actions.Count > 0)
                {
                    BusinessUser bUser = new BusinessUser();
                    List<User> editors = new List<User>();
                    User editor = null;
                    foreach (UsersTypeAction action in actions)
                    {
                        if (!action.UserReference.IsLoaded)
                        {
                            action.UserReference.Load();
                        }

                        editor = bUser.Get(userContext, action.User.ID, true);
                        editors.Add(editor);
                        editor = null;
                    }

                    if (editors.Count > 0)
                    {
                        BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();

                        if (!newProduct.CompanyReference.IsLoaded)
                        {
                            newProduct.CompanyReference.Load();
                        }

                        // "[User] added product [Product] to company [Company] in unspecified category. Wanted category for this product is [wanted categgory], if you forget and ...";
                        string description = string.Format("{0} {1} '{2}' {3} '{4}' {5} {6} '{7}' {8}", userAdding.username, Tools.GetResource("SMtoEditorForAddOfUnsProduct"),
                            newProduct.name, Tools.GetResource("SMtoEditorForAddOfUnsProduct1"), newProduct.Company.name,
                            Tools.GetResource("SMtoEditorForAddOfUnsProduct2"), Tools.GetResource("SMtoEditorForAddOfUnsProduct3"),
                        Tools.CategoryName(objectContext, wantedCategory, false), Tools.GetResource("SMtoEditorForAddOfUnsProduct4"));

                        foreach (User user in editors)
                        {
                            bSystemMessages.Add(userContext, user, description);
                        }
                    }

                }
            }
        }

        public void AddProduct(EntitiesUsers userContext, Entities objectContext, BusinessLog bLog, Company currCompany
            , Category currCategory, User currUser, string name, string description, string site, Category wantedCategory)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty");
            }

            // important check
            CheckIfNewProductWillBeValidWithConnections(userContext, objectContext, currCompany
                , currCategory, currUser, name, wantedCategory);

            Product newProduct = new Product();

            newProduct.name = name;
            newProduct.description = description;

            if (string.IsNullOrEmpty(site))
            {
                newProduct.website = null;
            }
            else
            {
                newProduct.website = Tools.GetCorrectedUrl(site);
            }

            newProduct.Company = currCompany;
            newProduct.Category = currCategory;

            newProduct.CreatedBy = Tools.GetUserID(objectContext, currUser);
            newProduct.LastModifiedBy = newProduct.CreatedBy;

            newProduct.rating = 0;
            newProduct.visible = true;
            newProduct.dateCreated = DateTime.UtcNow;
            newProduct.lastModified = newProduct.dateCreated;
            newProduct.usersRated = 0;
            newProduct.comments = 0;

            newProduct.canUserTakeRoleIfNoEditors = Configuration.ProductsCanUserTakeRoleIfNoEditors;

            Add(userContext, objectContext, newProduct, bLog, currUser, wantedCategory);
        }

        /// <summary>
        /// Adds new product characteristic to database
        /// </summary>
        public void AddProductCharacteristic(EntitiesUsers userContext, Entities objectContext
            , Product product, BusinessLog bLog, User currUser, string name, string description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (product == null)
            {
                throw new ArgumentNullException("product");
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

            if (!Tools.NameValidatorPassed(objectContext, "prodChar", name, product.ID))
            {
                throw new BusinessException(string.Format("User ID : {0} cannot add characteristic with name : {1} for product ID : {2}, because there is already with such name."
                    , currUser.ID, product.ID, name));
            }

            ProductCharacteristics testChar = objectContext.ProductCharacteristicsSet.FirstOrDefault
                (pc => pc.Product.ID == product.ID && pc.name == name);

            if (testChar == null)
            {
                ProductCharacteristics newPChar = new ProductCharacteristics();

                newPChar.CreatedBy = Tools.GetUserID(objectContext, currUser);
                newPChar.Product = product;
                newPChar.name = name;
                newPChar.description = description;
                newPChar.dateCreated = DateTime.UtcNow;
                newPChar.visible = true;
                newPChar.LastModifiedBy = newPChar.CreatedBy;
                newPChar.lastModified = newPChar.dateCreated;
                newPChar.comments = 0;

                objectContext.AddToProductCharacteristicsSet(newPChar);
                Tools.Save(objectContext);

                UpdateLastModifiedAndModifiedBy(objectContext, product, newPChar.CreatedBy);

                bLog.LogProductCharacteristic(objectContext, newPChar, LogType.create, string.Empty, string.Empty, currUser);

                BusinessStatistics stat = new BusinessStatistics();
                stat.ProdCharCreated(userContext, objectContext);
            }
            else
            {
                if (testChar.visible == true)
                {
                    throw new BusinessException(string.Format("User ID : {0} cannot add characteristic with name : {1} for product ID : {2}, because there is already with such name."
                    , currUser.ID, product.ID, name));
                }

                testChar.visible = true;
                testChar.lastModified = DateTime.UtcNow;
                testChar.LastModifiedBy = Tools.GetUserID(objectContext, currUser);
                Tools.Save(objectContext);

                UpdateLastModifiedAndModifiedBy(objectContext, product, testChar.LastModifiedBy);

                bLog.LogProductCharacteristic(objectContext, testChar, LogType.undelete, string.Empty, string.Empty, currUser);

                if (testChar.description != description)
                {
                    string oldValue = testChar.description;
                    testChar.description = description;
                    Tools.Save(objectContext);
                    bLog.LogProductCharacteristic(objectContext, testChar, LogType.edit, "description", oldValue, currUser);
                }
            }
        }

        /// <summary>
        /// Returns all Products created by User
        /// </summary>
        public IEnumerable<Product> GetAllProductsCreatedBy(Entities objectContext, long? userid)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;
            if ((userid == null) || (userid.Value < 1))
            {
                throw new BusinessException("userid is null or < 1");
            }
            else
            {
                BusinessUser businessUser = new BusinessUser();
                UserID user = Tools.GetUserID(objectContext, userid.Value, true);

                user.Products.Load();

                products = user.Products;
            }
            return products;
        }

        /// <summary>
        /// Returns all visible=true (and with good connections) products which are from company
        /// </summary>
        public IEnumerable<Product> GetAllProductsFromCompany(Entities objectContext, long? compID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;
            if ((compID == null) || (compID.Value < 1))
            {
                throw new BusinessException("Error : invalid company !");
            }

            BusinessCompany businessCompany = new BusinessCompany();
            Company company = businessCompany.GetCompanyWV(objectContext, compID.Value);
            if (company == null)
            {
                throw new BusinessException(string.Format("Theres no company ID = {0}", compID.Value));
            }
            if (company.Products.Count == 0)
            {
                company.Products.Load();
            }

            products = company.Products.Where(prod => prod.visible == true);

            string error = "";

            List<Product> correctProductsList = new List<Product>();
            foreach (Product product in products)
            {
                if (CheckIfProductsIsValidWithConnections(objectContext, product, out error))
                {
                    correctProductsList.Add(product);
                }
            }

            return correctProductsList;
        }

        /// <summary>
        /// Returns all visible=true (and with good connections) products which are from company and are within range
        /// </summary>
        public List<Product> GetAllProductsFromCompany(Entities objectContext, long? compID, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.CheckFromToParameters(from, to);
            if ((compID == null) || (compID.Value < 1))
            {
                throw new BusinessException("Error : invalid company !");
            }
            BusinessCompany businessCompany = new BusinessCompany();
            Company company = businessCompany.GetCompanyWV(objectContext, compID.Value);
            if (company == null)
            {
                throw new BusinessException(string.Format("Theres no company ID = {0}", compID.Value));
            }

            IEnumerable<Product> products = objectContext.GetAllProductsFromCompany(compID, from, to);

            return products.ToList();
        }

        /// <summary>
        /// Returns all visible=false Products from Company sorted by descending
        /// </summary>
        /// <param name="compID">Company ID</param>
        public IEnumerable<Product> GetAllDeletedProductsFromCompanyByDescending(Entities objectContext, long? compID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;
            if ((compID == null) || (compID.Value < 1))
            {
                throw new BusinessException("compID is null or empty");
            }
            else
            {
                BusinessCompany businessCompany = new BusinessCompany();
                Company company = businessCompany.GetCompanyWV(objectContext, compID.Value);
                if (company == null)
                {
                    throw new BusinessException(string.Format("Theres no company ID = {0}", compID.Value));
                }
                if (company.Products.Count == 0)
                {
                    company.Products.Load();
                }

                products = company.Products.Where(prod => prod.visible == false).
                    OrderByDescending<Product, long>(new Func<Product, long>(IdSelector));
            }
            return products;
        }

        /// <summary>
        /// Returns all products which are visible=false sorted by descending
        /// </summary>
        public IEnumerable<Product> GetAllDeletedProductsByDescending(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;

            products = objectContext.ProductSet.Where
                (prod => prod.visible == false).
                OrderByDescending<Product, long>(new Func<Product, long>(IdSelector));

            return products;
        }

        /// <summary>
        /// Returns all visible=true products sorted by descending
        /// </summary>
        public IEnumerable<Product> GetAllVisibleProductsByDescending(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;

            products = objectContext.ProductSet.Where
                (prod => prod.visible == true).
                OrderByDescending<Product, long>(new Func<Product, long>(IdSelector));

            return products;
        }

        /// <summary>
        /// Returns all Products from company which are in category
        /// </summary>
        /// <param name="onlyVisible">true if should return only visible=true products, otherwise false</param>
        public IEnumerable<Product> GetAllProductsFromCompanyWithCategory(Entities objectContext, long compID, long catID, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;
            if (compID < 1)
            {
                throw new BusinessException("compID is < 1");
            }
            if (catID < 1)
            {
                throw new BusinessException("catID is < 1");
            }
            if (onlyVisible == true)
            {
                products = objectContext.ProductSet.Where
                   (prod => prod.Company.ID == compID && prod.visible == true && prod.Category.ID == catID);
            }
            else
            {
                products = objectContext.ProductSet.Where
                  (prod => prod.Company.ID == compID && prod.Category.ID == catID);
            }

            return products;
        }

        /// <summary>
        /// Returns all deleted products in category from company, which were deleted at some period (when company was deleted)
        /// </summary>
        public List<Product> GetAllDeletedProductsFromCompanyWithCategory(Entities objectContext, long compID, long catID
            , DateTime deletedFrom, DateTime deletedTo, BusinessLog bLog)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            if (compID < 1)
            {
                throw new BusinessException("compID is < 1");
            }
            if (catID < 1)
            {
                throw new BusinessException("catID is < 1");
            }

            if (deletedFrom >= deletedTo)
            {
                throw new BusinessException("deletedFrom >= deletedTo");
            }


            List<Product> products = objectContext.ProductSet.Where
                   (prod => prod.Company.ID == compID && prod.visible == false && prod.Category.ID == catID).ToList();

            List<Product> productsDeletedAtSameTime = new List<Product>();

            if (products != null && products.Count > 0)
            {
                List<Log> lastDeleting = new List<Log>();

                foreach (Product product in products)
                {
                    lastDeleting = bLog.GetLogs(objectContext, "setDeleted", "product", product.ID, 1, 0);

                    if (lastDeleting.Count > 0 && lastDeleting[0].dateCreated >= deletedFrom && lastDeleting[0].dateCreated <= deletedTo)
                    {
                        productsDeletedAtSameTime.Add(product);
                    }
                }
            }

            return productsDeletedAtSameTime;
        }

        public List<Product> GetAllProductsFromCompanyWithCategory(Entities objectContext, Company currCompany, long catID, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }
            if (catID < 1)
            {
                throw new BusinessException("catID is < 1");
            }
            if (from >= to)
            {
                throw new BusinessException("from >= to");
            }

            IEnumerable<Product> products = null;

            products = objectContext.GetAllCompanyProductsInCategory(currCompany.ID, catID, from, to);

            return products.ToList();
        }

        /// <summary>
        /// Returns all vsible=true products from Category
        /// </summary>
        public List<Product> GetAllProductsFromCategory(Entities objectContext, long? catID, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;
            if ((catID == null) || (catID.Value < 1))
            {
                throw new BusinessException("catID is null or < 1");
            }

            Tools.CheckFromToParameters(from, to);

            BusinessCategory businessCategory = new BusinessCategory();
            Category currCategory = businessCategory.GetWithoutVisible(objectContext, catID.Value);
            if (currCategory == null)
            {
                throw new BusinessException(string.Format("Theres no Category ID = {0}", catID.Value));
            }

            products = objectContext.GetAllProductsFromCategory(catID.Value, from, to);
            return products.ToList();
        }

        /// <summary>
        /// Returns all visible.true products with VALID connections from category sorted by descending
        /// </summary>
        public IEnumerable<Product> GetAllProductsFromCategoryByDescending(Entities objectContext, long? catID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;
            if ((catID == null) || (catID.Value < 1))
            {
                throw new BusinessException("catID is null or < 1");
            }

            BusinessCategory businessCategory = new BusinessCategory();
            Category currCategory = businessCategory.GetWithoutVisible(objectContext, catID.Value);
            if (currCategory == null)
            {
                throw new BusinessException(string.Format("Theres no Category ID = {0}", catID.Value));
            }

            currCategory.Products.Load();

            products = currCategory.Products.Where(prod => prod.visible == true);

            List<Product> validProducts = new List<Product>();
            string error = "";
            foreach (Product product in products)
            {
                if (CheckIfProductsIsValidWithConnections(objectContext, product, out error))
                {
                    validProducts.Add(product);
                }
            }

            validProducts.OrderByDescending<Product, long>(new Func<Product, long>(IdSelector)); ;

            return validProducts;
        }

        public List<Product> GetLastAddedProductsInCategory(Entities objectContext, Category currCategory, long number)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }

            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            List<Product> lastProducts = objectContext.GetLastAddedProductsInCategory(number, currCategory.ID).ToList();

            return lastProducts;
        }

        /// <summary>
        /// Returns all visible=true Products from category Sorted by the number of comments they have typed in
        /// </summary>
        public IEnumerable<Product> GetAllProductsFromCategorySortedByComments(Entities objectContext, long? catID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;
            if ((catID == null) || (catID.Value < 1))
            {
                throw new BusinessException("catID is null or < 1");
            }

            BusinessCategory businessCategory = new BusinessCategory();
            Category currCategory = businessCategory.GetWithoutVisible(objectContext, catID.Value);
            if (currCategory == null)
            {
                throw new BusinessException(string.Format("Theres no Category ID = {0}", catID.Value));
            }
            if (currCategory.Products.Count == 0)
            {
                currCategory.Products.Load();
            }

            products = currCategory.Products.Where(prod => prod.visible == true && prod.comments > 0);

            // check if products have valid conenctions

            List<Product> validProducts = new List<Product>();
            string error = "";
            foreach (Product product in products)
            {
                if (CheckIfProductsIsValidWithConnections(objectContext, product, out error))
                {
                    validProducts.Add(product);
                }
            }

            validProducts.OrderByDescending<Product, long>(new Func<Product, long>(CommentsSelector));

            return validProducts;
        }

        public List<Product> GetMostCommentedProductsInCategory(Entities objectContext, Category currCategory, long number)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }

            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            List<Product> commProducts = objectContext.GetMostCommentedProductsInCategory(number, currCategory.ID).ToList();

            return commProducts;
        }

        /// <summary>
        /// Returns all visible=false Products from category sorted by descending 
        /// </summary>
        public IEnumerable<Product> GetAllDeletedProductsFromCategoryByDescending(Entities objectContext, long? catID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;
            if ((catID == null) || (catID.Value < 1))
            {
                throw new BusinessException("catID is null or < 1");
            }
            else
            {
                BusinessCategory businessCategory = new BusinessCategory();
                Category currCategory = businessCategory.GetWithoutVisible(objectContext, catID.Value);
                if (currCategory == null)
                {
                    throw new BusinessException(string.Format("Theres no Category ID = {0}", catID.Value));
                }
                if (currCategory.Products.Count == 0)
                {
                    currCategory.Products.Load();
                }

                products = currCategory.Products.Where(prod => prod.visible == false).
                    OrderByDescending<Product, long>(new Func<Product, long>(IdSelector));
            }
            return products;
        }

        /// <summary>
        /// Counts visible=false products in category
        /// </summary>
        public long CountAllDeletedProductsFromCategory(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            currCategory.Products.Load();
            long count = currCategory.Products.Count(prod => prod.visible == false);
            return count;
        }

        /// <summary>
        /// Returns all visible=false Products from category
        /// </summary>
        public IEnumerable<Product> GetAllDeletedProductsFromCategory(Entities objectContext, long? catID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Product> products;
            if ((catID == null) || (catID.Value < 1))
            {
                throw new BusinessException("catID is null or < 1");
            }
            else
            {
                BusinessCategory businessCategory = new BusinessCategory();
                Category currCategory = businessCategory.GetWithoutVisible(objectContext, catID.Value);
                if (currCategory == null)
                {
                    throw new BusinessException(string.Format("Theres no Category ID = {0}", catID.Value));
                }
                if (currCategory.Products.Count == 0)
                {
                    currCategory.Products.Load();
                }

                products = currCategory.Products.Where(prod => prod.visible == false);
            }
            return products;
        }

        /// <summary>
        /// Returns all Product`s Characteristics
        /// </summary>
        /// <param name="withVisibleFalse">True if should return all characteristics , false for only visible=true</param>
        public IEnumerable<ProductCharacteristics> GetAllProductCharacteristics(Entities objectContext, long? productID, Boolean withVisibleFalse)
        {
            Tools.AssertObjectContextExists(objectContext);

            if ((productID == null) || (productID.Value < 1))
            {
                throw new BusinessException("productID is null or < 1");
            }

            Product product = GetProductByIDWV(objectContext, productID.Value);
            if (product == null)
            {
                throw new BusinessException(string.Format("Theres no Product Id = {0}", productID.Value));
            }

            IEnumerable<ProductCharacteristics> productCharacteristics;
            product.Characteristics.Load();

            if (withVisibleFalse)
            {
                productCharacteristics = product.Characteristics;
            }
            else
            {
                productCharacteristics = product.Characteristics.Where(prod => prod.visible == true);
            }


            return productCharacteristics;
        }

        /// <summary>
        /// Returns product with id
        /// </summary>
        public Product GetProductByIDWV(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            Product product = objectContext.ProductSet.FirstOrDefault<Product>(prod => prod.ID == id);

            return product;
        }

        /// <summary>
        /// Returns visibile=true product with id
        /// </summary>
        public Product GetProductByID(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }
            Product product = objectContext.ProductSet.FirstOrDefault<Product>(prod => prod.ID == id && prod.visible == true);

            return product;
        }

        /// <summary>
        /// Returns visible=true product which have the name
        /// </summary>
        public Product GetProductByName(Entities objectContext, String name)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (name == null || name.Length < 1)
            {
                throw new BusinessException("name is null or empty");
            }
            Product product = objectContext.ProductSet.FirstOrDefault<Product>(prod => prod.name == name && prod.visible == true);
            return product;
        }

        /// <summary>
        /// Returns visible=true characteristic with ID
        /// </summary>
        public ProductCharacteristics GetCharacteristic(Entities objectContext, long? charId)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (charId == null || charId < 1)
            {
                throw new BusinessException("charId is null or < 1");
            }
            ProductCharacteristics productChar
                = objectContext.ProductCharacteristicsSet.FirstOrDefault<ProductCharacteristics>
                (prodC => prodC.ID == charId && prodC.visible == true);
            return productChar;
        }

        /// <summary>
        /// Returns characteristic with ID
        /// </summary>
        public ProductCharacteristics GetCharacteristicEvenIfVisibleFalse(Entities objectContext, long? charId)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (charId == null || charId < 1)
            {
                throw new BusinessException("charId is null or < 1");
            }
            ProductCharacteristics productChar
                = objectContext.ProductCharacteristicsSet.FirstOrDefault<ProductCharacteristics>
                (prodC => prodC.ID == charId);
            return productChar;
        }

        /// <summary>
        /// Returns Name of the characteristic with ID
        /// </summary>
        public String GetCharacteristicName(Entities objectContext, long? charId)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (charId == null || charId < 1)
            {
                throw new BusinessException("charId is null or < 1");
            }
            ProductCharacteristics productChar
                = objectContext.ProductCharacteristicsSet.FirstOrDefault<ProductCharacteristics>
                (prodC => prodC.ID == charId);

            if (productChar == null)
            {
                string error = string.Format("theres no product characteristic with ID {0}", charId);
                throw new BusinessException(error);
            }

            return productChar.name;
        }

        /// <summary>
        /// Changes Product`s Description
        /// </summary>
        /// <param name="currUser">user changing description</param>
        public void ChangeProductDescription(Entities objectContext, Product currProduct, string newDescr, User currUser, BusinessLog Blog)
        {
            Tools.AssertBusinessLogExists(Blog);
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currProduct.description != newDescr)
            {
                string oldDescr = currProduct.description;

                currProduct.description = newDescr;
                currProduct.lastModified = DateTime.UtcNow;
                currProduct.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                Blog.LogProduct(objectContext, currProduct, LogType.edit, "description", oldDescr, currUser);
            }
        }

        /// <summary>
        /// Changes product website
        /// </summary>
        public void ChangeProductWebSite(Entities objectContext, Product currProduct, string newSite, User currUser, BusinessLog Blog)
        {
            Tools.AssertBusinessLogExists(Blog);
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currProduct.website != newSite)
            {
                string oldSite = currProduct.website;

                currProduct.website = newSite;
                currProduct.lastModified = DateTime.UtcNow;
                currProduct.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                Blog.LogProduct(objectContext, currProduct, LogType.edit, "website", oldSite, currUser);
            }
        }

        /// <summary>
        /// Changes Product`s Company 
        /// </summary>
        public void ChangeProductCompany(Entities objectContext, Product currProduct, Company newCompany, User currUser, BusinessLog Blog)
        {
            Tools.AssertBusinessLogExists(Blog);
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("invalid product");
            }

            if (newCompany == null)
            {
                throw new BusinessException("invalid new company");
            }

            if (currUser == null)
            {
                throw new BusinessException("invalid curr user");
            }

            if (!currProduct.CompanyReference.IsLoaded)
            {
                currProduct.CompanyReference.Load();
            }

            if (currProduct.Company != newCompany)
            {
                string oldCompany = currProduct.Company.name;

                currProduct.Company = newCompany;
                currProduct.lastModified = DateTime.UtcNow;
                currProduct.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                Blog.LogProduct(objectContext, currProduct, LogType.edit, "company", oldCompany, currUser);
            }
        }

        /// <summary>
        /// Changes Product`s name
        /// </summary>
        public void ChangeProductName(Entities objectContext, Product currProduct, User currUser, BusinessLog Blog, string newName)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(Blog);
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (string.IsNullOrEmpty(newName))
            {
                throw new BusinessException("newName is null or empty");
            }

            bool changeName = false;

            if (string.Equals(currProduct.name, newName, StringComparison.InvariantCultureIgnoreCase) == true
               && string.Equals(currProduct.name, newName, StringComparison.InvariantCulture) == false)
            {
                changeName = true;
            }
            else
            {
                changeName = Tools.NameValidatorPassed(objectContext, "products", newName, 0);
            }

            if (changeName == true)
            {
                string oldName = currProduct.name;

                currProduct.name = newName;
                currProduct.lastModified = DateTime.UtcNow;
                currProduct.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                Blog.LogProduct(objectContext, currProduct, LogType.edit, "name", oldName, currUser);
            }
            else
            {
                throw new BusinessException(string.Format("Product ID = {0} cannot change name to '{1}' , user ID = {2}",
                    currProduct.ID, newName, currUser.ID));
            }
        }

        /// <summary>
        /// Changes Product`s category
        /// </summary>
        public void ChangeProductCategory(EntitiesUsers userContext, Entities objectContext, Product currProduct, Category newCategory
            , User currUser, BusinessLog Blog, bool moveOfUnspecifiedProduct)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(Blog);
            Tools.AssertObjectContextExists(userContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }
            if (newCategory == null)
            {
                throw new BusinessException("newCategory is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            BusinessUser bUser = new BusinessUser();

            if (!currProduct.CategoryReference.IsLoaded)
            {
                currProduct.CategoryReference.Load();
            }

            if (currProduct.Category != newCategory)
            {
                BusinessCompany businessCompany = new BusinessCompany();
                Company currCompany = GetProductCompany(objectContext, currProduct);

                if (!businessCompany.CheckIfCompanyCanHaveProductsInCategory(objectContext, currCompany, newCategory))
                {
                    Company other = businessCompany.GetOther(objectContext);
                    ChangeProductCompany(objectContext, currProduct, other, currUser, Blog);
                }

                String oldCategory = string.Format(currProduct.Category.name + " ID '" + currProduct.Category.ID + "' ");

                currProduct.Category = newCategory;
                currProduct.lastModified = DateTime.UtcNow;
                currProduct.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                Blog.LogProduct(objectContext, currProduct, LogType.edit, "category", oldCategory, currUser);

                //SEND system message to product`s editors, that it was moved AND removes the row from ProductInUnspecifiedCategory table
                if (moveOfUnspecifiedProduct == true)
                {
                    Category wantedCategory = null;

                    //Deletes the row in ProductInUnspecifiedCategory after product is moved
                    ProductInUnspecifiedCategory piuProduct = objectContext.ProductInUnspecifiedCategorySet.FirstOrDefault
               (pic => pic.Product.ID == currProduct.ID);

                    if (piuProduct != null)
                    {
                        if (!piuProduct.WantedCategoryReference.IsLoaded)
                        {
                            piuProduct.WantedCategoryReference.Load();
                        }
                        wantedCategory = piuProduct.WantedCategory;

                        objectContext.DeleteObject(piuProduct);
                        Tools.Save(objectContext);
                    }

                    // SEND system messages to product editors
                    if (currProduct.visible == true && wantedCategory.ID == newCategory.ID) // doesnt send System messages if wanted category is different from newcategory
                    {
                        BusinessUserTypeActions butActions = new BusinessUserTypeActions();
                        List<UsersTypeAction> actions = butActions.GetProductModificators(objectContext, currProduct.ID).ToList();

                        if (actions.Count > 0)
                        {
                            List<User> users = new List<User>();
                            User productEditor = null;

                            foreach (UsersTypeAction action in actions)
                            {
                                if (!action.UserReference.IsLoaded)
                                {
                                    action.UserReference.Load();
                                }

                                if (action.User.ID != currUser.ID)
                                {
                                    productEditor = bUser.Get(userContext, action.User.ID, true);
                                    users.Add(productEditor);
                                    productEditor = null;
                                }

                                if (users.Count > 0)
                                {
                                    BusinessSystemMessages bSystemMessage = new BusinessSystemMessages();

                                    //"The product [product] with company [company] which is added to 'unspecified' category, was moved to the wanted category [wanted category] ... .";
                                    string description = string.Format("{0} '{1}' {2} '{3}' {4} '{5}' {6}", Tools.GetResource("SMtoEditorWhenProdInUnsCatIsMoved")
                                        , currProduct.name, Tools.GetResource("SMtoEditorWhenProdInUnsCatIsMoved1"), currCompany.name,
                                        Tools.GetResource("SMtoEditorWhenProdInUnsCatIsMoved2"), Tools.CategoryName(objectContext, newCategory, false),
                                        Tools.GetResource("SMtoEditorWhenProdInUnsCatIsMoved3"));

                                    foreach (User editor in users)
                                    {
                                        bSystemMessage.Add(userContext, editor, description);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Changes product characteristic`s name
        /// </summary>
        public void ChangeProductCharacteristicName(Entities objectContext, ProductCharacteristics currProductChar,
            string newName, User currUser, BusinessLog Blog)
        {
            Tools.AssertBusinessLogExists(Blog);
            Tools.AssertObjectContextExists(objectContext);

            if (currProductChar == null)
            {
                throw new BusinessException("currProductChar is null");
            }

            if (newName == null || newName.Length < 1)
            {
                throw new BusinessException("newName is null or empty");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            BusinessUser bUser = new BusinessUser();
            if (bUser.IsFromUserTeam(currUser))
            {
                BusinessComment bComment = new BusinessComment();

                if (bComment.AreThereVisibleCommentsForCharacteristic(objectContext, currProductChar) == true)
                {
                    throw new BusinessException(string.Format("User id : {0} cannot change product characteristic id : {1} name, because there are opinions for it."
                    , currUser.ID, currProductChar.ID));
                }
            }


            if (currProductChar.name != newName)
            {
                string oldName = currProductChar.name;

                currProductChar.name = newName;
                currProductChar.lastModified = DateTime.UtcNow;
                currProductChar.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                Blog.LogProductCharacteristic(objectContext, currProductChar, LogType.edit, "name", oldName, currUser);

                if (!currProductChar.ProductReference.IsLoaded)
                {
                    currProductChar.ProductReference.Load();
                }
                UpdateLastModifiedAndModifiedBy(objectContext, currProductChar.Product, currProductChar.LastModifiedBy);
            }
        }

        public void ChangeIfUsersCanTakeActionIfThereAreNoEditors(Entities objectContext, Product currProduct,
            User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("curruser is null");
            }

            BusinessUser bUser = new BusinessUser();
            if (!bUser.IsAdminOrGlobalAdmin(currUser))
            {
                throw new BusinessException(string.Format("User id : {0} cannot change canUserTakeRoleIfNoEditors of product id : {1}, because he is not administrator or global administrator"
                    , currUser.ID, currProduct.ID));
            }

            bool oldValue = currProduct.canUserTakeRoleIfNoEditors;

            if (currProduct.canUserTakeRoleIfNoEditors == true)
            {
                currProduct.canUserTakeRoleIfNoEditors = false;
            }
            else
            {
                currProduct.canUserTakeRoleIfNoEditors = true;
            }

            Tools.Save(objectContext);

            bLog.LogProduct(objectContext, currProduct, LogType.edit, "canUserTakeRoleIfNoEditors", oldValue.ToString(), currUser);

        }

        /// <summary>
        /// Changes prouct characteristic`s description
        /// </summary>
        public void ChangeProductCharacteristicDescription(Entities objectContext, ProductCharacteristics currProductChar,
            string newDescr, User currUser, BusinessLog Blog)
        {
            Tools.AssertBusinessLogExists(Blog);
            Tools.AssertObjectContextExists(objectContext);

            if (currProductChar == null)
            {
                throw new BusinessException("currProductChar is null");
            }

            if (newDescr == null || newDescr.Length < 1)
            {
                throw new BusinessException("newDescr is null or empty");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currProductChar.description != newDescr)
            {
                string oldDescr = currProductChar.description;

                currProductChar.description = newDescr;
                currProductChar.lastModified = DateTime.UtcNow;
                currProductChar.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                Blog.LogProductCharacteristic(objectContext, currProductChar, LogType.edit, "description", oldDescr, currUser);

                if (!currProductChar.ProductReference.IsLoaded)
                {
                    currProductChar.ProductReference.Load();
                }
                UpdateLastModifiedAndModifiedBy(objectContext, currProductChar.Product, currProductChar.LastModifiedBy);
            }
        }

        /// <summary>
        /// Makes visible=false product characteristic
        /// </summary>
        public void DeleteProductCharacteristic(EntitiesUsers userContext, Entities objectContext
            , ProductCharacteristics currProductChar, User currUser, BusinessLog Blog)
        {
            Tools.AssertBusinessLogExists(Blog);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (currProductChar == null)
            {
                throw new BusinessException("currProductChar is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currProductChar.visible == false)
            {
                throw new BusinessException(string.Format("Product characteristic ID = {0} is already visible=false" +
                    " , ther are valdiators checking for that before this function", currProductChar.ID));
            }
            currProductChar.visible = false;
            currProductChar.lastModified = DateTime.UtcNow;
            currProductChar.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            Blog.LogProductCharacteristic(objectContext, currProductChar, LogType.delete, string.Empty, string.Empty, currUser);

            if (!currProductChar.ProductReference.IsLoaded)
            {
                currProductChar.ProductReference.Load();
            }
            UpdateLastModifiedAndModifiedBy(objectContext, currProductChar.Product, currProductChar.LastModifiedBy);

            BusinessStatistics stat = new BusinessStatistics();
            stat.ProdCharDeleted(userContext, objectContext);
        }

        /// <summary>
        /// Makes visible false Product
        /// </summary>
        public void DeleteProduct(Entities objectContext, EntitiesUsers userContext, Product currProduct, User currUser, BusinessLog Blog)
        {
            Tools.AssertBusinessLogExists(Blog);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null ");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (currProduct.visible == false)
            {
                throw new BusinessException(string.Format("Product ID = {0} is already visible=false" +
                    " , there are valdiators checking for that before this function", currProduct.ID));
            }

            currProduct.visible = false;
            currProduct.lastModified = DateTime.UtcNow;
            currProduct.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            Blog.LogProduct(objectContext, currProduct, LogType.delete, string.Empty, string.Empty, currUser);
            BusinessStatistics stat = new BusinessStatistics();
            stat.ProductDeleted(userContext, objectContext);

            BusinessNotifies businessNotifies = new BusinessNotifies();
            businessNotifies.RemoveNotifiesForType(objectContext, userContext, Blog, NotifyType.Product, currProduct.ID);
            businessNotifies.RemoveNotifiesForType(objectContext, userContext, Blog, NotifyType.ProductForum, currProduct.ID);

            BusinessUserTypeActions bTypeActions = new BusinessUserTypeActions();
            bTypeActions.RemoveAllTypeActionsForProductWhenDeleted(objectContext, userContext, currProduct, currUser, Blog);
        }

        public void MakeVisibleProduct(Entities objectContext, Product currProduct, User currUser, BusinessLog bLog)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("invalid product ");
            }

            if (currUser == null)
            {
                throw new BusinessException("invalid curr user");
            }

            currProduct.visible = true;
            currProduct.lastModified = DateTime.UtcNow;
            currProduct.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogProduct(objectContext, currProduct, LogType.undelete, string.Empty, string.Empty, currUser);
        }

        private long IdSelector(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("statistic");
            }
            return product.ID;
        }

        private long CommentsSelector(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("statistic");
            }
            return product.comments;
        }

        /// <summary>
        /// Returns all visible=true Products which Start with Character and are in Category
        /// </summary>
        public IEnumerable<Product> GetAllProductsWhichStartsWith(Entities objectContext, Category currCategory, Char letter)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            string startingStr = new string(new char[] { letter });


            currCategory.Products.Load();


            IEnumerable<Product> allProducts =
                currCategory.Products.Where<Product>(prod => prod.visible == true && prod.name.StartsWith(startingStr, StringComparison.InvariantCultureIgnoreCase));  // Try to avoid enumerating all the products

            string error = "";
            List<Product> finalProducts = new List<Product>();
            if (allProducts.Count<Product>() > 0)
            {
                foreach (Product product in allProducts)
                {
                    if (CheckIfProductsIsValidWithConnections(objectContext, product, out error))
                    {
                        finalProducts.Add(product);
                    }
                }
            }

            return finalProducts;
        }

        /// <summary>
        /// Returns all visible=true Products which Start with Character and are in Category within range
        /// </summary>
        public List<Product> GetAllProductsWhichStartsWith(Entities objectContext, Category currCategory, Char letter, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            Tools.CheckFromToParameters(from, to);

            string startingStr = new string(new char[] { letter });

            IEnumerable<Product> allProducts = objectContext.GetAllProductsFromCategoryWhichStartWith(currCategory.ID, from, to, letter.ToString());

            return allProducts.ToList();
        }

        /// <summary>
        /// Returns all visivle=true products which dont start with english alphabetic chars or numbers , from category
        /// </summary>
        public IEnumerable<Product> GetAllOtherProducts(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }


            ProductNameOtherCategoryID = currCategory.ID;
            PredOBjectContext = objectContext;
            IEnumerable<Product> allProducts = objectContext.ProductSet.Where(OtherProductPredicate);

            return allProducts;
        }


        /// <summary>
        /// Returns all visivle=true products which dont start with english alphabetic chars or numbers , from category within range
        /// </summary>
        public List<Product> GetAllOtherProducts(Entities objectContext, Category currCategory, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            Tools.CheckFromToParameters(from, to);

            IEnumerable<Product> allProducts = objectContext.GetAllOtherProductsFromCategory(currCategory.ID, from, to);

            return allProducts.ToList();
        }

        /// <summary>
        /// To be only used by GetAllOtherProducts() and OtherProductPredicate().
        /// <para>Necessary, because OtherProductPredicate has only one parameter.</para>
        /// </summary>
        private long ProductNameOtherCategoryID { get; set; }

        /// <summary>
        /// Used for OtherProductPredicate()
        /// </summary>
        private Entities PredOBjectContext { get; set; }


        /// <summary>
        /// Represents a predicate (condition) that matches a product which name starts
        /// with a symbol that is not among the ones returned by Tools.GetEnglishChars(true).
        /// The comparizon is case-insensitive.
        /// </summary>
        /// <param name="prod">The product to test.</param>
        /// <returns><c>true</c> if the product satisfies the condition (i.e. must be selected); orterwise, <c>false</c>.</returns>
        bool OtherProductPredicate(Product prod)
        {
            if (prod == null)
            {
                throw new ArgumentNullException("prod");
            }

            if (PredOBjectContext == null)
            {
                throw new ArgumentNullException("PredOBjectContext");
            }

            char[] lettersToLower = Tools.GetEnglishChars(true);
            char[] lettersToUpper = new char[lettersToLower.Length];
            for (int i = 0; i < lettersToUpper.Length; i++)
            {
                lettersToUpper[i] = char.ToUpperInvariant(lettersToLower[i]);
            }

            string notused = "";

            bool result = false;
            if (prod.CategoryReference.IsLoaded == false)
            {
                prod.CategoryReference.Load();
            }
            if (prod.Category != null)
            {
                if ((prod.Category.ID == ProductNameOtherCategoryID) &&
                    (prod.name.IndexOfAny(lettersToLower) != 0) && (prod.name.IndexOfAny(lettersToUpper) != 0))
                {
                    if (CheckIfProductsIsValidWithConnections(PredOBjectContext, prod, out notused))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returns all visible=true Products which Start with Number and are in Category with in range
        /// </summary>
        public List<Product> GetAllProductsWhichStartsWithNumber(Entities objectContext, Category currCategory, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            Tools.CheckFromToParameters(from, to);

            IEnumerable<Product> Products = objectContext.GetAllProductsFromCategoryWhichStartWithNumber(currCategory.ID, from, to);

            return Products.ToList();
        }

        /// <summary>
        /// returns only those with good relations (visible true)
        /// </summary>
        public List<Product> GetLastAddedProducts(Entities objectContext, long number, bool withImagesOnly)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }

            return objectContext.GetLastAddedProducts(number, withImagesOnly).ToList();
        }

        /// <summary>
        /// returns last added products (no check for relations)
        /// </summary>
        public List<Product> GetLastProducts(Entities objectContext, long number)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }

            return objectContext.GetLastProducts(number).ToList();
        }

        public List<Product> GetProductsWithMostRecentComments(Entities objectContext, int number)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }

            return objectContext.GetProductsWithMostRecentComments(number).ToList();
        }

        /// <summary>
        /// Returns all visible=true Products which Start with Number and are in Category
        /// </summary>
        public IEnumerable<Product> GetAllProductsWhichStartsWithNumber(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            IEnumerable<Product> Products = null;

            List<string> numbers = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            List<Product> products = new List<Product>();

            string error = "";

            foreach (string number in numbers)
            {
                Products = objectContext.ProductSet.Where
                (prod => prod.Category.ID == currCategory.ID && prod.name.StartsWith(number));

                foreach (Product product in Products)
                {
                    if (CheckIfProductsIsValidWithConnections(objectContext, product, out error))
                    {
                        products.Add(product);
                    }
                }
            }

            return products;
        }

        /// <summary>
        /// Returns Characters which products in category names start with 
        /// </summary>
        public List<Char> GetCharsOnProducts(Entities objectContext, Category currCategory, out bool hasOthers)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            hasOthers = false;

            char[] letters = Tools.GetEnglishChars(true);
            string lettersStr = new string(letters, 0, letters.Length);

            SortedList<string, string> sortedExisting = new SortedList<string, string>();

            currCategory.Products.Load();

            IEnumerable<Product> categoryProducts =
                currCategory.Products.Where<Product>(prod => prod.visible == true);  // Try to avoid enumerating all the products

            foreach (Product product in categoryProducts)
            {
                if (string.IsNullOrEmpty(product.name) == false)
                {
                    string startStr = product.name.Substring(0, 1).ToLower();
                    if (lettersStr.IndexOf(startStr[0]) == -1)
                    {
                        hasOthers = true;
                    }
                    else if (sortedExisting.ContainsKey(startStr) == false)
                    {
                        sortedExisting.Add(startStr, startStr);
                    }
                }
            }

            List<Char> existing = new List<Char>();

            foreach (string key in sortedExisting.Keys)
            {
                if (string.IsNullOrEmpty(key) == false)
                {
                    existing.Add(key[0]);
                }
            }
            return existing;
        }

        /// <summary>
        /// Returns Product`s Company
        /// </summary>
        public Company GetProductCompany(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);
            BusinessCompany businessCompany = new BusinessCompany();
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            if (!currProduct.CompanyReference.IsLoaded)
            {
                currProduct.CompanyReference.Load();
            }

            Company currCompany = businessCompany.GetCompanyWV(objectContext, currProduct.Company.ID);
            if (currCompany == null)
            {
                String error = string.Format("Product {0}`s company {1} is null", currProduct.name, currProduct.Company.ID);
                throw new BusinessException(error);
            }

            return currCompany;
        }

        /// <summary>
        /// Checks if Product is visible=true and if its all connections are visible=true
        /// </summary>
        /// <returns>true if product is valid , otherwise false</returns>
        public Boolean CheckIfProductsIsValidWithConnections(Entities objectContext, Product currProduct, out string error)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is Null");
            }

            if (currProduct.CompanyReference.IsLoaded == false)
            {
                currProduct.CompanyReference.Load();
            }
            if (currProduct.CategoryReference.IsLoaded == false)
            {
                currProduct.CategoryReference.Load();
            }

            Boolean result = true;

            System.Text.StringBuilder errors = new StringBuilder();
            error = "";

            BusinessCompany businessCompany = new BusinessCompany();
            Company other = businessCompany.GetOther(objectContext);
            if (currProduct.Company.ID != other.ID)
            {
                CategoryCompany catComp = objectContext.CategoryCompanySet.FirstOrDefault
                                (cc => cc.visible == true && cc.Company.ID == currProduct.Company.ID &&
                                    cc.Category.ID == currProduct.Category.ID);

                if (catComp == null)
                {
                    result = false;
                    errors.Append(" Invalid company category (doesnt exist or it is visible = false).");
                }
            }

            if (currProduct.visible == false)
            {
                result = false;
                errors.Append(" Thr Product is visible = false.");
            }
            if (currProduct.Company.visible == false)
            {
                result = false;
                errors.Append(" The Product`s Company is visible = false.");
            }
            if (currProduct.Category.visible == false)
            {
                result = false;
                errors.Append(" The Category in which is the product is visible = false.");
            }
            if (currProduct.Category.last == false)
            {
                result = false;
                errors.Append(" The Category in which is the product is last = false.");
            }

            error = errors.ToString();
            return result;
        }

        /// <summary>
        /// Throws exception if there is invalid connection/data when adding new product with company, otherwise not
        /// </summary>
        public void CheckIfNewProductWillBeValidWithConnections(EntitiesUsers userContext, Entities objectContext, Company currCompany
            , Category currCategory, User currUser, String prodName, Category wantedCategory)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            if (currCompany == null)
            {
                throw new BusinessException("currCompany is null");
            }

            if (currCategory == null)
            {
                throw new BusinessException("currCategory is null");
            }

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (string.IsNullOrEmpty(prodName))
            {
                throw new BusinessException("prodName is null or empty");
            }

            if (!Tools.NameValidatorPassed(objectContext, "products", prodName, 0))
            {
                throw new BusinessException(string.Format("There is already product with name = {0} , User ID = {1}",
                    prodName, currUser.ID));
            }

            BusinessCompany businessCompany = new BusinessCompany();
            BusinessUser businessUser = new BusinessUser();
            BusinessCategory businessCategory = new BusinessCategory();

            if (!businessCompany.IsOther(objectContext, currCompany))
            {
                CategoryCompany catComp = objectContext.CategoryCompanySet.FirstOrDefault
                                (cc => cc.visible == true && cc.Company.ID == currCompany.ID &&
                                    cc.Category.ID == currCategory.ID);

                if (catComp == null)
                {
                    throw new BusinessException(string.Format("New product cannot be added in Category ID = {0} with Company ID = {1} (invalid connection), User ID = {2}",
                        currCategory.ID, currCompany.ID, currUser.ID));
                }
            }

            if (wantedCategory != null)
            {
                Category unspecifiedCategory = businessCategory.GetUnspecifiedCategory(objectContext);
                if (currCategory != unspecifiedCategory)
                {
                    throw new BusinessException("wantedCategory is not unspecifiedCategory");
                }

                CategoryCompany wantedCategoryCompany = businessCompany.GetCategoryCompany(objectContext, currCompany, wantedCategory);
                if (wantedCategoryCompany != null && wantedCategoryCompany.visible == true)
                {
                    throw new BusinessException(string.Format("New product cannot be in unspecified category with wanted category ID = {0}, in company ID = {1}, because that company can have products in the wanted category, user id = {2}"
                        , currCategory.ID, currCompany.ID, currUser.ID));
                }

                if (wantedCategory.visible == false)
                {
                    throw new BusinessException(string.Format("Wanted category id = {0} is visible.false, user id = {1}", wantedCategory.ID, currUser.ID));
                }
            }

            if (currCompany.visible == false || currCategory.visible == false ||
                  currUser.visible == false || !businessUser.CanUserDo(userContext, currUser, UserRoles.AddProducts))
            {
                throw new BusinessException(string.Format("New Product name = '{0}' cannot be added because there is invalid data, Company ID = {1}, Category ID = {2}, User ID = {3}",
                    prodName, currCompany.ID, currCategory.ID, currUser.ID));
            }

        }

        /// <summary>
        /// Returns number of visible=true Product`s characteristics
        /// </summary>
        public int CountProductCharacteristics(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is Null");
            }

            if (currProduct.Characteristics.Count == 0)
            {
                currProduct.Characteristics.Load();
            }

            int count = currProduct.Characteristics.Count(pc => pc.visible == true);

            return count;
        }

        /// <summary>
        /// Returns number of visible=true Products in category
        /// </summary>
        public int CountProductsInCategory(Entities objectContext, Category currCategory)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCategory == null)
            {
                throw new BusinessException("currCategory is Null");
            }

            if (currCategory.Products.Count == 0)
            {
                currCategory.Products.Load();
            }

            int count = currCategory.Products.Count(pc => pc.visible == true);

            return count;
        }

        /// <summary>
        /// Returns number of visible=true Product`s comments (doesnt include subcomments)
        /// </summary>
        public long CountProductComments(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            long comments = objectContext.CommentSet.Count<Comment>
                (cm => cm.type == "product" && cm.subType == "comment" && cm.visible == true && cm.typeID == currProduct.ID);

            return comments;
        }

        /// <summary>
        /// Returns number of visible=true Comments about Characteristic (doesnt include subcomments)
        /// </summary>
        public long CountProductCharComments(Entities objectContext, long charID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (charID < 1)
            {
                throw new BusinessException("charID < 1");
            }

            long comments = objectContext.CommentSet.Count<Comment>
                (cm => cm.type == "product" && cm.visible == true && cm.ForCharacteristic.ID == charID);

            return comments;
        }

        /// <summary>
        /// Returns number of visible=true comments which aren`t about characteristic or variant
        /// </summary>
        public long CountProductNoAboutComments(Entities objectContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            long comments = objectContext.CommentSet.Count<Comment>
                (cm => cm.type == "product" && cm.subType == "comment" && cm.typeID == product.ID && cm.visible == true
                    && cm.ForCharacteristic == null && cm.ForVariant == null && cm.ForSubVariant == null);

            return comments;
        }

        /// <summary>
        /// Increases by 1 Product`s comments
        /// </summary>
        public void IncreaseProductComments(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is Null");
            }

            lock (commentsSync)
            {
                currProduct.comments += 1;
                Tools.Save(objectContext);
            }
        }

        /// <summary>
        /// Decreases by 1 product`s comments 
        /// </summary>
        public void DecreaseProductComments(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is Null");
            }

            if (currProduct.comments < 1)
            {
                throw new BusinessException(string.Format("Currently Comments in product ID = '{0}' are 0 " +
                    "there shouldnt be comments to delete.", currProduct.ID));
            }

            lock (commentsSync)
            {
                currProduct.comments -= 1;
                Tools.Save(objectContext);
            }

        }

        /// <summary>
        /// Increases product characteristic`s comments by 1
        /// </summary>
        public void IncreaseProductCharComments(Entities objectContext, long prodCharID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (prodCharID < 1)
            {
                throw new BusinessException("prodCharID is < 1");
            }

            ProductCharacteristics pChar = GetCharacteristic(objectContext, prodCharID);
            if (pChar == null)
            {
                throw new BusinessException(string.Format("Theres noi product characteristic with ID = {0}", prodCharID));
            }

            lock (commentsSync)
            {
                pChar.comments += 1;
                Tools.Save(objectContext);
            }
        }

        /// <summary>
        /// Increases product characteristic`s comments by 1
        /// </summary>
        public void IncreaseProductCharComments(Entities objectContext, ProductCharacteristics prodChar)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (prodChar == null)
            {
                throw new BusinessException("prodChar is null");
            }

            lock (commentsSync)
            {
                prodChar.comments += 1;
                Tools.Save(objectContext);
            }
        }

        public void IncreaseProductVariantComments(Entities objectContext, ProductVariant variant)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (variant == null)
            {
                throw new BusinessException("variant is null");
            }

            lock (commentsSync)
            {
                variant.comments += 1;
                Tools.Save(objectContext);
            }
        }

        public void IncreaseProductSubVariantComments(Entities objectContext, ProductSubVariant subvariant)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (subvariant == null)
            {
                throw new BusinessException("subvariant is null");
            }

            lock (commentsSync)
            {
                subvariant.comments += 1;
                Tools.Save(objectContext);
            }
        }

        /// <summary>
        /// Decreases product characteristic`s comment by 1
        /// </summary>
        public void DecreaseProductCharComments(Entities objectContext, long prodCharID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (prodCharID < 1)
            {
                throw new BusinessException("prodCharID is < 1");
            }

            ProductCharacteristics pChar = GetCharacteristicEvenIfVisibleFalse(objectContext, prodCharID);
            if (pChar == null)
            {
                throw new BusinessException(string.Format("Theres no product characteristic with id = {0}", prodCharID));
            }

            if (pChar.comments == 0)
            {
                throw new BusinessException(string.Format("Product Characteristic with ID = {0} have already" +
                    " 0 comments , there shouldnt be comments to delete", prodCharID));
            }

            lock (commentsSync)
            {
                pChar.comments -= 1;
                Tools.Save(objectContext);
            }
        }

        /// <summary>
        /// Decreases product characteristic`s comment by 1
        /// </summary>
        public void DecreaseProductCharComments(Entities objectContext, ProductCharacteristics prodChar)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (prodChar == null)
            {
                throw new BusinessException("prodChar is null");
            }

            if (prodChar.comments == 0)
            {
                throw new BusinessException(string.Format("Product Characteristic with ID = {0} have already" +
                    " 0 comments , there shouldnt be comments to delete", prodChar.ID));
            }

            lock (commentsSync)
            {
                prodChar.comments -= 1;
                Tools.Save(objectContext);
            }
        }

        public void DecreaseProductVariantComments(Entities objectContext, ProductVariant variant)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (variant == null)
            {
                throw new BusinessException("prodChar is null");
            }

            if (variant.comments == 0)
            {
                throw new BusinessException(string.Format("Product Variant with ID = {0} have already" +
                    " 0 comments , there shouldnt be comments to delete", variant.ID));
            }

            lock (commentsSync)
            {
                variant.comments -= 1;
                Tools.Save(objectContext);
            }
        }

        public void DecreaseProductSubVariantComments(Entities objectContext, ProductSubVariant subvariant)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (subvariant == null)
            {
                throw new BusinessException("subvariant is null");
            }

            if (subvariant.comments == 0)
            {
                throw new BusinessException(string.Format("Product Sub variant with ID = {0} have already" +
                    " 0 comments , there shouldnt be comments to delete", subvariant.ID));
            }

            lock (commentsSync)
            {
                subvariant.comments -= 1;
                Tools.Save(objectContext);
            }
        }

        /// <summary>
        /// Returns number of visible=true subcomments of comment
        /// </summary>
        private int GetSubcommentsNumber(Entities objectContext, Comment currComment)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currComment == null)
            {
                throw new BusinessException("currComment is null");
            }

            int comments = 0;

            List<Comment> subComments = new List<Comment>();

            subComments = objectContext.CommentSet.Where(comm => comm.type == "subcomment" && comm.typeID == currComment.ID && comm.visible == true).ToList();
            if (subComments.Count > 0)
            {
                foreach (Comment comment in subComments)
                {
                    comments++;
                    comments += GetSubcommentsNumber(objectContext, comment);
                }
            }

            return comments;
        }

        /// <summary>
        /// Returns true if Configuration.ProductsTimeWhichNeedsToPassToAddAnother time passed after last added product by user, always true for admins
        /// </summary>
        public bool CheckIfMinimumTimeBetweenAddingProductsPassed(Entities objectContext, User currUser, out int minToWait)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser in null");
            }

            minToWait = 1;
            bool result = true;

            if (Configuration.ProductsTimeWhichNeedsToPassToAddAnother > 0)
            {
                BusinessUser businessUser = new BusinessUser();
                if (businessUser.IsFromAdminTeam(currUser))
                {
                    return true;
                }

                Product lastAddedByUser = null;
                List<Product> productsAddedbyUser = objectContext.ProductSet.Where(pr => pr.CreatedBy.ID == currUser.ID).ToList();
                if (productsAddedbyUser.Count > 0)
                {
                    lastAddedByUser = productsAddedbyUser.Last();
                }

                if (lastAddedByUser != null)
                {
                    DateTime prodTime = lastAddedByUser.dateCreated;

                    TimeSpan span = DateTime.UtcNow - lastAddedByUser.dateCreated;
                    int minPassed = (int)span.TotalMinutes;

                    if (minPassed < Configuration.ProductsTimeWhichNeedsToPassToAddAnother)
                    {
                        result = false;

                        minToWait = Configuration.ProductsTimeWhichNeedsToPassToAddAnother - minPassed;
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
        /// Updates product`s LastModified and ModifiedBy properties. Should be called when product characteristics/images/variants/subvariants are modified
        /// </summary>
        public void UpdateLastModifiedAndModifiedBy(Entities objectContext, Product currProduct, UserID userID)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }
            if (userID == null)
            {
                throw new BusinessException("userID is null");
            }

            currProduct.lastModified = DateTime.UtcNow;
            currProduct.LastModifiedBy = userID;
            Tools.Save(objectContext);
        }

        public List<Product> GetLastDeletedProducts(Entities objectContext, int number, string nameContains, long prodId, User byUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<Product> products = new List<Product>();

            if (byUser != null)
            {
                if (!string.IsNullOrEmpty(nameContains))
                {
                    if (prodId > 0)
                    {
                        products = objectContext.GetLastDeletedProducts(nameContains, (long)number, byUser.ID, prodId).ToList();
                    }
                    else
                    {
                        products = objectContext.GetLastDeletedProducts(nameContains, (long)number, byUser.ID, null).ToList();
                    }
                }
                else
                {
                    if (prodId > 0)
                    {
                        products = objectContext.GetLastDeletedProducts(null, (long)number, byUser.ID, prodId).ToList();
                    }
                    else
                    {
                        products = objectContext.GetLastDeletedProducts(null, (long)number, byUser.ID, null).ToList();
                    }
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(nameContains))
                {
                    if (prodId > 0)
                    {
                        products = objectContext.GetLastDeletedProducts(nameContains, (long)number, null, prodId).ToList();
                    }
                    else
                    {
                        products = objectContext.GetLastDeletedProducts(nameContains, (long)number, null, null).ToList();
                    }
                }
                else
                {
                    if (prodId > 0)
                    {
                        products = objectContext.GetLastDeletedProducts(null, (long)number, null, prodId).ToList();
                    }
                    else
                    {
                        products = objectContext.GetLastDeletedProducts(null, (long)number, null, null).ToList();
                    }
                }
            }

            return products;
        }


    }
}
