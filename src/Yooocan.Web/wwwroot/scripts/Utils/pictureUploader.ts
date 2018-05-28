import "../../lib/croppie/croppie"

export class PictureUploaderOptions {
    constructor(public rootElementId: string,
        public modalId: string,
        public cropWidth: number,
        public cropHeight: number,
        public displayCropWidth: number = cropWidth,
        public displayCropHeight: number = cropHeight,
        public isCircle: boolean = false,
        public placeHolderUrl: string = "",
        public overlayColor = "rgba(74, 73, 74, 0.78)",
        public successCallback?: () => void) {

        var aspectRatio = displayCropWidth / displayCropHeight;
        var widthShrinkRatio = displayCropWidth / (window.screen.width - 170);
        var heightShrinkRation = displayCropHeight / (window.screen.height - 170);
        var shrinkRatio = Math.max(widthShrinkRatio, heightShrinkRation);

        if (shrinkRatio > 1) {
            this.displayCropWidth = displayCropWidth / shrinkRatio;
            this.displayCropHeight = displayCropHeight / shrinkRatio;
        }
    }
}

export function initPictureUploader(options: PictureUploaderOptions) {

    var $root = $(document.getElementById(options.rootElementId));
    var cropWidth = options.cropWidth;
    var cropHeight = options.cropHeight;
    var displayCropWidth = options.displayCropWidth;
    var displayCropHeight = options.displayCropHeight;
    var isCircle = options.isCircle;
    var modalId = options.modalId;
    var placeholderUrl = options.placeHolderUrl;
    var overlayColor = options.overlayColor;

    var $previewModal = $(document.getElementById(modalId));
    var $profilePicPreview = $previewModal.find('.crop-preview')
        .croppie({
            viewport: {
                width: displayCropWidth,
                height: displayCropHeight,
                type: isCircle ? 'circle' : 'square'
            },
            boundary: {
                width: displayCropWidth + 100,
                height: displayCropHeight + 100
            }
        });

    var cancelPicture = function () {
        $previewModal.closeModal();
        $root.find('.picture-upload').val("");
    };

    var setPreviewPicture = function (url: string) {
        $root.find('.picture-preview')[0].style.background = "linear-gradient(" + overlayColor + ", " + overlayColor + ")" +
            (url ? ",url('" + url + "') center / cover" : "");
    }

    var intialImage = $root.data("initial-image");
    if (intialImage) {
        setPreviewPicture(intialImage);
    }

    var removePicture = function () {
        $root.find('.picture-upload').val("");
        $root.find('.picture-data-uri').val("");
        setPreviewPicture(placeholderUrl);
    };

    var readFile = function (input: HTMLInputElement) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (//why event is any - https://github.com/Microsoft/TypeScript/issues/299
                event: any) {
                $previewModal
                    .openModal({
                        complete: cancelPicture
                    });

                $profilePicPreview.croppie('bind',
                    {
                        url: event.target.result
                    });
            }
            reader.readAsDataURL(input.files[0]);
        }
    };

    $root.find('.upload-picture-button').click(
        function () {
            $root.find('.picture-upload').click();
        });

    $root.find('.remove-picture-button').click(
        function () {
            removePicture();
        });

    $root.find('.picture-upload').on('change', function () { readFile(this); });

    $previewModal.find('.take-picture').click(function () {
        $profilePicPreview.croppie('result',
            {
                size: {
                    width: cropWidth,
                    height: cropHeight
                },
                quality: 0.8,
                format: "jpeg"
            })
            .then(function (data: string) {
                $root.find('.picture-data-uri').val(data);
                setPreviewPicture(data);
                $previewModal.closeModal();
                if (options.successCallback)
                    options.successCallback();
            });
        return false;
    });

    $previewModal.find('.close-modal').click(cancelPicture);
};