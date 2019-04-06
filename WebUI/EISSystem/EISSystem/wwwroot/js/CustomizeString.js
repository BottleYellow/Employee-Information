function DecorateString(val) {
    var a = val.split(' ');
    var b = 0;
    var result = "";

    while (a.length > b) {
        result += a[b].charAt(0).toUpperCase() + a[b].slice(1).toLowerCase()+" ";
        b++;
    }
    return result;
}