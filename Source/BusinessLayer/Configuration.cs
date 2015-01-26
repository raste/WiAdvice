﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Specialized;
using System.Configuration;

namespace BusinessLayer
{
    public static class Configuration
    {
        static Configuration()
        {
            Load();
        }

        /// <summary>
        /// The default application variant - english.
        /// </summary>
        public static readonly string DefaultApplicationVariant = "en";

        // General
        public static Boolean SiteRunning = false;

        public static Boolean CheckForAlreadyLoggedUserOnLogIn = true;
        public static Boolean CheckIfUserIsActivatedOnLogin = false;
        public static Boolean SendActivationCodeOnRegistering = false;

        public static int MaximumNumberOfUsersRegisteredWithMail = 1;   // min 1, max 10
        public static int RegisterMaxNumberRegistrationsFromIp = 3;             // min 1

        public static Boolean UseUrlRewriting = false;

        /// <summary>
        /// Whether to use an external URL Rewrite module.
        /// <para>If an external URL Rewrite module is used - <c>true</c>; otherwise - <c>false</c>.</para>
        /// </summary>
        public static bool UseExternalUrlRewriteModule = false;

        public static byte UrlRewritingDirectoryLevel = 255;

        public static Boolean SendMailsViaSSL = false;
        public static string SiteSupportMail = "mail@domain.com";         // For automatic messages mostly

        public static string SiteGeneralSectionMail = "mail@domain.com";         //NOT USED ATM.. For contact mails from users
        public static string SiteSupportSectionMail = "mail@domain.com";
        public static string SiteAdvertisementsSectionMail = "mail@domain.com";

        public static string SiteDomainAdress = "http://localhost/SiteProject/";        // Needs to End with /
        public static string SiteName = "www.wiadvice.com";

        // IP attempts
        public static int IpAttemptMaxNumTries = 3;                             // min 1
        public static int IpAttemptMinTimeWhichNeedsToPassAfterPageLoad = 2;    // in seconds , min 1
        public static int IpAttemptTimeWhichNeedsToPassToResetTries = 30;       // in minutes , min 1
        public static int IpAttemptFailuresAfterWhichConsideredBot = 3;         // min 1

        // CACHE
        public static int CacheDefaultExpireTimeInMinutes = 1;                  // min 1

        // Other
        public static int ProdCompMinAfterWhichUserCannotEditCrucialData = 30;  // in minutes, min 0

        // Products
        public static int ProductsMinProductNameLength = 2;                     // min 2
        public static int ProductsMaxProductNameLength = 20;                    // max 100
        public static int ProductsMinCommentsOnPage = 25;                       // min 1
        public static int ProductsDefCommentsOnPage = 50;
        public static int ProductsMaxCommentsOnPage = 100;                      // max 500
        public static int ProductsCommentsOnPage = ProductsMinCommentsOnPage;   // not in web.config     
        public static int ProductsMaxNumberUserCommentsPerProduct = 2;          // min 1
        public static bool ProductsCheckForIpAdressIfUserIsLogged = true;
        public static bool ProductsCheckForIpAdressIfUserNotLogged = true;
        public static int ProductsMinTimeBetweenComments = 10;                  // min 0
        public static int ProductsMinCommentsAfterWhichTimeIsInvalid = 20;      // min 0
        public static int ProductsMinDescriptionLength = 10;                    // min 0                     
        public static int ProductsMaxDescriptionLength = 500;
        public static bool ProductsCanUserTakeRoleIfNoEditors = true;
        public static int ProductsTimeWhichNeedsToPassToAddAnother = 5;         // min 0
        public static int ProductsMaxAlternativeNames = 3;                      // min 1

        public static int ProductsMaxVariants = 10;                             // min 1                     
        public static int ProductsMaxVariantSubVariants = 500;                  // min 1 

        public static int ProductLinksMaxPerProduct = 10;                       // min 1
        public static int ProductLinksMinDescrLength = 10;                      // min 0
        public static int ProductLinksMaxDescrLength = 200;                     // min 1   
        public static int ProductLinksMinTimeBetweenAdding = 2;                 // min 0


        // Warnings
        public static int WarningsNumberOnActionsOnWhichShouldRemoveAction = 3; // min 1                     
        public static int WarningsOnHowManyToDeleteUser = 6;                    // min 1

        // Comments
        public static int CommentsMaxCommentsReplyLevel = 5;                    // max 20
        public static int CommentsMinCommentDescriptionLength = 2;              // min 2
        public static int CommentsMaxCommentDescriptionLength = 7000;           // max 50 000
        public static int CommentsMaxWordLength = 30;                           // min 10, max 100

        // Ratings
        public static int CommRatingMaxUserRatingsPerProduct = 10;              // min 1
        public static int CommRatingMaxUserRatingsForDay = 30;                  // min 1
        public static int CommRatingMaxUserRatingsPerTopic = 30;                // min 1

        // Images
        public static int ImagesThumbnailPictureWidth = 150;                    // min 50 , max 500
        public static int ImagesThumbnailPrictureHeight = 150;
        public static int ImagesMaxProductImagesCount = 20;                     // min 1 , max 100
        public static int ImagesMaxCompanyImagesCount = 20;
        public static int ImagesMinPictureDescriptionLength = 0;
        public static int ImagesMaxPictureDescriptionLength = 250;              // max 1000
        public static int ImagesMinImageWidth = 500;                            // min 100
        public static int ImagesMinImageHeight = 500;                           // min 100
        public static int ImagesMainImageWidth = 300;                           // min 200
        public static int ImagesMainImageHeight = 300;                          // min 200
        public static int ImagesCategoryMaxHeight = 150;                        // min 50
        public static int ImagesCategoryMaxWidth = 250;                         // min 50
        public static int ImagesMinCompLogoWidth = 100;                         // min 50
        public static int ImagesMinCompLogoHeight = 100;                        // min 50

        // Users
        public static int UsersMinUserNameLength = 2;                           // min 2
        public static int UsersMaxUserNameLength = 20;                          // max 100
        public static int UsersMinPasswordLength = 4;                           // min 3
        public static int UsersMaxPasswordLength = 100;                         // max 100
        public static int UsersMinSignatureLength = 2;                          // min 1
        public static int UsersMaxSignatureLength = 250;                        // max 500
        public static int UsersUserCommentsOnPage = 5;                          // 0 < comm <= 200

        // Fields
        public static int FieldsMinDescriptionFieldLength = 10;                 // min 1
        public static int FieldsMaxDescriptionFieldLength = 7000;               // max 100 000
        public static int FieldsMinIdFieldLength = 4;                           // min 4  
        public static int FieldsMaxIdFieldLength = 11;                          // max 11
        public static int FieldsDefMaxWordLength = 50;                          // 10 <= num <= 100

        // Home
        public static long HomePageNumberLastAddedProducts = 10;                // min 1, max 100
        public static long HomePageNumberNewsOnPage = 5;                        // min 1, max 20
        public static long HomePageNumberLastAddedCompanies = 10;

        // Notifies
        public static int NotifiesMaxNumberNotifies = 100;                // min 1, max 1000

        // Company
        public static int CompaniesMinCompanyNameLength = 2;                    // min 2
        public static int CompaniesMaxCompanyNameLength = 20;                   // max 200
        public static int CompaniesProductsOnPage = 50;                         // min 1, max 200
        public static string CompanyName = "maker";                             // lowercase, single number
        public static int CompaniesMinDescriptionLength = 10;                   // min 0            
        public static int CompaniesMaxDescriptionLength = 500;
        public static bool CompaniesCanUserTakeRoleIfNoEditors = true;
        public static int CompaniesTimeWhichNeedsToPassToAddAnother = 10;       // min 0
        public static int CompaniesMaxAlternativeNames = 3;                     // min 1

        // Category
        public static int CategoriesMinCategoryNameLength = 2;                  // min 1
        public static int CategoriesMaxCategoryNameLength = 40;                 // max 200
        public static int CategoriesMinCategoryDescriptionLength = 0;           // min 0
        public static int CategoriesMaxCategoryDescriptionLength = 7000;        // max 100 000
        public static int CategoriesMinProdNumToShowLastProductsTbl = 40;       // min 1 , max 100
        public static int CategoriesNumOfProductsToShowInLastProductsTbl = 20;  // min 1 , max 100
        public static int CategoriesNumOfMostCommentedProducts = 10;            // min 1 , max 100
        public static int CategoriesProdsPerPage = 30;                          // min 1 , max 100

        // Messages
        public static int MessagesMinSubjectLength = 0;                         // min 0
        public static int MessagesMaxSubjectLength = 50;                        // max 100

        // Site Texts  
        public static int SiteTextsNumLastTextsToShow = 20;                     // 0 < num <= 200
        public static int SiteTextsMinSiteTextDescr = 1;                        // min 1
        public static int SiteTextsMaxSiteTextDescr = 20000;                    // max 100 000
        public static int SiteTextsMinSiteTextNameLength = 1;                   // min 1
        public static int SiteTextsMaxSiteTextNameLength = 100;                 // max 200

        // Search
        public static int SearchItemsPerSearchPage = 50;                        // 0 < num <= 200
        public static int SearchMaxSearchStringLength = 100;                    // 10 <= num <= 500
        public static int SearchAlternativeItemsPerSearchPage = 10;         // min 1

        // Suggestions
        public static int SuggestionsPerPage = 50;                              // 0 < num <= 200
        public static int SuggestionsMaxUserSuggestions = 3;                    // 1 <= num <= 10

        // Adverts
        public static bool AdvertsCanAdvertBeUndeleted = false;
        public static int AdvertsNumAdvertsOnCategoryPage = 3;                  // 
        public static int AdvertsNumAdvertsOnCompanyPage = 3;                   // 
        public static int AdvertsNumAdvertsOnProductPage = 1;                   //  0 <= num <= 50   , for all
        public static int AdvertsNumAdvertsOnSearchPage = 2;                    // 
        public static int AdvertsNumAdvertsOnUserPage = 1;                      // 
        public static int AdvertsNumAdvertsOnSuggestionsPage = 3;               //
        public static int AdvertsNumAdvertsOnMessagesPage = 3;
        public static int AdvertsNumAdvertsOnTopicPage = 3;

        // Reports
        public static int ReportsNumMaxNotResolvedIrregularity = 5;             //  0 <= num <= 500
        public static int ReportsNumMaxNotResolvedSpam = 10;                    //  0 <= num <= 500

        // Pages
        public static int PagesNumOfLinks = 5;                                  //  (5 <= num <= 20)  && (num % 2 != 0)

        // Company Types
        public static int CompanyTypesMaxDescriptionLenght = 500;               //  0 <= num <= 10000

        // Transact actions
        public static int ActionTransactionNumDaysActive = 3;                   // min 1

        // Take action from user
        public static int GetUserActionMaxNumberWarnings = 0;                   // min 0
        public static int GetUserActionMinNumberComments = 10;                  // min 0
        public static int GetUserActionTimeAfterWhichActionsCanBeTaken = 60;    // min 10 (in days)

