// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;
using log4net;

namespace BusinessLayer
{
    public class BusinessRating
    {
        private static object ratingSync = new object();
        private static ILog log = LogManager.GetLogger(typeof(BusinessRating));

        public void RateProduct(Entities objectContext, User User, Product product, int rating, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (product == null)
            {
                throw new BusinessException("product is null");
            }
            if (User == null)
            {
                throw new BusinessException("User is null");
            }

            if (rating == 0)
            {
                throw new BusinessException("rating is 0");
            }

            if (IsProductRatedByUser(objectContext, product, User) == true)
            {
                throw new BusinessException(string.Format("user id = {0}, cannot rate AGAIN product id = {1}"
                    , User.ID, product.ID));
            }

            if (rating < 1 || rating > 6)
            {
                string msg = string.Format("Invalid rating: {0}.", rating);
                throw new ArgumentException(msg, "rating");
            }

            ProductRating newRating = new ProductRating();

            newRating.Product = product;
            newRating.rating = rating;

            newRating.dateCreated = DateTime.UtcNow;
            newRating.User = Tools.GetUserID(objectContext, User);
            newRating.LastModifiedBy = newRating.User;
            newRating.lastModified = newRating.dateCreated;
            newRating.visible = true;

            objectContext.AddToProductRatingSet(newRating);
            Tools.Save(objectContext);

            UpdateProductRating(objectContext, product, User, rating);
            bLog.LogRateProduct(objectContext, product, newRating, LogType.create, User);
        }

        public void RateComment(Entities objectContext, User User, Comment comment, int rating, BusinessLog bLog, DateTime userLocalTimeNow)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }
            if (User == null)
            {
                throw new BusinessException("User is null");
            }

            if (rating == 0)
            {
                throw new BusinessException("rating is 0");
            }

            string error = string.Empty;

            if (CanUserRateThisComment(objectContext, User, comment, userLocalTimeNow, out error) == false)
            {
                throw new BusinessException(string.Format("user id = {0} cannot rate comment id = {1}, Error : {2}"
                    , User.ID, comment.ID, error));
            }

            if (rating != 1 && rating != -1)
            {
                string msg = string.Format("Invalid rating: {0}.", rating);
                throw new ArgumentException(msg, "rating");
            }

            CommentRating newRating = new CommentRating();

            newRating.User = Tools.GetUserID(objectContext, User);
            newRating.Comment = comment;
            newRating.rating = rating;
            newRating.dateCreated = DateTime.UtcNow;
            newRating.lastModified = newRating.dateCreated;
            newRating.LastModifiedBy = newRating.User;
            newRating.visible = true;

            objectContext.AddToCommentRatingSet(newRating);
            Tools.Save(objectContext);

