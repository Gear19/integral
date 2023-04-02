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

			Console.ReadLine();
		}
	}
}
