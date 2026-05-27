using System.Collections.Generic;

class Keywords
{
    private Dictionary<byte, Dictionary<string, Sym>> _kw;

    public Keywords()
    {
        _kw = new Dictionary<byte, Dictionary<string, Sym>>
        {
            [2] = new Dictionary<string, Sym>
            {
                ["do"] = Sym.Dosy,
                ["if"] = Sym.Ifsy,
                ["in"] = Sym.Insy,
                ["of"] = Sym.Ofsy,
                ["or"] = Sym.Orsy,
                ["to"] = Sym.Tosy,
            },
            [3] = new Dictionary<string, Sym>
            {
                ["end"] = Sym.Endsy,
                ["var"] = Sym.Varsy,
                ["div"] = Sym.Divsy,
                ["and"] = Sym.Andsy,
                ["not"] = Sym.Notsy,
                ["for"] = Sym.Forsy,
                ["mod"] = Sym.Modsy,
                ["nil"] = Sym.Nilsy,
                ["set"] = Sym.Setsy,
            },
            [4] = new Dictionary<string, Sym>
            {
                ["then"] = Sym.Thensy,
                ["else"] = Sym.Elsesy,
                ["case"] = Sym.Casesy,
                ["file"] = Sym.Filesy,
                ["goto"] = Sym.Gotosy,
                ["type"] = Sym.Typesy,
                ["with"] = Sym.Withsy,
            },
            [5] = new Dictionary<string, Sym>
            {
                ["begin"] = Sym.Beginsy,
                ["while"] = Sym.Whilesy,
                ["array"] = Sym.Arraysy,
                ["const"] = Sym.Constsy,
                ["label"] = Sym.Labelsy,
                ["until"] = Sym.Untilsy,
            },
            [6] = new Dictionary<string, Sym>
            {
                ["downto"] = Sym.Downtosy,
                ["packed"] = Sym.Packedsy,
                ["record"] = Sym.Recordsy,
                ["repeat"] = Sym.Repeatsy,
            },
            [7] = new Dictionary<string, Sym> 
            {
                ["program"] = Sym.Programsy
            },
            [8] = new Dictionary<string, Sym>
            {
                ["function"] = Sym.Functionsy
            },
            [9] = new Dictionary<string, Sym>
            {
                ["procedure"] = Sym.Procedurensy
            },
        };
    }

    
    public Sym Lookup(string name)
    {
        byte len = (byte)name.Length;
        if (_kw.TryGetValue(len, out var dict) &&
            dict.TryGetValue(name, out Sym code))
        {
            return code;
        }
        return Sym.Ident;
    }
}