let pageIndex = 0;
let pageSize = 20;
let _totalRowsCount = 0;

$(document).ready(function () {
    LetterGrid("#letterGrid", "Letters");

});
function LetterGrid(onLoadElement, exportFileName) {
    $(function () {
        $(onLoadElement).dxDataGrid({
            dataSource: LetterDataSource(),
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
                visible: true,
                highlightCaseSensitive: false
            },
            columns: [
                {
                    caption: "Name",
                    dataField: "name"
                },
                {
                    caption: "Uploaded Date",
                    dataField: "uploadedDate"
                },

                {
                    caption: "Action",
                    width: 120,

                    cellTemplate: function (element, info) {
                        element.append(getPermissibleLinkStringForLetter(info.key));
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
                    LetterDataSource();
                }
            },

        });
    });
}
function LetterDataSource(selectedBranch) {
    let url = "/Home/LoadLetter";
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
                    selectedBranch: selectedBranch
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

function getPermissibleLinkStringForLetter(dataObj) {
    var html = '';

    html += '<a " data-id="' + dataObj.id + '" class="glyphicon glyphicon-download-alt download_letter" data-toggle="tooltip" title="Download Letter"></a> |&nbsp';
    if (dataObj.currentUser === 'SuperAdmin') {
        html += '<a data-id="' + dataObj.id + '" class="glyphicon glyphicon-trash delete_letter" data-toggle="tooltip" title="Delete Letter"></a>';
    }
    return html;

}

$(document).on('click', '.download_letter', function () {
    var Id = $(this).data('id');
    DownloadLetter(Id);
});
function DownloadLetter(Id) {
    window.open("/Home/DownloadLetter?id=" + Id);

}
$(document).on('click', '.delete_letter', function () {
    var Id = $(this).data('id');
    Deleteletter(Id);
})
//function Deleteletter(Id) {
//    window.open("/Home/DeleteLetter?id=" + Id);

//}
function Deleteletter(Id) {
    $.ajax({
        url: '/Home/DeleteLetter',
        method: 'Post', // Ensure this matches what the server expects
        data: { id: Id },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message); 
                LetterGrid("#letterGrid", "Letters");
               
            } else {
                toastr.error(response.message);
               
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', status, error);
            toastr.error('Failed to delete the letter. Please try again.');
        }
    });
}