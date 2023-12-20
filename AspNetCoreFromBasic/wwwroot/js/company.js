var dataTable;
$(document).ready(function(){
    LoadDataTable();
})
function LoadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url" : "/admin/company/getall"
        },
        "columns": [
            { "data": "name","width":"15%"},
            { "data": "streetAddress","width":"15%"},
            { "data": "city","width":"20%"},
            { "data": "state", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div>
                            <a href="/Admin/Company/Edit?Id=${data}"><i class="bi bi-pen"></i></a> || 
                            <a href="/Admin/Company/Details?Id=${data}"><i class="bi bi-eye"></i></a> || 
                            <a onClick="Delete('/Admin/Company/Delete/${data}')"><i class="bi bi-trash"></i></a>
                        </div>
                    `
                },
                "width": "15%"
            }
        ]
    })
}
function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                method: "delete",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message)
                    } else {
                        toastr.error(data.message)
                    }
                }
            });
        }
    });
}