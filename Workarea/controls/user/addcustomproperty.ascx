<%@ Control Language="C#" AutoEventWireup="true" Inherits="addcustomproperty" CodeFile="addcustomproperty.ascx.cs" %>
<asp:Literal ID="ltrJS" runat="server"></asp:Literal>

<script type="text/javascript" language="javascript">
    /***********************************************
    * Contractible Headers script- © Dynamic Drive (www.dynamicdrive.com)
    * This notice must stay intact for legal use. Last updated Oct 21st, 2003.
    * Visit http://www.dynamicdrive.com/ for full source code
    ***********************************************/

    var enablepersist = "off" //Enable saving state of content structure using session cookies? (on/off)
    var collapseprevious = "no" //Collapse previously open content when opening present? (yes/no)

    function getElementbyClass(classname)
    {
        ccollect = new Array()
        var inc = 0
        var alltags = document.all ? document.all : document.getElementsByTagName("*")
        for (i = 0; i < alltags.length; i++)
        {
            if (alltags[i].className == classname)
                ccollect[inc++] = alltags[i]
        }
    }

    function contractcontent(omit)
    {
        var inc = 0
        while (ccollect[inc])
        {
            if (ccollect[inc].id != omit)
                ccollect[inc].style.display = "none"
            inc++
        }
    }


    function expandcontent(cid)
    {
        if (typeof ccollect != "undefined")
        {
            if (collapseprevious == "yes")
                contractcontent(cid)
            document.getElementById(cid).style.display = (document.getElementById(cid).style.display != "block") ? "block" : "none"
        }
    }
    function revivecontent()
    {
        contractcontent("omitnothing")
        selectedItem = getselectedItem()
        selectedComponents = selectedItem.split("|")
        for (i = 0; i < selectedComponents.length - 1; i++)
            document.getElementById(selectedComponents[i]).style.display = "block"
    }
    function get_cookie(Name)
    {
        var search = Name + "="
        var returnvalue = "";
        if (document.cookie.length > 0)
        {
            offset = document.cookie.indexOf(search)
            if (offset != -1)
            {
                offset += search.length
                end = document.cookie.indexOf(";", offset);
                if (end == -1) end = document.cookie.length;
                returnvalue = unescape(document.cookie.substring(offset, end))
            }
        }
        return returnvalue;
    }

    function getselectedItem()
    {
        if (get_cookie(window.location.pathname) != "")
        {
            selectedItem = get_cookie(window.location.pathname)
            return selectedItem
        }
        else
            return ""
    }

    function saveswitchstate()
    {
        var inc = 0, selectedItem = ""
        while (ccollect[inc])
        {
            if (ccollect[inc].style.display == "block")
                selectedItem += ccollect[inc].id + "|"
            inc++
        }

        document.cookie = window.location.pathname + "=" + selectedItem
    }
    function do_onload()
    {
        getElementbyClass("switchcontent")
        if (enablepersist == "on" && typeof ccollect != "undefined")
            revivecontent()
    }


    if (window.addEventListener)
        window.addEventListener("load", do_onload, false)
    else if (window.attachEvent)
        window.attachEvent("onload", do_onload)
    else if (document.getElementById)
        window.onload = do_onload

    if (enablepersist == "on" && document.getElementById)
        window.onunload = saveswitchstate
    function trim(s)
    {
        while (s.substring(0, 1) == ' ')
        {
            s = s.substring(1, s.length);
        }
        while (s.substring(s.length - 1, s.length) == ' ')
        {
            s = s.substring(0, s.length - 1);
        }
        return s;
    }
    function expand_it(cid)
    {
        if (typeof ccollect != "undefined")
        {
            document.getElementById(cid).className = "";
        }
    }

    function collapse_it(cid)
    {
        if (typeof ccollect != "undefined")
        {
            document.getElementById(cid).className = "switchcontent";
        }
    }
    function show_range2(Min, Max, Obj)
    {
        var myindex
        var bMin
        var bMax
        var arMin = Min.split(',');
        var arMax = Max.split(',');
        bMin = false;
        bMax = false;
        if (typeof Obj == "undefined") return;
        if (Obj.selectedIndex == -1) return;
        myindex = Obj[Obj.selectedIndex].value;
        for (var i = 0; i < arMin.length; i++)
        {
            //alert(myindex + ' == ' + trim(arMin[i]));
            if (myindex == trim(arMin[i]))
            {
                bMin = true;
                break;
            }
        }
        for (var i = 0; i < arMax.length; i++)
        {
            if (myindex == trim(arMax[i]))
            {
                bMax = true;
                break;
            }
        }
        if (bMin || bMax)
        {
            if (document.getElementById("addCustomProp_TR_Min") != null)
            {
                expand_it('addCustomProp_TR_Min');
            }
            if (document.getElementById("addCustomProp_TR_Max") != null)
            {
                expand_it('addCustomProp_TR_Max');
            } 
            if (document.getElementById("editCustomProp_TR_Min") != null)
            {
                expand_it('editCustomProp_TR_Min');
            }
            if (document.getElementById("editCustomProp_TR_Max") != null)
            {
                expand_it('editCustomProp_TR_Max');
            }
            
            if (myindex == 4)
            {
                expand_it('sc2');
                expand_it('sc4');
            }
            else
            {
                collapse_it('sc2');
                collapse_it('sc4');
            }
        }
        else
        {
            if (document.getElementById("addCustomProp_TR_Min") != null)
            {
                collapse_it('addCustomProp_TR_Min');
            }
            if (document.getElementById("addCustomProp_TR_Max") != null)
            {
                collapse_it('addCustomProp_TR_Max');
            }
             if (document.getElementById("editCustomProp_TR_Min") != null)
            {
                collapse_it('editCustomProp_TR_Min');
            }
            if (document.getElementById("editCustomProp_TR_Max") != null)
            {
                collapse_it('editCustomProp_TR_Max');
            }             
            collapse_it('sc2');
        }
    }
