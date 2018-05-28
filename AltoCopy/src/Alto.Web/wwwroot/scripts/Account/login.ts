// not sure why this bloody thing is even needed, but it is... :(
setTimeout(() => {
    $('#email').focus();
}, 50);

$("#login-form").validate({
    submitHandler: (form: HTMLFormElement, e: Event) => {
        e.preventDefault();
        $(form).ajaxSubmit({
            success: () => {
                if (isInModal($(form))) {
                    window.location.reload();
                }
                else {
                    location.href = getQueryParams()["returnUrl"] || "/";
                }
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
            }
        });

        return false;
    },
    rules: {
        email: 'required',
        password: 'required'
    }
});


$('#modal-container #change-to-signup').click(function () {
    $('#modal-container').closeModal({
        complete: () => {
            // https://github.com/Dogfalo/materialize/issues/1647
            $('.lean-overlay').remove();

            $('#modal-container > div.modal-content').empty().load(this.href, () => {
                $('#modal-container').openModal({
                    opacity: 0.75,
                    ready: () => {
                        //to fix height
                        setRegistrationState(0);
                    }
                });
            });
        }
    });
    
    return false;
});

$('#forgot-password-link').click(function () {
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

$('#social-form button').click(function() {
    var left = ($(window).width() / 2) - 260;
    var top = ($(window).height() / 2) - 280;
    var $form = $('#social-form');
    var returnUrl = $form.data('return-url');
    var $newWindowUrl = $form.data('window-url');
    window.open(`/Account/ExternalLoginWindow?provider=${this.value}&returnUrl=${returnUrl}`, "SignIn",
        `width=520,height=560,toolbar=0,scrollbars=0,status=0,resizable=0,location=0,menuBar=0,left= ${left},top= ${top}`);
    return false;
});