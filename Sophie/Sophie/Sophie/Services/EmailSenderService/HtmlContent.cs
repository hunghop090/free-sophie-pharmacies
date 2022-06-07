using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Extensions.Localization;
using Sophie.Languages;
using Sophie.Units;

namespace Sophie.Services.EmailSenderService
{
    public static class HtmlContent
    {
        // === Basic - Mẫu cơ bản mặc định
        public static string Example_HtmlBasic()
        {
            return $@"
            <style type=""text/css"">
                body {{ font-family: Arial; font-size: 10pt; }}
            </style>
            <html>
                <body>
                    <table width=""100%"">
                        <tr align=""center"" style=""width:100% !important;"">
                            <td align=""right"" valign=""middle"" style=""width:50%"">
                                <img src=cid:e_basic_logo style=""with:80px; height:80px;"">
                            </td>
                            <td align=""left"" valign=""middle"" style=""width:50%"">
                                <h2 style=""font-style:arial; color:#0ac775;"">{Constants.AppDomain}</h2>
                            </td>
                        </tr>
                        <tr align=""left"" style=""width:100% !important;"">
                            <td width=""100%"" colspan=""2"" align=""left"" valign=""top"" text-align=""left"" style=""font-style:arial; color:#2e3e4f; font-weight:normal; width:100% !important;"">
                                Thank you for signing up to use {Constants.AppDomain}. Once you click on the link below, your account will become active. You will be redirected to enter your Username and Password once again - but
                                <strong>please note that your Username is the email address you used for signup and the password is the one you set at signup.</strong>
                                <br><br>
                                <a href='#' target=""_blank"" style=""padding: 8px 12px; border: 1px solid #0ac775;border-radius: 2px;font-family: Arial, sans-serif;font-size: 14px; color: #2e3e4f; background-color: #0ac775; text-decoration: none;font-weight:bold;display: inline-block; cursor: pointer;"">
                                    Confirm email
                                </a>
                            </td>
                        </tr>
                        <tr align=""left"" style=""width:100% !important;"">
                            <td width=""100%"" colspan=""2"" align=""left"" valign=""top"" text-align=""left"" style=""font-style:arial; color:#2e3e4f; font-weight:normal; width:100% !important;"">
                                <br><br>
                                <strong>{Constants.AppDomain},</strong><br>
                                The {Constants.AppDomain} Team<br>
                                <i>{DateTimes.Now():dddd, MMMM d, yyyy h:mm:ss tt}</i>
                            </td>
                        </tr>
                    </table>
                </body>
            </html>
            ";
        }

        public static string HtmlBasic(IStringLocalizer localizer, string htmlContent)
        {
            return $@"
            <style type=""text/css"">
                body {{ font-family: Arial; font-size: 10pt; }}
            </style>
            <html>
                <body>
                    <table width=""100%"">
                        <tr align=""center"" style=""width:100% !important;"">
                            <td align=""right"" valign=""middle"" style=""width:50%"">
                                <img src=cid:e_basic_logo style=""with:80px; height:80px;"">
                            </td>
                            <td align=""left"" valign=""middle"" style=""width:50%"">
                                <h2 style=""font-style:arial; color:#0ac775;"">{Constants.AppDomain}</h2>
                            </td>
                        </tr>
                        <tr align=""left"" style=""width:100% !important;"">
                            <td width=""100%"" colspan=""2"" align=""left"" valign=""top"" text-align=""left"" style=""font-style:arial; color:#2e3e4f; font-weight:normal; width:100% !important;"">
                                {htmlContent}
                            </td>
                        </tr>
                        <tr align=""left"" style=""width:100% !important;"">
                            <td width=""100%"" colspan=""2"" align=""left"" valign=""top"" text-align=""left"" style=""font-style:arial; color:#2e3e4f; font-weight:normal; width:100% !important;"">
                                <br><br>
                                <strong>{Constants.AppDomain},</strong><br>
                                The {Constants.AppDomain} Team<br>
                                <i>{DateTimes.Now():dddd, MMMM d, yyyy h:mm:ss tt}</i>
                            </td>
                        </tr>
                    </table>
                </body>
            </html>
            ";
        }

        public static AlternateView AddImageForHtmlBasic(AlternateView htmlView)
        {
            string rootpath = Directory.GetCurrentDirectory() + @"/wwwroot/img/email";
            LinkedResource logoResource = new LinkedResource(rootpath + @"/logo_500x500.png", new ContentType("image/png")) { ContentId = "e_basic_logo" };
            htmlView.LinkedResources.Add(logoResource);
            return htmlView;
        }
        // === Basic - Mẫu cơ bản mặc định ./





