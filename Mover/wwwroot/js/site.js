// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function BlockWindow(message) {
    $(".block-window").css("display", "block");
    if (message === "" || message == undefined)
        message = "Loading...";
    $(".spinner-message").html(message);
}

function UnBlockWindow() {
    $(".block-window").css("display", "none");
}


function ShowNotificationMessageWithReload(Mes) {
    if (Mes.Message.Status != undefined)
        ShowNotificationWithReload(Mes.Message.Status, Mes.Message.Message);
    else
        ShowNotificationWithReload(Mes.Status, Mes.Message);
}
function ShowNotification(status, message) {
    if (status == "info")
        toastr.info(message);
    if (status == "error")
        toastr.error(message);
    if (status == "success")
        toastr.success(message);

}
function ShowNotificationMessage(Mes) {
    if (Mes.Message != undefined && Mes.Message.Status != undefined)
        ShowNotification(Mes.Message.Status, Mes.Message.Message);
    else
        ShowNotification(Mes.Status, Mes.Message);
}
function ShowNotificationWithReload(status, message) {
    if (status == "info")
        toastr.info('Message', message, {
            timeOut: 1000,
            fadeOut: 1000,
            onHidden: function () {
                window.location.reload();
            }
        });
    if (status == "error")
        toastr.error('Message', message, {
            timeOut: 1000,
            fadeOut: 1000,
            onHidden: function () {
                window.location.reload();
            }
        });
    if (status == "success")
        toastr.success('Message', message, {
            timeOut: 1000,
            fadeOut: 1000,
            onHidden: function () {
                window.location.reload();
            }
        });

}


function SuccessToast(message) {
    Swal.fire({
        icon: 'success',
        html: message,
        toast: true,
        iconColor: 'green',
        position: 'top-right',
        showConfirmButton: false,
        timer: 3000
    });
}

function FailureToast(message) {
    console.log(message)
    Swal.fire({
        icon: 'error',
        html: message,
        toast: true,
        iconColor: 'red',
        position: 'top-right',
        showConfirmButton: false,
        timer: 3000
       
    });
}

function InfoToast(message) {
    Swal.fire({
        icon: 'info',
        html: message,
        toast: true,
        position: 'top-right',
        showConfirmButton: false,
        timer: 3000
    });
}

function ShowDialog(title, text, status) {
    return Swal.fire({
        title: title,
        text: text,
        icon: status,
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No',
    });
}

function ShowDialogWithTextInput(title, text, status) {
    return Swal.fire({
        title: title,
        text: text,
        icon: status,
        input: 'textarea',
        inputPlaceholder: 'Type your message here...',
        inputAttributes: {
            'aria-label': 'Type your message here'
        },
        showCancelButton: true,
        confirmButtonText: 'Submit',
        cancelButtonText: 'Cancel',
    });
}
