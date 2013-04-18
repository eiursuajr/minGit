Ektron.ready(function() 
{
    if ("undefined" == typeof(Ektron.UX))
    {
        Ektron.UX = {};
    }
    
    if ("undefined" == typeof(Ektron.UX.Login))
    {
        Ektron.UX.Login = 
        {
            //properties
            loginButton: $ektron("div.userLogin .inputLoginButton"),
            inputUsername: $ektron("div.userLogin .inputUsername"),
            inputPassword: $ektron("div.userLogin .inputPassword"),
            fieldsRequired: "",
        
            // methods
            Init: function()
            {
                Ektron.UX.Login.PlaceFocus();
            },
            PlaceFocus:  function()
            {
                if (Ektron.UX.Login.loginButton.length > 0)
                {
                    // place focus on the username field
                    try
                    {
                        Ektron.UX.Login.inputUsername[0].focus();
                    }
                    catch (e) 
                    {
                        // do nothing
                    }
                }
            },            
            Validate: function()
            {
                // remove previously invalidField classes, and any error messages
                $ektron(".invalidField").removeClass("invalidField");
                $ektron(".errorMessage").remove();
                var result = true;
                var username = Ektron.UX.Login.inputUsername.val();
                var password = Ektron.UX.Login.inputPassword.val();
                if (username.length == 0)
                {
                    Ektron.UX.Login.inputUsername.addClass("invalidField");
                    result = false;
                }
                
                if (password.length == 0)
                {
                    Ektron.UX.Login.inputPassword.addClass("invalidField");
                    result = false;
                }
                var invalidFields = $ektron(".invalidField");
                if (invalidFields.length > 0)
                {
                    // reveal the Error message
                    $ektron("p.intro").after("<div class='ui-widget errorMessage'><div class='ui-state-error ui-corner-all ui-helper-clearfix'><span class='ui-icon ui-icon-alert errorIcon'></span>" + Ektron.UX.Login.fieldsRequired + "</div></div>");
                    // put focus on the first invalid field
                    try
                    {
                        $ektron(".invalidField")[0].focus();
                    }
                    catch(e)
                    {
                        // do nothing if focus failed
                    }
                }
                return result;
            }
        };
    }
    // initialize
    Ektron.UX.Login.Init();
});