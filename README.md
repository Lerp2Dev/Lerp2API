#Lerp2API#
Our API for Unity. It contains several features to make easier your coding in this Engine.
## Content ##
As example, we will mention the folders that my API has and explain them:

 - **All Noises** folder has two subfolders:
	 - **Noises** folder has all the Noise generators for my Terrain Asset (specially, to do height-map).
	 - **Voronoi** folder, it is used to delimit Biome (also, from my Terrain Asset).
 - **Controllers**, and all they subfolders are scripts fro Standard Assets of Unity.
 - **Game** folder has hand-made scripts, by me, they have a FPS Counter and a basic Game Console.
 - **MenuSrc** folder has an Utility to enable and disable Debug from UnityEngine and Unity Analytics script (I prefer to use it in my script, in your, you can disable it)
 - **Other APIs** folder has three big APIs: BigInteger (to allow Unity has higher values from ulong capacities), HtmlAgilityPack (to manage HTML from website easily, I use it on my Unity Wiki in Editor Asset) and UniGif (to display gifs in Unity GUI)
 - **Serializers** folder has another two tools (yep, they are Serializers) that works together to make everything from **Unity Scenes** be saved and stored successfully in our HDD or SDD.
 - **Utility** folder, has some utilities from Unity Standard Assets and Unity Community (like, something to work with Directives References, other script to make Debug Rays and Lines be available in Playmode, other thing to work with Window Context Menus easily, a Mesh Serializer and other tool that I made to make users Identificable by their Unique Computer ID, I will use this in my [Website](http://lerp2dev.com/))

## Usage ##

Just drop Lerp2API.dll and Lerp2API.pdb in a folder called Lerp2API in a the root of your Project Asset folder. 

Documentation (not available yet)
Manual (not available yet)

## Next features ##

The ability of save every scene in Unity.
Maybe I will add CS-Script Asset (it's to load .cs files at runtime), yep, Unity modding with my API is coming stronger and stronger. (Serializers do a good job, but they need something more :P).
I'm searching some way to avoid Floating Point precision errors, I founded some time ago, Floating Origin, but I am not sure about it (and it's compatibility in Multiplayer).
An installer for my API (this will use to install the API in your projects in a easy way).
Debug Enabled/Disable option in-game.

## Known issues ##

 - The menu checkmark from Debug Enabler doesn't get marked when Editor is recompiled and it's active in the config file.

## Changelogs ##

Version 1.0a:

 - Initial release.

Version 1.0.1a:

 - Splitted the dll into Editor and Game part.
 - Added internal XML documentation file.
 - Cleaned some unused references.
 - Added some more utilities to the repo.
 - Fixed some errors in Editor.

## Do you want to make an API? ##

Tutorial is here: [http://lerp2dev.com/UnityDllReferencesTutorial/](http://lerp2dev.com/UnityDllReferencesTutorial/) (it's still in Spanish).

## Credits and thanks ##

Ikillnukes - I made all the scripts that are not listed below:

Unity Team for Standard Assets

[scgarland](https://www.codeplex.com/site/users/view/scgarland) thanks for [BigIntegers in Unity](http://biginteger.codeplex.com/) (a downgraded version from .NET FW 4 System.Numerics.BigIntegers Class).

[WestHill](http://forum.unity3d.com/members/westhill.145073/) thanks for [UniGif](http://forum.unity3d.com/threads/unigif-gif-image-decoder-for-unity-now-available-for-free-on-github.290126/).

[TheSniperFan](https://gitgud.io/TheSniperFan) thanks for [unityserializer-ng](https://gitgud.io/TheSniperFan/unityserializer-ng).

[jacobdufault](https://github.com/jacobdufault) thanks for [FullSerializer](https://github.com/jacobdufault/fullserializer).

[DarthObiwan](https://www.codeplex.com/site/users/view/DarthObiwan) thanks for [HtmlAgilityPack](https://htmlagilitypack.codeplex.com/).

Somebody for Noises folder (I did Voronoi Class by using [this resource](https://en.wikipedia.org/wiki/Voronoi_diagram)).

#### I hope you found this useful ####
Lerp2Dev Team

Ikillnukes~~