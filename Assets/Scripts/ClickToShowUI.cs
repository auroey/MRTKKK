// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit;
using TMPro;

/// <summary>
/// Shows or toggles a UI when the object is clicked. Requires StatefulInteractable on the same object (or assigned).
/// </summary>
[AddComponentMenu("Scripts/MRTK/Click To Show UI")]
public class ClickToShowUI : MonoBehaviour
{
    /// <summary>
    /// Static default description library. Key = organ name (case-insensitive), Value = default description.
    /// Used when organDescription is left empty in the Inspector.
    /// </summary>
    private static readonly Dictionary<string, string> DefaultOrganDescriptions = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
    {
        { "Sense organs", "Collect light, sound, chemical and mechanical stimuli from the environment and convert them into electrical signals for the central nervous system." },
        { "Central nervous system", "Composed of the brain and spinal cord, it is the body's command center, integrating sensory information and issuing motor commands." },
        { "Peripheral nervous system", "Connects the central nervous system to the limbs and viscera, relaying information bidirectionally between the brain and the rest of the body." },
        { "Digestive system", "Responsible for the physical and chemical breakdown of food, extracting nutrients for life processes and excreting metabolic waste." },
        { "Cranium", "Provides a rigid protective enclosure for the brain and supports the facial skeleton and positioning of sensory organs." },
        { "Bony pelvis", "Bears the weight of the upper body, protects pelvic organs, and serves as a stable attachment base for muscles of the lower limb." },
        { "Bones of upper limb", "Provide a lever system that supports grasping, load-bearing and fine hand movements." },
        { "Bones of lower limb", "Form the main weight-bearing framework of the body, maintaining balance and enabling walking and running through complex joints." },
        { "Cranial part of muscular system", "Controls facial expression, chewing and precise movements of the head in space." },
        { "Abdominal part of muscular system", "Forms the abdominal wall, protects viscera, assists in breathing and provides core stability during trunk movement." },
        { "Muscular system of lower limb", "Drives movement at the joints of the lower limb and provides the force for standing, postural control and locomotion." }
    };

    [Tooltip("要显示/隐藏的 UI（Canvas 或 Panel 等 GameObject）")]
    [SerializeField]
    private GameObject uiPanel;

    [Tooltip("可选：显示描述的 TextMeshPro。若指定，点击时显示「器官名：器官功能描述」。")]
    [SerializeField]
    private TMP_Text descriptionText;

    [Tooltip("器官显示名。留空则从物体名自动取（并去掉末尾的 Controller）。")]
    [SerializeField]
    private string organDisplayName;

    [Tooltip("器官功能描述，格式示例：心脏：负责泵血，将血液输送到全身。")]
    [SerializeField]
    [TextArea(2, 4)]
    private string organDescription;

    [Tooltip("为 true 时：每次点击在显示/隐藏之间切换；为 false 时：每次点击只显示 UI")]
    [SerializeField]
    private bool toggleMode = true;

    [Tooltip("若为空则使用同物体上的 StatefulInteractable")]
    [SerializeField]
    private StatefulInteractable interactable;

    private void Awake()
    {
        if (interactable == null)
        {
            interactable = GetComponent<StatefulInteractable>();
        }

        if (interactable == null)
        {
            Debug.LogWarning("ClickToShowUI: 未找到 StatefulInteractable，请在该物体上添加 StatefulInteractable 或指定 Interactable。", this);
            return;
        }

        if (uiPanel != null && !toggleMode)
        {
            uiPanel.SetActive(false);
        }

        interactable.OnClicked.AddListener(OnObjectClicked);
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.OnClicked.RemoveListener(OnObjectClicked);
        }
    }

    private void OnObjectClicked()
    {
        if (descriptionText != null)
        {
            descriptionText.text = GetDescriptionText();
        }

        if (uiPanel == null)
        {
            return;
        }

        if (toggleMode)
        {
            uiPanel.SetActive(!uiPanel.activeSelf);
        }
        else
        {
            uiPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Gets the description text to display. Priority: 1) Inspector organDescription, 2) static default library by organ name, 3) organ name only.
    /// </summary>
    private string GetDescriptionText()
    {
        string name = string.IsNullOrWhiteSpace(organDisplayName)
            ? GetNameWithoutController(gameObject.name)
            : organDisplayName.Trim();

        string description;

        if (!string.IsNullOrWhiteSpace(organDescription))
        {
            description = organDescription.Trim();
        }
        else if (DefaultOrganDescriptions.TryGetValue(name, out string defaultDesc))
        {
            description = defaultDesc;
        }
        else
        {
            return name;
        }

        return $"{name}: {description}";
    }

    /// <summary>
    /// Removes the "_Controller" suffix from the object name (case-insensitive). Object names always end with "_Controller".
    /// </summary>
    private static string GetNameWithoutController(string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) return objectName;
        const string suffix = "_Controller";
        if (objectName.Length > suffix.Length &&
            objectName.EndsWith(suffix, System.StringComparison.OrdinalIgnoreCase))
        {
            return objectName.Substring(0, objectName.Length - suffix.Length).TrimEnd();
        }
        return objectName;
    }

    /// <summary>
    /// 供外部调用：关闭 UI（例如 UI 上的“关闭”按钮）。
    /// </summary>
    public void HideUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 供外部调用：显示 UI。
    /// </summary>
    public void ShowUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }
    }
}
