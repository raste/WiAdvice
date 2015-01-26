﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Collections;

using DataAccess;
using BusinessLayer;

namespace UserInterface
{
    public partial class Administrators : BasePage
    {
        private User currentUser = null;        // used in all actions done by the user

        private EntitiesUsers userContext = new EntitiesUsers();
        private Entities objectContext = null;
        private BusinessLog businessLog = null;

        private void Page_Init(object sender, EventArgs e)
        {
            objectContext = CreateEntities();
            businessLog = new BusinessLog(CommonCode.CurrentUser.GetCurrentUserId(), Request.UserHostAddress);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            tbUsername.Attributes.Add("onblur", string.Format("JSCheckData('{0}','usernameReg','{1}','');", tbUsername.ClientID, lblCUsername.ClientID));
            tbPassword.Attributes.Add("onkeyup", string.Format("JSCheckData('{0}','passFormat','{1}','');", tbPassword.ClientID, lblCPassword.ClientID));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetNeedsToBeLogged();
            CheckUser();
            if (currentUser == null)
            {
                throw new CommonCode.UIException("currentUser is null");
            }
            ShowInfo();
            CommonCode.UiTools.HideUserNotificationPnl(pnlUsrNotification, lblUsrNotification, Page);
        }

        /// <summary>
        /// Checks if user is global admin or administrator , otherwise redirrectts to error page
        /// </summary>
        private void CheckUser()
        {
            BusinessUser businessUser = new BusinessUser();
            User currUser = GetCurrentUser(userContext, objectContext);

            if (currUser == null || !businessUser.IsAdminOrGlobalAdmin(currUser))
            {
                CommonCode.UiTools.RedirrectToErrorPage(Response, Session, "This Page is only for administrators.");
            }

            currentUser = currUser;
        }

        private void ShowInfo()
        {
            Title = "Register Administrator Page";

            // Fills radio list with registration options
            if (IsPostBack == false)
            {
                RadioButtonListRegOptions(currentUser);
            }

            BusinessSiteText siteText = new BusinessSiteText();
            SiteNews aboutExtended = siteText.GetSiteText(objectContext, "aboutAdmins");
            if (aboutExtended != null && aboutExtended.visible)
            {
                lblAbout.Text = aboutExtended.description; 
            }
            else
            {
                lblAbout.Text = "About Admin Page text not typed.";
            }

            // fills the admins table
            FillAdminsTable(currentUser);

            lblCUsername.Text = "";
            lblCPassword.Text = "";
        }

