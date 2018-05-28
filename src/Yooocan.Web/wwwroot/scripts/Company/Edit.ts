import { initPictureUploader, PictureUploaderOptions } from "../Utils/pictureUploader";
import { markRequiredFields } from "../Shared/Layout";

initPictureUploader(new PictureUploaderOptions("image-uploader",
    "preview-image-modal",
    100,
    100,
    undefined,
    undefined,
    false,
    undefined,
    "rgba(0, 0, 0, 0.4)",
    () => {
        //var uriUploadUrl = $("#header-image-uploader").data('upload-url');
        //let uri = $('#headerImageDataUri').val();
    }));

markRequiredFields("#company-form");