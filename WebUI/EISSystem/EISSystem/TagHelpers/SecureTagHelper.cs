using EIS.Repositories.IRepository;
using EIS.WebAPI.RedisCache;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebApp.TagHelpers
{
    [HtmlTargetElement("secure")]
    public class SecureTagHelper:TagHelper
    {
        public RedisAgent Cache;
        public SecureTagHelper()
        {
            Cache = new RedisAgent();
        }


        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var access = $"/{Controller}/{Action}";
            string data = Cache.GetStringValue("Access");
            var accessList = JsonConvert.DeserializeObject<List<string>>(data);
            if (accessList.Contains(access))
            {
                return;
            }
            output.SuppressOutput();
        }
    }
}
