using UnityEngine;
using UnityEditor;
using CBX.TileMapping.Unity;

// TilemapLayerHelper
// Use PageDown/PageUp to select between tilemap layers
// First assign tileRoot into the editorwindow field and hit GetTileMaps button
// Set scene view zoom: Press G to toggle between: Framed, PixelPerfect, HalfWay

namespace Lerp2APIEditor.Utility.UnityLib.Two_D.TileMaps
{
    /// <summary>
    /// Class TilemapLayerHelper.
    /// </summary>
    public class TilemapLayerHelper : EditorWindow
    {
        private Transform tileRoot;
        private GameObject[] tilemapGos;

        private bool fadeOtherLayers = false;
        private int selectedLayer = 0;
        private string[] layerNames = new string[] { "" };

        // new frame tool
        private FrameMode frameMode = FrameMode.Framed;

        [MenuItem("Window/TilemapLayerHelper")]
        private static void Init()
        {
            TilemapLayerHelper window = (TilemapLayerHelper)EditorWindow.GetWindow(typeof(TilemapLayerHelper));
            window.titleContent = new GUIContent("TilemapLayerHelper");
            window.minSize = new Vector2(320, 128);
            window.Show();
        }

        private void OnEnable()
        {
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

            // reset colors on exit
            fadeOtherLayers = false;
            SetTileMapLayerColors();
        }

        private void OnGUI()
        {
            GUILayout.Label("tileRoot", EditorStyles.boldLabel);
            GUI.changed = false;
            tileRoot = (Transform)EditorGUILayout.ObjectField("", tileRoot, typeof(Transform), true);
            if (GUI.changed && tileRoot)
            {
                // get list of tilemap layers
                var childTileMaps = tileRoot.GetComponentsInChildren<TileMap>();
                layerNames = new string[childTileMaps.Length];
                tilemapGos = new GameObject[childTileMaps.Length];

                int i = 0;
                foreach (var tm in childTileMaps)
                {
                    layerNames[i] = tm.name;
                    tilemapGos[i] = tm.gameObject;
                    i++;
                }
            }

            GUI.changed = false;
            fadeOtherLayers = GUILayout.Toggle(fadeOtherLayers, "Fade other layers");

            if (GUI.changed) // reset tilemap layer colors
            {
                if (tileRoot == null) return;
                SetTileMapLayerColors();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (tileRoot == null) return;

            // USER EVENTS
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    case KeyCode.PageUp: // select higher
                        selectedLayer = Wrap(--selectedLayer, layerNames.Length);
                        Selection.activeGameObject = tilemapGos[selectedLayer];
                        e.Use();
                        break;

                    case KeyCode.PageDown: // select lower
                        selectedLayer = Wrap(++selectedLayer, layerNames.Length);
                        Selection.activeGameObject = tilemapGos[selectedLayer];
                        e.Use();
                        break;

                    case KeyCode.Escape: // reset brush, TODO
                        break;

                    case KeyCode.G: // Toggle Frame selected and PixelPerfectZoom *Cannot override F key..

                        switch (frameMode)
                        {
                            case FrameMode.Framed:
                                Selection.activeGameObject = tilemapGos[selectedLayer];
                                sceneView.FrameSelected();
                                frameMode = FrameMode.PixelPerfect;
                                break;

                            case FrameMode.PixelPerfect:
                                if (tilemapGos[selectedLayer] != null)
                                {
                                    //                                var tm = tilemapGos[selectedLayer].GetComponent<TileMap>();
                                    //                                var s = tm.GetSprite(new Vector3Int(0, 0, 0));
                                    //                                var tr = tilemapGos[selectedLayer].GetComponent<TileMapRenderer>();

                                    var cam = SceneView.currentDrawingSceneView.camera;
                                    var pixelPerUnits = 16; // TODO get from sprites
                                    var orthoSize = cam.pixelHeight / pixelPerUnits / 2;

                                    Vector3 mousePosition = Event.current.mousePosition;
                                    mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
                                    var mouseWorldPos = cam.ScreenToWorldPoint(mousePosition);

                                    sceneView.LookAtDirect(mouseWorldPos, SceneView.lastActiveSceneView.rotation, orthoSize);
                                    //sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, SceneView.lastActiveSceneView.rotation, orthoSize);
                                }
                                frameMode = FrameMode.HalfWay;
                                break;

                            case FrameMode.HalfWay:
                                if (tilemapGos[selectedLayer] != null)
                                {
                                    var cam = SceneView.currentDrawingSceneView.camera;
                                    var pixelPerUnits = 16; // TODO get from sprites
                                    var orthoSize = cam.pixelHeight / pixelPerUnits * 2;

                                    Vector3 mousePosition = Event.current.mousePosition;
                                    mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
                                    var mouseWorldPos = cam.ScreenToWorldPoint(mousePosition);

                                    sceneView.LookAtDirect(mouseWorldPos, SceneView.lastActiveSceneView.rotation, orthoSize);
                                }
                                frameMode = FrameMode.Framed;
                                break;

                            default:
                                break;
                        }

                        e.Use();
                        break;

                    default:
                        break;
                }
            }

            // SCENE UI
            Handles.BeginGUI();
            selectedLayer = GUI.SelectionGrid(new Rect(0, 32, 128, 64), selectedLayer, layerNames, 1);
            if (fadeOtherLayers)
            {
                SetTileMapLayerColors();
            }
            Handles.EndGUI();
        }

        // http://answers.unity3d.com/answers/249513/view.html
        private Rect BoundsToScreenRect(Bounds bounds, Camera cam)
        {
            Vector3 origin = cam.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
            Vector3 extent = cam.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));
            // Create rect in screen space and return - does not account for camera perspective
            return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
        }

        // make non-selected layers semi-transparent
        private void SetTileMapLayerColors()
        {
            for (int i = 0; i < layerNames.Length; i++)
            {
                if (i == selectedLayer || !fadeOtherLayers)
                {
                    tilemapGos[i].GetComponent<TileMap>().color = Color.white;
                }
                else
                {
                    tilemapGos[i].GetComponent<TileMap>().color = Color.white * 0.5f;
                }
            }
        }

        // HELPERS

        // wrap value between 0-max
        private int Wrap(int i, int i_max)
        {
            return ((i % i_max) + i_max) % i_max;
        }
    }
}