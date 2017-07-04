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
    /// Load/Save Tags & Layers
    /// </summary>
    public class AutoSetup : Editor
    {
        //Levarme esto pa la api, sacar los required tags de una suma de archivos. Se buscará por el archivos RequiredTags.txt (en todo el proyecto) y cada linea será un tag más a sumar.
        //Estas mecánicass las tengo que ir explicando poco a poco en una guía o algo (I have to)

        //private static readonly string[] reqTags = new string[5] { "Turret", "Projectile", "Trajectory", "Ejection", "Throwable" };

        private static NamedData[] _tags;
        private static LayerData[] _layers;

        public static NamedData[] Tags
        {
            get
            {
                if (_tags == null)
                    _tags = JsonUtility.FromJson<NamedData[]>(File.ReadAllText(GetFileNameFromData(RequiredData.Tags)));
                return _tags;
            }
        }

        public static LayerData[] Layers
        {
            get
            {
                if (_layers == null)
                    _layers = JsonUtility.FromJson<LayerData[]>(File.ReadAllText(GetFileNameFromData(RequiredData.Layers)));
                return _layers;
            }
        }

        [DidReloadScripts]
        private static void OnScriptRecompile()
        { //En este mensaje si no estan los tags voy a mostrarlo, voy a decir q si los quieres añadir
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            //Get unloaded Tags & Layers
            foreach (RequiredData dataType in Enum.GetValues(typeof(RequiredData)))
            {
                object[] rawData = null; //Aqui vendria bien hacer unos cuantos polimorfismos...
                if (!GetRequiredData(dataType, out rawData))
                    return;

                IEnumerable<string> reqData = ((NamedData[])rawData).Select(z => z.Name),
                                    neededData = reqData.Where(x => !CheckData(x, dataType));

                //Before we replace anything, we will save the new Tags & Layers

                object[] newData = GetDefinedData(dataType);
                int i = 0;

                foreach (object data in newData)
                    if (!reqData.Contains(GetDataName(dataType, data)))
                    {
                        AddData(dataType, data);
                        ++i;
                    }

                if (i > 0) //If it was modified then update it.
                    SaveData(dataType);

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

        private static void AddLayer(LayerData data)
        {
            List<LayerData> lList = new List<LayerData>();
            if (Layers != null)
                lList = Layers.ToList();
            lList.Add(data);
            _layers = lList.ToArray();
        }

        private static void AddTag(NamedData data)
        {
            List<NamedData> tList = new List<NamedData>();
            if (Layers != null)
                tList = Tags.ToList();
            tList.Add(data);
            _tags = tList.ToArray();
        }

        private static void AddData(RequiredData data, object obj)
        {
            switch (data)
            {
                case RequiredData.Layers:
                    AddLayer((LayerData)obj);
                    break;

                case RequiredData.Tags:
                    AddTag((NamedData)obj);
                    break;
            }
        }

        private static void SaveData(RequiredData data)
        {
            string path = GetFileNameFromData(data);
            switch (data)
            {
                case RequiredData.Tags:
                    File.WriteAllText(path, JsonUtility.ToJson(_tags));
                    break;

                case RequiredData.Layers:
                    File.WriteAllText(path, JsonUtility.ToJson(_layers));
                    break;
            }
        }

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
            switch (rData)
            {
                case RequiredData.Layers:
                    data = Layers;
                    break;

                case RequiredData.Tags:
                    data = Tags;
                    break;
            }
            data = null;
            return data != null;
        }

        private static string GetFileNameFromData(RequiredData rData)
        {
            string str = Path.Combine(Application.dataPath, "Lerp2API");
            switch (rData)
            {
                case RequiredData.Tags:
                    str = Path.Combine(str, "RequiredTags.json");
                    break;

                case RequiredData.Layers:
                    str = Path.Combine(str, "RequiredLayers.json");
                    break;
            }
            return str;
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

        private static object[] GetDefinedData(RequiredData data)
        {
            switch (data)
            {
                case RequiredData.Tags:
                    return EditorHelpers.GetDefinedTags();

                case RequiredData.Layers:
                    return EditorHelpers.GetDefinedLayers();
            }
            return null;
        }

        private static string GetDataName(RequiredData data, object obj)
        {
            return ((NamedData)obj).Name;
        }
    }
}