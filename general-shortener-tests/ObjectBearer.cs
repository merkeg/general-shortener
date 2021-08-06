using System;

namespace general_shortener_tests
{
    public class ObjectBearer<T> : IDisposable where T : class, new()
    {
        
        public T Object { get; set; }

        public ObjectBearer()
        {
            Object = new T();
        }

        public void Dispose()
        {
            Object = null;
        }
    }
}