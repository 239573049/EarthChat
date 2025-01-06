using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarthChat.Core.Contract
{
	public interface IJwtContext
	{
		string CreateToken(Dictionary<string, string> dist, Guid userId, string[] roles);
	}
}
