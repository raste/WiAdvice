<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Advertisements.aspx.cs" Inherits="UserInterface.Advertisements" MasterPageFile="MasterPage.Master" Theme="MainTheme" %>

<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">

    <asp:Panel ID="pnlShowAbout" runat="server" CssClass="accordionHeaders">
        <asp:Label ID="lblShowAbout" runat="server" Text="Show about"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="pnlAbout" runat="server">

        <asp:Label ID="lblAbout" runat="server" Text="Advertisements About"></asp:Label>
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
    <table class="editBGR" style="width: 100%">
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:Label ID="lblAddEditAdvert" runat="server" Text="Add new Advertisement"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 200px;">
                <asp:Label ID="lblHtml" runat="server" Text="Html : "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbHtml" runat="server" Columns="70" Rows="5"
                    TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblFile" runat="server" Text="Upload File : "></asp:Label>
            </td>
            <td>
                <asp:FileUpload ID="fuUpload" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblTargetUrl" runat="server" Text="Advert url : "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbAdUrl" runat="server" Columns="40"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblCategory" runat="server" Text="Category IDs : "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbCategoryID" runat="server" Columns="30"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbCategoryID_FilteredTextBoxExtender"
                    runat="server" TargetControlID="tbCategoryID" ValidChars="0123456789,">
                </ajaxToolkit:FilteredTextBoxExtender>
                &nbsp;<asp:Label ID="lblCurrCatIDs" runat="server" Text="Current Category ID links"
                    Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblCompany" runat="server" Text="Company IDs : "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbCompanyID" runat="server" Columns="30"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbCompanyID_FilteredTextBoxExtender"
                    runat="server" TargetControlID="tbCompanyID" ValidChars="0123456789,">
                </ajaxToolkit:FilteredTextBoxExtender>
                &nbsp;<asp:Label ID="lblCurrCompanyIDs" runat="server"
                    Text="Current Company ID links" Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblProduct" runat="server" Text="Product IDs : "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbProductID" runat="server" Columns="30"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbProductID_FilteredTextBoxExtender"
                    runat="server" TargetControlID="tbProductID" ValidChars="0123456789,">
                </ajaxToolkit:FilteredTextBoxExtender>
                &nbsp;<asp:Label ID="lblCurrProductIDs" runat="server"
                    Text="Current Product ID Links" Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblExpireDate" runat="server" Text="Expire Date : "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbExpireDate" runat="server" Columns="40"></asp:TextBox>

                <ajaxToolkit:PopupControlExtender ID="tbExpireDate_PopupControlExtender"
                    runat="server" TargetControlID="tbExpireDate" PopupControlID="PopUp" Position="Bottom">
                </ajaxToolkit:PopupControlExtender>

                <ajaxToolkit:FilteredTextBoxExtender ID="tbExpireDate_FilteredTextBoxExtender"
                    runat="server" FilterType="Custom, Numbers" TargetControlID="tbExpireDate"
                    ValidChars="/">
                </ajaxToolkit:FilteredTextBoxExtender>
                &nbsp;<asp:Label ID="lblDateFormat" runat="server" Text="mm/dd/yyyy"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:CheckBoxList ID="cbActVisGen" runat="server" RepeatDirection="Horizontal"
                    RepeatLayout="Flow">
                </asp:CheckBoxList>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblInfo" runat="server" Text="Information : "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbAdInfo" runat="server" Columns="70" Rows="5"
                    TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click"
                    Text="Submit" />
            </td>
            <td>

                <asp:Button ID="btnPreviewAd" runat="server" Text="Preview"
                    OnClick="btnPreviewAd_Click" />
                &nbsp;<asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click"
                    Text="Cancel" Visible="False" />
                &nbsp;<asp:PlaceHolder ID="phAddEditAdvert" runat="server" Visible="False"></asp:PlaceHolder>
            </td>
        </tr>
    </table>
    <p>
        <asp:PlaceHolder ID="phPreviewAdvert" runat="server" Visible="False"></asp:PlaceHolder>
    </p>
    <p>
    </p>
    <asp:Panel ID="pnlShowAdverts" runat="server" DefaultButton="btnShow">

        <table>
            <tr>
                <td>&nbsp;</td>
                <td class="style3">Number</td>
                <td style="width: 50px;">Visible</td>
                <td style="width: 50px;">General</td>
                <td style="width: 50px;">Active</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td align="right" style="width: 200px;">
                    <asp:Label ID="lblShowAdverts" runat="server" Text="Show Last Adverts : "></asp:Label>
                </td>
                <td class="style3">
                    <asp:TextBox ID="tbShowLastNum" runat="server" Columns="7">50</asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="tbShowLastNum_FilteredTextBoxExtender"
                        runat="server" FilterType="Numbers" TargetControlID="tbShowLastNum">
                    </ajaxToolkit:FilteredTextBoxExtender>
                </td>
                <td>
                    <asp:DropDownList ID="ddVisible" runat="server">
                        <asp:ListItem Value="0">all</asp:ListItem>
                        <asp:ListItem Value="1">yes</asp:ListItem>
                        <asp:ListItem Value="2">no</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="ddGeneal" runat="server">
                        <asp:ListItem Value="0">all</asp:ListItem>
                        <asp:ListItem Value="1">yes</asp:ListItem>
                        <asp:ListItem Value="2">no</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="ddActive" runat="server">
                        <asp:ListItem Value="0">all</asp:ListItem>
                        <asp:ListItem Value="1">yes</asp:ListItem>
                        <asp:ListItem Value="2">no</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="btnShow" runat="server" Text="Show" OnClick="btnShow_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <asp:PlaceHolder ID="phShowAds" runat="server" Visible="False"></asp:PlaceHolder>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />

    <asp:Panel ID="pnlShowAdvert" runat="server" DefaultButton="btnShowAdvert">


        <table>
            <tr>
                <td></td>
                <td align="center">Type</td>
                <td class="style11" align="center">Type ID</td>
                <td align="center">Level</td>
                <td class="style12"></td>
            </tr>
            <tr>
                <td align="right" style="width: 270px;">
                    <asp:Label ID="lblShowAdvert" runat="server"
                        Text="Show visible and active adverts for : "></asp:Label>
                </td>
                <td class="style7">
                    <asp:DropDownList ID="ddlShowType" runat="server">
                    </asp:DropDownList>
                </td>
                <td class="style8">
                    <asp:TextBox ID="tbShowID" runat="server" Columns="7"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="tbShowID_FilteredTextBoxExtender"
                        runat="server" FilterType="Numbers" TargetControlID="tbShowID">
                    </ajaxToolkit:FilteredTextBoxExtender>
                </td>
                <td align="center" style="width: 150px;">
                    <asp:RadioButtonList ID="rblAdvertsLevel" runat="server"
                        RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True">1</asp:ListItem>
                        <asp:ListItem Value="2">1,2</asp:ListItem>
                        <asp:ListItem Value="3">all</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td>
                    <asp:Button ID="btnShowAdvert" runat="server" Text="Show"
                        OnClick="btnShowAdvert_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <asp:PlaceHolder ID="phShowAd" runat="server" Visible="False"></asp:PlaceHolder>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <p>
        <asp:Table ID="tblAdverts" runat="server" BorderColor="#CCCCCC" CellSpacing="5"
            CssClass="margins" Width="100%">
        </asp:Table>
    </p>

    <asp:HiddenField ID="hfAdToEdit" runat="server" />



    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FFCCFF">
        <asp:Label ID="lblError" runat="server" Text="Error : " CssClass="errors"></asp:Label>
        <br />
        <br />
        <br />
        <br />
    </asp:Panel>

    <asp:Panel ID="PopUp" runat="server" CssClass="popUpControl" Width="220px">
        <asp:Calendar ID="cExpireDate" runat="server"
            Height="115px" BackColor="White" BorderColor="Black" BorderStyle="Solid"
            BorderWidth="1px" OnSelectionChanged="cExpireDate_SelectionChanged"></asp:Calendar>
    </asp:Panel>

</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">

    <style type="text/css">
        .style3 {
            width: 55px;
        }

        .style7 {
            width: 82px;
        }

        .style8 {
            width: 56px;
        }

        .style11 {
            width: 56px;
            height: 23px;
        }

        .style12 {
            height: 23px;
        }
    </style>

</asp:Content>
