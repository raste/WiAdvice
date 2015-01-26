<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="LogIn.aspx.cs" Inherits="UserInterface.LogIn" Theme="MainTheme" %>

<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div style="margin: 15px 0px 10px 0px;">
        <div class="trb">
            <div class="tlb">
                <div class="brb2">
                    <div class="blb2">


                        <div class="blhr">

                            <div class="contentBoxTopBottomHr">


                                <table style="width: 100%;">
                                    <tr>
                                        <td valign="top" style="width: 500px; padding-left: 30px;" runat="server" id="aboutCell">
                                            <div style="text-align: center;">
                                                <asp:Label ID="lblWelcome" runat="server" Text="Welcome" CssClass="textHeader"></asp:Label>
                                            </div>
                                            <br />
                                            <asp:Label ID="lblAbout" runat="server" Text="About"></asp:Label>
                                        </td>
                                        <td valign="top">
                                            <br />
                                            <br />
                                            <asp:Panel ID="pnlLogIn" runat="server" DefaultButton="btnLogIn" CssClass="LogInPanel">
                                                <table>
                                                    <tr>
                                                        <td style="width: 130px;" align="right">
                                                            <asp:Label ID="lblusername" runat="server" Text="Username :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbUsername" runat="server" CssClass="margins1px" SkinID="LogInBox"
                                                                ValidationGroup="99" Width="130px"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvForUsername" runat="server" ControlToValidate="tbUsername"
                                                                ErrorMessage="*" ValidationGroup="99"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblpassword" runat="server" Text="Password :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbPassword" runat="server" CssClass="margins1px" SkinID="LogInBox"
                                                                TextMode="Password" ValidationGroup="99" Width="130px"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvForPassword" runat="server" ControlToValidate="tbPassword"
                                                                ErrorMessage="*" ValidationGroup="99"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">&nbsp;
                                                        </td>
                                                        <td align="right" style="padding-right: 10px;">
                                                            <cc1:DecoratedButton ID="btnLogIn" runat="server" CssClass="margins1px" OnClick="btnLogIn_Click"
                                                                SkinID="headerBtn" Text="Log In" ValidationGroup="99" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <asp:Panel ID="pnlChangeName" runat="server" Visible="False">
                                                <asp:Label ID="lblChangeNameInfo" runat="server" Font-Size="Larger" ForeColor="#CC3300"
                                                    Text="Change name info"></asp:Label>
                                                <br />
                                                <br />
                                                <table>
                                                    <tr>
                                                        <td align="right" style="width: 160">
                                                            <asp:Label ID="lblCurrNameT" runat="server" Text="Current name :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblCurrName" runat="server" Text="Name"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblNewName" runat="server" Text="New name :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbNewName" SkinID="LogInBox" runat="server"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvChngName" runat="server" ControlToValidate="tbNewName"
                                                                ErrorMessage="*" ValidationGroup="chngName"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblRepName" runat="server" Text="Repeat name :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbRepName" SkinID="LogInBox" runat="server"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvRepName" runat="server" ControlToValidate="tbRepName"
                                                                ErrorMessage="*" ValidationGroup="chngName"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">&nbsp;
                                                        </td>
                                                        <td align="right" style="padding-right: 10px;">
                                                            <cc1:DecoratedButton ID="btnChangeName" runat="server" CssClass="margins1px" OnClick="btnChangeName_Click"
                                                                SkinID="headerBtn" Text="Change" ValidationGroup="chngName" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <asp:Panel ID="pnlError" runat="server" Visible="False">
                                                <asp:Label ID="lblError" runat="server" ForeColor="#CC3300" Text="Error:" Font-Size="Larger"></asp:Label>
                                            </asp:Panel>
                                        </td>
                                        <td valign="top" runat="server" id="adcell">
                                            <asp:PlaceHolder ID="phAdvert" runat="server" Visible="False"></asp:PlaceHolder>
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



</asp:Content>
