/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using EasingCore;

namespace FancyScrollView
{
    /// <summary>
    /// Abstract base class for implementing ScrollRect-style scroll views. (ScrollRect スタイルのスクロールビューを実装するための抽象基底クラス。)
    /// Does not support infinite scrolling or snapping. (無限スクロールおよびスナップには対応していません。)
    /// If <see cref="FancyScrollView{TItemData, TContext}.Context"/> is not needed, use <see cref="FancyScrollRect{TItemData}"/> instead. (<see cref="FancyScrollView{TItemData, TContext}.Context"/> が不要な場合は代わりに <see cref="FancyScrollRect{TItemData}"/> を使用します。)
    /// </summary>
    /// <typeparam name="TItemData">Item data type. (アイテムのデータ型。)</typeparam>
    /// <typeparam name="TContext">Type of <see cref="FancyScrollView{TItemData, TContext}.Context"/>. (<see cref="FancyScrollView{TItemData, TContext}.Context"/> の型。)</typeparam>
    [RequireComponent(typeof(Scroller))]
    public abstract class FancyScrollRect<TItemData, TContext> : FancyScrollView<TItemData, TContext>
        where TContext : class, IFancyScrollRectContext, new()
    {
        /// <summary>
        /// Number of margin cells before cells are reused during scrolling. (スクロール中にセルが再利用されるまでの余白のセル数。)
        /// </summary>
        /// <remarks>
        /// Specifying <c>0</c> reuses cells immediately after they are completely hidden. (<c>0</c> を指定するとセルが完全に隠れた直後に再利用されます。)
        /// Specifying <c>1</c> or more scrolls that many extra cells before reusing. (<c>1</c> 以上を指定すると, そのセル数だけ余分にスクロールしてから再利用されます。)
        /// </remarks>
        [SerializeField] protected float reuseCellMarginCount = 0f;

        /// <summary>
        /// Padding at the beginning of content. (コンテンツ先頭の余白。)
        /// </summary>
        [SerializeField] protected float paddingHead = 0f;

        /// <summary>
        /// Padding at the end of content. (コンテンツ末尾の余白。)
        /// </summary>
        [SerializeField] protected float paddingTail = 0f;

        /// <summary>
        /// Spacing between cells in the scroll axis direction. (スクロール軸方向のセル同士の余白。)
        /// </summary>
        [SerializeField] protected float spacing = 0f;

        /// <summary>
        /// Cell size. (セルのサイズ。)
        /// </summary>
        protected abstract float CellSize { get; }

        /// <summary>
        /// Whether scrolling is possible. (スクロール可能かどうか。)
        /// </summary>
        /// <remarks>
        /// Returns <c>false</c> if the number of items is small enough that all cells fit within the viewport, otherwise <c>true</c>. (アイテム数が十分少なくビューポート内に全てのセルが収まっている場合は <c>false</c>, それ以外は <c>true</c> になります。)
        /// </remarks>
        protected virtual bool Scrollable => MaxScrollPosition > 0f;

        Scroller cachedScroller;

        /// <summary>
        /// Instance of <see cref="FancyScrollView.Scroller"/> that controls the scroll position. (スクロール位置を制御する <see cref="FancyScrollView.Scroller"/> のインスタンス。)
        /// </summary>
        /// <remarks>
        /// When changing the scroll position of <see cref="Scroller"/>, always use the position converted with <see cref="ToScrollerPosition(float)"/>. (<see cref="Scroller"/> のスクロール位置を変更する際は必ず <see cref="ToScrollerPosition(float)"/> を使用して変換した位置を使用してください。)
        /// </remarks>
        protected Scroller Scroller => cachedScroller ?? (cachedScroller = GetComponent<Scroller>());

        float ScrollLength => 1f / Mathf.Max(cellInterval, 1e-2f) - 1f;

        float ViewportLength => ScrollLength - reuseCellMarginCount * 2f;

        float PaddingHeadLength => (paddingHead - spacing * 0.5f) / (CellSize + spacing);

        float MaxScrollPosition => ItemsSource.Count
            - ScrollLength
            + reuseCellMarginCount * 2f
            + (paddingHead + paddingTail - spacing) / (CellSize + spacing);

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            Context.ScrollDirection = Scroller.ScrollDirection;
            Context.CalculateScrollSize = () =>
            {
                var interval = CellSize + spacing;
                var reuseMargin = interval * reuseCellMarginCount;
                var scrollSize = Scroller.ViewportSize + interval + reuseMargin * 2f;
                return (scrollSize, reuseMargin);
            };

            AdjustCellIntervalAndScrollOffset();
            Scroller.OnValueChanged(OnScrollerValueChanged);
        }

