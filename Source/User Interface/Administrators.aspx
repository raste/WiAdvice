<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Administrators.aspx.cs" Inherits="UserInterface.Administrators" MasterPageFile="~/MasterPage.Master" Theme="MainTheme" %>

<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">


    <br />
    <asp:Label ID="lblAbout" runat="server" Text="About text"></asp:Label>
    <p>
        <asp:Label ID="lblRegAdm" runat="server" Text="Register new administrator : " CssClass="textHeader"></asp:Label>
    </p>
    <asp:RadioButtonList ID="rblRegOptions" runat="server"
        RepeatLayout="Flow" RepeatDirection="Horizontal">
    </asp:RadioButtonList>
    &nbsp;&nbsp;<cc1:DecoratedButton ID="btnContinue" runat="server"
        OnClick="btnContinue_Click" Text="Continue" />
    <br />
    <br />
    <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False"
        CssClass="usrNotificationPnl">
        <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="pnlReg" runat="server" Visible="False" CssClass="editBGR"
        DefaultButton="btnSubmit">
        <hr />
        <table style="width: 100%;">
            <tr>
                <td rowspan="9" valign="top" style="vertical-align: text-top; width: 200px;">
                    <asp:CheckBoxList ID="cbRoles" runat="server">
                    </asp:CheckBoxList>
                </td>
                <td align="right" style="width: 150px">
                    <asp:Label ID="lblUserPass" runat="server" Text="Your Password :"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbUserPass" runat="server" TextMode="Password" Width="140px"
                        ValidationGroup="3"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPass" runat="server"
                        ControlToValidate="tbUserPass" ValidationGroup="3"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="style17" align="right">
                    <asp:Label ID="lblUsername" runat="server" Text="Username :"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbUsername" runat="server" Columns="20" Width="140px"
                        ValidationGroup="3"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="tbUsername_FilteredTextBoxExtender"
                        runat="server"
                        FilterType="Custom, Numbers, UppercaseLetters, LowercaseLetters"
                        TargetControlID="tbUsername" ValidChars=" ">
                    </ajaxToolkit:FilteredTextBoxExtender>
                    <asp:RequiredFieldValidator ID="rfvUsername" runat="server"
                        ControlToValidate="tbUsername" ValidationGroup="3"></asp:RequiredFieldValidator>
                    &nbsp;<asp:Label ID="lblCUsername" runat="server" Text="CheckUsername"
                        CssClass="smallerText" Font-Bold="True" ForeColor="#C02E29"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right" class="style17">&nbsp;</td>
                <td>
                    <asp:Label ID="lblUsernameRules" runat="server" Text="Username Rules"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="lblPassword" runat="server" Text="Password : "></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbPassword" runat="server" TextMode="Password" Width="140px"
                        ValidationGroup="3"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="tbPassword_FilteredTextBoxExtender"
                        runat="server" FilterType="Custom, Numbers, UppercaseLetters, LowercaseLetters" TargetControlID="tbPassword">
                    </ajaxToolkit:FilteredTextBoxExtender>
                    <ajaxToolkit:PasswordStrength ID="tbPassword_PasswordStrength" runat="server"
                        MinimumLowerCaseCharacters="2" MinimumNumericCharacters="2"
                        MinimumUpperCaseCharacters="2" PreferredPasswordLength="10"
                        RequiresUpperAndLowerCaseCharacters="True" TargetControlID="tbPassword"
                        TextCssClass="passwordStrength">
                    </ajaxToolkit:PasswordStrength>
                    &nbsp;<asp:RequiredFieldValidator ID="rfvNewPass" runat="server"
                        ControlToValidate="tbPassword" ValidationGroup="3"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td align="right">&nbsp;</td>
                <td>
                    <asp:Label ID="lblCPassword" runat="server" Text="CheckPassword"
                        CssClass="errors" Font-Size="Small"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right">&nbsp;</td>
                <td>
                    <asp:Label ID="lblPasswordRules" runat="server" Text="Password Rules"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="lblRepPassword" runat="server" Text="Repeat password : "></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbPassword2" runat="server" TextMode="Password" Width="140px"
                        ValidationGroup="3"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="tbPassword2_FilteredTextBoxExtender"
                        runat="server" FilterType="Custom, Numbers, UppercaseLetters, LowercaseLetters" TargetControlID="tbPassword2">
                    </ajaxToolkit:FilteredTextBoxExtender>
                    <asp:RequiredFieldValidator ID="rfvRepPass" runat="server"
                        ControlToValidate="tbPassword2" ValidationGroup="3"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="lblMail" runat="server" Text="Email :" Visible="False"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbUserEmail" runat="server" Visible="False" Width="140px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2" valign="top">
                    <cc1:DecoratedButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click"
                        Text="Submit" ValidationGroup="3" />
                    &nbsp;<cc1:DecoratedButton ID="btnCancel" runat="server" OnClick="btnCancel_Click"
                        Text="Cancel" />
                    &nbsp;<asp:Label ID="lblError" runat="server" Text="ERROR : "
                        Visible="False" CssClass="errors"></asp:Label>
                </td>
            </tr>
        </table>
        <hr />
    </asp:Panel>
    <p>
        <asp:Label ID="lblAdmins" runat="server" Text="Administrators table : "
            Visible="False"></asp:Label>
    </p>
    <p>
        <asp:Table ID="tblAdmins" runat="server" BorderWidth="2px" CellPadding="2"
            CellSpacing="2" GridLines="Both" Visible="False" BorderColor="#CCCCCC"
            Width="100%">
        </asp:Table>
    </p>
    <p>
    </p>



    <asp:Panel ID="pnlHidden" runat="server" BackColor="#FF99FF" Visible="False">
        <br />
        <asp:Button ID="btnSubmitOld" runat="server" OnClick="btnSubmit_Click"
            Text="Submit" />
        &nbsp;
        <asp:Button ID="btnCancelOld" runat="server" OnClick="btnCancel_Click"
            Style="height: 26px" Text="Cancel" />
        &nbsp;
        <asp:Button ID="btnContinueOLd" runat="server" OnClick="btnContinue_Click"
            Text="Continue" />
        <br />
        <br />
        <br />
    </asp:Panel>



</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">

    <style type="text/css">
        .style17 {
            height: 26px;
        }
    </style>

</asp:Content>

