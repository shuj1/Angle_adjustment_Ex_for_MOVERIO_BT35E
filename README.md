# Angle adjustment Ex for MOVERIO BT35E

## 概要
これは2020年1月に作成した心理物理学実験のプログラムです。
2019年3月に作成した[Angle-adjustment-Ex](https://github.com/Fukky21/Angle-adjustment-Ex)とは異なり、
このプログラムではスマートグラス(EPSON MOVERIO BT-35E)を用います。
左右の眼球それぞれにラインを提示し、観察者はこの2本のラインを水平になるように角度を調節します。
2本のラインは左右を独立して動かすことも、同時に動かすこともできます。
実験中、MOVERIOからセンサーデータとカメラ映像データを取得し、記録することができます。

## 動作確認環境
- PC
  - VAIOシリーズ LAPTOP-SQLOQKSV
  - Intel(R) Core(TM) i7-8565U CPU @ 1.80GHz 1.99GHz
  - Windows 10 Pro ver.1809
- 統合開発環境
  - Microsoft Visual Studio Community 2019 Version 16.4.2
- スマートグラス
  - EPSON(R) Smart Glasses MOVERIO BT-35E
- コントローラー
  - Xbox 360(R) CONTROLLER

## 仕様
- 固定点の周りをラインが回転する
- ラインは固定点の左右に1つずつ
- 左のラインは左目に、右のラインは右目に提示する(固定点は両眼に提示する)
- サイドバイサイド方式で提示する
- 実験プラグラム開始後に以下を選択できる
  - ラインの操作方法
    - 左右のラインを同時に操作する
    - 左右のラインを独立に操作する
  - センサーを使う
    - No
    - Yes(AccelerometerかGyrometerを使う)
  - カメラを使う
    - No
    - Yes(録画する)
- センサーデータはトライアル開始時点から全トライアル終了時点まで記録される
- カメラ映像データはトライアル開始時点から録画停止ボタンが押されるまで記録される

## 使用方法
### Step1 機器を準備する
以下が全て揃っているか確認してください。
- PC
  - OSがWindows10でなければ、このプログラムは動作しません。
  - USB Type-Cポートを標準搭載していることを確認してください。Type-Cポートがなければセンサー機能とカメラ機能が使えません。
  - Visual Studio 2019がインストールされているか確認してください。インストールされていなければ、[Visual Studio 2019 for Windows および Mac のダウンロード](https://visualstudio.microsoft.com/ja/downloads/)からダウンロードしてください。
  - (オプション)標準搭載しているUSB Type-Cポートが、DisplayPort Alternate Mode on USB Type-Cに対応しているか確認してください。対応している場合は、映像出力, センサーの使用, カメラの使用が、USB Type-C to Cケーブル1本で行えます。
- MOVERIO BT-35E
  - このプログラムを使用する前にファームウェアのアップデートを行ってください。方法は[ファームウェアリリースノート - ドキュメント - BT-35E/30E - 技術情報 - MOVERIO - エプソン](https://tech.moverio.epson.com/ja/bt-35e/release_note/)を参照してください。
- USB Type-C(オス) to C(オス)ケーブル
  - センサーデータとカメラ映像データのやりとりに必須です。
  - 変換アダプタを使用してはいけません。
- Xbox 360 CONTROLLER
  - 他のXboxコントローラーでもおそらく問題ありませんが、動作確認はXbox 360でしか行っておりませんのでご注意ください。
  - Xbox以外のコントローラーで正常に動作するかはわかりません。
- ※HDMIケーブル
  - PCに標準搭載しているUSB Type-Cポートが、DisplayPort Alternate Mode on USB Type-Cに対応していない場合は映像出力のために必要です。
  - ※DisplayPort Alternate Mode on USB Type-Cに対応している場合は不要です。

### Step2 プログラムをダウンロードする
画像の赤矢印のボタンから、Angle_adjustment_Ex_for_MOVERIO_BT35EのUWP(Universal Windows Platform)プログラムをダウンロードする。

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img001.png)

### Step3 Visual Studio 2019を起動する
ダウンロードしたZIPファイルを展開した後、Visual Studio 2019を起動し、`Angle_adjustment_Ex_for_MOVERIO_BT35E.sln`を選択する。

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img002.png)
![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img003.png)

### Step4 出力されるファイル名を変更する
`StoreData.cs`を開き、11行目の`trialDataFileName`(観察者の応答が記録される)と12行目の`sensorDataFileName`(センサーデータが記録される)を変更する。

`CameraViewPage.xaml.cs`を開き、251行目の`CreateFileAsync`内の文字列(録画データ名)を変更する。

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img004.png)
![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img005.png)

### Step5 各種パラメータを変更する
- トライアル数
  - `SubPage.xaml.cs`の46行目の`nTrials`を変更する(初期値は50)
- Lineの初期最大角度
  - `SubPage.xaml.cs`の50行目の`rangeAngle`を変更する(単位はdeg, 初期値は30)
- 固定点のサイズ
  - `Common.cs`の17行目の`fixEllipseSize`を変更する(単位はdeg, 初期値は0.5)
- 固定点とラインの間の距離
  - `Common.cs`の18行目の`Distance_fixEllipse_and_Line`を変更する(単位はdeg, 初期値は1.5)
- ラインの長さ
  - `Common.cs`の19行目の`lineSize`を変更する(単位はdeg, 初期値は4.0)
