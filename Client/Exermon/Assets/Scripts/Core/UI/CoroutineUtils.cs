using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Core.UI.Utils {

    /// <summary>
    /// 协程动作
    /// </summary>
    public class CoroutineAction {

        /// <summary>
        /// 等待类型
        /// </summary>
        public enum WaitType {
            Null,
            WaitForSeconds,
            //WaitForSecondsRealtime, 
            WaitForEndOfFrame, 
            WaitForFixedUpdate,
        }

        /// <summary>
        /// 行动
        /// </summary>
        public UnityAction action;

        /// <summary>
        /// 等待类型
        /// </summary>
        public WaitType waitType;

        /// <summary>
        /// 延时
        /// </summary>
        public float duration;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CoroutineAction(UnityAction action, 
            WaitType waitType, float duration = 0) {
            this.action = action; this.waitType = waitType;
            this.duration = duration;
        }

    }

    /// <summary>
    /// 协程工具类
    /// </summary>
    public static class CoroutineUtils {

        /// <summary>
        /// 协程项列表
        /// </summary>
        static List<CoroutineAction> actions = new List<CoroutineAction>();

        /// <summary>
        /// 添加行动
        /// </summary>
        /// <param name="action">行动</param>
        /// <param name="waitType">等待类型</param>
        /// <param name="duration">等待时间</param>
        public static void addAction(UnityAction action,
            CoroutineAction.WaitType waitType, float duration = 0) {
            actions.Add(new CoroutineAction(action, waitType, duration));
        }
        public static void addAction(UnityAction action, float duration) {
            addAction(action, CoroutineAction.WaitType.WaitForSeconds, duration);
        }
        public static void addAction(UnityAction action) {
            addAction(action, CoroutineAction.WaitType.Null);
        }

        /// <summary>
        /// 重置行动列表
        /// </summary>
        public static void resetActions() {
            actions.Clear();
        }

        /// <summary>
        /// 生成用于运行的协程
        /// </summary>
        /// <param name="actions">行动</param>
        /// <returns></returns>
        public static IEnumerator generateCoroutine(
            CoroutineAction[] actions = null) {
            if (actions == null) {
                actions = CoroutineUtils.actions.ToArray();
                resetActions();
            }
            foreach(var action in actions) {
                action.action.Invoke();
                yield return getInstruction(
                    action.waitType, action.duration);
            }
        }

        /// <summary>
        /// 获取等待指示器
        /// </summary>
        /// <param name="waitType">等待类型</param>
        /// <param name="duration">等待时间</param>
        /// <returns></returns>
        static YieldInstruction getInstruction(
            CoroutineAction.WaitType waitType, float duration = 0) {
            switch(waitType) {
                case CoroutineAction.WaitType.WaitForEndOfFrame:
                    return new WaitForEndOfFrame();
                case CoroutineAction.WaitType.WaitForFixedUpdate:
                    return new WaitForFixedUpdate();
                case CoroutineAction.WaitType.WaitForSeconds:
                    return new WaitForSeconds(duration);
                //case CoroutineAction.WaitType.WaitForSecondsRealtime:
                //    return new WaitForSecondsRealtime(duration);
                default:
                    return null;
            }
        }
    }
}