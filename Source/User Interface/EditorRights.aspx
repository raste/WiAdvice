<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="EditorRights.aspx.cs" Inherits="UserInterface.EditorRights" Theme="MainTheme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="margin-top: 10px; margin-bottom: 10px;">
        <div class="trb">
            <div class="tlb">
                <div class="brb2">
                    <div class="blb2">


                        <div class="blhr">

                            <div class="contentBoxTopBottomHr">



                                <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False"
                                    CssClass="usrNotificationPnl">
                                    <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
                                </asp:Panel>
                                <div style="padding-left: 20px;">
                                    <asp:PlaceHolder ID="phInfo" runat="server"></asp:PlaceHolder>
                                    <br />
                                    <asp:Panel ID="pnlInfo" runat="server">
                                        <asp:Label ID="lblInfo3" runat="server" CssClass="searchPageRatings"
                                            Text="log in info"></asp:Label>
                                    </asp:Panel>
                                    <br />
                                </div>
                                <asp:PlaceHolder ID="phTransfers" runat="server"></asp:PlaceHolder>
                                <asp:Panel ID="pnlEditRolesInfo" runat="server" Style="text-align: center;">
                                    <asp:Label ID="lblRolesInfo" runat="server" CssClass="textHeader" Text="Roles info"></asp:Label>
                                </asp:Panel>

                                <p>
                                    <asp:PlaceHolder ID="phEditRoles" runat="server"></asp:PlaceHolder>
                                    <asp:Panel ID="pnlReadHowToAdd" runat="server" HorizontalAlign="Right" Visible="false">
                                        <asp:Label ID="lblHowToHaveEditRights" runat="server"
                                            Text="Main way to have edit rights is by adding companies and/or products."></asp:Label>
                                        <asp:Panel ID="pnlLearnHowToAddProduct" runat="server">
                                            <asp:HyperLink ID="hlClickToAddProduct" Target="_blank" runat="server">Click here</asp:HyperLink>
                                            &nbsp;<asp:Label ID="lblClickToAddProduct" runat="server" Text="to add product or"></asp:Label>
                                            &nbsp;<asp:HyperLink ID="hlClickToReadHowToAddProduct" Target="_blank" runat="server">click here</asp:HyperLink>
                                            &nbsp;<asp:Label ID="lblClickToReadHowToAddProduct" runat="server" Text="to read how to add product."></asp:Label>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlLearnHowToAddCompany" runat="server">
                                            <asp:HyperLink ID="hlClickToAddCompany" Target="_blank" runat="server">Click here</asp:HyperLink>
                                            &nbsp;<asp:Label ID="lblClickToAddCompany" runat="server" Text="to add company or"></asp:Label>
                                            &nbsp;<asp:HyperLink ID="hlClickToReadHowToAddCompany" Target="_blank" runat="server">click here</asp:HyperLink>
                                            &nbsp;<asp:Label ID="lblClickToReadHowToAddCompany" runat="server" Text="to read how to add company."></asp:Label>
                                        </asp:Panel>
                                    </asp:Panel>
                                </p>





                            </div>

                            <img src="images/SiteImages/horL.png" align="left" />
                            <img src="images/SiteImages/horR.png" align="right" />

                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>




    <asp:Panel ID="pnlHidden" runat="server" BackColor="#FFCCFF" Visible="False">
        <asp:Label ID="lblInfo" runat="server">Info</asp:Label>
    </asp:Panel>

    <asp:Panel ID="pnlTransferEnd" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5"></asp:Panel>

    <asp:Panel ID="pnlTransfer" CssClass="pnlPopUpSendMessage roundedCorners5" runat="server" Width="350px">
        <div class="sectionTextHeader">
            <asp:Label ID="lblTransferRole" runat="server" Text="Role name"></asp:Label>
        </div>
        <asp:Label ID="lblTransferTo" runat="server" Text="Transfer to :"></asp:Label>
        <input id="tbTransferTo" class="standardTextBoxes" type="text" style="margin-top: 5px; width: 170px;" />
        <textarea id="tbTransferDescription" class="standardTextBoxes" rows="5"
            style="margin-top: 5px; margin-bottom: 5px; width: 336px;"></textarea>
        <br />

        <input id="btnCreateTransfer" runat="server" type="button" class="htmlButtonStyle"
            onclick="CreateTransfer()" value="Send" />
        <input id="btnCancelTransfer" runat="server" type="button" class="htmlButtonStyle" value="Cancel" onclick="hideTransferData()" />
        <br />
    </asp:Panel>

    <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle roundedCorners5"></asp:Panel>
</asp:Content>
