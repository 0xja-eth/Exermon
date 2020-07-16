
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Systems;

using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls.LogDisplay {

	/// <summary>
	/// 游戏中用于显示日志
	/// </summary>
	public class LogItemDisplay : ItemDisplay<TestSystem.LogItem> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text type, output, stack;

		#region 界面控制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(TestSystem.LogItem item) {
			base.drawExactlyItem(item);
			output.text = item.output;
			stack.text = item.stack;
			type.text = item.type.ToString();
		}
		
        #endregion
    }
}