using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Core.Enum
{
    public enum EnumDataRetriveType
    {
        FirstOrDefault,
        List
    }
    public class MapItem
    {
        public Type Type { get; private set; }
        public EnumDataRetriveType DataRetriveType { get; private set; }
        public string PropertyName { get; private set; }

        public MapItem(Type type, EnumDataRetriveType dataRetriveType, string propertyName)
        {
            Type = type;
            DataRetriveType = dataRetriveType;
            PropertyName = propertyName;
        }
    }
}
