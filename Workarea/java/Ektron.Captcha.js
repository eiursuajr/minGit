//define Ektron object only if it's not already defined
if (typeof (Ektron) == "undefined")
{
    Ektron = {};
}

//define Ektron.Captcha object only if it's not already defined
if (typeof (Ektron.Captcha) == "undefined")
{
    Ektron.Captcha = {

        ApplicationPath: "",

        clientID: "",

        InitSoundManager: function()
        {
            Ektron.ready(function()
            {
                soundManager.debugMode = false;
                soundManager.url = Ektron.Captcha.ApplicationPath + "soundmanager/swf/";
                soundManager.onload = function()
                {
                    var mySound = soundManager.createSound({ id: "aSound",
                        url: Ektron.Captcha.ApplicationPath + "soundmanager/en-us/audiofile.mp3?r=" + Math.random(),
                        autoLoad: false,
                        autoPlay: false
                    });
                    $ektron("#__playAudioBtn").click(function() { mySound.play(); });
                }
            });
        },

        ReInitSM: function()
        {
            var myReSound = soundManager.createSound({ id: "aReSound" + Math.floor(Math.random() * 10001),
                url: Ektron.Captcha.ApplicationPath + "soundmanager/en-us/audiofile.mp3?r=" + Math.floor(Math.random() * 10001),
                autoLoad: false,
                autoPlay: false
            });

            $ektron("#__playAudioBtn").bind("click", function() { myReSound.play(); });

        },

        SubmitValidator: function()
        {
            Ektron.ready(function()
            {
                $ektron("form").submit(function()
                {
                    if ($ektron("#__ekActivateCaptcha").val() == "")
                    {
                        alert("Captcha is empty. Please enter the word to proceed.");
                        return false;
                    }
                    CallbackCaptcha(Ektron.Captcha.GetArguements(), "control=" + Ektron.Captcha.clientID);
                    return false;
                });
            });
        },

        OnCaptchaCallBack: function(result, context)
        {

            this.QueryString = function(key)
            {

                var value = null;
                for (var i = 0; i < this.QueryString.keys.length; i++)
                {
                    if (this.QueryString.keys[i] == key)
                    {
                        value = this.QueryString.values[i];
                        break;
                    }
                }
                return value;
            };

            this.ParseQueryString = function(args)
            {
                var query = args;
                var pairs = query.split("&");
                for (var i = 0; i < pairs.length; i++)
                {
                    var pos = pairs[i].indexOf('=');
                    if (pos >= 0)
                    {
                        var argname = unescape(pairs[i].substring(0, pos));
                        var value = unescape(pairs[i].substring(pos + 1));
                        this.QueryString.keys[this.QueryString.keys.length] = argname;
                        this.QueryString.values[this.QueryString.values.length] = value;
                    }
                }
            };

            this.QueryString.keys = new Array();
            this.QueryString.values = new Array();

            this.ParseQueryString(context);


            if (result == "false")
            {
                alert("Captcha Is Incorrect. Please try again");
                return false;
            }
            else
            {
                var ctrl = this.QueryString("control");
                document.getElementById(ctrl).innerHTML = "";
                var dataelem = document.createElement("div");
                if (result == "true")
                    dataelem.innerHTML = "";
                else
                {
                    dataelem.innerHTML = result;
                }
                document.getElementById(ctrl).appendChild(dataelem);
                if (result == "true")
                    document.forms[0].submit();
                else
                {
                    Ektron.Captcha.ReInitSM();
                }
            }

        },

        GetArguements: function()
        {
            return (Ektron.Captcha.SerializeForm());
        },


        DisplayError: function(message, context)
        {
            alert('An unhandled exception has occurred:\n' + message);
            try
            {
                Ektron.ready.ClientScriptCallbackEvent.errorCallback(message, context);
            }
            catch (ex) { }
        },


        SerializeForm: function()
        {

            var element = document.forms[0].elements;
            var len = element.length;
            var query_string = "";
            this.AddFormField = function(name, value)
            {
                if (query_string.length > 0)
                {
                    query_string += "&";
                }
                query_string += encodeURIComponent(name) + "=" + encodeURIComponent(value);
            };

            for (var i = 0; i < len; i++)
            {
                var item = element[i];
                if (item.name != "__VIEWSTATE" && item.name != "__EVENTTARGET" && item.name != "__EVENTARGUMENT")
                {
                    try
                    {
                        switch (item.type)
                        {
                            case 'text': case 'password': case 'hidden': case 'textarea':
                                this.AddFormField(item.name, item.value);
                                break;
                            case 'select-one':
                                if (item.selectedIndex >= 0)
                                {
                                    this.AddFormField(item.name, item.options[item.selectedIndex].value);
                                }
                                break;
                            case 'select-multiple':
                                for (var j = 0; j < item.options.length; j++)
                                {
                                    if (item.options[j].selected)
                                    {
                                        this.AddFormField(item.name, item.options[j].value);
                                    }
                                }
                                break;
                            case 'checkbox': case 'radio':
                                if (item.checked)
                                {
                                    this.AddFormField(item.name, item.value);
                                }
                                break;
                        }
                    }
                    catch (e)
                    {
                    }
                }
            }
            return query_string;
        }
    };
}

