<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Menu.aspx.cs" Inherits="Menu" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />

    <script language="javascript" type="text/javascript">
	function IsBrowserSafari() {
		var posn;
		posn = parseInt(navigator.appVersion.indexOf('Safari'));
		return (0 <= posn);
	}
	
	function SelectButton(btn) {
	}

    function ShowTransString(Text) {
		var ObjId = "WorkareaTitlebar";
		var ObjShow = document.getElementById('_' + ObjId);
		var ObjHide = document.getElementById(ObjId);
		if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
			ObjShow.innerHTML = Text;
			ObjShow.style.display = "inline";
			if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
				ObjHide.style.display = "none";
			}
		}

	}
	function HideTransString() {
		var ObjId = "WorkareaTitlebar";
		var ObjShow = document.getElementById(ObjId);
		var ObjHide = document.getElementById('_' + ObjId);

		if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
			ObjShow.style.display = "inline";
			if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
				ObjHide.style.display = "none";
			}
		}
	}

	function GetCellObject(MyObj) {
		var tmpName = "";

		tmpName = MyObj.id;
		if (tmpName.indexOf("link_") >= 0) {
			tmpName = tmpName.replace("link_", "cell_");
		}
		else if (tmpName.indexOf("cell_") >= 0) {
			tmpName = tmpName;
		}
		else {
			tmpName = tmpName.replace("image_", "image_cell_");
		}
		MyObj = document.getElementById(tmpName);
		return (MyObj);
	}

	function RollOver(MyObj) {
		MyObj = GetCellObject(MyObj);
		if (IsBrowserSafari()){
			if (m_prevObj && (m_prevObj != MyObj)) {
				RollOut(m_prevObj);
			}
			MyObj.className = "button-over";
			m_prevObj = MyObj;
		} else {
		    MyObj.className = "button-over";
		}
	}

	function RollOut(MyObj) {		
		MyObj = GetCellObject(MyObj);
		MyObj.className = "button";
	}

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:PlaceHolder ID="DataHolder" runat="server" />
        <div class="ektronPageInfo">
            <asp:Label ID="Message" runat="server" /></div>
    </form>
</body>
</html>
