import { isAuthenticated, initSocialForm, reopenModal, openModal } from "../Shared/Layout";

function smallRegisterModal() {
    ga('send', 'event', 'User', 'small register popup', location.href, { nonInteraction: true });
    $('#spinner-container').hide();
    $('#modal-container > div.modal-content').empty().append($("#small-register-template .small-register").clone());
    openModal();
    initSocialForm($('#modal-container .small-register .social-form'));
    $('#modal-container .small-register .email-register-button,.login-button').click(function () {
        return reopenModal(this.href, () => $("#modal-container #register-container .social").hide());
    });
    $('#modal-container .small-register .social-numbers').flickity({
        autoPlay: true,
        pageDots: false,
        wrapAround: true
    });
    return false;
}

if (!isAuthenticated()) {
    smallRegisterModal();
}