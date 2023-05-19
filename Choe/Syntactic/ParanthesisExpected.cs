namespace Choe.Syntactic
{
  public class ParanthesisExpected : ParanthesisError
  {
    protected override string GetDescription() => string.Format(Messages.ParanthesisExpectedFormat, (object) this.errorpair.Closing);

    public ParanthesisExpected(string value, int pos1, int pos2, SymbolPair p)
      : base(value, pos1, pos2, p)
    {
    }
  }
}
