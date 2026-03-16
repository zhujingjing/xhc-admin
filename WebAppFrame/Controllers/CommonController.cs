
using Com.Alipay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace WebAppFrame.Controllers
{
    public class CommonController : Controller
    {
        #region 微信相关

        //微信支付参数配置
        public string WxPayFor(string data)
        {
            return CommonTool.WXOperate.WxPayFor(data);
        }

        //微信分享参数配置
        public string WxConfig(string data)
        {
            return CommonTool.WXOperate.WxConfig(data);
        }

        public string Notify()
        {
            string strRtn = string.Empty;

            string resultFromWx = GetPostStr();

            if (CommonTool.Common.GetAppSetting("useWxMoniter") == "1")
            {
                CommonTool.WriteLog.Write("微信支付, 支付回调成功, 微信回传参数为: " + resultFromWx);
            }

            //订单编号
            string out_trade_no = CommonTool.XmlOper.GetTextByTag(resultFromWx, "out_trade_no");
            //支付状态
            string trade_status = CommonTool.XmlOper.GetTextByTag(resultFromWx, "result_code");
            //实际支付金额
            string price = CommonTool.XmlOper.GetTextByTag(resultFromWx, "total_fee");


            strRtn = Deal_WxOrder_Member(out_trade_no, trade_status, price);

            return strRtn;
        }

        public string Deal_WxOrder_Member(string out_trade_no, string trade_status, string price)
        {
            string strRtn = string.Empty;

            //订单支付时间
            string real_time = DateTime.Now.ToString();
            int intPrice = Convert.ToInt32(price);
            price = Math.Round((intPrice / 100.0), 2).ToString();

            //订单状态
            BLL.OrderState order_status = BLL.OrderState.付款成功;
            if (trade_status.Trim() == "SUCCESS")
            {
                order_status = BLL.OrderState.付款成功;
            }
            else
            {
                order_status = BLL.OrderState.付款失败;
            }
           
            double real_paymoney = Math.Round((intPrice / 100.0), 2);

            string strSql = string.Empty;
            int exetag = 0;

            //更新订单状态之前做检验，防止重复更新，防止重复插入奖励金额
            strSql = "select 1 from dbo.[order] where charge_no = '{0}' and state = '{1}'";
            strSql = string.Format(strSql, out_trade_no, order_status);
            if (DBHelper.SqlHelper.GetDataItemString(strSql) == "1")
            {
                if (CommonTool.Common.GetAppSetting("useWxMoniter") == "1")
                {
                    CommonTool.WriteLog.Write("微信支付, 支付回调成功, 重复回调返回动作");
                }
                return "";
            }

            //更新订单状态
            strSql = @"update dbo.[order] set state = '{0}',real_time = '{1}', real_money = '{3}', comment = '{4}' where charge_no ='{2}' ;";
            strSql = string.Format(strSql, order_status, real_time, out_trade_no, price, "");
            exetag = DBHelper.SqlHelper.ExecuteSql(strSql);

            if (exetag > 0)
            {
                strRtn = "SUCCESS";
            }

            return strRtn;
        }

        public string GetPostStr()
        {
            Int32 intLen = Convert.ToInt32(Request.InputStream.Length);
            byte[] b = new byte[intLen];
            Request.InputStream.Read(b, 0, intLen);
            return System.Text.Encoding.UTF8.GetString(b);
        }

        public string Token = "www0430com";
        public string URLRedirect(string parms)
        {
            string resultFromWx = GetPostStr();
            if (!string.IsNullOrEmpty(resultFromWx))
            {
                //消息处理
                string FromUserName = CommonTool.XmlOper.GetTextByTag(resultFromWx, "FromUserName");
                string MsgType = CommonTool.XmlOper.GetTextByTag(resultFromWx, "MsgType");
                string Event = CommonTool.XmlOper.GetTextByTag(resultFromWx, "Event");
                string EventKey = CommonTool.XmlOper.GetTextByTag(resultFromWx, "EventKey");
                string Ticket = CommonTool.XmlOper.GetTextByTag(resultFromWx, "Ticket");

                //记录事件具体内容
                if (Event == "subscribe")
                {
                    //建立分站和微信用户的关系
                    CommonTool.WriteLog.Write("有关注:" + EventKey + "," + FromUserName);

                    //给用户发送消息-2022.10.19新增备注
                    //string msg_txt = BCL.ZCZBConfig.GetConfig("wx_msg1");
                    //CommonTool.WXOperate.SendMessage_Txt(FromUserName, msg_txt);
                }

                //记录微信事件
                CommonTool.WriteLog.Write("微信发来的通知:" + MsgType + "," + Event + "," + FromUserName);
            }

            //记录具体事件内容
            //string wxevent_path = CommonTool.PathHelper.GetCurrentBasePath() + @"ConfigFile/wxevent.txt";
            //if (System.IO.File.Exists(wxevent_path))
            //{
            //    CommonTool.FileOper.CreateTxt(wxevent_path, DateTime.Now.ToString() + ":" + resultFromWx + "\r\n");
            //}

            if (string.IsNullOrEmpty(Request.QueryString["echoStr"])) { Response.End(); }
            string echoStr = Request.QueryString["echoStr"].ToString();
            if (CheckSignature())
            {
                if (!string.IsNullOrEmpty(echoStr))
                {
                    return echoStr;
                }
            }
            return string.Empty;
        }

        private bool CheckSignature()
        {
            return true;
        }

        #endregion 

        #region 小程序相关 2019.4.12

        public string XCX_Login(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            if(string.IsNullOrEmpty(id))
            {
                info.State = "0";
                info.Msg = "参数错误,code为空";
                return info.ToString();
            }

            string code = id.Trim();
            string openid = CommonTool.WxOperate2.GetOpenID(code);

            if (!string.IsNullOrEmpty(openid))
            {
                info.State = "1";
                info.Msg = openid;
                CommonTool.WriteLog.Write("获取openid成功：" + openid);
                //保存数据
               
                
            }
            else
            {
                info.State = "0";
                info.Msg = "";
            }

            return info.ToString();

        }

        public string XCX_Login2(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            if (string.IsNullOrEmpty(id))
            {
                info.State = "0";
                info.Msg = "参数错误,code为空";
                return info.ToString();
            }

            string code = id.Trim();
            Dictionary<string, string> dic = CommonTool.WxOperate2.GetOpenID2SessionKey(code);

            if (dic.ContainsKey("openid"))
            {
                string openid = dic["openid"];
                string session_key = dic.ContainsKey("session_key") ? dic["session_key"] : "";
                info.State = "1";
                info.Msg = "成功";
                info.DicParm.Add("openid", openid);
                info.DicParm.Add("session_key", session_key);

                CommonTool.WriteLog.Write("获取成功：openid：" + openid + "; session_key:" + session_key);
     
             
            }
            else
            {
                info.State = "0";
                info.Msg = "";
            }

            return info.ToString();

        }

        public string XCX_GetWxBindTel(string encryptedData, string iv, string session_key)
        {
            string strRtn = AES_decrypt(encryptedData, session_key, iv);
            return strRtn;
        }

        public string AES_decrypt(string encryptedDataStr, string key, string iv)
        {
            RijndaelManaged rijalg = new RijndaelManaged();
            //-----------------    
            //设置 cipher 格式 AES-128-CBC    

            rijalg.KeySize = 128;

            rijalg.Padding = PaddingMode.PKCS7;
            rijalg.Mode = CipherMode.CBC;

            rijalg.Key = Convert.FromBase64String(key);
            rijalg.IV = Convert.FromBase64String(iv);


            byte[] encryptedData = Convert.FromBase64String(encryptedDataStr);
            //解密    
            ICryptoTransform decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);

            string result;

            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        result = srDecrypt.ReadToEnd();
                    }
                }
            }

            return result;
        }

        #endregion 

        #region 支付宝支付

        public string ZhifubaoPay(string SendData)
        {
            string strRtn = string.Empty;

            strRtn = Zhifb_pay();

            return strRtn;
        }

        private string Zhifb_pay()
        {
            ////////////////////////////////////////////请求参数////////////////////////////////////////////


            //商户订单号，商户网站订单系统中唯一订单号，必填
            string out_trade_no = CommonTool.Common.CreateOrderNo("fys");

            //订单名称，必填
            string subject = "fuyoushie";

            //付款金额，必填
            string total_fee = "0.01";

            //收银台页面上，商品展示的超链接，必填
            string show_url = "http://zhuochuangzb.com";

            //商品描述，可空
            string body = "zanwu";



            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.partner);
            sParaTemp.Add("seller_id", Config.seller_id);
            sParaTemp.Add("_input_charset", Config.input_charset.ToLower());
            sParaTemp.Add("service", Config.service);
            sParaTemp.Add("payment_type", Config.payment_type);
            sParaTemp.Add("notify_url", Config.notify_url);
            sParaTemp.Add("return_url", Config.return_url);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("show_url", show_url);
            //sParaTemp.Add("app_pay","Y");//启用此参数可唤起钱包APP支付。
            sParaTemp.Add("body", body);
            //其他业务参数根据在线开发文档，添加参数.文档地址:https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.2Z6TSk&treeId=60&articleId=103693&docType=1
            //如sParaTemp.Add("参数名","参数值");

            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
            return sHtmlText;

        }

        public static string Alipay()
        {
            //string OrderNumber = "fys" + DateTime.Now.ToString("yyyyMMddHHmmss");
            //string app_id = "2017081008125449";
            ////应用商户私钥
            //string merchant_private_key = "";
            //merchant_private_key = "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBANFpLMNfJoflpM+L7z7Zjv9lTKaL0K2ONaW0IaZhXjDzsECdYeF8lCT8PMr1z8g26WkW7N8ZcE6QQm8GEJ20nNxXLSk6tRONW+mQuuqe4woQEn1iLKtL+8t5btNo7gKLIAeYsojWIOF92cU35COv9nT+qCeoxOR5BeziDJbnz6IpAgMBAAECgYEAlyT4YBMfNlrzt7FiftR8N64fwt2LQpkv82euGe6RE3fvsNNnQF6wdnlwT3VYAURQ977KPMSZoLrSmFuiRpGD3N4JwMh8QfB8WhPQXmo0veFcKB10OA3mMxMGNrT85wXPn9tOMGqaJSV8ow704382wH7V7F++DssVRbUWwMBJgAECQQD5mrTVYKhVq6sQFVHWLAlAsYFn3zYKzmFF5eVqZyONsaYslkIFYJY6zM/CzM40lJnL/y+djxnDISZ37C1499gBAkEA1sbRKugZnyd1Fl+u1F25zbu/NqqlI77R8YBIXmMLGBvnKNEl9cm+qMbAP5wWsOUklljwpxtYgqx/F8OICa4KKQJBAN6hvf/AmgTn/Ml4qqjkWFBqwyi58EMNN7gXyVvxqMWemcCVOUgAkZ7axclT0e0WSmOpNnhLhkgEjoJSl3CZaAECQQC+1xCa2HuTwKK4i7K0PsfPd+jS8VQla/Pua/dh7w23kWuYCCd7u5SvMVM38kDzU9hrw4GqapD+I7oWaoahW0TpAkAEbWXvPzztWJt+wWZfVUkSMaSprX7QqaWydosaJ38RMeQfufAqRBixbuVf+nX1+StSwbITkCAOgwc9U9PhUkGJ";

            ////支付宝公钥
            //string alipay_public_key = "";
            //alipay_public_key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmdliSAGovUcEQCqExk8P8MNUKvTG2uWahDh9ZFtO5052VhBefCvuG7wRhjnFk2E8Rk9etH5ZnLpz+RWzcdIk017RQ/Cb6MVpHhbfjQCT4AYXnUI8703S7tlKsJzBMgNYzOQ2iMVuTQNTm84Zrrgkhoc6DKFhBoRPs08MXOCjv8c1c+7yAqOMpa2g6QrcSM5bjsxcQkGLnU9lAuBQA2UhlE//kyGetXIbgvwsPLzR1sAiNXS0DUxx4z/R0VY/nisEzAeWWarMKD+TNpSc+vqE13Pf8KNk96d/UtKYziLw3D+V7OKqZxi7A+YFJXSYM/j9lq04kXhWR5VTj1znO2QXeQIDAQAB";
            

            //string timeout_express = "30m";//订单有效时间（分钟）
            //string postUrl = "https://openapi.alipay.com/gateway.do";
            //string sign_type = "RSA";//加签方式 有两种RSA和RSA2 我这里使用的RSA2（支付宝推荐的）
            //string version = "1.0";//固定值 不用改
            //string format = "json";//固定值
            //string Amount = "0.01";//订单金额
            //string method = "alipay.trade.wap.pay";//调用接口 固定值 不用改
            //IAopClient client = new DefaultAopClient(postUrl, app_id, merchant_private_key, format, version, sign_type, alipay_public_key, "UTF-8", false);
            //AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            //request.SetNotifyUrl("http://zhuochuangzb.com/Common/Notify_ZFB");
            //request.SetReturnUrl("http://zhuochuangzb.com");
            //request.BizContent = "{" +
            //"    \"body\":\"福友社会员加入申请\"," +
            //"    \"subject\":\"福友社会员支付\"," +
            //"    \"out_trade_no\":\"" + OrderNumber + "\"," +
            //"    \"timeout_express\":\"" + timeout_express + "\"," +
            //"    \"total_amount\":" + Amount + "," +
            //"    \"product_code\":\"" + method + "\"" +
            //"  }";
            //AlipayTradeWapPayResponse response = client.pageExecute(request);
            //string form = response.Body.Substring(0, response.Body.IndexOf("<script>"));
            //return form;

            return string.Empty;
        }


        public string Notify_ZFB()
        {
            if (CommonTool.Common.GetAppSetting("useWxMoniter") == "1")
            {
                CommonTool.WriteLog.Write("支付宝支付, 支付完成, 通知函数启动！ ");
            }

            string strRtn = string.Empty;

            //订单编号
            string out_trade_no = Request.Form["out_trade_no"];
            //支付状态
            string trade_status = Request.Form["trade_status"];
            //实际支付金额
            string price = Request.Form["price"];

            BLL.OrderState order_status = BLL.OrderState.待付款;
            if (trade_status.Trim() == "TRADE_SUCCESS")
            {
                order_status = BLL.OrderState.付款成功;
            }
            else
            {
                order_status = BLL.OrderState.付款失败;
            }

            #region 更新状态
            CommonTool.WriteLog.Write(out_trade_no);
//            string strSql = @"update dbo.salary 
//                                            set state = '{0}', money = '{1}'
//                                            where salary_no = '{2}';";
//            strSql = string.Format(strSql, order_status, price, out_trade_no);

//            if (DBHelper.SqlHelper.ExecuteSql(strSql) > 0)
//            {
//                strRtn = "SUCCESS";
//            }
            #endregion

            return strRtn;
        }

        #endregion

        #region 短信验证码

        static Dictionary<string, string> DicMsgCode = new Dictionary<string, string>();

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public string SendVerifyCode(string phone)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            //判断电话号码
            if (!CommonTool.Common.IsTelphone(phone.Trim()))
            {
                info.Msg = "电话号码错误,请输入正确的电话号码";
                info.State = "0";
                return info.ToString();
            }

            Random rnd = new Random();
            int rndcode = rnd.Next(1000, 9999);
            if (CommonTool.Common.GetAppSetting("checkCodeLength") == "6")
            {
                rndcode = rnd.Next(100000, 999999);
            }
            string strContent = "您的验证码为:" + rndcode.ToString();

            External.SMS.SendMsg(phone, strContent, CommonTool.Common.GetAppSetting("appName"));

            if (DicMsgCode.Keys.Contains(phone))
            {
                DicMsgCode[phone] = rndcode.ToString();
            }
            else
            {
                DicMsgCode.Add(phone, rndcode.ToString());
            }

            //加session
            Session["msgcode"] = rndcode;
            Session.Timeout = 3;

            info.State = "1";
            info.Msg = "发送成功";

            return info.ToString();
        }

        /// <summary>
        /// 检查验证码的正确性
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public string CheckMsgCode(string Code)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            if (Session["msgcode"] == null || string.IsNullOrEmpty(Code))
            {
                info.Msg = "验证码错误";
                info.State = "0";

                return info.ToString();
            }

            if (Session["msgcode"].ToString() == Code)
            {
                info.Msg = "验证码输入正确";
                info.State = "1";
            }
            else
            {
                info.Msg = "验证码错误";
                info.State = "0";
            }


            return info.ToString();
        }


        public string CheckMsgCode_v2(string code, string phone)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(phone))
            {
                info.State = "0";
                info.Msg = "参数错误";
                return info.ToString();
            }

            if (DicMsgCode.Keys.Contains(phone) && DicMsgCode[phone] == code)
            {
                info.State = "1";
                info.Msg = "验证码输入正确";
            }
            else
            {
                info.State = "0";
                info.Msg = "验证码输入错误";
            }

            return info.ToString();
        }

        #endregion 

        #region 图片上传
        public string UpLoadImage()
        {
            var file = Request.Files[0];
            string file_name = Guid.NewGuid().ToString() + ".jpg";
            string strFileName = "/UpLoadFile/" + file_name;
            string path = Server.MapPath(strFileName);
            file.SaveAs(path);

            return strFileName;
        }

        #endregion 

        #region web工具部分
        public string Test()
        {
            string strRtn = "";

            strRtn = "<a>下载</a>";

            return strRtn;
        }

        //数据库备份
        public string DBBackUp()
        {
            return "";
        }

        #endregion 

    }
}
