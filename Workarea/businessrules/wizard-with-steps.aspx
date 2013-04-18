<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wizard-with-steps.aspx.cs" Inherits="wizard_with_steps" ValidateRequest="false"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Reference Control="../controls/wizard/wizard.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title><asp:Literal id="ltr_Title" runat="server"/></title>
    <script type="text/javascript" language="javascript">
        var g_relativeClassPath = '../csslib/';
        g_relativeClassPath = g_relativeClassPath.toLowerCase();
        UpdateWorkareaTitleToolbars();
        var g_OldBtnObject = null;

        function GetRelativeClassPath()
        {
            return(g_relativeClassPath);
        }

        function IsBrowserIE()
        {
	        // document.all is an IE only property
	        return (document.all ? true : false);
        }

        function UpdateWorkareaTitleToolbars()
        {
            if (document.styleSheets.length > 0) {
                MakeClassPathRelative('*', 'button', 'backgroundImage', '../images/application/', g_relativeClassPath)
                MakeClassPathRelative('*', 'button-over', 'backgroundImage', '../images/application/', g_relativeClassPath)
                MakeClassPathRelative('*', 'button-selected', 'backgroundImage', '../images/application/', g_relativeClassPath)
                MakeClassPathRelative('*', 'button-selectedOver', 'backgroundImage', '../images/application/', g_relativeClassPath)
                MakeClassPathRelative('*', 'ektronToolbar', 'backgroundImage', '../images/application/', g_relativeClassPath)
                MakeClassPathRelative('*', 'ektronTitlebar', 'backgroundImage', '../images/application/', g_relativeClassPath)
            } else {
                setTimeout('UpdateWorkareaTitleToolbars()', 500);
            }
        }

        function ShowTransString(Text)
        {
            var ObjId = "ektronTitlebar";
            var ObjShow = document.getElementById('_' + ObjId);
            var ObjHide = document.getElementById(ObjId);
            if ((typeof ObjShow != "undefined") && (ObjShow != null))
            {
                ObjShow.innerHTML = Text;
                ObjShow.style.display = "inline";
                if ((typeof ObjHide != "undefined") && (ObjHide != null))
                {
                    ObjHide.style.display = "none";
                }
            }
        }

        function HideTransString()
        {
            var ObjId = "ektronTitlebar";
            var ObjShow = document.getElementById(ObjId);
            var ObjHide = document.getElementById('_' + ObjId);
            if ((typeof ObjShow != "undefined") && (ObjShow != null))
            {
                ObjShow.style.display = "inline";
                if ((typeof ObjHide != "undefined") && (ObjHide != null))
                {
                    ObjHide.style.display = "none";
                }
            }
        }

        function GetCellObject(MyObj)
        {
            var tmpName = "";
            tmpName = MyObj.id;
            if (tmpName.indexOf("link_") >= 0)
            {
                tmpName = tmpName.replace("link_", "cell_");
            }
            else if (tmpName.indexOf("cell_") >= 0)
            {
                tmpName = tmpName;
            }
            else
            {
                tmpName = tmpName.replace("image_", "image_cell_");
            }
            MyObj = document.getElementById(tmpName);
            return (MyObj);
        }



        function ClearPrevBtn()
        {
            if (g_OldBtnObject)
            {
              RollOut(g_OldBtnObject);
              g_OldBtnObject = null;
            }
        }

        function RollOver(MyObj)
        {
            ClearPrevBtn()
            g_OldBtnObject = MyObj;
            var tmpClassExt = "";
            MyObj = GetCellObject(MyObj);tmpClassExt = MyObj.className.                substring(MyObj.className.lastIndexOf("-"));
            if (tmpClassExt == "-selected")
            {
                MyObj.className = "button-selectedOver";
            }
            else
            {
                MyObj.className = "button-over";
            }
        }

        function RollOut(MyObj)
        {
            if (g_OldBtnObject == MyObj)
            {
                g_OldBtnObject = null;
            }
            var tmpClassExt = "";
            MyObj = GetCellObject(MyObj);
            tmpClassExt = MyObj.className.substring(MyObj.className.lastIndexOf("-"));
            if (tmpClassExt == "-selectedOver")
            {
                MyObj.className = "button-selected";
            }
            else
            {
                MyObj.className = "button";
            }
        }

        function SelectButton(MyObj)
        {
        }

        function UnSelectButtons()
        {
            var iLoop = 100;
            while (document.getElementById("image_cell_" + iLoop.toString()) != null)
            {
                document.getElementById("image_cell_" + iLoop.toString()).className = "button";
                iLoop++;
            }
        }

        function noenter(eObj)
        {
            var iKey = eObj.keyCode;
            iKey = iKey + 1;
            iKey = iKey - 1;
            if (eObj != null && (iKey == 13))
            {
                RuleWizardManager.submitInputFormCheck();
                return false;
            }
        }

        Ektron.ready(function()
        {
            var parentToolbar = $ektron(".ektronPageHeader > .ektronTitlebar #stepTitle");
            var panelWizard = $ektron("#pnlwizard");
            if (panelWizard.html() != "")
            {
            	if($ektron("#WorkareaTitlebar").text() != "View Rule"){
                	$ektron(".wizardHeader, span.stepLabel").hide();
                }
            }

            $ektron(".ektronPageHeader > .ektronToolbar .stepFuture, .ektronPageHeader > .ektronToolbar .stepNext").hover(function(){

                var buttonCaption = $(this).attr('tt');
                parentToolbar.html(buttonCaption);
               
            },function(){
                 
            });
        });

    </script>
