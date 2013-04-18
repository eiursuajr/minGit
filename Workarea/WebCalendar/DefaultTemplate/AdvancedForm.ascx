<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdvancedForm.ascx.cs" Inherits="SchedulerTemplatesCS.AdvancedForm" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../../Community/DistributionWizard/Metadata.ascx" TagName="Metadata" TagPrefix="ektronUC" %>
<%@ Register Src="../../PageBuilder/taxonomytree.ascx" TagName="SelectTaxonomy" TagPrefix="ektronUC" %>

<%@ Register src="../../controls/Editor/ContentDesignerWithValidator.ascx" tagname="ContentDesignerWithValidator" tagprefix="uc1" %>

<div class="rsAdvancedEdit" style="position: relative">
    <input type="hidden" id="AddEventFormDisplay" name="AddEventFormDisplay" value="true" />
	<%-- Title bar. --%>
	<div class="rsAdvTitle">
		<%-- The rsAdvInnerTitle element is used as a drag handle when the form is modal. --%>
		<h1 class="rsAdvInnerTitle">
		    <span class="leftTitle">
		        <%#ContentApi.EkMsgRef.GetMessage("edit ev")%>
		    </span>
		    <asp:LinkButton
			    runat="server" ID="AdvancedEditCloseButton"
			    CssClass="rsAdvEditClose"
			    CommandName="Cancel"
			    CausesValidation="false"
		        OnClientClick="Ektron.WebCalendar.AdvancedForm.destroy(true);"
			    ToolTip='<%# Owner.Localization.AdvancedClose %>'>
			    <%# Owner.Localization.AdvancedClose %>
		    </asp:LinkButton>
		</h1>
		<br class="clearBR" />
	</div>
	<div class="rsAdvContentWrapper">
		<%-- use tabs here - tabs are Event, Recurrence, Taxonomy, Metadata --%>
		<asp:Panel	runat="server" ID="AdvancedEditOptionsPanel" CssClass="rsAdvOptions">
		    <div class="initialization" style="display:none;">
		        <asp:TextBox ID="initUserCulture" CssClass="initUserCulture" runat="server"></asp:TextBox>
                <asp:TextBox ID="initTimeDisplayFormat" CssClass="initTimeDisplayFormat" runat="server"></asp:TextBox>
                <asp:TextBox ID="initTime8AM" CssClass="initTime8AM" runat="server"></asp:TextBox>
                <asp:TextBox ID="initTime9AM" CssClass="initTime9AM" runat="server"></asp:TextBox>
                <asp:TextBox ID="initTimeDayStart" CssClass="initTimeDayStart" runat="server"></asp:TextBox>
                <asp:TextBox ID="initErrTitleRequired" CssClass="initErrTitleRequired" runat="server"></asp:TextBox>
                <asp:TextBox ID="initErrStartRequired" CssClass="initErrStartRequired" runat="server"></asp:TextBox>
                <asp:TextBox ID="initErrEndRequired" CssClass="initErrEndRequired" runat="server"></asp:TextBox>
                <asp:TextBox ID="initErrMetaDataRequired" CssClass="initErrMetaDataRequired" runat="server"></asp:TextBox>
                <asp:TextBox ID="initErrTaxonomyRequired" CssClass="initErrTaxonomyRequired" runat="server"></asp:TextBox>
                <asp:TextBox ID="initErrAliasRequired" CssClass="initErrAliasRequired" runat="server"></asp:TextBox>
                <asp:TextBox ID="initErrStartBeforeEnd" CssClass="initErrStartBeforeEnd" runat="server"></asp:TextBox>
                <asp:TextBox ID="initCalendarButtonAlt" CssClass="initCalendarButtonAlt" runat="server"></asp:TextBox>
                <asp:TextBox ID="initCalendarButton" CssClass="initCalendarButton" runat="server"></asp:TextBox>
                <asp:TextBox ID="initTimePickButtonAlt" CssClass="initTimePickButtonAlt" runat="server"></asp:TextBox>
                <asp:TextBox ID="initTimePickButton" CssClass="initTimePickButton" runat="server"></asp:TextBox>
                <asp:TextBox ID="initErrorIcon" CssClass="initErrorIcon" runat="server"></asp:TextBox>
                <asp:TextBox ID="initLocationMaxLength" CssClass="initLocationMaxLength" runat="server"></asp:TextBox>
                <asp:TextBox ID="initTitleMaxLength" CssClass="initTitleMaxLength" runat="server"></asp:TextBox>
                <asp:TextBox ID="initInvalidCharTitle" CssClass="initInvalidCharTitle" runat="server"></asp:TextBox>
                <asp:TextBox ID="initInvalidCharLocation" CssClass="initInvalidCharLocation" runat="server"></asp:TextBox>
		    </div>
		    <div class="rsAdvOptionsScroll" id="TabsContainer" runat="server">
                <ul>
                    <li><a title="Event" href='#Event''>Event</a></li>
                    <li><a title="Recurrence" href='#Recurrence''>Recurrence</a></li>
                    <asp:PlaceHolder ID="phTaxonomyTab" runat="server">
                        <li><a title="Taxonomy" href='#Taxonomy''>Taxonomy</a></li>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phMetadata" runat="server">
                        <li><a title="Metadata" href='#Metadata''>Metadata</a></li>
                     </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phAliasTab" runat="server" Visible="false">
                        <li><a title="Alias" href='#Alias''>Alias</a></li>
                    </asp:PlaceHolder>                                   
                    <li class="floater"></li>
                    <asp:literal id="btnHelp" runat="server"/>    
                </ul>
                <br class="clearBR" />
                <div id='Event'>
				    <asp:Panel runat="server" ID="BasicControlsPanel" CssClass="rsAdvBasicControls" OnDataBinding="BasicControlsPanel_DataBinding">
				        <asp:Label ID="labelfortitle" CssClass="topLabel" runat="server"></asp:Label><asp:TextBox ToolTip="Title" ID="txtTitle" runat="server" CssClass="titleTextBox"></asp:TextBox><br />
				        <asp:Label ID="labelforlocation" CssClass="topLabel" runat="server"></asp:Label><asp:TextBox ToolTip="Location" ID="txtLocation" runat="server" CssClass="locationTextBox"></asp:TextBox><br />
				        <asp:Label ID="labelfordescription" runat="server"><%# Owner.Localization.AdvancedDescription %>:</asp:Label>
				        <uc1:ContentDesignerWithValidator ID="ContentDesigner" runat="server" Width="100%" ShowHtmlMode="true" />
                        <br />
                        
                        <div class="AdvCalendarSelect" id="AdvCalendarSelect" runat="server">
                            <span>Select Calendar to insert this event into:</span><br />
                            <asp:DropDownList ToolTip="Select Calendar from Drop Down Menu" ID="sourceSelector" runat="server" RepeatLayout="Flow" CssClass="CalendarSelect"></asp:DropDownList>
                        </div>
					    
					    <ul class="rsTimePickers">
						    <li class="rsTimePick">
							    <span style="display:inline-block; width:80px;text-align:right;"><label class="lblStartDate" for='<%# StartDate.ClientID %>'><%# Owner.Localization.AdvancedFrom %></label></span>
							    <asp:TextBox ToolTip="Start Date" ID="StartDate" CssClass="datetime startdate" runat="server"></asp:TextBox>
							    <asp:TextBox ToolTip="Start Time" ID="StartTime" CssClass="timepick starttime" runat="server"></asp:TextBox>
							    <asp:HiddenField ID="hdnOriginalStartDateTime" runat="server" Value="" />
                            </li>							    
						    <li class="rsTimePick">
							    <span style="display:inline-block; width:80px;text-align:right;"><label class="lblEndDate" runat="server" id="lblEndDate" for='<%# EndDate.ClientID %>'><%# Owner.Localization.AdvancedTo%></label></span>
							    <asp:TextBox ToolTip="Enter End Date here" ID="EndDate" CssClass="datetime enddate" runat="server"></asp:TextBox>
							    <asp:TextBox ToolTip="Enter End Time here" ID="EndTime" CssClass="timepick endtime" runat="server"></asp:TextBox>
                            </li>							    
							    
							    
						    <li class="rsAllDayWrapper">
							    <asp:CheckBox runat="server" ID="AllDayEvent" CssClass="rsAdvChkWrap allday" Checked="false" />
						    </li>
					    </ul>
   				    </asp:Panel>
                </div>
                <div id='Recurrence'>
				    <asp:Panel runat="server" ID="RecurrenceCheckBoxPanel">
					    <asp:CheckBox runat="server" ID="RecurrentAppointment" CssClass="rsAdvChkWrap recurCheck" Checked="false" style="float: none" />
				    </asp:Panel>
				    <asp:Panel runat="server" ID="RecurrencePanel" Style="display: none;" CssClass="rsAdvRecurrencePanel">

				        <asp:Panel runat="server" ID="RecurrencePatternPanel" CssClass="rsAdvRecurrencePatterns"  OnDataBinding="RecurrencePatternPanel_DataBinding">
					        <span class="rsAdvResetExceptions">
						        <asp:LinkButton runat="server" ID="ResetExceptions" OnClick="ResetExceptions_OnClick" />
					        </span>
        											
					        <asp:Panel runat="server" ID="RecurrenceFrequencyPanel" CssClass="rsAdvRecurrenceFreq">
						        <ul class="rsRecurrenceOptionList">
							        <li><asp:RadioButton runat="server" GroupName="RepeatFrequency" ID="RepeatFrequencyDaily" /></li>	
							        <li><asp:RadioButton runat="server" GroupName="RepeatFrequency" ID="RepeatFrequencyWeekly" /></li>
							        <li><asp:RadioButton runat="server" GroupName="RepeatFrequency" ID="RepeatFrequencyMonthly" /></li>
							        <li><asp:RadioButton runat="server" GroupName="RepeatFrequency" ID="RepeatFrequencyYearly" /></li>
						        </ul>
					        </asp:Panel>
					        <asp:Panel runat="server" ID="RecurrencePatternDailyPanel" CssClass="rsAdvPatternPanel rsAdvDaily" Style="display:none;">
						        <ul>
							        <li>
								        <asp:RadioButton	runat="server" ID="RepeatEveryNthDay" Checked="true"									
													        GroupName="DailyRecurrenceDetailRadioGroup" CssClass="rsAdvRadio" />
									    <asp:TextBox ToolTip="Daily Repeat Interval" ID="DailyRepeatInterval" runat="server" CssClass="ui-numeric-input"></asp:TextBox>
								        <%# Owner.Localization.AdvancedDays %>
							        </li>
							        <li>
								        <asp:RadioButton runat="server" ID="RepeatEveryWeekday" GroupName="DailyRecurrenceDetailRadioGroup" CssClass="rsAdvRadio" />
							        </li>
						        </ul>
					        </asp:Panel>
					        <asp:Panel runat="server" ID="RecurrencePatternWeeklyPanel" CssClass="rsAdvPatternPanel rsAdvWeekly" Style="display:none;">
						        <div>
							        <%# Owner.Localization.AdvancedRecurEvery %>
								    <asp:TextBox ToolTip="Weekly Repeat Interval" ID="WeeklyRepeatInterval" runat="server" CssClass="ui-numeric-input"></asp:TextBox>
							        <%# Owner.Localization.AdvancedWeeks %>
						        </div>
						        <ul class="rsAdvWeekly_WeekDays">
							        <li><asp:CheckBox runat="server" CssClass="rsAdvCheckboxWrapper" ID="WeeklyWeekDaySunday" /></li>
							        <li><asp:CheckBox runat="server" CssClass="rsAdvCheckboxWrapper" ID="WeeklyWeekDayMonday" /></li>
							        <li><asp:CheckBox runat="server" CssClass="rsAdvCheckboxWrapper" ID="WeeklyWeekDayTuesday" /></li>
							        <li><asp:CheckBox runat="server" CssClass="rsAdvCheckboxWrapper" ID="WeeklyWeekDayWednesday" /></li>
							        <li><asp:CheckBox runat="server" CssClass="rsAdvCheckboxWrapper" ID="WeeklyWeekDayThursday" /></li>
							        <li><asp:CheckBox runat="server" CssClass="rsAdvCheckboxWrapper" ID="WeeklyWeekDayFriday" /></li>
							        <li><asp:CheckBox runat="server" CssClass="rsAdvCheckboxWrapper" ID="WeeklyWeekDaySaturday" /></li>
						        </ul>
					        </asp:Panel>
					        <asp:Panel runat="server" ID="RecurrencePatternMonthlyPanel" CssClass="rsAdvPatternPanel rsAdvMonthly" Style="display:none;">
						        <ul>
							        <li>
								        <asp:RadioButton    runat="server" ID="RepeatEveryNthMonthOnDate" Checked="true"									
													        GroupName="MonthlyRecurrenceRadioGroup" CssClass="rsAdvRadio" />
    								    <asp:TextBox ID="MonthlyRepeatDate" runat="server" CssClass="ui-numeric-input"></asp:TextBox>

								        <%# Owner.Localization.AdvancedOfEvery %>

    								    <asp:TextBox ID="MonthlyRepeatIntervalForDate" runat="server" CssClass="ui-numeric-input"></asp:TextBox>
								        <%# Owner.Localization.AdvancedMonths %>
							        </li>
							        <li>
								        <asp:RadioButton    runat="server" ID="RepeatEveryNthMonthOnGivenDay"									
													        GroupName="MonthlyRecurrenceRadioGroup" CssClass="rsAdvRadio" />
								        <asp:DropDownList	runat="server" ID="MonthlyDayOrdinalDropDown" Width="70px"
									        CssClass="rsAdvRecurrenceDropDown">
									        <%--Populated from code-behind--%>
								        </asp:DropDownList>
								        <asp:DropDownList	runat="server" ID="MonthlyDayMaskDropDown" Width="110px"
									        CssClass="rsAdvRecurrenceDropDown">
									        <%--Populated from code-behind--%>
								        </asp:DropDownList>
								        <%# Owner.Localization.AdvancedOfEvery %>
    								    <asp:TextBox ID="MonthlyRepeatIntervalForGivenDay" runat="server" CssClass="ui-numeric-input"></asp:TextBox>
								        <%# Owner.Localization.AdvancedMonths %>
							        </li>
						        </ul>
					        </asp:Panel>
					        <asp:Panel runat="server" ID="RecurrencePatternYearlyPanel" CssClass="rsAdvPatternPanel rsAdvYearly" Style="display:none;">
						        <ul>
							        <li>
								        <asp:RadioButton    runat="server" ID="RepeatEveryYearOnDate" Checked="true"									
													        GroupName="YearlyRecurrenceRadioGroup" CssClass="rsAdvRadio" />
								        <asp:DropDownList runat="server" ID="YearlyRepeatMonthForDate" Width="90px">
									        <%--Populated from code-behind--%>
								        </asp:DropDownList>
    								    <asp:TextBox ID="YearlyRepeatDate" runat="server" CssClass="ui-numeric-input"></asp:TextBox>
							        </li>
							        <li>
								        <asp:RadioButton    runat="server" ID="RepeatEveryYearOnGivenDay"									
													        GroupName="YearlyRecurrenceRadioGroup" CssClass="rsAdvRadio" />
								        <asp:DropDownList	runat="server" ID="YearlyDayOrdinalDropDown" Width="70px"
									        CssClass="rsAdvRecurrenceDropDown">
									        <%--Populated from code-behind--%>
								        </asp:DropDownList>
								        <asp:DropDownList	runat="server" ID="YearlyDayMaskDropDown" Width="110px"
									        CssClass="rsAdvRecurrenceDropDown">
									        <%--Populated from code-behind--%>
								        </asp:DropDownList>
								        <%# Owner.Localization.AdvancedOf %>
								        <asp:DropDownList	runat="server" ID="YearlyRepeatMonthForGivenDay" Width="90px">
									        <%--Populated from code-behind--%>
								        </asp:DropDownList>
							        </li>
						        </ul>
					        </asp:Panel>
				        </asp:Panel>
				        <asp:Panel runat="server" ID="RecurrenceRangePanel" CssClass="rsAdvRecurrenceRangePanel" OnDataBinding="RecurrenceRangePanel_DataBinding">
						    <ul>
						        <li class="rsTimePick">
    							    <label class="lblStartDate" title="Start Date">Start Date</label>
        						    <input type="text" title="Enter Start Date Here" class="datetime startdatesecondary" /><br />
						        </li>
						    </ul>
						    <br style="clear:both;" />
					        <ul>
						        <li>
							        <asp:RadioButton    runat="server" ID="RepeatIndefinitely" Checked="true"								
												        GroupName="RecurrenceRangeRadioGroup" CssClass="rsAdvRadio" />
						        </li>
						        <li>
							        <asp:RadioButton	runat="server" ID="RepeatGivenOccurrences"								
												        GroupName="RecurrenceRangeRadioGroup" CssClass="rsAdvRadio" />
					                <asp:TextBox ToolTip="Range Occurrences" ID="RangeOccurrences" runat="server" CssClass="ui-numeric-input"></asp:TextBox>
							        <%# Owner.Localization.AdvancedOccurrences %>
						        </li>
						        <li class="rsTimePick" style="width:440px;">
							        <asp:RadioButton	runat="server" ID="RepeatUntilGivenDate"								
												        GroupName="RecurrenceRangeRadioGroup" CssClass="rsAdvRadio" />
							        <asp:TextBox ToolTip="Range End Date" ID="RangeEndDate" CssClass="datetime rangeenddate" runat="server"></asp:TextBox>
    							    <asp:TextBox ToolTip="Range End Time" ID="RangeEndTime" CssClass="timepick" runat="server"></asp:TextBox>
						        </li>
					        </ul>
				        </asp:Panel>

				    </asp:Panel>
				    <asp:HiddenField runat="server" ID="OriginalRecurrenceRule" />
                </div>
                <asp:PlaceHolder ID="phTaxonomy" runat="server">
                    <div id='Taxonomy'>
                        <ektronUC:SelectTaxonomy ID="TaxonomySelector" runat="server" FolderID="0" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phAliases" runat="server" Visible="false">
                    <div id="Alias">
                        <asp:Panel ID="pnlManualAlias" runat="server" Visible="false">
                            <div class="ektronHeader">
                                <asp:Label ID="lblAliasHeader" runat="server"><%#ContentApi.EkMsgRef.GetMessage("lbl tree url manual aliasing") %></asp:Label>
                            </div>
                            <div class="ektronBorder" style="width: auto; height: auto; overflow: auto;">
                                <table class="ektronGrid" width="100%">
                                    <col width="20%" />
                                    <col width="80%" />
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblManualAlias" runat="server">
                                            <b><%#ContentApi.EkMsgRef.GetMessage("lbl primary") + " " + ContentApi.EkMsgRef.GetMessage("lbl alias name") + ":"%></b>
                                                </asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblSitePath" runat="server"><%#ContentApi.SitePath %></asp:Label>
                                                <asp:TextBox ID="txtAliasName" runat="server" MaxLength="35" CssClass="AliasName"></asp:TextBox>
                                                <asp:DropDownList ID="ddlAliasExtensions" runat="server" CssClass="AliasDropDown">
                                                </asp:DropDownList>
                                                <input type="hidden" class="AliasExtension" id="hdnAliasExtension" runat="server"
                                                    value="" style="display: none;" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <span id="aliasRequired" class="AliasRequiredBool" runat="server" style="display: none;">
                            </span>
                        </asp:Panel>
                        <asp:Panel ID="pnlAutoAlias" runat="server" Visible="false">
                            <div class="ektronHeader">
                                <asp:Label ID="lblAutoAliasHeader" runat="server"><%#ContentApi.EkMsgRef.GetMessage("lbl automatic")%></asp:Label>
                            </div>
                            <div class="ektronBorder" style="width: auto; height: auto; overflow: auto;" id="autoAliasList">
                                <table class="ektronGrid" width="100%">
                                    <col width="20%" />
                                    <col width="80%" />
                                    <tbody>
                                        <tr class="title-header">
                                            <th>
                                                <b>
                                                    <asp:Label ID="lblAliasType" runat="server"><%#ContentApi.EkMsgRef.GetMessage("generic type")%></asp:Label></b>
                                            </th>
                                            <th>
                                                <b>
                                                    <asp:Label ID="lblAutoAliasName" runat="server"><%#ContentApi.EkMsgRef.GetMessage("lbl alias name")%></asp:Label></b>
                                            </th>
                                        </tr>
                                        <asp:Repeater ID="rpAutoAlias" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <%#(Container.DataItem as Ektron.Cms.Common.UrlAliasAutoData).AutoAliasType%>
                                                    </td>
                                                    <td>
                                                        <%#(Container.DataItem as Ektron.Cms.Common.UrlAliasAutoData).AliasName%>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </asp:Panel>
                    </div>
                </asp:PlaceHolder>
                <div id='Metadata'>
                    <ektronUC:Metadata ID="MetadataSelector" runat="server" FolderID="0" ForceNewWindow="true" ResultType="Staged" />
                </div>
		    </div>
		</asp:Panel>
		
		<div class="rsAdvancedSubmitArea">
		    <div class="rsErrors rsAdvButtonWrapper ui-state-error ui-corner-all" style="float:left; padding:6px; margin:4px; display:none;">
	            <span class="ui-icon ui-icon-alert" style="margin-right: 0.3em; float:left;"></span>
                <strong>field required</strong>
		    </div>
			<div class="rsAdvButtonWrapper" style="float:right;">
				<asp:LinkButton ToolTip="Update"
					runat="server" ID="UpdateButton"
					CssClass="rsAdvEditSave" 
					OnClientClick="return Ektron.WebCalendar.AdvancedForm.Validation.Validate();">
					<span><%# Owner.Localization.Save %></span>
				</asp:LinkButton>

                <!-- jsEnableWorkareaNav is set inside Ektron.WebCalendar.AdvancedForm.destroy() -->
				<asp:LinkButton ToolTip="Cancel"
					runat="server" ID="CancelButton"
					CssClass="rsAdvEditCancel"
					CommandName="Cancel"
				    OnClientClick="Ektron.WebCalendar.AdvancedForm.destroy(true);"
					CausesValidation="false">
					<span><%# Owner.Localization.Cancel %></span>
				</asp:LinkButton>
			</div>
			<span style="display:block; height:0px; clear:both;"></span>
		</div>
	</div>
</div>
<asp:Literal ID="extrascript" runat="server"></asp:Literal>
