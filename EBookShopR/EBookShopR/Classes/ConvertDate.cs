using EBookShopR.Classes;
using MD.PersianDateTime.Core;

namespace BookShop.Classes
{
    public class ConvertDate: IConvertDate
    {
        public DateTime ConvertShamsiToMiladi(string Date)
        {
            //var date=DateTime.Parse(Date,System.Globalization.CultureInfo.InvariantCulture);
            //var persianDate = new PersianDateTime(date);
            PersianDateTime persianDateTime = PersianDateTime.Parse(Date);
            return persianDateTime.ToDateTime();
            
        }
        public string ConverMiladitoShamsi(DateTime Date,string Format)
        {
            PersianDateTime persianDate = new PersianDateTime(Date);
            return persianDate.ToString(Format);
            //return persianDate.ToString("dddd d MMMM  yyyy ساعت hh:mm:ss tt ");

        }

       
    }
}
