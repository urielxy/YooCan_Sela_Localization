import ComponentForm from '../Utils/GoogleMapsComponentForm';
import * as PictureUploader from "../Utils/pictureUploader";
import * as layout from "../Shared/Layout"

//TODO: try to get updated typings for googlemaps, the current ones don't support the current TS version, PR was merged but not yet showing at the dt~ repo
declare const google: any;

if (layout.isMobile())
    $('#header-mobile-second-line').addClass('hide-on-small-and-down')

var $paragraphContainer = $('.paragraph-container').first().clone();
$paragraphContainer.find('input,textarea').val('');

var tipsRadio = (document.getElementById("tips-radio") as HTMLInputElement);
var productRadio = (document.getElementById("product-radio") as HTMLInputElement);

if (layout.getQueryParams()["template"] == "tips") {
    tipsRadio.checked = true;
}
else if(layout.getQueryParams()["template"] == "product") {
    productRadio.checked = true;
}

if ((tipsRadio && tipsRadio.checked) || (productRadio && productRadio.checked))
    toggleTemplate(true);

$('input[type=radio][name=Template]').change(toggleTemplate);

function toggleTemplate(initialization: boolean = false): void {
    if (!initialization)
        (<any>window).ga('send', 'event', 'Upload Story', 'Change story template');

    if (!tipsRadio.checked) {
        switchToStoryTemplate(productRadio.checked ? "PRODUCT RECOMMENDATION" : undefined);
    }
    else {
        switchToTipsTemplate();
    }

    var isProductInput = document.getElementById(productRadio.attributes["data-toggle-id"].value) as HTMLInputElement;
    if (productRadio.checked) {
        isProductInput.value = "true";        
    }
    else {
        isProductInput.value = "false";
    }
}

