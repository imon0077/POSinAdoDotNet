using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Reflection;
using Softify;

namespace SoftifyFoodPOSNew.Models.Helpers
{
    public static class ImageExtension
    {
        public static MvcHtmlString ImageActionLink(
            this HtmlHelper helper,
            string imageUrl,
            string altText,
            string actionName,
            string controllerName,
            object imgHtmlAttributes)
        {
            var imgAttributes = AnonymousObjectToKeyValue(imgHtmlAttributes);
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", imageUrl);
            imgBuilder.MergeAttribute("alt", altText);
            imgBuilder.MergeAttributes(imgAttributes, true);
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            var linkBuilder = new TagBuilder("a");
            linkBuilder.MergeAttribute("href", urlHelper.Action(actionName, controllerName));
            var text = linkBuilder.ToString(TagRenderMode.StartTag);
            text += imgBuilder.ToString(TagRenderMode.SelfClosing);
            text += linkBuilder.ToString(TagRenderMode.EndTag);
            return MvcHtmlString.Create(text);
        }
        public static MvcHtmlString ImageThumbnail(
           this HtmlHelper helper,
           string imageUrl,
           string altText,
           string actionName,
           string controllerName)
        {
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", imageUrl);
            imgBuilder.MergeAttribute("alt", altText);
            imgBuilder.AddCssClass("img-thumbnail");
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            var linkBuilder = new TagBuilder("a");
            linkBuilder.MergeAttribute("href", urlHelper.Action(actionName, controllerName));
            var text = linkBuilder.ToString(TagRenderMode.StartTag);
            text += imgBuilder.ToString(TagRenderMode.SelfClosing);
            text += linkBuilder.ToString(TagRenderMode.EndTag);
            return MvcHtmlString.Create(text);
        }
        private static Dictionary<string, object> AnonymousObjectToKeyValue(object anonymousObject)
        {
            var dictionary = new Dictionary<string, object>();
            if (anonymousObject != null)
            {
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
                {
                    dictionary.Add(propertyDescriptor.Name, propertyDescriptor.GetValue(anonymousObject));
                }
            }
            return dictionary;
        }
    }
}
