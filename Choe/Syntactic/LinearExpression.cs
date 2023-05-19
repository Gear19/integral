using System.Collections.Generic;

namespace Choe.Syntactic
{
  public class LinearExpression : SumExpression
  {
    protected VariableExpression _x;
    protected BaseExpression _a;
    protected BaseExpression _b;

    public VariableExpression X => this._x;

    public BaseExpression A => this._a;

    public BaseExpression B => this._b;

    private static List<BaseExpression> CreateOperands(
      VariableExpression x,
      BaseExpression a,
      BaseExpression b)
    {
      if (x == null)
        return null;
      if (a == null)
        return null;
      if (LiteralExpression.IsZero(a))
      { 
        a.Reconstruct();
        return null;
      }
      if (a.DependsOn(x.Name))
      { 
        a.Reconstruct();
        return null;
      }
      if (b != null && b.DependsOn(x.Name))
      { 
        b.Reconstruct();
        return null;
      }
      List<BaseExpression> operands = new List<BaseExpression>();
      BaseExpression x1 = (BaseExpression) null;
      if (LiteralExpression.IsUnit(a))
        x1 = (BaseExpression) LiteralExpression.Unit;
      else if (LiteralExpression.IsNegation(a))
        x1 = (BaseExpression) LiteralExpression.Negation;
      else if (LiteralExpression.IsInfinity(a))
        x1 = (BaseExpression) LiteralExpression.Infinity;
      if (x1 == null)
        x1 = a;
      BaseExpression baseExpression = ProductExpression.MakeProduct(x1, (BaseExpression) x);
      operands.Add(baseExpression);
      if (b != null)
        operands.Add(b);
      return operands;
    }

    private static List<string> CreateOperators(
      VariableExpression x,
      BaseExpression a,
      BaseExpression b)
    {
      List<string> operators = new List<string>();
      if (b != null)
        operators.Add(Symbols.AddOperator.ToString());
      return operators;
    }

    public LinearExpression(
      string aValue,
      VariableExpression x,
      BaseExpression a,
      BaseExpression b)
      : base(aValue, LinearExpression.CreateOperators(x, a, b), LinearExpression.CreateOperands(x, a, b))
    {
      this._x = x;
      this._b = b;
      this._a = a;
    }