        /// <summary>
        /// Processing when the scroll position of <see cref="Scroller"/> is changed. (<see cref="Scroller"/> のスクロール位置が変更された際の処理。)
        /// </summary>
        /// <param name="p">Scroll position of <see cref="Scroller"/>. (<see cref="Scroller"/> のスクロール位置。)</param>
        void OnScrollerValueChanged(float p)
        {
            base.UpdatePosition(ToFancyScrollViewPosition(Scrollable ? p : 0f));

            if (Scroller.Scrollbar)
            {
                if (p > ItemsSource.Count - 1)
                {
                    ShrinkScrollbar(p - (ItemsSource.Count - 1));
                }
                else if (p < 0f)
                {
                    ShrinkScrollbar(-p);
                }
            }
        }

        /// <summary>
        /// Shrinks the scrollbar size based on the amount scrolled beyond the scroll range. (スクロール範囲を超えてスクロールされた量に基づいて, スクロールバーのサイズを縮小します。)
        /// </summary>
        /// <param name="offset">Amount scrolled beyond the scroll range. (スクロール範囲を超えてスクロールされた量。)</param>
        void ShrinkScrollbar(float offset)
        {
            var scale = 1f - ToFancyScrollViewPosition(offset) / (ViewportLength - PaddingHeadLength);
            UpdateScrollbarSize((ViewportLength - PaddingHeadLength) * scale);
        }

        /// <inheritdoc/>
        protected override void Refresh()
        {
            AdjustCellIntervalAndScrollOffset();
            RefreshScroller();
            base.Refresh();
        }

        /// <inheritdoc/>
        protected override void Relayout()
        {
            AdjustCellIntervalAndScrollOffset();
            RefreshScroller();
            base.Relayout();
        }

        /// <summary>
        /// Updates various states of <see cref="Scroller"/>. (<see cref="Scroller"/> の各種状態を更新します。)
        /// </summary>
        protected void RefreshScroller()
        {
            Scroller.Draggable = Scrollable;
            Scroller.ScrollSensitivity = ToScrollerPosition(ViewportLength - PaddingHeadLength);
            Scroller.Position = ToScrollerPosition(currentPosition);

            if (Scroller.Scrollbar)
            {
                Scroller.Scrollbar.gameObject.SetActive(Scrollable);
                UpdateScrollbarSize(ViewportLength);
            }
        }

        /// <inheritdoc/>
        protected override void UpdateContents(IList<TItemData> items)
        {
            AdjustCellIntervalAndScrollOffset();
            base.UpdateContents(items);

            Scroller.SetTotalCount(items.Count);
            RefreshScroller();
        }

        /// <summary>
        /// スクロール位置を更新します.
        /// </summary>
        /// <param name="position">スクロール位置.</param>
        protected new void UpdatePosition(float position)
        {
            Scroller.Position = ToScrollerPosition(position, 0.5f);
        }

        /// <summary>
        /// 指定したアイテムの位置までジャンプします.
        /// </summary>
        /// <param name="itemIndex">アイテムのインデックス.</param>
        /// <param name="alignment">ビューポート内におけるセル位置の基準. 0f(先頭) ~ 1f(末尾).</param>
        protected virtual void JumpTo(int itemIndex, float alignment = 0.5f)
        {
            Scroller.Position = ToScrollerPosition(itemIndex, alignment);
        }

        /// <summary>
        /// 指定したアイテムの位置まで移動します.
        /// </summary>
        /// <param name="index">アイテムのインデックス.</param>
        /// <param name="duration">移動にかける秒数.</param>
        /// <param name="alignment">ビューポート内におけるセル位置の基準. 0f(先頭) ~ 1f(末尾).</param>
        /// <param name="onComplete">移動が完了した際に呼び出されるコールバック.</param>
        protected virtual void ScrollTo(int index, float duration, float alignment = 0.5f, Action onComplete = null)
        {
            Scroller.ScrollTo(ToScrollerPosition(index, alignment), duration, onComplete);
        }

        /// <summary>
        /// 指定したアイテムの位置まで移動します.
        /// </summary>
        /// <param name="index">アイテムのインデックス.</param>
        /// <param name="duration">移動にかける秒数.</param>
        /// <param name="easing">移動に使用するイージング.</param>
        /// <param name="alignment">ビューポート内におけるセル位置の基準. 0f(先頭) ~ 1f(末尾).</param>
        /// <param name="onComplete">移動が完了した際に呼び出されるコールバック.</param>
        protected virtual void ScrollTo(int index, float duration, Ease easing, float alignment = 0.5f, Action onComplete = null)
        {
            Scroller.ScrollTo(ToScrollerPosition(index, alignment), duration, easing, onComplete);
        }

        /// <summary>
        /// ビューポートとコンテンツの長さに基づいてスクロールバーのサイズを更新します.
        /// </summary>
        /// <param name="viewportLength">ビューポートのサイズ.</param>
        protected void UpdateScrollbarSize(float viewportLength)
        {
            var contentLength = Mathf.Max(ItemsSource.Count + (paddingHead + paddingTail - spacing) / (CellSize + spacing), 1);
            Scroller.Scrollbar.size = Scrollable ? Mathf.Clamp01(viewportLength / contentLength) : 1f;
        }

