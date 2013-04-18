function fcNode(folderDescription, hreference, CustomEvents)
{
this.desc = folderDescription;
this.hreference = hreference;
this.customevents = CustomEvents;
this.id = -1;
this.navObj = null;
this.divObj = null;
this.iconImg = null;
this.nodeImg = null;
this.isLastNode = 0;
this.openIcon = "";
this.closedIcon = "";
this.openIconOver = "";
this.closedIconOver = "";
this.statusText = "";
this.hidden = false;
this.userDef = "";
this.isOpen = false;
this.c = new Array();
this.nC = 0;
this.nodeLeftSide = "";
this.nodeLevel = 0;
this.nodeParent = 0;
this.isInitial = false;
this.isFolder = true;
this.initialize = fcInitialize;
this.setState = fcSetState;
this.moveState = fcMoveStateFolder;
this.addChild = fcAddChild;
this.createIndex = fcCreateEntryIndex;
this.hide = fcHide;
this.display = fcDisplay;
this.initMode = fcInitMode;
this.collExp = fcCollExp;
this.initLayer = fcInitLayer;
this.setNodeDraw = fcSetNodeDraw;
this.setInitial = fcSetInitial;
this.setIcon = fcSetIcon;
this.setStatusBar = fcSetStatusBar;
this.setUserDef = fcSetUserDef;
this.getUserDef = fcGetUserDef;
this.setNodeFont = fcSetNodeFont;
this.nodeIcon = fcNodeIcon;
this.nodeTIcon = fcNodeTIcon;

}
function fcSetState(isOpen)
{
var totalHeight;
var fIt = 0;
var i=0;
this.isOpen = isOpen;
if (bv > 0)
	fcChangeState(this);
}
function fcMoveStateFolder(isOpen)
{
var totalHeight;
var fIt = 0;
var i=0;
var j=0;
var subopen = 1;
var parent = 0;
var thisnode = 0;
var found = false;
var width = 0;
totalHeight = 0;
for (i=0; i < this.nC; i++)
	{
	if (!noDocs || this.c[i].isFolder)
		{
		totalHeight += this.c[i].navObj.clip.height;
		if (isOpen)
			width = Math.max(width,this.c[i].navObj.clip.width);
		}
	}
if (!isOpen) totalHeight = - totalHeight;
if (isOpen && noFrame && cascade)this.divObj.clip.height = totalHeight;
if (!(noFrame && cascade))this.navObj.clip.height +=  totalHeight;
if (isOpen && !(noFrame && cascade)) this.navObj.clip.width = Math.max(width, this.navObj.clip.width);
thisnode = this;
parent = thisnode.nodeParent;
for (i=0; i < this.nodeLevel ; i++)
	{
if (parent.nodeLevel == 0 || !(noFrame && cascade)) parent.navObj.clip.height +=  totalHeight;
	if (isOpen)
		parent.navObj.clip.width = Math.max(width, parent.navObj.clip.width);
	found = false;
	for (j=0; j < parent.nC; j++)
		{
		if (!noDocs || parent.c[j].nC != null)
			{
			if (found && !(noFrame && cascade)) parent.c[j].navObj.moveBy(0,totalHeight);
			else if (parent.c[j] == thisnode) found = true;
			}
		}
	thisnode = parent;
	parent = thisnode.nodeParent;
	}
newHeight= fC.navObj.clip.height + topLayer.layers["header"].clip.height + topLayer.layers["footer"].clip.height;
topLayer.clip.height = newHeight;
topLayer.clip.width = Math.max(topLayer.clip.width,fC.navObj.clip.width);
newHeight = newHeight + topGap;
if (isOpen && !noFrame){
	frameHeight = thisFrame.innerHeight;
	if (doc.height < newHeight)
		doc.height = newHeight;
	else if (newHeight < frameHeight) {
		doc.height = frameHeight;
		thisFrame.scrollTo(0,0);
	}
       else if (doc.height > newHeight + 0.5*frameHeight){
		doc.height = doc.height*0.5 + (newHeight + 0.5*frameHeight)*0.5;
		}
}
if (!(noFrame && cascade)) topLayer.layers["footer"].top = topLayer.layers["footer"].top + totalHeight;
}
function fcChangeState(folder)
{
var i=0;
if (treeLines == 1) 
{
if (!folder.nodeImg)
 {
 if (bv == 2) folder.nodeImg = folder.navObj.document.images["treeIcon"+folder.id];
 else if (bv == 1 || doc.images) folder.nodeImg = doc.images["treeIcon"+folder.id];
 }
if (folder.nodeLevel > 0) folder.nodeImg.src = folder.nodeTIcon();
}
if (folder.isOpen && folder.isInitial)
	{
	if (cascade && noFrame && bv == 1) doc.all["div"+folder.id].style.display = "block";
	if (cascade && noFrame && bv == 2) folder.divObj.visibility = "show";
	for (i=0; i<folder.nC; i++) {
		if (!noDocs || folder.c[i].isFolder)
			folder.c[i].display();
	}
	}
else
	{
	if (folder.isInitial){
		if (cascade && noFrame && bv == 1) doc.all["div"+folder.id].style.display = "none";
		if (cascade && noFrame && bv == 2) folder.divObj.visibility = "hidden";
		for (i=0; i<folder.nC; i++) 
			if (!noDocs || folder.c[i].isFolder)
				folder.c[i].hide();
	}
	}
var iA = iNA;
if (!folder.iconImg)
{
if (bv == 2) folder.iconImg = folder.navObj.document.images["nodeIcon"+folder.id];
else if (bv == 1 || doc.images) folder.iconImg = doc.images["nodeIcon"+folder.id];
}
folder.iconImg.src = folder.nodeIcon("",iA);
}
function fcDisplay()
{
var i=0;
if (bv == 1)
{
	if (!this.navObj) this.navObj = doc.all["node" + this.id];
	this.navObj.style.display = "block";
}
else if (bv ==2)
	this.navObj.visibility = "show";
if (bv == 2 && cascade && noFrame && this.divObj && (this.isOpen || !this.isFolder)) {
	this.divObj.visibility = "show";
}
if (bv == 1 || (bv == 2 && cascade && noFrame)) {
if (this.isInitial && this.isOpen)

	for (i=0; i < this.nC; i++) {
		if (!noDocs || this.c[i].isFolder || (bv ==2 && cascade && noFrame))
			this.c[i].display();
}
}
}
function fcHide()
{
var i = 0;
if (bv == 1)
{
	if (!this.navObj) this.navObj = doc.all["node" + this.id];
	this.navObj.style.display = "none";
}
else if (bv ==2)
	this.navObj.visibility = "hidden";
if (bv == 2 && cascade && noFrame && this.divObj) {
	this.divObj.visibility = "hidden";
}
if (bv == 1 || (bv == 2 && cascade && noFrame) ) {
if (this.isInitial)
	for (i=this.nC-1; i>-1; i--)
		{
		if (!noDocs || this.c[i].isFolder)
			this.c[i].hide();
		}
}
}
function fcInitialize(level, lastNode, leftSide, doc, prior)
{
this.createIndex();
this.nodeLevel = level;
if(!this.isFolder) this.isInitial = true;
if (level>0)
	{
	this.isLastNode = lastNode;
	if (!(cascade && noFrame)) tmpIcon = this.nodeTIcon();
	tmpIcon = this.nodeTIcon();
	if (this.isLastNode == 1)
		tmp2Icon = iTA["b"].src;
	else
		tmp2Icon = iTA["vl"].src;
	if (treeLines == 0) tmp2Icon = iTA["b"].src;
	if (this.hidden == false) 
		{
		if (level == 1 && treeLines == 0 && noTopFolder)
			this.setNodeDraw(leftSide, doc, prior);
		else
			{
			if (this.isFolder){
				auxEv = "<a href='javascript:void(0);' onClick='return " + frameParent + ".clickOnNode("+this.id+");'";
				auxEv += " onMouseOver='return " + frameParent + ".mouseOverNode(0,"+this.id+");'";
				auxEv += " onMouseOut='return " + frameParent + ".mouseOutNode(0,"+this.id+");' alt=''>";
				if (!(cascade && noFrame)) auxEv += "<img name='treeIcon" + this.id + "' src='" + tmpIcon + "' border=0 alt='' align = 'absmiddle'></a>";
			}
			else {
				if (!(cascade && noFrame)) auxEv = "<img src='" + tmpIcon + "'  border=0  align = 'absmiddle'>";
			}
			this.setNodeDraw(leftSide + auxEv, doc, prior);
			if (this.isFolder && !(cascade && noFrame))
				leftSide +=  "<img src='" + tmp2Icon + "'  border=0  align = 'absmiddle'>";
			}
		}
	}
else
	this.setNodeDraw("", doc, prior);
if (this.isFolder) {
this.nodeLeftSide = leftSide;
if (this.nC > 0 && this.isInitial)
	{
	level = level + 1;
	for (var i=0 ; i < this.nC; i++)
		{
		this.c[i].nodeParent = this;
		if (noDocs)
			{
			newLastNode = 1;
			for (var j=i+1; j < this.nC; j++)
				if (this.c[j].isFolder) newLastNode = 0;
			}
		else
			{
			newLastNode = 0;
			if (i == this.nC-1) newLastNode = 1;
			}
		if (i==0 && level == 1 && noTopFolder) newLastNode = -1;
		if (!noDocs || this.c[i].isFolder)
			{
			this.c[i].initialize(level, newLastNode, leftSide, doc, prior);
			}
		}
	}
}
if (bv == 2 && this.hidden == false && !prior) doc.write("</layer>");
}

