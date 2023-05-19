using System;
using System.Collections.Generic;

namespace Choe.Syntactic
{
  public static class Builder
  {
    public static string Surround(string value, SymbolPair bounds, bool empty)
    {
      string str1;
      if (bounds != (SymbolPair) null && !string.IsNullOrEmpty(value) | empty)
      {
        char ch = bounds.Opening;
        string str2 = ch.ToString();
        string str3 = value;
        ch = bounds.Closing;
        string str4 = ch.ToString();
        str1 = str2 + str3 + str4;
      }
      else
        str1 = value;
      return str1;
    }

    public static string MakeList(List<string> values, string separator)
    {
      string str = string.Empty;
      if (values != null && values.Count > 0)
      {
        str = values[0];
        for (int index = 1; index < values.Count; ++index)
          str = str + separator + values[index];
      }
      return str;
    }

    public static string MakeList(string value, string separator, int count, bool addNumber)
    {
      string str = string.Empty;
      if (count > 0)
      {
        str = value;
        if (addNumber)
          str += 0.ToString();
        for (int index = 1; index < count; ++index)
        {
          string empty = string.Empty;
          if (addNumber)
            empty = index.ToString();
          str = str + separator + value + empty;
        }
      }
      return str;
    }

    public static List<string> MakeList(string value, int count, bool addNumber)
    {
      List<string> stringList = new List<string>();
      if (count > 0)
      {
        for (int index = 0; index < count; ++index)
        {
          string empty = string.Empty;
          if (addNumber)
            empty = index.ToString();
          stringList.Add(value + empty);
        }
      }
      return stringList;
    }

    public static string MakeList(
      List<string> values,
      string separator,
      SymbolPair bounds,
      bool empty)
    {
      return Builder.Surround(Builder.MakeList(values, separator), bounds, empty);
    }

    public static string MakeMultilist(List<string> values, SymbolPair bounds)
    {
      string empty = string.Empty;
      if (values != null && values.Count > 0)
      {
        for (int index = 0; index < values.Count; ++index)
          empty += Builder.Surround(values[index], bounds, true);
      }
      return empty;
    }

    public static string BuildArrayIndexes(List<string> indexes) => Builder.MakeMultilist(indexes, Elements.IndexBrackets);

    public static string BuildFunctionParameters(List<string> parameters) => Builder.MakeList(parameters, Choe.Symbols.ArgumentSeparator.ToString(), Elements.ParameterBrackets, false);

    public static string BuildFunctionArguments(List<string> arguments) => Builder.MakeList(arguments, Choe.Symbols.ArgumentSeparator.ToString(), Elements.FunctionBrackets, true);

    public static string BuildArrayItem(string name, List<string> indexes) => name + Builder.BuildArrayIndexes(indexes);

    public static string BuildFunction(
      string name,
      List<string> parameters,
      List<string> arguments)
    {
      return name + Builder.BuildFunctionParameters(parameters) + Builder.BuildFunctionArguments(arguments);
    }

    public static string BuildUnaryOperation(
      string op,
      string operand,
      OperatorPosition position,
      bool enclose = false)
    {
      string str1 = (string) null;
      if (enclose)
        operand = Builder.Surround(operand, Elements.FunctionBrackets, true);
      string str2;
      string str3;
      switch (position)
      {
        case OperatorPosition.Prefix:
          str2 = op;
          str3 = operand;
          break;
        case OperatorPosition.Postfix:
          str2 = operand;
          str3 = op;
          break;
        case OperatorPosition.Outfix:
          str2 = op[0].ToString();
          str3 = operand;
          str1 = op[1].ToString();
          break;
        default:
          throw new NotImplementedException("Build unary operator: position = " + position.ToString());
      }
      return str2 + str3 + str1;
    }

    public static string BuildBinaryOperations(
      List<string> operands,
      List<string> operators,
      List<bool> enclose = null)
    {
      string str1 = (string) null;
      if (operands != null && operands.Count > 0)
      {
        bool flag = enclose != null;
        str1 = !flag || !enclose[0] ? operands[0] : Builder.Surround(operands[0], Elements.FunctionBrackets, true);
        for (int index = 1; index < operands.Count; ++index)
        {
          string str2 = !flag || !enclose[index] ? operands[index] : Builder.Surround(operands[index], Elements.FunctionBrackets, true);
          str1 = str1 + operators[index - 1] + str2;
        }
      }
      return str1;
    }

    public static string BuildVector(string[] items)
    {
      int length = items.Length;
      string empty = string.Empty;
      for (int index = 0; index < length; ++index)
      {
        empty += items[index];
        if (index < length - 1)
          empty += Choe.Symbols.VectorItemSeparator.ToString();
      }
      return Builder.Surround(empty, Elements.VectorBrackets, true);
    }

    public static string BuildMatrix(string[] items, int m, int n)
    {
      string empty = string.Empty;
      string[] items1 = new string[m];
      string[] items2 = new string[n];
      for (int index1 = 0; index1 < m; ++index1)
      {
        int num = index1 * n;
        for (int index2 = 0; index2 < n; ++index2)
          items2[index2] = items[num + index2];
        items1[index1] = Builder.BuildVector(items2);
      }
      return Builder.BuildVector(items1);
    }

    public static string BuildDerivative(
      string operand,
      bool enclose,
      string[] vars,
      int[] orders)
    {
      int index1 = 0;
      int length = vars.Length;
      string str1 = string.Empty;
      for (int index2 = 0; index2 < length; ++index2)
      {
        int order = orders[index2];
        string str2 = order != 1 ? NRTE.Symbols.SuperscriptDigits[order].ToString() ?? "" : string.Empty;
        str1 = str1 + Choe.Symbols.DerivativeOperator.ToString() + vars[index2] + str2;
        index1 += orders[index2];
      }
      string str3 = operand;
      if (enclose)
        str3 = Builder.Surround(str3, Elements.FunctionBrackets, true);
      string str4 = index1 != 1 ? NRTE.Symbols.SuperscriptDigits[index1].ToString() ?? "" : string.Empty;
      return Choe.Symbols.DerivativeOperator.ToString() + str4 + str3 + Choe.Symbols.DerivativeSeparator.ToString() + str1;
    }

    public static string BuildIntegral(
      string operand,
      string ivar,
      string lim1,
      string lim2,
      bool enclose)
    {
      string str1 = operand;
      if (enclose)
        str1 = Builder.Surround(str1, Elements.FunctionBrackets, true);
      string str2 = Choe.Symbols.IntegralOperator.ToString() + str1 + Choe.Symbols.DerivativeOperator.ToString() + ivar;
      if (!string.IsNullOrEmpty(lim1) || !string.IsNullOrEmpty(lim2))
        str2 = str2 + Choe.Symbols.ParameterLeftBracket.ToString() + lim1 + Choe.Symbols.ArgumentSeparator.ToString() + lim2 + Choe.Symbols.ParameterRightBracket.ToString();
      return str2;
    }
  }
}
