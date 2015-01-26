<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="LastCP.aspx.cs" Inherits="UserInterface.LastCP" Theme="MainTheme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <br />
    <asp:Panel ID="pnlShowLast" runat="server" DefaultButton="btnShow">
        &nbsp;&nbsp;Show last added
            <asp:RadioButtonList ID="rblType" Style="margin-left: 10px; margin-right: 10px;" runat="server" RepeatDirection="Horizontal"
                RepeatLayout="Flow">
                <asp:ListItem Selected="True">products</asp:ListItem>
                <asp:ListItem>companies</asp:ListItem>
                <asp:ListItem>topics</asp:ListItem>
            </asp:RadioButtonList>
        &nbsp;, number
            <asp:TextBox ID="tbNumber" runat="server" Text="20" Columns="5"></asp:TextBox>
        <ajaxToolkit:FilteredTextBoxExtender ID="tbNumber_FilteredTextBoxExtender"
            runat="server" FilterType="Numbers" TargetControlID="tbNumber">
        </ajaxToolkit:FilteredTextBoxExtender>
        <asp:Button ID="btnShow" runat="server" Text="Show" OnClick="btnShow_Click" />
        <br />
        &nbsp;
            <asp:Label ID="lblError" runat="server" CssClass="errors" Text="Error"
                Visible="False"></asp:Label>
    </asp:Panel>
    <br />
    <asp:PlaceHolder ID="phProducts" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="phCompanies" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="phTopics" runat="server"></asp:PlaceHolder>

    <asp:Panel ID="pnlPopUp" runat="server" CssClass="pnlPopUpStyle" Width="450px">
    </asp:Panel>
</asp:Content>
