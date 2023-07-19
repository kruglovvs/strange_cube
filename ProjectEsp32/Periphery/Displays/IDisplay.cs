namespace ProjectESP32.Periphery.Displays
{
    public interface IDisplay <ImageType>
    {
        public void SetImage(ImageType image);
    }
}
