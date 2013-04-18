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

var EkProductSearchCookie=
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

var EkProductSearchResult=
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
	        EkProductSearch.showSearchOps(searchMode.value);
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

    	EkProductSearchResult.parseContextData(context);
		EkProductSearchCookie.setCookie('contextparams',context);
		if (document.getElementById('ecmsearchpage')!==null)
		{
			EkProductSearchCookie.setCookie('pageparams',document.getElementById('ecmsearchpage').value);
		}
		if (document.getElementById(EkProductSearchResult.getKeyValue("control"))!==null)
		{
		    //applying fix for 35291 to product search - Some browsers like safari do not execute inline scripts in callback result, use Ektron Javascript Library to fix this
            //document.getElementById(EkProductSearchResult.getKeyValue("control")).innerHTML='';
		    //var dataelem = document.createElement('div');dataelem.innerHTML=result; document.getElementById(EkProductSearchResult.getKeyValue("control")).appendChild(dataelem);
		    $ektron("#" + EkProductSearchResult.getKeyValue("control")).html(result);
		}
		try
		{
			if(document.getElementById("__EkAjaxHidden$"+EkProductSearchResult.getKeyValue("control"))!==null)
			{
			
				document.getElementById("__EkAjaxHidden$"+EkProductSearchResult.getKeyValue("control")).value=result;
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

var EkProductSearch =
{
    imagepath: 'images', 
    searchInit: function()
    {
        _backbuttonaction = false;
        var searchType = document.getElementById('ecmsearchmode');
        if (searchType !== null)
        {
            EkProductSearch.showSearchOps(searchType.value);
            EkProductSearch.addMeta('init');
            if (EkProductSearchCookie.getCookie('searchcookie') !== null || EkProductSearchCookie.getCookie('searchcookie') !== '')
            {
                _backbuttonaction = true;
            }
        }
    }, 
    showSearchOps: function(value)
	{
	    if (value == 'normal')
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
	}, 
	searchBackButton: function()
	{
	    if (_backbuttonaction)
	    {
	        var cookie_str = EkProductSearchCookie.getCookie('searchcookie');
	        if (cookie_str !== null && cookie_str !== '')
	        {
	            EkProductSearchResult.parseContextData(cookie_str);
	            if (document.getElementById('ecmsearchpage').value == EkProductSearchCookie.getCookie('pageparams'))
	            {
	                if (document.getElementById('ecmBasicKeywords') !== null)
	                {
	                    var basicText = EkProductSearchResult.getKeyValue('ecmBasicKeywords');
	                    var mode = EkProductSearchResult.getKeyValue('ecmsearchmode');
	                    if (mode != "advanced")
	                        document.getElementById('ecmBasicKeywords').value = basicText;
	                }
	                //33563 - Taxonomy category filtering becomes blank on back button in firefox 
	                if (!browseris.ie5up)
	                    redrawTreeOnFF = true;
	                __LoadSearchResult(cookie_str, EkProductSearchCookie.getCookie('contextparams'));
	            }
	            else
	            {
	                EkProductSearch.clrCookie();
	            }
	        }
	        else
	        {
	            //if postback type search being used, and hit back button on results, use the postback cookie.
	            //debugger;
	            var postbackSearch = document.getElementById('ecmpostbacksearch');
	            if ((postbackSearch !== null) && (postbackSearch.value == "true"))
	            {
	                var postbackcookie = EkProductSearchCookie.getCookie('postbackcookie');
	                if (postbackcookie !== null && postbackcookie !== '')
	                {
	                    EkProductSearchResult.parseContextData(postbackcookie);
	                    if ((EkProductSearchResult.getKeyValue("postbacktext") !== null) && (EkProductSearchResult.getKeyValue("postbacktext").length > 0))
	                    {
	                        //set current search text
	                        if (document.getElementById('ecmBasicKeywords') !== null)
	                            document.getElementById('ecmBasicKeywords').value = EkProductSearchResult.getKeyValue("postbacktext");

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
	                            if (EkProductSearchResult.getKeyValue("searchcategory") !== null && EkProductSearchResult.getKeyValue("searchcategory") !== '' && EkProductSearchResult.getKeyValue("searchcategory").length > 0)
	                            {
	                                readCategoryFromTree = false;
	                            }
	                        }

	                        var currentpage = 1;
	                        if (EkProductSearchResult.getKeyValue("postbackpagenum") !== null && EkProductSearchResult.getKeyValue("postbackpagenum") !== '' && EkProductSearchResult.getKeyValue("postbackpagenum").length > 0 && EkProductSearchResult.getKeyValue("postbackpagenum") > 1)
	                        {
	                            currentpage = EkProductSearchResult.getKeyValue("postbackpagenum");
	                        }

	                        var context = "";
	                        if (EkProductSearchResult.getKeyValue("control") == null || EkProductSearchResult.getKeyValue("control") == "")
	                        {
	                            if (document.getElementById('ecmResultTagId') !== null)
	                                context = document.getElementById('ecmResultTagId').value;
	                        }
	                        else
	                        {
	                            context = EkProductSearchResult.getKeyValue("control");
	                        }
	                        //33563 - Taxonomy category filtering becomes blank on back button in firefox 
	                        if (!browseris.ie5up)
	                        {
	                            redrawTreeOnFF = true;
	                        }
	                        __LoadSearchResult(EkProductSearch.getArguements(), "control=" + context + "&__ecmcurrentpage=" + currentpage);
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
	                                if (EkProductSearchResult.getKeyValue("control") == null || EkProductSearchResult.getKeyValue("control") == "")
	                                {
	                                    if (document.getElementById('ecmResultTagId') !== null)
	                                        context = document.getElementById('ecmResultTagId').value;
	                                }
	                                else
	                                {
	                                    context = EkProductSearchResult.getKeyValue("control");
	                                }

	                                __LoadSearchResult(EkProductSearch.getArguements(), "control=" + context + "&__ecmcurrentpage=1");
	                            }
	                        }
	                    }
	                }
	            }
	        }
	    }
	}, 
	serializeForm: function()
	{
	    var element = document.forms[0].elements;
	    var len = element.length;
	    var query_string = "";
	    this.AddFormField = function(name, value)
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
	            if ($ektron("#taxonomyFilter").length > 0)
	            {
	                var categoryText = '';
	                var taxnomyvalues = $ektron("div#divTreePane input.searchTaxonomyPath");
	                for (var count = 0; count < taxnomyvalues.length; count++)
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
	                if (selTax != null)
	                    selTax.value = "";
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
	        EkProductSearchCookie.setCookie('postbackcookie', "searchcategory=" + selTax.value.replace(/&/gi, "#eksepamp#"));
	    }
	    else
	    {
	        //backbutton in Postback search if UI is enabled
	        if (readCategoryFromTree == false)
	        {
	            var selTax = document.getElementById('ecmSelTaxCategory');
	            var cookie_str = EkProductSearchCookie.getCookie('postbackcookie');
	            if (cookie_str !== null && cookie_str !== '')
	            {
	                EkProductSearchResult.parseContextData(cookie_str);
	                //If taxonomy name contained an &, decode #eksepamp# to get &
	                selTax.value = EkProductSearchResult.getKeyValue('searchcategory').replace(/#eksepamp#/gi, "&");
	            }
	            else
	                selTax.value = "";
	            readCategoryFromTree = true;
	        }
	    }

	    for (var i = 0; i < len; i++)
	    {
	        var item = element[i];
	        if (typeof (item.name) != 'undefined')
	        {
	            if ((item.name.indexOf('ecm') != -1) || (item.name.indexOf('selLang') != -1) || (item.name.indexOf('EVENTTARGET') != -1) || (item.name.indexOf('EVENTARGUMENT') != -1))
	            {
	                try
	                {
	                    switch (item.type)
	                    {
	                        case 'text':
	                        case 'password':
	                        case 'hidden':
	                        case 'textarea':
	                            this.AddFormField(item.name, item.value);
	                            break;
	                        case 'select-one':
	                            if (item.selectedIndex >= 0)
	                            {
	                                this.AddFormField(item.name, item.options[item.selectedIndex].value);
	                            }
	                            break;
	                        case 'select-multiple':
	                            for (var j = 0; j < item.options.length; j++)
	                            {
	                                if (item.options[j].selected)
	                                {
	                                    this.AddFormField(item.name, item.options[j].value);
	                                }
	                            }
	                            break;
	                        case 'checkbox':
	                        case 'radio':
	                            if (item.checked)
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
	    EkProductSearch.clrCookie();
	    EkProductSearchCookie.setCookie('searchcookie', query_string);
	    return query_string;
	}
	, ReadCategoryFromTree: function()
	{
	    //a page link was clicked
	    pageLinkClick = true;
	}
	, getArguements: function()
	{
	    return this.serializeForm();
	}
	, validateKey: function(item, control)
	{
	    if (item.keyCode == 13)
	    {
	        //hitting search button or enter key implies new search, clear old tree/selections
	        ClearTaxonomySearch();
	        __LoadSearchResult(EkProductSearch.getArguements(), 'control=' + control + '&__ecmcurrentpage=1');
	        return false;
	    }
	}
	, getKeyWords: function()
	{
	    var val = "";
	    try
	    {
	        if (document.getElementById("ecmBasicKeywords") !== null)
	        {
	            val = document.getElementById("ecmBasicKeywords").value;
	        }
	    }
	    catch (e)
		{ }
	    return val;
	}
	, setKeyWords: function(val)
	{
	    try
	    {
	        if (document.getElementById("ecmBasicKeywords") !== null && val !== "")
	        {
	            document.getElementById("ecmBasicKeywords").value = val;
	        }
	    }
	    catch (e)
		{ }
	}
	, addLoadEvent: function(func)
	{
	    var _currentloadevent = window.onload;
	    if (typeof window.onload != 'function')
	    {
	        window.onload = func;
	    }
	    else
	    {
	        window.onload = function()
	        {
	            if (_currentloadevent)
	            {
	                _currentloadevent();
	            }
	            func();
	        };
	    }
	}
	, togDisp: function(e, name)
	{
	    stopB(e);
	    var elems = document.getElementsByName(name);
	    for (var i = 0; i < elems.length; i++)
	    {
	        var obj = elems[i];
	        var dp = "";
	        if (obj.style.display === "")
	        {
	            dp = "none";
	        }
	        obj.style.display = dp;
	    }
	    return false;
	}
	, stopB: function(e)
	{
	    if (!e) { e = window.event; }
	    e.cancelBubble = true;
	}
	, checkDateFormat: function(selectId)
	{
	    var elem = document.getElementById(selectId);
	    var currentSel = elem.options[elem.selectedIndex].value;
	    var parentElem = elem.parentNode;
	    if (typeof (parentElem) != 'undefined')
	    {
	        for (var dEl = 0; dEl < parentElem.childNodes.length; dEl++)
	        {
	            if (parentElem.childNodes[dEl].nodeName == "INPUT")
	            {
	                if (currentSel == "@datecreatedB" || currentSel == "@datecreatedA" || currentSel == "@datemodifiedB" || currentSel == "@datemodifiedA")
	                {
	                    //32298 - display date format regardless of what exists in the text box
	                    parentElem.childNodes[dEl].value = "YYYY/MM/DD";
	                } else
	                {
	                    if (parentElem.childNodes[dEl].value !== "") { parentElem.childNodes[dEl].value = ""; }
	                }
	            }
	        }
	    }
	}
	, clearMeta: function(inputId)
	{
	    if (document.getElementById(inputId) === null) { return; }
	    var firstInput = document.getElementById(inputId);
	    firstInput.value = '';
	}
	, removeMeta: function(metaId)
	{
	    var parentElem = document.getElementById('parentForFilters');
	    var metaElem;
	    if (typeof (parentElem) != 'undefined')
	    {
	        var divCount = 0;
	        for (var i = 0; i < parentElem.childNodes.length; i++)
	        {
	            if (parentElem.childNodes[i].nodeName == "DIV")
	            {
	                ++divCount;
	            }
	        }
	        if (divCount == 1)
	        {
	            metaElem = document.getElementById(metaId);
	            if (typeof (metaElem) != 'undefined')
	            {
	                for (var dEl = 0; dEl < metaElem.childNodes.length; dEl++)
	                {
	                    if (metaElem.childNodes[dEl].nodeName == "INPUT")
	                    {
	                        metaElem.childNodes[dEl].value = "";
	                    }
	                }
	            }
	        } else
	        {
	            metaElem = document.getElementById(metaId);
	            if (typeof (metaElem) != 'undefined')
	            {
	                parentElem = metaElem.parentNode;
	                if (typeof (parentElem) != 'undefined')
	                {
	                    parentElem.removeChild(metaElem);
	                }
	            }
	        }
	    }
	}
	, addMeta: function(init)
	{
	    if (document.getElementById('parentForFilters') === null) { return; }
	    var parentElem = document.getElementById('parentForFilters');
	    if (typeof (parentElem) != 'undefined')
	    {
	        if (document.getElementById('parentForFiltersError') !== null)
	        {
	            parentElem.removeChild(parentElem.firstChild);
	        }

	        var eLI = document.createElement("li");
	        var randomnumber = Math.floor(Math.random() * 1000);
	        eLI.setAttribute("id", "li_Meta" + randomnumber);
	        var metaLI = '<select name="ecm_MT{0}" id="ecm_M{0}" class="addRemoveMeta" onchange="EkProductSearch.checkDateFormat(\'ecm_M{0}\');">';
	        metaLI += EkProductSearch.getProductControlAdvancedFilterOptionsMarkup();
	        metaLI = metaLI + '</select>';
	        metaLI = metaLI + '<input name="ecm_MV{0}" id="ecm_MV{0}" type="text" value="" />';

	        var ulChildNodes = parentElem.childNodes;
	        if (ulChildNodes.length > 0)
	        {
	            metaLI = metaLI + '<a href="#" title="Remove" class="removeMeta" onclick="EkProductSearch.removeMeta(\'li_Meta{0}\');return false">' + EkProductSearch.GetLocalizedString('remove') + '</a>';
	        }

	        eLI.innerHTML = metaLI.format(randomnumber, EkProductSearch.imagepath);
	        parentElem.appendChild(eLI);
	    }
	}
	, getProductControlAdvancedFilterOptionsMarkup: function()
	{
	    var result = '<option class="ecmSearch_productControls_advancedFilterOptions" value="@productpricebelow">' + EkProductSearch.GetLocalizedString('priceBelow') + '</option>';
	    result += '<option class="ecmSearch_productControls_advancedFilterOptions" value="@productpriceabove">' + EkProductSearch.GetLocalizedString('priceAbove') + '</option>';
	    result += '<option class="ecmSearch_productControls_advancedFilterOptions" value="@productsku">' + EkProductSearch.GetLocalizedString('productSku') + '</option>';
	    return (result);
	}
	, testSearchType: function(testType)
	{
	    var searchType = document.getElementById('ecmSearchForTypes');
	    if (searchType && (document.forms[0].searchScope != null) && (document.forms[0].searchScope.options != null))
	    {
	        searchType.value = document.forms[0].searchScope.options[document.forms[0].searchScope.selectedIndex].value;
	        return ("undefined" != typeof searchType.value && testType == searchType.value);
	    }
	    else
	        return (false);
	}
	, changeSearchType: function(controlArgs)
	{
	    //Advanced Search header - change text based on search type
	    var advSearchLegend = document.getElementById('advancedTermsAnchor');
	    var advSearchdefault = document.getElementById('ecmDefaultAdvText');
	    if ((advSearchLegend != null) && (advSearchdefault != null))
	    {
	        if (!EkProductSearch.testSearchType('all'))
	        {
	            if ((document.forms[0].searchScope != null) && (document.forms[0].searchScope.options != null))
	                advSearchLegend.innerHTML = advSearchdefault.value + " - " + document.forms[0].searchScope.options[document.forms[0].searchScope.selectedIndex].text;
	        }
	        else
	            advSearchLegend.innerHTML = advSearchdefault.value;
	    }
	    //Redraw tree after postback, start fresh with taxonomy search.
	    ClearTaxonomySearch();

	    // show or hide the product-search controls, as appropriate:
	    //		if (EkProductSearch.testSearchType("products")){
	    $ektron(".ecmSearch_productControls").show();
	    if (0 == $ektron(".addRemoveMeta .ecmSearch_productControls_advancedFilterOptions").length)
	    {
	        $ektron(".addRemoveMeta").append(EkProductSearch.getProductControlAdvancedFilterOptionsMarkup());
	    }
	    //		}
	    //		else{
	    //		    $ektron(".ecmSearch_productControls").hide();
	    //		    $ektron(".addRemoveMeta .ecmSearch_productControls_advancedFilterOptions").remove();
	    //		}

	    __LoadSearchResult(EkProductSearch.getArguements(), "control=" + controlArgs + "&__ecmcurrentpage=1");
	}

	, StrReplace: function(str, findstr, replacestr)
	{
	    var ret = str;
	    if (ret != "" && findstr != "")
	    {
	        var index = ret.indexOf(findstr);
	        while (index >= 0)
	        {
	            ret = ret.replace(findstr, replacestr);
	            index = ret.indexOf(findstr);
	        }
	    }
	    return (ret);
	}

	, GetTreePath: function(id)
	{
	    var path = '';
	    var parentid = id;
	    path = '\\' + catTree.getItemText(id);
	    while (parentid != 0)
	    {
	        parentid = catTree.getParentId(parentid);
	        path = '\\' + catTree.getItemText(parentid) + path;
	    }
	    path = EkProductSearch.StrReplace(path, '\\\\', '\\');
	    return path;

	}

	, DoTaxonomySearch: function()
	{
	    //alert("TaxonomySearch");
	    try
	    {
	        //debugger;
	        if (EkProductSearchResult.getKeyValue("control") == null && EkProductSearchResult.getKeyValue("control") != "")
	        {
	            var cookie_str = EkProductSearchCookie.getCookie('searchcookie');
	            if (cookie_str !== null && cookie_str !== '')
	                EkProductSearchResult.parseContextData(cookie_str);
	        }
	        readCategoryFromTree = true;
	        //EkProductSearchCookie.setCookie('postbackcookie',"searchcategory=");
	        //EkProductSearchCookie.setCookie('searchcategory',"");
	        //set hidden field to ensure taxonomy tree is not redrawn after search.
	        var refreshTaxTree = document.getElementById('ecmTaxonomySearch');
	        if (refreshTaxTree != null)
	            refreshTaxTree.value = "taxonomysearch";
	        __LoadSearchResult(EkProductSearch.getArguements(), "control=" + EkProductSearchResult.getKeyValue("control") + "&__ecmcurrentpage=1");

	    }
	    catch (e)
	    {
	        alert(e.description);
	    }
	}
	, SetPostBackCookie: function(searchtext, pagenum)
	{
	    //debugger;
	    var postbackSearch = document.getElementById('ecmpostbacksearch');
	    if ((postbackSearch !== null) && (postbackSearch.value == "true"))
	    {
	        EkProductSearch.UpdatePostBackCookie('postbacktext', searchtext);
	        EkProductSearch.UpdatePostBackCookie('postbackpagenum', pagenum);
	    }
	}
	, UpdatePostBackCookie: function(name, value)
	{
	    if (EkProductSearchCookie.getCookie('postbackcookie') !== null || EkProductSearchCookie.getCookie('postbackcookie') !== '')
	    {
	        var SearchCookie = EkProductSearchCookie.getCookie('postbackcookie')
	        var indexSrchText = SearchCookie.indexOf(name);
	        var arrSearchCookieNameValuePairs = "";
	        if (indexSrchText > 0)
	        {
	            arrSearchCookieNameValuePairs = SearchCookie.split("&");
	            var newSearchCookieValue = "";
	            for (var index = 0; index < arrSearchCookieNameValuePairs.length; index++)
	            {
	                var arrNameValuePair = arrSearchCookieNameValuePairs[index].split("=");
	                if (arrNameValuePair[0] == name)
	                {
	                    arrSearchCookieNameValuePairs[index] = name + "=" + value;
	                }
	                if (newSearchCookieValue.length > 0)
	                {
	                    newSearchCookieValue += "&" + arrSearchCookieNameValuePairs[index];
	                }
	                else
	                {
	                    newSearchCookieValue += arrSearchCookieNameValuePairs[index];
	                }
	            }
	            EkProductSearchCookie.setCookie("postbackcookie", newSearchCookieValue);
	        }
	        else
	        {
	            if (SearchCookie.length > 0)
	                SearchCookie += "&";
	            SearchCookie += name + "=" + value;
	            EkProductSearchCookie.setCookie("postbackcookie", SearchCookie);
	        }
	    }
	    else
	    {
	        EkProductSearchCookie.setCookie("postbackcookie", name + "=" + value);
	    }
	}
	, loadTaxonomy: function(categoryXml, context)
	{
	    //debugger;
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
	            EkProductSearchResult.parseContextData(context);

	        if ((dvCat != null) && (categoryXml.length > 0))
	        {
	            dvTreePane.innerHTML = "";
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
	    catch (e)
	    {
	        alert(e.description);
	    }
	}

	, doAdvanced: function(type)
	{
	    EkProductSearch.clrCookie();
	    var searchType = document.getElementById('ecmsearchmode');
	    searchType.value = type;
	    EkProductSearch.showSearchOps(type);
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
	, doBasic: function(type)
	{
	    EkProductSearch.clrCookie();
	    var searchType = document.getElementById('ecmsearchmode');
	    searchType.value = type;
	    EkProductSearch.showSearchOps(type);
	    if (document.getElementById('__ecmsearchresult') != null)
	        document.getElementById('__ecmsearchresult').innerHTML = "";
	}
	, clrCookie: function()
	{
	    EkProductSearchCookie.deleteCookie('pageparams', '');
	    EkProductSearchCookie.deleteCookie('searchcookie', '');
	    EkProductSearchCookie.deleteCookie('contextparams', '');
	}
    , GetLocalizedString: function(key)
    {
        var result = $ektron("#EktronProductSearch_" + key).val();
        return (result != null ? result : key);
    }
};
EkProductSearch.addLoadEvent(SetSearchTypesFromDropDown);
EkProductSearch.addLoadEvent(EkProductSearch.searchInit);
EkProductSearch.addLoadEvent(EkProductSearch.searchBackButton);

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
		if (dvResult!==null) {dvResult.innerHTML='<p>' + EkProductSearch.GetLocalizedString('loadingElipsis') + '</p>';}
	}
}

function ShowLoadingMessageLocalized(message, tagName){
	if(tagName)
	{
		var dvResult=document.getElementById(tagName);
		if (dvResult!==null) {dvResult.innerHTML='<p>' + message + '</p>';}
	}
}

function EktronToggleCategorySearch()
{
    //debugger;
    var displayValue = $ektron('#EktronCategorySearch').css('display');
    $ektron('#EktronCategorySearch').toggle('fast');

    if ($ektron('#EktronCategorySearch').css('display') == "none"){ 
        $ektron('#EktronCategorySearchToggle').css('backgroundImage', 'url(' + EkProductSearch.imagepath + '/arrowClosed.gif)');
    }
    else { 
        //$ektron('#EktronCategorySearchToggle').css('backgroundImage', 'url(../../Workarea/images/application/arrowOpen.gif)');
        $ektron('#EktronCategorySearchToggle').css('backgroundImage', 'url(' + EkProductSearch.imagepath + '/arrowOpen.gif)');
    }
    
}

function SetSearchTypesFromDropDown() {
    if (EkProductSearchCookie.getCookie('searchcookie')!==null || EkProductSearchCookie.getCookie('searchcookie')!=='') {
        var SearchCookie = EkProductSearchCookie.getCookie('searchcookie')
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
        EkProductSearchCookie.setCookie("searchcookie", newSearchCookieValue);
    }
}

function ClearTaxonomySearch()
{
    var dvTreePane = document.getElementById('divTreePane');
    if (dvTreePane !== null)
        dvTreePane.innerHTML = "";
    var refreshTaxTree = document.getElementById('ecmTaxonomySearch');
    if (refreshTaxTree != null)
	    refreshTaxTree.value = "";
    var selTax = document.getElementById('ecmSelTaxCategory');
    if (selTax != null)
        selTax.value = "";
    readCategoryFromTree = true;	    
    //EkProductSearchCookie.setCookie('searchcategory',"");
    //EkProductSearchCookie.setCookie('postbackcookie',"searchcategory=");
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