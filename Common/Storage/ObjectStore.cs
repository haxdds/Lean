﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using QuantConnect.Interfaces;

namespace QuantConnect.Storage
{
    /// <summary>
    /// Helper class for easier access to <see cref="IObjectStore"/> methods
    /// </summary>
    public static class ObjectStore
    {
        /// <summary>
        /// Returns the string object data for the specified key
        /// </summary>
        public static string ReadText(this IObjectStore store, string key, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            return encoding.GetString(store.Read(key));
        }

        /// <summary>
        /// Returns the JSON deserialized object data for the specified key
        /// </summary>
        public static T ReadJson<T>(this IObjectStore store, string key, Encoding encoding = null, JsonSerializerSettings settings = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            var json = store.ReadText(key, encoding);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// Returns the XML deserialized object data for the specified key
        /// </summary>
        public static T ReadXml<T>(this IObjectStore store, string key, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            var xml = store.ReadText(key, encoding);

            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Saves the object data in text format for the specified key
        /// </summary>
        public static bool SaveText(this IObjectStore store, string key, string text, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            return store.Save(key, encoding.GetBytes(text));
        }

        /// <summary>
        /// Saves the object data in JSON format for the specified key
        /// </summary>
        public static bool SaveJson<T>(this IObjectStore store, string key, T obj, Encoding encoding = null, JsonSerializerSettings settings = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            var json = JsonConvert.SerializeObject(obj, settings);
            return store.SaveText(key, json, encoding);
        }

        /// <summary>
        /// Saves the object data in XML format for the specified key
        /// </summary>
        public static bool SaveXml<T>(this IObjectStore store, string key, T obj, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            using (var writer = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, obj);

                var xml = writer.ToString();
                return store.SaveText(key, xml, encoding);
            }
        }
    }
}
