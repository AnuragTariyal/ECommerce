var dataTable;
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "13%" },
            { "data": "streetAddress", "width": "13%" },
            { "data": "city", "width": "13%" },
            { "data": "state", "width": "13%" },
            { "data": "contactNumber", "width": "16%" },
            {
                "data": "isAuthorizedCompany", "width": "14%",
                "render": function (data) {
                    if (data) {
                        return `
                            <input type="checkbox" checked disabled/>`;
                    }
                    else {
                        return `
                            <input type="checkbox" disabled/>
                        `;
                    }
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                        <a href="/Admin/Company/Upsert/${data}" class="btn btn-info"><i class="fas fa-edit"></i></a>
                        <a class="btn btn-danger" onclick=Delete("/Admin/Company/Delete/${data}")> <i class="fas fa-trash-alt"></i></a>
                        </div>
                        `;
                }
            }

        ]
    })
}

function Delete(url) {
    swal({
        title: "Want to delete the data ?",
        text: "Delete Information!",
        icon: "error",
        buttons: true,
        dangerModel: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}