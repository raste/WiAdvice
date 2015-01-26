<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddCompany.aspx.cs" Inherits="UserInterface.AddCompany" MasterPageFile="MasterPage.Master" Theme="MainTheme" %>


<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>


<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>


<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">

    <style type="text/css">
        .style1 {
            width: 100%;
        }
    </style>

    <script type="text/javascript" language="javascript">
        //This is where you create the javascript variable to hold the update panels ID
        var upMakerID = '<%= this.upMakerType.ClientID %>';
    </script>

</asp:Content>


<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">


    <asp:Panel ID="pnlAddSucc" runat="server" Visible="False">
        <br />
        <div style="text-align: center;">
            <asp:Label ID="lblAddSucc" runat="server" Font-Size="X-Large"
                Text="Adding maker Succesful !" CssClass="sectionTextHeader"></asp:Label>
        </div>

        <div style="margin-top: 10px; margin-bottom: 10px;">
            <div class="brb">
                <div class="blb">
                    <div class="blhrgreen">
                        <div class="contentBoxBottomHr">

                            <div style="padding-left: 100px; font-size: x-large; font-family: 'Times New Roman';">
                                <asp:Label ID="lblGoToCompsPage" runat="server" Text="Go to"></asp:Label>

                                <asp:Label ID="lblArrowsLeft" runat="server" CssClass="darkOrange" Text=" << "></asp:Label>

                                <asp:HyperLink ID="hlCompPage" Font-Size="X-Large" runat="server">Company</asp:HyperLink>

                                <asp:Label ID="lblArrowsRight" runat="server" CssClass="darkOrange" Text=" >> "></asp:Label>

                                <asp:Label ID="lblGoToCompsPage2" runat="server" Text="page to :"></asp:Label>
                            </div>
                            <div style="padding-left: 300px;">

                                <asp:BulletedList ID="blToDo1" runat="server" BulletStyle="CustomImage"
                                    Style="margin-bottom: 0px; font-family: 'Times New Roman'; font-size: large;" CssClass="searchPageRatings"
                                    BulletImageUrl="~/images/SiteImages/triangle.png">
                                    <asp:ListItem Value="0">To add categories in which the company will have products.</asp:ListItem>
                                </asp:BulletedList>
                                <asp:BulletedList ID="blToDo2" runat="server" BulletStyle="CustomImage"
                                    Style="margin-top: 0px; font-family: 'Times New Roman'; font-size: large;"
                                    BulletImageUrl="~/images/SiteImages/triangle.png">
                                    <asp:ListItem Value="1">To add product to the company.</asp:ListItem>
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


    <asp:Panel ID="pnlAddCompanyForm" runat="server" DefaultButton="btnCompSubmit">



        <div style="margin-top: 10px; margin-bottom: 10px;">
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
                                        <asp:Label ID="lblAbout" runat="server" Text="About Add Company"></asp:Label>
                                    </div>

                                    <br />

                                    <div class="addMakerBGR">
                                        <table style="width: 100%">
                                            <tr>
                                                <td colspan="3">
                                                    <div style="padding-bottom: 5px; text-align: center;">
                                                        <asp:Label ID="lblCompanyData" CssClass="sectionTextHeader" runat="server" Text="Maker Data"></asp:Label>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 150px;">
                                                    <asp:Label ID="lblCompName" runat="server" Text="Company name : "></asp:Label>
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="tbName" runat="server" Columns="40" ValidationGroup="1"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvName" runat="server"
                                                        ControlToValidate="tbName" ValidationGroup="1"></asp:RequiredFieldValidator>
                                                    &nbsp;<asp:Label ID="lblCcompName" runat="server" Text="Check Name"
                                                        CssClass="smallerText" Font-Bold="True" ForeColor="#C02E29"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td colspan="2">
                                                    <asp:Label ID="lblNameRules" runat="server" Text="Name Rules"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td colspan="2">&nbsp;&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblSite" runat="server" Text="Web site :"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbSite" runat="server" Columns="60"></asp:TextBox>
                                                </td>
                                                <td rowspan="7" style="vertical-align: top; width: 250px;">

                                                    <asp:Panel ID="pnlSimilarNames" runat="server">
                                                    </asp:Panel>

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
                                                <td align="right">&nbsp;</td>
                                                <td>
                                                    <asp:Label ID="lblDescriptionRules" runat="server" Text="Description Rules"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblDescription" runat="server" Text="Description :"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbDescription" runat="server" Columns="70" Rows="10"
                                                        TextMode="MultiLine" ValidationGroup="1"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server"
                                                        ControlToValidate="tbDescription" ValidationGroup="1"></asp:RequiredFieldValidator>
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
                                                        <cc1:DecoratedButton ID="btnCompSubmit" runat="server"
                                                            OnClick="btnCompSubmit_Click" Text="Submit" ValidationGroup="1" />

                                                        <asp:Label ID="lblError" runat="server" Text="ERROR :"
                                                            Visible="False" CssClass="errors"></asp:Label>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
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


    </asp:Panel>

    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FF99FF">
        <br />
        <asp:Label ID="lblCompType" runat="server" Text="Company type : "></asp:Label>
        <asp:Label ID="lblTypeRules" runat="server" Text="Type rules"></asp:Label>
        <br />
        <asp:UpdatePanel ID="upMakerType" runat="server">
            <ContentTemplate>
                <table class="style1">
                    <tr>
                        <td style="width: 10px;" valign="top">
                            <asp:DropDownList ID="ddlType" runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td valign="top">
                            <asp:Label ID="lblTypeInfo" runat="server" Text="Type Info"></asp:Label>
                            <br />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <ajaxToolkit:UpdatePanelAnimationExtender ID="upaeEdit" runat="server"
            BehaviorID="animation" TargetControlID="upMakerType">
            <Animations>
                        <OnUpdating>
                        <Sequence>
                         <%-- place the update progress div over the gridview control --%>
                           <ScriptAction Script="onUpdating(upMakerID);" />
                            <%-- Disable all the controls --%>
                            <Parallel duration="0">
                                <EnableAction AnimationTarget="ddlType" Enabled="false"></EnableAction>  
                                <EnableAction AnimationTarget="btnCompSubmit" Enabled="false"></EnableAction>                
                            </Parallel>
                            </Sequence>
                        </OnUpdating>
                        <OnUpdated>
                        <Sequence>
                        <%-- Do each of the selected effects --%>
                        <Parallel duration="0" >
                        <%-- Enable all the controls --%>
                                 <EnableAction AnimationTarget="ddlType" Enabled="true"></EnableAction>  
                                <EnableAction AnimationTarget="btnCompSubmit" Enabled="true"></EnableAction> 
                               <ScriptAction Script="onUpdated();" />
                        </Parallel>
                        </Sequence>
                </OnUpdated>
            </Animations>
        </ajaxToolkit:UpdatePanelAnimationExtender>
        <br />
        <asp:Button ID="btnCompSubmitOld" runat="server" OnClick="btnCompSubmit_Click"
            Style="height: 26px" Text="Submit" />
        <br />
        <br />
    </asp:Panel>

    <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle roundedCorners5"></asp:Panel>

</asp:Content>

