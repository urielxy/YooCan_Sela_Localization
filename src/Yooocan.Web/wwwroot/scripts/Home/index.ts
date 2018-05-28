var $contentImFollowing = $('#content-im-following');
var url = $contentImFollowing.data('url');
$(".load-more").hide();
$(".load-more-preloader").show();
$contentImFollowing.load(url, () => {
    $(".load-more").show();
    $(".load-more-preloader").hide();
});
