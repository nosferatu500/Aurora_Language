using System;
using System.Collections.Generic;
using System.Linq;
using Aurora_Language.Data;
using Aurora_Language.Obj;
using Boolean = Aurora_Language.Obj.Boolean;
using Environment = Aurora_Language.Obj.Environment;

namespace Aurora_Language
{
    public class Evaluator
    {
        public static IObject Evaluate(INode node, Environment environment)
        {
            while (true)
            {
                switch (node.GetType().ToString())
                {
                    case "Aurora_Language.Program":
                        return EvalProgram(node as Program, environment);
                    case "Aurora_Language.Data.ExpressionStatement":
                        node = node.Expression;
                        continue;
                    case "Aurora_Language.Data.IntegerLiteral":
                        return new Integer(Convert.ToInt64(node.TokenLiteral()));
                    case "Aurora_Language.Data.BooleanExpression":
                        return new Boolean(Convert.ToBoolean(node.TokenLiteral()));
                    case "Aurora_Language.Data.PrefixExpression":
                    {
                        var right = Evaluate(node.Right, environment);
                        return IsError(right) ? right : EvalPrefixExpression(node.Operator, right);
                    }

                    case "Aurora_Language.Data.InfixExpression":
                    {
                        var left = Evaluate(node.Left, environment);
                        var right = Evaluate(node.Right, environment);

                        if (IsError(left)) return left;

                        return IsError(right) ? right : EvalInfixExpression(node.Operator, left, right);
                    }
                    case "Aurora_Language.Data.BlockStatement":
                        return EvalBlockStatement(node as BlockStatement, environment);
                    case "Aurora_Language.Data.IfExpression":
                        return EvalIfExpression(node as IfExpression, environment);
                    case "Aurora_Language.Data.ReturnStatement":
                    {
                        var value = Evaluate(node.ReturnValue, environment);
                        return IsError(value) ? value : new ReturnValue(value);
                    }
                    case "Aurora_Language.Data.LetStatement":
                    {
                        var value = Evaluate(node.Name, environment);
                        if (IsError(value)) return value;

                        environment.Set(node.Name.StringValue, value);


                        return new ReturnValue(value);
                    }

                    case "Aurora_Language.Data.Identifier":
                    {
                        return EvalIdentifier(node as Identifier, environment);
                    }

                    case "Aurora_Language.Data.FunctionLiteral":
                    {
                        var @params = node.Parameters;
                        var body = node.Body;
                        return new Function(@params, body, environment);
                    }

                    case "Aurora_Language.Data.CallExpression":
                    {
                        var function = Evaluate(node.Function, environment);
                        if (IsError(function)) return function;

                        var args = evalExpressions(node.Arguments, environment);
                        if (args.Count == 1 && IsError(args.First())) return args.First();

                        return applyFunction(function, args);
                    }

                    default:
                        return null;
                }
            }
        }

        private static IObject applyFunction(IObject funciton, List<IObject> args)
        {
            if (funciton.GetType() != typeof(Function))
            {
                return NewError(string.Format("not a function: {0}", funciton.Type()));
            }

            var extendedEnviroment = extendFunctionEnv(funciton as Function, args);
            var evaluated = Evaluate(funciton.Body, extendedEnviroment);
            return unwrapReturnValue(evaluated);
        }

        private static IObject unwrapReturnValue(IObject obj)
        {
            return obj.GetType() == typeof(ReturnValue) ? obj.ObjectValue : obj;
        }

        private static Environment extendFunctionEnv(Function function, List<IObject> args)
        {
            var enviroment = Environment.EnclosedEnvironment(function.Enviroment);

            for (int i = 0; i < function.Parameters.Count; i++)
            {
                enviroment.Set(function.Parameters[i].StringValue, args[i]);
            }

            return enviroment;
        }

        private static List<IObject> evalExpressions(IEnumerable<IExpression> expressions, Environment environment)
        {
            var result = new List<IObject>();

            foreach (var expression in expressions)
            {
                var evaluated = Evaluate(expression, environment);
                if (IsError(evaluated))
                {
                    result.Add(evaluated);
                    return result;
                }

                result.Add(evaluated);
            }

            return result;
        }

        private static IObject EvalIdentifier(INode node, Environment enviroment)
        {
            (var value, var result) = enviroment.Get(node.StringValue);
            return !result ? NewError(string.Format("Identifier not found: {0}", node.StringValue)) : value;
        }

