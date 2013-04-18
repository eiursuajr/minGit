<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Scope.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.Scope.Scope" %>
<div class="scope ektronPageInfo">
    <input type="hidden" id="hdnScopeLocalizedJavascriptStrings" runat="server" class="scopeLocalizedStrings" />
    <input type="hidden" class="currentControl" name="CouponScope" value="Scope" />
    <input type="hidden" class="initialized" id="hdnInitialized" runat="server" value="false" />
    <input type="hidden" class="dateFormat" id="hdnDateFormat" runat="server" />
    <table class="ektronGrid">
        <thead>
            <tr class="title-header">
                <th colspan="2"><asp:Literal ID="litCouponScopeHeader" runat="server" /></th>
            </tr>
        </thead>
        <tbody>
            <tr class="application">
                <td class="label"><asp:Literal ID="litApplicationLabel" runat="server" /></td>
                <td class="content">
                    <asp:MultiView ID="mvApplication" runat="server">
                        <asp:View ID="vwViewApplication" runat="server">
                            <asp:Literal ID="litViewApplicationValue" runat="server" />
                        </asp:View>
                        <asp:View ID="vwEditApplication" runat="server">
                            <span class="applicationChoice"><asp:RadioButton ID="rbEntireBasket" runat="server" GroupName="CouponApplication" /></span>
                            <span class="applicationChoice"><asp:RadioButton ID="rbAllApprovedItems" runat="server" GroupName="CouponApplication" /></span>
                            <span class="applicationChoice"><asp:RadioButton ID="rbMostExpensiveItem" runat="server" GroupName="CouponApplication" /></span>
                            <span class="applicationChoice"><asp:RadioButton ID="rbLeastExpensiveItem" runat="server" GroupName="CouponApplication" /></span>
                        </asp:View>
                    </asp:MultiView>
                </td>
            </tr>
            <tr class="customerLimit stripe">
                <asp:MultiView ID="mvCustomerLimit" runat="server">
                    <asp:View ID="vwViewCustomerLimit" runat="server">
                        <td class="label"><asp:Literal ID="litViewCustomerLimitLabel" runat="server" /></td>
                        <td class="content"><asp:Literal ID="litViewCustomerLimitValue" runat="server" /></td>
                    </asp:View>
                    <asp:View ID="vwEditCustomerLimit" runat="server">
                        <td class="label"><asp:Label ID="lblCustomerLimit" runat="server" AssociatedControlID="cbCustomerLimit" /></td>
                        <td class="content">
                            <asp:CheckBox ID="cbCustomerLimit" runat="server" CssClass="customerLimit" />
                        </td>
                    </asp:View>
                </asp:MultiView>
            </tr>
            <tr class="combination">
                <asp:MultiView ID="mvCombination" runat="server">
                    <asp:View ID="vwViewCombination" runat="server">
                        <td class="label"><asp:Literal ID="litViewCombinationLabel" runat="server" /></td>
                        <td class="content"><asp:Literal ID="litViewCombinationValue" runat="server" /></td>
                    </asp:View>
                    <asp:View ID="vwEditCombination" runat="server">
                        <td class="label"><asp:Label ID="lblCombination" runat="server" AssociatedControlID="cbCombination" /></td>
                        <td><asp:CheckBox ID="cbCombination" runat="server" CssClass="customerLimit" /></td>
                    </asp:View>
                </asp:MultiView>
            </tr>
			<tr class = "combination">
               <asp:MultiView ID="mvApplicabletosubcriptions" runat="server">
                    <asp:View ID="vwViewApplicabletosubcriptions" runat="server">
                        <td class="label"><asp:Literal ID="litviewApplicabletosubcriptionsLabel" runat="server" /></td>
                        <td class="content"><asp:Literal ID="litviewApplicabletosubcriptionsValue" runat="server" /></td>
                    </asp:View>
                    <asp:View ID="vwEditApplicabletosubcriptions" runat="server">
                        <td class="label"><asp:Label ID="lblApplicabletosubcriptions" runat="server" AssociatedControlID="cbApplicabletosubcriptions" /></td>
                        <td><asp:CheckBox ID="cbApplicabletosubcriptions" runat="server" CssClass="customerLimit" /></td>
                    </asp:View>
                </asp:MultiView>
            </tr>
			<tr class = "combination">
               <asp:MultiView ID="mvApplicabletoallquantities" Visible="true" runat="server">
                    <asp:View ID="vwViewApplicabletoallquantities" runat="server">
                        <td class="label"><asp:Literal ID="litviewApplicabletoallquantitiesLabel" runat="server" /></td>
                        <td class="content"><asp:Literal ID="litviewApplicabletoallquantitiesValue" runat="server" /></td>
                    </asp:View>
                    <asp:View ID="vwEditApplicabletoallquantities" runat="server">
                        <td class="label"><asp:Label ID="lblApplicabletoallquantities" runat="server" AssociatedControlID="cbApplicabletoallquantities" /></td>
                        <td><asp:CheckBox ID="cbApplicabletoallquantities" runat="server" CssClass="customerLimit" /></td>
                    </asp:View>
                </asp:MultiView>
            </tr>
            <tr class="maxRedemptions">
                <asp:MultiView ID="vwMaxRedemptions" runat="server">
                    <asp:View ID="vwViewMaxRedemptions" runat="server">
                        <td class="label"><asp:Literal ID="litViewMaxRedemptionsLabel" runat="server" /></td>
                        <td class="content"><asp:Literal ID="litViewMaxRedemptionsValue" runat="server" /></td>
                    </asp:View>
                    <asp:View ID="vwEditMaxRedemptions" runat="server">
                        <td class="label"><asp:Label ID="lblMaxRedemptionsLabel" runat="server" AssociatedControlID="txtMaxRedemptions" /></td>
                        <td class="content"><asp:TextBox ID="txtMaxRedemptions" runat="server" CssClass="maxRedemptions" /></td>
                    </asp:View>
                </asp:MultiView>
            </tr>
            <tr class="minRequiredValue">
                <asp:MultiView ID="vwMinRequiredValue" runat="server">
                    <asp:View ID="vwViewMinRequiredValue" runat="server">
                        <td class="label"><asp:Literal ID="litViewMinRequiredValueLabel" runat="server" /></td>
                        <td class="content"><asp:Literal ID="litViewMinRequiredValueValue" runat="server" /></td>
                    </asp:View>
                    <asp:View ID="vwEditMinRequiredValue" runat="server">
                        <td class="label"><asp:Label ID="lblMinRequiredValue" runat="server" AssociatedControlID="txtDollars" /></td>
                        <td class="content">
                            <span class="currencySymbol" title="<%= GetCurrencyName() %>"><%= GetCurrencySymbol() %></span>
                            <asp:TextBox ID="txtDollars" runat="server" CssClass="dollars" />
                            <span class="decimal">.</span>
                            <asp:TextBox ID="txtCents" runat="server" CssClass="cents" MaxLength="2" />
                        </td>
                    </asp:View>
                </asp:MultiView>
            </tr>
            <tr class="startDate">
                <asp:PlaceHolder ID="phViewStartDate" runat="server">
                        <td class="label"><asp:Literal ID="litViewStartDateLabel" runat="server" /></td>
                        <td class="content"><asp:Literal ID="litViewStartDateValue" runat="server" /></td>
               </asp:PlaceHolder>
               <asp:PlaceHolder ID="phEditStartDate" runat="server">
                        <td class="label"><asp:Label ID="lblStartDate" runat="server" AssociatedControlID="hdnStartDate" /></td>
                        <td class="content">
                            <a href="#SetStartDateTime" class="set start" title="<%= GetSetStartDateTimeLabel() %>" onclick="Ektron.Commerce.Coupons.Scope.Modal.show(this);return false;">
                                <img class="set" alt="<%= GetSetStartDateTimeLabel() %>" title="<%= GetSetStartDateTimeLabel() %>" src="<%= GetScopeImagesPath() %>/addCalendarEntry.gif" />
                            </a>
                            <a href="#ClearStartDateTime" class="clear <%= GetStartClearClass() %>" title="<%= GetClearStartDateTimeLabel() %>" onclick="Ektron.Commerce.Coupons.Scope.Dates.clear(this, 'start');return false;">
                                <img class="ektronModalClose" alt="<%= GetClearStartDateTimeLabel() %>" title="<%= GetClearStartDateTimeLabel() %>" src="<%= GetScopeImagesPath() %>/deleteCalendarEntry.gif" />
                            </a>
                            <span class="date"><asp:Literal ID="litStartDate" runat="server" /></span>
                            <span class="time"><asp:Literal ID="litStartTim" runat="server" /></span>
                            <input id="hdnTodaysDate" runat="server" class="todaysDate" name="EktronCommerceCouponsScopeTodaysDate" type="hidden" />
                            <input id="hdnStartDate" runat="server" class="startDate date" name="EktronCommerceCouponsScopeStartDate" type="hidden" />
                            <input id="hdnStartTime" runat="server" class="startTime time" name="EktronCommerceCouponsScopeStartTime" type="hidden" />
                        </td>
                </asp:PlaceHolder>
            </tr>
            <tr class="endDate">
                <asp:PlaceHolder ID="phViewEndDate" runat="server">
                        <td class="label"><asp:Literal ID="litViewEndDateLabel" runat="server" /></td>
                        <td class="content"><asp:Literal ID="litViewEndDateValue" runat="server" /></td>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phEditEndDate" runat="server">
                        <td class="label"><asp:Label ID="lblEndDate" runat="server" AssociatedControlID="hdnEndDate" /></td>
                        <td class="content">
                            <a href="#SetEndDateTime" class="set end" title="<%= GetSetEndDateTimeLabel() %>" onclick="Ektron.Commerce.Coupons.Scope.Modal.show(this);return false;">
                                <img class="ektronModalClose" alt="<%= GetSetEndDateTimeLabel() %>" title="<%= GetSetEndDateTimeLabel() %>" src="<%= GetScopeImagesPath() %>/addCalendarEntry.gif" />
                            </a>
                            <a href="#ClearEndDateTime" class="clear<%= GetEndClearClass() %>" title="<%= GetClearEndDateTimeLabel() %>" onclick="Ektron.Commerce.Coupons.Scope.Dates.clear(this, 'end');return false;">
                                <img class="ektronModalClose" alt="<%= GetClearEndDateTimeLabel() %>" title="<%= GetClearEndDateTimeLabel() %>" src="<%= GetScopeImagesPath() %>/deleteCalendarEntry.gif" />
                            </a>
                            <input id="hdnEndDate" runat="server" class="endDate date" name="EktronCommerceCouponsScopeEndDate" type="hidden" />
                            <input id="hdnEndTime" runat="server" class="endTime time" name="EktronCommerceCouponsScopeEndTime" type="hidden" />
                            <span class="date"><asp:Literal ID="litEndDate" runat="server" /></span>
                            <span class="time"><asp:Literal ID="litEndTime" runat="server" /></span>
                        </td>
                </asp:PlaceHolder>
            </tr>
        </tbody>
    </table>
    <asp:PlaceHolder ID="phModal" runat="server">
        <div id="EktronCouponsScopeModalDatePicker" class="ektronWindow ektronModalWidth-25 ui-dialog ui-widget ui-widget-content ui-corner-all" id="AttributesModal">
            <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix scopeModalHeader">
                <span class="ui-dialog-title dateSelectorHeader"><asp:Literal ID="litModalHeaderLabel" runat="server" /></span>
                <a href="#" class="ui-dialog-titlebar-close ui-corner-all ektronModalClose">
                    <span class="ui-icon ui-icon-closethick"><%= GetLocalizedStringModalClose() %></span>
                </a>
            </div>
            <div class="ui-dialog-content ui-widget-content ektronPageInfo">
                <table>
                    <tbody>
                        <tr class="dateSelector">
                            <td>
                                <div class="time">
                                    <select class="hour" name="CouponScope">
                                        <option value="1">1</option>
                                        <option value="2">2</option>
                                        <option value="3">3</option>
                                        <option value="4">4</option>
                                        <option value="5">5</option>
                                        <option value="6">6</option>
                                        <option value="7">7</option>
                                        <option value="8">8</option>
                                        <option value="9">9</option>
                                        <option value="10">10</option>
                                        <option value="11">11</option>
                                        <option value="12" selected="selected">12</option>
                                    </select>
                                    <span class="separator">:</span>
                                    <select class="minute" name="CouponScope">
                                        <option value="00" selected="selected">00</option>
                                        <option value="01">01</option>
                                        <option value="02">02</option>
                                        <option value="03">03</option>
                                        <option value="04">04</option>
                                        <option value="05">05</option>
                                        <option value="06">06</option>
                                        <option value="07">07</option>
                                        <option value="08">08</option>
                                        <option value="09">09</option>
                                        <option value="10">10</option>
                                        <option value="11">11</option>
                                        <option value="12">12</option>
                                        <option value="13">13</option>
                                        <option value="14">14</option>
                                        <option value="15">15</option>
                                        <option value="16">16</option>
                                        <option value="17">17</option>
                                        <option value="18">18</option>
                                        <option value="19">19</option>
                                        <option value="20">20</option>
                                        <option value="21">21</option>
                                        <option value="22">22</option>
                                        <option value="23">23</option>
                                        <option value="24">24</option>
                                        <option value="25">25</option>
                                        <option value="26">26</option>
                                        <option value="27">27</option>
                                        <option value="28">28</option>
                                        <option value="29">29</option>
                                        <option value="30">30</option>
                                        <option value="31">31</option>
                                        <option value="32">32</option>
                                        <option value="33">33</option>
                                        <option value="34">34</option>
                                        <option value="35">35</option>
                                        <option value="36">36</option>
                                        <option value="37">37</option>
                                        <option value="38">38</option>
                                        <option value="39">39</option>
                                        <option value="40">40</option>
                                        <option value="41">41</option>
                                        <option value="42">42</option>
                                        <option value="43">43</option>
                                        <option value="44">44</option>
                                        <option value="45">45</option>
                                        <option value="46">46</option>
                                        <option value="47">47</option>
                                        <option value="48">48</option>
                                        <option value="49">49</option>
                                        <option value="50">50</option>
                                        <option value="51">51</option>
                                        <option value="52">52</option>
                                        <option value="53">53</option>
                                        <option value="54">54</option>
                                        <option value="55">55</option>
                                        <option value="56">56</option>
                                        <option value="57">57</option>
                                        <option value="58">58</option>
                                        <option value="59">59</option>
                                    </select>
                                    <select class="ampm" name="CouponScope">
                                        <option value="AM" selected="selected">AM</option>
                                        <option value="PM">PM</option>
                                    </select>
                                </div>
                                <div id="EktronScopeDatePicker"></div>
                                <input id="EktronCouponsScopeSelectedDate" type="hidden" name="CouponScope" value="<%= GetTodaysDate() %>" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                <p class="addDefaultNodeButtons ektronModalButtonWrapper clearfix">
                    <a href="#Cancel" class="button buttonRight redHover buttonRemove" title="<%= GetLocalizedStringCancel() %>" onclick="Ektron.Commerce.Coupons.Scope.Modal.hide();return false;">
                        <%= GetLocalizedStringCancel() %>
                    </a>
                    <a href="#OK" class="button buttonRight greenHover buttonAdd" title="<%= GetLocalizedStringOk() %>" onclick="Ektron.Commerce.Coupons.Scope.Modal.save();return false;">
                        <%= GetLocalizedStringOk() %>
                    </a>    
                </p>
            </div>
        </div>
    </asp:PlaceHolder>
</div>