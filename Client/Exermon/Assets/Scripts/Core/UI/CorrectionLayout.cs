using UI.Common.Controls.ItemDisplays;
using UI.ExerPro.EnglishPro.CorrectionScene.Controls;
using UI.ExerPro.EnglishPro.CorrectionScene.Controls.Test;
using UnityEngine;
using UnityEngine.EventSystems;
//改错布局
namespace Assets.Scripts.Core.UI {
    public class CorrectionLayout : UIBehaviour {

        public ArticleTestDisplay articledisplay;
        public GameObject content;

        public void initialize() {
            //string[] items = { "aaa", "bbb", "ccc" };
            //articledisplay.setItems(items);

            ItemDisplay<string>[] words = articledisplay.getSubViews();
            RectTransform start = content.GetComponent<RectTransform>();
            Debug.Log("aaa" + articledisplay.subViewsCount());
            RectTransform transform = (words[0] as WordTestDisplay).gameObject.GetComponent<RectTransform>();

            Debug.Log("aaa" + start.rect.size);
            float x = 0.0f;
            float y = 0.0f;
            float spacing = 10.0f;
            for (int i = 0; i < articledisplay.subViewsCount(); i++) {
                transform = (words[i] as WordTestDisplay).gameObject.GetComponent<RectTransform>();

                if (x + transform.rect.size.x + spacing >= 620.0f) {
                    x = 0.0f;
                    y -= 50.0f + spacing;
                }
                transform.localPosition = new Vector3(x, y, 0.0f);
                x += transform.rect.size.x + spacing;
                Debug.Log("aaa" + i + "-" + x + "," + y);

            }

        }
        ///// <summary>
        ///// Which corner is the starting corner for the grid.
        ///// </summary>
        //public enum Corner {
        //    /// <summary>
        //    /// Upper Left corner.
        //    /// </summary>
        //    UpperLeft = 0,
        //    /// <summary>
        //    /// Upper Right corner.
        //    /// </summary>
        //    UpperRight = 1,
        //    /// <summary>
        //    /// Lower Left corner.
        //    /// </summary>
        //    LowerLeft = 2,
        //    /// <summary>
        //    /// Lower Right corner.
        //    /// </summary>
        //    LowerRight = 3
        //}

        ///// <summary>
        ///// The grid axis we are looking at.
        ///// </summary>
        ///// <remarks>
        ///// As the storage is a [][] we make access easier by passing a axis.
        ///// </remarks>
        //public enum Axis {
        //    /// <summary>
        //    /// Horizontal axis
        //    /// </summary>
        //    Horizontal = 0,
        //    /// <summary>
        //    /// Vertical axis.
        //    /// </summary>
        //    Vertical = 1
        //}

        //[SerializeField] protected Corner m_StartCorner = Corner.UpperLeft;

        ///// <summary>
        ///// Which corner should the first cell be placed in?
        ///// </summary>
        //public Corner startCorner { get { return m_StartCorner; } set { SetProperty(ref m_StartCorner, value); } }
        //[SerializeField] protected Axis m_StartAxis = Axis.Horizontal;

        ///// <summary>
        ///// Which axis should cells be placed along first
        ///// </summary>
        ///// <remarks>
        ///// When startAxis is set to horizontal, an entire row will be filled out before proceeding to the next row. When set to vertical, an entire column will be filled out before proceeding to the next column.
        ///// </remarks>
        //public Axis startAxis { get { return m_StartAxis; } set { SetProperty(ref m_StartAxis, value); } }


        //protected CorrectionLayout() { }

        ///// <summary>
        ///// Called by the layout system to calculate the horizontal layout size.
        ///// Also see ILayoutElement
        ///// </summary>
        //public override void CalculateLayoutInputHorizontal() {
        //    base.CalculateLayoutInputHorizontal();

        //    SetLayoutInputForAxis(645, 645, -1, 0);

        //}

        ///// <summary>
        ///// Called by the layout system to calculate the vertical layout size.
        ///// Also see ILayoutElement
        ///// </summary>
        //public override void CalculateLayoutInputVertical() {
        //    SetLayoutInputForAxis(600, 600, -1, 1);
        //}

