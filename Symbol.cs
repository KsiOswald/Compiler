class Symbol
{
    private string _name;
    private string _type;
    private string _kind;
    private uint _line;
    private List<Symbol> _parameters;

    public string Name
    {
        get
        {
            return _name;
        }
    }

    public string Type
    {
        get
        {
            return _type;
        }
    }

    public string Kind
    {
        get
        {
            return _kind;
        }
    }

    public uint Line
    {
        get
        {
            return _line;
        }
    }

    public List<Symbol> Parameters
    {
        get
        {
            return _parameters;
        }
    }

    public Symbol(string name, string type)
    {
        _name = name;
        _type = type;
        _kind = "variable";
        _line = 0;
        _parameters = null;
    }

    public Symbol(string name, string type, uint line)
    {
        _name = name;
        _type = type;
        _kind = "parameter";
        _line = line;
        _parameters = null;
    }

    public Symbol(string name, string returnType, List<Symbol> parameters)
    {
        _name = name;
        _type = returnType;
        _kind = "function";
        _line = 0;
        if (parameters == null)
        {
            _parameters = new List<Symbol>();
        }
        else
        {
            _parameters = parameters;
        }
    }
}
