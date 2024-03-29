﻿// ReSharper disable All

using System.ComponentModel.DataAnnotations;
using general_shortener.Models.Entry;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#pragma warning disable 1591
namespace general_shortener.Models
{
    public class BaseResponse<T>
    {
        
        /// <summary>
        /// Status of the response
        /// </summary>
        [EnumDataType(typeof(StatusType))]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusType Status { get; set; }
        
        /// <summary>
        /// Response data
        /// </summary>
        public T Result { get; set; }
    }

    public enum StatusType
    {
        ok,
        error
    }
}