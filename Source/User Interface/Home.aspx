<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="UserInterface.Home"
    MasterPageFile="~/MasterPage.Master" Theme="MainTheme" %>

<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc2" %>

<%@ Register Assembly="CustomServerControls" Namespace="CustomServerControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" runat="server"
    ContentPlaceHolderID="ContentPlaceHolder1">

    <table style="width: 100%;">
        <tr>
            <td style="padding-right: 11px;"
                valign="top">


                <div>
                    <div class="brb2">
                        <div class="blb2">
                            <div class="blhr">
                                <div class="contentBoxBottomHr">





                                    <asp:Panel ID="pnlChooseLang" runat="server" Visible="False">
                                        <br />
                                        <div style="text-align: center;">

                                            <asp:Label ID="lblLangInfoEng" runat="server" Text="Language info"
                                                Font-Size="Larger" ForeColor="#CC3300" Font-Bold="True"></asp:Label>
                                            <br />
                                            <asp:Label ID="lblLangInfoBgr" runat="server" Text="Информация за езика"
                                                Font-Size="Larger" ForeColor="#CC3300" Font-Bold="True"></asp:Label>
                                        </div>
                                        <br />
                                        <table style="width: 100%">
                                            <tr>
                                                <td style="border-right: solid 1px Silver; text-align: right; padding-right: 10px; width: 50%;">
                                                    <asp:Label ID="lblClickForEng" runat="server" CssClass="lblEditors"
                                                        Text="English"></asp:Label>
                                                    <br />
                                                    <asp:Label ID="lblClickForBulg" runat="server" CssClass="lblEditors"
                                                        Text="Bulgarian"></asp:Label>
                                                </td>
                                                <td style="width: 1px;">

                                                    <asp:Image ID="imgEnFlag" runat="server" Height="16px" Style="margin-bottom: 4px; cursor: pointer;"
                                                        ImageUrl="~/images/SiteImages/British.bmp" />

                                                    <br />
                                                    <asp:Image ID="imgBgFlag" runat="server" Height="16px" Style="cursor: pointer;"
                                                        ImageUrl="~/images/SiteImages/Bulgaria.jpg" />
                                                </td>
                                                <td style="padding-left: 10px; border-left: solid 1px Silver;">
                                                    <asp:Label ID="lblClickForEng1" runat="server" CssClass="lblEditors"
                                                        Text="Английски"></asp:Label>
                                                    <br />
                                                    <asp:Label ID="lblClickForBulg1" runat="server" CssClass="lblEditors"
                                                        Text="Bulgarian"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                        <hr />
                                    </asp:Panel>

                                    <div style="padding-top: 8px;">
                                        <asp:Panel ID="pnlAboutSiteHeader" runat="server" CssClass="sectionTextHeader" Style="margin-bottom: 2px;">
                                            <asp:Label ID="lblAboutSiteHeader" runat="server"
                                                Text="About site header" Font-Size="22px"></asp:Label>
                                        </asp:Panel>

                                        <asp:Label ID="lblAboutSite" runat="server" Text="About Site"></asp:Label>

                                    </div>






                                </div>

                                <img src="images/SiteImages/horL.png" align="left" />
                                <img src="images/SiteImages/horR.png" align="right" />

                            </div>
                        </div>
                    </div>
                </div>








                <div class="panelNearSideElements">

                    <ajaxToolkit:Accordion ID="aNewComers" runat="server" FramesPerSecond="40" TransitionDuration="1"
                        RequireOpenedPane="False" SelectedIndex="-1" SuppressHeaderPostbacks="True">
                        <Panes>

                            <ajaxToolkit:AccordionPane ID="apSuggestions" runat="server">
                                <Header>
                                    <asp:Panel ID="pnlShowForNewComersHeader" runat="server" CssClass="accordionHeaders12" BackColor="#62A556">

                                        <asp:Label ID="lblForNewcomers" runat="server" ForeColor="White" Text="                     Вашата бърза обиколка в сайта започва - показваме ви образно най-важните елементи,
                      с които ще работите и чието съдържание зависи изцяло от вас ... Възполвайте се!"></asp:Label>

                                        <asp:Image ID="imgHand" ImageUrl="images/SiteImages/hand.png" ImageAlign="AbsBottom" runat="server" Height="24px" Width="21px" />

                                    </asp:Panel>
                                </Header>

                                <Content>



                                    <asp:Panel ID="pnlForNewcomersContent" runat="server" CssClass="newComersPnl">
                                        <div style="text-align: center;">
                                        </div>
                                        <br />
                                        <div style="text-align: center;">
                                            <span class="darkOrange">>> </span>
                                            <span style="font-size: 19px; font-family: 'Times New Roman'; color: #003399;">
                                                <asp:Label ID="lblNcProducts" runat="server" Text="Продукти"></asp:Label>
                                            </span>
                                            <span class="darkOrange"><< </span>
                                        </div>

                                        <div style="padding: 0px 10px 0px 10px; margin-top: 5px;">

                                            <asp:Table ID="tblNcProducts" runat="server" CssClass="ncBgrBlue">
                                            </asp:Table>

                                            <div class="NCblueText">

                                                <asp:Label ID="lblNcProductsText" runat="server" Text=" Напук на кризата ги ползваме всеки ден - затова качеството им
