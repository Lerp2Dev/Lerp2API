using UnityEngine;
using UnityEditor;
using Lerp2API;
using Lerp2API.Game;
using System.IO;
using Debug = Lerp2API._Debug.Debug;
using Lerp2API.Hepers.JSON_Extensions;

namespace Lerp2APIEditor.CustomIEditor 
{
    /// <summary>
    /// Class CommandsEditor.
    /// </summary>
    /// <seealso cref="UnityEditor.Editor" />
    [CustomEditor(typeof(GameConsole))]
	public class CommandsEditor : Editor 
	{
        /// <summary>
        /// Gets the command path.
        /// </summary>
        /// <value>The command path.</value>
        public static string commandPath 
		{
			get 
			{
				return Path.Combine(Application.dataPath, "Lerp2API/L2Commands.sav");
			}
		}

        /// <summary>
        /// My target
        /// </summary>
        public static GameConsole myTarget;

        /// <summary>
        /// Called when [inspector GUI].
        /// </summary>
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