
$(document).ready(function () {
    $('#SearchCustomer').on('click', function () {

        $('#SearchCustomerModalBody').load('/Home/GetCustomerPartialView', function () {
            $('#SearchCustomerModal').modal('show')
        });
        
        
    });
    $(".ScheduledFollowupCount");
    $(document).on('click', '#btnSearchCustomer', function () {
        var customerCode = $('#txtSearch').val();
       
        $.ajax({
            url: '/Loan/Customer/SearchCustomer',
            type: 'GET',
            data: {
                customerCode: customerCode,
            
            },
            
            success: function (result) {
                if (result && result.id) {
                    // Close the modal
                    $('#SearchCustomerModal').modal('hide');

                    // Redirect to the CustomerDetail view
                    window.location.href = '/Loan/Customer/CustomerDetail?id=' + result.id;
                } else {
                    // Handle cases where result is not valid
                    alert('No customer found or there was an error.');
                }
            },
            error: function () {

            }

        });
    });

});

$(document).ready(function () {
    $.ajax({
        url: '/Loan/Customer/CountTodayFollowUp',
        type: 'GET',
        success: function (result) {
           var count = result.count;
            $('.ScheduledFollowupCount').html("(" + count + ")");
        },
        error: function () {
            console.log('Request failed');
        }
    });
});

function TodayFollowupScheduleGrid(onLoadElement, exportFileName) {
    $(function () {
        $(onLoadElement).dxDataGrid({
            dataSource: TodayFollowUpDataSource(),
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
                    caption: "Customer Code",
                    dataField: "customerCode"
                },
                {
                    caption: "Customer Name",
                    dataField: "customerName"
                },
                {
                    caption: "Loan Main Code",
                    dataField: "loanMainCode"

                },

                {
                    caption: "FollowUp Type",
                    dataField: "followUpType"
                },
                {
                    caption: "Created By",
                    dataField: "createdUserName"
                },
                
                {
                    caption: "Entered Date",
                    dataField: "enteredDate",

                    cellTemplate: function (element, info) {
                        var entereddate = ConvertJavascriptDateDigitToDateFormat(info.data.enteredDate)
                        element.append(entereddate);
                    }

                },
                {
                    caption: "Remarks",
                    dataField: "remarks"
                },
                {
                    caption: "Action",
                    width: 70,

                    cellTemplate: function (element, info) {
                        element.append(getPermissibleLinkStringTodayScheduleFollowupHistory(info.key));
                    }
                },
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

                }
            },

        });
    });
}

function TodayFollowUpDataSource() {
    return new DevExpress.data.DataSource({
        paginate: true,
        load: function (loadOptions) {
            if (typeof loadOptions.skip == 'undefined' && typeof loadOptions.take == 'undefined') {
                loadOptions.skip = 0;
                loadOptions.take = 20;
            }
            var d = $.Deferred();
            var res = $.ajax({
                method: 'Get',
                dataType: "json",
                async: true,
                data: {
                    PageIndex: loadOptions.skip,
                    PageSize: loadOptions.take,
                    
                },
                url: '/Loan/Customer/GetTodayFollowUpSchedule', 
            }).done(function (result) {
                console.log(result)
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

function ConvertJavascriptDateDigitToDateFormat(dateToConvert) {
    if (dateToConvert != null) {
        var date = null;

        if (dateToConvert.indexOf("/Date(") == 0) {
            date = (new Date(parseInt(dateToConvert.replace("/Date(", "").replace(")/", ""))));
        }
        else {
            date = new Date(dateToConvert);
        }
        var dd = date.getDate();
        var mm = date.getMonth() + 1;
        var yyyy = date.getFullYear();
        return `${yyyy}/${mm}/${dd}`;
    }
    return "";
}
function getPermissibleLinkStringTodayScheduleFollowupHistory(dataObj) {
    var html = '';
    //var DetailUrlss = "/Loan/Customer/FollowUpDetail?id=" + dataObj.id;
    //html += '<a href="' + DetailUrlss + '"  class="glyphicon glyphicon-eye-open followupss_btn" data-toggle="tooltip" title="View Customer Detail"></a>&nbsp';

    html += '<a href="javascript:void(0);" data-id="' + dataObj.id + '" class="glyphicon glyphicon-eye-open today_folloup_schedule_btn" data-toggle="tooltip" title="View followup history"></a>';
    return html;
}
$(document).on('click', '.today_folloup_schedule_btn', function () {
    var customerId = $(this).data('id');
    GetTodayScheduleFollowupHistory(customerId);
});
function GetTodayScheduleFollowupHistory(customerId) {
    $.ajax({
        url: '/Loan/Customer/FollowUpDetail',
        method: 'GET',
        data: { id: customerId },
        success: function (data) {
            // Load the partial view into the modal body
            $('#TodayScheduleFollowupHistory').html(data);
            // Show the modal
            $('#GetTodayScheduleFollowupHistoryModal').modal('show');
        },
        error: function () {
            alert('Failed to load FollowUp details.');
        }
    });
}
