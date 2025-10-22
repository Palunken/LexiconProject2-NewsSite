using Azure.Data.Tables;

namespace The_Post.Models.VM
{
    public class HistoricalElectricityPricesViewModel
    {
        public List<TableEntity> ElectricityPrices { get; set; } = new List<TableEntity>();
        public bool NoHistoricalData { get; set; } = false;
        public bool ErrorOccurred { get; set; } = false;

        public string  SelectedDate { get; set; }
    }
}
