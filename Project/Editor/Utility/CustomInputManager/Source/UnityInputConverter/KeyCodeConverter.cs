using UnityEngine;
using System.Collections.Generic;

namespace UnityInputConverter
{
	internal static class KeyCodeConverter
	{
		public static Dictionary<string, KeyCode> Map = new Dictionary<string, KeyCode>()
		{
			{ "a", KeyCode.A },
			{ "b", KeyCode.B },
			{ "c", KeyCode.C },
			{ "d", KeyCode.D },
			{ "e", KeyCode.E },
			{ "f", KeyCode.F },
			{ "g", KeyCode.G },
			{ "h", KeyCode.H },
			{ "i", KeyCode.I },
			{ "j", KeyCode.J },
			{ "k", KeyCode.K },
			{ "l", KeyCode.L },
			{ "m", KeyCode.M },
			{ "n", KeyCode.N },
			{ "o", KeyCode.O },
			{ "p", KeyCode.P },
			{ "q", KeyCode.Q },
			{ "r", KeyCode.R },
			{ "s", KeyCode.S },
			{ "t", KeyCode.T },
			{ "u", KeyCode.U },
			{ "v", KeyCode.V },
			{ "w", KeyCode.W },
			{ "x", KeyCode.X },
			{ "y", KeyCode.Y },
			{ "z", KeyCode.Z },
			{ "0", KeyCode.Alpha0 },
			{ "1", KeyCode.Alpha1 },
			{ "2", KeyCode.Alpha2 },
			{ "3", KeyCode.Alpha3 },
			{ "4", KeyCode.Alpha4 },
			{ "5", KeyCode.Alpha5 },
			{ "6", KeyCode.Alpha6 },
			{ "7", KeyCode.Alpha7 },
			{ "8", KeyCode.Alpha8 },
			{ "9", KeyCode.Alpha9 },
			{ "[0]", KeyCode.Keypad0 },
			{ "[1]", KeyCode.Keypad1 },
			{ "[2]", KeyCode.Keypad2 },
			{ "[3]", KeyCode.Keypad3 },
			{ "[4]", KeyCode.Keypad4 },
			{ "[5]", KeyCode.Keypad5 },
			{ "[6]", KeyCode.Keypad6 },
			{ "[7]", KeyCode.Keypad7 },
			{ "[8]", KeyCode.Keypad8 },
			{ "[9]", KeyCode.Keypad9 },
			{ "mouse 0", KeyCode.Mouse0 },
			{ "mouse 1", KeyCode.Mouse1 },
			{ "mouse 2", KeyCode.Mouse2 },
			{ "mouse 3", KeyCode.Mouse3 },
			{ "mouse 4", KeyCode.Mouse4 },
			{ "mouse 5", KeyCode.Mouse5 },
			{ "mouse 6", KeyCode.Mouse6 },
			{ "f1", KeyCode.F1 },
			{ "f2", KeyCode.F2 },
			{ "f3", KeyCode.F3 },
			{ "f4", KeyCode.F4 },
			{ "f5", KeyCode.F5 },
			{ "f6", KeyCode.F6 },
			{ "f7", KeyCode.F7 },
			{ "f8", KeyCode.F8 },
			{ "f9", KeyCode.F9 },
			{ "f10", KeyCode.F10 },
			{ "f11", KeyCode.F11 },
			{ "f12", KeyCode.F12 },
			{ "f13", KeyCode.F13 },
			{ "f14", KeyCode.F14 },
			{ "f15", KeyCode.F15 },
			{ "joystick button 0", KeyCode.JoystickButton0 },
			{ "joystick button 1", KeyCode.JoystickButton1 },
			{ "joystick button 2", KeyCode.JoystickButton2 },
			{ "joystick button 3", KeyCode.JoystickButton3 },
			{ "joystick button 4", KeyCode.JoystickButton4 },
			{ "joystick button 5", KeyCode.JoystickButton5 },
			{ "joystick button 6", KeyCode.JoystickButton6 },
			{ "joystick button 7", KeyCode.JoystickButton7 },
			{ "joystick button 8", KeyCode.JoystickButton8 },
			{ "joystick button 9", KeyCode.JoystickButton9 },
			{ "joystick button 10", KeyCode.JoystickButton10 },
			{ "joystick button 11", KeyCode.JoystickButton11 },
			{ "joystick button 12", KeyCode.JoystickButton12 },
			{ "joystick button 13", KeyCode.JoystickButton13 },
			{ "joystick button 14", KeyCode.JoystickButton14 },
			{ "joystick button 15", KeyCode.JoystickButton15 },
			{ "joystick button 16", KeyCode.JoystickButton16 },
			{ "joystick button 17", KeyCode.JoystickButton17 },
			{ "joystick button 18", KeyCode.JoystickButton18 },
			{ "joystick button 19", KeyCode.JoystickButton19 },
			{ "joystick 1 button 0", KeyCode.Joystick1Button0 },
			{ "joystick 1 button 1", KeyCode.Joystick1Button1 },
			{ "joystick 1 button 2", KeyCode.Joystick1Button2 },
			{ "joystick 1 button 3", KeyCode.Joystick1Button3 },
			{ "joystick 1 button 4", KeyCode.Joystick1Button4 },
			{ "joystick 1 button 5", KeyCode.Joystick1Button5 },
			{ "joystick 1 button 6", KeyCode.Joystick1Button6 },
			{ "joystick 1 button 7", KeyCode.Joystick1Button7 },
			{ "joystick 1 button 8", KeyCode.Joystick1Button8 },
			{ "joystick 1 button 9", KeyCode.Joystick1Button9 },
			{ "joystick 1 button 10", KeyCode.Joystick1Button10 },
			{ "joystick 1 button 11", KeyCode.Joystick1Button11 },
			{ "joystick 1 button 12", KeyCode.Joystick1Button12 },
			{ "joystick 1 button 13", KeyCode.Joystick1Button13 },
			{ "joystick 1 button 14", KeyCode.Joystick1Button14 },
			{ "joystick 1 button 15", KeyCode.Joystick1Button15 },
			{ "joystick 1 button 16", KeyCode.Joystick1Button16 },
			{ "joystick 1 button 17", KeyCode.Joystick1Button17 },
			{ "joystick 1 button 18", KeyCode.Joystick1Button18 },
			{ "joystick 1 button 19", KeyCode.Joystick1Button19 },
			{ "joystick 2 button 0", KeyCode.Joystick2Button0 },
			{ "joystick 2 button 1", KeyCode.Joystick2Button1 },
			{ "joystick 2 button 2", KeyCode.Joystick2Button2 },
			{ "joystick 2 button 3", KeyCode.Joystick2Button3 },
			{ "joystick 2 button 4", KeyCode.Joystick2Button4 },
			{ "joystick 2 button 5", KeyCode.Joystick2Button5 },
			{ "joystick 2 button 6", KeyCode.Joystick2Button6 },
			{ "joystick 2 button 7", KeyCode.Joystick2Button7 },
			{ "joystick 2 button 8", KeyCode.Joystick2Button8 },
			{ "joystick 2 button 9", KeyCode.Joystick2Button9 },
			{ "joystick 2 button 10", KeyCode.Joystick2Button10 },
			{ "joystick 2 button 11", KeyCode.Joystick2Button11 },
			{ "joystick 2 button 12", KeyCode.Joystick2Button12 },
			{ "joystick 2 button 13", KeyCode.Joystick2Button13 },
			{ "joystick 2 button 14", KeyCode.Joystick2Button14 },
			{ "joystick 2 button 15", KeyCode.Joystick2Button15 },
			{ "joystick 2 button 16", KeyCode.Joystick2Button16 },
			{ "joystick 2 button 17", KeyCode.Joystick2Button17 },
			{ "joystick 2 button 18", KeyCode.Joystick2Button18 },
			{ "joystick 2 button 19", KeyCode.Joystick2Button19 },
			{ "joystick 3 button 0", KeyCode.Joystick3Button0 },
			{ "joystick 3 button 1", KeyCode.Joystick3Button1 },
			{ "joystick 3 button 2", KeyCode.Joystick3Button2 },
			{ "joystick 3 button 3", KeyCode.Joystick3Button3 },
			{ "joystick 3 button 4", KeyCode.Joystick3Button4 },
			{ "joystick 3 button 5", KeyCode.Joystick3Button5 },
			{ "joystick 3 button 6", KeyCode.Joystick3Button6 },
			{ "joystick 3 button 7", KeyCode.Joystick3Button7 },
			{ "joystick 3 button 8", KeyCode.Joystick3Button8 },
			{ "joystick 3 button 9", KeyCode.Joystick3Button9 },
			{ "joystick 3 button 10", KeyCode.Joystick3Button10 },
			{ "joystick 3 button 11", KeyCode.Joystick3Button11 },
			{ "joystick 3 button 12", KeyCode.Joystick3Button12 },
			{ "joystick 3 button 13", KeyCode.Joystick3Button13 },
			{ "joystick 3 button 14", KeyCode.Joystick3Button14 },
			{ "joystick 3 button 15", KeyCode.Joystick3Button15 },
			{ "joystick 3 button 16", KeyCode.Joystick3Button16 },
			{ "joystick 3 button 17", KeyCode.Joystick3Button17 },
			{ "joystick 3 button 18", KeyCode.Joystick3Button18 },
			{ "joystick 3 button 19", KeyCode.Joystick3Button19 },
			{ "joystick 4 button 0", KeyCode.Joystick4Button0 },
			{ "joystick 4 button 1", KeyCode.Joystick4Button1 },
			{ "joystick 4 button 2", KeyCode.Joystick4Button2 },
			{ "joystick 4 button 3", KeyCode.Joystick4Button3 },
			{ "joystick 4 button 4", KeyCode.Joystick4Button4 },
			{ "joystick 4 button 5", KeyCode.Joystick4Button5 },
			{ "joystick 4 button 6", KeyCode.Joystick4Button6 },
			{ "joystick 4 button 7", KeyCode.Joystick4Button7 },
			{ "joystick 4 button 8", KeyCode.Joystick4Button8 },
			{ "joystick 4 button 9", KeyCode.Joystick4Button9 },
			{ "joystick 4 button 10", KeyCode.Joystick4Button10 },
			{ "joystick 4 button 11", KeyCode.Joystick4Button11 },
			{ "joystick 4 button 12", KeyCode.Joystick4Button12 },
			{ "joystick 4 button 13", KeyCode.Joystick4Button13 },
			{ "joystick 4 button 14", KeyCode.Joystick4Button14 },
			{ "joystick 4 button 15", KeyCode.Joystick4Button15 },
			{ "joystick 4 button 16", KeyCode.Joystick4Button16 },
			{ "joystick 4 button 17", KeyCode.Joystick4Button17 },
			{ "joystick 4 button 18", KeyCode.Joystick4Button18 },
			{ "joystick 4 button 19", KeyCode.Joystick4Button19 },
			{ "joystick 5 button 0", KeyCode.Joystick5Button0 },
			{ "joystick 5 button 1", KeyCode.Joystick5Button1 },
			{ "joystick 5 button 2", KeyCode.Joystick5Button2 },
			{ "joystick 5 button 3", KeyCode.Joystick5Button3 },
			{ "joystick 5 button 4", KeyCode.Joystick5Button4 },
			{ "joystick 5 button 5", KeyCode.Joystick5Button5 },
			{ "joystick 5 button 6", KeyCode.Joystick5Button6 },
			{ "joystick 5 button 7", KeyCode.Joystick5Button7 },
			{ "joystick 5 button 8", KeyCode.Joystick5Button8 },
			{ "joystick 5 button 9", KeyCode.Joystick5Button9 },
			{ "joystick 5 button 10", KeyCode.Joystick5Button10 },
			{ "joystick 5 button 11", KeyCode.Joystick5Button11 },
			{ "joystick 5 button 12", KeyCode.Joystick5Button12 },
			{ "joystick 5 button 13", KeyCode.Joystick5Button13 },
			{ "joystick 5 button 14", KeyCode.Joystick5Button14 },
			{ "joystick 5 button 15", KeyCode.Joystick5Button15 },
			{ "joystick 5 button 16", KeyCode.Joystick5Button16 },
			{ "joystick 5 button 17", KeyCode.Joystick5Button17 },
			{ "joystick 5 button 18", KeyCode.Joystick5Button18 },
			{ "joystick 5 button 19", KeyCode.Joystick5Button19 },
			{ "joystick 6 button 0", KeyCode.Joystick6Button0 },
			{ "joystick 6 button 1", KeyCode.Joystick6Button1 },
			{ "joystick 6 button 2", KeyCode.Joystick6Button2 },
			{ "joystick 6 button 3", KeyCode.Joystick6Button3 },
			{ "joystick 6 button 4", KeyCode.Joystick6Button4 },
			{ "joystick 6 button 5", KeyCode.Joystick6Button5 },
			{ "joystick 6 button 6", KeyCode.Joystick6Button6 },
			{ "joystick 6 button 7", KeyCode.Joystick6Button7 },
			{ "joystick 6 button 8", KeyCode.Joystick6Button8 },
			{ "joystick 6 button 9", KeyCode.Joystick6Button9 },
			{ "joystick 6 button 10", KeyCode.Joystick6Button10 },
			{ "joystick 6 button 11", KeyCode.Joystick6Button11 },
			{ "joystick 6 button 12", KeyCode.Joystick6Button12 },
			{ "joystick 6 button 13", KeyCode.Joystick6Button13 },
			{ "joystick 6 button 14", KeyCode.Joystick6Button14 },
			{ "joystick 6 button 15", KeyCode.Joystick6Button15 },
			{ "joystick 6 button 16", KeyCode.Joystick6Button16 },
			{ "joystick 6 button 17", KeyCode.Joystick6Button17 },
			{ "joystick 6 button 18", KeyCode.Joystick6Button18 },
			{ "joystick 6 button 19", KeyCode.Joystick6Button19 },
			{ "joystick 7 button 0", KeyCode.Joystick7Button0 },
			{ "joystick 7 button 1", KeyCode.Joystick7Button1 },
			{ "joystick 7 button 2", KeyCode.Joystick7Button2 },
			{ "joystick 7 button 3", KeyCode.Joystick7Button3 },
			{ "joystick 7 button 4", KeyCode.Joystick7Button4 },
			{ "joystick 7 button 5", KeyCode.Joystick7Button5 },
			{ "joystick 7 button 6", KeyCode.Joystick7Button6 },
			{ "joystick 7 button 7", KeyCode.Joystick7Button7 },
			{ "joystick 7 button 8", KeyCode.Joystick7Button8 },
			{ "joystick 7 button 9", KeyCode.Joystick7Button9 },
			{ "joystick 7 button 10", KeyCode.Joystick7Button10 },
			{ "joystick 7 button 11", KeyCode.Joystick7Button11 },
			{ "joystick 7 button 12", KeyCode.Joystick7Button12 },
			{ "joystick 7 button 13", KeyCode.Joystick7Button13 },
			{ "joystick 7 button 14", KeyCode.Joystick7Button14 },
			{ "joystick 7 button 15", KeyCode.Joystick7Button15 },
			{ "joystick 7 button 16", KeyCode.Joystick7Button16 },
			{ "joystick 7 button 17", KeyCode.Joystick7Button17 },
			{ "joystick 7 button 18", KeyCode.Joystick7Button18 },
			{ "joystick 7 button 19", KeyCode.Joystick7Button19 },
			{ "joystick 8 button 0", KeyCode.Joystick8Button0 },
			{ "joystick 8 button 1", KeyCode.Joystick8Button1 },
			{ "joystick 8 button 2", KeyCode.Joystick8Button2 },
			{ "joystick 8 button 3", KeyCode.Joystick8Button3 },
			{ "joystick 8 button 4", KeyCode.Joystick8Button4 },
			{ "joystick 8 button 5", KeyCode.Joystick8Button5 },
			{ "joystick 8 button 6", KeyCode.Joystick8Button6 },
			{ "joystick 8 button 7", KeyCode.Joystick8Button7 },
			{ "joystick 8 button 8", KeyCode.Joystick8Button8 },
			{ "joystick 8 button 9", KeyCode.Joystick8Button9 },
			{ "joystick 8 button 10", KeyCode.Joystick8Button10 },
			{ "joystick 8 button 11", KeyCode.Joystick8Button11 },
			{ "joystick 8 button 12", KeyCode.Joystick8Button12 },
			{ "joystick 8 button 13", KeyCode.Joystick8Button13 },
			{ "joystick 8 button 14", KeyCode.Joystick8Button14 },
			{ "joystick 8 button 15", KeyCode.Joystick8Button15 },
			{ "joystick 8 button 16", KeyCode.Joystick8Button16 },
			{ "joystick 8 button 17", KeyCode.Joystick8Button17 },
			{ "joystick 8 button 18", KeyCode.Joystick8Button18 },
			{ "joystick 8 button 19", KeyCode.Joystick8Button19 },
			{ "up", KeyCode.UpArrow },
			{ "down", KeyCode.DownArrow },
			{ "left", KeyCode.LeftArrow },
			{ "right", KeyCode.RightArrow },
			{ "left shift", KeyCode.LeftShift },
			{ "right shift", KeyCode.RightShift },
			{ "left ctrl", KeyCode.LeftControl },
			{ "right ctrl", KeyCode.RightControl },
			{ "left alt", KeyCode.LeftAlt },
			{ "right alt", KeyCode.RightAlt },
			{ "right cmd", KeyCode.LeftCommand },
			{ "left cmd", KeyCode.RightCommand },
			{ "backspace", KeyCode.Backspace },
			{ "tab", KeyCode.Tab },
			{ "return", KeyCode.Return },
			{ "escape", KeyCode.Escape },
			{ "space", KeyCode.Space },
			{ "delete", KeyCode.Delete },
			{ "enter", KeyCode.Return },
			{ "insert", KeyCode.Insert },
			{ "home", KeyCode.Home },
			{ "end", KeyCode.End },
			{ "page up", KeyCode.PageUp },
			{ "page down", KeyCode.PageDown },
			{ "pause", KeyCode.Pause },
			{ ".", KeyCode.Period },
			{ "/", KeyCode.Slash },
			{ "=", KeyCode.Equals },
			{ "[", KeyCode.LeftBracket },
			{ "]", KeyCode.RightBracket },
			{ "`", KeyCode.BackQuote },
			{ ";", KeyCode.Semicolon },
			{ "'", KeyCode.Quote },
			{ ",", KeyCode.Comma },
			{ "-", KeyCode.Minus },
		};
	}
}