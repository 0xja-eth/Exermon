using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Core.UI {

    /// <summary>
    /// 基本状态行为
    /// </summary>
    public class BaseStateBehaviour : StateMachineBehaviour {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}

        /// <summary>
        /// 参数储存
        /// </summary>
        Animator animator;
        AnimatorStateInfo stateInfo;
        AnimatorControllerPlayable controller;

        int layerIndex;
        int stateMachinePathHash;

        /// <summary>
        /// 状态进入
        /// </summary>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            this.animator = animator;
            this.stateInfo = stateInfo;
            this.layerIndex = layerIndex;

            onStatusEnter(animator.gameObject);
        }

        /// <summary>
        /// 状态进入
        /// </summary>
        protected virtual void onStatusEnter(GameObject go) {}

        /// <summary>
        /// 状态更新
        /// </summary>
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            this.animator = animator;
            this.stateInfo = stateInfo;
            this.layerIndex = layerIndex;

            onStatusUpdate(animator.gameObject);
        }

        /// <summary>
        /// 状态更新
        /// </summary>
        protected virtual void onStatusUpdate(GameObject go) { }

        /// <summary>
        /// 状态结束
        /// </summary>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateExit(animator, stateInfo, layerIndex);

            this.animator = animator;
            this.stateInfo = stateInfo;
            this.layerIndex = layerIndex;

            onStatusExit(animator.gameObject);
        }
        /// <summary>
        /// 状态结束
        /// </summary>
        protected virtual void onStatusExit(GameObject go) { }
        
    }

}
