
$.validator.addMethod("checkenumrange", function (value, element) {
    debugger;
    //if (value === null || value === undefined || value === "") {
    //    return false;
    //}
    //console.log('Hello 123')
    //var intValue = parseInt(value, 10);

    //if (isNaN(intValue)) {
    //    return false; // If not a number, fail validation
    //}

    //// Retrieve the min and max values from the data attributes
    //var minValueOfEnum = parseInt($(element).data('min-value-of-enum'), 10);
    //var maxValueOfEnum = parseInt($(element).data('max-value-of-enum'), 10);

    //return intValue >= minValueOfEnum && intValue <= maxValueOfEnum;
});
$.validator.unobtrusive.adapters.add("checkenumrange", function (options) {
    //debugger;
    //options.rules["checkenumrange"] = []
    //options.messages["checkenumrange"] = options.message;
});

$.validator.addMethod("checkcity", function (value, element) {
    debugger;
    if (value === null || value === undefined || value === "") {
        return false;
    }
    var listOfCitiesInString = element.dataset.listOfCities;
    var arrayofCities = listOfCitiesInString.split(",");
    var cityNameFilled = value.charAt(0).toUpperCase() + value.slice(1);
    
    return arrayofCities.includes(cityNameFilled);
    //var maxValueOfEnum = parseInt($(element).data('max-value-of-enum'), 10);

    //return intValue >= minValueOfEnum && intValue <= maxValueOfEnum;
});
$.validator.unobtrusive.adapters.add("checkcity", function (options) {
    options.rules["checkcity"] = []
    options.messages["checkcity"] = options.message;
});

