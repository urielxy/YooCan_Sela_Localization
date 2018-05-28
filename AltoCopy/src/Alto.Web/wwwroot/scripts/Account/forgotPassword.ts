setTimeout(() => {
    $('#email').focus();
}, 200);

$("#forgot-password-form").validate({
    submitHandler: (form: HTMLFormElement, e: Event) => {
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
                    Materialize.toast('Error has occurred :(', 3000);
                }
            }
        });

        return false;
    },
    rules: {
        email: 'required'
    }
});


$('#login-link').click(function () {
    $('#modal-container').closeModal({
        complete: () => {
            // https://github.com/Dogfalo/materialize/issues/1647
            $('.lean-overlay').remove();

            $('#modal-container > div.modal-content').load(this.href, () => {
                $('#modal-container').openModal({ opacity: 0.75 });
            });
        }
    });
    
    return false;
});