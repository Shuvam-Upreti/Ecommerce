$(document).ready(function () {

    UserGrid("#userGrid", "UserDetails");
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


function UserGrid(onLoadElement, exportFileName) {
    $(function () {
        $(onLoadElement).dxDataGrid({
            dataSource: UserDataSorce(),
            allowColumnResizing: true,
            paging: {
                pageSize: 20
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [20, 30, 50, 100]
            },
            grouping: {
                contextMenuEnabled: true
            },
            groupPanel: {
                visible: true   // or "auto"
            },
            sorting: {
                mode: "multiple" // or "multiple" | "none"
            },
            showBorders: true,
            selection: {
                mode: "multiple",// or "multiple" | "none"

            },
            searchPanel: {
                visible: false,
                highlightCaseSensitive: false
            },
            columns: [
                {
                    caption: "Full Name",
                    dataField: "fullName"
                },
                {
                    caption: "Department",
                    dataField: "department"
                },
                {
                    caption: "Email",
                    dataField: "email"
                },
                {
                    caption: "Phone Number",
                    dataField: "phoneNumber"
                },
                {
                    caption: "Date of Joining",
                    dataField: "dateOfJoin",

                    cellTemplate: function (element, info) {
                        var dateOfJoin = ConvertJavascriptDateDigitToDateFormat(info.data.dateOfJoin)
                        element.append(dateOfJoin);
                    }
                },

                {
                    caption: "Roles",
                    dataField: "role"
                },

                {
                    caption: "Action",
                    width: 70,
                    cellTemplate: function (element, info) {
                        element.append(userActionButtons(info.key));
                    }
                }
            ],
            grouping: {
                contextMenuEnabled: true
            },
            sorting: {
                mode: "multiple"
            },
            showBorders: true,

            onContentReady: function () {
                //    initializeCopyToClipboard();
            },
            remoteOperations: {
                paging: true
            },
            onOptionChanged: function (e) {
                if (e.fullName === "paging.pageIndex") {
                    pageIndex = e.value;
                    UserDataSorce();
                }
            },
            //onSelectionChanged: function (selectedItems) {
            //    var selectedRowsData = selectedItems.selectedRowsData;
            //    if (selectedRowsData.length > 1) {
            //        $('#bulk_edit_customer_prority').removeClass('hidden');
            //    } else {
            //        $('#bulk_edit_customer_prority').addClass('hidden');
            //    }
            //}
        });
    });
}



function userActionButtons(dataObj) {
    var html = '';
    var EditDetailUrl = "/Account/Edit?id=" + dataObj.id;
    html += '<a href="' + EditDetailUrl + '"  class="glyphicon glyphicon-edit" data-toggle="tooltip" title="Edit User"></a>';
    html += ' |&nbsp';
    html += '<a  href="#" class="glyphicon glyphicon-trash deleteUser" data-id="' + dataObj.id + '" data-toggle="tooltip" title="Delete User"></a>';

    return html;
}
$(document).on('click', '.deleteUser', function (e) {
    console.log("delete")
    e.preventDefault();
    var userId = $(this).data('id');
    $.ajax({
        url: '/Account/Delete',
        type: 'POST',
        data: { id: userId },
        success: function () {
            toastr.success("Sucessfully deleted user");
            window.location.href = '/Account/Index'
        },
        error: function () {
            toastr.error("Failed to delete the user.");
        }
    })
});
function UserDataSorce() {
    let url = "/Account/LoadUser";
    return new DevExpress.data.DataSource({
        paginate: true,
        load: function (loadOptions) {
            if (typeof loadOptions.skip == 'undefined' && typeof loadOptions.take == 'undefined') {
                loadOptions.skip = 0;
                loadOptions.take = 20;
            }
            var d = $.Deferred();
            var res = $.ajax({
                method: 'POST',
                dataType: "json",
                data: {
                    PageIndex: loadOptions.skip,
                    PageSize: loadOptions.take,
                    //Search: searchText
                },
                url: url,
            }).done(function (result) {

                _totalRowsCount = result.totalCount;
                d.resolve(result.data, { totalCount: result.totalCount });
                /* UnBlockWindow();*/
            }).fail(function (error) {
                console.log()
                /* UnBlockWindow();*/

            });
            return res;
        }
    });
}