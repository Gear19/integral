using System;
using System.Numerics;

namespace Choe.Syntactic
{
  public class LiteralExpression : SimpleExpression
  {
    private static LiteralExpression zero = new LiteralExpression(Symbols.ZeroString);
    private static LiteralExpression unit = new LiteralExpression(Symbols.UnitString);
    private static LiteralExpression negation = LiteralExpression.Make(-1L);
    private static LiteralExpression euler = new LiteralExpression(Symbols.EulerString);
    private static LiteralExpression pi = new LiteralExpression(Symbols.PiString);
    private static LiteralExpression nan = new LiteralExpression(Symbols.NaNName);
    private static LiteralExpression infinity = new LiteralExpression(Symbols.InfinityString);
    private static LiteralExpression falseexp = new LiteralExpression(Symbols.FalseName);
    private static LiteralExpression trueexp = new LiteralExpression(Symbols.TrueName);

    protected override int GetPrecedence() => Utilities.GetVariablePrecedence();

    public LiteralExpression(string aValue)
      : base(aValue)
    {
    }

    public string Value => this.value;

    public override void Simplify()
    {
      CheckedData<double> checkedData1 = Parser.IsReal(this.value, true);
      if (checkedData1.Is)
      {
        CheckedData<string> checkedData2 = Parser.IsRealConstant(checkedData1.Value);
        if (!checkedData2.Is)
          return;
        this.value = checkedData2.Value;
      }
      else
      {
        CheckedData<Complex> checkedData3 = Parser.IsComplex(this.value, true);
        if (!checkedData3.Is)
          return;
        CheckedData<string> checkedData4 = Parser.IsComplexConstant(checkedData3.Value);
        if (!checkedData4.Is)
          return;
        this.value = checkedData4.Value;
      }
    }

    public override string Reconstruct() => this.value;

    public override bool IsConstant() => true;

    public override bool DependsOn(string vName) => false;

    protected override BaseExpression SelfIntegral(string vName) => ProductExpression.MakeProduct(this.Copy(false), (BaseExpression) new VariableExpression(vName));

    public override string ToString() => "Literal: " + this.Value;

    public override string Print(int level) => new string(' ', level * 2) + this.ToString();

    public static LiteralExpression Zero => LiteralExpression.zero;

    public static LiteralExpression Unit => LiteralExpression.unit;

    public static LiteralExpression Negation => LiteralExpression.negation;

    public static LiteralExpression Euler => LiteralExpression.euler;

    public static LiteralExpression Pi => LiteralExpression.pi;

    public static LiteralExpression NaN => LiteralExpression.nan;

    public static LiteralExpression Infinity => LiteralExpression.infinity;

    public static LiteralExpression False => LiteralExpression.falseexp;

    public static LiteralExpression True => LiteralExpression.trueexp;

    public static bool IsZero(BaseExpression value)
    {
      if (!(value is LiteralExpression literalExpression))
        return false;
      bool flag = literalExpression.Value == Symbols.ZeroString;
      if (!flag)
      {
        CheckedData<double> checkedData = Parser.IsReal(literalExpression.Value, true);
        if (checkedData.Is)
          flag = checkedData.Value == 0.0;
      }
      return flag;
    }

    public static bool IsUnit(BaseExpression value)
    {
      if (!(value is LiteralExpression literalExpression))
        return false;
      bool flag = literalExpression.Value == Symbols.UnitString;
      if (!flag)
      {
        CheckedData<double> checkedData = Parser.IsReal(literalExpression.Value, true);
        if (checkedData.Is)
          flag = Math.Abs(checkedData.Value - 1.0) < Constants.RealConstantPrecision;
      }
      return flag;
    }

    public static bool IsNegation(BaseExpression value)
    {
      if (!(value is LiteralExpression literalExpression))
        return false;
      bool flag = false;
      CheckedData<double> checkedData = Parser.IsReal(literalExpression.Value, true);
      if (checkedData.Is)
        flag = Math.Abs(checkedData.Value + 1.0) < Constants.RealConstantPrecision;
      return flag;
    }

    public static bool IsEuler(BaseExpression value)
    {
      if (!(value is LiteralExpression literalExpression))
        return false;
      bool flag = literalExpression.Value == Symbols.EulerString;
      if (!flag)
      {
        CheckedData<double> checkedData = Parser.IsReal(literalExpression.Value, true);
        if (checkedData.Is)
          flag = Math.Abs(checkedData.Value - Math.E) < Constants.RealConstantPrecision;
      }
      return flag;
    }

