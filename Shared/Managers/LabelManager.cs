using System.Linq;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Shared.Managers
{
    public class LabelManager
    {
        private DatabaseContext _databaseContext;

        public LabelManager(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Label FetchOrInsertLabelByName(string name)
        {
            var existingLabel = _databaseContext.Labels.SingleOrDefault(label => label.Name == name);

            if (existingLabel != null)
            {
                return existingLabel;
            }

            var label = new Label
            {
                Name = name
            };

            _databaseContext.Labels.Add(label);

            _databaseContext.SaveChanges();

            return label;
        }
    }
}
