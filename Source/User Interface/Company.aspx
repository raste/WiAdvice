<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Company.aspx.cs" Inherits="UserInterface.CompanyPage"
    MasterPageFile="MasterPage.Master" Theme="MainTheme" %>

<%@ Register Assembly="Microsoft.Web.GeneratedImage" Namespace="Microsoft.Web" TagPrefix="cc2" %>
<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript" language="javascript">
      
    </script>

    <style type="text/css">
        .style1 {
            width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

    <div>
        <div class="brb2">
            <div class="blb2">
                <div class="blhr">
                    <div class="contentBoxBottomHr clearfix2" style="padding-top: 10px;">

                        <div class="clearfix2">

                            <asp:HyperLink ID="imgLink" runat="server">
                                <cc2:GeneratedImage ID="giLogo" runat="server" CssClass="margin clearfix" ImageAlign="Left"
                                    BorderColor="Black" BorderWidth="1px" BorderStyle="Solid">
                                </cc2:GeneratedImage>
                            </asp:HyperLink>
                            <div style="text-align: right">
                                <asp:PlaceHolder ID="phAddedAndModified" runat="server"></asp:PlaceHolder>
                            </div>
                            <br />
                            <div style="text-align: center">
                                <asp:HyperLink ID="hlCompanyName" runat="server" CssClass="PCentered" Font-Size="X-Large">[hlCompanyName]</asp:HyperLink>
                            </div>
                            <br />
                            <asp:PlaceHolder ID="phWebSite" runat="server"></asp:PlaceHolder>
                            <br />
                            <asp:Label ID="lblAlternativeNames" runat="server" CssClass="searchPageComments" Text="Alternative names:"></asp:Label>
                            <div style="margin-top: 2px;">
                                <asp:Label ID="lblShare" runat="server" Text="Share :"></asp:Label>
                            </div>

                            <asp:Panel ID="pnlCompDescr" runat="server">
                                <br />
                                <asp:Label ID="lblCompDescr" runat="server" Text="Firm Description"></asp:Label>
                                <br />
                            </asp:Panel>
                            <br />
                            <asp:PlaceHolder ID="phAddedBy" runat="server"></asp:PlaceHolder>
                            <div class="floatRight">
                                <asp:Label ID="lblCompanyEditors" runat="server" Text="Maker editors"
                                    CssClass="lblEditors"></asp:Label>
                                <ajaxToolkit:PopupControlExtender ID="lblCompanyEditors_PopupControlExtender"
                                    runat="server" DynamicServicePath="" Enabled="True" ExtenderControlID=""
                                    TargetControlID="lblCompanyEditors" OffsetX="-270"
                                    PopupControlID="pnlPopUpEditors" Position="Bottom">
                                </ajaxToolkit:PopupControlExtender>

                            </div>

                        </div>

                        <asp:Panel ID="pnlUsrNotification" runat="server" Visible="False" CssClass="usrNotificationPnl">
                            <asp:Label ID="lblUsrNotification" runat="server" Text="User Notification"></asp:Label>
                        </asp:Panel>
                        <asp:Panel ID="pnlNotification" runat="server" Visible="False" HorizontalAlign="Center">
                            <asp:Label ID="lblCompNotif" runat="server" Font-Size="Larger" ForeColor="#CC3300"
                                Text="Notification :"></asp:Label>
                        </asp:Panel>


                    </div>

                    <img src="images/SiteImages/horL.png" align="left" />
                    <img src="images/SiteImages/horR.png" align="right" />

                </div>
            </div>
        </div>
    </div>


    <div class="panelNearSideElements">

        <ajaxToolkit:Accordion ID="accAdmin" runat="server" Visible="False" FramesPerSecond="40"
            RequireOpenedPane="False" SelectedIndex="-1">
            <Panes>
                <ajaxToolkit:AccordionPane ID="apAdmin" runat="server" Visible="True">
                    <Header>
                        <asp:Panel ID="pnlShowAdminPnl" runat="server" Visible="True" CssClass="accordionHeaders">
                            <asp:Label ID="lblAdminPanel" runat="server" CssClass="sectionTextHeader" Text="Admin Panel"></asp:Label>
                        </asp:Panel>
                    </Header>
                    <Content>
                        <asp:Panel ID="pnlAdmin" runat="server" Visible="True" CssClass="admBGR" Style="padding-left: 10px; padding-right: 10px;">
                            <br />
                            <asp:Label ID="lblVisible" runat="server" Text="Visible"></asp:Label>
                            <asp:Button ID="btnDeleteCompany" runat="server" OnClick="btnDeleteCompany_Click"
                                Text="Delete" />
                            <asp:Button ID="btnUndoDelete" runat="server" OnClick="btnUndoDelete_Click" Text="Make Visible" />

                            <asp:CheckBox ID="cbDeleteAllCompanyProducts" runat="server" Checked="True" Visible="False" CssClass="searchPageRatings"
                                Text="Change company products to company 'Other' after deleting. If not checked will delete all company products." />

                            <br />
                            <asp:Label ID="lblVisInfo" runat="server" Text="(NOTE : Deleting company will result in deleting all its connections , Make Visible is the opposite)"></asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblCanUserTakeRoleIfNoEditors" runat="server" Text="Can user take role if no editors"></asp:Label>
                            <asp:Button ID="btnChangeCanUserTakeRole" runat="server" Text="Change" OnClick="btnChangeCanUserTakeRole_Click" />
                            <br />
                            <br />
                            <asp:Panel ID="pnlGiveRoleToUser" runat="server">

                                <asp:Label ID="lblGiveRolesInfo" runat="server" Text="Give user edit roles for this company :"></asp:Label>
                                <asp:TextBox ID="tbUserRoles" runat="server" Columns="8"></asp:TextBox>
                                <ajaxToolkit:FilteredTextBoxExtender ID="tbUserRoles_FilteredTextBoxExtender" runat="server"
                                    FilterType="Numbers" TargetControlID="tbUserRoles">
                                </ajaxToolkit:FilteredTextBoxExtender>
                                <ajaxToolkit:TextBoxWatermarkExtender ID="tbUserRoles_TextBoxWatermarkExtender" runat="server"
                                    TargetControlID="tbUserRoles" WatermarkText="User ID">
                                </ajaxToolkit:TextBoxWatermarkExtender>

                                <asp:Button ID="btnGiveUserRoles" runat="server" OnClick="btnGiveUserRoles_Click"
                                    Text="Submit" />
                                &nbsp;<asp:PlaceHolder ID="phUserRoles" runat="server" Visible="False"></asp:PlaceHolder>

                            </asp:Panel>



                            <asp:Label ID="lblCompEditors" runat="server" Text="Users who can edit current company :"></asp:Label>
                            <asp:Table ID="tblCompEditors" runat="server" BorderColor="Black" BorderWidth="1px" Width="100%"
                                CssClass="margins">
                            </asp:Table>
                            <br />
                            <asp:Panel ID="pnlGiveRoleForAllProducts" runat="server">

                                <asp:Label ID="lblRolesInfo" runat="server" Text="Give user edit roles for all company products :"></asp:Label>
                                <asp:TextBox ID="tbRolesForAllProducts" runat="server" Columns="8"></asp:TextBox>
                                <ajaxToolkit:FilteredTextBoxExtender ID="tbRolesForAllProducts_FilteredTextBoxExtender"
                                    runat="server" FilterType="Numbers" TargetControlID="tbRolesForAllProducts">
                                </ajaxToolkit:FilteredTextBoxExtender>
                                <ajaxToolkit:TextBoxWatermarkExtender ID="tbRolesForAllProducts_TextBoxWatermarkExtender"
                                    runat="server" TargetControlID="tbRolesForAllProducts" WatermarkText="User ID">
                                </ajaxToolkit:TextBoxWatermarkExtender>

                                <asp:Button ID="btnGiveRolesForAllProducts" runat="server" OnClick="btnGiveRolesForAllProducts_Click"
                                    Text="Submit" />
                                &nbsp;<asp:PlaceHolder ID="phACompProdRoles" runat="server" Visible="False"></asp:PlaceHolder>

                            </asp:Panel>

                            <asp:Label ID="lblACompProdModificators" runat="server" Text="Users who can edit all company products :"></asp:Label>
                            <asp:Table ID="tblACompProdModificators" runat="server" BorderColor="Black" BorderWidth="1px" Width="100%"
                                CssClass="margins">
                            </asp:Table>
                        </asp:Panel>
                    </Content>
                </ajaxToolkit:AccordionPane>
            </Panes>
        </ajaxToolkit:Accordion>
        <asp:Panel ID="pnlEdit" runat="server" Visible="False">
            <div style="text-align: right">
                <asp:HyperLink ID="hlMoreInfo" runat="server" NavigateUrl="~/Rules.aspx#29">Click here</asp:HyperLink>
                <asp:Label ID="lblMoreInfo" runat="server" Text="for more information on editing makers."></asp:Label>
            </div>
            <ajaxToolkit:Accordion ID="accAddEdit" runat="server" FramesPerSecond="40" RequireOpenedPane="False"
                SelectedIndex="-1" SuppressHeaderPostbacks="True">
                <Panes>
                    <ajaxToolkit:AccordionPane ID="apEdit" runat="server">
                        <Header>
                            <asp:Panel ID="pnlAccEdit" runat="server" CssClass="accordionHeadersEdit">
                                <asp:Label ID="lblAccEdit" runat="server" Text="Edit"></asp:Label>
                            </asp:Panel>
                        </Header>
                        <Content>
                            <ajaxToolkit:Accordion ID="accEdit" runat="server" FramesPerSecond="40" RequireOpenedPane="False"
                                SelectedIndex="-1" SuppressHeaderPostbacks="True">
                                <Panes>
                                    <ajaxToolkit:AccordionPane ID="apEditDescription" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAccEditDescriptionHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditDescriptionHEader" runat="server" Text="Edit Description"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditDescriptionCOntent" runat="server" CssClass="editDescriptionBGR">
                                                <table class="style1">
                                                    <tr>
                                                        <td align="right" style="width: 150px;">
                                                            <asp:Label ID="lblEditDescription" runat="server" Text="Edit description : "></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbAccEditDescription" runat="server" Columns="70" Rows="10" TextMode="MultiLine"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <div class="accordionsDivPadding">
                                                                <asp:Label ID="lblSymbolsCount" runat="server" Text="Characters : "></asp:Label>
                                                                <asp:TextBox ID="tbSymbolsCount" runat="server" Columns="3" ReadOnly="True" Width="35px">0</asp:TextBox>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="dbEditDescription" runat="server" OnClick="dbEditDescription_Click"
                                                                Text="Update" />
                                                            <asp:PlaceHolder ID="phEditDescription" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apEditCharacteristics" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlEditCharacteristicHEader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditCharHeader" runat="server" Text="Edit Characteristics"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditCharacteristicsCOntent" runat="server" CssClass="editCharacteristicBGR">
                                                <asp:UpdatePanel ID="upEditCharacteristic" runat="server" UpdateMode="Always">
                                                    <ContentTemplate>
                                                        <table class="style1">
                                                            <tr>
                                                                <td align="right" style="width: 150px;">
                                                                    <asp:Label ID="lblEditCharChoose" runat="server" Text="Characteristic :"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddlEditCharacteristics" runat="server" AutoPostBack="True"
                                                                        OnSelectedIndexChanged="ddlEditCharacteristics_SelectedIndexChanged">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>&nbsp;
                                                                </td>
                                                                <td>
                                                                    <div class="accordionsDivPadding">
                                                                        <asp:Label ID="lblEditCharInfo1" runat="server" Text="Information 1"></asp:Label>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
                                                                    <asp:Label ID="lblEditCharNewName" runat="server" Text="New name :"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="tbEditCharNewName" runat="server" Columns="40"></asp:TextBox>
                                                                    <asp:Label ID="lblEditCharCHeckNewName" Font-Bold="true" ForeColor="#C02E29" runat="server" Text="Check Name"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">&nbsp;
                                                                </td>
                                                                <td>
                                                                    <div class="accordionsDivPadding">
                                                                        <asp:Label ID="lblEditCharInfo2" runat="server" Text="Information 2"></asp:Label>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
                                                                    <asp:Label ID="lblEditCharDescription" runat="server" Text="Description : "></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="tbEditCharDescription" runat="server" Columns="70" Rows="15"
                                                                        TextMode="MultiLine" Style="margin-bottom: 5px;"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <ajaxToolkit:UpdatePanelAnimationExtender ID="upaeEditCharacteristic" runat="server"
                                                    TargetControlID="upEditCharacteristic" BehaviorID="upaeEditCharacteristic" Enabled="true">
                                                    <Animations>
                                        <OnUpdating>
                                        <Sequence>       
                                            <%-- Disable all the controls --%>
                                            <Parallel duration="0">
                                                <EnableAction AnimationTarget="dbEditCharacteristic" Enabled="false"></EnableAction>  
                                                <EnableAction AnimationTarget="dbDeleteCharacteristic" Enabled="false"></EnableAction>  
                                                <EnableAction AnimationTarget="ddlEditCharacteristics" Enabled="false"></EnableAction>       
                                                <EnableAction AnimationTarget="tbEditCharNewName" Enabled="false"></EnableAction> 
                                                <EnableAction AnimationTarget="tbEditCharDescription" Enabled="false"></EnableAction>           
                                            </Parallel>
                                            </Sequence>
                                        </OnUpdating>
                                        <OnUpdated>
                                        <Sequence>
                                        <%-- Do each of the selected effects --%>
                                        <Parallel duration="0" >
                                        <%-- Enable all the controls --%>
                                                <EnableAction AnimationTarget="dbEditCharacteristic" Enabled="true"></EnableAction>  
                                                <EnableAction AnimationTarget="dbDeleteCharacteristic" Enabled="true"></EnableAction>  
                                                <EnableAction AnimationTarget="ddlEditCharacteristics" Enabled="true"></EnableAction>       
                                                <EnableAction AnimationTarget="tbEditCharNewName" Enabled="true"></EnableAction> 
                                                <EnableAction AnimationTarget="tbEditCharDescription" Enabled="true"></EnableAction>         
                                        </Parallel>
                                        </Sequence>
                                </OnUpdated>
                                                    </Animations>
                                                </ajaxToolkit:UpdatePanelAnimationExtender>
                                                <table>
                                                    <tr>
                                                        <td align="right" style="width: 150px;"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="dbEditCharacteristic" runat="server" OnClick="dbEditCharacteristic_Click"
                                                                Text="Update" />

                                                            <cc1:DecoratedButton ID="dbDeleteCharacteristic" runat="server" OnClick="dbDeleteCharacteristic_Click"
                                                                Text="Delete" />
                                                            <asp:PlaceHolder ID="phEditCharacteristic" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apDeleteCategories" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAccDeleteCategoryHEader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblAccDeleteCategoryesHEader" runat="server" Text="Delete category/ies"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlAccDeleteCategoriesContent" runat="server" CssClass="delCategoriesBGR">
                                                <table class="style1">
                                                    <tr>
                                                        <td align="right" style="width: 150px;">
                                                            <asp:Label ID="lblDeleteCategories" runat="server" Text="Categories : "></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBoxList ID="cblDeleteCategories" runat="server" CssClass="autoWidth">
                                                            </asp:CheckBoxList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;
                                                        </td>
                                                        <td>
                                                            <div class="accordionsDivPadding">
                                                                <asp:CheckBox ID="cbAdminDeleteCategories" runat="server" Visible="False" Checked="True" />
                                                                <asp:Label ID="lblDeleteCategoriesInfo" runat="server" Text="Information"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="top"></td>
                                                        <td>

                                                            <cc1:DecoratedButton ID="dbDeleteCategories" runat="server" OnClick="dbDeleteCategories_Click"
                                                                Text="Delete" />

                                                            <asp:PlaceHolder ID="phDeleteCategories" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                    <ajaxToolkit:AccordionPane ID="apChangeWebSite" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAccEditWebsiteHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditWebsiteHEader" runat="server" Text="Change website"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditWebisteCOntent" runat="server" CssClass="editSiteBGR">
                                                <table class="style1">
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblWebSiteInfo" runat="server" Text="Information"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="width: 150px;">
                                                            <asp:Label ID="lblNewWebSite" runat="server" Text="New website :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbEditWebsite" runat="server" Columns="60"></asp:TextBox>
                                                            <cc1:DecoratedButton ID="dbEditWebite" runat="server" OnClick="dbEditWebite_Click" Text="Change" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <asp:PlaceHolder ID="phEditWebsite" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                    <ajaxToolkit:AccordionPane ID="apChangeName" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlEditNameHEader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditNameHeader" runat="server" Text="Change name" CssClass="accordionCrucialDataLabels"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditNameContent" runat="server" CssClass="editSiteBGR">
                                                <table class="style1">
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblAccNewNameInfo" runat="server" Text="Information"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="width: 150px;">
                                                            <asp:Label ID="lblNewName" runat="server" Text="New name :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div class="accordionsDivPadding">
                                                                <asp:TextBox ID="tbEditName" runat="server" Columns="40"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvChangeName" runat="server" ErrorMessage="*" ControlToValidate="tbEditName" ValidationGroup="changeName"></asp:RequiredFieldValidator>

                                                                <asp:Label ID="lblAccCheckNewName" Font-Bold="true" ForeColor="#C02E29" runat="server" Text="CHeck name"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="dbChangeName" runat="server" OnClick="dbChangeName_Click" ValidationGroup="changeName"
                                                                Text="Change" />
                                                            <asp:PlaceHolder ID="phChangeName" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                    <ajaxToolkit:AccordionPane ID="apChangeType" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlEditTypeHEader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblEditTypeHEader" runat="server" Text="Change type"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlEditTypeContent" runat="server" CssClass="editBGR">
                                                <asp:UpdatePanel ID="upChangeType" runat="server" UpdateMode="Always">
                                                    <ContentTemplate>
                                                        <table class="style1">
                                                            <tr>
                                                                <td align="right" valign="top" style="width: 150px;">
                                                                    <asp:Label ID="lblChooseType" runat="server" Text="Choose type :"></asp:Label>
                                                                </td>
                                                                <td valign="top" style="width: 10px;">
                                                                    <asp:DropDownList ID="ddlChangeType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlChangeType_SelectedIndexChanged">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblChangeTypeInfo" runat="server" Text="Type info"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <ajaxToolkit:UpdatePanelAnimationExtender ID="upaeChangeType" runat="server" TargetControlID="upChangeType" Enabled="true">
                                                    <Animations>
                                                    <OnUpdating>
                                                    <Sequence>
                                                        <%-- Disable all the controls --%>
                                                        <Parallel duration="0">
                                                            <EnableAction AnimationTarget="ddlChangeType" Enabled="false"></EnableAction>  
                                                            <EnableAction AnimationTarget="dbChangeType" Enabled="false"></EnableAction>          
                                                        </Parallel>
                                                        </Sequence>
                                                    </OnUpdating>
                                                    <OnUpdated>
                                                    <Sequence>
                                                    <%-- Do each of the selected effects --%>
                                                    <Parallel duration="0" >
                                                    <%-- Enable all the controls --%>
                                                            <EnableAction AnimationTarget="ddlChangeType" Enabled="true"></EnableAction>  
                                                            <EnableAction AnimationTarget="dbChangeType" Enabled="true"></EnableAction>        
                                                    </Parallel>
                                                    </Sequence>
                                            </OnUpdated>
                                                    </Animations>
                                                </ajaxToolkit:UpdatePanelAnimationExtender>
                                                <table>
                                                    <tr>
                                                        <td align="right" style="width: 150px;">
                                                            <cc1:DecoratedButton ID="dbChangeType" runat="server" OnClick="dbChangeType_Click"
                                                                Text="Change" />
                                                        </td>
                                                        <td>&nbsp;
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                    <ajaxToolkit:AccordionPane ID="apRemoveAlternativeNames" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlRemoveAlternativeNamesHeader" runat="server"
                                                CssClass="accordionHeadersOptions">
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

                                </Panes>
                            </ajaxToolkit:Accordion>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                    <ajaxToolkit:AccordionPane ID="apAdd" runat="server">
                        <Header>
                            <asp:Panel ID="pnlAccAdd" runat="server" CssClass="accordionHeadersEdit">
                                <asp:Label ID="lblAccAdd" runat="server" Text="Add"></asp:Label>
                            </asp:Panel>
                        </Header>
                        <Content>
                            <ajaxToolkit:Accordion ID="accAdd" runat="server" FramesPerSecond="40" RequireOpenedPane="False"
                                SelectedIndex="-1" SuppressHeaderPostbacks="True">
                                <Panes>
                                    <ajaxToolkit:AccordionPane ID="apAddImage" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAddImageHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblAddImageHEader" runat="server" Text="Upload image"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlAddImage" runat="server" CssClass="uploadImgBGR" DefaultButton="btnUpload">
                                                <table width="100%">
                                                    <tr>
                                                        <td style="width: 150px; text-align: right; vertical-align: top;">
                                                            <asp:Label ID="lblChooseImg" runat="server" Text="Image :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:FileUpload ID="fuImage" runat="server" />&nbsp;&nbsp;  
                                                        
                                                        
                                                         <div class="accordionsDivPadding">
                                                             <asp:Label ID="lblUpImgINfo2" runat="server" Text="JPG,BMP,PNG`s are allowed."></asp:Label>
                                                             <br />
                                                             <asp:Label ID="lblUpImgINfo" runat="server" Text="Max allowed image size is 2mb."></asp:Label>
                                                             <br />
                                                             <asp:HyperLink ID="hlClickForMinImage" Target="_blank" class="galleryLink" runat="server">Click here</asp:HyperLink>
                                                             <asp:Label ID="lblClickForMinImage2" runat="server" Text="to see minimum image size."></asp:Label>
                                                             <br />
                                                             <asp:Label ID="lblUpImgLogoInfo" runat="server" Text="Min logo width/height: <br/>"></asp:Label>
                                                             <asp:HyperLink ID="hlClickForMinLogoSize" Target="_blank" class="galleryLink" runat="server">Click here</asp:HyperLink>
                                                             <asp:Label ID="lblClickForMinLogoSize2" runat="server" Text="to see."></asp:Label>
                                                         </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblUploadImageDescr" runat="server" Text="Description :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbImageDescr" runat="server" Columns="50" Rows="3" TextMode="MultiLine"
                                                                Style="margin-bottom: 5px;"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:CheckBox ID="cbLogo" runat="server" Text="Logo" Visible="False" />
                                                        </td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnUpload" runat="server" OnClick="btnUpload_Click" Text="Upload" />
                                                            <span id="uploadingSpan" runat="server" style="display: none">
                                                                <img alt="" src="images/SiteImages/uploading.gif" style="width: 32px; height: 16px" />
                                                                <asp:Label ID="lblUploadingImg" runat="server" Text="Uploading ..."></asp:Label>
                                                            </span>
                                                            <asp:PlaceHolder ID="phAddImage" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apAddCharacteristic" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAddCharacteristicHeader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblAddCharHeader" runat="server" Text="Add characteristic"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlAddChar" runat="server" CssClass="addCharBGR" DefaultButton="btnSubmitChar">
                                                <table width="100%">
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblAddCharInfo1" runat="server" Text="Information 1"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="width: 150px">
                                                            <asp:Label ID="lblCharName1" runat="server" Text="Name : "></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div class="accordionsDivPadding">
                                                                <asp:TextBox ID="tbCharName1" runat="server" Columns="40"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvCHarName" runat="server" ControlToValidate="tbCharName1"
                                                                    ErrorMessage="*" ValidationGroup="23"></asp:RequiredFieldValidator>
                                                                &nbsp;<asp:Label ID="lblCCName" ForeColor="#C02E29" Font-Bold="true" runat="server" Text="Check Name"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblAddCharInfo2" runat="server" Text="Information 2"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblAddCharDescription" runat="server" Text="Description :"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div class="accordionsDivPadding">
                                                                <asp:TextBox ID="tbCharDescr1" runat="server" Columns="70" Rows="15" TextMode="MultiLine"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvCharDescr" runat="server" ControlToValidate="tbCharDescr1"
                                                                    ValidationGroup="23"></asp:RequiredFieldValidator>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">&nbsp;
                                                        </td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnSubmitChar" runat="server" OnClick="btnSubmitChar_Click"
                                                                Text="Submit" ValidationGroup="23" />
                                                            <asp:PlaceHolder ID="phAddChar" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>
                                    <ajaxToolkit:AccordionPane ID="apAddCategory" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAddCategoriesHEader" runat="server" CssClass="accordionHeadersOptions">
                                                <asp:Label ID="lblAddCategoriesHEader" runat="server" Text="Add category/ies"></asp:Label>
                                            </asp:Panel>
                                        </Header>
                                        <Content>
                                            <asp:Panel ID="pnlAddCategory" runat="server" CssClass="addCategoryBGR" DefaultButton="btnSubmitCategory">
                                                <asp:UpdatePanel ID="upAddCategory" runat="server" UpdateMode="Always">
                                                    <ContentTemplate>


                                                        <asp:Label ID="lblAddCatInfo" runat="server" Style="margin-left: 150px;"
                                                            Text="If you choose category which have subcategories, all of them will be added."></asp:Label>


                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td align="right" style="width: 150px">
                                                                    <asp:Label ID="lblCat1" runat="server" Text="Category :"></asp:Label>
                                                                </td>
                                                                <td style="width: 1%;">
                                                                    <div class="accordionsDivPadding">
                                                                        <asp:Menu ID="navMenu" runat="server" CssClass="autoWidth"
                                                                            DynamicPopOutImageUrl="~/images/SiteImages/catMenuImgS.png"
                                                                            Font-Names="arial,georgia,&quot;Trebuchet MS&quot;,&quot;Times New Roman&quot;"
                                                                            DisappearAfter="1000"
                                                                            MaximumDynamicDisplayLevels="50"
                                                                            OnMenuItemClick="navMenu_MenuItemClick"
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
                                                                    </div>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="tbCatName1" runat="server" Columns="70" Enabled="False"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="rfvAddCategory" runat="server" ControlToValidate="tbCatName1"
                                                                        ErrorMessage="*" ValidationGroup="24"></asp:RequiredFieldValidator>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <ajaxToolkit:UpdatePanelAnimationExtender ID="upaeCategories" runat="server" TargetControlID="upAddCategory" Enabled="true">
                                                    <Animations>
                                                        <OnUpdating>
                                                        <Sequence>
                                                            <%-- Disable all the controls --%>
                                                            <Parallel duration="0">
                                                                <EnableAction AnimationTarget="btnSubmitCategory" Enabled="false"></EnableAction>                         
                                                            </Parallel>
                                                            </Sequence>
                                                        </OnUpdating>
                                                        <OnUpdated>
                                                        <Sequence>
                                                        <%-- Do each of the selected effects --%>
                                                        <Parallel duration="0" >
                                                        <%-- Enable all the controls --%>
                                                               <EnableAction AnimationTarget="btnSubmitCategory" Enabled="true"></EnableAction>
                                                        </Parallel>
                                                        </Sequence>
                                                </OnUpdated>
                                                    </Animations>
                                                </ajaxToolkit:UpdatePanelAnimationExtender>
                                                <table>
                                                    <tr>
                                                        <td align="right" style="width: 150px;"></td>
                                                        <td>
                                                            <cc1:DecoratedButton ID="btnSubmitCategory" runat="server" OnClick="btnSubmitCategory_Click"
                                                                Text="Submit" ValidationGroup="24" />
                                                            <asp:PlaceHolder ID="phAddCategory" runat="server" Visible="False"></asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </Content>
                                    </ajaxToolkit:AccordionPane>

                                    <ajaxToolkit:AccordionPane ID="apAddAlternativeName" runat="server">
                                        <Header>
                                            <asp:Panel ID="pnlAddAlternativeNameHeader" runat="server"
                                                CssClass="accordionHeadersOptions">
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
                                                            <div class="accordionsDivPadding">
                                                                <asp:Label ID="lblAddAlternativeNamesInfo" runat="server"
                                                                    Text="Alternative names info"></asp:Label>
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

        <ajaxToolkit:Accordion ID="accInformation" runat="server" FramesPerSecond="40" RequireOpenedPane="False"
            SelectedIndex="-1" SuppressHeaderPostbacks="True">
            <Panes>
                <ajaxToolkit:AccordionPane ID="apSHowGallery" runat="server">
                    <Header>
                        <asp:Panel ID="pnlShowGallery" runat="server" CssClass="accordionHeaders">
                            <asp:Label ID="lblGallery" runat="server" Text="Gallery"></asp:Label>
                        </asp:Panel>
                    </Header>
                    <Content>
                        <asp:Table ID="tblGallery" runat="server" Visible="True" CssClass="margins GalleryTable">
                        </asp:Table>
                    </Content>
                </ajaxToolkit:AccordionPane>
                <ajaxToolkit:AccordionPane ID="apShowCharacteristics" runat="server">
                    <Header>
                        <asp:Panel ID="pnlShowCharacteristics" runat="server" CssClass="accordionHeaders">
                            <asp:Label ID="lblChars" runat="server" Text="Characteristics"></asp:Label>
                        </asp:Panel>
                    </Header>
                    <Content>
                        <asp:Table ID="tblCharacteristics" runat="server" Visible="True" CssClass="margins CharacteristicsTable">
                        </asp:Table>
                    </Content>
                </ajaxToolkit:AccordionPane>
                <ajaxToolkit:AccordionPane ID="apShowCategories" runat="server" Visible="True">
                    <Header>
                        <asp:Panel ID="pnlShowCategories" runat="server" CssClass="accordionHeaders" Visible="True">
                            <asp:Label ID="lblCategories" runat="server" Text="Categories"></asp:Label>
                        </asp:Panel>
                    </Header>
                    <Content>
                        <asp:Table ID="tblCategories" runat="server" CssClass="margins CategoriesTable" Visible="False">
                        </asp:Table>
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

                                    <asp:Panel ID="pnlActions" runat="server">
                                        <asp:HyperLink ID="hlAddProduct" runat="server" Visible="False"
                                            CssClass="marginsLR8">Add Product</asp:HyperLink>
                                        <asp:Label ID="lblSendSuggestion" runat="server" CssClass="lblEditors"
                                            Text="Send suggestion"></asp:Label>


                                        <asp:Label ID="lblSendReport" runat="server" CssClass="sendReport"
                                            Text="Write report" Visible="False"></asp:Label>


                                        <input id="btnSignForNotifies" runat="server" class="htmlButtonStyle"
                                            style="margin-left: 5px; margin-right: 5px;" type="button" value="Notify"
                                            visible="false" />&nbsp;<asp:Button
                                                ID="btnTakeAction" runat="server" ForeColor="Red"
                                                OnClick="btnTakeAction_Click" Text="Take Role" Visible="False"
                                                CssClass="marginsLR8" Style="float: right;" />
                                        <hr />
                                    </asp:Panel>

                                    <table id="Table1" runat="server" style="width: 100%; margin: 5px 0px 5px 0px;">
                                        <tr>
                                            <td style="vertical-align: top;" valign="top">
                                                <asp:Label ID="lblSortBy" runat="server" Style="margin-left: 3px;" Text="Sort by : "></asp:Label>
                                                <asp:DropDownList ID="ddlSortByCat" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSortByCat_SelectedIndexChanged">
                                                </asp:DropDownList>
                                                <asp:Table ID="tblPages" runat="server" CssClass="autoWidth" Style="margin: 5px 0px 5px 0px;"></asp:Table>
                                                <asp:Table ID="tblProducts" runat="server" CellPadding="1"
                                                    CellSpacing="0" Width="100%" CssClass="bluePanels">
                                                </asp:Table>
                                                <asp:Table ID="tblPagesBtm" runat="server" CssClass="autoWidth" Style="margin-top: 5px;"></asp:Table>
                                            </td>
                                            <td id="adCell" style="width: 1px;">
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


    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FFCCFF">
        &nbsp;&nbsp;&nbsp;<br />
        <asp:Label ID="lblERROR" runat="server" Text="ERROR :" CssClass="errors"></asp:Label>
        <br />
        <asp:Label ID="lblAdded" runat="server" Text="Added by : "
            CssClass="searchPageComments"></asp:Label>
        <asp:Label ID="lblAddedOn" runat="server" CssClass="searchPageComments"
            Text="Added on : "></asp:Label>
        <br />
        <asp:Label ID="lblLastModified" runat="server" Text="Last Modified : "
            CssClass="searchPageComments"></asp:Label>
        <asp:Label ID="lblLastModifiedBy" runat="server" CssClass="searchPageComments"
            Text="Last modified by"></asp:Label>
        <br />
        <asp:Label ID="lblCompType" runat="server" Text="Type : "></asp:Label>
        <asp:Label ID="lblActions" runat="server" Style="margin-left: 5px;"
            Text="Actions :"></asp:Label>
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlPopUp" runat="server" CssClass="pnlPopUpStyle roundedCorners5" Width="450px">
    </asp:Panel>
    <asp:Panel ID="pnlSendReport" runat="server" CssClass="pnlPopUpReport roundedCorners5">
        <div class="sectionTextHeader" style="padding: 5px 0px 5px 0px;">
            <asp:Label ID="lblRepIrregularity" runat="server" Text="Report irregularity" ForeColor="#C02E29"></asp:Label>
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

    <asp:Panel ID="pnlNotify" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5"></asp:Panel>

    <asp:Panel ID="pnlPopUpEditors" CssClass="pnlPopUpEditors roundedCorners5" runat="server">
        <asp:PlaceHolder ID="phPublicCompEditors" runat="server"></asp:PlaceHolder>
    </asp:Panel>

    <asp:Panel ID="pnlSendTypeSuggestion" CssClass="pnlPopUpSendMessage roundedCorners5" runat="server" Width="350px">
        <div class="sectionTextHeader">
            <asp:Label ID="lblSendSuggestionInfo" runat="server" Text="Send suggestion"></asp:Label>
        </div>
        <asp:Label ID="lblSendSuggestionTo" runat="server" Text="to :"></asp:Label>
        <asp:DropDownList ID="ddlSuggestionUsers" runat="server">
        </asp:DropDownList>
        <asp:HyperLink ID="hlSuggestionUser" runat="server">user link</asp:HyperLink>
        <br />
        <textarea id="tbTypeSuggestion" class="standardTextBoxes" rows="8" style="margin-top: 5px; margin-bottom: 5px; width: 336px;"></textarea>
        <br />

        &nbsp;<input id="btnSendTypeSuggestion" runat="server" type="button" class="htmlButtonStyle" value="Send" onclick="SendTypeSuggestionToUser()" />
        <input id="btnCancelSuggestion" runat="server" type="button" class="htmlButtonStyle" value="Cancel" onclick="hideSendTypeSuggestionData()" />
        <br />
    </asp:Panel>

    <asp:Panel ID="pnlSendTypeSuggestionEnd" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5"></asp:Panel>

</asp:Content>
