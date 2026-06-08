using System;

class LexicalAnalyzer
{
    private static Sym _symbol;
    private static TextPosition _token;
    private static string _addrName;
    private static string _addrString;
    private static int _nmbInt;
    private static float _nmbFloat;
    private static char _oneSymbol;

    private static Keywords _keywords = new Keywords();

    public static Sym Symbol
    {
        get
        {
            return _symbol;
        }
    }
    public static TextPosition Token
    {
        get
        {
            return _token;
        }
    }
    public static string AddrName
    {
        get
        {
            return _addrName;
        }
    }
    public static string AddrString
    {
        get
        {
            return _addrString;
        }
    }

    public static int NmbInt
    {
        get
        {
            return _nmbInt;
        }
    }
    public static float NmbFloat
    {
        get
        {
            return _nmbFloat;
        }
    }
    public static float OneSymbol
    {
        get
        {
            return _oneSymbol;
        }
    }

    public static Sym NextSym()
    {
        bool skipping = false;
        while (!skipping && !InputOutput.IsEof)
        {
            while (!InputOutput.IsEof &&
                   (InputOutput.Ch == ' ' || InputOutput.Ch == '\t'))
            {
                InputOutput.NextCh();
            }

            if (InputOutput.IsEof)
            {
                break;
            }

            if (InputOutput.Ch == '{')
            {
                SkipBraceComment();
            }
            else if (InputOutput.Ch == '(')
            {
                TextPosition startPos = InputOutput.PositionNow;
                InputOutput.NextCh();
                if (InputOutput.Ch == '*')
                {
                    InputOutput.NextCh(); 
                    SkipParenStarComment();
                }
                else
                {
                    _token = startPos;
                    _symbol = Sym.Leftpar;
                    return _symbol;
                }
            }
            else if (InputOutput.Ch == '/')
            {
                TextPosition startPos = InputOutput.PositionNow;
                InputOutput.NextCh();
                if (InputOutput.Ch == '/')
                {
                    SkipLineComment();
                }
                else
                {
                    _token = startPos;
                    _symbol = Sym.Slash;
                    return _symbol;
                }
            }
            else
            {
                skipping = true;
            }
        }

        if (InputOutput.IsEof)
        {
            _symbol = Sym.Unknown;
            return _symbol;
        }

        _token = InputOutput.PositionNow;
        char ch = InputOutput.Ch;

        if (IsLetter(ch))
        {
            _addrName = "";
            while (IsLetter(InputOutput.Ch) || IsDigit(InputOutput.Ch))
            {
                _addrName += InputOutput.Ch;
                InputOutput.NextCh();
            }
            _symbol = _keywords.Lookup(_addrName);
            return _symbol;
        }

        if (IsDigit(ch))
        {
            int maxInt = short.MaxValue;
            _nmbInt = 0;
            bool overflow = false;
            while (IsDigit(InputOutput.Ch))
            {
                byte digit = (byte)(InputOutput.Ch - '0');
                if (!overflow)
                {
                    if (_nmbInt < maxInt / 10 ||
                       (_nmbInt == maxInt / 10 && digit <= maxInt % 10))
                    {
                        _nmbInt = 10 * _nmbInt + digit;
                    }
                    else
                    {
                        InputOutput.Error(InputOutput.PositionNow, 11);
                        _nmbInt = 0;
                        overflow = true;
                    }
                }
                InputOutput.NextCh();
            }
            _symbol = Sym.Intc;
            return _symbol;
        }
        if (ch == '\'')
        {
            ReadStringConstant();
            return _symbol;
        }

        switch (ch)
        {
            case '<':
                InputOutput.NextCh();
                if (InputOutput.Ch == '=')
                {
                    _symbol = Sym.Laterequal;
                    InputOutput.NextCh();
                }
                else if (InputOutput.Ch == '>')
                {
                    _symbol = Sym.Latergreater;
                    InputOutput.NextCh();
                }
                else
                {
                    _symbol = Sym.Later;
                }
                break;

            case '>':
                InputOutput.NextCh();
                if (InputOutput.Ch == '=')
                {
                    _symbol = Sym.Greaterequal;
                    InputOutput.NextCh();
                }
                else
                {
                    _symbol = Sym.Greater;
                }
                break;

            case ':':
                InputOutput.NextCh();
                if (InputOutput.Ch == '=')
                {
                    _symbol = Sym.Assign;
                    InputOutput.NextCh();
                }
                else
                {
                    _symbol = Sym.Colon;
                }
                break;

            case '.':
                InputOutput.NextCh();
                if (InputOutput.Ch == '.')
                {
                    _symbol = Sym.Twopoints;
                    InputOutput.NextCh();
                }
                else
                {
                    _symbol = Sym.Point;
                }
                break;

            case ';': _symbol = Sym.Semicolon; InputOutput.NextCh(); break;
            case ',': _symbol = Sym.Comma; InputOutput.NextCh(); break;
            case '=': _symbol = Sym.Equal; InputOutput.NextCh(); break;
            case '+': _symbol = Sym.Plus; InputOutput.NextCh(); break;
            case '-': _symbol = Sym.Minus; InputOutput.NextCh(); break;
            case '*': _symbol = Sym.Star; InputOutput.NextCh(); break;
            case ')': _symbol = Sym.Rightpar; InputOutput.NextCh(); break;
            case '[': _symbol = Sym.Lbracket; InputOutput.NextCh(); break;
            case ']': _symbol = Sym.Rbracket; InputOutput.NextCh(); break;
            case '^': _symbol = Sym.Arrow; InputOutput.NextCh(); break;

            default:
                InputOutput.Error(InputOutput.PositionNow, 5);
                InputOutput.NextCh();
                return NextSym();
        }

        return _symbol;
    }

