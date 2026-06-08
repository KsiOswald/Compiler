struct IdentRef
{
    private string _name;
    private TextPosition _position;

    public string Name
    {
        get
        {
            return _name;
        }
    }

    public TextPosition Position
    {
        get
        {
            return _position;
        }
    }

    public IdentRef(string name, TextPosition position)
    {
        _name = name;
        _position = position;
    }
}
