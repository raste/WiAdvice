<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="UserInterface.Error" MasterPageFile="MasterPage.Master" Theme="MainTheme" %>


<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">

    <br />

    <div align="center">
        <asp:Label ID="lblError" runat="server" Font-Size="X-Large" ForeColor="Red"
            Text="Error :"></asp:Label>
    </div>
    <asp:Panel ID="pnlOccTime" runat="server" Visible="false">
        <asp:Label ID="lblUtcTime" runat="server"
            Text="Utc time"></asp:Label>

    </asp:Panel>

    <br />

</asp:Content>
