﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using AjaxControlToolkit;

using BusinessLayer;
using DataAccess;

namespace UserInterface
{
    public partial class Statistics : BasePage
    {
        private Entities objectContext = null;
        private EntitiesUsers userContext = new EntitiesUsers();

        protected void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetNeedsToBeLogged();

            CheckUser();                    // checks user , redirrects if not admin
            ShowAtTheMoment();              // shows users label and button for showing online users 

            ShowStatistics(null);
            
            ShowInfo();
        }

        private void ShowInfo()
        {
            Title = "Statistics Page";

            BusinessSiteText siteText = new BusinessSiteText();

            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutStatistics");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblAbout.Text = aboutExtended.description; 
            }
            else
            {
                lblAbout.Text = "About statistics text not typed.";
            }

            CheckShowHideButtons();
        }

        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);

            if (currUser != null && businessUser.IsFromAdminTeam(currUser))
            {
                // ok .. 
            }
            else
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, "That page is for administrators only.");
            }
        }

        private void ShowAtTheMoment()
        {
            BusinessStatistics stats = new BusinessStatistics();
            BusinessUser businessUser = new BusinessUser();

            int loggedUsers = businessUser.GetLoggedUsers().Count;

            lblAtm.Text = string.Format("Currently online are : {0} guests, and {1} users.",
                businessUser.GetGuests(), loggedUsers);

            if (loggedUsers > 0)
            {
                btnShowRegOn.Visible = true;
            }
            else
            {
                btnShowRegOn.Visible = false;
            }

        }

        private void ShowStatistics(List<Statistic> Statistics)
        {
            tblLast5Days.Rows.Clear();

            BusinessStatistics businessStatistics = new BusinessStatistics();

            List<Statistic> stats = new List<Statistic>();

            if (Statistics == null || Statistics.Count < 1)
            {
                stats = businessStatistics.GetLastNumStatistics(objectContext, 5);
            }
            else
            {
                stats = Statistics;
            }

            int count = stats.Count;

            if (count > 0)
            {

                TableRow zeroRow = new TableRow();
                TableCell zeroCell = new TableCell();
                zeroCell.CssClass = "textHeaderWA";
                zeroCell.ColumnSpan = 13;
                zeroCell.HorizontalAlign = HorizontalAlign.Center;
                if (count == 1)
                {
                    zeroCell.Text = string.Format("Statistic for {0}",stats[0].forDate.ToString("d", System.Globalization.CultureInfo.InvariantCulture));
                }
                else
                {
                    zeroCell.Text = string.Format("Statistics for last {0} days", count);
                }
                
                zeroRow.Cells.Add(zeroCell);
                tblLast5Days.Rows.Add(zeroRow);

                ShowFirstAndSecRowForLastDaysStatsTable();

                foreach(Statistic statistic in stats)
                {
                    // date , users , admins , products , prod char , companies , comp char
                    // categries , pictures , comments , reports , sessions , users L/O

                    TableRow newRow = new TableRow();

                    TableCell DateCell = new TableCell();
                    DateCell.CssClass = "commentsDate";
                    DateCell.Text = statistic.forDate.ToShortDateString();
                    newRow.Cells.Add(DateCell);

                    TableCell usersCell = new TableCell();
                    usersCell.Text = string.Format("{0} / {1}", statistic.usersRegistered,statistic.usersDeleted);
                    newRow.Cells.Add(usersCell);

                    TableCell adminsCell = new TableCell();
                    adminsCell.Text = string.Format("{0} / {1}", statistic.adminsRegistered,statistic.adminsDeleted);
                    newRow.Cells.Add(adminsCell);

                    TableCell prodCell = new TableCell();
                    prodCell.Text = string.Format("{0} / {1}", statistic.productsCreated,statistic.productsDeleted);
                    newRow.Cells.Add(prodCell);

                    TableCell prodChCell = new TableCell();
                    prodChCell.Text = string.Format("{0} / {1}", statistic.prodCharsCreated,statistic.prodCharsDeleted);
                    newRow.Cells.Add(prodChCell);

                    TableCell compCell = new TableCell();
                    compCell.Text = string.Format("{0} / {1}", statistic.companiesCreated,statistic.companiesDeleted);
                    newRow.Cells.Add(compCell);

                    TableCell compChCell = new TableCell();
                    compChCell.Text = string.Format("{0} / {1}", statistic.compCharsCreated,statistic.compCharsDeleted);
                    newRow.Cells.Add(compChCell);

                    TableCell catCell = new TableCell();
                    catCell.Text = string.Format("{0} / {1}", statistic.categoriesCreated,statistic.categoriesDeleted);
                    newRow.Cells.Add(catCell);

                    TableCell picCell = new TableCell();
                    picCell.Text = string.Format("{0} / {1}", statistic.picturesUploaded,statistic.picturesDeleted);
                    newRow.Cells.Add(picCell);

                    TableCell commCell = new TableCell();
                    commCell.Text = string.Format("{0} / {1}", statistic.commentsWritten, statistic.commentsDeleted);
                    newRow.Cells.Add(commCell);

                    TableCell topicCell = new TableCell();
                    topicCell.Text = string.Format("{0} / {1}", statistic.topicsCreated, statistic.topicsDeleted);
                    newRow.Cells.Add(topicCell);


                    TableCell repCell = new TableCell();
                    repCell.Text = statistic.reportsWritten.ToString();
                    newRow.Cells.Add(repCell);

                    TableCell usrLoCell = new TableCell();
                    usrLoCell.Text = string.Format("{0} / {1}", statistic.usersLogged, statistic.usersLoggedOut);
                    newRow.Cells.Add(usrLoCell);

                    tblLast5Days.Rows.Add(newRow);

                }

            }
            else
            {
                tblLast5Days.Visible = false;
            }
        }

        private void ShowFirstAndSecRowForLastDaysStatsTable()
        {

            TableRow firstRow = new TableRow();
            TableCell empCell = new TableCell();
            firstRow.Cells.Add(empCell);

            TableCell createDelCell = new TableCell();
            createDelCell.ColumnSpan = 10;
            createDelCell.HorizontalAlign = HorizontalAlign.Center;
            createDelCell.Text = "Created/Deleted";
            firstRow.Cells.Add(createDelCell);

            TableCell writtenCell = new TableCell();
            writtenCell.ColumnSpan = 1;
            writtenCell.HorizontalAlign = HorizontalAlign.Center;
            writtenCell.Text = "Written";
            firstRow.Cells.Add(writtenCell);

            TableCell otherCell = new TableCell();
            otherCell.ColumnSpan = 1;
            otherCell.HorizontalAlign = HorizontalAlign.Center;
            otherCell.Text = "others";
            firstRow.Cells.Add(otherCell);

            tblLast5Days.Rows.Add(firstRow);

            TableRow secRow = new TableRow();

            TableCell dateCell = new TableCell();
            dateCell.Text = "Date";
            secRow.Cells.Add(dateCell);

            ///
            TableCell usersCell = new TableCell();
            usersCell.Text = "users";
            secRow.Cells.Add(usersCell);

            TableCell adminsCell = new TableCell();
            adminsCell.Text = "admins";
            secRow.Cells.Add(adminsCell);

            TableCell prodsCell = new TableCell();
            prodsCell.Text = "products";
            secRow.Cells.Add(prodsCell);

            TableCell prodChCell = new TableCell();
            prodChCell.Text = "prod Chars";
            secRow.Cells.Add(prodChCell);

            TableCell compCell = new TableCell();
            compCell.Text = string.Format("{0}s", Configuration.CompanyName);
            secRow.Cells.Add(compCell);

            TableCell compChCell = new TableCell();
            compChCell.Text = "comp Chars";
            secRow.Cells.Add(compChCell);

            TableCell catCell = new TableCell();
            catCell.Text = "categories";
            secRow.Cells.Add(catCell);

            TableCell picCell = new TableCell();
            picCell.Text = "pictures";
            secRow.Cells.Add(picCell);
            
            TableCell commCell = new TableCell();
            commCell.Text = "comments";
            secRow.Cells.Add(commCell);

            TableCell topicCell = new TableCell();
            topicCell.Text = "topics";
            secRow.Cells.Add(topicCell);

            ////
            TableCell repCell = new TableCell();
            repCell.Text = "reports";
            secRow.Cells.Add(repCell);

            TableCell usrLOCell = new TableCell();
            usrLOCell.Text = "users L/O ";
            secRow.Cells.Add(usrLOCell);

            tblLast5Days.Rows.Add(secRow);
        }

        protected void btnShowStats_Click(object sender, EventArgs e)
        {
            phShowStats.Visible = true;
            phShowStats.Controls.Add(lblError);

            String strDate = tbDate.Text;
            String strNum = tbNum.Text;

            if (string.IsNullOrEmpty(strDate) && string.IsNullOrEmpty(strNum))
            {
                lblError.Text = "Type date or number";
            }
            else if (!string.IsNullOrEmpty(strDate) && !string.IsNullOrEmpty(strNum))
            {
                lblError.Text = "Type only date or number";
            }
            else
            {
                BusinessStatistics businessStatistics = new BusinessStatistics();

                if (strDate.Length > 0)
                {
                    DateTime newTime = new DateTime();

                    if (Tools.ParseStringToDateTime(strDate, out newTime) == true)
                    {
                        Statistic currStat = businessStatistics.Get(objectContext, newTime);
                        if (currStat == null)
                        {
                            lblError.Text = string.Format("No statistics for date {0}", newTime.ToShortDateString());
                        }
                        else
                        {
                            phShowStats.Visible = false;

                            List<Statistic> Stat = new List<Statistic>();
                            Stat.Add(currStat);

                            ShowStatistics(Stat);
                        }
                    }
                    else
                    {
                        lblError.Text = "Type correct date.";
                    }
                }

                if (strNum.Length > 0)
                {

                    long number = -1;
                    if (long.TryParse(strNum, out number))
                    {
                        if (number > 0)
                        {
                            List<Statistic> Stats = businessStatistics.GetLastNumStatistics(objectContext, number);
                            if (Stats.Count > 0)
                            {
                                phShowStats.Visible = false;

                                ShowStatistics(Stats);
                            }
                            else
                            {
                                lblError.Text = "No statistics.";
                            }


                        }
                        else
                        {
                            lblError.Text = "Number must be positive";
                        }
                    }
                    else
                    {
                        lblError.Text = "Type correct number";
                    }

                }

            }
        }

        protected void cExpireDate_SelectionChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cExpireDate.SelectedDate.ToString()))
            {

                string dateStr = Tools.ParseDateTimeToString(cExpireDate.SelectedDate);

                PopupControlExtender.GetProxyForCurrentPopup(Page).Commit(dateStr);

            }
        }

        private void CheckShowHideButtons()
        {
            if (btnShowRegOn.Text == "Hide logged users")
            {
                tblShowRegOn.Visible = true;
                ShowLoggedOnlineUsers();
            }
        }

        protected void btnShowRegOn_Click(object sender, EventArgs e)
        {
            if (btnShowRegOn.Text == "Show logged users")
            {
                btnShowRegOn.Text = "Hide logged users";
                tblShowRegOn.Visible = true;
                ShowLoggedOnlineUsers();
            }
            else
            {
                btnShowRegOn.Text = "Show logged users";
                tblShowRegOn.Visible = false;
            }
            
        }

        private void ShowLoggedOnlineUsers()
        {
            BusinessUser businessUser = new BusinessUser();
            List<User> loggedUsers = businessUser.GetLoggedUsers(userContext);
            tblShowRegOn.Rows.Clear();


            TableRow firstRow = new TableRow();
            tblShowRegOn.Rows.Add(firstRow);

            TableCell firstCell = new TableCell();
            firstRow.Cells.Add(firstCell);

            firstCell.CssClass = "textHeaderWA";
            firstCell.HorizontalAlign = HorizontalAlign.Center;

            if (loggedUsers.Count > 0)
            {
                firstCell.Text = "Currently logged users";

                TableRow currentRow = new TableRow();
                tblShowRegOn.Rows.Add(currentRow);
                int i = 0;
                foreach (User user in loggedUsers)
                {
                    if (i % 4 == 0)
                    {
                        currentRow = new TableRow();
                        tblShowRegOn.Rows.Add(currentRow);
                    }

                    TableCell userCell = new TableCell();
                    currentRow.Cells.Add(userCell);
                    userCell.HorizontalAlign = HorizontalAlign.Center;
                    userCell.Width = Unit.Percentage(25);

                    HyperLink userLink = CommonCode.UiTools.GetUserHyperLink(user);
                    userCell.Controls.Add(userLink);
                    if (businessUser.IsFromAdminTeam(user))
                    {
                        userLink.ForeColor = System.Drawing.Color.Red;
                    }

                    i++;
                }

                if (i > 4)
                {
                    firstCell.ColumnSpan = 4;
                }
                else
                {
                    firstCell.ColumnSpan = i;
                }
            }
            else
            {
                firstCell.Text = "Currently there are no logged users.";
            }
        }

    
    }
}
