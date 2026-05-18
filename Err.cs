struct Err
{
    private TextPosition _errorPosition;
    private byte _errorCode;
    public Err(TextPosition errorPosition, byte errorCode)
    {
        ErrorPosition = errorPosition;
        ErrorCode = errorCode;
    }
    public TextPosition ErrorPosition
    {
        get
        {
            return _errorPosition;
        }
        set
        {
            _errorPosition = value;
        }
    }
    public byte ErrorCode
    {
        get
        {
            return _errorCode;
        }
        set
        {
            _errorCode = value;
        }
    }
}