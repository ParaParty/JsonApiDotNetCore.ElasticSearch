using System.Collections.Generic;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.ElasticSearch.Resources;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using JsonApiDotNetCore.Serialization.Building;
using JsonApiDotNetCore.Serialization.Objects;

namespace JsonApiDotNetCore.ElasticSearch.Serialization.Building
{
    public class IgnoreRelationshipsResponseResourceObjectBuilder: ResponseResourceObjectBuilder
    {
        public IgnoreRelationshipsResponseResourceObjectBuilder(ILinkBuilder linkBuilder, IIncludedResourceObjectBuilder includedBuilder,
            IEnumerable<IQueryConstraintProvider> constraintProviders, IResourceContextProvider resourceContextProvider,
            IResourceDefinitionAccessor resourceDefinitionAccessor, IResourceObjectBuilderSettingsProvider settingsProvider)
            : base(linkBuilder, includedBuilder, constraintProviders, resourceContextProvider, resourceDefinitionAccessor, settingsProvider)
        {
        }

        /// <inheritdoc />
        protected override RelationshipEntry GetRelationshipData(RelationshipAttribute relationship, IIdentifiable resource)
        {
            if (resource is ElasticSearchIdentifiable)
            {
                return null;
            }

            return base.GetRelationshipData(relationship, resource);
        } 
    }
}