- ラインの太さ
  - `Common.cs`の20行目の`lineThickness`を変更する(単位はdeg, 初期値は0.1)

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img006.png)

### Step6 プログラムを実行する
ソリューションプラットフォームを"x86"に指定し、ローカルコンピューターで実行ボタンを押す。

※初回実行時は、カメラとマイクへのアクセスを求められます。全てOKしてください。

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img007.png)

### Step7 トライアル開始の準備をする
ラインの操作方法、センサーを使うかどうか、カメラを使うかどうかをそれぞれ選択し、選択が完了したら"各種ウィンドウを表示する"ボタンを押す。

※MOVERIOを3D表示にしてください。方法は[ユーザーズガイド](https://tech.moverio.epson.com/ja/bt-35e/pdf/bt35e_userguide.pdf)を参照してください。

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img008.png)

### Step8 センサーの種類を選択する
"各種ウィンドウを表示する"ボタンを押すと、いくつかのウィンドウが出現する。

刺激提示ウィンドウはスマートグラス側にフルスクリーンで出されます(「準備中...」と表示されています)。

センサーを使う場合、センサーウィンドウがPC側に出されます。センサーの種類は2種類(AccelerometerとGyrometer)あり、センサーウィンドウのタブを操作して種類を選択します。トライアル中にタブを切り替えると、その時点から記録されるセンサーデータの種類が変わるのでご注意ください。

カメラを使う場合、カメラウィンドウがPC側に出されます。MOVERIOのカメラではなく、PCのカメラが起動してしまう場合、MOVERIOのデバイスIDが異なる可能性があります。詳しくは後述の「MOVERIO BT-35E デバイスIDについて」を参照してください。

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img009.png)

### Step9 トライアルを開始する
"トライアルを開始する"ボタンを押すと、トライアルがスタートします。この時点からセンサーデータの取得とカメラによる録画がスタートします。

### Step10 トライアルを進行する
トライアルは以下2つを交互に行いながら進行する。
1. トライアルの進行状況が"●●/●●"の形式で表示される。 → 観察者はBボタンを押して次へ進む。
2. 固定点と、その左右にラインが表示される。 → 観察者はスティックでラインの角度を調節し、Rトリガーを引いて応答を確定する。

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img010.png)

### Step11 実験を終了する
スマートグラス側に「終了しました」と表示されれば全トライアル終了です。終了したら、以下の作業を行います。
1. カメラを使用している場合、カメラウィンドウにある"録画を停止する"ボタンを押します。その後、"カメラをシャットダウンする"ボタンを押してください。このシャットダウンは必ず行ってください。
2. "データを書き出す"ボタンを押して、センサーデータと観察者の応答データをテキストファイルに書き込み、保存します。
3. "終了するボタン"を押してプログラムを終了します。

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img011.png)
![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img012.png)

### Step12 データを確認する
センサーデータと観察者の応答データは"ドキュメント"ディレクトリに保存されます。

録画した動画データは"ビデオ"ディレクトリに保存されます。

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img013.png)
![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img014.png)

## MOVERIO BT-35E デバイスIDについて
プログラムに記述してあるMOVERIOのデバイスIDが異なる場合、MOVERIOのカメラが起動しません。ここでは、自分の所持するMOVERIOのデバイスIDを調べる方法と設定方法について説明します。
1. `CameraViewPage.xaml.cs`の148行目の`ShowMessageToUser(device.ID)`のコメントを外して、変更を保存(Ctrl + S)する。
2. プログラムを実行する。
3. カメラを使うを"Yes"にして、"各種ウィンドウを表示する"ボタンを押す。
4. Visual Studioの出力コンソールに表示されたデバイスIDを確認する。"VID"からはじまる文字列がデバイスIDです。

下の写真のように"VID"からはじまる文字列が2つある場合は、おそらく上がPCのカメラ、下がMOVERIOのカメラのデバイスIDを示しています。

5. `CameraViewPage.xaml.cs`の36行目の`MoverioCameraId`を書き変えて、変更を保存(Ctrl + S)する。

![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/usage_img015.png)

## Q&A
### カメラ, センサーが反応しません。
- PCにUSB Type-Cポートが標準搭載されていないと、カメラ・センサーは使用できません。
- MOVERIOのファームアップデートを行っていないと、反応しない場合があります。
- もう一度MOVERIOを接続し直してみてください。

### プログラムを終了できません。
- カメラをシャットダウンしていない場合は終了できません。
- データの書き出し中は終了できません。書き出しが終了するまでお待ちください。

### データの書き出しがうまくできない。
プログラムを実行する前に、出力ファイル名をきちんと変更していますか？変更していない場合は、前のファイルに上書きされます。

### プログラムの挙動がおかしい。
観察者が持つコントローラーの操作が、PC側のウィンドウを操作する場合があります。観察者がコントローラーを操作しているときは、PC側のウィンドウに注意してください。

### トライアルの途中だけど、中断したい。
通常の終了手順と同様に、カメラのシャットダウンは必須です。"データを書き出す"ボタンを押せば、現時点までのデータを書き出すことができます。

## 詳細
![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/detail_img001.png)
![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/detail_img002.png)
![](https://github.com/Fukky21/Angle_adjustment_Ex_for_MOVERIO_BT35E/blob/images/detail_img003.png)
