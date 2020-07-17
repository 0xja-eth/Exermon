
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Core.Systems {

    /// <summary>
    /// 测试系统控制类
    /// </summary>
    /// <remarks>
    /// 实现游戏的性能测试功能，将 Enable 设置为 True 时启用测试功能
    /// </remarks>
    public class TestSystem : BaseSystem<TestSystem> {

		/// <summary>
		/// 日志项数据
		/// </summary>
		public class LogItem {

			/// <summary>
			/// 日志文本
			/// </summary>
			public string output, stack;

			/// <summary>
			/// 日志类型
			/// </summary>
			public LogType type;

			/// <summary>
			/// 构造函数
			/// </summary>
			public LogItem(string output, string stack, LogType type) {
				this.output = output; this.stack = stack; this.type = type;
			}
		}

		/// <summary>
		/// 启用测试
		/// </summary>
		public const bool EnableLog = true;
		public const bool EnableLogWarning = true;
		public const bool EnableLogError = true;

		public const bool EnableTimer = false;

		public const bool EnableLogCallback = true;

		public const string LogFormat = "{0}: {1}";

        /// <summary>
        /// 测试用计时器
        /// </summary>
        static string testTitle;
        static Stopwatch testTimer = null;
        static List<Tuple<string, decimal>> testPoints =
            new List<Tuple<string, decimal>>();

		#region 日志封装

		/// <summary>
		/// 日志
		/// </summary>
		/// <param name="str"></param>
		public static void log(object message, UnityEngine.Object obj = null) {
			if (!EnableLog) return;

			if (obj != null) message = string.Format(
				LogFormat, obj.name, message);

			Debug.Log(message);
		}

		/// <summary>
		/// 警告
		/// </summary>
		/// <param name="str"></param>
		public static void warning(object message, UnityEngine.Object obj = null) {
			if (!EnableLogWarning) return;

			if (obj != null) message = string.Format(
				LogFormat, obj.name, message);

			Debug.LogWarning(message);
		}

		/// <summary>
		/// 报错
		/// </summary>
		/// <param name="str"></param>
		public static void error(object message, UnityEngine.Object obj = null) {
			if (!EnableLogError) return;

			if (obj != null) message = string.Format(
				LogFormat, obj.name, message);

			Debug.LogError(message);
		}

		#endregion

		#region 异常委托

		/// <summary>
		/// 设置日志委托
		/// </summary>
		public static void setLogCallback(Application.LogCallback cb) {
			if (EnableLogCallback) Application.logMessageReceived += cb;
		}

		/// <summary>
		/// 删除日志委托
		/// </summary>
		public static void removeLogCallback(Application.LogCallback cb) {
			if (EnableLogCallback) Application.logMessageReceived -= cb;
		}

		#endregion

		#region 计时控制

		/// <summary>
		/// 开始测试
		/// </summary>
		/// <param name="title">测试名称</param>
		public static void startTimer(string title) {
            if (!EnableTimer) return;
            if (testTimer != null) catchTimer(title);
            else {
                testTimer = new Stopwatch();
                testTitle = title;
                testTimer.Start();
            }
        }

        /// <summary>
        /// 捕捉测试点数据
        /// </summary>
        /// <param name="name">测试点名称</param>
        public static void catchTimer(string name = "") {
            if (testTimer == null) return;
            testTimer.Stop();
            decimal time = testTimer.ElapsedTicks;
            testPoints.Add(new Tuple<string, decimal>(name, time));

			log(name + ":" + time);
            testTimer.Restart();
        }

        /// <summary>
        /// 结束测试
        /// </summary>
        /// <param name="name">测试点名称</param>
        /// <param name="alert">是否弹窗</param>
        public static void endTimer(string name = "End", bool alert = false) {
            if (testTimer == null) return;
            catchTimer(name);
            testTimer.Stop();
            displayTimers(alert);
            clearTimer();
        }

        /// <summary>
        /// 显示测试数据
        /// </summary>
        /// <param name="alert">是否弹窗</param>
        static void displayTimers(bool alert = false) {
            string result = testTitle + ":";
            decimal sum = 0;
            foreach (var point in testPoints) {
                sum += point.Item2;
                var ms = point.Item2 / Stopwatch.Frequency * 1000;
                result += "\n" + point.Item1 + ": " + ms + "ms";
            }
            string final = "Sum: " + (sum / Stopwatch.Frequency * 1000) + "ms";
            if (alert) GameSystem.get().requestAlert(
                "<size=18>" + result + "\n</size>" + final);

			log(result + "\n" + final);
        }

        /// <summary>
        /// 清空测试数据
        /// </summary>
        static void clearTimer() {
            testTimer = null;
            testPoints.Clear();
        }

        #endregion

    }
}