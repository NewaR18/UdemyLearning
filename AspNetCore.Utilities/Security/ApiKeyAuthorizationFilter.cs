using AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.Security
{
    public sealed class ApiKeyAuthorizationFilter : Attribute, IAuthorizationFilter
    {

        //Currently Not Used || Later It can be used in case of API
        //it can be used like this
        /*
        [ApiKeyAuthorizationFilter]
        public IEnumerable<Category> GetAllCategories()
        {
            IEnumerable<Category> entities = _repo.CategoryRepo.GetAll();
            return entities;
        }
        */
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var providedAPIKey = request.Headers["ApiKey"].FirstOrDefault();
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = GetApiKey(configuration);
            if(string.IsNullOrWhiteSpace(providedAPIKey) ||  providedAPIKey !=apiKey)
            {
                context.Result = new UnauthorizedResult();
            }
        }
        public string GetApiKey(IConfiguration configuration)
        {
            string apiKey = configuration["APIKEY"];
            if(apiKey == null)
            {
                return "";

            }
            return apiKey;
        }
    }
}
