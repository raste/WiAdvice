// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class AutoComplete
    {
        private static object UpdatingAutoCompleteString = new object();

        /// <summary>
        /// Function for Adding new Table Row into AutoCompleteSearch table
        /// </summary>
        private void Add(Entities objectContext, AutoCompleteSearch newSearch)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (newSearch == null)
            {
                throw new BusinessException("newSearch which is wanted to be added is null");
            }

            objectContext.AddToAutoCompleteSearchSet(newSearch);
            Tools.Save(objectContext);
        }

        /// <summary>
        /// Function for getting a row from AutoCompleteSearch table which is for a search string
        /// </summary>
        /// <param name="search">search string for which we want to get row</param>
        /// <returns>AutoCompleteSearch obect id found, null if not</returns>
        private AutoCompleteSearch Get(Entities objectContext, string search)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(search))
            {
                throw new BusinessException("The search string is null or empty");
            }

            AutoCompleteSearch acs = objectContext.AutoCompleteSearchSet.FirstOrDefault(src => src.@string == search);

            return acs;
        }

        /// <summary>
        /// Add new row for a string that is searched for first time , or updates the fields of already saved in AutoCompleteSearch table string
        /// </summary>
        /// <param name="search">String which was searched</param>
        /// <param name="companies">found companies</param>
        /// <param name="products">found products</param>
        /// <param name="categories">found categories</param>
        /// <param name="users">found users</param>
        public void StringSearched(Entities objectContext, string search, long companies, long products, long categories, long users)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(search))
            {
                throw new BusinessException("search string is null or empry");
            }
            if (companies < 0)
            {
                throw new BusinessException("companies found are less than 0");
            }
            if (products < 0)
            {
                throw new BusinessException("products found are less than 0");
            }
            if (categories < 0)
            {
                throw new BusinessException("categories found are less than 0");
            }
            if (users < 0)
            {
                throw new BusinessException("users found are less than 0");
            }

            AutoCompleteSearch stringSearched;

            lock (UpdatingAutoCompleteString)
            {
                if (CheckIfStringIsSearched(objectContext, search, out stringSearched))
                {
                    IncrementStringSearched(objectContext, stringSearched);
                    UpdateResults(objectContext, stringSearched, companies, products, categories, users);
                }
                else if ((categories + companies + products + users) > 0)
                {
                    stringSearched = new AutoCompleteSearch();
                    stringSearched.searches = 1;
                    stringSearched.@string = search;
                    stringSearched.foundCategories = categories;
                    stringSearched.foundCompanies = companies;
                    stringSearched.foundProducts = products;
                    stringSearched.foundUsers = users;
                    stringSearched.foundResults = categories + companies + products + users;
                    stringSearched.dateFirstSearched = DateTime.UtcNow;

                    Add(objectContext, stringSearched);
                }
            }
        }

        /// <summary>
        /// Updates the found results for a row in AutoCompleteSearch table
        /// </summary>
        /// <param name="stringSearched">AutoCompleteSearch obect for which is updating</param>
        /// <param name="companies">companies count</param>
        /// <param name="products">products count</param>
        /// <param name="categories">categories count</param>
        /// <param name="users">users count</param>
        private void UpdateResults(Entities objectContext, AutoCompleteSearch stringSearched, long companies,
            long products, long categories, long users)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (stringSearched == null)
            {
                throw new BusinessException("stringSearched is null");
            }

            if (companies < 0)
            {
                throw new BusinessException("companies count is less than 0");
            }
            if (products < 0)
            {
                throw new BusinessException("products count is less than 0");
            }
            if (categories < 0)
            {
                throw new BusinessException("categories count is less than 0");
            }
            if (users < 0)
            {
                throw new BusinessException("users count is less than 0");
            }

            bool resultsChange = false;
            if (stringSearched.foundCompanies != companies)
            {
                stringSearched.foundCompanies = companies;
                resultsChange = true;
            }
            if (stringSearched.foundCategories != categories)
            {
                stringSearched.foundCategories = categories;
                resultsChange = true;
            }
            if (stringSearched.foundProducts != products)
            {
                stringSearched.foundProducts = products;
                resultsChange = true;
            }
            if (stringSearched.foundUsers != users)
            {
                stringSearched.foundUsers = users;
                resultsChange = true;
            }

            if (resultsChange)
            {
                stringSearched.foundResults = companies + categories + products + users;
                Tools.Save(objectContext);
            }
        }

        /// <summary>
        /// Increases with 1 Searches field on AutoCompleteSearch obect 
        /// </summary>
        /// <param name="stringSearched">AutoCompleteSearch obect</param>
        private void IncrementStringSearched(Entities objectContext, AutoCompleteSearch stringSearched)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (stringSearched == null)
            {
                throw new BusinessException("stringSearched is null");
            }

            stringSearched.searches++;
            Tools.Save(objectContext);
        }

        /// <summary>
        /// Checks if a theres a row in AutoCompleteSearch for a string
        /// </summary>
        /// <returns>true if theres row , false if not</returns>
        private bool CheckIfStringIsSearched(Entities objectContext, string search, out AutoCompleteSearch acs)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(search))
            {
                throw new BusinessException("search is null or empry");
            }

            acs = Get(objectContext, search);
            if (acs == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Gets Number of rows from AutoCompleteSearch table which starts whit specified string
        /// </summary>
        /// <param name="search">string for which is looking for results</param>
        /// <param name="numResults">number of returned results which is wanted</param>
        /// <returns>number of results if found , otherwise less or none</returns>
        public string[] GetResults(Entities objectContext, string search, int numResults)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (numResults < 0)
            {
                throw new BusinessException("numResults wanted is < 0");
            }
            if (string.IsNullOrEmpty(search))
            {
                throw new BusinessException("search is null or empty");
            }

            string[] results;
            int count;
            List<AutoCompleteSearch> allResults = GetAllResultsForString(objectContext, search);
            count = allResults.Count;
            if (count > numResults)
            {
                results = new string[numResults];

                for (int i = 0; i < numResults; i++)
                {
                    results[i] = Tools.TrimString(allResults[i].@string, 30, false, false);
                }
            }
            else if (count > 0)
            {
                results = new string[count];

                for (int i = 0; i < count; i++)
                {
                    results[i] = Tools.TrimString(allResults[i].@string, 30, false, false);
                }
            }
            else
            {
                results = null;
            }

            return results;
        }

        public string[] GetCompanyNamesResults(Entities objectContext, string search, int numResults)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (numResults < 0)
            {
                throw new BusinessException("numResults wanted is < 0");
            }
            if (string.IsNullOrEmpty(search))
            {
                throw new BusinessException("search is null or empty");
            }

            string[] results;
            int count;
            List<Company> companies = objectContext.GetAutoCompleteCompanies(search, (long)numResults).ToList();
            count = companies.Count;
            if (count > 0)
            {
                results = new string[count];

                for (int i = 0; i < count; i++)
                {
                    results[i] = companies[i].name;
                }
            }
            else
            {
                results = null;
            }

            return results;
        }

        public void GetCompanyResults(Entities objectContext, string search, int numResults,
            out List<Company> companies, out List<AlternativeCompanyName> altNames)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (numResults < 0)
            {
                throw new BusinessException("numResults wanted is < 0");
            }
            if (string.IsNullOrEmpty(search))
            {
                throw new BusinessException("search is null or empty");
            }

            companies = objectContext.GetAutoCompleteCompanies(search, (long)numResults).ToList();
            altNames = new List<AlternativeCompanyName>();

            int remResults = numResults - companies.Count;

            altNames = objectContext.GetAutoCompleteCompaniesWithAlternativeNames(search, remResults).ToList();
        }

        public void GetProductResults(Entities objectContext, string search, int numResults, out List<Product> products, out List<AlternativeProductName> altNames
            , out List<ProductVariant> variants, out List<ProductSubVariant> subvariants)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (numResults < 0)
            {
                throw new BusinessException("numResults wanted is < 0");
            }
            if (string.IsNullOrEmpty(search))
            {
                throw new BusinessException("search is null or empty");
            }

            products = new List<Product>();
            altNames = new List<AlternativeProductName>();
            variants = new List<ProductVariant>();
            subvariants = new List<ProductSubVariant>();

            products = objectContext.GetAutoCompleteProducts(search, (long)numResults).ToList();

            int remResults = numResults - products.Count;

            if (remResults > 0)
            {
                altNames = objectContext.GetAutoCompleteProductsWithAlternativeNames(search, (long)numResults).ToList();

                remResults = remResults - altNames.Count;

                if (remResults > 0)
                {
                    variants = objectContext.GetAutoCompleteProductsVariants(search, (long)numResults).ToList();

                    remResults = remResults - variants.Count;

                    if (remResults > 0)
                    {
                        subvariants = objectContext.GetAutoCompleteProductsSubVariants(search, (long)numResults).ToList();
                    }
                }
            }
        }

        /// <summary>
        /// Gets all results for a string from AutoCompleteSearch table
        /// </summary>
        private List<AutoCompleteSearch> GetAllResultsForString(Entities objectContext, string search)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (string.IsNullOrEmpty(search))
            {
                throw new BusinessException("search is null or empty");
            }

            IEnumerable<AutoCompleteSearch> iResults = objectContext.GetAutocompleteResults(search);
            return iResults.ToList();
        }

        private long FoundResultsSelector(AutoCompleteSearch acs)
        {
            if (acs == null)
            {
                throw new ArgumentNullException("acs");
            }
            return acs.foundResults;
        }

    }
}