</script>

<style type="text/css">
    .switchcontent
    {
        display: none;
    }
</style>
<div id="dhtmltooltip">
</div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server">
    </div>
    <div class="ektronToolbar" id="htmToolBar" runat="server">
    </div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
        <tr>
            <td class="label" title="Label">
                <%=m_refMsg.GetMessage("lbl Label")%>
            </td>
            <td class="value">
                <asp:TextBox ID="txtLabel" EnableViewState="True" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label" title="Type">
                <asp:Label ID="lblType" ToolTip="Type" runat="server"><%=m_refMsg.GetMessage("type label")%></asp:Label>
            </td>
            <td class="value">
                <asp:DropDownList ID="ddTypes" ToolTip="Select Type from Drop Down Menu" EnableViewState="True" runat="server" AutoPostBack="True" />
            </td>
        </tr>
        <tr id="TR_inputType" runat="server">
            <td class="label">
                <asp:Label ID="lblInputType" ToolTip="Input Type" runat="server"><%=m_refMsg.GetMessage("lbl Input Type")%></asp:Label>
            </td>
            <td class="value">
                <asp:DropDownList ID="ddInputType" ToolTip="Select Input Type from Drop Down Menu" EnableViewState="True" runat="server" AutoPostBack="True" />
            </td>
        </tr>
        <tr id="TR_Validation" runat="server">
            <td class="label">
                <asp:Label ID="lblValidation" ToolTip="Validation" runat="server"><%=m_refMsg.GetMessage("lbl Validation")%></asp:Label>
            </td>
            <td class="value">
                <asp:DropDownList ID="ddValidationType" ToolTip="Select Validation Type from Drop Down Menu" EnableViewState="True" runat="server" />
            </td>
        </tr>
        <tr id="TR_Min" class="switchcontent" runat="server">
            <td class="label">
                <asp:Label ID="lblMinVal" ToolTip="Minimum Value" runat="server">Min Value:</asp:Label>
            </td>
            <td class="value">
                <asp:TextBox ID="txtMinValue" EnableViewState="True" Rows="20" runat="server" />
                <span class="switchcontent" id="sc2">
                    <asp:Literal ID="dtStart" runat="server" />
                </span>
            </td>
        </tr>
        <tr id="TR_Max" class="switchcontent" runat="server">
            <td class="label">
                <asp:Label ID="lblMaxVal" ToolTip="Maximum Value" runat="server">Max Value:</asp:Label>
            </td>
            <td class="value">
                <asp:TextBox ID="txtMaxValue" EnableViewState="True" Rows="20" runat="server" />
                <span class="switchcontent" id="sc4">
                    <asp:Literal ID="dtEnd" runat="server" />
                </span>
            </td>
        </tr>
        <tr id="TR_Message" runat="server">
            <td class="label">
                <asp:Label ID="lblMessage" ToolTip="Message" runat="server"><%=m_refMsg.GetMessage("lbl desc")%>:</asp:Label>
            </td>
            <td class="value">
                <asp:TextBox ID="txtMessage" EnableViewState="True" runat="server" />
            </td>
        </tr>
    </table>  
          <% if(DisplaySelect){%> 
        <div class="ektronTopSpace"></div>        
        <%--selectlist code goes here--%>
        <div>
            <script language="javascript" type="text/javascript" src='<%=m_CommAPI.ApplicationPath%>java/selectlist.js'></script>
            <span>
                <%= m_refMsg.GetMessage("lbl add items you want to display in select list")%></span>
            <div class="ektronTopSpaceSmall">
            </div>
            <table>
                <tr>
                    <td style="width: 50%">
                        <select onchange="javascript:editSelectList('availableItems','ItemText');" onclick="javascript:editSelectList('availableItems','ItemText');"
                            name="availableItems" id="availableItems" multiple="true" size="5" style="width: 100%">
                        </select>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td style="margin-left: 4px; margin-right: 4px;">
                        <a href="javascript: void moveItemUp('availableItems')">
                            <img src='<%=m_CommAPI.ApplicationPath%>images/application/uparrow.gif' />
                        </a>
                        <br />
                        <a href="javascript: void moveItemDown('availableItems')">
                            <img src='<%=m_CommAPI.ApplicationPath%>images/application/downarrow.gif' />
                        </a>
                    </td>
                    <td>
                        &nbsp;&nbsp;
                    </td>
                    <td style="width: 50%">
                        <table class="ektronForm">
                            <tr>
                                <td class="label">
                                    <%=m_refMsg.GetMessage("lbl Text")%>
                                </td>
                                <td class="value">
                                    <input type="text" id="ItemText" name="ItemText" value="" size="25" />
                                </td>
                            </tr>
                        </table>
                        <div class="ektronTopSpace">
                        </div>
                        <div style="padding-left: 50px">
                            <input type="button" id="btnAdd" name="btnAdd" value='<%=m_refMsg.GetMessage("btn Add")%>'
                                onclick="javascript:addItemToSelectList('availableItems','ItemText','ItemText');" />
                            &nbsp;&nbsp;
                            <input type="button" id="btnChange" name="btnChange" value='<%=m_refMsg.GetMessage("btn Change")%>'
                                onclick="javascript:updateItemToSelectList('availableItems','ItemText','ItemText');" />
                            &nbsp;&nbsp;
                            <input type="button" id="btnRemove" name="btnRemove" value='<%=m_refMsg.GetMessage("btn remove")%>'
                                onclick="javascript:removeItemsFromSelectList('availableItems');" />
                        </div>
                    </td>
                </tr>
            </table>
            <div class="ektronTopSpaceSmall">
            </div>
            <input type="checkbox" id="chkValidation" name="chkValidation" />
            <span>
                <%=m_refMsg.GetMessage("lbl first item is not a valid selection for example, [Select]")%></span>
            <input type="hidden" name="selectedvalues" id="selectedvalues" value="" />
        </div>

        <script type="text/javascript" language="javascript">
            function InitializeSelect(){
                populateSelectedList("availableItems","<%=m_strSelectedValue%>");
                document.getElementById("selectedvalues").value ="<%=m_strSelectedValue%>";
                var intValidationType = '<%= m_intValidationType %>';                
                if(intValidationType > 0){
                    document.getElementById("chkValidation").checked=true;
                }
            }
            setTimeout('InitializeSelect()',50);
        </script>  
         <% } %>            
</div>
<%if(ddValidationType.Enabled){ %>
<script type="text/javascript" language="javascript">
   setTimeout('myBodyLoad()', 50);
</script>
<% }%>

