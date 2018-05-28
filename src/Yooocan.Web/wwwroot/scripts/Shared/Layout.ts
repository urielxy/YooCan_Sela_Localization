export var isAuthenticated = (function () {
    var isAuthenticated = $('#isAuthenticated').val() === 'True';
    return function () {
        return isAuthenticated;
    }
})();

(<any>window).isAuthenticated = isAuthenticated;

export function deferCss(src: string) {
    var stylesheet = document.createElement('link');
    stylesheet.href = src;
    stylesheet.rel = 'stylesheet';
    stylesheet.type = 'text/css';
    document.getElementsByTagName('head')[0].appendChild(stylesheet);
};

// Init all selects
$('select').material_select();
$("select[required]").css({ display: "inline", height: 0, padding: 0, width: 0 });
$.validator.setDefaults({
    errorClass: 'invalid',
    validClass: "valid",
    errorPlacement: function (error, element) {
        $(element)
            .closest("form")
            .find("label[for='" + element.attr("id") + "']")
            .attr('data-error', error.text());
    },
    ignore: ".cr-slider"
});

export function markRequiredFields(rootElement: string = "") {
    $(`${rootElement} input[data-val-required], ${rootElement} input[required], ${rootElement} select[data-val-required], ${rootElement} select[required], ${rootElement} textarea[data-val-required], ${rootElement} textarea[required]`)
        .closest(".input-field")
        .children("label")
        .append("<span style='color:#F44336;'> *</span>");
}

//http://stackoverflow.com/a/1186309/601179
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

export function deferStyleSheet(src: string) {
    var stylesheet = document.createElement('link');
    stylesheet.href = src;
    stylesheet.rel = 'stylesheet';
    stylesheet.type = 'text/css';
    document.getElementsByTagName('head')[0].appendChild(stylesheet);
};

(<any>window).deferStyleSheet = deferStyleSheet;
(<any>window).getLeft = function (width: number) {
    var left = (screen.width / 2) - (width / 2);
    return left;
};

(<any>window).getTop = function (top: number) {
    var top = (screen.height / 2) - (top / 2);
    return top;
};

$('form').on('keyup keypress', '*', function (e) {
    if (e.which === 13 && $(e.target).is(':not(button):not(a):not(input[type="button"]):not(textarea)') && !$(e.target).data('allow-enter'))
        return false;
});

if ((<any>window).broadcastFbLoad) {
    (<any>window).broadcastFbLoad = false;
    $('body').trigger('fb:load');
}


var hasNotificationsLoaded = false;
$('.open-notifications-button').click((e) => {
    //e.preventDefault();
    if (!hasNotificationsLoaded) {
        //$('#open-notifications-button').dropdown('open');
        $("#notifications-popup").load("/Notification/Index", function () {
            hasNotificationsLoaded = true;
        });
        $(".new-notifications").hide();
    }
});
if (isAuthenticated()) {
    $.ajax("/Notification/UnreadCount").done((data) => {
        if (data !== 0) {
            $(".new-notifications").text(data).show();
        }
    });
}

//https://github.com/Dogfalo/materialize/issues/2051 fix adapted for hover activation and made it more keyboard accessible
var headerDropDowns = $("#page-header .dropdown-button");
headerDropDowns.each((i, dropDown) => {
    var $dropDown = $(dropDown);
    var activates = $("#" + $dropDown.attr('data-activates'));
    $dropDown.on("mouseenter focus click", function () {
        $dropDown.after(activates);
    });

    $dropDown.on("keydown", (e: JQueryKeyEventObject) => {
        var isActive = activates.hasClass("active");
        if (!isActive && (e.which == 32 || e.which == 40)) {
            for (var i= 0; i < headerDropDowns.length; i++) {
                ($(headerDropDowns[i]) as any).dropdown("close");
            }
            ($dropDown as any).dropdown("open");
            e.preventDefault();
        }
        else if (isActive && (e.which == 32 || e.which == 38)) {
            ($dropDown as any).dropdown("close");
            e.preventDefault();
        }
    });
});

var $headerLoginBtn = $('.header-login-btn');
var $headerRegisterBtn = $('.header-register-btn');
var loginUrl = $headerLoginBtn.attr('href');
var registerUrl = $headerRegisterBtn.attr('href');

var mobileWidth = 768;
export function isMobile(): boolean {
    return screen.width < mobileWidth;
}
(<any>window).isMobile = isMobile;

