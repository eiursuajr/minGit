<%@ Page Language="C#" %>

<script runat="server">
    protected string WorkareaPath;
    void Page_Init(object sender, EventArgs args)
    {
        Response.ContentType = "application/javascript";
        Ektron.Cms.SiteAPI siteApi = new Ektron.Cms.SiteAPI();
        WorkareaPath = siteApi.RequestInformationRef.ApplicationPath;
    }
</script>

if(typeof Ektron == "undefined")
    window.Ektron = {};

if(typeof Ektron.PrivateData == "undefined")
{
    Ektron.PrivateData = {

        LoadEncryptedData : function(){
            Ektron.PrivateData.GetSecurityKey(function(){});
                $ektron(".EktronEncryptedData").each(function(index){
                    Ektron.PrivateData.LoadEncryptedDataCallback(($ektron(".EktronEncryptedData"))[index]);
            });
        },

        LoadEncryptedDataCallback : function(item){
            var html = item.innerHTML;
            if(html.substring(0,4) == "key:"){
                var keyValue = html.substring(4);
                Ektron.PrivateData.Log(keyValue);
                Ektron.PrivateData.Get(keyValue, function(data){
                    var decData = data;
                    Ektron.PrivateData.Log(data);
                    item.innerHTML = decData;
                    $ektron(item).removeClass("EktronEncryptedData");
                });
            }
        },

        Get : function(key, callback) {
            Ektron.PrivateData.Log("Ektron.PrivateData.Get(" + key + ", " + callback + ")\n");
            $ektron.post("<%= WorkareaPath %>PrivateData/PrivateData.ashx?action=get&key="+key, {}, function(data){
				if(Ektron.PrivateData.SecurityKey == ""){
				    var dataToProcess = data;
				    Ektron.PrivateData.GetSecurityKey(function(){
			            callback(Ektron.PrivateData.Decrypt(Ektron.Crypto.Base64.decode(dataToProcess)));
			        });
				}else{
				    callback(Ektron.PrivateData.Decrypt(Ektron.Crypto.Base64.decode(data)));
				}
			});
        },

        Set : function(key, data, callback) {
            Ektron.PrivateData.GetSecurityKey(function(){
                Ektron.PrivateData.Log("Ektron.PrivateData.Set(" + key + ", " + data + ", " + callback + ")\n");
                var encdata = Ektron.PrivateData.Encrypt(data);
                $ektron.post("<%= WorkareaPath %>PrivateData/PrivateData.ashx?action=set&key="+key, encdata, callback);
            });
        },

        Encrypt : function(data) {
            Ektron.PrivateData.Log("Ektron.PrivateData.Encrypt(" + data + ")\n");
			Ektron.Crypto.SetEncryptionAlgorithm(Ektron.Crypto.EncryptionAlgorithms.AES256);
			if(Ektron.PrivateData.SecurityKey == ""){
				Ektron.PrivateData.Log("Security Key Needed");
				Ektron.PrivateData.GetSecurityKey();
				return "";
			}else{
				return Ektron.Crypto.Convert.ByteArrayToString(Ektron.Crypto.EncryptData(data, Ektron.Crypto.Convert.StringToByteArray(Ektron.PrivateData.SecurityKey)));
			}
        },

        Decrypt : function(data) {
			Ektron.Crypto.SetEncryptionAlgorithm(Ektron.Crypto.EncryptionAlgorithms.AES256);
            Ektron.PrivateData.Log("Ektron.PrivateData.Decrypt(" + data + ")\n");
			if(Ektron.PrivateData.SecurityKey == ""){
				Ektron.PrivateData.Log("Security Key Needed");
				Ektron.PrivateData.GetSecurityKey();
				return "";
			}else{
			    Ektron.PrivateData.Log("Key Check : " + Ektron.PrivateData.SecurityKey);
				return Ektron.Crypto.DecryptData(Ektron.Crypto.Convert.StringToByteArray(data), Ektron.Crypto.Convert.StringToByteArray(Ektron.PrivateData.SecurityKey));
			}
        },

        SetLoginInfo : function(username, password) {
			Ektron.Crypto.SetCryptographicHashAlgorithm(Ektron.Crypto.CryptographicHashAlgorithms.SHA512);
			Ektron.PrivateData.SecurityKey = Ektron.Crypto.Hash(password);
			Ektron.PrivateData.GetBrowserInfo();
			Ektron.PrivateData.CreateStoredValueBox();
			Ektron.PrivateData.SaveValue();
			Ektron.PrivateData.DeleteStoredValueBox();
        },

		ClearLoginInfo : function(){
			Ektron.PrivateData.SecurityKey = "";
			Ektron.PrivateData.GetBrowserInfo();
			Ektron.PrivateData.CreateStoredValueBox();
			Ektron.PrivateData.SaveValue();
			Ektron.PrivateData.DeleteStoredValueBox();
		},

		SaveValue : function(){
			if( Ektron.PrivateData.ValidBroswer ){
				Ektron.Cache.Set(Ektron.PrivateData.SecurityKey, Ektron.PrivateData.SaveCallback);
			}else{
				Ektron.PrivateData.Log('Warning: Your browser is not compatable with Ektron Secure Data');
			}
		},

		SaveCallback : function(){
		},

		LoadValue : function(secondCallback){
			if( Ektron.PrivateData.ValidBroswer){
				Ektron.Cache.Get(Ektron.PrivateData.LoadCallback, secondCallback);
			}else{
				Ektron.PrivateData.Log('Warning: Your browser is not compatable with Ektron Secure Data');
			}
		},

		LoadCallback : function(data, secondCallback){
			Ektron.PrivateData.SecurityKey = data;
			Ektron.PrivateData.Log('Load Callback : ' + Ektron.PrivateData.SecurityKey);
			if(typeof secondCallback != "undefined"){
			    secondCallback();
			}
		},

		GetSecurityKey : function(callback){
			if(Ektron.PrivateData.SecurityKey == ""){
			    Ektron.PrivateData.CreateStoredValueBox();
			    Ektron.PrivateData.GetBrowserInfo();
			    if(Ektron.PrivateData.ValidBroswer == true){
				    Ektron.PrivateData.Log("Browser Type Id: " + Ektron.PrivateData.BrowserType + " Version OK");
				    Ektron.PrivateData.LoadValue(callback);
			    }else{
				    Ektron.PrivateData.Log("Browser Type Id: " + Ektron.PrivateData.BrowserType + " Unsecure Version");
				    //show login here
			    }
	        }else{
	            callback();
	        }
		},

		CreateStoredValueBox : function(){
			var hashBox = $ektron(".ektronPrivateDataTempStorage");
			if(hashBox.length == 0){
				var hashBox = document.createElement("div");
				hashBox.className = "ektronPrivateDataTempStorage";
				hashBox.Id = "ektronPrivateDataTempStorage";
				$ektron(hashBox).html("<input type='hidden' id='ektronPrivateDataTempStorageText' value='" + Ektron.PrivateData.SecurityKey + "' style='width:1000px;behavior:url(#default#userData);' />");
				$ektron(hashBox).appendTo("body");
			}else{
				$ektron("#ektronPrivateDataTempStorageText").val( Ektron.PrivateData.SecurityKey );
			}
			Ektron.PrivateData.Log("Text Box Created");
		},

		DeleteStoredValueBox : function(){
			$ektron("#ektronPrivateDataTempStorageText").val("");
			switch(Ektron.PrivateData.BrowserType){
				case Ektron.PrivateData.BrowserTypes.MSIE:
					break;
				case Ektron.PrivateData.BrowserTypes.Firefox:
					$ektron(".ektronPrivateDataTempStorage").remove();
					break;
				case Ektron.PrivateData.BrowserTypes.Safari:
					$ektron(".ektronPrivateDataTempStorage").remove();
					break;
				default:
					Ektron.PrivateData.Log('Warning: Your browser is not compatable with Ektron Secure Data');
					break;
			}
			Ektron.PrivateData.Log("Text Box Removed");
		},

		GetBrowserInfo : function(){
			if($ektron.browser.msie){
				Ektron.PrivateData.BrowserType = Ektron.PrivateData.BrowserTypes.MSIE;
				if($ektron.browser.version == "7.0"){
					Ektron.PrivateData.ValidBroswer = true;
				}
			}else if($ektron.browser.safari){
				Ektron.PrivateData.BrowserType = Ektron.PrivateData.BrowserTypes.Safari;
				if($ektron.browser.version > 312){
					Ektron.PrivateData.ValidBroswer = true;
				}
			}else if($ektron.browser.mozilla){
				Ektron.PrivateData.BrowserType = Ektron.PrivateData.BrowserTypes.Firefox;
				if(parseFloat($ektron.browser.version.substring(0,3)) >= 1.9){
					Ektron.PrivateData.ValidBroswer = true;
				}
			}
		},

        Log : function(text) {
            if(Ektron.PrivateData.Debug){
                if(typeof console != "undefined") {
                    console.log(text);
                } else {
                    alert(text);
                }
            }
        }
    };

	Ektron.PrivateData.Debug = false;

	Ektron.PrivateData.BrowserTypes = { MSIE : 1, Safari: 2, Firefox : 3, Other: 4};
	Ektron.PrivateData.BrowserType = Ektron.PrivateData.BrowserTypes.Other;

	Ektron.PrivateData.ValidBroswerAccepted = false;

	Ektron.PrivateData.KeyStrength = "SHA-512";

	Ektron.PrivateData.SecurityKey = "";

	Ektron.PrivateData.GetSecurityKey();
}
if (typeof Sys != "undefined") {
    Sys.Application.notifyScriptLoaded();
}