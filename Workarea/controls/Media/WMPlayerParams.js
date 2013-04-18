
function ektWindowsMedia() {
    this.name = "windowsmedia";
    this.filled = false;

    //Name: fill
    //Desc:
    this.fill = function (id) {
        if (this.filled == true)
            return true;
        var elemWM = getObject(id);
        if (elemWM != null) {
            //document.getElementById( "media_name" ).value = elemWM.id;
            //document.getElementById( "media_fileName" ).value = elemWM.object.url;
            if (this.defaultPlayer) {
                document.getElementById("media_width").value = elemWM.width;
                document.getElementById("media_height").value = elemWM.height;
                if (navigator.appName.indexOf("Microsoft Internet") == -1) {
                    document.getElementById("media_autostart").checked = readValue(elemWM, 'autostart');
                    document.getElementById("media_loop").checked = readValue(elemWM, 'playCount') == 9999;
                } else {
                    document.getElementById("media_autostart").checked = readValue(elemWM.settings, 'autostart');
                    document.getElementById("media_loop").checked = readValue(elemWM.settings, 'playCount') == 9999;
                }

            }

            if (navigator.appName.indexOf("Microsoft Internet") == -1) {
                document.getElementById("wm_playcount").value = readValue(elemWM, 'playCount');
                document.getElementById("wm_contextmenu").checked = readValue(elemWM, 'ContextMenu');
                //document.getElementById( "wm_stretchToFit" ).checked = readValue(elemWM, 'sendplaystatechangeevents');

            } else {
                document.getElementById("wm_playcount").value = readValue(elemWM.settings, 'playCount');
                if (elemWM.object != null) {
                    document.getElementById("wm_contextmenu").checked = elemWM.object.enableContextMenu;
                }
                //document.getElementById( "wm_stretchToFit" ).checked = readValue(elemWM, 'stretchToFit');

            }

            document.getElementById("wm_enabled").checked = readValue(elemWM, 'enabled');
            document.getElementById("wm_windowlessVideo").checked = readValue(elemWM, 'windowlessVideo');


            ektWindowsMedia.prototype.selectOption(document.getElementById("wm_uimode"), readValue(elemWM, 'uimode').toString());

            this.filled = true;
            return true;
        }
        return false;
    };

    //Name: createFlashObject
    //Desc:
    this.createMeidaPlayerObject = function (action) {
        var sContent;
        var strName = "";
        var strWidth = "";
        var strHeight = "";
        var strFile = "";
        var strUiMode = "";
        var strContextMenu = "";
        var strFullScreen = "";
        var strEnabled = "";
        var strStretchToFit = "";
        var strBalance = "0";
        var strPlayCount = "1";
        var strAutoStart = "";
        var objWindow;
        objWindow = document.getElementById("wm_uimode")

        ///common
        if (document.getElementById("media_title").value != null) {
            strName = document.getElementById("media_title").value + '_windowsmedia';
        }
        if (document.getElementById("media_width").value != null) {
            strWidth = document.getElementById("media_width").value;
        }
        if (document.getElementById("media_height").value != null) {
            strHeight = document.getElementById("media_height").value;
        }
//        if (typeof action == "undefined") {
//            action = "save";
//        }
        if (action == "save" || action == "checkin" || action == "preview") {
            strFile = this.URL;
        }
        else {
            if (document.getElementById("media_fileName").value != null) {
                strFile = document.getElementById("media_fileName").value;
            }
        }
        strAutoStart = document.getElementById("media_autostart").checked.toString();

        strContextMenu = document.getElementById("wm_contextmenu").checked.toString();
        strEnabled = document.getElementById("wm_enabled").checked.toString();
        //strStretchToFit = document.getElementById( "wm_stretchToFit" ).checked.toString();
        strWindowlessVideo = document.getElementById("wm_windowlessVideo").checked.toString();
        //strBalance = document.getElementById( "wm_balance" ).value;
        if (document.getElementById("media_loop").checked) {
            strPlayCount = "9999";
        } else {
            strPlayCount = document.getElementById("wm_playcount").value;
        }

        if (objWindow != null) {
            strUiMode = objWindow.options[objWindow.selectedIndex].value;
        }

        if (strWidth != "") {
            if (!isNaN(strWidth.charAt(strWidth.length - 1))) {
                strWidth = strWidth + "px";
            }
            strWidth = "width=\"" + strWidth + "\" ";
        } else {
            strWidth = "width=\"100%\" ";
        }
        if (strHeight != "") {
            if (!isNaN(strHeight.charAt(strHeight.length - 1))) {
                strHeight = strHeight + "px";
            }
            strHeight = "height=\"" + strHeight + "\" ";
        } else {
            strHeight = "height=\"100%\" ";
        }

        //Let's create object tag based on settings
        sContent = "";
        sContent = "<object id=\"" + strName + "\" name=\"" + strName + "\"";
        sContent += "               " + strWidth + " " + strHeight;
        sContent += "				classid=\"CLSID:" + this.CLSID + "\" ";
        if (this.Codebase != "") {
            sContent += " codebase=\"" + this.Codebase + "\" ";
        }
        sContent += "				type=\"application/x-oleobject\">";
        sContent += "				<param name=\"balance\" value=\"" + strBalance + "\"/>";
        sContent += "				<param name=\"windowlessVideo\" value=\"" + strWindowlessVideo + "\"/>";
        sContent += "				<param name=\"enabled\" value=\"" + strEnabled + "\"/>";
        sContent += "				<param name=\"EnableContextMenu\" value=\"" + strContextMenu + "\"/>";
        sContent += "				<param name=\"URL\" value=\"" + strFile + "\"/>";
        sContent += "				<param name=\"SendPlayStateChangeEvents\" value=\"True\"/>";
        sContent += "				<param name=\"AutoStart\" value=\"" + strAutoStart + "\"/>";
        sContent += "				<param name=\"uiMode\" value=\"" + strUiMode + "\"/>";
        sContent += "				<param name=\"PlayCount\" value=\"" + strPlayCount + "\"/>";
        sContent += "               <embed type=\"application/x-mplayer2\" ";
        sContent += "               pluginspage = \"http://www.microsoft.com/windows/windowsmedia/download/\" ";
        sContent += "                id=\"" + strName + "\" name=\"" + strName + "\" fileName=\"" + strFile + "\" ";
        sContent += "               " + strWidth + " " + strHeight;
        sContent += "               playcount=\"" + strPlayCount + "\" ";
        sContent += "               autostart=\"" + convertStringBoolToInt(strAutoStart) + "\" ";
        sContent += "               EnableContextMenu=\"" + convertStringBoolToInt(strContextMenu) + "\" ";
        sContent += "               enabled=\"" + convertStringBoolToInt(strEnabled) + "\" ";
        sContent += "               SendPlayStateChangeEvents=\"1\" ";
        sContent += "               />";
        sContent += "			</object>";
        return sContent;
    };

    //Name: validateForm
    //Desc:
    this.validateForm = function () {
        return true;
    };


}

ektWindowsMedia.prototype = new ektBaseMultimedia();

convertStringBoolToInt = function (stringBool) {
    var returnInt = "0";

    if (stringBool == "true") {
        returnInt = "1";
    }

    return returnInt;
};