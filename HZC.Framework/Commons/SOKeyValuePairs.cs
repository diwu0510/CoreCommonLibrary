using System.Collections.Generic;

namespace HZC.Framework
{
    public class SoKeyValuePairs : List<KeyValuePair<string, object>>
    {
        public static SoKeyValuePairs New()
        {
            return new SoKeyValuePairs();
        }

        public SoKeyValuePairs Add(string key, object value)
        {
            Add(new KeyValuePair<string, object>(key, value));
            return this;
        }
    }
}
