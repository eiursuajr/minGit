<%@ Control Language="C#" AutoEventWireup="true" Inherits="cmswizard" CodeFile="wizard.ascx.cs" %>

<div class="ektronPageHeader">
    <div class="ektronTitlebar">
        <span id="stepTitle"><%=m_msgRef.GetMessage("lbl response")%></span>
    </div>
    <div class="ektronToolbar">
        <table width="100%" cellspacing="0">
		    <tr>
			    <td class="ektronToolbar">
				    <table id="stepsTable">
					    <tr>
						    <td id="stepsCaption" valign="middle">
						        <%=m_msgRef.GetMessage("lbl step")%>
						        <span id="currentStep">1</span>
						        <%=m_msgRef.GetMessage("lbl of")%>
						        <span id="totalSteps">4</span>
						    </td>
						    <td>
							    <table id="stepsGraphicsTable">
								    <tr>
									    <td id="step1" class="stepCurrent">1</td>
						                <!-- stepVisited stepNext-->
									    <td id="step2" class="stepNext">2</td>
									    <td id="step3" class="stepFuture">3</td>
									    <td id="step4" class="stepFuture">4</td>
								    </tr>
							    </table>
						    </td>
						    <td valign="middle">
							    <a id="btnBackStep" href="#" class="button buttonInline blueHover ruleBack" onclick="oProgressSteps.back(); return false;" title="<%=m_msgRef.GetMessage("lbl back")%>"><%=m_msgRef.GetMessage("lbl back")%></a>
							    <a
							    id="btnNextStep" href="#" class="button buttonRightIcon buttonInline blueHover ruleNext" onclick="oProgressSteps.next(); return false;" title="<%=m_msgRef.GetMessage("next")%>"><%=m_msgRef.GetMessage("next")%></a>
							    <a
							    id="btnDoneSteps" href="#" class="button buttonInline greenHover ruleDone" onclick="oProgressSteps.done(); return false;" title="<%=m_msgRef.GetMessage("res_pcm_exit")%>"><%=m_msgRef.GetMessage("res_pcm_exit")%></a>
							    <a
							    id="btnCancelSteps" href="#" class="button buttonInline redHover ruleCancel" onclick="oProgressSteps.cancel(); return false;" title="Cancel"><%=m_msgRef.GetMessage("btn cancel")%></a>
						    </td>
						    <td align="right" valign="middle">
							    &nbsp;
							    <span id="helpBtn1" style="display: none;">
								    <asp:Literal ID="HelpButton1" runat="server" />
							    </span>
							    <span id="helpBtn2" style="display: none;">
								    <asp:Literal ID="HelpButton2" runat="server" />
							    </span>
							    <span id="helpBtn3" style="display: none;">
								    <asp:Literal ID="HelpButton3" runat="server" />
							    </span>
							    <span id="helpBtn4" style="display: none;">
								    <asp:Literal ID="HelpButton4" runat="server" />
							    </span>
							    <span id="helpBtn5" style="display: none;">
								    <asp:Literal ID="HelpButton5" runat="server" />
							    </span>
						    </td>
					    </tr>
				    </table>
			    </td>
		    </tr>
		</table>
    </div>
</div>
<div id="stepsContainer">
	<div id="stepDescription"><%=m_msgRef.GetMessage("msg conditions match")%></div>
</div>
<asp:Literal runat="server" ID="ltr_wizard_js"></asp:Literal>

