// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

function toggleElementVisibility(elementId) {
    var newVisibility = "";
    if (elementId) {
        var element = document.getElementById("" + elementId);
        if (element) {
            var elementStyle = element.style;
            if (elementStyle) {
                if (elementStyle.visibility) {
                    if (elementStyle.visibility == 'hidden') {
                        newVisibility = 'visible';
                    } else {
                        newVisibility = 'hidden';
                    }
                } else {
                    newVisibility = 'hidden';
                }
                elementStyle.visibility = newVisibility;
                if (elementStyle.visibility == 'hidden') {
                    elementStyle.height = 0;
                } else {
                    elementStyle.height = null;
                }
            } else {
                alert("The element with ID=\"" + elementId + "\" has no style.");
            }
        } else {
            alert("The element with ID=\"" + elementId + "\"  not found.");
        }
    } else {
        alert("elementId not specified.");
    }
    return newVisibility;
}


/////////////////////////////////////
var tb;

function JSCheckData(tbElementId, type, LblElementId, charId) {
    tb = document.getElementById(LblElementId);
    var textBox = document.getElementById(tbElementId);
    PageMethods.CheckData(textBox.value, type, charId, success, timeOut, error);
}

function success(str) {
    tb.innerHTML = str;
}

function timeOut() {
}

function error() {
}

///////////////////////////////////

var pnl;
var elementID;

function ShowData(type, typeId, currElementId, pnlId) {
    pnl = document.getElementById(pnlId);


    var currElement = document.getElementById(currElementId);
    if (pnl != null && currElement != null) {

        var mooveL = moovePnlToLeft(pnl);
        var mouseX = tempX;

        elementID = currElementId;
        setTimeout(function() { GetPopUpPanelData(type, typeId, currElementId, currElement, mooveL, mouseX) }, 500);
    }
}


function GetPopUpPanelData(type, typeId, currElementId, currElement, mooveL, mouseX) {

    if (elementID == currElementId && currElement != null && pnl != null) {

        PageMethods.WMGetData(type, typeId, successGetData, timeOut, error);

        var yPos = getY(currElement);
        var xPos = getX(currElement);

        pnl.style.position = "absolute";
        if (currElement.offsetHeight) {
            yPos += (currElement.offsetHeight + 1);
        }
        else {
            yPos += 25;
        }

        if (currElement.offsetWidth) {
            xPos += currElement.offsetWidth;
        }

        // "px" needed for Firefox and Chrome
        pnl.style.top = yPos + "px";
        pnl.style.left = mouseX - mooveL + "px";
    }
}


function successGetData(str) {
    if (str != null && str.length > 0) {
        if (pnl != null) {
            pnl.innerHTML = str;
            pnl.style.visibility = "visible";
        }
    }
}

function HideData() {
    if (pnl != null) {
        pnl.style.visibility = "hidden";
        pnl = null;
        elementID = "";
    }
}

function changeTbPassClassOnBlur(password) {
    if (password != null) {
        var textbox = document.getElementById(password);
        if (textbox != null && textbox.type == "password") {
            if (textbox.value.length == 0 && textbox.className != "tbPassTextBgr") {
                textbox.className = "tbPassTextBgr";
            }
        }
    }
}

function changeTbUsrnameClassOnBlur(username) {
    if (username != null) {
        var textbox = document.getElementById(username);
        if (textbox != null && textbox.type == "text") {
            if (textbox.value.length == 0 && textbox.className != "tbUsrnameTextBgr") {
                textbox.className = "tbUsrnameTextBgr";
            }
        }
    }
}

/////////////////UPDATE PANEL PROGRESS ANIMATION////////////////
// http://www.e-webdevelopers.com/132/how-to-create-amp-place-a-progress-image-over-an-updatepanel/
//Show div for update animation extender

