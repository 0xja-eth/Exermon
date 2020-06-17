
using Core.Systems;
using Core.UI;
using UnityEngine;
using UI.ExerPro.EnglishPro.ListenScene.Windows;
using ExerPro.EnglishModule.Services;
using PlayerModule.Services;
using System.Diagnostics;

/// <summary>
/// 背包场景
/// </summary>
namespace UI.ExerPro.EnglishPro.ListenScene {

    /// <summary>
    /// 商店场景
    /// </summary>
    public class ListenScene : BaseScene {
        /// <summary>
        /// 外部变量
        /// </summary>
        public ListenWindow listenWindow;

        /// <summary>
        /// 外部系统
        /// </summary>
        EnglishService engSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.EnglishProListenScene;
        }

        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            refresh();
            //测试用函数
			//GameSystem.get().start();
			//PlayerService.get().login("804173948", "123456789", configureQuestion);
		}

        //测试用
        void configureQuestion()
        {
            if (GameSystem.get().isLoaded() == true)
                UnityEngine.Debug.Log("danteding 登陆成功");
        }
        #endregion

        #region 场景控制

        /// <summary>
        /// 刷新场景
        /// </summary>
        public void refresh() {
            listenWindow.startWindow();
        }

        public override void popScene() {
            engSer.exitNode();
        }

        #endregion

    }
}