            UpdateCommentRating(objectContext, comment, rating);
            bLog.LogRateComment(objectContext, comment, newRating, LogType.create, User);
        }

        /// <summary>
        /// Returns product`s rating
        /// </summary>
        public String GetProductRating(Product currentProduct)
        {
            if (currentProduct == null)
            {
                throw new BusinessException("currentProduct is null");
            }

            string strRating = Tools.GetConfigurationResource("RateProductNotRated");

            double summary = 0;
            long rating = currentProduct.rating;
            if (rating > 0)
            {
                long usersRated = currentProduct.usersRated;
                if (usersRated > 0)
                {
                    summary = (1.0 * rating) / usersRated;

                    strRating = summary.ToString(".##");
                }
                else
                {
                    throw new BusinessException(string.Format("no users rated and rating is above 0 product with id {0}", currentProduct.ID));
                }
            }

            return strRating;
        }

        /// <summary>
        /// Checks if Product is rated by user
        /// </summary>
        /// <returns>true if its rated , otherwise false</returns>
        public Boolean IsProductRatedByUser(Entities objectContext, Product currProduct, User currUser)
        {
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            Boolean result = false;

            currProduct.ProductRatings.Load();
            if (currProduct.ProductRatings.Count > 0)
            {
                ProductRating isProdRated = objectContext.ProductRatingSet.FirstOrDefault
                    (rat => rat.Product.ID == currProduct.ID && rat.User.ID == currUser.ID);
                if (isProdRated != null)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Changes Products Rating 
        /// </summary>
        /// <param name="rate">between 1 and 6</param>
        private void UpdateProductRating(Entities objectContext, Product currProduct, User currUser, int rate)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currProduct == null)
            {
                throw new BusinessException("currProduct = null");
            }

            if (currUser == null)
            {
                throw new BusinessException("invalid curr user");
            }
            if (rate < 1 || rate > 6)
            {
                string msg = string.Format("Invalid rate: {0}.", rate);
                throw new ArgumentException(msg, "rate");
            }

            lock (ratingSync)
            {
                long oldRating = currProduct.rating;
                long oldUsersRated = currProduct.usersRated;

                currProduct.rating += rate;
                currProduct.usersRated += 1;

                Tools.Save(objectContext);
            }
        }

        private void UpdateCommentRating(Entities objectContext, Comment currComment, int rate)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currComment == null)
            {
                throw new BusinessException("currComment = null");
            }

            if (rate != 1 && rate != -1)
            {
                string msg = string.Format("Invalid rate: {0}.", rate);
                throw new ArgumentException(msg, "rate");
            }

            lock (ratingSync)
            {
                if (rate == 1)
                {
                    currComment.agrees++;
                }
                else
                {
                    currComment.disagrees++;
                }

                Tools.Save(objectContext);
            }
        }


        /// <summary>
        /// Checks if user can rate comment
        /// </summary>
        /// <returns>true if user can rate , otherwise false</returns>
        public Boolean CanUserRateThisComment(Entities objectContext, User user, Comment currComment, DateTime userLocalTimeNow, out string error)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }
            if (currComment == null)
            {
                throw new BusinessException("currComment is null");
            }

            error = string.Empty;

            userLocalTimeNow = userLocalTimeNow.Date;

            int lastDayCommRatings = objectContext.CommentRatingSet.Count(cm => cm.User.ID == user.ID && cm.dateCreated >= userLocalTimeNow);

            if (lastDayCommRatings >= Configuration.CommRatingMaxUserRatingsForDay)
            {
                error = Tools.GetResource("errCommRatMaxRatingsForDayReached");
                return false;
            }

            Boolean result = false;
            Boolean countChecksPassed = true;

            BusinessComment bComment = new BusinessComment();
            CommentType commType = bComment.GetCommentTypeFromString(currComment.type);

            string errorCommRated = string.Empty;
            string errorCantRateUrComms = string.Empty;

            switch (commType)
            {
                case CommentType.Product:

                    errorCommRated = Tools.GetResource("errOpinionRatAlreadyRated");
                    errorCantRateUrComms = Tools.GetResource("errOpinionCantRateUrs");

                    BusinessProduct bProduct = new BusinessProduct();
                    Product commentProduct = bProduct.GetProductByID(objectContext, currComment.typeID);
                    if (commentProduct != null)
                    {

                        if (bProduct.CheckIfProductsIsValidWithConnections(objectContext, commentProduct, out error))
                        {
                            int prodCommRatings = objectContext.CommentRatingSet.Count(cm => cm.User.ID == user.ID && cm.Comment.type == currComment.type
                                && cm.Comment.typeID == currComment.typeID);

                            if (prodCommRatings >= Configuration.CommRatingMaxUserRatingsPerProduct)
                            {
                                countChecksPassed = false;
                                error = Tools.GetResource("errCommRatMaxRatingsForProductReached");
                            }
                        }
                        else
                        {
                            error = string.Empty;
                            countChecksPassed = false;
                        }

                    }
                    else
                    {
                        countChecksPassed = false;
                    }

                    break;
                case CommentType.Topic:

                    errorCommRated = Tools.GetResource("errCommRatAlreadyRated");
                    errorCantRateUrComms = Tools.GetResource("errCommCantRateUrs");

                    BusinessProductTopics bpTopic = new BusinessProductTopics();
                    ProductTopic topic = bpTopic.Get(objectContext, currComment.typeID, true, false);
                    if (topic != null && topic.visible == true && topic.locked == false)
                    {
                        if (!topic.ProductReference.IsLoaded)
                        {
                            topic.ProductReference.Load();
                        }

                        BusinessProduct businessProduct = new BusinessProduct();
                        if (businessProduct.CheckIfProductsIsValidWithConnections(objectContext, topic.Product, out error))
                        {
                            int topicCommRatings = objectContext.CommentRatingSet.Count(cm => cm.User.ID == user.ID && cm.Comment.type == currComment.type
                                && cm.Comment.typeID == currComment.typeID);

                            if (topicCommRatings >= Configuration.CommRatingMaxUserRatingsPerTopic)
                            {
                                countChecksPassed = false;
                                error = Tools.GetResource("errCommRatMaxRatingsForTopicReached");
                            }

                        }
                        else
                        {
                            error = string.Empty;
                            countChecksPassed = false;
                        }

                    }
                    else
                    {
                        countChecksPassed = false;
                    }

                    break;
                default:
                    throw new BusinessException(string.Format("Comment type = {0} is not supported when checking if user can rate comment"
                        , commType));
            }


            if (countChecksPassed == true)
            {
                if (!currComment.UserIDReference.IsLoaded)
                {
                    currComment.UserIDReference.Load();
                }
                if (currComment.UserID.ID != user.ID)
                {
                    try
                    {
                        CommentRating commRating = objectContext.CommentRatingSet.FirstOrDefault(rat => rat.Comment.ID == currComment.ID && rat.User.ID == user.ID);
                        if (commRating == null)
                        {
                            result = true;
                        }
                        else
                        {
                            error = errorCommRated;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (log.IsErrorEnabled)
                        {
                            log.Error("Error retrieving comment ratings.", ex);
                        }
                    }
                }
                else
                {
                    error = errorCantRateUrComms;
                }
            }

            return result;
        }

    }
}
