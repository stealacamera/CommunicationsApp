const joinChannelForm = document.getElementById('joinChannelForm');

joinChannelForm.addEventListener('submit', e => {
    e.preventDefault();
    const code = joinChannelForm.querySelector('input[name="code"]').value;

    $.ajax({
        url: joinChannelForm.action + `?code=${code}`,
        type: 'POST',
        success: membership => connection.invoke('JoinChannel', membership.channel.code),
        error: xhr =>
            Toastify({
                text: xhr.status == 500 ? 'Something went wrong, please try again in a moment' : xhr.responseText,
                duration: 3000,
                close: true,
                gravity: "bottom",
                position: "right",
            }).showToast()
    });
});