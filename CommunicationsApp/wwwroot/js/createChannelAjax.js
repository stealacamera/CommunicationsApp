const createChannelForm = document.getElementById('createChannelForm'),
      channelSidebar = document.getElementById('channelsSidebar');
const createChannelModal = document.getElementById('createChannelModal'),
      modal = bootstrap.Modal.getInstance(createChannelModal);

const memberSearchInput = document.getElementById('memberSearchInput'),
      membersSearchResult = document.getElementById('membersSearchResult');

addUserQueryFunctionality(memberSearchInput, membersSearchResult);

createChannelForm.addEventListener('submit', e => {
    e.preventDefault();

    const data = {
        ChannelName: createChannelForm.querySelector('input[name=newChannelName]').value.trim(),
        MemberIds: Array.from(createChannelForm.querySelectorAll('input[name=memberIds]:checked'))
            .map(checkbox => parseInt(checkbox.value))
    };

    $.ajax({
        url: createChannelForm.action,
        contentType: "application/json",
        type: 'POST',
        data: JSON.stringify(data),
        success: newChannel => connection.invoke('CreateChannel', newChannel.channel.code),
        error: xhr => showToast(
            xhr.status === 500 ?
                'Something went wrong, try again later' :
                'A channel must have a name and at least one member')
    });
})