using System.Collections.Generic;
using App.Metrics;
using App.Metrics.Counter;

namespace general_shortener.Metrics
{
    /// <summary>
    /// The metrics registry
    /// </summary>
    public class MetricsRegistry
    {

        /// <summary>
        /// Counter on successful uploads
        /// </summary>
        public static CounterOptions Upload => CreateCounter("upload", Unit.Calls);
        
        /// <summary>
        /// Counter on successful uploads
        /// </summary>
        public static CounterOptions UploadMimetype => CreateCounter("upload mimetype", Unit.Calls);
        
        /// <summary>
        /// Counter on Entries served
        /// </summary>
        public static CounterOptions EntryServed => CreateCounter("serve", Unit.Calls);
        
        /// <summary>
        /// Counter on text served
        /// </summary>
        public static CounterOptions EntryMimetypeServed => CreateCounter("serve mimetype", Unit.Calls);


        
        
        
        /// <summary>
        /// Creates a counter
        /// </summary>
        /// <returns></returns>
        public static CounterOptions CreateCounter(string name, Unit unit, string context = "general-shortener")
        {
            return new()
            {
                Name = name,
                Context = context,
                MeasurementUnit = unit
            };
        }
    }
}