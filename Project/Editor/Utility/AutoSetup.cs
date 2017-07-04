using System.Linq;
using UnityEditor;
using Lerp2API;
using UnityEditor.Callbacks;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;
using Lerp2API.Utility;

namespace Lerp2APIEditor.Utility
{
    /// <summary>
    /// Class AutoSetup.
    /// </summary>
    public class AutoSetup : Editor
    {
        //Levarme esto pa la api, sacar los required tags de una suma de archivos. Se buscará por el archivos RequiredTags.txt (en todo el proyecto) y cada linea será un tag más a sumar.
        //Estas mecánicass las tengo que ir explicando poco a poco en una guía o algo (I have to)

        //private static readonly string[] reqTags = new string[5] { "Turret", "Projectile", "Trajectory", "Ejection", "Throwable" };

        [DidReloadScripts]
        private static void OnScriptRecompile()
        { //En este mensaje si no estan los tags voy a mostrarlo, voy a decir q si los quieres añadir
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            foreach (RequiredData dataType in Enum.GetValues(typeof(RequiredData)))
            {
                object[] rawData = null; //Aqui vendria bien hacer unos cuantos polimorfismos...
                if (!GetRequiredData(dataType, out rawData))
                    return;
                IEnumerable<string> reqData = ((NamedData[])rawData).Select(z => z.Name),
                                    neededData = reqData.Where(x => !CheckData(x, dataType));
                if (!GetDisabledData(dataType) && neededData.Count() > 0)
                {
                    int x = EditorUtility.DisplayDialogComplex("Project message", string.Format("There are unsetted tags, do you want to define them automatically? (Required tags: {0})", string.Join(", ", reqData.ToArray())), "Ok", "No", "Never");
                    switch (x)
                    {
                        case 0:
                            foreach (string dataValue in reqData)
                                DefineData(dataValue, dataType);
                            break;

                        case 2:
                            SetDataState(dataType, true);
                            EditorUtility.DisplayDialog("Project message", "You disabled this message, now you have to set the tags manually.", "Ok");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetTags(out NamedData[] tags)
        {
            object[] objs;
            bool r = GetRequiredData(RequiredData.Tags, out objs);
            tags = (NamedData[])objs;
            return r;
            //return GetRequiredData(RequiredData.Tags, out tags);
        }

        /// <summary>
        /// Gets the layers.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetLayers(out LayerData[] layers)
        {
            object[] objs;
            bool r = GetRequiredData(RequiredData.Layers, out objs);
            layers = (LayerData[])objs;
            return r;
        }

        //Esto se hace por otro lado
        /*private static bool GetAxis(out AxisData[] axis)
        {
            string[] rawData;
            bool r = GetRequiredData(RequiredData.Axis, out rawData);
            List<string[]> arr = new List<string[]>();
            for (int i = 0; i < rawData.Length; ++i)
                arr.Add(rawData[i].Split(';'));
            axis = arr.ToArray();
            return r;
        }*/

        private static bool CheckData(string name, RequiredData data)
        {
            switch (data)
            {
                case RequiredData.Tags:
                    return name.CheckTag();

                case RequiredData.Layers:
                    return name.CheckLayer();
            }
            return false;
        }

        private static void DefineData(string name, RequiredData data)
        {
            switch (data)
            {
                case RequiredData.Tags:
                    name.DefineTag();
                    break;

                case RequiredData.Layers:
                    name.DefineLayer();
                    break;
            }
        }

        private static bool GetRequiredData(RequiredData rData, out object[] data)
        {
            FileInfo[] files = new DirectoryInfo(Application.dataPath).GetFiles(GetFileNameFromData(rData), SearchOption.AllDirectories);
            List<string> reqData = new List<string>();
            foreach (FileInfo fil in files)
                foreach (string line in File.ReadAllLines(fil.FullName))
                    reqData.Add(line);
            if (reqData.Count == 0)
            {
                data = null;
                return false;
            }
            data = reqData.ToArray();
            return true;
        }

        private static string GetFileNameFromData(RequiredData rData)
        {
            switch (rData)
            {
                case RequiredData.Tags:
                    return "RequiredTags.txt";

                case RequiredData.Layers:
                    return "RequiredLayers.txt";
            }
            return "";
        }

        private static bool GetDisabledData(RequiredData rData)
        {
            switch (rData)
            {
                case RequiredData.Tags:
                    return LerpedCore.GetBool(LerpedCore.disableTagCheck);

                case RequiredData.Layers:
                    return LerpedCore.GetBool(LerpedCore.disableLayerCheck);
            }
            return true;
        }

        private static void SetDataState(RequiredData rData, bool enabled)
        {
            switch (rData)
            {
                case RequiredData.Tags:
                    LerpedCore.SetBool(LerpedCore.disableTagCheck, enabled);
                    break;

                case RequiredData.Layers:
                    LerpedCore.SetBool(LerpedCore.disableLayerCheck, enabled);
                    break;
            }
        }
    }
}