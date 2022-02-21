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

var myModal = document.getElementById('modaL')
var myInput = document.getElementById('myInput')
myModal.addEventListener('shown.bs.modal', function () {
    myInput.focus()
})

$("#basic-datatable").DataTable({
    keys: !0,
    language: {
        paginate: {
            previous: "<i class='mdi mdi-chevron-left'>",
            next: "<i class='mdi mdi-chevron-right'>"
        }
    },
    drawCallback: function () {
        $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
    }
});
var a = $("#datatable-buttons").DataTable({
    lengthChange: !1,
    buttons: ["copy", "print"],
    language: {
        paginate: {
            previous: "<i class='mdi mdi-chevron-left'>",
            next: "<i class='mdi mdi-chevron-right'>"
        }
    },
    drawCallback: function () {
        $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
    }
});
$("#selection-datatable").DataTable({
    select: {
        style: "multi"
    },
    language: {
        paginate: {
            previous: "<i class='mdi mdi-chevron-left'>",
            next: "<i class='mdi mdi-chevron-right'>"
        }
    },
    drawCallback: function () {
        $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
    }
}), a.buttons().container().appendTo("#datatable-buttons_wrapper .col-md-6:eq(0)"), $("#alternative-page-datatable").DataTable({
    pagingType: "full_numbers",
    drawCallback: function () {
        $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
    }
}), $("#scroll-vertical-datatable").DataTable({
    scrollY: "350px",
    scrollCollapse: !0,
    paging: !1,
    language: {
        paginate: {
            previous: "<i class='mdi mdi-chevron-left'>",
            next: "<i class='mdi mdi-chevron-right'>"
        }
    },
    drawCallback: function () {
        $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
    }
}), $(".scroll-horizontal-datatable").DataTable({
    scrollX: !0,
    language: {
        paginate: {
            previous: "<i class='mdi mdi-chevron-left'>",
            next: "<i class='mdi mdi-chevron-right'>"
        }
    },
    drawCallback: function () {
        $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
    }
}), $("#complex-header-datatable").DataTable({
    language: {
        paginate: {
            previous: "<i class='mdi mdi-chevron-left'>",
            next: "<i class='mdi mdi-chevron-right'>"
        }
    },
    drawCallback: function () {
        $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
    },
    columnDefs: [{
        visible: !1,
        targets: -1
    }]
}), $("#row-callback-datatable").DataTable({
    language: {
        paginate: {
            previous: "<i class='mdi mdi-chevron-left'>",
            next: "<i class='mdi mdi-chevron-right'>"
        }
    },
    drawCallback: function () {
        $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
    },
    createdRow: function (a, e, t) {
        15e4 < +e[5].replace(/[\$,]/g, "") && $("td", a).eq(5).addClass("text-danger")
    }
}), $("#state-saving-datatable").DataTable({
    stateSave: !0,
    language: {
        paginate: {
            previous: "<i class='mdi mdi-chevron-left'>",
            next: "<i class='mdi mdi-chevron-right'>"
        }
    },
    drawCallback: function () {
        $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
    }
}), $("#fixed-header-datatable").DataTable({
    fixedHeader: !0
}), $("#fixed-columns-datatable").DataTable({
    scrollY: 300,
    scrollX: !0,
    scrollCollapse: !0,
    paging: !1,
    fixedColumns: !0
}), $(".dataTables_length select").addClass("form-select form-select-sm"), $(".dataTables_length label").addClass("form-label")