<%@ Page Language="C#" AutoEventWireup="true" CodeFile="segments.aspx.cs" Inherits="Analytics_reporting_segments" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        #seg_dropbox_content {
        background-color:#FFFFFF;
        border:1px solid #AAAAAA;
        overflow:auto;
        padding:1em 0 0;
        }
        #seg_dropbox_data .seg_dropbox_listbox {
        float:left;
        margin:0 0 0 1em;
        width:47%;
        }
        #seg_dropbox_data .seg_dropbox_listbox ul {
        background-color:#FFFFFF;
        border:1px solid #999999;
        height:12em;
        list-style:none outside none;
        margin:0.4em 0;
        overflow:auto;
        padding:0;
        }
        #seg_dropbox_data ul li {
        margin:0;
        padding:0;
        }
        .selectedSegment
        {
            background-color:#FFCC66;
        }
    </style>
    <script type="text/javascript">
        function onSegmentCheckBoxClick(chkSegment) 
        {
            var checked = $ektron("input[type=checkbox]:checked").length;
            if (chkSegment.checked && checked > 4) 
            {
                chkSegment.checked = false;
                return false;
            }
            if (chkSegment.checked) 
            {
                $ektron('#' + chkSegment.id).closest("li").addClass('selectedSegment');
            }
            else
            {
                $ektron('#' + chkSegment.id).closest("li").removeClass('selectedSegment');
            }
        }	   

        function AddCustomSegment(segmentType, displayText)
        {
            var jUl = $ektron("#lstCustomSegment");
            var jLi = $ektron("input[id^='segment_id_prop_']", jUl);
            var idx = jLi.length;
            while ($ektron("input#segment_id_prop_" + idx + "_" + segmentType, jUl).length > 0)
            {
                idx++;
            }
            var sChecked = "";
            var checked = $ektron("input[type=checkbox]:checked").length;
            if (checked < 4) 
            {
                sChecked = " checked=\"checked\"";
            }
            var html = "<li><label for=\"segment_id_prop_" + idx + "_" + segmentType + "\">";
            html += "<input type=\"checkbox\" onclick=\"onSegmentCheckBoxClick(this);\"" + sChecked + " value=\"prop_" + idx + "_" + segmentType + "\" name=\"chk_prop_" + idx + "_" + segmentType +  "\" id=\"segment_id_prop_" + idx + "_" + segmentType +  "\" />";
            html += "&#160;" + displayText + "&#160;</label>";
            html += "<input type=\"text\" class=\"prop_" + idx + "_" + segmentType + " ektronTextXSmall\" id=\"txt_prop_" +  idx + "_" + segmentType + "\" name=\"prop_" + idx + "_" + segmentType + "\" value=\"\" /></li>";
            jUl.append(html);
            if (sChecked.length > 0)
            {
                $ektron("#segment_id_prop_" + idx + "_" + segmentType).closest("li").addClass("selectedSegment");
            }
            
            $ektron("div.Menu").hide();
        }
        function UpdateSegments(segmentId, widgetId) {
            if (typeof(window.opener.UpdateSegments) != 'undefined') {
                window.opener.UpdateSegments(segmentId, widgetId);
            }
            self.close();
        }
    </script>
</head>
<body onclick="MenuUtil.hide();">
    <form id="form2" runat="server">
        <div class="ektronPageContainer ektronPageInfo" style="z-index:1000;">
            <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
                <div class="AnalyticsErrorMessage"><asp:Literal ID="litErrorMessage" runat="server" /></div>
            </asp:Panel>
            <p><asp:Label ID="SelectStatement" runat="server" Text="Select up to four segments by which to filter your report"></asp:Label></p>
            <div id="seg_dropbox_content">
                <div id="seg_dropbox_data">
                    
                    <asp:Repeater id="rpt_Default" runat="server">
                        <HeaderTemplate>
                            <div class="seg_dropbox_listbox">
                            <h1>Default</h1>
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li <%# (bool)Eval("Selected") ? " class=\"selectedSegment\" " : "" %>>
                                <label for="segment_id_<%# Eval("Id")%>">
                                    <input type="checkbox" onclick="onSegmentCheckBoxClick(this);"
                                        <%# (bool)Eval("Selected") ? " checked " : "" %> value="<%# Eval("Id")%>" name="chk_<%# Eval("Id")%>" id="segment_id_<%# Eval("Id")%>">
                                    <%# Eval("Name")%>
                                </label>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Repeater id="rpt_Custom" runat="server">
                        <HeaderTemplate>
                            <div class="seg_dropbox_listbox">
                            <h1>Custom</h1>
                            <ul id="lstCustomSegment">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li <%# (bool)Eval("Selected") ? " class=\"selectedSegment\" " : "" %>>
                                <label for="segment_id_<%# Eval("Id")%>">
                                    <input type="checkbox" onclick="onSegmentCheckBoxClick(this);"
                                        <%# (bool)Eval("Selected") ? " checked " : "" %> value="<%# Eval("Id")%>" name="chk_<%# Eval("Id")%>" id="segment_id_<%# Eval("Id")%>">
                                    <%# Eval("Name")%>
                                </label>
                                <input type="text" class="<%# Eval("Id")%> ektronTextXSmall" id="txt_<%# Eval("Id")%>" name="<%# Eval("Id")%>" value="<%# Eval("CmsSegmentValue")%>" 
                                    <%# Eval("CmsSegmentValue") == "" ? " style=\"display:none;\" " : "" %> />
                           </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
