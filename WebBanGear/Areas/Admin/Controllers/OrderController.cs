using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGear.Models;
using WebBanGear.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanGear.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;

        public OrderController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            // Số lượng orders trên mỗi trang
            const int pageSize = 5;

            // Nếu trang hiện tại nhỏ hơn 1 thì gán là trang 1
            if (pg < 1)
            {
                pg = 1;
            }
            var orders = await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync();
            // Tổng số lượng orders
            int recsCount = orders.Count;

            // Tạo đối tượng phân trang
            var pager = new Paginate(recsCount, pg, pageSize);

            // Tính toán số lượng bản ghi cần bỏ qua
            int recSkip = (pg - 1) * pageSize;

            // Lấy dữ liệu của trang hiện tại
            var data = orders.Skip(recSkip).Take(pager.PageSize).ToList();

            // Gửi đối tượng phân trang tới view
            ViewBag.Pager = pager;

            // Trả dữ liệu về view
            return View(data);
        }

        public async Task<IActionResult> ViewOrder(string id)
        {
            // Tìm order dựa trên mã đơn hàng
            var order = await _dataContext.Orders
                .FirstOrDefaultAsync(o => o.OrderCode == id);

            if (order == null)
            {
                return NotFound();
            }

            // Lấy chi tiết đơn hàng và eager load thông tin sản phẩm
            var orderDetails = await _dataContext.OrderDetails
                .Include(od => od.Product)  // Eager load thông tin sản phẩm
                .Where(od => od.OrderCode == id)
                .ToListAsync();

            if (!orderDetails.Any())
            {
                return NotFound();
            }

            // Tính toán tổng giá trị
            decimal grandTotal = orderDetails.Sum(od => od.Price * od.Quantity);

            // Truyền dữ liệu qua ViewBag
            ViewBag.ShippingCost = order.ShippingCost; // Giá vận chuyển
            ViewBag.GrandTotal = grandTotal;

            return View(orderDetails);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occured while updating the order status");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            _dataContext.Update(order);

            if (status == 0)
            {
                // lấy dữ liệu order detail dựa vào order.OrderCode
                var DetailsOrder = await _dataContext.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == order.OrderCode)
                    .Select(od => new
                    {
                        od.Quantity,
                        od.Product.Price,
                        od.Product.CapitalPrice
                    }).ToListAsync();

                // lấy data thống kê dựa vào ngày đặt hàng
                var statisticalModel = await _dataContext.Statistticals.FirstOrDefaultAsync(s => s.DateCreated.Date == order.CreateDate.Date);
                if (statisticalModel != null)
                {
                    foreach (var orderDetail in DetailsOrder)
                    {
                        statisticalModel.Quantity += 1;
                        statisticalModel.Sold += orderDetail.Quantity;
                        statisticalModel.Revenue += orderDetail.Quantity * orderDetail.Price;
                        statisticalModel.Profit += orderDetail.Price - orderDetail.CapitalPrice;
                    }
                    _dataContext.Update(statisticalModel);
                }
                else
                {
                    int new_quantity = 0;
                    int new_sold = 0;
                    decimal new_profit = 0;
                    foreach (var orderDetail in DetailsOrder)
                    {
                        new_quantity += 1;
                        new_sold += orderDetail.Quantity;
                        new_profit += orderDetail.Price - orderDetail.CapitalPrice;

                        statisticalModel = new StatistticalModel
                        {
                            DateCreated = order.CreateDate,
                            Quantity = new_quantity,
                            Sold = new_sold,
                            Revenue = orderDetail.Quantity * orderDetail.Price,
                            Profit = new_profit
                        };
                    }
                    _dataContext.Add(statisticalModel);

                }
            }
            try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { success = true, message = "Order status updated successfully" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An error occured while updating the order status");
                }
        }
        public async Task<IActionResult> Delete(int Id)
        {
            OrderModel order = await _dataContext.Orders.FindAsync(Id);
            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();
            TempData["error"] = "Xoá đơn hàng thành công !";
            return RedirectToAction("Index");
        }
    }
}
