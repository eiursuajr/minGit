// MSDropDown - jquery.dd.js
// author: Marghoob Suleman
// Date: 12th Aug, 2009
// Version: 2.1 {date: 3rd Sep 2009}
// Revision: 25
// web: www.giftlelo.com | www.marghoobsuleman.com
;(function($ektron) {
	var oldDiv = "";
	$ektron.fn.dd = function(options) {
		$this =  this;
		options = $ektron.extend({
			height:120,
			visibleRows:7,
			rowHeight:23,
			showIcon:true,
			zIndex:9999,
			style:''
		}, options);
		var selectedValue = "";
		var actionSettings ={};
		actionSettings.insideWindow = true;
		actionSettings.keyboardAction = false;
		actionSettings.currentKey = null;
		var ddList = false;
		config = {postElementHolder:'_msddHolder', postID:'_msdd', postTitleID:'_title',postTitleTextID:'_titletext',postChildID:'_child',postAID:'_msa',postOPTAID:'_msopta',postInputID:'_msinput', postArrowID:'_arrow', postInputhidden:'_inp'};
		styles = {dd:'dd', ddTitle:'ddTitle', arrow:'arrow', ddChild:'ddChild', disbaled:.30};
		attributes = {actions:"onfocus,onblur,onchange,onclick,ondblclick,onmousedown,onmouseup,onmouseover,onmousemove,onmouseout,onkeypress,onkeydown,onkeyup", prop:"size,multiple,disabled,tabindex"};
		var elementid = $ektron(this).attr("id");
		var inlineCSS = $ektron(this).attr("style");
		options.style += (inlineCSS==undefined) ? "" : inlineCSS;
		var allOptions = $ektron(this).children();
		ddList = ($ektron(this).attr("size")>0 || $ektron(this).attr("multiple")==true) ? true : false;
		if(ddList) {options.visibleRows = $ektron(this).attr("size");};
		var a_array = {};//stores id, html & value etc
		//create wrapper
		createDropDown();
	
	function getPostID(id) {
		return elementid+config[id];
	};
	function getOptionsProperties(option) {
		var currentOption = option;
		var styles = $ektron(currentOption).attr("style");
		return styles;
	};
	function matchIndex(index) {
		var selectedIndex = $ektron("#"+elementid+" option:selected");
		if(selectedIndex.length>1) {
			for(var i=0;i<selectedIndex.length;i++) {
				if(index == selectedIndex[i].index) {
					return true;
				};
			};
		} else if(selectedIndex.length==1) {
			if(selectedIndex[0].index==index) {
				return true;
			};
		};
		return false;
	}
	function createATags() {
		var childnodes = allOptions;
		var aTag = "";
		var aidfix = getPostID("postAID");
		var aidoptfix = getPostID("postOPTAID");
		childnodes.each(function(current){
								 var currentOption = childnodes[current];
								 //OPTGROUP
								 if(currentOption.nodeName == "OPTGROUP") {
								  	aTag += "<div class='opta'>";
									 aTag += "<span style='font-weight:bold;font-style:italic; clear:both;'>"+$ektron(currentOption).attr("label")+"</span>";
									 var optChild = $ektron(currentOption).children();
									 optChild.each(function(currentopt){
															var currentOptOption = optChild[currentopt];
															 var aid = aidoptfix+"_"+(current)+"_"+(currentopt);
															 var arrow = $ektron(currentOptOption).attr("title");
															 arrow = (arrow.length==0) ? "" : '<img src="'+arrow+'" align="left" /> ';
															 var sText = $ektron(currentOptOption).text();
															 var sValue = $ektron(currentOptOption).val();
															 var sEnabledClass = ($ektron(currentOptOption).attr("disabled")==true) ? "disabled" : "enabled";
															 a_array[aid] = {html:arrow + sText, value:sValue, text:sText, index:currentOptOption.index, id:aid};
															 var innerStyle = getOptionsProperties(currentOptOption);
															 if(matchIndex(currentOptOption.index)==true) {
																 aTag += '<a href="javascript:void(0);" class="selected '+sEnabledClass+'"';
															 } else {
															 	aTag += '<a  href="javascript:void(0);" class="'+sEnabledClass+'"';
															 };
															 if(innerStyle!=false)
															 aTag +=  ' style="'+innerStyle+'"';
															 aTag +=  ' id="'+aid+'">';
															 aTag += arrow + sText+'</a>';															 
															});
									 aTag += "</div>";
									 
								 } else {
									 var aid = aidfix+"_"+(current);
									 var arrow = $ektron(currentOption).attr("title");
									 arrow = (arrow.length==0) ? "" : '<img src="'+arrow+'" align="left" /> ';									 
									 var sText = $ektron(currentOption).text();
									 var sValue = $ektron(currentOption).val();
									 var sEnabledClass = ($ektron(currentOption).attr("disabled")==true) ? "disabled" : "enabled";
									 a_array[aid] = {html:arrow + sText, value:sValue, text:sText, index:currentOption.index, id:aid};
									 var innerStyle = getOptionsProperties(currentOption);
									 if(matchIndex(currentOption.index)==true) {
										 aTag += '<a href="javascript:void(0);" class="selected '+sEnabledClass+'"';
									 } else {
										aTag += '<a  href="javascript:void(0);" class="'+sEnabledClass+'"';
									 };
									 if(innerStyle!=false)
									 aTag +=  ' style="'+innerStyle+'"';
									 aTag +=  ' id="'+aid+'">';
									 aTag += arrow + sText+'</a>';															 
								 };
								 });
		return aTag;
	};
	function createChildDiv() {
		var id = getPostID("postID");
		var childid = getPostID("postChildID");
		var sStyle = options.style;
		sDiv = "";
		sDiv += '<div id="'+childid+'" class="'+styles.ddChild+'"';
		if(!ddList) {
			sDiv += (sStyle!="") ? ' style="'+sStyle+'"' : ''; 
		} else {
			sDiv += (sStyle!="") ? ' style="border-top:1px solid #c3c3c3;display:block;position:relative;'+sStyle+'"' : ''; 
		}
		sDiv += '>';		
		return sDiv;
	};

	function createTitleDiv() {
		var titleid = getPostID("postTitleID");
		var arrowid = getPostID("postArrowID");
		var titletextid = getPostID("postTitleTextID");
		var inputhidden = getPostID("postInputhidden");
		var sText = $ektron("#"+elementid+" option:selected").text();
		var arrow = $ektron("#"+elementid+" option:selected").attr("title");
		arrow = (arrow.length==0 || arrow==undefined || options.showIcon==false) ? "" : '<img src="'+arrow+'" align="left" /> ';
		var sDiv = '<div id="'+titleid+'" class="'+styles.ddTitle+'"';
		sDiv += '>';
		sDiv += '<span id="'+arrowid+'" class="'+styles.arrow+'"></span><span class="textTitle" id="'+titletextid+'">'+arrow + sText+'</span></div>';
		return sDiv;
	};
	function createDropDown() {
		var changeInsertionPoint = false;
		var id = getPostID("postID");
		var titleid = getPostID("postTitleID");
		var titletextid = getPostID("postTitleTextID");
		var childid = getPostID("postChildID");
		var arrowid = getPostID("postArrowID");
		var iWidth = $ektron("#"+elementid).width();
		var sStyle = options.style;
		if($ektron("#"+id).length>0) {
			$ektron("#"+id).remove();
			changeInsertionPoint = true;
		}
		var sDiv = '<div id="'+id+'" class="'+styles.dd+'"';
		sDiv += (sStyle!="") ? ' style="'+sStyle+'"' : '';
		sDiv += '>';
		//create title bar
		if(!ddList)
		sDiv += createTitleDiv();
		//create child
		sDiv += createChildDiv();
		sDiv += createATags();
		sDiv += "</div>";
		sDiv += "</div>";
		if(changeInsertionPoint==true) {
			var sid =getPostID("postElementHolder");
			$ektron("#"+sid).after(sDiv);
		} else {
			$ektron("#"+elementid).after(sDiv);
		}
		$ektron("#"+id).css("width", iWidth+"px");
		$ektron("#"+childid).css("width", (iWidth-2)+"px");
		if(allOptions.length>options.visibleRows) {
			var margin = parseInt($ektron("#"+childid+" a:first").css("padding-bottom")) + parseInt($ektron("#"+childid+" a:first").css("padding-top"));
			var iHeight = ((options.rowHeight)*options.visibleRows) - margin;
			$ektron("#"+childid).css("height", iHeight+"px");
		}
		//set out of vision
		if(changeInsertionPoint==false) {
			setOutOfVision();
			addNewEvents(elementid);
		}
		if($ektron("#"+elementid).attr("disabled")==true) {
			$ektron("#"+id).css("opacity", styles.disbaled);
		} else {
			applyEvents();
			//add events
			//arrow hightlight
			if(!ddList) {
				$ektron("#"+titleid).bind("mouseover", function(event) {
														  hightlightArrow(1);
														  });
				$ektron("#"+titleid).bind("mouseout", function(event) {
														  hightlightArrow(0);
														  });
			};
			//open close events
			$ektron("#"+childid+ " a.enabled").bind("click", function(event) {
														 event.preventDefault();
														 manageSelection(this);
														 if(!ddList) {
															 $ektron("#"+childid).unbind("mouseover");
															 setInsideWindow(false);															 
															 var sText = (options.showIcon==false) ? $ektron(this).text() : $ektron(this).html();
															  setTitleText(sText);
															  closeMe();
														 };
														 setValue();
														 //actionSettings.oldIndex = a_array[$(this).attr("id")].index;
														 });
			$ektron("#"+childid+ " a.disabled").css("opacity", styles.disbaled);
			if(ddList) {
				$ektron("#"+childid).bind("mouseover", function(event) {if(!actionSettings.keyboardAction) {
																	 actionSettings.keyboardAction = true;
																	 $ektron(document).bind("keydown", function(event) {
																										var keyCode = event.keyCode;	
																										actionSettings.currentKey = keyCode;
																										if(keyCode==39 || keyCode==40) {
																											//move to next
																											event.preventDefault(); event.stopPropagation();
																											next();
																											setValue();
																										};
																										if(keyCode==37 || keyCode==38) {
																											event.preventDefault(); event.stopPropagation();
																											//move to previous
																											previous();
																											setValue();
																										};
																										  });
																	 
																	 }});
			};
			$ektron("#"+childid).bind("mouseout", function(event) {setInsideWindow(false);$ektron(document).unbind("keydown");actionSettings.keyboardAction = false;actionSettings.currentKey=null;});
			if(!ddList) {
				$ektron("#"+titleid).bind("click", function(event) {
													  setInsideWindow(false);
														if($ektron("#"+childid+":visible").length==1) {
															$ektron("#"+childid).unbind("mouseover");
														} else {
															$ektron("#"+childid).bind("mouseover", function(event) {setInsideWindow(true);});
															openMe();
														};
													  });
			};
			$ektron("#"+titleid).bind("mouseout", function(evt) {
													 setInsideWindow(false);
													 })
		};
	};
	function getByIndex(index) {
		for(var i in a_array) {
			if(a_array[i].index==index) {
				return a_array[i];
			}
		}
	}
	function manageSelection(obj) {
		var childid = getPostID("postChildID");
		if(!ddList) {
			$ektron("#"+childid+ " a.selected").removeClass("selected");
		} 
		var selectedA = $ektron("#"+childid + " a.selected").attr("id");
		if(selectedA!=undefined) {
			var oldIndex = (actionSettings.oldIndex==undefined || actionSettings.oldIndex==null) ? a_array[selectedA].index : actionSettings.oldIndex;
		};
		if(obj && !ddList) {
			$ektron(obj).addClass("selected");
		};				
		if(ddList) {
			var keyCode = actionSettings.currentKey;
			if($ektron("#"+elementid).attr("multiple")==true) {
				if(keyCode == 17) {
					//control
						actionSettings.oldIndex = a_array[$ektron(obj).attr("id")].index;
						$ektron(obj).toggleClass("selected");
					//multiple
				} else if(keyCode==16) {
					$ektron("#"+childid+ " a.selected").removeClass("selected");
					$ektron(obj).addClass("selected");
					//shift
					var currentSelected = $ektron(obj).attr("id");
					var currentIndex = a_array[currentSelected].index;
					for(var i=Math.min(oldIndex, currentIndex);i<=Math.max(oldIndex, currentIndex);i++) {
						$ektron("#"+getByIndex(i).id).addClass("selected");
					}
				} else {
					$ektron("#"+childid+ " a.selected").removeClass("selected");
					$ektron(obj).addClass("selected");
					actionSettings.oldIndex = a_array[$ektron(obj).attr("id")].index;
				};
			} else {
					$ektron("#"+childid+ " a.selected").removeClass("selected");
					$ektron(obj).addClass("selected");
					actionSettings.oldIndex = a_array[$ektron(obj).attr("id")].index;				
			};
		};		
	};
	function addNewEvents(id) {
		document.getElementById(id).refresh = function(e) {
			$ektron("#"+this.id).dd(options);
		};
	};
	function setInsideWindow(val) {
		actionSettings.insideWindow = val;
	};
	function getInsideWindow() {
		return actionSettings.insideWindow;
	};
	function applyEvents() {
		var mainid = getPostID("postID");
		var actions_array = attributes.actions.split(",");
		for(var iCount=0;iCount<actions_array.length;iCount++) {
			var action = actions_array[iCount];
			var actionFound = $ektron("#"+elementid).attr(action);
			if(actionFound!=undefined) {
				switch(action) {
					case "onfocus": 
					$ektron("#"+mainid).bind("mouseenter", function(event) {
													   document.getElementById(elementid).focus();
													   });
					break;
					case "onclick": 
					$ektron("#"+mainid).bind("click", function(event) {
													   document.getElementById(elementid).onclick();
													   });
					break;
					case "ondblclick": 
					$ektron("#"+mainid).bind("dblclick", function(event) {
													   document.getElementById(elementid).ondblclick();
													   });
					break;
					case "onmousedown": 
					$ektron("#"+mainid).bind("mousedown", function(event) {
													   document.getElementById(elementid).onmousedown();
													   });
					break;
					case "onmouseup": 
					//has in closeMe mthod
					$ektron("#"+mainid).bind("mouseup", function(event) {
													   document.getElementById(elementid).onmouseup();
													   //setValue();
													   });
					break;
					case "onmouseover": 
					$ektron("#"+mainid).bind("mouseover", function(event) {
													   document.getElementById(elementid).onmouseover();
													   });
					break;
					case "onmousemove": 
					$ektron("#"+mainid).bind("mousemove", function(event) {
													   document.getElementById(elementid).onmousemove();
													   });
					break;
					case "onmouseout": 
					$ektron("#"+mainid).bind("mouseout", function(event) {
													   document.getElementById(elementid).onmouseout();
													   });
					break;
				};
			};
		};
		
	};
	function setOutOfVision() {
		var sId = getPostID("postElementHolder");
		$ektron("#"+elementid).after("<div style='height:0px;overflow:hidden;position:absolute;' id='"+sId+"'></div>");
		$ektron("#"+elementid).appendTo($ektron("#"+sId));
	};
	function setTitleText(sText) {
		var titletextid = getPostID("postTitleTextID");
		$ektron("#"+titletextid).html(sText);
	};
	function next() {
		var titletextid = getPostID("postTitleTextID");
		var childid = getPostID("postChildID");
		var allAs = $ektron("#"+childid + " a.enabled");
		for(var current=0;current<allAs.length;current++) {
			var currentA = allAs[current];
			var id = $ektron(currentA).attr("id");
			if($ektron(currentA).hasClass("selected") && current<allAs.length-1) {
				$ektron("#"+childid + " a.selected").removeClass("selected");
				$ektron(allAs[current+1]).addClass("selected");
				//manageSelection(allAs[current+1]);
				var selectedA = $ektron("#"+childid + " a.selected").attr("id");
				if(!ddList) {
					var sText = (options.showIcon==false) ? a_array[selectedA].text : a_array[selectedA].html;
					setTitleText(sText);
				}
				if(parseInt(($ektron("#"+selectedA).position().top+$ektron("#"+selectedA).height()))>=parseInt($ektron("#"+childid).height())) {
					$ektron("#"+childid).scrollTop(($ektron("#"+childid).scrollTop())+$ektron("#"+selectedA).height()+$ektron("#"+selectedA).height());
				};
				break;
			};
		};
	};
	function previous() {
		var titletextid = getPostID("postTitleTextID");
		var childid = getPostID("postChildID");
		var allAs = $ektron("#"+childid + " a.enabled");
		for(var current=0;current<allAs.length;current++) {
			var currentA = allAs[current];
			var id = $ektron(currentA).attr("id");
			if($ektron(currentA).hasClass("selected") && current!=0) {
				$ektron("#"+childid + " a.selected").removeClass("selected");
				$ektron(allAs[current-1]).addClass("selected");				
				//manageSelection(allAs[current-1]);
				var selectedA = $ektron("#"+childid + " a.selected").attr("id");
				if(!ddList) {
					var sText = (options.showIcon==false) ? a_array[selectedA].text : a_array[selectedA].html;
					setTitleText(sText);
				}
				if(parseInt(($ektron("#"+selectedA).position().top+$ektron("#"+selectedA).height())) <=0) {
					$ektron("#"+childid).scrollTop(($ektron("#"+childid).scrollTop()-$ektron("#"+childid).height())-$ektron("#"+selectedA).height());
				};
				break;
			};
		};
	};
	function setValue() {
		var childid = getPostID("postChildID");
		var allSelected = $ektron("#"+childid + " a.selected");
		if(allSelected.length==1) {
			var sText = $ektron("#"+childid + " a.selected").text();
			var selectedA = $ektron("#"+childid + " a.selected").attr("id");
			if(selectedA!=undefined) {
				var sValue = a_array[selectedA].value;
				document.getElementById(elementid).selectedIndex = a_array[selectedA].index;
			};
		} else if(allSelected.length>1) { 
			var alls = $ektron("#"+elementid +" > option:selected").removeAttr("selected");
			for(var i=0;i<allSelected.length;i++) {
				var selectedA = $ektron(allSelected[i]).attr("id");
				var index = a_array[selectedA].index;
				document.getElementById(elementid).options[index].selected = "selected";
			};
		};
	};
	function openMe() {
		var childid = getPostID("postChildID");
		if(oldDiv!="" && childid!=oldDiv) { 
			$ektron("#"+oldDiv).slideUp("fast");
			$ektron("#"+oldDiv).css({zIndex:'0'});
		};
		if($ektron("#"+childid).css("display")=="none") {
			selectedValue = a_array[$ektron("#"+childid +" a.selected").attr("id")].text;
			$ektron(document).bind("keydown", function(event) {
													var keyCode = event.keyCode;											
													if(keyCode==39 || keyCode==40) {
														//move to next
														event.preventDefault(); event.stopPropagation();
														next();
													};
													if(keyCode==37 || keyCode==38) {
														event.preventDefault(); event.stopPropagation();
														//move to previous
														previous();
													};
													if(keyCode==27 || keyCode==13) {
														closeMe();
														setValue();
													};
													if($ektron("#"+elementid).attr("onkeydown")!=undefined) {
															document.getElementById(elementid).onkeydown();
														};														
													   });
			$ektron(document).bind("keyup", function(event) {
													if($ektron("#"+elementid).attr("onkeyup")!=undefined) {
														//$("#"+elementid).keyup();
														document.getElementById(elementid).onkeyup();
													};												 
												 });

					$ektron(document).bind("mouseup", function(evt){
															if(getInsideWindow()==false) {
															 closeMe();
															}
														 });													  
			$ektron("#"+childid).css({zIndex:options.zIndex});
			$ektron("#"+childid).slideDown("fast");
		if(childid!=oldDiv) {
			oldDiv = childid;
		}
		};
	};
	function closeMe() {
				var childid = getPostID("postChildID");
				$ektron(document).unbind("keydown");
				$ektron(document).unbind("keyup");
				$ektron(document).unbind("mouseup");
				$ektron("#"+childid).slideUp("fast", function(event) {
															checkMethodAndApply();
															$ektron("#"+childid).css({zIndex:'0'});
															});
		
	};
	function checkMethodAndApply() {
		var childid = getPostID("postChildID");
		if($ektron("#"+elementid).attr("onchange")!=undefined) {
			var currentSelectedValue = a_array[$ektron("#"+childid +" a.selected").attr("id")].text;
			if(selectedValue!=currentSelectedValue){document.getElementById(elementid).onchange();};
		}
		if($ektron("#"+elementid).attr("onmouseup")!=undefined) {
			document.getElementById(elementid).onmouseup();
		}
		if($ektron("#"+elementid).attr("onblur")!=undefined) { 
			$ektron(document).bind("mouseup", function(evt) {
												   $ektron("#"+elementid).focus();
												   $ektron("#"+elementid)[0].blur();
												   setValue();
												   $ektron(document).unbind("mouseup");
												});
		};
	};
	function hightlightArrow(ison) {
		var arrowid = getPostID("postArrowID");
		if(ison==1)
			$ektron("#"+arrowid).css({backgroundPosition:'0 100%'});
		else 
			$ektron("#"+arrowid).css({backgroundPosition:'0 0'});
	};
	};
	$ektron.fn.msDropDown = function(properties) {
		var dds = $ektron(this);
		for(var iCount=0;iCount<dds.length;iCount++) {
			var id = $ektron(dds[iCount]).attr("id");
			if(properties==undefined) {
				$ektron("#"+id).dd();
			} else {
				$ektron("#"+id).dd(properties);
			};
		};		
	};
})($ektron);