
// Definition of Folder class
// *****************************************************************

// Global variables
// ****************

ekFolderFontSize = 0;
ekFolderCreateTextLinks = 0;
ekFolderIndexEntries = new Array;
ekFolderNumberOfEntries = 0;
doc = document;
ekBV = 0;
ekFolderImagePath = "";
imageNumber = 0;
hyperNumber = 0;
// these are images for open/closed folders
// 0 = default, 1 = blog folder, 2 = domain folder
var ekFolderOimg = new Array(10);
ekFolderOimg[0] = "folderOpen.png";
ekFolderOimg[1] = "folderBlogOpen.png";
ekFolderOimg[2] = "folderSiteOpen.png";
ekFolderOimg[3] = "folderBoardOpen.png";
ekFolderOimg[4] = "folderBoardOpen.png";
ekFolderOimg[5] = "root.png";
ekFolderOimg[6] = "folderCommunityOpen.png";
ekFolderOimg[7] = "folderFilmOpen.png";
ekFolderOimg[8] = "folderCalendarOpen.png"; // calendar
ekFolderOimg[9] = "folderGreenOpen.png";
var ekFolderCimg = new Array(10);
ekFolderCimg[0] = "folderClosed.png";
ekFolderCimg[1] = "folderBlogClosed.png";
ekFolderCimg[2] = "folderSiteClosed.png";
ekFolderCimg[3] = "folderBoardClosed.png";
ekFolderCimg[4] = "folderBoardClosed.png";
ekFolderCimg[5] = "root.png";
ekFolderCimg[6] = "folderCommunityClosed.png";
ekFolderCimg[7] = "folderFilmClosed.png";
ekFolderCimg[8] = "folderCalendarClosed.png"; // calendar
ekFolderCimg[9] = "folderGreenClosed.png";

// Main functions
// *************

function CFI2(description, urlArray, foldertype) {
	return CFI(description, urlArray, foldertype);
}

function CFI(description, urlArray, foldertype) {
	return CreateFolderInstance(description, urlArray, foldertype);
}

function eIF(parentFolder, childFolder) {
	return InsertFolder(parentFolder, childFolder);
}

function InitializeFolderControl() {
	imageNumber = doc.images.length;
	if (doc.all) {
		ekBV = 1; //IE4
	}
	else if (doc.layers) {
		ekBV = 2; //NS4
	}
    else if (!doc.all && doc.getElementById) {
		ekBV = 3; //NS6
	}
	else {
		ekBV = 0; //other
	}
	//foldersTree (with the site's data) is created in an external .js
	TopTreeLevel.initialize(0, 1, "");

	if (ekBV == 2) {
		doc.write("<layer top=" + ekFolderIndexEntries[ekFolderNumberOfEntries-1].navObj.top + ">&nbsp;</layer>");
	}

	//The tree starts in full display
	if (ekBV > 0) {
		// open the root folder
		clickOnNode(0);
	}
}

function CreateFolderInstance2(description, urlArray, foldertype) {
	return CreateFolderInstance(description, urlArray, foldertype);
}

function CreateFolderInstance(description, urlArray, foldertype) {
	// for compatibility w/ old code..we'd use different functions if Javascript supported function overloading
	if (foldertype == undefined)  foldertype= 0;

	var completeUrl = "";
	var CustomEvents = "";
	var urlNumber = 0;

	for (var i = 0; i < (urlArray.length / 3); i++) {
		urlNumber = (i * 3);
		if (urlArray[urlNumber + 1] != "") {
			if (urlArray[urlNumber].toLowerCase() == "event") {
				CustomEvents += " " + urlArray[urlNumber + 1] + "=\"" + urlArray[urlNumber + 2] + "\";";
			}
			else if (urlArray[urlNumber].toLowerCase() == "javascript") {
				completeUrl += urlArray[urlNumber + 1] + ";";
			}
			else if (urlArray[urlNumber].toLowerCase() == "window") {
				completeUrl += "ekFolderOpenNewLinkWindow('" + urlArray[urlNumber + 1] + "', '" + urlArray[urlNumber + 2] + "');";
			}
			else {
				completeUrl += "ekFolderOpenNewLinkFrame('" + urlArray[urlNumber + 1] + "', '" + urlArray[urlNumber + 2] + "');";
			}
		}
	}
	if ((ekFolderMaxDescriptionLength != 0) && (description.length > ekFolderMaxDescriptionLength)) {
		description = description.substring(0, ekFolderMaxDescriptionLength - 1) + "...";
	}
	folder = new ekFolderCreateFolder(description, completeUrl, CustomEvents, foldertype);
	return folder;
}

