(function($) {
    var cultures = $.global.cultures,
        en = cultures.en,
        standard = en.calendars.standard,
        culture = cultures["uk"] = $.extend(true, {}, en, {
        name: "uk",
        englishName: "Ukrainian",
        nativeName: "українська",
        language: "uk",
        numberFormat: {
            ',': " ",
            '.': ",",
            percent: {
                pattern: ["-n%","n%"],
                ',': " ",
                '.': ","
            },
            currency: {
                pattern: ["-n$","n$"],
                ',': " ",
                '.': ",",
                symbol: "₴"
            }
        },
        calendars: {
            standard: $.extend(true, {}, standard, {
                '/': ".",
                firstDay: 1,
                days: {
                    names: ["неділя","понеділок","вівторок","середа","четвер","п'ятниця","субота"],
                    namesAbbr: ["Нд","Пн","Вт","Ср","Чт","Пт","Сб"],
                    namesShort: ["Нд","Пн","Вт","Ср","Чт","Пт","Сб"]
                },
                months: {
                    names: ["Січень","Лютий","Березень","Квітень","Травень","Червень","Липень","Серпень","Вересень","Жовтень","Листопад","Грудень",""],
                    namesAbbr: ["Січ","Лют","Бер","Кві","Тра","Чер","Лип","Сер","Вер","Жов","Лис","Гру",""]
                },
                monthsGenitive: {
                    names: ["січня","лютого","березня","квітня","травня","червня","липня","серпня","вересня","жовтня","листопада","грудня",""],
                    namesAbbr: ["січ","лют","бер","кві","тра","чер","лип","сер","вер","жов","лис","гру",""]
                },
                AM: null,
                PM: null,
                patterns: {
                    d: "dd.MM.yyyy",
                    D: "d MMMM yyyy' р.'",
                    t: "H:mm",
                    T: "H:mm:ss",
                    f: "d MMMM yyyy' р.' H:mm",
                    F: "d MMMM yyyy' р.' H:mm:ss",
                    M: "d MMMM",
                    Y: "MMMM yyyy' р.'"
                }
            })
        }
    }, cultures["uk"]);
    culture.calendar = culture.calendars.standard;
})($ektron);