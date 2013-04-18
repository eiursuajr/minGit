// JScript File
if ("undefined" === typeof Ektron.Widgets)
{
    Ektron.Widgets = {};
}

if ("undefined" === typeof Ektron.Widgets.Twitter)
{
    Ektron.Widgets.Twitter =
    {
        // Properties
        options: {
            stacked: true,
            horizontal: false,
            title: "Ektron on Twitter"
        },

        // Methods
        twitter: function(title, url) {
            this.title = title;
            this.url = url;

        },

        FormTwitterObjectArray: function(str, CID) {
            var arr = [];
            arr = str.split('#');

            var twitterArray = [];
            for (var i = 0; i < arr.length; i++) {
                _title = arr[i].split(',')[0];
                _url = arr[i].split(',')[1];
                if (typeof (_url) != "string" || typeof (_title) != "string") return;
                twitterArray.push(new Ektron.Widgets.Twitter.twitter(_title, _url));
            }

            new GFdynamicFeedControl(twitterArray, 'feed-control' + CID, Ektron.Widgets.Twitter.options);
        },

        LoadDynamicFeedControl: function(feedstring, CID) {
            Ektron.Widgets.Twitter.FormTwitterObjectArray(feedstring, CID);
        },

        AddTwit: function(btnID) {
            var _url = $ektron("#" + btnID).parents(".TFWidget").find(".textbox")[1].value;
            if (_url == '') {
                alert('Please add a twitter feed');
                return false;
            }
        }
    };
}