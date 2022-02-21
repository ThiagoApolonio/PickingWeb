$(document).ready(function () {

    var empresa_id = document.getElementById("script_baixa").getAttribute("data-empresa");
    dataset_itens = [];


    document.getElementById('CodPn').onkeydown = function (e) {
        if (e.keyCode === 13) {
            ValidaPN();

        }
    };

    document.getElementById('itemcodeLote').onkeydown = function (e) {
        if (e.keyCode === 13) {
            ItemCodeLote = GetLoteCode();
            cardCode = $('#CardCode').val();
            $.ajax({
                url: DefaultApiPath + "/GetItensPedidoBaixa?empresa_id=" + empresa_id + "&ItemCodeLote=" + ItemCodeLote,
                method: "GET",
                data: {
                    empresa_id: empresa_id,
                },
                success: function (data) {



                    if (dataset_itens.length > 0) {

                        for (var i = 0; i < dataset_itens.length; i++) {
                            var ArrayNovo = dataset_itens[i];
                            var ArrayGrid = data.itens[0];
                            var same = ArrayNovo['numLote'] === ArrayGrid['numLote']
                            if (same) {
                                toastr.error("Atenção ! Lote Já Selecionado.");
                                return error;
                            }

                        }

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
                },
                error: _DEFAULT_ERROR_TREATMENT
            });
        }
    };


    var ResetarItemELote = function () {
        var elements_cabecalho = $('#itemcodeLote');
        elements_cabecalho.attr('disabled', 'disabled');
        var elements_cabecalhoLimpar = $('#CodPn,#itemcodeLote');
        elements_cabecalhoLimpar.val('');
    }
    var PreencherDados = function (data) {
        var html = "";
        for (var i = 0; i < data.itens.length; i++) {

            var row = data.itens[i];

            AdicionarLinhaItem(
                row["item"],
                row["descricao"],
                row["deposito"],
                row["numLote"],
            );
        }

        DesenhaTabelaItens();
    }
    var DesenhaTabelaItens = function () {

        var html = "";

        for (var i = 1; i <= dataset_itens.length; i++) {
            var curr_dataset = dataset_itens[i - 1];
            // sempre que desenha, adiciona o index dinamicamente
            dataset_itens[i - 1]["_index_"] = i;

            html +=
                "<tr id='tr" + curr_dataset["_index_"] + "'> " +
                "   <td><input type='checkbox' checked='checked' id='check'></td> " +
                "   <td>" + curr_dataset["item"] + "</td> " +
                "   <td>" + curr_dataset["descricao"] + "</td> " +
                "   <td>" + curr_dataset["deposito"] + "</td> " +
                "   <td>" + curr_dataset["numLote"] + "</td> " +
                /* "   <td align='center'> " +
                "       <button data-index='" + i + "' type='button' class='btn btn-danger js-remover-item'> " +
                "           <span class='glyphicon glyphicon-trash'></span>&nbsp; " +
                "       </button> " +
                "   </td> " + */
                "</tr>";
        }


        $('#itens_Baixa tbody').html(html);
    }
    var AdicionarLinhaItem = function (itemcode, itemname, deposito, numlote) {

        dataset_itens.push(
            {
                "item": itemcode,
                "descricao": itemname,
                "deposito": deposito,
                "numLote": numlote,
            }
        );

    }

    $('#GoBaixa').on('click', function () {
        if (dataset_itens.length > 0) {
            var $btn = $(this);
            bootbox.confirm("Deseja Efetuar a Baixa?", function (result) {
                if (result) {
                    EfetuaBaixaParaDepositoPN($btn);
                }
            });
        } else {
            audio.play();
            toastr.error("Nenhum item a ser Baixado");
        }
    });

    var ValidaPN = function () {
        var CardCode_Pn = $('#CodPn').val();

        $.ajax({
            url: DefaultApiPath + "/ValidaParceiroDeNegocio?empresa_id=" + empresa_id + "&Card_Code=" + CardCode_Pn,
            method: "GET",
            data: {
                empresa_id: empresa_id,
            },
            success:
                function (response) {
                    PodeBaixar();
                    $('#itemcodeLote').focus();
                },
            error: function (jqXHR, textStatus, errorThrown) {
                toastr.clear();
                var msg = jqXHR.responseJSON.message;
                toastr.error(msg, "Erro", _DEFAULT_ERROR_TIMEOUT);
                audio.play();
                NaoPodeBaixar();
            }
        });

    }
    var PodeBaixar = function () {
        var elements_cabecalho = $('#itemcodeLote');
        elements_cabecalho.removeAttr('disabled');
    }
    var NaoPodeBaixar = function () {
        var elements_cabecalho = $('#itemcodeLote');
        elements_cabecalho.attr('disabled', 'disabled');
        elements_cabecalho.val('');
        datatable_itens_pedido.clear().draw();

        $operador = $('#CodPn');
        $operador.val('');
        $('#CodPn').focus();
    }


    var GetLoteCode = function () {
        var $codigo_barras = $('#itemcodeLote');
        var codigo_barrasOri = $codigo_barras.val();
        return codigo_barrasOri;
    }

    var ResetTela = function () {
        var elements_cabecalho = $('#itemcodeLote');
        elements_cabecalho.attr('disabled', 'disabled');
        var elements_cabecalhoLimpar = $('#CodPn,#itemcodeLote');
        elements_cabecalhoLimpar.val('');
        ResetarItens();

    }

    var ResetarItens = function () {
        dataset_itens = [];
        DesenhaTabelaItens();
    }

    var aguardeMsg = function () {
        toastr.info("aguarde...", { timeOut: 50000 });
    }

    var EfetuaBaixaParaDepositoPN = function ($button) {
        AtualizarQuantidadesDataset();
        $.ajax({
            url: DefaultApiPath + "/EfetuarBaixaPN",
            method: "POST",
            data: {
                empresa_id: empresa_id,
                CardCode_Pn: $('#CodPn').val(),
                dataset_itens: dataset_itens,
            },
            beforeSend: function () {
                $button.button('loading');
                aguardeMsg();
            },
            success:
                function (response) {
                    toastr.clear();
                    bootbox.hideAll();
                    toastr.success("Baixa realizada com sucesso.");

                    ResetTela();
                },
            error: function (jqXHR, textStatus, errorThrown) {
                toastr.clear();
                var msg = jqXHR.responseJSON.message;
                toastr.error(msg, "Erro", _DEFAULT_ERROR_TIMEOUT);
                audio.play();
            }
        }).always(function () {
            $button.button('reset');
        });

    }
    var AtualizarQuantidadesDataset = function () {
        for (var i = 0; i < dataset_itens.length; i++) {
            var index = dataset_itens[i]['_index_'];

            //dataset_itens[i]["quantidade"] = $('#itens_recebimento tbody #tr' + index + ' #qtd').val();
            dataset_itens[i]["checked"] = $('#itens_Baixa tbody #tr' + index + ' #check').is(':checked');
        }
    }
});