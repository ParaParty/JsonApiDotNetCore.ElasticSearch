using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.ElasticSearch.Queries.Internal.QueryableBuilding;
using JsonApiDotNetCore.ElasticSearch.Services;
using JsonApiDotNetCore.Errors;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Queries.Expressions;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Nest;

namespace JsonApiDotNetCore.ElasticSearch.Repositories
{
    /// <summary>
    /// JsonApi.Net ElasticSearch Repository
    ///
    /// Only read implemented.
    /// </summary>
    public class ElasticSearchRepository<TResource, TId> : IResourceRepository<TResource, TId>
        where TResource : class, IIdentifiable<TId>
    {
        private readonly IJsonApiElasticSearchProvider _nestProvider;
        private readonly ITargetedFields _targetedFields;
        private readonly IResourceGraph _resourceGraph;
        private readonly IResourceFactory _resourceFactory;
        private readonly IEnumerable<IQueryConstraintProvider> _constraintProviders;
        private readonly IResourceDefinitionAccessor _resourceDefinitionAccessor;

        public ElasticSearchRepository(IJsonApiElasticSearchProvider nestProvider, ITargetedFields targetedFields, IResourceGraph resourceGraph, IResourceFactory resourceFactory,
            IEnumerable<IQueryConstraintProvider> constraintProviders, IResourceDefinitionAccessor resourceDefinitionAccessor)
        {
            _nestProvider = nestProvider;
            _targetedFields = targetedFields;
            _resourceGraph = resourceGraph;
            _resourceFactory = resourceFactory;
            _constraintProviders = constraintProviders;
            _resourceDefinitionAccessor = resourceDefinitionAccessor;

            if (typeof(TId) != typeof(string))
            {
                throw new InvalidConfigurationException("ElasticSearch can only be used for resources with an 'Id' property of type 'string'.");
            }
        }
        
        public Task<IReadOnlyCollection<TResource>> GetAsync(QueryLayer layer, CancellationToken cancellationToken)
        {
            var result = _nestProvider.Client.Search<TResource>(s =>
            {
                var builder = new ElasticSearchQueryableBuilder<TResource>(_nestProvider);
                return builder.Query(s, layer);
            });

            var resultSet = result.Documents;
            
            return Task.FromResult(resultSet);
        }
        
        public Task<int> CountAsync(FilterExpression topFilter, CancellationToken cancellationToken)
        {
            var result = _nestProvider.Client.Search<TResource>(s =>
            {
                var builder = new ElasticSearchQueryableBuilder<TResource>(_nestProvider);
                return builder.Count(s, topFilter);
            });

            var resultCount = result.HitsMetadata.Total.Value > 0x7FFFFFFF ? 0x7FFFFFFF : (int) result.HitsMetadata.Total.Value;

            return Task.FromResult(resultCount);
        }

        public Task<TResource> GetForCreateAsync(Type resourceClrType, TId id, CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

        public Task CreateAsync(TResource resourceFromRequest, TResource resourceForDatabase, CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

        public Task<TResource> GetForUpdateAsync(QueryLayer queryLayer, CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

        public Task UpdateAsync(TResource resourceFromRequest, TResource resourceFromDatabase, CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

        public Task DeleteAsync(TResource resourceFromDatabase, TId id, CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

        public Task SetRelationshipAsync(TResource primaryResource, object secondaryResourceIds, CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

        public Task AddToToManyRelationshipAsync(TResource leftResource, TId leftId, ISet<IIdentifiable> rightResourceIds, CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

        public Task RemoveFromToManyRelationshipAsync(TResource primaryResource, ISet<IIdentifiable> secondaryResourceIds,
            CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }
    }
}
