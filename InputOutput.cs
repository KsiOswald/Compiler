using System;
using System.Collections.Generic;
using System.IO;

class InputOutput
{
    const byte ERRMAX = 9;

    private static char _ch;
    private static TextPosition _positionNow;
    private static List<Err> _err;
    private static bool _isEof;

    private static string _line;
    private static byte _lastInLine;
    private static StreamReader _file;
    private static uint _errCount;
    public static char Ch
    {
        get
        {
            return _ch;
        }
        set
        {
            _ch = value;
        }
    }

    public static TextPosition PositionNow
    {
        get
        {
            return _positionNow;
        }
        set
        {
            _positionNow = value;
        }
    }
    
    public static List<Err> Err
    {
        get
        {
            return _err;
        }
        set
        {
            _err = value;
        }
    }


    public static bool IsEof
    {
        get
        {
            return _isEof;
        }
    }

    static public void Init(string inputPath)
    {
        if (!File.Exists(inputPath))
        {
            return;
        }
        _isEof = false;
        _positionNow = new TextPosition();
        _file = new StreamReader(inputPath);
        _err = new List<Err>();
        _errCount = 0;
        _positionNow.LineNumber = 0;
        _positionNow.CharNumber = 0;
        _lastInLine = 0;
        if (!_file.EndOfStream)
        {
            _line = _file.ReadLine() + " ";
            _lastInLine = (byte)(_line.Length - 1);
            _ch = _line[0];
            _positionNow.LineNumber = 1;
            _positionNow.CharNumber = 0;
        }
        else
        {
            _line = " ";
            _lastInLine = 0;
            _ch = (char)0;
        }
    }

    static public void NextCh()
    {
        if (_isEof)
        {
            return;
        }
        if (PositionNow.CharNumber == _lastInLine)
        {
            ListThisLine();
            if (Err.Count > 0)
            {
                ListErrors();
            }
            ReadNextLine();
            if (_isEof)
            {
                return;
            }
            ++_positionNow.LineNumber;
            _positionNow.CharNumber = 0;
        }
        else
        {
            ++_positionNow.CharNumber;
        }
        Ch = _line[PositionNow.CharNumber];
    }

    private static void ListThisLine()
    {
        _line = "      " + _line;
        Console.WriteLine(_line);
    }

    private static void ReadNextLine()
    {
        if (!_file.EndOfStream)
        {
            _line = _file.ReadLine() + " ";
            _lastInLine = (byte)(_line.Length - 1);
            _err = new List<Err>();
        }
        else
        {
            End();
        }
    }
    static void End()
    {
        _isEof = true;
        _ch = (char)0;
        _file?.Close();
        Console.WriteLine($"Компиляция завершена:" +
            $" ошибок — {_errCount}!");
    }

    static void ListErrors()
    {
        int pos = 6 - $"{PositionNow.LineNumber}".Length;
        string s;
        foreach (Err item in Err)
        {
            ++_errCount;
            s = "**";
            if (_errCount < 10) s += "0";
            s += $"{_errCount}**";
            while (s.Length - 1 < pos + item.ErrorPosition.CharNumber)
            {
                s += " ";
            }
            s += $"^ ошибка код {item.ErrorCode}";
            Console.WriteLine(s);
        }
    }

    static public void Error(TextPosition position,  byte errorCode)
    {
        if (_err == null)
        {
            return;
        }
        if (_err.Count <= ERRMAX)
        {
            _err.Add(new Err(position, errorCode));
        }
    }
}