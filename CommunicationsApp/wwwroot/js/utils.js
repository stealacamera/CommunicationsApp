function showToast(errorMessage) {
    Toastify({
        text: errorMessage,
        duration: 3000,
        close: true,
        gravity: "bottom",
        position: "right",
    }).showToast()
}

function showErrorsToast(errors) {
    Array.from(errors).forEach(error => showToast(error.reasons.join('\r\n')));
}