function onUpdating(theUpdatePanel) {
    // get the update progress div
    var updateProgressDiv = $get('updateProgressDiv');

    //  get the gridview element
    var updatePanel = $get(theUpdatePanel);

    if (updateProgressDiv != null && updatePanel != null) {
        // make it visible
        updateProgressDiv.style.display = '';
        // get the bounds of both the gridview and the progress div
        var updatePanelBounds = Sys.UI.DomElement.getBounds(updatePanel);
        var updateProgressDivBounds = Sys.UI.DomElement.getBounds(updateProgressDiv);

        //    do the math to figure out where to position the element (the center of the gridview)
        var x = updatePanelBounds.x + Math.round(updatePanelBounds.width / 2) - Math.round(updateProgressDivBounds.width / 2);
        var y = updatePanelBounds.y + Math.round(updatePanelBounds.height / 2) - Math.round(updateProgressDivBounds.height / 2);

        //    set the progress element to this position
        Sys.UI.DomElement.setLocation(updateProgressDiv, x, y);
    }
}

function onUpdated() {
    // get the update progress div
    var updateProgressDiv = $get('updateProgressDiv');
    if (updateProgressDiv != null) {
        // make it invisible
        updateProgressDiv.style.display = 'none';
    }
}

/////////////////////END Animation Extender


function ShowUploadingMsg(lblID, lblError) {

    var label = $get(lblID);

    if (label != null) {
        label.style.display = '';
    }

    var error = $get(lblError);
    if (error != null) {
        error.style.display = 'none';
    }
}


/////////////////////////MODAL POP UP///////////////
function fnClickOK(sender, e) {

    var hf = $get(hfUpdateID);
    if (hf != null) {
        if (hf.value == 'true') {
            window.location = document.location.href; ;
            //__doPostBack(sender, e);
        }
    }
}


/////////////////////////

////////////////// SPAM SUGGESTION ////////////////

var suggActPnl;

function ShowSuggActionData(strActionPanelId, spamLblID, suggID) {

    if (suggActPnl == null && strActionPanelId != null) {

        suggActPnl = document.getElementById(strActionPanelId);
        var suggSpamLbl = document.getElementById(spamLblID);

        if (suggActPnl != null && suggSpamLbl != null && suggID != null) {

            PageMethods.WMSetSuggAsSpam(suggID, successSuggSpamMsg, timeOut, error);

            var yPos = getY(suggSpamLbl);
            var xPos = getX(suggSpamLbl);

            suggActPnl.style.position = "absolute";

            if (suggSpamLbl.offsetHeight) {
                yPos += (suggSpamLbl.offsetHeight + 5);
            }
            else {
                yPos += 25;
            }

            if (suggSpamLbl.offsetWidth) {
                xPos += suggSpamLbl.offsetWidth;
            }

            // "px" needed for Firefox and Chrome
            suggActPnl.style.top = yPos + "px";

            var mooveL = moovePnlToLeft(suggActPnl);
            var mouseX = tempX;
            suggActPnl.style.left = tempX - mooveL + "px";
        } else {
            HideSuggestionData();
        }

    }
}

function successSuggSpamMsg(str) {
    if (str != null && str.length > 0) {
        if (suggActPnl != null) {

            suggActPnl.innerHTML = str;
            suggActPnl.style.visibility = "visible";

            setTimeout(function() { HideSuggestionData() }, 3000);
        }
    }
    else {
        HideSuggestionData();
    }
}

function HideSuggestionData() {
    if (suggActPnl != null) {
        suggActPnl.style.visibility = "hidden";
    }

    suggActPnl = null;
}

////////////////////////////////////////////////////


function ShowOptionsDiv() {

    var optDiv = document.getElementById("divOptions");
    var optLbl = document.getElementById("optionsLbl");

    if (optDiv != null && optLbl != null) {


        var yPos = getY(optLbl);
        var xPos = getX(optLbl);

        optDiv.style.position = "absolute";

        if (optLbl.offsetHeight) {
            yPos += (optLbl.offsetHeight + 5);
        }
        else {
            yPos += 25;
        }

        if (optLbl.offsetWidth) {
            xPos += optLbl.offsetWidth;
        }

        // "px" needed for Firefox and Chrome
        optDiv.style.top = yPos + "px";
        optDiv.style.left = xPos - 370 + "px";

        optDiv.style.visibility = "visible";
    }

}


