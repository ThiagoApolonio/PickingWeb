$(document).ready(function () {

    var empresa_id = document.getElementById("script_acompanhamento_picking").getAttribute("data-empresa");
    var timer = document.getElementById("script_acompanhamento_picking").getAttribute("data-timer");
    var GetSearchValue = function () {
        return $('#campo_pesquisa').val();
    };
    var GetURLDocumentoEmSeparacao = function () {
        return DefaultApiPath + "/listadocumentosemseparacao?empresa_id=" +
            empresa_id +
            "&numdoc=" + GetSearchValue();
    }
    var GetURLDocumentoAguardandoConferencia = function () {
        return DefaultApiPath + "/listadocumentosaguardandoconferencia?empresa_id=" +
            empresa_id +
            "&numdoc=" + GetSearchValue();
    }
    var GetURLDocumentoEmConferencia = function () {
        return DefaultApiPath + "/listadocumentosemconferencia?empresa_id=" +
            empresa_id +
            "&numdoc=" + GetSearchValue();
    }
    var GetURLDocumentoEmEmbalagens = function () {
        return DefaultApiPath + "/listadocumentosemembalagens?empresa_id=" +
            empresa_id +
            "&numdoc=" + GetSearchValue();
    }
    var GetURLDocumentoAguardandoEmbalagens = function () {
        return DefaultApiPath + "/listadocumentosaguardandoembalagens?empresa_id=" +
            empresa_id +
            "&numdoc=" + GetSearchValue();
    }
    var GetURLDocumentoEmFaturamento = function () {
        return DefaultApiPath + "/listadocumentosemfaturamento?empresa_id=" +
            empresa_id +
            "&numdoc=" + GetSearchValue();
    }

    var columns_default = [
        {
            data: "numDoc"
        },
        {
            data: "localFisico"
        },
        {
            data: "prioridade",
            render: function(data, type, row) {
                return row._prioridade === 'S' ? "<span class='item-prioritario'>" + data + "</span>" : data;
            }
        },
        {
            data: "nomeCliente"
        },
        {
            data: "statusPicking"
        },
        {
            data: "cidade"
        },
        {
            data: "dataEntrega"
        },
        {
            data: "vendedor"
        },
        {
            data: "operador"
        },
        {
            data: "observacoes"
        },
        {
            data: "horaInicio"
        },
        {
            data: "dataInicio"
        },
        {
            data: "numDoc",
            className: 'td-button',
            render: function(data, type, row) {
                return "<button data-numdoc-id='" + data + "' type='button' class='btn btn-danger js-cancelar-picking'><span class='glyphicon glyphicon-trash'></span >&nbsp;</button>";
            }
        },
        {
            data: "numDoc",
            className: 'td-button',
            render: function(data, type, row) {
                return "<button data-numdoc-id='" +
                    data +
                    "' data-separador='" + row.operador + "' type='button' class='btn btn-primary js-imprimir-picking'><span class='glyphicon glyphicon-print'></span >&nbsp;</button>";
            }
        },
        {
            data: "numDoc",
            className: 'td-button',
            render: function(data, type, row) {
                return "<button data-numdoc-id='" + data + "' data-operador-id='" + row.operadorId + "' type='button' class='btn btn-success js-confirmar-picking'><span class='glyphicon glyphicon-ok'></span >&nbsp;</button>";
            }
        }
    ];
    
    var datatable_documento_em_separacao = $("#acompanhamento_em_separacao").DataTable({
        order: [],
        searching: false,
        ordering: false,
        paging: false,
        info: false,
        ajax: {
            url: GetURLDocumentoEmSeparacao(),
            dataSrc: "",
            method: "GET",
            error: _DEFAULT_ERROR_TREATMENT
        },
        //columns: columns_default,
        columns: [
            {
                data: "numDoc"
            },
            {
                data: "numPk"
            },
            {
                data: "localFisico"
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
                data: "statusPicking"
            },
            {
                data: "cidade"
            },
            {
                data: "dataEntrega"
            },
            {
                data: "vendedor"
            },
            {
                data: "operador"
            },
            {
                data: "observacoes"
            },
            {
                data: "horaInicio"
            },
            {
                data: "dataInicio"
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" + data + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-danger js-cancelar-picking'><span class='glyphicon glyphicon-trash'></span >&nbsp;</button>";
                }
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" +
                        data +
                        "' data-separador='" + row.operador + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-primary js-imprimir-picking'><span class='glyphicon glyphicon-print'></span >&nbsp;</button>";
                }
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" + data + "' data-numpk-id='" + row.numPk + "' data-operador-id='" + row.operadorId + "' type='button' class='btn btn-success js-confirmar-picking'><span class='glyphicon glyphicon-ok'></span >&nbsp;</button>";
                }
            }
        ],
        "oLanguage": _DEFAULT_SCRIPT_LANG
    });
    var datatable_documento_aguardando_conferencia = $("#acompanhamento_aguardando_conferencia").DataTable({
        order: [],
        searching: false,
        ordering: false,
        paging: false,
        info: false,
        ajax: {
            url: GetURLDocumentoAguardandoConferencia(),
            dataSrc: "",
            method: "GET",
            error: _DEFAULT_ERROR_TREATMENT
        },
        //columns: columns_default.slice(0, 16),
        columns: [
            {
                data: "numDoc"
            },
            {
                data: "numPk"
            },
            {
                data: "localFisico"
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
                data: "statusPicking"
            },
            {
                data: "cidade"
            },
            {
                data: "dataEntrega"
            },
            {
                data: "vendedor"
            },
            {
                data: "operador"
            },
            {
                data: "observacoes"
            },
            {
                data: "horaInicio"
            },
            {
                data: "dataInicio"
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" + data + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-danger js-cancelar-picking'><span class='glyphicon glyphicon-trash'></span >&nbsp;</button>";
                }
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" +
                        data +
                        "' data-separador='" + row.operador + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-primary js-imprimir-picking'><span class='glyphicon glyphicon-print'></span >&nbsp;</button>";
                }
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" + data + "' data-numpk-id='" + row.numPk + "' data-operador-id='" + row.operadorId + "' type='button' class='btn btn-success js-confirmar-picking'><span class='glyphicon glyphicon-ok'></span >&nbsp;</button>";
                }
            }
        ],
        "oLanguage": _DEFAULT_SCRIPT_LANG
    });
    var datatable_documento_em_conferencia = $("#acompanhamento_em_conferencia").DataTable({
        order: [],
        searching: false,
        ordering: false,
        paging: false,
        info:false,
        ajax: {
            url: GetURLDocumentoEmConferencia(),
            dataSrc: "",
            method: "GET",
            error: _DEFAULT_ERROR_TREATMENT
        },
        //columns: columns_default.slice(0, 14),
        columns: [
            {
                data: "numDoc"
            },
            {
                data: "numPk"
            },
            {
                data: "localFisico"
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
                data: "statusPicking"
            },
            {
                data: "cidade"
            },
            {
                data: "dataEntrega"
            },
            {
                data: "vendedor"
            },
            {
                data: "operador"
            },
            {
                data: "observacoes"
            },
            {
                data: "horaInicio"
            },
            {
                data: "dataInicio"
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" + data + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-danger js-cancelar-picking'><span class='glyphicon glyphicon-trash'></span >&nbsp;</button>";
                }
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" +
                        data +
                        "' data-separador='" + row.operador + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-primary js-imprimir-picking'><span class='glyphicon glyphicon-print'></span >&nbsp;</button>";
                }
            }
        ],
        "oLanguage": _DEFAULT_SCRIPT_LANG
    });
    var datatable_documento_aguardando_embalagens = $("#acompanhamento_aguardando_embalagens").DataTable({
        order: [],
        searching: false,
        ordering: false,
        paging: false,
        info: false,
        ajax: {
            url: GetURLDocumentoAguardandoEmbalagens(),
            dataSrc: "",
            method: "GET",
            error: _DEFAULT_ERROR_TREATMENT
        },
        columns: [
            {
                data: "numDoc"
            },
            {
                data: "numPk"
            },
            {
                data: "localFisico",
                render: function (data, type, x) {
                    if (data === "--") {
                        return "<button type='button' class='btn btn-link js-trocar-local' data-numdoc='" + x.numDoc + "'>Alterar</button>";
                    }
                    else {
                        return "<button type='button' class='btn btn-link js-trocar-local' data-numdoc='" + x.numDoc + "'>" + data + "</button>";
                    }
                }
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
                data: "statusPicking"
            },
            {
                data: "cidade"
            },
            {
                data: "dataEntrega"
            },
            {
                data: "vendedor"
            },
            {
                data: "operador"
            },
            {
                data: "observacoes"
            },
            {
                data: "horaInicio"
            },
            {
                data: "dataInicio"
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" + data + "' data-numpk-id='" + row.numPk + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-danger js-cancelar-picking'><span class='glyphicon glyphicon-trash'></span >&nbsp;</button>";
                }
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" +
                        data +
                        "' data-separador='" + row.operador + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-primary js-imprimir-picking'><span class='glyphicon glyphicon-print'></span >&nbsp;</button>";
                }
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" + data + "' data-numpk-id='" + row.numPk + "' data-operador-id='" + row.operadorId + "' type='button' class='btn btn-success js-confirmar-picking'><span class='glyphicon glyphicon-ok'></span >&nbsp;</button>";
                }
            }
        ],
        "oLanguage": _DEFAULT_SCRIPT_LANG
    });
    var datatable_documento_em_embalagens = $("#acompanhamento_em_embalagens").DataTable({
        order: [],
        searching: false,
        ordering: false,
        paging: false,
        info: false,
        ajax: {
            url: GetURLDocumentoEmEmbalagens(),
            dataSrc: "",
            method: "GET",
            error: _DEFAULT_ERROR_TREATMENT
        },
        //columns: columns_default.slice(0, 14),
        columns: [
            {
                data: "numDoc"
            },
            {
                data: "numPk"
            },
            {
                data: "localFisico"
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
                data: "statusPicking"
            },
            {
                data: "cidade"
            },
            {
                data: "dataEntrega"
            },
            {
                data: "vendedor"
            },
            {
                data: "operador"
            },
            {
                data: "observacoes"
            },
            {
                data: "horaInicio"
            },
            {
                data: "dataInicio"
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" + data + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-danger js-cancelar-picking'><span class='glyphicon glyphicon-trash'></span >&nbsp;</button>";
                }
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" +
                        data +
                        "' data-separador='" + row.operador + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-primary js-imprimir-picking'><span class='glyphicon glyphicon-print'></span >&nbsp;</button>";
                }
            }
        ],
        "oLanguage": _DEFAULT_SCRIPT_LANG
    });
    var datatable_documento_em_faturamento = $("#acompanhamento_em_faturamento").DataTable({
        order: [],
        searching: false,
        ordering: false,
        paging: false,
        info: false,
        ajax: {
            url: GetURLDocumentoEmFaturamento(),
            dataSrc: "",
            method: "GET",
            error: _DEFAULT_ERROR_TREATMENT
        },
        //columns: columns_default.slice(0, 14),
        columns: [
            {
                data: "numDoc"
            },
            {
                data: "numPk"
            },
            {
                data: "localFisico"
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
                data: "statusPicking"
            },
            {
                data: "cidade"
            },
            {
                data: "dataEntrega"
            },
            {
                data: "vendedor"
            },
            {
                data: "operador"
            },
            {
                data: "observacoes"
            },
            {
                data: "horaInicio"
            },
            {
                data: "dataInicio"
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" + data + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-danger js-cancelar-picking'><span class='glyphicon glyphicon-trash'></span >&nbsp;</button>";
                }
            },
            {
                data: "numDoc",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<button data-numdoc-id='" +
                        data +
                        "' data-separador='" + row.operador + "' data-numpk-id='" + row.numPk + "' type='button' class='btn btn-primary js-imprimir-picking'><span class='glyphicon glyphicon-print'></span >&nbsp;</button>";
                }
            }
        ],
        "oLanguage": _DEFAULT_SCRIPT_LANG
    });

    var RefreshDatatables = function () {
        datatable_documento_em_separacao.ajax.url(GetURLDocumentoEmSeparacao()).load();
        datatable_documento_aguardando_conferencia.ajax.url(GetURLDocumentoAguardandoConferencia()).load();
        datatable_documento_em_conferencia.ajax.url(GetURLDocumentoEmConferencia()).load();
        datatable_documento_aguardando_embalagens.ajax.url(GetURLDocumentoAguardandoEmbalagens()).load();
        datatable_documento_em_embalagens.ajax.url(GetURLDocumentoEmEmbalagens()).load();
        datatable_documento_em_faturamento.ajax.url(GetURLDocumentoEmFaturamento()).load();
    };

    $tables = $("#acompanhamento_em_separacao, #acompanhamento_aguardando_conferencia, #acompanhamento_em_conferencia, #acompanhamento_aguardando_embalagens, #acompanhamento_em_embalagens, #acompanhamento_em_faturamento");

    $tables.on("click", ".js-cancelar-picking", function() {

        var button = $(this);
        
        bootbox.confirm("Deseja cancelar a Lista de Picking?",
            function (result) {
                if (result) {
                    $.ajax({
                        url: DefaultApiPath + "/cancelarlistapicking",
                        method: "POST",
                        data: {
                            empresa_id: empresa_id,
                            numdoc: button.attr("data-numdoc-id"),
                            numpk: button.attr("data-numpk-id"),
                        },
                        beforeSend: function () {
                            aguardeMsg();
                        },
                        success: function (data) {
                            toastr.clear();
                            toastr.success("Lista de Picking cancelada com sucesso");
                            RefreshDatatables();
                        },
                        error: _DEFAULT_ERROR_TREATMENT
                    });
                }
            }
        );
    });

    $tables.on("click", ".js-imprimir-picking", function () {

        var button = $(this);
        
        bootbox.confirm("Deseja reimprimir a Lista de Picking?",
            function (result) {
                if (result) {
                    var aux = button.attr("data-separador");
                    $.ajax({
                        url: DefaultApiPath + "/reimprimirlistapicking?empresa_id=" + empresa_id + "&numdoc=" + button.attr("data-numdoc-id") + "&numpk=" + button.attr("data-numpk-id") + "&nome_impressora=" + $('#nome_impressora').val(),
                        method: "GET",
                        data: {
                            empresa_id: empresa_id,
                            numdoc: button.attr("data-numdoc-id"),
                            numpk: button.attr("data-numpk-id"),
                            nome_impressora: $('#nome_impressora').val(),
                            separador: button.attr("data-separador"),
                        },
                        beforeSend: function () {
                            aguardeMsg();
                        },
                        success: function (data) {
                            toastr.clear();
                            toastr.success("Lista de Picking reimpressa com sucesso");
                        },
                        error: _DEFAULT_ERROR_TREATMENT
                    });
                }
            }
        );
    });

    $("#acompanhamento_em_separacao").on("click", ".js-confirmar-picking", function () {
        
        var button = $(this),
            div_id_name = "detalhe_acompanhamento_em_separacao",
            btn_add_lista_classname = "btn-add-lista",
            error_class = "input-validation-error";


        var dialog = bootbox.dialog({
            title: 'Encerramento de Separação',
            size: 'large',
            message: '<div id="' + div_id_name + '"><p><i class="fa fa-spin fa-spinner"></i> Carregando...</p></div>',
            buttons: {
                cancel: {
                    label: 'Cancelar',
                    className: 'btn-default',
                },
                adicionar: {
                    label: 'Atualizar',
                    className: 'btn-success ' + btn_add_lista_classname,
                    callback: function () {
                        var $btnAdd = $('.' + btn_add_lista_classname);
                        $btnAdd.attr('data-loading-text', 'Aguarde...');

                        $("#" + div_id_name + " #operadorespicking").removeClass(error_class);
                        var operador = $("#" + div_id_name + " #operadorespicking").val();
                        if (operador) {
                            $.ajax({
                                url: DefaultApiPath + "/encerrarseparacao",
                                method: "POST",
                                data: {
                                    empresa_id: empresa_id,
                                    numpk: $("#" + div_id_name + " #numpk").val(),
                                    numdoc: $("#" + div_id_name + " #numdoc").val(),
                                    operador: operador, 
                                    local: $("#" + div_id_name + " #local").val(),
                                },
                                beforeSend: function () {
                                    $btnAdd.button('loading');
                                    aguardeMsg();
                                },
                                success: function (data) {

                                    toastr.clear();
                                    bootbox.hideAll();
                                    toastr.success("Confirmado com sucesso.");

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
        var num_pk = button.attr("data-numpk-id");
        var operador_default_id = button.attr("data-operador-id");

        if (num_doc) {
            $.ajax({
                url: DefaultApiPath + "/detalheencerramentoseparacao?empresa_id=" + empresa_id,
                method: "GET",
                success: function (data) {

                    $('#' + div_id_name).html($('#form_encerramento_separacao_detalhe').html());
                    
                    if (data.operadores) {
                        var options = "<option value=''>Escolha um Operador</option>";
                        for (var i = 0; i < data.operadores.length; i++) {
                            var operador_id = data.operadores[i]['id'];
                            var operador_name = data.operadores[i]['userName'];
                            var selected = operador_id === operador_default_id ? "selected='selected'" : "";

                            options += "<option value= '" + operador_id + "' " + selected + ">" + operador_name + "</option >";
                        }
                        $('#' + div_id_name + ' #operadorespicking').html(options);
                    }

                    $('#' + div_id_name + ' #numdoc').val(num_doc);
                    $('#' + div_id_name + ' #numpk').val(num_pk);
                }
            });
        } else {
            toastr.error("Pedido de Venda não encontrado.", _DEFAULT_ERROR_TIMEOUT);
        }
    });

    $("#acompanhamento_aguardando_conferencia").on("click", ".js-confirmar-picking", function () {

        var button = $(this),
            div_id_name = "detalhe_inicio_conferencia",
            btn_add_lista_classname = "btn-add-lista",
            error_class = "input-validation-error";


        var dialog = bootbox.dialog({
            title: 'Início de Conferência',
            size: 'large',
            message: '<div id="' + div_id_name + '"><p><i class="fa fa-spin fa-spinner"></i> Carregando...</p></div>',
            buttons: {
                cancel: {
                    label: 'Cancelar',
                    className: 'btn-default',
                },
                adicionar: {
                    label: 'OK - Atualizar',
                    className: 'btn-success ' + btn_add_lista_classname,
                    callback: function () {
                        var $btnAdd = $('.' + btn_add_lista_classname);
                        $btnAdd.attr('data-loading-text', 'Aguarde...');

                        $("#" + div_id_name + " #inicio_operadorespicking").removeClass(error_class);
                        var operador = $("#" + div_id_name + " #inicio_operadorespicking").val();
                        if (operador) {
                            $.ajax({
                                url: DefaultApiPath + "/iniciarconferencia",
                                method: "POST",
                                data: {
                                    empresa_id: empresa_id,
                                    numdoc: $("#" + div_id_name + " #inicio_numdoc").val(),
                                    numpk: $("#" + div_id_name + " #inicio_numpk").val(),
                                    operador: operador,
                                    local: $("#" + div_id_name + " #inicio_local").val(),
                                },
                                beforeSend: function () {
                                    $btnAdd.button('loading');
                                    aguardeMsg();
                                },
                                success: function (data) {

                                    toastr.clear();
                                    bootbox.hideAll();
                                    toastr.success("Iniciado com sucesso.");

                                    RefreshDatatables();

                                },
                                error: _DEFAULT_ERROR_TREATMENT
                            }).always(function () {
                                $btnAdd.button('reset');
                            });
                        } else {
                            $("#" + div_id_name + " #inicio_operadorespicking").addClass(error_class);
                            toastr.error("Selecione o Operador.", _DEFAULT_ERROR_TIMEOUT);
                        }
                        return false;
                    },
                },
            },
        });

        var num_doc = button.attr("data-numdoc-id");
        var num_pk = button.attr("data-numpk-id");
        if (num_doc) {
            $.ajax({
                url: DefaultApiPath + "/detalheinicioconferencia?empresa_id=" + empresa_id,
                method: "GET",
                success: function (data) {

                    $('#' + div_id_name).html($('#form_inicio_embalagens').html());

                    if (data.operadores) {
                        var options = "<option value=''>Escolha um Operador</option>";
                        for (var i = 0; i < data.operadores.length; i++) {
                            options += "<option value= '" +
                                data.operadores[i]['id'] +
                                "'>" +
                                data.operadores[i]['userName'] +
                                "</option >";
                        }
                        $('#' + div_id_name + ' #inicio_operadorespicking').html(options);
                    }

                    $('#' + div_id_name + ' #inicio_numdoc').val(num_doc);
                    $('#' + div_id_name + ' #inicio_numpk').val(num_pk);
                }
            });
        } else {
            toastr.error("Pedido de Venda não encontrado.", _DEFAULT_ERROR_TIMEOUT);
        }
    });

    $("#acompanhamento_aguardando_embalagens").on("click", ".js-confirmar-picking", function () {

        var button = $(this),
            div_id_name = "detalhe_inicio_embalagens",
            btn_add_lista_classname = "btn-add-lista",
            error_class = "input-validation-error";


        var dialog = bootbox.dialog({
            title: 'Início de Embalagens',
            size: 'large',
            message: '<div id="' + div_id_name + '"><p><i class="fa fa-spin fa-spinner"></i> Carregando...</p></div>',
            buttons: {
                cancel: {
                    label: 'Cancelar',
                    className: 'btn-default',
                },
                adicionar: {
                    label: 'OK - Atualizar',
                    className: 'btn-success ' + btn_add_lista_classname,
                    callback: function () {
                        var $btnAdd = $('.' + btn_add_lista_classname);
                        $btnAdd.attr('data-loading-text', 'Aguarde...');

                        $("#" + div_id_name + " #inicio_operadorespicking").removeClass(error_class);
                        var operador = $("#" + div_id_name + " #inicio_operadorespicking").val();
                        if (operador) {
                            $.ajax({
                                url: DefaultApiPath + "/iniciarembalagens",
                                method: "POST",
                                data: {
                                    empresa_id: empresa_id,
                                    numdoc: $("#" + div_id_name + " #inicio_numdoc").val(),
                                    numpk: $("#" + div_id_name + " #inicio_numpk").val(),
                                    operador: operador,
                                    local: $("#" + div_id_name + " #inicio_local").val(),
                                },
                                beforeSend: function () {
                                    $btnAdd.button('loading');
                                    aguardeMsg();
                                },
                                success: function (data) {

                                    toastr.clear();
                                    bootbox.hideAll();
                                    toastr.success("Iniciado com sucesso.");

                                    RefreshDatatables();

                                },
                                error: _DEFAULT_ERROR_TREATMENT
                            }).always(function () {
                                $btnAdd.button('reset');
                            });
                        } else {
                            $("#" + div_id_name + " #inicio_operadorespicking").addClass(error_class);
                            toastr.error("Selecione o Operador.", _DEFAULT_ERROR_TIMEOUT);
                        }
                        return false;
                    },
                },
            },
        });

        var num_doc = button.attr("data-numdoc-id");
        var num_pk = button.attr("data-numpk-id");
        if (num_doc) {
            $.ajax({
                url: DefaultApiPath + "/detalheinicioembalagens?empresa_id=" + empresa_id,
                method: "GET",
                success: function (data) {

                    $('#' + div_id_name).html($('#form_inicio_embalagens').html());

                    if (data.operadores) {
                        var options = "<option value=''>Escolha um Operador</option>";
                        for (var i = 0; i < data.operadores.length; i++) {
                            options += "<option value= '" +
                                data.operadores[i]['id'] +
                                "'>" +
                                data.operadores[i]['userName'] +
                                "</option >";
                        }
                        $('#' + div_id_name + ' #inicio_operadorespicking').html(options);
                    }

                    $('#' + div_id_name + ' #inicio_numdoc').val(num_doc);
                    $('#' + div_id_name + ' #inicio_numpk').val(num_pk);
                }
            });
        } else {
            toastr.error("Pedido de Venda não encontrado.", _DEFAULT_ERROR_TIMEOUT);
        }
    });

    $("#acompanhamento_aguardando_embalagens").on("click", ".js-trocar-local", function () {
        var pedido = $(this).attr("data-numdoc");

        bootbox.dialog({
            title: 'Alterar Local Físico do Pedido ' + pedido,
            size: 'large',
            message: '<form class="bootbox-form" >' +
            '<div class="form-group">' +
            '<label class="control-label col-md-2">Local Físico</label>' +
            '<input class="bootbox-input bootbox-input-text form-control" type="text" id="trocar_local_fisico">' +
            '</div>' +
            '</form>',
            buttons: {
                cancel: {
                    label: 'Cancelar',
                    className: 'btn-default',
                },
                adicionar: {
                    label: 'OK - Atualizar',
                    className: 'btn-success local_fisico',
                    callback: function () {

                        $.ajax({
                            url: DefaultApiPath + "/trocarlocalfisico",
                            method: "POST",
                            data: {
                                empresa_id: empresa_id,
                                numdoc: pedido,
                                local: $("#trocar_local_fisico").val(),
                            },
                            success: function (data) {

                                toastr.clear();
                                bootbox.hideAll();
                                toastr.success("Local físico alterado com sucesso.");

                                RefreshDatatables();

                            },
                            error: _DEFAULT_ERROR_TREATMENT
                        });

                        return false;
                    },
                },
            },
        });
    });

    $("#botao_pesquisa").click(function (e) {
        e.preventDefault();
        RefreshDatatables();
    });

    if (timer > 0) {
        setInterval(function () {
            RefreshDatatables();
        }, timer);
    }
});