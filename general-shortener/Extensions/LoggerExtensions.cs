using System.Net;
using System.Text;
using general_shortener.Models.Entry;
using general_shortener.Models.Logging;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace general_shortener.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogEntryInfo<T>(this ILogger<T> logger, EntryInfoLoggingModel model)
        {
            logger.LogInformation(model.ToJson());
        }
    }
}