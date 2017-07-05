using System.Collections.Generic;
using System.IO;
using System.Linq;
using TeamUtility.IO;
using UnityEngine;

namespace Lerp2API.Utility
{
    /// <summary>
    /// Class LerpedInputs.
    /// </summary>
    public class LerpedInputs
    {
        /// <summary>
        /// The configuration name
        /// </summary>
        public const string configName = "LerpedConfig";

        //Not usable anymore
        /*private static ButtonData[] Buttons;
        private static AxisData[] Axis;

        private readonly static string axisJsonPath = Path.Combine(Application.dataPath, "Lerp2API/AxisData.json"),
                                       buttonJsonPath = Path.Combine(Application.dataPath, "Lerp2API/ButtonData.json");

        /// <summary>
        /// Loads the axis.
        /// </summary>
        public static void LoadAxis()
        {
            //InputManager.CreateInputConfiguration(configName);
            AxisData[] aData = JsonUtility.FromJson<AxisData[]>(File.ReadAllText(axisJsonPath));
            foreach (AxisData axis in aData)
                AddAxis(axis);
        }

        /// <summary>
        /// Saves the axis.
        /// </summary>
        public static void SaveAxis()
        {
            File.WriteAllText(axisJsonPath, JsonUtility.ToJson(Axis, true));
        }

        /// <summary>
        /// Loads the buttons.
        /// </summary>
        public static void LoadButtons()
        {
            //InputManager.CreateInputConfiguration(configName);
            ButtonData[] bData = JsonUtility.FromJson<ButtonData[]>(File.ReadAllText(buttonJsonPath));
            foreach (ButtonData button in bData)
                AddButton(button);
        }

        /// <summary>
        /// Saves the buttons.
        /// </summary>
        public static void SaveButtons()
        {
            File.WriteAllText(buttonJsonPath, JsonUtility.ToJson(Buttons, true));
        }

        /// <summary>
        /// Adds the axis.
        /// </summary>
        /// <param name="data">The data.</param>
        public static void AddAxis(AxisData data)
        {
            //I have to introducir los nuevos datos del axis aquí en el custom input manager.
            //https://github.com/daemon3000/InputManager/wiki/Key-Remap
            InputManager.CreateDigitalAxis(configName, data.Name, data.Positive, data.Negative, data.AltPositive, data.AltNegative, data.Gravity, data.Sensibility);
            List<AxisData> aList = new List<AxisData>();
            if (Axis != null)
                aList = Axis.ToList();
            aList.Add(data);
            Axis = aList.ToArray();
        }

        /// <summary>
        /// Adds the button.
        /// </summary>
        /// <param name="data">The data.</param>
        public static void AddButton(ButtonData data)
        {
            //I have to introducir los nuevos datos del axis aquí en el custom input manager.
            //https://github.com/daemon3000/InputManager/wiki/Key-Remap
            InputManager.CreateButton(configName, data.Name, data.PrimaryKey);
            List<ButtonData> bList = new List<ButtonData>();
            if (Axis != null)
                bList = Buttons.ToList();
            bList.Add(data);
            Buttons = bList.ToArray();
        }*/
    }
}