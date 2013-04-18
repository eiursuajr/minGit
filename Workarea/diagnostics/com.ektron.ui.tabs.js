//////////
//
// name: TabUtil
// desc: A static class for manipulating Tab related classes and
//       handling Tab related events.
//

var TabUtil =
{
	//////////
	//
	// name: onMouseOver
	// desc: The onmouseover event handler for a tab. This
	//       handler sets the tab's className. The class gives
	//       the tab the look of a hover tab.
	//

	onMouseOver: function( tabElement )
	{
		if( tabElement.className != "tabSelected" ) {
			tabElement.className = "tabHover";
		}
	},

	//////////
	//
	// name: onMouseOut
	// desc: The onmouseover event handler for a tab. This
	//       handler sets the tab's className. The class gives
	//       the tab the look of a default tab.
	//

	onMouseOut: function( tabElement )
	{
		if( tabElement.className == "tabHover" ) {
			tabElement.className = "tabDefault";
		}
	},

	//////////
	//
	// name: onClick
	// desc: The onclick event handler for a tab. This
	//       handler selects the clicked tab and shows
	//       the corresponding tab panel.
	//

	onClick: function( tabElement )
	{
		TabUtil.selectTab( tabElement );
		TabUtil.showTabPanel( tabElement );
	},

	//////////
	//
	// name: selectTab
	// desc: Given a tab DOM element, find its sibling tabs,
	//       make them default tabs, and make the passed tab the
	//       selected tab.
	//

	selectTab: function( tabElement )
	{
		var parent = tabElement.parentNode;
		for( var i = 0; i < parent.children.length; i++ ) {
			var child = parent.children[i];
			child.className = "tabDefault";
			child.style.zIndex = TabUtil.zIndexTab;
		}
		
		tabElement.className = "tabSelected";
		tabElement.style.zIndex = TabUtil.zIndexTab + 1;
	},
	
	//////////
	//
	// name: getTabPanel
	// desc: Given a tab DOM element, get the corresponding
	//       tab panel DOM element
	//

	getTabPanel: function( tabElement )
	{
		// given a tab, get the coresponding tab pabel
		var tabId = tabElement.id;
		var panId = "tabPanel" + tabId.replace( "tab", "" );
		var element = document.getElementById( panId );
		
		return element;
	},
	
	//////////
	//
	// name: showTabPanel
	// desc: Given a tab DOM element, we find its corresponding
	//       tabPanel, display it, and hide the other panels. We
	//		 need to hide the other panels (as opposed to just
	//       setting its z-index) to fix an IE bug that causes
	//       form elements to "show through"
	//

	showTabPanel: function( tabElement )
	{
		var panel = TabUtil.getTabPanel( tabElement );
		var parent = panel.parentNode;
		for( var i = 0; i < parent.children.length; i++ ) {
			var child = parent.children[i];
			child.style.display = 'none';
		}
		
		panel.style.zIndex = TabUtil.zIndexTab;
		panel.style.display = 'inline'
	},

	zIndexTab: 1
}

function TabPanel( name, width, height, tabwidth, tabheight )
{
	this.display	 = __TabPanel_display;
	this.addTab		 = __TabPanel_addTab;
	this.setPanel	 = __TabPanel_setPanel;
	this.getPanel    = __TabPanel_getPanel;
	this.getPanels   = __TabPanel_getPanels;
	this.hidePanels  = __TabPanel_hidePanels;

	this.name	   = name;
	this.width	   = width;
	this.height	   = height;
	this.tabwidth  = tabwidth;
	this.tabheight = tabheight;

	this.tabs = new Array();
	this.panels = new Array();
	this.tabcount = 0;
}

function __TabPanel_addTab( name )
{
	this.tabs[name] = this.tabcount++;
}

function __TabPanel_display()
{
	var tabs = '';
	var style = "";
	
	style += "width: " + this.width + "px; ";
	style += "height: " + this.height + "px; ";
	tabs += '<div style="' + style + '" id="' + this.name + 'Group" class="tabPanelGroup">';
	tabs += '<div class="tabGroup">';

	// adds tabs
	var tabPosition = 0;
	for( var name in this.tabs ) {
		var type = ( tabPosition == 0 ) ? "tabSelected" : "tabDefault";
		var zIndex = ( tabPosition == 0 ) ? TabUtil.zIndexTab : 0;
		var id = "tab" + tabPosition;
		var style = "left:" + ( this.tabwidth * tabPosition ) + ";";
		style += "z-index:" + zIndex + ";"
		tabs += '<div onmouseover="TabUtil.onMouseOver(this)" onmouseout="TabUtil.onMouseOut(this)" style="' + style + '" id="' + id + '" class="' + type + '" onclick="TabUtil.onClick(this)">' + name + '</div>';
		tabPosition++;
	}

	tabs += '</div>';

	// adds tab panels
	tabs += '<div>';
	var panelPosition = 0;
	for( var name in this.panels ) {
		var id = "tabPanel" + panelPosition;
		var data = this.panels[name];
		var style = "display: " + (( panelPosition == 0 ) ? "inline" : "none" );
		style += "; height: " + (this.height - this.tabheight ) + "px; ";
		style += "width: " + this.width + "px; "
		style += "top: " + ( this.tabheight - 1 ) + "px;"
		tabs += '<div style="' + style + '" id="' + id + '" class="tabPanel">' + data + '</div>';
		panelPosition++;
	}
	tabs += '</div>';

	tabs += '</div>';
	
	var container = document.getElementById( this.name );
	if( container == null ) {
		container = document.createElement( "div" );
		container.setAttribute( "id", this.name );
		document.body.appendChild( container );
	}

	container.innerHTML = tabs;
}

function __TabPanel_setPanel( name, data )
{
	this.panels[name] = data;
}

function __TabPanel_hidePanels()
{
	var panels = this.getPanels();
	
	for( var i = 0; i < panels.length; i++ ) {
		var panel = panels[i];
		panel.style.display = 'none';
	}
}

function __TabPanel_getPanels()
{
	var panelPosition = 0;
	var panels = new Array();
	for( var n in this.panels ) {
		var id = "tabPanel" + panelPosition;
		panels[panelPosition] = document.getElementById( id );
		panelPosition++;
	}

	return pannels;
}

function __TabPanel_getPanel( name )
{
	var panelPosition = 0;
	for( var n in this.panels ) {
		if( n == name ) {
			var id = "tabPanel" + panelPosition;
			return document.getElementById( id );
		}
		panelPosition++;
	}
}