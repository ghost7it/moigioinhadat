using Common;
using Entities.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("album")]
    public class AlbumController : BaseController
    {
        [Route("quan-ly-album", Name = "AlbumIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Album)]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Album> albums = await _repository.GetRepository<Album>().GetAllAsync();
            return View(albums);
        }
        [Route("nhap-album", Name = "AlbumCreate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Album)]
        public async Task<JsonResult> Create(Album model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Name = StringHelper.KillChars(model.Name);
                    model.Description = StringHelper.KillChars(model.Description);
                    model.CreateDate = DateTime.Now;
                    int result = await _repository.GetRepository<Album>().CreateAsync(model, AccountId);
                    if (result > 0)
                    {
                        string createDate = model.CreateDate.HasValue ? model.CreateDate.Value.ToString("dd/MM/yyyy") : "";
                        return Json(new { success = true, id = model.Id, createdate = createDate }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không nhập được album!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi:" + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ và chính xác các thông tin!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("cap-nhat-album", Name = "AlbumUpdate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Album)]
        public async Task<JsonResult> Update(Album model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Album album = await _repository.GetRepository<Album>().ReadAsync(model.Id);
                    if (album == null)
                        return Json(new { success = false, message = "Không tìm thấy album!" }, JsonRequestBehavior.AllowGet);
                    album.Name = StringHelper.KillChars(model.Name);
                    album.Description = StringHelper.KillChars(model.Description);
                    int result = await _repository.GetRepository<Album>().UpdateAsync(album, AccountId);
                    if (result > 0)
                    {
                        string createDate = model.CreateDate.HasValue ? model.CreateDate.Value.ToString("dd/MM/yyyy") : "";
                        return Json(new { success = true, id = model.Id, createdate = createDate }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không cập nhật được album!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi:" + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ và chính xác các thông tin!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("xoa-album/{id?}", Name = "AlbumDelete")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Album)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Album categoryType = await _repository.GetRepository<Album>().ReadAsync(id);
                if (categoryType != null)
                {
                    if (categoryType.AlbumDetails != null && categoryType.AlbumDetails.Any())
                    {
                        //Nếu album đang có ảnh thì không cho xóa
                        return Json(new { success = false, message = "Bạn phải xóa ảnh trong album trước!" });
                    }
                    int result = await _repository.GetRepository<Album>().DeleteAsync(categoryType, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được album!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy album!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("danh-sach-anh-album/{albumId?}", Name = "AlbumDetail")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Album)]
        public async Task<ActionResult> Detail(long albumId)
        {
            Album album = await _repository.GetRepository<Album>().ReadAsync(albumId);
            IEnumerable<AlbumDetail> albumDetail = await _repository.GetRepository<AlbumDetail>().GetAllAsync(o => o.AlbumId == albumId);
            ViewBag.AlbumName = album.Name;
            ViewBag.AlbumId = albumId;
            return View(albumDetail);
        }
        [Route("nhap-anh-cho-album/{albumId?}", Name = "AlbumDetailCreate")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Album)]
        public async Task<ActionResult> DetailCreate(long albumId)
        {
            Album album = await _repository.GetRepository<Album>().ReadAsync(albumId);
            ViewBag.AlbumName = album.Name;
            ViewBag.AlbumId = albumId;
            AlbumDetail albumDetail = new AlbumDetail()
            {
                Id = 0,
                AlbumId = albumId,
                UploadDate = DateTime.Now,
            };
            return View(albumDetail);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("nhap-anh-cho-album/{albumId?}")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Album)]
        public async Task<ActionResult> DetailCreate(long albumId, AlbumDetail model)
        {
            if (ModelState.IsValid)
            {
                AlbumDetail albumDetail = new AlbumDetail();
                albumDetail.Title = StringHelper.KillChars(model.Title);
                albumDetail.Description = StringHelper.KillChars(model.Description);
                albumDetail.SmallTitle = StringHelper.KillChars(model.SmallTitle);
                albumDetail.OrdinalNumber = model.OrdinalNumber;
                albumDetail.UploadDate = DateTime.Now;
                albumDetail.Link = StringHelper.KillChars(model.Link);
                albumDetail.PhotoLocation = StringHelper.KillChars(model.PhotoLocation);
                albumDetail.AlbumId = albumId;
                int result = await _repository.GetRepository<AlbumDetail>().CreateAsync(albumDetail, AccountId);
                if (result > 0)
                {
                    TempData["Success"] = "Nhập ảnh mới thành công!";
                    return RedirectToRoute("AlbumDetail", new { albumId = albumId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nhập ảnh mới không thành công! Vui lòng kiểm tra và thử lại!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin!");
                return View(model);
            }
        }
        [Route("cap-nhat-anh-cho-album/{albumDetailId?}", Name = "AlbumDetailUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Album)]
        public async Task<ActionResult> DetailUpdate(long albumDetailId)
        {
            AlbumDetail albumDetail = await _repository.GetRepository<AlbumDetail>().ReadAsync(albumDetailId);
            ViewBag.AlbumName = albumDetail.Album.Name;
            ViewBag.AlbumId = albumDetail.AlbumId;
            return View(albumDetail);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("cap-nhat-anh-cho-album/{albumDetailId?}")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Album)]
        public async Task<ActionResult> DetailUpdate(long albumDetailId, AlbumDetail model)
        {
            if (ModelState.IsValid)
            {
                AlbumDetail albumDetail = await _repository.GetRepository<AlbumDetail>().ReadAsync(albumDetailId);
                albumDetail.Title = StringHelper.KillChars(model.Title);
                albumDetail.Description = StringHelper.KillChars(model.Description);
                albumDetail.SmallTitle = StringHelper.KillChars(model.SmallTitle);
                albumDetail.OrdinalNumber = model.OrdinalNumber;
                albumDetail.Link = StringHelper.KillChars(model.Link);
                albumDetail.PhotoLocation = StringHelper.KillChars(model.PhotoLocation);
                int result = await _repository.GetRepository<AlbumDetail>().UpdateAsync(albumDetail, AccountId);
                if (result > 0)
                {
                    TempData["Success"] = "Cập nhật ảnh thành công!";
                    return RedirectToRoute("AlbumDetail", new { albumId = model.AlbumId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Cập nhật ảnh không thành công! Vui lòng kiểm tra và thử lại!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin!");
                return View(model);
            }
        }
        /// <summary>
        /// Xóa ảnh trong album
        /// </summary>
        /// <param name="albumDetailId"></param>
        /// <returns></returns>
        [Route("xoa-anh-album/{albumDetailId?}", Name = "AlbumDetailDelete")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Album)]
        public async Task<ActionResult> DetailDelete(long albumDetailId)
        {
            try
            {
                AlbumDetail albumDetail = await _repository.GetRepository<AlbumDetail>().ReadAsync(albumDetailId);
                long albumId = albumDetail.AlbumId;
                if (albumDetail != null)
                {
                    int result = await _repository.GetRepository<AlbumDetail>().DeleteAsync(albumDetail, AccountId);
                    if (result > 0)
                    {
                        TempData["Success"] = "Xóa ảnh thành công!";
                        return RedirectToRoute("AlbumDetail", new { albumId = albumId });
                    }
                    else
                    {
                        TempData["Error"] = "Xóa ảnh không thành công!";
                        return RedirectToRoute("AlbumDetail", new { albumId = albumId });
                    }
                }
                else
                {
                    TempData["Error"] = "Xóa ảnh không thành công! (Không tìm thấy ảnh)";
                    return RedirectToRoute("AlbumDetail", new { albumId = albumId });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Xóa ảnh không thành công! Lỗi: " + ex.Message;
                return RedirectToRoute("AlbumIndex");
            }
        }
    }
}