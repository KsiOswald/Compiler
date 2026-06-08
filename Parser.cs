using System.Collections.Generic;
using System.IO;

class Parser
{
    private StreamWriter _tokensWriter;
    private SymbolTable _scope;

    public Parser(StreamWriter tokensWriter)
    {
        _tokensWriter = tokensWriter;
        _scope = new SymbolTable();
    }

    public void Parse()
    {
        Advance();
        ParseProgram();

        while (LexicalAnalyzer.Symbol != Sym.Unknown)
        {
            Advance();
        }
    }

    private void Advance()
    {
        LexicalAnalyzer.NextSym();
        Sym sym = LexicalAnalyzer.Symbol;
        if (sym != Sym.Unknown && _tokensWriter != null)
        {
            _tokensWriter.Write((byte)sym);
            _tokensWriter.Write(' ');
        }
    }

    private void ReportError(byte code)
    {
        InputOutput.Error(LexicalAnalyzer.Token, code);
    }

    private void ReportError(byte code, TextPosition pos)
    {
        InputOutput.Error(pos, code);
    }

    private void SkipTo(params Sym[] stopSet)
    {
        while (LexicalAnalyzer.Symbol != Sym.Unknown)
        {
            Sym cur = LexicalAnalyzer.Symbol;
            for (int i = 0; i < stopSet.Length; i++)
            {
                if (cur == stopSet[i])
                {
                    return;
                }
            }
            Advance();
        }
    }

    private bool IsUnsupportedDeclaration(Sym sym)
    {
        return sym == Sym.Constsy
            || sym == Sym.Typesy
            || sym == Sym.Procedurensy
            || sym == Sym.Labelsy;
    }

    private bool IsUnsupportedStatement(Sym sym)
    {
        return sym == Sym.Ifsy
            || sym == Sym.Whilesy
            || sym == Sym.Forsy
            || sym == Sym.Repeatsy
            || sym == Sym.Casesy
            || sym == Sym.Withsy
            || sym == Sym.Gotosy;
    }

    private bool IsRelation(Sym sym)
    {
        return sym == Sym.Equal
            || sym == Sym.Latergreater
            || sym == Sym.Later
            || sym == Sym.Greater
            || sym == Sym.Laterequal
            || sym == Sym.Greaterequal;
    }

    private bool IsStandardType(string name)
    {
        string n = name.ToLower();
        return n == "integer" || n == "real" || n == "char"
            || n == "boolean" || n == "string";
    }

    private bool IsIntegerLike(string type)
    {
        return type == "integer"|| type == "subrange";
    }

    private bool IsNumeric(string type)
    {
        return IsIntegerLike(type) || type == "real";
    }

    private bool IsAssignable(string lhs, string rhs)
    {
        if (lhs == "" || rhs == "")
        {
            return true;
        }
        if (lhs == rhs)
        {
            return true;
        }
        if (lhs == "real" && rhs == "integer")
        {
            return true;
        }
        if (lhs == "string" && rhs == "char")
        {
            return true;
        }
        if (lhs == "subrange" && IsNumeric(rhs))
        {
            return true;
        }
        if (IsNumeric(lhs) && rhs == "subrange")
        {
            return true;
        }
        if (lhs == "enum" && rhs == "enum_value")
        {
            return true;
        }
        if (lhs == "enum_value" && rhs == "enum")
        {
            return true;
        }
        return false;
    }

    private bool IsComparable(string a, string b)
    {
        if (a == "" || b == "")
        {
            return true;
        }
        if (a == b)
        {
            return true;
        }
        if (IsNumeric(a) && IsNumeric(b))
        {
            return true;
        }
        if ((a == "char" && b == "string") || (a == "string" && b == "char"))
        {
            return true;
        }
        return false;
    }

