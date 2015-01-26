<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyTypes.aspx.cs" Inherits="UserInterface.CompanyTypes" MasterPageFile="~/MasterPage.Master" Theme="MainTheme" %>

<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">


    <br />

    <p>
        <asp:Label ID="lblInfo" runat="server" Text="Information how to use the page"></asp:Label>
    </p>
    <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False"
        CssClass="usrNotificationPnl">
        <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="pnllAddEditType" runat="server" CssClass="editBGR">
        <hr />
        <table class="style1" style="width: 100%;">
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Label ID="lblEditType" runat="server" Text="Add maker type"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right" style="width: 250px;">
                    <asp:Label ID="lblName" runat="server" Text="Name : "></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbName" runat="server"></asp:TextBox>
                    &nbsp;<asp:Label ID="lblCheckName" runat="server" Text="Check name"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="lblDescription" runat="server" Text="Description : "></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbDescription" runat="server" Columns="50" Rows="5"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit"
                        OnClick="btnSubmit_Click" />
                </td>
                <td>
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Visible="False"
                        OnClick="btnCancel_Click" />
                    <asp:Label ID="lblError" runat="server" CssClass="errors" Text="Error"
                        Visible="False"></asp:Label>
                </td>
            </tr>
        </table>
        <hr />
    </asp:Panel>

    <br />
    &nbsp;
    <asp:Label ID="lblShowCretors" runat="server"
        Text="Show makers with type : "></asp:Label>
    <asp:DropDownList ID="ddlTypes" runat="server">
    </asp:DropDownList>
    &nbsp;<asp:Button ID="btnShowCompanies" runat="server"
        OnClick="btnShowCompanies_Click" Text="Show" />
    <br />
    <br />
    <asp:Table ID="tblCompanies" runat="server" BorderColor="#CCCCCC"
        CellPadding="0" CellSpacing="5" CssClass="margins" GridLines="Both"
        Visible="False" Width="100%">
    </asp:Table>
    <asp:Table ID="tblCompanyTypes" runat="server" BorderColor="#CCCCCC"
        CellPadding="0" CellSpacing="5" CssClass="margins" Width="100%">
    </asp:Table>
    <p></p>

</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">

    <style type="text/css">
        .style1 {
            width: 100%;
        }
    </style>

</asp:Content>
