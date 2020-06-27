using System.ComponentModel;
using System.Data;

namespace BatchProductUpdate.Api.Extensions
{
    public static class DataRowExtensions
    {
        public static T GetValue<T>(this DataRow target, string key)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));

            var result = converter.ConvertFrom(target[key].ToString());

            if (result == null)
            {
                return default;
            }

            return (T)result;
        }
    }
}
