using Aurora_Language;
using Xunit;

namespace Test_Aurora_Language
{
    public class Test_Lexer
    {
        private struct TokenStruct
        {
            public readonly TokenType Type;
            public readonly string Literal;

            public TokenStruct(TokenType type, string literal)
            {
                Type = type;
                Literal = literal;
            }
        }

        [Fact]
        public void TestNextToken()
        {
            const string input = @"
                            let five = 5;
                            let ten = 10;
                            
                            let add = fn(x, y) {
                                x + y;
                            }

                            let result = add(five, ten);

                            !-/*5;

                            1 < 1 > 1;

                            if (1 < 2) {
                                return true;
                            } else {
                                return false;
                            }

                            7 == 7;
                            5 != 5;
                        ";

            var data = new TokenStruct[73];

            data[0] = new TokenStruct(TokenType.LET, "let");
            data[1] = new TokenStruct(TokenType.IDENT, "five");
            data[2] = new TokenStruct(TokenType.ASSIGN, "=");
            data[3] = new TokenStruct(TokenType.INT, "5");
            data[4] = new TokenStruct(TokenType.SEMMICOLON, ";");
            data[5] = new TokenStruct(TokenType.LET, "let");
            data[6] = new TokenStruct(TokenType.IDENT, "ten");
            data[7] = new TokenStruct(TokenType.ASSIGN, "=");
            data[8] = new TokenStruct(TokenType.INT, "10");
            data[9] = new TokenStruct(TokenType.SEMMICOLON, ";");
            data[10] = new TokenStruct(TokenType.LET, "let");
            data[11] = new TokenStruct(TokenType.IDENT, "add");
            data[12] = new TokenStruct(TokenType.ASSIGN, "=");
            data[13] = new TokenStruct(TokenType.FUNCTION, "fn");
            data[14] = new TokenStruct(TokenType.LPAREN, "(");
            data[15] = new TokenStruct(TokenType.IDENT, "x");
            data[16] = new TokenStruct(TokenType.COMMA, ",");
            data[17] = new TokenStruct(TokenType.IDENT, "y");
            data[18] = new TokenStruct(TokenType.RPAREN, ")");
            data[19] = new TokenStruct(TokenType.LBRACE, "{");
            data[20] = new TokenStruct(TokenType.IDENT, "x");
            data[21] = new TokenStruct(TokenType.PLUS, "+");
            data[22] = new TokenStruct(TokenType.IDENT, "y");
            data[23] = new TokenStruct(TokenType.SEMMICOLON, ";");
            data[24] = new TokenStruct(TokenType.RBRACE, "}");
            data[25] = new TokenStruct(TokenType.LET, "let");
            data[26] = new TokenStruct(TokenType.IDENT, "result");
            data[27] = new TokenStruct(TokenType.ASSIGN, "=");
            data[28] = new TokenStruct(TokenType.IDENT, "add");
            data[29] = new TokenStruct(TokenType.LPAREN, "(");
            data[30] = new TokenStruct(TokenType.IDENT, "five");
            data[31] = new TokenStruct(TokenType.COMMA, ",");
            data[32] = new TokenStruct(TokenType.IDENT, "ten");
            data[33] = new TokenStruct(TokenType.RPAREN, ")");
            data[34] = new TokenStruct(TokenType.SEMMICOLON, ";");
            data[35] = new TokenStruct(TokenType.BANG, "!");
            data[36] = new TokenStruct(TokenType.MINUS, "-");
            data[37] = new TokenStruct(TokenType.SLASH, "/");
            data[38] = new TokenStruct(TokenType.ASTERISK, "*");
            data[39] = new TokenStruct(TokenType.INT, "5");
            data[40] = new TokenStruct(TokenType.SEMMICOLON, ";");
            data[41] = new TokenStruct(TokenType.INT, "1");
            data[42] = new TokenStruct(TokenType.LT, "<");
            data[43] = new TokenStruct(TokenType.INT, "1");
            data[44] = new TokenStruct(TokenType.GT, ">");
            data[45] = new TokenStruct(TokenType.INT, "1");
            data[46] = new TokenStruct(TokenType.SEMMICOLON, ";");
            data[47] = new TokenStruct(TokenType.IF, "if");
            data[48] = new TokenStruct(TokenType.LPAREN, "(");
            data[49] = new TokenStruct(TokenType.INT, "1");
            data[50] = new TokenStruct(TokenType.LT, "<");
            data[51] = new TokenStruct(TokenType.INT, "2");
            data[52] = new TokenStruct(TokenType.RPAREN, ")");
            data[53] = new TokenStruct(TokenType.LBRACE, "{");
            data[54] = new TokenStruct(TokenType.RETURN, "return");
            data[55] = new TokenStruct(TokenType.TRUE, "true");
            data[56] = new TokenStruct(TokenType.SEMMICOLON, ";");
            data[57] = new TokenStruct(TokenType.RBRACE, "}");
            data[58] = new TokenStruct(TokenType.ELSE, "else");
            data[59] = new TokenStruct(TokenType.LBRACE, "{");
            data[60] = new TokenStruct(TokenType.RETURN, "return");
            data[61] = new TokenStruct(TokenType.FALSE, "false");
            data[62] = new TokenStruct(TokenType.SEMMICOLON, ";");
            data[63] = new TokenStruct(TokenType.RBRACE, "}");
            data[64] = new TokenStruct(TokenType.INT, "7");
            data[65] = new TokenStruct(TokenType.EQ, "==");
            data[66] = new TokenStruct(TokenType.INT, "7");
            data[67] = new TokenStruct(TokenType.SEMMICOLON, ";");
            data[68] = new TokenStruct(TokenType.INT, "5");
            data[69] = new TokenStruct(TokenType.NOT_EQ, "!=");
            data[70] = new TokenStruct(TokenType.INT, "5");
            data[71] = new TokenStruct(TokenType.SEMMICOLON, ";");
            data[72] = new TokenStruct(TokenType.EOF, "0");

            var lexer = new Lexer(input);

            foreach (var tokenStruct in data)
            {
                var token = lexer.NextToken();

                Assert.Equal(tokenStruct.Type, token.Type);
                Assert.Equal(tokenStruct.Literal, token.Literal);
            }
        }
    }
}