        // Type suggestions
        public static int TypeSuggestionMaxActiveSuggestionsPerUser = 5;        // min 1
        public static int TypeSuggestionDaysAfterWhichSuggestionExpires = 7;    // min 0
        public static int TypeSuggestionMaxDaysToUpdateWhenAccepted = 2;        // min 0

        // Topics
        public static int TopicsPerPage = 30;            // min 1
        public static int TopicCommentsPerPage = 30;     // min 1
        public static int TopicsTimeWhichNeedsToPassToAddAnother = 15;     // min 0
        public static int TopicSubjectMinLength = 5;     // min 0
        public static int TopicSubjectMaxLength = 50;     //  


        /// <summary>
        /// Default User Interface theme.
        /// </summary>
        public static string DefaultUITheme = string.Empty;

        /// <summary>
        /// The prefix of the names of application variant connection strings.
        /// </summary>
        private static readonly string VariantDatabaseConnStrPrefix = "Entities_";

        /// <summary>
        /// Loads Configuration Keys from Web.Config and overwrites their values with the static fields
        /// </summary>
        private static void Load()
        {
            NameValueCollection appSettingsCollection = ConfigurationManager.AppSettings;
            if (appSettingsCollection != null)
            {
                // Load data - begin
                LoadGeneral(appSettingsCollection);
                LoadCache(appSettingsCollection);
                LoadSiteTexts(appSettingsCollection);
                LoadIpAttempts(appSettingsCollection);
                LoadOther(appSettingsCollection);
                LoadProducts(appSettingsCollection);
                LoadProductLinks(appSettingsCollection);
                LoadRatings(appSettingsCollection);
                LoadComments(appSettingsCollection);
                LoadImages(appSettingsCollection);
                LoadUsers(appSettingsCollection);
                LoadWarnings(appSettingsCollection);
                LoadRegistration(appSettingsCollection);
                LoadFields(appSettingsCollection);
                LoadHome(appSettingsCollection);
                LoadNotifies(appSettingsCollection);
                LoadCompany(appSettingsCollection);
                LoadCategory(appSettingsCollection);
                LoadMessages(appSettingsCollection);
                LoadSearch(appSettingsCollection);
                LoadSuggestions(appSettingsCollection);
                LoadAdverts(appSettingsCollection);
                LoadReports(appSettingsCollection);
                LoadPages(appSettingsCollection);
                LoadCompanyTypes(appSettingsCollection);
                LoadDefaultUITheme(appSettingsCollection);
                LoadTransactActions(appSettingsCollection);
                LoadGetUserActions(appSettingsCollection);
                LoadTypeSuggestions(appSettingsCollection);
                LoadTopics(appSettingsCollection);
                // Load data - end

                CheckLoadedData();
            }

        }

