﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;

using BusinessLayer;
using DataAccess;

namespace UserInterface
{
    public partial class ActivateAccount : BasePage
    {
        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckParams();
        }

        private void CheckParams()
        {
            string strUserID = Request.Params["user"];
            string strKey = Request.Params["activatekey"];
            string strResetPass = Request.Params["resetkey"];

            if (string.IsNullOrEmpty(strUserID))
            {
                RedirrectToErrorPage(GetGlobalResourceObject("SiteResources", "errorIncParameters").ToString());
            }
            else
            {
                if (string.IsNullOrEmpty(strKey) && string.IsNullOrEmpty(strResetPass))
                {
                    RedirrectToErrorPage(GetGlobalResourceObject("SiteResources", "errorIncParameters").ToString());
                }
                else if (!string.IsNullOrEmpty(strKey) && !string.IsNullOrEmpty(strResetPass))
                {
                    RedirrectToErrorPage(GetGlobalResourceObject("SiteResources", "errorIncParameters").ToString());
                }

                if (!string.IsNullOrEmpty(strKey))
                {
                    long userId = -1;
                    if (long.TryParse(strUserID, out userId))
                    {
                        BusinessUser businessUser = new BusinessUser();

                        User activatingUser = businessUser.Get(userContext, userId, false);
                        if (activatingUser == null)
                        {
                            RedirrectToErrorPage(GetGlobalResourceObject("SiteResources", "errorIncParameters").ToString());
                        }

                        BusinessUserOptions userOptions = new BusinessUserOptions();
                        if (!activatingUser.UserOptionsReference.IsLoaded)
                        {
                            activatingUser.UserOptionsReference.Load();
                        }

                        if (!userOptions.IsUserActivated(activatingUser.UserOptions))
                        {
                            if (activatingUser.UserOptions.activationCode == strKey)
                            {
                                businessLog = new BusinessLog(userId, Request.UserHostAddress);

                                userOptions.SetAccountAsActivated(userContext, objectContext, activatingUser.UserOptions, businessLog);

                                lblNotification.Text = GetLocalResourceObject("accActivated").ToString();
                            }
                            else
                            {
                                RedirrectToErrorPage(GetGlobalResourceObject("SiteResources", "errorIncParameters").ToString());
                            }
                        }
                        else
                        {
                            RedirrectToErrorPage(GetLocalResourceObject("errUserActivated").ToString());
                        }

                    }
                    else
                    {
                        RedirrectToErrorPage(GetGlobalResourceObject("SiteResources", "errorIncParameters").ToString());
                    }
                }
                else if (!string.IsNullOrEmpty(strResetPass))
                {
                    long userId = -1;
                    if (long.TryParse(strUserID, out userId))
                    {
                        BusinessUser businessUser = new BusinessUser();

                        User user = businessUser.Get(userContext, userId, false);
                        if (user == null)
                        {
                            RedirrectToErrorPage(GetGlobalResourceObject("SiteResources", "errorIncParameters").ToString());
                        }

                        BusinessUserOptions userOptions = new BusinessUserOptions();
                        if (!user.UserOptionsReference.IsLoaded)
                        {
                            user.UserOptionsReference.Load();
                        }

                        if (userOptions.IsUserActivated(user.UserOptions))
                        {
                            if (user.UserOptions.resetPasswordKey == strResetPass)
                            {
                                if (!userOptions.CanUserCreateNewResetPasswordKey(objectContext, user.UserOptions))
                                {
                                    businessLog = new BusinessLog(userId, Request.UserHostAddress);

                                    userOptions.ResetUserPasswordAndSendHimMail(userContext, objectContext, user.UserOptions, businessLog);

                                    lblNotification.Text = GetLocalResourceObject("passResetSucc").ToString();
                                }
                                else
                                {
                                    RedirrectToErrorPage(GetLocalResourceObject("errInvLink").ToString());
                                }
                            }
                            else
                            {
                                RedirrectToErrorPage(GetGlobalResourceObject("SiteResources", "errorIncParameters").ToString());
                            }
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("user id = {0}{2}{3}", user.ID
                                , ", which is not activated has received reset password link,"
                                , " account which is not activated cannot receive reset passwrod link"));
                        }

                    }
                    else
                    {
                        RedirrectToErrorPage(GetGlobalResourceObject("SiteResources", "errorIncParameters").ToString());
                    }
                }
                else
                {
                    RedirrectToErrorPage(GetGlobalResourceObject("SiteResources", "errorIncParameters").ToString());
                }

            }
        }

        private void RedirrectToErrorPage(string error)
        {
            CommonCode.UiTools.RedirrectToErrorPage(Response, Session, error);
        }

    }
}