    private string CheckBinaryOperator(Sym op, string left, string right,
        TextPosition pos)
    {
        if (left == "" || right == "")
        {
            return "";
        }

        if (op == Sym.Divsy || op == Sym.Modsy)
        {
            if (!IsIntegerLike(left) || !IsIntegerLike(right))
            {
                ReportError(31, pos);
            }
            return "integer";
        }

        if (op == Sym.Andsy || op == Sym.Orsy)
        {
            if (left != "boolean" || right != "boolean")
            {
                ReportError(30, pos);
            }
            return "boolean";
        }

        if (IsRelation(op))
        {
            if (!IsComparable(left, right))
            {
                ReportError(29, pos);
            }
            return "boolean";
        }

        if (!IsNumeric(left) || !IsNumeric(right))
        {
            ReportError(31, pos);
            return "";
        }
        if (op == Sym.Slash)
        {
            return "real";
        }
        if (left == "real" || right == "real")
        {
            return "real";
        }
        return "integer";
    }

    private void ParseProgram()
    {
        if (LexicalAnalyzer.Symbol == Sym.Programsy)
        {
            Advance();
        }
        else
        {
            ReportError(1);
            SkipTo(Sym.Programsy, Sym.Varsy, Sym.Beginsy, Sym.Functionsy);
            if (LexicalAnalyzer.Symbol == Sym.Programsy)
            {
                Advance();
            }
        }

        if (LexicalAnalyzer.Symbol == Sym.Ident)
        {
            Advance();
        }
        else
        {
            ReportError(13);
        }

        if (LexicalAnalyzer.Symbol == Sym.Semicolon)
        {
            Advance();
        }
        else
        {
            ReportError(4);
        }

        Block();

        if (LexicalAnalyzer.Symbol != Sym.Point)
        {
            ReportError(14);
        }
    }

    private void Block()
    {
        bool inDeclarations = true;
        while (inDeclarations &&
               LexicalAnalyzer.Symbol != Sym.Unknown &&
               LexicalAnalyzer.Symbol != Sym.Beginsy)
        {
            Sym cur = LexicalAnalyzer.Symbol;
            if (cur == Sym.Varsy)
            {
                VarSection();
            }
            else if (cur == Sym.Functionsy)
            {
                FunctionDeclaration();
            }
            else if (IsUnsupportedDeclaration(cur))
            {
                Advance();
                SkipTo(Sym.Varsy, Sym.Functionsy, Sym.Beginsy);
            }
            else
            {
                ReportError(21);
                SkipTo(Sym.Varsy, Sym.Functionsy, Sym.Beginsy);
                Sym after = LexicalAnalyzer.Symbol;
                if (after != Sym.Varsy &&
                    after != Sym.Functionsy &&
                    after != Sym.Beginsy)
                {
                    inDeclarations = false;
                }
            }
        }

        CompoundStatement();
    }

    private void VarSection()
    {
        Advance();

        while (LexicalAnalyzer.Symbol == Sym.Ident)
        {
            VarDeclaration();
        }
    }

    private void VarDeclaration()
    {
        List<IdentRef> names = IdentifierList();

        if (LexicalAnalyzer.Symbol == Sym.Colon)
        {
            Advance();
        }
        else
        {
            ReportError(15);
            SkipTo(Sym.Semicolon, Sym.Ident, Sym.Beginsy, Sym.Functionsy);
        }

        string type = TypeDescription();

        foreach (IdentRef ir in names)
        {
            if (!_scope.AddVariable(ir.Name, type))
            {
                ReportError(24, ir.Position);
            }
        }

        if (LexicalAnalyzer.Symbol == Sym.Semicolon)
        {
            Advance();
        }
        else
        {
            ReportError(4);
            SkipTo(Sym.Semicolon, Sym.Ident, Sym.Beginsy, Sym.Functionsy);
            if (LexicalAnalyzer.Symbol == Sym.Semicolon)
            {
                Advance();
            }
        }
    }

