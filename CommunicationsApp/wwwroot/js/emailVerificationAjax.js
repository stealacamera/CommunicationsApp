const resendEmailVerificationForm = document.getElementById('resendEmailVerificationForm');

resendEmailVerificationForm.addEventListener('submit', e => {
    e.preventDefault();

    $.ajax({
        url: resendEmailVerificationForm.action,
        contentType: "application/json",
        type: 'POST',
        data: JSON.stringify({ email: resendEmailVerificationForm.elements['email'].value }),
        success: () =>
            Toastify({
                text: "Email was sent successfully",
                duration: 3000,
                close: true,
                gravity: "bottom",
                position: "right",
            }).showToast()
        ,
        error: () =>
            Toastify({
                text: "Something went wrong. Please make sure that you inputted the email you signed up with",
                duration: 3000,
                close: true,
                gravity: "bottom",
                position: "right",
            }).showToast()
    });
});