if(typeof window.Ektron == "undefined")
    Ektron = {};	
        
if(typeof Ektron.Cache == "undefined")
{
	Ektron.Cache = {
		DatabaseName : "ektron",
		DatabaseVersion : "1.0", 
		DatabaseDisplayName : "Ektron Offline Storage", 
		DatabaseSize: 65535, 
		Set : function(value, callback) {
			if($ektron.browser.safari){
				try
				{
					if(typeof Ektron.Cache.DB == "undefined")
						Ektron.Cache.DB = openDatabase(Ektron.Cache.DatabaseName, 
													   Ektron.Cache.DatabaseVersion, 
													   Ektron.Cache.DatabaseDisplayName, 
													   Ektron.Cache.DatabaseSize);
				} catch(e) {
					if(e == INVALID_STATE_ERR)
						Log("Ektron.Cache.Get: Invalid database version");
					else
						Log("Ektron.Cache.Get: Error");
					return;
				}
				
				Ektron.Cache.DB.transaction(function(transaction) {
					transaction.executeSql("CREATE TABLE ektron_key(data_key NVARCHAR(64) NOT NULL UNIQUE);", 
						[], function(a, b) {}, function (a, b) {});
					transaction.executeSql("REPLACE INTO ektron_key (data_key) VALUES (?);", 
						[value], function(a, b) {callback();}, function (a, b) {});
				});
			}else if($ektron.browser.msie){
				var oPersist = document.getElementById('ektronPrivateDataTempStorageText');
				oPersist.setAttribute("sPersist", oPersist.value);
				oPersist.save("oXMLBranch");
				callback();
			}else if($ektron.browser.mozilla){
				try
				{
					globalStorage[location.host].setItem("EktronSecurityTokenData", Ektron.PrivateData.SecurityKey);
				}
				catch(e)
				{
					globalStorage[location.host + ".localdomain"].setItem("EktronSecurityTokenData", Ektron.PrivateData.SecurityKey);
				}
				callback();
			}
		}, 
		
		Get : function(callback, secondCallback) {
			if($ektron.browser.safari){
				try
				{
					if(typeof Ektron.Cache.DB == "undefined")
						Ektron.Cache.DB = openDatabase(Ektron.Cache.DatabaseName, 
													   Ektron.Cache.DatabaseVersion, 
													   Ektron.Cache.DatabaseDisplayName, 
													   Ektron.Cache.DatabaseSize);
				} catch(e) {
					if(e == INVALID_STATE_ERR)
						Log("Ektron.Cache.Get: Invalid database version");
					else
						Log("Ektron.Cache.Get: Error");
					return;
				}
				var retval = "";
				
				Ektron.Cache.DB.transaction(function(transaction) {
					transaction.executeSql("SELECT data_key FROM ektron_key;", [], 
						function(t, r) {
							if(r.rows.length > 0){
								var row = r.rows.item(0);
								callback(row['data_key'], secondCallback);
							}
						}, 
						function(a,b) {});
				});
			}else if($ektron.browser.msie){
				var retVal = "";
				var oPersist = document.getElementById('ektronPrivateDataTempStorageText');
				try{
				    oPersist.load("oXMLBranch");
				    oPersist.value=oPersist.getAttribute("sPersist");
				    retVal = oPersist.value;
				    oPersist.value="";
				    callback(retVal, secondCallback);
				}
				catch(err){
				
				}
			}else if($ektron.browser.mozilla && globalStorage[location.host] != null && globalStorage[location.host].length > 0){
				var retVal = "";
				try
				{
					retVal = globalStorage[location.host].EktronSecurityTokenData.value;
				}
				catch(e)
				{
					retVal = globalStorage[location.host + ".localdomain"].EktronSecurityTokenData.value;
				}
				callback(retVal, secondCallback);
			}
			return "";
		}
	};
}

/*if (typeof Sys != "undefined") {
    Sys.Application.notifyScriptLoaded();
}*/