using Choe.Syntactic;
using System.Collections.Generic;

namespace Choe
{
  public static class PairUtilities
  {
    public static int FindPair(List<SymbolPair> pairs, SymbolPair value)
    {
      int count = pairs.Count;
      for (int index = 0; index < count; ++index)
      {
        if (value == pairs[index])
          return index;
      }
      return -1;
    }

    public static List<SymbolPair> GetAllPairs(List<char> symbols)
    {
      List<SymbolPair> allPairs = new List<SymbolPair>();
      int count = symbols.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        for (int index2 = index1; index2 < count; ++index2)
        {
          SymbolPair symbolPair1 = new SymbolPair(symbols[index1], symbols[index2]);
          allPairs.Add(symbolPair1);
          if (index1 != index2)
          {
            SymbolPair symbolPair2 = new SymbolPair(symbols[index2], symbols[index1]);
            allPairs.Add(symbolPair2);
          }
        }
      }
      return allPairs;
    }

    public static List<SymbolPair> GetAllPairs(List<char> symbols1, List<char> symbols2)
    {
      List<SymbolPair> allPairs = new List<SymbolPair>();
      int count1 = symbols1.Count;
      int count2 = symbols2.Count;
      for (int index1 = 0; index1 < count1; ++index1)
      {
        for (int index2 = 0; index2 < count2; ++index2)
        {
          SymbolPair symbolPair = new SymbolPair(symbols1[index1], symbols2[index2]);
          allPairs.Add(symbolPair);
        }
      }
      return allPairs;
    }

    public static void AddPairs(List<SymbolPair> additional, List<SymbolPair> pairs)
    {
      int count = additional.Count;
      for (int index = 0; index < count; ++index)
      {
        SymbolPair symbolPair = additional[index];
        if (PairUtilities.FindPair(pairs, symbolPair) < 0)
          pairs.Add(symbolPair);
      }
    }

    public static void RemovePairs(List<SymbolPair> removed, List<SymbolPair> pairs)
    {
      int count = removed.Count;
      for (int index = 0; index < count; ++index)
      {
        SymbolPair symbolPair = removed[index];
        int pair = PairUtilities.FindPair(pairs, symbolPair);
        if (pair >= 0)
          pairs.RemoveAt(pair);
      }
    }

    public static void AddSymbols(List<char> additional, List<char> symbols)
    {
      int count = additional.Count;
      for (int index = 0; index < count; ++index)
      {
        char ch = additional[index];
        if (symbols.IndexOf(ch) < 0)
          symbols.Add(ch);
      }
    }

    public static void RemoveSymbols(List<char> removed, List<char> symbols)
    {
      int count = removed.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        char ch = removed[index1];
        int index2 = symbols.IndexOf(ch);
        if (index2 >= 0)
          symbols.RemoveAt(index2);
      }
    }
  }
}
