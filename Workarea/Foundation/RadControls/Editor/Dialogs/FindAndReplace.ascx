<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.FindAndReplace" AutoEventWireUp="false" CodeBehind="FindAndReplace.ascx.cs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ OutputCache Duration="600" VaryByParam="Language;SkinPath" %>

<style type="text/css">
    .Ektron_StandardButton
    {
    	width: 110px;
    }
</style>
<div id="MainContainer">
    <div class="Ektron_DialogTabstrip_Container">
	    <telerik:tabcontrol id="TabHolder" runat="server" resizecontrolid="MainContainer">
		    <telerik:tab text="<script>localization.showText('Tab1HeaderText');</script>" selected="True" elementid="TabbedEmptySpan1" onclientclick="SwitchTab(false);" />
		    <telerik:tab text="<script>localization.showText('Tab2HeaderText');</script>" elementid="TabbedEmptySpan2" onclientclick="SwitchTab(true);" />
	    </telerik:tabcontrol>
    </div>

    <span id="TabbedEmptySpan1"></span><span id="TabbedEmptySpan2"></span>

    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <div class="Ektron_TopLabel">
            <label title="Search" for="<%=this.ClientID%>_searchText">
	            <script>localization.showText('SearchText')</script>
            </label>
        </div>
		<input title="Enter Text for Search Here" type="text" id="<%=this.ClientID%>_searchText" class="Ektron_StandardTextBox" tabindex="1" />
		<span class="Ektron_LeftSpaceVerySmall"></span><%--Ektron start--%>
        <div style="position: absolute; top: 40px; right: 8px;">
            <button title="Find" class="Ektron_StandardButton" type="button" onclick="javascript:Find();" tabindex="9">
	            <script>localization.showText('FindNext')</script>
            </button>
            <span id="<%=this.ClientID%>_replaceButtonsContainer">
                <br />
                <button title="Replace" class="Ektron_StandardButton" style="margin: 2px auto 2px auto; align: right;" type="button" onclick="javascript:Replace();" tabindex="10">
                    <script>localization.showText('Replace')</script>
                </button>
                <br />
                <button title="Replace All" class="Ektron_StandardButton" type="button" onclick="javascript:ReplaceAll();" tabindex="11">
                    <script>localization.showText('ReplaceAll')</script>
                </button>	
            </span>
        </div><%--Ektron end--%>
        
        <div id="<%=this.ClientID%>_replaceContainer">
            <div class="Ektron_TopLabel">
	            <label title="Replacement Text" for="<%=this.ClientID%>_replacementText">
		            <script>localization.showText('ReplacementText')</script>
	            </label>
	        </div>
	        <input title="Enter Replacement Text" type="text" id="<%=this.ClientID%>_replacementText" name="replacementText" class="Ektron_StandardTextBox" tabindex="2" />                   
        </div>       
        	        	
        <div class="Ektron_TopSpaceVerySmall" style="width:100%">
            <table width="100%">
            <tbody>
	            <tr>
		            <td style="width:50%">
			            <fieldset>
				            <legend title="Options to choose the selection of text to perform the Search in">
					            <script>localization.showText('SearchIn')</script>
				            </legend>
				            <input type="radio" title="Search Whole Document" name="SearchType" id="searchTypeRadioWholeText" onclick="SetSearchType(true);" value="Whole" checked="checked" tabindex="3" />
				            <label title="Search Whole Document" for="searchTypeRadioWholeText">
				                <script>localization.showText('SearchEntireText')</script>
				            </label>
				            <br>
				            <input type="radio" title="Search Selection Only" name="SearchType" id="searchTypeRadioSelectionOnly" onclick="SetSearchType(false);" value="Selection" tabindex="4" />
				            <label title="Search Selection Only" for="searchTypeRadioSelectionOnly">
				                <script>localization.showText('SearchSelection')</script>
				            </label>
			            </fieldset>
		            </td>
		            <td><span class="Ektron_LeftSpaceSmall"></span></td>
		            <td style="width:50%">
			            <fieldset>
				            <legend title="Options to choose the Direction of the Search">
					            <script>localization.showText('SearchDirection')</script>
				            </legend>
				            <input type="radio" title="Searches the Text Up" id="searchDirectionRadioUp" name="SearchDir" onclick="SetSearchDirection(false);" value="Up" tabindex="5" />
				            <label title="Search Up" for="searchDirectionRadioUp">
				                <script>localization.showText('SearchUp')</script>
				            </label>
				            <br>
				            <input type="radio" title="Searches the text Down" id="searchDirectionRadioDown" name="SearchDir" onclick="SetSearchDirection(true);" checked="checked" value="Down" tabindex="6" />
				            <label title="Search Down" for="searchDirectionRadioDown">
				                <script>localization.showText('SearchDown')</script>
				            </label>
			            </fieldset>
		            </td>
	            </tr>
	           </tbody>
            </table>
        </div>
        
        <div class="Ektron_TopSpaceVerySmall"></div>
        
        <input title="Match Text Case" type="checkbox" id="searchModifierMatchCaseCheckBox" onclick="SetCaseMatch(this.checked);" tabindex="7" />
        <label title="Match Case" for="searchModifierMatchCaseCheckBox">
            <script>localization.showText('MatchCase')</script>
        </label>
        <br />
        <input title="Search using Whole Words Only" type="checkbox" id="searchModifierMatchWholeWordsCheckBox" onclick="SetWholeWordMatch(this.checked);" tabindex="8" />
        <label title="Whole Words Only" for="searchModifierMatchWholeWordsCheckBox">
            <script>localization.showText('WholeWordsOnly')</script>
        </label>

        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>

    <div class="Ektron_Dialogs_ButtonContainer"><%--Ektron start--%>
        <button title="Submit" class="Ektron_StandardButton" id="submitButton" onclick="CloseWindow();" type="button" tabindex="12">
	        <script>localization.showText('Close')</script>
        </button><%--Ektron end--%>
    </div>
