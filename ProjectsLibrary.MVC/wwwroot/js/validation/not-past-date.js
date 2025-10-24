$.validator.addMethod("notpastdate", function (value, element, params) {
    if (!value) return true;
    var inputDate = new Date(value);

    var today = new Date();
    today.setHours(0, 0, 0, 0);
    inputDate.setHours(0, 0, 0, 0);

    return inputDate >= today;
});

$.validator.unobtrusive.adapters.addBool("notpastdate");