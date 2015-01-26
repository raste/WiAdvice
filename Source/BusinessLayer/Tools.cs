﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security.Cryptography;

using DataAccess;
using System.IO;

namespace BusinessLayer
{
    /// <summary>
    /// Valdiators to be written here
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// Returns System user
        /// </summary>
        public static User GetSystem()
        {
            EntitiesUsers userContext = new EntitiesUsers();
            AssertObjectContextExists(userContext);

            BusinessUser bUser = new BusinessUser();
            User system = bUser.GetSystem(userContext);

            return system;
        }

        public static string GetCorrectedUrl(string url)
        {
            if (!url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
                && !url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                url = string.Format("http://{0}", url);
            }

            return url;
        }

        /// <summary>
        /// Checks if objectContext exists otherwise throws exception
        /// </summary>
        public static void AssertObjectContextExists(Entities objectContext)
        {
            if (objectContext == null)
            {
                throw new ArgumentNullException("objectContext");
            }
        }

        public static void AssertObjectContextExists(EntitiesUsers objectContext)
        {
            if (objectContext == null)
            {
                throw new ArgumentNullException("objectContext");
            }
        }

        /// <summary>
        /// Checks if BusinessLog exists otherwise throws exception
        /// </summary>
        public static void AssertBusinessLogExists(BusinessLog businessLog)
        {
            if (businessLog == null)
            {
                throw new ArgumentNullException("businessLog");
            }
        }

        public static User GetUserFromUserDatabase(long id)
        {
            if (id < 1)
            {
                throw new BusinessException("id < 1");
            }

            EntitiesUsers userContext = new EntitiesUsers();
            BusinessUser businessUser = new BusinessUser();
            User currUser = businessUser.GetWithoutVisible(userContext, id, true);

            return currUser;
        }

        public static User GetUserFromUserDatabase(EntitiesUsers userContext, long id)
        {
            if (id < 1)
            {
                throw new BusinessException("id < 1");
            }
            Tools.AssertObjectContextExists(userContext);

            BusinessUser businessUser = new BusinessUser();
            User currUser = businessUser.GetWithoutVisible(userContext, id, true);

            return currUser;
        }

        public static User GetUserFromUserDatabase(UserID user)
        {
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            EntitiesUsers userContext = new EntitiesUsers();
            BusinessUser businessUser = new BusinessUser();
            User currUser = businessUser.GetWithoutVisible(userContext, user.ID, true);

            return currUser;
        }

        public static User GetUserFromUserDatabase(EntitiesUsers userContext, UserID user)
        {
            if (user == null)
            {
                throw new BusinessException("user is null");
            }

            AssertObjectContextExists(userContext);
            BusinessUser businessUser = new BusinessUser();
            User currUser = businessUser.GetWithoutVisible(userContext, user.ID, true);

            return currUser;
        }

        public static UserID GetUserID(Entities objectContext, User user)
        {
            AssertObjectContextExists(objectContext);

            if (user == null)
            {
                throw new BusinessException("user is null");
            }


            UserID currUser = objectContext.UserIDSet.FirstOrDefault(us => us.ID == user.ID);
            if (currUser == null)
            {
                throw new BusinessException(string.Format("There is no UserID = {0}", user.ID));
            }

            return currUser;
        }

        public static UserID GetUserID(Entities objectContext, long id, bool throwExcpetionIfNull)
        {
            AssertObjectContextExists(objectContext);

            UserID currUser = objectContext.UserIDSet.FirstOrDefault(us => us.ID == id);

            if (throwExcpetionIfNull && currUser == null)
            {
                throw new BusinessException(string.Format("There is no UserID = {0}", id));
            }

            return currUser;
        }

        /// <summary>
        /// Saves changes to the objectContext
        /// </summary>
        public static void Save(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            objectContext.SaveChanges();
        }

        public static void Save(EntitiesUsers objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            objectContext.SaveChanges();
        }

