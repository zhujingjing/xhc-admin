using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace WebAppFrame.Controllers
{
    public class Base_Home : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           
            //判断用户是否登录
            string actionName = filterContext.ActionDescriptor.ActionName.ToLower();

            string strAddHomeBaseActions = "merchantssettled,merchantssettled2";
            strAddHomeBaseActions = strAddHomeBaseActions.ToLower();
            string[] aryAction = strAddHomeBaseActions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (aryAction.Contains(actionName))
            {
                //是登录
                if (true)
                {
                    filterContext.Result = RedirectToRoute(new { Controller = "Home", Action = "login"});
                    RedirectToAction("login", "Home");
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }


    #region 微信基类(负责获取openid)
    public class Wx_Base : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           
            string actionName = filterContext.ActionDescriptor.ActionName.ToLower();

            string strAddHomeBaseActions = "GetOpenId,Bottom,Pay_ZFB";
            strAddHomeBaseActions = strAddHomeBaseActions.ToLower();
            string[] aryAction = strAddHomeBaseActions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!aryAction.Contains(actionName))
            {
                if (CommonTool.Common.GetAppSetting("useWxMoniter") == "1")
                {
                    CommonTool.WriteLog.Write("获取openid，开始获取");
                }
                //微信专用 获取openid
                if (CommonTool.Common.GetAppSetting("useBasePageGetOpenID") == "1")
                {
                    if (CommonTool.Common.GetAppSetting("isTest") == "1")
                    {
                        if (CommonTool.Common.GetAppSetting("useWxMoniter") == "1")
                        {
                            CommonTool.WriteLog.Write("获取openid，使用web.config文件中配置的固定openid");
                        }
                        Session["openid"] = CommonTool.Common.GetAppSetting("openID");
                    }
                    if (Session["openid"] == null)
                    {
                        string askUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={2}?callBackUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect";
                        string p0 = CommonTool.WXParam.APP_ID;
                        string p1 = Request.Url.ToString();
                        string p2 = CommonTool.Common.GetAppSetting("redirectUri");
                        askUrl = string.Format(askUrl, p0, p1, p2);
                        if (CommonTool.Common.GetAppSetting("useWxMoniter") == "1")
                        {
                            CommonTool.WriteLog.Write("获取openid第一步，请求url:" + askUrl);
                        }
                        Response.Redirect(askUrl);
                    }
                    else
                    {
                        if (CommonTool.Common.GetAppSetting("useWxMoniter") == "1")
                        {
                            CommonTool.WriteLog.Write("获取openid成功，根据缓存得到openid:" + Session["openid"].ToString());
                        }
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }

    #endregion 


}