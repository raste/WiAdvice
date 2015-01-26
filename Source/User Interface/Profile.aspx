<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="UserInterface.Profile" MasterPageFile="MasterPage.Master" Theme="MainTheme" %>


<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>


<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">


    <div>
        <div class="brb2">
            <div class="blb2">
                <div class="blhr">
                    <div class="contentBoxBottomHr">



                        <div style="padding-top: 10px;">

                            <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False"
                                CssClass="usrNotificationPnl">
                                <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
                            </asp:Panel>

                            <div style="padding-bottom: 8px;">


                                <div runat="server" id="visitedDivBr1">
                                    <br />
                                </div>
                                <div runat="server" id="visitedDivBr2">
                                    <br />
                                </div>



                                <asp:Label ID="lbUserName" Style="margin-left: 10px;" runat="server" Text="Username" Visible="True" ForeColor="#428220"></asp:Label>

                                <asp:Label ID="lbDateCreated" Style="margin-left: 15px;" CssClass="commentsDate" runat="server" Text="Date Created"></asp:Label>

                                <asp:Label ID="lbEmail" runat="server" CssClass="commentsDate" Style="margin-left: 15px;" Text="Email"></asp:Label>

                                <asp:Label ID="lblLastLogIn" Style="margin-left: 15px;" runat="server" CssClass="commentsDate" Text="Last log in" Visible="True"></asp:Label>

                                <div style="padding-right: 10px; text-align: right">
                                    <asp:HyperLink ID="hlEditRoles" Text="Edit roles" runat="server" Visible="False"></asp:HyperLink>
                                    <asp:Label ID="lblSendReport" runat="server" CssClass="sendReport" Text="Report user" Visible="False"></asp:Label>
                                </div>
                            </div>



                            <ajaxToolkit:Accordion ID="accAdmin" runat="server" FramesPerSecond="40" RequireOpenedPane="False" SelectedIndex="-1" Visible="False">
                                <Panes>

                                    <ajaxToolkit:AccordionPane ID="AccordionPane1" runat="server">
                                        <Header>

                                            <asp:Panel ID="pnlShowAdminPnl" runat="server" Visible="True" CssClass="accordionHeaders">
                                                <asp:Label ID="lblAdminPanel" runat="server" CssClass="sectionTextHeader"
                                                    Text="Admin Panel"></asp:Label>
                                            </asp:Panel>

                                        </Header>
                                        <Content>

                                            <asp:Panel ID="pnlEdit" runat="server" Visible="True" CssClass="admBGR">

                                                <asp:Label ID="lblDeleteUser" runat="server" Text="isDeleted : " CssClass="marginsLR"></asp:Label>
                                                <asp:Button ID="btnDeleteUser" runat="server" OnClick="btnDeleteUser_Click" Text="Delete" />

                                                &nbsp;&nbsp;&nbsp;,&nbsp;&nbsp;&nbsp;
                <asp:Label ID="lblUserActivated" runat="server" Text="USer activated"></asp:Label>
                                                <asp:Button ID="btnActivateUser" runat="server" Text="Activate" OnClick="btnActivateUser_Click" />
                                                <br />
                                                <asp:Label ID="lblDelOpinions" runat="server" Text="Delete all user comments (for products and topics) for local variant. (NOTE: cannot be reversed)" CssClass="marginsLR" Visible="False"></asp:Label>
                                                <asp:Button ID="btnDelOpinions" runat="server" Text="Delete Comments" Visible="False" OnClick="btnDelOpinions_Click" />
                                                &nbsp;&nbsp;&nbsp;
                <asp:CheckBox ID="cbSendWarning" runat="server" Text="send warning for each comment" />
                                                <br />
                                                <asp:Label ID="lblChangeUserNameInfo" runat="server" Text="Set change name :" CssClass="marginsLR"></asp:Label>
                                                <asp:Button ID="btnSetChangeNameToUser" runat="server" OnClick="btnSetChangeNameToUser_Click" Text="Change" />
                                                <br />
                                                <asp:Table ID="tblRoles" runat="server" BorderWidth="2px" CellPadding="2" Width="100%"
                                                    CellSpacing="2" GridLines="Both" CssClass="margins" BorderStyle="Solid" BorderColor="Black">
                                                </asp:Table>

                                                <asp:Table ID="tblRolesThatDontHave" runat="server" BorderWidth="2px" Width="100%"
                                                    CellPadding="2" CellSpacing="2" GridLines="Both" CssClass="margins" BorderStyle="Solid" BorderColor="Black">
                                                </asp:Table>

                                                <hr />
                                            </asp:Panel>

                                        </Content>
                                    </ajaxToolkit:AccordionPane>


                                </Panes>
                            </ajaxToolkit:Accordion>



                            <asp:Panel ID="pnlSelfEdit" runat="server" Visible="False">
                                <hr style="margin-bottom: 4px; margin-top: 0px;" />
                                &nbsp;<cc1:DecoratedButton ID="btnShowEditPanel" runat="server"
                                    OnClick="btnShowEditPanel_Click" Text="Edit" />
                                &nbsp;<asp:RadioButtonList ID="rblEditChoices" runat="server"
                                    RepeatDirection="Horizontal" RepeatLayout="Flow">
                                </asp:RadioButtonList>
                                <asp:Panel ID="pnlSelfEditSub" runat="server" Visible="False"
                                    CssClass="margins userPageBGR" DefaultButton="btnSubmitAction">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td align="right" style="width: 200px">
                                                <asp:Label ID="lbEdit1" runat="server" Text="Edit1" Visible="False"></asp:Label>
                                            </td>
                                            <td valign="top">
                                                <asp:TextBox ID="tbEdit1" runat="server" Style="margin-left: 0px"
                                                    Visible="False"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" style="width: 200px" valign="top"></td>
                                            <td valign="top">
                                                <asp:Label ID="lblInfo1" runat="server" Text="Information" Visible="False"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lbEdit2" runat="server" Text="Edit2" Visible="False"></asp:Label>
                                            </td>
                                            <td valign="top">
                                                <asp:TextBox ID="tbEdit2" runat="server" Visible="False"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" valign="top"></td>
                                            <td valign="top">
                                                <asp:Label ID="lblInfo2" runat="server" Text="Information" Visible="False"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" valign="top">
                                                <asp:Label ID="lbEdit3" runat="server" Text="Edit3" Visible="False"></asp:Label>
                                            </td>
                                            <td valign="top">
                                                <asp:TextBox ID="tbEdit3" runat="server" Visible="False"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" valign="top"></td>
                                            <td valign="top">
                                                <asp:Label ID="lblInfo3" runat="server" Text="Information" Visible="False"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right"></td>
                                            <td>
                                                <div class="accordionsDivPadding">
                                                    <cc1:DecoratedButton ID="btnSubmitAction" runat="server"
                                                        OnClick="btnSubmitAction_Click" Text="Submit" Visible="False" />

                                                    <cc1:DecoratedButton ID="btnDiscard" runat="server" OnClick="btnDiscard_Click"
                                                        Text="Cancel" Visible="False" />
                                                    &nbsp;<asp:PlaceHolder ID="phEdit" runat="server" Visible="False"></asp:PlaceHolder>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <hr style="margin-top: 4px;" />
                            </asp:Panel>

                            <ajaxToolkit:Accordion ID="aUserInfo" runat="server" FramesPerSecond="40"
                                RequireOpenedPane="False" SelectedIndex="-1" SuppressHeaderPostbacks="True">
                                <Panes>
                                    <ajaxToolkit:AccordionPane ID="apSuggestions" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlShowSuggestions" runat="server" CssClass="accordionHeaders">
                                                <asp:Label ID="lblSuggestions" runat="server" Text="Suggestions"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:PlaceHolder ID="phSuggestions" runat="server"></asp:PlaceHolder>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                    <ajaxToolkit:AccordionPane ID="apSystemMessages" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlShowSystemMessages" runat="server" CssClass="accordionHeaders">
                                                <asp:Label ID="lblSystemMessages" runat="server" Text="System messages"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:PlaceHolder ID="phSystemMessages" runat="server"></asp:PlaceHolder>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                    <ajaxToolkit:AccordionPane ID="apWarnings" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlShowWarnings" runat="server" CssClass="accordionHeaders">
                                                <asp:Label ID="lblWarnings" runat="server" CssClass="searchPageRatings" Text="Warnings"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:PlaceHolder ID="phWarnings" runat="server"></asp:PlaceHolder>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                </Panes>
                            </ajaxToolkit:Accordion>


                        </div>



                    </div>

                    <img src="images/SiteImages/horL.png" align="left" />
                    <img src="images/SiteImages/horR.png" align="right" />

                </div>
            </div>
        </div>
    </div>




    <div style="margin-bottom: 10px; margin-top: 10px;">
        <div class="trb2">
            <div class="tlb2">


                <div class="brb2">
                    <div class="blb2">

                        <div class="tlhr">

                            <img src="images/SiteImages/horL.png" align="left" />
                            <img src="images/SiteImages/horR.png" align="right" />

                            <div class="blhr2">

                                <div class="contentBoxTopBottomHr">





                                    <table id="Table1" runat="server" style="width: 100%;">
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lbComments" runat="server" Text="Comments" CssClass="commentsLarger"></asp:Label>
                                                <div style="margin: 5px 0px 5px 0px;">
                                                    <asp:Table ID="tblPages" runat="server" CssClass="autoWidth" Style="display: inline-table;">
                                                    </asp:Table>
                                                </div>
                                                <asp:PlaceHolder ID="phComments" runat="server"></asp:PlaceHolder>

                                                <asp:Table ID="tblPagesBtm" runat="server" CssClass="autoWidth" Style="margin: 5px 0px 5px 0px;">
                                                </asp:Table>
                                            </td>
                                            <td id="adCell" width="1">
                                                <asp:PlaceHolder ID="phAdverts" runat="server" Visible="False"></asp:PlaceHolder>
                                            </td>
                                        </tr>
                                    </table>


                                </div>

                                <img src="images/SiteImages/horL.png" align="left" />
                                <img src="images/SiteImages/horR.png" align="right" />

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>





    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FFCCFF">
        <asp:Label ID="lblError" runat="server" Text="Error:" CssClass="errors"></asp:Label>
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lbSignature"
            runat="server" CssClass="searchPageComments" Style="margin-left: 30px;"
            Text="Signature" Visible="False"></asp:Label>
        <br />
        <span>
            <asp:Label ID="lblRemoveUserSignature" runat="server" CssClass="marginsLR"
                Text="Remove user signature :"></asp:Label>
            <asp:Button ID="btnRemoveUserSignature" runat="server"
                OnClick="btnRemoveUserSignature_Click" Text="Delete" />
            <br />
        </span>

        <br />
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlPopUp" runat="server" CssClass="pnlPopUpStyle roundedCorners5" Width="450px">
    </asp:Panel>
    <asp:HiddenField ID="hfReplyToReport" runat="server" />


    <asp:Panel ID="pnlSendReport" runat="server" CssClass="pnlPopUpReport roundedCorners5">
        <div class="sectionTextHeader" style="padding: 5px 0px 5px 0px;">
            <asp:Label ID="lblReportIrregularity" runat="server" Text="Report irregularity" ForeColor="#C02E29"></asp:Label>
        </div>
        <table style="width: 100%;">
            <tr>
                <td style="width: 10; padding-right: 15px;" valign="top">
                    <textarea id="taReportText" class="standardTextBoxes" style="width: 350px; height: 200px;" cols="20" rows="5" name="S1"></textarea></td>
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


</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
</asp:Content>

