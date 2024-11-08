# SignalR
SignalR is a free, open-source software library that allows developers to add real-time communication in a bi-directional way to web applications.

## Core Components of SignalR
### 1#Hubs
A Hub acts as a communication bridge between the server and clients.

### 2#Connections:
SignalR manages a persistent connection, allowing it to send messages in real-time. SignalR decides the best transport method `(WebSockets, Server-Sent Events, or Long Polling)` based on what the client and server support.

### 3#Groups:
SignalR allows you to manage groups of connected clients, making it easy to broadcast messages to specific groups rather than all connected clients.
For example, in a chat application, you might create a group for each chat room, and send messages to everyone in that room.
1. **JoinGroup:** This method adds the user (identified by Context.ConnectionId) to a group. You can think of the groupName as the chat room name or game room name. When a user joins, we notify others in the group by sending a message.
2. **LeaveGroup:** Removes the user from the group, ensuring they won't receive any further messages from that group.
3. **SendMessageToGroup:** This broadcasts a message to all clients connected to the specified group.

### 4#Users
To send messages to specific users, you need to track which connection IDs belong to a user. For example, if a `user logs in from two devices`, both `connections should be tracked` and a `message sent to both`.
1. Clients.Client(connectionId): You may use this in a system where users have a single session or connection and you specifically track connections through their IDs.
2. Clients.User(userId): Ideal for a scenario where a user can be logged in on multiple devices, and you want to ensure that the user receives messages on any of their active sessions.

### SignalR all methods
| Method                               | Description                                                             | Example Usage                                                                 |
|--------------------------------------|-------------------------------------------------------------------------|-------------------------------------------------------------------------------|
| `Clients.All.SendAsync()`             | Sends a message to all connected clients.                                | `await Clients.All.SendAsync("ReceiveMessage", message);`                    |
| `Clients.Client(connectionId).SendAsync()` | Sends a message to a specific client identified by connection ID.       | `await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);`   |
| `Clients.User(userId).SendAsync()`   | Sends a message to a specific user identified by their user ID.         | `await Clients.User(userId).SendAsync("ReceiveMessage", message);`           |
| `Clients.Group(groupName).SendAsync()`| Sends a message to all clients in a specific group.                     | `await Clients.Group(groupName).SendAsync("ReceiveMessage", message);`       |
| `Clients.AllExcept(connectionId).SendAsync()` | Sends a message to all clients except the one identified by the connection ID. | `await Clients.AllExcept(Context.ConnectionId).SendAsync("ReceiveMessage", message);` |
| `Clients.GroupExcept(groupName, connectionId).SendAsync()` | Sends a message to all clients in a group, except the one identified by the connection ID. | `await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveMessage", message);` |
| `Groups.AddToGroupAsync(connectionId, groupName)` | Adds a client to a group.                                                | `await Groups.AddToGroupAsync(Context.ConnectionId, groupName);`             |
| `Groups.RemoveFromGroupAsync(connectionId, groupName)` | Removes a client from a group.                                           | `await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);`        |
| `OnConnectedAsync()`                 | This method is called when a client connects to the server.              | `public override Task OnConnectedAsync() { ... }`                            |
| `OnDisconnectedAsync(exception)`     | This method is called when a client disconnects from the server.         | `public override Task OnDisconnectedAsync(Exception exception) { ... }`    |
| `SendAsync()`                         | Sends a message from the server to the client (used by clients and groups). | `await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);` |
| `Clients.All.SendAsync("MethodName", ...)` | Sends a message to all clients, invoking a specific method.             | `await Clients.All.SendAsync("ReceiveMessage", message);`                    |
| `Clients.User(userId).SendAsync()`   | Sends a message to a specific user by their user ID.                     | `await Clients.User(userId).SendAsync("ReceiveMessage", message);`           |
| `Clients.Client(connectionId).SendAsync()` | Sends a message to a specific client identified by their connection ID. | `await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);`  |
| `Clients.Group(groupName).SendAsync()` | Sends a message to a group of clients identified by a group name.        | `await Clients.Group(groupName).SendAsync("ReceiveMessage", message);`      |
| `Clients.GroupExcept(groupName, connectionId).SendAsync()` | Sends a message to all clients in a group except one identified by a connection ID. | `await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveMessage", message);` |
| `Groups.AddToGroupAsync(connectionId, groupName)` | Adds a client to a group using connection ID and group name.             | `await Groups.AddToGroupAsync(Context.ConnectionId, groupName);`             |
| `Groups.RemoveFromGroupAsync(connectionId, groupName)` | Removes a client from a group using connection ID and group name.        | `await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);`        |
| `OnConnectedAsync()`                 | Triggered when a connection is established with the client.              | `public override Task OnConnectedAsync() { ... }`                            |
| `OnDisconnectedAsync(exception)`     | Triggered when a client disconnects from the server.                     | `public override Task OnDisconnectedAsync(Exception exception) { ... }`    |
| `Clients.Caller.SendAsync()`         | Sends a message to the client that called the method.                    | `await Clients.Caller.SendAsync("ReceiveMessage", message);`                 |
| `Clients.Others.SendAsync()`         | Sends a message to all clients except the caller.                        | `await Clients.Others.SendAsync("ReceiveMessage", message);`                 |
| `Clients.All.SendAsync("ReceiveMessage", ...)` | Sends a message to all clients connected to the SignalR server.         | `await Clients.All.SendAsync("ReceiveMessage", message);`                    |
| `Clients.Others.SendAsync("ReceiveMessage", ...)` | Sends a message to all clients except the caller.                       | `await Clients.Others.SendAsync("ReceiveMessage", message);`                 |
| `Groups.AddToGroupAsync()`           | Adds a client to a group.                                                | `await Groups.AddToGroupAsync(connectionId, groupName);`                     |
| `Groups.RemoveFromGroupAsync()`      | Removes a client from a group.                                           | `await Groups.RemoveFromGroupAsync(connectionId, groupName);`                |
| `Clients.Group(groupName)`           | Sends a message to a group of clients by group name.                     | `await Clients.Group(groupName).SendAsync("ReceiveMessage", message);`       |
| `Groups.AddToGroupAsync(connectionId, groupName)` | Adds the current connection to a group using the connection ID and group name. | `await Groups.AddToGroupAsync(Context.ConnectionId, groupName);`             |
| `Groups.RemoveFromGroupAsync(connectionId, groupName)` | Removes the current connection from a group using connection ID and group name. | `await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);`        |
| `Clients.All.SendAsync("MethodName", ...)` | Calls a method on all clients connected to the SignalR server.           | `await Clients.All.SendAsync("ReceiveMessage", message);`                    |
| `Clients.User(userId).SendAsync()`   | Sends a message to a specific user using their user ID.                  | `await Clients.User(userId).SendAsync("ReceiveMessage", message);`           |
| `Clients.Client(connectionId).SendAsync()` | Sends a message to a client by connection ID.                           | `await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);`  |
| `Clients.Group(groupName).SendAsync()` | Sends a message to all clients in a group identified by a group name.   | `await Clients.Group(groupName).SendAsync("ReceiveMessage", message);`      |
| `Groups.AddToGroupAsync()`           | Adds the current connection to a group identified by a group name.      | `await Groups.AddToGroupAsync(Context.ConnectionId, groupName);`             |
| `Groups.RemoveFromGroupAsync()`      | Removes the current connection from a group identified by a group name. | `await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);`        |


