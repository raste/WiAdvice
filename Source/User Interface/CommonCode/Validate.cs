﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Linq;
using System.Web;

using DataAccess;
using BusinessLayer;

namespace UserInterface.CommonCode
{
    public class Validate
    {
        /// <summary>
        /// Checks if comment description is in correct format
        /// </summary>
        /// <returns>true if its correct , otherwise false</returns>
        public static bool ValidateComment(ref String text, out string error)
        {
            bool pass = false;
            error = "";

            pass = PrvValidateComment(ref text, out error
                , Configuration.CommentsMinCommentDescriptionLength, Configuration.CommentsMaxCommentDescriptionLength);

            return pass;
        }

        public static bool ValidateComment(ref String text, out string error, int minCommLength, int maxCommLength)
        {
            bool pass = false;
            error = "";

            pass = PrvValidateComment(ref text, out error, minCommLength, maxCommLength);

            return pass;
        }

        private static bool PrvValidateComment(ref String text, out string error, int minCommLength, int maxCommLength)
        {
            Boolean pass = false;
            error = "";

            CommonCode.UiTools.ChangeUiCultureFromSession();

            text = Tools.DeleteExtraWhitespaseAndNewLines(text, 5);

            if (string.IsNullOrEmpty(text))
            {
                if (minCommLength > 0)
                {
                    error = HttpContext.GetGlobalResourceObject("Validate", "errTypeText").ToString();
                    return false;
                }
                else
                {
                    return true;
                }
            }

            if (Tools.StringRangeValidatorPassed(minCommLength, maxCommLength, text))
            {
                if (Tools.IsTextWrapped(text, Configuration.CommentsMaxWordLength))
                {
                    pass = true;
                }
                else
                {
                    error = string.Format("{0} {1} {2}."
                        , HttpContext.GetGlobalResourceObject("Validate", "errWordsLength")
                        , Configuration.CommentsMaxWordLength
                        , HttpContext.GetGlobalResourceObject("Validate", "symbols"));
                }
            }
            else
            {
                error = string.Format("{0} {1}-{2} {3}.", HttpContext.GetGlobalResourceObject("Validate", "errTextLength")
                    , minCommLength, maxCommLength, HttpContext.GetGlobalResourceObject("Validate", "symbols"));
            }


            return pass;
        }

        /// <summary>
        /// Checks if email adress is in correct format
        /// </summary>
        /// <returns>true if its correct , otherwise false</returns>
        public static Boolean ValidateEmailAdress(String text, out string error)
        {
            Boolean pass = false;
            error = "";

            CommonCode.UiTools.ChangeUiCultureFromSession();

            if (Tools.NullValidatorPassed(text))
            {
                if (Tools.EmailValidatorPassed(text))
                {
                    pass = true;
                }
                else
                {
                    error = HttpContext.GetGlobalResourceObject("Validate", "errInvMail").ToString();
                }
            }
            else
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeMail").ToString();
            }

            return pass;
        }

        /// <summary>
        /// Checks if password is equal to hashed one
        /// </summary>
        public static Boolean ValidateCurrUserPassword(User currUser, string password, out string error)
        {
            Boolean pass = false;
            error = "";

            if (currUser == null)
            {
                throw new CommonCode.UIException("currUser is null");
            }

            CommonCode.UiTools.ChangeUiCultureFromSession();

            if (Tools.NullValidatorPassed(password))
            {
                BusinessUser businessUser = new BusinessUser();
                if (businessUser.CheckPassword(password, currUser.password))
                {
                    pass = true;
                }
                else
                {
                    error = HttpContext.GetGlobalResourceObject("Validate", "errIncCurrPass").ToString();
                }
            }
            else
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeCurrPass").ToString();
            }

