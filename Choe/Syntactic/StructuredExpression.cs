using System.Collections.Generic;

namespace Choe.Syntactic
{
  public abstract class StructuredExpression : BaseExpression
  {
    protected override BaseExpression GetThis() => this.IsDegenerated ? this.Degenerated : (BaseExpression) this;

    protected abstract int GetSelfPrecedence();

    protected override int GetPrecedence() => this.IsDegenerated ? this.Degenerated.Precedence : this.GetSelfPrecedence();

    protected virtual void Degenerate()
    {
    }

    protected abstract int ReplaceInside(string vName, BaseExpression expr);

    protected abstract BaseExpression GetExplicit(bool simplified);

    public StructuredExpression(string aValue)
      : base(aValue)
    {
    }

    public virtual int Replace(string vName, BaseExpression expr)
    {
      int num = this.ReplaceInside(vName, expr);
      if (num <= 0)
        return num;
      this.value = Builder.BuildFunction(nameof (Replace), new List<string>()
      {
        vName + Symbols.FunctionSign + expr.PrimaryValue
      }, new List<string>() { this.value });
      return num;
    }

    public virtual bool IsDegenerated => false;

    public virtual BaseExpression Degenerated => (BaseExpression) null;

    public virtual BaseExpression CopyExplicit(bool simplified)
    {
      if (this.IsDegenerated)
        return BaseExpression.ExplicitExpression(this.Degenerated, simplified);
      BaseExpression baseExpression = this.GetExplicit(simplified);
      if (simplified)
        baseExpression.Simplify();
      return baseExpression;
    }

    public static int ReplaceInList(
      List<BaseExpression> expressions,
      string vName,
      BaseExpression expr)
    {
      if (expressions == null || expressions.Count == 0)
        return 0;
      int num = 0;
      int count = expressions.Count;
      for (int index = 0; index < count; ++index)
      {
        if (expressions[index] is StructuredExpression expression2)
          num += expression2.Replace(vName, expr);
        else if (expressions[index] is VariableExpression expression1 && expression1.Name == vName)
        {
          expressions[index] = expr;
          ++num;
        }
      }
      return num;
    }

    public static int MergeDegenerated(List<BaseExpression> expressions)
    {
      if (expressions == null)
        return 0;
      int num = 0;
      int count = expressions.Count;
      for (int index = 0; index < count; ++index)
      {
        if (expressions[index] is StructuredExpression)
        {
          StructuredExpression expression = expressions[index] as StructuredExpression;
          if (expression.IsDegenerated)
          {
            BaseExpression degenerated = expression.Degenerated;
            if (degenerated != null)
            {
              expressions[index] = degenerated;
              ++num;
            }
          }
        }
      }
      return num;
    }
  }
}
