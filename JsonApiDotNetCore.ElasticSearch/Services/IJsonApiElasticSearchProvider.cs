using Nest;

namespace JsonApiDotNetCore.ElasticSearch.Services
{
    /// <summary>
    /// JsonApi.Net ElasticSearch Provider
    ///
    /// This interface was used for JsonApi.Net ElasticSearch repository.
    /// </summary>
    public interface IJsonApiElasticSearchProvider
    {
        /// <summary>
        /// The ElasticClient used for JsonApi.Net .
        /// </summary>
        public ElasticClient Client { get; }

        /// <summary>
        /// Document name prefix.
        /// </summary>
        public string Prefix { get; }
    }
}
