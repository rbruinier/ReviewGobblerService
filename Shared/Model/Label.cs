using System;
using System.Collections.Generic;

namespace ReviewGobbler.Shared.Model
{
    public class Label
    {
        public int LabelId { get; set; }
        public string Name { get; set; }
        public IList<AlbumLabel> AlbumLabels { get; set; }

        public Label()
        {
        }
    }
}
