const channelsSidebar = document.getElementById('channelsSidebar');
const messagesViewingDiv = document.getElementById('messagesViewing');

Array.from(channelsSidebar.children).forEach(channel => {
    if (channel.hasAttribute('data-sidebar-channel-id'))
        addShowFunctionality(channel);
})