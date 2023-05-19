namespace Choe.Syntactic
{
  public class InvalidEndSymbolError : InvalidSymbolError
  {
    protected override string GetDescription() => string.Format(Messages.InvalidEndSymbolFormat, (object) this.invalid.ToString());

    public InvalidEndSymbolError(string value, int pos1, char s)
      : base(value, pos1, s)
    {
    }
  }
}
