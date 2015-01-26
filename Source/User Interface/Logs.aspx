<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logs.aspx.cs" Inherits="UserInterface.Logs.Logs" MasterPageFile="MasterPage.Master" Theme="MainTheme" %>

<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">
    <br />
    <asp:Label ID="lblAbout" runat="server" Text="Logs About"></asp:Label>
    <br />
    <br />
    <table style="width: 100%;">
        <tr>
            <td>&nbsp;</td>
            <td class="style14" align="center">Number</td>
            <td class="style15" align="center">Logs Type</td>
            <td class="style16" align="center">About Type</td>
            <td class="style17" align="center">with ID</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="right" style="width: 100px;">Show logs :
            </td>
            <td class="style14">
                <asp:TextBox ID="tbNumLogs" runat="server" Columns="7">50</asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbNumLogs_FilteredTextBoxExtender"
                    runat="server" FilterType="Numbers" TargetControlID="tbNumLogs">
                </ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td class="style15">
                <asp:DropDownList ID="ddlLogsType" runat="server">
                </asp:DropDownList>
            </td>
            <td class="style16">
                <asp:DropDownList ID="ddLogsOptions" runat="server">
                </asp:DropDownList>
            </td>
            <td class="style17">
                <asp:TextBox ID="tbID" runat="server" Columns="7"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbID_FilteredTextBoxExtender"
                    runat="server" FilterType="Numbers" TargetControlID="tbID">
                </ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td>
                <asp:Button ID="btnGetLogs" runat="server" OnClick="btnGetLogs_Click"
                    Text="Show" />
                &nbsp;<asp:PlaceHolder ID="phGetLogsByID"
                    runat="server" Visible="False"></asp:PlaceHolder>
            </td>
        </tr>
    </table>
    <br />
    <table style="width: 100%;">
        <tr>
            <td>&nbsp;</td>
            <td class="style30" align="center">Number</td>
            <td class="style27" align="center">Logs Type</td>
            <td align="center" style="width: 100px;">About Types</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="right" style="width: 100px;">Show logs :</td>
            <td class="style30">
                <asp:TextBox ID="tbNumLogs2" runat="server" Columns="7">50</asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbNumLogs2_FilteredTextBoxExtender"
                    runat="server" FilterType="Numbers" TargetControlID="tbNumLogs2">
                </ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td class="style28">
                <asp:DropDownList ID="ddlLogsType2" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="ddlLogsOptions2" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btnGetLogs2" runat="server" Text="Show"
                    OnClick="btnGetLogs2_Click" />
                &nbsp;<asp:PlaceHolder ID="phGetLogs" runat="server"
                    Visible="False"></asp:PlaceHolder>
            </td>
        </tr>
    </table>
    <br />

    <table style="width: 100%;">
        <tr>
            <td>&nbsp;</td>
            <td class="style14">Number</td>
            <td style="width: 80px;">From User</td>
            <td class="style25">Logs Type</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="right" style="width: 100px;">Show logs :</td>
            <td class="style14">
                <asp:TextBox ID="tbNumLogs3" runat="server" Columns="7">50</asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbNumLogs3_FilteredTextBoxExtender"
                    runat="server" FilterType="Numbers" TargetControlID="tbNumLogs3">
                </ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td align="center">
                <asp:TextBox ID="tbLogsFromUser" runat="server" Columns="7"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbLogsFromUser_FilteredTextBoxExtender"
                    runat="server" FilterType="Numbers" TargetControlID="tbLogsFromUser">
                </ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td class="style25">
                <asp:DropDownList ID="ddlLogsType3" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btnGetLogs3" runat="server" OnClick="btnGetLogs3_Click"
                    Text="Show" />
                &nbsp;<asp:PlaceHolder ID="phGetLogsFrom" runat="server"></asp:PlaceHolder>
            </td>
        </tr>
    </table>


    <br />

    <br />

    <asp:Panel ID="pnlShowLastDeleted" runat="server" Style="" DefaultButton="btnLdShow">

        <table cellspacing="0" style="width: auto;">
            <tr>
                <td style="border-right: solid 1px Silver; border-bottom: solid 1px Silver;">&nbsp;</td>
                <td align="center" style="border-right: solid 1px Silver; border-bottom: solid 1px Silver;">Type ID or name contains (optional)</td>
                <td align="center" style="border-right: solid 1px Silver; border-bottom: solid 1px Silver;">Number deleted types and deleted by user (optional)</td>
                <td align="center" style="width: 120px; border-bottom: solid 1px Silver; border-right: solid 1px Silver;">Type wanted</td>
                <td style="border-bottom: solid 1px Silver;">&nbsp;</td>
            </tr>
            <tr>
                <td align="center" style="width: 100px; border-right: solid 1px Silver;">Last deleted
                </td>
                <td style="width: 270px; border-right: solid 1px Silver;">ID :
                    <asp:TextBox ID="tbLdID" runat="server" Columns="5"></asp:TextBox>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; .. or<br />
                    Name contains :                   
                    <asp:TextBox ID="tbLdNameContains"
                        runat="server" Width="110px"></asp:TextBox>
                </td>
                <td style="width: 400px; border-right: solid 1px Silver;">Last number :
                    <asp:TextBox ID="tbLdLastNumber" runat="server" Columns="5"></asp:TextBox>
                    <br />
                    By user :
                    <asp:TextBox ID="tbLdByName" runat="server"></asp:TextBox>
                    , or user ID :
                    <asp:TextBox ID="tbLdByUserId" runat="server" Columns="5"></asp:TextBox>
                </td>
                <td align="center" style="border-right: solid 1px Silver; width: 120px;">
                    <asp:DropDownList ID="ddlLdChooseType" runat="server">
                        <asp:ListItem Value="0">... choose</asp:ListItem>
                        <asp:ListItem Value="1">products</asp:ListItem>
                        <asp:ListItem Value="2">companies</asp:ListItem>
                        <asp:ListItem Value="3">categories</asp:ListItem>
                        <asp:ListItem Value="4">users</asp:ListItem>
                        <asp:ListItem Value="5">topics</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td style="padding-left: 10px; padding-right: 10px;">
                    <asp:Button ID="btnLdShow" runat="server" Text="Show"
                        OnClick="btnLdShow_Click" />
                </td>
            </tr>
        </table>
        <asp:PlaceHolder ID="phLastDeleted" runat="server" Visible="False"></asp:PlaceHolder>
    </asp:Panel>
    <br />

    <p>
        <asp:Table ID="tblShowLastDeleted" runat="server" Visible="False" BorderWidth="1px"
            CellPadding="1" CellSpacing="1" GridLines="Both" BorderColor="#CCCCCC"
            Width="100%">
        </asp:Table>
        <asp:Table ID="tblGetLogs" runat="server" Visible="False" BorderWidth="1px"
            CellPadding="1" CellSpacing="1" GridLines="Both" BorderColor="#CCCCCC"
            Width="100%">
        </asp:Table>
        <asp:Table ID="tblSpecLogs" runat="server" Visible="False" BorderWidth="1px"
            CellPadding="1" CellSpacing="1" GridLines="Both" BorderColor="#CCCCCC"
            Width="100%">
        </asp:Table>
    </p>
    <asp:Panel ID="pnlGLobal" runat="server" Visible="False">
        registered admins :
        <asp:Button ID="btnShowRegAdmins" runat="server"
            OnClick="btnShowRegAdmins_Click" Text="Show" />
        <br />
        <asp:Table ID="tblRegAdmins" runat="server" Visible="False"
            BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px"
            GridLines="Both" CssClass="margins" Width="100%">
        </asp:Table>
        <br />
        deleted admins :
        <asp:Button ID="btnShowDelAdmins" runat="server"
            OnClick="btnShowDelAdmins_Click" Text="Show" />
        <br />
        <asp:Table ID="tblDelAdmins" runat="server" Visible="False"
            BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px"
            GridLines="Both" CssClass="margins" Width="100%">
        </asp:Table>
        <br />
        deleted categories :
        <asp:Button ID="btnShowDeletedCategories" runat="server" Text="Show"
            OnClick="btnShowDeletedCategories_Click" />
        <asp:Table ID="tblDeletedCategories" runat="server" BorderColor="#CCCCCC"
            BorderStyle="Solid" BorderWidth="1px" CssClass="margins" Width="100%"
            GridLines="Both" Visible="False">
        </asp:Table>
    </asp:Panel>
    <asp:Panel ID="pnlAdmin" runat="server" Visible="False">
        <br />
        registered moderators :
        <asp:Button ID="btnShowRegModer" runat="server" OnClick="btnShowRegModer_Click"
            Text="Show" />
        <br />
        <asp:Table ID="tblRegModer" runat="server" Visible="False"
            BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px"
            GridLines="Both" CssClass="margins" Width="100%">
        </asp:Table>
        <br />
        deleted moderators :
        <asp:Button ID="btnShowDelModer" runat="server" OnClick="btnShowDelModer_Click"
            Text="Show" />
        <br />
        <asp:Table ID="tblDelModer" runat="server" Visible="False"
            BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px"
            GridLines="Both" CssClass="margins" Width="100%">
        </asp:Table>
    </asp:Panel>
    <br />
    registered users :&nbsp;<asp:Button ID="btnShowRegUsers" runat="server" OnClick="btnShowRegUsers_Click"
        Text="Show" />
    <asp:Table ID="tblShowAddedUsers" runat="server" Visible="False"
        BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px"
        GridLines="Both" CssClass="margins" Width="100%">
    </asp:Table>
    <asp:Panel ID="pnlHidden" runat="server" Visible="False">
        <br />
        <asp:Label ID="lblError" runat="server" Text="Error :" CssClass="errors"></asp:Label>
        <br />
        <br />
        <br />
        <br />
        <br />
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">

    <style type="text/css">
        .style14 {
            width: 57px;
        }

        .style15 {
            width: 84px;
        }

        .style16 {
            width: 83px;
        }

        .style17 {
            width: 59px;
        }

        .style25 {
            width: 86px;
        }

        .style27 {
            width: 77px;
            height: 23px;
        }

        .style28 {
            width: 77px;
        }

        .style30 {
            width: 53px;
        }
    </style>

</asp:Content>
