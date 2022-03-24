$(document).ready(function () {

    var empresa_id = document.getElementById("script_lista_picking").getAttribute("data-empresa");
    var GetSearchValue = function () {
        return $('#campo_pesquisa').val();
    };
    var GetURLPickingAberto = function () {
        return DefaultApiPath + "/listapedidosemaberto?empresa_id=" +
            empresa_id +
            "&numdoc=" + GetSearchValue();
    }
    var GetURLSeparacaoFutura = function () {
        return DefaultApiPath + "/listapedidosemseparacaofutura?empresa_id=" +
            empresa_id +
            "&numdoc=" + GetSearchValue();
    }
    var GetURLDocsPendentes = function () {
        return DefaultApiPath + "/listapedidospendentes?empresa_id=" +
            empresa_id +
            "&numdoc=" + GetSearchValue();
    }
    var GetURLTransferencias = function () {
        return DefaultApiPath + "/listatransferencias?empresa_id=" + empresa_id
    }
    var GetURLItensPedidoParcial = function () {
        return DefaultApiPath + "/GetItensPedidoListaParcial?empresa_id=" +
            empresa_id +
            "&numdoc=" + GetSearchValue();
    }
    var columns_default = [
        {
            data: "tipo"
        },
        {
            data: "numDoc"
        },
        {
            data: "prioridade",
            render: function (data, type, row) {
                return row._prioridade === 'S' ? "<span class='item-prioritario'>" + data + "</span>" : data;
            }
        },
        {
            data: "nomeCliente"
        },
        {
            data: "cidade"
        },
        {
            data: "dataEntrega",
            render: function (data, type, row) {
                return data + "  " + row.horaEntrega;
            }
        },
        {
            data: "vendedor"
        },
        {
            data: "observacoes"
        },
        {
            data: "numDoc",
            className: 'td-button',
            render: function (data, type, row) {
                return "<button data-numdoc-id='" +
                    data +
                    "' type='button' class='btn btn-primary js-gerar-parcial'><span class='glyphicon glyphicon-ok'></span >&nbsp;</button>";
            }
        },
        //{
        //    data: "numDoc",
        //    className: 'td-button',
        //    render: function(data, type, row) {
        //        return "<button data-numdoc-id='" +
        //            data +
        //            "' type='button' class='btn btn-success js-gerar-lista'><span class='glyphicon glyphicon-ok'></span >&nbsp;</button>";
        //    }
        //},
        {
            data: "numDoc",
            className: 'td-button',
            render: function (data, type, row) {
                return "<button data-numdoc-id='" +
                    data +
                    "' type='button' class='btn btn-danger js-impedir-lista'><span class='glyphicon glyphicon-warning-sign'></span >&nbsp;</button>";
            }
        }
    ];
    var lista_picking_itens = [];

    var datatable_lista_picking_aberto = $("#lista_picking_em_aberto").DataTable({
        ajax: {
            url: GetURLPickingAberto(),
            dataSrc: "",
            method: "GET",
            error: _DEFAULT_ERROR_TREATMENT
        },
        columns: columns_default, order: [5, "asc"],
        language: {
            paginate: {
                previous: "Anterior",
                next: "Próximo",
                first: "Primeiro",
                last: "Ultimo",
            },
            info: "Mostrando de _START_ até _END_ de _TOTAL_ registros",
            lengthMenu: 'Resultados por paginas <select class=\'form-select form-select-sm ms-1 me-1\'><option value="1">1</option><option value="5">5</option><option value="15">15</option><option value="-1">Todos</option></select>'
            , search: "Buscar", searchPlaceholder: "Buscar por nome..."
        },
        pageLength: 5,
        scrollX: !0,
        drawCallback: function () {
            $(".dataTables_paginate > .pagination").addClass("pagination-rounded"),
                $("#lista_picking_em_aberto").addClass("form-label"),
                document.querySelector(".dataTables_wrapper .row").querySelectorAll(".col-md-6").forEach(function (e) {
                    e.classList.add("col-sm-6"),
                        e.classList.remove("col-sm-12"),
                        e.classList.remove("col-md-6")
                })
        },



    });
    var datatable_lista_separacao_futura = $("#lista_picking_separacao_futura").DataTable({
        ajax: {
            url: GetURLSeparacaoFutura(),
            dataSrc: "",
            method: "GET",
            error: _DEFAULT_ERROR_TREATMENT
        },
        //columns: columns_default.slice(0, 8),
        columns: columns_default,
        order: [5, "asc"],
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
        pageLength: 5,
        scrollX: !0,
        drawCallback: function () {
            $(".dataTables_paginate > .pagination").addClass("pagination-rounded"),
                $("#lista_picking_separacao_futura").addClass("form-label"),
                document.querySelector(".dataTables_wrapper .row").querySelectorAll(".col-md-6").forEach(function (e) {
                    e.classList.add("col-sm-6"),
                        e.classList.remove("col-sm-12"),
                        e.classList.remove("col-md-6")
                })
        },
    });
    var datatable_lista_docs_pendentes = $("#lista_picking_docs_pendentes").DataTable({
        ajax: {
            url: GetURLDocsPendentes(),
            dataSrc: "",
            method: "GET",
            error: _DEFAULT_ERROR_TREATMENT
        },
        order: [5, "asc"],
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
        pageLength: 5,
        scrollX: !0,
        drawCallback: function () {
            $(".dataTables_paginate > .pagination").addClass("pagination-rounded"),
                $("#lista_picking_docs_pendentes").addClass("form-label"),
                document.querySelector(".dataTables_wrapper .row").querySelectorAll(".col-md-6").forEach(function (e) {
                    e.classList.add("col-sm-6"),
                        e.classList.remove("col-sm-12"),
                        e.classList.remove("col-md-6")
                })
        },
        ColumnSettings: { width: "100%" },
        columns: [
            {
                data: "tipo"
            },
            {
                data: "numDoc"
            },
            {
                data: "prioridade",
                render: function (data, type, row) {
                    return row._prioridade === 'S' ? "<span class='item-prioritario'>" + data + "</span>" : data;
                }
            },
            {
                data: "nomeCliente"
            },
            {
                data: "cidade"
            },
            {
                data: "dataEntrega",
                render: function (data, type, row) {
                    return data + "  " + row.horaEntrega;
                }
            },
            {
                data: "vendedor"
            },
            {
                data: "observacoes"
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" +
                        data +
                        "' type='button' class='btn btn-primary js-gerar-parcial'><span class='mdi mdi-check'></span >&nbsp;</button>";
                }
            },
            //{
            //    data: "numDoc",
            //    className: 'td-button',
            //    render: function (data, type, row) {
            //        return "<button data-numdoc-id='" +
            //            data +
            //            "' type='button' class='btn btn-success js-gerar-lista'><span class='glyphicon glyphicon-ok'></span >&nbsp;</button>";
            //    }
            //},
            {
                data: "statusPicking",
                className: 'td-button',
                render: function (data, type, row) {

                    var disabled = data === 'SP' ? "" : "disabled='disabled'";

                    return "<button data-numdoc-id='" +
                        row.numDoc +
                        "' type='button' " + disabled + " class='btn btn-primary js-desimpedir-lista'><span class='mdi mdi-upload'></span >&nbsp;</button>";
                }
            }
        ]

    });
    var datatable_lista_transferencias = $("#lista_picking_transferencias").DataTable({
        order: [],
        ajax: {
            url: GetURLTransferencias(),
            dataSrc: "",
            method: "GET",
            error: _DEFAULT_ERROR_TREATMENT
        },
        ColumnSettings: { width: "100%" },
        columns: columns_default.slice(0, 8),
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
        }, scrollX: !0,

        pageLength: 5,
        drawCallback: function () {
            $(".dataTables_paginate > .pagination").addClass("pagination-rounded"),
                $("#lista_picking_transferencias").addClass("form-label"),
                document.querySelector(".dataTables_wrapper .row").querySelectorAll(".col-md-6").forEach(function (e) {
                    e.classList.add("col-sm-6"),
                        e.classList.remove("col-sm-12"),
                        e.classList.remove("col-md-6")
                })

        },

    });

    var datatable_itens_pedido_parcial = $("#itens_pedido_parcial").DataTable({
        order: [],
        searching: false,    
        paging: false,
        info: false,
        autoWidth: false,
        ajax: {
            url: GetURLItensPedidoParcial(),
            dataSrc: "",
            method: "GET",
            error: function (jqXHR, textStatus, errorThrown) {
                toastr.clear();
                var msg = jqXHR.responseJSON.message;
                toastr.error(msg, "Erro", _DEFAULT_ERROR_TIMEOUT);
                audio.play();
            }
        },
        columns: [
            {
                data: "item"
            },
            {
                data: "descricao",
                className: 'td-center',
            },
            //{
            //    data: "numSerie",
            //    className: 'td-center',
            //},
            //{
            //    data: "codigoBarras",
            //    className: 'td-center',
            //},
            //{
            //    data: "numLote",
            //    className: 'td-center',
            //},
            //{
            //    data: "quantidadeTotal",
            //    className: 'td-center',
            //},
        ],
        drawCallback: function () {
            
        },
      
    });

    var RefreshDatatables = function () {
        datatable_lista_picking_aberto.ajax.url(GetURLPickingAberto()).load();
        datatable_lista_separacao_futura.ajax.url(GetURLSeparacaoFutura()).load();
        datatable_lista_docs_pendentes.ajax.url(GetURLDocsPendentes()).load();
    };

    $("#lista_picking_em_aberto,#lista_picking_docs_pendentes,#lista_picking_separacao_futura").on("click", ".js-gerar-lista", function () {

        var button = $(this),
            div_id_name = "detalhe_lista_picking_em_aberto",
            btn_add_lista_classname = "btn-add-lista",
            error_class = "input-validation-error";

        var dialog = bootbox.dialog({
            title: 'Detalhamento da Lista de Picking',
            size: 'large',
            message: '<div id="' + div_id_name + '"><p><i class="fa fa-spin fa-spinner"></i> Carregando...</p></div>',
            buttons: {
                cancel: {
                    label: 'Cancelar',
                    className: 'btn-default',
                },
                adicionar: {
                    label: 'Adicionar',
                    className: 'btn-success ' + btn_add_lista_classname,
                    callback: function () {
                        var $btnAdd = $('.' + btn_add_lista_classname);
                        $btnAdd.attr('data-loading-text', 'Aguarde...');

                        $("#" + div_id_name + " #operadorespicking").removeClass(error_class);
                        var operador = $("#" + div_id_name + " #operadorespicking").val();
                        if (operador) {
                            $.ajax({
                                url: DefaultApiPath + "/gerarlistapicking",
                                method: "POST",
                                data: {
                                    empresa_id: empresa_id,
                                    numdoc: $("#" + div_id_name + " #numdoc").val(),
                                    operador: operador,
                                    usuario_logado_nome: $("#" + div_id_name + " #usuario_logado_nome").val(),
                                    observacoes: $("#" + div_id_name + " #observacoespicking").val(),
                                    nome_impressora: $('#nome_impressora').val()
                                },
                                beforeSend: function () {
                                    $btnAdd.button('loading');
                                    aguardeMsg();
                                },
                                success: function () {

                                    toastr.clear();
                                    bootbox.hideAll();
                                    toastr.success("Lista de Picking gerada com sucesso.");

                                    RefreshDatatables();

                                },
                                error: _DEFAULT_ERROR_TREATMENT
                            }).always(function () {
                                $btnAdd.button('reset');
                            });
                        } else {
                            $("#" + div_id_name + " #operadorespicking").addClass(error_class);
                            toastr.error("Selecione o Operador.", _DEFAULT_ERROR_TIMEOUT);
                        }
                        return false;
                    },
                },
            },
        });

        var num_doc = button.attr("data-numdoc-id");

        if (num_doc) {
            $.ajax({
                url: DefaultApiPath + "/detalhelistapicking?empresa_id=" + empresa_id + "&numdoc=" + num_doc,
                method: "GET",
                success: function (data) {

                    $('#detalhe_lista_picking_em_aberto').html($('#form_lista_picking_detalhe').html());

                    if (data.numPicking) {
                        $('#' + div_id_name + ' #numpicking').val(data.numPicking);
                    }
                    if (data.dataPicking) {
                        $('#' + div_id_name + ' #datapicking').val(data.dataPicking);
                    }
                    if (data.usuario) {
                        $('#' + div_id_name + ' #usuariopicking').val(data.usuario);
                    }
                    if (data.operadores) {
                        var options = "<option value=''>Escolha um Operador</option>";
                        for (var i = 0; i < data.operadores.length; i++) {
                            options += "<option value= '" +
                                data.operadores[i]['id'] +
                                "'>" +
                                data.operadores[i]['userName'] +
                                "</option >";
                        }
                        $('#' + div_id_name + ' #operadorespicking').html(options);
                    }

                    $('#' + div_id_name + ' #numdoc').val(num_doc);
                }
            });
        } else {
            toastr.error("Pedido de Venda não encontrado.", _DEFAULT_ERROR_TIMEOUT);
        }
    });


    $("#lista_picking_em_aberto,#lista_picking_docs_pendentes,#lista_picking_separacao_futura").on("click", ".js-gerar-parcial", function () {

        var button = $(this),
            div_id_name = "detalhe_lista_picking_em_aberto_parcial",
            btn_add_lista_classname = "btn-add-lista",
            btn_add_lista_completa_classname = "btn-add-lista_completa",
            error_class = "input-validation-error";

        var dialog = bootbox.dialog({
            title: 'Detalhamento da Lista de Picking Parcial',
            size: 'large',
            message: '<div id="' + div_id_name + '"><p><i class="fa fa-spin fa-spinner"></i> Carregando...</p></div>',
            buttons: {
                cancel: {
                    label: 'Cancelar',
                    className: 'btn-default',
                },
                adicionar: {
                    label: 'Adicionar',
                    className: 'btn-success ' + btn_add_lista_classname,
                    callback: function () {
                        var $btnAdd = $('.' + btn_add_lista_classname);
                        $btnAdd.attr('data-loading-text', 'Aguarde...');

                        $("#" + div_id_name + " #operadorespicking").removeClass(error_class);
                        var operador = $("#" + div_id_name + " #operadorespicking").val();
                        var num_doc = button.attr("data-numdoc-id");
                        if (operador) {
                            for (var i = 0; i < lista_picking_itens.length; i++) {
                                lista_picking_itens[i].qtdPicking = $("#num" + i).val();
                                lista_picking_itens[i].check = $("#chk" + i).is(":checked");
                            }
                            $.ajax({
                                url: DefaultApiPath + "/gerarlistapickingparcial",
                                method: "POST",
                                data: {
                                    empresa_id: empresa_id,
                                    numdoc: num_doc,
                                    //numdoc: $("#" + div_id_name + " #numdoc").val(),
                                    operador: operador,
                                    usuario_logado_nome: $("#" + div_id_name + " #usuario_logado_nome").val(),
                                    observacoes: $("#" + div_id_name + " #observacoespicking").val(),
                                    nome_impressora: $('#nome_impressora').val(),
                                    itens: JSON.stringify(lista_picking_itens)
                                },
                                beforeSend: function () {
                                    $btnAdd.button('loading');
                                    aguardeMsg();
                                },
                                success: function (response) {

                                    toastr.clear();
                                    bootbox.hideAll();
                                    toastr.success("Lista de Picking gerada com sucesso.");

                                    RefreshDatatables();

                                },
                                error: _DEFAULT_ERROR_TREATMENT
                            }).always(function () {
                                $btnAdd.button('reset');
                            });
                        } else {
                            $("#" + div_id_name + " #operadorespicking").addClass(error_class);
                            toastr.error("Selecione o Operador.", _DEFAULT_ERROR_TIMEOUT);
                        }
                        return false;
                    },
                },
                adicionarCompleto: {
                    label: 'Adicionar Pedido Completo',
                    className: 'btn-primary ' + btn_add_lista_completa_classname,
                    callback: function () {

                        var $btnAdd = $('.' + btn_add_lista_completa_classname);
                        $btnAdd.attr('data-loading-text', 'Aguarde...');

                        $("#" + div_id_name + " #operadorespicking").removeClass(error_class);
                        var operador = $("#" + div_id_name + " #operadorespicking").val();
                        var num_doc = button.attr("data-numdoc-id");
                        if (operador) {
                            for (var i = 0; i < lista_picking_itens.length; i++) {
                                lista_picking_itens[i].qtdPicking = $("#num" + i).val();
                                lista_picking_itens[i].check = $("#chk" + i).is(":checked");
                            }
                            $.ajax({
                                url: DefaultApiPath + "/gerarlistapickingcompleta",
                                method: "POST",
                                data: {
                                    empresa_id: empresa_id,
                                    numdoc: num_doc,
                                    //numdoc: $("#" + div_id_name + " #numdoc").val(),
                                    operador: operador,
                                    usuario_logado_nome: $("#" + div_id_name + " #usuario_logado_nome").val(),
                                    observacoes: $("#" + div_id_name + " #observacoespicking").val(),
                                    nome_impressora: $('#nome_impressora').val(),
                                    itens: JSON.stringify(lista_picking_itens)
                                },

                                beforeSend: function () {
                                    $btnAdd.button('loading');
                                    aguardeMsg();
                                },
                                success: function (response) {

                                    toastr.clear();
                                    bootbox.hideAll();
                                    toastr.success("Lista de Picking gerada com sucesso.");

                                    RefreshDatatables();

                                },
                                error: _DEFAULT_ERROR_TREATMENT
                            }).always(function () {
                                $btnAdd.button('reset');
                            });
                        } else {
                            $("#" + div_id_name + " #operadorespicking").addClass(error_class);
                            toastr.error("Selecione o Operador.", _DEFAULT_ERROR_TIMEOUT);
                        }
                        return false;
                    },
                },
            },
        });

        var num_doc = button.attr("data-numdoc-id");

        if (num_doc) {
            $.ajax({
                url: DefaultApiPath + "/detalhelistapickingparcial?empresa_id=" + empresa_id + "&numdoc=" + num_doc,
                method: "GET",
                success: function (data) {

                    $('#detalhe_lista_picking_em_aberto_parcial').html($('#form_lista_picking_parcial').html());

                    if (data.numPicking) {
                        $('#' + div_id_name + ' #numpicking').val(data.numPicking);
                    }
                    if (data.dataPicking) {
                        $('#' + div_id_name + ' #datapicking').val(data.dataPicking);
                    }
                    if (data.usuario) {
                        $('#' + div_id_name + ' #usuariopicking').val(data.usuario);
                    }
                    if (data.operadores) {
                        var options = "<option value=''>Escolha um Operador</option>";
                        for (var i = 0; i < data.operadores.length; i++) {
                            options += "<option value= '" +
                                data.operadores[i]['id'] +
                                "'>" +
                                data.operadores[i]['userName'] +
                                "</option >";
                        }
                        $('#' + div_id_name + ' #operadorespicking').html(options);
                    }

                    if (data.itens) {
                        lista_picking_itens = data.itens;

                        $("#detalhe_lista_picking_em_aberto_parcial #itens_pedido_parcial tbody").empty();

                        for (var i = 0; i < data.itens.length; i++) {
                            var tr = $("<tr>");
                            for (param in data.itens[i]) {
                                if (param != "index" && param != "qtdPicking" && param != "check") {
                                    $("<td>").html(data.itens[i][param]).appendTo(tr);
                                }
                            }
                            $("<td>").html($("<input>").attr({ "type": "number", "id": "num" + i, "min": "0", "disabled": "disabled", "style": "width:40px" })).appendTo(tr);
                            $("<td>").html($("<input>").attr({ "type": "checkbox", "ref_id": i, "id": "chk" + i }).on("click", function () {
                                //console.log($("#num"+$(this).attr("ref_id")))

                                var num = $("#num" + ($(this).attr("ref_id")));
                                if ($(this).is(":checked"))
                                    num.removeAttr("disabled");
                                else
                                    num.attr("disabled", "disabled").val("");
                            })).appendTo(tr);
                            $("#detalhe_lista_picking_em_aberto_parcial #itens_pedido_parcial tbody").append(tr);
                        }

                    }

                }
            });
        } else {
            toastr.error("Pedido de Venda não encontrado.", _DEFAULT_ERROR_TIMEOUT);
        }

    });

    $("#lista_picking_em_aberto").on("click", ".js-impedir-lista", function () {

        var button = $(this);

        bootbox.confirm("Deseja deixar esta Lista de Picking Pendente?",
            function (result) {
                if (result) {
                    $.ajax({
                        url: DefaultApiPath + "/impedirlistapicking",
                        method: "POST",
                        data: {
                            empresa_id: empresa_id,
                            numdoc: button.attr("data-numdoc-id"),
                        },
                        beforeSend: function () {
                            aguardeMsg();
                        },
                        success: function (data) {
                            toastr.clear();
                            toastr.success("Lista de Picking pendente");

                            RefreshDatatables();
                        },
                        error: _DEFAULT_ERROR_TREATMENT
                    });
                }
            }
        );
    });
    $("#lista_picking_docs_pendentes").on("click", ".js-desimpedir-lista", function () {

        var button = $(this);

        bootbox.confirm("Deseja desimpedir esta Lista de Picking Pendente?",
            function (result) {
                if (result) {
                    $.ajax({
                        url: DefaultApiPath + "/desimpedirlistapicking",
                        method: "POST",
                        data: {
                            empresa_id: empresa_id,
                            numdoc: button.attr("data-numdoc-id"),
                        },
                        beforeSend: function () {
                            aguardeMsg();
                        },
                        success: function (data) {
                            toastr.clear();
                            toastr.success("Lista de Picking aguardando separação");

                            RefreshDatatables();
                        },
                        error: _DEFAULT_ERROR_TREATMENT
                    });
                }
            }
        );
    });

    $("#botao_pesquisa").click(function (e) {
        e.preventDefault();
        RefreshDatatables();
    });

    setInterval(function () {
        RefreshDatatables();
    }, 10000);
});




