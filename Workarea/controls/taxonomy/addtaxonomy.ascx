<%@ Control Language="C#" AutoEventWireup="true" CodeFile="addtaxonomy.ascx.cs" Inherits="addtaxonomy" %>
<asp:Literal ID="styles" Visible="false" runat="server">
<style type="text/css">
    /*html, body, .ektronPageHeader { width: 50%; }*/
    div.ektronPageContainer { padding: 0; }
    div.tabContainer { margin: .5em 0 .5em 0 !important; padding: .2em 0 .2em 0 !important; }
    div.ui-tabs div.ui-tabs-panel { padding: 1em 0 1em 0; }
</style>
</asp:Literal>
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
        if((taxonomyName.indexOf('>') > -1) || (taxonomyName.indexOf('<') > -1) || (taxonomyName.indexOf('"') > -1) || (taxonomyName.indexOf(';') > -1))
        {
            alert("The taxonomy name can not contain <, >, \" or ; character");
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
    
    function OnInheritTemplateClicked(e){
        var control="<%=taxonomytemplate.ClientID%>";
        if(e.checked)
            document.getElementById(control).disabled=true;
        else
            document.getElementById(control).disabled=false;
        return true;
    }
    
    function RemoveTaxonomyImage(path) {
	    var elem = null;
	    var elemThumb = null;
	    elem = document.getElementById( '<%=taxonomy_image.ClientID%>' );
	    if (elem != null)
	    {
	        elem.value = '';
	    }
	    elemThumb = document.getElementById( '<%=taxonomy_image_thumb.ClientID%>' );
	    if ( elemThumb != null )
	    {
	        elemThumb.src = path;
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
    function ToggleTaxSynch(obj, createLangObj)
    {
        if (obj.checked)
        {
            $ektron('.taxsynccreate').hide();
            $ektron("#" + createLangObj).removeAttr("checked");
            $ektron("#" + createLangObj).removeAttr("disabled");
        }
        else
        {
            $ektron('.taxsynccreate').show();
            $ektron("#" + createLangObj).attr("disabled", false);
        }
    }
    <%=closeWindow %>
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
                    <li>
                        <a title="Custom Properties" href="#dvMetadata">
                            <%=m_refMsg.GetMessage("custom properties")%>
                        </a>
                    </li>
                </ul>
                <div id="dvProperties">
                    <table class="ektronForm">
                        <tr>
                            <td class="label" title="Breadcrumb"><%=m_refMsg.GetMessage("lbl sitemap path")%>:</td>
                            <td title="Breadcrumb - <%=m_strCurrentBreadcrumb%>"><%=m_strCurrentBreadcrumb%></td>
                        </tr>
                        <tr>
                            <td class="label" title="Category Title"><%=m_refMsg.GetMessage("categorytitle")%>:</td>
                            <td><asp:TextBox ToolTip="Category Title Text" ID="taxonomytitle" runat="server" />&nbsp;<asp:Label ToolTip="Language" ID="lblLanguage" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Category Description"><%=m_refMsg.GetMessage("categorydescription")%>:</td>
                            <td><asp:TextBox ToolTip="Category Description Text" ID="taxonomydescription" Rows="5" TextMode="MultiLine" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="<%=m_refMsg.GetMessage("taxonomyimage")%>"><%=m_refMsg.GetMessage("taxonomyimage")%>:</td>
                            <td>
                                <ul class="ui-helper-clearfix buttonWrapper" style="width:auto">
                                    <li><span id="sitepath">
                                        <asp:Literal ID="ltr_sitepath" runat="Server" /></span>
                                        <asp:TextBox ToolTip="Taxonomy Image Text" ID="taxonomy_image" runat="server" ReadOnly="true" />
                                        &#160; </li>
                                    <li><a class="button buttonRemove redHover buttonRight" title="<%=m_refMsg.GetMessage("tooltip taxonomy remove imgbtn")%>"
                                        href="#" onclick="RemoveTaxonomyImage('images/application/spacer.gif');return false">
                                        <%=m_refMsg.GetMessage("lbl taxonomy remove imgbtn")%></a> </li>
                                    <li><a class="button buttonChange greenHover buttonRight" title="<%=m_refMsg.GetMessage("tooltip taxonomy change imgbtn")%>"
                                        href="#" onclick="PopUpWindow('mediamanager.aspx?scope=images&upload=true&retfield=<%=taxonomy_image.ClientID%>&showthumb=false&autonav=0', 'Meadiamanager', 790, 580, 1,1);return false;">
                                        <%=m_refMsg.GetMessage("lbl taxonomy change imgbtn")%></a> &nbsp;</li>
                                </ul>
                                <div style="margin-top:2em;"></div>
                                <asp:Image ToolTip="Taxonomy Image" ID="taxonomy_image_thumb" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Template"><%=m_refMsg.GetMessage("template label")%>:</td>
                            <td><asp:DropDownList ToolTip="Select Template" ID="taxonomytemplate" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Inherit"><%=m_refMsg.GetMessage("inherit label")%>:</td>
                            <td>
                                <asp:CheckBox ToolTip="Inherit Parnet Template" ID="inherittemplate" Text="(check here to inherit from the parent template)" runat="server" />
                                <asp:Label ID="lblInherited" runat="server" />
                            </td>
                        </tr>
                        <tr>
                           <td class="label" title="Category Link"><%=m_refMsg.GetMessage("lbl category link")%>:</td>
                            <td><asp:TextBox ToolTip="Category Link Text" ID="categoryLink" runat="server" /></td>
                        </tr>
                        <tr>
                           <td class="label" title="Synchronized"><%=m_refMsg.GetMessage("lbl tax synch")%>:</td>
                            <td>
                                <asp:CheckBox ToolTip="Enable/Disable Language Synchronization" ID="chkTaxSynch" runat="server" />
                                <%=m_refMsg.GetMessage("lbl tax synch msg")%>
                                <fieldset class="taxsynccreate" runat="server" id="fs_taxsynccreate">
                                    <asp:CheckBox ToolTip="Create the taxonomy in all languages" ID="chkCreateLang" runat="server" />
                                    <%=m_refMsg.GetMessage("lbl tax lang synch create msg")%>
                                </fieldset>
                            </td>
                        </tr>
                        <tr id="tr_enableDisable" runat="server" >
                           <td class="label" title="Enable"><%=m_refMsg.GetMessage("lbl enable")%>:</td>
                            <td>
                                <asp:CheckBox ToolTip="Enable/Disable Message" ID="chkEnableDisable" onclick="confirmSubmit(this)" Checked="true" runat="server" />
                                <div class="ektronCaption"><%=m_refMsg.GetMessage("lbl enable/disable msg")%></div>
                            </td>            
                        </tr>
                        <tr id="tr_config" runat="server">
                            <td class="label" title="Configuration"><%=m_refMsg.GetMessage("config page html title")%>:</td>
                            <td>
                                <asp:CheckBox ToolTip="Configure Content" ID="chkConfigContent" runat="server" Text="Content"/>
                                <br />
                                <asp:CheckBox ToolTip="Configure User" ID="chkConfigUser" runat="server" Text="User"/>
                                <br />
                                <asp:CheckBox ToolTip="Configure Group" ID="chkConfigGroup" runat="server"  Text="Group"/>
                            </td>
                        </tr>
                    </table>
                </div>    
                <div id="dvMetadata">
                    <div style="display:inline">
                        <asp:DropDownList ToolTip="Select a Custom Property" runat="server" ID="availableCustomProp" AppendDataBoundItems="true">
                        </asp:DropDownList>
                        <a title="Add Custom Property" class="buttonAddTag" style="display:inline-block; vertical-align:middle;" onclick="AddCustomProperty();"></a>
                    </div>
                    <div class="ektronTopSpace"></div>
                    <table id="customPropertyTable" class="ektronGrid" runat="server">
                        <tr class="title-header">
                            <td width="40%" ><%=m_refMsg.GetMessage("lbl taxonomy custom prop title")%></td>
                            <td width="20%" ><%=m_refMsg.GetMessage("lbl taxonomy custom prop datatype")%></td>
                            <td width="30" ><%=m_refMsg.GetMessage("lbl taxonomy custom prop value")%></td>
                            <td width="10%" ><%=m_refMsg.GetMessage("lbl taxonomy custom prop action")%></td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>    
    <input type="hidden" id="alllanguages" runat="server"  />
    <input type="hidden" runat="server" id="txtValue" name="txtValue" />
    <input type='hidden' id='hdnSelectedIDS' name='hdnSelectedIDS' runat="server" />
    <input type='hidden' id='hdnSelectValue' name='hdnSelect' runat="server" />
    
</div>
