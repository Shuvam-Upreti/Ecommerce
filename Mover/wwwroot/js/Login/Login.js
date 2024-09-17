$(document).ready(function () {
});
function seralizeFormCommon(formDom) {
    var data = new FormData();
    formDom.find("input,select,textarea,td").each(function (inpIndex, inpElement) {
        var propName = $(inpElement).attr("name");
        if ($(inpElement).attr("type") === "file") {
            if ($(inpElement)[0].files[0] !== undefined) {
                if ($(inpElement).attr("multiple") !== undefined) {
                    $.each($(inpElement)[0].files, function (key, value) {
                        data.append(propName + "[" + key + "]", $(inpElement)[0].files[key]);
                    })
                } else {
                    data.append(propName, $(inpElement)[0].files[0]);
                }
                //console.log(propName, $(inpElement)[0].files);
            }
        }
        else {
            var inputValue = $(inpElement).val();
            if ($(inpElement).attr("type") == "checkbox") {
                inputValue = $(inpElement).is(":checked");
            }
            else if ($(inpElement).hasClass('comma-number')) {
                data.append(propName, inputValue.replace(/,/g, ""))
            }
            //if array
            else if (inputValue && inputValue.constructor === Array) {
                $.each(inputValue, function (index, value) {
                    data.append(propName + "[" + index + "]", value)
                })
            } else {
                data.append(propName, inputValue);
            }
        }
    });
    return data;
}