function fcSetNodeDraw(leftSide,doc,prior)
{
var strbuf = "";
var font;
if (bv == 2)
	{
	if (!prior)
		{
		if (noFrame && cascade) 
			strbuf += "<layer id='node" + this.id + "' visibility='hidden'  bgColor='" + menuBackColor + "' width = '" + menuWidth + "' Z-INDEX=1>";
		else
			strbuf += "<layer id='node" + this.id + "' visibility='hidden' Z-INDEX=1>";

		}
	else
		{
		if (noWrap) layWidth = menuWidth;
		else layWidth = thisFrame.innerWidth - 20;
		if (noFrame && cascade) layWidth = menuWidth;
		var testlayer = new Layer(layWidth, prior);
		}
	}
fullLink = "";

if (this.hreference)
	{

	fullLink = " href='#' onClick=\"" + this.hreference + ";return false;\"" + this.customevents;
	}
else
	fullLink = " href='javascript:void(0); ' ";
fullLink += " onMouseOver='return " + frameParent + ".mouseOverNode(1,"+this.id+");' onMouseOut='return " + frameParent + ".mouseOutNode(1,"+this.id+");' ";
	fullLink += " onClick='return " + frameParent + ".clickNode("+this.id+");' ";
if (this.statusText == "")
	var toolTip= this.desc;
else
	var toolTip = this.statusText;
if (bv > 0) {
	eval("toolTip = toolTip.replace(/<[^<>]*>/g,'');");
}
else {
	int1 = toolTip.indexOf("<");
	toolTip = toolTip.substring(0,int1);
}
fullLink += " TITLE ='" + toolTip + "' ";

if (bv == 1)
	strbuf += "<div id='node" + this.id + "' style='position:static'>";
if (bv == 2 && noFrame && cascade) 
	strbuf += "<table border=0 cellspacing=0 cellpadding=0 width='" + menuWidth + "'><tr><td valign = ";
else 
	strbuf += "<table border=0 cellspacing=0 cellpadding=0><tr><td valign = ";
if (noWrap) strbuf += " 'middle' ";
else strbuf += " 'top' ";
strbuf += " nowrap>" + leftSide + "<a " + fullLink + "><img name='nodeIcon" + this.id + "' ";
var iA = iNA;
tmpIcon = this.nodeIcon("",iA);
strbuf += "src='" + tmpIcon + "' border=0 align = 'absmiddle' alt = '" + toolTip + "'>";
if (this.isFolder)
	var space = folderIconSpace;
else
	var space = documentIconSpace;
if (space > 0) {
hspace = parseInt("" + (space/2 + .5) + "");
wspace = 1;
if (hspace*2 == space) wspace = 2;
hspace = hspace - 1; 
 strbuf += "<IMG border=0 align = 'absmiddle' height = '" + wspace + "' width = '" + wspace + "' src='" + iTA["b"].src + "' hspace = '" + hspace + "'>";
}
if (!(cascade && noFrame)) 
strbuf += "</a></td><td valign=middle ";
else {strbuf += "</a></td><td valign=middle width = '" + textWidth + "' ";}

if (noWrap) strbuf += "nowrap>";
else strbuf += ">";
font = this.setNodeFont();
if (ekFolderCreateTextLinks && this.hreference)
strbuf += "<a " + fullLink + " >" + font + this.desc + "</font></a>";
else {strbuf += font + this.desc + "</font>";}
if (!(cascade && noFrame)) {
strbuf += "</td></tr></table>";
} else {
strbuf += "</td><td align=right>";
if (this.isFolder) strbuf += "<IMG border=0 src='" + iTA["arr"].src + "'>";
else strbuf += "<IMG border=0 src='" + iTA["b"].src + "'>";

strbuf += "</td></tr></table>";
}
if (bv == 1) strbuf += "</div>";
if (this.nodeLevel == 0 && noTopFolder)
	{
	if (bv == 2) strbuf = "<layer id='node" + this.id + "' visibility='hidden'>";
	else if (bv == 1) strbuf = "<div id='node" + this.id + "'></div>";
	else if (bv == 0) strbuf = "";
	}
this.navObj = null;
if (this.isFolder) this.nodeImg = null;
this.iconImg = null;
if (bv == 0 || !prior) 
{
if (bv != 1)
doc.write(strbuf);
else {strbufarray[this.id] = strbuf;}
}
else
	{
	if (bv == 2) 
		{
		testlayer.document.open();
		testlayer.document.write(strbuf);
		testlayer.document.close();
		testlayer.zIndex=1;
		this.navObj = testlayer;
		this.navObj.top = doc.yPos;
		this.navObj.visibility = "show";
		doc.yPos += this.navObj.clip.height;
		}
	else if (bv == 1)
		{
		strbufarray[strbufIndex] = strbuf;
		strbufIndex++;
		}
	}
}

