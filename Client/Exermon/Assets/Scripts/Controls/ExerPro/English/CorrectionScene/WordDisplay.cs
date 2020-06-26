using UI.Common.Controls.ItemDisplays;
using UnityEngine;
using UnityEngine.UI;

using FrontendWrongItem = ExerPro.EnglishModule.Data.
	CorrectionQuestion.FrontendWrongItem;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {

    /// <summary>
    /// 单词
    /// </summary
    public class WordDisplay : SelectableItemDisplay<string> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
        public GameObject deletedImage;
        public Text text;

        /// <summary>
        /// 状态枚举
        /// </summary>
        public enum State {
            Original, Modefied,
			AddedNext, AddedPrev, // 后一个增加，前一个增加
			Deleted
        }

		/// <summary>
		/// 内部变量定义
		/// </summary>
        public string originalWord { get; set; }
        public State state { get; set; } = State.Original;

		#region 数据控制

		/// <summary>
		/// 是否有修改
		/// </summary>
		/// <returns></returns>
		public bool isChanged() {
			return state != State.Original;
		}

		/// <summary>
		/// 生成错误项
		/// </summary>
		/// <param name="display"></param>
		/// <returns></returns>
		public FrontendWrongItem generateWrongItem(int sid, int wid) {
			if (isChanged()) {
				Debug.Log("generateWrongItem: " + sid + ", " + wid + 
					": " + originalWord + " -> " + item);
				return new FrontendWrongItem(sid, wid, item);
			}
			return null;
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(string item) {
            base.drawExactlyItem(item);
            text.text = item;
            switch (state) {
                case State.Modefied:
                    text.color = new Color(1.0f, 0.0f, 0.0f);
                    break;
				case State.AddedNext:
				case State.AddedPrev:
					text.color = new Color(1.0f, 0.0f, 0.0f);
                    break;
                case State.Deleted:
                    text.color = new Color(1.0f, 0.0f, 0.0f);
                    deletedImage.SetActive(true);
                    break;
                default:
                    text.color = new Color(1.0f, 1.0f, 1.0f);
                    deletedImage.SetActive(false);
                    break;
            }
        }
  
		#endregion
    }


}
