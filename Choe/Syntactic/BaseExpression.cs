using Choe.Integrals;
using System;
using System.Collections.Generic;

namespace Choe.Syntactic
{
  public abstract class BaseExpression
  {
    protected ExpressionContext _context;

    protected string value;

    protected abstract ExpressionContext GetInternalContext();

    protected abstract void SetInternalContext(ExpressionContext ctxt);

    protected virtual ExpressionContext GetContext()
    {
      if (this._context != null)
        return this._context;
      this._context = this.GetInternalContext();
      return this._context;
    }

    protected virtual void SetContext(ExpressionContext ctxt)
    {
      this._context = ctxt;
      this.SetInternalContext(ctxt);
    }

    protected virtual BaseExpression GetThis() => this;

    protected virtual BaseExpression SelfIntegral(string vName) => (BaseExpression) null;

    protected virtual string GetValue() => this.value == Symbols.NotDefinedSign ? this.Reconstruct() : this.value;

    protected abstract int GetPrecedence();

    public BaseExpression(string aValue) => this.value = aValue;

    public ExpressionContext Context
    {
      get => this.GetContext();
      set => this.SetContext(value);
    }

    public BaseExpression This => this.GetThis();

    public string PrimaryValue => this.value;

    public int Precedence => this.GetPrecedence();

    public abstract string Reconstruct();

    public abstract bool IsConstant();

    public abstract bool DependsOn(string vName);

    public virtual BaseExpression Integrate(string vName)
    {
      BaseExpression expr = this.This;
      if (!expr.DependsOn(vName))
      {
        BaseExpression baseExpression = ProductExpression.MakeProduct(expr.Copy(false), (BaseExpression) new VariableExpression(vName));
        baseExpression.Context = this.Context;
        return baseExpression;
      }
      BaseExpression baseExpression1 = expr.SelfIntegral(vName);
      if (baseExpression1 == null)
      {
        if(this.Context == null || this.Context.Integrals == null)
        {
             this.Reconstruct();
             return null;
								}
        IntegralContext integralContext = this.Context.Integrals;
        if (integralContext != null)
        {
          int integratorCount = integralContext.IntegratorCount;
          if (integratorCount > 0)
          {
            for (int index1 = 1; index1 <= 3; ++index1)
            {
              Type c = (Type) null;
              switch (index1)
              {
                case 1:
                  c = typeof (TemplateIntegrator);
                  break;
                case 2:
                  c = typeof (SimplifyIntegrator);
                  break;
                case 3:
                  c = typeof (AlgorithmIntegrator);
                  break;
              }
              for (int index2 = 0; index2 < integratorCount; ++index2)
              {
                Integrator integrator = integralContext[index2];
                if (integrator.GetType().IsSubclassOf(c))
                {
                  baseExpression1 = integrator.Integral(expr, vName);
                  if (baseExpression1 != null)
                  {
                    if (baseExpression1.Context == null)
                      baseExpression1.Context = this.Context;
                    return baseExpression1;
                  }
                }
              }
            }
          }
        }
      }
      return baseExpression1;
    }

  public virtual BaseExpression Integral(string vName) 
  {
   if (this.Integrate(vName) == null) { this.Reconstruct(); return null; }
   else return this.Integrate(vName);
  }

    public virtual void Simplify()
    {
    }

    public virtual BaseExpression Copy(bool simplified)
    {
      BaseExpression baseExpression = BaseExpression.Build(this.Context, this.Reconstruct());
      if (simplified)
        baseExpression.Simplify();
      return baseExpression;
    }

    public abstract string Print(int level);

    public static BaseExpression CreateBinaryExpression(
      string x,
      BinaryOperationType bt,
      List<string> operators,
      List<BaseExpression> operands)
    {
      switch (bt)
      {
        case BinaryOperationType.Relational:
          return (BaseExpression) new RelationalExpression(x, operators, operands);
        case BinaryOperationType.Sum:
          return (BaseExpression) new SumExpression(x, operators, operands);
        case BinaryOperationType.Product:
          return (BaseExpression) new ProductExpression(x, operators, operands);
        case BinaryOperationType.Power:
          return (BaseExpression) new PowerExpression(x, operators, operands);
        default:
          return null;
      }
    }