function InsertFolder(parentFolder, childFolder) {
	return parentFolder.addChild(childFolder);
}

function InsertFile(parentFolder, file) {
	parentFolder.addChild(file);
}


function CreateLink(description, urlArray) {
	var completeUrl = "";
	var CustomEvents = "";
	var urlNumber = 0;

	for (var i = 0; i < (urlArray.length / 3); i++) {
		urlNumber = (i * 3);
		if (urlArray[urlNumber + 1] != "") {
			if (urlArray[urlNumber].toLowerCase() == "event") {
				CustomEvents += " " + urlArray[urlNumber + 1] + "=\"" + urlArray[urlNumber + 2] + "\";";
			}
			else if (urlArray[urlNumber].toLowerCase() == "javascript") {
				completeUrl += urlArray[urlNumber + 1] + ";";
			}
			else if (urlArray[urlNumber].toLowerCase() == "window") {
				completeUrl += "javascript:ekFolderOpenNewLinkWindow('" + urlArray[urlNumber + 1] + "', '" + urlArray[urlNumber + 2] + "');";
			}
			else {
				completeUrl += "javascript:ekFolderOpenNewLinkFrame('" + urlArray[urlNumber + 1] + "', '" + urlArray[urlNumber + 2] + "');";
			}
		}
	}
	if ((ekFolderMaxDescriptionLength != 0) && (description.length > ekFolderMaxDescriptionLength)) {
		description = description.substring(0, ekFolderMaxDescriptionLength - 1) + "...";
	}
	linkItem = new ekFolderCreateItem(description, completeUrl, CustomEvents);
	return linkItem;
}

function ekFolderCreateFolder(folderDescription, hreference, CustomEvents, folderType) {
	 //constant data
	 this.desc = folderDescription;
	 this.hreference = hreference;
	 this.customevents = CustomEvents;
	 this.id = -1;
	 this.navObj = null;
	 this.iconImg = 0;
	 this.nodeImg = 0;
	 this.isLastNode = 0;
	 this.nodeId = -1;
	 this.folderId = -1;
	 this.nodeLinkId = -1;
	 this.nodeLink = "";
	 this.folderLinkId = -1;
	 this.textLinkId = -1;
	 this.type = folderType;

	 //dynamic data
	 this.isOpen = (0);
	 this.iconSrc = ekFolderImagePath + ekFolderCimg[folderType];
	 this.children = new Array;
	 this.nChildren = 0;

	 //methods
	 this.initialize = ekFolderInitializeFolder;
	 this.setState = ekFolderSetStateFolder1;
	 this.noRecursiveSetState = ekFolderSetStateFolder;
	 this.addChild = ekFolderAddChild;
	 this.createIndex = ekFolderCreateEntryIndex;
	 this.escondeBlock = ekFolderEscondeBlock;
	 this.esconde = ekFolderEscondeFolder;
	 this.mostra = mostra;
	 this.renderOb = ekFolderDrawFolder;
	 this.totalHeight = ekFolderTotalHeight;
	 this.subEntries = ekFolderSubEntries;
	 this.outputLink = ekFolderOutputFolderLink;
	 this.blockStart = ekFolderBlockStart;
	 this.blockEnd = ekFolderBlockEnd;
	 this.initVars = ekInitializeFolderObjs;
	 this.inited = false;
}

