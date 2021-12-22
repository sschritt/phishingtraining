

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PhishingTraining.Web.Helpers
{
    public static class MvcHelper
    {
        public static string ToControllerName(this string controllerTypeName)
        {
            return controllerTypeName.Replace("Controller", string.Empty);
        }

        public static IEnumerable<SelectListItem> AsSelectListItems(this Enum selectedValue, Type type, [Optional] bool removeNullableEntry, string emptyValueText = "")
        {
            if (type.IsNullableType() && !removeNullableEntry)
            {
                yield return new SelectListItem
                {
                    Text = emptyValueText,
                    Value = "",
                    Selected = selectedValue == null
                };
            }

            if (type.IsGenericType && type.IsNullableType())
            {
                type = Nullable.GetUnderlyingType(type);
            }

            foreach (var x in Enum.GetValues(type ?? throw new ArgumentNullException(nameof(type))))
            {
                yield return new SelectListItem
                {
                    Text = GetLocalizedEnumText(type, x),
                    Value = x.ToString(),
                    Selected = x.Equals(selectedValue)
                };
            }
        }

        private static string GetLocalizedEnumText(Type enumType, object value)
        {
            var name = $"{enumType.Name}_{value}";
            return Resources.Enums.ResourceManager.GetString(name);
        }

        public static bool HasFeatureClaim(this ClaimsPrincipal user, string featureClaim)
        {
            return user.HasClaim(claim => claim.Type == featureClaim);
        }
    }
}
