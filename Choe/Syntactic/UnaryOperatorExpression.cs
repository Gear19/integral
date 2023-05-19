using System;
using System.Collections.Generic;

namespace Choe.Syntactic
{
  public class UnaryOperatorExpression : StructuredExpression
  {
    protected string sign;
    protected OperatorPosition position;
    protected BaseExpression operand;

    protected override ExpressionContext GetInternalContext()
    {
      ExpressionContext internalContext = (ExpressionContext) null;
      if (this.operand != null)
        internalContext = this.operand.Context;
      return internalContext;
    }

    protected override void SetInternalContext(ExpressionContext ctxt)
    {
      if (this.operand == null)
        return;
      this.operand.Context = ctxt;
    }

    protected override int GetSelfPrecedence() => Utilities.GetOperatorPrecedence(this.Operator);

    protected override void Degenerate() => this.sign = string.Empty;

    protected override int ReplaceInside(string vName, BaseExpression expr)
    {
      List<BaseExpression> expressions = new List<BaseExpression>()
      {
        this.operand
      };
      int num = StructuredExpression.ReplaceInList(expressions, vName, expr);
      if (num <= 0)
        return num;
      this.operand = expressions[0];
      return num;
    }

    protected override BaseExpression GetExplicit(bool simplified)
    {
      BaseExpression anOperand = BaseExpression.ExplicitExpression(this.operand, simplified);
      return (BaseExpression) new UnaryOperatorExpression(Symbols.NotDefinedSign, this.Sign, this.Position, anOperand);
    }

    protected virtual void MergeDegenerated()
    {
      BaseExpression operand = this.operand;
      if (!(operand is StructuredExpression) || !(operand as StructuredExpression).IsDegenerated)
        return;
      BaseExpression degenerated = (operand as StructuredExpression).Degenerated;
      if (degenerated == null)
        return;
      this.operand = degenerated;
    }

    public UnaryOperatorExpression(
      string aValue,
      string aSign,
      OperatorPosition pos,
      BaseExpression anOperand)
      : base(aValue)
    {
      this.sign = aSign;
      this.position = pos;
      this.operand = anOperand;
      this._context = this.GetInternalContext();
    }

    public string Sign => this.sign;

    public OperatorType Operator => this.IsDegenerated ? OperatorType.Unknown : Utilities.GetOperatorType(this.Sign, OperatorArity.Unary);

    public OperatorPosition Position => this.IsDegenerated ? OperatorPosition.Unknown : this.position;

    public BaseExpression Operand => this.operand;

    public override bool IsDegenerated => string.IsNullOrEmpty(this.sign);

    public override BaseExpression Degenerated => this.operand;

    public virtual bool MustEnclose(BaseExpression opnd)
    {
      bool flag = true;
      if (this.position == OperatorPosition.Outfix)
        flag = false;
      else if (opnd is VariableExpression || opnd is FunctionExpression)
      {
        flag = false;
      }
      else
      {
        if (opnd is StructuredExpression)
        {
          StructuredExpression structuredExpression = opnd as StructuredExpression;
          if (structuredExpression.IsDegenerated)
            return this.MustEnclose(structuredExpression.Degenerated);
        }
        if (opnd is UnaryOperatorExpression)
        {
          UnaryOperatorExpression operatorExpression = opnd as UnaryOperatorExpression;
          if (this.Operator == OperatorType.Minus && operatorExpression.Operator == OperatorType.Minus)
            flag = true;
          else if (operatorExpression.Position != OperatorPosition.Prefix || this.Position != OperatorPosition.Postfix)
            flag = false;
        }
        else if (opnd is LiteralExpression)
        {
          LiteralExpression literalExpression = opnd as LiteralExpression;
          flag = !Parser.IsReal(literalExpression.Value) || (int) literalExpression.Value[0] == (int) Symbols.MinusOperator;
        }
        else if (opnd is BinaryOperationsExpression)
        {
          flag = true;
        }
        else
        {
          if (!(opnd is ArrayExpression))
            return false;
          flag = false;
        }
      }
      return flag;
    }

