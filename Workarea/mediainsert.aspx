<%@ Page Language="C#" AutoEventWireup="true" Inherits="mediainsert" CodeFile="mediainsert.aspx.cs" %>
<%@ Reference Control="controls/library/MediaUploaderCommon.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>mediainsert</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<meta http-equiv="Pragma" content="no-cache" />
		<asp:literal id="StyleSheetJS" runat="server"></asp:literal>
		
		<style type="text/css" >
		<!--/*--><![CDATA[/*><!--*/
		div.ektronWindow a.ektronModalClose
		{
		    background-color:transparent;
            background-image:url(images/application/close_red_sm.JPG);
            text-indent:-10000px;
            background-repeat:no-repeat;
            display:block;
            height:21px;
            overflow:hidden;
            position:absolute;
            right:0.5em;
            text-decoration:none;
            top:0.90em;
            width:21px;
            color:#FFFFFF;

	    }
	    div.ektronWindow h6
	     {
            background-color:#3163BD;
            background-image:url(images/application/darkblue_gradiant-nm.gif);
            background-position:0pt -2px;
            background-repeat:repeat-x;
            color:#FFFFFF;
            font-size:1em;
            margin:0pt;
            padding:0.6em 0.25em;
            position:relative;
          }
          /*]]>*/-->
		</style>
	</head>
	<body MS_POSITIONING="GridLayout" onload="load_handler();" class="PopupLibrary">
		<form id="Form1" method="post" runat="server">
		<div class="ektronWindow" id="selAliasDialog">
		    <div class="ektronModalHeader" >
		        <h6><strong title="Insert Quicklink"><%=m_refContentApi.EkMsgRef.GetMessage("btn insert")%>  <%=m_refContentApi.EkMsgRef.GetMessage("lbl quicklink")%></strong></h6>
		        <a href="#" class="ektronModalClose"></a>
		    </div>
            <div class="divAliasList" id="divAliasList" runat="server"></div>
            <div class="divOk" id="divOk" runat="server">
                <input type="submit" title="Ok" name="aliasSubmit" id="aliasSubmit"  size="20" value="<%=m_refContentApi.EkMsgRef.GetMessage("lbl ok")%>"  onclick="getRadioValue(0);" />
            </div>
        </div>
        
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
		    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
		</div>
		
	    <div class="ektronPageContainer ektronPageGrid">
	        <asp:datagrid id="MediaListGrid"
	            CssClass="ektronGrid"
	            OnItemDataBound="MediaListGrid_ItemBound"
	            AutoGenerateColumns="False"
	            Runat="server"
	            AllowPaging="False"
	            AllowCustomPaging="True"
	            PageSize="10"
	            EnableViewState="False"
	            PagerStyle-Visible="False" 
	            Width="100%"
	            GridLines="None">
	            <HeaderStyle CssClass="title-header" />
	        </asp:datagrid>
		    <p class="pageLinks">
			    <asp:Label ToolTip="Page" runat="server" id="PageLabel">Page</asp:Label>
			    <asp:Label id="CurrentPage" CssClass="pageLinks" runat="server" />
			    <asp:Label ToolTip="of" runat="server" id="OfLabel">of</asp:Label>
			    <asp:Label id="TotalPages" CssClass="pageLinks" runat="server" />
		    </p>
		    <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" id="FirstPage" Text="[First Page]" OnCommand="NavigationLink_Click"
			    CommandName="First" />
		    <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" id="lnkBtnPreviousPage" Text="[Previous Page]" OnCommand="NavigationLink_Click"
			    CommandName="Prev" />
		    <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" id="NextPage" Text="[Next Page]" OnCommand="NavigationLink_Click"
			    CommandName="Next" />
		    <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" id="LastPage" Text="[Last Page]" OnCommand="NavigationLink_Click"
			    CommandName="Last" />
    				
		    <asp:literal id="UpdFld" Runat="server"></asp:literal>
		    <asp:PlaceHolder ID="DataHolder" runat="server"></asp:PlaceHolder>
		    <input type="hidden" value="<%= LibType %>" name="preview_type" id="preview_type" />
		    <input type="hidden" name="preview_filename" />
		    <input type="hidden" name="contentID" id="contentID" />
		    <input type="hidden" value="<%= contLangID %>" name="contentLangId" id="contentLangId" />
		</div>
		</form>
		<script type="text/javascript">
		<!--//--><![CDATA[//><!--

		//Returns the Key/Value pairs 
        function getQuerystringValues(key, default_,url)
        {
          if (default_==null) default_="";
          key = key.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
          var regex = new RegExp("[\\?&]"+key+"=([^&#]*)");
          var qs = regex.exec(url);
          if(qs == null)
            return default_;
          else
            return qs[1];
        }
        

		var ResourceText =
		{
			sDeleteLibItem : '<asp:literal id="sDeleteLibItem" runat="server"/>'
		};

		setTimeout('initHdnVals()', 100);
		function initHdnVals()
		{
			var enhancedmetaselect = '<%=Request.QueryString["enhancedmetaselect"]%>';
			var metadataformtagid = '<%=Request.QueryString["metadataformtagid"]%>';

			if (enhancedmetaselect != '')
			{
				if ((parent.frames[0].document) && (parent.frames[0].document.forms[0]))
				{
					hdnObj = parent.frames[0].document.forms[0].enhancedmetaselect;
					if (hdnObj)
					{
						hdnObj.value = enhancedmetaselect;
					}

					hdnObj = parent.frames[0].document.forms[0].metadataformtagid;
					if (hdnObj)
					{
						hdnObj.value = metadataformtagid;
					}
				}
				else
				{
					setTimeout('initHdnVals()', 100);
					return;
				}
			}
		}

		var m_LibID, m_Folder, m_Title, m_FileName, m_Type, m_PreviewThumbnail;
		function Insert_thumb(title, filename, thumb_filename)
		{	var pastevalue;
			var popupscript;
			var ephox = "false";
			var bContentDesigner = false;
			try
			    {
			       var args = parent.GetDialogArguments();
			       if(args)
			        bContentDesigner = true;
			    }
			catch(e){}
			if ("undefined" == typeof m_Title)
			{
			    m_Title = title;
			}

			var strTitle = $ektron.htmlEncode(m_Title);
			popupscript = "try{window.open('" + encodeURI(filename) + "', 'MyImage', 'resizable=yes, scrollbars=yes, width=790, height=580')}catch(e){};return false;";
			pastevalue = '<a href="#" onclick="' + popupscript + '" onkeypress="this.onclick();" title="' + $ektron.htmlEncode(filename) + '" >';
			pastevalue += "<img src=\"" + $ektron.htmlEncode(thumb_filename) + "\" border=\"0\" alt=\"" + strTitle + "\" title=\"" + strTitle + "\" /></a>";
			try
			    {
		            var sEphoxFieldType = typeof(top.opener.document.forms[0].ephox);
		            if (sEphoxFieldType.toLowerCase() != "undefined")
		            {
			            if (typeof top.opener.document.forms[0].ephox.value != "undefined")
			            {
				            ephox = top.opener.document.forms[0].ephox.value.toLowerCase();
			            }
			        }
			    }
			catch(e){}
			if (ephox == "true") 
			{
				top.opener.insertHTML(pastevalue);
				top.close();					
			}
			else if (bContentDesigner == true) //content designer
		    {
		        parent.CloseRadDlg(pastevalue, m_Title, "thumbnail");	
		    }
			else 
			{
				if (!eWebEditProUtil.isOpenerAvailable())
				{
				    alert("Your image could not be inserted because the editor page has been closed.");
				}
				else
				{            
					// javascript:ecmPopUpWindow(filename, 'MyImage', 790, 580, 1, 1);			
					
					parent.opener.eWebEditPro.instances['<%=sEditor%>'].editor.pasteHTML(pastevalue);
					parent.close();
				}
			}
		}
	
		function ThumbnailForContentImage(thumbnail){
			m_PreviewThumbnail = thumbnail;
        }	    
		function Insert(libraryid, folder, title, filename, type, contentid){
			m_LibID = libraryid;
			m_Folder = folder;
			m_Title = title;
            if(type == 'images')
			{
			    m_FileName = filename + "?n=" + Math.floor(Math.random()* 10000 + 1);
			}
			else
			{
			    m_FileName = filename;
            }
			m_Type = type;
			document.forms[0].contentID.value = contentid;
			 
			<% if (sEditor == "JSEditor") { %>                  
			    <% if (sLinkText != "") { %> var slinktext = '<%Response.Write(sLinkText.Replace("\'","\\\'"));%>' <% } else { %> var slinktext = title; <% } %>
			    var sval = '';
			    if (type == 'images') {
			        // sval = '<img src="' + filename + '" alt="' + title + '" title="' + title + '"/>';
			        try {
			        window.opener.JSEIMGInsert(filename,title);
			    } catch(ex) {}
			    }else {
			        // sval = '<a href="' + filename + '">' + slinktext + '</a>';
			        try {
			        window.opener.JSEURLInsert(filename,slinktext);
			    } catch(ex) {}
			    }
			    self.close();
			<% } else { %>			
			    parent.SetSelectedLibraryItem(m_LibID, m_Folder, m_Type, m_Title, m_FileName);
			    //parent.mediauploader.document.forms.LibraryItem.hidden_title.value = title;
			    //parent.mediauploader.document.forms.LibraryItem.hidden_filename.value = filename;
			    //parent.mediauploader.document.forms.LibraryItem.frm_libtype.value = type;
			    //parent.mediauploader.document.forms.LibraryItem.frm_folder_id.value = folder;	
			    //parent.mediauploader.document.forms.LibraryItem.frm_library_id.value = libraryid;
			    var szPreviewFormType = typeof(document.forms[0]);
			    if (szPreviewFormType.toLowerCase() != "undefined") 
			    {
				    document.forms[0].preview_filename.value = filename;
			    }
			<% } %>
		}
		
		function SubmitInsert() {
		    
			if (document.forms[0].preview_filename.value == "") {
				alert("<%= m_refMsg.GetMessage("js: alert double click lib name") %>");
				return false;
			}
			var retFiled = "";
			retField = '<%=Request.QueryString["retfield"]%>';
			sitePath = '<%=SitePath%>';
			
			if (m_FileName.indexOf('javascript') != -1)
		    {
			    var b_Slash = document.forms[0].preview_filename.value.indexOf('\\');
						 
			    if(b_Slash != -1)
    			{    			    
    			    document.forms[0].preview_filename.value = document.forms[0].preview_filename.value.replace(/\\/g,'');
    			    m_FileName = document.forms[0].preview_filename.value;    			 
    			}    			
			}
			
			if ((m_Type != "") && (m_Type == "quicklinks" || m_Type == "forms" )) 
			{
			    if(m_FileName.toLowerCase().indexOf("linkit.aspx") > -1)
			        m_FileName = m_FileName + "&libID=" + m_LibID ;
			    else
			        m_FileName = m_FileName;
			 
			  showSelAliasdialog();
			  return false;
			  
			}
			else
			{
			  InsertValue();
			}
		}	
		function InsertValue()
		{
			if ((m_Type != "") && (m_FileName != "") && (m_Title != ""))
			{
			    if (retField != '')
			    {
			        InserValueToField(m_FileName,m_PreviewThumbnail, sitePath, retField);
			        //retField = parent.opener.document.getElementById(retField);
			        //if (eval(retField) != null)
			        //{
			        //    retField.value = m_FileName.replace(sitePath, '');
					//    parent.close();
			        //}
			    }
			    else
			    {
				    InsertFunction(m_FileName, m_Title, m_Type, m_LibID);
				}
			}
			//parent.mediauploader.document.forms.LibraryItem.frm_insert_server_file.value = 1;
			//parent.mediauploader.EditorInsert('all');
		}

		function PreviewFunct(oldURL){
			var regexp1 = / /gi;
			if (document.forms[0].preview_filename.value == "") {
				alert("<%= m_refMsg.GetMessage("js: alert single click lib name") %>");
				return false;
			}
			else {
				if (document.forms[0].preview_type.value == "quicklinks") {
					if (document.forms[0].preview_filename.value.indexOf("?") != -1) {
						var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20") + "&Preview=True";
					}
					else {
						var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20") + "?Preview=True";
					}
				}
				else if ((document.forms[0].preview_type.value == "files") 
						|| (document.forms[0].preview_type.value == "images")) {
					var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20");
				}
				else if (document.forms[0].preview_type.value == "hyperlinks") {
					var tempString = document.forms[0].preview_filename.value.toLowerCase();
					if ((tempString.substring(0,7) == "http://")
							|| (tempString.substring(0,8) == "https://") || (tempString.substring(0,1) == "/")) {
						var tempHREF = document.forms[0].preview_filename.value.replace(regexp1, "%20");
					}
					else {
						var tempHREF = "http://" + document.forms[0].preview_filename.value.replace(regexp1, "%20");
					}			
				}
				for (var i = 0; i < document.links.length; i++) {
					if (document.links[i].href == oldURL) {
						break;
					}
				}
				document.links[i].href = tempHREF;
				return true;
			}
		}
		
		function previewItem(scope) {
			var regexp1 = / /gi;
			var filename = document.forms[0].preview_filename.value;		
			//check if there is a value
			if (filename == "") {
				alert("<%= m_refMsg.GetMessage("js: alert single click lib name")%>");				
			} else {
				//create the popup 
				popup = window.open('', 'imagePreview', 'width=600,height=450,left=100,top=75,screenX=100,screenY=75,scrollbars,location,menubar,status,toolbar,resizable=1');
				//start writing in the html code
				popup.document.writeln("<html><body bgcolor='#FFFFFF'>");
				//get the extension of the file to see if it has one of the image extensions					
				if (scope == "images") {					
					popup.document.writeln("<img src='" + filename + "' />");
				}
				else if(scope == "hyperlinks") {
					var tempString = document.forms[0].preview_filename.value.toLowerCase();
					var tempHREF;
					if ((tempString.substring(0,7) == "http://")
							|| (tempString.substring(0,8) == "https://") || (tempString.substring(0,1) == "/")) {
						tempHREF = document.forms[0].preview_filename.value;						
					}
					else {
						tempHREF = "http://" + document.forms[0].preview_filename.value;
					}
					popup.document.writeln("<a href='" + tempHREF.replace(regexp1, "%20") + "'>" + tempHREF + "</a>");
					
				}
				else {
					//if not extension fron list above write URL to file 
					popup.document.writeln("<a href='" + filename + "'>" + filename + "</a>");
				}
				popup.document.writeln("</body></html>");
				popup.document.close();
				popup.focus();
			}
		}
		
		var lastSelected = null;
		var lastSelectedColor;
		
		function Blink(CellName, Color) {
			if (lastSelected != CellName) {
				if (lastSelected != null) {
					if (document.all) {
						document.all[lastSelected].style.backgroundColor = lastSelectedColor;
						document.all[lastSelected + "_0"].style.backgroundColor = lastSelectedColor;
						<%if (LibType == "quicklinks" || LibType == "forms") {	%>
							document.all[lastSelected + "_2"].style.backgroundColor = lastSelectedColor;
						<%} else {%>
							document.all[lastSelected + "_1"].style.backgroundColor = lastSelectedColor;
						<%}%>
					}
					else if (document.getElementById) {
						// Bugfix for Safri/Mac: Mac Leaves previous selection 
						// highlighted, if new background color value is a 
						// zero length string (fails to update in this case).
						if (('' == lastSelectedColor) && (IsPlatformMac()) && (IsBrowserSafari())) {
							//alert('condition met!');
							lastSelectedColor = 'white';
						}
						var MyObj = document.getElementById(lastSelected);
						MyObj.style.background = lastSelectedColor;
						var MyObj = document.getElementById(lastSelected + "_0");
						MyObj.style.background = lastSelectedColor;
						<%if (LibType == "quicklinks" || LibType == "forms") {	%>
							var MyObj = document.getElementById(lastSelected + "_2");
							MyObj.style.background = lastSelectedColor;
						<% } else {%>
							var MyObj = document.getElementById(lastSelected + "_1");
							MyObj.style.background = lastSelectedColor;
						<%}%>
					}
					else {
						var layername = "layer" + lastSelected;
						var NsObj = document.layers[layername];
						NsObj.bgColor = lastSelectedColor;
						var NsObj = document.layers[layername + "_0"];
						NsObj.bgColor = lastSelectedColor;
						<%if (LibType == "quicklinks" || LibType == "forms") {	%>
							var NsObj = document.layers[layername + "_2"];
							NsObj.bgColor = lastSelectedColor;
						<% } else  {%>
							var NsObj = document.layers[layername + "_1"];
							NsObj.bgColor = lastSelectedColor;
						<%}%>
					}
				}
				lastSelected = CellName;

				if (document.all) {
					lastSelectedColor = document.all[CellName].style.backgroundColor;
				}
				else if (document.getElementById) {
					var MyObj = document.getElementById(CellName);
					lastSelectedColor = MyObj.style.background;
				}
				else {
					var layername = "layer" + CellName;
					var NsObj = document.layers[layername];
					lastSelectedColor = NsObj.bgColor;
				}
			}
			lastSelected = CellName;
			if (document.all) {
				document.all[CellName].style.backgroundColor = Color;
				document.all[CellName + "_0"].style.backgroundColor = Color;
				<%if (LibType == "quicklinks" || LibType == "forms") {	%>
					document.all[CellName + "_2"].style.backgroundColor = Color;
				<% } else  {%>
					document.all[CellName + "_1"].style.backgroundColor = Color;
				<%}%>
			}
			else if (document.getElementById) {
				var MyObj = document.getElementById(CellName);
				MyObj.style.background = Color;
				var MyObj = document.getElementById(CellName + "_0");
				MyObj.style.background = Color;
				<%if (LibType == "quicklinks" || LibType == "forms") {	%>
					var MyObj = document.getElementById(CellName + "_2");
					MyObj.style.background = Color;
				<% }else {%>
					var MyObj = document.getElementById(CellName + "_1");
					MyObj.style.background = Color;
				<%}%>
			}
			else {
				var layername = "layer" + CellName;
				var NsObj = document.layers[layername];
				NsObj.bgColor = Color;
				var NsObj = document.layers[layername + "_0"];
				NsObj.bgColor = Color;
				<%if (LibType == "quicklinks" || LibType == "forms") {	%>
					var NsObj = document.layers[layername + "_2"];
					NsObj.bgColor = Color;
				<% } else {%>
					var NsObj = document.layers[layername + "_1"];
					NsObj.bgColor = Color;
				<%}%>
			}
			$ektron("img[src *= 'delete.png']").closest("td").show();
		}
		
		function updateFolders(Folder, folderType, imagepermission, filepermission, overwritepermission, libid){
			alert("UpdateFolders!");
			//parent.mediauploader.document.forms.LibraryItem.frm_folder_id.value = Folder;
			//if ((imagepermission != 0) && (imagepermission != 2)) {
			//	imagepermission = 1;
			//}
			//if ((filepermission != 0) && ((filepermission != 2))) {
			//	filepermission = 1;
			//}
			//if ((overwritepermission != 0) && ((overwritepermission != 2))) {
			//	overwritepermission = 1;
			//}
			//parent.mediauploader.document.forms.LibraryItem.frm_folder_imagepermission.value = imagepermission;
			//parent.mediauploader.document.forms.LibraryItem.frm_folder_filepermission.value = filepermission;
			//parent.mediauploader.document.forms.LibraryItem.frm_folder_overwritepermission.value = overwritepermission;
			//parent.mediauploader.document.forms.LibraryItem.frm_library_id.value = libid;
			//parent.mediauploader.document.forms.LibraryItem.frm_libtype.value = folderType;
		}
		
		function ClearFolderInfo() {
			return true;
		}
		function SelLibLanguage(inVal) {
			// document.location = 'collections.aspx?action=ViewMenuReport&LangType=' + inVal ;
			// TODO: Use RegEx to replace the querystring			
			//document.location = '<%=Request.ServerVariables["PATH_INFO"] + "?" + Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.ServerVariables["QUERY_STRING"].Replace("LangType", "L").Replace("SelectAll=1&", "").Replace("\'", "\\\'"))%>&LangType=' + inVal ;
			var link1 = '<asp:literal id="jsLink1" runat="server"/>';	
		    document.location.href = link1 + inVal;	
		}
		
		var m_isMac = false;
		var m_isMacInit = false;
		function IsPlatformMac() {
			if (m_isMacInit) {
				return (m_isMac);
			} else {
				var posn;
				var sUsrAgent = new String(navigator.userAgent);
				sUsrAgent = sUsrAgent.toLowerCase();
				posn = parseInt(sUsrAgent.indexOf('mac'));
				m_isMac = (0 <= posn);
				m_isMacInit = true;
				return (m_isMac);
			}
		}

		var m_isSafari = false;
		var m_isSafariInit = false;
		function IsBrowserSafari() {
			if (m_isSafariInit) {
				return (m_isSafari);
			} else {
				var posn;
				var sUsrAgent = new String(navigator.userAgent);
				sUsrAgent = sUsrAgent.toLowerCase();
				posn = parseInt(sUsrAgent.indexOf('safari'));
				m_isSafari = (0 <= posn);
				m_isSafariInit = true;
				return (m_isSafari);
			}
		}
		function load_handler()
		{
		    try 
		    {
		        parent.SetLoadStatus('insert');
		    }
		    catch( ex ) { }
		}
		
		Ektron.ready(function() 
		{
		    $ektron("#selAliasDialog").modal({
                modal: true,
                overlay: 0,
                trigger: ""
                });
                $ektron("img[src *= 'delete.png']").closest("td").hide();
		});
		
		function showSelAliasdialog()
		{
		  $ektron.ajax({
              url: "urlaliasdialogHandler.ashx?action=getaliaslist&contID=" + document.forms[0].contentID.value + "&LangType=" + document.forms[0].contentLangId.value,
              cache: false,
              success: function(html){
                    if (html.indexOf("<error>") == -1) {
                        $ektron("#divAliasList").empty();
                        $ektron("#divAliasList").append("<p>" + html + "</p>");
                        if(html.indexOf("<aliascount>") != -1) {
                          if(html.indexOf("<linkmanage>")!=-1) {
                                $ektron("#selAliasDialog").modalShow();
                                return false;
                          }
                          else {
                                getRadioValue(1);
                                return false;
                          }
                        }
                        else {
                          $ektron("#selAliasDialog").modalShow();
                          return false;
                        }
                    } else{
                        InsertValue();
                        //SubmitInsert();
                    }
              }
            });
		 }
		 
		 function SaveAlias(selradio)
		 {
		    document.forms[0].preview_filename.value= selradio;
		    m_FileName = document.forms[0].preview_filename.value;
            if ('<%=Request.QueryString["enhancedmetaselect"]%>' != '' && '<%=Request.QueryString["enhancedmetaselect"]%>' == '1') {
                m_FileName = m_FileName + '?id=' + document.forms[0].contentID.value;
            }
            InsertValue();  
		 }
		 
		 function getRadioValue(count) 
		 {
		    if(count ==1 ) {
		      var radioValue = document.forms.Form1.aliasSelect.value;
		      SaveAlias(radioValue);
			  return false;
		    } else {
		        var index;
		        for(index=0; index < document.forms.Form1.aliasSelect.length; index++)
		        {
			        if (document.forms.Form1.aliasSelect[index].checked) {
					        var radioValue = document.forms.Form1.aliasSelect[index].value;
					        SaveAlias(radioValue);
					        break;
				    }
			      }  		       	
    		   }   
	       }
	       
         function SubmitDelete()
         {
            if(confirm(ResourceText.sDeleteLibItem))
            {
    			location.href = "mediainsert.aspx?action=deleteItem&folderid=" + this.m_Folder + "&id=" + this.m_LibID + "";
                return false;
            }
         }
		//--><!]]>
        </script>
	</body>
</html>

