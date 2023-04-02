using Mathematics.Fractions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Choe.Syntactic
{
  public class SumExpression : BinaryOperationsExpression
  {
    protected override int GetSelfPrecedence() => Utilities.GetOperatorPrecedence(OperatorType.Add);

    protected override BinaryOperationType GetBinaryType() => BinaryOperationType.Sum;

    public static bool IsInverseOperator(string op)
    {
      OperatorType operatorType = Utilities.GetOperatorType(op, OperatorArity.Binary);
      switch (operatorType)
      {
        case OperatorType.Add:
          return false;
        case OperatorType.Subtract:
          return true;
        default:
          return false;
      }
    }

    protected void InverseOperation(ref string op)
    {
      OperatorType operatorType = Utilities.GetOperatorType(op, OperatorArity.Binary);
      switch (operatorType)
      {
        case OperatorType.Add:
          op = Utilities.GetOperatorSign(OperatorType.Subtract);
          break;
        case OperatorType.Subtract:
          op = Utilities.GetOperatorSign(OperatorType.Add);
          break;
        default:
          return;
      }
    }

    protected override void MergeOperands()
    {
      if (this.IsDegenerated)
        return;
      int num1 = Utilities.SafeCount((IList) this.operands);
      bool flag1 = false;
      for (int index1 = num1 - 1; index1 >= 0; --index1)
      {
        BaseExpression operand1 = this.operands[index1];
        switch (operand1)
        {
          case UnaryOperatorExpression _:
            UnaryOperatorExpression operatorExpression1 = (UnaryOperatorExpression) operand1;
            if (!operatorExpression1.IsDegenerated && operatorExpression1.Operator == OperatorType.Minus)
            {
              BaseExpression operand2 = operatorExpression1.Operand;
              if (index1 > 0)
              {
                string op = this.operators[index1 - 1];
                this.InverseOperation(ref op);
                this.operands[index1] = operand2;
                this.operators[index1 - 1] = op;
                flag1 = true;
                break;
              }
              bool flag2 = operand2 is SumExpression;
              if (!flag2)
              {
                flag2 = operand2 is UnaryOperatorExpression;
                if (flag2)
                {
                  UnaryOperatorExpression operatorExpression2 = (UnaryOperatorExpression) operand2;
                  flag2 = !operatorExpression2.IsDegenerated;
                  if (flag2)
                    flag2 = operatorExpression2.Operator == OperatorType.Minus;
                }
              }
              if (flag2)
              {
                this.operands[0] = (BaseExpression) LiteralExpression.Zero;
                this.InsertOperationRight(Utilities.SafeCount((IList) this.operands) - 1, Symbols.SubtractOperator.ToString() ?? "", operand2);
                flag1 = true;
                break;
              }
              break;
            }
            break;
          case SumExpression _:
            SumExpression sumExpression = (SumExpression) operand1;
            if (!sumExpression.IsDegenerated)
            {
              int num2 = Utilities.SafeCount((IList) sumExpression.Operands);
              bool flag3 = false;
              if (index1 > 0)
                flag3 = SumExpression.IsInverseOperator(this.operators[index1 - 1]);
              string[] array1 = sumExpression.Operators.ToArray();
              BaseExpression[] array2 = sumExpression.Operands.ToArray();
              flag1 = true;
              if (index1 > 0)
              {
                this.RemoveOperationRight(index1 - 1);
                for (int index2 = num2 - 1; index2 >= 0; --index2)
                {
                  string op = index2 != 0 ? array1[index2 - 1] : Symbols.AddOperator.ToString() ?? "";
                  if (flag3)
                    this.InverseOperation(ref op);
                  this.InsertOperationRight(index1 - 1, op, array2[index2]);
                }
                break;
              }
              for (int index3 = num2 - 1; index3 >= 0; --index3)
              {
                if (index3 == 0)
                {
                  this.operands[0] = array2[0];
                }
                else
                {
                  string op = array1[index3 - 1];
                  if (flag3)
                    this.InverseOperation(ref op);
                  this.InsertOperationRight(0, op, array2[index3]);
                }
              }
              break;
            }
            break;
          case ProductExpression _:
            ProductExpression productExpression = operand1 as ProductExpression;
            if (productExpression.Operands.Count > 0)
            {
              BaseExpression operand3 = productExpression.Operands[0];
              double x = 0.0;
              if (index1 > 0 && (UnaryOperatorExpression.IsUnary(operand3, OperatorType.Minus) || LiteralExpression.IsRealValue(operand3, ref x) && x < 0.0))
              {
                productExpression.Negate();
                string op = this.operators[index1 - 1];
                this.InverseOperation(ref op);
                this.operators[index1 - 1] = op;
                flag1 = true;
                break;
              }
              break;
            }
            break;
        }
      }
      if (!flag1)
        return;
      this.SimplifyOperands();
      this.MergeOperands();
    }

    protected override void SimplifyBase()
    {
      if (this.OperationCount <= 0)
        return;
      int count = this.operands.Count;
      List<string> stringList = new List<string>();
      stringList.AddRange((IEnumerable<string>) BaseExpression.GetReconstructed(this.operands));
      for (int index1 = 0; index1 <= count - 2; ++index1)
      {
        int num1 = this.operands.Count - 1;
        if (index1 < num1)
        {
          BaseExpression operand = this.operands[index1];
          BaseExpression x2 = operand;
          double num2 = 1.0;
          bool flag = false;
          if (index1 == 0)
          {
            if (UnaryOperatorExpression.IsUnary(operand, OperatorType.Minus))
            {
              num2 = -1.0;
              x2 = (operand as UnaryOperatorExpression).Operand;
              stringList[0] = x2.Reconstruct();
            }
          }
          else if (Utilities.GetOperatorType(this.operators[index1 - 1], OperatorArity.Binary) == OperatorType.Subtract)
            num2 = -1.0;
          double num3 = num2;
          for (int index2 = num1; index2 >= index1 + 1; --index2)
          {
            if (stringList[index1] == stringList[index2])
            {
              double num4 = 1.0;
              if (Utilities.GetOperatorType(this.operators[index2 - 1], OperatorArity.Binary) == OperatorType.Subtract)
                num4 = -1.0;
              num3 += num4;
              flag = true;
              this.RemoveOperationRight(index2 - 1);
              stringList.RemoveAt(index2);
            }
          }
          if (flag)
          {
            if (num3 == 0.0)
            {
              this.operands[index1] = (BaseExpression) LiteralExpression.Zero;
            }
            else
            {
              string operatorSign = Utilities.GetOperatorSign(OperatorType.Add);
              if (index1 != 0 && num3 < 0.0)
              {
                num3 = Math.Abs(num3);
                operatorSign = Utilities.GetOperatorSign(OperatorType.Subtract);
              }
              BaseExpression baseExpression = ProductExpression.MakeProduct((BaseExpression) LiteralExpression.Make(num3), x2);
              baseExpression.Simplify();
              this.operands[index1] = baseExpression;
              if (index1 != 0)
                this.operators[index1 - 1] = operatorSign;
            }
          }
        }
        else
          break;
      }
      for (int operation = this.operators.Count - 1; operation >= 0; --operation)
      {
        if (LiteralExpression.IsZero(this.operands[operation + 1]))
          this.RemoveOperationRight(operation);
      }
      if (this.OperationCount <= 0 || !LiteralExpression.IsZero(this.operands[0]))
        return;
      int operatorType = (int) Utilities.GetOperatorType(this.operators[0], OperatorArity.Binary);
      this.RemoveOperationLeft(0);
      if (operatorType != 11)
        return;
      BaseExpression baseExpression1 = UnaryOperatorExpression.Negate(this.operands[0]);
      baseExpression1.Simplify();
      this.operands[0] = baseExpression1;
    }

    private BaseExpression MakeExpression(double sum)
    {
      long num;
      long den;
      return !Utilities.IsFraction(sum, out num, out den) ? (BaseExpression) LiteralExpression.Make(sum) : (den != 1L ? ProductExpression.MakeDivision((BaseExpression) LiteralExpression.Make(num), (BaseExpression) LiteralExpression.Make(den)) : (BaseExpression) LiteralExpression.Make(sum));
    }

    protected override void MergeConstants()
    {
      int length = Utilities.SafeCount((IList) this.operands);
      if (length < 2)
        return;
      bool[] iv = (bool[]) null;
      double[] rv = (double[]) null;
      int realValues = this.GetRealValues(ref iv, ref rv);
      bool[] flagArray = new bool[length];
      Fraction[] fractionArray = new Fraction[length];
      int num1 = 0;
      for (int index = 0; index < length; ++index)
      {
        flagArray[index] = false;
        if (!iv[index])
        {
          Fraction r;
          flagArray[index] = ProductExpression.IsRational(this.operands[index], out r);
          if (flagArray[index])
          {
            fractionArray[index] = r;
            ++num1;
          }
        }
      }
      if (realValues < 1 && num1 < 2)
        return;
      double sum = 0.0;
      for (int index = length - 1; index >= 0; --index)
      {
        if (iv[index] || flagArray[index])
        {
          double num2 = !iv[index] ? fractionArray[index].ToDouble() : rv[index];
          if (index == 0)
          {
            sum += num2;
          }
          else
          {
            OperatorType operatorType = Utilities.GetOperatorType(this.operators[index - 1], OperatorArity.Binary);
            double num3;
            if (operatorType != OperatorType.Add)
            {
              if (operatorType != OperatorType.Subtract)
                return;
              num3 = -1.0;
            }
            else
              num3 = 1.0;
            sum += num3 * num2;
            this.RemoveOperationRight(index - 1);
          }
        }
      }
      int operationCount = this.OperationCount;
      bool flag = sum == 0.0;
      if (flag)
        sum = 0.0;
      if (iv[0] || flagArray[0])
      {
        if (flag && operationCount > 0)
        {
          int operatorType = (int) Utilities.GetOperatorType(this.operators[0], OperatorArity.Binary);
          this.RemoveOperationLeft(0);
          if (operatorType != 11)
            return;
          this.operands[0] = UnaryOperatorExpression.Negate(this.operands[0]);
        }
        else
          this.operands[0] = this.MakeExpression(sum);
      }
      else
      {
        if (flag)
          return;
        string op = Symbols.AddOperator.ToString() ?? "";
        if (sum < 0.0)
        {
          op = Symbols.SubtractOperator.ToString() ?? "";
          sum = Math.Abs(sum);
        }
        BaseExpression opnd = this.MakeExpression(sum);
        this.InsertOperationRight(operationCount, op, opnd);
      }
    }

    public SumExpression(
      string aValue,
      List<string> theOperators,
      List<BaseExpression> theOperands)
      : base(aValue, theOperators, theOperands)
    {
    }

    protected override BaseExpression SelfIntegral(string vName)
    {
      BaseExpression baseExpression = (BaseExpression) null;
      List<BaseExpression> theOperands = BaseExpression.Integrals(this.operands, vName);
      if (theOperands != null && theOperands.Count == this.operands.Count)
      {
        List<string> theOperators = new List<string>();
        theOperators.AddRange((IEnumerable<string>) this.operators.ToArray());
        baseExpression = (BaseExpression) new SumExpression(Symbols.NotDefinedSign, theOperators, theOperands);
      }
      return baseExpression;
    }

    public void Negate()
    {
      if (this.IsDegenerated)
      {
        this.operands[0] = UnaryOperatorExpression.Negate(this.operands[0]);
      }
      else
      {
        int operationCount = this.OperationCount;
        this.operands[0] = UnaryOperatorExpression.Negate(this.operands[0]);
        for (int index = 0; index < operationCount; ++index)
        {
          string op = this.operators[index];
          this.InverseOperation(ref op);
          this.operators[index] = op;
        }
      }
    }

    protected static BaseExpression MakeOperation(
      BaseExpression operand1,
      BaseExpression operand2,
      string sign)
    {
      List<string> theOperators = new List<string>((IEnumerable<string>) new string[1]
      {
        sign
      });
      List<BaseExpression> theOperands = new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[2]
      {
        operand1,
        operand2
      });
      return (BaseExpression) new SumExpression(Symbols.NotDefinedSign, theOperators, theOperands);
    }

    public static BaseExpression MakeSum(BaseExpression x1, BaseExpression x2)
    {
      if (LiteralExpression.IsZero(x1))
        return x2;
      if (LiteralExpression.IsZero(x2))
        return x1;
      int num1 = UnaryOperatorExpression.IsUnary(x1, OperatorType.Minus) ? 1 : 0;
      bool flag = UnaryOperatorExpression.IsUnary(x2, OperatorType.Minus);
      int num2 = flag ? 1 : 0;
      if ((num1 & num2) != 0)
        return UnaryOperatorExpression.Negate(SumExpression.MakeSum((x1 as UnaryOperatorExpression).Operand, (x2 as UnaryOperatorExpression).Operand));
      if (!flag)
        return SumExpression.MakeOperation(x1, x2, Symbols.AddOperator.ToString());
      BaseExpression operand = (x2 as UnaryOperatorExpression).Operand;
      return SumExpression.MakeDifference(x1, operand);
    }

    public static BaseExpression MakeDifference(BaseExpression x1, BaseExpression x2)
    {
      if (LiteralExpression.IsZero(x2))
        return x1;
      if (LiteralExpression.IsZero(x1))
        return UnaryOperatorExpression.Negate(x2);
      if (!UnaryOperatorExpression.IsUnary(x2, OperatorType.Minus))
        return SumExpression.MakeOperation(x1, x2, Symbols.SubtractOperator.ToString());
      BaseExpression operand = (x2 as UnaryOperatorExpression).Operand;
      return SumExpression.MakeSum(x1, operand);
    }
  }
}