    public static bool IsPi(BaseExpression value)
    {
      if (!(value is LiteralExpression literalExpression))
        return false;
      bool flag = literalExpression.Value == Symbols.PiString || literalExpression.Value == Symbols.PiName;
      if (!flag)
      {
        CheckedData<double> checkedData = Parser.IsReal(literalExpression.Value, true);
        if (checkedData.Is)
          flag = Math.Abs(checkedData.Value - Math.PI) < Constants.RealConstantPrecision;
      }
      return flag;
    }

    public static bool IsNaN(BaseExpression value)
    {
      if (!(value is LiteralExpression literalExpression))
        return false;
      bool flag = literalExpression.Value == Symbols.NaNName;
      if (!flag)
      {
        CheckedData<double> checkedData = Parser.IsReal(literalExpression.Value, true);
        if (checkedData.Is)
          flag = double.IsNaN(checkedData.Value);
      }
      return flag;
    }

    public static bool IsInfinity(BaseExpression value)
    {
      if (!(value is LiteralExpression literalExpression))
        return false;
      return literalExpression.Value == Symbols.InfinityString || literalExpression.Value == Symbols.InfinityName || literalExpression.Value == Symbols.InfinityFullName;
    }

    public static bool IsFalse(BaseExpression value)
    {
      if (value is LiteralExpression literalExpression)
      {
        CheckedData<bool> checkedData = Parser.IsBoolean(literalExpression.Value, true);
        if (checkedData.Is)
          return !checkedData.Value;
      }
      return false;
    }

    public static bool IsTrue(BaseExpression value)
    {
      if (value is LiteralExpression literalExpression)
      {
        CheckedData<bool> checkedData = Parser.IsBoolean(literalExpression.Value, true);
        if (checkedData.Is)
          return checkedData.Value;
      }
      return false;
    }

    public static bool IsUnnamed(BaseExpression expr)
    {
      if (!(expr is LiteralExpression))
        return false;
      string primaryValue = expr.PrimaryValue;
      CheckedData<double> checkedData1 = Parser.IsReal(primaryValue, true);
      if (checkedData1.Is)
      {
        if (Parser.IsRealConstant(checkedData1.Value).Is)
          return false;
      }
      else
      {
        CheckedData<Complex> checkedData2 = Parser.IsComplex(primaryValue, true);
        if (checkedData2.Is && Parser.IsComplexConstant(checkedData2.Value).Is)
          return false;
      }
      return true;
    }

    public static bool IsRealValue(BaseExpression expr, ref double x)
    {
      x = double.NaN;
      switch (expr)
      {
        case LiteralExpression _:
          CheckedData<double> checkedData = Parser.IsReal(((LiteralExpression) expr).Value, true);
          if (checkedData.Is)
          {
            x = checkedData.Value;
            checkedData.Is = !Parser.IsRealConstant(x).Is;
          }
          return checkedData.Is;
        case UnaryOperatorExpression _:
          UnaryOperatorExpression operatorExpression = (UnaryOperatorExpression) expr;
          bool flag;
          if (operatorExpression.IsDegenerated)
          {
            flag = LiteralExpression.IsRealValue(operatorExpression.Operand, ref x);
          }
          else
          {
            flag = operatorExpression.Operator == OperatorType.Minus;
            if (flag)
              flag = LiteralExpression.IsRealValue(operatorExpression.Operand, ref x);
            if (flag)
              x = -x;
          }
          return flag;
        case BinaryOperationsExpression _:
          BinaryOperationsExpression operationsExpression = (BinaryOperationsExpression) expr;
          return operationsExpression.IsDegenerated && LiteralExpression.IsRealValue(operationsExpression.Operands[0], ref x);
        default:
          return false;
      }
    }

    public static bool IsInteger(BaseExpression expr, ref long x)
    {
      x = 0L;
      double x1 = 0.0;
      bool flag = LiteralExpression.IsRealValue(expr, ref x1);
      if (flag)
        flag = Utilities.IsInteger(x1, out x);
      return flag;
    }

    public static LiteralExpression Make(double value)
    {
      LiteralExpression literalExpression = new LiteralExpression(Parser.RealToString(value));
      literalExpression.Simplify();
      return literalExpression;
    }

    public static LiteralExpression Make(Complex value) => new LiteralExpression(Parser.ComplexToString(value));

    public static LiteralExpression Make(long value) => new LiteralExpression(value.ToString());

    public static LiteralExpression Make(bool value) => new LiteralExpression(Parser.BooleanToString(value));
  }
}
