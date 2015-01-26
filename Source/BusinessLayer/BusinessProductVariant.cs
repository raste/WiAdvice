// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessProductVariant
    {
        public void AddVariant(Entities objectContext, Product currProduct, BusinessLog bLog, User currUser
            , string name, string description)
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

            if (!UniqueVariantName(objectContext, currProduct, name, 0))
            {
                throw new BusinessException(string.Format("There is already variant with name : {0}, USer id : {1}", name, currUser.ID));
            }

            ProductVariant deletedVariant = objectContext.ProductVariantSet.FirstOrDefault
                (var => var.Product.ID == currProduct.ID && var.name == name && var.visible == false);

            if (deletedVariant == null)
            {
                ProductVariant newVariant = new ProductVariant();
                newVariant.name = name;
                newVariant.Product = currProduct;
                newVariant.CreatedBy = Tools.GetUserID(objectContext, currUser);
                newVariant.dateCreated = DateTime.UtcNow;
                newVariant.LastModifiedBy = newVariant.CreatedBy;
                newVariant.lastModified = newVariant.dateCreated;
                newVariant.description = description;
                newVariant.visible = true;
                newVariant.comments = 0;

                objectContext.AddToProductVariantSet(newVariant);
                Tools.Save(objectContext);

                bLog.LogProductVariant(objectContext, newVariant, LogType.create, string.Empty, string.Empty, currUser);

                BusinessProduct bProduct = new BusinessProduct();
                bProduct.UpdateLastModifiedAndModifiedBy(objectContext, currProduct, newVariant.CreatedBy);
            }
            else
            {
                bool changeDescription = false;
                string oldDescription = deletedVariant.description;

                deletedVariant.visible = true;
                if (deletedVariant.description != description)
                {
                    deletedVariant.description = description;
                    changeDescription = true;
                }

                deletedVariant.lastModified = DateTime.UtcNow;
                deletedVariant.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                bLog.LogProductVariant(objectContext, deletedVariant, LogType.undelete, string.Empty, string.Empty, currUser);

                BusinessProduct bProduct = new BusinessProduct();
                bProduct.UpdateLastModifiedAndModifiedBy(objectContext, currProduct, deletedVariant.LastModifiedBy);

                if (changeDescription == true)
                {
                    bLog.LogProductVariant(objectContext, deletedVariant, LogType.edit, "description", oldDescription, currUser);
                }
            }

        }

        public void AddSubVariant(Entities objectContext, Product currProduct, BusinessLog bLog, User currUser
            , ProductVariant currVariant, string name, string description)
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
            if (currVariant == null)
            {
                throw new BusinessException("currVariant is null");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty");
            }

            if (!UniqueVariantName(objectContext, currProduct, name, currVariant.ID))
            {
                throw new BusinessException(string.Format("There is already variant with name : {0}, USer id : {1}", name, currUser.ID));
            }

            ProductSubVariant deletedVariant = objectContext.ProductSubVariantSet.FirstOrDefault
                (var => var.Variant.ID == currVariant.ID && var.name == name && var.visible == false);

            if (deletedVariant == null)
            {
                ProductSubVariant newVariant = new ProductSubVariant();
                newVariant.name = name;
                newVariant.Product = currProduct;
                newVariant.CreatedBy = Tools.GetUserID(objectContext, currUser);
                newVariant.dateCreated = DateTime.UtcNow;
                newVariant.LastModifiedBy = newVariant.CreatedBy;
                newVariant.lastModified = newVariant.dateCreated;
                newVariant.description = description;
                newVariant.visible = true;
                newVariant.Variant = currVariant;
                newVariant.comments = 0;

                objectContext.AddToProductSubVariantSet(newVariant);
                Tools.Save(objectContext);

                bLog.LogProductSubVariant(objectContext, newVariant, LogType.create, string.Empty, string.Empty, currUser);

                BusinessProduct bProduct = new BusinessProduct();
                bProduct.UpdateLastModifiedAndModifiedBy(objectContext, currProduct, newVariant.CreatedBy);
            }
            else
            {
                bool changeDescription = false;
                string oldDescription = deletedVariant.description;

                deletedVariant.visible = true;
                if (deletedVariant.description != description)
                {
                    deletedVariant.description = description;
                    changeDescription = true;
                }

                deletedVariant.lastModified = DateTime.UtcNow;
                deletedVariant.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                bLog.LogProductSubVariant(objectContext, deletedVariant, LogType.undelete, string.Empty, string.Empty, currUser);

                BusinessProduct bProduct = new BusinessProduct();
                bProduct.UpdateLastModifiedAndModifiedBy(objectContext, currProduct, deletedVariant.LastModifiedBy);

                if (changeDescription == true)
                {
                    bLog.LogProductSubVariant(objectContext, deletedVariant, LogType.edit, "description", oldDescription, currUser);
                }
            }

        }

        /// <summary>
        /// Returns true if variant/subvariant name is unique (checks only visible:true), if checking for variant variantID must be 0
        /// </summary>
        public static bool UniqueVariantName(Entities objectContext, Product currProduct, string name, long variantID)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is empty");
            }

            bool result = false;

            if (variantID > 0)
            {   // checking for subvariant
                ProductVariant variant = objectContext.ProductVariantSet.FirstOrDefault
                    (var => var.ID == variantID && var.Product.ID == currProduct.ID && var.visible == true);
                if (variant == null)
                {
                    throw new BusinessException(string.Format("There is no variant ID : {0} which is for product ID : {1} "
                        , variantID, currProduct.ID));
                }

                ProductSubVariant subVariant = objectContext.ProductSubVariantSet.FirstOrDefault
                    (var => var.Variant.ID == variant.ID && var.name == name && var.visible == true);

                if (subVariant == null)
                {
                    result = true;
                }
            }
            else
            {
                ProductVariant variant = objectContext.ProductVariantSet.FirstOrDefault
                    (var => var.Product.ID == currProduct.ID && var.name == name && var.visible == true);
                if (variant == null)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns variant
        /// </summary>
        public ProductVariant Get(Entities objectContext, Product currProduct, long id, bool checkForVisibility, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            ProductVariant variant = null;
            if (checkForVisibility == true)
            {
                variant = objectContext.ProductVariantSet.FirstOrDefault
                    (var => var.ID == id && var.Product.ID == currProduct.ID && var.visible == true);
            }
            else
            {
                variant = objectContext.ProductVariantSet.FirstOrDefault(var => var.ID == id && var.Product.ID == currProduct.ID);
            }

            if (variant == null && throwExcIfNull == true)
            {
                throw new BusinessException(string.Format("There is no variant with ID : {0} which is for product id : {1}"
                    , id, currProduct.ID));
            }

            return variant;
        }

        public ProductSubVariant GetSubVariant(Entities objectContext, Product currProduct, long id, bool checkForVisibility, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            ProductSubVariant variant = null;
            if (checkForVisibility == true)
            {
                variant = objectContext.ProductSubVariantSet.FirstOrDefault
                    (var => var.ID == id && var.Product.ID == currProduct.ID && var.visible == true);
            }
            else
            {
                variant = objectContext.ProductSubVariantSet.FirstOrDefault(var => var.ID == id && var.Product.ID == currProduct.ID);
            }

            if (variant == null && throwExcIfNull == true)
            {
                throw new BusinessException(string.Format("There is no sub variant with ID : {0} which is for product id : {1}"
                    , id, currProduct.ID));
            }

            return variant;

        }

        public void DeleteVariant(Entities objectContext, BusinessLog bLog, User currUser, ProductVariant currVariant)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currVariant == null)
            {
                throw new BusinessException("currVariant is null");
            }

            if (currVariant.visible == false)
            {
                throw new BusinessException(string.Format("Variant ID : {0} is already visible:false, User id : {1}", currVariant.ID, currUser.ID));
            }

            currVariant.visible = false;
            currVariant.lastModified = DateTime.UtcNow;
            currVariant.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogProductVariant(objectContext, currVariant, LogType.delete, string.Empty, string.Empty, currUser);

            if (!currVariant.ProductReference.IsLoaded)
            {
                currVariant.ProductReference.Load();
            }
            BusinessProduct bProduct = new BusinessProduct();
            bProduct.UpdateLastModifiedAndModifiedBy(objectContext, currVariant.Product, currVariant.LastModifiedBy);

            List<ProductSubVariant> subvariants = GetVisibleSubVariants(objectContext, currVariant);
            if (subvariants.Count > 0)
            {
                foreach (ProductSubVariant variant in subvariants)
                {
                    DeleteSubVariant(objectContext, bLog, currUser, variant);
                }
            }
        }

        public void DeleteSubVariant(Entities objectContext, BusinessLog bLog, User currUser, ProductSubVariant currVariant)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currVariant == null)
            {
                throw new BusinessException("currVariant is null");
            }

            if (currVariant.visible == false)
            {
                throw new BusinessException(string.Format("Variant ID : {0} is already visible:false, User id : {1}", currVariant.ID, currUser.ID));
            }

            currVariant.visible = false;
            currVariant.lastModified = DateTime.UtcNow;
            currVariant.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogProductSubVariant(objectContext, currVariant, LogType.delete, string.Empty, string.Empty, currUser);

            if (!currVariant.ProductReference.IsLoaded)
            {
                currVariant.ProductReference.Load();
            }
            BusinessProduct bProduct = new BusinessProduct();
            bProduct.UpdateLastModifiedAndModifiedBy(objectContext, currVariant.Product, currVariant.LastModifiedBy);
        }

        public void ChangeVariantDescription(Entities objectContext, BusinessLog bLog, User currUser, ProductVariant currVariant, string newDescription)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currVariant == null)
            {
                throw new BusinessException("currVariant is null");
            }

            if (currVariant.description == newDescription)
            {
                throw new BusinessException(string.Format("Variant ID : {0} `s description is same as new one, user id : {1}", currVariant.ID, currUser.ID));
            }

            string oldDescription = currVariant.description;

            currVariant.description = newDescription;
            currVariant.lastModified = DateTime.UtcNow;
            currVariant.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogProductVariant(objectContext, currVariant, LogType.edit, "description", oldDescription, currUser);

            if (!currVariant.ProductReference.IsLoaded)
            {
                currVariant.ProductReference.Load();
            }
            BusinessProduct bProduct = new BusinessProduct();
            bProduct.UpdateLastModifiedAndModifiedBy(objectContext, currVariant.Product, currVariant.LastModifiedBy);
        }

        public void ChangeSubVariantDescription(Entities objectContext, BusinessLog bLog, User currUser, ProductSubVariant currVariant, string newDescription)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currVariant == null)
            {
                throw new BusinessException("currVariant is null");
            }

            if (currVariant.description == newDescription)
            {
                throw new BusinessException(string.Format("Variant ID : {0} `s description is same as new one, user id : {1}", currVariant.ID, currUser.ID));
            }

            string oldDescription = currVariant.description;

            currVariant.description = newDescription;
            currVariant.lastModified = DateTime.UtcNow;
            currVariant.LastModifiedBy = Tools.GetUserID(objectContext, currUser);

            Tools.Save(objectContext);

            bLog.LogProductSubVariant(objectContext, currVariant, LogType.edit, "description", oldDescription, currUser);

            if (!currVariant.ProductReference.IsLoaded)
            {
                currVariant.ProductReference.Load();
            }
            BusinessProduct bProduct = new BusinessProduct();
            bProduct.UpdateLastModifiedAndModifiedBy(objectContext, currVariant.Product, currVariant.LastModifiedBy);
        }

        public List<ProductVariant> GetVisibleVariants(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            List<ProductVariant> variants = objectContext.ProductVariantSet.Where
                (var => var.Product.ID == currProduct.ID && var.visible == true).ToList();

            return variants;
        }

        public List<ProductVariant> GetAllProductVariants(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            List<ProductVariant> variants = objectContext.ProductVariantSet.Where
                (var => var.Product.ID == currProduct.ID).ToList();

            return variants;
        }

        /// <summary>
        /// counts visible variants for product
        /// </summary>
        public int CountVariants(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            int count = objectContext.ProductVariantSet.Count
                (var => var.Product.ID == currProduct.ID && var.visible == true);

            return count;
        }

        public long CountCommentsForVariant(Entities objectContext, long variantID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (variantID < 1)
            {
                throw new BusinessException("variantID < 1");
            }

            long comments = objectContext.CommentSet.Count<Comment>
                (cm => cm.type == "product" && cm.visible == true && cm.ForVariant.ID == variantID);

            return comments;
        }

        public long CountCommentsForSubVariant(Entities objectContext, long variantID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (variantID < 1)
            {
                throw new BusinessException("variantID < 1");
            }

            long comments = objectContext.CommentSet.Count<Comment>
                (cm => cm.type == "product" && cm.visible == true && cm.ForSubVariant.ID == variantID);

            return comments;
        }

        /// <summary>
        /// counts visible sub-variants for product
        /// </summary>
        public int CountSubVariants(Entities objectContext, Product currProduct)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            int count = objectContext.ProductSubVariantSet.Count
                (var => var.Product.ID == currProduct.ID && var.visible == true);

            return count;
        }

        /// <summary>
        /// Returns visible sub variants of variant
        /// </summary>
        public List<ProductSubVariant> GetVisibleSubVariants(Entities objectContext, ProductVariant currVariant)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currVariant == null)
            {
                throw new BusinessException("currVariant is null");
            }

            List<ProductSubVariant> variants = objectContext.ProductSubVariantSet.Where
                (var => var.Variant.ID == currVariant.ID && var.visible == true).ToList();

            return variants;
        }

        public List<ProductSubVariant> GetAllSubVariants(Entities objectContext, ProductVariant currVariant)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currVariant == null)
            {
                throw new BusinessException("currVariant is null");
            }

            List<ProductSubVariant> variants = objectContext.ProductSubVariantSet.Where
                (var => var.Variant.ID == currVariant.ID).ToList();

            return variants;
        }
    }
}
