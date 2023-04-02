using NRTE;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Choe.Integrals
{
  public class AutoIntegralContext : IntegralContext
  {
    public AutoIntegralContext() => this.UpdateDefault();

    public override void UpdateDefault()
    {
      this.UpdateRegisteredFunctions();
      this.UpdateRegisteredIntegrators();
    }

    public virtual void UpdateRegisteredFunctions()
    {
      this._functions = new List<FunctionalIntegral>();
      List<Type> allDescendants = ClassFinder.FindAllDescendants(typeof (FunctionalIntegral));
      int num = Utilities.SafeCount((IList) allDescendants);
      if (num <= 0)
        return;
      for (int index = 0; index < num; ++index)
      {
        if (!allDescendants[index].IsAbstract && allDescendants[index].IsSealed && ClassFinder.InstantiateClass(allDescendants[index]) is FunctionalIntegral functionalIntegral)
          this._functions.Add(functionalIntegral);
      }
    }

    public virtual void UpdateRegisteredIntegrators()
    {
      this._integrators = new List<Integrator>();
      List<Type> allDescendants = ClassFinder.FindAllDescendants(typeof (Integrator));
      int num = Utilities.SafeCount((IList) allDescendants);
      if (num <= 0)
        return;
      for (int index = 0; index < num; ++index)
      {
        if (!allDescendants[index].IsAbstract && allDescendants[index].IsSealed && ClassFinder.InstantiateClass(allDescendants[index]) is Integrator integrator)
          this._integrators.Add(integrator);
      }
    }
  }
}
