// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

///////////////////////// Add topic /////////////////////////////////

function ShowAddTopicData(strActionPnlId, strCallBtnId) {

    if (strActionPnlId != null && strCallBtnId != null) {

        var callingBtn = document.getElementById(strCallBtnId);
        actionPnl = document.getElementById(strActionPnlId);
        var addTopicPnl = document.getElementById("divAddTopic");

        if (callingBtn != null && actionPnl != null && addTopicPnl != null) {


            var mouseX = tempX;
            var mooveL = moovePnlToLeft(addTopicPnl);

            var mouseY = tempY;
            var mooveT = moovePnlToTop(addTopicPnl);

            var yPos = getY(callingBtn);

            addTopicPnl.style.position = "absolute";

            if (callingBtn.offsetHeight) {
                yPos += (callingBtn.offsetHeight + 5);
            }
            else {
                yPos += 25;
            }

            // "px" needed for Firefox and Chrome
            addTopicPnl.style.top = yPos - mooveT + "px";
            addTopicPnl.style.left = mouseX - mooveL + "px";

            actionPnl.style.top = yPos + "px";
            actionPnl.style.left = mouseX - mooveL + "px";
            actionPnl.style.position = "absolute";

            addTopicPnl.style.visibility = "visible";

        }
    }

}


function AddTopic(toForum) {

    if (toForum != null) {

        var subject = document.getElementById("taTopicSubject");
        var description = document.getElementById("taTopicDescription");

        if (description == null || subject == null) {
            HideElementWithID("divAddTopic", "true");
        }
        else {
            PageMethods.AddTopic(toForum, subject.value, description.value, successAddTopic, timeOut, error)
        }
    }
    else {
        HideElementWithID("divAddTopic", "true");
    }

}

function successAddTopic(str) {
    if (str != null && str.length > 0) {

        var addTopicPnl = document.getElementById("divAddTopic");

        if (actionPnl != null && addTopicPnl != null) {

            actionPnl.innerHTML = str;
            actionPnl.style.visibility = "visible";

            addTopicPnl.style.visibility = "hidden"


            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function() { HideElement(actionPnl, true); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function() { HideElement(actionPnl, true); }, 3000);
            }

        }
    }
    else {
        HideElementWithID("divAddTopic", "true");
    }
}


////////////////////// REPLY TO TOPIC ///////////////////////////

function ShowReplyToTopicPnl(strActionPnlId, strCallBtnId) {

    if (strActionPnlId != null && strCallBtnId != null) {

        var callingBtn = document.getElementById(strCallBtnId);
        actionPnl = document.getElementById(strActionPnlId);
        var addReplyPnl = document.getElementById("divReplyToTopic");

        if (callingBtn != null && actionPnl != null && addReplyPnl != null) {

            var mouseX = tempX;
            var mooveL = moovePnlToLeft(addReplyPnl);

            var mouseY = tempY;
            var mooveT = moovePnlToTop(addReplyPnl);

            var yPos = getY(callingBtn);

            addReplyPnl.style.position = "absolute";

            if (callingBtn.offsetHeight) {
                yPos += (callingBtn.offsetHeight + 5);
            }
            else {
                yPos += 25;
            }

            // "px" needed for Firefox and Chrome
            addReplyPnl.style.top = yPos - mooveT + "px";
            addReplyPnl.style.left = mouseX - mooveL + "px";

            actionPnl.style.top = yPos + "px";
            actionPnl.style.left = mouseX - mooveL + "px";
            actionPnl.style.position = "absolute";

            addReplyPnl.style.visibility = "visible";

        }
    }
}

function AddReplyToTopic(strTopicId) {

    if (strTopicId != null) {

        var description = document.getElementById("taReplyDescription");

        if (description == null) {
            HideElementWithID("divReplyToTopic", "true");
        }
        else {
            PageMethods.WMAddReplyToTopic(strTopicId, description.value, successReplyToTopic, timeOut, error)
        }
    }
    else {
        HideElementWithID("divReplyToTopic", "true");
    }

}

function successReplyToTopic(str) {
    if (str != null && str.length > 0) {

        var addReplyPnl = document.getElementById("divReplyToTopic");

        if (actionPnl != null && addReplyPnl != null) {

            actionPnl.innerHTML = str;
            actionPnl.style.visibility = "visible";

            addReplyPnl.style.visibility = "hidden"

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function() { HideElement(actionPnl, true); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function() { HideElement(actionPnl, true); }, 3000);
            }

        }
    }
    else {
        HideElementWithID("divReplyToTopic", "true");
    }
}


/////////////////////////////////////////////////////



//////////////////////// REPLY TO COMMENT ///////////////////////


function ShowReplyToCommentPnl(strActionPnlId, strCallBtnId, strCommId) {

    if (strActionPnlId != null && strCallBtnId != null && strCommId != null) {

        commID = strCommId;
        var callingBtn = document.getElementById(strCallBtnId);
        actionPnl = document.getElementById(strActionPnlId);
        var addReplyPnl = document.getElementById("divReplyToComment");

        if (callingBtn != null && actionPnl != null && addReplyPnl != null) {

            var mouseX = tempX;
            var mooveL = moovePnlToLeft(addReplyPnl);

            var mouseY = tempY;
            var mooveT = moovePnlToTop(addReplyPnl);

            var yPos = getY(callingBtn);

            addReplyPnl.style.position = "absolute";

            if (callingBtn.offsetHeight) {
                yPos += (callingBtn.offsetHeight + 5);
            }
            else {
                yPos += 25;
            }


            // "px" needed for Firefox and Chrome
            addReplyPnl.style.top = yPos - mooveT + "px";
            addReplyPnl.style.left = mouseX - mooveL + "px";

            actionPnl.style.top = yPos + "px";
            actionPnl.style.left = mouseX - mooveL + "px";
            actionPnl.style.position = "absolute";

            addReplyPnl.style.visibility = "visible";

        }
    }


}

