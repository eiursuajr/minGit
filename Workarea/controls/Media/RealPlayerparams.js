function ektRealPlayer () {
    this.name = "realplayer";
    this.filled = false;
    //Name: fill
    //Desc:
    this.fill = function ( id ) {
            if (this.filled == true) 
                return true;
            var elemMedia = getObject( id );
            if ( elemMedia != null ) {            
                try
                {
                    if (this.defaultPlayer) {
                        document.getElementById( "media_width" ).value = elemMedia.width;
                        document.getElementById( "media_height" ).value = elemMedia.height;
                        document.getElementById( "media_autostart" ).checked = elemMedia.GetAutoStart();
                        document.getElementById( "media_loop" ).checked = elemMedia.GetLoop();
                    }
                    document.getElementById( "rp_type" ).value = elemMedia.type;

                    document.getElementById( "rp_center" ).checked = elemMedia.GetCenter();
                    document.getElementById( "rp_aspect" ).checked = elemMedia.GetMaintainAspect();
                    document.getElementById( "rp_nologo" ).checked = elemMedia.GetNoLogo();
                    document.getElementById( "rp_numloops" ).value = elemMedia.GetNumLoop();
                    ektRealPlayer.prototype.selectOption( document.getElementById( "rp_controls" ),  elemMedia.GetControls() );  
                }
                catch (e)
                {
                    //----- Defect 19985.  If real player is not installed, web.config is configured to use it and the multimedia
                    //----- file is being editted, the code will throw.  Just do nothing since the default player will be used to
                    //----- edit the file.
                }  
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
            var strBGColor = "";
            var strLoop = "";
            var strConsole = "one";
            var strControls = "";
            var strType = "audio/x-pn-realaudio-plugin";
            var objControls;
            objControls = document.getElementById( "rp_controls" );
            if ( objControls != null) {
                strControls = objControls.options[objControls.selectedIndex].value;
            }
            if (document.getElementById( "rp_type" ).value != null)
            {
                if (document.getElementById( "rp_type" ).value != "")
                    strType = document.getElementById( "rp_type" ).value;
            }
            ///common
            if (document.getElementById( "media_title" ).value != null) {
                strName = document.getElementById( "media_title" ).value + '_realplayer';
            }
            if (document.getElementById( "media_width" ).value != null) {
                strWidth = document.getElementById( "media_width" ).value;
            }
            if (document.getElementById( "media_height" ).value != null) {
                strHeight = document.getElementById( "media_height" ).value;
            }
            if (typeof action == "undefined"){
                action = "save";
            }
            if (action == "save" || action == "checkin" || action == "preview") {
                strFile = this.URL;
            }
            else {
                if (document.getElementById( "media_fileName" ).value != null) {
                    strFile = document.getElementById( "media_fileName" ).value;
                }
            }
            
            strAutoStart = document.getElementById( "media_autostart" ).checked.toString();
            strLoop = document.getElementById( "media_loop" ).checked.toString();
        
            //if (document.getElementById( "bgcolor" ).value != null) {
            //    strBGColor = document.getElementById( "bgcolor" ).value;
            //} 
            if ( strWidth != "" ) {
                if (!isNaN(strWidth.charAt(strWidth.length - 1))) {
                    strWidth = strWidth + "px";
                }
                strWidth = "width=\"" + strWidth + "\" ";
            } else {
                strWidth = "width=\"100%\" ";
            }
            if ( strHeight != "" ) {
                if (!isNaN(strHeight.charAt(strHeight.length - 1))) {
                    strHeight = strHeight + "px";
                } 
                strHeight = "height=\"" + strHeight + "\" ";
            } else {
                strHeight = "height=\"100%\" ";
            }
  
            // Real Player    
            sContent = "<object id=\"" + strName + "\" name=\"" + strName + "\" classid=\"clsid:" + this.CLSID + "\" ";
            if (this.Codebase != "") {
                sContent += " codebase=\"" + this.Codebase + "\" ";
            }
            sContent += strWidth + " " + strHeight;           
            sContent += ">";
            sContent += "<param name=\"SRC\" value=\"" + strFile + "\" />";
            sContent += "<param name=\"controls\" value=\"" + strControls + "\" />";
            sContent += "<param name=\"autostart\" value=\"" + strAutoStart + "\" />";
            sContent += "<param name=\"loop\" value=\"" + strLoop + "\" />";
            sContent += "<param name=\"type\" value=\"" + strType + "\" />";
            if (strConsole != "") { sContent += "<param name=\"CONSOLE\" value=\"" + strConsole + "\" />"; }
            sContent += "<embed type=\"" + strType + "\" id=\"" + strName + "\" name=\"" + strName + "\" src=\"" + strFile + "\" loop=\"" + strLoop + "\" ";
            sContent += strWidth + " " + strHeight;            
            sContent += " nojava=\"true\" controls=\"" + strControls + "\" ";
            if (strConsole != "") { sContent += "console=\"" + strConsole + "\" "; }
            sContent += " autostart=\"" + strAutoStart + "\" ";
            sContent += " />";
            sContent += "</object>";
            
            return sContent;
        };

    //Name: validateForm
    //Desc:
    this.validateForm = function () {
            return true;
        };
}
ektRealPlayer.prototype = new ektBaseMultimedia();
