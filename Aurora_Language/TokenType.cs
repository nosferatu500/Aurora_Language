namespace Aurora_Language
{
    public sealed class TokenType
    {
        public static readonly TokenType STRING = new TokenType("STRING");
        
        public static readonly TokenType ILLEGAL = new TokenType("ILLEGAL");
        public static readonly TokenType EOF = new TokenType("EOF");

        public static readonly TokenType IDENT = new TokenType("IDENT");
        public static readonly TokenType INT = new TokenType("INT");

        public static readonly TokenType ASSIGN = new TokenType("=");
        public static readonly TokenType PLUS = new TokenType("+");
        public static readonly TokenType MINUS = new TokenType("-");

        public static readonly TokenType BANG = new TokenType("!");
        public static readonly TokenType ASTERISK = new TokenType("*");
        public static readonly TokenType SLASH = new TokenType("/");

        public static readonly TokenType LT = new TokenType("<");
        public static readonly TokenType GT = new TokenType(">");

        public static readonly TokenType EQ = new TokenType("==");
        public static readonly TokenType NOT_EQ = new TokenType("!=");

        public static readonly TokenType COMMA = new TokenType(",");
        public static readonly TokenType SEMMICOLON = new TokenType(";");

        public static readonly TokenType LPAREN = new TokenType("(");
        public static readonly TokenType RPAREN = new TokenType(")");
        public static readonly TokenType LBRACE = new TokenType("{");
        public static readonly TokenType RBRACE = new TokenType("}");

        public static readonly TokenType FUNCTION = new TokenType("FUNCTION");
        public static readonly TokenType LET = new TokenType("LET");

        public static readonly TokenType IF = new TokenType("IF");
        public static readonly TokenType ELSE = new TokenType("ELSE");
        public static readonly TokenType TRUE = new TokenType("TRUE");
        public static readonly TokenType FALSE = new TokenType("FALSE");
        public static readonly TokenType RETURN = new TokenType("RETURN");
        private readonly string value;

        private TokenType(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }
    }
}