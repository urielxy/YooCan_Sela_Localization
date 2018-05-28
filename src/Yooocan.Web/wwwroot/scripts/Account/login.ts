import { reopenModal, initSocialForm } from "../Shared/Layout";

// not sure why this bloody thing is even needed, but it is... :(
setTimeout(() => {
    $('#email').focus();
}, 50);

$("#modal-container #login-form").validate({
    submitHandler: function (form: HTMLFormElement, e: Event) {
        e.preventDefault();
        $(form).ajaxSubmit({
            success: () => {
                window.location.reload();
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
                    alert('Error has occured');
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


$('#modal-container #change-to-signup,#forgot-password-link').click(function () {
    return reopenModal(this.href);
});

initSocialForm($("#login-container .social-form"));