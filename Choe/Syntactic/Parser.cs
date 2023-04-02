using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Choe.Syntactic
{
  public static class Parser
  {
    private static readonly List<LiteralEvaluator> evaluators;
    public static readonly CharacterRuleSet OperatorRules = new CharacterRuleSet();

    static Parser()
    {
      Parser.OperatorRules.SetRule(Choe.Symbols.AddOperator, new CharacterRule(Parser.IsNotExponentSign));
      Parser.OperatorRules.SetRule(Choe.Symbols.SubtractOperator, new CharacterRule(Parser.IsNotExponentSign));
      Parser.OperatorRules.SetRule(Choe.Symbols.SubtractOperator, new CharacterRule(Parser.IsNotPrefixSign));
      Parser.evaluators = new List<LiteralEvaluator>();
      Parser.RegisterLiteralEvaluator(new LiteralIs(Parser.IsBoolean), new LiteralEvaluate(Parser.EvaluateBoolean));
      Parser.RegisterLiteralEvaluator(new LiteralIs(Parser.IsReal), new LiteralEvaluate(Parser.EvaluateReal));
      Parser.RegisterLiteralEvaluator(new LiteralIs(Parser.IsComplex), new LiteralEvaluate(Parser.EvaluateComplex));
    }

    public static bool IsValidName(string value)
    {
      if (string.IsNullOrEmpty(value) || !char.IsLetter(value[0]))
        return false;
      for (int index = 1; index < value.Length; ++index)
      {
        if (!char.IsLetterOrDigit(value[index]) && value[index] != '_' && !((IEnumerable<char>) NRTE.Symbols.SubscriptDigits).Contains<char>(value[index]) && !((IEnumerable<char>) NRTE.Symbols.SuperscriptDigits).Contains<char>(value[index]))
          return false;
      }
      return true;
    }

    public static bool IsExponentSign(string value, int index)
    {
      bool flag = index > 1 && index < value.Length - 1;
      if (flag)
      {
        char ch = value[index - 1];
        flag = ch == 'e' || ch == 'E';
        if (flag)
          flag = char.IsDigit(value[index - 2]) && char.IsDigit(value[index + 1]);
      }
      return flag;
    }

    public static bool IsNotExponentSign(string value, int index) => !Parser.IsExponentSign(value, index);

    public static bool IsPrefixSign(string value, int index)
    {
      bool flag = index == 0;
      if (!flag)
      {
        char c = value[index - 1];
        flag = Parser.IsOpening(c, Elements.Brackets) >= 0;
        if (!flag)
          flag = (int) c == (int) Choe.Symbols.PowerOperator;
      }
      return flag;
    }

    public static bool IsNotPrefixSign(string value, int index) => !Parser.IsPrefixSign(value, index);

    public static int IsOpening(char c, List<SymbolPair> pairs)
    {
      for (int index = 0; index < pairs.Count; ++index)
      {
        if ((int) c == (int) pairs[index].Opening)
          return index;
      }
      return -1;
    }

    public static int IsClosing(char c, List<SymbolPair> pairs)
    {
      for (int index = 0; index < pairs.Count; ++index)
      {
        if ((int) c == (int) pairs[index].Closing)
          return index;
      }
      return -1;
    }

    public static void UpdateLevel(char c, List<SymbolPair> pairs, LevelData lData)
    {
      for (int index = 0; index < pairs.Count; ++index)
      {
        int num = pairs[index].Counter(c);
        if (num != 0)
        {
          lData.Levels[index] += num;
          if (lData.Levels[index] < 0)
          {
            lData.Total = lData.Levels[index];
            break;
          }
          lData.Total += num;
          break;
        }
      }
    }

    public static string GetSubstring(string value, PairIndexes indexes) => value.Substring(indexes.Index1, indexes.Length);

    public static string GetSubstringInside(string value, PairIndexes indexes) => value.Substring(indexes.Index1 + 1, indexes.LengthInside);

    public static CheckedData<PairIndexes> FindPairMultilevel(
      string value,
      int first,
      SymbolPair pair,
      List<SymbolPair> levels)
    {
      PairIndexes v = new PairIndexes(-1, -1);
      bool _is = !string.IsNullOrEmpty(value) && value.Length - first >= 2;
      if (_is)
      {
        int num1 = 0;
        int num2 = 0;
        _is = false;
        for (int index = first; index < value.Length; ++index)
        {
          char c = value[index];
          int num3 = pair.Counter(c);
          if (num3 != 0)
          {
            if (num1 == 0)
            {
              num2 += num3;
              if (num3 > 0)
              {
                if (num2 > 1)
                {
                  num1 += num3;
                  num2 -= num3;
                }
                else
                  v.Index1 = index;
              }
              else if (num3 < 0)
                v.Index2 = index;
              if (num2 == 0)
              {
                _is = true;
                break;
              }
              if (num2 < 0)
              {
                _is = false;
                break;
              }
            }
            else
              num1 += num3;
          }
          else if (Parser.IsOpening(c, levels) >= 0)
            ++num1;
          else if (Parser.IsClosing(c, levels) >= 0)
            --num1;
          if (num1 < 0)
          {
            _is = false;
            break;
          }
        }
      }
      return new CheckedData<PairIndexes>(_is, v);
    }

    public static OperatorList<string, char> SplitMultilevel(
      string value,
      List<char> separators,
      List<SymbolPair> levels,
      bool checkpairs,
      CharacterRuleSet rules)
    {
      if (string.IsNullOrEmpty(value))
        return (OperatorList<string, char>) null;
      PairIndexes indexes = new PairIndexes(-1, -1);
      LevelData lData = new LevelData(levels.Count);
      char minValue = char.MinValue;
      bool flag = false;
      OperatorList<string, char> operatorList = (OperatorList<string, char>) null;
      for (int index = 0; index <= value.Length; ++index)
      {
        if (index == value.Length || separators.Contains(value[index]))
        {
          if ((lData.Total == 0 || !checkpairs && index == value.Length) && (index >= value.Length || rules == null || rules.CheckRules(value[index], value, index)))
          {
            indexes.Index2 = index;
            if (indexes.LengthInside > 0)
            {
              string substringInside = Parser.GetSubstringInside(value, indexes);
              if (operatorList == null)
              {
                operatorList = new OperatorList<string, char>();
                operatorList.Operands = new List<string>();
              }
              operatorList.Operands.Add(substringInside);
              if (flag)
              {
                if (operatorList.Operators == null)
                  operatorList.Operators = new List<char>();
                operatorList.Operators.Add(minValue);
              }
              if (index < value.Length)
              {
                flag = true;
                minValue = value[index];
              }
              indexes.Index1 = indexes.Index2;
            }
            else
            {
              operatorList = (OperatorList<string, char>) null;
              break;
            }
          }
        }
        else
        {
          Parser.UpdateLevel(value[index], levels, lData);
          if (lData.Total < 0 & checkpairs)
          {
            operatorList = (OperatorList<string, char>) null;
            break;
          }
        }
      }
      return operatorList;
    }

    public static OperatorList<string, char> SplitMultilevel(
      string value,
      List<char> separators,
      List<SymbolPair> levels,
      char ssep,
      bool checkpairs,
      CharacterRuleSet rules)
    {
      if (string.IsNullOrEmpty(value))
        return (OperatorList<string, char>) null;
      PairIndexes indexes = new PairIndexes(-1, -1);
      LevelData lData = new LevelData(levels.Count);
      char minValue = char.MinValue;
      bool flag = false;
      int num = 0;
      OperatorList<string, char> operatorList = (OperatorList<string, char>) null;
      for (int index = 0; index <= value.Length; ++index)
      {
        if (index < value.Length && (int) value[index] == (int) ssep)
        {
          if (num > 0)
            --num;
          else
            ++num;
        }
        else if (index == value.Length || separators.Contains(value[index]))
        {
          if ((lData.Total == 0 && num == 0 || !checkpairs && index == value.Length) && (index >= value.Length || rules == null || rules.CheckRules(value[index], value, index)))
          {
            indexes.Index2 = index;
            if (indexes.LengthInside > 0)
            {
              string substringInside = Parser.GetSubstringInside(value, indexes);
              if (operatorList == null)
              {
                operatorList = new OperatorList<string, char>();
                operatorList.Operands = new List<string>();
              }
              operatorList.Operands.Add(substringInside);
              if (flag)
              {
                if (operatorList.Operators == null)
                  operatorList.Operators = new List<char>();
                operatorList.Operators.Add(minValue);
              }
              if (index < value.Length)
              {
                flag = true;
                minValue = value[index];
              }
              indexes.Index1 = indexes.Index2;
            }
            else
            {
              operatorList = (OperatorList<string, char>) null;
              break;
            }
          }
        }
        else
        {
          Parser.UpdateLevel(value[index], levels, lData);
          if (lData.Total < 0 & checkpairs)
          {
            operatorList = (OperatorList<string, char>) null;
            break;
          }
        }
      }
      return operatorList;
    }

    public static OperatorList<string, char> SplitMultilevel(
      string value,
      List<char> separators,
      List<SymbolPair> levels,
      List<char> ssep,
      bool checkpairs,
      CharacterRuleSet rules)
    {
      if (string.IsNullOrEmpty(value))
        return (OperatorList<string, char>) null;
      PairIndexes indexes = new PairIndexes(-1, -1);
      LevelData lData = new LevelData(levels.Count);
      LevelData levelData = new LevelData(ssep.Count);
      char minValue = char.MinValue;
      bool flag = false;
      OperatorList<string, char> operatorList = (OperatorList<string, char>) null;
      for (int index1 = 0; index1 <= value.Length; ++index1)
      {
        if (index1 < value.Length && ssep.Contains(value[index1]))
        {
          int index2 = ssep.IndexOf(value[index1]);
          if (levelData.Levels[index2] > 0)
            --levelData.Levels[index2];
          else
            ++levelData.Levels[index2];
          levelData.Total = 0;
          for (int index3 = 0; index3 < levelData.Levels.Length; ++index3)
            levelData.Total += levelData.Levels[index3];
        }
        else if (index1 == value.Length || separators.Contains(value[index1]))
        {
          if ((lData.Total == 0 && levelData.Total == 0 || !checkpairs && index1 == value.Length) && (index1 >= value.Length || rules == null || rules.CheckRules(value[index1], value, index1)))
          {
            indexes.Index2 = index1;
            if (indexes.LengthInside > 0)
            {
              string substringInside = Parser.GetSubstringInside(value, indexes);
              if (operatorList == null)
              {
                operatorList = new OperatorList<string, char>();
                operatorList.Operands = new List<string>();
              }
              operatorList.Operands.Add(substringInside);
              if (flag)
              {
                if (operatorList.Operators == null)
                  operatorList.Operators = new List<char>();
                operatorList.Operators.Add(minValue);
              }
              if (index1 < value.Length)
              {
                flag = true;
                minValue = value[index1];
              }
              indexes.Index1 = indexes.Index2;
            }
            else
            {
              operatorList = (OperatorList<string, char>) null;
              break;
            }
          }
        }
        else
        {
          Parser.UpdateLevel(value[index1], levels, lData);
          if (lData.Total < 0 & checkpairs)
          {
            operatorList = (OperatorList<string, char>) null;
            break;
          }
        }
      }
      return operatorList;
    }

    public static OperatorList<string, char> SplitMultilevel(
      string value,
      List<char> separators,
      List<SymbolPair> levels,
      bool checkpairs)
    {
      return Parser.SplitMultilevel(value, separators, levels, checkpairs, (CharacterRuleSet) null);
    }

    public static List<string> SplitMultilevel(
      string value,
      char separator,
      List<SymbolPair> levels,
      bool checkpairs)
    {
      if (string.IsNullOrEmpty(value))
        return (List<string>) null;
      PairIndexes indexes = new PairIndexes(-1, -1);
      LevelData lData = new LevelData(levels.Count);
      List<string> stringList = (List<string>) null;
      for (int index = 0; index <= value.Length; ++index)
      {
        if (index == value.Length || (int) value[index] == (int) separator)
        {
          if (lData.Total == 0 || !checkpairs && index == value.Length)
          {
            indexes.Index2 = index;
            if (indexes.LengthInside > 0)
            {
              string substringInside = Parser.GetSubstringInside(value, indexes);
              if (stringList == null)
                stringList = new List<string>();
              stringList.Add(substringInside);
            }
            indexes.Index1 = indexes.Index2;
          }
        }
        else
        {
          Parser.UpdateLevel(value[index], levels, lData);
          if (lData.Total < 0 & checkpairs)
          {
            stringList = (List<string>) null;
            break;
          }
        }
      }
      return stringList;
    }

    public static CheckedData<string> IsSurrounded(
      string value,
      SymbolPair pair,
      List<SymbolPair> levels)
    {
      CheckedData<string> checkedData = new CheckedData<string>(false, (string) null);
      CheckedData<PairIndexes> pairMultilevel = Parser.FindPairMultilevel(value, 0, pair, levels);
      if (pairMultilevel.Is)
      {
        checkedData.Is = pairMultilevel.Value.Index1 == 0 && pairMultilevel.Value.Index2 == value.Length - 1;
        if (checkedData.Is)
          checkedData.Value = Parser.GetSubstringInside(value, pairMultilevel.Value);
      }
      return checkedData;
    }

    public static string RemoveSurrounders(string value, SymbolPair pair, List<SymbolPair> levels)
    {
      CheckedData<string> checkedData;
      while ((checkedData = Parser.IsSurrounded(value, pair, levels)).Is)
        value = checkedData.Value;
      return value;
    }

    public static CheckedData<FunctionData> IsFunction(string value)
    {
      CheckedData<FunctionData> checkedData = new CheckedData<FunctionData>(false, new FunctionData());
      CheckedData<PairIndexes> pairMultilevel1 = Parser.FindPairMultilevel(value, 0, Elements.FunctionBrackets, Elements.Brackets);
      PairIndexes indexes1 = pairMultilevel1.Value;
      checkedData.Is = pairMultilevel1.Is;
      if (checkedData.Is)
      {
        checkedData.Is = indexes1.Index2 == value.Length - 1;
        if (checkedData.Is)
        {
          CheckedData<PairIndexes> pairMultilevel2 = Parser.FindPairMultilevel(value, 0, Elements.ParameterBrackets, Elements.Brackets);
          PairIndexes indexes2 = pairMultilevel2.Value;
          bool flag = pairMultilevel2.Is;
          checkedData.Is = !flag && indexes2.Index1 < 0 && indexes2.Index2 < 0 || flag && indexes2.Index2 + 1 == indexes1.Index1;
          if (checkedData.Is)
          {
            PairIndexes indexes3 = new PairIndexes(0, !flag ? indexes1.Index1 - 1 : indexes2.Index1 - 1);
            checkedData.Is = indexes3.Length > 0;
            if (checkedData.Is)
            {
              checkedData.Value.Name = Parser.GetSubstring(value, indexes3);
              checkedData.Is = !string.IsNullOrWhiteSpace(checkedData.Value.Name);
              if (indexes1.Length > 1)
              {
                string substringInside = Parser.GetSubstringInside(value, indexes1);
                checkedData.Value.Arguments = Parser.SplitMultilevel(substringInside, Choe.Symbols.ArgumentSeparator, Elements.Brackets, false);
              }
              if (flag && indexes2.Length > 1)
              {
                string substringInside = Parser.GetSubstringInside(value, indexes2);
                checkedData.Value.Parameters = Parser.SplitMultilevel(substringInside, Choe.Symbols.ArgumentSeparator, Elements.Brackets, false);
              }
            }
          }
        }
      }
      return checkedData;
    }

    public static CheckedData<ItemData> IsArrayItem(string value)
    {
      CheckedData<ItemData> checkedData = new CheckedData<ItemData>(false, new ItemData());
      CheckedData<PairIndexes> pairMultilevel1 = Parser.FindPairMultilevel(value, 0, Elements.IndexBrackets, Elements.Brackets);
      PairIndexes indexes1 = pairMultilevel1.Value;
      checkedData.Is = pairMultilevel1.Is;
      if (checkedData.Is)
      {
        checkedData.Value.Indexes = new List<string>();
        string substringInside1 = Parser.GetSubstringInside(value, indexes1);
        checkedData.Value.Indexes.Add(substringInside1);
        PairIndexes indexes2 = new PairIndexes(0, indexes1.Index1 - 1);
        checkedData.Is = indexes2.Length > 0;
        if (checkedData.Is)
        {
          checkedData.Value.Name = Parser.GetSubstring(value, indexes2);
          checkedData.Is = !string.IsNullOrWhiteSpace(checkedData.Value.Name);
          PairIndexes indexes3;
          for (int first = indexes1.Index2 + 1; first < value.Length; first = indexes3.Index2 + 1)
          {
            CheckedData<PairIndexes> pairMultilevel2 = Parser.FindPairMultilevel(value, first, Elements.IndexBrackets, Elements.Brackets);
            indexes3 = pairMultilevel2.Value;
            if (pairMultilevel2.Is)
            {
              string substringInside2 = Parser.GetSubstringInside(value, indexes3);
              checkedData.Value.Indexes.Add(substringInside2);
              if (indexes3.Index1 != first)
              {
                checkedData.Is = false;
                break;
              }
            }
            else
            {
              checkedData.Is = false;
              break;
            }
          }
        }
      }
      return checkedData;
    }


    public static CheckedData<double> IsReal(string value, CultureInfo culture)
    {
      CheckedData<double> checkedData = Utilities.TryParseDouble(value, culture);
      if (!checkedData.Is)
      {
        if (value == Choe.Symbols.InfinityName || value == Choe.Symbols.InfinityFullName || value.Length == 1 && (int) value[0] == (int) Choe.Symbols.InfinitySymbol)
        {
          checkedData.Value = double.PositiveInfinity;
          checkedData.Is = true;
        }
        else if (value == Choe.Symbols.EulerSymbol.ToString())
        {
          checkedData.Is = true;
          checkedData.Value = Math.E;
        }
        else if (value == Choe.Symbols.PiSymbol.ToString() || value == Choe.Symbols.PiName)
        {
          checkedData.Is = true;
          checkedData.Value = Math.PI;
        }
      }
      return checkedData;
    }

    public static CheckedData<double> IsReal(string value, bool eval) => Parser.IsReal(value, CultureInfo.CurrentCulture);

    public static bool IsReal(string value) => Parser.IsReal(value, CultureInfo.CurrentCulture).Is;

    public static CheckedData<string> IsRealConstant(double x)
    {
      string v = (string) null;
      bool _is = false;
      if (Math.Abs(x - Math.PI) <= Choe.Constants.RealConstantPrecision)
      {
        v = Choe.Symbols.PiString;
        _is = true;
      }
      else if (Math.Abs(x - Math.E) <= Choe.Constants.RealConstantPrecision)
      {
        v = Choe.Symbols.EulerString;
        _is = true;
      }
      return new CheckedData<string>(_is, v);
    }

    public static ValueData EvaluateReal(string value)
    {
      CheckedData<double> checkedData = Parser.IsReal(value, true);
      return checkedData.Is ? new ValueData(Types.RealType, (object) checkedData.Value) : (ValueData) null;
    }

    public static string RealToString(double x) => x.ToString((IFormatProvider) CultureInfo.CurrentCulture);

    public static CheckedData<Complex> IsComplex(string value, CultureInfo culture)
    {
      Complex v = Complex.Zero;
      bool _is = !string.IsNullOrEmpty(value) && value.Length >= 1;
      if (_is)
      {
        if (value.Length == 1)
        {
          _is = (int) value[0] == (int) Choe.Symbols.ImaginarySymbol;
          if (_is)
            v = Complex.ImaginaryOne;
        }
        else
        {
          char c = value[value.Length - 2];
          _is = (int) value[value.Length - 1] == (int) Choe.Symbols.ImaginarySymbol && (char.IsDigit(c) || (int) c == (int) Choe.Symbols.AddOperator || (int) c == (int) Choe.Symbols.SubtractOperator);
          if (_is)
          {
            int num = value.LastIndexOf(Choe.Symbols.AddOperator);
            if (num < 0)
              num = value.LastIndexOf(Choe.Symbols.SubtractOperator);
            if (num > 0 && Parser.IsExponentSign(value, num))
            {
              int count = num;
              num = value.LastIndexOf(Choe.Symbols.AddOperator, count - 1, count);
              if (num < 0)
                num = value.LastIndexOf(Choe.Symbols.SubtractOperator, count - 1, count);
            }
            string str1 = string.Empty;
            string empty = string.Empty;
            string str2;
            if (num > 0)
            {
              str1 = value.Substring(0, num);
              str2 = value.Substring(num, value.Length - num - 1);
            }
            else
              str2 = value.Substring(0, value.Length - 1);
            if (str2 == Choe.Symbols.AddOperator.ToString() || str2 == Choe.Symbols.SubtractOperator.ToString())
              str2 += Choe.Symbols.UnitSymbol.ToString();
            double real = 0.0;
            CheckedData<double> checkedData1 = Parser.IsReal(str2, culture);
            _is = checkedData1.Is;
            if (_is)
            {
              double imaginary = checkedData1.Value;
              if (!string.IsNullOrEmpty(str1))
              {
                CheckedData<double> checkedData2 = Parser.IsReal(str1, culture);
                _is = checkedData2.Is;
                real = checkedData2.Value;
              }
              if (_is)
                v = new Complex(real, imaginary);
            }
          }
        }
      }
      return new CheckedData<Complex>(_is, v);
    }

    public static CheckedData<Complex> IsComplex(string value, bool eval) => Parser.IsComplex(value, CultureInfo.CurrentCulture);

    public static bool IsComplex(string value) => Parser.IsComplex(value, false).Is;

    public static CheckedData<string> IsComplexConstant(Complex z)
    {
      string v = (string) null;
      bool _is = false;
      if (z.Magnitude == 0.0)
      {
        v = Choe.Symbols.ZeroString + Choe.Symbols.ImaginarySymbol.ToString();
        _is = true;
      }
      else
      {
        double real = z.Real;
        double imaginary = z.Imaginary;
        if (real != 0.0 && Math.Abs(imaginary / real) <= Choe.Constants.RealConstantPrecision)
        {
          imaginary = 0.0;
          _is = true;
        }
        else if (imaginary != 0.0 && Math.Abs(real / imaginary) <= Choe.Constants.RealConstantPrecision)
        {
          real = 0.0;
          _is = true;
        }
        if (_is)
          v = Parser.ComplexToString(new Complex(real, imaginary));
      }
      return new CheckedData<string>(_is, v);
    }

    public static ValueData EvaluateComplex(string value)
    {
      CheckedData<Complex> checkedData = Parser.IsComplex(value, true);
      return checkedData.Is ? new ValueData(Types.ComplexType, (object) checkedData.Value) : (ValueData) null;
    }

    public static string ComplexToString(Complex x)
    {
      if (double.IsNaN(x.Real) && double.IsNaN(x.Imaginary))
        return Choe.Symbols.NaNName + Choe.Symbols.ImaginaryString;
      string empty = string.Empty;
      string str1 = string.Empty;
      double num;
      if (x.Real != 0.0)
      {
        num = x.Real;
        empty = num.ToString();
      }
      if (Math.Abs(x.Imaginary) != 1.0)
      {
        num = x.Imaginary;
        str1 = num.ToString();
      }
      if (!string.IsNullOrEmpty(empty) && x.Imaginary >= 0.0)
        str1 = Choe.Symbols.AddOperator.ToString() + str1;
      else if (x.Imaginary < 0.0 && string.IsNullOrEmpty(str1))
        str1 = Choe.Symbols.SubtractOperator.ToString();
      string str2 = str1 + Choe.Symbols.ImaginarySymbol.ToString();
      return empty + str2;
    }

    public static CheckedData<bool> IsBoolean(string value, CultureInfo culture)
    {
      CheckedData<bool> checkedData = Utilities.TryParseBool(value);
      if (!checkedData.Is)
      {
        if (string.Compare(value, Choe.Symbols.TrueName, true) == 0)
        {
          checkedData.Is = true;
          checkedData.Value = true;
        }
        else if (string.Compare(value, Choe.Symbols.FalseName, true) == 0)
        {
          checkedData.Is = true;
          checkedData.Value = false;
        }
        else
        {
          bool flag = true;
          string strB1 = flag.ToString((IFormatProvider) culture);
          if (string.Compare(value, strB1, true) == 0)
          {
            checkedData.Is = true;
            checkedData.Value = true;
          }
          else
          {
            flag = false;
            string strB2 = flag.ToString((IFormatProvider) culture);
            if (string.Compare(value, strB2, true) == 0)
            {
              checkedData.Is = true;
              checkedData.Value = false;
            }
          }
        }
      }
      return checkedData;
    }

    public static CheckedData<bool> IsBoolean(string value, bool eval) => Parser.IsBoolean(value, CultureInfo.CurrentCulture);

    public static bool IsBoolean(string value) => Parser.IsBoolean(value, false).Is;

    public static ValueData EvaluateBoolean(string value)
    {
      ValueData boolean = (ValueData) null;
      CheckedData<bool> checkedData = Parser.IsBoolean(value, true);
      if (checkedData.Is)
        boolean = new ValueData(Types.BooleanType, (object) checkedData.Value);
      return boolean;
    }

    public static string BooleanToString(bool x) => x ? Choe.Symbols.TrueName : Choe.Symbols.FalseName;

    public static void RegisterLiteralEvaluator(LiteralIs li, LiteralEvaluate le)
    {
      LiteralEvaluator literalEvaluator = new LiteralEvaluator(li, le);
      Parser.evaluators.Add(literalEvaluator);
    }

    public static bool IsLiteral(string literal)
    {
      int count = Parser.evaluators.Count;
      for (int index = 0; index < count; ++index)
      {
        if (Parser.evaluators[index].Is(literal))
          return true;
      }
      return false;
    }

    public static ValueData EvaluateLiteral(string literal)
    {
      int count = Parser.evaluators.Count;
      for (int index = 0; index < count; ++index)
      {
        if (Parser.evaluators[index].Is(literal))
          return Parser.evaluators[index].Evaluate(literal);
      }
      return (ValueData) null;
    }

    private static CheckedData<StringOperators> IsBinaryExpression(string value, List<char> signs)
    {
      CheckedData<StringOperators> checkedData = new CheckedData<StringOperators>(false, (StringOperators) null);
      checkedData.Is = !string.IsNullOrEmpty(value);
      if (checkedData.Is)
      {
        OperatorList<string, char> operatorList = Parser.SplitMultilevel(value, signs, Elements.Brackets, Elements.OutfixOperators, true, Parser.OperatorRules);
        checkedData.Is = operatorList != null && operatorList.Operands != null && operatorList.Operands.Count > 1;
        if (checkedData.Is)
        {
          checkedData.Value = new StringOperators();
          checkedData.Value.Operands = operatorList.Operands;
          checkedData.Value.Operators = new List<string>();
          for (int index = 0; index < operatorList.Operators.Count; ++index)
            checkedData.Value.Operators.Add(operatorList.Operators[index].ToString());
        }
      }
      return checkedData;
    }

    public static CheckedData<StringOperators> IsRelationalExpression(string value) => Parser.IsBinaryExpression(value, Elements.RelationalOperators);

    public static CheckedData<StringOperators> IsSummExpression(string value) => Parser.IsBinaryExpression(value, Elements.SummOperators);

    public static CheckedData<StringOperators> IsProductExpression(string value) => Parser.IsBinaryExpression(value, Elements.ProductOperators);

    public static CheckedData<StringOperators> IsPowerExpression(string value) => Parser.IsBinaryExpression(value, Elements.PowerOperators);


    public static CheckedData<OperatorData> IsPrefixExpression(string value)
    {
      CheckedData<OperatorData> checkedData = new CheckedData<OperatorData>(false, new OperatorData());
      checkedData.Is = !string.IsNullOrEmpty(value) && value.Length > 1;
      if (checkedData.Is)
      {
        checkedData.Is = Elements.PrefixOperators.Contains(value[0]);
        if (checkedData.Is)
        {
          checkedData.Value.Operator = value[0].ToString();
          checkedData.Value.Operand = value.Substring(1, value.Length - 1);
        }
      }
      return checkedData;
    }

    public static CheckedData<OperatorData> IsPostfixExpression(string value)
    {
      CheckedData<OperatorData> checkedData = new CheckedData<OperatorData>(false, new OperatorData());
      checkedData.Is = !string.IsNullOrEmpty(value) && value.Length > 1;
      if (checkedData.Is)
      {
        int num = value.Length - 1;
        checkedData.Is = Elements.PostfixOperators.Contains(value[num]);
        if (checkedData.Is)
        {
          checkedData.Value.Operator = value[num].ToString();
          checkedData.Value.Operand = value.Substring(0, num);
        }
      }
      return checkedData;
    }

    public static CheckedData<OperatorData> IsOutfixExpression(string value)
    {
      CheckedData<OperatorData> checkedData = new CheckedData<OperatorData>(false, new OperatorData());
      checkedData.Is = !string.IsNullOrEmpty(value) && value.Length > 2;
      if (checkedData.Is)
      {
        checkedData.Is = (int) value[0] == (int) value[value.Length - 1] && Elements.OutfixOperators.Contains(value[0]);
        if (checkedData.Is)
        {
          checkedData.Value.Operand = value.Substring(1, value.Length - 2);
          checkedData.Value.Operator = value[0].ToString() + value[value.Length - 1].ToString();
        }
      }
      return checkedData;
    }

    public static CheckedData<VectorData> IsVectorExpression(string value)
    {
      CheckedData<VectorData> checkedData = new CheckedData<VectorData>(false, new VectorData());
      checkedData.Is = !string.IsNullOrEmpty(value) && value.Length > 2;
      if (checkedData.Is)
      {
        CheckedData<PairIndexes> pairMultilevel = Parser.FindPairMultilevel(value, 0, Elements.VectorBrackets, Elements.Brackets);
        PairIndexes indexes = pairMultilevel.Value;
        checkedData.Is = pairMultilevel.Is;
        if (checkedData.Is)
          checkedData.Is = indexes.Index1 == 0 && indexes.Index2 == value.Length - 1;
        if (checkedData.Is)
        {
          List<string> stringList = Parser.SplitMultilevel(Parser.GetSubstringInside(value, indexes), Choe.Symbols.VectorItemSeparator, Elements.Brackets, false);
          checkedData.Is = stringList != null && stringList.Count > 0;
          if (checkedData.Is)
            checkedData.Value.Items = stringList.ToArray();
        }
      }
      return checkedData;
    }

    public static CheckedData<MatrixData> IsMatrixExpression(string value)
    {
      CheckedData<MatrixData> checkedData1 = new CheckedData<MatrixData>(false, new MatrixData());
      checkedData1.Is = !string.IsNullOrEmpty(value) && value.Length > 4;
      if (checkedData1.Is)
      {
        CheckedData<VectorData> checkedData2 = Parser.IsVectorExpression(value);
        checkedData1.Is = checkedData2.Is;
        if (checkedData1.Is)
        {
          string[] items = checkedData2.Value.Items;
          int length = items.Length;
          checkedData1.Value.Items = new string[length][];
          for (int index = 0; index < length; ++index)
          {
            CheckedData<VectorData> checkedData3 = Parser.IsVectorExpression(items[index]);
            checkedData1.Is = checkedData3.Is;
            checkedData1.Value.Items[index] = checkedData3.Value.Items;
            if (!checkedData1.Is)
              break;
          }
        }
      }
      return checkedData1;
    }

    public static CheckedData<LimitData> IsIntegralLimits(string value)
    {
      CheckedData<LimitData> checkedData = new CheckedData<LimitData>(false, new LimitData());
      if (string.IsNullOrEmpty(value))
        return checkedData;
      int length1 = value.Length;
      if ((int) value[length1 - 1] == (int) Choe.Symbols.ParameterRightBracket)
      {
        int length2 = value.IndexOf(Choe.Symbols.ParameterLeftBracket);
        checkedData.Is = length2 > 0;
        if (checkedData.Is)
        {
          checkedData.Value.Variable = value.Substring(0, length2);
          checkedData.Is = Parser.IsValidName(checkedData.Value.Variable);
          if (checkedData.Is)
          {
            string str = value.Substring(length2 + 1, length1 - length2 - 2);
            checkedData.Is = str.Length > 2;
            if (checkedData.Is)
            {
              List<string> stringList = Parser.SplitMultilevel(str, Choe.Symbols.ArgumentSeparator, Elements.Brackets, false);
              checkedData.Is = stringList != null && stringList.Count == 2 && !string.IsNullOrEmpty(stringList[0]) && !string.IsNullOrEmpty(stringList[1]);
              if (checkedData.Is)
              {
                checkedData.Value.Limit1 = stringList[0];
                checkedData.Value.Limit2 = stringList[1];
              }
            }
          }
        }
      }
      else
      {
        checkedData.Is = Parser.IsValidName(value);
        if (checkedData.Is)
          checkedData.Value.Variable = value;
      }
      return checkedData;
    }

    public static CheckedData<IntegralData> IsIntegralExpression(string value)
    {
      CheckedData<IntegralData> checkedData1 = new CheckedData<IntegralData>(false, new IntegralData());
      checkedData1.Value.Limits = new LimitData();
      checkedData1.Is = !string.IsNullOrEmpty(value) && value.Length > 2 && (int) value[0] == (int) Choe.Symbols.IntegralOperator;
      if (checkedData1.Is)
      {
        string str = value.Substring(1, value.Length - 1);
        if ((int) str[0] == (int) Choe.Symbols.DerivativeOperator)
        {
          CheckedData<LimitData> checkedData2 = Parser.IsIntegralLimits(str.Substring(1, str.Length - 1));
          checkedData1.Value.Limits = checkedData2.Value;
          checkedData1.Is = checkedData2.Is;
        }
        else
        {
          List<string> stringList = Parser.SplitMultilevel(str, Choe.Symbols.DerivativeOperator, Elements.Brackets, false);
          checkedData1.Is = stringList != null && stringList.Count == 2 && !string.IsNullOrEmpty(stringList[0]);
          if (checkedData1.Is)
          {
            checkedData1.Value.Operand = stringList[0];
            CheckedData<LimitData> checkedData3 = Parser.IsIntegralLimits(stringList[1]);
            checkedData1.Value.Limits = checkedData3.Value;
            checkedData1.Is = checkedData3.Is;
          }
        }
      }
      return checkedData1;
    }
  }
}
