using System.Collections.Generic;

static class ErrorTable
{
    private static Dictionary<byte, string> _messages
        = new Dictionary<byte, string>
        {
            { 1,  "Ожидался program" },
            { 2,  "Ожидался end" },
            { 3,  "Ожидался begin" },
            { 4,  "Ожидался ;" },
            { 5,  "Недопустимый символ" },
            { 6,  "Ожидался )" },
            { 7,  "Ожидался =" },
            { 8,  "Ожидался :=" },
            { 9,  "Ожидался do" },
            { 10, "Ожидался if" },
            { 11, "Целое число вне допустимого диапазона" },
            { 12, "Ожидался символ начала или окончания строкового значения " }
        };

    public static string GetMessage(byte code)
    {
        if (_messages.TryGetValue(code, out string msg))
        {
            return msg;
        }
        return "неизвестная ошибка";
    }
}