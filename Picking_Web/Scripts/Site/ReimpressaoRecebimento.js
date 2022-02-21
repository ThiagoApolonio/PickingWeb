$(document).ready(function () {

    var empresa_id = document.getElementById("script_Reimpressaorecebimentos").getAttribute("data-empresa"),
        dataset_itens = [];

    

    //document.getElementById('codbarras').onkeydown = function (e) {
    //    if (e.keyCode === 13) {
    //        var codbarras = $(this).val();
    //        BuscarCodigoBarras(codbarras);
    //    }
    //};

    $("#itemcode").chosen();

   


    $('#itemcode').on('change', function () {
        var itemcode = $(this).val();
        $('#CodeItem').val('');
        BuscarLotes(itemcode);
    });

    $('#numlote').on('change', function () {
        var numlote = $(this).val();
        var ItemCode = $("#itemcode").val();
        if (numlote) {
            $.ajax({
                url: DefaultApiPath + "/GetLotesInfoReimpressao",
                method: "GET",
                data: {
                    empresa_id: empresa_id,
                    numlote: numlote,
                    itemCode: ItemCode,
                },
                success: function (data) {
                    if (data["itens"]) {

                    if (dataset_itens.length > 0) {
                        bootbox.confirm("Deseja remover os itens que já estão na tela?", function (result) {
                            if (result) {
                                dataset_itens = [];
                            }
                            PreencherDados(data);
                        });
                    } else {
                        PreencherDados(data);
                    }
                        toastr.success("Linha adicionada");
                        ResetarItemELote();
                       

                    } else {
                        toastr.error("Lote não encontrado");
                        ResetarItemELote();
                    }
                },
                error: _DEFAULT_ERROR_TREATMENT
            });
        }
    });

    $('#imprimir').on('click', function () {
        if (dataset_itens.length > 0) {
            var $btn = $(this);
            bootbox.confirm("Deseja imprimir?", function (result) {
                if (result) {
                    Imprimir($btn);
                }
            });
        }
    });

    $('#reset').on('click', function () {
        var $btn = $(this);
        bootbox.confirm("Deseja resetar?", function (result) {
            if (result) {
                Reset($btn);
            }
        });
    });

    $('#cancelar').on('click', function () {
        bootbox.confirm("Deseja cancelar?", function (result) {
            if (result) {
                Cancelar();
            }
        });
    });

    //var BuscarNota = function (numnota) {
    //    $.ajax({
    //        url: DefaultApiPath + "/getnotafiscal",
    //        method: "GET",
    //        data: {
    //            empresa_id: empresa_id,
    //            numnota: numnota,
    //        },
    //        success: function (data) {

    //            if (data["itens"]) {

    //                toastr.success("Nota encontrada");

    //                if (dataset_itens.length > 0) {
    //                    bootbox.confirm("Deseja remover os itens que já estão na tela?", function (result) {
    //                        if (result) {
    //                            dataset_itens = [];
    //                        }

    //                        PreencherDados(data);
    //                    });
    //                } else {
    //                    PreencherDados(data);
    //                }
    //            } else {
    //                toastr.error("Nota não encontrada");
    //            }
    //        },
    //        error: _DEFAULT_ERROR_TREATMENT
    //    });
    //}

    var PreencherDados = function (data) {
        var html = "";
        for (var i = 0; i < data.itens.length; i++) {

            var row = data.itens[i];

            AdicionarLinhaItem(
                row["itemCode"],
                row["itemName"],
                row["numlote"],
                row["codBarras"],
                row["ambiente"],
            );
        }

        DesenhaTabelaItens();
    }

    var BuscarCodigoBarras = function (codbarras) {
        $.ajax({
            url: DefaultApiPath + "/getcodbarrasinfo",
            method: "GET",
            data: {
                empresa_id: empresa_id,
                codbarras: codbarras,
            },
            success: function (data) {

                if (data) {

                    toastr.success("Linha adicionada");

                    AdicionarLinhaItem(
                        data[0]["itemCode"],
                        data[0]["itemName"],
                        1,
                        data[0]["unidadeMedida"],
                        data[0]["numLote"],
                        data[0]["codBarras"],
                        data[0]["dataVenc"],
                        data[0]["ambiente"],
                        data[0]["deposito"],
                        data[0]["loteFabricante"],
                        data[0]["fornecedor"]
                    );

                    DesenhaTabelaItens();

                    ResetarItemELote();

                } else {
                    toastr.error("Lote não encontrado");
                }
            },
            error: _DEFAULT_ERROR_TREATMENT
        });
    }

    var BuscarLotes = function (itemcode) {

        $.ajax({
            url: DefaultApiPath + "/getlotes",
            method: "GET",
            data: {
                empresa_id: empresa_id,
                itemcode: itemcode,
            },
            success: function (data) {

                if (data["lotes"]) {
                    var options = "";
                    for (var i = 0; i < data.lotes.length; i++) {
                        options += "<option value= '" + data.lotes[i] + "'>" + data.lotes[i] + "</option>";
                    }
                    populaLotes(options);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                toastr.clear();
                var msg = jqXHR.responseJSON.message;
                toastr.error(msg, "Erro", _DEFAULT_ERROR_TIMEOUT);
                ResetarItemELote();
            }
        });
    }

    var populaLotes = function (options) {
        $lotes = $('#numlote'),
            default_option = "<option value=''>Selecione o Lote</option>";
        if (options && options.length > 0) {
            $lotes.html(default_option + options);
        } else {
            $lotes.html(default_option);
        }
    }
   

    var AdicionarLinhaItem = function (itemcode, itemname, numlote, cod_barras, ambiente) {

        dataset_itens.push(
            {
                "itemCode": itemcode,
                "itemName": itemname,
                "numlote": numlote,
                "codBarras": cod_barras,
                "ambiente": ambiente,
            }
        );

    }

    var DesenhaTabelaItens = function () {

        var html = "";

        for (var i = 1; i <= dataset_itens.length; i++) {
            var curr_dataset = dataset_itens[i - 1];
            var ambiente_upper = curr_dataset["ambiente"].toUpperCase();
            var nome_ambiente = ambiente_upper === "A" ? "AMBIENTE" : (ambiente_upper === "G" ? "GELADEIRA" : "--");

            // sempre que desenha, adiciona o index dinamicamente
            dataset_itens[i - 1]["_index_"] = i;

            html +=
                "<tr id='tr" + curr_dataset["_index_"] + "'> " +
                "   <td><input type='checkbox' checked='checked' id='check'></td> " +
                "   <td>" + curr_dataset["itemCode"] + "</td> " +
                "   <td>" + curr_dataset["itemName"] + "</td> " +
                "   <td>" + curr_dataset["numlote"] + "</td> " +
                "   <td>" + curr_dataset["codBarras"] + "</td> " +
                "   <td>" + nome_ambiente + "</td> " +
                /* "   <td align='center'> " +
                "       <button data-index='" + i + "' type='button' class='btn btn-danger js-remover-item'> " +
                "           <span class='glyphicon glyphicon-trash'></span>&nbsp; " +
                "       </button> " +
                "   </td> " + */
                "</tr>";
        }

        $('#itens_recebimento tbody').html(html);
    }

    $("#itens_recebimento").on("click", ".js-remover-item", function () {

        var button = $(this);

        bootbox.confirm("Deseja remover este item?",
            function (result) {
                if (result) {
                    RemoverItem(button.attr('data-index'));
                }
            }
        );
    });

    var RemoverItem = function (index) {

        var pos = -1;

        // tentando encontrar qual é a posição real do elemento que eu quero remover com base no índice incremental.
        for (var i = 0; i < dataset_itens.length; i++) {
            if (dataset_itens[i]["_index_"] === index) {
                pos = i;
                break;
            }
        }

        if (pos > -1) {
            dataset_itens.splice(pos, 1);
            $('#itens_recebimento tbody #tr' + index).remove();
        } else {
            toastr.error("Erro interno. Erro ao encontrar índice do array.")
        }
    }

    var ResetarItemELote = function () {
        $('#itemcode')
            .find('option:first-child').prop('selected', true)
            .end().trigger('chosen:updated');
        populaLotes('');
    }

    var Imprimir = function ($button) {

        AtualizarQuantidadesDataset();

        $.ajax({
            url: DefaultApiPath + "/ReImprimir",
            method: "POST",
            data: {
                empresa_id: empresa_id,
                nome_impressora_ambiente: $('#nome_impressora_ambiente').val(),
                nome_impressora_geladeira: $('#nome_impressora_geladeira').val(),
                dataset_itens: dataset_itens,
            },
            beforeSend: function () {
                $button.button('loading');
                aguardeMsg();
            },
            success: function (data) {
                toastr.clear();
                bootbox.hideAll();
                toastr.success("Etiqueta Reimpressa com sucesso.");
                Cancelar();

            },
            error: _DEFAULT_ERROR_TREATMENT
        }).always(function () {
            $button.button('reset');
        });
    }

    var Reset = function ($button) {
        ResetarItens();
    }

    var Cancelar = function () {
        ResetarItens();
        ResetarItemELote();
        $('#numnota').val('');
        $('#codbarras').val('');
    }

    var ResetarItens = function () {
        dataset_itens = [];
        DesenhaTabelaItens();
    }

    var AtualizarQuantidadesDataset = function () {
        for (var i = 0; i < dataset_itens.length; i++) {
            var index = dataset_itens[i]['_index_'];

            //dataset_itens[i]["quantidade"] = $('#itens_recebimento tbody #tr' + index + ' #qtd').val();
            dataset_itens[i]["checked"] = $('#itens_recebimento tbody #tr' + index + ' #check').is(':checked');
        }
    }
});