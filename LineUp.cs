using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

internal class LineUp : MonoBehaviour {
    private List<GameObject> objs = new List<GameObject>();
    [SerializeField] GameObject prefab;
    [SerializeField] Transform parent = null;
    private Transform Parent { get { return this.parent != null ? this.parent : this.transform; } }
    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] bool autoClear = true;
    [SerializeField] bool GenerateOnStart = true;
    [SerializeField] Copy[] copy = new Copy[1];
    [Header("--- Gizmos ---")]
    [SerializeField] float gizmosSize = 1f;
    [System.Serializable]
    private class Copy {
        [SerializeField] Vector3 offset;
        [SerializeField] int num;
        public GameObject[] CopyObject(GameObject obj, Vector3 originPos, Transform parent = null) {
            GameObject[] objs = new GameObject[this.num];
            for (int i = 0; i < this.num; i++) {
                var o = Instantiate(obj, parent);
                o.transform.localPosition = originPos + this.offset * (i + 1);
                objs[i] = o;
            }
            return objs;
        }
        public Vector3[] CopiedPos(Vector3 originPos) {
            Vector3[] pos = new Vector3[this.num];
            for (int i = 0; i < this.num; i++)
                pos[i] = originPos + this.offset * (i + 1);
            return pos;
        }
    }

    // Start is called before the first frame update
    void Start() {
        if (this.GenerateOnStart) LineUpObject();
    }

    void OnDrawGizmos() {
        List<Vector3> pos = new List<Vector3>();
        pos.Add(this.offset);
        foreach (var c in copy) {
            foreach (var p in pos.ToArray()) {
                var newPos = c.CopiedPos(p);
                foreach (var n in newPos) {
                    pos.Add(n);
                    var _n = this.Parent.TransformPoint(n);
                    var _p = this.Parent.TransformPoint(p);
                    Gizmos.DrawLine(_n, _p);
                    Gizmos.DrawWireSphere(_n, this.gizmosSize);
                }
            }
        }
    }

    public void LineUpObject() {
        if (this.autoClear) ClearObject();
        GameObject obj = Instantiate(this.prefab, this.Parent);
        obj.transform.localPosition = this.offset;
        this.objs.Add(obj);
        foreach (var c in copy) {
            foreach (var o in this.objs.ToArray()) {
                var newObjs = c.CopyObject(o, o.transform.localPosition, this.Parent);
                foreach (var n in newObjs)
                    objs.Add(n);
            }
        }
    }
    public void ClearObject() {
        foreach (var o in this.objs)
            DestroyImmediate(o);
        this.objs.Clear();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(LineUp))]
internal class LineUpEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        LineUp cls = target as LineUp;
        EditorGUILayout.LabelField("~~~ Controller ~~~");
        if (GUILayout.Button("Line Up!!")) cls.LineUpObject();
        if (GUILayout.Button("Clear!!")) cls.ClearObject();
    }
}
#endif
