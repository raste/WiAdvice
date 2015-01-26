<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Messages.aspx.cs" Inherits="UserInterface.Messages" Theme="MainTheme" %>

<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1 {
            width: 100%;
            border-collapse: collapse;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlMessages" runat="server" DefaultButton="btnSendMessage">


        <div style="margin-top: 10px;">
            <div class="trb">
                <div class="tlb">
                    <div class="brb2">
                        <div class="blb2">


                            <div class="blhr">

                                <div class="contentBoxTopBottomHr">


                                    <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False" CssClass="usrNotificationPnl">
                                        <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
                                    </asp:Panel>



                                    <div class="sendMsgBGR" style="margin-bottom: 0px; margin-top: 5px;">
                                        <table style="width: 100%;">
                                            <tr>
                                                <td align="center">&nbsp;&nbsp;&nbsp;
                                                </td>
                                                <td align="left" colspan="2">
                                                    <div style="margin: 0px 0px 8px 300px;">
                                                        <asp:Label ID="lblWriteMessage" runat="server" Text="Write Message" CssClass="sectionTextHeader"></asp:Label>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" colspan="1" style="width: 110px">
                                                    <asp:Label ID="lblMsgTo" runat="server" Text="To :"></asp:Label>
                                                </td>
                                                <td style="width: 400px;">
                                                    <div style="margin: 2px 0px 2px 0px;">
                                                        <asp:TextBox ID="tbMsgTo" runat="server" ValidationGroup="10"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="tbMsgTo"
                                                            ValidationGroup="10"></asp:RequiredFieldValidator>
                                                        &nbsp;<asp:Label ID="lblCName" runat="server" Text="Check Name" CssClass="smallerText"
                                                            Font-Bold="True" Font-Italic="False" ForeColor="#C02E29"></asp:Label>
                                                    </div>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblBlockMsgs" runat="server" Text="Block all incoming Messages : "></asp:Label>
                                                    <cc1:DecoratedButton ID="btnBlockAllMsgs" runat="server" OnClick="btnBlockAllMsgs_Click"
                                                        Text="Block" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" colspan="1">
                                                    <asp:Label ID="lblMsgSubject" runat="server" Text="Subject :"></asp:Label>
                                                </td>
                                                <td>
                                                    <div style="margin: 2px 0px 2px 0px;">
                                                        <asp:TextBox ID="tbMsgSubject" runat="server"></asp:TextBox>
                                                        &nbsp;<asp:CheckBox ID="cbMsgSave" runat="server" Text="Save in sent Messages" />
                                                    </div>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblBlockUser" runat="server" Text="Block User : "></asp:Label>
                                                    <asp:TextBox ID="tbBlockUser" runat="server" ValidationGroup="11" Columns="17"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvBlockUser" runat="server" ControlToValidate="tbBlockUser"
                                                        ValidationGroup="11"></asp:RequiredFieldValidator>
                                                    <cc1:DecoratedButton ID="btnBlockUser" runat="server" OnClick="btnBlockUser_Click"
                                                        Text="Block" ValidationGroup="11" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" colspan="1">
                                                    <asp:Label ID="lblMsg" runat="server" Text="Message :"></asp:Label>
                                                </td>
                                                <td>
                                                    <div style="margin: 2px 0px 2px 0px;">
                                                        <asp:TextBox ID="tbMsgDescr" runat="server" Columns="20" Rows="8" TextMode="MultiLine"
                                                            ValidationGroup="11" Width="370px"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="tbMsgDescr"
                                                            ValidationGroup="10"></asp:RequiredFieldValidator>
                                                    </div>
                                                </td>
                                                <td valign="top">
                                                    <asp:Label ID="lblCBName" runat="server" Text="Check Name" CssClass="smallerText"
                                                        Font-Bold="True" ForeColor="#C02E29"></asp:Label>
                                                    <br />
                                                    <asp:PlaceHolder ID="phBlockUser" runat="server" Visible="False"></asp:PlaceHolder>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" colspan="1"></td>
                                                <td colspan="2">
                                                    <div style="padding: 5px 0px 0px 0px;">
                                                        <cc1:DecoratedButton ID="btnSendMessage" runat="server" OnClick="btnSendMessage_Click"
                                                            Text="Send" ValidationGroup="10" />
                                                        <cc1:TransliterateButton ID="btnTransliterateMessage" runat="server" />
                                                        <asp:PlaceHolder ID="phMessage" runat="server" Visible="False"></asp:PlaceHolder>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>



                                </div>

                                <img src="images/SiteImages/horL.png" align="left" />
                                <img src="images/SiteImages/horR.png" align="right" />

                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>

    </asp:Panel>

    <div class="panelNearSideElements">
        <ajaxToolkit:Accordion ID="aUserInfo" runat="server" FramesPerSecond="40" RequireOpenedPane="False"
            SelectedIndex="-1" SuppressHeaderPostbacks="True">
            <Panes>
                <ajaxToolkit:AccordionPane ID="apBlockedUsers" runat="server">
                    <Header>
                        <asp:Panel ID="pnlShowBlockedUsers" runat="server" CssClass="accordionHeaders">
                            <asp:Label ID="lblBlockedUsers" runat="server" Text="Blocked users"></asp:Label>
                        </asp:Panel>
                    </Header>
                    <Content>
                        <asp:Table ID="tblBlockedUsers" runat="server" Visible="True" Width="100%" BorderStyle="Solid"
                            BorderWidth="1px">
                        </asp:Table>
                    </Content>
                </ajaxToolkit:AccordionPane>
            </Panes>
        </ajaxToolkit:Accordion>
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



                                    <table class="style1" style="width: 100%;">
                                        <tr>
                                            <td valign="top">
                                                <asp:PlaceHolder ID="phMessages" runat="server"></asp:PlaceHolder>
                                            </td>
                                            <td runat="server" id="adCell" width="1">
                                                <asp:PlaceHolder ID="phAdverts" runat="server" Visible="False"></asp:PlaceHolder>
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
    </div>




    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FFCCFF">
        <asp:Label ID="lblError" runat="server" Text="Error:" CssClass="errors"></asp:Label>
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />
    </asp:Panel>
</asp:Content>