///////////////////////////// SEND REPORT ///////////////////////////////


var repType;
var repTypeId;
var callBtn;
var repActPnl;
var repPnl;

function ShowReportData(type, strTypeID, strCallBtnId, strActionPanelId, strRepPanelId) {

    if (repType == null && repTypeId == null && callBtn == null && repActPnl == null
    && repPnl == null) {

        if (type != null && strTypeID != null && strCallBtnId != null
         && strActionPanelId != null && strRepPanelId != null) {


            repType = type;
            repTypeId = strTypeID;
            callBtn = document.getElementById(strCallBtnId);
            repActPnl = document.getElementById(strActionPanelId);
            repPnl = document.getElementById(strRepPanelId);

            if (repType != null && repTypeId != null && callBtn != null && repActPnl != null
            && repPnl != null) {


                var mouseX = tempX;
                var mooveL = moovePnlToLeft(repPnl);

                var yPos = getY(callBtn);
                var xPos = getX(callBtn);

                repPnl.style.position = "absolute";

                if (callBtn.offsetHeight) {
                    yPos += (callBtn.offsetHeight + 5);
                }
                else {
                    yPos += 25;
                }

                if (callBtn.offsetWidth) {
                    xPos += callBtn.offsetWidth;
                }

                // "px" needed for Firefox and Chrome
                repPnl.style.top = yPos + "px";
                repPnl.style.left = mouseX - mooveL + "px";

                repActPnl.style.top = yPos + "px";
                repActPnl.style.left = mouseX - mooveL + "px";
                repActPnl.style.position = "absolute";

                repPnl.style.visibility = "visible";

            }
        }
    }
}

function SendReport() {

    if (repType != null && repTypeId != null && callBtn != null && repActPnl != null
            && repPnl != null) {

        var reportBox = document.getElementById("taReportText");

        if (reportBox == null) {
            HideReportData();
        }
        else {
            PageMethods.WMSendReport(repType, repTypeId, reportBox.value, successSendReport, timeOut, error)
        }
    }
    else {
        HideReportData();
    }

}

function successSendReport(str) {
    if (str != null && str.length > 0) {
        if (repActPnl != null && repPnl != null) {

            repActPnl.innerHTML = str;
            repActPnl.style.visibility = "visible";

            repPnl.style.visibility = "hidden"

            var reportBox = document.getElementById("taReportText");

            if (reportBox != null) {
                reportBox.value = "";
            }

            if (str.toString().indexOf("<span id=\"hideBtn\" style=\"visibility:hidden;\"></span>") > 0) {

                callBtn.style.visibility = "hidden"
                setTimeout(function() { HideReportData() }, 3000);
            }
            else {
                setTimeout(function() { HideReportData() }, 3000);
            }
        }
    }
    else {
        HideReportData();
    }
}

function HideReportData() {
    if (repActPnl != null) {
        HideElement(repActPnl, true);
    }

    if (repPnl != null) {
   
        HideElement(repPnl, true);
    }

    repType = null;
    repTypeId = null;
    callBtn = null;
    repActPnl = null;
    repPnl = null;
}


/////////////////////////////// SIGN FOR NOTIFIES//////////////////////////////////////////

var notifyPnl;
var notifyBtn;

function NotifyForUpdates(type, strTypeID, strCallBtnId, strNotifyPnlId) {

    if (notifyPnl == null && notifyBtn == null) {

        if (type != null && strTypeID != null && strCallBtnId != null
         && strNotifyPnlId != null) {

            notifyPnl = document.getElementById(strNotifyPnlId);
            notifyBtn = document.getElementById(strCallBtnId);

            if (notifyPnl != null && notifyBtn != null) {


                var mouseX = tempX;
                var mooveL = moovePnlToLeft(notifyPnl);

                var yPos = getY(notifyBtn);
                var xPos = getX(notifyBtn);

                notifyPnl.style.position = "absolute";

                if (notifyBtn.offsetHeight) {
                    yPos += (notifyBtn.offsetHeight + 5);
                }
                else {
                    yPos += 25;
                }

                if (notifyBtn.offsetWidth) {
                    xPos += notifyBtn.offsetWidth;
                }

                notifyPnl.style.top = yPos + "px";
                notifyPnl.style.left = mouseX - mooveL + "px";

                PageMethods.WMSignForNotifies(type, strTypeID, successSignForNotify, timeOut, error)

            }
        }
    }
}