function isInRegisterOrLoginPage(): boolean {
    //currently this if will return false positive for routes like /Accou, /Account/, /Account/Regis - not existing routes
    return location.pathname.length > 4 && //prevent being true on the home page
        (loginUrl.indexOf(location.pathname) > -1 ||
            registerUrl.indexOf(location.pathname) > -1);
}

function addParamsToReturnUrls(params: Map<string, string>) {
    var paramsString = "";
    if (registerUrl.toLowerCase().indexOf("%3f") > -1) {
        paramsString += "&";
    }
    else {
        paramsString += "?";
    }
    var newParams = Array.from(params).filter(([key, value]) => registerUrl.indexOf(key) < 0);
    if (newParams.length > 0) {
        paramsString += newParams.map(([key, value]) => `${key}=${value}&`);
        registerUrl += encodeURIComponent(paramsString);
        loginUrl += encodeURIComponent(paramsString);
    }
}

export function registerModal(callerSource?: string, paramsForReturnUrl?: Map<string, string>) {
    if (paramsForReturnUrl) {
        addParamsToReturnUrls(paramsForReturnUrl);
    }

    var source = callerSource || $(this).data('source') || $(this).data('action') || this.id;
    ga('send', 'event', 'User', 'Register popup', source);
    if (isInRegisterOrLoginPage()) {
        location.href = registerUrl;
        return false;
    }

    $('#spinner-container').show();
    $('#modal-container > div.modal-content').empty().load(registerUrl, function () {
        $('#spinner-container').hide();
    });
    openModal();
    return false;
}
(<any>window).registerModal = registerModal;

export function reopenModal(nextModalContent: JQuery | string, complete?: Function) {
    $('#modal-container').closeModal({
        complete: () => {
            // https://github.com/Dogfalo/materialize/issues/1647
            $('.lean-overlay').remove();
            var emptyModal = $('#modal-container > div.modal-content').empty();
            if (typeof nextModalContent === "string") {
                emptyModal.load(nextModalContent, () => {
                    openModal();
                    if (complete)
                        complete();
                });
            }
            else {
                emptyModal.append(nextModalContent);
                openModal();
                $('#spinner-container').hide();
                if (complete)
                    complete();
            }
        }
    });

    return false;
}

$headerLoginBtn.click(function () {
    if (isInRegisterOrLoginPage()) {
        location.href = loginUrl;
        return false;
    }

    $('#spinner-container').show();
    $('#modal-container > div.modal-content').empty().load(loginUrl, function () {
        $('#spinner-container').hide();
    });
    openModal();
    return false;
});
var registerModalEventHandler = function () { return registerModal.apply(this); };
$headerRegisterBtn.click(registerModalEventHandler);
$(".register-button").click(registerModalEventHandler);

export function openModal() {
    $('#modal-container').openModal({
        opacity: 0.75,
        complete: iOS11BugWorkaroundRestore
    });
    $('#modal-container .modal-close-corner').focus();
    iOS11BugWorkaround();
}

function iOS11BugWorkaroundRestore() {
    if (isIOS11()) {
        document.body.style.position = "static";
    }
}

function iOS11BugWorkaround() {
    if (isIOS11()) {
        document.body.style.position = "fixed";
    }
}

function isIOS11(): boolean {
    var iosVersion = getiOSversion();
    return iosVersion && iosVersion[0] == 11;
}

function getiOSversion() {
    try {
        if (/iP(hone|od|ad)/.test(navigator.platform)) {
            // supports iOS 2.0 and later: <http://bit.ly/TJjs1V>
            var v = (navigator.appVersion).match(/OS (\d+)_(\d+)_?(\d+)?/);
            return [parseInt(v[1], 10), parseInt(v[2], 10), parseInt(v[3] || "0", 10)];
        }
    }
    catch (e) { }

    return null;
}

export function initSocialForm(element: JQuery) {
    element.find("button").click((e) => {
        var left = ($(window).width() / 2) - 260;
        var top = ($(window).height() / 2) - 280;
        var returnUrl = element.data('return-url');
        window.open(`/Account/ExternalLoginWindow?provider=${e.currentTarget.getAttribute("value")}&returnUrl=${returnUrl}`, "SignIn",
            `width=520,height=560,toolbar=0,scrollbars=0,status=0,resizable=0,location=0,menuBar=0,left= ${left},top= ${top}`);
        return false;
    });
}

