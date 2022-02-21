$(document).ready(function () {
    table = $("#itens_recebimento").DataTable({


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
        columns: [
            {
                data:"itemCode"

            },
            
            {
                data:"itemName",

            }
            ,
            {

                data :"unidadeMedida",

            }
            ,
            {
                data:"numLote",
            }
            ,
            {
                data:"codBarras",
            }
            ,
            {
                data:"dataVenc",
            }
            ,
            {
                data:"ambiente",
            }
            ,
            {
                data:"deposito",
            }
            ,
            {
                data:"loteFabricante",
            }
            ,
            {
                data:"fornecedor"
            }

        ],
        pageLength: 1,
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
    var empresa_id = document.getElementById("script_recebimentos").getAttribute("data-empresa"),
        dataset_itens = [];

    $('#itemcode').on('change', function () {
        var itemcode = $(this).val();
        BuscarLotes(itemcode);

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

    var BuscarLotes = function (itemcode) {

        $.ajax({
            url: DefaultApiPath + "/GetLotes",
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
                toastr.error(msg, "Erro", _DEFAULT_ERROR_TIMEOUT, { timeOut: 500 });
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


    document.getElementById('numnota').onkeydown = function (e) {
        if (e.keyCode === 13) {
            var numnota = $(this).val();
            BuscarNota(numnota);
        }
    };

    document.getElementById('codbarras').onkeydown = function (e) {
        if (e.keyCode === 13) {
            var codbarras = $(this).val();
            BuscarCodigoBarras(codbarras);
        }
    };

    $('#numlote').on('change', function () {
        var numlote = $(this).val();
        var ItemCode = $('#itemcode').val();
        if (numlote) {
            $.ajax({
                url: DefaultApiPath + "/getlotesinfo",
                method: "GET",
                data: {
                    empresa_id: empresa_id,
                    numlote: numlote,
                    ItemCode: ItemCode,
                },
                success: function (data) {

                    if (data) {



                        AdicionarLinhaItem(
                            data[0]["itemCode"],
                            data[0]["itemName"],
                            1,
                            data[0]["unidadeMedida"],
                            numlote,
                            data[0]["codBarras"],
                            data[0]["dataVenc"],
                            data[0]["ambiente"],
                            data[0]["deposito"],
                            data[0]["loteFabricante"],
                            data[0]["fornecedor"]
                        );
                        toastr.success("Linha adicionada");
                        ResetarItemELote();

                   

                    } else {
                        toastr.error("Lote não encontrado");
                    }
                },
                error: _DEFAULT_ERROR_TREATMENT
            });
        }
    });

    var BuscarNota = function (numnota) {
        $.ajax({
            url: DefaultApiPath + "/getnotafiscal",
            method: "GET",
            data: {
                empresa_id: empresa_id,
                numnota: numnota,
            },
            success: function (data) {

                if (data["itens"]) {

                    toastr.success("Nota encontrada");

                    if (dataset_itens.length > 0) {
                        bootbox.confirm("Deseja remover os itens que já estão na tela?", function (result) {
                            if (result) {
                                dataset_itens = [];
                            }

                            PreencherDadosNotaRecebimento(data);
                        });
                    } else {
                        PreencherDadosNotaRecebimento(data);
                    }
                } else {
                    toastr.error("Nota não encontrada");
                }
            },
            error: _DEFAULT_ERROR_TREATMENT
        });
    }

    var PreencherDadosNotaRecebimento = function (data) {
        var html = "";
        for (var i = 0; i < data.itens.length; i++) {

            var row = data.itens[i];

            AdicionarLinhaItem(
                row["itemCode"],
                row["itemName"],
                row["quantidade"],
                row["unidadeMedida"],
                row["numLote"],
                row["codBarras"],
                row["dataVenc"],
                row["ambiente"],
                row["deposito"],
                row["loteFabricante"],
                row["fornecedor"]
            );
        }


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


                    AdicionarLinhaItem(
                        data[0]["itemCode"],
                        data[0]["itemName"],
                        data[0]["unidadeMedida"],
                        data[0]["numLote"],
                        data[0]["codBarras"],
                        data[0]["dataVenc"],
                        data[0]["ambiente"],
                        data[0]["deposito"],
                        data[0]["loteFabricante"],
                        data[0]["fornecedor"]
                    );
                    toastr.success("Linha adicionada");
                    ResetarItemELote();

                } else {
                    toastr.error("Lote não encontrado");
                }
            },
            error: _DEFAULT_ERROR_TREATMENT
        });
    }



    var AdicionarLinhaItem = function (itemcode, itemname, qtd, unid_medida, numlote, cod_barras, data_venc, ambiente, deposito, lote_fabricante, fornecedor, index) {




        for (var i = 0; i < dataset_itens.length; i++) {
            var ArrayNovo = dataset_itens[i];
            var same = ArrayNovo['numLote'] === numlote
            if (same) {
                toastr.error("Atenção ! Lote Já Selecionado.");
                return error;
            }

        }

        table.rows.add(
            {
                "itemCode": itemcode,
                "itemName": itemname,
                "quantidade": qtd,
                "unidadeMedida": unid_medida,
                "numLote": numlote,
                "codBarras": cod_barras,
                "dataVenc": data_venc,
                "ambiente": ambiente,
                "deposito": deposito,
                "loteFabricante": lote_fabricante,
                "fornecedor": fornecedor,
            }
        ).draw();

        table.draw();

    }


    var Imprimir = function ($button) {

        AtualizarQuantidadesDataset();

        $.ajax({
            url: DefaultApiPath + "/imprimir",
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
                toastr.success("Etiqueta impressa com sucesso.");
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

            dataset_itens[i]["quantidade"] = $('#itens_recebimento tbody #tr' + index + ' #qtd').val();
            dataset_itens[i]["checked"] = $('#itens_recebimento tbody #tr' + index + ' #check').is(':checked');
        }
    }
    $("#itens_recebimento").on("click", ".js-delete", function () {

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