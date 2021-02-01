using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YLOAuth2.Common;
using YLOAuth2.Test;

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
        [TestErrFilter]
        public string Test(string str, string key, int mode)
        {
            var aa = CryptHelper.GetKeyPair1();
            var bb = CryptHelper.GetKeyPair2();

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
        [TestErrFilter]
        public string DeTest(string str, string key)
        {
            return CryptHelper.DecryptByDES(str, key);
        }




        public string LoginByUser(string account, string pwd)
        {

            return "1234";
        }
    }
}