let pageIndex = 0;
let pageSize = 20;
let _totalRowsCount = 0;

$(document).ready(function () {
    ProductGrid("#productGrid", "ProductDetails");

    $(document).on('click', '#deleteProduct', function () {
        var productId = $(this).data('productProductid'); // This should correctly retrieve the productId
        console.log("Product ID:", productId); // Add this log to confirm the value of productId

        DeleteProduct(productId);
    });


});


function ProductGrid(onLoadElement, exportFileName) {
    $(function () {
        $(onLoadElement).dxDataGrid({
            dataSource: ProductDataSorce(),
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
                    caption: "Product Name",
          
                    dataField: "productName"
                },
                {
                    caption: "Description",
          
                    dataField: "description"
                },
                {
                    caption: "Price",
                    dataField: "discountedPrice",
                },
                {
                    caption: "Category",
                    dataField: "category",
                },
                {
                    caption: "Action",
                    width:"100px",    
                    cellTemplate: function (element, info) {
                        // For active or non-completed orders
                        element.append(productActionButtons(info.key));
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
                    ProductDataSorce();
                }
            },
            onSelectionChanged: function (selectedItems) {
                var selectedRowsData = selectedItems.selectedRowsData;

                window.selectedOrders = selectedRowsData;

            }
        });
    });
}
function productActionButtons(dataObj) {
    var html = '';
    var EditDetailUrl = "/admin/product/Edit?id=" + dataObj.productId;
    console.log(dataObj.productId);
    // Edit button
    html += '<a href="' + EditDetailUrl + '" class="glyphicon glyphicon-edit nochangeonhover" data-toggle="tooltip" title="Edit Product"></a>';
    html += ' |&nbsp;';

    // Delete button
    html += '<button type="button" id="deleteProduct" class="glyphicon glyphicon-trash nochangeonhover" data-toggle="tooltip" title="Delete Product" style="background:none; border:none; color:red;" data-product-productId="' + dataObj.productId + '"></button>';
    //html += '<button type="button" id="deleteProduct" class="glyphicon glyphicon-trash nochangeonhover" data-toggle="tooltip" title="Delete Product" style="background:none; border:none; color:red;" data-product-productId="' + dataObj.productId + '"></button>';

    return html;
}




function ProductDataSorce() {
    let url = "/admin/product/loadproducts";
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
function DeleteProduct(productId) {
    console.log("cat", productId);

    if (!productId) {
        return;
    }

    console.log("Calling ShowDialog"); // Debugging line

    ShowDialog("Confirm Delivered", "Are you sure you want to delete the product?", "warning")
        .then((result) => {
            console.log("Dialog result:", result); // Debugging line
            if (result.isConfirmed) {
                BlockWindow("Confirming delete...");

                $.ajax({
                    url: '/admin/product/delete',
                    method: 'POSt',
                    data: { productId: productId },
                    traditional: true,
                    success: function (data) {
                        UnBlockWindow();
                        toastr.success("Product deleted successfully!");
                        window.location.href = '/admin/product/index';
                    },
                    error: function () {
                        UnBlockWindow();

                        toastr.error("Failed to delete product.");
                    }
                });
            }
        });
}

