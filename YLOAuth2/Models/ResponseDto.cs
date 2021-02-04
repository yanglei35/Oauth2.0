using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YLOAuth2.Models
{
    [Serializable]
    public class ResponseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Object Data { get; set; }
    }
}