using System;
using System.Collections.Generic;
using Aurora_Language.Data;

namespace Aurora_Language
{
    public class Parser
    {
        private static readonly IDictionary<TokenType, ExpressionType> precedences =
            new Dictionary<TokenType, ExpressionType>
            {
                {TokenType.EQ, ExpressionType.Equals},
                {TokenType.NOT_EQ, ExpressionType.Equals},

                {TokenType.LT, ExpressionType.LessGreater},
                {TokenType.GT, ExpressionType.LessGreater},

                {TokenType.PLUS, ExpressionType.Sum},
                {TokenType.MINUS, ExpressionType.Sum},

                {TokenType.SLASH, ExpressionType.Product},
                {TokenType.ASTERISK, ExpressionType.Product},

                {TokenType.LPAREN, ExpressionType.Call}
            };

        private readonly List<string> errors;
        private readonly Dictionary<TokenType, FunctionType> infixFunctions = new Dictionary<TokenType, FunctionType>();
        private readonly Lexer lexer;

        private readonly Dictionary<TokenType, FunctionType>
            prefixFunctions = new Dictionary<TokenType, FunctionType>();

        private Token currentToken;
        private Token peekToken;

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;

            errors = new List<string>();

            registerPrefixFunction(TokenType.IDENT, FunctionType.Prefix);
            registerPrefixFunction(TokenType.INT, FunctionType.Prefix);

            registerPrefixFunction(TokenType.BANG, FunctionType.Prefix);
            registerPrefixFunction(TokenType.MINUS, FunctionType.Prefix);

            registerPrefixFunction(TokenType.IF, FunctionType.Prefix);

            registerPrefixFunction(TokenType.TRUE, FunctionType.Prefix);
            registerPrefixFunction(TokenType.FALSE, FunctionType.Prefix);

            registerPrefixFunction(TokenType.LPAREN, FunctionType.Prefix);
            registerPrefixFunction(TokenType.FUNCTION, FunctionType.Prefix);


            registerInfixFunction(TokenType.PLUS, FunctionType.Infix);
            registerInfixFunction(TokenType.MINUS, FunctionType.Infix);
            registerInfixFunction(TokenType.SLASH, FunctionType.Infix);
            registerInfixFunction(TokenType.ASTERISK, FunctionType.Infix);
            registerInfixFunction(TokenType.EQ, FunctionType.Infix);
            registerInfixFunction(TokenType.NOT_EQ, FunctionType.Infix);
            registerInfixFunction(TokenType.LT, FunctionType.Infix);
            registerInfixFunction(TokenType.GT, FunctionType.Infix);

            registerInfixFunction(TokenType.LPAREN, FunctionType.Infix);

            NextToken();
            NextToken();
        }

        private void NextToken()
        {
            currentToken = peekToken;
            peekToken = lexer.NextToken();
        }

        public Program ParseProgram()
        {
            var program = new Program();

            while (currentToken.Type != TokenType.EOF)
            {
                var statement = ParseStatement();

                if (statement != null)
                    if (statement.TokenLiteral() != TokenType.RBRACE.ToString())
                        program.Statements.Add(statement);

                NextToken();
            }

            return program;
        }

        private IStatement ParseStatement()
        {
            switch (currentToken.Type.ToString())
            {
                case "LET":
                    return ParseLetStatement();
                case "RETURN":
                    return ParseReturnStatement();

                default: return ParseExpressionStatement();
            }
        }

        private LetStatement ParseLetStatement()
        {
            var statement = new LetStatement(currentToken);

            if (!ExpectPeek(TokenType.IDENT)) return null;

            statement.Name = new Identifier(currentToken, currentToken.Literal);

            if (!ExpectPeek(TokenType.ASSIGN)) return null;

            statement.Value = ParseExpression(ExpressionType.Lowest);

            while (!IsCurrentToken(TokenType.SEMMICOLON)) NextToken();

            return statement;
        }

        private ReturnStatement ParseReturnStatement()
        {
            var statement = new ReturnStatement(currentToken);

            NextToken();

            statement.ReturnValue = ParseExpression(ExpressionType.Lowest);

            if (!IsCurrentToken(TokenType.SEMMICOLON)) NextToken();

            return statement;
        }

        private IntegerLiteral ParseIntegerLiteral()
        {
            var literal = new IntegerLiteral(currentToken);

            var value = long.Parse(currentToken.Literal);

            literal.LongValue = value;

            return literal;
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            var statement = new ExpressionStatement(currentToken) {Expression = ParseExpression(ExpressionType.Lowest)};

            if (IsPeeKToken(TokenType.SEMMICOLON)) NextToken();

            return statement;
        }

        private IExpression ParseExpression(ExpressionType precedence)
        {
            var result = prefixFunctions.TryGetValue(currentToken.Type, out var prefix);

            if (!result) return null;

            var leftExpression = prefixExpression(currentToken.Type);

            while (!IsPeeKToken(TokenType.SEMMICOLON) && precedence < PeekPrecedence())
            {
                var infix = infixFunctions.TryGetValue(peekToken.Type, out prefix);

                if (!infix) return leftExpression;

                NextToken();

                leftExpression = infixExpression(leftExpression, currentToken.Type);
            }


            return leftExpression;
        }

        private bool ExpectPeek(TokenType type)
        {
            if (IsPeeKToken(type))
            {
                NextToken();
                return true;
            }

            PeekError(type);
            return false;
        }

        private bool IsPeeKToken(TokenType type)
        {
            return peekToken.Type == type;
        }

