using System;
using System.Linq;
using System.Threading.Tasks;
using general_shortener.Models;
using general_shortener.Models.Entry;
using general_shortener.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace general_shortener.Services
{
    /// <inheritdoc />
    public class TransferService: ITransferService
    {
        private readonly ILogger<TransferService> _logger;
        private readonly TransferOptions _options;

        /// <inheritdoc />
        public ConnectionMultiplexer RedisConnection { get; }

        /// <inheritdoc />
        public IDatabase RedisDatabase { get; }

        /// <inheritdoc />
        public bool TransferActivated => _options.Transfer;
        
        private readonly IMongoCollection<Entry> _entries;

        /// <summary>
        /// TransferService ctor
        /// </summary>
        public TransferService(IOptions<TransferOptions> options, IMongoDatabase mongoDatabase, ILogger<TransferService> logger)
        {
            _logger = logger;
            _entries = mongoDatabase.GetCollection<Entry>(Entry.Collection);
            _options = options.Value;

            if (TransferActivated)
            {
                var configuration = new ConfigurationOptions
                {
                    EndPoints = {$"{_options.Redis.Host}:{_options.Redis.Port}"}
                };

                if (!string.IsNullOrEmpty(_options.Redis.Password))
                {
                    configuration.Password = _options.Redis.Password;
                }
                
                RedisConnection = ConnectionMultiplexer.Connect(configuration);

                RedisDatabase = RedisConnection.GetDatabase();
                Console.WriteLine("Transferring redis data to MongoDB");
#pragma warning disable 4014
                TransferData();
#pragma warning restore 4014
            }

            
        }
        
        /// <inheritdoc />
        public async Task TransferData()
        {
            if (!TransferActivated)
                return;
            
            var endPoint = RedisConnection.GetEndPoints().First();
            RedisKey[] keys = RedisConnection.GetServer(endPoint).Keys(pattern: "*").ToArray();

            foreach (RedisKey key in keys)
            {
                string jsonValue = await RedisDatabase.StringGetAsync(key);
                
                if(string.IsNullOrEmpty(jsonValue))
                    continue;
                
                RedisTransferEntryModel model = JsonConvert.DeserializeObject<RedisTransferEntryModel>(jsonValue);

                Entry newEntry = new Entry
                {
                    DeletionCode = model.DeletionCode,
                    Slug = key.ToString(),
                    Type = model.Type,
                    Value = model.Type == EntryType.file ? null : model.Value,
                    Meta = new EntryMetadata
                    {
                        Filename = model.Type == EntryType.file ? model.Value : null,
                        Mime = model.Mime,
                        Size = model.Size,
                        OriginalFilename = model.Type == EntryType.file ? model.Value : null
                    }
                };

                if (model.Type == EntryType.text)
                {
                    newEntry.Meta.Filename = newEntry.Slug + ".txt";
                    newEntry.Meta.OriginalFilename = newEntry.Slug + ".txt";
                    newEntry.Meta.Mime = "text/plain";
                    newEntry.Meta.Size = newEntry.Value.Length;

                }
                
                _logger.LogDebug($"Transferred entry {newEntry.Slug} with type '{newEntry.Type}' to MongoDB");
                await _entries.ReplaceOneAsync( filter: f => f.Slug == newEntry.Slug, options: new ReplaceOptions {IsUpsert = true}, replacement: newEntry);
            }
            
            Console.WriteLine("Finished transferring data to MongoDB");
        }
    }
}