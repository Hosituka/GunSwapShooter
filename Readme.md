# GunSwapShooter

## 概要
※geminicliがunitymcpを介して、プロジェクトを調査しまとめたのがこのReadme.mdです。誤情報があったらすまん。
このプロジェクトは、Unity 6 (6000.0.60f1) で制作された、ブラウザ（WebGL/unityroom）向けの3DoF ARシューティングゲームです。
AR Foundationなどの外部フレームワークに頼らず、WebCamTextureとデバイスの姿勢センサー（AttitudeSensor）を直接制御することで、デバイスの回転に同期したAR体験を実現しています。

### デモプレイ
[unityroom - GunSwapShooter](https://unityroom.com/games/gun_swap_shooter)

## ゲームシステムの特徴

- **3DoF ARアクション**: デバイスを物理的に回転させ、現実の風景の中に現れるターゲットを撃ち抜きます。
- **2丁の銃の使い分け**: プレイヤーは赤と青の2丁の銃を使い分け、ターゲットの色に合わせた射撃が求められます。
- **コンボ・スコアリング**: 連続ヒットによる倍率上昇（ScoreMultiplier）や、破壊時の判定（Great, Goodなど）により、戦略的なハイスコア狙いが可能です。
- **多様なターゲットギミック**:
    - **基本**: `RedTarget`, `BlueTarget`
    - **移動**: `MoveRedTarget`, `MoveBlueTarget`
    - **特殊**: 
        - `GunSwapTarget`: 銃を入れ替えることで反応するターゲット
        - `LineUpTarget`: 複数のターゲットが回転扉のように連なる
        - `ButtonMashingTarget`: 連打が必要なターゲット
        - `RedBlueTarget`: 左が赤、右が青と言った感じで色が異なり、それに合わせて同時撃ちするターゲットです。

## 画面・シーン構成

プロジェクトは以下のシーン遷移で構成されています。

1.  **AR_Setup (`Assets/Scenes/AR_Setup.unity`)**: 
    - AR体験の土台となる初期化シーン。
    - Webカメラの使用許可、デバイスの向き確認、背面カメラの特定、姿勢センサーの有効化を順次行います。
2.  **title (`Assets/Scenes/title.unity`)**: 
    - 難易度（Easy, Normal, Hard）の選択。
3.  **stage1 / stage2 (`Assets/Scenes/stage1.unity`, `stage2.unity`)**: 
    - メインのゲームプレイシーン。
    - `PointObjectGenerator2` による動的なターゲット配置。
## 技術的実装の詳細

### AR制御ロジック
- **`GameManager.cs`**: 
    - シングルトンとして存在し、`WebCamTexture` を通じた背面カメラ映像の取得と、`AttitudeSensor` のライフサイクル管理を行います。
- **`AttitudeTransformController.cs`**: 
    - ジャイロセンサーから得られる `attitude`（Quaternion）を、スマートフォンの持ち方（ScreenOrientation）に合わせて補正し、ゲーム内のメインカメラの回転に反映させます。

### ターゲット生成アルゴリズム (`PointObjectGenerator2.cs`)
- **グリッド管理**: プレイヤーの周囲を仮想的なグリッドで分割し、ターゲットの重複配置を回避。
- **Perlinノイズ配置**: ターゲットの出現位置を単純なランダムではなく、Perlinノイズを用いることで、ゲーム性に適した「散らばり」と「まとまり」を制御。
- **コストシステム**: 出現させるターゲットの種類ごとに「コスト」を設定し、一度に画面に現れる負荷を調整。

### オブジェクト管理と最適化
- **`ObjectPoolManager.cs`**: 
    - `IPoolable<T>` インターフェースを用いたジェネリックなオブジェクトプールを実装。
    - 大量の弾（Bullet）やUI演出（JudgeText, ExplosionEffect）の生成・破棄によるオーバーヘッドを最小化しています。

### UI演出
- **2D/3Dの融合**: 
    - Canvas上の `Indicator`（画面外の敵を指し示す矢印）と、ワールド空間に浮かぶ `JudgeText` や `ExplosionEffect` が連動。

## 制作・開発環境

- **Unity Version**: `6000.0.60f1`
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Input System**: `com.unity.inputsystem` (1.14.2)
- **API連携**: `com.unityroom.client` (ランキング・スコア送信)

## 更新履歴
- **2026-02-23**: 
    - 汎用オブジェクトプールシステム（`ObjectPoolManager.cs`）を導入し、メモリ効率を改善。
    - 全シーンおよび全スクリプトの徹底調査に基づき、README.mdを詳細版へ刷新。

## ライセンス・外部アセットについて
- **`UnityroomApiClient`**: unityroom公式パッケージ。
- **その他**: スクリプト、シェーダー、演出ロジックの多くは、このプロジェクト特有のAR体験のために独自に設計・実装されています。

---
最終更新日: 2026年2月23日
