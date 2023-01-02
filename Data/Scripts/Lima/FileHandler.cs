using System;
using Sandbox.ModAPI;
using VRage.Utils;
using System.Collections.Generic;
using System.IO;

namespace Lima
{
  public class FileHandler
  {
    public readonly string FileName = "ElectricInfo.cfg";
    private readonly List<ElectricNetworkInfoTSS> _activeTSSs = new List<ElectricNetworkInfoTSS>();

    private FileContent _currentFileContent;
    public FileContent CurrentFileContent
    {
      get
      {
        if (_currentFileContent != null)
          return _currentFileContent;
        _currentFileContent = Load();
        return _currentFileContent;
      }
    }

    public FileHandler()
    {
    }

    public void AddActiveTSS(ElectricNetworkInfoTSS tss)
    {
      _activeTSSs.Add(tss);
    }

    public void RemoveActiveTSS(ElectricNetworkInfoTSS tss)
    {
      _activeTSSs.Remove(tss);
    }

    public AppContent? GetAppContent(long blockId, string surfaceName)
    {
      if (CurrentFileContent == null)
        return null;

      foreach (var app in CurrentFileContent.Apps)
      {
        if (app.BlockId == blockId && app.SurfaceName == surfaceName)
          return app;
      }

      return null;
    }

    public ManagerContent? GetManagerContent(long gridId)
    {
      if (CurrentFileContent == null)
        return null;

      foreach (var man in CurrentFileContent.Managers)
      {
        if (man.GridId == gridId)
          return man;
      }

      return null;
    }

    public void Save(List<ElectricNetworkManager> managers)
    {
      TextWriter writer = null;
      try
      {
        string stringXML = MyAPIGateway.Utilities.SerializeToXML(new FileContent(_activeTSSs, managers));
        writer = MyAPIGateway.Utilities.WriteFileInLocalStorage(FileName, typeof(FileHandler));
        writer.Write(stringXML);
        writer.Flush();
      }
      catch (Exception e)
      {
        MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");
      }
      finally
      {
        if (writer != null)
          writer.Close();
      }
    }

    public FileContent Load()
    {
      if (!MyAPIGateway.Utilities.FileExistsInLocalStorage(FileName, typeof(FileHandler)))
        return null;

      TextReader reader = null;
      FileContent content = null;
      try
      {
        reader = MyAPIGateway.Utilities.ReadFileInLocalStorage(FileName, typeof(FileHandler));
        content = MyAPIGateway.Utilities.SerializeFromXML<FileContent>(reader.ReadToEnd());
      }
      catch (Exception e)
      {
        MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");
      }
      finally
      {
        if (reader != null)
          reader.Close();
      }

      return content;
    }

    public class FileContent
    {
      public List<AppContent> Apps = new List<AppContent>();
      public List<ManagerContent> Managers = new List<ManagerContent>();

      public FileContent(List<ElectricNetworkInfoTSS> activeTSSs, List<ElectricNetworkManager> managers = null)
      {
        foreach (var tss in activeTSSs)
          Apps.Add(tss.GenerateAppContent());

        foreach (var man in managers)
          Managers.Add(man.GenerateManagerContent());
      }

      public FileContent() { }
    }

    public struct AppContent
    {
      public long BlockId;
      public string SurfaceName;

      public int Layout;
      public int ChartIntervalIndex;
      public bool BatteryChartEnabled;
    }

    public struct ManagerContent
    {
      public long GridId;
      public List<ElectricNetworkManager.PowerStats> PowerStatsHistory;
    }
  }
}