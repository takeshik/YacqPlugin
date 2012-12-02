using System;
using System.Linq;
using System.Linq.Expressions;
using Dulcet.Twitter;
using Inscribe.Common;
using Inscribe.Configuration;
using Inscribe.Filter.Core;
using XSpect.Yacq.Expressions;
using XSpect.Yacq.Symbols;

namespace YacqPlugin
{
    partial class YacqFilter
    {
        internal static class Symbols
        {
            [YacqSymbol(DispatchTypes.None, null)]
            public static Expression Missing(DispatchExpression e, SymbolTable s, Type t)
            {
                Type type;
                if (e.DispatchType == DispatchTypes.Method
                    && !s.ExistsKey(DispatchTypes.Method, e.Name)
                    && (type = FilterRegistrant.GetFilter(e.Name).FirstOrDefault()) != null
                )
                {
                    return YacqExpression.Dispatch(
                        s,
                        DispatchTypes.Constructor,
                        YacqExpression.TypeCandidate(type),
                        null,
                        e.Arguments
                    )
                        .Method(s, "Filter", YacqExpression.Identifier(s, "it"));
                }
                return DispatchExpression.DefaultMissing(e, s, t);
            }

            [YacqSymbol(DispatchTypes.Method, "from")]
            public static Expression RangeFrom(DispatchExpression e, SymbolTable s, Type t)
            {
                return YacqExpression.TypeCandidate(typeof(LongRange))
                    .Method(s, "FromFromValue", e.Arguments[0]);
            }

            [YacqSymbol(DispatchTypes.Method, "to")]
            public static Expression RangeTo(DispatchExpression e, SymbolTable s, Type t)
            {
                return YacqExpression.TypeCandidate(typeof(LongRange))
                    .Method(s, "FromToValue", e.Arguments[0]);
            }

            [YacqSymbol(DispatchTypes.Method, typeof(Int32), "to")]
            [YacqSymbol(DispatchTypes.Method, typeof(Int64), "to")]
            public static Expression RangeFromTo(DispatchExpression e, SymbolTable s, Type t)
            {
                return YacqExpression.TypeCandidate(typeof(LongRange))
                    .Method(s, "FromBetweenValues", e.Left, e.Arguments[0]);
            }

            [YacqSymbol(DispatchTypes.Method, "exact")]
            public static Expression RangeExact(DispatchExpression e, SymbolTable s, Type t)
            {
                return YacqExpression.TypeCandidate(typeof(LongRange))
                    .Method(s, "FromPivotValue", e.Arguments[0]);
            }

            [YacqSymbol(DispatchTypes.Method, "tab")]
            public static Expression Tab(DispatchExpression e, SymbolTable s, Type t)
            {
                return YacqExpression.TypeCandidate(typeof(Setting))
                    .Member(s, "Instance")
                    .Member(s, "StateProperty")
                    .Member(s, "TabInformations")
                    .Method(s, "SelectMany",
                        YacqExpression.Function(s, "\\",
                            YacqExpression.Vector(s, YacqExpression.Identifier(s, "e")),
                            YacqExpression.Identifier(s, "e")
                        )
                    )
                    .Method(s, "First",
                        YacqExpression.LambdaList(s,
                            YacqExpression.Identifier(s, "=="),
                            YacqExpression.Identifier(s, "$0").Member(s, "Name"),
                            e.Arguments[0]
                        )
                    )
                    .Member(s, "TweetSources")
                    .Method(s, "Single");
            }
        }
    }
}
