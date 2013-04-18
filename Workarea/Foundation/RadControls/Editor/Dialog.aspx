<%@ Page language="c#" Codebehind="Dialog.aspx.cs" SmartNavigation="false" AutoEventWireup="false" Inherits="Ektron.Telerik.WebControls.Dialogs.Dialog" %>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="head" runat="server"></head>
	<body class="dialog">	
	    <link rel="stylesheet" type="text/css" href="<%=ResolveUrl("~/WorkArea/csslib/editor/WorkArea.css")%>" /><!-- Ektron --> 
		<script type="text/javascript">
		//TEKI: Memory leaks in dialogs
		var _disposeManager =
		{
			_disposeList : [],
			
			Add : function(control)
			{
				this._disposeList[this._disposeList.length] = control;
			},
			
			Initialize : function()
			{	
				//By default controls are loaded after a postback is performed, so the controsl script is not loaded the first time.	
				if (typeof(AttachEvent) == "undefined") return;
				
				AttachEvent(window, "unload", function()
				{
					GetDisposeManager().DisposeAll();
				});
			},
			
			DisposeAll : function()
			{
				//alert("Dispose all called!");
				 for (var i=0; i < this._disposeList.length; i++)
				 {
					var control = this._disposeList[i];
					if (control && control.Dispose)
					{
						try
						{
							control.Dispose();
						} catch(ex) 
						{
							//alert(ex.message);
						}
					}
				 }
			}
		}
						
		function GetDisposeManager()
		{
			return _disposeManager;
		}
		
		</script>
		<form id="mainForm" runat="server" enctype="multipart/form-data">
			<asp:placeholder id="plchControl" runat="server" />
		</form>
		<script type="text/javascript">
		    <!--
			//Initialize the manager here, when the AttachEvent is already defined.
			_disposeManager.Initialize();		
			// Ektron start - close the RadWindow at ESC key.
			if (typeof Ektron != "undefined")
			{
			    Ektron.ready(function(){
			        BindOnRadWindowKeyDown(); // at RadWindow.js
                });	
            }
            // Ektron end
            //-->
		</script>
	</body>
</html>