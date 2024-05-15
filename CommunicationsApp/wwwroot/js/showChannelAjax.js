const errorToast = Toastify({
    text: "Something went wrong, please try again in a moment",
    duration: 3000,
    close: true,
    gravity: "bottom",
    position: "right",
});

const domParser = new DOMParser();

// Show channel
const channelsSidebar = document.getElementById('channelsSidebar');
const messagesViewingDiv = document.getElementById('messagesViewing');

Array.from(channelsSidebar.children).forEach(channel => {
    channel.style.cursor = 'pointer';

    channel.addEventListener('click', () => {
        $.ajax({
            url: `channels/${channel.dataset.sidebarChannelId}`,
            type: 'GET',
            success: data => {
                channel.classList.remove('list-group-item-primary');

                messagesViewingDiv.innerHTML = data;
                addMessagingFunctionality(dropzone);
                addEditFunctionality();

                const messages = document.getElementById('channelMessages');
                messages.scrollTop = messages.scrollHeight;

                Array.from(messages.children)
                    .forEach(message => addDeleteFunctionality(message));

                addPaginationScrollFunctionality(messages);
            },
            error: xhr =>
                Toastify({
                    text: xhr.responseText,
                    duration: 3000,
                    close: true,
                    gravity: "bottom",
                    position: "right",
                }).showToast()
        });
    });
})

// Set up receiving / sending messages
connection.on('ReceiveMessage', (message, channelCode) => {
    const channelMessagesDiv = document.getElementById('channelMessages');
    const currOpenChannel = document.getElementsByClassName('chat-container')[0];

    // If the given chat is open, show message
    // Otherwise, highlight channel in the sidebar if there
    if (!currOpenChannel || currOpenChannel.dataset.openChatCode != channelCode) {
        if (sidebarChannel)
            sidebarChannel.classList.add('list-group-item-primary');
    }
    else
        appendMessagePartial(message, channelMessagesDiv);

    // Update sidebar with latest message
    updateSidebarChannelPreviewMessage(channelCode, message.id, message.user.userName, message.Text);
});

connection.on('DeleteMessage', (messageId, channelCode) => {
    // Delete message if chat is open
    const messageContainer = document.getElementById(`openMessage${messageId}`);

    if (messageContainer) {
        const textContainer = document.getElementById(`openMessageText${messageId}`);
        textContainer.innerHTML = '<span class="fst-italic">Message was deleted</span>';

        const optionsList = messageContainer.querySelector('[data-options-list]');

        if (optionsList)
            optionsList.remove();
    }

    // Delete message in preview
    updateSidebarChannelPreviewMessage(channelCode, messageId, None, None);
})

function addMessagingFunctionality() {
    const sendMessageForm = document.getElementById('messageSendForm');
    const messageInput = document.getElementById('messageInput'),
        messageMediaInput = document.getElementById('messageMediaUpload'),
        submitBtn = sendMessageForm.querySelector('button[type="submit"]');

    const spinnerIconString = '<div class="spinner-border" role="status">' +
        '<span class="visually-hidden">Loading...</span></div>',
        sendIconString = '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-send" viewBox="0 0 16 16">' +
            '< path d="M15.854.146a.5.5 0 0 1 .11.54l-5.819 14.547a.75.75 0 0 1-1.329.124l-3.178-4.995L.643 7.184a.75.75 0 0 1 .124-1.33L15.314.037a.5.5 0 0 1 .54.11ZM6.636 10.07l2.761 4.338L14.13 2.576zm6.787-8.201L1.591 6.602l4.339 2.76z" />' +
            '</svg>';

    sendMessageForm.addEventListener('submit', e => {
        console.log(messageMediaInput.files);
        e.preventDefault();
        const channelCode = document.getElementById('channelCode').value;

        messageData = new FormData();
        messageData.append('Message', messageInput.value);
        messageData.append('Media', []);

        //$.ajax({
        //    url: sendMessageForm.action,
        //    processData: false,
        //    contentType: false,
        //    type: 'POST',
        //    data: messageData,
        //    beforeSend: () => submitBtn.innerHTML = spinnerIconString,
        //    complete: () => submitBtn.innerHTML = sendIconString,
        //    success: newMessage => {
        //        // Send message to channel server
        //        connection.invoke("SendMessageToChannel", newMessage, channelCode);

        //        messageInput.value = '';
        //    },
        //    error: () => errorToast.showToast()
        //});
    });
}