        private bool IsCurrentToken(TokenType type)
        {
            return currentToken.Type == type;
        }

        public List<string> Errors()
        {
            return errors;
        }

        private void PeekError(TokenType type)
        {
            var message = $"Expected token: {type}, acctualy: {peekToken.Type}";
            errors.Add(message);
        }

        private void registerPrefixFunction(TokenType type, FunctionType functionType)
        {
            prefixFunctions.Add(type, functionType);
        }

        private void registerInfixFunction(TokenType type, FunctionType functionType)
        {
            infixFunctions.Add(type, functionType);
        }

        private IExpression prefixExpression(TokenType type)
        {
            switch (type.ToString())
            {
                case "IDENT":
                    return parseIdentifier();
                case "INT":
                    return ParseIntegerLiteral();
                case "!":
                    return ParsePrefixExpression();
                case "-":
                    return ParsePrefixExpression();
                case "TRUE":
                    return ParseBooleanExpression();
                case "IF":
                    return parseIfExpression();
                case "FALSE":
                    return ParseBooleanExpression();
                case "(":
                    return parseGroupedExpression();
                case "FUNCTION":
                    return parseFunctionLiteral();
                default: return null;
            }
        }

        private IExpression parseFunctionLiteral()
        {
            var literal = new FunctionLiteral(currentToken);

            if (!ExpectPeek(TokenType.LPAREN)) return null;

            literal.Parameters = parseFunctionParameters();

            if (!ExpectPeek(TokenType.LBRACE)) return null;

            literal.Body = parseBlockStatement();

            return literal;
        }

        private List<Identifier> parseFunctionParameters()
        {
            var identifiers = new List<Identifier>();

            if (IsPeeKToken(TokenType.RPAREN))
            {
                NextToken();
                return identifiers;
            }

            NextToken();

            var identifier = new Identifier(currentToken, currentToken.Literal);

            identifiers.Add(identifier);

            while (IsPeeKToken(TokenType.COMMA))
            {
                NextToken();
                NextToken();

                identifier = new Identifier(currentToken, currentToken.Literal);
                identifiers.Add(identifier);
            }

            while (!ExpectPeek(TokenType.RPAREN)) return null;

            return identifiers;
        }

        private IExpression parseIfExpression()
        {
            var expression = new IfExpression(currentToken);

            if (!ExpectPeek(TokenType.LPAREN)) throw new Exception("Error with parse if statement");

            NextToken();

            expression.Condition = ParseExpression(ExpressionType.Lowest);

            if (!ExpectPeek(TokenType.RPAREN)) throw new Exception("Error with parse if statement");

            if (!ExpectPeek(TokenType.LBRACE)) throw new Exception("Error with parse if statement");

            expression.Consequence = parseBlockStatement();

            if (!IsPeeKToken(TokenType.ELSE)) return expression;
            NextToken();

            if (!ExpectPeek(TokenType.LBRACE)) throw new Exception("Error with parse else statement");

            expression.Alternative = parseBlockStatement();

            return expression;
        }

        private BlockStatement parseBlockStatement()
        {
            var block = new BlockStatement(currentToken) {Statements = new List<IStatement>()};

            NextToken();

            while (!IsCurrentToken(TokenType.RBRACE))
            {
                var statement = ParseStatement();
                if (statement != null) block.Statements.Add(statement);
                NextToken();
            }

            return block;
        }

        private IExpression infixExpression(IExpression expression, TokenType type)
        {
            switch (type.ToString())
            {
                case "(":
                    return parseCallExpression(expression);
                default:
                    return parseInfixExpression(expression);
            }
        }

        private IExpression parseCallExpression(IExpression function)
        {
            var expression = new CallExpression(currentToken, function) {Arguments = parseCallArguments()};

            return expression;
        }

        private List<IExpression> parseCallArguments()
        {
            var arguments = new List<IExpression>();

            if (IsPeeKToken(TokenType.RPAREN))
            {
                NextToken();
                return arguments;
            }

            NextToken();

            arguments.Add(ParseExpression(ExpressionType.Lowest));

            while (IsPeeKToken(TokenType.COMMA))
            {
                NextToken();
                NextToken();

                arguments.Add(ParseExpression(ExpressionType.Lowest));
            }

            return !ExpectPeek(TokenType.RPAREN) ? null : arguments;
        }

        private IExpression parseGroupedExpression()
        {
            NextToken();

            var expression = ParseExpression(ExpressionType.Lowest);

            while (!ExpectPeek(TokenType.RPAREN)) return null;

            return expression;
        }

        private IExpression ParseBooleanExpression()
        {
            return new BooleanExpression(currentToken, IsCurrentToken(TokenType.TRUE));
        }

        private IExpression parseInfixExpression(IExpression left)
        {
            var expression = new InfixExpression(currentToken, currentToken.Literal, left);

            var precedence = CurrentPrecedence();

            NextToken();

            expression.Right = ParseExpression(precedence);
            return expression;
        }

        private IExpression parseIdentifier()
        {
            return new Identifier(currentToken, currentToken.Literal);
        }

        private IExpression ParsePrefixExpression()
        {
            var expression = new PrefixExpression(currentToken, currentToken.Literal);

            NextToken();

            expression.Right = ParseExpression(ExpressionType.Prefix);

            return expression;
        }

        private ExpressionType PeekPrecedence()
        {
            return precedences.TryGetValue(peekToken.Type, out var type) ? type : 0;
        }

        private ExpressionType CurrentPrecedence()
        {
            return precedences.TryGetValue(currentToken.Type, out var type) ? type : 0;
        }
    }
}