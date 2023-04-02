using Choe.Syntactic;
using System.Collections.Generic;

namespace Choe
{
  public static class Syntax
  {
    private static List<SymbolPair> NotAllowedPairs;
    private static List<char> NotAllowedAtBeginning;
    private static List<char> NotAllowedAtEnd;
    private static List<SyntaxRule> rules;

    private static void RecreateSyntaxData()
    {
      Syntax.NotAllowedPairs = new List<SymbolPair>();
      PairUtilities.AddPairs(PairUtilities.GetAllPairs(Elements.BinaryOperators), Syntax.NotAllowedPairs);
      PairUtilities.RemovePairs(new List<SymbolPair>((IEnumerable<SymbolPair>) new SymbolPair[1]
      {
        new SymbolPair(Symbols.PowerOperator, Symbols.MinusOperator)
      }), Syntax.NotAllowedPairs);
      PairUtilities.RemovePairs(PairUtilities.GetAllPairs(new List<char>((IEnumerable<char>) Elements.RelationalOperators), new List<char>((IEnumerable<char>) new char[1]
      {
        Symbols.MinusOperator
      })), Syntax.NotAllowedPairs);
      PairUtilities.RemovePairs(PairUtilities.GetAllPairs(new List<char>((IEnumerable<char>) Elements.ArrowOperators), new List<char>((IEnumerable<char>) new char[1]
      {
        Symbols.MinusOperator
      })), Syntax.NotAllowedPairs);
      List<char> symbols1_1 = new List<char>((IEnumerable<char>) Elements.OpeningBrackets);
      symbols1_1.AddRange((IEnumerable<char>) Elements.BinaryOperators);
      List<char> symbols2_1 = new List<char>((IEnumerable<char>) Elements.PostfixOperators);
      PairUtilities.AddPairs(PairUtilities.GetAllPairs(symbols1_1, symbols2_1), Syntax.NotAllowedPairs);
      List<char> symbols1_2 = new List<char>((IEnumerable<char>) Elements.PrefixOperators);
      List<char> symbols2_2 = new List<char>((IEnumerable<char>) Elements.ClosingBrackets);
      symbols2_2.AddRange((IEnumerable<char>) Elements.BinaryOperators);
      PairUtilities.AddPairs(PairUtilities.GetAllPairs(symbols1_2, symbols2_2), Syntax.NotAllowedPairs);
      List<char> symbols1_3 = new List<char>((IEnumerable<char>) Elements.OpeningBrackets);
      List<char> symbols2_3 = new List<char>((IEnumerable<char>) Elements.BinaryOperators);
      List<SymbolPair> allPairs = PairUtilities.GetAllPairs(symbols1_3, symbols2_3);
      PairUtilities.RemovePairs(PairUtilities.GetAllPairs(symbols1_3, new List<char>()
      {
        Symbols.MinusOperator
      }), allPairs);
      PairUtilities.AddPairs(allPairs, Syntax.NotAllowedPairs);
      PairUtilities.AddPairs(PairUtilities.GetAllPairs(new List<char>((IEnumerable<char>) Elements.BinaryOperators), new List<char>((IEnumerable<char>) Elements.ClosingBrackets)), Syntax.NotAllowedPairs);
      Syntax.NotAllowedPairs.Add(new SymbolPair(Symbols.FunctionRightBracket, Symbols.FunctionLeftBracket));
      List<char> symbols1_4 = new List<char>((IEnumerable<char>) Elements.ClosingBrackets);
      List<char> symbols2_4 = new List<char>((IEnumerable<char>) Elements.Letters);
      symbols2_4.AddRange((IEnumerable<char>) Elements.Digits);
      PairUtilities.AddPairs(PairUtilities.GetAllPairs(symbols1_4, symbols2_4), Syntax.NotAllowedPairs);
      Syntax.NotAllowedAtBeginning = new List<char>();
      Syntax.NotAllowedAtEnd = new List<char>();
      List<char> binaryOperators = Elements.BinaryOperators;
      PairUtilities.AddSymbols(binaryOperators, Syntax.NotAllowedAtBeginning);
      PairUtilities.AddSymbols(binaryOperators, Syntax.NotAllowedAtEnd);
      PairUtilities.RemoveSymbols(new List<char>((IEnumerable<char>) new char[1]
      {
        Symbols.MinusOperator
      }), Syntax.NotAllowedAtBeginning);
      List<char> additional = new List<char>((IEnumerable<char>) new char[1]
      {
        Symbols.ArgumentSeparator
      });
      PairUtilities.AddSymbols(additional, Syntax.NotAllowedAtBeginning);
      PairUtilities.AddSymbols(additional, Syntax.NotAllowedAtEnd);
      PairUtilities.AddSymbols(Elements.PrefixOperators, Syntax.NotAllowedAtEnd);
      PairUtilities.AddSymbols(Elements.PostfixOperators, Syntax.NotAllowedAtBeginning);
    }

