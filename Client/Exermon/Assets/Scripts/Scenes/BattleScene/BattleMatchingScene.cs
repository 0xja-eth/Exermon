
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using Core.Systems;

using PlayerModule.Services;
using SeasonModule.Services;
using BattleModule.Services;

using UI.BattleStartScene.Controls;
using UI.BattleMatchingScene.Controls;

/// <summary>
/// 对战匹配场景
/// </summary>
namespace UI.BattleMatchingScene {

    /// <summary>
    /// 对战匹配场景
    /// </summary>
    public class BattleMatchingScene : BaseScene {

        /// <summary>
        /// 文本定义
        /// </summary>
        public const string ProgressFormat = "匹配成功！正在加载对战场景… {0}%";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public TopInfoDisplay topInfoDisplay;
        public BattlerDisplay selfBattler;
        public BattlerDisplay oppoBattler;
        public MatchingClock clock;

        public Text progress;

        /// <summary>
        /// 内部系统声明
        /// </summary>
        PlayerService playerSer;
        BattleService battleSer;
        SeasonService seasonSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override string sceneName() {
            return SceneSystem.Scene.BattleMatchingScene;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
            battleSer = BattleService.get();
            seasonSer = SeasonService.get();
        }

        /// <summary>
        /// 初始化其他
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
            SceneUtils.depositSceneObject("Scene", this);
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            clock.startTimer();
            topInfoDisplay.startView(playerSer.player);
            selfBattler.setItem(playerSer.player);
            progress.text = "";
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            if (battleSer.isStateChanged())
                onStateChanged();
        }

        #endregion

        #region 场景控制

        #endregion

        #region 流程控制

        /// <summary>
        /// 取消匹配
        /// </summary>
        public void cancelMatch() {
            battleSer.cancelMatch(onMatchCancelled);
        }

        /// <summary>
        /// 匹配取消回调
        /// </summary>
        void onMatchCancelled() {
            popScene();
        }

        #region 状态回调处理

        /// <summary>
        /// 状态改变回调
        /// </summary>
        void onStateChanged() {
            switch ((BattleService.State)battleSer.state) {
                case BattleService.State.Matched: onMatched(); break;
            }
        }

        /// <summary>
        /// 匹配完成
        /// </summary>
        void onMatched() {
            var battle = battleSer.battle;
            selfBattler.setItem(battle.self());
            oppoBattler.setItem(battle.oppo());
            prepareBattleScene();
        }

        /// <summary>
        /// 发送进度
        /// </summary>
        /// <param name="progress">设置进度</param>
        void setProgress(float progress) {
            int progress_ = (int)Mathf.Round(progress * 100);
            battleSer.matchProgress(progress_);
        }

        /// <summary>
        /// 匹配进度回调
        /// </summary>
        void onProgresss() {
            var battle = battleSer.battle;
            selfBattler.requestRefresh(true);
            oppoBattler.requestRefresh(true);
            progress.text = string.Format(
                ProgressFormat, battle.self().progress);

            if (battleSer.isMatchingCompleted())
                sceneSys.operReady = true;
        }

        /// <summary>
        /// 准备开始游戏
        /// </summary>
        void prepareBattleScene() {
            sceneSys.changeScene(SceneSystem.Scene.BattleScene);
            createCoroutine(sceneSys.startAsync(setProgress));
        }

        #endregion

        #endregion

    }
}