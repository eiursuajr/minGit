<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddEditDeviceConfiguration.ascx.cs" Inherits="AddEditDeviceConfiguration" %>
<%@ Register Src="../../pagebuilder/foldertree.ascx" TagName="FolderTree" TagPrefix="CMS" %>
<style type="text/css" >
    .DeviceModel
    {
      white-space: nowrap;
    }
    span.spanHover:hover{color:blue;}
    .MainCentreBottom
     {
        font-size:7.8pt; 
        text-align:center;
        color:#5D8FB4;
     }
</style>
<script type="text/javascript">
        var controlid = "deviceConfiguration_";
        var arDeviceNodes = new Array();
        var selectedNode;
        
        function node(title, order)
        {
            this.title = title;
            this.order = order;
        }
        
         CheckKeyValue = function (item, keys) {
			var keyArray = keys.split(",");
			for (var i = 0; i < keyArray.length; i++) {
				if ((document.layers) || ((!document.all) && (document.getElementById))) {
					if (item.which == keyArray[i]) {
						return false;
					}
				}
				else {
					if (event.keyCode == keyArray[i]) {
						return false;
					}
				}
			}
		}

           
       function ArrayContains(obj,find) 
         {
            for (var i = 0;i< obj.length; i++)
            {
                if (obj[i].title == find)
                     return true;
            }
            return false;
         }
        
        function addDeviceModelNode(button)
        {            
           if (Trim(button) == '')
           {
               alert("Device model is required.");
               return false;
           }
           
           if(!ArrayContains(arDeviceNodes,button.toString()))
           {
                objNode = new node(button.toString() ,arDeviceNodes.length);
                arDeviceNodes[arDeviceNodes.length] = objNode;   
                createNode(objNode,"<%=_action %>");
                return true;
           }
           else
           {
                alert("Device model already exists");
                return false;
           }
        }
        
        function renderDeviceModels()
         {	
    							    												        
            for (var i = 0; i < arDeviceNodes.length; i++)
            {
                createNode(arDeviceNodes[i],"<%=_action %>");
            }
         }
        
        function createNode(node,action)
        {
            var span_elem; 
            var att;
            var NodesCollection ;
            if(action == "adddeviceconfiguration" )
               NodesCollection = document.getElementById("add_device_nodes") 
            else
               NodesCollection = document.getElementById("edit_device_nodes");
            var firfox = document.createElement("div")
            
            var newEntry = document.createElement("div")
            newEntry.id = "device_" + node.title.toLowerCase();
            newEntry.className = "DeviceModel";
            
            span_elem = document.createElement("span");
            span_elem.id = "devicemodel_links_" + node.title.toLowerCase();
            span_elem.innerHTML = "<img onclick=\"deleteNode('" + node.title + "','" + node.order + "');\" alt=\"Delete path\" src=\"images/UI/Icons/delete.png\" />&nbsp;&nbsp;&nbsp;&nbsp;";
            newEntry.appendChild(span_elem);
         
            span_elem = document.createElement("span");
            span_elem.id = "devicemodel_title_" + node.title.toLowerCase();
            span_elem.innerHTML =  node.title;
            newEntry.appendChild(span_elem);
            
            firfox.appendChild(newEntry)
            NodesCollection.appendChild(firfox);
        }        
        
            
        function deleteNode(id,order)
        {
            var obj = document.getElementById("device_" + id.toLowerCase()).parentNode;
            obj.parentNode.removeChild(obj);
            arDeviceNodes.splice(order, 1);
            for (var i=0; i< arDeviceNodes.length; i++) 
            {
                arDeviceNodes[i].order = i;
            }
        }
        
        function Validate()
        {
            //var deviceField ;
            if("<%=_action%>" == 'adddeviceconfiguration')
            {
                if(document.getElementById(controlid + "tbAddDeviceName").value=='')
                {
                    alert('<%=_MessageHelper.GetMessage("js:alert device name required field")%>');
                    return false;
                }
                  
                //deviceField = document.getElementById(controlid + "tbAddPreviewTemplate").value
//                if(deviceField =='')
//                {
//                    alert('<%=_MessageHelper.GetMessage("js:alert device preview required field")%>');
//                    return false;
//                }
                  
                if(document.getElementById(controlid + "tbAddPreviewWidth").value=='')
                {
                    alert('<%=_MessageHelper.GetMessage("js:alert device preview width required field")%>');
                    return false;
                }
                  
                if(document.getElementById(controlid + "tbAddPreviewHeight").value=='')
                {
                    alert('<%=_MessageHelper.GetMessage("js:alert device preview height required field")%>');
                    return false;
                }
                if(isNaN(document.getElementById(controlid + "tbAddPreviewWidth").value) || isNaN(document.getElementById(controlid + "tbAddPreviewHeight").value))
                {
                    alert('<%=_MessageHelper.GetMessage("js: alert device width height numeric") %>');
                    return false;
                }
                document.getElementById(controlid +"tbAddDeviceName").value =document.getElementById(controlid + "tbAddDeviceName").value.replace(/</g, "").replace(/>/g, "");
              }
              else
              {
                if(document.getElementById(controlid + "tbEditDeviceName").value=='')
                {
                     alert('<%=_MessageHelper.GetMessage("js:alert device name required field")%>');
                     return false;
                }
                  
                //deviceField = document.getElementById(controlid + "tbEditPreviewTemplate").value
//                if(deviceField =='')
//                {
//                    alert('<%=_MessageHelper.GetMessage("js:alert device preview required field")%>');
//                    return false;
//                }                           
                 
                if(document.getElementById(controlid + "tbEditPreviewWidth").value=='')
                {
                    alert('<%=_MessageHelper.GetMessage("js:alert device preview width required field")%>');
                    return false;
                }
                  
                if(document.getElementById(controlid + "tbEditPreviewHeight").value=='')
                {
                    alert('<%=_MessageHelper.GetMessage("js:alert device preview height required field")%>');
                    return false;
                }
                if(isNaN(document.getElementById(controlid + "tbEditPreviewWidth").value) || isNaN(document.getElementById(controlid + "tbEditPreviewHeight").value))
                {
                    alert('<%=_MessageHelper.GetMessage("js: alert device width height numeric") %>');
                    return false;
                }
                document.getElementById(controlid +"tbEditDeviceName").value =document.getElementById(controlid + "tbEditDeviceName").value.replace(/</g, "").replace(/>/g, "");
            }
              
//            if ((deviceField.indexOf("\\") >= 0) || (deviceField.indexOf(":") >= 0) || (deviceField.indexOf("*") >= 0) || (deviceField.indexOf("\"") >= 0) || (deviceField.indexOf("<") >= 0) || (deviceField.indexOf(">") >= 0) || (deviceField.indexOf("|") >= 0) || (deviceField.indexOf("\'") >= 0))
//            {
//                        alert("Device preview template name cannot include ('\\', ':', '*', ' \" ', '<', '>', '|', '\'').");
//                        return ;
//            }
              
            if(arDeviceNodes.length == 0)
            {
                 alert('<%=_MessageHelper.GetMessage("js:alert device models required")%>');
                     return false;
            }
              
            SaveDeviceModels();
            document.forms[0].submit();
		}
        function OnDeviceClicked(path)
        {
           if(addDeviceModelNode(path))
           {
                $ektron("#dlgBrowse").modal().modalHide();
           }
        }
        
        function SaveDeviceModels()
        {
            var node;
            var serializeNode = "";
            for (var i=0;i<arDeviceNodes.length;i++)
            {
                node = arDeviceNodes[i];
                serializeNode += "<node>";
                serializeNode += "<title>" +node.title.toString() + "</title>";
                serializeNode += "</node>";
            }
            serializeNode = "<devicemodels>" + serializeNode + "</devicemodels>";
            document.getElementById("savedDeviceModelList").value = encodeURIComponent(serializeNode);
        }
        
        function ShowBrowseDialog()
        {

        }
                
        function OnFileClicked(path)
        {
            var input;
            //input = $ektron("#" + controlid + "tbEditPreviewTemplate");
//            if(input.length == 0)
//                input = $ektron("#" + controlid + "tbAddPreviewTemplate");
            if(input.length == 0)
                return;
            input.attr("value", path.replace(/\\/g, "/"));
            $ektron("#dlgPreview").modal().modalHide();
        }
        
         Ektron.ready(function ()
          {
                // Initialize browse dialog
            $ektron("#dlgBrowse").modal({height: 100, modal: true,  onShow: function(h) {
                $ektron("#dlgBrowse").css("margin-top", -1 * Math.round($ektron("#dlgBrowse").outerHeight()/2));h.w.show();
            }});
             $ektron("#dlgPreview").modal({height: 100, modal: true,trigger: ".ektronModalPreview", onShow: function(h) {
                $ektron("#dlgPreview").css("margin-top", -1 * Math.round($ektron("#dlgPreview").outerHeight()/2));h.w.show();
            }});
             $ektron("div.folderTree li span").css("cursor", "pointer").css("cursor", "pointer");
             $ektron("div.folderTree li span").addClass("spanHover");
          });
      
