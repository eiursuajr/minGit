<script language="javascript" src="java/selectlist.js"></script>

<span><%=m_refmsg.getmessage("lbl add items you want to display in select list") %></span>

<div class="ektronTopSpaceSmall"></div>
<table>
    <tr>
        <td style="width:50%">
            <select onchange="javascript:editSelectList('availableItems','ItemText');" onclick="javascript:editSelectList('availableItems','ItemText');" name="availableItems"
                id="availableItems" multiple="true"  size="5" style="width: 100%">
            </select>
        </td>
        <td>&nbsp;</td>
        <td style="margin-left:4px;margin-right:4px;">
            <a href="javascript: void moveItemUp('availableItems')">
                <img src="images/application/uparrow.gif" />
            </a>
            <br />
            <a href="javascript: void moveItemDown('availableItems')">
                <img src="images/application/downarrow.gif" />
            </a>
        </td>
        <td>&nbsp;&nbsp;</td>
        <td style="width:50%">
            <table class="ektronForm">
                <tr>
                    <td class="label"><%=m_refmsg.getmessage("lbl Text")%></td>
                    <td class="value"><input type="text" id="ItemText" name="ItemText" value="" size="25"/></td>
                </tr>
            </table>
            <div class="ektronTopSpace"></div>
            <div style="padding-left:50px">
                <input type="button" id="btnAdd" name="btnAdd" value='<%=m_refmsg.getmessage("btn Add")%>' onclick="javascript:addItemToSelectList('availableItems','ItemText','ItemText');" />
                &nbsp;&nbsp;
                <input type="button" id="btnChange" name="btnChange" value='<%=m_refmsg.getmessage("btn Change")%>' onclick="javascript:updateItemToSelectList('availableItems','ItemText','ItemText');" />
                &nbsp;&nbsp;
                <input type="button" id="btnRemove" name="btnRemove" value='<%=m_refmsg.getmessage("btn remove")%>' onclick="javascript:removeItemsFromSelectList('availableItems');" />            
            </div>
        </td>
    </tr>
</table>

<div class="ektronTopSpaceSmall"></div>
<input type="checkbox" id="chkValidation" name="chkValidation" />
<span><%=m_refmsg.getmessage("lbl first item is not a valid selection for example, [Select]")%></span>

<input type="hidden" name="selectedvalues" id="selectedvalues" value="" />
