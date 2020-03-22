
using Core.Systems;
using Exception = System.Exception;

/// <summary>
/// 游戏异常
/// </summary>
namespace Core.Data.Exceptions {

    /// <summary>
    /// 游戏错误处理类
    /// </summary>
    public class GameException : Exception {

        /// <summary>
        /// 错误类型枚举
        /// </summary>
        public enum Type {
            SystemError = -1, // 系统错误
            NetworkError = -2, // 网络错误
            GameDisconnected = -3, // 游戏未连接
            ServerParamError = -4, // 服务器参数错误
            RequestObjectNotFound = -5, // 找不到请求对象
            UserUnlogin = -10 // 用户未登录
        }

        /// <summary>
        /// 错误代码
        /// </summary>
        public int code { get; protected set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string message { get; protected set; }

        /// <summary>
        /// 处理函数
        /// </summary>
        NetworkSystem.RequestObject.ErrorAction _action;
        public NetworkSystem.RequestObject.ErrorAction action {
            get { return _action; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">错误代码</param>
        /// <param name="action">处理函数</param>
        public GameException(Type code, NetworkSystem.RequestObject.ErrorAction action = null) :
            this((int)code, getMessage(code), action) {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">错误代码</param>
        /// <param name="message">错误消息</param>
        /// <param name="action">处理函数</param>
        public GameException(int code, string message, NetworkSystem.RequestObject.ErrorAction action = null) :
            base(message) {
            this.code = code; this.message = message; _action = action;
        }

        /// <summary>
        /// 获取错误码对应的错误消息
        /// </summary>
        /// <param name="code">错误码</param>
        /// <returns>错误消息</returns>
        public static string getMessage(Type code) {
            switch (code) {
                case Type.SystemError: return "系统错误！";
                case Type.NetworkError: return "网络错误！";
                case Type.GameDisconnected: return "游戏未连接！";
                case Type.ServerParamError: return "服务器参数错误！";
                case Type.RequestObjectNotFound: return "找不到请求对象！";
                case Type.UserUnlogin: return "用户未登录！";
            }
            return "未知错误！";
        }
    }
}