</script>


      <div id="dhtmltooltip"></div>
      <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
      </div>
      <div id="dlgBrowse" class="ektronWindow ektronModalStandard" >
                <div class="ektronModalHeader">
                  <h3>
                    <asp:Literal ID="selectDevice" runat="server" />
                    <asp:HyperLink  ID="closeDialogLink" CssClass="ektronModalClose" runat="server" />
                  </h3>
                </div>
                <div class="ektronModalBody" style="height: 425px;width: auto;" >
                    <iframe id="iframeAddItemsModal" scrolling="no" width="100%" height="80%"  runat="server" frameborder="0" ></iframe>
                    <div style="margin-top: -5px;">
                        <p class="cancelAddItemModal clearfix">
                            <a title='<%=this._MessageHelper.GetMessage("tooltip:close the device selection screen") %>' href="#Close" class="redHover button buttonRight ektronModalClose" onclick="return false;">
                                <asp:Image ID="imgCloseAddItemModal" runat="server"  />
                                <span><%=this._MessageHelper.GetMessage("close title") %></span>
                            </a>
                        </p>
                    </div>
                    <div class ="MainCentreBottom">
                        <asp:Literal id="ltrNotification" runat="server" ></asp:Literal>
                    </div>
                </div>
       </div>
       <div id="dlgPreview" class="ektronWindow ektronModalStandard">
                <div class="ektronModalHeader">
                  <h3>
                    <asp:Literal ID="selectTemplate" runat="server" />
                    <asp:HyperLink  ID="closeDialogLnk" CssClass="ektronModalClose" runat="server" />
                  </h3>
                </div>
                <div class="ektronModalBody"  style="height: 500px;width: auto; overflow:scroll;">
                  <div class="folderTree">
                    <CMS:FolderTree ID="folderTree" runat="server" Filter="[.]aspx$" />
                  </div>
                </div>
            </div>
            
       <asp:MultiView ID="deviceConfigurationView" runat="server">
           <asp:View ID="Add" runat="server" >
               <div class="ektronPageContainer">
                     <div id="dvDeviceAdd" class="ektronPageInfo">
                         <table class="ektronForm">
                             <tr>
                                <td class="label" title="<%=_MessageHelper.GetMessage("lbl Device")%>"><%=_MessageHelper.GetMessage("lbl Device")%>:</td>
                                <td class="value">
                                   <asp:TextBox Columns="100" ToolTip="Enter title of the Device Configuration." id="tbAddDeviceName" runat="server"/>
                                </td>
                             </tr>
                                 
		                          <tr>
                                     <td class="label" runat="server" id="lbladddisplayfor" title=""></td>
                                     <td class="value">
                                        <a  title="Click here to browse device models." class="ektronModal browseButton button buttonAdd buttonLeft greenHover" href="#" id="lnkAddDevice" runat="server"></a>
                                        <div style="margin-top: 3em !important;" id="add_device_nodes">
		                                </div>
                                     </td>
                                  </tr>
                                  <tr id="trDeviceType1" runat="server" visible="false">

                                      <td id="tdDeviceType1" class="label" title="" >:</td>
                                         <td class="value">
                                            <asp:RadioButtonList ID="devicetype" runat="server">
                                                <asp:ListItem id="both" runat="server" Value="0" Text="Both" />
                                                <asp:ListItem id="handheld" runat="server" Value="1" Text="Handheld" />
                                                <asp:ListItem id="tablet" runat="server" Value="2" Text="Tablet" />
                                            </asp:RadioButtonList>
                                         </td>
                                     </tr>
                                 <tr>
                                     <td class="label" title="<%=_MessageHelper.GetMessage("lbl device preview width")%>"><%=_MessageHelper.GetMessage("lbl device preview width")%>:</td>
                                     <td class="value">
                                        <input title="Enter width of the preview template." type="text" maxlength="3" style="width:30px;" id="tbAddPreviewWidth" runat="server"/> px                   
                                    </td>
                                 </tr>
                                 <tr>
                                     <td class="label" title="<%=_MessageHelper.GetMessage("lbl device preview height")%>"><%=_MessageHelper.GetMessage("lbl device preview height")%>:</td>
                                     <td class="value">
                                        <input title="Enter height of the preview template." type="text" maxlength="3" style="width:30px;" id="tbAddPreviewHeight" runat="server"/> px                   
                                    </td>
                                 </tr>
                             </table>
                     </div>
              </div>
           </asp:View>
           
            <asp:View ID="Edit"  runat="server" >
               <div class="ektronPageContainer">
                     <div id="dvDeviceEdit" class="ektronPageInfo">
                        <div id="Div1" class="ektronPageInfo">
                         <table class="ektronForm">
                                 <tr>
                                    <td class="label" title="<%=_MessageHelper.GetMessage("lbl Device")%>" ><%=_MessageHelper.GetMessage("lbl Device")%>:</td>
                                    <td class="value">
                                         <asp:TextBox ToolTip="Enter title of Device Configuration here." Columns="100" id="tbEditDeviceName" runat="server"/>
                                    </td>
                                 </tr>
		                          <tr>
                                     <td class="label" runat="server" id="lbleditdisplayfor" title=""></td>
                                     <td class="value">
                                        <a  title="Click here to browse device models." class="ektronModal browseButton button buttonAdd buttonLeft greenHover" href="#" id="lnkEditDevice" runat="server"></a>
                                        <div style="margin-top: 3em !important;" id="edit_device_nodes">
		                                </div>
                                     </td>
                                  </tr>
                                 <tr id="trDeviceType" runat="server" visible="false">

                                        <td id="tdDeviceType" class="label" title="" >:</td>
                                        <td class="value">
                                            <asp:RadioButtonList ID="edevicetype" runat="server">
                                                <asp:ListItem id="eboth" runat="server" Value="0" Text="Both" />
                                                <asp:ListItem id="ehandheld" runat="server" Value="1" Text="Handheld" />
                                                <asp:ListItem id="etablet" runat="server" Value="2" Text="Tablet" />
                                            </asp:RadioButtonList>
                                        </td>

                                 </tr>
                                 <tr>
                                     <td class="label" title="<%=_MessageHelper.GetMessage("lbl device preview width")%>"><%=_MessageHelper.GetMessage("lbl device preview width")%>:</td>
                                     <td class="value">
                                          <input title="Enter the width for the preview template." type="text" maxlength="3" style="width:30px;" id="tbEditPreviewWidth" runat="server"/> px
                                    </td>
                                 </tr>
                                 <tr>
                                     <td class="label" title="<%=_MessageHelper.GetMessage("lbl device preview height")%>"><%=_MessageHelper.GetMessage("lbl device preview height")%>:</td>
                                     <td class="value">
                                        <input title="Enter the width for the preview template." type="text" maxlength="3" style="width:30px;" id="tbEditPreviewHeight" runat="server"/> px
                                    </td>
                                 </tr>
                             </table>
                     </div>
                     </div>
              </div>
           </asp:View>
           
  </asp:MultiView>
  <input type="hidden" id="savedDeviceModelList" name="savedDeviceModelList" value="" />