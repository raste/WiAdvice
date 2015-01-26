<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SuggestionsForSite.aspx.cs" Inherits="UserInterface.SuggestionsForSite" MasterPageFile="~/MasterPage.Master" Theme="MainTheme"%>


<%@ Register assembly="CustomServerControls" namespace="CustomServerControls" tagprefix="cc1" %>


<asp:Content ID="Content1" runat="server" 
    contentplaceholderid="ContentPlaceHolder1">



        <div style="margin-top:10px; margin-bottom:10px;">
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
                        
                            <asp:Label ID="lblAbout" runat="server" Text="Suggestions About"></asp:Label>
                            <br />
                        &nbsp;<asp:Label ID="lblCountInfo" style="margin-left:10px;" runat="server" Text="Count info"></asp:Label>
                            <br />
                        <asp:Panel ID="pnlWriteSuggestion" runat="server" Visible="False" style="margin-bottom:10px; margin-top:10px;" 
                            CssClass="addSuggestionBGR" DefaultButton="btnAddSuggestion">
                            <table class="style1" style="width:100%;">
                                <tr>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="lblWriteSugg" CssClass="sectionTextHeader" style="margin-left:200px;" runat="server" Text="Write Suggestion : "></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="lblSuggType" runat="server" Text="About :"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSuggType" runat="server">
                                            <asp:ListItem Value="0">general</asp:ListItem>
                                            <asp:ListItem Value="1">design</asp:ListItem>
                                            <asp:ListItem Value="2">features</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width:150px">
                                        <asp:Label ID="lblSuggestionDescr" runat="server" Text="Description : "></asp:Label>
                                    </td>
                                    <td valign="top">
                                    <div style="padding:5px 0px 5px 0px;">
                                        <asp:TextBox ID="tbSuggestionDescr" runat="server" Columns="80" Rows="8" 
                                            TextMode="MultiLine" ValidationGroup="12"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvSuggestion" runat="server" 
                                            ControlToValidate="tbSuggestionDescr" ValidationGroup="12"></asp:RequiredFieldValidator>
                                            </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        
                                    </td>
                                    <td>
                                    <cc1:DecoratedButton ID="btnAddSuggestion" runat="server" 
                                            onclick="btnAddSuggestion_Click" Text="Submit" ValidationGroup="12" />
                                             
                                        <asp:PlaceHolder ID="phAddSuggestion" runat="server" Visible="False">
                                        </asp:PlaceHolder>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        
                        <ajaxToolkit:Accordion ID="accAdmin" runat="server" Visible="False" 
                                FramesPerSecond="40" RequireOpenedPane="False" SelectedIndex="-1">
                        <Panes>
                            <ajaxToolkit:AccordionPane ID="apAdmin" runat="server">
                            <Header>
                            <asp:Panel ID="pnlShowAdminPnl" runat="server" Visible="True" CssClass="accordionHeaders">
                               <asp:Label ID="lblAdminPanel" runat="server" CssClass="sectionTextHeader" 
                            Text="Admin Panel"></asp:Label>
                            </asp:Panel>
                            </Header>
                            <Content>
                            
                            <asp:Panel ID="pnlAdmin" runat="server" CssClass="admBGR" 
                            DefaultButton="btnShowDeleted">
                            
                            <br />
                            &nbsp;<asp:Label ID="lblShowDelSugg" runat="server" 
                                Text="Show Last Deleted Suggestions : "></asp:Label>
                            <asp:TextBox ID="tbShowDeleted" runat="server" Columns="7"></asp:TextBox>
                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="tbShowDeleted" FilterType="Numbers">
                                </ajaxToolkit:FilteredTextBoxExtender>
                               
                            &nbsp;<asp:Button ID="btnShowDeleted" runat="server" onclick="btnShowDeleted_Click" 
                                Text="Show" />
                            &nbsp;&nbsp;<asp:PlaceHolder ID="phShowDeleted" runat="server" Visible="False">
                            </asp:PlaceHolder>
                                    <br />
                            <asp:Table ID="tblShowLastDeleted" runat="server" CellSpacing="0" 
                                CssClass="margins" Visible="False" Width="100%" BorderColor="Black" 
                                BorderStyle="Solid" BorderWidth="1px">
                            </asp:Table>
                            <hr />
                        </asp:Panel>
                            
                            </Content>
                            </ajaxToolkit:AccordionPane>
                        </Panes>
                        </ajaxToolkit:Accordion>

                            
                        
                        <div style="margin:10px 0px 5px 0px; ">
                        
                        &nbsp;<asp:Label ID="lblCount" runat="server" Text="Suggestions  : " 
                                CssClass="textHeader"></asp:Label>
                        <div class="floatRightNoMrg">
                            <asp:Label ID="lblShow" runat="server" Text="Show :" style=""></asp:Label>
                        
                            &nbsp;<asp:DropDownList ID="ddlShowByType" runat="server" AutoPostBack="True" 
                            onselectedindexchanged="ddlShowByType_SelectedIndexChanged" style="margin-right:10px;">
                                <asp:ListItem>select</asp:ListItem>
                                <asp:ListItem Value="all"></asp:ListItem>
                                <asp:ListItem Value="0">general</asp:ListItem>
                                <asp:ListItem Value="1">design</asp:ListItem>
                                <asp:ListItem Value="2">features</asp:ListItem>
                        </asp:DropDownList>
                        </div>
                        </div>
                        
                            <asp:Table ID="tblPages" runat="server" CssClass="autoWidth" style="display:inline-table;">
                        </asp:Table>

                        <table style="width:100%; border-collapse: collapse;  " cellpadding="0" cellspacing="0">
                            <tr>
                                <td valign="top">
                        
                            <asp:PlaceHolder ID="phSuggestions" runat="server"></asp:PlaceHolder>
                        
                        
                        
                            <asp:Table ID="tblPagesBtm" runat="server" CssClass="autoWidth" style="margin:5px 0px 5px 0px;">
                        </asp:Table>
                        
                        
                                </td>
                                <td runat="server" id="adcell" valign="top">
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


      


    

        <asp:HiddenField ID="hfDeletedNum" runat="server" />
    
    <asp:Panel ID="pnlHidden" runat="server" Visible="False" BackColor="#FFCCFF">
        <asp:Label ID="lblError" runat="server" Text="Error : " CssClass="errors"></asp:Label>
        <br />
        <br />
        <asp:Button ID="btnAddSuggestionOLD" runat="server" 
            onclick="btnAddSuggestion_Click" Text="Submit" />
        <asp:Table ID="tblSuggestions" runat="server" CellSpacing="5" Width="100%">
        </asp:Table>
    </asp:Panel>   
    
    
    <asp:Panel ID="pnlSuggAction" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5"></asp:Panel>
    
</asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="head">

    <style type="text/css">
        .style1
        {
            width: 100%;
        }
    </style>

</asp:Content>

