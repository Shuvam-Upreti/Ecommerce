let pageIndex = 0;
let pageSize = 20;
let _totalRowsCount = 0;

$(document).ready(function () {
    OrderGrid("#orderGrid", "OrderDetails");

    $(document).on('click', ".deleteOrder", function () {
        var orderId = $(this).data('order-id'); 

        DeleteOrder(orderId);
    });
    $('#searchBtn').on('click', function () {
        let orderStatus = $('#filterOrderStatus').val();
        let filterDateFrom = $('#filterDateFrom').val();
        let filterDateTo = $('#filterDateTo').val();
        let searchInput = $('#searchInputOrder').val();
        OrderGrid("#orderGrid", "OrderDetails", orderStatus, filterDateFrom, filterDateTo, searchInput);
    });

});

$('#printSelectedRowsBtn').on('click', function () {
    const grid = $("#orderGrid").dxDataGrid('instance');
    const selectedData = grid.getSelectedRowsData();

    if (selectedData.length === 0) {
        toastr.info("Please select at least one row to print.");
        return;
    }

    // Create an HTML table
    let tableHtml = '<h2>Orders</h2><table border="1" cellspacing="0" cellpadding="8" style="width:100%; font-family:Arial; font-size:12px;">';
    tableHtml += `
        <tr>
            <th>Order ID</th>
            <th>Name</th>
            <th>Phone Number</th>
            <th>Address</th>
            <th>Total Amount</th>
            <th>Order Date</th>
        </tr>`;

    selectedData.forEach(row => {
        tableHtml += `
            <tr>
                <td>${row.orderId}</td>
                <td>${row.createdBy}</td>
                <td>${row.phoneNumber}</td>
                <td>${row.shippingAddressLine}</td>
                <td>${row.totalAmount}</td>
                <td>${ConvertJavascriptDateDigitToDateFormat(row.orderDate)}</td>
            
            </tr>`;
    });

    tableHtml += '</table>';

    const printWindow = window.open('', '', 'width=1000,height=800');
    printWindow.document.write(`
        <html>
        <head>
            <title>Print Selected Orders</title>
        </head>
        <body>
            ${tableHtml}
        </body>
        </html>
    `);
    printWindow.document.close();
    printWindow.focus();

    // Wait for content to render before printing
    setTimeout(() => {
        printWindow.print();
        printWindow.close();
    }, 500);
});

function OrderGrid(onLoadElement, exportFileName, orderStatus, filterDateFrom, filterDateTo, searchInput) {
    $(function () {
        $(onLoadElement).dxDataGrid({
            dataSource: OrderDataSorce(orderStatus, filterDateFrom, filterDateTo, searchInput),
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
                    caption: "Name",
                    dataField: "createdBy"
                },
                {
                    caption: "Phone Number",
                    dataField: "phoneNumber"
                },
                {
                    caption: "Address",
                    dataField: "shippingAddressLine"
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
                    OrderDataSorce(orderStatus, filterDateFrom, filterDateTo, searchInput);
                }
            },
            onSelectionChanged: function (selectedItems) {
                var selectedRowsData = selectedItems.selectedRowsData;

                window.selectedOrders = selectedRowsData;

            }
        });
    });
}
//function orderActionButtons(dataObj) {
//    var html = '';
//    if (dataObj.currentUserRole == "Admin") {
//        var EditDetailUrl = "/admin/order/editorderstatus?id=" + dataObj.orderId;
//        // Edit button
//        html += '<a href="' + EditDetailUrl + '" class="glyphicon glyphicon-edit nochangeonhover" data-toggle="tooltip" title="Edit Product"></a>';
//        html += ' |&nbsp;';
//    }
//    var viewUrl = "/admin/order/details/" + dataObj.orderId;
//    html += '<a href="' + viewUrl + '" class="glyphicon glyphicon-eye-open nochangeonhover" data-toggle="tooltip" title="View Detail">' +
//        '</a>';
//    if (dataObj.orderStatus == "Pending") {
//        html += ' |&nbsp;';
//        // Delete button
//        html += '<a class="glyphicon glyphicon-trash nochangeonhover deleteOrder" data-toggle="tooltip" title="Delete Product" style="background:none; border:none; color:red;" data-order-orderId="' + dataObj.orderId + '"></a>';
//        //html += '<button type="button" id="deleteProduct" class="glyphicon glyphicon-trash nochangeonhover" data-toggle="tooltip" title="Delete Product" style="background:none; border:none; color:red;" data-product-productId="' + dataObj.productId + '"></button>';
//    }

