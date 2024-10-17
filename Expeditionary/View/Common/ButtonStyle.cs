namespace Expeditionary.View.Common
{
    public class ButtonStyle
    {
        public string Container { get; }
        public string Image { get; }
        public string Text { get; }

        public ButtonStyle(string container, string image, string text)
        {
            Container = container;
            Image = image;
            Text = text;
        }
    }
}
