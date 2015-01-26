<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MyReports.aspx.cs" Inherits="UserInterface.MyReports" Theme="MainTheme" %>

<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <br />
    <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False"
        CssClass="usrNotificationPnl">
        <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
    </asp:Panel>


    <asp:Label ID="lblRemIrrReports" runat="server" Style="padding-left: 30px;" CssClass="searchPageComments"
        Text="Remaining reports"></asp:Label>
    <br />
    <asp:Label ID="lblRemSpamReports" runat="server" Style="padding-left: 30px;" CssClass="searchPageComments"
        Text="Remaining spam reports"></asp:Label>



    <div style="margin: 10px 0px 10px 0px;">
        <div class="trb">
            <div class="tlb">
                <div class="brb2">
                    <div class="blb2">


                        <div class="blhr">

                            <div class="contentBoxTopBottomHr">

                                <asp:Panel ID="pnlReport" runat="server" Visible="False" CssClass="reportsBGR" DefaultButton="btnSubmitReport"
                                    BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" Style="padding: 5px; margin-bottom: 10px;">
                                    <div class="sectionTextHeader" style="padding: 5px 0px 5px 0px;">
                                        <asp:Label ID="lblReport" ForeColor="#C02E29" runat="server" Text="Report irregularity"></asp:Label>
                                    </div>

                                    <table style="width: 100%;">
                                        <tr>
                                            <td style="width: 1px; padding-left: 20px;" valign="top" colspan="1" rowspan="1">
                                                <asp:TextBox ID="tbReport" runat="server" Columns="65" CssClass="margins"
                                                    Rows="11" TextMode="MultiLine" ValidationGroup="11"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvReport" runat="server"
                                                    ControlToValidate="tbReport" ValidationGroup="9"></asp:RequiredFieldValidator>
                                            </td>
                                            <td style="padding: 0px 20px 0px 10px;" colspan="1" rowspan="1">
                                                <asp:Label ID="lblAboutReporting" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>



                                    <div style="padding: 5px 0px 8px 20px;">
                                        <cc1:DecoratedButton ID="btnSubmitReport" runat="server"
                                            OnClick="btnSubmitReport_Click" Text="Report" ValidationGroup="9" />
                                        &nbsp;<asp:PlaceHolder ID="phReport" runat="server" Visible="False"></asp:PlaceHolder>
                                    </div>
                                </asp:Panel>


                                <asp:PlaceHolder ID="phReports" runat="server"></asp:PlaceHolder>

                            </div>

                            <img src="images/SiteImages/horL.png" align="left" />
                            <img src="images/SiteImages/horR.png" align="right" />

                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>









    <asp:HiddenField ID="hfReplyToReport" runat="server" />
    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FFCCFF">
        <asp:Label ID="lblError" runat="server" Text="Error:" CssClass="errors"></asp:Label>
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />
    </asp:Panel>
    <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle roundedCorners5"></asp:Panel>

    <asp:Panel ID="pnlSendReplyToReport" CssClass="pnlPopUpSendMessage roundedCorners5" runat="server" Width="350px">
        <div class="sectionTextHeader">
            <asp:Label ID="lblReplyToReport" runat="server" Text="Label"></asp:Label>
        </div>

        <textarea id="tbReplyDescription" class="standardTextBoxes" rows="5"
            style="margin-top: 5px; margin-bottom: 5px; width: 336px;"></textarea>
        <br />

        <input id="btnSendReply" runat="server" type="button" class="htmlButtonStyle"
            onclick="SendReplyToReport()" value="Send" />
        <input id="btnCancelSendReply" runat="server" type="button" class="htmlButtonStyle" value="Cancel" onclick="hideReplyToReportData()" />
        <br />
    </asp:Panel>

    <asp:Panel ID="pnlReplyToReportEnd" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5"></asp:Panel>

    <asp:Panel ID="pnlPopUpTypeSuggestion" Style="visibility: hidden;" runat="server">
        <asp:Label ID="lblTypeSuggestion" runat="server" Text=""></asp:Label>
    </asp:Panel>

</asp:Content>
