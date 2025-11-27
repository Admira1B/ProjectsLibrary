$.validator.addMethod("notearlierdate", function (value, element, params) {
    if (!value) return true; // пустое значение валидно

    var form = $(element).closest("form");
    var startDateProperty = params.startdateproperty;
    var startDateInput = form.find("[name='" + startDateProperty + "']");

    if (!startDateInput.length) return true; // если стартовый элемент не найден - пропускаем

    var startDateVal = startDateInput.val();
    if (!startDateVal) return true;

    var startDate = new Date(startDateVal);
    var endDate = new Date(value);

    if (isNaN(startDate) || isNaN(endDate))
        return true;

    return endDate >= startDate;
});

// Адаптер для unobtrusive validation
$.validator.unobtrusive.adapters.add("notearlierdate", ["startdateproperty"], function (options) {
    options.rules["notearlierdate"] = {
        startdateproperty: options.params.startdateproperty
    };
    options.messages["notearlierdate"] = options.message;
});