import * as PictureUploader from "../Utils/pictureUploader";

var textarea = document.getElementsByTagName("textarea")[0];
tinymce.init({
    selector: "textarea",
    plugins: [
        "advlist autolink lists link image charmap print preview anchor hr",
        "searchreplace visualblocks code fullscreen",
        "insertdatetime media table contextmenu paste"
    ],
    paste_as_text: true,
    toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
    relative_urls: false,
    content_css: `/css/layout.css?${new Date().getTime()},/css/story/index.css?${new Date().getTime()},https://fonts.googleapis.com/css?family=Montserrat:400%2C700|Source+Sans+Pro|Material+Icons`,
    body_class: "article-content",
    body_id: "html",
    height: 500,
    images_upload_url: textarea.attributes["data-upload-url"].value,
});

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