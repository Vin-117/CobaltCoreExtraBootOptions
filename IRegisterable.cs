using Nanoray.PluginManager;
using Nickel;

namespace Vintage.NewBootOptions;

internal interface IRegisterable
{
    static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}