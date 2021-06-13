using JsonApiDotNetCore.ElasticSearch.Services;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Queries.Expressions;
using Nest;

namespace JsonApiDotNetCore.ElasticSearch.Queries.Internal.QueryableBuilding
{
    public class
        ElasticSearchQueryableBuilder<TResource> where TResource : class
    {
        private readonly IJsonApiElasticSearchProvider _nestService;
        private readonly QueryLayer _layer;

        public ElasticSearchQueryableBuilder(IJsonApiElasticSearchProvider nestService, QueryLayer layer)
        {
            _nestService = nestService;
            _layer = layer;
        }

        public SearchDescriptor<TResource> Build(SearchDescriptor<TResource> searchDescriptor)
        {
            searchDescriptor.Index($"{_nestService.Prefix}log_chat_general"); // TODO

            if (_layer.Filter != null)
            {
                var builder = new ElasticSearchConstraintsBuilder<TResource>();
                searchDescriptor.Query(s => builder.Visit(_layer.Filter, s));
            }

            if (_layer.Sort != null)
            {
                var builder = new ElasticSearchSortsBuilder<TResource>();
                searchDescriptor.Sort(s => builder.Visit(_layer.Sort, s));
            }

            if (_layer.Pagination != null)
            {
                searchDescriptor.Take(_layer.Pagination.PageSize.Value);
                searchDescriptor.Skip(
                    _layer.Pagination.PageSize.Value *
                    (_layer.Pagination.PageNumber.OneBasedValue - 1)
                );
            }

            return searchDescriptor;
        }
    }
}
