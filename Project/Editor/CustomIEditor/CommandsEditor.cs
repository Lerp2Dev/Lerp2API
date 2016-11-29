using UnityEngine;
using UnityEditor;
using Lerp2API;
using Lerp2API.Game;
using System.IO;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2APIEditor.CustomIEditor 
{
	[CustomEditor(typeof(GameConsole))]
	public class CommandsEditor : Editor 
	{
		public static string commandPath 
		{
			get 
			{
				return Path.Combine(Application.dataPath, "Lerp2API/L2Commands.sav");
			}
		}

		public static GameConsole myTarget;

	    public override void OnInspectorGUI()
	    {
	    	DrawDefaultInspector();

	    	myTarget = (GameConsole)target;

	        if (GUILayout.Button("Save Commands"))
	        	SaveCommands();

	        if (GUILayout.Button("Load Commands"))
	        	LoadCommands();
	    }

	    internal static void SaveCommands() 
	    {
	    	JSONHelpers.SerializeToFile(commandPath, myTarget.commandList);
	    	Debug.Log("Command file saved!"); //Better a dialog, not?
	    }

	    internal static void LoadCommands() 
	    {
	    	if(File.Exists(commandPath))
	    		myTarget.commandList = JSONHelpers.Deserialize<Command[]>(File.ReadAllText(commandPath));
	    	else
	    		Debug.LogError("Command file couldn't be found!");
	    }
	}
}