function successSignForNotify(str) {
    if (str != null && str.length > 0) {
        if (notifyPnl != null) {

            notifyPnl.innerHTML = str;
            notifyPnl.style.visibility = "visible";


            notifyBtn.style.visibility = "hidden"
            setTimeout(function() { HideNotifyData() }, 3000);
        }
    }
    else {
        HideNotifyData();
    }
}

function HideNotifyData() {
    if (notifyPnl != null) {
        notifyPnl.style.visibility = "hidden";
    }

    notifyPnl = null;
    notifyBtn = null;
}

//////////////////////////////////////////////////////////////////////

/////////////////// PATTERNS /////////////////////

var textTextBox;

function SetText(patternID, textType, strWarningBoxId) {

    if (patternID != null && strWarningBoxId != null && textType != null) {

        textTextBox = document.getElementById(strWarningBoxId);

        if (textTextBox != null) {

            PageMethods.WMGetSiteText(patternID, textType, successGetSiteText, timeOut, error)
        }
    }

}

function successGetSiteText(str) {
    if (str != null && str.length > 0) {
        if (textTextBox != null) {

            textTextBox.value = str;
            textTextBox = null;
        }
    }
}

//////////////////PRODUCTS/COMPANIES with similar names////////////////

var pnlSimilarNames;

function GetSimilarNames(pnlID, type, textboxID, pnlPopUpID) {

    if (pnlID != null && type != null && textboxID != null && pnlPopUpID != null) {

        pnlSimilarNames = document.getElementById(pnlID);
        pnlSimilarNames.innerHTML = "";

        var textbox = document.getElementById(textboxID);
        var popUp = document.getElementById(pnlPopUpID);

        if (pnlSimilarNames != null && textbox != null && popUp != null) {

            PageMethods.WMGetSimilarNames(type, textbox.value, pnlPopUpID, successGetSimilarNames, timeOut, error)
        }
    }

}


function successGetSimilarNames(str) {
    if (str != null && str.length > 0) {
        if (pnlSimilarNames != null) {

            pnlSimilarNames.innerHTML = str;
            pnlSimilarNames = null;
        }
    }
}

/////////////////////////////////////////////////

////////////////Transfer role//////////////////

var transferPnl;
var transferEndPnl;
var transferID;

function ShowTransferPnl(lblTransferID, strTransferID, strPnlTransferID, strTransferEndPnlId, roleName, strLblRoleID) {

    if (lblTransferID != null && strTransferID != null && strPnlTransferID != null
    && strTransferEndPnlId != null && roleName != null && strLblRoleID != null) {

        transferID = strTransferID;
        transferEndPnl = document.getElementById(strTransferEndPnlId);
        transferPnl = document.getElementById(strPnlTransferID);
        var label = document.getElementById(lblTransferID);
        var lblRoleName = document.getElementById(strLblRoleID);

        if (label != null && transferPnl != null && transferEndPnl != null && lblRoleName != null) {


            lblRoleName.innerHTML = roleName;

            var mouseX = tempX;
            var mooveL = moovePnlToLeft(transferPnl);

            var yPos = getY(label);

            transferPnl.style.position = "absolute";

            if (label.offsetHeight) {
                yPos += (label.offsetHeight + 5);
            }
            else {
                yPos += 25;
            }

            transferPnl.style.top = yPos + "px";
            transferPnl.style.left = mouseX - mooveL + "px";

            transferPnl.style.visibility = "visible";


            transferEndPnl.style.top = yPos + "px";
            transferEndPnl.style.left = mouseX - mooveL + "px";
            transferEndPnl.style.position = "absolute";

        }
        else {
            hideTransferData();
        }
    }
}

