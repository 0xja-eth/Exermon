using UnityEngine;
using Core.UI;
using Core.UI.Utils;
using UI.ExerPro.EnglishPro.PlotScene.Controls;
using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;
using Core.Data.Loaders;
using System.IO;
using Core.Systems;
using GameModule.Services;
using UI.ExerPro.EnglishPro.Common.Controls;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UI.ExerPro.EnglishPro.Common.Windows {
    /// <summary>
    /// 剧情窗口
    /// </summary>
    public class RewardWindow : BaseWindow {
        const string addMaskFormat = "+{0}";
        const string scoreFormat = "{0}";

        /// <summary>
        /// 外部变量
        /// </summary>
        public CardDisplay cardDisplay;
        public Text integral;
        public Text gold;
        public Button confirm;

        /// <summary>
        /// 外部系统
        /// </summary>
        CalcService calServ;
        EnglishService engServ;
        ExerProRecord record;

        /// <summary>
        /// 内部变量
        /// </summary>
        bool isConfigured { get; set; } = false;
        ExerProMapNode node { get; set; } = null;
        EnglishService.RewardInfo rewardInfo { get; set; } = null;
        UnityAction<bool> terminateCallback = null;
        int selectIndex { get; set; } = -1;

        #region 初始化
        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            calServ = CalcService.get();
            engServ = EnglishService.get();
            record = engServ.record;
        }

        #endregion


        #region 窗口控制
        /// <summary>
        /// 奖励系统请调用该接口
        /// </summary>
        /// <param name="rewardInfo">通过EnglishService的rewardInfo获取</param>
        public void startWindow(EnglishService.RewardInfo rewardInfo) {
            base.startWindow();
            node = engServ.record.currentNode();
            terminateCallback = engServ.exitNode;
            this.rewardInfo = rewardInfo;

            configureMarks();
            configureCardDisplay();
            confirm.gameObject.SetActive(false);

            if((ExerProMapNode.Type)node.typeId == ExerProMapNode.Type.Boss) {
                configureBoss();
            }
        }
        
        /// <summary>
        /// 用于剧情奖励，直接设定奖励情况
        /// </summary>
        /// <param name="gold"></param>
        public void startWindow(int gold) {
            base.startWindow();

        }
        /// <summary>
        /// 配置分数
        /// </summary>
        void configureMarks() {
            configureGold();
            configureScore();
        }
        
        /// <summary>
        /// 配置金币奖励
        /// </summary>
        void configureGold() {
            int gold = 0;
            switch ((ExerProMapNode.Type)node.typeId) {
                case ExerProMapNode.Type.Enemy:
                case ExerProMapNode.Type.Elite:
                    gold = CalcService.RewardGenerator.getGoldReward(record, enemy: rewardInfo.killEnemyNumber);
                    break;
                case ExerProMapNode.Type.Story:
                case ExerProMapNode.Type.Treasure:
                    gold = CalcService.RewardGenerator.getGoldReward(record, question: rewardInfo.correctQuestionNumber);
                    break;
                case ExerProMapNode.Type.Boss:
                    gold = CalcService.RewardGenerator.getBossGoldReward(record);
                    break;
            }
            this.gold.text = string.Format(addMaskFormat, gold);
        }

        /// <summary>
        /// 配置积分
        /// </summary>
        void configureScore() {
            var actor = engServ.record.actor;
            int score = CalcService.RewardGenerator.generateScore(record, 
                rewardInfo.killBossNumber, rewardInfo.isPerfect);
            integral.text = string.Format(scoreFormat, score);
        }

        /// <summary>
        /// 配置卡牌
        /// </summary>
        void configureCardDisplay() {
            var cards = CalcService.RewardGenerator.getCardRewards((ExerProMapNode.Type)node.typeId);
            cardDisplay.startView();
            cardDisplay.setItems(cards);
            cardDisplay.addClickedCallback(onCardSelected);
        }

        /// <summary>
        /// 配置Boss点的情况
        /// </summary>
        void configureBoss() {
            var actor = engServ.record.actor;
            actor.changeHP(actor.mhp());
        }

        /// <summary>
        /// 结束窗口
        /// 结束前用户必须选择了一个卡牌
        /// </summary>
        public override void terminateWindow() {
            if (selectIndex == -1)
                return;
            engServ.record.actor.cardGroup.addCard(cardDisplay.getItem(selectIndex));
            base.terminateWindow();
            terminateCallback?.Invoke(true);
        }

        #endregion

        void onCardSelected(int index) {
            selectIndex = index;
            confirm.gameObject.SetActive(true);
        }

    }
}
