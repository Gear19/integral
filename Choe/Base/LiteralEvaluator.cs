namespace Choe
{
  public class LiteralEvaluator
  {
    protected LiteralIs isfunction;
    protected LiteralEvaluate evaluationfunction;

    public LiteralEvaluator(LiteralIs li, LiteralEvaluate le)
    {
      this.isfunction = li;
      this.evaluationfunction = le;
    }

    public LiteralIs Is => this.isfunction;

    public LiteralEvaluate Evaluate => this.evaluationfunction;
  }
}
