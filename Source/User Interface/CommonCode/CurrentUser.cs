﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Web;

namespace UserInterface.CommonCode
{
    public class CurrentUser
    {
        public static string CurrentUserIdKey = "CurrentUserId";

        /// <summary>
        /// Returns the current user ID if its logged , otherwise -1
        /// </summary>
        public static long GetCurrentUserId()
        {
            HttpContext currentContext = HttpContext.Current;
            if (currentContext == null)
            {
                throw new InvalidOperationException("The current HttpContext is not available.");
            }

            // Similar code is present in Global.Session_End() - "Global.asax".
            // Keep both pieces of code consistent --------------------------------------
            long currentUserId = -1;
            object currentUserIdObj = currentContext.Session[CurrentUserIdKey];
            if (currentUserIdObj != null)
            {
                try
                {
                    currentUserId = Convert.ToInt64(currentUserIdObj);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        string.Format("The current user ID is not a \"{0}\".",
                        typeof(long).FullName),
                        ex);
                }
            }
            // --------------------------------------------------------------------------

            return currentUserId;
        }


    }
}
