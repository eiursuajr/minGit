<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditOfficeAsset.aspx.cs" Inherits="Workarea_EditOfficeAsset" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript">

        function Browseris() {
            var a = navigator.userAgent.toLowerCase();
            this.osver = 1;
            if (a) {
                var g = a.substring(a.indexOf("windows ") + 11);
                this.osver = parseFloat(g)
            }
            this.major = parseInt(navigator.appVersion);
            this.nav = a.indexOf("mozilla") != -1 && (a.indexOf("spoofer") == -1 && a.indexOf("compatible") == -1);
            this.nav6 = this.nav && this.major == 5;
            this.nav6up = this.nav && this.major >= 5;
            this.nav7up = false;
            if (this.nav6up) {
                var b = a.indexOf("netscape/");
                if (b >= 0) this.nav7up = parseInt(a.substring(b + 9)) >= 7
            }
            this.ie = a.indexOf("msie") != -1;
            this.aol = this.ie && a.indexOf(" aol ") != -1;
            if (this.ie) {
                var e = a.substring(a.indexOf("msie ") + 5);
                this.iever = parseInt(e);
                this.verIEFull = parseFloat(e)
            } else this.iever = 0;
            this.ie4up = this.ie && this.major >= 4;
            this.ie5up = this.ie && this.iever >= 5;
            this.ie55up = this.ie && this.verIEFull >= 5.5;
            this.ie6up = this.ie && this.iever >= 6;
            this.ie7down = this.ie && this.iever <= 7;
            this.ie7up = this.ie && this.iever >= 7;
            this.ie8standard = this.ie && document.documentMode && document.documentMode == 8;
            this.winnt = a.indexOf("winnt") != -1 || a.indexOf("windows nt") != -1;
            this.win32 = this.major >= 4 && navigator.platform == "Win32" || a.indexOf("win32") != -1 || a.indexOf("32bit") != -1;
            this.win64bit = a.indexOf("win64") != -1;
            this.win = this.winnt || this.win32 || this.win64bit;
            this.mac = a.indexOf("mac") != -1;
            this.w3c = this.nav6up;
            this.safari = a.indexOf("webkit") != -1;
            this.safari125up = false;
            this.safari3up = false;
            if (this.safari && this.major >= 5) {
                var b = a.indexOf("webkit/");
                if (b >= 0) this.safari125up = parseInt(a.substring(b + 7)) >= 125;
                var f = a.indexOf("version/");
                if (f >= 0) this.safari3up = parseInt(a.substring(f + 8)) >= 3
            }
            this.firefox = this.nav && a.indexOf("firefox") != -1;
            this.firefox3up = false;
            this.firefox36up = false;
            if (this.firefox && this.major >= 5) {
                var d = a.indexOf("firefox/");
                if (d >= 0) {
                    var c = a.substring(d + 8);
                    this.firefox3up = parseInt(c) >= 3;
                    this.firefox36up = parseFloat(c) >= 3.6
                }
            }
        }
        var browseris = new Browseris,
    bis = browseris;

        function IsSupportedFirefoxOnWin() {
            return (browseris.winnt || browseris.win32 || browseris.win64bit) && browseris.firefox3up
        }
        function IsFirefoxOnWindowsPluginInstalled() {

            return navigator.mimeTypes && navigator.mimeTypes["application/x-sharepoint"] && navigator.mimeTypes["application/x-sharepoint"].enabledPlugin
        }
        function CreateFirefoxOnWindowsPlugin() {

            var b = null;
            if (IsSupportedFirefoxOnWin()) try {
                b = document.getElementById("winFirefoxPlugin");
                if (!b && IsFirefoxOnWindowsPluginInstalled()) {
                    var a = document.createElement("object");
                    a.id = "winFirefoxPlugin";
                    a.type = "application/x-sharepoint";
                    a.width = 0;
                    a.height = 0;
                    a.style.setProperty("visibility", "hidden", "");
                    document.body.appendChild(a);
                    b = document.getElementById("winFirefoxPlugin")
                }
            } catch (c) {
                b = null
            }
            return b
        }


        function editDocumentWithProgIDNoUI(strDocument, varProgID, varEditor, bCheckout, strhttpRoot, strCheckouttolocal) {
            var objEditor;
            var fRet;
            var fUseLocalCopy = false;
            varEditor = varEditor.replace(/(?:\.\d+)$/, '');
            if (strDocument.charAt(0) == "/" || strDocument.substr(0, 3).toLowerCase() == "%2f")
                strDocument = document.location.protocol + "//" + document.location.host + strDocument;
            //var strextension = SzExtension(unescapeProperly(strDocument));

            try {
                objEditor = CreateFirefoxOnWindowsPlugin(); //StsOpenEnsureEx2(varEditor + ".3");
                if (objEditor != null) {
                    if (!objEditor.EditDocument3(window, strDocument, fUseLocalCopy, varProgID))
                        return 1;
                    return;
                }
            }
            catch (e) {
            }
            return 1;
        }
        function editInMSOffice(fileName) {
            var obj = new ActiveXObject('SharePoint.OpenDocuments.2');
            if (obj != null)
                obj.EditDocument2(window, fileName, '');
        }
    </script>
</head>
<body>


    <form id="form1" runat="server">
    <div>
        
    </div>
    </form>
    
</body>
</html>

