<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="LastComments.aspx.cs" Inherits="UserInterface.LastComments" Theme="MainTheme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="s" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <br />

    <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False"
        CssClass="usrNotificationPnl">
        <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="pnlShowComms" runat="server" Style="padding-left: 10px;" DefaultButton="btnShow">
        Last comments :&nbsp;num.
        <asp:TextBox ID="tbNumber" runat="server" Columns="3" Text="20"></asp:TextBox>
        <ajaxToolkit:FilteredTextBoxExtender ID="tbNumber_FilteredTextBoxExtender"
            runat="server" FilterType="Numbers" TargetControlID="tbNumber">
        </ajaxToolkit:FilteredTextBoxExtender>
        , max length
        <asp:TextBox ID="tbMaxLength" runat="server" Columns="3" Text="20"></asp:TextBox>
        <ajaxToolkit:FilteredTextBoxExtender ID="tbMaxLength_FilteredTextBoxExtender"
            runat="server" FilterType="Numbers" TargetControlID="tbMaxLength">
        </ajaxToolkit:FilteredTextBoxExtender>
        ,
        <asp:RadioButtonList ID="rblVisibility" runat="server"
            RepeatDirection="Horizontal" RepeatLayout="Flow">
            <asp:ListItem Value="0">all</asp:ListItem>
            <asp:ListItem Selected="True" Value="1">visible</asp:ListItem>
            <asp:ListItem Value="2">deleted</asp:ListItem>
        </asp:RadioButtonList>
        &nbsp;, opt. Ip adress
        <asp:TextBox ID="tbIpAdress" runat="server"></asp:TextBox>
        <ajaxToolkit:FilteredTextBoxExtender ID="tbIpAdress_FilteredTextBoxExtender"
            runat="server" TargetControlID="tbIpAdress" ValidChars="0123456789.:">
        </ajaxToolkit:FilteredTextBoxExtender>
        <asp:Button ID="btnShow" runat="server" Text="Show" OnClick="btnShow_Click" />
        <br />
        <asp:Label ID="lblError" runat="server" CssClass="errors" Text="Error"
            Visible="False"></asp:Label>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlResults" runat="server" HorizontalAlign="Center"
        Visible="False">
        <asp:Label ID="lblResults" runat="server" Text="Results"></asp:Label>
    </asp:Panel>
    <p>
        <asp:PlaceHolder ID="phComments" runat="server"></asp:PlaceHolder>
    </p>
    <p>
    </p>
    <p>
    </p>
    <p>
    </p>

    <asp:Panel ID="pnlPopUp" runat="server" CssClass="pnlPopUpStyle" Width="450px">
    </asp:Panel>

</asp:Content>
