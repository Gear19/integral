namespace Choe.Syntactic
{
  public abstract class SimpleExpression : BaseExpression
  {
    protected override ExpressionContext GetInternalContext() => (ExpressionContext) null;

    protected override void SetInternalContext(ExpressionContext ctxt)
    {
    }

    public SimpleExpression(string aValue)
      : base(aValue)
    {
    }
  }
}