        /// <summary>
        /// Checks loaded data if its in correct boundaries , if no throws exceptions
        /// </summary>
        private static void CheckLoadedData()
        {
            // General
            if (string.IsNullOrEmpty(SiteName))
            {
                throw new BusinessException("SiteName is null or empty");
            }
            if (string.IsNullOrEmpty(SiteDomainAdress))
            {
                throw new BusinessException("SiteDomainAdress is null or empty");
            }
            if (MaximumNumberOfUsersRegisteredWithMail < 1 || MaximumNumberOfUsersRegisteredWithMail > 10)
            {
                throw new BusinessException(string.Format("Maximum number of users registered with mail is {0}", MaximumNumberOfUsersRegisteredWithMail));
            }

            // CACHE
            if (CacheDefaultExpireTimeInMinutes < 1)
            {
                throw new BusinessException("CacheDefaultExpireTimeInMinutes is < 1");
            }

            //Products
            if (ProductsMinProductNameLength < 1)
            {
                throw new BusinessException("ProductsMinProductNameLength is < 1");
            }
            if (ProductsMaxProductNameLength <= ProductsMinProductNameLength)
            {
                throw new BusinessException("ProductsMaxProductNameLength <= ProductsMinProductNameLength");
            }
            if (ProductsMaxProductNameLength > 200)
            {
                throw new BusinessException("ProductsMaxProductNameLength > 200");
            }
            if (ProductsMinCommentsOnPage < 1)
            {
                throw new BusinessException("ProductsMinCommentsOnPage < 1");
            }
            if (ProductsDefCommentsOnPage <= ProductsMinCommentsOnPage)
            {
                throw new BusinessException("ProductsDefCommentsOnPage <= ProductsMinCommentsOnPage");
            }
            if (ProductsMaxCommentsOnPage <= ProductsDefCommentsOnPage)
            {
                throw new BusinessException("ProductsMaxCommentsOnPage <= ProductsDefCommentsOnPage");
            }
            if (ProductsMaxCommentsOnPage > 500)
            {
                throw new BusinessException("ProductsMaxCommentsOnPage > 500");
            }
            if (ProductsMinTimeBetweenComments < 0)
            {
                throw new BusinessException("ProductsMinTimeBetweenComments < 0");
            }
            if (ProductsMaxNumberUserCommentsPerProduct < 1)
            {
                throw new BusinessException("ProductsMaxNumberUserCommentsPerProduct < 1");
            }
            if (ProductsMinCommentsAfterWhichTimeIsInvalid < 0)
            {
                throw new BusinessException("ProductsMinCommentsAfterWhichTimeIsInvalid < 0");
            }
            if (ProductsMinDescriptionLength < 0)
            {
                throw new BusinessException("ProductsMinDescriptionLength < 0");
            }
            if (ProductsMinDescriptionLength >= ProductsMaxDescriptionLength)
            {
                throw new BusinessException("ProductsMinDescriptionLength >= ProductsMaxDescriptionLength");
            }

            // Product links
            if (ProductLinksMinDescrLength >= ProductLinksMaxDescrLength)
            {
                throw new BusinessException("ProductLinksMinDescrLength >= ProductLinksMaxDescrLength");
            }

            //Comments
            if (CommentsMaxCommentsReplyLevel < 0)
            {
                throw new BusinessException("CommentsMaxCommentsReplyLevel < 0");
            }
            if (CommentsMaxCommentsReplyLevel > 20)
            {
                throw new BusinessException("CommentsMaxCommentsReplyLevel > 20");
            }
            if (CommentsMinCommentDescriptionLength < 1)
            {
                throw new BusinessException("CommentsMinCommentDescriptionLength < 1");
            }
            if (CommentsMaxCommentDescriptionLength < CommentsMinCommentDescriptionLength)
            {
                throw new BusinessException("CommentsMaxCommentDescriptionLength < CommentsMinCommentDescriptionLength");
            }
            if (CommentsMaxCommentDescriptionLength > 50000)
            {
                throw new BusinessException("CommentsMaxCommentDescriptionLength > 50000");
            }
            if (CommentsMaxWordLength < 10)
            {
                throw new BusinessException("CommentsMaxWordLength < 10");
            }
            if (CommentsMaxWordLength > 100)
            {
                throw new BusinessException("CommentsMaxWordLength > 100");
            }

            //Images

            if (ImagesThumbnailPictureWidth < 50 || ImagesThumbnailPrictureHeight < 50)
            {
                throw new BusinessException("ImagesThumbnailPictureWidth < 50 || ImagesThumbnailPrictureHeight < 50");
            }
            if (ImagesThumbnailPictureWidth > 500 || ImagesThumbnailPrictureHeight > 500)
            {
                throw new BusinessException("ImagesThumbnailPictureWidth > 500 || ImagesThumbnailPrictureHeight > 500");
            }
            if (ImagesMaxProductImagesCount < 0 || ImagesMaxCompanyImagesCount < 0)
            {
                throw new BusinessException("ImagesMaxProductImagesCount < 0 || ImagesMaxCompanyImagesCount < 0");
            }
            if (ImagesMaxCompanyImagesCount > 100 || ImagesMaxProductImagesCount > 100)
            {
                throw new BusinessException("ImagesMaxCompanyImagesCount > 100 || ImagesMaxProductImagesCount > 100");
            }
            if (ImagesMinPictureDescriptionLength < 0)
            {
                throw new BusinessException("ImagesMinPictureDescriptionLength < 0");
            }
            if (ImagesMaxPictureDescriptionLength < ImagesMinPictureDescriptionLength)
            {
                throw new BusinessException("ImagesMaxPictureDescriptionLength < ImagesMinPictureDescriptionLength");
            }
            if (ImagesMaxPictureDescriptionLength > 1000)
            {
                throw new BusinessException("ImagesMaxPictureDescriptionLength > 1000");
            }
            if (ImagesMinImageWidth < 100)
            {
                throw new BusinessException("ImagesMinImageWidth < 100");
            }
            if (ImagesMinImageHeight < 100)
            {
                throw new BusinessException("ImagesMinImageHeight < 100");
            }
            if (ImagesMainImageWidth < 200)
            {
                throw new BusinessException("ImagesMainImageWidth < 200");
            }


            // Nothing about ImagesPhysicalPathRoot yet


            //Users

            if (UsersMinUserNameLength < 2)
            {
                throw new BusinessException("UsersMinUserNameLength < 2");
            }
            if (UsersMaxUserNameLength < UsersMinUserNameLength)
            {
                throw new BusinessException("UsersMaxUserNameLength < UsersMinUserNameLength");
            }
            if (UsersMaxUserNameLength > 100)
            {
                throw new BusinessException("UsersMaxUserNameLength > 100");
            }
            if (UsersMinPasswordLength < 3)
            {
                throw new BusinessException("UsersMinPasswordLength < 3");
            }
            if (UsersMaxPasswordLength < UsersMinPasswordLength)
            {
                throw new BusinessException("UsersMaxPasswordLength < UsersMinPasswordLength");
            }
            if (UsersMaxPasswordLength > 100)
            {
                throw new BusinessException("UsersMaxPasswordLength > 100");
            }
            if (UsersMinSignatureLength < 1)
            {
                throw new BusinessException("UsersMinSignatureLength < 1");
            }
            if (UsersMaxSignatureLength < UsersMinSignatureLength)
            {
                throw new BusinessException("UsersMaxSignatureLength < UsersMinSignatureLength");
            }
            if (UsersMaxSignatureLength > 500)
            {
                throw new BusinessException("UsersMaxSignatureLength > 500");
            }
            if (UsersUserCommentsOnPage < 1)
            {
                throw new BusinessException("UsersUserCommentsOnPage < 1");
            }
            if (UsersUserCommentsOnPage > 200)
            {
                throw new BusinessException("UsersUserCommentsOnPage > 200");
            }

            //Fields

            if (FieldsMinDescriptionFieldLength < 1)
            {
                throw new BusinessException("FieldsMinDescriptionFieldLength < 1");
            }
            if (FieldsMaxDescriptionFieldLength < FieldsMinDescriptionFieldLength)
            {
                throw new BusinessException("FieldsMaxDescriptionFieldLength < FieldsMinDescriptionFieldLength");
            }
            if (FieldsMaxDescriptionFieldLength > 100000)
            {
                throw new BusinessException("FieldsMaxDescriptionFieldLength > 100000");
            }
            if (FieldsMinIdFieldLength < 4)
            {
                throw new BusinessException("FieldsMinIdFieldLength < 4");
            }
            if (FieldsMaxIdFieldLength < FieldsMinIdFieldLength)
            {
                throw new BusinessException("FieldsMaxIdFieldLength < FieldsMinIdFieldLength");
            }
            if (FieldsMaxIdFieldLength > 11)
            {
                throw new BusinessException("FieldsMaxIdFieldLength > 11");
            }
            if (FieldsDefMaxWordLength < 10)
            {
                throw new BusinessException("FieldsDefMaxWordLength < 10");
            }
            if (FieldsDefMaxWordLength > 100)
            {
                throw new BusinessException("FieldsDefMaxWordLength > 100");
            }

            // HOME PAGE
            if (HomePageNumberLastAddedProducts < 1 || HomePageNumberLastAddedProducts > 100)
            {
                throw new BusinessException("HomePageNumberLastAddedProducts < 1 or HomePageNumberLastAddedProducts > 100");
            }
            if (HomePageNumberNewsOnPage < 1 || HomePageNumberNewsOnPage > 20)
            {
                throw new BusinessException("HomePageNumberNewsOnPage < 1 or HomePageNumberNewsOnPage > 20");
            }


            // Notifies
            if (NotifiesMaxNumberNotifies < 1 || NotifiesMaxNumberNotifies > 1000)
            {
                throw new BusinessException("NotifiesMaxNumberNotifies < 1 or NotifiesMaxNumberNotifies > 1000");
            }

            //Company

            if (CompaniesMinCompanyNameLength < 2)
            {
                throw new BusinessException("CompaniesMinCompanyNameLength < 2");
            }
            if (CompaniesMaxCompanyNameLength < CompaniesMinCompanyNameLength)
            {
                throw new BusinessException("CompaniesMaxCompanyNameLength < CompaniesMinCompanyNameLength");
            }
            if (CompaniesMaxCompanyNameLength > 200)
            {
                throw new BusinessException("CompaniesMaxCompanyNameLength > 200");
            }
            if (CompaniesProductsOnPage < 1)
            {
                throw new BusinessException("CompaniesProductsOnPage < 1");
            }
            if (CompaniesProductsOnPage > 200)
            {
                throw new BusinessException("CompaniesProductsOnPage > 200");
            }
            if (string.IsNullOrEmpty(CompanyName))
            {
                throw new BusinessException("CompanyName is null or empty");
            }
            if (CompaniesMinDescriptionLength < 0)
            {
                throw new BusinessException("CompaniesMinDescriptionLength < 0");
            }
            if (CompaniesMaxDescriptionLength <= CompaniesMinDescriptionLength)
            {
                throw new BusinessException("CompaniesMaxDescriptionLength <= CompaniesMinDescriptionLength");
            }

            //Category

            if (CategoriesMaxCategoryNameLength < 1)
            {
                throw new BusinessException("CategoriesMaxCategoryNameLength < 1");
            }
            if (CategoriesMaxCategoryNameLength < CategoriesMinCategoryNameLength)
            {
                throw new BusinessException("CategoriesMaxCategoryNameLength < CategoriesMinCategoryNameLength");
            }
            if (CategoriesMaxCategoryNameLength > 200)
            {
                throw new BusinessException("CategoriesMaxCategoryNameLength > 200");
            }
            if (CategoriesMinCategoryDescriptionLength < 0)
            {
                throw new BusinessException("CategoriesMinCategoryDescriptionLength < 0");
            }
            if (CategoriesMaxCategoryDescriptionLength < CategoriesMinCategoryDescriptionLength)
            {
                throw new BusinessException("CategoriesMaxCategoryDescriptionLength < CategoriesMinCategoryDescriptionLength");
            }
            if (CategoriesMaxCategoryDescriptionLength > 100000)
            {
                throw new BusinessException("CategoriesMaxCategoryDescriptionLength > 100000");
            }
            if (CategoriesMinProdNumToShowLastProductsTbl < 1)
            {
                throw new BusinessException("CategoriesMinProdNumToShowLastProductsTbl < 1");
            }
            if (CategoriesMinProdNumToShowLastProductsTbl > 100)
            {
                throw new BusinessException("CategoriesMinProdNumToShowLastProductsTbl > 100");
            }
            if (CategoriesNumOfProductsToShowInLastProductsTbl < 1)
            {
                throw new BusinessException("CategoriesNumOfProductsToShowInLastProductsTbl < 1");
            }
            if (CategoriesNumOfProductsToShowInLastProductsTbl > 100)
            {
                throw new BusinessException("CategoriesNumOfProductsToShowInLastProductsTbl > 100");
            }
            if (CategoriesNumOfMostCommentedProducts < 1)
            {
                throw new BusinessException("ategoriesNumOfMostCommentedProducts < 1");
            }
            if (CategoriesNumOfMostCommentedProducts > 100)
            {
                throw new BusinessException("CategoriesNumOfMostCommentedProducts > 100");
            }
            if (CategoriesProdsPerPage > 100 || CategoriesProdsPerPage < 1)
            {
                throw new BusinessException("CategoriesProdsPerPage > 100 or CategoriesProdsPerPage < 1");
            }


            //Messages

            if (MessagesMinSubjectLength < 0)
            {
                throw new BusinessException("MessagesMinSubjectLength < 0");
            }
            if (MessagesMaxSubjectLength < MessagesMinSubjectLength)
            {
                throw new BusinessException("MessagesMaxSubjectLength < MessagesMinSubjectLength");
            }
            if (MessagesMaxSubjectLength > 100)
            {
                throw new BusinessException("MessagesMaxSubjectLength > 100");
            }

            //SiteText

            if (SiteTextsNumLastTextsToShow < 0)
            {
                throw new BusinessException("SiteTextsNumLastTextsToShow < 0");
            }
            if (SiteTextsNumLastTextsToShow > 200)
            {
                throw new BusinessException("SiteTextsNumLastTextsToShow > 200");
            }
            if (SiteTextsMinSiteTextDescr < 1)
            {
                throw new BusinessException("SiteTextsMinSiteTextDescr < 1");
            }
            if (SiteTextsMaxSiteTextDescr < SiteTextsMinSiteTextDescr)
            {
                throw new BusinessException("SiteTextsMaxSiteTextDescr < SiteTextsMinSiteTextDescr");
            }
            if (SiteTextsMaxSiteTextDescr > 100000)
            {
                throw new BusinessException("SiteTextsMaxSiteTextDescr > 100000");
            }
            if (SiteTextsMinSiteTextNameLength < 1)
            {
                throw new BusinessException("SiteTextsMinSiteTextNameLength < 1");
            }
            if (SiteTextsMaxSiteTextNameLength < SiteTextsMinSiteTextNameLength)
            {
                throw new BusinessException("SiteTextsMaxSiteTextNameLength < SiteTextsMinSiteTextNameLength");
            }
            if (SiteTextsMaxSiteTextNameLength > 200)
            {
                throw new BusinessException("SiteTextsMaxSiteTextNameLength > 200");
            }
            //Search

            if (SearchItemsPerSearchPage < 0)
            {
                throw new BusinessException("SearchItemsPerSearchPage < 0");
            }
            if (SearchItemsPerSearchPage > 200)
            {
                throw new BusinessException("SearchItemsPerSearchPage > 200");
            }
            if (SearchMaxSearchStringLength < 10)
            {
                throw new BusinessException("SearchMaxSearchStringLength < 10");
            }
            if (SearchMaxSearchStringLength > 500)
            {
                throw new BusinessException("SearchMaxSearchStringLength > 500");
            }

            //Suggestions

            if (SuggestionsPerPage < 0)
            {
                throw new BusinessException("SuggestionsPerPage < 0");
            }
            if (SuggestionsPerPage > 200)
            {
                throw new BusinessException("SuggestionsPerPage > 200");
            }
            if (SuggestionsMaxUserSuggestions < 0)
            {
                throw new BusinessException("SuggestionsMaxUserSuggestions < 0");
            }
            if (SuggestionsMaxUserSuggestions > 10)
            {
                throw new BusinessException("SuggestionsMaxUserSuggestions > 10");
            }



            //Adverts

            if (AdvertsNumAdvertsOnUserPage < 0 || AdvertsNumAdvertsOnUserPage > 50)
            {
                throw new BusinessException("AdvertsNumAdvertsOnUserPage < 0 || AdvertsNumAdvertsOnUserPage > 50");
            }
            if (AdvertsNumAdvertsOnCategoryPage < 0 || AdvertsNumAdvertsOnCategoryPage > 50)
            {
                throw new BusinessException("AdvertsNumAdvertsOnCategoryPage < 0 || AdvertsNumAdvertsOnCategoryPage > 50");
            }
            if (AdvertsNumAdvertsOnCompanyPage < 0 || AdvertsNumAdvertsOnCompanyPage > 50)
            {
                throw new BusinessException("AdvertsNumAdvertsOnCompanyPage < 0 || AdvertsNumAdvertsOnCompanyPage > 50");
            }
            if (AdvertsNumAdvertsOnProductPage < 0 || AdvertsNumAdvertsOnProductPage > 50)
            {
                throw new BusinessException("AdvertsNumAdvertsOnProductPage < 0 || AdvertsNumAdvertsOnProductPage > 50");
            }
            if (AdvertsNumAdvertsOnSearchPage < 0 || AdvertsNumAdvertsOnSearchPage > 50)
            {
                throw new BusinessException("AdvertsNumAdvertsOnSearchPage < 0 || AdvertsNumAdvertsOnSearchPage > 50");
            }
            if (AdvertsNumAdvertsOnSuggestionsPage < 0 || AdvertsNumAdvertsOnSuggestionsPage > 50)
            {
                throw new BusinessException("AdvertsNumAdvertsOnSuggestionsPage < 0 || AdvertsNumAdvertsOnSuggestionsPage > 50");
            }
            if (AdvertsNumAdvertsOnMessagesPage < 0 || AdvertsNumAdvertsOnMessagesPage > 50)
            {
                throw new BusinessException("AdvertsNumAdvertsOnMessagesPage < 0 || AdvertsNumAdvertsOnMessagesPage > 50");
            }




            // Reports

            if (ReportsNumMaxNotResolvedIrregularity < 0 || ReportsNumMaxNotResolvedIrregularity > 500)
            {
                throw new BusinessException("ReportsNumMaxNotResolvedIrregularity is < 0 or > 500");
            }
            if (ReportsNumMaxNotResolvedSpam < 0 || ReportsNumMaxNotResolvedSpam > 500)
            {
                throw new BusinessException("ReportsNumMaxNotResolvedSpam is < 0 or > 500");
            }

            // Pages

            if (PagesNumOfLinks < 5 || PagesNumOfLinks > 20 || (PagesNumOfLinks % 2 == 0))
            {
                throw new BusinessException("PagesNumOfLinks is < 5 or > 20 or (PagesNumOfLinks % 2 == 0)");
            }

            // Company Types

            if (CompanyTypesMaxDescriptionLenght < 0 || CompanyTypesMaxDescriptionLenght > 10000)
            {
                throw new BusinessException("CompanyTypesMaxDescriptionLenght is < 0 or > 10000");
            }

            // Topics

            if (TopicSubjectMinLength >= TopicSubjectMaxLength)
            {
                throw new BusinessException("TopicSubjectMinLength is >= TopicSubjectMaxLength");
            }

            // Default User Interface theme.
            if (DefaultUITheme == null)
            {
                // Do not throw an exception. Just make sure DefaultUITheme is not null.
                DefaultUITheme = string.Empty;
            }

        }

