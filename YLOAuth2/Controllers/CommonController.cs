using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YLOAuth2.Common;

namespace YLOAuth2.Controllers
{
    public class CommonController : Controller
    {

        public ActionResult Login()
        {

            return View();
        }

        public ActionResult Test()
        {

            return View();
        }

        [HttpPost]
        public string Test(string str, string key, int mode)
        {
            if (mode == 1)
            {
                return CryptHelper.EncryptByMD5(str, 16);
            }
            else
            {
                return CryptHelper.EncryptByDES(str, key);
            }
        }

        [HttpPost]
        public string DeTest(string str, string key)
        {
            return CryptHelper.DeCryptByDES(str, key);
        }




        public string LoginByUser(string account, string pwd)
        {

            return "1234";
        }
    }
}