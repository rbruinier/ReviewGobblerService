using System;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Service.Controllers.Model
{
    public class ResponsePlayer
    {
        public int PlayerId { get; set; }
        public string LocalUri { get; set; }
        public string WebUrl { get; set; }

        public ResponsePlayer()
        {
        }
    }
}
