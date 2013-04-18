<%@ Control language="c#" CodeFile="ucFieldTreeView.ascx.cs" Inherits="Ektron.ContentDesigner.Dialogs.ucFieldTreeView" AutoEventWireup="false" %>
<%@ Register TagPrefix="radT" Namespace="Telerik.WebControls" Assembly="RadTreeView.NET2" %>
<%@ Register TagPrefix="rada" Namespace="Telerik.WebControls" Assembly="RadAjax.NET2" %>

<clientscript id="EkFieldTreeViewScript" runat="server">
function EkFieldTreeViewControl(clientID)
{
	var m_objXPathExpr = null;
	var m_strSelection = "";
	var m_strFieldPath = "";
	var m_objTextSel = null;

	this.clientID = clientID;
	this.load = function(sContentTree)
	{
        var ajaxManager = window[clientID + "_RadAjaxManager1"];
        ajaxManager.AjaxRequest(sContentTree);
	};
	
	this.setXPathExpression = function(objXPathExpr)
	{
		m_objXPathExpr = objXPathExpr;
		var oElem = document.getElementById(m_objXPathExpr.txtFieldId);
		if (oElem)
		{
			$ektron(oElem).bind("change", m_clearTextSel).bind("click", m_clearTextSel).bind("select", m_clearTextSel);
			oElem = null;
		}
	};
	
	this.updateTextField = function()
	{
		m_objXPathExpr.updateXPath(m_objTextSel, m_strFieldPath);
		m_clearTextSel();
	};
	
	this.treeNodeDoubleClick = function(node)
	{
		this.treeNodeClick(node);
		this.updateTextField();
		return false;
	};

	this.treeNodeClick = function(node)
	{
		if(node != null)
            m_strFieldPath = node.Value;
		m_objTextSel = m_objXPathExpr.expandFieldNameSelection();
		m_updateXPathLink(m_objTextSel.text);
	};
	
	this.setupTreeNodes = function()
	{
	    var treeview = window[clientID + "_RadTree1"];
	    if (!treeview) return;
	    var startTabIdx = $ektron(this.clientID + "_UpdateLink").attr("tabindex");
        $ektron("span.TreeNode").each(function(i, e){
            if ($ektron(this).attr("tabindex") > 0) return false; //exit the each loop
            $ektron(this).attr("tabindex", i + startTabIdx);
        });
        
        $ektron("div#" + this.clientID + "_RadTree1 span").keydown(function(e)
        {
            if (treeview != null)
            {
                if (32 == e.keyCode) //space bar
                { 
                    treeview.UnSelectAllNodes();
                    var element = e.srcElement ? e.srcElement: e.target;
                    var $curFocusNode = $ektron(element);
                    //var selectedVal = $curFocusNode.text();
                    var node = treeview.FindNodeByText($curFocusNode.text());
                    if  (node != null)
                    {
                        if (node.Selected != true)
                        {
                            node.Select();
                        } 
                        else
                        {
                            node.UnSelect();
                        }
                    }
                }
            }
        });
        //set focus to first node
        var $firstNode = $ektron("span.TreeNode:first");
        if (treeview != null && 0 == treeview.SelectedNodesCount())
        {
            var node = treeview.FindNodeByText($firstNode.text());
            if  (node != null)
            {
                if (node.Selected != true)
                {
                    node.Select();
                } 
            }
        }
        return treeview;
	};
	
	function m_clearTextSel()
	{
		if (m_objTextSel)
		{
			m_objTextSel = null;
			m_updateXPathLink();
		}
	}
	
	function m_updateXPathLink(selectedText)
	{
		var oLink = document.getElementById(clientID + "_UpdateLink");
		if (m_objTextSel)
		{
			if (selectedText)
			{
				oLink.innerHTML = Ektron.String.format(EkFieldTreeViewResourceText.sReplace0in1, selectedText, m_objXPathExpr.txtFieldName);
			}
			else
			{
				oLink.innerHTML = EkFieldTreeViewResourceText.sInsertField;
			}
			oLink.style.visibility = "visible";
		}
		else
		{
			var tree = window[clientID + "_RadTree1"];
			tree.UnSelectAllNodes();
			oLink.style.visibility = "hidden";
		}
	}
}
var ekFieldTreeViewControl = new Array();
</clientscript>
<clientscript id="EkFieldTreeViewArray" runat="server">
ekFieldTreeViewControl["&lt;%=this.ClientID%&gt;"] = new EkFieldTreeViewControl("&lt;%=this.ClientID%&gt;");
</clientscript>

<fieldset id="fsTreeControl" style="padding-bottom: 0; padding-right: 0;" runat="server">
    <legend title="To complete the Condition, select a field to insert" id="lblSelectField" runat="server">Select a field to insert:</legend>
    <asp:HyperLink ToolTip="Insert field" ID="UpdateLink" style="visibility: hidden" runat="server">Insert field</asp:HyperLink>
    <radT:RadTreeView id="RadTree1" runat="server" 
		AllowNodeEditing="False" SingleExpandPath="True"
		MultipleSelect="False" CheckBoxes="False"
		Style="overflow: auto; height: 150px; margin-top: 2px;" />
</fieldset>
<rada:RadAjaxManager id="RadAjaxManager1" runat="server" OnAjaxRequest="AjaxManager1_AjaxRequest">
    <ajaxsettings>
		<rada:AjaxSetting AjaxControlID="RadAjaxManager1">
			<UpdatedControls>
				<rada:AjaxUpdatedControl ControlID="RadTree1" />
			</UpdatedControls>
		</rada:AjaxSetting>
	</ajaxsettings>
</rada:RadAjaxManager>
<script type="text/javascript">
<!--
    //poll dom to figure out when rad treeview has finished
    var RadTreeReady = setInterval(function()
    {
        var treeview = null;
        try
        {
            // could be a tree that is in a different tab, not visible yet.
            treeview = <%=RadTree1.ClientID %>;
        }
        catch (ex) {}
        if ($ektron("span.TreeNode").length > 0 && treeview && 0 == treeview.SelectedNodesCount())
        {
            ekFieldTreeViewControl["<%=this.ClientID %>"].setupTreeNodes();
            clearInterval(RadTreeReady);
        }
    }, 100);
//-->
</script>