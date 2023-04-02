namespace Choe.Syntactic
{
  public class InvalidConstructionError : SyntaxError
  {
    protected string invalid;

    protected override string GetDescription() => string.Format(Messages.InvalidConstructionFormat, (object) this.invalid);

    public InvalidConstructionError(string value, int pos1, int pos2, string s)
      : base(value, pos1, pos2)
    {
      this.invalid = s;
    }
  }
}
