//var catTree=null;
var readCategoryFromTree=true;
var redrawTreeOnFF = false;
var pageLinkClick=false;

String.prototype.format=function()
{
	var toReturn=this;for(var i=0;i<arguments.length;i++)
	{
		var regex=new RegExp("\\{"+i+"\\}","g");toReturn=toReturn.replace(regex,arguments[i]);
	}
	return toReturn;
};

String.format=function()
{
	var pattern=arguments[0];var params=[];
	for(var i=1;i<arguments.length;i++) 
	{
		params[i - 1] = arguments[i];
	}
	return pattern.format(params);
};

var EkCookie=
{
	setCookie:function(name,value)
	{
		if(name.length<1)
		{
			return;	
		}
		if(0<value.length)
		{
			document.cookie=""+name+"="+value;
		}else{
			document.cookie=name+"=";
		}
	}
	,getCookie:function(name)
	{
		var value="";
		var index=0;
		var oDoc=document;
		if(oDoc.cookie)
		{
			index=oDoc.cookie.indexOf(name+"=");
		}
		else 
		{
			index=-1;
		}
		if(index<0)
		{
		value="";
		}
		else
		{
			var countbegin=(oDoc.cookie.indexOf("=",index)+1);
			if(0<countbegin)
			{
				var countend=oDoc.cookie.indexOf(";",countbegin);
				if(countend<0)
				{
					countend=oDoc.cookie.length;
				}
				value=oDoc.cookie.substring(countbegin,countend);
			}
			else
			{
				value="";
			}
		}
		return value;
	}
	,deleteCookie:function(name)
	{
		document.cookie=name+"=";
	}
};

var EkSearchResult=
{
	keys:Array(),values:Array(),parseContextData:function(query)
	{
		var keys = [];
		var values = [];
		var pairs = query.split("&");
		for(var i=0;i<pairs.length;i++)
		{
			var pos=pairs[i].indexOf('=');
			if(pos>=0)
			{
				var argname=decodeURIComponent(pairs[i].substring(0,pos));
				var value=decodeURIComponent(pairs[i].substring(pos + 1));
				this.keys[this.keys.length]=argname;
				this.values[this.values.length]=value;
			}
		}
	}
	,getKeyValue:function(key)
	{
		var value=null;
		for(var i=0;i<this.keys.length;i++)
		{
			if(this.keys[i]==key)
			{
				value=this.values[i];break;
			}
		}
		return value;
	}
	,displayResult:function(result,context)
	{
	    //28489 - always show basic UI, when displaying results.
	    //debugger;
	    var searchMode=document.getElementById('ecmsearchmode');
	    if ((searchMode != null) && (searchMode.value == "advanced"))
	    {
	        searchMode.value = "normal";
	        EkSearch.showSearchOps(searchMode.value);
	    }
		
		//Advanced Search header - change text based on search type
		var searchType=document.getElementById('ecmSearchForTypes');
		var advSearchLegend=document.getElementById('advancedTermsAnchor');
		var advSearchdefault=document.getElementById('ecmDefaultAdvText');
		if ((advSearchLegend != null) && (advSearchdefault != null))
		{ 
		    if (searchType.value != 'all')
		    {
		        if ((document.forms[0].searchScope != null) && (document.forms[0].searchScope.options != null))
		            advSearchLegend.innerHTML = advSearchdefault.value + " - " + document.forms[0].searchScope.options[document.forms[0].searchScope.selectedIndex].text;
		    }
		    else
		        advSearchLegend.innerHTML = advSearchdefault.value;
		}

    	EkSearchResult.parseContextData(context);
		EkCookie.setCookie('contextparams',context);
		if (document.getElementById('ecmsearchpage')!==null)
		{
			EkCookie.setCookie('pageparams',document.getElementById('ecmsearchpage').value);
		}
		if (document.getElementById(EkSearchResult.getKeyValue("control"))!==null)
		{
		    //35291 - Some browsers like safari do not execute inline scripts in callback result, use Ektron Javascript Library to fix this
		    //document.getElementById(EkSearchResult.getKeyValue("control")).innerHTML='';
		    //var dataelem = document.createElement('div');dataelem.innerHTML=result; document.getElementById(EkSearchResult.getKeyValue("control")).appendChild(dataelem);
		    $ektron("#" + EkSearchResult.getKeyValue("control")).html(result);
		}
		try
		{
			if(document.getElementById("__EkAjaxHidden$"+EkSearchResult.getKeyValue("control"))!==null)
			{
			
				document.getElementById("__EkAjaxHidden$"+EkSearchResult.getKeyValue("control")).value=result;
			}
		}
		catch(e)
		{}
	}
	,displayError:function(message,context)
	{
		alert('An unhandled exception has occurred:\n'+message);
	}
};

