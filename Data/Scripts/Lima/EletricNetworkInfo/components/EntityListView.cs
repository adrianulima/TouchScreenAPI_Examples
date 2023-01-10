using System.Collections.Generic;
using VRageMath;
using Lima.API;
using System.Linq;
using VRage.Game.GUI.TextPanel;

namespace Lima
{
  public class EntityListView : TouchView
  {
    private int _odd = 0;

    private TouchScrollView _scrollView;
    private List<TouchView> _views = new List<TouchView>();

    public string Title;
    private int _cols;

    public EntityListView(string title, int cols = 2) : base(ViewDirection.Column)
    {
      Title = title;
      _cols = cols;

      CreateElements();
    }

    public void SetScrollViewBgColor(Color color)
    {
      _scrollView.BgColor = color;
    }

    private void CreateElements()
    {
      var titleLabel = new TouchLabel(Title, 0.4f, TextAlignment.LEFT);
      titleLabel.Alignment = TextAlignment.CENTER;
      AddChild(titleLabel);

      _scrollView = new TouchScrollView(ViewDirection.Column);
      _scrollView.Padding = new Vector4(2, 2, 2, 0);
      _scrollView.Gap = 2;
      AddChild(_scrollView);
    }

    public void Dispose()
    {
      _views.Clear();
    }

    public void RemoveAllChildren()
    {
      foreach (var v in _views)
      {
        foreach (var ch in v.Children)
          v.RemoveChild(ch);
        _scrollView.RemoveChild(v);
      }

      _views.Clear();

      _odd = 0;
    }

    public void FillLastView()
    {
      if (_views.Count == 0) return;

      var view = _views.Last<TouchView>();
      var childCount = view.Children.Count;
      if (childCount < _cols)
      {
        var fill = new TouchView();
        view.AddChild(fill);
        fill.Scale = new Vector2(_cols - childCount, 0);
      }
    }

    public void AddItem(EntityItem item)
    {
      if (_odd % _cols != 0)
      {
        var view = _views.Last<TouchView>();
        view.AddChild(item);
      }
      else
      {
        var view = new TouchView(ViewDirection.Row);
        view.Gap = 2;
        view.AddChild(item);
        view.Scale = new Vector2(1, 0);
        view.Pixels = new Vector2(0, 34);
        _scrollView.AddChild(view);
        _views.Add(view);
      }
      _odd++;
    }
  }
}