function switchToTipsTemplate(): void {
    $("label[for=Title]")[0].innerText = "YOUR ARTICLE TITLE *";
    $("#Title").prop("placeholder", "10 Tips for...");
    var currentNumberOfParagraphs = $(".paragraph-container").length;
    const wantedNumberOfTips = 10;
    for (var i = 0; i < wantedNumberOfTips - currentNumberOfParagraphs + 1; i++) {
        addParagraph();
    }
    initEditor();

    var $paragraphs = $(".paragraph-container");
    $paragraphs.each((i, paragraph) => {
        var $paragraph = $(paragraph);
        $paragraph.find(".input-field label")[0].innerText = i == 0 ? "INTRODUCTION TITLE (OPTIONAL)" : "TITLE";
        var $titleInput = $paragraph.find(".input-field input");
        $titleInput.prop("placeholder", i == 0 ? "" : `Tip #${i}: ...`);
        if (!$titleInput.prop("value")) {
            $titleInput.prop("value", i == 0 ? "" : `Tip #${i}: `);
        }
        $titleInput.prop("value", (_, val) => val.replace(/Tip #\d+:/i, `Tip #${i}:`));
        $paragraph.find(".editor-container label")[0].innerText = i == 0 ? "INTRODUCTION CONTENT (OPTIONAL)" : "CONTENT";
    });
}

function switchToStoryTemplate(type: string = "STORY"): void {
    $("label[for=Title]")[0].innerText = `YOUR ${type} TITLE *`;
    $("#Title").prop("placeholder", "");
    var $paragraphs = $(".paragraph-container");
    $paragraphs.each((i, paragraph) => {
        var $paragraph = $(paragraph);
        var $titleInput = $paragraph.find(".input-field input");
        $titleInput.prop("value", (_, val) => val.replace(/Tip #\d+:\s*/i, ""));

        var paragraphElement = $paragraph.find("textarea")[0] as HTMLInputElement;
        var tinymceObject = tinymce.get(paragraphElement.id);
        var contentValue = tinymceObject ? tinymceObject.getContent().trim() : paragraphElement.value;
        if (i > 0 && !$titleInput.prop("value") && !contentValue) {
            $paragraph.remove();
            return;
        }

        $paragraph.find(".input-field label")[0].innerText = "PARAGRAPH TITLE";
        $titleInput.prop("placeholder", "");
        $paragraph.find(".editor-container label")[0].innerText = "CONTENT";
    });
}

PictureUploader.initPictureUploader(new PictureUploader.PictureUploaderOptions("header-image-uploader", "preview-header-modal",
    1500, 500, 900, 300, false, undefined, "rgba(0, 0, 0, 0.4)", () => {
        var uriUploadUrl = $("#header-image-uploader").data('upload-url')
        let uri = $('#headerImageDataUri').val();
        $.post(uriUploadUrl, {
            dataUri: uri,
            containerName: 'images'
        }, (url: string) => {
            $('#headerImageDataUri').val('');
            $('#HeaderImageUrl').val(url);
        })
    }));


(<any>window).deferStyleSheet("https://fonts.googleapis.com/css?family=Roboto");

window.onbeforeunload = () => {
    return "";
};

function initEditor($container?: JQuery) {
    if (layout.isMobile())
        return;
    tinymce.init({
        selector: '.editor.notInit',
        plugins: "autoresize",
        autoresize_max_height: 500,
        menu: {

        },
        toolbar: [
            'bold italic underline | undo redo'
        ],
        statusbar: false,
        entity_encoding: 'raw',
        forced_root_block: false,
        content_css: '/css/story/editor.css',
        setup: function (editor: any) {
            editor.on('init', function () {
                $('.editor.notInit').removeClass('notInit');
                if ($container)
                    $container.find('.input-field input').focus();
            });
        }
    });
}

initEditor();
$('#Title').keyup(function () {
    $('.header .title').text(this.value);
}).change(function() {
    (<any>window).ga('send', 'event', 'Upload Story', 'Edit title', this.value);
});

var paragraphId = 0;
$('#add-paragraph').click(() => {
    (<any>window).ga('send', 'event', 'Upload Story', 'Add paragraph');
    var $newContainer = addParagraph();
    initEditor($newContainer);
    return false;
});

function addParagraph(): JQuery {
    var $newContainer = $paragraphContainer.clone();
    var inputId = 'id-' + (++paragraphId);
    $newContainer.find('.input-field input').attr('id', inputId);
    $newContainer.find('.input-field label').attr('for', inputId);
    $newContainer.find('textarea').attr('id', 'textarea' + paragraphId);

    $newContainer.hide().insertBefore($('#add-paragraph-container').last()).slideDown();
    return $newContainer;
}

$('.publish.btn, .preview.btn').click(function () {

    $('#paragraphs-container textarea').each(function (index) {
        if (!layout.isMobile()) {
            this.value = tinymce.get(this.id).getContent().trim();
        }
        this.name = `paragraphs[${index}].Text`;
    });

    $('#paragraphs-container .input-field input').each(function (index) {
        this.name = `paragraphs[${index}].Title`;
        this.value = this.value.trim();
    });
});

$('.publish.btn').click(() => {
    (window as any).ga('send', 'event', 'Upload Story', 'Click publish');
});

function receiveMessage(event: any) {
    switch (event.data.property) {
        case "name":
            {
                $('#author-name').text(event.data.value || 'No name set');
                break;
            }
        case "about":
            {
                $('#author-about-me').text(event.data.value || 'About me is empty');
                break;
            }
        case "avatar":
            {
                $('#author-avatar').prop('src', event.data.value || '/images/no-avatar.jpg');
                break;
            }
    }
}

window.addEventListener("message", receiveMessage, false);

function displayAlertToast(message: string) {
    layout.toastHideChat(message, 5000, "grey lighten-2 red-text text-darken-2");
}

$('#create-story-container').submit(function () {
    layout.toggleMobileHeader(true);

    if (!$('#Title').val()) {
        displayAlertToast('Please give your story a title');
        $('#Title').focus();
        return false;
    }

    if ($('#paragraphs-container .input-field input').filter(function () {
        return this.value;
    }).length === 0) {
        displayAlertToast('Please upload at least one paragraph to your story');
        var $firstParagraphTitle = $('#paragraphs-container .input-field input').first();
        if ($firstParagraphTitle.length > 0)
            $firstParagraphTitle.focus();
        else
            $("#add-paragraph").focus();
        return false;
    }

    if ($('#hiddenImages input').filter(function () {
        return this.value;
    }).length === 0) {
        displayAlertToast('Please upload at least one image to your story');
        $(window).scrollTop($("#dZUpload").offset().top);
        return false;
    }

    if ($('input[name="Categories"][type=checkbox]:checked').length === 0) {
        displayAlertToast('Please select the categories where you want your story to appear');
        $(window).scrollTop($("#categories-header").offset().top);
        return false;
    }


    if (!$('#PrimaryCategoryId').val()) {
        displayAlertToast('Please select the main category where you want your story to appear');
        $(window).scrollTop($("label[for='PrimaryCategoryId']").offset().top);
        return false;
    }

    if ($('input[name="Limitations"][type=checkbox]:checked').length === 0) {
        displayAlertToast('Please select disabilities or conditions related to your story');
        $(window).scrollTop($("#limitations-header").offset().top);
        return false;
    }

    if (!$('#PrimaryLimitationId').val()) {
        displayAlertToast('Please select one main disability or condition related to your story');
        $(window).scrollTop($("label[for='PrimaryLimitationId']").offset().top);
        return false;
    }

    $('.publish.btn').prop('disabled', true);

    window.onbeforeunload = null;
});

$('#paragraphs-container').on('click', '.delete-paragraph-btn', function () {
    (<any>window).ga('send', 'event', 'Upload Story', 'Delete paragraph');
    $(this).closest('.paragraph-container').slideUp(undefined, function () {
        $(this).remove();
    });
});

Dropzone.autoDiscover = false;
let $dZUpload = $("#dZUpload");
let uploadImagesUrl = $dZUpload.data('url');
$("#dZUpload")
    .dropzone({
        url: uploadImagesUrl,
        previewTemplate: $('#dropzone-preview-template').html(),
        addRemoveLinks: false,
        paramName: "images",
        maxFiles: 10,
        dictMaxFilesExceeded: "Maximum 10 images",
        maxFilesize: 5,
        removedfile: function (file) {
            var response = (<any>file).xhr && (<any>file).xhr.status === 200 && (<any>file).xhr.response;
            if (response) {
                $('.hidden-image')
                    .filter(function () { return this.value === eval(response)[0] })
                    .remove();
            }
            else {
                $('.hidden-image')
                    .filter(function () { return this.value.indexOf((<any>file).name) > -1 })
                    .remove();
            }
            $(document).find(file.previewElement).remove();
        },
        sending: function (file, width, formData) {
            formData.append("containerName", "images");
            (<any>window).ga('send', 'event', 'Upload Story', 'Upload image');
        },
        acceptedFiles: "image/*",
        success: function (file, imageUrl) {
            file.previewElement.classList.add("dz-success")
            $(file.previewElement).data('url', imageUrl.toString());

            if ($('.dz-preview').length === 0)
                file.previewElement.classList.add("main")

            $('#hiddenImages .hidden-image:last')
                .val(<string>imageUrl)
                .clone()
                .val('')
                .insertAfter($('#hiddenImages .hidden-image:last').last());
            return false;
        },
        error: function (file, response) {
            file.previewElement.classList.add("dz-error");
            $(file.previewElement).find('.dz-error-message').text(<string>response);
        }
    });

var dropzone = (<any>Dropzone).forElement("#dZUpload");

var images = $('#hiddenImages .hidden-image').filter(function () {
    return this.value;
}).map(function () {
    return this.value;
}).get();

for (var image of images) {
    var mockFile = { name: image, size: 123 };
    dropzone.emit("addedfile", mockFile);
    dropzone.emit("thumbnail", mockFile, `https://res.cloudinary.com/yooocan/image/fetch/f_auto,q_80,w_150,h_150,c_limit/${image}`);
    dropzone.emit("complete", mockFile);
}

$('.dz-preview').addClass('dz-success').eq(0).addClass('main');
var regex = /.*(https:\/\/yooocan.+)/;
$('#dZUpload .dz-preview .dz-image img').each(function() {
    $(this).closest('.dz-preview').data('url', regex.exec(this.src)[1]);
})

$('#dZUpload').on('click', '.set-as-main', function () {
    (<any>window).ga('send', 'event', 'Upload Story', 'Set main image');
    var $previewElement = $(this).closest('.dz-preview')
    var url = $previewElement.data('url');

    $('#hiddenImages input').filter(function () {
        return this.value === url
    }).prependTo('#hiddenImages');
    $previewElement.addClass('main').siblings().removeClass('main');

    return false;
});

$('#dZUpload').on('hover', '.dz-preview', function () {
    return false;
});

$('#youtube-link').change(function () {
    if (!this.value) {
        $('#YoutubeId').val('');
        return;
    }

    (<any>window).ga('send', 'event', 'Upload Story', 'Set YouTube video');
    var regExp = /^.*(youtu\.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?\s]*).*/;
    var match = this.value.match(regExp);
    if (match && match[2].length >= 5) {
        $('#YoutubeId').val(match[2]);;
    } else {
        $('#YoutubeId').val('');
        layout.toastHideChat('Incorrect YouTube URL', 3000);
    }
});

function initCheckboxList($checkboxList: JQuery, $primaryOption: JQuery, defaultPrimaryOptionText: string) {
    $checkboxList.change(function (e) {
        if ((<HTMLInputElement>e.target).checked) {
            (<any>window).ga('send', 'event', 'Upload Story', 'Select category', this.value);
        } else {
            (<any>window).ga('send', 'event', 'Upload Story', 'Unselect category', this.value);
        }

        var $options = $(this).find('input:checked').map(function () {
            return $('<option>', {
                value: this.value,
                selected: $primaryOption.val() === this.value
            }).text($(this).next().text());
        });
        var $defaultOption = $('<option>', { disabled: true, selected: !$primaryOption.val() }).text(defaultPrimaryOptionText);
        $primaryOption.empty();
        $primaryOption.append($defaultOption);

        for (var i = 0; i < $options.length; i++) {
            $primaryOption.append($options[i]);
        }

        // Recreate the select widget based on the new options.
        $primaryOption.material_select();
    });
}

initCheckboxList($('#categories-container'), $('#PrimaryCategoryId'), 'Choose Category');
initCheckboxList($('#limitations-container'), $('#PrimaryLimitationId'), 'Choose disability or condition');

$('.publish-strip .preview.btn').click(function () {
    (<any>window).ga('send', 'event', 'Upload Story', 'Preview story');
    var $createContainer = $('#create-story-container');
    //todo: add this plugin to ts
    var data = (<any>$createContainer).serializeObject();
    $createContainer.fadeOut();
    var url = this.href
    $('<div>').hide().attr('id', 'preview-container').insertAfter($createContainer).load(url, data, function (response: any, status: string) {
        if (status == "error") {
            layout.toastHideChat('Preview is unavailable now :(', 3000);
            $createContainer.fadeIn();
        }
    }).css({ width: '100vw', }).fadeIn();
    return false;
});

(<any>window).initAutocomplete = function () {
    //set your google maps parameters
    var latitude = $('#Latitude').val() || 37.0902,
        longitude = $('#Longitude').val() || -95.7129,
        map_zoom = 4;

    var componentForm = new ComponentForm();

    //define the basic color of your map, plus a value for saturation and brightness
    var mainColor = '#2d313f',
        saturationValue = -20,
        brightnessValue = 5;

    //we define here the style of the map
    var style = [
        {
            //set saturation for the labels on the map
            elementType: "labels",
            stylers: [
                { saturation: saturationValue }
            ]
        },
        { //poi stands for point of interest - don't show these lables on the map
            featureType: "poi",
            elementType: "labels",
            stylers: [
                { visibility: "off" }
            ]
        },
        {
            //don't show highways lables on the map
            featureType: 'road.highway',
            elementType: 'labels',
            stylers: [
                { visibility: "off" }
            ]
        },
        {
            //don't show local road lables on the map
            featureType: "road.local",
            elementType: "labels.icon",
            stylers: [
                { visibility: "off" }
            ]
        },
        {
            //don't show arterial road lables on the map
            featureType: "road.arterial",
            elementType: "labels.icon",
            stylers: [
                { visibility: "off" }
            ]
        },
        {
            //don't show road lables on the map
            featureType: "road",
            elementType: "geometry.stroke",
            stylers: [
                { visibility: "off" }
            ]
        },
        //style different elements on the map
        {
            featureType: "transit",
            elementType: "geometry.fill",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        },
        {
            featureType: "poi",
            elementType: "geometry.fill",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        },
        {
            featureType: "poi.government",
            elementType: "geometry.fill",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        },
        {
            featureType: "poi.sport_complex",
            elementType: "geometry.fill",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        },
        {
            featureType: "poi.attraction",
            elementType: "geometry.fill",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        },
        {
            featureType: "poi.business",
            elementType: "geometry.fill",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        },
        {
            featureType: "transit",
            elementType: "geometry.fill",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        },
        {
            featureType: "transit.station",
            elementType: "geometry.fill",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        },
        {
            featureType: "landscape",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]

        },
        {
            featureType: "road",
            elementType: "geometry.fill",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        },
        {
            featureType: "road.highway",
            elementType: "geometry.fill",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        },
        {
            featureType: "water",
            elementType: "geometry",
            stylers: [
                { hue: mainColor },
                { visibility: "on" },
                { lightness: brightnessValue },
                { saturation: saturationValue }
            ]
        }
    ];

    //set google map options
    var map_options = {
        center: new google.maps.LatLng(latitude, longitude),
        zoom: map_zoom,
        panControl: false,
        zoomControl: false,
        mapTypeControl: false,
        streetViewControl: false,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        scrollwheel: false,
        styles: style,
    }

    //inizialize the map
    var map = new google.maps.Map(document.getElementById('google-container'), map_options);

    google.maps.event.addDomListener(window, "resize", function () {
        var center = map.getCenter();
        google.maps.event.trigger(map, "resize");
        map.setCenter(center);
    });

    var input = <HTMLInputElement>document.getElementById('ActivityLocation');
    var autocomplete = new google.maps.places.Autocomplete(input);
    map.addListener('bounds_changed', () => {
        autocomplete.setBounds(map.getBounds());
    });

    var marker = new google.maps.Marker({
        map: map,
        position: new google.maps.LatLng(latitude, longitude),
        visible: !!$('#Latitude').val(),
        icon: {
            url: '/images/place-red.svg',
            size: new google.maps.Size(71, 71),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
        }
    });

    // Listen for the event fired when the user selects a prediction and retrieve
    // more details for that place.
    autocomplete.addListener('place_changed', function () {
        (<any>window).ga('send', 'event', 'Upload Story', 'Set location');

        marker.setVisible(false);
        var place = autocomplete.getPlace();
        if (!place.geometry) {
            // User entered the name of a Place that was not suggested and
            // pressed the Enter key, or the Place Details request failed.
            layout.toastHideChat("No details available on Google for: '" + place.name + "' please try to a different location.", 3000);
            return;
        }

        // If the place has a geometry, then present it on a map.
        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        } else {
            map.setCenter(place.geometry.location);
        }

        map.setZoom(4);

        marker.setPosition(place.geometry.location);
        marker.setVisible(true);

        $('#GooglePlaceId').val(place.place_id);
        $('#ActivityLocation').val(place.formatted_address);
        $('#Longitude').val(place.geometry.location.lng());
        $('#Latitude').val(place.geometry.location.lat());
        var $container = $('#cd-google-map');
        for (var i = 0; i < place.address_components.length; i++) {
            var addressType = place.address_components[i].types[0];
            if (componentForm[addressType]) {
                let val = (<any>place.address_components[i])[componentForm[addressType]];
                $container.find('input[data-type="' + addressType + '"]').val(val);
            }
        }
        var city = $('#City').val();
        let state = $('#State').val();
        var country = $('#Country').val();
        var address = `${city}, ${state} ${country}`;
        $('#address-footer').show();
        $('#address-text').text(address);
    });
}

layout.deferStyleSheet('/css/vectorMap.css');