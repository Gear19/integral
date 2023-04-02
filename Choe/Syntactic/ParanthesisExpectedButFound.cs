namespace Choe.Syntactic
{
  public class ParanthesisExpectedButFound : ParanthesisError
  {
    protected override string GetDescription() => string.Format(Messages.ParanthesisExpectedButFoundFormat, (object) this.errorpair.Opening, (object) this.errorpair.Closing);

    public ParanthesisExpectedButFound(string value, int pos1, int pos2, SymbolPair p)
      : base(value, pos1, pos2, p)
    {
    }
  }
}