function fcSetNodeFont()
{
var font = "<font ";
	if (ekFolderFontSize) {
		font += "size='" + ekFolderFontSize + "'";
	}
	if (!ekFolderFontFace == ""){
		font += "name='" + ekFolderFontFace + "'";
	}
font += "</font>";
return font;
}

function fcCreateEntryIndex()
{
this.id = nEntries;
indexOfEntries[nEntries] = this;
nEntries++;
}
function mouseOverNode(type,folderId)
{
var mouseNode = 0;
mouseNode = indexOfEntries[folderId];
if (!mouseNode) return false;
if (bv == 1 && !mouseNode.navObj) {
	if (noFrame) {
		doc = document;
	}
	else
	{
		doc = document;
	}
	mouseNode.navObj = doc.all["node" + mouseNode.id];
}
if (bv == 1 && (doc != mouseNode.navObj.document && !isNav6)) {
	clearTimeout(rewriteID);
	checkload();
	return true;
}
if (cascade && noFrame && bv == 1) mouseNode.navObj.style.backgroundColor = menuBackColorOver;
if (cascade && noFrame && bv == 2) mouseNode.navObj.bgColor = menuBackColorOver;
if (type == 0)
	if (mouseNode.isOpen)
		{
		setStatus("Click to close");
		if (mouseOverPMMode == 2) clickOnNode(folderId);
		}
	else
		{
		setStatus("Click to open");
		if (mouseOverPMMode > 0) clickOnNode(folderId);
		}
else if (type == 1)
	{
	clearTimeout(timeoutIDOver);
	if (mouseNode.statusText == "")
		{
		setStatus(mouseNode.desc);
		}
	else
		{
		setStatus(mouseNode.statusText);
		}
	if (mouseNode.isFolder)
		if ((!mouseNode.isOpen && mouseOverIconMode == 1) || mouseOverIconMode == 2) {
 if (cascade && noFrame) timeoutIDOver = setTimeout("clickOnNode(" + folderId + ")",50);
else  timeoutIDOver = setTimeout("clickOnNode(" + folderId + ")",350);
}
	}
if (document.images && type == 1)
	{
	over = "Over";
	var iA = iNAO;
if (!mouseNode.iconImg)
{
	if (bv == 2) mouseNode.iconImg = mouseNode.navObj.document.images["nodeIcon"+mouseNode.id];
	else if (bv == 1 || doc.images) mouseNode.iconImg = doc.images["nodeIcon"+mouseNode.id];
}
	mouseNode.iconImg.src = mouseNode.nodeIcon(over,iA);
}
if (cascade && noFrame && bv == 1) {
if (modalClick && (mouseNode.nodeLevel > 0))

	for (i=0; i < mouseNode.nodeParent.nC; i++)
		{
		if (mouseNode.nodeParent.c[i].isOpen && (mouseNode.nodeParent.c[i] != mouseNode))
			{
			for (j=0; j < mouseNode.nodeParent.c[i].nC; j++) {
				if (mouseNode.nodeParent.c[i].c[j].isFolder && mouseNode.nodeParent.c[i].c[j].isOpen) mouseNode.nodeParent.c[i].c[j].setState(false);
			}
			mouseNode.nodeParent.c[i].setState(false);
			}
		}

}



return true;
}
function clickNode(folderId)
{
var thisNode = 0;
thisNode = indexOfEntries[folderId];
if (!thisNode) return false;
if (thisNode.isFolder) {
if (clickIconMode == 1 && thisNode.isOpen != null)
{
	if(!thisNode.isOpen) clickOnNode(folderId);
}
else if (clickIconMode == 2 && thisNode.isOpen != null)
		clickOnNode(folderId);
}
if (clickAction)
	clickAction(thisNode);
if (thisNode.hreference) return true;
else return false;
}
function fcNodeTIcon (){
iName = "";
if (this.isFolder) {
if (this.isOpen)
	{
	if (this.isLastNode == 0)
		iName = "mn";
	else if (this.isLastNode == 1)
		iName = "mln";
	else
		iName = "mfn";
	}
else
	{
	if (this.isLastNode == 0)
		iName = "pn";
	else if (this.isLastNode == 1)
		iName = "pln";
	else
		iName = "pfn";
	}
if (noDocs)
	{
	folderChildren = false;
	for (i=0 ; i < this.nC; i++)
		{
		if (this.c[i].isFolder) folderChildren = true;
		}
	if (!folderChildren) 
		if (this.isLastNode == 0)
			iName = "n";
		else if (this.isLastNode == 1)
			iName = "ln";
		else
			iName = "fn";
	}
}
else
{
	if (this.isLastNode == 0)
		iName = "n";
	else if (this.isLastNode == 1)
		iName = "ln";
	else
		iName = "fn";
}
if (treeLines == 0) iName = "b";
tmpIcon = iTA[iName].src;
return tmpIcon;
}
function fcNodeIcon(over,iA){

	tmpIcon = "";
	if (this.isFolder)
		{
		if (this.isOpen)
			{
			if (this["openIcon"+over] != "")
				tmpIcon = imageArray[this["openIcon"+over]].src;
			else if (this.nodeLevel == 0)
				tmpIcon = iA["tOF"].src;
			else
				tmpIcon = iA["oF"].src;
			}
		else
			{
				if (this["closedIcon" + over] != "")
					tmpIcon = imageArray[this["closedIcon"+over]].src;
				else if (this.nodeLevel == 0)
					tmpIcon = iA["tCF"].src;
				else
					tmpIcon = iA["cF"].src;
			}
		}
	else
		{
		if (this["openIcon"+over] != "")
			tmpIcon = imageArray[this["openIcon"+over]].src;
		else
			tmpIcon = iA["d"].src;
		}
	if (tmpIcon == "") tmpIcon = iTA["b"].src;
return tmpIcon;
}
function mouseOutNode(type,folderId)
{
var mouseNode = 0;
mouseNode = indexOfEntries[folderId];
if (!mouseNode) return false;
clearTimeout(timeoutIDOver);
if (document.images && type == 1)
	{
	over = "";
	var iA = iNA;
if (!mouseNode.iconImg)
{
if (bv == 2) mouseNode.iconImg = mouseNode.navObj.document.images["nodeIcon"+mouseNode.id];
else if (bv == 1 || doc.images) mouseNode.iconImg = doc.images["nodeIcon"+mouseNode.id];
}
	mouseNode.iconImg.src = mouseNode.nodeIcon(over,iA);
	}
 if (cascade && noFrame && bv == 1) mouseNode.navObj.style.backgroundColor = menuBackColor;
if (cascade && noFrame && bv == 2) mouseNode.navObj.bgColor = menuBackColor;

setStatus("");
return true;
}
function setStatus(statusText){
var str = statusText;
if (bv > 0)
	eval("str = str.replace(/<[^<>]*>/g,'');");
top.window.defaultStatus = "";
top.window.status = str;
thisFrame.defaultStatus = str;
thisFrame.status = str;
if (bv == 0)
	{
	clearTimeout(timeoutID);
	timeoutID = setTimeout("top.status = ''",5000);
	}
}
function clickOnNode(folderId)
{
var cF = 0;
var state = 0;
oldwinheight = thisFrame.innerHeight;
oldwinwidth = thisFrame.innerWidth;
cF = indexOfEntries[folderId];
if (!cF) return false;
if (!cF.navObj && bv == 1) cF.navObj = doc.all["node" + cF.id];
state = cF.isOpen;
if (!state) {
	if (cF.isInitial == false) {
		if(cF.nC == 0) 
			if (!addToTree){
				cF.setState(!state);
			}
			else
				if (addToTree(cF) == true) return false;
		if(cF.nC > 0) {
			if (bv == 2)
				if (noFrame && cascade ) doc.yPos = 0;
				else doc.yPos = cF.navObj.clip.height;
			if (bv > 0) prior = cF.navObj;
			if (bv == 2 && noFrame && cascade) {
				var divLayer = new Layer(menuWidth, topLayer);
				divLayer.document.open();
				divLayer.document.write("");
				divLayer.document.close();
				divLayer.top = cF.navObj.top;
				if (cF.nodeParent.divObj)
					divLayer.top += cF.nodeParent.divObj.top;
				divLayer.bgColor = menuBackColor;
				divLayer.left = menuWidth * cF.nodeLevel;
				topLayer.clip.width = Math.max(topLayer.clip.width, menuWidth * (cF.nodeLevel + 1));
				divLayer.visibility = "show";
				divLayer.zIndex=6000 + cF.id;
				divLayer.clip.width = menuWidth;
				cF.divObj = divLayer;

			}
			if (bv > 0) {
				level = cF.nodeLevel;
				leftSide = cF.nodeLeftSide;
				if (bv == 1) {
					strbufarray = new Array();
					strbufIndex = 0;
				}
				for (var i=0 ; i < cF.nC; i++)
					{
					cF.c[i].nodeParent = cF;
					if (i == cF.nC-1)
						newLastNode = 1;
					else
						last = 0;
					if (noDocs)
						{
						newLastNode = 1;
						for (var j=i+1; j < cF.nC; j++)
							if (cF.c[j].isFolder) newLastNode = 0;
						}
					else
						{
						newLastNode = 0;
						if (i == cF.nC-1) newLastNode = 1;
						}
					if (!noDocs || cF.c[i].isFolder) {
						if (bv == 2 && noFrame && cascade) 
							cF.c[i].initialize(level + 1, newLastNode, leftSide, doc, divLayer);
						else
							cF.c[i].initialize(level + 1, newLastNode, leftSide, doc, prior);

						needRewrite = true;
					}
				}
				if (bv == 1){ 
      				htmlStr = strbufarray.join("");
					if (cascade && noFrame) {
						leftdivpos = prior.offsetWidth + prior.offsetLeft;
						topdivpos = prior.offsetTop;
						zpos = 6000 + level;
					}
					if (isNav6) { 
						newObj = document.createElement("DIV");
						newObj.innerHTML = htmlStr;
						prior.appendChild(newObj);
						if (cascade && noFrame) {
							newObj.style.position = "absolute";
							newObj.style.backgroundColor = menuBackColor;
							newObj.style.top = topdivpos;
							newObj.style.left = leftdivpos;
							newObj.style.borderWidth = "2px";
							newObj.style.borderStyle = "solid";
							newObj.style.borderColor = menuBorderColor;
							newObj.style.width = menuWidth;
							newObj.zindex = zpos;
							newObj.id = "div" + cF.id;
						}								
					} else {
						if (!(cascade && noFrame))prior.insertAdjacentHTML("AfterEnd",htmlStr);
						else {
							htmlStr = "<DIV id = 'div" + cF.id + "' style='position:absolute;border-style:solid;border-color:" + menuBorderColor + ";border-width:2px;background-color:" + menuBackColor + ";zindex:" + zpos + ";top:" + topdivpos + ";left:" +leftdivpos + "'>" + htmlStr + "</DIV>";
							prior.insertAdjacentHTML("AfterBegin",htmlStr);
						}
					}
				}
			}
			cF.setState(!state);
			cF.isInitial = true;
		}
	}
	else {
			cF.setState(!state);
	}
}
else {
	if (bv == 0) cF.isInitial = false;
	cF.setState(!state);
}
if (bv == 2)cF.moveState(!state);
if (!state && modalClick && (cF.nodeLevel > 0))
	for (i=0; i < cF.nodeParent.nC; i++)
		{
		if (cF.nodeParent.c[i].isOpen && (cF.nodeParent.c[i] != cF))
			{
			if (bv == 2) cF.nodeParent.c[i].moveState(false);
			if (bv == 0) cF.nodeParent.c[i].isInitial = false;
			cF.nodeParent.c[i].setState(false);
			}
		}
if (bv == 0)
	setTimeout("CreateFolderControl()",50);
else
	doc.close();
	
return false;
}
function fcCollExp(mode)
{
var i=0;
if (mode == 1)
	{
	this.isInitial = true;
	if (this.isFolder)
		this.isOpen = true;
	}
else
	{
	this.isInitial = false;
	if (this.isFolder)
		this.isOpen = false;
	}
if (this.isFolder) {
for (i=0; i<this.nC; i++)
	this.c[i].collExp(mode);
}
}
function fcInitMode()
{
var i=0;
if (initialMode == 2)
	{
	if (this.isFolder) this.isOpen = true;
	this.isInitial = true;
	}
if (this.isFolder) {
for (i=0; i<this.nC; i++)
	{
	this.c[i].initMode();
	if (this.c[i].isOpen && this.c[i].isInitial)
		{
		this.isOpen = true;
		this.isInitial = true;
		}
	}
}
}
function initializeDocument()
{
if (firstInitial)
	{
	if (initialMode == 0)
		{
		fC.isInitial = false;
		fC.isOpen = false;
		}
	if (initialMode == 1)
		{
		fC.isInitial = true;
		fC.isOpen = true;
		}
	fC.initMode();
	}
prior = null;
fC.initialize(0, 1, "", doc, prior);
firstInitial = false;
}
function collapseAll(){
var i=0;
if (noFrame)
	{
	if (initialMode == 0)
	{
	if (!fC.navObj && bv == 1) fC.navObj = doc.all["node" + fC.id];
	if (fC.isOpen){
		fC.setState(!state);
		if (bv == 2)fC.moveState(!state);
		}
	}
	if (initialMode > 0)
		{
		for (i=0; i<fC.nC; i++)
		{
		if (fC.c[i].isOpen) {
		if (!fC.c[i].navObj && bv == 1) fC.c[i].navObj = doc.all["node" + fC.c[i].id];
		state = fC.c[i].isOpen;
		fC.c[i].setState(!state);
		if (bv == 2)fC.c[i].moveState(!state);
		}
		}	
		}
	}
else
	{
	if (initialMode == 0)
		{
		fC.isInitial = false;
		fC.isOpen = false;
		}
	if (initialMode > 0)
		{
		fC.isInitial = true;
		fC.isOpen = true;
		}
	for (i=0; i<fC.nC; i++)
		fC.c[i].collExp(0);
	backButton = false;
	setTimeout("CreateFolderControl()",50);
	}
}
function expandAll(){
var i=0;
if (noFrame)
	{
	for (i=0; i<nEntries; i++)
		if (!indexOfEntries[i].isOpen)
			{
			indexOfEntries[i].setState(!state);
			if (bv == 2)indexOfEntries[i].moveState(!state);
			}
	}
else
	{
	fC.collExp(1);
	backButton = false;
	setTimeout("CreateFolderControl()",50);
	}
}
function fcInitLayer() {
var i;
var totalHeight;
var oldyPos;
var width = 0;
if (!this.nodeParent)
	layer = topLayer;
else
	layer = this.nodeParent.navObj;
this.navObj = layer.document.layers["node"+this.id];
this.navObj.top = doc.yPos;
this.navObj.visibility = "show";
if (this.nC > 0 && this.isInitial)
	{
	doc.yPos += this.navObj.document.layers[0].top;
	oldyPos = doc.yPos;
	doc.yPos = this.navObj.document.layers[0].top;
	this.navObj.clip.height = doc.yPos;
	totalHeight = 0;
	for (i=0 ; i < this.nC; i++)
		{
		if (!noDocs || this.c[i].isFolder)
			{
			if (this.c[i].hidden == false) this.c[i].initLayer();
			if (bv == 2) 
				{
				totalHeight +=  this.c[i].navObj.clip.height;
				width = Math.max(width,this.c[i].navObj.clip.width);
				}
			}
		}
	if (this.isOpen)
		{
		doc.yPos = oldyPos + totalHeight;
		this.navObj.clip.height += totalHeight;
		this.navObj.clip.width = Math.max(width, this.navObj.clip.width);
		}
	else
		{
		doc.yPos = oldyPos;
		}
	}
else
	doc.yPos += this.navObj.clip.height;
}








