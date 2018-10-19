﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class OrientationCubeComponentSolver : ComponentSolver
{
	public OrientationCubeComponentSolver(BombCommander bombCommander, BombComponent bombComponent) :
		base(bombCommander, bombComponent)
	{
		_submit = (MonoBehaviour) SubmitField.GetValue(bombComponent.GetComponent(ComponentType));
		_left = (MonoBehaviour) LeftField.GetValue(bombComponent.GetComponent(ComponentType));
		_right = (MonoBehaviour) RightField.GetValue(bombComponent.GetComponent(ComponentType));
		_ccw = (MonoBehaviour) CcwField.GetValue(bombComponent.GetComponent(ComponentType));
		_cw = (MonoBehaviour) CwField.GetValue(bombComponent.GetComponent(ComponentType));
		//virtualAngleEmulator = (float)_virtualField.GetValue(bombComponent.GetComponent(_componentType));
		ModInfo = ComponentSolverFactory.GetModuleInfo(GetModuleType(), "Move the cube with !{0} press cw l set. The buttons are l, r, cw, ccw, set.");
	}

	protected internal override IEnumerator RespondToCommandInternal(string inputCommand)
	{
		List<MonoBehaviour> buttons = new List<MonoBehaviour>();

		string[] split = inputCommand.Trim().ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		if (split.Length < 2 || split[0] != "press")
			yield break;

		if (_submit == null || _left == null || _right == null || _ccw == null || _cw == null)
		{
			yield return "autosolve due to required buttons not present.";
			yield break;
		}

		foreach (string cmd in split.Skip(1))
		{
			switch (cmd)
			{
				case "left": case "l": buttons.Add(_left); _interaction.Add("Left rotation"); break;

				case "right": case "r": buttons.Add(_right); _interaction.Add("Right rotation"); break;

				case "counterclockwise":
				case "counter-clockwise":
				case "ccw":
				case "anticlockwise":
				case "anti-clockwise":
				case "acw": buttons.Add(_ccw); _interaction.Add("Counterclockwise rotation"); break;

				case "clockwise": case "cw": buttons.Add(_cw); _interaction.Add("Clockwise rotation"); break;

				case "set": case "submit": buttons.Add(_submit); _interaction.Add("submit"); break;

				default: yield break;
			} //Check for any invalid commands.  Abort entire sequence if any invalid commands are present.
		}

		yield return "Orientation Cube Solve Attempt";
		string debugStart = "[Orientation Cube TP#" + Code + "]";
		Debug.LogFormat("{0} Inputted commands: {1}", debugStart, string.Join(", ", _interaction.ToArray()));

		foreach (MonoBehaviour button in buttons)
		{
			yield return DoInteractionClick(button);
		}
	}

	static OrientationCubeComponentSolver()
	{
		ComponentType = ReflectionHelper.FindType("OrientationModule");
		SubmitField = ComponentType.GetField("SubmitButton", BindingFlags.Public | BindingFlags.Instance);
		LeftField = ComponentType.GetField("YawLeftButton", BindingFlags.Public | BindingFlags.Instance);
		RightField = ComponentType.GetField("YawRightButton", BindingFlags.Public | BindingFlags.Instance);
		CcwField = ComponentType.GetField("RollLeftButton", BindingFlags.Public | BindingFlags.Instance);
		CwField = ComponentType.GetField("RollRightButton", BindingFlags.Public | BindingFlags.Instance);
		//_virtualField = _componentType.GetField("virtualViewAngle", BindingFlags.NonPublic | BindingFlags.Instance);
	}

	private static readonly Type ComponentType;
	private static readonly FieldInfo SubmitField;
	private static readonly FieldInfo LeftField;
	private static readonly FieldInfo RightField;
	private static readonly FieldInfo CcwField;
	private static readonly FieldInfo CwField;
	//private static FieldInfo _virtualField = null;

	private readonly List<string> _interaction = new List<string>();
	/*private string[] sides = new string[] { "l", "r", "f", "b", "t", "o" };
	private Quaternion emulatedView;
	private bool first = true;
	private float originalAngle;*/

	private readonly MonoBehaviour _submit;
	private readonly MonoBehaviour _left;
	private readonly MonoBehaviour _right;
	private readonly MonoBehaviour _ccw;
	private readonly MonoBehaviour _cw;
	//private float virtualAngleEmulator;
}
