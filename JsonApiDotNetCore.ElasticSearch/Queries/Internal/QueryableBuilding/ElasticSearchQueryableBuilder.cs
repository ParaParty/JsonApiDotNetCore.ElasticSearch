using System;
using System.Linq;
using JsonApiDotNetCore.ElasticSearch.Attributes;
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

        public ElasticSearchQueryableBuilder(IJsonApiElasticSearchProvider nestService)
        {
            _nestService = nestService;
        }

        private string GetIndexName()
        {
            var indexName = "";
            if (System.Attribute.IsDefined(typeof(TResource), typeof(ResourceIndexNameAttribute)))
            {
                var attrs = System.Attribute.GetCustomAttributes(typeof(TResource), typeof(ResourceIndexNameAttribute));
                for (int i = 0; i < attrs.Length; i++)
                {
                    indexName = (attrs[i] as ResourceIndexNameAttribute)?.IndexName ?? "";
                }
            }
            
            if (indexName == null || indexName.Trim().Length == 0){
                throw new ArgumentException("Resource didn't set resource index name.");
            }

            return indexName;
        }

        public SearchDescriptor<TResource> Query(SearchDescriptor<TResource> searchDescriptor, QueryLayer layer)
        {
            var indexName = GetIndexName();
            searchDescriptor.Index($"{_nestService.Prefix}{indexName}"); // TODO

            if (layer.Filter != null)
            {
                var builder = new ElasticSearchConstraintsBuilder<TResource>();
                searchDescriptor.Query(s => builder.Visit(layer.Filter, s));
            }

            if (layer.Sort != null)
            {
                if (layer.Sort.Elements.Count != 1 ||
                    layer.Sort.Elements.First().TargetAttribute!.Fields.Count != 1 ||
                    layer.Sort.Elements.First().TargetAttribute!.Fields.First().Property.Name.ToLower() != "id")
                {
                    var builder = new ElasticSearchSortsBuilder<TResource>();
                    searchDescriptor.Sort(s => builder.Visit(layer.Sort, s));
                }
            }

            if (layer.Pagination != null)
            {
                searchDescriptor.Take(layer.Pagination.PageSize!.Value);
                searchDescriptor.Skip(
                    layer.Pagination.PageSize.Value *
                    (layer.Pagination.PageNumber.OneBasedValue - 1)
                );
            }

            return searchDescriptor;
        }

        public SearchDescriptor<TResource> Count(SearchDescriptor<TResource> searchDescriptor, FilterExpression topFilter)
        {
            var indexName = GetIndexName();
            searchDescriptor.Index($"{_nestService.Prefix}{indexName}"); // TODO

            if (topFilter != null)
            {
                var builder = new ElasticSearchConstraintsBuilder<TResource>();
                searchDescriptor.Query(s => builder.Visit(topFilter, s));
            }
            
            return searchDescriptor;
        }
    }
}
