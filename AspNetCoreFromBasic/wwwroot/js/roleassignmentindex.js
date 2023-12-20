var dataTable;
$(document).ready(function(){
    LoadDataTable();
})
function LoadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/admin/role/GetUserRoles"
        },
        "columns": [
            {"data": "userName","width":"15%"},
            {"data": "roleName","width":"15%"},
            {
                "data": "userId",
                "render": function (data) {
                    return `
                        <div>
                            <a href="/Admin/Role/RoleAssignmentUpdate?UserId=${data}"><i class="bi bi-pen"></i></a> || 
                            <a href="/Admin/Role/RoleAssignmentDetails?UserId=${data}"><i class="bi bi-eye"></i></a> || 
                            <a onClick="Delete('/Admin/Role/RoleAssignmentDelete/${data}')"><i class="bi bi-trash"></i></a>
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