    private List<IdentRef> IdentifierList()
    {
        List<IdentRef> names = new List<IdentRef>();

        if (LexicalAnalyzer.Symbol == Sym.Ident)
        {
            names.Add(new IdentRef(LexicalAnalyzer.AddrName,
                                   LexicalAnalyzer.Token));
            Advance();
        }
        else
        {
            ReportError(13);
        }

        while (LexicalAnalyzer.Symbol == Sym.Comma)
        {
            Advance();
            if (LexicalAnalyzer.Symbol == Sym.Ident)
            {
                names.Add(new IdentRef(LexicalAnalyzer.AddrName,
                                       LexicalAnalyzer.Token));
                Advance();
            }
            else
            {
                ReportError(13);
            }
        }

        return names;
    }

    private string TypeDescription()
    {
        Sym cur = LexicalAnalyzer.Symbol;

        if (cur == Sym.Leftpar)
        {
            Advance();
            List<IdentRef> values = IdentifierList();

            foreach (IdentRef val in values)
            {
                if (!_scope.AddVariable(val.Name, "enum_value"))
                {
                    ReportError(24, val.Position);
                }
            }

            if (LexicalAnalyzer.Symbol == Sym.Rightpar)
            {
                Advance();
            }
            else
            {
                ReportError(6);
            }
            return "enum";
        }

        if (cur == Sym.Intc)
        {
            Advance();
            if (LexicalAnalyzer.Symbol == Sym.Twopoints)
            {
                Advance();
                if (LexicalAnalyzer.Symbol == Sym.Intc)
                {
                    Advance();
                }
                else
                {
                    ReportError(16);
                }
            }
            else
            {
                ReportError(17);
            }
            return "subrange";
        }

        if (cur == Sym.Ident)
        {
            string typeName = LexicalAnalyzer.AddrName;
            TextPosition typePos = LexicalAnalyzer.Token;
            if (!IsStandardType(typeName))
            {
                ReportError(28, typePos);
            }
            Advance();
            return typeName.ToLower();
        }

        ReportError(18);
        return "";
    }

    private void FunctionDeclaration()
    {
        Advance();

        string funcName = "";
        TextPosition funcPos = LexicalAnalyzer.Token;

        if (LexicalAnalyzer.Symbol == Sym.Ident)
        {
            funcName = LexicalAnalyzer.AddrName;
            Advance();
        }
        else
        {
            ReportError(13);
        }

        SymbolTable savedScope = _scope;
        _scope = new SymbolTable(savedScope);

        List<Symbol> parameters = new List<Symbol>();
        if (LexicalAnalyzer.Symbol == Sym.Leftpar)
        {
            ParameterList(parameters);
        }

        if (LexicalAnalyzer.Symbol == Sym.Colon)
        {
            Advance();
        }
        else
        {
            ReportError(15);
        }

        string returnType = TypeDescription();

        if (funcName != "")
        {
            if (!savedScope.AddFunction(funcName, returnType, parameters))
            {
                ReportError(24, funcPos);
            }
            _scope.AddVariable(funcName, returnType);
        }

        if (LexicalAnalyzer.Symbol == Sym.Semicolon)
        {
            Advance();
        }
        else
        {
            ReportError(4);
        }

        if (LexicalAnalyzer.Symbol == Sym.Varsy)
        {
            VarSection();
        }

        CompoundStatement();

        _scope = savedScope;

        if (LexicalAnalyzer.Symbol == Sym.Semicolon)
        {
            Advance();
        }
        else
        {
            ReportError(4);
        }
    }

    private void ParameterList(List<Symbol> outParams)
    {
        Advance();

        ParameterGroup(outParams);
        while (LexicalAnalyzer.Symbol == Sym.Semicolon)
        {
            Advance();
            ParameterGroup(outParams);
        }

        if (LexicalAnalyzer.Symbol == Sym.Rightpar)
        {
            Advance();
        }
        else
        {
            ReportError(6);
        }
    }

