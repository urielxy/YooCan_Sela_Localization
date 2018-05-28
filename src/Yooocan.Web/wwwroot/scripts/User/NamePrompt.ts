import { patchUserFromInputsJquery } from "../Utils/UserEditor";
import { reopenModal, getQueryParams } from "../Shared/Layout";

$(document).on("click", ".name-form button", e => {
    var $form = $(e.target).closest(".name-form");
    $form.validate();
    if ($form.valid()) {
        var preloader = $("#modal-container .name-form-preloader");
        preloader.show();
        var xhr = patchUserFromInputsJquery($("#modal-container input:not([type='hidden'])"));
        xhr.done(() => location.reload())
           .always(() => preloader.hide())
           .fail(() => Materialize.toast("An error occurred, please try again later.", 3000));
    }
});

export function openNameFormModal() {
    reopenModal($(".name-form-container"), () => $("#modal-container input[name='FirstName']").focus());
}