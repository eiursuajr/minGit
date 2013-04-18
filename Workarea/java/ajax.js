//-------------------------------------------------------------------------------

// Copyright (C) Ektron Inc. All rights reserved.

//-------------------------------------------------------------------------------

// ajax.js

// Ektron Ajax Framework.

// Resource for Search, Poll, Listsummary, Collection, Metadatalist and Taxonomy.

function __LoadTaxonomyShowAll(v){

    var location=document.location.href;

    location=location.replace(/&__taxonomyshowall=[0-9]/ig,'');

    if(location.indexOf("?")==-1) location=location+"?";

    if(v.checked)

        document.location.href=location+"&__taxonomyshowall=1";

    else

        document.location.href=location+"&__taxonomyshowall=0";

}

var EBubble=new function(){

    this.bubblewidth="0";

    this.CurrentStyle=function(element,styleprop){

        var x;var y;

        try{

              x = document.getElementById(element);

              if (x.currentStyle)

                    y = x.currentStyle[styleprop];

              else if (window.getComputedStyle)

                    y = document.defaultView.getComputedStyle(x,null).getPropertyValue(styleprop);

          }

          catch(e){

              y='0';

          }

          return y;

    };

    this.CreateBubble=function(){

        try{

            if(document.getElementById('FloatBubble')==null){

                var divElt = document.createElement('div');

                divElt.setAttribute("id", "FloatBubble");

                document.getElementsByTagName("body")[0].appendChild(divElt);

                var e=document.getElementById('FloatBubble');

                var s='<TABLE id="EkBubbleTable" cellspacing="0" cellpadding="0">';

                s+='<TR>';

                s+='<TD id="EkTopL"></TD>';

                s+='<TD id="EkTopTile"><div id="EkTop">&#160;</div></TD>';

                s+='<TD id="EkTopR"></TD>';

                s+='</TR>';

                s+='<TR>';

                s+='<TD id="EkCenL"></TD>';

                s+='<TD><DIV id="EkCloseBubbleIcon"><IMG id="EkClose" src="'+__$BubbleCloseIconPath+'images/application/bubble/close.gif" onclick="EBubble.ShowHoverOverBubble(0,\'\',0,0,0);" /></DIV><DIV id="EkBubbleData"></DIV></TD>';

                s+='<TD id="EkCenR"></TD>';

                s+='</TR>';

                s+='<TR>';

                s+='<TD id="EkBotL"></TD>';

                s+='<TD id="EkBot"></TD>';

                s+='<TD id="EkBotR"></TD>';

                s+='</TR>';

                s+='</TABLE>';

                e.innerHTML=s;

            }

            if(document.getElementById('FloatBubbleT')==null){

                var divElt = document.createElement('div');

                divElt.setAttribute("id", "FloatBubbleT");

                document.getElementsByTagName("body")[0].appendChild(divElt);

                var e=document.getElementById('FloatBubbleT');

                var s='<TABLE id="EkBubbleTableT" cellspacing="0" cellpadding="0">';

                s+='<TR>';

                s+='<TD id="EkTopLT"></TD>';

                s+='<TD id="EkTopT"></TD>';

                s+='<TD id="EkTopRT"></TD>';

                s+='</TR>';

                s+='<TR>';

                s+='<TD id="EkCenLT"></TD>';

                s+='<TD><DIV id="EkCloseBubbleIconT"><IMG id=EkCloseT src="'+__$BubbleCloseIconPath+'images/application/bubble/close.gif" onclick="EBubble.ShowHoverOverBubble(0,\'\',0,0,0,0,0);"></DIV><DIV id="EkBubbleDataT"></DIV></TD>';

                s+='<TD id="EkCenRT"></TD>';

                s+='</TR>';

                s+='<TR>';

                s+='<TD id="EkBotLT"></TD>';

                s+='<TD id="EkBotTile"><div id="EkBotT">&#160;</div></TD>';

                s+='<TD id="EkBotRT"></TD>';

                s+='</TR>';

                s+='</TABLE>';

                e.innerHTML=s;

            }

        }

        catch(e)

        {

        }

    };

    this.ShowHoverOverBubble=function(flag,text,widthheight,X,Y,SX,SY){

        var width='';var height='';var suffix='';var wtop=0;

        if(widthheight!='0'){var str=widthheight.split(",");width=str[0];height=str[1]};

        if(height!='' && parseInt(SY)-parseInt(height)>=180)

            suffix='T';

        var element =document.getElementById("FloatBubble"+suffix);

        document.getElementById("FloatBubble").style.display = "none";

        document.getElementById("FloatBubbleT").style.display = "none";

        if(ECommon.Ie()){wtop=parseInt(document.documentElement.scrollTop);}else{wtop=0;}

        if(flag==1){

            if(EBubble.bubblewidth=="0"){EBubble.bubblewidth=EBubble.CurrentStyle("EkBubbleTable"+suffix,"width").replace(/\px/g,'');}

            document.getElementById("EkBubbleData"+suffix).innerHTML=text;

            if(!(parseInt(width)>40)){

               width=parseInt(EBubble.bubblewidth);

            }

            document.getElementById("EkBubbleTable"+suffix).style.width=width+"px";

            document.getElementById("EkBubbleData"+suffix).style.width=(parseInt(width)-40)+"px";

            if(height!=undefined && height!='' && parseInt(height)>100){document.getElementById("EkBubbleData"+suffix).style.maxHeight=height+"px";}

            else{document.getElementById("EkBubbleData"+suffix).style.height="auto";}

            element.style.display = "block";

            if(height!='' && parseInt(SY)-parseInt(height)>=180){

                element.style.top = wtop+parseInt(Y)-(parseInt(height)+120) + 'px';

                document.getElementById("EkBubbleData"+suffix).style.height=height+"px";

            }

            else{

                element.style.top = parseInt(parseInt(Y) +wtop)+ "px"; 

            }

            element.style.left = parseInt(X)-50+ "px";

        }

    };

    this.EventX=function(event){

          var xVal;if (ECommon.Ie()){xVal = event.x;}else{ xVal = event.pageX;}

          return(xVal+'&SX='+event.screenX)

    };

    this.EventY=function(event){

          var yVal;if (ECommon.Ie()){yVal = event.y;}else{yVal = event.pageY;}

          return(yVal+'&SY='+event.screenY)

      };

};