function CreateTransfer() {
    if (transferPnl != null && transferEndPnl != null && transferID != null) {

        var userBox = document.getElementById("tbTransferTo");
        var descriptionBox = document.getElementById("tbTransferDescription");

        if (userBox != null && descriptionBox != null) {
            PageMethods.WMCreateTransfer(transferID, userBox.value, descriptionBox.value,
            successCreateTransfer, timeOut, error)
        }
        else {
            hideTransferData();
        }
    }
    else {
        hideTransferData();
    }
}

function successCreateTransfer(str) {
    if (str != null && str.length > 0) {
        if (transferEndPnl != null && transferPnl != null) {

            transferEndPnl.innerHTML = str;
            transferEndPnl.style.visibility = "visible";

            transferPnl.style.visibility = "hidden"

            var userBox = document.getElementById("tbTransferTo");
            var descriptionBox = document.getElementById("tbTransferDescription");

            if (userBox != null && descriptionBox != null) {

                userBox.value = "";
                descriptionBox.value = "";
            }

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function() { hideTransferData(); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function() { hideTransferData() }, 3000);
            }
        }
    }
    else {
        hideTransferData();
    }
}

function hideTransferData() {

    if (transferPnl != null) {
        transferPnl.style.visibility = "hidden"
    }
    if (transferEndPnl != null) {
        transferEndPnl.style.visibility = "hidden"
    }

    transferPnl = null;
    transferEndPnl = null;
    transferID = null;
}

////////////////////Reply to report////////////////////////

var replyToReportPnl;
var replyToReportEndPnl;
var reportID;

function ShowReplyToReportPnl(btnReplyId, strReportID, strPnlReplyID, strReplyEndPnlId) {

    if (btnReplyId != null && strReportID != null && strPnlReplyID != null
    && strReplyEndPnlId != null) {

        reportID = strReportID;
        replyToReportEndPnl = document.getElementById(strReplyEndPnlId);
        replyToReportPnl = document.getElementById(strPnlReplyID);
        
        var button = document.getElementById(btnReplyId);

        if (btnReplyId != null && replyToReportPnl != null && replyToReportEndPnl != null) {

            var mouseX = tempX;
            var mooveL = moovePnlToLeft(replyToReportPnl);

            var yPos = getY(button);

            replyToReportPnl.style.position = "absolute";

            if (button.offsetHeight) {
                yPos -= (button.offsetHeight + 70);
            }
            else {
                yPos -= 95;
            }

            replyToReportPnl.style.top = yPos + "px";
            replyToReportPnl.style.left = mouseX - mooveL + "px";

            replyToReportPnl.style.visibility = "visible";


            replyToReportEndPnl.style.top = yPos + "px";
            replyToReportEndPnl.style.left = mouseX - mooveL + "px";
            replyToReportEndPnl.style.position = "absolute";

        }
        else {
            replyToReportEndPnl = null;
            replyToReportPnl = null;
            reportID = null;
        }
    }
}

function SendReplyToReport() {
    if (replyToReportPnl != null && replyToReportEndPnl != null && reportID != null) {

        var descriptionBox = document.getElementById("tbReplyDescription");

        if (descriptionBox != null) {
            PageMethods.WMSendReplyToReport(reportID, descriptionBox.value,
            successSendReplyToReport, timeOut, error)
        }
        else {
            hideReplyToReportData()
        }
    }
    else {
        hideReplyToReportData()
    }
}

function successSendReplyToReport(str) {
    if (str != null && str.length > 0) {
        if (replyToReportPnl != null && replyToReportEndPnl != null) {
            
            replyToReportEndPnl.innerHTML = str;
            replyToReportEndPnl.style.visibility = "visible";

            replyToReportPnl.style.visibility = "hidden"

            var descriptionBox = document.getElementById("tbReplyDescription");

            if (descriptionBox != null) {
                descriptionBox.value = "";
            }

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function() { hideReplyToReportData(); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function() { __doPostBack('', ''); }, 3000);
            }
        }
    }
    else {
        hideReplyToReportData();
    }
}

