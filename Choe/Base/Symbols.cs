namespace Choe
{
  public static class Symbols
  {
    public static char NotOperator = '¬';
    public static char AndOperator = '&';
    public static char OrOperator = "\\"[0];
    public static char IdenticallyOperator = '≡';
    public static char ApproximatelyOperator = '≈';
    public static char NotequalOperator = '≠';
    public static char GreaterOperator = '>';
    public static char LessOperator = '<';
    public static char GreaterorequalOperator = '≥';
    public static char LessorequalOperator = '≤';
    public static char MinusOperator = '-';
    public static char AddOperator = '+';
    public static char SubtractOperator = Symbols.MinusOperator;
    public static char MultiplyOperator = '*';
    public static char DivideOperator = '/';
    public static char PowerOperator = '^';
    public static char FactorialOperator = '!';
    public static char ApostropheOperator = "'"[0];
    public static char AccentOperator = '`';
    public static char DotOperator = '•';
    public static char CrossOperator = '×';
    public static char TildeOperator = '~';
    public static char SquareRootOperator = '√';
    public static char QuestionOperator = '?';
    public static char NumberOperator = '#';
    public static char DeltaOperator = '∆';
    public static char SumOperator = '∑';
    public static char ProductOperator = '∏';
    public static char LeftarrowOperator = '←';
    public static char RightarrowOperator = '→';
    public static char UparrowOperator = '↑';
    public static char DownarrowOperator = '↓';
    public static char LeftrightarrowOperator = '↔';
    public static char UpdownarrowOperator = '↕';
    public static char DerivativeOperator = '∂';
    public static char IntegralOperator = '∫';
    public static char AbsoluteOperator = '|';
    public static char NormOperator = '‖';
    public static char AssignementOperator = '=';
    public static char ArgumentSeparator = ' ';
    public static char VectorItemSeparator = ' ';
    public static char DerivativeSeparator = '/';
    public static char ImaginarySymbol = 'I';
    public static char UnitSymbol = '1';
    /// <summary>Zero symbol</summary>
    public static char ZeroSymbol = '0';
    /// <summary>Imaginary string</summary>
    public static string ImaginaryString = Symbols.ImaginarySymbol.ToString();
    /// <summary>Unit string</summary>
    public static string UnitString = Symbols.UnitSymbol.ToString();
    /// <summary>Zero string</summary>
    public static string ZeroString = Symbols.ZeroSymbol.ToString();
    public static char InfinitySymbol = '∞';
    /// <summary>Infinity string (short)</summary>
    public static string InfinityName = "Inf";
    /// <summary>Infinity string (full)</summary>
    public static string InfinityFullName = "Infinity";
    /// <summary>NaN string</summary>
    public static string NaNName = "NaN";
    /// <summary>Infinity string (symbol)</summary>
    public static string InfinityString = Symbols.InfinitySymbol.ToString();
    /// <summary>Pi symbol</summary>
    public static char PiSymbol = 'π';
    /// <summary>Pi string</summary>
    public static string PiName = "Pi";
    /// <summary>e symbol</summary>
    public static char EulerSymbol = 'e';
    /// <summary>Pi string (symbol)</summary>
    public static string PiString = Symbols.PiSymbol.ToString();
    /// <summary>e string (symbol)</summary>
    public static string EulerString = Symbols.EulerSymbol.ToString();
    /// <summary>True constant</summary>
    public static string TrueName = "True";
    /// <summary>False constant</summary>
    public static string FalseName = "False";
    /// <summary>
    /// Universal zero symbol
    /// TODO: not realized (how to use?)
    /// </summary>
    public static char UniversalZeroSymbol = 'Ø';
    /// <summary>Universal zero string</summary>
    public static string UniversalZeroString = Symbols.UniversalZeroSymbol.ToString();
    /// <summary>Function left</summary>
    public static char FunctionLeftBracket = '(';
    /// <summary>Function right</summary>
    public static char FunctionRightBracket = ')';
    /// <summary>Index left</summary>
    public static char IndexLeftBracket = '[';
    /// <summary>Index right</summary>
    public static char IndexRightBracket = ']';
    /// <summary>Parameter left</summary>
    public static char ParameterLeftBracket = '{';
    /// <summary>Parameter right</summary>
    public static char ParameterRightBracket = '}';
    /// <summary>Left double quotation</summary>
    public static char LeftDoublequotation = '“';
    /// <summary>Right double quotation</summary>
    public static char RightDoublequotation = '”';
    /// <summary>Left quotation</summary>
    public static char LeftQuotation = '‘';
    /// <summary>Right quotation</summary>
    public static char RightQuotation = '’';
    /// <summary>Left triangle</summary>
    public static char LeftTrianglebracket = '‹';
    /// <summary>Right triangle</summary>
    public static char RightTrianglebracket = '›';
    /// <summary>Left double triangle</summary>
    public static char LeftDoubletrianglebracket = '«';
    /// <summary>Right double triangle</summary>
    public static char RightDoubletrianglebracket = '»';
    /// <summary>Function operator sign</summary>
    public static string FunctionSign = "->";
    /// <summary>Null string</summary>
    public static string NullValue = "NULL";
    /// <summary>Replace symbol</summary>
    public static string ReplacerSign = "_";
    /// <summary>Not defined string</summary>
    public static string NotDefinedSign = "NOT_DEFINED";
  }
}
