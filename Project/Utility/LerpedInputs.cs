using System.Collections.Generic;
using System.IO;
using System.Linq;
using TeamUtility.IO;
using UnityEngine;

namespace Lerp2API.Utility
{
    public class LerpedInputs
    {
        private static ButtonData[] Buttons;
        private static AxisData[] Axis;

        private readonly static string axisJsonPath = Path.Combine(Application.dataPath, "Lerp2API/AxisData.json"),
                                       buttonJsonPath = Path.Combine(Application.dataPath, "Lerp2API/ButtonData.json");

        public static void LoadAxis()
        {
            InputManager.CreateInputConfiguration("DefaultConfig");
            AxisData[] aData = JsonUtility.FromJson<AxisData[]>(File.ReadAllText(axisJsonPath));
            foreach (AxisData axis in aData)
                AddAxis(axis);
        }

        public static void SaveAxis()
        {
            File.WriteAllText(axisJsonPath, JsonUtility.ToJson(Axis, true));
        }

        public static void LoadButtons()
        {
            InputManager.CreateInputConfiguration("DefaultConfig");
            ButtonData[] bData = JsonUtility.FromJson<ButtonData[]>(File.ReadAllText(buttonJsonPath));
            foreach (ButtonData button in bData)
                AddButton(button);
        }

        public static void SaveButtons()
        {
            File.WriteAllText(buttonJsonPath, JsonUtility.ToJson(Buttons, true));
        }

        public static void AddAxis(AxisData data)
        {
            //I have to introducir los nuevos datos del axis aquí en el custom input manager.
            //https://github.com/daemon3000/InputManager/wiki/Key-Remap
            List<AxisData> aList = new List<AxisData>();
            if (Axis != null)
                aList = Axis.ToList();
            aList.Add(data);
            Axis = aList.ToArray();
        }

        public static void AddButton(ButtonData data)
        {
            //I have to introducir los nuevos datos del axis aquí en el custom input manager.
            //https://github.com/daemon3000/InputManager/wiki/Key-Remap
            List<ButtonData> bList = new List<ButtonData>();
            if (Axis != null)
                bList = Buttons.ToList();
            bList.Add(data);
            Buttons = bList.ToArray();
        }
    }
}