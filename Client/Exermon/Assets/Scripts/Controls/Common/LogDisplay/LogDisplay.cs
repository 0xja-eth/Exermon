
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
    public class LogDisplay : ContainerDisplay<TestSystem.LogItem> {

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public bool showLog = true;
		public bool showWarning = true;
		public bool showError = true;

		/// <summary>
		/// 正在显示日志
		/// </summary>
		bool showingLog = false;

        #region 初始化

        /// <summary>
        /// 初次打开时初始化（子类中重载）
        /// </summary>
        protected override void initializeOnce() {
			TestSystem.setLogCallback(addLog);
		}

		/// <summary>
		/// 释放回调
		/// </summary>
		private void OnDestroy() {
			TestSystem.removeLogCallback(addLog);
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否包含
		/// </summary>
		protected override bool isIncluded(TestSystem.LogItem item) {
			return base.isIncluded(item) && 
				((item.type == LogType.Log && showLog) ||
				(item.type == LogType.Warning && showWarning) ||
				(item.type == LogType.Error && showError));
		}

		/// <summary>
		/// 绘制日志
		/// </summary>
		/// <param name="output">日志文本</param>
		/// <param name="stack">调用堆栈</param>
		/// <param name="type">日志类型</param>
		void addLog(string output, string stack, LogType type) {
			if (showingLog) return;
			showingLog = true;
			addItem(new TestSystem.LogItem(output, stack, type));
			showingLog = false;
		}

        #endregion
    }
}