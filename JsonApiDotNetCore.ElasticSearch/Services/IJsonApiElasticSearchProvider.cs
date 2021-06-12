using Nest;

namespace JsonApiDotNetCore.ElasticSearch.Services
{
    public interface IJsonApiElasticSearchProvider
    {
        public bool Enabled { get; }
        
        public ElasticClient Client { get; }

        public string Prefix { get; }
    }
}
