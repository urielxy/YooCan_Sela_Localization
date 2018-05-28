import { throttle } from 'lodash-es'

var $root = $("#feed-items");
var originalFooterHeight = $root.find(".footer").height();
var categoryId = $root.data("category-id");

var currentPageIndexOnTop = initCurrentPageIndexOnTop();
var currentDirection = Hammer.DIRECTION_NONE;
var cardHeight = $root.height();
var cardsCount = getCardCount();
var isLoadingInProgress = false;

function initCurrentPageIndexOnTop(): number {
    const count = $(".page[style*='180deg']").length;
    return count - 1;
}

function takeFullHeight() {
    //making the height divisible by 4 helps solving weird 1 pixel border bug on mobile chrome
    cardHeight = Math.floor(window.innerHeight / 4) * 4 - originalFooterHeight;

    var footerHeight = window.innerHeight - cardHeight;
    $("html,body,#feed-items").height(window.innerHeight);
    $(".page .card").height(cardHeight);
    $(".pages").height(cardHeight);
    $(".front .inner").css("top", cardHeight / -2);
    $root.find(".footer").height(footerHeight);
}

takeFullHeight();
$(window).resize(takeFullHeight);

var hammer = new Hammer($root[0], {
    recognizers: [
        [Hammer.Swipe, { direction: Hammer.DIRECTION_VERTICAL }],
        [Hammer.Pan, { direction: Hammer.DIRECTION_ALL }]
    ]
});
hammer.get("swipe").recognizeWith("pan");

hammer.on("panstart", ev => {
    if (currentDirection === Hammer.DIRECTION_NONE)
        currentDirection = ev.direction;
    onMove(ev);
});

hammer.on("panend", endGesture);

hammer.on("panmove", throttle((ev: HammerInput) => {
    onMove(ev);
}, 10));

hammer.on("swipe", ev => {
    var ratio = 1;

    //sometimes swipe direction is incorrect, trying to fix that
    if (ev.direction !== Hammer.DIRECTION_UP && ev.direction !== Hammer.DIRECTION_DOWN) {
        ev.direction = currentDirection;
    }

    if (currentDirection === Hammer.DIRECTION_NONE) {
        currentDirection = ev.direction;
    }
    else {
        ratio = currentDirection === ev.direction ? 1 : 0;
    }
    endGesture(ev, ratio);
});

$root.find(".pages").click((ev) => {
    ev.stopImmediatePropagation();
    var url = getPage(currentPageIndexOnTop).find(".back a").prop("href");
    location.href = url;
    return false;
});

function endGesture(ev: HammerInput, ratio: number = undefined) {
    if (currentDirection === Hammer.DIRECTION_NONE)
        return;

    if (ratio === undefined)
        ratio = onMove(ev);
   
    if (ratio >= 0.5 &&
        (currentPageIndexOnTop !== 0 || currentDirection === Hammer.DIRECTION_UP) &&
        (currentPageIndexOnTop < cardsCount - 1 || currentDirection === Hammer.DIRECTION_DOWN)) {
        flip(currentDirection, 1);
        ga('send', 'event', 'mobile category page', currentDirection === Hammer.DIRECTION_UP ? "flip up" : "flip down", categoryId);
        currentPageIndexOnTop += currentDirection === Hammer.DIRECTION_UP ? 1 : -1;

        updateQueryStringParam("card", currentPageIndexOnTop.toString());

        if (currentPageIndexOnTop + 5 >= cardsCount && !isLoadingInProgress) {
            loadMoreCards();
        }
        setVisiblePages();
    }
    else {
        flip(currentDirection, 0);
    }
    currentDirection = Hammer.DIRECTION_NONE;
}

function setVisiblePages() {
    var $pages = $root.find(".page");
    $pages.each((i, page) => {
        if (i === currentPageIndexOnTop) {            
        }            
        else if (i >= currentPageIndexOnTop - 2 && i <= currentPageIndexOnTop + 2) {
            if (page.classList.contains("hide"))
                page.classList.remove("hide");            
        }
        else {
            if(!page.classList.contains("hide"))
                page.classList.add("hide");
            
        }
    });    
}

