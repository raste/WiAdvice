<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="UserInterface.Search" MasterPageFile="~/MasterPage.Master" Theme="MainTheme" %>


<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>


<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">

    <div style="margin-bottom: 10px; margin-top: 10px;">
        <div class="trb">
            <div class="tlb">
                <div class="brb2">
                    <div class="blb2">


                        <div class="blhr">

                            <div class="contentBoxTopBottomHr">

                                <asp:Panel ID="pnlAdvSearch" runat="server" Visible="False">

                                    <div style="text-align: center">
                                        <asp:Label ID="lblAdvSearch" runat="server"
                                            Text="Advanced search" CssClass="textHeader"></asp:Label>

                                        <asp:Panel ID="pnlGenSearch" runat="server" DefaultButton="btnAdvSearch" Style="padding-top: 3px;">
                                            <asp:Label ID="lblSearch" runat="server" Text="search :"></asp:Label>
                                            <asp:TextBox ID="tbSearch" runat="server" Columns="30"
                                                ValidationGroup="13"></asp:TextBox>
                                            <asp:Label ID="lblSearchIn" runat="server" Text="in :"></asp:Label>
                                            &nbsp;<asp:CheckBoxList ID="cblSearchOptions" runat="server"
                                                RepeatDirection="Horizontal" RepeatLayout="Flow">
                                            </asp:CheckBoxList>
                                            <cc1:DecoratedButton ID="btnAdvSearch" runat="server"
                                                OnClick="btnAdvSearch_Click" Text="Search" />
                                            <asp:RequiredFieldValidator ID="rfvGenSearch" runat="server"
                                                ControlToValidate="tbSearch" ValidationGroup="13"></asp:RequiredFieldValidator>
                                        </asp:Panel>
                                    </div>

                                    <hr style="margin-bottom: 0px" />
                                </asp:Panel>

                                <table id="Table1" runat="server" style="width: 100%;">
                                    <tr>
                                        <td valign="top">
                                            <br />
                                            <asp:Label ID="lblError" runat="server" Text="Error :" Visible="False"
                                                CssClass="errors"></asp:Label>

                                            <asp:Panel ID="pnlUsers" Style="margin: 10px 0px 10px 0px;" runat="server">
                                            </asp:Panel>

                                            <asp:Panel ID="pnlCategories" Style="margin: 10px 0px 10px 0px;" runat="server">
                                            </asp:Panel>

                                            <asp:Panel ID="pnlCompaniesResults" Style="margin: 10px 0px 10px 0px;" runat="server">
                                            </asp:Panel>

                                            <asp:Panel ID="pnlProductResults" Style="margin: 10px 0px 10px 0px;" runat="server">
                                            </asp:Panel>


                                        </td>
                                        <td id="adCell" width="1">
                                            <asp:PlaceHolder ID="phAdvert" runat="server" Visible="False"></asp:PlaceHolder>
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


    <asp:Panel ID="pnlHidden" runat="server" Visible="False">
        <asp:Label ID="lblSearchResults" runat="server" Text="Search Results"></asp:Label>
        <br />
        <asp:Panel ID="pnlMakerSearch" runat="server" DefaultButton="btnSearchMakers"
            Style="padding-left: 200px;">
            <asp:Label ID="lblSearchMaker" runat="server"
                Text="search for makers with type : "></asp:Label>
            <asp:DropDownList ID="ddlMakers" runat="server">
            </asp:DropDownList>
            <asp:TextBox ID="tbSearchMaker" runat="server" Columns="30"
                ValidationGroup="14"></asp:TextBox>
            <cc1:DecoratedButton ID="btnSearchMakers" runat="server"
                OnClick="btnSearchMakers_Click" Text="Search" />
            <asp:RequiredFieldValidator ID="rfvMakers" runat="server"
                ControlToValidate="tbSearchMaker" ValidationGroup="14"></asp:RequiredFieldValidator>
        </asp:Panel>
    </asp:Panel>
    <asp:Panel ID="pnlPopUp" runat="server" CssClass="pnlPopUpStyle roundedCorners5" Width="450px">
    </asp:Panel>
</asp:Content>


