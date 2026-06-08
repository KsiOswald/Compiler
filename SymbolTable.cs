class SymbolTable
{
    private Dictionary<string, Symbol> _symbols;
    private SymbolTable _parent;

    public SymbolTable Parent
    {
        get
        {
            return _parent;
        }
    }

    public SymbolTable() : this(null)
    {

    }

    public SymbolTable(SymbolTable parent)
    {
        _symbols = new Dictionary<string, Symbol>();
        _parent = parent;
    }

    public bool AddVariable(string name, string type)
    {
        string key = name.ToLower();
        if (_symbols.ContainsKey(key))
        {
            return false;
        }

        _symbols[key] = new Symbol(name, type);
        return true;
    }

    public bool AddParameter(string name, string type, uint line)
    {
        string key = name.ToLower();
        if (_symbols.ContainsKey(key))
        {
            return false;
        }

        _symbols[key] = new Symbol(name, type, line);
        return true;
    }

    public bool AddFunction(string name, string returnType,
        List<Symbol> parameters)
    {
        if (name == null)
        {
            return false;
        }

        string key = name.ToLower();
        if (_symbols.ContainsKey(key))
        {
            return false;
        }

        _symbols[key] = new Symbol(name, returnType, parameters);
        return true;
    }

    public Symbol GetSymbol(string name)
    {
        if (name == null)
        {
            return null;
        }

        string key = name.ToLower();
        Symbol symbol;

        if (_symbols.TryGetValue(key, out symbol))
        {
            return symbol;
        }

        if (_parent != null)
        {
            return _parent.GetSymbol(name);
        }

        return null;
    }

    public bool ContainsVariable(string name)
    {
        Symbol symbol = GetSymbol(name);
        if (symbol == null)
        {
            return false;
        }
        return symbol.Kind == "variable" ||
               symbol.Kind == "parameter";
    }

    public bool ContainsFunction(string name)
    {
        Symbol symbol = GetSymbol(name);
        if (symbol == null)
        {
            return false;
        }
        return symbol.Kind == "function";
    }
}