function hideReplyToReportData() {

    if (replyToReportPnl != null) {
        replyToReportPnl.style.visibility = "hidden"
    }
    if (replyToReportEndPnl != null) {
        replyToReportEndPnl.style.visibility = "hidden"
    }

    replyToReportEndPnl = null;
    replyToReportPnl = null;
    reportID = null;
}

/////////////////////////////////////////////////////////////////

var btnSubmitComm;
var ddlSubVariants;

function CommentsTypeData(btnSubmitComm, ddlChars, ddlVariants, ddlSubVars, selectedDdl) {
    if (btnSubmitComm != null && ddlChars != null && ddlVariants != null && ddlSubVars != null && selectedDdl != null) {

        btnSubmitComm = document.getElementById(btnSubmitComm);
        ddlSubVariants = document.getElementById(ddlSubVars);
        var variants = document.getElementById(ddlVariants);
        var chars = document.getElementById(ddlChars);

        btnSubmitComm.style.enabled = "false";

        var selectedValue;

        switch (selectedDdl) {
            case "variants":

                selectedValue = variants.options[variants.selectedIndex].value;

                if (chars != null) {
                    chars.value = 0;
                }

                if (ddlSubVariants != null) {
                    ddlSubVariants.value = 0;
                    ddlSubVariants.style.enabled = "false";
                }

                break;
            case "chars":

                selectedValue = variants.options[variants.selectedIndex].value;

                if (ddlSubVariants != null) {
                    ddlSubVariants.value = 0;
                    ddlSubVariants.style.enabled = "false";
                }

                if (variants != null) {
                    variants.value = 0;
                }

                break;
            case "subvars":


                selectedValue = ddlSubVariants.options[ddlSubVariants.selectedIndex].value;

                if (chars != null) {
                    chars.value = 0;
                }

                break;
            default:
                if (chars != null) {
                    chars.value = 0;
                }
                if (variants != null) {
                    variants.value = 0;
                }
                if (ddlSubVariants != null) {
                    ddlSubVariants.value = 0;
                    ddlSubVariants.style.enabled = "false";
                }

                btnSubmitComm.style.enabled = "true";
                return;
                break;
        }

        PageMethods.WMGetCommentSubVariantData(selectedDdl, selectedValue, successGetCommentSubVariantData, timeOut, error);

    }
}


function successGetCommentSubVariantData(str) {

    ddlSubVariants.disabled = false;

    var opt = document.createElement("option");

    // Add an Option object to Drop Down/List Box
    ddlSubVariants.options.add(opt);        // Assign text and value to Option object
    opt.text = "testJS...";
    opt.value = 6;
}

///////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////Send Suggestion///////////////////////////////////////

var sendSuggestionPnl
var sendSuggestionEndPnl;
var suggType;
var suggTypeId;
var suggDdl;
var suggToUser;

function ShowSendTypeSuggestion(btnSuggestionId, type, strTypeId, strToUserID, strDdlId, strPnlSuggId, strSuggestionEndPnl) {

    if (sendSuggestionPnl == null && sendSuggestionEndPnl == null && suggType == null && suggTypeId == null && suggDdl == null && suggToUser == null) {
    
        if (btnSuggestionId != null && type != null && strTypeId != null && strSuggestionEndPnl != null
            && (strToUserID != null || strDdlId != null) && strPnlSuggId != null) {


            sendSuggestionPnl = document.getElementById(strPnlSuggId);
            sendSuggestionEndPnl = document.getElementById(strSuggestionEndPnl);
            var button = document.getElementById(btnSuggestionId);
            suggType = type;
            suggTypeId = strTypeId;
            suggToUser = strToUserID
            suggDdl = document.getElementById(strDdlId);
            
            if (button != null && sendSuggestionEndPnl != null && sendSuggestionPnl != null) {

                var mouseX = tempX;
                var mooveL = moovePnlToLeft(sendSuggestionPnl);

                var yPos = getY(button);

                sendSuggestionPnl.style.position = "absolute";

                if (button.offsetHeight) {
                    yPos += (button.offsetHeight + 5);
                }
                else {
                    yPos += 25;
                }

                sendSuggestionPnl.style.top = yPos + "px";
                sendSuggestionPnl.style.left = mouseX - mooveL + "px";

                sendSuggestionPnl.style.visibility = "visible";


                sendSuggestionEndPnl.style.top = yPos + "px";
                sendSuggestionEndPnl.style.left = mouseX - mooveL + "px";
                sendSuggestionEndPnl.style.position = "absolute";

            }
            else {
            
                hideSendTypeSuggestionData();
            }
        }
    }
}

