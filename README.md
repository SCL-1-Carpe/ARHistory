# ARHistory

## Networking　（レプリケーション≒同期）
### サーバーサイド　使い方　Assets/Networking/CoreScripts　配下のNetworkManager_Serverをアタッチする。
LaunchOnStartにチェックを入れるか、LaunchNetworkServer()を呼び出すことで起動する。

### クライアントサイド　使い方　Assets/Networking/CoreScripts　配下のNetworkManager_Clientをアタッチする。
TargetIPにサーバーのIPアドレスを入力してLaunchOnStartにチェックをいれるか、LaunchNetworkClient()を呼び出すことで起動する。

### オブジェクトの同期の仕方
#### サーバーが管理するオブジェクトを作成し、同期する
CreateNetworkPrefab()を呼び出す。PrefabNameにはAssets/Resources/Prefabs/配下のPrefabの名前を入れる。
同期する情報はReplicatorを使って指定する。
同期するPrefabには必ずReplicatorBaseクラスを継承したスクリプトをアタッチしなければならない。
サーバーは同期する情報を作成する際にReplicatorのGetReplicationData()を呼び出すので、
GetReplicationData()をオーバーライドして同期する情報のシリアライズ処理を実装してください。
クライアントは情報を受け取り、対応するReplicatorに対してReceiveReplicationData(byte[] repdata)を呼び出すので、
GetReplicationData()の逆の処理（デシリアライズ）を実装してください。

### クライアントが管理するオブジェクト（自治的オブジェクト,Autonomous）を作成し、同期する
クライアントからRequestCreatingNewAutonomousObject(ReplicatiorBase replicatior, string ReplicatedPrefabName, Vector3 pos, Vector3 eular, string ParentName)
を呼び出す。replicatorにはクライアントのローカルオブジェクトを、ReplicatedPrefabNameにはサーバーと他のクライアント上での同期されるPrefabの名前を渡す。
同期するプレハブ及びAutonomousObjectにはReplicatorBaseを継承したスクリプトがアタッチされていなければならない。
クライアントは同期する情報を作成する際にReplicatorのGetAutonomousData()を呼び出すので、
GetAutonomousData()をオーバーライドして同期する情報のシリアライズ処理を実装してください。
サーバーは情報を受け取り、対応するReplicatorに対してReceiveAutonomousData(byte[] repdata)を呼び出すので、
GetAutonomousData()の逆の処理（デシリアライズ）を実装してください。

### 通信の最適化（パケット削減）のために・・・
ReplicatorBaseにはDoesClientNeedReplication()とDoesServerNeedReplication()が用意されている。
サーバーはレプリケーションのパケット作成時にDoesClientNeedReplication()を呼び出し、返り値が偽であればレプリケートしない。
値に変更がないときなど、不要であればレプリケーションを行わない処理を実装してください。
