
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

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }
    }
}