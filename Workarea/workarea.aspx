<%@ Page Language="C#" AutoEventWireup="true" Inherits="workarea" CodeFile="workarea.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />

    <script type="text/javascript">
            var PerReadOnlyLib = '<asp:Literal ID="litPerReadOnlyLib" runat="server"/>';
            var PerContentTreeLang = '<asp:Literal ID="litLanguageId1" runat="server"/>';
            var PerLibraryTreeLang = '<asp:Literal ID="litLanguageId2" runat="server"/>';
            var PerMainPage = '<asp:Literal ID="litMainPage" runat="server"/>';
            var PerWorkareaPrefix = '<asp:Literal ID="litWorkareaPrefix" runat="server"/>';
            
            if("undefined" == typeof Ektron.Workarea)
            {
                Ektron.Workarea = {};
            }
            
            if("undefined" == typeof Ektron.Workarea.ClipBoard)
            {
                Ektron.Workarea.ClipBoard = {
                    action: "",
                    items:[]
                };
            }
            if("undefined" == typeof Ektron.Workarea.isnoncontentframe)
            {
                Ektron.Workarea.backurl = false;
            }
            
            if("undefined" == typeof Ektron.Workarea.backurl)
            {
                Ektron.Workarea.backurl = "";
            }
            if("undefined" == typeof Ektron.Workarea.TaxonomyClipBoard)
            {
                Ektron.Workarea.TaxonomyClipBoard = {
                    action: "",
                    items:[]
                };
            }
            
            if("undefined" == typeof Ektron.Workarea.FolderClipBoard)
            {
                Ektron.Workarea.FolderClipBoard = {
                    action: "",
                    folderid:"",
                    foldertype:""
                };
            }
            
            if("undefined" == typeof Ektron.Workarea.TaxonomyTreeClipBoard)
            {
                Ektron.Workarea.TaxonomyTreeClipBoard = {
                    action: "",
                    taxonomyid:""
                };
            }

            // Setting the required values for content page folder variables,
            // so that api call to get folder can be avoided.
            
            if("undefined" == typeof Ektron.Workarea.FolderContext)
            {
                Ektron.Workarea.FolderContext = {
                    folderType:  "",
                    folderId: "",
                    folderParentId: "",
                    folderLanguage: ""
                };
            }
            
            $ektron("#ek_nav_bottom").resize(function(){
                NavTreeResizing();
            });

            onload = function kickTabSwitch() {
                if (PerMainPage != '') {
                    // kick off timer switch to Content tab
                    setTimeout(switchContentTab, 250);
                }
            }
            
            if ( Ektron.Workarea.AddPermissionItems === undefined) { Ektron.Workarea.AddPermissionItems = {}; }

            Ektron.Workarea.Height = {
                callbacks: [],
                init: function () {
                    $ektron(window).resize(function () {
                        Ektron.Workarea.Height.execute();
                    });
                },
                heightFix: function (callback) {
                    this.callbacks.push(callback);
                },
                execute: function () {
                    setTimeout(function () {
                        //execute for ie and chrome
                        var isChrome = false;
                        if ( $.browser.webkit && !$.browser.opera && !$.browser.msie && !$.browser.mozilla ) {
                            var userAgent = navigator.userAgent.toLowerCase();
                            if ( userAgent.indexOf("chrome") !== -1 ) { 
                                isChrome = true;
                            }
                        }
                        var version = parseInt($ektron.browser.version, 10);
                        if (isChrome || ($ektron.browser.msie && (version === 8 || version === 9))) {
                            var newHeight = parseInt($ektron(window).height(), 10) - 59;
                            for (var i = 0; i < Ektron.Workarea.Height.callbacks.length; i++) {
                                try {
                                    Ektron.Workarea.Height.callbacks[i](newHeight);
                                }
                                catch (e) {
                                    //frame has likely been reloaded, remove script
                                    Ektron.Workarea.Height.callbacks.splice(i, 1);
                                }
                            }
                        }
                    }, 5);
                }
            }
            Ektron.ready(function() {
                Ektron.Workarea.Height.init();
            });
    </script>

    <script type="text/javascript" src="java/workareawindow.js">
    </script>

    <script type="text/javascript">
            if (document.layers) {
                onresize = function reDo() {top.ek_nav_bottom.document.location.href="reloadworkareatree.htm";}
            }
    </script>

</head>
<frameset rows="59,*" cols="100%" border="0">
		<frame id="workareatop" name="workareatop" src="workareatop.aspx" scrolling="no" noresize="noresize" frameborder="0" />
		<frameset id="BottomFrameSet" cols="0,*" rows="100%" border="0">
			<frame name="ek_nav_bottom" id="ek_nav_bottom" src="workareanavigationtree.aspx" scrolling="no" frameborder="0" runat="server" />
			<frameset id="BottomRightFrame" rows="*,1" cols="100%" border="0">
				<frame id="ek_main" name="ek_main" src="dashboard.aspx" scrolling="auto" frameborder="0" runat="server" />
		</frameset>
	</frameset>
	</frameset>
	<noframes></noframes>
</html>
