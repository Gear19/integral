namespace Choe.Syntactic
{
  public class InvalidBeginSymbolError : InvalidSymbolError
  {
    protected override string GetDescription() => string.Format(Messages.InvalidBeginSymbolFormat, (object) this.invalid.ToString());

    public InvalidBeginSymbolError(string value, int pos1, char s)
      : base(value, pos1, s)
    {
    }
  }
}