function ekFolderInitializeFolder(level, lastNode, leftSide) {
	var j = 0;
	var i = 0;
	var numberOfFolders;
	var numberOfDocs;
	var nc;

	nc = this.nChildren;

	this.createIndex();

	var auxEv = "";

	this.nodeLinkId = hyperNumber++;
	this.nodeLink = "javascript:clickOnNode(" + this.id + ");return false;";
	if (ekBV > 0) {
		auxEv = "<a href=\"#\">";
	}
	else {
		auxEv = "<a>";
	}
	if (ekBV == 2) {
		imageNumber = 0;
	}
	if (level > 0) {

			imageNumber += level - 1;
			this.nodeId = imageNumber++;
			this.renderOb(leftSide + auxEv + "<img src='" + ekFolderImagePath + "plus.png' width='9' height='16' border='0'></a>");
			leftSide = leftSide + "<img src='" + ekFolderImagePath + "blank.png' width='16' height='16' border='0'>";
			this.isLastNode = 0;

	}
	else {
		this.nodeId = 0;
		hyperNumber--;
		this.renderOb("");
	}
	if (nc > 0) {
		level = level + 1;
		for (i= 0; i < this.nChildren; i++) {
			if (i == this.nChildren-1) {
				this.children[i].initialize(level, 1, leftSide);
			}
			else {
				this.children[i].initialize(level, 0, leftSide);
			}
		}
	}
}

function ekFolderSetStateFolder(isOpen) {
	this.setState(isOpen);
	if (ekBV == 2)  {
		var MyNew = this.navObj.pageY + this.navObj.clip.height;
		subEntries = this.subEntries();
		for (fIt = this.id + 1; fIt < ekFolderNumberOfEntries; fIt++) {
			if (ekFolderIndexEntries[fIt].navObj.visibility == "show") {
				ekFolderIndexEntries[fIt].navObj.moveTo(0, MyNew);
				MyNew += ekFolderIndexEntries[fIt].navObj.clip.height;
			}
		}
	}
}

function ekFolderSetStateFolder1(isOpen) {

	if (isOpen == this.isOpen) {
		return;
	}

	this.isOpen = isOpen;
	ekFolderPropagateChangesInState(this);
}

function ekFolderPropagateChangesInState(folder) {
	var i = 0;

	if (folder.id == 0) {
		if (folder.inited == false) {
			folder.initVars(1);
		}
		folder.mostra();
	}
	if (folder.isOpen) {
		if (folder.inited == false) {
			folder.initVars(1);
		}
		if (folder.nodeImg) {

				folder.nodeImg.src = ekFolderImagePath + "minus.png";

		}
		if( folder.id != 0 ) {
		    folder.iconImg.src = ekFolderImagePath + ekFolderOimg[folder.type];
		}
		else {
		    folder.iconImg.src = ekFolderImagePath + ekFolderOimg[5];
		}
		for (i = 0; i < folder.nChildren; i++) {
			if (folder.children[i].navObj == null) folder.children[i].initVars(0);
			folder.children[i].mostra();
		}
	}
	else {
		if (folder.nodeImg) {

				folder.nodeImg.src = ekFolderImagePath + "plus.png";

		}
		folder.iconImg.src = ekFolderImagePath + ekFolderCimg[folder.type];
		for (i = 0; i < folder.nChildren; i++) {
			if (folder.children[i].navObj) {
				folder.children[i].esconde();
			}
		}
	}
}

function ekFolderEscondeFolder() {
	this.escondeBlock();
	this.setState(0);
}

function ekFolderDrawFolder(leftSide) {

	if (ekBV == 2) {
		if (!doc.yPos) {
			doc.yPos = 20;
		}
	}
	this.folderId = imageNumber++;
	var localString = this.blockStart("folder") + "" + leftSide + this.outputLink(true) + "<img class='type' src='" + this.iconSrc + "'></a>";

		var startFont = "";
		var stopFont = "";
	if (ekFolderCreateTextLinks) {
		localString += this.outputLink(false) + startFont + this.desc + stopFont + "</a>";
	}
	else {
		localString += startFont + this.desc + stopFont;
	}
	localString += "" + this.blockEnd();
	doc.write(localString);

	if (ekBV == 2) {
		this.navObj = doc.layers["folder" + this.id];
		this.iconImg = this.navObj.document.images[this.folderId];
		this.nodeImg = this.navObj.document.images[this.nodeId];
		doc.yPos = doc.yPos+this.navObj.clip.height;
		this.inited = true;
	}
}

