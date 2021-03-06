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
  
        pageLength: -1,
 
     
        columns: [
       
            {
                data: "userName",
                render: function (data, type, usuario) {
                    return "<a  data-user-edit='" + usuario.id + "'>"+data+"</a>"
                }
            },
            {
                data: "email"
            },
            {
                data: "ativo",
                render: function (data, type, usuario) {
                    return data ? "Sim" : "Não";
                }
            },
            {
                data: "licenciado",
                render: function (data, type, usuario) {
                    return data ? "Sim" : "Não";
                }
            },
            {
                data: "operador",
                render: function (data, type, usuario) {
                    return data ? "Sim" : "Não";
                }
            },
            {
                data: "id",
                render: function (data, type, usuario) {
                    return "<button data-usuario-id='" +
                        data +
                        "' type='button' class='btn btn-danger js-delete'><span class='mdi mdi-trash-can-outline'></span >&nbsp;</button>     <input type='submit' value='Editar' class='btn btn-primary edit' data-user-edit='" + usuario.id + "'/>";
                }
            }
        ],
        scrollX: !0,
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
    $("#usuarios").on("click", ".edit", function () {
        var id = $(this).attr("data-user-edit");
        var dialog = bootbox.dialog({
            title: 'Editar Usuarios',
            size: 'large', closeButton: "",
            message: '<div id="editUsuarios"><p><i class="fa fa-spin fa-spinner"></i> Aguarde... Isso pode levar ate 1min</p></div>',

        });
        if (id) {
            $.ajax({
                url: "/Usuarios/Editar?id=" + id,
                method: "GET",
                success: function (data) {
                 
                    $('#editUsuarios').html($(data).html());                

                },
                error: _DEFAULT_ERROR_TREATMENT
            });
        } else {
            toastr.error("Erro ao Carregar Empresa", _DEFAULT_ERROR_TIMEOUT);
        }
    });



 
   
});

function CarregarUsuariosEdit() {
    var DefaultApiPath = 'api/usuarios';
    var opts = "<option>Selecione o Usuário SAP</option>";
    var select_empresa = $('#select_empresaedit');
    var select_usuario = $('#select_usuarioedit');
    var empresa_id = select_empresa[0].selectedOptions[0].value;

    if (empresa_id) {
        $.ajax({
            url: DefaultApiPath + "/getusuariossap?empresa_id=" + empresa_id,
            method: "GET",
            beforeSend: function () {
                select_usuario.html("<option>Carregando...</option>");
            },
            success: function (data) {
                for (var i = 0; i < data.length; i++) {
                    opts += "<option value='" + data[i]["id"] + "'>" + data[i]["nome"] + "</option>";
                }
                select_usuario.html(opts);
            },
            error: _DEFAULT_ERROR_TREATMENT
        });
    }
    select_usuario.html(opts);
};