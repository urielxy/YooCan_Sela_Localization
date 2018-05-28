$("select").material_select();

function markRequiredFields(rootElement: string = "") {
    $(`${rootElement} input[data-val-required], ${rootElement} input[required], ${rootElement} select[data-val-required], ${rootElement} select[required], ${rootElement} textarea[data-val-required], ${rootElement} textarea[required]`)
        .closest(".input-field")
        .children("label")
        .append("<span style='color:#F44336;'> *</span>");
}
markRequiredFields();

//allows jquery validation to work on some materialize controls like select (if validator was loaded)
$.validator && $.validator.setDefaults({
    errorClass: 'invalid',
    validClass: "valid",
    errorPlacement: function (error, element) {
        $(element)
            .closest("form")
            .find("label[for='" + element.attr("id") + "']")
            .attr('data-error', error.text());
    },
    ignore: ""
});

function deferCss(src: string) {
    var stylesheet = document.createElement('link');
    stylesheet.href = src;
    stylesheet.rel = 'stylesheet';
    stylesheet.type = 'text/css';
    document.getElementsByTagName('head')[0].appendChild(stylesheet);
};

//storing the state from previous open of the modal
var currentRegistrationModalState:number = undefined;

function getTargetRegistrationState(membershipState: MembershipState = MembershipState.Unregistered): number {
    if (currentRegistrationModalState)
        return currentRegistrationModalState;
    else if (membershipState === MembershipState.Registered)
        return 1;
    else if (membershipState === MembershipState.FilledDetails || membershipState === MembershipState.Expired)
        return 3;
    return 0;
}

function registerModal(membershipState: MembershipState = MembershipState.Unregistered, isCompletionNeeded = false, paymentReplacementUrl: string = null) {
    var source = $(this).data('source') || $(this).data('action') || this.id;
    //window.ga('send', 'event', 'User', 'Register popup', source);

    var targetRegistrationState = getTargetRegistrationState(membershipState);

    var urlWithParams = registerUrl + "?" + (isCompletionNeeded ? "&isCompletionNeeded=true" : "")
        + (paymentReplacementUrl ? `&paymentReplacement=${paymentReplacementUrl}` : "");

    const returnUrl = encodeURIComponent(getQueryParams()["returnUrl"] || location.pathname);
    if (membershipState === MembershipState.YoocanUnregistered) {       
        const yoocanHostName = "yoocanfind.com";
        const yoocanTokenRedirectUrl = encodeURIComponent(`/External/RedirectToAltoWithToken/?returnUrl=${returnUrl}`);
        location.href = `https://${yoocanHostName}/Account/Register?fromAlto=1&returnUrl=${yoocanTokenRedirectUrl}`;
        return false;
    }

    if (isMobile()) {
        location.href = urlWithParams +
            `&returnUrl=${returnUrl}` +
            (targetRegistrationState !== 0 ? `&registrationState=${targetRegistrationState}` :"");
        return false;
    }

    $('#spinner-container').show();
    $('#modal-container > div.modal-content').empty().load(urlWithParams, function () {
        $('#spinner-container').hide();
        if (targetRegistrationState > 0) {
            setRegistrationState(targetRegistrationState);
        }
        else {
            sendRegistrationStateToAnalytics(0);
        }
    });
    $('#modal-container').openModal({ opacity: 0.75 });
    return false;
}

function loginModal() {
    if (isMobile()) {
        location.href = loginUrl +
            `?returnUrl=${encodeURIComponent(getQueryParams()["returnUrl"] || location.pathname)}`;
        return false;
    }
    $('#spinner-container').show();
    $('#modal-container > div.modal-content').empty().load(loginUrl, function () {
        $('#spinner-container').hide();
    });
    $('#modal-container').openModal({ opacity: 0.75 });
    ga("send", "pageview", "/Account/Login");
    return false;
};

var $headerLoginBtn = $('#header-login-btn');
var $headerRegisterBtn = $('#header-register-btn');
var loginUrl = $headerLoginBtn.attr('href');
var registerUrl = $headerRegisterBtn.attr('href');

