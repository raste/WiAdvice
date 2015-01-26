<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AddressActivity.aspx.cs" Inherits="UserInterface.AddressActivity" Theme="MainTheme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1 {
            width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <br />

    <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False"
        CssClass="usrNotificationPnl">
        <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="pnlShowLogs" runat="server" DefaultButton="btnShowActivity">
        &nbsp;Ip adress :
        <asp:TextBox ID="tbIpAdress" runat="server"></asp:TextBox>
        <ajaxToolkit:FilteredTextBoxExtender ID="tbIpAdress_FilteredTextBoxExtender"
            runat="server" TargetControlID="tbIpAdress" ValidChars="0123456789.:">
        </ajaxToolkit:FilteredTextBoxExtender>
        <asp:TextBox ID="tbNumber" runat="server" Columns="5">50</asp:TextBox>
        <ajaxToolkit:FilteredTextBoxExtender ID="tbNumber_FilteredTextBoxExtender"
            runat="server" FilterType="Numbers" TargetControlID="tbNumber">
        </ajaxToolkit:FilteredTextBoxExtender>
        <asp:Button ID="btnShowActivity" runat="server" OnClick="btnShowActivity_Click"
            Text="Show" />
        &nbsp;<asp:PlaceHolder ID="phShowError" runat="server" Visible="False"></asp:PlaceHolder>
        <br />
        &nbsp;Show banned Ip adresses :
            <asp:Button ID="btnShowBanned" runat="server" OnClick="btnShowBanned_Click"
                Text="Show" />
    </asp:Panel>



    <br />
    <asp:Panel ID="pnlStatus" runat="server" HorizontalAlign="Center"
        Visible="False">
        <asp:Label ID="lblStatus" runat="server" Text="Status"
            CssClass="searchPageRatings"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="pnlBanDescription" Style="margin-left: 50px; margin-right: 50px;" CssClass="panelRows" runat="server" Visible="False">
        <br />
        <asp:Label ID="lblBanDescription" runat="server" Text="ban description"></asp:Label>

    </asp:Panel>




    <br />
    <asp:Panel ID="pnlBanOrUnban" runat="server" CssClass="panelRows editSiteBGR" Style="margin-left: 50px;"
        Visible="False" Width="800px">
        &nbsp;<asp:Label ID="lblEditInfo" runat="server" Text="Change date button changes only date. Type reason for every ban."></asp:Label>
        <br />
        <table class="style1">
            <tr>
                <td style="width: 1px;">
                    <asp:TextBox ID="tbDescription" runat="server" Columns="40" Rows="3"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
                <td valign="top" style="padding-left: 10px;">
                    <asp:Panel ID="pnlUntillDate" Style="margin-bottom: 5px;" runat="server">
                        Untill date :
                            <asp:TextBox ID="tbUntillDate" runat="server"></asp:TextBox>
                        <ajaxToolkit:PopupControlExtender ID="tbUntillDate_PopupControlExtender"
                            runat="server" CommitProperty="value" CommitScript="e.value;"
                            TargetControlID="tbUntillDate" PopupControlID="PopUp" Position="Bottom">
                        </ajaxToolkit:PopupControlExtender>

                        &nbsp;month/day/year format
                    </asp:Panel>
                    <asp:Button ID="btnChngDate" runat="server" OnClick="btnChngDate_Click"
                        Text="Change date" />
                    <asp:Button ID="btnBanUnban" runat="server" OnClick="btnBanUnban_Click"
                        Text="Ban or unban" />
                </td>
            </tr>
        </table>
        <asp:PlaceHolder ID="phEditError" runat="server" Visible="False"></asp:PlaceHolder>
        <br />
    </asp:Panel>
    <br />
    <asp:PlaceHolder ID="phActivity" runat="server"></asp:PlaceHolder>

    <p>
    </p>
    <asp:Panel ID="pnlHidden" runat="server" BackColor="#FFCCFF" Visible="False">

        <asp:Label ID="lblError" runat="server" Text="Error" CssClass="errors"></asp:Label>

    </asp:Panel>

    <asp:Panel ID="PopUp" runat="server" CssClass="popUpControl" Width="220px">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <asp:Calendar ID="cExpireDate" runat="server"
                    Height="115px" BackColor="White" BorderColor="Black" BorderStyle="Solid"
                    BorderWidth="1px" OnSelectionChanged="cExpireDate_SelectionChanged"></asp:Calendar>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>

</asp:Content>
