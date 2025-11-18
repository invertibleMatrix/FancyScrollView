/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections.Generic;
using UnityEngine;

namespace FancyScrollView
{
    /// <summary>
    /// Abstract base class for implementing scroll views. (スクロールビューを実装するための抽象基底クラス。)
    /// Supports infinite scrolling and snapping. (無限スクロールおよびスナップに対応しています。)
    /// If <see cref="FancyScrollView{TItemData, TContext}.Context"/> is not needed, use <see cref="FancyScrollView{TItemData}"/> instead. (<see cref="FancyScrollView{TItemData, TContext}.Context"/> が不要な場合は代わりに <see cref="FancyScrollView{TItemData}"/> を使用します。)
    /// </summary>
    /// <typeparam name="TItemData">Item data type. (アイテムのデータ型。)</typeparam>
    /// <typeparam name="TContext">Type of <see cref="Context"/>. (<see cref="Context"/> の型。)</typeparam>
    public abstract class FancyScrollView<TItemData, TContext> : MonoBehaviour where TContext : class, new()
    {
        /// <summary>
        /// Interval between cells. (セル同士の間隔。)
        /// </summary>
        [SerializeField, Range(1e-2f, 1f)] protected float cellInterval = 0.2f;

        /// <summary>
        /// Scroll position reference. (スクロール位置の基準。)
        /// </summary>
        /// <remarks>
        /// For example, if <c>0.5</c> is specified and the scroll position is <c>0</c>, the first cell is placed in the center. (たとえば、 <c>0.5</c> を指定してスクロール位置が <c>0</c> の場合, 中央に最初のセルが配置されます。)
        /// </remarks>
        [SerializeField, Range(0f, 1f)] protected float scrollOffset = 0.5f;

        /// <summary>
        /// Whether to arrange cells in a circular manner. (セルを循環して配置させるかどうか。)
        /// </summary>
        /// <remarks>
        /// When set to <c>true</c>, the first cell follows the last cell, and the last cell precedes the first cell. (<c>true</c> にすると最後のセルの後に最初のセル, 最初のセルの前に最後のセルが並ぶようになります。)
        /// Set to <c>true</c> to implement infinite scrolling. (無限スクロールを実装する場合は <c>true</c> を指定します。)
        /// </remarks>
        [SerializeField] protected bool loop = false;

        /// <summary>
        /// The <c>Transform</c> that is the parent element of the cells. (セルの親要素となる <c>Transform</c>。)
        /// </summary>
        [SerializeField] protected Transform cellContainer = default;

        readonly IList<FancyCell<TItemData, TContext>> pool = new List<FancyCell<TItemData, TContext>>();

        /// <summary>
        /// Whether initialized. (初期化済みかどうか。)
        /// </summary>
        protected bool initialized;

        /// <summary>
        /// Current scroll position. (現在のスクロール位置。)
        /// </summary>
        protected float currentPosition;

        /// <summary>
        /// Cell Prefab. (セルの Prefab。)
        /// </summary>
        protected abstract GameObject CellPrefab { get; }

        /// <summary>
        /// Item list data. (アイテム一覧のデータ。)
        /// </summary>
        protected IList<TItemData> ItemsSource { get; set; } = new List<TItemData>();

        /// <summary>
        /// Instance of <typeparamref name="TContext"/>. (<typeparamref name="TContext"/> のインスタンス。)
        /// The same instance is shared between cells and scroll views. Used for passing information and maintaining state. (セルとスクロールビュー間で同じインスタンスが共有されます。 情報の受け渡しや状態の保持に使用します。)
        /// </summary>
        protected TContext Context { get; } = new TContext();

        /// <summary>
        /// Performs initialization. (初期化を行います。)
        /// </summary>
        /// <remarks>
        /// Called just before the first cell is created. (最初にセルが生成される直前に呼び出されます。)
        /// </remarks>
        protected virtual void Initialize() { }

        /// <summary>
        /// Updates the display content based on the passed item list. (渡されたアイテム一覧に基づいて表示内容を更新します。)
        /// </summary>
        /// <param name="itemsSource">Item list. (アイテム一覧。)</param>
        protected virtual void UpdateContents(IList<TItemData> itemsSource)
        {
            ItemsSource = itemsSource;
            Refresh();
        }

