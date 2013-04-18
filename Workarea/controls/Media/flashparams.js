function ektFlashPlayer() {
    this.name = "flash";
    this.filled = false;

    //Name: fill
    //Desc:
    this.fill = function ( id ) {
            if (this.filled == true) 
                return true;
            var elemFlash = getObject( id );
            if ( elemFlash != null ) {                
                document.getElementById( "media_width" ).value = elemFlash.width;
                document.getElementById( "media_height" ).value = elemFlash.height;
                document.getElementById( "fl_bgcolor" ).value = readValue(elemFlash, 'bgcolor');
                if (navigator.appName.indexOf("Microsoft Internet")==-1) {                
                    document.getElementById( "media_autostart" ).checked = readValue(elemFlash, 'play');
                } else {
                    document.getElementById( "media_autostart" ).checked = readValue(elemFlash, 'playing');
                }                
                //document.getElementById( "media_loop" ).checked = readValue(elemFlash, 'loop');
                document.getElementById( "menu" ).checked = readValue(elemFlash, 'menu');
                ektFlashPlayer.prototype.selectOption( document.getElementById( "quality" ), readValue(elemFlash, 'quality2').toString() ); 
                ektFlashPlayer.prototype.selectOption( document.getElementById( "scale" ), readValue(elemFlash, 'scale').toString() );
                ektFlashPlayer.prototype.selectOption( document.getElementById( "wmode" ), readValue(elemFlash,'wmode').toString() );
                ektFlashPlayer.prototype.selectOption( document.getElementById( "align" ), readValue(elemFlash,'align').toString() );
                ektFlashPlayer.prototype.selectOption( document.getElementById( "salign" ), readValue(elemFlash,'salign').toString() );
                this.filled = true;
                return true;
            }
            return false;
        } ;


    //Name: createMeidaPlayerObject
    //Desc: 
    this.createMeidaPlayerObject = function (action) {
            var objQuality;
            var objScale;
            var objWindow;
            var objAlign;
            var objSAlign;
            var sContent = "";
            var strName = "";
            var strWidth = "";
            var strHeight = "";
            var strFile = "";
            var strQuality = "";
            var strScale = "";
            var strWMode = "";
            var strBGColor = "";
            var strPlay = "";
            var strLoop = "";
            var strMenu = "";
            var strAlign = "";
            var strSAlign = "";
           
            objQuality = document.getElementById( "quality" )
            objScale = document.getElementById( "scale" )
            objWindow = document.getElementById( "wmode" )
            objAlign = document.getElementById( "align" )
            objSAlign = document.getElementById( "salign" )
            
            ///Read values
            if (document.getElementById( "media_title" ).value != null) {
                strName = document.getElementById( "media_title" ).value + "_" + this.name;
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
            strPlay = document.getElementById( "media_autostart" ).checked.toString();
//            if (document.getElementById( "media_loop" ) != null) {
//                strLoop = document.getElementById( "media_loop" ).checked.toString();
//            }
            if (document.getElementById( "fl_bgcolor" ).value != null) {
                strBGColor = document.getElementById( "fl_bgcolor" ).value;
            }               
           
            if (document.getElementById( "menu" ) != null) {
                strMenu = document.getElementById( "menu" ).checked.toString();
            }
            if ( objQuality != null) {
                strQuality = objQuality.options[objQuality.selectedIndex].value;
            }
            if ( objScale != null) {
                strScale = objScale.options[objScale.selectedIndex].value;
            }
            if ( objWindow != null) {
                strWMode = objWindow.options[objWindow.selectedIndex].value;
            }
            if ( objAlign != null) {
                strAlign = objAlign.options[objAlign.selectedIndex].value;
            }
            if ( objSAlign != null) {
                strSAlign = objSAlign.options[objSAlign.selectedIndex].value;
            }
                      
            if ( strWidth != "" ) {
                 if (!isNaN(strWidth.charAt(strWidth.length - 1))) {
                    strWidth = strWidth + "px";
                }
                strWidth = "width=\"" + strWidth + "\"";
            }
            if ( strHeight != "" ) {
                if (!isNaN(strHeight.charAt(strHeight.length - 1))) {
                    strHeight = strHeight + "px";
                } 
                strHeight = "height=\"" + strHeight + "\"";
            }
            
            //create object tag
            sContent = "<object classid=\"clsid:" + this.CLSID + "\" codebase=\"" + this.Codebase + "\"";
            sContent += "            " + strWidth + " " + strHeight + " id=\"" + strName + "\" name=\"" + strName + "\">";   
            sContent += "            <param name=\"movie\" value=\"" + strFile + "\" />";
            sContent += "            <param name=\"quality\" value=\"" + strQuality + "\" />";
            sContent += "            <param name=\"bgcolor\" value=\"" + strBGColor + "\" />";
            sContent += "            <param name=\"align\" value=\"" + strSAlign + "\" />";
            sContent += "            <param name=\"salign\" value=\"" + strAlign + "\" />";
            if ( strPlay != "") {
                sContent += "            <param name=\"play\" value=\"" + strPlay + "\" />";
            }
            if ( strLoop != "") {
                sContent += "            <param name=\"loop\" value=\"" + strLoop + "\" />";
            }
            if ( strScale != "") {
                sContent += "            <param name=\"scale\" value=\"" + strScale + "\" />";
            }
            if ( strWMode != "") {
                sContent += "            <param name=\"wmode\" value=\"" + strWMode + "\" />";
            }
            if ( strMenu != "") {
                sContent += "            <param name=\"menu\" value=\"" + strMenu + "\" />";
            }
            sContent += "            <embed src=\"" + strFile + "\" quality=\"" + strQuality + "\"  quality2=\"" + strQuality + "\" bgcolor=\"" + strBGColor + "\"  " + strWidth;
            sContent += "                " + strHeight + " id=\"" + strName + "\" name=\"" + strName + "\" align=\"" + strAlign + "\" salign=\"" + strSAlign + "\" type=\"application/x-shockwave-flash\"";
            if ( strPlay != "") {
                sContent += "            play=\"" + strPlay + "\" ";
            }
            if ( strLoop != "") {
                sContent += "            loop=\"" + strLoop + "\" ";
            }
            if ( strScale != "") {
                sContent += "            scale=\"" + strScale + "\" ";
            }
            if ( strWMode != "") {
                sContent += "            wmode=\"" + strWMode + "\" ";
            }
            if ( strMenu != "") {
                sContent += "            menu=\"" + strMenu + "\" ";
            }
            sContent += "                pluginspace=\"http://www.macromedia.com/go/getflashplayer\"></embed>";
            sContent += "        </object>";
                    
            return sContent;            
        } ;

    //Name: validateForm
    //Desc:
    this.validateForm = function () {
        return true;
    };
           
}
ektFlashPlayer.prototype = new ektBaseMultimedia();
