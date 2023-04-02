namespace Choe
{
  public class LevelData
  {
    public int[] Levels;
    public int Total;

    public LevelData(int count)
    {
      this.Levels = new int[count];
      this.Total = 0;
    }
  }
}
