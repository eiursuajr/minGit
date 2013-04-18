//ektron json (via json.org)
if("undefined" == typeof(Ektron)){Ektron = {};}if("undefined" == typeof(Ektron.JSON)){Ektron.JSON=function(){function f(n){return n<10?"0"+n:n}Date.prototype.toJSON=function(key){return this.getUTCFullYear()+"-"+f(this.getUTCMonth()+1)+"-"+f(this.getUTCDate())+"T"+f(this.getUTCHours())+":"+f(this.getUTCMinutes())+":"+f(this.getUTCSeconds())+"Z"};var cx=/[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,escapeable=/[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,gap,indent,meta={"\b":"\\b","\t":"\\t","\n":"\\n","\f":"\\f","\r":"\\r",'"':'\\"',"\\":"\\\\"},rep;function quote(string){escapeable.lastIndex=0;return escapeable.test(string)?'"'+string.replace(escapeable,function(a){var c=meta[a];if(typeof c==="string"){return c}return"\\u"+("0000"+(+(a.charCodeAt(0))).toString(16)).slice(-4)})+'"':'"'+string+'"'}function str(key,holder){var i,k,v,length,mind=gap,partial,value=holder[key];if(value&&typeof value==="object"&&typeof value.toJSON==="function"){value=value.toJSON(key)}if(typeof rep==="function"){value=rep.call(holder,key,value)}switch(typeof value){case"string":return quote(value);case"number":return isFinite(value)?String(value):"null";case"boolean":case"null":return String(value);case"object":if(!value){return"null"}gap+=indent;partial=[];if(typeof value.length==="number"&&!(value.propertyIsEnumerable("length"))){length=value.length;for(i=0;i<length;i+=1){partial[i]=str(i,value)||"null"}v=partial.length===0?"[]":gap?"[\n"+gap+partial.join(",\n"+gap)+"\n"+mind+"]":"["+partial.join(",")+"]";gap=mind;return v}if(rep&&typeof rep==="object"){length=rep.length;for(i=0;i<length;i+=1){k=rep[i];if(typeof k==="string"){v=str(k,value,rep);if(v){partial.push(quote(k)+(gap?": ":":")+v)}}}}else{for(k in value){if(Object.hasOwnProperty.call(value,k)){v=str(k,value,rep);if(v){partial.push(quote(k)+(gap?": ":":")+v)}}}}v=partial.length===0?"{}":gap?"{\n"+gap+partial.join(",\n"+gap)+"\n"+mind+"}":"{"+partial.join(",")+"}";gap=mind;return v}}return{stringify:function(value,replacer,space){var i;gap="";indent="";if(typeof space==="number"){for(i=0;i<space;i+=1){indent+=" "}}else{if(typeof space==="string"){indent=space}}rep=replacer;if(replacer&&typeof replacer!=="function"&&(typeof replacer!=="object"||typeof replacer.length!=="number")){throw new Error("JSON.stringify")}return str("",{"":value})},parse:function(text,reviver){var j;function walk(holder,key){var k,v,value=holder[key];if(value&&typeof value==="object"){for(k in value){if(Object.hasOwnProperty.call(value,k)){v=walk(value,k);if(v!==undefined){value[k]=v}else{delete value[k]}}}}return reviver.call(holder,key,value)}cx.lastIndex=0;if(cx.test(text)){text=text.replace(cx,function(a){return"\\u"+("0000"+(+(a.charCodeAt(0))).toString(16)).slice(-4)})}if(/^[\],:{}\s]*$/.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g,"@").replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g,"]").replace(/(?:^|:|,)(?:\s*\[)+/g,""))){j=eval("("+text+")");return typeof reviver==="function"?walk({"":j},""):j}throw new SyntaxError("JSON.parse")}}}()};

