using System;

namespace JsonApiDotNetCore.ElasticSearch.Attributes
{
    /// <summary>
    /// Index name in ElasticSearch.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ResourceIndexNameAttribute : Attribute
    {
        /// <summary>
        /// Index name.
        ///
        /// Final index name will be present as <code>$"{IJsonApiElasticSearchProvider.Prefix}{indexName}"</code>
        /// </summary>
        public string IndexName { get; private set; }
        
        public ResourceIndexNameAttribute(string indexName)
        {
            IndexName = indexName;
        }
    }
}
