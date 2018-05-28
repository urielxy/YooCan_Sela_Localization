declare var YT: any;

export function initVideo() {
    var videoId = $(".youtube-bg-video").data("youtube-id");
    $(".youtube-bg-video").width($(".youtube-bg-video").width());
    var tag = document.createElement('script') as any;
    tag.src = 'https://www.youtube.com/player_api';
    var firstScriptTag = document.getElementsByTagName('script')[0];
    firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
    var player: any,
        playerDefaults = {
            autoplay: 1,
            mute: 1,
            autohide: 1,
            modestbranding: 1,
            rel: 0,
            showinfo: 0,
            controls: 0,
            disablekb: 1,
            enablejsapi: 0,
            iv_load_policy: 3
        };

    function onYouTubePlayerAPIReady() {
        player = new YT.Player('youtube-player', {
            events: {
                'onReady': onPlayerReady,
                'onStateChange': onPlayerStateChange
            },
            playerVars: playerDefaults
        });
    }

    (window as any).onYouTubePlayerAPIReady = onYouTubePlayerAPIReady;

    function onPlayerReady() {
        player.loadVideoById(videoId);
    }

    function onPlayerStateChange(e: any) {
        if (e.data === YT.PlayerState.ENDED) {
            player.playVideo();
        }
    }

    function vidRescale() {
       
        var width = $(".youtube-bg-video").width();
        var height = $(".youtube-bg-video").height();

        if (width / height > 16 / 9) {
            player.setSize(width, width / 16 * 9);
            $('#youtube-player').css({ 'left': '0' });
        } else {
            player.setSize(height / 9 * 16, height);
            $('#youtube-player').css({ 'left': -($('#youtube-player').outerWidth() - width) / 2 });
        }
    }
    
    $(window).on('resize', function () {
        $(".youtube-bg-video").width("unset");
        vidRescale();
    });

    $(window).on('load', function () {        
        vidRescale();
    });
}