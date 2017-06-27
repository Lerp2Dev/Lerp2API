using System.Collections;
using UnityEngine;

namespace Lerp2API.Effects._Text
{
    /// <summary>
    /// Class TextSize.
    /// </summary>
    public class TextSize
    {
        #region fields

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public float width { get { return GetTextWidth(textMesh.text); } }

        private Hashtable dict; //map character -> width

        private TextMesh textMesh;
        private Renderer renderer;

        #endregion fields

        /// <summary>
        /// Initializes a new instance of the <see cref="TextSize"/> class.
        /// </summary>
        /// <param name="tm">The tm.</param>
        public TextSize(TextMesh tm)
        {
            textMesh = tm;
            renderer = tm.GetComponent<Renderer>();
            dict = new Hashtable();
            getSpace();
        }

        private void getSpace()
        { //the space can not be got alone
            string oldText = textMesh.text;

            Quaternion oldRotation = renderer.transform.rotation;
            renderer.transform.rotation = Quaternion.identity;

            textMesh.text = "a";
            float aw = renderer.bounds.size.x;
            textMesh.text = "a a";
            float cw = renderer.bounds.size.x - 2 * aw;

            renderer.transform.rotation = oldRotation;

            dict.Add(' ', cw);
            dict.Add('a', aw);

            textMesh.text = oldText;
        }

        private float GetTextWidth(string s)
        {
            char[] charList = s.ToCharArray();
            float w = 0;
            char c;
            string oldText = textMesh.text;

            for (int i = 0; i < charList.Length; i++)
            {
                c = charList[i];

                if (dict.ContainsKey(c))
                {
                    w += (float)dict[c];
                }
                else
                {
                    Quaternion oldRotation = renderer.transform.rotation;
                    renderer.transform.rotation = Quaternion.identity;
                    textMesh.text = "" + c;
                    float cw = renderer.bounds.size.x;
                    dict.Add(c, cw);
                    w += cw;
                    renderer.transform.rotation = oldRotation;
                    //MonoBehaviour.print("char<" + c +"> " + cw);
                }
            }

            textMesh.text = oldText;
            return w;
        }

        /// <summary>
        /// Fits to width.
        /// </summary>
        /// <param name="wantedWidth">Width of the wanted.</param>
        /// <param name="maxLines">The maximum lines.</param>
        public void FitToWidth(float wantedWidth, int maxLines = -1)
        {
            //if(width <= wantedWidth) return;

            string oldText = textMesh.text;
            textMesh.text = "";

            string[] lines = oldText.Split('\n');

            int numLines = 0;
            foreach (string line in lines)
            {
                textMesh.text += wrapLine(line, wantedWidth, maxLines - numLines);
                numLines++;
                if (maxLines != -1 && numLines >= maxLines)
                    return;
                textMesh.text += "\n";
            }
        }

        private string wrapLine(string s, float w, int maxLines = -1)
        {
            // need to check if smaller than maximum character length, really...
            if (w == 0 || s.Length <= 0) return s;

            char c;
            char[] charList = s.ToCharArray();

            float wordWidth = 0;
            float currentWidth = 0;

            string word = "";
            string newText = "";
            string oldText = textMesh.text;

            int numLines = 0;

            for (int i = 0; i < charList.Length; i++)
            {
                c = charList[i];

                float charWidth = 0;
                if (dict.ContainsKey(c))
                {
                    charWidth = (float)dict[c];
                }
                else
                {
                    textMesh.text = "" + c;
                    Quaternion oldRotation = renderer.transform.rotation;
                    renderer.transform.rotation = Quaternion.identity;
                    charWidth = renderer.bounds.size.x;
                    renderer.transform.rotation = oldRotation;
                    dict.Add(c, charWidth);
                    //here check if max char length
                }

                if (c == ' ' || i == charList.Length - 1)
                {
                    if (c != ' ')
                    {
                        word += c.ToString();
                        wordWidth += charWidth;
                    }

                    if (currentWidth + wordWidth < w)
                    {
                        currentWidth += wordWidth;
                        newText += word;
                    }
                    else
                    {
                        if (maxLines != -1 && numLines >= maxLines)
                            break;
                        currentWidth = wordWidth;
                        newText += word.Replace(" ", "\n");
                        numLines++;
                    }

                    word = "";
                    wordWidth = 0;
                }

                word += c.ToString();
                wordWidth += charWidth;
            }

            textMesh.text = oldText;
            return newText;
        }
    }

    /// <summary>
    /// Class TextSizeUtils.
    /// </summary>
    public static class TextSizeUtils
    {
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 GetSize(this TextMesh mesh)
        {
            float width = 0;
            foreach (char symbol in mesh.text)
            {
                CharacterInfo info;
                if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
                    width += info.advance;
            }
            int lines = mesh.text.Split('\n').Length;
            List<int> heighs = new List<int>();
            foreach (char symbol in mesh.text)
            {
                CharacterInfo info;
                if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
                    heighs.Add(info.glyphWidth);
            }
            return new Vector2(width * mesh.characterSize * 0.1f + 30, heighs.Max() * lines * mesh.characterSize * 0.1f);
        }
    }
}