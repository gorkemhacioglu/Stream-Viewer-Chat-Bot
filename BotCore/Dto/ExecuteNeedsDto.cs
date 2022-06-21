using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BotCore.Dto;

public class ExecuteNeedsDto
{
    public string ProxyListDirectory { get; set; }

    public List<string> UserAgentStrings { get; set; } = new List<string>();

    public List<string> ChatMessages { get; set; } = new List<string>();

    public string Stream { get; set; }

    public StreamService.Service Service { get; set; }

    public bool Headless { get; set; }

    public int BrowserLimit { get; set; }

    public int RefreshInterval { get; set; }

    public string PreferredQuality { get; set; }

    public ConcurrentQueue<LoginDto> LoginInfos { get; set; } = new ConcurrentQueue<LoginDto>();

    public bool UseLowCpuRam { get; set; }
}