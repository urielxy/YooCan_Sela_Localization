import { registerModal, isAuthenticated } from "../Shared/Layout";

declare var paypal: any;
var $paypalButton = $("#paypal-button");

var $paypalProductPreloader = $("#paypal-product-preloader");
$paypalProductPreloader.show();

paypal.Button.render({

    env: $paypalButton.data("environment"),

    style: {
        size: 'medium',
        color: 'blue',
        shape: 'rect'
    },

    onClick: () => {
        ga('send', 'event', 'products', 'buy in yoocan button click', $("#product-page").data("product-name"));
    },

    payment: function (resolve: any, reject: any) {
        if (!isAuthenticated()) {
            registerModal("product paypal button");
            return false;
        }
        var variations = {};
        var $selects = $('#product-page .select-container select');
        var $emptySelects = $selects.filter(function () { return !this.value });
        if ($emptySelects.length) {
            var $firstEmpty = $emptySelects.eq(0);
            var variationName = $firstEmpty.siblings('label').text();
            var vowels = 'aeiou';
            $firstEmpty.addClass('error');
            setTimeout(() => { $firstEmpty.removeClass('error'); }, 4000);
            var a_an = vowels.indexOf(variationName[0].toLowerCase()) > -1 ? 'an' : 'a';
            Materialize.toast(`Please select ${a_an} ${variationName}`, 4000);
            return false;
        }
        $selects.each(function () {
            var variationId = this.id.split('var-')[1];
            variations[variationId] = parseInt(this.value);
        });

        $.post($paypalButton.data("create-url"), { variations: JSON.stringify(variations) })
            .done(data => {
                ga("send", "pageview", "/Product/PaymentAuthorization");
                resolve(data.paymentID);
            })
            .fail(err => { reject("server error"); });
    },

    onAuthorize: (data: any) => {
        $('#spinner-container').show();
        $('#modal-container > div.modal-content').empty();
        $('#modal-container').openModal({ opacity: 0.75 });

        $.post($paypalButton.data("execute-url"),
            { paymentID: data.paymentID, payerID: data.payerID })
            .done(confirmationPage => {
                $('#modal-container > div.modal-content').empty().append(confirmationPage);
                $('#spinner-container').hide();
            })
            .fail(err => {
                $('#modal-container').closeModal();
                alert("Payment failed - money wasn't taken");
            });
    },

    onError: function (err: any) {
    },

    onCancel: function (err: any) {
    },

    onDisplay: () => $paypalProductPreloader.hide()

}, '#paypal-button');