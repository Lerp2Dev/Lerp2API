using System.Collections;
using System.Text;
using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;

public class ResultLogger : Object
{
    // helper to log Arraylists and Hashtables
    public static void logObject(object result)
    {
        if (result.GetType() == typeof(ArrayList))
            logArraylist((ArrayList)result);
        else if (result.GetType() == typeof(Hashtable))
            logHashtable((Hashtable)result);
        else
            Debug.Log("result is not a hashtable or arraylist");
    }

    public static void logArraylist(ArrayList result)
    {
        StringBuilder builder = new StringBuilder();

        // we start off with an ArrayList of Hashtables
        foreach (Hashtable item in result)
        {
            addHashtableToString(builder, item);
            builder.Append("\n--------------------\n");
        }
        Debug.Log(builder.ToString());
    }

    public static void logHashtable(Hashtable result)
    {
        StringBuilder builder = new StringBuilder();
        addHashtableToString(builder, result);

        Debug.Log(builder.ToString());
    }

    // simple helper to add a hashtable to a StringBuilder to make reading the output easier
    public static void addHashtableToString(StringBuilder builder, Hashtable item)
    {
        foreach (DictionaryEntry entry in item)
        {
            if (entry.Value is Hashtable)
            {
                builder.AppendFormat("{0}: ", entry.Key);
                addHashtableToString(builder, (Hashtable)entry.Value);
            }
            else if (entry.Value is ArrayList)
            {
                builder.AppendFormat("{0}: ", entry.Key);
                addArraylistToString(builder, (ArrayList)entry.Value);
            }
            else
            {
                builder.AppendFormat("{0}: {1}\n", entry.Key, entry.Value);
            }
        }
    }

    public static void addArraylistToString(StringBuilder builder, ArrayList result)
    {
        // we start off with an ArrayList of Hashtables
        foreach (object item in result)
        {
            if (item is Hashtable)
                addHashtableToString(builder, (Hashtable)item);
            else if (item is ArrayList)
                addArraylistToString(builder, (ArrayList)item);
            builder.Append("\n--------------------\n");
        }
        Debug.Log(builder.ToString());
    }
}