struct TextPosition
{
    private uint _lineNumber;
    private byte _charNumber;

    public TextPosition()
    {
        LineNumber = 0;
        CharNumber = 0;
    }

    public uint LineNumber
    {
        get 
        {
            return _lineNumber;
        }
        set 
        {
            _lineNumber = value;
        }
    }
    public byte CharNumber
    {
        get
        {
            return _charNumber;
        }
        set
        {
            _charNumber = value;
        }
    }
}