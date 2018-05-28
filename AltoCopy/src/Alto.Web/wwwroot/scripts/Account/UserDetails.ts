var $datePicker = <any>$('.date-picker');
$datePicker.pickadate({
    selectMonths: false,
    selectYears: 120,
    today: '',
    clear: '',
    close: '',
    //newer versions of pickadate have 'closeOnSelect' option, materialized haven't updated theirs for a couple of years, so walkaround is below:
    onSet: function (context) {
        if (context.select)
            this.close();
    }
});

$("select").material_select();

$('#registration-container .btn.next').click(function () {
    if($("#user-details-form").valid())
        transitionRegistrationState(1);
    return false;
});

$('#LimitationOtherOption').click(function () {
    $('#LimitationOther').focus();
});

$('#LimitationOther').on("input",
    () => {
        if ($('#LimitationOther').val() === "") {
            $('#LimitationOtherOption').prop('checked', false);
        } else {
            $('#LimitationOtherOption').prop('checked', true);
        }
    });

$('#user-details-form').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$.validator.unobtrusive.parse($("#user-details-form"));
$("#user-details-form").data("validator").settings.submitHandler = function (form) {
    var $submitButton = $(".done-container button");
    $submitButton.prop("disabled", true);
    $(form).ajaxSubmit({
        success: (state) => {
            if (state === MembershipState.FilledDetails)
                setRegistrationState(3);
            else {
                showRegistrationConfirmation();
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
            $submitButton.prop("disabled", false);
        }
    });
    return false;
};