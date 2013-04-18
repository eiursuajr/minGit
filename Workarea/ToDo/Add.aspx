<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Add.aspx.cs" Inherits="Workarea_ToDo_Add" %>
<%@ Register TagPrefix="ektron" TagName="ContentDesigner" Src="../controls/Editor/ContentDesignerWithValidator.ascx" %>
<%@ Register TagPrefix="ektron" TagName="DateRangePicker" Src="../controls/generic/date/DateRangePicker.ascx" %>
<%@ Register TagPrefix="Community" TagName="UserSelectControl" Src="../controls/Community/Components/UserSelectControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Task</title>
    <asp:literal id="StyleSheetJS" runat="server" />
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        function GetCommunityMsgObject(id){
	        var fullId = "CommunityMsgObj_" + id;
	        if (("undefined" != typeof window[fullId])
		        && (null != window[fullId])){
		        return (window[fullId]);
	        }
	        else {
	            var obj = new CommunityMsgClass(id);
	            var fullId = "CommunityMsgObj_" + id;
	            window[fullId] = obj;
		        return (obj);
	        }
        }

        function CommunityMsgClass (idTxt) {
	        this.getId = function(){
		        return (this.id);
	        },

	        this.SetUserSelectId = function (userSelId){
	            this.userSelId = userSelId;
	        },

	        this.MsgShowMessageTargetUI = function(hdnId){
	            $ektron('#MessageTargetUI' + this.getId()).modalShow();
	        },

	        this.MsgSaveMessageTargetUI = function(){
		        var hdbObj = document.getElementById('ektouserid' + this.getId());
		        var toObj = document.getElementById('ekpmsgto' + this.getId());
		        var idx;
		        if (hdbObj){
			        hdbObj.value = '';
			        var users = UserSelectCtl_GetSelectUsers(this.userSelId);
			        if (("undefined" != users) && (null != users)){
			            hdbObj.value = users;
			            if (toObj){
			                toObj.value='';
			                var userArray = users.split(',');
			                for (idx = 0; idx < userArray.length; idx++){
				                if(toObj && toObj.value.length > 0){
					                toObj.value += ', ';
				                }
				                var userName = UserSelectCtl_GetUserName(this.userSelId, userArray[idx]);
				                if (userName)
    				                toObj.value += userName;
			                }
		                }
		            }
		        }
		        this.MsgCloseMessageTargetUI();
	        },


	        this.MsgCloseMessageTargetUI = function(){
	            $ektron('#MessageTargetUI' + this.getId()).modalHide();
	        },

	        this.MsgCancelMessageTargetUI = function(){
		        this.MsgCloseMessageTargetUI();
	        },

	        ///////////////////////////
	        // initialize properties:
	        this.id = idTxt;
	        this.name = '';
	        this.MsgUsersSelArray = new Object();
	        this.userSelId = '';
        }
	    //--><!]]>
    </script>
    <script type="text/javascript">
		<!--//--><![CDATA[//><!--
        Ektron.ready( function()
            {
            
            //Tag, Upload, Browse Modal
            $ektron("#newTagNameDiv, #avatar_upload_panel, #MessageTargetUI__Page").modal({
                trigger: '',
                modal: true,
                toTop: true,
                onShow: function(hash){
                            hash.w.css("margin-top", 0); //hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
                    hash.o.fadeIn();
                    hash.w.fadeIn();
                    setTimeout(CommunityGroupAddEdit__SetScrollable, 500);
                    $(window).resize(CommunityGroupAddEdit__SetScrollable);
                },
                onHide: function(hash){
                    hash.w.fadeOut("fast");
	                        hash.o.fadeOut("fast", function()
	                            {
                        if (hash.o)
                        {
                            hash.o.remove();
                        }
                    });
                }
            });
        });

    function CommunityGroupAddEdit__SetScrollable(){
        var container = $ektron(".EktMsgTargets").eq(0);

        container.height($ektron(window).height() -
            (container.offset().top
                + (container.innerHeight()
                    - container.height())));

        $ektron(".analyticsReport .ektronToolbar").eq(0).width(container.outerWidth());
        container.eq(0).css("overflow-y", "auto");
        container.eq(0).css("overflow-x", "auto");
        container.find("* *").eq(0).css("overflow", "visible");
    }

        //--><!]]>
    </script>
    <script type="text/javascript">

      function SubmitForm(FormName,Validate) {
       if (Validate.length > 0) {
			if (eval(Validate)) {
				document.forms[FormName].submit();
				return false;
			}
			else {
				return false;
			}
		}
		else {
			document.forms[FormName].submit();
			return false;
		}
	}
	function VerifyAddItem() {
		var es = '' ;
		if(document.forms.form1.txtTitle.value=='') {
			es+= '<asp:Literal id="ltr_titleErr" runat="server" />\n' ;
		}
		if (document.forms.form1.ekpmsgto__Page.value==''){
			es += '<asp:Literal id="ltr_subErr" runat="server" />\n' ;
		}
		if(es!='') {
			alert('<asp:Literal id="ltr_follErr" runat="server" />'  + es) ; return false;
		}
		else {
			return true ;
		}
	}
	</script>
    <style type="text/css">
        <!--/*--><![CDATA[/*><!--*/
			#T0{ float:none; position:relative; }
			ul.ektree{ float:none; position:relative; background-color: #ffffff; border: 1px solid #000000; margin: 10px 10px 10px 10px; padding: 10px 10px 10px 10px; }
            #TreeOutput{ position:relative; width:100%; }
			#d_dvCategory table{ width:100%; }
			div#newTagNameDiv { height: 175px !important; width:375px !important; margin: 17em 0 0 -15em !important; border: solid 1px #aaaaaa; z-index: 10; background-color: white; left: 50%; position: fixed; margin-left: -20em;}
			div#avatar_upload_panel { height: 125px; width:400px;top:7%;left:35%; margin: 10em 0 0 -15em; border: solid 1px #aaaaaa; z-index: 10; background-color: white; }
			div#MessageTargetUI__Page { margin: 10em 0 0 1.0em;top:3px;left:2px; border: solid 1px #aaaaaa; z-index: 10; background-color: white; }
		    /* Styling for the Browse Members/Friends elements */
		    .EktMsgTargetCtl{ font-family: Verdana,Geneva,Arial,Helvetica,sans-serif; font-size: 12px; position: relative; top: 2px; left: 0px; background-color: white; z-index:12; }
		    .EktMsgTargets{ position: relative; top: 20px; left: 16px; border: solid 1px #dddddd; padding: 10px; }
		    .EktMsgTargets div.CommunitySearch_ResultsContainer { width: 440px; }
		    #newTagName { width: 275px !important; }
		    #newTagNameScrollingDiv { height: 80px; overflow: auto; border: z-index: 1; }
            a.btnUpload { padding-top: .2em !important; padding-bottom: .2em !important;line-height: 16pt !important; display:inline-block; text-decoration: none !important; }
            .nameWidth { color:#1D5987; font-weight: bold; text-align: right; white-space: nowrap; width:10%; }
            a.buttonBrowseUSer {background-image: url(images/ui/icons/User.png); background-position: .6em center;}
            #FrameContainer { position: relative; top: 0px; left: 0px; width: 1px; height: 1px; display: none; z-index: 1000; }
            body div.ektronWindow {position: relative !important; top: 0; left: 0; margin-left: 0; margin-top: 0;}
            div#MessageTargetUI__Page.dv_MessageTargetUI {width: auto; margin-top: -42px !important; margin-left: -21px !important;}
            .dv_MessageTargetUI .ektronModalBody {padding: 2px;}
            .CommunitySearch_BasicTextboxContainer input {width: 200px;}
		/*]]>*/-->
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="MessageTargetUI__Page" class="dv_MessageTargetUI ektronWindow ektronModalStandard">
            <div class="ektronModalBody">
            <div class="EkTB_dialog">
                <div class="EktMsgTargetCtl">
                    <div>
                        <div id="browseCommunityUsers" class="EktMsgTargets">
                            <Community:UserSelectControl id="Invite_UsrSel" FriendsOnly="false" runat="server" />
                            <asp:button id="cgae_userselect_done_btn" runat="server" />
                            <asp:button id="cgae_userselect_cancel_btn" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
      </div>
      <div id="dhtmltooltip"></div>
		<div class="ektronPageHeader">
		    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
		    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
		</div>
        <div class="ektronPageContainer">
            <table>
                <tr>
                    <td class="label">
                        <asp:Label ToolTip="Title" ID="lblTitle" runat="server" Text="Title" />
                    </td>
                    <td class="value">
                        <asp:TextBox ToolTip="Enter Title here" ID="txtTitle" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ToolTip="Assigned To" ID="lblAssign" runat="server" Text="Assigned To" />
                    </td>
                    <td class="value">
                        <input title="Enter what the item is Assigned To here" type="text" id="ekpmsgto__Page" name="ekpmsgto__Page" disabled="disabled"
                            runat="server" class="ektronTextMedium" />
                        <input type="hidden" id="ektouserid__Page" name="ektouserid__Page" value="" runat="server" />
                        <asp:Literal ID="BrowseUsers" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ToolTip="Priority" ID="lblPriority" runat="server" Text="Priority" />
                    </td>
                    <td>
                        <asp:DropDownList ToolTip="Select Priority from Drop Down Menu" ID="ddlPriority" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ToolTip="Status" ID="lblStatus" runat="server" Text="Status" />
                    </td>
                    <td>
                        <asp:DropDownList ToolTip="Select Status from Drop Down Menu" ID="ddlStatus" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label">
                    <asp:Label ToolTip="Duration" ID="lblDuration" runat="server" Text="Duration" />  
                    </td>
                    <td class="value">
                        <ektron:DateRangePicker ID="DateRangePicker1" runat="server" />
                    </td>
                </tr>
                <tr>
                <td class="label">
                <asp:Label ToolTip="Description" ID="lblDesc" runat="server" Text="Description" />
                </td>
                <td class="value">
                <input type="hidden" name="ephox" id="ephox" value="false" />
				<ektron:ContentDesigner ID="txtTextAddEdit" AllowScripts="false" Height="100%" Width="98%" runat="server" />
                <br />
                <asp:Literal ID="viewContentHTML" runat="server" />
                </td>
                </tr>
            </table>
        </div>
        <asp:Literal ID="litInitialize" runat="server"  />
    </form>
</body>
</html>
