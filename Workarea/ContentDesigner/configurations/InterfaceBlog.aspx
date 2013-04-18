<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InterfaceBlog.aspx.cs" Inherits="ContentDesigner_configurations_InterfaceBlog" %>

<root>
	<modules>
		<!-- RadEditorHtmlInspector causes defect #30021 - cursor goes back and forth in the plain text field in data entry mode -->
		<module name="RadEditorHtmlInspector" dockingZone="Module" enabled="false" visible="false" />
	</modules>
	<tools name="Edit" enabled="true" dockable="false">
		<%if(!(IsForum && !options.ContainsKey("clipboardmenu"))){%>	    
	    <tool separator="true"/>
		<tool name="Cut" />
		<tool name="Copy" />
		<tool name="Paste" />
		<tool name="PasteFromWordNoFontsNoSizes" />
		<tool name="PastePlainText" />
		<tool name="Undo" />
		<tool name="Redo" />
		<tool separator="true"/>
		<% } %>
		<%if(!(IsForum && !options.ContainsKey("stylemenu"))){%>	  	 		
		<tool name="FormatBlock"/>
		<%}%>
		<%if(!(IsForum && !options.ContainsKey("fontmenu"))){%>	  		
		<tool name="FontName"/>
		<tool name="FontSize"/>
		<tool name="RealFontSize"/>
		<tool name="ForeColor" />
		<tool separator="true"/>
		<% } %>
		<%if(!(IsForum && !options.ContainsKey("textformatmenu"))){%>	  		 
		<tool name="Bold" />
		<tool name="Italic" />
		<tool name="Underline" />
		<tool separator="true"/>	
		<tool name="AjaxSpellCheck" />
		<tool separator="true"/>
		<% } %>
		<%if(!(IsForum && !options.ContainsKey("linkmenu"))){%>	  
		<tool name="LinkManager" />
		<tool name="Unlink" />
		<% } %>
		<%if(((!string.IsNullOrEmpty(Request.QueryString["LibraryAllowed"])) && ( Convert.ToBoolean(Request.QueryString["LibraryAllowed"]) == true)) || (IsForum ==true && options.ContainsKey("library") == true ) ){ %>
		<tool name="EkLibrary" />
		<% } %>
		<%if(!(IsForum && !options.ContainsKey("symbolsmenu"))){%>	  		
		<tool name="InsertSymbol" />
		<% } %>
	</tools>	
	<tools name="Paragraph Format" enabled="true" dockable="false">
	<%if(!(IsForum && !options.ContainsKey("paragraphmenu"))){%>	  			
	    <tool separator="true"/> 
		<tool name="InsertOrderedList" />
		<tool name="InsertUnorderedList"/>
		<tool name="Outdent" />
		<tool name="Indent" />
		<tool separator="true"/>
		<tool name="JustifyLeft" />
		<tool name="JustifyCenter" />
		<tool name="JustifyRight" />
		<tool name="JustifyFull" /> 
		<tool separator="true"/>
		<tool name="InsertHorizontalRule" />
		<% } %>
		
		<%if (!string.IsNullOrEmpty(Request.QueryString["wiki"])) {%>
		<tool name="EkAddLinkPage" />
		<%}%>
	</tools>
	<tools name="Table" dockable="false" enabled="true">
	<%if(!(IsForum && !options.ContainsKey("table"))){%>	   
		<tool name="InsertTable" />
		<%}%>
		<%if(!(IsForum && !options.ContainsKey("wmv"))){%>		
		<tool name="InsertWMV" />
		<%}%>
		<%if(!(IsForum && !options.ContainsKey("emoticonselect"))){%>		
		<tool name="EkEmoticonSelect" />
		<%}%>
	</tools>
</root>


