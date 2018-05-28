$("select").material_select();
initPictureUploader(new PictureUploaderOptions("image-uploader",
    "preview-image-modal",
    500,
    500,
    200,
    200,
    false,
    undefined,
    "rgba(0, 0, 0, 0.4)",
    () => {
        //var uriUploadUrl = $("#header-image-uploader").data('upload-url');
        //let uri = $('#headerImageDataUri').val();
    }));