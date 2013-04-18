<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="Workarea_Notifications_Settings" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Notification Settings</title>
    <asp:literal id="StyleSheetJS" runat="server" />
    
    <style type="text/css">
        .ektronForm table.EktronReplyDetails{border:1px solid silver;border-collapse:collapse;margin-top:10px;}
        .ektronForm table.EktronReplyDetails th{border:1px solid silver;font-weight:bold;text-align:left;}
        .ektronForm table.EktronReplyDetails th span.note{font-weight:normal;}
        .ektronForm table.EktronReplyDetails td{border:1px solid silver;text-align:left;}
        .ektronForm table.EktronReplyDetails td div#ektronTestEmailResults{float:left;margin:5px 0 0 10px;}
        .ektronForm table.EktronReplyDetails input.TestEmailSettingsButton{width:120px;text-align:center;margin-top:5px;float:left;}
        .ektronForm table.EktronReplyDetails input.TestEmailSettingsButton:hover{cursor:pointer;}
        .ektronForm table.EktronReplyDetails tr.extraSettings td,
        .ektronForm table.EktronReplyDetails tr.extraSettings th {border-top-width:2px;}
    </style>

    <script type="text/javascript">
        function ChangeSite() 
        {
            document.forms['form1'].submit();
        }
        
        function SubmitForm(FormName){
        
            var msg='';
            if (document.getElementById('chkEnableEmailReply').checked) {
                var port = $.trim(document.getElementById('txtEmailReplyServerPort').value);
                
                if ($.trim(document.getElementById('txtEmailReplyServer').value) == '')
                    msg += 'Email server is required.\n';
                if ($.trim(document.getElementById('txtEmailReplyAccount').value) == '')
                    msg += 'Email account is required.\n';
                if ($.trim(port)=='' ||  !IsNumeric(port)) 
                    msg += 'Valid port required.\n';
                if ($.trim(document.getElementById('txtEmailReplyPassword').value) == '')
                    msg += 'Password is required.\n';
                if (!IsConnectionTested()) {
                    msg += 'You must test the connection first.\n'
                }
            }
            if (msg != '') {
                alert(msg);
            }
            else {
                document.forms[FormName].submit();
            }
            return false;
         }

	     function IsNumeric(str) {
	         var chars = "0123456789";
	         var c;
	         for (i = 0; i < str.length; i++) {
	             c = str.charAt(i);
	             if (chars.indexOf(c) == -1) {
	                 return false;
	                 break;
	             }
	         }
	         return true;
	     }

       
        if (typeof(Ektron.Workarea.NotificationSettings) == "undefined")
        {
            Ektron.Workarea.NotificationSettings =
            {
                init: function () {
                    
                    $ektron(".TestEmailSettingsButton").button();

                    $ektron("input#chkEnableEmailReply").bind("click", function () {
                        $("table.EktronReplyDetails").toggle(this.checked);
                    });

                    $ektron(".TestEmailSettingsButton").bind("click", function () {
                        $("div#ektronTestEmailResults").html("Verifying account...");
                        $("div#ektronTestEmailResults").load("TestEmailConnection.ashx", {
                            'account': $ektron('input.txtEmailReplyAccount').val(),
                            'password': $ektron('input.txtEmailReplyPassword').val(),
                            'server': $ektron('input.txtEmailReplyServer').val(),
                            'serverPort': $ektron('input.txtEmailReplyServerPort').val(),
                            'useSsl': $ektron('input#chkUseSsl')[0].checked
                        });
                        return false;
                    });


                    // attach edit mode click function passing site id
                    var editbtn = $("a.editButton");
                    $(editbtn).click(function () {
                        // get the site id
                        var siteid = $("#siteSearchItem").val();

                        // change the the link on the fly and navigate
                        $(editbtn).attr("href", "Settings.aspx?mode=edit&siteId=" + siteid);
                        $(editbtn).click();
                    });

                    // display email settings if selected
                    if ($ektron("input#chkEnableEmailReply:checked").val() != undefined) {
                        $("table.EktronReplyDetails").show();
                    }

                }
            }
        }

        function IsConnectionTested() 
        {
            var resulttext = $.trim($("div#ektronTestEmailResults").html());
            var res = (resulttext != undefined && resulttext != '' && resulttext.indexOf('Failed') == -1 );
            return res;
        }

        Ektron.ready(Ektron.Workarea.NotificationSettings.init);


    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronForm">
                <tr>
                    <td class="label">
                        <asp:Label ID="lblPublishNotification" runat="server" /></td>
                    <td class="value">
                        <asp:CheckBox ID="chkEnablePublishNotifications" runat="server" Enabled="false"  />
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblEnableEmailReply" runat="server" />
                    </td>
                    <td class="value">
                        <asp:CheckBox ID="chkEnableEmailReply" class="chkEnableEmailReply" runat="server" Enabled="false"  />

                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="Label1" runat="server" Text="Email Settings"  />
                    </td>
                    <td>
                        <table id="ucEktronReplyDetails" runat="server" class="EktronReplyDetails" style="display:none;">
                            <thead>
                                <tr>
                                    <th colspan="2" style="border:1px solid silver;font-weight:bold;">
                                        <asp:Label ID="lblEmailReplyHeader" runat="server" Text="Email Reply Details - " />
                                        <span class="note"><asp:Label ID="lblDetailsNote" runat="server" Text="This must be a POP Server and must be the same account used to send the notification emails." /></span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr class="EktronEmailReplySetting">
                                    <th>
                                        <asp:Label ID="lblEmailReplyAccount" runat="server" Text="Email Account Name" />
                                    </th>
                                    <td>
                                        <asp:Label ID="lblEmailReplyAccountValue" runat="server" Text="" />
                                        <asp:TextBox ID="txtEmailReplyAccount" class="txtEmailReplyAccount" runat="server" Visible="false" />
                                    </td>
                                </tr>
                                <tr class="EktronEmailReplySetting">
                                    <th>
                                        <asp:Label ID="lblEmailReplyPassword" runat="server" Text="Email Account Password" />
                                    </th>
                                    <td>
                                        <asp:Label ID="lblEmailReplyPasswordValue" runat="server" Text="*******" />
                                        <asp:TextBox ID="txtEmailReplyPassword" class="txtEmailReplyPassword" runat="server" TextMode="Password" Visible="false" />
                                    </td>
                                </tr>
                                <tr class="EktronEmailReplySetting">
                                    <th>
                                        <asp:Label ID="lblEmailReplyServer" runat="server" Text="Email Server" />
                                    </th>
                                    <td>
                                        <asp:Label ID="lblEmailReplyServerValue" runat="server" Text="" />
                                        <asp:TextBox ID="txtEmailReplyServer" class="txtEmailReplyServer" runat="server" Visible="false" />
                                    </td>
                                </tr>
                                <tr class="EktronEmailReplySetting">
                                    <th>
                                        <asp:Label ID="lblEmailReplyServerPort" runat="server" Text="Email Server Port" />
                                    </th>
                                    <td>
                                        <asp:Label ID="lblEmailReplyServerPortValue" runat="server" Text="25" />
                                        <asp:TextBox ID="txtEmailReplyServerPort" class="txtEmailReplyServerPort" runat="server" Visible="false" />
                                    </td>
                                </tr>
                                    <tr class="EktronEmailReplySetting">
                                    <th>
                                        <asp:Label ID="lblUseSsl" runat="server" Text="Use SSL" />
                                    </th>
                                    <td>
                                        <asp:CheckBox ID="chkUseSsl" class="chkUseSsl" runat="server" Enabled="false" />
                                    </td>
                                </tr>
                                <tr class="EktronEmailReplySetting" id="ucTestRow" runat="server" visible="false">
                                    <td colspan="2"> 
                                        <input ID="btnTestEmailSettings" class="TestEmailSettingsButton" value="Test Connection" runat="server" />
                                        <div id="ektronTestEmailResults"></div>
                                    </td>
                                </tr>
                                  <tr class="EktronEmailReplySetting extraSettings">
                                    <th>
                                        <asp:Label ID="Label2" runat="server" Text="Prepend Reply Message" />
                                    </th>
                                    <td>
                                        <asp:CheckBox ID="chkPrependReplyMessage" class="chkUseSsl" runat="server" Enabled="false" Text="If checked the email reply message will be prepended to the Notification message." />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                 </tr>
       <%--     <tr>
                    <td colspan="2" class="EktronNestedTable">
                        <table>
                            <tr>
                                <td class="label">
                                    <asp:Label ID="Label1" runat="server" />
                                </td>
                                <td class="value">
                                    <asp:Label ID="Label2" runat="server" Text="Off" />
                                    <asp:CheckBox ID="CheckBox1" runat="server" Visible="false" />
                                </td>
                            </tr>
                            <tr class="EktronEmailReplySetting">
                                <td class="label">
                                    <asp:Label ID="Label3" runat="server" Text="Email Account Name" />
                                </td>
                                <td class="value">
                                    <asp:Label ID="Label4" runat="server" Text="derek.barka@ektron.com" />
                                    <asp:TextBox ID="TextBox1" runat="server" Visible="false" />
                                </td>
                            </tr>
                            <tr class="EktronEmailReplySetting">
                                <td class="label">
                                    <asp:Label ID="Label5" runat="server" Text="Email Account Name" />
                                </td>
                                <td class="value">
                                    <asp:Label ID="Label6" runat="server" Text="derek.barka@ektron.com" />
                                    <asp:TextBox ID="TextBox2" runat="server" Visible="false" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>--%>
            </table>
        </div>
    </form>
</body>
</html>