function CreateFolderInstance(d, urlArray) {
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
				completeUrl += "javascript:fcNewLinkWindow('" + urlArray[urlNumber + 1] + "', '" + urlArray[urlNumber + 2] + "');";
			}
			else {
				completeUrl += "javascript:fcNewLinkFrame('" + urlArray[urlNumber + 1] + "', '" + urlArray[urlNumber + 2] + "');";
			}
		}
	}
//	if ((ekFolderMaxDescriptionLength != 0) && (d.length > ekFolderMaxDescriptionLength)) {
//		d = d.substring(0, ekFolderMaxDescriptionLength - 1) + "...";
//	}
	folder = new fcNode(d, completeUrl, CustomEvents);
	folder.isFolder = true;
	return folder;
}

function CreateLink(d, urlArray)
{
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
				completeUrl += "javascript:fcNewLinkWindow('" + urlArray[urlNumber + 1] + "', '" + urlArray[urlNumber + 2] + "');";
			}
			else {
				completeUrl += "javascript:fcNewLinkFrame('" + urlArray[urlNumber + 1] + "', '" + urlArray[urlNumber + 2] + "');";
			}
		}
	}
//	if ((ekFolderMaxDescriptionLength != 0) && (d.length > ekFolderMaxDescriptionLength)) {
//		d = d.substring(0, ekFolderMaxDescriptionLength - 1) + "...";
//	}
	linkItem = new fcNode(d, completeUrl, CustomEvents);
	linkItem.isFolder = false;
	return linkItem;

}








