$(document).ready(function () {

    var table = $("#empresas").DataTable({
        ajax: {
            url: DefaultApiPath + "/getempresas",
            dataSrc: ""
        },

        language: {
            paginate: {
                previous: "Anterior",
                next:"Próximo",
                first: "Primeiro",
                last: "Ultimo",


            },


            info: " Mostrando de _START_ até _END_ de _TOTAL_ registros",
            lengthMenu: 'Resultados por paginas <select class=\'form-select form-select-sm ms-1 me-1\'><option value="1">1</option><option value="5">5</option><option value="15">15</option><option value="-1">Todos</option></select>'
            , search: "Buscar", searchPlaceholder: "Buscar por nome..."
        },

        pageLength: 1,
        columns: [
            {
                data: "nome",
                render: function (data, type, empresa) {
                    return "<a href='" + DefaultPath + "/editar/" + empresa.id + "'>" + data + "</a>";
                }
            },
            {
                data: "ativo",
                render: function (data, type, empresa) {
                    return data ? "Sim" : "Não";
                }
            },
            {
                data: "licenciado",
                render: function (data, type, empresa) {
                    return data ? "Sim" : "Não";
                }
            },
            {
                data: "instanciaBanco",
            },
            {
                data: "nomeBanco",
            },
            {
                data: "usuarioBanco",
            },
            {
                data: "id",
                render: function (data, type, empresa) {
                    return "<button data-empresa-id='" +
                        data +
                        "' type='button' class='btn btn-danger js-delete'><span class='mdi mdi-trash-can-outline'></span >&nbsp;</button>";
                }
            }
        ],
        scrollX: !0,

        order: [[1, "asc"]],
        drawCallback: function () {
            $(".dataTables_paginate > .pagination").addClass("pagination-rounded"),
                $("#empresas").addClass("form-label"),
                document.querySelector(".dataTables_wrapper .row").querySelectorAll(".col-md-6").forEach(function (e) {
                    e.classList.add("col-sm-6"),
                        e.classList.remove("col-sm-12"),
                        e.classList.remove("col-md-6")
                })
        },
       
    });

    $("#empresas").on("click", ".js-delete", function () {

        var button = $(this);

        bootbox.confirm("Deseja deletar esta Empresa?", function (result) {

            if (result) {
                $.ajax({
                    url: DefaultApiPath + "/deletarempresas/" + button.attr("data-empresa-id"),
                    method: "DELETE",
                    success: function () {
                        toastr.success("empresa removida com sucesso.");
                        table.row(button.parents("tr")).remove().draw();
                    },
                    error: _DEFAULT_ERROR_TREATMENT
                });
            }
        });
    });
});