        /// <summary>
        /// fills the admins table
        /// </summary>
        private void FillAdminsTable(User currUser)
        {
            tblAdmins.Rows.Clear();

            BusinessUser businessUser = new BusinessUser();
            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditGlobalAdministrators))
            {
                lblAdmins.Visible = true;
                tblAdmins.Visible = true;

                IEnumerable<User> globals = businessUser.GetGlobals(userContext);
                if (globals.Count<User>() > 0)
                {
                    foreach (User user in globals)
                    {

                        TableRow newRow = new TableRow();

                        TableCell idCell = new TableCell();
                        idCell.Text = user.ID.ToString();
                        newRow.Cells.Add(idCell);

                        newRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(user));

                        TableCell typeCell = new TableCell();
                        typeCell.Text = user.type;
                        newRow.Cells.Add(typeCell);

                        TableCell dateCell = new TableCell();
                        dateCell.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(user.dateCreated);
                        newRow.Cells.Add(dateCell);

                        newRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(userContext, user.createdBy));

                        tblAdmins.Rows.Add(newRow);
                    }
                }
            }
            else
            {
                lblAdmins.Visible = false;
                tblAdmins.Visible = false;

            }

            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditAdministrators))
            {
                IEnumerable<User> admins = businessUser.GetAdministrators(userContext);

                if (admins.Count<User>() > 0)
                {
                    foreach (User admin in admins)
                    {
                        TableRow newRow = new TableRow();

                        TableCell idCell = new TableCell();
                        idCell.Text = admin.ID.ToString();
                        newRow.Cells.Add(idCell);

                        newRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(admin));

                        TableCell typeCell = new TableCell();
                        typeCell.Text = admin.type;
                        newRow.Cells.Add(typeCell);

                        TableCell dateCell = new TableCell();
                        dateCell.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(admin.dateCreated);
                        newRow.Cells.Add(dateCell);

                        newRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(userContext, admin.createdBy));

                        tblAdmins.Rows.Add(newRow);
                    }
                }
            }

            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditModerators))
            {
                IEnumerable<User> moderators = businessUser.GetModerators(userContext);
                if (moderators.Count<User>() > 0)
                {
                    foreach (User moder in moderators)
                    {
                        TableRow newRow = new TableRow();

                        TableCell idCell = new TableCell();
                        idCell.Text = moder.ID.ToString();
                        newRow.Cells.Add(idCell);

                        newRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(moder));

                        TableCell typeCell = new TableCell();
                        typeCell.Text = moder.type;
                        newRow.Cells.Add(typeCell);

                        TableCell dateCell = new TableCell();
                        dateCell.Text = CommonCode.UiTools.DateTimeToLocalShortDateString(moder.dateCreated);
                        newRow.Cells.Add(dateCell);

                        newRow.Cells.Add(CommonCode.UiTools.GetUserTableCell(userContext, moder.createdBy));

                        tblAdmins.Rows.Add(newRow);
                    }
                }
            }

        }

        /// <summary>
        /// Fills radio list with registration options
        /// </summary>
        private void RadioButtonListRegOptions(User currUser)
        {
            rblRegOptions.Items.Clear();
            BusinessUser businessUser = new BusinessUser();


            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditGlobalAdministrators))
            {
                ListItem globalItem = new ListItem();
                globalItem.Text = "global administrator ";
                globalItem.Value = "1";
                rblRegOptions.Items.Add(globalItem);
            }

            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditAdministrators))
            {
                ListItem adminItem = new ListItem();
                adminItem.Text = "administrator ";
                adminItem.Value = "2";
                rblRegOptions.Items.Add(adminItem);
            }

            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditModerators))
            {
                ListItem moderItem = new ListItem();
                moderItem.Text = "moderator ";
                moderItem.Value = "3";
                rblRegOptions.Items.Add(moderItem);
            }

            if (businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditGlobalAdministrators)
                && businessUser.CanAdminDo(userContext, currUser, AdminRoles.EditUsers))
            {
                ListItem userItem = new ListItem();
                userItem.Text = "user ";
                userItem.Value = "4";
                rblRegOptions.Items.Add(userItem);

                ListItem writerItem = new ListItem();
                writerItem.Text = "writer ";
                writerItem.Value = "5";
                rblRegOptions.Items.Add(writerItem);

            }

            if (rblRegOptions.Items.Count > 0)
            {
                rblRegOptions.SelectedIndex = 0;

                rblRegOptions.Visible = true;
                btnContinue.Visible = true;

                lblRegAdm.Text = "Register new administrator : ";
            }
            else
            {
                rblRegOptions.Visible = false;
                btnContinue.Visible = false;

                lblRegAdm.Text = "You can't register administrators! ";
            }
        }

        /// <summary>
        /// Fills Check Box List Roles with roles
        /// </summary>
        private void CheckBoxListRoles()
        {
            cbRoles.Items.Clear();

            int selected = -1;
            if (!int.TryParse(rblRegOptions.SelectedValue, out selected))
            {
                throw new CommonCode.UIException(string.Format("Couldnt parse rblRegOptions.SelectedValue to Int , user ID = {0}", currentUser.ID));
            }

            switch (selected) // 1 global, 2 admin, 3 moderator, 4 user, 5 writer
            {
                case 1:

                    // 1 reg admin , 2 modif cat , 3 modif comp , 4 modif prod , 5 modif comm , 6 modif users 

                    ListItem adminItem = new ListItem();
                    adminItem.Text = "register admins";
                    cbRoles.Items.Add(adminItem);

                    ListItem catItem = new ListItem();
                    catItem.Text = "modify all categories";
                    cbRoles.Items.Add(catItem);

                    ListItem compItem = new ListItem();
                    compItem.Text = "modify all companies";
                    cbRoles.Items.Add(compItem);
                    cbRoles.Items[2].Enabled = false;

                    ListItem prodItem = new ListItem();
                    prodItem.Text = "modify all products";
                    cbRoles.Items.Add(prodItem);
                    cbRoles.Items[3].Enabled = false;

                    ListItem commItem = new ListItem();
                    commItem.Text = "modify all comments";
                    cbRoles.Items.Add(commItem);
                    cbRoles.Items[4].Enabled = false;

                    ListItem userItem = new ListItem();
                    userItem.Text = "modify all users";
                    cbRoles.Items.Add(userItem);


                    for (int i = 0; i <= 5; i++)
                    {
                        cbRoles.Items[i].Selected = true;
                    }

                    break;
                case 2:

                    // 0 reg moders , 1 modif comp , 2 modif prod , 3 modif comm , 4 modif users

                    ListItem moderItem = new ListItem();
                    moderItem.Text = "register moderators";
                    cbRoles.Items.Add(moderItem);

                    ListItem comp2Item = new ListItem();
                    comp2Item.Text = "modify all companies";
                    cbRoles.Items.Add(comp2Item);
                    cbRoles.Items[1].Enabled = false;

                    ListItem prod2Item = new ListItem();
                    prod2Item.Text = "modify all products";
                    cbRoles.Items.Add(prod2Item);
                    cbRoles.Items[2].Enabled = false;

                    ListItem comm2Item = new ListItem();
                    comm2Item.Text = "modify all comments";
                    cbRoles.Items.Add(comm2Item);
                    cbRoles.Items[3].Enabled = false;

                    ListItem user2Item = new ListItem();
                    user2Item.Text = "modify all users";
                    cbRoles.Items.Add(user2Item);

                    for (int i = 0; i <= 4; i++)
                    {
                        cbRoles.Items[i].Selected = true;
                    }

                    break;
                case 3:

                    // 0 comm modif , 1 user modif
                    ListItem comm3Item = new ListItem();
                    comm3Item.Text = "modify all comments";
                    cbRoles.Items.Add(comm3Item);
                    cbRoles.Items[0].Enabled = false;

                    ListItem user3Item = new ListItem();
                    user3Item.Text = "modify all users";
                    cbRoles.Items.Add(user3Item);
                    cbRoles.Items[1].Enabled = false;

                    for (int i = 0; i <= 1; i++)
                    {
                        cbRoles.Items[i].Selected = true;
                    }

                    break;

                case 4:

                    // items arent needed
                    lblMail.Visible = true;
                    tbUserEmail.Visible = true;

                    break;

                case 5:

                    // items arent needed

                    break;

                default:
                    throw new CommonCode.UIException(string.Format("index = {0} was selected from cbRoles which is invalid", selected));
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            if (rblRegOptions.SelectedIndex >= 0 && rblRegOptions.SelectedIndex <= 4)
            {
                CheckBoxListRoles(); 

                rblRegOptions.Enabled = false;
                btnContinue.Enabled = false;

                pnlReg.Visible = true;

                lblUsernameRules.Text = string.Format("Username can contain only letters A-Z or numbers 0-9 and up to 4 words <br/> (1 space between words). Username length must be between {0}-{1}."
                    , Configuration.UsersMinUserNameLength, Configuration.UsersMaxUserNameLength);
                lblPasswordRules.Text = string.Format
                    ("Password can contain only letters A-Z or numbers 0-9. <br />Password length must be between {0}-{1}."
                    , Configuration.UsersMinPasswordLength, Configuration.UsersMaxPasswordLength);

            }
            else
            {
                throw new CommonCode.UIException(string.Format("rblRegOptions.SelectedIndex = {0} is invalid , user = {1}",
                    rblRegOptions.SelectedIndex, currentUser.ID));
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            BusinessUser businessUser = new BusinessUser();
            if (businessUser.IsAdminOrGlobalAdmin(currentUser))
            {
                String error = "";
                lblError.Visible = true;

                if (ValidateCurrPasswordAndNewUserPassword(out error) == true)
                {
                    int selected = -1;
                    if (!int.TryParse(rblRegOptions.SelectedValue, out selected))
                    {
                        throw new CommonCode.UIException(string.Format("Couldnt parse rblRegOptions.SelectedValue to int , user id = {0}", currentUser.ID));
                    }

                    int items = cbRoles.Items.Count;

                    Boolean allchecked = true;

                    for (int i = 0; i < items; i++)
                    {
                        if (!cbRoles.Items[i].Selected)
                        {
                            allchecked = false;
                            break;
                        }
                    }

                    string notUsed = "none";
                    string typeRegistered = string.Empty;
                    BusinessUserActions businessUserActions = new BusinessUserActions();

                    switch (selected)  // 1 global, 2 admin, 3 moderator, 4 user, 5 writer
                    {
                        case 1:

                            User newGlobal = new User();
                            newGlobal.username = tbUsername.Text;
                            newGlobal.password = businessUser.GetHashed(tbPassword.Text);
                            newGlobal.email = null;
                            newGlobal.dateCreated = DateTime.UtcNow;
                            newGlobal.lastLogIn = newGlobal.dateCreated;
                            newGlobal.visible = true;
                            newGlobal.type = "global";
                            newGlobal.createdBy = currentUser.ID;
                            newGlobal.userData = null;
                            newGlobal.rating = 0;

                            businessUser.AddUser(userContext, objectContext, newGlobal, businessLog, notUsed, notUsed, true, currentUser);

                            if (allchecked)
                            {
                                businessUserActions.AddUserRolesForNewGlobalAdministrator(objectContext, userContext, newGlobal.ID, currentUser.ID, businessLog);
                            }
                            else
                            {
                                // 0 reg admin , 1 modif cat , 2 modif comp , 3 modif prod , 4 modif comm , 5 modif users 

                                if (cbRoles.Items[0].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newGlobal, currentUser, AdminRoles.EditGlobalAdministrators, businessLog);
                                    businessUserActions.AddAdminRole(userContext, objectContext, newGlobal, currentUser, AdminRoles.EditAdministrators, businessLog);
                                    businessUserActions.AddAdminRole(userContext, objectContext, newGlobal, currentUser, AdminRoles.EditModerators, businessLog);
                                }

                                if (cbRoles.Items[1].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newGlobal, currentUser, AdminRoles.EditCategories, businessLog);
                                }

                                if (cbRoles.Items[2].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newGlobal, currentUser, AdminRoles.EditCompanies, businessLog);
                                }

                                if (cbRoles.Items[3].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newGlobal, currentUser, AdminRoles.EditProducts, businessLog);
                                }

                                if (cbRoles.Items[4].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newGlobal, currentUser, AdminRoles.EditComments, businessLog);
                                }

                                if (cbRoles.Items[5].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newGlobal, currentUser, AdminRoles.EditUsers, businessLog);
                                }
                            }

                            typeRegistered = "Global administrator added!";

                            break;
                        case 2:

                            User newAdmin = new User();
                            newAdmin.username = tbUsername.Text;
                            newAdmin.password = businessUser.GetHashed(tbPassword.Text);
                            newAdmin.email = null;
                            newAdmin.dateCreated = DateTime.UtcNow;
                            newAdmin.lastLogIn = newAdmin.dateCreated;
                            newAdmin.visible = true;
                            newAdmin.type = "administrator";
                            newAdmin.createdBy = currentUser.ID;
                            newAdmin.userData = null;
                            newAdmin.rating = 0;

                            businessUser.AddUser(userContext, objectContext, newAdmin, businessLog, notUsed, notUsed, true, currentUser);

                            if (allchecked)
                            {
                                businessUserActions.AddUserRolesForNewAdministrator(userContext, objectContext, newAdmin.ID, currentUser.ID, businessLog);
                            }
                            else
                            {
                                // 0 reg moders , 1 modif comp , 2 modif prod , 3 modif comm , 4 modif users

                                if (cbRoles.Items[0].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newAdmin, currentUser, AdminRoles.EditModerators, businessLog);
                                }

                                if (cbRoles.Items[1].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newAdmin, currentUser, AdminRoles.EditCompanies, businessLog);
                                }

                                if (cbRoles.Items[2].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newAdmin, currentUser, AdminRoles.EditProducts, businessLog);
                                }

                                if (cbRoles.Items[3].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newAdmin, currentUser, AdminRoles.EditComments, businessLog);
                                }

                                if (cbRoles.Items[4].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newAdmin, currentUser, AdminRoles.EditUsers, businessLog);
                                }
                            }

                            Tools.Save(objectContext);

                            typeRegistered = "Administrator added!";

                            break;
                        case 3:

                            User newModer = new User();
                            newModer.username = tbUsername.Text;
                            newModer.password = businessUser.GetHashed(tbPassword.Text);
                            newModer.email = null;
                            newModer.dateCreated = DateTime.UtcNow;
                            newModer.lastLogIn = newModer.dateCreated;
                            newModer.visible = true;
                            newModer.type = "moderator";
                            newModer.createdBy = currentUser.ID;
                            newModer.userData = null;
                            newModer.rating = 0;

                            businessUser.AddUser(userContext, objectContext, newModer, businessLog, notUsed, notUsed, true, currentUser);
                            Tools.Save(objectContext);

                            if (allchecked)
                            {
                                businessUserActions.AddUserRolesForNewModerator(userContext, objectContext, newModer.ID, currentUser.ID, businessLog);
                            }
                            else
                            {
                                // 0 comm modif , 1 user modif

                                if (cbRoles.Items[0].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newModer, currentUser, AdminRoles.EditComments, businessLog);
                                }

                                if (cbRoles.Items[1].Selected)
                                {
                                    businessUserActions.AddAdminRole(userContext, objectContext, newModer, currentUser, AdminRoles.EditUsers, businessLog);
                                }
                            }

                            Tools.Save(objectContext);

                            typeRegistered = "Moderator added!";

                            break;

                        case 4:

                            if (businessUser.IsGlobalAdministrator(currentUser) == false ||
                                businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditUsers) == false)
                            {
                                throw new CommonCode.UIException(string.Format("Admin id : {0} cannot register new Users/Writers, because he is not global admin or he cannot edit all users."));
                            }

                            if (CommonCode.Validate.ValidateEmailAdress(tbUserEmail.Text, out error) == true)
                            {
                                if (businessUser.CountRegisteredUsersWithMail(userContext, tbUserEmail.Text)
                                    < Configuration.MaximumNumberOfUsersRegisteredWithMail)
                                {
                                    string secQuestion = Tools.GetRandomNumberIn16Bit(15);
                                    string secAnswer = Tools.GetRandomNumberIn16Bit(15);

                                    businessUser.RegisterUser(userContext, objectContext, tbUsername.Text, tbPassword.Text, tbUserEmail.Text,
                                        businessLog, secQuestion, secAnswer, true, currentUser);

                                    typeRegistered = "User added!";
                                }
                                else
                                {
                                    lblError.Text = "Maximum number registered with this mail reached!";
                                    return;
                                }
                            }
                            else
                            {
                                lblError.Text = error;
                                return;
                            }

                            break;

                        case 5:

                            if (businessUser.IsGlobalAdministrator(currentUser) == false ||
                               businessUser.CanAdminDo(userContext, currentUser, AdminRoles.EditUsers) == false)
                            {
                                throw new CommonCode.UIException(string.Format("Admin id : {0} cannot register new Users/Writers, because he is not global admin or he cannot edit all users."));
                            }

                            string secWQuestion = Tools.GetRandomNumberIn16Bit(15);
                            string secWAnswer = Tools.GetRandomNumberIn16Bit(15);

                            businessUser.RegisterUser(userContext, objectContext, tbUsername.Text, tbPassword.Text, string.Empty,
                                businessLog, secWQuestion, secWAnswer, true, currentUser);

                            typeRegistered = "Writer added!";

                            break;

                        default:
                            throw new CommonCode.UIException(string.Format("Selected index = {0} from blRegOptions.SelectedValue " +
                                "is invalid , user = {1}", selected, currentUser.ID));
                    }

                    lblError.Visible = false;
                    Discard();
                    CommonCode.UiTools.ShowUserNotificationPnl(pnlUsrNotification, lblUsrNotification, typeRegistered);
                    FillAdminsTable(currentUser);
                }

                lblError.Text = error;
            }
            else
            {
                throw new CommonCode.UIException(string.Format("User id = {0} cannot register admins", currentUser.ID));
            }
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Discard();
        }

        public void Discard()
        {
            rblRegOptions.Enabled = true;
            btnContinue.Enabled = true;

            tbUsername.Text = "";
            tbUserPass.Text = "";
            tbPassword.Text = "";
            tbPassword2.Text = "";
            lblError.Visible = false;

            tbUserEmail.Text = string.Empty;
            lblMail.Visible = false;
            tbUserEmail.Visible = false;

            pnlReg.Visible = false;
        }

        private bool ValidateCurrPasswordAndNewUserPassword(out string error)
        {
            bool result = false;
            error = string.Empty;

            if (CommonCode.Validate.ValidateCurrUserPassword(currentUser, tbUserPass.Text, out error))
            {

                string username = tbUsername.Text;

                if (CommonCode.Validate.ValidateUserName(objectContext, ref username, out error))
                {
                    if (CommonCode.Validate.ValidatePassword(tbPassword.Text, out error))
                    {
                        if (CommonCode.Validate.ValidateRepeatPassword(username, tbPassword.Text, tbPassword2.Text, out error))
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        [WebMethod]
        public static string CheckData(string text, string type, string notUsed)
        {
            string error = "";

            CommonCode.WebMethods.ValidateUserInput(text, type, "", out error);

            return error;

        }
    }
}
