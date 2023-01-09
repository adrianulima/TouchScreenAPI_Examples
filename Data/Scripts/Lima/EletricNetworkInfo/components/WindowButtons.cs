using System;
using Lima.API;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace Lima
{
  public class WindowButtons : FancyView
  {
    public FancyButton HelpBt;
    public FancyEmptyButton SettingsButton;
    public FancyEmptyButton LayoutButton;
    public FancyEmptyElement GearIcon;
    public FancyEmptyElement LayoutIcon;

    public int CurrentLayout = 0;

    public Action OnChangeConfig;
    public Action<string> OnChangePage;

    public WindowButtons(Action onChangeConfig, Action<string> onChangePage)
    {
      OnChangeConfig = onChangeConfig;
      OnChangePage = onChangePage;

      Direction = ViewDirection.RowReverse;
      Anchor = ViewAnchor.End;
      Alignment = ViewAlignment.Center;
      Margin = new Vector4(4, 0, 4, 0);
      Gap = 4;

      HelpBt = new FancyButton("?", OnClickHelp);
      HelpBt.Label.FontSize = 0.5f;
      HelpBt.Scale = Vector2.Zero;
      HelpBt.Pixels = new Vector2(16);
      AddChild(HelpBt);

      SettingsButton = new FancyEmptyButton(OnClickSettings);
      SettingsButton.Scale = Vector2.Zero;
      SettingsButton.Pixels = new Vector2(16);
      AddChild(SettingsButton);

      GearIcon = new FancyEmptyElement();
      SettingsButton.AddChild(GearIcon);

      LayoutButton = new FancyEmptyButton(OnClickLayout);
      LayoutButton.Scale = Vector2.Zero;
      LayoutButton.Pixels = new Vector2(16);
      AddChild(LayoutButton);

      LayoutIcon = new FancyEmptyElement();
      LayoutButton.AddChild(LayoutIcon);

      RegisterUpdate(Update);
    }

    private void OnClickHelp()
    {
      OnChangePage("help");
    }

    private void OnClickSettings()
    {
      OnChangePage("settings");
    }

    private void OnClickLayout()
    {
      CurrentLayout++;
      if (CurrentLayout >= 4)
        CurrentLayout = 0;

      OnChangeConfig();
    }

    private void Update()
    {
      // TODO: dirty?
      DrawLayoutIconSprites();
      DrawSettingsIconSprites();
    }

    private void DrawSettingsIconSprites()
    {
      var color = App.Theme.WhiteColor;

      if (SettingsButton.Handler.IsMousePressed)
        color = App.Theme.GetMainColorDarker(4);

      GearIcon.GetSprites().Clear();
      var gear = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = "Textures\\FactionLogo\\Builders\\BuilderIcon_7.dds",
        // Data = "Textures\\FactionLogo\\Others\\OtherIcon_22.dds",
        RotationOrScale = 0,
        Position = GearIcon.Position + new Vector2(0, 8) * App.Theme.Scale,
        Size = new Vector2(16, 16) * App.Theme.Scale,
        Color = color
      };

      GearIcon.GetSprites().Add(gear);
    }

    private void DrawLayoutIconSprites()
    {
      LayoutIcon.GetSprites().Clear();
      var square = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        RotationOrScale = 0,
        Color = App.Theme.GetMainColorDarker(2)
      };

      switch (CurrentLayout)
      {
        default:
        case 0:
          square.Position = LayoutIcon.Position + new Vector2(1, 8 - 3) * App.Theme.Scale;
          square.Size = new Vector2(14, 7) * App.Theme.Scale;
          break;
        case 1:
          square.Position = LayoutIcon.Position + new Vector2(1, 8) * App.Theme.Scale;
          square.Size = new Vector2(7, 14) * App.Theme.Scale;
          break;
        case 2:
          square.Position = LayoutIcon.Position + new Vector2(1, 8) * App.Theme.Scale;
          square.Size = new Vector2(14, 14) * App.Theme.Scale;
          break;
        case 3:
          square.Position = LayoutIcon.Position + new Vector2(1, 8) * App.Theme.Scale;
          square.Size = new Vector2(14, 14) * App.Theme.Scale;
          square.Color = new Color(App.Theme.GetMainColorDarker(2), 0.5f);
          break;
      }

      LayoutIcon.GetSprites().Add(square);
    }
  }
}