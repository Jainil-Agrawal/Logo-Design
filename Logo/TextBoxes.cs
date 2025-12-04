using System.ComponentModel;
using System.Windows.Controls;

namespace Logo
{
    public class TextBoxes
    {
        public int? Id {  get; set; }
        public string? Text { get; set; }
        public string? TextBoxName {  get; set; }
        public float? PositionX { get; set; }
        public float? PositionY { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
    }
    public class Container
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public required  Grid TextControl { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        public override string? ToString()
        {
            return Name;
        }
    }

}