    private void ParameterGroup(List<Symbol> outParams)
    {
        List<IdentRef> names = IdentifierList();

        if (LexicalAnalyzer.Symbol == Sym.Colon)
        {
            Advance();
        }
        else
        {
            ReportError(15);
        }

        string type = TypeDescription();

        foreach (IdentRef ir in names)
        {
            if (!_scope.AddParameter(ir.Name, type, ir.Position.LineNumber))
            {
                ReportError(24, ir.Position);
            }
            outParams.Add(new Symbol(ir.Name, type, ir.Position.LineNumber));
        }
    }

    private void CompoundStatement()
    {
        if (LexicalAnalyzer.Symbol == Sym.Beginsy)
        {
            Advance();
        }
        else
        {
            ReportError(3);
            SkipTo(Sym.Beginsy, Sym.Endsy, Sym.Point);
            if (LexicalAnalyzer.Symbol == Sym.Beginsy)
            {
                Advance();
            }
        }

        Statement();
        while (LexicalAnalyzer.Symbol == Sym.Semicolon)
        {
            Advance();
            Statement();
        }

        if (LexicalAnalyzer.Symbol == Sym.Endsy)
        {
            Advance();
        }
        else
        {
            ReportError(2);
            SkipTo(Sym.Endsy, Sym.Point, Sym.Semicolon);
            if (LexicalAnalyzer.Symbol == Sym.Endsy)
            {
                Advance();
            }
        }
    }

    private void Statement()
    {
        Sym cur = LexicalAnalyzer.Symbol;

        if (cur == Sym.Ident)
        {
            AssignmentStatement();
        }
        else if (cur == Sym.Beginsy)
        {
            CompoundStatement();
        }
        else if (IsUnsupportedStatement(cur))
        {
            SkipTo(Sym.Semicolon, Sym.Endsy);
        }
        else if (cur != Sym.Semicolon &&
                 cur != Sym.Endsy &&
                 cur != Sym.Unknown &&
                 cur != Sym.Point)
        {
            ReportError(22);
            Advance();
        }
    }

    private void AssignmentStatement()
    {
        string lhsName = LexicalAnalyzer.AddrName;
        TextPosition lhsPos = LexicalAnalyzer.Token;

        Symbol lhsSym = _scope.GetSymbol(lhsName);
        string lhsType = "";

        if (lhsSym == null)
        {
            ReportError(23, lhsPos);
        }
        else if (lhsSym.Kind == "function")
        {
            ReportError(25, lhsPos);
        }
        else
        {
            lhsType = lhsSym.Type;
        }

        Advance();

        if (LexicalAnalyzer.Symbol == Sym.Assign)
        {
            Advance();
        }
        else
        {
            ReportError(8);
            SkipTo(Sym.Semicolon, Sym.Endsy);
            return;
        }

        string rhsType = Expression();
        if (lhsType != "" && rhsType != "" && !IsAssignable(lhsType, rhsType))
        {
            ReportError(29, lhsPos);
        }
    }

    // выражение = простое_выражение [отношение простое_выражение]
    private string Expression()
    {
        string leftType = SimpleExpression();

        if (IsRelation(LexicalAnalyzer.Symbol))
        {
            Sym op = LexicalAnalyzer.Symbol;
            TextPosition opPos = LexicalAnalyzer.Token;
            Advance();
            string rightType = SimpleExpression();
            leftType = CheckBinaryOperator(op, leftType, rightType, opPos);
        }

        return leftType;
    }

