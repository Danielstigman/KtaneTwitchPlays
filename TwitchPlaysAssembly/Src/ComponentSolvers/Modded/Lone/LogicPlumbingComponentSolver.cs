﻿using System.Collections;
using System.Text.RegularExpressions;

public class LogicPlumbingComponentSolver : ReflectionComponentSolver
{
	public LogicPlumbingComponentSolver(TwitchModule module) :
		base(module, "logicPlumbing", "!{0} swap <coord1> <coord2> [Swaps the tiles at the specified coordinates (letter = column, number = row)] | !{0} check [Holds the top left button briefly] | Swaps can be chained using spaces, commas, or semicolons")
	{
	}

	public override IEnumerator Respond(string[] split, string command)
	{
		if (_component.GetValue<bool>("solved")) yield break;
		if (command.StartsWith("swap "))
		{
			if (split.Length < 3 || split.Length % 2 == 0) yield break;
			for (int i = 1; i < split.Length; i++)
			{
				if (!Regex.IsMatch(split[i], @"^\s*[a-f][1-6]\s*$")) yield break;
			}

			yield return null;
			for (int i = 1; i < split.Length; i++)
			{
				yield return Click((int.Parse(split[i][1].ToString()) - 1) * 6 + "abcdef".IndexOf(split[i][0]));
			}
		}
		else if (command.Equals("check"))
		{
			yield return null;
			DoInteractionStart(selectables[36]);
			while (_component.GetValue<int>("waveStep") <= 24) yield return "trycancel";
			DoInteractionEnd(selectables[36]);
			if (_component.GetValue<bool>("solved")) yield return "solve";
		}
	}
}