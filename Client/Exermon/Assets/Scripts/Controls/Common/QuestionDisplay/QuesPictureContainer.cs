using UnityEngine;

using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls.QuestionDisplay {

    using Question = QuestionModule.Data.Question;

    /// <summary>
    /// 题目图片容器
    /// </summary>
    public class QuesPictureContainer :
        SelectableContainerDisplay<Texture2D>, IItemDetailDisplay<Question> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuesBigPicture detail; // 帮助界面
        
        #region 接口实现

        /// <summary>
        /// 题目
        /// </summary>
        Question question;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="container"></param>
        public void configure(IContainerDisplay<Question> container) { }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns></returns>
        public Question getItem() { return question; }

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index"></param>
        /// <param name="refresh"></param>
        public void setItem(Question item, int _ = -1, bool __ = false) {
            setItems(item.textures());
        }

        public void setItem(Question item, bool _ = false) {
            setItem(item, -1, _);
        }

        public void startView(Question item, int index = -1) {
            startView();
            setItem(item, index, true);
        }

        public void startView(Question item) {
            startView();
            setItem(item, true);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        public override IItemDetailDisplay<Texture2D> getItemDetail() {
            return detail;
        }
        
        #endregion
    }
}