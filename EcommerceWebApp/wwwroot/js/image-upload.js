// wwwroot/js/product-image-upload.js

function initProductImageUpload(uploadEndpoint, baseUrl) {
    $(document).ready(function () {
        $("#Photo").on("input", function () {
            $("#PhotoPreview").attr("src", $(this).val());
        });

        $("#OtherPhotos").on("input", function () {
            $("#OtherPhotosPreview").attr("src", $(this).val());
        });

        $("#photoFileInput").on("change", function () {
            var file = this.files[0];
            if (!file) return;

            uploadFile(file, function (url) {
                $("#Photo").val(url);
                $("#PhotoPreview").attr("src", baseUrl + "/" + url);
                console.log("ImgUrl:", url);
            });
        });

        $("#otherPhotosInput").on("change", function () {
            var files = this.files;
            uploadOtherPhotos(files, function (urls) {
                $("#OtherPhotos").val(urls.join(";"));
                $("#OtherPhotosPreview").attr("src", baseUrl + "/" + urls[0]);
            });
        });

        $("form").on("submit", function (e) {
            e.preventDefault();

            var form = this;
            var photoFile = $("#photoFileInput")[0].files[0];
            var otherFiles = $("#otherPhotosInput")[0].files;

            if (photoFile) {
                uploadFile(photoFile, function (photoUrl) {
                    $("#Photo").val(photoUrl);
                    $("#PhotoPreview").attr("src", baseUrl + "/" + photoUrl);

                    uploadOtherPhotos(otherFiles, function (urls) {
                        $("#OtherPhotos").val(urls.join(";"));
                        $(form).off("submit").submit();
                    });
                });
            } else {
                uploadOtherPhotos(otherFiles, function (urls) {
                    $("#OtherPhotos").val(urls.join(";"));
                    form.submit();
                });
            }
        });
    });

    function uploadFile(file, successCallback, errorCallback) {
        var formData = new FormData();
        formData.append("file", file);

        $.ajax({
            url: uploadEndpoint,
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (url) {
                successCallback(url);
            },
            error: function (xhr) {
                if (errorCallback) errorCallback(xhr.responseText);
                else alert("Upload failed: " + xhr.responseText);
            }
        });
    }

    function uploadOtherPhotos(files, callback) {
        var urls = [];
        var index = 0;

        function next() {
            if (index >= files.length) {
                callback(urls);
                return;
            }

            uploadFile(files[index], function (url) {
                urls.push(url);
                index++;
                next();
            });
        }

        if (files.length > 0) next();
        else callback([]);
    }
}
