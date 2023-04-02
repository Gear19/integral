namespace Choe.Syntactic
{
  public abstract class SyntaxError
  {
    protected string primary;
    protected int position1;
    protected int position2;

    protected abstract string GetDescription();

    public SyntaxError(string value, int pos1, int pos2)
    {
      this.primary = value;
      this.position1 = pos1;
      this.position2 = pos2;
    }

    public string Description => Messages.SyntaxErrorMessage + " " + this.GetDescription();

    public string Primary => this.primary;

    public int Position1 => this.position1;

    public int Position2 => this.position2;
  }
}
