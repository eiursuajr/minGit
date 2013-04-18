window.OnClientAppointmentInserting = function (sender, eventArgs) {
    var contentLang = parseInt(folderjslanguage, 10);
    if (typeof (contentLang) != 'number') {
        contentLang = parseInt(jsContentLanguage, 10);
    }
    var multiSupport = jsEnableMultilingual;
    if ((contentLang < 1) && multiSupport) {
        bContinue = confirm("Do you wish to add an event in the default language?");
        eventArgs.set_cancel(true);
        if (bContinue) {
            contentLang = jsDefaultContentLanguage;
            top.notifyLanguageSwitch(contentLang);
        } else {
            return;
        }
        var starttime = eventArgs.get_startTime();
        var startarg = "";
        try {
            startarg = starttime.getFullYear();
            startarg = startarg + ((starttime.getMonth() + 1) < 10 ? '0' : '') + (starttime.getMonth() + 1);
            startarg = startarg + (starttime.getDate() < 10 ? '0' : '') + starttime.getDate();
            startarg = startarg + (starttime.getHours() < 10 ? '0' : '') + starttime.getHours();
            startarg = startarg + (starttime.getMinutes() < 10 ? '0' : '') + starttime.getMinutes();
            startarg = "&startDT=" + startarg;
        } catch (Error) { }

        if (contentLang > 1) {
            self.location.href = "content.aspx?id=" + url_id + "&action=" + url_action + "&LangType=" + contentLang + "&showAddEventForm=true" + startarg;
        } else {
            self.location.href = "content.aspx?id=" + url_id + "&action=" + url_action + "&LangType=" + jsDefaultContentLanguage + "&showAddEventForm=true" + startarg;
        }
    }
}

window.OnClientAppointmentEditing = function (sender, eventArgs) {
    var contentLang = parseInt(folderjslanguage, 10);
    if (typeof (contentLang) != 'number') {
        contentLang = parseInt(jsContentLanguage, 10);
    }
    var multiSupport = jsEnableMultilingual;
    if ((contentLang < 1) && multiSupport) {
        var appointment = eventArgs.get_appointment();
        var appointmentid = appointment.get_id();

        var components = appointmentid.split("~");
        var folderid = components[0];
        var eventid = components[1];
        var langid = components[2];

        eventArgs.set_cancel(true);
        top.notifyLanguageSwitch(langid);
        self.location.href = "content.aspx?id=" + url_id + "&action=" + url_action + "&LangType=" + langid + "&editEvent=" + appointmentid;
    }
}