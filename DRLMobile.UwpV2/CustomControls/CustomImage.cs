namespace DRLMobile.UwpV2.CustomControls
{
    public class CustomImage
    {
        public string Path { get; set; }

        public CustomImage()
        {
        }

        public CustomImage(string uri)
        {
            Path = uri;
        }
    }
}
