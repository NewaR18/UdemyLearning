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
    public class ChatHub: Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        public ChatHub(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task SendMessage(string message)
        {
            var userClaimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            if (userId != null)
            {
                var userDetails = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
                var userName = userDetails.Name;
                await Clients.All.SendAsync("ReceiveMessage", userName, message);
            }
        }
    }
}
