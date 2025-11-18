# FancyScrollView

[![license](https://img.shields.io/badge/license-MIT-green.svg?style=flat&cacheSeconds=2592000)](https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
[![WebGL Demo](https://img.shields.io/badge/demo-WebGL-orange.svg?style=flat&logo=google-chrome&logoColor=white&cacheSeconds=2592000)](https://setchi.jp/FancyScrollView/demo)
[![API Documentation](https://img.shields.io/badge/API-Documentation-ff69b4.svg?style=flat&logo=c-sharp&cacheSeconds=2592000)](https://setchi.jp/FancyScrollView/api/FancyScrollView.html)
[![openupm](https://img.shields.io/npm/v/jp.setchi.fancyscrollview?label=openupm&registry_uri=https://package.openupm.com&style=flat)](https://openupm.com/packages/jp.setchi.fancyscrollview/)

[English](https://translate.google.com/translate?sl=ja&tl=en&u=https://github.com/setchi/FancyScrollView) (by Google Translate)

A highly flexible ScrollView component for implementing animations. Also supports infinite scrolling. (高度に柔軟なアニメーションを実装できる汎用の ScrollView コンポーネントです。 無限スクロールもサポートしています。)

<img src="https://user-images.githubusercontent.com/8326814/69004520-d2b36b80-0957-11ea-8277-06bfd3e8f033.gif" width="320"><img src="https://user-images.githubusercontent.com/8326814/70638335-0b571400-1c7c-11ea-8701-a0d1ae0cb7e3.gif" width="320"><img src="https://user-images.githubusercontent.com/8326814/59548448-a3549900-8f8a-11e9-9a27-b04f1410a7b5.gif" width="320"><img src="https://user-images.githubusercontent.com/8326814/59548462-b8c9c300-8f8a-11e9-8985-5f1c2e610309.gif" width="320"><img src="https://user-images.githubusercontent.com/8326814/59550410-7f528100-8fa5-11e9-8f1b-41e59b645571.gif" width="320"><img src="https://user-images.githubusercontent.com/8326814/59550411-7f528100-8fa5-11e9-8bfb-bd42da47f7a0.gif" width="320">

## Requirements
[![Unity 2019.4+](https://img.shields.io/badge/unity-2019.4+-black.svg?style=flat&logo=unity&cacheSeconds=2592000)](https://unity3d.com/get-unity/download/archive)
[![.NET 4.x Scripting Runtime](https://img.shields.io/badge/.NET-4.x-blueviolet.svg?style=flat&cacheSeconds=2592000)](https://docs.unity3d.com/2018.3/Documentation/Manual/ScriptingRuntimeUpgrade.html)

## Installation
### Unity Asset Store
Consider purchasing from [Unity Asset Store](https://assetstore.unity.com/packages/tools/gui/fancyscrollview-96530) to support further development. (Unity Asset Store から購入して、さらなる開発のサポートを検討してください。)

### OpenUPM
Add the package from the [OpenUPM](https://openupm.com/) registry to your Unity Project. (OpenUPM レジストリからパッケージを Unity Project に追加します。)

```
openupm add jp.setchi.fancyscrollview
```

### Unity Package Manager
Add a reference to the repository in your project directory's [`Packages/manifest.json`](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html#project-manifests) file. (プロジェクトディレクトリの [`Packages/manifest.json`](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html#project-manifests) ファイルにリポジトリへの参照を追加します。)

```json
{
  "dependencies": {
    "jp.setchi.fancyscrollview": "https://github.com/setchi/FancyScrollView.git#upm"
  }
}
```

## Features
### Freely implement scroll animations (自由にスクロールアニメーションを実装できます)
FancyScrollView passes the normalized position within the viewport range to each cell when updating the scroll position. On the cell side, the cell [controls its own position and appearance during scrolling](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyCell-2.html#FancyScrollView_FancyCell_2_UpdatePosition_System_Single_) based on values from `0.0` to `1.0`. The examples implement scrolling behavior using Animator and mathematical formulas. (FancyScrollView はスクロール位置を更新するとき、ビューポート範囲の正規化された位置を各セルに渡します。セル側では `0.0` ~ `1.0` の値に基づいてスクロール中の位置や見た目を[セル自身で制御](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyCell-2.html#FancyScrollView_FancyCell_2_UpdatePosition_System_Single_)します。サンプルでは Animator や数式を使用してスクロール中の動きを実装しています。)

### Operates smoothly even with large amounts of data (データ件数が多くても軽快に動作します)
Only the necessary number of cells for display are created, and cells are reused. You can check the actual operation by increasing the data count in the [Demo](https://setchi.jp/FancyScrollView/demo/). In [FancyScrollRect](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyScrollRect-2.html) and [FancyGridView](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyGridView-2.html), you can also specify the [margin before cells are reused during scrolling](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyScrollRect-2.html#FancyScrollView_FancyScrollRect_2_reuseCellMarginCount). (表示に必要なセル数のみが生成され、セルは再利用されます。 [Demo](https://setchi.jp/FancyScrollView/demo/) で実際にデータ件数を増やしながら動作を確認できます。 [FancyScrollRect](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyScrollRect-2.html) および [FancyGridView](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyGridView-2.html) では、[スクロール中にセルが再利用されるまでの余白](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyScrollRect-2.html#FancyScrollView_FancyScrollRect_2_reuseCellMarginCount)も指定できます。)

### Freely exchange messages between cells and scroll views (セルとスクロールビュー間で自由にメッセージのやりとりができます)
Through `Context`, you can simply implement processing where the scroll view detects when a cell is clicked, or where the scroll view gives instructions to cells. An implementation example ([Examples/02_FocusOn](https://github.com/setchi/FancyScrollView/tree/master/Assets/FancyScrollView/Examples/Sources/02_FocusOn)) is included, so please use it as a reference. (`Context` 経由で、セルがクリックされたことをスクロールビューで検知したり、スクロールビューからセルに指示を出す処理がシンプルに実装できます。実装例（[Examples/02_FocusOn](https://github.com/setchi/FancyScrollView/tree/master/Assets/FancyScrollView/Examples/Sources/02_FocusOn)）が含まれていますので、参考にしてください。)

### Scroll or jump to specific cells (特定のセルにスクロールやジャンプができます)
You can also specify the number of seconds to move and easing. For details, refer to [Class Scroller](https://setchi.jp/FancyScrollView/api/FancyScrollView.Scroller.html#FancyScrollView_Scroller_ScrollTo_System_Single_System_Single_EasingCore_Ease_System_Action_) in the [API Documentation](https://setchi.jp/FancyScrollView/api/FancyScrollView.html). (移動にかける秒数や Easing の指定もできます。詳しくは [API Documentation](https://setchi.jp/FancyScrollView/api/FancyScrollView.html) の [Class Scroller](https://setchi.jp/FancyScrollView/api/FancyScrollView.Scroller.html#FancyScrollView_Scroller_ScrollTo_System_Single_System_Single_EasingCore_Ease_System_Action_) を参照してください。)

### Fine-tune scroll behavior (スクロールの挙動を細かく設定できます)
You can configure scroll-related behavior such as the presence or absence of inertia and deceleration rate. For details, refer to [Class Scroller](https://setchi.jp/FancyScrollView/api/FancyScrollView.Scroller.html) in the [API Documentation](https://setchi.jp/FancyScrollView/api/FancyScrollView.html). (慣性の有無、減速率などスクロールに関する挙動の設定ができます。詳しくは [API Documentation](https://setchi.jp/FancyScrollView/api/FancyScrollView.html) の [Class Scroller](https://setchi.jp/FancyScrollView/api/FancyScrollView.Scroller.html) を参照してください。)

### Supports snapping (スナップをサポートしています)
When snapping is enabled, it moves to the nearest cell just before scrolling stops. You can specify the velocity threshold at which snapping begins, the number of seconds to move, and easing. [FancyScrollRect](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyScrollRect-2.html) and [FancyGridView](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyGridView-2.html) do not support snapping. (スナップを有効にすると、スクロールが止まる直前に最寄りのセルへ移動します。スナップがはじまる速度のしきい値、移動にかける秒数、 Easing を指定できます。[FancyScrollRect](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyScrollRect-2.html) および [FancyGridView](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyGridView-2.html) はスナップをサポートしていません。)

### Supports infinite scrolling (無限スクロールをサポートしています)
You can implement infinite scrolling by making the following settings in the Inspector: (Inspector で下記の設定をすることで無限スクロールを実装できます。)
1. When you turn on `Loop` in `FancyScrollView`, cells circulate, with the last cell before the first cell and the first cell after the last cell. (1. `FancyScrollView` の `Loop` をオンにするとセルが循環し、先頭のセルの前に末尾のセル、末尾のセルの後に先頭のセルが並ぶようになります。)
1. When using the `Scroller` used in the samples, setting `Movement Type` to `Unrestricted` makes the scroll range unlimited. Combined with 1., you can achieve infinite scrolling. (1. サンプルで使用されている `Scroller` を使うときは、 `Movement Type` を `Unrestricted` に設定することで、スクロール範囲が無制限になります。 1. と組み合わせることで無限スクロールを実現できます。)

An implementation example ([Examples/03_InfiniteScroll](https://github.com/setchi/FancyScrollView/tree/master/Assets/FancyScrollView/Examples)) is included, so please refer to it. [FancyScrollRect](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyScrollRect-2.html) and [FancyGridView](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyGridView-2.html) do not support infinite scrolling. (実装例（[Examples/03_InfiniteScroll](https://github.com/setchi/FancyScrollView/tree/master/Assets/FancyScrollView/Examples)）が含まれていますので、こちらも参考にしてください。[FancyScrollRect](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyScrollRect-2.html) および [FancyGridView](https://setchi.jp/FancyScrollView/api/FancyScrollView.FancyGridView-2.html) は無限スクロールをサポートしていません。)

## Examples
[![WebGL Demo](https://img.shields.io/badge/demo-WebGL-orange.svg?style=flat&logo=google-chrome&logoColor=white&cacheSeconds=2592000)](https://setchi.jp/FancyScrollView/demo)

Refer to [FancyScrollView/Examples](https://github.com/setchi/FancyScrollView/tree/master/Assets/FancyScrollView/Examples). ([FancyScrollView/Examples](https://github.com/setchi/FancyScrollView/tree/master/Assets/FancyScrollView/Examples) を参照してください。)

| Name | Description |
|:-----------|:------------|
|01_Basic|The simplest configuration implementation example. (最もシンプルな構成の実装例です。)|
|02_FocusOn|Implementation example focusing on left and right cells with buttons. (ボタンで左右のセルにフォーカスする実装例です。)|
|03_InfiniteScroll|Infinite scrolling implementation example. (無限スクロールの実装例です。)|
|04_Metaball|Metaball implementation example using shaders. (シェーダーを使用したメタボールの実装例です。)|
|05_Voronoi|Voronoi implementation example using shaders. (シェーダーを使用したボロノイの実装例です。)|
|06_LoopTabBar|Implementation example switching screens with tabs. (タブで画面を切り替える実装例です。)|
|07_ScrollRect|`ScrollRect` style implementation example with scrollbar. (スクロールバー付きの `ScrollRect` スタイルの実装例です。)|
|08_GridView|Grid layout implementation example. (グリッドレイアウトの実装例です。)|
|09_LoadTexture|Implementation example loading and displaying textures. (テクスチャをロードして表示する実装例です。)|

## Usage
In the simplest configuration, the following implementations are required: (もっともシンプルな構成では、以下の実装が必要です。)

- Object for passing data to cells (セルにデータを渡すためのオブジェクト)
- Cell (セル)
- Scroll view (スクロールビュー)

### Implementation
Define an object for passing data to cells. (セルにデータを渡すためのオブジェクトを定義します。)
```csharp
class ItemData
{
    public string Message { get; }

    public ItemData(string message)
    {
        Message = message;
    }
}
```
`FancyCell<TItemData>` を継承して自分のセルを実装します。
```csharp
using UnityEngine;
using UnityEngine.UI;
using FancyScrollView;

class MyCell : FancyCell<ItemData>
{
    [SerializeField] Text message = default;

    public override void UpdateContent(ItemData itemData)
    {
        message.text = itemData.Message;
    }

    public override void UpdatePosition(float position)
    {
        // position is a value from 0.0 to 1.0 (position は 0.0 ~ 1.0 の値です)
        // You can freely control the appearance of scrolling based on position (position に基づいてスクロールの外観を自由に制御できます)
    }
}
```
Inherit from `FancyScrollView<TItemData>` to implement your own scroll view. (`FancyScrollView<TItemData>` を継承して自分のスクロールビューを実装します。)
```csharp
using UnityEngine;
using System.Linq;
using FancyScrollView;

class MyScrollView : FancyScrollView<ItemData>
{
    [SerializeField] Scroller scroller = default;
    [SerializeField] GameObject cellPrefab = default;

    protected override GameObject CellPrefab => cellPrefab;

    void Start()
    {
        scroller.OnValueChanged(base.UpdatePosition);
    }

    public void UpdateData(IList<ItemData> items)
    {
        base.UpdateContents(items);
        scroller.SetTotalCount(items.Count);
    }
}
```
Feed data into the scroll view. (スクロールビューにデータを流し込みます。)
```csharp
using UnityEngine;
using System.Linq;

class EntryPoint : MonoBehaviour
{
    [SerializeField] MyScrollView myScrollView = default;

    void Start()
    {
        var items = Enumerable.Range(0, 20)
            .Select(i => new ItemData($"Cell {i}"))
            .ToArray();

        myScrollView.UpdateData(items);
    }
}
```

For other details, refer to [Examples](https://github.com/setchi/FancyScrollView/tree/master/Assets/FancyScrollView/Examples) and [API Documentation](https://setchi.jp/FancyScrollView/api/FancyScrollView.html). (その他の詳細は [Examples](https://github.com/setchi/FancyScrollView/tree/master/Assets/FancyScrollView/Examples) および [API Documentation](https://setchi.jp/FancyScrollView/api/FancyScrollView.html) を参照してください。)

## Author
[setchi](https://github.com/setchi)

## License
[MIT](https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
