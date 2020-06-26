using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.ExerPro.EnglishPro.Common.Controls;

/// <summary>
/// 特训通用窗口
/// </summary>
namespace UI.ExerPro.EnglishPro.Common.Windows {

    /// <summary>
    /// 剧情窗口
    /// </summary>
    public class RewardWindow : BaseWindow {

		/// <summary>
		/// 常量定义
		/// </summary>
        const string addMaskFormat = "+{0}";
        const string scoreFormat = "{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public CardRewardDisplay cardDisplay;
        public Text integral;
        public Text gold;
        public Button confirm;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        CalcService calServ;
        EnglishService engServ;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		BaseScene scene;

		ExerProRecord record;
		ExerProMapNode node;

		EnglishService.RewardInfo rewardInfo;

		UnityAction<bool> terminateCallback;

		int selectedIndex = -1;

		#region 初始化

		/// <summary>
		/// 初始化场景
		/// </summary>
		protected override void initializeScene() {
			base.initializeScene();
			scene = SceneUtils.getCurrentScene();
		}

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
            base.initializeSystems();
            calServ = CalcService.get();
            engServ = EnglishService.get();

            record = engServ.record;
			terminateCallback = engServ.exitNode;
		}

        #endregion
		
        #region 窗口控制

        /// <summary>
        /// 奖励系统请调用该接口
        /// </summary>
        /// <param name="rewardInfo">通过EnglishService的rewardInfo获取</param>
        public void startWindow(EnglishService.RewardInfo rewardInfo) {
            base.startWindow(); setupInfo(rewardInfo);
        }
        
        /// <summary>
        /// 用于剧情奖励，直接设定奖励情况
        /// </summary>
        /// <param name="gold"></param>
        public void startWindow(int gold) {
            base.startWindow();
			// TODO: 剧情效果
		}

		/// <summary>
		/// 结束窗口
		/// 结束前用户必须选择了一个卡牌
		/// </summary>
		public override void terminateWindow() {
			base.terminateWindow();

			if (selectedIndex == -1) return;
			record.actor.cardGroup.addCard(
				cardDisplay.getItem(selectedIndex));
		}

		/// <summary>
		/// 窗口关闭回调
		/// </summary>
		protected override void onWindowHidden() {
			base.onWindowHidden();

			if (terminateCallback != null)
				terminateCallback.Invoke(true);
			else scene?.popScene();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 配置奖励信息
		/// </summary>
		/// <param name="rewardInfo"></param>
		void setupInfo(EnglishService.RewardInfo rewardInfo) {
			this.rewardInfo = rewardInfo;
			//confirm.gameObject.SetActive(false);
			setupNodeReward();
		}

		/// <summary>
		/// 配置据点奖励
		/// </summary>
		void setupNodeReward() {
			node = record.currentNode();
			setupMarks(); setupCardsDisplay();
			if (node.isBoss()) setupBoss();
		}

		/// <summary>
		/// 配置分数
		/// </summary>
		void setupMarks() {
			setupGold(); setupScore();
		}

		/// <summary>
		/// 配置金币奖励
		/// </summary>
		void setupGold() {
			int gold = 0;
			Debug.Log("setupGold: " + node.typeEnum());

			switch (node.typeEnum()) {
				case ExerProMapNode.Type.Enemy:
				case ExerProMapNode.Type.Elite:
					gold = CalcService.RewardGenerator.getGoldReward(
						node, record, enemy: rewardInfo.killEnemyNumber);
					break;

				case ExerProMapNode.Type.Story:
				case ExerProMapNode.Type.Treasure:
					gold = CalcService.RewardGenerator.getGoldReward(
						node, record, question: rewardInfo.correctQuestionNumber);
					break;

				case ExerProMapNode.Type.Boss:
					gold = CalcService.RewardGenerator.getBossGoldReward(record);
					break;
			}

			record.gainGold(gold);

			this.gold.text = string.Format(addMaskFormat, gold);
		}

		/// <summary>
		/// 配置积分
		/// </summary>
		void setupScore() {
			int score = CalcService.RewardGenerator.generateScore(record,
				rewardInfo.killBossNumber, rewardInfo.isPerfect);

			integral.text = string.Format(scoreFormat, score);
		}

		/// <summary>
		/// 配置卡牌
		/// </summary>
		void setupCardsDisplay() {
			Debug.Log("setupCardsDisplay: " + node.typeEnum());

			var cards = CalcService.RewardGenerator.
				getCardRewards(node.typeEnum());

			cardDisplay.startView();
			cardDisplay.setItems(cards);
			cardDisplay.addClickedCallback(onCardSelected);
		}

		/// <summary>
		/// 配置Boss点的情况
		/// </summary>
		void setupBoss() {
			record.actor.recoverAll();
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 卡牌选中回调
		/// </summary>
		/// <param name="index"></param>
		void onCardSelected(int index) {
            selectedIndex = index;
            confirm.gameObject.SetActive(true);
        }

		#endregion

	}
}