$('#newsletter-form').submit(function () {
    if (!$('#newsletter-email').val())
        return false;

    ga('send', 'event', 'Newsletter', 'Signup', isAuthenticated().toString());
    $(this).ajaxSubmit({
        error: function () {
            if (!navigator.onLine)
                alert('Please make sure you have a good internet connection and try again.');
            else {
                alert('Error has occured :( our engineers are on it!');
            }
        },
        success: () => {
            var duration = 3000;
            Materialize.toast('Subscribed!', duration, 'rounded')
            $('#toast-container').addClass('bottom')
            this.reset();
            setTimeout(() => $('#toast-container').removeClass('bottom'), duration);
        }
    });

    return false;
});

export function getFirstName(): string {
    return $('#hid-first-name').val();
}

export function getLastName(): string {
    return $('#hid-last-name').val();
}

export function getUserAvatar(): string {
    return $('#hid-user-avatar').val();
}

var $searchDropDown = $("#search-dropdown");

//prevents dropdown from closing when clicking inside
$searchDropDown.on("click touchstart", e => {
    if (e.eventPhase === 3)
        e.stopPropagation();
});

$("#search-dropdown-button").click(() => setTimeout(() => $searchDropDown.find("input").focus()));
$("#side-menu-button").sideNav({ menuWidth: 285 });

var $mobileHeader = $('#header-mobile');
var lastScrollTop = 0;
var delta = 5;
var navbarHeight = $mobileHeader.outerHeight();

export function toggleMobileHeader(hide: boolean) {
    if (hide)
        $mobileHeader.addClass('header-hidden');
    else
        $mobileHeader.removeClass('header-hidden');
}

import { throttle, debounce } from 'lodash-es'

var isLeading = false;
//done to prevent the event from triggering when the document jumps to location when loading or when doing form validation
$(window).scroll(debounce(() => isLeading = true, 500, { leading: true, trailing: false }));
$(window).scroll(throttle(() => isLeading = false, 160, { leading: false, trailing: true }));

$(window).scroll(throttle(onScroll, 150, { trailing: true }));

function onScroll() {
    if (isLeading) {
        return;
    }
    var scrollTop = $(this).scrollTop();
    if (Math.abs(lastScrollTop - scrollTop) <= delta)
        return;

    if (scrollTop > lastScrollTop && scrollTop > navbarHeight) {
        // Scroll Down
        toggleMobileHeader(true);
        executeCallbacks(scrollDownCallbacks);
    }
    else if (scrollTop + $(window).height() < $(document).height()) {
        // Scroll Up
        toggleMobileHeader(false);
        executeCallbacks(scrollUpCallbacks);
    }

    lastScrollTop = scrollTop;
}

var scrollUpCallbacks: Array<Function> = [];
var scrollDownCallbacks: Array<Function> = [];

function executeCallbacks(callbacks: Array<Function>) {
    callbacks.forEach(callback => callback());
}

export function onScrollUp(callback: Function) {
    scrollUpCallbacks.push(callback);
}

export function onScrollDown(callback: Function) {
    scrollDownCallbacks.push(callback);
}

export function toastHideChat(message: string, duration: number = undefined, className?: string): void {
    if (!isMobile()) {
        Materialize.toast(message, duration, className);
        return;
    }
    var $chatContainer = $(".iflychat-popup");
    $chatContainer.hide();
    Materialize.toast(message, duration, className, () => $chatContainer.show());
}

export function addProductCardsAnalytics(category: string) {
    $(".product-card-link").click(getProductCardAnalyticsHandler(category));
}
(window as any).addProductCardsAnalytics = addProductCardsAnalytics;

export function getProductCardAnalyticsHandler(category: string): (event: JQueryEventObject) => void {
    return function (event: JQueryEventObject) {
        var clickedProductElement = $(event.currentTarget);
        var productId = clickedProductElement.data("product-id");
        var productName = clickedProductElement.data("product-name");
        ga('send', 'event', category, 'product card click', productName, { "dimension2": productId });
    }
}

export function getQueryParams() {
    var url = location.search;
    var querystringPairs = url.substring(url.indexOf('?') + 1).split('&');
    var result = {};
    for (var i = 0; i < querystringPairs.length; i++) {
        var pair = querystringPairs[i].split('=');
        result[pair[0].substring(0, 1).toLowerCase() + pair[0].substring(1)] = decodeURIComponent(pair[1]);
    }
    return result;
}