    private static void SkipBraceComment()
    {
        InputOutput.NextCh();
        while (!InputOutput.IsEof && InputOutput.Ch != '}')
        {
            InputOutput.NextCh();
        }
        if (!InputOutput.IsEof)
        {
            InputOutput.NextCh();
        }
        else
        {
            InputOutput.Error(InputOutput.PositionNow, 20);
        }
    }

    private static void SkipParenStarComment()
    {
        bool finished = false;
        while (!finished && !InputOutput.IsEof)
        {
            if (InputOutput.Ch == '*')
            {
                InputOutput.NextCh();
                if (!InputOutput.IsEof && InputOutput.Ch == ')')
                {
                    InputOutput.NextCh();
                    finished = true;
                }
            }
            else
            {
                InputOutput.NextCh();
            }
        }
    }

    private static void SkipLineComment()
    {
        uint startLine = InputOutput.PositionNow.LineNumber;
        while (!InputOutput.IsEof &&
               InputOutput.PositionNow.LineNumber == startLine)
        {
            InputOutput.NextCh();
        }
    }
    private static void ReadStringConstant()
    {
        uint startLine = _token.LineNumber;
        InputOutput.NextCh();
        _addrString = "";

        bool finished = false;
        bool broken = false;

        while (!finished && !broken && !InputOutput.IsEof)
        {
            if (InputOutput.IsEol)
            {
                broken = true;
            }
            else if (InputOutput.Ch == '\'')
            {
                InputOutput.NextCh();
                if (!InputOutput.IsEof &&
                    !InputOutput.IsEol &&
                    InputOutput.Ch == '\'')
                {
                    _addrString += '\'';
                    InputOutput.NextCh();
                }
                else
                {
                    finished = true;
                }
            }
            else
            {
                _addrString += InputOutput.Ch;
                InputOutput.NextCh();
            }
        }

        if (!finished)
        {
            InputOutput.Error(_token, 12);
        }

        if (_addrString.Length == 1)
        {
            _oneSymbol = _addrString[0];
        }

        _symbol = Sym.Stringc;
    }
    private static bool IsLetter(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z');
    }

    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }
}