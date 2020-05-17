
namespace Core.Data {

    /// <summary>
    /// 配置数据
    /// </summary>
    public class ConfigureData : BaseData {

        /// <summary>
        /// 设置项
        /// </summary>
        [AutoConvert]
        public string rememberPassword { get; set; } = null; // 记住密码
        [AutoConvert]
        public string rememberUsername { get; set; } = null; // 记住账号
        [AutoConvert]
        public bool autoLogin { get; set; } = false; // 自动登录

        [AutoConvert]
        public int exerSubjectId { get; set; } = 1; // 刷题科目ID记忆
        [AutoConvert]
        public int exerGenType { get; set; } = 0; // 刷题类型记忆
        [AutoConvert]
        public int exerCount { get; set; } = 1; // 刷题数目记忆

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }
    }
}