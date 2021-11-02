namespace ReviewGobbler.Shared.Model
{
    public class AlbumLabel
    {
        public int AlbumId { get; set; }
        public Album Album { get; set; }

        public int LabelId { get; set; }
        public Label Label { get; set; }
    }
}
