using Newtonsoft.Json;

namespace Core.Infrastructure.Serializing
{
    public class JsonSerializer
    {
        public string Serialize(object instance)
        {
            return JsonConvert.SerializeObject(instance);
        }
        
        public T Deserialize<T>(string payload)
        {
            return JsonConvert.DeserializeObject<T>(payload);
        }
    }
}