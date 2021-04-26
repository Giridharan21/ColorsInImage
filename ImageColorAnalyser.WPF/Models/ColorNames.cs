namespace ImageColorAnalyser.WPF.Models
{
using Prism.Mvvm;
    public class ColorNames : BindableBase
    {
        private string red;
        private string green;
        private string orange;
        private string violet;
        private string indigo;
        private string blue;
        private string yellow;

        public ColorNames()
        {
            Red = "Red";
            Green = "Green";
            Yellow = "Yellow";
            Blue = "Blue";
            Indigo = "Indigo";
            Violet = "Violet";
            Orange = "Orange";
        }
        public string Red { get => red; set => SetProperty(ref red, value); }
        public string Green { get => green; set => SetProperty(ref green , value); }
        public string Yellow { get => yellow; set => SetProperty(ref yellow, value); }
        public string Blue { get => blue; set => SetProperty(ref blue, value); }
        public string Indigo { get => indigo; set => SetProperty(ref indigo, value); }
        public string Violet { get => violet; set => SetProperty(ref violet, value); }
        public string Orange { get => orange; set => SetProperty(ref orange, value); }
    }
}