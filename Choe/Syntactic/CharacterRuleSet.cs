using System.Collections.Generic;

namespace Choe.Syntactic
{
  public class CharacterRuleSet
  {
    protected Dictionary<char, List<CharacterRule>> rules;

    protected List<CharacterRule> FindRules(char character) => this.rules.ContainsKey(character) ? this.rules[character] : (List<CharacterRule>) null;

    protected void AddRule(char character, CharacterRule rule) => this.rules.Add(character, new List<CharacterRule>((IEnumerable<CharacterRule>) new CharacterRule[1]
    {
      rule
    }));

    public CharacterRuleSet() => this.rules = new Dictionary<char, List<CharacterRule>>();

    public void SetRule(char character, CharacterRule rule)
    {
      List<CharacterRule> rules = this.FindRules(character);
      if (rules != null)
        rules.Add(rule);
      else
        this.AddRule(character, rule);
    }

    public bool CheckRules(char symbol, string value, int index)
    {
      List<CharacterRule> rules = this.FindRules(symbol);
      if (rules == null)
        return true;
      for (int index1 = 0; index1 < rules.Count; ++index1)
      {
        if (!rules[index1](value, index))
          return false;
      }
      return true;
    }
  }
}