</head>
<body>
<form id="ruleForm" runat="server" style="background-color: white;">
    <asp:Panel ID="pnlwizard" runat="server"></asp:Panel>
    <asp:literal runat="server" ID="ltrjs"></asp:literal>
    <div id="dhtmltooltip"></div>
    <div id="dvHoldMessage" style="border-right: 1px; border-top: 1px; display: none;
        border-left: 1px; width: 100%; border-bottom: 1px; position: absolute; top: 48px;
        height: 1px; background-color: white; text-align:center">
        <table width="100%" style="background-color:White; border-style:solid">
            <tr>
                <td valign="top" style="white-space:nowrap" align="center">
                    <h3 style="color: red">
                        <strong>
                            <%=m_refMsg.GetMessage("one moment msg")%>
                        </strong>
                    </h3>
                </td>
            </tr>
        </table>
    </div>
    <div class="ektronPageHeader wizardHeader">
        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
        <div class="ektronToolbar" id="divToolBar" runat="server"></div>
    </div>
    <div class="ektronPageContainer">
        <table cellpadding="0">
            <tr>
                <td>
                    <span id="dataBox" onclick="event.cancelBubble=true;">
                        <input name="dataBoxText" type="text" id="dataBoxText" onkeypress="javascript:return noenter(event);" />
                        <input title="<% =m_refMsg.GetMessage("btn ok")%>" type="button" value="ok" onclick="RuleWizardManager.submitInputFormCheck();" />
                        <input title="<% =m_refMsg.GetMessage("btn cancel")%>" type="button" value="cancel" onclick="RuleWizardManager.hideInputForm();" />
                        <input name="dataBoxId" type="hidden" value="" id="dataBoxId" />
                        <input name="dataBoxName" type="hidden" value="" id="dataBoxName" />
                        <input name="dataBoxType" type="hidden" value="action" id="dataBoxType" />
                    </span>
                    <span id="wizard" class="wizard">
                        <span class="wizardStep" id="wizardStep1">
                            <span class="stepLabel" title="Step 1 of 4. Select your conditions"><%=m_refMsg.GetMessage("lbl steps conditions")%></span>
                            <p class="matchLabel" title="Match">
                                    <%=m_refMsg.GetMessage("lbl match")%>
                                    <select title="Select the conditions to match" id="logicalOperator" name="logicalOperator">
                                        <option title="Abbreviation for all the words" value="and"><%=m_refMsg.GetMessage("lbl abbreviation for all the words")%></option>
                                        <option title="Abbreviation for any of the words" value="or"><%=m_refMsg.GetMessage("lbl abbreviation for any of the words")%></option>
                                    </select>
                                    <%=m_refMsg.GetMessage("lbl of the following conditions.")%>
                            </p>
                            <p title="Conditions" id="conditionContainer" class="conditionContainer"></p>
                        </span>

                        <span class="wizardStep" id="wizardStep2"><asp:literal id="step2" runat="server" />
                            <p title="Step 2" id="actiontrueContainer" class="actionContainer"></p>
                        </span>

                        <span class="wizardStep" id="wizardStep3"><asp:literal id="step3" runat="server" />
                            <p title="Step 3" id="actionfalseContainer" class="actionContainer"></p>
                        </span>

                        <span class="wizardStep" id="wizardStep4"><asp:literal id="step4" runat="server" />
                            <p title="Name" id="ruleNameContainer" class="ruleNameContainer">
                                <%=m_refMsg.GetMessage("name label")%>
                                <input title="Enter Name here" type="text" name="rulename" value="" id="ruleNameText" runat="server"/>
                            </p>
                        </span>
                    </span>
                    <asp:Literal id="templateJS" runat="server"></asp:Literal>
                    <asp:Literal id="ltr_wizardjs" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
        <asp:Literal ID="ltrhidden" runat="server"></asp:Literal>
    </div>
</form>
</body>
</html>
