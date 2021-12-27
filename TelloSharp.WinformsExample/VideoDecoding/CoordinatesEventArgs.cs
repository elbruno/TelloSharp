namespace TelloSharp.WinformsExample
{
    public class CoordinatesEventArgs : EventArgs
    {
        public CoordinatesEventArgs(Point location, double area, string label) { Location = location; Area = area; Label = label; }
        public double Area { get; }
        public Point Location { get; }
        public string Label { get; }
    }
}
