var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/OrderInfo/Pen"
        },
        "columns": [
            {
                "data": "id", "width": "20%",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/OrderInfo/Detail/${data}" class="text">${data}</a>
                            </div>
                    `;
                }
            },
            { "data": "name", "width": "20%" },
            { "data": "orderDate", "widht": "20%" },
            { "data": "orderTotal", "width": "20%" },
            { "data": "orderStatus", "width": "20%" }
        ]
    })
}