function InsertFolder(p, c)
{
return p.addChild(c);
}
function InsertFile(p, d)
{
return p.addChild(d);
}
function fcAddChild(childNode)
{
this.c[this.nC] = childNode;
childNode.nodeParent = this;
childNode.nodeLevel = this.nodeLevel + 1;
this.nC++;
return childNode;
}

function fcSetInitial(initial){
if (initial && this.isFolder)
	{
	this.isInitial = true;
	this.isOpen = true;
	}
return;
}
function fcSetIcon(o,c, oO,cO){
if (this.isFolder){
if (o != null) this.openIcon = o;
if (c != null) this.closedIcon = c;
if (oO != null) this.openIconOver = oO;
if (cO != null) this.closedIconOver = cO;
}
else
{
if (o != null) this.openIcon = o;
if (c != null) this.openIconOver = c;
}
return;
}
function fCimage(f){
this.src = ekFolderImagePath + f;
return
}
function addImage(name, f){
if (bv != 1)
	imageArray[name] = new fCimage(f);
else
	{
	imageArray[name] = new Image();
	imageArray[name].src = ekFolderImagePath + f;
	}
nImageArray++;
}
function addIcon(icon,prop,f) {
if (bv != 1)
	icon[prop] = new fCimage(f);
else
	{
	icon[prop] = new Image();
	icon[prop].src = ekFolderImagePath + f;
	}
}
function fcSetStatusBar(s){
if (s != null) this.statusText = s;
return;
}
function fcSetUserDef(name,text){
if (text != null) this.userDef += "<" + name + ">" + text + "</" + name + ">";
return;
}
function fcGetUserDef(name){
substr1 = "<" + name + ">";
substr2 = "</" + name + ">";
length1 = substr1.length;
index1 = this.userDef.indexOf(substr1);
index2 = this.userDef.indexOf(substr2);
if (index1 == -1 || index2 == -1) return "";
return this.userDef.substring(index1+length1,index2);
}
function initImage(){
addIcon(iNA,"tOF",topOpenFolderIcon);
addIcon(iNA,"tCF",topClosedFolderIcon);
addIcon(iNA,"oF",openFolderIcon);
addIcon(iNA,"cF",closedFolderIcon);
addIcon(iNA,"d",documentIcon);
addIcon(iNAO,"tOF",topOpenFolderIconOver);
addIcon(iNAO,"tCF",topClosedFolderIconOver);
addIcon(iNAO,"oF",openFolderIconOver);
addIcon(iNAO,"cF",closedFolderIconOver);
addIcon(iNAO,"d",documentIconOver);
addIcon(iTA,"mn",mnIcon);
addIcon(iTA,"pn",pnIcon);
addIcon(iTA,"pln",plnIcon);
addIcon(iTA,"mln",mlnIcon);
addIcon(iTA,"pfn",pfnIcon);
addIcon(iTA,"mfn",mfnIcon);
addIcon(iTA,"b",bIcon);
addIcon(iTA,"ln",lnIcon);
addIcon(iTA,"fn",fnIcon);
addIcon(iTA,"vl",vlIcon);
addIcon(iTA,"n",nIcon);
addIcon(iTA,"arr",arrIcon);

}

