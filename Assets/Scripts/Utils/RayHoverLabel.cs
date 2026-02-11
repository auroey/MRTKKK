using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class RayHoverLabel : MonoBehaviour
{
    [Header("UI 引用")]
    public TextMeshPro labelText; // 拖入场景中那个固定的 TMP 物体
    public string defaultMessage = "请指向部位";

    [Header("射线引用")]
    public XRRayInteractor leftRay;
    public XRRayInteractor rightRay;

    void Update()
    {
        // 1. 内容更新逻辑：射线检测物体名称
        if (!UpdateTextContent(rightRay))
        {
            if (!UpdateTextContent(leftRay))
            {
                labelText.text = defaultMessage;
            }
        }

        // 2. 视角对齐逻辑：让文字始终与视角平行
        // 方案：让文字的旋转直接等于主相机的旋转，实现完美平行
        if (Camera.main != null)
        {
            labelText.transform.rotation = Camera.main.transform.rotation;
        }
    }

    bool UpdateTextContent(XRRayInteractor interactor)
    {
        if (interactor == null) return false;

        // 探测 3D 物体碰撞
        if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj != null)
            {
                // 清洗后缀逻辑
                string cleanName = hitObj.name
                    .Replace("_Controller", "")
                    .Replace(".g", "")
                    .Replace(".s", "")
                    .Replace(".j", "");
                
                labelText.text = cleanName;
                return true;
            }
        }
        return false;
    }
}