//    return html;
//}
function orderActionButtons(dataObj) {
    var html = '';
    //if (dataObj.currentUserRole == "Admin") {
    //    var EditDetailUrl = "/admin/order/editorderstatus?id=" + dataObj.orderId;
    //    // Edit button using FontAwesome
    //    html += '<a href="' + EditDetailUrl + '" class="nochangeonhover" data-bs-toggle="tooltip" title="Edit Order">';
    //    html += '<i class="fas fa-edit"></i></a>';
    //    html += ' |&nbsp;';
    //}
        if (dataObj.currentUserRole == "Admin") {
            var EditDetailUrl = "/admin/order/editorderstatus?id=" + dataObj.orderId;

            html += '<a href="#" class="open-edit-modal nochangeonhover" data-bs-toggle="tooltip"';
            html += ' data-url="' + EditDetailUrl + '" title="Edit Order">';
            html += '<i class="fas fa-edit"></i></a> |&nbsp;';
    }

    var viewUrl = "/admin/order/details/" + dataObj.orderId;
    // View details button using FontAwesome
    html += '<a href="' + viewUrl + '" class="nochangeonhover" data-bs-toggle="tooltip" title="View Detail">';
    html += '<i class="fas fa-eye"></i></a>';


    if (dataObj.orderStatus == "Pending") {
        html += ' |&nbsp;';
        // Delete button using FontAwesome
        html += '<a class="nochangeonhover deleteOrder" data-bs-toggle="tooltip" title="Delete Order" style="background:none; border:none; color:red;" data-order-id="' + dataObj.orderId + '">';
        html += '<i class="fas fa-trash"></i></a>';
    }

    return html;
}


$(document).on('click', '.open-edit-modal', function (e) {
    e.preventDefault(); // <-- THIS is important
    var url = $(this).data('url');
    $('#editOrderStatusModal .modal-content').load(url, function () {
        $('#editOrderStatusModal').modal('show');
    });
});



function OrderDataSorce(orderStatus, filterDateFrom, filterDateTo, searchInput) {
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
                    orderStatus:  orderStatus,
                    filterDateFrom: filterDateFrom,
                    filterDateTo: filterDateTo,
                    searchInput: searchInput
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

    ShowDialog("Confirm Delete", "Are you sure you want to delete the order?", "warning")
        .then((result) => {
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

$('#sendToLogisticsBtn').on('click', function () {
    const grid = $("#orderGrid").dxDataGrid('instance');
    const selectedData = grid.getSelectedRowsData();

    if (selectedData.length === 0) {
        toastr.info("Please select at least one order.");
        return;
    }

    const orderList = selectedData.map(row => ({
        OrderId: row.orderId,
        CreatedBy: row.createdBy,
        PhoneNumber: row.phoneNumber,
        TotalAmount: row.totalAmount,
        ShippingAddressLine: row.shippingAddressLine
    }));

    $('#sendToLogisticsBtn').prop('disabled', true).text("Sending...");

    $.ajax({
        url: '/admin/order/SendOrderToLogisticsBulk',
        type: 'POST',
        data: JSON.stringify(orderList),
        contentType: 'application/json',
        success: function (response) {
            toastr.success("All selected orders sent successfully.");
            $('#sendToLogisticsBtn').prop('disabled', false).text("Send Selected to Logistics");
            // Optionally: refresh or reload grid
            $("#orderGrid").dxDataGrid("instance").refresh();
        },
        error: function (xhr) {
            toastr.error("Failed to send one or more orders.");
            $('#sendToLogisticsBtn').prop('disabled', false).text("Send Selected to Logistics");
            console.error(xhr.responseText);
        }
    });
});