    public override string Reconstruct()
    {
      if (this.IsDegenerated)
        return this.operand.Reconstruct();
      bool enclose = this.MustEnclose(this.operand);
      return Builder.BuildUnaryOperation(this.sign, this.operand.Reconstruct(), this.position, enclose);
    }

    public override bool IsConstant() => this.operand == null || this.operand.IsConstant();

    public override bool DependsOn(string vName) => this.operand != null && this.operand.DependsOn(vName);

    protected override BaseExpression SelfIntegral(string vName)
    {
      BaseExpression baseExpression = (BaseExpression) null;
      switch (Utilities.GetOperatorType(this.Sign, OperatorArity.Unary))
      {
        case OperatorType.Question:
        case OperatorType.Number:
        case OperatorType.Tilde:
        case OperatorType.Apostrophe:
        case OperatorType.Accent:
        case OperatorType.Norm:
          this.Reconstruct();
          return null;
        case OperatorType.Minus:
          BaseExpression anOperand1 = this.operand.Integrate(vName);
          if (anOperand1 != null)
          {
            baseExpression = (BaseExpression) new UnaryOperatorExpression(Symbols.NotDefinedSign, this.Sign, this.Position, anOperand1);
            break;
          }
          break;
        case OperatorType.SquareRoot:
          if (VariableExpression.IsVariable(this.operand, vName))
          {
            baseExpression = UnaryOperatorExpression.SqrtIntegral((BaseExpression) new VariableExpression(vName));
            break;
          }
          LinearExpression linearExpression = LinearExpression.Extract(this.operand, vName);
          if (linearExpression != null)
          {
            baseExpression = ProductExpression.MakeDivision(UnaryOperatorExpression.SqrtIntegral(this.operand.Copy(false)), linearExpression.A.Copy(false));
            break;
          }
          break;
        case OperatorType.Integral:
        case OperatorType.Delta:
        case OperatorType.Sum:
          BaseExpression anOperand2 = this.operand.Integrate(vName);
          if (anOperand2 != null)
          {
            baseExpression = (BaseExpression) new UnaryOperatorExpression(Symbols.NotDefinedSign, this.Sign, this.Position, anOperand2);
            break;
          }
          break;
        case OperatorType.Factorial:
          this.Reconstruct();
          return null;
        case OperatorType.Absolute:
          BaseExpression x2 = this.operand.Integrate(vName);
          if (x2 != null)
          {
            baseExpression = ProductExpression.MakeProduct((BaseExpression) FunctionExpression.CreateSimple(Names.Signum, this.operand.Copy(false)), x2);
            break;
          }
          break;
      }
      return baseExpression;
    }

    public static BaseExpression SqrtIntegral(BaseExpression x)
    {
      BaseExpression x2 = PowerExpression.MakePower(x, ProductExpression.MakeDivision((BaseExpression) LiteralExpression.Make(3L), (BaseExpression) LiteralExpression.Make(2L)));
      return ProductExpression.MakeProduct(ProductExpression.MakeDivision((BaseExpression) LiteralExpression.Make(2L), (BaseExpression) LiteralExpression.Make(3L)), x2);
    }

    public static BaseExpression Negate(BaseExpression expr)
    {
      if (LiteralExpression.IsZero(expr))
        return (BaseExpression) LiteralExpression.Zero;
      if (expr is UnaryOperatorExpression operatorExpression && operatorExpression.Operator == OperatorType.Minus)
        return operatorExpression.Operand;
      double x = 0.0;
      return LiteralExpression.IsRealValue(expr, ref x) ? (BaseExpression) LiteralExpression.Make(-x) : (BaseExpression) new UnaryOperatorExpression(Symbols.NotDefinedSign, Utilities.GetOperatorSign(OperatorType.Minus), Utilities.GetOperatorPosition(OperatorType.Minus), expr);
    }

