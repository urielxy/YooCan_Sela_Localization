import * as LoadMore from "../Utils/LoadMore";
import "../../../node_modules/jquery.counterup/jquery.counterup.min.js";
import "../../../node_modules/waypoints/lib/jquery.waypoints.min.js";
import { isMobile, getProductCardAnalyticsHandler } from "../Shared/Layout";

($('.partners') as any).removeClass('not-init').flickity({
    pageDots: false,
    contain: true,
    percentPosition: false,
    groupCells: 2,
    dragThreshold: 10,
    freeScroll: true,
    wrapAround: true
}).flickity('playPlayer');

var $eventsStrip = ($('.events-strip') as any).removeClass('not-init').flickity({
    pageDots: false,
    contain: true,
    percentPosition: false,
    wrapAround: true,
    autoPlay: 4000
});

$('.events-strip .carousel-cell:first-child img').on('load', () => {
    $eventsStrip.flickity('resize');
});

LoadMore.initLoadMoreButton("homepage", 12);

($('#mainBody .social-numbers .counter') as any).counterUp({ delay: 25, time: 2000, offset: 95});

$(".feed").on("click", ".yoocan-shirt-card", function () {
    ga("send", "event", "homepage", "buy yoocan shirt click")
});

$(".feed").on("click", ".product-card-link", getProductCardAnalyticsHandler("homepage"));

setInterval(() => {
    var shopCategories = $(".shop-category.card");
    if (shopCategories.length > 0) {
        shopCategories.each((i, c) => {
            if (!c.classList.contains("hide")) {
                c.classList.add("hide");
                shopCategories[(i + 1) % shopCategories.length].classList.remove("hide");
                return false;
            }
        });
    }
}, 2000)

