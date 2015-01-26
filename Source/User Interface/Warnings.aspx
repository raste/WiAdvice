<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Warnings.aspx.cs" Inherits="UserInterface.Warnings" Theme="MainTheme"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
    <asp:Panel ID="pnlUsrNotification" runat="server" CssClass="usrNotificationPnl" 
        Visible="False">
        <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
    </asp:Panel>
 <br />
    <asp:Label ID="lblInfo" runat="server" Text="Info"></asp:Label>

 <p style="margin-left:20px;" class="searchPageRatings">
        <asp:Label ID="lblWarningsToRemoveRole" runat="server" 
            Text="Number to remove role"></asp:Label>
            <br />
             <asp:Label ID="lblWarningsToDelUser" runat="server" 
            Text="Number to delete user"></asp:Label>
</p>
    
       

    <asp:Panel ID="pnlAddWarning" runat="server" style="padding-left:20px;" DefaultButton="btnAddUserWarning">
        Add user warning :
        <asp:TextBox ID="tbAddUserWarning" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvTbUsername" runat="server" 
            ControlToValidate="tbAddUserWarning" ErrorMessage="*" ValidationGroup="123"></asp:RequiredFieldValidator>
        &nbsp;for role
        <asp:DropDownList ID="ddlUserRoles" runat="server">
            <asp:ListItem Value="0">... choose</asp:ListItem>
            <asp:ListItem Value="general">General</asp:ListItem>
            <asp:ListItem Value="commenter">Write comments and messages</asp:ListItem>
            <asp:ListItem Value="product">Add products</asp:ListItem>
            <asp:ListItem Value="company">Add companies</asp:ListItem>
            <asp:ListItem Value="suggestor">Write suggestions</asp:ListItem>
            <asp:ListItem Value="prater">Rate products</asp:ListItem>
            <asp:ListItem Value="commrater">Rate comments</asp:ListItem>
            <asp:ListItem Value="flagger">Report</asp:ListItem>
        </asp:DropDownList>
        &nbsp;
        <br />
        <span style="margin-left:337px;"> or&nbsp;</span>
        <asp:DropDownList ID="ddlTypeRoles" runat="server">
            <asp:ListItem Value="0">... choose</asp:ListItem>
            <asp:ListItem>product</asp:ListItem>
            <asp:ListItem>company</asp:ListItem>
            <asp:ListItem Value="aCompProdModificator">all company products</asp:ListItem>
        </asp:DropDownList>
        &nbsp;id :
        <asp:TextBox ID="tbTypeID" runat="server" Columns="5"></asp:TextBox>
        <ajaxToolkit:FilteredTextBoxExtender ID="tbTypeID_FilteredTextBoxExtender" 
            runat="server" FilterType="Numbers" TargetControlID="tbTypeID">
        </ajaxToolkit:FilteredTextBoxExtender>
        <br />
        <asp:TextBox ID="tbWarningDescription" runat="server" Columns="90" Rows="10" style="margin-bottom:2px;margin-top:2px;"
            TextMode="MultiLine"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvTbWarningDescription" runat="server" 
            ControlToValidate="tbWarningDescription" ErrorMessage="*" ValidationGroup="123"></asp:RequiredFieldValidator>
        <br />
        <asp:Button ID="btnAddUserWarning" runat="server" 
            onclick="btnAddUserWarning_Click" Text="Add" ValidationGroup="123" />
        <br />
        <asp:PlaceHolder ID="phErrorAddWarning" runat="server" Visible="False">
        </asp:PlaceHolder>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlShowPatterns" runat="server" CssClass="accordionHeaders">
        <asp:Label ID="lblShowPatterns" runat="server" Text="Show patterns"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="pnlWarningPatterns" runat="server">
        <asp:Table ID="tblPatterns" runat="server" Width="100%">
        </asp:Table>
    </asp:Panel>
    <ajaxToolkit:CollapsiblePanelExtender ID="pnlWarningPatterns_CollapsiblePanelExtender" 
        runat="server" Enabled="True" TargetControlID="pnlWarningPatterns" 
        ExpandControlID="pnlShowPatterns" CollapseControlID="pnlShowPatterns" 
        Collapsed="True" CollapsedSize="0" CollapsedText="Show patterns" 
        ExpandedText="Hide patterns" TextLabelID="lblShowPatterns">
    </ajaxToolkit:CollapsiblePanelExtender>
    <br />
    <asp:Panel ID="pnlUserWarnings" style="padding-left:20px;" runat="server" 
        DefaultButton="btnViewUserWarnings">
        View user warnings :
        <asp:TextBox ID="tbUserWarnings" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvTbUserWarning" runat="server" 
            ControlToValidate="tbUserWarnings" ErrorMessage="*" ValidationGroup="124"></asp:RequiredFieldValidator>
        <asp:Button ID="btnViewUserWarnings" runat="server" Text="View" 
            onclick="btnViewUserWarnings_Click" ValidationGroup="124" />
        <asp:PlaceHolder ID="phErrorUserWarnings" runat="server" Visible="False">
        </asp:PlaceHolder>
    </asp:Panel>
<p style="text-align:center;">
    <asp:Label ID="lblStatus" runat="server" Text="Status" Visible="False" 
        CssClass="sectionTextHeader" ForeColor="Red"></asp:Label>
</p>
    <p>
        <asp:PlaceHolder ID="phUserWarnings" runat="server"></asp:PlaceHolder>
</p>
    <asp:Panel ID="pnlHidden" runat="server" BackColor="#FFCCFF" Visible="False">
        <asp:Label ID="lblError" runat="server" CssClass="errors" Text="Label"></asp:Label>
        <br />
        <br />
        <br />
    </asp:Panel>
    
    <asp:Panel ID="pnlPopUp" runat="server" CssClass="pnlPopUpStyle" Width="450px"></asp:Panel>
</asp:Content>
