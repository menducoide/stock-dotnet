
namespace stock_dotnet.utils.cache{
    public interface ICacheHandler
    {
        void Invalidate(object key);
        void Add(object key, object value, int duration = 0);
         T GetT<T>(object key, out T value);
    }
}