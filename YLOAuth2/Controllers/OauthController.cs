using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YLOAuth2.Models;

namespace YLOAuth2.Controllers
{
    public class OauthController : Controller
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        /// <summary>
        /// 获取授权码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult Authorize(RequestParam param)
        {
            if (string.IsNullOrEmpty(param.client_id))
            {
                throw new ArgumentNullException("client_id参数不能为空！");
            }
            if (string.IsNullOrEmpty(param.redirect_uri))
            {
                throw new ArgumentNullException("redirect_uri参数不能为空！");
            }
            if (string.IsNullOrEmpty(param.response_type))
            {
                throw new ArgumentNullException("response_type参数不能为空！");
            }
            ViewBag.RedirectUrl = CreateUrl(param);
            var db=redis.GetDatabase();
            db.StringSet(param.client_id, param.state);
            return View();
        }

        private string CreateUrl(RequestParam param)
        {
            var uri = new Uri(param.redirect_uri);
            var code = CreateAuthorazitionCode(param);
            if (string.IsNullOrEmpty(uri.Query))
            {
                return $"{param.redirect_uri}?code={code}";
            }
            return $"{param.redirect_uri}&code={code}";
        }


        private string CreateAuthorazitionCode(RequestParam param)
        {
            Random rd = new Random(0);
            var code= rd.Next(100, 999).ToString();
            var db = redis.GetDatabase();
            db.StringSet(param.client_id, code, TimeSpan.FromSeconds(600));
            return code;
        }
        public RedirectResult RedirectResult()
        {
            string url = "http://baidu.com";
            return new RedirectResult(url);

        }
    }
}