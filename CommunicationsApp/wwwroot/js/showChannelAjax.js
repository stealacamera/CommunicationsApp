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
            url: `channels/${channel.dataset.channelId}`,
            type: 'GET',
            success: data => {
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

function addMessagingFunctionality() {
    const sendMessageForm = document.getElementById('messageSendForm');
    const messageInput = document.getElementById('messageInput'),
        submitBtn = sendMessageForm.querySelector('button[type="submit"]');

    const channelMessagesDiv = document.getElementById('channelMessages');

    const spinnerIconString = '<div class="spinner-border" role="status">' +
                                '<span class="visually-hidden">Loading...</span></div>',
        sendIconString = '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-send" viewBox="0 0 16 16">' +
            '< path d="M15.854.146a.5.5 0 0 1 .11.54l-5.819 14.547a.75.75 0 0 1-1.329.124l-3.178-4.995L.643 7.184a.75.75 0 0 1 .124-1.33L15.314.037a.5.5 0 0 1 .54.11ZM6.636 10.07l2.761 4.338L14.13 2.576zm6.787-8.201L1.591 6.602l4.339 2.76z" />' +
            '</svg>';

    // Set up connection
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatApp").build();
    submitBtn.disabled = true;

    connection.start().then(function () {
        submitBtn.disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });

    // Set up receiving / sending messages
    connection.on("ReceiveMessage", function (message) {
        appendMessagePartial(message, channelMessagesDiv)
    });

    sendMessageForm.addEventListener('submit', e => {
        e.preventDefault();

        $.ajax({
            url: sendMessageForm.action,
            contentType: "application/json",
            type: 'POST',
            data: JSON.stringify(messageInput.value),
            beforeSend: () => submitBtn.innerHTML = spinnerIconString,
            complete: () => submitBtn.innerHTML = sendIconString,
            success: newMessage => {
                // Send message to channel server
                connection.invoke("SendMessage", newMessage.User, newMessage).catch(function (err) {
                    return console.error(err.toString());
                });

                //// Add delete functionality
                //const newMessageElement = domParser.parseFromString(messageHtml, 'text/html').body.firstElementChild;
                //addDeleteFunctionality(newMessageElement);

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
        success: messagePartial => chatBoxContainer.insertAdjacentHTML('beforeend', messagePartial)
    });
}

function addDeleteFunctionality(messageDivElement) {
    const deleteForm = messageDivElement.querySelector('form');
    console.log(deleteForm);

    if (!deleteForm)
        return;

    deleteForm.addEventListener('submit', e => {
        e.preventDefault();

        $.ajax({
            url: deleteForm.action,
            type: 'DELETE',
            success: data => messageDivElement.replaceWith(data),
            error: () => errorToast.showToast()
        });
    })
}