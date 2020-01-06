using System;
using System.Collections.Generic;
using System.Text;

namespace KRFCommon.Helpers
{
    public class CaseInsensitiveDictionary<ValueType> : Dictionary<string, ValueType>
    {
        public CaseInsensitiveDictionary() : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