    private string SimpleExpression()
    {
        TextPosition signPos = LexicalAnalyzer.Token;
        bool hasUnarySign =
            LexicalAnalyzer.Symbol == Sym.Plus ||
            LexicalAnalyzer.Symbol == Sym.Minus;

        if (hasUnarySign)
        {
            Advance();
        }

        string leftType = Term();

        if (hasUnarySign && leftType != "" && !IsNumeric(leftType))
        {
            ReportError(31, signPos);
            leftType = "";
        }

        while (LexicalAnalyzer.Symbol == Sym.Plus ||
               LexicalAnalyzer.Symbol == Sym.Minus ||
               LexicalAnalyzer.Symbol == Sym.Orsy)
        {
            Sym op = LexicalAnalyzer.Symbol;
            TextPosition opPos = LexicalAnalyzer.Token;
            Advance();
            string rightType = Term();
            leftType = CheckBinaryOperator(op, leftType, rightType, opPos);
        }

        return leftType;
    }

    private string Term()
    {
        string leftType = Factor();

        while (LexicalAnalyzer.Symbol == Sym.Star ||
               LexicalAnalyzer.Symbol == Sym.Slash ||
               LexicalAnalyzer.Symbol == Sym.Divsy ||
               LexicalAnalyzer.Symbol == Sym.Modsy ||
               LexicalAnalyzer.Symbol == Sym.Andsy)
        {
            Sym op = LexicalAnalyzer.Symbol;
            TextPosition opPos = LexicalAnalyzer.Token;
            Advance();
            string rightType = Factor();
            leftType = CheckBinaryOperator(op, leftType, rightType, opPos);
        }

        return leftType;
    }

    private string Factor()
    {
        Sym cur = LexicalAnalyzer.Symbol;

        switch (cur)
        {
            case Sym.Ident:
                return FactorIdentifier();

            case Sym.Intc:
                Advance();
                return "integer";

            case Sym.Floatc:
                Advance();
                return "real";

            case Sym.Stringc:
                string sType = LexicalAnalyzer.AddrString.Length == 1
                    ? "char" : "string";
                Advance();
                return sType;

            case Sym.Leftpar:
                Advance();
                string innerType = Expression();
                if (LexicalAnalyzer.Symbol == Sym.Rightpar)
                {
                    Advance();
                }
                else
                {
                    ReportError(6);
                }
                return innerType;

            case Sym.Notsy:
                TextPosition notPos = LexicalAnalyzer.Token;
                Advance();
                string operandType = Factor();
                if (operandType != "" && operandType != "boolean")
                {
                    ReportError(30, notPos);
                }
                return "boolean";

            default:
                ReportError(19);
                Advance();
                return "";
        }
    }

    private string FactorIdentifier()
    {
        string identName = LexicalAnalyzer.AddrName;
        TextPosition identPos = LexicalAnalyzer.Token;
        Symbol identSym = _scope.GetSymbol(identName);

        Advance();

        if (LexicalAnalyzer.Symbol == Sym.Leftpar)
        {
            Advance();

            List<string> argTypes = new List<string>();
            if (LexicalAnalyzer.Symbol != Sym.Rightpar)
            {
                argTypes.Add(Expression());
                while (LexicalAnalyzer.Symbol == Sym.Comma)
                {
                    Advance();
                    argTypes.Add(Expression());
                }
            }

            if (LexicalAnalyzer.Symbol == Sym.Rightpar)
            {
                Advance();
            }
            else
            {
                ReportError(6);
            }

            if (identSym == null)
            {
                ReportError(23, identPos);
                return "";
            }
            if (identSym.Kind != "function")
            {
                ReportError(26, identPos);
                return "";
            }
            if (identSym.Parameters.Count != argTypes.Count)
            {
                ReportError(27, identPos);
            }
            else
            {
                for (int i = 0; i < argTypes.Count; i++)
                {
                    string paramType = identSym.Parameters[i].Type;
                    if (!IsAssignable(paramType, argTypes[i]))
                    {
                        ReportError(29, identPos);
                    }
                }
            }
            return identSym.Type;
        }
        else
        {
            if (identSym == null)
            {
                ReportError(23, identPos);
                return "";
            }
            if (identSym.Kind == "function" &&
                identSym.Parameters.Count > 0)
            {
                ReportError(27, identPos);
            }
            return identSym.Type;
        }
    }
}