using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace general_shortener_tests
{
    public static class HttpUtils
    {

        public static MultipartFormDataContent ConstructFormDataContent(object formData, string filePath = null)
        {
            var multipartFormDataContent = new MultipartFormDataContent();

            foreach (var item in ToDictionary<string>(formData))
            {
                if(string.IsNullOrEmpty(item.Value))
                    continue;
                multipartFormDataContent.Add(new StringContent(item.Value), String.Format("\"{0}\"", item.Key));
            }

            if (filePath != null)
            {
                multipartFormDataContent.Add(new ByteArrayContent(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, filePath))), "\"file\"", Path.GetFileName(filePath));
            }
            
            return multipartFormDataContent;
        }
        
        /// <summary>
        /// https://stackoverflow.com/questions/11576886/how-to-convert-object-to-dictionarytkey-tvalue-in-c
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, TValue> ToDictionary<TValue>(object obj)
        {       
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);   
            return dictionary;
        }
        
    }
}