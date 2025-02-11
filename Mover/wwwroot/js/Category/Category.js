let pageIndex = 0;
let pageSize = 20;
let _totalRowsCount = 0;

$(document).ready(function () {
    CategoryGrid("#categoryGrid", "CategoryDetails");

    $(document).on('click', '#deleteCategory', function () {
        var categoryId = $(this).data('category-id'); // Get category ID from data attribute
        DeleteCategory(categoryId);
    });

});


function CategoryGrid(onLoadElement, exportFileName) {
    $(function () {
        $(onLoadElement).dxDataGrid({
            dataSource: CategoryDataSorce(),
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
                    caption: "Category Name",
          
                    dataField: "name"
                },
                {
                    caption: "Created On",
         
                    dataField: "createdOn",
                    cellTemplate: function (element, info) {
                        var createdOn = ConvertJavascriptDateDigitToDateFormat(info.data.createdOn)
                        element.append(createdOn);
                    }
                },
                {
                    caption: "Action",
                    width:"100px",    
                    cellTemplate: function (element, info) {
                        // For active or non-completed orders
                        element.append(categoryActionButtons(info.key));
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
                    CategoryDataSorce();
                }
            },
            onSelectionChanged: function (selectedItems) {
                var selectedRowsData = selectedItems.selectedRowsData;

                window.selectedOrders = selectedRowsData;

            }
        });
    });
}
function categoryActionButtons(dataObj) {
    var html = '';
    var EditDetailUrl = "/admin/category/Edit?id=" + dataObj.id;
    var DeleteDetailUrl = "/admin/category/Delete?id=" + dataObj.id;

    // Edit button
    html += '<a href="' + EditDetailUrl + '" class="glyphicon glyphicon-edit nochangeonhover" data-toggle="tooltip" title="Edit Category"></a>';
    html += ' |&nbsp;';

    // Delete button
    html += '<button type="button" id="deleteCategory" class="glyphicon glyphicon-trash nochangeonhover" data-toggle="tooltip" title="Delete Category" style="background:none; border:none; color:red;" data-category-id="' + dataObj.id + '"></button>';

    return html;
}




function CategoryDataSorce() {
    let url = "/admin/category/loadcategories";
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
function DeleteCategory(categoryId) {
    console.log("cat", categoryId);

    if (!categoryId) {
        return;
    }

    console.log("Calling ShowDialog"); // Debugging line

    ShowDialog("Confirm Delivered", "Are you sure you want to delete the category?", "warning")
        .then((result) => {
            console.log("Dialog result:", result); // Debugging line
            if (result.isConfirmed) {
                BlockWindow("Confirming delete...");

                $.ajax({
                    url: '/admin/category/delete',
                    method: 'POSt',
                    data: { categoryId: categoryId },
                    traditional: true,
                    success: function (data) {
                        UnBlockWindow();
                        toastr.success("Category deleted successfully!");
                        window.location.href = '/admin/category/index';
                    },
                    error: function () {
                        UnBlockWindow();

                        toastr.error("Failed to confirm delivery.");
                    }
                });
            }
        });
}

