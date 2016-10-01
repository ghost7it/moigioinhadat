using Common;
using Entities.Enums;
using Entities.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("quan-ly-thong-tin-phan-hoi")]
    public class FeedbackController : BaseController
    {
        [Route("danh-sach-thong-tin-phan-hoi", Name = "FeedbackIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Feedback)]
        public async Task<ActionResult> Index()
        {
            var feedbacks = await _repository.GetRepository<Feedback>().GetAllAsync();
            return View(feedbacks);
        }
        [Route("chi-tiet-thong-tin-phan-hoi/{id?}", Name = "FeedbackDetail")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Feedback)]
        public async Task<ActionResult> Detail(long id)
        {
            var feedback = await _repository.GetRepository<Feedback>().ReadAsync(id);
            return View(feedback);
        }
        [Route("xoa-thong-tin-phan-hoi/{id?}", Name = "FeedbackDelete")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Feedback)]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var feedback = await _repository.GetRepository<Feedback>().ReadAsync(id);
                if (feedback == null)
                {
                    TempData["Error"] = "Không tìm thấy phản hồi!";
                    return RedirectToRoute("FeedbackIndex");
                }
                if (feedback.RespondFeedbacks != null && feedback.RespondFeedbacks.Any())
                {
                    try
                    {
                        await _repository.GetRepository<RespondFeedback>().DeleteAsync(feedback.RespondFeedbacks, AccountId);
                    }
                    catch { }
                }
                int result = await _repository.GetRepository<Feedback>().DeleteAsync(feedback, AccountId);
                if (result > 0)
                {
                    TempData["Success"] = "Đã xóa phản hồi thành công!";
                    return RedirectToRoute("FeedbackIndex");
                }
                else
                {
                    TempData["Error"] = "Không xóa được phản hồi!";
                    return RedirectToRoute("FeedbackIndex");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không xóa được phản hồi! Lỗi: " + ex.Message;
                return RedirectToRoute("FeedbackIndex");
            }
        }
        [Route("tra-loi-phan-hoi", Name = "FeedbackRespondFeedback")]
        [HttpPost]
        //[ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Feedback)]
        public async Task<ActionResult> RespondFeedback(FormCollection model)
        {
            try
            {
                string id = StringHelper.KillChars(Request.Form["Id"]);
                string respond = StringHelper.KillChars(Request.Form["Respond"]);
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Không tìm thấy dữ liệu!";
                    return RedirectToRoute("FeedbackIndex");
                }
                long feedbackId = Convert.ToInt64(id);
                if (string.IsNullOrEmpty(respond))
                {
                    TempData["Error"] = "Vui lòng nhập nội dung trả lời!";
                    return RedirectToRoute("FeedbackDetail", new { id = feedbackId });
                }
                var feedback = await _repository.GetRepository<Feedback>().ReadAsync(feedbackId);
                RespondFeedback respondFeedback = new RespondFeedback()
                {
                    Content = respond,
                    AccountId = AccountId,
                    FeedbackId = feedbackId,
                    RespondDate = DateTime.Now
                };
                var systemInfo = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();
                int result = await _repository.GetRepository<RespondFeedback>().CreateAsync(respondFeedback, AccountId);
                if (result > 0 && systemInfo != null)
                {
                    //Gửi thư
                    string title = "Trả lời: " + feedback.Title;
                    StringBuilder body = new StringBuilder();
                    body.Append("Kính gửi " + StringHelper.KillChars(feedback.Name) + ",<br /><br />");
                    body.Append(" Chúng tôi xin trả lời thông tin phản hồi của quí vị như sau: <br />");
                    body.Append(respond);
                    body.Append("<br /><br />Vô cùng xin lỗi nếu email này làm phiền quí vị!<br /><br />");
                    body.Append("<br />Kính thư, <br /><br />");
                    body.Append(systemInfo.SiteName + "<br />");
                    body.Append("Phát triển bởi eBtech Team<br />");
                    body.Append("Webmaster: quochuy7it@gmail.com");
                    bool result2 = await SendEmail.SendAsync(systemInfo.SMTPName, systemInfo.SMTPEmail, systemInfo.SMTPPassword, feedback.Email, title, body.ToString());
                    if (result2)
                    {
                        try
                        {
                            feedback.IsResponded = true;
                            await _repository.GetRepository<Feedback>().UpdateAsync(feedback, AccountId);
                        }
                        catch { }
                        TempData["Success"] = "Đã trả lời phản hồi thành công!";
                        return RedirectToRoute("FeedbackDetail", new { id = feedbackId });
                    }
                    else
                    {
                        try
                        {
                            await _repository.GetRepository<RespondFeedback>().DeleteAsync(respondFeedback, AccountId);
                        }
                        catch { }
                        TempData["Error"] = "Không nhập được nội dung trả lời. (Không gửi được email)";
                        return RedirectToRoute("FeedbackDetail", new { id = feedbackId });
                    }
                }
                else
                {
                    TempData["Error"] = "Không nhập được nội dung trả lời.";
                    return RedirectToRoute("FeedbackDetail", new { id = feedbackId });
                }
            }
            catch (Exception ex)
            {
                string message = "Đã xảy ra lỗi: " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}