var ECommon=new function(){

    this.Decode=function(str){

        var ret=str;

        ret = ret.replace(/\&lt;/g,'<');

        ret = ret.replace(/\&gt;/g,'>');

        ret = ret.replace(/\&quot;/g,'"');

        ret = ret.replace(/\&#39;/g,'\'');

        return ret;

    };

    this.Ie=function(){

        var ua = window.navigator.userAgent.toLowerCase();

          return((ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1)));

    };

};

function IAjax(){

}

IAjax.Pop=function(url){

    window.open(url,'showcontent','toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=700,height=600');

};

IAjax.Digit=function(str) {

      if (str==null) return false;

      for (var i = 0; i < str.length; i++) {

            var d = str.charAt(i);

          if ((d < "0") || (d > "9"))

                  return false;

      }

      return true;

};

IAjax.ValidateKey=function(item,control){

    if (item.keyCode==13){

        __LoadSearchResult(IAjax.getArguments(),'control='+control+'&__ecmcurrentpage=1');

        return false;

    }

};

IAjax.ValidateTaxonomyKey=function(item,control){

    try{

        if (item.keyCode==13){

            __LoadTaxonomy(IAjax.getArguments(),'pagerequest=search&control='+control+'&__ecmcurrentpage=1');

            return false;

        }

    }catch(e){}

};

IAjax.DisplayError=function(message, context) {

    alert('An unhandled exception has occurred:\n' + message);
    
    try
    {
		Ektron.ready.ClientScriptCallbackEvent.errorCallback(message, context);
	}
	catch (ex) {}
};