е вашият фокус. Добавяйте на воля ..."></asp:Label>


                                            </div>

                                        </div>
                                        <br />
                                        <div style="text-align: center;">
                                            <span class="darkOrange" style="color: #0B83FB;">>> </span>
                                            <span style="font-size: 19px; font-family: 'Times New Roman'; color: #003399;">
                                                <asp:Label ID="lblNcCompanies" runat="server" Text="Компании"></asp:Label>
                                            </span>
                                            <span class="darkOrange" style="color: #0B83FB;"><< </span>
                                        </div>

                                        <div style="padding: 0px 10px 0px 10px; margin-top: 5px;">

                                            <asp:Table ID="tblNcCompanies" runat="server" CssClass="ncBgrOrange">
                                            </asp:Table>


                                            <div class="NCorangeText">
                                                <asp:Label ID="lblNcCompaniesText" runat="server" Text="Идеята е да създадем каталог и да разберем какво се крие зад 
рекламите на компаниите чрез вашата оценка на продуктите им."></asp:Label>


                                            </div>
                                        </div>
                                        <br />
                                        <div style="text-align: center;">
                                            <span class="darkOrange">>> </span>
                                            <span style="font-size: 19px; font-family: 'Times New Roman'; color: #003399;">
                                                <asp:Label ID="lblNcOpinions" runat="server" Text="Мнения"></asp:Label>
                                            </span>
                                            <span class="darkOrange"><< </span>
                                        </div>
                                        <div style="padding: 0px 10px 0px 10px; margin-top: 5px;">

                                            <asp:Table ID="tblNcOpinions" runat="server" CssClass="ncBgrGreen">
                                            </asp:Table>

                                            <div class="NCGreenText">

                                                <asp:Label ID="lblNcOpinionsText" runat="server" Text="Всъщност вашите мнения са истинската реклама - без 'грим 
, промоции и фалшиви гаранции'. Всички се нуждаем от това."></asp:Label>

                                            </div>
                                        </div>
                                        <br />

                                        <div style="text-align: center;">
                                            <span class="darkOrange" style="color: #0B83FB;">>> </span>
                                            <span style="font-size: 19px; font-family: 'Times New Roman'; color: #003399;">
                                                <asp:Label ID="lblNcCategories" runat="server" Text="Категории"></asp:Label>
                                            </span>
                                            <span class="darkOrange" style="color: #0B83FB;"><< </span>
                                        </div>

                                        <div style="padding: 0px 10px 0px 10px; margin-top: 5px;">

                                            <asp:Table ID="tblNcCategories" runat="server" CssClass="ncBgrOrange">
                                            </asp:Table>

                                            <div class="NCorangeText">
                                                <asp:Label ID="lblNcCategoriesText" runat="server" Text="Определят някои от основните области на материалното ни битие, 