    private static void RecreateRules()
    {
      Syntax.rules = new List<SyntaxRule>();
      SyntaxRule syntaxRule1 = new SyntaxRule(new SyntaxRuleFunction(Syntax.ParitiesRule));
      Syntax.rules.Add(syntaxRule1);
      SyntaxRule syntaxRule2 = new SyntaxRule(new SyntaxRuleFunction(Syntax.ParanthesesRule));
      Syntax.rules.Add(syntaxRule2);
      SyntaxRule syntaxRule3 = new SyntaxRule(new SyntaxRuleFunction(Syntax.InvalidConstructionRule));
      Syntax.rules.Add(syntaxRule3);
      SyntaxRule syntaxRule4 = new SyntaxRule(new SyntaxRuleFunction(Syntax.ErrorSymbolRule));
      Syntax.rules.Add(syntaxRule4);
    }

    public static PairErrorData PairsRule(string value, List<SymbolPair> pairs)
    {
      PairErrorData pairErrorData = new PairErrorData(0, -1, (SymbolPair) null);
      if (string.IsNullOrEmpty(value) || pairs == null || pairs.Count == 0)
        return pairErrorData;
      int length = value.Length;
      List<SymbolPair> symbolPairList = new List<SymbolPair>();
      for (int index1 = 0; index1 < length; ++index1)
      {
        char c = value[index1];
        int index2 = Parser.IsOpening(c, pairs);
        if (index2 >= 0)
        {
          symbolPairList.Add(pairs[index2]);
        }
        else
        {
          int index3 = Parser.IsClosing(c, pairs);
          if (index3 >= 0)
          {
            SymbolPair pair = pairs[index3];
            if (symbolPairList.Count == 0)
            {
              pairErrorData.Error = -1;
              pairErrorData.ErrorIndex = index1;
              pairErrorData.ErrorPair = pair;
              break;
            }
            SymbolPair symbolPair = symbolPairList[symbolPairList.Count - 1];
            if (symbolPair == pair)
            {
              symbolPairList.RemoveAt(symbolPairList.Count - 1);
            }
            else
            {
              pairErrorData.Error = -2;
              pairErrorData.ErrorIndex = index1;
              pairErrorData.ErrorPair = new SymbolPair(symbolPair.Closing, pair.Closing);
              break;
            }
          }
        }
      }
      if (pairErrorData.Error == 0 && symbolPairList.Count > 0)
      {
        pairErrorData.Error = symbolPairList.Count;
        pairErrorData.ErrorIndex = length;
        pairErrorData.ErrorPair = symbolPairList[symbolPairList.Count - 1];
      }
      return pairErrorData;
    }

    public static CheckedData<SyntaxError> ParanthesesRule(string value)
    {
      CheckedData<SyntaxError> checkedData = new CheckedData<SyntaxError>(true, (SyntaxError) null);
      PairErrorData pairErrorData = Syntax.PairsRule(value, Elements.Brackets);
      checkedData.Is = pairErrorData.Error == 0;
      if (!checkedData.Is)
      {
        if (pairErrorData.Error == -1)
          checkedData.Value = (SyntaxError) new UnexpectedParanthesis(value, pairErrorData.ErrorIndex, pairErrorData.ErrorIndex, pairErrorData.ErrorPair);
        else if (pairErrorData.Error == -2)
          checkedData.Value = (SyntaxError) new ParanthesisExpectedButFound(value, pairErrorData.ErrorIndex, pairErrorData.ErrorIndex, pairErrorData.ErrorPair);
        else if (pairErrorData.Error > 0)
          checkedData.Value = (SyntaxError) new ParanthesisExpected(value, pairErrorData.ErrorIndex, pairErrorData.ErrorIndex, pairErrorData.ErrorPair);
      }
      return checkedData;
    }

    public static PairErrorData SymbolParityRule(string value, char parity, List<SymbolPair> pairs)
    {
      PairErrorData pairErrorData = new PairErrorData(0, -1, (SymbolPair) null);
      if (string.IsNullOrEmpty(value) || pairs == null || pairs.Count == 0)
        return pairErrorData;
      int num = 0;
      int length = value.Length;
      List<SymbolPair> symbolPairList = new List<SymbolPair>();
      for (int index1 = 0; index1 < length; ++index1)
      {
        char c = value[index1];
        if ((int) c == (int) parity)
        {
          if (num > 0)
            --num;
          else
            ++num;
          if (num == 0 && symbolPairList.Count > 0)
          {
            pairErrorData.Error = -symbolPairList.Count;
            pairErrorData.ErrorIndex = index1;
            pairErrorData.ErrorPair = new SymbolPair(symbolPairList[symbolPairList.Count - 1].Opening, parity);
            break;
          }
        }
        else if (num > 0)
        {
          int index2 = Parser.IsOpening(c, pairs);
          if (index2 >= 0)
          {
            symbolPairList.Add(pairs[index2]);
          }
          else
          {
            int index3 = Parser.IsClosing(c, pairs);
            if (index3 >= 0 && symbolPairList.Count > 0)
            {
              int index4 = symbolPairList.Count - 1;
              if (symbolPairList[index4] == pairs[index3])
                symbolPairList.RemoveAt(index4);
            }
          }
        }
      }
      if (num > 0)
      {
        pairErrorData.Error = 1;
        pairErrorData.ErrorIndex = length;
        pairErrorData.ErrorPair = new SymbolPair(parity, parity);
      }
      return pairErrorData;
    }

