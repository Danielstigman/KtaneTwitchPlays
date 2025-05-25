using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ModuleID("AnagramsModule")]
[ModuleID("WordScrambleModule")]
public class AnagramsComponentSolver : ComponentSolver
{
	public AnagramsComponentSolver(TwitchModule module) :
		base(module)
	{
		_buttons = module.BombComponent.GetComponent<KMSelectable>().Children;
		string modType = GetModuleType();
		_component = Module.BombComponent.GetComponent(ReflectionHelper.FindType(modType));
		SetHelpMessage("Submit your answer with !{0} submit poodle");
	}

	protected internal override IEnumerator RespondToCommandInternal(string inputCommand)
	{
		inputCommand = inputCommand.Trim();
		object[] anagramWords =
		{
			"stream", "master", "tamers", "looped", "poodle", "pooled",
			"cellar", "caller", "recall", "seated", "sedate", "teased",
			"rescue", "secure", "recuse", "rashes", "shears", "shares",
			"barely", "barley", "bleary", "duster", "rusted", "rudest"
		};

		object[] wordscrambleWords =
		{
			"archer", "attack", "banana", "blasts", "bursts", "button",
			"cannon", "casing", "charge", "damage", "defuse", "device",
			"disarm", "flames", "kaboom", "kevlar", "keypad", "letter",
			"module", "mortar", "napalm", "ottawa", "person", "robots",
			"rocket", "sapper", "semtex", "weapon", "widget", "wiring",
		};

		List<KMSelectable> buttons = new List<KMSelectable>();
		List<string> buttonLabels = _buttons.Select(button => button.GetComponentInChildren<TextMesh>().text.ToLowerInvariant()).ToList();
		if (!inputCommand.StartsWith("submit ", System.StringComparison.InvariantCultureIgnoreCase)) yield break;
		inputCommand = inputCommand.Substring(7).ToLowerInvariant();
		foreach (char c in inputCommand)
		{
			int index = buttonLabels.IndexOf(c.ToString());
			if (index < 0)
			{
				if (!inputCommand.EqualsAny(anagramWords) && !inputCommand.EqualsAny(wordscrambleWords)) yield break;
				yield return null;
				yield return "unsubmittablepenalty";
				yield break;
			}
			buttons.Add(_buttons[index]);
		}

		if (buttons.Count != 6) yield break;

		yield return null;
		yield return DoInteractionClick(_buttons[3]);
		foreach (KMSelectable b in buttons)
		{
			yield return DoInteractionClick(b);
		}
		yield return DoInteractionClick(_buttons[7]);
	}

	protected override IEnumerator ForcedSolveIEnumerator()
	{
		yield return null;
		string curr = _component.GetValue<TextMesh>("AnswerDisplay").text;
		List<string> ans = GetModuleType() == "AnagramsModule" ?
			_component.GetValue<List<string>>("_solution") :
			new List<string>() { _component.GetValue<string>("_solution") };
		if (curr.Length > 6)
		{
			yield return DoInteractionClick(_buttons[3]);
			curr = "";
		}
		for (int j = 0; j < ans.Count; j++)
		{
			for (int i = 0; i < curr.Length; i++)
			{
				if (curr[i] != ans[j][i])
				{
					ans.RemoveAt(j);
					j--;
					break;
				}
			}
		}
		if (ans.Count == 0)
		{
			yield return DoInteractionClick(_buttons[3]);
			if (GetModuleType() == "AnagramsModule")
				ans = _component.GetValue<List<string>>("_solution");
			else
				ans.Add(_component.GetValue<string>("_solution"));
			curr = "";
		}
		int start = curr.Length;
		int ansIndex = Random.Range(0, ans.Count);
		for (int j = start; j < 6; j++)
			yield return DoInteractionClick(_buttons.Where(button => button.GetComponentInChildren<TextMesh>().text.EqualsIgnoreCase(ans[ansIndex][j].ToString())).ToList()[0]);
		yield return DoInteractionClick(_buttons[7], 0);
	}

	private readonly object _component;
	private readonly KMSelectable[] _buttons;
}