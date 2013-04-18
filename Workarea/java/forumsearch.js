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

var EkForumSearchResult=
{
	keys:Array(),values:Array(),parseContextData:function(query)
	{
		var keys = [];
		var values = [];
		var pairs = query.split("&");
		//debugger;
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
	    //alert("displayResult");

        //debugger;        
    	EkForumSearchResult.parseContextData(context);
		EkCookie.setCookie('forumcontextparams',context);
		if (document.getElementById('ecmsearchpage')!==null)
		{
			EkCookie.setCookie('forumpageparams',document.getElementById('ecmsearchpage').value);
		}
		if (document.getElementById(EkForumSearchResult.getKeyValue("control"))!==null)
		{
		    //alert("displayResult - forumsearchresult = true");
		    EkCookie.setCookie('forumsearchresult','true');
		    document.getElementById(EkForumSearchResult.getKeyValue("control")).innerHTML='';
		    var dataelem = document.createElement('div');dataelem.innerHTML=result; document.getElementById(EkForumSearchResult.getKeyValue("control")).appendChild(dataelem);
		}
	}
	,displayError:function(message,context)
	{
		alert('An unhandled exception has occurred:\n'+message);
	}
};

var EkForumSearch=
{
	imagepath:'images',searchInit:function()
	{
	    //debugger;
		//if(EkCookie.getCookie('forumsearchcookie')!==null || EkCookie.getCookie('forumsearchcookie')!=='')
		//{
			//_backbuttonaction=true;
		//}
	}
	,searchBackButton:function()
	{
		var _backbuttonaction = false;
		if(EkCookie.getCookie('forumsearchcookie')!==null || EkCookie.getCookie('forumsearchcookie')!=='')
		{
			_backbuttonaction=true;
		}
	    var searchResultsMode =EkCookie.getCookie('forumsearchresult');
		if ((document.getElementById('txtSearchString') !== null) && (searchResultsMode!==null && searchResultsMode!=='' && searchResultsMode == 'true') && (_backbuttonaction))
		{
			var cookie_str=EkCookie.getCookie('forumsearchcookie');
			if(cookie_str!==null && cookie_str!=='')
			{
				EkForumSearchResult.parseContextData(cookie_str);
				if (document.getElementById('ecmsearchpage')!==null)
				{
				    if(document.getElementById('ecmsearchpage').value==EkCookie.getCookie('forumpageparams'))
				    {
		                EkCookie.setCookie('forumsearchresult','false');
					    var searchText = EkForumSearchResult.getKeyValue('txtSearchString');
					    document.getElementById('txtSearchString').value = searchText;
				    	//33563 - Taxonomy category filtering becomes blank on back button in firefox 
                        if (!browseris.ie5up)
					        redrawTreeOnFF = true;
					    __LoadForumSearchResult(cookie_str,EkCookie.getCookie('forumcontextparams'));
				    }
				}
			    else
			    {
				    EkForumSearch.clrCookie();
			    }
			}
		}
	}	
	,serializeForm:function()
	{
	    //alert("serializeForm");
		var element=document.forms[0].elements;
		var len=element.length;
		var query_string="";
		this.AddFormField=function(name,value)
			{
				if(query_string.length>0)
				{
					query_string+="&";
				}
				query_string+=encodeURIComponent(name)+"="+encodeURIComponent(value);
			};
		
		//debugger;
		//If Search button was clicked, assume new taxonomy search will be performed.
		//So do not get any selected taxonomies from tree
        var refreshTaxTree = document.getElementById('ecmTaxonomySearch');
        if ((refreshTaxTree != null) && (refreshTaxTree.value == "taxonomysearch"))
        {
		    //Set selected taxonomy categories from tree into ecmSelTaxCategory
            if ($ektron("#taxonomyFilter").length > 0 ) {
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
        
		for(var i=0;i<len;i++)
		{
			var item=element[i];
			if(typeof(item.name)!='undefined')
			{
                if(item.name!="__VIEWSTATE" && item.name!="__EVENTTARGET" && item.name!="__EVENTARGUMENT")
				{
					try
					{
						switch(item.type)
						{
							case'text':
							case'password':
							case'hidden':
							case'textarea':
								this.AddFormField(item.name,item.value);
								break;
							case'select-one':
								if(item.selectedIndex>=0)
								{
									this.AddFormField(item.name,item.options[item.selectedIndex].value);
								}
								break;
							case'select-multiple':
								for(var j=0;j<item.options.length;j++)
								{
									if(item.options[j].selected)
									{
										this.AddFormField(item.name,item.options[j].value);
									}
								}
								break;
							case'checkbox':
							case'radio':
								if(item.checked)
								{
									this.AddFormField(item.name,item.value);
								}
								break;
						}
					}
					catch(e)
					{}
				}
			}
		}
		EkForumSearch.clrCookie();
		EkCookie.setCookie('forumsearchcookie',query_string);
		return query_string;
	}
	,getArguements:function()
	{
		return this.serializeForm();
	}
	,validateKey:function(item,control)
	{
		if(item.keyCode==13)
		{
		    //hitting search button or enter key implies new search, clear old tree/selections
		    ClearTaxonomySearch();
			__LoadSearchResult(EkForumSearch.getArguements(),'control='+control+'&__ecmcurrentpage=1');
			//alert("after");
			return false;
		}
	}
	,getKeyWords:function()
	{
		var val="";
		try
		{
			if(document.getElementById("ecmBasicKeywords")!==null)
			{
				val=document.getElementById("ecmBasicKeywords").value;
			}
		}
		catch(e)
		{}
		return val;
	}
	,setKeyWords:function(val)
	{
		try
		{
			if(document.getElementById("ecmBasicKeywords")!==null && val!=="")
			{
				document.getElementById("ecmBasicKeywords").value=val;
			}
		}
		catch(e)
		{}
	}
	,addLoadEvent:function(func)
	{
	    //debugger;
        try
		{
		    var _currentloadevent =window.onload;
		    if(typeof window.onload!='function')
		    {
			    window.onload=func;
		    }
		    else
		    {
			    window.onload=function()
			    {
				    //if(_currentloadevent)
    		    	if ("undefined" != typeof _currentloadevent)
				    {
				        if (_currentloadevent)
					        _currentloadevent();
				    }
    		    	if ("undefined" != typeof func)
				    {
					    func();
				    }
			    };
		    }
		}
		catch(e)
		{}
	}
	,togDisp:function(e,name)
	{
		stopB(e);
		var elems=document.getElementsByName(name);
		for(var i=0;i<elems.length;i++)
		{
			var obj=elems[i];
			var dp="";
			if(obj.style.display==="")
			{
				dp="none";
			}
			obj.style.display=dp;
		}
		return false;
	}
	,stopB:function(e)
	{
		if(!e){e=window.event;}
		e.cancelBubble=true;
	}
	
	,StrReplace:function(str,findstr,replacestr)
	{
        var ret = str;
        if(ret!="" && findstr!=""){
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
        var path='';
        var parentid=id;
        path='\\'+catTree.getItemText(id);
        while(parentid!=0){
            parentid=catTree.getParentId(parentid);
            path='\\'+catTree.getItemText(parentid)+path;
        }
        path=EkForumSearch.StrReplace(path,'\\\\','\\');
        return path;

    }
    
	,clrCookie:function()
	{
		EkCookie.deleteCookie('pageparams','');
		EkCookie.deleteCookie('searchcookie','');
		EkCookie.deleteCookie('contextparams','');
	}
};
//EkForumSearch.addLoadEvent(EkForumSearch.searchInit);
EkForumSearch.addLoadEvent(EkForumSearch.searchBackButton);

//var catTree=null;
var redrawTreeOnFF = false;

var EkTaxonomySearch=
{
	imagepath:'images'
	
	,DoTaxonomySearch:function()
	{
		try
		{
			//set hidden field to ensure taxonomy tree is not redrawn after search.
            var refreshTaxTree = document.getElementById('ecmTaxonomySearch');
            if (refreshTaxTree != null)
	            refreshTaxTree.value = "taxonomysearch";
		   __LoadForumSearchResult(EkForumSearch.getArguements(),"control="+ "__forumsearchresult" + "&__ecmcurrentpage=1");
   	    }
   		catch(e)
		{
		    alert(e.description);
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
	            $ektron('#EktronCategorySearchToggle').css('display','block');
	            if (!redrawTreeOnFF)
	            {
                    return;
                }
	        }
	        
	        redrawTreeOnFF = false;
	        if ((dvCat != null) && (categoryXml.length > 0))
	        {
	            dvTreePane.innerHTML = "";
	            dvCat.style.display="block";
	            dvCat.style.display="block";
                $ektron("#divTreePane").html(categoryXml);
                $ektron("#taxonomyFilter").treeview({
		            collapsed: true
		        });          

                $ektron('#EktronCategorySearchToggle').css('display','block');
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
};

function EktronToggleCategorySearch()
{
    //debugger;
    var displayValue = $ektron('#EktronCategorySearch').css('display');
    $ektron('#EktronCategorySearch').toggle('fast');

    if ($ektron('#EktronCategorySearch').css('display') == "none"){ 
        $ektron('#EktronCategorySearchToggle').css('backgroundImage', 'url(' + EkTaxonomySearch.imagepath + '/arrowClosed.gif)');
    }
    else { 
        //$ektron('#EktronCategorySearchToggle').css('backgroundImage', 'url(../../Workarea/images/application/arrowOpen.gif)');
        $ektron('#EktronCategorySearchToggle').css('backgroundImage', 'url(' + EkTaxonomySearch.imagepath + '/arrowOpen.gif)');
    }
}

function ClearTaxonomySearch()
{
    //catTree = null;
    var dvTreePane = document.getElementById('divTreePane');
    if (dvTreePane !== null)
        dvTreePane.innerHTML = "";
    var refreshTaxTree = document.getElementById('ecmTaxonomySearch');
    if (refreshTaxTree != null)
	    refreshTaxTree.value = "";
    var selTax = document.getElementById('ecmSelTaxCategory');
    if (selTax != null)
        selTax.value = "";	    
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