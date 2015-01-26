﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Web;

using DataAccess;
using BusinessLayer;
using log4net;

namespace UserInterface.CommonCode
{
    public static class IpAttempts
    {
        private static string AnswerSecQuestTries = "AnswerSecQuestTries";
        private static string LastAnswerSecQuestTry = "LastAnswerSecQuestTry";

        private static string GuessUserAndMailTries = "GuessUserAndMailTries";
        private static string LastGuessUserAndMailTry = "LastGuessUserAndMailTry";

        private static string LogInTries = "LogInTries";
        private static string LastLogInTry = "LastLogInTry";

        private static ILog log = LogManager.GetLogger(typeof(IpAttempts));

        public static void AttemptWrong(EntitiesUsers userContext, IpAttemptTry attemptTry)
        {
            Tools.AssertObjectContextExists(userContext);
            string ipAddress = HttpContext.Current.Request.UserHostAddress;

            BusinessIpAttempts bIpAttempts = new BusinessIpAttempts();
            bIpAttempts.AttemptWrong(userContext, attemptTry, ipAddress);

            switch (attemptTry)
            {
                case IpAttemptTry.AnswerSecQuestion:

                    AddWrongAttempt(AnswerSecQuestTries, LastAnswerSecQuestTry);

                    break;
                case IpAttemptTry.guessUserAndMail:

                    AddWrongAttempt(GuessUserAndMailTries, LastGuessUserAndMailTry);

                    break;
                case IpAttemptTry.LogIn:

                    AddWrongAttempt(LogInTries, LastLogInTry);

                    break;
                default:
                    throw new CommonCode.UIException(string.Format("attemptTry = '{0}' is not supported attempt", attemptTry));
            }
        }

        private static void AddWrongAttempt(string sessionTriesParam, string sessionLastTry)
        {
            if (string.IsNullOrEmpty(sessionTriesParam))
            {
                throw new UIException("sessionTriesParam is empty");
            }
            if (string.IsNullOrEmpty(sessionLastTry))
            {
                throw new UIException("sessionLastTry is empty");
            }

            object objSessParam = HttpContext.Current.Session[sessionTriesParam];
            object objLastFailureTime = HttpContext.Current.Session[sessionLastTry];

            if (objSessParam != null && objLastFailureTime != null)
            {
                DateTime lastTry;
                if (DateTime.TryParse(objLastFailureTime.ToString(), out lastTry))
                {
                    DateTime now = DateTime.UtcNow;
                    TimeSpan span = now - lastTry;

                    if (span.TotalMinutes >= Configuration.IpAttemptTimeWhichNeedsToPassToResetTries)
                    {
                        HttpContext.Current.Session[sessionTriesParam] = 1;
                    }
                    else
                    {
                        int wrongAttempts = 0;

                        if (int.TryParse(objSessParam.ToString(), out wrongAttempts))
                        {
                            wrongAttempts++;
                            HttpContext.Current.Session[sessionTriesParam] = wrongAttempts;
                        }
                        else
                        {
                            throw new CommonCode.UIException(string.Format("Couldn`t parse Session['{0}'] to int", sessionTriesParam));
                        }
                    }
                }
                else
                {
                    throw new CommonCode.UIException(string.Format("couldn`t parse Session['{0}'] to DateTime", sessionLastTry));
                }
            }
            else
            {
                HttpContext.Current.Session.Add(sessionTriesParam, "1");
            }

            HttpContext.Current.Session.Add(sessionLastTry, DateTime.UtcNow.ToString());
        }

        /// <summary>
        /// Checks if the minimum time after which page loaded passed, true if passed, otherwise false
        /// </summary>
        public static bool MinTimeAfterPageLoadPassed()
        {
            bool passed = false;

            object objDate = HttpContext.Current.Session["PageLoaded"];
            if (objDate == null)
            {
                if (log.IsWarnEnabled == true)
                {
                    log.Warn("LogIn page Session[\"PageLoaded\"] is null");
                }
                return true;
            }
            DateTime? pageCreationTimeNullable = objDate as DateTime?;
            if (pageCreationTimeNullable.HasValue == false)
            {
                throw new CommonCode.UIException("Session['PageLoaded'] is not DateTime?");
            }
            DateTime pageCreationTime = pageCreationTimeNullable.Value;
            DateTime now = DateTime.UtcNow;
            TimeSpan span = now - pageCreationTime;

            if (span.TotalSeconds >= Configuration.IpAttemptMinTimeWhichNeedsToPassAfterPageLoad)
            {
                passed = true;
            }
            else
            {
                object objTimesFailed = HttpContext.Current.Session["TimesFailedToWaitMinTimeAfterPageLoad"];
                if (objTimesFailed != null)
                {
                    int times = 0;
                    if (int.TryParse(objTimesFailed.ToString(), out times))
                    {
                        times++;
                        HttpContext.Current.Session["TimesFailedToWaitMinTimeAfterPageLoad"] = times;

                        if (times >= Configuration.IpAttemptFailuresAfterWhichConsideredBot)
                        {
                            object objIsBot = HttpContext.Current.Session["IsBot"];
                            if (objIsBot == null)
                            {
                                HttpContext.Current.Session.Add("IsBot", "true");
                            }
                            else
                            {
                                HttpContext.Current.Session["IsBot"] = "true";
                            }
                        }

                    }
                    else
                    {
                        throw new CommonCode.UIException("Couldn`t parse Session['TimesFailedToWaitMinTimeAfterPageLoad'] to int");
                    }
                }
                else
                {
                    HttpContext.Current.Session.Add("TimesFailedToWaitMinTimeAfterPageLoad", "1");
                }
            }

            return passed;
        }

