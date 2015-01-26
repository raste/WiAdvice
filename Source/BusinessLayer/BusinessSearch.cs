// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessSearch
    {
        private class SearchResults
        {
            public long ID { get; set; }
            public int HitsCount { get; set; }
        }

        private int SearchResultsHitsCountSelector(SearchResults searchRes)
        {
            int key = -1;

            if (searchRes != null)
            {
                key = searchRes.HitsCount;
            }
            return key;
        }

        /// <summary>
        /// Returns Search results in products
        /// </summary>
        public List<Product> SearchInProducts(Entities objectContext, String search, long id)
        {
            Tools.AssertObjectContextExists(objectContext);

            BusinessProduct businessProduct = new BusinessProduct();

            List<string> wordsList = SearchWords(ref search);
            IList<SearchResults> results = new List<SearchResults>();

            string error = ""; // not used

            if (id < 1)
            {
                foreach (Product product in objectContext.ProductSet.Where(prod => prod.visible == true))
                {
                    int hitsCount = 0;

                    foreach (string word in wordsList)
                    {
                        if (product.name.IndexOf(word, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            hitsCount++;
                        }
                    }

                    if (hitsCount > 0 && businessProduct.CheckIfProductsIsValidWithConnections(objectContext, product, out error))
                    {

                        SearchResults searchRes = new SearchResults();
                        searchRes.ID = product.ID;
                        searchRes.HitsCount = hitsCount;
                        results.Add(searchRes);
                    }
                }
            }
            else
            {
                foreach (Product product in objectContext.ProductSet.Where(prod => prod.visible == true && prod.Category.ID == id))
                {
                    int hitsCount = 0;

                    foreach (string word in wordsList)
                    {
                        if (product.name.IndexOf(word, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            hitsCount++;
                        }
                    }

                    if (hitsCount > 0 && businessProduct.CheckIfProductsIsValidWithConnections(objectContext, product, out error))
                    {
                        SearchResults searchRes = new SearchResults();
                        searchRes.ID = product.ID;
                        searchRes.HitsCount = hitsCount;
                        results.Add(searchRes);
                    }
                }
            }

            IOrderedEnumerable<SearchResults> orderedSearchResults =
                results.OrderByDescending<SearchResults, int>(SearchResultsHitsCountSelector);


            Product currProduct = null;
            List<Product> products = new List<Product>();

            foreach (SearchResults srchRes in orderedSearchResults)
            {
                currProduct = businessProduct.GetProductByIDWV(objectContext, srchRes.ID);
                if (currProduct == null)
                {
                    throw new BusinessException("currProduct == null");
                }
                products.Add(currProduct);
            }

            return products;
        }

        public List<Product> SearchInProducts(Entities objectContext, String search, long id, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.CheckFromToParameters(from, to);

            IEnumerable<Product> products = null;

            if (id < 1)
            {
                products = objectContext.FindProducts(search, from, to);
            }
            else
            {
                products = objectContext.FindProductsInCategory(search, from, to, id);
            }

            return products.ToList<Product>();
        }

        public List<ProductVariant> SearchInProductsVariants(Entities objectContext, String search, long id, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.CheckFromToParameters(from, to);

            IEnumerable<ProductVariant> products = null;

            if (id < 1)
            {
                products = objectContext.FindProductsVariants(search, from, to);
            }
            else
            {
                products = objectContext.FindProductsVariantsInCategory(search, from, to, id);
            }

            return products.ToList<ProductVariant>();
        }

        public List<ProductSubVariant> SearchInProductsSubVariants(Entities objectContext, String search, long id, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.CheckFromToParameters(from, to);

            IEnumerable<ProductSubVariant> products = null;

            if (id < 1)
            {
                products = objectContext.FindProductsSubVariants(search, from, to);
            }
            else
            {
                products = objectContext.FindProductsSubVariantsInCategory(search, from, to, id);
            }

            return products.ToList<ProductSubVariant>();
        }

        public List<AlternativeProductName> SearchInAlternativeProducts(Entities objectContext, String search, long id, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.CheckFromToParameters(from, to);

            List<AlternativeProductName> products = new List<AlternativeProductName>();
            if (id < 1)
            {
                products = objectContext.FindProductsWithAlternativeNames(search, from, to).ToList();
            }
            else
            {
                products = objectContext.FindProductsInCategoryWithAlternativeNames(search, from, to, id).ToList();
            }

            return products;
        }

        public List<AlternativeCompanyName> SearchInAlternativeCompanies(Entities objectContext, String search, long id, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.CheckFromToParameters(from, to);

            List<AlternativeCompanyName> companies = new List<AlternativeCompanyName>();
            if (id < 1)
            {
                companies = objectContext.FindCompaniesWithAlternativeNames(search, from, to).ToList();
            }
            else
            {
                throw new BusinessException(string.Format("Search for alternative company names WITH TYPE is not supported"));
            }

            return companies;
        }

        public long SearchCountInProducts(  
            String search, long id, long from, long to, out long alternativesCount)
        {
            Tools.CheckFromToParameters(from, to);

            IEnumerable<ScalarValue> scalarValues;
            IList<ScalarValue> svList;
            long productsCount = 0;
            alternativesCount = 0;

            // Because ScalarValue is a fake table which we only use to work round an Entity Data Model limitation,
            // we need to be know that no ScalarValue has been read yet in the ObjectContext.
            // To be sure we get the count properly, a new temporary ObjectContext is created.
            Entities temporaryObjectContext = Tools.GetTemporaryEntities();

            if (id < 1)
            {
                scalarValues = temporaryObjectContext.FindCountProducts(search, from, to);
            }
            else
            {
                scalarValues = temporaryObjectContext.FindCountProductsInCategory(search, from, to, id);
            }
            svList = scalarValues.ToList<ScalarValue>();

            if (svList.Count > 0)
            {
                ScalarValue cnt = svList[0];
                if (cnt != null)
                {
                    BusinessScalarValue bScalarVal = new BusinessScalarValue(cnt);

                    productsCount = bScalarVal.LongValue;
                }
            }

            // GET Alternative names COUNT
            // Call stored procedure which gets alternatives count
            temporaryObjectContext = Tools.GetTemporaryEntities();
            if (id < 1)
            {
                scalarValues = temporaryObjectContext.FindCountProductsWithAlternativeNames(search, from, to);
            }
            else
            {
                scalarValues = temporaryObjectContext.FindCountProductsInCategoryWithAlternativeNames(search, from, to, id);
            }
            svList = scalarValues.ToList<ScalarValue>();

            if (svList.Count > 0)
            {
                ScalarValue cnt = svList[0];
                if (cnt != null)
                {
                    BusinessScalarValue bScalarVal = new BusinessScalarValue(cnt);

                    alternativesCount = bScalarVal.LongValue;
                }
            }

            // gets variants count
            long variantsCount = 0;
            temporaryObjectContext = Tools.GetTemporaryEntities();
            if (id < 1)
            {
                scalarValues = temporaryObjectContext.FindCountProductsVariants(search, from, to);
            }
            else
            {
                scalarValues = temporaryObjectContext.FindCountProductsVariantsInCategory(search, from, id, to);
            }
            svList = scalarValues.ToList<ScalarValue>();

            if (svList.Count > 0)
            {
                ScalarValue cnt = svList[0];
                if (cnt != null)
                {
                    BusinessScalarValue bScalarVal = new BusinessScalarValue(cnt);

                    variantsCount = bScalarVal.LongValue;
                }
            }

            // get subvariant counts
            long subVariantsCount = 0;
            temporaryObjectContext = Tools.GetTemporaryEntities();
            if (id < 1)
            {
                scalarValues = temporaryObjectContext.FindCountProductsSubvariants(search, from, to);
            }
            else
            {
                scalarValues = temporaryObjectContext.FindCountProductsSubvariantsInCategory(search, from, id, to);
            }
            svList = scalarValues.ToList<ScalarValue>();

            if (svList.Count > 0)
            {
                ScalarValue cnt = svList[0];
                if (cnt != null)
                {
                    BusinessScalarValue bScalarVal = new BusinessScalarValue(cnt);

                    subVariantsCount = bScalarVal.LongValue;
                }
            }


            if (variantsCount < subVariantsCount)
            {
                if (alternativesCount < subVariantsCount)
                {
                    alternativesCount = subVariantsCount;
                }
            }
            else
            {
                if (alternativesCount < variantsCount)
                {
                    alternativesCount = variantsCount;
                }
            }

            return productsCount;
        }

        /// <summary>
        /// Returns search results in COmpanies
        /// </summary>
        public List<Company> SearchInCompanies(Entities objectContext, String search)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<string> wordsList = SearchWords(ref search);

            IList<SearchResults> results = new List<SearchResults>();

            foreach (Company company in objectContext.CompanySet.Where(comp => comp.visible == true))
            {
                int hitsCount = 0;

                foreach (string word in wordsList)
                {
                    if (company.name.IndexOf(word, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        hitsCount++;
                    }
                }

                if (hitsCount > 0)
                {
                    SearchResults searchRes = new SearchResults();
                    searchRes.ID = company.ID;
                    searchRes.HitsCount = hitsCount;
                    results.Add(searchRes);
                }
            }

            IOrderedEnumerable<SearchResults> orderedSearchResults =
                results.OrderByDescending<SearchResults, int>(SearchResultsHitsCountSelector);

            BusinessCompany businessCompany = new BusinessCompany();
            Company currCompany = null;
            List<Company> companies = new List<Company>();

            foreach (SearchResults srchRes in orderedSearchResults)
            {
                currCompany = businessCompany.GetCompanyWV(objectContext, srchRes.ID);
                if (currCompany == null)
                {
                    throw new BusinessException("currCompany == null");
                }
                companies.Add(currCompany);
            }

            return companies;
        }

        public void SearchCountInCompanies(String search, long from, long to, long typeID, out long companiesCount, out long alternativesCount)
        {
            Tools.CheckFromToParameters(from, to);

            IEnumerable<ScalarValue> scalarValues;
            IList<ScalarValue> svList;
            companiesCount = 0;
            alternativesCount = 0;

            // Because ScalarValue is a fake table which we only use to work round an Entity Data Model limitation,
            // we need to be know that no ScalarValue has been read yet in the ObjectContext.
            // To be sure we get the count properly, a new temporary ObjectContext is created.
            Entities temporaryObjectContext = Tools.GetTemporaryEntities();

            if (typeID < 1)
            {
                scalarValues = temporaryObjectContext.FindCountCompanies(search, from, to);
            }
            else
            {
                scalarValues = temporaryObjectContext.FindCountCompaniesWithType(search, from, to, typeID);
            }

            svList = scalarValues.ToList<ScalarValue>();
            if (svList.Count > 0)
            {
                ScalarValue cnt = svList[0];
                if (cnt != null)
                {
                    BusinessScalarValue bScalarVal = new BusinessScalarValue(cnt);

                    companiesCount = bScalarVal.LongValue;
                }
            }

            // GET alternative names
            temporaryObjectContext = Tools.GetTemporaryEntities();
            if (typeID < 1)
            {
                scalarValues = temporaryObjectContext.FindCountCompaniesWithAlternativeNames(search, from, to);
            }
            else
            {
                throw new BusinessException(string.Format("Search with company types is not supported"));
            }
            svList = scalarValues.ToList<ScalarValue>();

            if (svList.Count > 0)
            {
                ScalarValue cnt = svList[0];
                if (cnt != null)
                {
                    BusinessScalarValue bScalarVal = new BusinessScalarValue(cnt);

                    alternativesCount = bScalarVal.LongValue;
                }
            }
        }

        public List<Company> SearchInCompanies(Entities objectContext, String search, long from, long to, long typeID)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.CheckFromToParameters(from, to);

            IEnumerable<Company> companies = null;
            if (typeID < 1)
            {
                companies = objectContext.FindCompanies(search, from, to);
            }
            else
            {
                companies = objectContext.FindCompaniesWithType(search, from, to, typeID);
            }

            return companies.ToList<Company>();
        }

        /// <summary>
        /// Returns search resuts in categories
        /// </summary>
        public List<Category> SearchInCategories(Entities objectContext, String search)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<string> wordsList = SearchWords(ref search);

            IList<SearchResults> results = new List<SearchResults>();

            foreach (Category category in objectContext.CategorySet.Where(cat => cat.visible == true))
            {
                int hitsCount = 0;

                foreach (string word in wordsList)
                {
                    if (category.name.IndexOf(word, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        hitsCount++;
                    }
                }

                if (hitsCount > 0)
                {
                    SearchResults searchRes = new SearchResults();
                    searchRes.ID = category.ID;
                    searchRes.HitsCount = hitsCount;
                    results.Add(searchRes);
                }
            }

            IOrderedEnumerable<SearchResults> orderedSearchResults =
                results.OrderByDescending<SearchResults, int>(SearchResultsHitsCountSelector);

            BusinessCategory businessCategory = new BusinessCategory();
            Category currCategory = null;
            List<Category> categories = new List<Category>();

            foreach (SearchResults srchRes in orderedSearchResults)
            {
                currCategory = businessCategory.GetWithoutVisible(objectContext, srchRes.ID);
                if (currCategory == null)
                {
                    throw new BusinessException("currCategory == null");
                }
                categories.Add(currCategory);
            }

            return categories;
        }

        public long SearchCountInCategories(  
            String search, long from, long to)
        {
            Tools.CheckFromToParameters(from, to);

            IEnumerable<ScalarValue> scalarValues;
            IList<ScalarValue> svList;
            long categoriesCount = 0;

            // Because ScalarValue is a fake table which we only use to work round an Entity Data Model limitation,
            // we need to be know that no ScalarValue has been read yet in the ObjectContext.
            // To be sure we get the count properly, a new temporary ObjectContext is created.
            Entities temporaryObjectContext = Tools.GetTemporaryEntities();

            scalarValues = temporaryObjectContext.FindCountCategories(search, from, to);

            svList = scalarValues.ToList<ScalarValue>();
            if (svList.Count > 0)
            {
                ScalarValue cnt = svList[0];
                if (cnt != null)
                {
                    BusinessScalarValue bScalarVal = new BusinessScalarValue(cnt);

                    categoriesCount = bScalarVal.LongValue;
                }
            }
            return categoriesCount;
        }

        /// <summary>
        /// Returns search resuts in categories within range
        /// </summary>

        public List<Category> SearchInCategories(Entities objectContext, String search, long from, long to)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.CheckFromToParameters(from, to);

            IEnumerable<Category> categories = objectContext.FindCategories(search, from, to);

            return categories.ToList<Category>();
        }

        /// <summary>
        /// Returns search results in SiteNews
        /// </summary>
        public List<SiteNews> SearchInSiteTexts(Entities objectContext, String search)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<string> wordsList = SearchWords(ref search);

            IList<SearchResults> results = new List<SearchResults>();

            foreach (SiteNews siteText in objectContext.SiteNewsSet)
            {
                int hitsCount = 0;

                foreach (string word in wordsList)
                {
                    if (siteText.name.IndexOf(word, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        hitsCount++;
                    }
                }

                if (hitsCount > 0)
                {
                    SearchResults searchRes = new SearchResults();
                    searchRes.ID = siteText.ID;
                    searchRes.HitsCount = hitsCount;
                    results.Add(searchRes);
                }
            }

            IOrderedEnumerable<SearchResults> orderedSearchResults =
                results.OrderByDescending<SearchResults, int>(SearchResultsHitsCountSelector);

            BusinessSiteText businessSiteText = new BusinessSiteText();
            SiteNews currSiteText = null;
            List<SiteNews> siteTexts = new List<SiteNews>();

            foreach (SearchResults srchRes in orderedSearchResults)
            {
                currSiteText = businessSiteText.Get(objectContext, srchRes.ID);
                if (currSiteText == null)
                {
                    throw new BusinessException("currSiteText == null");
                }
                siteTexts.Add(currSiteText);
            }
            return siteTexts;
        }

        public long SearchCountInUsers( 
            String search, long from, long to)
        {
            Tools.CheckFromToParameters(from, to);

            IEnumerable<ScalarValue> scalarValues;
            IList<ScalarValue> svList;
            long usersCount = 0;

            // Because ScalarValue is a fake table which we only use to work round an Entity Data Model limitation,
            // we need to be know that no ScalarValue has been read yet in the ObjectContext.
            // To be sure we get the count properly, a new temporary ObjectContext is created.
            Entities temporaryObjectContext = Tools.GetTemporaryEntities();

            scalarValues = temporaryObjectContext.FindCountUsers(search, from, to);

            svList = scalarValues.ToList<ScalarValue>();
            if (svList.Count > 0)
            {
                ScalarValue cnt = svList[0];
                if (cnt != null)
                {
                    BusinessScalarValue bScalarVal = new BusinessScalarValue(cnt);

                    usersCount = bScalarVal.LongValue;
                }
            }
            return usersCount;
        }

        public List<User> SearchInUsers(EntitiesUsers userContext, String search, long from, long to)
        {
            Tools.AssertObjectContextExists(userContext);
            Tools.CheckFromToParameters(from, to);

            List<User> users = userContext.FindUsers(search, from, to).ToList();

            return users;
        }

        private static List<string> SearchWords(ref String search)
        {
            if (search == null)
            {
                search = "";
            }

            string[] separator = new string[] { " " };
            string[] words = search.ToLower().Split(separator, StringSplitOptions.RemoveEmptyEntries);

            List<string> wordsList = new List<string>();

            foreach (string word in words)
            {
                if (!wordsList.Contains(word))
                {
                    wordsList.Add(word);
                }

            }
            return wordsList;
        }

        /// <summary>
        /// Returns number of found results in searchInType
        /// </summary>
        /// <param name="searchInType">companies,products,users,categories</param>

        public long CountSearchResults(EntitiesUsers userContext, Entities objectContext, string search, string searchInType)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            if (string.IsNullOrEmpty(search))
            {
                throw new BusinessException("search is empty or null");
            }
            if (string.IsNullOrEmpty(searchInType))
            {
                throw new BusinessException("searchInType is mepty or null");
            }

            List<string> wordsList = SearchWords(ref search);
            long results = 0;

            switch (searchInType)
            {
                case ("companies"):
                    foreach (Company company in objectContext.CompanySet.Where(comp => comp.visible == true))
                    {
                        foreach (string word in wordsList)
                        {
                            if (company.name.IndexOf(word, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                results++;
                                break;
                            }
                        }
                    }
                    break;
                case ("products"):
                    foreach (Product product in objectContext.ProductSet.Where(prod => prod.visible == true))
                    {
                        foreach (string word in wordsList)
                        {
                            if (product.name.IndexOf(word, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                results++;
                                break;
                            }
                        }
                    }
                    break;
                case ("users"):
                    foreach (User user in userContext.UserSet.Where(usr => usr.visible == true))
                    {
                        foreach (string word in wordsList)
                        {
                            if (user.username.IndexOf(word, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                results++;
                                break;
                            }
                        }
                    }
                    break;
                case ("categories"):
                    foreach (Category category in objectContext.CategorySet.Where(cat => cat.visible == true))
                    {
                        foreach (string word in wordsList)
                        {
                            if (category.name.IndexOf(word, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                results++;
                                break;
                            }
                        }
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("searchInType = {0} , is not supported type", searchInType));
            }

            return results;
        }



    }

}
