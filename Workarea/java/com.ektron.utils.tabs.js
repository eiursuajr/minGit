////
// name: EktTabs
// desc: Manages DHTML tabs and its panels 
//
// Usage:
//      var myTabs = ektTabs[array of span or div Ids]
//      myTabs.showPanel(HtmlElement) //Will display the panel with an id of tab id followed by "-panel".
//

////
// name: ektTabs
// desc: Give an array of tab Ids as string.
//
function ektTabs( arTabs ) {
    this.tabs = arTabs;
    this.appendToId = "-panel";  
    this.showPanel = ektTabs_showPanel;
}

////
// name: showPanel
// disc: A function highlight given tab and displays its panel.
//       It accepts the tab as HtmlElemet (tabElem).
//
function ektTabs_showPanel( tabElem ) {
    var i=0;
    var objPanel;
    var objTab;
    for (i=0; i<this.tabs.length; i++) {
        objPanel = document.getElementById( this.tabs[i] + this.appendToId );
        objTab = document.getElementById( this.tabs[i] );
        if (tabElem.id.toLowerCase() == this.tabs[i].toLowerCase()) {
            objPanel.className = "EktTabPanelActive";
        } else {        
            objPanel.className = "EktTabPanelNotActive";
        }
        if (objTab != null) {
            objTab.className = "EktTabNotActive";
        }
    }
    tabElem.className = "EktTabActive";
}