    public static BaseExpression Sqrt(BaseExpression expr)
    {
      if (LiteralExpression.IsZero(expr))
        return (BaseExpression) LiteralExpression.Zero;
      if (LiteralExpression.IsUnit(expr))
        return (BaseExpression) LiteralExpression.Unit;
      if (LiteralExpression.IsInfinity(expr))
        return (BaseExpression) LiteralExpression.Infinity;
      return LiteralExpression.IsNaN(expr) ? (BaseExpression) LiteralExpression.NaN : (BaseExpression) new UnaryOperatorExpression(Symbols.NotDefinedSign, Utilities.GetOperatorSign(OperatorType.SquareRoot), Utilities.GetOperatorPosition(OperatorType.SquareRoot), expr);
    }

    public static BaseExpression Factorial(BaseExpression expr)
    {
      if (LiteralExpression.IsZero(expr))
        return (BaseExpression) LiteralExpression.Unit;
      if (LiteralExpression.IsUnit(expr))
        return (BaseExpression) LiteralExpression.Unit;
      if (LiteralExpression.IsInfinity(expr))
        return (BaseExpression) LiteralExpression.Infinity;
      return LiteralExpression.IsNaN(expr) ? (BaseExpression) LiteralExpression.NaN : (BaseExpression) new UnaryOperatorExpression(Symbols.NotDefinedSign, Utilities.GetOperatorSign(OperatorType.Factorial), Utilities.GetOperatorPosition(OperatorType.Factorial), expr);
    }

    public static BaseExpression Abs(BaseExpression expr)
    {
      if (LiteralExpression.IsZero(expr))
        return (BaseExpression) LiteralExpression.Zero;
      if (LiteralExpression.IsUnit(expr))
        return (BaseExpression) LiteralExpression.Unit;
      if (LiteralExpression.IsInfinity(expr))
        return (BaseExpression) LiteralExpression.Infinity;
      return LiteralExpression.IsNaN(expr) ? (BaseExpression) LiteralExpression.NaN : (BaseExpression) new UnaryOperatorExpression(Symbols.NotDefinedSign, Utilities.GetOperatorSign(OperatorType.Absolute), Utilities.GetOperatorPosition(OperatorType.Absolute), expr);
    }

    public static bool IsNegation(BaseExpression expr)
    {
      if (!(expr is UnaryOperatorExpression operatorExpression))
        return false;
      if (operatorExpression.IsDegenerated)
        return UnaryOperatorExpression.IsNegation(operatorExpression.Operand);
      return operatorExpression.Operator == OperatorType.Minus && LiteralExpression.IsUnit(operatorExpression.Operand);
    }

