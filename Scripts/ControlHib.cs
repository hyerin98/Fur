// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using JoyStreamControlServer.Data;
// using JoyStreamControlServer.Services;
// using Microsoft.AspNetCore.SignalR;
// using Microsoft.Extensions.Configuration;

// namespace JoyStreamControlServer.Hubs
// {
//     //
//     // https://www.notion.so/imfine/JoyStream-WEB-Protocol-22b95e5e31bc484aab7d2c2c74f89c96
//     //
//     public class ControlHub : Hub
//     {

//         public static readonly string METHOD_RECEIVE_CONTROL_DATA = "ReceiveControlData";
//         public static readonly string METHOD_RECEIVE_SERVER_MESSAGE = "ReceiveServerMessage";
//         public static readonly string METHOD_RECEIVE_WALL_MESSAGE = "ReceiveWallMessage";

//         private readonly ControlUserService userService;
        

//         public ControlHub(IConfiguration config , ControlUserService userService)
//         {
//             this.userService = userService;
//         }


//         //-------------------------------------------------------- Client Senders
//         #region Client Senders

//         /// <summary>
//         /// mobile to wall
//         /// </summary>
//         /// <param name="user_id"></param>
//         /// <param name="name"></param>
//         /// <param name="value"></param>
//         /// <returns></returns>
//         public async Task SendControlData(string user_id, string name, string value)
//         {
//             foreach (var wall in userService.WallConnetionIds)
//             {
//                 Clients.Client(wall).SendAsync(METHOD_RECEIVE_CONTROL_DATA, user_id, name, value);
//             }
//         }

//         /// <summary>
//         /// wall to mobile
//         /// </summary>
//         /// <param name="user_id"></param>
//         /// <param name="message"></param>
//         /// <returns></returns>
//         public async Task SendWallMessage(string user_id, string message, string value)
//         {
//             ControlUser user = userService.FindUserByUserId(user_id);
//             if (user == null)
//             {
//                 userService.ShootServerMessage(Context.ConnectionId, SERVER_CODES.ERR_USER_NOT_EXISTS, $"ERR_USER_NOT_EXISTS : {user_id}");
//                 return;
//             }

//             await Clients.Client(user.conn_id).SendAsync(METHOD_RECEIVE_WALL_MESSAGE, user_id, message , value);
//         }


//         #endregion Client Senders


//         //sample
//         public async Task ShootServerMessage(short code , string message)
//         {
//             Clients.All.SendAsync(METHOD_RECEIVE_SERVER_MESSAGE, code, message);
//         }



//         //-------------------------------------------------------- Connection Ctrl
//         #region Connection Ctrl


//         public override Task OnConnectedAsync()
//         {
//             var conn_id = Context.ConnectionId;
//             var user_id = Context.GetHttpContext().Request.Query["user_id"].ToString();
//             userService.OnConnectedAsync(conn_id , user_id);

//             return base.OnConnectedAsync();
//         }

//         public override Task OnDisconnectedAsync(Exception exception)
//         {
//             var id = Context.ConnectionId;
//             userService.OnDisconnectedAsync(id);

//             return base.OnDisconnectedAsync(exception);
//         }

//         #endregion Connection Ctrl



       
        

        



//     }

    
        

// }
