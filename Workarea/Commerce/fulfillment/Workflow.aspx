<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Workflow.aspx.cs" Inherits="FulfillmentWorkflow" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <title>Workflow configuration</title>
    <script type="text/javascript">
    function setImageUrl(url){
        document.getElementById ("wfImgIframe").src=url+document.forms[0].ddWf.options[document.forms[0].ddWf.selectedIndex].text;
    }
    Ektron.ready(function() {
            $ektron('#wfImgModal').modal({
                trigger: '',
                modal: true,
		        toTop: true,
                onShow: function(hash) {
                    var pageWidth = $ektron("body").width();
                    hash.w.css("width", (pageWidth - 40));
                    hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).show();
                    hash.w.css("margin-left", -1 * Math.round(hash.w.outerWidth()/2)).show();
                    hash.o.fadeTo("fast", 0.5, function() {
				        hash.w.fadeIn("fast");
			        });
                },
                onHide: function(hash) {
                    hash.w.fadeOut("fast");
			        hash.o.fadeOut("fast", function(){
				        if (hash.o)
				        {
					        hash.o.remove();
			            }
			        });
                }
            });
        });
    </script>
    <style type="text/css">
        div.ektronWindow {font-size: 12px;}
        #wfImgModal .ektronModalBody {padding: 0;}
        #wfImgIframe {width: 100%; height: 35em; border: 0;}
    </style>
    <!--[if IE 6]>
    <style type="text/css">
        #wfImgIframe {width: 97%;}
    </style>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <!--Workflow image modal-->
        <div class="ektronWindow ektronModalStandard" id="wfImgModal">
            <div class="ektronModalHeader">
                <h3>
                    <span class="headerText"><asp:Literal ID="workflowTitle" runat="server" /></span>
                    <a href="#closeDialog" class="ektronModalClose">Close</a>
                </h3>
            </div>
            <div class="ektronModalBody">
                <iframe id="wfImgIframe" src=""></iframe>
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronGrid">
                <tr>
                    <td class="label"><asp:Literal ID="ltr_workflow" runat="server"></asp:Literal>:</td>
                    <td class="value" style="white-space:nowrap;">
                        <asp:DropDownList ID="ddWf" runat="server" />
                        &nbsp;
                        <asp:Literal ID="lnkWorkflow" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

