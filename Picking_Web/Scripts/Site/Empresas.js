$(document).ready(function () {

    var table = $("#empresas").DataTable({
        ajax: {
            url: DefaultApiPath + "/getempresas",
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
            , search: "Buscar", searchPlaceholder: "Buscar por nome..."
        },

        pageLength: -1,
        columns: [
            {
                data: "nome",

                render: function (data, type, empresa) {
                    return "<a data-empresa-edit='" + empresa.id + "'class='edit'>" + data + "</a>";
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
                        location.reload();
                    },
                    error: _DEFAULT_ERROR_TREATMENT
                });
            }
        });
    });

    $("#empresas").on("click", ".edit", function () {
        var id = $(this).attr("data-empresa-edit");
        var dialog = bootbox.dialog({
            title: 'Editar Empresa',
            size: 'large',
            message: '<div data-empresa-edit="' + id + '" id="editEmpresa"><p><i class="fa fa-spin fa-spinner"></i> Aguarde... Isso pode levar ate 1min</p></div>',

        });
        if (id) {
            $.ajax({
                url: "/Empresas/Editar?id=" + id,
                method: "GET",
                success: function (data) {                   

                    $('#editEmpresa').html($(data).html());

                },
                error: _DEFAULT_ERROR_TREATMENT
            });
        } else {
            toastr.error("Erro ao Carregar Empresa", _DEFAULT_ERROR_TIMEOUT);
        }
    });

  

   
});
var $btnCriarCampos = $('#btn-criar-campos');
var $btnTestarConexao = $('#btn-testar-conexao');


function criar_campos_na_base() {
    var id = $("#editEmpresa").attr("data-empresa-edit");
    bootbox.confirm("Deseja criar os campos de usuário nesta empresa?",
        function (result) {
            if (result) {

                $.ajax({
                    url:"API/Empresas/criarcampos/" + id,
                    method: "PUT",
                    beforeSend: function () {
                        $btnCriarCampos.button('loading');
                        aguardeMsg();
                    },
                    success: function (data) {
                        toastr.clear();
                        toastr.success("campos de usuário criados com sucesso.");
                    },
                    error: _DEFAULT_ERROR_TREATMENT
                }).always(function () {
                    $btnCriarCampos.button('reset');
                });
            }
        }
    );
};

function testar_conexao() {
    var id = $("#editEmpresa").attr("data-empresa-edit");
    bootbox.confirm("Deseja testar conexão?",
        function (result) {
            if (result) {

                $.ajax({
                    url:"API/Empresas/testarconexao/" + id,
                    method: "PUT",
                    beforeSend: function () {
                        $btnTestarConexao.button('loading');
                        aguardeMsg();
                    },
                    success: function (data) {
                        toastr.clear();
                        toastr.success("conexão realizada com sucesso.");
                    },
                    error: _DEFAULT_ERROR_TREATMENT
                }).always(function () {
                    $btnTestarConexao.button('reset');
                });
            }
        }
    );
};

$(function () {
    $("#select_deposito").chosen({
        width: "30%"
    });
});