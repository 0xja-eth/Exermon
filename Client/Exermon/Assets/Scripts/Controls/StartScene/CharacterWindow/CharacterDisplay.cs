
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using UI.Common.Controls.ItemDisplays;

namespace UI.StartScene.Controls.Character {

    using PlayerModule.Data;

    /// <summary>
    /// 人物显示项
    /// </summary>
    public class CharacterDisplay : SelectableItemDisplay<Character> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const float MoveDuration = 0.5f; // 移动用时

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image image; // 图片

        /// <summary>
        /// 内部变量声明
        /// </summary>
        RectTransform rectTransform;

        int posIndex;
        float realIndex;

        #region 初始化

        /// <summary>
        /// 初次打开时初始化（子类中重载）
        /// </summary>
        protected override void initializeOnce() {
            rectTransform = transform as RectTransform;
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateAnimation();
        }

        /// <summary>
        /// 更新动画
        /// </summary>
        void updateAnimation() {
            if (!isMoving()) return;
            updateRealIndex();
            updateSwitching();
        }

        /// <summary>
        /// 更新实际位置索引
        /// </summary>
        void updateRealIndex() {
            var dt = Time.deltaTime;
            var sign1 = Mathf.Sign(posIndex - realIndex);
            realIndex += dt / MoveDuration * sign1;
            var sign2 = Mathf.Sign(posIndex - realIndex);
            if (sign1 != sign2) realIndex = posIndex;
        }

        /// <summary>
        /// 更新切换效果
        /// </summary>
        void updateSwitching() {
            updateMoving();
            updateScaling();
            updateFading();
            updateSiblingIndex();
        }

        /// <summary>
        /// 分别更新三种动画
        /// </summary>
        void updateMoving() {
            var pos = rectTransform.anchoredPosition;
            pos.x = getContainer().calcX(realIndex);
            rectTransform.anchoredPosition = pos;
        }
        void updateScaling() {
            var s = getContainer().calcScale(realIndex);
            rectTransform.localScale = new Vector3(s, s, 1);
        }
        void updateFading() {
            var f = getContainer().calcFade(realIndex);
            image.color = new Color(f, f, f, 1);
        }
        void updateSiblingIndex() {
            var cnt = getContainer().itemDisplaysCount();
            int index = (int)Mathf.Round(realIndex);
            index = (index % cnt + cnt) % cnt;
            transform.SetSiblingIndex(cnt - index - 1);
        }

        /// <summary>
        /// 是否正在移动中
        /// </summary>
        /// <returns>移动</returns>
        public bool isMoving() {
            return realIndex != posIndex;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new CharacterContainer getContainer() {
            return container as CharacterContainer;
        }

        /// <summary>
        /// 设置位置索引
        /// </summary>
        /// <param name="posIndex">位置索引</param>
        public void setPosIndex(int posIndex, bool force = false) {
            this.posIndex = posIndex;
            if (force) {
                realIndex = posIndex;
                updateSwitching();
            }
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制物品
        /// </summary>
        protected override void drawExactlyItem(Character character) {
            image.gameObject.SetActive(true);
            image.overrideSprite = character.bust;
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            image.overrideSprite = null;
            image.gameObject.SetActive(false);
        }

        #endregion

    }
}