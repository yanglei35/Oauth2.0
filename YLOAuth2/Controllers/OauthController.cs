using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using YLOAuth2.Common;
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
           /// var db=redis.GetDatabase();
         //   db.StringSet(param.client_id, param.state);
            return View();
        }

        [HttpPost]
        public JsonResult GetToken(OauthRequestDto oauthRequestDto)
        {
            ResponseDto responseDto = new ResponseDto();
            if (oauthRequestDto == null || string.IsNullOrEmpty(oauthRequestDto.code) || string.IsNullOrEmpty(oauthRequestDto.client_id) || string.IsNullOrEmpty(oauthRequestDto.redirect_uri))
            {
                responseDto.Success = 0;
                responseDto.Message = "传入的参数为空";
                responseDto.Data = null;
                return Json(responseDto);
            }
            //从redis中取授权码
            var db = redis.GetDatabase();
            string code = db.StringGet(oauthRequestDto.client_id);
            //取不到，则说明授权码过期
            if (string.IsNullOrEmpty(code))
            {
                responseDto.Success = 0;
                responseDto.Message = "无效的授权码！";
                responseDto.Data = null;
                return Json(responseDto);
            }
            //
            if (code != oauthRequestDto.code)
            {
                responseDto.Success = 0;
                responseDto.Message = "无效的授权码！";
                responseDto.Data = null;
                return Json(responseDto);
            }

            //从redis获取授权码是否被使用过的标记
            string useInt = db.StringGet(code);
            if (useInt == "1") //表示该码已被使用
            {
                responseDto.Success = 0;
                responseDto.Message = "无效的授权码！";
                responseDto.Data = null;
                return Json(responseDto);
            }
            //获取token
            string token = db.StringGet(oauthRequestDto.client_id + "Token");

            OauthResponseDto oauthResponseDto = new OauthResponseDto();
            if (string.IsNullOrEmpty(token))
            {
                responseDto.Success = 1;
                responseDto.Message = "授权成功！";
                responseDto.Data = CreateToken(oauthRequestDto);
            }
            else
            {
                responseDto.Success = 1;
                responseDto.Message = "授权成功！";
                responseDto.Data = new OauthResponseDto
                {
                    access_token = token,
                    refresh_token = db.StringGet(oauthRequestDto.client_id + "RefreshToken"),
                    expires_in = 7200,
                    token_type = "bearer"
                };
            }
            return Json(responseDto);
        }



        private string CreateUrl(RequestParam param)
        {
            var uri = new Uri(param.redirect_uri);
            var code = CreateAuthorazitionCode(param);
            if (string.IsNullOrEmpty(uri.Query))
            {
                return $"{param.redirect_uri}?code={code}&state=123";
            }
            return $"{param.redirect_uri}&code={code}&state=123";
        }


        private string CreateAuthorazitionCode(RequestParam param)
        {
            var db = redis.GetDatabase();
            string code = db.StringGet(param.client_id);
            if (string.IsNullOrEmpty(code))
            {
                //生成授权码
                code = CryptHelper.EncryptByMD5_Base64(DateTime.Now.Ticks.ToString());
                //保存授权码 
                db.StringSet(param.client_id, code, TimeSpan.FromSeconds(600));
                //授权码是否不使用，0表示没有使用，1表示使用过
                db.StringSet(code, 0, TimeSpan.FromSeconds(600));
            }
            return code;
        }


        private OauthResponseDto CreateToken(OauthRequestDto oauthRequestDto)
        {
            OauthResponseDto responseDto = new OauthResponseDto();
            //生成token
            var token= CryptHelper.EncryptByAES(oauthRequestDto.client_id + DateTime.Now.Ticks.ToString(), "1qaz2wsx1qaz2wsx1qaz2wsx1qaz2wsx");
            var refreshToken= CryptHelper.EncryptByAES(oauthRequestDto.client_id + "refreshToken" + DateTime.Now.Ticks.ToString(), "1qaz2wsx1qaz2wsx1qaz2wsx1qaz2wsx");
            responseDto.access_token = token;
            responseDto.token_type = "bearer";
            responseDto.expires_in = 7200;
            responseDto.refresh_token = refreshToken;

            var db = redis.GetDatabase();
            db.StringSet(oauthRequestDto.client_id + "Token", token, TimeSpan.FromSeconds(7200));
            //TODO 刷新的token要怎么处理
            db.StringSet(oauthRequestDto.client_id + "RefreshToken", refreshToken);
            return responseDto;
        }

        public RedirectResult RedirectResult()
        {
            string url = "http://baidu.com";
            return new RedirectResult(url);

        }
    }
}