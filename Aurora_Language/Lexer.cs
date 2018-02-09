namespace Aurora_Language
{
    public class Lexer
    {
        private readonly string input;
        private char character;
        private int position;
        private int readPosition;

        public Lexer(string input)
        {
            this.input = input;
            ReadCharacter();
        }

        private void ReadCharacter()
        {
            character = readPosition >= input.Length ? '0' : input[readPosition];

            position = readPosition;
            readPosition++;
        }

        public Token NextToken()
        {
            Token token;

            SkipWhitespace();

            switch (character)
            {
                case '=':
                    if (PeekCharacter() == '=')
                    {
                        var character = this.character;

                        ReadCharacter();

                        token = new Token(TokenType.EQ,
                            string.Format("{0}{1}", character.ToString(), this.character.ToString()));
                    }
                    else
                    {
                        token = new Token(TokenType.ASSIGN, character.ToString());
                    }

                    break;
                case '+':
                    token = new Token(TokenType.PLUS, character.ToString());
                    break;
                case '-':
                    token = new Token(TokenType.MINUS, character.ToString());
                    break;
                case '!':
                    if (PeekCharacter() == '=')
                    {
                        var character = this.character;

                        ReadCharacter();

                        token = new Token(TokenType.NOT_EQ,
                            string.Format("{0}{1}", character.ToString(), this.character.ToString()));
                    }
                    else
                    {
                        token = new Token(TokenType.BANG, character.ToString());
                    }

                    break;
                case '/':
                    token = new Token(TokenType.SLASH, character.ToString());
                    break;
                case '*':
                    token = new Token(TokenType.ASTERISK, character.ToString());
                    break;
                case '<':
                    token = new Token(TokenType.LT, character.ToString());
                    break;
                case '>':
                    token = new Token(TokenType.GT, character.ToString());
                    break;
                case '(':
                    token = new Token(TokenType.LPAREN, character.ToString());
                    break;
                case ')':
                    token = new Token(TokenType.RPAREN, character.ToString());
                    break;
                case '{':
                    token = new Token(TokenType.LBRACE, character.ToString());
                    break;
                case '}':
                    token = new Token(TokenType.RBRACE, character.ToString());
                    break;
                case ',':
                    token = new Token(TokenType.COMMA, character.ToString());
                    break;
                case ';':
                    token = new Token(TokenType.SEMMICOLON, character.ToString());
                    break;
                case '0':
                    token = new Token(TokenType.EOF, character.ToString());
                    break;
                case '"':
                    token = new Token(TokenType.STRING, readString());
                    break;
                default:
                    if (IsLetter(character))
                    {
                        var identifier = ReadIdentifier();
                        return new Token(Token.LookupIdentifier(identifier), identifier);
                    }
                    else
                    {
                        return IsNumber(character)
                            ? new Token(TokenType.INT, ReadNumber())
                            : new Token(TokenType.ILLEGAL, character.ToString());
                    }
            }

            ReadCharacter();
            return token;
        }

        private string readString()
        {
            var position = this.position + 1;
            while (true)
            {
                ReadCharacter();
                if (character == '"')
                {
                    break;
                }                
            }

            return GetRange(position, this.position);
        }

        private static bool IsLetter(char character)
        {
            return 'a' <= character && character <= 'z' || 'A' <= character && character <= 'Z' || character == '_';
        }

        private static bool IsNumber(char character)
        {
            return '0' <= character && character <= '9';
        }

        private string ReadIdentifier()
        {
            var position = this.position;

            while (IsLetter(character)) ReadCharacter();

            return GetRange(position, this.position - 1);
        }

        private string ReadNumber()
        {
            var position = this.position;

            while (IsNumber(character)) ReadCharacter();

            return GetRange(position, this.position - 1);
        }

        private void SkipWhitespace()
        {
            while (character == ' ' || character == '\t' || character == '\n' || character == '\r') ReadCharacter();
        }

        private string GetRange(int startSymbol, int endSymbol)
        {
            return input.Substring(startSymbol, endSymbol - startSymbol + 1);
        }

        private char PeekCharacter()
        {
            return position >= input.Length ? '0' : input[readPosition];
        }
    }
}