function ekInitializeFolderObjs(initType)  {
	if (ekBV == 1) {
		this.navObj = doc.all["folder" + this.id];
		if (this.id > 0) {
			doc.links[this.nodeLinkId].onclick = new Function(this.nodeLink);
		}
		if (this.textLinkId > 0) {
			doc.links[this.iconLinkId].onclick = new Function(this.hreference + ";ekFolderClickedOnFolder(" + this.id + ");" + this.customevents + ";return false;");
		}
		if (this.textLinkId > 0) {
			doc.links[this.textLinkId].onclick = new Function(this.hreference + ";ekFolderClickedOnFolder(" + this.id + ");" + this.customevents + ";return false;");
		}
		if (initType != 0) {
			this.iconImg = doc.images[this.folderId];
			this.nodeImg = doc.images[this.nodeId];
			this.inited = true;
		}
	}
	else if (ekBV == 2) {
		this.navObj = doc.layers["folder" + this.id];
		doc.yPos = doc.yPos+this.navObj.clip.height;
		if (initType != 0) {
			this.iconImg = this.navObj.document.images[this.folderId];
			this.nodeImg = this.navObj.document.images[this.nodeId];
			this.inited = true;
		}
	}
	else if (ekBV == 3) {
		this.navObj = doc.getElementById("folder" + this.id);
		if (this.id > 0) {
			doc.links[this.nodeLinkId].onclick = new Function(this.nodeLink);
		}
		if (this.iconLinkId > 0) {
			doc.links[this.iconLinkId].onclick = new Function(this.hreference + ";ekFolderClickedOnFolder(" + this.id + ");" + this.customevents + ";return false;");
		}
		if (this.textLinkId > 0) {
			doc.links[this.textLinkId].onclick = new Function(this.hreference + ";ekFolderClickedOnFolder(" + this.id + ");" + this.customevents + ";return false;");
		}
		if (initType != 0) {
			this.iconImg = doc.images[this.folderId];
			this.nodeImg = doc.images[this.nodeId];
			this.inited = true;
		}
	}
}

function ekFolderOutputFolderLink(Folder) {

	if ((Folder == true) && (this.hreference))  {
		this.iconLinkId = hyperNumber++;
	}
	else if ((Folder == false) && (this.hreference)){
		this.textLinkId = hyperNumber++;
	}
	if (this.hreference) {
		if (ekBV == 0) {
			var localString = "<a href=\"" + this.hreference + "\" TARGET=\"basefrm\" >";
		}
		else {
			var localString = "<a href=\"#\">";
		}
	}
	else {
		var localString = "<a onmouseover=\"this.className='hover'\" onmouseout=\"this.className=''\">";
	}
	return localString;
}

function ekFolderAddChild(childNode) {
	this.children[this.nChildren] = childNode;
	this.nChildren++;
	return childNode;
}

function ekFolderSubEntries() {
	var i = 0;
	var se = this.nChildren;

	for (i = 0; i < this.nChildren; i++){
		if (this.children[i].children) { //is a folder
			se = se + this.children[i].subEntries();
		}
	}
	return se;
}


// Definition of class Item (a document or link inside a Folder)
// *************************************************************

