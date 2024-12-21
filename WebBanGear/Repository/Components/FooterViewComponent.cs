using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace WebBanGear.Repository.Components
{
	public class FooterViewComponent : ViewComponent
	{
		private readonly DataContext _dataContext;
		public FooterViewComponent(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Contact.FirstOrDefaultAsync());
	}
}