function appendMessagePartial(messageData, chatBoxContainer, appendToEnd = true, isAsync = true) {
    $.ajax({
        async: isAsync,
        url: 'partialViews/messagePartial',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(messageData),
        success: messagePartial => {
            chatBoxContainer.insertAdjacentHTML(appendToEnd ? 'beforeend' : 'afterbegin', messagePartial);
            addDeleteFunctionality(document.getElementById(`openMessage${messageData.id}`));
        },
        error: () => errorToast.showToast()
    });
}

function addEditFunctionality() {
    const channelName = document.getElementById('channelName');
    const editChannelNameBtn = document.getElementById('editChannelNameBtn');

    if (channelName && editChannelNameBtn) {
        editChannelNameBtn.addEventListener('click', () => {
            channelName.contentEditable = true;
            channelName.focus();

            channelName.addEventListener('keydown', e => {
                if (e.code === 'Enter') {
                    e.preventDefault();

                    $.ajax({
                        url: `channels/${editChannelNameBtn.dataset.channelId}`,
                        contentType: 'application/json',
                        type: 'PATCH',
                        data: JSON.stringify(channelName.textContent),
                        success: () => {
                            channelName.contentEditable = false;
                            updateSidebarChannelName(editChannelNameBtn.dataset.channelCode, channelName.textContent);

                            Toastify({
                                text: "Channel name updated successfully",
                                duration: 3000,
                                close: true,
                                gravity: "bottom",
                                position: "right",
                            }).showToast();
                        },
                        error: () => errorToast.showToast()
                    })
                }
            });
        })
    }
}

function addPaginationScrollFunctionality(channelMessagesContainer) {
    channelMessagesContainer.addEventListener('scrollend', () => {
        /* Don't call for messages if user hasn't scrolled to the top
           or the cursor is at the end */
        if (channelMessagesContainer.scrollTop != 0
            || channelMessagesContainer.dataset.nextCursor == 0)
            return;

        $.ajax({
            url: `channels/${channelMessagesContainer.dataset.channelId}/messages?cursor=${channelMessagesContainer.dataset.nextCursor}&pageSize=12`,
            type: 'GET',
            success: paginatedMessages => {
                channelMessagesContainer.dataset.nextCursor = paginatedMessages.nextCursor;
                const messagesList = Array.from(paginatedMessages.values);

                if (messagesList.length > 0)
                    messagesList.forEach(messageData => appendMessagePartial(messageData, channelMessagesContainer, false, false));
                else
                    channelMessagesContainer.dataset.nextCursor = 0;
            },
            error: () => errorToast.showToast()
        });
    });
}

function addDeleteFunctionality(messageDivElement) {
    const deleteForm = document.querySelector(`[data-delete-form-id="${messageDivElement.dataset.id}"]`);

    if (!deleteForm)
        return;

    deleteForm.addEventListener('submit', e => {
        e.preventDefault();

        const currOpenChat = document.querySelector('[data-open-chat-code]');

        if (!currOpenChat)
            errorToast.showToast();

        $.ajax({
            url: deleteForm.action,
            type: 'DELETE',
            success: () => connection.invoke('DeleteMessageFromChannel', parseInt(messageDivElement.dataset.id), currOpenChat.dataset.openChatCode),
            error: () => errorToast.showToast()
        });
    })
}

function updateSidebarChannelName(channelCode, newName) {
    const sidebarChannel = document.querySelector(`[data-sidebar-channel-code='${channelCode}']`);

    if (sidebarChannel)
        document.getElementById(`sidebarChannelName${channelCode}`).textContent = newName;
}

function updateSidebarChannelPreviewMessage(channelCode, messageId, messageSenderUserName, messageText) {
    const sidebarChannel = document.querySelector(`[data-sidebar-channel-code='${channelCode}']`);

    if (sidebarChannel) {
        const latestMessageContainer = sidebarChannel.querySelector('[data-latest-message-for]');

        // If message is deleted, showcase
        // Otherwise, update text
        if (messageSenderUserName == None) {
            if (latestMessageContainer.id == `latestMessage${messageId}`)
                latestMessageContainer.textContent = `${latestMessageContainer.textContent.split(':')[0]}: Message was deleted`
        } else {
            latestMessageContainer.id = latestMessageContainer.replace(/[0-9]/g, '') + messageId;
            latestMessageContainer.textContent = `${messageSenderUserName}: ${messageText}`;
        }
    }
}