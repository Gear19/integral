using System;
using System.Collections;
using System.Collections.Generic;

namespace Choe.Syntactic
{
  public class PowerExpression : BinaryOperationsExpression
  {
    protected override int GetSelfPrecedence() => Utilities.GetOperatorPrecedence(OperatorType.Power);

    protected override BinaryOperationType GetBinaryType() => BinaryOperationType.Power;

    public PowerExpression(
      string aValue,
      List<string> theOperators,
      List<BaseExpression> theOperands)
      : base(aValue, theOperators, theOperands)
    {
    }

    protected BaseExpression ToHierarchical()
    {
      if (this.operands == null || this.operands.Count < 2)
        return (BaseExpression) this;
      BaseExpression hierarchical = (BaseExpression) null;
      BaseExpression x = this.operands[0];
      int count = this.operands.Count;
      for (int index = 1; index < count; ++index)
      {
        BaseExpression operand = this.operands[index];
        hierarchical = PowerExpression.MakePower(x, operand);
        x = hierarchical;
      }
      return hierarchical;
    }

    protected BaseExpression ToSimple()
    {
      if (this.operands == null || this.operands.Count < 2)
        return this.Copy(false);
      BaseExpression x = this.operands[0].Copy(false);
      List<BaseExpression> expressions = new List<BaseExpression>();
      int num = this.operands.Count - 1;
      for (int index = 1; index <= num; ++index)
      {
        BaseExpression baseExpression = this.operands[index].Copy(false);
        expressions.Add(baseExpression);
      }
      BaseExpression y = BinaryOperationsExpression.MakeBinary(expressions, Utilities.GetOperatorSign(OperatorType.Multiply));
      return PowerExpression.MakePower(x, y);
    }

    public override bool CanExpand()
    {
      bool flag1 = base.CanExpand();
      if (flag1 || this.OperationCount != 1)
        return flag1;
      BaseExpression baseExpression = this.operands[0].This;
      switch (baseExpression)
      {
        case ProductExpression _:
          return ((BinaryOperationsExpression) baseExpression).OperationCount >= 1;
        case SumExpression _:
          bool flag2 = ((BinaryOperationsExpression) baseExpression).OperationCount >= 1;
          if (flag2)
          {
            BaseExpression expr = this.operands[1].This;
            long num = 0;
            ref long local = ref num;
            flag2 = LiteralExpression.IsInteger(expr, ref local) && num > 1L;
          }
          return flag2;
        default:
          return flag1;
      }
    }

    public override BaseExpression Expand()
    {
      BaseExpression baseExpression1 = base.Expand();
      if (baseExpression1 == null)
      {
        if (this.OperationCount != 1)
          return baseExpression1;
        BaseExpression baseExpression2 = this.operands[0].This;
        if (baseExpression2 is ProductExpression)
        {
          ProductExpression productExpression = (ProductExpression) baseExpression2;
          if (productExpression.OperationCount < 1)
            return baseExpression1;
          List<BaseExpression> operands = new List<BaseExpression>();
          int operationCount = productExpression.OperationCount;
          for (int index = 0; index <= operationCount; ++index)
          {
            BaseExpression y = this.operands[1].Copy(false);
            BaseExpression baseExpression3 = PowerExpression.MakePower(productExpression.Operands[index].Copy(false), y);
            operands.Add(baseExpression3);
          }
          List<string> operators = new List<string>((IEnumerable<string>) productExpression.Operators);
          baseExpression1 = BaseExpression.CreateBinaryExpression(Symbols.NotDefinedSign, BinaryOperationType.Product, operators, operands);
          baseExpression2 = (BaseExpression) null;
        }
        if (baseExpression2 is SumExpression)
        {
          SumExpression sumExpression = (SumExpression) baseExpression2;
          if (sumExpression.OperationCount < 1)
            return baseExpression1;
          BaseExpression expr = this.operands[1].This;
          long num1 = 0;
          ref long local = ref num1;
          if (!LiteralExpression.IsInteger(expr, ref local) || num1 <= 1L)
            return baseExpression1;
          List<BaseExpression> expressions = new List<BaseExpression>();
          int num2 = (int) num1;
          for (int index = 1; index <= num2; ++index)
          {
            BaseExpression baseExpression4 = sumExpression.Copy(false);
            expressions.Add(baseExpression4);
          }
          baseExpression1 = BinaryOperationsExpression.MakeBinary(expressions, Utilities.GetOperatorSign(OperatorType.Multiply));
        }
      }
      if (baseExpression1 != null && baseExpression1.This is BinaryOperationsExpression)
      {
        BinaryOperationsExpression operationsExpression = (BinaryOperationsExpression) baseExpression1.This;
        if (operationsExpression.CanExpand())
          baseExpression1 = operationsExpression.Expand();
      }
      return baseExpression1;
    }

