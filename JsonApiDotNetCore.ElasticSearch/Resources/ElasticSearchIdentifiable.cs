using JsonApiDotNetCore.Resources;
using Nest;

namespace JsonApiDotNetCore.ElasticSearch.Resources
{
    /// <summary>
    /// JsonApi.Net ElasticSearch Identifiable
    ///
    /// Only string accepted.
    /// </summary>
    public abstract class ElasticSearchIdentifiable : IIdentifiable<string>
    {
        public virtual string Id { get; set; }
        
        [Ignore]
        public string StringId
        {
            get => Id;
            set => Id = value;
        }

        [Ignore]
        public string LocalId { get; set; }
    }
}
