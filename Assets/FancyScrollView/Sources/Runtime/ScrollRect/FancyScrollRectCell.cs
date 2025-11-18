/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;

namespace FancyScrollView
{
    /// <summary>
    /// Abstract base class for implementing cells of <see cref="FancyScrollRect{TItemData, TContext}"/>. (<see cref="FancyScrollRect{TItemData, TContext}"/> のセルを実装するための抽象基底クラス。)
    /// If <see cref="FancyCell{TItemData, TContext}.Context"/> is not needed, use <see cref="FancyScrollRectCell{TItemData}"/> instead. (<see cref="FancyCell{TItemData, TContext}.Context"/> が不要な場合は代わりに <see cref="FancyScrollRectCell{TItemData}"/> を使用します。)
    /// </summary>
    /// <typeparam name="TItemData">Item data type. (アイテムのデータ型。)</typeparam>
    /// <typeparam name="TContext">Type of <see cref="FancyCell{TItemData, TContext}.Context"/>. (<see cref="FancyCell{TItemData, TContext}.Context"/> の型。)</typeparam>
    public abstract class FancyScrollRectCell<TItemData, TContext> : FancyCell<TItemData, TContext>
        where TContext : class, IFancyScrollRectContext, new()
    {
        /// <inheritdoc/>
        public override void UpdatePosition(float position)
        {
            var (scrollSize, reuseMargin) = Context.CalculateScrollSize();

            var normalizedPosition = (Mathf.Lerp(0f, scrollSize, position) - reuseMargin) / (scrollSize - reuseMargin * 2f);

            var start = 0.5f * scrollSize;
            var end = -start;

            UpdatePosition(normalizedPosition, Mathf.Lerp(start, end, position));
        }

        /// <summary>
        /// Updates the position of this cell. (このセルの位置を更新します。)
        /// </summary>
        /// <param name="normalizedPosition">
        /// Scroll position normalized in the viewport range. (ビューポートの範囲で正規化されたスクロール位置。)
        /// Based on the value of <see cref="FancyScrollRect{TItemData, TContext}.reuseCellMarginCount"/>, values beyond the range of <c>0.0</c> to <c>1.0</c> may be passed. (<see cref="FancyScrollRect{TItemData, TContext}.reuseCellMarginCount"/> の値に基づいて <c>0.0</c> ~ <c>1.0</c> の範囲を超えた値が渡されることがあります。)
        /// </param>
        /// <param name="localPosition">Local position. (ローカル位置。)</param>
        protected virtual void UpdatePosition(float normalizedPosition, float localPosition)
        {
            transform.localPosition = Context.ScrollDirection == ScrollDirection.Horizontal
                ? new Vector2(-localPosition, 0)
                : new Vector2(0, localPosition);
        }
    }

    /// <summary>
    /// Abstract base class for implementing cells of <see cref="FancyScrollRect{TItemData}"/>. (<see cref="FancyScrollRect{TItemData}"/> のセルを実装するための抽象基底クラス。)
    /// </summary>
    /// <typeparam name="TItemData">Item data type. (アイテムのデータ型。)</typeparam>
    /// <seealso cref="FancyScrollRectCell{TItemData, TContext}"/>
    public abstract class FancyScrollRectCell<TItemData> : FancyScrollRectCell<TItemData, FancyScrollRectContext>
    {
        /// <inheritdoc/>
        public sealed override void SetContext(FancyScrollRectContext context) => base.SetContext(context);
    }
}
