
Ektron.ready(
    function () {
        if ("undefined" == typeof Ektron.FacebookLogin) {
            Ektron.FacebookLogin =
            {
                signupTemplate: "",
                uniqueId: "",
                FBUserId: "",
                FBToken: "",
                CmsUserId: "",
                AutoLogin: false,
                IsInitialized: false,
                init: function (apiKey) {

                    if (Ektron.FacebookLogin.IsInitialized == true) {
                        return;
                    }
                    if (typeof FB == "undefined") {
                        return;
                    }

                    FB.init({ appId: apiKey, status: true, cookie: true, xfbml: true });
                    $ektron('a.ek_fbButton').bind("click", Ektron.FacebookLogin.login);

                    //if user isn't already logged in to the CMS, get his FB status
                    if ((Ektron.FacebookLogin.CmsUserId.length == 0 || Ektron.FacebookLogin.CmsUserId == "0") && Ektron.FacebookLogin.AutoLogin == true) {
                        FB.getLoginStatus(function (response) {
                            if (response.session) {
                                //User is logged into FB - Check if they are a CMS user and auto-login if so.
                                Ektron.FacebookLogin.setSessionVariables(response.session);
                                $ektron.ajaxCallback(Ektron.FacebookLogin.uniqueId, Ektron.FacebookLogin.FBUserId, Ektron.FacebookLogin.VerifyAndLoginExistingUser, Ektron.FacebookLogin.VerifyAndLoginExistingUser)
                            }
                        });
                    }

                    //even if FB user isn't logged in, we want the user's token and ID
                    if ($ektron.cookie('fbToken') == null) {
                        FB.Event.subscribe('auth.sessionChange', function (response) {
                            Ektron.FacebookLogin.setSessionVariables(response.session);
                        });
                    }

                    Ektron.FacebookLogin.IsInitialized = true;
                },
                initUserSession: function (apiKey) {
                    //this method is used to get current user's Id, Token, but not log them in.

                    if (FB == null) {
                        FB.init({ appId: apiKey, status: true, cookie: true, xfbml: true });
                    }

                    if ($ektron.cookie('fbToken') == null) {
                        FB.getLoginStatus(function (response) {
                            if (response.session) {
                                Ektron.FacebookLogin.setSessionVariables(response.session);
                            }
                        });
                    }

                    //even if FB user isn't logged in, we want the user's token and ID
                    if ($ektron.cookie('fbToken') == null) {
                        FB.Event.subscribe('auth.sessionChange', function (response) {
                            Ektron.FacebookLogin.setSessionVariables(response.session);
                        });
                    }
                },
                setSessionVariables: function (session) {

                    origUserId = Ektron.FacebookLogin.FBUserId;

                    Ektron.FacebookLogin.FBUserId = session.uid;
                    Ektron.FacebookLogin.FBToken = session.access_token;
                    $ektron(".ek_facebooklogin_userId").val(session.uid);
                    $ektron.cookie('fbToken', session.access_token);
                    $ektron.cookie('fbUid', session.uid);

                    if (origUserId != Ektron.FacebookLogin.FBUserId) {
                        var data = { 'facebookUserId': Ektron.FacebookLogin.FBUserId, "fbToken": session.access_token };
                        $ektron(document).trigger("Ektron_FacebookSessionDetected", [data])
                    }

                },
                login: function () {
                    FB.login(function (response) {
                        if (response.session) {
                            //User has successfully logged into facebook
                            Ektron.FacebookLogin.setSessionVariables(response.session);

                            //If FB user is CMS user, login - else - redirect to registration page
                            $ektron.ajaxCallback(Ektron.FacebookLogin.uniqueId, Ektron.FacebookLogin.FBUserId, Ektron.FacebookLogin.LoginOrRegister, Ektron.FacebookLogin.LoginOrRegister)
                        } else { /*login failed*/ }
                    }, { perms: 'user_likes,user_birthday,user_work_history,user_relationships' });

                },
                VerifyAndLoginExistingUser: function (result) {
                    //if existing user, auto-login
                    if (result > 0) {
                        var linkPostBackId = $ektron(".EktronFacebookLoginLoginBtn").attr("id");
                        linkPostBackId = linkPostBackId.replace(/_/g, "$");
                        __doPostBack(linkPostBackId, "");
                    }
                },
                LoginOrRegister: function (result) {
                    if (result > 0) {
                        //do postback to login.
                        var linkPostBackId = $ektron(".EktronFacebookLoginLoginBtn").attr("id");
                        linkPostBackId = linkPostBackId.replace(/_/g, "$");
                        __doPostBack(linkPostBackId, "");
                    } else {
                        //if signup template supplied, redirect - else - raise NewMemberLoggedIn event.
                        if (Ektron.FacebookLogin.signupTemplate.length > 0) {
                            window.location = Ektron.FacebookLogin.signupTemplate + "?authType=1&AuthUserId=" + Ektron.FacebookLogin.FBUserId;
                        } else {
                            var data = { 'facebookUserId': Ektron.FacebookLogin.FBUserId };
                            $ektron(document).trigger("Ektron_FacebookNewMemberLoggedIn", [data]);
                        }
                    }
                }

            } //end fblogin
        } //end if undefeined
    } //end function
);           //end ready


