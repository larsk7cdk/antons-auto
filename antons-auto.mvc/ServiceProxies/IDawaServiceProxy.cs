using antons_auto.mvc.Models;

namespace antons_auto.mvc.ServiceProxies
{
	public interface IDawaServiceProxy
	{
		LocationModel GetLocation(string streetName, string streetNo, int postalCode);
	}
}