function SendTypeSuggestionToUser() {
    if (sendSuggestionEndPnl != null && sendSuggestionPnl != null && (suggToUser != null || suggDdl != null)) {

        var suggUserId;

        if (suggToUser != "empty") {
            suggUserId = suggToUser;
        } else {
            if (suggDdl != null) {

                suggUserId = suggDdl.options[suggDdl.selectedIndex].value;
                if (suggUserId == 0) {
                    hideSendTypeSuggestionData();
                    return;
                }
                
            } else {
                hideSendTypeSuggestionData();
                return;
            }
        }

        var descriptionBox = document.getElementById("tbTypeSuggestion");

        if (descriptionBox != null) {
            PageMethods.WMSendTypeSuggestionToUser(suggUserId, suggType, suggTypeId, descriptionBox.value,
            successSendTypeSuggestion, timeOut, error)
        }
        else {
            hideSendTypeSuggestionData()
        }
    }
    else {
        hideSendTypeSuggestionData()
    }
}

function successSendTypeSuggestion(str) {
    if (str != null && str.length > 0) {
        if (sendSuggestionPnl != null && sendSuggestionEndPnl != null) {

            sendSuggestionEndPnl.innerHTML = str;
            sendSuggestionEndPnl.style.visibility = "visible";

            sendSuggestionPnl.style.visibility = "hidden"

            var descriptionBox = document.getElementById("tbTypeSuggestion");

            if (descriptionBox != null) {
                descriptionBox.value = "";
            }


            setTimeout(function() { hideSendTypeSuggestionData() }, 3000);

        }
    }
    else {
        hideSendTypeSuggestionData();
    }
}

function hideSendTypeSuggestionData() {

    if (sendSuggestionPnl != null) {
        sendSuggestionPnl.style.visibility = "hidden"
    }
    if (sendSuggestionEndPnl != null) {
        sendSuggestionEndPnl.style.visibility = "hidden"
    }

    sendSuggestionEndPnl = null;
    sendSuggestionPnl = null;

    suggType = null;
    suggTypeId = null;

    suggDdl = null;
    suggToUser = null;
}


//////////////////////// ADD COMMENT TO SUGGESTION /////////////

var addCommentToSuggestionPnl
var addCommentToSuggestionEndPnl;
var commentToSuggestionID;

function ShowAddCommentToSuggestion(btnAddComment, strSuggID, strPnlSuggId, strSuggestionEndPnl) {

    if (addCommentToSuggestionPnl == null && addCommentToSuggestionEndPnl == null && commentToSuggestionID == null) {

        if (btnAddComment != null && strSuggID != null && strPnlSuggId != null && strSuggestionEndPnl != null) {

            addCommentToSuggestionPnl = document.getElementById(strPnlSuggId);
            addCommentToSuggestionEndPnl = document.getElementById(strSuggestionEndPnl);
            var button = document.getElementById(btnAddComment);
            commentToSuggestionID = strSuggID;

            if (button != null && addCommentToSuggestionPnl != null && addCommentToSuggestionEndPnl != null) {

                var mouseX = tempX;
                var mooveL = moovePnlToLeft(addCommentToSuggestionPnl);

                var yPos = getY(button);

                addCommentToSuggestionPnl.style.position = "absolute";

                if (button.offsetHeight) {
                    yPos += (button.offsetHeight + 5);
                }
                else {
                    yPos += 25;
                }

                addCommentToSuggestionPnl.style.top = yPos + "px";
                addCommentToSuggestionPnl.style.left = mouseX - mooveL + "px";

                addCommentToSuggestionPnl.style.visibility = "visible";


                addCommentToSuggestionEndPnl.style.top = yPos + "px";
                addCommentToSuggestionEndPnl.style.left = mouseX - mooveL + "px";
                addCommentToSuggestionEndPnl.style.position = "absolute";

            }
            else {

                hideAddCommentToSuggestionData();
            }
        }
    }
}

