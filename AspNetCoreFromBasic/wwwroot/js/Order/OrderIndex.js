var dataTable;
$(document).ready(function () {
    $('#tblData thead tr')
        .clone(true)
        .addClass('filters')
        .appendTo('#tblData thead');
    var url = window.location.search;
    debugger;
    switch (url) {
        case url.includes("pending") ? url : "Other":
            LoadDataTable("Pending")
            break;
        case url.includes("approved") ? url : "Other":
            LoadDataTable("Approved")
            break;
        case url.includes("processing") ? url : "Other":
            LoadDataTable("Processing")
            break;
        case url.includes("shipped") ? url : "Other":
            LoadDataTable("Shipped");
            break;
        default:
            LoadDataTable()
            break;
    }
})
function LoadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll",
            "type": "POST",
            "contentType": "application/json",
            "data": function (d) {
                debugger;
                d.status = status == undefined?'':status;
                return JSON.stringify(d);
            }
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "phoneNumber", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            { "data": "orderDate", "width": "10%" },
            { "data": "paymentDate", "width": "10%" },
            { "data": "shippingDate", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            { "data": "paymentStatus", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div>
                            <a href="/Admin/Order/Edit?Id=${data}"><i class="bi bi-pen"></i></a> || 
                            <a href="/Admin/Order/Details?Id=${data}"><i class="bi bi-eye"></i></a> || 
                            <a onClick="Delete('/Admin/Order/Delete/${data}')"><i class="bi bi-trash"></i></a>
                        </div>
                    `
                },
                "width": "15%"
            }
        ],
        "columnDefs": [
            {
                targets: [3, 4, 5],
                render: function (data, type, row) {
                    if (type === 'display') {
                        if (moment(data).format('YYYY') === '0001') {
                            return '';
                        }
                        return moment(data).format('YYYY-MM-DD');
                        //return moment(data).format('YYYY-MM-DD hh:mm A');
                    }
                    return data;
                }
            }
        ],
        orderCellsTop: true,
        ordering: false,
        fixedHeader: true,
        searching: true,
        processing: true,
        serverSide: true,
        sInfo: "Showing _START_ to _END_ of _TOTAL_ entries",
        dom: 'rt<"bottom"<"d-flex justify-content-between"lip>>',
        initComplete: function () {
            var api = this.api();
            $("#tblData_info").addClass("pt-2");
            api
                .columns()
                .eq(0)
                .each(function (colIdx) {
                    var cell = $('.filters th').eq(
                        $(api.column(colIdx).header()).index()
                    );
                    var title = $(cell).text();
                    if (title != "") {
                        $(cell).html('<input type="text" id="'+title.replace(' ','')+'" style="width:100px;" />');
                    }
                    $(
                        'input',
                        $('.filters th').eq($(api.column(colIdx).header()).index())
                    )
                        .off('keyup change')
                        .on('change', function (e) {
                            // Get the search value
                            $(this).attr('title', $(this).val());
                            var regexr = '({search})'; 

                            var cursorPosition = this.selectionStart;
                            api
                                .column(colIdx)
                                .search(
                                    this.value != ''
                                        ? regexr.replace('{search}', '(((' + this.value + ')))')
                                        : '',
                                    this.value != '',
                                    this.value == ''
                                )
                                .draw();
                        })
                        .on('keyup', function (e) {
                            e.stopPropagation();

                            $(this).trigger('change');
                            $(this)
                                .focus()[0]
                                .setSelectionRange(cursorPosition, cursorPosition);
                        });
                });
        },
    })
}