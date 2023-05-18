using System;
using System.Collections.Generic;

namespace Choe.Syntactic
{
  public static class Elements
  {
    public static readonly List<char> Letters = new List<char>();
    public static readonly List<char> Digits;
    public static readonly SymbolPair FunctionBrackets;
    public static readonly SymbolPair IndexBrackets;
    public static readonly SymbolPair ParameterBrackets;
    public static readonly SymbolPair VectorBrackets;
    public static readonly List<SymbolPair> Brackets;
    public static readonly List<char> LogicalOperators;
    public static readonly List<char> RelationalOperators;
    public static readonly List<char> SummOperators;
    public static readonly List<char> ProductOperators;
    public static readonly List<char> PowerOperators;
    public static readonly List<char> ArrowOperators;
    public static readonly List<char> PrefixOperators;
    public static readonly List<char> PostfixOperators;
    public static readonly List<char> OutfixOperators;
    public static readonly List<char> BinaryOperators;
    public static readonly List<char> SpecialOperators;
    public static readonly List<char> OpeningBrackets;
    public static readonly List<char> ClosingBrackets;
    public static readonly List<string> Operators;
    public static readonly List<string> AuxiliarySymbols;
    public static readonly SymbolPair Quotations;
    public static readonly SymbolPair DoubleQuotations;
    public static readonly SymbolPair TriangleBrackets;
    public static readonly SymbolPair DoubletriangleBrackets;
    public static readonly List<SymbolPair> SpecialPairs;
    public static readonly List<char> FutureOperators;

