<%@ Control Language="C#" AutoEventWireup="true" Inherits="viewform" CodeFile="viewform.ascx.cs" %>

<script  type="text/javascript">
<!--
	function LoadContent(FormName,opt){
		document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';
		if(opt=='VIEW'){
			var num=document.forms[0].viewcontent.selectedIndex;
			document.forms[0].action="cmsform.aspx?folder_id="+vFolderId+"&form_id="+jsFormId+"&action="+jsAction+"&LangType="+document.forms[0].viewcontent.options[num].value;
			document.forms[0].submit();
		}else{
			var num=document.forms[0].addcontent.selectedIndex;
			if(document.forms[0].addcontent.options[num].value!=0)
			document.forms[0].action="cmsform.aspx?back_LangType="+jsContentLanguage+"&form_id="+jsFormId+"&LangType="+document.forms[0].addcontent.options[num].value+"&action=Addform&folder_id="+vFolderId+"&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=folder_id&value2="+vFolderId+"&parm3=form_id&value3="+jsFormId+"&parm4=LangType&value4="+jsContentLanguage;
			document.forms[0].submit();
			return false;
		}
	}
	function ShowPane(tabID) {
		var arTab = new Array("dvForm", "dvPostback", "dvFormProperties");
		var dvShow; //tab
		var _dvShow; //pane
		var dvHide;
		var _dvHide;
		for (var i=0; i < arTab.length; i++) {
			if (tabID == arTab[i]) {
			dvShow = eval('document.getElementById("' + arTab[i] + '");');
			_dvShow = eval('document.getElementById("_' + arTab[i] + '");');
		} else {

		dvHide = eval('document.getElementById("' + arTab[i] + '");');
		if (dvHide != null) {
		dvHide.className = "tab_disabled";
		}
		_dvHide = eval('document.getElementById("_' + arTab[i] + '");');
		if (_dvHide != null) {
		_dvHide.style.display = "none";
		}
		}
		}
		_dvShow.style.display = "block";
		dvShow.className = "tab_actived";
	}
	$ektron.addLoadEvent(function(){
	    $ektron("#form_td_vf_content > *").attr("disabled", "disabled");
	    $ektron("#form_td_vf_content input").attr("disabled", "disabled");
	});
// -->
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <div class="tabContainerWrapper">
        <div class="ektronPageTabbed">
            <div class="tabContainer">
                <ul>
                    <li>
                        <a title="Properties" href="#dvProperties">
                            <%=m_refMsg.GetMessage("form properties text")%>
                        </a>
                    </li>
                    <li>
                        <a title="Form" href="#dvForm">
                            <%=m_refMsg.GetMessage("form text")%>
                        </a>
                    </li>
                    <li>
                        <a title="Postback" href="#dvPostback">
                            <%=m_refMsg.GetMessage("postback text")%>
                        </a>
                    </li>
                </ul>

                <div id="dvProperties">
                    <div class="ektronPageGrid">
                        <asp:DataGrid ID="ViewFormPropertiesGrid"
                            runat="server"
                            OnItemDataBound="ViewFormPropertiesGrid_ItemDataBound"
                            EnableViewState="False"
                            GridLines="None"
                            CssClass="ektronForm" />
                    </div>
                </div>
                <div id="dvForm">
                    <div title="Form Content" class="info-header"><%=m_refMsg.GetMessage("lbl form content")%></div>
                    <div id="td_vf_content" runat="server"></div>
                </div>
                <div id="dvPostback">
                    <div title="Post back content (summary)" class="info-header"><%=m_refMsg.GetMessage("lbl post back")%></div>
                    <div id="td_vf_summary" runat="server"></div>
                </div>
            </div>
        </div>
    </div>
    <asp:Literal ID="EmailArea" runat="server" />
    <script  type="text/javascript">
    <!--
	    // This function is in viewform.ascx, viewcontent.ascx
	    function disableFormElements(containingElement)
	    {
		    var oFormElem = null;
		    if ("object" == typeof containingElement && containingElement != null)
		    {
			    oFormElem = containingElement;
		    }
		    else if ("string" == typeof containingElement && containingElement.length > 0)
		    {
			    if (typeof document.getElementById != "undefined")
			    {
				    oFormElem = document.getElementById(containingElement);
			    }
		    }
		    if (!oFormElem) return;
		    if ("undefined" == typeof oFormElem.getElementsByTagName) return;

		    var aryTagNames = ["input", "select", "textarea"];
		    var aryElems;
		    for (var iTagName = 0; iTagName < aryTagNames.length; iTagName++)
		    {
			    aryElems = oFormElem.getElementsByTagName(aryTagNames[iTagName]);
			    for (var i = 0; i < aryElems.length; i++)
			    {
				    aryElems[i].disabled = true;
			    }
		    }
		    aryTagNames = ["label", "legend"];
		    for (var iTagName = 0; iTagName < aryTagNames.length; iTagName++)
		    {
			    aryElems = oFormElem.getElementsByTagName(aryTagNames[iTagName]);
			    for (var i = 0; i < aryElems.length; i++)
			    {
				    aryElems[i].contentEditable = false;
			    }
		    }
	    }

	    setTimeout("disableFormElements('_dvForm')", 100);

	    //defect#17545
        try{
            top.HideDragDropWindow();
        }catch(e){}

    // -->
    </script>
</div>