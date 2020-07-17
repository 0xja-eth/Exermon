using System;
using System.Collections.Generic;

using Core.Systems;

/// <summary>
/// 配置项
/// </summary>
public class Config {

	/// <summary>
	/// 服务器地址
	/// </summary>
	//public const string ServerURL = "ws://111.230.227.84:8001/game/";
	//public const string ServerURL = "ws://111.230.227.84:8002/game/";            
	public const string ServerURL = "ws://120.79.176.90:8001/game/";
	//public const string ServerURL = "ws://127.0.0.1:8000/game/";

	/// <summary>
	/// 前端主版本号
	/// </summary>
	public const string LocalMainVersion = "0.3.2";

	/// <summary>
	/// 前端副版本号
	/// </summary>
	public const string LocalSubVersion = "20200717";

	/// <summary>
	/// 初始场景
	/// </summary>
	public const SceneSystem.Scene FirstScene = SceneSystem.Scene.TitleScene;

}