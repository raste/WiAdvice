<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SiteTexts.aspx.cs" Inherits="UserInterface.SiteTexts" MasterPageFile="~/MasterPage.Master" Theme="MainTheme"%>


<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit.HTMLEditor" tagprefix="cc1" %>


<%@ Register assembly="UserInterface" namespace="UserInterface" tagprefix="cc2" %>


<%@ Register assembly="CustomServerControls" namespace="CustomServerControls" tagprefix="cc3" %>


<asp:Content ID="Content1" runat="server" 
    contentplaceholderid="ContentPlaceHolder1">

    <asp:Panel ID="pnlShowAbout" runat="server" CssClass="accordionHeaders">
        <asp:Label ID="lblShowAbout" runat="server" Text="Show about"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="pnlAbout" runat="server">
        <asp:Label ID="lblAbout" runat="server" Text="Site Texts About"></asp:Label>
        </asp:Panel>
    <ajaxToolkit:CollapsiblePanelExtender ID="pnlAbout_CollapsiblePanelExtender" 
        runat="server" CollapseControlID="pnlShowAbout" Collapsed="True" 
        CollapsedText="Show about" Enabled="True" ExpandControlID="pnlShowAbout" 
        ExpandedText="Hide about" TargetControlID="pnlAbout" 
        TextLabelID="lblShowAbout">
    </ajaxToolkit:CollapsiblePanelExtender>
          <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False" 
            CssClass="usrNotificationPnl">
        <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
    </asp:Panel>  
    <table class="editBGR" style="width:100%">
        <tr>
            <td align="right">
                &nbsp;</td>
            <td>
                <asp:Label ID="lblWrite" runat="server" Text="Add/Edit Site Text"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 150px;">
                <asp:Label ID="lblName" runat="server" Text="Name : "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbName" runat="server" Columns="65"></asp:TextBox>
            &nbsp;<asp:Label ID="lblCName" runat="server" Text="Check Name" CssClass="errors"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblType" runat="server" Text="Type : "></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddlType" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblLinkId" runat="server" Text="Link ID :"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbLinkId" runat="server" Columns="30"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblDescr" runat="server" Text="Description : "></asp:Label>
            </td>
            <td>
                <cc1:Editor ID="ajxEditor" runat="server" BackColor="White" Height="350px" 
                    NoScript="True" Width="700px" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Button ID="btnAddText" runat="server" onclick="btnAddText_Click" 
                    Text="Submit" />
            </td>
            <td>
                <asp:Button ID="btnCancel" runat="server" onclick="btnCancel_Click" 
                    Text="Cancel" Visible="False" />
                <asp:PlaceHolder ID="phAddText" runat="server" Visible="False">
                </asp:PlaceHolder>
            </td>
        </tr>
    </table>
    <br />
    <asp:Panel ID="pnlShowLast" runat="server" DefaultButton="btnShowLast">
        <asp:Label ID="lblShowLast" runat="server" Text="Show Last : "></asp:Label>
        <asp:TextBox ID="tbShowLast" runat="server" Columns="7">50</asp:TextBox>
        <ajaxToolkit:FilteredTextBoxExtender ID="tbShowLast_FilteredTextBoxExtender" 
            runat="server" FilterType="Numbers" TargetControlID="tbShowLast">
        </ajaxToolkit:FilteredTextBoxExtender>
        <asp:DropDownList ID="ddlShowLastTypes" runat="server">
        </asp:DropDownList>
        &nbsp;Visible :
        <asp:DropDownList ID="ddlVisible" runat="server">
            <asp:ListItem Value="0">all</asp:ListItem>
            <asp:ListItem Value="1">true</asp:ListItem>
            <asp:ListItem Value="2">false</asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="btnShowLast" runat="server" onclick="btnShowLast_Click" 
            Text="Show" />
        <asp:PlaceHolder ID="phShowLast" runat="server" Visible="False">
        </asp:PlaceHolder>
    </asp:Panel>
    <asp:Panel ID="pnlShowIthId" runat="server" DefaultButton="btnShowId">
        <asp:Label ID="lblShowId" runat="server" Text="Show Text with Id : "></asp:Label>
        <asp:TextBox ID="tbShowId" runat="server" Columns="7"></asp:TextBox>
        <ajaxToolkit:FilteredTextBoxExtender ID="tbShowId_FilteredTextBoxExtender" 
            runat="server" FilterType="Numbers" TargetControlID="tbShowId">
        </ajaxToolkit:FilteredTextBoxExtender>
        <asp:Button ID="btnShowId" runat="server" onclick="btnShowId_Click" 
            Text="Show" />
        <asp:PlaceHolder ID="phShowId" runat="server" Visible="False"></asp:PlaceHolder>
    </asp:Panel>
    <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearchText">
        <asp:Label ID="lblSearchText" runat="server" 
            Text="Search for text which name contains : "></asp:Label>
        <asp:TextBox ID="tbSearchFor" runat="server" Columns="40"></asp:TextBox>
        <asp:Button ID="btnSearchText" runat="server" onclick="btnSearchText_Click" 
            Text="Search" />
        <asp:PlaceHolder ID="phSearch" runat="server"></asp:PlaceHolder>
    </asp:Panel>
    <p>
        <asp:Table ID="tblMissing" runat="server" CellSpacing="5" Visible="False" 
            BorderColor="#CCCCCC" Width="100%" CssClass="searchPageRatings">
        </asp:Table>
    </p>
    <p>
        <asp:Table ID="tblLastTexts" runat="server" CellSpacing="5" 
            BorderColor="#CCCCCC" Width="100%">
        </asp:Table>
    </p>
    
    <asp:HiddenField ID="hfId" runat="server" />
    <asp:HiddenField ID="hfShowLast" runat="server" />
    <asp:HiddenField ID="hfShowTypes" runat="server" />
    <asp:HiddenField ID="hfVisible" runat="server" />
    
    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FFCCFF">
        <asp:Label ID="lblError" runat="server" Text="Error : " CssClass="errors"></asp:Label>
        <asp:TextBox ID="tbDescription" runat="server" Columns="50" Rows="5" 
            TextMode="MultiLine"></asp:TextBox>
    </asp:Panel>
        
</asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="head">

    </asp:Content>

