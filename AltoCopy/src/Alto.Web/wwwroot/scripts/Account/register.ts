$(window).one('resize', function () { $('#registration-container').height('unset'); });

if($('#create-account-step').outerHeight() > 0) {
    $('#registration-container').height($('#create-account-step').outerHeight() + 50);
}

$('#registration-container .back').click(function () {
    transitionRegistrationState(-1);
    return false;
});

function showRegistrationConfirmation() {
    $(".completion-message").hide();
    setRegistrationState(4, true);
}

$(".finish-button").click(finalizeRegistration);

function finalizeRegistration() : void {
    if (isInModal($('#registration-container'))) {
        location.reload();
    } else {
        var returnUrl = getQueryParams()["returnUrl"];
        location.href = returnUrl || "/";
    }
}

function sendRegistrationStateToAnalytics(state: number) {
    const registrationStateToVirtualPage = {
        0: "/Account/Register",
        1: "/Account/UserDetails",
        2: "/Account/LimitationDetails",
        3: "/Account/Payment"
    };

    var path = registrationStateToVirtualPage[state];
    if (path) {
        ga("send", "pageview", path);
    }
}

function setRegistrationState(state: number, animate = false, fieldValues?: {}) {
    var percentage = state * 100;
    
    if (state > 0) {
        hideUnloggedButtonsAndShowLogout();
    }
    if (state > 0 && $('#registration-container').data("payment-replacement")) {
        finalizeRegistration();
        return;
    }
    if (fieldValues) {
        for (var fieldId in fieldValues) {
            if (fieldValues.hasOwnProperty(fieldId)) {
                var fieldValue = fieldValues[fieldId];
                $(`#${fieldId}`).val(fieldValue);
            }
        }
    }
    var neededHeight = $('#registration-container .steps-container .step').eq(state).outerHeight() + 50;
    //400 magic number for a race condition where the step isn't fully initialized sometimes
    if (neededHeight > 400)
        $('#registration-container').height(neededHeight);
    if (animate && currentRegistrationModalState !== undefined && Math.abs(state - currentRegistrationModalState) <= 1.1)
        $('#registration-container .steps-container').animate({ left: `-${percentage}%` });
    else
        $('#registration-container .steps-container').css({ left: `-${percentage}%` });

    $("#modal-container").scrollTop(0);
    currentRegistrationModalState = state;
    sendRegistrationStateToAnalytics(currentRegistrationModalState);
}

function transitionRegistrationState(steps: number) {
    setRegistrationState(currentRegistrationModalState + steps, true);
}

markRequiredFields("#modal-container");
var registrationState = Number(getQueryParams()["registrationState"]);
if (registrationState) {
    setRegistrationState(registrationState);
}

$('.steps-container .step').each(function () {
    $(this)
           //http://stackoverflow.com/questions/7668525/is-there-a-jquery-selector-to-get-all-elements-that-can-get-focus
           .find('a[href], area[href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), button:not([disabled]), iframe, object, embed, *[tabindex], *[contenteditable]')
           .last()
           .keydown(function (e) {
                if (e.which === 9 && !e.shiftKey) {            
                    return false;
                }
           });
});