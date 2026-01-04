using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Game.Domain.Entities
{
    public class UserLibraryGame 
    {
        public Guid idLibraryGame {  get; set; }
        public Guid orderId { get; set; }
        public Guid userId { get; set; }
        public Guid idGame { get; set; }
        public bool isActive { get; set; }
        public DateTime createdAt { get; set; }

    }
}
