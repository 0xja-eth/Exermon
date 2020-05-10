
/*
using System;
using UnityEngine;

using Core.Data.Loaders;

using Core.UI;
using Core.UI.Utils;

using BattleModule.Data;

/// <summary>
/// 对战匹配场景控件
/// </summary>
namespace UI.BattleScene.Controls.Animation {

    /// <summary>
    /// 玩家准备显示
    /// </summary>
    public class BattlerPrepareStatus : BattlerStatus {

        /// <summary>
        /// 半身像高度
        /// </summary>
        const int BustHeight = 616;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GameObject waitingText;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        BaseWindow selfWindow;

        bool _waiting = true; // 是否等待中
        public bool waiting {
            get { return _waiting; }
            set {
                _waiting = value;
                requestRefresh();
            }
        }

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            selfWindow = SceneUtils.get<BaseWindow>(gameObject);
        }

        #endregion

        //#region 更新控制
        
        ///// <summary>
        ///// 更新
        ///// </summary>
        //protected override void update() {
        //    base.update();
        //    if (!selfWindow.shown && shown)
        //        base.terminateView(); 
        //}
        
        //#endregion

        #region 启动/结束控制

        /// <summary>
        /// 启动视窗
        /// </summary>
        public override void startView() {
            base.startView();
            if (selfWindow) selfWindow.startWindow();
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void terminateView() {
            shown = false;
            if (waitingText) waitingText.SetActive(false);
            if (selfWindow) selfWindow.terminateWindow();
            else base.terminateView();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="battler">对战者</param>
        protected override void drawExactlyItem(RuntimeBattlePlayer battler) {
            base.drawExactlyItem(battler);

            if (waitingText) waitingText.SetActive(waiting);
        }

        /// <summary>
        /// 绘制半身像
        /// </summary>
        /// <param name="battler">对战者</param>
        protected override void drawFace(RuntimeBattlePlayer battler) {

            var bust = AssetLoader.getCharacterBustSprite(
                battler.characterId, BustHeight);
            face.gameObject.SetActive(true);
            face.overrideSprite = bust;
        }

        /// <summary>
        /// 清空物品
        /// </summary>
        public override void clearItem() {
            base.clearItem();
            if (waitingText) waitingText.SetActive(false);
        }

        #endregion
    }
}
*/