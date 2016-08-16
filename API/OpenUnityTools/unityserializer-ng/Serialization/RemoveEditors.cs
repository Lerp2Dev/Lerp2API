using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Storage/Internal/Cleanup Maintenance (Immediately removes itself)")]
public class RemoveEditors : MonoBehaviour {
    private void Awake() {
        LevelSerializer.SavedGames.Clear();
        LevelSerializer.SaveDataToFilePrefs();

        DestroyImmediate(this);
    }
}