    protected override BaseExpression SelfIntegral(string vName)
    {
      BaseExpression baseExpression1 = (BaseExpression) null;
      if (this.OperationCount == 1)
      {
        BaseExpression operand1 = this.operands[0];
        BaseExpression operand2 = this.operands[1];
        bool flag1 = !operand1.DependsOn(vName);
        if (!operand2.DependsOn(vName))
        {
          bool flag2 = VariableExpression.IsVariable(operand1, vName);
          bool flag3 = false;
          LinearExpression linearExpression = (LinearExpression) null;
          if (!flag2)
          {
            linearExpression = LinearExpression.Extract(operand1, vName);
            flag3 = linearExpression != null;
          }
          if (flag2 | flag3)
          {
            if (LiteralExpression.IsNegation(operand2) || UnaryOperatorExpression.IsNegation(operand2))
            {
              BaseExpression baseExpression2 = flag2 || flag3 && linearExpression.B == null ? (BaseExpression) new VariableExpression(vName) : operand1.Copy(false);
              BaseExpression x1 = (BaseExpression) FunctionExpression.CreateSimple(Names.NaturalLogarithm, baseExpression2);
              if (flag3)
              {
                BaseExpression x2 = linearExpression.A.Copy(false);
                x1 = ProductExpression.MakeDivision(x1, x2);
              }
              x1.Context = this.Context;
              baseExpression1 = x1;
            }
            else
            {
              BaseExpression y = SumExpression.MakeSum(operand2.Copy(false), (BaseExpression) LiteralExpression.Unit);
              BaseExpression x2 = SumExpression.MakeSum(operand2.Copy(false), (BaseExpression) LiteralExpression.Unit);
              BaseExpression x1;
              if (flag2)
              {
                x1 = PowerExpression.MakePower((BaseExpression) new VariableExpression(vName), y);
              }
              else
              {
                x1 = PowerExpression.MakePower(operand1.Copy(false), y);
                x2 = ProductExpression.MakeProduct(linearExpression.A.Copy(false), x2);
              }
              x1.Context = this.Context;
              baseExpression1 = ProductExpression.MakeDivision(x1, x2);
            }
          }
          return baseExpression1;
        }
        if (flag1)
        {
          int num = VariableExpression.IsVariable(operand2, vName) ? 1 : 0;
          bool flag4 = false;
          LinearExpression linearExpression = (LinearExpression) null;
          if (num == 0)
          {
            linearExpression = LinearExpression.Extract(operand2, vName);
            flag4 = linearExpression != null;
          }
          if ((num | (flag4 ? 1 : 0)) != 0)
          {
            BaseExpression x1 = PowerExpression.MakePower(operand1.Copy(false), operand2.Copy(false));
            x1.Context = this.Context;
            BaseExpression baseExpression3 = (BaseExpression) null;
            if (!LiteralExpression.IsEuler(operand1))
            {
              BaseExpression baseExpression4 = operand1.Copy(false);
              baseExpression3 = (BaseExpression) FunctionExpression.CreateSimple(Names.NaturalLogarithm, baseExpression4);
              baseExpression3.Context = this.Context;
            }
            if (flag4)
            {
              BaseExpression x2 = linearExpression.A.Copy(false);
              baseExpression3 = baseExpression3 == null ? x2 : ProductExpression.MakeProduct(baseExpression3, x2);
            }
            baseExpression1 = baseExpression3 == null ? x1 : ProductExpression.MakeDivision(x1, baseExpression3);
          }
          return baseExpression1;
        }
      }
      else if (this.OperationCount >= 2)
        baseExpression1 = this.ToSimple().Integrate(vName);
      return baseExpression1;
    }

    protected override void MergeOperands()
    {
      if (this.IsDegenerated)
        return;
      bool flag1 = false;
      bool flag2 = false;
      BaseExpression operand = this.operands[0];
      if (operand is PowerExpression)
      {
        PowerExpression powerExpression = (PowerExpression) operand;
        if (!powerExpression.IsDegenerated)
        {
          int num = Utilities.SafeCount((IList) powerExpression.Operands);
          this.operands[0] = powerExpression.Operands[0];
          for (int index = 1; index < num; ++index)
            this.InsertOperationRight(index - 1, Symbols.PowerOperator.ToString() ?? "", powerExpression.Operands[index]);
          flag1 = true;
          flag2 = true;
        }
      }
      else if (operand is UnaryOperatorExpression)
      {
        UnaryOperatorExpression expr = operand as UnaryOperatorExpression;
        if (UnaryOperatorExpression.IsUnary((BaseExpression) expr, OperatorType.SquareRoot) && this.OperationCount == 1)
        {
          this.operands[0] = expr.Operand;
          this.operands[1] = ProductExpression.MakeProduct(ProductExpression.OneHalf, this.operands[1]);
          this.operands[1].Simplify();
          flag1 = true;
          flag2 = true;
        }
      }
      if (flag2)
        this.MergeDegenerated();
      if (!flag1)
        return;
      this.MergeOperands();
    }

