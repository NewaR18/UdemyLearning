using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Utilities.StaticDefinitions;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace AspNetCoreFromBasic.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _repo;
        public ShoppingCartViewComponent(IUnitOfWork repo)
        {
            _repo = repo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
            var userClaims = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaims != null)
            {
                if (HttpContext.Session.GetInt32(StaticStrings.ShoppingCartCountForUser) != null
                    &&
                    HttpContext.Session.GetInt32(StaticStrings.ShoppingCartCountForUser) != 0)
                {
                    return View(HttpContext.Session.GetInt32(StaticStrings.ShoppingCartCountForUser));
                }
                else
                {
                    var count = _repo.ShoppingCartRepo
                                    .GetAll(x => x.ApplicationUserId
                                                .Equals(userClaims.Value))
                                    .Count();
                    HttpContext.Session.SetInt32(StaticStrings.ShoppingCartCountForUser, count);
                    return View(HttpContext.Session.GetInt32(StaticStrings.ShoppingCartCountForUser));
                }
            }
            else
            {
                HttpContext.Session.SetInt32(StaticStrings.ShoppingCartCountForUser, 0);
                return View(0);
            }
        }
    }
}
