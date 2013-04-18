function GadgetEscapeHTML(id)
{
    var tbData = $ektron("#"+id);
    var result;
    if(tbData.length) {  
    result = tbData.val();
        // less-thans (<)	    result = result.replace(/\</g,'&lt;');	    // greater-thans (>)	    result = result.replace(/\>/g,'&gt;');
	    tbData.val(result);
    }    
  return true;
}



if( typeof Sys != "undefined" ){
   Sys.Application.notifyScriptLoaded();
}