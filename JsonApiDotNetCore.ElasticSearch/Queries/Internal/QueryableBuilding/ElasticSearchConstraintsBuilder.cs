﻿using System;
using System.Collections.Generic;
using System.Linq;
using JsonApiDotNetCore.ElasticSearch.Services;
using JsonApiDotNetCore.ElasticSearch.Utils;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Queries.Expressions;
using Nest;

namespace JsonApiDotNetCore.ElasticSearch.Queries.Internal.QueryableBuilding
{
    public class ElasticSearchConstraintsBuilder<TResource> : 
        QueryExpressionVisitor<QueryContainerDescriptor<TResource>, QueryContainerDescriptor<TResource>> where TResource : class
    {

        private readonly ICollection<Type> _integerType = new HashSet<Type>()
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

        private readonly ICollection<Type> _floatType = new HashSet<Type>()
        {
            typeof(float), typeof(float?),
            typeof(double), typeof(double?),
            typeof(decimal), typeof(decimal?)
        };

        public override QueryContainerDescriptor<TResource> VisitComparison(ComparisonExpression expression,
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
                IntegerQueryContainerDescriptor(expression, search, lhs, rhs, fieldName);
            }
            else if (_floatType.Contains(propType))
            {
                FloatQueryContainerDescriptor(expression, search, lhs, rhs, fieldName);
            }
            else if (propType == typeof(DateTime))
            {
                DateTimeQueryContainerDescriptor(expression, search, lhs, rhs, fieldName);
            }
            else if (propType == typeof(string))
            {
                TermQueryContainerDescriptor(expression, search, lhs, rhs, fieldName);
            } else throw new ArgumentOutOfRangeException();


            return search;
        }

        private QueryContainerDescriptor<TResource> IntegerQueryContainerDescriptor(ComparisonExpression expression,
            QueryContainerDescriptor<TResource> search, ResourceFieldChainExpression lhs, LiteralConstantExpression rhs,
            string fieldName)
        {
            if (!long.TryParse(rhs.Value, out var rhsVal))
            {
                throw new NotSupportedException("Rhs must be a number.");
            }

            if (expression.Operator == ComparisonOperator.Equals)
            {
                search.Term(c =>
                {
                    c.Field(new Field(fieldName));
                    c.Value(rhsVal);
                    return c;
                });
            }
            else
            {
                search.LongRange(c =>
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

            return search;
        }
        

        private QueryContainerDescriptor<TResource> FloatQueryContainerDescriptor(ComparisonExpression expression,
            QueryContainerDescriptor<TResource> search, ResourceFieldChainExpression lhs, LiteralConstantExpression rhs,
            string fieldName)
        {
            if (!double.TryParse(rhs.Value, out var rhsVal))
            {
                throw new NotSupportedException("Rhs must be a number.");
            }

            if (expression.Operator == ComparisonOperator.Equals)
            {
                search.Term(c =>
                {
                    c.Field(new Field(fieldName));
                    c.Value(rhsVal);
                    return c;
                });
            }
            else
            {
                search.Range(c =>
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

            return search;
        }
        
        private QueryContainerDescriptor<TResource> DateTimeQueryContainerDescriptor(ComparisonExpression expression,
            QueryContainerDescriptor<TResource> search, ResourceFieldChainExpression lhs, LiteralConstantExpression rhs,
            string fieldName)
        {
            if (!DateTime.TryParse(rhs.Value, out var rhsVal))
            {
                throw new NotSupportedException("Rhs must be a number.");
            }

            if (expression.Operator == ComparisonOperator.Equals)
            {
                search.Term(c =>
                {
                    c.Field(new Field(fieldName));
                    c.Value(rhsVal);
                    return c;
                });
            }
            else
            {
                search.DateRange(c =>
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

            return search;
        }
        
        private QueryContainerDescriptor<TResource> TermQueryContainerDescriptor(ComparisonExpression expression,
            QueryContainerDescriptor<TResource> search, ResourceFieldChainExpression lhs, LiteralConstantExpression rhs,
            string fieldName)
        {
            var rhsVal = rhs.Value;
            
            if (expression.Operator == ComparisonOperator.Equals)
            {
                search.Term(c =>
                {
                    c.Field(new Field(fieldName));
                    c.Value(rhsVal);
                    return c;
                });
            }
            else
            {
                search.TermRange(c =>
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

            return search;
        }


        public override QueryContainerDescriptor<TResource> VisitMatchText(MatchTextExpression expression,
            QueryContainerDescriptor<TResource> search)
        {
            // TODO 属性链
            if (expression.TargetAttribute.Fields.Count != 1)
            {
                throw new NotSupportedException("Lhs not support field chain.");
            }
            
            var fieldName = PropertyHelper.GetPropName(expression.TargetAttribute.Fields.First().Property.Name);

            search.Match(c =>
            {
                c.Field(fieldName);
                c.Fuzziness(Fuzziness.Auto);
                c.Query(expression.TextValue.Value);
                return c;
            });

            return search;
        }

        public override QueryContainerDescriptor<TResource> VisitEqualsAnyOf(EqualsAnyOfExpression expression,
            QueryContainerDescriptor<TResource> search)
        {
            // TODO 属性链
            if (expression.TargetAttribute.Fields.Count != 1)
            {
                throw new NotSupportedException("Lhs not support field chain.");
            }
            
            var fieldName = PropertyHelper.GetPropName(expression.TargetAttribute.Fields.First().Property.Name);

            search.Terms(c =>
            {
                c.Field(fieldName);
                c.Terms(expression.Constants.Select<LiteralConstantExpression, string>(s => s.Value).ToArray());
                return c;
            });
            
            return search;
        }

    }
}
