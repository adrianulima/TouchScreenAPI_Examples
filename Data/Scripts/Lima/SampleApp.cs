using System;
using System.Collections.Generic;
using VRageMath;
using Lima.API;
using VRage.Game.GUI.TextPanel;

namespace Lima
{
  public class SampleApp : FancyApp
  {
    public SampleApp()
    {
      DefaultBg = true;
    }

    public void CreateElements()
    {
      var window = new FancyView(FancyView.ViewDirection.Row);
      var windowBar = new FancyWindowBar("Sample App");

      // window.Border=new Vector4(4);
      // window.Padding=new Vector4(14);

      var col1 = new FancyScrollView();
      col1.Scale = new Vector2(1, 0.75f);

      col1.Margin = new Vector4(4);
      col1.Border = new Vector4(4);
      col1.BorderColor = Color.Red;
      col1.Padding = new Vector4(4);
      col1.ScrollAlwaysVisible = true;

      var col2 = new FancyView();
      // col2.Scale=new Vector2(1, 1);
      // col2.Border=new Vector4(2);

      var header1 = new FancyLabel("Column 1\nMultiline Test", 0.6f);

      var separator1 = new FancySeparator();

      var tabs = new FancySwitch(new string[] { "Tab 1", "Tab 2", "Tab 3", "Tab 4" });

      var labelSlider = new FancyLabel("A Fancy Slider");
      labelSlider.Margin = Vector4.UnitY * 8;
      var slider = new FancySlider(-100, 100, (float v) =>
      {
        // Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{v}", "SampleApp");
      });
      slider.Value = 50f;
      var labelSliderRange = new FancyLabel("Range Slider");
      labelSliderRange.Margin = Vector4.UnitY * 8;
      var sliderRange = new FancySliderRange(-100, 100, (float v, float v2) =>
      {
        Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{v}, {v2}", "SampleApp");
      });
      sliderRange.Value = 50f;
      sliderRange.ValueLower = -50f;

      var labelProgressBar = new FancyLabel("Progress Bar");
      labelProgressBar.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
      labelProgressBar.Margin = Vector4.UnitY * 8;
      var progressBar = new FancyProgressBar(0, 100);
      progressBar.Value = 50;
      progressBar.Label = "50%";
      progressBar.LabelAlignment = VRage.Game.GUI.TextPanel.TextAlignment.RIGHT;

      var labelSelector = new FancyLabel("Color");
      labelSelector.Margin = Vector4.UnitY * 8;
      var list = new List<Color> { Color.MediumSeaGreen, Color.Magenta, Color.Maroon, Color.MediumAquamarine, Color.MediumBlue, Color.MediumOrchid, Color.MediumPurple, Color.MediumSlateBlue, Color.MintCream, Color.MediumTurquoise, Color.MediumVioletRed, Color.MidnightBlue, Color.Linen, Color.MistyRose, Color.Moccasin, Color.NavajoWhite, Color.Navy, Color.MediumSpringGreen, Color.LimeGreen, Color.LightSkyBlue, Color.LightYellow, Color.Ivory, Color.Khaki, Color.Lavender, Color.LavenderBlush, Color.LawnGreen, Color.LemonChiffon, Color.LightBlue, Color.LightCoral, Color.Lime, Color.LightCyan, Color.LightGreen, Color.LightGray, Color.LightPink, Color.LightSalmon, Color.LightSeaGreen, Color.OldLace, Color.LightSlateGray, Color.LightSteelBlue, Color.LightGoldenrodYellow, Color.Olive, Color.PaleGoldenrod, Color.Orange, Color.Silver, Color.SkyBlue, Color.SlateBlue, Color.SlateGray, Color.Snow, Color.SpringGreen, Color.SteelBlue, Color.Tan, Color.Sienna, Color.Teal, Color.Tomato, Color.Turquoise, Color.Violet, Color.Wheat, Color.White, Color.WhiteSmoke, Color.Yellow, Color.YellowGreen, Color.Thistle, Color.OliveDrab, Color.SeaShell, Color.SandyBrown, Color.OrangeRed, Color.Orchid, Color.Indigo, Color.PaleGreen, Color.PaleTurquoise, Color.PaleVioletRed, Color.PapayaWhip, Color.PeachPuff, Color.SeaGreen, Color.Peru, Color.Plum, Color.PowderBlue, Color.Red, Color.RosyBrown, Color.RoyalBlue, Color.SaddleBrown, Color.Salmon, Color.Pink, Color.IndianRed, Color.HotPink, Color.Honeydew, Color.DarkGoldenrod, Color.DarkCyan, Color.DarkBlue, Color.Cyan, Color.Crimson, Color.Cornsilk, Color.CornflowerBlue, Color.Coral, Color.Chocolate, Color.Chartreuse, Color.CadetBlue, Color.DarkGray, Color.BurlyWood, Color.BlueViolet, Color.Blue, Color.BlanchedAlmond, Color.Bisque, Color.Beige, Color.Azure, Color.Aquamarine, Color.Aqua, Color.AntiqueWhite, Color.AliceBlue, Color.Brown, Color.DarkGreen, Color.DarkMagenta, Color.DarkKhaki, Color.GreenYellow, Color.Green, Color.Gray, Color.Goldenrod, Color.Gold, Color.GhostWhite, Color.Gainsboro, Color.Fuchsia, Color.ForestGreen, Color.FloralWhite, Color.Firebrick, Color.DodgerBlue, Color.DeepSkyBlue, Color.DeepPink, Color.DarkViolet, Color.DarkTurquoise, Color.DarkSlateGray, Color.DarkSlateBlue, Color.DarkSeaGreen, Color.DarkSalmon, Color.DarkRed, Color.DarkOrchid, Color.DarkOrange, Color.DarkOliveGreen, Color.DimGray };
      var names = new List<String> { "MediumSeaGreen", "Magenta", "Maroon", "MediumAquamarine", "MediumBlue", "MediumOrchid", "MediumPurple", "MediumSlateBlue", "MintCream", "MediumTurquoise", "MediumVioletRed", "MidnightBlue", "Linen", "MistyRose", "Moccasin", "NavajoWhite", "Navy", "MediumSpringGreen", "LimeGreen", "LightSkyBlue", "LightYellow", "Ivory", "Khaki", "Lavender", "LavenderBlush", "LawnGreen", "LemonChiffon", "LightBlue", "LightCoral", "Lime", "LightCyan", "LightGreen", "LightGray", "LightPink", "LightSalmon", "LightSeaGreen", "OldLace", "LightSlateGray", "LightSteelBlue", "LightGoldenrodYellow", "Olive", "PaleGoldenrod", "Orange", "Silver", "SkyBlue", "SlateBlue", "SlateGray", "Snow", "SpringGreen", "SteelBlue", "Tan", "Sienna", "Teal", "Tomato", "Turquoise", "Violet", "Wheat", "White", "WhiteSmoke", "Yellow", "YellowGreen", "Thistle", "OliveDrab", "SeaShell", "SandyBrown", "OrangeRed", "Orchid", "Indigo", "PaleGreen", "PaleTurquoise", "PaleVioletRed", "PapayaWhip", "PeachPuff", "SeaGreen", "Peru", "Plum", "PowderBlue", "Red", "RosyBrown", "RoyalBlue", "SaddleBrown", "Salmon", "Pink", "IndianRed", "HotPink", "Honeydew", "DarkGoldenrod", "DarkCyan", "DarkBlue", "Cyan", "Crimson", "Cornsilk", "CornflowerBlue", "Coral", "Chocolate", "Chartreuse", "CadetBlue", "DarkGray", "BurlyWood", "BlueViolet", "Blue", "BlanchedAlmond", "Bisque", "Beige", "Azure", "Aquamarine", "Aqua", "AntiqueWhite", "AliceBlue", "Brown", "DarkGreen", "DarkMagenta", "DarkKhaki", "GreenYellow", "Green", "Gray", "Goldenrod", "Gold", "GhostWhite", "Gainsboro", "Fuchsia", "ForestGreen", "FloralWhite", "Firebrick", "DodgerBlue", "DeepSkyBlue", "DeepPink", "DarkViolet", "DarkTurquoise", "DarkSlateGray", "DarkSlateBlue", "DarkSeaGreen", "DarkSalmon", "DarkRed", "DarkOrchid", "DarkOrange", "DarkOliveGreen", "DimGray" };
      var selector = new FancySelector(names, (int i, string color) =>
      {
        this.Screen.Surface.ScriptForegroundColor = list[i];
      });
      // Random random = new Random();
      // this.GetScreen().GetSurface().ScriptForegroundColor = list[random.Next(0, list.Count - 1)];

      var labelSwitch = new FancyLabel("Toggle: Off");
      labelSwitch.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.RIGHT;
      labelSwitch.Margin = Vector4.UnitY * 8;
      var switcher = new FancySwitch(new string[] { "Off", "On" }, 0, (int v) =>
      {
        if (v == 1)
          labelSwitch.Text = "Toggle: On";
        else
          labelSwitch.Text = "Toggle: Off";
      });
      // switcher.Margin=new Vector4(8, 0, 8, 0);

      var button = new FancyButton("A Fancy Button", () =>
      {
        Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{col1.GetFlexSize().Y}", "SampleApp");
        // Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{window.GetPosition().X}+{window.GetSize().X}={window.GetPosition().X + window.GetSize().X}", "SampleApp");
        // Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{col1.GetPosition().X}+{col1.GetSize().X}={col2.GetPosition().X + col2.GetSize().X}", "SampleApp");
        // Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage("Test", "SampleApp");
      });

      button.Pixels = new Vector2(0, 42);
      button.Margin = Vector4.UnitY * 22;

      var labelTextField = new FancyLabel("Text Field");
      labelTextField.Margin = Vector4.UnitY * 8;
      var textField = new FancyTextField("", (string text) =>
      {
        Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage(text, "SampleApp");
      });

      var checkboxView = new FancyView(ViewDirection.Row);
      checkboxView.Padding = new Vector4(4);
      var checkboxLabel = new FancyLabel("Checkbox", 0.5f, TextAlignment.RIGHT);
      checkboxLabel.Margin = new Vector4(4);
      var checkbox = new FancyCheckbox((bool v) =>
      {
        Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{v}", "SampleApp");
      });
      checkboxView.AddChild(checkboxLabel);
      checkboxView.AddChild(checkbox);


      col1.AddChild(header1);
      col1.AddChild(separator1);
      col1.AddChild(labelSlider);
      col1.AddChild(slider);
      col1.AddChild(labelSliderRange);
      col1.AddChild(sliderRange);
      col1.AddChild(labelProgressBar);
      col1.AddChild(progressBar);
      col1.AddChild(labelSwitch);
      col1.AddChild(switcher);
      col1.AddChild(labelSelector);
      col1.AddChild(selector);
      col2.AddChild(tabs);
      col2.AddChild(button);
      col2.AddChild(labelTextField);
      col2.AddChild(textField);
      col2.AddChild(checkboxView);
      window.AddChild(col1);
      window.AddChild(col2);
      AddChild(windowBar);
      AddChild(window);
    }
  }
}