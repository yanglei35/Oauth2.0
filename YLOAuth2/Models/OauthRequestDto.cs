using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YLOAuth2.Models
{
    public class OauthRequestDto
    {
        /// <summary>
        /// grant_type：表示使用的授权模式，必选项，此处的值固定为"authorization_code"。
        /// </summary>
        public string grant_type { get; set; }

        /// <summary>
        /// code：表示上一步获得的授权码，必选项。
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// redirect_uri：表示重定向URI，必选项，且必须与A步骤中的该参数值保持一致。
        /// </summary>
        public string redirect_uri { get; set; }

        /// <summary>
        /// client_id：表示客户端ID，必选项。
        /// </summary>
        public string client_id { get; set; }

    }
}