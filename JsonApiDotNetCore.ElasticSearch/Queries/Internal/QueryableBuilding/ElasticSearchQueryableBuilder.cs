
using System;
using System.Linq;
using JsonApiDotNetCore.ElasticSearch.Services;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Queries.Expressions;
using Nest;

namespace JsonApiDotNetCore.ElasticSearch.Queries.Internal.QueryableBuilding
{
    public class ElasticSearchQueryableBuilder<TResource> : QueryExpressionVisitor<QueryContainerDescriptor<TResource>, QueryContainerDescriptor<TResource>> where TResource : class
    {
        private readonly IJsonApiElasticSearchProvider _nestService;
        private readonly QueryLayer _layer;

        public ElasticSearchQueryableBuilder(IJsonApiElasticSearchProvider nestService,  QueryLayer layer)
        {
            _nestService = nestService;
            _layer = layer;
        }

        public SearchDescriptor<TResource> Build(SearchDescriptor<TResource> searchDescriptor)
        {
            searchDescriptor.Index($"{_nestService.Prefix}log_chat_general"); // TODO
            if (_layer.Filter != null)
            {
                searchDescriptor.Query(s => Visit(_layer.Filter, s));
            }
            return searchDescriptor;
        }
        
        public override QueryContainerDescriptor<TResource> VisitComparison(ComparisonExpression expression, QueryContainerDescriptor<TResource> search)
        {
            if (expression.Left is not ResourceFieldChainExpression lhs)
            {
                throw new NotSupportedException("Lhs only support field name.");
            }
            
            if (expression.Right is not LiteralConstantExpression rhs)
            {
                throw new NotSupportedException("Rhs only support literal constant.");
            }
            var fieldName = lhs.Fields.First().Property.Name;
            if ('A' <= fieldName[0] && fieldName[0] <= 'Z')
            {
                fieldName = (char)(fieldName[0] - 'A' + 'a') + fieldName[1..];
            }

            if (expression.Operator == ComparisonOperator.Equals)
            {
                search.Term(c =>
                {
                    c.Field(new Field(fieldName));
                    c.Value(rhs.Value);
                    return c;
                });
            } else {
                if (!double.TryParse(rhs.Value, out var rhsVal))
                {
                    throw new NotSupportedException("Rhs must be a number.");
                }
                
                search.Range(c =>
                {
                    c.Field(new Field(fieldName));
                    switch (expression.Operator)
                    {
                        case ComparisonOperator.Equals:
                            break;
                        case ComparisonOperator.GreaterThan:
                            c.GreaterThan(rhsVal);
                            break;
                        case ComparisonOperator.GreaterOrEqual:
                            c.GreaterThanOrEquals(rhsVal);
                            break;
                        case ComparisonOperator.LessThan:
                            c.LessThan(rhsVal);
                            break;
                        case ComparisonOperator.LessOrEqual:
                            c.LessThanOrEquals(rhsVal);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
     
                    return c;
                });
            }
            return search;
        }

    }
}
