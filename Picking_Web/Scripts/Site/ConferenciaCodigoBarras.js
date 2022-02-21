$(document).ready(function () {

    var empresa_id = document.getElementById("script_conferencia_codigo_barras").getAttribute("data-empresa"),
        url_numdoc = document.getElementById("script_conferencia_codigo_barras").getAttribute("data-numdoc"),
        url_operador = document.getElementById("script_conferencia_codigo_barras").getAttribute("data-operador"), 
        currNumDoc = null;
    


    document.getElementById('numdoc').onkeydown = function (e) {
        if (e.keyCode === 13) {

            $.ajax({
                url: DefaultApiPath + "/LimpaListaLoteConferencia",
                method: "POST",
                error: function (jqXHR, textStatus, errorThrown) {
                    toastr.clear();
                    var msg = jqXHR.responseJSON.message;
                    toastr.error(msg, "Erro", _DEFAULT_ERROR_TIMEOUT);
                    audio.play();
                }
            });

            BuscarPedido();
        }
    };
    document.getElementById('codigo_barras').onkeydown = function (e) {
        if (e.keyCode === 13) {
            LerCodigoBarras();
        }
    };

    $('#operador').on('change', function() {
        SelecionarOperador();
        $('#numdoc').focus();
    });

    $('#encerrar').on('click', function() {
    var $btn = $(this);
        if (datatable_itens_pedido.data().count() > 0) {
            if (ConferenciaValida()) {
                EncerrarConferencia($btn);
            }
        } else {
            audio.play();
            toastr.error("Nenhum item a ser conferido.");
        }
    });


    $('#conferencia_parcial').on('click', function() {
        var $btn = $(this);
        bootbox.confirm("Deseja realizar uma conferência parcial?", function (result) {
            var operador = $('#operador').val();
            if (operador) {
                if (result && datatable_itens_pedido.data().count() > 0) {
                    ConferenciaParcial($btn, operador);
                } else {
                    audio.play();
                    toastr.error("Selecione um pedido.");
                }
            } else {
                audio.play();
                toastr.error("Selecione um operador.");
            }
        });
    });

    $('#reset').on('click', function() {
        var $btn = $(this);
        bootbox.confirm("Deseja resetar a conferência?", function (result) {
            if (result && datatable_itens_pedido.data().count() > 0) {
                Reset($btn);
            }
        });
    });

    $('#cancelar').on('click', function() {
        var $btn = $(this);
        bootbox.confirm("Deseja cancelar a conferência?", function (result) {
            if (result) {
                Cancelar($btn);
            }
        });
    });

    var SelecionarOperador = function () {
        if ($('#operador').val()) {
            PodeConferir();
        } else {
            NaoPodeConferir();
        }
    }

    var TogglePodeConferir = function (pode) {
        if (pode) {
            var elements_cabecalho = $('#numdoc, #quantidade, #codigo_barras');
            elements_cabecalho.removeAttr('disabled');
        } else {
            ResetTela();
        }
    }

    var NaoPodeConferir = function () {
        TogglePodeConferir(false);
    }

    var PodeConferir = function () {
        TogglePodeConferir(true);
    }

    var ResetTela = function () {
        var elements_cabecalho = $('#numdoc, #quantidade, #codigo_barras');
        elements_cabecalho.attr('disabled', 'disabled');
        elements_cabecalho.val('');
        ResetQuantidadeDefault();
        currNumDoc = -1;
        datatable_itens_pedido.clear().draw();

        $operador = $('#operador');
        $operador.val('');
        $('#operador').focus();
    }

    var ResetQuantidadeDefault = function () {
        $('#quantidade').val('1');
    }

    var GetURLBuscarItensPedido = function (numdoc) {
        return DefaultApiPath + "/getitenspedido?empresa_id=" + empresa_id + "&numdoc=" + numdoc;
    }

    var datatable_itens_pedido = $("#itens_pedido").DataTable({
        order: [],
        searching: false,
        ordering: false,
        paging: false,
        info: false,
        autoWidth: false,
        ajax: {
            url: GetURLBuscarItensPedido("-1"),
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
            {
                data: "deposito",
                className: 'td-center',
            },
            {
                data: "numSerie",
                className: 'td-center',
            },
            {
                data: "codigoBarras",
                className: 'td-center',
            },
            {
                data: "numLote",
                className: 'td-center',
            },
            {
                data: "lineNum",
                visible: false
            },
            {
                data: "quantidade",
                className: 'td-button',
                render: function (data, type, row) {
                    return "<input type='number' class='form-control input-conferido' id='qtd" + row.index + "' value='" + data + "' style='width:70px;text-align: center;' disabled='disabled' >";
                }
            },
            {
                data: "quantidadeTotal",
                className: 'td-center',
            },
        ],
        "oLanguage": _DEFAULT_SCRIPT_LANG
    });

    var BuscarPedido = function () {
        currNumDoc = $('#numdoc').val();
        datatable_itens_pedido.ajax.url(GetURLBuscarItensPedido(currNumDoc)).load();
        $('#codigo_barras').focus();
    }
    //var LerCodigoBarras = function () {
    //    var $codigo_barras = $('#codigo_barras');
    //    var codigo_barras = $codigo_barras.val();
    //    var encontrou = false;
    //    for (var i = 0; i < datatable_itens_pedido.data().count(); i++) {
    //        var row = datatable_itens_pedido.data()[i];
    //        var codigo_barras_upercase = codigo_barras.toUpperCase();
    //        if (row["numSerie"].toUpperCase() === codigo_barras_upercase
    //            || row["numLote"].toUpperCase() === codigo_barras_upercase
    //            || row["codigoFabricante"].toUpperCase() === codigo_barras_upercase
    //            || row["codigoBarras"].toUpperCase() === codigo_barras_upercase) {

    //            encontrou = true;
    //            var qtdDefault = 1;
    //            $('#quantidade').val(function (r, oldQtdDefaultVal) {
    //                qtdDefault = oldQtdDefaultVal;
    //                return oldQtdDefaultVal;
    //            });

    //            var $qtd = $('#qtd' + i);
    //            $qtd.val(function (j, oldval) {
    //                var result = parseFloat(oldval) + parseFloat(qtdDefault);
    //                if (result <= row["quantidadeTotal"]) {
    //                    return result;
    //                } else {
    //                    GerarNaoConformidade(
    //                        "<b>QUANTIDADE INFORMADA É SUPERIOR A QUANTIDADE DEMANDADA</b><br/><br/>" +
    //                        "<b>ITEM:</b> " + row["item"] + " - " + row["descricao"] + "<br/>" +
    //                        "<b>LOTE:</b> " + codigo_barras, row["item"], row["numLote"]
    //                    );
    //                    return oldval;
    //                }
    //            });

    //            $codigo_barras.val('');
    //            return false;
    //        }
    //    }

    //    if (!encontrou) {
    //        GerarNaoConformidade("LOTE <b>" + codigo_barras + "</b> NÃO ENCONTRADO PARA O PEDIDO DE VENDA " + currNumDoc, "", "");
    //    }
    //}

    var LerCodigoBarras = function () {
        var rowcount = datatable_itens_pedido.data().count();
        var $codigo_barras = $('#codigo_barras'); 
        var codigo_barrasOri = $codigo_barras.val();
        var separacao = codigo_barrasOri.split("-");
        var lenght = separacao.length;
        if (lenght > 1) {
            lenght = separacao.length - 1;
        }
        var codigo_barrasVet = separacao.slice(0, lenght);
        var codigo_barras = codigo_barrasVet.join("-");
        var aux = codigo_barras;
        $.ajax({
            url: DefaultApiPath + "/VerificaLote?codigo_barras=" + codigo_barrasOri + "&empresa_id=" + empresa_id + "&Codigo_barrasOri=" + codigo_barras,
            method: "POST",
            data: {
                countRow: rowcount,
                data: GetTableData(),
            },
            success:
                function (response) {
                    var linha = response;
                    var row = datatable_itens_pedido.data()[linha];
                        var qtdDefault = 1;
                        $('#quantidade').val(function (r, oldQtdDefaultVal) {
                            qtdDefault = oldQtdDefaultVal;
                            return oldQtdDefaultVal;
                        });

                    var $qtd = $('#qtd' + linha);
                        $qtd.val(function (j, oldval) {
                            var aux = oldval;
                            var result = parseFloat(oldval) + parseFloat(qtdDefault);
                            if (result <= row["quantidadeTotal"]) {
                                return result;
                            } else {
                                GerarNaoConformidade(
                                    "<b>QUANTIDADE INFORMADA É SUPERIOR A QUANTIDADE DEMANDADA</b><br/><br/>" +
                                    "<b>ITEM:</b> " + row["item"] + " - " + row["descricao"] + "<br/>" +
                                    "<b>LOTE:</b> " + codigo_barras, row["item"], row["numLote"]
                                );
                                return oldval;
                            }
                        });

                        $codigo_barras.val('');
                        return false;
                },
            error: function (jqXHR, textStatus, errorThrown) {
                toastr.clear();
                var msg = jqXHR.responseJSON.message;
                toastr.error(msg, "Erro", _DEFAULT_ERROR_TIMEOUT);
                audio.play();
            }

            
        } ).always(function () {
            ResetQuantidadeDefault();
           
        });
        
    }
   
    var GetTableData = function () {
        var rows = [];
        var aux = datatable_itens_pedido.data().count();
        if (currNumDoc) {
            for (var i = 0; i < datatable_itens_pedido.data().count(); i++) {
                var row = datatable_itens_pedido.data()[i];
                row["qtd"] = $('#qtd' + row["index"]).val();
                rows.push(row);
            }
        }
        return rows;
    }

    var EncerrarConferencia = function ($button) {

        var div_id_name = "detalhe_encerramento_conferencia",
            btn_add_lista_classname = "btn-add-conferencia",
            error_class = "input-validation-error";
        var rowcount = datatable_itens_pedido.data().count();
        var dialog = bootbox.dialog({
            title: 'Encerramento de Conferência',
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

                        $("#" + div_id_name + " #operadorespicking").removeClass(error_class);
                        var operador = $("#" + div_id_name + " #operadorespicking").val();
                        if (operador) {
                            $.ajax({
                                url: DefaultApiPath + "/encerrarconferencia",
                                method: "POST",
                                data: {
                                    empresa_id: empresa_id,
                                    numdoc: currNumDoc,
                                    operador: operador,
                                    countRow: rowcount,
                                    local: $("#" + div_id_name + " #local").val(),
                                    data: GetTableData(),
                                },
                                beforeSend: function () {
                                    $btnAdd.button('loading');
                                    aguardeMsg();
                                },
                                success: function (data) {

                                    toastr.clear();
                                    bootbox.hideAll();
                                    toastr.success("Conferência realizada com sucesso.");

                                    ResetTela();

                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    toastr.clear();
                                    var msg = jqXHR.responseJSON.message;
                                    toastr.error(msg, "Erro", _DEFAULT_ERROR_TIMEOUT);
                                    audio.play();
                                }
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

        // carrega/popula os campos na tela
        $.ajax({
            url: DefaultApiPath + "/detalheencerramentoconferencia?empresa_id=" + empresa_id,
            method: "GET",
            success: function (data) {

                var operador_default = $('#operador').val();
                $('#' + div_id_name).html($('#form_encerramento_conferencia').html());

                if (data.operadores) {
                    var options = "<option value=''>Escolha um Operador</option>";
                    for (var i = 0; i < data.operadores.length; i++) {
                        var selected = data.operadores[i]['usuarioSAPId'] === operador_default ? "selected='selected'" : "";
                        options +=
                            "<option value= '" + data.operadores[i]['id'] + "' " + selected + " >" +
                                data.operadores[i]['userName'] +
                            "</option >";
                    }
                    $('#' + div_id_name + ' #operadorespicking').html(options);
                }


                $("#" + div_id_name + " #operadorespicking").val($('#operador').val());
                $('#' + div_id_name + ' #numdoc').val(currNumDoc);
            }
        });
    }

    var ConferenciaValida = function () {
        var res = true;
        for (var i = 0; i < datatable_itens_pedido.data().count(); i++) {
            var row = datatable_itens_pedido.data()[i],
                $qtd = $('#qtd' + row["index"]), 
                qtd = $qtd.val(),
                total = row["quantidadeTotal"];
                
            if(qtd > total)
            {
                msg = "<b>QUANTIDADE INFORMADA É SUPERIOR A QUANTIDADE DEMANDADA</b><br/><br/>" +
                    "<b>ITEM:</b> " + row["item"] + " - " + row["descricao"] + "<br/>" +
                    "<b>LOTE:</b> " + row["numLote"] + row["numSerie"];
            }
            else if (qtd < total)
            {
                msg = "<b>QUANTIDADE INFORMADA É INFERIOR A QUANTIDADE DEMANDADA</b><br/><br/>" +
                    "<b>ITEM:</b> " + row["item"] + " - " + row["descricao"] + "<br/>" +
                    "<b>LOTE:</b> " + row["numLote"] + row["numSerie"];
            }
            else
            {
                continue;
            }

            res = false;
            GerarNaoConformidade(msg, row["item"], row["numLote"]);
            break;
        }
        return res;
    }

    var ConferenciaParcial = function ($button, operador) {
        $.ajax({
            url: DefaultApiPath + "/conferenciaparcial",
            method: "POST",
            data: {
                empresa_id: empresa_id,
                numdoc: currNumDoc,
                data: GetTableData(),
                operador: operador
            },
            beforeSend: function () {
                $button.button('loading');
                aguardeMsg();
            },
            success: function (data) {
                toastr.clear();
                toastr.success("Conferência Parcial realizada.");
                ResetTela();
            },
            error: _DEFAULT_ERROR_TREATMENT
        }).always(function () {
            $button.button('reset');
        });
    }

    var Reset = function ($button) {
        $.ajax({
            url: DefaultApiPath + "/resetconferencia",
            method: "POST",
            data: {
                empresa_id: empresa_id,
                numdoc: currNumDoc
            },
            beforeSend: function () {
                $button.button('loading');
                aguardeMsg();
            },
            success: function (data) {
                toastr.clear();
                toastr.success("Conferência resetada.");
                ResetTela();
            },
            error: _DEFAULT_ERROR_TREATMENT
        }).always(function () {
            $button.button('reset');
        });
    }

    var Cancelar = function ($button) {
        ResetTela();
    }

    var GerarNaoConformidade = function (msg, item, lote) {
        audio.play();

        bootbox.alert({
            title: "<b>ATENÇÃO!!!</b>",
            message: msg,
            show: true,
            callback: function () {
                setTimeout(function () {
                    $codigo_barras = $('#codigo_barras');
                    $codigo_barras.val('');
                    $codigo_barras.focus();
                },10);
            }
        });

        setTimeout(function () {
            $operador = $('#operador');
            $operador.focus();
            $operador.blur();
        }, 500);

        $.ajax({
            url: DefaultApiPath + "/salvarlog",
            method: "POST",
            data: {
                empresa_id: empresa_id,
                numdoc: currNumDoc,
                item: item,
                lote: lote,
                msg: msg,
                operador: $('#operador').val()
            },
            error: _DEFAULT_ERROR_TREATMENT
        });
    }

    SelecionarOperador();

    $('#data').val(currentDay());

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });

    if (url_operador) {
        var element = document.getElementById('operador');
        element.value = url_operador
        var event = new Event('change');
        element.dispatchEvent(event);
    }

    if (url_numdoc > 0) {
        $('#numdoc').val(url_numdoc);
        //BuscarPedido();
    }
});