function ekFolderCreateItem(itemDescription, itemLink, CustomEvents) {  // Constructor
	// constant data
	this.desc = itemDescription;
	this.link = itemLink;
	this.customevents = CustomEvents;
	this.id = -1; //initialized in initalize()
	this.navObj = 0; //initialized in render()
	this.iconImg = 0; //initialized in render()
	this.iconSrc =  ekFolderImagePath + "e.png";
	this.nodeId = -1;
	this.nodeLinkId = -1;
	this.iconLinkId = -1;
	this.itemId = -1;

	// methods
	this.initialize = ekInitializeItem;
	this.createIndex = ekFolderCreateEntryIndex;
	this.esconde = ekFolderEscondeBlock;
	this.mostra = mostra;
	this.renderOb = ekFolderDrawItem;
	this.totalHeight = ekFolderTotalHeight;
	this.blockStart = ekFolderBlockStart;
	this.blockEnd = ekFolderBlockEnd;
	this.initVars = ekInitializeItemObjs;
}

function ekInitializeItem(level, lastNode, leftSide) {
	this.createIndex();

	if (level > 0) {

			imageNumber += level - 1;
			this.nodeId = imageNumber++;
			this.renderOb(leftSide + "<img src='" + ekFolderImagePath + "blank.png' width='16' height='16' border='0'>");
			leftSide = leftSide + "<img src='" + ekFolderImagePath + "blank.png' width='16' height='16' border='0'>";
	}
	else {
		this.renderOb("");
	}
}

function ekFolderDrawItem(leftSide) {

	this.itemId = imageNumber++;
	this.folderLinkId = hyperNumber++;
	var localString = this.blockStart("item") + "" + leftSide + "<a href='#' onClick=\"" + this.link + ";return false;\"" + this.customevents + "><img src='" + this.iconSrc + "'></a>";
		var startFont = "";
		var stopFont = "";
	if (ekFolderCreateTextLinks) {
		this.textLinkId = hyperNumber++;
		localString += "<a href=\"#\" onClick=\"" + this.link + ";return false;\"" + this.customevents + ">" + startFont + this.desc + stopFont + "</a>";
	}
	else {
		localString += startFont + this.desc + stopFont;
	}
	localString +=  "" + this.blockEnd();
	doc.write(localString);
	this.initVars(1);
}

function ekInitializeItemObjs(initType) {
	if (ekBV == 1) {
		this.navObj = doc.all["item" + this.id];
	}
	else if (ekBV == 2) {
		this.navObj = doc.layers["item" + this.id];
		doc.yPos = doc.yPos+this.navObj.clip.height;
	}
	else if (ekBV == 3) {
		this.navObj = doc.getElementById("item" + this.id);
	}
}


// Methods common to both objects (pseudo-inheritance)
// ********************************************************

function mostra() {
	if (ekBV == 1 || ekBV == 3) {
		this.navObj.style.display = "block";
	}
	else {
		this.navObj.visibility = "show";
	}
}

function ekFolderEscondeBlock() {
	if (ekBV == 1 || ekBV == 3) {
		if (this.navObj.style.display == "none") {
			return;
		}
		this.navObj.style.display = "none";
	}
	else {
		if (this.navObj.visibility == "hidden") {
			return;
		}
		this.navObj.visibility = "hidden";
	}
}

function ekFolderBlockStart(idprefix) {
	var localString = "";
	var idParam = "id='" + idprefix + this.id + "'";

	if (ekBV == 2) {
		localString += "<layer "+ idParam + " top=" + doc.yPos + " visibility='hidden'>";
	}
	else if (ekBV == 3) { //N6 has bug on display property with tables
		localString += "<div " + idParam + " style='display:none;'>";
	}

	localString += "<li cellpadding='0'";

	if (ekBV == 1) {
		localString += idParam + " style='display:none;'>";
	}
	else {
		localString += ">";
	}
	return localString;
}

function ekFolderBlockEnd() {
	var localString = "</li>";

	if (ekBV == 2) {
		localString += "</layer>";
	}
	else if (ekBV == 3) {
		localString += "</div>";
	}
	return localString;
}

function ekFolderCreateEntryIndex() {
	this.id = ekFolderNumberOfEntries;
	ekFolderIndexEntries[ekFolderNumberOfEntries] = this;
	ekFolderNumberOfEntries++;
}

