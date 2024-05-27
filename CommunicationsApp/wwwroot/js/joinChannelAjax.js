const joinChannelForm = document.getElementById('joinChannelForm');

joinChannelForm.addEventListener('submit', e => {
    e.preventDefault();
    const code = joinChannelForm.querySelector('input[name="code"]').value.trim();

    if (!code) {
        showToast("Invalid code");
        return;
    }

    $.ajax({
        url: joinChannelForm.action + `?code=${encodeURIComponent(code)}`,
        type: 'POST',
        success: membership => connection.invoke('AddMemberToChannel', membership[0]),
        error: xhr =>
            xhr.status === 500 ?
                showToast('Something went wrong, please try again in a moment') :
                showErrorsToast(xhr.responseJSON)
    });
});