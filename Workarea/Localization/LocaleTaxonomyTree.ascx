<%@ Control Language="C#" AutoEventWireup="false" CodeFile="LocaleTaxonomyTree.ascx.cs" Inherits="Workarea_LocaleTaxonomyTree" %> 
<script type="text/javascript">
<!--//--><![CDATA[//><!--
    function fetchtaxonomyid(pid)
    {
        for (var i = 0; i < taxonomytreearr.length; i++)
        {
            if (taxonomytreearr[i] == pid)
            {
                return true;
            }
        }
        
        return false;
    }
    
    function fetchdisabletaxonomyid(pid)
    {
        for (var i = 0; i < taxonomytreedisablearr.length; i++)
        {
            if (taxonomytreedisablearr[i] == pid)
            {
                return true;
            }
        }
        
        return false;
    }
    
    function updatetreearr(pid, op)
    {
        if (op == "remove")
        {
            for (var i = 0; i < taxonomytreearr.length; i++)
            {
                if (taxonomytreearr[i] == pid)
                {
                    taxonomytreearr.splice(i, 1); 
                    break;
                }
            }
        }
        else
        {
            taxonomytreearr.splice(0, 0, pid);
        }

        var taxonomyselectedtree = "<%=taxonomyselectedtree.ClientID%>";
        var ids = "";
        for (var i = 0; i < taxonomytreearr.length; i++)
        {
            if (ids.length > 0)
            {
                ids += ",";
            } 
            ids += taxonomytreearr[i];
        }
        
        var selectedIdsElement = document.getElementById(taxonomyselectedtree);
        selectedIdsElement.value = ids;
    }
    
    function selecttaxonomy(control)
    {
        var pid = control.value;
        if (control.checked)
        {
            updatetreearr(pid, "add");
        }
        else
        {
            updatetreearr(pid, "remove");
        }
        
        var currval = eval(document.getElementById("chkTree_T" + pid).value);
        var node = document.getElementById("T" + pid);
        var newvalue = !currval;
        document.getElementById("chkTree_T" + pid).value = eval(newvalue);
        
        var impliedInheritance = $ektron(".ImpliedInheritance").val();
        switch (impliedInheritance)
        {
            case "Ancestor":
                if (control.checked)
                {
                    Traverse(node, true);
                }
                else
                {
                    Traverse(node, false);
                    var hasSibling = false;
                    if (taxonomytreearr != "")
                    {
                        for (var i = 0; i < taxonomytreearr.length; i++)
                        {
                            if (taxonomytreearr[i] != "")
                            {
                                var newnode = document.getElementById("T" + taxonomytreearr[i]);
                                if (newnode != null && newnode.parentNode == node.parentNode)
                                {
                                    Traverse(node, true);
                                    hasSibling = true;
                                    break; 
                                }
                            }
                        }
                    }
                    
                    if (!hasSibling)
                    {
                        checkParent(node);
                    }
                }
                break;
                
            case "Descendants":
                // #55469: timing issue. the subTree are invisible/hidden and therefore this following might not work if subTree are not yet expanded.
                CheckAllSubNodes(node, control.checked);
                break;
                
            default:
                // nothing
                break;
        } 
    }
    
    function CheckAllSubNodes(node, newvalue)
    {
        if (node != null)
        {
            if (node != null && node.id != "")
            {
                for (var j = 0; j < node.childNodes.length; j++)
                {
                    var subn = node.childNodes[j];
                    
                    //check all the Subnodes.
                    if (subn.nodeName == "INPUT" && subn.attributes["type"].value == "checkbox")
                    {
                        if (navigator.userAgent.indexOf("Firefox") > -1 ||
                                navigator.userAgent.indexOf("MSIE 8.0") > -1)
                        {
                            subn.checked = eval(newvalue);
                        }
                        else
                        {
                            subn.setAttribute("checked", eval(newvalue));
                        }
                    }
                    
                    if (HasChildrenContainCheckBoxes(subn))
                    {
                        //check the checkboxes inside the div.
                        //get all the checkboxes inside the Subnode and check it.
                        var containCheckboxes = $ektron("#" + subn.id + " :checkbox");
                        for (k = 0; k < containCheckboxes.length; k++)
                        {
                            //check all the check boxes.
                            var subn = containCheckboxes[k];
                            if (navigator.userAgent.indexOf("Firefox") > -1 ||
                                    navigator.userAgent.indexOf("MSIE 8.0") > -1)
                            {
                                subn.checked = eval(newvalue);
                                subn.disabled = eval(newvalue);
                            }
                            else
                            {
                                subn.setAttribute("checked", eval(newvalue));
                                subn.setAttribute("disabled", eval(newvalue));
                            }
                        }
                    }
                }
            }
        }
    }

    function checkParent(node)
    {
        if (node != null)
        {
            var subnode = node.parentNode;
            if (subnode != null && subnode.id != "T0" && subnode.id != "")
            {
                for (var j = 0; j < subnode.childNodes.length; j++)
                {
                    var pid = subnode.childNodes[j].id;
                    var v = document.getElementById("chkTree_" + pid).value;
                    if (v == true || v == "true")
                    {
                        Traverse(subnode.childNodes[j], true);
                        return;
                    }
                }
                checkParent(subnode.parentNode);
            }
        }
    }
    
    function Traverse(node, newvalue)
    {
        if (node != null)
        {
            subnode = node.parentNode;
            if (subnode != null && subnode.id != "T0" && subnode.id != "")
            {
                for (var j = 0; j < subnode.childNodes.length; j++)
                {
                    var n = subnode.childNodes[j]
                    if (n.nodeName == "INPUT" && n.attributes["type"].value == "checkbox")
                    {
                        var pid = subnode.id;
                        updatetreearr(pid.replace("T", ""), "remove");
                        document.getElementById("chkTree_" + pid).value = eval(newvalue);
                        if (navigator.userAgent.indexOf("Firefox") > -1 ||
                                navigator.userAgent.indexOf("MSIE 8.0") > -1)
                        {
                            n.checked = eval(newvalue);
                            n.disabled = eval(newvalue);
                        }
                        else
                        {
                            n.setAttribute("checked", eval(newvalue));
                            n.setAttribute("disabled", eval(newvalue));
                        }
                    }
                }

                if (HasChildren(subnode) && subnode.getAttribute("checked"))
                {
                    subnode.setAttribute("checked", true);
                    subnode.setAttribute("disabled", true);
                }
                
                Traverse(subnode, newvalue);
            }
        }
    }
    
    function HasChildrenContainCheckBoxes(subnode)
    {
        if (subnode != null)
        {
            for (var i = 0; i < subnode.childNodes.length; i++)
            {
                var n = subnode.childNodes[i];
                var containCheckboxes = $ektron("#" + n.id + " :checkbox");
                if (containCheckboxes.length > 0)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    function HasChildren(subnode)
    {
        if (subnode != null)
        {
            for (var i = 0; i < subnode.childNodes.length; i++)
            {
                var n = subnode.childNodes[i];
                if (n.nodeName == "INPUT" && n.attributes["type"].value == "checkbox")
                {
                    var pid = subnode.id;
                    var v = document.getElementById("chkTree_" + pid).value;
                    if (v == true || v == "true")
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
//--><!]]>
</script>
<div id="divCategories" runat="server">
    <asp:Literal runat="server" ID="EditTaxonomyHtml" />
    <div style="clear: both;">
    </div>
    <br />
    <div id="wamm_float_menu_block_menunode" class="Menu" 
        onmouseout="wamm_float_menu_block_mouseout(this)"
        onmouseover="wamm_float_menu_block_mouseover(this)" 
        style="position: absolute; left: 203px; top: 311px; z-index: 3200; display: none;">
        <input type="hidden" name="LastClickedParent" id="LastClickedParent" value="" />
        <input type="hidden" name="ClickRootCategory" id="ClickRootCategory" value="false" />
        <ul>
            <li class="MenuItem add"><a title="Route Action" href="#" onclick="routeAction(true, 'add');">
                <asp:Literal ID="lit_add_string" runat="server" />
            </a></li>
        </ul>
    </div>
</div>
<input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
<input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
<input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value="" runat="server" /> 
<input type="hidden" name="ShowCheckBox" class="ShowCheckBoxTaxonomy" id="ShowCheckBox" runat="server" />
<input type="hidden" name="hdnImpliedInheritance" class="ImpliedInheritance" id="hdnImpliedInheritance" runat="server" />


