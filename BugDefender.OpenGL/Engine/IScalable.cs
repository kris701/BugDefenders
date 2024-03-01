namespace BugDefender.OpenGL.Engine
{
    public interface IScalable
    {
        public GameWindow Parent { get; set; }
        public float ScaleValue { get; set; }

        public int Scale(int value);
        public double Scale(double value);
        public float Scale(float value);
        public int InvScale(int value);
        public float InvScale(float value);
    }
}