    public static BaseExpression Build(ExpressionContext context, string value)
    {
      if (string.IsNullOrEmpty(value))
        return (BaseExpression) null;
      string str = Parser.RemoveSurrounders(value, Elements.FunctionBrackets, Elements.Brackets);
      BaseExpression baseExpression;
      if (Parser.IsLiteral(str)) 
      { 
        baseExpression = (BaseExpression) new LiteralExpression(str);
        //Console.WriteLine("literal " + baseExpression.value);
      }
      else if (Parser.IsValidName(str))
      {
        baseExpression = (BaseExpression) new VariableExpression(str);
        //Console.WriteLine("variable " + baseExpression.value);
      }
      else
      {
        BinaryOperationType bt = BinaryOperationType.Unknown;
        CheckedData<StringOperators> checkedData1 = Parser.IsRelationalExpression(str);
        checkedData1 = Parser.IsRelationalExpression(str);
        if (checkedData1.Is)
        {
          bt = BinaryOperationType.Relational;
        }
        else
        {
          checkedData1 = Parser.IsSummExpression(str);
          if (checkedData1.Is)
          {
            bt = BinaryOperationType.Sum;
          }
          else
          {
            checkedData1 = Parser.IsProductExpression(str);
            if (checkedData1.Is)
            {
              bt = BinaryOperationType.Product;
            }
            else
            {
              checkedData1 = Parser.IsPowerExpression(str);
              if (checkedData1.Is)
              {
                bt = BinaryOperationType.Power;
              }
            }
          }
        }
        if (bt != BinaryOperationType.Unknown)
        {
          List<string> operands1 = checkedData1.Value.Operands;
          List<string> operators = checkedData1.Value.Operators;
          List<BaseExpression> operands2 = BaseExpression.BuildList(context, operands1);
          int count = operands2.Count;
          for (int index = 0; index < count; ++index)
          {
            if (operands2[index] == null)
              return null;
          }
          baseExpression = BaseExpression.CreateBinaryExpression(value, bt, operators, operands2);
          //Console.WriteLine("Binary Expr " + baseExpression.value);
        }
        else
        {
          OperatorPosition pos1 = OperatorPosition.Unknown;
          CheckedData<OperatorData> checkedData2 = Parser.IsPrefixExpression(str);
          if (checkedData2.Is)
          {
            pos1 = OperatorPosition.Prefix;
          }
          else
          {
            checkedData2 = Parser.IsPostfixExpression(str);
            if (checkedData2.Is)
              pos1 = OperatorPosition.Postfix;
          }
          if (checkedData2.Is)
          {//return null;
            var buildExpr = BaseExpression.Build(context, checkedData2.Value.Operand);
            if(buildExpr == null) return null;
            else
            baseExpression = (BaseExpression) new UnaryOperatorExpression(value, checkedData2.Value.Operator, pos1, buildExpr);
            //Console.WriteLine("Unary Operator Expr postfix" + baseExpression.value);
          }
          else
          {
            CheckedData<OperatorData> checkedData3 = Parser.IsOutfixExpression(str);
            if (checkedData3.Is)
            {
              BaseExpression anOperand = BaseExpression.Build(context, checkedData3.Value.Operand);
              if (anOperand == null)
                return null;
              OperatorPosition pos2 = OperatorPosition.Outfix;
              baseExpression = (BaseExpression) new UnaryOperatorExpression(value, checkedData3.Value.Operator, pos2, anOperand);
              //Console.WriteLine("Unary Operator Expr outfix " + baseExpression.value);
            }
            else
            {
              CheckedData<FunctionData> checkedData4 = Parser.IsFunction(str);
              if (!checkedData4.Is)
                return null;
              if (!Parser.IsValidName(checkedData4.Value.Name))
                return null;
              List<BaseExpression> theParameters = BaseExpression.BuildList(context, checkedData4.Value.Parameters);
              List<BaseExpression> theArguments = BaseExpression.BuildList(context, checkedData4.Value.Arguments);
              baseExpression = (BaseExpression) new FunctionExpression(value, checkedData4.Value.Name, theParameters, theArguments);
              //Console.WriteLine("function " + baseExpression.value);
            }
          }
        }
      }
      if (baseExpression != null && baseExpression.Context == null)
        baseExpression.Context = context;
      return baseExpression;
    }

