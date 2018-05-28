import { deferCss, isMobile } from "../Shared/Layout";

deferCss("/lib/flickity/dist/flickity.min.css");

if (isMobile()) {
    ($('.products-strip > .carousel-container,.benefits-strip > .carousel-container') as any).flickity({
        cellAlign: 'left',
        contain: true,
        percentPosition: false,
        groupCells: '100%',
        dragThreshold: 10
    });
}