        private static void LoadCache(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["CacheDefaultExpireTimeInMinutes"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CacheDefaultExpireTimeInMinutes = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CacheDefaultExpireTimeInMinutes is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CacheDefaultExpireTimeInMinutes to int from Web.config");
                }
            }
        }

        private static void LoadHome(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            long tmp;

            settingValueStr = appSettingsCollection["HomePageNumberLastAddedProducts"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (long.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        HomePageNumberLastAddedProducts = tmp;
                    }
                    else
                    {
                        throw new BusinessException("HomePageNumberLastAddedProducts is < 1");
                    }
                }
            }
            else
            {
                throw new BusinessException("Couldn`t parse HomePageNumberLastAddedProducts to long from Web.config");
            }

            settingValueStr = appSettingsCollection["HomePageNumberNewsOnPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (long.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        HomePageNumberNewsOnPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("HomePageNumberNewsOnPage is < 1");
                    }
                }
            }
            else
            {
                throw new BusinessException("Couldn`t parse HomePageNumberNewsOnPage to long from Web.config");
            }

            settingValueStr = appSettingsCollection["HomePageNumberLastAddedCompanies"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (long.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        HomePageNumberLastAddedCompanies = tmp;
                    }
                    else
                    {
                        throw new BusinessException("HomePageNumberLastAddedCompanies is < 1");
                    }
                }
            }
            else
            {
                throw new BusinessException("Couldn`t parse HomePageNumberLastAddedCompanies to long from Web.config");
            }


        }

        private static void LoadNotifies(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["NotifiesMaxNumberNotifies"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        NotifiesMaxNumberNotifies = tmp;
                    }
                    else
                    {
                        throw new BusinessException("NotifiesMaxNumberNotifies is < 1");
                    }
                }
            }
            else
            {
                throw new BusinessException("Couldn`t parse NotifiesMaxNumberNotifies to long from Web.config");
            }
        }



        private static void LoadGeneral(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["SiteRunning"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                bool boolTmp = false;
                if (bool.TryParse(settingValueStr, out boolTmp) == true)
                {
                    SiteRunning = boolTmp;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse SiteRunning to boolean from Web.config");
                }
            }
            else
            {
                throw new BusinessException("Missing SiteRunning in Web.config");
            }


            settingValueStr = appSettingsCollection["GeneralCheckForAlreadyLoggedUserOnLogIn"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                bool boolTmp = false;
                if (bool.TryParse(settingValueStr, out boolTmp) == true)
                {
                    CheckForAlreadyLoggedUserOnLogIn = boolTmp;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse GeneralCheckForAlreadyLoggedUserOnLogIn to bool from Web.config");
                }
            }
            else
            {
                throw new BusinessException("GeneralCheckForAlreadyLoggedUserOnLogIn is empty");
            }

            settingValueStr = appSettingsCollection["CheckIfUserIsActivatedOnLogin"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                bool boolTmp = false;
                if (bool.TryParse(settingValueStr, out boolTmp) == true)
                {
                    CheckIfUserIsActivatedOnLogin = boolTmp;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CheckIfUserIsActivatedOnLogin to bool from Web.config");
                }
            }
            else
            {
                throw new BusinessException("CheckIfUserIsActivatedOnLogin is empty");
            }

            settingValueStr = appSettingsCollection["SendActivationCodeOnRegistering"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                bool boolTmp = false;
                if (bool.TryParse(settingValueStr, out boolTmp) == true)
                {
                    SendActivationCodeOnRegistering = boolTmp;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse SendActivationCodeOnRegistering to bool from Web.config");
                }
            }
            else
            {
                throw new BusinessException("SendActivationCodeOnRegistering is empty");
            }

            settingValueStr = appSettingsCollection["SiteSupportMail"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (settingValueStr.Length < 100)
                {
                    SiteSupportMail = settingValueStr;
                }
                else
                {
                    throw new BusinessException("SiteSupportMail length >= 100");
                }
            }
            else
            {
                throw new BusinessException("SiteSupportMail is empty");
            }

            settingValueStr = appSettingsCollection["SiteGeneralSectionMail"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (settingValueStr.Length < 100)
                {
                    SiteGeneralSectionMail = settingValueStr;
                }
                else
                {
                    throw new BusinessException("SiteGeneralSectionMail length >= 100");
                }
            }
            else
            {
                throw new BusinessException("SiteGeneralSectionMail is empty");
            }

            settingValueStr = appSettingsCollection["SiteSupportSectionMail"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (settingValueStr.Length < 100)
                {
                    SiteSupportSectionMail = settingValueStr;
                }
                else
                {
                    throw new BusinessException("SiteSupportSectionMail length >= 100");
                }
            }
            else
            {
                throw new BusinessException("SiteSupportSectionMail is empty");
            }

            settingValueStr = appSettingsCollection["SiteAdvertisementsSectionMail"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (settingValueStr.Length < 100)
                {
                    SiteAdvertisementsSectionMail = settingValueStr;
                }
                else
                {
                    throw new BusinessException("SiteAdvertisementsSectionMail length >= 100");
                }
            }
            else
            {
                throw new BusinessException("SiteAdvertisementsSectionMail is empty");
            }



            settingValueStr = appSettingsCollection["SiteDomainAdress"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (settingValueStr.Length < 100)
                {
                    SiteDomainAdress = settingValueStr;
                }
                else
                {
                    throw new BusinessException("SiteDomainAdress length >= 100");
                }
            }
            else
            {
                throw new BusinessException("SiteDomainAdress is empty");
            }

            settingValueStr = appSettingsCollection["SiteName"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (settingValueStr.Length < 100)
                {
                    SiteName = settingValueStr;
                }
                else
                {
                    throw new BusinessException("SiteName length >= 100");
                }
            }
            else
            {
                throw new BusinessException("SiteName is empty");
            }

            settingValueStr = appSettingsCollection["SendMailsViaSSL"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                bool boolTmp = false;
                if (bool.TryParse(settingValueStr, out boolTmp) == true)
                {
                    SendMailsViaSSL = boolTmp;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse SendMailsViaSSL to boolean from Web.config");
                }
            }
            else
            {
                throw new BusinessException("Missing SendMailsViaSSL in Web.config");
            }

            settingValueStr = appSettingsCollection["MaximumNumberOfUsersRegisteredWithMail"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        MaximumNumberOfUsersRegisteredWithMail = tmp;
                    }
                    else
                    {
                        throw new BusinessException("MaximumNumberOfUsersRegisteredWithMail is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse MaximumNumberOfUsersRegisteredWithMail to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("MaximumNumberOfUsersRegisteredWithMail is empty");
            }

            settingValueStr = appSettingsCollection["UseUrlRewriting"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                bool boolTmp = false;
                if (bool.TryParse(settingValueStr, out boolTmp) == true)
                {
                    UseUrlRewriting = boolTmp;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse UseUrlRewriting to boolean from Web.config");
                }
            }
            else
            {
                throw new BusinessException("Missing UseUrlRewriting in Web.config");
            }

            settingValueStr = appSettingsCollection["UseExternalUrlRewriteModule"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                bool boolTmp = false;
                if (bool.TryParse(settingValueStr, out boolTmp) == true)
                {
                    UseExternalUrlRewriteModule = boolTmp;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse UseExternalUrlRewriteModule to boolean from Web.config");
                }
            }
            else
            {
                throw new BusinessException("Missing UseExternalUrlRewriteModule in Web.config");
            }

            settingValueStr = appSettingsCollection["UrlRewritingDirectoryLevel"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                byte byteTmp = 0;
                if (byte.TryParse(settingValueStr, out byteTmp) == true)
                {
                    UrlRewritingDirectoryLevel = byteTmp;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse UrlRewritingDirectoryLevel to byte from Web.config");
                }
            }
            else
            {
                throw new BusinessException("Missing UrlRewritingDirectoryLevel in Web.config");
            }
        }

        private static void LoadAdverts(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["AdvertsCanAdvertBeUndeleted"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                bool boolTmp = false;
                if (bool.TryParse(settingValueStr, out boolTmp) == true)
                {
                    AdvertsCanAdvertBeUndeleted = boolTmp;
                }
                else
                {
                    throw new BusinessException("Couln`t parse AdvertsCanAdvertBeUndeleted to bool from web.config");
                }
            }
            else
            {
                throw new BusinessException("AdvertsCanAdvertBeUndeleted is empty");
            }

            settingValueStr = appSettingsCollection["AdvertsNumAdvertsOnCategoryPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        AdvertsNumAdvertsOnCategoryPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("AdvertsNumAdvertsOnCategoryPage is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse AdvertsNumAdvertsOnCategoryPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("AdvertsNumAdvertsOnCategoryPage is empty");
            }

            settingValueStr = appSettingsCollection["AdvertsNumAdvertsOnCompanyPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        AdvertsNumAdvertsOnCompanyPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("AdvertsNumAdvertsOnCompanyPage is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse AdvertsNumAdvertsOnCompanyPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("AdvertsNumAdvertsOnCompanyPage is empty");
            }

            settingValueStr = appSettingsCollection["AdvertsNumAdvertsOnProductPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        AdvertsNumAdvertsOnProductPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("AdvertsNumAdvertsOnProductPage is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse AdvertsNumAdvertsOnProductPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("AdvertsNumAdvertsOnProductPage is empty");
            }

            settingValueStr = appSettingsCollection["AdvertsNumAdvertsOnSearchPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        AdvertsNumAdvertsOnSearchPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("AdvertsNumAdvertsOnSearchPage is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse AdvertsNumAdvertsOnSearchPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("AdvertsNumAdvertsOnSearchPage is empty");
            }

            settingValueStr = appSettingsCollection["AdvertsNumAdvertsOnUserPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        AdvertsNumAdvertsOnUserPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("AdvertsNumAdvertsOnUserPage is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse AdvertsNumAdvertsOnUserPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("AdvertsNumAdvertsOnUserPage is empty");
            }

            settingValueStr = appSettingsCollection["AdvertsNumAdvertsOnSuggestionsPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        AdvertsNumAdvertsOnSuggestionsPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("AdvertsNumAdvertsOnSuggestionsPage is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse AdvertsNumAdvertsOnSuggestionsPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("AdvertsNumAdvertsOnSuggestionsPage is empty");
            }

            settingValueStr = appSettingsCollection["AdvertsNumAdvertsOnMessagesPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        AdvertsNumAdvertsOnMessagesPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("AdvertsNumAdvertsOnMessagesPage is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse AdvertsNumAdvertsOnMessagesPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("AdvertsNumAdvertsOnMessagesPage is empty");
            }

            settingValueStr = appSettingsCollection["AdvertsNumAdvertsOnTopicPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        AdvertsNumAdvertsOnTopicPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("AdvertsNumAdvertsOnTopicPage is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse AdvertsNumAdvertsOnTopicPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("AdvertsNumAdvertsOnTopicPage is empty");
            }




        }

        private static void LoadSuggestions(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["SuggestionsPerPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        SuggestionsPerPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("SuggestionsPerPage is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse SuggestionsPerPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("SuggestionsPerPage is empty");
            }

            settingValueStr = appSettingsCollection["SuggestionsMaxUserSuggestions"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        SuggestionsMaxUserSuggestions = tmp;
                    }
                    else
                    {
                        throw new BusinessException("SuggestionsMaxUserSuggestions is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse SuggestionsMaxUserSuggestions to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("SuggestionsMaxUserSuggestions is empty");
            }


        }

        private static void LoadTopics(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["TopicsPerPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        TopicsPerPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("TopicsPerPage is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse TopicsPerPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("TopicsPerPage is empty");
            }

            settingValueStr = appSettingsCollection["TopicCommentsPerPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        TopicCommentsPerPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("TopicCommentsPerPage is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse TopicCommentsPerPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("TopicCommentsPerPage is empty");
            }

            settingValueStr = appSettingsCollection["TopicsTimeWhichNeedsToPassToAddAnother"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        TopicsTimeWhichNeedsToPassToAddAnother = tmp;
                    }
                    else
                    {
                        throw new BusinessException("TopicsTimeWhichNeedsToPassToAddAnother is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse TopicsTimeWhichNeedsToPassToAddAnother to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("TopicsTimeWhichNeedsToPassToAddAnother is empty");
            }

            settingValueStr = appSettingsCollection["TopicSubjectMinLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        TopicSubjectMinLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("TopicSubjectMinLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse TopicSubjectMinLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("TopicSubjectMinLength is empty");
            }

            settingValueStr = appSettingsCollection["TopicSubjectMaxLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        TopicSubjectMaxLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("TopicSubjectMaxLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse TopicSubjectMaxLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("TopicSubjectMaxLength is empty");
            }






        }


        private static void LoadTypeSuggestions(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["TypeSuggestionMaxActiveSuggestionsPerUser"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        TypeSuggestionMaxActiveSuggestionsPerUser = tmp;
                    }
                    else
                    {
                        throw new BusinessException("TypeSuggestionMaxActiveSuggestionsPerUser is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse TypeSuggestionMaxActiveSuggestionsPerUser to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("TypeSuggestionMaxActiveSuggestionsPerUser is empty");
            }

            settingValueStr = appSettingsCollection["TypeSuggestionDaysAfterWhichSuggestionExpires"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        TypeSuggestionDaysAfterWhichSuggestionExpires = tmp;
                    }
                    else
                    {
                        throw new BusinessException("TypeSuggestionDaysAfterWhichSuggestionExpires is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse TypeSuggestionDaysAfterWhichSuggestionExpires to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("TypeSuggestionDaysAfterWhichSuggestionExpires is empty");
            }

            settingValueStr = appSettingsCollection["TypeSuggestionMaxDaysToUpdateWhenAccepted"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        TypeSuggestionMaxDaysToUpdateWhenAccepted = tmp;
                    }
                    else
                    {
                        throw new BusinessException("TypeSuggestionMaxDaysToUpdateWhenAccepted is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse TypeSuggestionMaxDaysToUpdateWhenAccepted to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("TypeSuggestionMaxDaysToUpdateWhenAccepted is empty");
            }



        }

        private static void LoadGetUserActions(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["GetUserActionMaxNumberWarnings"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        GetUserActionMaxNumberWarnings = tmp;
                    }
                    else
                    {
                        throw new BusinessException("GetUserActionMaxNumberWarnings is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse GetUserActionMaxNumberWarnings to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("GetUserActionMaxNumberWarnings is empty");
            }

            settingValueStr = appSettingsCollection["GetUserActionMinNumberComments"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        GetUserActionMinNumberComments = tmp;
                    }
                    else
                    {
                        throw new BusinessException("GetUserActionMinNumberComments is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse GetUserActionMinNumberComments to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("GetUserActionMinNumberComments is empty");
            }

            settingValueStr = appSettingsCollection["GetUserActionTimeAfterWhichActionsCanBeTaken"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 10)
                    {
                        GetUserActionTimeAfterWhichActionsCanBeTaken = tmp;
                    }
                    else
                    {
                        throw new BusinessException("GetUserActionTimeAfterWhichActionsCanBeTaken is < 10");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse GetUserActionTimeAfterWhichActionsCanBeTaken to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("GetUserActionTimeAfterWhichActionsCanBeTaken is empty");
            }


        }


        private static void LoadTransactActions(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["ActionTransactionNumDaysActive"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        ActionTransactionNumDaysActive = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ActionTransactionNumDaysActive is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse ActionTransactionNumDaysActive to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ActionTransactionNumDaysActive is empty");
            }
        }


        private static void LoadPages(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["PagesNumOfLinks"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        PagesNumOfLinks = tmp;
                    }
                    else
                    {
                        throw new BusinessException("PagesNumOfLinks is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse PagesNumOfLinks to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("PagesNumOfLinks is empty");
            }
        }

        private static void LoadCompanyTypes(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["CompanyTypesMaxDescriptionLenght"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        CompanyTypesMaxDescriptionLenght = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CompanyTypesMaxDescriptionLenght is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CompanyTypesMaxDescriptionLenght to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CompanyTypesMaxDescriptionLenght is empty");
            }
        }

        private static void LoadReports(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["ReportsNumMaxNotResolvedIrregularity"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ReportsNumMaxNotResolvedIrregularity = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ReportsNumMaxNotResolvedIrregularity is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse ReportsNumMaxNotResolvedIrregularity to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ReportsNumMaxNotResolvedIrregularity is empty");
            }

            settingValueStr = appSettingsCollection["ReportsNumMaxNotResolvedSpam"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ReportsNumMaxNotResolvedSpam = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ReportsNumMaxNotResolvedSpam is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse ReportsNumMaxNotResolvedSpam to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ReportsNumMaxNotResolvedSpam is empty");
            }
        }

        private static void LoadSearch(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["SearchItemsPerSearchPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        SearchItemsPerSearchPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("SearchItemsPerSearchPage is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse SearchItemsPerSearchPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("SearchItemsPerSearchPage is empty");
            }

            settingValueStr = appSettingsCollection["SearchMaxSearchStringLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 5)
                    {
                        SearchMaxSearchStringLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("SearchMaxSearchStringLength is < 6");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse SearchMaxSearchStringLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("SearchMaxSearchStringLength is empty");
            }

            settingValueStr = appSettingsCollection["SearchAlternativeItemsPerSearchPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        SearchAlternativeItemsPerSearchPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("SearchAlternativeItemsPerSearchPage is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse SearchAlternativeItemsPerSearchPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("SearchAlternativeItemsPerSearchPage is empty");
            }



        }

        private static void LoadSiteTexts(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["SiteTextsNumLastTextsToShow"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        SiteTextsNumLastTextsToShow = tmp;
                    }
                    else
                    {
                        throw new BusinessException("SiteTextsNumLastTextsToShow is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse SiteTextsNumLastTextsToShow to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("SiteTextsNumLastTextsToShow is empty");
            }

            settingValueStr = appSettingsCollection["SiteTextsMinSiteTextDescr"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        SiteTextsMinSiteTextDescr = tmp;
                    }
                    else
                    {
                        throw new BusinessException("SiteTextsMinSiteTextDescr is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse SiteTextsMinSiteTextDescr to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("SiteTextsMinSiteTextDescr is empty");
            }

            settingValueStr = appSettingsCollection["SiteTextsMaxSiteTextDescr"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > SiteTextsMinSiteTextDescr)
                    {
                        SiteTextsMaxSiteTextDescr = tmp;
                    }
                    else
                    {
                        throw new BusinessException("SiteTextsMaxSiteTextDescr is < SiteTextsMinSiteTextDescr");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse SiteTextsMaxSiteTextDescr to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("SiteTextsMaxSiteTextDescr is empty");
            }

            settingValueStr = appSettingsCollection["SiteTextsMinSiteTextNameLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        SiteTextsMinSiteTextNameLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("SiteTextsMinSiteTextNameLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse SiteTextsMinSiteTextNameLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("SiteTextsMinSiteTextNameLength is empty");
            }

            settingValueStr = appSettingsCollection["SiteTextsMaxSiteTextNameLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > SiteTextsMinSiteTextNameLength)
                    {
                        SiteTextsMaxSiteTextNameLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("SiteTextsMaxSiteTextNameLength is < SiteTextsMinSiteTextNameLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse SiteTextsMaxSiteTextNameLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("SiteTextsMaxSiteTextNameLength is empty");
            }
        }

        private static void LoadMessages(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["MessagesMinSubjectLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        MessagesMinSubjectLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("MessagesMinSubjectLength is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse MessagesMinSubjectLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("MessagesMinSubjectLength is empty");
            }

            settingValueStr = appSettingsCollection["MessagesMaxSubjectLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > MessagesMinSubjectLength)
                    {
                        MessagesMaxSubjectLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("MessagesMaxSubjectLength is < MessagesMinSubjectLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse MessagesMaxSubjectLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("MessagesMaxSubjectLength is empty");
            }
        }

        private static void LoadCategory(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["CategoriesMinCategoryNameLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CategoriesMinCategoryNameLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CategoriesMinCategoryNameLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CategoriesMinCategoryNameLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CategoriesMinCategoryNameLength is empty");
            }

            settingValueStr = appSettingsCollection["CategoriesMaxCategoryNameLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > CategoriesMinCategoryNameLength)
                    {
                        CategoriesMaxCategoryNameLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CategoriesMaxCategoryNameLength is < CategoriesMinCategoryNameLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CategoriesMaxCategoryNameLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CategoriesMaxCategoryNameLength is empty");
            }

            settingValueStr = appSettingsCollection["CategoriesMinCategoryDescriptionLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        CategoriesMinCategoryDescriptionLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CategoriesMinCategoryDescriptionLength is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CategoriesMinCategoryDescriptionLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CategoriesMinCategoryDescriptionLength is empty");
            }

            settingValueStr = appSettingsCollection["CategoriesMaxCategoryDescriptionLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > CategoriesMinCategoryDescriptionLength)
                    {
                        CategoriesMaxCategoryDescriptionLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CategoriesMaxCategoryDescriptionLength is < CategoriesMinCategoryDescriptionLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CategoriesMaxCategoryDescriptionLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CategoriesMaxCategoryDescriptionLength is empty");
            }

            settingValueStr = appSettingsCollection["CategoriesMinProdNumToShowLastProductsTbl"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CategoriesMinProdNumToShowLastProductsTbl = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CategoriesMinProdNumToShowLastProductsTbl is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CategoriesMinProdNumToShowLastProductsTbl to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CategoriesMinProdNumToShowLastProductsTbl is empty");
            }

            settingValueStr = appSettingsCollection["CategoriesNumOfProductsToShowInLastProductsTbl"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CategoriesNumOfProductsToShowInLastProductsTbl = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CategoriesNumOfProductsToShowInLastProductsTbl is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CategoriesNumOfProductsToShowInLastProductsTbl to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CategoriesNumOfProductsToShowInLastProductsTbl is empty");
            }

            settingValueStr = appSettingsCollection["CategoriesNumOfMostCommentedProducts"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CategoriesNumOfMostCommentedProducts = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CategoriesNumOfMostCommentedProducts is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CategoriesNumOfMostCommentedProducts to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CategoriesNumOfMostCommentedProducts is empty");
            }

            settingValueStr = appSettingsCollection["CategoriesProdsPerPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CategoriesProdsPerPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CategoriesProdsPerPage is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CategoriesProdsPerPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CategoriesProdsPerPage is empty");
            }


        }

        private static void LoadCompany(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;
            bool boolTmp = false;

            settingValueStr = appSettingsCollection["CompaniesMinCompanyNameLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CompaniesMinCompanyNameLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CompaniesMinCompanyNameLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CompaniesMinCompanyNameLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CompaniesMinCompanyNameLength is empty");
            }

            settingValueStr = appSettingsCollection["CompaniesMaxCompanyNameLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > CompaniesMinCompanyNameLength)
                    {
                        CompaniesMaxCompanyNameLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CompaniesMaxCompanyNameLength is < CompaniesMinCompanyNameLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CompaniesMaxCompanyNameLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CompaniesMaxCompanyNameLength is empty");
            }

            settingValueStr = appSettingsCollection["CompaniesProductsOnPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CompaniesProductsOnPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CompaniesProductsOnPage is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couln`t parse CompaniesProductsOnPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CompaniesProductsOnPage is empty");
            }

            settingValueStr = appSettingsCollection["CompaniesMinDescriptionLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp))
                {
                    if (tmp >= 0)
                    {
                        CompaniesMinDescriptionLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CompaniesMinDescriptionLength length is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CompaniesMinDescriptionLength to int.");
                }
            }
            else
            {
                throw new BusinessException("CompaniesMinDescriptionLength is empty");
            }

            settingValueStr = appSettingsCollection["CompaniesMaxDescriptionLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp))
                {
                    if (tmp >= 0)
                    {
                        CompaniesMaxDescriptionLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CompaniesMaxDescriptionLength length is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CompaniesMaxDescriptionLength to int.");
                }
            }
            else
            {
                throw new BusinessException("CompaniesMaxDescriptionLength is empty");
            }

            settingValueStr = appSettingsCollection["CompaniesCanUserTakeRoleIfNoEditors"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (bool.TryParse(settingValueStr, out boolTmp))
                {
                    CompaniesCanUserTakeRoleIfNoEditors = boolTmp;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CompaniesCanUserTakeRoleIfNoEditors to bool.");
                }
            }
            else
            {
                throw new BusinessException("CompaniesCanUserTakeRoleIfNoEditors is empty");
            }

            settingValueStr = appSettingsCollection["CompaniesTimeWhichNeedsToPassToAddAnother"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp))
                {
                    if (tmp >= 0)
                    {
                        CompaniesTimeWhichNeedsToPassToAddAnother = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CompaniesTimeWhichNeedsToPassToAddAnother length is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CompaniesTimeWhichNeedsToPassToAddAnother to int.");
                }
            }
            else
            {
                throw new BusinessException("CompaniesTimeWhichNeedsToPassToAddAnother is empty");
            }


            settingValueStr = appSettingsCollection["CompaniesMaxAlternativeNames"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp))
                {
                    if (tmp >= 1)
                    {
                        CompaniesMaxAlternativeNames = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CompaniesMaxAlternativeNames length is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CompaniesMaxAlternativeNames to int.");
                }
            }
            else
            {
                throw new BusinessException("CompaniesMaxAlternativeNames is empty");
            }



        }

        private static void LoadFields(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["FieldsMinDescriptionFieldLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        FieldsMinDescriptionFieldLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("FieldsMinDescriptionFieldLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse FieldsMinDescriptionFieldLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("FieldsMinDescriptionFieldLength is empty");
            }

            settingValueStr = appSettingsCollection["FieldsMaxDescriptionFieldLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > FieldsMinDescriptionFieldLength)
                    {
                        FieldsMaxDescriptionFieldLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("FieldsMaxDescriptionFieldLength is < FieldsMinDescriptionFieldLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse FieldsMaxDescriptionFieldLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("FieldsMaxDescriptionFieldLength is empty");
            }

            settingValueStr = appSettingsCollection["FieldsMinIdFieldLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        FieldsMinIdFieldLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("FieldsMinIdFieldLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse FieldsMinIdFieldLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("FieldsMinIdFieldLength is empty");
            }

            settingValueStr = appSettingsCollection["FieldsMaxIdFieldLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > FieldsMinIdFieldLength)
                    {
                        FieldsMaxIdFieldLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("FieldsMaxIdFieldLength is < FieldsMinIdFieldLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse FieldsMaxIdFieldLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("FieldsMaxIdFieldLength is empty");
            }

            settingValueStr = appSettingsCollection["FieldsDefMaxWordLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        FieldsDefMaxWordLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("FieldsDefMaxWordLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse FieldsDefMaxWordLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("FieldsDefMaxWordLength is empty");
            }
        }

        private static void LoadWarnings(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["WarningsNumberOnActionsOnWhichShouldRemoveAction"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        WarningsNumberOnActionsOnWhichShouldRemoveAction = tmp;
                    }
                    else
                    {
                        throw new BusinessException("WarningsNumberOnActionsOnWhichShouldRemoveAction is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse WarningsNumberOnActionsOnWhichShouldRemoveAction to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("WarningsNumberOnActionsOnWhichShouldRemoveAction is empty");
            }

            settingValueStr = appSettingsCollection["WarningsOnHowManyToDeleteUser"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        WarningsOnHowManyToDeleteUser = tmp;
                    }
                    else
                    {
                        throw new BusinessException("WarningsOnHowManyToDeleteUser is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse WarningsOnHowManyToDeleteUser to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("WarningsOnHowManyToDeleteUser is empty");
            }
        }

        private static void LoadRegistration(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["RegisterMaxNumberRegistrationsFromIp"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        RegisterMaxNumberRegistrationsFromIp = tmp;
                    }
                    else
                    {
                        throw new BusinessException("RegisterMaxNumberRegistrationsFromIp is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse RegisterMaxNumberRegistrationsFromIp to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("RegisterMaxNumberRegistrationsFromIp is empty");
            }
        }



        private static void LoadUsers(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["UsersMinUserNameLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        UsersMinUserNameLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("UsersMinUserNameLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse UsersMinUserNameLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("UsersMinUserNameLength is empty");
            }

            settingValueStr = appSettingsCollection["UsersMaxUserNameLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > UsersMinUserNameLength)
                    {
                        UsersMaxUserNameLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("UsersMaxUserNameLength is < UsersMinUserNameLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse UsersMaxUserNameLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("UsersMaxUserNameLength is empty");
            }

            settingValueStr = appSettingsCollection["UsersMinPasswordLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 3)
                    {
                        UsersMinPasswordLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("UsersMinPasswordLength is < 4");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse UsersMinPasswordLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("UsersMinPasswordLength is empty");
            }

            settingValueStr = appSettingsCollection["UsersMaxPasswordLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > UsersMinPasswordLength)
                    {
                        UsersMaxPasswordLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("UsersMaxPasswordLength is < UsersMinPasswordLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse UsersMaxPasswordLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("UsersMaxPasswordLength is empty");
            }

            settingValueStr = appSettingsCollection["UsersMinSignatureLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        UsersMinSignatureLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("UsersMinSignatureLength is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse UsersMinSignatureLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("UsersMinSignatureLength is empty");
            }

            settingValueStr = appSettingsCollection["UsersMaxSignatureLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > UsersMinSignatureLength)
                    {
                        UsersMaxSignatureLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("UsersMaxSignatureLength is < UsersMinSignatureLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse UsersMaxSignatureLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("UsersMaxSignatureLength is empty");
            }

            settingValueStr = appSettingsCollection["UsersUserCommentsOnPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        UsersUserCommentsOnPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("UsersUserCommentsOnPage is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse UsersUserCommentsOnPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("UsersUserCommentsOnPage is empty");
            }
        }

        private static void LoadImages(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["ImagesThumbnailPictureWidth"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 50 && tmp < 500)
                    {
                        ImagesThumbnailPictureWidth = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesThumbnailPictureWidth is < 50 or > 500");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesThumbnailPictureWidth to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesThumbnailPictureWidth is empty");
            }

            settingValueStr = appSettingsCollection["ImagesThumbnailPrictureHeight"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 50 && tmp < 500)
                    {
                        ImagesThumbnailPrictureHeight = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesThumbnailPrictureHeight is < 50 or > 500");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesThumbnailPrictureHeight to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesThumbnailPrictureHeight is empty");
            }

            settingValueStr = appSettingsCollection["ImagesMaxProductImagesCount"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        ImagesMaxProductImagesCount = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesMaxProductImagesCount is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesMaxProductImagesCount to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesMaxProductImagesCount is empty");
            }

            settingValueStr = appSettingsCollection["ImagesMaxCompanyImagesCount"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        ImagesMaxCompanyImagesCount = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesMaxCompanyImagesCount is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesMaxCompanyImagesCount to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesMaxCompanyImagesCount is empty");
            }

            settingValueStr = appSettingsCollection["ImagesMinPictureDescriptionLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ImagesMinPictureDescriptionLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesMinPictureDescriptionLength is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesMinPictureDescriptionLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesMinPictureDescriptionLength is empty");
            }

            settingValueStr = appSettingsCollection["ImagesMaxPictureDescriptionLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > ImagesMinPictureDescriptionLength)
                    {
                        ImagesMaxPictureDescriptionLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesMaxPictureDescriptionLength is < ImagesMinPictureDescriptionLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesMaxPictureDescriptionLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesMaxPictureDescriptionLength is empty");
            }

            settingValueStr = appSettingsCollection["ImagesMinImageWidth"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 100)
                    {
                        ImagesMinImageWidth = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesMinImageWidth is < 100");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesMinImageWidth to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesMinImageWidth is empty");
            }

            settingValueStr = appSettingsCollection["ImagesMinImageHeight"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 100)
                    {
                        ImagesMinImageHeight = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesMinImageHeight is < 100");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesMinImageHeight to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesMinImageHeight is empty");
            }

            settingValueStr = appSettingsCollection["ImagesMainImageWidth"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 200)
                    {
                        ImagesMainImageWidth = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesMainImageWidth is < 200");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesMainImageWidth to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesMainImageWidth is empty");
            }

            settingValueStr = appSettingsCollection["ImagesMainImageHeight"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 200)
                    {
                        ImagesMainImageHeight = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesMainImageHeight is < 200");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesMainImageHeight to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesMainImageHeight is empty");
            }

            settingValueStr = appSettingsCollection["ImagesCategoryMaxHeight"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 50)
                    {
                        ImagesCategoryMaxHeight = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesCategoryMaxHeight is < 50");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesCategoryMaxHeight to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesCategoryMaxHeight is empty");
            }

            settingValueStr = appSettingsCollection["ImagesCategoryMaxWidth"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 50)
                    {
                        ImagesCategoryMaxWidth = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesCategoryMaxWidth is < 50");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesCategoryMaxWidth to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesCategoryMaxWidth is empty");
            }

            settingValueStr = appSettingsCollection["ImagesMinCompLogoWidth"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 50)
                    {
                        ImagesMinCompLogoWidth = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesMinCompLogoWidth is < 50");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesMinCompLogoWidth to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesMinCompLogoWidth is empty");
            }

            settingValueStr = appSettingsCollection["ImagesMinCompLogoHeight"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 50)
                    {
                        ImagesMinCompLogoHeight = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ImagesMinCompLogoHeight is < 50");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ImagesMinCompLogoHeight to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ImagesMinCompLogoHeight is empty");
            }
        }

        private static void LoadRatings(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["CommRatingMaxUserRatingsPerProduct"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CommRatingMaxUserRatingsPerProduct = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CommRatingMaxUserRatingsPerProduct is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CommRatingMaxUserRatingsPerProduct to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CommRatingMaxUserRatingsPerProduct is empty");
            }

            settingValueStr = appSettingsCollection["CommRatingMaxUserRatingsForDay"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CommRatingMaxUserRatingsForDay = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CommRatingMaxUserRatingsForDay is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CommRatingMaxUserRatingsForDay to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CommRatingMaxUserRatingsForDay is empty");
            }

            settingValueStr = appSettingsCollection["CommRatingMaxUserRatingsPerTopic"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CommRatingMaxUserRatingsPerTopic = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CommRatingMaxUserRatingsPerTopic is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CommRatingMaxUserRatingsPerTopic to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CommRatingMaxUserRatingsPerTopic is empty");
            }



        }

        private static void LoadComments(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;


            settingValueStr = appSettingsCollection["CommentsMaxCommentsReplyLevel"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        CommentsMaxCommentsReplyLevel = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CommentsMaxCommentsReplyLevel is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CommentsMaxCommentsReplyLevel to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CommentsMaxCommentsReplyLevel is empty");
            }

            settingValueStr = appSettingsCollection["CommentsMinCommentDescriptionLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        CommentsMinCommentDescriptionLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CommentsMinCommentDescriptionLength is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CommentsMinCommentDescriptionLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CommentsMinCommentDescriptionLength is empty");
            }

            settingValueStr = appSettingsCollection["CommentsMaxCommentDescriptionLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > CommentsMinCommentDescriptionLength)
                    {
                        CommentsMaxCommentDescriptionLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CommentsMaxCommentDescriptionLength is < CommentsMinCommentDescriptionLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CommentsMaxCommentDescriptionLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CommentsMaxCommentDescriptionLength is empty");
            }

            settingValueStr = appSettingsCollection["CommentsMaxWordLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 10)
                    {
                        CommentsMaxWordLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("CommentsMaxWordLength is < 10");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse CommentsMaxWordLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("CommentsMaxWordLength is empty");
            }
        }

        private static void LoadIpAttempts(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["IpAttemptFailuresAfterWhichConsideredBot"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        IpAttemptFailuresAfterWhichConsideredBot = tmp;
                    }
                    else
                    {
                        throw new BusinessException("IpAttemptFailuresAfterWhichConsideredBot is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse IpAttemptFailuresAfterWhichConsideredBot to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("IpAttemptFailuresAfterWhichConsideredBot is empty");
            }

            settingValueStr = appSettingsCollection["IpAttemptMaxNumTries"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        IpAttemptMaxNumTries = tmp;
                    }
                    else
                    {
                        throw new BusinessException("IpAttemptMaxNumTries is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse IpAttemptMaxNumTries to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("IpAttemptMaxNumTries is empty");
            }


            settingValueStr = appSettingsCollection["IpAttemptMinTimeWhichNeedsToPassAfterPageLoad"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        IpAttemptMinTimeWhichNeedsToPassAfterPageLoad = tmp;
                    }
                    else
                    {
                        throw new BusinessException("IpAttemptMinTimeWhichNeedsToPassAfterPageLoad is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse IpAttemptMinTimeWhichNeedsToPassAfterPageLoad to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("IpAttemptMinTimeWhichNeedsToPassAfterPageLoad is empty");
            }


            settingValueStr = appSettingsCollection["IpAttemptTimeWhichNeedsToPassToResetTries"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        IpAttemptTimeWhichNeedsToPassToResetTries = tmp;
                    }
                    else
                    {
                        throw new BusinessException("IpAttemptTimeWhichNeedsToPassToResetTries is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse IpAttemptTimeWhichNeedsToPassToResetTries to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("IpAttemptTimeWhichNeedsToPassToResetTries is empty");
            }



        }

        private static void LoadOther(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["ProdCompMinAfterWhichUserCannotEditCrucialData"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ProdCompMinAfterWhichUserCannotEditCrucialData = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProdCompMinAfterWhichUserCannotEditCrucialData is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProdCompMinAfterWhichUserCannotEditCrucialData to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProdCompMinAfterWhichUserCannotEditCrucialData is empty");
            }
        }

        private static void LoadProducts(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;
            bool tmpBool = false;

            settingValueStr = appSettingsCollection["ProductsMinProductNameLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        ProductsMinProductNameLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMinProductNameLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMinProductNameLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMinProductNameLength is empty");
            }

            settingValueStr = appSettingsCollection["ProductsMaxProductNameLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > ProductsMinProductNameLength)
                    {
                        ProductsMaxProductNameLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMaxProductNameLength is < ProductsMinProductNameLength");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMaxProductNameLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMaxProductNameLength is empty");
            }

            settingValueStr = appSettingsCollection["ProductsMinCommentsOnPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        ProductsMinCommentsOnPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMinCommentsOnPage is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMinCommentsOnPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMinCommentsOnPage is empty");
            }

            settingValueStr = appSettingsCollection["ProductsDefCommentsOnPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > ProductsMinCommentsOnPage)
                    {
                        ProductsDefCommentsOnPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsDefCommentsOnPage is < ProductsMinCommentsOnPage");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsDefCommentsOnPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsDefCommentsOnPage is empty");
            }

            settingValueStr = appSettingsCollection["ProductsMaxCommentsOnPage"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > ProductsDefCommentsOnPage)
                    {
                        ProductsMaxCommentsOnPage = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMaxCommentsOnPage is < ProductsDefCommentsOnPage");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMaxCommentsOnPage to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMaxCommentsOnPage is empty");
            }

            ProductsCommentsOnPage = ProductsMinCommentsOnPage;

            settingValueStr = appSettingsCollection["ProductsMaxNumberUserCommentsPerProduct"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        ProductsMaxNumberUserCommentsPerProduct = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMaxNumberUserCommentsPerProduct is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMaxNumberUserCommentsPerProduct to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMaxNumberUserCommentsPerProduct is empty");
            }

            settingValueStr = appSettingsCollection["ProductsMinTimeBetweenComments"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ProductsMinTimeBetweenComments = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMinTimeBetweenComments is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMinTimeBetweenComments to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMinTimeBetweenComments is empty");
            }

            settingValueStr = appSettingsCollection["ProductsCheckForIpAdressIfUserIsLogged"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (bool.TryParse(settingValueStr, out tmpBool) == true)
                {
                    ProductsCheckForIpAdressIfUserIsLogged = tmpBool;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsCheckForIpAdressIfUserIsLogged to bool from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsCheckForIpAdressIfUserIsLogged is empty");
            }

            settingValueStr = appSettingsCollection["ProductsCheckForIpAdressIfUserNotLogged"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (bool.TryParse(settingValueStr, out tmpBool) == true)
                {
                    ProductsCheckForIpAdressIfUserNotLogged = tmpBool;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsCheckForIpAdressIfUserNotLogged to bool from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsCheckForIpAdressIfUserNotLogged is empty");
            }

            settingValueStr = appSettingsCollection["ProductsMinCommentsAfterWhichTimeIsInvalid"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ProductsMinCommentsAfterWhichTimeIsInvalid = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMinCommentsAfterWhichTimeIsInvalid is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMinCommentsAfterWhichTimeIsInvalid to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMinCommentsAfterWhichTimeIsInvalid is empty");
            }

            settingValueStr = appSettingsCollection["ProductsMinDescriptionLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ProductsMinDescriptionLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMinDescriptionLength is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMinDescriptionLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMinDescriptionLength is empty");
            }

            settingValueStr = appSettingsCollection["ProductsMaxDescriptionLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ProductsMaxDescriptionLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMaxDescriptionLength is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMaxDescriptionLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMaxDescriptionLength is empty");
            }

            settingValueStr = appSettingsCollection["ProductsMaxVariants"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 1)
                    {
                        ProductsMaxVariants = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMaxVariants is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMaxVariants to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMaxVariants is empty");
            }

            settingValueStr = appSettingsCollection["ProductsMaxVariantSubVariants"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 1)
                    {
                        ProductsMaxVariantSubVariants = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMaxVariantSubVariants is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMaxVariantSubVariants to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMaxVariantSubVariants is empty");
            }

            settingValueStr = appSettingsCollection["ProductsCanUserTakeRoleIfNoEditors"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (bool.TryParse(settingValueStr, out tmpBool))
                {
                    ProductsCanUserTakeRoleIfNoEditors = tmpBool;
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsCanUserTakeRoleIfNoEditors to bool.");
                }
            }
            else
            {
                throw new BusinessException("ProductsCanUserTakeRoleIfNoEditors is empty");
            }

            settingValueStr = appSettingsCollection["ProductsTimeWhichNeedsToPassToAddAnother"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ProductsTimeWhichNeedsToPassToAddAnother = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsTimeWhichNeedsToPassToAddAnother is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsTimeWhichNeedsToPassToAddAnother to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsTimeWhichNeedsToPassToAddAnother is empty");
            }

            settingValueStr = appSettingsCollection["ProductsMaxAlternativeNames"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 1)
                    {
                        ProductsMaxAlternativeNames = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductsMaxAlternativeNames is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductsMaxAlternativeNames to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductsMaxAlternativeNames is empty");
            }
        }

        private static void LoadProductLinks(NameValueCollection appSettingsCollection)
        {
            string settingValueStr;
            int tmp;

            settingValueStr = appSettingsCollection["ProductLinksMaxPerProduct"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        ProductLinksMaxPerProduct = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductLinksMaxPerProduct is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductLinksMaxPerProduct to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductLinksMaxPerProduct is empty");
            }

            settingValueStr = appSettingsCollection["ProductLinksMinDescrLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ProductLinksMinDescrLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductLinksMinDescrLength is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductLinksMinDescrLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductLinksMinDescrLength is empty");
            }

            settingValueStr = appSettingsCollection["ProductLinksMaxDescrLength"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp > 0)
                    {
                        ProductLinksMaxDescrLength = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductLinksMaxDescrLength is < 1");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductLinksMaxDescrLength to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductLinksMaxDescrLength is empty");
            }

            settingValueStr = appSettingsCollection["ProductLinksMinTimeBetweenAdding"];
            if (string.IsNullOrEmpty(settingValueStr) == false)
            {
                if (int.TryParse(settingValueStr, out tmp) == true)
                {
                    if (tmp >= 0)
                    {
                        ProductLinksMinTimeBetweenAdding = tmp;
                    }
                    else
                    {
                        throw new BusinessException("ProductLinksMinTimeBetweenAdding is < 0");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn`t parse ProductLinksMinTimeBetweenAdding to int from web.config");
                }
            }
            else
            {
                throw new BusinessException("ProductLinksMinTimeBetweenAdding is empty");
            }

        }


        private static void LoadDefaultUITheme(NameValueCollection appSettingsCollection)
        {
            // TODO: The correct place for the theme setting is in "/configuration/system.web/pages" node, "theme" attribute,
            // see "http://msdn.microsoft.com/en-us/library/0yy5hxdk(VS.80).aspx".
            string settingValueStr = appSettingsCollection["DefaultTheme"];

            DefaultUITheme = settingValueStr ?? string.Empty;
        }

        /// <summary>
        /// Gets the connection string for Entities ObjectContext dependent on the supplied culture (language) string.
        /// <para>If there is no supplied culture (language), a connection string for the English database is returned.</para>
        /// </summary>
        /// <param name="cultureStr">The identifier of the culture (language), e.g. "en", "bg".</param>
        /// <returns>The requested connection string. Does not return <c>null</c>.</returns>
        public static string GetEntitiesConnectionString(string cultureStr)
        {
            string connStr = GetEntitiesConnectionString(cultureStr, false);
            if (string.IsNullOrEmpty(connStr))
            {
                string errMsg =
                    string.Format("GetEntitiesConnectionString(string{0}{1}) returned an invalid result.", ", bool", "");
                throw new BusinessException(errMsg);
            }
            return connStr;
        }

        /// <summary>
        /// Gets the connection string for Entities ObjectContext dependent on the supplied culture (language) string.
        /// <para>If there is no supplied culture (language), and the <c>throwExceptionOnInvalidCultureStr</c> argument is
        /// <c>false</c>, a connection string for the English database is returned.</para>
        /// </summary>
        /// <param name="cultureStr">The identifier of the culture (language), e.g. "en", "bg".</param>
        /// <param name="throwExceptionOnInvalidCultureStr"><c>true</c> to throw an exception if the <c>cultureStr</c>
        /// argument is invalid; otherwise, <c>false</c>.</param>
        /// <returns>The requested connection string. Does not return <c>null</c>.</returns>
        public static string GetEntitiesConnectionString(string cultureStr, bool throwExceptionOnInvalidCultureStr)
        {
            string connStr = GetEntitiesConnectionString(cultureStr, throwExceptionOnInvalidCultureStr, DefaultApplicationVariant);
            if (string.IsNullOrEmpty(connStr))
            {
                string errMsg =
                    string.Format("GetEntitiesConnectionString(string{0}{1}) returned an invalid result.",
                    ", bool", ", string");
                throw new BusinessException(errMsg);
            }
            return connStr;
        }

        /// <summary>
        /// Gets the connection string for Entities ObjectContext dependent on the supplied culture (language) string.
        /// <para>If there is no supplied culture (language), and the <c>throwExceptionOnInvalidCultureStr</c> argument is
        /// <c>false</c>, a connection string for the English database is returned.</para>
        /// </summary>
        /// <param name="cultureStr">The identifier of the culture (language), e.g. "en", "bg".</param>
        /// <param name="throwExceptionOnInvalidCultureStr"><c>true</c> to throw an exception if the <c>cultureStr</c>
        /// argument is invalid; otherwise, <c>false</c>.</param>
        /// <param name="defaultCultureStr">The default culture (language) to use if the <c>cultureStr</c>
        /// argument is invalid.
        /// <para>If this argument is <c>null</c> or an empty string, "en" is used.</para></param>
        /// <returns>The requested connection string. Does not return <c>null</c>.</returns>
        public static string GetEntitiesConnectionString(string cultureStr, bool throwExceptionOnInvalidCultureStr,
            string defaultCultureStr)
        {
            string resultConnStr = null;

            // Try to get the connection string with the supplied cultureStr
            if (string.IsNullOrEmpty(cultureStr) == true)
            {
                if (throwExceptionOnInvalidCultureStr == true)
                {
                    throw new BusinessException("cultureStr is null or empty");
                }
            }
            else
            {
                resultConnStr = GetConnectionStringByCulture(cultureStr);
            }

            // Check whether connection string was found.
            // If not found, and if no exception on invalid culture string must be thrown,
            // try with the default culture
            if (string.IsNullOrEmpty(resultConnStr) == true)
            {
                if (throwExceptionOnInvalidCultureStr == true)
                {
                    string invalidCultureStrErrMsg =
                        string.Format("{0} (\"{1}\") is invalid. No connection string for that culture.", "cultureStr", cultureStr);
                    throw new BusinessException(invalidCultureStrErrMsg);
                }
                if (string.IsNullOrEmpty(defaultCultureStr) == true)
                {
                    defaultCultureStr = DefaultApplicationVariant;
                }
                resultConnStr = GetConnectionStringByCulture(defaultCultureStr);

                // If there is no connection string for the default culture either, throw an exception.
                if (string.IsNullOrEmpty(resultConnStr) == true)
                {
                    string invalidDefaultCultureStrErrMsg =
                        string.Format("{0} (\"{1}\") is invalid. No connection string for that culture.",
                        "defaultCultureStr", defaultCultureStr);
                    throw new BusinessException(invalidDefaultCultureStrErrMsg);
                }
            }
            if (string.IsNullOrEmpty(resultConnStr) == true)
            {
                throw new BusinessException("Could not get Entities connection string.");
            }
            return resultConnStr;
        }

        /// <summary>
        /// Gets the connection string for Entities ObjectContext that corresponds to the supplied culture string.
        /// </summary>
        /// <param name="cultureStrArg">The culture (e.g. "en", "bg").</param>
        /// <returns>The requested connection string or <c>null</c> if not found.</returns>
        private static string GetConnectionStringByCulture(string cultureStrArg)
        {
            if (cultureStrArg == null)
            {
                throw new ArgumentNullException(cultureStrArg);
            }
            if (cultureStrArg == string.Empty)
            {
                throw new ArgumentException("cultureStrArg" + " is empty.");
            }
            string resultConnStr = null;
            ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;
            string cultureStr = cultureStrArg.ToLower();
            string connStrName = VariantDatabaseConnStrPrefix + cultureStr;
            ConnectionStringSettings connStrSettings = connectionStrings[connStrName];
            if (connStrSettings != null)
            {
                resultConnStr = connStrSettings.ConnectionString;
            }
            return resultConnStr;
        }

        /// <summary>
        /// Gets a collection of the application variant connection strings.
        /// <para>If no such connection string is found and <c>currentIncluded</c> is <c>true</c>, 
        /// a <see cref="ConfigurationErrorsException"/> is thrown.</para>
        /// </summary>
        /// <param name="currentIncluded"><c>true</c> to include the connection string for the current application variant;
        /// otherwise, <c>false</c>.</param>
        /// <returns>The requested collection of the application variant connection strings.</returns>
        public static StringCollection ApplicationVariantConnectionStrings(bool currentIncluded)
        {
            StringCollection appVariantConnStrings = new StringCollection();
            ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;
            string currentAppVariant = Tools.ApplicationVariantString;
            foreach (ConnectionStringSettings connStrSettings in connectionStrings)
            {
                if (connStrSettings.Name.StartsWith(VariantDatabaseConnStrPrefix) == true)
                {
                    if (currentIncluded == false)
                    {
                        bool isCurrent =
                            ((connStrSettings.Name.Length == (VariantDatabaseConnStrPrefix.Length + currentAppVariant.Length)) &&
                            (connStrSettings.Name.EndsWith(currentAppVariant, StringComparison.InvariantCultureIgnoreCase) == true));
                        if (isCurrent == false)
                        {
                            appVariantConnStrings.Add(connStrSettings.ConnectionString);
                        }
                    }
                    else
                    {
                        appVariantConnStrings.Add(connStrSettings.ConnectionString);
                    }
                }
            }
            if ((currentIncluded == true) && (appVariantConnStrings.Count < 1))
            {
                throw new ConfigurationErrorsException("No application variant connection string configured.");
            }
            return appVariantConnStrings;
        }

        /// <summary>
        /// Gets a collection of the supported application variants.
        /// <para>If no application variant is supported, an exception is thrown.</para>
        /// </summary>
        public static StringCollection SupportedApplicationVariants
        {
            get
            {
                StringCollection appVariantStrings = new StringCollection();
                ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;
                foreach (ConnectionStringSettings connStrSettings in connectionStrings)
                {
                    if (connStrSettings.Name.StartsWith(VariantDatabaseConnStrPrefix) == true)
                    {
                        string appVariant = connStrSettings.Name.Substring(VariantDatabaseConnStrPrefix.Length);
                        if (appVariant.Length > 0)
                        {
                            // TODO: In case the application variant contains country/regioncode (i.e. "-US" in "en-US"),
                            //       consider not making it lowercase.
                            appVariantStrings.Add(appVariant.ToLower());
                        }
                    }
                }
                if (appVariantStrings.Count < 1)
                {
                    throw new ConfigurationErrorsException("No application variant connection string configured.");
                }
                return appVariantStrings;
            }
        }

        /// <summary>
        /// Check whether the supplied application variant is supported by the current configuration.
        /// </summary>
        /// <param name="applicationVariant">The application variant to be checked.</param>
        /// <returns>If supported - <c>true</c>; otherwise - <c>false</c>.</returns>
        public static bool IsSupportedApplicationVariant(string applicationVariant)
        {
            if (applicationVariant == null)
            {
                throw new ArgumentNullException("applicationVariant");
            }

            StringCollection supportedAppVariants = SupportedApplicationVariants;
            bool supported = false;

            for (int i = 0; (supported == false) && (i < supportedAppVariants.Count); i++)
            {
                string currentSupported = supportedAppVariants[i];
                if (applicationVariant == currentSupported)
                {
                    supported = true;
                }
            }
            return supported;
        }

        /// <summary>
        /// Check whether the supplied application variant is supported by the current configuration.
        /// <para>If not supported, an exception is thrown.</para>
        /// </summary>
        /// <param name="applicationVariant">The application variant to be checked.</param>
        public static void VerifyApplicationVariantSupported(string applicationVariant)
        {
            if (applicationVariant == null)
            {
                throw new ArgumentNullException("applicationVariant");
            }
            bool supported = IsSupportedApplicationVariant(applicationVariant);
            if (supported == false)
            {
                string msg = string.Format("The \"{0}\" application variant is not supported.", applicationVariant);
                throw new BusinessException(msg);
            }
        }
    }
}
