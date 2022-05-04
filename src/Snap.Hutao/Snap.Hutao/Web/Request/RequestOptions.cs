using System.Collections.Generic;

namespace Snap.Hutao.Web.Request
{
    /// <summary>
    /// 请求选项
    /// 用于添加到请求头中
    /// </summary>
    public class RequestOptions : Dictionary<string, string>
    {
        /// <summary>
        /// 不再使用
        /// </summary>
        [Obsolete("不再使用")]
        public const string CommonUA2101 = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBS/2.10.1";

        /// <summary>
        /// 不再使用
        /// </summary>
        [Obsolete("不再使用")]
        public const string CommonUA2111 = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBS/2.11.1";

        /// <summary>
        /// 支持更新的DS2算法
        /// </summary>
        public const string CommonUA2161 = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBS/2.16.1";

        /// <summary>
        /// 应用程序/Json
        /// </summary>
        public const string Json = "application/json";

        /// <summary>
        /// 指示请求由米游社发起
        /// </summary>
        public const string Hyperion = "com.mihoyo.hyperion";

        /// <summary>
        /// 设备Id
        /// </summary>
        public static readonly string DeviceId = Guid.NewGuid().ToString("D");
    }
}
