import { deferCss, registerModal, isAuthenticated } from "../Shared/Layout";

deferCss("/lib/flickity/dist/flickity.min.css");
function changeBenefitActiveImage() { 
    const $this = $(this);
    $this.siblings('.active').removeClass('active');
    $this.addClass('active');
    const src = $this.css('background-image').slice(4, -1).replace(/"|'/g, '');
    console.log(src);
    $('#product-page img.main').prop('src', src);
}
$('#benefit-page .img-container').click(changeBenefitActiveImage).hover(changeBenefitActiveImage);

($('.products-strip > .carousel-container,.benefits-strip > .carousel-container')as any).flickity({
    cellAlign: 'left',
    contain: true,
    percentPosition: false,
    groupCells: '100%',
    dragThreshold: 10
});

$(".read-more").click(() => {
    ga('send', 'event', 'benefits', 'read more button click', $(".read-more").data("benefit-name"));
    if (!isAuthenticated()) {
        registerModal("benefit page");
        return false;
    }   
})