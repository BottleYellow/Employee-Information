using EIS.WebApp.Models;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EIS.WebApp.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("HeaderWithButton")]
    //[HtmlTargetElement("HeaderWithButton")]
    public class HeaderTagHelper : TagHelper
    {
        public string HeaderName
        {
            get;
            set;
        }
        public string BtnHref
        {
            get;
            set;
        }
        public string IconName
        {
            get;
            set;
        }
        public string BtnText
        {
            get;
            set;
        }
        public string BtnId
        {
            get;
            set;
        }
        public string BtnHref2
        {
            get;
            set;
        }
        public string IconName2
        {
            get;
            set;
        }
        public string BtnText2
        {
            get;
            set;
        }
        public string BtnId2
        {
            get;
            set;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            
            output.TagName = "h1";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(HeaderName);
            output.Attributes.Add("style", "font-size:30px;margin:0;display:inline-block;");
            if (BtnText != null)
            {
                output.PostElement.AppendHtml("<button style='float:right;' id='" + BtnId + "' class='btn btn-primary waves-effect' onclick=" + BtnHref + "><i class='material-icons'>" + IconName + "</i><span style='font-size:15px;'>" + BtnText + "</span></button>");
            }
            if (BtnText2 != null)
            {
                output.PostElement.AppendHtml("<button style='margin-right:16px;' id='" + BtnId2 + "' class='btn btn-primary waves-effect pull-right' onclick=" + BtnHref2 + "><i class='material-icons'>" + IconName2 + "</i><span style='font-size:15px;'>" + BtnText2 + "</span></button>");
            }
        }
    }
}