IAjax.DisplayResult=function(result, context) {

    this.QueryString=function(key){

          var value = null;

          for (var i=0;i<this.QueryString.keys.length;i++)

          {

                if (this.QueryString.keys[i]==key)

                {

                      value = this.QueryString.values[i];

                      break;

                }

          }

          return value;

    };

    this.QueryString.keys = new Array();

    this.QueryString.values = new Array();

    this.ParseQueryString=function(args){

          var query = args;

          var pairs = query.split("&");

      

          for (var i=0;i<pairs.length;i++)

          {

                var pos = pairs[i].indexOf('=');

                if (pos >= 0)

                {

                      var argname = unescape(pairs[i].substring(0,pos));

                      var value = unescape(pairs[i].substring(pos+1));

                      this.QueryString.keys[this.QueryString.keys.length] = argname;

                      this.QueryString.values[this.QueryString.values.length] = value;          

                }

          }

    };

    this.ParseQueryString(context);

    var ____pagerequest=this.QueryString("pagerequest");

    if(____pagerequest=='bubble' || ____pagerequest=='showcontent')

    {

        switch (____pagerequest){

            case "bubble":

                EBubble.ShowHoverOverBubble(1,result,this.QueryString("bubblewidth"),this.QueryString("X"),this.QueryString("Y"),this.QueryString("SX"),this.QueryString("SY"));

                break;

            case "showcontent":

                var __showcontrol=this.QueryString("__ecmdiv");

                try{

                    document.getElementById(__showcontrol).innerHTML=result;

                }catch(e){};

                break;

        }

    }

    else

    {      

        var c1=this.QueryString("control");

        if (result == '')

        {
            result = document.getElementById(c1).innerHTML;

        } else {
            $ektron("#" + this.QueryString("control")).html(result);
        }

        try

        {

            if(document.getElementById("__EkAjaxHidden"+c1)!=null){

                document.getElementById("__EkAjaxHidden"+c1).value=result;

                if(ECommon.Ie()){

                    window.location.replace((window.location.href).replace(window.location.hash,"")+"#"+c1)

                    IAjax.CacheHtml(c1);

                }

            }

        }

        catch(e)

        {

        }

        

        try

        {

             var astViewFrm = window.frames[0] //document.frames[0];

             if(astViewFrm)

             {

                  if( astViewFrm.setPostInfo )

                  {

                    astViewFrm.setPostInfo();

                  }

                

                if( astViewFrm.SetFormDataInfo )

                {

                    astViewFrm.SetFormDataInfo();

                }

             }

         }

          catch(e)

        {

        }

    }
    
    try
    {
		Ektron.ready.ClientScriptCallbackEvent.eventCallback(result, context);
	}
	catch (ex) {}
};

IAjax.getArguments=IAjax.getArguements=function(){

    return(IAjax.serializeForm());

};

IAjax.serializeForm = function() {

    var element = document.forms[0].elements;

    var len = element.length;

    var query_string = "";

    this.AddFormField = 

    function(name,value) { 

        if (query_string.length>0) { 

        query_string += "&";

        }

        query_string += encodeURIComponent(name) + "=" + encodeURIComponent(value);

    };

        

    for (var i=0; i<len; i++) {

        var item = element[i];

        if(item.name!="__VIEWSTATE" && item.name!="__EVENTTARGET" && item.name!="__EVENTARGUMENT"){

            try{

                switch(item.type) {

                    case 'text': case 'password': case 'hidden': case 'textarea': 

                        this.AddFormField(item.name,item.value);

                        break;

                    case 'select-one':

                        if (item.selectedIndex>=0) {

                            this.AddFormField(item.name,item.options[item.selectedIndex].value);

                        }

                        break;

                    case 'select-multiple':

                        for (var j=0; j<item.options.length; j++) {

                            if (item.options[j].selected) {

                                this.AddFormField(item.name,item.options[j].value);

                            }

                        }

                        break;

                    case 'checkbox': case 'radio':

                        if (item.checked) {

                            this.AddFormField(item.name,item.value);

                        }

                        break;

                }

            }

            catch(e)

            {

            }

        }

    }

    return query_string;

};

IAjax.ResetValueOnBackButton=function(){

    IAjax.LoadHtml();

};

