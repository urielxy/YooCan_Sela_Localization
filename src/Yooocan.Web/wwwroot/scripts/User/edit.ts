import * as PictureUploader from "../Utils/pictureUploader";
import { patchUserFromInputsJquery, clearUserProperty } from "../Utils/UserEditor";

PictureUploader.initPictureUploader(new PictureUploader.PictureUploaderOptions("profile-picture-uploader", "preview-profile-modal",
    200, 200, undefined, undefined, true, "/images/no-avatar.jpg", "rgba(74, 73, 74, 0.78)", () => {
        var promise = patchUserFromInputsJquery($("input[name='PictureDataUri']"));
        if (!window.opener)
            return;

        promise.done(() => $.get('/User/GetAvatar', (response) =>
            window.opener.postMessage({
                property: "avatar",
                value: response
            }, window.location.origin)
        )
        )
    }));

PictureUploader.initPictureUploader(new PictureUploader.PictureUploaderOptions("background-picture-uploader", "preview-background-modal",
    1500, 500, 600, 200, false, undefined, "rgba(127, 143, 164, 0.78)", () => patchUserFromInputsJquery($("input[name='HeaderImageDataUri']"))));

const userAboutTextareaId = "user-about-textarea";
const userAboutParagraphId = "user-about";
const userAboutClass = "user-about";
const userAboutEditButtonId = "edit-user-about";

$(`#${userAboutEditButtonId}`).click(() => {
    var p = $(`#${userAboutParagraphId}`).hide();
    $(`#${userAboutTextareaId}`).text(p.text())
        .width(p.width())
        .height(p.height() + 10)
        .show()
        .focus();
    $(`#${userAboutEditButtonId}`).css('visibility', 'hidden');
});

$(".profile-basic-info").on("focusout", `#${userAboutTextareaId}`, function () {
    var $textArea = $(`#${userAboutTextareaId}`).hide();
    $(`#${userAboutParagraphId}`).text($textArea.val())
        .show();

    $(`#${userAboutEditButtonId}`).css('visibility', 'visible');
    patchUserFromInputsJquery($textArea);

    if (window.opener) {
        window.opener.postMessage({
            property: "about",
            value: $textArea.val()
        }, window.location.origin);
    }
});

var $firstName = $("#first-name");
var $lastName = $("#last-name");
var $firstNameTextBox = $("#first-name-textbox");
var $lastNameTextBox = $("#last-name-textbox");
var $editUserNameButton = $("#edit-user-name");

$editUserNameButton.click(() => {
    $firstName.hide();
    $lastName.hide();
    $firstNameTextBox.show()
        .width($firstName.width() + 10)
        .focus();
    $lastNameTextBox.show()
        .width($lastName.width() + 10);
    $editUserNameButton.hide();
    $firstNameTextBox.next('label').show();
    $lastNameTextBox.next('label').show();
});
$firstNameTextBox.focusout(() => {
    patchUserFromInputsJquery($firstNameTextBox);
    $firstName.text($firstNameTextBox.val())
        .show();
    $firstNameTextBox.hide();
    $editUserNameButton.show();
    $firstNameTextBox.next('label').hide();
    if (window.opener) {
        window.opener.postMessage({
            property: "name",
            value: $firstNameTextBox.val() + ' ' + $lastNameTextBox.val()
        }, window.location.origin);
    }
});
$lastNameTextBox.focusout(() => {
    patchUserFromInputsJquery($lastNameTextBox);
    $lastName.text($lastNameTextBox.val())
        .show();
    $lastNameTextBox.hide();
    $editUserNameButton.show();
    $lastNameTextBox.next('label').hide();
    if (window.opener) {
        window.opener.postMessage({
            property: "name",
            value: $firstNameTextBox.val() + ' ' + $lastNameTextBox.val()
        }, window.location.origin);
    }
});

$('#remove-profile-picture').click(() => {
    clearUserProperty("PictureDataUri");
    if (window.opener) {
        window.opener.postMessage({
            property: "avatar",
            value: ''
        }, window.location.origin);
    }
});

$('#remove-header-picture').click(() => clearUserProperty("HeaderImageDataUri"));

var retries = 0;
$(document).ajaxError((event, jqxhr, settings) => {
    //the server currently refuses to send 409 - it sends 404 instead, currently checking for both for forward compatibility
    if (jqxhr.status === 409 || jqxhr.status === 404) {
        if (retries < 3) {
            retries++;
            $.ajax(settings.url, settings).done(() => { retries = 0; });
        } else {
            retries = 0;
        }
    }
});