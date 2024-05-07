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
                addMessagingFunctionality();

                const chatDOM = domParser.parseFromString(data, 'text/html').body.firstElementChild;
                const chatMessages = chatDOM.querySelector('#channelMessages');

                Array.from(chatMessages.children)
                    .forEach(message => addDeleteFunctionality(message));
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
    const sidebarChannel = document.querySelector(`[data-sidebar-channel-code='${channelCode}']`);

    // Update sidebar w/ latest message
    if (sidebarChannel) {
        const latestMessageContainer = sidebarChannel.querySelector('[data-latest-message-for]');
        latestMessageContainer.textContent = `${message.user.userName}: ${message.text}`;
    }

    // If the given chat is open, show message
    // Otherwise, highlight channel in the sidebar if there
    if (!currOpenChannel || currOpenChannel.dataset.openChatCode != channelCode) {
        if (sidebarChannel)
            sidebarChannel.classList.add('list-group-item-primary');
    }
    else
        appendMessagePartial(message, channelMessagesDiv)
});

connection.on('DeleteMessage', messageId => {
    const messageContainer = document.getElementById(`openMessage${messageId}`);

    if (messageContainer) {
        const textContainer = messageDivElement.querySelector('[data-message-text]');
        textContainer.innerHTML = '<span class="fst-italic">Message was deleted</span>';

        const optionsList = messageContainer.querySelector('[data-options-list]');

        if (optionsList)
            optionsList.remove();
    }
})

function addMessagingFunctionality() {
    const sendMessageForm = document.getElementById('messageSendForm');
    const messageInput = document.getElementById('messageInput'),
        submitBtn = sendMessageForm.querySelector('button[type="submit"]');


    const spinnerIconString = '<div class="spinner-border" role="status">' +
        '<span class="visually-hidden">Loading...</span></div>',
        sendIconString = '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-send" viewBox="0 0 16 16">' +
            '< path d="M15.854.146a.5.5 0 0 1 .11.54l-5.819 14.547a.75.75 0 0 1-1.329.124l-3.178-4.995L.643 7.184a.75.75 0 0 1 .124-1.33L15.314.037a.5.5 0 0 1 .54.11ZM6.636 10.07l2.761 4.338L14.13 2.576zm6.787-8.201L1.591 6.602l4.339 2.76z" />' +
            '</svg>';

    sendMessageForm.addEventListener('submit', e => {
        e.preventDefault();
        const channelCode = document.getElementById('channelCode').value;

        $.ajax({
            url: sendMessageForm.action,
            contentType: "application/json",
            type: 'POST',
            data: JSON.stringify(messageInput.value),
            beforeSend: () => submitBtn.innerHTML = spinnerIconString,
            complete: () => submitBtn.innerHTML = sendIconString,
            success: newMessage => {
                // Send message to channel server
                connection.invoke("SendMessageToChannel", newMessage, channelCode);

                // Add delete functionality
                const newMessageElement = domParser.parseFromString(messageHtml, 'text/html').body.firstElementChild;
                addDeleteFunctionality(newMessageElement);

                messageInput.value = '';
            },
            error: () => errorToast.showToast()
        });
    });
}

function appendMessagePartial(messageData, chatBoxContainer) {
    $.ajax({
        url: 'partialViews/messagePartial',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(messageData),
        success: messagePartial => chatBoxContainer.insertAdjacentHTML('beforeend', messagePartial),
        error: () => errorToast.showToast()
    });
}

function addDeleteFunctionality(messageDivElement) {
    const deleteForm = messageDivElement.querySelector('form[data-delete-form]');

    if (!deleteForm)
        return;

    deleteForm.addEventListener('submit', e => {
        e.preventDefault();

        $.ajax({
            url: deleteForm.action,
            type: 'DELETE',
            success: () => connection.invoke('DeleteMessageFromChannel', messageDivElement.dataset.id),
            error: () => errorToast.showToast()
        });
    })
}