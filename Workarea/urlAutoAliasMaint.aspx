<%@ Page Language="C#" AutoEventWireup="true" CodeFile="urlAutoAliasMaint.aspx.cs"
    Inherits="Workarea_urlAutoAliasMaint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Automatic Alias</title>

    <asp:literal id="StyleSheetJS" runat="server" />

    <script type="text/javascript">
        <!--//--><![CDATA[//><!--

            function SubmitForm(FormName, Validate) {
            resetCPostback();
	            if (Validate.length > 0) {
		            if (eval(Validate)) {
			            document.forms[FormName].submit();
			            return false;
		            }
		            else {
			            return false;
		            }
	            }
	            else {
		            document.forms[FormName].submit();
		            return false;
	            }
            }
           function resetCPostback(){
            document.forms["frm_autoalias"].isCPostData.value = "";
            }

            function VerifyAddAlias() {
	            var es = '' ;

	            if((document.forms.frm_autoalias.ddlSource && document.forms.frm_autoalias.ddlSource.value=='' )|| (document.forms.frm_autoalias.txtFolderPath && document.forms.frm_autoalias.txtFolderPath.value == '')) {
		            es+= '<asp:Literal id="ltr_noSrcSel" runat="server" />' ;
		            document.forms["frm_autoalias"].isCPostData.value = "false";
	            }
	            if (document.forms.frm_autoalias.ddlExt.value==''){
		            es += '<asp:Literal id="ltr_noExtSel" runat="server" />' ;
		            document.forms["frm_autoalias"].isCPostData.value = "false";
	            }
                if(es!='') {
		            alert('<asp:Literal id="ltr_msgFollErr" runat="server" />' + es) ; return false;
		            document.forms["frm_autoalias"].isCPostData.value = "false";
	            }
	            else {
		            return true ;
	            }
            }

             Ektron.ready(function() {
                $ektron('#FolderSelect').modal({
                    toTop: true,
                    trigger: '.viewFolderList',
                    modal: true,
                    onShow: function(hash)
                    {
                        hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
                        hash.o.fadeTo("fast", 0.5, function()
                        {
			                hash.w.fadeIn("fast");
		                });
                    },
                    onHide: function(hash)
                    {
		                hash.w.fadeOut("fast");
		                hash.o.fadeOut("fast", function()
		                {
		                    if (hash.o)
		                    {
			                    hash.o.remove();
		                    }
		                });
	                }
                });
                $ektron('#TaxonomySelect').modal({
                    toTop: true,
                    trigger: '.viewTaxonomyList',
                    modal: true,
                    onShow: function(hash)
                    {
                        hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
                        hash.o.fadeTo("fast", 0.5, function()
                            {
			                    hash.w.fadeIn("fast");
		                    }
		                );
                    },
                    onHide: function(hash)
                    {
		                hash.w.fadeOut("fast");
		                hash.o.fadeOut("fast", function()
		                    {
		                        if (hash.o)
		                        {
			                        hash.o.remove();
		                        }
		                    }
		                );
	                }
                });

                $ektron(".ui-widget-header .ektronModalClose").hover(
                    function(){
                        $ektron(this).addClass("ui-state-hover");
                    },
                    function(){
                        $ektron(this).removeClass("ui-state-hover");
                    }
                );
              });

              function ReturnChildValue(folderid, folderpath, targetFolderIsXml)
		            {
			            // take value, store it, write to display

			            if(folderid == 0 || folderpath == '\\')
			            {
			              alert('<asp:Literal id="ltr_rootNotAliased" runat="server" />');
			              return false;
			            }
        				$ektron('#FolderSelect').modalHide();
			            $ektron('#TaxonomySelect').modalHide();
			            folderpath = folderpath.replace(/\\/g,'/');
			            document.getElementById("frm_folder_id").value = folderid;
			            document.getElementById("frm_folder_path").value = folderpath;
			            document.getElementById("txtFolderPath").value = folderpath;
			            GetLinkPreview(folderpath);
			            SplitPath();
		            }
	            function GetLinkPreview(folderpath)
	            {

	                var aliasNameType;
	                var selectedfolderpath = folderpath;
	                if( selectedfolderpath == "")
	                {
	                    var pathList = document.forms[0].pathList;
	                    folderpath = document.getElementById("txtFolderPath").value;
	                    var fullFolderpath = folderpath.substring(1) + "/";
	                    if ( fullFolderpath == pathList.options[pathList.selectedIndex].text)
	                    {
	                        selectedfolderpath = "";
	                    }
	                    else
	                    {
	                        selectedfolderpath = folderpath.replace(pathList.options[pathList.selectedIndex].text,"");
	                    }
	                }
	                if( "ContentTitle" == document.getElementById("ddlNameSrc").value )
			            {
			                    aliasNameType = "ContentTitle";
			            }
			            else if ( "ContentId" == document.getElementById("ddlNameSrc").value )
			            {
			                    aliasNameType = "354";

			            }
			            else
			            {
			                    aliasNameType = "354/1033";
			            }
			            $ektron("#txtExample").attr("value", selectedfolderpath + "/"+ aliasNameType + document.getElementById("ddlExt").value);
        //				$ektron("#hdn_txtExample").attr("value", selectedfolderpath + "/"+ aliasNameType + document.getElementById("ddlExt").value);
	            }
                function SplitPath()
                {
                  var seperator = "/";
                  var slashPos = 0;
                  var i =0;
                  var optArr = "";
                  var path = document.getElementById("txtFolderPath").value;
                  path = path.substring(1);
                  path = path + "/";

                  if( document.getElementById("txtFolderPath").value != "" )
                  {

                      optArr += "<option> Please Select </option>";
                      for(i =0 ; i<= path.length; i++)
                      {
                            slashPos = path.indexOf("/",slashPos);
                            if(slashPos == -1)
                            {
                              break;
                            }
                            slashPos= slashPos+1;

                            if(document.getElementById("frm_selected_path").value != "" && document.getElementById("frm_selected_path").value == path.substring(0,slashPos))
                            {
                            optArr += "<option selected>" + path.substring(0,slashPos) +  "</option>";
                            }
                            else
                            {
                             optArr += "<option>" + path.substring(0,slashPos) +  "</option>";
                            }
                       }

                       $ektron('#pathList option').remove();
                       $ektron('#pathList').append(optArr);
                    }
             }
        //--><!]]>
    </script>

    <style type="text/css">
        <!--/*--><![CDATA[/*><!--*/
	        .viewFolderList { display: inline-block !important;}
	        .viewTaxonomyList{ display: inline-block !important;}
	        div.modalIframeWrapper {border: solid 1px #ccc;}
	        div.modalIframeWrapper iframe.modalIframe {height: 30em; width: 100%;}
	    /*]]>*/-->
	</style>
</head>
<body onload="SplitPath()">
    <form id="frm_autoalias" runat="server">
        <input type="hidden" value="<%= sourceID %>" name="frm_folder_id" id="frm_folder_id" />
        <input type="hidden" value="<%= sourcePath %>" name="frm_folder_path" id="frm_folder_path" />
        <input type="hidden" value="<%= excludedPath %>" name="frm_selected_path" id="frm_selected_path" />

        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <asp:CheckBox ToolTip="Active Option" ID="activeChkBox" runat="server" Checked="true" />
            <asp:Label ID="lblActive" runat="server" />

                <div class="ektronTopSpace"></div>
                <fieldset id="linkSource">
                    <legend><strong title="URL Definition"><%=msgHelper.GetMessage("lbl urldef")%></strong></legend>
                    <%-- <%=MsgHelper.GetMessage("lbl link src desc")%>--%>
                    <div class="ektronWindow ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all" id="FolderSelect">
                        <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix propertiesModalHeader">
                            <span class="ui-dialog-title header"><%=msgHelper.GetMessage("lbl select folder")%></span>
                            <a title="Close" href="#" class="ui-dialog-titlebar-close ui-corner-all ektronModalClose"><span class="ui-icon ui-icon-closethick">&nbsp;</span></a>
                        </div>
                        <div class="ui-dialog-content ui-widget-content">
                            <div class="modalIframeWrapper">
                                <iframe frameborder="0" scrolling="auto" src="urlAutoAliasSourceSelector.aspx?FolderID=<%=folderId%>&browser=1&WantXmlInfo=1&noblogfolders=0&mode=Folder" class="modalIframe">
                                </iframe>
                            </div>
                        </div>
                    </div>
                    <div class="ektronWindow ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all" id="TaxonomySelect">
                        <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix propertiesModalHeader">
                            <span class="ui-dialog-title header" title="Select Taxonomy"><%=msgHelper.GetMessage("lbl select taxonomy")%></span>
                            <a title="Close" href="#" class="ui-dialog-titlebar-close ui-corner-all ektronModalClose"><span class="ui-icon ui-icon-closethick">&nbsp;</span></a>
                        </div>
                        <div class="ui-dialog-content ui-widget-content">
                            <div class="modalIframeWrapper">
                                <iframe frameborder="0" scrolling="auto" src="urlAutoAliasSourceSelector.aspx?FolderID=0&browser=1&WantXmlInfo=1&noblogfolders=1&mode=Taxonomy" class="modalIframe">
                                </iframe>
                            </div>
                        </div>
                    </div>
                    <table class="ektronGrid">
                        <tr>
                            <td class="label"><asp:Label ID="lblType" runat="server" /></td>
                            <td><asp:DropDownList ToolTip="Select Source Type from the Drop Down Menu" ID="ddltype" AutoPostBack="true" OnSelectedIndexChanged="GetSourceList" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Label ID="lblSource" runat="server" /></td>
                            <td>
                                <asp:DropDownList ToolTip="Select Alias Root from the Drop Down Menu" ID="ddlSource" AutoPostBack="true" OnSelectedIndexChanged="ShowLinkExample" runat="server" />
                                <asp:TextBox ID="txtFolderPath" ToolTip="Enter Folder Path here"  CssClass="ektronTextMedium" Enabled="false" runat="server" />
                                <asp:Label ID="lblFolderSelect" runat="server"><a title="Folder List" href="#" class="button greenHover buttonCheckAll viewFolderList">Select</a></asp:Label>
                                <asp:Label ID="lblTaxonomySelect" runat="server"><a title="Taxonomy List" href="#" class="button greenHover buttonCheckAll viewTaxonomyList">Select</a></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Label ID="lblNameSrc" runat="server" /></td>
                            <td><asp:DropDownList ToolTip="Select Alias Format from the Drop Down Menu" ID="ddlNameSrc" AutoPostBack="true" OnSelectedIndexChanged="ShowLinkExample" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Label ID="lblExt" runat="server" /></td>
                            <td><asp:DropDownList ToolTip="Select Extension from the Drop Down Menu" ID="ddlExt" AutoPostBack="true" OnSelectedIndexChanged="ShowLinkExample" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Label ID="lblReplaceChar" runat="server" /></td>
                            <td class="value"><asp:TextBox ToolTip="Enter Replacement Character here" ID="txtReplaceChar" Text="_" MaxLength="1" Width="13" runat="server" /><%--&nbsp;&nbsp;&nbsp;&nbsp;(<%=MsgHelper.GetMessage("lbl default")%>:&nbsp;"_")--%></td>
                        </tr>
                        <tr>
                        <td class="label"><asp:Label id="lblQueryStringParam" runat="server" /></td>
                        <td OnTextChanged="txtQueryStringParam_TextChanged"><asp:TextBox ToolTip="Enter Soure Query String Parameters here" ID="txtQueryStringParam" runat="server" AutoPostBack="true" OnTextChanged="txtQueryStringParam_TextChanged" /></td>
                        </tr>
                        <tr id="primary" visible="false" runat="server">
                            <td class="label" colspan="2">
                                <asp:CheckBox ToolTip="Primary Option" ID="primaryChkBox" runat="server" />
                                <asp:Label ID="lblPrimary" runat="server" />
                            </td>
                        </tr>
                    </table>
                </fieldset>

                <div class="ektronTopSpace"></div>
                <fieldset id="enhanceAlias">
                    <legend><strong title="Customize Alias Path"><%=msgHelper.GetMessage("lbl customize alias path")%></strong></legend>
                    <table class="ektronGrid">
                        <tr>
                            <td Class="label"><asp:Label ID="lblPathList" runat="server" /></td>
                            <td><select title="Select Path to Exclude from the Drop Down Menu" id="pathList" name="pathList" runat="server" onchange="GetLinkPreview('');"></select></td>
                        </tr>
                    </table>
                </fieldset>

                <div class="ektronTopSpace"></div>
                <fieldset id="previewAlias">
                    <legend><strong title="Preview Alias"><%=msgHelper.GetMessage("lbl preview alias")%></strong></legend>
                    <table class="ektronGrid">
                        <tr>
                            <td Class="label"><asp:Label ID="lblOriginal" runat="server" /></td>
                            <td><asp:TextBox ToolTip="Enter Origional Url here" ID="txtOriginal" Enabled="false" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Label ID="lblExample" runat="server" /></td>
                            <td><input title="Enter Link Example Preview here" type="text" id="txtExample" runat="server" /></td>
                        </tr>
                    </table>
                </fieldset>
        </div>

        <%--<input type="hidden" id="hdn_txtExample" name="hdn_txtExample" runat="server" />--%>
        <input type="hidden" runat="server" id="isCPostData" value="false" />
    </form>
</body>
</html>

