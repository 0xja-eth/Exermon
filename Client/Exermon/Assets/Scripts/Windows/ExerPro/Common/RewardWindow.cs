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

        /// <summary>
        /// 内部变量
        /// </summary>
        bool isConfigured { get; set; } = false;
        ExerProMapNode node { get; set; } = null;
        int enenyNumber { get; set; } = 0;
        int questionNumber { get; set; } = 0;
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
        }

        #endregion


        #region 窗口控制
        /// <summary>
        /// 开始窗口
        /// </summary>
        public override void startWindow() {
            if (!isConfigured)
                return;
            configureMarks();
            configureCardDisplay();
            confirm.gameObject.SetActive(false);
            base.startWindow();
        }
        
        /// <summary>
        /// 配置分数
        /// </summary>
        void configureMarks() {
            int gold = 0;
            switch ((ExerProMapNode.Type)node.typeId) {
                case ExerProMapNode.Type.Enemy:
                    gold = CalcService.RewardGenerator.getGoldReward((ExerProMapNode.Type)node.typeId, 
                        node.xOrder, enemy: this.enenyNumber);
                    break;
                case ExerProMapNode.Type.Elite:
                    gold = CalcService.RewardGenerator.getGoldReward((ExerProMapNode.Type)node.typeId,
                        enemy: this.enenyNumber);
                    break;
                case ExerProMapNode.Type.Story:
                case ExerProMapNode.Type.Treasure:
                    gold = CalcService.RewardGenerator.getGoldReward((ExerProMapNode.Type)node.typeId,
                        node.xOrder, question:this.questionNumber);
                    break;
            }
            this.gold.text = string.Format(addMaskFormat, gold);

            this.integral.text = string.Format(addMaskFormat, 100);
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
        /// 启用该窗口必须先进行参数配置
        /// </summary>
        /// <param name="node"></param>
        /// <param name="enenyNumber"></param>
        /// <param name="questionNumber"></param>
        /// <param name="terminateCallback"></param>
        public void configure(ExerProMapNode node, int enenyNumber = 0,
            int questionNumber = 0, UnityAction<bool> terminateCallback = null) {
            this.node = node;
            this.enenyNumber = enenyNumber;
            this.questionNumber = questionNumber;
            this.terminateCallback = terminateCallback;
            this.isConfigured = true;
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
