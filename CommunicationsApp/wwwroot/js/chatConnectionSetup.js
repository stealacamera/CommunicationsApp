const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatApp")
    .withAutomaticReconnect()
    .build();

connection.onreconnecting(() => {
    // TODO block page from functioning

    alert('Connection lost. Trying to reconnect');
});

connection.onreconnected(() => {
    location.reload();
    alert('Sucessfully reconnected');
})

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

connection.on('DeleteMessage', (messageId, channelCode) => {
    // Delete message if chat is open
    const messageContainer = document.getElementById(`openMessage${messageId}`);

    if (messageContainer) {
        const textContainer = document.getElementById(`openMessageText${messageId}`);

        if (!textContainer)
            return;

        document.getElementById(`messageMedia${message.id}`).remove();
        textContainer.innerHTML = `<span class="fst-italic">Message was deleted</span>`;

        const optionsList = messageContainer.querySelector('[data-options-list]');

        if (optionsList)
            optionsList.remove();
    }

    // Delete message in preview
    updateSidebarChannelPreviewMessage(channelCode, message, true);
})

connection.on('JoinChannel', channel => {
    if (document.querySelector(`data-sidebar-channel-code="${channelCode}"`))
        return;

    $.ajax({
        url: 'partialViews/channelSidebarPartial',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(channel),
        success: sidebarView => {
            const channelSidebar = document.getElementById('channelsSidebar');
            channelSidebar.insertAdjacentHTML('afterbegin', sidebarView);
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
    }
});

async function start() {
    try {
        contentBlock = document.createElement('div');

        contentBlock.style.position = 'absolute';
        contentBlock.style.top = 0;
        contentBlock.style.left = 0;
        contentBlock.style.bottom = 0;
        contentBlock.style.right = 0;
        contentBlock.style.height = '100%';
        contentBlock.style.width = '100%';

        contentBlock.style.opacity = 0.7;
        contentBlock.style.color = 'white';

        document.body.appendChild(contentBlock);

        await connection.start();
        console.log("SignalR connection established.");

        contentBlock.remove();
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

start();