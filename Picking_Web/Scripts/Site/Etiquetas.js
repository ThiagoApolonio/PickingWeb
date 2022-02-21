
$(document).ready(function () {

    var empresa_id = document.getElementById("script_etiquetas").getAttribute("data-empresa"), 
    ip = document.getElementById("script_etiquetas").getAttribute("data-ip"), 
    porta = document.getElementById("script_etiquetas").getAttribute("data-porta"), 
    dataset_volume = [],
    currNumDoc = null,
    currStatusDoc = null;

    document.getElementById('numdoc').onkeydown = function (e) {
        if (e.keyCode === 13) {
            BuscarPedido();
        }
    };

    document.getElementById('peso').onkeydown = function (e) {
        if (e.keyCode === 13) {
            AdicionarVolume();
        } else if (e.keyCode === 9) {
            BuscarPesoBalanca();
        }
    };

    var BuscarPesoBalanca = function () {
        if (ip && porta) {
            $.ajax({
                url: DefaultApiPath + "/buscarpesobalanca",
                method: "GET",
                data: {
                    empresa_id: empresa_id,
                    ip: ip,
                    porta: porta
                },
                beforeSend: function () {
                    toastr.clear();
                    toastr.info("Aguarde... Comunicando com a balança...", { timeOut: 9000 });
                },
                success: function (data) {
                    toastr.clear();
                    if (data) {
                        __AdicionarVolume(data);
                        toastr.success('Dados recuperados da balança com sucesso.');
                    } else {
                        toastr.error("Nenhum valor retornado da balança");
                    }
                },
                error: _DEFAULT_ERROR_TREATMENT
            });
        }
    }

    $('#operador').on('change', function() {
        SelecionarOperador();
    });

    $('#transportadora').on('change', function () {
        SelecionarTransportadora();
    });

    $('#encerrar').on('click', function() {
        var $btn = $(this);
        if (PesagemValida()) {
            bootbox.confirm("Deseja encerrar?", function (result) {
                if (result) {
                    EncerrarPesagem($btn);
                }
            });
        }
    });

    $('#imprimir').on('click', function() {
        var $btn = $(this);
        bootbox.confirm("Deseja reimprimir todos os volumes?", function (result) {
            if (result) {
                Reimprimir($btn);
            }
        });
    });

    $('#reset').on('click', function() {
        var $btn = $(this);
        bootbox.confirm("Deseja resetar?", function (result) {
            if (result) {
                Reset($btn);
            }
        });
    });

    $('#cancelar').on('click', function() {
        var $btn = $(this);
        bootbox.confirm("Deseja cancelar?", function (result) {
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
        $('#numdoc').focus();
    }

    var TogglePodeConferir = function (pode) {
        if (pode) {
            var elements_cabecalho = $('#local, #numdoc');
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
        var $elements_cabecalho = $('#local, #numdoc, #peso, #parceiro, #endereco, #transportadora');
        $elements_cabecalho.attr('disabled', 'disabled');
        $elements_cabecalho.val('');
        setTransportadoras();
        currNumDoc = null;
        currStatusDoc = null;
        $('#observacoes').val('');

        $operador = $('#operador');
        $operador.val('');
        $operador.focus();

        ResetarVolume();
        GerenciaPossibilidadeDeReimpressao();
    }

    var setTransportadoras = function (options) {
        $transportadoras = $('#transportadora'),
            default_option = "<option value=''>Selecione o Transportador</option>";
        if (options && options.length > 0) {
            $transportadoras.html(default_option + options);
        } else {
            $transportadoras.html(default_option);
        }
    }

    var SelecionarTransportadora = function () {
        $('#peso').focus();
    }   

    var BuscarPedido = function () {
        currNumDoc = $('#numdoc').val();
        if (currNumDoc) {
            $.ajax({
                url: DefaultApiPath + "/getpedidodata",
                method: "GET",
                data: {
                    empresa_id: empresa_id,
                    numdoc: currNumDoc,
                },
                success: function (data) {
                    console.log(data)
                    if (data["parceiro"]) {
                        $('#parceiro').val(data["parceiro"]);
                    }
                    if (data["endereco"]) {
                        $('#endereco').val(data["endereco"]);
                    }
                    if (data["status"]) {
                        currStatusDoc = data["status"];
                    }
                    if (data["transportadoras"]) {
                        var options = "";
                        for (var i = 0; i < data.transportadoras.length; i++) {
                            options += "<option value= '" + data.transportadoras[i]['cardCode'] + "'>" + data.transportadoras[i]['cardName'] + "</option>";
                        }
                        setTransportadoras(options);
                    }

                    if (data["volumes"].length > 0) {
                        dataset_volume = data["volumes"];
                        DesenharTabelaVolume();
                    }
                    
                    GerenciaPossibilidadeDeReimpressao();

                    $('#docped').val(data['docEntry']);
                    $('#observacoes').val(data['observacoes']);
                    $('#peso, #transportadora').removeAttr('disabled');
                    toastr.clear();
                    toastr.success("Pedido encontrado.");
                    $('#transportadora').focus();
                },
                error: _DEFAULT_ERROR_TREATMENT
            });
        }
    }
    
    var EncerrarPesagem = function ($button) {
        $.ajax({
            url: DefaultApiPath + "/encerrarpesagem",
            method: "POST",
            data: {
                empresa_id: empresa_id,
                numdoc: currNumDoc,
                numped: $('#docped').val(),
                operador: $('#operador').val(),
                local: $('#local').val(),
                transportadora: $('#transportadora').val(),
                pesobruto: GetPesoBruto(),
                nome_impressora: $('#nome_impressora').val(),
                'dataset_volume': dataset_volume
            },
            beforeSend: function () {
                $button.button('loading');
                aguardeMsg();
            },
            success: function (data) {
                toastr.clear();
                bootbox.hideAll();
                toastr.success("Pesagem realizada com sucesso.");

                ResetTela();
            },
            error: _DEFAULT_ERROR_TREATMENT
        }).always(function () {
            $button.button('reset');
        });
    }

    var PesagemValida = function () {
        if ($('#operador').val()) {
            if (currNumDoc > 0) {
                if ($('#transportadora').val()) {
                    if (dataset_volume.length > 0) {
                        return true;
                    } else {
                        toastr.error("NENHUMA PESAGEM FOI REALIZADA");
                        $('#peso').focus();
                        return false;
                    }
                } else {
                    toastr.error("SELECIONE A TRANSPORTADORA");
                    $('#transportadora').focus();
                    return false;
                }
            } else {
                toastr.error("SELECIONE O PEDIDO DE VENDA");
                $('#numdoc').focus();
                return false;
            }
        } else {
            toastr.error("SELECIONE O Operador");
            $('#operador').focus();
            return false;
        }
    }
    
    var Reset = function ($button) {
        $.ajax({
            url: DefaultApiPath + "/reset",
            method: "GET",
            data: {
                empresa_id: empresa_id,
                numdoc: currNumDoc,
            },
            beforeSend: function () {
                $button.button('loading');
                aguardeMsg();
            },
            success: function (data) {
                toastr.clear();
                bootbox.hideAll();
                toastr.success("Pedido resetado com sucesso.");
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

    var __AdicionarVolume = function(peso) {
        var num_peso = Number(peso.replace(",","."));
        if (isNaN(num_peso)) {
            toastr.error("Peso inválido.")
        } else {
            dataset_volume.push(num_peso);
            DesenharTabelaVolume();
        }
        var $peso = $('#peso');
        $peso.val('');
        $peso.focus();
    }

    var AdicionarVolume = function () {
        var $peso = $('#peso');
        var peso = $peso.val();
        __AdicionarVolume(peso);
    }

    var RemoverVolume = function (index) {
        dataset_volume.splice(index-1, 1);
        DesenharTabelaVolume();
    }

    var DesenharTabelaVolume = function() {
        var html = "",
            peso_bruto = 0,
            dataset_len = dataset_volume.length,
            pode_reimprimir = currStatusDoc === 'PE'
        ;

        for (var i = 1; i <= dataset_len; i++) {

            var html_td_reimpressao =
                "   <td align='center'> " +
                "       <button data-index='" + i + "' type='button' class='btn btn-primary js-reimprimir-volume' " + (pode_reimprimir ? 'enabled' : 'disabled') + " > " +
                "           <span class='glyphicon glyphicon-print'></span>&nbsp; " +
                "       </button> " +
                "   </td> ";

            peso = dataset_volume[(i - 1)];
            peso_bruto = peso_bruto + parseFloat(peso);
            html +=
                "<tr> " +
                "   <td align= 'center'>Volume " + i + " / " + dataset_len + "</td> " +
                "   <td align='right'>" + peso + "</td> " +
                html_td_reimpressao +
                "   <td align='center'> " +
                "       <button data-index='" + i + "' type='button' class='btn btn-danger js-remover-volume'> " +
                "           <span class='glyphicon glyphicon-trash'></span>&nbsp; " +
                "       </button> " +
                "   </td> " +
                "</tr >";
        }

        if (peso_bruto > 0) {
            html +=
                "<tr class='text-success'> " +
                "   <td></td> " +
                "   <td align='right'>" + peso_bruto + "</td> " +
                "   <td></td> " +
                "   <td></td> " +
                "</tr >";
        }

        $('#volumes tbody').html(html);
    }

    var ResetarVolume = function () {
        dataset_volume = [];
        DesenharTabelaVolume();
    }

    var GetPesoBruto = function (){
        var peso_bruto = 0;
        var dataset_len = dataset_volume.length;
        for (var i = 1; i <= dataset_len; i++) {
            peso = dataset_volume[(i - 1)];
            peso_bruto = peso_bruto + parseFloat(peso);
        }
        return parseFloat(peso_bruto);
    }

    var Reimprimir = function ($button) {
        $.ajax({
            url: DefaultApiPath + "/reimprimir",
            method: "GET",
            data: {
                empresa_id: empresa_id,
                numdoc: currNumDoc,
                nome_impressora: $('#nome_impressora').val(),
            },
            beforeSend: function () {
                $button.button('loading');
                aguardeMsg();
            },
            success: function (data) {
                toastr.clear();
                bootbox.hideAll();
                toastr.success("Etiqueta reimpressa com sucesso.");
            },
            error: _DEFAULT_ERROR_TREATMENT
        }).always(function () {
            $button.button('reset');
        });
    }

    var ReimprimirVolume = function (index) {
        $.ajax({
            url: DefaultApiPath + "/reimprimirvolume",
            method: "GET",
            data: {
                empresa_id: empresa_id,
                numdoc: currNumDoc,
                nome_impressora: $('#nome_impressora').val(),
                index:index
            },
            beforeSend: function () {
                aguardeMsg();
            },
            success: function (data) {
                toastr.clear();
                bootbox.hideAll();
                toastr.success("Etiqueta reimpressa com sucesso.");
            },
            error: _DEFAULT_ERROR_TREATMENT
        })
    }

    var GerenciaPossibilidadeDeReimpressao = function () {
        var pode_reimprimir = currStatusDoc === 'PE';
        //var $encerrar = $('#encerrar');
        $elements = $('#imprimir,#volumes .js-remover-volume');
        if (pode_reimprimir) {
            //$elements.removeAttr('disabled');
            //$encerrar.attr('disabled', 'disabled');
        } else {
            //$elements.attr('disabled', 'disabled');
            //$encerrar.removeAttr('disabled');
        }
    }

    $("#volumes").on("click", ".js-remover-volume", function () {

        var button = $(this);

        bootbox.confirm("Deseja remover o volume?",
            function (result) {
                if (result) {
                    RemoverVolume(button.attr('data-index'));
                }
            }
        );
    });

    $("#volumes").on("click", ".js-reimprimir-volume", function () {

        var button = $(this);
        var volume_selecionado = button.attr('data-index');

        bootbox.confirm("Deseja reimprimir o volume " + volume_selecionado + "?",
            function (result) {
                if (result) {
                    ReimprimirVolume(volume_selecionado);
                }
            }
        );
    });

    SelecionarOperador();
    GerenciaPossibilidadeDeReimpressao();


    $(function () {
        $('[data-toggle="tooltip"]').tooltip();

        $().alert();
    });
});