            return pass;
        }


        /// <summary>
        /// Checks if variant/subvariant name is in correct format and if its unique
        /// </summary>
        /// <param name="variantID">when checking for subvariant type variantID otherwise 0</param>
        public static Boolean ValidateVariantName(Entities objectContext, Product currProduct, ref string name, int minLength,
            int maxLength, out string error, long variantID)
        {
            if (minLength < 1)
            {
                throw new UIException("minLength < 1");
            }
            if (minLength >= maxLength)
            {
                throw new UIException("minLength >= maxLength");
            }
            if (variantID < 0)
            {
                throw new UIException("variantID < 0");
            }

            CommonCode.UiTools.ChangeUiCultureFromSession();

            error = "";
            Boolean pass = false;

            name = Tools.RemoveSpacesAtEndOfString(name);

            if (Tools.NullValidatorPassed(name))
            {
                if (Tools.StringRangeValidatorPassed(minLength, maxLength, name))
                {
                    if (ValidateNamesForSpacesFormat(ref name, out error))
                    {
                        if (BusinessProductVariant.UniqueVariantName(objectContext, currProduct, name, variantID))
                        {
                            pass = true;
                        }
                        else
                        {
                            if (variantID > 0)
                            {
                                error = HttpContext.GetGlobalResourceObject("Validate", "errDuplicateSubVariantName").ToString();
                            }
                            else
                            {
                                error = HttpContext.GetGlobalResourceObject("Validate", "errDuplicateVariantName").ToString();
                            }
                        }
                    }
                }
                else
                {
                    error = string.Format("{0} {1}-{2} {3}!", HttpContext.GetGlobalResourceObject("Validate", "errNameLength")
                        , minLength, maxLength, HttpContext.GetGlobalResourceObject("Validate", "symbols"));
                }
            }
            else
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeName").ToString();
            }

            return pass;
        }

        /// <summary>
        /// Checks if description string is in correct format , if not return error
        /// </summary>
        /// <param name="type">subject, name, description, signature  (if something else..need to be added to the code in method)</param>
        /// <returns>true if its correct , otherwise false</returns>
        public static Boolean ValidateDescription(int minLength, int maxLength, ref String text, string type, out string error, int maxWordLength)
        {
            text = Tools.DeleteExtraWhitespaseAndNewLines(text, 5);

            error = string.Empty;

            bool pass = prvValidateDescription(minLength, maxLength, text, type, out error, maxWordLength);

            return pass;
        }

        /// <summary>
        /// Checks if description string is in correct format , if not return error.
        /// IF CHECKS FOR REPEATING NEW LINES OR SPACES IS NEEDED, CALL THE OTHER validation function
        /// </summary>
        /// <param name="type">subject, name, description, signature  (if something else..need to be added to the code in method)</param>
        /// <returns>true if its correct , otherwise false</returns>
        public static Boolean ValidateDescription(int minLength, int maxLength, String text, string type, out string error, int maxWordLength)
        {
            error = string.Empty;

            bool pass = prvValidateDescription(minLength, maxLength, text, type, out error, maxWordLength);

            return pass;
        }

        private static bool prvValidateDescription(int minLength, int maxLength, String text, string type, out string error, int maxWordLength)
        {

            if (minLength < 0)
            {
                throw new UIException("minLength < 0");
            }
            if (minLength >= maxLength)
            {
                throw new UIException("minLength >= maxLength");
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new UIException("type is null or empty");
            }
            if (maxWordLength < 1)
            {
                throw new UIException("maxWordLength < 1");
            }

            string localWordForLengthError = string.Empty;
            string localWordForTypeItError = string.Empty;

            switch (type)
            {
                case "subject":

                    localWordForLengthError = HttpContext.GetGlobalResourceObject("Validate", "errLocalWordForLengthSubject").ToString();
                    localWordForTypeItError = HttpContext.GetGlobalResourceObject("Validate", "errLocalWordForTypeItSubject").ToString();

                    break;
                case "name":

                    localWordForLengthError = HttpContext.GetGlobalResourceObject("Validate", "errLocalWordForLengthName").ToString();
                    localWordForTypeItError = HttpContext.GetGlobalResourceObject("Validate", "errLocalWordForTypeItName").ToString();

                    break;
                case "description":

                    localWordForLengthError = HttpContext.GetGlobalResourceObject("Validate", "errLocalWordForLengthDescription").ToString();
                    localWordForTypeItError = HttpContext.GetGlobalResourceObject("Validate", "errLocalWordForTypeItDescription").ToString();

                    break;
                case "signature":

                    localWordForLengthError = HttpContext.GetGlobalResourceObject("Validate", "errLocalWordForLengthSignature").ToString();
                    localWordForTypeItError = HttpContext.GetGlobalResourceObject("Validate", "errLocalWordForTypeItSignature").ToString();

                    break;
                default:
                    throw new UIException(string.Format("Type = {0} is not supported type for ValidateDescription", type));
            }

            Boolean pass = false;
            error = "";

            CommonCode.UiTools.ChangeUiCultureFromSession();

            if ((minLength == 0) || Tools.NullValidatorPassed(text))
            {
                if (Tools.StringRangeValidatorPassed(minLength, maxLength, text))
                {
                    if (Tools.IsTextWrapped(text, maxWordLength))
                    {
                        pass = true;
                    }
                    else
                    {
                        error = string.Format("{0} {1}."
                            , HttpContext.GetGlobalResourceObject("Validate", "errWordsLength")
                            , maxWordLength);
                    }
                }
                else
                {
                    error = string.Format("{0} {1}-{2}.", localWordForLengthError, minLength, maxLength);
                }
            }
            else
            {
                error = localWordForTypeItError;
            }

            return pass;
        }

        /// <summary>
        /// For validating secret questions or answers
        /// </summary>
        public static Boolean ValidateSecretQnA(int minLength, int maxLength, String text)
        {
            if (minLength < 0)
            {
                throw new UIException("minLength < 0");
            }
            if (minLength >= maxLength)
            {
                throw new UIException("minLength >= maxLength");
            }

            Boolean pass = false;

            if ((minLength == 0) || Tools.NullValidatorPassed(text))
            {
                if (Tools.StringRangeValidatorPassed(minLength, maxLength, text))
                {
                    if (!text.StartsWith(" ") && !text.EndsWith(" "))
                    {
                        if (Tools.IsTextWrapped(text, Configuration.FieldsDefMaxWordLength))
                        {
                            pass = true;
                        }
                    }
                }
            }

            return pass;
        }

        /// <summary>
        /// Checks if description string is in correct format without giving length params
        /// </summary>
        /// <returns>true if its correct , otherwise false</returns>
        public static Boolean ValidateDescription(ref String text, out string error)
        {
            int minLength = Configuration.FieldsMinDescriptionFieldLength;
            int maxLength = Configuration.FieldsMaxDescriptionFieldLength;

            CommonCode.UiTools.ChangeUiCultureFromSession();

            if (minLength < 0)
            {
                throw new UIException("minLength < 0");
            }
            if (minLength >= maxLength)
            {
                throw new UIException("minLength >= maxLength");
            }

            Boolean pass = false;
            error = "";

            text = Tools.DeleteExtraWhitespaseAndNewLines(text, 5);

            if ((minLength == 0) || Tools.NullValidatorPassed(text))
            {
                if (Tools.StringRangeValidatorPassed(minLength, maxLength, text))
                {
                    if (Tools.IsTextWrapped(text, Configuration.FieldsDefMaxWordLength))
                    {
                        pass = true;
                    }
                    else
                    {
                        error = string.Format("{0} {1}.", HttpContext.GetGlobalResourceObject("Validate", "errWordsLength")
                            , Configuration.FieldsDefMaxWordLength);
                    }
                }
                else
                {
                    error = string.Format("{0} {1}-{2} {3}!", HttpContext.GetGlobalResourceObject("Validate", "errDescrLength")
                        , minLength, maxLength, HttpContext.GetGlobalResourceObject("Validate", "symbols"));
                }
            }
            else
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeDescr").ToString();
            }
            return pass;
        }

        /// <summary>
        /// Checks if Name format is correct , if not returns error
        /// </summary>
        /// <returns>true if its correct , otherwise false</returns>
        public static Boolean ValidateUserNameFormat(ref String text, out string error, bool checkForChars)
        {
            Boolean pass = true;
            error = "";

            CommonCode.UiTools.ChangeUiCultureFromSession();

            text = Tools.RemoveSpacesAtEndOfString(text);

            if (Tools.NullValidatorPassed(text))
            {
                if (Tools.StringRangeValidatorPassed(Configuration.UsersMinUserNameLength,
                    Configuration.UsersMaxUserNameLength, text))
                {

                    if (checkForChars == true)
                    {
                        char[] textChars = text.ToLowerInvariant().ToCharArray();
                        if (textChars[0] == ' ' || textChars[textChars.Length - 1] == ' ')
                        {
                            pass = false;
                        }
                        else
                        {
                            char[] alphchars = Tools.GetEnglishChars(true);
                            int spaces = 0;

                            for (int i = 0; i < textChars.Length; i++)
                            {
                                if ((alphchars.Contains(textChars[i]) || textChars[i] == ' ') && spaces <= 3)
                                {
                                    if (textChars[i] == ' ')
                                    {
                                        if (textChars[i - 1] == ' ' || textChars[i + 1] == ' ')
                                        {
                                            pass = false;
                                            break;
                                        }
                                        else
                                        {
                                            spaces++;
                                        }
                                    }
                                }
                                else
                                {
                                    pass = false;
                                    break;
                                }
                            }

                        }

                        if (pass == false)
                        {
                            error = HttpContext.GetGlobalResourceObject("Validate", "errIncNameFormat").ToString();
                        }
                    }
                }
                else
                {
                    pass = false;
                    error = string.Format("{0} {1}-{2} {3}.", HttpContext.GetGlobalResourceObject("Validate", "errNameLength")
                        , Configuration.UsersMinUserNameLength, Configuration.UsersMaxUserNameLength
                        , HttpContext.GetGlobalResourceObject("Validate", "characters"));
                }
            }
            else
            {
                pass = false;
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeName").ToString();
            }

            return pass;
        }

        /// <summary>
        /// Checks if name/text start/ends with spaces and if there are 2spaces together , returns true if theese sircumstances dont occur
        /// , otherwise false
        /// </summary>
        public static Boolean ValidateNamesForSpacesFormat(ref String text, out string error)
        {
            text = Tools.RemoveSpacesAtEndOfString(text);

            Boolean pass = true;
            error = "";

            CommonCode.UiTools.ChangeUiCultureFromSession();

            if (Tools.NullValidatorPassed(text))
            {
                char[] textChars = text.ToCharArray();
                if (textChars[0] == ' ' || textChars[textChars.Length - 1] == ' ')
                {
                    pass = false;
                }
                else
                {
                    for (int i = 0; i < textChars.Length; i++)
                    {
                        if (textChars[i] == ' ')
                        {
                            if (textChars[i - 1] == ' ' || textChars[i + 1] == ' ')
                            {
                                pass = false;
                                break;
                            }
                        }
                    }

                }

                if (pass == false)
                {
                    error = HttpContext.GetGlobalResourceObject("Validate", "errIncNameFormat").ToString();
                }
            }
            else
            {
                pass = false;
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeName").ToString();
            }

            return pass;
        }

        /// <summary>
        /// Checks if string can be parsed to long , and if that long is > 0 , if not returns error
        /// </summary>
        /// <returns>true if it can be parsed , otherwise false</returns>
        public static Boolean ValidateLong(String text, out string error)
        {
            Boolean pass = false;
            error = "";

            CommonCode.UiTools.ChangeUiCultureFromSession();

            if (Tools.NullValidatorPassed(text))
            {
                if (Tools.StringRangeValidatorPassed(1, Configuration.FieldsMaxIdFieldLength, text))
                {
                    if (Tools.TypeValidatorPassed("long", text))
                    {
                        long test = -1;
                        if (long.TryParse(text, out test))
                        {
                            if (test > 0)
                            {
                                pass = true;
                            }
                            else
                            {
                                error = HttpContext.GetGlobalResourceObject("Validate", "errParseToLong").ToString();
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format
                                ("Couldnt parse text = {0} to long , there is check in same function for that before parsing.", text));
                        }
                    }
                    else
                    {
                        error = HttpContext.GetGlobalResourceObject("Validate", "errTypeNumber").ToString();
                    }
                }
                else
                {
                    error = string.Format("{0} {1}-{2}.", HttpContext.GetGlobalResourceObject("Validate", "errIdLength"),
                        Configuration.FieldsMinIdFieldLength, Configuration.FieldsMaxIdFieldLength);
                }
            }
            else
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeNumber").ToString();
            }

            return pass;
        }



        /// <summary>
        /// Checks if HTML is in xml format
        /// </summary>
        /// <returns>true if it is in XML format , otherwise false</returns>
        public static Boolean ValidateHtml(string decodedHtml)
        {
            Boolean passed = true;

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(decodedHtml);  // must be decoded
            }
            catch (System.Xml.XmlException)
            {
                passed = false;
            }

            return passed;
        }


        public static Boolean ValidateMessageToUser(BusinessUser businessUser, BusinessMessages businessMessages,
           User currUser, String username, out string error)
        {
            Boolean passed = false;
            error = "";

            CommonCode.UiTools.ChangeUiCultureFromSession();

            if (Tools.NullValidatorPassed(username))
            {
                EntitiesUsers userContext = new EntitiesUsers();

                User ToUser = businessUser.GetByName(userContext, username, false, true);
                if (ToUser != null)
                {
                    if (currUser.ID != ToUser.ID)
                    {
                        if (businessMessages.CanUserSendMessageTo(userContext, currUser.ID, ToUser.ID))
                        {
                            passed = true;
                        }
                        else
                        {
                            error = HttpContext.GetGlobalResourceObject("Validate", "errCantSendMsgToUser").ToString();
                        }
                    }
                    else
                    {
                        error = HttpContext.GetGlobalResourceObject("Validate", "errMsgCantSendToSelf").ToString();
                    }
                }
                else
                {
                    error = HttpContext.GetGlobalResourceObject("Validate", "errNoSuchUser").ToString();
                }
            }
            else
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeUserToSendMsg").ToString();
            }

            return passed;
        }

        /// <summary>
        /// Checks if name is taken and if its in correct format in table
        /// </summary>
        /// <param name="table">users,categories,companies,products,compChar,prodChar,siteText,companyType</param>
        /// <param name="parentID">only for prodChar(product id) or compChar(company id) or category( parent category ID) ..else 0</param>
        /// <returns>true if name is not taken , otherwise false</returns>
        public static Boolean ValidateName(Entities objectContext, string table, ref String text, int minLength,
            int maxLength, out string error, long parentID)
        {
            if (minLength < 1)
            {
                throw new UIException("minLength < 1");
            }
            if (minLength >= maxLength)
            {
                throw new UIException("minLength >= maxLength");
            }
            if (parentID < 0)
            {
                throw new UIException("parentID < 0");
            }

            CommonCode.UiTools.ChangeUiCultureFromSession();

            error = "";
            Boolean pass = false;

            text = Tools.RemoveSpacesAtEndOfString(text);

            if (Tools.NullValidatorPassed(text))
            {
                if (Tools.StringRangeValidatorPassed(minLength, maxLength, text))
                {
                    if (ValidateNamesForSpacesFormat(ref text, out error))
                    {
                        if (Tools.NameValidatorPassed(objectContext, table, text, parentID))
                        {
                            pass = true;
                        }
                        else
                        {

                            switch (table)
                            {
                                case "categories":
                                    error = "There is category with same name in the group!";
                                    break;
                                case "products":
                                    error = HttpContext.GetGlobalResourceObject("Validate", "errProdNameTaken").ToString();
                                    break;
                                default:
                                    error = HttpContext.GetGlobalResourceObject("Validate", "errNameTaken").ToString();
                                    break;
                            }

                        }
                    }
                }
                else
                {
                    error = string.Format("{0} {1}-{2}!", HttpContext.GetGlobalResourceObject("Validate", "errNameLength")
                        , minLength, maxLength);
                }
            }
            else
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeName").ToString();
            }

            return pass;
        }


        /// <summary>
        /// Checks if password is in correct format , if its not returns error
        /// </summary>
        /// <returns>true if its correct , otherwise false</returns>
        public static Boolean ValidatePassword(String text, out string error)
        {
            Boolean pass = false;
            error = "";

            CommonCode.UiTools.ChangeUiCultureFromSession();

            if (Tools.NullValidatorPassed(text))
            {
                if (Tools.StringRangeValidatorPassed(Configuration.UsersMinPasswordLength, Configuration.UsersMaxPasswordLength, text))
                {
                    if (Tools.SymbolsValidatorPassed(text))
                    {
                        pass = true;
                    }
                    else
                    {
                        error = HttpContext.GetGlobalResourceObject("Validate", "errNewUserPass").ToString();
                    }
                }
                else
                {
                    error = string.Format("{0} {1}-{2}.", HttpContext.GetGlobalResourceObject("Validate", "errPassLength")
                        , Configuration.UsersMinPasswordLength, Configuration.UsersMaxPasswordLength);
                }
            }
            else
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypePass").ToString();
            }

            return pass;
        }

        /// <summary>
        /// Checks if two strings are equal , if not returns error
        /// </summary>
        /// <returns>true if strings are equal, otherwise false</returns>
        public static Boolean ValidateRepeatPassword(string username, String pass1, String pass2, out string error)
        {
            CommonCode.UiTools.ChangeUiCultureFromSession();

            Boolean pass = false;
            error = "";

            if (!Tools.NullValidatorPassed(pass1))
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypePassForNewUser").ToString();
                return pass;
            }

            if (Tools.NullValidatorPassed(pass2))
            {
                if (Tools.MatchFieldsValidatorPassed(pass1, pass2))
                {
                    if (pass1.Contains(username) == false)
                    {
                        pass = true;
                    }
                    else
                    {
                        error = HttpContext.GetGlobalResourceObject("Validate", "errPassCannotContainUsername").ToString();
                    }
                }
                else
                {
                    error = HttpContext.GetGlobalResourceObject("Validate", "errPassFieldsDoNotMatch").ToString();
                }
            }
            else
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeRepPass").ToString();
            }

            return pass;
        }


        /// <summary>
        /// Validates website adress
        /// </summary>
        public static Boolean ValidateSiteAdress(string text, out string error, Boolean withNullAsOption)
        {
            Boolean passed = false;
            error = string.Empty;
            CommonCode.UiTools.ChangeUiCultureFromSession();

            if (withNullAsOption)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    if (text.Length > 500)
                    {
                        error = HttpContext.GetGlobalResourceObject("Validate", "errSiteLength").ToString();
                    }
                    else
                    {
                        passed = Tools.WebSiteValidatorPassed(text);
                    }
                }
                else
                {
                    passed = true;
                }
            }
            else
            {
                if (Tools.NullValidatorPassed(text))
                {
                    if (text.Length > 300)
                    {
                        error = error = HttpContext.GetGlobalResourceObject("Validate", "errSiteLength").ToString();
                    }
                    else
                    {
                        passed = Tools.WebSiteValidatorPassed(text);
                    }
                }
                else
                {
                    error = HttpContext.GetGlobalResourceObject("Validate", "errTypeSite").ToString();
                }
            }

            if (passed == false && string.IsNullOrEmpty(error))
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errInvSiteFormat").ToString();
            }

            return passed;
        }

        /// <summary>
        /// Checks if Name is in correct format and if its not taken
        /// </summary>
        /// <param name="text">name</param>
        /// <param name="error">error message</param>
        /// <returns>true if its correct , otherwise false</returns>
        public static Boolean ValidateUserName(Entities objectContext, ref String text, out string error)
        {
            Tools.AssertObjectContextExists(objectContext);
            Boolean passed = false;
            error = "";

            CommonCode.UiTools.ChangeUiCultureFromSession();

            text = Tools.RemoveSpacesAtEndOfString(text);

            if (Tools.NullValidatorPassed(text))
            {
                if (Tools.StringRangeValidatorPassed(Configuration.UsersMinUserNameLength, Configuration.UsersMaxUserNameLength, text))
                {
                    if (ValidateUserNameFormat(ref text, out error, true))
                    {
                        if (Tools.NameValidatorPassed(objectContext, "users", text, 0))
                        {
                            passed = true;
                        }
                        else
                        {
                            error = HttpContext.GetGlobalResourceObject("Validate", "errUserNameTaken").ToString();
                        }
                    }
                }
                else
                {
                    error = string.Format("{0} {1} {2} {3}.", HttpContext.GetGlobalResourceObject("Validate", "errNameLength")
                        , Configuration.UsersMinUserNameLength, HttpContext.GetGlobalResourceObject("Validate", "and")
                        , Configuration.UsersMaxUserNameLength);
                }
            }
            else
            {
                error = HttpContext.GetGlobalResourceObject("Validate", "errTypeName").ToString();
            }

            return passed;
        }

    }
}
