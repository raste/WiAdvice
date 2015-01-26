// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

using DataAccess;

namespace BusinessLayer
{
    public class BusinessIpAttempts
    {
        /// <summary>
        ///  Checks if user reached max attempt tries, true if reached, otherwise false
        /// </summary>
        public bool MaxTriesReached(EntitiesUsers userContext, IpAttemptTry attTry, string ipAdress, out int minutesLeftToWait)
        {
            Tools.AssertObjectContextExists(userContext);
            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            minutesLeftToWait = 0;

            IpAttempt ipAttempt = userContext.IpAttemptSet.FirstOrDefault(ip => ip.IPaddress == ipAdress);
            if (ipAttempt == null)
            {
                return false;
            }

            bool result = false;

            int tries = 0;
            DateTime lastAttempt = DateTime.UtcNow;

            // Gets lastAttempt date and number tries, If there arent returns false
            switch (attTry)
            {
                case IpAttemptTry.AnswerSecQuestion:
                    if (ipAttempt.lastAnsSecQuestTry != null && ipAttempt.ansSecQuestAttempts != null)
                    {
                        lastAttempt = ipAttempt.lastAnsSecQuestTry.Value;
                        tries = ipAttempt.ansSecQuestAttempts.Value;
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case IpAttemptTry.guessUserAndMail:
                    if (ipAttempt.lastAnsUserAndMailTry != null && ipAttempt.ansUserAndMailAttempts != null)
                    {
                        lastAttempt = ipAttempt.lastAnsUserAndMailTry.Value;
                        tries = ipAttempt.ansUserAndMailAttempts.Value;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case IpAttemptTry.LogIn:
                    if (ipAttempt.lastLogIn != null && ipAttempt.loginAttempts != null)
                    {
                        lastAttempt = ipAttempt.lastLogIn.Value;
                        tries = ipAttempt.loginAttempts.Value;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                default:
                    throw new BusinessException(string.Format("attemptTry = '{0}' is not supported attempt", attTry));
            }

            bool setToNullTries = false;

            DateTime now = DateTime.UtcNow;
            TimeSpan span = now - lastAttempt;

            if (tries >= Configuration.IpAttemptMaxNumTries)
            {
                // if there are more tries..check when is last try..and if time is more than needs to pass..set to Null tries and Last time 
                if (span.TotalMinutes < Configuration.IpAttemptTimeWhichNeedsToPassToResetTries)
                {
                    minutesLeftToWait = Configuration.IpAttemptTimeWhichNeedsToPassToResetTries - (int)span.TotalMinutes;
                    result = true;
                }
                else
                {
                    setToNullTries = true;
                }
            }
            else
            {
                // check when is last try..and if time is more than needs to pass..set to Null tries and Last time 
                if (span.TotalMinutes >= Configuration.IpAttemptTimeWhichNeedsToPassToResetTries)
                {
                    setToNullTries = true;
                }
            }

            if (setToNullTries == true)
            {
                switch (attTry)
                {
                    case IpAttemptTry.AnswerSecQuestion:
                        ipAttempt.ansSecQuestAttempts = 0;
                        Tools.Save(userContext);

                        break;
                    case IpAttemptTry.guessUserAndMail:
                        ipAttempt.ansUserAndMailAttempts = 0;
                        Tools.Save(userContext);

                        break;
                    case IpAttemptTry.LogIn:
                        ipAttempt.loginAttempts = 0;
                        Tools.Save(userContext);

                        break;
                    default:
                        throw new BusinessException(string.Format("attemptTry = '{0}' is not supported attempt", attTry));
                }
            }

            return result;
        }

        public void AttemptWrong(EntitiesUsers userContext, IpAttemptTry attemptTry, string ipAdress)
        {
            Tools.AssertObjectContextExists(userContext);
            if (string.IsNullOrEmpty(ipAdress))
            {
                throw new BusinessException("ipAdress is empty");
            }

            DateTime now = DateTime.UtcNow;
            DateTime lastTry;
            TimeSpan span;

            IpAttempt attempt = userContext.IpAttemptSet.FirstOrDefault(ip => ip.IPaddress == ipAdress);
            if (attempt != null)
            {
                switch (attemptTry)
                {
                    case IpAttemptTry.AnswerSecQuestion:

                        if (attempt.ansSecQuestAttempts == null || attempt.lastAnsSecQuestTry == null)
                        {
                            attempt.ansSecQuestAttempts = 1;
                        }
                        else
                        {
                            lastTry = attempt.lastAnsSecQuestTry.Value;
                            span = now - lastTry;
                            if (span.TotalMinutes < Configuration.IpAttemptTimeWhichNeedsToPassToResetTries)
                            {
                                attempt.ansSecQuestAttempts++;
                            }
                            else
                            {
                                attempt.ansSecQuestAttempts = 1;
                            }
                        }

                        attempt.lastAnsSecQuestTry = now;

                        break;
                    case IpAttemptTry.guessUserAndMail:

                        if (attempt.ansUserAndMailAttempts == null || attempt.lastAnsUserAndMailTry == null)
                        {
                            attempt.ansUserAndMailAttempts = 1;
                        }
                        else
                        {
                            lastTry = attempt.lastAnsUserAndMailTry.Value;
                            span = now - lastTry;
                            if (span.TotalMinutes < Configuration.IpAttemptTimeWhichNeedsToPassToResetTries)
                            {
                                attempt.ansUserAndMailAttempts++;
                            }
                            else
                            {
                                attempt.ansUserAndMailAttempts = 1;
                            }
                        }

                        attempt.lastAnsUserAndMailTry = now;

                        break;
                    case IpAttemptTry.LogIn:

                        if (attempt.loginAttempts == null || attempt.lastLogIn == null)
                        {
                            attempt.loginAttempts = 1;
                        }
                        else
                        {
                            lastTry = attempt.lastLogIn.Value;
                            span = now - lastTry;
                            if (span.TotalMinutes < Configuration.IpAttemptTimeWhichNeedsToPassToResetTries)
                            {
                                attempt.loginAttempts++;
                            }
                            else
                            {
                                attempt.loginAttempts = 1;
                            }
                        }

                        attempt.lastLogIn = now;

                        break;
                    default:
                        throw new BusinessException(string.Format("attemptTry = '{0}' is not supported attempt", attemptTry));
                }

                Tools.Save(userContext);
            }
            else
            {
                IpAttempt newAttempt = new IpAttempt();

                switch (attemptTry)
                {
                    case IpAttemptTry.AnswerSecQuestion:

                        newAttempt.IPaddress = ipAdress;
                        newAttempt.ansSecQuestAttempts = 1;
                        newAttempt.lastAnsSecQuestTry = now;

                        break;
                    case IpAttemptTry.guessUserAndMail:

                        newAttempt.IPaddress = ipAdress;
                        newAttempt.ansUserAndMailAttempts = 1;
                        newAttempt.lastAnsUserAndMailTry = now;

                        break;
                    case IpAttemptTry.LogIn:

                        newAttempt.IPaddress = ipAdress;
                        newAttempt.loginAttempts = 1;
                        newAttempt.lastLogIn = now;

                        break;
                    default:
                        throw new BusinessException(string.Format("attemptTry = '{0}' is not supported attempt", attemptTry));
                }

                userContext.AddToIpAttemptSet(newAttempt);
                Tools.Save(userContext);
            }
        }

        public void DeleteOldIpAttempts(EntitiesUsers userContext)
        {
            Tools.AssertObjectContextExists(userContext);

            List<IpAttempt> none = new List<IpAttempt>();

            none = userContext.DeleteOldIPAttempts().ToList();
        }

    }
}
