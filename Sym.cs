enum Sym : byte
{
    Unknown      = 0,    // служебное «нет лексемы» (EOF, пропущенный символ)
    Ident        = 2,    // идентификатор
    Rightpar     = 4,    // )
    Colon        = 5,    // :
    Leftpar      = 9,    // (
    Lbracket     = 11,   // [
    Rbracket     = 12,   // ]
    Semicolon    = 14,   // ;
    Intc         = 15,   // целая константа
    Equal        = 16,   // =
    Comma        = 20,   // ,
    Star         = 21,   // *
    Casesy       = 31,   // case
    Elsesy       = 32,   // else
    Gotosy       = 33,   // goto
    Typesy       = 34,   // type
    Withsy       = 37,   // with
    Assign       = 51,   // :=
    Thensy       = 52,   // then
    Untilsy      = 53,   // until
    Dosy         = 54,   // do
    Ifsy         = 56,   // if
    Filesy       = 57,   // file
    Slash        = 60,   // /
    Point        = 61,   // .
    Arrow        = 62,   // ^
    Flpar        = 63,   // {
    Frpar        = 64,   // }
    Later        = 65,   // <
    Greater      = 66,   // >
    Laterequal   = 67,   // <=
    Greaterequal = 68,   // >=
    Latergreater = 69,   // <>
    Plus         = 70,   // +
    Minus        = 71,   // -
    Lcomment     = 72,   // (*
    Rcomment     = 73,   // *)
    Twopoints    = 74,   // ..
    Floatc       = 82,   // вещественная константа
    Insy         = 100,  // in
    Ofsy         = 101,  // of
    Orsy         = 102,  // or
    Tosy         = 103,  // to
    Endsy        = 104,  // end
    Varsy        = 105,  // var
    Divsy        = 106,  // div
    Andsy        = 107,  // and
    Notsy        = 108,  // not
    Forsy        = 109,  // for
    Modsy        = 110,  // mod
    Nilsy        = 111,  // nil
    Setsy        = 112,  // set
    Beginsy      = 113,  // begin
    Whilesy      = 114,  // while
    Arraysy      = 115,  // array
    Constsy      = 116,  // const
    Labelsy      = 117,  // label
    Downtosy     = 118,  // downto
    Packedsy     = 119,  // packed
    Recordsy     = 120,  // record
    Repeatsy     = 121,  // repeat
    Programsy    = 122,  // program
    Functionsy   = 123,  // function
    Procedurensy = 124,  // procedure
}
