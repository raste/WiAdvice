<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Guide.aspx.cs" Inherits="UserInterface.Guide" Theme="MainTheme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div>
        <div class="brb2">
            <div class="blb2">
                <div class="blhr">
                    <div class="contentBoxBottomHr">


                        <div class="sectionTextHeader" style="margin-top: 10px; margin-bottom: 10px;">
                            <asp:Label ID="lblPageIntro" runat="server" Text="Site guide"></asp:Label>
                        </div>

                        <div style="margin-left: 20px;">
                            <asp:Label ID="lblAbout" runat="server" Text="Guide About"></asp:Label>
                        </div>
                        <br />

                        <asp:Table ID="tblInformation" runat="server" Width="100%">
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




                                    <asp:PlaceHolder ID="phInformation" runat="server"></asp:PlaceHolder>




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
        <asp:Table ID="tblInformationDescr" runat="server" Width="100%">
        </asp:Table>
    </asp:Panel>


</asp:Content>
