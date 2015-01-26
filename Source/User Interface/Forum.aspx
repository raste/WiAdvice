<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Forum.aspx.cs"
    Inherits="UserInterface.Forum" Theme="MainTheme" %>

<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1 {
            width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div style="margin: 10px 0px 10px 0px;">
        <div class="trb">
            <div class="tlb">
                <div class="brb2">
                    <div class="blb2">


                        <div class="blhr">

                            <div class="contentBoxTopBottomHr">





                                <div class="sectionTextHeader" style="margin: 5px 0px 10px 0px;">
                                    <asp:Label ID="lblForumFor" runat="server" Text="Forum"></asp:Label>
                                    <asp:HyperLink ID="hlProduct" runat="server" Style="font-size: 21px;">Product</asp:HyperLink>
                                </div>

                                <div class="clearfix" style="margin-bottom: 15px;">

                                    <asp:Label ID="lblNoPostedTopics" runat="server" Visible="false" CssClass="errors" Style="margin-right: 10px;" Text="No posted topics"></asp:Label>

                                    <asp:Panel ID="pnlAddTopicTop" runat="server" Style="display: inline-block;" CssClass="">

                                        <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle' id="tblAddTopicTop" runat="server">
                                            <tr>
                                                <td>
                                                    <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                                                </td>
                                                <td>
                                                    <input id="btnAddTopicTop" runat="server" type="button" value="Add topic" class="defaultDecButton" />
                                                </td>
                                                <td>
                                                    <img alt="" src="images/SiteImages/btnBGRRight.png" />
                                                </td>
                                            </tr>
                                        </table>


                                        <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle' id="notifyTbl" runat="server">
                                            <tr>
                                                <td>
                                                    <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                                                </td>
                                                <td>
                                                    <input id="btnSignNotifiesTop" runat="server" type="button" value="Notify" class="defaultDecButton" />
                                                </td>
                                                <td>
                                                    <img alt="" src="images/SiteImages/btnBGRRight.png" />
                                                </td>
                                            </tr>
                                        </table>

                                    </asp:Panel>
                                    <div class="floatRightNoMrg">
                                        <asp:PlaceHolder ID="phPagesTop" runat="server"></asp:PlaceHolder>
                                    </div>
                                </div>


                                <asp:PlaceHolder ID="phTopics" runat="server"></asp:PlaceHolder>


                                <div class="clearfix" style="margin-top: 15px;">


                                    <asp:Panel ID="pnlAddTopicBottom" runat="server" Style="display: inline-block;" CssClass="">

                                        <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle' id="tblAddTopicBottom" runat="server">
                                            <tr>
                                                <td>
                                                    <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                                                </td>
                                                <td>
                                                    <input id="btnAddTopicBottom" runat="server" type="button" value="Add topic" class="defaultDecButton" />
                                                </td>
                                                <td>
                                                    <img alt="" src="images/SiteImages/btnBGRRight.png" />
                                                </td>
                                            </tr>
                                        </table>

                                    </asp:Panel>

                                    <asp:Panel ID="pnlRegToAdd" CssClass="panelInline" runat="server">

                                        <asp:Label ID="lblRegToAdd1" runat="server" Text="Искаш и ти да дадеш мнението си? Ами,"></asp:Label>
                                        <asp:HyperLink ID="hlRegToAdd1" runat="server">регистрирай</asp:HyperLink>
                                        /
                                   <asp:HyperLink ID="hlRegToAdd2" runat="server">логни</asp:HyperLink>
                                        <asp:Label ID="lblRegToAdd2" runat="server" Text="се тогава...!"></asp:Label>
                                    </asp:Panel>


                                    <div class="floatRightNoMrg">
                                        <asp:PlaceHolder ID="phPagesBottom" runat="server"></asp:PlaceHolder>
                                    </div>
                                </div>







                            </div>

                            <img src="images/SiteImages/horL.png" align="left" />
                            <img src="images/SiteImages/horR.png" align="right" />

                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>






    <div id="divAddTopic" class="pnlPopUpAddTopic roundedCorners5">

        <div class="sectionTextHeader" style="margin-bottom: 5px;">

            <asp:Label ID="lblAddTopic" runat="server" Text="Add topic"></asp:Label>

        </div>

        <table cellspacing="1" class="style1">

            <tr>
                <td></td>
                <td>
                    <asp:Label ID="lblTopicRules1" runat="server" Text="Rules1"></asp:Label>
                    <asp:HyperLink ID="hlTopicRules" Style="color: #C02E29;" runat="server">Rules</asp:HyperLink>
                    <asp:Label ID="lblTopicRules2" runat="server" Text="Rulse2"></asp:Label>
                </td>
            </tr>

            <tr>
                <td style="width: 150px; text-align: right;">

                    <asp:Label ID="lblTopicSubject" runat="server" Text="Subject :"></asp:Label>

                </td>
                <td>
                    <input id="taTopicSubject" type="text" class="standardTextBoxes" style="width: 300px;" name="S2" />
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">

                    <asp:Label ID="lblTopicDescription" runat="server" Text="Description :"></asp:Label>

                </td>
                <td>
                    <div style="padding: 5px 0px 5px 0px;">


                        <textarea id="taTopicDescription" class="standardTextBoxes" style="width: 580px; height: 200px;" name="S1"></textarea>


                    </div>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <div style="padding: 5px 0px 0px 0px;">
                        <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'>
                            <tr>
                                <td>
                                    <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                                </td>
                                <td>
                                    <input id="btnCreateTopic" runat="server" type="button" value="Submit" class="defaultDecButton" />
                                </td>
                                <td>
                                    <img alt="" src="images/SiteImages/btnBGRRight.png" />
                                </td>
                            </tr>
                        </table>

                        <cc1:TransliterateButton ID="btnTransAddTopivComment" runat="server" Visible="false" />

                        <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'>
                            <tr>
                                <td>
                                    <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                                </td>
                                <td>
                                    <input id="btnHideTopicData" runat="server" type="button" value="Cancel" onclick="HideElementWithID('divAddTopic', 'true')" class="defaultDecButton" />
                                </td>
                                <td>
                                    <img alt="" src="images/SiteImages/btnBGRRight.png" />
                                </td>
                            </tr>
                        </table>



                    </div>
                </td>
            </tr>
        </table>


    </div>

    <asp:Panel ID="pnlAction" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5"></asp:Panel>
    <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle roundedCorners5"></asp:Panel>

</asp:Content>
