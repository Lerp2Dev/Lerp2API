namespace HtmlAgilityPack
{
    using System;

    internal class HtmlCmdLine
    {
        internal static bool Help = false;

        static HtmlCmdLine()
        {
            ParseArgs();
        }

        private static void GetBoolArg(string Arg, string Name, ref bool ArgValue)
        {
            if (((Arg.Length >= (Name.Length + 1)) && (('/' == Arg[0]) || ('-' == Arg[0]))) && (Arg.Substring(1, Name.Length).ToLower() == Name.ToLower()))
            {
                ArgValue = true;
            }
        }

        private static void GetIntArg(string Arg, string Name, ref int ArgValue)
        {
            if (((Arg.Length >= (Name.Length + 3)) && (('/' == Arg[0]) || ('-' == Arg[0]))) && (Arg.Substring(1, Name.Length).ToLower() == Name.ToLower()))
            {
                try
                {
                    ArgValue = Convert.ToInt32(Arg.Substring(Name.Length + 2, (Arg.Length - Name.Length) - 2));
                }
                catch
                {
                }
            }
        }

        internal static string GetOption(int index, string def)
        {
            string argValue = def;
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            int num = 0;
            for (int i = 1; i < commandLineArgs.Length; i++)
            {
                if (GetStringArg(commandLineArgs[i], ref argValue))
                {
                    if (index == num)
                    {
                        return argValue;
                    }
                    argValue = def;
                    num++;
                }
            }
            return argValue;
        }

        internal static bool GetOption(string name, bool def)
        {
            bool argValue = def;
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            for (int i = 1; i < commandLineArgs.Length; i++)
            {
                GetBoolArg(commandLineArgs[i], name, ref argValue);
            }
            return argValue;
        }

        internal static int GetOption(string name, int def)
        {
            int argValue = def;
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            for (int i = 1; i < commandLineArgs.Length; i++)
            {
                GetIntArg(commandLineArgs[i], name, ref argValue);
            }
            return argValue;
        }

        internal static string GetOption(string name, string def)
        {
            string argValue = def;
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            for (int i = 1; i < commandLineArgs.Length; i++)
            {
                GetStringArg(commandLineArgs[i], name, ref argValue);
            }
            return argValue;
        }

        private static bool GetStringArg(string Arg, ref string ArgValue)
        {
            if (('/' == Arg[0]) || ('-' == Arg[0]))
            {
                return false;
            }
            ArgValue = Arg;
            return true;
        }

        private static void GetStringArg(string Arg, string Name, ref string ArgValue)
        {
            if (((Arg.Length >= (Name.Length + 3)) && (('/' == Arg[0]) || ('-' == Arg[0]))) && (Arg.Substring(1, Name.Length).ToLower() == Name.ToLower()))
            {
                ArgValue = Arg.Substring(Name.Length + 2, (Arg.Length - Name.Length) - 2);
            }
        }

        private static void ParseArgs()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            for (int i = 1; i < commandLineArgs.Length; i++)
            {
                GetBoolArg(commandLineArgs[i], "?", ref Help);
                GetBoolArg(commandLineArgs[i], "h", ref Help);
                GetBoolArg(commandLineArgs[i], "help", ref Help);
            }
        }
    }
}