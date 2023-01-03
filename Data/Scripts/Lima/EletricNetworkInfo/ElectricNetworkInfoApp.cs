using Lima.API;
using VRage.Utils;
using System.Text;
using System;

namespace Lima
{
  public class ElectricNetworkInfoApp : FancyApp
  {
    private ElectricNetworkManager _electricMan;

    public FancyView MainView;
    public WindowButtons WindowBarButtons;
    public OverviewPanel OverviewPanel;
    public EntitiesPanel EntitiesPanel;

    public Action SaveConfigAction;

    public ElectricNetworkInfoApp(ElectricNetworkManager electricManager, Action saveConfigAction)
    {
      _electricMan = electricManager;
      _electricMan.UpdateEvent += UpdateValues;

      SaveConfigAction = saveConfigAction;
    }

    public void CreateElements()
    {
      var windowBar = new FancyWindowBar("Electric Network Info");
      AddChild(windowBar);

      WindowBarButtons = new WindowButtons(OnChangeConfig);
      windowBar.AddChild(WindowBarButtons);

      MainView = new FancyView();
      AddChild(MainView);

      OverviewPanel = new OverviewPanel(OnChangeConfig);
      MainView.AddChild(OverviewPanel);
      OverviewPanel.CreateElements();

      EntitiesPanel = new EntitiesPanel();
      MainView.AddChild(EntitiesPanel);
      EntitiesPanel.CreateElements();
    }

    public void OnChangeConfig()
    {
      SaveConfigAction();
      UpdateLayout();
    }

    public void ApplySettings(AppContent content)
    {
      WindowBarButtons.CurrentLayout = content.Layout;
      OverviewPanel.ApplySettings(content, _electricMan);

      UpdateLayout();
      UpdateValues();
    }

    public void UpdateLayout()
    {
      switch (WindowBarButtons.CurrentLayout)
      {
        default:
        case 0:
          MainView.Direction = ViewDirection.Column;
          OverviewPanel.Enabled = true;
          EntitiesPanel.Enabled = true;
          break;
        case 1:
          MainView.Direction = ViewDirection.Row;
          OverviewPanel.Enabled = true;
          EntitiesPanel.Enabled = true;
          break;
        case 2:
          MainView.Direction = ViewDirection.Column;
          OverviewPanel.Enabled = true;
          EntitiesPanel.Enabled = false;
          break;
        case 3:
          MainView.Direction = ViewDirection.Column;
          OverviewPanel.Enabled = false;
          EntitiesPanel.Enabled = true;
          break;
      }
    }

    public void UpdateValues()
    {
      if (OverviewPanel.Enabled)
        OverviewPanel.UpdateValues(_electricMan);

      if (EntitiesPanel.Enabled)
        EntitiesPanel.UpdateValues(_electricMan);
    }

    public void Dispose()
    {
      ForceDispose();
      OverviewPanel.Dispose();
      EntitiesPanel.Dispose();
      _electricMan.UpdateEvent -= UpdateValues;
    }

    public static string PowerFormat(float MW, string decimals = "0.##")
    {
      if (MW >= 1000000000000)
        return $"{MW.ToString("E2")} MW";
      if (MW >= 1000000000)
        return $"{(MW / 1000000000).ToString(decimals)} PW";
      if (MW >= 1000000)
        return $"{(MW / 1000000).ToString(decimals)} TW";
      if (MW >= 1000)
        return $"{(MW / 1000).ToString(decimals)} GW";
      if (MW >= 1)
        return $"{MW.ToString(decimals)} MW";
      if (MW >= 0.001)
        return $"{(MW * 1000f).ToString(decimals)} kW";
      return $"{(MW * 1000000f).ToString(decimals)} W";
    }

    private readonly static StringBuilder _str = new StringBuilder();
    public static string HoursFormat(float hours, string decimals = "0.##")
    {
      if (hours > 24 * 365)
        return "1 year +";

      _str.Clear();
      MyValueFormatter.AppendTimeInBestUnit(hours * 3600f, _str);
      return _str.ToString();
    }
  }
}