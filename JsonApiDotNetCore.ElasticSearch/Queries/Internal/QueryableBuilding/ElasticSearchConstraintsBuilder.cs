using System;
using System.Collections.Generic;
using System.Linq;
using JsonApiDotNetCore.ElasticSearch.Utils;
using JsonApiDotNetCore.Queries.Expressions;
using Nest;

namespace JsonApiDotNetCore.ElasticSearch.Queries.Internal.QueryableBuilding
{
    public class ElasticSearchConstraintsBuilder<TResource> : 
        QueryExpressionVisitor<QueryContainerDescriptor<TResource>, QueryContainer> where TResource : class
    {

        private readonly ICollection<Type> _integerType = new HashSet<Type>
        {
            typeof(sbyte), typeof(sbyte?),
            typeof(byte), typeof(byte?),
            typeof(short), typeof(short?),
            typeof(ushort), typeof(ushort?),
            typeof(int), typeof(int?),
            typeof(uint), typeof(uint?),
            typeof(long), typeof(long?),
            typeof(ulong), typeof(ulong?),
        };

        private readonly ICollection<Type> _floatType = new HashSet<Type>
        {
            typeof(float), typeof(float?),
            typeof(double), typeof(double?),
            typeof(decimal), typeof(decimal?)
        };

        public override QueryContainer VisitComparison(ComparisonExpression expression,
            QueryContainerDescriptor<TResource> search)
        {
            if (expression.Left is not ResourceFieldChainExpression lhs)
            {
                throw new NotSupportedException("Lhs only support field name.");
            }

            // TODO 属性链
            if (lhs.Fields.Count != 1)
            {
                throw new NotSupportedException("Lhs not support field chain.");
            }

            if (expression.Right is not LiteralConstantExpression rhs)
            {
                throw new NotSupportedException("Rhs only support literal constant.");
            }

            var propType = lhs.Fields.First().Property.PropertyType;
            var fieldName = PropertyHelper.GetPropName(lhs.Fields.First().Property.Name);
            
            if (fieldName == "id")
            {
                fieldName = long.TryParse(rhs.Value, out var rhsVal) ? "databaseId" : $"_{fieldName}";
            }

            // 这个 database Id 指的是关系型数据库里这一条记录所对应的数据库自增主键
            if (_integerType.Contains(propType) || fieldName == "databaseId")
            {
                return IntegerQueryContainerDescriptor(expression, search, lhs, rhs, fieldName);
            }

            if (_floatType.Contains(propType))
            {
                return FloatQueryContainerDescriptor(expression, search, lhs, rhs, fieldName);
            }

            if (propType == typeof(DateTime))
            {
                return DateTimeQueryContainerDescriptor(expression, search, lhs, rhs, fieldName);
            }

            if (propType == typeof(string))
            {
                return TermQueryContainerDescriptor(expression, search, lhs, rhs, fieldName);
            }

            throw new ArgumentOutOfRangeException();


        }

        private QueryContainer IntegerQueryContainerDescriptor(ComparisonExpression expression,
            QueryContainerDescriptor<TResource> search, ResourceFieldChainExpression lhs, LiteralConstantExpression rhs,
            string fieldName)
        {
            if (!long.TryParse(rhs.Value, out var rhsVal))
            {
                throw new NotSupportedException("Rhs must be a number.");
            }

            if (expression.Operator == ComparisonOperator.Equals)
            {
                return search.Term(c =>
                {
                    c.Field(new Field(fieldName));
                    c.Value(rhsVal);
                    return c;
                });
            }
            else
            {
                return search.LongRange(c =>
                {
                    c.Field(new Field(fieldName));
                    switch (expression.Operator)
                    {
                        case ComparisonOperator.Equals:
                            throw new ArgumentOutOfRangeException();
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
        }
        

        private QueryContainer FloatQueryContainerDescriptor(ComparisonExpression expression,
            QueryContainerDescriptor<TResource> search, ResourceFieldChainExpression lhs, LiteralConstantExpression rhs,
            string fieldName)
        {
            if (!double.TryParse(rhs.Value, out var rhsVal))
            {
                throw new NotSupportedException("Rhs must be a number.");
            }

            if (expression.Operator == ComparisonOperator.Equals)
            {
                return search.Term(c =>
                {
                    c.Field(new Field(fieldName));
                    c.Value(rhsVal);
                    return c;
                });
            }
            else
            {
                return search.Range(c =>
                {
                    c.Field(new Field(fieldName));
                    switch (expression.Operator)
                    {
                        case ComparisonOperator.Equals:
                            throw new ArgumentOutOfRangeException();
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
        }
        
        private QueryContainer DateTimeQueryContainerDescriptor(ComparisonExpression expression,
            QueryContainerDescriptor<TResource> search, ResourceFieldChainExpression lhs, LiteralConstantExpression rhs,
            string fieldName)
        {
            if (!DateTime.TryParse(rhs.Value, out var rhsVal))
            {
                throw new NotSupportedException("Rhs must be a number.");
            }

            if (expression.Operator == ComparisonOperator.Equals)
            {
                return search.Term(c =>
                {
                    c.Field(new Field(fieldName));
                    c.Value(rhsVal);
                    return c;
                });
            }
            else
            {
                return search.DateRange(c =>
                {
                    c.Field(new Field(fieldName));
                    switch (expression.Operator)
                    {
                        case ComparisonOperator.Equals:
                            throw new ArgumentOutOfRangeException();
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
        }
        
        private QueryContainer TermQueryContainerDescriptor(ComparisonExpression expression,
            QueryContainerDescriptor<TResource> search, ResourceFieldChainExpression lhs, LiteralConstantExpression rhs,
            string fieldName)
        {
            var rhsVal = rhs.Value;
            
            if (expression.Operator == ComparisonOperator.Equals)
            {
                return search.Term(c =>
                {
                    c.Field(new Field(fieldName));
                    c.Value(rhsVal);
                    return c;
                });
            }
            else
            {
                return search.TermRange(c =>
                {
                    c.Field(new Field(fieldName));
                    switch (expression.Operator)
                    {
                        case ComparisonOperator.Equals:
                            throw new ArgumentOutOfRangeException();
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
        }


        public override QueryContainer VisitMatchText(MatchTextExpression expression,
            QueryContainerDescriptor<TResource> search)
        {
            // TODO 属性链
            if (expression.TargetAttribute.Fields.Count != 1)
            {
                throw new NotSupportedException("Lhs not support field chain.");
            }
            
            var fieldName = PropertyHelper.GetPropName(expression.TargetAttribute.Fields.First().Property.Name);

            // TODO 对 StartsWith EndsWith 进行特化。使得使用 StartsWith 时在文档开头匹配到的结果优先度比在文档末尾匹配到的结果高。
            return search.Match(c =>
            {
                c.Field(fieldName);
                c.Fuzziness(Fuzziness.Auto);
                c.Query(expression.TextValue.Value);
                return c;
            });
        }

        public override QueryContainer VisitEqualsAnyOf(EqualsAnyOfExpression expression,
            QueryContainerDescriptor<TResource> search)
        {
            // TODO 属性链
            if (expression.TargetAttribute.Fields.Count != 1)
            {
                throw new NotSupportedException("Lhs not support field chain.");
            }
            
            var fieldName = PropertyHelper.GetPropName(expression.TargetAttribute.Fields.First().Property.Name);

            return search.Terms(c =>
            {
                c.Field(fieldName);
                c.Terms(expression.Constants.Select(s => s.Value).ToArray());
                return c;
            });
        }

        public override QueryContainer VisitLogical(LogicalExpression expression, QueryContainerDescriptor<TResource> search)
        {
            QueryContainer ret = null;
            
            switch (expression.Operator)
            {
                case LogicalOperator.And:
                    foreach (var queryExpression in expression.Terms)
                    {
                        if (ret == null)
                        {
                            ret = Visit(queryExpression, search);
                        }
                        else
                        {
                            ret = ret && Visit(queryExpression, search); 
                        }
                    }
                    break;
                case LogicalOperator.Or:
                    foreach (var queryExpression in expression.Terms)
                    {
                        if (ret == null)
                        {
                            ret = Visit(queryExpression, search);
                        }
                        else
                        {
                            ret = ret || Visit(queryExpression, search); 
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return ret;
        }
        
        public override QueryContainer VisitNot(NotExpression expression, QueryContainerDescriptor<TResource> search)
        {
            throw new NotSupportedException("Unary operator \"not\" is not supported.");
        }
        
        public override QueryContainer VisitResourceFieldChain(ResourceFieldChainExpression expression, QueryContainerDescriptor<TResource> search)
        {
            throw new NotSupportedException("FieldChain not supported.");
        }
        
        public override QueryContainer VisitCollectionNotEmpty(CollectionNotEmptyExpression expression, QueryContainerDescriptor<TResource> search)
        {
            throw new NotSupportedException("Relationship operator not supported.");
        }

    }
}
