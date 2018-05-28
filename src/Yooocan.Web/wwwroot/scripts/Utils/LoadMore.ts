export function initLoadMoreButton(buttonHostName: string, perPageCount = 12, maxInitialCount?: number, actualInitialCount?: number, rootSelector: string = "", useApproximatePageSize: boolean = false) {

    var currentPageIndex = 0;

    if (!maxInitialCount) {
        maxInitialCount = perPageCount;
    }
    var $feed = $(`${rootSelector} .feed`);
    var count = maxInitialCount;

    if (actualInitialCount === undefined) {
        actualInitialCount = $feed.find(".card").length;
    }

    if ((!useApproximatePageSize && actualInitialCount > 0 && actualInitialCount < count) || (useApproximatePageSize && actualInitialCount <= maxInitialCount / 2)) {
        $(`${rootSelector} .load-more`).hide();
        return;
    }

    $(`${rootSelector} .load-more`).click(e => {
        var $preloader = $(`${rootSelector} .load-more-preloader`);
        var $button = $(e.currentTarget);
        var $lastCard = $feed.find(".card").last();
        var baseUrl = $button.data('url') as string;
        var delimiter = baseUrl.indexOf("?") > -1 ? "&" : "?";
        var url = `${baseUrl}${delimiter}maxDate=${$lastCard.data("publish-date")}&lastId=${$lastCard.data("id")}&count=${perPageCount}&offset=${count}&page=${currentPageIndex + 1}`;

        $preloader.show();
        $button.hide();
        ga('send', 'event', buttonHostName, 'load more button click');
        $.get(url)
            .done(data => {
                $feed.append(data);
                var resultsCount = (data.match(/data-query-result/g) || []).length;
                if (resultsCount >= perPageCount || (useApproximatePageSize && resultsCount >= perPageCount / 2)) {
                    count += resultsCount;
                    $button.show();
                }
                currentPageIndex++;
            })
            .fail(() => {
                $button.hide();
            })
            .always(() => {
                $preloader.hide();
            });
    });
}