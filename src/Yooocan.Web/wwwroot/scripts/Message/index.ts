import {isMobile, getUserAvatar, getFirstName, getLastName } from "../Shared/Layout";
var loadUrl = $('#preview').data('load-url');
var loading = false;
var $preloader = $('.preloader-wrapper')
var $conversation = $('#conversation');

// Currently the page isn't support on mobile.
if (isMobile())
    throw "not suppoted";

$('#preview ul').on('click','.collection-item', function() {
    const $this = $(this);
    if ($this.hasClass('active'))
        return false;

    if (loading)
        return false;
    $('#preview .collection-item').removeClass('active');
    $this.addClass('active');
    $preloader.addClass('active');
    $conversation.empty().load(loadUrl, { userId: $this.data('user-id') }, () => {
        $preloader = $preloader.removeClass('active');
        var $ul = $conversation.find('ul');
        $ul[0].scrollTop = $ul[0].scrollHeight;
        $('#message').focus();
    });
}).find('.collection-item:first').click();

$('#conversation').on('click', '#send', function () {
    var userId = $('#preview .collection-item.active').data('user-id');
});
$('#conversation').on('keydown', '#message', function (e) {
    if ((e.keyCode == 10 || e.keyCode == 13) && !e.shiftKey) {
        $(this).closest('form').submit();
        return false;
    }
});
$('#conversation').on('submit', '#conversation-form', function () {
    var message = $('#message').val().trim();
    if (message.length === 0)
        return false;

    var $newMessage = $(
        `<li class="collection-item avatar">
    <img src="${getUserAvatar()}" alt="Avatar" class="circle">
    <span class="author-name">${getFirstName()} ${getLastName()}</span> <span class="date">Now</span>
    <p>${message}</p>
</li>`);

    var $ul = $('#conversation ul');
    $ul.append($newMessage);
    $ul[0].scrollTop = $ul[0].scrollHeight;
    $(this).ajaxSubmit({
        error: (response) => {
            if (!navigator.onLine)
                alert('Please make sure you have a good internet connection and try again.');
            else {
                alert('Error has occured :( our engineers are on it!');
            }
            $newMessage.css('opacity', 0.5);
        }
    });

    $('#message').val('');
    return false;
});

$("#search-author").autocomplete({
    delay: 100,
    minLength: 2,
    source: function(request: any, response: any) {
        $.getJSON("/Message/SearchUsers", { q: request.term }, function(data) {
                response(data);
            })
            .fail(function() {
                response([]);
            });
    },
    select: function(event, ui) {
        event.preventDefault();
        $("#search-author").val(ui.item.label);
        const userId = ui.item.value;
        $('#preview .collection-item').removeClass('active');
        $conversation.empty().load(loadUrl, { userId: userId }, () => {
            $preloader = $preloader.removeClass('active');
            var $ul = $conversation.find('ul');
            $ul[0].scrollTop = $ul[0].scrollHeight;
            $('#message').focus();
        });
        var $li = $('#preview ul li').filter(function() {
            return $(this).data('user-id') === ui.item.value;
        });
        console.log($li);
        
        if (!$li.length) {
            var name = ui.item.label;
            var avatar = ui.item.avatar;
            $li = $(`<li class="collection-item avatar active" data-user-id="${userId}">
                <img src="${avatar || '/images/no-avatar.jpg'}" alt="Avatar" class="circle">
                <span class="author-name">${name}</span>
                <p></p>
            </li>`);
        }
        
        $('#preview ul').prepend($li);
    },
    focus: function (event, ui) {
        event.preventDefault();
        $("#search-author").val(ui.item.label);
    },
});
$('#search-author').data('uiAutocomplete')._renderItem = function(ul: HTMLElement, item: any) {
    console.log(ul);
    return $("<li class='valign-wrapper'>")
        .append(`<img style="padding:0;margin:3px 1em 3px .4em;width:42px;height:42px; border-radius:50%;" src='${item.avatar || '/images/no-avatar.jpg'}'>`)
        .append(`<span class="valign">${item.label}</span>`)
        .appendTo(ul);
};