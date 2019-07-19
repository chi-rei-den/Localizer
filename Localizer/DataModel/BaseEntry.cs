namespace Localizer.DataModel
{
    public class BaseEntry : IEntry
    {
        public string Origin { get; set; }

        public string Translation { get; set; }

        public IEntry Clone()
        {
            return new BaseEntry()
            {
                Origin = this.Origin,
                Translation = this.Translation
            };
        }
    }
}
