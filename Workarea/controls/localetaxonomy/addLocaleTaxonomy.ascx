<%@ Control Language="C#" AutoEventWireup="true" CodeFile="addLocaleTaxonomy.ascx.cs" Inherits="addLocaleTaxonomy" %>

<script type="text/javascript">
    var controlid="taxonomy_";
    function Validate(){
        var taxonomyName = document.getElementById(controlid+"taxonomytitle").value;
        if(document.getElementById(controlid+"taxonomytitle").value==''){
            alert('<%=m_refMsg.GetMessage("js:alert taxonomy required field")%>');
            return false;
        }
        if(document.getElementById(controlid+'chkConfigContent') != null && document.getElementById(controlid+'chkConfigUser') != null && document.getElementById(controlid+'chkConfigGroup')!= null)
        {
            if(document.getElementById(controlid+'chkConfigContent').checked == false && document.getElementById(controlid+'chkConfigUser').checked == false && document.getElementById(controlid+'chkConfigGroup').checked == false)
             {
                   alert('<%=m_refMsg.GetMessage("js:alert configuration selection required")%>');
                    return false;
             }
         }
        if((taxonomyName.indexOf('>') > -1) || (taxonomyName.indexOf('<') > -1) || (taxonomyName.indexOf('"') > -1))
        {
            alert("The taxonomy name can not contain <, >, \"");
        }
        else if(taxonomyName.toLowerCase() == 'assets' || taxonomyName.toLowerCase() == 'privateassets')
        {
            alert('<%=m_refMsg.GetMessage("js:alert tax name cannot be assets")%>');
        }
        else
        {
            document.forms[0].submit();
        }

    }
    
    
	
	function confirmSubmit(chkBox)
	{
	   var outcome;
	   if (chkBox.checked)
	   {
	       outcome = confirm('<%=m_refMsg.GetMessage("js:Confirm enable taxonomy all languages")%>');
	       if(outcome)
	          document.getElementById('<%=alllanguages.ClientID%>').value = "";      
	   }
	   else
	   {
	     outcome = confirm('<%=m_refMsg.GetMessage("js:Confirm disable taxonomy all languages")%>');
	     if(outcome)
	          document.getElementById('<%=alllanguages.ClientID%>').value = "false";
	          
	   }
	}
	
    function updateText(obj)
    {
        $ektron("#taxonomy_txtValue")[0].value = obj.value;
    }
    
    function ToggleSelection(obj)
    {
        $ektron("#taxonomy_hdnSelectedIDS")[0].value = obj.checked;
        if(!obj.checked)
        {
            $ektron($ektron(obj)[0].parentNode.parentNode).find("td")[3].childNodes.item().disabled = true;
        }
        else
        {
            $ektron($ektron(obj)[0].parentNode.parentNode).find("td")[3].childNodes.item().disabled = false;
        }
        return false;
    }
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div id="searchpanel" class="ektronPageContainer ektronPageInfo">
        <div class="tabContainerWrapper">
            <div class="tabContainer">
                <ul>
                    <li>
                        <a title="Properties" href="#dvProperties">
                            <%=m_refMsg.GetMessage("properties text")%>
                        </a>
                    </li>
                    <%--<li>
                        <a title="Metadata" href="#dvMetadata">
                            <%=m_refMsg.GetMessage("custom properties")%>
                        </a>
                    </li>--%>
                </ul>
                <div id="dvProperties">
                    <table class="ektronForm">
                        <tr>
                            <td class="label" title="Category Title"><%=m_refMsg.GetMessage("localecategorytitle")%>:</td>
                            <td><asp:TextBox ToolTip="Category Title" ID="taxonomytitle" runat="server" />&nbsp;<asp:Label ID="lblLanguage" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Category Description"><%=m_refMsg.GetMessage("localecategorydescription")%>:</td>
                            <td><asp:TextBox ToolTip="Category Description" ID="taxonomydescription" Rows="5" TextMode="MultiLine" runat="server" /></td>
                        </tr>
