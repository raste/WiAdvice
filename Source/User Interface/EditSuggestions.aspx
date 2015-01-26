<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="EditSuggestions.aspx.cs" Inherits="UserInterface.EditSuggestions" Theme="MainTheme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div style="margin-top: 10px; margin-bottom: 10px;">
        <div class="brb2">
            <div class="blb2">
                <div class="blhr">
                    <div class="contentBoxBottomHr">


                        <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False"
                            CssClass="usrNotificationPnl">
                            <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
                        </asp:Panel>
                        <div style="padding-left: 20px;">

                            <asp:Label ID="lblGeneralInfo" runat="server" Text="Info .."></asp:Label>
                            <br />
                        </div>
                        <p>
                        </p>
                        <ajaxToolkit:Accordion ID="accMySuggestions" runat="server" FramesPerSecond="40" RequireOpenedPane="False"
                            SelectedIndex="-1" SuppressHeaderPostbacks="True">
                            <Panes>
                                <ajaxToolkit:AccordionPane ID="apMySuggestions" runat="server">
                                    <Header>

                                        <asp:Panel ID="pnlShowMySuggestions" runat="server" CssClass="accordionHeaders">
                                            <asp:Label ID="lblMySuggestions" runat="server" Text="My suggestions"></asp:Label>
                                        </asp:Panel>
                                    </Header>
                                    <Content>
                                        <asp:PlaceHolder ID="phMySuggestions" runat="server"></asp:PlaceHolder>
                                    </Content>
                                </ajaxToolkit:AccordionPane>
                            </Panes>
                        </ajaxToolkit:Accordion>

                        <ajaxToolkit:Accordion ID="accSuggestionsToMe" runat="server" FramesPerSecond="40" RequireOpenedPane="False"
                            SelectedIndex="-1" SuppressHeaderPostbacks="True">
                            <Panes>
                                <ajaxToolkit:AccordionPane ID="apSuggestionsToMe" runat="server">
                                    <Header>

                                        <asp:Panel ID="pnlShowSuggestionsToMe" runat="server" CssClass="accordionHeaders">
                                            <asp:Label ID="lblSugegstionsToMe" runat="server" Text="Suggestions to me"></asp:Label>
                                        </asp:Panel>
                                    </Header>
                                    <Content>
                                        <asp:PlaceHolder ID="phSuggestionsToMe" runat="server"></asp:PlaceHolder>
                                    </Content>
                                </ajaxToolkit:AccordionPane>
                            </Panes>
                        </ajaxToolkit:Accordion>


                    </div>

                    <img src="images/SiteImages/horL.png" align="left" />
                    <img src="images/SiteImages/horR.png" align="right" />

                </div>
            </div>
        </div>
    </div>


    <asp:Panel ID="pnlAddCommentToSuggestion" CssClass="pnlPopUpSendMessage roundedCorners5" runat="server" Width="350px">
        <div class="sectionTextHeader">
            <asp:Label ID="lblReplySuggestion" runat="server" Text="Reply"></asp:Label>
        </div>
        <div style="text-align: center;">
            <textarea id="tbCommentDescription" class="standardTextBoxes" rows="8" style="margin-top: 5px; margin-bottom: 5px; width: 336px;"></textarea>
        </div>

        &nbsp;<input id="btnAddComment" runat="server" type="button" class="htmlButtonStyle" value="Send" onclick="AddCommentToSuggestion()" />
        <input id="btnCancelComment" runat="server" type="button" class="htmlButtonStyle" value="Cancel" onclick="hideAddCommentToSuggestionData()" />
        <br />
    </asp:Panel>

    <asp:Panel ID="pnlAddCommentToSuggestionEndPnl" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5"></asp:Panel>


    <asp:Panel ID="pnlSendReport" runat="server" CssClass="pnlPopUpReport roundedCorners5">
        <div class="sectionTextHeader" style="padding: 5px 0px 5px 0px;">
            <asp:Label ID="lblReport" runat="server" Text="Report irregularity" ForeColor="#C02E29"></asp:Label>
        </div>
        <table style="width: 100%;">
            <tr>
                <td style="width: 10; padding-right: 15px;" valign="top">
                    <textarea id="taReportText" class="standardTextBoxes" style="width: 350px; height: 220px;" cols="20" rows="5" name="S1"></textarea></td>
                <td valign="middle">
                    <asp:Label ID="lblReporting" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="padding: 10px 0px 0px 0px;">

                    <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'>
                        <tr>
                            <td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td>
                            <td>
                                <input id="btnSendReport" runat="server" type="button" value="Report" onclick="SendReport();" class="defaultDecButton" />
                            </td>
                            <td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td>
                        </tr>
                    </table>

                    <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'>
                        <tr>
                            <td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td>
                            <td>
                                <input id="btnHideRepData" runat="server" type="button" value="Cancel" onclick="HideReportData();" class="defaultDecButton" />
                            </td>
                            <td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td>&nbsp;</td>
            </tr>
        </table>

    </asp:Panel>
    <asp:Panel ID="pnlActionReport" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5"></asp:Panel>

    <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle roundedCorners5"></asp:Panel>
</asp:Content>