var EkSearch =
{
    imagepath:'images',searchInit:function()
	{
        var searchType = document.getElementById('ecmsearchmode');
        EkSearch.showSearchOps(searchType.value);
        EkSearch.addMeta('init');
		if(EkCookie.getCookie('searchcookie')!==null || EkCookie.getCookie('searchcookie')!=='')
		{
            _backbuttonaction = true;
        }
    }
	,showSearchOps:function(value)
	{
		if(value=='normal')
		{
	        if (document.getElementById('advancedTerms') !== null) { document.getElementById('advancedTerms').style.display = "none"; }
	        if (document.getElementById('advancedTermsAnchor') !== null) { document.getElementById('advancedTermsAnchor').className = ""; }
	        if (document.getElementById('basicTerms') !== null) { document.getElementById('basicTerms').style.display = "block"; }
	        if (document.getElementById('basicTermsAnchor') !== null) { document.getElementById('basicTermsAnchor').className = "selected"; }
	    }
		else
		{
	        if (document.getElementById('advancedTerms') !== null) { document.getElementById('advancedTerms').style.display = "block"; }
	        if (document.getElementById('advancedTermsAnchor') !== null) { document.getElementById('advancedTermsAnchor').className = "selected"; }
	        if (document.getElementById('basicTerms') !== null) { document.getElementById('basicTerms').style.display = "none"; }
	        if (document.getElementById('basicTermsAnchor') !== null) { document.getElementById('basicTermsAnchor').className = ""; }
	    }
	}
	,searchBackButton:function()
	{   
	    //debugger;
		if(_backbuttonaction)
		{
	        var cookie_str = EkCookie.getCookie('searchcookie');

			if(cookie_str!==null && cookie_str!=='')
			{
	            EkSearchResult.parseContextData(cookie_str);
				if(document.getElementById('ecmsearchpage').value==EkCookie.getCookie('pageparams'))
				{
	                var autoTagHdnObj = $ektron("#ecmBasicAutoTagSearch");
	                var autoTagSearch = (autoTagHdnObj != null && autoTagHdnObj[0] != null && autoTagHdnObj[0].value == "1")
					if (document.getElementById('ecmBasicKeywords') !== null)
					{
	                    var basicText = EkSearchResult.getKeyValue('ecmBasicKeywords');
	                    var mode = EkSearchResult.getKeyValue('ecmsearchmode');
	                    if (mode != "advanced" && !autoTagSearch)
	                        document.getElementById('ecmBasicKeywords').value = basicText;
	                }
					if (autoTagSearch)
					{
	                    var dmy = "";
	                    // clear auto-search flag:
	                    $ektron("#ecmBasicAutoTagSearch").val("0")
	                    var autoTagName = $ektron("#ecmBasicKeywords");
	                    if (autoTagName && autoTagName.val().length) {
	                        var matchStr = "ecmBasicKeywords=";
	                        var cParts = cookie_str.split(matchStr);
	                        if (cParts.length > 0) {
	                            var idx = cParts[1].indexOf("&");
	                            if (idx > 0) {
	                                var cookie_str = cParts[0] + matchStr + autoTagName.val() + cParts[1].substr(idx)
	                            }
	                        }
	                    }
	                }
					else
					{
	                    //33563 - Taxonomy category filtering becomes blank on back button in firefox 
	                    if (!browseris.ie5up)
	                        redrawTreeOnFF = true;
	                    __LoadSearchResult(cookie_str, EkCookie.getCookie('contextparams'));
	                }
	            }
				else
				{
	                EkSearch.clrCookie();
	            }
	        }
			else
			{
	            //if postback type search being used, and hit back button on results, use the postback cookie.
	            //debugger;
	            var postbackSearch = document.getElementById('ecmpostbacksearch');
			    if ((postbackSearch !== null) && (postbackSearch.value == "true"))
			    {
	                var postbackcookie = EkCookie.getCookie('postbackcookie');
		            if(postbackcookie!==null && postbackcookie!=='')
	                {
	                    EkSearchResult.parseContextData(postbackcookie);
		                if ((EkSearchResult.getKeyValue("postbacktext") !== null) && (EkSearchResult.getKeyValue("postbacktext").length > 0))
		                {   
	                        var stripSpaceText = EkSearchResult.getKeyValue("postbacktext");
	                        if (stripSpaceText.indexOf('< ') >= 0)
	                            stripSpaceText = stripSpaceText.replace("< ", "<");
	                        //set current search text
	                        if (document.getElementById('ecmBasicKeywords') !== null)
	                            document.getElementById('ecmBasicKeywords').value = stripSpaceText;

	                        //get category and search type only if UI is enabled                                
	                        var postbackUI = document.getElementById('ecmsearchui');
		                    if ((postbackUI !== null) && (postbackUI.value == "true"))
                            {
	                            //set current search type
	                            var searchType = document.getElementById('ecmSearchForTypes');
                                if ((document.forms[0].searchScope != null) && (document.forms[0].searchScope.options != null))
                                {
	                                var selType = document.forms[0].searchScope.options[document.forms[0].searchScope.selectedIndex].value;
	                                searchType.value = selType;
	                            }
                                if (EkSearchResult.getKeyValue("searchcategory")!==null && EkSearchResult.getKeyValue("searchcategory")!=='' && EkSearchResult.getKeyValue("searchcategory").length > 0)
	                            {
	                                readCategoryFromTree = false;
	                            }
	                        }

	                        var currentpage = 1;
                            if (EkSearchResult.getKeyValue("postbackpagenum")!==null && EkSearchResult.getKeyValue("postbackpagenum")!=='' && EkSearchResult.getKeyValue("postbackpagenum").length > 0 && EkSearchResult.getKeyValue("postbackpagenum") > 1)
	                        {
	                            currentpage = EkSearchResult.getKeyValue("postbackpagenum");
	                        }

	                        var context = "";
                            if (EkSearchResult.getKeyValue("control") == null || EkSearchResult.getKeyValue("control") == "")
		                    {
	                            if (document.getElementById('ecmResultTagId') !== null)
	                                context = document.getElementById('ecmResultTagId').value;
	                        }
		                    else
		                    {
	                            context = EkSearchResult.getKeyValue("control");
	                        }
	                        //33563 - Taxonomy category filtering becomes blank on back button in firefox 
		                    if (!browseris.ie5up)
		                    {
	                            redrawTreeOnFF = true;
	                        }
	                        __LoadSearchResult(EkSearch.getArguements(), "control=" + context + "&__ecmcurrentpage=" + currentpage);
	                    }
	                }
	                else
	                {
	                    //if postback cookie is empty but if old search type is not "all", execute search for new search type
	                    if ((document.getElementById('ecmBasicKeywords') !== null) && (document.getElementById('ecmBasicKeywords').value.length > 0))
	                    {
	                        var searchType = document.getElementById('ecmSearchForTypes');
                            if ((document.forms[0].searchScope != null) && (document.forms[0].searchScope.options != null))
                            {
	                            var selType = document.forms[0].searchScope.options[document.forms[0].searchScope.selectedIndex].value;
                                if (selType != "all")
                                {
	                                searchType.value = selType;
                                    if (EkSearchResult.getKeyValue("control") == null || EkSearchResult.getKeyValue("control") == "")
		                            {
	                                    if (document.getElementById('ecmResultTagId') !== null)
	                                        context = document.getElementById('ecmResultTagId').value;
	                                }
		                            else
		                            {
	                                    context = EkSearchResult.getKeyValue("control");
	                                }

	                                __LoadSearchResult(EkSearch.getArguements(), "control=" + context + "&__ecmcurrentpage=1");
	                            }
	                        }
	                    }
	                }
	            }
	        }
	        // clear auto-search flag: (if cookie was empty, first time, it never got cleared
	        $ektron("#ecmBasicAutoTagSearch").val("0")
	    }
	}
	,serializeForm:function()
	{
	    var SearchText = document.getElementById('ecmBasicKeywords');
	    if (SearchText != null && SearchText.value.match(/<(\w[^<>]*?)>/gi))
	    {
	        SearchText.value = SearchText.value.replace(/<(\w[^<>]*?)>/gi, "< $1>");
	    }

	    var element = document.forms[0].elements;
	    var len = element.length;
	    var query_string = "";
	    this.AddFormField = function (name, value) 
        {
            if (query_string.length > 0) 
            {
	            query_string += "&";
	        }
	        query_string += encodeURIComponent(name) + "=" + encodeURIComponent(value);
	    };

	    //if backbutton in postback search, read from cookie (ecmSelTaxCategory is empty...)
        if ((readCategoryFromTree == null) || (readCategoryFromTree != false))
		{
	        //debugger;
	        //If Search button was clicked, assume new taxonomy search will be performed.
	        //So do not get any selected taxonomies from tree
	        //If pagelink is clicked get any selected taxonomies from tree
	        //If a category in tree is clicked get selected taxonomies from tree
	        //Clicking on search button/enter key/changing search type do not apply current filter to next search, but page links do
	        var refreshTaxTree = document.getElementById('ecmTaxonomySearch');
            if (((refreshTaxTree != null) && (refreshTaxTree.value == "taxonomysearch")) || (pageLinkClick = true))
            {
	            pageLinkClick = false;
	            //Set selected taxonomy categories from tree into ecmSelTaxCategory
	            if ($ektron("#taxonomyFilter").length > 0) {
	                var categoryText = '';
	                var taxnomyvalues = $ektron("div#divTreePane input.searchTaxonomyPath");
		            for(var count=0; count<taxnomyvalues.length; count++) 
		            {
		                if (taxnomyvalues[count].checked === true)
		                {
	                        if (categoryText.length <= 0)
	                            categoryText = taxnomyvalues[count].value;
	                        else
	                            categoryText += "," + taxnomyvalues[count].value;
	                    }
	                }

	                var selTax = document.getElementById('ecmSelTaxCategory');
	                if (selTax != null)
	                    selTax.value = categoryText;
	            }
                else
                {
	                var selTax = document.getElementById('ecmSelTaxCategory');
                    if (selTax !== null)
                    {
	                    //if postback search and category was set first time
	                    //clear the hidden field if taxonomy filtering UI is being displayed
	                    //do not clear if taxonomy filtering UI is NOT being displayed
	                    var postbackSearch = document.getElementById('ecmpostbacksearch');
	                    var showcategoryui = document.getElementById('ecmShowTaxCategory');
                        if ((postbackSearch !== null) && (postbackSearch.value == "true"))
                        {
                            if (((showcategoryui !== null) && (showcategoryui.value.toLowerCase() == "true")))
                            {
	                            //postback search and ShowCategories is true
	                            selTax.value = "";
	                        }
	                    }
	                    else
	                        selTax.value = "";
	                }
	            }
	        }
            else
            {
	            var selTax = document.getElementById('ecmSelTaxCategory');
	            if (selTax != null)
	                selTax.value = "";
	        }
	        //If taxonomy name contains &, encode to #eksepamp# before inserting in cookie
	        var selTax = document.getElementById('ecmSelTaxCategory');
	        EkCookie.setCookie('postbackcookie', "searchcategory=" + selTax.value.replace(/&/gi, "#eksepamp#"));
	    }
        else
        {
	        //backbutton in Postback search if UI is enabled
            if (readCategoryFromTree == false)
            {
	            var selTax = document.getElementById('ecmSelTaxCategory');
	            var cookie_str = EkCookie.getCookie('postbackcookie');
			    if(cookie_str!==null && cookie_str!=='')
			    {
	                EkSearchResult.parseContextData(cookie_str);
	                //If taxonomy name contained an &, decode #eksepamp# to get &
	                selTax.value = EkSearchResult.getKeyValue('searchcategory').replace(/#eksepamp#/gi, "&");
	            }
	            else
	                selTax.value = "";
	            readCategoryFromTree = true;
	        }
	    }

		for(var i=0;i<len;i++)
		{
	        var item = element[i];
			if(typeof(item.name)!='undefined')
			{
				if((item.name.indexOf('ecm')!=-1)||(item.name.indexOf('selLang')!=-1)||(item.name.indexOf('EVENTTARGET')!=-1)||(item.name.indexOf('EVENTARGUMENT')!=-1))
				{
					try
					{
						switch(item.type)
						{
	                        case 'text':
	                        case 'password':
	                        case 'hidden':
	                        case 'textarea':
	                            this.AddFormField(item.name, item.value);
	                            break;
	                        case 'select-one':
								if(item.selectedIndex>=0)
								{
	                                this.AddFormField(item.name, item.options[item.selectedIndex].value);
	                            }
	                            break;
	                        case 'select-multiple':
								for(var j=0;j<item.options.length;j++)
								{
									if(item.options[j].selected)
									{
	                                    this.AddFormField(item.name, item.options[j].value);
	                                }
	                            }
	                            break;
	                        case 'checkbox':
	                        case 'radio':
								if(item.checked)
								{
	                                this.AddFormField(item.name, item.value);
	                            }
	                            break;
	                    }
	                }
	                catch (e)
					{ }
	            }
	        }
	    }
	    EkSearch.clrCookie();
	    EkCookie.setCookie('searchcookie', query_string);
	    return query_string;
	}
	,ReadCategoryFromTree:function()
	{
	    //a page link was clicked
	    pageLinkClick = true;
	}
	,getArguements:function()
	{
	    return this.serializeForm();
	}
	,validateKey:function(item,control)
	{
		if(item.keyCode==13)
		{
	        //45452 - Validate UI fields in Advanced Search UI
		    if (ValidateAdvancedUIFields())
		    {
	            //hitting search button or enter key implies new search, clear old tree/selections
	            EkSearch.LogSearchAnalytics();
	            ClearTaxonomySearch();
	            __LoadSearchResult(EkSearch.getArguements(), 'control=' + control + '&__ecmcurrentpage=1');
	        }
	        return false;
	    }
	}
	,getKeyWords:function()
	{
	    var val = "";
		try
		{
			if(document.getElementById("ecmBasicKeywords")!==null)
			{
	            val = document.getElementById("ecmBasicKeywords").value;
	        }
	    }
	    catch (e)
		{ }
	    return val;
	}
	,setKeyWords:function(val)
	{
		try
		{
			if(document.getElementById("ecmBasicKeywords")!==null && val!=="")
			{
	            document.getElementById("ecmBasicKeywords").value = val;
	        }
	    }
	    catch (e)
		{ }
	}
	,addLoadEvent:function(func)
	{
	    var _currentloadevent = window.onload;
		if(typeof window.onload!='function')
		{
	        window.onload = func;
	    }
		else
		{
			window.onload=function()
			{
				if(_currentloadevent)
				{
	                _currentloadevent();
	            }
	            func();
	        };
	    }
	}
	,togDisp:function(e,name)
	{
	    stopB(e);
	    var elems = document.getElementsByName(name);
		for(var i=0;i<elems.length;i++)
		{
	        var obj = elems[i];
	        var dp = "";
			if(obj.style.display==="")
			{
	            dp = "none";
	        }
	        obj.style.display = dp;
	    }
	    return false;
	}
	,stopB:function(e)
	{
	    if (!e) { e = window.event; }
	    e.cancelBubble = true;
	}
	,checkDateFormat : function(selectId)
	{
	    var elem = document.getElementById(selectId);
	    var currentSel = elem.options[elem.selectedIndex].value;
	    var parentElem = elem.parentNode;
		if(typeof(parentElem) != 'undefined')
		{
			for (var dEl=0; dEl<parentElem.childNodes.length; dEl++)
			{
				if (parentElem.childNodes[dEl].nodeName=="INPUT")
				{
					if(currentSel ==  "@datecreatedB" || currentSel ==  "@datecreatedA" || currentSel ==  "@datemodifiedB" || currentSel ==  "@datemodifiedA")
					{
	                    //32298 - display date format regardless of what exists in the text box
	                    parentElem.childNodes[dEl].value = "YYYY/MM/DD";
	                } else {
	                    if (parentElem.childNodes[dEl].value !== "") { parentElem.childNodes[dEl].value = ""; }
	                }
	            }
	        }
	    }
	}
	,clearMeta : function(inputId)
	{
	    if (document.getElementById(inputId) === null) { return; }
	    var firstInput = document.getElementById(inputId);
	    firstInput.value = '';
	}
	,removeMeta : function(metaId)
	{
	    var parentElem = document.getElementById('parentForFilters');
	    var metaElem;
		if(typeof(parentElem) != 'undefined')
		{
	        var divCount = 0;
			for (var i=0; i<parentElem.childNodes.length; i++)
			{
				if (parentElem.childNodes[i].nodeName=="DIV")
				{
	                ++divCount;
	            }
	        }
			if(divCount == 1)
			{
	            metaElem = document.getElementById(metaId);
				if(typeof(metaElem) != 'undefined')
				{
					for (var dEl=0; dEl<metaElem.childNodes.length; dEl++)
					{
						if (metaElem.childNodes[dEl].nodeName=="INPUT")
						{
	                        metaElem.childNodes[dEl].value = "";
	                    }
	                }
	            }
	        } else {
	            metaElem = document.getElementById(metaId);
				if(typeof(metaElem) != 'undefined')
				{
	                parentElem = metaElem.parentNode;
					if(typeof(parentElem) != 'undefined')
					{
	                    parentElem.removeChild(metaElem);
	                }
	            }
	        }
	    }
	}
	,addMeta:function(init)
	{
	    if (document.getElementById('parentForFilters') === null) { return; }
	    var parentElem = document.getElementById('parentForFilters');
		if(typeof(parentElem)!='undefined')
		{
			if (document.getElementById('parentForFiltersError') !== null)
			{
	            parentElem.removeChild(parentElem.firstChild);
	        }

	        var eLI = document.createElement("li");
	        var randomnumber = Math.floor(Math.random() * 1000);
	        eLI.setAttribute("id", "li_Meta" + randomnumber);
	        var metaLI = '<select name="ecm_MT{0}" id="ecm_M{0}" class="addRemoveMeta" onchange="EkSearch.checkDateFormat(\'ecm_M{0}\');">';
	        metaLI = metaLI + '<option id="datecreatedB" value="@datecreatedB">Created Before</option>';
	        metaLI = metaLI + '<option id="datecreatedA" value="@datecreatedA">Created After</option>';
	        metaLI = metaLI + '<option id="datemodifiedB" value="@datemodifiedB">Modified Before</option>';
	        metaLI = metaLI + '<option id="datemodifiedA" value="@datemodifiedA">Modified After</option>';
	        metaLI = metaLI + '<option selected="selected" id="author" value="@docauthor">Author</option>';
	        metaLI = metaLI + '<option id="cmssize" value="@cmssize">File Size</option>';
	        metaLI = metaLI + '</select>';
	        metaLI = metaLI + '<input name="ecm_MV{0}" id="ecm_MV{0}" type="text" value="" />';

	        var ulChildNodes = parentElem.childNodes;
	        if (ulChildNodes.length > 0) 
            {
	            metaLI = metaLI + '<a href="#" title="Remove" class="removeMeta" onclick="EkSearch.removeMeta(\'li_Meta{0}\');return false">Remove</a>';
	        }
	        eLI.innerHTML = metaLI.format(randomnumber, EkSearch.imagepath);
	        parentElem.appendChild(eLI);
	    }
	}
	,changeSearchType:function(controlArgs)
	{
	    //debugger;
	    var searchType = document.getElementById('ecmSearchForTypes');
	    if ((document.forms[0].searchScope != null) && (document.forms[0].searchScope.options != null))
	        searchType.value = document.forms[0].searchScope.options[document.forms[0].searchScope.selectedIndex].value;
	    //Advanced Search header - change text based on search type
	    var advSearchLegend = document.getElementById('advancedTermsAnchor');
	    var advSearchdefault = document.getElementById('ecmDefaultAdvText');
		if ((advSearchLegend != null) && (advSearchdefault != null))
		{ 
		    if (searchType.value != 'all')
		    {
	            if ((document.forms[0].searchScope != null) && (document.forms[0].searchScope.options != null))
	                advSearchLegend.innerHTML = advSearchdefault.value + " - " + document.forms[0].searchScope.options[document.forms[0].searchScope.selectedIndex].text;
	        }
	        else
	            advSearchLegend.innerHTML = advSearchdefault.value;
	    }
	    //Redraw tree after postback, start fresh with taxonomy search.
	    ClearTaxonomySearch();
	    __LoadSearchResult(EkSearch.getArguements(), "control=" + controlArgs + "&__ecmcurrentpage=1");
	}

	,StrReplace:function(str,findstr,replacestr)
	{
	    var ret = str;
	    if (ret != "" && findstr != "") {
	        var index = ret.indexOf(findstr);
            while(index>=0)
            {
	            ret = ret.replace(findstr, replacestr);
	            index = ret.indexOf(findstr);
	        }
	    }
	    return (ret);
	}

	,GetTreePath:function(id)
	{
	    var path = '';
	    var parentid = id;
	    path = '\\' + catTree.getItemText(id);
	    while (parentid != 0) {
	        parentid = catTree.getParentId(parentid);
	        path = '\\' + catTree.getItemText(parentid) + path;
	    }
	    path = EkSearch.StrReplace(path, '\\\\', '\\');
	    path = EkSearch.StrReplace(path, '&amp;', '&');
	    return path;

	}

	,DoTaxonomySearch:function()
	{
	    //alert("TaxonomySearch");
		try
		{
	        //debugger;
			if(EkSearchResult.getKeyValue("control") == null && EkSearchResult.getKeyValue("control") != "")
			{
	            var cookie_str = EkCookie.getCookie('searchcookie');
	            if (cookie_str !== null && cookie_str !== '')
	                EkSearchResult.parseContextData(cookie_str);
	        }
	        readCategoryFromTree = true;
	        //EkCookie.setCookie('postbackcookie',"searchcategory=");
	        //EkCookie.setCookie('searchcategory',"");
	        //set hidden field to ensure taxonomy tree is not redrawn after search.
	        var refreshTaxTree = document.getElementById('ecmTaxonomySearch');
	        if (refreshTaxTree != null)
	            refreshTaxTree.value = "taxonomysearch";
	        __LoadSearchResult(EkSearch.getArguements(), "control=" + EkSearchResult.getKeyValue("control") + "&__ecmcurrentpage=1");

	    }
   		catch(e)
		{
	        alert(e.description);
	    }
	}
	,SetPostBackCookie:function(searchtext, pagenum)
	{
	    if (searchtext.match(/<(\w[^<>]*?)>/gi))
	    {
	        searchtext = searchtext.replace(/<(\w[^<>]*?)>/gi, "< $1>");
	    }

	    var postbackSearch = document.getElementById('ecmpostbacksearch');
	    if ((postbackSearch !== null) && (postbackSearch.value == "true"))
	    {
	        EkSearch.UpdatePostBackCookie('postbacktext', searchtext);
	        EkSearch.UpdatePostBackCookie('postbackpagenum', pagenum);
	    }
	}
	,UpdatePostBackCookie:function(name, value)
	{
        if (EkCookie.getCookie('postbackcookie')!==null || EkCookie.getCookie('postbackcookie')!=='') 
        {
	        var SearchCookie = EkCookie.getCookie('postbackcookie')
	        var indexSrchText = SearchCookie.indexOf(name);
	        var arrSearchCookieNameValuePairs = "";
			if (indexSrchText>0)
			{
	            arrSearchCookieNameValuePairs = SearchCookie.split("&");
	            var newSearchCookieValue = "";
	            for (var index = 0; index < arrSearchCookieNameValuePairs.length; index++) {
	                var arrNameValuePair = arrSearchCookieNameValuePairs[index].split("=");
	                if (arrNameValuePair[0] == name) {
	                    arrSearchCookieNameValuePairs[index] = name + "=" + value;
	                }
	                if (newSearchCookieValue.length > 0) {
	                    newSearchCookieValue += "&" + arrSearchCookieNameValuePairs[index];
	                }
	                else {
	                    newSearchCookieValue += arrSearchCookieNameValuePairs[index];
	                }
	            }
	            EkCookie.setCookie("postbackcookie", newSearchCookieValue);
	        }
            else
            {
	            if (SearchCookie.length > 0)
	                SearchCookie += "&";
	            SearchCookie += name + "=" + value;
	            EkCookie.setCookie("postbackcookie", SearchCookie);
	        }
	    }
        else
        {
	        EkCookie.setCookie("postbackcookie", name + "=" + value);
	    }
	}
	,loadTaxonomy:function(categoryXml,context)
	{
		try
		{
	        var dvCat = document.getElementById('divCategory');
	        var dvTreePane = document.getElementById('divTreePane');
	        var refreshTaxTree = document.getElementById('ecmTaxonomySearch');

	        //if one of the taxonomies in tree was clicked, do not redraw the tree
	        if ((refreshTaxTree != null) && (refreshTaxTree.value == "taxonomysearch"))
	        {
	            $ektron('#EktronCategorySearchToggle').css('display', 'block');
	            if (!redrawTreeOnFF)
	            {
	                return;
	            }
	        }

	        redrawTreeOnFF = false;
	        if (context != null && context != "")
	            EkSearchResult.parseContextData(context);

	        if ((dvCat != null) && (categoryXml.length > 0))
	        {
	            dvTreePane.innerHTML = "";
	            categoryXml = categoryXml.replace(/#eksep#/gi, "\\");
	            categoryXml = categoryXml.replace(/#eksepquote#/gi, "\'");
	            dvCat.style.display = "block";
	            $ektron("#divTreePane").html(categoryXml);
	            $ektron("#taxonomyFilter").treeview({
	                collapsed: true
	            });

	            $ektron('#EktronCategorySearchToggle').css('display', 'block');
	        }
	        else
	        {
	            if ((dvCat != null) && (categoryXml.length == 0))
	                HideTaxonomySearchUI();
	        }
	    }
   		catch(e)
		{
	        alert(e.description);
	    }
	}

	,doAdvanced:function(type)
	{
	    EkSearch.clrCookie();
	    var searchType = document.getElementById('ecmsearchmode');
	    searchType.value = type;
	    EkSearch.showSearchOps(type);
	    if (document.getElementById('__ecmsearchresult') != null)
	        document.getElementById('__ecmsearchresult').innerHTML = "";
		try
		{
	        HideTaxonomySearchUI();
	        ClearTaxonomySearch();
	    }
	    catch (e)
        { }

	}
	,doBasic:function(type)
	{
	    EkSearch.clrCookie();
	    var searchType = document.getElementById('ecmsearchmode');
	    searchType.value = type;
	    EkSearch.showSearchOps(type);
	    if (document.getElementById('__ecmsearchresult') != null)
	        document.getElementById('__ecmsearchresult').innerHTML = "";
	}
	,clrCookie:function()
	{
	    EkCookie.deleteCookie('pageparams', '');
	    EkCookie.deleteCookie('searchcookie', '');
	    EkCookie.deleteCookie('contextparams', '');
	},
	LogSearchAnalytics:function()
	{
        //Fire Search Analytics on the backend
        var callSearchAnalytics = document.getElementById("ecmSearchAnalytics");
        if (callSearchAnalytics !== null) { callSearchAnalytics.value = "1"; }
    },
	ClearSearchAnalytics:function()
	{
	    try
	    {
            //Clear the Search Analytics form field
            var callSearchAnalytics = document.getElementById("ecmSearchAnalytics");
            if (callSearchAnalytics !== null) { callSearchAnalytics.value = "0"; }

            //Clear the Search Analytics cookie value
		    if (EkCookie.getCookie('searchcookie')!==null || EkCookie.getCookie('searchcookie')!=='') 
		    {
                var SearchCookie = EkCookie.getCookie('searchcookie');
                arrSearchCookieNameValuePairs = SearchCookie.split("&");
                var newSearchCookieValue = "";
                var arrNameValuePair = "";
                for (var index = 0; index < arrSearchCookieNameValuePairs.length; index++) {
                    arrNameValuePair = arrSearchCookieNameValuePairs[index].split("=");
                    if (arrNameValuePair[0] == "ecmSearchAnalytics") {
                        arrSearchCookieNameValuePairs[index] = "ecmSearchAnalytics=";
                    }
                    if (newSearchCookieValue.length > 0) {
                        newSearchCookieValue += "&" + arrSearchCookieNameValuePairs[index];
                    }
                    else {
                        newSearchCookieValue += arrSearchCookieNameValuePairs[index];
                    }
                }
                EkCookie.setCookie("searchcookie", newSearchCookieValue);
            }
        }
        catch(e)
	    {
        }
    },
    SetSearchBox:function(searchtext)
    {
        if (document.getElementById('ecmBasicKeywords') != null)
            document.getElementById('ecmBasicKeywords').value = $ektron.htmlDecode(searchtext);
    }
};
EkSearch.addLoadEvent(SetSearchTypesFromDropDown);
EkSearch.addLoadEvent(EkSearch.searchInit);
EkSearch.addLoadEvent(EkSearch.searchBackButton);

function EkMarkPostOnEnter(item,keys)
{
	if(item.keyCode==13)
	{
		MarkPostBack();
	}
	return true;
}
function ShowLoadingMessage(tagName){
	if(tagName)
	{
		var dvResult=document.getElementById(tagName);
		if (dvResult!==null) {dvResult.innerHTML="<p>Loading...</p>";}
	}
}

function EktronToggleCategorySearch()
{
    //debugger;
    var displayValue = $ektron('#EktronCategorySearch').css('display');
    $ektron('#EktronCategorySearch').toggle('fast');

    if ($ektron('#EktronCategorySearch').css('display') == "none"){ 
        $ektron('#EktronCategorySearchToggle').css('backgroundImage', 'url(' + EkSearch.imagepath + '/arrowClosed.gif)');
    }
    else { 
        //$ektron('#EktronCategorySearchToggle').css('backgroundImage', 'url(../../Workarea/images/application/arrowOpen.gif)');
        $ektron('#EktronCategorySearchToggle').css('backgroundImage', 'url(' + EkSearch.imagepath + '/arrowOpen.gif)');
    }
    
}

function SetSearchTypesFromDropDown() {
    if (EkCookie.getCookie('searchcookie')!==null || EkCookie.getCookie('searchcookie')!=='') {
        var SearchCookie = EkCookie.getCookie('searchcookie');
        arrSearchCookieNameValuePairs = SearchCookie.split("&");
        var newSearchCookieValue = "";
        var arrNameValuePair = "";
        for (var index=0;index<arrSearchCookieNameValuePairs.length;index++) {
            arrNameValuePair = arrSearchCookieNameValuePairs[index].split("=");
            if (arrNameValuePair[0] == "ecmSearchForTypes") {
            	var searchType=document.getElementById('ecmSearchForTypes');
           	    if ((document.forms[0].searchScope != null) && (document.forms[0].searchScope.options != null))
		            searchType.value=document.forms[0].searchScope.options[document.forms[0].searchScope.selectedIndex].value;
                arrSearchCookieNameValuePairs[index] = "ecmSearchForTypes=" + searchType.value;
            }
            if (newSearchCookieValue.length > 0) {
                newSearchCookieValue += "&" + arrSearchCookieNameValuePairs[index];
              }
            else {
                newSearchCookieValue += arrSearchCookieNameValuePairs[index];
            }
        }
        EkCookie.setCookie("searchcookie", newSearchCookieValue);
    }
}

function ClearTaxonomySearch()
{
    var selTax = document.getElementById('ecmSelTaxCategory');
    var postbackSearch = document.getElementById('ecmpostbacksearch');
    var showcategoryui = document.getElementById('ecmShowTaxCategory');
    if ((postbackSearch !== null) && (postbackSearch.value == "true"))
    {
        if (((showcategoryui !== null) && (showcategoryui.value.toLowerCase() != "true")) || (showcategoryui == null))
        {
            if (selTax.value.length > 0)
                return;
        }
    }
    
    //catTree = null;
    var dvTreePane = document.getElementById('divTreePane');
    if (dvTreePane !== null)
        dvTreePane.innerHTML = "";
    var refreshTaxTree = document.getElementById('ecmTaxonomySearch');
    if (refreshTaxTree != null)
	    refreshTaxTree.value = "";
    
    if (selTax != null)
        selTax.value = "";
    readCategoryFromTree = true;	    
    //EkCookie.setCookie('searchcategory',"");
    //EkCookie.setCookie('postbackcookie',"searchcategory=");
}

function HideTaxonomySearchUI()
{
    //catTree = null;
    var dvTreePane = document.getElementById('divTreePane');
    if (dvTreePane != null)
    	dvTreePane.innerHTML = "";
    try
    {
	    $ektron('#EktronCategorySearch').css('display','none');
	    $ektron('#EktronCategorySearchToggle').css('display','none');
	}
	catch(e)
	{}
}

function ValidateAdvancedUIFields()
{
    //45452 - Validate UI fields in Advanced Search UI
    try
    {
        var searchMode=document.getElementById('ecmsearchmode');
	    if ((searchMode !== null) && (searchMode.value == "advanced"))
	    {
	        if ($ektron("#ecm_eq").val().length > 0 && ($ektron("#ecm_q").val().length <= 0 && $ektron("#ecm_epq").val().length <= 0 && $ektron("#ecm_oq").val().length <= 0))
            {
                alert($ektron("#ecmUnaryNotUnSupported").val());
                $ektron("#ecm_eq").focus();
                return false;
            }  

            if ($ektron("#ecm_eq").val().length > 0 && ($ektron.trim($ektron("#ecm_eq").val()).indexOf(" ") >= 0))
            {
                alert($ektron("#ecmMultipleWithoutWordErrorMsg").val());
                $ektron("#ecm_eq").focus();
                return false;
            }     	        
	    }       
	}
	catch(e)
	{
	}
	return true;
}