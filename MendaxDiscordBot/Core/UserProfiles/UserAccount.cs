using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MendaxDiscordBot.Core.UserProfiles
{
    public class UserAccount
    {
        public ulong UserID { get; set; }

        public uint EXP { get; set; }

        public uint Level
        {
            get
            {
                return (uint)Math.Sqrt(EXP / 50);
            }
            set
            {

            }
        }

        public DateTime NextMessageReward { get; set; }

        public int Warns { get; set; }
    }
}
