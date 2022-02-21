$(document).ready(function () {

    var table = $("#usuarios").DataTable({
        ajax: {
            url: DefaultApiPath + "/getusuarios",
            dataSrc: ""
        },
        
        language: {
            paginate: {
                previous: "Anterior",
                next: "Próximo",
                first: "Primeiro",
                last: "Ultimo",


            },
            
            
            info: " Mostrando de _START_ até _END_ de _TOTAL_ registros",
            lengthMenu: 'Resultados por paginas <select class=\'form-select form-select-sm ms-1 me-1\'><option value="1">1</option><option value="5">5</option><option value="15">15</option><option value="-1">Todos</option></select>'
            , search: "Buscar", searchPlaceholder:"Buscar por nome..."
        },
  
        pageLength: 1,
 
     
        columns: [
       
            {
                data: "userName",
                render: function (data, type, usuario) {
                    return "<a  href='" + DefaultPath + "/editar/" + usuario.id + "'>" + data + "</a>"
                }
            },
            {
                data: "email"
            },
            {
                data: "ativo ",
                render: function (data, type, usuario) {
                    return data ? "Sim" : "Não";
                }
            },
            {
                data: "licenciado ",
                render: function (data, type, usuario) {
                    return data ? "Sim" : "Não";
                }
            },
            {
                data: "operador ",
                render: function (data, type, usuario) {
                    return data ? "Sim" : "Não";
                }
            },
            {
                data: "id",
                render: function (data, type, usuario) {
                    return "<button data-usuario-id='" +
                        data +
                        "' type='button' class='btn btn-danger js-delete'><span class='mdi mdi-trash-can-outline'></span >&nbsp;</button>";
                }
            }
        ],     
        order: [[1, "asc"]],
        drawCallback: function () {
            $(".dataTables_paginate > .pagination").addClass("pagination-rounded "),
                $("#usuarios").addClass("form-label"),
                document.querySelector(".dataTables_wrapper .row").querySelectorAll(".col-md-6").forEach(function (e) {
                    e.classList.add("col-sm-6"),
                        e.classList.remove("col-sm-12"),
                        e.classList.remove("col-md-6")
                })
        }, 

    });

    $("#usuarios").on("click", ".js-delete", function () {

        var button = $(this);

        bootbox.confirm("Deseja deletar este usuário?", function (result) {

            if (result) {
                $.ajax({
                    url: DefaultApiPath + "/deletarusuario/" + button.attr("data-usuario-id"),
                    method: "DELETE",
                    success: function () {
                        toastr.success("Usuário removido com sucesso.");
                        table.row(button.parents("tr")).remove().draw();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        _DEFAULT_ERROR_TREATMENT(jqXHR, textStatus, errorThrown, "Erro ao deletar Usuário");
                    }
                });
            }
        });
    });
});