        // === Advance - Mẫu đã được chỉnh sửa
        private static string google_driver_shared = $"https://drive.google.com/uc?export=view&id=";
        public static string email_background = $"{google_driver_shared}1INjLijR-ZqHC-LTTRdP8pe6x-qEPZfyR";
        public static string email_logo = $"{google_driver_shared}1uSlmxPEHRfcImICL0yi2tbjjPvgxK7ck";
        public static string email_banner = $"{google_driver_shared}10Io3EDKFrxWh2gdODnQW3zdf5TY3W_BH";
        public static string email_footer = $"{google_driver_shared}10KUdGyhxHRpsf_n9KNgbvjMZuvuwu5yE";
        public static string email_line = $"{google_driver_shared}1tLoN5oQdncVczg3EKPEsFZh0NZwSBcz_";
        public static string email_facebook_blue = $"{google_driver_shared}1MsRnp6t3YONnkEBOP8fICaFIXce3EXbK";
        public static string email_facebook_gray = $"{google_driver_shared}12bZXse9c5HtV7p2sagQra5fyQu3315vm";
        public static string email_twitter_blue = $"{google_driver_shared}1qrLwFfqRua_t4x78b4aNWiWsZXA2ZSi9";
        public static string email_twitter_gray = $"{google_driver_shared}1Veg8vfPC46vdV-pk5nUaCsvahtf6CsOH";
        public static string email_youtube_blue = $"{google_driver_shared}17-CVhXqrqhbALBgOMbIEgWD0J2zOS3WY";
        public static string email_youtube_gray = $"{google_driver_shared}1MR7JYOwj-p_7hmBP3o82HV-AWaRqutrb";
        public static string email_google_play = $"{google_driver_shared}16enUzE8I4hqFeN6BOhJ7TFCU1vEo99GP";
        public static string email_app_store = $"{google_driver_shared}1B8ZLTtJlVvD4hkzfQ7AcBygC75aD1Xdz";

