using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace Lerp2API.Utility
{
    //This needs the API
    /*public class AnimUtil
    {

        public AnimUtil(string url, bool outputDebugLog = false, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            int loop;
            gifTexList = UniGif.GetTextureList(Resources.Load<TextAsset>(url).bytes, out loop, out _w, out _h, filterMode, wrapMode, outputDebugLog);
        }

        public AnimUtil(byte[] data)
        {

        }

        public AnimUtil(Texture2D spriteSheet)
        {

        }

        private int _ind, _w, _h;
        private float _time;
        private List<UniGif.GifTexture> gifTexList = new List<UniGif.GifTexture>();

        /*public IEnumerator LoadGifListCoroutine(MonoBehaviour m, string url, bool outputDebugLog = false, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("URL is nothing.");
                yield break;
            }

            string path;
            if (url.StartsWith("http"))
            {
                // from WEB
                path = url;
            }
            else
            {
                // from StreamingAssets
                if (!url.Contains(":"))
                    path = "file:///" + Application.dataPath + "/" + url;
                else
                    path = "file:///" + url;
            }

            // Load file
            using (WWW www = new WWW(path))
            {
                yield return www;

                if (!string.IsNullOrEmpty(www.error))
                    Debug.LogError("File load error.\n" + www.error);
                else
                {
                    gifTexList.Clear();
                    yield return m.StartCoroutine(UniGif.GetTextureListCoroutine(m, www.bytes, (gtList, loop, w, h) => {
                        gifTexList = gtList;
                        _w = w;
                        _h = h;
                    }, filterMode, wrapMode, outputDebugLog));
                }
            }
        } //*\

        public void Draw(float xMin, float yMin)
        {
            try
            {
                if (gifTexList != null && gifTexList.Count > 0)
                {
                    if (_ind == gifTexList.Count)
                        _ind = 0;

                    GUI.DrawTexture(new Rect(xMin, yMin, _w, _h), gifTexList[_ind].texture2d);

                    if (Time.time - _time > gifTexList[_ind].delaySec)
                    {
                        _ind++;
                        _time = Time.time;
                    }
                }
                else
                    Debug.LogError("Gif List is null...");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

    }*/

    /// <summary>
    /// Class TextureUtils.
    /// </summary>
    public class TextureUtils
    {
        /// <summary>
        /// Fills the specified c.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        /// <returns>Texture2D.</returns>
        public static Texture2D Fill(Color c, int w, int h)
        {
            Texture2D t = new Texture2D(w, h);
            for (int i = 0; i < w; ++i)
                for (int j = 0; j < h; ++j)
                    t.SetPixel(i, j, c);
            t.Apply();
            return t;
        }
        /// <summary>
        /// Fills the color.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        /// <returns>Color[].</returns>
        public static Color[,] FillColor(Color c, int w, int h)
        {
            Color[,] cc = new Color[w, h];
            for (int i = 0; i < w; ++i)
                for (int j = 0; j < h; ++j)
                    cc[i, j] = c;
            return cc;
        }
    }

    /// <summary>
    /// Class TextureCrop.
    /// </summary>
    public class TextureCrop
    {

        private Texture2D processedTexture;
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCrop"/> class.
        /// </summary>
        /// <param name="initialTexture">The initial texture.</param>
        /// <param name="rect">The rect.</param>
        public TextureCrop(Texture2D initialTexture, Rect rect)
        {
            if (initialTexture == null)
            {
                Debug.LogError("Texture passed is null");
                return;
            }
            if (rect.xMin > initialTexture.width || rect.yMin > initialTexture.height || rect.xMax > initialTexture.width || rect.yMax > initialTexture.height)
            {
                int reqWidth = (int)rect.xMax, reqHeight = (int)rect.yMax, newWidth = 0, newHeight = 0;
                if (reqWidth > reqHeight)
                {
                    newWidth = reqWidth;
                    newHeight = initialTexture.height * newWidth / initialTexture.width;
                }
                else
                {
                    newHeight = reqHeight;
                    newWidth = initialTexture.width * newHeight / initialTexture.height;
                }
                TextureScale.Point(initialTexture, newWidth, newHeight);
            }
            processedTexture = new Texture2D((int)rect.width, (int)rect.height);
            var p = initialTexture.GetPixels((int)rect.xMin, (int)rect.yMin, (int)rect.width, (int)rect.height);
            processedTexture.SetPixels(p);
            processedTexture.Apply();
        }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <returns>Texture2D.</returns>
        public Texture2D GetTexture()
        {
            return processedTexture;
        }

    }

    /// <summary>
    /// Class TextureAutocrop.
    /// </summary>
    public class TextureAutocrop
    {
        private Texture2D processedTexture;
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureAutocrop"/> class.
        /// </summary>
        /// <param name="initialTexture">The initial texture.</param>
        public TextureAutocrop(Texture2D initialTexture)
        {
            if (initialTexture == null)
            {
                Debug.LogError("Texture passed is null");
                return;
            }

            int xMinTrans = 0, yMinTrans = 0, xMaxTrans = 0, yMaxTrans = 0;

            for (int x = 0; x < initialTexture.width; ++x)
                for (int y = 0; y < initialTexture.height; ++y)
                {
                    Color c = initialTexture.GetPixel(x, y);

                    if (c.a > 0)
                    {
                        xMaxTrans = x;
                        break;
                    }
                }

            for (int x = initialTexture.width; x >= 0; --x)
                for (int y = 0; y < initialTexture.height; ++y)
                {
                    Color c = initialTexture.GetPixel(x, y);

                    if (c.a > 0)
                    {
                        xMinTrans = x;
                        break;
                    }
                }

            for (int y = 0; y < initialTexture.height; ++y)
                for (int x = 0; x < initialTexture.width; ++x)
                {
                    Color c = initialTexture.GetPixel(x, y);

                    if (c.a > 0)
                    {
                        yMaxTrans = y;
                        break;
                    }
                }

            for (int y = initialTexture.height; y >= 0; --y)
                for (int x = 0; x < initialTexture.width; ++x)
                {
                    Color c = initialTexture.GetPixel(x, y);

                    if (c.a > 0)
                    {
                        yMinTrans = y;
                        break;
                    }
                }

            int finalWidth = xMaxTrans - xMinTrans;
            int finalHeight = yMaxTrans - yMinTrans;

            processedTexture = new TextureCrop(processedTexture, new Rect(xMinTrans, yMinTrans, finalWidth, finalHeight)).GetTexture();

        }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <returns>Texture2D.</returns>
        public Texture2D GetTexture()
        {
            return processedTexture;
        }
    }

    /// <summary>
    /// Class TextureRotate.
    /// </summary>
    public class TextureRotate
    {

        private Texture2D processedTexture;
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureRotate"/> class.
        /// </summary>
        /// <param name="initialTexture">The initial texture.</param>
        /// <param name="degrees">The degrees.</param>
        public TextureRotate(Texture2D initialTexture, float degrees)
        {
            if (initialTexture == null)
            {
                Debug.LogError("Texture passed is null");
                return;
            }
            Vector2 centerPoint = new Vector2(initialTexture.width / 2, initialTexture.height / 2);
            int xMin = 0, yMin = 0, xMax = 0, yMax = 0;
            for (int x = 0; x < initialTexture.width; ++x)
                for (int y = 0; y < initialTexture.height; ++y)
                {
                    Vector2 p = RotatePoint(new Vector2(x, y), centerPoint, degrees);

                    int pointX = (int)p.x;
                    int pointY = (int)p.y;

                    if (pointX < xMin)
                        xMin = pointX;

                    if (pointX > xMax)
                        xMax = pointX;

                    if (pointY < yMin)
                        yMin = pointY;

                    if (pointY > yMax)
                        yMax = pointY;
                }
            int width = xMax - xMin;//initialTexture.width + Mathf.Abs(xMin) + (xMax - initialTexture.width);
            int height = yMax - yMin;//initialTexture.height + Mathf.Abs(yMin) + (yMax - initialTexture.height);
            Vector2 centerPoint1 = new Vector2(width / 2, height / 2);
            Texture2D preTex = new Texture2D(width, height);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    preTex.SetPixel(x, y, new Color(0, 0, 0, 0));

            for (int x = 0; x < initialTexture.width; ++x)
                for (int y = 0; y < initialTexture.height; ++y)
                {
                    Vector2 p = RotatePoint(new Vector2(x + Mathf.Abs(xMin), y + Mathf.Abs(yMin)), centerPoint1, degrees);
                    preTex.SetPixel((int)p.x, (int)p.y, initialTexture.GetPixel(x, y));
                }

            preTex.Apply();

            processedTexture = new TextureAutocrop(processedTexture).GetTexture();

        }

        private Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, float angleInDegrees)
        {
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
            float cosTheta = Mathf.Cos(angleInRadians);
            float sinTheta = Mathf.Sin(angleInRadians);
            return new Vector2(
                    (int)(cosTheta * (pointToRotate.x - centerPoint.x) - sinTheta * (pointToRotate.y - centerPoint.y) + centerPoint.x),
                    (int)(sinTheta * (pointToRotate.x - centerPoint.x) + cosTheta * (pointToRotate.y - centerPoint.y) + centerPoint.y));
        }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <returns>Texture2D.</returns>
        public Texture2D GetTexture()
        {
            return processedTexture;
        }

        /*private Color32[] pix1;
        private Color32[] pix2;
        private Color32[] pix3;

        public TextureRotate(Texture2D initialTexture, float degrees)
        {
             float rads = Mathf.Deg2Rad * degrees;
             pix2 = initialTexture.GetPixels32();
             int W = initialTexture.width;
             int H = initialTexture.height;

             //pix3 = rotateSquare(initialTexture, pix2, rads);
             int pointX;
             int pointY;
             float sn = Mathf.Sin(rads);
             float cs = Mathf.Cos(rads);
             int xc = W / 2;
             int yc = H / 2;
             int xMin = 0;
             int yMin = 0;
             int xMax = 0;
             int yMax = 0;
             pix3 = initialTexture.GetPixels32();

             for (int j = 0; j < H; j++)
             {
                 for (int i = 0; i < W; i++)
                 {
                     pix3[j * W + i] = new Color32(0, 0, 0, 0);

                     pointX = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                     pointY = (int)(-sn * (i - xc) + cs * (j - yc) + yc);

                     if (pointX < xMin)
                         xMin = pointX;

                     if (pointX > xMax)
                         xMax = pointX;

                     if (pointY < yMin)
                         yMin = pointY;

                     if (pointY > yMax)
                         yMax = pointY;

                     if ((pointX > -1) && (pointX < W) && (pointY > -1) && (pointY < H))
                     {
                         pix3[j * W + i] = pix2[pointY * W + pointX];
                     }
                 }
             }

             pix1 = new Color32[pix3.Length];

             int width = xMax - xMin;
             int height = yMax - yMin;

             for (int j = 0; j < height; ++j)
                 for (int i = 0; i < width; ++i)
                     pix1[initialTexture.width/2 - W/2 + i + initialTexture.width*(initialTexture.height/2-H/2+j)] = pix3[i + j*W];

             processedTexture = new Texture2D(initialTexture.width, initialTexture.height);
             processedTexture.SetPixels32(pix1);
             processedTexture.Apply();

        }*/

    }

    /// <summary>
    /// Class TextureBorder.
    /// </summary>
    public class TextureBorder //Its bugged
    { //Next feature: gradients and find real border, detect active border to blend
        /// <summary>
        /// The processed texture
        /// </summary>
        public Texture2D processedTexture, initTex;
        /// <summary>
        /// The general border
        /// </summary>
        public int generalBorder = 1;
        /// <summary>
        /// The general border type
        /// </summary>
        public BorderType generalBorderType;
        /// <summary>
        /// The general border color
        /// </summary>
        public Color generalBorderColor = Color.black;
        /// <summary>
        /// The left border
        /// </summary>
        public int leftBorder = 1;
        /// <summary>
        /// The left border type
        /// </summary>
        public BorderType leftBorderType;
        /// <summary>
        /// The left border color
        /// </summary>
        public Color leftBorderColor = Color.black;
        /// <summary>
        /// The top border
        /// </summary>
        public int topBorder = 1;
        /// <summary>
        /// The top border type
        /// </summary>
        public BorderType topBorderType;
        /// <summary>
        /// The top border color
        /// </summary>
        public Color topBorderColor = Color.black;
        /// <summary>
        /// The right border
        /// </summary>
        public int rightBorder = 1;
        /// <summary>
        /// The right border type
        /// </summary>
        public BorderType rightBorderType;
        /// <summary>
        /// The right border color
        /// </summary>
        public Color rightBorderColor = Color.black;
        /// <summary>
        /// The bottom border
        /// </summary>
        public int bottomBorder = 1;
        /// <summary>
        /// The bottom border type
        /// </summary>
        public BorderType bottomBorderType;
        /// <summary>
        /// The bottom border color
        /// </summary>
        public Color bottomBorderColor = Color.black;
        private List<BorderPoint> points = new List<BorderPoint>();
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureBorder"/> class.
        /// </summary>
        /// <param name="initialTexture">The initial texture.</param>
        public TextureBorder(Texture2D initialTexture)
        {
            if (initialTexture == null)
            {
                Debug.LogError("Texture passed is null");
                return;
            }
            initTex = initialTexture;
            points.Add(new BorderPoint(-initTex.width, initTex.height / 2, leftBorderColor));
            points.Add(new BorderPoint(initTex.width * 2, initTex.height / 2, rightBorderColor));
            points.Add(new BorderPoint(initTex.width / 2, 0, topBorderColor));
            points.Add(new BorderPoint(initTex.width / 2, initTex.height, topBorderColor));
        }
        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <param name="trans">if set to <c>true</c> [trans].</param>
        /// <returns>Texture2D.</returns>
        public Texture2D GetTexture(bool trans = false)
        {
            if (trans)
            {
                processedTexture = new Texture2D(initTex.width + generalBorder * 2, initTex.height + generalBorder * 2);
                var p = initTex.GetPixels(0, 0, initTex.width, initTex.height);
                processedTexture.SetPixels(generalBorder, generalBorder, initTex.width, initTex.height, p);
                processedTexture.Apply();
                for (int x = 0; x < processedTexture.width; ++x)
                    for (int y = 0; y < processedTexture.height; ++y)
                    {
                        Color nextPixel = initTex.GetPixel(x + generalBorder, y);
                        Color curColor = initTex.GetPixel(x, y);
                        if (nextPixel != curColor && curColor.a < 1)
                            processedTexture.SetPixel(x, y, generalBorderColor); //Has to be improved
                    }
                processedTexture.Apply();
                return processedTexture;
            }
            else
            {
                processedTexture = new Texture2D(initTex.width + leftBorder + rightBorder, initTex.height + topBorder + bottomBorder);
                for (int x = 0; x < processedTexture.width; ++x)
                    for (int y = 0; y < processedTexture.height; ++y)
                    {
                        if (leftBorder > 0)
                            if (x < leftBorder) processedTexture.SetPixel(x, y, leftBorderColor);
                        if (rightBorder > 0)
                            if (x > processedTexture.width - 1 - rightBorder) processedTexture.SetPixel(x, y, rightBorderColor);
                        if (topBorder > 0)
                            if (y < topBorder) processedTexture.SetPixel(x, y, topBorderColor);
                        if (bottomBorder > 0)
                            if (y > processedTexture.height - 1 - bottomBorder) processedTexture.SetPixel(x, y, bottomBorderColor);
                    }
                processedTexture.Apply();
                for (int x = 0; x < initTex.width; ++x)
                    for (int y = 0; y < initTex.height; ++y)
                        processedTexture.SetPixel(x + leftBorder, y + topBorder, new Color(0, 0, 0, 0));
                var p = initTex.GetPixels(0, 0, initTex.width, initTex.height);
                processedTexture.SetPixels(leftBorder, topBorder, initTex.width, initTex.height, p);
                processedTexture.Apply();
                return processedTexture;
            }
        }

        private Color GetColor(float x, float y, List<BorderPoint> points)
        {
            Color c = Color.black;
            float d = -1;
            foreach (BorderPoint p in points)
            {
                float actual = (new Vector2(p.x, p.y) - new Vector2(x, y)).magnitude;
                if (actual < d || d == -1)
                {
                    d = actual;
                    c = p.c;
                }
            }
            return c;
        }

        /// <summary>
        /// Simples the border.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="c">The c.</param>
        /// <returns>Texture2D.</returns>
        public static Texture2D SimpleBorder(Texture2D t, Color c) //By the moment I will leave it as a 1 pixel texture
        {
            for (int i = 0; i < t.width; ++i)
                for (int j = 0; j < t.height; ++j)
                    if (i == 0 || j == 0 || i == t.width - 1 || j == t.height - 1)
                        t.SetPixel(i, j, c);
            t.Apply();
            return t;
        }

        struct BorderPoint
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BorderPoint"/> struct.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="cl">The cl.</param>
            public BorderPoint(float x, float y, Color cl)
            {
                this.x = x;
                this.y = y;
                c = cl;
            }
            /// <summary>
            /// The x
            /// </summary>
            public float x, y;
            /// <summary>
            /// The c
            /// </summary>
            public Color c;
        }

    }

    /// <summary>
    /// Class TextureFixer.
    /// </summary>
    public class TextureFixer
    {
        private Texture2D processedTexture, initTex;
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureFixer"/> class.
        /// </summary>
        /// <param name="initialTexture">The initial texture.</param>
        public TextureFixer(Texture2D initialTexture)
        {
            if (initialTexture == null)
            {
                Debug.LogError("Texture passed is null");
                return;
            }
            initTex = initialTexture;
        }

        /// <summary>
        /// Fixes this instance.
        /// </summary>
        /// <returns>Texture2D.</returns>
        public Texture2D Fix()
        {
            Texture2D firstFix = IntFix(initTex);
            processedTexture = IntFix(firstFix);
            return processedTexture;
        }
        private Texture2D IntFix(Texture2D t)
        {
            Color lastColor = Color.black;
            Texture2D pt = new Texture2D(t.width, t.height);
            var p = t.GetPixels(0, 0, t.width, t.height);
            pt.SetPixels(0, 0, t.width, t.height, p);
            pt.Apply();
            //int i = 0;
            for (int y = 0; y < pt.height; ++y)
                for (int x = 0; x < pt.width; ++x)
                {
                    Color curColor = t.GetPixel(x, y);
                    if (curColor != lastColor) //Maybe checking near pixels with 2 more loops?
                        pt.SetPixel(x, y, t.GetPixel(x + 1, y));
                    lastColor = curColor;
                }
            pt.Apply();
            return pt;
        }
    }

    /// <summary>
    /// Class TextureScale.
    /// </summary>
    public class TextureScale
    {
        /// <summary>
        /// Class ThreadData.
        /// </summary>
        public class ThreadData
        {
            /// <summary>
            /// The start
            /// </summary>
            public int start;
            /// <summary>
            /// The end
            /// </summary>
            public int end;
            /// <summary>
            /// Initializes a new instance of the <see cref="ThreadData"/> class.
            /// </summary>
            /// <param name="s">The s.</param>
            /// <param name="e">The e.</param>
            public ThreadData(int s, int e)
            {
                start = s;
                end = e;
            }
        }

        private static Color[] texColors;
        private static Color[] newColors;
        private static int w;
        private static float ratioX;
        private static float ratioY;
        private static int w2;
        private static int finishCount;
        private static Mutex mutex;

        /// <summary>
        /// Points the specified tex.
        /// </summary>
        /// <param name="tex">The tex.</param>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        public static void Point(Texture2D tex, int newWidth, int newHeight)
        {
            ThreadedScale(tex, newWidth, newHeight, false);
        }

        /// <summary>
        /// Bilinears the specified tex.
        /// </summary>
        /// <param name="tex">The tex.</param>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
        {
            ThreadedScale(tex, newWidth, newHeight, true);
        }

        private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
        {
            texColors = tex.GetPixels();
            newColors = new Color[newWidth * newHeight];
            if (useBilinear)
            {
                ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
                ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
            }
            else
            {
                ratioX = ((float)tex.width) / newWidth;
                ratioY = ((float)tex.height) / newHeight;
            }
            w = tex.width;
            w2 = newWidth;
            var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
            var slice = newHeight / cores;

            finishCount = 0;
            if (mutex == null)
            {
                mutex = new Mutex(false);
            }
            if (cores > 1)
            {
                int i = 0;
                ThreadData threadData;
                for (i = 0; i < cores - 1; i++)
                {
                    threadData = new ThreadData(slice * i, slice * (i + 1));
                    ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                    Thread thread = new Thread(ts);
                    thread.Start(threadData);
                }
                threadData = new ThreadData(slice * i, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
                while (finishCount < cores)
                {
                    Thread.Sleep(1);
                }
            }
            else
            {
                ThreadData threadData = new ThreadData(0, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
            }

            tex.Resize(newWidth, newHeight);
            tex.SetPixels(newColors);
            tex.Apply();
        }

        /// <summary>
        /// Bilinears the scale.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void BilinearScale(System.Object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (var y = threadData.start; y < threadData.end; y++)
            {
                int yFloor = (int)Mathf.Floor(y * ratioY);
                var y1 = yFloor * w;
                var y2 = (yFloor + 1) * w;
                var yw = y * w2;

                for (var x = 0; x < w2; x++)
                {
                    int xFloor = (int)Mathf.Floor(x * ratioX);
                    var xLerp = x * ratioX - xFloor;
                    newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                                                           ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                                                           y * ratioY - yFloor);
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// Points the scale.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void PointScale(System.Object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (var y = threadData.start; y < threadData.end; y++)
            {
                var thisY = (int)(ratioY * y) * w;
                var yw = y * w2;
                for (var x = 0; x < w2; x++)
                {
                    newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
        {
            return new Color(c1.r + (c2.r - c1.r) * value,
                              c1.g + (c2.g - c1.g) * value,
                              c1.b + (c2.b - c1.b) * value,
                              c1.a + (c2.a - c1.a) * value);
        }
    }

    /*

         //public float animUpdate = .1f, res = 128;
        //public Vector2 offset;

        //private Texture2D[] bitcoinTextures;
        //private int bitcoinAnimIndex;
        //private float bitcoinTime;

     Start()

            /*Texture2D btcTex = Resources.Load<Texture2D>("Textures/bitcoin spin");
            string str = Resources.Load<TextAsset>("Text/sprites").text;
            Regex r = new Regex("(?<=\"frame\": \\{)\"x\":(?<x>.+?),\"y\":(?<y>.+?),");
            MatchCollection mc = r.Matches(str);
            List<Rect> rl = new List<Rect>();
            foreach(Match m in mc) 
                rl.Add(new Rect(int.Parse(m.Groups["x"].Value)+offset.x, int.Parse(m.Groups["y"].Value)+offset.y, res, res));
            Rect[] rs = rl.ToArray();
            bitcoinTextures = SliceFromRects(btcTex, rs);*/

    /*

    OnGUI();

     //DrawAnim(new Rect(5, 5, 130, 130), bitcoinTextures, ref bitcoinAnimIndex, ref bitcoinTime);

    /*void DrawAnim(Rect rect, Texture2D[] textures, ref int curIndex, ref float time)
        {
            if (curIndex == textures.Length)
                curIndex = 0;

            /*var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();

            GUI.DrawTexture(rect, textures[curIndex]);

            if (Time.time - time > animUpdate)
            {
                curIndex++;
                time = Time.time;
            }

        }

        Texture2D[] SliceFromRects(Texture2D tex, Rect[] rects)
        {
            List<Texture2D> texs = new List<Texture2D>();
            foreach(Rect r in rects) 
            {
                Texture2D t = new Texture2D((int)r.width, (int)r.height);
                var p = tex.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
                t.SetPixels(p);
                t.Apply();
                texs.Add(t);
            }
            return texs.ToArray();
        }*/

    /*private Bitmap ConvertBytesToImage(byte[] imageBytes)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            return null;
        }

        try
        {
            //Read bytes into a MemoryStream
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                //Recreate the frame from the MemoryStream
                using (Bitmap bmp = new Bitmap(ms))
                {
                    return (Bitmap)bmp.Clone();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Error type: " + ex.GetType().ToString() + "\n" +
                "Message: " + ex.Message,
                "Error in " + MethodBase.GetCurrentMethod().Name
                );
        }

        return null;
    }

    //Now that we have the frames and a way to recompile them, we'll
    //use the lbFrames ListBox to do so.
    private void lbFrames_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (lbFrames.SelectedIndex == -1)
            {
                return;
            }

            //Make sure frames have been extracted
            if (frames == null || frames.Count() == 0)
            {
                throw new NoNullAllowedException("Frames have not been extracted");
            }

            //Make sure the selected index is within range
            if (lbFrames.SelectedIndex > frames.Count() - 1)
            {
                throw new IndexOutOfRangeException("Frame list does not contain index: "
                + lbFrames.SelectedIndex.ToString());
            }

            //Clear the PictureBox
            ClearPictureBoxImage();

            //Load the image from the byte array
            pbImage.Image = ConvertBytesToImage(frames[lbFrames.SelectedIndex]);

        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Error type: " + ex.GetType().ToString() + "\n" +
                "Message: " + ex.Message,
                "Error in " + MethodBase.GetCurrentMethod().Name
                );
        }
    }*/
}