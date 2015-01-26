<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="UserInterface.Notifications" Theme="MainTheme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="margin-top: 10px; margin-bottom: 10px;">
        <div class="brb2">
            <div class="blb2">
                <div class="blhr">
                    <div class="contentBoxBottomHr">



                        <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False"
                            CssClass="usrNotificationPnl">
                            <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
                        </asp:Panel>



                        <asp:Panel ID="pnlNotifyInfo" runat="server" CssClass="sectionTextHeader">
                            <asp:Label ID="lblInfo" runat="server"
                                Text="Here you can see notifies for new content in products/makers for which you have signed."></asp:Label>
                        </asp:Panel>
                        <br />

                        <asp:Table ID="tblNotifies" Style="padding: 5px; border-collapse: separate;" runat="server" Width="100%" CellSpacing="3" CellPadding="3">
                        </asp:Table>


                    </div>

                    <img src="images/SiteImages/horL.png" align="left" />
                    <img src="images/SiteImages/horR.png" align="right" />

                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle"></asp:Panel>
</asp:Content>
