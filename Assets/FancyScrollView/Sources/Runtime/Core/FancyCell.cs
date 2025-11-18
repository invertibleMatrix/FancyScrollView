/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;

namespace FancyScrollView
{
    /// <summary>
    /// Abstract base class for implementing cells of <see cref="FancyScrollView{TItemData, TContext}"/>. (<see cref="FancyScrollView{TItemData, TContext}"/> のセルを実装するための抽象基底クラス。)
    /// If <see cref="FancyCell{TItemData, TContext}.Context"/> is not needed, use <see cref="FancyCell{TItemData}"/> instead. (<see cref="FancyCell{TItemData, TContext}.Context"/> が不要な場合は代わりに <see cref="FancyCell{TItemData}"/> を使用します。)
    /// </summary>
    /// <typeparam name="TItemData">Item data type. (アイテムのデータ型。)</typeparam>
    /// <typeparam name="TContext">Type of <see cref="Context"/>. (<see cref="Context"/> の型。)</typeparam>
    public abstract class FancyCell<TItemData, TContext> : MonoBehaviour where TContext : class, new()
    {
        /// <summary>
        /// Index of the data displayed by this cell. (このセルで表示しているデータのインデックス。)
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// Visibility state of this cell. (このセルの可視状態。)
        /// </summary>
        public virtual bool IsVisible => gameObject.activeSelf;

        /// <summary>
        /// Reference to <see cref="FancyScrollView{TItemData, TContext}.Context"/>. (<see cref="FancyScrollView{TItemData, TContext}.Context"/> の参照。)
        /// The same instance is shared between cells and scroll views. Used for passing information and maintaining state. (セルとスクロールビュー間で同じインスタンスが共有されます。 情報の受け渡しや状態の保持に使用します。)
        /// </summary>
        protected TContext Context { get; private set; }

        /// <summary>
        /// Sets the <see cref="Context"/>. (<see cref="Context"/> をセットします。)
        /// </summary>
        /// <param name="context">Context. (コンテキスト。)</param>
        public virtual void SetContext(TContext context) => Context = context;

        /// <summary>
        /// Performs initialization. (初期化を行います。)
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Sets the visibility state of this cell. (このセルの可視状態を設定します。)
        /// </summary>
        /// <param name="visible"><c>true</c> if visible, <c>false</c> if not visible. (可視状態なら <c>true</c>, 非可視状態なら <c>false</c>。)</param>
        public virtual void SetVisible(bool visible) => gameObject.SetActive(visible);

        /// <summary>
        /// Updates the display content of this cell based on item data. (アイテムデータに基づいてこのセルの表示内容を更新します。)
        /// </summary>
        /// <param name="itemData">Item data. (アイテムデータ。)</param>
        public abstract void UpdateContent(TItemData itemData);

        /// <summary>
        /// Updates the scroll position of this cell based on values from <c>0.0f</c> to <c>1.0f</c>. (<c>0.0f</c> ~ <c>1.0f</c> の値に基づいてこのセルのスクロール位置を更新します。)
        /// </summary>
        /// <param name="position">Normalized scroll position in the viewport range. (ビューポート範囲の正規化されたスクロール位置。)</param>
        public abstract void UpdatePosition(float position);
    }

    /// <summary>
    /// Abstract base class for implementing cells of <see cref="FancyScrollView{TItemData}"/>. (<see cref="FancyScrollView{TItemData}"/> のセルを実装するための抽象基底クラス。)
    /// </summary>
    /// <typeparam name="TItemData">Item data type. (アイテムのデータ型。)</typeparam>
    /// <seealso cref="FancyCell{TItemData, TContext}"/>
    public abstract class FancyCell<TItemData> : FancyCell<TItemData, NullContext>
    {
        /// <inheritdoc/>
        public sealed override void SetContext(NullContext context) => base.SetContext(context);
    }
}
