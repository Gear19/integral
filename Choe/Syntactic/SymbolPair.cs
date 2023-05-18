namespace Choe.Syntactic
{
  public class SymbolPair
  {
    protected char opening;
    protected char closing;

    public SymbolPair(char c1, char c2)
    {
      this.opening = c1;
      this.closing = c2;
    }

    public char Opening => this.opening;

    public char Closing => this.closing;

    public int Counter(char c)
    {
      if ((int) c == (int) this.opening)
        return 1;
      return (int) c == (int) this.closing ? -1 : 0;
    }

    public static bool operator ==(SymbolPair p1, SymbolPair p2)
    {
      if ((object) p1 == null && (object) p2 == null)
        return true;
      return (object) p1 != null && (object) p2 != null && (int) p1.Opening == (int) p2.Opening && (int) p1.Closing == (int) p2.Closing;
    }

    public static bool operator !=(SymbolPair p1, SymbolPair p2) => !(p1 == p2);

    public override string ToString() => this.opening.ToString() + this.closing.ToString();

    public override bool Equals(object obj) => obj as SymbolPair == this;

    public override int GetHashCode() => this.opening.GetHashCode() | this.closing.GetHashCode();
  }
}
