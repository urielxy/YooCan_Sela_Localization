var searchIframeId = "yoocan-service-providers";
var $searchIframe = $(`#${searchIframeId} iframe`);
$(".yoocan-search-link").click(loadIframe);
if (location.hash === "#" + searchIframeId) {
    loadIframe();
}

function loadIframe() {
    if (!$searchIframe.prop("src")) {
        $searchIframe.prop("src", $searchIframe.data("src"));
    }
}

$(window).on("message", e => {
    var data = (e.originalEvent as any).data;
    var $iframe = $(`#main iframe[src^="${data.location}"]`);
    $iframe.css("height", data.height + "px");
    $(`#${searchIframeId} .yoocan-search-preloader`).hide();
});

$(".tabs .tab a").click(e => {
    var searchType = $(e.target).attr("href");
    searchType = searchType.substr(1, searchType.length - 1);
    ga("send", "event", "search", "set search type", searchType);
})