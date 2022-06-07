using System;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Localization;
using Sophie.Languages;

namespace Sophie.Services.EmailSenderService
{
    public static class EmailStyles
    {
        // Mẫu đăng ký tài khoản
        public static string ConfirmRegistryAccount(IStringLocalizer _localizer, string subject, string confirmLink)
        {
            string basic = _localizer["EMAIL_TEXT_12", EnumLanguage.VI].Value ?? $@"Thank you for signing up to use Buffalo. Once you click on the link below, your account will become active. You will be redirected to enter your Username and Password once again.<br><strong>Please note that your Username is the email address you used for signup and the password is the one you set at signup.</strong>";
            string button = _localizer["EMAIL_TEXT_13", EnumLanguage.VI].Value ?? $@"Confirm email";

            return $@"
            <tr>
                <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding-bottom:6px;padding-top:15px"">
                    {subject}
                </td>
            </tr>
            <tr>
                <td height="" 6"" align=""left"" valign=""top"" style=""line-height:6px;display:none"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                    {basic}
                </td>
            </tr>
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""center"" valign=""top"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                        <tbody>
                            <tr>
                                <td align=""left"" valign=""top"">
                                    <table cellspacing=""0"" cellpadding=""0"" width="" 94"" role=""presentation"">
                                        <tbody>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""#"" data-saferedirecturl="" #"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height="" 6""border=""0"" alt="""" style=""display:block"">
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""border-radius:8px;background-color:#1a73e8;color:#ffffff!important;text-align:center;vertical-align:middle"" align=""center"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLink)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLink)}"" style=""display:block;border-radius:8px;color:#ffffff;text-decoration:none;font-family:Roboto,Helvetica Neue,Helvetica,Arial,sans-serif;font-size:14px;line-height:21px;font-weight:500;border-top:8px solid #1a73e8;border-right:16px solid #1a73e8;border-bottom:8px solid #1a73e8;border-left:16px solid #1a73e8;text-align:center;white-space:nowrap;letter-spacing:0.25px"" target=""_blank"">
                                                        {button}
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLink)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLink)}"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height=""5""border=""0"" alt="""" style=""display:block"">
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
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""20""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            ";
        }

        // Mẫu khôi phục mật khẩu
        public static string ConfirmPasswordRecovery(IStringLocalizer _localizer, string subject, string confirmLink)
        {
            string basic = _localizer["EMAIL_TEXT_14", EnumLanguage.VI].Value ?? $@"Thank you for use Buffalo. Once you click on the link below, you can recover your password. You will be redirected to enter your new password once again.<br><strong>Please note that you should not give out a password to anyone</strong>";
            string button = _localizer["EMAIL_TEXT_15", EnumLanguage.VI].Value ?? $@"Recovery Password";

            return $@"
            <tr>
                <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding-bottom:6px;padding-top:15px"">
                    {subject}
                </td>
            </tr>
            <tr>
                <td height="" 6"" align=""left"" valign=""top"" style=""line-height:6px;display:none"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                    {basic}
                </td>
            </tr>
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""center"" valign=""top"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                        <tbody>
                            <tr>
                                <td align=""left"" valign=""top"">
                                    <table cellspacing=""0"" cellpadding=""0"" width="" 94"" role=""presentation"">
                                        <tbody>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""#"" data-saferedirecturl="" #"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height="" 6""border=""0"" alt="""" style=""display:block"">
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""border-radius:8px;background-color:#1a73e8;color:#ffffff!important;text-align:center;vertical-align:middle"" align=""center"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLink)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLink)}"" style=""display:block;border-radius:8px;color:#ffffff;text-decoration:none;font-family:Roboto,Helvetica Neue,Helvetica,Arial,sans-serif;font-size:14px;line-height:21px;font-weight:500;border-top:8px solid #1a73e8;border-right:16px solid #1a73e8;border-bottom:8px solid #1a73e8;border-left:16px solid #1a73e8;text-align:center;white-space:nowrap;letter-spacing:0.25px"" target=""_blank"">
                                                        {button}
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLink)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLink)}"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height=""5""border=""0"" alt="""" style=""display:block"">
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
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""20""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            ";
        }

        // Mẫu tạo nhân viên mới
        public static string ConfirmRegistryEmployee(IStringLocalizer _localizer, string subject, string confirmLink)
        {
            string basic = _localizer["EMAIL_TEXT_16", EnumLanguage.VI].Value ?? $@"Thank you for signing up to use Buffalo. Once you click on the link below, your account will become active.<br><strong>Please note that your Username is the email address you used for signup and the password will be set according to the link below.</strong>";
            string button = _localizer["EMAIL_TEXT_17", EnumLanguage.VI].Value ?? $@"Verify email";

            return $@"
            <tr>
                <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding-bottom:6px;padding-top:15px"">
                    {subject}
                </td>
            </tr>
            <tr>
                <td height="" 6"" align=""left"" valign=""top"" style=""line-height:6px;display:none"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                    {basic}
                </td>
            </tr>
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""center"" valign=""top"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                        <tbody>
                            <tr>
                                <td align=""left"" valign=""top"">
                                    <table cellspacing=""0"" cellpadding=""0"" width="" 94"" role=""presentation"">
                                        <tbody>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""#"" data-saferedirecturl="" #"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height="" 6""border=""0"" alt="""" style=""display:block"">
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""border-radius:8px;background-color:#1a73e8;color:#ffffff!important;text-align:center;vertical-align:middle"" align=""center"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLink)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLink)}"" style=""display:block;border-radius:8px;color:#ffffff;text-decoration:none;font-family:Roboto,Helvetica Neue,Helvetica,Arial,sans-serif;font-size:14px;line-height:21px;font-weight:500;border-top:8px solid #1a73e8;border-right:16px solid #1a73e8;border-bottom:8px solid #1a73e8;border-left:16px solid #1a73e8;text-align:center;white-space:nowrap;letter-spacing:0.25px"" target=""_blank"">
                                                        {button}
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLink)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLink)}"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height=""5""border=""0"" alt="""" style=""display:block"">
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
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""20""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            ";
        }

        // Mẫu gửi mật khẩu cho nhân viên mới
        public static string SendPassForEmployee(IStringLocalizer _localizer, string subject, string confirmLink, string password)
        {
            string basic = _localizer["EMAIL_TEXT_29", EnumLanguage.VI].Value ?? $@"Thank you for signing up to use Buffalo. <br> <strong>Your default password is: </strong> ";
            string button = _localizer["EMAIL_TEXT_30", EnumLanguage.VI].Value ?? $@"Login Now";

            return $@"
            <tr>
                <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding-bottom:6px;padding-top:15px"">
                    {subject}
                </td>
            </tr>
            <tr>
                <td height="" 6"" align=""left"" valign=""top"" style=""line-height:6px;display:none"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                    {basic} <i>{password}</i>
                </td>
            </tr>
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""center"" valign=""top"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                        <tbody>
                            <tr>
                                <td align=""left"" valign=""top"">
                                    <table cellspacing=""0"" cellpadding=""0"" width="" 94"" role=""presentation"">
                                        <tbody>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""#"" data-saferedirecturl="" #"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height="" 6""border=""0"" alt="""" style=""display:block"">
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""border-radius:8px;background-color:#1a73e8;color:#ffffff!important;text-align:center;vertical-align:middle"" align=""center"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLink)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLink)}"" style=""display:block;border-radius:8px;color:#ffffff;text-decoration:none;font-family:Roboto,Helvetica Neue,Helvetica,Arial,sans-serif;font-size:14px;line-height:21px;font-weight:500;border-top:8px solid #1a73e8;border-right:16px solid #1a73e8;border-bottom:8px solid #1a73e8;border-left:16px solid #1a73e8;text-align:center;white-space:nowrap;letter-spacing:0.25px"" target=""_blank"">
                                                        {button}
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLink)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLink)}"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height=""5""border=""0"" alt="""" style=""display:block"">
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
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""20""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            ";
        }


        // Mẫu gửi báo cáo lương hàng tháng
        public static string SendPayslipForEmployee(IStringLocalizer _localizer, string subject, string lastName, DateTime startPayPeriod, DateTime endPayPeriod, string password)
        {
            string basic = $@"{_localizer["EMAIL_TEXT_25", EnumLanguage.VI].Value ?? "Dear"} {lastName},<br><br>
            {_localizer["EMAIL_TEXT_18", EnumLanguage.VI].Value ?? "Your Payroll for period"} <strong>{startPayPeriod.ToString("dd/MM")} ~ {endPayPeriod.ToString("dd/MM")}</strong> {_localizer["EMAIL_TEXT_19", EnumLanguage.VI].Value ?? "can be viewed, printed or download as PDF from the attachment below."}<br><br>
            <i>{_localizer["EMAIL_TEXT_26", EnumLanguage.VI].Value ?? "Note: the password attached is"} <strong>{password}</strong></i>";
            string button = _localizer["EMAIL_TEXT_20", EnumLanguage.VI].Value ?? $@"Buffalo, Thanks you !";

            return $@"
            <tr>
                <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding-bottom:6px;padding-top:15px"">
                    {subject}
                </td>
            </tr>
            <tr>
                <td height="" 6"" align=""left"" valign=""top"" style=""line-height:6px;display:none"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                    {basic}
                </td>
            </tr>
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                    <i>{button}</i>
                </td>
            </tr>
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""20""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            ";
        }

        // Mẫu đăng ký cho kế toán
        public static string ConfirmRegistryAccountant(IStringLocalizer _localizer, string subject, string title, string confirmLinkLogin, string confirmLinkRegistry)
        {
            string basic_1 = _localizer["EMAIL_TEXT_21", EnumLanguage.VI].Value ?? $@"<strong>If you already have an account, please login at<br></strong>";
            string basic_2 = _localizer["EMAIL_TEXT_22", EnumLanguage.VI].Value ?? $@"<strong>If you don't have an account, please sign up at<br></strong>";
            string button_1 = _localizer["EMAIL_TEXT_23", EnumLanguage.VI].Value ?? $@"Login Now";
            string button_2 = _localizer["EMAIL_TEXT_24", EnumLanguage.VI].Value ?? $@"Registry Now";

            return $@"
            <tr>
                <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding-bottom:6px;padding-top:15px"">
                    {subject}
                </td>
            </tr>
            <tr>
                <td height="" 6"" align=""left"" valign=""top"" style=""line-height:6px;display:none"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                    {title}<br><br>
                </td>
            </tr>
            <tr>
                <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                    {basic_1}
                </td>
            </tr>
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""center"" valign=""top"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                        <tbody>
                            <tr>
                                <td align=""left"" valign=""top"">
                                    <table cellspacing=""0"" cellpadding=""0"" width="" 94"" role=""presentation"">
                                        <tbody>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""#"" data-saferedirecturl="" #"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height="" 6""border=""0"" alt="""" style=""display:block"">
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""border-radius:8px;background-color:#1a73e8;color:#ffffff!important;text-align:center;vertical-align:middle"" align=""center"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLinkLogin)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLinkLogin)}"" style=""display:block;border-radius:8px;color:#ffffff;text-decoration:none;font-family:Roboto,Helvetica Neue,Helvetica,Arial,sans-serif;font-size:14px;line-height:21px;font-weight:500;border-top:8px solid #1a73e8;border-right:16px solid #1a73e8;border-bottom:8px solid #1a73e8;border-left:16px solid #1a73e8;text-align:center;white-space:nowrap;letter-spacing:0.25px"" target=""_blank"">
                                                        {button_1}
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLinkLogin)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLinkLogin)}"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height=""5""border=""0"" alt="""" style=""display:block"">
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
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""20""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                    {basic_2}
                </td>
            </tr>
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""center"" valign=""top"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                        <tbody>
                            <tr>
                                <td align=""left"" valign=""top"">
                                    <table cellspacing=""0"" cellpadding=""0"" width="" 94"" role=""presentation"">
                                        <tbody>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""#"" data-saferedirecturl="" #"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height=""6""border=""0"" alt="""" style=""display:block"">
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""border-radius:8px;background-color:#1a73e8;color:#ffffff!important;text-align:center;vertical-align:middle"" align=""center"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLinkRegistry)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLinkRegistry)}"" style=""display:block;border-radius:8px;color:#ffffff;text-decoration:none;font-family:Roboto,Helvetica Neue,Helvetica,Arial,sans-serif;font-size:14px;line-height:21px;font-weight:500;border-top:8px solid #1a73e8;border-right:16px solid #1a73e8;border-bottom:8px solid #1a73e8;border-left:16px solid #1a73e8;text-align:center;white-space:nowrap;letter-spacing:0.25px"" target=""_blank"">
                                                        {button_2}
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLinkRegistry)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLinkRegistry)}"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height=""5""border=""0"" alt="""" style=""display:block"">
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
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""20""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            ";
        }

        // Mẫu đổi mật khẩu
        public static string ChangePassword(IStringLocalizer _localizer, string subject, string confirmLink)
        {
            string basic = _localizer["EMAIL_TEXT_27", EnumLanguage.VI].Value ?? $@"Someone just logged in and changed your password. We send you this email to make sure the person is you.";
            string button = _localizer["EMAIL_TEXT_28", EnumLanguage.VI].Value ?? $@"Check operation";

            return $@"
            <tr>
                <td align=""center"" valign=""top"" style=""font-family:Google Sans,Roboto,Helvetica,Arial,sans-serif;font-size:18px;color:#212121;line-height:27px;font-weight:500;padding-bottom:6px;padding-top:15px"">
                    {subject}
                </td>
            </tr>
            <tr>
                <td height="" 6"" align=""left"" valign=""top"" style=""line-height:6px;display:none"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""left"" valign=""top"" style=""font-family:Roboto,Helvetica,Arial,sans-serif;font-size:14px;color:#757575;line-height:21px"">
                    {basic}
                </td>
            </tr>
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""1""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            <tr>
                <td align=""center"" valign=""top"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" role=""presentation"">
                        <tbody>
                            <tr>
                                <td align=""left"" valign=""top"">
                                    <table cellspacing=""0"" cellpadding=""0"" width="" 94"" role=""presentation"">
                                        <tbody>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""#"" data-saferedirecturl="" #"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height="" 6""border=""0"" alt="""" style=""display:block"">
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""border-radius:8px;background-color:#1a73e8;color:#ffffff!important;text-align:center;vertical-align:middle"" align=""center"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLink)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLink)}"" style=""display:block;border-radius:8px;color:#ffffff;text-decoration:none;font-family:Roboto,Helvetica Neue,Helvetica,Arial,sans-serif;font-size:14px;line-height:21px;font-weight:500;border-top:8px solid #1a73e8;border-right:16px solid #1a73e8;border-bottom:8px solid #1a73e8;border-left:16px solid #1a73e8;text-align:center;white-space:nowrap;letter-spacing:0.25px"" target=""_blank"">
                                                        {button}
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td aria-hidden=""true"" align=""center"" valign=""top"">
                                                    <a href=""{HtmlEncoder.Default.Encode(confirmLink)}"" data-saferedirecturl=""{HtmlEncoder.Default.Encode(confirmLink)}"" target=""_blank"">
                                                        <img src="" {HtmlContent.email_line}"" width=""100%"" height=""5""border=""0"" alt="""" style=""display:block"">
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
            <tr>
                <td height=""20"" align=""left"" valign=""top"" style=""line-height:20px"">
                    <img src="" {HtmlContent.email_line}"" width=""1"" height=""20""border=""0"" alt="""" style=""display:block"">
                </td>
            </tr>
            ";
        }
    }
}
