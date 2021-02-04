using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YLOAuth2.Models
{
    public class OauthResponseDto
    {




        /// <summary>
        /// access_token：表示访问令牌，必选项。
        /// </summary>
        public string access_token { get; set; }
      
        /// <summary>
        ///  token_type：表示令牌类型，该值大小写不敏感，必选项，可以是bearer类型或mac类型。
        /// </summary>
        public string token_type { get; set; }
       
        /// <summary>
        /// expires_in：表示过期时间，单位为秒。如果省略该参数，必须其他方式设置过期时间。
        /// </summary>
        public int expires_in { get; set; }
       
        /// <summary>
        /// refresh_token：表示更新令牌，用来获取下一次的访问令牌，可选项。
        /// </summary>
        public string refresh_token { get; set; }
        
        /// <summary>
        /// scope：表示权限范围，如果与客户端申请的范围一致，此项可省略。
        /// </summary>
        public string scope { get; set; }
       
    }
}