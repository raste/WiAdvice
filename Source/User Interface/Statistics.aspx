<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Statistics.aspx.cs" Inherits="UserInterface.Statistics" MasterPageFile="~/MasterPage.Master" Theme="MainTheme"%>


<asp:Content ID="Content1" runat="server" 
    contentplaceholderid="ContentPlaceHolder1">
    <br />
        <asp:Label ID="lblAbout" runat="server" Text="About statistics page."></asp:Label>
    <br />
     <br />
        &nbsp; &nbsp;<asp:Label ID="lblAtm" runat="server" Text="At the moment :"></asp:Label>
    <asp:Button ID="btnShowRegOn" runat="server" CssClass="marginsLR" 
        Text="Show logged users" Visible="False" onclick="btnShowRegOn_Click" />
    <br />
        <table style="width:100%;">
            <tr>
                <td>
                    &nbsp;</td>
                <td class="style23">
                    Date (mm/dd/yyyy)</td>
                <td class="style23">
                    Last Num Days</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td align="right" style="width: 200px;">
                    Show Statistics for :
                </td>
                <td class="style23">
                    <asp:TextBox ID="tbDate" runat="server"></asp:TextBox>
                    <ajaxToolkit:PopupControlExtender ID="tbDate_PopupControlExtender" 
                        runat="server" CommitProperty="value" CommitScript="e.value;" 
                        TargetControlID="tbDate" PopupControlID="PopUp" Position="Bottom">
                    </ajaxToolkit:PopupControlExtender>
                    <ajaxToolkit:FilteredTextBoxExtender ID="tbDate_FilteredTextBoxExtender" 
                        runat="server" FilterType="Custom, Numbers" TargetControlID="tbDate" 
                        ValidChars="/">
                    </ajaxToolkit:FilteredTextBoxExtender>
                </td>
                <td class="style23">
                    <asp:TextBox ID="tbNum" runat="server"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="tbNum_FilteredTextBoxExtender" 
                        runat="server" FilterType="Numbers" TargetControlID="tbNum">
                    </ajaxToolkit:FilteredTextBoxExtender>
                </td>
                <td>
                    <asp:Button ID="btnShowStats" runat="server" Text="Show" onclick="btnShowStats_Click" 
                         />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:PlaceHolder ID="phShowStats" runat="server" 
                        Visible="False"></asp:PlaceHolder>
                </td>
            </tr>
        </table>
        <p>
            <asp:Table ID="tblShowRegOn" runat="server" Visible="False" 
                BorderColor="#CCCCCC" BorderWidth="1px" CssClass="margins" 
                GridLines="Both" Width="100%">
            </asp:Table>
        
        <asp:Table ID="tblLast5Days" runat="server" BorderWidth="1px" BorderColor="#CCCCCC" 
            GridLines="Both" CssClass="margins" Width="100%">
        </asp:Table>
    </p>
    <p>
    </p>
    <asp:Panel ID="pnlHidden" runat="server" Visible="False">
        <asp:Label ID="lblError" runat="server" Text="Error :" CssClass="errors"></asp:Label>
        <br />
        <br />
        <br />
    </asp:Panel>
    
    <asp:Panel ID="PopUp" runat="server" CssClass="popUpControl" Width="220px">
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <asp:Calendar ID="cExpireDate" runat="server" 
            Height="115px" BackColor="White" BorderColor="Black" BorderStyle="Solid" 
                        BorderWidth="1px" onselectionchanged="cExpireDate_SelectionChanged">
                    </asp:Calendar>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
    
    </asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="head">

    <style type="text/css">
        .style23
        {
            width: 135px;
        }
    </style>

</asp:Content>