<%--                        <tr>
                            <td class="label" title="Taxonomy Image"><%=m_refMsg.GetMessage("taxonomyimage")%>:</td>
                            <td>
                                <span id="sitepath"><asp:Literal ID="ltr_sitepath" runat="Server" /></span>
                                <asp:TextBox ToolTip="Taxonomy Image" id="taxonomy_image" runat="server" ReadOnly="true" />
                                &#160;
                                <a href="#" title="Change" onclick="PopUpWindow('mediamanager.aspx?scope=images&upload=true&retfield=<%=taxonomy_image.ClientID%>&showthumb=false&autonav=0', 'Meadiamanager', 790, 580, 1,1);return false;">Change</a>
                                &nbsp;
                                <a href="#" title="Remove" onclick="RemoveTaxonomyImage('images/application/spacer.gif');return false">Remove</a>
                                <div class="ektronTopSpaceSmall"></div>
                                <asp:Image ID="taxonomy_image_thumb" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Template"><%=m_refMsg.GetMessage("template label")%>:</td>
                            <td><asp:DropDownList ToolTip="Select Template" ID="taxonomytemplate" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Inherit"><%=m_refMsg.GetMessage("inherit label")%>:</td>
                            <td>
                                <asp:CheckBox ToolTip="(check here to inherit from the parent template)" ID="inherittemplate" Text="(check here to inherit from the parent template)" runat="server" />
                                <asp:Label ID="lblInherited" runat="server" />
                            </td>
                        </tr>
                        <tr>
                           <td class="label" title="Category Link"><%=m_refMsg.GetMessage("lbl category link")%>:</td>
                            <td><asp:TextBox ToolTip="Category Link" ID="categoryLink" runat="server" /></td>
                        </tr>
                        <tr id="tr_enableDisable" runat="server" >
                           <td class="label" title="Enable"><%=m_refMsg.GetMessage("lbl enable")%>:</td>
                            <td>
                                <asp:CheckBox ToolTip="Enable/Disable" ID="chkEnableDisable" onclick="confirmSubmit(this)" Checked="true" runat="server" />
                                <div class="ektronCaption" title="Enable/Disable"><%=m_refMsg.GetMessage("lbl enable/disable msg")%></div>
                            </td>            
                        </tr>
                        <tr id="tr_config" runat="server">
                            <td class="label" title="Configuration"><%=m_refMsg.GetMessage("config page html title")%>:</td>
                            <td>
                                <asp:CheckBox ToolTip="Content" ID="chkConfigContent" runat="server" Text="Content"/>
                                <br />
                                <asp:CheckBox ToolTip="User" ID="chkConfigUser" runat="server" Text="User"/>
                                <br />
                                <asp:CheckBox ToolTip="Groups" ID="chkConfigGroup" runat="server"  Text="Group"/>
                            </td>
                        </tr>--%>
                    </table>
                </div>    
             <%--   <div id="dvMetadata">
                    <div style="display:inline">
                        <asp:DropDownList ToolTip="Select a Custom Property" runat="server" ID="availableCustomProp" AppendDataBoundItems="true">
                            <asp:ListItem Value="-1">
                            Select a Custom Property
                            </asp:ListItem>
                        </asp:DropDownList>
                        <a class="greenHover buttonAddTag" style="display:inline; vertical-align:middle;" onclick="AddCustomProperty();"></a>
                    </div>
                    <div class="ektronTopSpace"></div>
                    <table id="customPropertyTable" class="ektronGrid" runat="server">
                        <tr class="title-header">
                            <td title="Title" width="40%">Title</td>
                            <td title="Data Type" width="20%">Data Type</td>
                            <td title="Value" width="30">Value</td>
                            <td title="Action" width="10%">Action</td>
                        </tr>
                    </table>
                </div>--%>
            </div>
        </div>    
    <input type="hidden" id="alllanguages" runat="server"  />
    <input type="hidden" runat="server" id="txtValue" name="txtValue" />
    <input type='hidden' id='hdnSelectedIDS' name='hdnSelectedIDS' runat="server" />
    <input type='hidden' id='hdnSelectValue' name='hdnSelect' runat="server" />
    
</div>