// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

//////////////////// RATING COMMENT SCRIPT ///////////////////////////////////////

var ratingPNL;
var btn;
var ratLbl;
var rates;

function ShowRatingData(ratingLBL, numRatings, strRating, currBtnId, pnlId, commID) {

    if (ratingPNL == null && btn == null && ratLbl == null && rates == null &&
    ddlActions == null && actionPnl == null && msgPnl == null) {

        ratingPNL = document.getElementById(pnlId);
        btn = document.getElementById(currBtnId);
        ratLbl = document.getElementById(ratingLBL);

        rates = parseInt(numRatings);

        if (ratingPNL != null && btn != null) {

            ratingPNL.innerHTML = "";

            GetRatingPanelData(ratingLBL, strRating, commID);
        }
    }
}

function GetRatingPanelData(ratingLBL, strRating, commID) {

    PageMethods.WMRateComment(commID, strRating, successRatingData, timeOut, error);

    var yPos = getY(btn);
    var xPos = getX(btn);

    ratingPNL.style.position = "absolute";
    if (btn.offsetHeight) {
        yPos += (btn.offsetHeight + 5);
    }
    else {
        yPos += 25;
    }

    if (btn.offsetWidth) {
        xPos += btn.offsetWidth;
    }

    // "px" needed for Firefox and Chrome
    ratingPNL.style.top = yPos + "px";

    var mouseX = tempX;
    var mooveL = moovePnlToLeft(ratingPNL);
    ratingPNL.style.left = mouseX - mooveL + "px";
}

function successRatingData(str) {
    if (str != null && str.length > 0) {
        if (ratingPNL != null) {

            if (str.toString().indexOf("<span style=\"visibility:hidden;\"></span>") > 0) {
                rates = rates + 1;
                ratLbl.innerHTML = "(" + rates + ")";
            }

            ratingPNL.innerHTML = str;
            ratingPNL.style.visibility = "visible";

            setTimeout(function () { HideRatingData() }, 2000);
        }
        else {
            HideRatingData()
        }
    }
    else {
        HideRatingData()
    }
}

function HideRatingData() {
    if (ratingPNL != null) {
        ratingPNL.style.visibility = "hidden";
    }

    HideElement(ratingPNL, true);

    ratingPNL = null;
    btn = null;
    ratLbl = null;
    rates = null;
}

//////////////////////////Action List//////////////////////////////////////

var ddlActions;
var ddlText;
var commID;
var productActionPnl; /* for products action ddl ONLY*/
var msgPnl;
var username;
var replyPnl;
var msgSpan;
var replyToSpan;
var editPnl;


var actionPnl;

function ShowActionData(actionList, strCommID, strActionPanelId, strMsgPanelId, strUsername, strReplyPnl, strEditPnl) {

    if (ddlActions == null && productActionPnl == null && msgPnl == null && editPnl == null
    && ratingPNL == null && btn == null && ratLbl == null && rates == null && replyPnl == null) {

        if (actionList != null && strCommID != null && strActionPanelId != null
         && strMsgPanelId != null && strReplyPnl != null && strEditPnl != null) {

            productActionPnl = document.getElementById(strActionPanelId);
            ddlActions = document.getElementById(actionList);
            msgPnl = document.getElementById(strMsgPanelId);
            msgSpan = document.getElementById("spanMsgTo");
            replyPnl = document.getElementById(strReplyPnl);
            replyToSpan = document.getElementById("spanReplyToComm");
            editPnl = document.getElementById(strEditPnl);

            username = strUsername;

            if (ddlActions != null && productActionPnl != null && msgPnl != null && msgSpan != null
            && replyPnl != null && replyToSpan != null && editPnl != null) {

                commID = strCommID;
                ddlText = ddlActions.options[ddlActions.selectedIndex].value;

                ShowActionDataPanel();
            }
        }
    }
}

function ShowActionDataPanel() {

    if (ddlActions != null && ddlText != null && commID != null && editPnl != null
    && msgPnl != null && msgSpan != null && replyPnl != null && replyToSpan != null) {
        switch (ddlText) {
            case ("1"): //Reply
                ShowReplyToComm();
                break;
            case ("2"): //Message
                ShowMsgToUserPnl();
                break;
            case ("3"): //Spam
                SetMsgAsSpam();
                break;
            case ("4"):   //Edit
                ShowEditComm()
                break;
            case ("5"): //Delete
                DeleteComment("true");
                break;
            case ("6"): //Delete without sending warning
                DeleteComment("false");
                break;
            default:
                HideActionData();
                break;
        }
    }
}


