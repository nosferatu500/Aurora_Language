using System.Collections.Generic;

namespace Aurora_Language
{
    public class Token
    {
        private static readonly IDictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"fn", TokenType.FUNCTION},
            {"let", TokenType.LET},

            {"if", TokenType.IF},
            {"else", TokenType.ELSE},
            {"true", TokenType.TRUE},
            {"false", TokenType.FALSE},
            {"return", TokenType.RETURN}
        };

        public Token(TokenType type, string literal)
        {
            Type = type;
            Literal = literal;
        }

        public TokenType Type { get; }
        public string Literal { get; }

        public static TokenType LookupIdentifier(string identifier)
        {
            return Keywords.ContainsKey(identifier) ? Keywords[identifier] : TokenType.IDENT;
        }
    }
}