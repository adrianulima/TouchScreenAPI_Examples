using System;
using Sandbox.ModAPI;
using VRage.Utils;
using System.Collections.Generic;
using System.IO;

namespace Lima
{
  public class FileStorageHandler
  {
    public readonly string FileName = "ElectricInfo.cfg";

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

    public FileStorageHandler() { }

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
        string stringXML = MyAPIGateway.Utilities.SerializeToXML(new FileContent(managers));
        writer = MyAPIGateway.Utilities.WriteFileInWorldStorage(FileName, typeof(FileStorageHandler));
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
      if (!MyAPIGateway.Utilities.FileExistsInWorldStorage(FileName, typeof(FileStorageHandler)))
        return null;

      TextReader reader = null;
      FileContent content = null;
      try
      {
        reader = MyAPIGateway.Utilities.ReadFileInWorldStorage(FileName, typeof(FileStorageHandler));
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
      public List<ManagerContent> Managers = new List<ManagerContent>();

      public FileContent(List<ElectricNetworkManager> managers = null)
      {

        foreach (var man in managers)
          Managers.Add(man.GenerateManagerContent());
      }

      public FileContent() { }
    }

    public struct ManagerContent
    {
      public long GridId;
      public ElectricNetworkManager.PowerStats[][] History;
    }
  }
}