using CCommon.Common.Config.ResultValue;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCommon.Common.Config
{
    public interface IConfigurationProvider
    {
        Dictionary<string, IValue> Data { get; }
        int Order { get; }
        //
        // 摘要:
        //     Returns a change token if this provider supports change tracking, null otherwise.
        //IChangeToken GetReloadToken();
        //
        // 摘要:
        //     Loads configuration values from the source represented by this Microsoft.Extensions.Configuration.IConfigurationProvider.
        void Load();

        //
        // 摘要:
        //     Tries to get a configuration value for the specified key.
        //
        // 参数:
        //   key:
        //     The key.
        //
        //   value:
        //     The value.
        //
        // 返回结果:
        //     True if a value for the specified key was found, otherwise false.
        IValue Get(string key);
    }
}
