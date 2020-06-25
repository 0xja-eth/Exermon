
using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {

    /// <summary>
    /// 改错题目显示
    /// </summary
    public class ArticleDisplay : ContainerDisplay<string>,
        IItemDisplay<CorrectionQuestion> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public GameObject correctionWindow;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		public Text changedBeforeValue { get; set; }

		/// <summary>
		/// 内部变量定义
		/// </summary>
		CorrectionQuestion question;

		/// <summary>
		/// 获取物品
		/// </summary>
		/// <returns></returns>
        public CorrectionQuestion getItem() {
            return question;
        }

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">改错题项</param>
        /// <param name="force">强制</param>
        /// <returns>null</returns>
        public void setItem(CorrectionQuestion item, bool force = false) {
            question = item; setItems(item.sentences());
        }

        /// <summary>
        /// 开启视图
        /// </summary>
        /// <param name="item"></param>
        public void startView(CorrectionQuestion item) {
            startView(); setItem(item, true);
        }

    }
}
