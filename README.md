# MorningRoutine
M5StickCから計測した3軸加速度をRGB値に変換して時系列データから100x100 pixelの画像を生成します。

# Requirement
・M5StickCを使用します。  
・追加でインストールが必要なライブラリはございません。

# Demo
https://github.com/SohuKou106/MorningRoutine/issues/1

# Usage
1. M5StickCにスケッチ（）を書き込んで起動します。
2. アプリをビルドして実行します。
3. 腕にM5StickCを装備します。
4. 「一日を始める」をクリックします。
5. おおよそ1秒間に10点の間隔で、合計10000点の3軸加速度データを計測します。
6. 計測完了後、アプリケーションファイルと同じディレクトリの「Result」フォルダ内に画像が生成されます。
