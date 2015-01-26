// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

function htmlEncode(str) {
    if (!str) { str = ""; }
    var div = document.createElement('div');
    var divText = document.createTextNode(str);
    div.appendChild(divText);
    str = div.innerHTML;
    div.removeChild(divText);
    return str;
}

function htmlEncodeText(srcID, destID) {
    if (srcID && destID) {
        var src = document.getElementById(srcID);
        var dest = document.getElementById(destID);
        if (src && dest) {
            var txt = src.value;
            txt = htmlEncode(txt);
            dest.value = txt;
        }
    }
}

function htmlEncodeReplaceText(targetID) {
    if (targetID) {
        htmlEncodeText(targetID, targetID);
    }
}


//////////////////////////////////////////////////


function breakInputHtmlTags(elementId) {
    if (elementId) {
        var element = document.getElementById(elementId);
        var newValue = element.value.replace(/</g, "< ");
        element.value = newValue;
    }
}

function breakHtmlTags(elementId) {
    if (elementId) {
        var form = document.getElementById(elementId);

        for (var i = 0; i < form.length; i++) {
            if (form.elements[i].type == "text" || form.elements[i].type == "textarea" || form.elements[i].type == "password") {
                if (!form.elements[i].disabled && form.elements[i].value != null && form.elements[i].value.length > 0) {
                    form.elements[i].value = form.elements[i].value.replace(/<(?=\S)/g, "&lt;");
                }
            }
        }
    }
}


///////////////////////////////////////////////////

function moovePnlToLeft(pnl) {

    var windH = getWindowWidth();
    var panelW;

    panelW = pnl.offsetWidth;

    var mouseX = tempX;

    var mooveL = 0;
    if ((windH - 20 - mouseX) < panelW) {

        mooveL = panelW - (windH - mouseX) + 50;

    }

    return mooveL;
}

function moovePnlToTop(pnl) {

    var hrightH = getWindowHeight();
    var panelW;

    panelH = pnl.offsetHeight;

    var mouseY = tempY;

    var mooveT = 0;
    if ((hrightH - 20 - mouseY) < panelH) {

        mooveT = panelH - (hrightH - mouseY) + 50;

    }

    return mooveT;
}


///////////////////////////////////////////////////

function getY(oElement) {

    var iReturnValue = 0;
    while ((oElement != undefined) && (oElement != null)) {
        iReturnValue += oElement.offsetTop;
        oElement = oElement.offsetParent;
    }
    return iReturnValue;
}

function getX(oElement) {
    var iReturnValue = 0;
    while ((oElement != undefined) && (oElement != null)) {
        iReturnValue += oElement.offsetLeft;
        oElement = oElement.offsetParent;
    }
    return iReturnValue;
}

///////////////////////////////////////////////////


function getWindowWidth() {
    var myWidth = 0;

    //myHeight = 0;
    if (typeof (window.innerWidth) == 'number') {
        //Non-IE
        myWidth = window.innerWidth;
    }
    else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
        //IE 6+ in 'standards compliant mode'
        myWidth = document.documentElement.clientWidth;
    }
    else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
        //IE 4 compatible
        myWidth = document.body.clientWidth;
    }

    return myWidth;
}

function getWindowHeight() {
    var myHeight = 0;

    //myHeight = 0;
    if (typeof (window.innerHeight) == 'number') {
        //Non-IE
        myHeight = window.innerHeight;
    }
    else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
        //IE 6+ in 'standards compliant mode'
        myHeight = document.documentElement.clientHeight;
    }
    else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
        //IE 4 compatible
        myHeight = document.body.clientHeight;
    }

    return myHeight;
}

var tempX = 0;
var tempY = 0;

document.onmousemove = function(event) {
    if (!event) {
        event = window.event;
    }
    tempX = event.clientX;
    tempY = event.clientY;
    if (tempX < 0) { tempX = 0; }
    if (tempY < 0) { tempY = 0; }
}

///////////////////////////////////////////////////

function HideElementWithID(elementID, boolChangePos) {

    var elm = document.getElementById(elementID);
    if (elm != null) {

        elm.style.visibility = "hidden";
        elm.style.position = "absolute";
    }

    if (elm != null && boolChangePos != null && boolChangePos == "true") {
        elm.style.top = "1px";
        elm.style.left = "1px";
    }

}

function HideElement(element, boolChangePos) {

    if (element != null) {

        element.style.visibility = "hidden";
        element.position = "absolute";
    }

    if (element != null && boolChangePos != null && boolChangePos == true) {
        element.style.top = "1px";
        element.style.left = "1px";
    }

}
