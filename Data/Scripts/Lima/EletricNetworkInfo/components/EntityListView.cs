using System.Collections.Generic;
using VRageMath;
using Lima.API;
using System.Linq;
using VRage.Game.GUI.TextPanel;

namespace Lima2
{
  public class EntityListView : FancyView
  {
    private int _odd = 0;

    private FancyScrollView _scrollView;
    private List<FancyView> _views = new List<FancyView>();

    public string Title;
    private int _cols;

    public EntityListView(string title, int cols = 2) : base(FancyView.ViewDirection.Column)
    {
      Title = title;
      _cols = cols;

      CreateElements();
    }

    public void SetScrollViewBgColor(Color color)
    {
      _scrollView.SetBgColor(color);
    }

    private void CreateElements()
    {
      var titleLabel = new FancyLabel(Title, 0.4f, TextAlignment.LEFT);
      titleLabel.SetAlignment(TextAlignment.CENTER);
      AddChild(titleLabel);

      _scrollView = new FancyScrollView(FancyView.ViewDirection.Column);
      _scrollView.SetPadding(new Vector4(2, 2, 2, 0));
      _scrollView.SetGap(2);
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
        var children = v.GetChildren();
        foreach (var ch in children)
          v.RemoveChild(ch);
        _scrollView.RemoveChild(v);
      }

      _views.Clear();

      _odd = 0;
    }

    public void FillLastView()
    {
      var view = _views.Last<FancyView>();
      var childCount = view.GetChildren().Count;
      if (childCount < _cols)
      {
        var fill = new FancyView();
        view.AddChild(fill);
        fill.SetScale(new Vector2(_cols - childCount, 0));
      }
    }

    public void AddItem(EntityItem item)
    {
      if (_odd % _cols != 0)
      {
        var view = _views.Last<FancyView>();
        view.AddChild(item);
      }
      else
      {
        var view = new FancyView(ViewDirection.Row);
        view.SetGap(2);
        view.AddChild(item);
        view.SetScale(new Vector2(1, 0));
        view.SetPixels(new Vector2(0, 34));
        _scrollView.AddChild(view);
        _views.Add(view);
      }
      _odd++;
    }
  }
}