        public static bool MaxTriesReached(EntitiesUsers userContext, IpAttemptTry attemptTry, out int minutesLeftToWait)
        {
            Tools.AssertObjectContextExists(userContext);

            BusinessIpAttempts bIpAttempts = new BusinessIpAttempts();

            bool reached = false;
            minutesLeftToWait = 0;

            switch (attemptTry)
            {
                case IpAttemptTry.AnswerSecQuestion:

                    if (bIpAttempts.MaxTriesReached(userContext, attemptTry, HttpContext.Current.Request.UserHostAddress, out minutesLeftToWait) == false)
                    {
                        reached = MaxAttemptTriesReached(AnswerSecQuestTries, LastAnswerSecQuestTry, out minutesLeftToWait);
                    }
                    else
                    {
                        reached = true;
                    }

                    break;
                case IpAttemptTry.guessUserAndMail:

                    if (bIpAttempts.MaxTriesReached(userContext, attemptTry, HttpContext.Current.Request.UserHostAddress, out minutesLeftToWait) == false)
                    {
                        reached = MaxAttemptTriesReached(GuessUserAndMailTries, LastGuessUserAndMailTry, out minutesLeftToWait);
                    }
                    else
                    {
                        reached = true;
                    }

                    break;
                case IpAttemptTry.LogIn:

                    if (bIpAttempts.MaxTriesReached(userContext, attemptTry, HttpContext.Current.Request.UserHostAddress, out minutesLeftToWait) == false)
                    {
                        reached = MaxAttemptTriesReached(LogInTries, LastLogInTry, out minutesLeftToWait);
                    }
                    else
                    {
                        reached = true;
                    }

                    break;
                default:
                    throw new CommonCode.UIException(string.Format("attemptTry = '{0}' is not supported attempt", attemptTry));
            }

            if (reached == true && minutesLeftToWait < 1)
            {
                minutesLeftToWait = 1;
            }

            return reached;
        }

        private static bool MaxAttemptTriesReached(string sessionTriesParam, string sessionLastTry, out int minutesLeftToWait)
        {

            if (string.IsNullOrEmpty(sessionTriesParam))
            {
                throw new UIException("sessionTriesParam is empty");
            }
            if (string.IsNullOrEmpty(sessionLastTry))
            {
                throw new UIException("sessionLastTry is empty");
            }


            bool result = false;
            minutesLeftToWait = 0;

            object objTries = HttpContext.Current.Session[sessionTriesParam];
            if (objTries != null)
            {
                int tries;
                if (int.TryParse(objTries.ToString(), out tries))
                {
                    if (tries >= Configuration.IpAttemptMaxNumTries)
                    {
                        object objLastTry = HttpContext.Current.Session[sessionLastTry];
                        if (objLastTry != null)
                        {
                            DateTime now = DateTime.UtcNow;
                            DateTime lastTry;
                            if (DateTime.TryParse(objLastTry.ToString(), out lastTry))
                            {
                                TimeSpan span = now - lastTry;
                                if (span.TotalMinutes < Configuration.IpAttemptTimeWhichNeedsToPassToResetTries)
                                {
                                    result = true;
                                    minutesLeftToWait = Configuration.IpAttemptTimeWhichNeedsToPassToResetTries - (int)span.TotalMinutes;
                                }
                            }
                            else
                            {
                                throw new UIException(string.Format("Couldn`t parse Session['{0}'] to datetime.", sessionLastTry));
                            }

                        }
                        else
                        {
                            result = true;
                        }
                    }
                    // not reached
                }
                else
                {
                    throw new UIException(string.Format("Couldn`t parse Session['{0}'] to int.", sessionTriesParam));
                }
            }

            return result;
        }

    }
}
