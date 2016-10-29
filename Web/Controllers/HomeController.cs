using Common;
using Entities.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        [Route(Name = "FrontEndHomeIndex")]
        public ActionResult Index()
        {
            return View();
        }
        [Route("~/phan-hoi-thong-tin", Name = "FrontEndHomeFeedback")]
        public ActionResult Feedback()
        {
            ViewBag.Success = TempData["Success"];
            return View();
        }
        [CaptchaMvc.Attributes.CaptchaVerify("Mã bảo mật không chính xác!")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/phan-hoi-thong-tin")]
        public async Task<ActionResult> Feedback(Feedback model)
        {
            if (ModelState.IsValid)
            {
                Feedback feedback = new Feedback();
                feedback.Name = StringHelper.KillChars(model.Name);
                feedback.Email = StringHelper.KillChars(model.Email);
                feedback.PhoneNumber = StringHelper.KillChars(model.PhoneNumber);
                feedback.Title = StringHelper.KillChars(model.Title);
                feedback.Content = StringHelper.KillChars(model.Content);
                feedback.IsResponded = false;
                feedback.FeedbackDate = DateTime.Now;
                int result = await _repository.GetRepository<Feedback>().CreateAsync(feedback, 0);
                if (result > 0)
                {
                    TempData["Success"] = true;
                    return RedirectToRoute("FrontEndHomeFeedback");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin phản hồi");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin phản hồi");
                return View(model);
            }
        }
    }
}