function AddCommentToSuggestion() {
    if (addCommentToSuggestionPnl != null && addCommentToSuggestionEndPnl != null && commentToSuggestionID != null) {

        var descriptionBox = document.getElementById("tbCommentDescription");

        if (descriptionBox != null) {
            PageMethods.WMAddCommentSuggestionToUser(commentToSuggestionID, descriptionBox.value,
            successAddCommentToSuggestion, timeOut, error)
        }
        else {
            hideAddCommentToSuggestionData()
        }
    }
    else {
        hideAddCommentToSuggestionData()
    }
}

function successAddCommentToSuggestion(str) {
    if (str != null && str.length > 0) {
        if (addCommentToSuggestionPnl != null && addCommentToSuggestionEndPnl != null) {

            addCommentToSuggestionEndPnl.innerHTML = str;
            addCommentToSuggestionEndPnl.style.visibility = "visible";

            addCommentToSuggestionPnl.style.visibility = "hidden"

            var descriptionBox = document.getElementById("tbCommentDescription");

            if (descriptionBox != null) {
                descriptionBox.value = "";
            }

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function() { hideAddCommentToSuggestionData(); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function() { hideAddCommentToSuggestionData() }, 3000);
            }
        }
    }
    else {
        hideAddCommentToSuggestionData();
    }
}

function hideAddCommentToSuggestionData() {

    if (addCommentToSuggestionPnl != null) {
        addCommentToSuggestionPnl.style.visibility = "hidden"
    }
    if (addCommentToSuggestionEndPnl != null) {
        addCommentToSuggestionEndPnl.style.visibility = "hidden"
    }

    addCommentToSuggestionEndPnl = null;
    addCommentToSuggestionPnl = null;
    commentToSuggestionID = null;
}

///////////////////////////////////////

function ShowWantedCategoryPanel(onSelectedCategory, strPanelId, strDdlId) {

    var ddl = document.getElementById(strDdlId);
    var panel = document.getElementById(strPanelId);

    if (ddl != null && panel != null) {

        var selectedValue = ddl.options[ddl.selectedIndex].value;
        if (selectedValue == onSelectedCategory) {

            panel.style.visibility = "visible";
        
        }
        else {

            panel.style.visibility = "hidden";
        
        }
    
    }

}

///////////////////////////////

function ShowAdvancedSearchPanel(pnlAdvSearchID) {

    var panel = document.getElementById(pnlAdvSearchID);

    if (panel != null) {

        if (panel.style.visibility == "visible") {
            panel.style.visibility = "hidden"
        }
        else {
            var mouseX = tempX;
            var mouseY = tempY;
            var mooveL = moovePnlToLeft(panel);

            panel.style.top = mouseY + 8 + "px";
            panel.style.left = mouseX - 90 - mooveL + "px";

            panel.style.visibility = "visible";
        }
    }

}


///////////////////////////////

function ShowCharsCountInField(textboxID, counterID, minCount, maxCount) {

    var tb = document.getElementById(textboxID);
    var counter = document.getElementById(counterID);

    if (tb != null && counter != null && minCount != null && maxCount != null) {
        if (tb.value.length > 0) {
            counter.value = tb.value.length;

            if (tb.value.length < minCount || tb.value.length > maxCount) {
                counter.style.color = '#E60000';
            }
            else {
                counter.style.color = 'Black';
            }
        } else {

            counter.value = 0;

            if (minCount < 1) {
                counter.style.color = 'Black';
            } else {
                counter.style.color = '#E60000';
            }
        }
        
    }

}

/////////////////////////////////

function ChangeLabelText(lblID, text1, text2){

    var lbl = document.getElementById(lblID);
    if (lbl != null) {

        if (lbl.innerHTML == text1) {

            lbl.innerHTML = text2;
        }
        else {

            lbl.innerHTML = text1;
        }
    
    }

}