//////////////// reply to comment ////////////

function ShowReplyToComm() {
    if (commID != null && msgPnl != null && msgSpan != null && ddlActions != null
    && username != null && replyPnl != null && editPnl != null) {

        var yPos = getY(ddlActions);
        var xPos = getX(ddlActions);

        replyToSpan.innerHTML = username;

        replyPnl.style.position = "absolute";

        if (ddlActions.offsetHeight) {
            yPos += (ddlActions.offsetHeight + 5);
        }
        else {
            yPos += 25;
        }

        if (ddlActions.offsetWidth) {
            xPos += ddlActions.offsetWidth;
        }

        // "px" needed for Firefox and Chrome
        replyPnl.style.top = yPos + "px";

        var mouseX = tempX;
        var mooveL = moovePnlToLeft(replyPnl);
        replyPnl.style.left = mouseX - mooveL + "px";

        productActionPnl.style.top = yPos + "px";

        productActionPnl.style.left = xPos - 350 + "px";

        productActionPnl.style.position = "absolute";

        replyPnl.style.visibility = "visible";
    }

}

function ReplyToComment() {

    if (commID != null && msgPnl != null && replyToSpan != null && ddlActions != null
    && username != null && replyPnl != null && editPnl != null) {

        var messageBox = document.getElementById("tbReplyToUser");

        if (messageBox == null) {
            HideActionData();
        }
        else {
            PageMethods.WMReplyToComment(commID, username, messageBox.value, successReplyToComment, timeOut, error)
        }
    }
    else {
        HideActionData();
    }

}

function successReplyToComment(str) {
    if (str != null && str.length > 0) {
        if (productActionPnl != null && msgPnl != null) {

            productActionPnl.innerHTML = str;
            productActionPnl.style.visibility = "visible";

            replyPnl.style.visibility = "hidden"

            var messageBox = document.getElementById("tbReplyToUser");

            if (messageBox != null) {
                messageBox.value = "";
            }

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function () { window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function () { HideActionData() }, 3000);
            }
        }
    }
    else {
        HideActionData();
    }
}

////////////////////// SEND MESSAGE /////////////////////

function ShowMsgToUserPnl() {

    if (commID != null && msgPnl != null && msgSpan != null && ddlActions != null
    && username != null && replyPnl != null && editPnl != null) {

        var yPos = getY(ddlActions);
        var xPos = getX(ddlActions);

        msgSpan.innerHTML = username;

        msgPnl.style.position = "absolute";

        if (ddlActions.offsetHeight) {
            yPos += (ddlActions.offsetHeight + 5);
        }
        else {
            yPos += 25;
        }

        if (ddlActions.offsetWidth) {
            xPos += ddlActions.offsetWidth;
        }

        // "px" needed for Firefox and Chrome
        msgPnl.style.top = yPos + "px";
        msgPnl.style.left = xPos - 350 + "px";

        productActionPnl.style.top = yPos + "px";
        productActionPnl.style.left = xPos - 350 + "px";
        productActionPnl.style.position = "absolute";

        msgPnl.style.visibility = "visible";
    }
}

function SendMsgToUser() {
    if (commID != null && msgPnl != null && msgSpan != null && ddlActions != null
    && username != null && replyPnl != null && editPnl != null) {

        var checkBox = document.getElementById("cbSaveMsgInSent");
        var messageBox = document.getElementById("tbMsgToUser");
        var subjectBox = document.getElementById("tbMsgSubject");


        if (checkBox == null && messageBox == null && subjectBox == null) {
            HideActionData();
        }
        else {
            PageMethods.WMSendMsgToUser(commID, username, messageBox.value, subjectBox.value, checkBox.checked
            , successSendMessage, timeOut, error)
        }


    }
    else {
        HideActionData();
    }
}

function successSendMessage(str) {
    if (str != null && str.length > 0) {
        if (productActionPnl != null && msgPnl != null) {

            productActionPnl.innerHTML = str;
            productActionPnl.style.visibility = "visible";

            msgPnl.style.visibility = "hidden"

            var checkBox = document.getElementById("cbSaveMsgInSent");
            var messageBox = document.getElementById("tbMsgToUser");
            var subjectBox = document.getElementById("tbMsgSubject");

            if (checkBox != null && messageBox != null && subjectBox != null) {

                checkBox.checked = 0;
                messageBox.value = "";
                subjectBox.value = "";
            }

            setTimeout(function () { HideActionData() }, 3000);
        }
    }
    else {
        HideActionData();
    }
}

