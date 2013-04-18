/*
 * ektronDnR - Minimalistic Drag'n'Resize for jQuery.
 *
 * Copyright (c) 2007 Brice Burgess <bhb@iceburg.net>, http://www.iceburg.net
 * Licensed under the MIT License:
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * $Version: 2007.08.19 +r2

 * Conversion Notes:
 * 1) references to the string 'jqDnR' were replaced with 'ektronDnR'.
 * 2) the 'jqResize' method was renamed 'resize'.
 * 3) the 'jqDrag' method was renamed 'drag'.
  */
 
(function($){
$.fn.drag=function(h,s){return i(this,h,'d',s);};
$.fn.resize=function(h,s){return i(this,h,'r',s);};
$.ektronDnR={dnr:{},e:0,
drag:function(v){
 if(M.k == 'd')E.css({left:M.X+v.pageX-M.pX,top:M.Y+v.pageY-M.pY});
 else E.css({width:Math.max(v.pageX-M.pX+M.W,0),height:Math.max(v.pageY-M.pY+M.H,0)});
  return false;},
stop:function(){E.css('opacity',M.o);$().unbind('mousemove',J.drag).unbind('mouseup',J.stop);}
};
var J=$.ektronDnR,M=J.dnr,E=J.e,
i=function(e,h,k,s){return e.each(function(){h=(h)?$(h,e):e;
 h.bind('mousedown',{e:e,k:k},function(v){var d=v.data,p={};E=d.e;
 // attempt utilization of dimensions plugin to fix IE issues
 if(E.css('position') != 'relative'){try{E.position(p);}catch(e){}}
 M={X:p.left||f('left')||0,Y:p.top||f('top')||0,W:f('width')||E[0].scrollWidth||0,H:f('height')||E[0].scrollHeight||0,pX:v.pageX,pY:v.pageY,k:d.k,o:E.css('opacity')};
 if(s && s.opacity){ E.css({opacity:s.opacity})};$().mousemove($.ektronDnR.drag).mouseup($.ektronDnR.stop);
 return false;
 });
});},
f=function(k){return parseInt(E.css(k))||false;};
})($ektron);