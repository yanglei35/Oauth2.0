using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YLOAuth2.Test
{
    public class TestErrFilterAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var exp = filterContext.Exception;
            if(exp.GetType().Name== "MyException")
            {
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.ExceptionHandled = true;
                //关闭IIS自定义错误
              //  filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.Result = new ContentResult()
                {
                    Content = exp.Message
                };
            }
        }
    }
}