/////////////////////// SET MSG AS SPAM /////////////////

function SetMsgAsSpam() {
    if (commID != null && msgPnl != null && msgSpan != null && ddlActions != null
    && username != null && replyPnl != null && editPnl != null) {

        PageMethods.WMSetMsgAsSpam(commID, successSpamMsg, timeOut, error);

        var yPos = getY(ddlActions);
        var xPos = getX(ddlActions);

        productActionPnl.style.position = "absolute";
        if (ddlActions.offsetHeight) {
            yPos += (ddlActions.offsetHeight + 5);
        }
        else {
            yPos += 25;
        }

        if (ddlActions.offsetWidth) {
            xPos += ddlActions.offsetWidth;
        }

        // "px" needed for Firefox and Chrome
        productActionPnl.style.top = yPos + "px";

        var mouseX = tempX;
        var mooveL = moovePnlToLeft(productActionPnl);
        productActionPnl.style.left = mouseX - mooveL + "px";

    } else {
        HideActionData();
    }
}

function successSpamMsg(str) {
    if (str != null && str.length > 0) {
        if (productActionPnl != null) {

            productActionPnl.innerHTML = str;
            productActionPnl.style.visibility = "visible";

            setTimeout(function () { HideActionData() }, 3000);
        }
    }
    else {
        HideActionData()
    }
}

/////////////////////////// DELETE COMMENT /////////////////////////

function DeleteComment(blSendWarning) {
    if (commID != null && msgPnl != null && msgSpan != null && ddlActions != null
    && username != null && replyPnl != null && editPnl != null) {

        PageMethods.WMDeleteComment(commID, blSendWarning, successDelComm, timeOut, error);

        var yPos = getY(ddlActions);
        var xPos = getX(ddlActions);

        productActionPnl.style.position = "absolute";
        if (ddlActions.offsetHeight) {
            yPos += (ddlActions.offsetHeight + 5);
        }
        else {
            yPos += 25;
        }

        if (ddlActions.offsetWidth) {
            xPos += ddlActions.offsetWidth;
        }

        // "px" needed for Firefox and Chrome
        productActionPnl.style.top = yPos + "px";

        var mouseX = tempX;
        var mooveL = moovePnlToLeft(productActionPnl);
        productActionPnl.style.left = mouseX - mooveL + "px";
    } else {
        HideActionData();
    }
}

function successDelComm(str) {
    if (str != null && str.length > 0) {
        if (productActionPnl != null) {

            productActionPnl.innerHTML = str;
            productActionPnl.style.visibility = "visible";

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function () { window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function () { HideActionData() }, 3000);
            }
        }
    }
    else {
        HideActionData()
    }
}


//////////////// EDIT COMMENT ////////////

function ShowEditComm() {
    if (commID != null && msgPnl != null && msgSpan != null && ddlActions != null
    && username != null && replyPnl != null && editPnl != null) {

        var yPos = getY(ddlActions);
        var xPos = getX(ddlActions);

        spanEditComm.innerHTML = username;

        editPnl.style.position = "absolute";

        if (ddlActions.offsetHeight) {
            yPos += (ddlActions.offsetHeight + 5);
        }
        else {
            yPos += 25;
        }

        if (ddlActions.offsetWidth) {
            xPos += ddlActions.offsetWidth;
        }

        // "px" needed for Firefox and Chrome
        editPnl.style.top = yPos + "px";
        editPnl.style.left = xPos - 350 + "px";

        productActionPnl.style.top = yPos + "px";
        productActionPnl.style.left = xPos - 350 + "px";
        productActionPnl.style.position = "absolute";

        PageMethods.WMGetCommentDescription(commID, successGetCommentDescr, timeOut, error)
    }

}

function successGetCommentDescr(str) {
    if (str != null && str.length > 0) {

        var messageBox = document.getElementById("tbEditComment");

        if (productActionPnl != null && editPnl != null && messageBox != null) {

            messageBox.innerHTML = str;

            editPnl.style.visibility = "visible";
        }
    }
    else {
        HideActionData();
    }
}

function EditComment() {

    if (commID != null && msgPnl != null && replyToSpan != null && ddlActions != null
    && username != null && replyPnl != null && editPnl != null) {

        var messageBox = document.getElementById("tbEditComment");

        if (messageBox == null) {
            HideActionData();
        }
        else {
            PageMethods.WMEditComment(commID, username, messageBox.value, successEditComment, timeOut, error)
        }
    }
    else {
        HideActionData();
    }

}

