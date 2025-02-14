let pageIndex = 0;
let pageSize = 20;
let _totalRowsCount = 0;

$(document).ready(function () {
    OrderGrid("#orderGrid", "OrderDetails");

    $(document).on('click', ".deleteOrder", function () {
        var orderId = $(this).data('orderOrderid'); // This should correctly retrieve the productId
        console.log("Product ID:", orderId); // Add this log to confirm the value of productId

        DeleteOrder(orderId);
    });


});


function OrderGrid(onLoadElement, exportFileName) {
    $(function () {
        $(onLoadElement).dxDataGrid({
            dataSource: OrderDataSorce(),
            allowColumnResizing: true,
            paging: {
                pageSize: 20
            },
            pager: {
                showPageSizeSelector: true,
                showNavigationButtons: true,
                showInfo: true,
                allowedPageSizes: [20, 30, 50, 100],
                visible: true,
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
                    caption: "Order Id",

                    dataField: "orderId"
                },
                {
                    caption: "Created By",
                    dataField: "createdBy"
                },
                {
                    caption: "Phone Number",

                    dataField: "phoneNumber"
                },
                {
                    caption: "Total Amount",
                    dataField: "totalAmount",
                },
                {
                    caption: "Order Date",
                    dataField: "orderDate",
                    cellTemplate: function (element, info) {
                        var orderDate = ConvertJavascriptDateDigitToDateFormat(info.data.orderDate)
                        element.append(orderDate);
                    }
                },
                {
                    caption: "Order Status",
                    dataField: "orderStatus",
                },
                {
                    caption: "Action",
                    width: "100px",
                    cellTemplate: function (element, info) {
                        // For active or non-completed orders
                        element.append(orderActionButtons(info.key));
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
                    OrderDataSorce();
                }
            },
            onSelectionChanged: function (selectedItems) {
                var selectedRowsData = selectedItems.selectedRowsData;

                window.selectedOrders = selectedRowsData;

            }
        });
    });
}
function orderActionButtons(dataObj) {
    var html = '';
    if (dataObj.currentUserRole == "Admin") {
        var EditDetailUrl = "/admin/order/editorderstatus?id=" + dataObj.orderId;
        // Edit button
        html += '<a href="' + EditDetailUrl + '" class="glyphicon glyphicon-edit nochangeonhover" data-toggle="tooltip" title="Edit Product"></a>';
        html += ' |&nbsp;';
    }
    var viewUrl = "/admin/order/details/" + dataObj.orderId;
    html += '<a href="' + viewUrl + '" class="glyphicon glyphicon-eye-open nochangeonhover" data-toggle="tooltip" title="View Detail">' +
        '</a>';
    if (dataObj.orderStatus == "Pending") {
        html += ' |&nbsp;';
        // Delete button
        html += '<a class="glyphicon glyphicon-trash nochangeonhover deleteOrder" data-toggle="tooltip" title="Delete Product" style="background:none; border:none; color:red;" data-order-orderId="' + dataObj.orderId + '"></a>';
        //html += '<button type="button" id="deleteProduct" class="glyphicon glyphicon-trash nochangeonhover" data-toggle="tooltip" title="Delete Product" style="background:none; border:none; color:red;" data-product-productId="' + dataObj.productId + '"></button>';
    }

    return html;
}




function OrderDataSorce() {
    let url = "/admin/order/LoadOrders";
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
                d.resolve({
                    data: result.data,
                    totalCount: result.totalCount
                });

                /* UnBlockWindow();*/
            }).fail(function (error) {
                console.log();
                d.reject(error);
                /* UnBlockWindow();*/

            });
            return res;
        }
    });
}
function DeleteOrder(orderId) {
    console.log("cat", orderId);

    if (!orderId) {
        return;
    }

    console.log("Calling ShowDialog"); // Debugging line

    ShowDialog("Confirm Delivered", "Are you sure you want to delete the order?", "warning")
        .then((result) => {
            console.log("Dialog result:", result); // Debugging line
            if (result.isConfirmed) {
                BlockWindow("Confirming delete...");

                $.ajax({
                    url: '/admin/order/delete',
                    method: 'POST',
                    data: { id: orderId },
                    traditional: true,
                    success: function (data) {
                        UnBlockWindow();
                        toastr.success("Product deleted successfully!");
                        window.location.href = '/admin/order/index';
                    },
                    error: function () {
                        UnBlockWindow();

                        toastr.error("Failed to delete product.");
                    }
                });
            }
        });
}

