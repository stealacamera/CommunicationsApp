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