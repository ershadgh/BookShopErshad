namespace EBookShopR.Models.ViewModels
{
    public class ControllerViewModel
    {
        public string AreaName { get; set; }
        public IList<Attribute> ControllerAttributes { get; set; }
        public string ControllerDisplayName { get; set; }
        public string ControllerId => $"{AreaName}:{ControllerName}";
        public String ControllerName { get; set; }
        public IList<ActionViewModel> MvcAction { get; set; } = new List<ActionViewModel>();
    }
}