    public override void Simplify()
    {
      if (this.operand == null)
        return;
      this.operand.Simplify();
      this.MergeDegenerated();
      if (this.IsDegenerated)
        return;
      switch (this.Operator)
      {
        case OperatorType.Minus:
          if (LiteralExpression.IsZero(this.operand))
            this.Degenerate();
          if (UnaryOperatorExpression.IsUnary(this.operand, OperatorType.Minus))
          {
            UnaryOperatorExpression operand = this.operand as UnaryOperatorExpression;
            if (operand.IsDegenerated)
              break;
            this.operand = operand.Operand;
            this.Degenerate();
            break;
          }
          if (!(this.operand is BinaryOperationsExpression))
            break;
          BinaryOperationsExpression operand1 = this.operand as BinaryOperationsExpression;
          switch (operand1)
          {
            case SumExpression _:
              SumExpression sumExpression = operand1 as SumExpression;
              sumExpression.Negate();
              sumExpression.Simplify();
              this.Degenerate();
              return;
            case ProductExpression _:
              ProductExpression productExpression = operand1 as ProductExpression;
              productExpression.Negate();
              productExpression.Simplify();
              this.Degenerate();
              return;
            default:
              return;
          }
        case OperatorType.Tilde:
        case OperatorType.Apostrophe:
        case OperatorType.Accent:
          if (this.IsDegenerated || !UnaryOperatorExpression.IsUnary(this.operand, this.Operator))
            break;
          UnaryOperatorExpression operatorExpression = (UnaryOperatorExpression) null;
          if (this.operand is UnaryOperatorExpression)
            operatorExpression = this.operand as UnaryOperatorExpression;
          if (operatorExpression == null)
            break;
          this.operand = operatorExpression.Operand;
          this.Degenerate();
          break;
        case OperatorType.SquareRoot:
          if (LiteralExpression.IsZero(this.operand) || LiteralExpression.IsUnit(this.operand) || LiteralExpression.IsInfinity(this.operand) || LiteralExpression.IsNaN(this.operand))
          {
            this.Degenerate();
            break;
          }
          if (this.operand is PowerExpression && !(this.operand as PowerExpression).IsDegenerated)
          {
            BinaryOperationsExpression operand2 = this.operand as BinaryOperationsExpression;
            if (operand2.OperationCount != 1)
              break;
            BaseExpression baseExpression = ProductExpression.MakeProduct(operand2.Operands[1], ProductExpression.OneHalf);
            baseExpression.Simplify();
            operand2.Operands[1] = baseExpression;
            this.Degenerate();
            operand2.Simplify();
            break;
          }
          double x1 = 0.0;
          if (!LiteralExpression.IsRealValue(this.operand, ref x1))
            break;
          long ivalue = 0;
          if (x1 <= 0.0 || !Utilities.IsInteger(x1, out ivalue) || !Utilities.IsInteger(Math.Sqrt(x1), out ivalue))
            break;
          this.operand = (BaseExpression) LiteralExpression.Make(ivalue);
          this.Degenerate();
          break;
        case OperatorType.Factorial:
          if (LiteralExpression.IsZero(this.operand) || LiteralExpression.IsUnit(this.operand))
          {
            this.operand = (BaseExpression) LiteralExpression.Unit;
            this.Degenerate();
            break;
          }
          if (!LiteralExpression.IsInfinity(this.operand) && !LiteralExpression.IsNaN(this.operand))
            break;
          this.Degenerate();
          break;
        case OperatorType.Absolute:
          if (LiteralExpression.IsZero(this.operand) || LiteralExpression.IsUnit(this.operand) || LiteralExpression.IsInfinity(this.operand) || LiteralExpression.IsNaN(this.operand))
          {
            this.Degenerate();
            break;
          }
          if (UnaryOperatorExpression.IsUnary(this.operand, OperatorType.Minus))
          {
            if (!(this.operand is UnaryOperatorExpression operand3) || operand3.IsDegenerated)
              break;
            this.operand = operand3.Operand;
            break;
          }
          double x2 = 0.0;
          if (!LiteralExpression.IsRealValue(this.operand, ref x2))
            break;
          this.operand = (BaseExpression) LiteralExpression.Make(Math.Abs(x2));
          this.Degenerate();
          break;
      }
    }

    public override string ToString() => "Unary operation: " + this.Sign + " Position: " + this.Position.ToString() + " Operand: " + this.operand.Reconstruct();

    public override string Print(int level)
    {
      string str = new string(' ', level * 2);
      return str + "Operator: " + this.Sign + " Position: " + this.Position.ToString() + Environment.NewLine + str + "Operand: " + Environment.NewLine + this.operand.Print(level + 1);
    }

    public static bool IsUnary(BaseExpression expr, OperatorType t) => expr is UnaryOperatorExpression operatorExpression && !operatorExpression.IsDegenerated && operatorExpression.Operator == t;

    public static bool IsUnary(BaseExpression expr, OperatorType t, string vName)
    {
      bool flag = UnaryOperatorExpression.IsUnary(expr, t);
      if (flag && vName != string.Empty)
        flag = VariableExpression.IsVariable(((UnaryOperatorExpression) expr.This).Operand, vName);
      return flag;
    }
  }
}
