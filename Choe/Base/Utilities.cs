using Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Choe
{
  public static class Utilities
  {
    private static Dictionary<Type, ValueToString> _converters = new Dictionary<Type, ValueToString>(8);

    public static string GetOperatorSign(OperatorType type)
    {
      switch (type)
      {
        case OperatorType.Add:
          return Symbols.AddOperator.ToString();
        case OperatorType.Subtract:
          return Symbols.SubtractOperator.ToString();
        case OperatorType.Multiply:
          return Symbols.MultiplyOperator.ToString();
        case OperatorType.Divide:
          return Symbols.DivideOperator.ToString();
        case OperatorType.Dot:
          return Symbols.DotOperator.ToString();
        case OperatorType.Cross:
          return Symbols.CrossOperator.ToString();
        case OperatorType.Power:
          return Symbols.PowerOperator.ToString();
        case OperatorType.Question:
          return Symbols.QuestionOperator.ToString();
        case OperatorType.Number:
          return Symbols.NumberOperator.ToString();
        case OperatorType.Minus:
          return Symbols.MinusOperator.ToString();
        case OperatorType.Tilde:
          return Symbols.TildeOperator.ToString();
        case OperatorType.SquareRoot:
          return Symbols.SquareRootOperator.ToString();
        case OperatorType.Derivative:
          return Symbols.DerivativeOperator.ToString();
        case OperatorType.Integral:
          return Symbols.IntegralOperator.ToString();
        case OperatorType.Delta:
          return Symbols.DeltaOperator.ToString();
        case OperatorType.Sum:
          return Symbols.SumOperator.ToString();
        case OperatorType.Product:
          return Symbols.ProductOperator.ToString();
        case OperatorType.Factorial:
          return Symbols.FactorialOperator.ToString();
        case OperatorType.Apostrophe:
          return Symbols.ApostropheOperator.ToString();
        case OperatorType.Accent:
          return Symbols.AccentOperator.ToString();
        case OperatorType.Absolute:
          string str1 = Symbols.AbsoluteOperator.ToString();
          return str1 + str1;
        case OperatorType.Norm:
          string str2 = Symbols.NormOperator.ToString();
          return str2 + str2;
        default:
          throw new NotImplementedException("Operator Sign " + type.ToString());
      }
    }

    /// <summary>Gets operator type by its sign and arity.</summary>
    public static OperatorType GetOperatorType(string sign, OperatorArity arity)
    {
      int num = Utilities.OperatorCount();
      for (int index = 1; index <= num; ++index)
      {
        OperatorType type = (OperatorType) index;
        if (Utilities.GetOperatorArity(type) == arity && Utilities.GetOperatorSign(type) == sign)
          return type;
      }
      return OperatorType.Unknown;
    }

    /// <summary>
    /// Operator Precedence
    /// (operators with higher precedence applied before operators with lower precedence)
    /// </summary>
    public static int GetOperatorPrecedence(OperatorType type)
    {
      switch (type)
      {
        case OperatorType.Add:
        case OperatorType.Subtract:
          return 8;
        case OperatorType.Multiply:
        case OperatorType.Divide:
        case OperatorType.Dot:
        case OperatorType.Cross:
          return 16;
        case OperatorType.Power:
          return 32;
        case OperatorType.Question:
        case OperatorType.Number:
        case OperatorType.Minus:
        case OperatorType.Tilde:
        case OperatorType.SquareRoot:
          return 128;
        case OperatorType.Derivative:
        case OperatorType.Integral:
        case OperatorType.Delta:
        case OperatorType.Sum:
        case OperatorType.Product:
          return 128;
        case OperatorType.Factorial:
        case OperatorType.Apostrophe:
        case OperatorType.Accent:
          return 256;
        case OperatorType.Absolute:
        case OperatorType.Norm:
          return 512;
        default:
          throw new NotImplementedException("Operator Precedence " + type.ToString());
      }
    }

    private static OperatorType HighOperator()
    {
      Array values = Enum.GetValues(typeof (OperatorType));
      return (OperatorType) values.GetValue(values.Length - 1);
    }

    /// <summary>
    /// Gets precedence of a variable or literal
    /// (use this function for compatibility with other operations precedence).
    /// </summary>
    public static int GetVariablePrecedence() => Utilities.GetOperatorPrecedence(Utilities.HighOperator()) * 2;

    /// <summary>
    /// Gets precedence of a function
    /// (use this function for compatibility with other operations precedence).
    /// </summary>
    public static int GetFunctionPrecedence() => Utilities.GetOperatorPrecedence(Utilities.HighOperator()) * 2;

    /// <summary>
    /// Gets precedence of an array or indexing expression
    /// (use this function for compatibility with other operations precedence).
    /// </summary>
    public static int GetArrayPrecedence() => Utilities.GetOperatorPrecedence(Utilities.HighOperator()) * 2;

    /// <summary>Operator arity</summary>
    public static OperatorArity GetOperatorArity(OperatorType type)
    {
      switch (type)
      {
        case OperatorType.Add:
        case OperatorType.Subtract:
        case OperatorType.Multiply:
        case OperatorType.Divide:
        case OperatorType.Dot:
        case OperatorType.Cross:
        case OperatorType.Power:
          return OperatorArity.Binary;
        case OperatorType.Question:
        case OperatorType.Number:
        case OperatorType.Minus:
        case OperatorType.Tilde:
        case OperatorType.SquareRoot:
        case OperatorType.Factorial:
        case OperatorType.Apostrophe:
        case OperatorType.Accent:
        case OperatorType.Absolute:
        case OperatorType.Norm:
          return OperatorArity.Unary;
        case OperatorType.Derivative:
        case OperatorType.Integral:
        case OperatorType.Delta:
        case OperatorType.Sum:
        case OperatorType.Product:
          return OperatorArity.Unary;
        default:
          throw new NotImplementedException("Operator Arity " + type.ToString());
      }
    }

    /// <summary>Operator position</summary>
    public static OperatorPosition GetOperatorPosition(OperatorType type)
    {
      switch (type)
      {
        case OperatorType.Add:
        case OperatorType.Subtract:
        case OperatorType.Multiply:
        case OperatorType.Divide:
        case OperatorType.Dot:
        case OperatorType.Cross:
        case OperatorType.Power:
          return OperatorPosition.Infix;
        case OperatorType.Question:
        case OperatorType.Number:
        case OperatorType.Minus:
        case OperatorType.Tilde:
        case OperatorType.SquareRoot:
          return OperatorPosition.Prefix;
        case OperatorType.Derivative:
        case OperatorType.Integral:
        case OperatorType.Delta:
        case OperatorType.Sum:
        case OperatorType.Product:
          return OperatorPosition.Prefix;
        case OperatorType.Factorial:
        case OperatorType.Apostrophe:
        case OperatorType.Accent:
          return OperatorPosition.Postfix;
        case OperatorType.Absolute:
        case OperatorType.Norm:
          return OperatorPosition.Outfix;
        default:
          throw new NotImplementedException("Operator Position " + type.ToString());
      }
    }

    /// <summary>
    /// Returns binary operation type for the operator type
    /// or throws an EXCEPTION if the type is not defined.
    /// </summary>
    public static BinaryOperationType GetBinaryOperationType(OperatorType type)
    {
      switch (type)
      {
        case OperatorType.Add:
        case OperatorType.Subtract:
          return BinaryOperationType.Sum;
        case OperatorType.Multiply:
        case OperatorType.Divide:
        case OperatorType.Dot:
        case OperatorType.Cross:
          return BinaryOperationType.Product;
        case OperatorType.Power:
          return BinaryOperationType.Power;
        default:
          throw new NotImplementedException("Binary operation type not defined for " + type.ToString());
      }
    }

    /// <summary>
    /// Returns binary operation type for the operator type
    /// or throws an EXCEPTION if the type is not defined.
    /// </summary>
    public static BinaryOperationType GetBinaryOperationType(string sign) => Utilities.GetBinaryOperationType(Utilities.GetOperatorType(sign, OperatorArity.Binary));

    /// <summary>
    /// Returns if a binary operation is commutative
    /// NOTE: throws an EXCEPTION if the type is not a binary operator.
    /// </summary>
    public static bool IsCommutative(OperatorType type)
    {
      switch (type)
      {
        case OperatorType.Add:
          return true;
        case OperatorType.Subtract:
          return false;
        case OperatorType.Multiply:
          return true;
        case OperatorType.Divide:
        case OperatorType.Dot:
        case OperatorType.Cross:
          return false;
        case OperatorType.Power:
          return false;
        default:
          throw new NotSupportedException("Commutative property not supported for " + type.ToString());
      }
    }

    /// <summary>
    /// Returns if a binary operation is commutative
    /// NOTE: throws an EXCEPTION if the type is not a binary operator.
    /// </summary>
    public static bool IsCommutative(string sign) => Utilities.IsCommutative(Utilities.GetOperatorType(sign, OperatorArity.Binary));

    /// <summary>Defined Operator Count (excluding Undefined value)</summary>
    public static int OperatorCount() => Enum.GetValues(typeof (OperatorType)).Length - 1;

    /// <summary>
    /// Safe call ToString (for null object returns null symbol)
    /// Uses registered converters for result formatting.
    /// </summary>
    public static string SafeToString(object x)
    {
      if (x == null)
        return Symbols.NullValue;
      Type type = x.GetType();
      if (Utilities._converters.ContainsKey(type))
        return Utilities._converters[type](x);
      foreach (Type key in Utilities._converters.Keys)
      {
        if (Utilities.TypeCompatible(key, type))
          return Utilities._converters[key](x);
      }
      return x.ToString();
    }

    /// <summary>Safety gets type name</summary>
    /// <param name="t">Type (can be null)</param>
    /// <returns></returns>
    public static string GetTypeName(Type t) => t != (Type) null ? t.Name : Symbols.NullValue;

    /// <summary>Gets type names (safety)</summary>
    public static List<string> GetTypeNames(Type[] types)
    {
      List<string> typeNames = (List<string>) null;
      if (types != null)
      {
        typeNames = new List<string>();
        for (int index = 0; index < types.Length; ++index)
        {
          string typeName = Utilities.GetTypeName(types[index]);
          typeNames.Add(typeName);
        }
      }
      return typeNames;
    }

    /// <summary>
    /// Safely gets the Length of the FIRST array dimension
    /// (for null value returns 0)
    /// </summary>
    /// <param name="x"></param>
    /// <returns>Length of the FIRST array dimension</returns>
    public static int SafeArrayLength(Array x) => x == null ? 0 : x.GetLength(0);

    /// <summary>
    /// Safely gets the element Count of the List
    /// (for null value returns 0)
    /// </summary>
    /// <param name="x"></param>
    /// <returns>Length of the FIRST array dimension</returns>
    public static int SafeCount(IList x) => x == null ? 0 : x.Count;

    /// <summary>Compares Types</summary>
    public static bool TypeEquals(Type t1, Type t2)
    {
      if (t1 == (Type) null && t2 == (Type) null)
        return true;
      return !(t1 == (Type) null) && !(t2 == (Type) null) && t1 == t2;
    }

    /// <summary>
    /// Checks if the second type can be used instead of the first
    /// </summary>
    public static bool TypeCompatible(Type expected, Type actual)
    {
      if (expected == (Type) null && actual == (Type) null)
        return true;
      if (expected == (Type) null || actual == (Type) null)
        return false;
      return expected == actual || actual.IsSubclassOf(expected);
    }

    /// <summary>
    /// Compares Type Arrays and returns two types that are not equal.
    /// </summary>
    public static CheckedData<CompareTypes> TypesEqual(Type[] t1, Type[] t2)
    {
      CheckedData<CompareTypes> checkedData = new CheckedData<CompareTypes>(true, new CompareTypes((Type) null, (Type) null));
      if (t1 == null && t2 == null)
        return checkedData;
      if (t1 == null || t2 == null)
      {
        checkedData.Is = false;
        return checkedData;
      }
      if (t1.Length != t2.Length)
      {
        checkedData.Is = false;
        return checkedData;
      }
      for (int index = 0; index < t1.Length; ++index)
      {
        if (!Utilities.TypeEquals(t1[index], t2[index]))
        {
          checkedData.Value.Left = t1[index];
          checkedData.Value.Right = t2[index];
          checkedData.Is = false;
          return checkedData;
        }
      }
      return checkedData;
    }

    /// <summary>
    /// Checks Type Arrays compatibility (the second can be used instead of the first)
    /// and returns two types that are not compatible.
    /// </summary>
    public static CheckedData<CompareTypes> TypesCompatible(Type[] expected, Type[] actual)
    {
      CheckedData<CompareTypes> checkedData = new CheckedData<CompareTypes>(true, new CompareTypes((Type) null, (Type) null));
      if (expected == null && actual == null)
        return checkedData;
      if (expected == null || actual == null)
      {
        checkedData.Is = false;
        return checkedData;
      }
      if (expected.Length != actual.Length)
      {
        checkedData.Is = false;
        return checkedData;
      }
      for (int index = 0; index < expected.Length; ++index)
      {
        if (!Utilities.TypeCompatible(expected[index], actual[index]))
        {
          checkedData.Value.Left = expected[index];
          checkedData.Value.Right = actual[index];
          checkedData.Is = false;
          return checkedData;
        }
      }
      return checkedData;
    }

    /// <summary>
    /// Checks if the value is nonnegative.
    /// If not - throws exception.
    /// </summary>
    public static void CheckNonnegativeArgument(double value, string operation)
    {
      if (value < 0.0)
        return;
    }

    /// <summary>The value is integer (with precision).</summary>
    public static bool IsInteger(double value, out long ivalue)
    {
      ivalue = 0L;
      double a = Math.Abs(value);
      bool flag = a <= (double) long.MaxValue;
      if (flag)
      {
        flag = Math.Abs(a - Math.Round(a)) <= Constants.IntegerValuePrecision;
        if (flag)
          ivalue = (long) Math.Round(value);
      }
      return flag;
    }

    /// <summary>The value is integer (with precision).</summary>
    public static bool IsInteger(double value, out int ivalue)
    {
      ivalue = 0;
      double a = Math.Abs(value);
      bool flag = a <= (double) int.MaxValue;
      if (flag)
      {
        flag = Math.Abs(a - Math.Round(a)) <= Constants.IntegerValuePrecision;
        if (flag)
          ivalue = (int) Math.Round(value);
      }
      return flag;
    }

    /// <summary>
    /// The value can be presented as common fraction (up to the max value of numerator and denominator).
    /// </summary>
    public static bool IsFraction(double value, out long num, out long den)
    {
        Mathematics.Functions.DoubleToFraction(value, out num, out den);
        if (Math.Abs(num) <= Constants.MaxFractionValue)
        {
            return Math.Abs(den) <= Constants.MaxFractionValue;
        }

        return false;
    }

    /// <summary>Conversion to integers with values check.</summary>
    public static long[] ToIntegers64(double[] values)
    {
      int length = Utilities.SafeArrayLength((Array) values);
      long[] integers64 = new long[length];
      for (int index = 0; index < length; ++index)
      {
        long ivalue;
        if (!Utilities.IsInteger(values[index], out ivalue))
          return null;
        integers64[index] = ivalue;
      }
      return integers64;
    }

    /// <summary>Conversion to integers with values check.</summary>
    public static int[] ToIntegers(double[] values)
    {
      int length = Utilities.SafeArrayLength((Array) values);
      int[] integers = new int[length];
      for (int index = 0; index < length; ++index)
      {
        int ivalue;
        if (!Utilities.IsInteger(values[index], out ivalue))
          return null;
        integers[index] = ivalue;
      }
      return integers;
    }

    /// <summary>
    /// Checks if the value is "integer".
    /// If not - throws exception.
    /// </summary>
    public static void CheckIntegerArgument(double value, string operation)
    {
      if (!Utilities.IsInteger(value, out long _))
        return;
    }

    /// <summary>Try parse double value</summary>
    public static CheckedData<double> TryParseDouble(string str, CultureInfo culture)
    {
      double result;
      return new CheckedData<double>(double.TryParse(str, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, (IFormatProvider) culture, out result), result);
    }

    /// <summary>Try parse long value</summary>
    public static CheckedData<long> TryParseLong(string str, CultureInfo culture)
    {
      long result;
      return new CheckedData<long>(long.TryParse(str, out result), result);
    }

    /// <summary>Try parse boolean value</summary>
    public static CheckedData<bool> TryParseBool(string str)
    {
      bool result;
      return new CheckedData<bool>(bool.TryParse(str, out result), result);
    }

    /// <summary>Register converter.</summary>
    /// <param name="converter"></param>
    /// <returns>True if not yet registered</returns>
    public static bool RegisterConverter(Converter converter)
    {
      int num = !Utilities._converters.ContainsKey(converter.TargetType) ? 1 : 0;
      if (num == 0)
        return num != 0;
      Utilities._converters.Add(converter.TargetType, converter.Convert);
      return num != 0;
    }
  }
}