    protected override void MergeConstants()
    {
      bool[] iv = (bool[]) null;
      double[] rv = (double[]) null;
      if (this.GetRealValues(ref iv, ref rv) < 2)
        return;
      int num1 = Utilities.SafeCount((IList) this.operands);
      bool flag = false;
      if (num1 == 2)
      {
        double num2 = Math.Pow(rv[0], rv[1]);
        this.ClearOperations();
        this.operands.Add((BaseExpression) LiteralExpression.Make(num2));
      }
      else
      {
        int num3 = 1;
        for (int index = 1; index <= num1 - 2; ++index)
        {
          if (iv[index] && iv[index + 1])
          {
            this.RemoveOperationLeft(num3);
            BaseExpression operand = this.operands[num3];
            double num4 = rv[index] * rv[index + 1];
            LiteralExpression literalExpression = LiteralExpression.Make(num4);
            this.operands[num3] = (BaseExpression) literalExpression;
            rv[index + 1] = num4;
            flag = true;
          }
          else
            ++num3;
        }
      }
      if (!flag)
        return;
      this.MergeConstants();
    }

    protected override void SimplifyBase()
    {
      if (this.OperationCount > 0)
      {
        if (LiteralExpression.IsUnit(this.operands[0]))
        {
          this.ClearOperations();
          this.operands.Add((BaseExpression) LiteralExpression.Unit);
        }
        else
        {
          for (int index = this.operands.Count - 1; index > 0; --index)
          {
            if (LiteralExpression.IsZero(this.operands[index]))
            {
              this.ClearOperations();
              this.operands.Add((BaseExpression) LiteralExpression.Unit);
              return;
            }
          }
          if (this.OperationCount > 0)
          {
            for (int operation = this.operators.Count - 1; operation >= 0; --operation)
            {
              if (LiteralExpression.IsUnit(this.operands[operation + 1]))
                this.RemoveOperationRight(operation);
            }
          }
        }
      }
      if (this.OperationCount > 1)
      {
        List<BaseExpression> expressions = new List<BaseExpression>();
        int num = this.operands.Count - 1;
        for (int index = 1; index <= num; ++index)
        {
          BaseExpression operand = this.operands[index];
          expressions.Add(operand);
        }
        BaseExpression baseExpression = BinaryOperationsExpression.MakeBinary(expressions, Utilities.GetOperatorSign(OperatorType.Multiply));
        this.operands.RemoveRange(2, this.operands.Count - 2);
        this.operators.RemoveRange(1, this.operators.Count - 1);
        this.operands[1] = baseExpression;
        this.operands[1].Simplify();
      }
      if (this.OperationCount != 1)
        return;
      BaseExpression operand1 = this.operands[0];
      if (!UnaryOperatorExpression.IsUnary(operand1, OperatorType.Minus))
        return;
      BaseExpression operand2 = this.operands[1];
      double num1 = 0.0;
      ref double local = ref num1;
      long ivalue;
      if (!LiteralExpression.IsRealValue(operand2, ref local) || !Utilities.IsInteger(num1, out ivalue) || Math.Abs(ivalue) <= 1L || ivalue % 2L != 0L)
        return;
      this.operands[0] = ((UnaryOperatorExpression) operand1).Operand;
      this.operands[1] = (BaseExpression) LiteralExpression.Make(ivalue);
    }

    public static BaseExpression MakePower(BaseExpression x, BaseExpression y)
    {
      if (LiteralExpression.IsUnit(y))
        return x;
      if (LiteralExpression.IsZero(y))
        return (BaseExpression) LiteralExpression.Unit;
      if (LiteralExpression.IsUnit(x))
        return (BaseExpression) LiteralExpression.Unit;
      List<string> theOperators = new List<string>((IEnumerable<string>) new string[1]
      {
        Symbols.PowerOperator.ToString()
      });
      List<BaseExpression> theOperands = new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[2]
      {
        x,
        y
      });
      return (BaseExpression) new PowerExpression(Symbols.NotDefinedSign, theOperators, theOperands);
    }

    public static BaseExpression MakeSquare(BaseExpression value)
    {
      if (LiteralExpression.IsZero(value))
        return (BaseExpression) LiteralExpression.Zero;
      if (LiteralExpression.IsUnit(value))
        return (BaseExpression) LiteralExpression.Unit;
      if (LiteralExpression.IsInfinity(value))
        return (BaseExpression) LiteralExpression.Infinity;
      if (LiteralExpression.IsNaN(value))
        return (BaseExpression) LiteralExpression.NaN;
      BaseExpression y = (BaseExpression) LiteralExpression.Make(2L);
      return PowerExpression.MakePower(value, y);
    }

    public static BaseExpression MakeSquareRoot(BaseExpression value)
    {
      if (LiteralExpression.IsZero(value))
        return (BaseExpression) LiteralExpression.Zero;
      if (LiteralExpression.IsUnit(value))
        return (BaseExpression) LiteralExpression.Unit;
      if (LiteralExpression.IsInfinity(value))
        return (BaseExpression) LiteralExpression.Infinity;
      return LiteralExpression.IsNaN(value) ? (BaseExpression) LiteralExpression.NaN : PowerExpression.MakePower(value, ProductExpression.OneHalf);
    }
  }
}
