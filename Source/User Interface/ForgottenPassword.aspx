<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ForgottenPassword.aspx.cs" Inherits="UserInterface.ForgottenPassword" Theme="MainTheme" %>

<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1 {
            width: 100%;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div style="margin-top: 10px; margin-bottom: 10px;">
        <div class="brb">
            <div class="blb">
                <div class="blhrgreen">
                    <div class="contentBoxBottomHr">


                        <div style="text-align: center">
                            <asp:Label ID="lblLoggedSucc" runat="server"
                                Text="Logged in" Visible="False" CssClass="textHeader"></asp:Label>
                        </div>

                        <asp:Panel ID="pnlRetrievePass" runat="server">

                            <div class="sectionTextHeader">
                                <asp:Label ID="lblChangePass" runat="server" Text="Change password"></asp:Label>
                            </div>
                            <br />

                            <ul>
                                <li>
                                    <asp:Label ID="lblRetrPass1" runat="server" Text="Retrieve password reset link on your email by writing username and email adress"></asp:Label>
                                    &nbsp;<asp:Button ID="btnProceedEmail" runat="server" OnClick="btnProceedEmail_Click" Text="Proceed" />
                                    <br />
                                    <asp:Label ID="lblNote" runat="server" Text="(NOTE : Only for accounts registered with email.)"></asp:Label>
                                    <br />
                                    <br />
                                </li>
                                <li>

                                    <asp:Label ID="lblRetrPass2" runat="server" Text="Reset your password by answering your secret question."></asp:Label>
                                    &nbsp;<asp:Button ID="btnProceedSecQnA" runat="server" Text="Proceed" OnClick="btnProceedSecQnA_Click" />

                                </li>
                            </ul>


                            <asp:Panel ID="pnlByEmail" runat="server" Visible="False"
                                DefaultButton="btnSendMail">


                                <div class="divBottomHr" style="width: 750px; margin-top: 10px; margin-bottom: 10px;">

                                    <img src="images/SiteImages/horL.png" align="left" />
                                    <img src="images/SiteImages/horR.png" align="right" />

                                </div>

                                <table class="style1" style="width: 100%">
                                    <tr>
                                        <td style="width: 150px; text-align: right;">
                                            <asp:Label ID="lblUsername1" runat="server" Text="username :"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbMUsername" runat="server" CssClass="margins1px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvMusername" runat="server"
                                                ControlToValidate="tbMUsername" ErrorMessage="*" ValidationGroup="15"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">
                                            <asp:Label ID="lblMail" runat="server" Text="email adress :"></asp:Label>
                                        </td>
                                        <td>
                                            <div class="accordionsDivPadding">
                                                <asp:TextBox ID="tbMemail" runat="server" CssClass="margins1px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvMemail" runat="server"
                                                    ControlToValidate="tbMemail" ErrorMessage="*" ValidationGroup="15"></asp:RequiredFieldValidator>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">&nbsp;</td>
                                        <td>
                                            <cc1:CaptchaControl ID="ccMail" runat="server" CaptchaHeight="60"
                                                CaptchaLength="3" CaptchaMaxTimeout="3600" Height="60px" Width="180px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <div class="accordionsDivPadding">
                                                <asp:TextBox ID="tbMailCaptcha" runat="server" Width="179px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvMail" runat="server"
                                                    ControlToValidate="tbMailCaptcha" ErrorMessage="*" ValidationGroup="15"></asp:RequiredFieldValidator>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <asp:Button ID="btnSendMail" runat="server" OnClick="btnSendMail_Click"
                                                Text="Submit" ValidationGroup="15" />
                                            <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click"
                                                Text="Cancel" />
                                            &nbsp;<asp:PlaceHolder ID="phMail" runat="server" Visible="False"></asp:PlaceHolder>
                                        </td>
                                    </tr>
                                </table>

                            </asp:Panel>
                            <asp:Panel ID="pnlBySecQnA" runat="server" Visible="False"
                                DefaultButton="btnSecretStep2">

                                <div class="divBottomHr" style="width: 650px; margin-top: 10px; margin-bottom: 10px;">

                                    <img src="images/SiteImages/horL.png" align="left" />
                                    <img src="images/SiteImages/horR.png" align="right" />

                                </div>

                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                                <asp:Label ID="lblUsername2" runat="server" Text="username :"></asp:Label>
                                <asp:TextBox ID="tbSusername" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvSusername" runat="server"
                                    ControlToValidate="tbSusername" ErrorMessage="*" ValidationGroup="16"></asp:RequiredFieldValidator>
                                <asp:Button ID="btnSecretStep2" runat="server" OnClick="btnSecretStep2_Click"
                                    Text="Continue" ValidationGroup="16" />
                                &nbsp;<asp:Button ID="btnCancel1" runat="server" OnClick="btnCancel1_Click"
                                    Text="Cancel" />
                                &nbsp;<asp:PlaceHolder ID="phSecStep1" runat="server" Visible="False"></asp:PlaceHolder>
                                <asp:Panel ID="pnlSecQnAStep2" runat="server" Visible="False"
                                    DefaultButton="btnLogIn">
                                    <br />

                                    <table class="style1" style="width: 100%;">
                                        <tr>
                                            <td style="width: 150px; text-align: right;">
                                                <asp:Label ID="lblQuestion" runat="server" Text="question :"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblSecQustion" runat="server" Text="Secret Question"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right;">
                                                <asp:Label ID="lblAnswer" runat="server" Text="answer :"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="tbSecAnswer" runat="server" Columns="30" CssClass="margins1px"
                                                    TextMode="Password"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvSanswer" runat="server"
                                                    ControlToValidate="tbSecAnswer" ErrorMessage="*" ValidationGroup="17"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right;">&nbsp;</td>
                                            <td>
                                                <div class="accordionsDivPadding">
                                                    <cc1:CaptchaControl ID="ccSecretQuest" runat="server" CaptchaHeight="60"
                                                        CaptchaLength="3" CaptchaMaxTimeout="3600" Height="60px" Width="180px" />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="tbSecCaptcha" runat="server" Width="179px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvSecCaptcha" runat="server"
                                                    ControlToValidate="tbSecCaptcha" ErrorMessage="*" ValidationGroup="17"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>
                                                <div class="accordionsDivPadding">
                                                    <asp:Button ID="btnLogIn" runat="server" OnClick="btnLogIn_Click"
                                                        Text="Continue" ValidationGroup="17" />
                                                    <asp:Button ID="btnCancel2" runat="server" OnClick="btnCancel2_Click"
                                                        Text="Cancel" />
                                                    &nbsp;<asp:PlaceHolder ID="phSecStep2" runat="server" Visible="False"></asp:PlaceHolder>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>

                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>


                    </div>


                </div>
            </div>
        </div>
    </div>




    <asp:Panel ID="pnlNotVisible" runat="server" BackColor="#FFCCFF"
        Visible="False">
        <br />
        <asp:Label ID="lblNotif" runat="server" Text="Notification" CssClass="errors"></asp:Label>
        <br />
    </asp:Panel>



</asp:Content>
