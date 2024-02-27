using AspNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.WebSocketImplementation
{
    public class DashboardHub : Hub
    {
        //The simple answer is you can't directly call a hub method from an MVC controller or elsewhere. This is by design.
        //https://stackoverflow.com/questions/46904678/call-signalr-core-hub-method-from-controller
    }
}
