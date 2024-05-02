const createChannelForm = document.getElementById('createChannelForm');
const channelSidebar = document.getElementById('channelsSidebar');

const createChannelModal = document.getElementById('createChannelModal');
const modal = bootstrap.Modal.getInstance(createChannelModal);

createChannelForm.addEventListener('submit', e => {
    e.preventDefault();
    
    const data = {
        ChannelName: createChannelForm.querySelector('input[name=name]').value,
        MemberIds: Array.from(createChannelForm.querySelectorAll('input[name=memberIds]:checked'))
                                               .map(checkbox => parseInt(checkbox.value))
    };

    $.ajax({
        url: createChannelForm.action,
        contentType: "application/json",
        type: 'POST',
        data: JSON.stringify(data),
        success: data => {
            channelSidebar.insertAdjacentHTML('afterbegin', data);
            modal.hide(); // TODO fix, bootstrap
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