using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choe
{
	class MainProgram
	{
		private static Translator _translator;

		public static void Main()
		{
			//var fTranslator = new FormulaTranslator();
			//foreach (var item in fTranslator._formulaExpressions)
			//{
			//	Console.WriteLine(item.Reconstruct());
			//}
			_translator = new Translator();

			string newIntegral = Console.ReadLine();

			if (_translator.CheckSyntax(newIntegral))
			{
				string F = _translator.Integral(newIntegral, "x");
				Console.WriteLine(F);
			}
			else
			{
				Console.WriteLine("Wrong Syntax");
			}

			//Console.WriteLine(_translator.BuildReconstruct(newIntegral, "x"));

			Console.ReadLine();
		}
	}
}
