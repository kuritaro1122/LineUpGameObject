using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LineUpOnBox : MonoBehaviour {
    [SerializeField, HideInInspector] List<GameObject> objs = new List<GameObject>();
    [SerializeField] GameObject prefab;
    [SerializeField] Transform parent = null;
    private Transform Parent { get { return this.parent != null ? this.parent : this.transform; } }
    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] bool autoClear = true;
    [SerializeField] bool GenerateOnStart = true;
    [SerializeField] Vector3 distance = Vector3.zero;
    [SerializeField] Vector3[] copy = new Vector3[1];
    [Header("--- Gizmos ---")]
    [SerializeField] float gizmosSize = 1f;
    [SerializeField] bool drawGizmos = true;

    // Start is called before the first frame update
    void Start() {
        if (this.GenerateOnStart) LineUpObject();
    }
    void OnDrawGizmos() {
        if (!this.drawGizmos) return;
        foreach (var p in this.copy) {
            Gizmos.DrawWireSphere(this.Parent.TransformPoint(Vector3.Scale(this.distance, p) + this.offset), this.gizmosSize);
        }
    }

    public void LineUpObject() {
        if (this.autoClear) ClearObject();
        foreach (var p in this.copy) {
            GameObject obj = Instantiate(this.prefab, this.Parent);
            obj.transform.localPosition = this.offset + Vector3.Scale(this.distance, p);
            this.objs.Add(obj);
        }
    }
    public void ClearObject() {
        foreach (var o in this.objs)
            DestroyImmediate(o);
        this.objs.Clear();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LineUpOnBox))]
internal class LineUpOnBoxEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        LineUpOnBox cls = target as LineUpOnBox;
        EditorGUILayout.LabelField("~~~ Controller ~~~");
        if (GUILayout.Button("Line Up!!")) cls.LineUpObject();
        if (GUILayout.Button("Clear!!")) cls.ClearObject();
    }
}
#endif