//-----------------------------------------------------------------------
// Copyright (C) Ektron, Inc. All rights reserved.
//-----------------------------------------------------------------------
// Map.js
// Ektron Map Framework.
// Resource for Virtual Earth & Google
//-----------------------------------------------------------------------

var map = null;
var geocoder = null;
//var map_cat_tree = null;
var btnSearchStr = '';
var btnDisplayStr = '';

var EMessage = new function() {
    this.Where = "Where e.g. Boston MA or Zip";
    this.What = "What e.g. Pizza";
    this.Who = "What e.g. Bill";
    this.Start = "Start address";
    this.End = "End address";
    this.NoStartNoEnd = 'You have not selected a starting point and ending point for the directions.\n\r Move the cursor to the left text box above the map, and enter the starting and ending street address, city, state or zip code.';
    this.NoStart = 'You have not selected a starting point for the directions.\n\r Move the cursor to the left text box above the map, and enter the starting street address, city, state or zip code.';
    this.NoEnd = 'You have not selected a ending point for the directions.\n\r Move the cursor to the right text box above the map, and enter the ending street address, city, state or zip code.';
    this.Validate = function(str) {
        var self = EMessage;
        var ret = str;
        if (str == self.Where || str == self.What || str == self.Who || str == self.Start || str == self.End) { ret = ''; }
        return ret;
    }
};
var EGlobal = new function() {
    this.Decode = function(str) {
        var self = EGlobal;
        var ret = str;
        ret = ret.replace(/\&lt;/g, '<');
        ret = ret.replace(/\&gt;/g, '>');
        ret = ret.replace(/\&quot;/g, '"');
        ret = ret.replace(/\&#39;/g, '\'');
        return ret;
    };
    this.Format = function(str, param) {
        var self = EGlobal;
        var ret = str;
        for (var i = 0; i < param.length; i++) {
            var regex = new RegExp("\\{" + i + "\\}", "g");
            ret = ret.replace(regex, param[i]);
        }
        return ret;
    };
    this.Replace = function(str, findstr, replacestr) {
        var ret = str;
        if (ret != "" && findstr != "") {
            var index = ret.indexOf(findstr);
            while (index >= 0) {
                ret = ret.replace(findstr, replacestr);
                index = ret.indexOf(findstr);
            }
        }
        return (ret);
    };
    this.AttachLoadEvent = function(func) {
        var self = EGlobal;
        var currentloadevent = window.onload;
        if (typeof window.onload != 'function') {
            window.onload = func;
        } else {
            window.onload = function() {
                if (currentloadevent) {
                    currentloadevent();
                }
                func();
            }
        }
    };
};
var ECookie = new function() {
    this.SetCookie = function(name, value) {
        if (name.length < 1)
            return;
        name = EMap.CookieUniqueId + name;
        if (0 < value.length) {
            document.cookie = "" + name + "=" + value;
        }
        else { document.cookie = name + "="; }

    };
    this.GetCookie = function(name) {
        var value = "";
        var index = 0;
        var oDoc = document;
        name = EMap.CookieUniqueId + name;
        if (oDoc.cookie)
            index = oDoc.cookie.indexOf(name + "=");
        else
            index = -1;

        if (index < 0) {
            value = "";
        }
        else {
            var countbegin = (oDoc.cookie.indexOf("=", index) + 1);
            if (0 < countbegin) {
                var countend = oDoc.cookie.indexOf(";", countbegin);
                if (countend < 0)
                    countend = oDoc.cookie.length;
                value = oDoc.cookie.substring(countbegin, countend);
            }
            else { value = ""; }
        }
        return value;
    };
    this.DeleteCookie = function(name) { document.cookie = name + "="; };
};
var EQueryString = new function() {
    this.Param = function(key) {
        var self = EQueryString;
        var value = null;
        for (var i = 0; i < self.Param.keys.length; i++) {
            if (self.Param.keys[i] == key) {
                value = self.Param.values[i];
                break;
            }
        }
        return value;
    };
    this.Param.keys = null;
    this.Param.values = null;
    this.Parse = function(args) {
        var self = EQueryString;
        self.Param.keys = new Array();
        self.Param.values = new Array();
        var query = args;
        var pairs = query.split("&");

        for (var i = 0; i < pairs.length; i++) {
            var pos = pairs[i].indexOf('=');
            if (pos >= 0) {
                var argname = unescape(pairs[i].substring(0, pos));
                var value = unescape(pairs[i].substring(pos + 1));
                self.Param.keys[self.Param.keys.length] = argname;
                self.Param.values[self.Param.values.length] = value;
            }
        }
    };
};
var ETemplate = function(path) {
    this.ImagePath = path + 'images/application/maps/';
    this.GeoDirection = '<input class="t" size="30" type="text" id="__StartPoint" name="__StartPoint" onfocus="EMap.ClearValue(\'__StartPoint\');" Onkeypress="return EMap.ValidateKey(event,\'__StartPoint\');" value="" /><a href="javascript:EMap.FlipAddress();"><img border="0"  src="' + path + 'images/application/maps/ddirflip.gif" alt="Switch start and end address." /></a><input class="t" size="30" type="text" id="__EndPoint" name="__EndPoint" onfocus="EMap.ClearValue(\'__EndPoint\');" Onkeypress="return EMap.ValidateKey(event,\'__EndPoint\');" value="" />&#160;<input class="b" type="button" id="__EkGeoButDirection" name="__EkGeoButDirection" value="Get Directions" onclick="EMap.GetRoute(null,null,null,null,null);"/>';
    this.Geosearch = '<input class="t" size="40" type="text" id="__SearchAddr" name="__SearchAddr" value="" onfocus="EMap.ClearValue(\'__SearchAddr\');" Onkeypress="return EMap.ValidateKey(event,\'__SearchAddr\');"/>&#160;';
    this.Geosearchbutton = '<input class="b" type="button" id="__GetAddr" name="__GetAddr" value="'+ btnSearchStr +'" onclick="EMap.FindAll(true);" />';
    this.Geocatsearch = '<input class="t" size="40" type="text" id="__SearchKey" name="__SearchKey" value="" onfocus="EMap.ClearValue(\'__SearchKey\');" Onkeypress="return EMap.ValidateKey(event,\'__SearchKey\');"/>';
    this.Geolocation = 'Get direction: <a href="{0}">To here</a> - <a href="{1}">From here</a>';
    this.Directiontemplate = '<tr><td style="background-color: #f6f6f6">{0}</td><td>{1}--({2})&#160;{3}</td></tr>';
    this.Directionaddresstemplate = '<tr><td class="input-box-text" nowrap>{0}</td><td>{1}</td></tr>';
    this.Markertemplate = '<tr><td></td></tr>';
    this.Searchresulttemplate = '<tr class="{0}"><td valign="top">{7}.&#160;</td><td  valign="top"><a href="{1}">{2}</a><br/>{5}</td><td align=right  valign="top">{8} {9}</td><td align=center width="5%"  valign="top"><a href="{3}"><img border="0"  src="' + path + 'images/application/maps/map.gif" alt="click here to view map"/></a></td><td align=center  valign="top"><a href="{4}"><img border="0"  src="' + path + 'images/application/maps/direction.gif" alt="Click here to get Directions."/></a></td></tr>';
    this.UserSearchresulttemplate = '<tr class="{0}"><td valign="top">{7}.&#160;</td><td  valign="top"><a href="{1}" target="_self" >{2}</a><br/>{5}</td><td align=right  valign="top">{8} {9}</td><td align=center width="5%"  valign="top"><a href="{3}"><img border="0"  src="' + path + 'images/application/maps/map.gif" alt="click here to view map"/></a></td><td align=center  valign="top"><a href="{4}"><img border="0"  src="' + path + 'images/application/maps/direction.gif" alt="Click here to get Directions."/></a></td></tr>';
    this.UserSearchresulttemplateNoLink = '<tr class="{0}"><td valign="top">{6}.&#160;</td><td  valign="top">{1}<br/>{4}</td><td align=right  valign="top">{7} {8}</td><td align=center width="5%"  valign="top"><a href="{2}"><img border="0"  src="' + path + 'images/application/maps/map.gif" alt="click here to view map"/></a></td><td align=center  valign="top"><a href="{3}"><img border="0"  src="' + path + 'images/application/maps/direction.gif" alt="Click here to get Directions."/></a></td></tr>';
    this.Categorytemplate = '<input class="b" type="button" id="__ShowCategory" onclick="EMap.MapCatMouseAction(\'over\');" Value="'+ btnDisplayStr +'"/>';
    this.Searchheader = 'Results {0} - {1} of {2} {3}<br/> {4}';
    this.Searchheader2 = 'No record exists{0}<br/> {1}';
};
var EMap = new function () {
    EVar.call(this);
    btnSearchStr = this.searchBtnText;
    btnDisplayStr = this.displayBtnText;
    ETemplate.call(this, this.AppPath);
    this.SearchLocation = "";
    this.SearchAddr = "";
    this.SearchText = "";
    if (this.DistanceUnit == 'Kilometers')
        this.DisplayTextUnit = "Km";
    else
        this.DisplayTextUnit = "Mi";
    this.PageCount = 1;
    this.InitOnZoom = true;
    this.RouteRequest = false;
    this.SearchRequest = false;
    this.CurrentPage = 1;
    this.TypeControlRef = null;
    this.ZoomControlRef = null;
    this.ItemArray = null;
    this.ZoomLocked = false;
    this.CurrentView = "search";
    this.CategorySearch = true;
    this.CategoryText = '';
    this.RouteInfo = null;
    this.CookieValue = '';
    this.PinId = 1;
    this.SetCategoryState = function (state) {
        var taxnomyvalues = $ektron("div#__MapCategoryTreePane input.searchTaxonomyPath");
        for (var count = 0; count < taxnomyvalues.length; count++) {
            taxnomyvalues[count].checked = state;
        }
    };
    this.ClearAll = function (c) {
        if (EMap.DateSearch) {
            var oForm = getFormElement(document.getElementById('__EndDate'));
            clearDTvalue(oForm.__EndDate, "__EndDateSpan", "[None]");
            oForm = getFormElement(document.getElementById('__StartDate'));
            clearDTvalue(oForm.__StartDate, "__StartDateSpan", "[None]");
        }
        if (EMap.MapXml != '') {
            EMap.CategorySearch = false;
            //map_cat_tree.setSubChecked(0, false);
            EMap.SetCategoryState(false);
            EMap.CategorySearch = true;
        }
        EMap.FindAll(false);
    };
    this.TimeSearch = function (c) {
        if (c == 1) {
            document.getElementById('__time2').style.display = "none";
            document.getElementById('__time1_label').innerHTML = "Date:";
            if (document.getElementById('__StartDate').value != '') EMap.FindAll();
        }
        else {
            document.getElementById('__time2').style.display = "block";
            document.getElementById('__time1_label').innerHTML = "Start Date:";
            if (document.getElementById('__StartDate').value != '' || document.getElementById('__EndDate').value != '') EMap.FindAll();
        }
    };
    this.InitTree = function () {
        if (EMap.DateSearch == true || (EMap.MapXml != '' && document.getElementById('catdv') != null)) {
            if (EMap.MapXml != '') {
                document.getElementById('catdv').style.display = "block";
                $ektron("#__MapCategoryTreePane").html(EMap.MapXml);
                $ektron("#taxonomyFilter").treeview({
                    collapsed: true
                });

                /*
                map_cat_tree = new dhtmlXTreeObject("__MapCategoryTreePane", "100%", "100%", 0);
                map_cat_tree.setImagePath(EMap.AppPath + "images/application/maps/tree/");
                map_cat_tree.enableCheckBoxes(true);
                map_cat_tree.enableTreeLines(false);
                map_cat_tree.loadXMLString(EMap.MapXml);
                map_cat_tree.closeAllItems(0);
                map_cat_tree.setOnCheckHandler(EMap.OnNodeSelect);
                */
            }
            if (EMap.DateSearch) {
                document.getElementById('timedv').style.display = "block";
                document.getElementById('__time1_value').innerHTML = EGlobal.Decode(EMap.StartDatePicker);
                document.getElementById('__time2_value').innerHTML = EGlobal.Decode(EMap.EndDatePicker);
                document.getElementById('timedv').style.display = "block";
            }
            document.getElementById('__CatCloseIcon').innerHTML = '<img src="' + EMap.ImagePath + 'close.gif" onclick="EMap.MapCatMouseAction(\'out\');"/>';
            EMap.SetStyle('__CategoryPane', 'block');
        }
    };
    this.OnNodeSelect = function () {
        if (EMap.CategorySearch) {
            EMap.FindAll(false);
        }
    };
    this.OnNodeAllSelect = function (state) {
        EMap.CategorySearch = false;
        //map_cat_tree.setSubChecked(0, state);
        EMap.SetCategoryState(state);
        EMap.CategorySearch = true;
        EMap.OnNodeSelect();
    };
    this.MapCatMouseAction = function (action) {
        var e = document.getElementById("MapSearchOption");
        if (action == 'over') {
            e.style.display = "inline";
        }
        else {
            e.style.display = "";
        }
    };
    this.EmailDirection = function () {
        var self = EMap;
        var bodymesage = '';
        if (self.RouteInfo != null) {
            bodymesage += "Start Address:" + self.RouteInfo.StartLocation.Address + "%0A";
            bodymesage += "End Address:" + self.RouteInfo.EndLocation.Address + "%0A";
            bodymesage += "Distance:" + self.RouteInfo.Itinerary.Distance + ' ' + self.RouteInfo.Itinerary.DistanceUnit + '(' + self.RouteInfo.Itinerary.Time + ')' + '%0A';
            var len = self.RouteInfo.Itinerary.Segments.length;
            for (var i = 0; i < len; i++) {
                bodymesage += i + 1 + '. ' + self.RouteInfo.Itinerary.Segments[i].Instruction + ' ' + self.RouteInfo.Itinerary.Segments[i].Distance + ' ' + self.RouteInfo.Itinerary.DistanceUnit + '%0A';
            }
        }
        window.open('mailto:?subject=Driving direction&body=' + bodymesage);
    };
    this.ShowLoadingMessage = function () {
        if (EMap.TextResultOn) {
            document.getElementById('__SearchTxtResultPane').innerHTML = "loading...";
        }
    };
    this.LoadMap = function () {
        if (EMap.SearchData != "content") {
            EMessage.What = EMessage.Who;
            if (document.getElementById('__findtitle') != null) { document.getElementById('__findtitle').innerHTML = "Find Who"; }
        }
        EMap.SetStyle('__Map', "block");

        if (EMap.MapCategory != '') {
            EMap.InitTree();
        }
        EMap.CookieValue = ECookie.GetCookie('mapcookie');
        var callfindall = true;
        if (EMap.EnableCatalog) {
            if ((!(EMap.CookieValue == null || EMap.CookieValue == '')) && (ECookie.GetCookie('mapinitaddr') == EMap.Address)) {
                callfindall = false;
                EQueryString.Parse(EMap.CookieValue);
                EMap.SearchText = EQueryString.Param("backsearchtext");
                EMap.CurrentView = EQueryString.Param("currentview");
                if (EMap.SearchText == null) { EMap.SearchText = ""; }
                EMap.SearchAddr = EQueryString.Param("searchaddr");
                if (EMap.SearchAddr == null) { EMap.SearchAddr = ""; }
                EMap.CurrentZoomLevel = decodeURIComponent(EQueryString.Param("currentzoomlevel"));
                EMap.CategorySearch = false;
                var ids = EQueryString.Param("categoryids");
                if (ids != "" && $ektron("div#__MapCategoryTreePane #taxonomyFilter").length > 0) {
                    EMap.CategoryText = EQueryString.Param("category");
                    //If this id is selected check that category in the tree
                    var idlist = ids.split(",");
                    if (idlist != null) {
                        for (var i = 0; i < idlist.length; i++) {
                            try {
                                var chkCategory = $ektron('div#__MapCategoryTreePane input[data-ektron-MapCategoryId="' + idlist[i] + '"]');
                                if (chkCategory !== null && chkCategory[0] !== null)
                                    chkCategory[0].checked = true;
                            }
                            catch (e) {
                                alert('Error while loading category tree.');
                            }
                        }
                    }
                }
                EMap.CategorySearch = true;
                if (EMap.CurrentView == null || EMap.CurrentView == "") {
                    EMap.CurrentView = "search";
                }
                EMap.SearchLocation = decodeURIComponent(EQueryString.Param("searchlocation"));
            }
            else {
                ECookie.SetCookie('mapinitaddr', EMap.Address);
            }
        }
        else {
            ECookie.SetCookie('mapcookie', ''); ECookie.SetCookie('mapinitaddr', ''); ECookie.SetCookie('cookiepageindex', '');
        }
        switch (EMap.Provider) {
            case 'virtualearth':
                try {
                    var style = "r";
                    switch (EMap.MapStyle) {
                        case "Satellite":
                            style = "a"; break;
                        case "Aerial":
                            style = "a"; break;
                        case "Hybrid":
                            style = "h"; break;
                        case "Road":
                            style = "r"; break;
                    }
                    map = new VEMap('__Map');
                    if (EMap.SearchLocation != "") {
                        try { eval('map.LoadMap(new VELatLong(' + EMap.SearchLocation + '),' + EMap.CurrentZoomLevel + ' ,\'' + style + '\' ,false)'); } catch (e) { }
                    }
                    else {
                        try {
                            if (EMap.Address == "") {
                                map.LoadMap(new VELatLong(EMap.Latitude, EMap.Longitude), EMap.CurrentZoomLevel, style, false);
                            } else {
                                EMap.SearchAddr = EMap.Address;
                                map.LoadMap(null, EMap.CurrentZoomLevel, style, false);
                            }
                        } catch (e) { }
                    }
                    if (EMap.TypeControl) {
                        map.ShowDashboard();
                    }
                    else {
                        if (document.getElementById('__TypeControl') != null) { document.getElementById('__TypeControl').innerHTML = ''; }
                        map.HideDashboard();
                    }
                    if (EMap.GeoControl) {
                        EMap.SetControl(EMap.CurrentView, false); EMap.SetStyle('__MapTab', 'block');
                        if (EMap.TypeControl) { EMap.SetStyle('__TypeControl', 'block'); }
                    }
                    try {
                        if (!(EMap.GeoControl)) { EMap.Geolocation = ''; }
                        if (callfindall) { EMap.FindAll(true); } else { __LoadMap(EMap.CookieValue, 'mode=search&pageindex=' + ECookie.GetCookie('cookiepageindex')); }
                        if (EMap.EnableCatalog) { window.setTimeout(EMap.AttachMapEvent, 1000); }
                    } catch (e) { }
                }
                catch (e) { }
                break;
            case 'google':
                try {
                    var _mStyle = "G_NORMAL_MAP";
                    switch (EMap.MapStyle) {
                        case "Satellite":
                            _mStyle = "G_SATELLITE_MAP"; break;
                        case "Hybrid":
                            _mStyle = "G_HYBRID_MAP"; break;
                        case "Road":
                            _mStyle = "G_NORMAL_MAP"; break;
                    }
                    var initgeocoder = false;
                    if (GBrowserIsCompatible()) {
                        map = new GMap2(document.getElementById("__Map"));
                        if (EMap.ZoomControl) {
                            EMap.ZoomControlRef = new GLargeMapControl();
                            map.addControl(EMap.ZoomControlRef);
                        }
                        if (EMap.TypeControl) {
                            EMap.TypeControlRef = new GMapTypeControl();
                            map.addControl(EMap.TypeControlRef);
                        }
                        geocoder = new GClientGeocoder();
                        var self = EMap;
                        if (EMap.SearchLocation != "") {
                            eval('map.setCenter(new GLatLng(' + EMap.SearchLocation + '),' + EMap.CurrentZoomLevel + ',' + _mStyle + ')');
                        }
                        else {
                            if (EMap.Address == "") {
                                eval('map.setCenter(new GLatLng(' + EMap.Latitude + ',' + EMap.Longitude + '),' + EMap.CurrentZoomLevel + ',' + _mStyle + ')');
                            }
                            else {
                                EMap.SearchAddr = EMap.Address;
                                initgeocoder = true;
                                geocoder.getLatLng(EMap.Address, function (newPoint) {
                                    if (newPoint != null) {
                                        try { eval('map.setCenter(' + newPoint + ',' + self.CurrentZoomLevel + ',' + _mStyle + ')') } catch (e) { };
                                    }
                                    if (EMap.GeoControl) {
                                        EMap.SetControl(EMap.CurrentView, false); EMap.SetStyle('__MapTab', 'block');
                                        if (EMap.ZoomControl) { EMap.SetStyle('__TypeControl', 'block'); }
                                    }
                                    try {
                                        if (!(EMap.GeoControl)) { EMap.Geolocation = ''; }
                                        if (callfindall) { EMap.FindAll(true); } else { __LoadMap(EMap.CookieValue, 'mode=search&pageindex=' + ECookie.GetCookie('cookiepageindex')); }
                                        if (EMap.EnableCatalog) { window.setTimeout(EMap.AttachMapEvent, 1000); }
                                    } catch (e) { }
                                });
                            }
                        }
                    }
                    if (initgeocoder == false) {
                        if (EMap.GeoControl) {
                            EMap.SetControl(EMap.CurrentView, false); EMap.SetStyle('__MapTab', 'block');
                            if (EMap.ZoomControl) { EMap.SetStyle('__TypeControl', 'block'); }
                        }
                        try {
                            if (!(EMap.GeoControl)) { EMap.Geolocation = ''; }
                            if (callfindall) { EMap.FindAll(true); } else { __LoadMap(EMap.CookieValue, 'mode=search&pageindex=' + ECookie.GetCookie('cookiepageindex')); }
                            if (EMap.EnableCatalog) { window.setTimeout(EMap.AttachMapEvent, 1000); }
                        } catch (e) { }
                    }
                }
                catch (e) { }
                break;
            default:
                alert(EMap.Provider + ' is not supported.');
                break;
        }
    };
    this.AttachMapEvent = function () {
        try {
            switch (EMap.Provider) {
                case "virtualearth":
                    map.AttachEvent("onerror", EMap.Error);
                    map.AttachEvent("onendzoom", EMap.FindAllOnVEZoom);
                    map.AttachEvent("ondoubleclick", EMap.FindAllOnVEZoom);
                    map.AttachEvent("onmouseup", EMap.FindAllOnVEZoom);
                    map.AttachEvent("onmousewheel", EMap.FindAllOnVEZoom);
                    break;
                case "google":
                    GEvent.addListener(map, "moveend", EMap.FindAllOnGZoom); break;
            }
            if (EMap.Address != "" && EMap.CookieValue == "") { EMap.SetZoomLevel(); EMap.Search(); }
        } catch (e) { }
    };
    this.FindAll = function (reset) {
        if (document.getElementById('__SearchAddr') != null) {
            EMap.SearchAddr = EMessage.Validate(document.getElementById('__SearchAddr').value);
        }
        if (EMap.EnableCatalog) {
            EMap.InitOnZoom = false;
            if (EMap.ZoomLocked) {
                EMap.CleanDirection();
            }
            EMap.SearchRequest = true;
            if (reset) {
                EMap.SearchLocation = "";
            }
            EMap.ShowLoadingMessage();
            EMap.Find(null, EMap.SearchAddr, null, EMap.Search);
        }
        else {
            EMap.FindOnline();
        }
    };
    this.FindUser = function (info) {
        if (document.getElementById('__SearchTxtResultPane') != null) { document.getElementById('__SearchTxtResultPane').innerHTML = ''; }

        EMap.SearchAddr = '';
        EMap.SearchText = '';
        EMap.InitOnZoom = false;
        EMap.ZoomLocked = true;
        EMap.EnableCatalog = false;
        EMap.ContentInfo = info;
        EMap.FindOnline();
    };
    this.ResetUser = function () {
        EMap.InitOnZoom = true;
        EMap.ZoomLocked = false;
        EMap.EnableCatalog = true;
        EMap.ContentInfo = '';
    };
    this.FindAllOnVEZoom = function (e) {
        if (!(EMap.RouteRequest)) {
            if (document.getElementById('__SearchAddr') != null) {
                EMap.SearchAddr = EMessage.Validate(document.getElementById('__SearchAddr').value);
            }
            EMap.CurrentZoomLevel = map.GetZoomLevel();
            if ((EMap.SearchRequest == false) && (EMap.InitOnZoom) && ((parseInt(EMap.CurrentZoomLevel) >= parseInt(EMap.MinZoomLevel)))) {
                EMap.SearchLocation = map.GetCenter().Latitude + "," + map.GetCenter().Longitude;
                EMap.ShowLoadingMessage();
                EMap.InitOnZoom = false;
                EMap.Find(null, EMap.SearchAddr, null, EMap.Search);
            }
        }
        if (!(EMap.ZoomLocked)) {
            EMap.Reset();
        }
        EMap.SearchRequest = false;
    };
    this.FindAllOnGZoom = function () {
        if (!(EMap.RouteRequest)) {
            if (document.getElementById('__SearchAddr') != null) {
                EMap.SearchAddr = EMessage.Validate(document.getElementById('__SearchAddr').value);
            }
            EMap.CurrentZoomLevel = map.getZoom();
            if ((EMap.SearchRequest == false) && (EMap.InitOnZoom) && ((parseInt(EMap.CurrentZoomLevel) >= parseInt(EMap.MinZoomLevel)))) {
                var e = map.getCenter();
                EMap.SearchLocation = e.lat() + "," + e.lng();
                EMap.ShowLoadingMessage();
                EMap.InitOnZoom = false;
                EMap.Find(null, EMap.SearchAddr, null, EMap.Search);
            }
        }
        if (!(EMap.ZoomLocked)) {
            EMap.Reset();
        }
        EMap.SearchRequest = false;
    };
    this.backzoomlevel = 1;
    this.backzoom = false;
    this.Find = function (what, where, index, callback) {
        EMap.CurrentZoomLevel = EMap.GetZoomLevel();
        EMap.InitOnZoom = false;
        EMap.backzoomlevel = EMap.CurrentZoomLevel;
        EMap.backzoom = EMap.InitOnZoom;
        if (callback == null) {
            callback = 'EMap.OnFindResults';
        }
        try {
            if (EMap.SearchLocation == "") {
                switch (EMap.Provider) {
                    case "virtualearth":
                        map.Find(what, where, null, null, null, null, null, null, null, null, EMap.OnFindResults);
                        break;
                    case "google":
                        geocoder.getLatLng(where, EMap.OnFindResults);
                        break;
                }
            }
            else {
                EMap.Search();
            }
        }
        catch (e) {
            EMap.Search();
        }
    };
    this.OnFindResults = function (results) {
        EMap.InitOnZoom = EMap.backzoom;
        EMap.CurrentZoomLevel = EMap.backzoomlevel;
        try {
            switch (EMap.Provider) {
                case 'virtualearth':
                    for (r = 0; r < results.length; r++) {
                        if (results[r].ID != null) {
                            EMap.SearchLocation = results[r].LatLong;
                            eval('EMap.SetCenter(' + EMap.SearchLocation + ')');
                            break;
                        }
                    }
                    break;
                case 'google':
                    if (results != null) {
                        EMap.SearchLocation = results.lat() + "," + results.lng();
                        eval('EMap.SetCenter(' + EMap.SearchLocation + ')');
                    }
                    break;
            }
        }
        catch (e) {
        }
        EMap.Search();
    };
    this.FindOnline = function () {
        EMap.CurrentZoomLevel = EMap.GetZoomLevel();
        EMap.backzoomlevel = EMap.CurrentZoomLevel;
        if (EMap.SearchAddr != '') {
            switch (EMap.Provider) {
                case "virtualearth":
                    map.Find(null, EMap.SearchAddr, null, null, null, null, null, null, null, null, EMap.ShowOnlineData);
                    break;
                case "google":
                    geocoder.getLatLng(EMap.SearchAddr, EMap.ShowOnlineData);
                    break;
            }
        }
        else {
            EMap.ShowOnlineData(null);
        }
    };
    this.ShowOnlineData = function (results) {
        EMap.CurrentZoomLevel = EMap.backzoomlevel;
        var pin;
        EMap.RemoveAllMarker();
        try {
            if (results != null) {
                switch (EMap.Provider) {
                    case 'virtualearth':
                        for (var r = 0; r < results.length; r++) {
                            if (results[r].ID != null) {
                                EMap.SearchLocation = results[r].LatLong;
                                eval('EMap.SetCenter(' + EMap.SearchLocation + ')');

                                var pin = new VEShape(VEShapeType.Pushpin, EMap.SearchLocation);
                                pin.SetTitle(EMap.SearchAddr);
                                pin.SetDescription('<br/>' + EGlobal.Format(EMap.Geolocation, new Array('javascript:EMap.SetAddress(\'' + EGlobal.Replace(EMap.SearchAddr, '#', ' ') + '\',\'to\');', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(EMap.SearchAddr, '#', ' ') + '\',\'from\');')));
                                map.AddShape(pin);
                                EMap.PinId++;
                            }
                        }
                        break;
                    case 'google':
                        EMap.SearchLocation = results.lat() + "," + results.lng();
                        eval('EMap.SetCenter(' + EMap.SearchLocation + ')');
                        var marker = new GMarker(results);
                        GEvent.addListener(marker, "mouseover", function () {
                            marker.openInfoWindowHtml('<div id="IW_' + EMap.PinId + '" style="overflow:auto;width:240px;height:100px;">' + EMap.SearchAddr + '<br/>' + EGlobal.Format(EMap.Geolocation, new Array('javascript:EMap.SetAddress(\'' + EGlobal.Replace(EMap.SearchAddr, '#', ' ') + '\',\'to\');', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(EMap.SearchAddr, '#', ' ') + '\',\'from\');')) + '</div>');
                        });
                        GEvent.addListener(marker, "infowindowopen", function () { EMap.InitOnZoom = false; });
                        GEvent.addListener(marker, "infowindowclose", function () { EMap.InitOnZoom = true; });
                        map.addOverlay(marker);
                        EMap.PinId++;
                        break;
                }
            }
            if (EMap.ContentInfo != '') {
                var data;
                try { data = eval('(' + EMap.ContentInfo + ')'); } catch (e) { data = EMap.ContentInfo; }
                var item = data.items[0];
                eval('EMap.ShowContent(' + parseInt(1) + ',\'\',' + item.Latitude + ',' + item.Longitude + ',\'' + item.Title + '\',\'' + item.QLink + '\',\'' + item.Summary + '\',\'' + item.Address + '\',\'' + item.Description + '\');');
                if (results == null) { eval('EMap.SetCenter(' + item.Latitude + ',' + item.Longitude + ')'); }
            }
            EMap.SetZoomLevel()
        }
        catch (e) {
        }
    };
    this.Error = function (e) {
        alert('Map server error:' + e.error);
    };
    this.CreateMapPoint = function (imageicon, point, title, qlink, summary, address, description) {
        var marker;
        var markerid = 1;
        var _summarytxt = EGlobal.Decode(summary);
        var _userTB = '';
        if (EMap.SearchData != 'content')
            _summarytxt = '<table><tr><td><img src=\'' + description + '\'/></td><td>' + _summarytxt + '</td></tr></table>';
        switch (EMap.Provider) {
            case 'virtualearth':
                if (EMap.SearchData != 'content') {
                    if (qlink && qlink.length > 0) {
                        _userTB = '<a href="' + qlink + '">' + title + '</a>';
                    }
                    else {
                        _userTB = title;
                    }

                    marker = new VEShape(VEShapeType.Pushpin, point);
                    marker.SetTitle(markerid + '.' + _userTB);
                    marker.SetDescription(_summarytxt + '<br/>' + EGlobal.Format(EMap.Geolocation, new Array('javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'to\');', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'from\');')));
                }
                else {
                    marker = new VEShape(VEShapeType.Pushpin, point);
                    marker.SetCustomIcon(imageicon);
                    marker.SetTitle(markerid + '. <a href=\'' + qlink + '\'><b>' + title + '</b></a>');
                    marker.SetDescription(_summarytxt + '<br/>' + EGlobal.Format(EMap.Geolocation, new Array('javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'to\');', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'from\');')));
                }
                break;
            case 'google':
                var icon = new GIcon();
                icon.image = imageicon;
                icon.iconSize = new GSize(20, 33);
                icon.iconAnchor = new GPoint(6, 20);
                icon.infoWindowAnchor = new GPoint(5, 1);

                marker = new GMarker(point, icon);
                GEvent.addListener(marker, "mouseover", function () {
                    if (EMap.SearchData != 'content') {
                        if (qlink && qlink.length > 0) {
                            _userTB = '<a href="' + qlink + '">' + title + '</a>';
                        }
                        else {
                            _userTB = title;
                        }
                        marker.openInfoWindowHtml('<div id="IW_' + markerid + '" style="overflow:auto;width:240px;height:100px;">' + markerid + '. ' + _userTB + _summarytxt + '<br/>' + EGlobal.Format(EMap.Geolocation, new Array('javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'to\');', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'from\');')) + '</div>');
                    }
                    else {
                        marker.openInfoWindowHtml('<div id="IW_' + markerid + '" style="overflow:auto;width:240px;height:100px;">' + markerid + '. <a href=\'' + qlink + '\'><b>' + title + '</b></a><br/>' + _summarytxt + '<br/>' + EGlobal.Format(EMap.Geolocation, new Array('javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'to\');', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'from\');')) + '</div>');
                    }

                });
                GEvent.addListener(marker, "infowindowopen", function () { EMap.InitOnZoom = false; });
                GEvent.addListener(marker, "infowindowclose", function () { EMap.InitOnZoom = true; });
                break;
        }
        return marker;
    };
    this.CreateMarker = function (markerid, highlight, point, title, qlink, summary, address, description) {
        var marker;
        var _summarytxt = EGlobal.Decode(summary);
        var _userTB = '';
        if (EMap.SearchData != 'content')
            _summarytxt = '<table><tr><td><img src=\'' + description + '\'/></td><td>' + _summarytxt + '</td></tr></table>';
        switch (EMap.Provider) {
            case 'virtualearth':
                if (this.DistanceUnit == 'Kilometers')
                    map.SetScaleBarDistanceUnit(VEDistanceUnit.Kilometers);
                else
                    map.SetScaleBarDistanceUnit(VEDistanceUnit.Miles);
                if (EMap.SearchData != 'content') {
                    if (qlink && qlink.length > 0) {
                        _userTB = '<a href="' + qlink + '">' + title + '</a>';
                    }
                    else {
                        _userTB = title;
                    }

                    marker = new VEShape(VEShapeType.Pushpin, point);
                    marker.SetCustomIcon(EMap.ImagePath + 'flag_' + markerid + highlight + '.gif');
                    marker.SetTitle(markerid + '.' + _userTB);
                    marker.SetDescription(_summarytxt + '<br/>' + EGlobal.Format(EMap.Geolocation, new Array('javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'to\');', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'from\');')));
                }
                else {
                    marker = new VEShape(VEShapeType.Pushpin, point);
                    marker.SetCustomIcon(EMap.ImagePath + 'flag_' + markerid + highlight + '.gif');
                    marker.SetTitle(markerid + '. <a href=\'' + qlink + '\'><b>' + title + '</b></a>');
                    marker.SetDescription(_summarytxt + '<br/>' + EGlobal.Format(EMap.Geolocation, new Array('javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'to\');', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'from\');')));
                }

                break;
            case 'google':
                var icon = new GIcon();
                icon.image = EMap.ImagePath + 'flag_' + markerid + highlight + '.gif';
                icon.iconSize = new GSize(20, 33);
                icon.iconAnchor = new GPoint(6, 20);
                icon.infoWindowAnchor = new GPoint(5, 1);

                marker = new GMarker(point, icon);
                GEvent.addListener(marker, "mouseover", function () {
                    if (EMap.SearchData != 'content') {
                        if (qlink && qlink.length > 0) {
                            _userTB = '<a href="' + qlink + '">' + title + '</a>';
                        }
                        else {
                            _userTB = title;
                        }
                        marker.openInfoWindowHtml('<div id="IW_' + markerid + '" style="overflow:auto;width:240px;height:100px;">' + markerid + '. ' + _userTB + _summarytxt + '<br/>' + EGlobal.Format(EMap.Geolocation, new Array('javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'to\');', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'from\');')) + '</div>');
                    }
                    else {
                        marker.openInfoWindowHtml('<div id="IW_' + markerid + '" style="overflow:auto;width:240px;height:100px;">' + markerid + '. <a href=\'' + qlink + '\'><b>' + title + '</b></a><br/>' + _summarytxt + '<br/>' + EGlobal.Format(EMap.Geolocation, new Array('javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'to\');', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(address, '#', ' ') + '\',\'from\');')) + '</div>');
                    }

                });
                GEvent.addListener(marker, "infowindowopen", function () { EMap.InitOnZoom = false; });
                GEvent.addListener(marker, "infowindowclose", function () { EMap.InitOnZoom = true; });
                break;
        }
        return marker;
    };
    this.GetZoomLevel = function () {
        switch (EMap.Provider) {
            case 'virtualearth':
                return (map.GetZoomLevel());
                break;
            case 'google':
                return (map.getZoom());
                break;
        }
    };
    this.RemoveAllMarker = function () {
        switch (EMap.Provider) {
            case 'virtualearth':
                map.DeleteAllPushpins();
                break;
            case 'google':
                return (map.clearOverlays());
                break;
        }
    };
    this.SetCenterIndex = function (index, lat, lon) {
        EMap.InitOnZoom = false;
        EMap.RemoveAllMarker();
        for (var i = 0; i < EMap.ItemArray.length; i++) {
            var item = EMap.ItemArray[i];
            if (index == (i + 1)) {
                eval('EMap.ShowContent(' + index + ',\'h\',' + item.Latitude + ',' + item.Longitude + ',\'' + item.Title + '\',\'' + item.QLink + '\',\'' + item.Summary + '\',\'' + item.Address + '\',\'' + item.Description + '\');');
            }
            else {
                eval('EMap.ShowContent(' + parseInt(i + 1) + ',\'\',' + item.Latitude + ',' + item.Longitude + ',\'' + item.Title + '\',\'' + item.QLink + '\',\'' + item.Summary + '\',\'' + item.Address + '\',\'' + item.Description + '\');');
            }
        }
        switch (EMap.Provider) {
            case 'virtualearth':
                map.SetCenter(new VELatLong(lat, lon));
                break;
            case 'google':
                map.setCenter(new GLatLng(lat, lon));
                break;
        }
        window.setTimeout("EMap.Reset", 1000);
    };
    this.SetCenter = function (lat, lon) {
        switch (EMap.Provider) {
            case 'virtualearth':
                map.SetCenter(new VELatLong(lat, lon));
                break;
            case 'google':
                map.setCenter(new GLatLng(lat, lon));
                break;
        }
    };
    this.RemoveMarker = function (id) {
        switch (EMap.Provider) {
            case 'virtualearth':
                map.DeletePushpin(id);
                break;
            case 'google':
                return (map.removeOverlay(id));
                break;
        }
    };
    this.ShowContent = function (marker, highlight, latitude, longitude, title, qlink, summary, address, description) {
        var point;
        switch (EMap.Provider) {
            case 'virtualearth':
                point = new VELatLong(latitude, longitude);
                var marker = EMap.CreateMarker(marker, highlight, point, title, qlink, summary, address, description);
                map.AddShape(marker);
                break;
            case 'google':
                point = new GLatLng(latitude, longitude);
                map.addOverlay(EMap.CreateMarker(marker, highlight, point, title, qlink, summary, address, description));
                break;
        }
    };
    this.AddPoint = function (imageicon, latitude, longitude, title, qlink, summary, address, description) {
        var point;
        switch (EMap.Provider) {
            case 'virtualearth':
                point = new VELatLong(latitude, longitude);
                map.AddShape(EMap.CreateMapPoint(imageicon, point, title, qlink, summary, address, description));
                break;
            case 'google':
                point = new GLatLng(latitude, longitude);
                map.addOverlay(EMap.CreateMapPoint(imageicon, point, title, qlink, summary, address, description));
                break;
        }
    };
    this.LatLon = function (lat, lon) {
        switch (EMap.Provider) {
            case 'virtualearth':
                return (new VELatLong(lat, lon));
                break;
            case 'google':
                return (new GLatLng(lat, lon));
                break;
        }
    };
    this.GetRoute = function (start, end, units, route_type, callback) {
        if (EMap.GeoControl) {
            if (document.getElementById('__SearchAddr') != null) {
                EMap.SearchAddr = EMessage.Validate(document.getElementById('__SearchAddr').value);
                EMap.SearchAddr = EGlobal.Replace(EMap.SearchAddr, '#', ' ');
            }
            if (start == null || start == '') {
                start = EMessage.Validate(document.getElementById('__StartPoint').value);
                start = EGlobal.Replace(start, '#', ' ');
            }
            if (end == null || end == '') {
                end = EMessage.Validate(document.getElementById('__EndPoint').value);
                end = EGlobal.Replace(end, '#', ' ');
            }
            if (start == '' || end == '') {
                var message = '';
                if (start == '' && end == '') { message = EMessage.NoStartNoEnd; }
                else {
                    if (start == '') { message = EMessage.NoStart; }
                    else { message = EMessage.NoEnd; }
                }
                alert(message);
                EMap.Reset(); return;
            }
            if (start == end) {
                var message = '';
                if (start == end) { message = EMessage.NoStartNoEnd; }
                alert(message);
                EMap.Reset(); return;
            }
            switch (EMap.Provider) {
                case 'virtualearth':
                    EMap.RouteRequest = true;
                    EMap.RouteInfo = null;
                    map.DeleteRoute();
                    map.GetRoute(start, end, eval('VEDistanceUnit.' + EMap.DistanceUnit), null, EMap.ShowRoute);
                    break;
                case 'google':
                    var vend = '';
                    try { if (end.lat() != null) { vend = end.lat() + ',' + end.lng() } else { vend = end; } } catch (e) { vend = end; }
                    window.open('http://maps.google.com/maps?saddr=' + start + '&daddr=' + vend);
                    break;
            }
        }
        else {
            alert('Cannot find direction, GeoControl disabled.');
        }
    };
    this.FlipAddress = function () {
        var _addr1 = document.getElementById('__StartPoint').value;
        var _addr2 = document.getElementById('__EndPoint').value;
        document.getElementById('__EndPoint').value = _addr1;
        document.getElementById('__StartPoint').value = _addr2;
    };
    this.SetControl = function (control, findaction) {
        var _keyword = "";
        if (document.getElementById('__SearchKey') != null) {
            _keyword = document.getElementById('__SearchKey').value;
        }
        EMap.CurrentView = control;
        switch (control) {
            case "search":
                document.getElementById('__SearchPane').innerHTML = EMap.Geosearch + EMap.Geosearchbutton;
                document.getElementById('__CategoryPane').innerHTML = EMap.Categorytemplate;
                EMap.ShowPane('dvSearch');
                EMap.SetValue('__SearchAddr', EMap.SearchAddr);
                if (findaction) {
                    EMap.FindAll(false);
                }
                break;
            case "direction":
                if (EMap.GeoControl) {
                    if (document.getElementById('__SearchAddr') != null && EMessage.Validate(document.getElementById('__SearchAddr').value) != '') {
                        EMap.SearchAddr = document.getElementById('__SearchAddr').value;
                    }
                    document.getElementById('__DirectionPane').innerHTML = EMap.GeoDirection;
                    EMap.ShowPane('dvDirection');
                    EMap.SetValue('__StartPoint', EMap.SearchAddr);
                    EMap.SetValue('__EndPoint', "");
                    if (EMap.ContentInfo != '') {
                        var data;
                        try { data = eval('(' + EMap.ContentInfo + ')'); } catch (e) { data = EMap.ContentInfo; }
                        EMap.SetValue('__EndPoint', data.items[0].Address);
                    }
                }
                break;
            case "find":
                document.getElementById('__SearchPane').innerHTML = EMap.Geocatsearch + '&#160;' + EMap.Geosearch + EMap.Geosearchbutton;
                document.getElementById('__CategoryPane').innerHTML = EMap.Categorytemplate;
                EMap.ShowPane('dvFind');
                document.getElementById('__SearchKey').value = _keyword;
                EMap.SetValue('__SearchAddr', EMap.SearchAddr);
                EMap.SetValue('__SearchKey', EMap.SearchText);
                if (findaction) {
                    EMap.FindAll(false);
                }
                break;
        }
    };
    this.Reset = function () {
        EMap.InitOnZoom = true;
        EMap.RouteRequest = false;
        EMap.ZoomLocked = false;
    };
    this.SetAddress = function (addr, field) {
        if (EMap.GeoControl) {
            if (document.getElementById('__StartPoint') == null || document.getElementById('__EndPoint') == null) {
                EMap.SetControl('direction', false);
            }
            if (field == "from") {
                document.getElementById('__StartPoint').value = addr;
            }
            else {
                document.getElementById('__EndPoint').value = addr;
            }
        }
    };
    this.SetZoomLevel = function () {
        switch (EMap.Provider) {
            case 'virtualearth':
                map.SetZoomLevel(EMap.CurrentZoomLevel);
                break;
            case 'google':
                map.setZoom(EMap.CurrentZoomLevel);
                break;
        }
    };
    this.ShowRoute = function (route) {
        //The CallBackResult would be null if the search criteria isn't a valid address or zipcode ...Defect #:49645
        if (route == undefined) {
            return false;
        }
        EMap.RouteInfo = route;
        if (document.getElementById('__StartPoint') == null || document.getElementById('__EndPoint') == null) {
            EMap.SetControl('direction', false);
        }
        if (route.StartLocation.Address != '') {
            document.getElementById('__StartPoint').value = route.StartLocation.Address;
            document.getElementById('__StartPoint').style.color = "";
        }
        if (route.EndLocation.Address != '') {
            document.getElementById('__EndPoint').value = route.EndLocation.Address;
            document.getElementById('__EndPoint').style.color = "";
        }
        var routeinfo = '<table border="0" cellpadding="0" cellspacing="0" widt="100%"><tr><td><table>';
        routeinfo += EGlobal.Format(EMap.Directionaddresstemplate, new Array('Start address:', document.getElementById('__StartPoint').value));
        routeinfo += EGlobal.Format(EMap.Directionaddresstemplate, new Array('End address:', document.getElementById('__EndPoint').value));
        routeinfo += EGlobal.Format(EMap.Directionaddresstemplate, new Array('Distance:', route.Itinerary.Distance + ' ' + route.Itinerary.DistanceUnit + '(' + route.Itinerary.Time + ')'));
        routeinfo += "</table></td></tr></table> ";
        var steps = '<table border="0" cellspacing="0" width="100%"><tr><td><table border="1" width="100%" cellspacing="0" cellpadding="0"><tr><td><table cellspacing="0" style="border-color:White;border-width:0px;border-style:None;width:100%;border-collapse:collapse;"><tr class="dir_action_label"><td class="dir_action_label" align="right" valign="center"><a href="javascript:EMap.GetReverseRoute();">Reverse<img border=\"0\"  src="' + EMap.AppPath + 'images/application/maps/ddirflip.gif" alt="Click here to get reverse directions." /></a>&#160;&#160;&#160;<a href="javascript:EMap.EmailDirection();">Email<img border=\"0\"  src="' + EMap.AppPath + 'images/application/maps/email.gif" alt="Click here to email this directions." /></a>&#160;&#160;&#160;<a href="javascript:EMap.PrintDirection();">Print<img border=\"0\"  src="' + EMap.AppPath + 'images/application/maps/print.gif" alt="Click here to print this directions." /></a>&#160;&#160;&#160;<a href="javascript:EMap.CleanDirection();">Close<img border=\"0\"  src="' + EMap.AppPath + 'images/application/maps/exit.gif" alt="Click here to close these directions." /></a></td></tr>';
        steps += '<tr><td>' + routeinfo + '</td></tr><tr><td>&nbsp</tr><tr><td><table>';
        var len = route.Itinerary.Segments.length;
        var segmentReg = new RegExp("[-]?\\d+(?:\\.\\d+)?\\s*,\\s*[-]?\\d+(?:\\.\\d+)?");
        for (var i = 0; i < len; i++) {
            var segment = route.Itinerary.Segments[i].Instruction;
            if (i == 0) {
                segment = segment.replace(segmentReg, route.StartLocation.Address)
            }
            else if (i == len - 1) {
                segment = segment.replace(segmentReg, route.EndLocation.Address)
            }
            steps += EGlobal.Format(EMap.Directiontemplate, new Array(i + 1, segment, route.Itinerary.Segments[i].Distance, route.Itinerary.DistanceUnit));
        }
        steps += "</table></td></tr></table></td></tr></table>";
        routeinfo = steps;
        EMap.SetStyle('__SearchTxtResultPane', "none");
        EMap.SetStyle('__RouteInfoPane', "block");
        document.getElementById('__RouteInfoPane').innerHTML = routeinfo;
        EMap.InitOnZoom = false;
        EMap.RouteRequest = true;
        EMap.ZoomLocked = true;
        try { EMap.RemoveSearchMarker(); } catch (e) { }
    };
    this.RemoveSearchMarker = function () {
        if (EMap.ItemArray != null) {
            switch (EMap.Provider) {
                case 'virtualearth':
                    for (var i = 0; i < EMap.ItemArray.length; i++) {
                        map.DeletePushpin(i + 1);
                    }
                    break;
                case 'google':
                    for (var i = 0; i < EMap.ItemArray.length; i++) {
                    }
                    break;
            }
        }
    };
    this.CleanDirection = function () {
        EMap.DeleteRoute();
        EMap.DeleteAllPolylines();
        EMap.ReLoadMarkers();
        EMap.Reset();
        document.getElementById('__RouteInfoPane').innerHTML = "";
        EMap.SetStyle('__RouteInfoPane', "none");
        EMap.SetControl('search', true);
        EMap.SetStyle('__SearchTxtResultPane', "block");
    };
    this.ReLoadMarkers = function () {
        if (EMap.ItemArray != null) {
            for (var i = 0; i < EMap.ItemArray.length; i++) {
                var item = EMap.ItemArray[i];
                eval('EMap.ShowContent(' + parseInt(i + 1) + ',\'\',' + item.Latitude + ',' + item.Longitude + ',\'' + item.Title + '\',\'' + item.QLink + '\',\'' + item.Summary + '\',\'' + item.Address + '\',\'' + item.Description + '\');');
            }
        }
    };
    this.DeleteAllPolylines = function () {
        map.DeleteAllPolylines();
    };
    this.DeleteRoute = function () {
        map.DeleteRoute();
    };
    this.GetReverseRoute = function () {
        EMap.FlipAddress();
        EMap.GetRoute(null, null, null, null, null);
    };
    this.PrintDirection = function () {
        window.print();
    };
    this.Search = function () {
        __LoadMap(EMap.getArguements(), 'mode=search&pageindex=1');
    };
    this.Clear = function () {
        map.Clear();
    };
    this.GetCenter = function () {
        switch (EMap.Provider) {
            case 'virtualearth':
                return map.GetCenter();
                break;
            case 'google':
                var e = map.getCenter();
                return e.lat() + "," + e.lng();
                break;
        }
    };
    this.GetMapView = function () {
        return (map.GetMapView());
    };
    this.ShowPane = function (tabID) {
        switch (tabID) {
            case "dvSearch":
                document.getElementById('_dvSearch').style.display = "block";
                document.getElementById('dvSearch').className = "tab_actived";
                document.getElementById('_dvDirection').style.display = "none";
                document.getElementById('dvDirection').className = "tab_disabled";
                document.getElementById('dvFind').className = "tab_disabled";
                break;
            case "dvFind":
                document.getElementById('_dvSearch').style.display = "block";
                document.getElementById('dvSearch').className = "tab_disabled";
                document.getElementById('_dvDirection').style.display = "none";
                document.getElementById('dvDirection').className = "tab_disabled";
                document.getElementById('dvFind').className = "tab_actived";
                break;
            case "dvDirection":
                document.getElementById('_dvSearch').style.display = "none";
                document.getElementById('dvSearch').className = "tab_disabled";
                document.getElementById('_dvDirection').style.display = "block";
                document.getElementById('dvDirection').className = "tab_actived";
                document.getElementById('dvFind').className = "tab_disabled";
        }
    };
    this.DisplayError = function (message, context) {
        alert('An unhandled exception has occurred:\n' + message);
    };
    this.GetDisplayName = function (userObj) {
        var result = "";
        try {
            if (userObj) {
                if (("undefined" != typeof userObj.Title) && (userObj.Title.length > 0)) {
                    result = userObj.Title;
                }
                else if (("undefined" != typeof userObj.Summary) && (userObj.Summary.length > 0)) {
                    var names = userObj.Summary.split(" ");
                    if (names && (names.length > 0)) {
                        result = names[0];
                    }
                }
            }
        }
        catch (e) {
            result = "";
        }
        return (result);
    };
    this.DisplaySearchResult = function (result, context) {
        try {
            if (context != "") {
                EQueryString.Parse(context);
                EMap.CurrentPage = EQueryString.Param("pageindex");
            }
            ECookie.SetCookie('cookiepageindex', EMap.CurrentPage);
            EMap.RemoveAllMarker();
            var validdata = false;
            var data = null;
            if (result != '') {
                try {
                    data = eval('(' + result + ')');
                    validdata = true;
                }
                catch (e) {
                }
                if (data == null) {
                    validdata = false;
                    if (EMap.TextResultOn) { document.getElementById('__SearchTxtResultPane').innerHTML = "Invalid data loaded."; }
                }
            }
            var _result = "";
            var _class = "oddrow";
            var count = 0;
            var itemcount = 0;
            var keytext = "";
            if (EMap.CurrentView == "find") {
                keytext = EMap.SearchText;
            }
            if (keytext != "") { keytext = " for " + keytext; }
            if (EMap.CategoryText != "") { EMap.CategoryText = '<b>Categories:</b><br/>' + EMap.CategoryText; }
            if (validdata) {
                if (data != null) {
                    EMap.ItemArray = new Array();
                    itemcount = data.items.length;
                    for (var i = 0; i < data.items.length; i++) {
                        EMap.ItemArray.push(data.items[i]);
                        eval('EMap.ShowContent(' + (i + 1) + ',\'\',' + data.items[i].Latitude + ',' + data.items[i].Longitude + ',\'' + EMap.GetDisplayName(data.items[i]) + '\',\'' + data.items[i].QLink + '\',\'' + data.items[i].Summary + '\',\'' + data.items[i].Address + '\',\'' + data.items[i].Description + '\');');
                        if (EMap.TextResultOn) {
                            if (((i + 1) / 2) * 2 == Math.floor((i + 1) / 2) * 2) {
                                _class = "evenrow";
                            } else {
                                _class = "oddrow";
                            }
                            if (EMap.SearchData != "content") {
                                if (data.items[i].QLink.length > 0) {
                                    _result += EGlobal.Format(EMap.UserSearchresulttemplate, new Array(_class, data.items[i].QLink, EMap.GetDisplayName(data.items[i]), 'javascript:EMap.SetCenterIndex(' + parseInt(i + 1) + ',' + data.items[i].Latitude + ',' + data.items[i].Longitude + ')', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(data.items[i].Address, '#', ' ') + '\',\'to\');EMap.GetRoute(\'' + EMap.SearchAddr + '\',EMap.LatLon(' + data.items[i].Latitude + ',' + data.items[i].Longitude + ',null,null,null))', data.items[i].Address + '<br/>', '', (i + 1), data.items[i].Distance, EMap.DisplayTextUnit));
                                }
                                else {
                                    _result += EGlobal.Format(EMap.UserSearchresulttemplateNoLink, new Array(_class, EMap.GetDisplayName(data.items[i]), 'javascript:EMap.SetCenterIndex(' + parseInt(i + 1) + ',' + data.items[i].Latitude + ',' + data.items[i].Longitude + ')', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(data.items[i].Address, '#', ' ') + '\',\'to\');EMap.GetRoute(\'' + EMap.SearchAddr + '\',EMap.LatLon(' + data.items[i].Latitude + ',' + data.items[i].Longitude + ',null,null,null))', data.items[i].Address + '<br/>', '', (i + 1), data.items[i].Distance, EMap.DisplayTextUnit));
                                }
                            } else {
                                _result += EGlobal.Format(EMap.Searchresulttemplate, new Array(_class, data.items[i].QLink, EMap.GetDisplayName(data.items[i]), 'javascript:EMap.SetCenterIndex(' + parseInt(i + 1) + ',' + data.items[i].Latitude + ',' + data.items[i].Longitude + ')', 'javascript:EMap.SetAddress(\'' + EGlobal.Replace(data.items[i].Address, '#', ' ') + '\',\'to\');EMap.GetRoute(\'' + EMap.SearchAddr + '\',EMap.LatLon(' + data.items[i].Latitude + ',' + data.items[i].Longitude + ',null,null,null))', data.items[i].Address + '<br/>' + data.items[i].Description, '', (i + 1), data.items[i].Distance, EMap.DisplayTextUnit));
                            }
                        }
                    }
                    count = data.Count;
                }
                if (EMap.TextResultOn) {
                    var _displaycategorytext = '';
                    var startcount = 1;
                    if (itemcount == 0) { startcount = 0; }
                    var endcount = (EMap.PageSize * EMap.CurrentPage) - (EMap.PageSize - itemcount);
                    if (EMap.CurrentPage > 1) { startcount = (EMap.PageSize * (EMap.CurrentPage - 1)) + 1; }
                    _displaycategorytext = EGlobal.Format(EMap.Searchheader, new Array(startcount, endcount, count, keytext, EGlobal.Replace(EGlobal.Replace(EGlobal.Replace(EMap.CategoryText, ',', '<br/>'), EMap.MapCategoryPath, ''), '\\', '>')));
                    var _headertext = '<table><tr><td>' + _displaycategorytext + '</td></tr></table>';
                    if (_result != '') {
                        _result = _headertext + '<table border="0" cellspacing="0"><tr><td><table border="1" width="100%" cellspacing="0" cellpadding="0"><tr><td><table cellspacing="0" style="border-color:White;border-width:0px;border-style:None;width:100%;border-collapse:collapse;"><tr class="title-header"><td class="title-header">No.</td><td class="title-header">Title</td><td class="title-header">Distance</td><td class="title-header">Map</td><td class="title-header">Direction</td>' + _result + '</table></td></tr></table>';
                        _result += EMap.BuildPageLink(count);
                    }
                    else {
                        _displaycategorytext = EGlobal.Format(EMap.Searchheader2, new Array(keytext, EGlobal.Replace(EGlobal.Replace(EGlobal.Replace(EMap.CategoryText, ',', '<br/>'), EMap.MapCategoryPath, ''), '\\', '>')));
                        _result = _displaycategorytext;
                    }
                    document.getElementById('__SearchTxtResultPane').innerHTML = _result;
                    EMap.SetStyle('__SearchTxtResultPane', "block");
                }
            }
            else {
                _result = EGlobal.Format(EMap.Searchheader2, new Array(keytext, EGlobal.Replace(EGlobal.Replace(EGlobal.Replace(EMap.CategoryText, ',', '<br/>'), EMap.MapCategoryPath, ''), '\\', '>')));
                if (EMap.TextResultOn) { document.getElementById('__SearchTxtResultPane').innerHTML = _result; }
            }
        }
        catch (ex) {
            if (EMap.TextResultOn) { document.getElementById('__SearchTxtResultPane').innerHTML = "Load completed with error."; }
        }
        EMap.InitOnZoom = true;
    };
    this.SetStyle = function (element, display) {
        var control = document.getElementById(element);
        control.style.display = display;
        if (element == '__SearchTxtResultPane' || element == '__RouteInfoPane' || element == '__Map') {
            if (EMap.Width != '0' && EMap.Width != '0px')
                control.style.width = EMap.Width + "px";
            if (EMap.Height != '0' && EMap.Height != '0px')
                control.style.height = EMap.Height + "px";
        }
    };
    this.getArguements = function () {
        return (EMap.serializeForm());
    };
    this.serializeForm = function () {
        var element = document.forms[0].elements;
        var len = element.length;
        var query_string = "";
        this.AddFormField = function (name, value) {
            if (query_string.length > 0) {
                query_string += "&";
            }
            query_string += encodeURIComponent(name) + "=" + encodeURIComponent(value);
        };
        if (document.getElementById('__SearchAddr') != null) {
            EMap.SearchAddr = EMessage.Validate(document.getElementById('__SearchAddr').value);
        }
        this.AddFormField("searchaddr", EMap.SearchAddr);
        if (document.getElementById('__SearchKey') != null) {
            EMap.SearchText = EMessage.Validate(document.getElementById('__SearchKey').value);
        }
        if (EMap.CurrentView == "find") {
            this.AddFormField("searchtext", EMap.SearchText);
        }
        else {
            this.AddFormField("searchtext", "");
        }
        this.AddFormField("backsearchtext", EMap.SearchText);
        this.AddFormField("currentview", EMap.CurrentView);
        if (document.getElementById('____ekmapcategorypath') != null) {
            this.AddFormField("ActualCategoryPath", document.getElementById('____ekmapcategorypath').value);
        }
        EMap.InitOnZoom = false; //EMap.SetZoomLevel();
        if (EMap.SearchLocation == "") {
            EMap.SearchLocation = EMap.GetCenter()
        }
        EMap.CategoryText = '';
        var selectedids = '';
        if (EMap.MapCategory != '') {
            if ($ektron("div#__MapCategoryTreePane #taxonomyFilter").length > 0) {
                var taxnomyvalues = $ektron("div#__MapCategoryTreePane input.searchTaxonomyPath");
                if (taxnomyvalues !== null) {
                    for (var count = 0; count < taxnomyvalues.length; count++) {
                        if (taxnomyvalues[count].checked === true) {
                            if (EMap.CategoryText.length <= 0)
                                EMap.CategoryText = taxnomyvalues[count].value;
                            else
                                EMap.CategoryText += "," + taxnomyvalues[count].value;
                            if (selectedids.length <= 0)
                                selectedids = taxnomyvalues[count].getAttribute("data-ektron-MapCategoryId");
                            else
                                selectedids += "," + taxnomyvalues[count].getAttribute("data-ektron-MapCategoryId");
                        }
                    }
                }
            }

            /*            
            if (map_cat_tree != null && map_cat_tree.getAllChecked() != '') {
            selectedids = map_cat_tree.getAllChecked();
            selectedidlist = selectedids.split(',');
            for (var i = 0; i < selectedidlist.length; i++) {
            if (EMap.CategoryText == '') {
            EMap.CategoryText += EMap.MapCategoryPath + EMap.TreePath(selectedidlist[i]);
            }
            else {
            EMap.CategoryText += "," + EMap.MapCategoryPath + EMap.TreePath(selectedidlist[i]);
            }
            }
            }
            */
        }
        var timeoption = '-1';
        var starttime = '';
        var endtime = '';
        if (EMap.DateSearch) {
            try {
                if (document.forms[0].__timegroup[0].checked)
                    timeoption = document.forms[0].__timegroup[0].value;
                else
                    timeoption = document.forms[0].__timegroup[1].value;
            } catch (e) { }
            starttime = document.getElementById('__StartDate').value;
            endtime = document.getElementById('__EndDate').value;
        }
        this.AddFormField("timeoption", timeoption);
        this.AddFormField("starttime", starttime);
        this.AddFormField("endtime", endtime);
        this.AddFormField("categoryids", selectedids);
        this.AddFormField("category", EMap.CategoryText);
        this.AddFormField("searchlocation", EMap.SearchLocation);
        switch (EMap.Provider) {
            case 'virtualearth':
                var view = EMap.GetMapView();
                this.AddFormField("topleftlatlon", view.TopLeftLatLong);
                this.AddFormField("bottomrightlatlon", view.BottomRightLatLong);
                break;
            case 'google':
                var view = map.getBounds();
                var sw = view.getSouthWest();
                var ne = view.getNorthEast();
                this.AddFormField("topleftlatlon", ne.lat() + ',' + ne.lng());
                this.AddFormField("bottomrightlatlon", sw.lat() + ',' + sw.lng());
                break;
        }
        this.AddFormField("currentzoomlevel", EMap.CurrentZoomLevel);
        ECookie.SetCookie('mapcookie', query_string);
        return query_string;
    };
    this.SetMapControl = function () {
        switch (EMap.Provider) {
            case 'virtualearth':
                if (!(document.getElementById('__ChkControl').checked)) {
                    map.ShowDashboard();
                }
                else {
                    map.HideDashboard();
                }
                break;
            case 'google':
                if (!(document.getElementById('__ChkControl').checked)) {
                    if (EMap.ZoomControl) {
                        map.addControl(EMap.ZoomControlRef);
                    }
                    if (EMap.TypeControl) {
                        map.addControl(EMap.TypeControlRef);
                    }
                }
                else {
                    map.removeControl(EMap.ZoomControlRef);
                    map.removeControl(EMap.TypeControlRef);
                }
                break;
        }
    };
    this.ValidateKey = function (item, c) {
        document.getElementById(c).style.color = "";
        if (item.keyCode == 13) {
            if (EMessage.Validate(document.getElementById(c).value) == '') { document.getElementById(c).style.color = "gray"; }
            if (c == '__SearchKey' || c == '__SearchAddr') {
                EMap.FindAll(true);
            }
            else if (c == '__StartPoint' || c == '__EndPoint') {
                EMap.GetRoute(null, null, null, null, null);
            }
            return false;
        }
        else {
            // document.getElementById(c).value=EMessage.Validate(document.getElementById(c).value);
        }
    };
    this.SetValue = function (c, v) {
        if (document.getElementById(c) != null) {
            if (EMessage.Validate(v) == '') {
                document.getElementById(c).style.color = "gray";
                switch (c) {
                    case "__SearchAddr":
                        document.getElementById(c).value = EMessage.Where;
                        break;
                    case "__SearchKey":
                        document.getElementById(c).value = EMessage.What;
                        break;
                    case "__StartPoint":
                        document.getElementById(c).value = EMessage.Start;
                        break;
                    case "__EndPoint":
                        document.getElementById(c).value = EMessage.End;
                        break;
                }
            }
            else {
                document.getElementById(c).value = v;
                document.getElementById(c).style.color = "";
            }
        }
    };
    this.ClearValue = function (c) {
        document.getElementById(c).value = EMessage.Validate(document.getElementById(c).value);
        document.getElementById(c).style.color = "";
    };
    this.GetPageParam = function () {
        if (parseInt(EMap.GetZoomLevel()) >= parseInt(EMap.MinZoomLevel)) {
            return (EMap.getArguements());
        }
        else {
            return (ECookie.GetCookie('mapcookie'));
        }
    };
    this.BuildPageLink = function (count) {
        if (count > 0 && EMap.PageSize > 0) {
            EMap.PageCount = Math.floor(count / EMap.PageSize);
            if ((count != (EMap.PageCount * EMap.PageSize)) && (count > EMap.PageSize)) {
                EMap.PageCount++;
            }
        }
        var pageheader = "";
        if (EMap.PageCount > 1) {
            if (EMap.CurrentPage == 1) {
                pageheader += "<span id=__first disabled>[First]</span>";
                pageheader += "\t<span id=__previous disabled>[Previous]</span>";
            }
            else {
                pageheader += "<a href=\"javascript:__LoadMap(EMap.GetPageParam(),'mode=search&pageindex=1');\">[First]</a>";
                pageheader += "\t<a href=\"javascript:__LoadMap(EMap.GetPageParam(),'mode=search&pageindex=" + (parseInt(EMap.CurrentPage) - 1) + "');\">[Previous]</a>";
            }

            if (EMap.CurrentPage == EMap.PageCount) {
                pageheader += "\t<span id=ecmNext disabled>[Next]</span>";
                pageheader += "\t<span id=ecmLast disabled>[Last]</span>";
            }
            else {
                pageheader += "\t<a href=\"javascript:__LoadMap(EMap.GetPageParam(),'mode=search&pageindex=" + (parseInt(EMap.CurrentPage) + 1) + "');\">[Next]</a>";
                pageheader += "\t<a href=\"javascript:__LoadMap(EMap.GetPageParam(),'mode=search&pageindex=" + EMap.PageCount + "');\">[Last]</a>";
            }
            pageheader = "<table><tr><td>" + pageheader + "</td></tr></table>";
        }
        return pageheader;
    };
};
EGlobal.AttachLoadEvent(EMap.LoadMap);
function design_prevalidateElement() {
    try { EMap.FindAll(); } catch (e) { }
}
