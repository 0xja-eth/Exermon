using UnityEngine;

using RecordModule.Data;

using UI.Common.Controls.ItemDisplays;
using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.EnglishPro.ListenScene.Controls {
    /// <summary>
    /// 题目选项容器
    /// </summary>
    public class ListenChoiceContainer :
        SelectableContainerDisplay<ListeningSubQuestion.Choice>, IItemDetailDisplay<ListeningSubQuestion> {
        #region 接口实现

        /// <summary>
        /// 题目
        /// </summary>
        ListeningSubQuestion question;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="container"></param>
        public void configure(IContainerDisplay<ListeningSubQuestion> container) { }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns></returns>
        public ListeningSubQuestion getItem() { return question; }

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index"></param>
        /// <param name="refresh"></param>
        public void setItem(ListeningSubQuestion item, int _ = -1, bool __ = false) {
            question = item;
            var choices = item.choices;
            setItems(choices);
            maxCheck = 1;
        }

        public void setItem(ListeningSubQuestion item, bool _ = false) {
            setItem(item, -1, _);
        }

        public void clearItem() {
            setItem(null, -1);
        }

        public void startView(ListeningSubQuestion item, int _ = -1) {
            base.startView();
            setItem(item, _, true);
        }

        public void startView(ListeningSubQuestion item) {
            base.startView();
            setItem(item, true);
        }
        #endregion

    }
}