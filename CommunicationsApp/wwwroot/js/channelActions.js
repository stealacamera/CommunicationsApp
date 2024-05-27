function addShowFunctionality(sidebarChannel) {
    sidebarChannel.style.cursor = 'pointer';

    sidebarChannel.addEventListener('click', () => {
        $.ajax({
            url: `channels/${sidebarChannel.dataset.sidebarChannelId}`,
            type: 'GET',
            success: data => {
                sidebarChannel.classList.remove('list-group-item-primary');

                // Add base functionalities for channel
                messagesViewingDiv.innerHTML = data;
                addMessagingFunctionality();
                addEditChannelFunctionality();

                // Add member-related functionalities
                addDeleteMemberFunctionality(sidebarChannel.dataset.SidebarChannelCode);
                //addAddNewMembersFunctionality();

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
}

//function addAddNewMembersFunctionality() {
//    const newMemberSeachInput = document.getElementById('newMemberSearchInput'),
//        newMembersSearchResult = document.getElementById('newMembersSearchResult');

//    addUserQueryFunctionality(newMemberSeachInput, newMembersSearchResult);

//    const newMembersForm = document.getElementById('addNewMembersForm');

//    newMembersForm.addEventListener('submit', e => {
//        e.preventDefault();

//        const data = Array.from(newMembersForm.querySelectorAll('input[name=memberIds]:checked'))
//                          .map(checkbox => parseInt(checkbox.value));

//        $.ajax({
//            url: newMembersForm.action,
//            contentType: "application/json",
//            type: 'POST',
//            data: JSON.stringify(data),
//            success: newMembersData => connection.invoke('AddMembersToChannel', newMembersData),
//            error: xhr =>
//                Toastify({
//                    text: xhr.responseText,
//                    duration: 3000,
//                    close: true,
//                    gravity: "bottom",
//                    position: "right",
//                }).showToast()
//        });
//    })
//}

function addDeleteMemberFunctionality(channelCode) {
    var deleteMemberForms = document.getElementsByClassName('deleteChannelMemberForm');

    Array.from(deleteMemberForms).forEach(form =>
        form.addEventListener('submit', e => {
            e.preventDefault();

            const deletedMemberId = parseInt(form.dataset.memberId);
            deleteMember(e, form.action, channelCode, deletedMemberId);
        }));
}

function deleteMember(event, formAction, channelCode, deletedMemberId) {
    event.preventDefault();

    if (!confirm("Are you sure you want to remove this member?"))
        return;

    $.ajax({
        url: formAction,
        type: 'DELETE',
        success: () => connection.invoke('RemoveMemberFromChannel', channelCode, deletedMemberId),
        error: () => showToast('Something went wrong, try again later')
    })
}

function addMessagingFunctionality() {
    const sendMessageForm = document.getElementById('messageSendForm');
    const messageInput = document.getElementById('messageInput'),
        messageMediaInput = document.getElementById('messageMediaUpload'),
        submitBtn = sendMessageForm.querySelector('button[type="submit"]');

    const spinnerIconString = '<i class="fa-solid fa-spinner"></i>';
          sendIconString = '<i class="fa-regular fa-paper-plane"></i>';

    sendMessageForm.addEventListener('submit', e => {
        e.preventDefault();
        const channelCode = document.getElementById('channelCode').value;

        messageData = new FormData();
        messageData.append('Text', messageInput.value);

        for (var i = 0; i < messageMediaInput.files.length; i++)
            messageData.append(`Media`, messageMediaInput.files[i]);

        $.ajax({
            url: sendMessageForm.action,
            processData: false,
            contentType: false,
            type: 'POST',
            data: messageData,
            beforeSend: () => submitBtn.innerHTML = spinnerIconString,
            complete: () => submitBtn.innerHTML = sendIconString,
            success: newMessage => {
                // Send message to channel server
                connection.invoke("SendMessageToChannel", newMessage, channelCode);

                // Clear form
                messageInput.value = '';
                messageMediaInput.value = null;
            },
            error: xhr => {
                if (xhr.status == 400) {
                    const errors = JSON.parse(xhr.responseText);

                    for (const propertyErrors in errors)
                        errors[propertyErrors].forEach(error =>
                            Toastify({
                                text: error,
                                duration: 6000,
                                close: true,
                                gravity: "bottom",
                                position: "right",
                            }).showToast()
                        );
                }
                else
                    showToast('Something went wrong, please try again later');
            }
        });
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

            chatBoxContainer.scrollTop = chatBoxContainer.scrollHeight;
        },
        error: () => errorToast.showToast()
    });
}

function addEditChannelFunctionality() {
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
            success: () => connection.invoke('DeleteMessageFromChannel', currOpenChat.dataset.openChatCode, parseInt(deleteForm.dataset.deleteFormId)),
            error: () => showToast('Something went wrong, try again later')
        });
    })
}


function updateSidebarChannelName(channelCode, newName) {
    const sidebarChannel = document.querySelector(`[data-sidebar-channel-code='${channelCode}']`);

    if (sidebarChannel)
        document.getElementById(`sidebarChannelName${channelCode}`).textContent = newName;
}

function deleteSidebarChannelPreviewMessage(messageId) {
    const sidebarMessage = document.getElementById(`sidebarMessage${messageId}`);

    if (!sidebarMessage)
        return;

    const messageBegin = sidebarMessage.firstElementChild.textContent.split(':')[0];
    sidebarMessage.firstElementChild.textContent = `${messageBegin}: Message was deleted`;
}

function updateSidebarChannelPreviewMessage(channelCode, message) {
    const sidebarChannel = document.querySelector(`[data-sidebar-channel-code='${channelCode}']`);

    if (sidebarChannel) {
        if (sidebarChannel.childElementCount > 1)
            sidebarChannel.removeChild(sidebarChannel.children[1]);

        $.ajax({
            url: 'partialView/messagePreview',
            contentType: 'application/json',
            type: 'POST',
            data: JSON.stringify(message),
            success: messagePreviewView => sidebarChannel.insertAdjacentHTML('beforeend', messagePreviewView)
        });
    }
}