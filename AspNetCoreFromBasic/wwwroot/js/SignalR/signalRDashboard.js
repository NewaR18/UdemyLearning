//const connection = new signalR.HubConnectionBuilder()
//    .withUrl("/dashboardHub")
//    .build();
const connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Debug)
    .withUrl("/dashboardHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .build();
//This method receive the message and Append to our list  
connection.on("ReceiveMessage", (orderCount, message) => {
    const li = document.createElement("li");
    li.textContent = message;
    li.classList.add("listOfContents");
    //document.getElementById("messagesList").appendChild(li);
    document.getElementById("messagesList").prepend(li);
});

connection.start().catch(err => console.error(err.toString()));