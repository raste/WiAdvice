<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="UserInterface.Reports" MasterPageFile="MasterPage.Master" Theme="MainTheme" %>


<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">
    <asp:Panel ID="pnlShowAbout" runat="server" CssClass="accordionHeaders">
        <asp:Label ID="lblShowAbout" runat="server" Text="Show about"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="pnlAbout" runat="server">
        <asp:Label ID="lblAbout" runat="server" Text="About Register Page"></asp:Label>
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
    <br />
    <table style="width: 100%; empty-cells: hide" class="margins">
        <tr>
            <td align="right">&nbsp;</td>
            <td class="style16">Number</td>
            <td align="center" style="width: 100px;">&nbsp;Report Type&nbsp;</td>
            <td align="center">&nbsp;About Types&nbsp;</td>
            <td class="style30" align="center">View Mode</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="right" style="width: 100px;">&nbsp;Show last :&nbsp;&nbsp;
            </td>
            <td class="style16">
                <asp:TextBox ID="tbLastNumReports" runat="server" Columns="7">50</asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbLastNumReports_FilteredTextBoxExtender"
                    runat="server" FilterType="Numbers" TargetControlID="tbLastNumReports">
                </ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td>
                <asp:DropDownList ID="ddlTypeReport" runat="server">
                </asp:DropDownList>
            </td>
            <td style="width: 100px;">
                <asp:DropDownList ID="ddlAboutType" runat="server">
                </asp:DropDownList>
            </td>
            <td class="style30" align="center">
                <asp:RadioButtonList ID="rblViewedMode" runat="server"
                    RepeatDirection="Horizontal" CssClass="searchPageRatings">
                </asp:RadioButtonList>
            </td>
            <td>
                <asp:Button ID="btnGetNoType" runat="server" OnClick="btnGetNoType_Click"
                    Text="Show" />
            </td>
        </tr>
        <tr>
            <td colspan="6">&nbsp;&nbsp;&nbsp;&nbsp;<asp:PlaceHolder ID="ph1" runat="server"
                Visible="False"></asp:PlaceHolder>
            </td>
        </tr>
    </table>
    <table style="width: 100%;" class="margins">
        <tr>
            <td>&nbsp;</td>
            <td class="style28">Number</td>
            <td>Report Type</td>
            <td style="width: 100px;">About Type</td>
            <td class="style23" align="center">with ID</td>
            <td align="center" class="style24">View Mode</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="right" style="width: 100px;">Show last :</td>
            <td class="style28">
                <asp:TextBox ID="tbLastNumReports2" runat="server" Columns="7">50</asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbLastNumReports2_FilteredTextBoxExtender"
                    runat="server" FilterType="Numbers" TargetControlID="tbLastNumReports2">
                </ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td style="width: 100px;">
                <asp:DropDownList ID="ddlTypeReport2" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="ddlAboutType2" runat="server">
                </asp:DropDownList>
            </td>
            <td class="style23">
                <asp:TextBox ID="tbAboutTypeId" runat="server" Columns="7"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="tbAboutTypeId_FilteredTextBoxExtender"
                    runat="server" FilterType="Numbers" TargetControlID="tbAboutTypeId">
                </ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td align="center" style="width: 400px;">
                <asp:RadioButtonList ID="rblViewedMode2" runat="server"
                    RepeatDirection="Horizontal" CssClass="searchPageRatings">
                </asp:RadioButtonList>
            </td>
            <td>
                <asp:Button ID="btnGetReportsAboutType" runat="server"
                    OnClick="btnGetReportsAboutType_Click" Text="Show" />
            </td>
        </tr>
        <tr>
            <td colspan="7">&nbsp;&nbsp; &nbsp;<asp:PlaceHolder ID="ph2" runat="server" Visible="False"></asp:PlaceHolder>
            </td>
        </tr>
    </table>
    <asp:Table ID="tblReports" runat="server" Visible="False"
        CellPadding="0" CellSpacing="5" Width="100%" CssClass="margins" BorderColor="#CCCCCC">
    </asp:Table>

    <asp:HiddenField ID="hfReplyToReport" runat="server" />
    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FFCCFF">
        <asp:Label ID="lblError" runat="server" Text="Error:" CssClass="errors"></asp:Label>
        <asp:Label ID="lblSucces" runat="server" ForeColor="#009900" Text="Succes :"></asp:Label>
        <br />
        <br />
        <asp:Panel ID="pnlReplyToReport" runat="server" CssClass="editBGR"
            DefaultButton="btnReplyToReport" Visible="False">
            <hr />
            <table style="width: 100%;">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblReportInfo" runat="server" Text="Info:"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="right">Report Description :</td>
                    <td>
                        <asp:TextBox ID="tbReportDescription" runat="server" Columns="70"
                            Enabled="False" Rows="10" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="right">Your Reply :</td>
                    <td>
                        <asp:TextBox ID="tbReportReply" runat="server" Columns="70" Rows="10"
                            TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnReplyToReport" runat="server"
                            OnClick="btnReplyToReport_Click" Text="Submit" />
                    </td>
                    <td>&nbsp;<asp:Button ID="btnCancelReply" runat="server" OnClick="btnCancelReply_Click"
                        Text="Cancel" />
                        &nbsp;
                        <asp:PlaceHolder ID="phLblErrorReply" runat="server" Visible="False"></asp:PlaceHolder>
                    </td>
                </tr>
            </table>
            <hr />
        </asp:Panel>
        <br />
        <br />
    </asp:Panel>

    <asp:Panel ID="pnlSendReplyToReport" CssClass="pnlPopUpDefault" runat="server" Width="600px">

        <div class="sectionTextHeader">Reply to report</div>
        <textarea id="tbReplyDescription" class="standardTextBoxes" rows="8"
            style="margin-top: 5px; margin-bottom: 5px; width: 586px;"></textarea>
        <br />

        <input id="btnSendReply" type="button" class="htmlButtonStyle"
            onclick="SendReplyToReport()" value="Send" />
        <input id="btnCancelSendReply" type="button" class="htmlButtonStyle" value="Cancel" onclick="hideReplyToReportData()" />
        <asp:Panel ID="pnlShowPatterns" runat="server" CssClass="accordionHeaders" Style="margin-top: 2px;">
            <asp:Label ID="lblShowPatterns" runat="server" Text="Show patterns"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="pnlPatters" runat="server">
            <asp:Table ID="tblPatterns" runat="server" Width="100%"></asp:Table>
        </asp:Panel>
        <ajaxToolkit:CollapsiblePanelExtender ID="pnlPatters_CollapsiblePanelExtender"
            runat="server" Enabled="True" TargetControlID="pnlPatters"
            ExpandControlID="pnlShowPatterns" CollapseControlID="pnlShowPatterns"
            Collapsed="True" CollapsedSize="0" CollapsedText="Show patterns"
            ExpandedText="Hide patterns" TextLabelID="lblShowPatterns">
        </ajaxToolkit:CollapsiblePanelExtender>
    </asp:Panel>

    <asp:Panel ID="pnlReplyToReportEnd" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle"></asp:Panel>

    <asp:Panel ID="pnlPopUpTypeSuggestion" Style="visibility: hidden;" runat="server">
        <asp:Label ID="lblTypeSuggestion" runat="server" Text=""></asp:Label>
    </asp:Panel>


</asp:Content>


<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">

    <style type="text/css">
        .style16 {
            width: 41px;
        }

        .style23 {
            width: 60px;
        }

        .style24 {
            width: 296px;
        }

        .style28 {
            width: 56px;
        }

        .style30 {
            width: 400px;
        }
    </style>

</asp:Content>

