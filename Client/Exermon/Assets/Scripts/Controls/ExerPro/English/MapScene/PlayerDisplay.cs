
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI.Utils;

using ExermonModule.Data;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 地图场景控件
/// </summary>
namespace UI.ExerPro.EnglishPro.MapScene.Controls {

    /// <summary>
    /// 玩家显示组件
    /// </summary>
    public class PlayerDisplay : ItemDisplay<RuntimeActor> {

        /// <summary>
        /// 外部组件定义
        /// </summary>
        public Image full; // 艾瑟萌全身像
        public Text nickname; // 艾瑟萌昵称

        public Animation animation; // 动画

        /// <summary>
        /// 内部变量定义
        /// </summary>
        public bool isMoving { get; set; } = false;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        EnglishService engSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateAfterMove();
        }

        /// <summary>
        /// 更新移动后处理
        /// </summary>
        void updateAfterMove() {
            if (isMoving && !animation.isPlaying)
                onAfterMove();
        }

        /// <summary>
        /// 移动结束回调
        /// </summary>
        void onAfterMove() {
            isMoving = false;
            engSer.terminateMove();
        }

        #endregion

        #region 界面绘制

        #region 动画控制

        /// <summary>
        /// 移动到指定据点
        /// </summary>
        /// <param name="node"></param>
        public void gotoNode(ItemDisplay<ExerProMapNode> node, bool force = false) {
            gotoNode(node.transform as RectTransform, force);
        }
        public void gotoNode(RectTransform rt, bool force = false) {
            gotoNode(rt.anchoredPosition, force);
        }
        public void gotoNode(Vector2 pos, bool force = false) {
            var rt = transform as RectTransform;
            if (force) {
                rt.anchoredPosition = pos;
                startView(); onAfterMove();
            } else {
                isMoving = true;
                var curPos = rt.anchoredPosition;
                var ani = AnimationUtils.createAnimation();
                ani.addCurve(typeof(RectTransform), "m_AnchoredPosition.x", curPos.x, pos.x);
                ani.addCurve(typeof(RectTransform), "m_AnchoredPosition.y", curPos.y, pos.y);
                ani.setupAnimation(animation);
            }
        }

        #endregion

        /// <summary>
        /// 是否空物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool isNullItem(RuntimeActor item) {
            return base.isNullItem(item) || item.slotItem.isNullItem();
        }

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(RuntimeActor item) {
            base.drawExactlyItem(item);
            drawPlayerExer(item.slotItem.playerExer);
        }

        /// <summary>
        /// 绘制玩家艾瑟萌
        /// </summary>
        /// <param name="playerExer"></param>
        void drawPlayerExer(PlayerExermon playerExer) {
            nickname.text = playerExer.nickname;
            drawExermon(playerExer.exermon());
        }

        /// <summary>
        /// 绘制玩家信息
        /// </summary>
        void drawExermon(Exermon exermon) {
            full.gameObject.SetActive(true);
            full.overrideSprite = AssetLoader.generateSprite(exermon.full);
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            full.gameObject.SetActive(false);
            nickname.text = "";
        }

        #endregion

    }
}
