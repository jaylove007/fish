using UnityEngine;
using UnityEngine.UI;

namespace Fish.Prototype
{
    /// <summary>
    /// Screen-space reminder of the greybox controls. Built in play mode so the
    /// scene does not depend on a hand-authored canvas.
    /// </summary>
    public class PlayModeHints : MonoBehaviour
    {
        void Start()
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                Debug.LogWarning("PlayModeHints could not load LegacyRuntime.ttf.");
                return;
            }

            var canvasGo = new GameObject("HintCanvas");
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var textGo = new GameObject("Hint");
            textGo.transform.SetParent(canvasGo.transform, false);
            var text = textGo.AddComponent<Text>();
            text.font = font;
            text.fontSize = 24;
            text.color = new Color(1f, 0.96f, 0.9f, 0.94f);
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            text.text =
                "WASD 移动    鼠标环顾    Shift 慢跑    Esc 释放鼠标\n" +
                "WASD move    mouse look    Shift jog    Esc frees the cursor\n" +
                "Click the Game view to look around. Falling in the pond returns you to camp.";

            var rect = text.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(28f, -22f);
            rect.sizeDelta = new Vector2(1100f, 140f);

            var outline = textGo.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.78f);
            outline.effectDistance = new Vector2(1.4f, -1.4f);
            outline.useGraphicAlpha = true;
        }
    }
}
