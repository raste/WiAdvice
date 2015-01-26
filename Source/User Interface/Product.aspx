<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="UserInterface.ProductPage"
    MasterPageFile="MasterPage.Master" Theme="MainTheme" %>

<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>
<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc2" %>
<%@ Register Assembly="Microsoft.Web.GeneratedImage" Namespace="Microsoft.Web" TagPrefix="cc3" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .style24 {
            width: 100%;
        }

        .style27 {
            width: 100%;
            border-collapse: collapse;
        }
    </style>

    <script type="text/javascript" language="javascript">
        var upEditID = '<%= this.upEditCharacteristic.ClientID %>'; 
    </script>

</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div id="updateProgressDiv" style="display: none;" class="updateProgressDiv">
        <img src="images/SiteImages/loading.gif" />
        Please Wait...
    </div>


    <div style="margin-top: 10px;">
        <div class="brb2">
            <div class="blb2">
                <div class="blhr">
                    <div class="contentBoxBottomHr clearfix2">

                        <div class="clearfix2">

                            <asp:HyperLink ID="imgLink" runat="server">
                                <cc3:GeneratedImage ID="giMainImage" runat="server" CssClass="margin clearfix" ImageAlign="Left"
                                    BorderColor="Black" BorderWidth="1px" BorderStyle="Solid" BackColor="White">
                                </cc3:GeneratedImage>
                            </asp:HyperLink>
                            <div style="text-align: right">
                                <asp:PlaceHolder ID="phAddedAndModified" runat="server"></asp:PlaceHolder>
                            </div>
                            <br />
                            <div style="text-align: center">
                                <asp:HyperLink ID="hlProductName" runat="server" Font-Size="X-Large">Name</asp:HyperLink>
                            </div>
                            <br />
                            <asp:PlaceHolder ID="phCompany" runat="server"></asp:PlaceHolder>
                            <br />
                            <asp:PlaceHolder ID="phSite" runat="server"></asp:PlaceHolder>
                            <br />
                            <asp:PlaceHolder ID="phCategory" runat="server"></asp:PlaceHolder>
                            <br />
                            <asp:Label ID="lblAlternativeNames" runat="server" Text="Alternative names :"></asp:Label>
                            <div style="margin-top: 2px;">
                                <asp:Label ID="lblShare" runat="server" Text="Share :"></asp:Label>
                            </div>
                            <asp:Label ID="lblProdRating" runat="server" Text="Rating" CssClass="searchPageRatings"></asp:Label>
                            <asp:Label ID="lblRatingComa" runat="server" Text=" , "></asp:Label>
                            <asp:Label ID="lblUsersRated" runat="server" Text="Users Rated " CssClass="searchPageRatings"></asp:Label>
                            ,
                <asp:HyperLink ID="hlCommsFastLink" NavigateUrl="" runat="server">
                    <asp:Label ID="lblComments" runat="server" Text="Comments" CssClass="searchPageRatings"></asp:Label></asp:HyperLink>
                            <br />
                            <br />
                            <asp:Panel ID="pnlProdDescription" runat="server">
                                <asp:Label ID="lblPDescription" runat="server" Text="Label"></asp:Label>
                                <br />
                                <br />
                            </asp:Panel>

                            <asp:PlaceHolder ID="phAddedBy" runat="server"></asp:PlaceHolder>
                            <div class="floatRightNoMrg">

                                <asp:Label ID="lblLinks" runat="server" Text="Links" Style="margin-right: 10px;" CssClass="lblEditors"></asp:Label>

                                <asp:HyperLink ID="hlForum" runat="server" Style="margin-right: 10px;">Forum</asp:HyperLink>

                                <asp:Label ID="lblProductEditors" runat="server" Text="Product editors" CssClass="lblEditors"></asp:Label>
                                <ajaxToolkit:PopupControlExtender ID="lblProductEditors_PopupControlExtender" runat="server"
                                    DynamicServicePath="" Enabled="True" ExtenderControlID="" TargetControlID="lblProductEditors"
                                    OffsetX="-270" PopupControlID="pnlPopUpEditors" Position="Bottom">
                                </ajaxToolkit:PopupControlExtender>
                            </div>

                        </div>

                        <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False" CssClass="usrNotificationPnl">
                            <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlNotification" runat="server" Visible="False" HorizontalAlign="Center">
                            <asp:Label ID="lblProdNotif" runat="server" Text="Notification : " ForeColor="#CC3300"
                                Font-Size="Larger"></asp:Label>
                        </asp:Panel>


                    </div>

                    <img src="images/SiteImages/horL.png" align="left" />
                    <img src="images/SiteImages/horR.png" align="right" />

                </div>
            </div>
        </div>
    </div>





    <div class="panelNearSideElements">
        <ajaxToolkit:Accordion ID="accAdmin" runat="server" FramesPerSecond="40" RequireOpenedPane="False"
            SelectedIndex="-1" Visible="False">
            <Panes>
                <ajaxToolkit:AccordionPane ID="AccordionPane3" runat="server">
                    <Header>
                        <asp:Panel ID="pnlShowAdminPnl" runat="server" Visible="True" CssClass="accordionHeaders">
                            <asp:Label ID="lblAdminPanel" runat="server" CssClass="sectionTextHeader" Text="Admin Panel"></asp:Label>
                        </asp:Panel>
                    </Header>
                    <Content>
                        <asp:Panel ID="pnlAdmin" runat="server" Visible="True" CssClass="admBGR" Style="padding-left: 10px; padding-right: 10px;">

                            <br />
                            <asp:Label ID="lblVisible" runat="server" Text="Visible"></asp:Label>
                            <asp:Button ID="btnDeleteProduct" runat="server" OnClick="btnDeleteProduct_Click"
                                Text="Delete" CssClass="marginsLR" />
                            <asp:Button ID="btnMakeVisible" runat="server" OnClick="btnMakeVisible_Click" Text="Make Visible"
                                CssClass="marginsLR" />
                            When product is deleted, all editors loose their rights on it!
                            <br />
                            <br />

                            <asp:Label ID="lblCanUserTakeRoleIfNoEditors" runat="server" Text="Can user take role if no editors"></asp:Label>
                            <asp:Button ID="btnChangeCanUserTakeRole" runat="server" Text="Change" OnClick="btnChangeCanUserTakeRole_Click" />
                            <br />
                            <br />
                            Give user edit roles for this product :
                            <asp:TextBox ID="tbUserRoleProd" runat="server" Columns="8"></asp:TextBox>
                            <ajaxToolkit:FilteredTextBoxExtender ID="tbUserRoleProd_FilteredTextBoxExtender"
                                runat="server" FilterType="Numbers" TargetControlID="tbUserRoleProd">
                            </ajaxToolkit:FilteredTextBoxExtender>
                            <ajaxToolkit:TextBoxWatermarkExtender ID="tbUserRoleProd_TextBoxWatermarkExtender"
                                runat="server" TargetControlID="tbUserRoleProd" WatermarkText="User ID">
                            </ajaxToolkit:TextBoxWatermarkExtender>
                            <asp:Button ID="btnUserRoleProd" runat="server" OnClick="btnUserRoleProd_Click" Text="Submit" />
                            &nbsp;<asp:PlaceHolder ID="phRoles" runat="server" Visible="False"></asp:PlaceHolder>
                            <br />
                            <asp:Label ID="lblProdEditors" runat="server" CssClass="marginsLR" Text="Users who can edin this product :"></asp:Label>
                            <asp:Table ID="tblProdEditors" Width="100%" runat="server" BorderColor="Black" BorderStyle="Solid"
                                BorderWidth="1px">
                            </asp:Table>
                            <br />
                            <asp:Label ID="lblProdCompEditors" runat="server" Text="Users who can edit all company`s products :"
                                CssClass="marginsLR"></asp:Label>
                            <asp:Table ID="tblAllCompProdModificators" runat="server" Width="100%" BorderColor="Black"
                                BorderStyle="Solid" BorderWidth="1px">
                            </asp:Table>

                        </asp:Panel>
                    </Content>
                </ajaxToolkit:AccordionPane>
            </Panes>
        </ajaxToolkit:Accordion>
        <asp:Panel ID="pnlEdit" runat="server" Visible="False">
            <div style="text-align: right">
                <asp:HyperLink ID="hlMoreInfo" runat="server" NavigateUrl="~/Rules.aspx#30">Click here</asp:HyperLink>
                <asp:Label ID="lblMoreInfo" runat="server" Text="for more information on editing products."></asp:Label>
            </div>
            <ajaxToolkit:Accordion ID="accordionAddEdit" runat="server" FramesPerSecond="40"
                RequireOpenedPane="False" SelectedIndex="-1">
                <Panes>
                    <ajaxToolkit:AccordionPane ID="apEdit" runat="server">
                        <Header>
                            <asp:Panel ID="pnlEditProduct" runat="server" CssClass="accordionHeadersEdit">
                                <asp:Label ID="lblAccEdit" runat="server" Text="Edit"></asp:Label>
                            </asp:Panel>
                        </Header>
                        <Content>
                            <ajaxToolkit:Accordion ID="accEdit" runat="server" FramesPerSecond="40" RequireOpenedPane="False"
                                SelectedIndex="-1">
                                <Panes>
                                    <ajaxToolkit:AccordionPane ID="apEditDescription" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlEditDescriptionHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditDescription" runat="server" Text="Edit Description"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditDescriptionContent" runat="server" CssClass="editDescriptionBGR">
                                                <table class="style24">
                                                    <tr>
                                                        <td align="right" style="width: 150px;">
                                                            <asp:Label ID="lblAccEditDescription" runat="server" Text="Change description : "></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbAccEditDescription" runat="server" Columns="70" Rows="10" TextMode="MultiLine"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <div style="padding: 5px 0px 5px 0px;">
                                                                <asp:Label ID="lblSymbolsCount" runat="server" Text="Characters : "></asp:Label>
                                                                <asp:TextBox ID="tbSymbolsCount" runat="server" Columns="3" ReadOnly="True" Width="35px">0</asp:TextBox>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="dbEditDescription" runat="server" OnClick="dbEditDescription_Click"
                                                                Text="Save" />
                                                            <asp:PlaceHolder ID="phEditDescription" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apEditCharacteristics" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlEditCharacteristicsHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditCharacteristics" runat="server" Text="Edit Characteristics"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditCharacteristicsContent" runat="server" CssClass="editCharacteristicBGR">
                                                <asp:UpdatePanel ID="upEditCharacteristic" runat="server">
                                                    <ContentTemplate>
                                                        <table class="style24">
                                                            <tr>
                                                                <td align="right" style="width: 150px;">
                                                                    <asp:Label ID="lblChooseChar" runat="server" Text="Characteristic : "></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddlEditCharacteristics" runat="server" AutoPostBack="true"
                                                                        OnSelectedIndexChanged="ddlEditCharacteristics_SelectedIndexChanged">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
                                                                    <asp:Label ID="lblNewCharName" runat="server" Text="New name : "></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <div style="padding: 5px 0px 5px 0px;">
                                                                        <asp:TextBox ID="tbAccCharName" runat="server" Columns="40"></asp:TextBox>
                                                                        &nbsp;<asp:Label ID="lblAccCharNewName" ForeColor="#C02E29" Font-Bold="True" runat="server"
                                                                            Text="Check Name"></asp:Label>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblCharInfo1" runat="server" Text="information1"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblCharInfo2" runat="server" Text="information 2"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
                                                                    <asp:Label ID="lblCharDescrtiption" runat="server" Text="Description : "></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <div style="padding: 5px 0px 5px 0px;">
                                                                        <asp:TextBox ID="tbAccEditCharDescription" runat="server" Columns="70" Rows="15"
                                                                            TextMode="MultiLine"></asp:TextBox>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <ajaxToolkit:UpdatePanelAnimationExtender ID="upaeEditCharacteistics" runat="server"
                                                    BehaviorID="animation" TargetControlID="upEditCharacteristic">
                                                    <Animations>
                                                                <OnUpdating>
                                                                <Sequence>
                                                                    <Parallel duration="0">
                                                                        <EnableAction AnimationTarget="btnEditCharSave" Enabled="false"></EnableAction>  
                                                                        <EnableAction AnimationTarget="btnDeleteCharacteristic" Enabled="false"></EnableAction>
                                                                        <EnableAction AnimationTarget="ddlEditCharacteristics" Enabled="false"></EnableAction> 
                                                                        <EnableAction AnimationTarget="tbAccCharName" Enabled="false"></EnableAction>
                                                                        <EnableAction AnimationTarget="tbAccEditCharDescription" Enabled="false"></EnableAction> 
                                                                    </Parallel>
                                                                    </Sequence>
                                                                </OnUpdating>
                                                                <OnUpdated>
                                                                <Sequence>
                                                                <%-- Do each of the selected effects --%>
                                                                <Parallel duration="0" >
                                                                <%-- Enable all the controls --%>
                                                                        <EnableAction AnimationTarget="btnEditCharSave" Enabled="true"></EnableAction>  
                                                                        <EnableAction AnimationTarget="btnDeleteCharacteristic" Enabled="true"></EnableAction>
                                                                        <EnableAction AnimationTarget="ddlEditCharacteristics" Enabled="true"></EnableAction>
                                                                        <EnableAction AnimationTarget="tbAccCharName" Enabled="true"></EnableAction>
                                                                        <EnableAction AnimationTarget="tbAccEditCharDescription" Enabled="true"></EnableAction> 
                                                            
                                                                </Parallel>
                                                                </Sequence>
                                                        </OnUpdated>
                                                    </Animations>
                                                </ajaxToolkit:UpdatePanelAnimationExtender>
                                                <table>
                                                    <tr>
                                                        <td align="right" style="width: 150px; vertical-align: text-top"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnEditCharSave" runat="server" OnClick="btnEditCharSave_Click"
                                                                Text="Save" />
                                                            <cc1:DecoratedButton ID="btnDeleteCharacteristic" runat="server" OnClick="btnDeleteCharacteristic_Click"
                                                                Text="Delete" />
                                                            <asp:PlaceHolder ID="phEditChar" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                    <ajaxToolkit:AccordionPane ID="apEditWebsite" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlEditWebSiteHEader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditWebSite" runat="server" Text="Edit website"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditWebSiteContent" runat="server" CssClass="editSiteBGR">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td align="right" style="width: 150px;"></td>
                                                        <td>
                                                            <asp:Label ID="lblEditWebSiteINfo" runat="server" Text="Information"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblAccNewWebSite" runat="server" Text="New website : "></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbAccEditWebsite" runat="server" Columns="60"></asp:TextBox>
                                                            <cc1:DecoratedButton ID="btnUpdateWebSite" runat="server" OnClick="btnUpdateWebSite_Click"
                                                                Text="Update" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <asp:PlaceHolder ID="phNewWebsite" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apEditVariant" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlEditVariantHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditVariant" runat="server" Text="Edit Variant"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditVariantContent" runat="server" CssClass="addEditVariantBGR">
                                                <asp:UpdatePanel ID="upEditVariant" runat="server">
                                                    <ContentTemplate>
                                                        <table class="style24">
                                                            <tr>
                                                                <td align="right" style="width: 150px;">
                                                                    <asp:Label ID="lblEditVariantChoose" runat="server" Text="Variant : "></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddlEditVariant" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEditVariant_SelectedIndexChanged">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>&nbsp;
                                                                </td>
                                                                <td>
                                                                    <div style="padding: 5px 0px 5px 0px;">
                                                                        <asp:Label ID="lblEditVariantInfo1" runat="server" Text="information1"></asp:Label>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
                                                                    <asp:Label ID="lblEditVariantDescription" runat="server" Text="Description : "></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="tbEditVariantDescription" runat="server" Columns="70" Rows="5" TextMode="MultiLine"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <ajaxToolkit:UpdatePanelAnimationExtender ID="upaeEditVariant" runat="server" TargetControlID="upEditVariant">
                                                    <Animations>
                                                                <OnUpdating>
                                                                <Sequence>
                                                                 <%-- place the update progress div over the gridview control --%>
                                                                    <%-- Disable all the controls --%>
                                                                    <Parallel duration="0">
                                                                        <EnableAction AnimationTarget="btnEditVariant" Enabled="false"></EnableAction>  
                                                                        <EnableAction AnimationTarget="btnDeleteVariant" Enabled="false"></EnableAction>
                                                                        <EnableAction AnimationTarget="ddlEditVariant" Enabled="false"></EnableAction> 
                                                                        <EnableAction AnimationTarget="tbEditVariantDescription" Enabled="false"></EnableAction>
                                                                    </Parallel>
                                                                    </Sequence>
                                                                </OnUpdating>
                                                                <OnUpdated>
                                                                <Sequence>
                                                                <%-- Do each of the selected effects --%>
                                                                <Parallel duration="0" >
                                                                <%-- Enable all the controls --%>
                                                                        <EnableAction AnimationTarget="btnEditVariant" Enabled="true"></EnableAction>  
                                                                        <EnableAction AnimationTarget="btnDeleteVariant" Enabled="true"></EnableAction>
                                                                        <EnableAction AnimationTarget="ddlEditVariant" Enabled="true"></EnableAction> 
                                                                        <EnableAction AnimationTarget="tbEditVariantDescription" Enabled="true"></EnableAction>
                                                                </Parallel>
                                                                </Sequence>
                                                        </OnUpdated>
                                                    </Animations>
                                                </ajaxToolkit:UpdatePanelAnimationExtender>
                                                <table>
                                                    <tr>
                                                        <td align="right" style="width: 150px;"></td>
                                                        <td>
                                                            <div style="padding-top: 5px;">
                                                                <cc1:DecoratedButton ID="btnEditVariant" runat="server" OnClick="btnEditVariant_Click"
                                                                    Text="Save" />
                                                                <cc1:DecoratedButton ID="btnDeleteVariant" runat="server" OnClick="btnDeleteVariant_Click"
                                                                    Text="Delete" />
                                                                <asp:PlaceHolder ID="phEditVariant" runat="server" Visible="False"></asp:PlaceHolder>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apEditSubVariant" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlEditSubVariantHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditSubVariant" runat="server" Text="Edit sub variant"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditSubVariantContent" runat="server" CssClass="addEditVariantBGR">
                                                <asp:UpdatePanel ID="upEditSubVariant" runat="server">
                                                    <ContentTemplate>
                                                        <table class="style24">
                                                            <tr>
                                                                <td align="right" style="width: 150px;">
                                                                    <asp:Label ID="lblEditSubVariantName" runat="server" Text="Variant : "></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddlEditSubVariant" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEditSubVariant_SelectedIndexChanged">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>&nbsp;
                                                                </td>
                                                                <td>
                                                                    <div style="padding: 5px 0px 5px 0px;">
                                                                        <asp:Label ID="lblEditSubVariantInfo1" runat="server" Text="information1"></asp:Label>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
                                                                    <asp:Label ID="lblEditSubVariantDescription" runat="server" Text="Description : "></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="tbEditSubVariantDescription" runat="server" Columns="70" Rows="5"
                                                                        TextMode="MultiLine"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <ajaxToolkit:UpdatePanelAnimationExtender ID="upaeEditSubVariant" runat="server"
                                                    TargetControlID="upEditSubVariant">
                                                    <Animations>
                                                                <OnUpdating>
                                                                <Sequence>
                                                                 <%-- place the update progress div over the gridview control --%>
                                                                    <%-- Disable all the controls --%>
                                                                    <Parallel duration="0">
                                                                        <EnableAction AnimationTarget="btnEditSubVariant" Enabled="false"></EnableAction>  
                                                                        <EnableAction AnimationTarget="btnDeleteSubVariant" Enabled="false"></EnableAction>
                                                                        <EnableAction AnimationTarget="tbEditSubVariantDescription" Enabled="false"></EnableAction> 
                                                                        <EnableAction AnimationTarget="ddlEditSubVariant" Enabled="false"></EnableAction>
                                                                    </Parallel>
                                                                    </Sequence>
                                                                </OnUpdating>
                                                                <OnUpdated>
                                                                <Sequence>
                                                                <%-- Do each of the selected effects --%>
                                                                <Parallel duration="0" >
                                                                <%-- Enable all the controls --%>
                                                                        <EnableAction AnimationTarget="btnEditSubVariant" Enabled="true"></EnableAction>  
                                                                        <EnableAction AnimationTarget="btnDeleteSubVariant" Enabled="true"></EnableAction>
                                                                        <EnableAction AnimationTarget="tbEditSubVariantDescription" Enabled="true"></EnableAction> 
                                                                        <EnableAction AnimationTarget="ddlEditSubVariant" Enabled="true"></EnableAction>
                                                                </Parallel>
                                                                </Sequence>
                                                        </OnUpdated>
                                                    </Animations>
                                                </ajaxToolkit:UpdatePanelAnimationExtender>
                                                <table>
                                                    <tr>
                                                        <td align="right" style="width: 150px;"></td>
                                                        <td>
                                                            <div style="padding-top: 5px;">
                                                                <cc1:DecoratedButton ID="btnEditSubVariant" runat="server" OnClick="btnEditSubVariant_Click"
                                                                    Text="Save" />
                                                                <cc1:DecoratedButton ID="btnDeleteSubVariant" runat="server" OnClick="btnDeleteSubVariant_Click"
                                                                    Text="Delete" />
                                                                <asp:PlaceHolder ID="phEditSubVariant" runat="server" Visible="False"></asp:PlaceHolder>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apRemoveAlternativeNames" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlRemoveAlternativeNamesHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblShowRemoveAlternativeNames" runat="server" Text="Remove alternative names"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlRemoveAlternativeNamesContent" runat="server" CssClass="addVariantBGR"
                                                DefaultButton="btnRemoveAlternativeNames">
                                                <table>
                                                    <tr>
                                                        <td align="right" style="width: 150px;">
                                                            <asp:Label ID="lblRemoveAlternativeNames" runat="server" Text="Alternative names :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBoxList ID="cblRemoveAlternativeNames" RepeatDirection="Vertical" runat="server">
                                                            </asp:CheckBoxList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnRemoveAlternativeNames" runat="server" OnClick="btnRemoveAlternativeNames_Click"
                                                                Text="Remove" />
                                                            <asp:PlaceHolder ID="phRemoveAlternativeNamesError" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                    <ajaxToolkit:AccordionPane ID="apEditName" runat="server" Visible="false">
                                        <Header>
                                            <asp:Panel ID="pnlEditNameHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditName" runat="server" Text="Edit Name" CssClass="accordionCrucialDataLabels"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditNameContent" runat="server" CssClass="editSiteBGR">


                                                <asp:Label ID="lblAccProductNameRules" runat="server" Text="Information" Style="margin-left: 155px;"></asp:Label>

                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td align="right" style="width: 150px;">
                                                            <asp:Label ID="lblAccEditName" runat="server" Text="New name : "></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div style="margin: 5px 0px 5px 0px;">
                                                                <asp:TextBox ID="tbAccEditName" runat="server" Columns="40"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvChangeName" runat="server" ErrorMessage="*" ControlToValidate="tbAccEditName" ValidationGroup="changeName"></asp:RequiredFieldValidator>

                                                                <asp:Label ID="lblCheckProductNewName" ForeColor="#C02E29" Font-Bold="True"
                                                                    runat="server" Text="Check Name"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnUpdateProductName" runat="server" OnClick="btnUpdateProductName_Click" ValidationGroup="changeName"
                                                                Text="Update" />
                                                            <asp:PlaceHolder ID="phNewName" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                    <ajaxToolkit:AccordionPane ID="apEditCompany" runat="server">
                                        <Header>

                                            <asp:Panel ID="pnlEditCompanyHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditCompany" runat="server" Text="Edit company" CssClass="accordionCrucialDataLabels"></asp:Label>
                                            </asp:Panel>

                                        </Header>
                                        <Content>

                                            <asp:Panel ID="pnlChangeCompany" runat="server" CssClass="editSiteBGR">

                                                <asp:Panel ID="pnlAdminChangeCompany" runat="server">

                                                    <asp:Panel ID="pnlAdmSubRemCompany" runat="server" Style="margin-bottom: 10px;">

                                                        <asp:Label ID="lblRemoveCompany" runat="server" Text="Remove company : "></asp:Label>
                                                        <asp:Button ID="btnRemCompany" runat="server" Text="Remove" OnClick="btnRemCompany_Click" />
                                                        <asp:Label ID="lblRemCompanyInfo" runat="server" Text="Changes product company to 'Other'"></asp:Label>


                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlAdmSubChangeCompany" runat="server">

                                                        <asp:Label ID="lblChngCompany" runat="server" Text="Change Company (ID)"></asp:Label>
                                                        &nbsp;
            <asp:TextBox ID="tbChngCompany" runat="server" Columns="5"></asp:TextBox>
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="tbChngCompany_FilteredTextBoxExtender" runat="server"
                                                            FilterType="Numbers" TargetControlID="tbChngCompany">
                                                        </ajaxToolkit:FilteredTextBoxExtender>
                                                        <asp:Button ID="btnChngCompany" runat="server" OnClick="btnChngCompany_Click" Text="Change company" />
                                                        &nbsp;<asp:PlaceHolder ID="phChangeCompany" runat="server" Visible="False"></asp:PlaceHolder>
                                                        <br />
                                                        <asp:Label ID="lblNoteChngComp" runat="server" Text="(NOTE : changing only if the new company can have products in this category and it is visible , and the product also.)"></asp:Label>


                                                    </asp:Panel>

                                                </asp:Panel>


                                                <asp:Panel ID="pnlEditorChangeCompany" runat="server">
                                                    <asp:Label ID="lblEditorChnCompInfo" runat="server" Text="Info" Style="margin-left: 160px;"></asp:Label>

                                                    <table>
                                                        <tr>
                                                            <td style="width: 150px; text-align: right;">

                                                                <asp:Label ID="lblEditorNewCompany" runat="server" Text="New company :"></asp:Label>

                                                            </td>
                                                            <td>

                                                                <asp:DropDownList ID="ddlEditorChangeCompany" runat="server" Style="margin: 5px 0px 5px 0px;">
                                                                </asp:DropDownList>

                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td>
                                                                <cc1:DecoratedButton ID="dbEditorUpdateCompany" runat="server" Text="Update" OnClick="dbEditorUpdateCompany_Click" />

                                                                <asp:PlaceHolder ID="phEditorUpdCompError" runat="server"></asp:PlaceHolder>

                                                            </td>
                                                        </tr>
                                                    </table>


                                                </asp:Panel>

                                            </asp:Panel>

                                        </Content>
                                    </ajaxToolkit:AccordionPane>


                                    <ajaxToolkit:AccordionPane ID="apEditCategory" runat="server">
                                        <Header>

                                            <asp:Panel ID="pnlEditCategoryHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditCategoryHeader" runat="server" Text="Edit category" CssClass="accordionCrucialDataLabels"></asp:Label>
                                            </asp:Panel>

                                        </Header>
                                        <Content>

                                            <asp:Panel ID="pnlEditCategory" runat="server" CssClass="editSiteBGR">

                                                <asp:Panel ID="pnlSubAdmEditCategory" runat="server">
                                                    Change Category (ID)&nbsp;
            <asp:TextBox ID="tbChngCategory" runat="server" Columns="5"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="tbChngCategory_FilteredTextBoxExtender"
                                                        runat="server" FilterType="Numbers" TargetControlID="tbChngCategory">
                                                    </ajaxToolkit:FilteredTextBoxExtender>
                                                    <asp:Button ID="btnChngCategory" runat="server" OnClick="btnChngCategory_Click" Text="Change category" />
                                                    &nbsp;<asp:PlaceHolder ID="phChangeCategory" runat="server" Visible="False"></asp:PlaceHolder>
                                                    <br />
                                                    <asp:Label ID="lblChngCat" runat="server" Text="(NOTE : if current Product company cannot have products in the new category this Product will change company to Other.)"></asp:Label>
                                                </asp:Panel>

                                                <asp:Panel ID="pnlSubEditorEditCategoryWhenCompOther" runat="server">

                                                    <asp:UpdatePanel ID="upChangeCategory" runat="server" UpdateMode="Always">
                                                        <ContentTemplate>
                                                            <table style="width: 100%; margin: 5px 0px 5px 0px;">
                                                                <tr>
                                                                    <td style="width: 150px; text-align: right;">

                                                                        <asp:Label ID="lblEditorChooseCat" runat="server" Text="Choose category :"></asp:Label>

                                                                    </td>
                                                                    <td style="width: 1px;">


                                                                        <asp:Menu ID="menuChooseNewCategory" runat="server" CssClass="autoWidth"
                                                                            DynamicPopOutImageUrl="~/images/SiteImages/catMenuImgS.png"
                                                                            Font-Names="arial,georgia,&quot;Trebuchet MS&quot;,&quot;Times New Roman&quot;"
                                                                            DisappearAfter="1000"
                                                                            MaximumDynamicDisplayLevels="50"
                                                                            OnMenuItemClick="menuChooseNewCategory_MenuItemClick"
                                                                            StaticPopOutImageUrl="~/images/SiteImages/navMenusImg.png"
                                                                            StaticSubMenuIndent="" DynamicHorizontalOffset="1" Orientation="Horizontal"
                                                                            SkipLinkText="">
                                                                            <StaticMenuStyle BorderStyle="None" CssClass="autoWidth" />
                                                                            <LevelSubMenuStyles>
                                                                                <asp:SubMenuStyle />
                                                                                <asp:SubMenuStyle CssClass="menuBgr0" BackColor="#a1d495" />
                                                                                <asp:SubMenuStyle CssClass="menuBgr2" BackColor="#499650" />
                                                                                <asp:SubMenuStyle CssClass="menuBgr1" BackColor="#a1d495" />
                                                                                <asp:SubMenuStyle CssClass="menuBgr2" BackColor="#499650" />
                                                                                <asp:SubMenuStyle CssClass="menuBgr1" BackColor="#a1d495" />
                                                                                <asp:SubMenuStyle CssClass="menuBgr2" BackColor="#499650" />
                                                                                <asp:SubMenuStyle CssClass="menuBgr1" BackColor="#a1d495" />
                                                                                <asp:SubMenuStyle CssClass="menuBgr2" BackColor="#499650" />
                                                                                <asp:SubMenuStyle CssClass="menuBgr1" BackColor="#a1d495" />
                                                                                <asp:SubMenuStyle CssClass="menuBgr2" BackColor="#499650" />
                                                                            </LevelSubMenuStyles>
                                                                            <LevelMenuItemStyles>
                                                                                <asp:MenuItemStyle Font-Underline="False" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                                <asp:MenuItemStyle Font-Underline="False" Width="100%" />
                                                                            </LevelMenuItemStyles>
                                                                            <StaticMenuItemStyle CssClass="none" Font-Bold="True"
                                                                                Font-Names="&quot;Trebuchet MS&quot;,Verdana,&quot;Times New Roman&quot;"
                                                                                Font-Overline="False" Font-Size="15px" ForeColor="#254b28" />
                                                                            <DynamicHoverStyle Font-Italic="False"
                                                                                Font-Underline="False" CssClass="noEffect" ForeColor="#254b28" />
                                                                            <DynamicMenuItemStyle
                                                                                CssClass="categoryLinks" HorizontalPadding="3px" />
                                                                            <StaticHoverStyle CssClass="noEffect" />
                                                                        </asp:Menu>





                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="tbNewCategory" runat="server" Columns="70" Enabled="False"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="rfvAddCategory" runat="server" ControlToValidate="tbNewCategory"
                                                                            ErrorMessage="*" ValidationGroup="chooseCat"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                    <ajaxToolkit:UpdatePanelAnimationExtender ID="upaeChangeCategory" runat="server" TargetControlID="upChangeCategory" Enabled="true">
                                                        <Animations>
                        <OnUpdating>
                        <Sequence>
                            <%-- Disable all the controls --%>
                            <Parallel duration="0">
                                <EnableAction AnimationTarget="btnEditorChngCatWhenCompOther" Enabled="false"></EnableAction>                         
                            </Parallel>
                            </Sequence>
                        </OnUpdating>
                        <OnUpdated>
                        <Sequence>
                        <%-- Do each of the selected effects --%>
                        <Parallel duration="0" >
                        <%-- Enable all the controls --%>
                               <EnableAction AnimationTarget="btnEditorChngCatWhenCompOther" Enabled="true"></EnableAction>
                        </Parallel>
                        </Sequence>
                </OnUpdated>
                                                        </Animations>
                                                    </ajaxToolkit:UpdatePanelAnimationExtender>
                                                    <div style="padding-left: 160px;">
                                                        <cc1:DecoratedButton ID="btnEditorChngCatWhenCompOther" runat="server" Text="Submit"
                                                            ValidationGroup="chooseCat" OnClick="btnEditorChngCatWhenCompOther_Click" />
                                                        <asp:PlaceHolder ID="phEditorChngCatWhenCompOther" runat="server" Visible="False"></asp:PlaceHolder>
                                                    </div>
                                                </asp:Panel>

                                                <asp:Panel ID="pnlSubEditorEditCategory" runat="server">

                                                    <asp:Label ID="lblSubEditorChngCatInfo" runat="server" Style="margin-left: 160px;" Text="Only categories in which the company can have products are shown"></asp:Label>

                                                    <table class="style24">
                                                        <tr>
                                                            <td style="width: 150px; text-align: right;">
                                                                <asp:Label ID="lblEditorChooseCat2" runat="server" Text="Choose category :"></asp:Label>
                                                            </td>
                                                            <td>

                                                                <asp:DropDownList ID="ddlChangeCategory" runat="server" Style="margin: 5px 0px 5px 0px;">
                                                                </asp:DropDownList>

                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>&nbsp;</td>
                                                            <td>

                                                                <cc1:DecoratedButton ID="btnEditorChngCat" runat="server" Text="Submit" OnClick="btnEditorChngCat_Click" />
                                                                <asp:PlaceHolder ID="phEditorChngCat" runat="server" Visible="False"></asp:PlaceHolder>
                                                            </td>
                                                        </tr>
                                                    </table>


                                                </asp:Panel>

                                            </asp:Panel>


                                        </Content>
                                    </ajaxToolkit:AccordionPane>


                                </Panes>
                            </ajaxToolkit:Accordion>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                    <ajaxToolkit:AccordionPane ID="apAdd" runat="server">
                        <Header>
                            <asp:Panel ID="pnlAddHeader" runat="server" CssClass="accordionHeadersEdit">
                                <asp:Label ID="lblAddToProduct" runat="server" Text="Add"></asp:Label>
                            </asp:Panel>
                        </Header>
                        <Content>
                            <ajaxToolkit:Accordion ID="accAdd" runat="server" FramesPerSecond="40" RequireOpenedPane="False"
                                SelectedIndex="-1">
                                <Panes>
                                    <ajaxToolkit:AccordionPane ID="apUploadImage" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlUploadImageHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblUploadImage" runat="server" Text="Upload image"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlAddImage" runat="server" CssClass="uploadImgBGR">
                                                <table width="100%">
                                                    <tr>
                                                        <td style="width: 150px; text-align: right; vertical-align: top;">
                                                            <asp:Label ID="lblChooseImg" runat="server" Text="Image :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:FileUpload ID="fuImage" runat="server" />
                                                            <div style="padding: 5px 0px 5px 0px;">
                                                                <asp:Label ID="lblUpImgINfo2" runat="server" Text="JPG,BMP,PNG`s are allowed."></asp:Label>
                                                                <br />
                                                                <asp:Label ID="lblUpImgINfo" runat="server" Text="Max allowed image size is 2mb."></asp:Label>
                                                                <br />
                                                                <asp:HyperLink ID="hlClickForMinImage" Target="_blank" class="galleryLink" runat="server">Click here</asp:HyperLink>
                                                                <asp:Label ID="lblClickForMinImage2" runat="server" Text="to see minimum image size."></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblAddImageDescription" runat="server" Text="Description :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbImageDescr" runat="server" Columns="50" Rows="3" Style="margin-bottom: 5px;"
                                                                TextMode="MultiLine"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:CheckBox ID="cbImageMain" runat="server" Text="main    " Visible="False" />
                                                        </td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnUpload" runat="server" OnClick="btnUpload_Click" Text="Upload" />
                                                            <span id="uploadingSpan" runat="server" style="display: none">
                                                                <img alt="" src="images/SiteImages/uploading.gif" style="width: 32px; height: 16px" />
                                                                <asp:Label ID="lblUploadingImg" runat="server" Text="Uploading ..."></asp:Label>
                                                            </span>
                                                            <asp:PlaceHolder ID="phImage" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apAddCharacteristic" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAddCharacteristicHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblAddCharacteristic" runat="server" Text="Add characteristic"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlSubAddChar" runat="server" CssClass="addCharBGR" DefaultButton="btnAddChar">
                                                <table>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblAddCharInfo1" runat="server" Text="Information"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="width: 150px">
                                                            <asp:Label ID="lblAddCharName" runat="server" Text="Name :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div style="padding: 5px 0px 5px 0px;">
                                                                <asp:TextBox ID="tbCharName" runat="server" Columns="40"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvAddChar" runat="server" ControlToValidate="tbCharName"
                                                                    ErrorMessage="*" ValidationGroup="22"></asp:RequiredFieldValidator>
                                                                <asp:Label ID="lblCheckCharName" ForeColor="#C02E29" Font-Bold="True" runat="server"
                                                                    Text="Check Name"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblAddCharInfo2" runat="server" Text="Information2"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblAddCharDescription" runat="server" Text="Description :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div style="padding: 5px 0px 5px 0px;">
                                                                <asp:TextBox ID="tbCharDescription" runat="server" Columns="70" Rows="15" TextMode="MultiLine"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvCharDescr" runat="server" ControlToValidate="tbCharDescription"
                                                                    ValidationGroup="22"></asp:RequiredFieldValidator>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnAddChar" runat="server" OnClick="btnAddChar_Click" Text="Submit"
                                                                ValidationGroup="22" />
                                                            <asp:PlaceHolder ID="phAddChar" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apAddVariant" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAddVariantHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblAddVariant" runat="server" Text="Add variant"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlAddVariantContent" runat="server" CssClass="addVariantBGR" DefaultButton="btnAddVariant">
                                                <table>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblAddVariantInfo1" runat="server" Text="Information"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="width: 150px">
                                                            <asp:Label ID="lblAddVariantName" runat="server" Text="Name :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div style="padding: 5px 0px 5px 0px;">
                                                                <asp:TextBox ID="tbAddVariantName" runat="server" Columns="40"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvAddVariant" runat="server" ControlToValidate="tbAddVariantName"
                                                                    ErrorMessage="*" ValidationGroup="addVariant"></asp:RequiredFieldValidator>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblAddVariantInfo2" runat="server" Text="Information2"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblAddVariantDescription" runat="server" Text="Description :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div style="padding: 5px 0px 5px 0px;">
                                                                <asp:TextBox ID="tbAddVariantDescription" runat="server" Columns="70" Rows="5" TextMode="MultiLine"></asp:TextBox>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnAddVariant" runat="server" OnClick="btnAddVariant_Click"
                                                                Text="Submit" ValidationGroup="addVariant" />
                                                            <asp:PlaceHolder ID="phAddVariant" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apAddSubVariant" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAddSubVariantHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblAddSubVariant" runat="server" Text="Add sub-variant"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlAddSubVariantContent" runat="server" CssClass="addVariantBGR" DefaultButton="btnAddSubVariant">
                                                <table>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblAddSubVariantVariant" runat="server" Text="Variant : "></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlAddVariant" runat="server">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <div style="padding: 5px 0px 5px 0px;">
                                                                <asp:Label ID="lblAddSubVariantInfo1" runat="server" Text="Information"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="width: 150px">
                                                            <asp:Label ID="lblAddSubVariantName" runat="server" Text="Name :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbAddSubVariantName" runat="server" Columns="40"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvAddSbVariant" runat="server" ControlToValidate="tbAddSubVariantName"
                                                                ErrorMessage="*" ValidationGroup="addSubVariant"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <div style="padding: 5px 0px 5px 0px;">
                                                                <asp:Label ID="lblAddSubVariantInfo2" runat="server" Text="Information2"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblAddSubVariantDescription" runat="server" Text="Description :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbAddSubVariantDescription" runat="server" Columns="70" Rows="5"
                                                                Style="margin-bottom: 5px;" TextMode="MultiLine"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnAddSubVariant" runat="server" OnClick="btnAddSubVariant_Click"
                                                                Text="Submit" ValidationGroup="addSubVariant" />
                                                            <asp:PlaceHolder ID="phAddSubVariant" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apAddAlternativeName" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAddAlternativeNameHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblAddAlternativeNameShow" runat="server" Text="Add alternative name"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlAddAlternativeNameContent" runat="server" CssClass="addVariantBGR"
                                                DefaultButton="btnAddAlternativeName">
                                                <table>
                                                    <tr>
                                                        <td align="right" style="width: 150px">
                                                            <asp:Label ID="lblAddAlternativeNames" runat="server" Text="Alternative names :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbAlternativeNames" runat="server" Columns="40"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvAddAlternativeName" runat="server" ErrorMessage="*"
                                                                ControlToValidate="tbAlternativeNames" ValidationGroup="addAlternativeName"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <div style="padding: 5px 0px 5px 0px;">
                                                                <asp:Label ID="lblAddAlternativeNamesInfo" runat="server" Text="Alternative names info"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnAddAlternativeName" runat="server" OnClick="btnAddAlternativeName_Click"
                                                                Text="Submit" ValidationGroup="addAlternativeName" />
                                                            <asp:PlaceHolder ID="phAddAlternativeNameError" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                </Panes>
                            </ajaxToolkit:Accordion>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                </Panes>
            </ajaxToolkit:Accordion>
            <div class="divBottomHr" runat="server" id="hrBetweenAddEditAndInfo">
                <img src="images/SiteImages/horL.png" align="left" />
                <img src="images/SiteImages/horR.png" align="right" />
            </div>
        </asp:Panel>
        <ajaxToolkit:Accordion ID="accInformation" runat="server" FadeTransitions="False"
            FramesPerSecond="40" RequireOpenedPane="False" SelectedIndex="-1" SuppressHeaderPostbacks="True">
            <Panes>
                <ajaxToolkit:AccordionPane ID="apGallery" runat="server">
                    <Header>
                        <asp:Panel ID="pnlShowGallery" runat="server" Visible="True" CssClass="accordionHeaders">
                            <asp:Label ID="lblGallery" runat="server" Text="Gallery"></asp:Label>
                        </asp:Panel>
                    </Header>
                    <Content>
                        <asp:Table ID="tblGallery" runat="server" CssClass="margins GalleryTable">
                        </asp:Table>
                    </Content>
                </ajaxToolkit:AccordionPane>
                <ajaxToolkit:AccordionPane ID="apCharacteristics" runat="server">
                    <Header>
                        <asp:Panel ID="pnlShowChars" runat="server" Visible="True" CssClass="accordionHeaders">
                            <asp:Label ID="lblCharacteristics" runat="server" Text="Characteristics"></asp:Label>
                        </asp:Panel>
                    </Header>
                    <Content>
                        <asp:Table ID="tblChars" runat="server" CssClass="margins CharacteristicsTable">
                        </asp:Table>
                    </Content>
                </ajaxToolkit:AccordionPane>
                <ajaxToolkit:AccordionPane ID="apVariants" runat="server">
                    <Header>
                        <asp:Panel ID="pnlShowVariants" runat="server" Visible="True" CssClass="accordionHeaders">
                            <asp:Label ID="lblVariants" runat="server" Text="Variants"></asp:Label>
                        </asp:Panel>
                    </Header>
                    <Content>
                        <asp:PlaceHolder ID="phVariants" runat="server"></asp:PlaceHolder>
                    </Content>
                </ajaxToolkit:AccordionPane>
            </Panes>
        </ajaxToolkit:Accordion>
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




                                    <asp:Panel ID="pnlActions" runat="server" Visible="false">
                                        <asp:Label ID="lblRateProduct" runat="server" CssClass="marginsLR" Text="Rate Product :"
                                            Visible="False"></asp:Label>
                                        <asp:DropDownList ID="ddlRateProduct" runat="server" Visible="False">
                                        </asp:DropDownList>
                                        <asp:HyperLink ID="hlAddProduct" runat="server" CssClass="marginsLR8" Visible="False">Add product</asp:HyperLink>
                                        <asp:Label ID="lblSendSuggestion" runat="server" CssClass="lblEditors marginsLR8"
                                            Text="Send suggestion"></asp:Label>
                                        <asp:Label ID="lblSendReport" runat="server" CssClass="sendReport" Text="Write report"
                                            Visible="False"></asp:Label>
                                        <input id="btnSignForNotifies" runat="server" visible="false" class="htmlButtonStyle"
                                            style="margin-left: 8px; margin-right: 8px;" type="button" value="Notify" /><asp:Button
                                                ID="btnTakeAction" runat="server" ForeColor="Red" OnClick="btnTakeAction_Click"
                                                Text="Take Role" Visible="False" CssClass="marginsLR8" Style="float: right;" />
                                        <hr style="margin-bottom: 15px;" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlWriteComment" runat="server" CssClass="addComment" DefaultButton="btnAddComment">
                                        <table class="style24" cellpadding="0" cellspacing="0" style="border-collapse: collapse; empty-cells: hide;">
                                            <tr>
                                                <td>
                                                    <div style="margin: 0px 0px 10px 0px; padding: 0px;">
                                                        <div class="panelInline" style="width: 224px; text-align: left; margin: 0px; padding: 0px 10px 0px 14px;">
                                                            &nbsp;
                                        
                                                        </div>
                                                        <asp:Label ID="lblCommentAction" runat="server" Text="Write comment " CssClass="sectionTextHeader"
                                                            Font-Size="X-Large" ForeColor="#101915" Style="margin-left: 150px;"></asp:Label>
                                                    </div>
                                                    <asp:Panel ID="pnlAddCommentUsername" runat="server" Style="margin: 0px; padding: 5px 0px 5px 0px;">
                                                        <div class="panelInline" style="width: 197px; text-align: right; margin: 0px; padding: 0px;">
                                                            <asp:Label ID="lblCName" runat="server" Text="Name : "></asp:Label>
                                                        </div>
                                                        <asp:TextBox ID="tbName" runat="server" ForeColor="#0033CC" Style="margin-left: 2px;"></asp:TextBox>

                                                        <asp:Label ID="lblCCommenter" runat="server" Text="Check Name" CssClass="errors"></asp:Label>
                                                    </asp:Panel>
                                                    <asp:UpdatePanel ID="upCommentFor" runat="server">
                                                        <ContentTemplate>
                                                            <asp:Panel ID="pnlAddCommentForChar" runat="server" Style="margin: 0px; padding: 5px 0px 5px 0px;">
                                                                <div class="panelInline" style="width: 197px; text-align: right; margin: 0px; padding: 0px;">
                                                                    <asp:Label ID="lblCC" runat="server" Text="About Characteristic :"></asp:Label>
                                                                </div>
                                                                <asp:DropDownList ID="ddlChars" runat="server" AutoPostBack="True" Style="margin-left: 2px;"
                                                                    OnSelectedIndexChanged="ddlChars_SelectedIndexChanged">
                                                                </asp:DropDownList>
                                                            </asp:Panel>
                                                            <asp:Panel ID="pnlAddCommentForVariant" runat="server" Style="padding: 5px 0px 5px 0px;">
                                                                <div class="panelInline" style="width: 197px; text-align: right">
                                                                    <asp:Label ID="lblOr" runat="server" Style="margin-right: 20px;" Text="... or ..."></asp:Label>
                                                                    <asp:Label ID="lblAboutVariant" runat="server" Text="About Variant :"></asp:Label>
                                                                </div>
                                                                <asp:DropDownList ID="ddlAboutVariant" runat="server" AutoPostBack="True" Style="margin-left: 2px;"
                                                                    OnSelectedIndexChanged="ddlAboutVariant_SelectedIndexChanged">
                                                                </asp:DropDownList>
                                                                <asp:DropDownList ID="ddlAboutSubVariant" Style="margin-left: 5px;" runat="server">
                                                                </asp:DropDownList>
                                                            </asp:Panel>

                                                            <div class="clearfix2" style="width: 100%; margin-bottom: 10px; margin-top: 5px;">

                                                                <div style="float: left; width: 190px; padding-left: 14px;">


                                                                    <cc2:CaptchaControl ID="CaptchaControl" runat="server" Width="180px" CaptchaHeight="60"
                                                                        CaptchaLength="3" CaptchaMaxTimeout="3600" BackColor="#FFFFE1" />

                                                                    <div style="margin-top: 5px;">
                                                                        <asp:TextBox ID="tbCaptchaMsg" runat="server" Columns="20" Width="176px"></asp:TextBox>
                                                                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbCaptchaMsg_TextBoxWatermarkExtender"
                                                                            runat="server" TargetControlID="tbCaptchaMsg" WatermarkText="Type image letters here">
                                                                        </ajaxToolkit:TextBoxWatermarkExtender>
                                                                    </div>
                                                                    <div style="text-align: right;">
                                                                    </div>


                                                                </div>

                                                                <div style="vertical-align: bottom; margin-left: 201px;">

                                                                    <asp:TextBox ID="tbDescription" runat="server" Width="450px" Rows="5" Style="height: 170px;"
                                                                        TextMode="MultiLine" ValidationGroup="5" Font-Size="Small"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="rfvComment" runat="server" ControlToValidate="tbDescription"
                                                                        ValidationGroup="5"></asp:RequiredFieldValidator>
                                                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/SiteImages/WiAdvice_f.png"
                                                                        Height="165px" Style="margin-left: 10px;" />

                                                                </div>

                                                            </div>






                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                    <ajaxToolkit:UpdatePanelAnimationExtender ID="upaeCommentAbout" runat="server" TargetControlID="upCommentFor">
                                                        <Animations>
                                            <OnUpdating>
                                            <Sequence>
                                             <%-- place the update progress div over the gridview control --%>
                                        
                                                <%-- Disable all the controls --%>
                                                <Parallel duration="0">
                                                    <EnableAction AnimationTarget="btnAddComment" Enabled="false"></EnableAction>  
                                                </Parallel>
                                                </Sequence>
                                            </OnUpdating>
                                            <OnUpdated>
                                            <Sequence>
                                            <%-- Do each of the selected effects --%>
                                            <Parallel duration="0" >
                                            <%-- Enable all the controls --%>
                                                    <EnableAction AnimationTarget="btnAddComment" Enabled="true"></EnableAction>  
                                             
                                            </Parallel>
                                            </Sequence>
                                    </OnUpdated>
                                                        </Animations>
                                                    </ajaxToolkit:UpdatePanelAnimationExtender>

                                                    <div style="padding-left: 206px;">



                                                        <cc1:DecoratedButton ID="btnAddComment" runat="server" OnClick="btnAddComment_Click"
                                                            Text="Submit" ValidationGroup="5" />



                                                        <cc1:TransliterateButton ID="transliterateBtnComment" runat="server" />
                                                        <cc1:DecoratedButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"
                                                            Visible="False" />&nbsp;
                                    
                                    <br />
                                                        <asp:PlaceHolder ID="phComment" runat="server" Visible="False"></asp:PlaceHolder>
                                                    </div>
                                                </td>
                                                <td style="vertical-align: bottom;"></td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlError" runat="server" Font-Size="Large" ForeColor="#CC3300" HorizontalAlign="Center"
                                        Visible="False">
                                        <br />
                                        <asp:Label ID="lblCommError" runat="server" Text="Comment Action Error"></asp:Label>
                                        <br />
                                        <br />
                                    </asp:Panel>
                                    <a name="opinions"></a>
                                    <asp:Panel ID="pnlSortComments" runat="server" Style="margin: 5px 0px 5px 0px;">
                                        <asp:HyperLink ID="hlAllComments" runat="server" CssClass="marginsLR" Visible="False">All comments</asp:HyperLink>
                                        <asp:Label ID="lblSortBy" runat="server" Text="Sort by : " CssClass="marginsLR"></asp:Label>
                                        <asp:HyperLink ID="DateLink" runat="server" CssClass="marginsLR">Date</asp:HyperLink>
                                        <asp:HyperLink ID="hlSortRating" runat="server" CssClass="marginsLR">Rating</asp:HyperLink>
                                        <asp:DropDownList ID="ddlSortByCHar" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSortByCHar_SelectedIndexChanged"
                                            CssClass="marginsLR">
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="ddlSortByVariant" runat="server" AutoPostBack="True" CssClass="marginsLR"
                                            OnSelectedIndexChanged="ddlSortByVariant_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <asp:HyperLink ID="hlSortByNoAbout" runat="server" Visible="False">No about</asp:HyperLink>
                                    </asp:Panel>
                                    <asp:Table ID="tblPages" runat="server" CssClass="autoWidth" Style="margin: 5px 0px 5px 0px;">
                                    </asp:Table>
                                    <table id="Table1" cellpadding="0" class="style27" runat="server">
                                        <tr>
                                            <td valign="top">
                                                <asp:PlaceHolder ID="phComments" runat="server"></asp:PlaceHolder>
                                                <asp:Table ID="tblPagesBottom" runat="server" CssClass="autoWidth" Style="margin-top: 5px;">
                                                </asp:Table>
                                            </td>
                                            <td valign="top" width="252px" runat="server" id="adCell">
                                                <asp:PlaceHolder ID="phAdvertisement" runat="server" Visible="False"></asp:PlaceHolder>
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
        <asp:Label ID="lblAddedBy" runat="server" Text="Added by : " CssClass="searchPageComments"></asp:Label>
        <asp:Label ID="lblAddedOn" runat="server" CssClass="searchPageComments" Text="Added on : "></asp:Label>
        <br />
        <asp:Label ID="lblError" runat="server" Text="ERROR :" CssClass="errors"></asp:Label>
        &nbsp;<asp:Label ID="lblLastModifiedBy" runat="server" CssClass="searchPageComments"
            Text="Last modified by"></asp:Label>
        &nbsp;<asp:Label ID="lblLastModified" runat="server" CssClass="searchPageComments"
            Text="Last Modified : "></asp:Label>
        <br />
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblReply" runat="server" Text="Reply to : "
            Visible="False"></asp:Label>
        <asp:Label ID="lblReplyTo" runat="server" Text="ReplyTo" Visible="False"></asp:Label>
        <asp:Label ID="lblActions" runat="server" Style="margin-left: 5px;"
            Text="Actions : "></asp:Label>
        <br />
        &nbsp;&nbsp;&nbsp;<br />
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlRateComm" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle ">
    </asp:Panel>
    <asp:Panel ID="pnlActionComm" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5">
    </asp:Panel>
    <asp:Panel ID="pnlMsgToUser" CssClass="pnlPopUpSendMessage roundedCorners5" runat="server"
        Width="400px">
        <asp:Label ID="lblMessageTo" runat="server" Text="Message to :"></asp:Label>
        <span id="spanMsgTo" class="searchPageRatings"></span>
        <br />
        <asp:Label ID="lblMsgSubject" runat="server" Text="Subject : "></asp:Label>
        <input id="tbMsgSubject" class="standardTextBoxes" type="text" style="margin-top: 5px; width: 250px;" />
        <textarea id="tbMsgToUser" class="standardTextBoxes" rows="8" style="margin-top: 5px; margin-bottom: 5px; width: 386px;"></textarea>
        <br />
        <input id="cbSaveMsgInSent" type="checkbox" />
        <asp:Label ID="lblCbSaveInSent" runat="server" Text="Save in sent ?"></asp:Label>&nbsp;&nbsp;&nbsp;
        <input id="btnSendMsgToUser" runat="server" type="button" class="htmlButtonStyle"
            value="Send" onclick="SendMsgToUser()" />
        <cc1:TransliterateButton ID="btnTransMsgToUser" runat="server" SkinID="ordinaryBtn" />
        <input id="btnCancelSendMsg" runat="server" type="button" class="htmlButtonStyle"
            value="Cancel" onclick="HideActionData()" />
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlReplyToComm" CssClass="pnlPopUpSendMessage roundedCorners5" runat="server"
        Width="400px">
        &nbsp;<asp:Label ID="lblReplyToUser" runat="server" Text="Reply to :"></asp:Label>&nbsp;
        <span id="spanReplyToComm" class="searchPageRatings">username</span>
        <br />
        &nbsp;<textarea id="tbReplyToUser" class="standardTextBoxes" rows="8" style="margin-top: 5px; margin-bottom: 5px; width: 386px;"></textarea>
        <br />
        &nbsp;
        
     
        
        <input id="btnReplyToComment" runat="server" type="button" class="htmlButtonStyle"
            value="Send" onclick="ReplyToComment()" />
        <cc1:TransliterateButton ID="btnTransReplyToComment" runat="server" SkinID="ordinaryBtn" />
        <input id="btnCancelReply" runat="server" type="button" class="htmlButtonStyle" value="Cancel"
            onclick="HideActionData()" />
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlEditComment" CssClass="pnlPopUpSendMessage roundedCorners5" runat="server"
        Width="350px">
        Edit comment from : <span id="spanEditComm" class="searchPageRatings">username</span>
        <br />
        &nbsp;<textarea id="tbEditComment" class="standardTextBoxes" rows="5" style="margin-top: 5px; margin-bottom: 5px; width: 336px;"></textarea>
        <br />
        &nbsp;<input id="btnEditComment" type="button" class="htmlButtonStyle" value="Edit"
            onclick="EditComment()" />
        <input id="btnCancelEdit" type="button" class="htmlButtonStyle" value="Cancel" onclick="HideActionData()" />
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle roundedCorners5">
    </asp:Panel>
    <asp:Panel ID="pnlSendReport" runat="server" CssClass="pnlPopUpReport roundedCorners5">
        <div class="sectionTextHeader" style="padding: 5px 0px 5px 0px;">
            <asp:Label ID="lblReportIrregularity" runat="server" Text="Report irregularity" ForeColor="#C02E29"></asp:Label>
        </div>
        <table style="width: 100%;">
            <tr>
                <td style="width: 10; padding-right: 15px;" valign="top">
                    <textarea id="taReportText" class="standardTextBoxes" style="width: 350px; height: 200px;"
                        cols="20" rows="5" name="S1"></textarea>
                </td>
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
                                <input id="btnSendReport" runat="server" type="button" value="Report" onclick="SendReport();"
                                    class="defaultDecButton" />
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
                                <input id="btnHideRepData" runat="server" type="button" value="Cancel" onclick="HideReportData();"
                                    class="defaultDecButton" />
                            </td>
                            <td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td>&nbsp;
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlNotify" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5">
    </asp:Panel>
    <asp:Panel ID="pnlPopUpEditors" CssClass="pnlPopUpEditors roundedCorners5" runat="server">
        <asp:PlaceHolder ID="phPublicProductEditors" runat="server"></asp:PlaceHolder>
    </asp:Panel>

    <asp:Panel ID="pnlPopUpLinks" CssClass="pnlPopUpLinks roundedCorners5" Width="400px" runat="server">
        <div class="clearfix" style="margin-bottom: 3px; text-align: center;">

            <div class="panelInline" style="padding-top: 3px; padding-left: 10px;">
                <asp:Label ID="lblAddProdLink" CssClass="lblEditors" runat="server" Text="Add"></asp:Label>



            </div>
            <div runat="server" id="divClosePopUpLinks" class="closeButtonDiv">X</div>
        </div>
        <asp:PlaceHolder ID="phLinks" runat="server"></asp:PlaceHolder>
    </asp:Panel>

    <asp:Panel ID="pnlSendTypeSuggestion" CssClass="pnlPopUpSendMessage roundedCorners5"
        runat="server" Width="350px">
        <div class="sectionTextHeader">
            <asp:Label ID="lblSuggestionInfo" runat="server" Text="Send suggestion"></asp:Label>
        </div>
        <asp:Label ID="lblSuggestionTo" runat="server" Text="To : "></asp:Label>
        <asp:DropDownList ID="ddlSuggestionUsers" runat="server">
        </asp:DropDownList>
        <asp:HyperLink ID="hlSuggestionUser" runat="server">user link</asp:HyperLink>
        <br />
        <textarea id="tbTypeSuggestion" class="standardTextBoxes" rows="8" style="margin-top: 5px; margin-bottom: 5px; width: 336px;"></textarea>
        <br />
        <input id="btnSendTypeSuggestion" runat="server" type="button" class="htmlButtonStyle"
            value="Send" onclick="SendTypeSuggestionToUser()" />
        <input id="btnCancelSuggestion" runat="server" type="button" class="htmlButtonStyle"
            value="Cancel" onclick="hideSendTypeSuggestionData()" />
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlSendTypeSuggestionEnd" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5">
    </asp:Panel>

    <div id="divAddProdLink" class="pnlPopUpSendMessage roundedCorners5" style="width: 350px;">

        <div class="sectionTextHeader" style="margin-bottom: 5px;">
            <asp:Label ID="lblAddProductLink" runat="server" Text="Add link"></asp:Label>
        </div>

        <asp:Label ID="lblAddLinkRules" runat="server" Text="Please follow"></asp:Label>
        <asp:HyperLink ID="hlAddLinkRules" Style="color: #C02E29;" Target="_blank" runat="server">the rules</asp:HyperLink>
        <asp:Label ID="lblAddLinkRules2" runat="server" Text="when you add links."></asp:Label>

        <div style="margin: 5px 0px 5px 0px;">
            <asp:Label ID="lblProdLink" runat="server" Text="Link : "></asp:Label>

            <asp:TextBox ID="tbAddProdLinkUrl" Width="250px" runat="server"></asp:TextBox>

        </div>

        <asp:TextBox ID="tbAddProdLinkDescription" TextMode="MultiLine" Rows="4" Width="342px" runat="server"></asp:TextBox>

        <ajaxToolkit:TextBoxWatermarkExtender ID="tbAddProdLinkDescription_TextBoxWatermarkExtender"
            runat="server" Enabled="True" TargetControlID="tbAddProdLinkDescription"
            WatermarkText="Description">
        </ajaxToolkit:TextBoxWatermarkExtender>

        <div style="margin: 5px 0px 5px 0px;">


            <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'>
                <tr>
                    <td>
                        <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                    </td>
                    <td>
                        <input id="btnAddProductLink" runat="server" type="button" value="Add" class="defaultDecButton" />
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
                        <input id="btnHideAddProdLinkData" runat="server" type="button" value="Cancel" onclick="HideElementWithID('divAddProdLink', 'true'); ClearActionPnl();" class="defaultDecButton" />
                    </td>
                    <td>
                        <img alt="" src="images/SiteImages/btnBGRRight.png" />
                    </td>
                </tr>
            </table>

        </div>

    </div>

    <div id="divModifyProdLink" class="pnlPopUpSendMessage roundedCorners5" style="width: 350px;">

        <div class="sectionTextHeader" style="margin-bottom: 5px;">
            <asp:Label ID="lblModifyProductLink" runat="server" Text="Modify link"></asp:Label>
        </div>

        <textarea id="taModifProdLinkDescr" class="standardTextBoxes" rows="4" style="width: 342px;"></textarea>

        <div style="margin: 5px 0px 5px 0px;">


            <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'>
                <tr>
                    <td>
                        <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                    </td>
                    <td>
                        <input id="btnModifyProductLink" runat="server" type="button" value="Modify" onclick="ModifyProductLink()" class="defaultDecButton" />
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
                        <input id="btnDeleteProductLink" runat="server" type="button" value="Delete" onclick="DeleteProductLink()" class="defaultDecButton" />
                    </td>
                    <td>
                        <img alt="" src="images/SiteImages/btnBGRRight.png" />
                    </td>
                </tr>
            </table>

            <table id="tblDelProdLinkWarn" runat="server" cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'>
                <tr>
                    <td>
                        <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                    </td>
                    <td>
                        <input id="btnDeleteProductLinkWarn" runat="server" type="button" value="DeleteWarn" onclick="DeleteProductLinkW()" class="defaultDecButton" />
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
                        <input id="btnHideModifyProdLinkData" runat="server" type="button" value="Cancel" onclick="HideElementWithID('divModifyProdLink', 'true'); ClearActionPnl();" class="defaultDecButton" />
                    </td>
                    <td>
                        <img alt="" src="images/SiteImages/btnBGRRight.png" />
                    </td>
                </tr>
            </table>

        </div>

    </div>
    <asp:Panel ID="pnlActionReport" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5">
    </asp:Panel>

</asp:Content>