    public static BaseExpression ExplicitExpression(BaseExpression expr, bool simplified)
    {
      if (expr == null)
        return (BaseExpression) null;
      return expr is SimpleExpression ? expr.Copy(simplified) : (expr as StructuredExpression).CopyExplicit(simplified);
    }

    public static List<BaseExpression> BuildList(ExpressionContext context, List<string> values)
    {
      List<BaseExpression> baseExpressionList = (List<BaseExpression>) null;
      if (values != null)
      {
        baseExpressionList = new List<BaseExpression>();
        for (int index = 0; index < values.Count; ++index)
        {
          BaseExpression baseExpression = BaseExpression.Build(context, values[index]);
          baseExpressionList.Add(baseExpression);
        }
      }
      return baseExpressionList;
    }

    public static BaseExpression[] BuildArray(ExpressionContext context, string[] values)
    {
      BaseExpression[] baseExpressionArray = (BaseExpression[]) null;
      if (values != null)
      {
        int length = values.Length;
        baseExpressionArray = new BaseExpression[length];
        for (int index = 0; index < length; ++index)
          baseExpressionArray[index] = BaseExpression.Build(context, values[index]);
      }
      return baseExpressionArray;
    }

    public static void SimplifyList(List<BaseExpression> expressions, bool mergeDegenerated)
    {
      if (expressions == null || expressions.Count == 0)
        return;
      int count = expressions.Count;
      for (int index = 0; index < count; ++index)
        expressions[index]?.Simplify();
      if (!mergeDegenerated)
        return;
      StructuredExpression.MergeDegenerated(expressions);
    }

    public static List<string> GetPrimaries(List<BaseExpression> expressions)
    {
      List<string> primaries = (List<string>) null;
      if (expressions != null)
      {
        primaries = new List<string>();
        for (int index = 0; index < expressions.Count; ++index)
        {
          string str = (string) null;
          if (expressions[index] != null)
            str = expressions[index].PrimaryValue;
          primaries.Add(str);
        }
      }
      return primaries;
    }

    public static List<string> GetReconstructed(List<BaseExpression> expressions)
    {
      List<string> reconstructed = (List<string>) null;
      if (expressions != null)
      {
        reconstructed = new List<string>();
        for (int index = 0; index < expressions.Count; ++index)
        {
          string str = (string) null;
          if (expressions[index] != null)
            str = expressions[index].Reconstruct();
          reconstructed.Add(str);
        }
      }
      return reconstructed;
    }