function onMove(ev: HammerInput): number {
    var delta: number;

    if (currentDirection === Hammer.DIRECTION_NONE)
        return 0;

    var eventDirection = ev.direction;
    if (ev.direction !== Hammer.DIRECTION_UP && ev.direction !== Hammer.DIRECTION_DOWN) {
        if (ev.deltaY === 0) {
            return 0;
        }
        eventDirection = ev.deltaY > 0 ? Hammer.DIRECTION_DOWN : Hammer.DIRECTION_UP;
    }
    if (currentDirection !== Hammer.DIRECTION_UP && currentDirection !== Hammer.DIRECTION_DOWN) {
        currentDirection = eventDirection;
    }

    if (currentDirection === Hammer.DIRECTION_UP) {
        delta = ev.deltaY < 0 ? Math.abs(ev.deltaY) : 0;
    }
    else {
        delta = ev.deltaY > 0 ? ev.deltaY : 0;
    }

    var ratio = (delta / cardHeight) * 1.5;
    if (ratio > 1)
        ratio = 1;

    flip(currentDirection, ratio);

    return ratio;
}

function flip(direction: number, ratio: number): void {
    var topPage: JQuery, middlePage: JQuery, bottomPage: JQuery;
    var degrees = ratio * 180;

    if (direction === Hammer.DIRECTION_UP) {
        topPage = getPage(currentPageIndexOnTop);
        middlePage = getPage(currentPageIndexOnTop + 1);
        bottomPage = getPage(currentPageIndexOnTop + 2);
        middlePage.css("transform", `rotateX(${degrees}deg)`);
    }
    else {
        middlePage = getPage(currentPageIndexOnTop);
        topPage = getPage(currentPageIndexOnTop - 1);
        bottomPage = getPage(currentPageIndexOnTop + 1);
        middlePage.css("transform", `rotateX(${180 - degrees}deg)`);
    }
    
    if (ratio === 1 || ratio === 0) {
        middlePage.css("transition", "all 0.5s ease-out");
    } else {
        middlePage.css("transition", "none");
    }

    middlePage.css("z-index", Math.max(Number(topPage.css("z-index")) + 1 || 1, Number(bottomPage.css("z-index")) + 1 || 1));
}

function getPage(index: number): JQuery {
    return $($root.find(".page")[index]);
}

function getCardCount(): number {
    return $root.find(".card").length / 2;
}

function loadMoreCards() {
    isLoadingInProgress = true;
    const baseUrl = $root.data("load-more-url");
    const perPageCount = 10;
    const url = `${baseUrl}&count=${perPageCount}&offset=${cardsCount}`;
    const $preloader = $root.find(".preloader");
    const $errorMessage = $root.find(".error-message");
    ga('send', 'event', 'mobile category page', 'load more triggered', categoryId);
    $.get(url)
        .done(data => {
            $preloader.show();
            $errorMessage.hide();
            var $lastOldCard = getPage(cardsCount);
            var $preLoaderPage = getPage(cardsCount + 1);
            $preLoaderPage.remove();
            $root.find(".pages").append(data);
            var $firstNewCard = getPage(cardsCount + 1);
            $lastOldCard.remove(".back").append($firstNewCard.find(".back"));
            $firstNewCard.remove();
            takeFullHeight();

            var oldLowestZIndex = Number($lastOldCard.css("z-index"));
            var newHighestZIndex = Number(getPage(cardsCount + 1).css("z-index"));
            if (oldLowestZIndex < newHighestZIndex) {
                $root.find(`.page:nth-child(n + ${cardsCount + 2})`).css("z-index", i => oldLowestZIndex - i - 2);
            }

            cardsCount = getCardCount();            
        })
        .fail(() => $errorMessage.show())
        .always(() => {
            isLoadingInProgress = false;
            $preloader.hide();
        });
}

function updateQueryStringParam(key: string, value: string) {
    var baseUrl = [location.protocol, '//', location.host, location.pathname].join(''),
        urlQueryString = document.location.search,
        newParam = key + '=' + value,
        params = '?' + newParam;

    // If the "search" string exists, then build params from it
    if (urlQueryString) {
        var keyRegex = new RegExp('([\?&])' + key + '[^&]*');

        // If param exists already, update it
        if (urlQueryString.match(keyRegex) !== null) {
            params = urlQueryString.replace(keyRegex, "$1" + newParam);
        } else { // Otherwise, add it to end of query string
            params = urlQueryString + '&' + newParam;
        }
    }
    window.history.replaceState({}, "", baseUrl + params);
};