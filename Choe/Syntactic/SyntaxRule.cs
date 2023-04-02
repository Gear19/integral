namespace Choe.Syntactic
{
  public class SyntaxRule
  {
    protected SyntaxRuleFunction function;

    public SyntaxRule(SyntaxRuleFunction f) => this.function = f;

    public SyntaxRuleFunction RuleFunction => this.function;
  }
}