    public static bool IsDependant(List<BaseExpression> expressions, string vName)
    {
      bool flag = false;
      if (expressions != null)
      {
        int count = expressions.Count;
        for (int index = 0; index < count; ++index)
        {
          BaseExpression expression = expressions[index];
          if (expression != null && expression.DependsOn(vName))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public static List<BaseExpression> Integrals(List<BaseExpression> expressions, string vName)
    {
      List<BaseExpression> baseExpressionList = (List<BaseExpression>) null;
      if (expressions != null)
      {
        baseExpressionList = new List<BaseExpression>();
        int count = expressions.Count;
        for (int index = 0; index < count; ++index)
        {
          BaseExpression baseExpression = expressions[index].Integrate(vName);
          if (baseExpression == null)
            return (List<BaseExpression>) null;
          baseExpressionList.Add(baseExpression);
        }
      }
      return baseExpressionList;
    }

    public static bool AreConstant(List<BaseExpression> expressions)
    {
      if (expressions == null || expressions.Count == 0)
        return true;
      int count = expressions.Count;
      for (int index = 0; index < count; ++index)
      {
        if (!expressions[index].IsConstant())
          return false;
      }
      return true;
    }

    public static int RemoveSame(
      List<BaseExpression> e1,
      List<BaseExpression> e2,
      List<string> o1,
      List<string> o2)
    {
      int num = 0;
      if (e1 != null && e2 != null)
      {
        List<string> reconstructed1 = BaseExpression.GetReconstructed(e1);
        List<string> reconstructed2 = BaseExpression.GetReconstructed(e2);
        for (int index1 = e2.Count - 1; index1 >= 0; --index1)
        {
          int index2 = -1;
          int index3 = -1;
          string str = reconstructed2[index1];
          for (int index4 = e1.Count - 1; index4 >= 0; --index4)
          {
            if (reconstructed1[index4] == str)
            {
              index2 = index4;
              index3 = index1;
              break;
            }
          }
          if (index3 >= 0)
          {
            e1.RemoveAt(index2);
            reconstructed1.RemoveAt(index2);
            if (index2 > 0)
              o1.RemoveAt(index2 - 1);
            else if (o1.Count > 0)
              o1.RemoveAt(0);
            e2.RemoveAt(index3);
            reconstructed2.RemoveAt(index3);
            if (index3 > 0)
              o2.RemoveAt(index3 - 1);
            else if (o2.Count > 0)
              o2.RemoveAt(0);
            ++num;
          }
        }
      }
      return num;
    }

    public static int FindCount(List<BaseExpression> expressions, BaseExpression item)
    {
      int count1 = 0;
      if (expressions != null && item != null)
      {
        string str = item.Reconstruct();
        List<string> reconstructed = BaseExpression.GetReconstructed(expressions);
        int count2 = reconstructed.Count;
        for (int index = 0; index < count2; ++index)
        {
          if (reconstructed[index] == str)
            ++count1;
        }
      }
      return count1;
    }

    public static ExpressionContext GetDataContext(BaseExpression[] x)
    {
      ExpressionContext dataContext = (ExpressionContext) null;
      int num = Utilities.SafeArrayLength((Array) x);
      for (int index = 0; index < num; ++index)
      {
        if (x[index] != null)
        {
          ExpressionContext context = x[index].Context;
          if (context != null)
          {
            dataContext = context;
            break;
          }
        }
      }
      return dataContext;
    }

    public static void SetDataContext(BaseExpression[] x, ExpressionContext ctx)
    {
      int num = Utilities.SafeArrayLength((Array) x);
      for (int index = 0; index < num; ++index)
      {
        if (x[index] != null)
          x[index].Context = ctx;
      }
    }

    public static BaseExpression operator -(BaseExpression expr) => UnaryOperatorExpression.Negate(expr);

    public static BaseExpression operator +(BaseExpression expr1, BaseExpression expr2) => SumExpression.MakeSum(expr1, expr2);

    public static BaseExpression operator +(BaseExpression expr1, double value2)
    {
      BaseExpression x2 = (BaseExpression) LiteralExpression.Make(value2);
      return SumExpression.MakeSum(expr1, x2);
    }

    public static BaseExpression operator +(double value1, BaseExpression expr2) => SumExpression.MakeSum((BaseExpression) LiteralExpression.Make(value1), expr2);

    public static BaseExpression operator +(BaseExpression expr1, long value2)
    {
      BaseExpression x2 = (BaseExpression) LiteralExpression.Make(value2);
      return SumExpression.MakeSum(expr1, x2);
    }

    public static BaseExpression operator +(long value1, BaseExpression expr2) => SumExpression.MakeSum((BaseExpression) LiteralExpression.Make(value1), expr2);

    public static BaseExpression operator -(BaseExpression expr1, BaseExpression expr2) => SumExpression.MakeDifference(expr1, expr2);

    public static BaseExpression operator -(BaseExpression expr1, double value2)
    {
      BaseExpression x2 = (BaseExpression) LiteralExpression.Make(value2);
      return SumExpression.MakeDifference(expr1, x2);
    }

    public static BaseExpression operator -(double value1, BaseExpression expr2) => SumExpression.MakeDifference((BaseExpression) LiteralExpression.Make(value1), expr2);

    public static BaseExpression operator -(BaseExpression expr1, long value2)
    {
      BaseExpression x2 = (BaseExpression) LiteralExpression.Make(value2);
      return SumExpression.MakeDifference(expr1, x2);
    }

    public static BaseExpression operator -(long value1, BaseExpression expr2) => SumExpression.MakeDifference((BaseExpression) LiteralExpression.Make(value1), expr2);

    public static BaseExpression operator *(BaseExpression expr1, BaseExpression expr2) => ProductExpression.MakeProduct(expr1, expr2);

    public static BaseExpression operator *(BaseExpression expr1, double value2)
    {
      BaseExpression x2 = (BaseExpression) LiteralExpression.Make(value2);
      return ProductExpression.MakeProduct(expr1, x2);
    }

    public static BaseExpression operator *(double value1, BaseExpression expr2) => ProductExpression.MakeProduct((BaseExpression) LiteralExpression.Make(value1), expr2);

    public static BaseExpression operator *(BaseExpression expr1, long value2)
    {
      BaseExpression x2 = (BaseExpression) LiteralExpression.Make(value2);
      return ProductExpression.MakeProduct(expr1, x2);
    }

    public static BaseExpression operator *(long value1, BaseExpression expr2) => ProductExpression.MakeProduct((BaseExpression) LiteralExpression.Make(value1), expr2);

    public static BaseExpression operator /(BaseExpression expr1, BaseExpression expr2) => ProductExpression.MakeDivision(expr1, expr2);

    public static BaseExpression operator /(BaseExpression expr1, double value2)
    {
      BaseExpression x2 = (BaseExpression) LiteralExpression.Make(value2);
      return ProductExpression.MakeDivision(expr1, x2);
    }

    public static BaseExpression operator /(double value1, BaseExpression expr2) => ProductExpression.MakeDivision((BaseExpression) LiteralExpression.Make(value1), expr2);

    public static BaseExpression operator /(BaseExpression expr1, long value2)
    {
      BaseExpression x2 = (BaseExpression) LiteralExpression.Make(value2);
      return ProductExpression.MakeDivision(expr1, x2);
    }

    public static BaseExpression operator /(long value1, BaseExpression expr2) => ProductExpression.MakeDivision((BaseExpression) LiteralExpression.Make(value1), expr2);

    public static BaseExpression operator ^(BaseExpression expr1, BaseExpression expr2) => PowerExpression.MakePower(expr1, expr2);

    public static BaseExpression operator ^(BaseExpression expr1, double value2)
    {
      BaseExpression y = (BaseExpression) LiteralExpression.Make(value2);
      return PowerExpression.MakePower(expr1, y);
    }

    public static BaseExpression operator ^(double value1, BaseExpression expr2) => PowerExpression.MakePower((BaseExpression) LiteralExpression.Make(value1), expr2);

    public static BaseExpression operator ^(BaseExpression expr1, long value2)
    {
      BaseExpression y = (BaseExpression) LiteralExpression.Make(value2);
      return PowerExpression.MakePower(expr1, y);
    }

    public static BaseExpression operator ^(long value1, BaseExpression expr2) => PowerExpression.MakePower((BaseExpression) LiteralExpression.Make(value1), expr2);
  }
}
