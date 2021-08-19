namespace KRFCommon.CustomTypes
{
    using System;
    using System.Collections.Generic;

    public class CaseInsensitiveDictionary<ValueType> : Dictionary<string, ValueType>
    {
        public CaseInsensitiveDictionary() : base( StringComparer.OrdinalIgnoreCase )
        {
        }
    }
}
