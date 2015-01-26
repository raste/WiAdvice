<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rules.aspx.cs" Inherits="UserInterface.Rules" MasterPageFile="~/MasterPage.Master" Theme="MainTheme" %>


<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">


    <div>
        <div class="brb2">
            <div class="blb2">
                <div class="blhr">
                    <div class="contentBoxBottomHr">



                        <div class="sectionTextHeader" style="margin-top: 10px; margin-bottom: 10px;">
                            <asp:Label ID="lblPageIntro" runat="server" Text="Site guide" Font-Size="22px"
                                ForeColor="#C02E29"></asp:Label>
                        </div>
                        <div style="margin-left: 20px;">
                            <asp:Label ID="lblAbout" runat="server" Text="Rules About"></asp:Label>
                        </div>
                        <br />

                        <asp:Table ID="tblRules" runat="server" Width="100%">
                        </asp:Table>



                    </div>

                    <img src="images/SiteImages/horL.png" align="left" />
                    <img src="images/SiteImages/horR.png" align="right" />

                </div>
            </div>
        </div>
    </div>


    <div style="margin-bottom: 10px;">
        <div class="trb2">
            <div class="tlb2">


                <div class="brb2">
                    <div class="blb2">

                        <div class="tlhr">

                            <img src="images/SiteImages/horL.png" align="left" />
                            <img src="images/SiteImages/horR.png" align="right" />

                            <div class="blhr2">

                                <div class="contentBoxTopBottomHr">



                                    <asp:PlaceHolder ID="phRules" runat="server"></asp:PlaceHolder>



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





    <asp:Panel ID="pnlHidden" runat="server" Visible="False">
        <asp:Table ID="tblRulesDescr" runat="server" Width="100%">
        </asp:Table>
    </asp:Panel>

</asp:Content>