        ///// <summary>
        ///// Called by the layout system
        ///// Also see ILayoutElement
        ///// </summary>
        //public override void SetLayoutHorizontal() {
        //    SetCellsAlongAxis(0);
        //}

        ///// <summary>
        ///// Called by the layout system
        ///// Also see ILayoutElement
        ///// </summary>
        //public override void SetLayoutVertical() {
        //    SetCellsAlongAxis(1);
        //}

        //private void SetCellsAlongAxis(int axis) {
        //    // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
        //    // and only vertical values when invoked for the vertical axis.
        //    // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
        //    // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
        //    // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.

        //    if (axis == 0) {
        //        // Only set the sizes when invoked for horizontal axis, not the positions.
        //        for (int i = 0; i < rectChildren.Count; i++) {
        //            RectTransform rect = rectChildren[i];

        //            m_Tracker.Add(this, rect,
        //                DrivenTransformProperties.Anchors |
        //                DrivenTransformProperties.AnchoredPosition |
        //                DrivenTransformProperties.SizeDelta);

        //            rect.anchorMin = Vector2.up;
        //            rect.anchorMax = Vector2.up;
        //            rect.sizeDelta = cellSize;
        //        }
        //        return;
        //    }

        //    float width = rectTransform.rect.size.x;
        //    float height = rectTransform.rect.size.y;

        //    int cellCountX = 1;
        //    int cellCountY = 1;
        //    if (m_Constraint == Constraint.FixedColumnCount) {
        //        cellCountX = m_ConstraintCount;
        //        cellCountY = Mathf.CeilToInt(rectChildren.Count / (float)cellCountX - 0.001f);
        //    }
        //    else if (m_Constraint == Constraint.FixedRowCount) {
        //        cellCountY = m_ConstraintCount;
        //        cellCountX = Mathf.CeilToInt(rectChildren.Count / (float)cellCountY - 0.001f);
        //    }
        //    else {
        //        if (cellSize.x + spacing.x <= 0)
        //            cellCountX = int.MaxValue;
        //        else
        //            cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));

        //        if (cellSize.y + spacing.y <= 0)
        //            cellCountY = int.MaxValue;
        //        else
        //            cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
        //    }

        //    int cornerX = (int)startCorner % 2;
        //    int cornerY = (int)startCorner / 2;

        //    int cellsPerMainAxis, actualCellCountX, actualCellCountY;
        //    if (startAxis == Axis.Horizontal) {
        //        cellsPerMainAxis = cellCountX;
        //        actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildren.Count);
        //        actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
        //    }
        //    else {
        //        cellsPerMainAxis = cellCountY;
        //        actualCellCountY = Mathf.Clamp(cellCountY, 1, rectChildren.Count);
        //        actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
        //    }

        //    Vector2 requiredSpace = new Vector2(
        //        actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
        //        actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
        //    );
        //    Vector2 startOffset = new Vector2(
        //        GetStartOffset(0, requiredSpace.x),
        //        GetStartOffset(1, requiredSpace.y)
        //    );

        //    for (int i = 0; i < rectChildren.Count; i++) {
        //        int positionX;
        //        int positionY;
        //        if (startAxis == Axis.Horizontal) {
        //            positionX = i % cellsPerMainAxis;
        //            positionY = i / cellsPerMainAxis;
        //        }
        //        else {
        //            positionX = i / cellsPerMainAxis;
        //            positionY = i % cellsPerMainAxis;
        //        }

        //        if (cornerX == 1)
        //            positionX = actualCellCountX - 1 - positionX;
        //        if (cornerY == 1)
        //            positionY = actualCellCountY - 1 - positionY;

        //        SetChildAlongAxis(rectChildren[i], 0, startOffset.x + (cellSize[0] + spacing[0]) * positionX, cellSize[0]);
        //        SetChildAlongAxis(rectChildren[i], 1, startOffset.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);
        //    }
        //}

    }
}
