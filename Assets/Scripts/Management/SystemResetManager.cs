using UnityEngine;
using System.Collections.Generic;

public class SystemResetManager : MonoBehaviour
{
    public GameObject[] systemRoots; 

    // 1. 在结构体中增加 scale 字段
    private struct TransformData {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale; // 新增：用于记录缩放
    }

    private Dictionary<GameObject, TransformData> initialSnapshots = new Dictionary<GameObject, TransformData>();

    void Awake() {
        foreach (GameObject root in systemRoots) {
            if (root != null) {
                FindControllersRecursive(root.transform);
            }
        }
    }

    void FindControllersRecursive(Transform parent) {
        foreach (Transform child in parent) {
            if (child.name.EndsWith("_Controller")) {
                // 2. 在记录时同时保存 localScale
                initialSnapshots[child.gameObject] = new TransformData {
                    position = child.localPosition,
                    rotation = child.localRotation,
                    scale = child.localScale // 新增：保存初始缩放
                };
            }
            FindControllersRecursive(child);
        }
    }

    public void ResetAllControllers() {
        foreach (var entry in initialSnapshots) {
            GameObject obj = entry.Key;
            TransformData data = entry.Value;

            if (obj != null) {
                obj.transform.localPosition = data.position;
                obj.transform.localRotation = data.rotation;
                
                // 3. 在重置时恢复缩放
                obj.transform.localScale = data.scale; 

                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null) {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }
}