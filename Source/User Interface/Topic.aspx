<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Topic.aspx.cs" Inherits="UserInterface.Topic" Theme="MainTheme"%>

<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     


        <div style="margin:10px 0px 10px 0px;">
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
                         
                                <div class="sectionTextHeader" style="margin:5px 0px 10px 0px;">
                                
                                 <asp:HyperLink ID="hlProduct" style="font-size:21px; color:#003399;" runat="server">Product</asp:HyperLink>
                        &gt;
                                <asp:HyperLink ID="hlForum" style="font-size:21px;" runat="server">Forum</asp:HyperLink>
                        &gt;
                                <asp:HyperLink ID="hlTopic"  style="font-size:21px;" runat="server">Topic</asp:HyperLink>
                                
                                </div>

                            
                               
                            
                            
                            <ajaxToolkit:Accordion ID="accAdmin" runat="server" FramesPerSecond="40" RequireOpenedPane="False" SelectedIndex="-1" Visible="False" style="margin:10px 0px 10px 0px;">
                            <Panes>
                            
                                <ajaxToolkit:AccordionPane ID="apShowAdminPanel" runat="server">
                                <Header>
                                 
                                   <asp:Panel ID="pnlShowAdminPnl" runat="server" Visible="True" CssClass="accordionHeaders">
                                   <asp:Label ID="lblAdminPanel" runat="server" CssClass="sectionTextHeader" 
                                Text="Admin Panel"></asp:Label>
                                </asp:Panel>
                                
                                </Header>
                                
                                <Content>
                                
                                 <asp:Panel ID="pnlAdmin" runat="server" CssClass="admBGR">
                            
                                <asp:Label ID="lblTopicStatus" runat="server" Text="Status"></asp:Label>
                                &nbsp;<asp:Button ID="btnDelTopic" runat="server" Text="Delete" 
                                            onclick="btnDelTopic_Click" />
                                &nbsp;<asp:Button ID="btnDelTopicW" runat="server" onclick="btnDelTopicW_Click" 
                                    Text="Delete with warning" />
                                &nbsp;<asp:Button ID="btnUndelete" runat="server" onclick="btnUndelete_Click" 
                                    Text="Undelete" />
                                &nbsp;<asp:Button ID="btnLock" runat="server" onclick="btnLock_Click" 
                                    Text="Lock" />
                                &nbsp;<asp:Button ID="btnLockW" runat="server" onclick="btnLockW_Click" 
                                    Text="Lock with warning" />
                                &nbsp;<asp:Button ID="btnUnlock" runat="server" onclick="btnUnlock_Click" 
                                    Text="Unlock" />
                                
                            </asp:Panel>
                                
                                
                                </Content>
                                
                                </ajaxToolkit:AccordionPane>
                                </Panes>
                                </ajaxToolkit:Accordion>
                                

                            <div class="clearfix" style="margin-bottom:15px;">
                               
                            
                            <asp:Panel ID="pnlActionsTop" runat="server" CssClass="" style="display:inline-block;">
                          
                                <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle' id="tblReplyTop" runat="server"><tr><td>
                                                        <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                                                    </td><td>
                                                        <input id="btnReplyTop" runat="server" type="button" value="Reply" class="defaultDecButton" />
                                                    </td><td>
                                                        <img alt="" src="images/SiteImages/btnBGRRight.png" />
                                                    </td></tr>
                                                </table>
                                                
                                                
                                                 <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle' id="tblNotify" runat="server"><tr><td>
                                                        <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                                                    </td><td>
                                                        <input id="btnSignNotifiesTop" runat="server" type="button" value="Notify" class="defaultDecButton" />
                                                    </td><td>
                                                        <img alt="" src="images/SiteImages/btnBGRRight.png" />
                                                    </td></tr>
                                                </table>
                               
                            </asp:Panel>
                            
                            <div class="floatRightNoMrg">
                            <asp:PlaceHolder ID="phPagesTop" runat="server"></asp:PlaceHolder>
                            </div>
                                
                            </div>
                            
                            
                            <asp:Panel ID="pnlTopic" runat="server" CssClass="topicInfoPnl" style="">
                            <div class="topicInfoPnlHeader clearfix">
                            
                            <asp:Image ID="imgTopic" ImageAlign="Left" Height="33" style="margin:4px 10px 0px 0px;" runat="server" />
                            
                                
                                
                                    <asp:HyperLink ID="hlTopicName" runat="server" CssClass="topicSubjectLink">Topic name</asp:HyperLink>
                                    
                                    <div class="floatRightNoMrg">
                                    
                                    
                                        <asp:Label ID="lblModifyTopic" runat="server" Text="Modify" CssClass="commentActionButton" 
                                            Visible="False"></asp:Label>
                                    
                                        <asp:Label ID="lblReport" runat="server" Text="Report" CssClass="sendReport" 
                                            Visible="False"></asp:Label>
                                    
                                       
                                    
                                    </div>
                                    <br />
                                
                                
                                    <asp:Label ID="lblStartedBy" runat="server" CssClass="topicsTextStyleSmall" Text="Started by "></asp:Label>
                                    <asp:HyperLink ID="hlStartedBy" runat="server">User</asp:HyperLink>
                                <asp:Label ID="lblStartedByAdm" runat="server" Text="User"></asp:Label>
                                    <asp:Label ID="lblStartedOn" CssClass="topicsTextStyleSmall" runat="server" Text=", on Date"></asp:Label>
                                    
                                    
                                 <asp:Panel ID="pnlTopicModifications" runat="server" CssClass="floatRightNoMrg" style="padding-top:2px;">
                                    <asp:Label ID="lblTopicModification" CssClass="topicsModificationLbl" runat="server" Text="Modification"></asp:Label>
                                </asp:Panel>
                            </div>
                            <div class="topicInfoPnlDescription">
                                <asp:Label ID="lblDescription" runat="server" Text="Description"></asp:Label>
                            </div>    
                            </asp:Panel>
                            
                    <asp:Panel ID="pnlTopicInfo" runat="server" CssClass="topicsInfoPnl">
                    
                        <asp:Label ID="lblTopicName" style="color:White; font-weight:bold;" runat="server" Text="Name"></asp:Label>
                    
                    </asp:Panel>
                            
                            
                            
                             <div id="advertsDiv" runat="server" style="float:right;"> 
                            
                                 <asp:PlaceHolder ID="phAdverts" runat="server"></asp:PlaceHolder>
                            
                            </div>
                           
                           
                            <div id="commentsDiv" runat="server"> 
                            
                           
                            
                            <asp:PlaceHolder ID="phComments" runat="server"></asp:PlaceHolder>
                            
                             <div class="clearfix" style="margin-top:15px;">
                               
                           
                                <asp:Panel ID="pnlActionsBottom" runat="server" CssClass="" style="display:inline-block;">
                            
                            
                            <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle' id="tblReplyBottom" runat="server"><tr><td>
                                                        <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                                                    </td><td>
                                                        <input id="btnReplyBottom" runat="server" type="button" value="Reply" class="defaultDecButton" />
                                                    </td><td>
                                                        <img alt="" src="images/SiteImages/btnBGRRight.png" />
                                                    </td></tr>
                            </table>
 
                                 </asp:Panel>
                                 
                                  <asp:Panel ID="pnlRegToAdd" CssClass="panelInline" runat="server">
                               
                                   <asp:Label ID="lblRegToAdd1" runat="server" Text="Искаш и ти да дадеш мнението си? Ами,"></asp:Label>
                                   <asp:HyperLink ID="hlRegToAdd1" runat="server">регистрирай</asp:HyperLink>
                                   /
                                   <asp:HyperLink ID="hlRegToAdd2" runat="server">логни</asp:HyperLink>
                                   <asp:Label ID="lblRegToAdd2" runat="server" Text="се тогава...!"></asp:Label>
                               </asp:Panel>
                                 
                                 
                                 <div class="floatRightNoMrg">
                                
                                 <asp:PlaceHolder ID="phPagesBottom" runat="server"></asp:PlaceHolder>
                                
                                </div>
                                 
                            </div>
                            
                        
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
        
        



    
    
    <div id="divModifyTopic" class="pnlPopUpAddTopic roundedCorners5">
    
    <div class="sectionTextHeader" style="margin-bottom:5px;">
    
        <asp:Label ID="lblModifyTopicInfo" runat="server" Text="Modify topic"></asp:Label>
    
    </div>

        <table cellspacing="1" style="width:100%;">
            <tr>
                <td style="width:150px; text-align:right;">
                
                    <asp:Label ID="lblModifTopicSubject" runat="server" Text="New subject :"></asp:Label>
                    
                </td>
                <td>

                    <asp:TextBox ID="tbModifTopicSubject" runat="server" style="width:300px;"></asp:TextBox>
                    
                </td>
            </tr>
            <tr>
                <td style="text-align:right;">
                
                <asp:Label ID="lblModifTopicDescr" runat="server" Text="Description :"></asp:Label>
                
                </td>
                <td>
                <div style="padding:5px 0px 5px 0px;">
                
                 <asp:TextBox ID="tbModifTopicDescription" runat="server" style="width:580px; height:200px;" TextMode="MultiLine"></asp:TextBox>
                 
                </div>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;</td>
                <td>
                 <div style="padding:5px 0px 0px 0px;">
                    <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td><td>
                                <input id="btnModifyTopic" runat="server" type="button" value="Submit" class="defaultDecButton" />
                            </td><td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td></tr>
                        </table>
                        
                         <cc1:TransliterateButton ID="btnTransModifyTopic" runat="server" Visible="false" />
                        
                        <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td><td>
                                <input id="btnHideModifyTopicData" runat="server" type="button" value="Cancel" onclick="HideElementWithID('divModifyTopic','true')" class="defaultDecButton" />
                            </td><td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td></tr>
                        </table>
                </div>
                </td>
            </tr>
        </table>
    
    
    </div>
    
    <div id="divReplyToComment" class="pnlPopUpReplyModifyComment roundedCorners5">

    <div class="sectionTextHeader" style="margin:5px 0px 5px 0px;">
        <asp:Label ID="lblReplyToComment" runat="server" Text="Reply to comment"></asp:Label>
    </div>
    <div style="margin-bottom:5px;">
      <asp:Label ID="lblReplyCommRules1" runat="server" Text="Rules1"></asp:Label>
            <asp:HyperLink ID="hlReplyCommRules" style="color:#C02E29;" runat="server">Rules</asp:HyperLink>
         <asp:Label ID="lblReplyCommRules2" runat="server" Text="Rulse2"></asp:Label>
      </div>    
                 <textarea ID="taReplyToComment" class="standardTextBoxes" style="width:580px; height:200px;" name="S1" ></textarea>
                 
                 <div style="padding:10px 0px 0px 0px;">
                    <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td><td>
                                <input id="btnReplyToComment" runat="server" type="button" value="Submit" class="defaultDecButton" onclick="ReplyToTopicComment()" />
                            </td><td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td></tr>
                        </table>
                        
                         <cc1:TransliterateButton ID="btnTransReplyToComment" runat="server" Visible="false" />
                        
                        <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td><td>
                                <input id="btnCloseReplyToComment" runat="server" type="button" value="Cancel" onclick="HideElementWithID('divReplyToComment','true')" class="defaultDecButton" />
                            </td><td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td></tr>
                        </table>
                </div>

    </div>
    
    <div id="divModifyComment" class="pnlPopUpReplyModifyComment roundedCorners5">

    <div class="sectionTextHeader" style="margin:5px 0px 10px 0px;">
        <asp:Label ID="lblModifyComment" runat="server" Text="Modify comment"></asp:Label>
    </div>

                 <textarea ID="taModifCommDescr" class="standardTextBoxes" style="width:580px; height:200px;" name="S1" ></textarea>
                 
                 <div style="padding:10px 0px 0px 0px;">
                    <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td><td>
                                <input id="btnModifyComment" runat="server" type="button" value="Submit" class="defaultDecButton" onclick="ModifyComment()" />
                            </td><td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td></tr>
                        </table>
                        
                        <cc1:TransliterateButton ID="btnTransModifyComment" runat="server" Visible="false" />
                        
                        <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td><td>
                                <input id="btnCloseModifComm" runat="server" type="button" value="Cancel" onclick="HideElementWithID('divModifyComment','true')" class="defaultDecButton" />
                            </td><td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td></tr>
                        </table>
                </div>

    </div>
    
    
    <div id="divReplyToTopic" class="pnlPopUpReplyModifyComment roundedCorners5">
    
    <div class="sectionTextHeader" style="margin:5px 0px 5px 0px;">
    
        <asp:Label ID="lblReplyToTopic" runat="server" Text="Reply to topic"></asp:Label>
    
    </div>

            <div style="margin-bottom:5px;">
                <asp:Label ID="lblCommentRules1" runat="server" Text="Rules1"></asp:Label>
            <asp:HyperLink ID="hlCommentRules" style="color:#C02E29;" runat="server">Rules</asp:HyperLink>
         <asp:Label ID="lblCommentRules2" runat="server" Text="Rulse2"></asp:Label>
               </div>


                 <textarea ID="taReplyDescription" class="standardTextBoxes" style="width:580px; height:200px;" name="S1" ></textarea>
                 
              
                 <div style="padding:10px 0px 0px 0px;">
                    <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td><td>
                                <input id="btnAddReply" runat="server" type="button" value="Submit" class="defaultDecButton" />
                            </td><td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td></tr>
                        </table>
                        
                        <cc1:TransliterateButton ID="btnTransReplyToTopic" runat="server" Visible="false" />
                        
                        <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td><td>
                                <input id="btnHideReplyData" runat="server" type="button" value="Cancel" onclick="HideElementWithID('divReplyToTopic','true')" class="defaultDecButton" />
                            </td><td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td></tr>
                        </table>
                </div>
               
    
    </div>
    
    
  <asp:Panel ID="pnlAction" runat="server" Width="330px" CssClass="pnlPopUpRatingStyle roundedCorners5"></asp:Panel>
  <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle roundedCorners5"></asp:Panel>
    
    <asp:Panel ID="pnlSendReport" runat="server" CssClass="pnlPopUpReport roundedCorners5">
    <div class="sectionTextHeader" style="padding:5px 0px 5px 0px;">
        <asp:Label ID="lblReportInfo"  runat="server" Text="Report irregularity" ForeColor="#C02E29"></asp:Label>
          </div>
              <table style="width:100%;">
                  <tr>
                      <td style="width: 10; padding-right: 15px;" valign="top">
                          <textarea ID="taReportText" class="standardTextBoxes" style="width:350px; height:220px;" cols="20" rows="7" name="S1" ></textarea></td>
                      <td valign="middle">
                          <asp:Label ID="lblReporting" runat="server" Text=""></asp:Label>
                      </td>
                  </tr>
                  <tr>
                      <td style="padding:10px 0px 0px 0px;">
                         
                           <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td><td>
                                <input id="btnSendReport" runat="server" type="button" value="Report" onclick="SendReport();" class="defaultDecButton" />
                            </td><td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td></tr>
                        </table>
                        
                        <table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>
                                <img alt="" src="images/SiteImages/btnBGRLeft.png" />
                            </td><td>
                                <input id="btnHideRepData" runat="server" type="button" value="Cancel" onclick="HideReportData();" class="defaultDecButton" />
                            </td><td>
                                <img alt="" src="images/SiteImages/btnBGRRight.png" />
                            </td></tr>
                        </table>
                      </td>
                      <td>
                          &nbsp;</td>
                  </tr>
              </table>
    
    </asp:Panel>
    
</asp:Content>
