// not sure why this bloody thing is even needed, but it is... :(
setTimeout(() => {
    $('#email').focus();
}, 50);

$('#modal-container #login-btn').click(function () {
    $('#modal-container').closeModal({
        complete: () => {
            // https://github.com/Dogfalo/materialize/issues/1647
            $('.lean-overlay').remove();
            $('#modal-container > div.modal-content').empty().load(this.href, () => {
                $('#modal-container').openModal({ opacity: 0.75 });
            });
        }
    });

    return false;
});


$("#register-form").validate({    
    submitHandler: function (form: HTMLFormElement, e: JQueryEventObject) {
        var $registerButton = $("#register-form #submit");
        $registerButton.prop("disabled", true);
        e.preventDefault();
        $(form).ajaxSubmit({
            success: () => {                
                setRegistrationState(1);
                ga("send", "pageview", "/Account/RegistrationCompleted");
                fbq("track", "Lead");
            },
            error: (response) => {
                if (response.responseJSON && $.isArray(response.responseJSON))
                    response.responseJSON.forEach((value: any) => {
                        if (!value.field) {
                            value.field = 'email';
                        }

                        $(`#${value.field}`).addClass('invalid').next().attr('data-error', value.error);
                    });
                else {
                    alert('Error has occurred');
                }
                $registerButton.prop("disabled", false);
            }
        });

        return false;
    },
    rules: {
        password: "required",
        "confirmPassword": {
            equalTo: "#password"
        },
        email: 'required'
    },
    messages: {
        "confirmPassword": {
            equalTo: "Passwords don't match"
        }
    }
});
$('#social-form button').click(function () {
    var left = ($(window).width() / 2) - 260;
    var top = ($(window).height() / 2) - 280;
    var $form = $('#social-form');
    var returnUrl = $form.data('return-url');
    var $newWindowUrl = $form.data('window-url');
    window.open(`/Account/ExternalLoginWindow?provider=${this.value}&returnUrl=${returnUrl}`, "SignIn",
        `width=520,height=560,toolbar=0,scrollbars=0,status=0,resizable=0,location=0,menuBar=0,left= ${left},top= ${top}`);
    return false;
});