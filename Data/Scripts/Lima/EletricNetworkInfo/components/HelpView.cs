using VRageMath;
using Lima.API;

namespace Lima
{
  public class HelpView : FancyScrollView
  {
    public HelpView()
    {
      Direction = ViewDirection.Column;
    }

    public void CreateElements()
    {
      Padding = new Vector4(4);

      var consumptionLabel = new FancyLabel("Consumption", 0.4f);
      consumptionLabel.AutoBreakLine = true;
      consumptionLabel.TextColor = Color.Yellow;
      AddChild(consumptionLabel);
      var consumptionText = new FancyLabel("Current and consumption if all blocks require maximum input.", 0.5f);
      consumptionText.AutoBreakLine = true;
      consumptionText.Margin = new Vector4(0, 0, 0, 10);
      AddChild(consumptionText);

      var productionLabel = new FancyLabel("Production", 0.4f);
      productionLabel.AutoBreakLine = true;
      productionLabel.TextColor = Color.Yellow;
      AddChild(productionLabel);
      var productionText = new FancyLabel("Current and production if all producers are on maximum output.", 0.5f);
      productionText.AutoBreakLine = true;
      productionText.Margin = new Vector4(0, 0, 0, 10);
      AddChild(productionText);

      var batteryLabel = new FancyLabel("Battery Output", 0.4f);
      batteryLabel.AutoBreakLine = true;
      batteryLabel.TextColor = Color.Yellow;
      AddChild(batteryLabel);
      var batteryText = new FancyLabel("Current power production of batteries and maximum output.", 0.5f);
      batteryText.AutoBreakLine = true;
      batteryText.Margin = new Vector4(0, 0, 0, 10);
      AddChild(batteryText);

      var chartLabel = new FancyLabel("Chart Time Span", 0.4f);
      chartLabel.AutoBreakLine = true;
      chartLabel.TextColor = Color.Yellow;
      AddChild(chartLabel);
      var chartText = new FancyLabel("Set the time span for the graph. \"30s\" means over the last 30 seconds.", 0.5f);
      chartText.AutoBreakLine = true;
      chartText.Margin = new Vector4(0, 0, 0, 10);
      AddChild(chartText);

      var batteryCheckboxLabel = new FancyLabel("Battery Checkbox", 0.4f);
      batteryCheckboxLabel.AutoBreakLine = true;
      batteryCheckboxLabel.TextColor = Color.Yellow;
      AddChild(batteryCheckboxLabel);
      var batteryCheckboxText = new FancyLabel("If checked, battery output will count as production in the graph.", 0.5f);
      batteryCheckboxText.AutoBreakLine = true;
      batteryCheckboxText.Margin = new Vector4(0, 0, 0, 10);
      AddChild(batteryCheckboxText);

      var batteryStorageLabel = new FancyLabel("Battery Storage", 0.4f);
      batteryStorageLabel.AutoBreakLine = true;
      batteryStorageLabel.TextColor = Color.Yellow;
      AddChild(batteryStorageLabel);
      var batteryStorageText = new FancyLabel("Time until the battery is depleted.\nIf red border, the system is overloaded. Needs more production.", 0.5f);
      batteryStorageText.AutoBreakLine = true;
      batteryStorageText.Margin = new Vector4(0, 0, 0, 10);
      AddChild(batteryStorageText);

      var detailedLabel = new FancyLabel("Detailed Consumption and Production", 0.4f);
      detailedLabel.AutoBreakLine = true;
      detailedLabel.TextColor = Color.Yellow;
      AddChild(detailedLabel);
      var detailedText = new FancyLabel("A list of consumers and producers from highest power to lowest.", 0.5f);
      detailedText.AutoBreakLine = true;
      detailedText.Margin = new Vector4(0, 0, 0, 10);
      AddChild(detailedText);

      var buttonsLabel = new FancyLabel("Window Bar Buttons", 0.4f);
      buttonsLabel.AutoBreakLine = true;
      buttonsLabel.TextColor = Color.Yellow;
      AddChild(buttonsLabel);
      var buttonsText = new FancyLabel("Help button open this help panel.\nGears button open settings.\nChanges how info is displayed.", 0.5f);
      buttonsText.AutoBreakLine = true;
      buttonsText.Margin = new Vector4(0, 0, 0, 10);
      AddChild(buttonsText);

      var touchLabel = new FancyLabel("Touch Screen", 0.4f);
      touchLabel.AutoBreakLine = true;
      touchLabel.TextColor = Color.Yellow;
      AddChild(touchLabel);
      var touchText = new FancyLabel("Touch screen feature is provided by TouchScreenAPI mod (Steam # 2668820525).\nWhich makes both cursor and UI elements available for any modder.\nSend me a direct message on Steam or Discord.", 0.5f);
      touchText.AutoBreakLine = true;
      touchText.Margin = new Vector4(0, 0, 0, 10);
      AddChild(touchText);

      var creditsLabel = new FancyLabel("Developed by Adriano Lima#1786", 0.7f);
      creditsLabel.AutoBreakLine = true;
      creditsLabel.TextColor = Color.Yellow;
      AddChild(creditsLabel);
    }
  }
}