using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Choe.Syntactic
{
  public abstract class BinaryOperationsExpression : StructuredExpression
  {
    protected List<string> operators;
    protected List<BaseExpression> operands;

    protected override ExpressionContext GetInternalContext()
    {
      ExpressionContext internalContext = (ExpressionContext) null;
      if (this.operands != null)
        internalContext = BaseExpression.GetDataContext(this.operands.ToArray());
      return internalContext;
    }

    protected override void SetInternalContext(ExpressionContext ctxt)
    {
      if (this.operands == null)
        return;
      BaseExpression.SetDataContext(this.operands.ToArray(), ctxt);
    }

    protected override int GetSelfPrecedence() => Utilities.GetOperatorPrecedence(Utilities.GetOperatorType(this.operators[0], OperatorArity.Binary));

    protected override int ReplaceInside(string vName, BaseExpression expr) => StructuredExpression.ReplaceInList(this.operands, vName, expr);

    protected override BaseExpression GetExplicit(bool simplified)
    {
      List<BaseExpression> operands = (List<BaseExpression>) null;
      List<string> operators = (List<string>) null;
      if (this.operands != null)
      {
        int num = this.OperationCount + 1;
        operands = new List<BaseExpression>();
        for (int index = 0; index < num; ++index)
          operands.Add(BaseExpression.ExplicitExpression(this.operands[index], simplified));
      }
      if (this.operators != null)
        operators = new List<string>((IEnumerable<string>) this.operators);
      return BaseExpression.CreateBinaryExpression(Symbols.NotDefinedSign, this.BinaryType, operators, operands);
    }

    protected int GetRealValues(ref bool[] iv, ref double[] rv)
    {
      int realValues = 0;
      int length = Utilities.SafeCount((IList) this.operands);
      iv = new bool[length];
      rv = new double[length];
      for (int index = 0; index < length; ++index)
      {
        double x = double.NaN;
        iv[index] = LiteralExpression.IsRealValue(this.operands[index], ref x);
        if (iv[index])
        {
          rv[index] = x;
          ++realValues;
        }
        else
          rv[index] = double.NaN;
      }
      return realValues;
    }

    protected virtual BinaryOperationType GetBinaryType() => this.operators == null || this.operators.Count == 0 ? BinaryOperationType.Unknown : Utilities.GetBinaryOperationType(this.operators[0]);

    protected override void Degenerate()
    {
      if (this.operands != null)
      {
        for (int index = this.operands.Count - 1; index > 0; --index)
          this.operands.RemoveAt(index);
      }
      if (this.operators == null)
        return;
      this.operators.Clear();
    }

    protected virtual void SimplifyOperands() => BaseExpression.SimplifyList(this.operands, true);

    protected virtual void MergeDegenerated()
    {
      if (this.operands == null)
        return;
      int count = this.operands.Count;
      for (int index = 0; index < count; ++index)
      {
        BaseExpression operand = this.operands[index];
        if (operand is StructuredExpression && ((StructuredExpression) operand).IsDegenerated)
        {
          BaseExpression degenerated = (operand as StructuredExpression).Degenerated;
          if (degenerated != null)
            this.operands[index] = degenerated;
        }
      }
    }

    protected virtual void MergeOperands()
    {
    }

    protected virtual void SimplifyBase()
    {
    }

    protected virtual void MergeConstants()
    {
    }

    protected void InsertOperationLeft(int operand, string op, BaseExpression opnd)
    {
      this.operators.Insert(operand, op);
      this.operands.Insert(operand, opnd);
    }

    protected void InsertOperationRight(int operand, string op, BaseExpression opnd)
    {
      this.operators.Insert(operand, op);
      this.operands.Insert(operand + 1, opnd);
    }

    protected void RemoveOperationLeft(int operation)
    {
      this.operands.RemoveAt(operation);
      this.operators.RemoveAt(operation);
    }

    protected void RemoveOperationRight(int operation)
    {
      this.operands.RemoveAt(operation + 1);
      this.operators.RemoveAt(operation);
    }

    protected void ClearOperations()
    {
      this.operands = new List<BaseExpression>();
      this.operators = new List<string>();
    }

    protected virtual bool CanExpandInternal()
    {
      int num = Utilities.SafeCount((IList) this.operands);
      for (int index = 0; index < num; ++index)
      {
        BaseExpression baseExpression = this.operands[index].This;
        if (baseExpression is BinaryOperationsExpression && (baseExpression as BinaryOperationsExpression).CanExpand())
          return true;
      }
      return false;
    }

    protected virtual BaseExpression ExpandInternal()
    {
      if (!this.CanExpandInternal())
        return (BaseExpression) null;
      BaseExpression baseExpression1 = (BaseExpression) null;
      int num = Utilities.SafeCount((IList) this.operands);
      if (num > 0)
      {
        List<BaseExpression> operands = new List<BaseExpression>();
        for (int index = 0; index < num; ++index)
        {
          BaseExpression baseExpression2 = this.operands[index].This;
          BaseExpression baseExpression3 = (BaseExpression) null;
          if (baseExpression2 is BinaryOperationsExpression)
          {
            BinaryOperationsExpression operationsExpression = baseExpression2 as BinaryOperationsExpression;
            if (operationsExpression.CanExpand())
              baseExpression3 = operationsExpression.Expand();
          }
          if (baseExpression3 == null)
            baseExpression3 = baseExpression2.Copy(false);
          operands.Add(baseExpression3);
        }
        List<string> operators = new List<string>();
        operators.AddRange((IEnumerable<string>) this.operators);
        baseExpression1 = BaseExpression.CreateBinaryExpression(Symbols.NotDefinedSign, this.BinaryType, operators, operands);
      }
      return baseExpression1;
    }

    public BinaryOperationsExpression(
      string aValue,
      List<string> theOperators,
      List<BaseExpression> theOperands)
      : base(aValue)
    {
      if (theOperands.Count != theOperators.Count + 1)
        throw new ArgumentException("Operator-Operand count");
      this.operators = new List<string>((IEnumerable<string>) theOperators);
      this.operands = new List<BaseExpression>((IEnumerable<BaseExpression>) theOperands);
      this._context = this.GetInternalContext();
    }

    public int OperationCount => this.operands == null ? -1 : this.operands.Count - 1;

    public override bool IsDegenerated => this.OperationCount == 0;

    public override BaseExpression Degenerated => this.operands[0];

    public BinaryOperationType BinaryType => this.GetBinaryType();

    public List<string> Operators => this.operators;

    public List<BaseExpression> Operands => this.operands;

    public virtual bool MustEnclose(int index, BaseExpression opnd)
    {
      bool flag = true;
      if (opnd != null)
      {
        if (opnd is VariableExpression || opnd is FunctionExpression || opnd is ArrayExpression)
        {
          flag = false;
        }
        else
        {
          if (opnd is StructuredExpression)
          {
            StructuredExpression structuredExpression = opnd as StructuredExpression;
            if (structuredExpression.IsDegenerated)
              return this.MustEnclose(index, structuredExpression.Degenerated);
          }
          if (opnd is UnaryOperatorExpression)
          {
            if (Utilities.GetOperatorType((opnd as UnaryOperatorExpression).Sign, OperatorArity.Unary) == OperatorType.Minus)
            {
              if (index == 0)
                flag = false;
              else if (Utilities.GetOperatorType(this.operators[index - 1], OperatorArity.Binary) == OperatorType.Power)
                flag = false;
            }
            else
              flag = false;
          }
          else if (opnd is LiteralExpression)
          {
            LiteralExpression literalExpression = opnd as LiteralExpression;
            if (Parser.IsReal(literalExpression.Value))
            {
              if (index == 0 || (int) literalExpression.Value[0] != (int) Symbols.MinusOperator)
                flag = false;
              else if (Utilities.GetOperatorType(this.operators[index - 1], OperatorArity.Binary) == OperatorType.Power)
                flag = false;
            }
            else
            {
              CheckedData<Complex> checkedData = Parser.IsComplex(literalExpression.Value, true);
              if (checkedData.Is)
              {
                Complex complex = checkedData.Value;
                flag = complex.Real != 0.0 || complex.Imaginary < 0.0 && index > 0;
              }
              else
                flag = true;
            }
          }
          else if (opnd is BinaryOperationsExpression)
          {
            BinaryOperationsExpression operationsExpression = opnd as BinaryOperationsExpression;
            if (operationsExpression.Precedence > this.Precedence)
            {
              flag = false;
              if (index > 0 && operationsExpression.OperationCount >= 0)
              {
                flag = operationsExpression.Operands[0] is UnaryOperatorExpression && (operationsExpression.Operands[0] as UnaryOperatorExpression).Operator == OperatorType.Minus;
                if (!flag && operationsExpression.Operands[0] is LiteralExpression)
                {
                  CheckedData<double> checkedData = Parser.IsReal((operationsExpression.Operands[0] as LiteralExpression).Value, true);
                  flag = !checkedData.Is || checkedData.Value < 0.0;
                }
              }
            }
            else
              flag = true;
          }
          else
          {
            if (!(opnd is ImplicitOperatorExpression))
              return false;
            flag = (opnd as ImplicitOperatorExpression).Precedence <= this.Precedence;
          }
        }
      }
      return flag;
    }

    public override string Reconstruct()
    {
      if (this.IsDegenerated)
        return this.operands[0].Reconstruct();
      List<bool> enclose = (List<bool>) null;
      if (this.operands != null && this.operands.Count > 0)
      {
        enclose = new List<bool>();
        int count = this.operands.Count;
        for (int index = 0; index < count; ++index)
        {
          bool flag = this.MustEnclose(index, this.operands[index]);
          enclose.Add(flag);
        }
      }
      return Builder.BuildBinaryOperations(BaseExpression.GetReconstructed(this.operands), this.operators, enclose);
    }

    public override bool IsConstant() => BaseExpression.AreConstant(this.operands);

    public override bool DependsOn(string vName) => BaseExpression.IsDependant(this.operands, vName);

    public override void Simplify()
    {
      base.Simplify();
      this.SimplifyOperands();
      this.MergeDegenerated();
      this.MergeOperands();
      this.SimplifyBase();
      this.MergeConstants();
    }

    public virtual bool CanExpand() => this.CanExpandInternal();

    public virtual BaseExpression Expand() => this.ExpandInternal();

    public static BaseExpression MakeBinary(
      BinaryOperationType bt,
      List<BaseExpression> operands,
      List<string> operators)
    {
      if (operands == null)
        return (BaseExpression) null;
      return operands.Count == 1 ? operands[0] : BaseExpression.CreateBinaryExpression(Symbols.NotDefinedSign, bt, operators, operands);
    }

    public static BaseExpression MakeBinary(List<BaseExpression> expressions, string sign)
    {
      if (expressions == null)
        return (BaseExpression) null;
      int count = expressions.Count;
      List<string> operators = new List<string>();
      for (int index = 1; index < count; ++index)
        operators.Add(sign);
      return BinaryOperationsExpression.MakeBinary(Utilities.GetBinaryOperationType(sign), expressions, operators);
    }

    public override string ToString() => "Binary operations (" + this.BinaryType.ToString() + "). Count: " + this.OperationCount.ToString() + " = " + this.GetValue();

    public override string Print(int level)
    {
      string str1 = new string(' ', level * 2);
      string str2 = str1 + "Binary operations (" + this.BinaryType.ToString() + "). Count: " + this.OperationCount.ToString();
      if (this.operands != null && this.operands.Count > 0)
      {
        str2 = str2 + Environment.NewLine + this.operands[0].Print(level + 1);
        for (int index = 1; index < this.operands.Count; ++index)
          str2 = str2 + Environment.NewLine + str1 + this.operators[index - 1] + Environment.NewLine + this.operands[index].Print(level + 1);
      }
      return str2;
    }
  }
}
