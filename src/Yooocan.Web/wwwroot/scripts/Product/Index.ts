import { deferCss, isAuthenticated, registerModal } from "../Shared/Layout";

deferCss("/lib/flickity/dist/flickity.min.css");
function changeActiveImage() {
    const $this = $(this);
    $this.siblings('.active').removeClass('active');
    $this.addClass('active');
    if ($this.hasClass('video')) {
        $('#product-page .video-container').removeClass('hide');
        $('#product-page img.main').hide();
    } else {
        $('#product-page .video-container').addClass('hide');        
        var bigImage = $this.data("big-image-url");
        var $mainImage = $('#product-page img.main');
        $mainImage.prop('src', bigImage);
        if (!$this.data('loaded')) {
            $mainImage.fadeOut().on('load', () => {
                $mainImage.stop(true, true).show();
                $this.data('loaded', true);
            });
        }

    }
}

$('#product-page .img-container').click(changeActiveImage).hover(changeActiveImage);

($('.products-strip > .carousel-container,.benefits-strip > .carousel-container') as any).flickity({
    cellAlign: 'left',
    contain: true,
    percentPosition: false,
    groupCells: '100%',
    dragThreshold: 10
});

var $redirectWindow = $('#redirect-window');
$redirectWindow.find('.cancel').click(() => {
    $('#modal-container').closeModal();
    return false;
});

$redirectWindow.find('.continue').click(() => {
    var url = $('#product-page .redirect-button').attr('href');
    window.open(url, "vendor-page");
    return false;
});

$('#product-page .buy-subscribe-button').click(() => {
    ga('send', 'event', 'products', 'join yoocan button click', $("#product-page").data("product-name"));
    registerModal("product buy button");
    return false;
});

// no need to deduplicate because registerModal causes dom changes that prevent click after mousedown
$('#product-page .redirect-button').on("mousedown click", e => {
    ga('send', 'event', 'products', 'buy button click', $("#product-page").data("product-name"));    
    if (isAuthenticated()) {
        if ($redirectWindow.hasClass('hide')) {
            if ($redirectWindow.data("coupon-not-needed") !== "True") {
                $.post($redirectWindow.data('coupon-url'))
                    .done(code => $redirectWindow.find('.coupon').text(code))
                    .fail(() => {
                        $redirectWindow.find('.coupon').text('Contact support');
                        Materialize
                            .toast('yoocan seems to have run out of coupons! Please contact our support so we can help you.',
                            7000);
                    })
                    .always(() => ga('send', 'event', 'products', 'product coupon view', $("#product-page").data("product-name")));
            }
            $('#modal-container > div.modal-content').empty().append($redirectWindow.removeClass('hide'));
        }
        $('#spinner-container').hide();
        $('#modal-container').openModal({ opacity: 0.75 });
    }
    else {
        registerModal("product buy button");
    }
    e.stopPropagation();
    return false;
});

($('.images-carousel') as any).flickity({
    wrapAround: true,
    prevNextButtons: false,
    cellAlign: 'left'
});