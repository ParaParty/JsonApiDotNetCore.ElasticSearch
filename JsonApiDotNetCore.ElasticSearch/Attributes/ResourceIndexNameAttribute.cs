using System;

namespace JsonApiDotNetCore.ElasticSearch.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ResourceIndexNameAttribute : Attribute
    {
        public string IndexName { get; private set; } = "";
        
        public ResourceIndexNameAttribute(string indexName)
        {
            IndexName = indexName;
        }
    }
}
