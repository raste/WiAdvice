<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddProduct.aspx.cs" Inherits="UserInterface.AddProduct" MasterPageFile="MasterPage.Master" Theme="MainTheme" %>


<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>


<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">
    <asp:Panel ID="pnlChooseCategoryOrCompany" Visible="false" runat="server"
        DefaultButton="btnProceedCompany">

        <div style="margin-top: 10px; margin-bottom: 10px;">
            <div class="brb2">
                <div class="blb2">
                    <div class="blhr">
                        <div class="contentBoxBottomHr">




                            <asp:Label ID="lblChoose" runat="server" Style="margin-left: 50px;"
                                CssClass="textHeader" Text="Choose category ot company"></asp:Label>
                            <br />


                            <table style="width: 100%; margin-top: 20px;">
                                <tr>
                                    <td style="width: 150px;" align="right">
                                        <asp:Label ID="lblChooseCat" runat="server" Text="Category : "></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Menu ID="menuChooseCategory" runat="server" CssClass="inlineTable"
                                            DisappearAfter="1000"
                                            DynamicPopOutImageUrl="~/images/SiteImages/catMenuImgS.png"
                                            Font-Names="arial,georgia,&quot;Trebuchet MS&quot;,&quot;Times New Roman&quot;"
                                            Font-Size="16px" MaximumDynamicDisplayLevels="50"
                                            StaticPopOutImageUrl="~/images/SiteImages/MenuImgCircle.png"
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
                                                Font-Overline="False" Font-Size="Medium" ForeColor="#004F00" />
                                            <DynamicHoverStyle Font-Italic="False"
                                                Font-Underline="False" CssClass="noEffect" ForeColor="#254b28" />
                                            <DynamicMenuItemStyle
                                                CssClass="categoryLinks" HorizontalPadding="3px" />
                                            <StaticHoverStyle CssClass="noEffect" />
                                        </asp:Menu>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="lblChooseComp" runat="server" Text="Company : "></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbChooseCompany" runat="server" Columns="40" autocomplete="off"
                                            ValidationGroup="chooseCompany"></asp:TextBox>
                                        <ajaxToolkit:AutoCompleteExtender ID="tbChooseCompany_AutoCompleteExtender"
                                            runat="server" TargetControlID="tbChooseCompany"
                                            ServiceMethod="GetCompaniesList"
                                            CompletionSetCount="20"
                                            MinimumPrefixLength="2"
                                            CompletionListCssClass="completionListElement"
                                            CompletionListItemCssClass="completionListItem"
                                            CompletionListHighlightedItemCssClass="completionListHighlightedItem"
                                            CompletionInterval="10">
                                        </ajaxToolkit:AutoCompleteExtender>
                                        <asp:RequiredFieldValidator ID="rfvChooseCompany" runat="server"
                                            ControlToValidate="tbChooseCompany" ErrorMessage="*" ValidationGroup="chooseCompany"></asp:RequiredFieldValidator>
                                        <asp:Button ID="btnProceedCompany" runat="server" Text="Proceed"
                                            OnClick="btnProceedCompany_Click" ValidationGroup="chooseCompany" />
                                    </td>
                                </tr>
                            </table>

                            <asp:Label ID="lblErrorChooseCom" runat="server" Visible="false" Style="margin-left: 20px;" CssClass="errors"
                                Text="Error choose company"></asp:Label>
                            <br />
                            <div style="margin-left: 50px;">
                                <asp:HyperLink ID="hlAddProdGuide" runat="server">Information</asp:HyperLink>
                                <asp:Label ID="lblAddProdGuide" runat="server" Text="about adding product."></asp:Label>
                                <br />
                                <asp:HyperLink ID="hlAddProdRules" runat="server">Rules</asp:HyperLink>
                                <asp:Label ID="lblAddProdRules" runat="server" Text="about adding/editing product."></asp:Label>
                            </div>



                        </div>

                        <img src="images/SiteImages/horL.png" align="left" />
                        <img src="images/SiteImages/horR.png" align="right" />

                    </div>
                </div>
            </div>
        </div>


    </asp:Panel>

    <asp:Panel ID="pnlAddSucc" runat="server" Visible="False">




        <div style="margin-top: 10px; margin-bottom: 10px;">
            <div class="brb">
                <div class="blb">
                    <div class="blhrgreen">
                        <div class="contentBoxBottomHr">


                            <div style="text-align: center;">
                                <asp:Label ID="lblAddSucc" runat="server" Font-Size="X-Large"
                                    Text="Adding Product Succesful ! " CssClass="sectionTextHeader"></asp:Label>
                            </div>
                            <br />
                            <div style="padding-left: 100px; font-size: x-large; font-family: 'Times New Roman';">

                                <asp:Label ID="lblGoToProdsPage" runat="server" Text="Go to"></asp:Label>

                                <asp:Label ID="lblArrowsLeft" runat="server" CssClass="darkOrange" Text=" << "></asp:Label>

                                <asp:HyperLink ID="hlProductPage" runat="server" Font-Size="X-Large">Product Page</asp:HyperLink>

                                <asp:Label ID="lblArrowsRight" runat="server" CssClass="darkOrange" Text=" >> "></asp:Label>

                                <asp:Label ID="lblGoToProdsPage2" runat="server" Text="page to :"></asp:Label>
                            </div>
                            <div style="padding-left: 300px;">

                                <asp:BulletedList ID="blToDo" runat="server" BulletStyle="CustomImage"
                                    Style="font-family: 'Times New Roman'; font-size: large;"
                                    BulletImageUrl="~/images/SiteImages/triangle.png">
                                    <asp:ListItem Value="1">To add variants and subvariants.</asp:ListItem>
                                    <asp:ListItem Value="2">To modify the description.</asp:ListItem>
                                    <asp:ListItem Value="3">To add characteristics.</asp:ListItem>
                                    <asp:ListItem Value="4">To add alternative names.</asp:ListItem>
                                    <asp:ListItem Value="5">To add images in company`s gallery.</asp:ListItem>
                                </asp:BulletedList>

                            </div>




                        </div>
                    </div>
                </div>
            </div>
        </div>

    </asp:Panel>


    <asp:Panel ID="pnlAddProductForm" Visible="false" runat="server">



        <div style="margin-bottom: 10px; margin-top: 10px;">
            <div class="trb">
                <div class="tlb">
                    <div class="brb2">
                        <div class="blb2">


                            <div class="blhr">

                                <div class="contentBoxTopBottomHr">


                                    <div style="margin-bottom: 10px; /*margin-left: 365px; */ text-align: center;">
                                        <asp:Label ID="lblPageIntro" CssClass="sectionTextHeader" runat="server" Text="Add product"></asp:Label>
                                    </div>


                                    <div style="padding-left: 5px; padding-right: 5px;">
                                        <asp:Label ID="lblAbout" runat="server" Text="About Add Product"></asp:Label>
                                    </div>

                                    <br />

                                    <asp:Panel ID="pnlAddProduct" runat="server" Visible="False"
                                        CssClass="addProductBGR" DefaultButton="btnSubmit">

                                        <table style="width: 100%">
                                            <tr>
                                                <td colspan="3">

                                                    <div style="padding-bottom: 5px; text-align: center;">
                                                        <asp:Label ID="lblProductData" CssClass="sectionTextHeader" runat="server" Text="Product data"></asp:Label>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 150px;">&nbsp;<asp:Label ID="lblName" runat="server" Text="Name"></asp:Label>
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="tbName" runat="server" Columns="40" ValidationGroup="2"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvName" runat="server"
                                                        ControlToValidate="tbName" ValidationGroup="2"></asp:RequiredFieldValidator>
                                                    &nbsp;<asp:Label ID="lblCName" runat="server" Text="CheckName"
                                                        CssClass="smallerText" Font-Bold="True" ForeColor="#C02E29"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td colspan="2">
                                                    <asp:Label ID="lblNameRules" runat="server" Text="Name Rules" Width="600px"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td colspan="2">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="top">&nbsp;<asp:Label ID="lblCompany" runat="server" Text="Maker : "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddCompany" runat="server">
                                                    </asp:DropDownList>

                                                    <div style="padding: 10px 10px 20px 0px">
                                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblCompHint1" runat="server" Text="Ако компанията, за която искате да добавите продукт не е в списъка, можете да :"></asp:Label>
                                                        <br />
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <asp:Label ID="lblCompHint2" runat="server" Text="1. Добавите първо компанията и после продукта."></asp:Label>
                                                        <br />
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <asp:Label ID="lblCompHint3" runat="server" Text="2. Добавите продукта към „Друга”."></asp:Label>
                                                        <br />
                                                        <br />
                                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblCompHint4" runat="server" Text="Ако компанията е добавена към сайта, но не е отбелязана в падащото меню, свържете се с редактора и му предложете да добави тази категория. Повече за това може да"></asp:Label>
                                                        <asp:HyperLink ID="hlCompHint4" Target="_blank" runat="server">прочетете тук</asp:HyperLink>.
                                     
                                                    </div>

                                                </td>

                                                <td rowspan="11" style="vertical-align: top; width: 250px;">
                                                    <asp:Panel ID="pnlSimilarNames" runat="server">
                                                    </asp:Panel>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td align="right" valign="top">&nbsp;<asp:Label ID="lblCategory" runat="server" Text="Category : "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddCategory" runat="server">
                                                    </asp:DropDownList>

                                                    <div style="padding: 10px 10px 5px 0px">
                                                        &nbsp;&nbsp;&nbsp; 
                                                        <asp:Label ID="lblCatHint1" runat="server" Text="Ако искате да добавите продукт към „компания”, но тя няма съответната категория за него, може да :"></asp:Label>
                                                        <br />
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                                                        <asp:Label ID="lblCatHint2" runat="server" Text="1. Изпратите предложение на редактора на компанията да добави „категорията”."></asp:Label>
                                                        <br />
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <asp:Label ID="lblCatHint3" runat="server" Text="2. Изберете категорията „Други -> Неoпределени” от падащото меню и посочите категорията, съответстваща на продукта."></asp:Label>

                                                    </div>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td>
                                                    <asp:Panel ID="pnlWantedCategory" Style="visibility: hidden; margin-bottom: 10px;" runat="server">



                                                        <asp:UpdatePanel ID="upWantedCategory" runat="server">
                                                            <ContentTemplate>

                                                                <asp:Label ID="lblUnspecifiedCategoryInfo" runat="server" Style="margin-left: 130px; font-size: large; color: #003399;"
                                                                    Text="Unspecified Category Info"></asp:Label>
                                                                <br />

                                                                <table>
                                                                    <tr>
                                                                        <td style="width: 1px;">

                                                                            <asp:Menu ID="wantedCatMenu" runat="server" CssClass="inlineTable"
                                                                                DisappearAfter="1000"
                                                                                DynamicPopOutImageUrl="~/images/SiteImages/catMenuImgS.png"
                                                                                Font-Names="arial,georgia,&quot;Trebuchet MS&quot;,&quot;Times New Roman&quot;"
                                                                                MaximumDynamicDisplayLevels="50"
                                                                                OnMenuItemClick="wantedCatMenu_MenuItemClick"
                                                                                StaticPopOutImageUrl="~/images/SiteImages/navMenusImg.png"
                                                                                StaticSubMenuIndent="" DynamicHorizontalOffset="1" Orientation="Horizontal"
                                                                                SkipLinkText="">
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
                                                                                    Font-Overline="False" Font-Size="15px" ForeColor="#004F00" />
                                                                                <DynamicHoverStyle Font-Italic="False"
                                                                                    Font-Underline="False" CssClass="noEffect" ForeColor="#254b28" />
                                                                                <DynamicMenuItemStyle
                                                                                    CssClass="categoryLinks" HorizontalPadding="3px" />
                                                                                <StaticHoverStyle CssClass="noEffect" />
                                                                            </asp:Menu>

                                                                        </td>
                                                                        <td>

                                                                            <asp:TextBox ID="tbWantedCategory" runat="server" Enabled="False" Columns="60"></asp:TextBox>

                                                                        </td>
                                                                    </tr>
                                                                </table>

                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>


                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblSite" runat="server" Text="Web site : "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbSite" runat="server" Columns="60"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td>
                                                    <asp:Label ID="lblSiteRules" runat="server" Text="Site Rules"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td>&nbsp;&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right"></td>
                                                <td>
                                                    <asp:Label ID="lblDescriptionRules" runat="server" Text="Description Rules"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;<asp:Label ID="lblDescription" runat="server" Text="Description : "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbDescr" runat="server" Columns="70" Rows="10"
                                                        TextMode="MultiLine" ValidationGroup="2"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server"
                                                        ControlToValidate="tbDescr" ValidationGroup="2"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td>
                                                    <asp:Label ID="lblSymbolsCount" runat="server" Text="Characters : "></asp:Label>
                                                    <asp:TextBox ID="tbSymbolsCount" runat="server" Columns="3" ReadOnly="True" Width="35px">0</asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="top"></td>
                                                <td>
                                                    <div class="accordionsDivPadding">
                                                        <cc1:DecoratedButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click"
                                                            Text="Add Product" ValidationGroup="2" />
                                                        &nbsp;<asp:Label ID="lblError" runat="server" Text="ERROR"
                                                            Visible="False" CssClass="errors"></asp:Label>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>


                                    </asp:Panel>






                                </div>

                                <img src="images/SiteImages/horL.png" align="left" />
                                <img src="images/SiteImages/horR.png" align="right" />

                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>



    </asp:Panel>
    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FF99FF">
        <br />
        <asp:Button ID="btnSubmitOld" runat="server" Height="26px"
            OnClick="btnSubmit_Click" Text="Add Product" />
        <br />
        <div style="padding-left: 310px; font-size: large; font-family: 'Times New Roman';">

            <asp:Label ID="lblOr" runat="server" Text="or" Style="margin-right: 20px;"></asp:Label>

            <asp:Label ID="lblAddAnother" runat="server" Text="Add another"></asp:Label>
            &nbsp;<asp:HyperLink ID="hlAddAnother" Font-Size="Large" runat="server">HyperLink</asp:HyperLink>

            <asp:Label ID="lblAddToSameCategory" runat="server" Text="add to same category"></asp:Label>
            &nbsp;<asp:HyperLink ID="hlCategoryLink" Font-Size="Large" runat="server">HyperLink</asp:HyperLink>
        </div>
        <br />
    </asp:Panel>

    <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle roundedCorners5"></asp:Panel>

</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
</asp:Content>