    static Elements()
    {
      for (char ch = 'A'; ch <= 'Z'; ++ch)
        Elements.Letters.Add(ch);
      for (char ch = 'a'; ch <= 'z'; ++ch)
        Elements.Letters.Add(ch);
      Elements.Letters.AddRange((IEnumerable<char>) NRTE.Symbols.GreekLetterrs);
      Elements.Digits = new List<char>();
      for (char ch = '0'; ch <= '9'; ++ch)
        Elements.Digits.Add(ch);
      Elements.FunctionBrackets = new SymbolPair(Choe.Symbols.FunctionLeftBracket, Choe.Symbols.FunctionRightBracket);
      Elements.IndexBrackets = new SymbolPair(Choe.Symbols.IndexLeftBracket, Choe.Symbols.IndexRightBracket);
      Elements.ParameterBrackets = new SymbolPair(Choe.Symbols.ParameterLeftBracket, Choe.Symbols.ParameterRightBracket);
      Elements.VectorBrackets = Elements.IndexBrackets;
      Elements.Brackets = new List<SymbolPair>((IEnumerable<SymbolPair>) new SymbolPair[3]
      {
        Elements.FunctionBrackets,
        Elements.IndexBrackets,
        Elements.ParameterBrackets
      });
      Elements.LogicalOperators = new List<char>((IEnumerable<char>) new char[2]
      {
        Choe.Symbols.AndOperator,
        Choe.Symbols.OrOperator
      });
      Elements.RelationalOperators = new List<char>((IEnumerable<char>) new char[7]
      {
        Choe.Symbols.IdenticallyOperator,
        Choe.Symbols.ApproximatelyOperator,
        Choe.Symbols.NotequalOperator,
        Choe.Symbols.GreaterOperator,
        Choe.Symbols.LessOperator,
        Choe.Symbols.GreaterorequalOperator,
        Choe.Symbols.LessorequalOperator
      });
      Elements.SummOperators = new List<char>((IEnumerable<char>) new char[2]
      {
        Choe.Symbols.AddOperator,
        Choe.Symbols.SubtractOperator
      });
      Elements.ProductOperators = new List<char>((IEnumerable<char>) new char[4]
      {
        Choe.Symbols.MultiplyOperator,
        Choe.Symbols.DivideOperator,
        Choe.Symbols.DotOperator,
        Choe.Symbols.CrossOperator
      });
      Elements.PowerOperators = new List<char>((IEnumerable<char>) new char[1]
      {
        Choe.Symbols.PowerOperator
      });
      Elements.ArrowOperators = new List<char>((IEnumerable<char>) new char[6]
      {
        Choe.Symbols.LeftarrowOperator,
        Choe.Symbols.RightarrowOperator,
        Choe.Symbols.UparrowOperator,
        Choe.Symbols.DownarrowOperator,
        Choe.Symbols.LeftrightarrowOperator,
        Choe.Symbols.UpdownarrowOperator
      });
      Elements.SpecialOperators = new List<char>((IEnumerable<char>) new char[5]
      {
        Choe.Symbols.DerivativeOperator,
        Choe.Symbols.IntegralOperator,
        Choe.Symbols.DeltaOperator,
        Choe.Symbols.SumOperator,
        Choe.Symbols.ProductOperator
      });
      Elements.PrefixOperators = new List<char>((IEnumerable<char>) new char[9]
      {
        Choe.Symbols.NotOperator,
        Choe.Symbols.MinusOperator,
        Choe.Symbols.SquareRootOperator,
        Choe.Symbols.TildeOperator,
        Choe.Symbols.QuestionOperator,
        Choe.Symbols.NumberOperator,
        Choe.Symbols.DeltaOperator,
        Choe.Symbols.SumOperator,
        Choe.Symbols.ProductOperator
      });
      Elements.PostfixOperators = new List<char>((IEnumerable<char>) new char[3]
      {
        Choe.Symbols.FactorialOperator,
        Choe.Symbols.ApostropheOperator,
        Choe.Symbols.AccentOperator
      });
      Elements.OutfixOperators = new List<char>((IEnumerable<char>) new char[2]
      {
        Choe.Symbols.AbsoluteOperator,
        Choe.Symbols.NormOperator
      });
      Elements.BinaryOperators = new List<char>();
      Elements.BinaryOperators.AddRange((IEnumerable<char>) Elements.LogicalOperators);
      Elements.BinaryOperators.AddRange((IEnumerable<char>) Elements.RelationalOperators);
      Elements.BinaryOperators.AddRange((IEnumerable<char>) Elements.SummOperators);
      Elements.BinaryOperators.AddRange((IEnumerable<char>) Elements.ProductOperators);
      Elements.BinaryOperators.AddRange((IEnumerable<char>) Elements.PowerOperators);
      Elements.BinaryOperators.AddRange((IEnumerable<char>) Elements.ArrowOperators);
      Elements.OpeningBrackets = new List<char>((IEnumerable<char>) new char[3]
      {
        Choe.Symbols.FunctionLeftBracket,
        Choe.Symbols.ParameterLeftBracket,
        Choe.Symbols.IndexLeftBracket
      });
      Elements.ClosingBrackets = new List<char>((IEnumerable<char>) new char[3]
      {
        Choe.Symbols.FunctionRightBracket,
        Choe.Symbols.ParameterRightBracket,
        Choe.Symbols.IndexRightBracket
      });
      Array values = Enum.GetValues(typeof (OperatorType));
      Elements.Operators = new List<string>();
      for (int index = 1; index < values.Length; ++index)
      {
        string operatorSign = Utilities.GetOperatorSign((OperatorType) values.GetValue(index));
        if (!Elements.Operators.Contains(operatorSign))
          Elements.Operators.Add(operatorSign);
      }
      Elements.AuxiliarySymbols = new List<string>();
      Elements.AuxiliarySymbols.Add(Choe.Symbols.InfinityString);
      Elements.AuxiliarySymbols.Add(Choe.Symbols.ImaginaryString);
      foreach (SymbolPair bracket in Elements.Brackets)
        Elements.AuxiliarySymbols.Add(bracket.ToString());
      Elements.Quotations = new SymbolPair(Choe.Symbols.LeftQuotation, Choe.Symbols.RightQuotation);
      Elements.DoubleQuotations = new SymbolPair(Choe.Symbols.LeftDoublequotation, Choe.Symbols.RightDoublequotation);
      Elements.TriangleBrackets = new SymbolPair(Choe.Symbols.LeftTrianglebracket, Choe.Symbols.RightTrianglebracket);
      Elements.DoubletriangleBrackets = new SymbolPair(Choe.Symbols.LeftDoubletrianglebracket, Choe.Symbols.RightDoubletrianglebracket);
      Elements.SpecialPairs = new List<SymbolPair>((IEnumerable<SymbolPair>) new SymbolPair[4]
      {
        Elements.Quotations,
        Elements.DoubleQuotations,
        Elements.TriangleBrackets,
        Elements.DoubletriangleBrackets
      });
      foreach (SymbolPair specialPair in Elements.SpecialPairs)
        Elements.AuxiliarySymbols.Add(specialPair.ToString());
      foreach (char greekLetterr in NRTE.Symbols.GreekLetterrs)
        Elements.AuxiliarySymbols.Add(greekLetterr.ToString());
      foreach (char subscriptDigit in NRTE.Symbols.SubscriptDigits)
        Elements.AuxiliarySymbols.Add(subscriptDigit.ToString());
      foreach (char superscriptDigit in NRTE.Symbols.SuperscriptDigits)
        Elements.AuxiliarySymbols.Add(superscriptDigit.ToString());
      Elements.FutureOperators = new List<char>((IEnumerable<char>) new char[0]);
      foreach (char futureOperator in Elements.FutureOperators)
        Elements.AuxiliarySymbols.Add(futureOperator.ToString());
    }
  }
}