function ReplyToTopicComment() {

    if (commID != null && actionPnl != null) {

        var description = document.getElementById("taReplyToComment");

        if (description == null) {
            HideElementWithID("divReplyToComment", "true");
        }
        else {
            PageMethods.AddReplyToComment(commID, description.value, successReplyToTopicComment, timeOut, error)
        }
    }
    else {
        HideElementWithID("divReplyToComment", "true");
    }

}

function successReplyToTopicComment(str) {
    if (str != null && str.length > 0) {

        var addReplyPnl = document.getElementById("divReplyToComment");

        if (actionPnl != null && addReplyPnl != null) {

            actionPnl.innerHTML = str;
            actionPnl.style.visibility = "visible";

            addReplyPnl.style.visibility = "hidden"

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function() { HideElement(actionPnl, true); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function() { HideElement(actionPnl, true); }, 3000);
            }

        }
    }
    else {
        HideElementWithID("divReplyToComment", "true");
    }

    commID = null;
}

///////////////////////////////// MARK COMMENT AS SPAM /////////////////////

function ReportCommentAsSpam(strPnlAction, strCallingBtn, strCommId) {

    if (strPnlAction != null && strCallingBtn != null && strCommId != null) {

        commID = strCommId;
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

            PageMethods.WMSetMsgAsSpam(commID, successReportCommAsSpam, timeOut, error);

        }

    }


}


function successReportCommAsSpam(str) {
    if (str != null && str.length > 0) {

        if (actionPnl != null) {

            actionPnl.innerHTML = str;
            actionPnl.style.visibility = "visible";

            setTimeout(function() { HideElement(actionPnl, true); }, 3000);
        }
    }

    commID = null;
}

/////////////////////////////////// MODIFY COMMENT //////////////////////////////////

function ShowModifyCommentDiv(strPnlAction, strCallingBtn, strCommId) {

    if (strPnlAction != null && strCallingBtn != null && strCommId != null) {

        commID = strCommId;
        var callingBtn = document.getElementById(strCallingBtn);
        actionPnl = document.getElementById(strPnlAction);
        var modifyPnl = document.getElementById("divModifyComment");

        if (callingBtn != null && actionPnl != null) {

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


            PageMethods.WMGetComment(commID, successShowModifyCommentDiv, timeOut, error);
        }

    }
}

function successShowModifyCommentDiv(str) {
    if (str != null && str.length > 0) {

        var modifyPnl = document.getElementById("divModifyComment");

        if (actionPnl != null && commID != null && modifyPnl != null) {

            var description = document.getElementById("taModifCommDescr");

            if (description != null) {

                description.value = str;
                modifyPnl.style.visibility = "visible";

            }
        }
    }
}

function ModifyComment() {

    if (actionPnl != null && commID != null) {

        var description = document.getElementById("taModifCommDescr");
        if (description == null) {

            HideElementWithID("divModifyComment", "true");
            return;
        }

        HideElementWithID("divModifyComment", "true");

        PageMethods.WMModifyComment(commID, description.value, successModifyComment, timeOut, error);
    }
    else {
        HideElementWithID("divModifyComment", "true");
    }

    commID = null;

}

function successModifyComment(str) {
    if (str != null && str.length > 0) {

        if (actionPnl != null) {

            actionPnl.innerHTML = str;
            actionPnl.style.visibility = "visible";

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function() { HideElement(actionPnl, true); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function() { HideElement(actionPnl, true); }, 3000);
            }
        }
    }
}

////////////////////////////// MODIFY TOPIC ///////////////////////////////

function ShowModifyTopicData(strCallingBtn, strPnlAction) {

    if (strPnlAction != null && strCallingBtn != null) {

        var callingBtn = document.getElementById(strCallingBtn);
        actionPnl = document.getElementById(strPnlAction);
        var modifyPnl = document.getElementById("divModifyTopic");

        if (callingBtn != null && actionPnl != null && modifyPnl != null) {

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
        }

    }


}

function ModifyTopic(strTopicID, strTbSubjId, strTbDescrId) {

    if (strTopicID != null && actionPnl != null && strTbSubjId != null && strTbDescrId != null) {

        var subject = document.getElementById(strTbSubjId);
        var description = document.getElementById(strTbDescrId);

        if (description != null && subject != null) {

            PageMethods.WMModifyTopic(strTopicID, subject.value, description.value, successModifyTopic, timeOut, error);
        }
    }

    HideElementWithID("divModifyTopic", "true");
}

function successModifyTopic(str) {
    if (str != null && str.length > 0) {

        if (actionPnl != null) {

            actionPnl.innerHTML = str;
            actionPnl.style.visibility = "visible";

            if (str.toString().indexOf("<span id=\"reload\" style=\"visibility:hidden;\"></span>") > 0) {
                setTimeout(function() { HideElement(actionPnl, true); window.location = document.location.href; }, 3000);
            }
            else {
                setTimeout(function() { HideElement(actionPnl, true); }, 3000);
            }
        }
    }
}