IAjax.GetKeyWords=function(){

    var val="";

    try{

        if(document.getElementById("ecmBasicKeywords")!=null){

        val=document.getElementById("ecmBasicKeywords").value;

        }

    }

    catch(e)

    {

    }

    return val;

};

IAjax.SetKeyWords=function(val){

    try{

        if(document.getElementById("ecmBasicKeywords")!=null && val!=""){

        document.getElementById("ecmBasicKeywords").value=val;

        }

    }

    catch(e)

    {

    }

};

IAjax.GetCookie=function(cookiename){

    var cookieval="-1";

    try{

        var cookiestring=""+document.cookie;

        var index1=cookiestring.indexOf(cookiename);

        if (!(index1==-1 || cookiename=="")){

            var index2=cookiestring.indexOf(';',index1);

            if (index2==-1) index2=cookiestring.length; 

            cookieval= unescape(cookiestring.substring(index1+cookiename.length+1,index2));

        }

    }

    catch(e)

    {

    }

    return cookieval;

};

IAjax.CacheHtml=function(control){

    if(ECommon.Ie()){

        try{

            var oPersist=document.getElementById("__EkAjaxHidden"+control);

            oPersist.setAttribute(control+"data",oPersist.value);

            oPersist.setAttribute(control+"date",(new Date()).getTime());

            oPersist.setAttribute(control+"text",IAjax.GetKeyWords());

            

            oPersist.save(control+"xml");

        }

        catch(e)

        {

        }

    }

    else{

        try{

            var element = document.forms[0].elements;

            var len = element.length;

            for (var i=0; i<len; i++) {

                var item = element[i];

                if(item.type=="hidden" && item.name.indexOf("__EkAjaxHidden")!=-1){

                    if(document.getElementById(item.name.substring(14,item.name.length)).innerHTML!=""){

                        document.getElementById(item.name).value=document.getElementById(item.name.substring(14,item.name.length)).innerHTML;

                    }

                }

            }

        }

        catch(e)

        {

        }

    }

};

IAjax.LoadHtml=function(){

    if(ECommon.Ie()){

        try{

            var element = document.forms[0].elements;

            var len = element.length;

            for (var i=0; i<len; i++) {

                var item = element[i];

                if(item.type=="hidden" && item.name.indexOf("__EkAjaxHidden")!=-1){

                var oPersist=document.getElementById(item.name);

                oPersist.load(item.name.substring(14,item.name.length)+"xml");

                oPersist.value=oPersist.getAttribute(item.name.substring(14,item.name.length)+"data");

                var minute=((new Date()).getTime()-oPersist.getAttribute(item.name.substring(14,item.name.length)+"date"))/60000;

                    if(window.location.hash!="" && minute<=20 && oPersist.value!="null" && oPersist.value!=null && oPersist.value!=""){

                        IAjax.SetKeyWords(oPersist.getAttribute(item.name.substring(14,item.name.length)+"text"));

                        document.getElementById(item.name.substring(14,item.name.length)).innerHTML=oPersist.value;

                    }

                }

            }

        }

        catch(e)

        {

        }

    }

    else{

        try{

            var element = document.forms[0].elements;

            var len = element.length;

            for (var i=0; i<len; i++) {

                var item = element[i];

                if(item.type=="hidden" && item.name.indexOf("__EkAjaxHidden")!=-1){

                    if(document.getElementById(item.name).value!=""){

                        document.getElementById(item.name.substring(14,item.name.length)).innerHTML=document.getElementById(item.name).value;

                    }

                }

            }

        }

        catch(e)

        {

        }

    }

};

IAjax.addLoadEvent=function (func) {

    var _currentloadevent = window.onload;

    if (typeof window.onload != 'function') {

        window.onload = func;

    }else{

        window.onload = function() {

          if (typeof _currentloadevent != 'undefined') {

            _currentloadevent();

          }

          if (typeof func != 'undefined')

            func();

        }

    }

};

IAjax.addLoadEvent(EBubble.CreateBubble);

IAjax.addLoadEvent(IAjax.ResetValueOnBackButton);

if(typeof(Sys) !== "undefined") Sys.Application.notifyScriptLoaded();