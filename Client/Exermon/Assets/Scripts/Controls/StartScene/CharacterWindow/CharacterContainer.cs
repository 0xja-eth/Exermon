using UnityEngine;
using UnityEngine.UI;

using PlayerModule.Data;
using PlayerModule.Services;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 开始场景命名空间
/// </summary>
namespace UI.StartScene { }

/// <summary>
/// 开始场景控件
/// </summary>
namespace UI.StartScene.Controls { }

/// <summary>
/// 开始场景中人物窗口控件
/// </summary>
namespace UI.StartScene.Controls.Character {

    using PlayerModule.Data;

    /// <summary>
    /// 人物容器
    /// </summary>
    public class CharacterContainer : ItemContainer<Character> {

        /// <summary>
        /// 常量设置
        /// </summary>
        const float WidthRate = 1; // 移动宽度比率
        const float ScaleRate = 0.8f; // 缩放比率
        const float FadeRate = 0.25f; // 淡出比率

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text name, description;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        int posIndex;

        #region 启动/结束控制

        /// <summary>
        /// 启动视窗
        /// </summary>
        public override void startView(int index = 0) {
            base.startView();
            select(index, true);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 物品变更回调
        /// </summary>
        protected override void onItemsChanged() {
            base.onItemsChanged();
            select(0, true);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="index">索引</param>
        public override void select(int index) {
            select(index, false);
        }
        /// <param name="force">是否强制切换</param>
        public void select(int index, bool force) {
            var posIndex = force ? index :
                clacNearestPosIndex(index);
            select(index, posIndex, force);
        }
        /// <param name="posIndex">位置索引</param>
        public void select(int index, int posIndex, bool force = false) {
            this.posIndex = posIndex;
            refreshPosition(force);
            base.select(index);
        }

        /// <summary>
        /// 计算最接近当前真实索引的位置索引的值
        /// </summary>
        /// <param name="index">真实索引</param>
        /// <returns>最接近的位置索引</returns>
        int clacNearestPosIndex(int index) {
            var cnt = itemDisplaysCount();
            var pIndex = getLoopedIndex(posIndex);
            if (pIndex > index) {
                var d1 = pIndex - index;
                var d2 = index + cnt - pIndex;
                if (d1 <= d2) return posIndex - d1;
                return posIndex + d2;
            }
            if (pIndex < index) {
                var d1 = pIndex + cnt - index;
                var d2 = index - pIndex;
                if (d1 <= d2) return posIndex - d1;
                return posIndex + d2;
            }
            return posIndex;
        }

        #endregion

        #region 动画计算控制

        /// <summary>
        /// 获取动画的最大宽度
        /// </summary>
        /// <returns>宽度</returns>
        float maxWidth() {
            var rt = transform as RectTransform;
            return rt.rect.width / 2 * WidthRate;
        }

        /// <summary>
        /// 最大/最小缩放
        /// </summary>
        /// <returns>缩放</returns>
        float maxScale() { return 1; }
        float minScale() { return maxScale() * ScaleRate; }

        /// <summary>
        /// 最大/最小淡入
        /// </summary>
        /// <returns>淡入程度（明亮度）</returns>
        float maxFade() { return 1; }
        float minFade() { return maxFade() * FadeRate; }

        /// <summary>
        /// 计算X坐标
        /// </summary>
        /// <param name="i">索引</param>
        /// <returns>X坐标</returns>
        public float calcX(float i) {
            float n = itemDisplaysCount();
            float w = maxWidth();
            float k = Mathf.Floor((i + n / 4) / n);
            i -= k * n;
            if (i < n / 4) return 4 * w / n * i;
            if (i < 3 * n / 4) return 2 * w - 4 * w / n * i;
            return 0;
        }

        /// <summary>
        /// 计算缩放
        /// </summary>
        /// <param name="i">索引</param>
        /// <returns>缩放</returns>
        public float calcScale(float i) {
            float n = itemDisplaysCount();
            float maxS = maxScale();
            float minS = minScale();
            float k = Mathf.Floor(i / n);
            i -= k * n;
            if (i < n / 2) return 2 * (minS - maxS) / n * i + maxS;
            if (i <= n) return 2 * (maxS - minS) / n * i + 2 * minS - maxS;
            return 1;
        }

        /// <summary>
        /// 计算淡入
        /// </summary>
        /// <param name="i">索引</param>
        /// <returns>淡入</returns>
        public float calcFade(float i) {
            float n = itemDisplaysCount();
            float maxF = maxFade();
            float minF = minFade();
            float k = Mathf.Floor(i / n);
            i -= k * n;
            if (i < n / 2) return 2 * (minF - maxF) / n * i + maxF;
            if (i <= n) return 2 * (maxF - minF) / n * i + 2 * minF - maxF;
            return 1;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制实际物品帮助
        /// </summary>
        /// <param name="character">物品</param>
        protected override void drawExactlyItemHelp(Character character) {
            name.text = character.name;
            description.text = character.description;
        }

        /// <summary>
        /// 清除物品帮助
        /// </summary>
        protected override void clearItemHelp() {
            name.text = description.text = "";
        }

        /// <summary>
        /// 刷新位置
        /// </summary>
        public void refreshPosition(bool force = false) {
            var cnt = itemDisplaysCount();
            for (int i = 0; i < cnt; i++) {
                var item = subViews[i] as CharacterDisplay;
                var posIndex = i - this.posIndex;
                item.setPosIndex(posIndex, force);
            }
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 是否有BustItem在移动
        /// </summary>
        /// <returns>是否移动</returns>
        public bool isMoving() {
            foreach (var display in subViews)
                if (((CharacterDisplay)display).isMoving()) return true;
            return false;
        }

        /// <summary>
        /// 当“下一个”按钮按下时回调事件
        /// </summary>
        public void onNext() {
            if (isMoving()) return;
            select(selectedIndex + 1, posIndex + 1);
        }

        /// <summary>
        /// 当“上一个”按钮按下时回调事件
        /// </summary>
        public void onPrev() {
            if (isMoving()) return;
            select(selectedIndex - 1, posIndex - 1);
        }

        #endregion
    }
}