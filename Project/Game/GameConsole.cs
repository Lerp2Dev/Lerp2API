using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using Lerp2API.Game;
using Lerp2API.Controllers.Utils;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2API.Game
{
    public enum Types { Unknown, Int, String, Float, Bool }
    public enum Styles { None, Bold, Italic, Color }
    public enum SStyles { None, Underline, Obfuscated, Strike }
    [Serializable]
    public class Command //This more like a read-only class & cannot be inhretied from GameConsole because if not you will lost the attributes of the editor (I have to make a custom editor class to avoid this and inhetir without problems)
    {
        public string name,
                      command,
                      linkedMethod,
                      generalHelp;
        public string[] aliases;
        public ParamSet[] paramSets; //This is a constant from every command it's not established (and I don''t want to make it a propery because of the Unity Inspector)
        public bool ParseParams(string chatParams, ref Param[] listOfParams, ref int posUsed)
        {
            if (string.IsNullOrEmpty(chatParams))
                return false;
            string[] splitParams = chatParams.Split(' ');
            if (chatParams.Length == 0)
                return false; //In theory, this is not needed because, it's checked before it's called
            Param[] set = new Param[splitParams.Length];
            bool recognized = ParamSet.ParamSetRecognizion(splitParams, this, ref set, ref posUsed);
            if (recognized)
                listOfParams = set;
            return recognized;
        }
        public void DisplayHelp()
        {
            GameConsole.AddMessage(string.Format("Showing help from {0} command [/{1}]", name, command));
            GameConsole.AddMessage(new string('-', 15)); //I have to make a pagination system for this
            if (paramSets.Length > 0)
            {
                GameConsole.AddMessage(string.Format("There are {0} available command forms for /{1}. There are some explanations about them:", paramSets.Length, command));
                GameConsole.AddMessage(string.Format("/{0} help, ? or nothing - Shows help about this command", command));
                foreach (ParamSet ps in paramSets)
                    GameConsole.AddMessage(string.Format("/{0} {1}", command, ps));
            }
            else
                GameConsole.AddMessage("There are not command forms for this.");
            GameConsole.AddMessage(new string('-', 15));
        }
    }
    [Serializable]
    public class ParamSet
    {
        public Param[] paramSet;
        public string helpString;
        public ParamSet(Param[] ps) : this(ps, "There is not information about this param.") { }
        public ParamSet(Param[] ps, string hs)
        {
            paramSet = ps;
            helpString = hs;
        }
        public override string ToString()
        {
            return string.Format("{0} - {1}", string.Join(" ", paramSet.Select(x => x.ToString()).ToArray()), helpString);
        }
        public static bool ParamSetRecognizion(string[] chatParams, Command c, ref Param[] set, ref int posUsed)
        {
            if (chatParams.Length == 0)
                return true; //If there isn't any param defined exit the function
            if (set == null || set != null && set.Length == 0)
                set = new Param[chatParams.Length]; //If the set var is null or the array is empty, create a new array with a defined length
            int i = 0, j = 0, k = 0, l = 0;
            foreach (string cp in chatParams)
            { //Loop every chatparams to create a param system for a later use, we haven0t to create information (like the param name) because it's only info, this could be bring me problems in a near future
                set[i] = GetParam(cp);
                ++i;
            }
            List<List<Types>> types = new List<List<Types>>();
            foreach (ParamSet ps in c.paramSets)
            { //Now, loop every paramset in search of all the params to compare it later and make a recognition
                types.Add(new List<Types>());
                foreach (Param p in ps.paramSet)
                {
                    types[j].Add(p.varType);
                    ++k;
                }
                ++j;
            }
            bool rec = false;
            foreach (List<Types> lt in types)
            { //One time, we get all the type forms from a command, loop it, and try to find a match
                Types[] ltt = set.Select(x => x.varType).ToArray();
                if (string.Join(" ", ltt.Select(x => x.ToString()).ToArray()) == string.Join(" ", lt.ToArray().Select(x => x.ToString()).ToArray()))
                { //If there any match and everything goes correct, this should happen
                    rec = true;
                    posUsed = l;
                    break;
                }
                ++l;
            }
            if (!rec)
                set = null;
            return rec;
        }
        internal static Param GetParam(string param)
        {
            int rInt;
            decimal rFloat;
            bool rBool;
            if (int.TryParse(param, out rInt))
                return new Param(GetMainType(rInt.GetType().Name), rInt);
            else if (decimal.TryParse(param, out rFloat))
                return new Param(GetMainType(rFloat.GetType().Name), rFloat);
            else if (bool.TryParse(param, out rBool))
                return new Param(GetMainType(rBool.GetType().Name), rBool);
            else
                return new Param(GetMainType(param.GetType().Name), param);
        }
        internal static Types GetMainType(string name)
        {
            if (name == typeof(int).Name)
                return Types.Int;
            else if (name == typeof(decimal).Name)
                return Types.Float;
            else if (name == typeof(string).Name)
                return Types.String;
            else if (name == typeof(bool).Name)
                return Types.Bool;
            else
                return Types.Unknown;
        }
    }
    [Serializable]
    public class Param
    {
        public Types varType;
        public UniCast obj;
        public string paramName;
        public Param(Types t, UniCast o) : this(t, o, "unknown") { }
        public Param(Types t, UniCast o, string pn)
        {
            o.currentType = t;
            varType = t;
            obj = o;
            paramName = pn;
        }
        public override string ToString()
        {
            return string.Format("<{0}>", paramName);
        }
    }
    [Serializable]
    public class UniCast
    {
        public Types currentType;
        private string sobj;
        private int iobj;
        private decimal dobj;
        private bool bobj;
        public UniCast(string s)
        {
            sobj = s;
        }
        public UniCast(int i)
        {
            iobj = i;
        }
        public UniCast(decimal d)
        {
            dobj = d;
        }
        public UniCast(bool b)
        {
            bobj = b;
        }
        public static implicit operator UniCast(string s)
        {
            return new UniCast(s);
        }
        public static explicit operator string(UniCast uc)
        {
            return uc.sobj;
        }
        public static implicit operator UniCast(int i)
        {
            return new UniCast(i);
        }
        public static explicit operator int(UniCast uc)
        {
            return uc.iobj;
        }
        public static implicit operator UniCast(decimal d)
        {
            return new UniCast(d);
        }
        public static explicit operator decimal(UniCast uc)
        {
            return uc.dobj;
        }
        public static implicit operator UniCast(bool b)
        {
            return new UniCast(b);
        }
        public static explicit operator bool(UniCast uc)
        {
            return uc.bobj;
        }
    }
    public class GameConsole : LerpedCore
    {
        public static GameConsole me;
        public static CommandMethods cm;

        //Monobehaviour props
        public int width = 300,
                   height = 200;
        public Position m_consolePosition;
        public Command[] commandList;
        public bool m_showCursor,
                    m_disableDebug;

        //API Props
        private static Dictionary<int, FMessage> log = new Dictionary<int, FMessage>();

        private static Vector2 consolePos = Vector2.zero;
        private static string chatContent = "";
        private static bool chatEnabled;

        internal static float _long;
        internal const int keySeek = 2, 
                           ScrollMult = 10;
        internal static ulong nextSeek = 0, 
                              keyTicks = 0;

        internal static GUIStyle _text; //Fix this guistyle and put shadows to the words
        internal static float _error = 8,
                              _error2 = 4;

        internal static Rect lastRect;
        internal static KeyCode curKey;
        internal static int lastRandKey = 0,
                            randCycle = 0;

        //Ugly fixes
        internal static bool alreadyGoHistory,
                             focusTField;

        //Monobehaviour methods

        private void Start()
        {
            if (gameObject.GetComponent<CommandMethods>() == null)
                Init(); //This to avoid that the inherited class calls again start method... I have to improve this again
        }

        private void Init()
        { //There, we have to load the commands from JSON file
            me = this;
            cm = gameObject.AddComponent<CommandMethods>();
            _text = new GUIStyle("label") { wordWrap = true, richText = true };
            Debug.isGamingEnabled = !m_disableDebug;
        }

        private void Update()
        {
            //Nothing to do here...
            if (curKey == KeyCode.T || curKey == KeyCode.Return || curKey == KeyCode.Escape)
            {
                if (!chatEnabled)
                {
                    if (curKey == KeyCode.T)
                    { //I have to check for a keycode event in update
                        ShowChat();
                        StartCoroutine(FocusChatTField()); //Maybe could be good to check for the last focused control to avoid unncesary calls...
                    }
                }
                else
                {
                    bool w = !string.IsNullOrEmpty(chatContent);
                    if (w && curKey == KeyCode.Escape || !w && curKey == KeyCode.Return)
                        HideChat();
                    else if (w && curKey == KeyCode.Return)
                        SendMessage();
                }
                Cursor.visible = m_showCursor;
                if (!m_showCursor && Cursor.lockState == CursorLockMode.None)
                    Cursor.lockState = CursorLockMode.Locked;
                else if (m_showCursor && Cursor.lockState == CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.None;
            }
            if (chatEnabled)
            {
                if(Message.LogHistory.Count > 0)
                    if (!alreadyGoHistory && (curKey == KeyCode.UpArrow || curKey == KeyCode.DownArrow))
                    {
                        StartCoroutine(LockHistory());
                        if (curKey == KeyCode.DownArrow)
                        {
                            if (Message.currentIndex < Message.LogHistory.Count - 1)
                                ++Message.currentIndex;
                        }
                        else if (curKey == KeyCode.UpArrow)
                        {
                            if (Message.currentIndex > 0)
                                --Message.currentIndex;
                        }
                        Message.Show();
                    }
                if (Input.mouseScrollDelta != Vector2.zero)
                    NavigateChat(-Input.mouseScrollDelta.y * ScrollMult);
            }
        }

        private void OnGUI()
        {
            Event e = Event.current;
            if (keyTicks < nextSeek)
                curKey = e.keyCode;
            else
                curKey = KeyCode.None;
            if (e.isKey)
                nextSeek = keyTicks + keySeek;
            ++keyTicks;
            Draw();
        }

        //Coroutines works perfectly

        internal static IEnumerator FocusChatTField()
        { //This focus the text field after Unity passed one tick, ensuring that any exception will occur...
            yield return new WaitForSeconds(UnityTick);
            focusTField = true;
        }

        internal static IEnumerator LockHistory()
        { //This is to avoid double key presses... Matbe we will need to use it in more parts of the code...
            alreadyGoHistory = true;
            yield return new WaitForSeconds(UnityTick * 2);
            alreadyGoHistory = false;
        }

        //Chat methods

        internal static void ShowChat()
        {
            chatEnabled = true;
        }

        internal static void HideChat()
        {
            chatEnabled = false;
            Message.firstTime = false;
        }

        internal static void NavigateChat(float d)
        {
            if (d < 0)
            {
                if (consolePos.y > 0)
                    consolePos -= new Vector2(0, Mathf.Abs(d));
            }
            else
            {
                if (consolePos.y < _long)
                    consolePos += new Vector2(0, d);
            }
        }

        public static void SendMessage(string input = "", bool resetForce = false)
        {   //Check, if input is null, if yes, use input, if not, use chatcontent
            string str = string.IsNullOrEmpty(input) ? chatContent : input;
            if (str.IsEmptyOrWhiteSpace())
                return; //If string is empty or whitespace exit the function and don't send any message
            //Start doing this...
            //First, we add it to the log history (before the text is modified), it's used later if we want to go back (up & down keys)
            Message.LogHistory.Add(str);
            //Thirdly, we have to check if the str is a command
            if (str.StartsWith("/")) //If yes execute a command
                ExecuteCommand(str.Substring(1));
            else //Else, we send it to the chat with any parses
                AddMessage(str);
            if (string.IsNullOrEmpty(input) || resetForce) //If we use chatContent or we want to force its reset, do it
                chatContent = ""; //To finalize, we reset chatcontent
            //For last, hide chat...
            if (chatEnabled)
                HideChat();
        }

        //API
        internal static string FormattingCodes(string text)
        {
            int i = 0,
                skippedPos = -1;
            StringBuilder t = new StringBuilder(text);
            Styles lastStyle = Styles.None;
            for (; i < text.Length; ++i)
            {
                if (i == skippedPos)
                    continue;
                if (text[i] == '§' || text[i] == '&')
                {
                    int rpos = text[i] == '§' ? t.ToString().IndexOf('§') : t.ToString().IndexOf('&');
                    switch (text[i + 1]) //This is the next char after § or &
                    { //I have to set also the color of the shadow...
                        case '0':
                            UnityEngine.Debug.Log(t.ToString().Substring(rpos)); //I have to fix this shit...
                            t = IntReplace(t, rpos, "<color=#000000FF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case '1':
                            t = IntReplace(t, rpos, "<color=#0000AAFF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case '2':
                            t = IntReplace(t, rpos, "<color=#00AA00FF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case '3':
                            t = IntReplace(t, rpos, "<color=#00AAAAFF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case '4':
                            t = IntReplace(t, rpos, "<color=#AA0000FF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case '5':
                            t = IntReplace(t, rpos, "<color=#AA00AAFF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case '6':
                            t = IntReplace(t, rpos, "<color=#FFAA00FF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case '7':
                            t = IntReplace(t, rpos, "<color=#AAAAAAFF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case '8':
                            t = IntReplace(t, rpos, "<color=#555555FF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case '9':
                            t = IntReplace(t, rpos, "<color=#5555FFFF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case 'a':
                            t = IntReplace(t, rpos, "<color=#55FF55FF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case 'b':
                            t = IntReplace(t, rpos, "<color=#55FFFFFF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case 'c':
                            t = IntReplace(t, rpos, "<color=#FF5555FF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case 'd':
                            t = IntReplace(t, rpos, "<color=#FF55FFFF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case 'e':
                            t = IntReplace(t, rpos, "<color=#FFFF55FF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case 'f':
                            t = IntReplace(t, rpos, "<color=#FFFFFFFF>", lastStyle);
                            lastStyle = Styles.Color;
                            break;
                        case 'l':
                            t = IntReplace(t, rpos, "<b>", lastStyle);
                            lastStyle = Styles.Bold;
                            break;
                        case 'o':
                            t = IntReplace(t, rpos, "<i>", lastStyle);
                            lastStyle = Styles.Italic;
                            break;
                        case 'r': //Reset
                            t.Insert(rpos, CloseTag(lastStyle));
                            lastStyle = Styles.None;
                            break;
                    }
                    skippedPos = i + 1;
                }
            }
            if (lastStyle != Styles.None)
                t.Insert(t.ToString().Length, CloseTag(lastStyle));
            return t.ToString();
        }

        internal static StringBuilder IntReplace(StringBuilder sb, int pos, string str, Styles ls)
        {
            string realStr = CloseTag(ls) + str;
            sb.Remove(pos, 2);
            sb.Insert(pos, realStr);
            return sb;
        }

        internal static string CloseTag(Styles style)
        {
            switch (style)
            {
                case Styles.Color:
                    return "</color>";
                case Styles.Bold:
                    return "</b>";
                case Styles.Italic:
                    return "</i>";
                default:
                    return "";
            }
        }

        internal static void AddNewLine()
        {
            AddMessage("");
        }

        internal static void AddMessage(string message, params string[] pars)
        {
            AddMessage(string.Format(message, pars));
        }

        internal static void AddMessage(string message)
        {
            AddFormattedMessage(message, Color.white);
        }

        internal static void AddFormattedMessage(string message, Color color, bool bold = false, bool italics = false)
        {
            //CANNOT REFENCE ANY L2API.DEBUG METHOD HERE BECAUSE THIS CAN CAUSE AN INFINITE LOOP THAT MAKE UNITY TO FREEZE
            //Parse styles inside string
            message = FormattingCodes(message);
            //Then, add styles to the message
            message = string.Format("{0}{1}{2}", string.Format("<color=#{0}>", color.ColorToHex()), message, "</color>");
            if (bold)
                message = string.Format("{0}{1}{2}", "<b>", message, "</b>");
            if (italics)
                message = string.Format("{0}{1}{2}", "<i>", message, "</i>");
            //Then, add message
            Message.Add(message);
            //Finally, calculating the new height
            _long += _text.CalcHeight(new GUIContent(message), me.width) + _error + _error2;
        }

        internal static void ExecuteCommand(string command)
        {
            if (command.IndexOf(' ') >= 0)
                command = Regex.Replace(command, @"\s+", " "); //Remove repeated chars, for example: "Hello,   how are  you? " into "Hello, how are you?"
            if (command.EndsWith(" "))
                command = command.Substring(0, command.Length - 1); //Look for last char as space, to rem it
            string gettedCommand = command.IndexOf(' ') >= 0 ? command.Substring(0, command.IndexOf(' ')) : command, //Get command name
                   commandParams = command.IndexOf(' ') >= 0 ? command.Substring(gettedCommand.Length + 1) : ""; //Get the command params
            UnityEngine.Debug.Log("Searching for command /" + gettedCommand);
            Command c = null;
            Param[] set = null;
            foreach (Command cm in me.commandList)
            {
                if (cm.command == gettedCommand)
                {
                    c = cm;
                    break;
                }
                bool br = false;
                foreach (string al in cm.aliases)
                    if (al == gettedCommand)
                    {
                        c = cm;
                        br = true;
                        break;
                    }
                if (br)
                    break;
            }
            if (c == null)
            {
                AddMessage("Command not found!"); //There would be recomendable to send suggested commands (I have to).
                return;
            }
            if (commandParams.Length > 0 && c.paramSets.Length > 0)
            {
                int posUsed = -1;
                if (c.ParseParams(commandParams, ref set, ref posUsed))
                    CommandMethods.ProcessCommand(c.linkedMethod, c, set, posUsed);
                else
                {
                    AddMessage("Command form not recognized! Showing help below:");
                    c.DisplayHelp();
                }
            }
            else
                CommandMethods.ProcessCommand(c.linkedMethod, c, null, -1);
        }

        internal static FontStyle GetFontStyle(bool bold, bool italics)
        {
            FontStyle s = FontStyle.Normal;
            if (bold)
                s = FontStyle.Bold;
            else if (italics)
                s = FontStyle.Italic;
            else if (bold && italics)
                s = FontStyle.BoldAndItalic;
            return s;
        }

        public static void Draw(float maxQueries = Mathf.Infinity)
        {
            Rect r = me.m_consolePosition.GetPosition(me.width, me.height);
            if (chatEnabled)
            {
                GUILayout.BeginArea(new Rect(r.xMin, r.yMin, r.xMax + 5, r.yMax));
                GUILayout.Box("", GUILayout.Width(r.width + 5), GUILayout.Height(r.height));
                GUILayout.EndArea();
            }
            GUILayout.BeginArea(new Rect(r.xMin + (chatEnabled ? 5 : 0), r.yMin + (chatEnabled ? 5 : 0), r.width, r.height - 10));
            if (chatEnabled)
            {
                GUILayout.BeginVertical();
                consolePos = GUILayout.BeginScrollView(chatEnabled ? consolePos : Vector2.zero, false, chatEnabled, GUILayout.Width(r.width - 5), GUILayout.Height(r.height - 35));
            }
            else
                GUILayout.BeginArea(new Rect(0, -(_long < me.height ? 0 : _long - me.height), r.width, _long));
            if (log != null && log.Count > 0)
                for (int i = 0; i < log.Count; ++i)
                {
                    FMessage fm = log[i];
                    string m = fm.message;
                    float h = _text.CalcHeight(new GUIContent(m), me.width - (chatEnabled ? 30 : 0));
                    //GUILayout.Label(GetRandomness(fm), GUILayout.Width(me.width - (chatEnabled ? 30 : 0)), GUILayout.Height(h + _error));
                    //I have to put an option to disable this option...
                    //I have to fix the shadow
                    ShadowAndOutline.DrawLayoutShadow(new GUIContent(GetRandomness(fm)), _text, Color.black, Color.black, Vector2.down + Vector2.left, GUILayout.Width(me.width - (chatEnabled ? 30 : 0)), GUILayout.Height(h + _error));
                    //Searching for untracked styles here...
                    //I have to fix for example this kind of strings "aaa&nxdxd&0xd" that is bugged
                    DrawUnderline(GUILayoutUtility.GetLastRect(), fm);
                    if (i >= maxQueries)
                        break;
                }
            if (chatEnabled)
            {
                GUILayout.EndScrollView();
                GUILayout.FlexibleSpace();
                GUI.SetNextControlName("chatField");
                chatContent = GUILayout.TextField(chatContent);
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
            else
                GUILayout.EndArea();
            GUILayout.EndArea();
            if (focusTField)
            {
                GUI.FocusControl("chatField");
                focusTField = false;
            }
        }

        protected static void DrawUnderline(Rect r, FMessage fm)
        {
            string s = fm.unformatted;
            foreach (KeyValuePair<int, FText> kv in fm.formatted)
            {
                GUIContent gc = new GUIContent(s.Substring(0, kv.Key - 1));
                GetUnderline(r, new Vector2(_text.CalcSize(gc).x, _text.CalcHeight(gc, me.width)), kv.Key, s, s.Substring(kv.Key, kv.Value.len), kv.Value.style);
            }
        }

        protected static void GetUnderline(Rect r, Vector2 p, int rpos, string os, string s, SStyles ss)
        { //I have to fix this
            float mm = (new GUIStyle(_text) { wordWrap = false }).CalcSize(new GUIContent(os)).x / os.Length; //Get the average that letters measures in px
            int letters = Mathf.FloorToInt((me.width - 5) / (mm + 2)); //Get how many letter occupies one paragraph
            float w = s.Length * mm, //Get the underlined string length in px
                  wn = (letters - rpos % letters) * mm, //Get the left space that is after the first paragraph
                  factor = (((me.width - 5) - wn) / rpos), //Get in another way, another more accurately letter-width
                  rst = (rpos % letters) * factor, //Get how many space we have to displace to the right
                  rw = letters * factor; //Another accurately way of getting the paragraph width
            int times = (int)Math.Ceiling(w / me.width); //Get how many paragrpahs has to be underlined
            if (times == 1) //If there is only one paragraph, only get the margin-left and the width calculated getting the letters that remain on a paragraph
                GetTimes(1, r, p, rst, w, ss);
            else if (times == 2) //If there is 2 paragraph, do the same for the first and for the second calculate the remaining space we have calculated before
                for (int k = 0; k < times; ++k)
                    GetTimes(k + 1, r, p, rst, k == 0 ? wn : w - wn, ss);
            else if (times > 2) //If there is 3 paragraphs, calculate the space in the middle that is always the full chat
                for (int j = 0; j < times; ++j)
                    GetTimes(j + 1, r, p, rst, j == 0 ? wn : (j < times - 1 ? rw : w - wn), ss);
        }

        protected static void GetTimes(int t, Rect r, Vector2 p, float rst, float w, SStyles ss, bool more = false)
        {
            Rect rn = r;
            if (t == 1)
                rn.x += rst;
            rn.y += p.y + (p.y - 6) * (t - 1) + lastRect.height - (ss == SStyles.Underline ? 5 : 13);
            rn.width = w;
            rn.height = 2; //Thickness of the line
            GUI.DrawTexture(rn, Texture2D.whiteTexture);
        }

        /*
         Rect rn = r;
                    rn.x += rst;
                    rn.y += p.y + lastRect.height - (ss == SStyles.Underline ? 5 : 13); //Vertical alignment of the underline
                    rn.width = w;
                    rn.height = 2; //Thickness of the line
                    GUI.DrawTexture(rn, Texture2D.whiteTexture);
             
             */

        protected static string RemoveUndesiredTags(string s)
        {
            return Regex.Replace(Regex.Replace(s, "<color=#[A-Fa-f0-9]{6,8}>", ""), @"<(\/|)(b|i|color)>", "");
        }

        protected static string GetRandomness(FMessage fm)
        {
            if (randCycle >= 256)
                randCycle = 0;
            if (fm.randomness.Count == 0)
                return fm.message;
            StringBuilder sb = new StringBuilder(fm.message);
            foreach (KeyValuePair<int, FText> kv in fm.randomness)
            {
                sb.Remove(kv.Key, kv.Value.len);
                sb.Insert(kv.Key, GetRandString(kv.Value.len));
            }
            ++randCycle;
            lastRandKey = 0;    
            return sb.ToString();
        }

        protected static string GetRandString(int len)
        {
            string s = "";
            for(int i = 0; i < len; ++i)
            {
                int sum = randCycle + lastRandKey;
                if (sum >= 256)
                {
                    sum -= randCycle;
                    randCycle = 0;
                }
                s += (char)sum;
                ++lastRandKey;
                if (lastRandKey >= 256)
                    lastRandKey = 0;
            }
            return s;
        }

        public static void Clear()
        {
            log = new Dictionary<int, FMessage>();
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private static void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (Application.isPlaying && Debug.isGamingEnabled)
                AddFormattedMessage(logString + "\n\n" + stackTrace, GetColorFromLogType(type));
        }

        private static Color GetColorFromLogType(LogType type)
        {
            Color c = Color.white;
            switch (type)
            {
                case LogType.Error:
                    c = DebugColor.error;
                    break;
                case LogType.Assert:
                    c = DebugColor.assertion;
                    break;
                case LogType.Warning:
                    c = DebugColor.warning;
                    break;
                case LogType.Log:
                    c = DebugColor.normal;
                    break;
                case LogType.Exception:
                    c = DebugColor.exception;
                    break;
            }
            return c;
        }

        protected class Message
        {
            public static int currentIndex = 0;
            public static List<string> LogHistory = new List<string>();
            public static bool firstTime;

            public static void Add(FMessage fm)
            {
                log.Add(log.Count, fm);
            }

            public static void Show()
            {
                if (LogHistory.Count == 0)
                    return;
                if(!firstTime)
                {
                    currentIndex = LogHistory.Count - 1;
                    firstTime = true;
                }
                try
                { //This is only for testing... Normally, it works correctly
                    chatContent = LogHistory[currentIndex];
                }
                catch (Exception e)
                {
                    Debug.LogError(currentIndex + " " + LogHistory.Count);
                }
            }
        }

        protected class FMessage
        {
            public string message = "", unformatted = "";
            public Dictionary<int, FText> formatted = new Dictionary<int, FText>();
            public Dictionary<int, FText> randomness
            {
                get
                {
                    return formatted.Where(x => x.Value.style == SStyles.Obfuscated).ToDictionary(x => x.Key, x => x.Value);
                }
            }
            //public Dictionary<int, int> randomness = new Dictionary<int, int>();
            private FMessage(string m)
            {
                message = m;
                formatted = GetComputedFormat(message);
                //randomness = GetRandomness(message);
            }
            //I have to try to combine this two functions from below
            internal Dictionary<int, FText> GetComputedFormat(string s)
            {
                unformatted = RemoveUndesiredTags(s);
                Dictionary <int, FText> lcal = new Dictionary<int, FText>();
                int i = 0,
                    skipPos = -1,
                    lPos = -1;
                SStyles lastStyle = SStyles.None;
                StringBuilder sb = new StringBuilder(unformatted);
                bool active = false;
                //I need to get where are the <u> and <s>
                for (; i < s.Length; ++i)
                {
                    if (i == skipPos)
                        continue;
                    if (s[i] == '§' || s[i] == '&')
                    {
                        int rpos = s[i] == '§' ? sb.ToString().IndexOf('§') : sb.ToString().IndexOf('&');
                        skipPos = i + 1;
                        if (s[skipPos] == 'm' || s[skipPos] == 'n' || s[skipPos] == 'k')
                        {
                            active = true;
                            sb.Remove(rpos, 2); //I have to make something to mix the styles inside
                            lastStyle = GetStyle(s[i + 1]);
                            if (lPos == -1)
                            {
                                lPos = rpos;
                                continue;
                            }
                            lcal.Add(rpos, new FText(rpos - lPos, lastStyle));
                            lPos = rpos;
                        }
                    }
                    if (GetNewTag(sb.ToString(), i))
                        active = false;
                }
                if (active)
                    lcal.Add(lPos, new FText(GetEnd(sb.ToString(), lPos), lastStyle));
                //foreach (KeyValuePair<int, FText> t in lcal)
                //    UnityEngine.Debug.LogFormat("{0} {1}", t.Key, t.Value.ToString());
                message = Regex.Replace(s, "(&|§)(n|m|k)", "");
                return lcal;
            }
            /*internal Dictionary<int, int> GetRandomness(string s)
            {
                unformatted = RemoveUndesiredTags(s);
                Dictionary<int, int> lcal = new Dictionary<int, int>();
                int i = 0,
                    skipPos = -1,
                    lPos = -1;
                StringBuilder sb = new StringBuilder(unformatted);
                bool active = false;
                //I need to get where are the <u> and <s>
                for (; i < s.Length; ++i)
                {
                    if (i == skipPos)
                        continue;
                    if (s[i] == '§' || s[i] == '&')
                    {
                        int rpos = s[i] == '§' ? sb.ToString().IndexOf('§') : sb.ToString().IndexOf('&');
                        skipPos = i + 1;
                        if (s[skipPos] == 'k')
                        {
                            active = true;
                            sb.Remove(rpos, 2);
                            if (lPos == -1)
                            {
                                lPos = rpos;
                                continue;
                            }
                            lcal.Add(rpos, rpos - lPos);
                            lPos = rpos;
                        }
                    }
                    if (GetNewTag(s, i))
                        active = false;
                }
                if(active)
                    lcal.Add(lPos, GetEnd(s, lPos));
                message = Regex.Replace(s, "(&|§)k", "");
                return lcal;
            }*/
            internal static bool GetNewTag(string s, int i)
            {
                if (i >= s.Length - 14)
                    return false;
                if (s.Substring(i, 14).StartsWith("<color"))
                    return true;
                if (i >= s.Length - 3)
                    return false;
                //I have to add the case for another "&"
                return Regex.IsMatch(s.Substring(i, 3), "<(b|i)>");
            }
            internal static int GetEnd(string s, int lp)
            {
                if (s.IndexOf("<", lp) > -1)
                    return s.IndexOf("<", lp) - lp - 2;
                //I have to add the case for another "&"
                return s.Length - lp;
            }
            internal static SStyles GetStyle(char l)
            {
                switch(l)
                {
                    case 'm':
                        return SStyles.Strike;
                    case 'n':
                        return SStyles.Underline;
                    case 'k':
                        return SStyles.Obfuscated;
                    default:
                        return SStyles.None;
                }
            }
            public static implicit operator FMessage(string s)
            {
                return new FMessage(s);
            }
            public static explicit operator string(FMessage fm)
            {
                return fm.message;
            }
        }

        protected class FText
        {
            public int len;
            public SStyles style;
            public FText(int l, SStyles ss)
            {
                len = l;
                style = ss;
            }
            public override string ToString()
            {
                return string.Format("LEN: {0}; STYLE: {1}", len, style);
            }
        }
    }
}

//Commands from API
public partial class CommandMethods : GameConsole
{
    public static CommandMethods me;
    private void Awake()
    {
        me = GetComponent<CommandMethods>();
    }
    public static void ProcessCommand(string name, Command c, Param[] set, int posUsed)
    {
        int[] lSet = null;
        if (c != null)
            lSet = GameConsole.me.commandList.FirstOrDefault(x => x.name == c.name).paramSets.Select(x => x.paramSet.Length).ToArray();
        if (!lSet.Any(x => x == c.paramSets.Length))
            c.DisplayHelp();
        else if (set.Length == 1 && set[0].obj.currentType == Lerp2API.Game.Types.String && ((string)set[0].obj == "help" || (string)set[0].obj == "?"))
            c.DisplayHelp();
        else
            me.SendMessage(name, new object[] { c, set, posUsed });
    }
    //This are the methods from the commands of the API
    //Use of obj param is totally neccesary
    //Alternative, to overloaded parameters is to make an default case
    public void Help(object[] obj)
    {
        Command[] list = GameConsole.me.commandList;
        if (obj != null)
        {
            Param[] set = (Param[])obj[1];
            if (set == null || set.Length == 0)
            {
                Help(null);
                return;
            }
            switch (set.Length)
            {
                case 1:
                    string c = (string)set[0].obj;
                    if (c.ToLower() == "help") //I have to improve this
                        return;
                    bool f = false;
                    foreach (Command cm in list)
                        if (cm.command.ToLower() == c.ToLower())
                        {
                            cm.DisplayHelp();
                            f = true;
                            break;
                        }
                    if (!f)
                        AddMessage("Not found command name by '{0}'! Showing general help:", c);
                    Help(null);
                    break;
                default:
                    ((Command)obj[0]).DisplayHelp();
                    break;
            }
        }
        else
        { //I have to create an pagination for this also
            AddMessage(new string('-', 15));
            AddMessage("Showing {0} commands:");
            foreach (Command cm in list)
                AddMessage("/{0} - {1}", cm.command, cm.generalHelp);
            AddMessage(new string('-', 15));
        }
    }
    public void DebugSw(object[] obj)
    {
        SetBool("ENABLE_DEBUG", !GetBool("ENABLE_DEBUG"));
        AddMessage("Debug has been {0}!", GetBool("ENABLE_DEBUG") ? "enabled" : "disabled");
    }
    public void Teleport(object[] obj)
    {
        Param[] set = (Param[])obj[1];
        switch (set.Length)
        {
            case 2:
                Teleporter.Teleport((float)set[0].obj, (float)set[1].obj);
                break;
            case 3:
                Teleporter.Teleport((float)set[0].obj, (float)set[1].obj, (float)set[2].obj);
                break;
            case 4:
                Teleporter.Teleport((float)set[0].obj, (float)set[1].obj, (float)set[2].obj, (float)set[3].obj);
                break;
            case 5:
                Teleporter.Teleport((float)set[0].obj, (float)set[1].obj, (float)set[2].obj, (float)set[3].obj, (float)set[4].obj);
                break;
        }
    }
}