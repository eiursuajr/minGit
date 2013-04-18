var ExplorePanel =
{
	"getExplorePanel": function()
	{
		var treeRoot = null;

		//////////
		//
		// toolkit.getRootFolder expects a callback as a parameter.
		// Our callback is passed an asset. If the asset is non-null,
		// it creates a tree and renders it along with its children.
		// If it is null, we had an error connecting to CMS, so we
		// display an error message.
		// 

		document.body.style.cursor = "wait";
		toolkit.getRootFolder( function( folderRoot ) {
			document.body.style.cursor = "default";
			var folderName = null; try { folderName = folderRoot.name; } catch( e ) { ; }

			if( folderName != null ) {
				treeRoot = new Tree( folderName, 0, null, folderRoot, 0 );
				TreeDisplayUtil.showSelf( treeRoot, document.getElementById( "FolderTree" ) );
				TreeDisplayUtil.toggleTree( treeRoot.node.id );
			} else {
				//TODO: Clean this error message up; better error message
				var element = document.getElementById( "FolderTree" );
				element.style["padding"] = "10pt";
				var debugInfo = "<b>Cannot connect to the CMS server</b> " +
								"<p>Please verify that your connection information is correct " +
								"using the " +
								"<span style='cursor:hand;color:blue;text-decoration:underline' " +
								"onclick='Explorer.openConfigManager()'>Configuration Manager</span>. " +
								"Once you've made changes, " + 
								"<span style='cursor:hand;color:blue;text-decoration:underline' " +
								"onclick='Explorer.refresh()'>reconnect</span> " +
								"to the Ektron CMS 400. ";

				element.innerHTML = debugInfo;
			}
		
			Explorer.onLoadExplorePanel();
		}, 0 );
		
		return "<span id='FolderTree'></span>";
	}
}
