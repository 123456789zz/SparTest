using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Config
{
    public static class Util
    {
        public static int GetInt(this XElement elem, string attr)
        {
            return int.Parse(elem.GetString(attr));
        }
        public static string GetString(this XElement elem, string attr)
        {
            return elem.Attribute(attr).Value;
        }
        public static string GetID(this XElement elem)
        {
            return elem.GetString("id");
        }
        public static bool GetBool(this XElement elem, string attr)
        {
            return bool.Parse(elem.GetString(attr).ToLower());
        }

        public static void Set(this XElement elem, string attr, object value)
        {
            elem.Attribute(attr).Value = value.ToString();
        }

        public static IEnumerable<XElement> ListOf(this XElement parent, string containerElemName, string elemName)
        {
            return parent.Element(containerElemName).Elements(elemName);
        }

        public static IEnumerable<XElement> ListOf(this XElement parent, string elemName)
        {
            return parent.ListOf(elemName + "s", elemName);
            //return parent.Element(elemName + "s").Elements(elemName);
        }
        public static void AddToListOf<T>(this XElement parent, string elemName, T item, Func<T, XElement> predicate)
        {
            var elem = parent.Element(elemName + "s");
            elem.Add(predicate(item));
        }
        public static void UpdateListOf<T>(this XElement parent, string elemName, string id, T item, Func<T, XElement> predicate)
        {
            parent.ListOf(elemName).ByID(id).ReplaceWith(predicate(item));
        }
        public static void UpdateListOf<T>(this XElement parent, string elemName, IEnumerable<T> list, Func<T, XElement> predicate)
        {
            var elem = parent.Element(elemName + "s");
            elem.Elements(elemName).Remove();
            foreach (var item in list)
                elem.Add(predicate(item));
        }

        public static XElement ByID(this IEnumerable<XElement> elems, string id)
        {
            /*XElement e;
            foreach (var elem in elems)
            {
                e = elem;
            }*/
            return elems.Single(a => a.GetString("id") == id);
        }

        public static IEnumerable<T> To<T>(this IEnumerable<XElement> elems, Func<XElement, T> predicate)
        {
            return elems.Select(a => a.To(predicate));
        }
        public static T To<T>(this XElement elem, Func<XElement, T> predicate)
        {
            return predicate(elem);
        }
        public static T To<T>(this IEnumerable<XElement> elems, Func<IEnumerable<XElement>, T> predicate)
        {
            return predicate(elems);
        }
        public static IDictionary<string, T> ToIDMap<T>(this IEnumerable<XElement> elems, Func<XElement, T> predicate)
        {
            return elems.ToDictionary(a => a.GetString("id"), a => a.To(predicate));
        }
        public static XElement To<T>(this IEnumerable<T> items, string elemName, Func<T, XAttribute[]> predicate, params XAttribute[] attrs)
        {
            return items.To(elemName + "s", elemName, predicate, attrs);
        }
        public static XElement To<T>(this IEnumerable<T> items, string parentElemName, string childElemName, Func<T, XAttribute[]> predicate, params XAttribute[] attrs)
        {
            return new XElement(parentElemName, attrs, items.Select(item => new XElement(childElemName, predicate(item))));
        }
        public static XElement To<T>(this T item, string elemName, Func<T, IEnumerable<XAttribute[]>> predicate)
        {
            return new XElement(elemName + "s", predicate(item).Select(attributes => new XElement(elemName, attributes)));
        }
    }
}
