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
    public class ElasticSearchRepository<TResource, TId> : IResourceRepository<TResource, TId>
        where TResource : class, IIdentifiable<TId>
    {
        private readonly IJsonApiElasticSearchProvider _nestProvider;
        private readonly ITargetedFields _targetedFields;
        private readonly IResourceContextProvider _resourceContextProvider;
        private readonly IResourceFactory _resourceFactory;
        private readonly IEnumerable<IQueryConstraintProvider> _constraintProviders;

        public ElasticSearchRepository(IJsonApiElasticSearchProvider nestProvider, ITargetedFields targetedFields, IResourceContextProvider resourceContextProvider,
            IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders)
        {
            _nestProvider = nestProvider;
            _targetedFields = targetedFields;
            _resourceContextProvider = resourceContextProvider;
            _resourceFactory = resourceFactory;
            _constraintProviders = constraintProviders;

            if (typeof(TId) != typeof(string))
            {
                throw new InvalidConfigurationException("ElasticSearch can only be used for resources with an 'Id' property of type 'string'.");
            }
        }
        
        public Task<IReadOnlyCollection<TResource>> GetAsync(QueryLayer layer, CancellationToken cancellationToken)
        {
            var result = _nestProvider.Client.Search<TResource>(s =>
            {
                var builder = new ElasticSearchQueryableBuilder<TResource>(_nestProvider ,layer);
                return builder.Build(s);
            });

            var resultSet = result.Documents;
            
            return Task.FromResult(resultSet);
        }
        
        public Task<int> CountAsync(FilterExpression topFilter, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<TResource> GetForCreateAsync(TId id, CancellationToken cancellationToken)
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

        public Task DeleteAsync(TId id, CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

        public Task SetRelationshipAsync(TResource primaryResource, object secondaryResourceIds, CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

        public Task AddToToManyRelationshipAsync(TId primaryId, ISet<IIdentifiable> secondaryResourceIds, CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

        public Task RemoveFromToManyRelationshipAsync(TResource primaryResource, ISet<IIdentifiable> secondaryResourceIds,
            CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }
    }

    /// <summary>
    /// Do not use. This type exists solely to produce a proper error message when trying to use MongoDB with a non-string Id.
    /// </summary>
    public sealed class ElasticSearchRepository<TResource> : ElasticSearchRepository<TResource, int>, IResourceRepository<TResource>
        where TResource : class, IIdentifiable<int>
    {
        public ElasticSearchRepository(IJsonApiElasticSearchProvider nestProvider, ITargetedFields targetedFields, IResourceContextProvider resourceContextProvider,
            IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders)
            : base(nestProvider, targetedFields, resourceContextProvider, resourceFactory, constraintProviders)
        {
        }
    }
}
