/*!
   SoundManager 2: Javascript Sound for the Web
   --------------------------------------------
   http://schillmania.com/projects/soundmanager2/

   Copyright (c) 2007, Scott Schiller. All rights reserved.
   Code provided under the BSD License:
   http://schillmania.com/projects/soundmanager2/license.txt

   V2.95b.20100101
*/
var soundManager=null;function SoundManager(c,e){this.flashVersion=8;this.debugMode=false;this.debugFlash=false;this.useConsole=true;this.consoleOnly=false;this.waitForWindowLoad=false;this.nullURL="null.mp3";this.allowPolling=true;this.useFastPolling=false;this.useMovieStar=false;this.bgColor="#ffffff";this.useHighPerformance=false;this.flashLoadTimeout=1000;this.wmode=null;this.allowFullScreen=true;this.allowScriptAccess="always";this.defaultOptions={autoLoad:false,stream:true,autoPlay:false,onid3:null,onload:null,whileloading:null,onplay:null,onpause:null,onresume:null,whileplaying:null,onstop:null,onfinish:null,onbeforefinish:null,onbeforefinishtime:5000,onbeforefinishcomplete:null,onjustbeforefinish:null,onjustbeforefinishtime:200,multiShot:true,multiShotEvents:false,position:null,pan:0,volume:100};this.flash9Options={isMovieStar:null,usePeakData:false,useWaveformData:false,useEQData:false,onbufferchange:null,ondataerror:null};this.movieStarOptions={onmetadata:null,useVideo:false,bufferTime:null};var d=null;var i=this;var h="soundManager";this.version=null;this.versionNumber="V2.95b.20100101";this.movieURL=null;this.url=null;this.altURL=null;this.swfLoaded=false;this.enabled=false;this.o=null;this.id=(e||"sm2movie");this.oMC=null;this.sounds={};this.soundIDs=[];this.muted=false;this.isFullScreen=false;this.isIE=(navigator.userAgent.match(/MSIE/i));this.isSafari=(navigator.userAgent.match(/safari/i));this.debugID="soundmanager-debug";this.debugURLParam=/([#?&])debug=1/i;this.specialWmodeCase=false;this._onready=[];this._debugOpen=true;this._didAppend=false;this._appendSuccess=false;this._didInit=false;this._disabled=false;this._windowLoaded=false;this._hasConsole=(typeof console!="undefined"&&typeof console.log!="undefined");this._debugLevels=["log","info","warn","error"];this._defaultFlashVersion=8;this._oRemoved=null;this._oRemovedHTML=null;var f=function(j){return document.getElementById(j)};this.filePatterns={flash8:/\.mp3(\?.*)?$/i,flash9:/\.mp3(\?.*)?$/i};this.netStreamTypes=["aac","flv","mov","mp4","m4v","f4v","m4a","mp4v","3gp","3g2"];this.netStreamPattern=new RegExp("\\.("+this.netStreamTypes.join("|")+")(\\?.*)?$","i");this.filePattern=null;this.features={buffering:false,peakData:false,waveformData:false,eqData:false,movieStar:false};this.sandbox={type:null,types:{remote:"remote (domain-based) rules",localWithFile:"local with file access (no internet access)",localWithNetwork:"local with network (internet access only, no local access)",localTrusted:"local, trusted (local+internet access)"},description:null,noRemote:null,noLocal:null};this._setVersionInfo=function(){if(i.flashVersion!=8&&i.flashVersion!=9){alert(i._str("badFV",i.flashVersion,i._defaultFlashVersion));i.flashVersion=i._defaultFlashVersion}i.version=i.versionNumber+(i.flashVersion==9?" (AS3/Flash 9)":" (AS2/Flash 8)");if(i.flashVersion>8){i.defaultOptions=i._mergeObjects(i.defaultOptions,i.flash9Options);i.features.buffering=true}if(i.flashVersion>8&&i.useMovieStar){i.defaultOptions=i._mergeObjects(i.defaultOptions,i.movieStarOptions);i.filePatterns.flash9=new RegExp("\\.(mp3|"+i.netStreamTypes.join("|")+")(\\?.*)?$","i");i.features.movieStar=true}else{i.useMovieStar=false;i.features.movieStar=false}i.filePattern=i.filePatterns[(i.flashVersion!=8?"flash9":"flash8")];i.movieURL=(i.flashVersion==8?"soundmanager2.swf":"soundmanager2_flash9.swf");i.features.peakData=i.features.waveformData=i.features.eqData=(i.flashVersion>8)};this._overHTTP=(document.location?document.location.protocol.match(/http/i):null);this._waitingforEI=false;this._initPending=false;this._tryInitOnFocus=(this.isSafari&&typeof document.hasFocus=="undefined");this._isFocused=(typeof document.hasFocus!="undefined"?document.hasFocus():null);this._okToDisable=!this._tryInitOnFocus;this.useAltURL=!this._overHTTP;var a="";this.strings={};var b="";this._str=function(){var p=Array.prototype.slice.call(arguments);var n=p.shift();var m=i.strings&&i.strings[n]?i.strings[n]:"";if(m&&p&&p.length){for(var l=0,k=p.length;l<k;l++){m=m.replace("%s",p[l])}}return m};this.supported=function(){return(i._didInit&&!i._disabled)};this.getMovie=function(j){return i.isIE?window[j]:(i.isSafari?f(j)||document[j]:f(j))};this.loadFromXML=function(j){try{i.o._loadFromXML(j)}catch(k){i._failSafely();return true}};this.createSound=function(k){var m="soundManager.createSound(): ";if(!i._didInit){throw i._complain(m+i._str("notReady"),arguments.callee.caller)}if(arguments.length==2){k={id:arguments[0],url:arguments[1]}}var l=i._mergeObjects(k);var j=l;if(j.id.toString().charAt(0).match(/^[0-9]$/)){}if(i._idCheck(j.id,true)){return i.sounds[j.id]}if(i.flashVersion>8&&i.useMovieStar){if(j.isMovieStar===null){j.isMovieStar=(j.url.match(i.netStreamPattern)?true:false)}if(j.isMovieStar){}if(j.isMovieStar&&(j.usePeakData||j.useWaveformData||j.useEQData)){j.usePeakData=false;j.useWaveformData=false;j.useEQData=false}}i.sounds[j.id]=new d(j);i.soundIDs[i.soundIDs.length]=j.id;if(i.flashVersion==8){i.o._createSound(j.id,j.onjustbeforefinishtime)}else{i.o._createSound(j.id,j.url,j.onjustbeforefinishtime,j.usePeakData,j.useWaveformData,j.useEQData,j.isMovieStar,(j.isMovieStar?j.useVideo:false),(j.isMovieStar?j.bufferTime:false))}if(j.autoLoad||j.autoPlay){if(i.sounds[j.id]){i.sounds[j.id].load(j)}}if(j.autoPlay){i.sounds[j.id].play()}return i.sounds[j.id]};this.createVideo=function(j){if(arguments.length==2){j={id:arguments[0],url:arguments[1]}}if(i.flashVersion>=9){j.isMovieStar=true;j.useVideo=true}else{return false}if(!i.useMovieStar){}return i.createSound(j)};this.destroySound=function(k,j){if(!i._idCheck(k)){return false}for(var l=0;l<i.soundIDs.length;l++){if(i.soundIDs[l]==k){i.soundIDs.splice(l,1);continue}}i.sounds[k].unload();if(!j){i.sounds[k].destruct()}delete i.sounds[k]};this.destroyVideo=this.destroySound;this.load=function(j,k){if(!i._idCheck(j)){return false}i.sounds[j].load(k)};this.unload=function(j){if(!i._idCheck(j)){return false}i.sounds[j].unload()};this.play=function(j,k){if(!i._didInit){throw i._complain(b+i._str("notReady"),arguments.callee.caller)}if(!i._idCheck(j)){if(typeof k!="Object"){k={url:k}}if(k&&k.url){k.id=j;i.createSound(k)}else{return false}}i.sounds[j].play(k)};this.start=this.play;this.setPosition=function(j,k){if(!i._idCheck(j)){return false}i.sounds[j].setPosition(k)};this.stop=function(j){if(!i._idCheck(j)){return false}i.sounds[j].stop()};this.stopAll=function(){for(var j in i.sounds){if(i.sounds[j] instanceof d){i.sounds[j].stop()}}};this.pause=function(j){if(!i._idCheck(j)){return false}i.sounds[j].pause()};this.pauseAll=function(){for(var j=i.soundIDs.length;j--;){i.sounds[i.soundIDs[j]].pause()}};this.resume=function(j){if(!i._idCheck(j)){return false}i.sounds[j].resume()};this.resumeAll=function(){for(var j=i.soundIDs.length;j--;){i.sounds[i.soundIDs[j]].resume()}};this.togglePause=function(j){if(!i._idCheck(j)){return false}i.sounds[j].togglePause()};this.setPan=function(j,k){if(!i._idCheck(j)){return false}i.sounds[j].setPan(k)};this.setVolume=function(k,j){if(!i._idCheck(k)){return false}i.sounds[k].setVolume(j)};this.mute=function(j){if(typeof j!="string"){j=null}if(!j){for(var k=i.soundIDs.length;k--;){i.sounds[i.soundIDs[k]].mute()}i.muted=true}else{if(!i._idCheck(j)){return false}i.sounds[j].mute()}};this.muteAll=function(){i.mute()};this.unmute=function(j){if(typeof j!="string"){j=null}if(!j){for(var k=i.soundIDs.length;k--;){i.sounds[i.soundIDs[k]].unmute()}i.muted=false}else{if(!i._idCheck(j)){return false}i.sounds[j].unmute()}};this.unmuteAll=function(){i.unmute()};this.toggleMute=function(j){if(!i._idCheck(j)){return false}i.sounds[j].toggleMute()};this.getMemoryUse=function(){if(i.flashVersion==8){return 0}if(i.o){return parseInt(i.o._getMemoryUse(),10)}};this.disable=function(k){if(typeof k=="undefined"){k=false}if(i._disabled){return false}i._disabled=true;for(var j=i.soundIDs.length;j--;){i._disableObject(i.sounds[i.soundIDs[j]])}i.initComplete(k)};this.canPlayURL=function(j){return(j?(j.match(i.filePattern)?true:false):null)};this.getSoundById=function(k,l){if(!k){throw new Error("SoundManager.getSoundById(): sID is null/undefined")}var j=i.sounds[k];if(!j&&!l){}return j};this.onready=function(k,j){if(k&&k instanceof Function){if(i._didInit){}if(!j){j=window}i._addOnReady(k,j);i._processOnReady();return true}else{throw i._str("needFunction")}};this.oninitmovie=function(){};this.onload=function(){soundManager._wD("soundManager.onload()",1)};this.onerror=function(){};this._idCheck=this.getSoundById;this._complain=function(k,m){var l="Error: ";if(!m){return new Error(l+k)}var o=new Error("");var p=null;if(o.stack){try{var q="@";var r=o.stack.split(q);p=r[4]}catch(n){p=o.stack}}if(typeof console!="undefined"&&typeof console.trace!="undefined"){console.trace()}var j=l+k+". \nCaller: "+m.toString()+(o.stack?" \nTop of stacktrace: "+p:(o.message?" \nMessage: "+o.message:""));return new Error(j)};var g=function(){return false};g._protected=true;this._disableObject=function(k){for(var j in k){if(typeof k[j]=="function"&&typeof k[j]._protected=="undefined"){k[j]=g}}j=null};this._failSafely=function(j){if(typeof j=="undefined"){j=false}if(!i._disabled||j){i.disable(j)}};this._normalizeMovieURL=function(j){var k=null;if(j){if(j.match(/\.swf(\?.*)?$/i)){k=j.substr(j.toLowerCase().lastIndexOf(".swf?")+4);if(k){return j}}else{if(j.lastIndexOf("/")!=j.length-1){j=j+"/"}}}return(j&&j.lastIndexOf("/")!=-1?j.substr(0,j.lastIndexOf("/")+1):"./")+i.movieURL};this._getDocument=function(){return(document.body?document.body:(document.documentElement?document.documentElement:document.getElementsByTagName("div")[0]))};this._getDocument._protected=true;this._setPolling=function(j,k){if(!i.o||!i.allowPolling){return false}i.o._setPolling(j,k)};this._createMovie=function(q,n){var p=null;var w=(n?n:i.url);var m=(i.altURL?i.altURL:w);if(i._didAppend&&i._appendSuccess){return false}i._didAppend=true;i._setVersionInfo();i.url=i._normalizeMovieURL(i._overHTTP?w:m);n=i.url;if(i.useHighPerformance&&i.useMovieStar&&i.defaultOptions.useVideo===true){p="soundManager note: disabling highPerformance, not applicable with movieStar mode+useVideo";i.useHighPerformance=false}i.wmode=(!i.wmode&&i.useHighPerformance&&!i.useMovieStar?"transparent":i.wmode);if(i.wmode!==null&&i.flashLoadTimeout!==0&&(!i.useHighPerformance||i.debugFlash)&&!i.isIE&&navigator.platform.match(/win32/i)){i.specialWmodeCase=true;i.wmode=null}if(i.flashVersion==8){i.allowFullScreen=false}var u={name:q,id:q,src:n,width:"100%",height:"100%",quality:"high",allowScriptAccess:i.allowScriptAccess,bgcolor:i.bgColor,pluginspage:"http://www.macromedia.com/go/getflashplayer",type:"application/x-shockwave-flash",wmode:i.wmode,allowfullscreen:(i.allowFullScreen?"true":"false")};if(i.debugFlash){u.FlashVars="debug=1"}if(!i.wmode){delete u.wmode}var l=null;var v=null;var j=null;var t=null;if(i.isIE){l=document.createElement("div");j='<object id="'+q+'" data="'+n+'" type="'+u.type+'" width="'+u.width+'" height="'+u.height+'"><param name="movie" value="'+n+'" /><param name="AllowScriptAccess" value="'+i.allowScriptAccess+'" /><param name="quality" value="'+u.quality+'" />'+(i.wmode?'<param name="wmode" value="'+i.wmode+'" /> ':"")+'<param name="bgcolor" value="'+i.bgColor+'" /><param name="allowFullScreen" value="'+u.allowFullScreen+'" />'+(i.debugFlash?'<param name="FlashVars" value="'+u.FlashVars+'" />':"")+"<!-- --></object>"}else{l=document.createElement("embed");for(v in u){if(u.hasOwnProperty(v)){l.setAttribute(v,u[v])}}}var r=null;var B=null;if(i.debugMode){r=document.createElement("div");r.id=i.debugID+"-toggle";B={position:"fixed",bottom:"0px",right:"0px",width:"1.2em",height:"1.2em",lineHeight:"1.2em",margin:"2px",textAlign:"center",border:"1px solid #999",cursor:"pointer",background:"#fff",color:"#333",zIndex:10001};r.appendChild(document.createTextNode("-"));r.onclick=i._toggleDebug;r.title="Toggle SM2 debug console";if(navigator.userAgent.match(/msie 6/i)){r.style.position="absolute";r.style.cursor="hand"}for(v in B){if(B.hasOwnProperty(v)){r.style[v]=B[v]}}}var k=i._getDocument();if(k){i.oMC=f("sm2-container")?f("sm2-container"):document.createElement("div");var o=(i.debugMode?" sm2-debug":"")+(i.debugFlash?" flash-debug":"");if(!i.oMC.id){i.oMC.id="sm2-container";i.oMC.className="movieContainer"+o;var A=null;t=null;if(i.useHighPerformance){A={position:"fixed",width:"8px",height:"8px",bottom:"0px",left:"0px",overflow:"hidden"}}else{A={position:"absolute",width:"8px",height:"8px",top:"-9999px",left:"-9999px"}}var z=null;if(!i.debugFlash){for(z in A){if(A.hasOwnProperty(z)){i.oMC.style[z]=A[z]}}}try{if(!i.isIE){i.oMC.appendChild(l)}k.appendChild(i.oMC);if(i.isIE){t=i.oMC.appendChild(document.createElement("div"));t.className="sm2-object-box";t.innerHTML=j}i._appendSuccess=true}catch(y){throw new Error(i._str("appXHTML"))}}else{if(i.debugMode||i.debugFlash){i.oMC.className+=o}i.oMC.appendChild(l);if(i.isIE){t=i.oMC.appendChild(document.createElement("div"));t.className="sm2-object-box";t.innerHTML=j}i._appendSuccess=true}k=null}if(p){}};this._writeDebug=function(j,l,k){};this._writeDebug._protected=true;this._wdCount=0;this._wdCount._protected=true;this._wD=this._writeDebug;this._debug=function(){for(var l=0,k=i.soundIDs.length;l<k;l++){i.sounds[i.soundIDs[l]]._debug()}};this._debugTS=function(m,j,k){if(typeof sm2Debugger!="undefined"){try{sm2Debugger.handleEvent(m,j,k)}catch(l){}}};this._debugTS._protected=true;this._mergeObjects=function(k,j){var n={};for(var l in k){if(k.hasOwnProperty(l)){n[l]=k[l]}}var m=(typeof j=="undefined"?i.defaultOptions:j);for(var p in m){if(m.hasOwnProperty(p)&&typeof n[p]=="undefined"){n[p]=m[p]}}return n};this.createMovie=function(j){if(j){i.url=j}i._initMovie()};this.go=this.createMovie;this._initMovie=function(){if(i.o){return false}i.o=i.getMovie(i.id);if(!i.o){if(!i.oRemoved){i._createMovie(i.id,i.url)}else{if(!i.isIE){i.oMC.appendChild(i.oRemoved)}else{i.oMC.innerHTML=i.oRemovedHTML}i.oRemoved=null;i._didAppend=true}i.o=i.getMovie(i.id)}if(i.o){if(i.flashLoadTimeout>0){}}if(typeof i.oninitmovie=="function"){setTimeout(i.oninitmovie,1)}};this.waitForExternalInterface=function(){if(i._waitingForEI){return false}i._waitingForEI=true;if(i._tryInitOnFocus&&!i._isFocused){return false}if(i.flashLoadTimeout>0){if(!i._didInit){var j=i.getMoviePercent()}setTimeout(function(){var k=i.getMoviePercent();if(!i._didInit){if(!i._overHTTP){if(!i.debugFlash){}}if(k===0){}i._debugTS("flashtojs",false,": Timed out"+(i._overHTTP)?" (Check flash security or flash blockers)":" (No plugin/missing SWF?)")}if(!i._didInit&&i._okToDisable){i._failSafely(true)}},i.flashLoadTimeout)}else{if(!i._didInit){}}};this.getMoviePercent=function(){return(i.o&&typeof i.o.PercentLoaded!="undefined"?i.o.PercentLoaded():null)};this.handleFocus=function(){if(i._isFocused||!i._tryInitOnFocus){return true}i._okToDisable=true;i._isFocused=true;if(i._tryInitOnFocus){window.removeEventListener("mousemove",i.handleFocus,false)}i._waitingForEI=false;setTimeout(i.waitForExternalInterface,500);if(window.removeEventListener){window.removeEventListener("focus",i.handleFocus,false)}else{if(window.detachEvent){window.detachEvent("onfocus",i.handleFocus)}}};this.initComplete=function(j){if(i._didInit){return false}i._didInit=true;if(i._disabled||j){i._processOnReady();i._debugTS("onload",false);i.onerror.apply(window);return false}else{i._debugTS("onload",true)}if(i.waitForWindowLoad&&!i._windowLoaded){if(window.addEventListener){window.addEventListener("load",i._initUserOnload,false)}else{if(window.attachEvent){window.attachEvent("onload",i._initUserOnload)}}return false}else{if(i.waitForWindowLoad&&i._windowLoaded){}i._initUserOnload()}};this._addOnReady=function(k,j){i._onready.push({method:k,scope:(j||null),fired:false})};this._processOnReady=function(){if(!i._didInit){return false}var l={success:(!i._disabled)};var k=[];for(var n=0,m=i._onready.length;n<m;n++){if(i._onready[n].fired!==true){k.push(i._onready[n])}}if(k.length){for(n=0,m=k.length;n<m;n++){if(k[n].scope){k[n].method.apply(k[n].scope,[l])}else{k[n].method(l)}k[n].fired=true}}};this._initUserOnload=function(){window.setTimeout(function(){i._processOnReady();i.onload.apply(window)})};this.init=function(){i._initMovie();if(i._didInit){return false}if(window.removeEventListener){window.removeEventListener("load",i.beginDelayedInit,false)}else{if(window.detachEvent){window.detachEvent("onload",i.beginDelayedInit)}}try{i.o._externalInterfaceTest(false);if(!i.allowPolling){}else{i._setPolling(true,i.useFastPolling?true:false)}if(!i.debugMode){i.o._disableDebug()}i.enabled=true;i._debugTS("jstoflash",true)}catch(j){i._debugTS("jstoflash",false);i._failSafely(true);i.initComplete();return false}i.initComplete()};this.beginDelayedInit=function(){i._windowLoaded=true;setTimeout(i.waitForExternalInterface,500);setTimeout(i.beginInit,20)};this.beginInit=function(){if(i._initPending){return false}i.createMovie();i._initMovie();i._initPending=true;return true};this.domContentLoaded=function(){if(document.removeEventListener){document.removeEventListener("DOMContentLoaded",i.domContentLoaded,false)}i.go()};this._externalInterfaceOK=function(j){if(i.swfLoaded){return false}var k=new Date().getTime();i._debugTS("swf",true);i._debugTS("flashtojs",true);i.swfLoaded=true;i._tryInitOnFocus=false;if(i.isIE){setTimeout(i.init,100)}else{i.init()}};this._setSandboxType=function(j){var k=i.sandbox;k.type=j;k.description=k.types[(typeof k.types[j]!="undefined"?j:"unknown")];if(k.type=="localWithFile"){k.noRemote=true;k.noLocal=false}else{if(k.type=="localWithNetwork"){k.noRemote=false;k.noLocal=true}else{if(k.type=="localTrusted"){k.noRemote=false;k.noLocal=false}}}};this.reboot=function(){if(i.soundIDs.length){}for(var j=i.soundIDs.length;j--;){i.sounds[i.soundIDs[j]].destruct()}try{if(i.isIE){i.oRemovedHTML=i.o.innerHTML}i.oRemoved=i.o.parentNode.removeChild(i.o)}catch(k){}i.oRemovedHTML=null;i.oRemoved=null;i.enabled=false;i._didInit=false;i._waitingForEI=false;i._initPending=false;i._didAppend=false;i._appendSuccess=false;i._disabled=false;i._waitingforEI=true;i.swfLoaded=false;i.soundIDs={};i.sounds=[];i.o=null;for(j=i._onready.length;j--;){i._onready[j].fired=false}window.setTimeout(soundManager.beginDelayedInit,20)};this.destruct=function(){i.disable(true)};d=function(j){var k=this;this.sID=j.id;this.url=j.url;this.options=i._mergeObjects(j);this.instanceOptions=this.options;this._iO=this.instanceOptions;this.pan=this.options.pan;this.volume=this.options.volume;this._lastURL=null;this._debug=function(){};this._debug();this.id3={};this.resetProperties=function(l){k.bytesLoaded=null;k.bytesTotal=null;k.position=null;k.duration=null;k.durationEstimate=null;k.loaded=false;k.playState=0;k.paused=false;k.readyState=0;k.muted=false;k.didBeforeFinish=false;k.didJustBeforeFinish=false;k.isBuffering=false;k.instanceOptions={};k.instanceCount=0;k.peakData={left:0,right:0};k.waveformData={left:[],right:[]};k.eqData=[];k.eqData.left=[];k.eqData.right=[]};k.resetProperties();this.load=function(l){if(typeof l!="undefined"){k._iO=i._mergeObjects(l);k.instanceOptions=k._iO}else{l=k.options;k._iO=l;k.instanceOptions=k._iO;if(k._lastURL&&k._lastURL!=k.url){k._iO.url=k.url;k.url=null}}if(typeof k._iO.url=="undefined"){k._iO.url=k.url}if(k._iO.url==k.url&&k.readyState!==0&&k.readyState!=2){return false}k.url=k._iO.url;k._lastURL=k._iO.url;k.loaded=false;k.readyState=1;k.playState=0;try{if(i.flashVersion==8){i.o._load(k.sID,k._iO.url,k._iO.stream,k._iO.autoPlay,(k._iO.whileloading?1:0))}else{i.o._load(k.sID,k._iO.url,k._iO.stream?true:false,k._iO.autoPlay?true:false);if(k._iO.isMovieStar&&k._iO.autoLoad&&!k._iO.autoPlay){k.pause()}}}catch(m){i._debugTS("onload",false);i.onerror();i.disable()}};this.unload=function(){if(k.readyState!==0){if(k.readyState!=2){k.setPosition(0,true)}i.o._unload(k.sID,i.nullURL);k.resetProperties()}};this.destruct=function(){i.o._destroySound(k.sID);i.destroySound(k.sID,true)};this.play=function(m){if(!m){m={}}k._iO=i._mergeObjects(m,k._iO);k._iO=i._mergeObjects(k._iO,k.options);k.instanceOptions=k._iO;if(k.playState==1){var l=k._iO.multiShot;if(!l){return false}else{}}if(!k.loaded){if(k.readyState===0){k._iO.autoPlay=true;k.load(k._iO)}else{if(k.readyState==2){return false}else{}}}else{}if(k.paused){k.resume()}else{k.playState=1;if(!k.instanceCount||i.flashVersion>8){k.instanceCount++}k.position=(typeof k._iO.position!="undefined"&&!isNaN(k._iO.position)?k._iO.position:0);if(k._iO.onplay){k._iO.onplay.apply(k)}k.setVolume(k._iO.volume,true);k.setPan(k._iO.pan,true);i.o._start(k.sID,k._iO.loop||1,(i.flashVersion==9?k.position:k.position/1000))}};this.start=this.play;this.stop=function(l){if(k.playState==1){k.playState=0;k.paused=false;if(k._iO.onstop){k._iO.onstop.apply(k)}i.o._stop(k.sID,l);k.instanceCount=0;k._iO={}}};this.setPosition=function(m,l){if(typeof m=="undefined"){m=0}var n=Math.min(k.duration,Math.max(m,0));k._iO.position=n;if(!l){}i.o._setPosition(k.sID,(i.flashVersion==9?k._iO.position:k._iO.position/1000),(k.paused||!k.playState))};this.pause=function(){if(k.paused||k.playState===0){return false}k.paused=true;i.o._pause(k.sID);if(k._iO.onpause){k._iO.onpause.apply(k)}};this.resume=function(){if(!k.paused||k.playState===0){return false}k.paused=false;i.o._pause(k.sID);if(k._iO.onresume){k._iO.onresume.apply(k)}};this.togglePause=function(){if(k.playState===0){k.play({position:(i.flashVersion==9?k.position:k.position/1000)});return false}if(k.paused){k.resume()}else{k.pause()}};this.setPan=function(m,l){if(typeof m=="undefined"){m=0}if(typeof l=="undefined"){l=false}i.o._setPan(k.sID,m);k._iO.pan=m;if(!l){k.pan=m}};this.setVolume=function(l,m){if(typeof l=="undefined"){l=100}if(typeof m=="undefined"){m=false}i.o._setVolume(k.sID,(i.muted&&!k.muted)||k.muted?0:l);k._iO.volume=l;if(!m){k.volume=l}};this.mute=function(){k.muted=true;i.o._setVolume(k.sID,0)};this.unmute=function(){k.muted=false;var l=typeof k._iO.volume!="undefined";i.o._setVolume(k.sID,l?k._iO.volume:k.options.volume)};this.toggleMute=function(){if(k.muted){k.unmute()}else{k.mute()}};this._whileloading=function(l,m,n){if(!k._iO.isMovieStar){k.bytesLoaded=l;k.bytesTotal=m;k.duration=Math.floor(n);k.durationEstimate=parseInt((k.bytesTotal/k.bytesLoaded)*k.duration,10);if(k.durationEstimate===undefined){k.durationEstimate=k.duration}if(k.readyState!=3&&k._iO.whileloading){k._iO.whileloading.apply(k)}}else{k.bytesLoaded=l;k.bytesTotal=m;k.duration=Math.floor(n);k.durationEstimate=k.duration;if(k.readyState!=3&&k._iO.whileloading){k._iO.whileloading.apply(k)}}};this._onid3=function(o,l){var p=[];for(var n=0,m=o.length;n<m;n++){p[o[n]]=l[n]}k.id3=i._mergeObjects(k.id3,p);if(k._iO.onid3){k._iO.onid3.apply(k)}};this._whileplaying=function(n,o,q,m,p){if(isNaN(n)||n===null){return false}if(k.playState===0&&n>0){n=0}k.position=n;if(i.flashVersion>8){if(k._iO.usePeakData&&typeof o!="undefined"&&o){k.peakData={left:o.leftPeak,right:o.rightPeak}}if(k._iO.useWaveformData&&typeof q!="undefined"&&q){k.waveformData={left:q.split(","),right:m.split(",")}}if(k._iO.useEQData){if(typeof p!="undefined"&&p.leftEQ){var l=p.leftEQ.split(",");k.eqData=l;k.eqData.left=l;if(typeof p.rightEQ!="undefined"&&p.rightEQ){k.eqData.right=p.rightEQ.split(",")}}}}if(k.playState==1){if(k.isBuffering){k._onbufferchange(0)}if(k._iO.whileplaying){k._iO.whileplaying.apply(k)}if(k.loaded&&k._iO.onbeforefinish&&k._iO.onbeforefinishtime&&!k.didBeforeFinish&&k.duration-k.position<=k._iO.onbeforefinishtime){k._onbeforefinish()}}};this._onload=function(l){l=(l==1?true:false);if(!l){if(i.sandbox.noRemote===true){}if(i.sandbox.noLocal===true){}}k.loaded=l;k.readyState=l?3:2;if(k._iO.onload){k._iO.onload.apply(k)}};this._onbeforefinish=function(){if(!k.didBeforeFinish){k.didBeforeFinish=true;if(k._iO.onbeforefinish){k._iO.onbeforefinish.apply(k)}}};this._onjustbeforefinish=function(l){if(!k.didJustBeforeFinish){k.didJustBeforeFinish=true;if(k._iO.onjustbeforefinish){k._iO.onjustbeforefinish.apply(k)}}};this._onfinish=function(){if(k._iO.onbeforefinishcomplete){k._iO.onbeforefinishcomplete.apply(k)}k.didBeforeFinish=false;k.didJustBeforeFinish=false;if(k.instanceCount){k.instanceCount--;if(!k.instanceCount){k.playState=0;k.paused=false;k.instanceCount=0;k.instanceOptions={}}if(!k.instanceCount||k._iO.multiShotEvents){if(k._iO.onfinish){k._iO.onfinish.apply(k)}}}else{if(k.useVideo){}}};this._onmetadata=function(l){if(!l.width&&!l.height){l.width=320;l.height=240}k.metadata=l;k.width=l.width;k.height=l.height;if(k._iO.onmetadata){k._iO.onmetadata.apply(k)}};this._onbufferchange=function(l){if(k.playState===0){return false}if(l==k.isBuffering){return false}k.isBuffering=(l==1?true:false);if(k._iO.onbufferchange){k._iO.onbufferchange.apply(k)}};this._ondataerror=function(l){if(k.playState>0){if(k._iO.ondataerror){k._iO.ondataerror.apply(k)}}else{}}};this._onfullscreenchange=function(j){i.isFullScreen=(j==1?true:false);if(!i.isFullScreen){try{window.focus()}catch(k){}}};if(window.addEventListener){window.addEventListener("focus",i.handleFocus,false);window.addEventListener("load",i.beginDelayedInit,false);window.addEventListener("unload",i.destruct,false);if(i._tryInitOnFocus){window.addEventListener("mousemove",i.handleFocus,false)}}else{if(window.attachEvent){window.attachEvent("onfocus",i.handleFocus);window.attachEvent("onload",i.beginDelayedInit);window.attachEvent("unload",i.destruct)}else{i._debugTS("onload",false);soundManager.onerror();soundManager.disable()}}if(document.addEventListener){document.addEventListener("DOMContentLoaded",i.domContentLoaded,false)}}if(typeof SM2_DEFER=="undefined"||!SM2_DEFER){soundManager=new SoundManager()};
