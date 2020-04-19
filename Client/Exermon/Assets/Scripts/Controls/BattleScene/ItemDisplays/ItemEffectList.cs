
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using Core.UI;
using Core.UI.Utils;

using ItemModule.Data;
using PlayerModule.Data;
using QuestionModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.ItemDisplays {

    /// <summary>
    /// 普通星星显示
    /// </summary>
    public class ItemEffectList : ContainerDisplay<EffectData> {

        /// <summary>
        /// 效果显示间隔
        /// </summary>
        const float DeltaShowTime = 0.25f;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image effectArrows;

        public Texture2D recoveryArrows, promotionArrows;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            registerUpdateLayout(container);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="count">数目</param>
        public void setItem(IEffectsConvertable item) {
            setItems(item.convertToEffectData());
        }

        #endregion

        #region 界面绘制
        
        /// <summary>
        /// 绘制提升箭头
        /// </summary>
        void drawArrows() {
            if (items.Count <= 0) clearArrows();
            else {
                var effect = items[0];
                Texture2D texture = null;
                if (effect.isRecovery()) texture = recoveryArrows;
                if (effect.isPromotion()) texture = promotionArrows;

                if (texture == null) clearArrows();
                else {
                    effectArrows.gameObject.SetActive(true);
                    effectArrows.overrideSprite = AssetLoader.generateSprite(texture);
                }
            }
        }

        /// <summary>
        /// 创建子视图
        /// </summary>
        protected override void createSubViews() {
            CoroutineUtils.resetActions();
            base.createSubViews();
            doRoutine(CoroutineUtils.generateCoroutine());
        }

        /// <summary>
        /// 创建子窗口（动画）
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        protected override void createSubView(EffectData item, int index) {
            var subView = createSubView(index);
            CoroutineUtils.addAction(() =>
                subView.startView(item), DeltaShowTime);
        }

        /// <summary>
        /// 清除提升箭头
        /// </summary>
        void clearArrows() {
            effectArrows.gameObject.SetActive(false);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            drawArrows();
        }

        /// <summary>
        /// 清除
        /// </summary>
        protected override void clear() {
            base.clear();
            clearArrows();
        }

        #endregion

    }
}