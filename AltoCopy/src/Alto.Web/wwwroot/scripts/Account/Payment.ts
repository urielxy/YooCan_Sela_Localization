declare var paypal;

var $paypalButton = $(".paypal-button-registration");
$paypalButton.each((_, button) => {
    renderPaypalButton($(button));
});

function renderPaypalButton(button: JQuery) {
    var $paypalRegistrationPreloader = button.siblings(".paypal-registration-preloader");
    var membershipType = button.data("membership-type");
    var membershipPrice = button.data("membership-price");

    paypal.Button.render({
        env: button.data("environment"),

        style: {
            size: 'medium',
            color: 'blue',
            shape: 'rect'
        },

        payment: function (resolve, reject) {

            $.post(button.data("create-url"))
                .done(function (data) {
                    ga("send", "pageview", `/Account/PaymentAuthorization/${membershipType}`);
                    resolve(data.paymentID);
                })
                .fail((_, textError) => {
                    reject("server error");
                });
        },

        onAuthorize: function (data) {
            $paypalRegistrationPreloader.show();
            $.post(button.data("execute-url"),
                { paymentID: data.paymentID, payerID: data.payerID })

                //TODO - add payment confirmation and error pages
                .done(function (data) {
                    showRegistrationConfirmation();
                    ga("send", "pageview", `/Account/PaymentExecuted/${membershipType}`);
                    fbq("track", "CompleteRegistration", { currency: "USD", value: membershipPrice });
                })
                .fail(function (err) {
                    alert("payment failed - money wasn't taken");
                })
                .always(() => {                    
                    $paypalRegistrationPreloader.hide();
                });
        },

        onError: function (err) {
        },

        onCancel: function (err) {
        },

        onDisplay: function () {
            //workaround Edge rendering bug
            if (navigator.userAgent.indexOf("Edge") >= 0) {
                setTimeout(() => $("#payment-options").hide().show(0), 1000);
            }
            $paypalRegistrationPreloader.hide();
        }


    }, `${button[0].id}`);
}