// total height of subEntries open
function ekFolderTotalHeight() {  //used with ekBV == 2
	var h = this.navObj.clip.height;
	var i = 0;

	if (this.isOpen) {  //is a folder and _is_ open
		for (i = 0; i < this.nChildren; i++) {
			h = h + this.children[i].totalHeight();
		}
	}
	return h;
}


// Events
// *********************************************************

function ekFolderClickedOnFolder(folderId) {
	var clicked = ekFolderIndexEntries[folderId];

	if (typeof(top.CanNavigate) == "function") {
		if (!top.CanNavigate())
			return;
	}

	if (!clicked.isOpen) {
		clickOnNode(folderId);
	}
	return;
}

function clickOnNode(folderId) {
	var clickedFolder = 0;
	var state = 0;

	if (typeof(top.CanNavigate) == "function") {
		if (!top.CanNavigate())
			return;
	}
	clickedFolder = ekFolderIndexEntries[folderId];

	state = clickedFolder.isOpen;

	clickedFolder.noRecursiveSetState(!state); //open<->close
}

function clickOpenNode(folderId) {
	var clickedFolder = 0;
	var state = 0;

	if (typeof(top.CanNavigate) == "function") {
		if (!top.CanNavigate())
			return;
	}
	clickedFolder = ekFolderIndexEntries[folderId];

	if (clickedFolder.isOpen)
	    return;

	clickedFolder.noRecursiveSetState(!state); //open<->close
}


function ekFolderOpenNewLinkFrame (url, frame) {
	if (typeof(top.CanNavigate) == "function") {
		if (!top.CanNavigate())
			return;
	}
	return top[frame].location.href = url;
}

function ekFolderOpenNewLinkWindow (url, windowName) {
	if (typeof(top.CanNavigate) == "function") {
		if (!top.CanNavigate())
			return;
	}
	return window.open(url, windowName);
}

var MyTimer;

function OpenFolder (folderPath,DoHref) {
	var regexp1 = /\\/gi;
	var regexp2 = /\'/gi;
	folderPath = folderPath.replace(regexp1,"\\\\");
	folderPath = folderPath.replace(regexp2, "\\'");
	MyTimer = setTimeout("OpenFolderCallBack('" + folderPath + "'," + DoHref + " )", 100);
	return;
}

function DelayAutoClick(Hreference) {
	eval(Hreference);
}

function OpenFolderCallBack (folderPath, DoHref) {
	var folders = "";
	var lLevel = 0;
	var fObj = "";
	var lMyChildren;

	clearTimeout(MyTimer);
	folderPath = folderPath.toLowerCase();
	if (folderPath.substring(0, 1) == "\\") {
		folderPath = folderPath.substring(1, folderPath.length);
	}

	folders = folderPath.split("\\");
	if (folders.length == 0) {
		alert ("bad folder");
		return;
	}
	else if ((folders.length == 1) && (folders[0].length == 0)) {
		// if they are asking for the root folder than we just want to fire the href to the top folder.
		folders = new Array();
	}
	lLevel = 0;
	fObj = ekFolderIndexEntries[lLevel];
	for (var lLoop = 0; lLoop < folders.length; lLoop++) {
		var fFound = lLevel;
		for (lMyChildren = 0; lMyChildren < fObj.nChildren; lMyChildren++) {
			if (fObj.children[lMyChildren].desc.toLowerCase() == folders[lLoop]) {
				lLevel = fObj.children[lMyChildren].id;
				clickOpenNode(lLevel);
				fObj = fObj.children[lMyChildren];
				break;
			}
		}
		if ((folders[lLoop] != '') && (fFound == lLevel)) {
			alert("bad folder=" + folders[lLoop]);
			return;
		}
	}
	if (DoHref) {
		var regexp1 = /\\/gi;
		regexp2 = /\'/gi;
		tmpHref = fObj.hreference.replace(regexp1, "\\\\");
		var tmpHref = tmpHref.replace(regexp2, "\\'");
		setTimeout("DelayAutoClick('" + tmpHref +"')", 600);
	}
}
