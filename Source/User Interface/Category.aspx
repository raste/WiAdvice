<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Category.aspx.cs" Inherits="UserInterface.CategoryPage" MasterPageFile="MasterPage.Master" Theme="MainTheme" %>

<%@ Register Assembly="Microsoft.Web.GeneratedImage" Namespace="Microsoft.Web" TagPrefix="cc2" %>
<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>


<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">

    <table style="width: 100%; margin-top: 7px;">
        <tr>
            <td>
                <asp:HyperLink ID="imgLink" runat="server">
                    <cc2:GeneratedImage ID="imgCat" runat="server" CssClass="margin" ImageAlign="Left"
                        BorderColor="Black" BorderWidth="1px" BorderStyle="Solid">
                    </cc2:GeneratedImage>
                </asp:HyperLink>

                <p>

                    <asp:PlaceHolder ID="phPath" runat="server"></asp:PlaceHolder>

                </p>


                <asp:Label ID="lblDescription" runat="server" Text="Description :"
                    CssClass="description"></asp:Label>


            </td>
        </tr>
    </table>

    <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False"
        CssClass="usrNotificationPnl">
        <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
    </asp:Panel>

    <ajaxToolkit:Accordion ID="accAdmin" runat="server" Visible="False" FramesPerSecond="40" RequireOpenedPane="False" SelectedIndex="-1">
        <Panes>
            <ajaxToolkit:AccordionPane ID="apAdmin" runat="server">
                <Header>
                    <asp:Panel ID="pnlShowAdminPnl" runat="server" Visible="True" CssClass="accordionHeaders">
                        <asp:Label ID="Label1" runat="server" CssClass="sectionTextHeader"
                            Text="Admin Panel"></asp:Label>
                    </asp:Panel>
                </Header>
                <Content>

                    <asp:Panel ID="pnlGlobal" runat="server" CssClass="admBGR">
                        &nbsp;<asp:Label ID="lblNotify" runat="server" Text="Information :" Visible="False"></asp:Label>

                        <br />
                        <asp:RadioButtonList ID="rblEdit" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow">
                        </asp:RadioButtonList>
                        &nbsp;<asp:Button ID="btnEdit" runat="server" OnClick="btnEdit_Click"
                            Text="Edit" />

                        <asp:Panel ID="pnlSubEdit" runat="server" Visible="False" CssClass="editBGR"
                            DefaultButton="BtnSaveChanges">
                            <hr />
                            <table class="margins" style="width: 100%;">
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblInfo1" runat="server" Text="Information" Visible="False"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width: 250px">
                                        <asp:Label ID="lblEdit1" runat="server" Text="Edit1" Visible="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbEdit1" runat="server" Columns="30" Visible="False"></asp:TextBox>
                                        &nbsp;
                    <asp:Label ID="lblCName" CssClass="errors" runat="server" Text="Check Name"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblInfo2" runat="server" Text="Information" Visible="False"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="lblEdit2" runat="server" Text="Edit2" Visible="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbEdit2" runat="server" Columns="30" Visible="False"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblInfo3" runat="server" Text="Information" Visible="False"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="lblEdit3" runat="server" Text="Edit3" Visible="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbEdit3" runat="server" Columns="30" Visible="False"></asp:TextBox>

                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:CheckBox ID="cbLast" runat="server" Text="Last ?" Visible="False" />
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:Button ID="BtnSaveChanges" runat="server" OnClick="BtnSaveChanges_Click"
                                            Text="Save Changes" Visible="False" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnDiscard" runat="server" OnClick="btnDiscard_Click"
                                            Text="Cancel" Visible="False" />
                                        &nbsp;<asp:PlaceHolder ID="phEdit" runat="server" Visible="False"></asp:PlaceHolder>
                                    </td>
                                </tr>
                            </table>
                            <hr />
                        </asp:Panel>
                        <asp:Panel ID="pnlEditImage" runat="server" Visible="False" CssClass="editBGR">
                            <hr />
                            <table>
                                <tr>
                                    <td style="width: 250px;" align="right">
                                        <asp:Label ID="lblchngImage" runat="server" Text="Change image"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:FileUpload ID="fuImage" runat="server" />
                                        <asp:Button ID="btnUpImg" runat="server" Text="Upload" CssClass="marginsLR" OnClick="btnUpImg_Click" />
                                        <asp:Label ID="lblImgInfo" runat="server" Text="There might be only one image per category."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="lblDelImg" runat="server" Text="Delete image" Visible="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnDelImg" runat="server" Text="Delete" Visible="False" OnClick="btnDelImg_Click" />
                                        <asp:Button ID="btnCancImg" runat="server" CssClass="marginsLR" Text="Cancel" OnClick="btnCancImg_Click" />
                                        <asp:PlaceHolder ID="phEditImg" runat="server" Visible="False"></asp:PlaceHolder>
                                    </td>
                                </tr>
                            </table>
                            <hr />
                        </asp:Panel>
                        <br />

                        <table style="border-collapse: collapse; empty-cells: hide">
                            <tr>
                                <td>

                                    <asp:Label ID="lblVisible" runat="server" Text="Visible"></asp:Label>

                                    <asp:Button ID="btnUndoDelete" runat="server" OnClick="btnUndoDelete_Click"
                                        Text="Make Visible" />
                                    <asp:Label ID="lblUndoDInfo" runat="server" Text="(NOTE : This is the opposite of the DELETE btn action)"></asp:Label>

                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDelete" runat="server" Text="Delete Current Category :"></asp:Label>
                                    <asp:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click"
                                        Text="Delete" Style="height: 26px" />

                                    <asp:Label ID="lblDelInfo" runat="server" Text="(NOTE : if you delete , connections to this Category WILL BE DELETED)"></asp:Label>

                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:PlaceHolder ID="phError" Visible="false" runat="server"></asp:PlaceHolder>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblMakeLast" runat="server" Text="Make Last ?"></asp:Label>
                                    <asp:Button ID="btnMakeLast" runat="server" Enabled="True" Text="Yes"
                                        OnClick="btnMakeLast_Click" />
                                    <asp:Label ID="lblUncheckLast" runat="server" Text="Uncheck Last : "></asp:Label>
                                    <asp:Button ID="btnUncheckLast" runat="server" OnClick="btnUncheckLast_Click"
                                        Text="Yes" />
                                    <asp:Label ID="lblLastInfo" runat="server" Text="Information"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <asp:Label ID="lblShowDeletedProds" runat="server" Text="&nbsp;Deleted products : "></asp:Label>
                        <asp:Button ID="btnShowDeletedProd" runat="server"
                            OnClick="btnShowDeletedProd_Click" Text="Show" />
                        <asp:Table ID="tblShowDeletedProd" runat="server" Visible="False" Width="100%"
                            CssClass="margins" BorderColor="Black" BorderStyle="Solid"
                            BorderWidth="1px" GridLines="Both">
                        </asp:Table>
                        <hr />
                    </asp:Panel>


                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>
    </ajaxToolkit:Accordion>





    <asp:Panel ID="pnlLastPage" runat="server">

        <asp:Table ID="tblSearchAdd" runat="server" Style="margin-left: 5px;" Width="100%">
        </asp:Table>









        <div style="margin-bottom: 9px; margin-top: 9px;">
            <div class="trb2">
                <div class="tlb2">


                    <div class="brb2">
                        <div class="blb2">

                            <div class="tlhr">

                                <img src="images/SiteImages/horL.png" align="left" />
                                <img src="images/SiteImages/horR.png" align="right" />

                                <div class="blhr2">

                                    <div class="contentBoxTopBottomHr">


                                        <asp:Table ID="tblChars" runat="server" Style="width: auto; margin-left: 3px;">
                                        </asp:Table>
                                        <table id="tbl" runat="server" style="width: 100%;">
                                            <tr>
                                                <td id="prodcell" valign="top">
                                                    <asp:Table ID="tblPages" runat="server" Style="width: auto; margin: 0px 0px 5px 0px;">
                                                    </asp:Table>
                                                    <asp:Table ID="tblProducts" runat="server" CellPadding="1"
                                                        CellSpacing="0" Width="100%" CssClass="bluePanels">
                                                    </asp:Table>

                                                    <asp:Table ID="tblPagesBottom" runat="server" CssClass="autoWidth" Style="margin: 5px 0px 0px 0px;">
                                                    </asp:Table>
                                                </td>
                                                <td valign="top" id="middlecell">
                                                    <asp:Table ID="tblMostCommented" runat="server" Style="margin-bottom: 8px; margin-top: 9px;"
                                                        CellPadding="1" CellSpacing="0" Width="100%" CssClass="lightBluePanels">
                                                    </asp:Table>
                                                    <asp:Table ID="tblLast20" runat="server" CellPadding="1"
                                                        CellSpacing="0" Width="100%" CssClass="lightBluePanels">
                                                    </asp:Table>
                                                </td>
                                                <td valign="top" id="adcell" width="1">
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
        </div>




    </asp:Panel>






    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FFCCFF">
        <br />
        <cc1:DecoratedButton ID="btnSearch" runat="server" OnClick="btnSearch_Click"
            Text="Search" />
        &nbsp;<asp:Label ID="lblSendReport" runat="server" Text="Write report"
            CssClass="sendReport"></asp:Label>
        <br />
        <asp:Label ID="lblError" runat="server" CssClass="errors" Text="Error:"></asp:Label><br />
        <asp:HyperLink ID="hlAddProduct" runat="server" Visible="False"
            CssClass="marginsLR8">Add product</asp:HyperLink><br />
        <asp:Label ID="lblActions" runat="server" Text="Actions : "></asp:Label>
        <br />
        <asp:Label ID="lblName" runat="server" CssClass="sectionTextHeader" Text="Name"></asp:Label>
        <br />
        <br />
        &nbsp;&nbsp;<br />
    </asp:Panel>

    <asp:TextBox ID="tbSearch" runat="server" Columns="40"> </asp:TextBox>
    <ajaxToolkit:TextBoxWatermarkExtender ID="tbSearch_TextBoxWatermarkExtender"
        runat="server" TargetControlID="tbSearch"
        WatermarkText="Search in this category">
    </ajaxToolkit:TextBoxWatermarkExtender>


    <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle roundedCorners5"></asp:Panel>

    <asp:Panel ID="pnlSendReport" runat="server" CssClass="pnlPopUpReport roundedCorners5">
        <div class="sectionTextHeader" style="padding: 5px 0px 5px 0px;">
            <asp:Label ID="lblReport" runat="server" Text="Report irregularity" ForeColor="#C02E29"></asp:Label>
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




