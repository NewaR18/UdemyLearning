const connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Debug)
    .withUrl("/chatHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .build();
//This method receive the message and Append to our list  
connection.on("ReceiveMessage", (user, message) => {
    debugger;
    //const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;"); //To avoid XSS attack
    const encodedMsg = user + " : " + message;
    const li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().catch(err => console.error(err.toString()));


document.getElementById("sendMessage").addEventListener("click", event => {
    const message = $("#userMessage").val();
    $("#userMessage").val("");
    connection.invoke("SendMessage", message).catch(err => console.error(err.toString()));
    event.preventDefault();
});   