function successEditComment(str) {
    if (str != null && str.length > 0) {
        if (productActionPnl != null && msgPnl != null) {

            productActionPnl.innerHTML = str;
            productActionPnl.style.visibility = "visible";

            editPnl.style.visibility = "hidden"

            var messageBox = document.getElementById("tbEditComment");

            if (messageBox != null) {
                messageBox.value = "";
            }

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function () { window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function () { HideActionData() }, 3000);
            }
        }
    }
    else {
        HideActionData();
    }
}


//////////////////////// hide data /////////////////

function HideActionData() {
    if (productActionPnl != null && msgPnl != null && replyPnl != null && editPnl != null) {

        HideElement(productActionPnl, true);
        HideElement(msgPnl, true);
        HideElement(replyPnl, true);
        HideElement(editPnl, true);
    }


    if (ddlActions != null) {
        ddlActions.selectedIndex = 0;
    }

    productActionPnl = null;
    ddlActions = null;
    ddlText = null;
    commID = null;
    msgPnl = null;
    msgSpan.innerHTML = "";
    username = null;
    replyPnl = null;
    editPnl = null;
}

////////////////////////////////////////////////////////////////////////////

/////////////////////////// SHOW PRODUCT LINKS

function ShowProductLinks(strCallingBtn, strPnlPopUp, strOffSetX, strOffSetY) {

    if (strCallingBtn != null && strPnlPopUp != null && strOffSetX != null && strOffSetY != null) {

        var callingBtn = document.getElementById(strCallingBtn);
        var linksPnl = document.getElementById(strPnlPopUp);

        if (callingBtn == null || linksPnl == null) {
            return;
        }


        linksPnl.style.position = "absolute";

        var yPos = getY(callingBtn);
        var xPos = getX(callingBtn);

        var top = yPos + parseInt(strOffSetY);
        var left = xPos + parseInt(strOffSetX);

        linksPnl.style.top = top + "px";
        linksPnl.style.left = left + "px";
        linksPnl.style.visibility = "visible";

    }
}

////////// add product link //////////

function ShowAddProductLinkData(strCallingBtn, strPnlAction) {

    if (strCallingBtn != null && strPnlAction != null) {

        var callingBtn = document.getElementById(strCallingBtn);
        var addPnl = document.getElementById("divAddProdLink");
        actionPnl = document.getElementById(strPnlAction);  /*drug pnl*/

        if (callingBtn != null && addPnl != null && actionPnl != null) {

            var mouseX = tempX;
            var mooveL = moovePnlToLeft(addPnl);

            var mouseY = tempY;
            var mooveT = moovePnlToTop(addPnl);

            var yPos = getY(callingBtn);

            addPnl.style.position = "absolute";

            if (callingBtn.offsetHeight) {
                yPos += (callingBtn.offsetHeight + 5);
            }
            else {
                yPos += 25;
            }

            // "px" needed for Firefox and Chrome
            addPnl.style.top = yPos - mooveT + "px";
            addPnl.style.left = mouseX - 380 + "px";

            actionPnl.style.top = yPos + "px";
            actionPnl.style.left = mouseX - mooveL + "px";
            actionPnl.style.position = "absolute";

            addPnl.style.visibility = "visible";
        } else {

            ClearActionPnl();

        }
    }
}

function AddProductLink(strProdID, strTbLinkId, strTbDescrId) {

    if (strProdID != null && actionPnl != null && strTbLinkId != null && strTbDescrId != null) {

        var link = document.getElementById(strTbLinkId);
        var description = document.getElementById(strTbDescrId);

        if (description != null && link != null) {

            PageMethods.WMAddProductLink(strProdID, link.value, description.value, successAddProductLink, timeOut, error);
        }
    } else {

        ClearActionPnl();

    }

    HideElementWithID("divAddProdLink", "true");
}

function successAddProductLink(str) {
    if (str != null && str.length > 0) {

        if (actionPnl != null) {

            actionPnl.innerHTML = str;
            actionPnl.style.visibility = "visible";

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function () { HideElement(actionPnl, true); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function () { HideElement(actionPnl, true); ClearActionPnl(); }, 3000);
            }
        }
    } else {

        ClearActionPnl();

    }
}

function ClearActionPnl() {
    actionPnl = null;
}

///////////////////////////// modify product link //////////////////////

var ProductLinkID = 0;

