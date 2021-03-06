// Copyright (C) 2010 Ross Merrigan
// Licensed under the GPL licenses:
// http://www.gnu.org/licenses/gpl.html

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace ReduxArch.Web
{
    /// <summary>
    /// Strongly type to SelectListItem
    /// </summary>
    public static class SelectListItemExtensions
    {
       public static IEnumerable<SelectListItem> ToSelectList<TModel, TProperty>(this IEnumerable<TModel> collection, Expression<Func<TModel, object>> text,
                    Expression<Func<TModel, TProperty>> value)
        {
            var model = new List<TModel>();
            return collection.ToSelectList(text, value, model);
        }

        public static IEnumerable<SelectListItem> ToSelectList<TModel, TProperty>(this IEnumerable<TModel> collection, Expression<Func<TModel, object>> text,
            Expression<Func<TModel, TProperty>> value, TModel selected)
        {
            var model = new List<TModel> { selected };
            return collection.ToSelectList(text, value, model);
        }

        public static IEnumerable<SelectListItem> ToSelectList<TModel, TProperty>(this IEnumerable<TModel> collection, Expression<Func<TModel, object>> text,
            Expression<Func<TModel, TProperty>> value, IEnumerable<TModel> selected)
        {
            var dlgText = (Func<TModel, object>)CreatePropertyDelegate<TModel, object>(ExpressionHelper.GetExpressionText(text));
            var dlgValue = (Func<TModel, TProperty>)CreatePropertyDelegate<TModel, TProperty>(ExpressionHelper.GetExpressionText(value));

            if (dlgText == null) throw new NullReferenceException("Text Property not found");

            if (dlgValue == null) throw new NullReferenceException("Value Property not found");

            if (selected.Count() > 0)
            {
                var selectedValues = (selected.Where(p => p != null).Select(p => dlgValue(p).ToString())).ToList();

                return collection.Select(p => new SelectListItem
                {
                    Text = dlgText(p).ToString(),
                    Value = dlgValue(p).ToString(),
                    Selected = selectedValues.Contains(dlgValue(p).ToString())
                });
            }

            return collection.Select(p => new SelectListItem
            {
                Text = dlgText(p).ToString(),
                Value = dlgValue(p).ToString()
            });
        }

        private static Delegate CreatePropertyDelegate<TModel, TProperty>(string property)
        {
            var propertyInfo = typeof(TModel).GetProperty(property);

            var method = propertyInfo.GetAccessors(true);
            if (method != null && method.Count() > 0)
            {
                return Delegate.CreateDelegate(typeof(Func<TModel, TProperty>), method.First());
            }

            return null;
        }
    }
}
