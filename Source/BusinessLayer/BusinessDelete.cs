// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessDelete
    {
        public void DeleteVisibleFalseCategories(Entities objectContext, EntitiesUsers userContext)
        {
#if DEBUG

            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            //deletes categories from last to up..only visible false
            List<Category> lastCategories = objectContext.CategorySet.Where(cat => cat.visible == false && cat.last == true).ToList();
            if (lastCategories.Count > 0)
            {
                List<Category> parents = new List<Category>();

                foreach (Category category in lastCategories)
                {
                    if (category.parentID != null)
                    {
                        parents = objectContext.CategorySet.Where(cat => cat.visible == false && cat.ID == category.parentID).ToList();
                    }
                    else
                    {
                        parents = new List<Category>();
                    }

                    DeleteCategory(objectContext, userContext, category);

                    if (parents.Count > 0)
                    {
                        foreach (Category parent in parents)
                        {
                            DeleteCategory(objectContext, userContext, parent);
                        }
                    }
                }
            }

            lastCategories = objectContext.CategorySet.Where(cat => cat.visible == false && cat.last == true).ToList();
            if (lastCategories.Count > 0)
            {
                throw new BusinessException("There are still visible:false last categories which arent deleted.");
            }

            List<Category> nonLastCategoris = objectContext.CategorySet.Where(cat => cat.visible == false).ToList();
            if (nonLastCategoris.Count > 0)
            {
                List<Category> parents = new List<Category>();

                foreach (Category category in nonLastCategoris)
                {

                    if (category.parentID != null)
                    {
                        parents = objectContext.CategorySet.Where(cat => cat.visible == false && cat.ID == category.parentID).ToList();
                    }
                    else
                    {
                        parents = new List<Category>();
                    }

                    DeleteCategory(objectContext, userContext, category);

                    if (parents.Count > 0)
                    {
                        foreach (Category parent in parents)
                        {
                            DeleteCategory(objectContext, userContext, category);
                        }
                    }

                }
            }
#endif
        }

        private void DeleteCategory(Entities objectContext, EntitiesUsers userContext, Category category)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            if (category == null)
            {
                throw new BusinessException("category is null");
            }

            List<Category> childs = objectContext.CategorySet.Where(cat => cat.parentID == category.ID).ToList();
            if (childs.Count > 0)
            {
                foreach (Category child in childs)
                {
                    DeleteCategory(objectContext, userContext, child);
                }
            }

            // ads for category
            DeleteCategoryAdvertisements(objectContext, category);

            // reports
            DeleteCategoryReports(objectContext, category);

            // products
            List<Product> products = objectContext.ProductSet.Where(prod => prod.Category.ID == category.ID).ToList();
            if (products.Count > 0)
            {
                foreach (Product product in products)
                {
                    DeleteProduct(objectContext, userContext, product);
                }
            }

            // company categories
            List<CategoryCompany> compcats = objectContext.CategoryCompanySet.Where(cc => cc.Category.ID == category.ID).ToList();
            if (compcats.Count > 0)
            {
                foreach (CategoryCompany compcat in compcats)
                {
                    objectContext.DeleteObject(compcat);
                    Tools.Save(objectContext);
                }
            }

            Category currCategory = objectContext.CategorySet.FirstOrDefault(cat => cat.ID == category.ID);
            if (currCategory != null)
            {
                objectContext.DeleteObject(currCategory);
                Tools.Save(objectContext);
            }
        }

        private void DeleteProduct(Entities objectContext, EntitiesUsers userContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            // product ratings
            DeleteProductRatings(objectContext, product);

            // product images
            DeleteProductImages(objectContext, product);

            // product advertisements
            DeleteProductAdvertisements(objectContext, product);

            // product reports
            DeleteProductReports(objectContext, product);

            // product comments
            DeleteProductComments(objectContext, product);

            // type suggestions
            DeleteProductTypeSuggestions(objectContext, product);

            // product characteristics
            DeleteProductCharacteristics(objectContext, product);

            // product variants
            DeleteProductVariantsAndSubVariants(objectContext, product);

            // notifies
            DeleteProductNotifies(objectContext, product);

            // product type actions and user actions
            DeleteProductRolesAndUserRoles(objectContext, userContext, product);

            objectContext.DeleteObject(product);
            Tools.Save(objectContext);
        }

        private void DeleteProductRatings(Entities objectContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }


            List<ProductRating> ratings = objectContext.ProductRatingSet.Where(rat => rat.Product.ID == product.ID).ToList();

            if (ratings.Count > 0)
            {
                foreach (ProductRating rating in ratings)
                {
                    objectContext.DeleteObject(rating);
                    Tools.Save(objectContext);
                }
            }
        }

        private void DeleteProductImages(Entities objectContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }


            List<ProductImage> images = objectContext.ProductImageSet.Where(img => img.Product.ID == product.ID).ToList();

            if (images.Count > 0)
            {
                foreach (ProductImage image in images)
                {
                    objectContext.DeleteObject(image);
                    Tools.Save(objectContext);
                }
            }
        }

        private void DeleteProductAdvertisements(Entities objectContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }


            List<AdvertisementsForProduct> adverts = objectContext.AdvertisementsForProductSet.Where(adv => adv.Product.ID == product.ID).ToList();

            if (adverts.Count > 0)
            {
                foreach (AdvertisementsForProduct advert in adverts)
                {
                    objectContext.DeleteObject(advert);
                    Tools.Save(objectContext);
                }
            }
        }

        private void DeleteProductComments(Entities objectContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            List<Comment> comments = objectContext.CommentSet.Where(comm => comm.type == "product" && comm.typeID == product.ID).ToList();

            if (comments.Count > 0)
            {
                foreach (Comment comment in comments)
                {
                    DeleteComment(objectContext, comment);
                }
            }
        }


        private void DeleteComment(Entities objectContext, Comment comment)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }

            DeleteCommentRatings(objectContext, comment);
            DeleteCommentReports(objectContext, comment);

            objectContext.DeleteObject(comment);
            Tools.Save(objectContext);
        }


        private void DeleteCommentReports(Entities objectContext, Comment comment)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }

            List<Report> reports = objectContext.ReportSet.Where(rep => rep.aboutType == "comment" && rep.aboutTypeId == comment.ID).ToList();
            if (reports.Count > 0)
            {
                foreach (Report report in reports)
                {
                    DeleteReport(objectContext, report);
                }
            }

        }

        private void DeleteCommentRatings(Entities objectContext, Comment comment)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }

            List<CommentRating> ratings = objectContext.CommentRatingSet.Where(rat => rat.Comment.ID == comment.ID).ToList();
            if (ratings.Count > 0)
            {
                foreach (CommentRating rating in ratings)
                {
                    objectContext.DeleteObject(rating);
                    Tools.Save(objectContext);
                }
            }

        }


        private void DeleteProductReports(Entities objectContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            List<Report> reports = objectContext.ReportSet.Where(rep => rep.aboutType == "product" && rep.aboutTypeId == product.ID).ToList();

            if (reports.Count > 0)
            {
                foreach (Report report in reports)
                {
                    DeleteReport(objectContext, report);
                }
            }
        }

        private void DeleteReport(Entities objectContext, Report report)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (report == null)
            {
                throw new BusinessException("report is null");
            }

            List<ReportComment> comments = objectContext.ReportCommentSet.Where(comm => comm.Report.ID == report.ID).ToList();

            if (comments.Count > 0)
            {
                foreach (ReportComment comment in comments)
                {
                    objectContext.DeleteObject(comment);
                    Tools.Save(objectContext);
                }
            }

            objectContext.DeleteObject(report);
            Tools.Save(objectContext);
        }

        private void DeleteProductTypeSuggestions(Entities objectContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            List<TypeSuggestion> suggestions = objectContext.TypeSuggestionSet.Where(sugg => sugg.type == "product" && sugg.typeID == product.ID).ToList();

            if (suggestions.Count > 0)
            {
                foreach (TypeSuggestion suggestion in suggestions)
                {
                    DeleteTypeSuggestion(objectContext, suggestion);
                }
            }
        }

        private void DeleteTypeSuggestion(Entities objectContext, TypeSuggestion suggestion)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (suggestion == null)
            {
                throw new BusinessException("suggestion is null");
            }

            List<TypeSuggestionComment> comments = objectContext.TypeSuggestionCommentSet.Where(comm => comm.Suggestion.ID == suggestion.ID).ToList();

            if (comments.Count > 0)
            {
                foreach (TypeSuggestionComment comment in comments)
                {
                    objectContext.DeleteObject(comment);
                    Tools.Save(objectContext);
                }
            }

            objectContext.DeleteObject(suggestion);
            Tools.Save(objectContext);
        }

        private void DeleteProductCharacteristics(Entities objectContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            List<ProductCharacteristics> characteristics = objectContext.ProductCharacteristicsSet.Where
                (chars => chars.Product.ID == product.ID).ToList();

            if (characteristics.Count > 0)
            {
                foreach (ProductCharacteristics characteristic in characteristics)
                {
                    objectContext.DeleteObject(characteristic);
                    Tools.Save(objectContext);
                }
            }
        }

        private void DeleteProductVariantsAndSubVariants(Entities objectContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            List<ProductSubVariant> subvariants = objectContext.ProductSubVariantSet.Where
                (sv => sv.Product.ID == product.ID).ToList();

            if (subvariants.Count > 0)
            {
                foreach (ProductSubVariant subvar in subvariants)
                {
                    objectContext.DeleteObject(subvar);
                    Tools.Save(objectContext);
                }
            }

            List<ProductVariant> variants = objectContext.ProductVariantSet.Where
                (sv => sv.Product.ID == product.ID).ToList();

            if (variants.Count > 0)
            {
                foreach (ProductVariant variant in variants)
                {
                    objectContext.DeleteObject(variant);
                    Tools.Save(objectContext);
                }
            }

        }

        private void DeleteProductRolesAndUserRoles(Entities objectContext, EntitiesUsers userContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            TypeAction productAction = objectContext.TypeActionSet.FirstOrDefault(ta => ta.type == "product" && ta.typeID == product.ID);
            if (productAction != null)
            {

                List<UsersTypeAction> actions = objectContext.UsersTypeActionSet.Where
                    (uta => uta.TypeAction.ID == productAction.ID).ToList();

                if (actions.Count > 0)
                {
                    List<TransferTypeAction> transfers = new List<TransferTypeAction>();
                    List<TypeWarning> warnings = new List<TypeWarning>();

                    BusinessUser bUser = new BusinessUser();
                    User warningUser = null;

                    foreach (UsersTypeAction action in actions)
                    {
                        transfers = objectContext.TransferTypeActionSet.Where(tr => tr.UserTypeAction.ID == action.ID).ToList();
                        if (transfers.Count > 0)
                        {
                            foreach (TransferTypeAction transfer in transfers)
                            {
                                objectContext.DeleteObject(transfer);
                                Tools.Save(objectContext);
                            }
                        }

                        warnings = objectContext.TypeWarningSet.Where(tw => tw.UserTypeAction.ID == action.ID).ToList();
                        if (warnings.Count > 0)
                        {
                            foreach (TypeWarning warning in warnings)
                            {
                                if (!warning.UserReference.IsLoaded)
                                {
                                    warning.UserReference.Load();
                                }
                                warningUser = bUser.GetWithoutVisible(userContext, warning.User.ID, false);

                                objectContext.DeleteObject(warning);
                                Tools.Save(objectContext);

                                if (!warningUser.UserOptionsReference.IsLoaded)
                                {
                                    warningUser.UserOptionsReference.Load();
                                }

                                warningUser.UserOptions.warnings--;
                                Tools.Save(objectContext);
                            }
                        }


                        objectContext.DeleteObject(action);
                        Tools.Save(objectContext);
                    }
                }
            }

            objectContext.DeleteObject(productAction);
            Tools.Save(objectContext);
        }

        private void DeleteProductNotifies(Entities objectContext, Product product)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            List<NotifyOnNewContent> notifies = objectContext.NotifyOnNewContentSet.Where
                (ns => ns.typeID == product.ID && ns.type == "product").ToList();

            if (notifies.Count > 0)
            {
                foreach (NotifyOnNewContent notify in notifies)
                {
                    objectContext.DeleteObject(notify);
                    Tools.Save(objectContext);
                }
            }
        }

        private void DeleteCategoryAdvertisements(Entities objectContext, Category category)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (category == null)
            {
                throw new BusinessException("category is null");
            }


            List<AdvertisementsForCategory> adverts = objectContext.AdvertisementsForCategorySet.Where(adv => adv.Category.ID == category.ID).ToList();

            if (adverts.Count > 0)
            {
                foreach (AdvertisementsForCategory advert in adverts)
                {
                    objectContext.DeleteObject(advert);
                    Tools.Save(objectContext);
                }
            }
        }

        private void DeleteCategoryReports(Entities objectContext, Category category)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (category == null)
            {
                throw new BusinessException("category is null");
            }

            List<Report> reports = objectContext.ReportSet.Where(rep => rep.aboutType == "category" && rep.aboutTypeId == category.ID).ToList();

            if (reports.Count > 0)
            {
                foreach (Report report in reports)
                {
                    DeleteReport(objectContext, report);
                }
            }
        }
    }
}
