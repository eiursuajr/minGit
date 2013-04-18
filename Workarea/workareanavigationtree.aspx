<%@ Page Language="C#" AutoEventWireup="true" Inherits="workareanavigationtree" CodeFile="workareanavigationtree.aspx.cs" %>

<html>
<head runat="server">
    <title>Workarea Navigation Tree</title>
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />

    <script type="text/javascript">
            <!--//--><![CDATA[//><!--
                function blockWindowClick(bBlock)
                {
                    try
                    {
                        if(bBlock)
                            ektb_show_no_url();
                        else
                            ektb_remove();
                    }
                    catch(e)
                    {
                        ;
                    }
                }

			    var szOldColumnsSize = "157,*";
			    var iIe5FrameSizeAdjustment = 4;
			    var bPinned = true;
			    var iFrameSize = 1;
			    var iCurrentSize = 0;
			    var iChangeSize = 25;
			    var aszFrameCols = "";
			    var iFadeInterval = 20;
			    var bBusy = false;
			    var bShrinkPending = false;
			    var constMaxShrinkCount = 750;
			    var iCurrentShrinkCount = 0;
			    var bEnlargePending = false;
			    var constMaxEnlargeCount = 200;
			    var iCurrentEnlargeCount = 0;

			    var bContentTreeLoaded = false;
			    var bTaxTreeLoaded = false;
			    var bMenuTreeLoaded = false;
			    var bCollTreeLoaded = false;
			    var bFormsTreeLoaded = false;
			    var bLibraryTreeLoaded = false;
			    var bAdminTreeLoaded = false;
			    var bReportTreeLoaded = false;
			    

			    var browseFrameName = "";

			    var szContentNav = "<asp:Literal ID='litszContentNav' runat='server' />";
			    var szTaxNav = "<asp:Literal ID='litszTaxNav' runat='server' />";
			    var szMenuNav = "<asp:Literal ID='litszMenuNav' runat='server' />";
			    var szCollNav = "<asp:Literal ID='litszCollNav' runat='server' />";
			    var szFormsNav = "<asp:Literal ID='litszFormsNav' runat='server' />";
			    var szLibNav = "<asp:Literal ID='litszLibNav' runat='server' />";
			    var szAdminNav = "<asp:Literal ID='litszAdminNav' runat='server' />";
			    var szReportNav = "<asp:Literal ID='litszReportNav' runat='server' />";
			    var szVisibleStartTree = "<asp:Literal ID='litszVisibleStartTree' runat='server' />";

			    var m_OrigMouseOver = "";
			    var m_OrigMouseOut = "";
			    var m_loaded = false;
			    var m_MinFrameSize = 10;
			    var LinkFileName="<asp:Literal ID='litLinkFileName' runat='server' />";

			    function StartShrink(e) {
				    var myTarget;

				    if (document.all) {
					    myTarget = e.toElement;
				    }
				    else {
					    myTarget = e.relatedTarget;
				    }
				    // Is this a valid target?
				    // It should not be if we left the page.
				    if (myTarget == null) {
					    BeginShrink();
				    }
				    return;
			    }

			    function StartEnlarge(e) {
				    var myTarget;
				    if (!CanShowNavTree()) {
					    return;
				    }
				    if (document.all) {
					    myTarget = e.fromElement;
				    }
				    else {
				        if (IsBrowserSafari2() == true) {
				            myTarget = null;
				        } else {
					        myTarget = e.relatedTarget;
					    }
				    }
				    if (myTarget == null) {
					    setTimeout("BeginEnlarge();", 1);
				    }
				    return;
			    }

			    function PollShrink() {
				    bEnlargePending = false;
				    if (bShrinkPending == true) {
					    if (bBusy == true) {
						    setTimeout("PollShrink();", 100);
						    return;
					    }
					    else {
						    iCurrentShrinkCount = 0;
							    setTimeout("DelayShrink()", 10);
						    //}
					    }
				    }
			    }

			    function PollEnlarge() {
				    if (bEnlargePending == true) {
					    if (bBusy == true) {
						    setTimeout("PollEnlarge();", 100);
						    return;
					    }
					    else {
						    iCurrentEnlargekCount = 0;
							    setTimeout("DelayEnlarge()", 10);
					    }
				    }
			    }

			    function BeginShrink () {
				    bEnlargePending = false;
				    if (bPinned == false) {
					    if (iFrameSize == 0) {
						    if (bBusy == true) {
							    bShrinkPending = true;
							    setTimeout("PollShrink();", 100);
							    return;
						    }
					    }
					    else {
						    if (bBusy == false) {
							    iCurrentShrinkCount = 0;
							    if (bShrinkPending == false) {
								    bShrinkPending = true;
								    setTimeout("DelayShrink()", 10);
							    }
						    }
					    }
				    }
			    }

			    function DelayShrink () {
				    if (bShrinkPending == true) {
					    iCurrentShrinkCount += 10;
					    if (iCurrentShrinkCount > constMaxShrinkCount) {
						    iCurrentShrinkCount = 0;
						    ResizeFrame();
						    return;
					    }
					    setTimeout("DelayShrink()", 10);
				    }
			    }

			    function DelayEnlarge () {
				    if (bEnlargePending == true) {
					    iCurrentEnlargeCount += 10;
					    if ((iCurrentEnlargeCount > constMaxEnlargeCount) || (IsBrowserSafari2() == true)) {
						    iCurrentEnlargeCount = 0;
						    ResizeFrame();
						    return;
					    }
					    setTimeout("DelayEnlarge()", 10);
				    }
			    }

			    function BeginEnlarge () {
				    bShrinkPending = false;
				    if (iFrameSize == 1) {
					    if (bBusy == true) {
						    bEnlargePending = true;
						    setTimeout("PollEnlarge();", 100);
						    return;
					    }
				    }
				    else {
					    if (bBusy == false) {
						    iCurrentEnlargeCount = 0;
						    if (bEnlargePending == false) {
							    bEnlargePending = true;
							    setTimeout("DelayEnlarge()", 10);
						    }
					    }
				    }
			    }

			    function IsBrowserSafari(){
				    if ((typeof(top.IsBrowserSafari) == "function") && top != self) {
					    return (top.IsBrowserSafari());
				    } else {
					    return false;
				    }
			    }

			    function IsBrowserSafari2() {
			        var posn2;
			        posn2 = parseInt(navigator.appVersion.toLowerCase().indexOf('safari'));
			        return (0 <= posn2);
		        }

			    function IsPlatformMac(){
				    if ((typeof(top.IsPlatformMac) == "function") && top != self) {
					    return (top.IsPlatformMac());
				    } else {
					    return false;
				    }
			    }

			    function SnapNavFrame(){
				    if ((typeof(top.SnapNavFrame) == "function") && top != self) {
					    return (top.SnapNavFrame());
				    } else {
					    return false;
				    }
			    }

			    function FadeFrame(Direction) {
				    var NavFrame = top.document.getElementById('BottomFrameSet');
				    if (Direction == 0) {
					    bEnlargePending = false;
					    iCurrentSize = iCurrentSize - iChangeSize;
					    if ((iCurrentSize <= m_MinFrameSize) || SnapNavFrame()) {
						    iCurrentSize = m_MinFrameSize;
						    iFrameSize = 0;
						    bBusy = false;
						    document.body.onmouseover = m_OrigMouseOver;
						    document.body.onmouseout = m_OrigMouseOut;
					    }
					    else {
						    setTimeout("FadeFrame(" + Direction + ")", iFadeInterval);
					    }
				    }
				    else {
					    iCurrentSize = iCurrentSize + iChangeSize;
					    if ((iCurrentSize >= aszFrameCols[0]) || SnapNavFrame()) {
						    iCurrentSize = aszFrameCols[0];
						    iFrameSize = 1;
						    bBusy = false;
						    document.body.onmouseover = null;
						    document.body.onmouseout = null;
					    }
					    else {
						    setTimeout("FadeFrame(" + Direction + ")", iFadeInterval);
					    }
				    }
				    NavFrame.cols = iCurrentSize + ",*";
				    if (iCurrentSize <= m_MinFrameSize) {
					    iCurrentSize = 0;
				    }
				    document.getElementById("div2").style.left = (iCurrentSize - aszFrameCols[0]) + "px";
			    }

			    function ResizeFrame(Direction) {
				    var NavFrame = top.document.getElementById('BottomFrameSet');

				    if (Direction != null) {
					    if ((iFrameSize == 1) && (Direction == 1))
						    return;
					    if ((iFrameSize == 0) && (Direction == 0))
						    return;
				    }

				    bBusy = true;
				    iCurrentSize = 0;
				    if (((Direction == null) && (iFrameSize == 1)) || (Direction == 0)) {
					    szOldColumnsSize = NavFrame.cols;
					    if (top.IsBrowserIE() && (!((navigator.userAgent.toLowerCase()).indexOf("msie 5.") == -1)) ) {
						    var tmpFrameSizes = NavFrame.cols.split(",");
						    tmpFrameSizes[0] = (parseInt(document.body.clientWidth) + (iIe5FrameSizeAdjustment * 1));
						    szOldColumnsSize = "";
						    for (var iLoop = 0; iLoop < tmpFrameSizes.length; iLoop++) {
							    szOldColumnsSize += tmpFrameSizes[iLoop].toString() + ",";
						    }
						    szOldColumnsSize = szOldColumnsSize.substring(0, (szOldColumnsSize.length - 1));
					    }
					    aszFrameCols = szOldColumnsSize.split(",");
					    iCurrentSize = aszFrameCols[0];
					    setTimeout("FadeFrame(0)", 50);
				    }
				    else {
					    document.getElementById("div2").style.display = "";
					    aszFrameCols = szOldColumnsSize.split(",");
					    iCurrentSize = 0;
					    setTimeout("FadeFrame(1)", 50);
				    }
			    }

			    function NavTreeResizing() {
				    var NavFrame;
				    if ((iFrameSize == 0) && (!bBusy)) {
					    NavFrame = top.document.getElementById('BottomFrameSet');
					    if (NavFrame) {
						    NavFrame.cols = m_MinFrameSize + ",*";
					    }
				    }
			    }

			    function TogglePin() {
				    if (bPinned == true) {
					    bPinned = false;
				    }
				    else {
					    bPinned = true;
				    }
				    return (bPinned);
			    }

			    function ShrinkFrame() {
				    ResizeFrame();
			    }

			    function AdjustWindows() {
				    var offsetTop = 0;
				    var activeFrame = "";

				    if (document.getElementById("ContentTree").style.display.toLowerCase() != "none") {
					    activeFrame = document.getElementById("ContentTree");
					}
				    else if (document.getElementById("FormsTree").style.display.toLowerCase() != "none") {
					    activeFrame = document.getElementById("FormsTree");
				    }
				    else if (document.getElementById("LibraryTree").style.display.toLowerCase() != "none") {
					    activeFrame = document.getElementById("LibraryTree");
				    }
				    else if (document.getElementById("AdminTree").style.display.toLowerCase() != "none") {
					    activeFrame = document.getElementById("AdminTree");
				    }
				    activeFrame.style.height = "100%";
				    if (document.all) {
					    offsetTop = activeFrame.offsetTop;
				    }
				    else {
					    offsetTop = (activeFrame.offsetTop * 2);
				    }
				    if ((document.body.clientHeight - offsetTop) >= 0) {
					    activeFrame.style.height = document.body.clientHeight - offsetTop;
				    }
			    }

			    function MakeNavTreeVisible(FrameName, folderAreaObj) {

			        if (FrameName != "ContentTree" && FrameName != "TaxTree" && FrameName != "MenuTree" && FrameName != "CollTree") {
					    folderAreaObj.document.getElementById("ContentTree").style.display = "none";
				    }
				    if (FrameName != "AdminTree") {
					    folderAreaObj.document.getElementById("AdminTree").style.display = "none";
				    }
				    if (FrameName != "LibraryTree") {
					    folderAreaObj.document.getElementById("LibraryTree").style.display = "none";
				    }
				    if (FrameName != "ReportTree") {
					    folderAreaObj.document.getElementById("ReportTree").style.display = "none";
				    }
				    if (FrameName != "SmartDesktopTree" && FrameName != "Help") {
				        if (folderAreaObj.document.getElementById(FrameName))
				        {
				            folderAreaObj.document.getElementById(FrameName).style.display = "";
				        }
				    }

				    switch (FrameName) {
				        case ("ContentTree"): {
				                if (bContentTreeLoaded == false) {
				                    folderAreaObj.document.getElementById(FrameName).src = LinkFileName + FrameName.replace("Tree", "") + "&framename=" + browseFrameName + "&AutoNav=" + szContentNav;
				                    folderAreaObj.document.getElementById(FrameName).style.height = "99%";
				                    folderAreaObj.document.getElementById(FrameName).style.height = "100%";
				                    szContentNav = "";
				                    bContentTreeLoaded = true;
				                }
				                break;
				            }
				        case ("TaxTree"): 
				            {
				                if (bTaxTreeLoaded == false) {
				                    folderAreaObj.document.getElementById("ContentTree").src = LinkFileName.replace("TreeVisible=&", "TreeVisible=taxonomy&") + FrameName.replace("Tree", "") + "&framename=" + browseFrameName + "&AutoNav=" + szTaxNav;
				                    folderAreaObj.document.getElementById("ContentTree").style.height = "99%";
				                    folderAreaObj.document.getElementById("ContentTree").style.height = "100%";
				                    szTaxNav = "";
				                    bTaxTreeLoaded = true;
				                }
				                break;
				            }
				        case ("MenuTree"):
				            {
				                if (bMenuTreeLoaded == false) {
				                    folderAreaObj.document.getElementById("ContentTree").src = LinkFileName.replace("TreeVisible=&", "TreeVisible=menu&") + FrameName.replace("Tree", "") + "&framename=" + browseFrameName + "&AutoNav=" + szMenuNav;
				                    folderAreaObj.document.getElementById("ContentTree").style.height = "99%";
				                    folderAreaObj.document.getElementById("ContentTree").style.height = "100%";
				                    szMenuNav = "";
				                    bMenuTreeLoaded = true;
				                }
				                break;
				            }
				        case ("CollTree"):
				            {
				                if (bCollTreeLoaded == false) {
				                    folderAreaObj.document.getElementById("ContentTree").src = LinkFileName.replace("TreeVisible=&", "TreeVisible=coll&") + FrameName.replace("Tree", "") + "&framename=" + browseFrameName + "&AutoNav=" + szCollNav;
				                    folderAreaObj.document.getElementById("ContentTree").style.height = "99%";
				                    folderAreaObj.document.getElementById("ContentTree").style.height = "100%";
				                    szCollNav = "";
				                    bCollTreeLoaded = true;
				                }
				                break;
				            }
					    case ("LibraryTree"): {
						    if (bLibraryTreeLoaded == false) {
							    folderAreaObj.document.getElementById(FrameName).src = LinkFileName + FrameName.replace("Tree", "") + "&framename=" + browseFrameName + "&AutoNav=" + szLibNav;
							    folderAreaObj.document.getElementById(FrameName).style.height = "99%";
							    folderAreaObj.document.getElementById(FrameName).style.height = "100%";
							    szLibNav = "";
							    bLibraryTreeLoaded = true;
						    }
						    break;
					    }
					    case ("AdminTree"): {
						    if (bAdminTreeLoaded == false) {
							    folderAreaObj.document.getElementById(FrameName).src = "workareanavigationtrees.aspx?tree=" + FrameName.replace("Tree", "") + "&framename=" + browseFrameName + "&AutoNav=" + szAdminNav;
							    folderAreaObj.document.getElementById(FrameName).style.height = "99%";
							    folderAreaObj.document.getElementById(FrameName).style.height = "100%";
							    szAdminNav = "";
							    bAdminTreeLoaded = true;
						    }
						    break;
					    }
					    case ("ReportTree"): {
						    if (bReportTreeLoaded == false) {
							    folderAreaObj.document.getElementById(FrameName).src = 'workareanavigationtrees.aspx?tree=' + FrameName.replace("Tree", "") + "&framename=" + browseFrameName + "&AutoNav=" + szReportNav;
							    folderAreaObj.document.getElementById(FrameName).style.height = "99%";
							    folderAreaObj.document.getElementById(FrameName).style.height = "100%";
							    szReportNav = "";
							    bReportTreeLoaded = true;
						    }
						    break;
					    }
				    }
			    }

			    function Startup () {
				    m_loaded = true;

				    // First retrieve the mouseover event the writer of this file
				    // wants us to fire when the mouse comes back into the frame once it
				    // is collapsed.
				    m_OrigMouseOver = document.body.onmouseover;
				    m_OrigMouseOut = document.body.onmouseout;

				    // Are the tree Iframes availabe?
				    if (IsTreeLoaded() == false) {
					    // No so try again until they are
					    setTimeout("Startup()", 100);
				    }
				    else {
					    SetTreeVisible(szVisibleStartTree);
				    }
			    }

			    function IsTreeLoaded () {
				    // Find out if the requested folder tree is available.
				    // Assume tree is not ready.
				    var retValue = false;
				    // Get a reference to our Ifrme.
				    var localTreeObj = top.frames["ek_nav_bottom"]["NavIframeContainer"];

				    if (localTreeObj != null) {
					    // Get a reference to the frame that will hold the document (and therefor Iframes) of the trees
					    localTreeObj = localTreeObj.frames["nav_folder_area"];
					    if (localTreeObj != null) {
						    if ((typeof(localTreeObj.IsLoaded)).toLowerCase() != "undefined") {
							    if (localTreeObj.IsLoaded() == true) {
								    retValue = true;
							    }
						    }
					    }
				    }
				    return (retValue);
			    }

			    function SetTreeVisible (treeName, reload) {
				    if (treeName == null) {
					    treeName = "content";
				    }
				    treeName = treeName.toLowerCase();

				    if (reload == null) {
					    reload = false;
				    }
				    switch (treeName) {
					    case ("library") :
					    {
						    if (reload) {
							    bLibraryTreeLoaded = false;
						    }

						    top.MakeNavTreeVisible('LibraryTree');
						    break;
					    }

					    case ("admin") :
					    {
						    if (reload) {
							    bAdminTreeLoaded = false;
						    }

						    top.MakeNavTreeVisible('AdminTree');
						    break;
					    }
					    
					    case ("report") :
					    {
						    if (reload) {
							    bReportTreeLoaded = false;
						    }

						    top.MakeNavTreeVisible('ReportTree');
						    break;
					    }

		                case ("content"):
		                {
		                    if (reload) {
		                        bContentTreeLoaded = false;
		                    }

		                    if (typeof top.MakeNavTreeVisible == 'undefined') {
		                        alert('missing top.MakeNavTreeVisible function');
		                    } else {
		                        top.MakeNavTreeVisible('ContentTree');
		                    }
		                    break;
		                }
		                case ("tax"):
					    {
						    if (reload) {
							    bTaxTreeLoaded = false;
						    }

                            if (typeof top.MakeNavTreeVisible == 'undefined') {
                                alert('missing top.MakeNavTreeVisible function');
						    } else {
						        top.MakeNavTreeVisible('TaxTree');
						    }
						    break;
						}
		                case ("menu"):
		                    {
		                        if (reload) {
		                            bMenuTreeLoaded = false;
		                        }

		                        if (typeof top.MakeNavTreeVisible == 'undefined') {
		                            alert('missing top.MakeNavTreeVisible function');
		                        } else {
		                            top.MakeNavTreeVisible('MenuTree');
		                        }
		                        break;
		                    }
		                case ("coll"):
		                    {
		                        if (reload) {
		                            bCollTreeLoaded = false;
		                        }

		                        if (typeof top.MakeNavTreeVisible == 'undefined') {
		                            alert('missing top.MakeNavTreeVisible function');
		                        } else {
		                            top.MakeNavTreeVisible('CollTree');
		                        }
		                        break;
		                    }
				    }
			    }

			    function TreeNavigation(treeName, path) {
				    switch (treeName) {
					    case ("ContentTree"): {
						    szContentNav = path;
						    bContentTreeLoaded = false;
						    break;
					    }
	                    case ("TaxTree"): {
	                        szTaxNav = path;
		                    bTaxTreeLoaded = false;
		                    break;
		                }
		                case ("MenuTree"): 
		                {
		                    szMenuNav = path;
		                    bMenuTreeLoaded = false;
		                    break;
		                }
		                case ("CollTree"):
		                {
		                    szCollNav = path;
		                    bCollTreeLoaded = false;
		                    break;
		                }
					    case ("LibraryTree"): {
						    szLibNav = path;
						    bLibraryTreeLoaded = false;
						    break;
					    }
					    case ("AdminTree"): {
						    szAdminNav = path;
						    bAdminTreeLoaded = false;
						    break;
					    }
					    case ("ReportTree"): {
						    szReportNav = path;
						    bReportTreeLoaded = false;
						    break;
					    }
				    }
				    top.MakeNavTreeVisible(treeName);
			    }

			    function IsTreeVisible (TreeName) {
				    if (IsTreeLoaded(TreeName)) {
					    if (top.frames["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"].document.getElementById(TreeName).style.display != "") {
						    return (false);
					    }
				    }
				    return (true);
			    }

			    function LoadClearTree (TreeName) {
				    if (IsTreeLoaded(TreeName)) {
					    top.frames["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"].document.getElementById(TreeName).src = "blank.htm";
				    }
			    }

			    function ReloadTrees(treeNames) {
				    if (treeNames != null) {
					    if (treeNames.length > 0) {
						    treeNames = treeNames.toLowerCase();

						    treeNameArray = treeNames.split(",");

						    for (var iLoop = 0; iLoop < treeNameArray.length; iLoop++) {
							    switch (Trim(treeNameArray[iLoop])) {
								    case ("library") :
								    {
									    bLibraryTreeLoaded = false;
									    LoadClearTree('LibraryTree');
									    if (IsTreeVisible('LibraryTree')) {
										    top.MakeNavTreeVisible('LibraryTree');
									    }
									    break;
								    }

								    case ("admin") :
								    {
									    bAdminTreeLoaded = false;
									    LoadClearTree('AdminTree');
									    if (IsTreeVisible('AdminTree')) {
										    top.MakeNavTreeVisible('AdminTree');
									    }
									    break;
								    }

                                    case ("report") :
								    {
									    bReportTreeLoaded = false;
									    LoadClearTree('ReportTree');
									    if (IsTreeVisible('ReportTree')) {
										    top.MakeNavTreeVisible('ReportTree');
									    }
									    break;
								    }
								    case ("content") :
								    {
									    bContentTreeLoaded = false;
									    LoadClearTree('ContentTree');
									    if (IsTreeVisible('ContentTree')) {
										    top.MakeNavTreeVisible('ContentTree');
									    }
									    break;
								    }
							    }
						    }
					    }
				    }
			    }

			    function Trim (string) {
				    if (string.length > 0) {
					    string = RemoveLeadingSpaces (string);
				    }
				    if (string.length > 0) {
					    string = RemoveTrailingSpaces(string);
				    }
				    return string;
			    }

			    function RemoveLeadingSpaces(string) {
				    while(string.substring(0, 1) == " ") {
					    string = string.substring(1, string.length);
				    }
				    return string;
			    }

			    function RemoveTrailingSpaces(string) {
				    while(string.substring((string.length - 1), string.length) == " ") {
					    string = string.substring(0, (string.length - 1));
				    }
				    return string;
			    }

			    function IsLoaded() {
				    return (m_loaded);
			    }

			    function CanShowNavTree() {
				    if (typeof(top.CanShowNavTree) == "function") {
					    return (top.CanShowNavTree());
					    }
				    return (true);
			    }
			Ektron.ready(function() {
				    if ("undefined" !== typeof (top.Ektron.Workarea.Height)) {
				        top.Ektron.Workarea.Height.heightFix(function(height) {
				            $ektron("html, body, iframe").height(height);
				        });
				        top.Ektron.Workarea.Height.execute();
				    }
				});
            //--><!]]>
    </script>

    <style type="text/css">
    html,body
    {
        width:100%;
            height:100%;
            margin:0px;
            padding:0px;
    }
    </style>
</head>
<body class="UiNavigation" onmouseover="StartEnlarge(event);" onmouseout="StartShrink(event);"
    onload="Startup();">
    <div id="div2" style="display: block; left: 0px; position: relative">
        <iframe scrolling="auto" id="NavIframeContainer" name="NavIframeContainer" src="navframes.htm"
            height="100%" frameborder="0"></iframe>
    </div>
</body>
</html>