$headerLoginBtn.click(loginModal);
$(".login-button").click(loginModal);
$headerRegisterBtn.click(registerModal);
$(".register-button").click(registerModal);

function hideUnloggedButtonsAndShowLogout(): void {
    hideUnloggedButtons();
    $("#logout-container").show();
}

function hideUnloggedButtons(): void {
    $headerLoginBtn.hide();
    $headerRegisterBtn.hide();
    $(".register-button").hide();
    $(".login-button").hide();
}

function isUserAuthenticated(): boolean {
    //the display of the button is set correctly by Razor in Header.cshtml
    return $headerRegisterBtn.css("display") === "none";
}

if (isUserAuthenticated()) {
    hideUnloggedButtons();
}

function isMobile() {
    return screen.width < 768;
}

function isInModal(element: JQuery) {
    return (element.closest("#modal-container").length > 0);    
}

function getQueryParams() {
    var url = location.search;
    var querystringPairs = url.substring(url.indexOf('?') + 1).split('&');
    var result = {};
    for (var i = 0; i < querystringPairs.length; i++) {
        var pair = querystringPairs[i].split('=');
        result[pair[0].substring(0, 1).toLowerCase() + pair[0].substring(1)] = decodeURIComponent(pair[1]);
    }
    return result;
}

// Using https://github.com/kamens/jQuery-menu-aim/
!function (a) { function b(b) { var c = a(this), d = null, e = [], f = null, g = null, h = a.extend({ rowSelector: "> li", submenuSelector: "*", submenuDirection: "right", tolerance: 75, enter: a.noop, exit: a.noop, activate: a.noop, deactivate: a.noop, exitMenu: a.noop }, b), i = 3, j = 300, k = function (a) { e.push({ x: a.pageX, y: a.pageY }), e.length > i && e.shift() }, l = function () { g && clearTimeout(g), h.exitMenu(this) && (d && h.deactivate(d), d = null) }, m = function () { g && clearTimeout(g), h.enter(this), q(this) }, n = function () { h.exit(this) }, o = function () { p(this) }, p = function (a) { a != d && (d && h.deactivate(d), h.activate(a), d = a) }, q = function (a) { var b = r(); b ? g = setTimeout(function () { q(a) }, b) : p(a) }, r = function () { function o(a, b) { return (b.y - a.y) / (b.x - a.x) } if (!d || !a(d).is(h.submenuSelector)) return 0; var b = c.offset(), g = { x: b.left, y: b.top - h.tolerance }, i = { x: b.left + c.outerWidth(), y: g.y }, k = { x: b.left, y: b.top + c.outerHeight() + h.tolerance }, l = { x: b.left + c.outerWidth(), y: k.y }, m = e[e.length - 1], n = e[0]; if (!m) return 0; if (n || (n = m), n.x < b.left || n.x > l.x || n.y < b.top || n.y > l.y) return 0; if (f && m.x == f.x && m.y == f.y) return 0; var p = i, q = l; "left" == h.submenuDirection ? (p = k, q = g) : "below" == h.submenuDirection ? (p = l, q = k) : "above" == h.submenuDirection && (p = g, q = i); var r = o(m, p), s = o(m, q), t = o(n, p), u = o(n, q); return r < t && s > u ? (f = m, j) : (f = null, 0) }; c.mouseleave(l).find(h.rowSelector).mouseenter(m).mouseleave(n).click(o), a(document).mousemove(k) } a.fn.menuAim = function (a) { return this.each(function () { b.call(this, a) }), this } } (jQuery);
var $container = $('#sub-menu-container');
($(".menu-main-dropdown") as any).menuAim({
    activate: function (li) {
        $('.menu-main-dropdown li').removeClass('active');

        $(li).addClass('active');
        var index = $(li).index();
        $('.sub-menu-dropdown')
            .addClass('hide')
            .eq(index)
            .removeClass('hide');
    },
    deactivate: $.noop,
    exitMenu: ()=> { return true; },
    enter: function () {
        if ($container.hasClass('hide'))
            $container.width(0).removeClass('hide').show().animate({ width: '240px' });
    }
});

$('#drop-menu-container').mouseleave(function () {
    $('.sub-menu-dropdown, #sub-menu-container').addClass('hide');
});