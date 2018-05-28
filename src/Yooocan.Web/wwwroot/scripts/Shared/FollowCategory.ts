import { isAuthenticated, registerModal} from "./Layout";

$('.follow-category-form,.unfollow-category-form').submit(function () {
    if (!isAuthenticated()) {
        registerModal.call(this);
        return false;
    }
    var id = $('#Id').val();
    var action = $(this).data('action');
    (<any>window).ga('set', 'dimension1', id);
    (<any>window).ga('send', 'event', 'Category', action, id);

    var $container = $(this).parent();
    var $btn = $(this).find('.btn');
    $btn.prop('disabled', true);
    $(this).ajaxSubmit({
        error: (response) => {
            if (!navigator.onLine)
                alert('Please make sure you have a good internet connection and try again.');
            else {
                alert('Error has occured :( our engineers are on it!');
            }
        },
        complete: () => {
            $btn.prop('disabled', false);
        },
        success: () => {
            $container.find('.follow-category-form, .unfollow-category-form').toggleClass('hide');
        }
    });

    return false;
});