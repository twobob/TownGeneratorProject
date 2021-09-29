using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Debugging tools
/// </summary>
public static class Instruments
{
	static string stopwatchName;
	
	static System.Diagnostics.Stopwatch stopwatch;

	[System.Diagnostics.Conditional ("UNITY_EDITOR")]
	public static void BeginStopwatch (string name)
	{
		if (stopwatch == null) {
			stopwatch = new System.Diagnostics.Stopwatch();
		}
		stopwatchName = name;
		stopwatch.Stop ();
		stopwatch.Reset ();
		stopwatch.Start ();
	}

	[System.Diagnostics.Conditional ("UNITY_EDITOR")]
	public static void EndStopwatch ()
	{
		stopwatch.Stop ();
		var time = (stopwatch.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency) * 1000;
		Debug.Log ("[Instruments] " + stopwatchName + ": " + time + " ms");
	}
}