function ShowModifyLinkData(strCallingBtn, strPnlAction, strLinkID, strDescription) {

    if (strCallingBtn != null && strPnlAction != null && strLinkID != null && strDescription != null) {

        var callingBtn = document.getElementById(strCallingBtn);
        var modifyPnl = document.getElementById("divModifyProdLink");
        actionPnl = document.getElementById(strPnlAction);
        ProductLinkID = strLinkID;

        var description = document.getElementById("taModifProdLinkDescr");

        if (callingBtn != null && modifyPnl != null && actionPnl != null && description != null) {

            description.value = strDescription;

            var mouseX = tempX;
            var mooveL = moovePnlToLeft(modifyPnl);

            var mouseY = tempY;
            var mooveT = moovePnlToTop(modifyPnl);

            var yPos = getY(callingBtn);

            modifyPnl.style.position = "absolute";

            if (callingBtn.offsetHeight) {
                yPos += (callingBtn.offsetHeight + 5);
            }
            else {
                yPos += 25;
            }

            // "px" needed for Firefox and Chrome
            modifyPnl.style.top = yPos - mooveT + "px";
            modifyPnl.style.left = mouseX - mooveL + "px";

            actionPnl.style.top = yPos + "px";
            actionPnl.style.left = mouseX - mooveL + "px";
            actionPnl.style.position = "absolute";

            modifyPnl.style.visibility = "visible";
        } else {

            ClearActionPnl();

        }

    }


}

function ModifyProductLink() {

    if (actionPnl != null && ProductLinkID > 0) {

        var description = document.getElementById("taModifProdLinkDescr");

        if (description != null) {

            PageMethods.WMChangeProductLinkDescr(ProductLinkID, description.value, successChangeProductLinkDescr, timeOut, error);
        }
    } else {
        ClearActionPnl();
    }

    ProductLinkID = 0;
    HideElementWithID("divModifyProdLink", "true");
}

function successChangeProductLinkDescr(str) {
    if (str != null && str.length > 0) {

        if (actionPnl != null) {

            actionPnl.innerHTML = str;
            actionPnl.style.visibility = "visible";

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function () { HideElement(actionPnl, true); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function () { HideElement(actionPnl, true); ClearActionPnl(); }, 3000);
            }
        }
    } else {

        ClearActionPnl();
    }
}

function DeleteProductLink() {

    if (actionPnl != null && ProductLinkID > 0) {

        PageMethods.WMDeleteProductLink(ProductLinkID, successDeleteProductLink, timeOut, error);

    } else {
        ClearActionPnl();
    }

    ProductLinkID = 0;
    HideElementWithID("divModifyProdLink", "true");
}

function DeleteProductLinkW() {

    if (actionPnl != null && ProductLinkID > 0) {

        PageMethods.WMDeleteProductLinkW(ProductLinkID, successDeleteProductLink, timeOut, error);

    } else {
        ClearActionPnl();
    }

    ProductLinkID = 0;
    HideElementWithID("divModifyProdLink", "true");
}

function successDeleteProductLink(str) {
    if (str != null && str.length > 0) {

        if (actionPnl != null) {

            actionPnl.innerHTML = str;
            actionPnl.style.visibility = "visible";

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function () { HideElement(actionPnl, true); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function () { HideElement(actionPnl, true); ClearActionPnl(); }, 3000);
            }
        }
    } else {

        ClearActionPnl();
    }
}


///////////////////////////////// MARK product link AS violation /////////////////////

function ReportProductLinkAsSpam(strPnlAction, strCallingBtn, strLinkId) {

    if (strPnlAction != null && strCallingBtn != null && strLinkId != null) {

        var callingBtn = document.getElementById(strCallingBtn);
        actionPnl = document.getElementById(strPnlAction);

        if (callingBtn != null && actionPnl != null) {

            var mouseX = tempX;
            var mooveL = moovePnlToLeft(actionPnl);

            var yPos = getY(callingBtn);
            var xPos = getX(callingBtn);


            if (callingBtn.offsetHeight) {
                yPos += (callingBtn.offsetHeight + 5);
            }
            else {
                yPos += 25;
            }

            if (callingBtn.offsetWidth) {
                xPos += callingBtn.offsetWidth;
            }

            actionPnl.style.top = yPos + "px";
            actionPnl.style.left = mouseX - mooveL + "px";
            actionPnl.style.position = "absolute";

            PageMethods.WMSetLinkAsViolation(strLinkId, successReportLinkAsViolation, timeOut, error);

        }

    }
}


function successReportLinkAsViolation(str) {
    if (str != null && str.length > 0) {

        if (actionPnl != null) {

            actionPnl.innerHTML = str;
            actionPnl.style.visibility = "visible";

            setTimeout(function () { HideElement(actionPnl, true); }, 3000);
        }
    }
}
