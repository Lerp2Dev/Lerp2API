using System.Collections.Generic;
using UnityEngine;

namespace Lerp2API.Game
{
    public class GameConsole : MonoBehaviour
    {
        public static GameConsole me;

        //Monobehaviour props
        public Rect m_consolePosition = new Rect(5, Screen.height - 205, 300, 200);

        //API Props
        private static Dictionary<int, Message> log = new Dictionary<int, Message>();

        private static Vector2 consolePos = Vector2.zero;
        private static string chatContent = "";
        private static new bool enabled;

        //Monobehaviour calls
        private void Awake()
        { //There, we have to load the commands from JSON file
            me = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                enabled = !enabled;
            if (enabled && Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }

        private void OnGUI()
        {
            if (enabled)
                Draw();
        }

        public static void SendMessage(string input = "", bool resetForce = false)
        {   //Check, if input is null, if yes, use input, if not, use chatcontent
            string str = string.IsNullOrEmpty(input) ? chatContent : input;
            //Start doing this...
            //First, we have to check if the str is a command

            if (string.IsNullOrEmpty(input) || resetForce) //If we use chatContent or we want to force its reset, do it
                chatContent = ""; //To finalize, we reset chatcontent
        }

        //API
        internal static void AddMessage(string message, Color color)
        {
            Message.Add(message, color);
        }

        internal static void AddFormattedMessage(string message, Color color)
        {
            //WIP
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

        public static void Draw(long maxQueries = long.MaxValue)
        {
            Rect r = me.m_consolePosition;
            GUILayout.BeginArea(r); //if DebugManager.active avisar
            GUILayout.Box("", GUILayout.Width(r.width), GUILayout.Height(r.height));
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(r.xMin + 5, r.yMin + 5, r.width - 10, r.height - 10));
            GUILayout.BeginVertical();
            consolePos = GUILayout.BeginScrollView(consolePos, GUILayout.Width(r.width - 5), GUILayout.Height(r.height - 35));
            for (int i = log.Count - 1; i >= 0; --i)
            {
                Message m = log[i];
                GUIStyle s = new GUIStyle("label") { fontStyle = GetFontStyle(m.bold, m.italics), wordWrap = true, normal = new GUIStyleState() { textColor = m.color } };
                Vector2 size = s.CalcSize(new GUIContent(m.message));
                GUILayout.Label(m.message, GUILayout.Width(size.x), GUILayout.Height(size.y));
                if (i >= maxQueries)
                    break;
            }
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            //GUILayout.BeginHorizontal();
            chatContent = GUILayout.TextField(chatContent);
            //GUILayout.FlexibleSpace();
            //GUILayout.Button("Send!", GUILayout.wi);
            //GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        public static void Clear()
        {
            log = new Dictionary<int, Message>();
        }

        protected class Message : GameConsole
        {
            public Message(string m, Color c)
                : this(m, c, false, false)
            { }

            public Message(string m, Color c, bool b)
                : this(m, c, b, false)
            { }

            public Message(string m, Color c, bool b, bool i)
            {
                message = m;
                color = c;
                bold = b;
                italics = i;
            }

            public string message = "";
            public Color color = DebugColor.normal;

            public bool bold = false,
                        italics = false;

            public static void Add(string m, Color c)
            {
                log.Add(log.Count, new Message(m, c));
            }
        }
    }
}