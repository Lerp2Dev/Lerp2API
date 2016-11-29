using UnityEditor;
using UnityEngine;
using System.IO;
using Lerp2API.Utility;
using Lerp2API;

[InitializeOnLoad]
public class DownloadAPI
{
#if !LERP2API
	internal static string[] updateUrl = new string[] {
            "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Lerp2API.dll",
            "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Lerp2API.pdb",
            "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Lerp2API.xml" };

    private const string menuName = "Lerp2Dev Team Tools/Download Lerp2Dev API";
    private const string section = "DOWNLOAD_API";

    private static bool active
    {
        get
        {
            return LerpedCore.GetBool(section);
        }
    }

    static DownloadAPI() 
    {
        if(!active) 
        {
            int option = EditorUtility.DisplayDialogComplex("Asset message",
                                        "It seems you are using some Lerp2Dev content, do you want to download the Lerp2Dev Team API?",
                                        "Yes",
                                        "No",
                                        "Don't remind it more" );
            switch(option) 
            {
                case 0:
                    DownloadAPI();
                    break;
                case 2:
                    LerpedCore.SetBool(section, !active);
                    break;
                default:
                    break;
            }
        }
        //LerpedCore.SetBool(section, !active);
    }

	[MenuItem(menuName)]
	public static void DownloadAPI() 
	{
		string location = Path.Combine(Application.dataPath, "Lerp2API/Lerp2API.dll"),
			   path = location.Replace(".dll", ".pdb"),
               path1 = location.Replace(".dll", ".xml");
		WWW www = new WWW(updateUrl[0]);
        WWW www2 = new WWW(updateUrl[1]);
        WWW www3 = new WWW(updateUrl[2]);
        WWWHandler.Handle(www, () => 
        {
            File.WriteAllBytes(location, www.bytes);
        });
        WWWHandler.Handle(www2, () => 
        {
            File.WriteAllBytes(path, www2.bytes);
        });
        WWWHandler.Handle(www3, () =>
        {
            File.WriteAllBytes(path1, www3.bytes);
        });
        Debug.LogFormat("API downloaded successfully!");
        AssetDatabase.Refresh();
	}
#endif
}