        public static string Example_HtmlAdvance()
        {
            string text_1 = $"View our web {Constants.AppName.ToLower()}";
            string text_2 = $"if this email isn't displaying well.";
            string text_3 = $"Welcome To {Constants.AppDomain}";
            string text_4 = $"Link Application";
            string text_5 = $"www.payroll.vn";
            string text_6 = $"The {Constants.AppName} Platform:";
            string text_7 = $"Integrates easily with your HCM<br>Automates repeat tasks<br>Standardizes global payroll data<br>Delivers multi-country reporting<br>";
            string text_8 = $"You need to contact us ?";
            string text_9 = $"For more information, you can login to the website";
            string text_10 = $"{Constants.AppName}";
            string text_11 = $"{Constants.AppName}<br>The {Constants.AppName} Team<br>";
            return $@"
            <style type=""text/css"">
                body {{ font-family: Arial; font-size: 10pt; }}
            </style>
            <html>
                <body>
                    <div>
                        <div>
                            <div style=""margin:0 auto;padding:0px"" bgcolor=""#F6F7F9"">
                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" style=""width:100%;"" role=""presentation"">
                                    <tbody>
                                        <tr>
                                            <td>
                                                <p aria-hidden=""true"" style=""padding-top:0;font-size:0px;line-height:0px;color:#ffffff;background-color:#ffffff;display:none"" align=""center""></p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align=""center"" valign=""top"" bgcolor=""#F6F7F9"" background=""{email_background}"" style=""background-image:url('{email_background}');background-size:100% 100%;background-repeat:no-repeat;background-position:top center;padding:0 20px"">
                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" style=""max-width:600px;width:100%;"" role=""presentation"">
                                                    <tbody>
                                                        <tr>
                                                            <td align=""center"" valign=""top"">
                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td align=""center"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:11px;color:#ffffff;line-height:22px;padding-top:12px;padding-bottom:30px"">
                                                                                <a href=""https://{text_5}"" data-saferedirecturl=""https://{text_5}"" style=""text-decoration:underline;color:#000000;font-weight:500"" target=""_blank"">{text_1}</a>
                                                                                <span style=""color:#000000""> {text_2}</span>
                                                                            </td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td align=""center"" valign=""top"" style=""padding-bottom:33px"">
                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"" style=""padding-right:8px"">
                                                                                                <a href=""https://{text_5}"" data-saferedirecturl=""https://{text_5}"" style=""text-decoration:none;color:#ffffff"" target=""_blank"">
                                                                                                    <img src=""{email_logo}"" width=""197"" title=""Firebase"" style=""display:block;height:auto"">
                                                                                                </a>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td align=""center"" valign=""top"" bgcolor=""#ffffff"" style=""border-radius:16px;border:0px solid #e0e0e0"">
                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td align=""center"" valign=""top"" style=""padding:25px 32px 0"">
                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                    <tbody>
                                                                                                        <tr>
                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                                    <tbody>
                                                                                                                        <tr>
                                                                                                                            <td height=""5"" align=""left"" valign=""top"" style=""line-height:5px;display:none"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:26px;color:#212121;line-height:39px;font-weight:500;padding-bottom:15px"">
                                                                                                                                <b style=""color: #09b585"">{text_3}</b>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""5"" align=""left"" valign=""top"" style=""line-height:5px;display:none"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#ffffff"" target=""_blank"">
                                                                                                                                                    <img src=""{email_banner}"" width=""534"" style=""display:block;height:auto;border-radius:10px"">
                                                                                                                                                    <div>
                                                                                                                                                        <img src=""{email_banner}"" width=""534"" style=""display:block;border-radius:10px;height:0px"">
                                                                                                                                                    </div>
                                                                                                                                                </a>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""0"" align=""left"" valign=""top"" style=""line-height:0px;display:none"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>

                                                                                                                        <!-- Content  -->
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding-bottom:6px;padding-top:15px"">
                                                                                                                                Confirm your email address
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""6"" align=""left"" valign=""top"" style=""line-height:6px;display:none"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                                                                                                                                Thank you for signing up to use {Constants.AppDomain}. Once you click on the link below, your account will become active. You will be redirected to enter your Username and Password once again - but
                                                                                                                                <strong>
                                                                                                                                    please note that your Username is the email address you used for signup and the password is the one you set at
                                                                                                                                    <a href=""https://{text_5}/login"" data-saferedirecturl=""https://{text_5}/login"" style=""text-decoration:none;color:#1a73e8"" target=""_blank"">signup</a>.
                                                                                                                                </strong>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"">
                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                                <table cellspacing=""0"" cellpadding=""0"" width=""94"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" target=""_blank"">
                                                                                                                                                                    <img src=""{email_line}"" width=""100%"" height=""6"" border=""0"" alt="""" style=""display:block""></a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <tr>
                                                                                                                                                            <td style=""border-radius:8px;background-color:#1a73e8;color:#ffffff!important;text-align:center;vertical-align:middle"" align=""center"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""display:block;border-radius:8px;color:#ffffff;text-decoration:none;font-family:Roboto,Helvetica Neue,Helvetica,Arial,sans-serif;font-size:14px;line-height:21px;font-weight:500;border-top:8px solid #1a73e8;border-right:16px solid #1a73e8;border-bottom:8px solid #1a73e8;border-left:16px solid #1a73e8;text-align:center;white-space:nowrap;letter-spacing:0.25px"" target=""_blank"">
                                                                                                                                                                    Confirm email
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <tr>
                                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" target=""_blank"">
                                                                                                                                                                    <img src=""{email_line}"" width=""100%"" height=""5"" border=""0"" alt="""" style=""display:block""></a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""20"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <!-- ./ Content  -->

                                                                                                                        <!-- Link Application  -->
                                                                                                                        <tr>
                                                                                                                            <td height=""1"" align=""left"" valign=""top"" bgcolor=""#e0e0e0"" style=""line-height:1px"">
                                                                                                                                <img src=""/assets/temp/email_line.gif"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding:31px 0 11px""> 
                                                                                                                                {text_4}
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""left"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""259"" align=""left"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#ffffff"" target=""_blank""> 
                                                                                                                                                                    <img src=""{email_google_play}"" width=""173"" style=""display:block;height:auto"">
                                                                                                                                                                    <div> <img src=""{email_google_play}"" width=""173"" style=""display:block;height:0px""> </div>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"" style=""display:none"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#ffffff"" target=""_blank""> 
                                                                                                                                                                    <img src=""{email_google_play}"" width=""258"" style=""display:block;height:auto"">
                                                                                                                                                                    <div> <img src=""{email_google_play}"" width=""258"" style=""display:block;height:0px""> </div>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px;padding:12px 0px 2px 0px"">
                                                                                                                                                                <strong> (Android) </strong>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <!--
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#1a73e8;line-height:21px;font-weight:500;padding:30px 0 20px 0px"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#1a73e8"" target=""_blank"">
                                                                                                                                                                    <strong>Download</strong>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        -->
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                            <td width=""8"" height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                                                                                                                                                <img src=""/assets/temp/email_line.gif"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                                            </td>
                                                                                                                                            <td align=""center"" valign=""top"">
                                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""259"" align=""center"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#ffffff"" target=""_blank""> 
                                                                                                                                                                    <img src=""{email_app_store}"" width=""173"" style=""display:block;height:auto"">
                                                                                                                                                                    <div> <img src=""{email_app_store}"" width=""173"" style=""display:block;height:0px""> </div>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"" style=""display:none"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#ffffff"" target=""_blank""> 
                                                                                                                                                                    <img src=""{email_app_store}"" width=""258"" style=""display:block;height:auto"">
                                                                                                                                                                    <div> <img src=""{email_app_store}"" width=""258"" style=""display:block;height:0px""> </div>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px;padding:12px 0px 25px 0px"">
                                                                                                                                                                <strong> (iOS) </strong>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <!--
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#1a73e8;line-height:21px;font-weight:500;padding:7px 0 20px 0px"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#1a73e8"" target=""_blank"">
                                                                                                                                                                    <strong>Download</strong>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        -->
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px;padding:12px 0 18px 0px""> 
                                                                                                                                                Website:
                                                                                                                                                <a href=""http://{text_5}"" data-saferedirecturl=""http://{text_5}"" style=""text-decoration:none;color:#1a73e8"" target=""_blank"">{text_5}</a> 
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <!-- ./ Link Application  -->
                                                                                                                        
                                                                                                                        <!-- The {Constants.AppDomain} Platform  -->
                                                                                                                        <tr>
                                                                                                                            <td height=""1"" align=""left"" valign=""top"" bgcolor=""#e0e0e0"" style=""line-height:1px"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"" style=""padding-top:32px;padding-bottom:32px"">
                                                                                                                                <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                                <table cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""left"" valign=""center"">
                                                                                                                                                                <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"">
                                                                                                                                                                    <tbody>
                                                                                                                                                                        <tr>
                                                                                                                                                                            <td align=""left"" valign=""center"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#212121;line-height:21px;padding:0px 0px 6px 0px;font-weight:500"">
                                                                                                                                                                                {text_6}
                                                                                                                                                                            </td>
                                                                                                                                                                        </tr>
                                                                                                                                                                        <tr>
                                                                                                                                                                            <td align=""left"" valign=""center"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px;padding:0px 0px 6px 0px"">
                                                                                                                                                                                {text_7}
                                                                                                                                                                            </td>
                                                                                                                                                                        </tr>
                                                                                                                                                                    </tbody>
                                                                                                                                                                </table>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <!-- ./ The {Constants.AppDomain} Platform  -->

                                                                                                                        <tr>
                                                                                                                            <td height=""1"" align=""left"" valign=""top"" bgcolor=""#e0e0e0"" style=""line-height:1px"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""30"" align=""left"" valign=""top"" style=""line-height:30px"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding: 0px 0px 14px"">
                                                                                                                                {text_8}
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"" style=""padding:0px 0"">
                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                <a href=""https://www.facebook.com"" data-saferedirecturl=""https://www.facebook.com"" style=""text-decoration:none;color:#ffffff"" target=""_blank"">
                                                                                                                                                    <img src=""{email_facebook_gray}"" width=""30"" title=""Facebook"" style=""display:block;height:auto"">
                                                                                                                                                    <div>
                                                                                                                                                        <img src=""{email_facebook_blue}"" width=""30"" title=""Facebook"" style=""display:block;height:0px"">
                                                                                                                                                    </div>
                                                                                                                                                </a>
                                                                                                                                            </td>
                                                                                                                                            <td width=""16"" height=""1"" align=""left"" valign=""top"" style=""line-height:1px"">
                                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                                            </td>
                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                <a href=""https://twitter.com"" data-saferedirecturl=""https://twitter.com"" style=""text-decoration:none;color:#ffffff"" target=""_blank"">
                                                                                                                                                    <img src=""{email_twitter_gray}"" width=""30"" title=""Twitter"" style=""display:block;height:auto"">
                                                                                                                                                    <div>
                                                                                                                                                        <img src=""{email_twitter_blue}"" width=""30"" title=""Twitter"" style=""display:block;height:0px"">
                                                                                                                                                    </div>
                                                                                                                                                </a>
                                                                                                                                            </td>
                                                                                                                                            <td width=""16"" height=""1"" align=""left"" valign=""top"" style=""line-height:1px"">
                                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                                            </td>
                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                <a href=""https://www.youtube.com"" data-saferedirecturl=""https://www.youtube.com"" style=""text-decoration:none;color:#ffffff"" target=""_blank"">
                                                                                                                                                    <img src=""{email_youtube_gray}"" width=""30"" title=""YouTube"" style=""display:block;height:auto"">
                                                                                                                                                    <div>
                                                                                                                                                        <img src=""{email_youtube_blue}"" width=""30"" title=""YouTube"" style=""display:block;height:0px"">
                                                                                                                                                    </div>
                                                                                                                                                </a>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""30"" align=""left"" valign=""top"" style=""line-height:30px"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                    </tbody>
                                                                                                                </table>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </tbody>
                                                                                                </table>
                                                                                            </td>
                                                                                        </tr>

                                                                                        <tr>
                                                                                            <td align=""left"" valign=""top"" bgcolor=""#f2f4f5"" style=""padding:30px 32px 24px;border-bottom-left-radius:16px;border-bottom-right-radius:16px"">
                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                    <tbody>
                                                                                                        <tr>
                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                                    <tbody>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:12px;color:#606162;line-height:18px;padding:0 0 12px"">
                                                                                                                                {text_9}
                                                                                                                                <a href=""https://{text_5}"" data-saferedirecturl=""https://{text_5}"" style=""text-decoration:none;color:#1967d2;font-weight:500"" target=""_blank"">
                                                                                                                                    {text_5}
                                                                                                                                </a>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"">
                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""left"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                                                <table cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"">
                                                                                                                                                                    <tbody>
                                                                                                                                                                        <tr>
                                                                                                                                                                            <td aria-hidden=""true"" align=""left"" valign=""top"" style=""padding-top:21px"">
                                                                                                                                                                                <h3 width=""118"" title=""{text_10}"" style=""display:block;height:auto"">
                                                                                                                                                                                    <b style=""color: #5F6368"">{text_10}</b>
                                                                                                                                                                                </h3>
                                                                                                                                                                            </td>
                                                                                                                                                                        </tr>
                                                                                                                                                                    </tbody>
                                                                                                                                                                </table>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                            <td align=""right"" valign=""top"">
                                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""right"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""right"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:12px;color:#606162;line-height:18px;padding:0 0 0"">
                                                                                                                                                                <a href=""#m_6009720710001840022_"" style=""text-decoration:none;color:#606162"">
                                                                                                                                                                    {text_11}
                                                                                                                                                                    <i>{DateTimes.Now():dddd, MMMM d, yyyy h:mm:ss tt}</i>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                    </tbody>
                                                                                                                </table>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </tbody>
                                                                                                </table>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td width=""16"" height=""137"" align=""left"" valign=""top"" style=""line-height:137px"">
                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                            </td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <img height=""1"" width=""3"" src=""{email_footer}"">
                            </div>
                        </div>
                    </div>
                </body>
            </html>
            ";
        }

