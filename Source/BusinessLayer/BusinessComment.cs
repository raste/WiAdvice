// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Collections;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessComment
    {
        object DeletingComment = new object();

        /// <summary>
        /// Adds new comment , and if its type is product or subcomment increases that type`s comments with 1
        /// </summary>
        private void Add(EntitiesUsers userContext, Entities objectContext, Comment newComment, BusinessLog bLog, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(userContext);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (newComment == null)
            {
                throw new ArgumentNullException("newComment");
            }

            objectContext.AddToCommentSet(newComment);
            Tools.Save(objectContext);

            UpdateParentCommentSubCommentsField(objectContext, newComment);
            bLog.LogComment(objectContext, newComment, LogType.create, string.Empty, string.Empty, currUser);

            CommentType commType = GetCommentTypeFromString(newComment.type);

            BusinessStatistics businessStatistic = new BusinessStatistics();
            businessStatistic.CommentWritten(userContext, objectContext);

            BusinessNotifies businessNotifies = new BusinessNotifies();

            switch (commType)
            {
                case CommentType.Product:

                    long charID = 0;
                    long varID = 0;
                    long subVarID = 0;

                    GetSubCommentCharIdVariantOrSubVariant(objectContext, newComment, out charID, out varID, out subVarID);
                    IncreaseTypeCommentsAndVisits(objectContext, userContext, commType, newComment.typeID, charID, varID, subVarID, newComment);

                    businessNotifies.UpdateNotifiesForType(objectContext, NotifyType.Product, newComment.typeID, currUser);

                    break;
                case CommentType.Topic:

                    IncreaseTypeCommentsAndVisits(objectContext, userContext, commType, newComment.typeID, 0, 0, 0, newComment);
                    businessNotifies.UpdateNotifiesForType(objectContext, NotifyType.ProductTopic, newComment.typeID, currUser);

                    break;
                default:
                    throw new BusinessException(string.Format("Comment type = {0} is not supported when adding new comments", commType));
            }

        }


        /// <summary>
        /// Adds comment to database. Guestname is not used if currUser != null, parentCommId is not used if type != Subcomment,
        /// charID 0 for no characteristic otherwise characteristic ID
        /// </summary>
        public void AddProductComment(EntitiesUsers userContext, Entities objectContext, User currUser, string guestName, Product product,
            ProductCharacteristics prodCharacteristic, string description, BusinessLog bLog, string ipAdress,
            CommentSubType subType, long subTypeId, ProductVariant variant, ProductSubVariant subVariant)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            bool byGuest = false;

            if (currUser == null)
            {
                if (string.IsNullOrEmpty(guestName))
                {
                    throw new BusinessException("guestName is empty");
                }
                BusinessUser businessUser = new BusinessUser();
                currUser = businessUser.GetGuest(userContext);

                byGuest = true;
            }

            if (product == null)
            {
                throw new BusinessException("product is null");
            }

            if (subTypeId < 1)
            {
                throw new BusinessException("subTypeId < 1");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty or null");
            }

            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty or null");
            }

            Comment newComment = new Comment();

            newComment.UserID = Tools.GetUserID(objectContext, currUser);
            if (byGuest == true)
            {
                newComment.guestname = guestName;
            }
            else
            {
                newComment.guestname = null;
            }
            newComment.type = GetCommentTypeFromEnum(CommentType.Product);
            if (CommentSubType.SubComment == subType)
            {
                newComment.ForCharacteristic = null;
                newComment.ForSubVariant = null;
                newComment.ForVariant = null;
            }
            else
            {
                newComment.ForCharacteristic = prodCharacteristic;
                newComment.ForSubVariant = subVariant;
                newComment.ForVariant = variant;
            }
            newComment.typeID = product.ID;

            newComment.dateCreated = DateTime.UtcNow;
            newComment.description = description;
            newComment.visible = true;
            newComment.lastModified = newComment.dateCreated;
            newComment.LastModifiedBy = newComment.UserID;
            newComment.agrees = 0;
            newComment.disagrees = 0;
            newComment.haveSubcomments = false;
            newComment.ipAdress = ipAdress;


            newComment.subType = GetCommentSubTypeFromEnum(subType);
            newComment.subTypeID = subTypeId;

            Add(userContext, objectContext, newComment, bLog, currUser);
        }

        /// <summary>
        /// Increases type comments with 1 and updates visits if the type supports them
        /// </summary>
        /// <param name="type">types : product</param>
        /// <param name="typeId">ID of the type</param>
        /// <param name="charId">if its about characteristic in product type its ID</param>
        private void IncreaseTypeCommentsAndVisits(Entities objectContext, EntitiesUsers userContext, CommentType type, long typeId, long charId
            , long varId, long subVarId, Comment newComment)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (newComment == null)
            {
                throw new BusinessException("newComment is null");
            }

            if (typeId < 1)
            {
                throw new BusinessException("typeId us < 1");
            }

            switch (type)
            {
                case CommentType.Product:
                    BusinessProduct businessProduct = new BusinessProduct();
                    BusinessProductVariant bpVariant = new BusinessProductVariant();

                    Product currProduct = businessProduct.GetProductByIDWV(objectContext, typeId);
                    if (currProduct == null)
                    {
                        throw new BusinessException(string.Format("The product with ID = {0} which comments need to" +
                            " be increased wasnt found", typeId));
                    }
                    if (charId > 0)
                    {
                        ProductCharacteristics prodChar = businessProduct.GetCharacteristicEvenIfVisibleFalse(objectContext, charId);
                        if (prodChar == null)
                        {
                            throw new BusinessException(string.Format("The product characteristic with ID = {0} which " +
                                " comments need to be increased wasnt found", charId));
                        }
                        businessProduct.IncreaseProductCharComments(objectContext, prodChar);
                    }
                    if (varId > 0)
                    {
                        ProductVariant currVariant = bpVariant.Get(objectContext, currProduct, varId, false, true);

                        businessProduct.IncreaseProductVariantComments(objectContext, currVariant);
                    }
                    if (subVarId > 0)
                    {
                        ProductSubVariant currSubVariant = bpVariant.GetSubVariant(objectContext, currProduct, subVarId, false, true);

                        businessProduct.IncreaseProductSubVariantComments(objectContext, currSubVariant);
                    }

                    businessProduct.IncreaseProductComments(objectContext, currProduct);
                    break;

                case CommentType.Topic:

                    BusinessProductTopics bTopic = new BusinessProductTopics();
                    ProductTopic topic = bTopic.Get(objectContext, typeId, false, true);
                    BusinessUser bUser = new BusinessUser();

                    bTopic.IncreaseTopicComments(objectContext, topic, newComment);

                    if (!newComment.UserIDReference.IsLoaded)
                    {
                        newComment.UserIDReference.Load();
                    }
                    User commentUser = bUser.GetWithoutVisible(userContext, newComment.UserID.ID, true);

                    BusinessVisits bVisits = new BusinessVisits();
                    bVisits.UpdateProductTopicVisited(objectContext, userContext, topic, commentUser, newComment.ipAdress);

                    break;
                default:
                    string error = string.Format("type {0} is not supported type.", type);
                    throw new BusinessException(error);
            }
        }

        /// <summary>
        /// Returns visible=true Comment with ID 
        /// </summary>
        public Comment Get(Entities objectContext, long id)
        {
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }
            Tools.AssertObjectContextExists(objectContext);
            Comment comment = objectContext.CommentSet.FirstOrDefault<Comment>(comm => comm.ID == id && comm.visible == true);
            return comment;
        }

        public List<Comment> GetLastComments(Entities objectContext, int number, int maxLength, string ipAdress, bool all, bool onlyVisibleTrue)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (number < 1)
            {
                throw new BusinessException("number < 1");
            }

            if (maxLength < 1)
            {
                throw new BusinessException("maxLength < 1");
            }

            if (string.IsNullOrEmpty(ipAdress))
            {
                ipAdress = "null";
            }

            List<Comment> comments = objectContext.GetLastComments(number, ipAdress, maxLength, all, onlyVisibleTrue).ToList();

            return comments;
        }

        /// <summary>
        /// Returns Username of the Comment`s Writer
        /// </summary>
        /// <param name="id">Comment id</param>
        /// <returns></returns>
        public String GetCommentUsername(EntitiesUsers userContext, Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }

            Comment comment = objectContext.CommentSet.FirstOrDefault<Comment>(comm => comm.ID == id);
            if (comment == null)
            {
                throw new BusinessException(string.Format("theres no comment with id {0}", id));
            }

            String name;
            if (string.IsNullOrEmpty(comment.guestname))
            {
                BusinessUser businessUser = new BusinessUser();
                if (!comment.UserIDReference.IsLoaded)
                {
                    comment.UserIDReference.Load();
                }
                name = businessUser.GetUserName(userContext, comment.UserID.ID);
            }
            else
            {
                name = comment.guestname;
            }
            return name;
        }

        /// <summary>
        /// Returns Comment Description
        /// </summary>
        /// <param name="id">Comment id</param>
        /// <returns></returns>
        public String GetCommentDescription(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            Comment comment = objectContext.CommentSet.FirstOrDefault<Comment>(comm => comm.ID == id);
            if (comment == null)
            {
                throw new BusinessException(string.Format("theres no comment with id " + id));
            }

            return comment.description;
        }

        /// <summary>
        /// returns last visible.true comment for topic
        /// </summary>
        public Comment GetLastCommentForTopic(Entities objectContext, ProductTopic topic)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            Comment comment = null;
            List<Comment> comments = objectContext.CommentSet.Where(cm => cm.type == "topic" && cm.typeID == topic.ID && cm.visible == true).ToList();

            if (comments != null && comments.Count > 0)
            {
                comment = comments.Last();
            }

            return comment;
        }

        /// <summary>
        /// Gets Comment About type , typeID , and if its about Characateristic charID
        /// </summary>
        public void GetSubCommentTypeAndTypeIdOLD(Entities objectContext, Comment comment, out string type, out long typeID, out long charID)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }

            type = "";
            typeID = 0;
            charID = 0;

            int cycles = 0;
            long lastID = 0;
            while (comment.type == "subcomment" && cycles < 100)
            {
                lastID = comment.typeID;
                comment = GetWithoutVisible(objectContext, comment.typeID);
                if (comment == null)
                {
                    throw new BusinessException(string.Format("there`s no comment with id {0}", lastID));
                }
                cycles++;
            }

            if (comment.type == "subcomment" && cycles >= 100)
            {
                throw new BusinessException(string.Format("System couldnt find subcomment`s {0} type in 100 cycles", comment.ID));
            }

            if (!comment.ForCharacteristicReference.IsLoaded)
            {
                comment.ForCharacteristicReference.Load();
            }

            type = comment.type;
            typeID = comment.typeID;
            if (comment.ForCharacteristic == null)
            {
                charID = 0;
            }
            else
            {
                charID = comment.ForCharacteristic.ID;
            }

        }

        public void GetSubCommentCharIdVariantOrSubVariant(Entities objectContext, Comment comment
            , out long charID, out long varId, out long subVarId)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }

            charID = 0;
            varId = 0;
            subVarId = 0;

            int cycles = 0;
            long lastID = 0;
            while (comment.subType == "subcomment" && cycles < 100)
            {
                lastID = comment.subTypeID;
                comment = GetWithoutVisible(objectContext, comment.subTypeID);
                if (comment == null)
                {
                    throw new BusinessException(string.Format("there`s no comment with id {0}", lastID));
                }
                cycles++;
            }

            if (comment.subType == "subcomment" && cycles >= 100)
            {
                throw new BusinessException(string.Format("System couldnt find subcomment`s {0} type in 100 cycles", comment.ID));
            }

            if (!comment.ForCharacteristicReference.IsLoaded)
            {
                comment.ForCharacteristicReference.Load();
            }
            if (!comment.ForVariantReference.IsLoaded)
            {
                comment.ForVariantReference.Load();
            }
            if (!comment.ForSubVariantReference.IsLoaded)
            {
                comment.ForSubVariantReference.Load();
            }

            if (comment.ForCharacteristic != null)
            {
                charID = comment.ForCharacteristic.ID;
            }
            else if (comment.ForVariant != null)
            {
                varId = comment.ForVariant.ID;
            }
            else if (comment.ForSubVariant != null)
            {
                subVarId = comment.ForSubVariant.ID;
            }

        }

        /// <summary>
        /// Returns SubComment level, 0 if the comment isn`t subcomment
        /// </summary>
        public int GetSubCommentLevel(Entities objectContext, Comment comment)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }

            int level = 1;
            int cycles = 1;
            long lastID = 0;

            while (comment.subType == "subcomment" && cycles < 100)
            {
                lastID = comment.subTypeID;
                comment = GetWithoutVisible(objectContext, lastID);
                if (comment == null)
                {
                    throw new BusinessException(string.Format("there`s no comment with id {0}", lastID));
                }
                cycles++;
                level++;
            }

            if (comment.subType == "subcomment" && cycles >= 100)
            {
                throw new BusinessException(string.Format("System couldnt find subcomment`s {0} level in 100 cycles", comment.ID));
            }

            return level;

        }

        /// <summary>
        /// Returns Comment with ID
        /// </summary>
        /// <param name="id">Comment ID</param>
        public Comment GetWithoutVisible(Entities objectContext, long id)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }
            Comment comment = objectContext.CommentSet.FirstOrDefault<Comment>(comm => comm.ID == id);
            return comment;
        }

        /// <summary>
        /// Returns All visible=true comments written by user (comment type : product,subcomment)
        /// </summary>
        /// true if last posted comments should bt in front, othwerwise false
        /// </param>
        public IEnumerable<Comment> GetAllFromUser(EntitiesUsers userContext, Entities objectContext, long? userID, Boolean sortedByDesc, long from, long to)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);

            if (userID == null || userID < 1)
            {
                throw new BusinessException("invalid userID to get comments from");
            }

            if (from >= to || from < 0)
            {
                throw new BusinessException("invalid from-to range");
            }

            BusinessUser businessUser = new BusinessUser();
            User user = businessUser.GetWithoutVisible(userContext, userID.Value, true);

            IEnumerable<Comment> comments = objectContext.GetCommentsFromUser(user.ID, from, to, sortedByDesc);

            List<Comment> allExtractedComments = comments.ToList<Comment>();

            return allExtractedComments; 
        }

        /// <summary>
        /// Returns all visible=true comments from user
        /// </summary>
        public List<Comment> GetAllFromUser(Entities objectContext, User user)
        {
            Tools.AssertObjectContextExists(objectContext);

            BusinessUser businessUser = new BusinessUser();
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            UserID currUser = Tools.GetUserID(objectContext, user);

            currUser.Comments.Load();

            IEnumerable<Comment> comments = currUser.Comments.Where
            (comm => comm.visible == true);

            return comments.ToList();
        }

        /// <summary>
        /// Return number of all visible=true user comments
        /// </summary>
        public long CountUserComments(Entities objectContext, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            long count = objectContext.CommentSet.Count
                    (comm => comm.UserID.ID == currUser.ID && comm.visible == true);

            return count;
        }

        /// <summary>
        /// Returns all visible=true comments written about product/product characteristic/variant/subvariant (that doesnt include subcomments)
        /// true if comments should be sorted by date DESC; SortByRating = 0 for no sorting by rating, 1 for sorting by rating Biggest > Lowest, 
        /// 2 for sorting lowest > biggest; charId = 0 for not sorting by characteristic , ID for sorting by characteristic
        /// </summary>
        /// <param name="productID">preoduct id</param>
        /// <param name="SortedByDesc">true is they should be sorted by Desc , otherwise false</param>
        public IEnumerable<Comment> GetAllFromProduct(Entities objectContext, long? productID, Boolean SortedByDesc,
            SortOptions SortByRating, long charId, long from, long to, long variantID, long subVariantID, bool noAbout)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.CheckFromToParameters(from, to);

            if ((productID == null) || (productID.Value < 1))
            {
                throw new BusinessException("productID is null or < 1");
            }

            long checkId = charId;
            if (checkId > 0 && variantID > 0)
            {
                throw new BusinessException("only one of the : charId, variantID or subVariantID can be more than 0");
            }
            checkId += variantID;
            if (checkId > 0 && subVariantID > 0)
            {
                throw new BusinessException("only one of the : charId, variantID or subVariantID can be more than 0");
            }
            checkId += subVariantID;
            if (checkId > 0 && noAbout == true)
            {
                throw new BusinessException("only one of the : charId, variantID, subVariantID or noAbout can be more than 0 or 'true'");
            }

            IEnumerable<Comment> comments = null;

            if (SortByRating == SortOptions.None  // 0
                )
            {
                if (checkId < 1 && noAbout == false)
                {
                    comments = objectContext.GetCommentsFromProduct(productID, from, to, SortedByDesc);
                }
                else if (charId > 0)
                {
                    comments = objectContext.GetCommentsFromProductCharacteristic(charId, from, to, SortedByDesc);
                }
                else if (variantID > 0)
                {
                    comments = objectContext.GetCommentsFromProductVariant(variantID, from, to, SortedByDesc);
                }
                else if (subVariantID > 0)
                {
                    comments = objectContext.GetCommentsFromProductSubVariant(subVariantID, from, to, SortedByDesc);
                }
                else if (noAbout == true)
                {
                    comments = objectContext.GetCommentsFromProductWithNoAbout(productID, from, to, SortedByDesc);
                }
            }
            else
            {

                switch (SortByRating)
                {
                    case SortOptions.Descending:  // (1):
                        // sort by desc
                        if (checkId < 1 && noAbout == false)
                        {
                            comments = objectContext.GetCommentsFromProductByRating(productID, from, to, true);
                        }
                        else if (charId > 0)
                        {
                            comments = objectContext.GetCommentsFromProductCharacteristicByRating(charId, from, to, true);
                        }
                        else if (variantID > 0)
                        {
                            comments = objectContext.GetCommentsFromProductVariantByRating(variantID, from, to, true);
                        }
                        else if (subVariantID > 0)
                        {
                            comments = objectContext.GetCommentsFromProductSubVariantByRating(subVariantID, from, to, true);
                        }
                        else if (noAbout == true)
                        {
                            comments = objectContext.GetCommentsFromProductWithNoAboutByRating(productID, from, to, true);
                        }

                        break;
                    case SortOptions.Ascending:  // (2):

                        // sort by asc
                        if (checkId < 1 && noAbout == false)
                        {
                            comments = objectContext.GetCommentsFromProductByRating(productID, from, to, false);
                        }
                        else if (charId > 0)
                        {
                            comments = objectContext.GetCommentsFromProductCharacteristicByRating(charId, from, to, false);
                        }
                        else if (variantID > 0)
                        {
                            comments = objectContext.GetCommentsFromProductVariantByRating(variantID, from, to, false);
                        }
                        else if (subVariantID > 0)
                        {
                            comments = objectContext.GetCommentsFromProductSubVariantByRating(subVariantID, from, to, false);
                        }
                        else if (noAbout == true)
                        {
                            comments = objectContext.GetCommentsFromProductWithNoAbout(productID, from, to, false);
                        }

                        break;
                    default:
                        throw new BusinessException(string.Format("SortByRating = '{0}' is not valid, Product Id = {1}",
                            SortByRating, productID.Value));
                }
            }
            return comments.ToList<Comment>();
        }

        /// <summary>
        /// Returns all visible=true Comments written about Product Characteristics
        /// </summary>
        /// <param name="SortedByDesc">trued if they should be sorted by Dessc , otherwise false</param>
        public IEnumerable<Comment> GetAllFromProductChar(Entities objectContext, long? charID, Boolean SortedByDesc)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Comment> comments;
            if ((charID == null) || (charID.Value < 1))
            {
                throw new BusinessException("charID is null or < 1");
            }

            if (SortedByDesc)
            {
                comments = objectContext.CommentSet.Where
                    (comm => comm.ForCharacteristic.ID == charID && comm.type == "product"
                        && comm.subType == "comment" && comm.visible == true).
                    OrderByDescending<Comment, long>(new Func<Comment, long>(IdSelector));
            }
            else
            {
                comments = objectContext.CommentSet.Where
                    (comm => comm.ForCharacteristic.ID == charID && comm.type == "product"
                        && comm.subType == "comment" && comm.visible == true);
            }

            return comments;
        }

        /// <summary>
        /// The maximum index (exclusive) of the IQueryable that determines that the Comment must be skipped by SkipWhile().
        /// <para>To be only used by FilterSkipCommentByQueryResultIndex(Comment, int) and
        /// GetAllFromProduct(Entities, long?, int, int)</para>
        /// </summary>
        private int ProductCommentsFromInd { get; set; }

        /// <summary>
        /// The  index (exclusive) of the IQueryable that determines that the Comment must be selected by TakeWhile().
        /// <para>To be only used by FilterCommentByQueryResultIndex(Comment, int) and
        /// GetAllFromProduct(Entities, long?, int, int)</para>
        /// </summary>
        private int ProductCommentsToInd { get; set; }

        /// <summary>
        /// Determines whether a comment must be skipped from selection by SkipWhile() accordingly to the supplied query result index.
        /// </summary>
        /// <returns><c>true</c> if the comment must be skipped; otherwise, <c>false</c>.</returns>
        private bool FilterSkipCommentByQueryResultIndex(Comment comment, int queryResultIndex)
        {
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }
            bool result = (queryResultIndex < ProductCommentsFromInd);
            return result;
        }

        /// <summary>
        /// Determines whether a comment must be include in selection by TakeWhile() accordingly to the supplied query result index.
        /// </summary>
        /// <returns><c>true</c> if the comment must be selected; otherwise, <c>false</c>.</returns>
        private bool FilterTakeCommentByQueryResultIndex(Comment comment, int queryResultIndex)
        {
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }
            bool result = (queryResultIndex < (ProductCommentsToInd - ProductCommentsFromInd));
            return result;
        }

        /// <summary>
        /// Gets all comments for the specified <see cref="Product"/> starting with and finishing with specified
        /// query result indexes.
        /// </summary>
        /// <param name="objectContext">The <see cref="Entities"/> to be used in order to access the database.</param>
        /// <param name="productID">The <see cref="Product"/> identifier.</param>
        /// <param name="fromIndex">The mimimum index (inclusive) of the IQueryable that determines that the 
        /// <see cref="Comment"/> must be selected.</param>
        /// <param name="toIndex">The maximum index (inclusive) of the IQueryable that determines that the 
        /// <see cref="Comment"/> must be selected.
        /// <para>Specify <c>int.MaxValue</c> to return all the <see cref="Comment"/> instances from the query result.</para></param>
        /// <returns>An IEnumerable&lt;Comment&gt; containing the selected <see cref="Comment"/> instances.</returns>
        public IEnumerable<Comment> GetAllFromProduct(Entities objectContext, long? productID, int fromIndex, int toIndex)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Comment> comments;
            if ((productID == null) || (productID.Value < 1))
            {
                throw new BusinessException("Error : invalid product !");
            }
            else
            {
                ObjectResult<Comment> allProductComments =
                    objectContext.GetCommentsRange("product", productID, fromIndex, toIndex);

                List<Comment> commentsList = new List<Comment>();

                // It is important to read all of the items that are returned by the stored procedure
                // in order the underlying DataReaded to be closed!
                foreach (Comment selectedComment in allProductComments)
                {
                    commentsList.Add(selectedComment);
                }
                comments = commentsList;
            }
            return comments;
        }

        /// <summary>
        /// Returns all Comment`s SubComments which are visible=true
        /// </summary>
        public IEnumerable<Comment> GetAllSubComments(Entities objectContext, long? commentID)
        {
            Tools.AssertObjectContextExists(objectContext);
            IEnumerable<Comment> comments;
            if ((commentID == null) || (commentID.Value < 1))
            {
                throw new BusinessException("commentID is null or < 1");
            }

            comments = objectContext.CommentSet.Where
                (comm => comm.subTypeID == commentID && comm.subType == "subcomment" && comm.visible == true);

            return comments;
        }

        /// <summary>
        /// Returns all Comment`s SubComments which are visible=true
        /// </summary>
        public IEnumerable<Comment> GetAllSubComments(Entities objectContext, Comment comment)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (comment == null)
            {
                throw new BusinessException("comment is null !");
            }

            IEnumerable<Comment> comments;
            comments =
                    objectContext.CommentSet.Where(comm => comm.subTypeID == comment.ID && comm.subType == "subcomment" && comm.visible == true);

            return comments;
        }

        /// <summary>
        /// Changes comment description
        /// </summary>
        /// <param name="newDescr">cannot be null or empty</param>
        public void ModifyCommentDescription(Entities objectContext, Comment comment, string newDescr, User modifier
            , BusinessLog bLog, CommentType commType)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);

            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }
            if (string.IsNullOrEmpty(newDescr))
            {
                throw new BusinessException("new description is null or empty");
            }
            if (modifier == null)
            {
                throw new BusinessException("modifier is null");
            }

            if (comment.description == newDescr)
            {
                return;
            }

            string oldDescr = comment.description;

            comment.description = newDescr;
            comment.lastModified = DateTime.UtcNow;
            comment.LastModifiedBy = Tools.GetUserID(objectContext, modifier);

            Tools.Save(objectContext);

            bLog.LogComment(objectContext, comment, LogType.edit, "description", oldDescr, modifier);
        }

        /// <summary>
        /// Makes Comment visible=false , and Decreases the number of comments of Comment`s type
        /// </summary>
        public void DeleteComment(Entities objectContext, EntitiesUsers userContext, Comment comment, User modifier
            , BusinessLog bLog, bool sendSystemMessage, bool sendWarning)
        {
            Tools.AssertBusinessLogExists(bLog);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }

            if (modifier == null)
            {
                throw new BusinessException("modifier is null");
            }

            if (comment.visible == false)
            {
                return;
            }

            if (!comment.UserIDReference.IsLoaded)
            {
                comment.UserIDReference.Load();
            }

            comment.visible = false;
            comment.lastModified = DateTime.UtcNow;
            comment.LastModifiedBy = Tools.GetUserID(objectContext, modifier);

            Tools.Save(objectContext);

            bLog.LogComment(objectContext, comment, LogType.delete, string.Empty, string.Empty, modifier);

            CommentType commType = GetCommentTypeFromString(comment.type);

            BusinessStatistics statistics = new BusinessStatistics();

            string warning = string.Empty;
            string sysMsg = string.Empty;

            switch (commType)
            {
                case CommentType.Product:

                    statistics.CommentDeleted(userContext, objectContext);

                    DecreaseTypeComments(objectContext, comment);


                    warning = string.Format("{0}<br />{1} ' {2} '", Tools.GetResource("DeletingCommentW")
                            , Tools.GetResource("DeletingCommentW2"), comment.description);
                    sysMsg = string.Format("{0} \" {1} \"", Tools.GetResource("SMProdCommentDeleted"), comment.description);

                    break;
                case CommentType.Topic:

                    DecreaseTypeComments(objectContext, comment);


                    warning = string.Format("{0}<br />{1} ' {2} '", Tools.GetResource("DeletingTopicCommentW")
                         , Tools.GetResource("DeletingTopicCommentW2"), comment.description);
                    sysMsg = string.Format("{0} \" {1} \"", Tools.GetResource("SMTopicCommentDeleted"), comment.description);

                    break;
                default:
                    throw new BusinessException(string.Format("Comment type = {0} is not supported when deleting comment", commType));
            }

            UpdateParentCommentSubCommentsField(objectContext, comment);

            if (comment.haveSubcomments == true)
            {
                List<Comment> Comments = GetAllSubComments(objectContext, comment).ToList();

                if (Comments != null && Comments.Count<Comment>() > 0)
                {
                    foreach (Comment comm in Comments)
                    {
                        lock (DeletingComment)
                        {
                            DeleteComment(objectContext, userContext, comm, modifier, bLog, sendSystemMessage, false);
                        }
                    }
                }
            }

            BusinessReport businessReport = new BusinessReport();
            businessReport.ResolveReportsForCommentWhichIsBeingDeleted(objectContext, userContext, comment, bLog, modifier);

            BusinessUser businessUser = new BusinessUser();
            User commUser = businessUser.GetWithoutVisible(userContext, comment.UserID.ID, true);

            if (commUser != null && commUser.visible == true)
            {
                if (sendWarning == true)
                {
                    BusinessUserActions buActions = new BusinessUserActions();
                    UserAction action = buActions.GetUserAction(userContext, UserRoles.WriteCommentsAndMessages, commUser);
                    if (action != null && action.visible == true)
                    {
                        BusinessWarnings bWarning = new BusinessWarnings();

                        UserRoles forRole = UserRoles.WriteCommentsAndMessages;
                        bWarning.AddWarning(userContext, objectContext, action, forRole.ToString(), warning, commUser, modifier, bLog);
                    }
                }

                if (commUser.ID != modifier.ID)
                {
                    if (sendSystemMessage == true && string.IsNullOrEmpty(comment.guestname))
                    {
                        if (businessUser.IsGuest(userContext, commUser))
                        {
                            throw new BusinessException(string.Format("Comment ID : {0} don`t have guestname and comment user is guest", comment.ID));
                        }

                        BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();

                        bSystemMessages.Add(userContext, commUser, sysMsg);
                    }
                }
            }
        }

        public void DeleteAllUserComments(Entities objectContext, EntitiesUsers userContext, User user, User modifier, BusinessLog bLog, bool sendWarnings)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (user == null)
            {
                throw new BusinessException("user is null");
            }
            if (modifier == null)
            {
                throw new BusinessException("modifier is null");
            }

            List<Comment> userComments = GetAllFromUser(objectContext, user);
            if (userComments.Count > 0)
            {
                foreach (Comment comment in userComments)
                {
                    DeleteComment(objectContext, userContext, comment, modifier, bLog, false, sendWarnings);
                }
            }

            BusinessSystemMessages bSystemMessages = new BusinessSystemMessages();
            string description = Tools.GetResource("allCommsWereDeleted");
            bSystemMessages.Add(userContext, user, description);
        }


        /// <summary>
        /// Decreases by 1 type comments
        /// </summary>
        /// <param name="type">types : product</param>
        private void DecreaseTypeComments(Entities objectContext, Comment comment)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }

            CommentType type = GetCommentTypeFromString(comment.type);

            switch (type)
            {
                case CommentType.Product:
                    BusinessProduct businessProduct = new BusinessProduct();
                    BusinessProductVariant bpVariant = new BusinessProductVariant();
                    Product currProduct = businessProduct.GetProductByIDWV(objectContext, comment.typeID);
                    if (currProduct == null)
                    {
                        throw new BusinessException(string.Format("The product ID = {0} which comments need to be decreased doesnt exist", comment.typeID));
                    }

                    long charID = 0;
                    long varId = 0;
                    long subVarID = 0;

                    GetSubCommentCharIdVariantOrSubVariant(objectContext, comment, out charID, out varId, out subVarID);

                    if (charID > 0)
                    {
                        ProductCharacteristics prodChar = businessProduct.GetCharacteristicEvenIfVisibleFalse(objectContext, charID);
                        if (prodChar == null)
                        {
                            throw new BusinessException(string.Format("Product characteristic ID = {0} which comments need to be decreased doesnt exist", charID));
                        }
                        businessProduct.DecreaseProductCharComments(objectContext, prodChar);
                    }
                    if (varId > 0)
                    {
                        ProductVariant currVariant = bpVariant.Get(objectContext, currProduct, varId, false, true);

                        businessProduct.DecreaseProductVariantComments(objectContext, currVariant);
                    }
                    if (subVarID > 0)
                    {
                        ProductSubVariant currSubVariant = bpVariant.GetSubVariant(objectContext, currProduct, subVarID, false, true);

                        businessProduct.DecreaseProductSubVariantComments(objectContext, currSubVariant);
                    }

                    businessProduct.DecreaseProductComments(objectContext, currProduct);
                    break;
                case CommentType.Topic:

                    BusinessProductTopics bTopic = new BusinessProductTopics();
                    ProductTopic topic = bTopic.Get(objectContext, comment.typeID, false, true);

                    if (!topic.LastCommentByReference.IsLoaded)
                    {
                        topic.LastCommentByReference.Load();
                    }
                    if (!comment.UserIDReference.IsLoaded)
                    {
                        comment.UserIDReference.Load();
                    }

                    if (topic.comments > 1 && topic.lastCommentDate == comment.dateCreated && topic.LastCommentBy.ID == comment.UserID.ID)
                    {
                        Comment lastComment = GetLastCommentForTopic(objectContext, topic);
                        if (lastComment != null)
                        {
                            if (!lastComment.UserIDReference.IsLoaded)
                            {
                                lastComment.UserIDReference.Load();
                            }

                            topic.lastCommentDate = lastComment.dateCreated;
                            topic.LastCommentBy = lastComment.UserID;

                            Tools.Save(objectContext);
                        }
                    }

                    bTopic.DecreaseTopicComments(objectContext, topic);

                    break;
                // other types are added as case`s
                default:
                    throw new BusinessException(string.Format("type {0} is not supported type", type));
            }
        }

        /// <summary>
        /// Function used in Sort By Descending .. sorts by id
        /// </summary>
        private long IdSelector(Comment comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }
            return comment.ID;
        }

        public CommentType GetCommentTypeFromString(string strType)
        {
            CommentType type = CommentType.Topic;

            switch (strType)
            {
                case ("product"):
                    type = CommentType.Product;
                    break;
                case ("topic"):
                    type = CommentType.Topic;
                    break;
                default:
                    throw new BusinessException(string.Format("Comment type = {0} is not supported type", strType));
            }

            return type;
        }

        private string GetCommentTypeFromEnum(CommentType type)
        {
            string result = "";

            switch (type)
            {
                case (CommentType.Product):
                    result = "product";
                    break;
                case (CommentType.Topic):
                    result = "topic";
                    break;
                default:
                    throw new BusinessException(string.Format("CommentType = {0} is not supported type", type));
            }

            return result;
        }

        private string GetCommentSubTypeFromEnum(CommentSubType type)
        {
            string result = "";

            switch (type)
            {
                case (CommentSubType.Comment):
                    result = "comment";
                    break;
                case (CommentSubType.SubComment):
                    result = "subcomment";
                    break;
                default:
                    throw new BusinessException(string.Format("CommentSubType = {0} is not supported type", type));
            }

            return result;
        }

        public CommentSubType GetCommentSubTypeFromString(Comment comment)
        {
            if (comment == null)
            {
                throw new BusinessException("comment is null");
            }

            CommentSubType result = CommentSubType.Comment;

            switch (comment.subType)
            {
                case "comment":
                    result = CommentSubType.Comment;
                    break;
                case "subcomment":
                    result = CommentSubType.SubComment;
                    break;
                default:
                    throw new BusinessException(string.Format("CommentSubType = {0} is not supported type", comment.subType));
            }

            return result;
        }

        private void UpdateParentCommentSubCommentsField(Entities objectContext, Comment currComment)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currComment == null)
            {
                throw new BusinessException("currComment = null");
            }

            if (currComment.subType == GetCommentSubTypeFromEnum(CommentSubType.SubComment))
            {
                Comment parent = GetWithoutVisible(objectContext, currComment.subTypeID);
                if (parent == null)
                {
                    throw new BusinessException(string.Format("There is no comment with id = {0}, for which comment ID = {1} is subcomment"
                        , currComment.typeID, currComment.ID));
                }

                if (currComment.visible == true && parent.haveSubcomments == false)
                {
                    parent.haveSubcomments = true;
                    Tools.Save(objectContext);
                }
                else if (currComment.visible == false)
                {
                    if (parent.haveSubcomments == true)
                    {
                        List<Comment> parentSubComments = GetAllSubComments(objectContext, parent).ToList();
                        if (parentSubComments != null)
                        {
                            if (parentSubComments.Count < 1)
                            {
                                parent.haveSubcomments = false;
                                Tools.Save(objectContext);
                            }
                        }
                        else
                        {
                            parent.haveSubcomments = false;
                            Tools.Save(objectContext);
                        }
                    }
                }
            }
        }

        public bool CheckIfMaxUserCommentsForProductsAreReached(Entities objectContext, Product currProduct, User currUser, string ipAdress)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }
            if (currProduct == null)
            {
                throw new BusinessException("currProduct is null");
            }

            bool result = false;

            int usrComments = 0;

            if (currUser != null)
            {
                BusinessUser businessUser = new BusinessUser();
                if (businessUser.IsFromAdminTeam(currUser))
                {
                    return false;
                }

                if (Configuration.ProductsCheckForIpAdressIfUserIsLogged == true)
                {
                    usrComments = objectContext.CommentSet.Count(comm => comm.type == "product"
                        && comm.typeID == currProduct.ID && (comm.ipAdress == ipAdress || comm.UserID.ID == currUser.ID));
                }
                else
                {
                    usrComments = objectContext.CommentSet.Count(comm => comm.type == "product"
                        && comm.typeID == currProduct.ID && comm.UserID.ID == currUser.ID);
                }

                if (usrComments >= Configuration.ProductsMaxNumberUserCommentsPerProduct)
                {
                    result = true;
                }
            }
            else
            {
                if (Configuration.ProductsCheckForIpAdressIfUserNotLogged == true)
                {
                    usrComments = objectContext.CommentSet.Count(comm => comm.type == "product"
                        && comm.typeID == currProduct.ID && comm.ipAdress == ipAdress);
                }

                if (usrComments >= Configuration.ProductsMaxNumberUserCommentsPerProduct)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool CheckIfMinimumTimeBetweenPostingCommentsPassed(Entities objectContext, User currUser, string ipAdress, out int minToWait)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            bool result = true;
            minToWait = 0;
            Comment lastComment = null;

            List<Comment> comments = new List<Comment>();

            if (Configuration.ProductsMinTimeBetweenComments > 0)
            {
                if (currUser != null)
                {
                    BusinessUser businessUser = new BusinessUser();
                    if (businessUser.IsFromAdminTeam(currUser))
                    {
                        return true;
                    }

                    if (CountUserComments(objectContext, currUser)
                        < Configuration.ProductsMinCommentsAfterWhichTimeIsInvalid)
                    {
                        if (Configuration.ProductsCheckForIpAdressIfUserIsLogged == true)
                        {
                            comments = objectContext.CommentSet.Where
                                (comm => comm.UserID.ID == currUser.ID).ToList();

                            if (comments.Count > 0)
                            {
                                lastComment = comments.Last();
                            }

                            if (lastComment == null)
                            {
                                comments = objectContext.CommentSet.Where
                                (comm => comm.ipAdress == ipAdress).ToList();

                                if (comments.Count > 0)
                                {
                                    lastComment = comments.Last();
                                }
                            }
                            else
                            {
                                comments = objectContext.CommentSet.Where
                                (comm => comm.ipAdress == ipAdress).ToList();

                                Comment ipComment = null;

                                if (comments.Count > 0)
                                {
                                    ipComment = comments.Last();
                                }

                                if (ipComment != null)
                                {
                                    if (DateTime.Compare(ipComment.dateCreated, lastComment.dateCreated) > 0)
                                    {
                                        lastComment = ipComment;
                                    }
                                }
                            }

                        }
                        else
                        {
                            comments = objectContext.CommentSet.Where(comm => comm.UserID.ID == currUser.ID).ToList();

                            if (comments.Count > 0)
                            {
                                lastComment = comments.Last();
                            }
                        }

                        if (lastComment != null)
                        {
                            DateTime commTime = lastComment.dateCreated;

                            TimeSpan span = DateTime.UtcNow - lastComment.dateCreated;
                            minToWait = (int)span.TotalMinutes;

                            if (minToWait < Configuration.ProductsMinTimeBetweenComments)
                            {
                                result = false;
                            }
                        }
                    }
                }
                else
                {
                    if (Configuration.ProductsCheckForIpAdressIfUserNotLogged == true)
                    {
                        comments = objectContext.CommentSet.Where(comm => comm.ipAdress == ipAdress).ToList();

                        if (comments.Count > 0)
                        {
                            lastComment = comments.Last();
                        }
                    }

                    if (lastComment != null)
                    {
                        DateTime commTime = lastComment.dateCreated;

                        TimeSpan span = DateTime.UtcNow - lastComment.dateCreated;
                        minToWait = (int)span.TotalMinutes;

                        if (minToWait < Configuration.ProductsMinTimeBetweenComments)
                        {
                            result = false;
                        }
                    }
                }
            }

            if (result == false)
            {
                minToWait = Configuration.ProductsMinTimeBetweenComments - minToWait;
            }

            return result;
        }

        /// <summary>
        /// returns true if there is visible:true comment for characteristic, otherwise false
        /// </summary>
        public bool AreThereVisibleCommentsForCharacteristic(Entities objectContext, ProductCharacteristics characteristic)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (characteristic == null)
            {
                throw new BusinessException("characteristic is null");
            }

            bool result = false;

            Comment comment = objectContext.CommentSet.FirstOrDefault
                (comm => comm.ForCharacteristic.ID == characteristic.ID && comm.visible == true);

            if (comment != null)
            {
                result = true;
            }

            return result;
        }

        public void AddTopicComment(EntitiesUsers userContext, Entities objectContext, User currUser, ProductTopic topic,
            string description, BusinessLog bLog, string ipAdress, CommentSubType subType, long subTypeId)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            if (subTypeId < 1)
            {
                throw new BusinessException("subTypeId < 1");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException("description is empty or null");
            }

            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty or null");
            }

            Comment newComment = new Comment();

            newComment.guestname = null;
            newComment.UserID = Tools.GetUserID(objectContext, currUser);

            newComment.type = GetCommentTypeFromEnum(CommentType.Topic);

            newComment.ForCharacteristic = null;
            newComment.ForSubVariant = null;
            newComment.ForVariant = null;

            newComment.typeID = topic.ID;

            newComment.dateCreated = DateTime.UtcNow;
            newComment.description = description;
            newComment.visible = true;
            newComment.lastModified = newComment.dateCreated;
            newComment.LastModifiedBy = newComment.UserID;
            newComment.agrees = 0;
            newComment.disagrees = 0;
            newComment.haveSubcomments = false;
            newComment.ipAdress = ipAdress;

            newComment.subType = GetCommentSubTypeFromEnum(subType);
            newComment.subTypeID = subTypeId;

            Add(userContext, objectContext, newComment, bLog, currUser);
        }

        /// <summary>
        /// returns visible.true from-to comments for topic
        /// </summary>
        public List<Comment> GetTopicComments(Entities objectContext, long? topic, int fromIndex, int toIndex)
        {
            Tools.AssertObjectContextExists(objectContext);

            if ((topic == null) || (topic.Value < 1))
            {
                throw new BusinessException("Error : invalid product !");
            }

            List<Comment> comments = objectContext.GetCommentsRange("topic", topic, fromIndex, toIndex).ToList();

            return comments;
        }

        public long CountTopicMainComments(Entities objectContext, ProductTopic topic)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (topic == null)
            {
                throw new BusinessException("topic is null");
            }

            List<Comment> topicComments = GetTopicComments(objectContext, topic.ID, 0, int.MaxValue);

            long count = topicComments.Count;

            return count;
        }

    }
}