    public static CheckedData<SyntaxError> ParitiesRule(string value)
    {
      CheckedData<SyntaxError> checkedData = new CheckedData<SyntaxError>(true, (SyntaxError) null);
      int count = Elements.OutfixOperators.Count;
      for (int index = 0; index < count; ++index)
      {
        PairErrorData pairErrorData = Syntax.SymbolParityRule(value, Elements.OutfixOperators[index], Elements.Brackets);
        checkedData.Is = pairErrorData.Error == 0;
        if (!checkedData.Is)
        {
          checkedData.Value = pairErrorData.Error >= 0 ? (SyntaxError) new ClosingSymbolParityError(value, pairErrorData.ErrorIndex, pairErrorData.ErrorIndex, pairErrorData.ErrorPair) : (SyntaxError) new ParanthesisMustBeClosedError(value, pairErrorData.ErrorIndex, pairErrorData.ErrorIndex, pairErrorData.ErrorPair);
          return checkedData;
        }
      }
      return checkedData;
    }

    public static PairErrorData PairsNotAllowedRule(string value, List<SymbolPair> pairs)
    {
      PairErrorData pairErrorData = new PairErrorData(0, -1, (SymbolPair) null);
      if (string.IsNullOrEmpty(value) || pairs == null || pairs.Count == 0)
        return pairErrorData;
      int length = value.Length;
      char c1 = value[0];
      for (int index = 1; index < length; ++index)
      {
        char c2 = value[index];
        SymbolPair symbolPair = new SymbolPair(c1, c2);
        if (PairUtilities.FindPair(pairs, symbolPair) >= 0)
        {
          pairErrorData.Error = -1;
          pairErrorData.ErrorIndex = index - 1;
          pairErrorData.ErrorPair = symbolPair;
        }
        c1 = c2;
      }
      return pairErrorData;
    }

    public static CheckedData<SyntaxError> InvalidConstructionRule(string value)
    {
      CheckedData<SyntaxError> checkedData = new CheckedData<SyntaxError>(true, (SyntaxError) null);
      PairErrorData pairErrorData = Syntax.PairsNotAllowedRule(value, Syntax.NotAllowedPairs);
      checkedData.Is = pairErrorData.Error == 0;
      if (pairErrorData.Error < 0)
        checkedData.Value = (SyntaxError) new InvalidConstructionError(value, pairErrorData.ErrorIndex, pairErrorData.ErrorIndex + 1, pairErrorData.ErrorPair.ToString());
      return checkedData;
    }

    public static CheckedData<char> BeginSymbolError(string value, List<char> symbols)
    {
      CheckedData<char> checkedData = new CheckedData<char>(false, ' ');
      if (string.IsNullOrEmpty(value) || symbols == null || symbols.Count == 0)
        return checkedData;
      int index = symbols.IndexOf(value[0]);
      checkedData.Is = index >= 0;
      if (checkedData.Is)
        checkedData.Value = symbols[index];
      return checkedData;
    }

    public static CheckedData<char> EndSymbolError(string value, List<char> symbols)
    {
      CheckedData<char> checkedData = new CheckedData<char>(false, ' ');
      if (string.IsNullOrEmpty(value) || symbols == null || symbols.Count == 0)
        return checkedData;
      int index = symbols.IndexOf(value[value.Length - 1]);
      checkedData.Is = index >= 0;
      if (checkedData.Is)
        checkedData.Value = symbols[index];
      return checkedData;
    }

    public static CheckedData<SyntaxError> ErrorSymbolRule(string value)
    {
      CheckedData<SyntaxError> checkedData1 = new CheckedData<SyntaxError>(true, (SyntaxError) null);
      CheckedData<char> checkedData2 = Syntax.BeginSymbolError(value, Syntax.NotAllowedAtBeginning);
      checkedData1.Is = !checkedData2.Is;
      if (!checkedData1.Is)
      {
        checkedData1.Value = (SyntaxError) new InvalidBeginSymbolError(value, 0, checkedData2.Value);
      }
      else
      {
        CheckedData<char> checkedData3 = Syntax.EndSymbolError(value, Syntax.NotAllowedAtEnd);
        checkedData1.Is = !checkedData3.Is;
        if (!checkedData1.Is)
          checkedData1.Value = (SyntaxError) new InvalidEndSymbolError(value, value.Length - 1, checkedData3.Value);
      }
      return checkedData1;
    }

    static Syntax()
    {
      Syntax.RecreateSyntaxData();
      Syntax.RecreateRules();
    }

    public static bool Check(string value)
    {
      if (Syntax.rules != null && Syntax.rules.Count > 0)
      {
        int count = Syntax.rules.Count;
        for (int index = 0; index < count; ++index)
        {
          CheckedData<SyntaxError> checkedData = Syntax.rules[index].RuleFunction(value);
          if (!checkedData.Is)
            return false;
        }
      }
      return true;
    }
  }
}