</div>

<script type="text/javascript">
<!--
	var globalControlRef = null;
	var globalControlId = "<%=this.ClientID%>";
	var globalWorkingArea = null;
	var	globalOriginalHtml = null;
    var globalWorkingRange = null; // Ektron 
    var previousSearchWord = ""; // Ektron 
		
	var changesMade = false;
			
	var searchBox = document.getElementById(globalControlId + "_searchText");	
	var replaceBox = document.getElementById(globalControlId + "_replacementText");
		
	function SetSearchType(bWhole)
	{
		globalControlRef.SelectionOnly = !bWhole;
	}
	
	function SetSearchDirection(bDown)
	{
		globalControlRef.SearchUp = !bDown;
	}
	
	function SetCaseMatch(bCaseSensitive)
	{
		globalControlRef.CaseSensitive = bCaseSensitive;
	}
	
	function SetWholeWordMatch(bWholeWord)
	{
		globalControlRef.WholeWord = bWholeWord;
	}			
	
	function Find()
	{
        // Ektron start - #60730
		if (searchBox.value != previousSearchWord)
        {
            globalControlRef.ResetSelectionRange();
        }
        // Ektron end
        globalControlRef.SearchWord = searchBox.value;				
		globalControlRef.Find();
        previousSearchWord = searchBox.value; // Ektron - #60730		 
	}
	
	function Replace()
	{
		changesMade = true;
		globalControlRef.SearchWord = searchBox.value;
		globalControlRef.ReplaceWord = replaceBox.value;		
		globalControlRef.Replace();
	}
	
	function ReplaceAll()
	{
		changesMade = true;
		
		globalControlRef.ResetEngine();
			
		globalControlRef.SearchWord = searchBox.value;
		globalControlRef.ReplaceWord = replaceBox.value;		
		
		globalControlRef.ReplaceAll();
	}

	function SwitchTab(showReplace)
	{
		//Reset engine
		if (globalControlRef) 
		{
		    globalControlRef.ResetEngine();
		}
		
		//Set which fields to be visible
		var display = showReplace ? "" : "none";	
		var replaceContainer = document.getElementById(globalControlId + "_replaceContainer");
        replaceContainer.style.display = display;
        document.getElementById(globalControlId + "_replaceButtonsContainer").style.display = display;
						
		//Set focus
		var searchTextHolder = document.getElementById(globalControlId + "_searchText");
		if (searchTextHolder)
		{
		    searchTextHolder.focus();
		}
	}

	function CloseWindow()
	{	
		CloseDlg(null, null, false);
	}

	function InitControl()
	{
		var args = GetDialogArguments();

		globalWorkingArea = args.area;
        globalWorkingRange = args.range; // Ektron - #60730
		globalOriginalHtml = globalWorkingArea.innerHTML;	 			
		globalControlRef = new FindAndReplaceControl('<%=this.ClientID%>', globalWorkingArea, globalWorkingRange);	// Ektron - #60730					
        			
		globalControlRef.OnCloseWindow = CloseWindow;
	}

	AttachEvent(window, "load", InitControl);
// -->
</script>