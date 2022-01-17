using System;
using System.Linq;
using JsonApiDotNetCore.ElasticSearch.Utils;
using JsonApiDotNetCore.Queries.Expressions;
using Nest;

namespace JsonApiDotNetCore.ElasticSearch.Queries.Internal.QueryableBuilding
{
    public class ElasticSearchSortsBuilder<TResource> : 
        QueryExpressionVisitor<SortDescriptor<TResource>, SortDescriptor<TResource>> where TResource : class
    {
        public override SortDescriptor<TResource> VisitSortElement(SortElementExpression expression, SortDescriptor<TResource> search)
        {
            // TODO 属性链
            if (expression.TargetAttribute == null || expression.TargetAttribute.Fields.Count != 1)
            {
                throw new NotSupportedException("Lhs not support field chain.");
            }

            if (expression.Count != null)
            {
                throw new NotSupportedException("Aggregating sort not supported.");
            }
            
            var fieldName = PropertyHelper.GetPropName(expression.TargetAttribute.Fields.First().Property.Name);

            if (expression.IsAscending)
            {
                search.Ascending(fieldName);
            }
            else
            {
                search.Descending(fieldName);
            }

            return search;
        }

        public override SortDescriptor<TResource> VisitSort(SortExpression expression, SortDescriptor<TResource> search)
        {
            foreach (var sortElementExpression in expression.Elements)
            {
                Visit(sortElementExpression, search);
            }

            return search;
        }
    }
}
