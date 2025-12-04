using System.Windows.Controls;

namespace Logo
{
    public class LogoDesign
    {
        public int? Id { get; set; }
        public string? LogoName { get; set; }
        public string? DefaultImagePath { get; set; }
        public string? RedImagePath { get; set; }
        public string? BlueImagePath { get; set; }
        public string? GreenImagePath { get; set; }
        public string? SelectedColor { get; set; }
        public float? PositionX { get; set; }
        public float? PositionY { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float? Angle { get; set; }
    }
    public class LogoItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public Image ImageControl { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        public float Angle { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

}
