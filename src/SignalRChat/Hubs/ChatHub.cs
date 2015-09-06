using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalRChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        private ITableStorageWriter _writer;

        /// <summary>
        /// Example dependency-injected hub
        /// </summary>
        /// <param name="writer"></param>
        public ChatHub(ITableStorageWriter writer)
        {
            _writer = writer;
        }

        /// <summary>
        /// Maintain a list of connected users
        /// </summary>
        public static Dictionary<string, string> Users = new Dictionary<string, string>();

        /// <summary>
        /// Send a chat message
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void Send(string name, string message)
        {
            var timestamp = DateTime.Now.ToLongTimeString();

            var entity = new ChatMessageEntity(
                name.ToLower(),
                Context.ConnectionId + DateTime.Now.ToLongTimeString());
            entity.Message = message;

            _writer.Insert(entity);

            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }

        /// <summary>
        /// Maintain a count of connected users
        /// </summary>
        /// <param name="count"></param>
        public void Count(int count)
        {
            Clients.All.updateUsersOnlineCount(count);
            GetUsersOnline();
        }

        /// <summary>
        /// Subscribe an active online user by adding to list
        /// </summary>
        /// <param name="displayName"></param>
        public void Subscribe(string displayName)
        {
            Users.Add(Context.ConnectionId, displayName);
            Count(Users.Count);
        }

        /// <summary>
        /// Retrive a list of online users
        /// </summary>
        /// <returns></returns>
        public List<string> GetUsersOnline()
        {
            var onlineUsers = Users.Values.ToList<string>();
            Clients.All.getOnlineUsers(onlineUsers);
            return onlineUsers;
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _writer.Execute();


            if (Users.Keys.Contains(this.Context.ConnectionId))
                Users.Remove(this.Context.ConnectionId);

            // Send the current count of users
            Count(Users.Count);

            return base.OnDisconnected(false);
        }

        public override Task OnReconnected()
        {
            GetUsersOnline();

            return base.OnReconnected();
        }
    }
}