        /// <summary>
        /// CHecks if there is obect with 'name' in 'table', true if there is NOT, false if there is
        /// </summary>
        /// <param name="table">users,categories,companies,products,productLink,compChar,prodChar,siteText,companyType</param>
        /// <param name="name">name for checking</param>
        /// <param name="typeID">if prodChar(product id), compChar(company id), category(parent category id), productLink(product id) ..else 0</param>
        /// <returns>true if passed , false if not</returns>
        public static Boolean NameValidatorPassed(Entities objectContext, String table, String name, long typeID)
        {
            // table = users,categories,companies,products,compChar,prodChar
            if (name == null || name == "")
            {
                throw new BusinessException("name is null or empty");
            }

            if (table == null || table == "")
            {
                throw new BusinessException("table is null or empty");
            }

            AssertObjectContextExists(objectContext);
            Boolean result = false;

            switch (table)
            {
                case ("users"):
                    EntitiesUsers usersContext = new EntitiesUsers();
                    AssertObjectContextExists(usersContext);

                    User user = usersContext.UserSet.FirstOrDefault<User>(usr => usr.username == name);

                    if (user == null)
                    {
                        result = true;
                    }
                    break;
                case ("categories"):
                    result = true;

                    if (typeID != 0)
                    {
                        BusinessCategory businessCategory = new BusinessCategory();

                        Category parentCategory = businessCategory.GetWithoutVisible(objectContext, typeID);
                        if (parentCategory == null)
                        {
                            throw new BusinessException(string.Format("There is no category with id = {0}", typeID));
                        }

                        List<Category> subCategories = businessCategory.GetAllSubCategories(objectContext, parentCategory, true, true).ToList();

                        if (subCategories.Count > 0)
                        {
                            foreach (Category category in subCategories)
                            {
                                if (category.name == name)
                                {
                                    result = false;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case ("companies"):
                    Company company = objectContext.CompanySet.FirstOrDefault<Company>(comp => comp.name == name);

                    if (company == null)
                    {
                        result = true;
                    }
                    break;
                case ("products"):
                    Product product = objectContext.ProductSet.FirstOrDefault<Product>(prod => prod.name == name);

                    if (product == null)
                    {
                        result = true;
                    }
                    break;
                case ("productLink"):
                    List<ProductLink> links = objectContext.ProductLinkSet.Where(pl => pl.Product.ID == typeID && pl.visible == true).ToList();

                    ProductLink flink = null;

                    if (links != null && links.Count > 0)
                    {
                        foreach (ProductLink link in links)
                        {
                            if (link.link == name)
                            {
                                flink = link;
                                break;
                            }
                        }
                    }

                    if (flink == null)
                    {
                        result = true;
                    }

                    break;
                case ("compChar"):
                    if (typeID > 0)
                    {
                        CompanyCharacterestics compChar = objectContext.CompanyCharacteresticsSet.FirstOrDefault
                       <CompanyCharacterestics>(cc => cc.name == name && cc.Company.ID == typeID && cc.visible == true);

                        if (compChar == null)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        throw new BusinessException("typeId is < 1");
                    }
                    break;
                case ("companyType"):
                    CompanyType type = objectContext.CompanyTypeSet.FirstOrDefault(tp => tp.name == name);

                    if (type == null)
                    {
                        result = true;
                    }
                    break;
                case ("prodChar"):
                    if (typeID > 0)
                    {
                        ProductCharacteristics prodChar = objectContext.ProductCharacteristicsSet.FirstOrDefault
                       <ProductCharacteristics>(pc => pc.name == name && pc.Product.ID == typeID && pc.visible == true);
                        if (prodChar == null)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        throw new BusinessException("typeId is < 1");
                    }
                    break;
                case ("siteText"):
                    SiteNews siteText = objectContext.SiteNewsSet.FirstOrDefault(text => text.name == name);
                    if (siteText == null)
                    {
                        result = true;
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("table = {0} is not suported table", table));
            }
            return result;
        }

        /// <summary>
        /// Checks if string is null or empty
        /// </summary>
        /// <returns>true if its not null or empty , otherwise false</returns>
        public static Boolean NullValidatorPassed(string text)
        {
            Boolean result = true;

            if (text == null || text == string.Empty)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Checks if string is between minLength and maxLength
        /// </summary>
        /// <returns>true if its in wanted range , otherwise false</returns>
        public static Boolean StringRangeValidatorPassed(int minLength, int maxLength, string text)
        {
            if (minLength < 0)
            {
                throw new BusinessException("minLength is < 1");
            }
            if (maxLength < 0)
            {
                throw new BusinessException("maxLength is < 1");
            }
            if (maxLength > 0 && minLength > maxLength)
            {
                throw new BusinessException("minLenght is more than maxLength");
            }
            if (maxLength == 0 && minLength == 0)
            {
                throw new BusinessException("minLength and maxLength are 0");
            }

            Boolean result = true;

            if (minLength != 0 && string.IsNullOrEmpty(text))
            {
                result = false;
            }
            else
            {
                if (minLength > 0)
                {
                    if (text.Length < minLength)
                    {
                        result = false;
                    }
                }

                if (maxLength > 0)
                {
                    if (text.Length > maxLength)
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Checks if string can be parsed to wanted type
        /// </summary>
        /// <param name="type">int , long , float , double</param>
        /// <returns>true if it can , otherwise false</returns>
        public static Boolean TypeValidatorPassed(string type, string text)
        {
            //type = int,long,float,double       
            if (type == null || type == "")
            {
                throw new BusinessException("invalid type");
            }

            if (type != "int" && type != "long" && type != "float" && type != "double")
            {
                throw new BusinessException(string.Format("type = {0} is not supported type", type));
            }

            Boolean result = true;

            if (text == null || text == "")
            {
                result = false;
            }
            else
            {
                if (type == "int")
                {
                    int test = -1;
                    if (!int.TryParse(text, out test))
                    {
                        result = false;
                    }
                }
                else if (type == "long")
                {
                    long test = -1;
                    if (!long.TryParse(text, out test))
                    {
                        result = false;
                    }
                }
                else if (type == "float")
                {
                    float test = 1;
                    if (!float.TryParse(text, out test))
                    {
                        result = false;
                    }
                }
                else if (type == "double")
                {
                    double test = -1.0;
                    if (!double.TryParse(text, out test))
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if string have Special symbols in it
        /// </summary>
        /// <returns>true if it doesnt have , otherwise false</returns>
        public static Boolean SymbolsValidatorPassed(string text)
        {
            Boolean result = true;

            if (text == null || text == "")
            {
                result = false;
            }
            else
            {
                if (text.Contains(" "))
                {
                    result = false;
                }
                {
                    text = text.ToLowerInvariant();
                    Char[] symbols = GetEnglishChars(true);

                    for (int i = text.Length - 1; i >= 0; i--)
                    {
                        if (!symbols.Contains(text[i]))
                        {
                            result = false;
                            break;
                        }
                    }

                }

            }
            return result;
        }

        /// <summary>
        /// Checks if string have Offensive words in it
        /// </summary>
        /// <returns>false if it have offensive words, otherwise true</returns>
        public static Boolean WordsValidatorPassed(string text)
        {
            Boolean result = true;

            if (text == null || text == "")
            {
                result = false;
            }
            else
            {
                String[] words = new String[] { "fuck", "shit", "stupid" };
                string[] delimiters = new string[] { 
                    Environment.NewLine, " ", "\t", "\r", "\n", 
                    ",", "'", "\"", ";", ".", ":",
                    "~", "!", "?", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_",
                    "+", "-", "=", "/", "\\", "<", ">", "|"
                    // , etc.
                    };
                string[] textWords = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                string[] outerArr = words;
                string[] innerArr = textWords;

                // Performane optimization
                if (outerArr.Length > innerArr.Length)
                {
                    outerArr = textWords;
                    innerArr = words;
                }

                for (int i = 0;
                    result == true && i < outerArr.Length;
                    i++)
                {
                    string word1 = outerArr[i];

                    foreach (String word2 in innerArr)
                    {
                        if (string.Compare(word1, word2, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if Email adress has correct form 
        /// </summary>
        /// <returns>true if it have correct form, otherwise false</returns>
        public static bool EmailValidatorPassed(string emailAddress)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                emailAddress = emailAddress.Replace(" ", string.Empty);
            }

            // copy&paste from http://www.regular-expressions.info/email.html
            string patternStrict = @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$";
            Regex reStrict = new Regex(patternStrict, RegexOptions.IgnoreCase);

            bool isStrictMatch = reStrict.IsMatch(emailAddress);
            return isStrictMatch;
        }

        /// <summary>
        /// CHecks if website url is in correct form
        /// </summary>
        public static bool WebSiteValidatorPassed(string webSite)
        {   
            bool passed = false;

            int minLength = 4;
            if (webSite.Contains("http://"))
            {
                minLength += 7;
            }
            else if (webSite.Contains("https://"))
            {
                minLength += 8;
            }

            if (!string.IsNullOrEmpty(webSite))
            {
                if (!webSite.Contains(" ") && webSite.Contains(".") && webSite.Length >= minLength)
                {
                    passed = true;
                }
            }

            return passed;
        }

        /// <summary>
        /// returns english alphabet in lowercase
        /// </summary>
        /// <param name="withNumbers">if true returns also the 0-9 numbers , if false not</param>
        public static Char[] GetEnglishChars(bool withNumbers)
        {
            if (withNumbers)
            {
                char[] letters = {'0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f','g','h','i','j','k','l','m','n',
                             'o','p','q','r','s','t','u','v','w','x','y','z'}; //36
                return letters;
            }
            else
            {
                char[] letters = {'a','b','c','d','e','f','g','h','i','j','k','l','m','n',
                             'o','p','q','r','s','t','u','v','w','x','y','z'}; //26
                return letters;
            }

        }

        /// <summary>
        /// Checks if character is from english alphabet
        /// </summary>
        /// <param name="withNumbers">true if numbers are acceptable , oterwise false</param>
        /// <returns>true if it is , otherwise false</returns>
        public static Boolean IsCharFromEnglishAlphabet(Char letter, bool withNumbers)
        {
            Boolean result = false;
            if (withNumbers)
            {
                Char[] enChars = GetEnglishChars(true);
                foreach (Char character in enChars)
                {
                    if (character.Equals(letter))
                    {
                        result = true;
                        break;
                    }
                }
            }
            else
            {
                Char[] enChars = GetEnglishChars(false);
                foreach (Char character in enChars)
                {
                    if (character.Equals(letter))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Cuts string if its length is more than wanted
        /// </summary>
        /// <param name="cutFromStart">true if the string should be cutted from start or false if from end</param>
        /// <param name="withPoints">true if points "..." should be added at the place where string is cutted</param>
        /// <returns></returns>
        public static String TrimString(String text, int LengthWanted, bool cutFromStart, bool withPoints)
        {
            if (text == null || text.Length < 1)
            {
                return string.Empty;
            }
            if (LengthWanted < 1)
            {
                throw new BusinessException("LengthWanted is < 1");
            }

            int length = text.Length;
            if (length > LengthWanted)
            {
                String subStr = "";

                if (cutFromStart)
                {
                    subStr = text.Substring(length - LengthWanted, LengthWanted);
                    if (withPoints)
                    {
                        subStr = string.Format("...{0}", subStr);
                    }
                }
                else
                {
                    subStr = text.Substring(0, length - (length - LengthWanted));
                    if (withPoints)
                    {
                        subStr = string.Format("{0}...", subStr);
                    }
                }
                return subStr;
            }
            else
            {
                return text;
            }
        }

        /// <summary>
        /// Checks if string1 is equal with string2
        /// </summary>
        /// <returns>true if they are equal , otherwisw false</returns>
        public static Boolean MatchFieldsValidatorPassed(String text1, String text2)
        {
            Boolean pass = false;
            if (text1 == null || text1 == string.Empty || text2 == null || text2 == string.Empty)
            {
                return pass;
            }

            if (text1.Equals(text2, StringComparison.Ordinal))
            {
                pass = true;
            }

            return pass;
        }

        /// <summary>
        /// Returns formatted text from database (adds newline`s and spaces between words so the text can be shows as it was written)
        /// </summary>
        public static String GetFormattedTextFromDB(String text)
        {
            if (text == null || text == string.Empty)
            {
                return text;
            }

            string FormattedStr;

            FormattedStr = text.Replace(Environment.NewLine, "<br/>");
            FormattedStr = FormattedStr.Replace("\n", "<br/>");
            FormattedStr = FormattedStr.Replace("\r", "<br/>");

            FormattedStr = FormattedStr.Replace("<br/> ", "<br/>&nbsp;");
            FormattedStr = FormattedStr.Replace("  ", " &nbsp;");

            return FormattedStr;
        }

        /// <summary>
        /// Escapes \n\r in string. Used when giving string to javascript
        /// </summary>
        public static string EscapeNewLinesInString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string FormattedStr;

            FormattedStr = text;
            FormattedStr = FormattedStr.Replace("\\", "\\\\");
            FormattedStr = FormattedStr.Replace("\r", "\\r");
            FormattedStr = FormattedStr.Replace("\n", "\\n");

            return FormattedStr;
        }

        /// <summary>
        /// Returns CategoryName with the path to the category
        /// </summary>
        public static String CategoryName(Entities objectContext, Category currCategory, bool styled)
        {
            BusinessCategory businessCategory = new BusinessCategory();
            Category category = currCategory;

            if (currCategory.parentID == null)
            {
                return currCategory.name;
            }

            List<String> categoryNames = CategoryNameAsList(objectContext, category);

            System.Text.StringBuilder name = new StringBuilder();

            int i = categoryNames.Count<String>();
            if (i == 1)
            {
                name.Append(categoryNames.First<String>());
            }
            else if (i == 0)
            {
                throw new BusinessException("Error in cycle for creating a category name . List length is 0");
            }
            else
            {
                for (int num = 0; num < i; num++)
                {
                    if ((num + 1) == i)
                    {
                        name.Append(categoryNames[num]);
                    }
                    else
                    {
                        name.Append(categoryNames[num]);
                        if (styled)
                        {
                            name.Append("<span class=\"searchPageRatings\"> > </span>");
                        }
                        else
                        {
                            name.Append(" > ");
                        }
                    }
                }
            }

            return name.ToString();
        }

        public static List<string> CategoryNameAsList(Entities objectContext, Category currCategory)
        {
            BusinessCategory businessCategory = new BusinessCategory();

            Category category = currCategory;

            List<String> categoryNames = new List<String>();

            if (currCategory.parentID == null)
            {
                categoryNames.Add(category.name);

                return categoryNames;
            }

            while (category.parentID != null)
            {
                categoryNames.Add(category.name);
                if (category.parentID != null)
                {
                    category = businessCategory.GetWithoutVisible(objectContext, category.parentID.Value);
                }
                else
                {
                    break;
                }
            }

            categoryNames.Reverse();
            return categoryNames;
        }

        /// <summary>
        /// Checks if text have words with length more than maxLength
        /// </summary>
        /// <returns>true if there isnt words bigger than maxLength , otherwise false</returns>
        public static Boolean IsTextWrapped(String text, int maxLength)
        {
            Boolean result = true;

            if (text.Length > maxLength)
            {

                char[] separators = new char[] { ' ', '\n' };
                string[] separatedText;

                separatedText = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                long count = separatedText.Count();

                if (count > 0)
                {
                    for (long i = 0; i < count; i++)
                    {
                        if (separatedText[i].Length > maxLength)
                        {
                            result = false;
                            break;
                        }
                    }
                }

                return result;
            }
            else
            {
                return result;
            }

        }


        /// <summary>
        /// Parses DateTime to string in InvariantCulture
        /// </summary>
        /// <returns>parsed DateTime in string variant</returns>
        public static string ParseDateTimeToString(DateTime time)
        {
            string strDate = time.ToString("d", System.Globalization.CultureInfo.InvariantCulture); // mm/dd/yyyy format
            return strDate;
        }

        /// <summary>
        /// Parses string to Datetime, returns true if parsing succesfful otherwise false
        /// </summary>
        public static Boolean ParseStringToDateTime(string strDate, out DateTime time)
        {
            Boolean succ = DateTime.TryParse(strDate, System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out time);
            return succ;
        }

        /// <summary>
        /// Checks from and to parameters which are used for showing objects from range
        /// </summary>
        public static void CheckFromToParameters(long from, long to)
        {
            if (from >= to || from < 0)
            {
                throw new BusinessException("incorrect from - to parameters");
            }
        }

        /// <summary>
        /// Converts first char in string to Uppercase
        /// </summary>
        public static string GetStringWithCapital(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                return string.Format("{0}{1}", char.ToUpper(word[0]), word.Substring(1));
            }
            else
            {
                return word;
            }
        }


        /// <summary>
        /// Returns separated IDs, typed on ADD/EDIT Advertisement (categories, products, companies)
        /// , also removes duplicating values
        /// </summary>
        public static bool GetAdvertLinkIDsFromString(string ids, out List<long> sepIDs)
        {
            bool goodFormat = false;

            sepIDs = new List<long>();

            if (!string.IsNullOrEmpty(ids))
            {
                char[] charSeparators = new char[] { ',' };
                string[] separated = ids.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

                long id = -1;

                foreach (string strId in separated)
                {
                    if (long.TryParse(strId, out id))
                    {
                        if (!sepIDs.Contains(id))
                        {
                            sepIDs.Add(id);
                        }
                    }
                    else
                    {
                        return goodFormat;
                    }
                }

                goodFormat = true;
            }

            return goodFormat;
        }

        /// <summary>
        /// Returns true if two lists are diffent, otherwise false (NOTE : lists must not have repeating values)
        /// </summary>
        public static bool AreTwoListsDiffent(List<long> newList, List<long> oldList)
        {
            bool different = false;

            if (newList.Count == oldList.Count)
            {
                for (int i = 0; i < newList.Count; i++)
                {
                    if (!oldList.Contains(newList[i]) || !newList.Contains(oldList[i]))
                    {
                        different = true;
                    }
                }
            }
            else
            {
                different = true;
            }

            return different;
        }

        /// <summary>
        /// Returns List of ID`s as string in form of "123, 123, 345, 1, 10 ..."
        /// </summary>
        public static string GetIDsAsString(List<long> idList)
        {
            string ids = "";

            if (idList.Count > 0)
            {
                StringBuilder strIDs = new StringBuilder();
                foreach (long id in idList)
                {
                    strIDs.Append(id);
                    strIDs.Append(", ");
                }

                ids = strIDs.ToString();
            }

            return ids;
        }

        public static string BreakPossibleHtmlCode(string text)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(text))
            {
                result = text.Replace("<", "< ");
            }

            return result;
        }

        /// <summary>
        /// Gets a string value identifying the application variant that is derived from the current thread UI culture.
        /// <para>For example: "en", "bg", etc.</para>
        /// </summary>
        public static string ApplicationVariantString
        {
            get
            {
                CultureInfo uiCulture = Thread.CurrentThread.CurrentUICulture;
                string cultureStr = uiCulture.Name;
                int dashInd = cultureStr.IndexOf("-");
                if (dashInd != -1)
                {
                    cultureStr = cultureStr.Substring(0, dashInd);
                }
                if (string.IsNullOrEmpty(cultureStr) == true)
                {
                    cultureStr = Configuration.DefaultApplicationVariant;
                }
                return cultureStr;
            }
        }

        /// <summary>
        /// Gets a resource string by its name.
        /// <para>The current thread UI culture is taken in consideration.</para>
        /// </summary>
        /// <param name="name">The string resource name</param>
        /// <returns>The requested resource or an empty string on error.</returns>
        public static string GetResource(string name)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(name) == false)
            {
                System.Resources.ResourceManager rm =
                    new System.Resources.ResourceManager("BusinessLayer.Resources", System.Reflection.Assembly.GetExecutingAssembly());
                System.Globalization.CultureInfo cInfo = Thread.CurrentThread.CurrentUICulture;
                result = rm.GetString(name, cInfo);
            }
            if (result == null)
            {
                result = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// Gets a resource string by its name.
        /// <para>The current thread UI culture is taken in consideration.</para>
        /// </summary>
        /// <param name="name">The string resource name</param>
        /// <returns>The requested resource or an empty string on error.</returns>
        public static string GetConfigurationResource(string name)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(name) == false)
            {
                System.Resources.ResourceManager rm =
                    new System.Resources.ResourceManager("BusinessLayer.ConfigurationResources", System.Reflection.Assembly.GetExecutingAssembly());
                System.Globalization.CultureInfo cInfo = Thread.CurrentThread.CurrentUICulture;
                result = rm.GetString(name, cInfo);
            }
            if (result == null)
            {
                result = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// Gets an <see cref="Entities"/> object for the application variant that is derived from the current thread UI culture.
        /// </summary>
        /// <returns>The requested <see cref="Entities"/> instance.</returns>
        public static Entities GetTemporaryEntities()
        {
            string connStr = Configuration.GetEntitiesConnectionString(ApplicationVariantString);
            Entities result = new Entities(connStr);
            return result;
        }

        /// <summary>
        /// Deletes the redundant whitespace and adjacent repetitions of new-line sequences from a string.
        /// </summary>
        /// <param name="original">The string to delete the redundant substrings from.</param>
        /// <param name="toRemove">An array containing the substrings which redundant repetitions to delete.
        /// <para>This array cannot contain <c>null</c>s or empty strings.</para></param>
        /// <param name="maxRepetitions">The maximum allowed number of repetitions of the substrings.
        /// <para>Must be a positive number.</para></param>
        /// <returns>The resulting string without redundant substring repetitions.</returns>
        public static string DeleteExtraWhitespaseAndNewLines(string original, int maxRepetitions)
        {
            if (string.IsNullOrEmpty(original))
            {
                return original;
            }

            if (maxRepetitions <= 0)
            {
                throw new ArgumentException("Number must not be less than or equal to zero.", "maxRepetitions");
            }

            string intermediate = DeleteTrailingLineWhitespace(original);

            List<string> stringsToRemove = new List<string>();

            AddPrefixToNewLines(maxRepetitions, stringsToRemove, " ");
            AddPrefixToNewLines(maxRepetitions, stringsToRemove, "\t");

            string result = DeleteAdjacentSubstringRepetitions(intermediate, stringsToRemove, maxRepetitions);
            return result;
        }

        /// <summary>
        /// Deletes the whitespace sybmols that precede either of the '\r' or '\n' symbols.
        /// </summary>
        /// <param name="original">The string to remove the trailing whitestpace at line end from.</param>
        /// <returns>The resulting string without whitespace at line ends.</returns>
        public static string DeleteTrailingLineWhitespace(string original)
        {
            if (original == null)
            {
                throw new ArgumentNullException("original");
            }

            StringBuilder resulting = new StringBuilder();

            if (original != string.Empty)
            {
                bool lineEndPassed = true;  // Assume the end of the string is also a line end.

                for (int i = (original.Length - 1); i >= 0; i--)
                {
                    char ch = original[i];
                    bool atLineEndChar = ((ch == '\r') || (ch == '\n'));

                    if (atLineEndChar == true)
                    {
                        lineEndPassed = true;
                    }
                    else if (char.IsWhiteSpace(ch) == true)
                    {
                        // Nothing. lineEndPassed keeps its old value.
                    }
                    else
                    {
                        lineEndPassed = false;
                    }
                    if ((lineEndPassed == false) || (atLineEndChar == true))
                    {
                        resulting.Insert(0, ch);
                    }
                }
            }
            string resultStr = resulting.ToString();
            return resultStr;
        }

        /// <summary>
        /// Inserts a specified prefix fom zero to maxRepetitions times before the known end-of-line sequences and
        /// adds the resulting strings to a list. String duplications in the list are avoided.
        /// </summary>
        /// <param name="maxRepetitions">The maximum number of repetitions of the <paramref name="prefix"/>.</param>
        /// <param name="lineEnds">The <see cref="List&lt;string&gt;"/> to add the strings to.</param>
        /// <param name="prefix">The prefix to insert before end-of-line sequenses.</param>
        private static void AddPrefixToNewLines(int maxRepetitions, List<string> lineEnds, string prefix)
        {
            if (lineEnds == null)
            {
                throw new ArgumentNullException("lineEnds");
            }
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            if (maxRepetitions <= 0)
            {
                throw new ArgumentException("Number must not be less than or equal to zero.", "maxRepetitions");
            }

            string newLineThisOs = Environment.NewLine;
            string newLineWindows = "\r\n";
            string newLineLinux = "\n";
            string newLineOldMac = "\r";
            for (int i = 0; i <= maxRepetitions; i++)
            {
                if (i > 0)
                {
                    newLineWindows = prefix + newLineWindows;
                    newLineLinux = prefix + newLineLinux;
                    newLineOldMac = prefix + newLineOldMac;
                    newLineThisOs = prefix + newLineThisOs;
                }
                else
                {
                    lineEnds.Add(prefix);
                }
                if (lineEnds.Contains(newLineWindows) == false)
                {
                    lineEnds.Add(newLineWindows);
                }
                if (lineEnds.Contains(newLineLinux) == false)
                {
                    lineEnds.Add(newLineLinux);
                }
                if (lineEnds.Contains(newLineOldMac) == false)
                {
                    lineEnds.Add(newLineOldMac);
                }
                if (lineEnds.Contains(newLineThisOs) == false)
                {
                    lineEnds.Add(newLineThisOs);
                }
            }
        }

        /// <summary>
        /// Compares the lenghts of the supplied strings.
        /// </summary>
        /// <param name="a">The first string.</param>
        /// <param name="b">The second string.</param>
        /// <returns>If string <paramref name="a"/> is shorter - a negative value;
        /// If string <paramref name="a"/> is longer - a positive value;
        /// Otherwise - zero.</returns>
        private static int ShorterFirst(string a, string b)
        {
            if ((a == null) && (b == null))
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
            else
            {
                return (a.Length - b.Length);
            }
        }

        /// <summary>
        /// Compares the lenghts of the supplied strings.
        /// </summary>
        /// <param name="a">The first string.</param>
        /// <param name="b">The second string.</param>
        /// <returns>If string <paramref name="a"/> is longer - a negative value;
        /// If string <paramref name="a"/> is shorter - a positive value;
        /// Otherwise - zero.</returns>
        private static int LongerFirst(string a, string b)
        {
            int lenghtComparison = ShorterFirst(b, a);  // Order deliberately reversed.
            return lenghtComparison;
        }

        /// <summary>
        /// Deletes the redundant adjacent repetitions of specified substrings from a string.
        /// </summary>
        /// <param name="original">The string to delete the redundant substrings from.</param>
        /// <param name="toRemove">A <see cref="List&lt;string&gt;"/> containing the substrings which redundant repetitions to delete.
        /// <para>This array cannot contain <c>null</c>s or empty strings.</para></param>
        /// <param name="maxRepetitions">The maximum allowed number of repetitions of the substrings.
        /// <para>Must be a positive number.</para></param>
        /// <returns>The resulting string without redundant substring repetitions.</returns>
        public static string DeleteAdjacentSubstringRepetitions(string original, List<string> toRemove, int maxRepetitions)
        {
            if (string.IsNullOrEmpty(original))
            {
                return original;
            }

            if (toRemove == null)
            {
                throw new ArgumentNullException("toRemove");
            }

            if (maxRepetitions <= 0)
            {
                throw new ArgumentException("Number must not be less than or equal to zero.", "maxRepetitions");
            }
            string resultStr = original;
            string oldResultStr = resultStr;
            bool shortStringsFirst = false;
            do
            {
                oldResultStr = resultStr;
                if (shortStringsFirst == true)
                {
                    toRemove.Sort(new Comparison<string>(ShorterFirst));
                }
                else
                {
                    toRemove.Sort(new Comparison<string>(LongerFirst));
                }
                string[] toRemoveArr = toRemove.ToArray<string>();
                resultStr = InternalDeleteAdjacentSubstringRepetitions(resultStr, toRemoveArr, maxRepetitions);
                shortStringsFirst = (!shortStringsFirst);
            }
            while (resultStr != oldResultStr);
            return resultStr;
        }

        /// <summary>
        /// Deletes the redundant adjacent repetitions of specified substrings from a string.
        /// </summary>
        /// <param name="original">The string to delete the redundant substrings from.</param>
        /// <param name="toRemove">An array containing the substrings which redundant repetitions to delete.
        /// <para>This array cannot contain <c>null</c>s or empty strings.</para></param>
        /// <param name="maxRepetitions">The maximum allowed number of repetitions of the substrings.
        /// <para>Must be a positive number.</para></param>
        /// <returns>The resulting string without redundant substring repetitions.</returns>
        private static string InternalDeleteAdjacentSubstringRepetitions(string original, string[] toRemove, int maxRepetitions)
        {
            string resultStr = original;

            if (toRemove.Length > 0)
            {
                string[] longestForms = new string[toRemove.Length];

                for (int i = 0; i < toRemove.Length; i++)
                {
                    StringBuilder lfSb = new StringBuilder();
                    string toRmv = toRemove[i];
                    for (int r = 0; r < maxRepetitions; r++)
                    {
                        lfSb.Append(toRmv);
                    }
                    longestForms[i] = lfSb.ToString();
                }

                int position = 0;
                StringBuilder resulting = new StringBuilder();

                while (position < original.Length)
                {
                    int nextLongestFormInd = original.Length;
                    int toRemoveInd = -1;
                    int startingPosition = position;  // For error detection

                    // Find where the closest substring with maximum allowed repetitions is.
                    for (int j = 0; j < longestForms.Length; j++)
                    {
                        int ind = original.IndexOf(longestForms[j], position, StringComparison.OrdinalIgnoreCase);
                        if ((position <= ind) && (ind < nextLongestFormInd))
                        {
                            nextLongestFormInd = ind;  // Memorize the position
                            toRemoveInd = j;  // Memorize which is the closest substring
                        }
                    }

                    // Copy the symbols of the original string from the current position to the
                    // position where the closest substring with maximum allowed repetitions begins.
                    // This must be done even if no substring with maximum allowed repetitions is found.
                    for (int k = position; k < nextLongestFormInd; k++)
                    {
                        resulting.Append(original[k]);
                    }
                    position = nextLongestFormInd;

                    // if s substring with maximum allowed repetitions is found, copy it.
                    if (toRemoveInd != -1)
                    {
                        int afterLongestForm = nextLongestFormInd + longestForms[toRemoveInd].Length;
                        for (int l = position; l < afterLongestForm; l++)
                        {
                            resulting.Append(original[l]);
                        }
                        position = afterLongestForm;
                    }

                    // Skip the redundant occurences.
                    if (toRemoveInd != -1)
                    {
                        string testStart = toRemove[toRemoveInd];
                        while (original.IndexOf(testStart, position, StringComparison.OrdinalIgnoreCase) == position)
                        {
                            position += testStart.Length;
                        }
                    }
                    if (position <= startingPosition)
                    {
                        throw new InvalidOperationException(
                            "Position is not incremented. This will cause an endless loop.");
                    }
                    if (position > original.Length)
                    {
                        throw new InvalidOperationException(
                            "Incorrect processing: position greater than the lenght of the original string.");
                    }
                }

                resultStr = resulting.ToString();
            }
            return resultStr;
        }

        public static bool CheckIfFileExists(string appPath, string url)
        {

            bool result = false;

            if (!string.IsNullOrEmpty(appPath) && !string.IsNullOrEmpty(url))
            {

                string completePath = System.IO.Path.Combine(appPath, url);

                if (url != string.Empty)
                {
                    if (File.Exists(completePath))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Replaces br tags with new lines and &nbsp; with spaces 
        /// </summary>
        public static string DecodeHtmlSpacesAndNewLines(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Replace("<br />", "<br/>");
                text = text.Replace("<br/>", Environment.NewLine);
                text = text.Replace("&nbsp;", " ");
            }
            return text;
        }

        /// <summary>
        /// Breaks long words in string. (breaks with space)
        /// </summary>
        public static string BreakLongWordsInString(string text, int maxWordLength)
        {
            if (text.Length > maxWordLength)
            {
                string[] separator = { " " };
                string[] words = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                if (words.Count() > 0)
                {
                    string substring1 = string.Empty;
                    string substring2 = string.Empty;

                    bool wordBreaked = false;

                    for (int i = 0; i < words.Count(); i++)
                    {
                        if (words[i].Length > maxWordLength)
                        {
                            words[i] = BreakWord(words[i], maxWordLength);
                            wordBreaked = true;
                        }
                    }

                    if (wordBreaked == true)
                    {
                        StringBuilder breakedText = new StringBuilder();
                        foreach (string word in words)
                        {
                            breakedText.Append(word);
                            breakedText.Append(" ");
                        }

                        text = breakedText.ToString();
                    }
                }

            }

            return text;
        }

        private static string BreakWord(string word, int maxWordLength)
        {
            int length = word.Length;

            if (length > maxWordLength)
            {
                StringBuilder breakedWord = new StringBuilder();

                int startIndex = 0;

                while (startIndex < length)
                {
                    if ((startIndex + maxWordLength) >= length)
                    {
                        breakedWord.Append(word.Substring(startIndex));
                        break;
                    }
                    else
                    {
                        breakedWord.Append(word.Substring(startIndex, maxWordLength));
                        breakedWord.Append(" ");
                    }

                    int asd = breakedWord.Length;

                    startIndex += maxWordLength;
                }

                word = breakedWord.ToString();
            }

            return word;
        }

        /// <summary>
        /// Returns string with random numbers in 16bit variant
        /// </summary>
        public static string GetRandomNumberIn16Bit(int length)
        {
            byte[] numbers = new byte[length];
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();
            Gen.GetBytes(numbers);

            StringBuilder str = new StringBuilder();
            for (int j = 0; j < numbers.Length; j++)
            {
                str.Append(numbers[j].ToString("X2"));
            }

            return str.ToString();
        }

        public static string RemoveSpacesAtEndOfString(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                while (text.Length > 0 && text.EndsWith(" ", StringComparison.InvariantCultureIgnoreCase))
                {
                    text = text.Remove(text.Length - 1);
                }

            }

            return text;
        }

    }
}
