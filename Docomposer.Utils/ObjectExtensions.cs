using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Docomposer.Utils;

public static class ObjectExtensions
{
    public static T DeepCopy<T>(this T @object){
        var objectSerialized = JsonConvert.SerializeObject(@object);
        return JsonConvert.DeserializeObject<T>(objectSerialized);
    }
    
    public static List<T> DeepCopy<T>(this IEnumerable<T> listOfObjects) {
        return listOfObjects.Select(o => o.DeepCopy()).ToList();
    }
}