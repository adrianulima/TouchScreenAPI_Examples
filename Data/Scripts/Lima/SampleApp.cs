using System;
using System.Collections.Generic;
using VRageMath;
using Lima.API;

namespace Lima
{
  public class SampleApp : FancyApp
  {
    public SampleApp()
    {
    }

    public void CreateElements()
    {
      var window = new FancyView(FancyView.ViewDirection.Row);
      var windowBar = new FancyWindowBar("Sample App");

      var col1 = new FancyView();
      col1.SetScale(new Vector2(0.5f, 1));
      var col2 = new FancyView();
      col2.SetPosition(new Vector2(0.5f, 0));
      col2.SetScale(new Vector2(0.5f, 1));

      var header1 = new FancyLabel("Column 1", 0.6f);
      var header2 = new FancyLabel("Column 2", 0.6f);

      var separator1 = new FancySeparator();
      var separator2 = new FancySeparator();

      var labelSlider = new FancyLabel("A Fancy Slider");
      var slider = new FancySlider(-100, 100, (float v) =>
      {
        Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{v}", "SampleApp");
      });
      slider.SetValue(50f);
      var labelSliderRange = new FancyLabel("Range Slider");
      var sliderRange = new FancySliderRange(-100, 100, (float v, float v2) =>
      {
        Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{v}, {v2}", "SampleApp");
      });
      sliderRange.SetValue(50f);
      sliderRange.SetValueLower(-50f);

      var labelProgressBar = new FancyLabel("Progress Bar");
      labelProgressBar.SetAlignment(VRage.Game.GUI.TextPanel.TextAlignment.LEFT);
      var progressBar = new FancyProgressBar(-100, 100);

      var labelSelector = new FancyLabel("Color");
      var list = new List<Color> { Color.MediumSeaGreen, Color.Magenta, Color.Maroon, Color.MediumAquamarine, Color.MediumBlue, Color.MediumOrchid, Color.MediumPurple, Color.MediumSlateBlue, Color.MintCream, Color.MediumTurquoise, Color.MediumVioletRed, Color.MidnightBlue, Color.Linen, Color.MistyRose, Color.Moccasin, Color.NavajoWhite, Color.Navy, Color.MediumSpringGreen, Color.LimeGreen, Color.LightSkyBlue, Color.LightYellow, Color.Ivory, Color.Khaki, Color.Lavender, Color.LavenderBlush, Color.LawnGreen, Color.LemonChiffon, Color.LightBlue, Color.LightCoral, Color.Lime, Color.LightCyan, Color.LightGreen, Color.LightGray, Color.LightPink, Color.LightSalmon, Color.LightSeaGreen, Color.OldLace, Color.LightSlateGray, Color.LightSteelBlue, Color.LightGoldenrodYellow, Color.Olive, Color.PaleGoldenrod, Color.Orange, Color.Silver, Color.SkyBlue, Color.SlateBlue, Color.SlateGray, Color.Snow, Color.SpringGreen, Color.SteelBlue, Color.Tan, Color.Sienna, Color.Teal, Color.Tomato, Color.Turquoise, Color.Violet, Color.Wheat, Color.White, Color.WhiteSmoke, Color.Yellow, Color.YellowGreen, Color.Thistle, Color.OliveDrab, Color.SeaShell, Color.SandyBrown, Color.OrangeRed, Color.Orchid, Color.Indigo, Color.PaleGreen, Color.PaleTurquoise, Color.PaleVioletRed, Color.PapayaWhip, Color.PeachPuff, Color.SeaGreen, Color.Peru, Color.Plum, Color.PowderBlue, Color.Red, Color.RosyBrown, Color.RoyalBlue, Color.SaddleBrown, Color.Salmon, Color.Pink, Color.IndianRed, Color.HotPink, Color.Honeydew, Color.DarkGoldenrod, Color.DarkCyan, Color.DarkBlue, Color.Cyan, Color.Crimson, Color.Cornsilk, Color.CornflowerBlue, Color.Coral, Color.Chocolate, Color.Chartreuse, Color.CadetBlue, Color.DarkGray, Color.BurlyWood, Color.BlueViolet, Color.Blue, Color.BlanchedAlmond, Color.Bisque, Color.Beige, Color.Azure, Color.Aquamarine, Color.Aqua, Color.AntiqueWhite, Color.AliceBlue, Color.Brown, Color.DarkGreen, Color.DarkMagenta, Color.DarkKhaki, Color.GreenYellow, Color.Green, Color.Gray, Color.Goldenrod, Color.Gold, Color.GhostWhite, Color.Gainsboro, Color.Fuchsia, Color.ForestGreen, Color.FloralWhite, Color.Firebrick, Color.DodgerBlue, Color.DeepSkyBlue, Color.DeepPink, Color.DarkViolet, Color.DarkTurquoise, Color.DarkSlateGray, Color.DarkSlateBlue, Color.DarkSeaGreen, Color.DarkSalmon, Color.DarkRed, Color.DarkOrchid, Color.DarkOrange, Color.DarkOliveGreen, Color.DimGray };
      var names = new List<String> { "MediumSeaGreen", "Magenta", "Maroon", "MediumAquamarine", "MediumBlue", "MediumOrchid", "MediumPurple", "MediumSlateBlue", "MintCream", "MediumTurquoise", "MediumVioletRed", "MidnightBlue", "Linen", "MistyRose", "Moccasin", "NavajoWhite", "Navy", "MediumSpringGreen", "LimeGreen", "LightSkyBlue", "LightYellow", "Ivory", "Khaki", "Lavender", "LavenderBlush", "LawnGreen", "LemonChiffon", "LightBlue", "LightCoral", "Lime", "LightCyan", "LightGreen", "LightGray", "LightPink", "LightSalmon", "LightSeaGreen", "OldLace", "LightSlateGray", "LightSteelBlue", "LightGoldenrodYellow", "Olive", "PaleGoldenrod", "Orange", "Silver", "SkyBlue", "SlateBlue", "SlateGray", "Snow", "SpringGreen", "SteelBlue", "Tan", "Sienna", "Teal", "Tomato", "Turquoise", "Violet", "Wheat", "White", "WhiteSmoke", "Yellow", "YellowGreen", "Thistle", "OliveDrab", "SeaShell", "SandyBrown", "OrangeRed", "Orchid", "Indigo", "PaleGreen", "PaleTurquoise", "PaleVioletRed", "PapayaWhip", "PeachPuff", "SeaGreen", "Peru", "Plum", "PowderBlue", "Red", "RosyBrown", "RoyalBlue", "SaddleBrown", "Salmon", "Pink", "IndianRed", "HotPink", "Honeydew", "DarkGoldenrod", "DarkCyan", "DarkBlue", "Cyan", "Crimson", "Cornsilk", "CornflowerBlue", "Coral", "Chocolate", "Chartreuse", "CadetBlue", "DarkGray", "BurlyWood", "BlueViolet", "Blue", "BlanchedAlmond", "Bisque", "Beige", "Azure", "Aquamarine", "Aqua", "AntiqueWhite", "AliceBlue", "Brown", "DarkGreen", "DarkMagenta", "DarkKhaki", "GreenYellow", "Green", "Gray", "Goldenrod", "Gold", "GhostWhite", "Gainsboro", "Fuchsia", "ForestGreen", "FloralWhite", "Firebrick", "DodgerBlue", "DeepSkyBlue", "DeepPink", "DarkViolet", "DarkTurquoise", "DarkSlateGray", "DarkSlateBlue", "DarkSeaGreen", "DarkSalmon", "DarkRed", "DarkOrchid", "DarkOrange", "DarkOliveGreen", "DimGray" };
      var selector = new FancySelector(names, (int i, string color) =>
      {
        this.GetScreen().GetSurface().ScriptForegroundColor = list[i];
      });
      Random random = new Random();
      this.GetScreen().GetSurface().ScriptForegroundColor = list[random.Next(0, list.Count - 1)];

      var labelSwitch = new FancyLabel("Toggle: Off");
      labelSwitch.SetAlignment(VRage.Game.GUI.TextPanel.TextAlignment.RIGHT);
      var switcher = new FancySwitch((bool v) =>
      {
        if (v)
          labelSwitch.SetText("Toggle: On");
        else
          labelSwitch.SetText("Toggle: Off");
      });
      var button = new FancyButton("A Fancy Button", () =>
      {
        Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage("Test", "SampleApp");
      });

      var labelTextField = new FancyLabel("Text Field");
      var textField = new FancyTextField("", (string text) =>
      {
        Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage(text, "SampleApp");
      });

      col1.AddChild(header1);
      col1.AddChild(separator1);
      col1.AddChild(labelSlider);
      col1.AddChild(slider);
      col1.AddChild(labelProgressBar);
      col1.AddChild(progressBar);
      col1.AddChild(labelSwitch);
      col1.AddChild(switcher);
      col1.AddChild(button);
      col2.AddChild(header2);
      col2.AddChild(separator2);
      col2.AddChild(labelSliderRange);
      col2.AddChild(sliderRange);
      col2.AddChild(labelSelector);
      col2.AddChild(selector);
      col2.AddChild(labelTextField);
      col2.AddChild(textField);
      window.AddChild(col1);
      window.AddChild(col2);
      AddChild(windowBar);
      AddChild(window);
    }
  }
}