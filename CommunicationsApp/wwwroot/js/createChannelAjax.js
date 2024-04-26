const createChannelForm = document.getElementById('createChannelForm');

createChannelForm.addEventListener('submit', e => {
    e.preventDefault();
    
    const data = {
        ChannelName: createChannelForm.querySelector('input[name=name]').value,
        MemberIds: Array.from(createChannelForm.querySelectorAll('input[name=memberIds]:checked'))
                                               .map(checkbox => parseInt(checkbox.value))
    };

    //send aja
    $.ajax({
        url: createChannelForm.action,
        contentType: "application/json",
        type: 'POST',
        data: JSON.stringify(data),
        success: data => {
            console.log('yea');
        }
        ,
        error: xhr =>
            Toastify({
                text: xhr.responseText,
                duration: 3000,
                close: true,
                gravity: "bottom",
                position: "right",
            }).showToast()
    });
})