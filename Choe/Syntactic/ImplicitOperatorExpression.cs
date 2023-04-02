using System.Collections.Generic;

namespace Choe.Syntactic
{
  public abstract class ImplicitOperatorExpression : StructuredExpression
  {
    protected BaseExpression _operand;
    protected BaseExpression _explicit;

    protected override ExpressionContext GetInternalContext()
    {
      ExpressionContext internalContext = (ExpressionContext) null;
      if (this._operand != null)
        internalContext = this._operand.Context;
      return internalContext;
    }

    protected override void SetInternalContext(ExpressionContext ctxt)
    {
      if (this._operand == null)
        return;
      this._operand.Context = ctxt;
    }

    public ImplicitOperatorExpression(ExpressionContext ctxt, string aValue)
      : base(aValue)
    {
      this._context = ctxt;
    }

    public BaseExpression Operand => this._operand;

    public BaseExpression Explicit => this.GetExplicitLazy();

    protected abstract void Update();

    protected abstract void MergeOperand();

    protected abstract void SimplifyBase();

    public abstract BaseExpression BuildSame(BaseExpression opnd);

    public override bool IsConstant() => this._operand == null || this._operand.IsConstant();

    public override bool DependsOn(string vName) => this._operand != null && this._operand.DependsOn(vName);

    public override BaseExpression Degenerated => this._operand;

    protected virtual void FreeExplicit() => this._explicit = (BaseExpression) null;

    public virtual BaseExpression GetExplicitLazy()
    {
      if (this._explicit == null)
        this.Update();
      return this._explicit;
    }

    public virtual int ReplaceInOperand(string vName, BaseExpression expr)
    {
      List<BaseExpression> expressions = new List<BaseExpression>();
      expressions.Add(this._operand);
      int num = StructuredExpression.ReplaceInList(expressions, vName, expr);
      if (num <= 0)
        return num;
      this._operand = expressions[0];
      return num;
    }

    protected override int ReplaceInside(string vName, BaseExpression expr)
    {
      int num = this.ReplaceInOperand(vName, expr);
      if (num <= 0)
        return num;
      this.FreeExplicit();
      return num;
    }

    protected override BaseExpression GetExplicit(bool simplified) => this.Explicit != null ? this.Explicit.Copy(simplified) : (BaseExpression) null;

    protected virtual void SimplifyOperand()
    {
      if (this._operand == null)
        return;
      this._operand.Simplify();
    }

    protected virtual void MergeDegenerated()
    {
      if (!(this._operand is StructuredExpression))
        return;
      StructuredExpression operand = this._operand as StructuredExpression;
      if (!operand.IsDegenerated)
        return;
      this._operand = operand.Degenerated;
    }

    protected virtual void SimplifyExplicit()
    {
      if (this.Explicit == null)
        return;
      this._explicit.Simplify();
      if (!(this._explicit is StructuredExpression))
        return;
      StructuredExpression structuredExpression = this._explicit as StructuredExpression;
      if (!structuredExpression.IsDegenerated)
        return;
      this._explicit = structuredExpression.Degenerated;
    }

    public override void Simplify()
    {
      base.Simplify();
      this.SimplifyOperand();
      this.MergeDegenerated();
      this.MergeOperand();
      this.SimplifyBase();
      this.FreeExplicit();
      this.SimplifyExplicit();
    }
  }
}
