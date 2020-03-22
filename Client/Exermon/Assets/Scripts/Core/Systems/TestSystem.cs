
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
        /// 启用测试
        /// </summary>
        public const bool Enable = true;

        /// <summary>
        /// 测试用计时器
        /// </summary>
        static string testTitle;
        static Stopwatch testTimer = null;
        static List<Tuple<string, decimal>> testPoints =
            new List<Tuple<string, decimal>>();

        #region 测试控制

        /// <summary>
        /// 开始测试
        /// </summary>
        /// <param name="title">测试名称</param>
        public static void startTest(string title) {
            if (!Enable) return;
            if (testTimer != null) catchTest(title);
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
        public static void catchTest(string name = "") {
            if (testTimer == null) return;
            testTimer.Stop();
            decimal time = testTimer.ElapsedTicks;
            testPoints.Add(new Tuple<string, decimal>(name, time));
            Debug.Log(name + ":" + time);
            testTimer.Restart();
        }

        /// <summary>
        /// 结束测试
        /// </summary>
        /// <param name="name">测试点名称</param>
        /// <param name="alert">是否弹窗</param>
        public static void endTest(string name = "End", bool alert = false) {
            if (testTimer == null) return;
            catchTest(name);
            testTimer.Stop();
            displayTests(alert);
            clearTest();
        }

        /// <summary>
        /// 显示测试数据
        /// </summary>
        /// <param name="alert">是否弹窗</param>
        static void displayTests(bool alert = false) {
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
            Debug.Log(result + "\n" + final);
        }

        /// <summary>
        /// 清空测试数据
        /// </summary>
        static void clearTest() {
            testTimer = null;
            testPoints.Clear();
        }

        #endregion

    }
}