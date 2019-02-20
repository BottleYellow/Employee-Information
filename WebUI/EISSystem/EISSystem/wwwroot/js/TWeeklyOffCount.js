function calWeeklyOffYearly(year) {
    var arr = new Array();
    var weeklyOff = 0;
    var altSaturday = 0;
    if (year % 4 == 0) {
        d = new Date(year, 0, 1);
        var myDays = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
        var s = myDays[d.getDay()];
        if (s == "Saturday" || s == "Sunday") {
            weeklyOff = 53;
            altSaturday = 24;
            arr = [weeklyOff, altSaturday];
            return arr;
        }
        else {
            weeklyOff = 52;
            altSaturday = 24;
            arr = [weeklyOff, altSaturday];
            return arr;
        }
    }
    else {
        d = new Date(year, 0, 1);
        var myDays = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
        var s = myDays[d.getDay()];
        if (s == "Sunday") {
            weeklyOff = 53;
            altSaturday = 24;
            arr = [weeklyOff, altSaturday];
            return arr;
        }
        else {
            weeklyOff = 52;
            altSaturday = 24;
            arr = [weeklyOff, altSaturday];
            return arr;
        }
    }
}
function calWeeklyOffMonthly(month, year) {
    var day, counter, date;
    var m = parseInt(month) - 1;
    day = 1;
    counter = 0;
    date = new Date(year, m, day);
    while (date.getMonth() === m) {
        if (date.getDay() === 0) { // Sun=0, Mon=1, Tue=2, etc.
            counter += 1;
        }
        day += 1;
        date = new Date(year, m, day);
    }
    return counter;
}