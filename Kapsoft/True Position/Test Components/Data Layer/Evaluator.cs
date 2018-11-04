using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace TruePosition.Test.DataLayer
{
    public enum ExpressionType
    {
        Simple,
        Advanced
    }

    public sealed class Evaluator
    {
        public string RawExpression { get; private set; }
        private ExpressionType ExType { get; set; }
        private Expression Ex { get; set; }
        private ParameterExpression Parameter = null;

        private Func<int, bool> intDelegate = null;
        private Func<double, bool> doubleDelegate = null;
        private Func<string, bool> stringDelegate = null;

        private void Initialize(ExpressionType type, string expression)
        {
            ExType = type;
            RawExpression = expression;

            if (!string.IsNullOrEmpty(expression))
            {
                Ex = Parse(expression);
                if (Parameter != null)
                {
                    switch (Type.GetTypeCode(Parameter.Type))
                    {
                        case TypeCode.Int32:
                            intDelegate = Expression.Lambda<Func<int, bool>>(Ex, Parameter).Compile();
                            break;
                        case TypeCode.Double:
                            doubleDelegate = Expression.Lambda<Func<double, bool>>(Ex, Parameter).Compile();
                            break;
                        case TypeCode.String:
                            stringDelegate = Expression.Lambda<Func<string, bool>>(Ex, Parameter).Compile();
                            break;
                    }
                }
            }
        }

        public Evaluator(string expression) : this(ExpressionType.Advanced, expression) { }
        public Evaluator(ExpressionType type, string expression)
        {
            Initialize(type, expression);
        }

        public bool Evaluate(ValueHelper value)
        {
            bool result = false;

            if (Parameter != null)
            {
                switch (Type.GetTypeCode(Parameter.Type))
                {
                    case TypeCode.Int32:
                        result = intDelegate.Invoke(value.GetInt32Safe());
                        break;
                    case TypeCode.Double:
                        result = doubleDelegate.Invoke(value.GetDouble());
                        break;
                    case TypeCode.String:
                        result = stringDelegate.Invoke(value.GetString());
                        break;
                }
            }

            return result;
        }

        public string Constant
        {
            get
            {
                if (Type.GetTypeCode(Parameter.Type) != TypeCode.String)
                    throw new InvalidOperationException("Only the constant within a string expression can be returned.");

                string constant = null;
                if (Ex.NodeType == System.Linq.Expressions.ExpressionType.Call)
                {
                    constant = (string)(from e in ((MethodCallExpression)Ex).Arguments
                                        where e.NodeType == System.Linq.Expressions.ExpressionType.Constant
                                        select ((ConstantExpression)e).Value).SingleOrDefault();
                }
                else if (((BinaryExpression)Ex).Right.NodeType == System.Linq.Expressions.ExpressionType.Constant)
                {
                    constant = (string)((ConstantExpression)((BinaryExpression)Ex).Right).Value;
                }
                else
                {
                    constant = (string)((ConstantExpression)((BinaryExpression)Ex).Left).Value;
                }

                return constant;
            }
        }

        private static bool Same(string value)
        {
            string same = new string(value[0], value.Length);
            return value == same;
        }

        private Expression BuildConstant(string expression)
        {
            return Expression.Constant(expression.Contains("'") ? (object)expression.Trim().Remove(expression.Length - 1, 1).Remove(0, 1) : expression.Contains(".") ? (object)Convert.ToDouble(expression) : (object)Convert.ToInt32(expression),
                                       expression.Contains("'") ? typeof(string) : expression.Contains(".") ? typeof(double) : typeof(int));
        }

        private Expression Parse(string expression)
        {
            expression = expression.Trim();

            if (expression.Contains("value"))
            {
                string[] operands = expression.Trim().Split(new string[] { "value" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid expression. An expression can only contain one value evaluation.");
                }

                return Parse(operands[0]);
            }
            // DESIGN NOTE:
            // To allow operators to potentially exist in string expressions, move string processing to top of operator precedence.
            else if (expression.StartsWith("'"))
            {
                return BuildConstant(expression);
            }
            else if (expression.Contains("like"))
            {
                bool not = false;
                if (expression.Contains("not"))
                {
                    not = true;
                    expression = expression.Replace("not", "").Trim();
                }

                string[] operands = expression.Trim().Split(new string[] { "like" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid like expression. Too many operands.");
                }
                // TEST ONLY...
                // Why was this done ????
                //else if ((ExType == ExpressionType.Simple) && (operands[0].IndexOfAny(VBHelpers.AdvancedWildcards) != -1))
                //{
                //    int i = operands[0].IndexOfAny(VBHelpers.AdvancedWildcards);
                //    if ((operands[0].Length == 1) || (operands[0][i] != operands[0][i + 1]))
                //        throw new Exception("Invalid simple expression. A simple expression may only contain " +
                //                            VBHelpers.SimpleWildcards.Aggregate(string.Empty,
                //                                                                (agg, next) => string.IsNullOrEmpty(agg) ? next.ToString() : agg + ", " + next.ToString()) +
                //                            " wildcards. To use an advanced wildcard character within a simple expression, duplicate the wildcard. (i.e. value like '-->' for the - wildcard.)");
                //}

                Expression ex = Parse(operands[0]);
                if (ex.Type != typeof(string))
                {
                    throw new Exception("Invalid like expression. Like can only be used with string expressions.");
                }

                if (Parameter == null)
                    Parameter = Expression.Parameter(ex.Type, "value");

                Expression result = null;
                if (expression.StartsWith("like"))
                {
                    result = Expression.Call(typeof(VBHelpers).GetMethod("Match"), Parameter, ex);
                }
                else
                {
                    result = Expression.Call(typeof(VBHelpers).GetMethod("Match"), ex, Parameter);
                }

                return not ? Expression.Not(result) : result;
            }
            else if (expression.Contains("same"))
            {
                bool not = false;
                if (expression.Contains("not"))
                {
                    not = true;
                    expression = expression.Replace("not", "").Trim();
                }

                string[] operands = expression.Trim().Split(new string[] { "same" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 0)
                {
                    throw new Exception("Invalid same expression. Too many operands.");
                }

                if (Parameter == null)
                    Parameter = Expression.Parameter(typeof(string), "value");

                Expression result = Expression.Call(typeof(Evaluator).GetMethod("Same", BindingFlags.NonPublic | BindingFlags.Static), Parameter);
                return not ? Expression.Not(result) : result;
            }
            else if (expression.Contains("validesn"))
            {
                string[] operands = expression.Trim().Split(new string[] { "validesn" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 0)
                {
                    throw new Exception("Invalid validesn expression. Too many operands.");
                }

                if (Parameter == null)
                    Parameter = Expression.Parameter(typeof(string), "value");

                return Expression.Call(typeof(VBHelpers).GetMethod("ValidESN"), Parameter);
            }
            else if (expression.Contains("lenge"))
            {
                string[] operands = expression.Trim().Split(new string[] { "lenge" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid lenge expression. Too many operands.");
                }

                Expression ex = Parse(operands[0]);
                if (ex.Type != typeof(int))
                {
                    throw new Exception("Invalid lenge expression. Len can only be compared against an integer.");
                }

                if (Parameter == null)
                    Parameter = Expression.Parameter(typeof(string), "value");

                if (expression.StartsWith("lenge"))
                {
                    return Expression.GreaterThanOrEqual(Expression.Call(typeof(VBHelpers).GetMethod("Length"), Parameter), ex);
                }
                else
                {
                    return Expression.GreaterThanOrEqual(ex, Expression.Call(typeof(VBHelpers).GetMethod("Length"), Parameter));
                }
            }
            else if (expression.Contains("lenle"))
            {
                string[] operands = expression.Trim().Split(new string[] { "lenle" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid lenle expression. Too many operands.");
                }

                Expression ex = Parse(operands[0]);
                if (ex.Type != typeof(int))
                {
                    throw new Exception("Invalid lenle expression. Len can only be compared against an integer.");
                }

                if (Parameter == null)
                    Parameter = Expression.Parameter(typeof(string), "value");

                if (expression.StartsWith("lenle"))
                {
                    return Expression.LessThanOrEqual(Expression.Call(typeof(VBHelpers).GetMethod("Length"), Parameter), ex);
                }
                else
                {
                    return Expression.LessThanOrEqual(ex, Expression.Call(typeof(VBHelpers).GetMethod("Length"), Parameter));
                }
            }
            else if (expression.Contains("len"))
            {
                string[] operands = expression.Trim().Split(new string[] { "len" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid len expression. Too many operands.");
                }

                Expression ex = Parse(operands[0]);
                if (ex.Type != typeof(int))
                {
                    throw new Exception("Invalid len expression. Len can only be compared against an integer.");
                }

                if (Parameter == null)
                    Parameter = Expression.Parameter(typeof(string), "value");

                if (expression.StartsWith("len"))
                {
                    return Expression.Equal(Expression.Call(typeof(VBHelpers).GetMethod("Length"), Parameter), ex);
                }
                else
                {
                    return Expression.Equal(ex, Expression.Call(typeof(VBHelpers).GetMethod("Length"), Parameter));
                }
            }
            else if (expression.Contains("<="))
            {
                string[] operands = expression.Trim().Split(new string[] { "<=" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid less than or equal (<=) expression. Too many operands.");
                }

                Expression ex = Parse(operands[0]);
                if (Parameter == null)
                    Parameter = Expression.Parameter(ex.Type, "value");

                if (expression.StartsWith("<="))
                {
                    return Expression.LessThanOrEqual(Parameter, ex);
                }
                else
                {
                    return Expression.LessThanOrEqual(ex, Parameter);
                }
            }
            else if (expression.Contains(">="))
            {
                string[] operands = expression.Trim().Split(new string[] { ">=" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid greater than or equal (>=) expression. Too many operands.");
                }

                Expression ex = Parse(operands[0]);
                if (Parameter == null)
                    Parameter = Expression.Parameter(ex.Type, "value");

                if (expression.StartsWith(">="))
                {
                    return Expression.GreaterThanOrEqual(Parameter, ex);
                }
                else
                {
                    return Expression.GreaterThanOrEqual(ex, Parameter);
                }
            }
            else if (expression.Contains("!=") || expression.Contains("<>"))
            {
                string[] operands = expression.Trim().Split(new string[] { "<>", "!=" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid not equal (<>, !=) expression. Too many operands.");
                }

                Expression ex = Parse(operands[0]);
                if (Parameter == null)
                    Parameter = Expression.Parameter(ex.Type, "value");

                return Expression.GreaterThanOrEqual(Parameter, ex);
            }
            else if (expression.Contains("="))
            {
                string[] operands = expression.Trim().Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid equal (=) expression. Too many operands.");
                }

                Expression ex = Parse(operands[0]);
                if (Parameter == null)
                    Parameter = Expression.Parameter(ex.Type, "value");

                return Expression.Equal(Parameter, ex);
            }
            else if (expression.Contains("<"))
            {
                string[] operands = expression.Trim().Split(new string[] { "<" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid less than (<) expression. Too many operands.");
                }

                Expression ex = Parse(operands[0]);
                if (Parameter == null)
                    Parameter = Expression.Parameter(ex.Type, "value");

                if (expression.StartsWith("<"))
                {
                    return Expression.LessThan(Parameter, ex);
                }
                else
                {
                    return Expression.LessThan(ex, Parameter);
                }
            }
            else if (expression.Contains(">"))
            {
                string[] operands = expression.Trim().Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries);
                if (operands.Count() != 1)
                {
                    throw new Exception("Invalid greater than (>) expression. Too many operands.");
                }

                Expression ex = Parse(operands[0]);
                if (Parameter == null)
                    Parameter = Expression.Parameter(ex.Type, "value");

                if (expression.StartsWith(">"))
                {
                    return Expression.GreaterThan(Parameter, ex);
                }
                else
                {
                    return Expression.GreaterThan(ex, Parameter);
                }
            }
            else
            {
                return BuildConstant(expression);
            }
        }
    }
}
