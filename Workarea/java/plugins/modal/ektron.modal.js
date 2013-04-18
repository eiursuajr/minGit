﻿/*
 * ektron.modal.js is based on jqModal - Minimalist Modaling with jQuery
 *   (http://dev.iceburg.net/jquery/jqmodal/)
 *
 * Copyright (c) 2007,2008 Brice Burgess <bhb@iceburg.net>
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 * 
 * $Version: 07/06/2008 +r13
 *
 * Conversion Notes:
 * 1) references to the string 'modal' were replaced with 'modal'.
 * 2) default class names were changed for better namespacing purposes.
 * 3) Iframe class name was changed for better namespacing. 
 * 4) ID jqmP changed to ektronModalP.
 */
(function($) {
$.fn.modal=function(o){
var p={
overlay: 50,
overlayClass: 'ektronModalOverlay',
closeClass: 'ektronModalClose',
trigger: '.ektronModal',
ajax: F,
ajaxText: '',
target: F,
modal: F,
toTop: F,
onShow: F,
onHide: F,
onLoad: F
};
return this.each(function(){if(this._modal)return H[this._modal].c=$.extend({},H[this._modal].c,o);s++;this._modal=s;
H[s]={c:$.extend(p,$.modal.params,o),a:F,w:$(this).addClass('modalID'+s),s:s};
if(p.trigger)$(this).modalAddTrigger(p.trigger);
});};

$.fn.modalAddClose=function(e){return hs(this,e,'modalHide');};
$.fn.modalAddTrigger=function(e){return hs(this,e,'modalShow');};
$.fn.modalShow=function(t){return this.each(function(){$.modal.open(this._modal,t);});};
$.fn.modalHide=function(t){return this.each(function(){$.modal.close(this._modal,t)});};

$.modal = {
hash:{},
open:function(s,t){var h=H[s],c=h.c,cc='.'+c.closeClass,z=(parseInt(h.w.css('z-index'))),z=(z>0)?z:3000,o=$('<div></div>').css({height:'100%',width:'100%',position:'fixed',left:0,top:0,'z-index':z-1,opacity:c.overlay/100});if(h.a)return F;h.t=t;h.a=true;h.w.css('z-index',z);
 if(c.modal) {if(!A[0])L('bind');A.push(s);}
 else if(c.overlay > 0)h.w.modalAddClose(o);
 else o=F;

 h.o=(o)?o.addClass(c.overlayClass).prependTo('body'):F;
 if(ie6){$('html,body').css({height:'100%',width:'100%'});if(o){o=o.css({position:'absolute'})[0];for(var y in {Top:1,Left:1})o.style.setExpression(y.toLowerCase(),"(_=(document.documentElement.scroll"+y+" || document.body.scroll"+y+"))+'px'");}}

 if(c.ajax) {var r=c.target||h.w,u=c.ajax,r=(typeof r == 'string')?$(r,h.w):$(r),u=(u.substr(0,1) == '@')?$(t).attr(u.substring(1)):u;
  r.html(c.ajaxText).load(u,function(){if(c.onLoad)c.onLoad.call(this,h);if(cc)h.w.modalAddClose($(cc,h.w));e(h);});}
 else if(cc)h.w.modalAddClose($(cc,h.w));

 if(c.toTop&&h.o)h.w.before('<span id="ektronModalP'+h.w[0]._modal+'"></span>').insertAfter(h.o);	
 (c.onShow)?c.onShow(h):h.w.show();e(h);return F;
},
close:function(s){var h=H[s];if(!h||!h.a)return F;h.a=F;
 if(A[0]){A.pop();if(!A[0])L('unbind');}
 if(h.c.toTop&&h.o)$('#ektronModalP'+h.w[0]._modal).after(h.w).remove();
 if(h.c.onHide)h.c.onHide(h);else{h.w.hide();if(h.o)h.o.remove();} return F;
},
params:{}};
var s=0,H=$.modal.hash,A=[],ie6=$.browser.msie&&($.browser.version == "6.0"),F=false,
i=$('<iframe src="javascript:false;document.write(\'\');" class="ektronModalIframe"></iframe>').css({opacity:0}),
e=function(h){if(ie6)if(h.o)h.o.html('<p style="width:100%;height:100%"/>').prepend(i);else if(!$('iframe.ektronModalIframe',h.w)[0])h.w.prepend(i); f(h);},
f=function(h){try{$(':input:visible',h.w).eq(0).focus();}catch(_){}},
L=function(t){$()[t]("keypress",m)[t]("keydown",m)[t]("mousedown",m);},
m=function(e){var h=H[A[A.length-1]],r=(!$(e.target).parents('.modalID'+h.s)[0]);if(r)f(h);return !r;},
hs=function(w,t,c){return w.each(function(){var s=this._modal;$(t).each(function() {
 if(!this[c]){this[c]=[];$(this).click(function(){for(var i in {modalShow:1,modalHide:1})for(var s in this[i])if(H[this[i][s]])H[this[i][s]].w[i](this);return F;});}this[c].push(s);});});};
})($ektron);