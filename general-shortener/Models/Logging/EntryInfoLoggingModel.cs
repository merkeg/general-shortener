using System.Net;
using general_shortener.Models.Entry;
using MongoDB.Bson;

namespace general_shortener.Models.Logging
{
    public class EntryInfoLoggingModel
    {
        public HttpStatusCode Status;
        public string Id;
        public EntryType Type;
        public long Size = 0;
        public string Value;
        public string Notes;
        public string Owner;
        public string Mimetype;
    }
}