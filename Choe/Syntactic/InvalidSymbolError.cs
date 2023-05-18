namespace Choe.Syntactic
{
  public class InvalidSymbolError : SyntaxError
  {
    protected char invalid;

    protected override string GetDescription() => string.Format(Messages.InvalidSymbolFormat, (object) this.invalid.ToString());

    public InvalidSymbolError(string value, int pos1, char s)
      : base(value, pos1, pos1)
    {
      this.invalid = s;
    }
  }
}