    public static LinearExpression Extract(BaseExpression expr, string vName)
    {
      LinearExpression linearExpression1 = (LinearExpression) null;
      if (expr is StructuredExpression)
      {
        StructuredExpression structuredExpression = (StructuredExpression) expr;
        if (structuredExpression.IsDegenerated)
          expr = structuredExpression.Degenerated;
      }
      if (!expr.DependsOn(vName))
        return linearExpression1;
      switch (expr)
      {
        case VariableExpression _:
          if (((VariableExpression) expr).Name == vName)
          {
            VariableExpression x = new VariableExpression(vName);
            linearExpression1 = new LinearExpression(vName, x, (BaseExpression) LiteralExpression.Unit, (BaseExpression) null);
          }
          return linearExpression1;
        case UnaryOperatorExpression _:
          UnaryOperatorExpression operatorExpression = (UnaryOperatorExpression) expr;
          if (operatorExpression.Operator == OperatorType.Minus)
          {
            LinearExpression linearExpression2 = LinearExpression.Extract(operatorExpression.Operand, vName);
            if (linearExpression2 != null)
            {
              BaseExpression a = (BaseExpression) null;
              if (linearExpression2.A != null)
                a = UnaryOperatorExpression.Negate(linearExpression2.A.Copy(false));
              BaseExpression b = (BaseExpression) null;
              if (linearExpression2.B != null)
                b = UnaryOperatorExpression.Negate(linearExpression2.B.Copy(false));
              VariableExpression x = new VariableExpression(vName);
              linearExpression1 = new LinearExpression(Symbols.NotDefinedSign, x, a, b);
            }
          }
          return linearExpression1;
        case PowerExpression _:
          PowerExpression powerExpression = (PowerExpression) expr;
          if (powerExpression.OperationCount == 1 && LiteralExpression.IsUnit(powerExpression.Operands[1]))
            linearExpression1 = LinearExpression.Extract(powerExpression.Operands[0], vName);
          return linearExpression1;
        case ProductExpression _:
          ProductExpression productExpression = (ProductExpression) expr;
          int index1 = -1;
          int operationCount1 = productExpression.OperationCount;
          for (int index2 = 0; index2 <= operationCount1; ++index2)
          {
            if (productExpression.Operands[index2].DependsOn(vName))
            {
              if (index1 >= 0)
                return linearExpression1;
              index1 = index2;
            }
          }
          if (index1 >= 0 && (index1 <= 0 || Utilities.GetOperatorType(productExpression.Operators[index1 - 1], OperatorArity.Binary) == OperatorType.Multiply))
          {
            LinearExpression linearExpression3 = LinearExpression.Extract(productExpression.Operands[index1], vName);
            if (linearExpression3 != null)
            {
              List<string> theOperators = new List<string>();
              List<BaseExpression> theOperands = new List<BaseExpression>();
              for (int index3 = 0; index3 <= operationCount1; ++index3)
              {
                if (index3 != index1)
                {
                  BaseExpression baseExpression = productExpression.Operands[index3].Copy(false);
                  theOperands.Add(baseExpression);
                  if (index3 > 0)
                    theOperators.Add(productExpression.Operators[index3 - 1]);
                }
              }
              if (theOperands.Count == theOperators.Count)
              {
                if (ProductExpression.IsInverseOperator(theOperators[0]))
                  theOperands.Insert(0, (BaseExpression) LiteralExpression.Unit);
                else
                  theOperators.RemoveAt(0);
              }
              BaseExpression x1;
              if (theOperands.Count == 1)
                x1 = theOperands[0];
              else
                x1 = (BaseExpression) new ProductExpression(Symbols.NotDefinedSign, theOperators, theOperands);
              BaseExpression b = (BaseExpression) null;
              if (linearExpression3.B != null)
                b = ProductExpression.MakeProduct(x1.Copy(false), linearExpression3.B.Copy(false));
              BaseExpression a = ProductExpression.MakeProduct(x1, linearExpression3.A.Copy(false));
              VariableExpression x = new VariableExpression(vName);
              linearExpression1 = new LinearExpression(Symbols.NotDefinedSign, x, a, b);
            }
          }
          return linearExpression1;
        case SumExpression _:
          SumExpression sumExpression = (SumExpression) expr;
          int operationCount2 = sumExpression.OperationCount;
          List<string> theOperators1 = new List<string>();
          List<BaseExpression> theOperands1 = new List<BaseExpression>();
          List<string> theOperators2 = new List<string>();
          List<BaseExpression> theOperands2 = new List<BaseExpression>();
          for (int index4 = 0; index4 <= operationCount2; ++index4)
          {
            BaseExpression operand = sumExpression.Operands[index4];
            if (operand.DependsOn(vName))
            {
              LinearExpression linearExpression4 = LinearExpression.Extract(operand, vName);
              if (linearExpression4 == null)
                return linearExpression1;
              if (linearExpression4.A != null)
              {
                theOperands1.Add(linearExpression4.A.Copy(false));
                if (index4 > 0)
                  theOperators1.Add(sumExpression.Operators[index4 - 1]);
              }
              if (linearExpression4.B != null)
              {
                theOperands2.Add(linearExpression4.B.Copy(false));
                if (index4 > 0)
                  theOperators2.Add(sumExpression.Operators[index4 - 1]);
              }
            }
            else
            {
              BaseExpression baseExpression = operand.Copy(false);
              theOperands2.Add(baseExpression);
              if (index4 > 0)
                theOperators2.Add(sumExpression.Operators[index4 - 1]);
            }
          }
          if (theOperands1.Count == theOperators1.Count)
          {
            if (SumExpression.IsInverseOperator(theOperators1[0]))
              theOperands1[0] = UnaryOperatorExpression.Negate(theOperands1[0]);
            theOperators1.RemoveAt(0);
          }
          if (theOperands2.Count > 0 && theOperands2.Count == theOperators2.Count)
          {
            if (SumExpression.IsInverseOperator(theOperators2[0]))
              theOperands2[0] = UnaryOperatorExpression.Negate(theOperands2[0]);
            theOperators2.RemoveAt(0);
          }
          BaseExpression a1 = theOperands1.Count <= 1 ? theOperands1[0] : (BaseExpression) new SumExpression(Symbols.NotDefinedSign, theOperators1, theOperands1);
          BaseExpression b1 = (BaseExpression) null;
          if (theOperands2.Count > 0)
            b1 = theOperands2.Count <= 1 ? theOperands2[0] : (BaseExpression) new SumExpression(Symbols.NotDefinedSign, theOperators2, theOperands2);
          VariableExpression x2 = new VariableExpression(vName);
          return new LinearExpression(Symbols.NotDefinedSign, x2, a1, b1);
        default:
          return linearExpression1;
      }
    }
  }
}