//ektron component manager                
if ("undefined" == typeof(Ektron)) { Ektron = {}; }
if ("undefined" == typeof(Ektron.ComponentManager)) {
    Ektron.ComponentManager = {
        //properties
        initialized: false,
        input: {},
        isAjax: false,
        head: {},
        
        //methods
        Components: {
            Css: {
                //properties
                aggregated: [],
                loaded: [],
                
                //methods
                add: function(){
                    for (var c = 0;c < this.loaded.length; c++){
                        if (this.loaded[c].IsAggregated){
                            this.aggregated.push(this.loaded[c]);
                            if (c == this.loaded.length - 1){
                                Ektron.ComponentManager.Dom.Append.link(this.aggregated[0]);
                            }
                        } else {
                            if (this.aggregated.length > 0){
                                Ektron.ComponentManager.Dom.Append.link(this.aggregated[0]);
                            }
                            Ektron.ComponentManager.Dom.Append.link(this.loaded[c]);
                        }
                    }
                    this.clear();
                },
                clear: function(){
                    //clear arrays
                    this.loaded = [];
                    this.aggregated = [];
                },
                isRegistered: function(id, registeredComponents){
                    var isCssRegistered = false;
                    if ("undefined" != typeof(registeredComponents.Css)){
                        for (var a = 0; a < registeredComponents.Css.length; a++) {
                            if(registeredComponents.Css[a].Id == id){
                                isCssRegistered = true;
                                break;
                            }
                        }
                    }
                    return isCssRegistered;
                }
            },
            Js: {
                //properties
                aggregated: [],
                loaded: [],
                
                //methods
                add: function(){
                    for (var j = 0;j < this.loaded.length; j++){
                        var isLast = (j == this.loaded.length - 1) ? true : false;
                        if (this.loaded[j].Id != "EktronComponentManagerJS"){
                        
                            if (this.loaded[j].IsAggregated){
                                this.aggregated.push(this.loaded[j]);
                                if (isLast){
                                    Ektron.ComponentManager.Dom.Append.script(this.aggregated[0]);
                                    this.clear();
                                }
                            } else {
                                if (this.aggregated.length > 0){
                                    Ektron.ComponentManager.Dom.Append.script(this.aggregated[0]);
                                    if (Ektron.ComponentManager.isAjax) {
                                        this.pause();
                                        break;
                                    }
                                }
                                Ektron.ComponentManager.Dom.Append.script(this.loaded[j]);
                                this.loaded.splice(j, 1);
                                if (isLast){
                                    this.clear();
                                } else {
                                    if (Ektron.ComponentManager.isAjax) {
                                        this.pause();
                                        break;
                                    }
                                }
                            }
                            
                        }
                    }
                },
                clear: function(){
                    this.loaded = [];
                    this.aggregated = [];                    
                },
                isRegistered: function(id, registeredComponents){
                    var isJsRegistered = false;
                    if ("undefined" != typeof(registeredComponents.Js)){
                        for (var a = 0; a < registeredComponents.Js.length; a++) {
                            if(registeredComponents.Js[a].Id == id){
                                isJsRegistered = true;
                                break;
                            }
                        }
                    }
                    return isJsRegistered;
                },
                loadComplete: function(){
                    if (this.loaded.length > 0) {
                        this.add();
                    } else {
                        if ("undefined" !== typeof($ektron)){
                            $ektron(document).trigger("EktronReady", ["ComponentManagerReady"]);
                        }
                    }
                },
                pause: function(){
                    for (var a = 0;a < this.aggregated.length; a++){
                        for (var b = 0;b < this.loaded.length; b++){
                            if (this.aggregated[a].Id == this.loaded[b].Id) {
                                this.loaded.splice(b, 1);
                                break;
                            }
                        }
                    }
                    this.aggregated = [];
                }
            },
            load: function(){
                this.Css.add();
                this.Js.add();
            }
        },
        Data: {
            load: function(){
                //get scripts
                var scripts = Ektron.ComponentManager.Dom.getNodes();
                
                //if the number of scripts does not euqal data length, hang out...
                if ("undefined" == typeof(Ektron.ComponentManagerData)) { Ektron.ComponentManagerData = []; }
                if (scripts.length > Ektron.ComponentManagerData.length){
                    var timeout = 120000;
                    var counter = 0;
                    var clearTimeout = false;
                    var isReady = false;
                    var intervalId = setInterval(function(){
                        if (counter == timeout){ alert("Ektron Component Manager: cannot load components!");clearTimeout = true; }
                        if (scripts.length <= Ektron.ComponentManagerData.length){ isReady = true; clearTimeout = true; }
                        if (clearTimeout) { clearInterval(intervalId); }
                        if (isReady) { Ektron.ComponentManager.Data.ready(scripts); }
                    }, 10);
                } else {
                    Ektron.ComponentManager.Data.ready(scripts);
                }
            },
            ready: function(scripts){
                //remove script tags (if necessary)
                for (var a = 0; a < scripts.length; a++) {
                    //remove script node
                    Ektron.ComponentManager.Dom.removeNode(scripts[a].id);
                }
                //process loaded data
                for (var b = 0; b < Ektron.ComponentManagerData.length; b++) {
                    //ensure all components are unique
                    this.uniquify(Ektron.ComponentManagerData[b].data);
                }
                
                if (Ektron.ComponentManager.isAjax){
                    //load components
                    Ektron.ComponentManager.Components.load();
                    
                    //remove EktronComponentManagerLoader if necessary
                    var loader = document.getElementById("EktronComponentManagerLoader");
                    if (loader !== null) { Ektron.ComponentManager.Dom.removeNode(loader); }
                } else {
                    Ektron.ComponentManager.Components.Css.clear();
                    Ektron.ComponentManager.Components.Js.clear();
                }                  
            },
            uniquify: function(json){
                var registeredComponents = Ektron.JSON.parse(Ektron.ComponentManager.input.value);
                
                //register css
                if ("undefined" != typeof(json.Css)){
                    for (var a = 0; a < json.Css.length; a++) {
                        if (!Ektron.ComponentManager.Components.Css.isRegistered(json.Css[a].Id, registeredComponents)){
                            Ektron.ComponentManager.Components.Css.loaded.push(json.Css[a]);
                            this.update(json.Css[a].Id, "css", registeredComponents);
                        } 
                    }
                }
                
                //register js
                if ("undefined" != typeof(json.Js)){
                    for (var b = 0; b < json.Js.length; b++) {
                        if (!Ektron.ComponentManager.Components.Js.isRegistered(json.Js[b].Id, registeredComponents)){
                            Ektron.ComponentManager.Components.Js.loaded.push(json.Js[b]);
                            this.update(json.Js[b].Id, "js", registeredComponents);
                        } 
                    }
                }          
            },
            update: function(id, type, registeredComponents){
                var component = { Id: id };
                switch(type){
                    case "css":
                        if ("undefined" == typeof(registeredComponents.Css)){ registeredComponents.Css = []; }
                        registeredComponents.Css.push(component); 
                        break;
                    case "js":
                        if ("undefined" == typeof(registeredComponents.Js)){ registeredComponents.Js = []; }
                        registeredComponents.Js.push(component);
                        break;
                }
                //set json data to component manager hidden input (list of loaded js/css ids)
                Ektron.ComponentManager.input.value = Ektron.JSON.stringify(registeredComponents);
            }
        },
        Dom: {
            //properties
            componentNodes: 0,
        
            //objects
            Append: {
                link: function(css){
                    var link = document.createElement("link");
                    link.setAttribute("type", "text/css");
                    link.setAttribute("rel", "stylesheet");
                    
                    var href = css.Href;
                    var media = css.Media;
                    var querystring = "";
                    
                    if (css.IsAggregated) {
                        media = "screen";
                        for (var l = 0;l < Ektron.ComponentManager.Components.Css.aggregated.length; l++){
                            if (l === 0) { querystring = "?id="; }
                            if (l > 0) { querystring = querystring + "+"; }
                            querystring = querystring + Ektron.ComponentManager.Components.Css.aggregated[l].Id;
                        }
                        Ektron.ComponentManager.Components.Css.aggregated = [];
                    }
                    
                    
                    link.setAttribute("media", media);
                    link.setAttribute("href", href + querystring);
                    Ektron.ComponentManager.head.appendChild(link);
                },
                script: function(js){        
                    var script = document.createElement("script");
                    script.setAttribute("type", "text/javascript");
                    script.onreadystatechange= function () {
                      if (this.readyState == 'complete') { Ektron.ComponentManager.Components.Js.finished; }
                    }
                    script.onload = Ektron.ComponentManager.Components.Js.finished;
                    
                    var src = js.Src;
                    var querystring = "?id=";
                    
                    if (js.IsAggregated) {
                        for (var s = 0;s < Ektron.ComponentManager.Components.Js.aggregated.length; s++){
                            if (s > 0) { querystring = querystring + "+"; }
                            querystring = querystring + Ektron.ComponentManager.Components.Js.aggregated[s].Id;
                        }
                    } else {
                        querystring = querystring + js.Id;
                    }
                    if (Ektron.ComponentManager.isAjax) { querystring = querystring + "&isAjax=true"; }
                    
                    script.setAttribute("src", src + querystring);
                    Ektron.ComponentManager.head.appendChild(script);
                }
            },
            getNodes: function(){
                var nodes = [];
                var scripts = document.getElementsByTagName("script");
                for (var a = 0; a < scripts.length; a++) {
                    if (scripts[a].id.indexOf("EktronComponentManagerData_", 0) !== -1) {
                        nodes.push(scripts[a]);
                    }
                }
                return nodes;
            },
            removeNode: function(id){
                var script = document.getElementById(id);
                var parent = script.parentNode;
                parent.removeChild(script);
            },
            setNodes: function(){
                //set component manager dom nodes
                var input = document.getElementById("EktronComponentManager");
                if (input === null){
                    input = document.createElement("input"); 
                    input.setAttribute("id", "EktronComponentManager");
                    input.setAttribute("name", "EktronComponentManager");
                    input.setAttribute("class", "EktronComponentManager");
                    input.setAttribute("type", "hidden");
                    input.setAttribute("value", "{}");
                    document.getElementsByTagName("form")[0].appendChild(input);
                    input = document.getElementById("EktronComponentManager");
                }
                Ektron.ComponentManager.input = input;
                Ektron.ComponentManager.head = document.getElementsByTagName("head")[0];
            }
        },
        init: function(isAjax) {
            if (Ektron.ComponentManager.initialized === false){
                 //bind to ms-ajax if necessary
                if (typeof(Sys) != "undefined") { 
                    if (typeof(Sys.WebForms) != "undefined") { 
                        if (typeof(Sys.WebForms.PageRequestManager) != "undefined") {
                            //bind events
                            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(Ektron.ComponentManager.Microsoft.MsAjax.initializeRequest);
                            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.ComponentManager.Microsoft.MsAjax.endRequest);
                        }
                    }
                }
                //set dom nodes
                Ektron.ComponentManager.Dom.setNodes();
                
                //set initialized property to true
                Ektron.ComponentManager.initialized = true;                 
            }
            
            //set ajax flag
            Ektron.ComponentManager.isAjax = isAjax;
                
            //load data
            Ektron.ComponentManager.Data.load();
        },
        Microsoft: {
            ICallBackEventHandler: {
                post: function(controlId, args){},
                getPostParamName: function() { return "EktronComponentManager"; },
                getPostParamValue: function() { return Ektron.ComponentManager.input.value; }
            },
            MsAjax: {
                initializeRequest: function(sender, args){
                    //args = args + "EktronComponentManager=" + Ektron.ComponentManager.input.value;
                },
                endRequest: function(sender, args){
                    Ektron.ComponentManager.init(true);
                }
            }
        }
    };
}



//initialize on document ready if loaded on page load
window.onDomReady = EktronComponentManagerReady;
function EktronComponentManagerReady(fn){
    if(document.addEventListener){
        document.addEventListener("DOMContentLoaded", fn, false);
    }else{
        document.onreadystatechange = function(){EktronComponentManagerReadyState(fn)}
    }
}
function EktronComponentManagerReadyState(fn) {
    if(document.readyState == "interactive"){
        fn();
    }
}
window.onDomReady(onEktronComponentManagerReadyState);
function onEktronComponentManagerReadyState(){
    Ektron.ComponentManager.init(false);
}