function blank() {
icheck = 0;
ret = "<HTML><HEAD>";
if (bv < 2 ) ret += "<BASE HREF='" + document.location + "'>";
ret += "</HEAD><BODY " + bodyOption + " onLoad = 'parent.checkload()'";
ret += ">";
initImage();
ret += "<B><CENTER>Please wait for menu<BR>to be constructed</B><P>";
ret += "<font size=-1>Loading auxiliary bitmaps:<br>";
subret = "<img src='";
for (var propname in iNA)
	if (iNA[propname].src != "") ret += subret + iNA[propname].src + "'>";
for (var propname in iNAO)
	if (iNAO[propname].src != "") ret += subret + iNAO[propname].src + "'>";
for (var propname in iTA)
	if (iTA[propname].src != "") ret += subret + iTA[propname].src + "'>";
for (var propname in imageArray)
	ret += subret + imageArray[propname].src + "'>";
ret += "<br></CENTER></BODY></HTML>";
if (doc.all) doc.open("text/html","replace");
else doc.open();
doc.write(ret);
if (isNav6) doc.all = doc.getElementsByTagName("*");

if (doc.all) {
setTimeout("doc.close()",400);
if (navigator.userAgent.indexOf("Opera") >= 0) setTimeout("checkload()",200);
}
else
{
doc.close();
if (navigator.userAgent.indexOf("Opera") >= 0) setTimeout("checkload()",200);
parent.ek_nav_bottom.onload = checkload;
}
return ret;
}
icheck = 0;
function checkload() {
doc = parent.ek_nav_bottom.document;
if (isNav6) doc.all = doc.getElementsByTagName("*");

if (!doc.all) CreateFolderControl();
else {
   icheck++;
   if (!doc.readyState || doc.readyState == "complete") {
	  setTimeout("CreateFolderControl()",200);
   }
   else {
	if (icheck > 500)
		alert("Loading not complete");
   	else 
    	 setTimeout("checkload()",200);
   }
}
}
function CreateFolderControl() {
backButton = false;
if (rewriting) return false;
rewriting = true;
if (!fC)
{
alert("Menu unable to load due to error");
rewriting = false;
return false;
}
if (document.all || isNav6)
	bv = 1;
else 
	{
	if (document.layers)
		{
		bv = 2;
		if (inTable || inForm) {
			if (!noFrame) bv = 0;
			else {
				alert("Netscape version 4 cannot handle menu within table or form without frames");
				return false;
			}

		}
		}
	else
		bv = 0;
	}
if (bv == 0 && noFrame) {
	alert("Old browsers cannot use menus without frames. Please upgrade.");

}
if (bv == 1 && navigator.userAgent.indexOf("Mac") >= 0) bv = 0;
if (navigator.userAgent.indexOf("Opera") >= 0) bv = 0;
if (bv == 2 ) self.onresize = self.handleResize;

if (noFrame) {
doc = document;
frameParent = "self";
thisFrame = self;
}
else
{
thisFrame = self;
doc = thisFrame.document;
if (isNav6) doc.all = doc.getElementsByTagName("*");
if (bv == 2) {
	if (doc.width == 0)
	{
	clearTimeout(rewriteID);
	rewriteID = setTimeout("CreateFolderControl()",1000);
	rewriting = false;
	return false;
	}
}
}
setStatus("Menu is loading");
if (bv == 1) doc.open("text/html","replace");
if (bv == 0) doc.open();
nEntries = 0;
doc = thisFrame.document;
if (isNav6) doc.all = doc.getElementsByTagName("*");

if (!noFrame) { 
	doc.write("<html><head>");
	if (bv < 2 ) doc.write("<BASE HREF='" + document.location + "'>");
	doc.write("<Title></Title></head>");
	resizestr = "";
	if (bv == 2 && !noFrame) {
		resizestr = "onResize = 'parent.handleResize(event)' ";
		resizestr += " onLoad = 'parent.docLoad()' ";
		resizestr += " onUnLoad = 'parent.backLoad()' ";
	}
	doc.write("<BODY " + resizestr + bodyOption + ">");
	setStatus("Please wait for menu.");
}

initImage();
if (bv == 2) self.onresize = self.handleResize;
if (bv == 2) 
	{
      if (noFrame && inTable) {
		doc.write("<ILAYER id = 'foldertree' visibility = 'show' top=" + topGap + " left = " + leftGap + " Z-INDEX=1>");
      } else {
		doc.write("<LAYER id = 'foldertree' visibility = 'show' top=" + topGap + " left = " + leftGap + " Z-INDEX=1>");
      }
	doc.write("<layer id = 'header' visibility = 'hidden'>" + menuHeader + "</layer>");
	initializeDocument();
	doc.write("<layer id = 'footer' visibility = 'hidden'>" + menuFooter + "</layer>");
      if (noFrame && inTable) {
	   doc.write("</ILAYER>");
      } else {
	   doc.write("</LAYER>");
      }
	}
else
	{

	if (bv == 1 && noFrame ) {
        if (!cascade) {
		if (inTable)
			doc.write("<DIV id='foldertree' style='position:relative;'>");
		else
			doc.write("<DIV id='foldertree' style='position:absolute;'>");
		} else {
		doc.write("<DIV id='foldertree' style='position:absolute;zindex:4000;border-style:solid;border-color:" + menuBorderColor + ";border-width:2px;background-color:" + menuBackColor + "' >");
		}
	}
	else if (bv == 1) doc.write("<DIV id='foldertree' style='position:absolute; left:" + leftGap + "; top:" + topGap + ";'>");
	else doc.write("<DIV id='foldertree'>");

	doc.write(menuHeader);
strbufarray = new Array();
	initializeDocument();

	if (bv == 1) eval("htmlStr = strbufarray.join(''); doc.write(htmlStr);");
	doc.write(menuFooter);
     doc.write("</DIV>");
	if (bv == 1 && noFrame && cascade) doc.write("<DIV>");
	if (bv == 1 && noFrame && !cascade) doc.write("</DIV>");

	}

if (!noFrame){
doc.write("</BODY></HTML>");
if (bv == 2) doc.close();
}
if (bv == 1 && noFrame)
{
doc.body.topMargin = 0;
doc.body.leftMargin = 0;
doc.body.rightMargin = 0;
}

if (noFrame) doc = document;
else{if (bv == 2) doc = parent.ek_nav_bottom.document;}
if (isNav6) doc.all = doc.getElementsByTagName("*");

if (bv == 2)
	{
	topLayer = doc.layers["foldertree"];
	doc.yPos = topLayer.layers["header"].clip.height;
	fC.initLayer();
	topLayer.layers["footer"].top = doc.yPos;
	topLayer.clip.height = fC.navObj.clip.height + topLayer.layers["header"].clip.height + topLayer.document.layers["footer"].clip.height;
	if (!(noFrame && cascade)) doc.height = topLayer.clip.height + topGap;
	topLayer.layers["header"].visibility = "show";
	topLayer.layers["footer"].visibility = "show";

	topLayer.visibility = "show";
	fC.display();
	oldwinheight = thisFrame.innerHeight;
	oldwinwidth = thisFrame.innerWidth;
}

setStatus("");
rewriting = false;
needRewrite = false;
if (!noFrame)
	{
	if (doc.all)
		setTimeout("doc.close()",200);
	else
		if (bv == 0)
			{
			doc.close();
			doc = parent.ek_nav_bottom.document;
			}
	}
backButton = false;

return false;
}
function docLoad() {
if (bv == 2) {
if (!topLayer) {setTimeout("CreateFolderControl()",200);}
else {
if (topLayer.document.layers["node0"].visibility == "hide") {setTimeout("self.history.back()",200);}
}
}
return;
}
function backLoad() {
if (!backButton) {
backButton = true;
}
else {
	if (!remenu) {
	clearTimeout(rewriteID);
	rewriteID = setTimeout("self.history.back()",200);
	backButton = false;
	}
}
return false;
}
function handleResize(evt) {
if (noFrame) {
locref = doc.location.href;
doc.location.replace(locref);
return void(0);
}
backButton = false;
if (rewriting) {
alert("Please do not resize window while menu is loading.\n\n Resize again to redraw menu.");
rewriting = false;
return false;
}
if (!needRewrite && noWrap && navigator.userAgent.indexOf("Win") != -1) 
	{
	for (i=0 ; i < nEntries; i++)
		{
		thisnode = indexOfEntries[i];
		if (thisnode.nodeImg) thisnode.nodeImg.src = thisnode.nodeTIcon();
		}
	oldwinheight = thisFrame.innerHeight;
	oldwinwidth = thisFrame.innerWidth;
	return false;
	}
if (evt.target.name == menuFrame)
	{
	remenu = false;
	if(!(oldwinheight == evt.target.innerHeight && oldwinwidth == evt.target.innerWidth))
		{
		clearTimeout(rewriteID);
		if (topLayer) topLayer.clip.height = 0;
		rewriteID = setTimeout("CreateFolderControl()",500);
		rewriting = false;
		remenu = true;
		}
	}
else
	{
	if(!(oldtopheight == evt.target.innerHeight && oldtopwidth == evt.target.innerWidth))
		{
		if (!remenu){
			clearTimeout(rewriteID);
			rewriteID = setTimeout("CreateFolderControl()",500);
			rewriting = false;
		}
		oldtopheight = evt.target.innerHeight;
		oldtopwidth = evt.target.innerWidth;
		}
	remenu = false;
	}
return false;
}




function fcNewLinkFrame (url, frame) {
	return top[frame].location.href = url;
}

function fcNewLinkWindow (url, windowName) {
	return window.open(url, windowName);
}





