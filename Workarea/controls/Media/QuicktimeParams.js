function ektQuicktime() {
    this.name = "quicktime";
    this.filled = false;
        //Name: fill
        //Desc:
    this.fill = function ( id ) {
            if (this.filled == true) 
                return true;
            var elemMedia = getObject( id );
            var loop;    
            var controller = 0;
            var type;
            if ( elemMedia != null ) {
                if (this.defaultPlayer) {
                    document.getElementById( "media_width" ).value = elemMedia.width;
                    document.getElementById( "media_height" ).value = elemMedia.height;
                    type = typeof elemMedia.GetAutoPlay;
                    if (("undefined" == type))
                    {
                        document.getElementById( "media_autostart" ).checked = (readValue(elemMedia, "autoplay") == "true");
                    }
                    else
                    {
                        document.getElementById( "media_autostart" ).checked = elemMedia.GetAutoPlay();
                    }
                    type = typeof elemMedia.GetIsLooping;
                    if (("undefined" == type))
                    {
                        document.getElementById( "media_loop" ).checked = (readValue(elemMedia, "loop").toLowerCase() == "true" || readValue(elemMedia, "loop").toLowerCase() == "palindrome");
                    }
                    else
                    {
                        document.getElementById( "media_loop" ).checked = elemMedia.GetIsLooping();
                    }
                        
                }
                type = typeof elemMedia.GetControllerVisible;
                if (("undefined" == type))
                {
                    controller = (readValue(elemMedia, "controller") == "true");
                }
                else
                {
                    controller = elemMedia.GetControllerVisible()
                }
                if ( controller == 1 || controller == true)
                {
                    document.getElementById( "qt_controller" ).checked = true;
                }
                type = typeof elemMedia.GetBgColor;
                if (("undefined" == type))
                {
                    document.getElementById( "qt_bgcolor" ).value = readValue(elemMedia, "bgcolor");
                }
                else
                {
                    document.getElementById( "qt_bgcolor" ).value = elemMedia.GetBgColor();
                }             
                type = typeof elemMedia.GetPlayEveryFrame;
                if (("undefined" == type))
                {
                    document.getElementById( "qt_playeveryframe" ).checked = readValue(elemMedia, "playeveryframe");
                }
                else
                {
                    document.getElementById( "qt_playeveryframe" ).checked = elemMedia.GetPlayEveryFrame();
                }
                type = typeof elemMedia.GetLoopIsPalindrome;
                if (("undefined" == type))
                {
                    document.getElementById( "qt_palindrome" ).checked = (readValue(elemMedia, "loop").toLowerCase() == "palindrome");
                }
                else
                {
                    document.getElementById( "qt_palindrome" ).checked = elemMedia.GetLoopIsPalindrome();
                }
                              
                this.filled = true;
                return true;
            } 
            return false;
        } ;
        //Name: createFlashObject
        //Desc: 
    this.createMeidaPlayerObject = function (action) {
            var sContent;
            var strName = "";
            var strWidth = "";
            var strHeight = "";
            var strFile = "";
            var strBGColor = "";
            var strController = "";
            var bHidden = false;
            var strScale = "";
            var strLoop = "";
            var bPalindrome = false;
            var strAutoPlay = "false";
                        
            objScale = document.getElementById( "qt_scale" );
         
            if ( objScale != null) {
                strScale = objScale.options[objScale.selectedIndex].value;
            }
            
            if (document.getElementById( "media_title" ).value != null) {
                strName = document.getElementById( "media_title" ).value + "_quicktime";
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
            strAutoPlay = document.getElementById( "media_autostart" ).checked.toString();
            strLoop = document.getElementById( "media_loop" ).checked.toString();
            strController = document.getElementById( "qt_controller" ).checked.toString();
            bHidden = document.getElementById( "qt_hidden" ).checked
            strPlayeveryFrame = document.getElementById( "qt_playeveryframe" ).checked.toString();
            bPalindrome = document.getElementById( "qt_palindrome" ).checked;
            if (bPalindrome) {
             strLoop = "Palindrome";
            }            
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
            
            sContent = "<object id=\"" + strName + "\" name=\"" + strName + "\" classid=\"clsid:" + this.CLSID + "\" ";
            if (this.Codebase != "") {
                sContent += "codebase=\"" + this.Codebase + "\" ";
            }                
            sContent += strWidth + " " + strHeight;
            sContent += ">";
            sContent += "<param name=\"src\" value=\"" + strFile + "\" />";
            sContent += "<param name=\"autoplay\" value=\"" + strAutoPlay + "\" />";
            sContent += "<param name=\"bgcolor\" value=\"" + strBGColor + "\" />";
            sContent += "<param name=\"controller\" value=\"" + strController + "\" />";
            sContent += "<param name=\"loop\" value=\"" + strLoop + "\" />";
            if (bHidden) {
                sContent += "<param name=\"hidden\" value=\"true\" />";
            }
            sContent += "<param name=\"playeveryframe\" value=\"" + strPlayeveryFrame + "\" />";
            //sContent += "<param name=\"type\" value=\"video/quicktime\" />";
            sContent += "<param name=\"scale\" value=\"" + strScale + "\" />";
            sContent += "<param name=\"target\" value=\"myself\" />";
            sContent += "<embed pluginspage=\"http://www.apple.com/quicktime/download/\" id=\"" + strName + "\" name=\"" + strName + "\" src=\"" + strFile + "\" ";
            sContent += "type=\"video/quicktime\" autoplay=\"" + strAutoPlay + "\" controller=\"" + strController + "\" ";
            sContent += "loop=\"" + strLoop + "\" ";
            if (bHidden) {
                sContent += "hidden=\"true\" ";
            }
            sContent += "playeveryframe=\"" + strPlayeveryFrame + "\" ";
            sContent += "scale=\"" + strScale + "\" ";
            sContent += strWidth + " " + strHeight;           
            sContent += " target=\"myself\" />";
            sContent += "</object>";
            
            return sContent;

        } ;
        //Name: validateForm
        //Desc:
    this.validateForm = function () {
            return true;
        } ;
}
ektQuicktime.prototype = new ektBaseMultimedia();