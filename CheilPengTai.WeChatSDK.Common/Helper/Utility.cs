using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CheilPengTai.WeChatSDK.Common.Helper
{
   internal class Utility
   {
       public static Encoding DefaultEncoding = Encoding.UTF8;

       public static Dictionary<string, object> GetWeiXinJson(string url)
       {
           return GetWeiXinJson(url, "");
       }

       public static Dictionary<string, object> GetWeiXinJson(string url, Dictionary<string, string> values)
       {
           if (values == null)
           {
               if (!url.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                   url = CodeMessageEnum.RequestServerTokenUrl + url;

               var str = HttpHelper.GetWebContent(url);

               var data = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(str);
               return data;
           }

           var c = new NameValueCollection();
           foreach (string key in values.Keys)
           {
               c[key] = values[key];
           }

           return GetDataByPost(url, c);
       }

       public static string PostString(string url, string postData)
       {
           if (!url.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
               url = CodeMessageEnum.RequestServerTokenUrl + url;

           //Connection: keep-alive
           //Date: Thu, 23 Oct 2014 20:35:35 GMT
           //Server: nginx/1.4.4
           //Content-Type: application/json; encoding=utf-8
           //Content-Length: 27
           //{
           //    "errcode": 0, 
           //    "errmsg": "ok"
           //}
           return HttpHelper.GetWebContent(url);

       }

       public static Dictionary<string, object> GetWeiXinJson(string url, string postString)
       {
           if (!url.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
               url = CodeMessageEnum.RequestServerTokenUrl + url;

           var str = HttpHelper.GetWebContent(url);

           var data = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(str);

           return data;
       }

       private static Dictionary<string, object> GetDataByPost(string url, NameValueCollection values)
       {
           if (!url.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
               url = CodeMessageEnum.RequestServerTokenUrl + url;

           var client = new System.Net.WebClient();
           client.Encoding = DefaultEncoding;
           var bytes = client.UploadValues(url, "post", values);

           var str = System.Text.Encoding.UTF8.GetString(bytes);
           var data = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(str);

           return data;
       }

       public static Dictionary<string, object> ParseObjectToDictionary(object obj)
       {
           if (obj == null)
               throw Error.ArugmentNull("obj");

           var objectType = obj.GetType();
           var properties = objectType.GetProperties().Cast<PropertyInfo>();
           var propertyPairs = new Dictionary<string, PropertyInfo>(StringComparer.CurrentCultureIgnoreCase);
           foreach (var o in properties)
           {
               var attr = (DescriptionAttribute)o.GetCustomAttributes(typeof(DescriptionAttribute), true).SingleOrDefault();
               if (attr == null)
                   propertyPairs[o.Name] = o;
               else
                   propertyPairs[attr.Description] = o;
           }

           var result = new Dictionary<string, object>();
           foreach (var p in propertyPairs)
           {
               var property = p.Value;
               var value = property.GetValue(obj, null);
               if (value == null)
                   continue;

               if (p.Value.PropertyType.IsGenericType)
               {
                   var valueType = property.PropertyType;
                   var typeArgs = valueType.GetGenericArguments();
                   var itemType = valueType.GetGenericArguments()[0];
                   var enumType = typeof(IEnumerable<>).MakeGenericType(typeArgs);

                   if (!enumType.IsAssignableFrom(valueType))
                       throw Error.NotImplemented();

                   ArrayList list = new ArrayList();
                   foreach (var item in (IEnumerable)property.GetValue(obj, null))
                   {
                       var dic = ParseObjectToDictionary(item);
                       list.Add(dic);
                   }

                   if (list.Count > 0)
                       result[p.Key] = list;

               }
               else if (p.Value.PropertyType != typeof(string) && p.Value.PropertyType.IsAssignableFrom(typeof(object)))
                   result[p.Key] = ParseObjectToDictionary(value);
               else if (typeof(Enum).IsAssignableFrom(p.Value.PropertyType))
               {
                   result[p.Key] = value.ToString().ToLower();
               }
               else
                   result[p.Key] = value.ToString();
           }

           return result;
       }

       public static object ParseObjectFromDictionary(Type objectType, Dictionary<string, object> data)
       {
           if (data == null)
               throw Error.ArugmentNull("data");

           var properties = objectType.GetProperties().Cast<PropertyInfo>();
           var propertyPairs = new Dictionary<string, PropertyInfo>(StringComparer.CurrentCultureIgnoreCase);
           foreach (var o in properties)
           {
               var attr = (DescriptionAttribute)o.GetCustomAttributes(typeof(DescriptionAttribute), true).SingleOrDefault();
               if (attr == null)
                   propertyPairs[o.Name] = o;
               else
                   propertyPairs[attr.Description] = o;

           }

           var obj = Activator.CreateInstance(objectType);
           foreach (var key in data.Keys)
           {
               PropertyInfo property;
               if (propertyPairs.TryGetValue(key, out property))
               {

                   property.SetValue(obj, ConvertValueType(property.PropertyType, data[key]), null);
               }
           }
           return obj;
       }

       public static T ParseObjectFromDictionary<T>(Dictionary<string, object> data)
       {
           return (T)ParseObjectFromDictionary(typeof(T), data);
       }

       static object ConvertValueType(Type valueType, object value)
       {
           if (valueType.IsGenericType)
           {
               var typeArgs = valueType.GetGenericArguments();
               var itemType = valueType.GetGenericArguments()[0];
               var enumType = typeof(IEnumerable<>).MakeGenericType(typeArgs);

               if (enumType.IsAssignableFrom(valueType))
               {
                   var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(typeArgs));
                   foreach (var item in (IEnumerable)value)
                   {
                       var obj = ParseObjectFromDictionary(itemType, (Dictionary<string, object>)item);
                       list.Add(obj);
                   }
                   return list;
               }
           }


           if (typeof(Enum).IsAssignableFrom(valueType))
           {
               Dictionary<string, object> valuePairs = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
               //var values = Enum.GetValues(valueType);
               var properties = valueType.GetFields().Cast<FieldInfo>();
               var instance = Activator.CreateInstance(valueType);
               foreach (var property in properties)
               {
                   var attr = (DescriptionAttribute)property.GetCustomAttributes(typeof(DescriptionAttribute), true).SingleOrDefault();
                   if (attr != null)
                   {
                       valuePairs[attr.Description] = property.GetValue(instance);
                   }
                   else
                   {
                       valuePairs[property.Name] = property.GetValue(instance);
                   }
               }
               return valuePairs[(string)value];
           }
           return value;
       }

       internal static string ConvertEnumValue(Type valueType, object value)
       {
           Dictionary<object, string> valuePairs = new Dictionary<object, string>();
           var fields = valueType.GetFields().Cast<FieldInfo>();
           var instance = Activator.CreateInstance(valueType);
           foreach (var property in fields)
           {
               var attr = (DescriptionAttribute)property.GetCustomAttributes(typeof(DescriptionAttribute), true).SingleOrDefault();
               var enum_value = property.GetValue(instance);
               if (attr != null)
               {
                   valuePairs[enum_value] = attr.Description;
               }
               else
               {
                   valuePairs[enum_value] = property.Name;
               }
           }

           return valuePairs[value];
       }

   }
}
