// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessCompanyType
    {
        object changingCompanyTypes = new object();
        object addingNewType = new object();

        private void Add(Entities objectContext, CompanyType newType, BusinessLog bLog, User currentUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (newType == null)
            {
                throw new ArgumentNullException("newType");
            }
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }

            objectContext.AddToCompanyTypeSet(newType);
            Tools.Save(objectContext);

            bLog.LogCompanyType(objectContext, newType, LogType.create, string.Empty, string.Empty, currentUser);
        }

        public void AddCompanyType(Entities objectContext, BusinessLog bLog, User currentUser, string name, string description)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);
            if (currentUser == null)
            {
                throw new BusinessException("currentUser is null");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessException("name is null or empty");
            }

            if (!Tools.NameValidatorPassed(objectContext, "companyType", name, 0))
            {
                throw new BusinessException(string.Format("Cannot add new CompanyType with name = {0} , because it is already taken, user id = {1}"
                    , name, currentUser.ID));
            }

            if (!Tools.StringRangeValidatorPassed(0, Configuration.CompanyTypesMaxDescriptionLenght, description))
            {
                throw new BusinessException(string.Format("Cannot add company type with description = '{0}' because it is out of limit, user id = {1}"
                    , description, currentUser.ID));
            }

            ThrowExcIfUserIsIncorrect(currentUser);

            lock (addingNewType)
            {
                CompanyType newType = new CompanyType();
                newType.name = name;
                newType.description = description;
                newType.visible = true;
                newType.CreatedBy = Tools.GetUserID(objectContext, currentUser);
                newType.LstModifiedBy = newType.CreatedBy;
                newType.dateCreated = DateTime.UtcNow;
                newType.lastModified = newType.dateCreated;

                Add(objectContext, newType, bLog, currentUser);
            }
        }

        /// <summary>
        /// Returns CompanyType with ID if found
        /// </summary>
        /// <param name="onlyIfVisible">true if should look only for visible=true, otherwise false</param>
        public CompanyType Get(Entities objectContext, long id, bool onlyIfVisible)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (id < 1)
            {
                throw new BusinessException("id is < 1");
            }
            CompanyType currType = null;
            if (onlyIfVisible)
            {
                currType = objectContext.CompanyTypeSet.FirstOrDefault(type => type.ID == id && type.visible == true);
            }
            else
            {
                currType = objectContext.CompanyTypeSet.FirstOrDefault(type => type.ID == id);
            }

            return currType;
        }

        public CompanyType GetOtherCompanyType(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            User systemUser = Tools.GetSystem();

            string otherCompanyType = Tools.GetConfigurationResource("OtherCompanyType");

            CompanyType other = objectContext.CompanyTypeSet.FirstOrDefault(type => type.name == otherCompanyType
                && type.CreatedBy.ID == systemUser.ID && type.visible == true);

            if (other == null)
            {
                throw new BusinessException(string.Format("Couldn`t find other company type with local name = {0} .", otherCompanyType));
            }

            return other;
        }

        public CompanyType GetCompanyType(Entities objectContext, EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertObjectContextExists(userContext);

            CompanyType company = objectContext.CompanyTypeSet.FirstOrDefault(type => type.name == "company" && type.visible == true);

            if (company == null)
            {
                throw new BusinessException(string.Format("Couldn`t find other company type"));
            }

            return company;
        }

        /// <param name="sortedByAsc">true if tehy should be sorted, otherwise false</param>
        /// <param name="onlyVisible">true if should get only visible true</param>
        public List<CompanyType> GetAllCompanyTypes(Entities objectContext, bool sortedByAsc, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(objectContext);

            IEnumerable<CompanyType> types = null;
            if (onlyVisible == true)
            {
                if (sortedByAsc)
                {
                    types = objectContext.CompanyTypeSet.Where(type => type.visible == true)
                        .OrderBy<CompanyType, string>(new Func<CompanyType, string>(NameSelector));
                }
                else
                {
                    types = objectContext.CompanyTypeSet.Where(type => type.visible == true);
                }
            }
            else
            {
                if (sortedByAsc)
                {
                    types = objectContext.CompanyTypeSet.OrderBy<CompanyType, string>(new Func<CompanyType, string>(NameSelector));
                }
                else
                {
                    types = objectContext.CompanyTypeSet;
                }
            }

            List<CompanyType> listedTypes = types.ToList();
            if (listedTypes.Count < 1)
            {
                throw new BusinessException("There are no added company types.");
            }

            return listedTypes;
        }

        private string NameSelector(CompanyType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.name;
        }

        /// <summary>
        /// Changes name or description of company type
        /// </summary>
        /// <param name="field">name , description</param>
        public void ChangeField(Entities objectContext, CompanyType currType, User currUser, BusinessLog bLog
            , string field, string newValue)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currType == null)
            {
                throw new BusinessException("currType is null");
            }

            if (string.IsNullOrEmpty(field))
            {
                throw new BusinessException("field is null or empty");
            }

            if (currType == GetOtherCompanyType(objectContext))
            {
                throw new BusinessException("Other company type cannot be modified");
            }

            ThrowExcIfUserIsIncorrect(currUser);

            string oldValue = string.Empty;

            switch (field)
            {
                case ("name"):
                    if (string.IsNullOrEmpty(newValue))
                    {
                        throw new BusinessException("newValue is null or empty");
                    }

                    if (Tools.NameValidatorPassed(objectContext, "companyType", newValue, 0))
                    {
                        oldValue = currType.name;

                        currType.lastModified = DateTime.UtcNow;
                        currType.LstModifiedBy = Tools.GetUserID(objectContext, currUser);
                        currType.name = newValue;

                        Tools.Save(objectContext);

                        bLog.LogCompanyType(objectContext, currType, LogType.edit, "name", oldValue, currUser);
                    }
                    else
                    {
                        throw new BusinessException(string.Format("CompanyType cannot change name to = {0} , because it is already taken, user id = {1}"
                            , newValue, currUser.ID));
                    }

                    break;
                case ("description"):

                    if (currType.description != newValue)
                    {
                        if (!Tools.StringRangeValidatorPassed(0, Configuration.CompanyTypesMaxDescriptionLenght, currType.description))
                        {
                            throw new BusinessException(string.Format("Cannot change company type description with = '{0}' because it is out of limit, user id = {1}"
                                , newValue, currUser.ID));
                        }
                        oldValue = currType.description;

                        currType.lastModified = DateTime.UtcNow;
                        currType.LstModifiedBy = Tools.GetUserID(objectContext, currUser);
                        currType.description = newValue;

                        Tools.Save(objectContext);

                        bLog.LogCompanyType(objectContext, currType, LogType.edit, "description", oldValue, currUser);
                    }

                    break;
                default:
                    throw new BusinessException(string.Format("field = {0} is not supported field", field));
            }
        }

        public int CountVisibleTypes(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            int count = objectContext.CompanyTypeSet.Count(tp => tp.visible == true);

            return count;
        }

        /// <summary>
        /// Returns all companies which are from type
        /// </summary>
        public List<Company> GetAllCompaniesWhichAreFromType(Entities objectContext, CompanyType currType)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currType == null)
            {
                throw new BusinessException("currType is null");
            }

            currType.Companies.Load();
            IEnumerable<Company> companies = currType.Companies;

            return companies.ToList();
        }

        public long CountAllCompaniesWhichAreFromType(Entities objectContext, CompanyType currType)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currType == null)
            {
                throw new BusinessException("currType is null");
            }

            currType.Companies.Load();
            long count = currType.Companies.Count;

            return count;
        }


        public void DeleteCompanyType(Entities objectContext, CompanyType currType, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currType == null)
            {
                throw new BusinessException("currType is null");
            }

            if (CountVisibleTypes(objectContext) < 2)
            {
                throw new BusinessException("CompanyType cannot be deleted because it is the only one");
            }

            CompanyType other = GetOtherCompanyType(objectContext);
            if (currType == other)
            {
                throw new BusinessException("Other company type cannot be modified");
            }

            ThrowExcIfUserIsIncorrect(currUser);

            if (currType.visible == true)
            {
                lock (changingCompanyTypes)
                {

                    List<Company> companies = GetAllCompaniesWhichAreFromType(objectContext, currType);
                    if (companies.Count > 0)
                    {

                        string oldType = currType.name;

                        BusinessCompany businessCompany = new BusinessCompany();

                        foreach (Company company in companies)
                        {
                            businessCompany.ChangeCompanyType(objectContext, company, other, currUser, bLog);
                        }
                    }

                    currType.visible = false;
                    currType.lastModified = DateTime.UtcNow;
                    currType.LstModifiedBy = Tools.GetUserID(objectContext, currUser);

                    Tools.Save(objectContext);

                    bLog.LogCompanyType(objectContext, currType, LogType.delete, string.Empty, string.Empty, currUser);
                }
            }
        }

        public void UnDeleteCompanyType(Entities objectContext, CompanyType currType, User currUser, BusinessLog bLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            Tools.AssertBusinessLogExists(bLog);

            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }
            if (currType == null)
            {
                throw new BusinessException("currType is null");
            }

            ThrowExcIfUserIsIncorrect(currUser);

            CompanyType other = GetOtherCompanyType(objectContext);
            if (currType == other)
            {
                throw new BusinessException("Other company type cannot be modified");
            }

            if (currType.visible == false)
            {
                currType.visible = true;
                currType.lastModified = DateTime.UtcNow;
                currType.LstModifiedBy = Tools.GetUserID(objectContext, currUser);

                Tools.Save(objectContext);

                bLog.LogCompanyType(objectContext, currType, LogType.undelete, string.Empty, string.Empty, currUser);
            }
        }

        /// <summary>
        /// USed to check if user modifying is GlobalAdministrator ..if not throws exception
        /// </summary>
        /// <param name="currUser">user modifying company type</param>
        public void ThrowExcIfUserIsIncorrect(User currUser)
        {
            if (currUser == null)
            {
                throw new BusinessException("currUser is null");
            }

            BusinessUser businessUser = new BusinessUser();
            if (!businessUser.IsGlobalAdministrator(currUser))
            {   // Types created by system cannot be deleted
                throw new BusinessException(string.Format("CompanyType ID = {0} can be modified only by golbal admins"));
            }
        }

    }
}
