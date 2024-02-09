namespace TDGame.OpenGL.Engine
{
    public interface IChildContainer : IControl
    {
        public IControl Child { get; set; }
    }
}