        /// <summary>
        /// Forces an update of the cell layout. (セルのレイアウトを強制的に更新します。)
        /// </summary>
        protected virtual void Relayout() => UpdatePosition(currentPosition, false);

        /// <summary>
        /// Forces an update of the cell layout and display content. (セルのレイアウトと表示内容を強制的に更新します。)
        /// </summary>
        protected virtual void Refresh() => UpdatePosition(currentPosition, true);

        /// <summary>
        /// Updates the scroll position. (スクロール位置を更新します。)
        /// </summary>
        /// <param name="position">Scroll position. (スクロール位置。)</param>
        protected virtual void UpdatePosition(float position) => UpdatePosition(position, false);

        void UpdatePosition(float position, bool forceRefresh)
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            currentPosition = position;

            var p = position - scrollOffset / cellInterval;
            var firstIndex = Mathf.CeilToInt(p);
            var firstPosition = (Mathf.Ceil(p) - p) * cellInterval;

            if (firstPosition + pool.Count * cellInterval < 1f)
            {
                ResizePool(firstPosition);
            }

            UpdateCells(firstPosition, firstIndex, forceRefresh);
        }

        void ResizePool(float firstPosition)
        {
            Debug.Assert(CellPrefab != null);
            Debug.Assert(cellContainer != null);

            var addCount = Mathf.CeilToInt((1f - firstPosition) / cellInterval) - pool.Count;
            for (var i = 0; i < addCount; i++)
            {
                var cell = Instantiate(CellPrefab, cellContainer).GetComponent<FancyCell<TItemData, TContext>>();
                if (cell == null)
                {
                    throw new MissingComponentException(string.Format(
                        "FancyCell<{0}, {1}> component not found in {2}.",
                        typeof(TItemData).FullName, typeof(TContext).FullName, CellPrefab.name));
                }

                cell.SetContext(Context);
                cell.Initialize();
                cell.SetVisible(false);
                pool.Add(cell);
            }
        }

        void UpdateCells(float firstPosition, int firstIndex, bool forceRefresh)
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var index = firstIndex + i;
                var position = firstPosition + i * cellInterval;
                var cell = pool[CircularIndex(index, pool.Count)];

                if (loop)
                {
                    index = CircularIndex(index, ItemsSource.Count);
                }

                if (index < 0 || index >= ItemsSource.Count || position > 1f)
                {
                    cell.SetVisible(false);
                    continue;
                }

                if (forceRefresh || cell.Index != index || !cell.IsVisible)
                {
                    cell.Index = index;
                    cell.SetVisible(true);
                    cell.UpdateContent(ItemsSource[index]);
                }

                cell.UpdatePosition(position);
            }
        }

        int CircularIndex(int i, int size) => size < 1 ? 0 : i < 0 ? size - 1 + (i + 1) % size : i % size;

#if UNITY_EDITOR
        bool cachedLoop;
        float cachedCellInterval, cachedScrollOffset;

        void LateUpdate()
        {
            if (cachedLoop != loop ||
                cachedCellInterval != cellInterval ||
                cachedScrollOffset != scrollOffset)
            {
                cachedLoop = loop;
                cachedCellInterval = cellInterval;
                cachedScrollOffset = scrollOffset;

                UpdatePosition(currentPosition);
            }
        }
#endif
    }

    /// <summary>
    /// Context class for <see cref="FancyScrollView{TItemData}"/>. (<see cref="FancyScrollView{TItemData}"/> のコンテキストクラス。)
    /// </summary>
    public sealed class NullContext { }

    /// <summary>
    /// Abstract base class for implementing scroll views. (スクロールビューを実装するための抽象基底クラス。)
    /// Supports infinite scrolling and snapping. (無限スクロールおよびスナップに対応しています。)
    /// </summary>
    /// <typeparam name="TItemData"></typeparam>
    /// <seealso cref="FancyScrollView{TItemData, TContext}"/>
    public abstract class FancyScrollView<TItemData> : FancyScrollView<TItemData, NullContext> { }
}
