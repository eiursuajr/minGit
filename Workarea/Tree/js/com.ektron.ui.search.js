var SearchPanel =
{
	getFormData: function()
	{
		var searchForm = document.forms.search;
		var elements = { "qtitle": "text", "qtext": "text", "sf": "checkbox", "pw": "checkbox", "searchtype": "radio", "folderid": "select" };
		var doSearch = false;
		var query = "";

		for( var k in elements ) {
			var value = null;
			switch( elements[k] )
			{
				case "select":
					var selectList = searchForm[k];
					var index = selectList.selectedIndex;
					value = selectList[index].value;
				break;
				case "checkbox":
					value = searchForm[k].checked;
				break;
				case "radio":
					for( var i = 0; i < searchForm[k].length; i++ ) {
						if( searchForm[k][i].checked ) {
							query += k + "=" + escape(searchForm[k][i].value) + "&";
						}
					}
				break;
				default:
					value = searchForm[k].value;
					if( value ) {
						doSearch = true;
					}
				break;
			}
			if( value != null ) {
				query += k + "=" + escape( value ) + "&";
			}
		}

		if( doSearch ) {
			SearchManager.execute( query )
		}

		return false;
	},

	getSearchPanel: function()
	{
		var panel = '';
		panel += '<div class="searchPanel">';
		panel += '<form target=_mainWindow name="search" onsubmit="return SearchPanel.getFormData(this);" method="get">';
		panel += '<div class="searchTitle">Search by any or all of the criteria below</div>';
		panel += '<div class="searchCriteria">';
		panel += '<div class="searchLabel">All or part of the title:</div>';
		panel += '<input type="text" name="qtitle" ID="qtitle" style="width:100%" oncontextmenu="ContextMenuUtil.hide();ContextMenuUtil.enableDefaultMenu(event, true)">';
		panel += '</div>';
		panel += '<div class="searchCriteria">';
		panel += '<div class="searchLabel">Containing the word or phrase:</div>';
		panel += '<input type="text" name="qtext" ID="qtext" style="width:100%" oncontextmenu="ContextMenuUtil.hide();ContextMenuUtil.enableDefaultMenu(event, true)">';
		panel += '</div>';
		panel += '<div class="searchCriteria">';
		panel += '<div class="searchLabel">Look in:</div>';
		panel += '<select name="folderid" style="font-size:xx-small" onChange="if( this.value == \'{browse}\' ) { this.selectedIndex=0;Explorer.openFolderBrowser(this) }">';
		panel += '<option value="0">Content</option>';
		panel += '<option value="{browse}">Browse ...</option>';
		panel += '</select>';
		panel += '</div>';
		panel += '<div class="moreSearchCriteria" onclick="advancedPanel.style.display=(advancedPanel.style.display==\'none\'?\'\':\'none\')">';
		panel += 'More advanced options  <span style="color:red;">&raquo;</span>';
		panel += '</div>';
		panel += '<div class="advancedPanel" id="advancedPanel" style="display:none;">';
		panel += '<div class="advancedCriteria">';
		panel += '<input type="checkbox" name="sf" ID="Checkbox1" checked="true"/><label for="Checkbox1"> <b>Search all subfolders</b></label>';
		panel += '</div>';
		panel += '<div class="advancedCriteria">';
		panel += '<input type="checkbox" name="pw" ID="Checkbox2" checked="true"/><label for="Checkbox2"> <b>Match partial words</b></label>';
		panel += '</div>';
		panel += '<div class="advancedCriteria">';
		panel += '<input type="radio" name="searchtype" ID="radio1" value="a" checked="true"/><label for="radio1"> <b>All the words</b></label>';
		panel += '</div>';
		panel += '<div class="advancedCriteria">';
		panel += '<input type="radio" name="searchtype" ID="radio2" value="n"/><label for="radio2"> <b>Any of the words</b></label>';
		panel += '</div>';
		panel += '<div class="advancedCriteria">';
		panel += '<input type="radio" name="searchtype" ID="radio3" value="e"/><label for="radio3"> <b>Exact phrase</b></label>';
		panel += '</div>';
		panel += '</div>';
		panel += '<input class="searchButton" style="vertical-align:right" type="submit" value="Search" ID="Submit1"/>';
		panel += '</form>';
		panel += '</div>';

		return panel;
	}
}