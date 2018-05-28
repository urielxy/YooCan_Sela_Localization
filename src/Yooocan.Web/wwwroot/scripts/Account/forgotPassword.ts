import { reopenModal } from "../Shared/Layout";

setTimeout(() => {
    $('#email').focus();
}, 200);

$("#forgot-password-form").validate({
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
                    Materialize.toast('Error has occured :(', 3000);
                }
            }
        });

        return false;
    },
    rules: {
        email: 'required'
    }
});


$('#modal-container #login-link').click(function () {
    return reopenModal(this.href);
});