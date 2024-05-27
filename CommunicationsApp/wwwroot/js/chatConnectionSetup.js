const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatApp")
    .withAutomaticReconnect()
    .build();

connection.onreconnecting(() => {
    blockPage();
    alert('Connection lost. Trying to reconnect');
});

connection.onreconnected(() => {
    unblockPage();
    alert('Sucessfully reconnected');
})

connection.on('FunctionFailed', () => showToast('Something went wrong. Please referesh the page and try again'));

connection.on('ReceiveMessage', (message, channelCode) => {
    const channelMessagesDiv = document.getElementById('channelMessages');
    const currOpenChannel = document.getElementsByClassName('chat-container')[0];
    const sidebarChannel = document.querySelector(`[data-sidebar-channel-code='${channelCode}']`);

    // If the given chat is open, show message
    // Otherwise, highlight channel in the sidebar if there
    if (!currOpenChannel || currOpenChannel.dataset.openChatCode != channelCode) {
        if (sidebarChannel)
            sidebarChannel.classList.add('list-group-item-primary');
    }
    else
        appendMessagePartial(message, channelMessagesDiv);

    // Update sidebar with latest message
    updateSidebarChannelPreviewMessage(channelCode, message);
});

connection.on('DeleteMessage', (messageId) => {
    // Delete message if chat is open
    const newMessage = 'Message was deleted';
    const messageContainer = document.getElementById(`openMessage${messageId}`);
    const messageBubble = messageContainer.firstElementChild;

    if (messageContainer) {

        var optionsList = messageContainer.querySelector('[data-options-list]');
        var mediaDiv = document.getElementById(`messageMedia${messageId}`);

        if (optionsList)
            optionsList.remove();

        if (mediaDiv)
            mediaDiv.remove();

        var textContainer = document.getElementById(`openMessageText${messageId}`);

        if (!textContainer) {
            textContainer = document.createElement('p');
            messageBubble.lastElementChild.insertBefore(textContainer);
        }

        textContainer.innerHTML = `<span class="fst-italic">${newMessage}</span>`;
    }

    // Delete message if in preview
    deleteSidebarChannelPreviewMessage(messageId);
})

connection.on('JoinChannel', channel => {
    if (document.querySelector(`[data-sidebar-channel-code="${channel.channel.code}"]`))
        return;

    $.ajax({
        url: 'partialViews/channelSidebarPartial',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(channel),
        success: sidebarView => {
            const channelSidebar = document.getElementById('channelsSidebar');
            channelSidebar.insertAdjacentHTML('afterbegin', sidebarView);

            addShowFunctionality(channelSidebar.firstElementChild);
        },
        error: () => Toastify({
            text: 'Something went wrong. Please refresh the page if problem persists',
            duration: 3000,
            close: true,
            gravity: "bottom",
            position: "right",
        }).showToast()
    });
});

connection.on('LeaveChannel', channelCode => {
    // Remove open chat if it's the given channel
    var currentOpenChat = document.querySelector(`data-open-chat-code="${channelCode}"`);

    if (currentOpenChat)
        currentOpenChat.remove();

    // Remove channel from sidebar
    var channelSidebar = document.querySelector(`data-sidebar-channel-code="${channelCode}"`);

    if (channelSidebar)
        channelSidebar.remove();
});

connection.on('AddedToChannel', (channelCode, newMemberships) => {
    // If user doesn't have channel in sidebar, they're a new member
    if (!document.querySelector(`data-sidebar-channel-code="${channelCode}"`))
        connection.invoke('JoinChannel', channelCode);

    // Otherwise add members to list if channel is currently open
    else {
        if (!document.querySelector(`data-open-chat-code="${channelCode}"`))
            return;

        const membersList = document.getElementById('openChatMembersList');

        newMemberships.forEach(membership =>
            $.ajax({
                url: 'partialViews/openChannelMembersListItem',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(membership),
                success: listItemView => {
                    membersList.insertAdjacentHTML('beforeend', listItemView);

                    var listItem = document.getElementById(`openChannelMember${membership.member.id}`);
                    var listItemForm = listItem.getElementsByClassName('deleteChannelMemberForm')[0];

                    if (listItemForm)
                        listItemForm.addEventListener('submit', e => deleteMember(e, listItemForm.action, membership.channel.code));
                }
            })
        )
    }
});

connection.on('RemovedFromChannel', (channelCode, removedMemberIds) => {
    var currentUserId = document.getElementById('currentUserId').value;

    if (removedMemberIds.includes(currentUserId))
        connection.invoke('LeaveChannel', channelCode);
    else {
        if (!document.querySelector(`data-open-chat-code="${channelCode}"`))
            return;

        removedMemberIds.forEach(memberId =>
            document.getElementById(`openChannelMember${memberId}`).remove());

        const membersList = document.getElementById('openChatMembersList');

        if (membersList.childElementCount <= 2)
            document.getElementsByClassName('deleteChannelMemberForm').remove();
    }
});

async function start() {
    try {
        blockPage();
        await connection.start();
        unblockPage();
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

function blockPage() {
    contentBlock = document.createElement('div');
    contentBlock.id = 'blockPage';

    contentBlock.classList.add('d-flex');

    contentBlock.style.opacity = 0.5;
    contentBlock.style.background = '#6195d6';
    contentBlock.style.width = '100%';
    contentBlock.style.height = '100%';
    contentBlock.style.top = 0;
    contentBlock.style.left = 0;
    contentBlock.style.position = 'fixed';
    contentBlock.style.zIndex = 10;

    const spinnerSymbol = document.createElement('i');
    spinnerSymbol.classList.add('fa-solid', 'fa-spinner', 'fa-10x');

    const spinnerContainer = document.createElement('span');
    spinnerContainer.classList.add('position-absolute', 'top-50', 'start-50', 'translate-middle');
    spinnerContainer.appendChild(spinnerSymbol);

    contentBlock.appendChild(spinnerContainer);
    document.body.appendChild(contentBlock);
}

function unblockPage() {
    document.getElementById('blockPage').remove();
}

start();