---
## How does signalR work?
1. Client Connects: The client initiates a connection to the server using a Hub endpoint. SignalR negotiates the best transport protocol and assigns a Connection ID.
2. Protocol Selection: SignalR automatically chooses the best protocol available based on the client’s and server’s capabilities. It prefers WebSockets (a persistent, full-duplex communication channel) but falls back to Server-Sent Events (SSE) or Long Polling if WebSockets aren’t supported.
3. Communication flow: Once connected, clients can call methods on the server, and the server can invoke client methods:
   1. Server-to-Client Calls: The server can push updates to clients (e.g., a new chat message) without waiting for client requests.
   2. Client-to-Server Calls: Clients can request data changes or trigger server actions (e.g., submitting a chat message).
4. Reconnection: If the client’s connection is interrupted, SignalR tries to reconnect automatically using the same Connection ID.
5. Group Messaging: SignalR can organize connections into groups. When sending a message to a group, it reaches all members of that group without requiring individual broadcasts.
6. Client Disconnects: When a client disconnects (e.g., by closing their browser), SignalR releases resources associated with the Connection ID and removes the connection from any groups.

---
## Hearbeat in SignalR
In SignalR, a `heartbeat` is a mechanism used to `keep track of active connections`, detect dropped connections, and `manage reconnection attempts`.
1. `KeepAlive Interval:` The server sends regular keep-alive messages to the client to maintain the connection. If a client doesn’t respond to these messages, it can be considered disconnected.
2. `Server Timeout:` The server waits a certain period of time for a response after sending a heartbeat
    ```c#
    services.AddSignalR()
        .AddHubOptions<NotificationHub>(options =>
        {
            // Configure keep-alive interval and server timeout
            options.KeepAliveInterval = TimeSpan.FromSeconds(10); // Default is 15 seconds
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30); // Server timeout after 30 seconds
        });
    ```
---
## How can a client can acheive the connectionId?
In SignalR, the client is automatically assigned a unique Connection ID when it connects to the server, which is managed by SignalR itself.
1. `override the OnConnectedAsync` method in the Hub to send the Connection ID back to the client directly.
2. use a `custom method` in the Hub that the client calls immediately after connecting.

    ```c#
    public class NotificationHub : Hub
    {
        // 1# using OnConnectedAsync method
        public override async Task OnConnectedAsync()
        {
            // Get the Connection ID
            string connectionId = Context.ConnectionId;
            // Send the Connection ID to the client
            await Clients.Caller.SendAsync("ReceiveConnectionId", connectionId);
            await base.OnConnectedAsync();
        }
        // 2# custom method
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
    ```
### How can client acheieve the above connection?
1. `.invoke:` Client calls a server method.
2. `.on:` Client listens for server messages.

    ```js
    const connectionId = await connection.invoke("GetConnectionId");
    connection.on("ReceiveConnectionId", (user, message) => {
        console.log(`${user}: ${message}`);
    });
    ```