<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AboutUs.aspx.cs" Inherits="UserInterface.AboutUs" MasterPageFile="~/MasterPage.Master" Theme="MainTheme" %>


<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>


<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc2" %>


<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">

    <div style="margin-top: 10px; margin-bottom: 10px;">
        <div class="trb">
            <div class="tlb">
                <div class="brb2">
                    <div class="blb2">


                        <div class="blhr">

                            <div class="contentBoxTopBottomHr">




                                <asp:Panel ID="pnlNotification" runat="server" CssClass="usrNotificationPnl"
                                    Visible="False">
                                    <asp:Label ID="lblNotification" runat="server" Text="Notification"></asp:Label>
                                    <br />
                                </asp:Panel>

                                <table style="width: 100%">
                                    <tr>
                                        <td valign="top" style="padding-right: 10px;">

                                            <div class="sectionTextHeader" style="padding-bottom: 10px; padding-top: 8px;">
                                                <asp:Label ID="lblPageIntro" runat="server" Text="About site"></asp:Label>
                                            </div>

                                            <asp:Label ID="lblAbout" runat="server" Text="About Extended"></asp:Label>


                                        </td>
                                        <td style="width: 480px;" valign="top">

                                            <asp:Panel ID="pnlContact" runat="server" CssClass="aboutMsgPnl"
                                                DefaultButton="btnSubmit">
                                                <table>
                                                    <tr>
                                                        <td align="right">&nbsp;</td>
                                                        <td>
                                                            <div style="padding: 0px 0px 5px 0px;">
                                                                <asp:Label ID="lblContact" runat="server" Style="margin-left: 20px;"
                                                                    Text="Contact form " CssClass="sectionTextHeader"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="width: 120px;">
                                                            <asp:Label ID="lblName" runat="server" Text="Name : "></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div style="margin: 2px 0px 2px 0px;">
                                                                <asp:TextBox ID="tbName" runat="server" Columns="30"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvName" runat="server"
                                                                    ControlToValidate="tbName" ErrorMessage="*" ValidationGroup="20"></asp:RequiredFieldValidator>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblSection" runat="server" Text="Section :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div style="margin: 2px 0px 2px 0px;">
                                                                <asp:DropDownList ID="ddlSection" runat="server" Style="margin: 0px;">
                                                                </asp:DropDownList>
                                                                <div style="margin: 2px 0px 2px 0px;">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblSubject" runat="server" Text="Subject : "></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div style="margin: 2px 0px 2px 0px;">
                                                                <asp:TextBox ID="tbSubject" runat="server" Columns="30"></asp:TextBox>
                                                                <div style="margin: 2px 0px 2px 0px;">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblEmail" runat="server" Text="Email : "></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div style="margin: 2px 0px 2px 0px;">
                                                                <asp:TextBox ID="tbEmail" runat="server" Columns="30"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                                                    ControlToValidate="tbEmail" ErrorMessage="*" ValidationGroup="20"></asp:RequiredFieldValidator>
                                                                <div style="margin: 2px 0px 2px 0px;">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblDescr" runat="server" Text="Description : "></asp:Label>
                                                        </td>
                                                        <td valign="top">
                                                            <div style="margin: 2px 0px 2px 0px;">
                                                                <asp:TextBox ID="tbDescription" runat="server" Columns="20" Rows="9"
                                                                    TextMode="MultiLine" Width="310px"></asp:TextBox><asp:RequiredFieldValidator ID="rfvDescription" runat="server"
                                                                        ControlToValidate="tbDescription" ErrorMessage="*" ValidationGroup="20"></asp:RequiredFieldValidator>
                                                                <div style="margin: 2px 0px 2px 0px;">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">&nbsp;</td>
                                                        <td valign="top">
                                                            <div style="margin: 2px 0px 2px 0px;">
                                                                <cc2:CaptchaControl ID="ccContact" runat="server" CaptchaHeight="60"
                                                                    CaptchaLength="3" CaptchaMaxTimeout="3600" Height="60px"
                                                                    Width="180px" BackColor="#FFFFE1" />
                                                                <div style="margin: 2px 0px 2px 0px;">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">&nbsp;</td>
                                                        <td valign="top">
                                                            <div style="margin: 2px 0px 2px 0px;">
                                                                <asp:TextBox ID="tbCaptcha" runat="server" Width="179px"></asp:TextBox>
                                                                <ajaxToolkit:TextBoxWatermarkExtender ID="tbCaptcha_TextBoxWatermarkExtender"
                                                                    runat="server" TargetControlID="tbCaptcha"
                                                                    WatermarkText="Type image letters here">
                                                                </ajaxToolkit:TextBoxWatermarkExtender>
                                                                <asp:RequiredFieldValidator ID="rfvCaptcha" runat="server"
                                                                    ControlToValidate="tbCaptcha" ErrorMessage="*" ValidationGroup="20"></asp:RequiredFieldValidator>
                                                                <div style="margin: 2px 0px 2px 0px;">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <div style="padding: 5px 0px 5px 0px;">
                                                                <cc1:DecoratedButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click"
                                                                    Text="Send" ValidationGroup="20" />
                                                            </div>
                                                            <asp:Label ID="lblError" runat="server" Text="Error :"
                                                                Visible="False" CssClass="errors"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>

                                            <div class="uploadImgBGR" style="margin-top: 10px; padding: 10px; font-size: large; font-family: 'Times New Roman';">

                                                <div class="sectionTextHeader" style="margin-bottom: 5px;">
                                                    <asp:Label ID="lblSiteTeam" runat="server" Text="Team"></asp:Label>
                                                </div>
                                                <img src="images/SiteImages/triangle.png" />
                                                <asp:Label ID="lblTeamMember1" runat="server" Text="Member1"></asp:Label>
                                                <br />
                                                <img src="images/SiteImages/triangle.png" />
                                                <asp:Label ID="lblTeamMember2" runat="server" Text="Member2"></asp:Label>
                                                <br />
                                                <img src="images/SiteImages/triangle.png" />
                                                <asp:Label ID="lblTeamMember3" runat="server" Text="Member3"></asp:Label>


                                            </div>
                                        </td>
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



    <asp:Panel ID="pnlHidden" runat="server" Visible="False">
        <br />
        <asp:Button ID="btnSubmitOld" runat="server" OnClick="btnSubmit_Click"
            Text="Send" />
        <br />
    </asp:Panel>
    <ajaxToolkit:NoBot ID="NoBotAbout" runat="server" CutoffMaximumInstances="6"
        CutoffWindowSeconds="120" ResponseMinimumDelaySeconds="3" />
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
</asp:Content>


