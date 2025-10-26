using UnityEngine;

public static class RectTransformExtensions
{
    private static readonly Vector3[] corners1 = new Vector3[4];
    private static readonly Vector3[] corners2 = new Vector3[4];

    /// <summary>
    /// 2つのRectTransformが画面上で重なっているかを判定する
    /// （Canvasモードに依存しない）
    /// </summary>
    public static bool IsOverlapping(this RectTransform rect1, RectTransform rect2)
    {
        rect1.GetWorldCorners(corners1);
        rect2.GetWorldCorners(corners2);

        for (int i = 0; i < 4; i++) {
            corners1[i] = RectTransformUtility.WorldToScreenPoint(null, corners1[i]);
            corners2[i] = RectTransformUtility.WorldToScreenPoint(null, corners2[i]);
        }

        Rect r1 = new Rect(corners1[0], corners1[2] - corners1[0]);
        Rect r2 = new Rect(corners2[0], corners2[2] - corners2[0]);

        return r1.Overlaps(r2);
    }
}