        public static string HtmlAdvance(IStringLocalizer _localizer, string htmlContent)
        {
            string text_1 = _localizer["EMAIL_TEXT_1", EnumLanguage.VI].Value ?? $"View our web {Constants.AppName.ToLower()}";
            string text_2 = _localizer["EMAIL_TEXT_2", EnumLanguage.VI].Value ?? $"if this email isn't displaying well.";
            string text_3 = _localizer["EMAIL_TEXT_3", EnumLanguage.VI].Value ?? $"Welcome To {Constants.AppDomain}";
            string text_4 = _localizer["EMAIL_TEXT_4", EnumLanguage.VI].Value ?? $"Link Application";
            string text_5 = $"wwww.payroll.vn";
            string text_6 = _localizer["EMAIL_TEXT_6", EnumLanguage.VI].Value ?? $"The {Constants.AppName} Platform:";
            string text_7 = _localizer["EMAIL_TEXT_7", EnumLanguage.VI].Value ?? $"Integrates easily with your HCM<br>Automates repeat tasks<br>Standardizes global payroll data<br>Delivers multi-country reporting<br>";
            string text_8 = _localizer["EMAIL_TEXT_8", EnumLanguage.VI].Value ?? $"You need to contact us ?";
            string text_9 = _localizer["EMAIL_TEXT_9", EnumLanguage.VI].Value ?? $"For more information, you can login to the website";
            string text_10 = $"{Constants.AppDomain}";
            string text_11 = _localizer["EMAIL_TEXT_11", EnumLanguage.VI].Value ?? $"{Constants.AppName}<br>The {Constants.AppName} Team<br>";
            return $@"
            <style type=""text/css"">
                body {{ font-family: Arial; font-size: 10pt; }}
            </style>
            <html>
                <body>
                    <div>
                        <div>
                            <div style=""margin:0 auto;padding:0px"" bgcolor=""#F6F7F9"">
                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" style=""width:100%;"" role=""presentation"">
                                    <tbody>
                                        <tr>
                                            <td>
                                                <p aria-hidden=""true"" style=""padding-top:0;font-size:0px;line-height:0px;color:#ffffff;background-color:#ffffff;display:none"" align=""center""></p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align=""center"" valign=""top"" bgcolor=""#F6F7F9"" background=""{email_background}"" style=""background-image:url('{email_background}');background-size:100% 100%;background-repeat:no-repeat;background-position:top center;padding:0 20px"">
                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" style=""max-width:600px;width:100%;"" role=""presentation"">
                                                    <tbody>
                                                        <tr>
                                                            <td align=""center"" valign=""top"">
                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td align=""center"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:11px;color:#ffffff;line-height:22px;padding-top:12px;padding-bottom:30px"">
                                                                                <a href=""https://{text_5}"" data-saferedirecturl=""https://{text_5}"" style=""text-decoration:underline;color:#000000;font-weight:500"" target=""_blank"">{text_1}</a>
                                                                                <span style=""color:#000000""> {text_2}</span>
                                                                            </td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td align=""center"" valign=""top"" style=""padding-bottom:33px"">
                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"" style=""padding-right:8px"">
                                                                                                <a href=""https://{text_5}"" data-saferedirecturl=""https://{text_5}"" style=""text-decoration:none;color:#ffffff"" target=""_blank"">
                                                                                                    <img src=""{email_logo}"" width=""197"" title=""Firebase"" style=""display:block;height:auto"">
                                                                                                </a>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td align=""center"" valign=""top"" bgcolor=""#ffffff"" style=""border-radius:16px;border:0px solid #e0e0e0"">
                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td align=""center"" valign=""top"" style=""padding:25px 32px 0"">
                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                    <tbody>
                                                                                                        <tr>
                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                                    <tbody>
                                                                                                                        <tr>
                                                                                                                            <td height=""5"" align=""left"" valign=""top"" style=""line-height:5px;display:none"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:26px;color:#212121;line-height:39px;font-weight:500;padding-bottom:15px"">
                                                                                                                                <b style=""color: #09b585"">{text_3}</b>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""5"" align=""left"" valign=""top"" style=""line-height:5px;display:none"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#ffffff"" target=""_blank"">
                                                                                                                                                    <img src=""{email_banner}"" width=""534"" style=""display:block;height:auto;border-radius:10px"">
                                                                                                                                                    <div>
                                                                                                                                                        <img src=""{email_banner}"" width=""534"" style=""display:block;border-radius:10px;height:0px"">
                                                                                                                                                    </div>
                                                                                                                                                </a>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""0"" align=""left"" valign=""top"" style=""line-height:0px;display:none"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>

                                                                                                                        <!-- Content  -->
                                                                                                                        {htmlContent}
                                                                                                                        <!-- ./ Content  -->

                                                                                                                        <!-- Link Application  -->
                                                                                                                        <tr>
                                                                                                                            <td height=""1"" align=""left"" valign=""top"" bgcolor=""#e0e0e0"" style=""line-height:1px"">
                                                                                                                                <img src=""/assets/temp/email_line.gif"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding:31px 0 11px""> 
                                                                                                                                {text_4} 
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""left"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""259"" align=""left"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#ffffff"" target=""_blank""> 
                                                                                                                                                                    <img src=""{email_google_play}"" width=""173"" style=""display:block;height:auto"">
                                                                                                                                                                    <div> <img src=""{email_google_play}"" width=""173"" style=""display:block;height:0px""> </div>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"" style=""display:none"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#ffffff"" target=""_blank""> 
                                                                                                                                                                    <img src=""{email_google_play}"" width=""258"" style=""display:block;height:auto"">
                                                                                                                                                                    <div> <img src=""{email_google_play}"" width=""258"" style=""display:block;height:0px""> </div>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px;padding:12px 0px 2px 0px"">
                                                                                                                                                                <strong> (Android) </strong>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <!--
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#1a73e8;line-height:21px;font-weight:500;padding:30px 0 20px 0px"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#1a73e8"" target=""_blank"">
                                                                                                                                                                    <strong>Download</strong>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        -->
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                            <td width=""8"" height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                                                                                                                                                <img src=""/assets/temp/email_line.gif"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                                            </td>
                                                                                                                                            <td align=""center"" valign=""top"">
                                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""259"" align=""center"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#ffffff"" target=""_blank""> 
                                                                                                                                                                    <img src=""{email_app_store}"" width=""173"" style=""display:block;height:auto"">
                                                                                                                                                                    <div> <img src=""{email_app_store}"" width=""173"" style=""display:block;height:0px""> </div>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"" style=""display:none"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#ffffff"" target=""_blank""> 
                                                                                                                                                                    <img src=""{email_app_store}"" width=""258"" style=""display:block;height:auto"">
                                                                                                                                                                    <div> <img src=""{email_app_store}"" width=""258"" style=""display:block;height:0px""> </div>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px;padding:12px 0px 25px 0px"">
                                                                                                                                                                <strong> (iOS) </strong>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <!--
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#1a73e8;line-height:21px;font-weight:500;padding:7px 0 20px 0px"">
                                                                                                                                                                <a href=""#"" data-saferedirecturl=""#"" style=""text-decoration:none;color:#1a73e8"" target=""_blank"">
                                                                                                                                                                    <strong>Download</strong>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        -->
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px;padding:12px 0 18px 0px""> 
                                                                                                                                                Website:
                                                                                                                                                <a href=""http://{text_5}"" data-saferedirecturl=""http://{text_5}"" style=""text-decoration:none;color:#1a73e8"" target=""_blank"">{text_5}</a> 
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <!-- ./ Link Application  -->
                                                                                                                        
                                                                                                                        <!-- The {Constants.AppDomain} Platform  -->
                                                                                                                        <tr>
                                                                                                                            <td height=""1"" align=""left"" valign=""top"" bgcolor=""#e0e0e0"" style=""line-height:1px"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"" style=""padding-top:32px;padding-bottom:32px"">
                                                                                                                                <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                                <table cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""left"" valign=""center"">
                                                                                                                                                                <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"">
                                                                                                                                                                    <tbody>
                                                                                                                                                                        <tr>
                                                                                                                                                                            <td align=""left"" valign=""center"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#212121;line-height:21px;padding:0px 0px 6px 0px;font-weight:500"">
                                                                                                                                                                                {text_6}
                                                                                                                                                                            </td>
                                                                                                                                                                        </tr>
                                                                                                                                                                        <tr>
                                                                                                                                                                            <td align=""left"" valign=""center"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px;padding:0px 0px 6px 0px"">
                                                                                                                                                                                {text_7}
                                                                                                                                                                            </td>
                                                                                                                                                                        </tr>
                                                                                                                                                                    </tbody>
                                                                                                                                                                </table>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <!-- ./ The {Constants.AppDomain} Platform  -->

                                                                                                                        <tr>
                                                                                                                            <td height=""1"" align=""left"" valign=""top"" bgcolor=""#e0e0e0"" style=""line-height:1px"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""30"" align=""left"" valign=""top"" style=""line-height:30px"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding: 0px 0px 14px"">
                                                                                                                                {text_8}
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"" style=""padding:0px 0"">
                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                <a href=""https://www.facebook.com"" data-saferedirecturl=""https://www.facebook.com"" style=""text-decoration:none;color:#ffffff"" target=""_blank"">
                                                                                                                                                    <img src=""{email_facebook_gray}"" width=""30"" title=""Facebook"" style=""display:block;height:auto"">
                                                                                                                                                    <div>
                                                                                                                                                        <img src=""{email_facebook_blue}"" width=""30"" title=""Facebook"" style=""display:block;height:0px"">
                                                                                                                                                    </div>
                                                                                                                                                </a>
                                                                                                                                            </td>
                                                                                                                                            <td width=""16"" height=""1"" align=""left"" valign=""top"" style=""line-height:1px"">
                                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                                            </td>
                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                <a href=""https://twitter.com"" data-saferedirecturl=""https://twitter.com"" style=""text-decoration:none;color:#ffffff"" target=""_blank"">
                                                                                                                                                    <img src=""{email_twitter_gray}"" width=""30"" title=""Twitter"" style=""display:block;height:auto"">
                                                                                                                                                    <div>
                                                                                                                                                        <img src=""{email_twitter_blue}"" width=""30"" title=""Twitter"" style=""display:block;height:0px"">
                                                                                                                                                    </div>
                                                                                                                                                </a>
                                                                                                                                            </td>
                                                                                                                                            <td width=""16"" height=""1"" align=""left"" valign=""top"" style=""line-height:1px"">
                                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                                            </td>
                                                                                                                                            <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                                                                                                                <a href=""https://www.youtube.com"" data-saferedirecturl=""https://www.youtube.com"" style=""text-decoration:none;color:#ffffff"" target=""_blank"">
                                                                                                                                                    <img src=""{email_youtube_gray}"" width=""30"" title=""YouTube"" style=""display:block;height:auto"">
                                                                                                                                                    <div>
                                                                                                                                                        <img src=""{email_youtube_blue}"" width=""30"" title=""YouTube"" style=""display:block;height:0px"">
                                                                                                                                                    </div>
                                                                                                                                                </a>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height=""30"" align=""left"" valign=""top"" style=""line-height:30px"">
                                                                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                    </tbody>
                                                                                                                </table>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </tbody>
                                                                                                </table>
                                                                                            </td>
                                                                                        </tr>

                                                                                        <tr>
                                                                                            <td align=""left"" valign=""top"" bgcolor=""#f2f4f5"" style=""padding:30px 32px 24px;border-bottom-left-radius:16px;border-bottom-right-radius:16px"">
                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                    <tbody>
                                                                                                        <tr>
                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                                    <tbody>
                                                                                                                        <tr>
                                                                                                                            <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:12px;color:#606162;line-height:18px;padding:0 0 12px"">
                                                                                                                                {text_9}
                                                                                                                                <a href=""https://{text_5}"" data-saferedirecturl=""https://{text_5}"" style=""text-decoration:none;color:#1967d2;font-weight:500"" target=""_blank"">
                                                                                                                                    {text_5}
                                                                                                                                </a>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align=""center"" valign=""top"">
                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""center"" role=""presentation"">
                                                                                                                                    <tbody>
                                                                                                                                        <tr>
                                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""left"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""left"" valign=""top"">
                                                                                                                                                                <table cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"">
                                                                                                                                                                    <tbody>
                                                                                                                                                                        <tr>
                                                                                                                                                                            <td aria-hidden=""true"" align=""left"" valign=""top"" style=""padding-top:21px"">
                                                                                                                                                                                <h3 width=""118"" title=""{text_10}"" style=""display:block;height:auto"">
                                                                                                                                                                                    <b style=""color: #5F6368"">{text_10}</b>
                                                                                                                                                                                </h3>
                                                                                                                                                                            </td>
                                                                                                                                                                        </tr>
                                                                                                                                                                    </tbody>
                                                                                                                                                                </table>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                            <td align=""right"" valign=""top"">
                                                                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" align=""right"" role=""presentation"">
                                                                                                                                                    <tbody>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align=""right"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:12px;color:#606162;line-height:18px;padding:0 0 0"">
                                                                                                                                                                <a href=""#m_6009720710001840022_"" style=""text-decoration:none;color:#606162"">
                                                                                                                                                                    {text_11}
                                                                                                                                                                    <i>{DateTimes.Now():dddd, MMMM d, yyyy h:mm:ss tt}</i>
                                                                                                                                                                </a>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                    </tbody>
                                                                                                                                                </table>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </tbody>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                    </tbody>
                                                                                                                </table>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </tbody>
                                                                                                </table>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td width=""16"" height=""137"" align=""left"" valign=""top"" style=""line-height:137px"">
                                                                                <img src=""{email_line}"" width=""1"" height=""1"" border=""0"" alt="""" style=""display:block"">
                                                                            </td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <img height=""1"" width=""3"" src=""{email_footer}"">
                            </div>
                        </div>
                    </div>
                </body>
            </html>
            ";
        }

        public static AlternateView AddImageForHtmlAdvance(AlternateView htmlView)
        {
            //string rootpath = Directory.GetCurrentDirectory() + @"/wwwroot/img/email";
            //LinkedResource resource1 = new LinkedResource(rootpath + @"/logo_500x500.png", new ContentType("image/png")) { ContentId = "e_advance_logo" };
            //htmlView.LinkedResources.Add(resource1);
            return htmlView;
        }
        // === Advance - Mẫu đã được chỉnh sửa ./
    }
}
