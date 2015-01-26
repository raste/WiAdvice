<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="UserInterface.Registration" MasterPageFile="MasterPage.Master" Theme="MainTheme" %>

<%@ Register Assembly="System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.DynamicData" TagPrefix="cc1" %>

<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc2" %>

<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc3" %>

<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">

    <br />

    <div style="text-align: center">
        <asp:Label ID="lblReg" runat="server" Text="Registered:" Visible="False"
            CssClass="textHeader"></asp:Label>
    </div>

    <asp:Panel ID="regPanel" runat="server" DefaultButton="btnSubmit">


        <div>
            <div class="brb2">
                <div class="blb2">
                    <div class="blhr">
                        <div class="contentBoxBottomHr">


                            <asp:Label ID="lblRegAbout" runat="server" Text="About Register Page"></asp:Label>


                        </div>

                        <img src="images/SiteImages/horL.png" align="left" />
                        <img src="images/SiteImages/horR.png" align="right" />

                    </div>
                </div>
            </div>
        </div>


        <div style="margin-bottom: 10px;">
            <div class="trb2">
                <div class="tlb2">


                    <div class="brb2">
                        <div class="blb2">

                            <div class="tlhr">

                                <img src="images/SiteImages/horL.png" align="left" />
                                <img src="images/SiteImages/horR.png" align="right" />

                                <div class="blhr2">

                                    <div class="contentBoxTopBottomHr">




                                        <table class="regBGR" style="width: 100%;">
                                            <tr>
                                                <td align="right" style="width: 200px">&nbsp;</td>
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Label ID="lblRegForm" runat="server" CssClass="sectionTextHeader"
                                                        Font-Size="X-Large" ForeColor="#101915" Text="Registration Form : "></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 200px">&nbsp;</td>
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 200px">
                                                    <asp:Label ID="lblUsername" runat="server" Text="User name : "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbUsername" runat="server" Width="140px"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="tbUsername_FilteredTextBoxExtender"
                                                        runat="server" FilterType="Custom, Numbers, UppercaseLetters, LowercaseLetters"
                                                        TargetControlID="tbUsername" ValidChars=" ">
                                                    </ajaxToolkit:FilteredTextBoxExtender>
                                                    <asp:RequiredFieldValidator ID="rfvName" runat="server"
                                                        ControlToValidate="tbUsername" ErrorMessage="*" ValidationGroup="4"></asp:RequiredFieldValidator>
                                                    <asp:Label ID="lblCUser" runat="server" CssClass="smallerText" Font-Bold="True"
                                                        ForeColor="#C02E29" Text="CheckUsername"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 200px">&nbsp;</td>
                                                <td>
                                                    <asp:Label ID="lblUsernameRules" runat="server" Text="Username rules"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 200px">&nbsp;&nbsp;</td>
                                                <td>&nbsp;&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblPassword" runat="server" Text="Password : "></asp:Label>
                                                </td>
                                                <td>

                                                    <asp:TextBox ID="tbPassword" runat="server" TextMode="Password" Columns="20"
                                                        Width="140px"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="tbPassword_FilteredTextBoxExtender"
                                                        runat="server" FilterType="Custom, Numbers, UppercaseLetters, LowercaseLetters" TargetControlID="tbPassword">
                                                    </ajaxToolkit:FilteredTextBoxExtender>
                                                    <ajaxToolkit:PasswordStrength ID="tbPassword_PasswordStrength" runat="server"
                                                        HelpHandlePosition="RightSide" MinimumLowerCaseCharacters="2"
                                                        MinimumNumericCharacters="2" MinimumUpperCaseCharacters="2"
                                                        PreferredPasswordLength="10" RequiresUpperAndLowerCaseCharacters="True"
                                                        TargetControlID="tbPassword" TextCssClass="passwordStrength">
                                                    </ajaxToolkit:PasswordStrength>
                                                    <asp:RequiredFieldValidator ID="rfvPass" runat="server"
                                                        ControlToValidate="tbPassword" ValidationGroup="4" ErrorMessage="*"></asp:RequiredFieldValidator>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td>
                                                    <asp:Label ID="lblCPassword" runat="server" Text="CheckPassword"
                                                        CssClass="smallerText" Font-Bold="True" ForeColor="#C02E29"></asp:Label>
                                                    &nbsp;&nbsp;&nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td>
                                                    <asp:Label ID="lblPassRules" runat="server" Text="Password rules"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;&nbsp;</td>
                                                <td>&nbsp;&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblRPassword" runat="server" Text="Repeat password : "></asp:Label>

                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbRPassword" runat="server" TextMode="Password" Width="140px"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="tbRPassword_FilteredTextBoxExtender"
                                                        runat="server" FilterType="Custom, Numbers, UppercaseLetters, LowercaseLetters" TargetControlID="tbRPassword">
                                                    </ajaxToolkit:FilteredTextBoxExtender>
                                                    <asp:RequiredFieldValidator ID="rfvRepPass" runat="server"
                                                        ControlToValidate="tbRPassword" ValidationGroup="4" ErrorMessage="*"></asp:RequiredFieldValidator>
                                                    &nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;&nbsp;</td>
                                                <td>&nbsp;&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblEmail" runat="server" Text="Email : "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbEmail" runat="server" Columns="30"></asp:TextBox>
                                                    &nbsp;&nbsp;
                    <asp:Label ID="lblCEmail" runat="server" CssClass="smallerText"
                        Font-Bold="True" ForeColor="#C02E29" Text="CheckEmail"></asp:Label>
                                                    &nbsp;&nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td>
                                                    <asp:Label ID="lblMailRules" runat="server" Text="Mail Rules"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblSecQuest" runat="server" Text="Secret question : "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbSecQuestion" runat="server" Columns="30"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvSecQuestion" runat="server"
                                                        ControlToValidate="tbSecQuestion" ValidationGroup="4" ErrorMessage="*"></asp:RequiredFieldValidator>
                                                    <asp:Label ID="lblQuestInfo" runat="server" Text="This is needed in case you forgot your password."></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblSecAnswer" runat="server" Text="Question`s answer : "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbSecAnswer" runat="server" Columns="30"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvSecQuestAnswer" runat="server"
                                                        ControlToValidate="tbSecAnswer" ValidationGroup="4" ErrorMessage="*"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td>
                                                    <asp:Label ID="lblSecretRules" runat="server" Text="QnA rules"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div style="margin-left: 50px; text-align: center;">
                                                        <asp:Label ID="lblCaptchaInfo" runat="server" Text="Fill in the letters"></asp:Label>
                                                        <br />
                                                        <asp:Label ID="lblCaptchaInfo2" runat="server" Text="you see on the image."></asp:Label>
                                                    </div>
                                                </td>
                                                <td>
                                                    <cc3:CaptchaControl ID="ccReg" runat="server" CaptchaHeight="60"
                                                        CaptchaLength="3" CaptchaMaxTimeout="3600" Height="60px" Width="180px"
                                                        BackColor="#FFFFE1" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">&nbsp;</td>
                                                <td>
                                                    <asp:TextBox ID="tbCaptcha" runat="server" CssClass="marginTop3x" Width="179px"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvCaptcha" runat="server"
                                                        ControlToValidate="tbCaptcha" ValidationGroup="4">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center"></td>
                                                <td>
                                                    <asp:CheckBox ID="cbAgreeTerms" runat="server" />
                                                    <span style="font-size: large">
                                                        <asp:Label ID="lblIAgree" runat="server" Text="I agree with the"></asp:Label>&nbsp;<asp:HyperLink ID="hlTerms" runat="server" Target="_blank"
                                                            Style="color: #C02E29;" NavigateUrl="~/Rules.aspx" Font-Names="Trebuchet MS">terms</asp:HyperLink>
                                                        <asp:Label ID="lblIAgree2" runat="server" Text="to use this site."></asp:Label>
                                                    </span>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td align="right" valign="top">&nbsp;
                                                </td>
                                                <td align="left" style="padding-top: 5px;">
                                                    <cc2:DecoratedButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click"
                                                        Text="Submit" ValidationGroup="4" />
                                                    &nbsp;<asp:Label ID="lblError" runat="server" Text="Error : "
                                                        Visible="False" CssClass="errors"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="top">&nbsp;</td>
                                                <td align="left">&nbsp;</td>
                                            </tr>
                                        </table>


                                    </div>

                                    <img src="images/SiteImages/horL.png" align="left" />
                                    <img src="images/SiteImages/horR.png" align="right" />

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


    </asp:Panel>


    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FFCCFF">
        <p>
            <asp:Label ID="lblCheckUserName" runat="server" Text="Check username : "></asp:Label>
        </p>
        <p>
            <asp:Button ID="btnSubmitOLD" runat="server" OnClick="btnSubmit_Click"
                Text="Submit" />
        </p>
        <p>
            &nbsp;
        </p>
        <asp:UpdatePanel ID="upCheckUserName" runat="server">
            <ContentTemplate>
                <asp:Button ID="btnCheckUserName" runat="server" Text="Check" />
                &nbsp;<asp:Label ID="lblUserNameChecked" runat="server" Text="result"
                    Visible="False"></asp:Label>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>


</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
</asp:Content>