в които може да добавяте продукти. "></asp:Label>

                                            </div>
                                        </div>
                                        <br />

                                        <div style="text-align: center;">
                                            <span class="darkOrange">>> </span>
                                            <span style="font-size: 19px; font-family: 'Times New Roman'; color: #003399;">
                                                <asp:Label ID="lblNcOthers" runat="server" Text="Други възможности"></asp:Label>
                                            </span>
                                            <span class="darkOrange"><< </span>
                                        </div>

                                        <div style="padding: 0px 10px 0px 10px; margin-top: 5px;">
                                            <div style="border: solid 2px #ACDAF2; padding: 2px 5px 2px 30px; background-color: #C4E5F6;">


                                                <ul>
                                                    <li>

                                                        <asp:Label ID="lblNcOthersZero" CssClass="searchPageRatings" runat="server" Text="Обсъждане на всичко за всеки продукт в неговия форум."></asp:Label>

                                                    </li>
                                                    <li>

                                                        <asp:Label ID="lblNcOthersOne" runat="server" Text="Получаване на известия за нова информация за продукти 
                    и компании."></asp:Label>


                                                    </li>

                                                    <li>

                                                        <asp:Label ID="lblNcOthersTwo" runat="server" Text="Управление на правата за"></asp:Label>
                                                        <asp:HyperLink ID="hlNcOthersTwo" runat="server" Target="_blank">редактиране</asp:HyperLink>
                                                        <asp:Label ID="lblNcOthersTwo2" runat="server" Text="на компании и продукти."></asp:Label>

                                                    </li>

                                                    <li>

                                                        <asp:Label ID="lblNcOthersThree" runat="server" Text="Обмяна на лични съобщения между потребителите."></asp:Label>

                                                    </li>

                                                    <li>

                                                        <asp:Label ID="lblNcOthersFour" runat="server" Text="Опция за"></asp:Label>
                                                        <asp:HyperLink ID="hlNcOthersFour" runat="server" Target="_blank">предложения</asp:HyperLink>
                                                        <asp:Label ID="lblNcOthersFour2" runat="server" Text="към редакторите на продукти и компании."></asp:Label>

                                                    </li>

                                                    <li>
                                                        <asp:Label ID="lblNcOthersFive" runat="server" Text="и много други ... "></asp:Label>

                                                    </li>

                                                </ul>

                                            </div>
                                        </div>
                                        <br />
                                    </asp:Panel>

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


                                                <div class="HomesLastProducts roundedCorners">
                                                    <asp:Panel ID="pnlLastProducts" runat="server" CssClass="">
                                                        <div class="sectionTextHeader" style="margin-bottom: 5px;">
                                                            <asp:Label ID="lblLastProducts" runat="server"
                                                                Text="Last products"></asp:Label>
                                                        </div>

                                                        <asp:Table ID="tblLastProducts" runat="server" Width="100%">
                                                        </asp:Table>

                                                    </asp:Panel>
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



                                                <div class="HomeMessages roundedCorners">
                                                    <asp:Panel ID="pnlMessages" runat="server"
                                                        CssClass="sectionTextHeader">
                                                        <asp:Label ID="lblSiteMessages" runat="server"
                                                            Text="Messages"></asp:Label>
                                                    </asp:Panel>
                                                    <asp:HyperLink ID="hlAnchorNews" name="news" runat="server"></asp:HyperLink>
                                                    <asp:Table ID="tblPages" runat="server" Style="margin-left: 10px;">
                                                    </asp:Table>
                                                    <asp:PlaceHolder ID="phSiteNews" runat="server"></asp:PlaceHolder>
                                                    <asp:Table ID="tblPagesBtm" runat="server" Style="margin-left: 10px;">
                                                    </asp:Table>

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
                </div>




            </td>
            <td style="font-family: Arial, Helvetica, sans-serif; padding: 5px 0px 0px 10px; width: 240px; font-size: 17px;"
                valign="top">



                <div style="text-align: center; margin-top: 6px; margin-bottom: 4px;">
                    <asp:Label ID="lblStatisics" runat="server" CssClass="sectionTextHeader"
                        Text="Statistics for today "></asp:Label>
                </div>
                <asp:Label ID="lblUsersBrowsing" runat="server" Text="Users Browsing"></asp:Label>
                <br />
                <asp:Label ID="lblCommentsWritten" runat="server" Text="Comments written"></asp:Label>
                <br />
                <asp:Label ID="lblProductsAdded" runat="server" Text="Products Added"></asp:Label>
                <br />
                <asp:Label ID="lblCompaniesAdded" runat="server" Text="Companies Added"></asp:Label>
                <br />
                <asp:Label ID="lblUserRegistered" runat="server" Text="Users Registered"></asp:Label>
                <br />
                <asp:Label ID="lblVisits" runat="server" Text="Visits"></asp:Label>


                <div style="text-align: right; margin: 5px 0px 5px 0px;">
                    <img src="images/SiteImages/rightDoubleEl.png" />
                </div>

                <asp:Table ID="tblLastCompanies" runat="server" Width="100%">
                </asp:Table>

                <div style="text-align: right; margin: 5px 0px 5px 0px;">
                    <img src="images/SiteImages/rightDoubleEl.png" />
                </div>

                <asp:Table ID="tblLastProductsWithComments" runat="server" Width="100%">
                </asp:Table>

                <img src="images/SiteImages/bottomRightEl.png" />

                <br />

            </td>
        </tr>
    </table>


    <asp:Panel ID="pnlPopUp" runat="server" Width="450px" CssClass="pnlPopUpStyle roundedCorners5"></asp:Panel>




</asp:Content>


<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">

    <style type="text/css">
        #tbMsgSubject {
            width: 270px;
        }

        .style1 {
            width: 100%;
            border-collapse: collapse;
        }
    </style>

</asp:Content>



