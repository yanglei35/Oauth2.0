using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebTest.Models;

namespace WebTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public async Task<ActionResult> IndexAsync(string code, string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                /*
                 * grant_type：表示使用的授权模式，必选项，此处的值固定为"authorization_code"。
                 *  code：表示上一步获得的授权码，必选项。
                 *  redirect_uri：表示重定向URI，必选项，且必须与A步骤中的该参数值保持一致。
                 *  client_id：表示客户端ID，必选项
                 */
                var serverUrl = "http://localhost:8088/Oauth/GetToken";
                var grantType = "authorization_code";
                var redirectUrl = "www";
                var clientId = "AA";
                var url = $"{serverUrl}?grant_type={grantType}&code={code}&redirect_uri={redirectUrl}&client_id={clientId}";
                HttpClient httpClient = new HttpClient();
                HttpContent content = new StringContent(JsonConvert.SerializeObject(new { grantType = "authorization_code", code = code, redirect_uri = "www", client_id = "AA" }));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(serverUrl, content);
                response.EnsureSuccessStatusCode();
                // HttpContentExtensions

                //解决方案：添加一个System.Net.Http.Formatting.dll。此程序集也位于C：\ Program Files \ Microsoft ASP.NET \ ASP.NET MVC 4 \ Assemblies文件夹中。
                var responseBody = await response.Content.ReadAsAsync<ResponseDto>();
                ViewBag.Token = await response.Content.ReadAsStringAsync();
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}