        /// <summary>
        /// <see cref="Scroller"/> が扱うスクロール位置を <see cref="FancyScrollRect{TItemData, TContext}"/> が扱うスクロール位置に変換します.
        /// </summary>
        /// <param name="position"><see cref="Scroller"/> が扱うスクロール位置.</param>
        /// <returns><see cref="FancyScrollRect{TItemData, TContext}"/> が扱うスクロール位置.</returns>
        protected float ToFancyScrollViewPosition(float position)
        {
            return position / Mathf.Max(ItemsSource.Count - 1, 1) * MaxScrollPosition - PaddingHeadLength;
        }

        /// <summary>
        /// Converts the scroll position handled by <see cref="FancyScrollRect{TItemData, TContext}"/> to the scroll position handled by <see cref="Scroller"/>. (<see cref="FancyScrollRect{TItemData, TContext}"/> が扱うスクロール位置を <see cref="Scroller"/> が扱うスクロール位置に変換します。)
        /// </summary>
        /// <param name="position">Scroll position handled by <see cref="FancyScrollRect{TItemData, TContext}"/>. (<see cref="FancyScrollRect{TItemData, TContext}"/> が扱うスクロール位置。)</param>
        /// <returns>Scroll position handled by <see cref="Scroller"/>. (<see cref="Scroller"/> が扱うスクロール位置。)</returns>
        protected float ToScrollerPosition(float position)
        {
            return (position + PaddingHeadLength) / MaxScrollPosition * Mathf.Max(ItemsSource.Count - 1, 1);
        }

        /// <summary>
        /// Converts the scroll position handled by <see cref="FancyScrollRect{TItemData, TContext}"/> to the scroll position handled by <see cref="Scroller"/>. (<see cref="FancyScrollRect{TItemData, TContext}"/> が扱うスクロール位置を <see cref="Scroller"/> が扱うスクロール位置に変換します。)
        /// </summary>
        /// <param name="position">Scroll position handled by <see cref="FancyScrollRect{TItemData, TContext}"/>. (<see cref="FancyScrollRect{TItemData, TContext}"/> が扱うスクロール位置。)</param>
        /// <param name="alignment">Reference for cell position within viewport. 0f (beginning) to 1f (end). (ビューポート内におけるセル位置の基準。 0f(先頭) ~ 1f(末尾)。)</param>
        /// <returns>Scroll position handled by <see cref="Scroller"/>. (<see cref="Scroller"/> が扱うスクロール位置。)</returns>
        protected float ToScrollerPosition(float position, float alignment = 0.5f)
        {
            var offset = alignment * (ScrollLength - (1f + reuseCellMarginCount * 2f))
                + (1f - alignment - 0.5f) * spacing / (CellSize + spacing);
            return ToScrollerPosition(Mathf.Clamp(position - offset, 0f, MaxScrollPosition));
        }

        /// <summary>
        /// Calculates and applies <see cref="FancyScrollView{TItemData,TContext}.cellInterval"/> and <see cref="FancyScrollView{TItemData,TContext}.scrollOffset"/> to achieve the specified settings. (指定された設定を実現するための <see cref="FancyScrollView{TItemData,TContext}.cellInterval"/> と <see cref="FancyScrollView{TItemData,TContext}.scrollOffset"/> を計算して適用します。)
        /// </summary>
        protected void AdjustCellIntervalAndScrollOffset()
        {
            var totalSize = Scroller.ViewportSize + (CellSize + spacing) * (1f + reuseCellMarginCount * 2f);
            cellInterval = (CellSize + spacing) / totalSize;
            scrollOffset = cellInterval * (1f + reuseCellMarginCount);
        }

        protected virtual void OnValidate()
        {
            AdjustCellIntervalAndScrollOffset();

            if (loop)
            {
                loop = false;
                Debug.LogError("Loop is currently not supported in FancyScrollRect.");
            }

            if (Scroller.SnapEnabled)
            {
                Scroller.SnapEnabled = false;
                Debug.LogError("Snap is currently not supported in FancyScrollRect.");
            }

            if (Scroller.MovementType == MovementType.Unrestricted)
            {
                Scroller.MovementType = MovementType.Elastic;
                Debug.LogError("MovementType.Unrestricted is currently not supported in FancyScrollRect.");
            }
        }
    }

    /// <summary>
    /// Abstract base class for implementing ScrollRect-style scroll views. (ScrollRect スタイルのスクロールビューを実装するための抽象基底クラス。)
    /// Does not support infinite scrolling or snapping. (無限スクロールおよびスナップには対応していません。)
    /// </summary>
    /// <typeparam name="TItemData">Item data type. (アイテムのデータ型。)</typeparam>
    /// <seealso cref="FancyScrollRect{TItemData, TContext}"/>
    public abstract class FancyScrollRect<TItemData> : FancyScrollRect<TItemData, FancyScrollRectContext> { }
}
