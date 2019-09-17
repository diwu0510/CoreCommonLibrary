using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HZC.Utils.Mvcs
{
    public static class SelectListItemExtension
    {
        public static List<SelectListItem> ToSelectList(this string[] data, string value = null, string firstOption = null)
        {
            var result = new List<SelectListItem>();

            if (!string.IsNullOrWhiteSpace(firstOption))
            {
                result.Add(new SelectListItem { Text = firstOption, Value = string.Empty });
            }

            foreach (var d in data)
            {
                result.Add(new SelectListItem { Text = d, Value = d, Selected = d == value });
            }

            return result;
        }

        public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> data, string valueProp, string textProp)
        {
            var items = new List<SelectListItem>();

            if (data == null)
            {
                return items;
            }

            data = data.ToList();
            if (!data.Any())
            {
                return items;
            }

            var props = typeof(T).GetProperties();
            var vProp = props.SingleOrDefault(p => p.Name == valueProp);
            var tProp = props.SingleOrDefault(p => p.Name == textProp);

            if (vProp == null || tProp == null)
            {
                throw new ArgumentException("指定的字段名不存在");
            }

            foreach (var t in data)
            {
                var text = tProp.GetValue(t).ToString();
                var value = vProp.GetValue(t).ToString();

                items.Add(new SelectListItem { Value = value, Text = text });
            }
            return items;
        }

        public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> data, string valueProp, string textProp, string selectedValue)
        {
            if (data == null)
            {
                return new List<SelectListItem>();
            }

            data = data.ToList();
            if (!data.Any())
            {
                return new List<SelectListItem>();
            }

            var props = typeof(T).GetProperties();
            var vProp = props.SingleOrDefault(p => p.Name == valueProp);
            var tProp = props.SingleOrDefault(p => p.Name == textProp);

            if (vProp == null || tProp == null)
            {
                throw new ArgumentException("指定的字段名不存在");
            }

            var items = new List<SelectListItem>();
            foreach (var t in data)
            {
                var text = tProp.GetValue(t).ToString();
                var value = vProp.GetValue(t).ToString();
                items.Add(selectedValue == value
                    ? new SelectListItem { Value = value, Text = text, Selected = true }
                    : new SelectListItem { Value = value, Text = text });
            }
            return items;
        }
    }
}
