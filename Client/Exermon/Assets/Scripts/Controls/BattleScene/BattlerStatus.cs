using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using PlayerModule.Data;
using BattleModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 对战匹配场景控件
/// </summary>
namespace UI.BattleScene.Controls {

    /// <summary>
    /// 对战玩家显示
    /// </summary>
    public class BattlerStatus : ItemDisplay<RuntimeBattlePlayer>{

        /// <summary>
        /// 常量定义
        /// </summary>
        public const string HPFormat = "HP {0}/{1}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image face, status;

        public ParamDisplay baseStatus;

        public Texture2D correctStatus, wrongStatus;

        /// <summary>
        /// 内部变量
        /// </summary>
        protected bool answered = false, correct = false;

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateResult();
        }

        /// <summary>
        /// 更新结果
        /// </summary>
        void updateResult() {
            if (answered != isAnswered() || correct != isCorrect()) 
                requestRefresh();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置人物
        /// </summary>
        /// <param name="player">人物对象</param>
        public void setItem(Player player) {
            setItem(new RuntimeBattlePlayer(player));
        }

        /// <summary>
        /// 是否正确
        /// </summary>
        /// <returns>返回是否正确</returns>
        public bool isAnswered(RuntimeBattlePlayer battler = null) {
            if (battler == null) battler = item;
            if (battler == null) return false;
            return battler.isAnswered();
        }

        /// <summary>
        /// 是否正确
        /// </summary>
        /// <returns>返回是否正确</returns>
        public bool isCorrect(RuntimeBattlePlayer battler = null) {
            if (battler == null) battler = item;
            if (battler == null) return false;
            return battler.correct;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="battler">对战者</param>
        protected override void drawExactlyItem(RuntimeBattlePlayer battler) {
            base.drawExactlyItem(battler);

            answered = isAnswered(battler);
            correct = isCorrect(battler);

            drawFace(battler);
            drawStatus(battler);
            drawBaseInfo(battler);
        }

        /// <summary>
        /// 绘制头像
        /// </summary>
        /// <param name="battler">对战者</param>
        protected virtual void drawFace(RuntimeBattlePlayer battler) {
            face.gameObject.SetActive(true);
            face.overrideSprite = battler.character().face;
        }

        /// <summary>
        /// 绘制状态
        /// </summary>
        /// <param name="battler"></param>
        protected virtual void drawStatus(RuntimeBattlePlayer battler) {
            if (!status) return;
            var texture = correct ? correctStatus : wrongStatus;

            Debug.Log("status.enabled : " + status.enabled + 
                ": " + battler.toJson().ToJson());

            status.enabled = answered;
            status.overrideSprite = 
                AssetLoader.generateSprite(texture);
        }

        /// <summary>
        /// 绘制基础信息
        /// </summary>
        /// <param name="battler">对战者</param>
        void drawBaseInfo(RuntimeBattlePlayer battler) {
            baseStatus.setValue(battler, "base_status");
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            if (status) status.enabled = false;
            face.gameObject.SetActive(false);
            baseStatus.clearValue();
        }

        #endregion
    }
}
