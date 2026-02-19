// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

using UnityEngine;
using MixedReality.Toolkit;

/// <summary>
/// 点击物体时显示或切换指定 UI。
/// 需要与 StatefulInteractable 挂在同一物体上（或指定 Interactable）。
/// </summary>
[AddComponentMenu("Scripts/MRTK/Click To Show UI")]
public class ClickToShowUI : MonoBehaviour
{
    [Tooltip("要显示/隐藏的 UI（Canvas 或 Panel 等 GameObject）")]
    [SerializeField]
    private GameObject uiPanel;

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
