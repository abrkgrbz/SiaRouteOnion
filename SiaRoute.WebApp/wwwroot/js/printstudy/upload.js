
const id = "#kt_dropzonejs_example_2";
const dropzone = document.querySelector(id);

// set the preview element template
var previewNode = dropzone.querySelector(".dropzone-item");
previewNode.id = "";
var previewTemplate = previewNode.parentNode.innerHTML;
previewNode.parentNode.removeChild(previewNode);

var myDropzone = new Dropzone(id, {  
    url: "/UploadFile", 
    parallelUploads: 1,
    previewTemplate: previewTemplate,
    maxFilesize: 1,  
    autoQueue: false,  
    previewsContainer: id + " .dropzone-items",  
    clickable: id + " .dropzone-select", 
    acceptedFiles: ".txt"  
});

myDropzone.on("addedfile", function (file) {
    // Hookup the start button
    file.previewElement.querySelector(id + " .dropzone-start").onclick = function () { myDropzone.enqueueFile(file); };
    const dropzoneItems = dropzone.querySelectorAll('.dropzone-item');
    dropzoneItems.forEach(dropzoneItem => {
        dropzoneItem.style.display = '';
    });
    dropzone.querySelector('.dropzone-upload').style.display = "inline-block";
    dropzone.querySelector('.dropzone-remove-all').style.display = "inline-block";
});

// Update the total progress bar
myDropzone.on("totaluploadprogress", function (progress) {
    const progressBars = dropzone.querySelectorAll('.progress-bar');
    progressBars.forEach(progressBar => {
        progressBar.style.width = progress + "%";
    });
});

myDropzone.on("sending", function (file) {
    // Show the total progress bar when upload starts
    const progressBars = dropzone.querySelectorAll('.progress-bar');
    progressBars.forEach(progressBar => {
        progressBar.style.opacity = "1";
    });
    // And disable the start button
    file.previewElement.querySelector(id + " .dropzone-start").setAttribute("disabled", "disabled");
});

// Hide the total progress bar when nothing's uploading anymore
myDropzone.on("complete", function (file,progress) {
    const progressBars = dropzone.querySelectorAll('.dz-complete');
    if (file.status === Dropzone.SUCCESS) {
        Swal.fire({
            title: 'Yükleme Tamamlandı!',
            text: 'Tüm dosyalar başarıyla yüklendi.',
            icon: 'success',
            confirmButtonText: 'Tamam',
            customClass: {
                popup: 'minimalist-alert'
            }
        }).then(() => {
            myDropzone.removeAllFiles(true); // Dosyaları sil
        });
    } else if (file.status === Dropzone.ERROR) {
        let errorMessage = "Bir hata oluştu.";
        try {
            const responseObj = JSON.parse(file.xhr.response);
            if (responseObj && responseObj.Errors) {
                errorMessage = responseObj.Errors.join("\n");
            } else if (responseObj && responseObj.Message) {
                errorMessage = responseObj.Message;
            }
        } catch (e) {
            console.error("Response parsing error:", e);
        }
        Swal.fire({
            title: 'Hata!',
            text: errorMessage,
            icon: 'error',
            confirmButtonText: 'Tamam',
            customClass: {
                popup: 'minimalist-alert'
            }
        }).then(() => {
            myDropzone.removeAllFiles(true); // Dosyaları sil
        });
    }
    setTimeout(function () {
        progressBars.forEach(progressBar => {
            progressBar.querySelector('.progress-bar').style.opacity = "0";
            progressBar.querySelector('.progress').style.opacity = "0";
            progressBar.querySelector('.dropzone-start').style.opacity = "0";
        });
    }, 300);


});

// Setup the buttons for all transfers
dropzone.querySelector(".dropzone-upload").addEventListener('click', function () {
    myDropzone.enqueueFiles(myDropzone.getFilesWithStatus(Dropzone.ADDED));
});

// Setup the button for remove all files
dropzone.querySelector(".dropzone-remove-all").addEventListener('click', function () {
    dropzone.querySelector('.dropzone-upload').style.display = "none";
    dropzone.querySelector('.dropzone-remove-all').style.display = "none";
    myDropzone.removeAllFiles(true);
});

// On all files completed upload
myDropzone.on("queuecomplete", function (progress) { 
    const uploadIcons = dropzone.querySelectorAll('.dropzone-upload');
    uploadIcons.forEach(uploadIcon => {
        uploadIcon.style.display = "none";
    });
  
});

 

// On all files removed
myDropzone.on("removedfile", function (file) {
    if (myDropzone.files.length < 1) {
        dropzone.querySelector('.dropzone-upload').style.display = "none";
        dropzone.querySelector('.dropzone-remove-all').style.display = "none";
    }
});