using System.Threading.Tasks;
using StackExchange.Redis;

namespace general_shortener.Services
{
    /// <summary>
    /// Transfer service
    /// </summary>
    public interface ITransferService
    {
        /// <summary>
        /// Redis connection
        /// </summary>
        public ConnectionMultiplexer RedisConnection { get; }
        
        /// <summary>
        /// Redis database connection
        /// </summary>
        public IDatabase RedisDatabase { get; }
        
        /// <summary>
        /// Is transfer activated in config
        /// </summary>
        public bool TransferActivated { get; }
        
        /// <summary>
        /// Transfer data from the redis (>v1) model to the mongo (under v1) model 
        /// </summary>
        /// <returns></returns>
        public Task TransferData();
        
    }
}