        private static IObject EvalIfExpression(IfExpression expression, Environment environment)
        {
            var condition = Evaluate(expression.Condition, environment);

            if (IsError(condition)) return condition;

            if (IsTruthy(condition)) return Evaluate(expression.Consequence, environment);

            return expression.Alternative != null ? Evaluate(expression.Alternative, environment) : null;
        }

        private static IObject EvalBlockStatement(BlockStatement block, Environment environment)
        {
            foreach (var statement in block.Statements)
            {
                var result = Evaluate(statement, environment);

                return result != null && result.Type() == ObjectType.RETURN_VALUE_OBJ.ToString() ||
                       result?.Type() == ObjectType.ERROR_OBJ.ToString()
                    ? result
                    : result;
            }

            return null;
        }

        private static bool IsError(IObject obj)
        {
            if (obj != null) return obj.Type() == ObjectType.ERROR_OBJ.ToString();

            return false;
        }

        private static Error NewError(string message)
        {
            return new Error(message);
        }

        private static IObject EvalProgram(Program program, Environment environment)
        {
            foreach (var statement in program.Statements)
            {
                var result = Evaluate(statement, environment);

                if (result != null && result.GetType() == typeof(ReturnValue)) return result.ObjectValue;

                if (result != null && result.GetType() == typeof(Error)) return result;

                return result;
            }

            return null;
        }

        private static bool IsTruthy(IObject obj)
        {
            if (obj.GetType() == new Boolean(true).GetType() && obj.BoolValue) return true;

            if (obj.GetType() == new Boolean(false).GetType() && !obj.BoolValue) return false;

            return obj.GetType() != new Null().GetType();
        }

        private static IObject EvalPrefixExpression(string @operator, IObject right)
        {
            switch (@operator)
            {
                case "!":
                    return EvalBangOperatorExpression(right);
                case "-":
                    return EvalMinusPrefixOperatorExpression(right);
                default:
                    return NewError(string.Format("Unknown operator: {0}{1}", @operator, right.Type()));
            }
        }

        private static IObject EvalInfixExpression(string @operator, IObject left, IObject right)
        {
            if (left.Type() != right.Type())
                return NewError(string.Format("type mismatch: {0}{1}{2}", left.Type(), @operator, right.Type()));

            if (left.Type() == ObjectType.INTEGER_OBJ.ToString() && right.Type() == ObjectType.INTEGER_OBJ.ToString())
                return EvalIntegerInfixExpression(@operator, left, right);

            switch (@operator)
            {
                case "==":
                    return new Boolean(left.BoolValue == right.BoolValue);
                case "!=":
                    return new Boolean(left.BoolValue != right.BoolValue);
            }

            return NewError(string.Format("Unknown operator: {0}{1}{2}", left.Type(), @operator, right.Type()));
        }

        private static IObject EvalIntegerInfixExpression(string @operator, IObject left, IObject right)
        {
            switch (@operator)
            {
                case "+":
                    return new Integer(left.LongValue + right.LongValue);
                case "-":
                    return new Integer(left.LongValue - right.LongValue);
                case "*":
                    return new Integer(left.LongValue * right.LongValue);
                case "/":
                    return new Integer(left.LongValue / right.LongValue);
                case "<":
                    return new Boolean(left.LongValue < right.LongValue);
                case ">":
                    return new Boolean(left.LongValue > right.LongValue);
                case "==":
                    return new Boolean(left.LongValue == right.LongValue);
                case "!=":
                    return new Boolean(left.LongValue != right.LongValue);
                default:
                    return NewError(string.Format("Unknown operator: {0}{1}{2}", left.Type(), @operator, right.Type()));
            }
        }

        private static IObject EvalBangOperatorExpression(IObject right)
        {
            if (right.GetType() == new Boolean(true).GetType() && right.BoolValue) return new Boolean(false);

            if (right.GetType() == new Boolean(false).GetType() && !right.BoolValue) return new Boolean(true);

            return right.GetType() == new Null().GetType() ? new Boolean(true) : new Boolean(false);
        }

        private static IObject EvalMinusPrefixOperatorExpression(IObject right)
        {
            if (right.Type() != ObjectType.INTEGER_OBJ.ToString())
                return NewError(string.Format("Unknown operator: -{0}", right.Type()));

            return new Integer(-right.LongValue);
        }
    }
}