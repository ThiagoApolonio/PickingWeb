var currentDay = function() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    return dd + '/' + mm + '/' + yyyy;
}


// sobrescrevendo default do bootbox.
bootbox.setDefaults({
    locale: "pt"
});

var toggleFullScreen = function() {
    if ((document.fullScreenElement && document.fullScreenElement !== null) ||
        (!document.mozFullScreen && !document.webkitIsFullScreen)) {
        if (document.documentElement.requestFullScreen) {
            document.documentElement.requestFullScreen();
        } else if (document.documentElement.mozRequestFullScreen) {
            document.documentElement.mozRequestFullScreen();
        } else if (document.documentElement.webkitRequestFullScreen) {
            document.documentElement.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
        }
    } else {
        if (document.cancelFullScreen) {
            document.cancelFullScreen();
        } else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        } else if (document.webkitCancelFullScreen) {
            document.webkitCancelFullScreen();
        }
    }
}

var aguardeMsg = function()
{
    toastr.info("aguarde...", { timeOut: 50000 });
}

var _DEFAULT_SECONDS_ERROR_TIMEOUT = 50000;
var _DEFAULT_ERROR_TIMEOUT = { timeOut: _DEFAULT_SECONDS_ERROR_TIMEOUT, extendedTimeOut: _DEFAULT_SECONDS_ERROR_TIMEOUT };
var _DEFAULT_ERROR_TREATMENT = function(jqXHR, textStatus, errorThrown) {
    toastr.clear();
    var msg = jqXHR.responseJSON.message;
    toastr.error(msg, "Erro", _DEFAULT_ERROR_TIMEOUT);
};

var _DEFAULT_SCRIPT_LANG = {
    "sUrl": DefaultScriptPath + "/lang.txt"
};

var ws = null;
var primeiraVez = false;

var InitWS = function (name) {
    ws = new WebSocket(getWSURL(name));

    ws.onopen = function () {
        primeiraVez = true;
    };

    ws.onmessage = function (e) {
        
        if (e.data === name && !primeiraVez)
        {
            EncerrarSessao();
        }
        primeiraVez = false;
    };

    ws.onclose = function () {};

    //ws.onerror = function (e) {
    //    alert('Erro na conexão\nErro: ' + e);
    //};
}

var getWSURL = function (name) {
    var ashxFileName = "ws.ashx";
    var url = window.location.href.replace('http', 'ws');

    var lastURLPiece = window.location.pathname;
    if (lastURLPiece === "/") {
        url += ashxFileName;
    } else {
        url = url.replace(lastURLPiece, '/' + ashxFileName);
    }

    url += '?name=' + name;
    return url;
}

var EncerrarSessao = function () {
    var timeout = 10000;
    toastr.error("Este usuário está sendo acessado de outro local", "Você será desconectado", { timeOut: timeout });
    setTimeout(function () {
        document.getElementById('logoutForm').submit();
    }, timeout);
}

function _Reload() {

    location.reload();
}

function _SairdoSistema() {
    $.ajax({
        url: "/Account/SairdoSistema",
        method: "GET",
        data: {

        },
        success: function (data) {
            location.reload();
        },
        error: _DEFAULT_ERROR_TREATMENT
    });

}

function _Inicio() {
    $.ajax({
        url: "/Home/Index",
        method: "GET",
        data: {

        },
        success: function (data) {

        },
        error: _DEFAULT_ERROR_TREATMENT
    });

}
function _Register() {
    $.ajax({
        url: "/Account/Register",
        method: "POST",
        data: {

        },
        success: function (data) {

        },
        error: _DEFAULT_ERROR_TREATMENT
    });
}
function _Carregarlista() {
    $.ajax({
        url: "/Usuarios/Novo",
        method: "GET",
        data: {

        },
        success: function (data) {

        },
        error: _DEFAULT_ERROR_TREATMENT
    });
}
function _SimularClickSeparacao() {

    // Capturando todos os elementos action
    const collection = document.getElementsByTagName("a");
    collection[4].click();//Atualmente separação

  
}

function countPendente() {
    var empresa_id = document.getElementById("script_lista_picking").getAttribute("data-empresa");
    $.ajax({
        url: "/API/Picking/countAbertas?empresa_id=" + empresa_id,
        method: "GET",
        data: {
            empresa_id: empresa_id,
        },
        success: function (data) {

            let el = document.getElementById("qtdseparacao");
            el.textContent = data;
        },
        error: _DEFAULT_ERROR_TREATMENT
    });

    $.ajax({
        url: "/API/Picking/countfutura?empresa_id=" + empresa_id,
        method: "GET",
        data: {
            empresa_id: empresa_id,
        },
        success: function (data) {

            let el = document.getElementById("qtdseparacaofutura");
            el.textContent = data;
        },
        error: _DEFAULT_ERROR_TREATMENT
    });

    $.ajax({
        url: "/API/Picking/countpendente?empresa_id=" + empresa_id,
        method: "GET",
        data: {
            empresa_id: empresa_id,
        },
        success: function (data) {

            let el = document.getElementById("qtdseparacaopendente");
            el.textContent = data;
        },
        error: _DEFAULT_ERROR_TREATMENT
    });

    $.ajax({
        url: "/API/Picking/countOrdens?empresa_id=" + empresa_id,
        method: "GET",
        data: {
            empresa_id: empresa_id,
        },
        success: function (data) {

            let el = document.getElementById("qtdordenproducao");
            el.textContent = data;
        },
        error: _DEFAULT_ERROR_TREATMENT
    });
    //AcompanhamentoPicking
  
};
